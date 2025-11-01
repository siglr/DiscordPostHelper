// Decompiled with JetBrains decompiler
// Type: NB21_logger.IGCTask
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

#nullable enable
namespace NB21_logger;

public class IGCTask
{
  public string? name;
  public DateTime load_utc_time = DateTime.UtcNow;
  public string fly_sim_local_time_str = "000000";
  public string file_path = "";
  public string file_name = "";
  public bool available;
  public string? title;
  public string? title_args;
  public string? description;
  public string? departure_id;
  public string? departure_runway;
  public string? destination_id;
  public List<IGCWaypoint> waypoints = new List<IGCWaypoint>();
  public int index;
  public int start_index;
  public int finish_index;
  public bool start_index_set;
  public bool finish_index_set;
  public float task_distance_m;
  public int pln_ref_id;
  public string pln_file_str = "";

  public IGCTask() => this.reset();

  public void reset()
  {
    Console.WriteLine($"IGCTask.reset() igc_task.available was {this.available}");
    this.available = false;
    this.load_utc_time = DateTime.UtcNow;
    this.fly_sim_local_time_str = "000000";
    this.file_path = "";
    this.waypoints.Clear();
    this.name = (string) null;
    this.title = (string) null;
    this.title_args = (string) null;
    this.description = (string) null;
    this.departure_id = (string) null;
    this.departure_runway = (string) null;
    this.destination_id = (string) null;
    this.start_index_set = false;
    this.finish_index_set = false;
    this.task_distance_m = 0.0f;
    this.pln_file_str = "";
  }

  public string to_get_flightplan()
  {
    return "{" + FormattableString.Invariant(FormattableStringFactory.Create("\"pln_ref_id\": {0},", (object) this.pln_ref_id)) + FormattableString.Invariant(FormattableStringFactory.Create("\"name\": \"{0}\",", (object) this.name)) + FormattableString.Invariant(FormattableStringFactory.Create("\"title\": \"{0}\",", (object) this.title)) + FormattableString.Invariant(FormattableStringFactory.Create("\"description\": \"{0}\",", (object) this.description)) + FormattableString.Invariant(FormattableStringFactory.Create("\"departure_id\": \"{0}\",", (object) this.departure_id)) + FormattableString.Invariant(FormattableStringFactory.Create("\"departure_runway\": \"{0}\",", (object) this.departure_runway)) + FormattableString.Invariant(FormattableStringFactory.Create("\"destination_id\": \"{0}\",", (object) this.destination_id)) + FormattableString.Invariant(FormattableStringFactory.Create("\"waypoints\": {0}", (object) this.wps_to_get_flightplan())) + "}";
  }

  private string wps_to_get_flightplan()
  {
    string str = "[";
    for (int index = 0; index < this.waypoints.Count; ++index)
    {
      str += this.waypoints[index].to_get_flightplan();
      if (index < this.waypoints.Count - 1)
        str += ",";
    }
    return str + "]";
  }

  public void load_pln_file(string pln_filepath)
  {
    Console.WriteLine("Task.load_pln called with " + pln_filepath);
    this.reset();
    this.file_path = pln_filepath;
    string fileName = Path.GetFileName(this.file_path);
    if (fileName == null || fileName.Length < 5)
      return;
    if (fileName.ToLower() != "customflight.pln" || !this.available)
    {
      this.file_name = fileName;
      this.name = fileName.Substring(0, fileName.Length - 4);
    }
    Console.WriteLine("Task name from file: " + this.name);
    try
    {
      this.load_pln_str(File.ReadAllText(this.file_path));
    }
    catch (Exception ex)
    {
      this.pln_file_str = "";
      Console.WriteLine("PLN file read error " + this.file_path);
    }
  }

  public void load_pln_str(string pln_str)
  {
    Console.WriteLine("IGCTask load_pln_str():");
    Console.WriteLine(pln_str);
    this.pln_file_str = pln_str;
    XmlDocument xmlDocument = new XmlDocument();
    XmlNode flightplan_node;
    try
    {
      xmlDocument.LoadXml(pln_str);
      flightplan_node = xmlDocument?.DocumentElement?.SelectSingleNode("FlightPlan.FlightPlan");
    }
    catch (Exception ex)
    {
      this.pln_file_str = "";
      Console.WriteLine("load_pln_str() FlightPlan XML fail" + ex.ToString());
      return;
    }
    if (flightplan_node != null)
    {
      this.load_pln_xml(flightplan_node);
    }
    else
    {
      this.pln_file_str = "";
      Console.WriteLine("load_plan_str() No FlightPlan node found in XML");
    }
  }

  public void load_pln_xml(XmlNode flightplan_node)
  {
    try
    {
      this.title = flightplan_node?.SelectSingleNode("Title")?.InnerText;
      if (this.name == null || this.name.ToLower() == "customflight")
      {
        this.name = this.title;
        Console.WriteLine("Task name from PLN string title: " + this.name);
      }
      Console.WriteLine("Task title: " + this.title);
      if (this.title != null)
      {
        int startIndex = this.title.IndexOf(";");
        if (startIndex != -1)
        {
          this.title_args = this.title.Substring(startIndex);
          Console.WriteLine("Task title args:" + this.title_args);
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("No title in PLN " + ex.ToString());
    }
    try
    {
      this.description = flightplan_node?.SelectSingleNode("Descr")?.InnerText;
    }
    catch (Exception ex)
    {
      Console.WriteLine("No description in PLN " + ex.ToString());
    }
    try
    {
      this.departure_id = flightplan_node?.SelectSingleNode("DepartureID")?.InnerText;
    }
    catch (Exception ex)
    {
      this.departure_id = (string) null;
      Console.WriteLine("load_pln_file no DepartureID in .PLN file " + ex.ToString());
    }
    try
    {
      this.departure_runway = flightplan_node?.SelectSingleNode("DeparturePosition")?.InnerText;
      Console.WriteLine($"load_pln_file got DeparturePosition '{this.departure_runway}'");
    }
    catch (Exception ex)
    {
      this.departure_runway = (string) null;
      Console.WriteLine("load_pln_file no DeparturePosition in .PLN file " + ex.ToString());
    }
    if (this.departure_runway == null || this.departure_runway == "")
    {
      try
      {
        XmlNode xmlNode1 = flightplan_node?.SelectSingleNode("DepartureDetails");
        this.departure_runway = xmlNode1?.SelectSingleNode("RunwayNumberFP")?.InnerText;
        XmlNode xmlNode2 = xmlNode1?.SelectSingleNode("RunwayDesignatorFP");
        if (xmlNode2 != null)
        {
          switch (xmlNode2.InnerText.ToLower())
          {
            case "left":
              this.departure_runway += "L";
              break;
            case "right":
              this.departure_runway += "R";
              break;
          }
        }
        Console.WriteLine($"load_pln_file got DepartureDetails rw:'{this.departure_runway}'");
      }
      catch (Exception ex)
      {
        this.departure_runway = (string) null;
        Console.WriteLine("load_pln_file no DepartureDetails/RunwayNumberFP in .PLN file " + ex.ToString());
      }
    }
    try
    {
      this.destination_id = flightplan_node?.SelectSingleNode("DestinationID")?.InnerText;
    }
    catch (Exception ex)
    {
      this.destination_id = (string) null;
      Console.WriteLine("load_pln_file no DestinationID in .PLN file " + ex.ToString());
    }
    try
    {
      XmlNodeList xmlNodeList = flightplan_node?.SelectNodes("ATCWaypoint");
      if (xmlNodeList != null)
      {
        foreach (XmlNode wp_node in xmlNodeList)
        {
          try
          {
            int count = this.waypoints.Count;
            IGCWaypoint wp = new IGCWaypoint(wp_node, count);
            if (wp.name != "")
            {
              this.decode_wp_name(wp);
              this.waypoints.Add(wp);
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine("Exception loading PLN waypoint " + ex.ToString());
            return;
          }
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Failed reading WP from PLN " + ex.ToString());
      return;
    }
    if (this.waypoints.Count <= 2)
      return;
    ++this.pln_ref_id;
    this.available = true;
  }

  private void decode_wp_name(IGCWaypoint? wp)
  {
    if (wp == null || wp.name == "")
      return;
    if (wp.name.ToLower().StartsWith("start"))
    {
      this.start_index = wp.index;
      this.start_index_set = true;
    }
    else if (wp.name.ToLower().StartsWith("finish"))
      this.finish_index = wp.index;
    else if (wp.name.StartsWith("*"))
    {
      if (!this.start_index_set)
      {
        this.start_index_set = true;
        this.start_index = wp.index;
      }
      else
      {
        this.finish_index_set = true;
        this.finish_index = wp.index;
      }
    }
    string s = "";
    string[] strArray1 = wp.name.Split('+');
    if (strArray1.Length > 1)
    {
      s = strArray1[strArray1.Length - 1];
      try
      {
        float num = float.Parse(IGCTask.digits(s), (IFormatProvider) CultureInfo.InvariantCulture);
        wp.alt_m = (double) num / 3.28084;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Bad elevation for waypoint " + wp.index.ToString());
        wp.alt_m = 0.0;
      }
    }
    string[] strArray2 = wp.name.Split('|');
    if (strArray2.Length > 1)
    {
      s = strArray2[strArray2.Length - 1];
      try
      {
        float num = float.Parse(IGCTask.digits(s), (IFormatProvider) CultureInfo.InvariantCulture);
        wp.max_alt_m = (double) num / 3.28084;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Bad max alt for waypoint " + wp.index.ToString());
        wp.max_alt_m = 0.0;
      }
    }
    string[] strArray3 = s.Split('/');
    if (strArray3.Length > 1)
    {
      try
      {
        float num = float.Parse(IGCTask.digits(strArray3[strArray3.Length - 1]), (IFormatProvider) CultureInfo.InvariantCulture);
        wp.min_alt_m = (double) num / 3.28084;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Bad min alt for waypoint " + wp.index.ToString());
        wp.min_alt_m = 0.0;
      }
    }
    string[] strArray4 = s.Split('x');
    if (strArray4.Length <= 1)
      return;
    try
    {
      float num = float.Parse(IGCTask.digits(strArray4[strArray4.Length - 1]), (IFormatProvider) CultureInfo.InvariantCulture) / 2f;
      wp.radius_m = (double) num;
    }
    catch (Exception ex)
    {
      Console.WriteLine("Bad radius for waypoint " + wp.index.ToString());
      wp.min_alt_m = 0.0;
    }
  }

  public static string digits(string s)
  {
    string str = "";
    int index = 0;
    while (index < s.Length && (s[index] == '.' || s[index] == '+' || s[index] == '-' || char.IsDigit(s, index)))
      str += s[index++].ToString();
    return str;
  }
}
