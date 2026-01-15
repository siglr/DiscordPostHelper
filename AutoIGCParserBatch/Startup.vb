' Startup.vb
Imports System.IO
Imports System.Windows.Forms
Imports CefSharp
Imports CefSharp.WinForms

Module Startup

    <STAThread()>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Put CefSharp cache/profile on a local drive (Discord login persistence needs Cookies DB)
        Dim cachePath As String = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoIGCParserBatch",
            "cef_cache"
        )
        Directory.CreateDirectory(cachePath)

        Dim settings = New CefSettings() With {
            .CachePath = cachePath,
            .BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                "CefSharp.BrowserSubprocess.exe"
            ),
            .PersistSessionCookies = True
        }

        ' MUST happen before any ChromiumWebBrowser is instantiated
        Cef.Initialize(settings, performDependencyCheck:=True, browserProcessHandler:=Nothing)

        Application.Run(New frmDiscord())
    End Sub

End Module
