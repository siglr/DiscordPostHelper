Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class WindCloudDisplay
    Inherits Control

    Private _WeatherInfo As WeatherDetails = Nothing

    Public Sub SetWeatherInfo(thisWeatherInfo As WeatherDetails)
        _WeatherInfo = thisWeatherInfo
        Invalidate()
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

        ' Add wind and cloud layers
        If _WeatherInfo IsNot Nothing Then
            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim windRects As New List(Of Rectangle)
            Dim windInfos As New List(Of String)

            ' 1. Define all rectangles
            For Each wind In _WeatherInfo.WindLayers
                Dim altitudeInFeet As Single = Math.Round(wind.Altitude * meterToFeet)
                Dim windYPosition As Single = (maxAltitude - altitudeInFeet / 1000) * verticalSpacing + textHeight
                Dim windRect As New Rectangle(10, CInt(windYPosition - textHeight / 2), CInt(Width / 2) - 20, CInt(textHeight))
                windRects.Add(windRect)

                Dim windInfo As String = $"{altitudeInFeet}’ {wind.Angle}°@{wind.Speed}kts"
                windInfos.Add(windInfo)
            Next

            ' 2. Detect overlaps and adjust sizes and positions
            For i = 0 To windRects.Count - 1
                For j = i + 1 To windRects.Count - 1
                    If Math.Abs(windRects(i).Top - windRects(j).Top) < textHeight Then
                        ' Adjust both overlapping rectangles
                        windRects(i) = New Rectangle(windRects(i).Left, windRects(i).Top, windRects(i).Width / 2, windRects(i).Height)
                        windRects(j) = New Rectangle(windRects(j).Left + windRects(j).Width / 2, windRects(j).Top, windRects(j).Width / 2, windRects(j).Height)
                    End If
                Next
            Next

            ' 3. Draw the rectangles
            For i = 0 To windRects.Count - 1
                e.Graphics.FillRectangle(Brushes.Blue, windRects(i))
                Dim windSize As SizeF = e.Graphics.MeasureString(windInfos(i), Font)
                Dim windLocation As New Point(windRects(i).Left + (windRects(i).Width - windSize.Width) / 2, windRects(i).Top + (windRects(i).Height - windSize.Height) / 2)
                e.Graphics.DrawString(windInfos(i), Font, Brushes.White, windLocation)
            Next

            ' Draw cloud layers on the right side
            For Each cloud In _WeatherInfo.CloudLayers
                If cloud.IsValidCloudLayer Then
                    Dim topInFeet As Single = Math.Round(cloud.AltitudeTop * meterToFeet)
                    Dim bottomInFeet As Single = Math.Round(cloud.AltitudeBottom * meterToFeet)

                    Dim cloudTopY As Single = (maxAltitude - topInFeet / 1000) * verticalSpacing + textHeight
                    Dim cloudBottomY As Single = (maxAltitude - bottomInFeet / 1000) * verticalSpacing + textHeight

                    Dim cloudRect As New Rectangle(CInt(Width / 2) + 10, CInt(cloudTopY), CInt(Width / 2) - 20, CInt(cloudBottomY - cloudTopY))
                    e.Graphics.FillRectangle(Brushes.Gray, cloudRect) ' Change from Blue to Gray for better differentiation
                    Dim cloudInfo As String = $"{bottomInFeet}’ to {topInFeet}’ Cov. {cloud.Coverage}% Dens. {cloud.Density} Scat. {cloud.Scattering}%"
                    Dim cloudSize As SizeF = e.Graphics.MeasureString(cloudInfo, Font)
                    Dim cloudLocation As New Point(cloudRect.Left + (cloudRect.Width - cloudSize.Width) / 2, cloudRect.Top + (cloudRect.Height - cloudSize.Height) / 2)
                    e.Graphics.DrawString(cloudInfo, Font, Brushes.White, cloudLocation)
                End If
            Next
        End If

    End Sub
End Class

