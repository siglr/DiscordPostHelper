Imports System.IO
Imports System.Windows.Forms
Imports CefSharp
Imports CefSharp.WinForms

Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(
            sender As Object,
            e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs
        ) Handles Me.Startup
            Try
                ' Base folder where your EXE and all the CefSharp bits live
                Dim baseDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase

                ' Delete the cache directory if it exists
                Dim cacheDir = Path.Combine(baseDir, "cef_cache_DPHX")
                If Directory.Exists(cacheDir) Then
                    Directory.Delete(cacheDir, recursive:=True)
                End If

                ' Configure CefSettings
                Dim settings = New CefSettings With {
                    .CachePath = Path.Combine(baseDir, "cef_cache_DPHX"),
                    .BrowserSubprocessPath = Path.Combine(baseDir, "CefSharp.BrowserSubprocess.exe"),
                    .ResourcesDirPath = baseDir,
                    .LocalesDirPath = Path.Combine(baseDir, "locales"),
                    .Locale = "en-US",
                    .LogSeverity = LogSeverity.Error
                }

                settings.CefCommandLineArgs.Add("disable-extensions", "1")
                settings.CefCommandLineArgs.Add("disable-blink-features", "WebUSB")
                settings.CefCommandLineArgs.Add("disable-gcm", "1")
                settings.CefCommandLineArgs.Add("disable-push-api", "1")

                Cef.Initialize(settings, performDependencyCheck:=True, browserProcessHandler:=Nothing)

            Catch ex As Exception
                ' Show the error and stop the app from continuing
                MessageBox.Show(
                    $"Failed to start application:{Environment.NewLine}{ex.Message}",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                )
                e.Cancel = True
            End Try
        End Sub

    End Class
End Namespace
