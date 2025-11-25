Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
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

        Dim igcData As IgcParseResult = ParseIgc(igcPath)
        Dim aircraftTitle As String = If(String.IsNullOrWhiteSpace(igcData.SuggestedAircraftTitle), "AS 33 Me (18m)", igcData.SuggestedAircraftTitle)

        Dim data As FltRecData = BuildFltRecData(igcData.Records, aircraftTitle)
        WriteFltRec(data, fltRecPath)
    End Sub

    Private Class IgcParseResult
        Public Property Records As List(Of IgcRecord)
        Public Property SuggestedAircraftTitle As String
    End Class

    Private Shared Function Clamp(x As Double, lo As Double, hi As Double) As Double
        Return Math.Max(lo, Math.Min(hi, x))
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

    Private Shared Function DegreesToRadians(value As Double) As Double
        Return value * Math.PI / 180.0
    End Function

    Private Shared Function RadiansToDegrees(value As Double) As Double
        Return value * 180.0 / Math.PI
    End Function

    Private Shared Function Lerp(a As Double, b As Double, t As Double) As Double
        Return a + (b - a) * t
    End Function

    Private Shared Function LerpNullableInt(a As Integer?, b As Integer?, t As Double) As Integer?
        If a.HasValue AndAlso b.HasValue Then
            Return CInt(Math.Round(Lerp(a.Value, b.Value, t)))
        End If

        If a.HasValue Then Return a
        If b.HasValue Then Return b
        Return Nothing
    End Function

    Private Shared Function LerpNullableDouble(a As Double?, b As Double?, t As Double) As Double?
        If a.HasValue AndAlso b.HasValue Then
            Return Lerp(a.Value, b.Value, t)
        End If

        If a.HasValue Then Return a
        If b.HasValue Then Return b
        Return Nothing
    End Function

    Private Shared Function InterpolateHeading(h0 As Double, h1 As Double, t As Double) As Double
        Dim delta As Double = ((h1 - h0 + 540.0) Mod 360.0) - 180.0
        Return (h0 + delta * t + 360.0) Mod 360.0
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

    Private Shared Function BuildFltRecData(igcRecords As List(Of IgcRecord), aircraftTitle As String) As FltRecData
        Dim n As Integer = igcRecords.Count
        If n < 2 Then Throw New InvalidOperationException("Not enough IGC records.")

        Dim derived As New List(Of Tuple(Of Double, Double, Double, Double))()
        For i As Integer = 0 To n - 1
            Dim rec As IgcRecord = igcRecords(i)
            Dim t As Integer = rec.TimeSec
            Dim lat As Double = rec.Lat
            Dim lon As Double = rec.Lon
            Dim alt As Integer = rec.AltM

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

            derived.Add(Tuple.Create(vGround, vAir, head, climb))
        Next

        Dim g As Double = 9.81
        Dim rawBank As New List(Of Double)()
        Dim rawPitch As New List(Of Double)()

        For i As Integer = 0 To n - 1
            Dim rec As IgcRecord = igcRecords(i)
            Dim k As Tuple(Of Double, Double, Double, Double) = derived(i)
            Dim vGround As Double = k.Item1
            Dim vAir As Double = k.Item2
            Dim head As Double = k.Item3
            Dim climb As Double = k.Item4

            ' --- Compute heading with wind correction (Va = Vg - Vw) ---
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
            ' --- End of heading correction block ---

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
            Dim h0 As Double = derived(idx0).Item3
            Dim h1 As Double = derived(idx1).Item3
            Dim dpsi As Double = DegreesToRadians(((h1 - h0 + 540.0) Mod 360.0) - 180.0)
            Dim w As Double = dpsi / dt

            Dim V As Double = Math.Max(15.0, vAir)
            Dim latAcc As Double = V * w
            Dim bankDeg As Double
            If Math.Abs(latAcc) < 0.05 Then
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

            ' Replace original heading value
            derived(i) = Tuple.Create(vGround, vAir, correctedHeading, climb)
        Next

        Dim bankLong As List(Of Double) = MovingAverage(rawBank, 15)
        Dim bankShort As List(Of Double) = MovingAverage(rawBank, 5)
        Dim finalBank As New List(Of Double)()
        For i As Integer = 0 To rawBank.Count - 1
            Dim raw As Double = rawBank(i)
            Dim bLong As Double = bankLong(i)
            Dim bShort As Double = bankShort(i)
            Dim mag As Double = Math.Abs(raw)
            Dim wgt As Double = Math.Min(1.0, mag / 30.0)
            Dim b As Double = (1 - wgt) * bLong + wgt * bShort
            If Math.Abs(b) < 2.0 Then b = 0.0
            b = Clamp(b, -45.0, 45.0)
            finalBank.Add(-b)
        Next

        Dim pitchSmooth As List(Of Double) = MovingAverage(rawPitch, 9)
        Dim finalPitch As New List(Of Double)()
        For Each p As Double In pitchSmooth
            Dim clamped As Double = Clamp(p, -10.0, 20.0)
            finalPitch.Add(-clamped)
        Next

        Dim records As New List(Of FltRecRecord)()
        Dim t0 As Integer = igcRecords(0).TimeSec

        Dim createFrame As Func(Of Integer, Double, FltRecRecord) = Function(idx As Integer, frac As Double) As FltRecRecord
            Dim nextIdx As Integer = Math.Min(idx + 1, igcRecords.Count - 1)
            Dim tInterp As Double = Clamp(frac, 0.0, 1.0)

            Dim recA As IgcRecord = igcRecords(idx)
            Dim recB As IgcRecord = igcRecords(nextIdx)
            Dim kinA As Tuple(Of Double, Double, Double, Double) = derived(idx)
            Dim kinB As Tuple(Of Double, Double, Double, Double) = derived(nextIdx)

            Dim bankA As Double = finalBank(idx)
            Dim bankB As Double = finalBank(nextIdx)
            Dim pitchA As Double = finalPitch(idx)
            Dim pitchB As Double = finalPitch(nextIdx)

            Dim t As Double = If(nextIdx = idx, recA.TimeSec, Lerp(recA.TimeSec, recB.TimeSec, tInterp))
            Dim lat As Double = If(nextIdx = idx, recA.Lat, Lerp(recA.Lat, recB.Lat, tInterp))
            Dim lon As Double = If(nextIdx = idx, recA.Lon, Lerp(recA.Lon, recB.Lon, tInterp))
            Dim altM As Double = If(nextIdx = idx, recA.AltM, Lerp(recA.AltM, recB.AltM, tInterp))
            Dim altAglM As Double? = LerpNullableDouble(If(recA.AltAglM.HasValue, CDbl(recA.AltAglM.Value), Nothing),
                                                       If(recB.AltAglM.HasValue, CDbl(recB.AltAglM.Value), Nothing),
                                                       tInterp)

            Dim vGround As Double = If(nextIdx = idx, kinA.Item1, Lerp(kinA.Item1, kinB.Item1, tInterp))
            Dim vAir As Double = If(nextIdx = idx, kinA.Item2, Lerp(kinA.Item2, kinB.Item2, tInterp))
            Dim head As Double = If(nextIdx = idx, kinA.Item3, InterpolateHeading(kinA.Item3, kinB.Item3, tInterp))

            Dim timeMs As Integer = CInt(Math.Round((t - t0) * 1000))
            Dim tasMs As Double = vAir
            Dim tasKts As Double = tasMs / KT_TO_MS

            Dim pos As New FltRecPosition()
            pos.Milliseconds = 0
            pos.Latitude = lat
            pos.Longitude = lon
            pos.Altitude = altM * M_TO_FT
            pos.AltitudeAboveGround = If(altAglM.HasValue,
                             altAglM.Value * M_TO_FT,
                             0.0)

            Dim bankDeg As Double = If(nextIdx = idx, bankA, Lerp(bankA, bankB, tInterp))
            Dim pitchDeg As Double = If(nextIdx = idx, pitchA, Lerp(pitchA, pitchB, tInterp))

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
            pos.HeadingIndicator = head
            pos.AIPitch = pitchDeg
            pos.AIBank = bankDeg

            Dim onGroundFlag As Integer? = LerpNullableInt(recA.OnGroundFlag, recB.OnGroundFlag, tInterp)
            If Not onGroundFlag.HasValue Then
                If altAglM.HasValue AndAlso altAglM.Value < 2.0 AndAlso pos.GroundSpeed < 10.0 Then
                    onGroundFlag = 1
                Else
                    onGroundFlag = 0
                End If
            End If
            pos.IsOnGround = onGroundFlag.Value

            Dim flapIndexRaw As Integer? = LerpNullableInt(recA.FlapIndex, recB.FlapIndex, tInterp)
            Dim flapIndex As Integer = If(flapIndexRaw.HasValue, flapIndexRaw.Value - 1, 0)
            If flapIndex < 0 Then flapIndex = 0
            pos.FlapsHandleIndex = flapIndex

            Dim gearRaw As Integer? = LerpNullableInt(recA.GearFlag, recB.GearFlag, tInterp)
            Dim gearFlag As Integer = If(gearRaw.HasValue, gearRaw.Value, If(pos.IsOnGround = 1, 1, 0))
            pos.GearHandlePosition = gearFlag

            Dim windSpeedVal As Double? = LerpNullableDouble(recA.WindSpeedMs, recB.WindSpeedMs, tInterp)
            pos.WindVelocity = If(windSpeedVal.HasValue, windSpeedVal.Value, 0.0)

            Dim windDir As Double? = Nothing
            If recA.WindDirDeg.HasValue AndAlso recB.WindDirDeg.HasValue Then
                windDir = InterpolateHeading(recA.WindDirDeg.Value, recB.WindDirDeg.Value, tInterp)
            ElseIf recA.WindDirDeg.HasValue Then
                windDir = recA.WindDirDeg.Value
            ElseIf recB.WindDirDeg.HasValue Then
                windDir = recB.WindDirDeg.Value
            End If
            pos.WindDirection = If(windDir.HasValue, windDir.Value, 0.0)

            Dim thr As Double = 0.0
            Dim enlValue As Double? = LerpNullableDouble(If(recA.EnlRaw.HasValue, CDbl(recA.EnlRaw.Value), Nothing),
                                                         If(recB.EnlRaw.HasValue, CDbl(recB.EnlRaw.Value), Nothing),
                                                         tInterp)
            If enlValue.HasValue Then
                thr = Clamp(enlValue.Value / 900.0, 0.0, 1.0)
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
            Return frame
        End Function

        For i As Integer = 0 To igcRecords.Count - 1
            records.Add(createFrame(i, 0.0))
            If i < igcRecords.Count - 1 Then
                records.Add(createFrame(i, 0.5))
            End If
        Next

        Dim heads As New List(Of Double)()
        For Each r As FltRecRecord In records
            heads.Add(r.Position.TrueHeading)
        Next

        Dim unwrapped As New List(Of Double)()
        Dim prev As Double = heads(0)
        unwrapped.Add(prev)
        For i As Integer = 1 To heads.Count - 1
            Dim h As Double = heads(i)
            Dim delta As Double = (h - prev + 540.0) Mod 360.0 - 180.0
            prev = prev + delta
            unwrapped.Add(prev)
        Next

        Dim smoothUnwrapped As List(Of Double) = MovingAverage(unwrapped, 9)
        Dim smoothHeads As New List(Of Double)()
        For Each h As Double In smoothUnwrapped
            smoothHeads.Add(h Mod 360.0)
        Next

        For i As Integer = 0 To records.Count - 1
            Dim h As Double = smoothHeads(i)
            Dim pos As FltRecPosition = records(i).Position
            pos.TrueHeading = h
            pos.MagneticHeading = h
            pos.GyroHeading = h
            pos.HeadingIndicator = h
        Next

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

        Return data
    End Function

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
