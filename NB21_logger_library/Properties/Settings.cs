// Decompiled with JetBrains decompiler
// Type: NB21_logger.Properties.Settings
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
namespace NB21_logger.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.7.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
  private static 
  #nullable disable
  Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

  public static Settings Default => Settings.defaultInstance;

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("")]
  public string PilotName
  {
    get => (string) this[nameof (PilotName)];
    set => this[nameof (PilotName)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("")]
  public string IGCPath
  {
    get => (string) this[nameof (IGCPath)];
    set => this[nameof (IGCPath)] = (object) value;
  }

  [ApplicationScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("NB21 Logger")]
  public string AppName => (string) this[nameof (AppName)];

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("")]
  public string PilotId
  {
    get => (string) this[nameof (PilotId)];
    set => this[nameof (PilotId)] = (object) value;
  }

  [ApplicationScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("1.2.3")]
  public string AppVersion => (string) this[nameof (AppVersion)];

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("True")]
  public bool SettingsUpgradeRequired
  {
    get => (bool) this[nameof (SettingsUpgradeRequired)];
    set => this[nameof (SettingsUpgradeRequired)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("False")]
  public bool WindowsStart
  {
    get => (bool) this[nameof (WindowsStart)];
    set => this[nameof (WindowsStart)] = (object) value;
  }

  private void SettingChangingEventHandler(
  #nullable enable
  object sender, SettingChangingEventArgs e)
  {
  }

  private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
  {
  }
}
