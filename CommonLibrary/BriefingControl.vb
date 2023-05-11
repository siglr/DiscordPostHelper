Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml

Public Class BriefingControl

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _SF As SupportingFeatures
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private _sessionData As AllData
    Private _unpackFolder As String = String.Empty
    Public Property EventIsEnabled As Boolean

    Public Sub FullReset()
        txtBriefing.Clear()
        imageViewer.ClearImage()
        imagesTabViewerControl.ClearImage()
        txtFullDescription.Clear()
        restrictionsDataGrid.DataSource = Nothing
        waypointCoordinatesDataGrid.DataSource = Nothing
        cboWayPointDistances.SelectedIndex = 0

        If imagesListView.LargeImageList IsNot Nothing Then
            imagesListView.LargeImageList.Dispose()
            imagesListView.LargeImageList = Nothing
        End If
        imagesListView.Items.Clear()
        imagesListView.Clear()

        EventIsEnabled = False
        windLayersFlowLayoutPnl.Controls.Clear()

        CountDownReset()

        GC.Collect()

    End Sub

    Private Sub CountDownReset()
        Timer1.Stop()
        countDownToMeet.ZoomFactor = 2
        countDownToMeet.ResetToZero(True)
        countDownToSyncFly.ZoomFactor = 2
        countDownToSyncFly.ResetToZero(True)
        countDownToLaunch.ZoomFactor = 2
        countDownToLaunch.ResetToZero(True)
        countDownTaskStart.ZoomFactor = 2
        countDownTaskStart.ResetToZero(True)
        lblInsideOutside60Minutes.Text = String.Empty
        msfsLocalDateToSet.Text = String.Empty
        msfsLocalTimeToSet.Text = String.Empty

    End Sub

    Public Sub ChangeImage(imgFilename As String)
        imageViewer.LoadImage(imgFilename)
    End Sub

    Public Sub GenerateBriefing(supportFeat As SupportingFeatures,
                                sessionData As AllData,
                                flightplanfile As String,
                                weatherfile As String,
                                Optional unpackFolder As String = "NONE")

        _SF = supportFeat
        _sessionData = sessionData
        If unpackFolder = "NONE" Then
            _unpackFolder = String.Empty
        Else
            _unpackFolder = unpackFolder
        End If

        EventIsEnabled = sessionData.EventEnabled

        'Load flight plan
        _XmlDocFlightPlan = New XmlDocument
        _XmlDocFlightPlan.Load(flightplanfile)
        Dim totalDistance As Integer
        Dim trackDistance As Integer
        Dim altitudeRestrictions As String = _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, trackDistance, False)

        'Load weather info
        _XmlDocWeatherPreset = New XmlDocument
        _XmlDocWeatherPreset.Load(weatherfile)
        _WeatherDetails = Nothing
        _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        Dim dateFormat As String
        If sessionData.IncludeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder
        sb.Append("{\rtf1\ansi ")

        'Title
        sb.Append($"\b {sessionData.Title}\b0\line ")
        sb.Append($"{sessionData.Credits}\line ")
        sb.Append("\line ")

        'Credits

        'Local MSFS date and time 
        sb.Append($"MSFS Local date & time is \b {sessionData.SimLocalDateTime.ToString(dateFormat, _EnglishCulture)}, {sessionData.SimLocalDateTime.ToString("hh:mm tt", _EnglishCulture)} {_SF.ValueToAppendIfNotEmpty(sessionData.SimDateTimeExtraInfo.Trim, True, True)}\b0\line ")

        'Flight plan
        sb.Append($"The flight plan to load is \b {Path.GetFileName(sessionData.FlightPlanFilename)}\b0\line ")

        sb.Append("\line ")

        'Departure airfield And runway
        sb.Append($"You will depart from \b {_SF.ValueToAppendIfNotEmpty(sessionData.DepartureICAO)}{_SF.ValueToAppendIfNotEmpty(sessionData.DepartureName, True)}{_SF.ValueToAppendIfNotEmpty(sessionData.DepartureExtra, True, True)}\b0\line ")

        'Arrival airfield And expected runway
        sb.Append($"You will land at \b {_SF.ValueToAppendIfNotEmpty(sessionData.ArrivalICAO)}{_SF.ValueToAppendIfNotEmpty(sessionData.ArrivalName, True)}{_SF.ValueToAppendIfNotEmpty(sessionData.ArrivalExtra, True, True)}\b0\line ")

        'Type of soaring
        Dim soaringType As String = GetSoaringTypesSelected()
        If soaringType.Trim <> String.Empty OrElse sessionData.SoaringExtraInfo <> String.Empty Then
            sb.Append($"Soaring Type is \b {soaringType}{_SF.ValueToAppendIfNotEmpty(sessionData.SoaringExtraInfo, True, True)}\b0\line ")
        End If

        'Task distance And total distance
        sb.Append($"Distances are \b {_SF.GetDistance(totalDistance.ToString, trackDistance.ToString)}\b0\line ")

        'Approx. duration
        sb.Append($"Approx. duration should be \b {_SF.GetDuration(sessionData.DurationMin, sessionData.DurationMax)}{_SF.ValueToAppendIfNotEmpty(sessionData.DurationExtraInfo, True, True)}\b0\line ")

        'Recommended gliders
        If sessionData.RecommendedGliders.Trim <> String.Empty Then
            sb.Append($"Recommended gliders: \b {_SF.ValueToAppendIfNotEmpty(sessionData.RecommendedGliders)}\b0\line ")
        End If

        'Difficulty rating
        If sessionData.DifficultyRating.Trim <> String.Empty OrElse sessionData.DifficultyExtraInfo.Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as \b {_SF.GetDifficulty(CInt(sessionData.DifficultyRating.Substring(0, 1)), sessionData.DifficultyExtraInfo, True)}\b0\line ")
        End If

        sb.Append("\line ")

        If _WeatherDetails IsNot Nothing Then
            'Weather info (temperature, baro pressure, precipitations)
            sb.Append($"The weather profile to load is \b {_WeatherDetails.PresetName}\b0\line ")
            If sessionData.WeatherSummary <> String.Empty Then
                sb.Append($"Weather summary: \b {_SF.ValueToAppendIfNotEmpty(sessionData.WeatherSummary)}\b0\line ")
            End If
            If _WeatherDetails.AltitudeMeasurement = "AGL" Then
                sb.Append($"The elevation measurement used is \b AGL (Above Ground Level)\b0\line ")
            Else
                sb.Append($"The elevation measurement used is \b AMSL (Above Mean Sea Level)\b0\line ")
            End If
            sb.Append($"The barometric pressure is \b {_WeatherDetails.MSLPressure}\b0\line ")
            sb.Append($"The temperature is \b {_WeatherDetails.MSLTemperature}\b0\line ")
            sb.Append($"The humidity index is \b {_WeatherDetails.Humidity}\b0\line ")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"Precipitations: \b {_WeatherDetails.Precipitations}\b0\line ")
            End If

            sb.Append("\line ")

            'Winds
            sb.Append($"\b Wind Layers\b0\line ")
            Dim lines As String() = _WeatherDetails.WindLayersAsString.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}\line ")
            Next

            sb.Append(" \line ")

            'Clouds
            sb.Append($"\b Cloud Layers\b0\line ")
            lines = _WeatherDetails.CloudLayers.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}\line ")
            Next

            ' Create and add wind layer controls to flow layout panel
            Dim windLayer As WindLayer
            For i As Integer = _WeatherDetails.WindLayers.Count - 1 To 0 Step -1
                windLayer = _WeatherDetails.WindLayers(i)
                If windLayer.IsValidWindLayer Then
                    Dim windLayerControl As New WindLayerControl(windLayer)
                    windLayersFlowLayoutPnl.Controls.Add(windLayerControl)
                End If
            Next

        End If
        sb.Append("}")
        txtBriefing.Rtf = sb.ToString()
        SupportingFeatures.SetZoomFactorOfRichTextBox(txtBriefing)

        sb.Clear()

        imageViewer.Visible = True
        If sessionData.MapImageSelected = String.Empty Then
            imageViewer.Enabled = False
        Else
            Dim filename As String = sessionData.MapImageSelected
            If _unpackFolder <> String.Empty Then
                filename = Path.Combine(_unpackFolder, Path.GetFileName(filename))
            End If
            imageViewer.LoadImage(filename)
        End If


        'Build full description
        If sessionData.LongDescription <> String.Empty Then
            sb.AppendLine("**Full Description**")
            sb.AppendLine(sessionData.LongDescription.Replace("($*$)", Environment.NewLine))
            _SF.FormatMarkdownToRTF(sb.ToString.Trim, txtFullDescription)

        End If

        'Build altitude restrictions
        Dim dt As New DataTable()
        dt.Columns.Add("Waypoint Name", GetType(String))
        dt.Columns.Add("Restrictions", GetType(String))
        For Each waypoint In _SF.AllWaypoints.Where(Function(x) x.ContainsRestriction)
            dt.Rows.Add(waypoint.WaypointName, waypoint.Restrictions)
        Next
        restrictionsDataGrid.DataSource = dt
        restrictionsDataGrid.Font = New Font(restrictionsDataGrid.Font.FontFamily, 12)
        restrictionsDataGrid.RowTemplate.Height = 28
        restrictionsDataGrid.RowHeadersVisible = False
        restrictionsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        'XBOX Coordinates
        waypointCoordinatesDataGrid.Font = New Font(waypointCoordinatesDataGrid.Font.FontFamily, 12)
        waypointCoordinatesDataGrid.RowTemplate.Height = 30
        waypointCoordinatesDataGrid.DataSource = _SF.AllWaypoints
        waypointCoordinatesDataGrid.Columns("Latitude").DefaultCellStyle.Format = "N6"
        waypointCoordinatesDataGrid.Columns("Longitude").DefaultCellStyle.Format = "N6"
        waypointCoordinatesDataGrid.ColumnHeadersDefaultCellStyle.Font = New Font(waypointCoordinatesDataGrid.Font, FontStyle.Bold)
        waypointCoordinatesDataGrid.Columns(0).HeaderText = "#"
        For i As Integer = 0 To waypointCoordinatesDataGrid.Columns.Count - 2
            waypointCoordinatesDataGrid.Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        Next
        waypointCoordinatesDataGrid.RowHeadersWidth = 15
        'waypointCoordinatesDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        waypointCoordinatesDataGrid.AllowUserToResizeColumns = True
        waypointCoordinatesDataGrid.Columns("FullATCId").Visible = False
        waypointCoordinatesDataGrid.Columns("ContainsRestriction").Visible = False
        waypointCoordinatesDataGrid.Columns("Gate").HeaderText = "Gate diameter"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").HeaderText = "Previous (km)"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").HeaderText = "Total (km)"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").HeaderText = "Task (km)"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").HeaderText = "Previous (mi)"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").HeaderText = "Total (mi)"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").HeaderText = "Task (mi)"
        SetWPGridColumnsVisibility()

        'Images tab
        LoadImagesTabListView()

        'EventInfo tab
        BuildEventInfoTab()

    End Sub

    Private Sub BuildEventInfoTab()

        Dim sb As New StringBuilder
        sb.Append("{\rtf1\ansi ")

        If Not EventIsEnabled Then
            sb.Append($"There is currently no group flight or event defined with this task. \line ")
            CountDownReset()
        Else
            'Group/Club Name
            sb.Append($"Group or Club: \b {_sessionData.GroupClub}\b0\line ")

            Dim fullMeetDateTimeLocal As DateTime = _sessionData.MeetLocalDateTime
            Dim fullSyncFlyDateTimeLocal As DateTime = _sessionData.SyncFlyLocalDateTime
            Dim fullLaunchDateTimeLocal As DateTime = _sessionData.LaunchLocalDateTime
            Dim fullStartTaskDateTimeLocal As DateTime = _sessionData.StartTaskLocalDateTime
            Dim fullMSFSLocalDateTime As DateTime = _sessionData.SimLocalDateTime

            countDownToMeet.SetTargetDateTime(fullMeetDateTimeLocal)

            'Timezone
            Dim timezoneInfos As List(Of String) = SupportingFeatures.GetTimeZoneInformation
            sb.Append($"The local times displayed here are for the timezone: \b {timezoneInfos(0)} (UTC{timezoneInfos(1)})\b0\line ")
            sb.Append("\line ")

            'Date
            sb.Append($"Event Date: \b {fullMeetDateTimeLocal.ToString("dddd, MMMM d, yyyy", CultureInfo.CurrentCulture)}\b0\line ")
            sb.Append("\line ")

            'Meeting Time and Discord Voice Channel
            If _sessionData.VoiceChannel <> String.Empty Then
                sb.Append($"We meet at: \b {fullMeetDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu) ")
                sb.Append($"on voice channel: \b {_sessionData.VoiceChannel}\b0\line ")
                sb.Append("\line ")
            End If

            'MSFS Server
            If _sessionData.MSFSServer > 0 Then
                sb.Append($"Set MSFS Server to: \b {_SF.GetMSFSServers.ElementAt(_sessionData.MSFSServer)}\b0\line ")
                sb.Append("\line ")
            End If

            'Weather ad flight plan
            sb.Append($"Load weather preset: \b {_WeatherDetails.PresetName}\b0\line ")
            sb.Append($"And flight plan: \b ""{Path.GetFileName(_sessionData.FlightPlanFilename)}""\b0\line ")
            sb.Append("\line ")

            Dim dateFormat As String
            If _sessionData.IncludeYear Then
                dateFormat = "MMMM dd, yyyy"
            Else
                dateFormat = "MMMM dd"
            End If

            sb.Append($"The MSFS Local Date & Time should be: \b {fullMSFSLocalDateTime.ToString(dateFormat, CultureInfo.CurrentCulture)}, {fullMSFSLocalDateTime.ToString("t", CultureInfo.CurrentCulture)} {_SF.ValueToAppendIfNotEmpty(_sessionData.SimDateTimeExtraInfo, True, True)}\b0 ")
            If _sessionData.UseEventSyncFly Then
                sb.Append("when it's the Sync Fly time below. \line ")
            ElseIf _sessionData.UseEventLaunch Then
                sb.Append("when it's the Launch time below \line ")
            Else
                sb.Append("when it's the Meet time above \line ")
            End If
            sb.Append("\line ")

            'Sync Start or not?
            If _sessionData.UseEventSyncFly Then
                sb.Append($"This task requires a \b SYNC FLY \b0 so \b WAIT \b0 on the World Map for the signal. \line ")
                sb.Append($"Sync Fly expected at \b {fullSyncFlyDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullSyncFlyDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu) \line ")
                sb.Append("\line ")
                countDownToSyncFly.SetTargetDateTime(fullSyncFlyDateTimeLocal)
            Else
                sb.Append($"This task DOES NOT require a SYNC FLY so you can click Fly at your convenience and wait at the airfield. \line ")
                sb.Append("\line ")
                countDownToSyncFly.ResetToZero(True)
            End If

            'Launch
            If _sessionData.UseEventLaunch Then
                sb.Append($"Launch/Winch/Tow signal expected at \b {fullLaunchDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullLaunchDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu) \line ")
                sb.Append("\line ")
                countDownToLaunch.SetTargetDateTime(fullLaunchDateTimeLocal)
            Else
                sb.Append($"Once at the airfield, launch at your convenience and wait for the task start signal. \line ")
                sb.Append("\line ")
                countDownToLaunch.ResetToZero(True)
            End If

            'Start task
            If _sessionData.UseEventStartTask Then
                sb.Append($"Task start/Start gate opening signal expected at \b {fullStartTaskDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullStartTaskDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu) \line ")
                sb.Append("\line ")
                countDownTaskStart.SetTargetDateTime(fullStartTaskDateTimeLocal)
            Else
                sb.Append($"There is no specific task start time, you can cross the start gate at your convenience. \line ")
                sb.Append("\line ")
                countDownTaskStart.ResetToZero(True)
            End If
            sb.Append($"The expected duration should be \b {_SF.GetDuration(_sessionData.DurationMin, _sessionData.DurationMax)}{_SF.ValueToAppendIfNotEmpty(_sessionData.DurationExtraInfo, True, True)}\b0\line ")
            sb.Append("\line ")

            sb.Append($"See Main Task Info and Map tabs for other important task information. \line ")

            sb.Append("}")
            Timer1.Start()
        End If

        txtEventInfo.Rtf = sb.ToString()
        SupportingFeatures.SetZoomFactorOfRichTextBox(txtEventInfo)

    End Sub

    Private Sub LoadImagesTabListView()

        Dim imageFilenameList As New List(Of String)

        Dim imgList As New ImageList
        For Each filename As String In _sessionData.ExtraFiles
            If Path.GetExtension(filename.ToLower) = ".png" OrElse Path.GetExtension(filename.ToLower) = ".jpg" Then
                If _unpackFolder <> String.Empty Then
                    filename = Path.Combine(_unpackFolder, Path.GetFileName(filename))
                End If
                imgList.Images.Add(Image.FromFile(filename))
                imageFilenameList.Add(filename)
            End If
        Next
        imgList.ImageSize = New Size(64, 64) 'set the size of the icons

        imagesListView.View = View.LargeIcon
        imagesListView.LargeImageList = imgList

        For i = 0 To imgList.Images.Count - 1
            Dim item1 As New ListViewItem($"")
            item1.ImageIndex = i
            item1.Text = i + 1.ToString
            item1.Tag = imageFilenameList(i)
            imagesListView.Items.Add(item1)
        Next

        If imagesListView.Items.Count > 0 Then
            imagesListView.Items(0).Selected = True
        End If

    End Sub

    Private Function GetSoaringTypesSelected() As String
        Dim types As String = String.Empty

        If _sessionData.SoaringRidge And _sessionData.SoaringThermals Then
            types = "Ridge and Thermals"
        ElseIf _sessionData.SoaringRidge Then
            types = "Ridge"
        ElseIf _sessionData.SoaringThermals Then
            types = "Thermals"
        End If

        Return types

    End Function

    Private Sub mapPictureBox_DoubleClick(sender As Object, e As EventArgs)

        'Launch the ImageViewer program
        Dim startInfo As New ProcessStartInfo($"{Application.StartupPath}\ImageViewer.exe", $"""{_sessionData.MapImageSelected}""")

        Process.Start(startInfo)


    End Sub

    Private Sub tabsBriefing_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabsBriefing.SelectedIndexChanged

        AdjustRTBoxControls()

    End Sub

    Public Sub AdjustRTBoxControls()

        If _SF IsNot Nothing Then
            Select Case tabsBriefing.SelectedIndex
                Case 0
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtBriefing)
                Case 1
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtFullDescription)
                Case 2
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtEventInfo)
            End Select
        End If

    End Sub

    Private Sub mapSplitterLeftRight_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles mapSplitterLeftRight.SplitterMoved, mapSplitterUpDown.SplitterMoved
        AdjustRTBoxControls()
    End Sub

    Private Sub imagesListView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles imagesListView.SelectedIndexChanged


        If imagesListView.SelectedItems.Count > 0 AndAlso imagesListView.SelectedItems(0).Tag <> String.Empty Then
            imagesTabViewerControl.LoadImage(imagesListView.SelectedItems(0).Tag)
        End If

    End Sub

    Private Sub cboWayPointDistances_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboWayPointDistances.SelectedIndexChanged
        SetWPGridColumnsVisibility()
    End Sub

    Private Sub SetWPGridColumnsVisibility()

        Try
            'Distances
            Select Case cboWayPointDistances.SelectedIndex
                Case 0 'None
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").Visible = False
                Case 1 'Kilometers
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").Visible = False
                Case 2 'Miles
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").Visible = True

            End Select

            'Lat and Lon columns
            If chkWPEnableLatLonColumns.Checked Then
                waypointCoordinatesDataGrid.Columns("Latitude").Visible = True
                waypointCoordinatesDataGrid.Columns("Longitude").Visible = True
            Else
                waypointCoordinatesDataGrid.Columns("Latitude").Visible = False
                waypointCoordinatesDataGrid.Columns("Longitude").Visible = False
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub chkWPEnableLatLonColumns_CheckedChanged(sender As Object, e As EventArgs) Handles chkWPEnableLatLonColumns.CheckedChanged
        SetWPGridColumnsVisibility()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        countDownToMeet.UpdateTime()
        If _sessionData.UseEventSyncFly Then
            countDownToSyncFly.UpdateTime()
        End If
        If _sessionData.UseEventLaunch Then
            countDownToLaunch.UpdateTime()
        End If
        If _sessionData.UseEventStartTask Then
            countDownTaskStart.UpdateTime()
        End If
        AdjustMSFSLocalDateTime()

    End Sub

    Private Sub AdjustMSFSLocalDateTime()

        If Not _sessionData.EventEnabled Then
            lblInsideOutside60Minutes.Text = String.Empty
            msfsLocalDateToSet.Text = String.Empty
            msfsLocalTimeToSet.Text = String.Empty
            Exit Sub
        End If

        Dim timePassedEvent As TimeSpan
        Dim msfsDateToDisplay As Date = _sessionData.SimLocalDateTime
        Dim withinEvent60Min As Boolean = False

        If _sessionData.UseEventSyncFly Then
            'MSFS Local Date and Time should match with the Sync Fly time (only within 60 minutes before or after the SyncFly date)
            If countDownToSyncFly.RemainingTime > 0 AndAlso countDownToSyncFly.RemainingTime <= 3600 Then
                msfsDateToDisplay = msfsDateToDisplay.AddSeconds(countDownToSyncFly.RemainingTime * -1)
                withinEvent60Min = True
            Else
                timePassedEvent = DateTime.Now - _sessionData.SyncFlyLocalDateTime
                If timePassedEvent.TotalMinutes >= 0 AndAlso timePassedEvent.TotalMinutes < 61 Then
                    msfsDateToDisplay += timePassedEvent
                    withinEvent60Min = True
                End If
            End If
        ElseIf _sessionData.UseEventLaunch Then
            'MSFS Local Date and Time should match with the launch time (only within 60 minutes before or after the Launch date)
            If countDownToLaunch.RemainingTime > 0 AndAlso countDownToLaunch.RemainingTime <= 3600 Then
                msfsDateToDisplay = msfsDateToDisplay.AddSeconds(countDownToLaunch.RemainingTime * -1)
                withinEvent60Min = True
            Else
                timePassedEvent = DateTime.Now - _sessionData.LaunchLocalDateTime
                If timePassedEvent.TotalMinutes >= 0 AndAlso timePassedEvent.TotalMinutes < 61 Then
                    msfsDateToDisplay += timePassedEvent
                    withinEvent60Min = True
                End If
            End If
        Else
            'MSFS Local Date and Time should match with the meet time (only within 60 minutes before or after the Meet date)
            If countDownToMeet.RemainingTime > 0 AndAlso countDownToMeet.RemainingTime <= 3600 Then
                msfsDateToDisplay = msfsDateToDisplay.AddSeconds(countDownToMeet.RemainingTime * -1)
                withinEvent60Min = True
            Else
                timePassedEvent = DateTime.Now - _sessionData.MeetLocalDateTime
                If timePassedEvent.TotalMinutes >= 0 AndAlso timePassedEvent.TotalMinutes < 61 Then
                    If timePassedEvent.TotalMinutes < 61 Then
                        msfsDateToDisplay += timePassedEvent
                        withinEvent60Min = True
                    End If
                End If
            End If
        End If

        If withinEvent60Min = True Then
            'Set description label for within 60 minutes
            lblInsideOutside60Minutes.Text = $"Within 60 minutes of the event's time.{Environment.NewLine}{Environment.NewLine}If clicking Fly now, MSFS local time should be:"
        Else
            'Set description label for outside 60 minutes
            lblInsideOutside60Minutes.Text = $"This is a past or future event.{Environment.NewLine}{Environment.NewLine}MSFS local time{Environment.NewLine}should be:"
        End If

        'Set date and time
        If _sessionData.IncludeYear Then
            msfsLocalDateToSet.Text = msfsDateToDisplay.ToString("MMMM d, yyyy", CultureInfo.CurrentCulture)
        Else
            msfsLocalDateToSet.Text = msfsDateToDisplay.ToString("MMMM d", CultureInfo.CurrentCulture)
        End If
        msfsLocalTimeToSet.Text = msfsDateToDisplay.ToString("t", CultureInfo.CurrentCulture)

    End Sub
End Class
