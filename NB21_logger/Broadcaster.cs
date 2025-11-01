// Decompiled with JetBrains decompiler
// Type: NB21_logger.Broadcaster
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using WebSocketSharp;
using WebSocketSharp.Server;

#nullable enable
namespace NB21_logger;

public class Broadcaster : WebSocketBehavior
{
  public NB21Logger? logger;
  public bool json;

  protected override void OnOpen()
  {
    this.logger?.Invoke((Delegate) this.logger.ws_connected);
    base.OnOpen();
    Console.WriteLine("Broadcaster websocket open." + (this.json ? " (json)" : " (igc)"));
    if (this.logger == null)
      return;
    if (this.json)
      this.Send(this.logger.simdata.get_header_JSON());
    else
      this.Send("I073638FXA3943AGL4447TAS4851NET5254ENL5555FLP5658WSP5961WDI6262GND6363GEA");
  }

  protected override void OnError(ErrorEventArgs e)
  {
    Console.WriteLine("Broadcaster websocket error. " + e.Message);
    base.OnError(e);
  }

  protected override void OnClose(CloseEventArgs args)
  {
    this.logger?.Invoke((Delegate) this.logger.ws_disconnected);
    Console.WriteLine("Broadcaster websocket closed.");
    base.OnClose(args);
  }

  protected override void OnMessage(MessageEventArgs e)
  {
    int length = e.Data.IndexOf(';');
    if (length == -1)
      Console.WriteLine("Websocket bad command: " + e.Data);
    else
      Console.WriteLine($"Websocket received command: '{e.Data.Substring(0, length).ToLower().TrimStart()}'");
  }
}
