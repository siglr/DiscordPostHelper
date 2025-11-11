Imports System
Imports System.Drawing
Imports System.Windows.Forms

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

Public Partial Class UpcomingEventPrompt
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
        Dim maxTextWidth As Integer = Me.ClientSize.Width - TableLayoutPanelMain.Padding.Horizontal

        lblTitleValue.MaximumSize = New Size(maxTextWidth, 0)
        lblSubtitleValue.MaximumSize = New Size(maxTextWidth, 0)
        txtComments.MaximumSize = New Size(maxTextWidth, 0)

        lblTitleValue.Text = _info.Title
        lblSubtitleValue.Text = _info.Subtitle
        txtComments.Text = _info.Comments
        lblDateValue.Text = _info.EventDateLocal.ToString("f")
    End Sub
End Class
