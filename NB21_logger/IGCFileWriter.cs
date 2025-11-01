// Decompiled with JetBrains decompiler
// Type: NB21_logger.IGCFileWriter
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using NB21_logger.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

#nullable enable
namespace NB21_logger;

public class IGCFileWriter
{
  private SimDataConn simdata;
  private FlightDataModel flight_data_model;
  private Settings settings = Settings.Default;
  private IGCUtil u;
  public const string I_RECORD = "I073638FXA3943AGL4447TAS4851NET5254ENL5555FLP5658WSP5961WDI6262GND6363GEA";

  public IGCFileWriter(SimDataConn simdata)
  {
    this.simdata = simdata;
    this.flight_data_model = simdata.flight_data_model;
    this.u = new IGCUtil(simdata);
  }

  public string get_file_name()
  {
    DateTime utcNow = DateTime.UtcNow;
    string str1 = this.simdata.igc_aircraft.get_glider_type();
    Console.WriteLine($"IGCFileWriter get_file_name() plane_type='{str1}'");
    if (str1.Length > 12)
      str1 = str1.Substring(str1.Length - 12).Replace("_", "");
    string name = this.simdata.igc_task.name;
    string str2 = name == null || name == "" ? "no_task" : name;
    return Regex.Replace($"{this.settings.PilotId}_{str1}_{utcNow.ToString("yyyy-MM-dd")}_{utcNow.ToString("HHmm")}_{str2}.igc", string.Format("([{0}]*\\.+$)|([{0}]+)", (object) Regex.Escape(new string(Path.GetInvalidFileNameChars()) + ";")), "_");
  }

  public string write_file()
  {
    Console.WriteLine($"IGCFileWriter.write_file() task available = {this.simdata.igc_task.available}");
    string headerIgc = this.get_header_IGC();
    this.addRepeatRecords(ref headerIgc);
    string fileName = this.get_file_name();
    string path = $"{this.settings.IGCPath}\\{fileName}";
    try
    {
      string str = this.u.t(headerIgc);
      if (str == null)
        Console.WriteLine("IGCFileWriter chksum error " + fileName);
      else
        Console.WriteLine("IGCFileWriter chksum IGC: " + str);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"IGCFileWriter exception computing checksum {fileName} {ex}");
    }
    this.addFooter(ref headerIgc);
    try
    {
      StreamWriter streamWriter = new StreamWriter(path, false);
      streamWriter.Write(headerIgc);
      streamWriter.Close();
      Console.WriteLine("IGCFileWriter Wrote file " + fileName);
    }
    catch (Exception ex)
    {
      Console.WriteLine("Exception in ICGFileWriter " + ex.ToString());
      this.simdata.trigger(APPEVENT_ID.IGCWriteError);
      path = "";
    }
    return path;
  }

  public string get_header_IGC()
  {
    string str1 = "AXXX #LOGGER_TITLE#\r\nHFDTE#LOGGER_DATE#\r\nHFFXA035\r\nHFPLTPILOTINCHARGE: #LOGGER_PILOT_NAME#\r\nHFCM2CREW2: not recorded\r\nHFGTYGLIDERTYPE: #LOGGER_AIRCRAFT_TYPE#\r\nHFGIDGLIDERID: #LOGGER_GLIDER_ID#\r\nHFDTM100GPSDATUM: WGS-1984\r\nHFRFWFIRMWAREVERSION: #LOGGER_VERSION#\r\nHFRHWHARDWAREVERSION: 2023\r\nHFFTYFRTYPE: #LOGGER_TYPE#\r\nHFGPSGPS: Microsoft Flight Simulator\r\nHFPRSPRESSALTSENSOR: Microsoft Flight Simulator\r\nHFCIDCOMPETITIONID: #LOGGER_COMPETITION_ID#\r\nHFCCLCOMPETITIONCLASS: #LOGGER_COMPETITION_CLASS#\r\n".Replace("#LOGGER_TITLE#", $"NB21 {this.settings.AppVersion} {this.simdata.get_msfs_text()}");
    ref DateTime? local = ref this.flight_data_model.utc_begin;
    string newValue = local.HasValue ? local.GetValueOrDefault().ToString("ddMMyy") : (string) null;
    string str2 = str1.Replace("#LOGGER_DATE#", newValue).Replace("#LOGGER_PILOT_NAME#", this.settings.PilotName).Replace("#LOGGER_AIRCRAFT_TYPE#", this.simdata.igc_aircraft.get_glider_type()).Replace("#LOGGER_VERSION#", this.settings.AppVersion).Replace("#LOGGER_TYPE#", this.settings.AppName).Replace("#LOGGER_GLIDER_ID#", this.settings.PilotId).Replace("#LOGGER_COMPETITION_ID#", this.settings.PilotId).Replace("#LOGGER_COMPETITION_CLASS#", $"{this.simdata.igc_aircraft.wingspan_m:0}m {(this.simdata.igc_aircraft.flap_positions > 0.0 ? (object) "flapped" : (object) "no flaps")}");
    string hhmmss = IGCFileWriter.s_to_hhmmss(this.simdata.igc_aircraft.load_ts);
    string str3 = $"{str2}LNB21 {hhmmss} TITL {this.simdata.igc_aircraft.get_title()}{Environment.NewLine}";
    double wingspanM = this.simdata.igc_aircraft.get_wingspan_m();
    if (wingspanM > 0.0)
      str3 = str3 + $"LNB21 {hhmmss} WING {Convert.ToInt32(wingspanM):D2}" + Environment.NewLine;
    string headerIgc = $"{str3}I073638FXA3943AGL4447TAS4851NET5254ENL5555FLP5658WSP5961WDI6262GND6363GEA{Environment.NewLine}" + this.get_IGC_task(this.simdata.igc_task);
    if (this.simdata.igc_aircraft.u_aircraft_cfg != null)
    {
      headerIgc = $"{headerIgc}LNB21 {hhmmss} FCHK ACFG {this.simdata.igc_aircraft.u_aircraft_cfg}{Environment.NewLine}";
      if (this.simdata.igc_aircraft.u_flight_model_cfg != null)
        headerIgc = $"{headerIgc}LNB21 {hhmmss} FCHK FMCG {this.simdata.igc_aircraft.u_flight_model_cfg}{Environment.NewLine}";
    }
    else
      Console.WriteLine("IGCFileWriter aircraft.cfg checksum not available in flight_data_model");
    return headerIgc;
  }

  public string get_IGC_task(IGCTask igc_task)
  {
    if (!igc_task.available)
    {
      Console.WriteLine("IGCFileWriter.get_IGC_task task.available = false");
      return "";
    }
    string igcTask = $"{this.createCHeader(igc_task)}{Environment.NewLine}";
    Console.WriteLine("IGCFileWriter.get_IGC_task adding following C records:");
    int num = 0;
    foreach (IGCWaypoint waypoint in igc_task.waypoints)
    {
      string str = num != 0 ? (num != igc_task.waypoints.Count - 1 ? this.createCWPRecord(waypoint) : this.createLastCWPRecord(waypoint, igc_task)) : this.createFirstCWPRecord(waypoint, igc_task);
      igcTask = igcTask + str + Environment.NewLine;
      ++num;
    }
    Console.WriteLine(igcTask);
    return igcTask;
  }

  private string createCHeader(IGCTask igc_task)
  {
    return $"C{this.utc_datetime_to_str(igc_task.load_utc_time)}{igc_task.fly_sim_local_time_str}0001{igc_task.waypoints.Count:00}{igc_task.name}{igc_task.title_args}";
  }

  private string utc_datetime_to_str(DateTime utc_time) => utc_time.ToString("ddMMyyHHmmss");

  private string createFirstCWPRecord(IGCWaypoint wp, IGCTask igc_task)
  {
    string str1 = "";
    if (igc_task.departure_id != null)
    {
      string str2 = igc_task.departure_id + ";";
      str1 = igc_task.departure_runway == null ? str2 + ";" : $"{str2}{igc_task.departure_runway};";
    }
    string str3 = str1 + wp.name;
    return $"C{this.lat_to_str(wp.lat)}{this.lon_to_str(wp.lon)}{str3}";
  }

  private string createCWPRecord(IGCWaypoint wp)
  {
    return $"C{this.lat_to_str(wp.lat)}{this.lon_to_str(wp.lon)}{wp.name}";
  }

  private string createLastCWPRecord(IGCWaypoint wp, IGCTask igc_task)
  {
    string str1 = "";
    if (igc_task.destination_id != null)
      str1 = igc_task.destination_id + ";";
    string str2 = str1 + wp.name;
    return $"C{this.lat_to_str(wp.lat)}{this.lon_to_str(wp.lon)}{str2}";
  }

  public string lat_to_str(double deg)
  {
    string str1 = "N";
    if (deg < 0.0)
    {
      str1 = "S";
      deg = -deg;
    }
    double num = Math.Floor(deg);
    string str2 = $"{Math.Round((deg - num) * 60.0, 3) * 1000.0:00000}";
    if (str2 == "60000")
    {
      str2 = "00000";
      ++num;
    }
    return $"{num:00}" + str2 + str1;
  }

  public string lon_to_str(double deg)
  {
    string str1 = "E";
    if (deg < 0.0)
    {
      str1 = "W";
      deg = -deg;
    }
    double num = Math.Floor(deg);
    string str2 = $"{Math.Round((deg - num) * 60.0, 3) * 1000.0:00000}";
    if (str2 == "60000")
    {
      str2 = "00000";
      ++num;
      if (num == 180.0)
      {
        num = 0.0;
        str1 = str1 == "E" ? "W" : "E";
      }
    }
    return $"{num:000}" + str2 + str1;
  }

  private void addRepeatRecords(ref string file_str)
  {
    for (int index = 0; index < this.flight_data_model.repeat_records_length; ++index)
    {
      string str = this.repeat_record_str(this.flight_data_model.repeat_records[index]);
      file_str = file_str + str + Environment.NewLine;
    }
  }

  private string repeat_record_str(FlightDataModel.IGCRecord rec)
  {
    switch (rec.rec_type)
    {
      case FlightDataModel.IGC_RECORD_TYPE.B:
        return this.createBRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.SLEW:
        return this.createSLEWRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.LTIM:
        return this.createLTIMRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.PTIM:
        return this.createPTIMRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.LDAT:
        return this.createLDATRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.ENG:
        return this.createENGRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.PAUS:
        return this.createPAUSRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.TOTW:
        return this.createTOTWRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.LDAC:
        return this.createLDACRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.RATE:
        return this.createRATERecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.TOFF:
        return this.createTOFFRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.LAND:
        return this.createLANDRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.KADT:
        return this.createKADTRecord(rec);
      case FlightDataModel.IGC_RECORD_TYPE.WTHR:
        return this.createWTHRRecord(rec);
      default:
        return "LNB21 UNKNOWN IGC_RECORD_TYPE " + rec.rec_type.ToString();
    }
  }

  public unsafe string createBRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.B];
    string hhmmss = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]]);
    double dat1 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.lat_deg]];
    double dat2 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.lon_deg]];
    double num1 = Math.Max(0.0, Math.Min(99999.0, Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.alt_m]])));
    double num2 = Math.Max(0.0, Math.Min(99999.0, Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.alt_agl_m]])));
    string str1 = "007";
    double num3 = Math.Min(9999.0, Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.tas_kph]]));
    bool flag1 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.engine]] != 0.0;
    string str2 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.flap_pos]]:0}";
    bool flag2 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.on_ground]] > 0.5;
    double num4 = Math.Min(99.99, Math.Max(-9.99, rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.wind_y_ms]]));
    double num5 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.wind_kph]];
    double num6 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.wind_deg]];
    bool flag3 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.gear_down]] > 0.5;
    return $"B{hhmmss}{this.ConvertDecimalToDMS((object) dat1, true)}{this.ConvertDecimalToDMS((object) dat2, false)}A{num1.ToString("00000")}{num1.ToString("00000")}{str1}{num2.ToString("00000")}{num3.ToString("0000")}{((int) Math.Round(100.0 * num4)).ToString("0000;-000")}{(flag1 ? "999" : "000")}{str2}{num5.ToString("000")}{num6.ToString("000")}{(flag2 ? "1" : "0")}{(flag3 ? "1" : "0")}";
  }

  private unsafe string createSLEWRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.SLEW];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} SLEW {(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.slew]] > 0.0 ? " ON" : "OFF")} {Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.alt_m]]).ToString("00000")}";
  }

  private unsafe string createTOTWRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.TOTW];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} TOTW {Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.weight_kg]]).ToString("000000")}";
  }

  private unsafe string createLDACRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LDAC];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} LDAC {this.flight_data_model.get_igc_string(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.igc_string_index]])}";
  }

  private unsafe string createLTIMRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LTIM];
    string hhmmss1 = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]]);
    string hhmmss2 = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.localtime_s]]);
    string hhmmss3 = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_localtime_s]]);
    double num1 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.delta_s]];
    TimeSpan timeSpan = TimeSpan.FromSeconds(Math.Abs(num1));
    string str1 = Math.Sign(num1) < 0 ? "-" : "+";
    string str2 = timeSpan.Hours.ToString("00");
    int num2 = timeSpan.Minutes;
    string str3 = num2.ToString("00");
    num2 = timeSpan.Seconds;
    string str4 = num2.ToString("00");
    string str5 = str1 + str2 + str3 + str4;
    return $"LNB21 {hhmmss1} LTIM {hhmmss3} {hhmmss2} {str5}";
  }

  private unsafe string createPTIMRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.PTIM];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} PTIM {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_realtime_s]])}";
  }

  private unsafe string createLDATRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LDAT];
    string hhmmss = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]]);
    string str1 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.local_year]]:0000}";
    string str2 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.local_month]]:00}";
    string str3 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.local_day]]:00}";
    string str4 = str2;
    string str5 = str3;
    string str6 = str1 + str4 + str5;
    string str7 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_local_year]]:0000}";
    string str8 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_local_month]]:00}";
    string str9 = $"{rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_local_day]]:00}";
    string str10 = str8;
    string str11 = str9;
    string str12 = str7 + str10 + str11;
    return $"LNB21 {hhmmss} LDAT {str12} {str6}";
  }

  private unsafe string createRATERecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.RATE];
    string hhmmss = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]]);
    string str1 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.sim_rate]].ToString("00.00", (IFormatProvider) CultureInfo.InvariantCulture);
    string str2 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.prev_sim_rate]].ToString("00.00", (IFormatProvider) CultureInfo.InvariantCulture);
    return $"LNB21 {hhmmss} RATE {str2} {str1}";
  }

  private unsafe string createENGRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.ENG];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])}  ENG {(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.engine]] > 0.0 ? " ON" : "OFF")}";
  }

  private unsafe string createPAUSRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.PAUS];
    string hhmmss = IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]]);
    double num1 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.pause]];
    string str1 = num1 > 0.0 ? " ON" : "OFF";
    double num2 = rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.delta_s]];
    string str2 = num1 > 0.0 ? "" : $"{num2:00000}";
    return $"LNB21 {hhmmss} PAUS {str1} {str2}";
  }

  private unsafe string createTOFFRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.TOFF];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} TOFF ";
  }

  private unsafe string createLANDRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.LAND];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} LAND ";
  }

  private unsafe string createKADTRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.KADT];
    return $"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} KADT {rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.fold_wing_r]].ToString("000.00", (IFormatProvider) CultureInfo.InvariantCulture)}";
  }

  private unsafe string createWTHRRecord(FlightDataModel.IGCRecord rec)
  {
    Dictionary<FlightDataModel.IGC_VALUE_NAME, int> dictionary = this.flight_data_model.RECORD_VAR_INDEX[FlightDataModel.IGC_RECORD_TYPE.WTHR];
    return $"{$"LNB21 {IGCFileWriter.s_to_hhmmss(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.realtime_s]])} WTHR "}{((int) Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_hash]])).ToString("0000000", (IFormatProvider) CultureInfo.InvariantCulture)} {((int) Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_series]])).ToString("0000", (IFormatProvider) CultureInfo.InvariantCulture)} {rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_version]].ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture)} {((int) Math.Round(rec.var[dictionary[FlightDataModel.IGC_VALUE_NAME.weather_preset]])).ToString("000", (IFormatProvider) CultureInfo.InvariantCulture)}";
  }

  private void addFooter(ref string file_str)
  {
    this.addWindSynopsis(ref file_str);
    string str = this.u.t(file_str);
    file_str = $"{file_str}GB21DEADBEEF{Environment.NewLine}";
    file_str = $"{file_str}G{str}{Environment.NewLine}";
  }

  private void addWindSynopsis(ref string file_str)
  {
    string hhmmss = IGCFileWriter.s_to_hhmmss(SimDataConn.get_realtime_s());
    file_str = $"{file_str}LNB21 {hhmmss} WBND 0,100,200,300,400,500,600,700,800,900,1000,1500,2000,2500,3000,3500,4000,4500,5000,6000,7000,8000,9000,10000{Environment.NewLine}";
    file_str = $"{file_str}LNB21 {hhmmss} WCAG {this.make_synopsis_string(this.simdata.flight_data_model.wind_count_agl)}{Environment.NewLine}";
    file_str = $"{file_str}LNB21 {hhmmss} WSAG {this.make_synopsis_string(this.simdata.flight_data_model.wind_kph_agl)}{Environment.NewLine}";
    file_str = $"{file_str}LNB21 {hhmmss} WDAG {this.make_synopsis_string(this.simdata.flight_data_model.wind_deg_agl)}{Environment.NewLine}";
    file_str = $"{file_str}LNB21 {hhmmss} WCMS {this.make_synopsis_string(this.simdata.flight_data_model.wind_count_msl)}{Environment.NewLine}";
    file_str = $"{file_str}LNB21 {hhmmss} WSMS {this.make_synopsis_string(this.simdata.flight_data_model.wind_kph_msl)}{Environment.NewLine}";
    file_str = $"{file_str}LNB21 {hhmmss} WDMS {this.make_synopsis_string(this.simdata.flight_data_model.wind_deg_msl)}{Environment.NewLine}";
  }

  private string make_synopsis_string(double[] numbers)
  {
    string str = "";
    for (int index = 0; index < numbers.Length; ++index)
    {
      if (index > 0)
        str += ",";
      str += numbers[index].ToString("0");
    }
    return str;
  }

  private string ConvertDecimalToDMS(object dat, bool isLatitude)
  {
    try
    {
      double num1 = Convert.ToDouble(dat);
      int num2 = (int) num1;
      string str1 = Math.Round((num1 - (double) num2) * 60.0, 3).ToString("00.000", (IFormatProvider) CultureInfo.InvariantCulture).Replace(".", "").Replace("-", "");
      string str2;
      string str3;
      if (isLatitude)
      {
        str2 = num1 >= 0.0 ? "N" : "S";
        str3 = Math.Abs(num2).ToString("00");
      }
      else
      {
        str2 = num1 >= 0.0 ? "E" : "W";
        str3 = Math.Abs(num2).ToString("000");
      }
      return str3 + str1 + str2;
    }
    catch
    {
      return string.Empty;
    }
  }

  public static string s_to_hhmmss(double s)
  {
    TimeSpan timeSpan = TimeSpan.FromSeconds(s);
    int num = timeSpan.Hours;
    string str1 = num.ToString("00");
    num = timeSpan.Minutes;
    string str2 = num.ToString("00");
    num = timeSpan.Seconds;
    string str3 = num.ToString("00");
    return str1 + str2 + str3;
  }
}
