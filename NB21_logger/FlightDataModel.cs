// Decompiled with JetBrains decompiler
// Type: NB21_logger.FlightDataModel
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.Collections.Generic;

#nullable enable
namespace NB21_logger;

public class FlightDataModel
{
  public List<string>? igc_strings;
  public double sim_local_year;
  public double sim_local_month;
  public double sim_local_day;
  public double sim_rate;
  public DateTime? utc_begin;
  public double[] wind_count_agl = new double[24];
  public double[] wind_count_msl = new double[24];
  public double[] wind_kph_agl = new double[24];
  public double[] wind_deg_agl = new double[24];
  public double[] wind_kph_msl = new double[24];
  public double[] wind_deg_msl = new double[24];
  public FlightDataModel.IGCRecord[] repeat_records = new FlightDataModel.IGCRecord[200000];
  public int repeat_records_length;
  public int written_records_length;
  public readonly Dictionary<FlightDataModel.IGC_RECORD_TYPE, Dictionary<FlightDataModel.IGC_VALUE_NAME, int>> RECORD_VAR_INDEX = new Dictionary<FlightDataModel.IGC_RECORD_TYPE, Dictionary<FlightDataModel.IGC_VALUE_NAME, int>>()
  {
    {
      FlightDataModel.IGC_RECORD_TYPE.B,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.lat_deg,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.lon_deg,
          2
        },
        {
          FlightDataModel.IGC_VALUE_NAME.alt_m,
          3
        },
        {
          FlightDataModel.IGC_VALUE_NAME.alt_agl_m,
          4
        },
        {
          FlightDataModel.IGC_VALUE_NAME.tas_kph,
          5
        },
        {
          FlightDataModel.IGC_VALUE_NAME.engine,
          6
        },
        {
          FlightDataModel.IGC_VALUE_NAME.flap_pos,
          7
        },
        {
          FlightDataModel.IGC_VALUE_NAME.on_ground,
          8
        },
        {
          FlightDataModel.IGC_VALUE_NAME.wind_y_ms,
          9
        },
        {
          FlightDataModel.IGC_VALUE_NAME.wind_kph,
          10
        },
        {
          FlightDataModel.IGC_VALUE_NAME.wind_deg,
          11
        },
        {
          FlightDataModel.IGC_VALUE_NAME.gear_down,
          12
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.SLEW,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.alt_m,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.slew,
          2
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.LTIM,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.localtime_s,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.prev_localtime_s,
          2
        },
        {
          FlightDataModel.IGC_VALUE_NAME.delta_s,
          3
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.PTIM,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.prev_realtime_s,
          1
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.LDAT,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.local_year,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.local_month,
          2
        },
        {
          FlightDataModel.IGC_VALUE_NAME.local_day,
          3
        },
        {
          FlightDataModel.IGC_VALUE_NAME.prev_local_year,
          4
        },
        {
          FlightDataModel.IGC_VALUE_NAME.prev_local_month,
          5
        },
        {
          FlightDataModel.IGC_VALUE_NAME.prev_local_day,
          6
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.RATE,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.sim_rate,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.prev_sim_rate,
          2
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.ENG,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.engine,
          1
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.PAUS,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.pause,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.delta_s,
          2
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.TOTW,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.weight_kg,
          1
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.LDAC,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.igc_string_index,
          1
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.TOFF,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.LAND,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.KADT,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.fold_wing_r,
          1
        }
      }
    },
    {
      FlightDataModel.IGC_RECORD_TYPE.WTHR,
      new Dictionary<FlightDataModel.IGC_VALUE_NAME, int>()
      {
        {
          FlightDataModel.IGC_VALUE_NAME.realtime_s,
          0
        },
        {
          FlightDataModel.IGC_VALUE_NAME.weather_hash,
          1
        },
        {
          FlightDataModel.IGC_VALUE_NAME.weather_series,
          2
        },
        {
          FlightDataModel.IGC_VALUE_NAME.weather_version,
          3
        },
        {
          FlightDataModel.IGC_VALUE_NAME.weather_preset,
          4
        }
      }
    }
  };

  public FlightDataModel()
  {
    this.sim_local_year = 0.0;
    this.sim_local_month = 0.0;
    this.sim_local_day = 0.0;
    this.sim_rate = 1.0;
    this.reset();
  }

  public void reset()
  {
    Console.WriteLine("FlightDataModel.reset()");
    this.repeat_records_length = 0;
    this.written_records_length = 0;
    this.utc_begin = new DateTime?(DateTime.UtcNow);
    this.igc_strings = new List<string>();
    this.wind_count_agl = new double[24];
    this.wind_kph_agl = new double[24];
    this.wind_deg_agl = new double[24];
    this.wind_count_msl = new double[24];
    this.wind_kph_msl = new double[24];
    this.wind_deg_msl = new double[24];
  }

  public int get_length() => this.repeat_records_length;

  public int get_written_length() => this.written_records_length;

  public void set_written_length(int log_length) => this.written_records_length = log_length;

  public FlightDataModel.IGCRecord get_latest()
  {
    return this.repeat_records[this.repeat_records_length - 1];
  }

  private void inc_length()
  {
    if (this.repeat_records_length < 200000)
      ++this.repeat_records_length;
    else
      this.repeat_records_length = 0;
  }

  private double add_igc_string(string str)
  {
    if (this.igc_strings == null)
      return -1.0;
    this.igc_strings.Add(str);
    return (double) (this.igc_strings.Count - 1);
  }

  public string get_igc_string(double igc_string_index)
  {
    if (this.igc_strings == null)
      return "";
    try
    {
      return this.igc_strings[(int) igc_string_index];
    }
    catch (IndexOutOfRangeException ex)
    {
      return "";
    }
  }

  public void update_wind(double alt_m, double alt_agl_m, double wind_kph, double wind_deg)
  {
    int band1 = this.alt_to_band(alt_agl_m);
    ++this.wind_count_agl[band1];
    double num1 = Math.Min(100.0, this.wind_count_agl[band1]);
    if (num1 == 1.0)
    {
      this.wind_kph_agl[band1] = wind_kph;
      this.wind_deg_agl[band1] = wind_deg;
    }
    else
    {
      this.wind_kph_agl[band1] = this.wind_kph_agl[band1] * (num1 - 1.0) / num1 + wind_kph / num1;
      double num2 = wind_deg - this.wind_deg_agl[band1];
      if (num2 < -180.0)
        num2 += 360.0;
      else if (num2 > 180.0)
        num2 -= 360.0;
      double num3 = this.wind_deg_agl[band1] + num2;
      double num4 = this.wind_deg_agl[band1] * (num1 - 1.0) / num1 + num3 / num1;
      if (num4 < 0.0)
        num4 = 360.0 + num4;
      if (num4 >= 360.0)
        num4 -= 360.0;
      this.wind_deg_agl[band1] = num4;
    }
    int band2 = this.alt_to_band(alt_m);
    ++this.wind_count_msl[band2];
    double num5 = Math.Min(100.0, this.wind_count_msl[band2]);
    if (num5 == 1.0)
    {
      this.wind_kph_msl[band2] = wind_kph;
      this.wind_deg_msl[band2] = wind_deg;
    }
    else
    {
      this.wind_kph_msl[band2] = this.wind_kph_msl[band2] * (num5 - 1.0) / num5 + wind_kph / num5;
      double num6 = wind_deg - this.wind_deg_msl[band2];
      if (num6 < -180.0)
        num6 += 360.0;
      else if (num6 > 180.0)
        num6 -= 360.0;
      double num7 = this.wind_deg_msl[band2] + num6;
      double num8 = this.wind_deg_msl[band2] * (num5 - 1.0) / num5 + num7 / num5;
      if (num8 < 0.0)
        num8 = 360.0 + num8;
      if (num8 >= 360.0)
        num8 -= 360.0;
      this.wind_deg_msl[band2] = num8;
    }
  }

  private int alt_to_band(double alt_m)
  {
    if (alt_m < 1000.0)
      return (int) Math.Max(0.0, Math.Floor(alt_m / 100.0));
    if (alt_m < 5000.0)
      return 10 + (int) Math.Floor((alt_m - 1000.0) / 500.0);
    return alt_m < 10000.0 ? 18 + (int) Math.Floor((alt_m - 5000.0) / 1000.0) : 23;
  }

  public bool engine_on(double realtime_s, RepeatData rec)
  {
    return rec.combustion != 0.0 || rec.thrust_lbs > 9.0;
  }

  public unsafe FlightDataModel.IGCRecord make_b_rec(
    double realtime_s,
    double tas_kph,
    double wind_kph,
    double wind_deg,
    RepeatData rec)
  {
    if (!this.utc_begin.HasValue)
      this.utc_begin = new DateTime?(DateTime.UtcNow);
    FlightDataModel.IGCRecord igcRecord = new FlightDataModel.IGCRecord();
    igcRecord.rec_type = FlightDataModel.IGC_RECORD_TYPE.B;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.B];
    double num = (double) (this.engine_on(realtime_s, rec) ? 1 : 0);
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.lat_deg]] = rec.lat;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.lon_deg]] = rec.lon;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.alt_m]] = rec.alt_m;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.alt_agl_m]] = rec.alt_agl_m;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.tas_kph]] = tas_kph;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.engine]] = num;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.flap_pos]] = rec.flap_pos + 1.0;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.on_ground]] = rec.sim_on_ground;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.wind_y_ms]] = rec.wind_y_ms;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.wind_kph]] = wind_kph;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.wind_deg]] = wind_deg;
    igcRecord.var[dictionary[FlightDataModel.IGC_VALUE_NAME.gear_down]] = rec.gear_down;
    return igcRecord;
  }

  public void addB(
    double realtime_s,
    double tas_kph,
    double wind_kph,
    double wind_deg,
    RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length] = this.make_b_rec(realtime_s, tas_kph, wind_kph, wind_deg, rec);
    this.inc_length();
  }

  public unsafe void addSLEW(double realtime_s, RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.SLEW;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.SLEW];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.slew]] = rec.slew;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.alt_m]] = rec.alt_m;
    this.inc_length();
  }

  public unsafe void addPAUS(double realtime_s, bool pause_on, double delta_s)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.PAUS;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.PAUS];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.pause]] = (double) (pause_on ? 1 : 0);
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.delta_s]] = delta_s;
    this.inc_length();
  }

  public unsafe void addLTIM(
    double realtime_s,
    double prev_localtime_s,
    double delta_s,
    RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.LTIM;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LTIM];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.localtime_s]] = rec.localtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_localtime_s]] = prev_localtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.delta_s]] = delta_s;
    this.inc_length();
  }

  public unsafe void addPTIM(double realtime_s, double prev_realtime_s)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.PTIM;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.PTIM];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_realtime_s]] = prev_realtime_s;
    this.inc_length();
  }

  public unsafe void addLDAT(
    double realtime_s,
    double prev_local_year,
    double prev_local_month,
    double prev_local_day,
    RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.LDAT;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LDAT];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.local_year]] = rec.local_year;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.local_month]] = rec.local_month;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.local_day]] = rec.local_day;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_local_year]] = prev_local_year;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_local_month]] = prev_local_month;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_local_day]] = prev_local_day;
    this.inc_length();
  }

  public unsafe void addRATE(double realtime_s, double prev_sim_rate, RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.RATE;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.RATE];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_sim_rate]] = prev_sim_rate;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.sim_rate]] = rec.sim_rate;
    this.inc_length();
  }

  public unsafe void addENG(double realtime_s, RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.ENG;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.ENG];
    double num = (double) (this.engine_on(realtime_s, rec) ? 1 : 0);
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.engine]] = num;
    this.inc_length();
  }

  public unsafe void addTOTW(double realtime_s, RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.TOTW;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.TOTW];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.weight_kg]] = rec.weight_kg;
    this.inc_length();
  }

  public unsafe void addLDAC(double realtime_s, string igc_string)
  {
    Console.WriteLine($"FlightDataModel addLDAC '{igc_string}'");
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.LDAC;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LDAC];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    double num = this.add_igc_string(igc_string);
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.igc_string_index]] = num;
    this.inc_length();
  }

  public unsafe void addTOFF(double realtime_s)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.TOFF;
    this.repeat_records[this.repeat_records_length].var[this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.TOFF][FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.inc_length();
  }

  public unsafe void addLAND(double realtime_s)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.LAND;
    this.repeat_records[this.repeat_records_length].var[this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LAND][FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.inc_length();
  }

  public unsafe void addKADT(double realtime_s, double fold_wing_r)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.KADT;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.KADT];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.fold_wing_r]] = fold_wing_r;
    this.inc_length();
  }

  public unsafe void addWTHR(double realtime_s, RepeatData rec)
  {
    this.repeat_records[this.repeat_records_length].rec_type = FlightDataModel.IGC_RECORD_TYPE.WTHR;
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.WTHR];
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]] = realtime_s;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_hash]] = rec.weather_hash;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_series]] = rec.weather_series;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_version]] = rec.weather_version;
    this.repeat_records[this.repeat_records_length].var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_preset]] = rec.weather_preset;
    this.inc_length();
  }

  public enum IGC_RECORD_TYPE
  {
    B,
    SLEW,
    LTIM,
    PTIM,
    LDAT,
    ENG,
    PAUS,
    TOTW,
    LDAC,
    RATE,
    TOFF,
    LAND,
    KADT,
    WTHR,
  }

  public enum IGC_VALUE_NAME
  {
    realtime_s,
    localtime_s,
    prev_localtime_s,
    igc_string_index,
    lat_deg,
    lon_deg,
    alt_m,
    alt_agl_m,
    slew,
    sim_on_ground,
    gear_on_ground,
    tas_kph,
    engine,
    delta_s,
    flap_pos,
    pause,
    weight_kg,
    local_year,
    local_month,
    local_day,
    on_ground,
    wind_y_ms,
    wind_kph,
    wind_deg,
    prev_local_year,
    prev_local_month,
    prev_local_day,
    prev_realtime_s,
    sim_rate,
    prev_sim_rate,
    fold_wing_r,
    weather_hash,
    weather_series,
    weather_version,
    weather_preset,
    gear_down,
  }

  public struct IGCRecord
  {
    public FlightDataModel.IGC_RECORD_TYPE rec_type;
    public unsafe fixed double var[14];
  }
}
