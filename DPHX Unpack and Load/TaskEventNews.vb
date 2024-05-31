Imports System.Globalization

Partial Class TaskEventNews
    Inherits System.Windows.Forms.UserControl

    Private isMouseDown As Boolean = False
    Private isMouseHover As Boolean = False
    Private theBackColor As Color
    Private hoverBackColor As Color
    Private clickBackColor As Color
    Private hoverBorderColor As Color

    Public Enum NewsTypeEnum
        Task = 1
        [Event] = 2
        News = 3
    End Enum

    ' Define properties
    Public Property NewsDate As DateTime = Now.ToUniversalTime
    Public Property Title As String = "Title"
    Public Property Subtitle As String = "Subtitle"
    Public Property Comments As String = "Comments"
    Public Property Credits As String = "Credits"
    Public Property EventDate As DateTime = Now.ToUniversalTime
    Public Property News As String = "News"
    Public Property NewsType As NewsTypeEnum = NewsTypeEnum.News
    Public Property TaskEntrySeqID As Integer = 1
    Public Property URLToGo As String = String.Empty
    Public Event NewsClicked As EventHandler

    ' Define constants for fonts and colors
    Private ReadOnly TitleFont As New Font("Arial", 13, FontStyle.Bold)
    Private ReadOnly SubtitleFont As New Font("Arial", 10, FontStyle.Bold)
    Private ReadOnly CommentsFont As New Font("Arial", 9, FontStyle.Regular)
    Private ReadOnly DateFont As New Font("Arial", 9, FontStyle.Regular)
    Private ReadOnly FifthElementFont As New Font("Arial", 9, FontStyle.Regular)
    Private ReadOnly TextBrush As New SolidBrush(Color.Black)

    Public Sub New()
        InitializeComponent()

        ' Set DoubleBuffered to true to reduce flicker
        Me.DoubleBuffered = True

        ' Add mouse event handlers for all child controls
        AddMouseEventHandlers(Me)
        ' Set background colors based on NewsType
        SetBackgroundColor()
    End Sub

    Private Sub SetBackgroundColor()
        Select Case NewsType
            Case NewsTypeEnum.Task
                theBackColor = Color.FromArgb(220, 238, 251) ' Light Blue
                hoverBackColor = Color.FromArgb(192, 221, 247) ' Slightly Darker Blue
                clickBackColor = Color.FromArgb(176, 211, 242) ' Even Darker Blue
                hoverBorderColor = Color.DarkBlue
            Case NewsTypeEnum.Event
                theBackColor = Color.FromArgb(223, 255, 228) ' Light Green
                hoverBackColor = Color.FromArgb(207, 255, 208) ' Slightly Darker Green
                clickBackColor = Color.FromArgb(192, 245, 192) ' Even Darker Green
                hoverBorderColor = Color.DarkGreen
            Case NewsTypeEnum.News
                theBackColor = Color.FromArgb(255, 249, 219) ' Light Yellow
                hoverBackColor = Color.FromArgb(255, 243, 176) ' Slightly Darker Yellow
                clickBackColor = Color.FromArgb(255, 235, 140) ' Even Darker Yellow
                hoverBorderColor = Color.Orange
        End Select
        Me.BackColor = theBackColor
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If Not Me.DesignMode Then
            Me.Height = CalculateHeight()
        End If
        Me.Invalidate() ' Redraw the control to update the text layout
    End Sub

    Private Function CalculateHeight() As Integer
        ' Check if in design mode
        If Me.DesignMode Then
            Return Me.Height
        End If

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat

        ' Calculate the height based on the text parts
        Dim g As Graphics = Me.CreateGraphics()
        Dim contentHeight As Integer = 0
        contentHeight += CInt(g.MeasureString(NewsDate.ToLocalTime().ToString(dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), DateFont, Me.Width - 10).Height) + 5
        contentHeight += CInt(g.MeasureString(Title, TitleFont, Me.Width - 10).Height) + 5
        contentHeight += CInt(g.MeasureString(Subtitle, SubtitleFont, Me.Width - 10).Height) + 5
        contentHeight += CInt(g.MeasureString(Comments, CommentsFont, Me.Width - 10).Height) + 5

        Select Case NewsType
            Case NewsTypeEnum.Task
                contentHeight += CInt(g.MeasureString(Credits, FifthElementFont, Me.Width - 10).Height) + 5
            Case NewsTypeEnum.Event
                ' Include day of the week in the EventDate
                contentHeight += CInt(g.MeasureString(EventDate.ToLocalTime().ToString("dddd, " & dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), FifthElementFont, Me.Width - 10).Height) + 5
            Case NewsTypeEnum.News
                contentHeight += CInt(g.MeasureString(News, FifthElementFont, Me.Width - 10).Height) + 5
        End Select

        g.Dispose()
        Return contentHeight + 10 ' Add some padding
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Draw 3D border based on mouse state
        If isMouseDown Then
            ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken)
        ElseIf isMouseHover Then
            ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Raised)
            ' Overlay the custom color border
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, hoverBorderColor, ButtonBorderStyle.Solid)
        Else
            ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Raised)
        End If

        ' Draw text elements
        Dim y As Integer = 5
        Dim textSize As SizeF
        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat

        textSize = e.Graphics.MeasureString(NewsDate.ToLocalTime().ToString(dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), DateFont, Me.Width - 10)
        e.Graphics.DrawString(NewsDate.ToLocalTime().ToString(dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), DateFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
        y += CInt(textSize.Height) + 5

        textSize = e.Graphics.MeasureString(Title, TitleFont, Me.Width - 10)
        e.Graphics.DrawString(Title, TitleFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
        y += CInt(textSize.Height) + 5

        textSize = e.Graphics.MeasureString(Subtitle, SubtitleFont, Me.Width - 10)
        e.Graphics.DrawString(Subtitle, SubtitleFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
        y += CInt(textSize.Height) + 5

        textSize = e.Graphics.MeasureString(Comments, CommentsFont, Me.Width - 10)
        e.Graphics.DrawString(Comments, CommentsFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
        y += CInt(textSize.Height) + 5

        Select Case NewsType
            Case NewsTypeEnum.Task
                textSize = e.Graphics.MeasureString(Credits, FifthElementFont, Me.Width - 10)
                e.Graphics.DrawString(Credits, FifthElementFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
            Case NewsTypeEnum.Event
                ' Include day of the week in the EventDate
                Dim eventDateFormatted = EventDate.ToLocalTime().ToString("dddd, " & dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture)
                textSize = e.Graphics.MeasureString(eventDateFormatted, FifthElementFont, Me.Width - 10)
                e.Graphics.DrawString(eventDateFormatted, FifthElementFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
            Case NewsTypeEnum.News
                textSize = e.Graphics.MeasureString(News, FifthElementFont, Me.Width - 10)
                e.Graphics.DrawString(News, FifthElementFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
        End Select
    End Sub

    Private Sub AddMouseEventHandlers(ctrl As Control)
        AddHandler ctrl.MouseEnter, AddressOf TaskEventNews_MouseEnter
        AddHandler ctrl.MouseLeave, AddressOf TaskEventNews_MouseLeave
        AddHandler ctrl.MouseDown, AddressOf TaskEventNews_MouseDown
        AddHandler ctrl.MouseUp, AddressOf TaskEventNews_MouseUp

        ' Recursively add handlers for all child controls
        For Each child As Control In ctrl.Controls
            AddMouseEventHandlers(child)
        Next
    End Sub

    Private Sub RemoveMouseEventHandlers(ctrl As Control)
        RemoveHandler ctrl.MouseEnter, AddressOf TaskEventNews_MouseEnter
        RemoveHandler ctrl.MouseLeave, AddressOf TaskEventNews_MouseLeave
        RemoveHandler ctrl.MouseDown, AddressOf TaskEventNews_MouseDown
        RemoveHandler ctrl.MouseUp, AddressOf TaskEventNews_MouseUp

        ' Recursively remove handlers for all child controls
        For Each child As Control In ctrl.Controls
            RemoveMouseEventHandlers(child)
        Next
    End Sub

    Private Sub TaskEventNews_MouseEnter(sender As Object, e As EventArgs)
        isMouseHover = True
        Me.BackColor = hoverBackColor
        Me.Invalidate() ' Redraw to update border
    End Sub

    Private Sub TaskEventNews_MouseLeave(sender As Object, e As EventArgs)
        isMouseHover = False
        If Not isMouseDown Then
            Me.BackColor = theBackColor
        End If
        Me.Invalidate() ' Redraw to update border
    End Sub

    Private Sub TaskEventNews_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        isMouseDown = True
        Me.BackColor = clickBackColor
        Me.Invalidate() ' Redraw to update border
    End Sub

    Private Sub TaskEventNews_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        isMouseDown = False
        Me.BackColor = If(isMouseHover, hoverBackColor, theBackColor)
        Me.Invalidate() ' Redraw to update border
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        MyBase.OnMouseClick(e)
        RaiseEvent NewsClicked(Me, EventArgs.Empty)
    End Sub
End Class
