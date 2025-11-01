// Decompiled with JetBrains decompiler
// Type: NB21_logger.IGCUtil
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.Text;

#nullable enable
namespace NB21_logger;

public class IGCUtil
{
  private SimDataConn simdata;

  public IGCUtil(SimDataConn simdata) => this.simdata = simdata;

  public string t(string str)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(this.simdata.c_str(str));
    return BitConverter.ToString(this.simdata.s.ComputeHash(bytes, 0, bytes.Length)).Replace("-", "").Substring(12, 24);
  }
}
