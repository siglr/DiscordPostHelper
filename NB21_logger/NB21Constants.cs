// Decompiled with JetBrains decompiler
// Type: NB21_logger.NB21Constants
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

#nullable enable
namespace NB21_logger;

public static class NB21Constants
{
  public const bool DEVMODE = true;
  public const double M_TO_FEET = 3.28084;
  public const double M_TO_MILES = 0.000621371;
  public const double MS_TO_KPH = 3.6;
  public const double MS_TO_KNOTS = 1.94384;
  public const string IGC_MANUFACTURER = "NB21";
  public const int WATCHDOG_TIMER_INTERVAL_MS = 5000;
  public const int BLINKING_TIMER_INTERVAL_MS = 1000;
  public const int MAX_REPEATING_RECORDS = 200000;
  public const int MAX_VARS_PER_RECORD = 14;
  public const int MIN_RECORDS_FOR_IGC_WRITE = 60;
  public const double DEFAULT_RADIUS_M = 500.0;
  public const double DEFAULT_START_RADIUS_M = 2500.0;
  public const double DEFAULT_FINISH_RADIUS_M = 2000.0;
  public const int HttpPort = 54178;
  public const int WsPort = 54179;
  public const string WsPathIGC = "/NB21_logger";
  public const string WsPathJSON = "/NB21_logger_json";
  public const string DEVMODE_ROOT = "G:\\ian_lewis\\src\\flightsim";
}
