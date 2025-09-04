Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Pipes
Imports System.Threading
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq

Public Class ListenerContext
    Inherits ApplicationContext

    Private Const READY_EVENT_NAME As String = "WSG_DPHX_READY"

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

        SessionSettings.Load()

        ' Wire up click events:
        AddHandler _notifyIcon.MouseClick, AddressOf OnNotifyIconClick
        AddHandler _notifyIcon.MouseDoubleClick, AddressOf OnNotifyIconDoubleClick

        Dim menu = New ContextMenuStrip()
        menu.ShowImageMargin = False
        menu.ShowCheckMargin = False

        menu.Items.Add("Open DPHX Unpack && Load", Nothing, AddressOf OnOpenDPHX)
        Dim wsg = New ToolStripMenuItem("WeSimGlide.org")
        ' Turn off the image/check margins on its drop-down:
        Dim dd = CType(wsg.DropDown, ToolStripDropDownMenu)
        dd.ShowImageMargin = False
        dd.ShowCheckMargin = False
        wsg.DropDownItems.Add("🏠 Home", Nothing, AddressOf OnWSGOpenHome)
        wsg.DropDownItems.Add("📆 Events", Nothing, AddressOf OnWSGOpenEvents)
        wsg.DropDownItems.Add("🌐 Map", Nothing, AddressOf OnWSGOpenMap)
        menu.Items.Add(wsg)
        menu.Items.Add(New ToolStripSeparator())
        menu.Items.Add("Restart Listener", Nothing, AddressOf OnRestart)
        menu.Items.Add(New ToolStripSeparator())
        menu.Items.Add("End Listener", Nothing, AddressOf OnExit)

        _notifyIcon.ContextMenuStrip = menu
        _notifyIcon.Visible = True
    End Sub

    Private Sub OnNotifyIconClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            ' single‐click behavior - nothing
        End If
    End Sub

    Private Sub OnNotifyIconDoubleClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            ' double‐click behavior
            OnWSGOpenHome(sender, e)
        End If
    End Sub

    Private Sub OnWSGOpenHome(sender As Object, e As EventArgs)
        Process.Start("https://wesimglide.org/index.html?tab=home")
    End Sub

    Private Sub OnWSGOpenEvents(sender As Object, e As EventArgs)
        Process.Start("https://wesimglide.org/index.html?tab=events")
    End Sub

    Private Sub OnWSGOpenMap(sender As Object, e As EventArgs)
        Process.Start("https://wesimglide.org/index.html?tab=map")
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
                'SendToDPHX("{""action"":""foreground""}", True)
                SendToDPHX(payload, True)
                _listener.SendResponse(ctx, "OK")

            Case "receive-user-info"
                Dim raw = req.QueryString("user-info")
                If String.IsNullOrEmpty(raw) Then
                    _listener.SendResponse(ctx, "Missing user info", 400) : Return
                End If

                Dim jsonStr = Uri.UnescapeDataString(raw)

                Dim jo As JObject
                Try
                    jo = JObject.Parse(jsonStr)
                Catch
                    _listener.SendResponse(ctx, "Invalid JSON", 400) : Return
                End Try

                ' --- extract incoming values (with safe defaults) ---
                Dim newId As Integer = (If(jo.Value(Of Integer?)("id"), 0))
                Dim newComp As String = If(jo.Value(Of String)("compId"), String.Empty)
                Dim newPilot As String = If(jo.Value(Of String)("pilotName"), String.Empty)
                Dim newDisplay As String = If(jo.Value(Of String)("displayName"), String.Empty)

                ' --- detect change against current settings ---
                Dim changed As Boolean =
                    (SessionSettings.WSGUserID <> newId) OrElse
                    Not String.Equals(SessionSettings.WSGCompID, newComp, StringComparison.Ordinal) OrElse
                    Not String.Equals(SessionSettings.WSGPilotName, newPilot, StringComparison.Ordinal) OrElse
                    Not String.Equals(SessionSettings.WSGDisplayName, newDisplay, StringComparison.Ordinal)

                ' --- update settings ---
                If changed Then
                    SessionSettings.WSGUserID = newId
                    SessionSettings.WSGCompID = newComp
                    SessionSettings.WSGPilotName = newPilot
                    SessionSettings.WSGDisplayName = newDisplay
                    SessionSettings.Save()

                    ' --- notify DPHX only if something changed ---
                    Dim reloadPayload = New JObject(
                            New JProperty("action", "reload-user-info"),
                            New JProperty("user", jo)
                        ).ToString()
                    SendToDPHX(reloadPayload, True, False)
                End If

                _listener.SendResponse(ctx, "OK")

            Case "shutdown"
                _listener.SendResponse(ctx, "Shutting down")
                StopListener()
                Application.Exit()

            Case "health"
                _listener.SendResponse(ctx, "Ok")

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
        SendToDPHX("{""action"":""foreground""}", False)
    End Sub

    Private Sub EnsureDPHXRunning(preventWSGFromOpening As Boolean)
        ' Look for any existing instance
        Dim procs = Process.GetProcessesByName("DPHX Unpack and Load")
        If procs.Length = 0 Then
            Dim exe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DPHX Unpack and Load.exe")
            Dim args = If(preventWSGFromOpening, "--prevent-wsg", String.Empty)

            Dim psi As New ProcessStartInfo(exe) With {
            .Arguments = args,
            .UseShellExecute = True,
            .WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
        }
            Try
                Process.Start(psi)
                Threading.Thread.Sleep(1000)
            Catch ex As Exception
                MessageBox.Show($"Could not start DPHX Unpack and Load: {ex.Message}",
                            "WSGListener Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function WaitUntilDPHXReady(preventWSGFromOpening As Boolean,
                                    Optional timeoutMs As Integer = 30000,
                                    Optional allowAutoLaunch As Boolean = True) As Boolean
        ' Only auto-launch when allowed
        If allowAutoLaunch Then
            EnsureDPHXRunning(preventWSGFromOpening)
        End If

        Dim sw = Stopwatch.StartNew()
        Do
            Try
                Using ev = EventWaitHandle.OpenExisting(READY_EVENT_NAME)
                    If ev.WaitOne(0) Then Return True

                    If Not allowAutoLaunch Then
                        ' No spinning when we’re not allowed to launch/wait
                        Return False
                    End If

                    Dim remaining = CInt(Math.Max(0, timeoutMs - sw.ElapsedMilliseconds))
                    Dim slice = Math.Min(500, remaining)
                    If slice <= 0 Then Exit Do
                    If ev.WaitOne(slice) Then Return True
                End Using
            Catch ex As WaitHandleCannotBeOpenedException
                If Not allowAutoLaunch Then Return False
                Thread.Sleep(200) ' backoff only if we might be launching
            Catch ex As UnauthorizedAccessException
                Return False
            End Try
        Loop While sw.ElapsedMilliseconds < timeoutMs

        Return False
    End Function

    Private Sub SendToDPHX(message As String,
                       preventWSGFromOpening As Boolean,
                       Optional allowAutoLaunch As Boolean = True)
        Try
            If Not WaitUntilDPHXReady(preventWSGFromOpening, 5000, allowAutoLaunch) Then
                Return
            End If

            Using client = New NamedPipeClientStream(".", "DPHXPipe", PipeDirection.Out)
                client.Connect(2000)
                Using writer = New StreamWriter(client)
                    writer.AutoFlush = True
                    writer.Write(message) ' DPHX reads ReadToEnd()
                End Using
            End Using

        Catch ex As Exception
            If allowAutoLaunch Then
                MessageBox.Show("Unable to contact DPHX: " & ex.Message,
                            "WeSimGlide DPHX Listener",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
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
