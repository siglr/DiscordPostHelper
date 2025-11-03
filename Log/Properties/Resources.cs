// Decompiled with JetBrains decompiler
// Type: NB21_logger.Properties.Resources
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace NB21_logger.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (NB21_logger.Properties.Resources.resourceMan == null)
        NB21_logger.Properties.Resources.resourceMan = new ResourceManager("NB21_logger.Properties.Resources", typeof (NB21_logger.Properties.Resources).Assembly);
      return NB21_logger.Properties.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => NB21_logger.Properties.Resources.resourceCulture;
    set => NB21_logger.Properties.Resources.resourceCulture = value;
  }

  internal static Icon app_icon
  {
    get => (Icon) NB21_logger.Properties.Resources.ResourceManager.GetObject(nameof (app_icon), NB21_logger.Properties.Resources.resourceCulture);
  }

  internal static Bitmap file_icon
  {
    get
    {
      return (Bitmap) NB21_logger.Properties.Resources.ResourceManager.GetObject(nameof (file_icon), NB21_logger.Properties.Resources.resourceCulture);
    }
  }

  internal static Bitmap recording_tick
  {
    get
    {
      return (Bitmap) NB21_logger.Properties.Resources.ResourceManager.GetObject(nameof (recording_tick), NB21_logger.Properties.Resources.resourceCulture);
    }
  }

  internal static Bitmap recording_tock
  {
    get
    {
      return (Bitmap) NB21_logger.Properties.Resources.ResourceManager.GetObject(nameof (recording_tock), NB21_logger.Properties.Resources.resourceCulture);
    }
  }
}
