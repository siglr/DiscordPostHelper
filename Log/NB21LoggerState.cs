using System;

namespace NB21_logger;

public sealed class NB21LoggerState
{
    public string ConnectionStatus { get; set; } = "Waiting for MSFS.";

    public bool IsConnected { get; set; }

    public bool IsRecording { get; set; }

    public TimeSpan RecordingElapsed { get; set; }

    public DateTime? SimTimeUtc { get; set; }

    public string? AircraftTitle { get; set; }

    public string? FlightPlanName { get; set; }

    public string? LastIgcFilePath { get; set; }

    public string TracklogsDirectory { get; set; } = string.Empty;

    public NB21LoggerState With(
        string? connectionStatus = null,
        bool? isConnected = null,
        bool? isRecording = null,
        TimeSpan? recordingElapsed = null,
        DateTime? simTimeUtc = null,
        string? aircraftTitle = null,
        string? flightPlanName = null,
        string? lastIgcFilePath = null,
        string? tracklogsDirectory = null)
    {
        return new NB21LoggerState
        {
            ConnectionStatus = connectionStatus ?? ConnectionStatus,
            IsConnected = isConnected ?? IsConnected,
            IsRecording = isRecording ?? IsRecording,
            RecordingElapsed = recordingElapsed ?? RecordingElapsed,
            SimTimeUtc = simTimeUtc ?? SimTimeUtc,
            AircraftTitle = aircraftTitle ?? AircraftTitle,
            FlightPlanName = flightPlanName ?? FlightPlanName,
            LastIgcFilePath = lastIgcFilePath ?? LastIgcFilePath,
            TracklogsDirectory = tracklogsDirectory ?? TracklogsDirectory,
        };
    }
}
