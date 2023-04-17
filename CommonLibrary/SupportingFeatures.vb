Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization

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
    End Enum

    Public ReadOnly DefaultKnownClubEvents As New Dictionary(Of String, PresetEvent)
    Public ReadOnly AllWaypoints As New List(Of ATCWaypoint)
    Public ReadOnly ClientRunning As ClientApp
    Public ReadOnly CountryFlagCodes As Dictionary(Of String, String)

    Public Sub New(Optional RunningClientApp As ClientApp = ClientApp.DiscordPostHelper)

        ClientRunning = RunningClientApp

        If ClientRunning = ClientApp.DiscordPostHelper Then
            LoadDefaultClubEvents()
        End If

        CountryFlagCodes = New Dictionary(Of String, String)
        GetCountryFlagCodes()

    End Sub

    Private Sub LoadDefaultClubEvents()

        DefaultKnownClubEvents.Add("GG-STC", New PresetEvent("GG-STC", "East USA", "Unicom 1", DayOfWeek.Wednesday, DateTime.Parse("1:00 AM"), 10, 0, 10, False))
        DefaultKnownClubEvents.Add("GG-SFC", New PresetEvent("GG-SFC", "West USA", "Unicom 3", DayOfWeek.Friday, DateTime.Parse("9:00 PM"), 30, 0, 0, False))
        DefaultKnownClubEvents.Add("SSC SATURDAY", New PresetEvent("SSC Saturday", "West Europe", "Sim Soaring Club (PTT)", DayOfWeek.Saturday, DateTime.Parse("5:45 PM"), 15, 0, 30, True))
        DefaultKnownClubEvents.Add("AUS TUESDAYS", New PresetEvent("Aus Tuesdays", "Southeast Asia", "Flight 01", DayOfWeek.Tuesday, DateTime.Parse("8:30 AM"), 15, 0, 15, True))
        'DefaultKnownClubEvents.Add("DTS", New PresetEvent("DTS", "West USA", "Thermal Smashing", DayOfWeek.Tuesday, DateTime.Parse("8:30 AM"), True))

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

    Public Function GetDistance(totalDistanceKm As String, trackDistanceKm As String) As String

        Dim totalDistKm As Decimal
        Dim trackDistKm As Decimal
        Dim totalDistMiles As Decimal
        Dim trackDistMiles As Decimal

        Decimal.TryParse(totalDistanceKm, totalDistKm)
        Decimal.TryParse(trackDistanceKm, trackDistKm)
        totalDistMiles = Conversions.KmToMiles(totalDistKm)
        trackDistMiles = Conversions.KmToMiles(trackDistKm)

        Return String.Format("{0:N0} km total ({1:N0} km task) / {2:N0} mi total ({3:N0} mi task)", totalDistKm, trackDistKm, totalDistMiles, trackDistMiles)

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

    Public Function BuildAltitudeRestrictions(ByVal pXmlDocFlightPlan As XmlDocument, ByRef pFlightTotalDistanceInKm As Integer, ByRef pTaskTotalDistanceInKm As Integer) As String

        'Build altitude restrictions
        Dim previousATCWaypoing As ATCWaypoint = Nothing
        Dim strRestrictions As String = String.Empty
        Dim blnInTask As Boolean = False
        Dim dblDistanceToPrevious As Double = 0

        pFlightTotalDistanceInKm = 0
        pTaskTotalDistanceInKm = 0
        AllWaypoints.Clear()
        For i As Integer = 0 To pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint").Count - 1
            Dim atcWaypoint As New ATCWaypoint(pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint").Item(i).Attributes(0).Value, pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint/WorldPosition").Item(i).FirstChild.Value, i)
            AllWaypoints.Add(atcWaypoint)
            If atcWaypoint.ContainsRestriction Then
                strRestrictions = $"{strRestrictions}{Environment.NewLine}{atcWaypoint.Restrictions}"
            End If
            If i > 0 Then
                'Start adding distance between this waypoint and previous one to the total distance
                dblDistanceToPrevious = Conversions.GetDistanceInKm(previousATCWaypoing.Latitude, previousATCWaypoing.Longitude, atcWaypoint.Latitude, atcWaypoint.Longitude)
                pFlightTotalDistanceInKm += dblDistanceToPrevious
            End If
            If blnInTask Then
                'Start adding distance between this waypoint and previous one to the track distance
                pTaskTotalDistanceInKm += dblDistanceToPrevious
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

        strAltRestrictions = $"**Altitude Restrictions**{If(strRestrictions = String.Empty, $"{Environment.NewLine}None", strRestrictions)}"

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
        Dim seq As Integer = 0

        If AllWaypoints.Count > 0 Then
            sb.AppendLine("**Waypoint Coordinates for Xbox Users**")
            For Each wp As ATCWaypoint In AllWaypoints
                seq += 1
                If seq = 1 Or seq = AllWaypoints.Count Then
                    'Departure and arrival airports - do not add them
                Else
                    sb.AppendLine($"{wp.WPName}: {wp.Latitude:0.000000} {wp.Longitude:0.000000}")
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

    Public Function GetVersionInfo() As VersionInfo

        LogDateTime()

        Dim url As String = $"https://siglr.com/DiscordPostHelper/{ClientRunning.ToString}.VersionInfo.xml"
        Dim client As New WebClient()
        Dim responseBytes As Byte() = client.DownloadData(url)
        Dim responseString As String = Encoding.UTF8.GetString(responseBytes)

        Dim serializer As New XmlSerializer(GetType(VersionInfo))
        Dim reader As New StringReader(responseString)
        Dim versionInfo As VersionInfo = DirectCast(serializer.Deserialize(reader), VersionInfo)

        Return versionInfo

    End Function

    Public Sub LogDateTime()

        Dim url As String = $"https://siglr.com/DiscordPostHelper/DPHGetVersionInfo.php"
        Dim client As New WebClient()
        Dim responseString As String

        If Debugger.IsAttached Or File.Exists($"{Application.StartupPath}\{Environment.UserName}.txt") Then
            'Do nothing
        Else
            Try
                responseString = client.DownloadString(url)
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

        For Each release As Release In verInfo.Releases
            If release.ReleaseVersion = verInfo.CurrentLatestVersion Then
                versionForm.lblReleaseTitle.Text = release.ReleaseTitle
                versionForm.txtReleaseNotes.Text = release.ReleaseNotes.Replace(vbLf, vbCrLf)
            Else
                versionForm.txtReleaseHistory.Text = versionForm.txtReleaseHistory.Text & ($"{release.ReleaseVersion} - {release.ReleaseTitle}{Environment.NewLine}{release.ReleaseNotes.Replace(vbLf, vbCrLf)}{Environment.NewLine}{Environment.NewLine}")
            End If
        Next

        Return versionForm.ShowDialog()

    End Function

    Public Function DownloadLatestUpdate(version As String, ByRef message As String) As Boolean

        Try
            Dim url As String = "https://github.com/siglr/DiscordPostHelper/releases/download/"
            Dim localZip As String = String.Empty
            Dim zipFileName As String = String.Empty

            'Discord Post Helper format example: https://github.com/siglr/DiscordPostHelper/releases/download/DPH.23.3.20.1/Discord.Post.Helper.23.3.20.1.zip
            'Soaring Task Browser format example: https://github.com/siglr/DiscordPostHelper/releases/download/STB.23.3.20.1/Soaring.Task.Browser.23.3.20.1.zip

            Select Case ClientRunning
                Case ClientApp.DiscordPostHelper
                    zipFileName = $"Discord.Post.Helper.{version}.zip"
                    url = $"{url}DPH.{version}/{zipFileName}"
                    localZip = $"{Application.StartupPath}\{zipFileName}"
                Case ClientApp.SoaringTaskBrowser
                    zipFileName = $"Soaring.Task.Browser.{version}.zip"
                    url = $"{url}STB.{version}/{zipFileName}"
                    localZip = $"{Application.StartupPath}\{zipFileName}"
            End Select

            message = $"Downloading file {url} to {localZip}"
            Dim client As New WebClient()
            client.DownloadFile(url, localZip)

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
        countryFlagCodes.Add("Algeria", ":flag_dz:")
        countryFlagCodes.Add("Andorra", ":flag_ad:")
        countryFlagCodes.Add("Angola", ":flag_ao:")
        countryFlagCodes.Add("Antigua and Barbuda", ":flag_ag:")
        countryFlagCodes.Add("Argentina", ":flag_ar:")
        countryFlagCodes.Add("Armenia", ":flag_am:")
        countryFlagCodes.Add("Australia", ":flag_au:")
        countryFlagCodes.Add("Austria", ":flag_at:")
        countryFlagCodes.Add("Azerbaijan", ":flag_az:")
        countryFlagCodes.Add("Bahamas", ":flag_bs:")
        countryFlagCodes.Add("Bahrain", ":flag_bh:")
        countryFlagCodes.Add("Bangladesh", ":flag_bd:")
        countryFlagCodes.Add("Barbados", ":flag_bb:")
        countryFlagCodes.Add("Belarus", ":flag_by:")
        countryFlagCodes.Add("Belgium", ":flag_be:")
        countryFlagCodes.Add("Belize", ":flag_bz:")
        countryFlagCodes.Add("Benin", ":flag_bj:")
        countryFlagCodes.Add("Bhutan", ":flag_bt:")
        countryFlagCodes.Add("Bolivia", ":flag_bo:")
        countryFlagCodes.Add("Bosnia and Herzegovina", ":flag_ba:")
        countryFlagCodes.Add("Botswana", ":flag_bw:")
        countryFlagCodes.Add("Brazil", ":flag_br:")
        countryFlagCodes.Add("Brunei", ":flag_bn:")
        countryFlagCodes.Add("Bulgaria", ":flag_bg:")
        countryFlagCodes.Add("Burkina Faso", ":flag_bf:")
        countryFlagCodes.Add("Burundi", ":flag_bi:")
        countryFlagCodes.Add("Cambodia", ":flag_kh:")
        countryFlagCodes.Add("Cameroon", ":flag_cm:")
        countryFlagCodes.Add("Canada", ":flag_ca:")
        countryFlagCodes.Add("Cape Verde", ":flag_cv:")
        countryFlagCodes.Add("Central African Republic", ":flag_cf:")
        countryFlagCodes.Add("Chad", ":flag_td:")
        countryFlagCodes.Add("Chile", ":flag_cl:")
        CountryFlagCodes.Add("China", ":flag_cn:")
        CountryFlagCodes.Add("Christmas Island", ":flag_cx:")
        countryFlagCodes.Add("Cocos Islands", ":flag_cc:")
        countryFlagCodes.Add("Colombia", ":flag_co:")
        countryFlagCodes.Add("Comoros", ":flag_km:")
        countryFlagCodes.Add("Congo", ":flag_cg:")
        countryFlagCodes.Add("Congo (Democratic Republic of the)", ":flag_cd:")
        countryFlagCodes.Add("Cook Islands", ":flag_ck:")
        countryFlagCodes.Add("Costa Rica", ":flag_cr:")
        countryFlagCodes.Add("Croatia", ":flag_hr:")
        countryFlagCodes.Add("Cuba", ":flag_cu:")
        countryFlagCodes.Add("Curaçao", ":flag_cw:")
        countryFlagCodes.Add("Cyprus", ":flag_cy:")
        countryFlagCodes.Add("Czech Republic", ":flag_cz:")
        countryFlagCodes.Add("Denmark", ":flag_dk:")
        countryFlagCodes.Add("Djibouti", ":flag_dj:")
        countryFlagCodes.Add("Dominica", ":flag_dm:")
        countryFlagCodes.Add("Dominican Republic", ":flag_do:")
        countryFlagCodes.Add("East Timor", ":flag_tl:")
        countryFlagCodes.Add("Ecuador", ":flag_ec:")
        countryFlagCodes.Add("Egypt", ":flag_eg:")
        countryFlagCodes.Add("El Salvador", ":flag_sv:")
        countryFlagCodes.Add("Equatorial Guinea", ":flag_gq:")
        countryFlagCodes.Add("Eritrea", ":flag_er:")
        countryFlagCodes.Add("Estonia", ":flag_ee:")
        countryFlagCodes.Add("Ethiopia", ":flag_et:")
        countryFlagCodes.Add("Falkland Islands", ":flag_fk:")
        countryFlagCodes.Add("Faroe Islands", ":flag_fo:")
        countryFlagCodes.Add("Fiji", ":flag_fj:")
        countryFlagCodes.Add("Finland", ":flag_fi:")
        countryFlagCodes.Add("France", ":flag_fr:")
        countryFlagCodes.Add("French Guiana", ":flag_gf:")
        countryFlagCodes.Add("French Polynesia", ":flag_pf:")
        countryFlagCodes.Add("French Southern Territories", ":flag_tf:")
        countryFlagCodes.Add("Gabon", ":flag_ga:")
        countryFlagCodes.Add("Gambia", ":flag_gm:")
        countryFlagCodes.Add("Georgia", ":flag_ge:")
        countryFlagCodes.Add("Germany", ":flag_de:")
        countryFlagCodes.Add("Ghana", ":flag_gh:")
        countryFlagCodes.Add("Gibraltar", ":flag_gi:")
        countryFlagCodes.Add("Greece", ":flag_gr:")
        countryFlagCodes.Add("Greenland", ":flag_gl:")
        countryFlagCodes.Add("Grenada", ":flag_gd:")
        countryFlagCodes.Add("Guadeloupe", ":flag_gp:")
        countryFlagCodes.Add("Guam", ":flag_gu:")
        countryFlagCodes.Add("Guatemala", ":flag_gt:")
        countryFlagCodes.Add("Guernsey", ":flag_gg:")
        countryFlagCodes.Add("Guinea", ":flag_gn:")
        countryFlagCodes.Add("Guinea-Bissau", ":flag_gw:")
        countryFlagCodes.Add("Guyana", ":flag_gy:")
        countryFlagCodes.Add("Haiti", ":flag_ht:")
        countryFlagCodes.Add("Honduras", ":flag_hn:")
        countryFlagCodes.Add("Hungary", ":flag_hu:")
        countryFlagCodes.Add("Iceland", ":flag_is:")
        countryFlagCodes.Add("India", ":flag_in:")
        countryFlagCodes.Add("Indonesia", ":flag_id:")
        countryFlagCodes.Add("Iran", ":flag_ir:")
        countryFlagCodes.Add("Iraq", ":flag_iq:")
        countryFlagCodes.Add("Ireland", ":flag_ie:")
        countryFlagCodes.Add("Israel", ":flag_il:")
        countryFlagCodes.Add("Italy", ":flag_it:")
        countryFlagCodes.Add("Jamaica", ":flag_jm:")
        countryFlagCodes.Add("Japan", ":flag_jp:")
        countryFlagCodes.Add("Jordan", ":flag_jo:")
        countryFlagCodes.Add("Kazakhstan", ":flag_kz:")
        countryFlagCodes.Add("Kenya", ":flag_ke:")
        countryFlagCodes.Add("Kiribati", ":flag_ki:")
        countryFlagCodes.Add("North Korea", ":flag_kp:")
        CountryFlagCodes.Add("Kuwait", ":flag_kw:")
        CountryFlagCodes.Add("Kyrgyzstan", ":flag_kg:")
        countryFlagCodes.Add("Laos", ":flag_la:")
        countryFlagCodes.Add("Latvia", ":flag_lv:")
        countryFlagCodes.Add("Lebanon", ":flag_lb:")
        countryFlagCodes.Add("Lesotho", ":flag_ls:")
        countryFlagCodes.Add("Liberia", ":flag_lr:")
        countryFlagCodes.Add("Libya", ":flag_ly:")
        countryFlagCodes.Add("Liechtenstein", ":flag_li:")
        countryFlagCodes.Add("Lithuania", ":flag_lt:")
        countryFlagCodes.Add("Luxembourg", ":flag_lu:")
        countryFlagCodes.Add("Macedonia", ":flag_mk:")
        countryFlagCodes.Add("Madagascar", ":flag_mg:")
        countryFlagCodes.Add("Malawi", ":flag_mw:")
        countryFlagCodes.Add("Malaysia", ":flag_my:")
        countryFlagCodes.Add("Maldives", ":flag_mv:")
        countryFlagCodes.Add("Mali", ":flag_ml:")
        countryFlagCodes.Add("Malta", ":flag_mt:")
        countryFlagCodes.Add("Marshall Islands", ":flag_mh:")
        countryFlagCodes.Add("Martinique", ":flag_mq:")
        countryFlagCodes.Add("Mauritania", ":flag_mr:")
        countryFlagCodes.Add("Mauritius", ":flag_mu:")
        countryFlagCodes.Add("Mayotte", ":flag_yt:")
        countryFlagCodes.Add("Mexico", ":flag_mx:")
        countryFlagCodes.Add("Micronesia", ":flag_fm:")
        countryFlagCodes.Add("Moldova", ":flag_md:")
        countryFlagCodes.Add("Monaco", ":flag_mc:")
        countryFlagCodes.Add("Mongolia", ":flag_mn:")
        countryFlagCodes.Add("Montenegro", ":flag_me:")
        countryFlagCodes.Add("Montserrat", ":flag_ms:")
        countryFlagCodes.Add("Morocco", ":flag_ma:")
        countryFlagCodes.Add("Mozambique", ":flag_mz:")
        countryFlagCodes.Add("Myanmar", ":flag_mm:")
        countryFlagCodes.Add("Namibia", ":flag_na:")
        countryFlagCodes.Add("Nauru", ":flag_nr:")
        countryFlagCodes.Add("Nepal", ":flag_np:")
        countryFlagCodes.Add("Netherlands", ":flag_nl:")
        countryFlagCodes.Add("New Caledonia", ":flag_nc:")
        countryFlagCodes.Add("New Zealand", ":flag_nz:")
        countryFlagCodes.Add("Nicaragua", ":flag_ni:")
        countryFlagCodes.Add("Niger", ":flag_ne:")
        countryFlagCodes.Add("Nigeria", ":flag_ng:")
        countryFlagCodes.Add("Niue", ":flag_nu:")
        countryFlagCodes.Add("Norfolk Island", ":flag_nf:")
        countryFlagCodes.Add("North Macedonia", ":flag_mk:")
        countryFlagCodes.Add("Northern Mariana Islands", ":flag_mp:")
        countryFlagCodes.Add("Norway", ":flag_no:")
        countryFlagCodes.Add("Oman", ":flag_om:")
        countryFlagCodes.Add("Pakistan", ":flag_pk:")
        countryFlagCodes.Add("Palau", ":flag_pw:")
        countryFlagCodes.Add("Palestine, State of", ":flag_ps:")
        countryFlagCodes.Add("Panama", ":flag_pa:")
        countryFlagCodes.Add("Papua New Guinea", ":flag_pg:")
        countryFlagCodes.Add("Paraguay", ":flag_py:")
        countryFlagCodes.Add("Peru", ":flag_pe:")
        countryFlagCodes.Add("Philippines", ":flag_ph:")
        countryFlagCodes.Add("Pitcairn", ":flag_pn:")
        countryFlagCodes.Add("Poland", ":flag_pl:")
        countryFlagCodes.Add("Portugal", ":flag_pt:")
        countryFlagCodes.Add("Puerto Rico", ":flag_pr:")
        countryFlagCodes.Add("Qatar", ":flag_qa:")
        countryFlagCodes.Add("Réunion", ":flag_re:")
        countryFlagCodes.Add("Romania", ":flag_ro:")
        countryFlagCodes.Add("Russia", ":flag_ru:")
        countryFlagCodes.Add("Rwanda", ":flag_rw:")
        countryFlagCodes.Add("Saint Kitts and Nevis", ":flag_kn:")
        countryFlagCodes.Add("Saint Lucia", ":flag_lc:")
        countryFlagCodes.Add("Saint Vincent and the Grenadines", ":flag_vc:")
        countryFlagCodes.Add("Samoa", ":flag_ws:")
        countryFlagCodes.Add("San Marino", ":flag_sm:")
        countryFlagCodes.Add("São Tomé and Príncipe", ":flag_st:")
        countryFlagCodes.Add("Saudi Arabia", ":flag_sa:")
        countryFlagCodes.Add("Senegal", ":flag_sn:")
        countryFlagCodes.Add("Serbia", ":flag_rs:")
        countryFlagCodes.Add("Seychelles", ":flag_sc:")
        countryFlagCodes.Add("Sierra Leone", ":flag_sl:")
        countryFlagCodes.Add("Singapore", ":flag_sg:")
        countryFlagCodes.Add("Slovakia", ":flag_sk:")
        countryFlagCodes.Add("Slovenia", ":flag_si:")
        countryFlagCodes.Add("Solomon Islands", ":flag_sb:")
        countryFlagCodes.Add("Somalia", ":flag_so:")
        countryFlagCodes.Add("South Africa", ":flag_za:")
        countryFlagCodes.Add("South Georgia and the South Sandwich Islands", ":flag_gs:")
        countryFlagCodes.Add("South Korea", ":flag_kr:")
        countryFlagCodes.Add("South Sudan", ":flag_ss:")
        countryFlagCodes.Add("Spain", ":flag_es:")
        countryFlagCodes.Add("Sri Lanka", ":flag_lk:")
        countryFlagCodes.Add("Sudan", ":flag_sd:")
        countryFlagCodes.Add("Suriname", ":flag_sr:")
        countryFlagCodes.Add("Svalbard and Jan Mayen", ":flag_sj:")
        countryFlagCodes.Add("Swaziland", ":flag_sz:")
        countryFlagCodes.Add("Sweden", ":flag_se:")
        countryFlagCodes.Add("Switzerland", ":flag_ch:")
        countryFlagCodes.Add("Syria", ":flag_sy:")
        countryFlagCodes.Add("Taiwan", ":flag_tw:")
        countryFlagCodes.Add("Tajikistan", ":flag_tj:")
        countryFlagCodes.Add("Tanzania", ":flag_tz:")
        countryFlagCodes.Add("Thailand", ":flag_th:")
        countryFlagCodes.Add("Timor-Leste", ":flag_tl:")
        countryFlagCodes.Add("Togo", ":flag_tg:")
        countryFlagCodes.Add("Tokelau", ":flag_tk:")
        countryFlagCodes.Add("Tonga", ":flag_to:")
        countryFlagCodes.Add("Trinidad and Tobago", ":flag_tt:")
        countryFlagCodes.Add("Tunisia", ":flag_tn:")
        countryFlagCodes.Add("Turkey", ":flag_tr:")
        countryFlagCodes.Add("Turkmenistan", ":flag_tm:")
        countryFlagCodes.Add("Turks and Caicos Islands", ":flag_tc:")
        countryFlagCodes.Add("Tuvalu", ":flag_tv:")
        countryFlagCodes.Add("Uganda", ":flag_ug:")
        countryFlagCodes.Add("Ukraine", ":flag_ua:")
        countryFlagCodes.Add("United Arab Emirates", ":flag_ae:")
        countryFlagCodes.Add("United Kingdom", ":flag_gb:")
        countryFlagCodes.Add("United States", ":flag_us:")
        countryFlagCodes.Add("United States Minor Outlying Islands", ":flag_um:")
        countryFlagCodes.Add("Uruguay", ":flag_uy:")
        countryFlagCodes.Add("Uzbekistan", ":flag_uz:")
        countryFlagCodes.Add("Vanuatu", ":flag_vu:")
        countryFlagCodes.Add("Vatican City", ":flag_va:")
        countryFlagCodes.Add("Venezuela", ":flag_ve:")
        countryFlagCodes.Add("Vietnam", ":flag_vn:")
        countryFlagCodes.Add("Virgin Islands, British", ":flag_vg:")
        countryFlagCodes.Add("Virgin Islands, U.S.", ":flag_vi:")
        countryFlagCodes.Add("Wallis and Futuna", ":flag_wf:")
        countryFlagCodes.Add("Western Sahara", ":flag_eh:")
        countryFlagCodes.Add("Yemen", ":flag_ye:")
        countryFlagCodes.Add("Zambia", ":flag_zm:")
        countryFlagCodes.Add("Zimbabwe", ":flag_zw:")

    End Sub

    Public Sub FillCountryFlagList(ByVal ctlControl As IList)

        ctlControl.Clear()
        For Each country As String In CountryFlagCodes.Keys
            ctlControl.Add(country)
        Next

    End Sub
End Class
