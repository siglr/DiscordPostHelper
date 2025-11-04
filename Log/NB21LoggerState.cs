using System;

namespace NB21_logger;

public sealed class NB21LoggerState
{
    public string ConnectionStatus { get; init; } = "Waiting for MSFS.";

    public bool IsConnected { get; init; }

    public bool IsRecording { get; init; }

    public TimeSpan RecordingElapsed { get; init; }

    public DateTime? SimTimeUtc { get; init; }

    public string? AircraftTitle { get; init; }

    public string? FlightPlanName { get; init; }

    public string? LastIgcFilePath { get; init; }

    public string TracklogsDirectory { get; init; } = string.Empty;

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
