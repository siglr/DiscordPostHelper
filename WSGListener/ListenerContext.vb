Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.IO.Pipes
Imports System.Windows.Forms
Imports System.Drawing

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
        Dim path = req.Url.AbsolutePath.ToLowerInvariant().TrimStart("/"c)

        Select Case path
            Case "open"
                Dim fileParam = req.QueryString("file")
                If String.IsNullOrEmpty(fileParam) Then
                    _listener.SendResponse(e.Context, "Missing file parameter", 400)
                    Return
                End If
                HandleOpen(fileParam)
                _listener.SendResponse(e.Context, "OK")

            Case "shutdown"
                _listener.SendResponse(e.Context, "Shutting down")
                StopListener()
                Application.Exit()

            Case "set-port"
                Dim newPort = 0
                If Integer.TryParse(req.QueryString("port"), newPort) Then
                    _listener.SendResponse(e.Context, $"Port set to {newPort}")
                    _listener.Stop()
                    _listener.Port = newPort
                    _listener.Start()
                Else
                    _listener.SendResponse(e.Context, "Invalid port", 400)
                End If

            Case Else
                _listener.SendResponse(e.Context, "Unknown command", 404)
        End Select
    End Sub

    Private Sub OnOpenDPHX(sender As Object, e As EventArgs)
        HandleOpen(Nothing)
    End Sub

    Private Sub HandleOpen(filePath As String)
        EnsureDPHXRunning()
        Dim msg As String
        If Not String.IsNullOrEmpty(filePath) Then
            msg = $"{{ ""action"": ""open-dphx"", ""filePath"": ""{filePath.Replace("\", "\\")}"" }}"
        Else
            msg = "{ ""action"": ""foreground"" }"
        End If
        SendToDPHX(msg)
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
