Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Windows.Forms
Imports System.Xml

Public Class SupportingFeatures
    Public Enum DiscordTimeStampFormat As Integer
        TimeOnlyWithoutSeconds = 0
        FullDateTimeWithDayOfWeek = 1
        LongDateTime = 2
        CountDown = 3
    End Enum

    Public ReadOnly AirportsICAO As New Dictionary(Of String, String)
    Public ReadOnly DefaultKnownClubEvents As New Dictionary(Of String, PresetEvent)

    Public Sub New(Optional DiscordPostHelper As Boolean = True)

        If Not DiscordPostHelper Then
            LoadAirportsICAOFile()
            LoadDefaultClubEvents()
        End If

    End Sub

    Private Sub LoadDefaultClubEvents()

        DefaultKnownClubEvents.Add("TSC", New PresetEvent("TSC", "East USA", "Unicom 1", DayOfWeek.Wednesday, DateTime.Parse("1:00 AM"), 10, 0, 10, False))
        DefaultKnownClubEvents.Add("FSC", New PresetEvent("FSC", "West USA", "Unicom 3", DayOfWeek.Friday, DateTime.Parse("9:00 PM"), 30, 0, 0, False))
        DefaultKnownClubEvents.Add("SSC SATURDAY", New PresetEvent("SSC Saturday", "West Europe", "SSC Saturday", DayOfWeek.Saturday, DateTime.Parse("5:45 PM"), 15, 0, 30, True))
        DefaultKnownClubEvents.Add("AUS TUESDAYS", New PresetEvent("Aus Tuesdays", "Southeast Asia", "Flight 01", DayOfWeek.Tuesday, DateTime.Parse("8:30 AM"), 15, 0, 15, True))
        'DefaultKnownClubEvents.Add("DTS", New PresetEvent("DTS", "West USA", "Thermal Smashing", DayOfWeek.Tuesday, DateTime.Parse("8:30 AM"), True))

    End Sub

    Private Sub LoadAirportsICAOFile()

        Dim nbrErrors As Integer = 0

        Using MyReader As New FileIO.TextFieldParser($"{Application.StartupPath}\msfs_airports.csv")
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")

            Dim currentRow As String()
            currentRow = MyReader.ReadFields()
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    If currentRow IsNot Nothing Then
                        AirportsICAO.Add(currentRow(0), currentRow(1))
                    End If
                Catch ex As FileIO.MalformedLineException
                    nbrErrors += 1
                End Try
            End While
        End Using

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
        For i As Integer = 0 To pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint").Count - 1
            Dim atcWaypoint As New ATCWaypoint(pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint").Item(i).Attributes(0).Value, pXmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint/WorldPosition").Item(i).FirstChild.Value, i)
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

End Class
