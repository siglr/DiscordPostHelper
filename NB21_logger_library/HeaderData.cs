// Decompiled with JetBrains decompiler
// Type: NB21_logger.HeaderData
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System.Runtime.InteropServices;

#nullable enable
namespace NB21_logger;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HeaderData
{
  public double wingspan_m;
  public double flap_positions;
  public double local_year;
  public double local_month;
  public double local_day;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256 /*0x0100*/)]
  public string title_str;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256 /*0x0100*/)]
  public string atc_id_str;
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256 /*0x0100*/)]
  public string atc_type_str;
}
