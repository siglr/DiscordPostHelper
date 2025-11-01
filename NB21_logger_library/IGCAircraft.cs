// Decompiled with JetBrains decompiler
// Type: NB21_logger.IGCAircraft
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.IO;

#nullable enable
namespace NB21_logger;

public class IGCAircraft
{
  public double load_ts;
  public string? file_path;
  public string? file_name;
  public bool available;
  public string? glider_type;
  public string? title;
  public string? atc_id;
  public double wingspan_m;
  public double flap_positions;
  public string? u_aircraft_cfg;
  public string? u_flight_model_cfg;
  private IGCUtil u;

  public IGCAircraft(SimDataConn simdata)
  {
    this.u = new IGCUtil(simdata);
    this.reset();
  }

  public void reset()
  {
    DateTime utcNow = DateTime.UtcNow;
    this.load_ts = (double) (utcNow.Hour * 3600 + utcNow.Minute * 60 + utcNow.Second + utcNow.Millisecond / 1000);
    this.file_path = (string) null;
    this.file_name = (string) null;
    this.glider_type = (string) null;
    this.title = (string) null;
    this.flap_positions = 0.0;
    this.available = false;
    this.u_aircraft_cfg = (string) null;
    this.u_flight_model_cfg = (string) null;
  }

  public string get_title()
  {
    if (this.title != null)
      return this.title;
    return this.glider_type == null ? "unknown" : this.get_glider_type();
  }

  public double get_wingspan_m() => this.wingspan_m;

  public string get_glider_type()
  {
    if (this.glider_type != null)
    {
      string str = "";
      if (this.wingspan_m > 0.0)
        str = $"_{this.wingspan_m:0}m";
      return this.glider_type + str;
    }
    if (this.title == null)
      return "unknown";
    string[] strArray = this.title.Replace("_", " ").Split(' ');
    int index = strArray[0] == "Asobo" ? 1 : 0;
    return strArray[index] + (strArray.Length > index + 2 ? strArray[index + 1] : "");
  }

  public bool load_aircraft_cfg_file(string filepath)
  {
    bool flag = false;
    string str1 = (string) null;
    this.file_path = filepath;
    string fileName = Path.GetFileName(this.file_path);
    if (fileName == null || fileName.Length < 5)
      return false;
    if (fileName.ToLower() != "xxx")
      this.file_name = fileName;
    string str2;
    try
    {
      str2 = File.ReadAllText(this.file_path);
    }
    catch (DirectoryNotFoundException ex)
    {
      Console.WriteLine("IGCAircraft aircraft.cfg directory error: " + filepath);
      return false;
    }
    catch (Exception ex)
    {
      Console.WriteLine("IGCAircraft error reading: " + filepath);
      Console.WriteLine(ex.ToString());
      return false;
    }
    if (str2 != null && str2.Length > 0)
    {
      this.set_glider_type(str2);
      str1 = this.u.t(str2);
      flag = true;
    }
    else
      Console.WriteLine("Failed to read " + this.file_path);
    if (flag)
      this.u_aircraft_cfg = str1;
    return flag;
  }

  private void set_glider_type(string aircraft_cfg_str)
  {
    int startIndex1 = aircraft_cfg_str.IndexOf("icao_type_designator");
    Console.WriteLine($"IGC Aircraft icao_type_designator at {startIndex1}, current glider_type '{this.get_glider_type()}'");
    if (startIndex1 <= 0)
      return;
    int startIndex2 = aircraft_cfg_str.IndexOf('=', startIndex1);
    if (startIndex2 <= 0)
      return;
    int val1 = aircraft_cfg_str.IndexOf('\r', startIndex2);
    int val2 = aircraft_cfg_str.IndexOf('\n', startIndex2);
    int num = 0;
    if (val1 > 0)
      num = val2 == -1 ? val1 : Math.Min(val1, val2);
    else if (val2 > 0)
      num = val2;
    Console.WriteLine($"IGC Aircraft icao_type_designator line end at  {num}");
    if (num <= 0)
      return;
    string str1 = aircraft_cfg_str.Substring(startIndex2 + 1, num - startIndex2 - 1);
    Console.WriteLine($"IGCAircraft set_glider_type type_designator_str='{str1}'");
    string str2 = str1.Replace(" ", "").Replace("\\", "").Replace("/", "-").Replace("\"", "").Replace("'", "");
    if (this.glider_type == null || str2.Length > 0 && str2.Length < this.glider_type.Length)
      this.glider_type = str2;
    Console.WriteLine($"IGCAircraft glider_type:'{this.get_glider_type()}' type_designator_str:'{str2}'");
  }

  public bool load_flight_model_cfg_file(string filepath)
  {
    bool flag = false;
    string str1 = (string) null;
    string str2;
    try
    {
      str2 = File.ReadAllText(filepath);
    }
    catch (DirectoryNotFoundException ex)
    {
      Console.WriteLine("IGCAircraft flight_model.cfg directory error: " + filepath);
      return false;
    }
    catch (Exception ex)
    {
      Console.WriteLine("IGCAircraft error reading: " + filepath);
      Console.WriteLine(ex.ToString());
      return false;
    }
    if (str2 != null && str2.Length > 0)
    {
      str1 = this.u.t(str2);
      flag = true;
    }
    else
      Console.WriteLine("Failed to read " + this.file_path);
    if (flag)
      this.u_flight_model_cfg = str1;
    return flag;
  }
}
