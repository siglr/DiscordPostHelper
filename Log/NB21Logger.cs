using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using NB21_logger.Properties;

namespace NB21_logger;

public sealed class NB21Logger : IDisposable
{
    private readonly object stateLock = new();
    private readonly Timer watchdogTimer;
    private bool started;
    private bool disposed;
    private bool loggerFailed;
    private bool igcFileWritePending;
    private string? currentIgcFilePath;
    private NB21LoggerState state;
    private SimConnectMessageWindow? messageWindow;

    public NB21Logger(LoggerConfiguration? configuration = null)
    {
        Configuration = (configuration?.Clone()) ?? new LoggerConfiguration();

        ApplyConfigurationToSettings();

        state = new NB21LoggerState
        {
            TracklogsDirectory = Configuration.TracklogsDirectory
        };

        app_event_handler = HandleAppEvent;
        ws_connected = () => { };
        ws_disconnected = () => { };

        simdata = new SimDataConn(this);
        igc_file_writer = new IGCFileWriter(simdata);
        nb21_web_server = new NB21WebServer(this);
        nb21_ws_server = new NB21WSServer(this);

        watchdogTimer = new Timer(Watchdog, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public LoggerConfiguration Configuration { get; }

    public string AppName => Configuration.AppName;

    public string AppVersion => Configuration.AppVersion;

    public SimDataConn simdata { get; }

    public IGCFileWriter igc_file_writer { get; }

    public NB21WebServer nb21_web_server { get; }

    public NB21WSServer nb21_ws_server { get; }

    public EventHandler<AppEventArgs> app_event_handler { get; }

    public delegate void WsConnectDelegate();

    public WsConnectDelegate ws_connected { get; }

    public WsConnectDelegate ws_disconnected { get; }

    public NB21LoggerState State
    {
        get
        {
            lock (stateLock)
            {
                return state;
            }
        }
    }

    public event EventHandler<AppEventArgs>? AppEvent;

    public event EventHandler<LoggerStateChangedEventArgs>? StateChanged;

    public event EventHandler<IgcFileFinalizedEventArgs>? IgcFileFinalized;

    public event EventHandler<WebSocketClientEventArgs>? WebSocketClientConnected;

    public event EventHandler<WebSocketClientEventArgs>? WebSocketClientDisconnected;

    public void Start()
    {
        ThrowIfDisposed();
        if (started)
        {
            return;
        }

        started = true;
        ApplyConfigurationToSettings();
        EnsureTracklogsFolder();
        EnsureSimConnectWindow();
        StartServers();
        AppReset();
        watchdogTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    public void Stop()
    {
        if (!started)
        {
            return;
        }

        started = false;
        watchdogTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        nb21_web_server.stop();
        nb21_ws_server.stop();
        simdata.Disconnect();
        messageWindow?.Dispose();
        messageWindow = null;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        Stop();
        watchdogTimer.Dispose();
        disposed = true;
        GC.SuppressFinalize(this);
    }

    public void SetFlightPlanFromFile(string flightPlanPath)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(flightPlanPath))
        {
            throw new ArgumentException("Flight plan path cannot be empty", nameof(flightPlanPath));
        }

        string plnXml = File.ReadAllText(flightPlanPath);
        SetFlightPlan(plnXml);
    }

    public void SetFlightPlan(string flightPlanXml)
    {
        ThrowIfDisposed();
        if (string.IsNullOrWhiteSpace(flightPlanXml))
        {
            throw new ArgumentException("Flight plan XML cannot be empty", nameof(flightPlanXml));
        }

        simdata.load_pln_set_str(flightPlanXml);
    }

    public void ClearFlightPlan()
    {
        ThrowIfDisposed();
        simdata.igc_task.reset();
        simdata.trigger(APPEVENT_ID.HeaderUpdate);
    }

    internal void OnAppEvent(AppEventArgs args)
    {
        ProcessAppEvent(args);
        AppEvent?.Invoke(this, args);
    }

    internal void NotifyWebSocketConnected(bool isJsonChannel)
    {
        WebSocketClientConnected?.Invoke(this, new WebSocketClientEventArgs(isJsonChannel));
    }

    internal void NotifyWebSocketDisconnected(bool isJsonChannel)
    {
        WebSocketClientDisconnected?.Invoke(this, new WebSocketClientEventArgs(isJsonChannel));
    }

    public object? Invoke(Delegate method)
    {
        return Invoke(method, null);
    }

    public object? Invoke(Delegate method, object?[]? args)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        return method.DynamicInvoke(args);
    }

    public void send_clients_IGC(string msg)
    {
        nb21_ws_server.send_IGC(msg);
    }

    public void send_clients_JSON(string msg)
    {
        nb21_ws_server.send_JSON(msg);
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(nameof(NB21Logger));
        }
    }

    private void Watchdog(object? stateObj)
    {
        if (!started || loggerFailed)
        {
            return;
        }

        try
        {
            simdata.CheckConnect();
        }
        catch (FileNotFoundException ex) when (ex.FileName?.Contains("SimConnect", StringComparison.OrdinalIgnoreCase) == true)
        {
            AppFailure("Missing SimConnect DLLs");
        }
        catch (FileNotFoundException)
        {
            AppFailure("Failed loading MSFS connect.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Watchdog unexpected error: " + ex);
        }

        if (igcFileWritePending && Directory.Exists(Configuration.TracklogsDirectory))
        {
            igcFileWritePending = false;
            igc_file_write();
        }
    }

    private void EnsureSimConnectWindow()
    {
        if (messageWindow != null)
        {
            return;
        }

        var window = new SimConnectMessageWindow(simdata);
        window.Start();
        simdata.SetWindowHandle(window.Handle);
        messageWindow = window;
    }

    private void StartServers()
    {
        nb21_ws_server.start(Configuration.WebSocketPort);
        var prefixes = BuildPrefixes();
        try
        {
            nb21_web_server.start(prefixes);
        }
        catch (HttpListenerException ex) when (ex.ErrorCode == 5)
        {
            nb21_web_server.start(new List<string>
            {
                $"http://localhost:{Configuration.WebServerPort}/"
            });
        }
    }

    private List<string> BuildPrefixes()
    {
        var prefixes = new List<string>
        {
            $"http://localhost:{Configuration.WebServerPort}/"
        };

        string? localIp = TryGetLocalIp();
        if (!string.IsNullOrWhiteSpace(localIp))
        {
            prefixes.Insert(0, $"http://{localIp}:{Configuration.WebServerPort}/");
        }

        return prefixes;
    }

    private static string? TryGetLocalIp()
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
        catch (SocketException ex)
        {
            Console.WriteLine("Unable to resolve local IP address: " + ex.Message);
        }

        return null;
    }

    private void HandleAppEvent(object? sender, AppEventArgs e)
    {
        ProcessAppEvent(e);
    }

    private void ProcessAppEvent(AppEventArgs e)
    {
        switch (e.eventType)
        {
            case APPEVENT_ID.SimOpen:
                Console.WriteLine("APPEVENT SimOpen " + e.arg_str);
                AppReset();
                EnsureTracklogsFolder();
                UpdateState(e.eventType, s => s.With(
                    connectionStatus: $"Connected to {simdata.get_msfs_text()}",
                    isConnected: true,
                    tracklogsDirectory: Configuration.TracklogsDirectory));
                break;
            case APPEVENT_ID.SimStop:
                Console.WriteLine("APPEVENT SimStop");
                break;
            case APPEVENT_ID.SimQuit:
                Console.WriteLine("APPEVENT SimQuit");
                UpdateState(e.eventType, s => s.With(
                    connectionStatus: "Waiting for MSFS.",
                    isConnected: false,
                    isRecording: false));
                break;
            case APPEVENT_ID.SimHasCrashed:
                Console.WriteLine("APPEVENT SimHasCrashed");
                handle_sim_crash();
                break;
            case APPEVENT_ID.RecordingStart:
                Console.WriteLine("APPEVENT RecordingStart");
                recording_has_started();
                break;
            case APPEVENT_ID.RecordingStop:
                Console.WriteLine("APPEVENT RecordingStop");
                recording_has_stopped();
                break;
            case APPEVENT_ID.IGCFileWrite:
                Console.WriteLine("APPEVENT IGCFileWrite");
                igc_file_write();
                break;
            case APPEVENT_ID.IGCFileReset:
                Console.WriteLine("APPEVENT IGCFileReset");
                ResetIgcState();
                break;
            case APPEVENT_ID.SimDateChange:
                Console.WriteLine("APPEVENT SimDateChange");
                handle_sim_date_change(e.eventType);
                break;
            case APPEVENT_ID.SimTimeTick:
                handle_sim_date_change(e.eventType);
                break;
            case APPEVENT_ID.SimRateChange:
                Console.WriteLine($"APPEVENT SimRateChange {simdata.flight_data_model.sim_rate:000.000}");
                handle_sim_rate_change();
                break;
            case APPEVENT_ID.HeaderUpdate:
                Console.WriteLine("APPEVENT HeaderUpdate");
                handle_header_update();
                break;
            case APPEVENT_ID.HttpError:
                Console.WriteLine("APPEVENT HttpError");
                handle_http_error();
                break;
            default:
                break;
        }
    }

    private void AppReset()
    {
        loggerFailed = false;
        igcFileWritePending = false;
        simdata.app_init();
    }

    private void AppFailure(string message)
    {
        loggerFailed = true;
        Console.WriteLine(message);
        UpdateState(APPEVENT_ID.HttpError, s => s.With(
            connectionStatus: message,
            isConnected: false));
    }

    private void EnsureTracklogsFolder()
    {
        if (!Directory.Exists(Configuration.TracklogsDirectory))
        {
            Directory.CreateDirectory(Configuration.TracklogsDirectory);
        }
    }

    private void ApplyConfigurationToSettings()
    {
        var settings = Settings.Default;
        settings.PilotName = Configuration.PilotName;
        settings.PilotId = Configuration.PilotId;
        settings.IGCPath = Configuration.TracklogsDirectory;

        if (!string.IsNullOrWhiteSpace(Configuration.AppName))
        {
            settings[nameof(Settings.AppName)] = Configuration.AppName;
        }

        if (!string.IsNullOrWhiteSpace(Configuration.AppVersion))
        {
            settings[nameof(Settings.AppVersion)] = Configuration.AppVersion;
        }
    }

    private void handle_sim_crash()
    {
        UpdateState(APPEVENT_ID.SimHasCrashed, s => s.With(connectionStatus: "Sim Crash", isRecording: false));
    }

    private void handle_http_error()
    {
        UpdateState(APPEVENT_ID.HttpError, s => s.With(connectionStatus: "HTTP Error"));
    }

    private void handle_header_update()
    {
        string? aircraft = simdata.igc_aircraft.get_title();
        string? flightPlanName = null;
        if (simdata.igc_task.available)
        {
            flightPlanName = simdata.igc_task.title ?? simdata.igc_task.name;
        }

        nb21_web_server.set_header_data_IGC(igc_file_writer.get_header_IGC());
        string headerJson = simdata.get_header_JSON();
        nb21_web_server.set_header_data_JSON(headerJson);
        send_clients_JSON(headerJson);
        nb21_web_server.set_repeat_data_IGC(IGCFileWriter.I_RECORD);
        send_clients_IGC(IGCFileWriter.I_RECORD);

        UpdateState(APPEVENT_ID.HeaderUpdate, s => s.With(
            aircraftTitle: aircraft,
            flightPlanName: flightPlanName));
    }

    private void handle_sim_rate_change()
    {
        string status = State.IsRecording ? "Recording" : State.ConnectionStatus;
        if (State.IsRecording)
        {
            status = $"Recording x{simdata.flight_data_model.sim_rate:0.00}";
        }

        UpdateState(APPEVENT_ID.SimRateChange, s => s.With(connectionStatus: status));
    }

    private void handle_sim_date_change(APPEVENT_ID eventId)
    {
        var utcTime = TryGetSimUtc();
        UpdateState(eventId, s => s.With(simTimeUtc: utcTime));
    }

    private DateTime? TryGetSimUtc()
    {
        var model = simdata.flight_data_model;
        if (model.sim_local_year == 0.0 || model.sim_local_month == 0.0 || model.sim_local_day == 0.0)
        {
            return null;
        }

        double seconds = simdata.localtime_s;
        int hours = (int)(seconds / 3600.0) % 24;
        int minutes = (int)((seconds % 3600.0) / 60.0);
        int secs = (int)(seconds % 60.0);

        try
        {
            return new DateTime(
                (int)model.sim_local_year,
                (int)model.sim_local_month,
                (int)model.sim_local_day,
                hours,
                minutes,
                secs,
                DateTimeKind.Utc);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    private void recording_has_started()
    {
        simdata.recording_timer.Reset();
        simdata.recording_timer.Start();
        UpdateState(APPEVENT_ID.RecordingStart, s => s.With(
            connectionStatus: "Recording",
            isRecording: true,
            recordingElapsed: simdata.recording_timer.Elapsed));
    }

    private void recording_has_stopped()
    {
        simdata.recording_timer.Stop();
        UpdateState(APPEVENT_ID.RecordingStop, s => s.With(
            connectionStatus: $"Connected to {simdata.get_msfs_text()}",
            isRecording: false,
            recordingElapsed: simdata.recording_timer.Elapsed));
    }

    private void igc_file_write()
    {
        try
        {
            int length = simdata.flight_data_model.get_length();
            int writtenLength = simdata.flight_data_model.get_written_length();
            int delta = length - writtenLength;
            Console.WriteLine($"igc_file_write() {length} records with {delta} new records.");
            if (delta < 60)
            {
                Console.WriteLine("igc_file_write() short tracklog so ignoring file write.");
                return;
            }

            ResetIgcState();
            EnsureTracklogsFolder();
            currentIgcFilePath = igc_file_writer.write_file();
            if (!string.IsNullOrEmpty(currentIgcFilePath))
            {
                simdata.flight_data_model.set_written_length(length);
                igcFileWritePending = false;
                IgcFileFinalized?.Invoke(this, new IgcFileFinalizedEventArgs(currentIgcFilePath));
                UpdateState(APPEVENT_ID.IGCFileWrite, s => s.With(lastIgcFilePath: currentIgcFilePath));
            }
            else
            {
                igcFileWritePending = true;
                UpdateState(APPEVENT_ID.IGCWriteError, s => s.With(lastIgcFilePath: null));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("igc_file_write() error: " + ex);
            igcFileWritePending = true;
            UpdateState(APPEVENT_ID.IGCWriteError, s => s.With(lastIgcFilePath: null));
        }
    }

    private void ResetIgcState()
    {
        currentIgcFilePath = null;
        UpdateState(APPEVENT_ID.IGCFileReset, s => s.With(lastIgcFilePath: null));
    }

    private void UpdateState(APPEVENT_ID eventId, Func<NB21LoggerState, NB21LoggerState> updater)
    {
        NB21LoggerState newState;
        lock (stateLock)
        {
            newState = updater(state);
            state = newState;
        }

        StateChanged?.Invoke(this, new LoggerStateChangedEventArgs(eventId, newState));
    }
}
