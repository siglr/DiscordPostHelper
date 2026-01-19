Imports System.Diagnostics
Imports System.IO
Imports System.Windows.Forms
Imports CefSharp
Imports CefSharp.WinForms

Namespace My
    Partial Friend Class MyApplication

        Private Sub PromptToRemoveDeprecatedWhitelistFolder()
            Dim whitelistFolder As String = Path.Combine(Application.StartupPath, "Whitelist")

            If Not Directory.Exists(whitelistFolder) Then
                Return
            End If

            My.Application.Log.WriteEntry($"Deprecated Whitelist folder detected at {whitelistFolder}.", TraceEventType.Information)

            Dim message As String = "The Whitelist folder is no longer used by DPHX and can be safely removed." &
                                    $"{Environment.NewLine}Would you like to remove it now?" &
                                    $"{Environment.NewLine}{Environment.NewLine}If you choose not to, this message will appear again the next time DPHX starts."

            Dim result As DialogResult = MessageBox.Show(
                message,
                "Deprecated Whitelist Folder",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            )

            If result = DialogResult.Yes Then
                My.Application.Log.WriteEntry("User agreed to remove deprecated Whitelist folder.", TraceEventType.Information)
                Try
                    Directory.Delete(whitelistFolder, recursive:=True)
                    My.Application.Log.WriteEntry($"Deprecated Whitelist folder removed: {whitelistFolder}.", TraceEventType.Information)
                Catch ex As Exception
                    My.Application.Log.WriteEntry($"Failed to remove deprecated Whitelist folder at {whitelistFolder}: {ex.Message}", TraceEventType.Error)
                End Try
            Else
                My.Application.Log.WriteEntry("User chose to keep deprecated Whitelist folder for now.", TraceEventType.Information)
            End If
        End Sub

        Private Sub MyApplication_Startup(
            sender As Object,
            e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs
        ) Handles Me.Startup
            Try
                ' Base folder where your EXE and all the CefSharp bits live
                Dim baseDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase

                PromptToRemoveDeprecatedWhitelistFolder()

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
