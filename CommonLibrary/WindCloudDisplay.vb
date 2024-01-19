Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
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

        Me.DoubleBuffered = True

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

        ' Create a rectangle to fill the entire background
        Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)

        ' Define the gradient's start and end colors
        Dim gradientTop As Color = Color.FromArgb(0, 191, 255) ' Deep sky blue
        Dim gradientBottom As Color = Color.FromArgb(200, 240, 255) ' Lighter blue

        ' Create the LinearGradientBrush
        Using brush As New LinearGradientBrush(rect, gradientTop, gradientBottom, LinearGradientMode.Vertical)
            ' Apply the gradient to the background
            e.Graphics.FillRectangle(brush, rect)
        End Using

        ' Define offsets for the sun effect
        Dim offsetX As Integer = 100 ' Move the effect 100 pixels to the right
        Dim offsetY As Integer = 100 ' Move the effect 100 pixels down

        ' Define the radial gradient for the sunlight effect
        Dim sunGradientDiameter As Integer = Me.Width ' Use the width of the control for the diameter
        Dim sunGradientRect As New Rectangle(-sunGradientDiameter / 2 + offsetX, -sunGradientDiameter / 2 + offsetY, sunGradientDiameter, sunGradientDiameter)
        Dim sunGradientCenter As New PointF(sunGradientRect.X + sunGradientRect.Width / 2, sunGradientRect.Y + sunGradientRect.Height / 2)

        ' Create the radial gradient
        Using path As New Drawing2D.GraphicsPath()
            path.AddEllipse(sunGradientRect)
            Using brush As New PathGradientBrush(path)
                brush.CenterColor = Color.FromArgb(220, 255, 255, 255) ' Fully opaque white for a strong light effect
                brush.SurroundColors = New Color() {Color.Transparent}
                brush.CenterPoint = sunGradientCenter

                ' Apply the radial gradient over the sky gradient
                e.Graphics.FillEllipse(brush, sunGradientRect)
            End Using
        End Using

        If _prefUnits Is Nothing Then
            _prefUnits = New PreferredUnits
        End If

        ' Calculate the text height and drawable height
        Dim textHeight As Single = theFont.GetHeight()
        Dim drawableHeight As Single = Height - 1.5 * textHeight

        ' List of altitudes where lines and labels should be drawn
        Dim specifiedAltitudes As New List(Of Integer)({-2, 0, 10, 20, 30, 40, 50, 60})

        ' Call the new method to draw the grid lines and labels
        Dim altitudePositions As Dictionary(Of Integer, Single) = CalculateAltitudePositions(e)

        ' Background for below zero
        Dim groundGradientTop As Color = Color.FromArgb(34, 139, 34) ' Forest green
        Dim groundGradientBottom As Color = Color.FromArgb(139, 69, 19) ' Saddle brown
        Dim seaGradientTop As Color = Color.FromArgb(0, 105, 148) ' Deep sea blue
        Dim seaGradientBottom As Color = Color.FromArgb(0, 191, 255) ' Light sky blue

        If _WeatherInfo IsNot Nothing Then
            ' Choose the correct gradient based on the AltitudeMeasurement setting
            gradientTop = If(_WeatherInfo.AltitudeMeasurement = "AMGL", groundGradientTop, seaGradientTop)
            gradientBottom = If(_WeatherInfo.AltitudeMeasurement = "AMGL", groundGradientBottom, seaGradientBottom)
            ' Define the rectangle for below zero
            Dim belowZeroRect As New Rectangle(0, altitudePositions(0), Me.Width, Me.Height - altitudePositions(0))

            ' Create and apply the gradient for below zero
            Using brush As New LinearGradientBrush(belowZeroRect, gradientTop, gradientBottom, LinearGradientMode.Vertical)
                e.Graphics.FillRectangle(brush, belowZeroRect)
            End Using
        End If


        DrawGridLinesAndLabels(e, altitudePositions)

        ' Call the new method to draw wind layers
        DrawWindLayers(e, altitudePositions)

        DrawCloudLayers(e, altitudePositions)

    End Sub

    Private Function CalculateAltitudePositions(ByVal e As PaintEventArgs) As Dictionary(Of Integer, Single)
        ' Dictionary to store altitude vs. vertical position
        Dim altitudePositions As New Dictionary(Of Integer, Single)

        ' Calculate the true available space
        Dim textHeight As Single = theFont.GetHeight(e.Graphics)
        Dim drawableHeight As Single = Height - 2 * textHeight
        Dim yPos10k As Single = textHeight + drawableHeight / 2

        ' Define positions
        altitudePositions.Add(10000, yPos10k)
        altitudePositions.Add(5000, yPos10k + 5 * (yPos10k - textHeight) / 12)
        altitudePositions.Add(0, yPos10k + 10 * (yPos10k - textHeight) / 12)
        altitudePositions.Add(-2000, yPos10k + 12 * (yPos10k - textHeight) / 12)

        ' Incremental step from 10k up to 60k
        For i As Integer = 1 To 5
            Dim altitude As Integer = (10 + i * 10) * 1000
            altitudePositions.Add(altitude, yPos10k - i * (yPos10k - textHeight) / 5)
        Next

        Return altitudePositions
    End Function


    Private Sub DrawGridLinesAndLabels(ByVal e As PaintEventArgs, altitudePositions As Dictionary(Of Integer, Single))

        ' Calculate the true available space
        Dim textHeight As Single = theFont.GetHeight(e.Graphics)
        Dim drawableHeight As Single = Height - 2 * textHeight
        Dim drawableWidth As Single = e.ClipRectangle.Width
        Dim rightEdge As Single = drawableWidth

        For Each kvp As KeyValuePair(Of Integer, Single) In altitudePositions
            Dim altitude = kvp.Key
            Dim yPos = kvp.Value

            ' Determine if this is the zero line or not
            If altitude = 0 Then
                DrawHorizontal3DLine(e, New PointF(0, yPos), New PointF(drawableWidth, yPos), Color.DarkGray, 2)
                DrawHorizontal3DLine(e, New PointF(0, yPos - 1), New PointF(drawableWidth + 1, yPos - 1), Color.Black, 2)
                WriteAltitudeLabel(e, rightEdge, altitude, yPos)
            Else
                DrawHorizontal3DLine(e, New PointF(0, yPos), New PointF(drawableWidth, yPos), Color.DarkGray, 2)
                WriteAltitudeLabel(e, rightEdge, altitude, yPos)
            End If

        Next

        'Write Feet and Meters at the top of the graph
        WriteAltitudeLabel(e, rightEdge, 0, 0, True)

        'Write the legend for C D S on top
        Dim legend As String = "C:Coverage D:Density S:Scattered"
        Dim textSize As SizeF = e.Graphics.MeasureString(legend, theFont)
        Dim textXPosition As Single = CInt(Width * 0.75) - CInt(textSize.Width / 2)
        e.Graphics.DrawString(legend, theFont, Brushes.Black, textXPosition, 0)


        ' Draw the vertical line in the center
        DrawVertical3DLine(e, CInt(Width / 2), 0, Height, Color.DarkGray, 2)

    End Sub

    Private Sub DrawVertical3DLine(e As PaintEventArgs, lineX As Single, topY As Single, bottomY As Single, lineColor As Color, shadowOffset As Integer)
        ' Shadow color, semi-transparent black
        Dim shadowColor As Color = Color.FromArgb(50, 0, 0, 0)

        ' Draw shadow first
        e.Graphics.DrawLine(New Pen(shadowColor, 1), lineX + shadowOffset, topY, lineX + shadowOffset, bottomY)

        ' Create gradient brush for line
        Dim startPoint As New PointF(lineX, topY)
        Dim endPoint As New PointF(lineX, bottomY)
        Using gradientBrush As LinearGradientBrush = New LinearGradientBrush(startPoint, endPoint, Color.White, lineColor)
            ' Blend the colors for a more pronounced 3D effect
            Dim blend As New Blend()
            blend.Positions = New Single() {0.0F, 0.5F, 1.0F}
            blend.Factors = New Single() {0.0F, 1.0F, 0.0F}
            gradientBrush.Blend = blend

            ' Draw the line with the gradient brush
            e.Graphics.DrawLine(New Pen(gradientBrush, 1), startPoint, endPoint)
        End Using
    End Sub

    Private Function DrawHorizontal3DLine(e As PaintEventArgs, startPoint As PointF, endPoint As PointF, lineColor As Color, shadowOffset As Integer)
        ' Shadow color, semi-transparent black
        Dim shadowColor As Color = Color.FromArgb(50, 0, 0, 0)

        ' Draw shadow first
        Dim shadowStart As New PointF(startPoint.X + shadowOffset, startPoint.Y + shadowOffset)
        Dim shadowEnd As New PointF(endPoint.X + shadowOffset, endPoint.Y + shadowOffset)
        e.Graphics.DrawLine(New Pen(shadowColor, 1), shadowStart, shadowEnd)

        ' Create gradient brush for line
        Using gradientBrush As LinearGradientBrush = New LinearGradientBrush(startPoint, endPoint, Color.White, lineColor)
            ' Blend the colors for a more pronounced 3D effect
            Dim blend As New Blend()
            blend.Positions = New Single() {0.0F, 0.5F, 1.0F}
            blend.Factors = New Single() {0.0F, 1.0F, 0.0F}
            gradientBrush.Blend = blend

            ' Draw the line with the gradient brush
            e.Graphics.DrawLine(New Pen(gradientBrush, 1), startPoint, endPoint)
        End Using
    End Function

    Private Sub WriteAltitudeLabel(e As PaintEventArgs, rightEdge As Single, altitude As Integer, yPos As Single, Optional unitLabel As Boolean = False)
        Dim altitudeLabel As String
        If _WeatherInfo IsNot Nothing Then

            Select Case _prefUnits.Altitude
                Case PreferredUnits.AltitudeUnits.Metric
                    If Not unitLabel Then
                        altitudeLabel = (altitude * 0.3048).ToString("0")
                    Else
                        altitudeLabel = $"Meters {_WeatherInfo.AltitudeMeasurement}"
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
                        altitudeLabel = $"Feet {_WeatherInfo.AltitudeMeasurement}"
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
                        altitudeLabel = $"Feet {_WeatherInfo.AltitudeMeasurement}"
                    End If
                    e.Graphics.DrawString(altitudeLabel, theFont, Brushes.Black, 0, yPos)

                    'Meters on the right
                    If Not unitLabel Then
                        altitudeLabel = (altitude * 0.3048).ToString("0")
                    Else
                        altitudeLabel = $"Meters {_WeatherInfo.AltitudeMeasurement}"
                    End If
                    Dim textSize As SizeF = e.Graphics.MeasureString(altitudeLabel, theFont)
                    Dim textXPosition As Single = rightEdge - textSize.Width ' Align to the right
                    e.Graphics.DrawString(altitudeLabel, theFont, Brushes.Black, textXPosition, yPos)
            End Select

        End If

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
                        altPart = $"{Conversions.FeetToMeters(altitudeInFeet):0} m"
                    Case PreferredUnits.AltitudeUnits.Both
                        altPart = $"{altitudeInFeet}' ({Conversions.FeetToMeters(altitudeInFeet):0} m)"
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

            ' 3. Draw the rectangles with rounded corners and shadows
            For i = 0 To windRects.Count - 1
                ' Calculate wind rectangle color based on wind speed using the BlueGradientPalette
                Dim windSpeed As Single = windSpeeds(i)
                Dim windColorIndex As Integer = 25 - CInt(Math.Round((windSpeed / 25) * 25))
                If windColorIndex < 0 Then windColorIndex = 0
                If windColorIndex > 25 Then windColorIndex = 25
                Dim windColor As Color = BlueGradientPalette(windColorIndex)

                ' Set text color based on wind color brightness
                Dim textColor As Color = If(windColor.GetBrightness() > 0.8, Color.Black, Color.White)

                ' Draw the rectangle with shadow and rounded corners using the new helper method
                DrawRoundedRectangleWithShadow(e.Graphics, windRects(i), New Pen(Color.DarkBlue, 1), windColor)

                ' Measure and draw the text
                Dim windSize As SizeF = e.Graphics.MeasureString(windInfos(i), theFont)
                Dim windLocation As New Point(windRects(i).Left + (windRects(i).Width - windSize.Width) / 2, windRects(i).Top + (windRects(i).Height - windSize.Height) / 2)
                e.Graphics.DrawString(windInfos(i), theFont, New SolidBrush(textColor), windLocation)
            Next

        End If

    End Sub

    Private Sub DrawCloudLayers(ByVal e As PaintEventArgs, ByVal altitudePositions As Dictionary(Of Integer, Single))

        Dim backgroundColor As Color = Color.FromArgb(159, 213, 235)

        If _WeatherInfo IsNot Nothing Then

            ' Convert altitude in meters to feet for easier positioning
            Dim meterToFeet As Single = 3.28084

            Dim cloudRects As New List(Of Rectangle)
            Dim cloudInfos As New List(Of Tuple(Of String, String))
            Dim cloudDensities As New List(Of Single)  ' List to store cloud densities
            Dim cloudCoverages As New List(Of Single)  ' List to store cloud coverages
            Dim cloudScattereds As New List(Of Single)  ' List to store cloud scattereds

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
                    cloudCoverages.Add(cloud.Coverage)
                    cloudScattereds.Add(cloud.Scattering)
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
                textColor = Color.Black

                ' Draw the cloud rectangle with shadow and rounded corners using the new helper method
                DrawRoundedRectangleWithShadow(e.Graphics, cloudRects(i), New Pen(Color.Black, 1), backgroundColor)

                Dim cloudCoverageColor As Color = Color.FromArgb(
                                                    Math.Min(cloudColor.R - 35, 255),
                                                    Math.Min(cloudColor.G - 35, 255),
                                                    Math.Min(cloudColor.B - 35, 255))
                Dim cloudCoverage As Single = cloudCoverages(i) ' Assuming this is a value between 0 and 1
                DrawDynamicCoverageBars(e, cloudRects(i), cloudCoverage, cloudScattereds(i), cloudCoverageColor, 1)

                Dim paddingBetweenLines As Single = 2 ' Adjust this value for more space between lines

                Dim totalHeightForTwoLines As Single = 2 * theFont.GetHeight(e.Graphics) + paddingBetweenLines
                Dim startYForTwoLines As Single = cloudRects(i).Top + (cloudRects(i).Height - totalHeightForTwoLines) / 2

                Dim line1Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item1, theFont)

                ' Calculate the Y-position differently based on whether there's a second line of text
                Dim line1YPosition As Single
                If String.IsNullOrEmpty(cloudInfos(i).Item2) Then
                    ' Center the single line of text vertically in the cloud rectangle
                    line1YPosition = cloudRects(i).Top + (cloudRects(i).Height - line1Size.Height) / 2
                Else
                    ' Position the first of two lines of text
                    line1YPosition = startYForTwoLines
                End If

                Dim line1Location As New PointF(cloudRects(i).Left + (cloudRects(i).Width - line1Size.Width) / 2, line1YPosition)
                DrawTextWithBackground(e, cloudInfos(i).Item1, theFont, textColor, backgroundColor, line1Location)

                If Not String.IsNullOrEmpty(cloudInfos(i).Item2) Then
                    Dim line2Size As SizeF = e.Graphics.MeasureString(cloudInfos(i).Item2, theFont)
                    Dim line2Location As New PointF(cloudRects(i).Left + (cloudRects(i).Width - line2Size.Width) / 2, line1YPosition + theFont.GetHeight(e.Graphics) + paddingBetweenLines)
                    DrawTextWithBackground(e, cloudInfos(i).Item2, theFont, textColor, backgroundColor, line2Location)
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

    Private Sub DrawRoundedRectangleWithShadow(graphics As Graphics, rectangle As Rectangle, pen As Pen, fillColor As Color, Optional cornerRadius As Integer = 5, Optional shadowOffset As Integer = 4)

        Dim shadowColor As Color = Color.FromArgb(60, 0, 0, 0) ' Semi-transparent black for shadow

        ' Draw the shadow rectangle first with a slightly larger radius
        Dim shadowRect As New Rectangle(rectangle.X + shadowOffset, rectangle.Y + shadowOffset, rectangle.Width, rectangle.Height)
        DrawRoundedRectangle(graphics, shadowRect, cornerRadius + 2, Pens.Transparent, shadowColor)

        ' Now draw the main rectangle
        DrawRoundedRectangle(graphics, rectangle, cornerRadius, pen, Color.FromArgb(200, fillColor))
    End Sub

    Private Sub DrawRoundedRectangle(graphics As Graphics, rectangle As Rectangle, radius As Integer, pen As Pen, fillColor As Color)
        Dim path As New Drawing2D.GraphicsPath()
        With path
            .AddArc(rectangle.X, rectangle.Y, radius, radius, 180, 90)
            .AddArc(rectangle.X + rectangle.Width - radius, rectangle.Y, radius, radius, 270, 90)
            .AddArc(rectangle.X + rectangle.Width - radius, rectangle.Y + rectangle.Height - radius, radius, radius, 0, 90)
            .AddArc(rectangle.X, rectangle.Y + rectangle.Height - radius, radius, radius, 90, 90)
            .CloseFigure()
        End With

        graphics.FillPath(New SolidBrush(fillColor), path)
        If pen IsNot Pens.Transparent Then
            graphics.DrawPath(pen, path)
        End If
    End Sub

    Private Sub DrawDynamicCoverageBars(e As PaintEventArgs, cloudRect As Rectangle, coverage As Single, scattered As Single, barColor As Color, borderWidth As Integer)
        ' Adjust the rectangle to account for the border
        Dim innerRect As New Rectangle(cloudRect.X + borderWidth, cloudRect.Y + borderWidth,
                           cloudRect.Width - 2 * borderWidth, cloudRect.Height - 2 * borderWidth)

        ' Randomness generator
        Dim rnd As New Random()

        ' Dynamically determine the number of bars based on scattered value
        ' 2 bars for scatter = 0 and up to 12 bars for scatter = 100
        Dim barsCount As Integer = 2 + CInt((scattered / 100) * 10)

        ' The total width of all the bars combined represents the coverage
        Dim totalBarsWidth As Single = innerRect.Width * coverage / 100
        ' Ensure a minimum bar width of 1 pixel
        Dim barWidth As Single = Math.Max(1, totalBarsWidth / barsCount)
        ' Adjust totalBarsWidth if necessary due to the 1 pixel minimum
        totalBarsWidth = barWidth * barsCount
        Dim spaceBetweenBars As Single = (innerRect.Width - totalBarsWidth) / (barsCount + 1)

        ' Draw the bars
        Dim currentX As Single = innerRect.X + spaceBetweenBars
        For i As Integer = 0 To barsCount - 1
            ' Draw individual 1 pixel lines to make up the width of the bar
            For lineX As Single = currentX To currentX + barWidth - 1
                ' Introduce randomness into the line height
                Dim randomFactor As Single = rnd.NextDouble() * scattered / 100
                Dim dynamicLineHeight As Single = innerRect.Height * (1 - randomFactor)
                Dim bottomRandomness As Single = innerRect.Height * randomFactor * 0.1

                ' Ensure that the bottom randomness doesn't make the line go outside the rectangle
                Dim bottomPosition As Single = innerRect.Bottom - bottomRandomness

                ' Draw a single vertical line starting from the bottomPosition going up the dynamicLineHeight
                e.Graphics.FillRectangle(New SolidBrush(barColor), lineX, bottomPosition - dynamicLineHeight, 1, dynamicLineHeight)
            Next

            ' Increment X position by the width of a bar plus the space between bars
            currentX += barWidth + spaceBetweenBars
        Next
    End Sub

    Private Sub DrawTextWithBackground(e As PaintEventArgs, text As String, font As Font, textColor As Color, backColor As Color, location As PointF)
        ' Measure the size of the text
        Dim textSize As SizeF = e.Graphics.MeasureString(text, font)

        ' Define the rectangle area for the semi-transparent background
        Dim padding As Integer = 0 ' You can adjust the padding around the text
        Dim backgroundRect As New RectangleF(location.X - padding, location.Y - padding, textSize.Width + (2 * padding), textSize.Height + (2 * padding))

        ' Set up the semi-transparent background color
        Dim semiTransparentBrush As New SolidBrush(Color.FromArgb(175, backColor))

        ' Draw the semi-transparent background
        e.Graphics.FillRectangle(semiTransparentBrush, backgroundRect)

        ' Draw the text on top of the background
        e.Graphics.DrawString(text, font, New SolidBrush(textColor), location)
    End Sub

End Class

