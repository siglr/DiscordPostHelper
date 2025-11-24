Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Xml

''' <summary>
''' Converts an IGC flight log file into a KML file with:
''' - A LineString Placemark for the recorded track
''' - A LineString Placemark for the task route (from C-records)
''' - A Point Placemark for each task waypoint
''' </summary>
Public Class IgcToKmlConverter

    ''' <summary>
    ''' Simple container for a route waypoint.
    ''' </summary>
    Private Class RouteWaypoint
        Public Property Latitude As Double
        Public Property Longitude As Double
        Public Property Name As String
    End Class

    ''' <summary>
    ''' Convert an IGC file on disk into a KML file on disk.
    ''' </summary>
    ''' <param name="igcPath">Path to the source IGC file.</param>
    ''' <param name="kmlPath">Path where the KML file will be written.</param>
    Public Shared Sub Convert(igcPath As String, kmlPath As String)
        If igcPath Is Nothing Then Throw New ArgumentNullException(NameOf(igcPath))
        If kmlPath Is Nothing Then Throw New ArgumentNullException(NameOf(kmlPath))

        If Not File.Exists(igcPath) Then
            Throw New FileNotFoundException("IGC file not found.", igcPath)
        End If

        Dim converter As New IgcToKmlConverter()
        converter.ConvertInternal(igcPath, kmlPath)
    End Sub

    ' --- Internal state extracted from the IGC file ---

    Private _flightDateUtc As DateTime?
    Private _pilot As String
    Private _gliderId As String
    Private _gliderType As String

    Private _routeTitle As String
    Private _routeWaypoints As New List(Of RouteWaypoint)()

    Private Sub ConvertInternal(igcPath As String, kmlPath As String)
        Dim headerLines As New List(Of String)()
        Dim fixLines As New List(Of String)()
        Dim cLines As New List(Of String)()

        For Each line As String In File.ReadLines(igcPath, Encoding.UTF8)
            If String.IsNullOrWhiteSpace(line) Then Continue For
            Dim recordType As Char = line(0)
            Select Case recordType
                Case "H"c
                    headerLines.Add(line)
                Case "B"c
                    fixLines.Add(line)
                Case "C"c
                    cLines.Add(line)
                Case Else
                    ' ignore
            End Select
        Next

        ParseHeaders(headerLines)
        ParseCRoutes(cLines)

        Dim settings As New XmlWriterSettings() With {
            .Encoding = New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False),
            .Indent = True,
            .IndentChars = "  ",
            .NewLineChars = vbLf,
            .NewLineHandling = NewLineHandling.Replace
        }

        Using fs As New FileStream(kmlPath, FileMode.Create, FileAccess.Write, FileShare.None)
            Using xw As XmlWriter = XmlWriter.Create(fs, settings)
                Dim defaultName As String = Path.GetFileNameWithoutExtension(igcPath)
                WriteKmlDocument(xw, fixLines, defaultName)
            End Using
        End Using
    End Sub

    ' ---------------- H RECORDS (HEADERS) ----------------

    ''' <summary>
    ''' Parse useful information from H-records: date, pilot, glider type, glider ID.
    ''' </summary>
    Private Sub ParseHeaders(headerLines As IEnumerable(Of String))
        For Each line In headerLines
            ' Date: HFDTEddmmyy
            If line.StartsWith("HFDTE", StringComparison.OrdinalIgnoreCase) AndAlso line.Length >= 11 Then
                Dim dd As Integer
                Dim mm As Integer
                Dim yy As Integer
                If Integer.TryParse(line.Substring(5, 2), dd) AndAlso
                   Integer.TryParse(line.Substring(7, 2), mm) AndAlso
                   Integer.TryParse(line.Substring(9, 2), yy) Then

                    Dim year As Integer = 2000 + yy
                    Try
                        _flightDateUtc = New DateTime(year, mm, dd, 0, 0, 0, DateTimeKind.Utc)
                    Catch
                        ' Ignore invalid dates
                    End Try
                End If

            ElseIf line.StartsWith("HFPLTPILOTINCHARGE", StringComparison.OrdinalIgnoreCase) OrElse
                   line.StartsWith("HFPLTPILOT", StringComparison.OrdinalIgnoreCase) Then
                _pilot = ExtractHeaderValue(line)

            ElseIf line.StartsWith("HFGTYGLIDERTYPE", StringComparison.OrdinalIgnoreCase) Then
                _gliderType = ExtractHeaderValue(line)

            ElseIf line.StartsWith("HFGIDGLIDERID", StringComparison.OrdinalIgnoreCase) Then
                _gliderId = ExtractHeaderValue(line)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Extracts the value after the first ':' in an H record, trimming whitespace.
    ''' </summary>
    Private Shared Function ExtractHeaderValue(line As String) As String
        Dim idx As Integer = line.IndexOf(":"c)
        If idx >= 0 AndAlso idx + 1 < line.Length Then
            Return line.Substring(idx + 1).Trim()
        End If
        Return String.Empty
    End Function

    ' ---------------- C RECORDS (TASK / WAYPOINTS) ----------------

    ''' <summary>
    ''' Parse C-records: first line = task title, others = waypoints.
    ''' Example:
    ''' C231125125102130116000106Glider-ISR-North to South 408  (title)
    ''' C3312965N03535762ELLKS;3;Kiryat-Shmona                  (waypoint)
    ''' C3311650N03532796E*Start+2559|3000x5000                 (waypoint)
    ''' </summary>
    Private Sub ParseCRoutes(cLines As List(Of String))
        If cLines Is Nothing OrElse cLines.Count = 0 Then
            Return
        End If

        ' First C-line -> route title
        Dim first As String = cLines(0)
        _routeTitle = ParseCTaskTitle(first)

        ' Remaining C-lines -> waypoints
        For i As Integer = 1 To cLines.Count - 1
            Dim line As String = cLines(i)
            Dim wp As RouteWaypoint = ParseCWaypoint(line)
            If wp IsNot Nothing Then
                _routeWaypoints.Add(wp)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Extracts the task title from the first C-line
    ''' by skipping the leading digits after 'C' and taking the first non-digit onwards.
    ''' </summary>
    Private Shared Function ParseCTaskTitle(line As String) As String
        If String.IsNullOrWhiteSpace(line) OrElse line.Length < 2 Then
            Return String.Empty
        End If

        ' Find first non-digit after the initial 'C'
        Dim idx As Integer = -1
        For i As Integer = 1 To line.Length - 1
            Dim ch As Char = line(i)
            If Not Char.IsDigit(ch) Then
                idx = i
                Exit For
            End If
        Next

        If idx = -1 OrElse idx >= line.Length Then
            Return String.Empty
        End If

        Return line.Substring(idx).Trim()
    End Function

    ''' <summary>
    ''' Parse a C waypoint line into a RouteWaypoint:
    ''' C DDMMmmmH DDDMMmmmH NAME...
    ''' Example: C3312965N03535762ELLKS;3;Kiryat-Shmona
    '''          C3311650N03532796E*Start+2559|3000x5000
    ''' </summary>
    Private Shared Function ParseCWaypoint(line As String) As RouteWaypoint
        If String.IsNullOrWhiteSpace(line) Then Return Nothing
        If line(0) <> "C"c Then Return Nothing

        ' Basic length check: 1 + 8 + 9 = 18 chars minimum before name
        If line.Length < 19 Then Return Nothing

        Dim latStr As String = line.Substring(1, 8)
        Dim lonStr As String = line.Substring(9, 9)
        Dim rawName As String = line.Substring(18).Trim()

        Dim lat As Double
        Dim lon As Double
        If Not TryParseIgcLatitude(latStr, lat) Then Return Nothing
        If Not TryParseIgcLongitude(lonStr, lon) Then Return Nothing

        Dim name As String = CleanWaypointName(rawName)
        If String.IsNullOrEmpty(name) Then
            name = "WP"
        End If

        Dim wp As New RouteWaypoint()
        wp.Latitude = lat
        wp.Longitude = lon
        wp.Name = name
        Return wp
    End Function

    ''' <summary>
    ''' Clean the waypoint name:
    ''' - Strip everything after the first '+' (nav computer extras)
    ''' - If ';' present, take the last segment (e.g. "LLKS;3;Kiryat-Shmona" -> "Kiryat-Shmona")
    ''' - Strip leading '*' (e.g. "*Start" -> "Start")
    ''' </summary>
    Private Shared Function CleanWaypointName(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return String.Empty
        Dim name As String = raw.Trim()

        ' Remove anything after '+'
        Dim plusIdx As Integer = name.IndexOf("+"c)
        If plusIdx >= 0 Then
            name = name.Substring(0, plusIdx).Trim()
        End If

        ' If we still have ';', typically last segment is human-readable
        If name.IndexOf(";"c) >= 0 Then
            Dim parts As String() = name.Split(";"c)
            If parts.Length > 0 Then
                name = parts(parts.Length - 1).Trim()
            End If
        End If

        ' Strip leading '*' used for Start/Finish markers
        If name.StartsWith("*") Then
            name = name.Substring(1).Trim()
        End If

        Return name
    End Function

    ' ---------------- KML WRITING ----------------

    ''' <summary>
    ''' Writes the full KML document from the collected B- and C-records.
    ''' </summary>
    Private Sub WriteKmlDocument(xw As XmlWriter, fixLines As List(Of String), defaultName As String)
        Dim ns As String = "http://www.opengis.net/kml/2.2"

        Dim trackName As String = defaultName
        If Not String.IsNullOrWhiteSpace(_routeTitle) Then
            trackName = _routeTitle
        End If
        If Not String.IsNullOrWhiteSpace(_gliderId) Then
            trackName = _gliderId & " - " & trackName
        End If

        xw.WriteStartDocument()
        xw.WriteStartElement("kml", ns)
        xw.WriteStartElement("Document", ns)

        xw.WriteElementString("name", ns, trackName)

        Dim descBuilder As New StringBuilder()
        If Not String.IsNullOrWhiteSpace(_pilot) Then
            descBuilder.Append("Pilot: ").Append(_pilot)
        End If
        If Not String.IsNullOrWhiteSpace(_gliderType) Then
            If descBuilder.Length > 0 Then descBuilder.Append("; ")
            descBuilder.Append("Glider: ").Append(_gliderType)
        End If
        If _flightDateUtc.HasValue Then
            If descBuilder.Length > 0 Then descBuilder.Append("; ")
            descBuilder.Append("Date: ").Append(_flightDateUtc.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
        End If

        If descBuilder.Length > 0 Then
            xw.WriteElementString("description", ns, descBuilder.ToString())
        End If

        WriteTrackPlacemark(xw, fixLines, trackName, ns)

        If _routeWaypoints.Count > 0 Then
            WriteRoutePlacemark(xw, trackName, ns)
            WriteWaypointFolder(xw, ns)
        End If

        xw.WriteEndElement() ' </Document>
        xw.WriteEndElement() ' </kml>
        xw.WriteEndDocument()
    End Sub

    ''' <summary>
    ''' Writes a LineString Placemark for the recorded flight track.
    ''' </summary>
    Private Sub WriteTrackPlacemark(xw As XmlWriter, fixLines As List(Of String), trackName As String, ns As String)
        xw.WriteStartElement("Placemark", ns)
        xw.WriteElementString("name", ns, trackName & " Track")
        xw.WriteStartElement("LineString", ns)
        xw.WriteElementString("tessellate", ns, "1")

        xw.WriteStartElement("coordinates", ns)
        For Each line In fixLines
            Dim coord As String = ConvertBRecordToCoordinate(line)
            If coord IsNot Nothing Then
                xw.WriteString(coord)
            End If
        Next
        xw.WriteEndElement() ' </coordinates>

        xw.WriteEndElement() ' </LineString>
        xw.WriteEndElement() ' </Placemark>
    End Sub

    ''' <summary>
    ''' Writes a LineString Placemark that represents the task route from C-record waypoints.
    ''' </summary>
    Private Sub WriteRoutePlacemark(xw As XmlWriter, trackName As String, ns As String)
        xw.WriteStartElement("Placemark", ns)
        xw.WriteElementString("name", ns, If(String.IsNullOrWhiteSpace(_routeTitle), trackName & " Route", _routeTitle))
        xw.WriteStartElement("LineString", ns)
        xw.WriteElementString("tessellate", ns, "1")

        xw.WriteStartElement("coordinates", ns)
        For Each wp In _routeWaypoints
            xw.WriteString(String.Format(CultureInfo.InvariantCulture, "{0:0.000000},{1:0.000000},0 ", wp.Longitude, wp.Latitude))
        Next
        xw.WriteEndElement() ' </coordinates>

        xw.WriteEndElement() ' </LineString>
        xw.WriteEndElement() ' </Placemark>
    End Sub

    ''' <summary>
    ''' Writes individual Point Placemarks for each task waypoint.
    ''' </summary>
    Private Sub WriteWaypointFolder(xw As XmlWriter, ns As String)
        xw.WriteStartElement("Folder", ns)
        xw.WriteElementString("name", ns, "Task Waypoints")

        For Each wp In _routeWaypoints
            xw.WriteStartElement("Placemark", ns)
            xw.WriteElementString("name", ns, wp.Name)
            xw.WriteStartElement("Point", ns)
            xw.WriteElementString("coordinates", ns, String.Format(CultureInfo.InvariantCulture, "{0:0.000000},{1:0.000000},0", wp.Longitude, wp.Latitude))
            xw.WriteEndElement() ' </Point>
            xw.WriteEndElement() ' </Placemark>
        Next

        xw.WriteEndElement() ' </Folder>
    End Sub

    ''' <summary>
    ''' Converts a B-record to a KML coordinate string (lon,lat,altitude) with trailing space.
    ''' Returns Nothing if the record is invalid.
    ''' </summary>
    Private Function ConvertBRecordToCoordinate(line As String) As String
        If line.Length < 35 OrElse line(0) <> "B"c Then
            Return Nothing
        End If

        Dim timeStr As String = line.Substring(1, 6)
        Dim latStr As String = line.Substring(7, 8)
        Dim lonStr As String = line.Substring(15, 9)
        Dim validity As Char = line(24)
        Dim gpsAltStr As String = line.Substring(30, 5)

        If validity = "V"c Then
            Return Nothing
        End If

        Dim lat As Double
        Dim lon As Double
        If Not TryParseIgcLatitude(latStr, lat) Then Return Nothing
        If Not TryParseIgcLongitude(lonStr, lon) Then Return Nothing

        Dim gpsAlt As Integer
        If Not Integer.TryParse(gpsAltStr, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, gpsAlt) Then
            gpsAlt = 0
        End If

        Dim timestamp As String = Nothing
        If _flightDateUtc.HasValue Then
            Dim hh As Integer
            Dim mm As Integer
            Dim ss As Integer

            If Integer.TryParse(timeStr.Substring(0, 2), hh) AndAlso
               Integer.TryParse(timeStr.Substring(2, 2), mm) AndAlso
               Integer.TryParse(timeStr.Substring(4, 2), ss) Then

                Dim dt As DateTime = _flightDateUtc.Value _
                    .AddHours(hh) _
                    .AddMinutes(mm) _
                    .AddSeconds(ss)
                timestamp = dt.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
            End If
        End If

        Dim coord As New StringBuilder()
        coord.AppendFormat(CultureInfo.InvariantCulture, "{0:0.000000},{1:0.000000},{2} ", lon, lat, gpsAlt)
        If timestamp IsNot Nothing Then
            coord.Append("<!-- ").Append(timestamp).Append(" -->")
        End If

        Return coord.ToString()
    End Function

    ' ---------------- IGC COORD PARSING ----------------

    ''' <summary>
    ''' Parse an IGC latitude of the form DDMMmmmH into decimal degrees.
    ''' Example: "3312944N" -> 33° 12.944' N
    ''' </summary>
    Private Shared Function TryParseIgcLatitude(raw As String, ByRef result As Double) As Boolean
        result = 0.0
        If String.IsNullOrEmpty(raw) OrElse raw.Length <> 8 Then
            Return False
        End If

        Dim hemi As Char = raw(7)
        Dim deg As Integer
        Dim minWhole As Integer
        Dim minThousandths As Integer

        If Not Integer.TryParse(raw.Substring(0, 2), deg) Then Return False
        If Not Integer.TryParse(raw.Substring(2, 2), minWhole) Then Return False
        If Not Integer.TryParse(raw.Substring(4, 3), minThousandths) Then Return False

        Dim minutes As Double = minWhole + (minThousandths / 1000.0)
        Dim decimalDegrees As Double = deg + (minutes / 60.0)

        If hemi = "S"c Then
            decimalDegrees = -decimalDegrees
        End If

        result = decimalDegrees
        Return True
    End Function

    ''' <summary>
    ''' Parse an IGC longitude of the form DDDMMmmmH into decimal degrees.
    ''' Example: "03535762E" -> 35° 35.762' E
    ''' </summary>
    Private Shared Function TryParseIgcLongitude(raw As String, ByRef result As Double) As Boolean
        result = 0.0
        If String.IsNullOrEmpty(raw) OrElse raw.Length <> 9 Then
            Return False
        End If

        Dim hemi As Char = raw(8)
        Dim deg As Integer
        Dim minWhole As Integer
        Dim minThousandths As Integer

        If Not Integer.TryParse(raw.Substring(0, 3), deg) Then Return False
        If Not Integer.TryParse(raw.Substring(3, 2), minWhole) Then Return False
        If Not Integer.TryParse(raw.Substring(5, 3), minThousandths) Then Return False

        Dim minutes As Double = minWhole + (minThousandths / 1000.0)
        Dim decimalDegrees As Double = deg + (minutes / 60.0)

        If hemi = "W"c Then
            decimalDegrees = -decimalDegrees
        End If

        result = decimalDegrees
        Return True
    End Function

End Class
