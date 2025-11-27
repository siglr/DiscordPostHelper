Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports Newtonsoft.Json

''' <summary>
''' Converts an IGC flight log file into an MSFS Flight Recorder (.fltRec) archive.
''' Ported from the reference Python script with equivalent smoothing and attitude modelling.
''' </summary>
Public Class IgcToFltRec

    Private Const M_TO_FT As Double = 3.28084
    Private Const KT_TO_MS As Double = 0.514444
    Private Const KPH_TO_KT As Double = 1.0 / 1.852
    Private Const CLIENT_VERSION As String = "0.26.0.0"
    Private Const PITCH_SMOOTH_WINDOW As Integer = 5      ' was 9
    Private Const PITCH_MAX_UP As Double = 70.0           ' was 20
    Private Const PITCH_MAX_DOWN As Double = -40.0        ' was -10

    Private Class RunSegmentInfo
        Public Property StartTime As Integer
        Public Property EndTime As Integer
        Public Property StartLat As Double
        Public Property StartLon As Double
        Public Property EndLat As Double
        Public Property EndLon As Double
    End Class

    Private Class IgcRecord
        Public Property TimeSec As Integer
        Public Property Lat As Double
        Public Property Lon As Double
        Public Property AltM As Integer
        Public Property AltAglM As Integer?
        Public Property TasKts As Double?
        Public Property NettoMs As Double?
        Public Property EnlRaw As Integer?
        Public Property FlapIndex As Integer?
        Public Property WindSpeedMs As Double?
        Public Property WindDirDeg As Integer?
        Public Property OnGroundFlag As Integer?
        Public Property GearFlag As Integer?
        Public Property FxaM As Integer?
    End Class

    Private Class FltRecPosition
        Public Property Milliseconds As Integer
        Public Property Latitude As Double
        Public Property Longitude As Double
        Public Property Altitude As Double
        Public Property AltitudeAboveGround As Double
        Public Property Pitch As Double
        Public Property Bank As Double
        Public Property TrueHeading As Double
        Public Property MagneticHeading As Double
        Public Property GyroHeading As Double
        Public Property TrueAirspeed As Double
        Public Property IndicatedAirspeed As Double
        Public Property GpsGroundSpeed As Double
        Public Property GroundSpeed As Double
        Public Property MachAirspeed As Double
        Public Property AbsoluteTime As Double
        Public Property VelocityBodyX As Double
        Public Property VelocityBodyY As Double
        Public Property VelocityBodyZ As Double
        Public Property HeadingIndicator As Double
        Public Property AIPitch As Double
        Public Property AIBank As Double
        Public Property IsOnGround As Integer
        Public Property FlapsHandleIndex As Integer
        Public Property GearHandlePosition As Integer
        Public Property WindVelocity As Double
        Public Property WindDirection As Double
        Public Property ThrottleLeverPosition1 As Double
        Public Property WingFlexPercent1 As Double
        Public Property WingFlexPercent2 As Double
        Public Property WingFlexPercent3 As Double
        Public Property WingFlexPercent4 As Double
    End Class

    Private Class FltRecRecord
        Public Property Time As Integer
        Public Property Position As FltRecPosition
    End Class

    Private Class StartState
        Public Property PlaneInParkingState As Integer
        Public Property AircraftTitle As String
        Public Property AircraftAirline As String
        Public Property AircraftNumber As String
        Public Property AircraftId As String
        Public Property AircraftModel As String
        Public Property AircraftType As String
        Public Property AircraftOnParkingSpot As Integer
    End Class

    Private Class FltRecData
        Public Property ClientVersion As String
        Public Property StartTime As Integer
        Public Property EndTime As Integer
        Public Property StartState As StartState
        Public Property Records As List(Of FltRecRecord)
    End Class

    ''' <summary>
    ''' Convert an IGC file on disk into a Flight Recorder archive on disk.
    ''' </summary>
    ''' <param name="igcPath">Path to the source IGC file.</param>
    ''' <param name="fltRecPath">Destination .fltRec path.</param>
    Public Shared Sub Convert(igcPath As String, fltRecPath As String)
        If igcPath Is Nothing Then Throw New ArgumentNullException(NameOf(igcPath))
        If fltRecPath Is Nothing Then Throw New ArgumentNullException(NameOf(fltRecPath))

        If Not File.Exists(igcPath) Then
            Throw New FileNotFoundException("IGC file not found.", igcPath)
        End If

        Dim allLines As String() = File.ReadAllLines(igcPath)
        Dim igcData As IgcParseResult = ParseIgc(igcPath)
        Dim nb21Start As DateTime? = TryGetNb21DateTimeFromLRecords(allLines)
        Dim flightStartUtc As DateTime = If(nb21Start.HasValue, nb21Start.Value, GetIgcStartDateTimeUtc(allLines, igcData.Records))
        Dim absoluteTimeBase As Double = flightStartUtc.Ticks / 10000000.0R

        Dim aircraftTitle As String = If(String.IsNullOrWhiteSpace(igcData.SuggestedAircraftTitle), "AS 33 Me (18m)", igcData.SuggestedAircraftTitle)

        Dim data As FltRecData = BuildFltRecData(igcData.Records, aircraftTitle, absoluteTimeBase)
        WriteFltRec(data, fltRecPath)
    End Sub

    Private Class IgcParseResult
        Public Property Records As List(Of IgcRecord)
        Public Property SuggestedAircraftTitle As String
    End Class

    Private Shared Function Clamp(x As Double, lo As Double, hi As Double) As Double
        Return Math.Max(lo, Math.Min(hi, x))
    End Function

    Private Shared Function Lerp(a As Double, b As Double, t As Double) As Double
        Return (1.0 - t) * a + (t * b)
    End Function

    Private Shared Function LerpAngleDegrees(a As Double, b As Double, t As Double) As Double
        Dim delta As Double = NormalizeAngle(b - a)
        Dim value As Double = a + (delta * t)
        value = (value Mod 360.0 + 360.0) Mod 360.0
        Return value
    End Function

    Private Shared Function NormalizeAngle(angle As Double) As Double
        Dim a As Double = (angle + 540.0) Mod 360.0 - 180.0
        Return a
    End Function

    Private Shared Function ParseIgc(path As String) As IgcParseResult
        Dim records As New List(Of IgcRecord)()
        Dim extMap As New Dictionary(Of String, Tuple(Of Integer, Integer))()
        Dim suggestedAircraftTitle As String = String.Empty

        For Each rawLine As String In File.ReadLines(path)
            Dim line As String = rawLine.Trim()
            If String.IsNullOrWhiteSpace(line) Then Continue For

            If String.IsNullOrWhiteSpace(suggestedAircraftTitle) Then
                suggestedAircraftTitle = ExtractTitleFromHeader(line)
            End If

            If line.StartsWith("I"c) AndAlso line.Length >= 7 Then
                Dim i As Integer = 3
                While i + 6 <= line.Length
                    Dim part As String = line.Substring(i, 7)
                    Dim startValue As Integer
                    Dim endValue As Integer
                    If Not Integer.TryParse(part.Substring(0, 2), startValue) Then Exit While
                    If Not Integer.TryParse(part.Substring(2, 2), endValue) Then Exit While
                    Dim code As String = part.Substring(4, 3)
                    extMap(code) = Tuple.Create(startValue, endValue)
                    i += 7
                End While
                Continue For
            End If

            If Not line.StartsWith("B"c) OrElse line.Length < 35 Then Continue For

            Dim tStr As String = line.Substring(1, 6)
            Dim hh As Integer
            Dim mm As Integer
            Dim ss As Integer
            If Not Integer.TryParse(tStr.Substring(0, 2), hh) Then Continue For
            If Not Integer.TryParse(tStr.Substring(2, 2), mm) Then Continue For
            If Not Integer.TryParse(tStr.Substring(4, 2), ss) Then Continue For
            Dim sec As Integer = hh * 3600 + mm * 60 + ss

            Dim latStr As String = line.Substring(7, 7)
            Dim latHem As Char = line(14)
            Dim lonStr As String = line.Substring(15, 8)
            Dim lonHem As Char = line(23)

            Dim lat As Double
            Dim lon As Double
            If Not TryDecodeCoordinate(latStr, True, lat) Then Continue For
            If latHem = "S"c Then lat = -lat
            If Not TryDecodeCoordinate(lonStr, False, lon) Then Continue For
            If lonHem = "W"c Then lon = -lon

            Dim validity As Char = line(24)
            If validity = "V"c Then Continue For

            Dim altBaro As Integer = 0
            Dim altGps As Integer = 0
            Integer.TryParse(line.Substring(25, 5), altBaro)
            If Not Integer.TryParse(line.Substring(30, 5), altGps) Then
                altGps = altBaro
            End If

            Dim altM As Integer = altGps

            Dim fxaM As Integer? = Nothing
            Dim altAglM As Integer? = Nothing
            Dim tasKts As Double? = Nothing
            Dim nettoMs As Double? = Nothing
            Dim enlRaw As Integer? = Nothing
            Dim flapIndex As Integer? = Nothing
            Dim windSpeedMs As Double? = Nothing
            Dim windDirDeg As Integer? = Nothing
            Dim onGroundFlag As Integer? = Nothing
            Dim gearFlag As Integer? = Nothing

            Dim s As String = GetExtension(line, extMap, "FXA")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then fxaM = tmp
            End If

            s = GetExtension(line, extMap, "AGL")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then altAglM = tmp
            End If

            s = GetExtension(line, extMap, "TAS")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then tasKts = tmp * KPH_TO_KT
            End If

            s = GetExtension(line, extMap, "NET")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then nettoMs = tmp / 100.0
            End If

            s = GetExtension(line, extMap, "ENL")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then enlRaw = tmp
            End If

            s = GetExtension(line, extMap, "FLP")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then flapIndex = tmp
            End If

            s = GetExtension(line, extMap, "WSP")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then
                    Dim wspKts As Double = tmp * KPH_TO_KT
                    windSpeedMs = wspKts * KT_TO_MS
                End If
            End If

            s = GetExtension(line, extMap, "WDI")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then windDirDeg = tmp
            End If

            s = GetExtension(line, extMap, "GND")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then onGroundFlag = tmp
            End If

            s = GetExtension(line, extMap, "GEA")
            If Not String.IsNullOrWhiteSpace(s) Then
                Dim tmp As Integer
                If Integer.TryParse(s, tmp) Then gearFlag = tmp
            End If

            Dim rec As New IgcRecord()
            rec.TimeSec = sec
            rec.Lat = lat
            rec.Lon = lon
            rec.AltM = altM
            rec.AltAglM = altAglM
            rec.TasKts = tasKts
            rec.NettoMs = nettoMs
            rec.EnlRaw = enlRaw
            rec.FlapIndex = flapIndex
            rec.WindSpeedMs = windSpeedMs
            rec.WindDirDeg = windDirDeg
            rec.OnGroundFlag = onGroundFlag
            rec.GearFlag = gearFlag
            rec.FxaM = fxaM

            records.Add(rec)
        Next

        If records.Count = 0 Then
            Throw New InvalidOperationException("No valid B-records found in IGC file.")
        End If

        Return New IgcParseResult With {.Records = records, .SuggestedAircraftTitle = suggestedAircraftTitle}
    End Function

    Private Shared Function TryGetNb21DateTimeFromLRecords(allLines As IEnumerable(Of String)) As DateTime?
        Dim lastLdat As String = Nothing
        Dim lastLtim As String = Nothing

        For Each rawLine As String In allLines
            If String.IsNullOrWhiteSpace(rawLine) Then Continue For
            Dim line As String = rawLine.Trim()
            If Not line.StartsWith("L", StringComparison.OrdinalIgnoreCase) Then Continue For

            If line.IndexOf("LDAT", StringComparison.OrdinalIgnoreCase) >= 0 Then
                lastLdat = line
            End If

            If line.IndexOf("LTIM", StringComparison.OrdinalIgnoreCase) >= 0 Then
                lastLtim = line
            End If
        Next

        If String.IsNullOrWhiteSpace(lastLdat) OrElse String.IsNullOrWhiteSpace(lastLtim) Then
            Return Nothing
        End If

        Dim ldatTokens As String() = lastLdat.Split(New Char() {" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        If ldatTokens.Length = 0 Then Return Nothing
        Dim dateToken As String = ldatTokens(ldatTokens.Length - 1)

        Dim ltimTokens As String() = lastLtim.Split(New Char() {" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        Dim ltimIndex As Integer = Array.FindIndex(ltimTokens, Function(t) t.Equals("LTIM", StringComparison.OrdinalIgnoreCase))
        If ltimIndex < 0 OrElse ltimIndex + 2 >= ltimTokens.Length Then
            Return Nothing
        End If
        Dim timeToken As String = ltimTokens(ltimIndex + 2)

        Try
            If dateToken.Length <> 8 OrElse timeToken.Length <> 6 Then
                Return Nothing
            End If

            Dim year As Integer = Integer.Parse(dateToken.Substring(0, 4), CultureInfo.InvariantCulture)
            Dim month As Integer = Integer.Parse(dateToken.Substring(4, 2), CultureInfo.InvariantCulture)
            Dim day As Integer = Integer.Parse(dateToken.Substring(6, 2), CultureInfo.InvariantCulture)

            Dim hour As Integer = Integer.Parse(timeToken.Substring(0, 2), CultureInfo.InvariantCulture)
            Dim minute As Integer = Integer.Parse(timeToken.Substring(2, 2), CultureInfo.InvariantCulture)
            Dim second As Integer = Integer.Parse(timeToken.Substring(4, 2), CultureInfo.InvariantCulture)

            Dim dt As New DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc)
            Return dt
        Catch
            Return Nothing
        End Try
    End Function

    Private Shared Function GetIgcStartDateTimeUtc(allLines As IEnumerable(Of String), igcRecords As List(Of IgcRecord)) As DateTime
        Dim flightDate As DateTime = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc)

        For Each rawLine As String In allLines
            Dim line As String = rawLine.Trim()
            If line.StartsWith("HFDTE", StringComparison.OrdinalIgnoreCase) AndAlso line.Length >= 11 Then
                Dim datePart As String = New String(line.Substring(5).TakeWhile(AddressOf Char.IsDigit).ToArray())
                If datePart.Length >= 6 Then
                    Try
                        Dim day As Integer = Integer.Parse(datePart.Substring(0, 2), CultureInfo.InvariantCulture)
                        Dim month As Integer = Integer.Parse(datePart.Substring(2, 2), CultureInfo.InvariantCulture)
                        Dim yearShort As Integer = Integer.Parse(datePart.Substring(4, 2), CultureInfo.InvariantCulture)
                        Dim year As Integer = If(yearShort >= 90, 1900 + yearShort, 2000 + yearShort)
                        flightDate = New DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc)
                    Catch
                        ' Ignore and keep default date
                    End Try
                End If

                Exit For
            End If
        Next

        Dim timeSeconds As Integer = 0
        If igcRecords IsNot Nothing AndAlso igcRecords.Count > 0 Then
            timeSeconds = igcRecords(0).TimeSec
        Else
            For Each rawLine As String In allLines
                Dim line As String = rawLine.Trim()
                If line.StartsWith("B"c) AndAlso line.Length >= 7 Then
                    Dim hhStr As String = line.Substring(1, 2)
                    Dim mmStr As String = line.Substring(3, 2)
                    Dim ssStr As String = line.Substring(5, 2)
                    Dim hh As Integer
                    Dim mm As Integer
                    Dim ss As Integer
                    If Integer.TryParse(hhStr, NumberStyles.Integer, CultureInfo.InvariantCulture, hh) AndAlso
                       Integer.TryParse(mmStr, NumberStyles.Integer, CultureInfo.InvariantCulture, mm) AndAlso
                       Integer.TryParse(ssStr, NumberStyles.Integer, CultureInfo.InvariantCulture, ss) Then
                        timeSeconds = hh * 3600 + mm * 60 + ss
                        Exit For
                    End If
                End If
            Next
        End If

        Return flightDate.AddSeconds(timeSeconds)
    End Function

    Private Shared Function ExtractTitleFromHeader(line As String) As String
        Dim idx As Integer = line.IndexOf("TITL", StringComparison.OrdinalIgnoreCase)
        If idx < 0 Then Return String.Empty

        Dim titleValue As String = line.Substring(idx + 4)
        titleValue = titleValue.TrimStart(" "c, vbTab, ":"c, "="c)
        Return titleValue.Trim()
    End Function

    Private Shared Function TryDecodeCoordinate(value As String, isLat As Boolean, ByRef result As Double) As Boolean
        If String.IsNullOrWhiteSpace(value) Then Return False

        Try
            If isLat Then
                Dim dd As Integer = Integer.Parse(value.Substring(0, 2))
                Dim mmmm As Integer = Integer.Parse(value.Substring(2))
                result = dd + (mmmm / 1000.0) / 60.0
            Else
                Dim dd As Integer = Integer.Parse(value.Substring(0, 3))
                Dim mmmm As Integer = Integer.Parse(value.Substring(3))
                result = dd + (mmmm / 1000.0) / 60.0
            End If
            Return True
        Catch
            result = 0
            Return False
        End Try
    End Function

    Private Shared Function GetExtension(line As String, extMap As Dictionary(Of String, Tuple(Of Integer, Integer)), code As String) As String
        If Not extMap.ContainsKey(code) Then Return String.Empty
        Dim startEnd As Tuple(Of Integer, Integer) = extMap(code)
        Dim startIdx As Integer = startEnd.Item1 - 1
        Dim endIdx As Integer = startEnd.Item2
        If startIdx >= line.Length Then Return String.Empty
        Dim length As Integer = Math.Min(line.Length, endIdx) - startIdx
        If length <= 0 Then Return String.Empty
        Return line.Substring(startIdx, length)
    End Function

    Private Shared Function Haversine(lat1 As Double, lon1 As Double, lat2 As Double, lon2 As Double) As Double
        Const R As Double = 6371000.0
        Dim phi1 As Double = DegreesToRadians(lat1)
        Dim phi2 As Double = DegreesToRadians(lat2)
        Dim dphi As Double = DegreesToRadians(lat2 - lat1)
        Dim dlambda As Double = DegreesToRadians(lon2 - lon1)
        Dim a As Double = (Math.Sin(dphi / 2) ^ 2) +
                          (Math.Cos(phi1) * Math.Cos(phi2) * Math.Sin(dlambda / 2) ^ 2)
        Dim c As Double = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a))
        Return R * c
    End Function

    Private Shared Function Bearing(lat1 As Double, lon1 As Double, lat2 As Double, lon2 As Double) As Double
        Dim phi1 As Double = DegreesToRadians(lat1)
        Dim phi2 As Double = DegreesToRadians(lat2)
        Dim dlambda As Double = DegreesToRadians(lon2 - lon1)
        Dim x As Double = Math.Sin(dlambda) * Math.Cos(phi2)
        Dim y As Double = Math.Cos(phi1) * Math.Sin(phi2) - Math.Sin(phi1) * Math.Cos(phi2) * Math.Cos(dlambda)
        Dim brng As Double = RadiansToDegrees(Math.Atan2(x, y))
        Return (brng + 360.0) Mod 360.0
    End Function

    Private Shared Function ProjectFrom(lat As Double, lon As Double, bearingDeg As Double, distanceMeters As Double) As Tuple(Of Double, Double)
        Const R As Double = 6371000.0
        Dim angularDistance As Double = distanceMeters / R
        Dim bearingRad As Double = DegreesToRadians(bearingDeg)

        Dim latRad As Double = DegreesToRadians(lat)
        Dim lonRad As Double = DegreesToRadians(lon)

        Dim destLatRad As Double = Math.Asin(Math.Sin(latRad) * Math.Cos(angularDistance) +
                                             Math.Cos(latRad) * Math.Sin(angularDistance) * Math.Cos(bearingRad))
        Dim destLonRad As Double = lonRad + Math.Atan2(Math.Sin(bearingRad) * Math.Sin(angularDistance) * Math.Cos(latRad),
                                                       Math.Cos(angularDistance) - Math.Sin(latRad) * Math.Sin(destLatRad))

        Dim destLat As Double = RadiansToDegrees(destLatRad)
        Dim destLon As Double = (RadiansToDegrees(destLonRad) + 540.0) Mod 360.0 - 180.0

        Return Tuple.Create(destLat, destLon)
    End Function

    Private Shared Function DegreesToRadians(value As Double) As Double
        Return value * Math.PI / 180.0
    End Function

    Private Shared Function RadiansToDegrees(value As Double) As Double
        Return value * 180.0 / Math.PI
    End Function

    Private Shared Function MovingAverage(series As List(Of Double), window As Integer) As List(Of Double)
        Dim half As Integer = window \ 2
        Dim n As Integer = series.Count
        Dim output As New List(Of Double)(n)

        For i As Integer = 0 To n - 1
            Dim s As Double = 0.0
            Dim c As Integer = 0
            For j As Integer = i - half To i + half
                If j >= 0 AndAlso j < n Then
                    s += series(j)
                    c += 1
                End If
            Next
            If c > 0 Then
                output.Add(s / c)
            Else
                output.Add(series(i))
            End If
        Next

        Return output
    End Function

    Private Shared Function PitchTrimFromSpeedAndFlaps(tasKts As Double?, flapIndex As Integer?) As Double
        Dim speed As Double = If(tasKts.HasValue, tasKts.Value, 70.0)
        Dim baseVal As Double

        If speed <= 40 Then
            baseVal = 6.0
        ElseIf speed <= 60 Then
            baseVal = 4.0
        ElseIf speed <= 80 Then
            baseVal = 2.0
        ElseIf speed <= 100 Then
            baseVal = 1.0
        ElseIf speed <= 120 Then
            baseVal = 0.0
        Else
            baseVal = -1.5
        End If

        If flapIndex.HasValue Then
            If flapIndex.Value <= -1 Then
                baseVal -= 2.0
            ElseIf flapIndex.Value >= 4 Then
                baseVal += 2.0
            ElseIf flapIndex.Value >= 2 Then
                baseVal += 1.0
            End If
        End If

        Return baseVal
    End Function

    Private Shared Function CruisePitchOffset(tasKts As Double?) As Double
        If Not tasKts.HasValue Then Return 0.0

        If tasKts.Value <= 70 Then
            Return 0.0
        ElseIf tasKts.Value <= 90 Then
            Return -0.5
        ElseIf tasKts.Value <= 110 Then
            Return -1.5
        ElseIf tasKts.Value <= 130 Then
            Return -2.5
        Else
            Return -3.5
        End If
    End Function

    Private Shared Function LandingFlare(aglM As Double?, climbRateMs As Double) As Double
        If Not aglM.HasValue OrElse aglM.Value > 20.0 Then Return 0.0
        If climbRateMs >= 0.0 Then Return 0.0

        Dim factor As Double = 1.0 - (aglM.Value / 20.0)
        factor = Clamp(factor, 0.0, 1.0)

        Dim vsMag As Double = Math.Min(Math.Abs(climbRateMs), 3.0)
        Dim baseFlare As Double = 2.0 + (vsMag / 3.0) * 2.0

        Return baseFlare * factor
    End Function

    Private Shared Function DetectTakeoffRun(igcRecords As List(Of IgcRecord), vGround As List(Of Double)) As RunSegmentInfo
        If igcRecords Is Nothing OrElse vGround Is Nothing OrElse igcRecords.Count <> vGround.Count Then
            Return Nothing
        End If

        Dim startIdx As Integer = -1
        Dim endIdx As Integer = -1
        Dim minRollSpeed As Double = 4.0

        For i As Integer = 0 To igcRecords.Count - 1
            Dim rec As IgcRecord = igcRecords(i)

            Dim isOnGround As Boolean = False
            If rec.OnGroundFlag.HasValue Then
                isOnGround = (rec.OnGroundFlag.Value <> 0)
            ElseIf rec.AltAglM.HasValue AndAlso rec.AltAglM.Value <= 1 Then
                isOnGround = True
            End If

            Dim speedG As Double = vGround(i)

            If isOnGround AndAlso speedG >= minRollSpeed Then
                If startIdx = -1 Then
                    startIdx = i
                End If
                endIdx = i
            ElseIf endIdx <> -1 Then
                Exit For
            End If
        Next

        If startIdx >= 0 AndAlso endIdx > startIdx Then
            Return New RunSegmentInfo With {
                .StartTime = igcRecords(startIdx).TimeSec,
                .EndTime = igcRecords(endIdx).TimeSec,
                .StartLat = igcRecords(startIdx).Lat,
                .StartLon = igcRecords(startIdx).Lon,
                .EndLat = igcRecords(endIdx).Lat,
                .EndLon = igcRecords(endIdx).Lon
            }
        End If

        Return Nothing
    End Function

    Private Shared Function BuildFltRecData(igcRecords As List(Of IgcRecord), aircraftTitle As String, absoluteTimeBase As Double) As FltRecData
        Dim n As Integer = igcRecords.Count
        If n < 2 Then Throw New InvalidOperationException("Not enough IGC records.")

        Const ENABLE_DEBUG_OUTPUT As Boolean = False

        Dim vGroundList As New List(Of Double)()
        Dim derived As New List(Of Tuple(Of Double, Double, Double))()
        Dim trackHeading As New List(Of Double)()
        Dim noseHeading As New List(Of Double)()
        Dim sumLat As Double = 0.0
        Dim sumLon As Double = 0.0
        For i As Integer = 0 To n - 1
            Dim rec As IgcRecord = igcRecords(i)
            Dim t As Integer = rec.TimeSec
            Dim lat As Double = rec.Lat
            Dim lon As Double = rec.Lon
            Dim alt As Integer = rec.AltM

            sumLat += lat
            sumLon += lon

            Dim vGround As Double
            Dim head As Double
            Dim climb As Double

            If i = 0 Then
                Dim nxt As IgcRecord = igcRecords(i + 1)
                Dim rawDelta As Integer = nxt.TimeSec - t
                Dim dt As Double = If(rawDelta = 0, 1.0, Math.Max(0.001, rawDelta))
                Dim dist As Double = Haversine(lat, lon, nxt.Lat, nxt.Lon)
                vGround = dist / dt
                head = Bearing(lat, lon, nxt.Lat, nxt.Lon)
                climb = (nxt.AltM - alt) / dt
            Else
                Dim prv As IgcRecord = igcRecords(i - 1)
                Dim rawDelta As Integer = t - prv.TimeSec
                Dim dt As Double = If(rawDelta = 0, 1.0, Math.Max(0.001, rawDelta))
                Dim dist As Double = Haversine(prv.Lat, prv.Lon, lat, lon)
                vGround = dist / dt
                head = Bearing(prv.Lat, prv.Lon, lat, lon)
                climb = (alt - prv.AltM) / dt
            End If

            Dim vAir As Double
            If rec.TasKts.HasValue Then
                vAir = Math.Max(5.0, rec.TasKts.Value * KT_TO_MS)
            Else
                vAir = Math.Max(5.0, vGround)
            End If

            Dim correctedHeading As Double = head

            If rec.WindSpeedMs.HasValue AndAlso rec.WindSpeedMs.Value > 0.1 AndAlso rec.WindDirDeg.HasValue AndAlso vGround > 0.5 Then

                ' Ground vector Vg
                Dim trackRad As Double = DegreesToRadians(head)
                Dim vgNorth As Double = vGround * Math.Cos(trackRad)
                Dim vgEast As Double = vGround * Math.Sin(trackRad)

                ' Convert wind FROM → TO
                Dim windToDeg As Double = (rec.WindDirDeg.Value + 180.0) Mod 360.0
                Dim windToRad As Double = DegreesToRadians(windToDeg)
                Dim vwNorth As Double = rec.WindSpeedMs.Value * Math.Cos(windToRad)
                Dim vwEast As Double = rec.WindSpeedMs.Value * Math.Sin(windToRad)

                ' Air vector Va = Vg - Vw
                Dim vaNorth As Double = vgNorth - vwNorth
                Dim vaEast As Double = vgEast - vwEast
                Dim vaMag As Double = Math.Sqrt(vaNorth * vaNorth + vaEast * vaEast)

                If vaMag > 0.5 Then
                    correctedHeading = (RadiansToDegrees(Math.Atan2(vaEast, vaNorth)) + 360.0) Mod 360.0
                End If
            End If

            trackHeading.Add(head)
            noseHeading.Add(correctedHeading)
            vGroundList.Add(vGround)
            derived.Add(Tuple.Create(vGround, vAir, climb))
        Next

        Dim continuousTrack As New List(Of Double)()
        Dim prevTrack As Double = trackHeading(0)
        continuousTrack.Add(prevTrack)
        For i As Integer = 1 To trackHeading.Count - 1
            Dim delta As Double = (trackHeading(i) - prevTrack + 540.0) Mod 360.0 - 180.0
            prevTrack += delta
            continuousTrack.Add(prevTrack)
        Next

        ' Use a more responsive heading only for bank modelling
        Dim bankTrackUnwrapped As List(Of Double) = MovingAverage(continuousTrack, 3)

        Dim smoothTrackUnwrapped As List(Of Double) = MovingAverage(continuousTrack, 9)
        Dim smoothTrackHeading As New List(Of Double)()
        For Each h As Double In smoothTrackUnwrapped
            Dim wrapped As Double = (h Mod 360.0 + 360.0) Mod 360.0
            smoothTrackHeading.Add(wrapped)
        Next

        Dim continuousNose As New List(Of Double)()
        Dim prevNose As Double = noseHeading(0)
        continuousNose.Add(prevNose)
        For i As Integer = 1 To noseHeading.Count - 1
            Dim delta As Double = (noseHeading(i) - prevNose + 540.0) Mod 360.0 - 180.0
            prevNose += delta
            continuousNose.Add(prevNose)
        Next

        Dim smoothNoseUnwrapped As List(Of Double) = MovingAverage(continuousNose, 9)
        Dim smoothNoseHeading As New List(Of Double)()
        For Each h As Double In smoothNoseUnwrapped
            Dim wrapped As Double = (h Mod 360.0 + 360.0) Mod 360.0
            smoothNoseHeading.Add(wrapped)
        Next

        Dim takeoffRun As RunSegmentInfo = DetectTakeoffRun(igcRecords, vGroundList)

        Dim g As Double = 9.81
        Dim rawBank As New List(Of Double)()
        Dim rawPitch As New List(Of Double)()

        For i As Integer = 0 To n - 1
            Dim rec As IgcRecord = igcRecords(i)
            Dim k As Tuple(Of Double, Double, Double) = derived(i)
            Dim vGround As Double = k.Item1
            Dim vAir As Double = k.Item2
            Dim climb As Double = k.Item3

            Dim idx0 As Integer
            Dim idx1 As Integer
            If i = 0 Then
                idx0 = i
                idx1 = Math.Min(i + 2, n - 1)
            ElseIf i = n - 1 Then
                idx0 = Math.Max(0, i - 2)
                idx1 = i
            Else
                idx0 = Math.Max(0, i - 1)
                idx1 = Math.Min(n - 1, i + 1)
            End If

            Dim segmentStart As Integer = igcRecords(idx0).TimeSec
            Dim segmentEnd As Integer = igcRecords(idx1).TimeSec
            Dim rawDelta As Integer = segmentEnd - segmentStart
            Dim dt As Double = If(rawDelta = 0, 1.0, Math.Max(0.001, rawDelta))
            Dim psi0 As Double = DegreesToRadians(bankTrackUnwrapped(idx0))
            Dim psi1 As Double = DegreesToRadians(bankTrackUnwrapped(idx1))
            Dim dpsi As Double = psi1 - psi0
            Dim w As Double = dpsi / dt

            Dim V As Double = Math.Max(15.0, vAir)
            Dim latAcc As Double = V * w
            Dim bankDeg As Double
            If Math.Abs(latAcc) < 0.01 Then
                bankDeg = 0.0
            Else
                Dim bank As Double = Math.Atan(latAcc / g)
                bankDeg = RadiansToDegrees(bank)
            End If
            rawBank.Add(bankDeg)

            Dim tasKts As Double? = rec.TasKts
            Dim netto As Double = If(rec.NettoMs.HasValue, rec.NettoMs.Value, 0.0)
            Dim flapIndex As Integer? = rec.FlapIndex
            Dim aglM As Integer? = rec.AltAglM

            Dim gammaFp As Double = RadiansToDegrees(Math.Atan2(climb, V))
            Dim trimPitch As Double = PitchTrimFromSpeedAndFlaps(tasKts, flapIndex)
            Dim cruiseOffset As Double = CruisePitchOffset(tasKts)
            Dim liftFactor As Double = Clamp(netto / 3.0, 0.0, 1.0)
            Dim basePitch As Double = ((1.0 - liftFactor) * (gammaFp + cruiseOffset + 2.0)) + (liftFactor * trimPitch)
            Dim flare As Double = LandingFlare(If(aglM.HasValue, CDbl(aglM.Value), Nothing), climb)
            Dim pitchDeg As Double = basePitch + flare

            rawPitch.Add(pitchDeg)

            ' Refresh derived kinematic values with the latest smoothing output
            derived(i) = Tuple.Create(vGround, vAir, climb)
        Next

        ' Slightly less aggressive smoothing, earlier switch to short window,
        ' and allow full 90° bank.
        Dim bankLong As List(Of Double) = MovingAverage(rawBank, 7)
        Dim bankShort As List(Of Double) = MovingAverage(rawBank, 3)

        Dim finalBank As New List(Of Double)()
        For i As Integer = 0 To rawBank.Count - 1
            Dim raw As Double = rawBank(i)
            Dim bLong As Double = bankLong(i)
            Dim bShort As Double = bankShort(i)
            Dim mag As Double = Math.Abs(raw)

            ' Switch from long to short smoothing a bit earlier
            Dim wgt As Double = Math.Min(1.0, mag / 20.0)
            Dim b As Double = (1 - wgt) * bLong + wgt * bShort

            ' Optional: keep this deadband, or shrink it if you want more micro-movement
            If Math.Abs(b) < 0.2 Then b = 0.0

            ' Shallow-turn boost: you can keep or slightly reduce the factor
            Dim shallowMag As Double = Math.Abs(b)
            If shallowMag > 0 AndAlso shallowMag < 12.0 Then
                Dim boost As Double = 1.0 + ((12.0 - shallowMag) * 0.02)
                b *= boost
            End If

            ' Let it go to full knife-edge
            b = Clamp(b, -90.0, 90.0)
            finalBank.Add(-b)
        Next

        Dim pitchSmooth As List(Of Double) = MovingAverage(rawPitch, PITCH_SMOOTH_WINDOW)
        Dim finalPitch As New List(Of Double)()
        For Each p As Double In pitchSmooth
            Dim clamped As Double = Clamp(p, PITCH_MAX_DOWN, PITCH_MAX_UP)
            finalPitch.Add(-clamped)  ' keep MSFS axis convention
        Next

        Dim avgLat As Double = If(n > 0, sumLat / n, 0.0)
        Dim avgLon As Double = If(n > 0, sumLon / n, 0.0)
        Dim magVarDeg As Double = GetMagneticVariationDegrees(avgLat, avgLon)

        ' === Phase 5: build frames (mostly unchanged) =========================
        Dim records As New List(Of FltRecRecord)()
        Dim t0 As Integer = igcRecords(0).TimeSec

        For i As Integer = 0 To igcRecords.Count - 1
            Dim rec As IgcRecord = igcRecords(i)
            Dim kin As Tuple(Of Double, Double, Double) = derived(i)
            Dim bankDeg As Double = finalBank(i)
            Dim pitchDeg As Double = finalPitch(i)

            Dim t As Integer = rec.TimeSec
            Dim lat As Double = rec.Lat
            Dim lon As Double = rec.Lon
            Dim altM As Integer = rec.AltM
            Dim altAglM As Integer? = rec.AltAglM
            Dim vGround As Double = kin.Item1
            Dim vAir As Double = kin.Item2
            Dim head As Double = smoothNoseHeading(i)
            Dim climb As Double = kin.Item3

            ' --- Straighten the takeoff roll: force a straight line between
            '     the first rolling point and the liftoff point.
            If takeoffRun IsNot Nothing Then
                Dim takeoffT0 As Double = takeoffRun.StartTime
                Dim takeoffT1 As Double = takeoffRun.EndTime

                If t >= takeoffT0 AndAlso t <= takeoffT1 Then
                    Dim denom As Double = takeoffT1 - takeoffT0
                    Dim fracRun As Double = 0.0

                    If denom > 0.1 Then
                        fracRun = Clamp((t - takeoffT0) / denom, 0.0, 1.0)
                    End If

                    lat = Lerp(takeoffRun.StartLat, takeoffRun.EndLat, fracRun)
                    lon = Lerp(takeoffRun.StartLon, takeoffRun.EndLon, fracRun)
                End If
            End If

            Dim timeMs As Integer = CInt((t - t0) * 1000)
            Dim tasMs As Double = vAir
            Dim tasKts As Double = tasMs / KT_TO_MS

            Dim pos As New FltRecPosition()
            pos.Milliseconds = 0
            pos.Latitude = lat
            pos.Longitude = lon
            pos.Altitude = CDbl(altM) * M_TO_FT
            pos.AltitudeAboveGround = If(altAglM.HasValue,
                             CDbl(altAglM.Value) * M_TO_FT,
                             0.0)
            pos.Pitch = pitchDeg
            pos.Bank = bankDeg
            pos.TrueHeading = head
            pos.MagneticHeading = head
            pos.GyroHeading = head
            pos.TrueAirspeed = tasKts
            pos.IndicatedAirspeed = tasKts
            pos.GpsGroundSpeed = vGround / KT_TO_MS
            pos.GroundSpeed = pos.GpsGroundSpeed
            pos.MachAirspeed = tasMs / 340.0
            Dim speedFtPerSec As Double = tasKts * KT_TO_MS * M_TO_FT
            pos.AbsoluteTime = absoluteTimeBase + (timeMs / 1000.0R)
            pos.VelocityBodyX = 0.0R
            pos.VelocityBodyY = 0.0R
            pos.VelocityBodyZ = speedFtPerSec
            pos.HeadingIndicator = head
            pos.AIPitch = pitchDeg
            pos.AIBank = bankDeg

            Dim onGroundFlag As Integer? = rec.OnGroundFlag
            If Not onGroundFlag.HasValue Then
                If altAglM.HasValue AndAlso altAglM.Value < 2.0 AndAlso pos.GroundSpeed < 10.0 Then
                    onGroundFlag = 1
                Else
                    onGroundFlag = 0
                End If
            End If
            pos.IsOnGround = onGroundFlag.Value

            ' Map IGC FLP
            Dim flapIndex As Integer
            If rec.FlapIndex.HasValue Then
                flapIndex = rec.FlapIndex.Value - 1   ' shift down by 1
            Else
                flapIndex = 0
            End If
            ' Clamp to sensible range
            If flapIndex < 0 Then flapIndex = 0

            pos.FlapsHandleIndex = flapIndex

            Dim gearFlag As Integer = If(rec.GearFlag.HasValue, rec.GearFlag.Value, If(pos.IsOnGround = 1, 1, 0))
            pos.GearHandlePosition = gearFlag

            pos.WindVelocity = If(rec.WindSpeedMs.HasValue, rec.WindSpeedMs.Value, 0.0)
            pos.WindDirection = If(rec.WindDirDeg.HasValue, rec.WindDirDeg.Value, 0.0)

            Dim thr As Double
            If rec.EnlRaw.HasValue Then
                thr = Clamp(rec.EnlRaw.Value / 900.0, 0.0, 1.0)
            Else
                thr = 0.0
            End If
            pos.ThrottleLeverPosition1 = thr

            Dim nLoad As Double = 1.0 / Math.Max(0.2, Math.Cos(DegreesToRadians(bankDeg)))
            Dim extra As Double = Math.Max(0.0, nLoad - 1.0)
            Dim flex As Double = Clamp(extra * 0.6, 0.0, 0.6)
            pos.WingFlexPercent1 = flex
            pos.WingFlexPercent2 = flex
            pos.WingFlexPercent3 = flex
            pos.WingFlexPercent4 = flex

            Dim frame As New FltRecRecord()
            frame.Time = timeMs
            frame.Position = pos
            records.Add(frame)
        Next

        ' --- Takeoff window and heading: use the SAME run that we used to
        '     straighten lat/lon (takeoffRun).
        Dim takeoffHeading As Double = Double.NaN
        Dim takeoffStartMs As Integer = Integer.MinValue
        Dim takeoffEndMs As Integer = Integer.MinValue
        If takeoffRun IsNot Nothing Then
            takeoffHeading = Bearing(
                takeoffRun.StartLat, takeoffRun.StartLon,
                takeoffRun.EndLat, takeoffRun.EndLon
            )
            takeoffStartMs = CInt(Math.Round((takeoffRun.StartTime - t0) * 1000.0))
            takeoffEndMs = CInt(Math.Round((takeoffRun.EndTime - t0) * 1000.0))
        End If

        ' --- Final heading assignments (takeoff roll handled above) ---
        For i As Integer = 0 To records.Count - 1
            Dim frame As FltRecRecord = records(i)
            Dim pos As FltRecPosition = frame.Position
            Dim h As Double
            Dim trueNoseHeading As Double = smoothNoseHeading(i)

            Dim inTakeoffRun As Boolean = (Not Double.IsNaN(takeoffHeading) AndAlso
                                           frame.Time >= takeoffStartMs AndAlso
                                           frame.Time <= takeoffEndMs AndAlso
                                           pos.IsOnGround = 1)

            If inTakeoffRun Then
                ' Takeoff roll: constant heading from runway track
                h = takeoffHeading
                pos.Bank = 0.0
                pos.AIBank = 0.0
            ElseIf pos.IsOnGround = 1 Then
                ' Other ground frames (e.g. taxi): follow local track
                Dim hasPrev As Boolean = (i > 0 AndAlso records(i - 1).Position.IsOnGround = 1)
                Dim hasNext As Boolean = (i < records.Count - 1 AndAlso records(i + 1).Position.IsOnGround = 1)
                Dim groundTrackHeading As Double

                If hasPrev Then
                    Dim prevPos As FltRecPosition = records(i - 1).Position
                    groundTrackHeading = Bearing(prevPos.Latitude, prevPos.Longitude,
                                                 pos.Latitude, pos.Longitude)
                ElseIf hasNext Then
                    Dim nextPos As FltRecPosition = records(i + 1).Position
                    groundTrackHeading = Bearing(pos.Latitude, pos.Longitude,
                                                 nextPos.Latitude, nextPos.Longitude)
                Else
                    groundTrackHeading = smoothTrackHeading(i)
                End If

                h = groundTrackHeading
                pos.Bank = 0.0
                pos.AIBank = 0.0
            Else
                ' In the air: keep the smoothed heading
                h = trueNoseHeading
            End If

            Dim magHeading As Double = (h - magVarDeg) Mod 360.0
            If magHeading < 0 Then magHeading += 360.0

            pos.TrueHeading = h
            pos.MagneticHeading = magHeading
            pos.GyroHeading = pos.MagneticHeading
            pos.HeadingIndicator = pos.MagneticHeading
        Next

        records = InterpolateRecords(records)

        If ENABLE_DEBUG_OUTPUT Then
            Console.WriteLine("TimeMs, TrueHead, MagHead, IsOnGround, Lat, Lon")
            Dim airbornePrinted As Integer = 0
            For i As Integer = 0 To Math.Min(49, records.Count - 1)
                Dim pos As FltRecPosition = records(i).Position
                Console.WriteLine(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                                                 "{0},{1:F3},{2:F3},{3},{4:F7},{5:F7}",
                                                 records(i).Time, pos.TrueHeading,
                                                 pos.MagneticHeading, pos.IsOnGround,
                                                 pos.Latitude, pos.Longitude))
            Next

            Console.WriteLine("Airborne samples (first 20): TimeMs, TrueHead, MagHead, IsOnGround, Lat, Lon")
            For i As Integer = 0 To records.Count - 1
                Dim pos As FltRecPosition = records(i).Position
                If pos Is Nothing Then Continue For
                If pos.IsOnGround = 0 Then
                    Console.WriteLine(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                                                     "{0},{1:F3},{2:F3},{3},{4:F7},{5:F7}",
                                                     records(i).Time, pos.TrueHeading,
                                                     pos.MagneticHeading, pos.IsOnGround,
                                                     pos.Latitude, pos.Longitude))
                    airbornePrinted += 1
                    If airbornePrinted >= 20 Then Exit For
                End If
            Next
        End If

        Dim startState As New StartState()
        startState.PlaneInParkingState = 0
        startState.AircraftTitle = aircraftTitle
        startState.AircraftAirline = String.Empty
        startState.AircraftNumber = String.Empty
        startState.AircraftId = "ASXGS"
        startState.AircraftModel = "GenericGlider"
        startState.AircraftType = "GLIDER"
        startState.AircraftOnParkingSpot = 0

        Dim data As New FltRecData()
        data.ClientVersion = CLIENT_VERSION
        data.StartTime = records(0).Time
        data.EndTime = records(records.Count - 1).Time
        data.StartState = startState
        data.Records = records

        If ENABLE_DEBUG_OUTPUT Then
            DebugDumpTakeoffRun(takeoffRun, records, takeoffStartMs, takeoffEndMs)
        End If

        Return data
    End Function


    Private Shared Function InterpolateRecords(records As List(Of FltRecRecord)) As List(Of FltRecRecord)
        If records Is Nothing OrElse records.Count = 0 Then Return records

        Const TARGET_DT As Double = 0.25
        Dim output As New List(Of FltRecRecord)()

        For i As Integer = 0 To records.Count - 1
            Dim a As FltRecRecord = records(i)
            output.Add(a)

            If i = records.Count - 1 Then Exit For

            Dim b As FltRecRecord = records(i + 1)
            Dim dtSeconds As Double = (b.Time - a.Time) / 1000.0
            If dtSeconds < 0.01 Then Continue For

            Dim dist As Double = Haversine(a.Position.Latitude, a.Position.Longitude, b.Position.Latitude, b.Position.Longitude)
            If dist < 0.01 Then Continue For

            Dim nSteps As Integer = CInt(Math.Floor(dtSeconds / TARGET_DT))
            If nSteps < 1 Then nSteps = 1

            Dim trackBearing As Double = Bearing(a.Position.Latitude, a.Position.Longitude, b.Position.Latitude, b.Position.Longitude)
            For k As Integer = 1 To nSteps
                Dim u As Double = k / (nSteps + 1)
                Dim interp As New FltRecRecord()
                interp.Time = CInt(Math.Round(a.Time + (u * (b.Time - a.Time))))

                Dim interpPos As FltRecPosition = InterpolatePosition(a.Position, b.Position, u)
                Dim projected As Tuple(Of Double, Double) = ProjectFrom(a.Position.Latitude, a.Position.Longitude, trackBearing, dist * u)
                interpPos.Latitude = projected.Item1
                interpPos.Longitude = projected.Item2

                interp.Position = interpPos
                output.Add(interp)
            Next
        Next

        Return output
    End Function

    Private Shared Function InterpolatePosition(a As FltRecPosition, b As FltRecPosition, t As Double) As FltRecPosition
        Dim pos As New FltRecPosition()
        pos.Milliseconds = 0
        pos.Latitude = Lerp(a.Latitude, b.Latitude, t)
        pos.Longitude = Lerp(a.Longitude, b.Longitude, t)
        pos.Altitude = Lerp(a.Altitude, b.Altitude, t)
        pos.AltitudeAboveGround = Lerp(a.AltitudeAboveGround, b.AltitudeAboveGround, t)
        pos.Pitch = Lerp(a.Pitch, b.Pitch, t)
        pos.Bank = Lerp(a.Bank, b.Bank, t)
        pos.TrueHeading = LerpAngleDegrees(a.TrueHeading, b.TrueHeading, t)
        pos.MagneticHeading = LerpAngleDegrees(a.MagneticHeading, b.MagneticHeading, t)
        pos.GyroHeading = pos.MagneticHeading
        pos.TrueAirspeed = Lerp(a.TrueAirspeed, b.TrueAirspeed, t)
        pos.IndicatedAirspeed = Lerp(a.IndicatedAirspeed, b.IndicatedAirspeed, t)
        pos.GpsGroundSpeed = Lerp(a.GpsGroundSpeed, b.GpsGroundSpeed, t)
        pos.GroundSpeed = pos.GpsGroundSpeed
        pos.MachAirspeed = Lerp(a.MachAirspeed, b.MachAirspeed, t)
        pos.AbsoluteTime = Lerp(a.AbsoluteTime, b.AbsoluteTime, t)
        pos.VelocityBodyX = Lerp(a.VelocityBodyX, b.VelocityBodyX, t)
        pos.VelocityBodyY = Lerp(a.VelocityBodyY, b.VelocityBodyY, t)
        pos.VelocityBodyZ = Lerp(a.VelocityBodyZ, b.VelocityBodyZ, t)
        pos.HeadingIndicator = LerpAngleDegrees(a.HeadingIndicator, b.HeadingIndicator, t)
        pos.AIPitch = Lerp(a.AIPitch, b.AIPitch, t)
        pos.AIBank = Lerp(a.AIBank, b.AIBank, t)
        pos.IsOnGround = CInt(Math.Round(Lerp(a.IsOnGround, b.IsOnGround, t)))
        pos.FlapsHandleIndex = CInt(Math.Round(Lerp(a.FlapsHandleIndex, b.FlapsHandleIndex, t)))
        pos.GearHandlePosition = CInt(Math.Round(Lerp(a.GearHandlePosition, b.GearHandlePosition, t)))
        pos.WindVelocity = Lerp(a.WindVelocity, b.WindVelocity, t)
        pos.WindDirection = LerpAngleDegrees(a.WindDirection, b.WindDirection, t)
        pos.ThrottleLeverPosition1 = Lerp(a.ThrottleLeverPosition1, b.ThrottleLeverPosition1, t)
        pos.WingFlexPercent1 = Lerp(a.WingFlexPercent1, b.WingFlexPercent1, t)
        pos.WingFlexPercent2 = Lerp(a.WingFlexPercent2, b.WingFlexPercent2, t)
        pos.WingFlexPercent3 = Lerp(a.WingFlexPercent3, b.WingFlexPercent3, t)
        pos.WingFlexPercent4 = Lerp(a.WingFlexPercent4, b.WingFlexPercent4, t)

        Return pos
    End Function


    Private Shared Function GetMagneticVariationDegrees(lat As Double, lon As Double) As Double
        Return MagVarGrid.GetDeclination(lat, lon)
    End Function

    Private Class MagVarGridFile
        Public Property minLat As Double
        Public Property maxLat As Double
        Public Property minLon As Double
        Public Property maxLon As Double
        Public Property stepLat As Double
        Public Property stepLon As Double
        Public Property referenceYear As Double
        Public Property grid As Double()()
    End Class

    Private NotInheritable Class MagVarGrid
        Private Shared ReadOnly MinLat As Double
        Private Shared ReadOnly MaxLat As Double
        Private Shared ReadOnly MinLon As Double
        Private Shared ReadOnly MaxLon As Double
        Private Shared ReadOnly StepLat As Double
        Private Shared ReadOnly StepLon As Double
        Private Shared ReadOnly ReferenceYear As Double
        Private Shared ReadOnly DeclGrid As Double()()
        Private Shared ReadOnly RowCount As Integer
        Private Shared ReadOnly ColCount As Integer

        Shared Sub New()
            Dim basePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MagneticDeclinationGrid.json")
            Dim gridPath As String = If(File.Exists(basePath), basePath,
                                        Path.Combine(Directory.GetCurrentDirectory(), "MagneticDeclinationGrid.json"))

            If Not File.Exists(gridPath) Then
                Throw New FileNotFoundException("Magnetic declination grid file not found.", gridPath)
            End If

            Dim json As String = File.ReadAllText(gridPath)
            Dim gridData As MagVarGridFile = JsonConvert.DeserializeObject(Of MagVarGridFile)(json)

            If gridData Is Nothing OrElse gridData.grid Is Nothing OrElse gridData.grid.Length = 0 Then
                Throw New InvalidDataException("Magnetic declination grid data is missing or invalid.")
            End If

            MinLat = gridData.minLat
            MaxLat = gridData.maxLat
            MinLon = gridData.minLon
            MaxLon = gridData.maxLon
            StepLat = gridData.stepLat
            StepLon = gridData.stepLon
            ReferenceYear = gridData.referenceYear
            DeclGrid = gridData.grid
            RowCount = DeclGrid.Length

            If DeclGrid(0) Is Nothing OrElse DeclGrid(0).Length = 0 Then
                Throw New InvalidDataException("Magnetic declination grid rows must not be empty.")
            End If

            ColCount = DeclGrid(0).Length

            If StepLat <= 0 OrElse StepLon <= 0 Then
                Throw New InvalidDataException("Magnetic declination grid step sizes must be positive.")
            End If
        End Sub

        Public Shared Function GetDeclination(lat As Double, lon As Double) As Double
            Dim clampedLat As Double = Math.Max(MinLat, Math.Min(MaxLat, lat))
            Dim lonSpan As Double = MaxLon - MinLon
            Dim wrappedLon As Double = ((lon - MinLon) Mod lonSpan)
            If wrappedLon < 0 Then wrappedLon += lonSpan
            Dim adjustedLon As Double = MinLon + wrappedLon

            Dim latIndex As Double = (clampedLat - MinLat) / StepLat
            Dim lonIndex As Double = (adjustedLon - MinLon) / StepLon

            Dim i0 As Integer = CInt(Math.Floor(latIndex))
            Dim j0 As Integer = CInt(Math.Floor(lonIndex))
            Dim i1 As Integer = Math.Min(i0 + 1, RowCount - 1)
            Dim j1 As Integer = (j0 + 1) Mod ColCount

            Dim tLat As Double = latIndex - i0
            Dim tLon As Double = lonIndex - j0

            Dim q11 As Double = DeclGrid(i0)(j0)
            Dim q21 As Double = DeclGrid(i1)(j0)
            Dim q12 As Double = DeclGrid(i0)(j1)
            Dim q22 As Double = DeclGrid(i1)(j1)

            Dim qLat0 As Double = q11 * (1 - tLat) + q21 * tLat
            Dim qLat1 As Double = q12 * (1 - tLat) + q22 * tLat
            Dim q As Double = qLat0 * (1 - tLon) + qLat1 * tLon

            Return q
        End Function
    End Class

    Private Shared Sub DebugDumpTakeoffRun(takeoffRun As RunSegmentInfo,
                                           records As List(Of FltRecRecord),
                                           takeoffStartMs As Integer,
                                           takeoffEndMs As Integer)
        If takeoffRun Is Nothing Then Return

        Dim expectedHeading As Double = Bearing(takeoffRun.StartLat, takeoffRun.StartLon,
                                                takeoffRun.EndLat, takeoffRun.EndLon)

        Console.WriteLine("TimeMs, Lat, Lon, TrueHeading, ExpHeading, HeadingErrDeg, TrackErrDeg")

        For Each frame As FltRecRecord In records
            If frame.Time < takeoffStartMs OrElse frame.Time > takeoffEndMs Then Continue For
            If frame.Position Is Nothing OrElse frame.Position.IsOnGround <> 1 Then Continue For

            Dim pos As FltRecPosition = frame.Position
            Dim bearingFromStart As Double = Bearing(takeoffRun.StartLat, takeoffRun.StartLon,
                                                     pos.Latitude, pos.Longitude)
            Dim headingError As Double = NormalizeAngle(pos.TrueHeading - expectedHeading)
            Dim trackError As Double = NormalizeAngle(bearingFromStart - expectedHeading)

            Console.WriteLine(String.Format(System.Globalization.CultureInfo.InvariantCulture,
                                             "{0},{1:F7},{2:F7},{3:F3},{4:F3},{5:F4},{6:F4}",
                                             frame.Time, pos.Latitude, pos.Longitude,
                                             pos.TrueHeading, expectedHeading,
                                             headingError, trackError))
        Next
    End Sub

    Private Shared Sub WriteFltRec(data As FltRecData, outPath As String)
        Dim jsonBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))

        If File.Exists(outPath) Then
            File.Delete(outPath)
        End If

        Using archive As ZipArchive = ZipFile.Open(outPath, ZipArchiveMode.Create)
            Dim entry As ZipArchiveEntry = archive.CreateEntry("data.json")
            Using entryStream As Stream = entry.Open()
                entryStream.Write(jsonBytes, 0, jsonBytes.Length)
            End Using
        End Using
    End Sub

End Class
