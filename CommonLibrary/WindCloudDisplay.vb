Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class WindCloudDisplay
    Inherits Control

    Private _WeatherInfo As WeatherDetails = Nothing
    Private _blueGradientPalette As List(Of Color)
    Private _greyGradientPalette As List(Of Color)
    Private _prefUnits As PreferredUnits

    Public Sub New()
        MyBase.New

        ' Define the gradient palette with 26 shades of blue
        _blueGradientPalette = New List(Of Color) From {
            Color.FromArgb(0, 0, 255),    ' HSV(238, 100, 100)
            Color.FromArgb(11, 11, 255),  ' HSV(238, 97, 100)
            Color.FromArgb(21, 21, 255),  ' HSV(238, 95, 100)
            Color.FromArgb(32, 32, 255),  ' HSV(238, 92, 100)
            Color.FromArgb(42, 42, 255),  ' HSV(238, 90, 100)
            Color.FromArgb(53, 53, 255),  ' HSV(238, 87, 100)
            Color.FromArgb(63, 63, 255),  ' HSV(238, 84, 100)
            Color.FromArgb(74, 74, 255),  ' HSV(238, 82, 100)
            Color.FromArgb(84, 84, 255),  ' HSV(238, 79, 100)
            Color.FromArgb(95, 95, 255),  ' HSV(238, 77, 100)
            Color.FromArgb(105, 105, 255), ' HSV(238, 74, 100)
            Color.FromArgb(116, 116, 255), ' HSV(238, 71, 100)
            Color.FromArgb(126, 126, 255), ' HSV(238, 69, 100)
            Color.FromArgb(137, 137, 255), ' HSV(238, 66, 100)
            Color.FromArgb(147, 147, 255), ' HSV(238, 64, 100)
            Color.FromArgb(158, 158, 255), ' HSV(238, 61, 100)
            Color.FromArgb(168, 168, 255), ' HSV(238, 58, 100)
            Color.FromArgb(179, 179, 255), ' HSV(238, 56, 100)
            Color.FromArgb(189, 189, 255), ' HSV(238, 53, 100)
            Color.FromArgb(200, 200, 255), ' HSV(238, 51, 100)
            Color.FromArgb(210, 210, 255), ' HSV(238, 48, 100)
            Color.FromArgb(221, 221, 255), ' HSV(238, 45, 100)
            Color.FromArgb(231, 231, 255), ' HSV(238, 43, 100)
            Color.FromArgb(242, 242, 255), ' HSV(238, 40, 100)
            Color.FromArgb(252, 252, 255), ' HSV(238, 38, 100)
            Color.FromArgb(255, 255, 255)  ' HSV(238, 35, 100)
        }

        ' Define the gradient palette with 50 shades of grey
        _greyGradientPalette = New List(Of Color) From {
            Color.FromArgb(225, 225, 225),
            Color.FromArgb(222, 222, 222),
            Color.FromArgb(219, 219, 219),
            Color.FromArgb(216, 216, 216),
            Color.FromArgb(213, 213, 213),
            Color.FromArgb(210, 210, 210),
            Color.FromArgb(207, 207, 207),
            Color.FromArgb(204, 204, 204),
            Color.FromArgb(201, 201, 201),
            Color.FromArgb(198, 198, 198),
            Color.FromArgb(195, 195, 195),
            Color.FromArgb(192, 192, 192),
            Color.FromArgb(189, 189, 189),
            Color.FromArgb(186, 186, 186),
            Color.FromArgb(183, 183, 183),
            Color.FromArgb(180, 180, 180),
            Color.FromArgb(177, 177, 177),
            Color.FromArgb(174, 174, 174),
            Color.FromArgb(171, 171, 171),
            Color.FromArgb(168, 168, 168),
            Color.FromArgb(165, 165, 165),
            Color.FromArgb(162, 162, 162),
            Color.FromArgb(159, 159, 159),
            Color.FromArgb(156, 156, 156),
            Color.FromArgb(153, 153, 153),
            Color.FromArgb(150, 150, 150),
            Color.FromArgb(147, 147, 147),
            Color.FromArgb(144, 144, 144),
            Color.FromArgb(141, 141, 141),
            Color.FromArgb(138, 138, 138),
            Color.FromArgb(135, 135, 135),
            Color.FromArgb(132, 132, 132),
            Color.FromArgb(129, 129, 129),
            Color.FromArgb(126, 126, 126),
            Color.FromArgb(123, 123, 123),
            Color.FromArgb(120, 120, 120),
            Color.FromArgb(117, 117, 117),
            Color.FromArgb(114, 114, 114),
            Color.FromArgb(111, 111, 111),
            Color.FromArgb(108, 108, 108),
            Color.FromArgb(105, 105, 105),
            Color.FromArgb(102, 102, 102),
            Color.FromArgb(99, 99, 99),
            Color.FromArgb(96, 96, 96),
            Color.FromArgb(93, 93, 93),
            Color.FromArgb(90, 90, 90),
            Color.FromArgb(87, 87, 87),
            Color.FromArgb(84, 84, 84),
            Color.FromArgb(81, 81, 81),
            Color.FromArgb(78, 78, 78)
        }

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
        Dim textHeight As Single = Font.GetHeight()
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
        Dim textHeight As Single = Font.GetHeight(e.Graphics)
        Dim drawableHeight As Single = Height - 2 * textHeight
        Dim drawableWidth As Single = e.ClipRectangle.Width

        ' Position for 10k line at the vertical middle
        Dim yPos10k As Single = textHeight + drawableHeight / 2

        altitudePositions.Add(10000, yPos10k)

        ' Draw the 10k line in the middle
        Dim middleAltitudeLabel As String = If(_prefUnits.Altitude = PreferredUnits.AltitudeUnits.Metric, "3048", "10k")
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos10k, drawableWidth, yPos10k)
        e.Graphics.DrawString(middleAltitudeLabel, New Font("Arial", 10), Brushes.Black, 0, yPos10k)

        ' Calculate the decremental step from 10k down to -2k
        Dim decrementStep As Single = (yPos10k - textHeight) / 12 ' We have 12 increments from 10k to -2k 

        ' Draw lines only at 0 and -2k
        Dim yPos0 As Single = yPos10k + 10 * decrementStep
        altitudePositions.Add(0, yPos0)
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos0, drawableWidth, yPos0)
        e.Graphics.DrawString("0", New Font("Arial", 10), Brushes.Black, 0, yPos0)

        Dim yPosNeg2k As Single = yPos10k + 12 * decrementStep
        altitudePositions.Add(-2000, yPosNeg2k)
        Dim altitudeLabel As String
        If PreferredUnits.AltitudeUnits.Metric Then
            altitudeLabel = "-610"
        Else
            altitudeLabel = "-2k"
        End If
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPosNeg2k, drawableWidth, yPosNeg2k)
        e.Graphics.DrawString(altitudeLabel, New Font("Arial", 10), Brushes.Black, 0, yPosNeg2k)

        ' Calculate the incremental step from 10k up to 60k
        Dim incrementStep As Single = (yPos10k - textHeight) / 5 ' We have 5 increments from 10k to 60k 

        ' Convert and draw lines for 20k, 30k, 40k, 50k, and 60k
        For i As Integer = 1 To 5
            Dim altitude As Integer = (10 + i * 10) * 1000
            Dim yPos As Single = yPos10k - i * incrementStep
            If _prefUnits.Altitude = PreferredUnits.AltitudeUnits.Metric Then
                altitudeLabel = (altitude * 0.3048).ToString("0000")
            Else
                altitudeLabel = (altitude / 1000).ToString() + "k"
            End If
            altitudePositions.Add(altitude, yPos)
            e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), 0, yPos, drawableWidth, yPos)
            e.Graphics.DrawString(altitudeLabel, New Font("Arial", 10), Brushes.Black, 0, yPos)
        Next

        ' Draw the vertical line in the center
        e.Graphics.DrawLine(New Pen(Color.DarkGray, 1), CInt(Width / 2), 0, CInt(Width / 2), Height)

        Return altitudePositions

    End Function

    Private Sub DrawWindLayers(ByVal e As PaintEventArgs, ByVal altitudePositions As Dictionary(Of Integer, Single))

        If _WeatherInfo IsNot Nothing Then

            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim windRects As New List(Of Rectangle)
            Dim windInfos As New List(Of String)
            Dim windSpeeds As New List(Of Integer)

            ' 1. Define all rectangles
            For Each wind In _WeatherInfo.WindLayers
                Dim altitudeInFeet As Integer = CInt(Math.Round(wind.Altitude * meterToFeet))

                ' Check if the altitude exists in the dictionary
                Dim windYPosition As Single = InterpolateAltitudePosition(altitudeInFeet, altitudePositions)
                Dim windRect As New Rectangle(50, CInt(windYPosition - Font.GetHeight(e.Graphics) / 2), CInt(Width / 2) - 70, CInt(Font.GetHeight(e.Graphics)))
                windRects.Add(windRect)

                Dim windInfo As String
                Dim altPart As String
                Dim speedPart As String
                If _prefUnits.Altitude = PreferredUnits.AltitudeUnits.Metric Then
                    altPart = $"{Conversions.FeetToMeters(altitudeInFeet):N0} m"
                Else
                    altPart = $"{altitudeInFeet}'"
                End If
                If _prefUnits.WindSpeed = PreferredUnits.WindSpeedUnits.MeterPerSecond Then
                    speedPart = $"{Conversions.KnotsToMps(wind.Speed):N1} m/s"
                Else
                    speedPart = $"{wind.Speed} kts"
                End If

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
                ' Calculate wind rectangle color based on wind speed using the _blueGradientPalette
                Dim windSpeed As Single = windSpeeds(i)
                Dim windColorIndex As Integer = 25 - CInt(Math.Round((windSpeed / 25) * 25))
                If windColorIndex < 0 Then windColorIndex = 0
                If windColorIndex > 25 Then windColorIndex = 25
                Dim windColor As Color = _blueGradientPalette(windColorIndex)

                ' Set text color based on wind color brightness
                Dim textColor As Color = If(windColor.GetBrightness() > 0.8, Color.Black, Color.White)

                ' Draw the rectangle with a dark blue border
                e.Graphics.FillRectangle(New SolidBrush(windColor), windRects(i))
                e.Graphics.DrawRectangle(New Pen(Color.DarkBlue, 1), windRects(i))

                Dim windSize As SizeF = e.Graphics.MeasureString(windInfos(i), Font)
                Dim windLocation As New Point(windRects(i).Left + (windRects(i).Width - windSize.Width) / 2, windRects(i).Top + (windRects(i).Height - windSize.Height) / 2)
                e.Graphics.DrawString(windInfos(i), Font, New SolidBrush(textColor), windLocation)
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

                    Dim line1 As String
                    If _prefUnits.Altitude = PreferredUnits.AltitudeUnits.Metric Then
                        line1 = $"{Conversions.FeetToMeters(bottomInFeet):N0} m to {Conversions.FeetToMeters(topInFeet):N0} m"
                    Else
                        line1 = $"{bottomInFeet}’ to {topInFeet}’"
                    End If
                    Dim line2 As String = $"Cov. {cloud.Coverage}% Dens. {cloud.Density} Scat. {cloud.Scattering}%"

                    If cloudHeight < Font.GetHeight(e.Graphics) * 2 Then
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
                ' Calculate cloud rectangle color using the stored cloud density and the _greyGradientPalette
                Dim cloudDensity As Single = cloudDensities(i)
                Dim cloudColorIndex As Integer = CInt(Math.Round((cloudDensity / 5) * 49))
                If cloudColorIndex < 0 Then cloudColorIndex = 0
                If cloudColorIndex > 49 Then cloudColorIndex = 49
                Dim cloudColor As Color = _greyGradientPalette(cloudColorIndex)

                ' Set text color based on cloud color brightness
                Dim textColor As Color = If(cloudColor.GetBrightness() > 0.65, Color.Black, Color.White)

                ' Draw the rectangle with the calculated color
                e.Graphics.FillRectangle(New SolidBrush(cloudColor), cloudRects(i))

                ' Add dark blue border to cloud rectangle
                e.Graphics.DrawRectangle(New Pen(Color.Black, 1), cloudRects(i))

                Dim totalHeightForTwoLines As Single = 2 * Font.GetHeight(e.Graphics)
                Dim startYForTwoLines As Single = cloudRects(i).Top + (cloudRects(i).Height - totalHeightForTwoLines) / 2

                Dim line1Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item1, Font)
                Dim line1Location As New Point(cloudRects(i).Left + (cloudRects(i).Width - line1Size.Width) / 2, If(String.IsNullOrEmpty(cloudInfos(i).Item2), cloudRects(i).Top + (cloudRects(i).Height - line1Size.Height) / 2, startYForTwoLines))
                e.Graphics.DrawString(cloudInfos(i).Item1, Font, New SolidBrush(textColor), line1Location)

                If Not String.IsNullOrEmpty(cloudInfos(i).Item2) Then
                    Dim line2Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item2, Font)
                    Dim line2Location As New Point(cloudRects(i).Left + (cloudRects(i).Width - line2Size.Width) / 2, startYForTwoLines + Font.GetHeight(e.Graphics))
                    e.Graphics.DrawString(cloudInfos(i).Item2, Font, New SolidBrush(textColor), line2Location)
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

