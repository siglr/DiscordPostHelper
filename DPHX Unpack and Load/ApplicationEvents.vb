Imports System.IO
Imports CefSharp
Imports CefSharp.WinForms

Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(
            sender As Object,
            e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs
          ) Handles Me.Startup

            ' Base folder where your EXE and all the CefSharp bits live
            Dim baseDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase

            ' Configure CefSettings
            Dim settings = New CefSettings With {
                                          .CachePath = Path.Combine(baseDir, "cef_cache_DPHX"),
                                          .BrowserSubprocessPath = Path.Combine(baseDir, "CefSharp.BrowserSubprocess.exe"),
                                          .ResourcesDirPath = baseDir,
                                          .LocalesDirPath = Path.Combine(baseDir, "locales"),
                                          .Locale = "en-US",
                                          .LogSeverity = LogSeverity.Error
}

            ' you already have this:
            settings.CefCommandLineArgs.Add("disable-extensions", "1")

            ' add these three:
            settings.CefCommandLineArgs.Add("disable-blink-features", "WebUSB")
            settings.CefCommandLineArgs.Add("disable-gcm", "1")
            settings.CefCommandLineArgs.Add("disable-push-api", "1")

            Cef.Initialize(settings, performDependencyCheck:=True, browserProcessHandler:=Nothing)

        End Sub

    End Class
End Namespace
