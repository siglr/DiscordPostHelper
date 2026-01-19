Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml
Imports System.Collections.Generic
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

Public Class BriefingControl

#Region "Constants and global members"

    Private _flightplanfile As String
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
    Private _discordPostID As String = String.Empty
    Private _weatherFile As String = String.Empty
    Private _dragDropHandlersInitialized As Boolean
    Private _validDragActive As Boolean
    Private _isManualMode As Boolean = False
    Private ReadOnly _controlsWithDragHandlers As New HashSet(Of Control)()
    Private _renderContext As BriefingRenderContext = New BriefingRenderContext()

    Public Property EventIsEnabled As Boolean
    Private ReadOnly Property PrefUnits As New PreferredUnits

    Public Property RenderContext As BriefingRenderContext
        Get
            If _renderContext Is Nothing Then
                _renderContext = New BriefingRenderContext()
            End If
            Return _renderContext
        End Get
        Set(value As BriefingRenderContext)
            If value Is Nothing Then
                _renderContext = New BriefingRenderContext()
            Else
                _renderContext = value
            End If
        End Set
    End Property

    Public Event ValidFilesDragActiveChanged As EventHandler(Of ValidFilesDragActiveChangedEventArgs)
    Public Event FilesDropped As EventHandler(Of FilesDroppedEventArgs)

#End Region

#Region "Drag and drop support"

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        InitializeDragDropSupport()
    End Sub

    Private Sub InitializeDragDropSupport()
        If _dragDropHandlersInitialized Then
            Return
        End If

        _dragDropHandlersInitialized = True
        AttachDragDropHandlersRecursive(Me)
    End Sub

    Private Sub AttachDragDropHandlersRecursive(target As Control)
        If target Is Nothing Then
            Return
        End If

        If _controlsWithDragHandlers.Contains(target) Then
            Return
        End If
        _controlsWithDragHandlers.Add(target)

        Try
            target.AllowDrop = True
        Catch ex As NotSupportedException
            ' Some controls might not support AllowDrop (e.g., WebBrowser). Ignore and continue.
        End Try

        AddHandler target.DragEnter, AddressOf OnControlDragEnter
        AddHandler target.DragOver, AddressOf OnControlDragOver
        AddHandler target.DragLeave, AddressOf OnControlDragLeave
        AddHandler target.DragDrop, AddressOf OnControlDragDrop
        AddHandler target.ControlAdded, AddressOf OnChildControlAdded

        For Each child As Control In target.Controls
            AttachDragDropHandlersRecursive(child)
        Next
    End Sub

    Private Sub OnChildControlAdded(sender As Object, e As ControlEventArgs)
        If Not _dragDropHandlersInitialized OrElse e Is Nothing OrElse e.Control Is Nothing Then
            Return
        End If

        AttachDragDropHandlersRecursive(e.Control)
    End Sub

    Private Sub OnControlDragEnter(sender As Object, e As DragEventArgs)
        ProcessDragData(e)
    End Sub

    Private Sub OnControlDragOver(sender As Object, e As DragEventArgs)
        ProcessDragData(e)
    End Sub

    Private Sub OnControlDragLeave(sender As Object, e As EventArgs)
        Dim mousePosition = Me.PointToClient(Control.MousePosition)
        If Me.ClientRectangle.Contains(mousePosition) Then
            Return
        End If

        UpdateValidDragState(False, Nothing)
    End Sub

    Private Sub OnControlDragDrop(sender As Object, e As DragEventArgs)
        Dim files = GetValidFilesFromData(e.Data)
        If files IsNot Nothing Then
            RaiseEvent FilesDropped(Me, New FilesDroppedEventArgs(files))
        End If

        UpdateValidDragState(False, Nothing)
    End Sub

    Private Sub ProcessDragData(e As DragEventArgs)
        Dim files = GetValidFilesFromData(e.Data)
        If files IsNot Nothing Then
            e.Effect = DragDropEffects.Copy
            UpdateValidDragState(True, files, True)
        Else
            e.Effect = DragDropEffects.None
            UpdateValidDragState(False, Nothing)
        End If
    End Sub

    Private Function GetValidFilesFromData(data As IDataObject) As IReadOnlyList(Of String)
        If data Is Nothing Then
            Return Nothing
        End If

        Dim entries As New List(Of String)

        If data.GetDataPresent(DataFormats.FileDrop) Then
            Dim droppedFiles = TryCast(data.GetData(DataFormats.FileDrop), String())
            If droppedFiles IsNot Nothing AndAlso droppedFiles.Length > 0 Then
                entries.AddRange(droppedFiles.Where(Function(f) Not String.IsNullOrWhiteSpace(f)))
            End If
        ElseIf data.GetDataPresent(DataFormats.UnicodeText) OrElse data.GetDataPresent(DataFormats.Text) Then
            Dim textValue As String = Nothing
            If data.GetDataPresent(DataFormats.UnicodeText) Then
                textValue = TryCast(data.GetData(DataFormats.UnicodeText), String)
            Else
                textValue = TryCast(data.GetData(DataFormats.Text), String)
            End If

            If Not String.IsNullOrWhiteSpace(textValue) Then
                entries.AddRange(textValue.Split({Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select(Function(t) t.Trim()).Where(Function(t) Not String.IsNullOrWhiteSpace(t)))
            End If
        End If

        Dim fileArray = entries.ToArray()
        If fileArray.Length = 0 Then
            Return Nothing
        End If

        If ContainsWeSimGlideLink(fileArray) Then
            Return Array.AsReadOnly(fileArray)
        End If

        If fileArray.Length = 1 Then
            Dim extension = GetEntryExtension(fileArray(0))
            If IsSingleFileExtensionValid(extension) Then
                Return Array.AsReadOnly(fileArray)
            End If
        ElseIf fileArray.Length <= 3 Then
            Dim extensions = fileArray.Select(Function(f) GetEntryExtension(f)).ToList()

            Dim hasPln = extensions.Any(Function(ext) HasExtension(ext, ".pln"))
            Dim hasWpr = extensions.Any(Function(ext) HasExtension(ext, ".wpr"))
            Dim hasOnlySupported = extensions.All(Function(ext) HasExtension(ext, ".pln") OrElse HasExtension(ext, ".wpr") OrElse HasExtension(ext, ".dph"))

            If hasOnlySupported AndAlso hasPln AndAlso (fileArray.Length <= 2 OrElse hasWpr) Then
                Return Array.AsReadOnly(fileArray)
            End If
        End If

        Return Nothing
    End Function

    Private Function GetEntryExtension(entry As String) As String
        If String.IsNullOrWhiteSpace(entry) Then
            Return String.Empty
        End If

        Dim uri As Uri = Nothing
        If Uri.TryCreate(entry.Trim(), UriKind.Absolute, uri) AndAlso (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps) Then
            Return Path.GetExtension(uri.AbsolutePath)
        End If

        Return Path.GetExtension(entry)
    End Function

    Private Shared Function IsSingleFileExtensionValid(extension As String) As Boolean
        Return HasExtension(extension, ".dphx") OrElse HasExtension(extension, ".zip") OrElse HasExtension(extension, ".pln") OrElse HasExtension(extension, ".dph")
    End Function

    Private Shared Function HasExtension(extension As String, expectedExtension As String) As Boolean
        If String.IsNullOrEmpty(extension) Then
            Return False
        End If

        Return String.Equals(extension, expectedExtension, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function ContainsWeSimGlideLink(entries As IEnumerable(Of String)) As Boolean
        If entries Is Nothing Then
            Return False
        End If

        Return entries.Any(Function(entry) Not String.IsNullOrWhiteSpace(entry) AndAlso entry.IndexOf("wesimglide.org", StringComparison.OrdinalIgnoreCase) >= 0)
    End Function

    Private Sub UpdateValidDragState(isActive As Boolean, files As IReadOnlyList(Of String), Optional forceRaise As Boolean = False)
        Dim shouldRaise As Boolean = False

        If isActive Then
            If Not _validDragActive OrElse forceRaise Then
                shouldRaise = True
            End If
            _validDragActive = True
        Else
            If _validDragActive Then
                shouldRaise = True
            End If
            _validDragActive = False
        End If

        If shouldRaise Then
            Dim eventFiles As IReadOnlyList(Of String) = If(isActive, files, Nothing)
            RaiseEvent ValidFilesDragActiveChanged(Me, New ValidFilesDragActiveChangedEventArgs(isActive, eventFiles))
        End If
    End Sub

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
                Using New Centered_MessageBox()
                    MessageBox.Show("An error occured trying to open the specified URL!", "Trying to open URL", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
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
                                                                                            radioGateDiameterBoth.CheckedChanged,
                                                                                            radioGateMeasurementRadius.CheckedChanged,
                                                                                            radioGateMeasurementDiameter.CheckedChanged,
                                                                                            radioSpeedMetric.CheckedChanged,
                                                                                            radioSpeedKnots.CheckedChanged,
                                                                                            radioSpeedImperial.CheckedChanged

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

    Private Sub chkShowGraph_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowGraph.CheckedChanged
        UpdateWeatherGraphLabel()
        FullWeatherGraphPanel1.Visible = chkShowGraph.Checked

        Select Case chkShowGraph.Checked
            Case True
                SupportingFeatures.WriteRegistryKey("WeatherGraph", 1)
            Case False
                SupportingFeatures.WriteRegistryKey("WeatherGraph", 0)
        End Select

    End Sub

#End Region

#Region "Subs and functions"

#Region "Public"

    Public Sub Closing()
        Me.SuspendLayout()
    End Sub

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

        Select Case PrefUnits.Speed
            Case SpeedUnits.Metric
                radioSpeedMetric.Checked = True
            Case SpeedUnits.Imperial
                radioSpeedImperial.Checked = True
            Case SpeedUnits.Knots
                radioSpeedKnots.Checked = True
        End Select

        Select Case PrefUnits.GateDiameter
            Case GateDiameterUnits.Metric
                radioGateDiameterMetric.Checked = True
            Case GateDiameterUnits.Imperial
                radioGateDiameterImperial.Checked = True
            Case GateDiameterUnits.Both
                radioGateDiameterBoth.Checked = True
        End Select

        Select Case PrefUnits.GateMeasurement
            Case GateMeasurementChoices.Diameter
                radioGateMeasurementDiameter.Checked = True
            Case GateMeasurementChoices.Radius
                radioGateMeasurementRadius.Checked = True
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
        btnGotoDiscordTaskThread.Enabled = False

        If imagesListView.LargeImageList IsNot Nothing Then
            imagesListView.LargeImageList.Dispose()
            imagesListView.LargeImageList = Nothing
        End If
        imagesListView.Items.Clear()
        imagesListView.Clear()

        ClearCountryFlagPictures()

        EventIsEnabled = False
        FullWeatherGraphPanel1.ResetGraph()
        windLayersFlowLayoutPnl.Controls.Clear()

        CountDownReset()
        _weatherFile = String.Empty

        GC.Collect()

    End Sub

    Public Sub ChangeImage(imgFilename As String)
        If imgFilename = String.Empty Then
            imageViewer.ClearImage()
        Else
            imageViewer.LoadImage(imgFilename)
        End If
    End Sub

    Public Sub GenerateBriefing(supportFeat As SupportingFeatures,
                                sessionData As AllData,
                                flightplanfile As String,
                                weatherfile As String,
                                discordPostID As String,
                                Optional unpackFolder As String = "NONE",
                                Optional isManualMode As Boolean = False)

        _SF = supportFeat
        SupportingFeatures.PrefUnits = PrefUnits
        _isManualMode = isManualMode

        _sessionData = sessionData
        _weatherFile = weatherfile
        If unpackFolder = "NONE" Then
            _unpackFolder = String.Empty
            lblPrefUnitsMessage.Text = $"Units selected here are only used for YOUR briefing tabs.{Environment.NewLine}They DO NOT change the content of generated Discord posts which always include all formats.{Environment.NewLine}Also, any data specified in description fields is excluded and will appear as is."
        Else
            _unpackFolder = unpackFolder
        End If

        'Load flight plan
        If File.Exists(flightplanfile) Then
            _flightplanfile = flightplanfile
            _XmlDocFlightPlan = New XmlDocument
            _XmlDocFlightPlan.Load(_flightplanfile)
        Else
            Using New Centered_MessageBox()
                MessageBox.Show($"Flight plan file not found!{Environment.NewLine}{flightplanfile}{Environment.NewLine}Cannot generate briefing.", "Flight plan file missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        'Load weather info
        Dim weatherFileToLoad = SelectWeatherFileForDisplay(weatherfile)
        _weatherFile = weatherFileToLoad
        If File.Exists(weatherFileToLoad) Then
            _XmlDocWeatherPreset = New XmlDocument
            _XmlDocWeatherPreset.Load(weatherFileToLoad)
            _WeatherDetails = Nothing
            _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)
        Else
            Using New Centered_MessageBox()
                MessageBox.Show($"Weather file not found!{Environment.NewLine}{weatherFileToLoad}{Environment.NewLine}Cannot generate briefing.", "Weather file missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        _SF.DownloadCountryFlags(_sessionData.Countries)
        BuildTaskData()
        BuildCloudAndWindLayersDatagrids()
        AddCountryFlagPictures()
        FullWeatherGraphPanel1.SetWeatherInfo(_WeatherDetails, PrefUnits, SupportingFeatures.GetEnUSFormattedDate(_sessionData.SimLocalDateTime, _sessionData.SimLocalDateTime, _sessionData.IncludeYear))

        'TaskID
        If _sessionData.TaskID = String.Empty AndAlso _sessionData.DiscordTaskID <> String.Empty Then
            _sessionData.TaskID = _sessionData.DiscordTaskID
        ElseIf _sessionData.TaskID = String.Empty AndAlso _sessionData.DiscordTaskThreadURL <> String.Empty AndAlso SupportingFeatures.IsValidURL(_sessionData.DiscordTaskThreadURL) Then
            _sessionData.TaskID = SupportingFeatures.ExtractMessageIDFromDiscordURL(_sessionData.DiscordTaskThreadURL, True)
        End If

        _discordPostID = discordPostID
        If _discordPostID = String.Empty Then
            btnGotoDiscordTaskThread.Text = $"No task thread defined"
            btnGotoDiscordTaskThread.Enabled = False
        Else
            btnGotoDiscordTaskThread.Text = $"Bring me to this task's discussion thread on {SupportingFeatures.ReturnDiscordServer(String.Empty, True)} Discord server (right click if you need invite)"
            btnGotoDiscordTaskThread.Enabled = True
        End If

        'Generate the Setup tab
        BuildSetupTab()
        UpdateWeatherGraphLabel()

    End Sub

    Private Sub BuildSetupTab()

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat
        Dim dateFormat As String
        If _sessionData.IncludeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim showTrackerGroup As Boolean = False
        If _isManualMode Then
            showTrackerGroup = Not String.IsNullOrWhiteSpace(_sessionData.TrackerGroup)
        End If

        lblTaskTitle.Text = $"{_sessionData.Title} (#{_sessionData.EntrySeqID})"
        lblDeparture.Text = $"{_sessionData.DepartureICAO}/{_sessionData.DepartureExtra}"
        lblTaskName.Text = $"{Path.GetFileNameWithoutExtension(_sessionData.FlightPlanFilename)}"
        lblFlightPlanTitle.Text = SupportingFeatures.GetFlightPlanTitleFromPln(_flightplanfile)
        If lblTaskName.Text.Trim() = lblFlightPlanTitle.Text.Trim() Then
            pnlFlightPlanTitle.Visible = False
        Else
            pnlFlightPlanTitle.Visible = True
        End If

        Dim hasSimLocalInfo = _sessionData.SimDate <> Date.MinValue AndAlso _sessionData.SimTime <> Date.MinValue
        If hasSimLocalInfo Then
            lblSimLocalDateTime.Text = $"{_sessionData.SimLocalDateTime.ToString(dateFormat, _EnglishCulture)}, {_sessionData.SimLocalDateTime.ToString(dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture)} {SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.SimDateTimeExtraInfo.Trim, True, True)}"
        Else
            lblSimLocalDateTime.Text = "Not provided"
        End If
        UpdateSetupWeatherPanels()
        lblRecGliders.Text = If(String.IsNullOrWhiteSpace(_sessionData.RecommendedGliders), "Not provided", _sessionData.RecommendedGliders)

        'Unstandard Barometric pressure
        If Not _WeatherDetails.IsStandardMSLPressure Then
            pnlSetupBaroWarning.Visible = True
            lblBaroNote.Text = _WeatherDetails.MSLPressure(_sessionData.BaroPressureExtraInfo, _sessionData.SuppressBaroPressureWarningSymbol, PrefUnits, False)
        Else
            pnlSetupBaroWarning.Visible = False
        End If

        If SupportingFeatures.IsEventActive(_sessionData) Then
            pnlSetupEventTitle.Visible = True
            pnlSetupServer.Visible = True
            lblGroupEventTitle.Text = $"{SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.GroupClubName)} - {SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.EventTopic)}"
            lblEventMSFSServer.Text = _SF.GetMSFSServers.ElementAt(_sessionData.MSFSServer)
            If _sessionData.TrackerGroup.Trim <> String.Empty Then
                pnlSetupTrackerGroup.Visible = True
                lblEventTrackerGroup.Text = _sessionData.TrackerGroup
            Else
                pnlSetupTrackerGroup.Visible = False
            End If
        Else
            pnlSetupEventTitle.Visible = False
            pnlSetupServer.Visible = False
            If Not _sessionData.EventEnabled AndAlso showTrackerGroup Then
                pnlSetupTrackerGroup.Visible = True
                lblEventTrackerGroup.Text = _sessionData.TrackerGroup
            Else
                pnlSetupTrackerGroup.Visible = False
            End If
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
                Case 1 'Main Task Info
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtBriefing)
                Case 2 'Map
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtFullDescription)
                Case 3 'Event Info
                    SupportingFeatures.SetZoomFactorOfRichTextBox(txtEventInfo)
                Case 4 'Images
                Case 5 'All Waypoints
                Case 6 'Weather
                    FullWeatherGraphPanel1.Visible = chkShowGraph.Checked
                    FullWeatherGraphPanel1.SetWeatherInfo(_WeatherDetails, PrefUnits, SupportingFeatures.GetEnUSFormattedDate(_sessionData.SimLocalDateTime, _sessionData.SimLocalDateTime, _sessionData.IncludeYear))
                Case 7 'Add-ons
                Case 8 'Units
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
                Dim countryCode As String = _SF.CountryFlagCodes(countryName).Item2
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

    Private Function GetInstalledSimLabel() As String
        Dim sims = RenderContext.InstalledSims
        Dim has2020 As Boolean = (sims And InstalledSimFlags.MSFS2020) = InstalledSimFlags.MSFS2020
        Dim has2024 As Boolean = (sims And InstalledSimFlags.MSFS2024) = InstalledSimFlags.MSFS2024

        If has2020 AndAlso has2024 Then
            Return "MSFS 2020/2024"
        End If

        If has2024 Then
            Return "MSFS 2024"
        End If

        If has2020 Then
            Return "MSFS 2020"
        End If

        Return "MSFS"
    End Function

    Private Sub UpdateSetupWeatherPanels()
        Dim weather2024Path = GetWeatherFilePath(_sessionData.WeatherFilename)
        Dim weather2020Path = GetWeatherFilePath(_sessionData.WeatherFilenameSecondary)
        Dim has2024 As Boolean = Not String.IsNullOrWhiteSpace(weather2024Path)
        Dim has2020 As Boolean = Not String.IsNullOrWhiteSpace(weather2020Path)

        Dim allow2024 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2024) = InstalledSimFlags.MSFS2024
        Dim allow2020 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2020) = InstalledSimFlags.MSFS2020

        If Not allow2024 Then
            has2024 = False
        End If
        If Not allow2020 Then
            has2020 = False
        End If

        If has2024 AndAlso has2020 Then
            pnlSetupWeather2024.Visible = True
            pnlSetupWeather2020.Visible = True
            lblWeatherTitle2024.Text = "Weather (2024)"
            lblWeatherTitle2020.Text = "Weather (2020)"
            lblWeatherProfile2024.Text = GetWeatherPresetDisplayName(weather2024Path, GetWeatherDetailsPresetNameForFile(weather2024Path))
            lblWeatherProfile2020.Text = GetWeatherPresetDisplayName(weather2020Path, GetWeatherDetailsPresetNameForFile(weather2020Path))
        ElseIf has2024 Then
            pnlSetupWeather2024.Visible = True
            pnlSetupWeather2020.Visible = False
            lblWeatherTitle2024.Text = "Weather"
            lblWeatherProfile2024.Text = GetWeatherPresetDisplayName(weather2024Path, GetWeatherDetailsPresetNameForFile(weather2024Path))
        ElseIf has2020 Then
            pnlSetupWeather2024.Visible = False
            pnlSetupWeather2020.Visible = True
            lblWeatherTitle2020.Text = "Weather"
            lblWeatherProfile2020.Text = GetWeatherPresetDisplayName(weather2020Path, GetWeatherDetailsPresetNameForFile(weather2020Path))
        Else
            pnlSetupWeather2024.Visible = False
            pnlSetupWeather2020.Visible = False
        End If
    End Sub

    Private Function GetPrimaryWeatherPresetDisplayName() As String
        Return GetWeatherPresetDisplayName(_weatherFile, GetWeatherDetailsPresetNameForFile(_weatherFile))
    End Function

    Private Function GetWeatherDetailsPresetName() As String
        If _WeatherDetails Is Nothing Then
            Return String.Empty
        End If

        Return _WeatherDetails.PresetName
    End Function

    Private Function GetWeatherDetailsPresetNameForFile(weatherFilePath As String) As String
        If String.IsNullOrWhiteSpace(weatherFilePath) OrElse String.IsNullOrWhiteSpace(_weatherFile) Then
            Return String.Empty
        End If

        If String.Equals(Path.GetFullPath(weatherFilePath), Path.GetFullPath(_weatherFile), StringComparison.OrdinalIgnoreCase) Then
            Return GetWeatherDetailsPresetName()
        End If

        Return String.Empty
    End Function

    Private Function GetWeatherFilePath(weatherFilename As String) As String
        If String.IsNullOrWhiteSpace(weatherFilename) Then
            Return String.Empty
        End If

        If _unpackFolder <> String.Empty Then
            Return Path.Combine(_unpackFolder, Path.GetFileName(weatherFilename))
        End If

        Return weatherFilename
    End Function

    Private Function SelectWeatherFileForDisplay(requestedWeatherFile As String) As String
        Dim primaryWeatherPath = GetWeatherFilePath(_sessionData.WeatherFilename)
        Dim secondaryWeatherPath = GetWeatherFilePath(_sessionData.WeatherFilenameSecondary)

        Dim hasPrimary As Boolean = Not String.IsNullOrWhiteSpace(primaryWeatherPath)
        Dim hasSecondary As Boolean = Not String.IsNullOrWhiteSpace(secondaryWeatherPath)

        Dim allow2024 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2024) = InstalledSimFlags.MSFS2024
        Dim allow2020 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2020) = InstalledSimFlags.MSFS2020

        If allow2024 AndAlso allow2020 AndAlso hasPrimary AndAlso hasSecondary Then
            Return primaryWeatherPath
        End If

        If allow2020 AndAlso Not allow2024 AndAlso hasSecondary Then
            Return secondaryWeatherPath
        End If

        If allow2024 AndAlso Not allow2020 AndAlso hasPrimary Then
            Return primaryWeatherPath
        End If

        If hasPrimary Then
            Return primaryWeatherPath
        End If

        If hasSecondary Then
            Return secondaryWeatherPath
        End If

        Return requestedWeatherFile
    End Function

    Private Sub UpdateWeatherGraphLabel()
        Dim primaryWeatherPath = GetWeatherFilePath(_sessionData.WeatherFilename)
        Dim secondaryWeatherPath = GetWeatherFilePath(_sessionData.WeatherFilenameSecondary)
        Dim hasPrimary As Boolean = Not String.IsNullOrWhiteSpace(primaryWeatherPath)
        Dim hasSecondary As Boolean = Not String.IsNullOrWhiteSpace(secondaryWeatherPath)

        Dim allow2024 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2024) = InstalledSimFlags.MSFS2024
        Dim allow2020 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2020) = InstalledSimFlags.MSFS2020

        If allow2024 AndAlso allow2020 AndAlso hasPrimary AndAlso hasSecondary Then
            chkShowGraph.Text = "Show graph instead of data table (showing MSFS 2024 preset)"
        Else
            chkShowGraph.Text = "Show graph instead of data table"
        End If
    End Sub

    Private Function GetWeatherPresetDisplayName(weatherFilePath As String, fallbackPresetName As String) As String
        If RenderContext.PresetNameDisplayMode = PresetNameDisplayMode.Exact Then
            If Not String.IsNullOrWhiteSpace(weatherFilePath) Then
                Return Path.GetFileNameWithoutExtension(weatherFilePath)
            End If

            Return fallbackPresetName
        End If

        If Not String.IsNullOrWhiteSpace(fallbackPresetName) Then
            Return fallbackPresetName
        End If

        Dim presetName = TryReadWeatherPresetName(weatherFilePath)
        If Not String.IsNullOrWhiteSpace(presetName) Then
            Return presetName
        End If

        Return GetFriendlyPresetNameFromFilename(weatherFilePath)
    End Function

    Private Sub AppendTaskWeatherPresetLines(sb As StringBuilder)
        Dim entries = GetWeatherPresetEntriesForText()
        If entries.Count = 0 Then
            Return
        End If

        If entries.Count = 2 Then
            For Each entry In entries
                sb.Append($"For {entry.SimLabel}, the weather profile to load is **{entry.DisplayName}**($*$)")
            Next
            Return
        End If

        sb.Append($"The weather profile to load is **{entries(0).DisplayName}**($*$)")
    End Sub

    Private Sub AppendEventWeatherPresetLines(sb As StringBuilder)
        Dim entries = GetWeatherPresetEntriesForText()
        If entries.Count = 0 Then
            Return
        End If

        If entries.Count = 2 Then
            For Each entry In entries
                sb.Append($"For {entry.SimLabel}, load weather preset **{entry.DisplayName}**($*$)")
            Next
            Return
        End If

        sb.Append($"Load weather preset: **{entries(0).DisplayName}**($*$)")
    End Sub

    Private Function GetWeatherPresetEntriesForText() As List(Of WeatherPresetEntry)
        Dim results As New List(Of WeatherPresetEntry)()
        Dim allow2024 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2024) = InstalledSimFlags.MSFS2024
        Dim allow2020 As Boolean = (RenderContext.InstalledSims And InstalledSimFlags.MSFS2020) = InstalledSimFlags.MSFS2020

        Dim weather2024Path = GetWeatherFilePath(_sessionData.WeatherFilename)
        Dim weather2020Path = GetWeatherFilePath(_sessionData.WeatherFilenameSecondary)

        If allow2024 AndAlso Not String.IsNullOrWhiteSpace(weather2024Path) Then
            Dim displayName = GetWeatherPresetDisplayName(weather2024Path, GetWeatherDetailsPresetNameForFile(weather2024Path))
            If Not String.IsNullOrWhiteSpace(displayName) Then
                results.Add(New WeatherPresetEntry("MSFS 2024", displayName))
            End If
        End If

        If allow2020 AndAlso Not String.IsNullOrWhiteSpace(weather2020Path) Then
            Dim displayName = GetWeatherPresetDisplayName(weather2020Path, GetWeatherDetailsPresetNameForFile(weather2020Path))
            If Not String.IsNullOrWhiteSpace(displayName) Then
                results.Add(New WeatherPresetEntry("MSFS 2020", displayName))
            End If
        End If

        Return results
    End Function

    Private NotInheritable Class WeatherPresetEntry
        Public Sub New(simLabel As String, displayName As String)
            Me.SimLabel = simLabel
            Me.DisplayName = displayName
        End Sub

        Public ReadOnly Property SimLabel As String
        Public ReadOnly Property DisplayName As String
    End Class

    Private Function TryReadWeatherPresetName(weatherFilePath As String) As String
        If String.IsNullOrWhiteSpace(weatherFilePath) OrElse Not File.Exists(weatherFilePath) Then
            Return String.Empty
        End If

        Try
            Dim xmlWeather As New XmlDocument
            xmlWeather.Load(weatherFilePath)
            Dim nameNode = xmlWeather.DocumentElement.SelectNodes("WeatherPreset.Preset/Name").Item(0)
            If nameNode IsNot Nothing AndAlso nameNode.FirstChild IsNot Nothing Then
                Return nameNode.FirstChild.Value
            End If
        Catch ex As Exception
        End Try

        Return String.Empty
    End Function

    Private Function GetFriendlyPresetNameFromFilename(weatherFilePath As String) As String
        If String.IsNullOrWhiteSpace(weatherFilePath) Then
            Return String.Empty
        End If

        Dim nameOnly = Path.GetFileNameWithoutExtension(weatherFilePath)
        If nameOnly.StartsWith("0_", StringComparison.OrdinalIgnoreCase) Then
            nameOnly = nameOnly.Substring(2)
        End If

        Return nameOnly
    End Function

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
        Dim possibleElevationUpdateRequired As Boolean = False

        EventIsEnabled = _sessionData.EventEnabled

        _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, trackDistance, possibleElevationUpdateRequired, False)

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat
        Dim dateFormat As String
        If _sessionData.IncludeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        'Title
        sb.Append($"**{_sessionData.Title}**($*$)")
        If _sessionData.MainAreaPOI.Trim <> String.Empty Then
            sb.Append($"{_sessionData.MainAreaPOI}($*$)")
            sb.Append("($*$)")
        End If
        If _sessionData.ShortDescription.Trim <> String.Empty Then
            sb.Append($"{_sessionData.ShortDescription}($*$)")
            sb.Append("($*$)")
        End If
        sb.Append($"{_sessionData.Credits}($*$)")
        If _sessionData.EnableRepostInfo Then
            If _sessionData.RepostOriginalURL.Length > 0 Then
                Dim discordURL As String = SupportingFeatures.DiscordURL(_sessionData.RepostOriginalURL)
                sb.AppendLine($"This task was originally posted on [{SupportingFeatures.ReturnDiscordServer(_sessionData.RepostOriginalURL)}]({discordURL}) on {_sessionData.RepostOriginalDate.ToString("MMMM dd, yyyy", _EnglishCulture)}")
            Else
                sb.AppendLine($"This task was originally posted on {_sessionData.RepostOriginalDate.ToString("MMMM dd, yyyy", _EnglishCulture)}")
            End If
        End If
        sb.Append("($*$)")

        'Credits

        'Local MSFS date and time
        Dim hasSimLocalInfo = _sessionData.SimDate <> Date.MinValue AndAlso _sessionData.SimTime <> Date.MinValue
        If hasSimLocalInfo Then
            sb.Append($"MSFS Local date & time is **{_sessionData.SimLocalDateTime.ToString(dateFormat, _EnglishCulture)}, {_sessionData.SimLocalDateTime.ToString(dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture)} {SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.SimDateTimeExtraInfo.Trim, True, True)}**($*$)")
        Else
            sb.Append("MSFS Local date & time is **Not specified**($*$)")
        End If

        'Flight plan
        sb.Append($"The flight plan to load is **{Path.GetFileName(_sessionData.FlightPlanFilename)}**($*$)")

        sb.Append("($*$)")

        'Departure airfield And runway
        sb.Append($"You will depart from **{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.DepartureICAO)}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.DepartureName, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.DepartureExtra, True, True)}**($*$)")

        'Arrival airfield And expected runway
        sb.Append($"You will land at **{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.ArrivalICAO)}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.ArrivalName, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.ArrivalExtra, True, True)}**($*$)")

        'Type of soaring
        Dim soaringType As String = SupportingFeatures.GetSoaringTypesSelected(_sessionData.SoaringRidge, _sessionData.SoaringThermals, _sessionData.SoaringWaves, _sessionData.SoaringDynamic)
        If soaringType.Trim <> String.Empty OrElse _sessionData.SoaringExtraInfo <> String.Empty Then
            sb.Append($"Soaring Type is **{soaringType}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.SoaringExtraInfo, True, True)}**($*$)")
        End If

        'Task distance And total distance
        sb.Append($"Distance are **{SupportingFeatures.GetDistance(totalDistance.ToString, trackDistance.ToString, PrefUnits)}**($*$)")

        'Approx. duration
        sb.Append($"Approx. duration should be **{SupportingFeatures.GetDuration(_sessionData.DurationMin, _sessionData.DurationMax)}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.DurationExtraInfo, True, True)}**($*$)")
        If _SF.AATMinDuration > TimeSpan.Zero Then
            sb.Append($"This is an **AAT** with a minimum duration of **{SupportingFeatures.FormatTimeSpanAsText(_SF.AATMinDuration)}**($*$)")
        End If

        'Recommended gliders
        If _sessionData.RecommendedGliders.Trim <> String.Empty Then
            sb.Append($"Recommended gliders: **{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.RecommendedGliders)}**($*$)")
        End If

        'Difficulty rating
        If _sessionData.DifficultyRating.Trim <> String.Empty OrElse _sessionData.DifficultyExtraInfo.Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as **{SupportingFeatures.GetDifficulty(CInt(_sessionData.DifficultyRating.Substring(0, 1)), _sessionData.DifficultyExtraInfo, True)}**($*$)")
        End If

        sb.Append("($*$)")

        If _WeatherDetails IsNot Nothing Then
            'Weather info (temperature, baro pressure, precipitations)
            AppendTaskWeatherPresetLines(sb)
            If _sessionData.WeatherSummary <> String.Empty Then
                sb.Append($"Weather summary: **{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.WeatherSummary)}**($*$)")
            End If
            If _WeatherDetails.AltitudeMeasurement = "AMGL" Then
                sb.Append($"The elevation measurement used is **AMGL (Above Mean Ground Level)**($*$)")
            Else
                sb.Append($"The elevation measurement used is **AMSL (Above Mean Sea Level)**($*$)")
            End If
            sb.Append($"The barometric pressure is **{_WeatherDetails.MSLPressure(_sessionData.BaroPressureExtraInfo, _sessionData.SuppressBaroPressureWarningSymbol, PrefUnits, False)}**($*$)")
            sb.Append($"The temperature is **{_WeatherDetails.MSLTemperature(PrefUnits)}**($*$)")
            sb.Append($"The aerosol index is **{_WeatherDetails.Humidity}**($*$)")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"The precipitations are: **{_WeatherDetails.Precipitations}**($*$)")
            End If
            If _WeatherDetails.HasSnowCover Then
                sb.Append($"The snow cover is: **{_WeatherDetails.SnowCover}**($*$)")
            End If
            If _WeatherDetails.ThunderstormIntensity > 0 Then
                sb.Append($"The lightning intensity is: **{_WeatherDetails.ThunderstormIntensity}%**($*$)")
            End If

            sb.Append("($*$)")

            'Winds
            sb.Append($"**Wind Layers**($*$)")
            Dim lines As String() = _WeatherDetails.WindLayersAsString(PrefUnits).Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}($*$)")
            Next

            sb.Append(" ($*$)")

            'Clouds
            sb.Append($"**Cloud Layers**($*$)")
            lines = _WeatherDetails.CloudLayersText(PrefUnits).Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}($*$)")
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

        txtBriefing.Rtf = SupportingFeatures.ConvertMarkdownToRTF(sb.ToString.Trim)
        'SupportingFeatures.FormatMarkdownToRTF(sb.ToString(), txtBriefing)
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
            sb.Append("**Full Description**($*$)")
            sb.Append(_sessionData.LongDescription)
            txtFullDescription.Rtf = SupportingFeatures.ConvertMarkdownToRTF(sb.ToString.Trim)
            'SupportingFeatures.FormatMarkdownToRTF(sb.ToString.Trim, txtFullDescription)
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
        waypointCoordinatesDataGrid.Columns("MinAltFeet").Visible = False
        waypointCoordinatesDataGrid.Columns("MaxAltFeet").Visible = False
        waypointCoordinatesDataGrid.Columns("DiameterMeters").Visible = False
        waypointCoordinatesDataGrid.Columns("FullATCId").Visible = False
        waypointCoordinatesDataGrid.Columns("ContainsRestriction").Visible = False
        waypointCoordinatesDataGrid.Columns("PossibleElevationUpdateReq").Visible = False

        waypointCoordinatesDataGrid.Columns("Gate").HeaderText = PrefUnits.GateLabel
        waypointCoordinatesDataGrid.Columns("IsAAT").HeaderText = "AAT"
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

        If Not EventIsEnabled Then
            sb.Append($"There is currently no group flight or event defined with this task. ($*$)")
            CountDownReset()
        Else
            'Group/Club Name
            If _sessionData.URLGroupEventPost.Length > 0 Then
                Dim discordURL As String = SupportingFeatures.DiscordURL(_sessionData.URLGroupEventPost)
                sb.Append($"**[{SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.GroupClubName)} - {SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.EventTopic)}]({discordURL})**($*$)")
            Else
                sb.Append($"**{SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.GroupClubName)} - {SupportingFeatures.ConvertToUnicodeDecimal(_sessionData.EventTopic)}**($*$)")
            End If

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
            sb.Append($"The local times displayed here are for the timezone: **{timezoneInfos(0)} (UTC{timezoneInfos(1)})**($*$)")
            sb.Append("($*$)")

            'Date
            sb.Append($"Event Date: **{fullMeetDateTimeLocal.ToString("dddd, MMMM d, yyyy", CultureInfo.CurrentCulture)}**($*$)")
            sb.Append("($*$)")

            'Meeting Time and Discord Voice Channel
            If _sessionData.VoiceChannel <> String.Empty Then
                sb.Append($"We meet at: **{fullMeetDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)}** your local time ({Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu) ")
                sb.Append($"on voice channel: **{SupportingFeatures.GetTextPartFromURLMarkdown(_sessionData.VoiceChannel)}**($*$)")
                sb.Append("($*$)")
            End If

            'MSFS Server
            If _sessionData.MSFSServer > 0 Then
                sb.Append($"Set MSFS Server to: **{_SF.GetMSFSServers.ElementAt(_sessionData.MSFSServer)}**($*$)")
                sb.Append("($*$)")
            End If

            'Weather ad flight plan
            AppendEventWeatherPresetLines(sb)
            sb.Append($"And flight plan: **""{Path.GetFileName(_sessionData.FlightPlanFilename)}""**($*$)")
            sb.Append("($*$)")

            Dim dateFormat As String
            If _sessionData.IncludeYear Then
                dateFormat = "MMMM dd, yyyy"
            Else
                dateFormat = "MMMM dd"
            End If

            sb.Append($"The MSFS Local Date & Time should be: **{fullMSFSLocalDateTime.ToString(dateFormat, CultureInfo.CurrentCulture)}, {fullMSFSLocalDateTime.ToString("t", CultureInfo.CurrentCulture)} {SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.SimDateTimeExtraInfo, True, True)}**")
            If _sessionData.UseEventSyncFly Then
                sb.Append($"when it's the Sync Fly time ({fullSyncFlyDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} your local time) ($*$)")
            ElseIf _sessionData.UseEventLaunch Then
                sb.Append($"when it's the Launch time ({fullLaunchDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} your local time) ($*$)")
            Else
                sb.Append($"when it's the Meet time ({fullMeetDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)} your local time) ($*$)")
            End If
            sb.Append("($*$)")

            'Sync Start or not?
            If _sessionData.UseEventSyncFly Then
                sb.Append($"This task requires a **SYNC FLY** so **WAIT** on the World Map for the signal. ($*$)")
                sb.Append($"Sync Fly expected at **{fullSyncFlyDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)}** your local time ({Conversions.ConvertLocalToUTC(fullSyncFlyDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu / {fullSyncFlyDateTimeMSFS.ToString("t", CultureInfo.CurrentCulture)} in MSFS) ($*$)")
                sb.Append("($*$)")

                ' Add cues with their associated embedded resource paths - for Sync Fly countdown
                audioCueDictionary.Clear()
                audioCueDictionary.Add(121, "2MinSyncFly") ' 2 minutes to sync fly
                audioCueDictionary.Add(61, "60SecSyncFly") ' 60 seconds to sync fly
                audioCueDictionary.Add(31, "30SecSyncFly") ' 30 seconds to sync fly
                audioCueDictionary.Add(16, "15Seconds") ' 15 seconds
                audioCueDictionary.Add(11, "From10ToClickFly") ' Countdown from 10 to now!

                countDownToSyncFly.SetTargetDateTime(fullSyncFlyDateTimeLocal, audioCueDictionary)
            Else
                sb.Append($"This task DOES NOT require a SYNC FLY so you can click Fly at your convenience and wait at the airfield. ($*$)")
                sb.Append("($*$)")
                countDownToSyncFly.ResetToZero(True)
            End If

            'Unstandard Barometric pressure
            If Not _WeatherDetails.IsStandardMSLPressure Then
                sb.Append($"Barometric pressure is {_WeatherDetails.MSLPressure(_sessionData.BaroPressureExtraInfo, _sessionData.SuppressBaroPressureWarningSymbol, PrefUnits, False)} ($*$)")
                sb.Append("($*$)")
            End If

            'Launch
            If _sessionData.UseEventLaunch Then
                sb.Append($"Launch/Winch/Tow signal expected at **{fullLaunchDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)}** your local time ({Conversions.ConvertLocalToUTC(fullLaunchDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu / {fullLaunchDateTimeMSFS.ToString("t", CultureInfo.CurrentCulture)} in MSFS) ($*$)")
                sb.Append("($*$)")

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
                sb.Append($"Once at the airfield, launch at your convenience and wait for the task start signal. ($*$)")
                sb.Append("($*$)")
                countDownToLaunch.ResetToZero(True)
            End If

            'Start task
            If _sessionData.UseEventStartTask Then
                sb.Append($"Task start/Start gate opening signal expected at **{fullStartTaskDateTimeLocal.ToString("t", CultureInfo.CurrentCulture)}** your local time ({Conversions.ConvertLocalToUTC(fullStartTaskDateTimeLocal).ToString("t", CultureInfo.CurrentCulture)} Zulu / {fullStartTaskDateTimeMSFS.ToString("t", CultureInfo.CurrentCulture)} in MSFS) ($*$)")
                sb.Append("($*$)")

                ' Add cues with their associated embedded resource paths - for task start
                audioCueDictionary.Clear()
                audioCueDictionary.Add(121, "2MinTaskStart") ' 2 minutes to task start
                audioCueDictionary.Add(61, "60SecTaskStart") ' 60 seconds to task start
                audioCueDictionary.Add(31, "30SecTaskStart") ' 30 seconds to task start
                audioCueDictionary.Add(16, "15Seconds") ' 15 seconds
                audioCueDictionary.Add(11, "From10ToStartTask") ' Countdown from 10 to now!

                countDownTaskStart.SetTargetDateTime(fullStartTaskDateTimeLocal, audioCueDictionary)
            Else
                sb.Append($"There is no specific task start time, you can cross the start gate at your convenience. ($*$)")
                sb.Append("($*$)")
                countDownTaskStart.ResetToZero(True)
            End If
            sb.Append($"The expected duration should be **{SupportingFeatures.GetDuration(_sessionData.DurationMin, _sessionData.DurationMax)}{SupportingFeatures.ValueToAppendIfNotEmpty(_sessionData.DurationExtraInfo, True, True)}**($*$)")
            If _SF.AATMinDuration > TimeSpan.Zero Then
                sb.Append($"This is an **AAT** with a minimum duration of **{SupportingFeatures.FormatTimeSpanAsText(_SF.AATMinDuration)}**($*$)")
            End If
            sb.Append("($*$)")

            If _sessionData.EventDescription <> String.Empty AndAlso _sessionData.EventDescription <> _sessionData.ShortDescription AndAlso _sessionData.EventDescription <> _sessionData.LongDescription Then
                sb.Append($"{_sessionData.EventDescription}")
                sb.Append("($*$)")
                sb.Append("($*$)")
            End If

            sb.Append($"See Main Task Info and Map tabs for other important task information.($*$)")

            Timer1.Start()
        End If

        _loaded = True
        trackAudioCueVolume.Value = SupportingFeatures.ReadRegistryKey("AudioCues", 80)
        If SupportingFeatures.ReadRegistryKey("WeatherGraph", 0) = 1 Then
            chkShowGraph.Checked = True
        Else
            chkShowGraph.Checked = False
        End If

        txtEventInfo.Rtf = SupportingFeatures.ConvertMarkdownToRTF(sb.ToString.Trim)
        'SupportingFeatures.FormatMarkdownToRTF(sb.ToString, txtEventInfo)
        SupportingFeatures.SetZoomFactorOfRichTextBox(txtEventInfo)

    End Sub

    Private Sub LoadImagesTabListView()

        Dim imageFilenameList As New List(Of String)

        imagesListView.BeginUpdate()

        Dim previousImageList As ImageList = imagesListView.LargeImageList
        imagesListView.LargeImageList = Nothing
        imagesListView.Items.Clear()

        If previousImageList IsNot Nothing Then
            For Each existingImage As Image In previousImageList.Images
                existingImage.Dispose()
            Next
            previousImageList.Dispose()
        End If

        Dim imgList As New ImageList With {.ImageSize = New Size(64, 64)}

        For Each extraFile As String In _sessionData.ExtraFiles
            Dim filename As String = extraFile
            Dim extension As String = Path.GetExtension(filename.ToLowerInvariant)

            If extension = ".png" OrElse extension = ".jpg" Then
                If _unpackFolder <> String.Empty Then
                    filename = Path.Combine(_unpackFolder, Path.GetFileName(filename))
                End If

                If File.Exists(filename) Then
                    Dim imageToAdd As Image = Nothing

                    Try
                        Using fileStream As New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                            Using memoryStream As New MemoryStream()
                                fileStream.CopyTo(memoryStream)
                                memoryStream.Position = 0

                                Using loadedImage As Image = Image.FromStream(memoryStream)
                                    imageToAdd = CType(loadedImage.Clone(), Image)
                                End Using
                            End Using
                        End Using
                    Catch ex As Exception
                        imageToAdd = Nothing
                    End Try

                    If imageToAdd IsNot Nothing Then
                        imgList.Images.Add(imageToAdd)
                        imageFilenameList.Add(filename)
                    End If
                End If
            End If
        Next

        imagesListView.View = View.LargeIcon
        imagesListView.LargeImageList = imgList

        For i = 0 To imgList.Images.Count - 1
            Dim item As New ListViewItem(String.Empty)
            item.ImageIndex = i
            item.Text = (i + 1).ToString()
            item.Tag = imageFilenameList(i)
            imagesListView.Items.Add(item)
        Next

        If imagesListView.Items.Count > 0 Then
            imagesListView.Items(0).Selected = True
        End If

        imagesListView.EndUpdate()

    End Sub


    Private Sub SetWPGridColumnsVisibility()

        Try
            waypointCoordinatesDataGrid.Columns("MinAltFeet").Visible = False
            waypointCoordinatesDataGrid.Columns("MaxAltFeet").Visible = False
            waypointCoordinatesDataGrid.Columns("DiameterMeters").Visible = False
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
            ElseIf radioBtn.Name.Contains("Speed") Then
                PrefUnits.Speed = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("GateDiameter") Then
                PrefUnits.GateDiameter = CInt(radioBtn.Tag)
            ElseIf radioBtn.Name.Contains("GateMeasurement") Then
                PrefUnits.GateMeasurement = CInt(radioBtn.Tag)
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

        SupportingFeatures.BuildCloudAndWindLayersDatagrids(_WeatherDetails, windLayersDatagrid, cloudLayersDatagrid, PrefUnits)

    End Sub

    Private Sub btnGotoDiscordTaskThread_Click(sender As Object, e As EventArgs) Handles btnGotoDiscordTaskThread.Click

        If Not SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.TaskLibraryDiscordURL}/{_discordPostID}") Then
            Using New Centered_MessageBox()
                MessageBox.Show("Invalid URL provided! Please specify a valid URL.", "Error launching Discord", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private isRightMousePressed As Boolean = False

    Private Sub btnGotoDiscordTaskThread_MouseDown(sender As Object, e As MouseEventArgs) Handles btnGotoDiscordTaskThread.MouseDown
        If e.Button = MouseButtons.Right Then
            ' Mark that the right mouse button is pressed
            isRightMousePressed = True
        End If
    End Sub

    Private Sub btnGotoDiscordTaskThread_MouseUp(sender As Object, e As MouseEventArgs) Handles btnGotoDiscordTaskThread.MouseUp
        If e.Button = MouseButtons.Right AndAlso isRightMousePressed Then
            ' Check if the right mouse button was pressed and released within the button
            If btnGotoDiscordTaskThread.ClientRectangle.Contains(e.Location) Then
                ' Programmatically trigger the button's click event
                Dim inviteURL As String = String.Empty

                If _discordPostID <> String.Empty Then
                    inviteURL = "https://discord.gg/aW8YYe3HJF"
                End If
                If inviteURL <> String.Empty Then
                    Clipboard.SetText(inviteURL)
                    Using New Centered_MessageBox()
                        MessageBox.Show("The invite link has been copied to your clipboard. Paste it in the Join Discord Server invite field on Discord.", "Invite link copied", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End If
            End If
        End If

        ' Reset the flag when the mouse button is released
        isRightMousePressed = False
    End Sub

    Private Sub rtfControl_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles txtFullDescription.LinkClicked,
                                                                                            txtBriefing.LinkClicked,
                                                                                            txtEventInfo.LinkClicked
        Process.Start(e.LinkText)
    End Sub

    Private Sub flowSetup_Layout(sender As Object, e As LayoutEventArgs) Handles flowSetup.Layout
        For Each ctrl As Control In flowSetup.Controls
            ' Subtract margins so you don't get a horizontal scrollbar
            ctrl.Width = flowSetup.ClientSize.Width - ctrl.Margin.Left - ctrl.Margin.Right
        Next
    End Sub

#End Region

#End Region

End Class

Public NotInheritable Class ValidFilesDragActiveChangedEventArgs
    Inherits EventArgs

    Public Sub New(isActive As Boolean, files As IReadOnlyList(Of String))
        Me.IsActive = isActive
        Me.Files = files
    End Sub

    Public ReadOnly Property IsActive As Boolean

    Public ReadOnly Property Files As IReadOnlyList(Of String)
End Class
