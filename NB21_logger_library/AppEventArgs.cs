// Decompiled with JetBrains decompiler
// Type: NB21_logger.AppEventArgs
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;

#nullable enable
namespace NB21_logger;

public class AppEventArgs : EventArgs
{
  public string arg_str = "";

  public AppEventArgs(APPEVENT_ID event_type, string s = "")
  {
    this.eventType = event_type;
    this.arg_str = s;
  }

  public APPEVENT_ID eventType { get; set; }
}
