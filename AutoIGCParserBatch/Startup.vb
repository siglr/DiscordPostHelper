' Startup.vb
Imports System.IO
Imports System.Windows.Forms

Module Startup
    <STAThread()>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' 2) CefSharp settings — give this app its own cache folder
        Dim settings = New CefSharp.WinForms.CefSettings() With {
      .CachePath = Path.Combine(
        AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        "cef_cache_AutoIGCParserBatch"    ' ← unique per app
      ),
      .BrowserSubprocessPath = Path.Combine(
        AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
        "CefSharp.BrowserSubprocess.exe"
      )
    }
        ' 3) MUST happen before any ChromiumWebBrowser is instantiated
        CefSharp.Cef.Initialize(settings, performDependencyCheck:=True, browserProcessHandler:=Nothing)


        Application.Run(New frmDiscord())
    End Sub
End Module
