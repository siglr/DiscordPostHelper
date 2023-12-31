Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class WindCloudDisplay
    Inherits Control

    Private theFont As New Font("Arial", 9)
    Private _WeatherInfo As WeatherDetails = Nothing
    Public ReadOnly Property BlueGradientPalette As List(Of Color)
    Public ReadOnly Property GreyGradientPalette As List(Of Color)

    Private _prefUnits As PreferredUnits

    Public Sub New()
        MyBase.New

        Dim lowestValue As Integer = 0
        Dim highestValue As Integer = 225
        Dim totalShades As Integer = 26
        Dim stepValue As Integer = (highestValue - lowestValue) \ (totalShades - 1)

        BlueGradientPalette = New List(Of Color)
        For i As Integer = 0 To totalShades - 1
            Dim blueValue As Integer = lowestValue + (stepValue * i)
            ' Ensure that the grey value does not exceed the highest value
            If blueValue > highestValue Then
                blueValue = highestValue
            End If
            BlueGradientPalette.Add(Color.FromArgb(blueValue, blueValue, 255))
        Next

        ' Define the gradient palette with 50 shades of grey, reversed
        lowestValue = 78
        highestValue = 240
        totalShades = 50
        stepValue = (highestValue - lowestValue) \ (totalShades - 1)

        GreyGradientPalette = New List(Of Color)
        For i As Integer = 0 To totalShades - 1
            Dim greyValue As Integer = highestValue - (stepValue * i)
            ' Ensure that the grey value does not fall below the lowest value
            If greyValue < lowestValue Then
                greyValue = lowestValue
            End If
            GreyGradientPalette.Add(Color.FromArgb(greyValue, greyValue, greyValue))
        Next

    End Sub

    Public Sub SetWeatherInfo(thisWeatherInfo As WeatherDetails, prefUnits As PreferredUnits)
        _WeatherInfo = thisWeatherInfo
        _prefUnits = prefUnits
        Invalidate()
    End Sub

    Public Sub ResetGraph()
        _WeatherInfo = Nothing
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Set the background color
        e.Graphics.Clear(Color.White)

        If _prefUnits Is Nothing Then
            _prefUnits = New PreferredUnits
        End If

        ' Calculate the text height and drawable height
        Dim textHeight As Single = theFont.GetHeight()
        Dim drawableHeight As Single = Height - 1.5 * textHeight

        ' List of altitudes where lines and labels should be drawn
        Dim specifiedAltitudes As New List(Of Integer)({-2, 0, 10, 20, 30, 40, 50, 60})

        ' Call the new method to draw the grid lines and labels
        Dim altitudePositions As Dictionary(Of Integer, Single) = DrawGridLinesAndLabels(e)

        ' Call the new method to draw wind layers
        DrawWindLayers(e, altitudePositions)

        DrawCloudLayers(e, altitudePositions)

    End Sub

    Private Function DrawGridLinesAndLabels(ByVal e As PaintEventArgs) As Dictionary(Of Integer, Single)

        ' Dictionary to store altitude vs. vertical position
        Dim altitudePositions As New Dictionary(Of Integer, Single)

        ' Calculate the true available space
        Dim textHeight As Single = theFont.GetHeight(e.Graphics)
        Dim drawableHeight As Single = Height - 2 * textHeight
        Dim drawableWidth As Single = e.ClipRectangle.Width
        Dim rightEdge As Single = drawableWidth

        ' Position for 10k line at the vertical middle
        Dim yPos10k As Single = textHeight + drawableHeight / 2

        altitudePositions.Add(10000, yPos10k)

        ' Draw the 10k line in the middle
        Dim middleAltitudeLabel As String = If(_prefUnits.Altitude = PreferredUnits.AltitudeUnits.Metric, "3048", "10k")
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos10k, drawableWidth, yPos10k)
        WriteAltitudeLabel(e, rightEdge, 10000, yPos10k)

        ' Calculate the decremental step from 10k down to -2k
        Dim decrementStep As Single = (yPos10k - textHeight) / 12 ' We have 12 increments from 10k to -2k 

        ' Draw lines only at 5k, 0 and -2k
        Dim yPos5k As Single = yPos10k + 5 * decrementStep
        altitudePositions.Add(5000, yPos5k)
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos5k, drawableWidth, yPos5k)
        WriteAltitudeLabel(e, rightEdge, 5000, yPos5k)

        Dim yPos0 As Single = yPos10k + 10 * decrementStep
        altitudePositions.Add(0, yPos0)
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos0, drawableWidth, yPos0)
        WriteAltitudeLabel(e, rightEdge, 0, yPos0)

        Dim yPosNeg2k As Single = yPos10k + 12 * decrementStep
        altitudePositions.Add(-2000, yPosNeg2k)
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPosNeg2k, drawableWidth, yPosNeg2k)
        WriteAltitudeLabel(e, rightEdge, -2000, yPosNeg2k)

        ' Calculate the incremental step from 10k up to 60k
        Dim incrementStep As Single = (yPos10k - textHeight) / 5 ' We have 5 increments from 10k to 60k 

        ' Convert and draw lines for 20k, 30k, 40k, 50k, and 60k
        For i As Integer = 1 To 5
            Dim altitude As Integer = (10 + i * 10) * 1000
            Dim yPos As Single = yPos10k - i * incrementStep
            altitudePositions.Add(altitude, yPos)
            e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos, drawableWidth, yPos)
            WriteAltitudeLabel(e, rightEdge, altitude, yPos)
        Next

        'Write Feet and Meters at the top of the graph
        WriteAltitudeLabel(e, rightEdge, 0, 0, True)

        'Write the legend for C D S on top
        Dim legend As String = "C:Coverage D:Density S:Scattered"
        Dim textSize As SizeF = e.Graphics.MeasureString(legend, theFont)
        Dim textXPosition As Single = CInt(Width * 0.75) - CInt(textSize.Width / 2)
        e.Graphics.DrawString(legend, theFont, Brushes.Black, textXPosition, 0)


        ' Draw the vertical line in the center
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), CInt(Width / 2), 0, CInt(Width / 2), Height)

        Return altitudePositions

    End Function

    Private Sub WriteAltitudeLabel(e As PaintEventArgs, rightEdge As Single, altitude As Integer, yPos As Single, Optional unitLabel As Boolean = False)
        Dim altitudeLabel As String
        Select Case _prefUnits.Altitude
            Case PreferredUnits.AltitudeUnits.Metric
                If Not unitLabel Then
                    altitudeLabel = (altitude * 0.3048).ToString("0")
                Else
                    altitudeLabel = "Meters"
                End If
                e.Graphics.DrawString(altitudeLabel, theFont, Brushes.Black, 0, yPos)
            Case PreferredUnits.AltitudeUnits.Imperial
                If Not unitLabel Then
                    If altitude <> 0 Then
                        altitudeLabel = (altitude / 1000).ToString() + "k"
                    Else
                        altitudeLabel = (altitude / 1000).ToString()
                    End If
                Else
                    altitudeLabel = "Feet"
                End If
                e.Graphics.DrawString(altitudeLabel, theFont, Brushes.Black, 0, yPos)
            Case PreferredUnits.AltitudeUnits.Both
                'Feet on the left
                If Not unitLabel Then
                    If altitude <> 0 Then
                        altitudeLabel = (altitude / 1000).ToString() + "k"
                    Else
                        altitudeLabel = (altitude / 1000).ToString()
                    End If
                Else
                    altitudeLabel = "Feet"
                End If
                e.Graphics.DrawString(altitudeLabel, theFont, Brushes.Black, 0, yPos)

                'Meters on the right
                If Not unitLabel Then
                    altitudeLabel = (altitude * 0.3048).ToString("0")
                Else
                    altitudeLabel = "Meters"
                End If
                Dim textSize As SizeF = e.Graphics.MeasureString(altitudeLabel, theFont)
                Dim textXPosition As Single = rightEdge - textSize.Width ' Align to the right
                e.Graphics.DrawString(altitudeLabel, theFont, Brushes.Black, textXPosition, yPos)
        End Select
    End Sub

    Private Sub DrawWindLayers(ByVal e As PaintEventArgs, ByVal altitudePositions As Dictionary(Of Integer, Single))

        If _WeatherInfo IsNot Nothing Then

            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim windRects As New List(Of Rectangle)
            Dim windInfos As New List(Of String)
            Dim windSpeeds As New List(Of Integer)

            Dim spaceForLeftLabel As Integer = 20
            Select Case _prefUnits.Altitude
                Case PreferredUnits.AltitudeUnits.Imperial, PreferredUnits.AltitudeUnits.Both
                    spaceForLeftLabel = 30
                Case PreferredUnits.AltitudeUnits.Metric
                    spaceForLeftLabel = 50
            End Select

            ' 1. Define all rectangles
            For Each wind In _WeatherInfo.WindLayers
                Dim altitudeInFeet As Integer = CInt(Math.Round(wind.Altitude * meterToFeet))

                ' Check if the altitude exists in the dictionary
                Dim windYPosition As Single = InterpolateAltitudePosition(altitudeInFeet, altitudePositions)
                Dim windRect As New Rectangle(spaceForLeftLabel, CInt(windYPosition - theFont.GetHeight(e.Graphics) / 2), CInt(Width / 2) - 40, CInt(theFont.GetHeight(e.Graphics)))
                windRects.Add(windRect)

                Dim windInfo As String
                Dim altPart As String = String.Empty
                Dim speedPart As String = String.Empty

                Select Case _prefUnits.Altitude
                    Case PreferredUnits.AltitudeUnits.Imperial
                        altPart = $"{altitudeInFeet}'"
                    Case PreferredUnits.AltitudeUnits.Metric
                        altPart = $"{Conversions.FeetToMeters(altitudeInFeet):N0} m"
                    Case PreferredUnits.AltitudeUnits.Both
                        altPart = $"{altitudeInFeet}' ({Conversions.FeetToMeters(altitudeInFeet):N0} m)"
                End Select

                Select Case _prefUnits.WindSpeed
                    Case PreferredUnits.WindSpeedUnits.Knots
                        speedPart = $"{wind.Speed} kts"
                    Case PreferredUnits.WindSpeedUnits.MeterPerSecond
                        speedPart = $"{Conversions.KnotsToMps(wind.Speed):N1} m/s"
                    Case PreferredUnits.WindSpeedUnits.Both
                        speedPart = $"{wind.Speed} kts ({Conversions.KnotsToMps(wind.Speed):N1} m/s)"
                End Select

                windInfo = $"{altPart} {wind.Angle}°@{speedPart}"
                windSpeeds.Add(wind.Speed)
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

            ' 3. Draw the rectangles and set their color
            For i = 0 To windRects.Count - 1
                ' Calculate wind rectangle color based on wind speed using the BlueGradientPalette
                Dim windSpeed As Single = windSpeeds(i)
                Dim windColorIndex As Integer = 25 - CInt(Math.Round((windSpeed / 25) * 25))
                If windColorIndex < 0 Then windColorIndex = 0
                If windColorIndex > 25 Then windColorIndex = 25
                Dim windColor As Color = BlueGradientPalette(windColorIndex)

                ' Set text color based on wind color brightness
                Dim textColor As Color = If(windColor.GetBrightness() > 0.8, Color.Black, Color.White)

                ' Draw the rectangle with a dark blue border
                e.Graphics.FillRectangle(New SolidBrush(windColor), windRects(i))
                e.Graphics.DrawRectangle(New Pen(Color.DarkBlue, 1), windRects(i))

                Dim windSize As SizeF = e.Graphics.MeasureString(windInfos(i), theFont)
                Dim windLocation As New Point(windRects(i).Left + (windRects(i).Width - windSize.Width) / 2, windRects(i).Top + (windRects(i).Height - windSize.Height) / 2)
                e.Graphics.DrawString(windInfos(i), theFont, New SolidBrush(textColor), windLocation)
            Next

        End If

    End Sub

    Private Sub DrawCloudLayers(ByVal e As PaintEventArgs, ByVal altitudePositions As Dictionary(Of Integer, Single))

        If _WeatherInfo IsNot Nothing Then

            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim cloudRects As New List(Of Rectangle)
            Dim cloudInfos As New List(Of Tuple(Of String, String))
            Dim cloudDensities As New List(Of Single)  ' List to store cloud densities

            Dim spaceForRightLabel As Integer = 20
            If _prefUnits.Altitude = PreferredUnits.AltitudeUnits.Both Then
                spaceForRightLabel = 60
            End If

            ' 1. Define all rectangles
            For Each cloud In _WeatherInfo.CloudLayers
                If cloud.IsValidCloudLayer Then
                    Dim topInFeet As Single = Math.Round(cloud.AltitudeTop * meterToFeet)
                    Dim bottomInFeet As Single = Math.Round(cloud.AltitudeBottom * meterToFeet)

                    Dim cloudTopY As Single = If(altitudePositions.ContainsKey(topInFeet), altitudePositions(topInFeet), InterpolateAltitudePosition(topInFeet, altitudePositions))
                    Dim cloudBottomY As Single = If(altitudePositions.ContainsKey(bottomInFeet), altitudePositions(bottomInFeet), InterpolateAltitudePosition(bottomInFeet, altitudePositions))

                    Dim cloudHeight As Single = Math.Max(cloudBottomY - cloudTopY, theFont.GetHeight(e.Graphics) + 2) ' Ensure minimum height based on text size + padding
                    Dim cloudRect As New Rectangle(CInt(Width / 2) + 10, CInt(cloudTopY), CInt(Width / 2) - spaceForRightLabel, CInt(cloudHeight))
                    cloudRects.Add(cloudRect)

                    Dim line1 As String = String.Empty
                    Select Case _prefUnits.Altitude
                        Case PreferredUnits.AltitudeUnits.Imperial
                            line1 = $"{bottomInFeet} to {topInFeet}’"
                        Case PreferredUnits.AltitudeUnits.Metric
                            line1 = $"{Conversions.FeetToMeters(bottomInFeet):0} to {Conversions.FeetToMeters(topInFeet):0} m"
                        Case PreferredUnits.AltitudeUnits.Both
                            line1 = $"{bottomInFeet} to {topInFeet}’ ({Conversions.FeetToMeters(bottomInFeet):0} to {Conversions.FeetToMeters(topInFeet):0} m)"
                    End Select
                    Dim line2 As String = $"C: {cloud.Coverage}% D: {cloud.Density} S: {cloud.Scattering}%"

                    If cloudHeight < theFont.GetHeight(e.Graphics) * 2 Then
                        cloudInfos.Add(Tuple.Create(line1 + " " + line2, ""))
                    Else
                        cloudInfos.Add(Tuple.Create(line1, line2))
                    End If
                    cloudDensities.Add(cloud.Density)
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
                ' Calculate cloud rectangle color using the stored cloud density and the GreyGradientPalette
                Dim cloudDensity As Single = cloudDensities(i)
                Dim cloudColorIndex As Integer = CInt(Math.Round((cloudDensity / 5) * 49))
                If cloudColorIndex < 0 Then cloudColorIndex = 0
                If cloudColorIndex > 49 Then cloudColorIndex = 49
                Dim cloudColor As Color = GreyGradientPalette(cloudColorIndex)

                ' Set text color based on cloud color brightness
                Dim textColor As Color = If(cloudColor.GetBrightness() > 0.65, Color.Black, Color.White)

                ' Draw the rectangle with the calculated color
                e.Graphics.FillRectangle(New SolidBrush(cloudColor), cloudRects(i))

                ' Add dark blue border to cloud rectangle
                e.Graphics.DrawRectangle(New Pen(Color.Black, 1), cloudRects(i))

                Dim totalHeightForTwoLines As Single = 2 * theFont.GetHeight(e.Graphics)
                Dim startYForTwoLines As Single = cloudRects(i).Top + (cloudRects(i).Height - totalHeightForTwoLines) / 2

                Dim line1Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item1, theFont)
                Dim line1Location As New Point(cloudRects(i).Left + (cloudRects(i).Width - line1Size.Width) / 2, If(String.IsNullOrEmpty(cloudInfos(i).Item2), cloudRects(i).Top + (cloudRects(i).Height - line1Size.Height) / 2, startYForTwoLines))
                e.Graphics.DrawString(cloudInfos(i).Item1, theFont, New SolidBrush(textColor), line1Location)

                If Not String.IsNullOrEmpty(cloudInfos(i).Item2) Then
                    Dim line2Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item2, theFont)
                    Dim line2Location As New Point(cloudRects(i).Left + (cloudRects(i).Width - line2Size.Width) / 2, startYForTwoLines + theFont.GetHeight(e.Graphics))
                    e.Graphics.DrawString(cloudInfos(i).Item2, theFont, New SolidBrush(textColor), line2Location)
                End If
            Next

        End If

    End Sub

    Private Function InterpolateAltitudePosition(altitudeInFeet As Single, altitudePositions As Dictionary(Of Integer, Single)) As Single
        ' Find the nearest upper and lower reference altitudes
        If altitudeInFeet = 60000 Then
            altitudeInFeet = 59999
        End If
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

