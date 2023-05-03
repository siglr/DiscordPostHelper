Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Public Class Countdown

    Private _targetDateTime As DateTime
    Private _remainingTime As Double = 0

    Public Sub UpdateTime()
        ZoomFactor = 2
        SetRemainingTimeLabel()
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

    Public Sub SetTargetDateTime(targetDateTime As DateTime)
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
    End Sub

    Public ReadOnly Property RemainingTime As Double
        Get
            Return _remainingTime
        End Get
    End Property

End Class


