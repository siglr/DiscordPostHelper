Imports System
Imports System.Drawing
Imports System.Windows.Forms

Partial Public Class UpcomingEventPrompt
    Inherits ZoomForm

    Private ReadOnly _info As UpcomingEventInfo

    Public Sub New(info As UpcomingEventInfo)
        If info Is Nothing Then Throw New ArgumentNullException(NameOf(info))
        _info = info
        InitializeComponent()
        ApplyInfo()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Rescale()
    End Sub

    Private Sub ApplyInfo()

        lblTitleValue.Text = _info.Title
        lblSubtitleValue.Text = _info.Subtitle
        txtComments.Text = _info.Comments
        lblDateValue.Text = _info.EventDateLocal.ToString("f")
    End Sub
End Class

Public Class UpcomingEventInfo
    Public Property Title As String = String.Empty
    Public Property Subtitle As String = String.Empty
    Public Property Comments As String = String.Empty
    Public Property EventDateUtc As DateTime
    Public Property EntrySeqID As Integer
    Public Property TaskID As String = String.Empty

    Public ReadOnly Property EventDateLocal As DateTime
        Get
            Return EventDateUtc.ToLocalTime()
        End Get
    End Property
End Class
