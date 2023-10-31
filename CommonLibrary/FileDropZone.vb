Imports System.Drawing
Imports System.Windows.Forms

Public Class FileDropZone
    Inherits UserControl

    Private isDraggingOver As Boolean = False
    Private originalBorderColor As Color
    Private borderPen As Pen

    Public Sub New()

        MyBase.New

        ' Initialize the UserControl
        Me.AutoSize = False
        Me.Size = New Size(200, 100) ' Set an initial size (you can change this)

        ' Create and configure the Label
        Dim label As New Label()
        label.Text = "File Drop Zone"
        label.TextAlign = ContentAlignment.MiddleCenter
        label.Dock = DockStyle.Fill
        label.AutoSize = False

        ' Add the Label to the UserControl
        Me.Controls.Add(label)

        ' Configure the UserControl for drag and drop
        Me.AllowDrop = True

        ' Set the initial border color and style
        originalBorderColor = SystemColors.ControlDark
        borderPen = New Pen(originalBorderColor, 3) ' Set initial border width

        ' Attach event handlers
        AddHandler Me.DragEnter, AddressOf FileDropZone_DragEnter
        AddHandler Me.DragLeave, AddressOf FileDropZone_DragLeave
        AddHandler Me.DragDrop, AddressOf FileDropZone_DragDrop
        AddHandler label.Click, AddressOf Label_Click
    End Sub

    Private Sub FileDropZone_DragEnter(sender As Object, e As DragEventArgs)
        ' Check if files are being dragged into the control
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            isDraggingOver = True
            originalBorderColor = borderPen.Color ' Store the original border color
            borderPen.Width = 5 ' Set a thicker border in dragging mode
            Me.Invalidate() ' Force a redraw to update the border
            Me.RefreshBackground() ' Refresh the background to change the color
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub FileDropZone_DragLeave(sender As Object, e As EventArgs)
        ' Reset the border style and color when dragging leaves
        isDraggingOver = False
        borderPen.Color = originalBorderColor ' Restore the original border color
        Me.Invalidate() ' Force a redraw to update the border
        Me.RefreshBackground() ' Refresh the background to change the color
    End Sub

    Private Sub FileDropZone_DragDrop(sender As Object, e As DragEventArgs)
        ' Handle the dropped files in the main form
        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Me.OnFilesDropped(New FilesDroppedEventArgs(files))

        ' Reset the border style and color
        isDraggingOver = False
        Me.Invalidate() ' Force a redraw to update the border
        Me.RefreshBackground() ' Refresh the background to change the color
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Set the border style to dashed if not dragging, otherwise solid
        If Not isDraggingOver Then
            borderPen.Width = 1 ' Restore the original border width
            borderPen.Color = originalBorderColor ' Restore the original border color
            ' Set the border style to dashed with a custom dash pattern
            borderPen.DashStyle = Drawing2D.DashStyle.Custom
            ' Define a custom dash pattern (e.g., longer dash, more space, longer dash, more space)
            borderPen.DashPattern = New Single() {5, 3, 5, 3} ' Adjust the values as needed
        Else
            borderPen.Color = Color.Green ' Change the border color to green
            borderPen.Width = 5 ' Restore the original border width
            borderPen.DashStyle = Drawing2D.DashStyle.Solid
        End If

        ' Draw the border with the specified color and style
        Dim rect As Rectangle = New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        e.Graphics.DrawRectangle(borderPen, rect)

    End Sub

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        MyBase.OnPaintBackground(e)

        ' Change the background color to light green if dragging over
        If isDraggingOver Then
            Using brush As New SolidBrush(Color.FromArgb(230, 255, 230)) ' Very light green
                e.Graphics.FillRectangle(brush, ClientRectangle)
            End Using
        End If
    End Sub

    ' Custom event to handle files dropped on the control
    Public Event FilesDropped As EventHandler(Of FilesDroppedEventArgs)

    Protected Overridable Sub OnFilesDropped(e As FilesDroppedEventArgs)
        RaiseEvent FilesDropped(Me, e)
    End Sub

    ' Click event for label (can be used for additional actions)
    Private Sub Label_Click(sender As Object, e As EventArgs)
        ' Perform custom actions when the label is clicked (if needed)
    End Sub

    ' Refresh the background to apply changes
    Private Sub RefreshBackground()
        Me.Invalidate()
    End Sub
End Class

Public Class FilesDroppedEventArgs
    Inherits EventArgs

    Public Property DroppedFiles As String()

    Public Sub New(files As String())
        DroppedFiles = files
    End Sub
End Class
