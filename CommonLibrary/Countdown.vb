Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports NAudio.MediaFoundation
Imports NAudio.Wave
Imports NAudio.Wave.SampleProviders

Public Class Countdown

    Private _targetDateTime As DateTime
    Private _remainingTime As Double = 0
    Private _audioCueDictionary As New Dictionary(Of Integer, String)
    Private _lastPlayedCueTime As Integer
    Private Shared _waveOut As New WaveOutEvent()
    Private Shared _audioVolume As Single

    Public Property PlayAudioCues As Boolean

    Public Sub UpdateTime()
        ZoomFactor = 2
        SetRemainingTimeLabel()

        If PlayAudioCues Then
            PlayAudioCueIfApplicable()
        End If
    End Sub

    Public Sub SetOutputVolume(volume As Integer)
        _audioVolume = volume / 100
    End Sub

    Private Sub SetRemainingTimeLabel(Optional useBlank As Boolean = False)
        Dim remainingTime As TimeSpan = _targetDateTime - DateTime.Now

        If remainingTime.TotalSeconds < 0 Then
            remainingTime = New TimeSpan(0, 0, 0, 0)
        End If

        Me.RemainingTimeLabel.ForeColor = GetColor(remainingTime)

        If Not useBlank Then
            Me.RemainingTimeLabel.Text = $"{FormatNumber(remainingTime.Days, "000")}:{FormatNumber(remainingTime.Hours, "00")}:{FormatNumber(remainingTime.Minutes, "00")}:{FormatNumber(remainingTime.Seconds, "00")}"
        Else
            Me.RemainingTimeLabel.Text = ""
        End If

        _remainingTime = remainingTime.TotalSeconds

    End Sub

    Private Function GetColor(remaining As TimeSpan) As Color

        Select Case remaining.TotalSeconds
            Case <= 0
                Return Color.Black
            Case <= 60
                Return Color.Red
            Case <= 3600
                Return Color.Orange
            Case Else
                Return Color.Black
        End Select

    End Function

    Public Sub SetTargetDateTime(targetDateTime As DateTime, Optional audioCueDictionary As Dictionary(Of Integer, String) = Nothing)
        _audioCueDictionary.Clear()
        If audioCueDictionary IsNot Nothing Then
            For Each kvp As KeyValuePair(Of Integer, String) In audioCueDictionary
                _audioCueDictionary.Add(kvp.Key, kvp.Value)
            Next
        End If
        _targetDateTime = targetDateTime
        Me.RemainingTimeLabel.ZoomFactor = Me.ZoomFactor
    End Sub

    Private Function FormatNumber(number As Integer, format As String) As String
        Return number.ToString(format)
    End Function

    Public Property ZoomFactor As Single
        Get
            Return RemainingTimeLabel.ZoomFactor
        End Get
        Set(value As Single)
            RemainingTimeLabel.ZoomFactor = value
        End Set
    End Property

    Public Sub ResetToZero(Optional useBlank As Boolean = False)
        _targetDateTime = New Date(0)
        SetRemainingTimeLabel(useBlank)
        _lastPlayedCueTime = 9999
    End Sub

    Public ReadOnly Property RemainingTime As Double
        Get
            Return _remainingTime
        End Get
    End Property

    Private Sub PlayAudioCueIfApplicable()

        For Each kvp As KeyValuePair(Of Integer, String) In _audioCueDictionary
            Dim cueTime As Integer = kvp.Key

            If cueTime < _lastPlayedCueTime AndAlso _remainingTime > 0 AndAlso
               _remainingTime <= cueTime AndAlso
               kvp.Value IsNot Nothing Then

                _lastPlayedCueTime = cueTime

                PlayAudioCue(kvp.Value, _audioVolume)

                Exit Sub ' Play only one audio cue at a time
            End If
        Next
    End Sub

    Private Sub PlayAudioCue(fileName As String, volume As Single)

        _waveOut.Stop()

        Dim wavFile As String = GetAudioFilePath(fileName)
        If File.Exists(wavFile) Then
            Dim audioFileReader As New AudioFileReader(wavFile)

            Dim volumeProvider As New VolumeSampleProvider(audioFileReader.ToSampleProvider())
            volumeProvider.Volume = volume

            _waveOut.Init(volumeProvider)
            _waveOut.Play()
        End If

    End Sub

    Private Function GetAudioFilePath(fileName As String) As String
        Dim appDirectory As String = AppDomain.CurrentDomain.BaseDirectory
        Dim audioCuesDirectory As String = Path.Combine(appDirectory, "AudioCues")
        Dim filePath As String = Path.Combine(audioCuesDirectory, $"{fileName}.wav")
        Return filePath
    End Function

End Class


