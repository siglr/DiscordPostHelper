# NB21 Logger Migration Notes

The NB21 logger has been refactored into a headless class library that can be
embedded into external automation.

## Removed UI features

* WinForms shell (home/settings tabs, tray icon, message boxes).
* Windows auto-start integration and registry updates.
* File picker dialogs and Explorer integrations for IGC folders.
* Task planner launcher and drag/drop UI.
* Status labels, blinking indicators, and other WinForms-only widgets.

## New public surface

* `LoggerConfiguration` – strongly-typed settings container for pilot and
  runtime configuration (pilot identity, track log folder, web ports, etc.).
* `NB21Logger` – headless orchestration class exposing:
  * `Start()` / `Stop()` / `Dispose()` for lifecycle management.
  * `SetFlightPlan(string xml)` / `SetFlightPlanFromFile(string path)` /
    `ClearFlightPlan()` for programmatic task control.
  * `State` and `StateChanged` event providing MSFS connection status, aircraft,
    sim time (UTC), and recording information.
  * `IgcFileFinalized` event notifying when IGC files are written.
  * `WebSocketClientConnected` / `WebSocketClientDisconnected` events for
    tracking client connections.
* `NB21LoggerState`, `LoggerStateChangedEventArgs`, and
  `IgcFileFinalizedEventArgs` – new data contracts for event consumers.

See `NB21Logger.Sample` for a full WinForms shell that mirrors the legacy
interface while driving the new API under the hood.

## Compatibility preservation

* `SimDataConn`, `IGCFileWriter`, and `Broadcaster` retain their original
  implementations so upstream updates can be merged without large diffs.
* A lightweight `SimConnectMessageWindow` hosts the Win32 message pump that the
  original `SimDataConn` expects, allowing the class to operate without the
  WinForms shell.
* The headless `NB21Logger` populates `Properties.Settings` at runtime so the
  legacy logging classes continue to read pilot identity, version, and IGC path
  values exactly as before.
