using System;
using System.IO;

namespace NB21_logger;

public sealed class LoggerConfiguration
{
    private string tracklogsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "NB21_logger",
        "Flights");

    public string AppName { get; set; } = "NB21 Logger";

    public string AppVersion { get; set; } = "1.2.3";

    public string PilotName { get; set; } = string.Empty;

    public string PilotId { get; set; } = string.Empty;

    public string TracklogsDirectory
    {
        get => tracklogsDirectory;
        set => tracklogsDirectory = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Tracklogs directory cannot be empty", nameof(value))
            : value;
    }

    public int WebServerPort { get; set; } = 54178;

    public int WebSocketPort { get; set; } = 54179;

    public LoggerConfiguration Clone()
    {
        return (LoggerConfiguration)MemberwiseClone();
    }
}
