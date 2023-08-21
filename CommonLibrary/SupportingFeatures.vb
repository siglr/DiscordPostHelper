Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq.Expressions
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits
Imports Microsoft.Win32
Imports System.Reflection

Public Class SupportingFeatures
    Public Enum DiscordTimeStampFormat As Integer
        TimeOnlyWithoutSeconds = 0
        FullDateTimeWithDayOfWeek = 1
        LongDateTime = 2
        CountDown = 3
    End Enum

    Public Enum ClientApp As Integer
        DiscordPostHelper = 1
        SoaringTaskBrowser = 2
        DPHXUnpackAndLoad = 3
    End Enum

    Public ReadOnly DefaultKnownClubEvents As New Dictionary(Of String, PresetEvent)
    Public ReadOnly AllWaypoints As New List(Of ATCWaypoint)
    Public ReadOnly CountryFlagCodes As Dictionary(Of String, String)
    Private Shared _ClientRunning As ClientApp

    Public Shared Property PrefUnits As PreferredUnits

    Public Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    Public Declare Function GetSystemMetrics Lib "user32.dll" (ByVal nIndex As Integer) As Integer
    Public Const GWL_STYLE As Integer = (-16)
    Public Const WS_VSCROLL As Integer = &H200000
    Public Const WS_HSCROLL As Integer = &H100000

    Public Shared ReadOnly Property ClientRunning As ClientApp
        Get
            Return _ClientRunning
        End Get
    End Property

    Public Sub New(Optional RunningClientApp As ClientApp = ClientApp.DiscordPostHelper)

        _ClientRunning = RunningClientApp

        If _ClientRunning = ClientApp.DiscordPostHelper Then
            LoadDefaultClubEvents()
        End If

        CountryFlagCodes = New Dictionary(Of String, String)
        GetCountryFlagCodes()

    End Sub

    Private Sub LoadDefaultClubEvents()
        Dim xmlDoc As New XmlDocument()
        xmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KnownSoaringClubs.xml"))

        Dim events As XmlNodeList = xmlDoc.GetElementsByTagName("KnownSoaringClub")

        For Each eventNode As XmlNode In events
            Dim clubId As String = eventNode("ClubId").InnerText
            Dim clubName As String = eventNode("ClubName").InnerText
            Dim clubFullName As String = eventNode("ClubFullName").InnerText
            Dim msfsServer As String = eventNode("MSFSServer").InnerText
            Dim voiceChannel As String = eventNode("VoiceChannel").InnerText
            Dim dayOfWeek As DayOfWeek = [Enum].Parse(GetType(DayOfWeek), eventNode("ZuluDayOfWeek").InnerText)
            Dim zuluTime As DateTime = DateTime.Parse(eventNode("ZuluTime").InnerText)
            Dim syncFlyDelay As Integer = Integer.Parse(eventNode("SyncFlyDelay").InnerText)
            Dim launchDelay As Integer = Integer.Parse(eventNode("LaunchDelay").InnerText)
            Dim startTaskDelay As Integer = Integer.Parse(eventNode("StartTaskDelay").InnerText)
            Dim eligibleAward As Boolean = Boolean.Parse(eventNode("EligibleAward").InnerText)
            Dim beginnerLink As String = eventNode("BeginnerLink").InnerText

            Dim presetEvent As New PresetEvent(clubId, clubName, clubFullName, msfsServer, voiceChannel, dayOfWeek, zuluTime, syncFlyDelay, launchDelay, startTaskDelay, eligibleAward, beginnerLink)
            DefaultKnownClubEvents.Add(clubId, presetEvent)
        Next
    End Sub

    Public Sub PopulateSoaringClubList(ByVal ctlControl As IList)

        ctlControl.Clear()

        For Each clubID As String In DefaultKnownClubEvents.Keys
            ctlControl.Add(clubID)
        Next

    End Sub
    Public Function RoundTo15Minutes(ByVal minutes As Integer) As Integer
        Return Math.Ceiling(minutes / 15.0) * 15
    End Function

    Public Function GetDistance(totalDistanceKm As String, trackDistanceKm As String, Optional prefUnits As PreferredUnits = Nothing) As String

        Dim totalDistKm As Decimal
        Dim trackDistKm As Decimal
        Dim totalDistMiles As Decimal
        Dim trackDistMiles As Decimal

        Decimal.TryParse(totalDistanceKm, totalDistKm)
        Decimal.TryParse(trackDistanceKm, trackDistKm)
        totalDistMiles = Conversions.KmToMiles(totalDistKm)
        trackDistMiles = Conversions.KmToMiles(trackDistKm)

        If prefUnits Is Nothing OrElse prefUnits.Distance = PreferredUnits.DistanceUnits.Both Then
            Return String.Format("{0:N0} km total ({1:N0} km task) / {2:N0} mi total ({3:N0} mi task)", totalDistKm, trackDistKm, totalDistMiles, trackDistMiles)
        Else
            Select Case prefUnits.Distance
                Case DistanceUnits.Imperial
                    Return String.Format("{0:N0} mi total ({1:N0} mi task)", totalDistMiles, trackDistMiles)
                Case DistanceUnits.Metric
                    Return String.Format("{0:N0} km total ({1:N0} km task)", totalDistKm, trackDistKm)
            End Select
        End If

        Return String.Empty

    End Function

    Public Function GetDuration(durationMin As String, durationMax As String) As String
        Dim minHoursM As String = String.Empty
        Dim maxHoursM As String = String.Empty
        Dim minMinutes As Integer = 0
        Dim maxMinutes As Integer = 0

        Integer.TryParse(durationMin, minMinutes)
        Integer.TryParse(durationMax, maxMinutes)

        Dim minHours As Decimal = minMinutes / 60
        Dim maxHours As Decimal = maxMinutes / 60

        Dim minHoursH As String
        If Math.Floor(minHours) = minHours Then
            minHoursH = $"{minHours:N0}h"
        Else
            minHoursH = $"{Math.Floor(minHours)}h"
            minHoursM = $"{(Math.Abs((Math.Floor(minHours) * 60) - minMinutes)):00}"
        End If

        Dim maxHoursH As String
        If Math.Floor(maxHours) = maxHours Then
            maxHoursH = $"{maxHours:N0}h"
        Else
            maxHoursH = $"{Math.Floor(maxHours)}h"
            maxHoursM = $"{(Math.Abs((Math.Floor(maxHours) * 60) - maxMinutes)):00}"
        End If

        Return $"{minMinutes} to {maxMinutes} minutes ({minHoursH:D2}{minHoursM:D2} to {maxHoursH:D2}{maxHoursM:D2})"

    End Function

    Public Function GetDifficulty(difficultyIndex As Integer, difficultyExtraInfo As String, Optional textOnly As Boolean = False) As String
        Dim difficulty As String = String.Empty

        If textOnly Then
            Select Case difficultyIndex
                Case 0
                    If String.IsNullOrEmpty(difficultyExtraInfo) Then
                        difficulty = "Unknown - Judge by yourself!"
                    Else
                        difficulty = difficultyExtraInfo
                    End If
                Case 1
                    difficulty = $"Beginner{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 2
                    difficulty = $"Student{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 3
                    difficulty = $"Experimented{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 4
                    difficulty = $"Professional{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 5
                    difficulty = $"Champion{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
            End Select

        Else
            Select Case difficultyIndex
                Case 0
                    If String.IsNullOrEmpty(difficultyExtraInfo) Then
                        difficulty = "Unknown - Judge by yourself!"
                    Else
                        difficulty = difficultyExtraInfo
                    End If
                Case 1
                    difficulty = $"★☆☆☆☆ - Beginner{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 2
                    difficulty = $"★★☆☆☆ - Student{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 3
                    difficulty = $"★★★☆☆ - Experimented{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 4
                    difficulty = $"★★★★☆ - Professional{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 5
                    difficulty = $"★★★★★ - Champion{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
            End Select
        End If
        Return difficulty

    End Function

    Public Function ValueToAppendIfNotEmpty(textValue As String, Optional addSpace As Boolean = False, Optional useBrackets As Boolean = False, Optional nbrLineFeed As Integer = 0) As String

        If String.IsNullOrEmpty(textValue) Then
            Return String.Empty
        End If

        Dim textToReturn As String = textValue

        If useBrackets Then
            textToReturn = $"({textToReturn})"
        End If

        If addSpace Then
            textToReturn = $" {textToReturn}"
        End If

        For i As Integer = 1 To nbrLineFeed
            textToReturn &= Environment.NewLine
        Next

        Return textToReturn

    End Function

    Public Function BuildAltitudeRestrictions(ByVal pXmlDocFlightPlan As XmlDocument,
                                              ByRef pFlightTotalDistanceInKm As Single,
                                              ByRef pTaskTotalDistanceInKm As Single,
                                              Optional includeWPName As Boolean = True) As String

        'Build altitude restrictions
        Dim previousATCWaypoing As ATCWaypoint = Nothing
        Dim strRestrictions As String = String.Empty
        Dim blnInTask As Boolean = False
        Dim dblDistanceToPrevious As Single = 0

        pFlightTotalDistanceInKm = 0
        pTaskTotalDistanceInKm = 0
        AllWaypoints.Clear()

        Dim xmlWaypointList As XmlNodeList = pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint")
        Dim ICAO As String = String.Empty
        Dim nbrICAO As Integer = pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint/ICAO/ICAOIdent").Count
        For i As Integer = 0 To xmlWaypointList.Count - 1

            Dim icaoNode As XmlNode = xmlWaypointList.Item(i).SelectSingleNode("ICAO/ICAOIdent")

            If icaoNode IsNot Nothing Then
                Dim icaoIdent As String = icaoNode.InnerText
                ' Do something with the ICAO value
                ICAO = icaoIdent
            Else
                ICAO = String.Empty
            End If

            Dim atcWaypoint As New ATCWaypoint(xmlWaypointList.Item(i).Attributes(0).Value,
                                               xmlWaypointList.Item(i).SelectNodes("WorldPosition").Item(0).FirstChild.Value,
                                               i,
                                               ICAO)
            AllWaypoints.Add(atcWaypoint)
            If atcWaypoint.ContainsRestriction Then
                strRestrictions = $"{strRestrictions}{Environment.NewLine}- {atcWaypoint.Restrictions(includeWPName)}"
            End If
            If i > 0 Then
                'Start adding distance between this waypoint and previous one to the total distance
                dblDistanceToPrevious = Conversions.GetDistanceInKm(previousATCWaypoing.Latitude, previousATCWaypoing.Longitude, atcWaypoint.Latitude, atcWaypoint.Longitude)
                atcWaypoint.DistanceFromPreviousKM = dblDistanceToPrevious
                pFlightTotalDistanceInKm += dblDistanceToPrevious
                atcWaypoint.DistanceFromDepartureKM = pFlightTotalDistanceInKm
            End If
            If blnInTask Then
                'Start adding distance between this waypoint and previous one to the track distance
                pTaskTotalDistanceInKm += dblDistanceToPrevious
                atcWaypoint.DistanceFromTaskStartKM = pTaskTotalDistanceInKm
            End If
            If atcWaypoint.IsTaskStart Then
                blnInTask = True
            End If
            If atcWaypoint.IsTaskEnd Then
                blnInTask = False
            End If
            previousATCWaypoing = atcWaypoint
        Next
        Dim strAltRestrictions As String

        strAltRestrictions = $"## ⚠️ Altitude Restrictions{If(strRestrictions = String.Empty, $"{Environment.NewLine}None", strRestrictions)}"

        Return strAltRestrictions

    End Function

    Public Function FormatEventDateTime(dateTimeToUse As DateTime, ByRef eventDay As DayOfWeek, useUTC As Boolean) As String

        Dim dateTimeInZulu As DateTime
        Dim dateTimeInLocal As DateTime

        Dim result As String

        If useUTC Then
            dateTimeInZulu = dateTimeToUse
            dateTimeInLocal = Conversions.ConvertUTCToLocal(dateTimeInZulu)
            result = $"{dateTimeInLocal.ToLongDateString()} {dateTimeInLocal:hh:mm tt} Local"
            eventDay = dateTimeInLocal.DayOfWeek
        Else
            dateTimeInLocal = dateTimeToUse
            dateTimeInZulu = Conversions.ConvertLocalToUTC(dateTimeInLocal)
            result = $"{dateTimeInZulu.ToLongDateString()} {dateTimeInZulu:hh:mm tt} UTC"
            eventDay = dateTimeInZulu.DayOfWeek
        End If

        Return result

    End Function

    Public Function FindNextDate(startDate As DateTime, dayOfWeek As DayOfWeek, clubDefaultMeetTime As DateTime) As DateTime
        Dim nextDate As DateTime = Conversions.ConvertLocalToUTC(startDate)
        Dim nextDateFound As Boolean = False

        'If today, check if time is before event start
        If nextDate.DayOfWeek = dayOfWeek Then
            If nextDate.TimeOfDay < clubDefaultMeetTime.TimeOfDay Then
                nextDateFound = True
            End If
        End If
        While Not nextDateFound
            nextDate = nextDate.AddDays(1)
            If nextDate.DayOfWeek = dayOfWeek Then
                nextDateFound = True
            End If
        End While
        Return nextDate
    End Function

    Public Function GetFullEventDateTimeInLocal(dateControl As DateTimePicker, timeControl As DateTimePicker, useUTC As Boolean) As DateTime

        Dim dateFromDateControl As DateTime = dateControl.Value
        Dim timeFromTimeControl As DateTime = timeControl.Value

        Dim returnDateTime As New Date(dateFromDateControl.Year, dateFromDateControl.Month, dateFromDateControl.Day, timeFromTimeControl.Hour, timeFromTimeControl.Minute, 0)

        If useUTC Then
            'Need conversion to local
            returnDateTime = Conversions.ConvertUTCToLocal(returnDateTime)
        End If

        Return returnDateTime

    End Function

    Public Shared Function GetFullEventDateTimeInLocal(theDate As Date, theTime As Date, useUTC As Boolean) As DateTime

        Dim returnDateTime As New Date(theDate.Year, theDate.Month, theDate.Day, theTime.Hour, theTime.Minute, 0)

        If useUTC Then
            'Need conversion to local
            returnDateTime = Conversions.ConvertUTCToLocal(returnDateTime)
        End If

        Return returnDateTime

    End Function

    Public Function GetDiscordTimeStampForDate(dateToUse As DateTime, format As DiscordTimeStampFormat) As String

        Dim formatAbbr As String = String.Empty

        Select Case format
            Case DiscordTimeStampFormat.TimeOnlyWithoutSeconds
                formatAbbr = "t"
            Case DiscordTimeStampFormat.FullDateTimeWithDayOfWeek
                formatAbbr = "F"
            Case DiscordTimeStampFormat.LongDateTime
                formatAbbr = "f"
            Case DiscordTimeStampFormat.CountDown
                formatAbbr = "constEarthRadius"

        End Select

        Return $"<t:{Conversions.ConvertDateToUnixTimestamp(dateToUse)}:{formatAbbr}>"


    End Function

    Public Sub UploadFile(folderName As String, fileName As String, xmlString As String)

        Dim request As WebRequest = WebRequest.Create("https://siglr.com/DiscordPostHelper/SaveFlightPlanFileUnderTempFolder.php")
        request.Method = "POST"
        Dim postData As String = $"xmlString={HttpUtility.UrlEncode(xmlString)}&folderName={HttpUtility.UrlEncode(folderName)}&fileName={HttpUtility.UrlEncode(fileName)}"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        Console.WriteLine(responseFromServer)
        reader.Close()
        dataStream.Close()
        response.Close()

        ' Output the response to the console
        Console.WriteLine(responseFromServer)

    End Sub

    Public Sub DeleteTempFile(ByVal fileName As String)

        Dim request As HttpWebRequest = CType(WebRequest.Create($"https://siglr.com/DiscordPostHelper/DeleteTempFolder.php?folder={fileName}"), HttpWebRequest)
        request.Method = "GET"

        Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)

        Using reader As New IO.StreamReader(response.GetResponseStream())
            Dim result As String = reader.ReadToEnd()
            Console.WriteLine(result)
        End Using

    End Sub

    Public Function GenerateRandomFileName() As String
        Dim randomBytes(11) As Byte
        Using rng As New RNGCryptoServiceProvider()
            rng.GetBytes(randomBytes)
        End Using
        Return BitConverter.ToString(randomBytes).Replace("-", "")
    End Function

    Public Function ExtraFileExtensionIsValid(filename As String) As Boolean

        'Check file extension
        Dim fileExtension As String = Path.GetExtension(filename)

        fileExtension = fileExtension.Replace(".", "")

        If String.IsNullOrEmpty(fileExtension) Then
            Return True
        End If

        Dim invalidExtensions As String() = {"exe", "zip", "xslm", "docm", "pptm", "msi", "vbs", "js", "bat", "cmd", "reg", "ps1", "com", "dll"}
        For Each extension As String In invalidExtensions
            If fileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If
        Next

        Return True

    End Function

    Public Function GetAllWPCoordinates() As String

        Dim sb As New StringBuilder()

        If AllWaypoints.Count > 0 Then
            sb.AppendLine("## 🗺 Waypoint Coordinates for Xbox Users")
            For Each wp As ATCWaypoint In AllWaypoints
                If wp.ICAO <> String.Empty Then
                    'Airports - add ICAO
                    sb.AppendLine($"- {wp.WaypointName}: {wp.ICAO}")
                Else
                    sb.AppendLine($"- {wp.WaypointName}: {wp.Latitude:0.000000} {wp.Longitude:0.000000}")
                End If
            Next
        End If

        Return sb.ToString

    End Function

    Public Sub CreateDPHXFile(ByVal dphxFilePath As String, ByVal filesToInclude As List(Of String))
        Using archive As ZipArchive = ZipFile.Open(dphxFilePath, ZipArchiveMode.Create)
            For Each fileToZip As String In filesToInclude
                archive.CreateEntryFromFile(fileToZip, Path.GetFileName(fileToZip))
            Next
        End Using
    End Sub

    Public Function UnpackDPHXFile(ByVal dphxFilePath As String) As String

        Dim folderToUnpackDialog As New FolderBrowserDialog

        folderToUnpackDialog.SelectedPath = Path.GetDirectoryName(Path.GetFullPath(dphxFilePath))
        folderToUnpackDialog.Description = $"Select folder where to unpack the session package ""{Path.GetFileName(dphxFilePath)}"""
        folderToUnpackDialog.ShowNewFolderButton = True

        Dim result As DialogResult = folderToUnpackDialog.ShowDialog()

        If result = DialogResult.Cancel Then
            Return String.Empty
        End If

        Dim overwriteResult As DialogResult
        If Directory.Exists(folderToUnpackDialog.SelectedPath) Then
            'Folder exists - files may be overwritten
            overwriteResult = MessageBox.Show("Existing files in this folder may get overwritten by the ones in the package, do you want to confirm each individual file?", $"Unpacking to {folderToUnpackDialog.SelectedPath}", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            If overwriteResult = DialogResult.Cancel Then
                Return String.Empty
            End If
        End If

        'Unpack files
        Dim individualFileOverwrite As DialogResult
        Dim extractFile As Boolean = True
        Dim dphFilename As String = String.Empty
        Dim fileDestination As String = String.Empty
        Using archive As ZipArchive = ZipFile.OpenRead(dphxFilePath)
            For Each entry As ZipArchiveEntry In archive.Entries
                fileDestination = Path.Combine(folderToUnpackDialog.SelectedPath, entry.Name)
                If Path.GetExtension(fileDestination) = ".dph" Then
                    dphFilename = fileDestination
                End If
                If File.Exists(fileDestination) Then
                    If overwriteResult = DialogResult.Yes Then
                        individualFileOverwrite = MessageBox.Show($"File {entry.Name} already exists - do you want to overwrite?", "Confirm file overwrite", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                        Select Case individualFileOverwrite
                            Case DialogResult.Cancel
                                Return String.Empty
                            Case DialogResult.No
                                'Do not overwrite
                                extractFile = False
                            Case DialogResult.Yes
                                'Overwrite
                                extractFile = True
                        End Select
                    Else
                        extractFile = True
                    End If
                Else
                    extractFile = True
                End If
                If extractFile Then
                    entry.ExtractToFile(fileDestination, True)
                End If
            Next
        End Using

        Return dphFilename

    End Function

    Public Function CleanupDPHXTempFolder(ByVal unpackFolder As String) As Boolean

        Dim success As Boolean = True

        If Directory.Exists(unpackFolder) Then
            'Folder exists - delete files
            Dim files As String() = Directory.GetFiles(unpackFolder)
            For Each file As String In files
                Try
                    IO.File.Delete(file)
                Catch ex As Exception
                    success = False
                End Try
            Next
        End If

        Return success

    End Function
    Public Function UnpackDPHXFileToTempFolder(ByVal dphxFilePath As String, ByVal unpackFolder As String) As String

        If Directory.Exists(unpackFolder) Then
            'Folder exists - delete files
            Dim files As String() = Directory.GetFiles(unpackFolder)
            For Each file As String In files
                Try
                    IO.File.Delete(file)
                Catch ex As Exception
                End Try
            Next
        Else
            'Create folder
            Directory.CreateDirectory(unpackFolder)
        End If

        'Unpack files
        Dim dphFilename As String = String.Empty
        Dim fileDestination As String = String.Empty
        Using archive As ZipArchive = ZipFile.OpenRead(dphxFilePath)
            For Each entry As ZipArchiveEntry In archive.Entries
                fileDestination = Path.Combine(unpackFolder, entry.Name)
                If Path.GetExtension(fileDestination) = ".dph" Then
                    dphFilename = fileDestination
                End If
                entry.ExtractToFile(fileDestination, True)
            Next
        End Using

        Return dphFilename

    End Function

    Public Function GetVersionInfo() As VersionInfo

        Dim cleanResponseString As String = String.Empty

        If Debugger.IsAttached OrElse File.Exists($"{Application.StartupPath}\ForceLocalUpdate.txt") Then
            'Read the XML locally instead
            Dim localFilePath As String = $"H:\DiscordHelper - 4.8.1\DiscordHelper\{ClientRunning.ToString}.VersionInfo.xml"
            cleanResponseString = File.ReadAllText(localFilePath)
        Else
            'Read the XML from GitHub
            LogDateTime($"{ClientRunning.ToString} {Assembly.GetExecutingAssembly().GetName().Version}")

            Dim url As String = $"https://raw.githubusercontent.com/siglr/DiscordPostHelper/master/{ClientRunning.ToString}.VersionInfo.xml"
            Dim client As New WebClient()
            Dim responseBytes As Byte() = Nothing

            Try
                responseBytes = client.DownloadData(url)
            Catch ex As WebException
                MessageBox.Show("It appears it is impossible to retrieve version information right now. You will have to manually check for the latest version.", "Checking latest version", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try

            Dim responseString As String = Encoding.UTF8.GetString(responseBytes)

            ' Remove ZWNBSP character using regular expression pattern
            cleanResponseString = Regex.Replace(responseString, "^\uFEFF", String.Empty)

        End If

        Dim versionInfo As VersionInfo
        Dim serializer As New XmlSerializer(GetType(VersionInfo))
        Dim reader As New StringReader(cleanResponseString)
        versionInfo = DirectCast(serializer.Deserialize(reader), VersionInfo)

        Return versionInfo

    End Function

    Public Sub LogDateTime(parameter As String)
        Dim url As String = $"https://siglr.com/DiscordPostHelper/DPHGetVersionInfo.php?param={Uri.EscapeDataString(parameter)}"
        Dim client As New WebClient()

        If Debugger.IsAttached Or File.Exists($"{Application.StartupPath}\{Environment.UserName}.txt") Then
            'Do nothing
        Else
            Try
                client.DownloadString(url)
            Catch ex As Exception
            End Try
        End If
    End Sub


    Public Function FormatVersionNumber(versionNumber As String) As String
        Dim parts As String() = versionNumber.Split("."c)
        Dim formattedMonth As String = parts(1).PadLeft(2, "0"c)
        Dim formattedDay As String = parts(2).PadLeft(2, "0"c)
        Dim formattedNumber As String = parts(3).PadLeft(2, "0"c)
        Return $"{parts(0)}.{formattedMonth}.{formattedDay}.{formattedNumber}"
    End Function

    Public Function ShowVersionForm(verInfo As VersionInfo, currentVersion As String) As DialogResult
        Dim versionForm As New VersionInfoForm

        versionForm.lblLocalVersion.Text = currentVersion
        versionForm.lblLatestVersion.Text = verInfo.CurrentLatestVersion

        Dim sbCumulativeRelease As New StringBuilder
        Dim sbHistoryRelease As New StringBuilder

        sbCumulativeRelease.Append("{\rtf1\ansi ")
        sbHistoryRelease.Append("{\rtf1\ansi ")

        For Each release As Release In verInfo.Releases
            If FormatVersionNumber(release.ReleaseVersion) > FormatVersionNumber(currentVersion) Then
                ' Format release version and title as header or bold
                sbCumulativeRelease.Append($"\b {release.ReleaseVersion} - {release.ReleaseTitle}\b0\line ")
                sbCumulativeRelease.Append($"{release.ReleaseNotes.Replace("        ", "").Trim.Replace(vbLf, "\line ")}")
                sbCumulativeRelease.Append("\line ")
                sbCumulativeRelease.Append("\line ")
            Else
                ' Format release version and title as header or bold
                sbHistoryRelease.Append($"\b {release.ReleaseVersion} - {release.ReleaseTitle}\b0\line ")
                sbHistoryRelease.Append($"{release.ReleaseNotes.Replace("        ", "").Trim.Replace(vbLf, "\line ")}")
                sbHistoryRelease.Append("\line ")
                sbHistoryRelease.Append("\line ")
            End If
        Next

        ' Set formatted text to the controls
        versionForm.txtReleaseNotes.Rtf = sbCumulativeRelease.ToString()
        versionForm.txtReleaseHistory.Rtf = sbHistoryRelease.ToString()

        Return versionForm.ShowDialog()
    End Function

    Public Function DownloadLatestUpdate(version As String, ByRef message As String) As Boolean

        Try
            Dim url As String = String.Empty
            Dim localZip As String = String.Empty
            Dim zipFileName As String = String.Empty

            'Discord Post Helper format example: https://github.com/siglr/DiscordPostHelper/releases/download/DPH.23.3.20.1/Discord.Post.Helper.23.3.20.1.zip
            'Soaring Task Browser format example: https://github.com/siglr/TOBEDEFINED/releases/download/STB.23.3.20.1/Soaring.Task.Browser.23.3.20.1.zip
            'DPHX Unpack and Load format example: https://github.com/siglr/DPHXUnpackAndLoad/releases/download/DPHXUL.23.3.20.1/DPHX.Unpack.Load.23.3.20.1.zip

            Select Case ClientRunning
                Case ClientApp.DiscordPostHelper
                    zipFileName = $"Discord.Post.Helper.{version}.zip"
                    url = "https://github.com/siglr/DiscordPostHelper/releases/download/"
                    url = $"{url}DPH.{version}/{zipFileName}"
                    localZip = $"{Application.StartupPath}\{zipFileName}"
                Case ClientApp.SoaringTaskBrowser
                    zipFileName = $"Soaring.Task.Browser.{version}.zip"
                    url = "https://github.com/siglr/TOBEDEFINED/releases/download/"
                    url = $"{url}STB.{version}/{zipFileName}"
                    localZip = $"{Application.StartupPath}\{zipFileName}"
                Case ClientApp.DPHXUnpackAndLoad
                    zipFileName = $"DPHX.Unpack.Load.{version}.zip"
                    url = "https://github.com/siglr/DPHXUnpackAndLoad/releases/download/"
                    url = $"{url}DPHXUL.{version}/{zipFileName}"
                    localZip = $"{Application.StartupPath}\{zipFileName}"
            End Select

            If File.Exists($"{Application.StartupPath}\ForceLocalUpdate.txt") Then
                message = $"Forcing local update"
            Else
                message = $"Downloading file {url} to {localZip}"
                Dim client As New WebClient()
                client.DownloadFile(url, localZip)
            End If

            message = $"Openening zip to check for Updater entry"
            'open zip and check if updater is there
            Using archive As ZipArchive = ZipFile.OpenRead(localZip)
                For Each entry As ZipArchiveEntry In archive.Entries
                    If entry.Name = "Updater.exe" Then
                        'Updater found - unzip
                        message = $"Extracting Updater to {Application.StartupPath}\{entry.Name}"
                        entry.ExtractToFile($"{Application.StartupPath}\{entry.Name}", True)
                    End If
                Next
            End Using

            message = $"Launching the updater tool"

            'Launch the Updater program
            Dim startInfo As New ProcessStartInfo($"{Application.StartupPath}\Updater.exe", $"""{localZip}"" {Process.GetCurrentProcess.Id} ""{Process.GetCurrentProcess.MainModule.FileName}""")

            Process.Start(startInfo)

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Sub GetCountryFlagCodes()

        CountryFlagCodes.Add("", "")
        CountryFlagCodes.Add("Afghanistan", ":flag_af:")
        CountryFlagCodes.Add("Albania", ":flag_al:")
        CountryFlagCodes.Add("Algeria", ":flag_dz:")
        CountryFlagCodes.Add("Andorra", ":flag_ad:")
        CountryFlagCodes.Add("Angola", ":flag_ao:")
        CountryFlagCodes.Add("Antigua and Barbuda", ":flag_ag:")
        CountryFlagCodes.Add("Argentina", ":flag_ar:")
        CountryFlagCodes.Add("Armenia", ":flag_am:")
        CountryFlagCodes.Add("Australia", ":flag_au:")
        CountryFlagCodes.Add("Austria", ":flag_at:")
        CountryFlagCodes.Add("Azerbaijan", ":flag_az:")
        CountryFlagCodes.Add("Bahamas", ":flag_bs:")
        CountryFlagCodes.Add("Bahrain", ":flag_bh:")
        CountryFlagCodes.Add("Bangladesh", ":flag_bd:")
        CountryFlagCodes.Add("Barbados", ":flag_bb:")
        CountryFlagCodes.Add("Belarus", ":flag_by:")
        CountryFlagCodes.Add("Belgium", ":flag_be:")
        CountryFlagCodes.Add("Belize", ":flag_bz:")
        CountryFlagCodes.Add("Benin", ":flag_bj:")
        CountryFlagCodes.Add("Bhutan", ":flag_bt:")
        CountryFlagCodes.Add("Bolivia", ":flag_bo:")
        CountryFlagCodes.Add("Bosnia and Herzegovina", ":flag_ba:")
        CountryFlagCodes.Add("Botswana", ":flag_bw:")
        CountryFlagCodes.Add("Brazil", ":flag_br:")
        CountryFlagCodes.Add("Brunei", ":flag_bn:")
        CountryFlagCodes.Add("Bulgaria", ":flag_bg:")
        CountryFlagCodes.Add("Burkina Faso", ":flag_bf:")
        CountryFlagCodes.Add("Burundi", ":flag_bi:")
        CountryFlagCodes.Add("Cambodia", ":flag_kh:")
        CountryFlagCodes.Add("Cameroon", ":flag_cm:")
        CountryFlagCodes.Add("Canada", ":flag_ca:")
        CountryFlagCodes.Add("Cape Verde", ":flag_cv:")
        CountryFlagCodes.Add("Central African Republic", ":flag_cf:")
        CountryFlagCodes.Add("Chad", ":flag_td:")
        CountryFlagCodes.Add("Chile", ":flag_cl:")
        CountryFlagCodes.Add("China", ":flag_cn:")
        CountryFlagCodes.Add("Christmas Island", ":flag_cx:")
        CountryFlagCodes.Add("Cocos Islands", ":flag_cc:")
        CountryFlagCodes.Add("Colombia", ":flag_co:")
        CountryFlagCodes.Add("Comoros", ":flag_km:")
        CountryFlagCodes.Add("Congo", ":flag_cg:")
        CountryFlagCodes.Add("Congo (Democratic Republic of the)", ":flag_cd:")
        CountryFlagCodes.Add("Cook Islands", ":flag_ck:")
        CountryFlagCodes.Add("Costa Rica", ":flag_cr:")
        CountryFlagCodes.Add("Croatia", ":flag_hr:")
        CountryFlagCodes.Add("Cuba", ":flag_cu:")
        CountryFlagCodes.Add("Curaçao", ":flag_cw:")
        CountryFlagCodes.Add("Cyprus", ":flag_cy:")
        CountryFlagCodes.Add("Czech Republic", ":flag_cz:")
        CountryFlagCodes.Add("Denmark", ":flag_dk:")
        CountryFlagCodes.Add("Djibouti", ":flag_dj:")
        CountryFlagCodes.Add("Dominica", ":flag_dm:")
        CountryFlagCodes.Add("Dominican Republic", ":flag_do:")
        CountryFlagCodes.Add("East Timor", ":flag_tl:")
        CountryFlagCodes.Add("Ecuador", ":flag_ec:")
        CountryFlagCodes.Add("Egypt", ":flag_eg:")
        CountryFlagCodes.Add("El Salvador", ":flag_sv:")
        CountryFlagCodes.Add("Equatorial Guinea", ":flag_gq:")
        CountryFlagCodes.Add("Eritrea", ":flag_er:")
        CountryFlagCodes.Add("Estonia", ":flag_ee:")
        CountryFlagCodes.Add("Ethiopia", ":flag_et:")
        CountryFlagCodes.Add("Falkland Islands", ":flag_fk:")
        CountryFlagCodes.Add("Faroe Islands", ":flag_fo:")
        CountryFlagCodes.Add("Fiji", ":flag_fj:")
        CountryFlagCodes.Add("Finland", ":flag_fi:")
        CountryFlagCodes.Add("France", ":flag_fr:")
        CountryFlagCodes.Add("French Guiana", ":flag_gf:")
        CountryFlagCodes.Add("French Polynesia", ":flag_pf:")
        CountryFlagCodes.Add("French Southern Territories", ":flag_tf:")
        CountryFlagCodes.Add("Gabon", ":flag_ga:")
        CountryFlagCodes.Add("Gambia", ":flag_gm:")
        CountryFlagCodes.Add("Georgia", ":flag_ge:")
        CountryFlagCodes.Add("Germany", ":flag_de:")
        CountryFlagCodes.Add("Ghana", ":flag_gh:")
        CountryFlagCodes.Add("Gibraltar", ":flag_gi:")
        CountryFlagCodes.Add("Greece", ":flag_gr:")
        CountryFlagCodes.Add("Greenland", ":flag_gl:")
        CountryFlagCodes.Add("Grenada", ":flag_gd:")
        CountryFlagCodes.Add("Guadeloupe", ":flag_gp:")
        CountryFlagCodes.Add("Guam", ":flag_gu:")
        CountryFlagCodes.Add("Guatemala", ":flag_gt:")
        CountryFlagCodes.Add("Guernsey", ":flag_gg:")
        CountryFlagCodes.Add("Guinea", ":flag_gn:")
        CountryFlagCodes.Add("Guinea-Bissau", ":flag_gw:")
        CountryFlagCodes.Add("Guyana", ":flag_gy:")
        CountryFlagCodes.Add("Haiti", ":flag_ht:")
        CountryFlagCodes.Add("Honduras", ":flag_hn:")
        CountryFlagCodes.Add("Hungary", ":flag_hu:")
        CountryFlagCodes.Add("Iceland", ":flag_is:")
        CountryFlagCodes.Add("India", ":flag_in:")
        CountryFlagCodes.Add("Indonesia", ":flag_id:")
        CountryFlagCodes.Add("Iran", ":flag_ir:")
        CountryFlagCodes.Add("Iraq", ":flag_iq:")
        CountryFlagCodes.Add("Ireland", ":flag_ie:")
        CountryFlagCodes.Add("Israel", ":flag_il:")
        CountryFlagCodes.Add("Italy", ":flag_it:")
        CountryFlagCodes.Add("Jamaica", ":flag_jm:")
        CountryFlagCodes.Add("Japan", ":flag_jp:")
        CountryFlagCodes.Add("Jordan", ":flag_jo:")
        CountryFlagCodes.Add("Kazakhstan", ":flag_kz:")
        CountryFlagCodes.Add("Kenya", ":flag_ke:")
        CountryFlagCodes.Add("Kiribati", ":flag_ki:")
        CountryFlagCodes.Add("North Korea", ":flag_kp:")
        CountryFlagCodes.Add("Kuwait", ":flag_kw:")
        CountryFlagCodes.Add("Kyrgyzstan", ":flag_kg:")
        CountryFlagCodes.Add("Laos", ":flag_la:")
        CountryFlagCodes.Add("Latvia", ":flag_lv:")
        CountryFlagCodes.Add("Lebanon", ":flag_lb:")
        CountryFlagCodes.Add("Lesotho", ":flag_ls:")
        CountryFlagCodes.Add("Liberia", ":flag_lr:")
        CountryFlagCodes.Add("Libya", ":flag_ly:")
        CountryFlagCodes.Add("Liechtenstein", ":flag_li:")
        CountryFlagCodes.Add("Lithuania", ":flag_lt:")
        CountryFlagCodes.Add("Luxembourg", ":flag_lu:")
        CountryFlagCodes.Add("Macedonia", ":flag_mk:")
        CountryFlagCodes.Add("Madagascar", ":flag_mg:")
        CountryFlagCodes.Add("Malawi", ":flag_mw:")
        CountryFlagCodes.Add("Malaysia", ":flag_my:")
        CountryFlagCodes.Add("Maldives", ":flag_mv:")
        CountryFlagCodes.Add("Mali", ":flag_ml:")
        CountryFlagCodes.Add("Malta", ":flag_mt:")
        CountryFlagCodes.Add("Marshall Islands", ":flag_mh:")
        CountryFlagCodes.Add("Martinique", ":flag_mq:")
        CountryFlagCodes.Add("Mauritania", ":flag_mr:")
        CountryFlagCodes.Add("Mauritius", ":flag_mu:")
        CountryFlagCodes.Add("Mayotte", ":flag_yt:")
        CountryFlagCodes.Add("Mexico", ":flag_mx:")
        CountryFlagCodes.Add("Micronesia", ":flag_fm:")
        CountryFlagCodes.Add("Moldova", ":flag_md:")
        CountryFlagCodes.Add("Monaco", ":flag_mc:")
        CountryFlagCodes.Add("Mongolia", ":flag_mn:")
        CountryFlagCodes.Add("Montenegro", ":flag_me:")
        CountryFlagCodes.Add("Montserrat", ":flag_ms:")
        CountryFlagCodes.Add("Morocco", ":flag_ma:")
        CountryFlagCodes.Add("Mozambique", ":flag_mz:")
        CountryFlagCodes.Add("Myanmar", ":flag_mm:")
        CountryFlagCodes.Add("Namibia", ":flag_na:")
        CountryFlagCodes.Add("Nauru", ":flag_nr:")
        CountryFlagCodes.Add("Nepal", ":flag_np:")
        CountryFlagCodes.Add("Netherlands", ":flag_nl:")
        CountryFlagCodes.Add("New Caledonia", ":flag_nc:")
        CountryFlagCodes.Add("New Zealand", ":flag_nz:")
        CountryFlagCodes.Add("Nicaragua", ":flag_ni:")
        CountryFlagCodes.Add("Niger", ":flag_ne:")
        CountryFlagCodes.Add("Nigeria", ":flag_ng:")
        CountryFlagCodes.Add("Niue", ":flag_nu:")
        CountryFlagCodes.Add("Norfolk Island", ":flag_nf:")
        CountryFlagCodes.Add("North Macedonia", ":flag_mk:")
        CountryFlagCodes.Add("Northern Mariana Islands", ":flag_mp:")
        CountryFlagCodes.Add("Norway", ":flag_no:")
        CountryFlagCodes.Add("Oman", ":flag_om:")
        CountryFlagCodes.Add("Pakistan", ":flag_pk:")
        CountryFlagCodes.Add("Palau", ":flag_pw:")
        CountryFlagCodes.Add("Palestine, State of", ":flag_ps:")
        CountryFlagCodes.Add("Panama", ":flag_pa:")
        CountryFlagCodes.Add("Papua New Guinea", ":flag_pg:")
        CountryFlagCodes.Add("Paraguay", ":flag_py:")
        CountryFlagCodes.Add("Peru", ":flag_pe:")
        CountryFlagCodes.Add("Philippines", ":flag_ph:")
        CountryFlagCodes.Add("Pitcairn", ":flag_pn:")
        CountryFlagCodes.Add("Poland", ":flag_pl:")
        CountryFlagCodes.Add("Portugal", ":flag_pt:")
        CountryFlagCodes.Add("Puerto Rico", ":flag_pr:")
        CountryFlagCodes.Add("Qatar", ":flag_qa:")
        CountryFlagCodes.Add("Réunion", ":flag_re:")
        CountryFlagCodes.Add("Romania", ":flag_ro:")
        CountryFlagCodes.Add("Russia", ":flag_ru:")
        CountryFlagCodes.Add("Rwanda", ":flag_rw:")
        CountryFlagCodes.Add("Saint Kitts and Nevis", ":flag_kn:")
        CountryFlagCodes.Add("Saint Lucia", ":flag_lc:")
        CountryFlagCodes.Add("Saint Vincent and the Grenadines", ":flag_vc:")
        CountryFlagCodes.Add("Samoa", ":flag_ws:")
        CountryFlagCodes.Add("San Marino", ":flag_sm:")
        CountryFlagCodes.Add("São Tomé and Príncipe", ":flag_st:")
        CountryFlagCodes.Add("Saudi Arabia", ":flag_sa:")
        CountryFlagCodes.Add("Senegal", ":flag_sn:")
        CountryFlagCodes.Add("Serbia", ":flag_rs:")
        CountryFlagCodes.Add("Seychelles", ":flag_sc:")
        CountryFlagCodes.Add("Sierra Leone", ":flag_sl:")
        CountryFlagCodes.Add("Singapore", ":flag_sg:")
        CountryFlagCodes.Add("Slovakia", ":flag_sk:")
        CountryFlagCodes.Add("Slovenia", ":flag_si:")
        CountryFlagCodes.Add("Solomon Islands", ":flag_sb:")
        CountryFlagCodes.Add("Somalia", ":flag_so:")
        CountryFlagCodes.Add("South Africa", ":flag_za:")
        CountryFlagCodes.Add("South Georgia and the South Sandwich Islands", ":flag_gs:")
        CountryFlagCodes.Add("South Korea", ":flag_kr:")
        CountryFlagCodes.Add("South Sudan", ":flag_ss:")
        CountryFlagCodes.Add("Spain", ":flag_es:")
        CountryFlagCodes.Add("Sri Lanka", ":flag_lk:")
        CountryFlagCodes.Add("Sudan", ":flag_sd:")
        CountryFlagCodes.Add("Suriname", ":flag_sr:")
        CountryFlagCodes.Add("Svalbard and Jan Mayen", ":flag_sj:")
        CountryFlagCodes.Add("Swaziland", ":flag_sz:")
        CountryFlagCodes.Add("Sweden", ":flag_se:")
        CountryFlagCodes.Add("Switzerland", ":flag_ch:")
        CountryFlagCodes.Add("Syria", ":flag_sy:")
        CountryFlagCodes.Add("Taiwan", ":flag_tw:")
        CountryFlagCodes.Add("Tajikistan", ":flag_tj:")
        CountryFlagCodes.Add("Tanzania", ":flag_tz:")
        CountryFlagCodes.Add("Thailand", ":flag_th:")
        CountryFlagCodes.Add("Timor-Leste", ":flag_tl:")
        CountryFlagCodes.Add("Togo", ":flag_tg:")
        CountryFlagCodes.Add("Tokelau", ":flag_tk:")
        CountryFlagCodes.Add("Tonga", ":flag_to:")
        CountryFlagCodes.Add("Trinidad and Tobago", ":flag_tt:")
        CountryFlagCodes.Add("Tunisia", ":flag_tn:")
        CountryFlagCodes.Add("Turkey", ":flag_tr:")
        CountryFlagCodes.Add("Turkmenistan", ":flag_tm:")
        CountryFlagCodes.Add("Turks and Caicos Islands", ":flag_tc:")
        CountryFlagCodes.Add("Tuvalu", ":flag_tv:")
        CountryFlagCodes.Add("Uganda", ":flag_ug:")
        CountryFlagCodes.Add("Ukraine", ":flag_ua:")
        CountryFlagCodes.Add("United Arab Emirates", ":flag_ae:")
        CountryFlagCodes.Add("United Kingdom", ":flag_gb:")
        CountryFlagCodes.Add("United States", ":flag_us:")
        CountryFlagCodes.Add("United States Minor Outlying Islands", ":flag_um:")
        CountryFlagCodes.Add("Uruguay", ":flag_uy:")
        CountryFlagCodes.Add("Uzbekistan", ":flag_uz:")
        CountryFlagCodes.Add("Vanuatu", ":flag_vu:")
        CountryFlagCodes.Add("Vatican City", ":flag_va:")
        CountryFlagCodes.Add("Venezuela", ":flag_ve:")
        CountryFlagCodes.Add("Vietnam", ":flag_vn:")
        CountryFlagCodes.Add("Virgin Islands, British", ":flag_vg:")
        CountryFlagCodes.Add("Virgin Islands, U.S.", ":flag_vi:")
        CountryFlagCodes.Add("Wallis and Futuna", ":flag_wf:")
        CountryFlagCodes.Add("Western Sahara", ":flag_eh:")
        CountryFlagCodes.Add("Yemen", ":flag_ye:")
        CountryFlagCodes.Add("Zambia", ":flag_zm:")
        CountryFlagCodes.Add("Zimbabwe", ":flag_zw:")
    End Sub
    Public Sub FillCountryFlagList(ByVal ctlControl As IList)
        ctlControl.Clear()
        For Each country As String In CountryFlagCodes.Keys
            ctlControl.Add(country)
        Next
    End Sub
    Public Function DSTAppliesForLocalDate(localDateTime As Date) As Boolean
        Dim localTimeZone As TimeZoneInfo = TimeZoneInfo.Local
        ' Check if the local time zone is currently observing daylight saving time
        Return localTimeZone.IsDaylightSavingTime(localDateTime)
    End Function
    Public Sub FormatMarkdownToRTF(ByVal input As String, ByRef richTextBox As RichTextBox)
        ' Regex patterns to match bold and italic texts
        Dim boldPattern As String = "\*\*(.+?)\*\*"
        Dim italicPattern As String = "(?<!\*)\*(.+?)\*(?!\*)"

        ' Replace bold and italic markdown syntax with corresponding RTF code
        Dim boldReplaced As String = Regex.Replace(input, boldPattern, "{\b $1\b0 }")
        Dim rtfFormatted As String = Regex.Replace(boldReplaced, italicPattern, "{\i $1\i0 }")

        ' Replace vbcrlf with RTF line break code
        rtfFormatted = rtfFormatted.Replace(vbCrLf, "\line ")

        ' Set the RTF-formatted text to the RichTextBox control
        richTextBox.Rtf = "{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Arial;}}\viewkind4\uc1\pard\lang1033\f0\fs20 " & rtfFormatted & "\par}"
    End Sub

    Public Function CheckRequiredNetFrameworkVersion() As Boolean

        'Const net481 As Integer = 533320
        Const net480 As Integer = 528049

        Dim result As Boolean = False
        Dim subkey As String = "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"

        Using ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)
            If ndpKey IsNot Nothing AndAlso ndpKey.GetValue("Release") IsNot Nothing Then
                If CInt(ndpKey.GetValue("Release")) >= net480 Then
                    result = True
                End If
            End If
        End Using

        Return result

    End Function

    Public Function GetVoiceChannels() As List(Of String)

        Return New List(Of String) From {"Unicom 1", "Unicom 2", "Unicom 3", "Push to talk 1", "Sim Soaring Club (PTT)",
                                         "Flight 01", "Flight 02", "Thermal Smashing"}

    End Function

    Public Function GetMSFSServers() As List(Of String)

        Return New List(Of String) From {"West Europe", "North Europe", "West USA", "East USA", "Southeast Asia"}

    End Function

    Public Shared Sub SetZoomFactorOfRichTextBox(rtfControl As RichTextBox)

        If rtfControl.Text.Trim = String.Empty Then
            Exit Sub
        End If

        Dim bVScrollBar As Boolean
        bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
        Select Case bVScrollBar
            Case True
                'Scrollbar is visible - Make it smaller
                Do
                    If (rtfControl.ZoomFactor) - 0.01 <= 0.015625 Then
                        Exit Do
                    End If
                    rtfControl.ZoomFactor = rtfControl.ZoomFactor - 0.01
                    bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
                    'If the scrollbar is no longer visible we are done!
                    If bVScrollBar = False Then Exit Do
                Loop
            Case False
                'Scrollbar is not visible - Make it bigger
                Do
                    If (rtfControl.ZoomFactor + 0.01) >= 64 Then
                        Exit Do
                    End If
                    rtfControl.ZoomFactor = rtfControl.ZoomFactor + 0.01
                    bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
                    If bVScrollBar = True Then
                        Do
                            'Found the scrollbar, make smaller until bar is not visible
                            If (rtfControl.ZoomFactor) - 0.01 <= 0.015625 Then
                                Exit Do
                            End If
                            rtfControl.ZoomFactor = rtfControl.ZoomFactor - 0.01
                            bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
                            'If the scrollbar is no longer visible we are done!
                            If bVScrollBar = False Then Exit Do
                        Loop
                        Exit Do
                    End If
                Loop
        End Select

    End Sub

    Public Shared Function GetTimeZoneInformation() As List(Of String)
        Dim currentTimeZone As TimeZone = TimeZone.CurrentTimeZone
        Dim isDaylightSavingTime As Boolean = currentTimeZone.IsDaylightSavingTime(DateTime.Now)
        Dim offset As Double = currentTimeZone.GetUtcOffset(DateTime.Now).TotalHours
        Dim result As New List(Of String)

        If isDaylightSavingTime Then
            result.Add(currentTimeZone.DaylightName)
        Else
            result.Add(currentTimeZone.StandardName)
        End If
        If offset >= 0 Then
            result.Add($"+{offset.ToString}")
        Else
            result.Add(offset.ToString)
        End If

        Return result

    End Function

    Public Function GetIntegerFromString(ByVal input As String) As Integer
        Dim result As Integer = 0
        Integer.TryParse(input, result)
        Return result
    End Function

    Public Shared Sub EnteringTextBox(txtbox As Windows.Forms.TextBox)
        txtbox.SelectAll()
        txtbox.SelectionStart = 0
    End Sub

    Public Shared Sub WriteRegistryKey(valueName As String, value As Integer)

        Dim keyPath As String = "Software\SIGLR\MSFS Soaring Task Tools"

        Try
            Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
                key.SetValue(valueName, value)
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred trying to write to the registry!", "Writing user settings to registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Public Shared Sub WriteRegistryKey(valueName As String, value As String)

        Dim keyPath As String = "Software\SIGLR\MSFS Soaring Task Tools"

        Try
            Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
                key.SetValue(valueName, value)
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred trying to write to the registry!", "Writing user settings to registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Public Shared Function ReadRegistryKey(valueName As String, defaultValue As Integer) As Integer

        Dim keyPath As String = "Software\SIGLR\MSFS Soaring Task Tools"

        Dim value As Integer = defaultValue

        Try
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
                If key IsNot Nothing Then
                    value = CInt(key.GetValue(valueName, defaultValue))
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred trying to read from the registry!", "Reading user settings from registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return value

    End Function

    Public Shared Function ReadRegistryKey(valueName As String, defaultValue As String) As String

        Dim keyPath As String = "Software\SIGLR\MSFS Soaring Task Tools"

        Dim value As String = defaultValue

        Try
            Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
                If key IsNot Nothing Then
                    value = key.GetValue(valueName, defaultValue).ToString
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred trying to read from the registry!", "Reading user settings from registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return value

    End Function

    Public Sub DownloadCountryFlags(selectedCountries As List(Of String))

        Dim flagsDirectory As String = ReadRegistryKey("CountryFlagsFolder", String.Empty)

        If flagsDirectory = String.Empty Then
            'Set the CountryFlags folder to use
            flagsDirectory = Path.Combine(Application.StartupPath, "CountryFlags")
            WriteRegistryKey("CountryFlagsFolder", flagsDirectory)
        Else
            If Not Directory.Exists(flagsDirectory) Then
                flagsDirectory = Path.Combine(Application.StartupPath, "CountryFlags")
                WriteRegistryKey("CountryFlagsFolder", flagsDirectory)
            End If
        End If

        ' Create the CountryFlags directory if it doesn't exist
        If Not Directory.Exists(flagsDirectory) Then
            Directory.CreateDirectory(flagsDirectory)
        End If

        For Each countryName As String In selectedCountries
            If countryName <> String.Empty AndAlso CountryFlagCodes.ContainsKey(countryName) Then
                Dim countryCode As String = CountryFlagCodes(countryName)
                Dim flagFileName As String = countryCode.Substring(6, 2) & ".png"
                Dim flagFilePath As String = Path.Combine(flagsDirectory, flagFileName)

                ' Check if the flag image is already downloaded
                If Not File.Exists(flagFilePath) Then
                    ' Download the flag image
                    Dim flagUrl As String = $"https://flagcdn.com/56x42/{countryCode.Substring(6, 2)}.png"
                    Using client As New WebClient()
                        client.DownloadFile(flagUrl, flagFilePath)
                    End Using
                End If
            End If
        Next
    End Sub

    Public Sub RemoveForbiddenPrefixes(textCtrl As Windows.Forms.TextBox)
        Dim pattern As String = "^\s*(#|##)\s"
        Dim replacement As String = ""
        Dim modifiedLines As New List(Of String)()

        For Each line As String In textCtrl.Lines
            modifiedLines.Add(Regex.Replace(line, pattern, replacement))
        Next

        textCtrl.Text = String.Join(Environment.NewLine, modifiedLines)
    End Sub

    Public Sub ExpressEventTimesInMSFSTime(fullMeetDateTimeLocal As DateTime,
                                           fullSyncFlyDateTimeLocal As DateTime,
                                           fullLaunchDateTimeLocal As DateTime,
                                           fullStartTaskDateTimeLocal As DateTime,
                                           fullMSFSLocalDateTime As DateTime,
                                           useEventSyncFly As Boolean,
                                           useEventLaunch As Boolean,
                                           ByRef fullMeetDateTimeMSFS As DateTime,
                                           ByRef fullSyncFlyDateTimeMSFS As DateTime,
                                           ByRef fullLaunchDateTimeMSFS As DateTime,
                                           ByRef fullStartTaskDateTimeMSFS As DateTime)

        If useEventSyncFly Then
            fullSyncFlyDateTimeMSFS = fullMSFSLocalDateTime
            fullLaunchDateTimeMSFS = fullSyncFlyDateTimeMSFS.Add(fullLaunchDateTimeLocal - fullSyncFlyDateTimeLocal)
            fullStartTaskDateTimeMSFS = fullSyncFlyDateTimeMSFS.Add(fullStartTaskDateTimeLocal - fullSyncFlyDateTimeLocal)
        ElseIf useEventLaunch Then
            fullLaunchDateTimeMSFS = fullMSFSLocalDateTime
            fullStartTaskDateTimeMSFS = fullLaunchDateTimeMSFS.Add(fullStartTaskDateTimeLocal - fullLaunchDateTimeLocal)
        Else
            fullMeetDateTimeMSFS = fullMSFSLocalDateTime
            fullStartTaskDateTimeMSFS = fullMeetDateTimeMSFS.Add(fullStartTaskDateTimeLocal - fullMeetDateTimeLocal)
        End If

    End Sub

    Public Shared Function ConvertToUnicodeDecimal(input As String) As String
        Dim result As New StringBuilder()

        For i As Integer = 0 To input.Length - 1
            If Char.IsSurrogatePair(input, i) Then
                Dim codePoint As Integer = Char.ConvertToUtf32(input(i), input(i + 1))
                result.Append("\u" + codePoint.ToString() + "  ")
                i += 1 ' Skip the low surrogate
            Else
                result.Append(input(i))
            End If
        Next

        Return result.ToString()
    End Function
End Class

