// Decompiled with JetBrains decompiler
// Type: NB21_logger.NB21WebServer
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace NB21_logger;

public class NB21WebServer
{
  private static HttpListener? listener;
  private static int RequestNumber = 0;
  private static readonly DateTime StartupDate = DateTime.UtcNow;
  private bool devmode;
  private NB21Logger logger;
  private string repeat_data_IGC = "LNB21 000000 INIT";
  private string header_data_IGC = "LNB21 000000 INIT";
  private string repeat_data_JSON = "{ \"ver\": \"v2\", \"msg\": \"repeat\"}";
  private string header_data_JSON = "{ \"ver\": \"v2\", \"msg\": \"header\"}";

  public NB21WebServer(NB21Logger logger)
  {
    this.logger = logger;
    if (HttpListener.IsSupported)
      return;
    Console.WriteLine("HttpListener is not supported on this platform.");
  }

  public void start(List<string> web_addresses)
  {
    NB21WebServer.listener = new HttpListener();
    for (int index = 0; index < web_addresses.Count; ++index)
      NB21WebServer.listener.Prefixes.Add(web_addresses[index]);
    NB21WebServer.listener.Start();
    NB21WebServer.listener.BeginGetContext(new AsyncCallback(this.GetContextCallback), (object) null);
    Console.WriteLine("Webserver Started.");
  }

  public void stop() => NB21WebServer.listener?.Stop();

  public void set_repeat_data_IGC(string repeat_data) => this.repeat_data_IGC = repeat_data;

  public void set_repeat_data_JSON(string repeat_data) => this.repeat_data_JSON = repeat_data;

  public void set_header_data_IGC(string header_data) => this.header_data_IGC = header_data;

  public void set_header_data_JSON(string header_data) => this.header_data_JSON = header_data;

  private string content_type(string path)
  {
    if (path.EndsWith(".js"))
      return "text/javascript";
    if (path.EndsWith(".css"))
      return "text/css";
    if (path.EndsWith(".png"))
      return "image/png";
    if (path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
      return "image/jpeg";
    if (path.EndsWith(".html"))
      return "text/html";
    if (path.EndsWith(".svg"))
      return "image/svg+xml";
    if (path.EndsWith(".ico"))
      return "image/vnd.microsoft.icon";
    if (path.EndsWith(".json"))
      return "application/json";
    if (path.EndsWith(".xml"))
      return "application/xml";
    path.EndsWith(".txt");
    return "text/plain";
  }

  private bool requesting_index(string request_url)
  {
    return request_url.ToLower() == "/b21_task_planner" || request_url.ToLower() == "/b21_task_planner/" || request_url.ToLower().StartsWith("/b21_task_planner/index.html");
  }

  private void GetContextCallback(IAsyncResult ar)
  {
    ++NB21WebServer.RequestNumber;
    if (NB21WebServer.listener == null)
      return;
    try
    {
      HttpListenerContext context = NB21WebServer.listener.EndGetContext(ar);
      NB21WebServer.listener.BeginGetContext(new AsyncCallback(this.GetContextCallback), (object) null);
      DateTime utcNow = DateTime.UtcNow;
      Console.WriteLine("NB21WebServer request: " + context.Request.RawUrl);
      byte[] buffer = Encoding.UTF8.GetBytes("404 not found");
      int num = 404;
      HttpListenerResponse response = context.Response;
      response.ContentType = "text/html";
      string rawUrl = context.Request.RawUrl;
      if (rawUrl != null)
      {
        if (rawUrl.ToLower().StartsWith("/httperror"))
          throw new InvalidOperationException("HTTP Error");
        if (rawUrl.ToLower().StartsWith("/repeatdata_json"))
        {
          response.ContentType = this.content_type(".json");
          buffer = Encoding.UTF8.GetBytes(this.repeat_data_JSON);
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/repeatdata"))
        {
          buffer = Encoding.UTF8.GetBytes(this.repeat_data_IGC);
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/headerdata_json"))
        {
          if (this.header_data_JSON == "")
            this.header_data_JSON = FormattableString.Invariant(FormattableStringFactory.Create("{{ \"ver\": \"v2\", \"msg\": \"header\", \"logger_title\": \"{0} {1}\"}}", (object) this.logger.AppName, (object) this.logger.AppVersion));
          response.ContentType = this.content_type(".json");
          buffer = Encoding.UTF8.GetBytes(this.header_data_JSON);
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/headerdata"))
        {
          buffer = Encoding.UTF8.GetBytes(this.header_data_IGC);
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/i_record"))
        {
          buffer = Encoding.UTF8.GetBytes("I073638FXA3943AGL4447TAS4851NET5254ENL5555FLP5658WSP5961WDI6262GND6363GEA");
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/pln_set"))
        {
          HttpListenerRequest request = context.Request;
          if (!request.HasEntityBody)
          {
            Console.WriteLine("NB21WebServer pln_set request no EntityBody: " + context.Request.RawUrl);
            buffer = Encoding.UTF8.GetBytes("NOK");
            num = 200;
          }
          else
          {
            using (Stream inputStream = request.InputStream)
            {
              using (StreamReader streamReader = new StreamReader(inputStream, request.ContentEncoding))
                this.logger.simdata.load_pln_set_str(streamReader.ReadToEnd());
            }
            buffer = Encoding.UTF8.GetBytes("OK");
            num = 200;
          }
        }
        else if (rawUrl.ToLower().StartsWith("/pln_get") || rawUrl.ToLower().StartsWith("/pln_xml"))
        {
          Console.WriteLine("NB21WebServer pln_xml");
          response.ContentType = this.content_type(".xml");
          buffer = Encoding.UTF8.GetBytes(this.logger.simdata.get_flightplan_XML());
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/pln_json"))
        {
          Console.WriteLine("NB21WebServer pln_json");
          response.ContentType = this.content_type(".json");
          buffer = Encoding.UTF8.GetBytes(this.logger.simdata.get_flightplan_JSON());
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/pln_igc"))
        {
          Console.WriteLine("NB21WebServer pln_igc");
          response.ContentType = this.content_type(".txt");
          buffer = Encoding.UTF8.GetBytes(this.logger.simdata.get_flightplan_IGC());
          num = 200;
        }
        else if (rawUrl.ToLower().StartsWith("/pln_check"))
        {
          Console.WriteLine("NB21WebServer pln_check");
          string s = FormattableString.Invariant(FormattableStringFactory.Create("{{\"ver\": \"v2\", \"msg\": \"pln_check\",\"pln_ref_id\": {0}}}", (object) this.logger.simdata.igc_task.pln_ref_id));
          response.ContentType = this.content_type(".json");
          buffer = Encoding.UTF8.GetBytes(s);
          num = 200;
        }
        else if (rawUrl.Contains("?devmode") || this.devmode)
        {
          try
          {
            if (this.requesting_index(rawUrl))
            {
              if (!rawUrl.Contains("index.html?devmode"))
              {
                Console.WriteLine("NB21WebServer resetting devmode");
                this.devmode = false;
                buffer = System.IO.File.ReadAllBytes("b21_task_planner/index.html");
                num = 200;
              }
              else
              {
                Console.WriteLine("NB21WebServer setting devmode");
                this.devmode = true;
              }
            }
            if (this.devmode)
            {
              string str1 = "G:\\ian_lewis\\src\\flightsim" + rawUrl.Replace('/', '\\');
              int length = str1.IndexOf("?");
              if (length > 0)
                str1 = str1.Substring(0, length);
              string lower = str1.ToLower();
              string str2 = this.content_type(lower);
              Console.WriteLine($"Serving web {str2} content from '{lower}'");
              buffer = System.IO.File.ReadAllBytes(lower);
              num = 200;
              response.ContentType = str2;
            }
          }
          catch (FileNotFoundException ex)
          {
            buffer = Encoding.UTF8.GetBytes("NB21 Logger required file not found.");
            num = 404;
          }
          catch (Exception ex)
          {
            buffer = Encoding.UTF8.GetBytes($"NB21 Logger unexpected exception: {ex.Message}.");
            num = 404;
          }
        }
        else if (this.requesting_index(rawUrl))
        {
          buffer = System.IO.File.ReadAllBytes("b21_task_planner/index.html");
          num = 200;
        }
        else if (rawUrl.ToLower() == "/favicon.ico")
        {
          buffer = System.IO.File.ReadAllBytes("b21_task_planner/favicon.ico");
          response.ContentType = "image/vnd.microsoft.icon";
          num = 200;
        }
      }
      response.Headers.Add("Access-Control-Allow-Origin", "*");
      response.Headers.Add("Access-Control-Allow-Headers", "*");
      response.Headers.Add("Access-Control-Allow-Methods", "POST,GET");
      response.ContentLength64 = (long) buffer.Length;
      response.StatusCode = num;
      response.OutputStream.Write(buffer, 0, buffer.Length);
      response.OutputStream.Close();
    }
    catch (Exception ex)
    {
      Console.WriteLine("NB21WebServer.GetContextCallback() exception:");
      Console.WriteLine(ex.ToString());
      this.logger.simdata.trigger(APPEVENT_ID.HttpError);
    }
  }
}
