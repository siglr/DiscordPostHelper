Imports System.Xml
Imports System.IO
Imports System.Xml.Serialization
Imports System.Net
Imports System.Text
Imports System.Web
Imports System.Threading
Imports System.Security.Cryptography
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Globalization

Public Class Main

    Private xmldocFlightPlan As XmlDocument
    Private xmldocWeatherPreset As XmlDocument
    Private AirportsICAO As New Dictionary(Of String, String)
    Private weatherDetails As WeatherDetails = Nothing
    Private DefaultKnownClubEvents As New Dictionary(Of String, PresetEvent)
    Private ClubPreset As PresetEvent = Nothing
    Private _currentDistanceUnit As Integer
    Private Const DiscordLimit As Integer = 2000
    Private Const LimitMsg As String = "Caution! Discord Characters Limit: "
    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"
    Private intGuideCurrentStep As Integer = 0
    Private EnglishCulture As CultureInfo = New CultureInfo("en-US")

    Private dblFlightTotalDistanceInKm As Double = 0
    Private dblTaskTotalDistanceInKm As Double = 0

#Region "Startup"

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ResetForm()

        LoadAirportsICAOFile()

        LoadDefaultClubEvents()

        SetTimePickerFormat()

        cboSpeedUnits.SelectedIndex = 0
        cboDifficulty.SelectedIndex = 0
        cboRecommendedGliders.SelectedIndex = 0

        txtFilesText.Text = "**Files**" & vbCrLf & "Required: Flight plan (.pln) and Weather preset (.wpr)" & vbCrLf & "Optional: XCSoar Track (.trk)"

        btnCopyAllSecPosts.Top = btnAltRestricCopy.Top

        If Screen.PrimaryScreen.Bounds.Height > Me.Height Then
        Else
            Me.Height = Screen.PrimaryScreen.Bounds.Height - 20
        End If

        Me.Text = Me.Text & " " & Me.GetType.Assembly.GetName.Version.ToString

        'Load previous session data
        LoadSessionData(Application.StartupPath & "\LastSession.dph")

    End Sub

    Private Sub SetTimePickerFormat()

        Dim dtfi As DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat
        Dim timeFormatToUse As String = String.Empty

        If dtfi.ShortTimePattern.Contains("H") Then
            ' Use 24-hour time format
            timeFormatToUse = "HH:mm"
        Else
            ' Use AM/PM time format
            timeFormatToUse = "hh:mm tt"
        End If

        dtSimLocalTime.CustomFormat = timeFormatToUse
        dtEventMeetTime.CustomFormat = timeFormatToUse
        dtEventSyncFlyTime.CustomFormat = timeFormatToUse
        dtEventLaunchTime.CustomFormat = timeFormatToUse
        dtEventStartTaskTime.CustomFormat = timeFormatToUse

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

        Using MyReader As New Microsoft.VisualBasic.
                      FileIO.TextFieldParser(Application.StartupPath & "\msfs_airports.csv")
            MyReader.TextFieldType = FileIO.FieldType.Delimited
            MyReader.SetDelimiters(",")

            Dim currentRow As String()
            currentRow = MyReader.ReadFields()
            While Not MyReader.EndOfData
                Try
                    currentRow = MyReader.ReadFields()
                    If Not currentRow Is Nothing Then
                        AirportsICAO.Add(currentRow(1), currentRow(3))
                    End If
                Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                    nbrErrors = nbrErrors + 1
                End Try
            End While
        End Using

    End Sub

    Private Sub ResetForm()

        xmldocFlightPlan = New XmlDocument
        xmldocWeatherPreset = New XmlDocument
        weatherDetails = Nothing
        dblFlightTotalDistanceInKm = 0
        dblTaskTotalDistanceInKm = 0

        cboSpeedUnits.SelectedIndex = 0
        cboDifficulty.SelectedIndex = 0
        cboRecommendedGliders.SelectedIndex = 0
        lstAllFiles.Items.Clear()

        txtFilesText.Text = "**Files**" & vbCrLf & "Required: Flight plan (.pln) and Weather preset (.wpr)" & vbCrLf & "Optional: XCSoar Track (.trk)"

        txtFlightPlanFile.Text = String.Empty
        txtWeatherFile.Text = String.Empty
        chkTitleLock.Checked = False
        txtTitle.Text = String.Empty
        chkIncludeYear.Checked = False
        txtSimDateTimeExtraInfo.Text = String.Empty
        txtMainArea.Text = String.Empty
        txtDepartureICAO.Text = String.Empty
        txtDepName.Text = String.Empty
        chkDepartureLock.Checked = False
        txtDepExtraInfo.Text = String.Empty
        txtArrivalICAO.Text = String.Empty
        txtArrivalName.Text = String.Empty
        chkArrivalLock.Checked = False
        txtArrivalExtraInfo.Text = String.Empty
        chkSoaringTypeRidge.Checked = False
        chkSoaringTypeThermal.Checked = False
        txtSoaringTypeExtraInfo.Text = String.Empty
        txtDistanceTotal.Text = String.Empty
        txtDistanceTrack.Text = String.Empty
        txtMaxAvgSpeed.Text = String.Empty
        txtMinAvgSpeed.Text = String.Empty
        txtDurationExtraInfo.Text = String.Empty
        txtDurationMin.Text = String.Empty
        txtDurationMax.Text = String.Empty
        txtDifficultyExtraInfo.Text = String.Empty
        chkDescriptionLock.Checked = False
        txtShortDescription.Text = String.Empty
        txtCredits.Text = "All credits to @UserName for this track."
        txtLongDescription.Text = String.Empty
        chkUseOnlyWeatherSummary.Checked = False
        txtWeatherSummary.Text = String.Empty
        txtAltRestrictions.Text = String.Empty
        txtWeatherFirstPart.Text = String.Empty
        txtWeatherWinds.Text = String.Empty
        txtWeatherClouds.Text = String.Empty
        txtFullDescriptionResults.Text = String.Empty
        cboGroupOrClubName.SelectedIndex = -1
        cboMSFSServer.SelectedIndex = -1
        cboVoiceChannel.SelectedIndex = -1
        chkDateTimeUTC.Checked = True
        chkUseSyncFly.Checked = False
        chkUseLaunch.Checked = False
        chkUseStart.Checked = False
        txtEventDescription.Text = String.Empty
        cboEligibleAward.SelectedIndex = -1
        txtTaskFlightPlanURL.Text = String.Empty
        txtGroupEventPostURL.Text = String.Empty
        chkIncludeGotGravelInvite.Enabled = False
        chkIncludeGotGravelInvite.Checked = False

        btnRemoveExtraFile.Enabled = False
        btnExtraFileDown.Enabled = False
        btnExtraFileUp.Enabled = False

        SetVisibilityForSecPosts()

        BuildFPResults()
        BuildGroupFlightPost()

    End Sub

#End Region

#Region "Main FP Infos and altitude restrictions"

    Private Sub BuildFPResults()

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        txtFPResults.Text = ""
        txtFPResults.AppendText("**" & txtTitle.Text & "**" & vbCrLf & vbCrLf)
        txtFPResults.AppendText(ValueToAppendIfNotEmpty(txtShortDescription.Text,,, 2))
        If Not txtMainArea.Text.Trim = String.Empty Then
            txtFPResults.AppendText("**Main area/POI:** " & ValueToAppendIfNotEmpty(txtMainArea.Text) & vbCrLf)
        End If
        txtFPResults.AppendText("**Flight plan file:** " & Chr(34) & Path.GetFileName(txtFlightPlanFile.Text) & Chr(34) & vbCrLf)
        txtFPResults.AppendText("**Departure:** " & ValueToAppendIfNotEmpty(txtDepartureICAO.Text) & ValueToAppendIfNotEmpty(txtDepName.Text, True) & ValueToAppendIfNotEmpty(txtDepExtraInfo.Text, True, True) & vbCrLf)
        txtFPResults.AppendText("**Arrival:** " & ValueToAppendIfNotEmpty(txtArrivalICAO.Text) & ValueToAppendIfNotEmpty(txtArrivalName.Text, True) & ValueToAppendIfNotEmpty(txtArrivalExtraInfo.Text, True, True) & vbCrLf)
        txtFPResults.AppendText("**Sim Date & Time:** " & dtSimDate.Value.ToString(dateFormat, EnglishCulture) & ", " & dtSimLocalTime.Value.ToString("hh:mm tt", EnglishCulture) & " local" & ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True) & vbCrLf)
        txtFPResults.AppendText("**Soaring Type:** " & GetSoaringTypesSelected() & ValueToAppendIfNotEmpty(txtSoaringTypeExtraInfo.Text, True, True) & vbCrLf)
        txtFPResults.AppendText("**Distance:** " & GetDistance() & vbCrLf)
        txtFPResults.AppendText("**Duration:** " & GetDuration(txtDurationMin, txtDurationMax) & ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True) & vbCrLf)
        txtFPResults.AppendText("**Recommended gliders:** " & ValueToAppendIfNotEmpty(cboRecommendedGliders.Text) & vbCrLf)
        txtFPResults.AppendText("**Difficulty:** " & GetDifficulty() & vbCrLf & vbCrLf)
        txtFPResults.AppendText(ValueToAppendIfNotEmpty(txtCredits.Text,,, 2))
        txtFPResults.AppendText("See inside thread for most up-to-date files And more information.")
        If txtLongDescription.Text.Trim.Length > 0 Then
            txtFullDescriptionResults.Text = "**Full Description**" & vbCrLf & txtLongDescription.Text.Trim
        End If

    End Sub

    Private Sub CalculateDuration()

        Dim minAvgspeedInKmh As Single
        Dim maxAvgspeedInKmh As Single
        Dim totalDistanceInKm As Single

        If Not Single.TryParse(txtMinAvgSpeed.Text, minAvgspeedInKmh) Then
            minAvgspeedInKmh = 0
        End If
        If Not Single.TryParse(txtMaxAvgSpeed.Text, maxAvgspeedInKmh) Then
            maxAvgspeedInKmh = 0
        End If

        Select Case cboSpeedUnits.SelectedIndex
            Case 0 ' KM/h
                'Already in the right units - do nothing

            Case 1 ' Miles/h
                minAvgspeedInKmh = Conversions.MilesToKm(minAvgspeedInKmh)
                maxAvgspeedInKmh = Conversions.MilesToKm(maxAvgspeedInKmh)

            Case 2 'Knots
                minAvgspeedInKmh = Conversions.KnotsToKmh(minAvgspeedInKmh)
                maxAvgspeedInKmh = Conversions.KnotsToKmh(maxAvgspeedInKmh)

        End Select

        'Use distance in km
        If Not Single.TryParse(txtDistanceTotal.Text, totalDistanceInKm) Then
            totalDistanceInKm = 0
        End If

        If totalDistanceInKm > 0 Then
            If minAvgspeedInKmh > 0 Then
                txtDurationMax.Text = FormatNumber(RoundTo15Minutes((totalDistanceInKm / minAvgspeedInKmh) * 60), 0)
            End If
            If maxAvgspeedInKmh > 0 Then
                txtDurationMin.Text = FormatNumber(RoundTo15Minutes((totalDistanceInKm / maxAvgspeedInKmh) * 60), 0)
            End If
        End If

        BuildFPResults()

    End Sub

    Private Function RoundTo15Minutes(ByVal minutes As Integer) As Integer
        Return CInt(Math.Ceiling(minutes / 15.0) * 15)
    End Function

    Private Sub ChangeDetection(sender As Object, e As EventArgs) Handles txtTitle.Leave, txtSoaringTypeExtraInfo.Leave, txtSimDateTimeExtraInfo.Leave, txtShortDescription.Leave, txtMainArea.Leave, txtDurationMin.Leave, txtDurationMax.Leave, txtDurationExtraInfo.Leave, txtDifficultyExtraInfo.Leave, txtDepName.Leave, txtDepExtraInfo.Leave, txtDepartureICAO.Leave, txtCredits.Leave, txtArrivalName.Leave, txtArrivalICAO.Leave, txtArrivalExtraInfo.Leave, dtSimLocalTime.ValueChanged, dtSimLocalTime.Leave, dtSimDate.ValueChanged, dtSimDate.Leave, chkSoaringTypeThermal.CheckedChanged, chkSoaringTypeRidge.CheckedChanged, chkIncludeYear.CheckedChanged, cboRecommendedGliders.SelectedIndexChanged, cboRecommendedGliders.Leave, cboDifficulty.SelectedIndexChanged

        If sender Is txtDepartureICAO Or sender Is txtArrivalICAO Then
            AirportICAOChanged(sender)
        End If

        BuildFPResults()

        If sender Is dtSimDate Or sender Is dtSimLocalTime Or sender Is chkIncludeYear Then
            CopyToEventFields(sender, e)
        End If

        'For text box, make sure to display the value from the start
        If sender.GetType().ToString = "System.Windows.Forms.TextBox" Then
            LeavingTextBox(DirectCast(sender, Windows.Forms.TextBox))
        End If

    End Sub

    Private Sub LeavingTextBox(txtbox As Windows.Forms.TextBox)
        txtbox.SelectionLength = 0
        txtbox.SelectionStart = 0
    End Sub
    Private Sub EnteringTextBox(txtbox As Windows.Forms.TextBox)

        txtbox.SelectAll()
        txtbox.SelectionStart = 0

    End Sub

    Private Function GetSoaringTypesSelected() As String
        Dim types As String = String.Empty

        If chkSoaringTypeRidge.Checked And chkSoaringTypeThermal.Checked Then
            types = "Ridge and Thermals"
        ElseIf chkSoaringTypeRidge.Checked Then
            types = "Ridge"
        ElseIf chkSoaringTypeThermal.Checked Then
            types = "Thermals"
        End If

        Return types

    End Function

    Private Function GetDistance() As String
        Dim distance As String = String.Empty
        Dim totalDistKm As Decimal
        Dim trackDistKm As Decimal
        Dim totalDistMiles As Decimal
        Dim trackDistMiles As Decimal

        Decimal.TryParse(txtDistanceTotal.Text, totalDistKm)
        Decimal.TryParse(txtDistanceTrack.Text, trackDistKm)
        totalDistMiles = Conversions.KmToMiles(totalDistKm)
        trackDistMiles = Conversions.KmToMiles(trackDistKm)

        distance = FormatNumber(totalDistKm, 0) & " km total (" & FormatNumber(trackDistKm, 0) & " km task) / " & FormatNumber(totalDistMiles, 0) & " mi total (" & FormatNumber(trackDistMiles, 0) & " mi task)"

        Return distance

    End Function

    Private Sub DistanceNumberValidation(ByVal sender As Windows.Forms.TextBox, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDistanceTotal.KeyPress, txtDistanceTrack.KeyPress
        Dim keyChar = e.KeyChar

        If Char.IsControl(keyChar) Then
            'Allow all control characters.
        ElseIf Char.IsDigit(keyChar) OrElse keyChar = "."c Then
            Dim text = sender.Text
            Dim selectionStart = sender.SelectionStart
            Dim selectionLength = sender.SelectionLength

            text = text.Substring(0, selectionStart) & keyChar & text.Substring(selectionStart + selectionLength)

            If Not (Integer.TryParse(text, New Integer) Or Double.TryParse(text, New Double)) Then
                e.Handled = True
            End If
        Else
            'Reject all other characters.
            e.Handled = True
        End If
    End Sub

    Private Sub DurationNumberValidation(ByVal sender As Windows.Forms.TextBox, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDurationMin.KeyPress, txtDurationMax.KeyPress
        Dim keyChar = e.KeyChar

        If Char.IsControl(keyChar) Then
            'Allow all control characters.
        ElseIf Char.IsDigit(keyChar) OrElse keyChar = "."c Then
            Dim text = sender.Text
            Dim selectionStart = sender.SelectionStart
            Dim selectionLength = sender.SelectionLength

            text = text.Substring(0, selectionStart) & keyChar & text.Substring(selectionStart + selectionLength)

            If Not (Integer.TryParse(text, New Integer)) Then
                e.Handled = True
            End If
        Else
            'Reject all other characters.
            e.Handled = True
        End If
    End Sub

    Private Function GetDuration(durationMinControl As Windows.Forms.TextBox, durationMaxControl As Windows.Forms.TextBox) As String
        Dim duration As String = String.Empty
        Dim minHours As Decimal = 0
        Dim maxHours As Decimal = 0
        Dim minHoursH As String = String.Empty
        Dim maxHoursH As String = String.Empty
        Dim minHoursM As String = String.Empty
        Dim maxHoursM As String = String.Empty
        Dim minMinutes As Integer = 0
        Dim maxMinutes As Integer = 0

        Integer.TryParse(durationMinControl.Text, minMinutes)
        Integer.TryParse(durationMaxControl.Text, maxMinutes)

        minHours = minMinutes / 60
        maxHours = maxMinutes / 60

        If Math.Floor(minHours) = minHours Then
            minHoursH = FormatNumber(minHours, 0) & "h"
        Else
            minHoursH = Math.Floor(minHours).ToString & "h"
            minHoursM = (Math.Abs((Math.Floor(minHours) * 60) - minMinutes)).ToString("00")
        End If
        If Math.Floor(maxHours) = maxHours Then
            maxHoursH = FormatNumber(maxHours, 0) & "h"
        Else
            maxHoursH = Math.Floor(maxHours).ToString & "h"
            maxHoursM = (Math.Abs((Math.Floor(maxHours) * 60) - maxMinutes)).ToString("00")
        End If

        duration = minMinutes.ToString & " to " & maxMinutes.ToString & " minutes (" & minHoursH & minHoursM & " to " & maxHoursH & maxHoursM & ")"

        Return duration

    End Function

    Private Function GetDifficulty() As String
        Dim difficulty As String = String.Empty

        Select Case cboDifficulty.SelectedIndex
            Case 0
                If String.IsNullOrEmpty(txtDifficultyExtraInfo.Text) Then
                    difficulty = "Unknown - Judge by yourself!"
                Else
                    difficulty = txtDifficultyExtraInfo.Text
                End If
            Case 1
                difficulty = "★☆☆☆☆ - Beginner" & ValueToAppendIfNotEmpty(txtDifficultyExtraInfo.Text, True, True)
            Case 2
                difficulty = "★★☆☆☆ - Student" & ValueToAppendIfNotEmpty(txtDifficultyExtraInfo.Text, True, True)
            Case 3
                difficulty = "★★★☆☆ - Experimented" & ValueToAppendIfNotEmpty(txtDifficultyExtraInfo.Text, True, True)
            Case 4
                difficulty = "★★★★☆ - Professional" & ValueToAppendIfNotEmpty(txtDifficultyExtraInfo.Text, True, True)
            Case 5
                difficulty = "★★★★★ - Champion" & ValueToAppendIfNotEmpty(txtDifficultyExtraInfo.Text, True, True)
        End Select
        Return difficulty

    End Function

    Private Sub btnSelectFlightPlan_Click(sender As Object, e As EventArgs) Handles btnSelectFlightPlan.Click

        If txtWeatherFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtWeatherFile.Text)
        End If

        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select flight plan file"
        OpenFileDialog1.Filter = "Flight plan|*.pln"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            LoadFlightPlan(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub LoadFlightPlan(filename As String)

        Dim atcWaypoint As ATCWaypoint = Nothing
        Dim previousATCWaypoing As ATCWaypoint = Nothing

        'read file
        txtFlightPlanFile.Text = filename
        xmldocFlightPlan.Load(filename)

        If Not chkTitleLock.Checked Then
            txtTitle.Text = xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Title").Item(0).FirstChild.Value
        End If

        If xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Count > 0 AndAlso (Not chkDepartureLock.Checked) Then
            txtDepExtraInfo.Text = "Rwy " & xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Item(0).FirstChild.Value
        End If

        If Not chkDescriptionLock.Checked Then
            txtShortDescription.Text = xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Descr").Item(0).FirstChild.Value
        End If

        txtDepartureICAO.Text = xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DepartureID").Item(0).FirstChild.Value
        AirportICAOChanged(txtDepartureICAO)
        txtArrivalICAO.Text = xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DestinationID").Item(0).FirstChild.Value
        AirportICAOChanged(txtArrivalICAO)

        'Build altitude restrictions
        Dim strRestrictions As String = String.Empty
        Dim blnInTask As Boolean = False
        Dim dblDistanceToPrevious As Double = 0
        txtAltRestrictions.Text = String.Empty
        dblFlightTotalDistanceInKm = 0
        dblTaskTotalDistanceInKm = 0
        For i As Integer = 0 To xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint").Count - 1
            atcWaypoint = New ATCWaypoint(xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint").Item(i).Attributes(0).Value, xmldocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/ATCWaypoint/WorldPosition").Item(i).FirstChild.Value, i)
            If atcWaypoint.ContainsRestriction Then
                strRestrictions = strRestrictions & vbCrLf & atcWaypoint.Restrictions
            End If
            If i > 0 Then
                'Start adding distance between this waypoint and previous one to the total distance
                dblDistanceToPrevious = Conversions.GetDistanceInKm(previousATCWaypoing.Latitude, previousATCWaypoing.Longitude, atcWaypoint.Latitude, atcWaypoint.Longitude)
                dblFlightTotalDistanceInKm = dblFlightTotalDistanceInKm + dblDistanceToPrevious
            End If
            If blnInTask Then
                'Start adding distance between this waypoint and previous one to the track distance
                dblTaskTotalDistanceInKm = dblTaskTotalDistanceInKm + dblDistanceToPrevious
            End If
            If atcWaypoint.IsTaskStart Then
                blnInTask = True
            End If
            If atcWaypoint.IsTaskEnd Then
                blnInTask = False
            End If
            previousATCWaypoing = atcWaypoint
        Next
        If strRestrictions = String.Empty Then
            txtAltRestrictions.Text = "**Altitude Restrictions**" & vbCrLf & "None"
        Else
            txtAltRestrictions.Text = "**Altitude Restrictions**" & strRestrictions
        End If

        txtDistanceTotal.Text = FormatNumber(dblFlightTotalDistanceInKm, 0)
        txtDistanceTrack.Text = FormatNumber(dblTaskTotalDistanceInKm, 0)

        BuildFPResults()
        BuildGroupFlightPost()

    End Sub

    Private Sub AirportICAOChanged(sender As Windows.Forms.Control)

        Select Case sender.Name
            Case "txtDepartureICAO"
                If AirportsICAO.ContainsKey(txtDepartureICAO.Text) And Not chkDepartureLock.Checked Then
                    txtDepName.Text = AirportsICAO(txtDepartureICAO.Text)
                End If
            Case "txtArrivalICAO"
                If AirportsICAO.ContainsKey(txtArrivalICAO.Text) And Not chkArrivalLock.Checked Then
                    txtArrivalName.Text = AirportsICAO(txtArrivalICAO.Text)
                End If
        End Select

        BuildFPResults()

    End Sub

    Private Sub txtFPResults_TextChanged(sender As Object, e As EventArgs) Handles txtFPResults.TextChanged
        lblNbrCarsMainFP.Text = txtFPResults.Text.Length
    End Sub

    Private Sub txtAltRestrictions_TextChanged(sender As Object, e As EventArgs) Handles txtAltRestrictions.TextChanged
        lblNbrCarsRestrictions.Text = txtAltRestrictions.Text.Length
    End Sub

    Private Sub txtMaxAvgSpeed_Leave(sender As Object, e As EventArgs) Handles txtMinAvgSpeed.Leave, txtMaxAvgSpeed.Leave
        CalculateDuration()
    End Sub

    Private Sub cboSpeedUnits_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSpeedUnits.SelectedIndexChanged
        CalculateDuration()
    End Sub

#End Region

#Region "Various functions"

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        ResetForm()
    End Sub

    Private Function ValueToAppendIfNotEmpty(textValue As String, Optional addSpace As Boolean = False, Optional useBrackets As Boolean = False, Optional nbrLineFeed As Integer = 0) As String
        Dim textToReturn As String = String.Empty

        If Not String.IsNullOrEmpty(textValue) Then
            If addSpace Then
                textToReturn = " "
            End If
            If useBrackets Then
                textToReturn = textToReturn & "(" & textValue & ")"
            Else
                textToReturn = textToReturn & textValue
            End If
            For i As Integer = 1 To nbrLineFeed
                textToReturn = textToReturn & vbCrLf
            Next
        End If

        Return textToReturn

    End Function

#End Region

#Region "Clipboard buttons"
    Private Sub btnFPMainInfoCopy_Click(sender As Object, e As EventArgs) Handles btnFPMainInfoCopy.Click
        Clipboard.SetText(txtFPResults.Text)
        MsgBox("You can now post the main flight plan message directly in the tasks/plans channel, then create a thread (make sure the name is the same as the title) where we will put the other informations.", vbOKOnly Or MsgBoxStyle.Information, "Step 1 - Creating main FP post")
        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If
    End Sub

    Private Sub btnAltRestricCopy_Click(sender As Object, e As EventArgs) Handles btnAltRestricCopy.Click
        Clipboard.SetText(txtAltRestrictions.Text & vbCrLf & vbCrLf & txtWeatherFirstPart.Text & vbCrLf & vbCrLf & txtWeatherWinds.Text & vbCrLf & vbCrLf & txtWeatherClouds.Text & vbCrLf & ".")
        MsgBox("Now paste the content as the second message in the thread!", vbOKOnly Or MsgBoxStyle.Information, "Step 2 - Creating secondary post for weather in the thread.")
        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If

    End Sub

    Private Sub btnFilesCopy_Click(sender As Object, e As EventArgs) Handles btnFilesCopy.Click
        Dim allFiles As New Specialized.StringCollection

        If File.Exists(txtFlightPlanFile.Text) Then
            allFiles.Add(txtFlightPlanFile.Text)
        End If
        If File.Exists(txtWeatherFile.Text) Then
            allFiles.Add(txtWeatherFile.Text)
        End If

        For i = 0 To lstAllFiles.Items.Count() - 1
            If File.Exists(lstAllFiles.Items(i)) Then
                allFiles.Add(lstAllFiles.Items(i))
            End If
        Next

        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            If chkGroupSecondaryPosts.Checked Then
                MsgBox("Now paste the copied files as the final message.", vbOKOnly Or MsgBoxStyle.Exclamation, "Step 3 - Inserting the files in the thread.")
            Else
                MsgBox("Now paste the copied files as the third message without posting it and come back for the text info (button 3b).", vbOKOnly Or MsgBoxStyle.Exclamation, "Step 3a - Creating the files post in the thread - actual files first")
            End If
            If intGuideCurrentStep <> 0 Then
                intGuideCurrentStep = intGuideCurrentStep + 1
                ShowGuide()
            End If
        Else
            MsgBox("No files to copy!", vbOKOnly Or MsgBoxStyle.Critical, "Step 3a - Creating the files post in the thread - actual files first")
        End If

    End Sub

    Private Sub btnFilesTextCopy_Click(sender As Object, e As EventArgs) Handles btnFilesTextCopy.Click
        Clipboard.SetText(txtFilesText.Text)
        MsgBox("Now enter the info (legend) in the third message and post it. Also pin this message in the thread.", vbOKOnly Or MsgBoxStyle.Information, "Step 3b - Creating the files post in the thread - file info")
        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If
    End Sub

    Private Sub btnFullDescriptionCopy_Click(sender As Object, e As EventArgs) Handles btnFullDescriptionCopy.Click

        If txtFullDescriptionResults.Text.Length = 0 Then
            MsgBox("The last message (Full Description) is empty. Cannot proceed!", vbOKOnly Or MsgBoxStyle.Critical, "Step 4 - Creating full description post in the thread.")
        Else
            Clipboard.SetText(txtFullDescriptionResults.Text)
            MsgBox("Now post the last message in the thread to complete your flight plan entry.", vbOKOnly Or MsgBoxStyle.Information, "Step 4 - Creating full description post in the thread.")
            If intGuideCurrentStep <> 0 Then
                intGuideCurrentStep = intGuideCurrentStep + 1
                ShowGuide()
            End If
        End If

    End Sub

#End Region

#Region "Discord Characters Limit Check"

    Private Sub NbrCarsCheckDiscordLimit(sender As Object, e As EventArgs) Handles lblRestrictWeatherTotalCars.TextChanged, lblNbrCarsMainFP.TextChanged, lblNbrCarsFullDescResults.TextChanged

        Dim lblLabel As Windows.Forms.Label = DirectCast(sender, Windows.Forms.Label)

        Select Case CInt(lblLabel.Text)
            Case > DiscordLimit
                lblLabel.Visible = True
                lblLabel.Font = New Font(lblLabel.Font, lblLabel.Font.Style Or FontStyle.Bold)
            Case > DiscordLimit - 200
                lblLabel.Visible = True
                lblLabel.Font = New Font(lblLabel.Font, lblLabel.Font.Style And Not FontStyle.Bold)
            Case Else
                lblLabel.Visible = False
                lblLabel.Font = New Font(lblLabel.Font, lblLabel.Font.Style And Not FontStyle.Bold)
        End Select

    End Sub

    Private Sub SetDiscordLimitMessage(sender As Object, e As EventArgs) Handles lblRestrictWeatherTotalCars.VisibleChanged, lblNbrCarsMainFP.VisibleChanged, lblNbrCarsFullDescResults.VisibleChanged

        Dim lblLabel As Windows.Forms.Label = DirectCast(sender, Windows.Forms.Label)

        If lblLabel.Visible = True Then
            ToolTip1.SetToolTip(lblLabel, LimitMsg & DiscordLimit.ToString)
        End If

    End Sub

#End Region

#Region "Weather sections"

    Private Sub btnSelectWeatherFile_Click(sender As Object, e As EventArgs) Handles btnSelectWeatherFile.Click
        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select weather file"
        OpenFileDialog1.Filter = "Weather preset file|*.wpr"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            LoadWeatherfile(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub LoadWeatherfile(filename As String)
        'read file
        txtWeatherFile.Text = filename
        xmldocWeatherPreset.Load(filename)

        weatherDetails = Nothing
        weatherDetails = New WeatherDetails(xmldocWeatherPreset)

        BuildWeatherInfoResults()
        BuildGroupFlightPost()

        If Not (chkUseOnlyWeatherSummary.Checked Or weatherDetails Is Nothing) Then
            BuildWeatherCloudLayers()
            BuildWeatherWindLayers()
        End If

    End Sub

    Private Sub BuildWeatherInfoResults()

        txtWeatherFirstPart.Text = "**Weather Basic Information**" & vbCrLf

        If chkUseOnlyWeatherSummary.Checked Or weatherDetails Is Nothing Then
            txtWeatherFirstPart.AppendText("Summary: " & ValueToAppendIfNotEmpty(txtWeatherSummary.Text,,, 1))
        Else
            txtWeatherFirstPart.AppendText("Weather file & profile name: " & Chr(34) & Path.GetFileName(txtWeatherFile.Text) & Chr(34) & " (" & weatherDetails.PresetName & ")" & vbCrLf)
            'txtWeatherFirstPart.AppendText("Preset name: " & ValueToAppendIfNotEmpty(weatherDetails.PresetName,,, 1))
            If txtWeatherSummary.Text.Trim = String.Empty Then
            Else
                txtWeatherFirstPart.AppendText("Summary: " & ValueToAppendIfNotEmpty(txtWeatherSummary.Text) & vbCrLf)
            End If
            txtWeatherFirstPart.AppendText("Elevation measurement: " & weatherDetails.AltitudeMeasurement & vbCrLf)
            txtWeatherFirstPart.AppendText("MSLPressure: " & weatherDetails.MSLPressure & vbCrLf)
            txtWeatherFirstPart.AppendText("MSLTemperature: " & weatherDetails.MSLTemperature & vbCrLf)
            txtWeatherFirstPart.AppendText("Humidity: " & weatherDetails.Humidity)
            If weatherDetails.HasPrecipitations Then
                txtWeatherFirstPart.AppendText(vbCrLf & "Precipitations: " & weatherDetails.Precipitations)
            End If
            If weatherDetails.HasSnowCover Then
                txtWeatherFirstPart.AppendText(vbCrLf & "Snow Cover: " & weatherDetails.SnowCover)
            End If
        End If

    End Sub

    Private Sub BuildWeatherCloudLayers()
        txtWeatherClouds.Text = "**Cloud Layers**"
        txtWeatherClouds.AppendText(weatherDetails.CloudLayers)
    End Sub

    Private Sub BuildWeatherWindLayers()
        txtWeatherWinds.Text = "**Wind Layers**"
        txtWeatherWinds.AppendText(weatherDetails.WindLayers)
    End Sub

    Private Sub WeatherChangeDetection(sender As Object, e As EventArgs) Handles txtWeatherSummary.Leave, chkUseOnlyWeatherSummary.CheckedChanged
        BuildWeatherInfoResults()
        If Not (chkUseOnlyWeatherSummary.Checked Or weatherDetails Is Nothing) Then
            BuildWeatherCloudLayers()
            BuildWeatherWindLayers()
        Else
            txtWeatherClouds.Text = String.Empty
            txtWeatherWinds.Text = String.Empty
        End If
        If TypeOf (sender) Is TextBox Then
            LeavingTextBox(DirectCast(sender, System.Windows.Forms.TextBox))
        End If
    End Sub

    Private Sub txtWeatherFirstPart_TextChanged(sender As Object, e As EventArgs) Handles txtWeatherFirstPart.TextChanged
        lblNbrCarsWeatherInfo.Text = txtWeatherFirstPart.Text.Length
    End Sub

    Private Sub txtWeatherClouds_TextChanged(sender As Object, e As EventArgs) Handles txtWeatherClouds.TextChanged
        lblNbrCarsWeatherClouds.Text = txtWeatherClouds.Text.Length
    End Sub

    Private Sub txtWeatherWinds_TextChanged(sender As Object, e As EventArgs) Handles txtWeatherWinds.TextChanged
        lblNbrCarsWeatherWinds.Text = txtWeatherWinds.Text.Length

    End Sub

    Private Sub txtFilesText_TextChanged(sender As Object, e As EventArgs) Handles txtFilesText.TextChanged
        lblNbrCarsFilesText.Text = txtFilesText.Text.Length
    End Sub

    Private Sub txtLongDescription_Leave(sender As Object, e As EventArgs) Handles txtLongDescription.Leave
        BuildFPResults()
        LeavingTextBox(DirectCast(sender, System.Windows.Forms.TextBox))
    End Sub

    Private Sub txtFullDescriptionResults_TextChanged(sender As Object, e As EventArgs) Handles txtFullDescriptionResults.TextChanged
        lblNbrCarsFullDescResults.Text = txtLongDescription.Text.Length

    End Sub

    Private Sub lblNbrCarsWeatherInfo_TextChanged(sender As Object, e As EventArgs) Handles lblNbrCarsWeatherWinds.TextChanged, lblNbrCarsWeatherInfo.TextChanged, lblNbrCarsWeatherClouds.TextChanged, lblNbrCarsRestrictions.TextChanged

        Dim lbl1, lbl2, lbl3, lbl4 As Integer

        If lblNbrCarsWeatherWinds.Text = String.Empty Then
            lbl1 = 0
        Else
            lbl1 = CInt(lblNbrCarsWeatherWinds.Text)
        End If

        If lblNbrCarsWeatherInfo.Text = String.Empty Then
            lbl2 = 0
        Else
            lbl2 = CInt(lblNbrCarsWeatherInfo.Text)
        End If

        If lblNbrCarsWeatherClouds.Text = String.Empty Then
            lbl3 = 0
        Else
            lbl3 = CInt(lblNbrCarsWeatherClouds.Text)
        End If

        If lblNbrCarsRestrictions.Text = String.Empty Then
            lbl4 = 0
        Else
            lbl4 = CInt(lblNbrCarsRestrictions.Text)
        End If

        lblRestrictWeatherTotalCars.Text = lbl1 + lbl2 + lbl3 + lbl4
    End Sub

#End Region

#Region "Extra files"
    Private Sub btnAddExtraFile_Click(sender As Object, e As EventArgs) Handles btnAddExtraFile.Click
        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select extra file"
        OpenFileDialog1.Filter = "All files|*.*"
        OpenFileDialog1.Multiselect = True

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            For i As Integer = 0 To OpenFileDialog1.FileNames.Count - 1
                'Check if one of the files is selected flight plan or weather file to exclude them
                If OpenFileDialog1.FileNames(i) <> txtFlightPlanFile.Text And OpenFileDialog1.FileNames(i) <> txtWeatherFile.Text Then
                    If lstAllFiles.Items.Count = 8 Then
                        MsgBox("Discord does not allow more than 10 files!", vbOK Or MsgBoxStyle.Critical)
                        Exit For
                    End If
                    lstAllFiles.Items.Add(OpenFileDialog1.FileNames(i))
                End If
            Next
        End If

    End Sub

    Private Sub btnRemoveExtraFile_Click(sender As Object, e As EventArgs) Handles btnRemoveExtraFile.Click

        For i As Integer = lstAllFiles.SelectedIndices.Count - 1 To 0 Step -1
            lstAllFiles.Items.RemoveAt(lstAllFiles.SelectedIndices(i))
        Next
    End Sub

#End Region

#Region "Group Flights Events Tab"

    Private Sub CopyToEventFields(sender As Object, e As EventArgs) Handles txtTitle.TextChanged, txtShortDescription.TextChanged, txtDurationMin.TextChanged, txtDurationMax.TextChanged, txtDurationExtraInfo.TextChanged, txtCredits.TextChanged, txtSimDateTimeExtraInfo.TextChanged

        If sender Is txtTitle Then
            txtEventTitle.Text = txtTitle.Text
        End If

        If sender Is txtShortDescription Then
            txtEventDescription.Text = txtShortDescription.Text
        End If

        BuildGroupFlightPost()

    End Sub

    Private Sub ClubSelected(sender As Object, e As EventArgs) Handles cboGroupOrClubName.SelectedIndexChanged, cboGroupOrClubName.Leave

        Dim clubExists As Boolean = DefaultKnownClubEvents.ContainsKey(cboGroupOrClubName.Text.ToUpper)

        If clubExists Then
            ClubPreset = DefaultKnownClubEvents(cboGroupOrClubName.Text.ToUpper)
            cboGroupOrClubName.Text = ClubPreset.ClubName
            cboMSFSServer.Text = ClubPreset.MSFSServer
            cboVoiceChannel.Text = ClubPreset.VoiceChannel
            CheckAndSetEventAward()
            chkDateTimeUTC.Checked = True
            dtEventMeetDate.Value = FindNextDate(Now, ClubPreset.EventDayOfWeek, ClubPreset.ZuluTime)
            dtEventMeetTime.Value = ClubPreset.ZuluTime
            Application.DoEvents()
            dtEventSyncFlyTime.Value = ClubPreset.ZuluTime.AddMinutes(ClubPreset.SyncFlyDelay)
            Application.DoEvents()
            dtEventLaunchTime.Value = dtEventSyncFlyTime.Value.AddMinutes(ClubPreset.LaunchDelay)
            Application.DoEvents()
            dtEventStartTaskTime.Value = dtEventLaunchTime.Value.AddMinutes(ClubPreset.StartTaskDelay)
        Else
            ClubPreset = Nothing
        End If

        BuildGroupFlightPost()

    End Sub

    Private Sub CheckAndSetEventAward()

        Dim trackDistanceKM As Integer = 0

        If (Not ClubPreset Is Nothing) AndAlso ClubPreset.EligibleAward AndAlso txtDistanceTrack.Text <> String.Empty Then

            trackDistanceKM = CInt(txtDistanceTrack.Text)

            lblEventTaskDistance.Text = trackDistanceKM.ToString() & " Km"
            lblEventTaskDistance.Visible = True

            Select Case trackDistanceKM
                Case >= 500
                    cboEligibleAward.Text = "Diamond"
                    Exit Select
                Case >= 400
                    cboEligibleAward.Text = "Gold"
                    Exit Select
                Case >= 300
                    cboEligibleAward.Text = "Silver"
                    Exit Select
                Case >= 200
                    cboEligibleAward.Text = "Bronze"
                    Exit Select
                Case Else
                    cboEligibleAward.Text = "None"

            End Select
        Else
            cboEligibleAward.SelectedIndex = 0
            lblEventTaskDistance.Visible = False
        End If
    End Sub

    Private Sub txtDistanceTrack_TextChanged(sender As Object, e As EventArgs)
        CheckAndSetEventAward()
    End Sub

    Private Sub EventDateChanged(sender As Object, e As EventArgs) Handles dtEventSyncFlyDate.ValueChanged, dtEventStartTaskDate.ValueChanged, dtEventMeetDate.ValueChanged, dtEventLaunchDate.ValueChanged

        Dim changedField As DateTimePicker = DirectCast(sender, DateTimePicker)

        Select Case changedField.Name
            Case dtEventMeetDate.Name
                dtEventSyncFlyDate.Value = dtEventMeetDate.Value

            Case dtEventSyncFlyDate.Name
                dtEventLaunchDate.Value = dtEventSyncFlyDate.Value

            Case dtEventLaunchDate.Name
                dtEventStartTaskDate.Value = dtEventLaunchDate.Value

        End Select

        BuildEventDatesTimes()

    End Sub

    Private Sub EventTimeChanged(sender As Object, e As EventArgs) Handles dtEventSyncFlyTime.ValueChanged, dtEventStartTaskTime.ValueChanged, dtEventMeetTime.ValueChanged, dtEventLaunchTime.ValueChanged

        Dim changedField As DateTimePicker = DirectCast(sender, DateTimePicker)

        Select Case changedField.Name
            Case dtEventMeetTime.Name
                If ClubPreset IsNot Nothing Then
                    dtEventSyncFlyTime.Value = dtEventMeetTime.Value.AddMinutes(ClubPreset.SyncFlyDelay)
                Else
                    dtEventSyncFlyTime.Value = dtEventMeetTime.Value
                End If

            Case dtEventSyncFlyTime.Name
                If ClubPreset IsNot Nothing Then
                    dtEventLaunchTime.Value = dtEventSyncFlyTime.Value.AddMinutes(ClubPreset.LaunchDelay)
                Else
                    dtEventLaunchTime.Value = dtEventSyncFlyTime.Value
                End If


            Case dtEventLaunchTime.Name
                If ClubPreset IsNot Nothing Then
                    dtEventStartTaskTime.Value = dtEventLaunchTime.Value.AddMinutes(ClubPreset.StartTaskDelay)
                Else
                    dtEventStartTaskTime.Value = dtEventLaunchTime.Value
                End If

        End Select

        BuildEventDatesTimes()

    End Sub

    Private Sub BuildEventDatesTimes()

        Dim eventDay As DayOfWeek

        lblMeetTimeResult.Text = FormatEventDateTime(New Date(dtEventMeetDate.Value.Year, dtEventMeetDate.Value.Month, dtEventMeetDate.Value.Day, dtEventMeetTime.Value.Hour, dtEventMeetTime.Value.Minute, 0), eventDay)
        ToolTip1.SetToolTip(lblMeetTimeResult, eventDay.ToString)

        lblSyncTimeResult.Text = FormatEventDateTime(New Date(dtEventSyncFlyDate.Value.Year, dtEventSyncFlyDate.Value.Month, dtEventSyncFlyDate.Value.Day, dtEventSyncFlyTime.Value.Hour, dtEventSyncFlyTime.Value.Minute, 0), eventDay)
        ToolTip1.SetToolTip(lblSyncTimeResult, eventDay.ToString)

        lblLaunchTimeResult.Text = FormatEventDateTime(New Date(dtEventLaunchDate.Value.Year, dtEventLaunchDate.Value.Month, dtEventLaunchDate.Value.Day, dtEventLaunchTime.Value.Hour, dtEventLaunchTime.Value.Minute, 0), eventDay)
        ToolTip1.SetToolTip(lblLaunchTimeResult, eventDay.ToString)

        lblStartTimeResult.Text = FormatEventDateTime(New Date(dtEventStartTaskDate.Value.Year, dtEventStartTaskDate.Value.Month, dtEventStartTaskDate.Value.Day, dtEventStartTaskTime.Value.Hour, dtEventStartTaskTime.Value.Minute, 0), eventDay)
        ToolTip1.SetToolTip(lblStartTimeResult, eventDay.ToString)

        BuildGroupFlightPost()

    End Sub

    Private Function FormatEventDateTime(dateTimeToUse As DateTime, ByRef eventDay As DayOfWeek) As String

        Dim dateTimeInZulu As DateTime
        Dim dateTimeInLocal As DateTime

        Dim result As String

        If chkDateTimeUTC.Checked Then
            dateTimeInZulu = dateTimeToUse
            dateTimeInLocal = Conversions.ConvertUTCToLocal(dateTimeInZulu)
            result = dateTimeInLocal.ToLongDateString & " " & dateTimeInLocal.ToString("hh:mm tt") & " Local"
            eventDay = dateTimeInLocal.DayOfWeek
        Else
            dateTimeInLocal = dateTimeToUse
            dateTimeInZulu = Conversions.ConvertLocalToUTC(dateTimeInLocal)
            result = dateTimeInZulu.ToLongDateString & " " & dateTimeInZulu.ToString("hh:mm tt") & " UTC"
            eventDay = dateTimeInZulu.DayOfWeek
        End If

        Return result

    End Function

    Private Function FindNextDate(startDate As DateTime, dayOfWeek As DayOfWeek, clubDefaultMeetTime As DateTime) As DateTime
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

    Private Sub chkDateTimeUTC_CheckedChanged(sender As Object, e As EventArgs) Handles chkDateTimeUTC.CheckedChanged
        BuildEventDatesTimes()
    End Sub

    Private Function GetFullEventDateTimeInLocal(dateControl As DateTimePicker, timeControl As DateTimePicker) As DateTime

        Dim dateFromDateControl As DateTime = dateControl.Value
        Dim timeFromTimeControl As DateTime = timeControl.Value

        Dim returnDateTime As DateTime = New Date(dateFromDateControl.Year, dateFromDateControl.Month, dateFromDateControl.Day, timeFromTimeControl.Hour, timeFromTimeControl.Minute, 0)

        If chkDateTimeUTC.Checked Then
            'Need conversion to local
            returnDateTime = Conversions.ConvertUTCToLocal(returnDateTime)
        End If

        Return returnDateTime

    End Function

    Private Sub BuildGroupFlightPost()

        Dim fullMeetDateTimeLocal As DateTime = GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime)
        Dim fullSyncFlyDateTimeLocal As DateTime = GetFullEventDateTimeInLocal(dtEventSyncFlyDate, dtEventSyncFlyTime)
        Dim fullLaunchDateTimeLocal As DateTime = GetFullEventDateTimeInLocal(dtEventLaunchDate, dtEventLaunchTime)
        Dim fullStartTaskDateTimeLocal As DateTime = GetFullEventDateTimeInLocal(dtEventStartTaskDate, dtEventStartTaskTime)

        'Test feature/test
        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        lblDiscordPostDateTime.Text = fullMeetDateTimeLocal.ToString("dddd, MMMM dd", EnglishCulture) & ", " &
                                      fullMeetDateTimeLocal.ToString("hh:mm tt", EnglishCulture)
        lblDiscordEventVoice.Text = cboVoiceChannel.Text

        txtGroupFlightEventPost.Text = String.Empty

        txtGroupFlightEventPost.AppendText("**" & Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("dddd, MMMM dd", EnglishCulture) & ", " &
                                                  Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("hhmm tt", EnglishCulture) & " Zulu / " &
                                                  GetDiscordTimeStampForDate(fullMeetDateTimeLocal, DiscordTimeStampFormat.FullDateTimeWithDayOfWeek) & " your local time**" & vbCrLf & vbCrLf)

        If txtEventTitle.Text <> String.Empty Then
            If cboGroupOrClubName.SelectedIndex > -1 Then
                txtGroupFlightEventPost.AppendText(cboGroupOrClubName.Text & " - ")
            End If
            txtGroupFlightEventPost.AppendText(txtEventTitle.Text & vbCrLf & vbCrLf)
        End If
        txtGroupFlightEventPost.AppendText(ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))
        txtGroupFlightEventPost.AppendText("**Server:** " & cboMSFSServer.Text & vbCrLf)
        txtGroupFlightEventPost.AppendText("**Voice:** " & cboVoiceChannel.Text & vbCrLf & vbCrLf)

        txtGroupFlightEventPost.AppendText("**Meet/Briefing:** " &
                                           Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("dddd, MMMM dd", EnglishCulture) & ", " &
                                           Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("hhmm tt", EnglishCulture) & " Zulu / " &
                                           GetDiscordTimeStampForDate(fullMeetDateTimeLocal, DiscordTimeStampFormat.FullDateTimeWithDayOfWeek) & " your local time" & vbCrLf &
                                           "At this time we meet in the voice chat and get ready." & vbCrLf)


        If txtTaskFlightPlanURL.Text <> String.Empty Then
            txtGroupFlightEventPost.AppendText(vbCrLf & "**Flight Plan Details, Weather and files** " & vbCrLf & txtTaskFlightPlanURL.Text & vbCrLf & vbCrLf)
            If chkIncludeGotGravelInvite.Checked AndAlso chkIncludeGotGravelInvite.Enabled Then
                txtGroupFlightEventPost.AppendText("If you did not join Got Gravel already, you will need this invite link first: https://discord.gg/BqUcbvDP69" & vbCrLf & vbCrLf)
            End If
        Else
            txtGroupFlightEventPost.AppendText(vbCrLf)
        End If
        txtGroupFlightEventPost.AppendText("**Sim date And time:** " & dtSimDate.Value.ToString(dateFormat, EnglishCulture) & ", " &
                                           dtSimLocalTime.Value.ToString("hh:mm tt", EnglishCulture) & " local" &
                                           ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True) & vbCrLf)
        If txtFlightPlanFile.Text <> String.Empty Then
            txtGroupFlightEventPost.AppendText("**Flight plan file:** " & Chr(34) & Path.GetFileName(txtFlightPlanFile.Text) & Chr(34) & vbCrLf)
        End If
        If txtWeatherFile.Text <> String.Empty AndAlso (Not weatherDetails Is Nothing) Then
            txtGroupFlightEventPost.AppendText("**Weather file & profile name:** " & Chr(34) & Path.GetFileName(txtWeatherFile.Text) & Chr(34) & " (" & weatherDetails.PresetName & ")" & vbCrLf)
        End If
        txtGroupFlightEventPost.AppendText(vbCrLf)

        If chkUseSyncFly.Checked Then
            txtGroupFlightEventPost.AppendText("**Synchronized Fly:** " &
                                    Conversions.ConvertLocalToUTC(fullSyncFlyDateTimeLocal).ToString("hhmm tt", EnglishCulture) & " Zulu / " &
                                    GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, DiscordTimeStampFormat.TimeOnlyWithoutSeconds) & " your local time" & vbCrLf &
                                    "At this time we simultaneously click fly to sync our weather." & vbCrLf)
            If chkUseLaunch.Checked AndAlso fullSyncFlyDateTimeLocal = fullLaunchDateTimeLocal Then
                txtGroupFlightEventPost.AppendText("At this time we can also start launching from the airfield." & vbCrLf & vbCrLf)
            Else
                txtGroupFlightEventPost.AppendText(vbCrLf)
            End If
        End If

        If chkUseLaunch.Checked AndAlso (fullSyncFlyDateTimeLocal <> fullLaunchDateTimeLocal OrElse Not chkUseSyncFly.Checked) Then
            txtGroupFlightEventPost.AppendText("**Launch:** " &
                                        Conversions.ConvertLocalToUTC(fullLaunchDateTimeLocal).ToString("hhmm tt", EnglishCulture) & " Zulu / " &
                                        GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, DiscordTimeStampFormat.TimeOnlyWithoutSeconds) & " your local time" & vbCrLf &
                                        "At this time we can start launching from the airfield." & vbCrLf & vbCrLf)
        End If

        If chkUseStart.Checked Then
            txtGroupFlightEventPost.AppendText("**Task Start:** " &
                                    Conversions.ConvertLocalToUTC(fullStartTaskDateTimeLocal).ToString("hhmm tt", EnglishCulture) & " Zulu / " &
                                    GetDiscordTimeStampForDate(fullStartTaskDateTimeLocal, DiscordTimeStampFormat.TimeOnlyWithoutSeconds) & " your local time" & vbCrLf &
                                    "At this time we cross the starting line and start the task." & vbCrLf & vbCrLf)
        End If

        txtGroupFlightEventPost.AppendText("**Duration:** " & GetDuration(txtDurationMin, txtDurationMax) & ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True) & vbCrLf)

        If cboEligibleAward.SelectedIndex > 0 Then
            txtGroupFlightEventPost.AppendText(vbCrLf & "Pilots who finish this task successfully during the event will be eligible to apply for the " & cboEligibleAward.Text & " Soaring Badge :" & cboEligibleAward.Text.ToLower & ":" & vbCrLf)
        End If

        If txtCredits.Text <> String.Empty Then
            txtGroupFlightEventPost.AppendText(vbCrLf & txtCredits.Text & vbCrLf)
        End If

        BuildDiscordEventDescription()

    End Sub

    Private Enum DiscordTimeStampFormat As Integer
        TimeOnlyWithoutSeconds = 0
        FullDateTimeWithDayOfWeek = 1
        LongDateTime = 2
        CountDown = 3
    End Enum
    Private Function GetDiscordTimeStampForDate(dateToUse As DateTime, format As DiscordTimeStampFormat) As String

        Dim formatAbbr As String = String.Empty

        Select Case format
            Case DiscordTimeStampFormat.TimeOnlyWithoutSeconds
                formatAbbr = ":t>"
            Case DiscordTimeStampFormat.FullDateTimeWithDayOfWeek
                formatAbbr = ":F>"
            Case DiscordTimeStampFormat.LongDateTime
                formatAbbr = ":f>"
            Case DiscordTimeStampFormat.CountDown
                formatAbbr = ":R>"

        End Select

        Return "<t:" & Conversions.ConvertDateToUnixTimestamp(dateToUse).ToString & formatAbbr

    End Function

    Private Sub GroupFlightFieldLeave(sender As Object, e As EventArgs) Handles cboVoiceChannel.Leave, cboMSFSServer.Leave, cboEligibleAward.Leave, chkUseSyncFly.CheckedChanged, chkUseStart.CheckedChanged, chkUseLaunch.CheckedChanged, cboVoiceChannel.SelectedIndexChanged, cboMSFSServer.SelectedIndexChanged, cboEligibleAward.SelectedIndexChanged
        BuildGroupFlightPost()
    End Sub

    Private Sub btnTaskFPURLPaste_Click(sender As Object, e As EventArgs) Handles btnTaskFPURLPaste.Click
        txtTaskFlightPlanURL.Text = Clipboard.GetText
        BuildGroupFlightPost()
    End Sub

    Private Sub btnDiscordGroupEventURL_Click(sender As Object, e As EventArgs) Handles btnDiscordGroupEventURL.Click
        txtGroupEventPostURL.Text = Clipboard.GetText
        BuildDiscordEventDescription()
    End Sub

    Private Sub btnGroupFlightEventInfoToClipboard_Click(sender As Object, e As EventArgs) Handles btnGroupFlightEventInfoToClipboard.Click
        Clipboard.SetText(txtGroupFlightEventPost.Text)
        MsgBox("You can now post the group flight event in the proper Discord channel for the club/group." & vbCrLf &
               "Then copy the link to that newly created message." & vbCrLf &
               "Finally, paste the link in the URL field on section 2 for Discord Event.", vbOKOnly Or MsgBoxStyle.Information, "Creating group flight post")

        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If

    End Sub

    Private Sub BuildDiscordEventDescription()

        txtDiscordEventTopic.Text = String.Empty

        If txtEventTitle.Text <> String.Empty Then
            If cboGroupOrClubName.SelectedIndex > -1 Then
                txtDiscordEventTopic.AppendText(cboGroupOrClubName.Text & " - ")
            End If
            txtDiscordEventTopic.AppendText(txtEventTitle.Text & vbCrLf & vbCrLf)
        End If

        txtDiscordEventDescription.Text = String.Empty
        txtDiscordEventDescription.AppendText("**Server:** " & cboMSFSServer.Text & vbCrLf)
        txtDiscordEventDescription.AppendText("**Duration:** " & GetDuration(txtDurationMin, txtDurationMax) & ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True) & vbCrLf & vbCrLf)
        txtDiscordEventDescription.AppendText(ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))
        txtDiscordEventDescription.AppendText("**More Information on this group flight event:** " & vbCrLf)
        txtDiscordEventDescription.AppendText(txtGroupEventPostURL.Text & vbCrLf)

    End Sub

    Private Sub txtGroupEventPostURL_Leave(sender As Object, e As EventArgs)
        BuildDiscordEventDescription()
    End Sub

    Private Sub btnEventTopicClipboard_Click(sender As Object, e As EventArgs) Handles btnEventTopicClipboard.Click
        If txtDiscordEventTopic.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventTopic.Text)
            MsgBox("Paste the topic into the Event Topic field on Discord.", vbOKOnly Or MsgBoxStyle.Information, "Creating Discord Event")
        End If
        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If

    End Sub

    Private Sub btnEventDescriptionToClipboard_Click(sender As Object, e As EventArgs) Handles btnEventDescriptionToClipboard.Click
        If txtDiscordEventDescription.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventDescription.Text)
            MsgBox("Paste the description into the Event Description field on Discord.", vbOKOnly Or MsgBoxStyle.Information, "Creating Discord Event")
        End If
        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If

    End Sub

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        SaveSessionData(Application.StartupPath & "\LastSession.dph")

    End Sub

    Private Sub SaveSessionData(filename As String)

        Dim allCurrentData As New AllData()

        With allCurrentData
            .FlightPlanFilename = txtFlightPlanFile.Text
            .WeatherFilename = txtWeatherFile.Text
            .LockTitle = chkTitleLock.Checked
            .Title = txtTitle.Text
            .SimDate = dtSimDate.Value
            .SimTime = dtSimLocalTime.Value
            .IncludeYear = chkIncludeYear.Checked
            .SimDateTimeExtraInfo = txtSimDateTimeExtraInfo.Text
            .MainAreaPOI = txtMainArea.Text
            .DepartureLock = chkDepartureLock.Checked
            .DepartureICAO = txtDepartureICAO.Text
            .DepartureName = txtDepName.Text
            .DepartureExtra = txtDepExtraInfo.Text
            .ArrivalLock = chkArrivalLock.Checked
            .ArrivalICAO = txtArrivalICAO.Text
            .ArrivalName = txtArrivalName.Text
            .ArrivalExtra = txtArrivalExtraInfo.Text
            .SoaringRidge = chkSoaringTypeRidge.Checked
            .SoaringThermals = chkSoaringTypeThermal.Checked
            .SoaringExtraInfo = txtSoaringTypeExtraInfo.Text
            .AvgSpeedsUnit = cboSpeedUnits.SelectedIndex
            .AvgMinSpeed = txtMinAvgSpeed.Text
            .AvgMaxSpeed = txtMaxAvgSpeed.Text
            .DurationMin = txtDurationMin.Text
            .DurationMax = txtDurationMax.Text
            .DurationExtraInfo = txtDurationExtraInfo.Text
            .RecommendedGliders = cboRecommendedGliders.Text
            .DifficultyRating = cboDifficulty.Text
            .DifficultyExtraInfo = txtDifficultyExtraInfo.Text
            .LockShortDescription = chkDescriptionLock.Checked
            .ShortDescription = txtShortDescription.Text.Replace(vbCrLf, "($*$)")
            .Credits = txtCredits.Text
            .LongDescription = txtLongDescription.Text.Replace(vbCrLf, "($*$)")
            .WeatherSummaryOnly = chkUseOnlyWeatherSummary.Checked
            .WeatherSummary = txtWeatherSummary.Text
            For i As Integer = 0 To lstAllFiles.Items.Count - 1
                .ExtraFiles.Add(lstAllFiles.Items(i))
            Next
            .GroupClub = cboGroupOrClubName.Text
            .EventTopic = txtEventTitle.Text
            .MSFSServer = cboMSFSServer.SelectedIndex
            .VoiceChannel = cboVoiceChannel.Text
            .UTCSelected = chkDateTimeUTC.Checked
            .EventMeetDate = dtEventMeetDate.Value
            .EventMeetTime = dtEventMeetTime.Value
            .UseEventSyncFly = chkUseSyncFly.Checked
            .EventSyncFlyDate = dtEventSyncFlyDate.Value
            .EventSyncFlyTime = dtEventSyncFlyTime.Value
            .UseEventLaunch = chkUseLaunch.Checked
            .EventLaunchDate = dtEventLaunchDate.Value
            .EventLaunchTime = dtEventLaunchTime.Value
            .UseEventStartTask = chkUseStart.Checked
            .EventStartTaskDate = dtEventStartTaskDate.Value
            .EventStartTaskTime = dtEventStartTaskTime.Value
            .EventDescription = txtEventDescription.Text.Replace(vbCrLf, "($*$)")
            .EligibleAward = cboEligibleAward.SelectedIndex
            .URLFlightPlanPost = txtTaskFlightPlanURL.Text
            .URLGroupEventPost = txtGroupEventPostURL.Text
            .IncludeGGServerInvite = chkIncludeGotGravelInvite.Checked
        End With

        Dim serializer As New XmlSerializer(GetType(AllData))
        Using stream As New FileStream(filename, FileMode.Create)
            serializer.Serialize(stream, allCurrentData)
        End Using

    End Sub

    Private Sub LoadSessionData(filename As String)
        If File.Exists(filename) Then
            Dim serializer As New XmlSerializer(GetType(AllData))
            Dim allCurrentData As AllData

            On Error Resume Next

            Using stream As New FileStream(filename, FileMode.Open)
                allCurrentData = CType(serializer.Deserialize(stream), AllData)
            End Using

            'Set all fields
            With allCurrentData
                If File.Exists(.FlightPlanFilename) Then
                Else
                    'Should expect the file to be in the same folder as the .dph file
                    .FlightPlanFilename = Path.GetDirectoryName(filename) & "\" & Path.GetFileName(.FlightPlanFilename)
                End If
                txtFlightPlanFile.Text = .FlightPlanFilename
                Me.Update()
                LoadFlightPlan(txtFlightPlanFile.Text)

                If File.Exists(.WeatherFilename) Then
                Else
                    'Should expect the file to be in the same folder as the .dph file
                    .WeatherFilename = Path.GetDirectoryName(filename) & "\" & Path.GetFileName(.WeatherFilename)
                End If
                txtWeatherFile.Text = .WeatherFilename
                Me.Update()
                LoadWeatherfile(txtWeatherFile.Text)

                chkTitleLock.Checked = .LockTitle
                txtTitle.Text = .Title
                dtSimDate.Value = .SimDate
                dtSimLocalTime.Value = .SimTime
                chkIncludeYear.Checked = .IncludeYear
                txtSimDateTimeExtraInfo.Text = .SimDateTimeExtraInfo
                txtMainArea.Text = .MainAreaPOI
                chkDepartureLock.Checked = .DepartureLock
                txtDepartureICAO.Text = .DepartureICAO
                txtDepName.Text = .DepartureName
                txtDepExtraInfo.Text = .DepartureExtra
                chkArrivalLock.Checked = .ArrivalLock
                txtArrivalICAO.Text = .ArrivalICAO
                txtArrivalName.Text = .ArrivalName
                txtArrivalExtraInfo.Text = .ArrivalExtra
                chkSoaringTypeRidge.Checked = .SoaringRidge
                chkSoaringTypeThermal.Checked = .SoaringThermals
                txtSoaringTypeExtraInfo.Text = .SoaringExtraInfo
                cboSpeedUnits.SelectedIndex = .AvgSpeedsUnit
                txtMinAvgSpeed.Text = .AvgMinSpeed
                txtMaxAvgSpeed.Text = .AvgMaxSpeed
                txtDurationMin.Text = .DurationMin
                txtDurationMax.Text = .DurationMax
                txtDurationExtraInfo.Text = .DurationExtraInfo
                cboRecommendedGliders.Text = .RecommendedGliders
                cboDifficulty.Text = .DifficultyRating
                txtDifficultyExtraInfo.Text = .DifficultyExtraInfo
                chkDescriptionLock.Checked = .LockShortDescription
                txtShortDescription.Text = .ShortDescription.Replace("($*$)", vbCrLf)
                txtCredits.Text = .Credits
                txtLongDescription.Text = .LongDescription.Replace("($*$)", vbCrLf)
                chkUseOnlyWeatherSummary.Checked = .WeatherSummaryOnly
                txtWeatherSummary.Text = .WeatherSummary
                If .ExtraFiles.Count > 0 Then
                    For i As Integer = 0 To .ExtraFiles.Count - 1

                        If File.Exists(.ExtraFiles(i)) Then
                        Else
                            'Should expect the file to be in the same folder as the .dph file
                            .ExtraFiles(i) = Path.GetDirectoryName(filename) & "\" & Path.GetFileName(.ExtraFiles(i))
                        End If

                        lstAllFiles.Items.Add(.ExtraFiles(i))
                    Next
                End If
                cboGroupOrClubName.Text = .GroupClub
                txtEventTitle.Text = .EventTopic
                cboMSFSServer.SelectedIndex = .MSFSServer
                cboVoiceChannel.Text = .VoiceChannel
                chkDateTimeUTC.Checked = .UTCSelected
                dtEventMeetDate.Value = .EventMeetDate
                dtEventMeetTime.Value = .EventMeetTime
                chkUseSyncFly.Checked = .UseEventSyncFly
                dtEventSyncFlyDate.Value = .EventSyncFlyDate
                dtEventSyncFlyTime.Value = .EventSyncFlyTime
                chkUseLaunch.Checked = .UseEventLaunch
                dtEventLaunchDate.Value = .EventLaunchDate
                dtEventLaunchTime.Value = .EventLaunchTime
                chkUseStart.Checked = .UseEventStartTask
                dtEventStartTaskDate.Value = .EventStartTaskDate
                dtEventStartTaskTime.Value = .EventStartTaskTime
                txtEventDescription.Text = .EventDescription.Replace("($*$)", vbCrLf)
                cboEligibleAward.SelectedIndex = .EligibleAward
                txtTaskFlightPlanURL.Text = .URLFlightPlanPost
                txtGroupEventPostURL.Text = .URLGroupEventPost
                chkIncludeGotGravelInvite.Checked = .IncludeGGServerInvite

            End With

            BuildFPResults()
            BuildWeatherInfoResults()
            BuildGroupFlightPost()
            BuildDiscordEventDescription()

        End If

    End Sub

    Private Sub btnLoadConfig_Click(sender As Object, e As EventArgs) Handles btnLoadConfig.Click

        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If

        OpenFileDialog1.FileName = String.Empty
        OpenFileDialog1.Title = "Select session file to load"
        OpenFileDialog1.Filter = "Discord Post Helper Session|*.dph"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            ResetForm()
            LoadSessionData(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub btnSaveConfig_Click(sender As Object, e As EventArgs) Handles btnSaveConfig.Click

        If txtFlightPlanFile.Text = String.Empty Then
            SaveFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            SaveFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If

        SaveFileDialog1.FileName = txtTitle.Text
        SaveFileDialog1.Title = "Select session file to save"
        SaveFileDialog1.Filter = "Discord Post Helper Session|*.dph"

        Dim result As DialogResult = SaveFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            SaveSessionData(SaveFileDialog1.FileName)
        End If

    End Sub

    Private Sub btnCreateShareablePack_Click(sender As Object, e As EventArgs) Handles btnCreateShareablePack.Click

        If txtTitle.Text = String.Empty Then
            MsgBox("A title must be specified to package your session.", vbOKOnly Or MsgBoxStyle.Critical, "No title")
        End If

        'Ask for a new folder where to put all the files
        FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(Application.StartupPath)
        FolderBrowserDialog1.Description = "Select folder where to create your session package called " & Chr(34) & txtTitle.Text & Chr(34)
        FolderBrowserDialog1.ShowNewFolderButton = False

        Dim result As DialogResult = FolderBrowserDialog1.ShowDialog()

        If result = DialogResult.OK Then
            'Check if folder already exists
            If Directory.Exists(FolderBrowserDialog1.SelectedPath & "\" & txtTitle.Text) Then
                If MsgBox("The folder already exists, do you want to continue?", vbYesNo Or MsgBoxStyle.Question, "Confirm creation of package in existing folder") = vbNo Then
                    Exit Sub
                End If
            End If
            'Create folder
            Dim packageTitle As String = txtTitle.Text
            Dim packageFolder As String = FolderBrowserDialog1.SelectedPath & "\" & packageTitle
            Directory.CreateDirectory(packageFolder)
            'Copy all files into that folder
            File.Copy(txtFlightPlanFile.Text, packageFolder & "\" & Path.GetFileName(txtFlightPlanFile.Text))
            File.Copy(txtWeatherFile.Text, packageFolder & "\" & Path.GetFileName(txtWeatherFile.Text))
            For i As Integer = 0 To lstAllFiles.Items.Count - 1
                File.Copy(lstAllFiles.Items(i), packageFolder & "\" & Path.GetFileName(lstAllFiles.Items(i)))
            Next

            'Save session file with incorrect paths
            SaveSessionData(packageFolder & "\" & packageTitle & ".dph")

            MsgBox("You can now zip and share your working session package with someone else.", vbOKOnly Or vbInformation, "Shareable Session Package created")

        End If

    End Sub

    Private Sub chkIncludeGotGravelInvite_CheckedChanged(sender As Object, e As EventArgs) Handles chkIncludeGotGravelInvite.CheckedChanged
        BuildGroupFlightPost()
    End Sub

    Private Sub txtTaskFlightPlanURL_TextChanged(sender As Object, e As EventArgs) Handles txtTaskFlightPlanURL.TextChanged
        If txtTaskFlightPlanURL.Text <> String.Empty Then
            If txtTaskFlightPlanURL.Text.Contains("channels/793376245915189268") Then
                'Got Gravel
                chkIncludeGotGravelInvite.Enabled = True
            Else
                chkIncludeGotGravelInvite.Checked = False
                chkIncludeGotGravelInvite.Enabled = False
            End If
        End If
    End Sub

    Private Sub txtFlightPlanFile_TextChanged(sender As Object, e As EventArgs) Handles txtFlightPlanFile.TextChanged

        If txtFlightPlanFile.Text = String.Empty Then
            grbTrackInfo.Enabled = False
        Else
            grbTrackInfo.Enabled = True
        End If

    End Sub

    Private Sub txtDistanceTotal_TextChanged(sender As Object, e As EventArgs) Handles txtDistanceTrack.TextChanged, txtDistanceTotal.TextChanged

        'One of the distance has changed - recalculate their corresponding labels and miles
        If txtDistanceTotal.Text <> String.Empty Then
            lblTotalDistanceAndMiles.Text = "km / " & FormatNumber(Conversions.KmToMiles(Decimal.Parse(txtDistanceTotal.Text)), 0) & " mi Total"
        End If
        If txtDistanceTrack.Text <> String.Empty Then
            lblTrackDistanceAndMiles.Text = "km / " & FormatNumber(Conversions.KmToMiles(Decimal.Parse(txtDistanceTrack.Text)), 0) & " mi Track"
        End If


    End Sub

    Private Sub btnLoadB21Planner_Click(sender As Object, e As EventArgs) Handles btnLoadB21Planner.Click

        If txtFlightPlanFile.Text Is String.Empty Then
            System.Diagnostics.Process.Start(B21PlannerURL)
        Else
            Dim tempFolderName As String = GenerateRandomFileName()
            Dim flightPlanName As String = Path.GetFileNameWithoutExtension(txtFlightPlanFile.Text)

            UploadFile(tempFolderName, flightPlanName)

            System.Diagnostics.Process.Start(B21PlannerURL & "?pln=siglr.com/DiscordPostHelper/FlightPlans/" & tempFolderName & "/" & flightPlanName & ".pln")

            'Wait 5 seconds
            Thread.Sleep(5000)
            DeleteTempFile(tempFolderName)

            If MsgBox("After reviewing or editing the flight plan, did you make any modification and would like to reload the flight plan here?", vbYesNo Or vbQuestion, "Coming back from B21 Planner") = vbYes Then
                'Reload the flight plan
                LoadFlightPlan(txtFlightPlanFile.Text)
            End If

        End If

    End Sub

    Private Sub btnCopyReqFilesToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyReqFilesToClipboard.Click

        Dim allFiles As New Specialized.StringCollection

        If File.Exists(txtFlightPlanFile.Text) Then
            allFiles.Add(txtFlightPlanFile.Text)
        End If
        If File.Exists(txtWeatherFile.Text) Then
            allFiles.Add(txtWeatherFile.Text)
        End If

        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            MsgBox("Now paste the copied files in a new post in the proper Discord channel for the club/group and come back for the text info (button 1 below).", vbOKOnly Or MsgBoxStyle.Exclamation, "Optional - Including the required files in the group flight post")
        End If

        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep = intGuideCurrentStep + 1
            ShowGuide()
        End If

    End Sub

    Private Sub EnterTextBox(sender As Object, e As EventArgs) Handles txtWeatherSummary.Enter, txtTitle.Enter, txtSoaringTypeExtraInfo.Enter, txtSimDateTimeExtraInfo.Enter, txtShortDescription.Enter, txtMinAvgSpeed.Enter, txtMaxAvgSpeed.Enter, txtMainArea.Enter, txtLongDescription.Enter, txtDurationMin.Enter, txtDurationMax.Enter, txtDurationExtraInfo.Enter, txtDifficultyExtraInfo.Enter, txtDepName.Enter, txtDepExtraInfo.Enter, txtDepartureICAO.Enter, txtCredits.Enter, txtArrivalName.Enter, txtArrivalICAO.Enter, txtArrivalExtraInfo.Enter, txtWeatherWinds.Enter, txtWeatherFirstPart.Enter, txtWeatherClouds.Enter, txtFullDescriptionResults.Enter, txtFPResults.Enter, txtFilesText.Enter, txtAltRestrictions.Enter, txtTaskFlightPlanURL.Enter, txtGroupFlightEventPost.Enter, txtGroupEventPostURL.Enter, txtEventTitle.Enter, txtEventDescription.Enter, txtDiscordEventTopic.Enter, txtDiscordEventDescription.Enter
        EnteringTextBox(sender)
    End Sub

    Private Sub txtEventTitle_Leave(sender As Object, e As EventArgs) Handles txtTaskFlightPlanURL.Leave, txtGroupFlightEventPost.Leave, txtGroupEventPostURL.Leave, txtEventTitle.Leave, txtEventDescription.Leave, txtDiscordEventTopic.Leave, txtDiscordEventDescription.Leave
        LeavingTextBox(sender)
    End Sub

#End Region

#Region "Guide"
    Private Sub btnGuideMe_Click(sender As Object, e As EventArgs) Handles btnGuideMe.Click

        If MsgBox("Do you want to start by resetting everything ?", vbYesNo Or vbQuestion, "Starting the Discord Post Helper Wizard") = vbYes Then
            ResetForm()
        End If
        TabControl1.SelectedTab = TabControl1.TabPages("tabFlightPlan")
        intGuideCurrentStep = 1
        btnTurnGuideOff.Visible = True
        ShowGuide()

    End Sub

    Private Sub ShowGuide(Optional fromF1Key As Boolean = False)

        If intGuideCurrentStep > 0 Then
            btnTurnGuideOff.Visible = True
        End If

        Select Case intGuideCurrentStep
            Case 0
                pnlGuide.Visible = False
                pnlWizardEvent.Visible = False
                btnTurnGuideOff.Visible = False
            Case 1 'Select flight plan
                SetGuidePanelToLeft()
                pnlGuide.Top = -9
                lblGuideInstructions.Text = "Click the ""Flight Plan"" button and select the flight plan to use with this task."
                SetFocusOnField(btnSelectFlightPlan, fromF1Key)
            Case 2 'Select weather file
                SetGuidePanelToLeft()
                pnlGuide.Top = 54
                lblGuideInstructions.Text = "Click the ""Weather file"" button and select the weather file to use with this task."
                SetFocusOnField(btnSelectWeatherFile, fromF1Key)
            Case 3 'Title
                SetGuidePanelToLeft()
                pnlGuide.Top = 95
                lblGuideInstructions.Text = "This is the title that was read from the flight plan. Is it ok? If not, change it and use the checkbox to prevent it from being overwritten."
                SetFocusOnField(txtTitle, fromF1Key)
            Case 4 'Sim date & time
                SetGuidePanelToLeft()
                pnlGuide.Top = 139
                lblGuideInstructions.Text = "Specify the appropriate local date and time to set inside MSFS. You can also add extra information in the text box if you want."
                SetFocusOnField(dtSimDate, fromF1Key)
            Case 5 'Main area
                SetGuidePanelToLeft()
                pnlGuide.Top = 194
                lblGuideInstructions.Text = "Do you want to specify the main area and/or point of interest for this task? It's only optionnal."
                SetFocusOnField(txtMainArea, fromF1Key)
            Case 6 'Departure
                SetGuidePanelToLeft()
                pnlGuide.Top = 231
                lblGuideInstructions.Text = "This is the departure airfield that was read from the flight plan. You can change the name and add extra information and use the checkbox to prevent overwritting."
                SetFocusOnField(txtDepName, fromF1Key)
            Case 7 'Arrival
                SetGuidePanelToLeft()
                pnlGuide.Top = 266
                lblGuideInstructions.Text = "Same for the arrival airfield. You can change the name and add extra information and use the checkbox to prevent overwritting."
                SetFocusOnField(txtArrivalName, fromF1Key)
            Case 8 'Soaring type
                SetGuidePanelToLeft()
                pnlGuide.Top = 299
                lblGuideInstructions.Text = "Specify the type of soaring that the user will expect during the task with the provided weather. You can also add extra information."
                SetFocusOnField(chkSoaringTypeRidge, fromF1Key)
            Case 9 'Distance
                SetGuidePanelToLeft()
                pnlGuide.Top = 332
                lblGuideInstructions.Text = "The total distance and track only distance are calculated automatically based on the provided flight plan, there's nothing to do here."
                SetFocusOnField(txtDistanceTotal, fromF1Key)
            Case 10 'Speeds
                SetGuidePanelToLeft()
                pnlGuide.Top = 371
                lblGuideInstructions.Text = "Optionally, you can specify the average min and max speeds to expect in the task. This pre-calculate the duration of the task below."
                SetFocusOnField(txtMinAvgSpeed, fromF1Key)
            Case 11 'Durations
                SetGuidePanelToLeft()
                pnlGuide.Top = 403
                lblGuideInstructions.Text = "If you entered speeds above, these will be pre-filled. You can adjust or enter the expected min and max duration of the task as you see fit, and also enter extra information."
                SetFocusOnField(txtDurationMin, fromF1Key)
            Case 12 'Gliders
                SetGuidePanelToLeft()
                pnlGuide.Top = 435
                lblGuideInstructions.Text = "Specify the recommended gliders for this task. You can select from the list, or enter anything else you like."
                SetFocusOnField(cboRecommendedGliders, fromF1Key)
            Case 13 'Difficulty
                SetGuidePanelToLeft()
                pnlGuide.Top = 470
                lblGuideInstructions.Text = "Specify the task difficulty rating and any extra information. If you don't want to use the rating system, just specify ""None/Custom"" and use the extra info."
                SetFocusOnField(cboDifficulty, fromF1Key)
            Case 14 'Short description
                SetGuidePanelToLeft()
                pnlGuide.Top = 514
                lblGuideInstructions.Text = "Optionally, you can provide a short description of the task that will be displayed in the main task post and use the checkbox to prevent overwritting."
                SetFocusOnField(txtShortDescription, fromF1Key)
            Case 15 'Credits
                SetGuidePanelToLeft()
                pnlGuide.Top = 565
                lblGuideInstructions.Text = "Optionally (but strongly suggested), you should give credits to the task designer. Try to use @DiscordUserName."
                SetFocusOnField(txtCredits, fromF1Key)
            Case 16 'Long description
                SetGuidePanelToLeft()
                pnlGuide.Top = 627
                lblGuideInstructions.Text = "Optionally, you should provide a more detailed description of the task. Context, history, hints, tips, tricks around waypoints, etc."
                SetFocusOnField(txtLongDescription, fromF1Key)
            Case 17 'Weather summary
                SetGuidePanelToLeft()
                pnlGuide.Top = 942
                lblGuideInstructions.Text = "Optional weather summary. If you don't want the full weather details to be included, tick the checkbox to the left. Only the summary will then be shown."
                SetFocusOnField(txtWeatherSummary, fromF1Key)
            Case 18 'Extra files
                SetGuidePanelToLeft()
                Me.pnlArrow.BackgroundImage = Global.DiscordHelper.My.Resources.Resources.left_arrow
                pnlGuide.Top = 1006
                lblGuideInstructions.Text = "Optionally, use this section to add and remove any extra files you want included with the task post. Maps, XCSoar track files, other images, etc."
                SetFocusOnField(btnAddExtraFile, fromF1Key)
            Case 19 'Save & Load
                SetGuidePanelToTop()
                pnlGuide.Left = 699
                lblGuideInstructions.Text = "Whenever you like, you can Save your current track's data to your computer and Load it back when you are ready to continue working on it."
                SetFocusOnField(btnSaveConfig, fromF1Key)
            Case 20 'Share package
                SetGuidePanelToTop()
                pnlGuide.Left = 885
                lblGuideInstructions.Text = "You can also share your task's data and all the files by creating a package which you can zip and send to someone else."
                SetFocusOnField(btnCreateShareablePack, fromF1Key)
            Case 21 'Create FP post
                SetGuidePanelToRight()
                pnlGuide.Top = 21
                lblGuideInstructions.Text = "You are now ready to create the task's primary post in Discord. Click this button to copy the content to your clipboard and receive instructions."
                SetFocusOnField(btnFPMainInfoCopy, fromF1Key)
            Case 22 'Create Restrictions and Weather post
                SetGuidePanelToRight()
                pnlGuide.Top = 392
                lblGuideInstructions.Text = "Once you've created the primary post and the thread on Discord, click this second button and receive instructions for the next post."
                SetFocusOnField(btnAltRestricCopy, fromF1Key)
            Case 23 'Copy Files
                SetGuidePanelToRight()
                pnlGuide.Top = 899
                lblGuideInstructions.Text = "Once you've created the second post, click this button to put the files into your clipboard and receive instructions."
                SetFocusOnField(btnFilesCopy, fromF1Key)
            Case 24 'Copy Files Legend
                If Not chkGroupSecondaryPosts.Checked Then
                    SetGuidePanelToRight()
                    pnlGuide.Top = 954
                    lblGuideInstructions.Text = "Once you've pasted the actual files in Discord, click this button to put the standard legend into your clipboard and receive instructions."
                    SetFocusOnField(btnFilesTextCopy, fromF1Key)
                Else
                    intGuideCurrentStep += 1
                    ShowGuide()
                End If

            Case 25 'Copy Description
                If Not chkGroupSecondaryPosts.Checked Then
                    SetGuidePanelToRight()
                    pnlGuide.Top = 1027
                    lblGuideInstructions.Text = "One last step, click this button to copy the full description to your clipboard and receive instructions."
                    SetFocusOnField(btnFullDescriptionCopy, fromF1Key)
                Else
                    intGuideCurrentStep += 1
                    ShowGuide()
                End If

            Case 26 'Event
                If MsgBox("The task's details are all posted. Are you also creating the group flight post on Discord ?", vbYesNo Or MsgBoxStyle.Question, "Discord Post Helper Wizard") = vbYes Then
                    intGuideCurrentStep += 1
                Else
                    intGuideCurrentStep = 999
                End If
                ShowGuide()

            Case 27 'Event
                'Resume wizard on the Event tab
                TabControl1.SelectedTab = TabControl1.TabPages("tabEvent")
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 69
                lblEventGuideInstructions.Text = "Start by selecting the soaring club or known group for which you want to create a new event, if this applies to you."
                SetFocusOnField(cboGroupOrClubName, fromF1Key)

            Case 28 'Group flight title / topic
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 108
                lblEventGuideInstructions.Text = "If you would like to specify a different title for the group flight, you can do so now. Otherwise, this is the same as the task's title."
                SetFocusOnField(txtEventTitle, fromF1Key)

            Case 29 'MSFS Server
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 143
                lblEventGuideInstructions.Text = "Specify the MSFS Server to use during the group flight. If you specified a known club, then you should not change this, unless it is incorrect."
                SetFocusOnField(cboMSFSServer, fromF1Key)

            Case 30 'Voice channel
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 179
                lblEventGuideInstructions.Text = "Specify the Discord Voice channel to use during the group flight. If you specified a known club, then you should not change this, unless it is incorrect."
                SetFocusOnField(cboVoiceChannel, fromF1Key)

            Case 31 'UTC Zulu
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 212
                lblEventGuideInstructions.Text = "For the sake of simplicity, leave this checkbox ticked to use UTC (Zulu) entries. Local times are still displayed to the right."
                SetFocusOnField(chkDateTimeUTC, fromF1Key)

            Case 32 'Meet time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 251
                lblEventGuideInstructions.Text = "Specify the meet date and time. This is the time when people will start gathering for the group flight and briefing."
                SetFocusOnField(dtEventMeetDate, fromF1Key)

            Case 33 'Sync Fly
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 280
                lblEventGuideInstructions.Text = "Only if the flight's conditions require a synchronized click ""Fly"", then tick the ""Yes"" checkbox and specify when it will happen."
                SetFocusOnField(chkUseSyncFly, fromF1Key)

            Case 34 'Launch Time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 315
                lblEventGuideInstructions.Text = "If you want to specify the time when people should start to launch from the airfield, tick the ""Yes"" checkbox and specify when it should happen."
                SetFocusOnField(chkUseLaunch, fromF1Key)

            Case 35 'Start Task Time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 349
                lblEventGuideInstructions.Text = "If you want to specify a time for the start of the task (going through the start gate), tick the ""Yes"" checkbox and specify when it should happen."
                SetFocusOnField(chkUseStart, fromF1Key)

            Case 36 'Group flight description
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 394
                lblEventGuideInstructions.Text = "If you would like to specify a different description for the group flight, you can do so now. Otherwise, this is the same as the task's short description."
                SetFocusOnField(txtEventDescription, fromF1Key)

            Case 37 'SSC Award
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 441
                lblEventGuideInstructions.Text = "This is usually set automatically if the club is SSC Saturday and depending on the task's distance. You should leave it alone, unless it's incorrect."
                SetFocusOnField(cboEligibleAward, fromF1Key)

            Case 38 'Task URL
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 477
                lblEventGuideInstructions.Text = "In Discord, find the published task's main post (created earlier) and copy the link to that post (usually by using the ... menu associated with the post). Then click Paste here."
                SetFocusOnField(btnTaskFPURLPaste, fromF1Key)

            Case 39 'GotGravel Invite
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 512
                lblEventGuideInstructions.Text = "If the link above is from GotGravel, you have the option to include an invite to the server along with the group flight info. This is useful if published outside of GotGravel."
                SetFocusOnField(chkIncludeGotGravelInvite, fromF1Key)

            Case 40 'Group flight post - files
                SetEventGuidePanelToRight()
                pnlWizardEvent.Top = 34
                lblEventGuideInstructions.Text = "You are now ready to create the group flight post in Discord. Optionnaly, you can click this button to first put the files into your clipboard and receive instructions."
                SetFocusOnField(btnCopyReqFilesToClipboard, fromF1Key)

            Case 41 'Group flight post
                SetEventGuidePanelToRight()
                pnlWizardEvent.Top = 149
                lblEventGuideInstructions.Text = "Now click this button to copy the group flight's post content and receive instructions."
                SetFocusOnField(btnGroupFlightEventInfoToClipboard, fromF1Key)

            Case 42 'Discord Event
                If MsgBox("Do you have the access rights to create Discord Event on the target Discord Server? Click No if you don't know.", vbYesNo Or MsgBoxStyle.Question, "Discord Post Helper Wizard") = vbYes Then
                    intGuideCurrentStep = intGuideCurrentStep + 1
                Else
                    intGuideCurrentStep = 999
                End If
                ShowGuide()

            Case 43 'Group flight post
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 588
                lblEventGuideInstructions.Text = "From Discord, copy the link to the group flight post you just created above, and click ""Paste"" here."
                SetFocusOnField(btnDiscordGroupEventURL, fromF1Key)

            Case 44 'Create Discord Event
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 633
                lblEventGuideInstructions.Text = "In Discord and in the proper Discord Server, start the creation of a new Event (Create Event). If you don't know how to do this, ask for help!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 45 'Select voice channel for event
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 675
                lblEventGuideInstructions.Text = "On the new event window, under ""Where is your event"", choose ""Voice Channel"" and select this voice channel. Then click ""Next"" on the event window."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 46 'Topic name
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 716
                lblEventGuideInstructions.Text = "Click this button to copy the event topic and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventTopicClipboard, fromF1Key)

            Case 47 'Event date & time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 761
                lblEventGuideInstructions.Text = "On the Discord event window, specify the date and time displayed here - these are all local times you have to use!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 48 'Event description
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 801
                lblEventGuideInstructions.Text = "Click this button to copy the event description and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventDescriptionToClipboard, fromF1Key)

            Case 49 'Cover image
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 845
                lblEventGuideInstructions.Text = "In the Discord event window, you can also upload a cover image for your event. This is optional."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 50 'Cover image
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 881
                lblEventGuideInstructions.Text = "In the Discord event window, click Next to review your event information and publish it."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case Else
                intGuideCurrentStep = 0
                pnlGuide.Visible = False
                pnlWizardEvent.Visible = False
                btnTurnGuideOff.Visible = False
                MsgBox("The wizard's guidance ends here! If you hover your mouse on any field or button, you will also get a tooltip help displayed!", vbOKOnly Or vbInformation, "Discord Post Helper Wizard")
        End Select
    End Sub

    Private Sub SetFocusOnField(controlToPutFocus As Windows.Forms.Control, fromF1Key As Boolean)

        If Not fromF1Key Then
            controlToPutFocus.Focus()
        End If

    End Sub

    Private Sub SetGuidePanelToLeft()
        pnlWizardEvent.Visible = False
        Me.pnlArrow.BackgroundImage = Global.DiscordHelper.My.Resources.Resources.left_arrow
        pnlGuide.Left = 737
        pnlGuide.Visible = True
        pnlArrow.Left = -6
        pnlArrow.Top = 0
        btnGuideNext.Left = 674
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToTop()
        pnlWizardEvent.Visible = False
        Me.pnlArrow.BackgroundImage = Global.DiscordHelper.My.Resources.Resources.up_arrow
        pnlGuide.Top = 0
        pnlGuide.Visible = True
        pnlArrow.Left = -6
        pnlArrow.Top = 0
        btnGuideNext.Left = 674
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToRight()
        pnlWizardEvent.Visible = False
        Me.pnlArrow.BackgroundImage = Global.DiscordHelper.My.Resources.Resources.right_arrow
        pnlGuide.Left = 737
        pnlGuide.Visible = True
        pnlArrow.Left = 667
        pnlArrow.Top = 0
        btnGuideNext.Left = 3
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetEventGuidePanelToLeft()
        pnlGuide.Visible = False
        Me.pnlEventArrow.BackgroundImage = Global.DiscordHelper.My.Resources.Resources.left_arrow
        pnlWizardEvent.Left = 852
        pnlWizardEvent.Visible = True
        pnlEventArrow.Left = -6
        pnlEventArrow.Top = 0
        btnEventGuideNext.Left = 674
        btnEventGuideNext.Top = 3
    End Sub

    Private Sub SetEventGuidePanelToRight()
        pnlGuide.Visible = False
        Me.pnlEventArrow.BackgroundImage = Global.DiscordHelper.My.Resources.Resources.right_arrow
        pnlWizardEvent.Left = 740
        pnlWizardEvent.Visible = True
        pnlEventArrow.Left = 667
        pnlEventArrow.Top = 0
        btnEventGuideNext.Left = 3
        btnEventGuideNext.Top = 3
    End Sub

    Private Sub btnGuideNext_Click(sender As Object, e As EventArgs) Handles btnGuideNext.Click, btnEventGuideNext.Click

        intGuideCurrentStep = intGuideCurrentStep + 1
        ShowGuide()

    End Sub

    Private Sub btnTurnGuideOff_Click(sender As Object, e As EventArgs) Handles btnTurnGuideOff.Click

        intGuideCurrentStep = 0
        btnTurnGuideOff.Visible = False
        ShowGuide()

    End Sub
#End Region

    Private Sub UploadFile(ByVal folderName As String, ByVal fileName As String)

        Dim xmlString As String = xmldocFlightPlan.InnerXml

        Dim request As WebRequest = WebRequest.Create("https://siglr.com/DiscordPostHelper/SaveFlightPlanFileUnderTempFolder.php")
        request.Method = "POST"
        Dim postData As String = "xmlString=" + HttpUtility.UrlEncode(xmlString) + "&folderName=" + HttpUtility.UrlEncode(folderName) + "&fileName=" + HttpUtility.UrlEncode(fileName)
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

    Private Sub DeleteTempFile(ByVal fileName As String)

        Dim request As HttpWebRequest = CType(WebRequest.Create("https://siglr.com/DiscordPostHelper/DeleteTempFolder.php?folder=" & fileName), HttpWebRequest)
        request.Method = "GET"

        Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)

        Using reader As New IO.StreamReader(response.GetResponseStream())
            Dim result As String = reader.ReadToEnd()
            Console.WriteLine(result)
        End Using

    End Sub

    Private Function GenerateRandomFileName() As String
        Dim randomBytes(11) As Byte
        Using rng As New RNGCryptoServiceProvider()
            rng.GetBytes(randomBytes)
        End Using
        Return BitConverter.ToString(randomBytes).Replace("-", "")
    End Function

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown, TabControl1.KeyDown

        Dim controlTag As Integer = 0

        If e.KeyCode = Keys.F1 Then
            Try
                controlTag = CInt(Me.ActiveControl.Tag)
            Catch ex As Exception
                'Do nothing
            End Try

            If intGuideCurrentStep = CInt(Me.ActiveControl.Tag) Then
                intGuideCurrentStep = 0
            Else
                intGuideCurrentStep = CInt(Me.ActiveControl.Tag)
            End If
            ShowGuide(True)
        End If

    End Sub

    Private Sub btnExtraFileUp_Click(sender As Object, e As EventArgs) Handles btnExtraFileUp.Click

        MoveExtraFilesSelectedItems(-1)
        btnExtraFileUp.Focus()

    End Sub

    Private Sub btnExtraFileDown_Click(sender As Object, e As EventArgs) Handles btnExtraFileDown.Click

        MoveExtraFilesSelectedItems(1)
        btnExtraFileDown.Focus()

    End Sub

    Private Sub MoveExtraFilesSelectedItems(ByVal direction As Integer)
        Dim selectedIndices As List(Of Integer) = lstAllFiles.SelectedIndices.Cast(Of Integer).ToList()
        If selectedIndices.Count = 0 Then
            Return
        End If

        Dim minIndex As Integer = selectedIndices.Min()
        Dim maxIndex As Integer = selectedIndices.Max()

        If direction = -1 AndAlso minIndex > 0 Then
            For Each index As Integer In selectedIndices
                Dim item As Object = lstAllFiles.Items(index)
                lstAllFiles.Items.RemoveAt(index)
                lstAllFiles.Items.Insert(index - 1, item)
            Next
            lstAllFiles.ClearSelected()
            For Each index As Integer In selectedIndices
                lstAllFiles.SetSelected(index - 1, True)
            Next
        ElseIf direction = 1 AndAlso maxIndex < lstAllFiles.Items.Count - 1 Then
            For Each index As Integer In selectedIndices.OrderByDescending(Function(i) i)
                Dim item As Object = lstAllFiles.Items(index)
                lstAllFiles.Items.RemoveAt(index)
                lstAllFiles.Items.Insert(index + 1, item)
            Next
            lstAllFiles.ClearSelected()
            For Each index As Integer In selectedIndices
                lstAllFiles.SetSelected(index + 1, True)
            Next
        End If
    End Sub


    Private Sub lstAllFiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAllFiles.SelectedIndexChanged

        If lstAllFiles.SelectedIndex = -1 Then
            btnRemoveExtraFile.Enabled = False
            btnExtraFileDown.Enabled = False
            btnExtraFileUp.Enabled = False
        Else
            btnRemoveExtraFile.Enabled = True
            btnExtraFileDown.Enabled = True
            btnExtraFileUp.Enabled = True
        End If

    End Sub

    Private Sub chkGroupSecondaryPosts_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupSecondaryPosts.CheckedChanged

        SetVisibilityForSecPosts()

    End Sub

    Private Sub SetVisibilityForSecPosts()

        If chkGroupSecondaryPosts.Checked Then
            btnCopyAllSecPosts.Visible = True
            btnAltRestricCopy.Visible = False
            btnFilesTextCopy.Visible = False
            btnFullDescriptionCopy.Visible = False
            btnFilesCopy.Text = "3. Files to clipboard"
        Else
            btnCopyAllSecPosts.Visible = False
            btnAltRestricCopy.Visible = True
            btnFilesTextCopy.Visible = True
            btnFullDescriptionCopy.Visible = True
            btnFilesCopy.Text = "3a. Files to clipboard"
        End If

    End Sub

    Private Sub btnCopyAllSecPosts_Click(sender As Object, e As EventArgs) Handles btnCopyAllSecPosts.Click

        Clipboard.SetText(txtAltRestrictions.Text & vbCrLf & vbCrLf &
                          txtWeatherFirstPart.Text & vbCrLf & vbCrLf &
                          txtWeatherWinds.Text & vbCrLf & vbCrLf &
                          txtWeatherClouds.Text & vbCrLf & vbCrLf &
                          txtFullDescriptionResults.Text & vbCrLf & vbCrLf &
                          txtFilesText.Text & vbCrLf)
        MsgBox("Now paste the content as the second message in the thread!", vbOKOnly Or MsgBoxStyle.Information, "Step 2 - Creating secondary post in the thread.")
        If intGuideCurrentStep <> 0 Then
            intGuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub
End Class



