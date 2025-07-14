Imports System.IO

Namespace My
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(
            sender As Object,
            e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs
          ) Handles Me.Startup

            Dim settings = New CefSharp.WinForms.CefSettings() With {
              .CachePath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                "cef_cache_DPHX"
              ),
              .BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                "CefSharp.BrowserSubprocess.exe"
              )
            }
            CefSharp.Cef.Initialize(settings, performDependencyCheck:=True, browserProcessHandler:=Nothing)

        End Sub

    End Class
End Namespace
