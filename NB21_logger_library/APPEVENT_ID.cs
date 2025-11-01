// Decompiled with JetBrains decompiler
// Type: NB21_logger.APPEVENT_ID
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

#nullable disable
namespace NB21_logger;

public enum APPEVENT_ID
{
  SimOpen,
  SimStop,
  SimQuit,
  SimHasCrashed,
  RecordingStart,
  RecordingStop,
  Slew,
  EngineOn,
  Takeoff,
  Landing,
  IGCFileWrite,
  IGCFileReset,
  SimTimeChange,
  SimDateChange,
  SimRateChange,
  SimTimeTick,
  HeaderUpdate,
  IGCWriteError,
  Pause,
  Weight,
  Pause_1,
  LoadAircraft,
  PlaneCrashed,
  PCClockChange,
  KADetected,
  WeatherChange,
  PositionChanged,
  HttpError,
}
