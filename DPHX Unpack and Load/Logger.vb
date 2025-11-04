Imports NB21_logger

Public Class Logger

    Private _pilotName As String
    Private _competitionID As String
    Private _appName As String
    Private _appVersion As String
    Private _config As LoggerConfiguration
    Private _logger As NB21Logger
    Private _compactView As Boolean = False

    Public Sub New(pilotName As String, competitionID As String, tracklogsPath As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _config = New LoggerConfiguration()
        _config.AppName = "NB21 Logger for DPHX"
        _config.AppVersion = "1.2.3"
        _config.PilotName = pilotName
        _config.PilotId = competitionID
        _config.TracklogsDirectory = tracklogsPath

        _logger = New NB21Logger(_config)

    End Sub

    Private Sub Logger_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        SetSize()

        Me.Text = $"{_config.AppName} {_config.AppVersion}"
        ui_pilot.Text = $"{_config.PilotName} ({_config.PilotId})"

        _logger.Start()

    End Sub

    Private Sub Logger_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        _logger.Stop()
        _logger.Dispose()
        _logger = Nothing
    End Sub

    Private Sub SetSize()
        If _compactView Then
            Me.Size = New Size(680, 124)
            ui_min_max.Text = "Full"
        Else
            Me.Size = New Size(680, 320)
            ui_min_max.Text = "Compact"
        End If
    End Sub

    Private Sub ui_min_max_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles ui_min_max.LinkClicked
        _compactView = Not _compactView
        SetSize()
    End Sub

End Class