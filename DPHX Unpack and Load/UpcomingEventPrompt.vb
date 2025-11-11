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

Public Class UpcomingEventPrompt
    Inherits ZoomForm

    Private ReadOnly _info As UpcomingEventInfo

    Public Sub New(info As UpcomingEventInfo)
        If info Is Nothing Then Throw New ArgumentNullException(NameOf(info))
        _info = info
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Rescale()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Upcoming Event"
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.ClientSize = New Size(480, 360)

        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 9,
            .Padding = New Padding(12)
        }

        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layout.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        Dim maxTextWidth As Integer = Me.ClientSize.Width - layout.Padding.Horizontal

        Dim lblTitleCaption As New Label With {
            .Text = "Title",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .UseMnemonic = False
        }

        Dim lblTitleValue As New Label With {
            .AutoSize = True,
            .Font = New Font("Segoe UI", 11.0F, FontStyle.Bold),
            .Text = _info.Title,
            .MaximumSize = New Size(maxTextWidth, 0),
            .UseMnemonic = False
        }

        Dim lblSubtitleCaption As New Label With {
            .Text = "Subtitle",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .UseMnemonic = False
        }

        Dim lblSubtitleValue As New Label With {
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.5F, FontStyle.Regular),
            .Text = _info.Subtitle,
            .MaximumSize = New Size(maxTextWidth, 0),
            .UseMnemonic = False
        }

        Dim lblCommentsCaption As New Label With {
            .Text = "Comments",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .UseMnemonic = False
        }

        Dim txtComments As New TextBox With {
            .Multiline = True,
            .ReadOnly = True,
            .Dock = DockStyle.Fill,
            .ScrollBars = ScrollBars.Vertical,
            .Text = _info.Comments,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Regular),
            .BackColor = SystemColors.Window,
            .BorderStyle = BorderStyle.FixedSingle,
            .TabStop = False
        }
        txtComments.Margin = New Padding(0, 2, 0, 0)

        Dim lblDateCaption As New Label With {
            .Text = "Event time",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .UseMnemonic = False
        }

        Dim lblDateValue As New Label With {
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.5F, FontStyle.Regular),
            .Text = _info.EventDateLocal.ToString("f"),
            .UseMnemonic = False
        }

        Dim btnJoin As New Button With {
            .Text = "Join",
            .DialogResult = DialogResult.OK,
            .AutoSize = True
        }

        Dim btnCancel As New Button With {
            .Text = "Cancel",
            .DialogResult = DialogResult.Cancel,
            .AutoSize = True
        }

        Dim buttonsPanel As New FlowLayoutPanel With {
            .FlowDirection = FlowDirection.RightToLeft,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Anchor = AnchorStyles.Right
        }

        buttonsPanel.Controls.Add(btnCancel)
        buttonsPanel.Controls.Add(btnJoin)

        layout.Controls.Add(lblTitleCaption, 0, 0)
        layout.Controls.Add(lblTitleValue, 0, 1)
        layout.Controls.Add(lblSubtitleCaption, 0, 2)
        layout.Controls.Add(lblSubtitleValue, 0, 3)
        layout.Controls.Add(lblCommentsCaption, 0, 4)
        layout.Controls.Add(txtComments, 0, 5)
        layout.Controls.Add(lblDateCaption, 0, 6)
        layout.Controls.Add(lblDateValue, 0, 7)
        layout.Controls.Add(buttonsPanel, 0, 8)

        lblDateCaption.Margin = New Padding(0, 8, 0, 0)
        lblDateValue.Margin = New Padding(0, 2, 0, 0)
        buttonsPanel.Margin = New Padding(0, 10, 0, 0)

        Me.AcceptButton = btnJoin
        Me.CancelButton = btnCancel

        Me.Controls.Add(layout)
    End Sub
End Class
