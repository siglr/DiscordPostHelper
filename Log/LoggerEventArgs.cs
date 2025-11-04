using System;

namespace NB21_logger;

public sealed class LoggerStateChangedEventArgs : EventArgs
{
    public LoggerStateChangedEventArgs(APPEVENT_ID eventType, NB21LoggerState state)
    {
        EventType = eventType;
        State = state;
    }

    public APPEVENT_ID EventType { get; }

    public NB21LoggerState State { get; }
}

public sealed class IgcFileFinalizedEventArgs : EventArgs
{
    public IgcFileFinalizedEventArgs(string filePath)
    {
        FilePath = filePath;
    }

    public string FilePath { get; }
}

public sealed class WebSocketClientEventArgs : EventArgs
{
    public WebSocketClientEventArgs(bool isJson)
    {
        IsJsonChannel = isJson;
    }

    public bool IsJsonChannel { get; }
}
