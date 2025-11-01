// Decompiled with JetBrains decompiler
// Type: NB21_logger.NB21Logger
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using Microsoft.Win32;
using NB21_logger.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

#nullable enable
namespace NB21_logger;

public class NB21Logger : UserControl
{
  public EventHandler<AppEventArgs> app_event_handler;
  private readonly Settings settings = Settings.Default;
  public string AppName;
  public string AppVersion;
  private readonly Image? connected_img = (Image) Resources.ResourceManager.GetObject("recording_tock");
  private readonly Image? recordingTick_img = (Image) Resources.ResourceManager.GetObject("recording_tick");
  private readonly Image? recordingTock_img = (Image) Resources.ResourceManager.GetObject("recording_tock");
  private readonly Image? notconnected_img = (Image) Resources.ResourceManager.GetObject("recording_tick");
  public NB21WSServer nb21_ws_server;
  public NB21WebServer nb21_web_server;
  public SimDataConn simdata;
  public IGCFileWriter igc_file_writer;
  private string? current_igc_file_path;
  private System.Windows.Forms.Timer watchdogTimer = new System.Windows.Forms.Timer();
  private System.Windows.Forms.Timer BlinkingTimer = new System.Windows.Forms.Timer();
  private bool recording_tick_tock;
  private const int WM_TIMECHANGE = 30;
  private bool compactView;
  private bool loggerFailed;
  private bool resetWhenRecording = true;
  private bool igc_file_write_pending;
  private bool LaunchedViaStartup;
  private string StartupRegEntry = Application.ExecutablePath.ToString() + " %startup";
  public NB21Logger.WsConnectDelegate ws_connected;
  public NB21Logger.WsConnectDelegate ws_disconnected;
  private Form? hostForm;
  private bool shutdownHandled;

  public NB21Logger()
    : this(false)
  {
  }

  public NB21Logger(bool LaunchedViaStartup)
  {
    this.InitializeComponent();
    Console.WriteLine($"NB21Logger constructor (with LaunchedViaStartup = {LaunchedViaStartup})");
    this.LaunchedViaStartup = LaunchedViaStartup;
    this.ws_connected = new NB21Logger.WsConnectDelegate(this.ui_web_urls_connected);
    this.ws_disconnected = new NB21Logger.WsConnectDelegate(this.ui_web_urls_disconnected);
    string localIpAddress = NB21Logger.GetLocalIPAddress();
    this.AppName = this.settings.AppName;
    this.AppVersion = this.settings.AppVersion;
    Console.WriteLine($"NB21 Logger {this.AppVersion} ip address: {localIpAddress}");
    this.app_event_handler += new EventHandler<AppEventArgs>(this.handleAppEvent);
    string str = "ws://localhost:" + 54179.ToString();
    this.nb21_ws_server = new NB21WSServer(this);
    this.nb21_ws_server.start(54179);
    List<string> web_addresses1 = new List<string>()
    {
      $"http://{localIpAddress}:{54178}/",
      $"http://localhost:{54178}/"
    };
    List<string> web_addresses2 = new List<string>()
    {
      $"http://localhost:{54178}/"
    };
    this.nb21_web_server = new NB21WebServer(this);
    this.ui_web_urls_disconnected();
    try
    {
      this.set_ui_web_urls($"{web_addresses1[1]}b21_task_planner [ip {localIpAddress} ]");
      this.nb21_web_server.start(web_addresses1);
      Console.WriteLine($"Administrator Mode so {web_addresses1[0]} and {web_addresses1[1]}.");
    }
    catch (HttpListenerException ex)
    {
      if (ex.ErrorCode == 5)
      {
        Console.WriteLine("Seems not Administrator Mode so data API localhost only.");
        this.set_ui_web_urls(web_addresses2[0] + "b21_task_planner");
        this.nb21_web_server.start(web_addresses2);
      }
      else
        Console.WriteLine($"Unexpected Http error starting Web Server. {ex}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Unexpected error starting Web Server. {ex}");
    }
    this.simdata = new SimDataConn(this);
    this.igc_file_writer = new IGCFileWriter(this.simdata);
    this.simdata.SetWindowHandle(this.Handle);
    Console.WriteLine($"{this.settings.AppName} version {this.settings.AppVersion} running.");
  }

  [Browsable(true)]
  [DefaultValue(false)]
  public bool LaunchViaStartup
  {
    get => this.LaunchedViaStartup;
    set => this.LaunchedViaStartup = value;
  }

  protected override void OnParentChanged(EventArgs e)
  {
    base.OnParentChanged(e);
    this.AttachToHostForm();
  }

  private void AttachToHostForm()
  {
    Form? form = this.FindForm();
    if (form == null || form == this.hostForm)
      return;
    this.DetachFromHostForm();
    this.hostForm = form;
    this.hostForm.Icon = Resources.app_icon;
    this.hostForm.Text = this.get_title();
    this.hostForm.FormClosing += new FormClosingEventHandler(this.Form1_Closing);
    this.hostForm.Resize += new EventHandler(this.Form1_Resize);
  }

  private void DetachFromHostForm()
  {
    if (this.hostForm == null)
      return;
    this.hostForm.FormClosing -= new FormClosingEventHandler(this.Form1_Closing);
    this.hostForm.Resize -= new EventHandler(this.Form1_Resize);
    this.hostForm = null;
  }

  private void SetHostWindowState(FormWindowState state)
  {
    if (this.hostForm == null)
      this.AttachToHostForm();
    if (this.hostForm == null)
      return;
    this.hostForm.WindowState = state;
  }

  private void upgrade_settings()
  {
    if (!this.settings.SettingsUpgradeRequired)
      return;
    try
    {
      Console.WriteLine("Settings upgrade requirement detected.");
      this.settings.Upgrade();
      this.settings.SettingsUpgradeRequired = false;
      this.settings.Save();
      Console.WriteLine("Settings upgrade completed.");
    }
    catch
    {
      Console.WriteLine("Settings copy from prior version failed.");
    }
  }

  private string get_title() => $"{this.settings.AppName} {this.settings.AppVersion}";

  private void set_ui_web_urls(string txt)
  {
    this.ui_web_urls.Text = txt;
    Size size = TextRenderer.MeasureText(txt, this.ui_web_urls.Font);
    this.ui_web_urls.ClientSize = new Size(size.Width + 5, size.Height + 1);
  }

  private void Form1_Load(object sender, EventArgs e)
  {
    this.AttachToHostForm();
    this.pictureBox_statusImage.Image = this.notconnected_img;
    this.ui_conn_status.Text = "Waiting for MSFS.";
    this.ui_recording_time.Visible = false;
    this.ui_sim_rate.Visible = false;
    this.upgrade_settings();
    this.load_settings();
    this.check_reg_entry();
    this.do_tracklogs_folder_check();
    this.BlinkingTimer.Interval = 1000;
    this.BlinkingTimer.Tick += new EventHandler(this.BlinkingTimer_Tick);
    this.watchdogTimer.Interval = 5000;
    this.watchdogTimer.Tick += new EventHandler(this.Watchdog);
    this.watchdogTimer.Start();
    if (this.settings.WindowsStart && this.LaunchedViaStartup)
      this.SetHostWindowState(FormWindowState.Minimized);
    else
      this.SetHostWindowState(FormWindowState.Normal);
  }

  private void load_settings()
  {
    Console.WriteLine(nameof (load_settings));
    if (this.settings.PilotId != null && this.settings.PilotId != "")
      this.ui_pilot.Text = this.settings.PilotId;
    if (this.settings.IGCPath != null && this.settings.IGCPath != "")
    {
      Console.WriteLine("load_settings() have IGCPath " + this.settings.IGCPath);
      this.view_tracklogs_button.Visible = true;
    }
    else
    {
      string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NB21_logger\\Flights";
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      this.settings.IGCPath = path;
      Console.WriteLine("load_settings() set default IGCPath " + this.settings.IGCPath);
    }
    this.ui_settings_pilot_name.Text = this.settings.PilotName;
    this.ui_settings_pilot_id.Text = this.settings.PilotId;
    this.ui_settings_igc_path.Text = this.settings.IGCPath;
    Console.WriteLine($"load_settings() WindowsStart {this.settings.WindowsStart}");
    this.ui_auto_start_checkbox.Checked = this.settings.WindowsStart;
  }

  private void save_settings()
  {
    string str1 = this.ui_settings_pilot_name.Text.Substring(0, Math.Min(20, this.ui_settings_pilot_name.Text.Length));
    if (str1 != "")
    {
      Console.WriteLine($"Saving Pilot Name: '{str1}'");
      this.settings.PilotName = str1;
    }
    string str2 = this.ui_settings_pilot_id.Text.Substring(0, Math.Min(6, this.ui_settings_pilot_id.Text.Length));
    if (str2 != "")
    {
      Console.WriteLine($"Saving Pilot Id: '{str2}'");
      this.settings.PilotId = str2;
      this.ui_pilot.Text = str2;
    }
    string text = this.ui_settings_igc_path.Text;
    if (text != "")
    {
      Console.WriteLine($"Saving IGC Path: '{text}'");
      this.settings.IGCPath = text;
      this.view_tracklogs_button.Visible = true;
    }
    Console.WriteLine($"Saving Windows Auto Start: '{this.ui_auto_start_checkbox.Checked}'");
    this.settings.WindowsStart = this.ui_auto_start_checkbox.Checked;
    this.settings.Save();
    this.SetAutoStartMethod();
  }

  private void check_reg_entry()
  {
    if (!this.settings.WindowsStart)
      return;
    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
    if (registryKey == null)
      return;
    object obj = registryKey.GetValue(this.settings.AppName);
    if (obj != null && !(obj.ToString() != this.StartupRegEntry))
      return;
    registryKey.SetValue(this.settings.AppName, (object) this.StartupRegEntry);
    registryKey.Close();
  }

  private void SetAutoStartMethod()
  {
    Console.WriteLine($"Setting up Windows for booting with logger: '{this.settings.WindowsStart}'");
    RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
    string[] valueNames = registryKey.GetValueNames();
    if (this.settings.WindowsStart && !((IEnumerable<string>) valueNames).Contains<string>(this.settings.AppName))
    {
      Console.WriteLine($"Adding to : 'CurrentUser\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run': '{this.settings.AppName}' '{Application.ExecutablePath.ToString()}'");
      registryKey.SetValue(this.settings.AppName, (object) (Application.ExecutablePath.ToString() + " startup"));
    }
    if (this.settings.WindowsStart || !((IEnumerable<string>) valueNames).Contains<string>(this.settings.AppName))
      return;
    Console.WriteLine($"Removing '{this.settings.AppName}' from 'CurrentUser\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run'");
    registryKey.DeleteValue(this.settings.AppName, false);
  }

  private void Watchdog(object? sender, EventArgs e)
  {
    if (this.loggerFailed)
      return;
    try
    {
      this.simdata.CheckConnect();
    }
    catch (FileNotFoundException ex)
    {
      if (ex.FileName != null && ex.FileName.Contains("SimConnect"))
        this.AppFailure("Missing SimConnect DLLs");
      else
        this.AppFailure("Failed loadng MSFS connect.");
    }
  }

  private void AppReset()
  {
    this.loggerFailed = false;
    this.igc_file_write_pending = false;
    this.simdata?.app_init();
  }

  private void AppFailure(string msg)
  {
    this.loggerFailed = true;
    this.ui_pilot_label.Visible = false;
    this.ui_pilot.Text = msg ?? "";
    this.ui_pilot.ForeColor = System.Drawing.Color.Red;
    this.ui_simtime_label.Visible = false;
    this.ui_local_date.Visible = false;
    this.ui_local_time.Visible = false;
    this.ui_aircraft_label.Visible = false;
    this.ui_aircraft.Visible = false;
    this.ui_task.Visible = false;
    Console.WriteLine(msg);
  }

  private void BlinkingTimer_Tick(object? sender, EventArgs e)
  {
    string str = "";
    if (this.simdata.isRecording)
    {
      TimeSpan elapsed = this.simdata.recording_timer.Elapsed;
      TimeSpan timeSpan = TimeSpan.FromSeconds(1.0);
      long num = elapsed.Ticks % timeSpan.Ticks;
      long ticks = num > timeSpan.Ticks / 2L ? timeSpan.Ticks : 0L;
      str = new TimeSpan(elapsed.Ticks + ticks - num).ToString("hh\\:mm\\:ss");
      if (this.resetWhenRecording && this.simdata.flight_data_model.get_length() > 60)
      {
        this.resetWhenRecording = false;
        this.ui_reset_igc();
      }
    }
    if (this.recording_tick_tock)
    {
      this.ui_recording_time.Text = $"[{str}]";
      this.pictureBox_statusImage.Image = this.recordingTick_img;
      this.recording_tick_tock = false;
    }
    else
    {
      this.ui_recording_time.Text = $"[{str}]";
      this.pictureBox_statusImage.Image = this.recordingTock_img;
      this.recording_tick_tock = true;
    }
  }

  private void handleAppEvent(object? sender, AppEventArgs e)
  {
    switch (e.eventType)
    {
      case APPEVENT_ID.SimOpen:
        Console.WriteLine("APPEVENT SimOpen " + e.arg_str);
        this.SetHostWindowState(FormWindowState.Normal);
        this.pictureBox_statusImage.Image = this.connected_img;
        this.ui_conn_status.Text = "Connected to " + this.simdata.get_msfs_text();
        this.AppReset();
        this.do_tracklogs_folder_check();
        break;
      case APPEVENT_ID.SimStop:
        Console.WriteLine("APPEVENT SimStop");
        break;
      case APPEVENT_ID.SimQuit:
        Console.WriteLine("APPEVENT SimExit");
        this.ui_conn_status.Text = "Waiting for MSFS.";
        this.pictureBox_statusImage.Image = this.notconnected_img;
        break;
      case APPEVENT_ID.SimHasCrashed:
        Console.WriteLine("APPEVENT SimHasCrashed");
        this.handle_sim_crash();
        break;
      case APPEVENT_ID.RecordingStart:
        Console.WriteLine("APPEVENT RecordingStart");
        this.recording_has_started();
        break;
      case APPEVENT_ID.RecordingStop:
        Console.WriteLine("APPEVENT RecordingStop");
        this.recording_has_stopped();
        break;
      case APPEVENT_ID.Slew:
        Console.WriteLine("APPEVENT Slew");
        break;
      case APPEVENT_ID.EngineOn:
        Console.WriteLine("APPEVENT EngineOn");
        break;
      case APPEVENT_ID.Takeoff:
        Console.WriteLine("APPEVENT Takeoff");
        break;
      case APPEVENT_ID.Landing:
        Console.WriteLine("APPEVENT Landing");
        break;
      case APPEVENT_ID.IGCFileWrite:
        Console.WriteLine("APPEVENT IGCFileWrite");
        this.igc_file_write();
        break;
      case APPEVENT_ID.IGCFileReset:
        Console.WriteLine("APPEVENT IGCFileReset");
        this.ui_reset_igc();
        break;
      case APPEVENT_ID.SimTimeChange:
        Console.WriteLine("APPEVENT SimTimeChange");
        break;
      case APPEVENT_ID.SimDateChange:
        Console.WriteLine("APPEVENT SimDateChange");
        this.handle_sim_date_change();
        break;
      case APPEVENT_ID.SimRateChange:
        Console.WriteLine($"APPEVENT SimRateChange {this.simdata.flight_data_model.sim_rate:000.000}");
        this.handle_sim_rate_change();
        break;
      case APPEVENT_ID.SimTimeTick:
        try
        {
          if (this.simdata.flight_data_model.sim_local_day == 0.0 || this.simdata.flight_data_model.sim_local_month == 0.0 || this.simdata.flight_data_model.sim_local_year == 0.0)
          {
            this.ui_local_date.Text = "??";
            this.ui_local_time.Text = "";
            break;
          }
          this.ui_local_time.Text = TimeSpan.FromSeconds(this.simdata.localtime_s).ToString("hh\\:mm\\:ss");
          break;
        }
        catch (Exception ex)
        {
          Console.WriteLine("Exception in handleAppEvent SimTimeTick: " + ex.Message);
          break;
        }
      case APPEVENT_ID.HeaderUpdate:
        Console.WriteLine("APPEVENT HeaderUpdate");
        this.handle_header_update();
        break;
      case APPEVENT_ID.IGCWriteError:
        Console.WriteLine("APPEVENT IGCWriteError");
        break;
      case APPEVENT_ID.Pause:
        Console.WriteLine("APPEVENT Pause");
        this.handle_pause();
        break;
      case APPEVENT_ID.Weight:
        Console.WriteLine($"APPEVENT weight {this.simdata.weight_kg:0}");
        break;
      case APPEVENT_ID.Pause_1:
        Console.WriteLine("APPEVENT Pause_1");
        break;
      case APPEVENT_ID.LoadAircraft:
        Console.WriteLine("APPEVENT LoadAircraft " + this.simdata.igc_aircraft.get_title());
        break;
      case APPEVENT_ID.PCClockChange:
        Console.WriteLine("APPEVENT PCClockChange UTC: " + DateTime.Now.ToString("U"));
        break;
      case APPEVENT_ID.KADetected:
        Console.WriteLine("APPEVENT KADetected");
        break;
      case APPEVENT_ID.WeatherChange:
        Console.WriteLine("APPEVENT WeatherChange");
        break;
      case APPEVENT_ID.PositionChanged:
        Console.WriteLine("APPEVENT PositionChanged");
        break;
      case APPEVENT_ID.HttpError:
        Console.WriteLine("APPEVENT HttpError");
        this.handle_http_error();
        break;
      default:
        Console.WriteLine("APPEVENT Unknown");
        break;
    }
  }

  private void handle_http_error() => this.ui_conn_status.Text = "HTTP Error";

  private void handle_header_update()
  {
    this.ui_aircraft.Text = this.simdata.igc_aircraft.get_title();
    if (this.simdata.igc_task.available)
    {
      this.ui_task.ForeColor = System.Drawing.Color.Black;
      this.ui_task.Text = (this.simdata.igc_task.departure_id == null ? "" : this.simdata.igc_task.departure_id + "/") + (this.simdata.igc_task.departure_id == null ? "" : (this.simdata.igc_task.departure_runway == null ? "? " : this.simdata.igc_task.departure_runway + " ")) + this.simdata.igc_task.name;
    }
    else
    {
      this.ui_task.ForeColor = System.Drawing.Color.Gray;
      this.ui_task.Text = "                 PLN Drop Zone";
    }
    this.nb21_web_server.set_header_data_IGC(this.igc_file_writer.get_header_IGC());
    string headerJson = this.simdata.get_header_JSON();
    this.nb21_web_server.set_header_data_JSON(headerJson);
    this.send_clients_JSON(headerJson);
    this.nb21_web_server.set_repeat_data_IGC("I073638FXA3943AGL4447TAS4851NET5254ENL5555FLP5658WSP5961WDI6262GND6363GEA");
    this.send_clients_IGC("I073638FXA3943AGL4447TAS4851NET5254ENL5555FLP5658WSP5961WDI6262GND6363GEA");
  }

  private void igc_file_write()
  {
    try
    {
      int length = this.simdata.flight_data_model.get_length();
      int writtenLength = this.simdata.flight_data_model.get_written_length();
      int num = length - writtenLength;
      Console.WriteLine($"Form1 igc_file_write() {length} records with {num} since last write.");
      if (num < 60)
      {
        Console.WriteLine("Form1 igc_file_write() short tracklog so ignoring file write.");
      }
      else
      {
        this.ui_reset_igc();
        this.current_igc_file_path = this.igc_file_writer.write_file();
        if (this.current_igc_file_path != "")
        {
          this.simdata.flight_data_model.set_written_length(length);
          this.ui_file_icon.Visible = true;
          string str = $"[{this.simdata.recording_timer.Elapsed.ToString("hh\\:mm\\:ss")}]";
          if (this.simdata.sim_has_crashed)
            this.ui_message_bar.Text = "Sim Crash IGC Saved: " + str;
          else
            this.ui_message_bar.Text = "IGC file written " + str;
          this.igc_file_write_pending = false;
        }
        else
        {
          this.ui_message_bar.Text = "Settings: missing folder?";
          this.igc_file_write_pending = true;
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Form1: exception calling igc_file_write(): " + ex.ToString());
      this.AppFailure("Error writing IGC file");
    }
  }

  private void ui_reset_igc()
  {
    Console.WriteLine("Form1 ui_reset_igc()");
    this.ui_message_bar.Text = "";
    this.ui_file_icon.Visible = false;
  }

  private void recording_has_started()
  {
    this.resetWhenRecording = true;
    this.pictureBox_statusImage.Image = this.recordingTock_img;
    this.ui_conn_status.Text = "Recording";
    this.ui_recording_time.Visible = true;
    this.BlinkingTimer.Start();
  }

  private void recording_has_stopped()
  {
    Console.WriteLine("APPEVENT RecordingStop");
    this.ui_conn_status.Text = "Connected to " + this.simdata.get_msfs_text();
    this.ui_recording_time.Text = "";
    this.ui_recording_time.Visible = false;
    this.pictureBox_statusImage.Image = this.connected_img;
    this.BlinkingTimer.Stop();
  }

  private void handle_sim_crash() => this.ui_conn_status.Text = "Sim Crash";

  private void handle_pause()
  {
    if (this.simdata.isPaused)
    {
      this.ui_conn_status.Text = this.simdata.get_msfs_text() + " paused";
      this.igc_file_write();
    }
    else if (this.simdata.isRecording)
      this.ui_conn_status.Text = "Recording";
    else
      this.ui_conn_status.Text = "Connected to " + this.simdata.get_msfs_text();
  }

  private void handle_sim_date_change()
  {
    this.ui_local_date.Text = $"{$"{this.simdata.flight_data_model.sim_local_day:0}"} {new string[12]
    {
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec"
    }[(int) this.simdata.flight_data_model.sim_local_month - 1] ?? ""} {$"{this.simdata.flight_data_model.sim_local_year:0}"}";
  }

  private void handle_sim_rate_change()
  {
    string str = $"{this.simdata.flight_data_model.sim_rate:0}";
    Console.WriteLine($"handle_sim_rate_change() '{str}'");
    if (this.simdata.flight_data_model.sim_rate < 1.0)
    {
      this.ui_sim_rate.Text = $"x{this.simdata.flight_data_model.sim_rate:0.00}";
      this.ui_sim_rate.Visible = true;
    }
    else if (str == "1")
    {
      this.ui_sim_rate.Text = "";
      this.ui_sim_rate.Visible = false;
    }
    else
    {
      this.ui_sim_rate.Text = "x" + str;
      this.ui_sim_rate.Visible = true;
    }
  }

  public void send_clients_IGC(string msg) => this.nb21_ws_server.send_IGC(msg);

  public void send_clients_JSON(string msg) => this.nb21_ws_server.send_JSON(msg);

  private bool do_tracklogs_folder_check()
  {
    if (Directory.Exists(this.settings.IGCPath))
      return true;
    this.ui_message_bar.Text = "Settings: missing folder?";
    return false;
  }

  protected override void WndProc(ref Message m)
  {
    bool flag = false;
    if (this.simdata != null)
    {
      if (this.simdata.sim != null)
      {
        try
        {
          if (m.Msg == this.simdata.GetUserSimConnectWinEvent())
          {
            this.simdata.ReceiveSimConnectMessage();
            flag = true;
          }
          else if (m.Msg == 30)
          {
            this.simdata.processPCClockChange();
            flag = true;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("Exception in WndProc " + ex.ToString());
          this.simdata.Disconnect();
        }
      }
    }
    if (flag)
      this.DefWndProc(ref m);
    else
      base.WndProc(ref m);
  }

  private void Form1_Resize(object? sender, EventArgs e)
  {
    if (this.hostForm == null || this.hostForm.WindowState != FormWindowState.Minimized)
      return;
    this.hostForm.Hide();
  }

  private void ctx_showForm_Click(object sender, EventArgs e)
  {
    if (this.hostForm == null)
      return;
    this.hostForm.Show();
    this.hostForm.WindowState = FormWindowState.Normal;
  }

  private void ctx_Quit_Click(object sender, EventArgs e)
  {
    this.hostForm?.Close();
  }

  public void ui_web_urls_connected() => this.ui_web_urls.BackColor = System.Drawing.Color.PaleGreen;

  public void ui_web_urls_disconnected() => this.ui_web_urls.BackColor = System.Drawing.Color.Lavender;

  private void ui_tab_click(object sender, EventArgs e)
  {
    Console.WriteLine($"Tab clicked. Pilot name: '{this.ui_settings_pilot_name.Text}'");
  }

  private void ui_choose_log_folder_click(object sender, EventArgs e)
  {
    Console.WriteLine("Choose Folder clicked. " + this.settings.IGCPath);
    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
    folderBrowserDialog.Description = "Select folder for your flight tracklogs.";
    folderBrowserDialog.ShowNewFolderButton = true;
    folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
    folderBrowserDialog.SelectedPath = this.settings.IGCPath;
    if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
      return;
    this.settings.IGCPath = folderBrowserDialog.SelectedPath;
    this.ui_settings_igc_path.Text = folderBrowserDialog.SelectedPath;
    this.ui_settings_igc_path.Focus();
    this.ui_settings_igc_path.Select(this.ui_settings_igc_path.Text.Length, 0);
  }

  private void ui_settings_losefocus(object sender, EventArgs e)
  {
    Console.WriteLine("Settings lose focus");
    this.save_settings();
    if (!this.do_tracklogs_folder_check() || !this.igc_file_write_pending)
      return;
    this.igc_file_write();
  }

  private void Form1_Closing(object? sender, FormClosingEventArgs e)
  {
    this.PerformShutdownTasks();
  }

  private void PerformShutdownTasks()
  {
    if (this.shutdownHandled)
      return;
    this.shutdownHandled = true;
    Console.WriteLine("NB21 Logger closing");
    if (this.simdata != null)
    {
      int length = this.simdata.flight_data_model.get_length();
      if (length > 60)
        this.igc_file_write();
      else
        Console.WriteLine($"Short tracklog ({length} records) not written to file.");
    }
    this.save_settings();
  }

  private void ui_settings_select(object sender, EventArgs e)
  {
    this.tabPage_Settings.ImageIndex = 1;
    this.tabPage_Home.ImageIndex = 2;
  }

  private void ui_home_select(object sender, EventArgs e)
  {
    this.tabPage_Home.ImageIndex = 0;
    this.tabPage_Settings.ImageIndex = 3;
  }

  private void ui_view_tracklogs(object sender, EventArgs e)
  {
    Console.WriteLine(nameof (ui_view_tracklogs));
    if (this.settings.IGCPath != null && this.settings.IGCPath != "" & Directory.Exists(this.settings.IGCPath))
    {
      Process.Start(new ProcessStartInfo()
      {
        Arguments = this.settings.IGCPath,
        FileName = "explorer.exe"
      });
    }
    else
    {
      int num = (int) MessageBox.Show("Please choose folder in Settings", this.get_title());
    }
  }

  private void ui_file_drag(object sender, MouseEventArgs e)
  {
    DataObject data = new DataObject();
    data.SetFileDropList(new StringCollection()
    {
      this.current_igc_file_path
    });
    int num = (int) this.ui_file_icon.DoDragDrop((object) data, DragDropEffects.Copy | DragDropEffects.Move);
  }

  private void ui_task_click(object sender, EventArgs e)
  {
    OpenFileDialog openFileDialog = new OpenFileDialog();
    openFileDialog.DefaultExt = ".pln";
    openFileDialog.Filter = "PLN files (*.pln)|*.pln";
    if (openFileDialog.ShowDialog() != DialogResult.OK)
      return;
    try
    {
      this.handle_pln_file(openFileDialog.FileName);
    }
    catch (Exception ex)
    {
      this.ui_task.Text = "TASK ERROR";
      Console.WriteLine($"Unexpected error on PLN file select. {ex}");
    }
  }

  private void ui_task_DragEnter(object sender, DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DataFormats.FileDrop))
      e.Effect = DragDropEffects.Copy;
    else
      e.Effect = DragDropEffects.None;
  }

  private void ui_task_DragDrop(object sender, DragEventArgs e)
  {
    try
    {
      this.ui_task.Text = "DROPPED";
      string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop);
      if (data.Length == 1 && Path.GetFileName(data[0]).EndsWith(".pln", true, (CultureInfo) null))
        this.handle_pln_file(data[0]);
      else
        Console.WriteLine("ui_task() Dropped file NOT A PLN");
    }
    catch (Exception ex)
    {
      this.ui_task.Text = "TASK ERROR";
      Console.WriteLine($"Unexpected error on PLN file drop. {ex}");
    }
  }

  private void handle_pln_file(string filepath)
  {
    Console.WriteLine("handle_pln_file " + filepath);
    this.ui_task.Text = Path.GetFileName(filepath);
    this.simdata.igc_task.load_pln_file(filepath);
    if (this.simdata.igc_task.title == null)
      this.ui_task.Text = this.simdata.igc_task.name;
    else
      this.ui_task.Text = this.simdata.igc_task.title;
    this.simdata.trigger(APPEVENT_ID.HeaderUpdate);
  }

  private void ui_home_dragover(object sender, DragEventArgs e) => e.Effect = DragDropEffects.None;

  private void ui_min_max_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
  {
    if (this.compactView)
    {
      this.Height = 275;
      this.Width = 610;
      this.compactView = false;
      this.ui_min_max.Text = "Compact";
    }
    else
    {
      this.Height = 112 /*0x70*/;
      this.Width = 525;
      this.compactView = true;
      this.ui_min_max.Text = "Full";
    }
  }

  private static string GetLocalIPAddress()
  {
    foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
    {
      if (address.AddressFamily == AddressFamily.InterNetwork)
        return address.ToString();
    }
    return "";
  }

  private void ui_auto_start_click(object sender, EventArgs e)
  {
    if (this.ui_auto_start_checkbox.Checked)
      Console.WriteLine("ui_auto_start_click Checked");
    else
      Console.WriteLine("ui_auto_start_click Unchecked");
  }

  private void ui_web_click(object sender, EventArgs e)
  {
    Console.WriteLine(nameof (ui_web_click));
    Process.Start(new ProcessStartInfo("cmd", "/c start " + "http://localhost:54178/b21_task_planner/index.html")
    {
      CreateNoWindow = true
    });
  }

  public delegate void WsConnectDelegate();
}
