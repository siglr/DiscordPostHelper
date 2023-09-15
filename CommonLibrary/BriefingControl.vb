Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

Public Class BriefingControl

#Region "Constants and global members"

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _SF As SupportingFeatures
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private _sessionData As AllData
    Private _unpackFolder As String = String.Empty
    Private _initPrefUnits As Boolean = False
    Private _onUnitsTab As Boolean = False
    Private _loaded As Boolean = False

    Public Property EventIsEnabled As Boolean
    Public Property PrefUnits As New PreferredUnits

#End Region

#Region "Form events handlers"

    Private Sub BriefingControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetPrefUnits()
    End Sub

    Private Sub tabsBriefing_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabsBriefing.SelectedIndexChanged
        AdjustRTBoxControls()
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

    Private Sub AddOnsDataGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles AddOnsDataGrid.CellDoubleClick
        ' Check if a valid row index and URL column
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = 3 Then
            ' Get the URL value from the clicked row
            Dim url As String = AddOnsDataGrid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString()
            ' Use the URL as needed (e.g., open in browser)
            Try
                Process.Start(url)
            Catch ex As Exception
                MessageBox.Show("An error occured trying to open the specified URL!", "Trying to open URL", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub unitRadioBox_CheckedChanged(sender As Object, e As EventArgs) Handles radioDistanceMetric.CheckedChanged,
                                                                                            radioDistanceImperial.CheckedChanged,
                                                                                            radioDistanceBoth.CheckedChanged,
                                                                                            radioBaroInHg.CheckedChanged,
                                                                                            radioBaroHPa.CheckedChanged,
                                                                                            radioBaroBoth.CheckedChanged,
                                                                                            radioAltitudeMeters.CheckedChanged,
                                                                                            radioAltitudeFeet.CheckedChanged,
                                                                                            radioAltitudeBoth.CheckedChanged,
                                                                                            radioWindSpeedMps.CheckedChanged,
                                                                                            radioWindSpeedKnots.CheckedChanged,
                                                                                            radioWindSpeedBoth.CheckedChanged,
                                                                                            radioTemperatureFarenheit.CheckedChanged,
                                                                                            radioTemperatureCelsius.CheckedChanged,
                                                                                            radioTemperatureBoth.CheckedChanged,
                                                                                            radioGateDiameterMetric.CheckedChanged,
                                                                                            radioGateDiameterImperial.CheckedChanged,
                                                                                            radioGateDiameterBoth.CheckedChanged

        UnitPrefChanged(sender)

    End Sub

    Private Sub trackAudioCueVolume_ValueChanged(sender As Object, e As EventArgs) Handles trackAudioCueVolume.ValueChanged

        If _loaded Then

            Dim playCue As Boolean = True
            If trackAudioCueVolume.Value = 0 Then
                playCue = False
            End If
            countDownToMeet.PlayAudioCues = playCue
            countDownToSyncFly.PlayAudioCues = playCue
            countDownToLaunch.PlayAudioCues = playCue
            countDownTaskStart.PlayAudioCues = playCue

            countDownToMeet.SetOutputVolume(trackAudioCueVolume.Value)

            SupportingFeatures.WriteRegistryKey("AudioCues", trackAudioCueVolume.Value)
        End If

    End Sub

    Private Sub btnTestAudioCueVolume_Click(sender As Object, e As EventArgs) Handles btnTestAudioCueVolume.Click

        countDownToMeet.SetOutputVolume(trackAudioCueVolume.Value)
        countDownToMeet.TestAudioCueVolume("MeetingHasStarted")

    End Sub

#End Region

#Region "Subs and functions"

#Region "Public"

    Public Sub SetPrefUnits()

        _initPrefUnits = True

        Select Case PrefUnits.Altitude
            Case AltitudeUnits.Metric
                radioAltitudeMeters.Checked = True
            Case AltitudeUnits.Imperial
                radioAltitudeFeet.Checked = True
            Case AltitudeUnits.Both
                radioAltitudeBoth.Checked = True
        End Select

        Select Case PrefUnits.Distance
            Case DistanceUnits.Metric
                radioDistanceMetric.Checked = True
            Case DistanceUnits.Imperial
                radioDistanceImperial.Checked = True
            Case DistanceUnits.Both
                radioDistanceBoth.Checked = True
        End Select

        Select Case PrefUnits.GateDiameter
            Case GateDiameterUnits.Metric
                radioGateDiameterMetric.Checked = True
            Case GateDiameterUnits.Imperial
                radioGateDiameterImperial.Checked = True
            Case GateDiameterUnits.Both
                radioGateDiameterBoth.Checked = True
        End Select

        Select Case PrefUnits.WindSpeed
            Case WindSpeedUnits.MeterPerSecond
                radioWindSpeedMps.Checked = True
            Case WindSpeedUnits.Knots
                radioWindSpeedKnots.Checked = True
            Case WindSpeedUnits.Both
                radioWindSpeedBoth.Checked = True
        End Select

        Select Case PrefUnits.Barometric
            Case BarometricUnits.hPa
                radioBaroHPa.Checked = True
            Case BarometricUnits.inHg
                radioBaroInHg.Checked = True
            Case BarometricUnits.Both
                radioBaroBoth.Checked = True
        End Select

        Select Case PrefUnits.Temperature
            Case TemperatureUnits.Celsius
                radioTemperatureCelsius.Checked = True
            Case TemperatureUnits.Fahrenheit
                radioTemperatureFarenheit.Checked = True
            Case TemperatureUnits.Both
                radioTemperatureBoth.Checked = True
        End Select

        _initPrefUnits = False

    End Sub

    Public Sub FullReset()
        txtBriefing.Clear()
        imageViewer.ClearImage()
        imagesTabViewerControl.ClearImage()
        txtFullDescription.Clear()
        restrictionsDataGrid.DataSource = Nothing
        waypointCoordinatesDataGrid.DataSource = Nothing
        windLayersDatagrid.DataSource = Nothing
        cloudLayersDatagrid.DataSource = Nothing
        cboWayPointDistances.SelectedIndex = 0
        AddOnsDataGrid.DataSource = Nothing

        If imagesListView.LargeImageList IsNot Nothing Then
            imagesListView.LargeImageList.Dispose()
            imagesListView.LargeImageList = Nothing
        End If
        imagesListView.Items.Clear()
        imagesListView.Clear()

        ClearCountryFlagPictures()

        EventIsEnabled = False
        windLayersFlowLayoutPnl.Controls.Clear()

        CountDownReset()

        GC.Collect()

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
        SupportingFeatures.PrefUnits = PrefUnits

        _sessionData = sessionData
        If unpackFolder = "NONE" Then
            _unpackFolder = String.Empty
            lblPrefUnitsMessage.Text = $"Units selected here are only used for YOUR briefing tabs.{Environment.NewLine}They DO NOT change the content of generated Discord posts which always include all formats.{Environment.NewLine}Also, any data specified in description fields is excluded and will appear as is."
        Else
            _unpackFolder = unpackFolder
        End If

        'Load flight plan
        _XmlDocFlightPlan = New XmlDocument
        _XmlDocFlightPlan.Load(flightplanfile)

        'Load weather info
        _XmlDocWeatherPreset = New XmlDocument
        _XmlDocWeatherPreset.Load(weatherfile)
        _WeatherDetails = Nothing
        _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        _SF.DownloadCountryFlags(_sessionData.Countries)
        BuildTaskData()
        BuildCloudAndWindLayersDatagrids()
        AddCountryFlagPictures()

        If _sessionData.DiscordTaskThreadURL = String.Empty OrElse SupportingFeatures.IsValidURL(_sessionData.DiscordTaskThreadURL) = False Then
            btnGotoDiscordTaskThread.Enabled = False
        Else
            btnGotoDiscordTaskThread.Enabled = True
        End If

    End Sub

    Public Sub AdjustRTBoxControls()

        'Check if we were on the Units tab - we may need to regenerate
        If _onUnitsTab Then
            FullReset()
            BuildTaskData()
            BuildEventInfoTab()
            BuildCloudAndWindLayersDatagrids()
            AddCountryFlagPictures()
            _onUnitsTab = False
        End If

        If _SF IsNot Nothing Then
            Select Case tabsBriefing.SelectedIndex
                Case 0 'Main Task Info
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtBriefing)
                Case 1 'Map
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtFullDescription)
                Case 2 'Event Info
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtEventInfo)
                Case 3 'Images
                Case 4 'All Waypoints
                Case 5 'Weather
                Case 6 'Add-ons
                Case 7 'Units
                    _onUnitsTab = True
            End Select
        End If

    End Sub

    Public ReadOnly Property WeatherProfileInnerXML As String
        Get
            If _XmlDocWeatherPreset IsNot Nothing Then
                Return _XmlDocWeatherPreset.InnerXml
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public ReadOnly Property FlightPlanInnerXML As String
        Get
            If _XmlDocFlightPlan IsNot Nothing Then
                Return _XmlDocFlightPlan.InnerXml
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private Sub AddCountryFlagPictures()
        Dim flagsDirectory As String = SupportingFeatures.ReadRegistryKey("CountryFlagsFolder", String.Empty)

        For Each countryName As String In _sessionData.Countries
            If countryName <> String.Empty AndAlso _SF.CountryFlagCodes.ContainsKey(countryName) Then
                Dim countryCode As String = _SF.CountryFlagCodes(countryName)
                Dim flagFileName As String = countryCode.Substring(6, 2) & ".png"
                Dim flagFilePath As String = Path.Combine(flagsDirectory, flagFileName)

                If File.Exists(flagFilePath) Then
                    Dim pictureBox As New PictureBox()
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage
                    pictureBox.Width = 56
                    pictureBox.Height = 42
                    pictureBox.Image = Image.FromFile(flagFilePath)

                    ToolTip1.SetToolTip(pictureBox, countryName)

                    countryFlagsLayoutPanel.Controls.Add(pictureBox)
                End If
            End If
        Next
    End Sub

    Public Sub ClearCountryFlagPictures()
        For Each control As Control In countryFlagsLayoutPanel.Controls
            If TypeOf control Is PictureBox Then
                Dim pictureBox As PictureBox = DirectCast(control, PictureBox)
                pictureBox.Image?.Dispose() ' Dispose the image resource
                pictureBox.Image = Nothing ' Clear the image property
                pictureBox.Dispose() ' Dispose the PictureBox control itself
            End If
        Next

        countryFlagsLayoutPanel.Controls.Clear() ' Clear all controls from the CountryFlagsPanel
    End Sub


#End Region

#Region "Private"

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

    Private Sub BuildTaskData()
        Dim totalDistance As Integer
        Dim trackDistance As Integer

        EventIsEnabled = _sessionData.EventEnabled

        _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, trackDistance, False)


        Dim dateFormat As String
        If _sessionData.IncludeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder
        sb.Append("{\rtf1\ansi ")

        'Title
        sb.Append($"\b {_sessionData.Title}\b0\line ")
        sb.Append($"{_sessionData.MainAreaPOI}\line ")
        sb.Append("\line ")
        sb.Append($"{_sessionData.ShortDescription}\line ")
        sb.Append("\line ")
        sb.Append($"{_sessionData.Credits}\line ")
        sb.Append("\line ")

        'Credits

        'Local MSFS date and time 
        sb.Append($"MSFS Local date & time is \b {_sessionData.SimLocalDateTime.ToString(dateFormat, _EnglishCulture)}, {_sessionData.SimLocalDateTime.ToString("hh:mm tt", _EnglishCulture)} {_SF.ValueToAppendIfNotEmpty(_sessionData.SimDateTimeExtraInfo.Trim, True, True)}\b0\line ")

        'Flight plan
        sb.Append($"The flight plan to load is \b {Path.GetFileName(_sessionData.FlightPlanFilename)}\b0\line ")

        sb.Append("\line ")

        'Departure airfield And runway
        sb.Append($"You will depart from \b {_SF.ValueToAppendIfNotEmpty(_sessionData.DepartureICAO)}{_SF.ValueToAppendIfNotEmpty(_sessionData.DepartureName, True)}{_SF.ValueToAppendIfNotEmpty(_sessionData.DepartureExtra, True, True)}\b0\line ")

        'Arrival airfield And expected runway
        sb.Append($"You will land at \b {_SF.ValueToAppendIfNotEmpty(_sessionData.ArrivalICAO)}{_SF.ValueToAppendIfNotEmpty(_sessionData.ArrivalName, True)}{_SF.ValueToAppendIfNotEmpty(_sessionData.ArrivalExtra, True, True)}\b0\line ")

        'Type of soaring
        Dim soaringType As String = GetSoaringTypesSelected()
        If soaringType.Trim <> String.Empty OrElse _sessionData.SoaringExtraInfo <> String.Empty Then
            sb.Append($"Soaring Type is \b {soaringType}{_SF.ValueToAppendIfNotEmpty(_sessionData.SoaringExtraInfo, True, True)}\b0\line ")
        End If

        'Task distance And total distance
        sb.Append($"Distance are \b {_SF.GetDistance(totalDistance.ToString, trackDistance.ToString, PrefUnits)}\b0\line ")

        'Approx. duration
        sb.Append($"Approx. duration should be \b {_SF.GetDuration(_sessionData.DurationMin, _sessionData.DurationMax)}{_SF.ValueToAppendIfNotEmpty(_sessionData.DurationExtraInfo, True, True)}\b0\line ")

        'Recommended gliders
        If _sessionData.RecommendedGliders.Trim <> String.Empty Then
            sb.Append($"Recommended gliders: \b {_SF.ValueToAppendIfNotEmpty(_sessionData.RecommendedGliders)}\b0\line ")
        End If

        'Difficulty rating
        If _sessionData.DifficultyRating.Trim <> String.Empty OrElse _sessionData.DifficultyExtraInfo.Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as \b {_SF.GetDifficulty(CInt(_sessionData.DifficultyRating.Substring(0, 1)), _sessionData.DifficultyExtraInfo, True)}\b0\line ")
        End If

        sb.Append("\line ")

        If _WeatherDetails IsNot Nothing Then
            'Weather info (temperature, baro pressure, precipitations)
            sb.Append($"The weather profile to load is \b {_WeatherDetails.PresetName}\b0\line ")
            If _sessionData.WeatherSummary <> String.Empty Then
                sb.Append($"Weather summary: \b {_SF.ValueToAppendIfNotEmpty(_sessionData.WeatherSummary)}\b0\line ")
            End If
            If _WeatherDetails.AltitudeMeasurement = "AGL" Then
                sb.Append($"The elevation measurement used is \b AGL (Above Ground Level)\b0\line ")
            Else
                sb.Append($"The elevation measurement used is \b AMSL (Above Mean Sea Level)\b0\line ")
            End If
            sb.Append($"The barometric pressure is \b {_WeatherDetails.MSLPressure(PrefUnits)}\b0\line ")
            sb.Append($"The temperature is \b {_WeatherDetails.MSLTemperature(PrefUnits)}\b0\line ")
            sb.Append($"The humidity index is \b {_WeatherDetails.Humidity}\b0\line ")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"Precipitations: \b {_WeatherDetails.Precipitations}\b0\line ")
            End If

            sb.Append("\line ")

            'Winds
            sb.Append($"\b Wind Layers\b0\line ")
            Dim lines As String() = _WeatherDetails.WindLayersAsString(PrefUnits).Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}\line ")
            Next

            sb.Append(" \line ")

            'Clouds
            sb.Append($"\b Cloud Layers\b0\line ")
            lines = _WeatherDetails.CloudLayersText(PrefUnits).Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}\line ")
            Next

            ' Create and add wind layer controls to flow layout panel
            Dim windLayer As WindLayer
            For i As Integer = _WeatherDetails.WindLayers.Count - 1 To 0 Step -1
                windLayer = _WeatherDetails.WindLayers(i)
                If windLayer.IsValidWindLayer Then
                    Dim windLayerControl As New WindLayerControl(windLayer, PrefUnits)
                    windLayersFlowLayoutPnl.Controls.Add(windLayerControl)
                End If
            Next

        End If
        sb.Append("}")
        txtBriefing.Rtf = sb.ToString()
        SupportingFeatures.SetZoomFactorOfRichTextBox(txtBriefing)

        sb.Clear()

        imageViewer.Visible = True
        If _sessionData.MapImageSelected = String.Empty Then
            imageViewer.Enabled = False
        Else
            Dim filename As String = _sessionData.MapImageSelected
            If _unpackFolder <> String.Empty Then
                filename = Path.Combine(_unpackFolder, Path.GetFileName(filename))
            End If
            imageViewer.LoadImage(filename)
        End If


        'Build full description
        If _sessionData.LongDescription <> String.Empty Then
            sb.AppendLine("**Full Description**")
            sb.AppendLine(_sessionData.LongDescription.Replace("($*$)", Environment.NewLine))
            _SF.FormatMarkdownToRTF(sb.ToString.Trim, txtFullDescription)

        End If

        'Build altitude restrictions
        Dim dt As New DataTable()
        dt.Columns.Add("Waypoint Name", GetType(String))
        dt.Columns.Add("Restrictions", GetType(String))
        For Each waypoint In _SF.AllWaypoints.Where(Function(x) x.ContainsRestriction)
            dt.Rows.Add(waypoint.WaypointName, waypoint.Restrictions(PrefUnits))
        Next
        restrictionsDataGrid.DataSource = dt
        restrictionsDataGrid.Font = New Font(restrictionsDataGrid.Font.FontFamily, 12)
        restrictionsDataGrid.RowTemplate.Height = 28
        restrictionsDataGrid.RowHeadersVisible = False
        restrictionsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        'Build add-ons grid
        Dim dtAddOns As New DataTable()
        dtAddOns.Columns.Add("#", GetType(Integer))
        dtAddOns.Columns.Add("Name", GetType(String))
        dtAddOns.Columns.Add("Type", GetType(String))
        dtAddOns.Columns.Add("URL (double-click to open in browser)", GetType(String))
        Dim seqAddOn As Integer = 0
        For Each addOn As RecommendedAddOn In _sessionData.RecommendedAddOns
            seqAddOn += 1
            dtAddOns.Rows.Add(seqAddOn, addOn.Name, addOn.Type.ToString, addOn.URL)
        Next
        AddOnsDataGrid.DataSource = dtAddOns
        AddOnsDataGrid.Font = New Font(restrictionsDataGrid.Font.FontFamily, 12)
        AddOnsDataGrid.RowTemplate.Height = 28
        AddOnsDataGrid.RowHeadersVisible = False
        AddOnsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        AddOnsDataGrid.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        AddOnsDataGrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        AddOnsDataGrid.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells

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
            sb.Append($"Group or Club: \b {SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.GroupClubName)}\b0\line ")

            Dim fullMeetDateTimeLocal As DateTime = _sessionData.MeetLocalDateTime
            Dim fullSyncFlyDateTimeLocal As DateTime = _sessionData.SyncFlyLocalDateTime
            Dim fullLaunchDateTimeLocal As DateTime = _sessionData.LaunchLocalDateTime
            Dim fullStartTaskDateTimeLocal As DateTime = _sessionData.StartTaskLocalDateTime
            Dim fullMSFSLocalDateTime As DateTime = _sessionData.SimLocalDateTime

            Dim fullMeetDateTimeMSFS As DateTime
            Dim fullSyncFlyDateTimeMSFS As DateTime
            Dim fullLaunchDateTimeMSFS As DateTime
            Dim fullStartTaskDateTimeMSFS As DateTime
            _SF.ExpressEventTimesInMSFSTime(fullMeetDateTimeLocal,
                                        fullSyncFlyDateTimeLocal,
                                        fullLaunchDateTimeLocal,
                                        fullStartTaskDateTimeLocal,
                                        fullMSFSLocalDateTime,
                                        _sessionData.UseEventSyncFly,
                                        _sessionData.UseEventLaunch,
                                        fullMeetDateTimeMSFS,
                                        fullSyncFlyDateTimeMSFS,
                                        fullLaunchDateTimeMSFS,
                                        fullStartTaskDateTimeMSFS)

            'Define audio cues
            Dim audioCueDictionary As New Dictionary(Of Integer, String)

            ' Add cues with their associated embedded resource paths - for Meeting countdown
            audioCueDictionary.Clear()
            audioCueDictionary.Add(121, "2MinMeet") ' 2 minutes to meeting start
            audioCueDictionary.Add(61, "60SecMeet") ' 60 seconds to meeting start
            audioCueDictionary.Add(31, "30SecMeet") ' 30 seconds to meeting start
            audioCueDictionary.Add(1, "MeetingHasStarted") ' Meeting has started, have a good flight!

            countDownToMeet.SetTargetDateTime(fullMeetDateTimeLocal, audioCueDictionary)

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
                sb.Append($" when it's the Sync Fly time ({fullSyncFlyDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} your local time) \line ")
            ElseIf _sessionData.UseEventLaunch Then
                sb.Append($" when it's the Launch time ({fullLaunchDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} your local time) \line ")
            Else
                sb.Append($" when it's the Meet time ({fullMeetDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} your local time) \line ")
            End If
            sb.Append("\line ")

            'Sync Start or not?
            If _sessionData.UseEventSyncFly Then
                sb.Append($"This task requires a \b SYNC FLY \b0 so \b WAIT \b0 on the World Map for the signal. \line ")
                sb.Append($"Sync Fly expected at \b {fullSyncFlyDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullSyncFlyDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu / {fullSyncFlyDateTimeMSFS.ToString("t", CultureInfo.CurrentCulture)} in MSFS) \line ")
                sb.Append("\line ")

                ' Add cues with their associated embedded resource paths - for Sync Fly countdown
                audioCueDictionary.Clear()
                audioCueDictionary.Add(121, "2MinSyncFly") ' 2 minutes to sync fly
                audioCueDictionary.Add(61, "60SecSyncFly") ' 60 seconds to sync fly
                audioCueDictionary.Add(31, "30SecSyncFly") ' 30 seconds to sync fly
                audioCueDictionary.Add(16, "15Seconds") ' 15 seconds
                audioCueDictionary.Add(11, "From10ToClickFly") ' Countdown from 10 to now!

                countDownToSyncFly.SetTargetDateTime(fullSyncFlyDateTimeLocal, audioCueDictionary)
            Else
                sb.Append($"This task DOES NOT require a SYNC FLY so you can click Fly at your convenience and wait at the airfield. \line ")
                sb.Append("\line ")
                countDownToSyncFly.ResetToZero(True)
            End If

            'Launch
            If _sessionData.UseEventLaunch Then
                sb.Append($"Launch/Winch/Tow signal expected at \b {fullLaunchDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullLaunchDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu / {fullLaunchDateTimeMSFS.ToString("t", CultureInfo.CurrentCulture)} in MSFS) \line ")
                sb.Append("\line ")

                audioCueDictionary.Clear()

                If _sessionData.UseEventSyncFly Then
                    Dim diffBetweenSyncAndLaunch As TimeSpan = (fullLaunchDateTimeLocal - fullSyncFlyDateTimeLocal)
                    If diffBetweenSyncAndLaunch.TotalMinutes >= 5 Then
                        ' Add cues with their associated embedded resource paths - for Launch countdown
                        audioCueDictionary.Add(121, "2MinLaunch") ' 2 minutes to launch
                        audioCueDictionary.Add(61, "60SecLaunch") ' 60 seconds to launch
                        audioCueDictionary.Add(31, "30SecLaunch") ' 30 seconds to launch
                        audioCueDictionary.Add(16, "15Seconds") ' 15 seconds
                        audioCueDictionary.Add(11, "From10ToLaunch") ' Countdown from 10 to now!
                    End If
                End If

                countDownToLaunch.SetTargetDateTime(fullLaunchDateTimeLocal, audioCueDictionary)
            Else
                sb.Append($"Once at the airfield, launch at your convenience and wait for the task start signal. \line ")
                sb.Append("\line ")
                countDownToLaunch.ResetToZero(True)
            End If

            'Start task
            If _sessionData.UseEventStartTask Then
                sb.Append($"Task start/Start gate opening signal expected at \b {fullStartTaskDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} \b0 your local time ({Conversions.ConvertLocalToUTC(fullStartTaskDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu / {fullStartTaskDateTimeMSFS.ToString("t", CultureInfo.CurrentCulture)} in MSFS) \line ")
                sb.Append("\line ")

                ' Add cues with their associated embedded resource paths - for task start
                audioCueDictionary.Clear()
                audioCueDictionary.Add(121, "2MinTaskStart") ' 2 minutes to task start
                audioCueDictionary.Add(61, "60SecTaskStart") ' 60 seconds to task start
                audioCueDictionary.Add(31, "30SecTaskStart") ' 30 seconds to task start
                audioCueDictionary.Add(16, "15Seconds") ' 15 seconds
                audioCueDictionary.Add(11, "From10ToStartTask") ' Countdown from 10 to now!

                countDownTaskStart.SetTargetDateTime(fullStartTaskDateTimeLocal, audioCueDictionary)
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

        _loaded = True
        trackAudioCueVolume.Value = SupportingFeatures.ReadRegistryKey("AudioCues", 80)

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

    Private Sub SetWPGridColumnsVisibility()

        Try
            'Distance
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

    Private Sub UnitPrefChanged(radioBtn As RadioButton)

        If Not _initPrefUnits AndAlso radioBtn.Checked Then

            If radioBtn.Name.Contains("Altitude") Then
                PrefUnits.Altitude = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("Distance") Then
                PrefUnits.Distance = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("GateDiameter") Then
                PrefUnits.GateDiameter = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("WindSpeed") Then
                PrefUnits.WindSpeed = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("Baro") Then
                PrefUnits.Barometric = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("Temperature") Then
                PrefUnits.Temperature = CInt(radioBtn.Tag)
            End If

        End If

    End Sub

    Private Sub BuildCloudAndWindLayersDatagrids()

        'Build wind layers grid
        Dim dtWinds As New DataTable()
        dtWinds.Columns.Add("#", GetType(Integer))
        Select Case PrefUnits.Altitude
            Case AltitudeUnits.Both
                dtWinds.Columns.Add("Altitude (f / m)", GetType(String))
            Case AltitudeUnits.Metric
                dtWinds.Columns.Add("Altitude (m)", GetType(String))
            Case AltitudeUnits.Imperial
                dtWinds.Columns.Add("Altitude (f)", GetType(String))
        End Select
        Select Case PrefUnits.WindSpeed
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
        For Each windL As WindLayer In _WeatherDetails.WindLayers
            seqWindL += 1
            dtWinds.Rows.Add(seqWindL, windL.AltitudeCorrectUnit(PrefUnits), windL.SpeedCorrectUnit(PrefUnits), windL.Angle.ToString, windL.GetGustText(PrefUnits))
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
        Select Case PrefUnits.Altitude
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
        For Each CloudL As CloudLayer In _WeatherDetails.CloudLayers
            seqCloudL += 1
            dtClouds.Rows.Add(seqCloudL, CloudL.AltitudeBottomCorrectUnit(PrefUnits), CloudL.AltitudeTopCorrectUnit(PrefUnits), CloudL.CoverageForGrid, CloudL.DensityForGrid, CloudL.ScatteringForGrid)
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

    Private Sub btnGotoDiscordTaskThread_Click(sender As Object, e As EventArgs) Handles btnGotoDiscordTaskThread.Click

        If Not SupportingFeatures.LaunchURL(_sessionData.DiscordTaskThreadURL) Then
            MessageBox.Show("Invalid URL provided! Please specify a valid URL.", "Error launching Discord", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


#End Region

#End Region

End Class
