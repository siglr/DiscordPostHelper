Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Pipes
Imports System.Threading
Imports System.Windows.Forms

Public Class ListenerContext
    Inherits ApplicationContext

    Private _listener As DPHXLocalWS
    Private _notifyIcon As NotifyIcon
    Public Shared SessionSettings As New AllSettings

    Public Sub New()
        InitializeComponent()
        StartListener()
    End Sub

    Private Sub InitializeComponent()
        ' --- tray icon & menu ---
        _notifyIcon = New NotifyIcon()
        Dim icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wsglistener.ico")
        _notifyIcon.Icon = New Icon(icoPath)
        _notifyIcon.Text = "WeSimGlide DPHX Listener"
        Dim menu = New ContextMenuStrip()
        menu.ShowImageMargin = False
        menu.ShowCheckMargin = False
        menu.Items.Add("Open DPHX Unpack && Load", Nothing, AddressOf OnOpenDPHX)
        menu.Items.Add("Restart Listener", Nothing, AddressOf OnRestart)
        menu.Items.Add("End Listener", Nothing, AddressOf OnExit)
        _notifyIcon.ContextMenuStrip = menu
        _notifyIcon.Visible = True
    End Sub

    Private Sub StartListener()
        If _listener IsNot Nothing AndAlso _listener.IsRunning Then Return
        ' Read port from shared settings
        Dim portValue = SessionSettings.LocalWebServerPort
        Dim port As Integer
        If Not Integer.TryParse(portValue, port) Then port = 54513

        _listener = New DPHXLocalWS(port)
        AddHandler _listener.RequestReceived, AddressOf OnHttpRequest
        _listener.Start()
    End Sub

    Private Sub StopListener()
        If _listener Is Nothing Then Return
        RemoveHandler _listener.RequestReceived, AddressOf OnHttpRequest
        _listener.Stop()
        _listener.Dispose()
        _listener = Nothing
    End Sub

    Private Sub OnHttpRequest(sender As Object, e As RequestReceivedEventArgs)
        Dim req = e.Context.Request
        Dim ctx = e.Context

        ' Handle CORS preflight
        If req.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase) Then
            ctx.Response.AddHeader("Access-Control-Allow-Origin", "*")
            ctx.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST,OPTIONS")
            ctx.Response.StatusCode = 204
            ctx.Response.OutputStream.Close()
            Return
        End If

        If Not req.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) Then
            ctx.Response.StatusCode = 405
            ctx.Response.OutputStream.Close()
            Return
        End If

        Dim path = req.Url.AbsolutePath.Trim("/"c).ToLowerInvariant()

        Select Case path
            Case "", "download-task"
                Dim taskId = req.QueryString("taskID")
                Dim title = req.QueryString("title")
                Dim sourceVal = req.QueryString("source")
                Dim fromEvent = String.Equals(sourceVal, "event", StringComparison.OrdinalIgnoreCase)

                If String.IsNullOrEmpty(taskId) OrElse String.IsNullOrEmpty(title) Then
                    _listener.SendResponse(ctx, "Missing taskID or title", 400)
                    Return
                End If

                ' Properly formatted JSON payload:
                Dim payload = $"{{""action"":""download-task"",""taskID"":""{taskId}"",""title"":""{title}"",""source"":""{If(fromEvent, "event", "")}""}}"
                SendToDPHX(payload)
                _listener.SendResponse(ctx, "OK")

            Case "shutdown"
                _listener.SendResponse(ctx, "Shutting down")
                StopListener()
                Application.Exit()

            Case "set-port"
                Dim newPort As Integer
                If Integer.TryParse(req.QueryString("port"), newPort) Then
                    _listener.SendResponse(ctx, $"Port set to {newPort}")
                    _listener.Stop()
                    _listener.Port = newPort
                    _listener.Start()
                Else
                    _listener.SendResponse(ctx, "Invalid port", 400)
                End If

            Case Else
                _listener.SendResponse(ctx, "Unknown command", 404)
        End Select
    End Sub

    Private Sub OnOpenDPHX(sender As Object, e As EventArgs)
        HandleOpen()
    End Sub

    Private Sub HandleOpen()
        SendToDPHX("{""action"":""foreground""}")
    End Sub

    Private Sub EnsureDPHXRunning()
        Dim procs = Process.GetProcessesByName("DPHX Unpack and Load")
        If procs.Length = 0 Then
            Dim exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DPHX Unpack and Load.exe")
            Process.Start(exe)
            Threading.Thread.Sleep(1000)
        End If
    End Sub

    Private Sub SendToDPHX(message As String)
        Try
            EnsureDPHXRunning()
            Using client = New NamedPipeClientStream(".", "DPHXPipe", PipeDirection.Out)
                client.Connect(2000)
                Using writer = New StreamWriter(client)
                    writer.AutoFlush = True
                    writer.Write(message)
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Unable to contact DPHX: " & ex.Message, "WeSimGlide DPHX Listener", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub OnRestart(sender As Object, e As EventArgs)
        StopListener()
        StartListener()
    End Sub

    Private Sub OnExit(sender As Object, e As EventArgs)
        StopListener()
        _notifyIcon.Visible = False
        Application.Exit()
    End Sub
End Class
