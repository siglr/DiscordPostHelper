// Decompiled with JetBrains decompiler
// Type: NB21_logger.Program
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

#nullable enable
namespace NB21_logger;

internal static class Program
{
  public static bool LaunchedViaStartup { get; set; }

  [STAThread]
  private static void Main(string[] args)
  {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    if (Process.GetProcessesByName("NB21_Logger").Length > 1)
    {
      int num = (int) MessageBox.Show("NB21 Logger is already running");
    }
    else
    {
      Program.LaunchedViaStartup = args != null && ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (arg => arg.Equals("startup", StringComparison.CurrentCultureIgnoreCase)));
      Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
      Console.WriteLine($"NB21 Logger program startup (with LaunchedViaStartup = {Program.LaunchedViaStartup})");
      Application.Run((Form) new NB21Logger(Program.LaunchedViaStartup));
    }
  }
}
