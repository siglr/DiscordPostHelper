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

        ' Calculate the text height and drawable height
        Dim textHeight As Single = Font.GetHeight()
        Dim drawableHeight As Single = Height - 1.5 * textHeight

        ' List of altitudes where lines and labels should be drawn
        Dim specifiedAltitudes As New List(Of Integer)({-2, 0, 10, 20, 30, 40, 50, 60})

        ' Call the new method to draw the grid lines and labels
        Dim altitudePositions As Dictionary(Of Integer, Single) = DrawGridLinesAndLabels(e)

        ' Call the new method to draw wind layers
        DrawWindLayers(e, altitudePositions)

        DrawCloudLayers(e, altitudePositions)

        ' Draw the vertical line in the center
        e.Graphics.DrawLine(New Pen(Color.Black, 1), CInt(Width / 2), 0, CInt(Width / 2), Height)

    End Sub

    Private Function DrawGridLinesAndLabels(ByVal e As PaintEventArgs) As Dictionary(Of Integer, Single)

        ' Dictionary to store altitude vs. vertical position
        Dim altitudePositions As New Dictionary(Of Integer, Single)

        ' Calculate the true available space
        Dim textHeight As Single = Font.GetHeight(e.Graphics)
        Dim drawableHeight As Single = Height - 2 * textHeight
        Dim drawableWidth As Single = e.ClipRectangle.Width

        ' Position for 10k line at the vertical middle
        Dim yPos10k As Single = textHeight + drawableHeight / 2
        altitudePositions.Add(10000, yPos10k)

        ' Draw the 10k line in the middle
        e.Graphics.DrawLine(New Pen(Color.Black, 1), 0, yPos10k, drawableWidth, yPos10k)
        e.Graphics.DrawString("10k", New Font("Arial", 10), Brushes.Black, 0, yPos10k)

        ' Calculate the decremental step from 10k down to -2k
        Dim decrementStep As Single = (yPos10k - textHeight) / 12 ' We have 12 increments from 10k to -2k 

        ' Draw lines only at 0 and -2k
        Dim yPos0 As Single = yPos10k + 10 * decrementStep
        altitudePositions.Add(0, yPos0)
        e.Graphics.DrawLine(New Pen(Color.Black, 1), 0, yPos0, drawableWidth, yPos0)
        e.Graphics.DrawString("0k", New Font("Arial", 10), Brushes.Black, 0, yPos0)

        Dim yPosNeg2k As Single = yPos10k + 12 * decrementStep
        altitudePositions.Add(-2000, yPosNeg2k)
        e.Graphics.DrawLine(New Pen(Color.Black, 1), 0, yPosNeg2k, drawableWidth, yPosNeg2k)
        e.Graphics.DrawString("-2k", New Font("Arial", 10), Brushes.Black, 0, yPosNeg2k)

        ' Calculate the incremental step from 10k up to 60k
        Dim incrementStep As Single = (yPos10k - textHeight) / 5 ' We have 5 increments from 10k to 60k 

        ' Draw lines for 20k, 30k, 40k, 50k, and 60k
        For i As Integer = 1 To 5
            Dim yPos As Single = yPos10k - i * incrementStep
            altitudePositions.Add((10 + i * 10) * 1000, yPos)
            e.Graphics.DrawLine(New Pen(Color.Black, 1), 0, yPos, drawableWidth, yPos)
            e.Graphics.DrawString((10 + i * 10).ToString() + "k", New Font("Arial", 10), Brushes.Black, 0, yPos)
        Next

        Return altitudePositions

    End Function

    Private Sub DrawWindLayers(ByVal e As PaintEventArgs, ByVal altitudePositions As Dictionary(Of Integer, Single))

        If _WeatherInfo IsNot Nothing Then

            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim windRects As New List(Of Rectangle)
            Dim windInfos As New List(Of String)

            ' 1. Define all rectangles
            For Each wind In _WeatherInfo.WindLayers
                Dim altitudeInFeet As Integer = CInt(Math.Round(wind.Altitude * meterToFeet))

                ' Check if the altitude exists in the dictionary
                Dim windYPosition As Single = InterpolateAltitudePosition(altitudeInFeet, altitudePositions)
                Dim windRect As New Rectangle(50, CInt(windYPosition - Font.GetHeight(e.Graphics) / 2), CInt(Width / 2) - 70, CInt(Font.GetHeight(e.Graphics)))
                windRects.Add(windRect)

                Dim windInfo As String = $"{altitudeInFeet}’ {wind.Angle}°@{wind.Speed}kts"
                windInfos.Add(windInfo)
            Next

            ' 2. Detect overlaps and adjust sizes and positions
            For i = 0 To windRects.Count - 1
                Dim overlapCount As Integer = GetOverlappingCount(windRects(i), windRects)
                If overlapCount > 1 Then
                    Dim newWidth As Integer = windRects(i).Width / overlapCount
                    windRects(i) = New Rectangle(windRects(i).Left, windRects(i).Top, newWidth, windRects(i).Height)

                    ' Adjust subsequent overlapping layers
                    Dim offset As Integer = 1
                    For j = i + 1 To windRects.Count - 1
                        If windRects(i).IntersectsWith(windRects(j)) Then
                            windRects(j) = New Rectangle(windRects(i).Left + newWidth * offset, windRects(j).Top, newWidth, windRects(j).Height)
                            offset += 1
                        End If
                    Next
                End If
            Next

            ' 3. Draw the rectangles
            For i = 0 To windRects.Count - 1
                e.Graphics.FillRectangle(Brushes.Blue, windRects(i))
                Dim windSize As SizeF = e.Graphics.MeasureString(windInfos(i), Font)
                Dim windLocation As New Point(windRects(i).Left + (windRects(i).Width - windSize.Width) / 2, windRects(i).Top + (windRects(i).Height - windSize.Height) / 2)
                e.Graphics.DrawString(windInfos(i), Font, Brushes.White, windLocation)
            Next

        End If

    End Sub

    Private Sub DrawCloudLayers(ByVal e As PaintEventArgs, ByVal altitudePositions As Dictionary(Of Integer, Single))

        If _WeatherInfo IsNot Nothing Then

            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim cloudRects As New List(Of Rectangle)
            Dim cloudInfos As New List(Of Tuple(Of String, String))

            ' 1. Define all rectangles
            For Each cloud In _WeatherInfo.CloudLayers
                If cloud.IsValidCloudLayer Then
                    Dim topInFeet As Single = Math.Round(cloud.AltitudeTop * meterToFeet)
                    Dim bottomInFeet As Single = Math.Round(cloud.AltitudeBottom * meterToFeet)

                    Dim cloudTopY As Single = If(altitudePositions.ContainsKey(topInFeet), altitudePositions(topInFeet), InterpolateAltitudePosition(topInFeet, altitudePositions))
                    Dim cloudBottomY As Single = If(altitudePositions.ContainsKey(bottomInFeet), altitudePositions(bottomInFeet), InterpolateAltitudePosition(bottomInFeet, altitudePositions))

                    Dim cloudHeight As Single = Math.Max(cloudBottomY - cloudTopY, Font.GetHeight(e.Graphics) + 2) ' Ensure minimum height based on text size + padding
                    Dim cloudRect As New Rectangle(CInt(Width / 2) + 10, CInt(cloudTopY), CInt(Width / 2) - 20, CInt(cloudHeight))
                    cloudRects.Add(cloudRect)

                    Dim line1 As String = $"{bottomInFeet}’ to {topInFeet}’"
                    Dim line2 As String = $"Cov. {cloud.Coverage}% Dens. {cloud.Density} Scat. {cloud.Scattering}%"

                    If cloudHeight < Font.GetHeight(e.Graphics) * 2 Then
                        cloudInfos.Add(Tuple.Create(line1 + " " + line2, ""))
                    Else
                        cloudInfos.Add(Tuple.Create(line1, line2))
                    End If
                End If
            Next

            For i = 0 To cloudRects.Count - 1
                Dim overlapCount As Integer = GetOverlappingCount(cloudRects(i), cloudRects)
                If overlapCount > 1 Then
                    Dim newWidth As Integer = cloudRects(i).Width / overlapCount
                    cloudRects(i) = New Rectangle(cloudRects(i).Left, cloudRects(i).Top, newWidth, cloudRects(i).Height)

                    ' Adjust subsequent overlapping layers
                    Dim offset As Integer = 1
                    For j = i + 1 To cloudRects.Count - 1
                        If cloudRects(i).IntersectsWith(cloudRects(j)) Then
                            cloudRects(j) = New Rectangle(cloudRects(i).Left + newWidth * offset, cloudRects(j).Top, newWidth, cloudRects(j).Height)
                            offset += 1
                        End If
                    Next
                End If
            Next

            ' 3. Draw the rectangles
            For i = 0 To cloudRects.Count - 1
                e.Graphics.FillRectangle(Brushes.Gray, cloudRects(i))

                Dim totalHeightForTwoLines As Single = 2 * Font.GetHeight(e.Graphics)
                Dim startYForTwoLines As Single = cloudRects(i).Top + (cloudRects(i).Height - totalHeightForTwoLines) / 2

                Dim line1Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item1, Font)
                Dim line1Location As New Point(cloudRects(i).Left + (cloudRects(i).Width - line1Size.Width) / 2, If(String.IsNullOrEmpty(cloudInfos(i).Item2), cloudRects(i).Top + (cloudRects(i).Height - line1Size.Height) / 2, startYForTwoLines))
                e.Graphics.DrawString(cloudInfos(i).Item1, Font, Brushes.White, line1Location)

                If Not String.IsNullOrEmpty(cloudInfos(i).Item2) Then
                    Dim line2Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item2, Font)
                    Dim line2Location As New Point(cloudRects(i).Left + (cloudRects(i).Width - line2Size.Width) / 2, startYForTwoLines + Font.GetHeight(e.Graphics))
                    e.Graphics.DrawString(cloudInfos(i).Item2, Font, Brushes.White, line2Location)
                End If
            Next

        End If

    End Sub

    Private Function InterpolateAltitudePosition(altitudeInFeet As Single, altitudePositions As Dictionary(Of Integer, Single)) As Single
        ' Find the nearest upper and lower reference altitudes
        Dim lowerAltitude As Integer = altitudePositions.Keys.Where(Function(alt) alt <= altitudeInFeet).Max()
        Dim upperAltitude As Integer = altitudePositions.Keys.Where(Function(alt) alt > altitudeInFeet).Min()

        Dim lowerYPosition As Single = altitudePositions(lowerAltitude)
        Dim upperYPosition As Single = altitudePositions(upperAltitude)

        ' Linear interpolation between the two reference altitudes
        Dim interpolatedYPosition As Single = lowerYPosition + (altitudeInFeet - lowerAltitude) * (upperYPosition - lowerYPosition) / (upperAltitude - lowerAltitude)

        Return interpolatedYPosition
    End Function

    ' A function to determine the number of overlapping layers for a given rectangle
    Private Function GetOverlappingCount(ByVal targetRect As Rectangle, ByVal layerRects As List(Of Rectangle)) As Integer
        Dim overlapCount As Integer = 0
        For Each rect In layerRects
            If targetRect.IntersectsWith(rect) Then
                overlapCount += 1
            End If
        Next
        Return overlapCount
    End Function

End Class

