Imports System.IO
Imports System.Text.Json
Imports System.Text.RegularExpressions

Module IgcParser

    ' ————————————————————————————————
    ' Glider-Type Title-Strings (only these are needed to normalize)
    ' ————————————————————————————————
    Private ReadOnly B21_GliderTitleMap As New Dictionary(Of String, String()) From {
      {"AS-33", {"as33", "as-33"}},
      {"ASW28", {"asw28", "asw-28"}},
      {"JS3-18", {"js3-18"}},
      {"JS3-15", {"js3-15"}},
      {"LS4", {"ls4"}},
      {"DG808S", {"dg808"}},
      {"AS7", {"as7", " k7", "k7 "}},
      {"D2C", {"d2c", "discus"}},
      {"Asobo_LS8", {"ls8", "mxs"}},
      {"DGF", {"dgf", "dg1001"}},
      {"T31", {"t31"}},
      {"SZD30", {"szd30", "szd-30", "pirat", "yanosik"}},
      {"S12G", {"stemme", "s12g"}},
      {"ASK21", {"ask21", "ask-21", "k21"}},
      {"Taurus", {"taurus", "pipistrel"}}
    }

    Private Function FindGliderType(titleStr As String) As String
        Dim lc = titleStr.ToLowerInvariant()
        For Each kvp In B21_GliderTitleMap
            For Each subStr In kvp.Value
                If lc.Contains(subStr) Then
                    Return kvp.Key
                End If
            Next
        Next
        Return ""
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
            Dim parts = raw.Split(";"c, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) s.Trim()).ToArray()
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

    ' ————————————————————————————————
    ' Main entry: parse the file and return a JsonDocument
    ' ————————————————————————————————
    Public Function ParseIgcFile(igcFilePath As String) As JsonDocument
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
        Dim norm = FindGliderType(gliderType)
        If norm <> "" Then gliderType = norm

        ' 2) C-lines: header + waypoints
        ' Declare headerData as a nullable tuple of the correct fields
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
        If headerData Is Nothing Then
            Return Nothing
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
            Dim m = Regex.Match(L, "LDAT\s+\d{8}\s+(\d{8})")
            If m.Success Then
                localDateRaw = m.Groups(1).Value
            End If
        Next
        Dim localDate = If(localDateRaw <> "",
          $"{localDateRaw.Substring(0, 4)}-{localDateRaw.Substring(4, 2)}-{localDateRaw.Substring(6, 2)}",
          "")

        ' 4) B-record for BeginTimeUTC
        Dim beginTimeUTC = ParseBRecord(lines)

        ' 5) Build IGCRecordDateTimeUTC = YYMMDDHHMMSS
        Dim ddmmyy = headerData.Value.utcDate
        Dim hhmmss = headerData.Value.utcTime
        Dim keyDT = ddmmyy.Substring(4, 2) & ddmmyy.Substring(2, 2) & ddmmyy.Substring(0, 2) & hhmmss

        ' 6) Assemble into an anonymous object
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
          .LocalTime = headerData.Value.localTime,
          .BeginTimeUTC = beginTimeUTC
        }

        ' 7) Serialize & parse so caller gets a JsonDocument
        Dim json = JsonSerializer.Serialize(payload, New JsonSerializerOptions With {.WriteIndented = False})
        Return JsonDocument.Parse(json)
    End Function

End Module
