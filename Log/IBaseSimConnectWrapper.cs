// Decompiled with JetBrains decompiler
// Type: NB21_logger.IBaseSimConnectWrapper
// Assembly: NB21_logger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7F4134B2-0CAD-453E-BB8E-645D7AF60F5A
// Assembly location: H:\MSFlightSimulator\NB21 Logger\Normal\NB21_logger.exe

using System;

#nullable disable
namespace NB21_logger;

internal interface IBaseSimConnectWrapper
{
  int GetUserSimConnectWinEvent();

  void ReceiveSimConnectMessage();

  void SetWindowHandle(IntPtr _hWnd);

  void Disconnect();
}
