# NB21 Logger Sample Application

This Windows Forms sample re-hosts the original NB21 Logger UI while delegating
all logging, SimConnect, and IGC responsibilities to the headless
`NB21_logger` class library. It demonstrates how an application such as DPHX
can embed the DLL and still present the familiar desktop interface.

## Features

* Identical tabs, status indicators, and controls from the legacy NB21 Logger UI.
* Programmatic configuration of pilot identity, auto-start, and tracklog
  folders that flow directly into the library.
* Drag-and-drop or file-picker loading of `.pln` flight plans through the
  library API.
* Real-time updates of connection status, recording timers, aircraft, and flight
  plan information via the library’s state events.
* Notifications when WebSocket clients connect and when an IGC file is
  finalized, including drag-out access to the completed tracklog.

## Running the sample

1. Build the solution (requires .NET Framework 4.8 targeting pack and SimConnect
   dependencies used by the original application):

   ```bash
   dotnet build NB21Logger.Sample/NB21Logger.Sample.csproj
   ```

2. Launch the WinForms harness:

   ```bash
   dotnet run --project NB21Logger.Sample/NB21Logger.Sample.csproj
   ```

   Use the **Task** button or drag-and-drop to load flight plans, adjust
   settings on the **Settings** tab, and observe status updates driven by the
   shared library.
