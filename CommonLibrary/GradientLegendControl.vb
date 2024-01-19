Imports System.Drawing
Imports System.Windows.Forms

Public Class GradientLegendControl
    Inherits Control

    Private theFont As New Font("Arial", 9)

    Public Property GradientPalette As List(Of Color) = New List(Of Color)()
    Public Property FirstValue As String = String.Empty
    Public Property LastValue As String = String.Empty

    Public Sub New()
        MyBase.New
        Me.DoubleBuffered = True
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' No need to draw anything if we don't have colors
        If GradientPalette.Count = 0 Then
            Return
        End If

        ' Calculate the width for each color block
        Dim blockWidth As Integer = Me.Width \ GradientPalette.Count

        ' There might be a remainder when dividing the width by the number of colors
        ' We will distribute it between the first and last block
        Dim remainder As Integer = Me.Width Mod GradientPalette.Count
        Dim extraForFirstAndLast As Integer = remainder \ 2

        For i As Integer = 0 To GradientPalette.Count - 1
            ' Set the color for the current block
            Dim color As Color = GradientPalette(i)

            ' Determine the width for the current block
            ' Distribute the remainder to the first and last block
            Dim currentBlockWidth As Integer = blockWidth
            If i = 0 Then ' First block
                currentBlockWidth += extraForFirstAndLast
            ElseIf i = GradientPalette.Count - 1 Then ' Last block
                currentBlockWidth += extraForFirstAndLast
            End If

            ' Create a rectangle for the current block
            Dim rect As New Rectangle(i * blockWidth + If(i > 0, extraForFirstAndLast, 0), 0, currentBlockWidth, Me.Height)

            ' Fill the rectangle with the current color
            e.Graphics.FillRectangle(New SolidBrush(color), rect)

            ' Draw the white line separator
            ' Skip for the last block
            If i < GradientPalette.Count - 1 Then
                e.Graphics.DrawLine(Pens.White, (i + 1) * blockWidth + extraForFirstAndLast - 1, 0, (i + 1) * blockWidth + extraForFirstAndLast - 1, Me.Height)
            End If
        Next

        ' Now draw the first and last values
        ' Choose a contrasting color for the text based on the first and last block colors
        Dim firstColorContrast As Color = If(GradientPalette(0).GetBrightness() > 0.5F, Color.Black, Color.White)
        Dim lastColorContrast As Color = If(GradientPalette(GradientPalette.Count - 1).GetBrightness() > 0.5F, Color.Black, Color.White)

        ' Define the string format for left and right aligned texts
        Dim leftFormat As New StringFormat With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center}
        Dim rightFormat As New StringFormat With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center}

        ' Draw the first value
        If Not String.IsNullOrEmpty(FirstValue) Then
            e.Graphics.DrawString(FirstValue, theFont, New SolidBrush(firstColorContrast), New PointF(0, Me.Height / 2), leftFormat)
        End If

        ' Draw the last value
        If Not String.IsNullOrEmpty(LastValue) Then
            e.Graphics.DrawString(LastValue, theFont, New SolidBrush(lastColorContrast), New PointF(Me.Width, Me.Height / 2), rightFormat)
        End If

    End Sub
End Class
