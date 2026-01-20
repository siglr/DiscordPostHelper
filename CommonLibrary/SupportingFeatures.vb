Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
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
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports Newtonsoft.Json.Linq

Public Class SupportingFeatures

    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"
    Private Const SW_RESTORE As Integer = 9
    Private Const MSFSSoaringToolsDiscordID As String = "1022705603489042472"
    Private Const MSFSSoaringToolsLibraryID As String = "1155511739799060552"
    Private Const MSFSWSGAnnouncementsDiscordID As String = "1266809849266831390"
    Private Const MSFSSoaringToolsPrivateTestingID As String = "1067288937527246868"
    Private Const MSFSSoaringToolsTestingID As String = "1067291857404571750"
    Private Const MSFSSoaringToolsEventsTestingID As String = "1067291913859891200"

    Public Enum DiscordTimeStampFormat As Integer
        TimeOnlyWithoutSeconds = 0
        FullDateTimeWithDayOfWeek = 1
        LongDateTime = 2
        CountDown = 3
        TimeStampOnly = 4
    End Enum

    Public Enum ClientApp As Integer
        DiscordPostHelper = 1
        SoaringTaskBrowser = 2
        DPHXUnpackAndLoad = 3
        TaskBrowserAdmin = 4
    End Enum

    Public Enum WSGTaskStatus As Integer
        NotCreated = 0
        PendingCreation = 10
        Inactive = 20
        Active = 99
    End Enum

    Public ReadOnly DefaultKnownClubEvents As New Dictionary(Of String, PresetEvent)
    Public ReadOnly DiscordNameIDPair As New Dictionary(Of String, String)
    Public ReadOnly KnownDesigners As New List(Of KnownDesignerInfo)
    Public ReadOnly AllWaypoints As New List(Of ATCWaypoint)
    Public AATMinDuration As TimeSpan = TimeSpan.Zero
    Public ReadOnly CountryFlagCodes As Dictionary(Of String, ValueTuple(Of String, String))
    Public ReadOnly CountryISO3166Codes As Dictionary(Of String, String)
    Private Shared _ClientRunning As ClientApp

    Public Shared Property PrefUnits As PreferredUnits

    Public Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    Public Declare Function GetSystemMetrics Lib "user32.dll" (ByVal nIndex As Integer) As Integer
    Public Const GWL_STYLE As Integer = (-16)
    Public Const WS_VSCROLL As Integer = &H200000
    Public Const WS_HSCROLL As Integer = &H100000

    Public Shared Function WaitForProcessExit(processName As String, timeoutMs As Integer, Optional checkIntervalMs As Integer = 100) As Boolean

        Dim sw = Stopwatch.StartNew()
        While sw.ElapsedMilliseconds < timeoutMs
            If Not Process.GetProcessesByName(processName).Any() Then
                Return True
            End If
            Threading.Thread.Sleep(checkIntervalMs)
        End While

        Return Not Process.GetProcessesByName(processName).Any()

    End Function

    Public Shared Function TaskLibraryDiscordURL(Optional testMode As Boolean = False) As String
        If testMode Then
            Return $"https://discord.com/channels/{MSFSSoaringToolsDiscordID}/{MSFSSoaringToolsTestingID}"
        End If
        If Not useTestServer Then
            Return $"https://discord.com/channels/{MSFSSoaringToolsDiscordID}/{MSFSSoaringToolsLibraryID}"
        Else
            Return $"https://discord.com/channels/{MSFSSoaringToolsDiscordID}/{MSFSSoaringToolsPrivateTestingID}"
        End If
    End Function

    Public Shared Function TestEventsDiscordURL() As String
        Return $"https://discord.com/channels/{MSFSSoaringToolsDiscordID}/{MSFSSoaringToolsEventsTestingID}"
    End Function

    Public Shared Function GetWeatherPresetTitleFromFile(weatherFilePath As String) As String
        If String.IsNullOrWhiteSpace(weatherFilePath) OrElse Not File.Exists(weatherFilePath) Then
            Return String.Empty
        End If

        Dim presetTitle As String = String.Empty
        Dim xmlDocWeatherPreset As New XmlDocument()
        xmlDocWeatherPreset.Load(weatherFilePath)
        Dim weatherDetails As WeatherDetails = New WeatherDetails(xmlDocWeatherPreset)
        presetTitle = weatherDetails.PresetName

        weatherDetails = Nothing
        xmlDocWeatherPreset = Nothing

        Return presetTitle
    End Function

    Public Shared Sub CenterFormOnOwner(ownerForm As Form, childForm As Form)
        'Center on parent form
        Dim ownerRect = If(ownerForm IsNot Nothing, ownerForm.Bounds, Screen.FromControl(childForm).WorkingArea)
        childForm.Location = New Point(ownerRect.Left + (ownerRect.Width - childForm.Width) \ 2, ownerRect.Top + (ownerRect.Height - childForm.Height) \ 2)
    End Sub

    Public Shared Function GetTaskThreadURLFromGroupURL(discordMessageUrl As String) As String
        Try
            ' Split the URL by slashes
            Dim urlParts As String() = discordMessageUrl.Split("/"c)

            ' Ensure the URL has the expected structure
            If urlParts.Length >= 7 Then
                ' Reconstruct the URL without the second ID (6th part is the thread ID)
                Dim baseUrl As String = $"{urlParts(0)}//{urlParts(2)}/channels/{urlParts(4)}/{urlParts(6)}"
                Return baseUrl
            Else
                ' Return the original URL if the structure is not as expected
                Return discordMessageUrl
            End If
        Catch ex As Exception
            ' Handle any errors gracefully and return the original URL
            Return discordMessageUrl
        End Try
    End Function

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

        CountryISO3166Codes = New Dictionary(Of String, String)
        GetCountryISO3166Codes()

        CountryFlagCodes = New Dictionary(Of String, ValueTuple(Of String, String))
        GetCountryFlagCodes()

    End Sub

    Private Sub LoadDefaultClubEvents()

        Try
            Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveSoaringClubs.php"
            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "GET"

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    If result("status").ToString() = "success" Then
                        Dim clubs As JArray = JArray.Parse(result("clubs").ToString())

                        For Each club As JObject In clubs
                            ' Get SharedPublishers as a List(Of String)
                            Dim sharedPublishers As New List(Of String)
                            If club("SharedPublishers") IsNot Nothing Then
                                For Each publisher As JToken In club("SharedPublishers")
                                    sharedPublishers.Add(publisher.ToString())
                                Next
                            End If
                            ' Get AuthorizedPublishers as a List(Of String)
                            Dim authorizedPublishers As New List(Of String)
                            If club("AuthorizedPublishers") IsNot Nothing Then
                                For Each publisher As JToken In club("AuthorizedPublishers")
                                    authorizedPublishers.Add(publisher.ToString())
                                Next
                            End If

                            Dim presetEvent As New PresetEvent(
                                club("ClubId").ToString(),
                                club("ClubName").ToString(),
                                club("ClubFullName").ToString(),
                                club("TrackerGroup").ToString(),
                                club("Emoji").ToString(),
                                club("EmojiID").ToString(),
                                club("EventNewsID").ToString(),
                                club("MSFSServer").ToString(),
                                club("VoiceChannel").ToString(),
                                club("TimeZoneID").ToString(),
                                [Enum].Parse(GetType(DayOfWeek), club("ZuluDayOfWeek").ToString()),
                                DateTime.Parse(club("ZuluTime").ToString()),
                                [Enum].Parse(GetType(DayOfWeek), club("SummerZuluDayOfWeek").ToString()),
                                DateTime.Parse(club("SummerZuluTime").ToString()),
                                Integer.Parse(club("SyncFlyDelay").ToString()),
                                Integer.Parse(club("LaunchDelay").ToString()),
                                Integer.Parse(club("StartTaskDelay").ToString()),
                                Boolean.Parse(club("EligibleAward").ToString()),
                                club("BeginnerLink").ToString(),
                                club("BeginnerLinkURL").ToString(),
                                Boolean.Parse(club("ForceSyncFly").ToString()),
                                Boolean.Parse(club("ForceLaunch").ToString()),
                                Boolean.Parse(club("ForceStartTask").ToString()),
                                club("DiscordURL").ToString(),
                                Integer.Parse(club("Language").ToString()),
                                club("MeetMessage").ToString(),
                                club("NoSyncMessage").ToString(),
                                club("SyncMessage").ToString(),
                                club("LaunchMessage").ToString(),
                                club("StartMessage").ToString(),
                                sharedPublishers,
                                authorizedPublishers
                                )
                            DefaultKnownClubEvents.Add(presetEvent.ClubId, presetEvent)
                            If Not DiscordNameIDPair.ContainsKey(presetEvent.Emoji) Then
                                DiscordNameIDPair.Add(presetEvent.Emoji, presetEvent.EmojiID)
                            End If
                        Next

                        ' Load Known Designers (extended version)
                        Dim designers As JArray = JArray.Parse(result("designersExtended").ToString())
                        KnownDesigners.Clear()
                        For Each designer As JObject In designers
                            Dim d As New KnownDesignerInfo()
                            d.Name = designer("Name").ToString()
                            d.DiscordID = designer("DiscordID").ToString()
                            KnownDesigners.Add(d)

                            Dim aboutName As String = d.Name.Substring(0, d.Name.LastIndexOf("#")).Trim.ToLower
                            If Not DiscordNameIDPair.ContainsKey(aboutName) Then
                                DiscordNameIDPair.Add(aboutName, d.DiscordID)
                            End If

                        Next
                    Else
                        Throw New Exception("Error retrieving soaring clubs: " & result("message").ToString())
                    End If
                End Using
            End Using
        Catch ex As Exception
            Throw New Exception("Failed to retrieve soaring clubs: " & ex.Message)
        End Try
    End Sub

    Public Function ListOfAllTrackerGroups() As List(Of String)

        Dim theList As New List(Of String)

        For Each club As PresetEvent In DefaultKnownClubEvents.Values
            If Not theList.Contains(club.TrackerGroup) Then
                theList.Add(club.TrackerGroup)
            End If
        Next

        Return theList

    End Function

    Public Sub PopulateSoaringClubList(ByVal ctlControl As IList)

        ctlControl.Clear()

        For Each clubID As String In DefaultKnownClubEvents.Keys
            ctlControl.Add(clubID)
        Next

    End Sub

    Public Sub PopulateKnownDesignersList(ByVal ctlControl As IList)
        ctlControl.Clear()

        For Each designer As KnownDesignerInfo In KnownDesigners
            ctlControl.Add(designer.Name)
        Next
    End Sub

    Public Function RoundTo15Minutes(ByVal minutes As Integer) As Integer
        Return Math.Ceiling(minutes / 15.0) * 15
    End Function

    Public Shared Function GetDistance(totalDistanceKm As String,
                                       trackDistanceKm As String,
                                       Optional prefUnits As PreferredUnits = Nothing,
                                       Optional taskTxt As String = "task",
                                       Optional totalTxt As String = "total") As String

        Dim totalDistKm As Decimal
        Dim trackDistKm As Decimal
        Dim totalDistMiles As Decimal
        Dim trackDistMiles As Decimal

        Decimal.TryParse(totalDistanceKm, totalDistKm)
        Decimal.TryParse(trackDistanceKm, trackDistKm)
        totalDistMiles = Conversions.KmToMiles(totalDistKm)
        trackDistMiles = Conversions.KmToMiles(trackDistKm)

        If prefUnits Is Nothing OrElse prefUnits.Distance = PreferredUnits.DistanceUnits.Both Then
            Return String.Format($"{totalDistKm:N0} km {totalTxt} ({trackDistKm:N0} km {taskTxt}) / {totalDistMiles:N0} mi {totalTxt} ({trackDistMiles:N0} mi {taskTxt})")
        Else
            Select Case prefUnits.Distance
                Case DistanceUnits.Imperial
                    Return String.Format($"{totalDistMiles:N0} mi {totalTxt} ({trackDistMiles:N0} mi {taskTxt})")
                Case DistanceUnits.Metric
                    Return String.Format($"{totalDistKm:N0} km {totalTxt} ({trackDistKm:N0} km {taskTxt})")
            End Select
        End If

        Return String.Empty

    End Function

    Public Shared Function GetDuration(durationMin As String,
                                       durationMax As String,
                                       Optional notSpecifiedTxt As String = "Not specified",
                                       Optional arroundTxt As String = "Around",
                                       Optional minutesTxt As String = "minutes",
                                       Optional toTxt As String = "to") As String

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

        If minMinutes = 0 AndAlso maxMinutes = 0 Then
            Return $"{notSpecifiedTxt}"
        ElseIf minMinutes > 0 AndAlso maxMinutes = 0 Then
            Return $"{arroundTxt} {minMinutes} {minutesTxt} ({minHoursH:D2}{minHoursM:D2})"
        ElseIf (minMinutes = 0 AndAlso maxMinutes > 0) OrElse (minMinutes = maxMinutes) Then
            Return $"{arroundTxt} {maxMinutes} {minutesTxt} ({maxHoursH:D2}{maxHoursM:D2})"
        Else
            Return $"{minMinutes} {toTxt} {maxMinutes} {minutesTxt} ({minHoursH:D2}{minHoursM:D2} {toTxt} {maxHoursH:D2}{maxHoursM:D2})"
        End If


    End Function

    Public Shared Function GetDifficulty(difficultyIndex As Integer, difficultyExtraInfo As String, Optional textOnly As Boolean = False) As String
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
                    difficulty = $"Experienced{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
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
                    difficulty = $"★★★☆☆ - Experienced{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 4
                    difficulty = $"★★★★☆ - Professional{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
                Case 5
                    difficulty = $"★★★★★ - Champion{ValueToAppendIfNotEmpty(difficultyExtraInfo, True, True)}"
            End Select
        End If
        Return difficulty

    End Function

    Public Shared Function ValueToAppendIfNotEmpty(textValue As String, Optional addSpace As Boolean = False, Optional useBrackets As Boolean = False, Optional nbrLineFeed As Integer = 0) As String

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

    Public Sub GetTaskBoundaries(ByRef minLongitude As Double, ByRef maxLongitude As Double, ByRef minLatitude As Double, ByRef maxLatitude As Double)
        ' Initialize the boundary variables with extreme values
        minLongitude = Double.MaxValue
        maxLongitude = Double.MinValue
        minLatitude = Double.MaxValue
        maxLatitude = Double.MinValue

        ' Loop through each waypoint in the list
        For Each waypoint As ATCWaypoint In AllWaypoints
            ' Update min and max longitude
            If waypoint.Longitude < minLongitude Then
                minLongitude = waypoint.Longitude
            End If

            If waypoint.Longitude > maxLongitude Then
                maxLongitude = waypoint.Longitude
            End If

            ' Update min and max latitude
            If waypoint.Latitude < minLatitude Then
                minLatitude = waypoint.Latitude
            End If

            If waypoint.Latitude > maxLatitude Then
                maxLatitude = waypoint.Latitude
            End If
        Next
    End Sub

    Public Function BuildAltitudeRestrictions(ByVal pXmlDocFlightPlan As XmlDocument,
                                              ByRef pFlightTotalDistanceInKm As Single,
                                              ByRef pTaskTotalDistanceInKm As Single,
                                              ByRef pPossibleElevationUpdateRequired As Boolean,
                                              Optional includeWPName As Boolean = True) As String

        'Build altitude restrictions
        Dim previousATCWaypoing As ATCWaypoint = Nothing
        Dim strRestrictions As String = String.Empty
        Dim blnInTask As Boolean = False
        Dim dblDistanceToPrevious As Single = 0
        AATMinDuration = TimeSpan.Zero

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

            If atcWaypoint.PossibleElevationUpdateReq Then
                pPossibleElevationUpdateRequired = True
            End If
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
                If atcWaypoint.IsAAT AndAlso atcWaypoint.AATMinDuration > TimeSpan.Zero Then
                    AATMinDuration = atcWaypoint.AATMinDuration
                End If
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

        ' Get the current culture's short time pattern and remove seconds if present.
        Dim timePattern As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern
        timePattern = timePattern.Replace(":ss", "").Replace(":s", "")

        ' Get the current culture's long date pattern.
        Dim datePattern As String = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern
        ' If the pattern doesn't already include the day-of-week, prepend it.
        If Not datePattern.Contains("dddd") Then
            datePattern = "dddd, " & datePattern
        End If

        If useUTC Then
            dateTimeInZulu = dateTimeToUse
            dateTimeInLocal = Conversions.ConvertUTCToLocal(dateTimeInZulu)
            result = $"{dateTimeInLocal.ToString(datePattern)} {dateTimeInLocal.ToString(timePattern)} Local"
            eventDay = dateTimeInLocal.DayOfWeek
        Else
            dateTimeInLocal = dateTimeToUse
            dateTimeInZulu = Conversions.ConvertLocalToUTC(dateTimeInLocal)
            result = $"{dateTimeInZulu.ToString(datePattern)} {dateTimeInZulu.ToString(timePattern)} UTC"
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

    Public Function FindPreviousDate(startDate As DateTime, dayOfWeek As DayOfWeek, clubDefaultMeetTime As DateTime) As DateTime
        Dim previousDate As DateTime = Conversions.ConvertLocalToUTC(startDate)
        Dim previousDateFound As Boolean = False

        ' If today, check if time is after event start
        If previousDate.DayOfWeek = dayOfWeek Then
            If previousDate.TimeOfDay > clubDefaultMeetTime.TimeOfDay Then
                previousDateFound = True
            End If
        End If

        ' Find the previous occurrence of the specified day
        While Not previousDateFound
            previousDate = previousDate.AddDays(-1)
            If previousDate.DayOfWeek = dayOfWeek Then
                previousDateFound = True
            End If
        End While

        Return previousDate
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
                formatAbbr = "R"
            Case DiscordTimeStampFormat.TimeStampOnly
                formatAbbr = String.Empty
        End Select

        If Not formatAbbr = String.Empty Then
            Return $"<t:{Conversions.ConvertDateToUnixTimestamp(dateToUse)}:{formatAbbr}>"
        Else
            Return $"{Conversions.ConvertDateToUnixTimestamp(dateToUse)}"
        End If

    End Function

    Public Sub UploadFile(folderName As String, fileName As String, xmlString As String)

        Dim request As WebRequest = WebRequest.Create($"{SIGLRDiscordPostHelperFolder()}SaveFileUnderTempFolder.php")
        request.Method = "POST"
        Dim postData As String = $"xmlString={HttpUtility.UrlEncode(xmlString)}&folderName={HttpUtility.UrlEncode(folderName)}&fileName={HttpUtility.UrlEncode(fileName)}"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
#If DEBUG Then
        Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
#End If
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
#If DEBUG Then
        Console.WriteLine(responseFromServer)
#End If
        reader.Close()
        dataStream.Close()
        response.Close()

        ' Output the response to the console
#If DEBUG Then
        Console.WriteLine(responseFromServer)
#End If

    End Sub

    Public Sub UploadTextFile(folderName As String, fileName As String, textContent As String)

        Dim request As WebRequest = WebRequest.Create($"{SIGLRDiscordPostHelperFolder()}SaveTextFileUnderTempFolder.php")
        request.Method = "POST"
        Dim postData As String = $"textContent={HttpUtility.UrlEncode(textContent)}&folderName={HttpUtility.UrlEncode(folderName)}&fileName={HttpUtility.UrlEncode(fileName)}"
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Using dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
        End Using

        Using response As WebResponse = request.GetResponse()
            Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
            Using reader As New StreamReader(response.GetResponseStream())
                Dim responseFromServer As String = reader.ReadToEnd()
                Console.WriteLine(responseFromServer)
            End Using
        End Using
    End Sub


    Public Sub UploadDirectFile(folderName As String, filePath As String)

        Dim request As HttpWebRequest = CType(WebRequest.Create($"{SIGLRDiscordPostHelperFolder()}SaveDirectFileUnderTempFolder.php"), HttpWebRequest)
        request.Method = "POST"

        ' Boundary string and content type for multipart/form-data
        Dim boundary As String = "---------------------------" + DateTime.Now.Ticks.ToString("x")
        request.ContentType = "multipart/form-data; boundary=" + boundary

        ' Start preparing the request body
        Dim requestBody As New StringBuilder()

        ' Add folderName part
        requestBody.AppendLine("--" + boundary)
        requestBody.AppendLine("Content-Disposition: form-data; name=""folderName""")
        requestBody.AppendLine()
        requestBody.AppendLine(HttpUtility.UrlEncode(folderName))

        ' Add fileName part
        Dim fileName As String = Path.GetFileName(filePath)
        requestBody.AppendLine("--" + boundary)
        requestBody.AppendLine($"Content-Disposition: form-data; name=""fileName""")
        requestBody.AppendLine()
        requestBody.AppendLine(HttpUtility.UrlEncode(fileName))

        ' Add file content part
        requestBody.AppendLine("--" + boundary)
        requestBody.AppendLine($"Content-Disposition: form-data; name=""file""; filename=""{fileName}""")
        requestBody.AppendLine("Content-Type: application/octet-stream")
        requestBody.AppendLine()

        Dim postHeader As String = requestBody.ToString()
        Dim postHeaderBytes As Byte() = Encoding.UTF8.GetBytes(postHeader)

        ' Get the file content
        Dim fileContent As Byte() = File.ReadAllBytes(filePath)
        Dim boundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" + boundary + "--" + vbCrLf)

        ' Calculate the total length of the request body
        Dim contentLength As Long = postHeaderBytes.Length + fileContent.Length + boundaryBytes.Length

        ' Set the content length of the request
        request.ContentLength = contentLength

        ' Write data to the request stream
        Using requestStream As Stream = request.GetRequestStream()
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length)
            requestStream.Write(fileContent, 0, fileContent.Length)
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
        End Using

        ' Get the response from the server
        Using response As WebResponse = request.GetResponse()
            Using reader As New StreamReader(response.GetResponseStream())
                Dim responseFromServer As String = reader.ReadToEnd()
                Console.WriteLine(responseFromServer)
            End Using
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
                    If wp.IsAAT Then
                        sb.AppendLine($"- {wp.WaypointName}: {wp.Latitude:0.000000} {wp.Longitude:0.000000} (AAT)")
                    Else
                        sb.AppendLine($"- {wp.WaypointName}: {wp.Latitude:0.000000} {wp.Longitude:0.000000}")
                    End If
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

    Public Function UnpackDPHXFile(ByVal dphxFilePath As String, Optional ByRef dphxUnpackFolder As String = "") As String

        Dim folderToUnpackDialog As New FolderBrowserDialog

        If dphxUnpackFolder = String.Empty OrElse (Not Directory.Exists(dphxUnpackFolder)) Then
            folderToUnpackDialog.SelectedPath = Path.GetDirectoryName(Path.GetFullPath(dphxFilePath))
        Else
            folderToUnpackDialog.SelectedPath = dphxUnpackFolder
        End If
        folderToUnpackDialog.Description = $"Select folder where to unpack the session package ""{Path.GetFileName(dphxFilePath)}"""
        folderToUnpackDialog.ShowNewFolderButton = True

        Dim result As DialogResult = folderToUnpackDialog.ShowDialog()

        If result = DialogResult.Cancel Then
            Return String.Empty
        End If

        Dim overwriteResult As DialogResult
        If Directory.Exists(folderToUnpackDialog.SelectedPath) Then
            'Folder exists - files may be overwritten
            Using New Centered_MessageBox()
                overwriteResult = MessageBox.Show("Existing files in this folder may get overwritten by the ones in the package, do you want to confirm each individual file?", $"Unpacking to {folderToUnpackDialog.SelectedPath}", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            End Using
            If overwriteResult = DialogResult.Cancel Then
                Return String.Empty
            End If
        End If

        dphxUnpackFolder = folderToUnpackDialog.SelectedPath

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
                        Using New Centered_MessageBox()
                            individualFileOverwrite = MessageBox.Show($"File {entry.Name} already exists - do you want to overwrite?", "Confirm file overwrite", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                        End Using
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

    Public Shared Function DeleteFolderAndFiles(ByVal folderToDelete As String, Optional ByVal recursively As Boolean = False) As Boolean

        If Not CleanupDPHXTempFolder(folderToDelete) Then
            Return False
        End If

        Try
            Directory.Delete(folderToDelete, recursively)
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Shared Function ContainsDifferentExtensions(folderPath As String, allowedExtension As String) As Boolean
        Try
            ' Ensure the extension starts with a dot
            If Not allowedExtension.StartsWith(".") Then
                allowedExtension = "." & allowedExtension
            End If

            ' Get all files in the folder
            For Each file As String In Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                If Path.GetExtension(file).ToLower() <> allowedExtension.ToLower() Then
                    Return True ' Found a file with a different extension
                End If
            Next

            ' If no different extension was found, return False
            Return False

        Catch ex As Exception
            Console.WriteLine("Error while checking folder: " & ex.Message)
            Return False
        End Try
    End Function

    Public Shared Function CleanupDPHXTempFolder(ByVal unpackFolder As String, Optional includeDroppedFolder As Boolean = False) As Boolean

        Dim success As Boolean = True

        success = CleanupFolderFiles(unpackFolder, success)

        If includeDroppedFolder Then
            Dim droppedFolder = Path.Combine(Path.GetTempPath(), "DPHXDropped")
            success = CleanupFolderFiles(droppedFolder, success)
        End If

        Return success

    End Function

    Private Shared Function CleanupFolderFiles(folder As String, currentSuccess As Boolean) As Boolean

        Dim success = currentSuccess

        If Directory.Exists(folder) Then
            Dim files As String() = Directory.GetFiles(folder)
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

    Public Function FixWPRFormat(wprFilename As String, expandSelfClosingTags As Boolean) As Boolean
        Try
            ' --- Step 1: Remove <ComputeWindFromDeparture> if present ---
            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(wprFilename)

            Dim nodesToRemove As XmlNodeList = xmlDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/ComputeWindFromDeparture")
            If nodesToRemove IsNot Nothing Then
                For Each node As XmlNode In nodesToRemove
                    node.ParentNode.RemoveChild(node)
                Next

                ' Save changes after removal
                xmlDoc.Save(wprFilename)
            End If

            ' --- Step 2: Optionally convert self-closing tags to explicit open/close form ---
            If expandSelfClosingTags Then
                Dim content As String = File.ReadAllText(wprFilename)

                Dim selfClosingTagRegex As New Regex("<([a-zA-Z0-9_]+)([^>]*)\s*/>", RegexOptions.Compiled)

                Dim updatedContent As String = selfClosingTagRegex.Replace(content, Function(m)
                                                                                        Dim tagName = m.Groups(1).Value
                                                                                        Dim attributes = m.Groups(2).Value.Trim()
                                                                                        If attributes.Length > 0 Then
                                                                                            Return $"<{tagName} {attributes}></{tagName}>"
                                                                                        Else
                                                                                            Return $"<{tagName}></{tagName}>"
                                                                                        End If
                                                                                    End Function)

                File.WriteAllText(wprFilename, updatedContent)
            End If

            Return True

        Catch ex As Exception
            ' Optional: log or display error
            Return False
        End Try
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
                Using New Centered_MessageBox()
                    MessageBox.Show("It appears it is impossible to retrieve version information right now. You will have to manually check for the latest version.", "Checking latest version", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
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
        Dim url As String = $"{SIGLRDiscordPostHelperFolder()}DPHGetVersionInfo.php?param={Uri.EscapeDataString(parameter)}"
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

    Public Sub GetCountryISO3166Codes()
        CountryISO3166Codes.Add("ABW", "Aruba")
        CountryISO3166Codes.Add("AFG", "Afghanistan")
        CountryISO3166Codes.Add("AGO", "Angola")
        CountryISO3166Codes.Add("AIA", "Anguilla")
        CountryISO3166Codes.Add("ALA", "Åland Islands")
        CountryISO3166Codes.Add("ALB", "Albania")
        CountryISO3166Codes.Add("AND", "Andorra")
        CountryISO3166Codes.Add("ARE", "United Arab Emirates")
        CountryISO3166Codes.Add("ARG", "Argentina")
        CountryISO3166Codes.Add("ARM", "Armenia")
        CountryISO3166Codes.Add("ASM", "American Samoa")
        CountryISO3166Codes.Add("ATA", "Antarctica")
        CountryISO3166Codes.Add("ATF", "French Southern Territories")
        CountryISO3166Codes.Add("ATG", "Antigua and Barbuda")
        CountryISO3166Codes.Add("AUS", "Australia")
        CountryISO3166Codes.Add("AUT", "Austria")
        CountryISO3166Codes.Add("AZE", "Azerbaijan")
        CountryISO3166Codes.Add("BDI", "Burundi")
        CountryISO3166Codes.Add("BEL", "Belgium")
        CountryISO3166Codes.Add("BEN", "Benin")
        CountryISO3166Codes.Add("BES", "Bonaire, Sint Eustatius and Saba")
        CountryISO3166Codes.Add("BFA", "Burkina Faso")
        CountryISO3166Codes.Add("BGD", "Bangladesh")
        CountryISO3166Codes.Add("BGR", "Bulgaria")
        CountryISO3166Codes.Add("BHR", "Bahrain")
        CountryISO3166Codes.Add("BHS", "Bahamas")
        CountryISO3166Codes.Add("BIH", "Bosnia and Herzegovina")
        CountryISO3166Codes.Add("BLM", "Saint Barthélemy")
        CountryISO3166Codes.Add("BLR", "Belarus")
        CountryISO3166Codes.Add("BLZ", "Belize")
        CountryISO3166Codes.Add("BMU", "Bermuda")
        CountryISO3166Codes.Add("BOL", "Bolivia")
        CountryISO3166Codes.Add("BRA", "Brazil")
        CountryISO3166Codes.Add("BRB", "Barbados")
        CountryISO3166Codes.Add("BRN", "Brunei")
        CountryISO3166Codes.Add("BTN", "Bhutan")
        CountryISO3166Codes.Add("BVT", "Bouvet Island")
        CountryISO3166Codes.Add("BWA", "Botswana")
        CountryISO3166Codes.Add("CAF", "Central African Republic")
        CountryISO3166Codes.Add("CAN", "Canada")
        CountryISO3166Codes.Add("CCK", "Cocos Islands")
        CountryISO3166Codes.Add("CHE", "Switzerland")
        CountryISO3166Codes.Add("CHL", "Chile")
        CountryISO3166Codes.Add("CHN", "China")
        CountryISO3166Codes.Add("CIV", "Ivory Coast")
        CountryISO3166Codes.Add("CMR", "Cameroon")
        CountryISO3166Codes.Add("COD", "Congo (Democratic Republic of the)")
        CountryISO3166Codes.Add("COG", "Congo")
        CountryISO3166Codes.Add("COK", "Cook Islands")
        CountryISO3166Codes.Add("COL", "Colombia")
        CountryISO3166Codes.Add("COM", "Comoros")
        CountryISO3166Codes.Add("CPV", "Cape Verde")
        CountryISO3166Codes.Add("CRI", "Costa Rica")
        CountryISO3166Codes.Add("CUB", "Cuba")
        CountryISO3166Codes.Add("CUW", "Curaçao")
        CountryISO3166Codes.Add("CXR", "Christmas Island")
        CountryISO3166Codes.Add("CYM", "Cayman Islands")
        CountryISO3166Codes.Add("CYP", "Cyprus")
        CountryISO3166Codes.Add("CZE", "Czech Republic")
        CountryISO3166Codes.Add("DEU", "Germany")
        CountryISO3166Codes.Add("DJI", "Djibouti")
        CountryISO3166Codes.Add("DMA", "Dominica")
        CountryISO3166Codes.Add("DNK", "Denmark")
        CountryISO3166Codes.Add("DOM", "Dominican Republic")
        CountryISO3166Codes.Add("DZA", "Algeria")
        CountryISO3166Codes.Add("ECU", "Ecuador")
        CountryISO3166Codes.Add("EGY", "Egypt")
        CountryISO3166Codes.Add("ERI", "Eritrea")
        CountryISO3166Codes.Add("ESH", "Western Sahara")
        CountryISO3166Codes.Add("ESP", "Spain")
        CountryISO3166Codes.Add("EST", "Estonia")
        CountryISO3166Codes.Add("ETH", "Ethiopia")
        CountryISO3166Codes.Add("FIN", "Finland")
        CountryISO3166Codes.Add("FJI", "Fiji")
        CountryISO3166Codes.Add("FLK", "Falkland Islands")
        CountryISO3166Codes.Add("FRA", "France")
        CountryISO3166Codes.Add("FRO", "Faroe Islands")
        CountryISO3166Codes.Add("FSM", "Micronesia")
        CountryISO3166Codes.Add("GAB", "Gabon")
        CountryISO3166Codes.Add("GBR", "United Kingdom")
        CountryISO3166Codes.Add("GEO", "Georgia")
        CountryISO3166Codes.Add("GGY", "Guernsey")
        CountryISO3166Codes.Add("GHA", "Ghana")
        CountryISO3166Codes.Add("GIB", "Gibraltar")
        CountryISO3166Codes.Add("GIN", "Guinea")
        CountryISO3166Codes.Add("GLP", "Guadeloupe")
        CountryISO3166Codes.Add("GMB", "Gambia")
        CountryISO3166Codes.Add("GNB", "Guinea-Bissau")
        CountryISO3166Codes.Add("GNQ", "Equatorial Guinea")
        CountryISO3166Codes.Add("GRC", "Greece")
        CountryISO3166Codes.Add("GRD", "Grenada")
        CountryISO3166Codes.Add("GRL", "Greenland")
        CountryISO3166Codes.Add("GTM", "Guatemala")
        CountryISO3166Codes.Add("GUF", "French Guiana")
        CountryISO3166Codes.Add("GUM", "Guam")
        CountryISO3166Codes.Add("GUY", "Guyana")
        CountryISO3166Codes.Add("HKG", "Hong Kong")
        CountryISO3166Codes.Add("HMD", "Heard Island and McDonald Islands")
        CountryISO3166Codes.Add("HND", "Honduras")
        CountryISO3166Codes.Add("HRV", "Croatia")
        CountryISO3166Codes.Add("HTI", "Haiti")
        CountryISO3166Codes.Add("HUN", "Hungary")
        CountryISO3166Codes.Add("IDN", "Indonesia")
        CountryISO3166Codes.Add("IMN", "Isle of Man")
        CountryISO3166Codes.Add("IND", "India")
        CountryISO3166Codes.Add("IOT", "British Indian Ocean Territory")
        CountryISO3166Codes.Add("IRL", "Ireland")
        CountryISO3166Codes.Add("IRN", "Iran")
        CountryISO3166Codes.Add("IRQ", "Iraq")
        CountryISO3166Codes.Add("ISL", "Iceland")
        CountryISO3166Codes.Add("ISR", "Israel")
        CountryISO3166Codes.Add("ITA", "Italy")
        CountryISO3166Codes.Add("JAM", "Jamaica")
        CountryISO3166Codes.Add("JEY", "Jersey")
        CountryISO3166Codes.Add("JOR", "Jordan")
        CountryISO3166Codes.Add("JPN", "Japan")
        CountryISO3166Codes.Add("KAZ", "Kazakhstan")
        CountryISO3166Codes.Add("KEN", "Kenya")
        CountryISO3166Codes.Add("KGZ", "Kyrgyzstan")
        CountryISO3166Codes.Add("KHM", "Cambodia")
        CountryISO3166Codes.Add("KIR", "Kiribati")
        CountryISO3166Codes.Add("KNA", "Saint Kitts and Nevis")
        CountryISO3166Codes.Add("KOR", "South Korea")
        CountryISO3166Codes.Add("KWT", "Kuwait")
        CountryISO3166Codes.Add("LAO", "Laos")
        CountryISO3166Codes.Add("LBN", "Lebanon")
        CountryISO3166Codes.Add("LBR", "Liberia")
        CountryISO3166Codes.Add("LBY", "Libya")
        CountryISO3166Codes.Add("LCA", "Saint Lucia")
        CountryISO3166Codes.Add("LIE", "Liechtenstein")
        CountryISO3166Codes.Add("LKA", "Sri Lanka")
        CountryISO3166Codes.Add("LSO", "Lesotho")
        CountryISO3166Codes.Add("LTU", "Lithuania")
        CountryISO3166Codes.Add("LUX", "Luxembourg")
        CountryISO3166Codes.Add("LVA", "Latvia")
        CountryISO3166Codes.Add("MAC", "Macao")
        CountryISO3166Codes.Add("MAF", "Saint Martin (French part)")
        CountryISO3166Codes.Add("MAR", "Morocco")
        CountryISO3166Codes.Add("MCO", "Monaco")
        CountryISO3166Codes.Add("MDA", "Moldova")
        CountryISO3166Codes.Add("MDG", "Madagascar")
        CountryISO3166Codes.Add("MDV", "Maldives")
        CountryISO3166Codes.Add("MEX", "Mexico")
        CountryISO3166Codes.Add("MHL", "Marshall Islands")
        CountryISO3166Codes.Add("MKD", "Macedonia")
        CountryISO3166Codes.Add("MLI", "Mali")
        CountryISO3166Codes.Add("MLT", "Malta")
        CountryISO3166Codes.Add("MMR", "Myanmar")
        CountryISO3166Codes.Add("MNE", "Montenegro")
        CountryISO3166Codes.Add("MNG", "Mongolia")
        CountryISO3166Codes.Add("MNP", "Northern Mariana Islands")
        CountryISO3166Codes.Add("MOZ", "Mozambique")
        CountryISO3166Codes.Add("MRT", "Mauritania")
        CountryISO3166Codes.Add("MSR", "Montserrat")
        CountryISO3166Codes.Add("MTQ", "Martinique")
        CountryISO3166Codes.Add("MUS", "Mauritius")
        CountryISO3166Codes.Add("MWI", "Malawi")
        CountryISO3166Codes.Add("MYS", "Malaysia")
        CountryISO3166Codes.Add("MYT", "Mayotte")
        CountryISO3166Codes.Add("NAM", "Namibia")
        CountryISO3166Codes.Add("NCL", "New Caledonia")
        CountryISO3166Codes.Add("NER", "Niger")
        CountryISO3166Codes.Add("NFK", "Norfolk Island")
        CountryISO3166Codes.Add("NGA", "Nigeria")
        CountryISO3166Codes.Add("NIC", "Nicaragua")
        CountryISO3166Codes.Add("NIU", "Niue")
        CountryISO3166Codes.Add("NLD", "Netherlands")
        CountryISO3166Codes.Add("NOR", "Norway")
        CountryISO3166Codes.Add("NPL", "Nepal")
        CountryISO3166Codes.Add("NRU", "Nauru")
        CountryISO3166Codes.Add("NZL", "New Zealand")
        CountryISO3166Codes.Add("OMN", "Oman")
        CountryISO3166Codes.Add("PAK", "Pakistan")
        CountryISO3166Codes.Add("PAN", "Panama")
        CountryISO3166Codes.Add("PCN", "Pitcairn")
        CountryISO3166Codes.Add("PER", "Peru")
        CountryISO3166Codes.Add("PHL", "Philippines")
        CountryISO3166Codes.Add("PLW", "Palau")
        CountryISO3166Codes.Add("PNG", "Papua New Guinea")
        CountryISO3166Codes.Add("POL", "Poland")
        CountryISO3166Codes.Add("PRI", "Puerto Rico")
        CountryISO3166Codes.Add("PRK", "North Korea")
        CountryISO3166Codes.Add("PRT", "Portugal")
        CountryISO3166Codes.Add("PRY", "Paraguay")
        CountryISO3166Codes.Add("PSE", "Palestine, State of")
        CountryISO3166Codes.Add("PYF", "French Polynesia")
        CountryISO3166Codes.Add("QAT", "Qatar")
        CountryISO3166Codes.Add("REU", "Réunion")
        CountryISO3166Codes.Add("ROU", "Romania")
        CountryISO3166Codes.Add("RUS", "Russia")
        CountryISO3166Codes.Add("RWA", "Rwanda")
        CountryISO3166Codes.Add("SAU", "Saudi Arabia")
        CountryISO3166Codes.Add("SDN", "Sudan")
        CountryISO3166Codes.Add("SEN", "Senegal")
        CountryISO3166Codes.Add("SGP", "Singapore")
        CountryISO3166Codes.Add("SGS", "South Georgia and the South Sandwich Islands")
        CountryISO3166Codes.Add("SHN", "Saint Helena, Ascension and Tristan da Cunha")
        CountryISO3166Codes.Add("SJM", "Svalbard and Jan Mayen")
        CountryISO3166Codes.Add("SLB", "Solomon Islands")
        CountryISO3166Codes.Add("SLE", "Sierra Leone")
        CountryISO3166Codes.Add("SLV", "El Salvador")
        CountryISO3166Codes.Add("SMR", "San Marino")
        CountryISO3166Codes.Add("SOM", "Somalia")
        CountryISO3166Codes.Add("SPM", "Saint Pierre and Miquelon")
        CountryISO3166Codes.Add("SRB", "Serbia")
        CountryISO3166Codes.Add("SSD", "South Sudan")
        CountryISO3166Codes.Add("STP", "São Tomé and Príncipe")
        CountryISO3166Codes.Add("SUR", "Suriname")
        CountryISO3166Codes.Add("SVK", "Slovakia")
        CountryISO3166Codes.Add("SVN", "Slovenia")
        CountryISO3166Codes.Add("SWE", "Sweden")
        CountryISO3166Codes.Add("SWZ", "Swaziland")
        CountryISO3166Codes.Add("SXM", "Sint Maarten (Dutch part)")
        CountryISO3166Codes.Add("SYC", "Seychelles")
        CountryISO3166Codes.Add("SYR", "Syria")
        CountryISO3166Codes.Add("TCA", "Turks and Caicos Islands")
        CountryISO3166Codes.Add("TCD", "Chad")
        CountryISO3166Codes.Add("TGO", "Togo")
        CountryISO3166Codes.Add("THA", "Thailand")
        CountryISO3166Codes.Add("TJK", "Tajikistan")
        CountryISO3166Codes.Add("TKL", "Tokelau")
        CountryISO3166Codes.Add("TKM", "Turkmenistan")
        CountryISO3166Codes.Add("TLS", "East Timor")
        CountryISO3166Codes.Add("TON", "Tonga")
        CountryISO3166Codes.Add("TTO", "Trinidad and Tobago")
        CountryISO3166Codes.Add("TUN", "Tunisia")
        CountryISO3166Codes.Add("TUR", "Turkey")
        CountryISO3166Codes.Add("TUV", "Tuvalu")
        CountryISO3166Codes.Add("TWN", "Taiwan")
        CountryISO3166Codes.Add("TZA", "Tanzania")
        CountryISO3166Codes.Add("UGA", "Uganda")
        CountryISO3166Codes.Add("UKR", "Ukraine")
        CountryISO3166Codes.Add("UMI", "United States Minor Outlying Islands")
        CountryISO3166Codes.Add("URY", "Uruguay")
        CountryISO3166Codes.Add("USA", "United States")
        CountryISO3166Codes.Add("UZB", "Uzbekistan")
        CountryISO3166Codes.Add("VAT", "Vatican City")
        CountryISO3166Codes.Add("VCT", "Saint Vincent and the Grenadines")
        CountryISO3166Codes.Add("VEN", "Venezuela")
        CountryISO3166Codes.Add("VGB", "Virgin Islands, British")
        CountryISO3166Codes.Add("VIR", "Virgin Islands, U.S.")
        CountryISO3166Codes.Add("VNM", "Vietnam")
        CountryISO3166Codes.Add("VUT", "Vanuatu")
        CountryISO3166Codes.Add("WLF", "Wallis and Futuna")
        CountryISO3166Codes.Add("WSM", "Samoa")
        CountryISO3166Codes.Add("YEM", "Yemen")
        CountryISO3166Codes.Add("ZAF", "South Africa")
        CountryISO3166Codes.Add("ZMB", "Zambia")
        CountryISO3166Codes.Add("ZWE", "Zimbabwe")
    End Sub

    Public Sub GetCountryFlagCodes()

        CountryFlagCodes.Add("", ("", ""))
        CountryFlagCodes.Add("Aruba", ("🇦🇼", ":flag_aw:"))
        CountryFlagCodes.Add("Afghanistan", ("🇦🇫", ":flag_af:"))
        CountryFlagCodes.Add("Albania", ("🇦🇱", ":flag_al:"))
        CountryFlagCodes.Add("Algeria", ("🇩🇿", ":flag_dz:"))
        CountryFlagCodes.Add("Andorra", ("🇦🇩", ":flag_ad:"))
        CountryFlagCodes.Add("Angola", ("🇦🇴", ":flag_ao:"))
        CountryFlagCodes.Add("Anguilla", ("🇦🇮", ":flag_ai:"))
        CountryFlagCodes.Add("Åland Islands", ("🇦🇽", ":flag_ax:"))
        CountryFlagCodes.Add("Antigua and Barbuda", ("🇦🇬", ":flag_ag:"))
        CountryFlagCodes.Add("Argentina", ("🇦🇷", ":flag_ar:"))
        CountryFlagCodes.Add("Armenia", ("🇦🇲", ":flag_am:"))
        CountryFlagCodes.Add("American Samoa", ("🇦🇸", ":flag_as:"))
        CountryFlagCodes.Add("Antarctica", ("🇦🇶", ":flag_aq:"))
        CountryFlagCodes.Add("Australia", ("🇦🇺", ":flag_au:"))
        CountryFlagCodes.Add("Austria", ("🇦🇹", ":flag_at:"))
        CountryFlagCodes.Add("Azerbaijan", ("🇦🇿", ":flag_az:"))
        CountryFlagCodes.Add("Bahamas", ("🇧🇸", ":flag_bs:"))
        CountryFlagCodes.Add("Bahrain", ("🇧🇭", ":flag_bh:"))
        CountryFlagCodes.Add("Bangladesh", ("🇧🇩", ":flag_bd:"))
        CountryFlagCodes.Add("Barbados", ("🇧🇧", ":flag_bb:"))
        CountryFlagCodes.Add("Belarus", ("🇧🇾", ":flag_by:"))
        CountryFlagCodes.Add("Belgium", ("🇧🇪", ":flag_be:"))
        CountryFlagCodes.Add("Belize", ("🇧🇿", ":flag_bz:"))
        CountryFlagCodes.Add("Benin", ("🇧🇯", ":flag_bj:"))
        CountryFlagCodes.Add("Bermuda", ("🇧🇲", ":flag_bm:"))
        CountryFlagCodes.Add("Bhutan", ("🇧🇹", ":flag_bt:"))
        CountryFlagCodes.Add("Bolivia", ("🇧🇴", ":flag_bo:"))
        CountryFlagCodes.Add("Bonaire, Sint Eustatius and Saba", ("🇧🇶", ":flag_bq:"))
        CountryFlagCodes.Add("Bosnia and Herzegovina", ("🇧🇦", ":flag_ba:"))
        CountryFlagCodes.Add("Bouvet Island", ("🇧🇻", ":flag_bv:"))
        CountryFlagCodes.Add("Botswana", ("🇧🇼", ":flag_bw:"))
        CountryFlagCodes.Add("Brazil", ("🇧🇷", ":flag_br:"))
        CountryFlagCodes.Add("British Indian Ocean Territory", ("🇮🇴", ":flag_io:"))
        CountryFlagCodes.Add("Brunei", ("🇧🇳", ":flag_bn:"))
        CountryFlagCodes.Add("Bulgaria", ("🇧🇬", ":flag_bg:"))
        CountryFlagCodes.Add("Burkina Faso", ("🇧🇫", ":flag_bf:"))
        CountryFlagCodes.Add("Burundi", ("🇧🇮", ":flag_bi:"))
        CountryFlagCodes.Add("Cambodia", ("🇰🇭", ":flag_kh:"))
        CountryFlagCodes.Add("Cameroon", ("🇨🇲", ":flag_cm:"))
        CountryFlagCodes.Add("Canada", ("🇨🇦", ":flag_ca:"))
        CountryFlagCodes.Add("Cape Verde", ("🇨🇻", ":flag_cv:"))
        CountryFlagCodes.Add("Cayman Islands", ("🇰🇾", ":flag_ky:"))
        CountryFlagCodes.Add("Central African Republic", ("🇨🇫", ":flag_cf:"))
        CountryFlagCodes.Add("Chad", ("🇹🇩", ":flag_td:"))
        CountryFlagCodes.Add("Chile", ("🇨🇱", ":flag_cl:"))
        CountryFlagCodes.Add("China", ("🇨🇳", ":flag_cn:"))
        CountryFlagCodes.Add("Christmas Island", ("🇨🇽", ":flag_cx:"))
        CountryFlagCodes.Add("Cocos Islands", ("🇨🇨", ":flag_cc:"))
        CountryFlagCodes.Add("Colombia", ("🇨🇴", ":flag_co:"))
        CountryFlagCodes.Add("Comoros", ("🇰🇲", ":flag_km:"))
        CountryFlagCodes.Add("Congo", ("🇨🇬", ":flag_cg:"))
        CountryFlagCodes.Add("Congo (Democratic Republic of the)", ("🇨🇩", ":flag_cd:"))
        CountryFlagCodes.Add("Cook Islands", ("🇨🇰", ":flag_ck:"))
        CountryFlagCodes.Add("Costa Rica", ("🇨🇷", ":flag_cr:"))
        CountryFlagCodes.Add("Croatia", ("🇭🇷", ":flag_hr:"))
        CountryFlagCodes.Add("Cuba", ("🇨🇺", ":flag_cu:"))
        CountryFlagCodes.Add("Curaçao", ("🇨🇼", ":flag_cw:"))
        CountryFlagCodes.Add("Cyprus", ("🇨🇾", ":flag_cy:"))
        CountryFlagCodes.Add("Czech Republic", ("🇨🇿", ":flag_cz:"))
        CountryFlagCodes.Add("Denmark", ("🇩🇰", ":flag_dk:"))
        CountryFlagCodes.Add("Djibouti", ("🇩🇯", ":flag_dj:"))
        CountryFlagCodes.Add("Dominica", ("🇩🇲", ":flag_dm:"))
        CountryFlagCodes.Add("Dominican Republic", ("🇩🇴", ":flag_do:"))
        CountryFlagCodes.Add("East Timor", ("🇹🇱", ":flag_tl:"))
        CountryFlagCodes.Add("Ecuador", ("🇪🇨", ":flag_ec:"))
        CountryFlagCodes.Add("Egypt", ("🇪🇬", ":flag_eg:"))
        CountryFlagCodes.Add("El Salvador", ("🇸🇻", ":flag_sv:"))
        CountryFlagCodes.Add("Equatorial Guinea", ("🇬🇶", ":flag_gq:"))
        CountryFlagCodes.Add("Eritrea", ("🇪🇷", ":flag_er:"))
        CountryFlagCodes.Add("Estonia", ("🇪🇪", ":flag_ee:"))
        CountryFlagCodes.Add("Ethiopia", ("🇪🇹", ":flag_et:"))
        CountryFlagCodes.Add("Falkland Islands", ("🇫🇰", ":flag_fk:"))
        CountryFlagCodes.Add("Faroe Islands", ("🇫🇴", ":flag_fo:"))
        CountryFlagCodes.Add("Fiji", ("🇫🇯", ":flag_fj:"))
        CountryFlagCodes.Add("Finland", ("🇫🇮", ":flag_fi:"))
        CountryFlagCodes.Add("France", ("🇫🇷", ":flag_fr:"))
        CountryFlagCodes.Add("French Guiana", ("🇬🇫", ":flag_gf:"))
        CountryFlagCodes.Add("French Polynesia", ("🇵🇫", ":flag_pf:"))
        CountryFlagCodes.Add("French Southern Territories", ("🇹🇫", ":flag_tf:"))
        CountryFlagCodes.Add("Gabon", ("🇬🇦", ":flag_ga:"))
        CountryFlagCodes.Add("Gambia", ("🇬🇲", ":flag_gm:"))
        CountryFlagCodes.Add("Georgia", ("🇬🇪", ":flag_ge:"))
        CountryFlagCodes.Add("Germany", ("🇩🇪", ":flag_de:"))
        CountryFlagCodes.Add("Ghana", ("🇬🇭", ":flag_gh:"))
        CountryFlagCodes.Add("Gibraltar", ("🇬🇮", ":flag_gi:"))
        CountryFlagCodes.Add("Greece", ("🇬🇷", ":flag_gr:"))
        CountryFlagCodes.Add("Greenland", ("🇬🇱", ":flag_gl:"))
        CountryFlagCodes.Add("Grenada", ("🇬🇩", ":flag_gd:"))
        CountryFlagCodes.Add("Guadeloupe", ("🇬🇵", ":flag_gp:"))
        CountryFlagCodes.Add("Guam", ("🇬🇺", ":flag_gu:"))
        CountryFlagCodes.Add("Guatemala", ("🇬🇹", ":flag_gt:"))
        CountryFlagCodes.Add("Guernsey", ("🇬🇬", ":flag_gg:"))
        CountryFlagCodes.Add("Guinea", ("🇬🇳", ":flag_gn:"))
        CountryFlagCodes.Add("Guinea-Bissau", ("🇬🇼", ":flag_gw:"))
        CountryFlagCodes.Add("Guyana", ("🇬🇾", ":flag_gy:"))
        CountryFlagCodes.Add("Haiti", ("🇭🇹", ":flag_ht:"))
        CountryFlagCodes.Add("Heard Island and McDonald Islands", ("🇭🇲", ":flag_hm:"))
        CountryFlagCodes.Add("Honduras", ("🇭🇳", ":flag_hn:"))
        CountryFlagCodes.Add("Hong Kong", ("🇭🇰", ":flag_hk:"))
        CountryFlagCodes.Add("Hungary", ("🇭🇺", ":flag_hu:"))
        CountryFlagCodes.Add("Iceland", ("🇮🇸", ":flag_is:"))
        CountryFlagCodes.Add("India", ("🇮🇳", ":flag_in:"))
        CountryFlagCodes.Add("Indonesia", ("🇮🇩", ":flag_id:"))
        CountryFlagCodes.Add("Iran", ("🇮🇷", ":flag_ir:"))
        CountryFlagCodes.Add("Iraq", ("🇮🇶", ":flag_iq:"))
        CountryFlagCodes.Add("Ireland", ("🇮🇪", ":flag_ie:"))
        CountryFlagCodes.Add("Isle of Man", ("🇮🇲", ":flag_im:"))
        CountryFlagCodes.Add("Israel", ("🇮🇱", ":flag_il:"))
        CountryFlagCodes.Add("Italy", ("🇮🇹", ":flag_it:"))
        CountryFlagCodes.Add("Ivory Coast", ("🇨🇮", ":flag_ci:"))
        CountryFlagCodes.Add("Jamaica", ("🇯🇲", ":flag_jm:"))
        CountryFlagCodes.Add("Japan", ("🇯🇵", ":flag_jp:"))
        CountryFlagCodes.Add("Jersey", ("🇯🇪", ":flag_je:"))
        CountryFlagCodes.Add("Jordan", ("🇯🇴", ":flag_jo:"))
        CountryFlagCodes.Add("Kazakhstan", ("🇰🇿", ":flag_kz:"))
        CountryFlagCodes.Add("Kenya", ("🇰🇪", ":flag_ke:"))
        CountryFlagCodes.Add("Kiribati", ("🇰🇮", ":flag_ki:"))
        CountryFlagCodes.Add("Kuwait", ("🇰🇼", ":flag_kw:"))
        CountryFlagCodes.Add("Kyrgyzstan", ("🇰🇬", ":flag_kg:"))
        CountryFlagCodes.Add("Laos", ("🇱🇦", ":flag_la:"))
        CountryFlagCodes.Add("Latvia", ("🇱🇻", ":flag_lv:"))
        CountryFlagCodes.Add("Lebanon", ("🇱🇧", ":flag_lb:"))
        CountryFlagCodes.Add("Lesotho", ("🇱🇸", ":flag_ls:"))
        CountryFlagCodes.Add("Liberia", ("🇱🇷", ":flag_lr:"))
        CountryFlagCodes.Add("Libya", ("🇱🇾", ":flag_ly:"))
        CountryFlagCodes.Add("Liechtenstein", ("🇱🇮", ":flag_li:"))
        CountryFlagCodes.Add("Lithuania", ("🇱🇹", ":flag_lt:"))
        CountryFlagCodes.Add("Luxembourg", ("🇱🇺", ":flag_lu:"))
        CountryFlagCodes.Add("Macao", ("🇲🇴", ":flag_mo:"))
        CountryFlagCodes.Add("Macedonia", ("🇲🇰", ":flag_mk:"))
        CountryFlagCodes.Add("Madagascar", ("🇲🇬", ":flag_mg:"))
        CountryFlagCodes.Add("Malawi", ("🇲🇼", ":flag_mw:"))
        CountryFlagCodes.Add("Malaysia", ("🇲🇾", ":flag_my:"))
        CountryFlagCodes.Add("Maldives", ("🇲🇻", ":flag_mv:"))
        CountryFlagCodes.Add("Mali", ("🇲🇱", ":flag_ml:"))
        CountryFlagCodes.Add("Malta", ("🇲🇹", ":flag_mt:"))
        CountryFlagCodes.Add("Marshall Islands", ("🇲🇭", ":flag_mh:"))
        CountryFlagCodes.Add("Martinique", ("🇲🇶", ":flag_mq:"))
        CountryFlagCodes.Add("Mauritania", ("🇲🇷", ":flag_mr:"))
        CountryFlagCodes.Add("Mauritius", ("🇲🇺", ":flag_mu:"))
        CountryFlagCodes.Add("Mayotte", ("🇾🇹", ":flag_yt:"))
        CountryFlagCodes.Add("Mexico", ("🇲🇽", ":flag_mx:"))
        CountryFlagCodes.Add("Micronesia", ("🇫🇲", ":flag_fm:"))
        CountryFlagCodes.Add("Moldova", ("🇲🇩", ":flag_md:"))
        CountryFlagCodes.Add("Monaco", ("🇲🇨", ":flag_mc:"))
        CountryFlagCodes.Add("Mongolia", ("🇲🇳", ":flag_mn:"))
        CountryFlagCodes.Add("Montenegro", ("🇲🇪", ":flag_me:"))
        CountryFlagCodes.Add("Montserrat", ("🇲🇸", ":flag_ms:"))
        CountryFlagCodes.Add("Morocco", ("🇲🇦", ":flag_ma:"))
        CountryFlagCodes.Add("Mozambique", ("🇲🇿", ":flag_mz:"))
        CountryFlagCodes.Add("Myanmar", ("🇲🇲", ":flag_mm:"))
        CountryFlagCodes.Add("Namibia", ("🇳🇦", ":flag_na:"))
        CountryFlagCodes.Add("Nauru", ("🇳🇷", ":flag_nr:"))
        CountryFlagCodes.Add("Nepal", ("🇳🇵", ":flag_np:"))
        CountryFlagCodes.Add("Netherlands", ("🇳🇱", ":flag_nl:"))
        CountryFlagCodes.Add("New Caledonia", ("🇳🇨", ":flag_nc:"))
        CountryFlagCodes.Add("New Zealand", ("🇳🇿", ":flag_nz:"))
        CountryFlagCodes.Add("Nicaragua", ("🇳🇮", ":flag_ni:"))
        CountryFlagCodes.Add("Niger", ("🇳🇪", ":flag_ne:"))
        CountryFlagCodes.Add("Nigeria", ("🇳🇬", ":flag_ng:"))
        CountryFlagCodes.Add("Niue", ("🇳🇺", ":flag_nu:"))
        CountryFlagCodes.Add("Norfolk Island", ("🇳🇫", ":flag_nf:"))
        CountryFlagCodes.Add("North Korea", ("🇰🇵", ":flag_kp:"))
        CountryFlagCodes.Add("Northern Mariana Islands", ("🇲🇵", ":flag_mp:"))
        CountryFlagCodes.Add("Norway", ("🇳🇴", ":flag_no:"))
        CountryFlagCodes.Add("Oman", ("🇴🇲", ":flag_om:"))
        CountryFlagCodes.Add("Pakistan", ("🇵🇰", ":flag_pk:"))
        CountryFlagCodes.Add("Palau", ("🇵🇼", ":flag_pw:"))
        CountryFlagCodes.Add("Palestine, State of", ("🇵🇸", ":flag_ps:"))
        CountryFlagCodes.Add("Panama", ("🇵🇦", ":flag_pa:"))
        CountryFlagCodes.Add("Papua New Guinea", ("🇵🇬", ":flag_pg:"))
        CountryFlagCodes.Add("Paraguay", ("🇵🇾", ":flag_py:"))
        CountryFlagCodes.Add("Peru", ("🇵🇪", ":flag_pe:"))
        CountryFlagCodes.Add("Philippines", ("🇵🇭", ":flag_ph:"))
        CountryFlagCodes.Add("Pitcairn", ("🇵🇳", ":flag_pn:"))
        CountryFlagCodes.Add("Poland", ("🇵🇱", ":flag_pl:"))
        CountryFlagCodes.Add("Portugal", ("🇵🇹", ":flag_pt:"))
        CountryFlagCodes.Add("Puerto Rico", ("🇵🇷", ":flag_pr:"))
        CountryFlagCodes.Add("Qatar", ("🇶🇦", ":flag_qa:"))
        CountryFlagCodes.Add("Réunion", ("🇷🇪", ":flag_re:"))
        CountryFlagCodes.Add("Romania", ("🇷🇴", ":flag_ro:"))
        CountryFlagCodes.Add("Russia", ("🇷🇺", ":flag_ru:"))
        CountryFlagCodes.Add("Rwanda", ("🇷🇼", ":flag_rw:"))
        CountryFlagCodes.Add("Saint Barthélemy", ("🇧🇱", ":flag_bl:"))
        CountryFlagCodes.Add("Saint Helena, Ascension and Tristan da Cunha", ("🇸🇭", ":flag_sh:"))
        CountryFlagCodes.Add("Saint Kitts and Nevis", ("🇰🇳", ":flag_kn:"))
        CountryFlagCodes.Add("Saint Lucia", ("🇱🇨", ":flag_lc:"))
        CountryFlagCodes.Add("Saint Martin (French part)", ("🇲🇫", ":flag_mf:"))
        CountryFlagCodes.Add("Saint Pierre and Miquelon", ("🇵🇲", ":flag_pm:"))
        CountryFlagCodes.Add("Saint Vincent and the Grenadines", ("🇻🇨", ":flag_vc:"))
        CountryFlagCodes.Add("Samoa", ("🇼🇸", ":flag_ws:"))
        CountryFlagCodes.Add("San Marino", ("🇸🇲", ":flag_sm:"))
        CountryFlagCodes.Add("São Tomé and Príncipe", ("🇸🇹", ":flag_st:"))
        CountryFlagCodes.Add("Saudi Arabia", ("🇸🇦", ":flag_sa:"))
        CountryFlagCodes.Add("Senegal", ("🇸🇳", ":flag_sn:"))
        CountryFlagCodes.Add("Serbia", ("🇷🇸", ":flag_rs:"))
        CountryFlagCodes.Add("Seychelles", ("🇸🇨", ":flag_sc:"))
        CountryFlagCodes.Add("Sierra Leone", ("🇸🇱", ":flag_sl:"))
        CountryFlagCodes.Add("Singapore", ("🇸🇬", ":flag_sg:"))
        CountryFlagCodes.Add("Sint Maarten (Dutch part)", ("🇸🇽", ":flag_sx:"))
        CountryFlagCodes.Add("Slovakia", ("🇸🇰", ":flag_sk:"))
        CountryFlagCodes.Add("Slovenia", ("🇸🇮", ":flag_si:"))
        CountryFlagCodes.Add("Solomon Islands", ("🇸🇧", ":flag_sb:"))
        CountryFlagCodes.Add("Somalia", ("🇸🇴", ":flag_so:"))
        CountryFlagCodes.Add("South Africa", ("🇿🇦", ":flag_za:"))
        CountryFlagCodes.Add("South Georgia and the South Sandwich Islands", ("🇬🇸", ":flag_gs:"))
        CountryFlagCodes.Add("South Korea", ("🇰🇷", ":flag_kr:"))
        CountryFlagCodes.Add("South Sudan", ("🇸🇸", ":flag_ss:"))
        CountryFlagCodes.Add("Spain", ("🇪🇸", ":flag_es:"))
        CountryFlagCodes.Add("Sri Lanka", ("🇱🇰", ":flag_lk:"))
        CountryFlagCodes.Add("Sudan", ("🇸🇩", ":flag_sd:"))
        CountryFlagCodes.Add("Suriname", ("🇸🇷", ":flag_sr:"))
        CountryFlagCodes.Add("Svalbard and Jan Mayen", ("🇸🇯", ":flag_sj:"))
        CountryFlagCodes.Add("Swaziland", ("🇸🇿", ":flag_sz:"))
        CountryFlagCodes.Add("Sweden", ("🇸🇪", ":flag_se:"))
        CountryFlagCodes.Add("Switzerland", ("🇨🇭", ":flag_ch:"))
        CountryFlagCodes.Add("Syria", ("🇸🇾", ":flag_sy:"))
        CountryFlagCodes.Add("Taiwan", ("🇹🇼", ":flag_tw:"))
        CountryFlagCodes.Add("Tajikistan", ("🇹🇯", ":flag_tj:"))
        CountryFlagCodes.Add("Tanzania", ("🇹🇿", ":flag_tz:"))
        CountryFlagCodes.Add("Thailand", ("🇹🇭", ":flag_th:"))
        CountryFlagCodes.Add("Togo", ("🇹🇬", ":flag_tg:"))
        CountryFlagCodes.Add("Tokelau", ("🇹🇰", ":flag_tk:"))
        CountryFlagCodes.Add("Tonga", ("🇹🇴", ":flag_to:"))
        CountryFlagCodes.Add("Trinidad and Tobago", ("🇹🇹", ":flag_tt:"))
        CountryFlagCodes.Add("Tunisia", ("🇹🇳", ":flag_tn:"))
        CountryFlagCodes.Add("Turkey", ("🇹🇷", ":flag_tr:"))
        CountryFlagCodes.Add("Turkmenistan", ("🇹🇲", ":flag_tm:"))
        CountryFlagCodes.Add("Turks and Caicos Islands", ("🇹🇨", ":flag_tc:"))
        CountryFlagCodes.Add("Tuvalu", ("🇹🇻", ":flag_tv:"))
        CountryFlagCodes.Add("Uganda", ("🇺🇬", ":flag_ug:"))
        CountryFlagCodes.Add("Ukraine", ("🇺🇦", ":flag_ua:"))
        CountryFlagCodes.Add("United Arab Emirates", ("🇦🇪", ":flag_ae:"))
        CountryFlagCodes.Add("United Kingdom", ("🇬🇧", ":flag_gb:"))
        CountryFlagCodes.Add("United States", ("🇺🇸", ":flag_us:"))
        CountryFlagCodes.Add("United States Minor Outlying Islands", ("🇺🇲", ":flag_um:"))
        CountryFlagCodes.Add("Uruguay", ("🇺🇾", ":flag_uy:"))
        CountryFlagCodes.Add("Uzbekistan", ("🇺🇿", ":flag_uz:"))
        CountryFlagCodes.Add("Vanuatu", ("🇻🇺", ":flag_vu:"))
        CountryFlagCodes.Add("Vatican City", ("🇻🇦", ":flag_va:"))
        CountryFlagCodes.Add("Venezuela", ("🇻🇪", ":flag_ve:"))
        CountryFlagCodes.Add("Vietnam", ("🇻🇳", ":flag_vn:"))
        CountryFlagCodes.Add("Virgin Islands, British", ("🇻🇬", ":flag_vg:"))
        CountryFlagCodes.Add("Virgin Islands, U.S.", ("🇻🇮", ":flag_vi:"))
        CountryFlagCodes.Add("Wallis and Futuna", ("🇼🇫", ":flag_wf:"))
        CountryFlagCodes.Add("Western Sahara", ("🇪🇭", ":flag_eh:"))
        CountryFlagCodes.Add("Yemen", ("🇾🇪", ":flag_ye:"))
        CountryFlagCodes.Add("Zambia", ("🇿🇲", ":flag_zm:"))
        CountryFlagCodes.Add("Zimbabwe", ("🇿🇼", ":flag_zw:"))

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

    Public Shared Function ConvertMarkdownToRTF(ByVal input As String) As String
        ' Replace special characters
        input = ReplaceSpecialCharactersWithUnicodeEscapes(input)

        ' Replace the degree symbol with its RTF escape sequence
        Dim degreeSymbol As Char = ChrW(&HB0)
        input = input.Replace(degreeSymbol.ToString(), "\u176\'")

        ' Replace custom newlines with actual carriage returns
        input = input.Replace("($*$)", vbCrLf)

        ' Step 1: Extract code blocks and replace them with placeholders
        Dim codeBlocks As New List(Of String)
        input = Regex.Replace(input, "```([\s\S]+?)```", Function(m)
                                                             Dim codeContent As String = m.Groups(1).Value
                                                             Dim placeholder As String = $"[CODE_BLOCK_{codeBlocks.Count}]"
                                                             codeBlocks.Add(codeContent)
                                                             Return placeholder
                                                         End Function, RegexOptions.Singleline)

        ' Ensure placeholders are on their own lines
        input = Regex.Replace(input, "(?<!\r?\n)\[CODE_BLOCK_(\d+)\]", vbCrLf & "[CODE_BLOCK_$1]" & vbCrLf) ' Add newline before placeholder if not at start
        input = Regex.Replace(input, "\[CODE_BLOCK_(\d+)\](?![\r?\n])", "[CODE_BLOCK_$1]" & vbCrLf) ' Add newline after placeholder if not at end

        ' Step 2: Convert markdown outside of code blocks
        ' Handle URLs: Convert Markdown-style links to RTF hyperlinks
        input = Regex.Replace(input, "\[(.+?)\]\((.+?)\)", Function(m)
                                                               Dim linkText As String = m.Groups(1).Value
                                                               Dim url As String = m.Groups(2).Value
                                                               Return "{\field{\*\fldinst{HYPERLINK """ & url & """}}{\fldrslt{\ul\cf1 " & linkText & "}}}"
                                                           End Function)


        ' Convert inline code
        input = Regex.Replace(input, "`(.+?)`", "{\f1 $1\f0}") ' Inline code (monospace)
        ' Convert markdown syntax to corresponding RTF code
        input = Regex.Replace(input, "\*\*(.+?)\*\*", "{\b $1\b0}") ' Bold
        input = Regex.Replace(input, "(?<!\*)\*(.+?)\*(?!\*)", "{\i $1\i0}") ' Italic
        input = Regex.Replace(input, "__(.+?)__", "{\ul $1\ul0}") ' Underline
        input = Regex.Replace(input, "~~(.+?)~~", "{\strike $1\strike0 }") ' Strikethrough

        ' Split the input into lines to handle list items separately
        Dim lines As String() = input.Split(New String() {vbCrLf}, StringSplitOptions.None)
        Dim processedLines As New List(Of String)

        Dim inList As Boolean = False
        Dim listLevel As Integer = 0 ' 0: no list, 1: first-level, 2: second-level

        For Each line As String In lines
            If Regex.IsMatch(line, "^\s{1,2}[-\*]\s") Or Regex.IsMatch(line, "^\s{1,2}\d+\.\s") Or Regex.IsMatch(line, "^\s{1,2}[a-zA-Z]\.\s") Then
                ' Handle second-level lists first
                If Regex.IsMatch(line, "^\s{1,2}[-\*]\s") Then
                    ' Ensure correct list level
                    If Not inList Or listLevel <> 2 Then
                        If inList Then
                            processedLines.Add("\pard") ' Close previous list
                        End If
                        listLevel = 2
                        inList = True
                        processedLines.Add("\pard\li480\fi-240") ' Adjusted indent for second-level bullet points
                    End If
                    processedLines.Add("{\pntext\f0\'B7\tab}{\*\pn\pnlvlblt\pnf0\pnindent240{\pntxtb\'B7}}\fi-240\li480 " & Regex.Replace(line, "^\s{1,2}[-\*]\s+", "") & "\par")
                ElseIf Regex.IsMatch(line, "^\s{1,2}[a-zA-Z]\.\s") Then
                    ' Ensure correct list level
                    If Not inList Or listLevel <> 2 Then
                        If inList Then
                            processedLines.Add("\pard") ' Close previous list
                        End If
                        listLevel = 2
                        inList = True
                        processedLines.Add("\pard\li480\fi-240") ' Adjusted indent for second-level lettered lists
                    End If
                    Dim listMarker As String = Regex.Match(line, "^\s{1,2}([a-zA-Z])\.\s").Groups(1).Value
                    processedLines.Add("{\pntext\f0 " & listMarker & ".\tab}{\*\pn\pnlvlbody\pnf0\pnindent240{\pntxta .}}\fi-240\li480 " & Regex.Replace(line, "^\s{1,2}[a-zA-Z]\.\s+", "") & "\par")
                ElseIf Regex.IsMatch(line, "^\s{1,2}\d+\.\s") Then
                    ' Ensure correct list level
                    If Not inList Or listLevel <> 2 Then
                        If inList Then
                            processedLines.Add("\pard") ' Close previous list
                        End If
                        listLevel = 2
                        inList = True
                        processedLines.Add("\pard\li480\fi-240") ' Adjusted indent for second-level numbered lists
                    End If
                    Dim listMarker As String = Regex.Match(line, "^\s{1,2}(\d+)\.\s").Groups(1).Value
                    processedLines.Add("{\pntext\f0 " & listMarker & ".\tab}{\*\pn\pnlvlbody\pnf0\pnindent240{\pntxta .}}\fi-240\li480 " & Regex.Replace(line, "^\s{1,2}\d+\.\s+", "") & "\par")
                End If
            ElseIf Regex.IsMatch(line, "^\s*[-\*]\s") Or Regex.IsMatch(line, "^\s*\d+\.\s") Then
                ' Handle first-level lists
                If Regex.IsMatch(line, "^\s*\d+\.\s") Then
                    ' Ensure correct list level
                    If Not inList Or listLevel <> 1 Then
                        If inList Then
                            processedLines.Add("\pard") ' Close previous list
                        End If
                        listLevel = 1
                        inList = True
                        processedLines.Add("\pard\li240\fi-240") ' Start first-level list
                    End If
                    Dim listMarker As String = Regex.Match(line, "^\s*(\d+)\.\s").Groups(1).Value
                    processedLines.Add("{\pntext\f0 " & listMarker & ".\tab}{\*\pn\pnlvlbody\pnf0\pnindent240{\pntxta .}}\fi-240\li240 " & Regex.Replace(line, "^\s*\d+\.\s+", "") & "\par")
                ElseIf Regex.IsMatch(line, "^\s*[-\*]\s") Then
                    ' Ensure correct list level
                    If Not inList Or listLevel <> 1 Then
                        If inList Then
                            processedLines.Add("\pard") ' Close previous list
                        End If
                        listLevel = 1
                        inList = True
                        processedLines.Add("\pard\li240\fi-240") ' Start first-level list
                    End If
                    processedLines.Add("{\pntext\f0\'B7\tab}{\*\pn\pnlvlblt\pnf0\pnindent240{\pntxtb\'B7}}\fi-240\li240 " & Regex.Replace(line, "^\s*[-\*]\s+", "") & "\par")
                End If
            Else
                ' Handle normal text
                If inList Then
                    processedLines.Add("\pard") ' Close list
                    inList = False
                    listLevel = 0
                End If
                processedLines.Add($"{line}\par")
            End If
        Next

        ' Close any open list
        If inList Then
            processedLines.Add("\pard")
        End If

        ' Step 3: Restore the code blocks
        Dim rtfFormatted As String = String.Join(" ", processedLines)
        For i As Integer = 0 To codeBlocks.Count - 1
            Dim codeContent As String = codeBlocks(i).Trim().Replace(vbCrLf, "\line ")
            Dim rtfCodeBlock As String = "{\pard\li0\ri0\qj\sa100\sb100\brdrb\brdrs\brdrw10\brsp20\f1\fs18 " & codeContent & "}"
            rtfFormatted = rtfFormatted.Replace($"[CODE_BLOCK_{i}]", rtfCodeBlock)
        Next

        ' Wrap the RTF content
        Return "{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Arial;}{\f1\fmodern\fcharset0 Courier New;}}\viewkind4\uc1\pard\lang1033\f0\fs20 " & rtfFormatted & "}"
    End Function

    '    Public Shared Sub FormatMarkdownToRTF(ByVal input As String, ByRef richTextBox As RichTextBox, Optional debugMode As Boolean = False)

    '#If DEBUG Then
    '        If debugMode Then
    '            Clipboard.SetText(input)
    '            MsgBox("DEBUG - Paste the content of your clipboard - this is the source text")
    '        End If
    '#End If

    '        input = ReplaceSpecialCharactersWithUnicodeEscapes(input)

    '        ' Regex patterns to match bold, italic, and underlined texts
    '        Dim newlinePattern As String = "\(\$\*\$\)"
    '        Dim boldPattern As String = "\*\*(.+?)\*\*"
    '        Dim italicPattern As String = "(?<!\*)\*(.+?)\*(?!\*)"
    '        Dim underlinePattern As String = "__(.+?)__"

    '        ' Replace line break, bold, italic, and underlined markdown syntax with corresponding RTF code
    '        Dim rtfFormatted As String = Regex.Replace(input, newlinePattern, "\line ")
    '        rtfFormatted = Regex.Replace(rtfFormatted, boldPattern, "{\b $1\b0 }")
    '        rtfFormatted = Regex.Replace(rtfFormatted, italicPattern, "{\i $1\i0 }")
    '        rtfFormatted = Regex.Replace(rtfFormatted, underlinePattern, "{\ul $1\ul0 }")

    '        ' Replace the degree symbol with its RTF escape sequence
    '        Dim degreeSymbol As Char = ChrW(&HB0)
    '        rtfFormatted = rtfFormatted.Replace(degreeSymbol.ToString(), "\u176'")

    '#If DEBUG Then
    '        If debugMode Then
    '            Clipboard.SetText(rtfFormatted)
    '            MsgBox("DEBUG - Paste the content of your clipboard - this is the transformed RTF code")
    '        End If
    '#End If

    '        ' Set the RTF-formatted text to the RichTextBox control
    '        richTextBox.Rtf = "{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 Arial;}}\viewkind4\uc1\pard\lang1033\f0\fs20 " & rtfFormatted & "\par}"
    '    End Sub

    Private Shared Function ReplaceSpecialCharactersWithUnicodeEscapes(ByVal input As String) As String
        Dim sb As New StringBuilder(input)

        ' Replace common special characters with RTF Unicode escape sequences
        Dim specialCharacters As New Dictionary(Of Char, Integer) From {
            {"ä"c, 228}, {"ö"c, 246}, {"ü"c, 252},
            {"Ä"c, 196}, {"Ö"c, 214}, {"Ü"c, 220},
            {"ß"c, 223}, {"é"c, 233}, {"ç"c, 231},
            {"à"c, 224}, {"è"c, 232}, {"ì"c, 236},
            {"ò"c, 242}, {"ù"c, 249}, {"â"c, 226},
            {"ê"c, 234}, {"î"c, 238}, {"ô"c, 244},
            {"û"c, 251}, {"å"c, 229}, {"Å"c, 197},
            {"æ"c, 230}, {"Æ"c, 198}, {"œ"c, 339},
            {"Œ"c, 338}, {"ñ"c, 241}, {"Ñ"c, 209},
            {"ý"c, 253}, {"Ý"c, 221}, {"í"c, 237},
            {"Í"c, 205}, {"Á"c, 193}, {"É"c, 201},
            {"Ú"c, 218}, {"Ó"c, 211}, {"ø"c, 248},
            {"Ø"c, 216}, {"€"c, 8364}, {"£"c, 163},
            {"¿"c, 191}, {"¡"c, 161}, {"«"c, 171},
            {"»"c, 187}, {"°"c, 176}}

        For Each kvp As KeyValuePair(Of Char, Integer) In specialCharacters
            sb.Replace(kvp.Key.ToString(), "\u" & kvp.Value & "\'3f")
        Next

        Return sb.ToString()
    End Function

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

        Return New List(Of String) From {"[Unicom 1](https//discord.com/channels/793376245915189268/793378730750771210)",
                                         "[Unicom 2](https://discord.com/channels/793376245915189268/793379061237284874)",
                                         "[Unicom 3](https://discord.com/channels/793376245915189268/793437043861487626)",
                                         "[Sim Soaring Club (PTT)](https://discord.com/channels/876123356385149009/876397825934626836)",
                                         "[Flight 01](https://discord.com/channels/876123356385149009/876123356385149015)",
                                         "[Flight 02](https://discord.com/channels/876123356385149009/876130658513203230)",
                                         "[General](https: //discord.com/channels/325227457445625856/448551355712274435)"}

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
            Using New Centered_MessageBox()
                MessageBox.Show("An error occurred trying to write to the registry!", "Writing user settings to registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End Try

    End Sub

    Public Shared Sub WriteRegistryKey(valueName As String, value As String)

        Dim keyPath As String = "Software\SIGLR\MSFS Soaring Task Tools"

        Try
            Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
                key.SetValue(valueName, value)
            End Using

        Catch ex As Exception
            Using New Centered_MessageBox()
                MessageBox.Show("An error occurred trying to write to the registry!", "Writing user settings to registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
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
            Using New Centered_MessageBox()
                MessageBox.Show("An error occurred trying to read from the registry!", "Reading user settings from registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
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
            Using New Centered_MessageBox()
                MessageBox.Show("An error occurred trying to read from the registry!", "Reading user settings from registry", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
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
                Dim countryCode As String = CountryFlagCodes(countryName).Item2
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

    Public Sub OpenB21Planner(Optional pFlightplanFilename As String = "",
                          Optional pFlightplanXML As String = "",
                          Optional pWeatherFilename As String = "",
                          Optional pWeatherXML As String = "",
                          Optional pNB21IGCFolder As String = "")

        Try
            Cursor.Current = Cursors.WaitCursor

            Dim firstPartURL As String = "siglr.com/DiscordPostHelper/FlightPlans/"

            If pFlightplanFilename = String.Empty Then
                Process.Start(B21PlannerURL)
                Exit Sub
            End If

            Dim tempFolderName As String = GenerateRandomFileName()
            Dim urlsList As New StringBuilder()

            ' Upload flight plan and append URL
            If pFlightplanFilename <> String.Empty Then
                Dim flightPlanFilename As String = Path.GetFileName(pFlightplanFilename)
                UploadFile(tempFolderName, flightPlanFilename, pFlightplanXML)
                urlsList.AppendLine($"https://{firstPartURL}{tempFolderName}/{flightPlanFilename}")
            End If

            ' Upload weather file and append URL
            If pWeatherFilename <> String.Empty Then
                Dim weatherFilename As String = Path.GetFileName(pWeatherFilename)
                UploadFile(tempFolderName, weatherFilename, pWeatherXML)
                urlsList.AppendLine($"https://{firstPartURL}{tempFolderName}/{weatherFilename}")
            End If

            ' Upload IGC files and append URLs
            If pNB21IGCFolder <> String.Empty AndAlso Directory.Exists(pNB21IGCFolder) Then
                Dim searchPattern As String = $"*_{Path.GetFileNameWithoutExtension(pFlightplanFilename)}.igc"
                Dim files As String() = Directory.GetFiles(pNB21IGCFolder, searchPattern)
                For i As Integer = 0 To files.Length - 1
                    Dim file As String = files(i)
                    UploadDirectFile(tempFolderName, file)

                    ' Append the URL
                    urlsList.Append($"https://{firstPartURL}{tempFolderName}/{Path.GetFileName(file)}")

                    ' Add a Unix-style newline (LF) except for the last URL
                    If i < files.Length - 1 Then
                        urlsList.Append(vbLf) ' Unix-style line break
                    End If
                Next
            End If

            ' Write the URLs to a text file
            UploadTextFile(tempFolderName, "listoffiles.comp", urlsList.ToString())

            ' Launch B21PlannerURL with the text file URL
            Dim processStartString As String
            If pWeatherFilename <> String.Empty Then
                processStartString = $"{B21PlannerURL}?wpr={firstPartURL}{tempFolderName}/{Path.GetFileName(pWeatherFilename)}&comp={firstPartURL}{tempFolderName}/listoffiles.comp"
            Else
                processStartString = $"{B21PlannerURL}?comp={firstPartURL}{tempFolderName}/listoffiles.comp"
            End If
            Process.Start(processStartString)

            Cursor.Current = Cursors.Default

        Catch ex As Exception
            Cursor.Current = Cursors.Default

        End Try

    End Sub

    Public Function GetCorrespondingIGCFiles(
    ByVal plnFilePath As String,
    ByVal igcFolder As String
) As List(Of String)

        Dim results As New List(Of String)

        ' Validate folder input
        If String.IsNullOrWhiteSpace(igcFolder) OrElse Not Directory.Exists(igcFolder) Then
            Return results
        End If

        ' If no .pln specified, return all .igc files
        If String.IsNullOrWhiteSpace(plnFilePath) Then
            results.AddRange(Directory.GetFiles(igcFolder, "*.igc"))
            results = results.OrderByDescending(Function(f) File.GetLastWriteTimeUtc(f)).ToList() ' <-- sort newest first
            Return results
        End If

        ' Otherwise build the search pattern "*_{basename}.igc"
        Dim baseName As String = Path.GetFileNameWithoutExtension(plnFilePath)
        Dim searchPattern As String = $"*_{baseName}.igc"

        ' Grab and return all matching files
        results.AddRange(Directory.GetFiles(igcFolder, searchPattern))
        results = results.OrderByDescending(Function(f) File.GetLastWriteTimeUtc(f)).ToList() ' <-- sort newest first
        Return results

    End Function


    Public Shared Function LaunchDiscordURL(ByRef theURL As String) As Boolean
        Dim isValid As Boolean = IsValidURL(theURL)
        Dim discordWorked As Boolean = False

        Try
            If isValid Then
                Dim lowerUrl As String = theURL.ToLowerInvariant()

                If lowerUrl.StartsWith("http://discord.com") OrElse
                   lowerUrl.StartsWith("http://discordapp.com") OrElse
                   lowerUrl.StartsWith("https://discord.com") OrElse
                   lowerUrl.StartsWith("https://discordapp.com") Then

                    Dim discordURL As String = SupportingFeatures.DiscordURL(theURL)

                    Try
                        Process.Start(discordURL)
                        discordWorked = True
                    Catch ex As Exception
                        discordWorked = False
                    End Try
                End If

                If Not discordWorked Then
                    Process.Start(theURL)
                End If
            End If
        Catch ex As Exception
            isValid = False
        End Try

        Return isValid
    End Function

    ' Function to validate a URL
    Public Shared Function IsValidURL(url As String) As Boolean
        ' Regular expression pattern for a URL
        Dim pattern As String = "^(https?|ftp)://[^\s/$.?#].[^\s]*$"

        ' Check if the URL matches the pattern
        Return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase)
    End Function

    Public Shared Function ExtractMessageIDFromDiscordURL(ByVal inputURL As String, Optional acceptFirstPartOnly As Boolean = False, Optional testMode As Boolean = False) As String

        ' Check if the inputURL starts with the expected base URL
        Dim baseURL As String = $"https://discord.com/channels/{MSFSSoaringToolsDiscordID}/"

        If inputURL.StartsWith(baseURL) Then
            ' Remove the base URL
            Dim remainingURL As String = inputURL.Substring(baseURL.Length)

            ' Split the remaining URL by '/'
            Dim parts As String() = remainingURL.Split("/"c)

            ' Check if there are 1 or 2 parts
            If parts.Length = 2 AndAlso (parts(0) = MSFSSoaringToolsLibraryID OrElse (parts(0) = MSFSSoaringToolsTestingID) OrElse (Debugger.IsAttached AndAlso parts(0) = MSFSSoaringToolsPrivateTestingID)) Then
                ' Two parts, the first is the library ID and the second is the message ID
                ' Check if part 0 is valid for the context
                If Debugger.IsAttached AndAlso (Not testMode) AndAlso (parts(0) = MSFSSoaringToolsLibraryID OrElse parts(0) = MSFSSoaringToolsPrivateTestingID) Then
                    'debugger mode and not test mode, allow official and private testing channels only
                    Return parts(1)
                End If
                If testMode AndAlso parts(0) = MSFSSoaringToolsTestingID Then
                    'in test mode, only allow testing channel ID and prefix it with "T"
                    Return parts(1)
                End If
                If Not testMode AndAlso parts(0) = MSFSSoaringToolsLibraryID Then
                    'in real prod mode, only allow official library ID
                    Return parts(1)
                End If
                'In every other situation, return blank
                Return String.Empty
            ElseIf parts.Length = 1 AndAlso acceptFirstPartOnly Then
                Return parts(0)
            End If
        End If

        ' Return blank if URL doesn't match the expected format
        Return String.Empty
    End Function

    Public Shared Function GetWeSimGlideTaskURL(entrySeqID As Integer) As String
        Return $"{WeSimGlide}index.html?task={entrySeqID.ToString.Trim}"
    End Function

    Public Shared Function GetWeSimGlideEventURL(eventKey As String) As String
        Return $"{WeSimGlide}index.html?event={eventKey.Trim}"
    End Function

    Public Shared ReadOnly Property GetMSFSSoaringToolsDiscordID As String
        Get
            Return MSFSSoaringToolsDiscordID
        End Get
    End Property

    Public Shared Function ReturnDiscordServer(urlExtract As String, Optional forceMSFSSoaringTools As Boolean = False) As String

        If urlExtract.Contains("channels/793376245915189268/1097354088892596234") Then
            Return "Got Gravel's Friday Soaring Club"
        ElseIf urlExtract.Contains("channels/793376245915189268/1097353400015921252") Then
            Return "Got Gravel's Diamonds Club"
        ElseIf urlExtract.Contains("channels/793376245915189268") Then
            Return "Got Gravel"

        ElseIf urlExtract.Contains($"channels/{MSFSSoaringToolsDiscordID}") OrElse forceMSFSSoaringTools Then
            Return "MSFS Soaring Task Tools"

        ElseIf urlExtract.Contains("channels/876123356385149009/1128345453063327835") Then
            Return "Wednesday's Sim Soaring Club"
        ElseIf urlExtract.Contains("channels/876123356385149009/987611111509590087") Then
            Return "Saturday's Sim Soaring Club"
        ElseIf urlExtract.Contains("channels/876123356385149009/1066655140733517844") Then
            Return "AusGlide's Sim Soaring Club"
        ElseIf urlExtract.Contains("channels/876123356385149009") Then
            Return "Sim Soaring Club"

        ElseIf urlExtract.Contains("channels/732949977768132660/1197570373525459106") Then
            Return "MSFS 20-24 Planeur"
        ElseIf urlExtract.Contains("channels/732949977768132660") Then
            Return "MSFS 20-24"

        ElseIf urlExtract.Contains("channels/325227457445625856") Then
            Return "UKVGA"

        Else
            Return "unknown"
        End If

    End Function

    Public Shared Sub BringWindowToFront(theForm As Form)
        If theForm.WindowState = FormWindowState.Minimized Then theForm.WindowState = FormWindowState.Normal
        theForm.BringToFront()
        theForm.Activate()
    End Sub

    ' Function to bring a specific window to the top and unminimize it if minimized
    Public Shared Function BringWindowToTopWithPartialTitle(partialTitle As String) As IntPtr
        Dim targetHandle As IntPtr = IntPtr.Zero

        NativeMethods.EnumWindows(Function(hWnd, lParam)
                                      Dim sbTitle As New StringBuilder(1024)
                                      NativeMethods.GetWindowText(hWnd, sbTitle, sbTitle.Capacity)
                                      Dim title As String = sbTitle.ToString()

                                      If title.Contains(partialTitle) AndAlso NativeMethods.IsWindowVisible(hWnd) Then
                                          targetHandle = hWnd
                                          Return False ' Return False to stop enumerating windows
                                      End If

                                      Return True ' Continue enumerating windows
                                  End Function, 0)

        If targetHandle <> IntPtr.Zero Then
            NativeMethods.SetForegroundWindow(targetHandle)
        End If

        Return targetHandle
    End Function
    Public Shared Function BringDiscordToTop() As IntPtr
        Return SupportingFeatures.BringWindowToTopWithPartialTitle(" - Discord")
    End Function

    Public Shared Sub BringDPHToolToTop(handle As IntPtr)
        NativeMethods.SetForegroundWindow(handle)
    End Sub

    Public Shared Function GetTextPartFromURLMarkdown(urlMarkdown As String) As String
        ' Define a regular expression pattern to match text inside brackets
        Dim pattern As String = "\[([^]]+)\]\(([^)]+)\)"

        ' Match the pattern in the input string
        Dim match As Match = Regex.Match(urlMarkdown, pattern)

        ' Check if a match was found
        If match.Success Then
            ' The first captured group (index 1) contains the text inside brackets
            Return match.Groups(1).Value
        Else
            ' No match found, return blank
            Return String.Empty
        End If
    End Function

    Public Shared Function ReturnTextFromURLMarkdown(fullMarkdownText As String) As String

        Dim result As String

        ' Define a regular expression pattern to match text inside square brackets
        Dim pattern As String = "\[(.*?)\]"

        ' Search for matches in the inputText
        Dim match As Match = Regex.Match(fullMarkdownText, pattern)

        ' Check if a match was found
        If match.Success Then
            ' Extract the text inside the square brackets
            Dim extractedText As String = match.Groups(1).Value
            result = extractedText
        Else
            ' No match found, handle this case as needed
            result = fullMarkdownText
        End If

        Return result

    End Function

    Public Shared Function SanitizeFilePath(filePath As String) As String

        If String.IsNullOrEmpty(filePath) Then
            Return filePath
        End If

        Try
            Dim directoryPath As String = Path.GetDirectoryName(filePath)
            Dim fileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(filePath)
            Dim fileExtension As String = Path.GetExtension(filePath)

            Dim cleanedFileName As String = If(fileNameWithoutExtension, String.Empty).Trim()

            Dim trimmedExtension As String = If(fileExtension, String.Empty).Trim()
            Dim cleanedExtensionWithoutDot As String = If(trimmedExtension.StartsWith("."), trimmedExtension.Substring(1), trimmedExtension)
            cleanedExtensionWithoutDot = cleanedExtensionWithoutDot.Trim()

            Dim cleanedExtension As String = If(String.IsNullOrEmpty(cleanedExtensionWithoutDot), String.Empty, $".{cleanedExtensionWithoutDot}")

            If String.IsNullOrWhiteSpace(cleanedFileName) Then
                Using New Centered_MessageBox()
                    MessageBox.Show("The application cannot accept the file as it is because the filename is empty after cleaning.", "Invalid file name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End If

            Dim recomposedFileName As String = $"{cleanedFileName}{cleanedExtension}"
            Dim sanitizedPath As String = If(String.IsNullOrEmpty(directoryPath), recomposedFileName, Path.Combine(directoryPath, recomposedFileName))

            If String.Equals(filePath, sanitizedPath, StringComparison.Ordinal) Then
                Return sanitizedPath
            End If

            If Not File.Exists(filePath) Then
                Using New Centered_MessageBox()
                    MessageBox.Show($"The application cannot accept the file as it is because it cannot be found:{Environment.NewLine}{filePath}", "Invalid file name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End If

            If File.Exists(sanitizedPath) Then
                Using New Centered_MessageBox()
                    MessageBox.Show($"The application cannot accept the file as it is because a file with the sanitized name already exists:{Environment.NewLine}{sanitizedPath}", "Invalid file name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End If

            File.Move(filePath, sanitizedPath)

            Return sanitizedPath
        Catch ex As Exception
            Using New Centered_MessageBox()
                MessageBox.Show($"The application cannot accept the file as it is.{Environment.NewLine}{ex.Message}", "Invalid file name", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return String.Empty
        End Try

    End Function

    Public Shared Function ValidateFileName(filename As String) As String

        Dim reasons As New List(Of String)

        ' Check for empty string
        If String.IsNullOrWhiteSpace(filename) Then
            reasons.Add("Filename is empty or only whitespace.")
        End If

        ' Check for invalid characters
        Dim invalidChars As String = New String(Path.GetInvalidFileNameChars())

        For Each invalidChar In invalidChars
            If filename.Contains(invalidChar) Then
                reasons.Add($"Filename contains invalid character: '{invalidChar}'")
            End If
        Next

        ' Check for reserved names
        Dim reservedNames As String() = {"CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"}
        Dim fileNameWithoutExtension As String = Path.GetFileNameWithoutExtension(filename).ToUpperInvariant()
        If reservedNames.Contains(fileNameWithoutExtension) Then
            reasons.Add($"Filename is a reserved name: '{filename}'")
        End If

        ' Check for trailing spaces or periods
        If filename.EndsWith(" ") OrElse filename.EndsWith(".") Then
            reasons.Add("Filename ends with a space or period.")
        End If

        ' Additional checks for filenames like "COM1.txt" which are also reserved
        If Regex.IsMatch(filename.ToUpperInvariant(), "^(COM[1-9]|LPT[1-9]|CON|PRN|AUX|NUL)(\..+)?$", RegexOptions.IgnoreCase) Then
            reasons.Add($"Filename is a reserved system name: '{filename}'")
        End If

        ' Return the reasons or an empty string if valid
        If reasons.Count > 0 Then
            Return String.Join(Environment.NewLine, reasons)
        Else
            Return String.Empty
        End If

    End Function

    Public Shared Function GetEnUSFormattedDate(theDate As Date, theTime As Date, includeYear As Boolean) As String

        Dim _EnglishCulture As New CultureInfo("en-US")

        Dim dateFormat As String
        If includeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Return $"{theDate.ToString(dateFormat, _EnglishCulture)}, {theTime.ToString("hh:mm tt", _EnglishCulture)}"

    End Function

    Public Shared Sub BuildCloudAndWindLayersDatagrids(_weatherDetails As WeatherDetails, windLayersDatagrid As DataGridView, cloudLayersDatagrid As DataGridView, specPrefUnits As PreferredUnits)

        'Build wind layers grid
        Dim dtWinds As New DataTable()
        dtWinds.Columns.Add("#", GetType(Integer))
        Select Case specPrefUnits.Altitude
            Case AltitudeUnits.Both
                dtWinds.Columns.Add("Altitude (f / m)", GetType(String))
            Case AltitudeUnits.Metric
                dtWinds.Columns.Add("Altitude (m)", GetType(String))
            Case AltitudeUnits.Imperial
                dtWinds.Columns.Add("Altitude (f)", GetType(String))
        End Select
        Select Case specPrefUnits.WindSpeed
            Case WindSpeedUnits.Both
                dtWinds.Columns.Add("Speed (kts / m/s)", GetType(String))
            Case WindSpeedUnits.Knots
                dtWinds.Columns.Add("Speed (kts)", GetType(String))
            Case WindSpeedUnits.MeterPerSecond
                dtWinds.Columns.Add("Speed (m/s)", GetType(String))
        End Select
        dtWinds.Columns.Add("Angle", GetType(String))
        dtWinds.Columns.Add("Gust", GetType(String))
        Dim seqWindL As Integer = 0
        For Each windL As WindLayer In _weatherDetails.WindLayers
            seqWindL += 1
            dtWinds.Rows.Add(seqWindL, windL.AltitudeCorrectUnit(specPrefUnits), windL.SpeedCorrectUnit(specPrefUnits), windL.Angle.ToString, windL.GetGustText(specPrefUnits))
        Next
        windLayersDatagrid.DataSource = dtWinds
        windLayersDatagrid.Font = New Font(windLayersDatagrid.Font.FontFamily, 12)
        windLayersDatagrid.RowTemplate.Height = 28
        windLayersDatagrid.RowHeadersVisible = False
        windLayersDatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        windLayersDatagrid.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        windLayersDatagrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        windLayersDatagrid.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        windLayersDatagrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells

        'Build cloud layers grid
        Dim dtClouds As New DataTable()
        dtClouds.Columns.Add("#", GetType(Integer))
        Select Case specPrefUnits.Altitude
            Case AltitudeUnits.Both
                dtClouds.Columns.Add("Base (f / m)", GetType(String))
                dtClouds.Columns.Add("Top (f / m)", GetType(String))
            Case AltitudeUnits.Metric
                dtClouds.Columns.Add("Base (m)", GetType(String))
                dtClouds.Columns.Add("Top (m)", GetType(String))
            Case AltitudeUnits.Imperial
                dtClouds.Columns.Add("Base (f)", GetType(String))
                dtClouds.Columns.Add("Top (f)", GetType(String))
        End Select
        dtClouds.Columns.Add("Coverage", GetType(String))
        dtClouds.Columns.Add("Density", GetType(String))
        dtClouds.Columns.Add("Scattering", GetType(String))
        Dim seqCloudL As Integer = 0
        For Each CloudL As CloudLayer In _weatherDetails.CloudLayers
            seqCloudL += 1
            dtClouds.Rows.Add(seqCloudL, CloudL.AltitudeBottomCorrectUnit(specPrefUnits), CloudL.AltitudeTopCorrectUnit(specPrefUnits), CloudL.CoverageForGrid, CloudL.DensityForGrid, CloudL.ScatteringForGrid)
        Next
        cloudLayersDatagrid.DataSource = dtClouds
        cloudLayersDatagrid.Font = New Font(cloudLayersDatagrid.Font.FontFamily, 12)
        cloudLayersDatagrid.RowTemplate.Height = 28
        cloudLayersDatagrid.RowHeadersVisible = False
        cloudLayersDatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        cloudLayersDatagrid.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        cloudLayersDatagrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        cloudLayersDatagrid.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        cloudLayersDatagrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        cloudLayersDatagrid.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells

    End Sub

    Public Shared Function GetMSLPressure(MSLPressureInPa As Single, Optional prefUnits As PreferredUnits = Nothing, Optional forceinHg As Boolean = False, Optional forcehPa As Boolean = False) As String

        If forcehPa Then
            Return String.Format("{0:N0} hPa", MSLPressureInPa / 100)
        End If
        If forceinHg Then
            Return String.Format("{0:F2} inHg", Conversions.PaToInHg(MSLPressureInPa))
        End If
        If prefUnits Is Nothing OrElse prefUnits.Barometric = BarometricUnits.Both Then
            Return String.Format("{0:F2} inHg / {1:N0} hPa", Conversions.PaToInHg(MSLPressureInPa), MSLPressureInPa / 100)
        Else
            Select Case prefUnits.Barometric
                Case BarometricUnits.hPa
                    Return String.Format("{0:N0} hPa", MSLPressureInPa / 100)
                Case BarometricUnits.inHg
                    Return String.Format("{0:F2} inHg", Conversions.PaToInHg(MSLPressureInPa))
            End Select
        End If
        Return String.Empty

    End Function

    Public Shared Function MSLTemperature(MSLTempKelvin As Single, Optional prefUnits As PreferredUnits = Nothing, Optional forceF As Boolean = False, Optional forceC As Boolean = False) As String

        If forceF Then
            Return String.Format("{0:N0}°F", Conversions.KelvinToFarenheit(MSLTempKelvin))
        End If
        If forceC Then
            Return String.Format("{0:N0}°C", Conversions.KelvinToCelsius(MSLTempKelvin))
        End If
        If prefUnits Is Nothing OrElse prefUnits.Temperature = TemperatureUnits.Both Then
            Return String.Format("{0:N0}°C / {1:N0}°F", Conversions.KelvinToCelsius(MSLTempKelvin), Conversions.KelvinToFarenheit(MSLTempKelvin))
        Else
            Select Case prefUnits.Temperature
                Case TemperatureUnits.Celsius
                    Return String.Format("{0:N0}°C", Conversions.KelvinToCelsius(MSLTempKelvin))
                Case TemperatureUnits.Fahrenheit
                    Return String.Format("{0:N0}°F", Conversions.KelvinToFarenheit(MSLTempKelvin))
            End Select
        End If
        Return String.Empty
    End Function

    Public Shared Function GetSnowCover(SnowCover As Single)
        Dim cover As String
        If SnowCover = 0 Then
            cover = "0"
        Else
            cover = String.Format("{0:N0} inches / {1:N2} m", Conversions.MeterToInches(SnowCover), SnowCover)
        End If

        Return cover

    End Function

    Public Shared Function IsValidFolderOrFileName(name As String) As Boolean
        ' Check for null or empty string
        If String.IsNullOrEmpty(name) Then Return False

        ' Check if the name contains any invalid characters
        Dim invalidChars = Path.GetInvalidFileNameChars()
        If name.Any(Function(ch) invalidChars.Contains(ch)) Then Return False

        ' Check for reserved names. Extend this list based on your requirements.
        Dim reservedNames As String() = {"CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"}
        Dim nameWithoutExtension As String = Path.GetFileNameWithoutExtension(name).ToUpperInvariant()
        If reservedNames.Contains(nameWithoutExtension) Then Return False

        ' If all checks passed, the name is valid
        Return True
    End Function

    ' Helper function to format TimeSpan as text
    Public Shared Function FormatTimeSpanAsText(duration As TimeSpan) As String
        Dim result As String = ""

        If duration.Hours > 0 Then
            result &= $"{duration.Hours} hour"
            If duration.Hours > 1 Then
                result &= "s"
            End If
        End If

        If duration.Minutes > 0 Then
            If result <> "" Then
                result &= " "
            End If
            result &= $"{duration.Minutes} minute"
            If duration.Minutes > 1 Then
                result &= "s"
            End If
        End If

        If result = "" Then
            result = "0 minutes"
        End If

        Return result
    End Function

    Public Shared Function GetSoaringTypesSelected(Ridge As Boolean, Thermals As Boolean, Waves As Boolean, Dynamic As Boolean) As String
        Dim selectedTypes As New List(Of String)

        If Ridge Then
            selectedTypes.Add("Ridge")
        End If

        If Thermals Then
            selectedTypes.Add("Thermal")
        End If

        If Waves Then
            selectedTypes.Add("Wave")
        End If

        If Dynamic Then
            selectedTypes.Add("Dynamic")
        End If

        ' Join the selected types into a single string, separated by " and "
        Return String.Join(", ", selectedTypes)
    End Function

    Public Shared Function GetDPHLastUpdateFromDPHXFile(dphxFilePath As String) As String
        Using archive As ZipArchive = ZipFile.OpenRead(dphxFilePath)
            For Each entry As ZipArchiveEntry In archive.Entries
                If Path.GetExtension(entry.Name) = ".dph" Then
                    Return entry.LastWriteTime.DateTime.ToUniversalTime.ToString
                End If
            Next
        End Using
        Return String.Empty
    End Function

    Public Shared useTestServer As Boolean = False
    Private Shared _testServerAskedOnce As Boolean = False

    Private Shared Sub AskTestServer()
        If Not _testServerAskedOnce Then
            If MsgBox("Do you want to run in TEST environment ?", vbYesNo Or vbQuestion, "Confirm TEST environment") = vbYes Then
                useTestServer = True
            Else
                useTestServer = False
            End If
            _testServerAskedOnce = True
        End If
    End Sub

    Public Shared ReadOnly Property UsingTestServer As Boolean
        Get
            If Debugger.IsAttached Then
                AskTestServer()
            End If
            Return useTestServer
        End Get
    End Property

    Public Shared Function SIGLRDiscordPostHelperFolder() As String
        If Debugger.IsAttached Then
            AskTestServer()
            If useTestServer Then
                Return "https://siglr.com/DiscordPostHelperTest/"
            Else
                Return "https://siglr.com/DiscordPostHelper/"
            End If
        End If
        Return "https://siglr.com/DiscordPostHelper/"
    End Function
    Public Shared Function TasksDatabase() As String

        If Debugger.IsAttached Then
            AskTestServer()
            If useTestServer Then
                Return "TasksDatabaseTest.db"
            Else
                Return "TasksDatabase.db"
            End If
        End If
        Return "TasksDatabase.db"
    End Function

    Public Shared Function WeSimGlideView() As String
        If Debugger.IsAttached Then
            AskTestServer()
            Return $"{WeSimGlide}/integrated.html?appContext=true"
        End If
        Return $"{WeSimGlide}integrated.html?appContext=true"

    End Function

    Public Shared Function DownloadTaskFile(taskID As String, taskTitle As String, localFolder As String) As String
        Dim baseUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}TaskBrowser/Tasks/"
        Dim remoteFileName As String = taskID & ".dphx"
        Dim localFileName As String = taskTitle & ".dphx"
        Dim remoteFileUrl As String = baseUrl & remoteFileName
        Dim localFilePath As String = Path.Combine(localFolder, localFileName)

        Try
            Cursor.Current = Cursors.WaitCursor

            ' Create the directory if it doesn't exist
            If Not Directory.Exists(localFolder) Then
                Directory.CreateDirectory(localFolder)
            End If

            ' Download the file
            Using client As New WebClient()
                client.DownloadFile(remoteFileUrl, localFilePath)
            End Using

            ' Check if the file exists
            If File.Exists(localFilePath) Then
                Return localFilePath
            Else
                Throw New Exception("Failed to download the task file.")
            End If

            Cursor.Current = Cursors.Default

        Catch ex As Exception
            ' Handle the exception (e.g., log the error)
            Cursor.Current = Cursors.Default
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred while downloading the task file:{Environment.NewLine}{ex.Message}", "Downloading task", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return String.Empty
        End Try
    End Function

    Public Shared ReadOnly Property WeSimGlide As String
        Get
            If useTestServer Then
                Return "https://soaring.siglr.com/"
            Else
                Return "https://wesimglide.org/"
            End If
        End Get
    End Property

    Public Shared Function LocalDateTimeMatch(ByVal localDate As String, ByVal localTime As String, ByVal simDateTime As String) As Boolean

        ' 0) Quick null/empty guard
        If String.IsNullOrWhiteSpace(localDate) _
           OrElse String.IsNullOrWhiteSpace(localTime) _
           OrElse String.IsNullOrWhiteSpace(simDateTime) Then
            Return False
        End If

        ' 1) Parse the task’s datetime (ignore year)
        Dim taskDT As DateTime
        If Not DateTime.TryParse(simDateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, taskDT) Then
            Return False
        End If
        Dim normalizedTask = New DateTime(2000, taskDT.Month, taskDT.Day, taskDT.Hour, taskDT.Minute, taskDT.Second, DateTimeKind.Unspecified)

        ' 2) Extract month & day from localDate
        Dim m = Regex.Match(localDate, "^(\d{4})-(\d{2})-(\d{2})$")
        If Not m.Success Then Return False
        Dim month = Integer.Parse(m.Groups(2).Value)
        Dim day = Integer.Parse(m.Groups(3).Value)

        ' 3) Extract hh, mm, ss from localTime ("HHmmss")
        If localTime.Length < 6 Then Return False
        Dim hh = Integer.Parse(localTime.Substring(0, 2))
        Dim mm = Integer.Parse(localTime.Substring(2, 2))
        Dim ss = Integer.Parse(localTime.Substring(4, 2))

        ' 4) Build the IGC DateTime using a dummy year
        Dim igcDT = New DateTime(2000, month, day, hh, mm, ss, DateTimeKind.Unspecified)

        ' 5) Compare absolute difference ≤ 60 minutes (allowing ±1 day)
        Dim deltas As Double() = {
            Math.Abs((normalizedTask - igcDT).TotalMinutes),
            Math.Abs((normalizedTask.AddDays(1) - igcDT).TotalMinutes),
            Math.Abs((normalizedTask.AddDays(-1) - igcDT).TotalMinutes)
        }

        Dim smallestDelta = Math.Min(deltas(0), Math.Min(deltas(1), deltas(2)))
        Return smallestDelta <= 60
    End Function

    ' --------- Public overload: takes datePart="yyyy-MM-dd" and timePart="HHmmss" ---------
    Public Shared Function FormatDateWithoutYearSecondsAndWeekday(datePart As String, timePart As String) As String

        Dim dtDate As DateTime
        ' 1) Parse the date
        If Not DateTime.TryParseExact(
           datePart, "yyyy-MM-dd",
           CultureInfo.InvariantCulture,
           DateTimeStyles.None,
           dtDate
       ) Then
            Return datePart
        End If

        ' 2) Parse the time (HHmmss or at least HHmm)
        Dim hh As Integer = 0, mm As Integer = 0, ss As Integer = 0
        If Not String.IsNullOrEmpty(timePart) AndAlso timePart.Length >= 4 Then
            Try
                hh = Integer.Parse(timePart.Substring(0, 2))
                mm = Integer.Parse(timePart.Substring(2, 2))
                If timePart.Length >= 6 Then ss = Integer.Parse(timePart.Substring(4, 2))
            Catch
                ' leave at zero on parse errors
            End Try
        End If

        ' 3) Combine into a single DateTime
        Dim combined As New DateTime(
        dtDate.Year, dtDate.Month, dtDate.Day,
        hh, mm, ss,
        DateTimeKind.Unspecified
    )

        ' 4) Delegate to the DateTime overload
        Return FormatDateWithoutYearSecondsAndWeekday(combined)
    End Function

    ' --------- Public overload: takes a DateTime directly ---------
    Public Shared Function FormatDateWithoutYearSecondsAndWeekday(dateTime As DateTime) As String
        Return FormatDateWithoutYearSecondsAndWeekdayCore(dateTime)
    End Function

    ' --------- Private shared core ---------
    Private Shared Function FormatDateWithoutYearSecondsAndWeekdayCore(dt As DateTime) As String

        Dim culture = CultureInfo.CurrentCulture
        Dim dtf = culture.DateTimeFormat

        ' Month‐day pattern (e.g. "MMMM dd" or "dd MMMM") → drop leading zero
        Dim mdPattern = dtf.MonthDayPattern.Replace("dd", "d")

        ' Time pattern (e.g. "H:mm" or "h:mm tt") → keep as‐is (no seconds)
        Dim tmPattern = dtf.ShortTimePattern

        ' Combine with comma: "May 15, 7:06 PM" or "15 mai, 19:06"
        Dim pattern = $"{mdPattern}, {tmPattern}"

        Return dt.ToString(pattern, culture)
    End Function

    Public Shared Function FormatUserDateTime(dt As DateTime, isUtc As Boolean) As String
        Dim localDt As DateTime =
            If(isUtc,
               DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToLocalTime(),
               DateTime.SpecifyKind(dt, DateTimeKind.Local))

        Dim c = CultureInfo.CurrentCulture
        Dim df = c.DateTimeFormat

        ' Date: keep user's style, ensure year, remove weekday if present
        Dim dateP = df.ShortDatePattern
        dateP = Regex.Replace(dateP, "\s*,?\s*(dddd|ddd)", "", RegexOptions.IgnoreCase)
        If dateP.IndexOf("y"c) = -1 Then dateP &= df.DateSeparator & "yyyy"

        ' Time: user's style, remove seconds + trailing separator
        Dim timeP = df.ShortTimePattern
        timeP = Regex.Replace(timeP, "(\s*[:\.]\s*)s{1,2}(\.f+)?", "", RegexOptions.IgnoreCase)
        timeP = Regex.Replace(timeP, "[:\.]\s*$", "").Trim()

        Dim pattern = (dateP & " " & timeP).Trim()
        Return localDt.ToString(pattern, c)
    End Function

    ' Overload: input as "yyyy-MM-dd HH:mm:ss" OR "yyyy-MM-dd HH:mm"
    Public Shared Function FormatUserDateTime(dtText As String, isUtc As Boolean) As String
        If String.IsNullOrWhiteSpace(dtText) Then Return String.Empty

        Dim formats() As String = {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm"
    }

        Dim dt As DateTime
        If Not DateTime.TryParseExact(dtText.Trim(),
                                  formats,
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.None,
                                  dt) Then
            ' Last-resort: try a generic parse, otherwise return original
            If Not DateTime.TryParse(dtText, CultureInfo.InvariantCulture, DateTimeStyles.None, dt) Then
                Return dtText
            End If
        End If

        ' Ensure a sensible Kind if none was present in the text
        If dt.Kind = DateTimeKind.Unspecified Then
            dt = DateTime.SpecifyKind(dt, If(isUtc, DateTimeKind.Utc, DateTimeKind.Local))
        End If

        Return FormatUserDateTime(dt, isUtc)
    End Function

    ''' <summary>
    ''' Returns True if the event is still “active” (i.e. enabled and not more than 
    ''' 90 minutes past its anchor point), False if no event or it’s older than 90 min.
    ''' </summary>
    Public Shared Function IsEventActive(sessionData As AllData) As Boolean
        ' 1) No event at all?
        If Not sessionData.EventEnabled Then
            Return False
        End If

        ' 2) Pick the correct anchor time
        Dim anchor As DateTime
        If sessionData.UseEventLaunch Then
            anchor = sessionData.LaunchLocalDateTime
        ElseIf sessionData.UseEventSyncFly Then
            anchor = sessionData.SyncFlyLocalDateTime
        Else
            anchor = sessionData.MeetLocalDateTime
        End If

        ' 3) Compute minutes difference: positive if in the future, negative if past
        Dim minutesDiff = (anchor - DateTime.Now).TotalMinutes

        ' 4) Active if ≥ –90 (i.e. up to 90 minutes in the past) or any time in the future
        Return minutesDiff >= -90

    End Function

    Public Shared Function FetchTaskIDUsingEntrySeqID(entrySeqID As String, Optional ByRef taskTitle As String = "", Optional onlyAvailable As Boolean = True) As String

        Dim onlyAvailableInt As Integer = If(onlyAvailable, 1, 0)

        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingEntrySeqID.php?EntrySeqID={entrySeqID}&OnlyAvailable={onlyAvailableInt}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()

                    ' Parse the JSON response to extract the TaskID
                    Dim json As JObject = JObject.Parse(jsonResponse)

                    ' Check if the status is "success" and return the TaskID, else return "0"
                    If json("status").ToString() = "success" Then
                        taskTitle = json("taskDetails")("Title").ToString()
                        Return json("taskDetails")("TaskID").ToString()
                    Else
                        ' Return Empty string if no task was found
                        Return String.Empty
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception (you could log this or handle it differently based on your needs)
            Return String.Empty ' Return Empty string if an exception occurs
        End Try
    End Function

    Public Shared Function ConvertImageToJpg(imagePath As String, Optional maxWidth As Integer = 1920, Optional maxHeight As Integer = 1080) As String
        Try
            If String.IsNullOrEmpty(imagePath) OrElse Not File.Exists(imagePath) Then
                Return String.Empty ' No cover image provided
            End If

            Dim tempJpgPath As String = Path.Combine(Path.GetTempPath(), "cover_image.jpg")

            ' Load the image
            Using img As Image = Image.FromFile(imagePath)
                ' Resize if needed
                Dim resizedImg As Image = ResizeImageIfNeeded(img, maxWidth, maxHeight)

                ' Save as JPG with specified quality
                SaveJpegWithQuality(resizedImg, tempJpgPath, 80L)

                ' Dispose resized image if it was created separately
                If Not Object.ReferenceEquals(img, resizedImg) Then
                    resizedImg.Dispose()
                End If
            End Using

            Return tempJpgPath
        Catch ex As Exception
            MessageBox.Show($"Failed to process cover image: {ex.Message}", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return String.Empty
        End Try
    End Function

    Public Shared Sub SaveJpegWithQuality(image As Drawing.Image, filePath As String, quality As Long)
        Dim qualityParam As New System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality)
        Dim jpegCodec As System.Drawing.Imaging.ImageCodecInfo = GetEncoderInfo("image/jpeg")
        Dim encoderParams As New System.Drawing.Imaging.EncoderParameters(1)
        encoderParams.Param(0) = qualityParam

        image.Save(filePath, jpegCodec, encoderParams)
    End Sub

    Private Shared Function ResizeImageIfNeeded(img As Image, maxWidth As Integer, maxHeight As Integer) As Image
        ' Check if resizing is needed
        If img.Width <= maxWidth AndAlso img.Height <= maxHeight Then
            Return img ' No resizing needed
        End If

        ' Calculate new dimensions while maintaining aspect ratio
        Dim ratioX As Double = maxWidth / img.Width
        Dim ratioY As Double = maxHeight / img.Height
        Dim ratio As Double = Math.Min(ratioX, ratioY)

        Dim newWidth As Integer = CInt(img.Width * ratio)
        Dim newHeight As Integer = CInt(img.Height * ratio)

        ' Create a new resized bitmap
        Dim resizedImg As New Bitmap(newWidth, newHeight)
        Using g As Graphics = Graphics.FromImage(resizedImg)
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.DrawImage(img, 0, 0, newWidth, newHeight)
        End Using

        Return resizedImg
    End Function

    Private Shared Function GetEncoderInfo(mimeType As String) As Imaging.ImageCodecInfo
        Dim codecs As Imaging.ImageCodecInfo() = Imaging.ImageCodecInfo.GetImageEncoders()
        For Each codec As Imaging.ImageCodecInfo In codecs
            If codec.MimeType = mimeType Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    ' Distance in meters between two waypoints (uses your Conversions helper)
    Public Shared Function HaversineMeters(a As ATCWaypoint, b As ATCWaypoint) As Double
        Return Conversions.GetDistanceInKm(a.Latitude, a.Longitude, b.Latitude, b.Longitude) * 1000.0
    End Function

    Public Shared Function DiscordURL(originalURL As String, Optional replaceHTTPWithDiscord As Boolean = True) As String
        Dim httpOrDiscord As String = "discord"
        If Not replaceHTTPWithDiscord Then
            Dim lower = originalURL.ToLowerInvariant()
            If lower.StartsWith("http://") Then
                httpOrDiscord = "http"
            ElseIf lower.StartsWith("https://") Then
                httpOrDiscord = "https"
            End If
        End If
        Dim computedDiscordURL As String = originalURL
        computedDiscordURL = computedDiscordURL.Replace("http://discord.com", $"{httpOrDiscord}://discord.com")
        computedDiscordURL = computedDiscordURL.Replace("https://discord.com", $"{httpOrDiscord}://discord.com")
        computedDiscordURL = computedDiscordURL.Replace("http://discordapp.com", $"{httpOrDiscord}://discord.com")
        computedDiscordURL = computedDiscordURL.Replace("https://discordapp.com", $"{httpOrDiscord}://discord.com")
        Return computedDiscordURL
    End Function

    ' Very lightweight XML normalization for “identical” test
    Public Shared Function NormalizeXml(xml As String) As String
        If String.IsNullOrWhiteSpace(xml) Then Return String.Empty
        ' Trim + collapse runs of whitespace between tags; normalize newlines
        Dim s = xml.Replace(vbCr, "").Replace(vbLf, "\n").Trim()
        s = Regex.Replace(s, ">\s+<", "><")
        s = Regex.Replace(s, "\s{2,}", " ")
        Return s
    End Function

    Public Shared Function FilesAreEquivalent(pathA As String, pathB As String) As Boolean
        If String.IsNullOrWhiteSpace(pathA) OrElse String.IsNullOrWhiteSpace(pathB) Then Return False
        If String.Equals(System.IO.Path.GetFullPath(pathA), System.IO.Path.GetFullPath(pathB), StringComparison.OrdinalIgnoreCase) Then Return True
        If Not File.Exists(pathA) OrElse Not File.Exists(pathB) Then Return False
        If ByteIdentical(pathA, pathB) Then Return True
        If IsXmlCompareCandidate(pathA) AndAlso IsXmlCompareCandidate(pathB) Then
            Return XmlSemanticallyEqual(pathA, pathB)
        End If
        Return False
    End Function

    Private Shared Function IsXmlCompareCandidate(path As String) As Boolean
        Dim ext As String = System.IO.Path.GetExtension(path)
        If String.IsNullOrEmpty(ext) Then Return False
        ext = ext.ToLowerInvariant()
        Return (ext = ".pln" OrElse ext = ".wpr")
    End Function

    Private Shared Function ByteIdentical(pathA As String, pathB As String) As Boolean
        Dim ia As New FileInfo(pathA)
        Dim ib As New FileInfo(pathB)
        If Not ia.Exists OrElse Not ib.Exists Then Return False
        If ia.Length <> ib.Length Then Return False
        Return String.Equals(GetSha256Hex(pathA), GetSha256Hex(pathB), StringComparison.Ordinal)
    End Function

    Private Shared Function GetSha256Hex(path As String) As String
        Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            Using sha As SHA256 = SHA256.Create()
                Dim bytes = sha.ComputeHash(fs)
                Dim sb As New StringBuilder(bytes.Length * 2)
                For Each b As Byte In bytes : sb.Append(b.ToString("x2")) : Next
                Return sb.ToString()
            End Using
        End Using
    End Function

    Private Shared Function XmlSemanticallyEqual(pathA As String, pathB As String) As Boolean
        Try
            Dim settings As New XmlReaderSettings() With {
                .IgnoreWhitespace = True,
                .IgnoreComments = True,
                .IgnoreProcessingInstructions = True
            }
            Dim aDoc As XDocument, bDoc As XDocument
            Using ra = XmlReader.Create(pathA, settings) : aDoc = XDocument.Load(ra, LoadOptions.None) : End Using
            Using rb = XmlReader.Create(pathB, settings) : bDoc = XDocument.Load(rb, LoadOptions.None) : End Using
            Dim aCanon As String = CanonicalizeElement(aDoc.Root)
            Dim bCanon As String = CanonicalizeElement(bDoc.Root)
            Return String.Equals(aCanon, bCanon, StringComparison.Ordinal)
        Catch
            Return False
        End Try
    End Function

    Private Shared Function CanonicalizeElement(el As XElement) As String
        Dim sb As New StringBuilder(4096)
        CanonicalizeElementRec(el, sb)
        Return sb.ToString()
    End Function

    Private Shared Sub CanonicalizeElementRec(el As XElement, sb As StringBuilder)
        sb.Append("E(").Append(el.Name.ToString()).Append(")")

        Dim attrs = el.Attributes().
            Where(Function(a) Not a.IsNamespaceDeclaration).
            OrderBy(Function(a) a.Name.NamespaceName).
            ThenBy(Function(a) a.Name.LocalName)

        For Each a In attrs
            sb.Append("|A(").Append(a.Name.ToString()).
               Append("="c).Append(EscapeForCanon(NormalizeWhitespace(a.Value))).
               Append(")")
        Next

        If Not el.HasElements Then
            Dim tv As String = NormalizeWhitespace(el.Value)
            If tv.Length > 0 Then sb.Append("|T(").Append(EscapeForCanon(tv)).Append(")")
        End If

        For Each child As XElement In el.Elements()
            sb.Append("|")
            CanonicalizeElementRec(child, sb)
        Next

        sb.Append("|/E")
    End Sub

    Private Shared Function NormalizeWhitespace(s As String) As String
        If String.IsNullOrEmpty(s) Then Return String.Empty
        Return Regex.Replace(s, "\s+", " ").Trim()
    End Function

    Private Shared Function EscapeForCanon(s As String) As String
        Return s.Replace("\", "\\").Replace(")", "\)")
    End Function

    Public Shared Function TryResolveContentDispositionFileName(contentDisposition As String) As String
        If String.IsNullOrWhiteSpace(contentDisposition) Then
            Return Nothing
        End If

        Dim parts = contentDisposition.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)

        Dim asciiFileName As String = Nothing
        Dim rfc5987FileName As String = Nothing

        For Each part In parts
            Dim trimmed = part.Trim()

            If trimmed.StartsWith("filename*=", StringComparison.OrdinalIgnoreCase) Then
                Dim value = trimmed.Substring("filename*=".Length)
                Dim encodingSeparator = value.IndexOf("''", StringComparison.Ordinal)

                If encodingSeparator >= 0 AndAlso encodingSeparator + 2 <= value.Length Then
                    Dim encoded = value.Substring(encodingSeparator + 2)
                    rfc5987FileName = Uri.UnescapeDataString(encoded.Trim(""""c))
                Else
                    rfc5987FileName = Uri.UnescapeDataString(value.Trim(""""c))
                End If

            ElseIf trimmed.StartsWith("filename=", StringComparison.OrdinalIgnoreCase) Then
                Dim value = trimmed.Substring("filename=".Length).Trim()
                asciiFileName = value.Trim(""""c)
            End If
        Next

        Dim candidate As String = Nothing

        ' Prefer RFC5987 filename*=
        If Not String.IsNullOrWhiteSpace(rfc5987FileName) Then
            candidate = rfc5987FileName

            ' Fallback to plain filename=
        ElseIf Not String.IsNullOrWhiteSpace(asciiFileName) Then
            candidate = asciiFileName

        Else
            ' Last resort: built-in parser
            Try
                Dim disposition As New System.Net.Mime.ContentDisposition(contentDisposition)
                If Not String.IsNullOrWhiteSpace(disposition.FileName) Then
                    candidate = disposition.FileName
                End If
            Catch ex As FormatException
                ' ignore
            End Try
        End If

        Return SanitizeContentDispositionFileName(candidate)
    End Function

    Private Shared Function SanitizeContentDispositionFileName(raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then
            Return Nothing
        End If

        ' Strip any directory / drive info
        Dim name = System.IO.Path.GetFileName(raw)

        ' Reject "." or ".." etc.
        If String.IsNullOrWhiteSpace(name) OrElse name = "." OrElse name = ".." Then
            Return Nothing
        End If

        ' Replace any invalid filename chars with "_"
        For Each ch In System.IO.Path.GetInvalidFileNameChars()
            name = name.Replace(ch, "_"c)
        Next

        Return name
    End Function

    Public Shared Function GetFlightPlanTitleFromPln(plnFilePath As String) As String

        If String.IsNullOrWhiteSpace(plnFilePath) OrElse Not File.Exists(plnFilePath) Then
            Return String.Empty
        End If

        Try
            Dim xdoc = XDocument.Load(plnFilePath, LoadOptions.PreserveWhitespace)

            ' Prefer Title under <FlightPlan.FlightPlan>
            Dim fp = xdoc.Descendants("FlightPlan.FlightPlan").FirstOrDefault()
            If fp IsNot Nothing Then
                Dim titleElem = fp.Element("Title")
                If titleElem IsNot Nothing Then
                    Dim title = titleElem.Value.Trim()
                    If title.Length > 0 Then
                        Return title
                    End If
                End If
            End If
            Return String.Empty

        Catch ex As Exception
            Return String.Empty
        End Try
    End Function
End Class

Public Class NativeMethods
        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Public Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
        End Function

        <DllImport("user32.dll")>
        Public Shared Function SetForegroundWindow(hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("user32.dll")>
        Public Shared Function IsIconic(hWnd As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll")>
        Public Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
        End Function

        Public Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As Integer) As Boolean

        <DllImport("user32.dll")>
        Public Shared Function EnumWindows(lpEnumFunc As EnumWindowsProc, lParam As Integer) As Boolean
        End Function

        <DllImport("user32.dll")>
        Public Shared Function GetWindowText(hWnd As IntPtr, lpString As StringBuilder, nMaxCount As Integer) As Integer
        End Function

        <DllImport("user32.dll")>
        Public Shared Function IsWindowVisible(hWnd As IntPtr) As Boolean
        End Function

    End Class
