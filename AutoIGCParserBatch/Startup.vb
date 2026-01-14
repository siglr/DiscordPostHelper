' Startup.vb
Imports System.IO
Imports System.Windows.Forms

Module Startup
    <STAThread()>
    Public Sub Main()
        AddHandler Application.ThreadException, AddressOf OnUiThreadException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf OnUnhandledException
        AddHandler Threading.Tasks.TaskScheduler.UnobservedTaskException, AddressOf OnUnobservedTaskException

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

    Private Sub OnUiThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
        LogUnhandledException("UI thread exception", e.Exception)
    End Sub

    Private Sub OnUnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        Dim ex = TryCast(e.ExceptionObject, Exception)
        LogUnhandledException("Unhandled exception", ex)
    End Sub

    Private Sub OnUnobservedTaskException(sender As Object, e As Threading.Tasks.UnobservedTaskExceptionEventArgs)
        LogUnhandledException("Unobserved task exception", e.Exception)
        e.SetObserved()
    End Sub

    Private Sub LogUnhandledException(context As String, ex As Exception)
        Try
            Dim baseDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            Dim logPath = Path.Combine(baseDir, "AutoIGCParserBatch.unhandled.log")
            Dim message = $"[{DateTime.UtcNow:u}] {context}{Environment.NewLine}{ex}{Environment.NewLine}"
            File.AppendAllText(logPath, message)
            MessageBox.Show($"{context}.{Environment.NewLine}Details logged to:{Environment.NewLine}{logPath}", "AutoIGCParserBatch Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch
            ' Swallow any logging failures to avoid recursive crashes.
        End Try
    End Sub
End Module
