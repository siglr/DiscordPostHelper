// Decompiled with JetBrains decompiler
// Type: NB21_logger.IGCWaypoint
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

#nullable enable
namespace NB21_logger;

public class IGCWaypoint
{
  public string name = "";
  public string icao = "";
  public int index;
  public double lat;
  public double lon;
  public double alt_feet;
  public double alt_m;
  public double max_alt_m;
  public double min_alt_m;
  public double radius_m;

  public IGCWaypoint(XmlNode wp_node, int index)
  {
    this.index = index;
    this.load_wp_from_pln(wp_node);
  }

  public string short_name()
  {
    string str = this.name;
    if (str.StartsWith("*"))
      str = str.Substring(1);
    return str.Split('+')[0].Split('|')[0];
  }

  public string to_json()
  {
    try
    {
      string str = "{" + FormattableString.Invariant(FormattableStringFactory.Create("\"name\": \"{0}\",", (object) this.name));
      if (this.icao != "")
        str += FormattableString.Invariant(FormattableStringFactory.Create("\"icao\": \"{0}\",", (object) this.icao));
      return str + FormattableString.Invariant(FormattableStringFactory.Create("\"lat\": {0:0.00000000},", (object) this.lat)) + FormattableString.Invariant(FormattableStringFactory.Create("\"lon\": {0:0.00000000},", (object) this.lon)) + FormattableString.Invariant(FormattableStringFactory.Create("\"alt_feet\": {0:0}", (object) this.alt_feet)) + "}";
    }
    catch (Exception ex)
    {
      Console.WriteLine("Bad waypoint to_json", (object) ex);
      return "";
    }
  }

  public string to_get_flightplan()
  {
    try
    {
      double num = this.alt_m == 0.0 ? this.alt_feet / 3.28084 : this.alt_m;
      return "{" + FormattableString.Invariant(FormattableStringFactory.Create("\"ident\": \"{0}\",", (object) this.name)) + FormattableString.Invariant(FormattableStringFactory.Create("\"icao\": \"{0}\",", (object) this.icao)) + FormattableString.Invariant(FormattableStringFactory.Create("\"lla\": {{ \"lat\": {0:0.00000000},\"long\": {1:0.00000000}, \"alt\": {2:0}}}", (object) this.lat, (object) this.lon, (object) num)) + "}";
    }
    catch (Exception ex)
    {
      Console.WriteLine("Bad waypoint to_get_flightplan", (object) ex);
      return "";
    }
  }

  private void load_wp_from_pln(XmlNode wp_node)
  {
    string innerText = wp_node?.Attributes?["id"]?.InnerText;
    switch (innerText)
    {
      case null:
        break;
      case "":
        break;
      case "TIMECRUIS":
        break;
      case "TIMECLIMB":
        break;
      case "TIMEVERT":
        break;
      case "TIMEDSCNT":
        break;
      default:
        this.name = innerText;
        XmlNode xmlNode1 = wp_node?.SelectSingleNode("WorldPosition");
        if (xmlNode1 != null)
        {
          try
          {
            string[] strArray1 = xmlNode1.InnerText.Split(',');
            string[] strArray2 = strArray1[0].Split(' ');
            this.lat = (double) int.Parse(IGCTask.digits(strArray2[0].Substring(1)), (IFormatProvider) CultureInfo.InvariantCulture) + double.Parse(IGCTask.digits(strArray2[1]), (IFormatProvider) CultureInfo.InvariantCulture) / 60.0 + double.Parse(IGCTask.digits(strArray2[2]), (IFormatProvider) CultureInfo.InvariantCulture) / 3600.0;
            this.lat = strArray2[0].StartsWith("N") ? this.lat : -1.0 * this.lat;
            string[] strArray3 = strArray1[1].Split(' ');
            this.lon = (double) int.Parse(IGCTask.digits(strArray3[0].Substring(1)), (IFormatProvider) CultureInfo.InvariantCulture) + double.Parse(IGCTask.digits(strArray3[1]), (IFormatProvider) CultureInfo.InvariantCulture) / 60.0 + double.Parse(IGCTask.digits(strArray3[2]), (IFormatProvider) CultureInfo.InvariantCulture) / 3600.0;
            this.lon = strArray3[0].StartsWith("E") ? this.lon : -1.0 * this.lon;
            string str = strArray1[2];
            this.alt_feet = double.Parse(IGCTask.digits(strArray1[2]), (IFormatProvider) CultureInfo.InvariantCulture);
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Bad world position in wp {this.name} {ex.ToString()}");
            this.name = "";
            break;
          }
        }
        XmlNode xmlNode2 = wp_node?.SelectSingleNode(".//ICAOIdent");
        if (xmlNode2 != null)
        {
          try
          {
            this.icao = xmlNode2.InnerText;
            break;
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Bad ICAO ident in wp {this.name} {ex.ToString()}");
            this.icao = "";
            break;
          }
        }
        else
        {
          Console.WriteLine("No ICAO ident in wp " + this.name);
          break;
        }
    }
  }
}
