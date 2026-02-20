' Startup.vb
Imports System.IO
Imports System.Threading
Imports System.Diagnostics
Imports System.Windows.Forms
Imports CefSharp
Imports CefSharp.WinForms

Module Startup

    Private Const CefUserAgent As String = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
    Private Const CefLocale As String = "en-US"
    Private _cefCachePath As String

    Public ReadOnly Property CefCachePath As String
        Get
            Return _cefCachePath
        End Get
    End Property

    Public ReadOnly Property CookieDbPath As String
        Get
            If String.IsNullOrWhiteSpace(_cefCachePath) Then
                Return String.Empty
            End If
            Return Path.Combine(_cefCachePath, "Default", "Network", "Cookies")
        End Get
    End Property

    Public Sub LogCefDiagnostic(message As String)
        Trace.WriteLine($"[CefSharp] {message}")
    End Sub

    <STAThread()>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        ' Put CefSharp cache/profile on a local drive (Discord login persistence needs Cookies DB)
        _cefCachePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoIGCParserBatch",
            "cef_cache"
        )
        Directory.CreateDirectory(_cefCachePath)
        LogCefDiagnostic($"CachePath = {_cefCachePath}")
        LogCefDiagnostic($"CookieDbPath = {CookieDbPath}")
        LogCefDiagnostic($"CookieDbExists = {File.Exists(CookieDbPath)}")

        Dim settings = New CefSettings() With {
            .CachePath = _cefCachePath,
            .BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                "CefSharp.BrowserSubprocess.exe"
            ),
            .PersistSessionCookies = True,
            .UserAgent = CefUserAgent,
            .Locale = CefLocale
        }

        ' MUST happen before any ChromiumWebBrowser is instantiated
        Cef.Initialize(settings, performDependencyCheck:=True, browserProcessHandler:=Nothing)
        LogCefDiagnostic($"Cef.IsInitialized = {Cef.IsInitialized}")
        LogCefDiagnostic($"UserAgent = {CefUserAgent}")

        AddHandler Application.ApplicationExit, AddressOf OnApplicationExit

        Try
            GliderMatcher.EnsureRulesLoaded()
            LogCefDiagnostic("Glider match rules loaded.")
        Catch ex As Exception
            MessageBox.Show($"Unable to load glider match rules: {ex.Message}", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Application.Run(New frmDiscord())
    End Sub

    Private Sub OnApplicationExit(sender As Object, e As EventArgs)
        If Not Cef.IsInitialized Then
            Return
        End If

        LogCefDiagnostic("ApplicationExit: flushing cookie store.")
        Dim cookieManager = Cef.GetGlobalCookieManager()
        If cookieManager IsNot Nothing Then
            Using completionEvent As New ManualResetEventSlim(False)
                Using callback As New CefShutdownCompletionCallback(completionEvent)
                    cookieManager.FlushStore(callback)
                    completionEvent.Wait(TimeSpan.FromSeconds(5))
                End Using
            End Using
        End If

        Cef.Shutdown()
        LogCefDiagnostic("Cef.Shutdown completed.")
    End Sub

    Private NotInheritable Class CefShutdownCompletionCallback
        Implements ICompletionCallback

        Private ReadOnly _completionEvent As ManualResetEventSlim
        Private _isDisposed As Boolean

        Public Sub New(completionEvent As ManualResetEventSlim)
            _completionEvent = completionEvent
        End Sub

        Public Sub OnComplete() Implements ICompletionCallback.OnComplete
            _completionEvent.Set()
        End Sub

        Public ReadOnly Property IsDisposed As Boolean Implements ICompletionCallback.IsDisposed
            Get
                Return _isDisposed
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
            _isDisposed = True
        End Sub
    End Class

End Module
