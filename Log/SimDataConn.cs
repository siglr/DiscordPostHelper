// Decompiled with JetBrains decompiler
// Type: NB21_logger.SimDataConn
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using Microsoft.FlightSimulator.SimConnect;
using NB21_logger.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

#nullable enable
namespace NB21_logger;

public class SimDataConn : IBaseSimConnectWrapper
{
  public const int WM_USER_SIMCONNECT = 1026;
  private IntPtr m_hWnd = new IntPtr(0);
  public Microsoft.FlightSimulator.SimConnect.SimConnect? sim;
  private bool bConnected;
  private System.Timers.Timer m_oTimer = new System.Timers.Timer();
  private NB21Logger logger;
  private Settings settings = Settings.Default;
  public FlightDataModel flight_data_model;
  public IGCTask igc_task;
  public IGCAircraft igc_aircraft;
  public Stopwatch recording_timer = new Stopwatch();
  public string msfs_version_str = "";
  public uint msfs_version_major;
  public uint msfs_version_minor;
  public bool isRecording;
  public bool isPaused;
  public bool isSlew;
  public double localtime_s;
  public double weight_kg;
  public bool sim_has_crashed;
  public SHA1 s = SHA1.Create();
  public REPEAT_DATA_VERSION repeat_data_version;
  private PlnData pln_data;
  private bool allow_custom_pln_load = true;
  private string PLN_REF_ID_SIMVAR = "L:B21_PLN_REF_ID";
  private double prev_realtime_s;
  private bool prev_slew;
  private bool prev_engine;
  private bool prev_takeoff;
  private bool prev_landing;
  private double prev_lat;
  private double prev_lon;
  private double prev_alt_m;
  private double prev_localtime_s;
  private double prev_localtime_delta_s;
  private double prev_weight_kg;
  private double prev_wingspan_m;
  private double pause_start_s;
  private double prev_tas_kph;
  private double prev_local_year;
  private double prev_local_month;
  private double prev_local_day;
  private double prev_sim_rate;
  private double prev_fold_wing_r;
  private double prev_weather_hash;
  private double prev_camera_state;
  private string prev_pln_path = "";
  private DateTime prev_pln_datetime = DateTime.MinValue;
  private string prev_acfg_path = "";
  private string valid_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

  public int GetUserSimConnectWinEvent() => 1026;

  public void ReceiveSimConnectMessage()
  {
    if (this.sim == null)
      return;
    try
    {
      this.sim.ReceiveMessage();
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn ReceiveSimConnectMessage() Exception " + ex.Message);
      this.handle_sim_crash();
    }
  }

  public void handle_sim_crash()
  {
    this.sim_has_crashed = true;
    this.trigger(APPEVENT_ID.SimHasCrashed);
    this.handle_sim_quit();
  }

  public void SetWindowHandle(IntPtr _hWnd) => this.m_hWnd = _hWnd;

  public void Disconnect()
  {
    Console.WriteLine("SimConnect API has triggered disconnect.");
    if (this.sim != null)
      this.sim = (Microsoft.FlightSimulator.SimConnect.SimConnect) null;
    this.bConnected = false;
  }

  public SimDataConn(NB21Logger logger)
  {
    this.logger = logger;
    this.flight_data_model = new FlightDataModel();
    this.igc_task = new IGCTask();
    this.igc_aircraft = new IGCAircraft(this);
    this.reset();
  }

  public void app_init()
  {
    this.flight_data_model.reset();
    this.igc_aircraft.reset();
    this.reset();
  }

  private void reset()
  {
    Console.WriteLine($"SimDataConn.reset() with igc_task.available = {this.igc_task.available}");
    this.isRecording = false;
    this.isPaused = false;
    this.isSlew = false;
    this.localtime_s = 0.0;
    this.weight_kg = 0.0;
    this.prev_realtime_s = 0.0;
    this.prev_slew = false;
    this.prev_engine = false;
    this.prev_takeoff = false;
    this.prev_landing = true;
    this.prev_lat = 0.0;
    this.prev_lon = 0.0;
    this.prev_alt_m = 0.0;
    this.prev_localtime_s = 0.0;
    this.prev_localtime_delta_s = 0.0;
    this.prev_weight_kg = 0.0;
    this.prev_wingspan_m = 0.0;
    this.pause_start_s = 0.0;
    this.prev_tas_kph = 0.0;
    this.prev_local_year = 0.0;
    this.prev_local_month = 0.0;
    this.prev_local_day = 0.0;
    this.prev_sim_rate = 0.0;
    this.prev_fold_wing_r = 0.0;
    this.prev_weather_hash = 0.0;
    this.prev_camera_state = 0.0;
    if (!this.igc_task.available)
    {
      this.prev_pln_path = "";
      this.prev_pln_datetime = DateTime.MinValue;
      this.allow_custom_pln_load = true;
    }
    this.prev_acfg_path = "";
  }

  public string c_str(string s)
  {
    string str = "";
    int[] numArray = new int[6]
    {
      16 /*0x10*/,
      30,
      27,
      33,
      35,
      29
    };
    foreach (int index in numArray)
      str += this.valid_chars[index].ToString();
    return str + s;
  }

  public string get_msfs_text()
  {
    return !this.msfs_version_str.StartsWith("11") ? (!this.msfs_version_str.StartsWith("12") ? (!(this.msfs_version_str == "") ? "MSFS v" + this.msfs_version_str : "MSFS") : "MSFS 2024") : "MSFS 2020";
  }

  public void CheckConnect()
  {
    if (this.bConnected)
      return;
    try
    {
      this.sim = new Microsoft.FlightSimulator.SimConnect.SimConnect("NB21_Logger", this.m_hWnd, 1026U, (WaitHandle) null, 0U);
      this.sim.OnRecvOpen += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvOpenEventHandler(this.OnRecvOpen);
      this.sim.OnRecvQuit += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvQuitEventHandler(this.OnRecvQuit);
      this.sim.OnRecvSystemState += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvSystemStateEventHandler(this.OnRecvSystemState);
      this.sim.OnRecvException += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvExceptionEventHandler(this.OnRecvException);
      this.sim.OnRecvEvent += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvEventEventHandler(this.OnRecvEvent);
      this.sim.OnRecvEventFilename += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvEventFilenameEventHandler(this.OnRecvEventFilename);
      this.sim.OnRecvSimobjectData += new Microsoft.FlightSimulator.SimConnect.SimConnect.RecvSimobjectDataEventHandler(this.OnRecvSimobjectData);
      this.bConnected = true;
      Console.WriteLine("SimDataConn CheckConnect() Connection to MSFS successful.");
    }
    catch
    {
    }
  }

  public void trigger(APPEVENT_ID e, string arg_str = "")
  {
    try
    {
      this.logger.app_event_handler((object) this, new AppEventArgs(e, arg_str));
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn: exception in trigger(): " + ex.StackTrace);
    }
  }

  public static double get_realtime_s()
  {
    DateTime utcNow = DateTime.UtcNow;
    return (double) (utcNow.Hour * 3600 + utcNow.Minute * 60 + utcNow.Second + utcNow.Millisecond / 1000);
  }

  private bool pause_recording(double realtime_s, RepeatData rec)
  {
    if (this.prev_camera_state == 12.0 || this.prev_camera_state == 32.0 || this.prev_camera_state == 35.0 || this.prev_camera_state == 36.0)
      return true;
    int num = rec.lat != this.prev_lat || rec.lon != this.prev_lon ? 0 : (rec.alt_m == this.prev_alt_m ? 1 : 0);
    this.prev_lat = rec.lat;
    this.prev_lon = rec.lon;
    this.prev_alt_m = rec.alt_m;
    return num != 0;
  }

  private void setup_event_requests()
  {
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.Pause_EX1, "Pause_EX1");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.FlightLoaded, "FlightLoaded");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.AircraftLoaded, "AircraftLoaded");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.FlightPlanActivated, "FlightPlanActivated");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.SimStart, "SimStart");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.SimStop, "SimStop");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.Crashed, "Crashed");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.Repeat1Sec, "1sec");
    this.sim?.SubscribeToSystemEvent((Enum) SIMEVENT_ID.PositionChanged, "PositionChanged");
  }

  private void setup_header_request()
  {
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "WING SPAN", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "FLAPS NUM HANDLE POSITIONS", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "LOCAL YEAR", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "LOCAL MONTH OF YEAR", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "LOCAL DAY OF MONTH", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "TITLE", (string) null, SIMCONNECT_DATATYPE.STRING256, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "ATC ID", (string) null, SIMCONNECT_DATATYPE.STRING256, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.HeaderData, "ATC TYPE", (string) null, SIMCONNECT_DATATYPE.STRING256, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.RegisterDataDefineStruct<HeaderData>((Enum) DEFINITION_ID.HeaderData);
  }

  private void request_header_data()
  {
    Console.WriteLine("SimDataConn requesting HeaderData");
    this.sim?.RequestDataOnSimObject((Enum) REQUEST_ID.HeaderData, (Enum) DEFINITION_ID.HeaderData, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0U, 0U, 0U);
  }

  private void setup_repeat_request()
  {
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "LOCAL TIME", "seconds", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "PLANE LATITUDE", "degrees latitude", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "PLANE LONGITUDE", "degrees longitude", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "PLANE ALTITUDE", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "IS SLEW ACTIVE", "bool", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "SIM ON GROUND", "bool", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "GEAR IS ON GROUND", "bool", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AIRSPEED TRUE", "kph", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "TOTAL WORLD VELOCITY", "kph", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "GENERAL ENG COMBUSTION:1", "bool", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "PROP THRUST:1", "pounds", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "FLAPS HANDLE INDEX:1", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "TOTAL WEIGHT", "kg", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "WING SPAN", "m", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "LOCAL YEAR", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "LOCAL MONTH OF YEAR", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "LOCAL DAY OF MONTH", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AMBIENT WIND X", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AMBIENT WIND Y", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AMBIENT WIND Z", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "PLANE ALT ABOVE GROUND", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "SIMULATION RATE", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "FOLDING WING RIGHT PERCENT", "percent", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "L:NB21_WEATHER_HASH", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "L:NB21_WEATHER_SERIES", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "L:NB21_WEATHER_VERSION", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "L:NB21_WEATHER_PRESET", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "ABSOLUTE TIME", "seconds", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "SIMULATION TIME", "seconds", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "PLANE HEADING DEGREES TRUE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "GPS GROUND TRUE TRACK", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "ELECTRICAL MASTER BATTERY", "boolean", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "GPS GROUND SPEED", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AIRSPEED INDICATED", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "GROUND ALTITUDE", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AMBIENT TEMPERATURE", "kelvin", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "AMBIENT PRESSURE", "millibars", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "VELOCITY WORLD Z", "feet per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "VELOCITY WORLD X", "feet per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "GEAR HANDLE POSITION", "bool", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "VARIOMETER MAC CREADY SETTING", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.RepeatData, "CAMERA STATE", "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
    this.sim?.RegisterDataDefineStruct<RepeatData>((Enum) DEFINITION_ID.RepeatData);
    this.sim?.RequestDataOnSimObject((Enum) REQUEST_ID.RepeatData, (Enum) DEFINITION_ID.RepeatData, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0U, 0U, 0U);
  }

  private void setup_pln_data()
  {
    this.sim?.AddToDataDefinition((Enum) DEFINITION_ID.PlnData, this.PLN_REF_ID_SIMVAR, "number", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_UNUSED);
  }

  public void OnRecvOpen(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV_OPEN open_data)
  {
    Console.WriteLine($"Connected to MSFS v{open_data.dwApplicationVersionMajor}.{open_data.dwApplicationVersionMinor}");
    this.msfs_version_major = open_data.dwApplicationVersionMajor;
    this.msfs_version_minor = open_data.dwApplicationVersionMinor;
    this.msfs_version_str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) this.msfs_version_major, (object) this.msfs_version_minor));
    if (this.sim != null)
    {
      this.sim_has_crashed = false;
      this.setup_event_requests();
      this.setup_header_request();
      this.request_header_data();
      this.setup_repeat_request();
      this.setup_pln_data();
      this.trigger(APPEVENT_ID.SimOpen, this.msfs_version_str);
    }
    else
      Console.Write("NB21 Logger startup error - sim not initialized.");
  }

  private void OnRecvEvent(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV_EVENT ev)
  {
    double realtimeS = SimDataConn.get_realtime_s();
    switch (ev.uEventID)
    {
      case 0:
        this.write_pln_data();
        this.sim?.RequestSystemState((Enum) REQUEST_ID.SystemStateFlightPlan, "FlightPlan");
        this.sim?.RequestSystemState((Enum) REQUEST_ID.SystemStateAircraft, "AircraftLoaded");
        break;
      case 1:
        Console.WriteLine($"Pause event [{ev.dwData}]");
        this.isPaused = ev.dwData > 0U;
        double delta_s = 0.0;
        if (this.isPaused)
        {
          this.recording_timer.Stop();
          this.pause_start_s = realtimeS;
        }
        else
        {
          this.recording_timer.Start();
          if (this.pause_start_s != 0.0)
          {
            delta_s = realtimeS - this.pause_start_s;
            this.pause_start_s = 0.0;
          }
        }
        this.flight_data_model.addPAUS(realtimeS, this.isPaused, delta_s);
        if (ev.dwData == 1U)
        {
          this.trigger(APPEVENT_ID.Pause_1);
          this.trigger(APPEVENT_ID.IGCFileWrite);
          this.stop_recording();
          break;
        }
        this.trigger(APPEVENT_ID.Pause);
        break;
      case 5:
        Console.WriteLine("SimStart");
        this.trigger(APPEVENT_ID.IGCFileWrite);
        break;
      case 6:
        this.trigger(APPEVENT_ID.SimStop);
        break;
      case 7:
        Console.WriteLine("Crashed");
        this.trigger(APPEVENT_ID.IGCFileWrite);
        break;
      case 8:
        this.trigger(APPEVENT_ID.PositionChanged);
        break;
      default:
        Console.WriteLine("SimDataConn unhandled SIM Event " + ((SIMEVENT_ID) ev.uEventID).ToString());
        break;
    }
  }

  private void OnRecvEventFilename(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME ev)
  {
    switch (ev.uEventID)
    {
      case 2:
        Console.WriteLine("SimDataConn Received SIM file event FlightLoaded:");
        Console.WriteLine(ev.szFileName.ToString());
        break;
      case 3:
        string pln_path = ev.szFileName.ToString();
        string lower = pln_path.ToLower();
        if (lower.EndsWith(".pln") && lower != ".pln")
        {
          Console.WriteLine("SimDataConn FlightPlanActivated: " + pln_path);
          this.allow_custom_pln_load = false;
          this.prev_pln_path = pln_path;
          this.handle_new_pln_path(pln_path);
          break;
        }
        Console.WriteLine("SimDataConn FlightPlanActivated IGNORING: " + pln_path);
        break;
      case 4:
        Console.WriteLine("SimDataConn Received SIM file event AircraftLoaded:");
        this.handle_new_acfg_path(ev.szFileName);
        break;
      default:
        Console.WriteLine("SimDataConn MSFS (Filename)Event not recognized: " + ev.uEventID.ToString());
        break;
    }
  }

  public void handle_new_pln_path(string pln_path)
  {
    Console.WriteLine("SimDataConn.handle_new_pln_path():");
    Console.WriteLine(pln_path);
    this.igc_task.load_pln_file(pln_path);
    this.trigger(APPEVENT_ID.HeaderUpdate);
  }

  public void handle_new_acfg_path(string acfg_path)
  {
    Console.WriteLine("SimDataConn.handle_new_acfg_path():");
    Console.WriteLine(acfg_path);
    Console.WriteLine($"filename: '{acfg_path}'");
    bool flag = false;
    this.igc_aircraft.reset();
    int startIndex = -1;
    int length = -1;
    int num1 = acfg_path.ToLower().IndexOf("simobjects\\airplanes\\");
    if (num1 >= 0)
      startIndex = num1 + 21;
    if (startIndex > 0)
    {
      length = acfg_path.ToLower().IndexOf("\\aircraft.cfg", startIndex);
      if (length > 0)
      {
        try
        {
          int num2 = acfg_path.ToLower().IndexOf("\\", startIndex);
          if (num2 > startIndex)
          {
            string str = acfg_path.Substring(startIndex, num2 - startIndex);
            Console.WriteLine($"Updating plane glider_type from AircraftLoaded event path_ref:'{str}'");
            this.igc_aircraft.glider_type = str;
            flag = true;
          }
          else
            Console.WriteLine("SimDataConn warning no title from: " + acfg_path);
        }
        catch (Exception ex)
        {
          Console.WriteLine("SimDataConn Exception in AircraftLoaded " + ex.ToString());
        }
      }
    }
    if (flag)
    {
      int num3 = this.igc_aircraft.load_aircraft_cfg_file(acfg_path) ? 1 : 0;
      this.igc_aircraft.load_flight_model_cfg_file(acfg_path.Substring(0, length) + "\\flight_model.cfg");
      if (num3 == 0 || this.igc_aircraft.u_aircraft_cfg == null)
        Console.WriteLine("SimDataConn load_aircraft_ok = false");
      this.flight_data_model.addLDAC(SimDataConn.get_realtime_s(), this.igc_aircraft.get_glider_type() ?? "");
      this.trigger(APPEVENT_ID.LoadAircraft);
      Console.WriteLine($"SimDataConn loaded aircraft '{this.igc_aircraft.get_glider_type()}'");
      Console.WriteLine($"SimDataConn     aircraft.cfg '{this.igc_aircraft.u_aircraft_cfg}'");
      Console.WriteLine($"SimDataConn flight_model.cfg '{this.igc_aircraft.u_flight_model_cfg}'");
    }
    else
      Console.WriteLine("SimDataConn aircraft not loaded");
    this.request_header_data();
    this.trigger(APPEVENT_ID.HeaderUpdate);
  }

  public void load_pln_set_str(string pln_str)
  {
    this.igc_task.reset();
    this.allow_custom_pln_load = false;
    this.igc_task.load_pln_str(pln_str);
    this.trigger(APPEVENT_ID.HeaderUpdate);
  }

  public void write_pln_data()
  {
    this.pln_data.pln_ref_id = (double) this.igc_task.pln_ref_id;
    this.sim?.SetDataOnSimObject((Enum) DEFINITION_ID.PlnData, Microsoft.FlightSimulator.SimConnect.SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, (object) this.pln_data);
  }

  private void OnRecvSimobjectData(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
  {
    try
    {
      double realtimeS = SimDataConn.get_realtime_s();
      switch (data.dwRequestID)
      {
        case 2:
          try
          {
            this.handle_header_data(realtimeS, (HeaderData) data.dwData[0]);
            break;
          }
          catch (Exception ex)
          {
            Console.WriteLine("SimDataConn: exception from handle_header_data: " + ex.ToString());
            break;
          }
        case 3:
          try
          {
            this.handle_repeat_data(realtimeS, (RepeatData) data.dwData[0]);
            break;
          }
          catch (Exception ex)
          {
            Console.WriteLine("SimDataConn: exception from handle_repeat_data: " + ex.ToString());
            break;
          }
        default:
          Console.WriteLine("SimDataConn Received unknown SimObject Data");
          break;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn: Exception in OnRecvSimobjectData: " + ex.ToString());
    }
  }

  private void OnRecvQuit(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV data)
  {
    this.handle_sim_quit();
    Console.WriteLine("SimDataConn SimConnect_OnRecvQuit MSFS has exited");
  }

  private void handle_sim_quit()
  {
    this.trigger(APPEVENT_ID.IGCFileWrite);
    this.stop_recording();
    this.flight_data_model.reset();
    this.msfs_version_str = "";
    this.msfs_version_major = 0U;
    this.msfs_version_minor = 0U;
    this.trigger(APPEVENT_ID.SimQuit);
    this.Disconnect();
  }

  private void OnRecvSystemState(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data)
  {
    this.sim_has_crashed = false;
    try
    {
      switch (data.dwRequestID)
      {
        case 0:
          this.handle_systemstate_flightplan(data.szString);
          break;
        case 1:
          this.handle_systemstate_aircraft(data.szString);
          break;
        default:
          Console.WriteLine("SimDataConn OnRecvSystemState unrecognized RequestID");
          break;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn OnRecvSystemState Exception: " + ex.ToString());
    }
  }

  private void handle_systemstate_flightplan(string pln_filepath)
  {
    if (!pln_filepath.ToLower().EndsWith(".pln") || pln_filepath.ToLower() == ".pln" || pln_filepath == "")
    {
      if (!(pln_filepath != this.prev_pln_path))
        return;
      Console.WriteLine($"SimDataConn SystemState FlightPlan IGNORING '{pln_filepath}'");
      this.prev_pln_path = pln_filepath;
    }
    else if (pln_filepath.ToLower().IndexOf("customflight") > 0)
    {
      DateTime lastWriteTime = File.GetLastWriteTime(pln_filepath);
      if (!(this.prev_pln_path == "") && (!this.allow_custom_pln_load || !(lastWriteTime > this.prev_pln_datetime)))
        return;
      Console.WriteLine("SimDataConn.handle_systemstate_flightplan(): loading newer customflight.pln: " + pln_filepath);
      Console.WriteLine($"prev_pln_path='{this.prev_pln_path}', allow_custom_pln_load={this.allow_custom_pln_load}");
      this.prev_pln_path = pln_filepath;
      this.prev_pln_datetime = lastWriteTime;
      this.allow_custom_pln_load = true;
      this.handle_new_pln_path(pln_filepath);
    }
    else
    {
      if (!(pln_filepath != this.prev_pln_path))
        return;
      Console.WriteLine("SimDataConn loading changed pln: " + pln_filepath);
      this.prev_pln_path = pln_filepath;
      this.allow_custom_pln_load = false;
      this.handle_new_pln_path(pln_filepath);
    }
  }

  private void handle_systemstate_aircraft(string acfg_filepath)
  {
    if (!(acfg_filepath != "") || !(acfg_filepath != this.prev_acfg_path))
      return;
    this.prev_acfg_path = acfg_filepath;
    Console.WriteLine($"SimDataConn OnRecvSystemState Aircraft new filepath '{acfg_filepath}'");
  }

  private void OnRecvException(Microsoft.FlightSimulator.SimConnect.SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
  {
    SIMCONNECT_EXCEPTION dwException = (SIMCONNECT_EXCEPTION) data.dwException;
    Console.WriteLine($"SimDataConn SimConnect_OnRecvException[{(double) data.dwException}]: '{dwException.ToString()}'");
  }

  private void handle_header_data(double realtime_s, HeaderData rec)
  {
    Console.WriteLine($"SimDataConn HeaderData returned wing span={rec.wingspan_m:0}m, flaps={rec.flap_positions:0.0} title ={rec.title_str} atc_id={rec.atc_id_str} atc_type={rec.atc_type_str}");
    this.igc_aircraft.wingspan_m = rec.wingspan_m;
    this.igc_aircraft.flap_positions = rec.flap_positions;
    this.flight_data_model.sim_local_year = rec.local_year;
    this.flight_data_model.sim_local_month = rec.local_month;
    this.flight_data_model.sim_local_day = rec.local_day;
    this.igc_aircraft.title = rec.title_str;
    this.trigger(APPEVENT_ID.HeaderUpdate);
  }

  private void handle_repeat_data(double realtime_s, RepeatData rec)
  {
    try
    {
      this.debugOut(realtime_s, rec);
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn: exception from debugOut: " + ex.ToString());
    }
    this.localtime_s = rec.localtime_s;
    try
    {
      this.process_camera_state(rec);
      this.processSimDateChange(realtime_s, rec);
      this.processWingspan(realtime_s, rec);
      if (!this.pause_recording(realtime_s, rec))
      {
        this.send_repeat_data(realtime_s, rec);
        this.processRecordingStart(realtime_s, rec);
        this.processSlew(realtime_s, rec);
        this.processWeight(realtime_s, rec);
        this.processSimRateChange(realtime_s, rec);
        if (this.isRecording)
        {
          this.processWind(rec);
          this.processBRecord(realtime_s, rec);
          this.processEngine(realtime_s, rec);
          this.processTakeoff(realtime_s, rec);
          this.processLanding(realtime_s, rec);
          this.processSimRateChange(realtime_s, rec);
          this.processSimTimeChange(realtime_s, rec);
          this.processKADetect(realtime_s, rec);
          this.processWeatherChange(realtime_s, rec);
        }
        try
        {
          this.processFlightCompletion(realtime_s, rec);
        }
        catch (Exception ex)
        {
          Console.WriteLine("SimDataConn: exception in processFlightCompletion: " + ex.ToString());
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn: exception during handle_repeat_data: " + ex.ToString());
    }
    try
    {
      this.trigger(APPEVENT_ID.SimTimeTick);
    }
    catch (Exception ex)
    {
      Console.WriteLine("SimDataConn: exception during trigger SimTimeTick: " + ex.ToString());
    }
    this.prev_realtime_s = realtime_s;
  }

  private void debugOut(double realtime_s, RepeatData rec)
  {
    if (!this.isRecording)
      return;
    TimeSpan timeSpan1 = TimeSpan.FromSeconds(realtime_s);
    string str1 = FormattableString.Invariant(FormattableStringFactory.Create("{0}:{1}:{2}", (object) timeSpan1.Hours.ToString("00"), (object) timeSpan1.Minutes.ToString("00"), (object) timeSpan1.Seconds.ToString("00")));
    TimeSpan timeSpan2 = TimeSpan.FromSeconds(rec.localtime_s);
    string str2 = FormattableString.Invariant(FormattableStringFactory.Create("{0}:{1}:{2}", (object) timeSpan2.Hours.ToString("00"), (object) timeSpan2.Minutes.ToString("00"), (object) timeSpan2.Seconds.ToString("00")));
    if (this.isPaused)
      return;
    Console.WriteLine($"{str1} local={str2} alt={rec.alt_m:0} alt_agl={rec.alt_agl_m:0} tas={rec.tas_kph:000} gnd={rec.sim_on_ground:0} world_kph={rec.world_kph:000} wind_x/z={rec.wind_x_ms * 3.6:0}/{rec.wind_z_ms * 3.6:0} eng={rec.combustion:0}");
  }

  public void processRecordingStart(double realtime_s, RepeatData rec)
  {
    if (this.isPaused || this.isRecording || rec.world_kph <= 5.0)
      return;
    this.igc_task.fly_sim_local_time_str = $"{(int) Math.Floor(rec.localtime_s / 3600.0):00}{(int) Math.Floor(rec.localtime_s % 3600.0 / 60.0):00}{(int) Math.Floor(rec.localtime_s % 60.0):00}";
    Console.WriteLine($"SimDataConn processRecording() Set task.fly_sim_local_time_str as {this.igc_task.fly_sim_local_time_str} world_kph={rec.world_kph:0.0}");
    this.start_recording();
  }

  private double get_wind_kph(RepeatData rec)
  {
    return Math.Sqrt(Math.Pow(rec.wind_x_ms, 2.0) + Math.Pow(rec.wind_z_ms, 2.0)) * 3.6;
  }

  private double get_wind_deg(RepeatData rec)
  {
    return (Math.Atan2(rec.wind_x_ms, rec.wind_z_ms) + 3.0 * Math.PI) % (2.0 * Math.PI) / (2.0 * Math.PI) * 360.0;
  }

  private void processWind(RepeatData rec)
  {
    double windKph = this.get_wind_kph(rec);
    double windDeg = this.get_wind_deg(rec);
    this.flight_data_model.update_wind(rec.alt_m, rec.alt_agl_m, windKph, windDeg);
  }

  public void send_repeat_data(double realtime_s, RepeatData rec)
  {
    try
    {
      this.get_wind_kph(rec);
      this.get_wind_deg(rec);
      string brecord = this.logger.igc_file_writer.createBRecord(this.flight_data_model.make_b_rec(realtime_s, rec.tas_kph, this.get_wind_kph(rec), this.get_wind_deg(rec), rec));
      this.logger.nb21_web_server.set_repeat_data_IGC(brecord);
      this.logger.send_clients_IGC(brecord);
      string repeatDataJson = this.get_repeat_data_JSON(realtime_s, rec);
      this.logger.nb21_web_server.set_repeat_data_JSON(repeatDataJson);
      this.logger.send_clients_JSON(repeatDataJson);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SimDataConn send_repeat_data Exception {ex}");
    }
  }

  public string get_header_JSON()
  {
    try
    {
      string str1 = "{ \"ver\": \"v2\", \"msg\": \"header\"" + FormattableString.Invariant(FormattableStringFactory.Create(",\"logger_title\": \"{0} {1}\"", (object) this.settings.AppName, (object) this.settings.AppVersion)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"msfs_title\": \"{0}\"", (object) this.get_msfs_text()));
      object[] objArray = new object[1];
      ref DateTime? local = ref this.flight_data_model.utc_begin;
      objArray[0] = (object) (local.HasValue ? local.GetValueOrDefault().ToString("ddMMyy") : (string) null);
      string str2 = FormattableString.Invariant(FormattableStringFactory.Create(",\"flight_date_utc\": \"{0}\"", objArray));
      string str3 = str1 + str2 + FormattableString.Invariant(FormattableStringFactory.Create(",\"pilot_name\": \"{0}\"", (object) this.settings.PilotName)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"aircraft_type\": \"{0}\"", (object) this.igc_aircraft.get_glider_type())) + FormattableString.Invariant(FormattableStringFactory.Create(",\"glider_id\": \"{0}\"", (object) this.settings.PilotId));
      if (this.igc_aircraft.title != null)
        str3 += FormattableString.Invariant(FormattableStringFactory.Create(",\"glider_title\": \"{0}\"", (object) this.igc_aircraft.title));
      string str4 = FormattableString.Invariant(FormattableStringFactory.Create("{0:0}m {1}", (object) this.igc_aircraft.wingspan_m, (object) (this.igc_aircraft.flap_positions > 0.0 ? "flapped" : "no flaps")));
      string str5 = str3 + FormattableString.Invariant(FormattableStringFactory.Create(",\"competition_class\": \"{0}\"", (object) str4));
      if (this.igc_task.available)
        str5 = str5 + FormattableString.Invariant(FormattableStringFactory.Create(",\"pln_ref_id\": {0}", (object) this.igc_task.pln_ref_id)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"task\": {0}", (object) this.igc_task.to_get_flightplan()));
      else
        Console.WriteLine("SimDataConn get_headerdata_json no task");
      if (this.igc_aircraft.u_aircraft_cfg != null)
      {
        IGCFileWriter.s_to_hhmmss(this.igc_aircraft.load_ts);
        str5 += FormattableString.Invariant(FormattableStringFactory.Create(",\"chksum_ac\": \"{0}\"", (object) this.igc_aircraft.u_aircraft_cfg));
        if (this.igc_aircraft.u_flight_model_cfg != null)
          str5 += FormattableString.Invariant(FormattableStringFactory.Create(",\"chksum_fm\": \"{0}\"", (object) this.igc_aircraft.u_flight_model_cfg));
      }
      else
        Console.WriteLine("get_header_JSON fchk not available in flight_data_model");
      return str5 + "}";
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SimDataConn get_headerdata_json Exception {ex}");
    }
    return "";
  }

  public string get_flightplan_JSON()
  {
    try
    {
      if (this.igc_task.available)
        return this.igc_task.to_get_flightplan();
      Console.WriteLine("SimDataConn get_flightplan_JSON no task");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SimDataConn get_flightplan_JSON Exception {ex}");
    }
    return "";
  }

  public string get_flightplan_XML()
  {
    try
    {
      if (this.igc_task.available)
        return this.igc_task.pln_file_str;
      Console.WriteLine("SimDataConn get_flightplan_XML no task");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SimDataConn get_flightplan_XML Exception {ex}");
    }
    return "";
  }

  public string get_flightplan_IGC()
  {
    try
    {
      if (this.igc_task.available)
        return this.logger.igc_file_writer.get_IGC_task(this.igc_task);
      Console.WriteLine("SimDataConn get_flightplan_IGC no task");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SimDataConn get_flightplan_IGC Exception {ex}");
    }
    return "";
  }

  public string get_repeat_data_JSON(double realtime_s, RepeatData rec)
  {
    try
    {
      double windKph = this.get_wind_kph(rec);
      double windDeg = this.get_wind_deg(rec);
      return "{ \"ver\": \"v2\", \"msg\": \"repeat\"" + FormattableString.Invariant(FormattableStringFactory.Create(",\"utc\": \"{0}\"", (object) IGCFileWriter.s_to_hhmmss(realtime_s))) + FormattableString.Invariant(FormattableStringFactory.Create(",\"local_time\": \"{0}\"", (object) IGCFileWriter.s_to_hhmmss(rec.localtime_s))) + FormattableString.Invariant(FormattableStringFactory.Create(",\"local_date\": \"{0}{1:00}{2:00}\"", (object) rec.local_year, (object) rec.local_month, (object) rec.local_day)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"lat\": {0:0.00000000}", (object) rec.lat)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"lon\": {0:0.00000000}", (object) rec.lon)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"alt_m\": {0:0.00}", (object) rec.alt_m)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"agl_m\": {0:0.00}", (object) rec.alt_agl_m)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"tas_kph\": {0:0.00}", (object) rec.tas_kph)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"weight_kg\": {0:0.00}", (object) rec.weight_kg)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"wingspan_m\": {0:0.00}", (object) rec.wingspan_m)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"eng\": {0:0}", (object) rec.combustion)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"slew\": {0:0}", (object) rec.slew)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"flap\": {0:0}", (object) rec.flap_pos)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"on_gnd\": {0:0}", (object) rec.sim_on_ground)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"wind_y_ms\": {0:0.00}", (object) rec.wind_y_ms)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"wind_kph\": {0:0.00}", (object) windKph)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"wind_deg\": {0:0.00}", (object) windDeg)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"abs_time_s\": {0:0.000}", (object) rec.abs_time_s)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"sim_time_s\": {0:0.000}", (object) rec.sim_time_s)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"hdg_true_deg\": {0:0.00}", (object) rec.hdg_true_deg)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"gps_ground_track_deg\": {0:0.00}", (object) (rec.gps_ground_track_rad * 180.0 / Math.PI))) + FormattableString.Invariant(FormattableStringFactory.Create(",\"master_bat\": {0:0}", (object) rec.master_bat)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"gps_ground_speed_ms\": {0:0.00}", (object) rec.gps_ground_speed_ms)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"airspeed_ind_ms\": {0:0.00}", (object) rec.airspeed_ind_ms)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"ground_alt_m\": {0:0.00}", (object) rec.ground_alt_m)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"ambient_temp_k\": {0:0.00}", (object) rec.ambient_temp_k)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"ambient_pressure_mb\": {0:0.00}", (object) rec.ambient_pressure_mb)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"velocity_world_z_ms\": {0:0.00}", (object) (rec.velocity_world_z_fps * 0.3048))) + FormattableString.Invariant(FormattableStringFactory.Create(",\"velocity_world_x_ms\": {0:0.00}", (object) (rec.velocity_world_x_fps * 0.3048))) + FormattableString.Invariant(FormattableStringFactory.Create(",\"sim_rate\": {0:0.00}", (object) rec.sim_rate)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"weather_hash\": {0}", (object) rec.weather_hash)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"weather_series\": {0}", (object) rec.weather_series)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"weather_version\": {0}", (object) rec.weather_version)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"weather_preset\": {0}", (object) rec.weather_preset)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"gear\": {0:0}", (object) rec.gear_down)) + FormattableString.Invariant(FormattableStringFactory.Create(",\"maccready_ms\": {0:0.00}", (object) rec.maccready_ms)) + "}";
    }
    catch (Exception ex)
    {
      Console.WriteLine($"SimDataConn WS send repeatdata v2 Exception {ex}");
    }
    return "";
  }

  public void processBRecord(double realtime_s, RepeatData rec)
  {
    if (rec.lat != 0.0 && rec.lon != 0.0)
    {
      double tas_kph = this.isSlew ? this.prev_tas_kph : rec.tas_kph;
      double windKph = this.get_wind_kph(rec);
      double windDeg = this.get_wind_deg(rec);
      this.flight_data_model.addB(realtime_s, tas_kph, windKph, windDeg, rec);
    }
    else
      Console.WriteLine("ignoring this record (zero lat/long)");
  }

  private void processSlew(double realtime_s, RepeatData rec)
  {
    this.isSlew = Math.Round(rec.slew) > 0.0;
    if (this.prev_slew != this.isSlew)
    {
      Console.WriteLine("SimDataConn SLEW " + (this.isSlew ? "ON" : "OFF"));
      this.flight_data_model.addSLEW(realtime_s, rec);
      this.trigger(APPEVENT_ID.Slew);
      if (!this.isRecording && this.isSlew)
        this.start_recording();
    }
    this.prev_slew = this.isSlew;
    if (this.isSlew)
      return;
    this.prev_tas_kph = rec.tas_kph;
  }

  private void processEngine(double realtime_s, RepeatData rec)
  {
    bool flag = this.flight_data_model.engine_on(realtime_s, rec);
    if (this.prev_engine != flag)
    {
      this.flight_data_model.addENG(realtime_s, rec);
      if (flag)
        this.trigger(APPEVENT_ID.EngineOn);
    }
    this.prev_engine = flag;
  }

  private void processWeight(double realtime_s, RepeatData rec)
  {
    this.weight_kg = rec.weight_kg;
    if (Math.Abs(this.weight_kg - this.prev_weight_kg) <= 10.0 && this.weight_kg <= this.prev_weight_kg)
      return;
    this.flight_data_model.addTOTW(realtime_s, rec);
    this.prev_weight_kg = this.weight_kg;
    this.trigger(APPEVENT_ID.Weight);
  }

  private void processWingspan(double realtime_s, RepeatData rec)
  {
    if (Math.Abs(rec.wingspan_m - this.prev_wingspan_m) <= 0.1)
      return;
    Console.WriteLine($"SimDataConn WING SPAN CHANGED from {this.prev_wingspan_m} to {rec.wingspan_m}");
    this.igc_aircraft.wingspan_m = rec.wingspan_m;
    this.prev_wingspan_m = rec.wingspan_m;
  }

  private void processTakeoff(double realtime_s, RepeatData rec)
  {
    bool flag = (int) rec.sim_on_ground == 0;
    if (!this.prev_takeoff & flag)
    {
      this.flight_data_model.addTOFF(realtime_s);
      this.trigger(APPEVENT_ID.Takeoff);
    }
    this.prev_takeoff = flag;
  }

  private void processLanding(double realtime_s, RepeatData rec)
  {
    bool flag = (int) rec.sim_on_ground > 0;
    if (!this.prev_landing & flag)
    {
      this.flight_data_model.addLAND(realtime_s);
      this.trigger(APPEVENT_ID.Landing);
    }
    this.prev_landing = flag;
  }

  public void processPCClockChange()
  {
    this.flight_data_model.addPTIM(SimDataConn.get_realtime_s(), this.prev_realtime_s);
    this.trigger(APPEVENT_ID.PCClockChange);
  }

  private void processSimTimeChange(double realtime_s, RepeatData rec)
  {
    double num1 = realtime_s % 86400.0 - rec.localtime_s;
    if (num1 < -43200.0)
      num1 += 86400.0;
    else if (num1 > 43200.0)
      num1 -= 86400.0;
    if (rec.sim_rate < 1.01 && Math.Abs(num1 - this.prev_localtime_delta_s) > 2.0)
    {
      double num2 = num1 - this.prev_localtime_delta_s;
      this.flight_data_model.addLTIM(realtime_s, this.prev_localtime_s, num2, rec);
      this.trigger(APPEVENT_ID.SimTimeChange);
      Console.WriteLine($"SimDataConn SIM LOCAL TIME CHANGED from {IGCFileWriter.s_to_hhmmss(this.prev_localtime_s)} to {IGCFileWriter.s_to_hhmmss(rec.localtime_s)} change:{IGCFileWriter.s_to_hhmmss(num2)}");
      this.prev_localtime_delta_s = num1;
    }
    this.prev_localtime_s = rec.localtime_s;
  }

  private void process_camera_state(RepeatData rec)
  {
    if (rec.camera_state == this.prev_camera_state)
      return;
    Console.WriteLine($"SimDataConn CAMERA CHANGE from {this.prev_camera_state:0} to {rec.camera_state:0}");
    if (rec.camera_state == 12.0)
    {
      Console.WriteLine("SimDataConn user at World Map so tracklog full reset");
      this.allow_custom_pln_load = true;
      this.trigger(APPEVENT_ID.IGCFileWrite);
      this.flight_data_model.reset();
      this.reset();
    }
    this.prev_camera_state = rec.camera_state;
  }

  private void processSimDateChange(double realtime_s, RepeatData rec)
  {
    if (this.prev_local_year == rec.local_year && this.prev_local_month == rec.local_month && this.prev_local_day == rec.local_day)
      return;
    this.flight_data_model.addLDAT(realtime_s, this.prev_local_year, this.prev_local_month, this.prev_local_day, rec);
    Console.WriteLine($"SimDataConn SIM DATE CHANGED from {this.prev_local_year:0000}-{this.prev_local_month:00}-{this.prev_local_day:00} to {rec.local_year:0000}-{rec.local_month:00}-{rec.local_day:00}");
    this.prev_local_year = rec.local_year;
    this.prev_local_month = rec.local_month;
    this.prev_local_day = rec.local_day;
    this.flight_data_model.sim_local_year = rec.local_year;
    this.flight_data_model.sim_local_month = rec.local_month;
    this.flight_data_model.sim_local_day = rec.local_day;
    this.trigger(APPEVENT_ID.SimDateChange);
  }

  private void processSimRateChange(double realtime_s, RepeatData rec)
  {
    if (this.prev_sim_rate == rec.sim_rate)
      return;
    this.flight_data_model.addRATE(realtime_s, this.prev_sim_rate, rec);
    Console.WriteLine($"SimDataConn SIM RATE CHANGED from {this.prev_sim_rate:0.00} to {rec.sim_rate:0.00}");
    this.flight_data_model.sim_rate = rec.sim_rate;
    this.prev_sim_rate = rec.sim_rate;
    this.trigger(APPEVENT_ID.SimRateChange);
  }

  private void processKADetect(double realtime_s, RepeatData rec)
  {
    if ((this.prev_fold_wing_r != 0.0 || rec.fold_wing_r <= 0.0) && (rec.fold_wing_r != 0.0 || this.prev_fold_wing_r <= 0.0))
      return;
    this.flight_data_model.addKADT(realtime_s, rec.fold_wing_r);
    this.trigger(APPEVENT_ID.KADetected);
    Console.WriteLine($"SimDataConn KA DETECTED from {this.prev_fold_wing_r:0.00} to {rec.fold_wing_r:0.00}");
    this.prev_fold_wing_r = rec.fold_wing_r;
  }

  private void processWeatherChange(double realtime_s, RepeatData rec)
  {
    if (this.prev_weather_hash == rec.weather_hash)
      return;
    this.flight_data_model.addWTHR(realtime_s, rec);
    this.trigger(APPEVENT_ID.WeatherChange);
    Console.WriteLine($"SimDataConn WEATHER CHANGED from {this.prev_weather_hash:0} to {rec.weather_hash:0}" + $" {rec.weather_series:0000} {rec.weather_version:0.00} {rec.weather_preset:000}");
    this.prev_weather_hash = rec.weather_hash;
  }

  private void processFlightCompletion(double realtime_s, RepeatData rec)
  {
    if ((int) rec.sim_on_ground <= 0 || !this.isRecording || rec.world_kph >= 4.0)
      return;
    if (this.flight_data_model.get_length() > 60)
    {
      Console.WriteLine($"SimDataConn triggering an IGCFileWrite {this.flight_data_model.get_length():0} records, igc_task.available = {this.igc_task.available}");
      this.trigger(APPEVENT_ID.IGCFileWrite);
      this.stop_recording();
    }
    else
      this.stop_recording();
  }

  private void start_recording()
  {
    Console.WriteLine("SimDataConn start_recording()");
    this.isRecording = true;
    this.recording_timer.Reset();
    this.recording_timer.Start();
    this.trigger(APPEVENT_ID.RecordingStart);
  }

  private void stop_recording()
  {
    Console.WriteLine("SimDataConn stop_recording()");
    this.isRecording = false;
    this.recording_timer.Reset();
    this.trigger(APPEVENT_ID.RecordingStop);
  }
}
