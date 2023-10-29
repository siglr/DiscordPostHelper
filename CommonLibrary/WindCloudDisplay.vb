Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class WindCloudDisplay
    Inherits Control

    Private _WeatherInfo As WeatherDetails = Nothing

    Public Sub SetWeatherInfo(thisWeatherInfo As WeatherDetails)
        _WeatherInfo = thisWeatherInfo
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Set the background color
        e.Graphics.Clear(Color.White)

        ' Set the line color
        Dim gridPen As New Pen(Color.Black, 1)

        ' Define the altitude range and interval
        Dim maxAltitude As Integer = 60 ' Represents 60k feet
        Dim minAltitude As Integer = -2 ' Represents -1k feet
        Dim totalAltitudeRange As Integer = maxAltitude - minAltitude + 1

        ' Calculate the number of lines, vertical spacing, and text height
        Dim textHeight As Single = Font.GetHeight()
        Dim drawableHeight As Single = Height - 1.5 * textHeight
        Dim verticalSpacing As Single = drawableHeight / totalAltitudeRange

        ' List of altitudes where lines and labels should be drawn
        Dim specifiedAltitudes As New List(Of Integer)({-2, 0, 10, 20, 30, 40, 50, 60})

        ' Draw grid lines and labels from 60k to -1k
        For i As Integer = 0 To totalAltitudeRange - 1
            Dim yPosition As Single = i * verticalSpacing + textHeight
            Dim altitudeLabel As Integer = maxAltitude - i

            ' Draw lines and labels only for the specified altitudes
            If specifiedAltitudes.Contains(altitudeLabel) Then
                e.Graphics.DrawLine(gridPen, 0, yPosition, Width, yPosition)
                ' Draw the altitude label below the line
                e.Graphics.DrawString(altitudeLabel & "k", Font, Brushes.Black, 0, yPosition)
            End If
        Next

        ' Draw the vertical line in the center
        e.Graphics.DrawLine(gridPen, CInt(Width / 2), 0, CInt(Width / 2), Height)

        gridPen.Dispose()
    End Sub
End Class

