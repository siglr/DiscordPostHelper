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
  private
  #nullable disable
  IContainer components;
  private TabControl tabControl1;
  private TabPage tabPage_Home;
  private TabPage tabPage_Settings;
  private ImageList imageList1;
  private PictureBox pictureBox_statusImage;
  private Label ui_conn_status;
  private Label label1;
  private TextBox ui_settings_pilot_name;
  private TextBox ui_settings_pilot_id;
  private TextBox ui_settings_igc_path;
  private Label label2;
  private Button button1;
  private Label label3;
  private Label ui_local_date;
  private Label ui_aircraft_label;
  private Label ui_pilot_label;
  private Label ui_local_time;
  private Label ui_aircraft;
  private Label ui_pilot;
  private Label ui_auto_start_label1;
  private CheckBox ui_auto_start_checkbox;
  private Label ui_task;
  private Label ui_recording_time;
  private Label ui_sim_rate;
  private Label ui_simtime_label;
  private Button view_tracklogs_button;
  private Label ui_message_bar;
  private PictureBox ui_file_icon;
  private LinkLabel ui_min_max;
  private LinkLabel ui_web_urls;
  private Button ui_task_button;
  private Label pln_drop_outline;

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

  private 
  #nullable enable
  string get_title() => $"{this.settings.AppName} {this.settings.AppVersion}";

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

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      this.DetachFromHostForm();
      this.PerformShutdownTasks();
      if (this.components != null)
        this.components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (NB21Logger));
    this.tabControl1 = new TabControl();
    this.tabPage_Home = new TabPage();
    this.ui_task_button = new Button();
    this.ui_file_icon = new PictureBox();
    this.ui_min_max = new LinkLabel();
    this.ui_message_bar = new Label();
    this.view_tracklogs_button = new Button();
    this.ui_simtime_label = new Label();
    this.ui_recording_time = new Label();
    this.ui_sim_rate = new Label();
    this.ui_task = new Label();
    this.ui_aircraft = new Label();
    this.ui_pilot = new Label();
    this.ui_aircraft_label = new Label();
    this.ui_pilot_label = new Label();
    this.ui_local_time = new Label();
    this.ui_local_date = new Label();
    this.ui_conn_status = new Label();
    this.pictureBox_statusImage = new PictureBox();
    this.pln_drop_outline = new Label();
    this.tabPage_Settings = new TabPage();
    this.ui_web_urls = new LinkLabel();
    this.label3 = new Label();
    this.button1 = new Button();
    this.ui_settings_igc_path = new TextBox();
    this.label2 = new Label();
    this.ui_settings_pilot_id = new TextBox();
    this.label1 = new Label();
    this.ui_settings_pilot_name = new TextBox();
    this.ui_auto_start_label1 = new Label();
    this.ui_auto_start_checkbox = new CheckBox();
    this.imageList1 = new ImageList(this.components);
    this.tabControl1.SuspendLayout();
    this.tabPage_Home.SuspendLayout();
    ((ISupportInitialize) this.ui_file_icon).BeginInit();
    ((ISupportInitialize) this.pictureBox_statusImage).BeginInit();
    this.tabPage_Settings.SuspendLayout();
    this.SuspendLayout();
    this.tabControl1.Alignment = TabAlignment.Left;
    this.tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.tabControl1.Controls.Add((Control) this.tabPage_Home);
    this.tabControl1.Controls.Add((Control) this.tabPage_Settings);
    this.tabControl1.ImageList = this.imageList1;
    this.tabControl1.ItemSize = new Size(115, 60);
    this.tabControl1.Location = new Point(2, 0);
    this.tabControl1.Margin = new Padding(0);
    this.tabControl1.Multiline = true;
    this.tabControl1.Name = "tabControl1";
    this.tabControl1.Padding = new Point(0, 0);
    this.tabControl1.SelectedIndex = 0;
    this.tabControl1.Size = new Size(592, 244);
    this.tabControl1.SizeMode = TabSizeMode.Fixed;
    this.tabControl1.TabIndex = 1;
    this.tabControl1.Click += new EventHandler(this.ui_tab_click);
    this.tabPage_Home.AllowDrop = true;
    this.tabPage_Home.BackColor = System.Drawing.Color.LightCyan;
    this.tabPage_Home.Controls.Add((Control) this.ui_task_button);
    this.tabPage_Home.Controls.Add((Control) this.ui_file_icon);
    this.tabPage_Home.Controls.Add((Control) this.ui_min_max);
    this.tabPage_Home.Controls.Add((Control) this.ui_message_bar);
    this.tabPage_Home.Controls.Add((Control) this.view_tracklogs_button);
    this.tabPage_Home.Controls.Add((Control) this.ui_simtime_label);
    this.tabPage_Home.Controls.Add((Control) this.ui_recording_time);
    this.tabPage_Home.Controls.Add((Control) this.ui_sim_rate);
    this.tabPage_Home.Controls.Add((Control) this.ui_task);
    this.tabPage_Home.Controls.Add((Control) this.ui_aircraft);
    this.tabPage_Home.Controls.Add((Control) this.ui_pilot);
    this.tabPage_Home.Controls.Add((Control) this.ui_aircraft_label);
    this.tabPage_Home.Controls.Add((Control) this.ui_pilot_label);
    this.tabPage_Home.Controls.Add((Control) this.ui_local_time);
    this.tabPage_Home.Controls.Add((Control) this.ui_local_date);
    this.tabPage_Home.Controls.Add((Control) this.ui_conn_status);
    this.tabPage_Home.Controls.Add((Control) this.pictureBox_statusImage);
    this.tabPage_Home.Controls.Add((Control) this.pln_drop_outline);
    this.tabPage_Home.ImageIndex = 0;
    this.tabPage_Home.Location = new Point(64 /*0x40*/, 4);
    this.tabPage_Home.Margin = new Padding(3, 2, 3, 2);
    this.tabPage_Home.Name = "tabPage_Home";
    this.tabPage_Home.Padding = new Padding(3, 2, 3, 2);
    this.tabPage_Home.Size = new Size(524, 236);
    this.tabPage_Home.TabIndex = 0;
    this.tabPage_Home.DragOver += new DragEventHandler(this.ui_home_dragover);
    this.tabPage_Home.Enter += new EventHandler(this.ui_home_select);
    this.ui_task_button.AllowDrop = true;
    this.ui_task_button.Font = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.ui_task_button.Location = new Point(22, 148);
    this.ui_task_button.Name = "ui_task_button";
    this.ui_task_button.Size = new Size(97, 30);
    this.ui_task_button.TabIndex = 17;
    this.ui_task_button.Text = "Task:";
    this.ui_task_button.TextAlign = ContentAlignment.MiddleRight;
    this.ui_task_button.UseVisualStyleBackColor = true;
    this.ui_task_button.Click += new EventHandler(this.ui_task_click);
    this.ui_task_button.DragDrop += new DragEventHandler(this.ui_task_DragDrop);
    this.ui_task_button.DragEnter += new DragEventHandler(this.ui_task_DragEnter);
    this.ui_file_icon.Image = (Image) Resources.file_icon;
    this.ui_file_icon.Location = new Point(476, 174);
    this.ui_file_icon.Name = "ui_file_icon";
    this.ui_file_icon.Size = new Size(38, 57);
    this.ui_file_icon.TabIndex = 15;
    this.ui_file_icon.TabStop = false;
    this.ui_file_icon.Visible = false;
    this.ui_file_icon.MouseDown += new MouseEventHandler(this.ui_file_drag);
    this.ui_min_max.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.ui_min_max.AutoSize = true;
    this.ui_min_max.Location = new Point(458, 5);
    this.ui_min_max.Name = "ui_min_max";
    this.ui_min_max.Size = new Size(56, 15);
    this.ui_min_max.TabIndex = 16 /*0x10*/;
    this.ui_min_max.TabStop = true;
    this.ui_min_max.Text = "Compact";
    this.ui_min_max.LinkClicked += new LinkLabelLinkClickedEventHandler(this.ui_min_max_LinkClicked);
    this.ui_message_bar.Font = new Font("Segoe UI", 15.75f);
    this.ui_message_bar.Location = new Point(140, 183);
    this.ui_message_bar.Name = "ui_message_bar";
    this.ui_message_bar.Size = new Size(293, 30);
    this.ui_message_bar.TabIndex = 14;
    this.ui_message_bar.TextAlign = ContentAlignment.MiddleRight;
    this.view_tracklogs_button.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
    this.view_tracklogs_button.Location = new Point(14, 187);
    this.view_tracklogs_button.Name = "view_tracklogs_button";
    this.view_tracklogs_button.Size = new Size(111, 31 /*0x1F*/);
    this.view_tracklogs_button.TabIndex = 13;
    this.view_tracklogs_button.Text = "Browse Files";
    this.view_tracklogs_button.UseVisualStyleBackColor = true;
    this.view_tracklogs_button.Visible = false;
    this.view_tracklogs_button.Click += new EventHandler(this.ui_view_tracklogs);
    this.ui_simtime_label.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
    this.ui_simtime_label.Location = new Point(3, 94);
    this.ui_simtime_label.Name = "ui_simtime_label";
    this.ui_simtime_label.Size = new Size(113, 26);
    this.ui_simtime_label.TabIndex = 12;
    this.ui_simtime_label.Text = "Sim Time:";
    this.ui_simtime_label.TextAlign = ContentAlignment.MiddleRight;
    this.ui_recording_time.BackColor = System.Drawing.Color.Transparent;
    this.ui_recording_time.Font = new Font("Lucida Console", 12f, FontStyle.Bold);
    this.ui_recording_time.Location = new Point(250, 28);
    this.ui_recording_time.MinimumSize = new Size(50, 24);
    this.ui_recording_time.Name = "ui_recording_time";
    this.ui_recording_time.Size = new Size(131, 24);
    this.ui_recording_time.TabIndex = 11;
    this.ui_recording_time.Text = "00:00:00";
    this.ui_sim_rate.BackColor = System.Drawing.Color.Transparent;
    this.ui_sim_rate.Font = new Font("Lucida Console", 14.25f, FontStyle.Bold);
    this.ui_sim_rate.Location = new Point(379, 28);
    this.ui_sim_rate.MinimumSize = new Size(10, 24);
    this.ui_sim_rate.Name = "ui_sim_rate";
    this.ui_sim_rate.Size = new Size(56, 24);
    this.ui_sim_rate.TabIndex = 10;
    this.ui_sim_rate.Text = "x16";
    this.ui_task.AllowDrop = true;
    this.ui_task.AutoEllipsis = true;
    this.ui_task.Font = new Font("Segoe UI", 14.25f);
    this.ui_task.ForeColor = SystemColors.GrayText;
    this.ui_task.Location = new Point(125, 148);
    this.ui_task.Name = "ui_task";
    this.ui_task.Size = new Size(365, 25);
    this.ui_task.TabIndex = 9;
    this.ui_task.Text = "                 PLN Drop Zone";
    this.ui_task.DragDrop += new DragEventHandler(this.ui_task_DragDrop);
    this.ui_task.DragEnter += new DragEventHandler(this.ui_task_DragEnter);
    this.ui_aircraft.AutoEllipsis = true;
    this.ui_aircraft.Font = new Font("Segoe UI", 14.25f);
    this.ui_aircraft.Location = new Point(125, 116);
    this.ui_aircraft.Name = "ui_aircraft";
    this.ui_aircraft.Size = new Size(365, 25);
    this.ui_aircraft.TabIndex = 8;
    this.ui_aircraft.Text = "??";
    this.ui_pilot.AutoSize = true;
    this.ui_pilot.Font = new Font("Segoe UI", 14.25f);
    this.ui_pilot.Location = new Point(125, 69);
    this.ui_pilot.Name = "ui_pilot";
    this.ui_pilot.Size = new Size(249, 25);
    this.ui_pilot.TabIndex = 7;
    this.ui_pilot.Text = "<Please update in Settings>";
    this.ui_aircraft_label.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
    this.ui_aircraft_label.Location = new Point(14, 117);
    this.ui_aircraft_label.Name = "ui_aircraft_label";
    this.ui_aircraft_label.Size = new Size(102, 27);
    this.ui_aircraft_label.TabIndex = 5;
    this.ui_aircraft_label.Text = "Aircraft:";
    this.ui_aircraft_label.TextAlign = ContentAlignment.MiddleRight;
    this.ui_pilot_label.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
    this.ui_pilot_label.Location = new Point(14, 65);
    this.ui_pilot_label.Name = "ui_pilot_label";
    this.ui_pilot_label.Size = new Size(102, 34);
    this.ui_pilot_label.TabIndex = 4;
    this.ui_pilot_label.Text = "Pilot:";
    this.ui_pilot_label.TextAlign = ContentAlignment.MiddleRight;
    this.ui_local_time.Font = new Font("Lucida Console", 18f, FontStyle.Bold);
    this.ui_local_time.Location = new Point(267, 97);
    this.ui_local_time.Name = "ui_local_time";
    this.ui_local_time.Size = new Size(160 /*0xA0*/, 23);
    this.ui_local_time.TabIndex = 3;
    this.ui_local_date.Font = new Font("Segoe UI", 15.75f, FontStyle.Bold);
    this.ui_local_date.Location = new Point(125, 88);
    this.ui_local_date.Name = "ui_local_date";
    this.ui_local_date.Size = new Size(146, 34);
    this.ui_local_date.TabIndex = 2;
    this.ui_local_date.Text = "??";
    this.ui_conn_status.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
    this.ui_conn_status.Location = new Point(77, 23);
    this.ui_conn_status.Margin = new Padding(3, 0, 0, 0);
    this.ui_conn_status.MinimumSize = new Size(80 /*0x50*/, 32 /*0x20*/);
    this.ui_conn_status.Name = "ui_conn_status";
    this.ui_conn_status.Size = new Size(380, 32 /*0x20*/);
    this.ui_conn_status.TabIndex = 1;
    this.ui_conn_status.Text = "MSFS Status";
    this.pictureBox_statusImage.InitialImage = (Image) null;
    this.pictureBox_statusImage.Location = new Point(25, 12);
    this.pictureBox_statusImage.Margin = new Padding(3, 2, 3, 2);
    this.pictureBox_statusImage.Name = "pictureBox_statusImage";
    this.pictureBox_statusImage.Size = new Size(50, 50);
    this.pictureBox_statusImage.SizeMode = PictureBoxSizeMode.Zoom;
    this.pictureBox_statusImage.TabIndex = 0;
    this.pictureBox_statusImage.TabStop = false;
    this.pln_drop_outline.BackColor = System.Drawing.Color.Transparent;
    this.pln_drop_outline.BorderStyle = BorderStyle.FixedSingle;
    this.pln_drop_outline.Location = new Point(19, 146);
    this.pln_drop_outline.Margin = new Padding(0);
    this.pln_drop_outline.Name = "pln_drop_outline";
    this.pln_drop_outline.Size = new Size(483, 35);
    this.pln_drop_outline.TabIndex = 18;
    this.tabPage_Settings.BackColor = System.Drawing.Color.Lavender;
    this.tabPage_Settings.Controls.Add((Control) this.ui_web_urls);
    this.tabPage_Settings.Controls.Add((Control) this.label3);
    this.tabPage_Settings.Controls.Add((Control) this.button1);
    this.tabPage_Settings.Controls.Add((Control) this.ui_settings_igc_path);
    this.tabPage_Settings.Controls.Add((Control) this.label2);
    this.tabPage_Settings.Controls.Add((Control) this.ui_settings_pilot_id);
    this.tabPage_Settings.Controls.Add((Control) this.label1);
    this.tabPage_Settings.Controls.Add((Control) this.ui_settings_pilot_name);
    this.tabPage_Settings.Controls.Add((Control) this.ui_auto_start_label1);
    this.tabPage_Settings.Controls.Add((Control) this.ui_auto_start_checkbox);
    this.tabPage_Settings.ImageIndex = 1;
    this.tabPage_Settings.Location = new Point(64 /*0x40*/, 4);
    this.tabPage_Settings.Margin = new Padding(3, 2, 3, 2);
    this.tabPage_Settings.Name = "tabPage_Settings";
    this.tabPage_Settings.Padding = new Padding(3, 2, 3, 2);
    this.tabPage_Settings.Size = new Size(524, 236);
    this.tabPage_Settings.TabIndex = 1;
    this.tabPage_Settings.Enter += new EventHandler(this.ui_settings_select);
    this.tabPage_Settings.Leave += new EventHandler(this.ui_settings_losefocus);
    this.ui_web_urls.AutoSize = true;
    this.ui_web_urls.BackColor = SystemColors.ButtonFace;
    this.ui_web_urls.Cursor = Cursors.Arrow;
    this.ui_web_urls.Location = new Point(6, 208 /*0xD0*/);
    this.ui_web_urls.Name = "ui_web_urls";
    this.ui_web_urls.Size = new Size(0, 15);
    this.ui_web_urls.TabIndex = 8;
    this.ui_web_urls.TabStop = true;
    this.ui_web_urls.LinkClicked += new LinkLabelLinkClickedEventHandler(this.ui_web_click);
    this.label3.AutoSize = true;
    this.label3.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
    this.label3.Location = new Point(36, 56);
    this.label3.Name = "label3";
    this.label3.Size = new Size(83, 25);
    this.label3.TabIndex = 7;
    this.label3.Text = "Pilot ID:";
    this.label3.TextAlign = ContentAlignment.MiddleRight;
    this.button1.Location = new Point(135, 92);
    this.button1.Name = "button1";
    this.button1.Size = new Size(59, 30);
    this.button1.TabIndex = 6;
    this.button1.Text = "Folder...";
    this.button1.UseVisualStyleBackColor = true;
    this.button1.Click += new EventHandler(this.ui_choose_log_folder_click);
    this.ui_settings_igc_path.Enabled = false;
    this.ui_settings_igc_path.Location = new Point(6, (int) sbyte.MaxValue);
    this.ui_settings_igc_path.Name = "ui_settings_igc_path";
    this.ui_settings_igc_path.Size = new Size(469, 23);
    this.ui_settings_igc_path.TabIndex = 5;
    this.label2.AutoSize = true;
    this.label2.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
    this.label2.Location = new Point(18, 94);
    this.label2.Name = "label2";
    this.label2.Size = new Size(102, 25);
    this.label2.TabIndex = 4;
    this.label2.Text = "Tracklogs:";
    this.label2.TextAlign = ContentAlignment.MiddleRight;
    this.ui_settings_pilot_id.Font = new Font("Segoe UI", 14.25f);
    this.ui_settings_pilot_id.Location = new Point(135, 52);
    this.ui_settings_pilot_id.Name = "ui_settings_pilot_id";
    this.ui_settings_pilot_id.Size = new Size(100, 33);
    this.ui_settings_pilot_id.TabIndex = 2;
    this.label1.AutoSize = true;
    this.label1.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
    this.label1.Location = new Point(0, 15);
    this.label1.Name = "label1";
    this.label1.Size = new Size(115, 25);
    this.label1.TabIndex = 1;
    this.label1.Text = "Pilot Name:";
    this.label1.TextAlign = ContentAlignment.MiddleRight;
    this.ui_settings_pilot_name.Font = new Font("Segoe UI", 14.25f);
    this.ui_settings_pilot_name.Location = new Point(135, 13);
    this.ui_settings_pilot_name.Name = "ui_settings_pilot_name";
    this.ui_settings_pilot_name.Size = new Size(300, 33);
    this.ui_settings_pilot_name.TabIndex = 0;
    this.ui_auto_start_label1.AutoSize = true;
    this.ui_auto_start_label1.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
    this.ui_auto_start_label1.Location = new Point(7, 176 /*0xB0*/);
    this.ui_auto_start_label1.Name = "ui_auto_start_label1";
    this.ui_auto_start_label1.Size = new Size(236, 25);
    this.ui_auto_start_label1.TabIndex = 9;
    this.ui_auto_start_label1.Text = "Auto-start with Windows";
    this.ui_auto_start_label1.TextAlign = ContentAlignment.MiddleLeft;
    this.ui_auto_start_checkbox.AutoSize = true;
    this.ui_auto_start_checkbox.Location = new Point(244, 180);
    this.ui_auto_start_checkbox.Name = "ui_auto_start_checkbox";
    this.ui_auto_start_checkbox.Size = new Size(15, 14);
    this.ui_auto_start_checkbox.TabIndex = 11;
    this.ui_auto_start_checkbox.UseVisualStyleBackColor = true;
    this.ui_auto_start_checkbox.Click += new EventHandler(this.ui_auto_start_click);
    this.imageList1.ImageStream = (ImageListStreamer) componentResourceManager.GetObject("imageList1.ImageStream");
    this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
    this.imageList1.Images.SetKeyName(0, "home.png");
    this.imageList1.Images.SetKeyName(1, "settings.png");
    this.imageList1.Images.SetKeyName(2, "home_small.png");
    this.imageList1.Images.SetKeyName(3, "settings_small.png");
    this.AutoScaleMode = AutoScaleMode.None;
    this.BackColor = System.Drawing.Color.LightCyan;
    this.Controls.Add((Control) this.tabControl1);
    this.Font = new Font("Segoe UI", 9f);
    this.Margin = new Padding(3, 2, 3, 2);
    this.Name = nameof (NB21Logger);
    this.Size = new Size(594, 236);
    this.Load += new EventHandler(this.Form1_Load);
    this.tabControl1.ResumeLayout(false);
    this.tabPage_Home.ResumeLayout(false);
    this.tabPage_Home.PerformLayout();
    ((ISupportInitialize) this.ui_file_icon).EndInit();
    ((ISupportInitialize) this.pictureBox_statusImage).EndInit();
    this.tabPage_Settings.ResumeLayout(false);
    this.tabPage_Settings.PerformLayout();
    this.ResumeLayout(false);
  }

  public delegate void WsConnectDelegate();
}
