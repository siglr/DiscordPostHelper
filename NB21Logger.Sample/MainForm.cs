using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Microsoft.Win32;
using NB21_logger;
using NB21Logger.Sample.Properties;

namespace NB21Logger.Sample;

public partial class MainForm : Form
{
    private readonly bool launchedViaStartup;
    private readonly NB21_logger.NB21Logger logger;
    private readonly LoggerConfiguration configuration;
    private readonly NB21_logger.Properties.Settings settings = NB21_logger.Properties.Settings.Default;
    private readonly Timer blinkingTimer;
    private readonly Timer uiRefreshTimer;
    private NB21LoggerState state;
    private bool recordingTickTock;
    private bool compactView;
    private string? finalizedIgcPath;
    private readonly Image? connectedImage = Resources.recording_tock;
    private readonly Image? recordingTickImage = Resources.recording_tick;
    private readonly Image? recordingTockImage = Resources.recording_tock;
    private readonly Image? notConnectedImage = Resources.recording_tick;
    private string localIpAddress = string.Empty;
    private int connectedClientCount;

    public MainForm(bool launchedViaStartup)
    {
        this.launchedViaStartup = launchedViaStartup;
        var initialConfig = BuildConfigurationFromSettings();
        logger = new NB21_logger.NB21Logger(initialConfig);
        configuration = logger.Configuration;
        logger.StateChanged += Logger_StateChanged;
        logger.IgcFileFinalized += Logger_IgcFileFinalized;
        logger.WebSocketClientConnected += Logger_WebSocketClientConnected;
        logger.WebSocketClientDisconnected += Logger_WebSocketClientDisconnected;
        logger.AppEvent += Logger_AppEvent;

        blinkingTimer = new Timer { Interval = 1000 };
        blinkingTimer.Tick += BlinkingTimer_Tick;

        uiRefreshTimer = new Timer { Interval = 1000 };
        uiRefreshTimer.Tick += UiRefreshTimer_Tick;

        state = logger.State;

        InitializeComponent();

        Text = $"{settings.AppName} {settings.AppVersion}";
        Icon = Resources.app_icon;
    }

    private LoggerConfiguration BuildConfigurationFromSettings()
    {
        var config = new LoggerConfiguration
        {
            PilotName = settings.PilotName,
            PilotId = settings.PilotId,
            AppName = settings.AppName,
            AppVersion = settings.AppVersion
        };

        if (!string.IsNullOrWhiteSpace(settings.IGCPath))
        {
            config.TracklogsDirectory = settings.IGCPath;
        }

        return config;
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        pictureBox_statusImage.Image = notConnectedImage;
        ui_conn_status.Text = "Waiting for MSFS.";
        ui_recording_time.Visible = false;
        ui_sim_rate.Visible = false;

        LoadSettingsIntoUi();
        ApplyAutoStartSetting();
        EnsureTracklogsFolder();

        localIpAddress = GetLocalIPAddress();
        UpdateWebUrls(false);
        ui_message_bar.Text = "Drop a .PLN or click Task to load.";
        ui_task.Text = "Drop a .PLN or click Task";

        blinkingTimer.Start();
        uiRefreshTimer.Start();

        logger.Start();

        if (settings.WindowsStart && launchedViaStartup)
        {
            WindowState = FormWindowState.Minimized;
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        blinkingTimer.Stop();
        uiRefreshTimer.Stop();
        SaveSettings();
        logger.Stop();
        logger.Dispose();
    }

    private void LoadSettingsIntoUi()
    {
        ui_settings_pilot_name.Text = settings.PilotName;
        ui_settings_pilot_id.Text = settings.PilotId;
        ui_settings_igc_path.Text = settings.IGCPath;
        ui_auto_start_checkbox.Checked = settings.WindowsStart;
        ui_pilot.Text = settings.PilotId;
        view_tracklogs_button.Visible = !string.IsNullOrWhiteSpace(settings.IGCPath);
    }

    private void SaveSettings()
    {
        settings.PilotName = ui_settings_pilot_name.Text.Trim();
        settings.PilotId = ui_settings_pilot_id.Text.Trim();
        settings.IGCPath = ui_settings_igc_path.Text.Trim();
        settings.WindowsStart = ui_auto_start_checkbox.Checked;
        settings.Save();
        configuration.PilotName = settings.PilotName;
        configuration.PilotId = settings.PilotId;
        if (!string.IsNullOrWhiteSpace(settings.IGCPath))
        {
            configuration.TracklogsDirectory = settings.IGCPath;
        }
        ApplyAutoStartSetting();
    }

    private void EnsureTracklogsFolder()
    {
        if (string.IsNullOrWhiteSpace(settings.IGCPath))
        {
            settings.IGCPath = configuration.TracklogsDirectory;
            ui_settings_igc_path.Text = settings.IGCPath;
            settings.Save();
        }

        if (!Directory.Exists(settings.IGCPath))
        {
            Directory.CreateDirectory(settings.IGCPath);
        }

        view_tracklogs_button.Visible = Directory.Exists(settings.IGCPath);
    }

    private void ApplyAutoStartSetting()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
            if (key == null)
            {
                return;
            }

            if (settings.WindowsStart)
            {
                key.SetValue(settings.AppName, Application.ExecutablePath + " %startup");
            }
            else
            {
                key.DeleteValue(settings.AppName, false);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to update auto-start setting: " + ex);
        }
    }

    private void Logger_StateChanged(object? sender, LoggerStateChangedEventArgs e)
    {
        state = e.State;
        if (IsDisposed)
        {
            return;
        }

        BeginInvoke(new Action(() => ApplyState(e.EventType, e.State)));
    }

    private void Logger_IgcFileFinalized(object? sender, IgcFileFinalizedEventArgs e)
    {
        finalizedIgcPath = e.FilePath;
        if (IsDisposed)
        {
            return;
        }

        BeginInvoke(new Action(() =>
        {
            ui_message_bar.Text = $"IGC saved: {Path.GetFileName(e.FilePath)}";
            ui_file_icon.Visible = true;
            view_tracklogs_button.Visible = true;
        }));
    }

    private void Logger_WebSocketClientConnected(object? sender, WebSocketClientEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        BeginInvoke(new Action(() =>
        {
            connectedClientCount++;
            ui_message_bar.Text = e.IsJsonChannel ? "Task planner connected." : "IGC stream connected.";
            UpdateWebUrls(connectedClientCount > 0);
        }));
    }

    private void Logger_WebSocketClientDisconnected(object? sender, WebSocketClientEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        BeginInvoke(new Action(() =>
        {
            ui_message_bar.Text = e.IsJsonChannel ? "Task planner disconnected." : "IGC stream disconnected.";
            connectedClientCount = Math.Max(connectedClientCount - 1, 0);
            UpdateWebUrls(connectedClientCount > 0);
        }));
    }

    private void Logger_AppEvent(object? sender, AppEventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        string message = e.eventType switch
        {
            APPEVENT_ID.SimOpen => "Connected to MSFS.",
            APPEVENT_ID.SimQuit => "Simulator closed.",
            APPEVENT_ID.RecordingStart => "Recording started.",
            APPEVENT_ID.RecordingStop => "Recording stopped.",
            APPEVENT_ID.IGCFileWrite => "IGC file finalized.",
            APPEVENT_ID.SimHasCrashed => "Simulator crash detected.",
            APPEVENT_ID.HttpError => "HTTP error.",
            _ => e.eventType.ToString()
        };

        BeginInvoke(new Action(() => ui_message_bar.Text = message));
    }

    private void ApplyState(APPEVENT_ID eventType, NB21LoggerState newState)
    {
        ui_conn_status.Text = newState.ConnectionStatus;
        ui_pilot.Text = string.IsNullOrWhiteSpace(settings.PilotId) ? "Pilot?" : settings.PilotId;
        ui_aircraft.Text = newState.AircraftTitle ?? string.Empty;
        ui_task.Text = newState.FlightPlanName ?? "Drop a .PLN or click Task";
        ui_local_time.Text = newState.SimTimeUtc?.ToString("HH:mm:ss") ?? string.Empty;
        ui_local_date.Text = newState.SimTimeUtc?.ToString("yyyy-MM-dd") ?? string.Empty;
        ui_recording_time.Visible = newState.IsRecording;
        ui_sim_rate.Visible = newState.IsRecording;
        if (!newState.IsRecording)
        {
            ui_recording_time.Text = "00:00:00";
            ui_sim_rate.Text = string.Empty;
        }

        if (!string.IsNullOrEmpty(newState.LastIgcFilePath))
        {
            finalizedIgcPath = newState.LastIgcFilePath;
            ui_file_icon.Visible = true;
        }
        else if (!newState.IsRecording)
        {
            ui_file_icon.Visible = false;
        }

        if (!newState.IsConnected)
        {
            pictureBox_statusImage.Image = notConnectedImage;
        }
        else if (!newState.IsRecording)
        {
            pictureBox_statusImage.Image = connectedImage;
        }

        if (eventType == APPEVENT_ID.HttpError)
        {
            ui_message_bar.Text = "HTTP error";
        }
        else if (eventType == APPEVENT_ID.SimHasCrashed)
        {
            ui_message_bar.Text = "Simulator crashed";
        }
    }

    private void BlinkingTimer_Tick(object? sender, EventArgs e)
    {
        if (state.IsRecording)
        {
            recordingTickTock = !recordingTickTock;
            pictureBox_statusImage.Image = recordingTickTock ? recordingTickImage : recordingTockImage;
            ui_recording_time.Text = state.RecordingElapsed.ToString("hh\\:mm\\:ss");
            ui_sim_rate.Text = $"x{logger.simdata.flight_data_model.sim_rate:0.00}";
        }
        else if (state.IsConnected)
        {
            pictureBox_statusImage.Image = connectedImage;
        }
        else
        {
            pictureBox_statusImage.Image = notConnectedImage;
        }
    }

    private void UiRefreshTimer_Tick(object? sender, EventArgs e)
    {
        if (state.SimTimeUtc.HasValue)
        {
            ui_local_time.Text = state.SimTimeUtc.Value.ToString("HH:mm:ss");
            ui_local_date.Text = state.SimTimeUtc.Value.ToString("yyyy-MM-dd");
        }
    }

    private void ui_tab_click(object? sender, EventArgs e)
    {
        ui_message_bar.Text = tabControl1.SelectedTab == tabPage_Settings
            ? "Update pilot info and folders here."
            : "Drop a .PLN on the Task area.";
    }

    private void ui_home_select(object? sender, EventArgs e)
    {
        ui_message_bar.Text = "Drop a .PLN or click Task to load.";
    }

    private void ui_home_dragover(object? sender, DragEventArgs e)
    {
        if (HasPlnFile(e.Data))
        {
            e.Effect = DragDropEffects.Copy;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void ui_task_DragEnter(object? sender, DragEventArgs e)
    {
        ui_home_dragover(sender, e);
    }

    private void ui_task_DragDrop(object? sender, DragEventArgs e)
    {
        if (!HasPlnFile(e.Data))
        {
            return;
        }

        var filePath = ((string[])e.Data!.GetData(DataFormats.FileDrop)!).First();
        LoadFlightPlan(filePath);
    }

    private void ui_task_click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Flight Plans (*.pln)|*.pln",
            Title = "Select Flight Plan"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadFlightPlan(dialog.FileName);
        }
    }

    private void ui_file_drag(object? sender, MouseEventArgs e)
    {
        if (string.IsNullOrEmpty(finalizedIgcPath) || !File.Exists(finalizedIgcPath))
        {
            return;
        }

        DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { finalizedIgcPath }), DragDropEffects.Copy);
    }

    private void ui_view_tracklogs(object? sender, EventArgs e)
    {
        EnsureTracklogsFolder();
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = settings.IGCPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Unable to open tracklogs folder: " + ex.Message, "NB21 Logger", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ui_min_max_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (compactView)
        {
            Height = 275;
            Width = 610;
            compactView = false;
            ui_min_max.Text = "Compact";
        }
        else
        {
            Height = 112;
            Width = 525;
            compactView = true;
            ui_min_max.Text = "Full";
        }
    }

    private void ui_web_urls_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var url = ui_web_urls.Text.Split(' ').FirstOrDefault(part => part.StartsWith("http", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Unable to open web status: " + ex.Message, "NB21 Logger", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ui_browse_tracklogs(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            SelectedPath = settings.IGCPath,
            Description = "Select tracklogs folder"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            ui_settings_igc_path.Text = dialog.SelectedPath;
            configuration.TracklogsDirectory = dialog.SelectedPath;
            settings.IGCPath = dialog.SelectedPath;
            settings.Save();
            EnsureTracklogsFolder();
            view_tracklogs_button.Visible = true;
        }
    }

    private void ui_settings_pilot_name_Leave(object? sender, EventArgs e)
    {
        settings.PilotName = ui_settings_pilot_name.Text.Trim();
        configuration.PilotName = settings.PilotName;
        settings.Save();
    }

    private void ui_settings_pilot_id_Leave(object? sender, EventArgs e)
    {
        settings.PilotId = ui_settings_pilot_id.Text.Trim();
        configuration.PilotId = settings.PilotId;
        ui_pilot.Text = settings.PilotId;
        settings.Save();
    }

    private void ui_settings_igc_path_Leave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ui_settings_igc_path.Text))
        {
            return;
        }

        configuration.TracklogsDirectory = ui_settings_igc_path.Text.Trim();
        settings.IGCPath = configuration.TracklogsDirectory;
        settings.Save();
        EnsureTracklogsFolder();
        view_tracklogs_button.Visible = true;
    }

    private void ui_auto_start_checkbox_CheckedChanged(object? sender, EventArgs e)
    {
        settings.WindowsStart = ui_auto_start_checkbox.Checked;
        settings.Save();
        ApplyAutoStartSetting();
    }

    private static bool HasPlnFile(IDataObject? data)
    {
        if (data == null || !data.GetDataPresent(DataFormats.FileDrop))
        {
            return false;
        }

        var files = (string[])data.GetData(DataFormats.FileDrop)!;
        return files.Any(file => string.Equals(Path.GetExtension(file), ".pln", StringComparison.OrdinalIgnoreCase));
    }

    private void LoadFlightPlan(string filePath)
    {
        try
        {
            logger.SetFlightPlanFromFile(filePath);
            ui_task.Text = Path.GetFileName(filePath);
            ui_message_bar.Text = $"Loaded {Path.GetFileName(filePath)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Failed to load flight plan: " + ex.Message, "NB21 Logger", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateWebUrls(bool hasClients)
    {
        var baseUrl = $"http://localhost:{configuration.WebServerPort}/b21_task_planner";
        if (!string.IsNullOrEmpty(localIpAddress) && !localIpAddress.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            ui_web_urls.Text = hasClients
                ? $"{baseUrl} [ip {localIpAddress}] - clients connected"
                : $"{baseUrl} [ip {localIpAddress}]";
        }
        else
        {
            ui_web_urls.Text = hasClients ? $"{baseUrl} - clients connected" : baseUrl;
        }
    }

    private static string GetLocalIPAddress()
    {
        try
        {
            foreach (var address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }
        }
        catch (SocketException)
        {
        }

        return string.Empty;
    }
}
