Imports System.Drawing
Imports System.IO
Imports NB21_logger
Imports SIGLR.SoaringTools.CommonLibrary

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
    Private ReadOnly _initialFlightPlanPath As String
    Private _currentFlightPlanPath As String = String.Empty

    Public Sub New(pilotName As String, competitionID As String, tracklogsPath As String, flightPlanPath As String)

        ' This call is required by the designer.
        InitializeComponent()

        _config = New LoggerConfiguration()
        _config.AppName = "NB21 Logger for DPHX"
        _config.AppVersion = "1.2.3"
        _config.PilotName = If(pilotName, String.Empty).Trim()
        _config.PilotId = If(competitionID, String.Empty).Trim()
        _config.TracklogsDirectory = If(tracklogsPath, String.Empty).Trim()
        _initialFlightPlanPath = If(String.IsNullOrWhiteSpace(flightPlanPath), String.Empty, flightPlanPath)

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

        Rescale()

        SetSize()
        SetPosition()

        Text = $"{_config.AppName} {_config.AppVersion}"
        UpdatePilotDisplay()
        ui_aircraft.Text = String.Empty
        ui_task.Text = "Awaiting task from DPHX."
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

        If Not String.IsNullOrWhiteSpace(_initialFlightPlanPath) Then
            LoadFlightPlan(_initialFlightPlanPath, False)
        End If

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

        Dim locationRestored As Boolean = False

        Dim locationString As String = Settings.SessionSettings.LoggerFormLocation

        If Not String.IsNullOrWhiteSpace(locationString) Then
            Try
                Dim locationArray As String() = locationString.TrimStart("{"c).TrimEnd("}"c).Split(","c)
                Dim x As Integer = CInt(locationArray(0).Split("="c)(1))
                Dim y As Integer = CInt(locationArray(1).Split("="c)(1))

                Dim potentialLocation As New Point(x, y)
                Dim formBounds As New Rectangle(potentialLocation, Size)
                Dim isLocationVisible As Boolean = False

                For Each scr As Screen In Screen.AllScreens
                    If scr.WorkingArea.IntersectsWith(formBounds) Then
                        isLocationVisible = True
                        Exit For
                    End If
                Next

                If isLocationVisible Then
                    StartPosition = FormStartPosition.Manual
                    Location = potentialLocation
                    locationRestored = True
                End If
            Catch
                ' Ignore and fall back to centering
            End Try
        End If

        If Not locationRestored Then
            If Owner IsNot Nothing Then
                SupportingFeatures.CenterFormOnOwner(Owner, Me)
                StartPosition = FormStartPosition.Manual
            Else
                StartPosition = FormStartPosition.CenterScreen
            End If
        End If

    End Sub

    Private Sub SavePosition()

        Try
            Settings.SessionSettings.LoggerFormLocation = Location.ToString()
            Settings.SessionSettings.Save()
        Catch
        End Try

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
        view_tracklogs_button.Visible = False

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
        ui_task.Text = If(String.IsNullOrEmpty(newState.FlightPlanName), "Awaiting task from DPHX.", newState.FlightPlanName)
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

    Public Sub SetFlightPlanFromCaller(filePath As String)
        Dim pathToLoad As String = filePath
        Dim showErrors As Boolean = True

        If String.IsNullOrWhiteSpace(filePath) Then
            pathToLoad = String.Empty
            showErrors = False
        ElseIf Not File.Exists(filePath) Then
            pathToLoad = String.Empty
        End If

        If InvokeRequired Then
            BeginInvoke(New Action(Sub() LoadFlightPlan(pathToLoad, showErrors AndAlso Not String.IsNullOrWhiteSpace(pathToLoad))))
        Else
            LoadFlightPlan(pathToLoad, showErrors AndAlso Not String.IsNullOrWhiteSpace(pathToLoad))
        End If
    End Sub

    Public Sub UpdateConfigurationFromCaller(Optional pilotName As String = "",
                                             Optional competitionID As String = "",
                                             Optional tracklogsPath As String = "",
                                             Optional flightPlanPath As String = "")
        If InvokeRequired Then
            BeginInvoke(New Action(Sub() UpdateConfigurationFromCaller(pilotName, competitionID, tracklogsPath, flightPlanPath)))
            Return
        End If

        Dim pilotNameChanged As Boolean = False
        Dim pilotIdChanged As Boolean = False

        If pilotName IsNot Nothing Then
            Dim sanitizedPilotName As String = pilotName.Trim()
            If Not String.Equals(_config.PilotName, sanitizedPilotName, StringComparison.Ordinal) Then
                _config.PilotName = sanitizedPilotName
                pilotNameChanged = True
            End If
        End If

        If competitionID <> String.Empty Then
            Dim sanitizedCompetitionId As String = competitionID.Trim()
            If Not String.Equals(_config.PilotId, sanitizedCompetitionId, StringComparison.Ordinal) Then
                _config.PilotId = sanitizedCompetitionId
                pilotIdChanged = True
            End If
        End If

        If pilotNameChanged OrElse pilotIdChanged Then
            UpdatePilotDisplay()
        End If

        If tracklogsPath <> String.Empty Then
            Dim sanitizedTracklogsPath As String = tracklogsPath.Trim()
            _config.TracklogsDirectory = sanitizedTracklogsPath
            EnsureTracklogsFolder()
        End If

        If flightPlanPath <> String.Empty Then
            Dim sanitizedFlightPlanPath As String = flightPlanPath.Trim()
            SetFlightPlanFromCaller(sanitizedFlightPlanPath)
        End If
    End Sub

    Private Sub LoadFlightPlan(filePath As String, showErrors As Boolean)
        If String.IsNullOrWhiteSpace(filePath) Then
            _currentFlightPlanPath = String.Empty
            ui_task.Text = "Awaiting task from DPHX."
            ui_message_bar.Text = "Awaiting task from DPHX."
            Return
        End If

        Try
            _logger.SetFlightPlanFromFile(filePath)
            _currentFlightPlanPath = filePath
            ui_task.Text = Path.GetFileName(filePath)
            ui_message_bar.Text = $"Loaded {Path.GetFileName(filePath)}"
        Catch ex As Exception
            ui_message_bar.Text = $"Failed to load flight plan: {ex.Message}"
            If showErrors Then
                MessageBox.Show(Me, $"Failed to load flight plan: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
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

    Private Sub UpdatePilotDisplay()
        Dim name As String = If(String.IsNullOrWhiteSpace(_config.PilotName), String.Empty, _config.PilotName.Trim())
        Dim id As String = If(String.IsNullOrWhiteSpace(_config.PilotId), String.Empty, _config.PilotId.Trim())

        If name.Length = 0 AndAlso id.Length = 0 Then
            ui_pilot.Text = String.Empty
        ElseIf name.Length = 0 Then
            ui_pilot.Text = id
        ElseIf id.Length = 0 Then
            ui_pilot.Text = name
        Else
            ui_pilot.Text = $"{name} ({id})"
        End If
    End Sub

    Public ReadOnly Property IsReady As Boolean
        Get
            'TODO: Define how to determine readiness
            Return True
        End Get
    End Property

End Class
