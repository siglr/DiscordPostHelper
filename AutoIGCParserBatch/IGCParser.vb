Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions

Module IgcParser

    Public Function FindGliderType(titleStr As String, competitionClass As String) As String
        Dim _ = competitionClass ' kept for compatibility, currently unused by database-driven rules
        Return GliderMatcher.MatchGliderKey(titleStr)
    End Function

    ' ————————————————————————————————
    ' JS → VB parsers
    ' ————————————————————————————————
    Private Function ParseALine(line As String) As (nb21Version As String, sim As String)
        Dim parts = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
        Dim nb21 = ""
        Dim sim = ""
        If parts.Length >= 3 AndAlso parts(2).Equals("logger", StringComparison.OrdinalIgnoreCase) Then
            If parts.Length >= 4 Then nb21 = parts(3)
            sim = "MSFS 2020"
        ElseIf parts.Length >= 3 Then
            nb21 = parts(2)
            sim = If(parts.Length >= 4, String.Join(" ", parts, 3, parts.Length - 3), "MSFS 2020")
        End If
        Return (nb21, sim)
    End Function

    Private Function ParseHFLine(line As String) As (key As String, value As String)?
        Dim idx = line.IndexOf(":"c)
        If idx < 0 Then Return Nothing
        Dim key = line.Substring(0, idx).Trim()
        Dim value = line.Substring(idx + 1).Trim()
        Return (key, value)
    End Function

    Private Function ParseHeader(line As String) As (utcDate As String, utcTime As String, localTime As String, flightId As String, numWaypoints As String, taskTitle As String)
        Dim m = Regex.Match(line, "^C(\d{6})(\d{6})(\d{6})(\d{4})(\d{2})(.*)$")
        If Not m.Success Then Throw New Exception("Invalid header C-line: " & line)
        Return (
          utcDate:=m.Groups(1).Value,
          utcTime:=m.Groups(2).Value,
          localTime:=m.Groups(3).Value,
          flightId:=m.Groups(4).Value,
          numWaypoints:=m.Groups(5).Value,
          taskTitle:=m.Groups(6).Value.Trim()
        )
    End Function

    Private Function FormatCoordinate(deg As String, min As String, thou As String, hem As String) As String
        Dim d = Integer.Parse(deg)
        Dim mn = Integer.Parse(min)
        Dim th = Integer.Parse(thou)
        Dim sec = (th / 1000.0) * 60
        Return $"{hem}{d}° {mn}' {sec:F2}"""
    End Function

    Private Function ParseWaypoint(line As String) As (originalId As String, latitude As String, longitude As String)?
        ' /^C(\d{2})(\d{2})(\d{3})([NS])(\d{3})(\d{2})(\d{3})([EW])(.*)$/
        Dim m = Regex.Match(line, "^C(\d{2})(\d{2})(\d{3})([NS])(\d{3})(\d{2})(\d{3})([EW])(.*)$")
        If Not m.Success Then Return Nothing
        Dim lat = FormatCoordinate(m.Groups(1).Value, m.Groups(2).Value, m.Groups(3).Value, m.Groups(4).Value)
        Dim lon = FormatCoordinate(m.Groups(5).Value, m.Groups(6).Value, m.Groups(7).Value, m.Groups(8).Value)
        Dim raw = m.Groups(9).Value.Trim()
        Dim orig As String
        If raw.EndsWith(";"c) Then
            orig = raw
        Else
            Dim parts = raw.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) s.Trim()) _
                .ToArray()
            If parts.Length = 3 Then
                orig = parts(2)
            ElseIf parts.Length = 2 Then
                orig = parts(1)
            Else
                orig = raw
            End If
        End If
        Return (orig, lat, lon)
    End Function

    Private Function ParseBRecord(lines As String()) As String
        For Each L In lines
            If L.StartsWith("B"c) Then
                Return If(L.Length >= 7, L.Substring(1, 6), "")
            End If
        Next
        Return ""
    End Function

    Private Function TryGetFirstCRecordUtc(lines As IEnumerable(Of String)) As DateTime?
        Dim c = lines.FirstOrDefault(Function(l) l.StartsWith("C"c))
        If String.IsNullOrEmpty(c) OrElse c.Length < 13 Then Return Nothing
        Try
            Dim dd = Integer.Parse(c.Substring(1, 2))
            Dim mm = Integer.Parse(c.Substring(3, 2))
            Dim yy = 2000 + Integer.Parse(c.Substring(5, 2))
            Dim hh = Integer.Parse(c.Substring(7, 2))
            Dim mi = Integer.Parse(c.Substring(9, 2))
            Dim ss = Integer.Parse(c.Substring(11, 2))
            Return New DateTime(yy, mm, dd, hh, mi, ss, DateTimeKind.Utc)
        Catch
            Return Nothing
        End Try
    End Function

    Private Function TryGetNB21Time(lines As IEnumerable(Of String), keyword As String) As TimeSpan?
        ' Accept "LNB21 HHMMSS KEYWORD" or "LNB21 HH:MM:SS KEYWORD"
        Dim rx As New Regex($"^LNB21\s+(\d{{6}}|\d{{2}}:\d{{2}}:\d{{2}})\s+{Regex.Escape(keyword)}\b",
                        RegexOptions.IgnoreCase)
        For Each l In lines
            Dim m = rx.Match(l)
            If m.Success Then
                Dim t = m.Groups(1).Value
                Try
                    If t.Contains(":"c) Then
                        Dim p = t.Split(":"c)
                        Return New TimeSpan(Integer.Parse(p(0)), Integer.Parse(p(1)), Integer.Parse(p(2)))
                    Else
                        Return New TimeSpan(Integer.Parse(t.Substring(0, 2)),
                                        Integer.Parse(t.Substring(2, 2)),
                                        Integer.Parse(t.Substring(4, 2)))
                    End If
                Catch
                    ' ignore and keep scanning
                End Try
            End If
        Next
        Return Nothing
    End Function

    Private Function TryGetFirstNb21TimeAfterHfdte(lines As IEnumerable(Of String)) As TimeSpan?
        Dim sawHfdte = False
        Dim rx As New Regex("^LNB21\s+(\d{6}|\d{2}:\d{2}:\d{2})\b", RegexOptions.IgnoreCase)

        For Each l In lines
            If Not sawHfdte Then
                If l.StartsWith("HFDTE") Then sawHfdte = True
                Continue For
            End If

            Dim m = rx.Match(l)
            If m.Success Then
                Dim t = m.Groups(1).Value
                Try
                    If t.Contains(":"c) Then
                        Dim p = t.Split(":"c)
                        Return New TimeSpan(Integer.Parse(p(0)), Integer.Parse(p(1)), Integer.Parse(p(2)))
                    Else
                        Return New TimeSpan(Integer.Parse(t.Substring(0, 2)),
                                            Integer.Parse(t.Substring(2, 2)),
                                            Integer.Parse(t.Substring(4, 2)))
                    End If
                Catch
                    ' ignore malformed NB21 time and keep scanning
                End Try
            End If
        Next

        Return Nothing
    End Function


    Private Function TryGetHfdteDateUtc(lines As IEnumerable(Of String)) As DateTime?
        Dim h = lines.FirstOrDefault(Function(l) l.StartsWith("HFDTE"))
        If String.IsNullOrEmpty(h) OrElse h.Length < 11 Then Return Nothing
        Try
            Dim dd = Integer.Parse(h.Substring(5, 2))
            Dim mm = Integer.Parse(h.Substring(7, 2))
            Dim yy = 2000 + Integer.Parse(h.Substring(9, 2))
            Return New DateTime(yy, mm, dd, 0, 0, 0, DateTimeKind.Utc)
        Catch
            Return Nothing
        End Try
    End Function

    Private Function TryGetFirstBRecordTime(lines As IEnumerable(Of String)) As TimeSpan?
        For Each l In lines
            If l.StartsWith("B"c) AndAlso l.Length >= 7 Then
                Try
                    Return New TimeSpan(Integer.Parse(l.Substring(1, 2)),
                                        Integer.Parse(l.Substring(3, 2)),
                                        Integer.Parse(l.Substring(5, 2)))
                Catch
                    ' ignore malformed B record and keep scanning
                End Try
            End If
        Next
        Return Nothing
    End Function

    Private Function ComputeRecordedUtcFromToff(lines As IEnumerable(Of String)) As DateTime?
        Dim toff = TryGetNB21Time(lines, "TOFF") ' NB21 takeoff TOD comes from the LNB21 prefix time
        Dim firstB = TryGetFirstBRecordTime(lines)
        Dim hfdteDate = TryGetHfdteDateUtc(lines)

        ' NB21 anchor rule: date comes from HFDTE, launch TOD comes from TOFF (or first B fallback).
        ' C-record and local-time fields are not used here because they can be stale or simulator-local.
        If hfdteDate.HasValue Then
            Dim launchTod = If(toff, firstB)
            If launchTod.HasValue Then
                Dim candidate = hfdteDate.Value.Add(launchTod.Value)
                Dim sessionStartTod = TryGetFirstNb21TimeAfterHfdte(lines)

                ' If session starts before midnight and TOFF is just after midnight, roll to next UTC day.
                If sessionStartTod.HasValue AndAlso launchTod.Value < sessionStartTod.Value Then
                    candidate = candidate.AddDays(1)
                End If

                Return DateTime.SpecifyKind(candidate, DateTimeKind.Utc)
            End If
        End If

        ' Keep non-HFDTE fallback behavior for older/non-NB21 files.
        Dim cUtc = TryGetFirstCRecordUtc(lines)
        If cUtc.HasValue AndAlso toff.HasValue Then
            Return DateTime.SpecifyKind(cUtc.Value.Date.Add(toff.Value), DateTimeKind.Utc)
        End If

        Return Nothing
    End Function

    ' ————————————————————————————————
    ' Main entry: parse the file and return a JsonDocument
    ' ————————————————————————————————
    Public Function ParseIgcFile(igcFilePath As String) As JToken
        Dim lines = File.ReadAllLines(igcFilePath)

        ' 1) preamble: AXXX + HF…
        Dim pilot = "", gliderID = "", competitionID = "", competitionClass = "", gliderType = ""
        Dim nb21Version = "", sim = ""
        For Each L In lines
            If L.StartsWith("C"c) Then Exit For
            If L.StartsWith("AXXX") Then
                Dim x = ParseALine(L)
                nb21Version = x.nb21Version
                sim = x.sim
            ElseIf L.StartsWith("HF") Then
                Dim hf = ParseHFLine(L)
                If hf.HasValue Then
                    Select Case hf.Value.key
                        Case "HFPLTPILOTINCHARGE" : pilot = hf.Value.value
                        Case "HFGIDGLIDERID" : gliderID = hf.Value.value
                        Case "HFCIDCOMPETITIONID" : competitionID = hf.Value.value
                        Case "HFCCLCOMPETITIONCLASS" : competitionClass = hf.Value.value
                        Case "HFGTYGLIDERTYPE" : gliderType = hf.Value.value
                    End Select
                End If
            End If
        Next

        ' normalize glider type
        Dim norm = FindGliderType(gliderType, competitionClass)
        If norm <> "" Then gliderType = norm

        ' 2) Try to read a real C-header + waypoints
        Dim headerData As (
            utcDate As String,
            utcTime As String,
            localTime As String,
            flightId As String,
            numWaypoints As String,
            taskTitle As String
        )? = Nothing
        Dim waypoints As New List(Of (id As String, coord As String))()
        For Each L In lines
            If Not L.StartsWith("C"c) Then Continue For
            If headerData Is Nothing Then
                headerData = ParseHeader(L)
                Continue For
            End If
            Dim wp = ParseWaypoint(L)
            If wp.HasValue Then
                waypoints.Add((wp.Value.originalId, $"{wp.Value.latitude},{wp.Value.longitude}"))
            End If
        Next

        ' 2b) If no C-header, synthesize one from HFDTE + B + LTIM
        If headerData Is Nothing Then
            Dim fallbackDate As DateTime = Date.UtcNow
            Dim hLine = lines.FirstOrDefault(Function(l) l.StartsWith("HFDTE"))
            If hLine IsNot Nothing Then
                Dim m = Regex.Match(hLine, "^HFDTE(\d{2})(\d{2})(\d{2})$")
                If m.Success Then
                    Dim d = Integer.Parse(m.Groups(1).Value)
                    Dim mo = Integer.Parse(m.Groups(2).Value)
                    Dim yy = 2000 + Integer.Parse(m.Groups(3).Value)
                    fallbackDate = New DateTime(yy, mo, d, 0, 0, 0, DateTimeKind.Utc)
                End If
            End If

            Dim bLine = lines.FirstOrDefault(Function(l) l.StartsWith("B"))
            If bLine IsNot Nothing AndAlso bLine.Length >= 7 Then
                Dim hh = Integer.Parse(bLine.Substring(1, 2))
                Dim mm = Integer.Parse(bLine.Substring(3, 2))
                Dim ss = Integer.Parse(bLine.Substring(5, 2))
                fallbackDate = New DateTime(fallbackDate.Year, fallbackDate.Month, fallbackDate.Day, hh, mm, ss, DateTimeKind.Utc)
            End If

            Dim localTimeF = ""
            Dim lt = lines.FirstOrDefault(Function(l) l.Contains("LTIM"))
            If lt IsNot Nothing Then
                Dim mm2 = Regex.Match(lt, "LTIM\s+\d{6}\s+(\d{6})")
                If mm2.Success Then localTimeF = mm2.Groups(1).Value
            End If

            Dim ddmmyy = fallbackDate.ToString("ddMMyy", Globalization.CultureInfo.InvariantCulture)
            Dim hhmmss = fallbackDate.ToString("HHmmss", Globalization.CultureInfo.InvariantCulture)

            headerData = (
                utcDate:=ddmmyy,
                utcTime:=hhmmss,
                localTime:=If(localTimeF <> "", localTimeF, hhmmss),
                flightId:="",
                numWaypoints:="0",
                taskTitle:=""
            )
            waypoints.Clear()
        End If

        ' 3) LDAT → LocalDate
        Dim localDateRaw = ""
        Dim bCount = 0
        For Each L In lines
            If L.StartsWith("B"c) Then
                bCount += 1
                If bCount = 2 Then Exit For
                Continue For
            End If
            Dim m2 = Regex.Match(L, "LDAT\s+\d{8}\s+(\d{8})")
            If m2.Success Then localDateRaw = m2.Groups(1).Value
        Next
        Dim localDate = If(localDateRaw <> "",
            $"{localDateRaw.Substring(0, 4)}-{localDateRaw.Substring(4, 2)}-{localDateRaw.Substring(6, 2)}",
            "")

        ' 4) B-record for BeginTimeUTC
        Dim beginTimeUTC = ParseBRecord(lines)

        ' 5) Build IGCRecordDateTimeUTC from NB21 TOFF with midnight rollover against C-record
        Dim recordedUtc As DateTime? = ComputeRecordedUtcFromToff(lines)
        Dim keyDT As String
        If recordedUtc.HasValue Then
            keyDT = recordedUtc.Value.ToString("yyMMddHHmmss", Globalization.CultureInfo.InvariantCulture)
        Else
            ' Fallback to existing C-header timestamp (old behavior)
            Dim dd2 = headerData.Value.utcDate
            Dim tt2 = headerData.Value.utcTime
            keyDT = dd2.Substring(4, 2) & dd2.Substring(2, 2) & dd2.Substring(0, 2) & tt2
        End If

        ' 6) LocalTime fallback if "000000"
        Dim localTime = headerData.Value.localTime
        If localTime = "000000" Then
            Dim ltimLine = lines.FirstOrDefault(Function(l) l.Contains("LTIM"))
            If ltimLine IsNot Nothing Then
                Dim m3 = Regex.Match(ltimLine, "LTIM\s+\d{6}\s+(\d{6})")
                If m3.Success Then localTime = m3.Groups(1).Value
            End If
        End If

        ' 7) Assemble into payload
        Dim payload = New With {
            .igcTitle = headerData.Value.taskTitle,
            .igcWaypoints = waypoints.Select(Function(w) New With {w.id, w.coord}).ToList(),
            .pilot = pilot,
            .gliderID = gliderID,
            .competitionID = competitionID,
            .competitionClass = competitionClass,
            .gliderType = gliderType,
            .NB21Version = nb21Version,
            .Sim = sim,
            .IGCRecordDateTimeUTC = keyDT,
            .IGCUploadDateTimeUTC = Date.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            .LocalDate = localDate,
            .LocalTime = localTime,
            .BeginTimeUTC = beginTimeUTC
        }

        ' 8) Serialize + return as JToken
        Return JToken.FromObject(payload)
    End Function

End Module
