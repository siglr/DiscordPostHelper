Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports NB21_logger

Public Class Logger

    Private ReadOnly _config As LoggerConfiguration
    Private ReadOnly _logger As NB21Logger
    Private ReadOnly _blinkingTimer As Timer
    Private ReadOnly _uiRefreshTimer As Timer

    Private ReadOnly _notConnectedImage As Image
    Private ReadOnly _connectedImage As Image
    Private ReadOnly _recordingTickImage As Image
    Private ReadOnly _recordingTockImage As Image

    Private _state As NB21LoggerState
    Private _recordingTickTock As Boolean
    Private _finalizedIgcPath As String = String.Empty
    Private _compactView As Boolean

    Public Sub New(pilotName As String, competitionID As String, tracklogsPath As String)

        ' This call is required by the designer.
        InitializeComponent()

        _config = New LoggerConfiguration()
        _config.AppName = "NB21 Logger for DPHX"
        _config.AppVersion = "1.2.3"
        _config.PilotName = pilotName
        _config.PilotId = competitionID
        _config.TracklogsDirectory = tracklogsPath

        _logger = New NB21Logger(_config)
        _state = _logger.State

        AddHandler _logger.StateChanged, AddressOf Logger_StateChanged
        AddHandler _logger.IgcFileFinalized, AddressOf Logger_IgcFileFinalized
        AddHandler _logger.WebSocketClientConnected, AddressOf Logger_WebSocketClientConnected
        AddHandler _logger.WebSocketClientDisconnected, AddressOf Logger_WebSocketClientDisconnected
        AddHandler _logger.AppEvent, AddressOf Logger_AppEvent

        _notConnectedImage = CreateStatusImage(Color.DimGray)
        _connectedImage = CreateStatusImage(Color.DodgerBlue)
        _recordingTickImage = CreateStatusImage(Color.Red)
        _recordingTockImage = CreateStatusImage(Color.OrangeRed)

        _blinkingTimer = New Timer With {.Interval = 1000}
        AddHandler _blinkingTimer.Tick, AddressOf BlinkingTimer_Tick

        _uiRefreshTimer = New Timer With {.Interval = 1000}
        AddHandler _uiRefreshTimer.Tick, AddressOf UiRefreshTimer_Tick

    End Sub

    Private Sub Logger_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SetSize()
        SetPosition()

        Text = $"{_config.AppName} {_config.AppVersion}"
        ui_pilot.Text = $"{_config.PilotName} ({_config.PilotId})"
        ui_aircraft.Text = String.Empty
        ui_task.Text = "Drop a .PLN or click Task"
        ui_local_time.Text = String.Empty
        ui_recording_time.Visible = False
        ui_recording_time.Text = "00:00:00"
        ui_conn_status.Text = "Waiting for MSFS."
        ui_message_bar.Text = "Waiting for MSFS."
        pictureBox_statusImage.Image = _notConnectedImage

        EnsureTracklogsFolder()

        _blinkingTimer.Start()
        _uiRefreshTimer.Start()

        _logger.Start()

    End Sub

    Private Sub Logger_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SavePosition()

        _blinkingTimer.Stop()
        _uiRefreshTimer.Stop()

        RemoveHandler _logger.StateChanged, AddressOf Logger_StateChanged
        RemoveHandler _logger.IgcFileFinalized, AddressOf Logger_IgcFileFinalized
        RemoveHandler _logger.WebSocketClientConnected, AddressOf Logger_WebSocketClientConnected
        RemoveHandler _logger.WebSocketClientDisconnected, AddressOf Logger_WebSocketClientDisconnected
        RemoveHandler _logger.AppEvent, AddressOf Logger_AppEvent

        _logger.Stop()
        _logger.Dispose()
    End Sub

    Private Sub SetPosition()

        'TODO: Try to restore previous position from registry - if not found, center on parent

    End Sub

    Private Sub SavePosition()

        'TODO: Save current position to registry

    End Sub

    Private Sub SetSize()
        If _compactView Then
            Size = New Size(634, 124)
            ui_min_max.Text = "Full"
        Else
            Size = New Size(634, 320)
            ui_min_max.Text = "Compact"
        End If
    End Sub

    Private Sub ui_min_max_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles ui_min_max.LinkClicked
        _compactView = Not _compactView
        SetSize()
    End Sub

    Private Sub EnsureTracklogsFolder()
        If String.IsNullOrWhiteSpace(_config.TracklogsDirectory) Then
            Return
        End If

        Try
            If Not Directory.Exists(_config.TracklogsDirectory) Then
                Directory.CreateDirectory(_config.TracklogsDirectory)
            End If
        Catch ex As Exception
            ui_message_bar.Text = $"Unable to create tracklogs folder: {ex.Message}"
        End Try

        view_tracklogs_button.Visible = Directory.Exists(_config.TracklogsDirectory)
    End Sub

    Private Sub view_tracklogs_button_Click(sender As Object, e As EventArgs) Handles view_tracklogs_button.Click
        EnsureTracklogsFolder()

        If String.IsNullOrWhiteSpace(_config.TracklogsDirectory) Then
            Return
        End If

        Try
            Process.Start(New ProcessStartInfo With {
                .FileName = _config.TracklogsDirectory,
                .UseShellExecute = True
            })
        Catch ex As Exception
            MessageBox.Show(Me, $"Unable to open tracklogs folder: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub Logger_StateChanged(sender As Object, e As LoggerStateChangedEventArgs)
        _state = e.State

        If IsDisposed OrElse Disposing Then
            Return
        End If

        BeginInvoke(New Action(Sub() ApplyState(e.EventType, e.State)))
    End Sub

    Private Sub Logger_IgcFileFinalized(sender As Object, e As IgcFileFinalizedEventArgs)
        _finalizedIgcPath = e.FilePath

        If IsDisposed OrElse Disposing Then
            Return
        End If

        BeginInvoke(New Action(Sub()
                                   ui_message_bar.Text = $"IGC saved: {Path.GetFileName(e.FilePath)}"
                               End Sub))
    End Sub

    Private Sub Logger_WebSocketClientConnected(sender As Object, e As WebSocketClientEventArgs)
        If IsDisposed OrElse Disposing Then
            Return
        End If

        BeginInvoke(New Action(Sub()
                                   ui_message_bar.Text = If(e.IsJsonChannel, "Task planner connected.", "IGC stream connected.")
                               End Sub))
    End Sub

    Private Sub Logger_WebSocketClientDisconnected(sender As Object, e As WebSocketClientEventArgs)
        If IsDisposed OrElse Disposing Then
            Return
        End If

        BeginInvoke(New Action(Sub()
                                   ui_message_bar.Text = If(e.IsJsonChannel, "Task planner disconnected.", "IGC stream disconnected.")
                               End Sub))
    End Sub

    Private Sub Logger_AppEvent(sender As Object, e As AppEventArgs)
        If IsDisposed OrElse Disposing Then
            Return
        End If

        Dim message As String
        Select Case e.eventType
            Case APPEVENT_ID.SimOpen
                message = "Connected to MSFS."
            Case APPEVENT_ID.SimQuit
                message = "Simulator closed."
            Case APPEVENT_ID.RecordingStart
                message = "Recording started."
            Case APPEVENT_ID.RecordingStop
                message = "Recording stopped."
            Case APPEVENT_ID.IGCFileWrite
                message = "IGC file finalized."
            Case APPEVENT_ID.SimHasCrashed
                message = "Simulator crash detected."
            Case APPEVENT_ID.HttpError
                message = "HTTP error."
            Case Else
                message = e.eventType.ToString()
        End Select

        BeginInvoke(New Action(Sub() ui_message_bar.Text = message))
    End Sub

    Private Sub ApplyState(eventType As APPEVENT_ID, newState As NB21LoggerState)
        ui_conn_status.Text = newState.ConnectionStatus
        ui_aircraft.Text = If(String.IsNullOrEmpty(newState.AircraftTitle), String.Empty, newState.AircraftTitle)
        ui_task.Text = If(String.IsNullOrEmpty(newState.FlightPlanName), "Drop a .PLN or click Task", newState.FlightPlanName)
        ui_local_time.Text = If(newState.SimTimeUtc.HasValue, newState.SimTimeUtc.Value.ToString("HH:mm:ss"), String.Empty)

        ui_recording_time.Visible = newState.IsRecording
        If Not newState.IsRecording Then
            ui_recording_time.Text = "00:00:00"
        End If

        If Not String.IsNullOrEmpty(newState.LastIgcFilePath) Then
            _finalizedIgcPath = newState.LastIgcFilePath
        End If

        If Not newState.IsConnected Then
            pictureBox_statusImage.Image = _notConnectedImage
        ElseIf Not newState.IsRecording Then
            pictureBox_statusImage.Image = _connectedImage
        End If

        If eventType = APPEVENT_ID.HttpError Then
            ui_message_bar.Text = "HTTP error."
        ElseIf eventType = APPEVENT_ID.SimHasCrashed Then
            ui_message_bar.Text = "Simulator crash detected."
        End If
    End Sub

    Private Sub BlinkingTimer_Tick(sender As Object, e As EventArgs)
        If _state.IsRecording Then
            _recordingTickTock = Not _recordingTickTock
            pictureBox_statusImage.Image = If(_recordingTickTock, _recordingTickImage, _recordingTockImage)
            ui_recording_time.Text = _state.RecordingElapsed.ToString("hh\:mm\:ss")
        ElseIf _state.IsConnected Then
            pictureBox_statusImage.Image = _connectedImage
        Else
            pictureBox_statusImage.Image = _notConnectedImage
        End If
    End Sub

    Private Sub UiRefreshTimer_Tick(sender As Object, e As EventArgs)
        If _state.SimTimeUtc.HasValue Then
            ui_local_time.Text = _state.SimTimeUtc.Value.ToString("HH:mm:ss")
        End If
    End Sub

    Private Sub ui_task_Click(sender As Object, e As EventArgs) Handles ui_task.Click
        Using dialog As New OpenFileDialog()
            dialog.Filter = "Flight Plans (*.pln)|*.pln"
            dialog.Title = "Select Flight Plan"

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                LoadFlightPlan(dialog.FileName)
            End If
        End Using
    End Sub

    Private Sub ui_task_DragEnter(sender As Object, e As DragEventArgs) Handles ui_task.DragEnter
        If HasPlnFile(e.Data) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub ui_task_DragDrop(sender As Object, e As DragEventArgs) Handles ui_task.DragDrop
        If Not HasPlnFile(e.Data) Then
            Return
        End If

        Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If files Is Nothing OrElse files.Length = 0 Then
            Return
        End If

        LoadFlightPlan(files(0))
    End Sub

    Private Function HasPlnFile(data As IDataObject) As Boolean
        If data Is Nothing OrElse Not data.GetDataPresent(DataFormats.FileDrop) Then
            Return False
        End If

        Dim files = CType(data.GetData(DataFormats.FileDrop), String())
        If files Is Nothing Then
            Return False
        End If

        For Each file In files
            If String.Equals(Path.GetExtension(file), ".pln", StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Sub LoadFlightPlan(filePath As String)
        Try
            _logger.SetFlightPlanFromFile(filePath)
            ui_task.Text = Path.GetFileName(filePath)
            ui_message_bar.Text = $"Loaded {Path.GetFileName(filePath)}"
        Catch ex As Exception
            MessageBox.Show(Me, $"Failed to load flight plan: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function CreateStatusImage(color As Color) As Image
        Dim bmp As New Bitmap(48, 48)

        Using g = Graphics.FromImage(bmp)
            g.Clear(Color.Transparent)
            Using brush As New SolidBrush(color)
                g.FillEllipse(brush, 4, 4, 40, 40)
            End Using
        End Using

        Return bmp
    End Function

End Class
