// Decompiled with JetBrains decompiler
// Type: NB21_logger.RepeatData
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System.Runtime.InteropServices;

#nullable disable
namespace NB21_logger;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RepeatData
{
  public double localtime_s;
  public double lat;
  public double lon;
  public double alt_m;
  public double slew;
  public double sim_on_ground;
  public double gear_on_ground;
  public double tas_kph;
  public double world_kph;
  public double combustion;
  public double thrust_lbs;
  public double flap_pos;
  public double weight_kg;
  public double wingspan_m;
  public double local_year;
  public double local_month;
  public double local_day;
  public double wind_x_ms;
  public double wind_y_ms;
  public double wind_z_ms;
  public double alt_agl_m;
  public double sim_rate;
  public double fold_wing_r;
  public double weather_hash;
  public double weather_series;
  public double weather_version;
  public double weather_preset;
  public double abs_time_s;
  public double sim_time_s;
  public double hdg_true_deg;
  public double gps_ground_track_rad;
  public double master_bat;
  public double gps_ground_speed_ms;
  public double airspeed_ind_ms;
  public double ground_alt_m;
  public double ambient_temp_k;
  public double ambient_pressure_mb;
  public double velocity_world_z_fps;
  public double velocity_world_x_fps;
  public double gear_down;
  public double maccready_ms;
  public double camera_state;
}
