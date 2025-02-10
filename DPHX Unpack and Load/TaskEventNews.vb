Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.IO

Partial Class TaskEventNews
    Inherits System.Windows.Forms.UserControl

    Private isMouseDown As Boolean = False
    Private isMouseHover As Boolean = False
    Private lightBackColor As Color
    Private darkBackColor As Color
    Private clickBackColor As Color
    Private hoverBorderColor As Color
    Private trueNews As Boolean = False
    Private taskImage As Image
    Private eventImage As Image
    Private newsImage As Image

    Public Enum NewsTypeEnum
        Task = 1
        [Event] = 2
        News = 3
    End Enum

    ' Define properties
    Public Property Key As String = String.Empty
    Public Property NewsDate As DateTime = Now.ToUniversalTime
    Public Property Title As String = "Title"
    Public Property Subtitle As String = "Subtitle"
    Public Property Comments As String = "Comments"
    Public Property Credits As String = "Credits"
    Public Property EventDate As DateTime = Now.ToUniversalTime
    Public Property News As String = "News"
    Public Property NewsType As NewsTypeEnum
    Public Property TaskEntrySeqID As Integer = 0
    Public Property URLToGo As String = String.Empty
    Public Event NewsClicked As EventHandler

    ' Define constants for fonts and colors
    Private ReadOnly TitleFont As New Font("Arial", 13, FontStyle.Bold)
    Private ReadOnly SubtitleFont As New Font("Arial", 10, FontStyle.Bold)
    Private ReadOnly CommentsFont As New Font("Arial", 9, FontStyle.Regular)
    Private ReadOnly PublishedDateFont As New Font("Arial", 8, FontStyle.Regular)
    Private ReadOnly FifthElementFont As New Font("Arial", 9, FontStyle.Regular)
    Private ReadOnly TextBrush As New SolidBrush(Color.Black)

    Public Sub New()

        InitializeComponent()

        'Create a fake news entry
        Dim fakeNews As New NewsEntry
        fakeNews.NewsType = NewsTypeEnum.News
        fakeNews.Key = $"N-{Guid.NewGuid.ToString}"
        fakeNews.Published = Now().ToUniversalTime
        fakeNews.Title = "Fake news entry"
        fakeNews.Subtitle = "This is a fake news entry!"
        fakeNews.News = "Wow, this will bring you to Google."
        fakeNews.URLToGo = "https://google.com"

        OnCreation(fakeNews)

    End Sub

    Public Sub New(newsEntry As NewsEntry)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        OnCreation(newsEntry)

    End Sub

    Public Sub OnCreation(newsEntry As NewsEntry)

        With newsEntry
            Select Case .NewsType
                Case 0
                    Me.NewsType = NewsTypeEnum.Task
                Case 1
                    Me.NewsType = NewsTypeEnum.Event
                Case 2
                    Me.NewsType = NewsTypeEnum.News
            End Select
            Me.Key = .Key
            Me.NewsDate = .Published
            Me.Title = .Title
            If .Subtitle IsNot Nothing Then
                Me.Subtitle = .Subtitle.Replace("($*$)", Environment.NewLine).Replace("*", "")
            End If
            If .News IsNot Nothing Then
                Me.News = .News.Replace("($*$)", Environment.NewLine).Replace("*", "")
            End If
            Me.URLToGo = .URLToGo
            If .Comments IsNot Nothing Then
                Me.Comments = .Comments.Replace("($*$)", Environment.NewLine).Replace("*", "")
            End If
            Me.Credits = .Credits
            If .EntrySeqID IsNot Nothing Then
                Me.TaskEntrySeqID = .EntrySeqID
            End If
            If .EventDate IsNot Nothing Then
                Me.EventDate = .EventDate
            End If
        End With

        ' Load images
        Dim folderForImages As String = Path.GetDirectoryName(Application.ExecutablePath)
        taskImage = Image.FromFile(Path.Combine(folderForImages, "Glider.png"))
        eventImage = Image.FromFile(Path.Combine(folderForImages, "Calendar.png"))
        newsImage = Image.FromFile(Path.Combine(folderForImages, "LoudSpeaker.png"))

        trueNews = True
        Me.Height = CalculateHeight()

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
                lightBackColor = Color.FromArgb(220, 238, 251) ' Light Blue
                darkBackColor = Color.FromArgb(176, 211, 242) ' Darker Blue
                clickBackColor = Color.FromArgb(176, 211, 242) ' Even Darker Blue
                hoverBorderColor = Color.DarkBlue
            Case NewsTypeEnum.Event
                lightBackColor = Color.FromArgb(223, 255, 228) ' Light Green
                darkBackColor = Color.FromArgb(192, 245, 192) ' Darker Green
                clickBackColor = Color.FromArgb(192, 245, 192) ' Even Darker Green
                hoverBorderColor = Color.DarkGreen
            Case NewsTypeEnum.News
                lightBackColor = Color.FromArgb(255, 249, 219) ' Light Yellow
                darkBackColor = Color.FromArgb(255, 235, 140) ' Darker Yellow
                clickBackColor = Color.FromArgb(255, 235, 140) ' Even Darker Yellow
                hoverBorderColor = Color.Orange
        End Select
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If trueNews Then
            Me.Height = CalculateHeight()
        End If
        Me.Invalidate() ' Redraw the control to update the text layout
    End Sub

    Private Function CalculateHeight() As Integer
        ' Check if in design mode
        If Not trueNews Then
            Return Me.Height
        End If

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat
        Dim longDatePatternWithoutYear As String = dateTimeFormat.LongDatePattern.Replace("yyyy", "").Replace("yy", "").Trim(" ", ",")

        ' Calculate the height based on the text parts
        Dim g As Graphics = Me.CreateGraphics()
        Dim contentHeight As Integer = 0
        contentHeight += CInt(g.MeasureString(NewsDate.ToLocalTime().ToString(dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), PublishedDateFont, Me.Width - 10).Height) + 5
        contentHeight += CInt(g.MeasureString(Title, TitleFont, Me.Width - 10).Height) + 5
        contentHeight += CInt(g.MeasureString(Subtitle, SubtitleFont, Me.Width - 10).Height) + 5
        contentHeight += CInt(g.MeasureString(Comments, CommentsFont, Me.Width - 10).Height) + 5

        Select Case NewsType
            Case NewsTypeEnum.Task
                contentHeight += CInt(g.MeasureString(Credits, FifthElementFont, Me.Width - 10).Height) + 5

            Case NewsTypeEnum.Event
                ' Include day of the week in the EventDate without the year
                contentHeight += CInt(g.MeasureString(EventDate.ToLocalTime().ToString("dddd, " & longDatePatternWithoutYear & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), FifthElementFont, Me.Width - 10).Height) + 5

            Case NewsTypeEnum.News
                contentHeight += CInt(g.MeasureString(News, FifthElementFont, Me.Width - 10).Height) + 5
        End Select

        g.Dispose()
        Return contentHeight + 10 ' Add some padding
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Draw gradient background
        Dim brush As LinearGradientBrush
        If isMouseHover Then
            brush = New LinearGradientBrush(ClientRectangle, darkBackColor, lightBackColor, LinearGradientMode.Vertical)
        Else
            brush = New LinearGradientBrush(ClientRectangle, lightBackColor, darkBackColor, LinearGradientMode.Vertical)
        End If
        e.Graphics.FillRectangle(brush, ClientRectangle)
        brush.Dispose()

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

        If NewsType = NewsTypeEnum.Event Then
            ' Include day of the week in the EventDate without the year
            Dim longDatePatternWithoutYear As String = dateTimeFormat.LongDatePattern.Replace("yyyy", "").Replace("yy", "").Trim(" ", ",")
            Dim eventDateFormatted = EventDate.ToLocalTime().ToString("dddd, " & longDatePatternWithoutYear & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture)
            textSize = e.Graphics.MeasureString(eventDateFormatted, FifthElementFont, Me.Width - 10)
            e.Graphics.DrawString(eventDateFormatted, FifthElementFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
            y += CInt(textSize.Height) + 5
        End If

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
                y += CInt(textSize.Height) + 5
            Case NewsTypeEnum.News
                textSize = e.Graphics.MeasureString(News, FifthElementFont, Me.Width - 10)
                e.Graphics.DrawString(News, FifthElementFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))
                y += CInt(textSize.Height) + 5
        End Select

        textSize = e.Graphics.MeasureString(NewsDate.ToLocalTime().ToString(dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), PublishedDateFont, Me.Width - 10)
        e.Graphics.DrawString(NewsDate.ToLocalTime().ToString(dateTimeFormat.LongDatePattern & " " & dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture), PublishedDateFont, New SolidBrush(Me.ForeColor), New RectangleF(5, y, Me.Width - 10, textSize.Height))

        ' Draw the image in the bottom right corner
        Dim cornerImage As Image = Nothing
        Select Case NewsType
            Case NewsTypeEnum.Task
                cornerImage = taskImage
            Case NewsTypeEnum.Event
                cornerImage = eventImage
            Case NewsTypeEnum.News
                cornerImage = newsImage
        End Select

        If cornerImage IsNot Nothing Then
            Dim imageX As Integer = Me.Width - cornerImage.Width - 5 ' 5 pixels padding from the right edge
            Dim imageY As Integer = Me.Height - cornerImage.Height - 5 ' 5 pixels padding from the bottom edge
            e.Graphics.DrawImage(cornerImage, imageX, imageY, cornerImage.Width, cornerImage.Height)
        End If

        CalculateHeight()
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

        'Dispose of other stuff
        taskImage.Dispose()
        taskImage = Nothing
        eventImage.Dispose()
        eventImage = Nothing
        newsImage.Dispose()
        newsImage = Nothing

        TitleFont.Dispose()
        SubtitleFont.Dispose()
        CommentsFont.Dispose()
        PublishedDateFont.Dispose()
        FifthElementFont.Dispose()
        TextBrush.Dispose()

    End Sub

    Private Sub TaskEventNews_MouseEnter(sender As Object, e As EventArgs)
        isMouseHover = True
        Me.Invalidate() ' Redraw to update border and background
    End Sub

    Private Sub TaskEventNews_MouseLeave(sender As Object, e As EventArgs)
        isMouseHover = False
        Me.Invalidate() ' Redraw to update border and background
    End Sub

    Private Sub TaskEventNews_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        isMouseDown = True
        Me.Invalidate() ' Redraw to update border and background
    End Sub

    Private Sub TaskEventNews_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        isMouseDown = False
        Me.Invalidate() ' Redraw to update border and background
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            MyBase.OnMouseClick(e)
            RaiseEvent NewsClicked(Me, EventArgs.Empty)
        End If
    End Sub
End Class
