// Decompiled with JetBrains decompiler
// Type: NB21_logger.NB21WSServer
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using WebSocketSharp.Server;

#nullable enable
namespace NB21_logger;

public class NB21WSServer
{
  private NB21Logger logger;
  private WebSocketServer ws_svr;

  public NB21WSServer(NB21Logger logger)
  {
    this.logger = logger;
    this.ws_svr = new WebSocketServer();
  }

  public void send_IGC(string msg)
  {
    try
    {
      this.ws_svr.WebSocketServices["/NB21_logger"].Sessions.Broadcast(msg);
    }
    catch (Exception ex)
    {
      Console.WriteLine("NB21WSServer send_IGC() Exception " + ex.ToString());
    }
  }

  public void send_JSON(string msg)
  {
    try
    {
      this.ws_svr.WebSocketServices["/NB21_logger_json"].Sessions.Broadcast(msg);
    }
    catch (Exception ex)
    {
      Console.WriteLine("NB21WSServer send_JSON() Exception " + ex.ToString());
    }
  }

  public void start(int port)
  {
    Console.WriteLine($"Starting WS Server on port {port}");
    this.ws_svr = new WebSocketServer(port, false);
    Console.WriteLine($"WS Server listening on {port}{"/NB21_logger"}");
    this.ws_svr.AddWebSocketService<Broadcaster>("/NB21_logger", (Action<Broadcaster>) (b =>
    {
      b.logger = this.logger;
      b.json = false;
    }));
    Console.WriteLine($"WS Server listening on {port}{"/NB21_logger_json"}");
    this.ws_svr.AddWebSocketService<Broadcaster>("/NB21_logger_json", (Action<Broadcaster>) (b =>
    {
      b.logger = this.logger;
      b.json = true;
    }));
    this.ws_svr.Start();
    Console.WriteLine("WS Server started.");
  }

  public void stop()
  {
    Console.WriteLine("Stopping WS Server");
    this.ws_svr.Stop();
  }
}
