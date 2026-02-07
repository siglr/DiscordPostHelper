Imports System.Collections.Specialized
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary
Imports SIGLR.SoaringTools.DiscordPostHelper.Localization
Imports SIGLR.SoaringTools.ImageViewer

Public Class Main

#Region "Members, Constants and Enums"

    Public Shared SessionSettings As New AllSettings
    Public CurrentSessionFile As String = String.Empty

    Private Enum SourceOfChange
        TaskTab
        EventTab
        DiscordTab
    End Enum

    Private Const DiscordLimit As Integer = 2000
    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"
    Private Const DefaultMapImageWidth As Integer = 1647
    Private Const DefaultMapImageHeight As Integer = 639
    Private Const WSGEmojiForBot As String = "<:wsg:1296813102893105203>"
    Private Const WSGEmojiForUser As String = ":wsg:"
    Private Const MaxExtraFiles As Integer = 10

    Private ReadOnly _SF As New SupportingFeatures
    Private ReadOnly _CurrentDistanceUnit As Integer
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private ReadOnly _FrenchCulture As New CultureInfo("fr-FR")

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _secondaryWeatherPresetName As String = String.Empty
    Private _WeatherPresetBrowser As New WeatherPresetBrowser
    Private _sscPresetName As String = String.Empty
    Private _primaryWPRFilename As String = String.Empty
    Private _secondaryWPRFilename As String = String.Empty
    Private _ClubPreset As PresetEvent = Nothing
    Private _GuideCurrentStep As Integer = 0
    Private _FlightTotalDistanceInKm As Double = 0
    Private _TaskTotalDistanceInKm As Double = 0
    Private _loadingFile As Boolean = False
    Private _sessionModified As Boolean = False
    Private _taskModified As Boolean = False
    Private _eventModified As Boolean = False
    Private _discordModified As Boolean = False
    Private _PossibleElevationUpdateRequired As Boolean = False
    Private _timeStampContextualMenuDateTime As DateTime
    Private _TaskEntrySeqID As Integer = 0
    Private _TaskStatus As SupportingFeatures.WSGTaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
    Private _TBTaskDBEntryUpdate As DateTime
    Private _TBTaskLastUpdate As DateTime
    Private _taskDiscordPostID As String = String.Empty
    Private _isInitiatizing As Boolean = True
    Private _directChange As Boolean = True
    Private _eventDateTimeChangeInProgress As Boolean = False
    Private _pastEventConfirmed As Boolean = False
    Private _temporaryRightToUpdateTask As Boolean = False
    Private _hasLeaderBoard As Boolean = False

    ' User related variables
    Private _userPermissionID As String
    Private _userPermissions As New Dictionary(Of String, Boolean)
    Private _userName As String = String.Empty
    Private _allAvailableUsers As New List(Of String)
    Private _isSuperUser As Boolean = False
    Private _userInteraction As Boolean = False
    Private _previousOwner As String = String.Empty
    Private _currentWSGTaskOwner As String = String.Empty
    Private _currentWSGSharedWith As New List(Of String)
    Private _useTestMode As Boolean = False

    Private _OriginalFlightPlanTitle As String = String.Empty
    Private _OriginalFlightPlanDeparture As String = String.Empty
    Private _OriginalFlightPlanDepRwy As String = String.Empty
    Private _OriginalFlightPlanArrival As String = String.Empty
    Private _OriginalFlightPlanShortDesc As String = String.Empty

#End Region

#Region "Global exception handler"

    Public Sub New()
        ' Subscribe to global exception handlers
        AddHandler Application.ThreadException, AddressOf Application_ThreadException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException

        ' Initialize the form and other components
        InitializeComponent()
    End Sub

    Private Sub Application_ThreadException(ByVal sender As Object, ByVal e As ThreadExceptionEventArgs)
        ' Handle the exception
        GlobalExceptionHandler(e.Exception)
    End Sub

    Private Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        ' Handle the exception
        GlobalExceptionHandler(e.ExceptionObject)
    End Sub

    Private Sub GlobalExceptionHandler(e As Exception)

        Dim CommonLibraryAssembly As Assembly = GetType(SupportingFeatures).Assembly
        Dim ImageViewerAssembly As Assembly = GetType(ImageBox).Assembly

        Clipboard.SetText($"{Me.GetType.Assembly.GetName.ToString}{vbCrLf}{CommonLibraryAssembly.GetName.ToString}{vbCrLf}{ImageViewerAssembly.GetName.ToString}{vbCrLf}{vbCrLf}{e.Message}{vbCrLf}{vbCrLf}{e.ToString}")
        MessageBox.Show($"An unhandled exception occurred and all details have been added to your clipboard.{vbCrLf}Please paste the content in the 'questions-support' channel on Discord.{vbCrLf}{vbCrLf}{Clipboard.GetText}", "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Application.Exit()
    End Sub

#End Region

#Region "Startup"

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not _SF.CheckRequiredNetFrameworkVersion Then
            MessageBox.Show("This application requires Microsoft .NET Framework 4.8 or later to be present.", "Installation does not meet requirement", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Application.Exit()
            Exit Sub
        End If

        Dim briefingContext As New BriefingRenderContext() With {
            .HostMode = BriefingHostMode.DesignerPreview,
            .InstalledSims = InstalledSimFlags.MSFS2020 Or InstalledSimFlags.MSFS2024,
            .PresetNameDisplayMode = PresetNameDisplayMode.Friendly
        }
        BriefingControl1.RenderContext = briefingContext

        _SF.FillCountryFlagList(cboCountryFlag.Items)

        'Get the permission ID
        _userPermissionID = GetUserIDFromPermissionsFile()

        If _userPermissionID = String.Empty Then
            ToggleTestMode(True)
        Else
            ToggleTestMode(False)
        End If
        chkTestMode.Checked = _useTestMode

        ResetForm()

        SetTimePickerFormat()
        SetDatePickerFormat()

        SessionSettings.Load()

        RestoreMainFormLocationAndSize()

        If SessionSettings.WaitSecondsForFiles >= 1 AndAlso SessionSettings.WaitSecondsForFiles <= 10 Then
            numWaitSecondsForFiles.Value = SessionSettings.WaitSecondsForFiles
        End If

        'Load clubs guides
        cboBeginnersGuide.Items.Clear()
        cboBeginnersGuide.Items.Add("None")
        For Each club As PresetEvent In _SF.DefaultKnownClubEvents.Values
            If Not cboBeginnersGuide.Items.Contains(club.BeginnerLink) Then
                cboBeginnersGuide.Items.Add(club.BeginnerLink)
            End If
        Next
        cboBeginnersGuide.Items.Add("Other (provide link below)")

        LoadDPOptions()
        LoadDGPOptions()

        chkRemindUserPostOptions.Checked = SessionSettings.RemindUserPostOptions

        If My.Application.CommandLineArgs.Count > 0 Then
            ' Open the file passed as an argument
            CurrentSessionFile = My.Application.CommandLineArgs(0)
            'Check if the selected file is a dph or dphx files
            If Path.GetExtension(CurrentSessionFile) = ".dphx" Then
                'Package - we need to unpack it first
                OpenFileDialog1.FileName = _SF.UnpackDPHXFile(CurrentSessionFile)

                If OpenFileDialog1.FileName = String.Empty Then
                    CurrentSessionFile = String.Empty
                Else
                    CurrentSessionFile = OpenFileDialog1.FileName
                End If
            End If
        Else
            'Load previous session data
            If Not SessionSettings.LastFileLoaded = String.Empty Then
                CurrentSessionFile = SessionSettings.LastFileLoaded
            End If
        End If
        Me.Refresh()
        _loadingFile = True
        LoadSessionData(CurrentSessionFile)
        _loadingFile = False
        UpdateWeatherPresetButtonState()
        CheckForNewVersion()

        _isInitiatizing = False

    End Sub

    Public Function GetUserRights(userID As String) As JObject
        Try
            Dim rightsUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveUserRights.php"

            ' Create the web request
            Dim request As HttpWebRequest = CType(WebRequest.Create(rightsUrl), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"

            ' Prepare the data to send
            Dim postData As String = "user_id=" & userID
            Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes(postData)
            request.ContentLength = byteArray.Length

            ' Write data to request stream
            Using dataStream As Stream = request.GetRequestStream()
                dataStream.Write(byteArray, 0, byteArray.Length)
            End Using

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    ' Check the status
                    If result("status").ToString() = "success" Then
                        _userName = result("name")

                        lblWSGUserName.Text = _userName
                        chkcboSharedWithUsers.LockedValueFromUser = _userName 'User will not be able to uncheck himself from shared owners
                        If _userName = "MajorDad" Then
                            _isSuperUser = True
                        Else
                            _isSuperUser = False
                        End If

                        _allAvailableUsers = result("allUserNames") _
                            .Select(Function(user) user.ToString()) _
                            .ToList()

                        For Each username As String In _allAvailableUsers
                            cboTaskOwner.Items.Add(username)
                            If username <> _userName Then
                                chkcboSharedWithUsers.AddItem(username, False)
                            End If
                        Next
                        cboTaskOwner.SelectedItem = _userName
                        chkcboSharedWithUsers.IsReadOnly = False
                        Return result("rights")
                    Else
                        Throw New Exception("Error retrieving user rights: " & result("message").ToString())
                    End If
                End Using
            End Using

        Catch ex As Exception
            Throw New Exception("Error: " & ex.Message)
        End Try
    End Function

    Public Function GetUserIDFromPermissionsFile() As String
        Dim filePath As String = $"{Application.StartupPath}\UserPermissions.key"
        Dim userID As String = String.Empty
        Dim rightsObject As JObject

        ' Check if the file exists
        If File.Exists(filePath) Then
            Try
                ' Load the XML document
                Dim xmlDoc As New XmlDocument()
                xmlDoc.Load(filePath)

                ' Retrieve the UserID from the XML
                Dim userNode As XmlNode = xmlDoc.SelectSingleNode("/User/ID")
                If userNode IsNot Nothing Then
                    userID = userNode.InnerText

                    'Retrieve rights from the server
                    rightsObject = GetUserRights(userID)

                    ' Iterate through the rightsObject properties and fill the dictionary
                    For Each right As JProperty In rightsObject.Properties()
                        _userPermissions.Add(right.Name, right.Value.ToObject(Of Boolean)())
                    Next

                Else
                    Throw New Exception("User ID not found in the XML file.")
                End If

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show($"Error reading UserPermissions.key file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End Try
        End If

        Return userID

    End Function

    Private Sub LoadDPOptions()

        If SessionSettings.DPO_DPOUseCustomSettings Then
            chkDPOIncludeCoverImage.Checked = SessionSettings.DPO_chkDPOIncludeCoverImage
        End If

    End Sub
    Private Sub LoadDGPOptions()

        If SessionSettings.DPO_DGPOUseCustomSettings Then
            chkDGPOCoverImage.Checked = SessionSettings.DPO_chkDGPOCoverImage
            chkDGPOMainGroupPost.Checked = SessionSettings.DPO_chkDGPOMainGroupPost
            chkDGPOThreadCreation.Checked = SessionSettings.DPO_chkDGPOThreadCreation
            chkDGPOTeaser.Checked = SessionSettings.DPO_chkDGPOTeaser
            chkDGPOMapImage.Checked = SessionSettings.DPO_chkDGPOFilesWithFullLegend
            chkDGPOMainPost.Checked = SessionSettings.DPO_chkDGPOMainPost
            chkDGPOAltRestrictions.Checked = SessionSettings.DPO_chkDGPOAltRestrictions
            chkDGPOFullDescription.Checked = SessionSettings.DPO_chkDGPOFullDescription
            chkDGPOPublishWSGEventNews.Checked = SessionSettings.DPO_chkDGPOPublishWSGEventNews
            chkDGPOEventLogistics.Checked = SessionSettings.DPO_chkDGPOEventLogistics
        End If

    End Sub

    Private Sub RestoreMainFormLocationAndSize()
        Dim sizeString As String = SessionSettings.MainFormSize
        Dim locationString As String = SessionSettings.MainFormLocation

        ' Restore form size
        If sizeString <> "" Then
            Dim sizeArray As String() = sizeString.TrimStart("{").TrimEnd("}").Split(",")
            Dim width As Integer = CInt(sizeArray(0).Split("=")(1))
            Dim height As Integer = CInt(sizeArray(1).Split("=")(1))
            Me.Size = New Size(width, height)
        End If

        ' Restore form location
        If locationString <> "" Then
            Dim locationArray As String() = locationString.TrimStart("{").TrimEnd("}").Split(",")
            Dim x As Integer = CInt(locationArray(0).Split("=")(1))
            Dim y As Integer = CInt(locationArray(1).Split("=")(1))
            Me.Location = New Point(x, y)
        End If

        ' Validate that the form is visible on at least one screen
        Dim formBounds As New Rectangle(Me.Location, Me.Size)
        Dim isVisibleOnAnyScreen As Boolean = False

        For Each screen In Windows.Forms.Screen.AllScreens
            If screen.WorkingArea.IntersectsWith(formBounds) AndAlso
           screen.Bounds.Width > 0 AndAlso
           screen.Bounds.Height > 0 Then
                isVisibleOnAnyScreen = True
                Exit For
            End If
        Next

        ' If the form is not visible, reset to default position and size
        If Not isVisibleOnAnyScreen Then
            Me.Left = 0
            Me.Top = 0
            Me.WindowState = FormWindowState.Normal
        End If

        ' Restore splitter position
        FlightPlanTabSplitter.SplitPosition = SessionSettings.FlightPlanTabSplitterLocation
    End Sub

    Private Sub SetTimePickerFormat()

        Dim dtfi As DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat
        Dim timeFormatToUse As String

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
        dtAvailabilityTime.CustomFormat = timeFormatToUse

    End Sub

    Private Sub SetDatePickerFormat()

        ' Force the user's long date format but without the day-of-week.
        Dim dtfi As DateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat
        Dim datePattern As String = dtfi.LongDatePattern
        ' Remove the day-of-week token ("dddd") and any trailing comma/space.
        Dim dateFormatToUse As String = Regex.Replace(datePattern, "dddd[, ]*", "").Trim()

        dtSimDate.CustomFormat = dateFormatToUse
        dtRepostOriginalDate.CustomFormat = dateFormatToUse
        dtEventMeetDate.CustomFormat = dateFormatToUse
        dtEventSyncFlyDate.CustomFormat = dateFormatToUse
        dtEventLaunchDate.CustomFormat = dateFormatToUse
        dtEventStartTaskDate.CustomFormat = dateFormatToUse
        dtAvailabilityDate.CustomFormat = dateFormatToUse

    End Sub

    Private Sub ResetForm()

        _loadingFile = True
        CurrentSessionFile = String.Empty
        UpdateWeatherPresetButtonState()

        BriefingControl1.FullReset()
        lblCountriesTestModeMsg.Visible = _useTestMode

        _XmlDocFlightPlan = New XmlDocument
        _XmlDocWeatherPreset = New XmlDocument
        _WeatherDetails = Nothing
        _secondaryWeatherPresetName = String.Empty
        _sscPresetName = String.Empty
        _primaryWPRFilename = String.Empty
        _secondaryWPRFilename = String.Empty
        _FlightTotalDistanceInKm = 0
        _TaskTotalDistanceInKm = 0
        _PossibleElevationUpdateRequired = False
        _TaskEntrySeqID = 0
        _taskDiscordPostID = String.Empty
        lblElevationUpdateWarning.Visible = _PossibleElevationUpdateRequired
        lblDiscordTaskPostID.Text = "❌ Discord Post ID"
        lblWSGTaskEntrySeqID.Text = "❌ WeSimGlide.org ID: None"
        lblEventClubNotAuthorized.Visible = False
        lblPrePublishingWarning.Visible = False
        _hasLeaderBoard = False

        cboDifficulty.SelectedIndex = 0
        cboVoiceChannel.Items.Clear()
        cboVoiceChannel.Items.AddRange(_SF.GetVoiceChannels.ToArray)
        cboMSFSServer.Items.Clear()
        cboMSFSServer.Items.AddRange(_SF.GetMSFSServers.ToArray)
        cboRecommendedGliders.SelectedIndex = 0
        lstAllFiles.Items.Clear()
        lstAllCountries.Items.Clear()
        lstAllRecommendedAddOns.Items.Clear()

        txtFlightPlanFile.Text = String.Empty
        txtWeatherFile.Text = String.Empty
        txtTitle.Text = String.Empty
        chkTitleLock.Checked = False
        chkIncludeYear.Checked = False
        txtSimDateTimeExtraInfo.Text = String.Empty
        txtMainArea.Text = String.Empty
        txtDepartureICAO.Text = String.Empty
        txtDepName.Text = String.Empty
        txtDepExtraInfo.Text = String.Empty
        chkDepartureLock.Checked = False
        txtArrivalICAO.Text = String.Empty
        txtArrivalName.Text = String.Empty
        txtArrivalExtraInfo.Text = String.Empty
        chkArrivalLock.Checked = False
        chkSoaringTypeRidge.Checked = False
        chkSoaringTypeDynamic.Checked = False
        chkSoaringTypeThermal.Checked = False
        chkSoaringTypeWave.Checked = False
        txtSoaringTypeExtraInfo.Text = String.Empty
        txtDistanceTotal.Text = String.Empty
        txtDistanceTrack.Text = String.Empty
        txtDurationExtraInfo.Text = String.Empty
        txtDurationMin.Text = String.Empty
        txtDurationMax.Text = String.Empty
        txtAATTask.Text = String.Empty
        txtDifficultyExtraInfo.Text = String.Empty
        txtShortDescription.Text = String.Empty
        chkDescriptionLock.Checked = False
        chkSuppressWarningForBaroPressure.Checked = False
        txtBaroPressureExtraInfo.Text = "Non standard: Set your altimeter! (Press ""B"" once in your glider)"
        txtCredits.Text = "All credits to @UserName for this task."
        If SessionSettings.TaskDescriptionTemplate Is Nothing Then
            txtLongDescription.Text = String.Empty
        Else
            txtLongDescription.Text = SessionSettings.TaskDescriptionTemplate.Replace("($*$)", Environment.NewLine)
        End If
        If SessionSettings.EventDescriptionTemplate Is Nothing Then
            txtEventDescription.Text = String.Empty
        Else
            txtEventDescription.Text = SessionSettings.EventDescriptionTemplate.Replace("($*$)", Environment.NewLine)
        End If
        txtWeatherSummary.Text = String.Empty
        txtAltRestrictions.Text = String.Empty
        txtWeatherFirstPart.Text = String.Empty
        txtWeatherWinds.Text = String.Empty
        txtWeatherClouds.Text = String.Empty
        txtFullDescriptionResults.Text = String.Empty
        cboGroupOrClubName.SelectedIndex = -1
        txtClubFullName.Text = String.Empty
        txtClubFullName.ReadOnly = True
        txtTrackerGroup.Text = String.Empty
        lblGroupEmoji.Text = String.Empty
        cboMSFSServer.SelectedIndex = -1
        cboVoiceChannel.SelectedIndex = -1
        chkDateTimeUTC.Checked = True
        chkUseSyncFly.Checked = False
        chkUseLaunch.Checked = False
        chkUseStart.Checked = False
        txtEventDescription.Text = String.Empty
        cboEligibleAward.SelectedIndex = -1
        txtGroupEventPostURL.Text = String.Empty
        txtRepostOriginalURL.Text = String.Empty
        txtDPHXPackageFilename.Text = String.Empty
        txtAddOnsDetails.Text = String.Empty
        txtWaypointsDetails.Text = String.Empty
        chkLockCoverImage.Checked = False
        chkLockMapImage.Checked = False
        cboBriefingMap.Items.Clear()
        cboCoverImage.Items.Clear()
        txtEventTitle.Text = String.Empty
        chkActivateEvent.Checked = False
        grpGroupEventPost.Enabled = False
        grpDiscordGroupFlight.Enabled = False
        cboBeginnersGuide.Text = "The Beginner's Guide to Soaring Events (GotGravel)"
        txtTaskID.Text = String.Empty
        txtEventTeaserAreaMapImage.Text = String.Empty
        txtEventTeaserMessage.Text = String.Empty
        chkEventTeaser.Checked = False

        btnRemoveExtraFile.Enabled = False
        btnExtraFileDown.Enabled = False
        btnExtraFileUp.Enabled = False
        chkSuppressWarningForBaroPressure.Enabled = False
        txtBaroPressureExtraInfo.Enabled = False
        lblNonStdBaroPressure.Enabled = False
        chkRepost.Checked = False
        txtLastUpdateDescription.Text = String.Empty

        cboTaskOwner.SelectedItem = _userName
        _currentWSGSharedWith.Clear()
        _currentWSGTaskOwner = _userName
        chkcboSharedWithUsers.IsInitializing = True
        chkcboSharedWithUsers.IsReadOnly = False
        chkcboSharedWithUsers.RemoveItem(_userName)
        chkcboSharedWithUsers.SelectNone()
        chkcboSharedWithUsers.IsInitializing = False
        btnStartTaskPost.Enabled = False
        tabFlightPlan.Enabled = True
        btnStartFullPostingWorkflow.Enabled = False
        chkDelayedAvailability.Checked = False
        chkDelayBasedOnEvent.Checked = True
        txtMinutesBeforeMeeting.Text = "1"
        cboDelayUnits.SelectedIndex = 0
        chkAvailabilityRefly.Checked = False
        btnSyncFlightPlanTitle.Enabled = False
        btnSyncWeatherTitle.Enabled = False
        lstEventsFeaturingTask.Items.Clear()

        _SF.PopulateSoaringClubList(cboGroupOrClubName.Items)
        _SF.PopulateKnownDesignersList(cboKnownTaskDesigners.Items)
        _SF.AllWaypoints.Clear()

        'BuildFPResults()
        'BuildGroupFlightPost()
        SetFormCaption(String.Empty)
        FixForDropDownCombos()
        SetTBTaskDetailsLabel()

        _loadingFile = False
        SessionUntouched()

    End Sub

    Public Sub CheckForNewVersion()
        Dim myVersionInfo As VersionInfo = _SF.GetVersionInfo()
        Dim message As String = String.Empty

        If myVersionInfo IsNot Nothing Then
            If _SF.FormatVersionNumber(myVersionInfo.CurrentLatestVersion) > _SF.FormatVersionNumber(Me.GetType.Assembly.GetName.Version.ToString) Then
                'New version available
                If _SF.ShowVersionForm(myVersionInfo, Me.GetType.Assembly.GetName.Version.ToString) = DialogResult.Yes Then
                    'update
                    'Download the file
                    If _SF.DownloadLatestUpdate(myVersionInfo.CurrentLatestVersion, message) Then
                        Application.Exit()
                    Else
                        'Show error updating
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show(Me, $"An error occured during the update process at this step:{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}The update did not complete.", "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using
                    End If
                End If
            End If
        End If

    End Sub

#End Region

#Region "Global form"

#Region "Event handlers"

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not CheckUnsavedAndConfirmAction("exit") Then
            e.Cancel = True
        Else
            SessionSettings.RemindUserPostOptions = chkRemindUserPostOptions.Checked
            SessionSettings.MainFormSize = Me.Size.ToString()
            SessionSettings.MainFormLocation = Me.Location.ToString()
            SessionSettings.WaitSecondsForFiles = numWaitSecondsForFiles.Value
            SessionSettings.LastFileLoaded = CurrentSessionFile
            SessionSettings.FlightPlanTabSplitterLocation = FlightPlanTabSplitter.SplitPosition
            SessionSettings.Save()
            BriefingControl1.Closing()
        End If

    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles toolStripResetAll.Click
        If CheckUnsavedAndConfirmAction("reset all") Then
            ResetForm()
            mainTabControl.SelectTab(0)
            Select Case mainTabControl.SelectedTab.Name
                Case "tabDiscord"
                    BuildFPResults()
                    BuildWeatherCloudLayers()
                    BuildWeatherWindLayers()
                    BuildWeatherInfoResults()
                    CheckWhichOptionsCanBeEnabled()
            End Select
        End If

    End Sub

    Private Sub grbTaskInfo_EnabledChanged(sender As Object, e As EventArgs) Handles grbTaskInfo.EnabledChanged,
                                                                                     grpGroupEventPost.EnabledChanged

        FixForDropDownCombos()

    End Sub

    Private Sub EnterTextBox(sender As Object, e As EventArgs) Handles txtWeatherSummary.Enter, txtTitle.Enter, txtSoaringTypeExtraInfo.Enter, txtSimDateTimeExtraInfo.Enter, txtShortDescription.Enter, txtMainArea.Enter, txtLongDescription.Enter, txtEventTitle.Enter, txtEventDescription.Enter, txtDurationMin.Enter, txtDurationMax.Enter, txtDurationExtraInfo.Enter, txtDifficultyExtraInfo.Enter, txtDepName.Enter, txtDepExtraInfo.Enter, txtCredits.Enter, txtArrivalName.Enter, txtArrivalExtraInfo.Enter, txtBaroPressureExtraInfo.Enter, txtOtherBeginnerLink.Enter, txtEventTeaserMessage.Enter, txtClubFullName.Enter, txtTrackerGroup.Enter, txtMinutesBeforeMeeting.Enter
        SupportingFeatures.EnteringTextBox(sender)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles mainTabControl.SelectedIndexChanged
        FixForDropDownCombos()
        Select Case mainTabControl.SelectedTab.Name
            Case "tabBriefing"
                GenerateBriefing()
            Case "tabDiscord"
                BuildFPResults()
                BuildWeatherCloudLayers()
                BuildWeatherWindLayers()
                BuildWeatherInfoResults()
                BuildGroupFlightPost()
                SetEventLabelAndButtons()
                CheckTestModes()
                CheckWhichOptionsCanBeEnabled()
                CalculateTaskAvailability()
        End Select
    End Sub

    Private Sub txtBaroPressureExtraInfo_DoubleClick(sender As Object, e As EventArgs) Handles txtBaroPressureExtraInfo.DoubleClick

        If txtBaroPressureExtraInfo.Text = String.Empty Then
            txtBaroPressureExtraInfo.Text = "Non standard: Set your altimeter! (Press ""B"" once in your glider)"
        End If

    End Sub

    Private Sub txtCredits_DoubleClick(sender As Object, e As EventArgs) Handles txtCredits.DoubleClick

        If txtCredits.Text = String.Empty Then
            txtCredits.Text = "All credits to @UserName for this task."
        End If

    End Sub

    Private Sub DiscordInviteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DiscordInviteToolStripMenuItem.Click

        Dim inviteURL As String = "https://discord.gg/aW8YYe3HJF"
        Clipboard.SetText(inviteURL)
        Using New Centered_MessageBox()
            MessageBox.Show("The invite link has been copied to your clipboard. Paste it in the Join Discord Server invite field on Discord.", "Invite link copied", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using

    End Sub

    Private Sub DiscordChannelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DiscordChannelToolStripMenuItem.Click

        SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/1022705603489042472/1068587750862893117")

    End Sub

    Private Sub GoToFeedbackChannelOnDiscordToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoToFeedbackChannelOnDiscordToolStripMenuItem.Click

        SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/1022705603489042472/1068587681531035781")

    End Sub

    Private Sub toolStripDiscordTaskLibrary_Click(sender As Object, e As EventArgs) Handles toolStripDiscordTaskLibrary.Click

        SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/1022705603489042472/1155511739799060552")

    End Sub

    Private Sub toolStripReload_Click(sender As Object, e As EventArgs) Handles toolStripReload.Click

        If CheckUnsavedAndConfirmAction("discard changes and reload current file") Then
            Dim currentFile As String = CurrentSessionFile
            ResetForm()
            LoadFile(currentFile)
            mainTabControl.SelectTab(0)
            Select Case mainTabControl.SelectedTab.Name
                Case "tabDiscord"
                    BuildFPResults()
                    BuildWeatherCloudLayers()
                    BuildWeatherWindLayers()
                    BuildWeatherInfoResults()
                    CheckWhichOptionsCanBeEnabled()
            End Select
        End If

    End Sub

#End Region

#Region "Global form subs & functions"

    Private Sub SaveSession()

        If txtTitle.Text.Trim = String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "currentTaskWayPoints title is required before saving!", "Title required", vbOKOnly, vbCritical)
            End Using
            Return
        End If

        'Check if user has the right to save / publish when an event is selected and he is not authorizede
        If lblEventClubNotAuthorized.Enabled Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "You are not authorized to save this file with event from this soaring club!", "Not authorized", vbOKOnly, vbCritical)
            End Using
            Return
        End If

        Dim saveWithoutAsking As Boolean = False
        Dim result As DialogResult
        Dim DPHFilename As String = txtTitle.Text
        Dim DPHXFilename As String = String.Empty

        If Not txtDPHXPackageFilename.Text = String.Empty Then
            DPHXFilename = Path.GetFileNameWithoutExtension(txtDPHXPackageFilename.Text)
        End If

        If DPHFilename = DPHXFilename Then
            saveWithoutAsking = True
        End If

        If saveWithoutAsking Then
            DPHXFilename = $"{Path.GetDirectoryName(CurrentSessionFile)}\{DPHXFilename}.dphx"
            DPHFilename = $"{Path.GetDirectoryName(CurrentSessionFile)}\{DPHFilename}.dph"
        Else
            If txtFlightPlanFile.Text = String.Empty Then
                SaveFileDialog1.InitialDirectory = SessionSettings.LastUsedFileLocation
            Else
                SaveFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
            End If
            SaveFileDialog1.FileName = txtTitle.Text
            SaveFileDialog1.Title = "Select session file to save"
            SaveFileDialog1.Filter = "Discord Post Helper Session|*.dph"
            result = SaveFileDialog1.ShowDialog()
            If result = DialogResult.OK Then
                SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(SaveFileDialog1.FileName)
                DPHFilename = SaveFileDialog1.FileName
                DPHXFilename = $"{Path.GetDirectoryName(SaveFileDialog1.FileName)}\{Path.GetFileNameWithoutExtension(SaveFileDialog1.FileName)}.dphx"
            End If
        End If

        If saveWithoutAsking OrElse result = DialogResult.OK Then
            txtDPHXPackageFilename.Text = DPHXFilename

            SaveSessionData(DPHFilename)
            CurrentSessionFile = DPHFilename

            'Then save the DPHX as well
            'Check if file already exists and delete it
            If File.Exists(DPHXFilename) Then
                File.Delete(DPHXFilename)
            End If

            ' Zip the selected files using the ZipFiles method
            Dim filesToInclude As New List(Of String)()
            filesToInclude.Add(CurrentSessionFile)
            If File.Exists(txtFlightPlanFile.Text) Then
                filesToInclude.Add(txtFlightPlanFile.Text)
            End If
            If File.Exists(txtWeatherFile.Text) Then
                filesToInclude.Add(txtWeatherFile.Text)
            End If
            If _secondaryWPRFilename <> String.Empty AndAlso File.Exists(_secondaryWPRFilename) Then
                filesToInclude.Add(_secondaryWPRFilename)
            End If
            For i As Integer = 0 To lstAllFiles.Items.Count - 1
                filesToInclude.Add(lstAllFiles.Items(i))
            Next

            _SF.CreateDPHXFile(DPHXFilename, filesToInclude)

            SessionUntouched()
            SetTBTaskDetailsLabel()

        End If
    End Sub

    Public Function CheckUnsavedAndConfirmAction(action As String) As Boolean

        If _sessionModified Then
            ' Display a confirmation dialog to the user
            Dim result As DialogResult
            Using New Centered_MessageBox(Me)
                result = MessageBox.Show($"There are unsaved changes. Are you sure you want to {action} ?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            End Using

            ' If the user chooses not to exit, cancel the form closing
            If result = DialogResult.No Then
                Return False
            Else
                Return True
            End If
        Else
            Return True
        End If

    End Function

    Public Sub SetFormCaption(filename As String)

        If filename = String.Empty Then
            filename = "No session file loaded"
        End If

        'Add version to form title
        Me.Text = $"Discord Post Helper v{Me.GetType.Assembly.GetName.Version} - {filename}"

    End Sub

    Private Sub LeavingTextBox(txtbox As Windows.Forms.TextBox)
        txtbox.SelectionLength = 0
        txtbox.SelectionStart = 0
    End Sub

    Private Function CopyWeatherGraphToClipboard() As Drawing.Image

        Dim control = New FullWeatherGraphPanel
        Dim imageWidth As Integer = 1333
        Dim imageHeight As Integer = 1000
        Dim oldUnits As New PreferredUnits
        Dim tempUnits As New PreferredUnits
        tempUnits.Altitude = PreferredUnits.AltitudeUnits.Both
        tempUnits.WindSpeed = PreferredUnits.WindSpeedUnits.Both
        tempUnits.Temperature = PreferredUnits.TemperatureUnits.Both
        tempUnits.Barometric = PreferredUnits.BarometricUnits.Both
        control.SetWeatherInfo(_WeatherDetails, tempUnits, SupportingFeatures.GetEnUSFormattedDate(dtSimDate.Value, dtSimLocalTime.Value, chkIncludeYear.Checked))

        ' Create a bitmap with the specified size
        Dim bmp As New Bitmap(imageWidth, imageHeight)

        ' Scale the drawing to the specified size
        control.Width = imageWidth
        control.Height = imageHeight

        ' Create a graphics object to draw the control's image
        Using g As Graphics = Graphics.FromImage(bmp)
            ' Draw the control onto the graphics object
            control.DrawToBitmap(bmp, New Rectangle(0, 0, imageWidth, imageHeight))
        End Using

        ' Set the bitmap to the clipboard
        Clipboard.SetImage(bmp)

        'Reset the preferred units
        tempUnits.Altitude = oldUnits.Altitude
        tempUnits.WindSpeed = oldUnits.WindSpeed

        Return bmp

    End Function

#End Region

#End Region

#Region "Flight Plan Tab"

#Region "Event Handlers"

    Private Sub cboKnownTaskDesigners_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboKnownTaskDesigners.SelectedIndexChanged

        If cboKnownTaskDesigners.SelectedIndex >= 0 Then
            Dim designerSelected As String = cboKnownTaskDesigners.Text.Substring(0, cboKnownTaskDesigners.Text.LastIndexOf("#")).Trim
            txtCredits.Text = $"All credits to @{designerSelected} for this task."
        End If

    End Sub

    Private Sub FileDropZone1_FilesDropped(sender As Object, e As FilesDroppedEventArgs) Handles FileDropZone1.FilesDropped

        Dim droppedFlightPlan As String = String.Empty
        Dim droppedWeather As String = String.Empty
        Dim droppedDPH As String = String.Empty
        Dim droppedDPHX As String = String.Empty
        Dim droppedOtherFiles As List(Of String) = New List(Of String)
        Dim invalidLinkEntries As New List(Of String)

        ' Iterate through the array of dropped file paths or links
        For Each entry As String In e.DroppedEntries
            If String.IsNullOrWhiteSpace(entry) Then
                Continue For
            End If

            Dim trimmedEntry As String = entry.Trim()
            Dim extension As String = GetEntryExtension(trimmedEntry)

            If String.IsNullOrWhiteSpace(extension) Then
                If IsWebLink(trimmedEntry) Then
                    invalidLinkEntries.Add(trimmedEntry)
                End If
                Continue For
            End If

            Select Case extension.ToUpperInvariant()
                Case ".PLN"
                    droppedFlightPlan = trimmedEntry
                Case ".WPR"
                    droppedWeather = trimmedEntry
                Case ".DPH"
                    If IsWebLink(trimmedEntry) Then
                        invalidLinkEntries.Add(trimmedEntry)
                    Else
                        droppedDPH = SupportingFeatures.SanitizeFilePath(trimmedEntry)
                    End If
                Case ".DPHX"
                    If IsWebLink(trimmedEntry) Then
                        invalidLinkEntries.Add(trimmedEntry)
                    Else
                        droppedDPHX = SupportingFeatures.SanitizeFilePath(trimmedEntry)
                    End If
                Case ".JPG", ".PNG"
                    droppedOtherFiles.Add(trimmedEntry)
                Case Else
                    If IsWebLink(trimmedEntry) Then
                        invalidLinkEntries.Add(trimmedEntry)
                    Else
                        droppedOtherFiles.Add(trimmedEntry)
                    End If
            End Select
        Next

        If invalidLinkEntries.Count > 0 Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "Only PLN, WPR, JPG and PNG direct links can be dropped here.", "Invalid link drop", vbOKOnly, MessageBoxIcon.Error)
            End Using
            Return
        End If

        'Handle all invalid cases
        Dim invalidMessage As String = String.Empty
        If droppedDPH <> String.Empty AndAlso droppedDPHX <> String.Empty Then
            invalidMessage = "Either one DPH or one DPHX files can be dropped here, not both."
        End If
        If (droppedDPH <> String.Empty OrElse droppedDPHX <> String.Empty) AndAlso
                (droppedFlightPlan <> String.Empty OrElse
                droppedWeather <> String.Empty OrElse
                droppedOtherFiles.Count > 0) Then
            invalidMessage = "currentTaskWayPoints DPH or DPHX file cannot be dropped along with any other files."
        End If
        If Not grbTaskInfo.Enabled AndAlso droppedFlightPlan = String.Empty AndAlso (droppedWeather <> String.Empty OrElse droppedOtherFiles.Count > 0) Then
            invalidMessage = "There is no flight plan loaded or dropped to load, so no other files can be specified in an empty session."
        End If
        If CurrentSessionFile = String.Empty AndAlso (IsWebLink(droppedWeather) OrElse droppedOtherFiles.Any(Function(fileEntry) IsWebLink(fileEntry))) Then
            invalidMessage = "You need to save a session before dropping weather or image links."
        End If
        If invalidMessage <> String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, invalidMessage, "Invalid file drop", vbOKOnly, MessageBoxIcon.Error)
            End Using
            Return
        End If

        Dim downloadFolder As String = String.Empty
        If CurrentSessionFile <> String.Empty Then
            downloadFolder = Path.GetDirectoryName(CurrentSessionFile)
        ElseIf IsWebLink(droppedFlightPlan) Then
            downloadFolder = PromptForDownloadFolder()
            If String.IsNullOrWhiteSpace(downloadFolder) Then
                Return
            End If
            SessionSettings.LastUsedFileLocation = downloadFolder
        End If

        'Process files
        If droppedDPH <> String.Empty Then
            LoadFile(droppedDPH)
            Return
        End If
        If droppedDPHX <> String.Empty Then
            LoadFile(droppedDPHX)
            Return
        End If
        If droppedFlightPlan <> String.Empty Then
            Dim resolvedFlightPlan = ResolveDroppedEntry(droppedFlightPlan, ".pln", downloadFolder)
            If resolvedFlightPlan <> String.Empty Then
                LoadFlightPlan(resolvedFlightPlan)
            End If
        End If
        If droppedWeather <> String.Empty Then
            Dim resolvedWeather = ResolveDroppedEntry(droppedWeather, ".wpr", downloadFolder)
            If resolvedWeather <> String.Empty Then
                ClearWeatherSelectionsForDrop()
                _primaryWPRFilename = resolvedWeather
                LoadWeatherfile(resolvedWeather)
                UpdateSecondaryWeatherPresetName()
            End If
        End If
        For Each otherFile As String In droppedOtherFiles
            If GetEffectiveExtraFileCountForLimit() >= MaxExtraFiles Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Maximum 10 extra files (map/cover images not included).", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit For
            End If
            Dim resolvedExtraFile As String = ResolveDroppedExtraEntry(otherFile, downloadFolder)
            If resolvedExtraFile <> String.Empty Then
                AddExtraFile(resolvedExtraFile)
            End If
        Next
        LoadPossibleImagesInMapDropdown(cboBriefingMap.SelectedItem)
        LoadPossibleImagesInCoverDropdown(cboCoverImage.SelectedItem)

    End Sub

    Private Function GetEffectiveExtraFileCountForLimit() As Integer
        Dim totalCount As Integer = lstAllFiles.Items.Count
        Dim itemsToExclude As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim mapSelection As String = cboBriefingMap.Text.Trim

        If Not String.IsNullOrEmpty(mapSelection) AndAlso lstAllFiles.Items.Contains(mapSelection) Then
            itemsToExclude.Add(mapSelection)
        End If

        Dim coverSelection As String = cboCoverImage.Text.Trim

        If Not String.IsNullOrEmpty(coverSelection) AndAlso lstAllFiles.Items.Contains(coverSelection) Then
            itemsToExclude.Add(coverSelection)
        End If

        Return Math.Max(0, totalCount - itemsToExclude.Count)
    End Function

    Private Sub AddExtraFile(otherFile As String)
        Dim sanitizedExtraFile As String = SupportingFeatures.SanitizeFilePath(otherFile)

        If String.IsNullOrEmpty(sanitizedExtraFile) Then
            Return
        End If

        If _SF.ExtraFileExtensionIsValid(sanitizedExtraFile) Then
            sanitizedExtraFile = EnsureExtraFilePathReady(sanitizedExtraFile)
            If String.IsNullOrEmpty(sanitizedExtraFile) Then
                Return
            End If

            If Not lstAllFiles.Items.Contains(sanitizedExtraFile) Then
                lstAllFiles.Items.Add(sanitizedExtraFile)
                SessionModified(SourceOfChange.TaskTab)
            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "File already exists in the list.", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "File type cannot be added as it may be unsafe.", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Function EnsureExtraFilePathReady(extraFilePath As String) As String
        Dim extension As String = Path.GetExtension(extraFilePath)
        Dim isImage As Boolean = extension.Equals(".png", StringComparison.OrdinalIgnoreCase) OrElse extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)

        If isImage AndAlso CurrentSessionFile = String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "You need to save a session file before adding image files.", "Session not saved", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return String.Empty
        End If

        If extension.Equals(".png", StringComparison.OrdinalIgnoreCase) Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "This PNG image can be converted to JPG to save space. The original PNG will be deleted. Convert it now?", "Convert PNG to JPG", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Dim sessionFolder As String = Path.GetDirectoryName(CurrentSessionFile)
                    Dim targetPath As String = Path.Combine(sessionFolder, $"{Path.GetFileNameWithoutExtension(extraFilePath)}.jpg")
                    Try
                        If File.Exists(targetPath) Then
                            File.Delete(targetPath)
                        End If

                        ConvertPngToJpgFile(extraFilePath, targetPath, 70)
                        File.Delete(extraFilePath)

                        If lstAllFiles.Items.Contains(extraFilePath) Then
                            lstAllFiles.Items.Remove(extraFilePath)
                        End If

                        Return targetPath
                    Catch ex As Exception
                        MessageBox.Show(Me, $"Unable to convert the PNG file:{Environment.NewLine}{ex.Message}", "Error converting image", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End Using
        End If

        Return extraFilePath
    End Function

    Private Sub ConvertPngToJpgFile(sourcePath As String, targetPath As String, quality As Long)
        Using image As Image = Image.FromFile(sourcePath)
            Dim jpgEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
            Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
            Dim myEncoderParameters As New EncoderParameters(1)
            Dim myEncoderParameter As New EncoderParameter(myEncoder, quality)
            myEncoderParameters.Param(0) = myEncoderParameter

            image.Save(targetPath, jpgEncoder, myEncoderParameters)
        End Using
    End Sub

    Private Function IsWebLink(entry As String) As Boolean
        Dim uri As Uri = Nothing
        Return Uri.TryCreate(entry, UriKind.Absolute, uri) AndAlso (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps)
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

    Private Function ResolveDroppedEntry(entry As String, expectedExtension As String, downloadFolder As String) As String
        If String.IsNullOrWhiteSpace(entry) Then
            Return String.Empty
        End If

        Dim uri As Uri = Nothing
        If Uri.TryCreate(entry.Trim(), UriKind.Absolute, uri) AndAlso (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps) Then
            If Not Path.GetExtension(uri.AbsolutePath).Equals(expectedExtension, StringComparison.OrdinalIgnoreCase) Then
                Return String.Empty
            End If

            If String.IsNullOrWhiteSpace(downloadFolder) Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "A download folder is required before downloading this file.", "Missing download folder", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End If

            Try
                Return DownloadDroppedFile(uri, downloadFolder)
            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to download the dropped file: {ex.Message}", "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End Try
        End If

        Dim sanitizedPath As String = SupportingFeatures.SanitizeFilePath(entry)
        If String.IsNullOrEmpty(sanitizedPath) Then
            Return String.Empty
        End If

        If Not Path.GetExtension(sanitizedPath).Equals(expectedExtension, StringComparison.OrdinalIgnoreCase) Then
            Return String.Empty
        End If

        If File.Exists(sanitizedPath) Then
            Return sanitizedPath
        End If

        Using New Centered_MessageBox(Me)
            MessageBox.Show(Me, $"The dropped file could not be found: {sanitizedPath}", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Using
        Return String.Empty
    End Function

    Private Function ResolveDroppedExtraEntry(entry As String, downloadFolder As String) As String
        If String.IsNullOrWhiteSpace(entry) Then
            Return String.Empty
        End If

        Dim uri As Uri = Nothing
        If Uri.TryCreate(entry.Trim(), UriKind.Absolute, uri) AndAlso (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps) Then
            Dim extension As String = Path.GetExtension(uri.AbsolutePath)
            If Not extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) AndAlso Not extension.Equals(".png", StringComparison.OrdinalIgnoreCase) Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Only JPG or PNG links can be used for extra images.", "Invalid link drop", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End If

            If String.IsNullOrWhiteSpace(downloadFolder) Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "You need to save a session before dropping image links.", "Session not saved", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End If

            Try
                Return DownloadDroppedFile(uri, downloadFolder)
            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to download the dropped file: {ex.Message}", "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return String.Empty
            End Try
        End If

        Dim sanitizedPath As String = SupportingFeatures.SanitizeFilePath(entry)
        If String.IsNullOrEmpty(sanitizedPath) Then
            Return String.Empty
        End If

        If File.Exists(sanitizedPath) Then
            Return sanitizedPath
        End If

        Using New Centered_MessageBox(Me)
            MessageBox.Show(Me, $"The dropped file could not be found: {sanitizedPath}", "File missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Using
        Return String.Empty
    End Function

    Private Function DownloadDroppedFile(uri As Uri, targetFolder As String) As String
        If Not Directory.Exists(targetFolder) Then
            Directory.CreateDirectory(targetFolder)
        End If

        Using client As New WebClient()
            Using stream = client.OpenRead(uri)
                If stream Is Nothing Then
                    Throw New InvalidOperationException("Unable to download the dropped file.")
                End If

                Dim fileName = ResolveDroppedFileName(uri, client.ResponseHeaders)
                Dim targetPath = Path.Combine(targetFolder, fileName)

                Using output As New FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None)
                    stream.CopyTo(output)
                End Using

                Return targetPath
            End Using
        End Using
    End Function

    Private Function ResolveDroppedFileName(uri As Uri, headers As WebHeaderCollection) As String
        Dim contentDisposition As String = Nothing
        If headers IsNot Nothing Then
            contentDisposition = headers("Content-Disposition")
        End If

        Dim fileNameFromHeader = SupportingFeatures.TryResolveContentDispositionFileName(contentDisposition)
        If Not String.IsNullOrWhiteSpace(fileNameFromHeader) Then
            Return fileNameFromHeader
        End If

        Dim fileName = Path.GetFileName(uri.LocalPath)
        If String.IsNullOrWhiteSpace(fileName) Then
            fileName = $"dropped_{Guid.NewGuid():N}{Path.GetExtension(uri.LocalPath)}"
        End If

        Return fileName
    End Function

    Private Function PromptForDownloadFolder() As String
        Using dialog As New FolderBrowserDialog()
            dialog.Description = "Select a folder to store the downloaded PLN file."
            dialog.ShowNewFolderButton = True

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                Return dialog.SelectedPath
            End If
        End Using

        Return String.Empty
    End Function

    Private Sub ClearWeatherSelectionsForDrop()
        _sscPresetName = String.Empty
        _secondaryWPRFilename = String.Empty
        _secondaryWeatherPresetName = String.Empty
        btnSyncWeatherTitle.Enabled = True
    End Sub

    Private Sub btnLoadEventDescriptionTemplate_Click(sender As Object, e As EventArgs) Handles btnLoadEventDescriptionTemplate.Click

        If SessionSettings.EventDescriptionTemplate IsNot Nothing AndAlso txtEventDescription.Text.Trim.Replace(Environment.NewLine, "($*$)") <> SessionSettings.EventDescriptionTemplate.Trim Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Are you sure you want to overwrite the current description with your saved template?", "Loading event description template", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
            End Using
        End If

        If SessionSettings.EventDescriptionTemplate Is Nothing Then
            Exit Sub
        Else
            txtEventDescription.Text = SessionSettings.EventDescriptionTemplate.Replace("($*$)", Environment.NewLine)
        End If

    End Sub

    Private Sub btnSaveEventDescriptionTemplate_Click(sender As Object, e As EventArgs) Handles btnSaveEventDescriptionTemplate.Click

        If Not SessionSettings.EventDescriptionTemplate = String.Empty AndAlso SessionSettings.EventDescriptionTemplate.Trim <> txtEventDescription.Text.Trim.Replace(Environment.NewLine, "($*$)") Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Are you sure you want to replace your existing description template?", "Saving event description template", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
            End Using
        End If

        SessionSettings.EventDescriptionTemplate = txtEventDescription.Text.Trim.Replace(Environment.NewLine, "($*$)")

    End Sub

    Private Sub btnPasteUsernameCredits_Click(sender As Object, e As EventArgs) Handles btnPasteUsernameCredits.Click

        Dim userNameFromCB As String = String.Empty

        Try
            userNameFromCB = Clipboard.GetText()
        Catch ex As Exception
        End Try

        If Not userNameFromCB = String.Empty Then
            txtCredits.Text = $"All credits to @{userNameFromCB} for this task."
        End If

    End Sub

    Private Sub btnSaveDescriptionTemplate_Click(sender As Object, e As EventArgs) Handles btnSaveDescriptionTemplate.Click

        If Not SessionSettings.TaskDescriptionTemplate = String.Empty AndAlso SessionSettings.TaskDescriptionTemplate.Trim <> txtLongDescription.Text.Trim.Replace(Environment.NewLine, "($*$)") Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Are you sure you want to replace your existing description template?", "Saving task description template", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
            End Using
        End If

        SessionSettings.TaskDescriptionTemplate = txtLongDescription.Text.Trim.Replace(Environment.NewLine, "($*$)")

    End Sub

    Private Sub btnRecallTaskDescriptionTemplate_Click(sender As Object, e As EventArgs) Handles btnRecallTaskDescriptionTemplate.Click

        If SessionSettings.TaskDescriptionTemplate IsNot Nothing AndAlso txtLongDescription.Text.Trim.Replace(Environment.NewLine, "($*$)") <> SessionSettings.TaskDescriptionTemplate.Trim Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Are you sure you want to overwrite the current description with your saved template?", "Loading task description template", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
            End Using
        End If

        If SessionSettings.TaskDescriptionTemplate Is Nothing Then
            Exit Sub
        Else
            txtLongDescription.Text = SessionSettings.TaskDescriptionTemplate.Replace("($*$)", Environment.NewLine)
        End If

    End Sub

    Private Sub lstAllCountries_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAllCountries.SelectedIndexChanged

        If lstAllCountries.SelectedIndex = -1 Then
            btnRemoveCountry.Enabled = False
            btnMoveCountryDown.Enabled = False
            btnMoveCountryUp.Enabled = False
        Else
            btnRemoveCountry.Enabled = True
            If lstAllCountries.Items.Count > 1 Then
                btnMoveCountryDown.Enabled = True
                btnMoveCountryUp.Enabled = True
            Else
                btnMoveCountryDown.Enabled = False
                btnMoveCountryUp.Enabled = False
            End If
        End If

        If lstAllCountries.SelectedIndex > -1 AndAlso lstAllCountries.SelectedItems.Count < lstAllCountries.Items.Count Then
            btnMoveCountryDown.Enabled = True
            btnMoveCountryUp.Enabled = True
        Else
            btnMoveCountryDown.Enabled = False
            btnMoveCountryUp.Enabled = False
        End If

    End Sub

    Private Sub txtTaskID_TextChanged(sender As Object, e As EventArgs) Handles txtTaskID.TextChanged
        If txtTaskID.Text.Trim = String.Empty Then
            lblTaskIDNotAcquired.Visible = True
            lblTaskIDAcquired.Visible = False
        Else
            lblTaskIDAcquired.Visible = True
            lblTaskIDNotAcquired.Visible = False
            'Try to retrieve the task EntrySeqID online
            Try
                If txtTemporaryTaskID.Text.Trim.Length > 0 Then
                    GetTaskDetails(txtTemporaryTaskID.Text.Trim, _TaskEntrySeqID)
                Else
                    GetTaskDetails(txtTaskID.Text.Trim, _TaskEntrySeqID)
                End If
            Catch ex As Exception
                'Do nothing - it means the task has not been pushed to the online database
            End Try
        End If
        AllFieldChanges(sender, e)
    End Sub

    Private Sub AllFieldChanges(sender As Object, e As EventArgs) Handles chkTitleLock.CheckedChanged,
                                                                          chkDepartureLock.CheckedChanged,
                                                                          chkArrivalLock.CheckedChanged,
                                                                          chkDescriptionLock.CheckedChanged,
                                                                          chkIncludeYear.CheckedChanged,
                                                                          chkSoaringTypeRidge.CheckedChanged,
                                                                          chkSuppressWarningForBaroPressure.CheckedChanged,
                                                                          chkLockMapImage.CheckedChanged,
                                                                          chkLockCoverImage.CheckedChanged,
                                                                          txtLongDescription.TextChanged,
                                                                          txtEventTitle.TextChanged,
                                                                          txtEventDescription.TextChanged,
                                                                          txtWeatherSummary.TextChanged,
                                                                          txtSimDateTimeExtraInfo.TextChanged,
                                                                          txtShortDescription.TextChanged,
                                                                          txtDurationMin.TextChanged,
                                                                          txtDurationMax.TextChanged,
                                                                          txtDurationExtraInfo.TextChanged,
                                                                          txtCredits.TextChanged,
                                                                          txtTitle.TextChanged,
                                                                          txtMainArea.TextChanged,
                                                                          txtDepName.TextChanged,
                                                                          txtDepExtraInfo.TextChanged,
                                                                          txtArrivalName.TextChanged,
                                                                          txtArrivalExtraInfo.TextChanged,
                                                                          txtSoaringTypeExtraInfo.TextChanged,
                                                                          txtDifficultyExtraInfo.TextChanged,
                                                                          txtGroupEventPostURL.TextChanged,
                                                                          txtBaroPressureExtraInfo.TextChanged,
                                                                          dtSimDate.ValueChanged,
                                                                          dtSimLocalTime.ValueChanged,
                                                                          cboRecommendedGliders.SelectedIndexChanged,
                                                                          cboDifficulty.TextChanged,
                                                                          cboDifficulty.SelectedIndexChanged,
                                                                          txtOtherBeginnerLink.TextChanged,
                                                                          chkSoaringTypeDynamic.CheckedChanged,
                                                                          txtEventTeaserMessage.TextChanged,
                                                                          txtClubFullName.TextChanged,
                                                                          txtRepostOriginalURL.TextChanged,
                                                                          dtRepostOriginalDate.ValueChanged,
                                                                          txtTrackerGroup.TextChanged, txtMinutesBeforeMeeting.TextChanged

        If _isInitiatizing Then
            Exit Sub
        End If

        'Check specific fields colateral actions
        If sender Is txtTitle AndAlso chkTitleLock.Checked = False AndAlso txtTitle.Text <> _OriginalFlightPlanTitle Then
            chkTitleLock.Checked = True
        End If
        If sender Is txtShortDescription AndAlso chkDescriptionLock.Checked = False AndAlso txtShortDescription.Text <> _OriginalFlightPlanShortDesc Then
            chkDescriptionLock.Checked = True
        End If
        If sender Is txtDepName AndAlso chkDepartureLock.Checked = False AndAlso txtDepName.Text <> _OriginalFlightPlanDeparture Then
            chkDepartureLock.Checked = True
        End If
        If sender Is txtArrivalName AndAlso chkArrivalLock.Checked = False AndAlso txtArrivalName.Text <> _OriginalFlightPlanArrival Then
            chkArrivalLock.Checked = True
        End If
        If sender Is txtDepExtraInfo AndAlso chkArrivalLock.Checked = False AndAlso txtDepExtraInfo.Text <> _OriginalFlightPlanDepRwy Then
            chkDepartureLock.Checked = True
        End If
        If Not _loadingFile AndAlso sender Is txtEventTeaserMessage Then
            chkDGPOTeaser.Checked = True
        End If

        CheckWhichOptionsCanBeEnabled()
        SessionModified(sender)

        If TypeOf sender IsNot Windows.Forms.TextBox Then
            GeneralFPTabFieldLeaveDetection(sender, e)
        End If

        HighlightExpectedFields()

    End Sub

    Private Sub GeneralFPTabFieldLeaveDetection(sender As Object, e As EventArgs) Handles txtSoaringTypeExtraInfo.Leave,
                                                                                           txtSimDateTimeExtraInfo.Leave,
                                                                                           txtTitle.Leave,
                                                                                           txtShortDescription.Leave,
                                                                                           txtMainArea.Leave,
                                                                                           txtDurationMin.Leave,
                                                                                           txtDurationMax.Leave,
                                                                                           txtDurationExtraInfo.Leave,
                                                                                           txtDifficultyExtraInfo.Leave,
                                                                                           txtDepName.Leave,
                                                                                           txtDepExtraInfo.Leave,
                                                                                           txtCredits.Leave,
                                                                                           txtArrivalName.Leave,
                                                                                           txtArrivalExtraInfo.Leave,
                                                                                           txtLongDescription.Leave,
                                                                                           txtWeatherSummary.Leave,
                                                                                           txtBaroPressureExtraInfo.Leave,
                                                                                           txtGroupEventPostURL.Leave,
                                                                                           txtRepostOriginalURL.Leave,
                                                                                           txtLastUpdateDescription.Leave,
                                                                                           cboRecommendedGliders.Leave, cboCountryFlag.Leave

        'Trim all text boxes!
        If TypeOf sender Is Windows.Forms.TextBox Then
            _SF.RemoveForbiddenPrefixes(sender)
            Dim theTextBox As Windows.Forms.TextBox = DirectCast(sender, Windows.Forms.TextBox)
            If theTextBox.Text <> theTextBox.Text.Trim Then
                theTextBox.Text = theTextBox.Text.Trim
            End If
        End If

        'Trim comboboxes!
        If TypeOf sender Is Windows.Forms.ComboBox Then
            Dim theComboBox As Windows.Forms.ComboBox = DirectCast(sender, Windows.Forms.ComboBox)
            If theComboBox.Text <> theComboBox.Text.Trim Then
                theComboBox.Text = theComboBox.Text.Trim
            End If
        End If

        'Some fields need to be copied to the Event tab
        If sender Is txtTitle OrElse sender Is txtShortDescription Then
            CopyToEventFields(sender, e)
        End If

        If sender Is txtWeatherSummary Then
            WeatherFieldChangeDetection()
        End If

        'For text box, make sure to display the value from the start
        If TypeOf sender Is Windows.Forms.TextBox Then
            LeavingTextBox(sender)
        End If

    End Sub

    Private Sub chkSoaringTypeThermal_CheckedChanged(sender As Object, e As EventArgs) Handles chkSoaringTypeThermal.CheckedChanged

        If chkSoaringTypeThermal.Checked Then
            chkUseSyncFly.Checked = True
        End If

        'General object change processing
        AllFieldChanges(sender, e)

    End Sub

    Private Sub chkSoaringTypeWave_CheckedChanged(sender As Object, e As EventArgs) Handles chkSoaringTypeWave.CheckedChanged

        If chkSoaringTypeWave.Checked Then
            chkUseSyncFly.Checked = True
        End If

        'General object change processing
        AllFieldChanges(sender, e)

    End Sub

    Private Sub DurationNumberValidation(ByVal sender As Windows.Forms.TextBox, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDurationMin.KeyPress, txtDurationMax.KeyPress, txtMinutesBeforeMeeting.KeyPress
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

    Private Sub SelectFlightPlan_Click(sender As Object, e As EventArgs) Handles btnSelectFlightPlan.Click

        If txtWeatherFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtWeatherFile.Text)
        End If

        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select flight plan file"
        OpenFileDialog1.Filter = "Flight plan|*.pln"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim selectedFlightPlan As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileName)
            Dim currentFlightPlan As String = SupportingFeatures.SanitizeFilePath(txtFlightPlanFile.Text)

            SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(selectedFlightPlan)
            If currentFlightPlan.Trim.Length > 0 AndAlso selectedFlightPlan <> currentFlightPlan Then
                'User has selected a different flight plan than the current one - ask to reset first?
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show(Me, "You have selected a different flight plan file. Do you want to reset first ?", "Selecting a new flight plan file", vbYesNo, MessageBoxIcon.Question) = vbYes Then
                        'Reset first
                        ResetForm()
                    End If
                End Using
            End If
            LoadFlightPlan(selectedFlightPlan)
        End If

    End Sub

    Private Sub txtFPResults_TextChanged(sender As Object, e As EventArgs) Handles txtFPResults.TextChanged
        lblNbrCarsMainFP.Text = $"{txtFPResults.Text.Length} chars"
    End Sub

    Private Sub NbrCarsCheckDiscordLimitEvent(sender As Object, e As EventArgs) Handles lblNbrCarsMainFP.TextChanged

        NbrCarsCheckDiscordLimit(DirectCast(sender, Windows.Forms.Label))

    End Sub

    Private Sub btnSelectWeatherFile_Click(sender As Object, e As EventArgs) Handles btnSelectWeatherFile.Click

        If CurrentSessionFile = String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "You need to load or create a session file before accessing the weather preset browser.", "No session loaded", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        Dim result As DialogResult = _WeatherPresetBrowser.ShowForm(Me, _sscPresetName, _primaryWPRFilename, _secondaryWPRFilename)

        If result = DialogResult.OK Then
            _sscPresetName = _WeatherPresetBrowser.SSCPresetName
            _primaryWPRFilename = _WeatherPresetBrowser.PrimaryWPRFilename
            _secondaryWPRFilename = _WeatherPresetBrowser.SecondaryWPRFilename

            'If an SSC preset, Sync must not be available and check for weather summary
            btnSyncWeatherTitle.Enabled = (_sscPresetName = String.Empty)
            If _sscPresetName <> String.Empty AndAlso (txtWeatherSummary.Text.Trim = String.Empty OrElse (txtWeatherSummary.Text.Trim.StartsWith("SSC v") AndAlso txtWeatherSummary.Text.Trim.Length = 10)) Then
                'Auto fill weather summary
                txtWeatherSummary.Text = $"SSC {Path.GetFileNameWithoutExtension(_primaryWPRFilename).Substring(6, 6)}"
            ElseIf _sscPresetName = String.Empty AndAlso txtWeatherSummary.Text.Trim.StartsWith("SSC v") AndAlso txtWeatherSummary.Text.Trim.Length = 10 Then
                'Most probably an old SSC preset was specified in the summary - Clear weather summary
                txtWeatherSummary.Text = String.Empty
            End If

            'Set the weather preset in the form and load it
            Dim selectedWeatherFile As String = SupportingFeatures.SanitizeFilePath(_primaryWPRFilename)
            SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(selectedWeatherFile)
            LoadWeatherfile(selectedWeatherFile)
            UpdateSecondaryWeatherPresetName()
        End If

    End Sub

    Private Sub CopyToEventFields(sender As Object, e As EventArgs)

        If sender Is txtTitle AndAlso txtEventTitle.Text.Trim = String.Empty Then
            txtEventTitle.Text = TaskTitleMinusClubName()
        End If

    End Sub

    Private Function TaskTitleMinusClubName() As String

        Dim taskTitle As String = txtTitle.Text.Trim()
        Dim eventTitle As String = String.Empty

        For Each groupName As String In _SF.ListOfAllTrackerGroups
            ' Normalize casing for comparison
            Dim taskTitleLower As String = taskTitle.ToLower()
            Dim groupNameLower As String = groupName.ToLower()

            ' Check if the task title starts with the group name followed by a common delimiter
            If taskTitleLower.StartsWith(groupNameLower) Then
                ' Define delimiters with optional spaces
                Dim delimiters As String() = {" - ", "-", " :", ":", " -", "- ", " "}
                For Each delimiter In delimiters
                    Dim prefix As String = groupNameLower & delimiter
                    If taskTitleLower.StartsWith(prefix) Then
                        ' Remove the prefix and trim any extra whitespace
                        eventTitle = taskTitle.Substring(prefix.Length).Trim()
                        Exit For
                    End If
                Next
            End If
            If eventTitle <> String.Empty Then
                Exit For
            End If
        Next
        If eventTitle = String.Empty Then
            eventTitle = taskTitle
        End If

        Return eventTitle

    End Function

    Private Sub txtFlightPlanFile_TextChanged(sender As Object, e As EventArgs) Handles txtFlightPlanFile.TextChanged

        If txtFlightPlanFile.Text = String.Empty Then
            grbTaskInfo.Enabled = False
            grbTaskPart2.Enabled = False
            grbTaskDiscord.Enabled = False
            btnSyncFlightPlanTitle.Enabled = False
        Else
            grbTaskInfo.Enabled = True
            grbTaskPart2.Enabled = True
            grbTaskDiscord.Enabled = True
            btnSyncFlightPlanTitle.Enabled = True
        End If

    End Sub

    Private Sub txtWeatherFile_TextChanged(sender As Object, e As EventArgs) Handles txtWeatherFile.TextChanged

        If txtWeatherFile.Text = String.Empty Then
            btnSyncWeatherTitle.Enabled = False
        Else
            btnSyncWeatherTitle.Enabled = (_sscPresetName = String.Empty)
        End If

    End Sub

    Private Sub txtDistanceTotal_TextChanged(sender As Object, e As EventArgs) Handles txtDistanceTrack.TextChanged, txtDistanceTotal.TextChanged

        'One of the distance has changed - recalculate their corresponding labels and miles
        If txtDistanceTotal.Text <> String.Empty Then
            lblTotalDistanceAndMiles.Text = "km / " & FormatNumber(Conversions.KmToMiles(Decimal.Parse(txtDistanceTotal.Text)), 0) & " mi Total"
        End If
        If txtDistanceTrack.Text <> String.Empty Then
            lblTrackDistanceAndMiles.Text = "km / " & FormatNumber(Conversions.KmToMiles(Decimal.Parse(txtDistanceTrack.Text)), 0) & " mi Task"
        End If

        CheckAndSetEventAward()

    End Sub

    Private Sub btnSyncFlightPlanTitles_Click(sender As Object, e As EventArgs) Handles btnSyncFlightPlanTitle.Click

        If Not CheckUnsavedAndConfirmAction("Sync titles") Then
            Exit Sub
        End If

        txtTitle.Text = txtTitle.Text.Trim

        'Check if title is valid as a filename
        Dim resultFilenameValidation As String = SupportingFeatures.ValidateFileName(txtTitle.Text)
        If resultFilenameValidation <> String.Empty Then
            'Title not valid as a filename
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"The title cannot be a filename:{Environment.NewLine}{resultFilenameValidation}", "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        'Sync flight plan title inside .pln file
        If _OriginalFlightPlanTitle.Trim <> txtTitle.Text Then
            _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Title").Item(0).FirstChild.Value = txtTitle.Text
            Try
                _XmlDocFlightPlan.Save(txtFlightPlanFile.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to save the flight plan file:{Environment.NewLine}{ex.Message}", "Error trying to save flight plan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub
            End Try
        End If

        'Sync flight plan .pln filename
        If Path.GetFileNameWithoutExtension(txtFlightPlanFile.Text) <> txtTitle.Text Then
            'Delete the original flightplan file
            Try
                File.Delete(txtFlightPlanFile.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to delete the original flight plan file:{Environment.NewLine}{ex.Message}", "Error trying to delete flight plan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub
            End Try

            Try
                txtFlightPlanFile.Text = $"{Path.GetDirectoryName(txtFlightPlanFile.Text)}\{txtTitle.Text}.pln"
                _XmlDocFlightPlan.Save(txtFlightPlanFile.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to save new flight plan file:{Environment.NewLine}{ex.Message}", "Error trying to save new flight plan", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub
            End Try

        End If

        'Change flight plan selection and reload flight plan
        _loadingFile = True
        LoadFlightPlan(txtFlightPlanFile.Text)
        chkTitleLock.Checked = False
        _loadingFile = False
        SaveSession()

    End Sub

    Private Sub btnSyncWeatherTitles_Click(sender As Object, e As EventArgs) Handles btnSyncWeatherTitle.Click

        If Not CheckUnsavedAndConfirmAction("Sync titles") Then
            Exit Sub
        End If

        txtTitle.Text = txtTitle.Text.Trim

        'Check if title is valid as a filename
        Dim resultFilenameValidation As String = SupportingFeatures.ValidateFileName(txtTitle.Text)
        If resultFilenameValidation <> String.Empty Then
            'Title not valid as a filename
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"The title cannot be a filename:{Environment.NewLine}{resultFilenameValidation}", "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        'Sync weather profile name inside .wpr
        If _WeatherDetails.PresetName <> txtTitle.Text Then
            _XmlDocWeatherPreset.DocumentElement.SelectNodes("WeatherPreset.Preset/Name").Item(0).FirstChild.Value = txtTitle.Text
            Try
                _XmlDocWeatherPreset.Save(txtWeatherFile.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to save the weather file:{Environment.NewLine}{ex.Message}", "Error trying to save weather file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub
            End Try
        End If

        'Sync weather .wpr filename
        If Path.GetFileNameWithoutExtension(txtWeatherFile.Text) <> txtTitle.Text Then
            'Delete the original weather file
            Try
                File.Delete(txtWeatherFile.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to delete the original weather file:{Environment.NewLine}{ex.Message}", "Error trying to delete the weather file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub
            End Try

            Try
                txtWeatherFile.Text = $"{Path.GetDirectoryName(txtWeatherFile.Text)}\{txtTitle.Text}.wpr"
                _XmlDocWeatherPreset.Save(txtWeatherFile.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Unable to save new weather file:{Environment.NewLine}{ex.Message}", "Error trying to save new weather file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit Sub
            End Try

        End If

        'Change weather file selection and reload weather
        _loadingFile = True
        LoadWeatherfile(txtWeatherFile.Text)
        _loadingFile = False
        SaveSession()

    End Sub

#End Region

#Region "Extra files Controls events"

    Private Sub btnAddExtraFile_Click(sender As Object, e As EventArgs) Handles btnAddExtraFile.Click
        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = "H: \MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select extra file"
        OpenFileDialog1.Filter = "All files|*.*"
        OpenFileDialog1.Multiselect = True

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim currentFlightPlan As String = SupportingFeatures.SanitizeFilePath(txtFlightPlanFile.Text)
            Dim currentWeatherFile As String = SupportingFeatures.SanitizeFilePath(txtWeatherFile.Text)

            For i As Integer = 0 To OpenFileDialog1.FileNames.Count - 1
                Dim sanitizedExtraFile As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileNames(i))

                If String.IsNullOrEmpty(sanitizedExtraFile) Then
                    Continue For
                End If

                'Check if one of the files is selected flight plan or weather file to exclude them
                If sanitizedExtraFile <> currentFlightPlan And sanitizedExtraFile <> currentWeatherFile Then
                    If GetEffectiveExtraFileCountForLimit() >= MaxExtraFiles Then
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show(Me, "Maximum 10 extra files (map/cover images not included).", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using
                        Exit For
                    End If
                    AddExtraFile(sanitizedExtraFile)
                End If
            Next
        End If

        If lstAllFiles.SelectedIndex > -1 AndAlso lstAllFiles.SelectedItems.Count < lstAllFiles.Items.Count Then
            btnExtraFileDown.Enabled = True
            btnExtraFileUp.Enabled = True
        Else
            btnExtraFileDown.Enabled = False
            btnExtraFileUp.Enabled = False
        End If

        LoadPossibleImagesInMapDropdown(cboBriefingMap.SelectedItem)
        LoadPossibleImagesInCoverDropdown(cboCoverImage.SelectedItem)

    End Sub

    Private Sub btnRemoveExtraFile_Click(sender As Object, e As EventArgs) Handles btnRemoveExtraFile.Click

        For i As Integer = lstAllFiles.SelectedIndices.Count - 1 To 0 Step -1
            lstAllFiles.Items.RemoveAt(lstAllFiles.SelectedIndices(i))
            SessionModified(SourceOfChange.TaskTab)
        Next

        If lstAllFiles.SelectedIndex > -1 AndAlso lstAllFiles.SelectedItems.Count < lstAllFiles.Items.Count Then
            btnExtraFileDown.Enabled = True
            btnExtraFileUp.Enabled = True
        Else
            btnExtraFileDown.Enabled = False
            btnExtraFileUp.Enabled = False
        End If

        LoadPossibleImagesInMapDropdown(cboBriefingMap.SelectedItem)
        LoadPossibleImagesInCoverDropdown(cboCoverImage.SelectedItem)

    End Sub

    Private Sub btnExtraFileUp_Click(sender As Object, e As EventArgs) Handles btnExtraFileUp.Click

        MoveExtraFilesSelectedItems(-1, lstAllFiles)
        btnExtraFileUp.Focus()
        SessionModified(SourceOfChange.TaskTab)

    End Sub

    Private Sub btnExtraFileDown_Click(sender As Object, e As EventArgs) Handles btnExtraFileDown.Click

        MoveExtraFilesSelectedItems(1, lstAllFiles)
        btnExtraFileDown.Focus()
        SessionModified(SourceOfChange.TaskTab)

    End Sub

    Private Sub lstAllFiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAllFiles.SelectedIndexChanged

        If lstAllFiles.SelectedIndex = -1 Then
            btnRemoveExtraFile.Enabled = False
            btnExtraFileDown.Enabled = False
            btnExtraFileUp.Enabled = False
        Else
            btnRemoveExtraFile.Enabled = True
            If lstAllFiles.Items.Count > 1 Then
                btnExtraFileDown.Enabled = True
                btnExtraFileUp.Enabled = True
            Else
                btnExtraFileDown.Enabled = False
                btnExtraFileUp.Enabled = False
            End If
        End If

        If lstAllFiles.SelectedIndex > -1 AndAlso lstAllFiles.SelectedItems.Count < lstAllFiles.Items.Count Then
            btnExtraFileDown.Enabled = True
            btnExtraFileUp.Enabled = True
        Else
            btnExtraFileDown.Enabled = False
            btnExtraFileUp.Enabled = False
        End If

    End Sub

#End Region

#Region "Add-Ons Controls events"

    Private Sub lstAllRecommendedAddOns_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAllRecommendedAddOns.SelectedIndexChanged

        If lstAllRecommendedAddOns.SelectedIndex = -1 Then
            btnEditSelectedAddOn.Enabled = False
            btnRemoveSelectedAddOns.Enabled = False
        ElseIf lstAllRecommendedAddOns.SelectedItems.Count > 1 Then
            btnEditSelectedAddOn.Enabled = False
            btnRemoveSelectedAddOns.Enabled = True
        Else
            btnEditSelectedAddOn.Enabled = True
            btnRemoveSelectedAddOns.Enabled = True
        End If

        If lstAllRecommendedAddOns.SelectedIndex > -1 AndAlso lstAllRecommendedAddOns.SelectedItems.Count < lstAllRecommendedAddOns.Items.Count Then
            btnAddOnUp.Enabled = True
            btnAddOnDown.Enabled = True
        Else
            btnAddOnUp.Enabled = False
            btnAddOnDown.Enabled = False
        End If

    End Sub

    Private Sub btnAddRecAddOn_Click(sender As Object, e As EventArgs) Handles btnAddRecAddOn.Click

        Dim addOn As New RecommendedAddOn
        addOn.Type = RecommendedAddOn.Types.Freeware

        If RecommendedAddOnsForm.ShowForm(Me, addOn, False) = DialogResult.OK Then
            lstAllRecommendedAddOns.Items.Add(addOn)
            SessionModified(SourceOfChange.TaskTab)
        End If

        If lstAllRecommendedAddOns.SelectedIndex > -1 AndAlso lstAllRecommendedAddOns.SelectedItems.Count < lstAllRecommendedAddOns.Items.Count Then
            btnAddOnUp.Enabled = True
            btnAddOnDown.Enabled = True
        Else
            btnAddOnUp.Enabled = False
            btnAddOnDown.Enabled = False
        End If

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

    End Sub

    Private Sub btnEditSelectedAddOn_Click(sender As Object, e As EventArgs) Handles btnEditSelectedAddOn.Click

        Dim addOn As RecommendedAddOn = lstAllRecommendedAddOns.SelectedItem
        Dim index As Integer = lstAllRecommendedAddOns.SelectedIndex

        Select Case RecommendedAddOnsForm.ShowForm(Me, addOn, True)
            Case DialogResult.OK
                'Save - remove
                lstAllRecommendedAddOns.Items.RemoveAt(index)
                ' Re-insert the modified item at the same index
                lstAllRecommendedAddOns.Items.Insert(index, addOn)
                lstAllRecommendedAddOns.SelectedIndex = index
                SessionModified(SourceOfChange.TaskTab)
            Case DialogResult.Cancel
                'Cancel
        End Select

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

    End Sub

    Private Sub btnRemoveSelectedAddOns_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedAddOns.Click

        For i As Integer = lstAllRecommendedAddOns.SelectedIndices.Count - 1 To 0 Step -1
            lstAllRecommendedAddOns.Items.RemoveAt(lstAllRecommendedAddOns.SelectedIndices(i))
            SessionModified(SourceOfChange.TaskTab)
        Next

        If lstAllRecommendedAddOns.SelectedIndex > -1 AndAlso lstAllRecommendedAddOns.SelectedItems.Count < lstAllRecommendedAddOns.Items.Count Then
            btnAddOnUp.Enabled = True
            btnAddOnDown.Enabled = True
        Else
            btnAddOnUp.Enabled = False
            btnAddOnDown.Enabled = False
        End If

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

    End Sub

    Private Sub btnAddOnUp_Click(sender As Object, e As EventArgs) Handles btnAddOnUp.Click

        MoveExtraFilesSelectedItems(-1, lstAllRecommendedAddOns)
        btnAddOnUp.Focus()

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

        SessionModified(SourceOfChange.TaskTab)

    End Sub

    Private Sub btnAddOnDown_Click(sender As Object, e As EventArgs) Handles btnAddOnDown.Click

        MoveExtraFilesSelectedItems(1, lstAllRecommendedAddOns)
        btnAddOnDown.Focus()

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

        SessionModified(SourceOfChange.TaskTab)

    End Sub

#End Region

#Region "Country controls events"

    Private Sub btnDetectCountries_Click(sender As Object, e As EventArgs) Handles btnDetectCountries.Click

        If _useTestMode Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "Country detection is disabled in test mode.", "Country detection disabled", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
            Exit Sub
        End If

        Dim currentAllCountriesForComparison As String = String.Empty
        Dim newAllCountriesForComparison As String = String.Empty
        Dim country As String

        If lstAllCountries.Items.Count > 0 Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "There is already a list of countries. Are you sure you want to run the detection again?", "Country detection", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
            End Using
        End If

        For Each country In lstAllCountries.Items
            currentAllCountriesForComparison = $"{currentAllCountriesForComparison};{country}"
        Next

        'Build countries
        lstAllCountries.Items.Clear()
        For Each waypoint As ATCWaypoint In _SF.AllWaypoints
            waypoint.CountryISO3166Code = CountryGeo.GetCountryFromCoordinatesAzure(waypoint.Latitude, waypoint.Longitude)
            If _SF.CountryISO3166Codes.ContainsKey(waypoint.CountryISO3166Code) Then
                country = _SF.CountryISO3166Codes(waypoint.CountryISO3166Code)
                If _SF.CountryFlagCodes.ContainsKey(country) AndAlso Not lstAllCountries.Items.Contains(country) Then
                    lstAllCountries.Items.Add(country)
                    newAllCountriesForComparison = $"{newAllCountriesForComparison};{country}"
                End If
            End If
        Next

        'Are there any changes?
        If currentAllCountriesForComparison <> newAllCountriesForComparison Then
            SessionModified(SourceOfChange.TaskTab)
        End If

    End Sub

    Private Sub btnAddCountry_Click(sender As Object, e As EventArgs) Handles btnAddCountry.Click
        If cboCountryFlag.SelectedIndex > 0 AndAlso Not lstAllCountries.Items.Contains(cboCountryFlag.Text) Then
            lstAllCountries.Items.Add(cboCountryFlag.Text)
            'BuildFPResults()
            SessionModified(SourceOfChange.TaskTab)
            If lstAllCountries.SelectedIndex > -1 AndAlso lstAllCountries.SelectedItems.Count < lstAllCountries.Items.Count Then
                btnMoveCountryDown.Enabled = True
                btnMoveCountryUp.Enabled = True
            Else
                btnMoveCountryDown.Enabled = False
                btnMoveCountryUp.Enabled = False
            End If
        End If
        HighlightExpectedFields()
    End Sub

    Private Sub btnRemoveCountry_Click(sender As Object, e As EventArgs) Handles btnRemoveCountry.Click
        For i As Integer = lstAllCountries.SelectedIndices.Count - 1 To 0 Step -1
            lstAllCountries.Items.RemoveAt(lstAllCountries.SelectedIndices(i))
            SessionModified(SourceOfChange.TaskTab)
        Next
        If lstAllCountries.SelectedIndex > -1 AndAlso lstAllCountries.SelectedItems.Count < lstAllCountries.Items.Count Then
            btnMoveCountryDown.Enabled = True
            btnMoveCountryUp.Enabled = True
        Else
            btnMoveCountryDown.Enabled = False
            btnMoveCountryUp.Enabled = False
        End If
        'BuildFPResults()
        HighlightExpectedFields()
    End Sub

    Private Sub btnMoveCountryUp_Click(sender As Object, e As EventArgs) Handles btnMoveCountryUp.Click
        MoveExtraFilesSelectedItems(-1, lstAllCountries)
        'BuildFPResults()
        SessionModified(SourceOfChange.TaskTab)
    End Sub

    Private Sub btnMoveCountryDown_Click(sender As Object, e As EventArgs) Handles btnMoveCountryDown.Click
        MoveExtraFilesSelectedItems(1, lstAllCountries)
        'BuildFPResults()
        SessionModified(SourceOfChange.TaskTab)
    End Sub

#End Region

#Region "Flight Plan tab Subs & Functions"

    Private Function HighlightExpectedFields(Optional showMessageBox As Boolean = False) As Boolean

        Dim messageText As New StringBuilder
        Dim requiredText As New StringBuilder
        Dim cannotContinue As Boolean = False

        If txtTitle.Text.Trim = String.Empty Then
            SetLabelFormat(lblTitle, LabelFormat.BoldRed, requiredText, "currentTaskWayPoints title is required!")
            cannotContinue = True
        Else
            SetLabelFormat(lblTitle, LabelFormat.Regular)
        End If
        If txtWeatherFile.Text.Trim = String.Empty OrElse (Not File.Exists(txtWeatherFile.Text)) Then
            requiredText.AppendLine("A weather file is required!")
            cannotContinue = True
        Else
            SetLabelFormat(lblTitle, LabelFormat.Regular)
        End If
        If chkSoaringTypeRidge.Checked = False AndAlso chkSoaringTypeThermal.Checked = False AndAlso chkSoaringTypeWave.Checked = False AndAlso chkSoaringTypeDynamic.Checked = False Then
            SetLabelFormat(lblSoaringType, LabelFormat.BoldRed, requiredText, "At least one soaring type is required!")
            cannotContinue = True
        Else
            SetLabelFormat(lblSoaringType, LabelFormat.Regular)
        End If

        If txtMainArea.Text.Trim = String.Empty Then
            SetLabelFormat(lblMainAreaPOI, LabelFormat.BoldBlack, messageText, "Missing Main Area / POI")
        Else
            SetLabelFormat(lblMainAreaPOI, LabelFormat.Regular)
        End If

        If txtDepExtraInfo.Text.Trim = String.Empty Then
            SetLabelFormat(lblDeparture, LabelFormat.BoldBlack, messageText, "Possibly missing departure runway info?")
        Else
            SetLabelFormat(lblDeparture, LabelFormat.Regular)
        End If

        If txtDurationMin.Text.Trim = String.Empty AndAlso txtDurationMax.Text.Trim = String.Empty AndAlso txtDurationExtraInfo.Text.Trim = String.Empty Then
            SetLabelFormat(lblDuration, LabelFormat.BoldBlack, messageText, "Possibly missing duration info?")
        Else
            SetLabelFormat(lblDuration, LabelFormat.Regular)
        End If

        If cboRecommendedGliders.Text.Trim = String.Empty Then
            SetLabelFormat(lblRecommendedGliders, LabelFormat.BoldBlack, messageText, "Possibly missing recommended gliders?")
        Else
            SetLabelFormat(lblRecommendedGliders, LabelFormat.Regular)
        End If

        If cboDifficulty.SelectedIndex = 0 AndAlso txtDifficultyExtraInfo.Text.Trim = String.Empty Then
            SetLabelFormat(lblDifficultyRating, LabelFormat.BoldBlack, messageText, "Possibly missing difficulty rating?")
        Else
            SetLabelFormat(lblDifficultyRating, LabelFormat.Regular)
        End If

        If txtCredits.Text.Trim = String.Empty OrElse txtCredits.Text.ToUpper.Contains("@USERNAME") Then
            SetLabelFormat(lblCredits, LabelFormat.BoldBlack, messageText, "Possibly missing credits info or set to @UserName?")
        Else
            SetLabelFormat(lblCredits, LabelFormat.Regular)
        End If

        If lstAllCountries.Items.Count = 0 Then
            SetLabelFormat(lblCountries, LabelFormat.BoldBlack, messageText, "Possibly missing country?")
        Else
            SetLabelFormat(lblCountries, LabelFormat.Regular)
        End If

        If txtWeatherSummary.Text.Trim = String.Empty Then
            SetLabelFormat(lblWeatherSummary, LabelFormat.BoldBlack, messageText, "Possibly missing weather summary?")
        Else
            SetLabelFormat(lblWeatherSummary, LabelFormat.Regular)
        End If

        If cboBriefingMap.SelectedIndex = -1 Then
            SetLabelFormat(lblMap, LabelFormat.BoldBlack, messageText, "Possibly missing a briefing map selection?")
        Else
            SetLabelFormat(lblMap, LabelFormat.Regular)
        End If

        Dim startWaypoint As ATCWaypoint = SupportingFeatures.GetTaskStartWaypoint(_SF.AllWaypoints)

        If startWaypoint IsNot Nothing AndAlso Not startWaypoint.MaxAltFeet.HasValue Then
            messageText.AppendLine("Possibly missing MAX altitude restriction on Start waypoint?")
        End If

        If showMessageBox Then
            If cannotContinue Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, $"Some fields are incomplete:{Environment.NewLine}{Environment.NewLine}{requiredText.ToString}", "Field validation prior to posting", vbOKOnly, vbCritical)
                End Using
                Return cannotContinue
            Else
                If messageText.ToString <> String.Empty Then
                    Using New Centered_MessageBox(Me)
                        If MessageBox.Show(Me, $"Some fields may be incomplete, do you want to continue?{Environment.NewLine}{Environment.NewLine}{messageText.ToString}", "Field validation prior to posting", vbYesNo, vbQuestion) = DialogResult.No Then
                            Return True
                        Else
                            Return False
                        End If
                    End Using
                End If
            End If
        End If

        Return cannotContinue

    End Function

    Private Enum LabelFormat
        Regular = 0
        BoldBlack = 1
        BoldRed = 2
    End Enum
    Private Sub SetLabelFormat(labelToSet As Windows.Forms.Label, format As LabelFormat, Optional ByRef messageBuilder As StringBuilder = Nothing, Optional tooltipText As String = "")
        Select Case format
            Case LabelFormat.Regular
                ' Set the font to regular (non-bold) and black color
                labelToSet.Font = New Font(labelToSet.Font, FontStyle.Regular)
                labelToSet.ForeColor = Color.Black
                ToolTip1.SetToolTip(labelToSet, String.Empty)
            Case LabelFormat.BoldBlack
                ' Set the font to bold and black color
                labelToSet.Font = New Font(labelToSet.Font, FontStyle.Bold)
                labelToSet.ForeColor = Color.Black
                ToolTip1.SetToolTip(labelToSet, tooltipText)
                If messageBuilder IsNot Nothing Then
                    messageBuilder.AppendLine(tooltipText)
                End If
            Case LabelFormat.BoldRed
                ' Set the font to bold and red color
                labelToSet.Font = New Font(labelToSet.Font, FontStyle.Bold)
                labelToSet.ForeColor = Color.Red
                ToolTip1.SetToolTip(labelToSet, tooltipText)
                If messageBuilder IsNot Nothing Then
                    messageBuilder.AppendLine(tooltipText)
                End If
        End Select
    End Sub

    Private Sub WeatherFieldChangeDetection()
        BuildWeatherInfoResults()
        If Not (_WeatherDetails Is Nothing) Then
            BuildWeatherCloudLayers()
            BuildWeatherWindLayers()
        Else
            txtWeatherClouds.Text = String.Empty
            txtWeatherWinds.Text = String.Empty
        End If
    End Sub

    Private Sub BuildRecAddOnsText()

        Dim sb As New StringBuilder

        If lstAllRecommendedAddOns.Items.Count > 0 Then
            sb.AppendLine("## 📀 Recommended add-ons")
            For Each addOn As RecommendedAddOn In lstAllRecommendedAddOns.Items
                If SupportingFeatures.IsValidURL(addOn.URL) Then
                    sb.AppendLine($"- [{addOn.Name} ({addOn.Type.ToString})]({addOn.URL})")
                End If
            Next
        End If

        txtAddOnsDetails.Text = sb.ToString

    End Sub

    Private Sub BuildFPResults(Optional fromGroup As Boolean = False, Optional onlyFullDetailsMode As Boolean = True)

        txtFPResults.Text = BuildFPResultBaseOnAvailabilityOrNot(fromGroup, True)
        If (Not onlyFullDetailsMode) AndAlso IsDelayedAvailability Then
            txtFPResultsDelayedAvailability.Text = BuildFPResultBaseOnAvailabilityOrNot(fromGroup, False)
        Else
            txtFPResultsDelayedAvailability.Text = String.Empty
        End If

        If txtLongDescription.Text.Trim.Length > 0 Then
            Dim localizer As LocalizationManager = LocalizationManager.Instance
            txtFullDescriptionResults.Text = $"{localizer.Format("group.thread.full_description_header")}{Environment.NewLine}{txtLongDescription.Text.Trim}"
        Else
            Dim localizer As LocalizationManager = LocalizationManager.Instance
            txtFullDescriptionResults.Text = $"{localizer.Format("group.thread.full_description_header")}{Environment.NewLine}{localizer.Format("group.thread.full_description_none")}"
        End If

        txtWaypointsDetails.Text = _SF.GetAllWPCoordinates()

    End Sub

    Private Function BuildFPResultBaseOnAvailabilityOrNot(fromGroup As Boolean, fullDetailsMode As Boolean) As String

        Dim sb As New StringBuilder()
        Dim localizer As LocalizationManager = LocalizationManager.Instance
        Dim currentLanguage As SupportedLanguage = localizer.Language

        If fromGroup Then
            sb.AppendLine(localizer.Format("group.thread.task_details_header"))
        Else
            localizer.Language = SupportedLanguage.English
            sb.AppendLine($"# {txtTitle.Text}{AddFlagsToTitle()}")
            If chkRepost.Checked Then
                If txtRepostOriginalURL.TextLength > 0 Then
                    sb.AppendLine($"This task was originally posted on [{SupportingFeatures.ReturnDiscordServer(txtRepostOriginalURL.Text)}](<{txtRepostOriginalURL.Text}>) on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}")
                Else
                    sb.AppendLine($"This task was originally posted on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}")
                End If
            End If
        End If

        Dim altRestrictions As String = String.Empty
        If txtAltRestrictions.Text.Trim.Length > 33 Then
            altRestrictions = localizer.Format("group.thread.altitude_restrictions")
        End If
        Dim baroWarning As String = String.Empty
        If _WeatherDetails IsNot Nothing AndAlso (Not _WeatherDetails.IsStandardMSLPressure AndAlso (Not chkSuppressWarningForBaroPressure.Checked)) Then
            baroWarning = txtBaroPressureExtraInfo.Text.Trim
        End If

        sb.AppendLine()
        sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(txtShortDescription.Text,,, 1))
        Dim WSGEmoji As String = If(fromGroup, WSGEmojiForUser, WSGEmojiForBot)
        sb.AppendLine($"## {WSGEmoji}WeSimGlide.org")
        sb.AppendLine(localizer.Format("group.thread.task_link", _TaskEntrySeqID, $"{SupportingFeatures.WeSimGlide}index.html?task={_TaskEntrySeqID}"))
        If fromGroup Then
            If Not _taskDiscordPostID = String.Empty AndAlso Not (IsDelayedAvailabilityEnabledAndChecked() AndAlso chkAvailabilityRefly.Checked) Then
                sb.AppendLine(localizer.Format("group.thread.discord_header"))
                sb.AppendLine(localizer.Format("group.thread.discord_link", txtEventTitle.Text.Trim, $"{SupportingFeatures.TaskLibraryDiscordURL(_useTestMode)}/{_taskDiscordPostID}"))
            End If
        End If
        If fullDetailsMode Then
            sb.AppendLine(localizer.Format("group.thread.files_header"))
        Else
            Dim availabilityText As String = _SF.GetDiscordTimeStampForDate(AvailabilityDateTimeToUse, SupportingFeatures.DiscordTimeStampFormat.LongDateTime)
            Dim countDownAvailability As String = _SF.GetDiscordTimeStampForDate(AvailabilityDateTimeToUse, SupportingFeatures.DiscordTimeStampFormat.CountDown)
            sb.AppendLine(localizer.Format("group.thread.files_header_available", availabilityText, countDownAvailability))
        End If
        If _useTestMode Then
            sb.AppendLine(localizer.Format("group.thread.test_mode_warning"))
            sb.AppendLine()
        End If
        Dim pendingText As String = localizer.Format("group.thread.link_pending")
        Dim dphxLabel As String = If(fullDetailsMode, $"{txtTitle.Text.Trim}.dphx", pendingText)
        Dim zipLabel As String = If(fullDetailsMode, $"{txtTitle.Text.Trim}.zip", pendingText)
        Dim plnLabel As String = If(fullDetailsMode, Path.GetFileName(txtFlightPlanFile.Text), pendingText)
        Dim wprLabel As String = If(fullDetailsMode, Path.GetFileName(txtWeatherFile.Text), pendingText)
        sb.AppendLine(localizer.Format("group.thread.file_dphx1", dphxLabel, $"{SupportingFeatures.WeSimGlide}download.html?getFileFromDiscord=dphx&entrySeqID={_TaskEntrySeqID}"))
        sb.AppendLine(localizer.Format("group.thread.file_dphx2"))
        sb.AppendLine(localizer.Format("group.thread.file_zip1", zipLabel, $"{SupportingFeatures.WeSimGlide}php/DownloadDiscordFile.php?fileType=zip&entrySeqID={_TaskEntrySeqID}"))
        sb.AppendLine(localizer.Format("group.thread.file_zip2"))
        sb.AppendLine(localizer.Format("group.thread.file_pln1", plnLabel, $"{SupportingFeatures.WeSimGlide}php/DownloadDiscordFile.php?fileType=pln&entrySeqID={_TaskEntrySeqID}"))
        sb.AppendLine(localizer.Format("group.thread.file_pln2"))
        sb.AppendLine(localizer.Format("group.thread.file_wpr1", wprLabel, $"{SupportingFeatures.WeSimGlide}php/DownloadDiscordFile.php?fileType=wpr&entrySeqID={_TaskEntrySeqID}"))
        sb.AppendLine(localizer.Format("group.thread.file_wpr2"))
        Dim primaryPresetMismatch As Boolean = _WeatherDetails IsNot Nothing AndAlso _WeatherDetails.PresetName.Trim <> Path.GetFileNameWithoutExtension(txtWeatherFile.Text.Trim)
        If primaryPresetMismatch Then
            If fullDetailsMode Then
                sb.AppendLine(localizer.Format("group.thread.weather_note_exact", _WeatherDetails.PresetName))
            Else
                sb.AppendLine(localizer.Format("group.thread.weather_note_different"))
            End If
        End If
        If Not String.IsNullOrWhiteSpace(_secondaryWPRFilename) Then
            Dim wprSecondaryLabel As String = If(fullDetailsMode, Path.GetFileName(_secondaryWPRFilename), pendingText)
            sb.AppendLine(localizer.Format("group.thread.file_wpr_secondary1", wprSecondaryLabel, $"{SupportingFeatures.WeSimGlide}php/DownloadDiscordFile.php?fileType=wpr2&entrySeqID={_TaskEntrySeqID}"))
            sb.AppendLine(localizer.Format("group.thread.file_wpr_secondary2"))
            Dim secondaryPresetMismatch As Boolean = Not String.IsNullOrWhiteSpace(_secondaryWeatherPresetName) AndAlso Not String.IsNullOrWhiteSpace(_secondaryWPRFilename) AndAlso _secondaryWeatherPresetName.Trim <> Path.GetFileNameWithoutExtension(_secondaryWPRFilename.Trim)
            If secondaryPresetMismatch Then
                If fullDetailsMode Then
                    sb.AppendLine(localizer.Format("group.thread.weather_note_secondary_exact", _secondaryWeatherPresetName))
                Else
                    sb.AppendLine(localizer.Format("group.thread.weather_note_secondary_different"))
                End If
            End If
        End If
        sb.AppendLine()

        sb.AppendLine(localizer.Format("Group.thread.task_details"))
        If txtMainArea.Text.Trim.Length > 0 Then
            sb.AppendLine("> 🗺 " & SupportingFeatures.ValueToAppendIfNotEmpty(txtMainArea.Text))
        End If

        If fullDetailsMode Then sb.AppendLine($"> 🛫 {SupportingFeatures.ValueToAppendIfNotEmpty(txtDepartureICAO.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDepName.Text, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDepExtraInfo.Text, True, True)}")
        If fullDetailsMode Then sb.AppendLine($"> 🛬 {SupportingFeatures.ValueToAppendIfNotEmpty(txtArrivalICAO.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtArrivalName.Text, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtArrivalExtraInfo.Text, True, True)}")
        Dim localSimDateText As String = FormatMsfsLocalDate(localizer.Language)
        Dim simExtraInfo As String = SupportingFeatures.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text.Trim, True, True)
        sb.AppendLine(localizer.Format("group.thread.msfs_local", localSimDateText, simExtraInfo))
        If fullDetailsMode Then sb.AppendLine($"> ↗️ {GetSoaringTypesSelected()}{SupportingFeatures.ValueToAppendIfNotEmpty(txtSoaringTypeExtraInfo.Text, True, True)}")
        If fullDetailsMode AndAlso txtWeatherSummary.Text.Trim.Length > 0 Then sb.AppendLine($"> ⛅ {txtWeatherSummary.Text.Trim}")
        If fullDetailsMode Then
            Dim taskTxt As String = localizer.Format("distance.task")
            Dim totalTxt As String = localizer.Format("distance.total")
            sb.AppendLine($"> 📏 {SupportingFeatures.GetDistance(txtDistanceTotal.Text, txtDistanceTrack.Text,, taskTxt, totalTxt)}")
        End If
        sb.AppendLine($"> {GetLocalizedDurationText(False)}")
        If txtAATTask.Text.Length > 0 Then
            sb.AppendLine($"> ⚠️ {txtAATTask.Text}")
        End If
        sb.AppendLine($"> ✈️ {SupportingFeatures.ValueToAppendIfNotEmpty(cboRecommendedGliders.Text)}")
        sb.AppendLine($"> 🎚 {SupportingFeatures.GetDifficulty(cboDifficulty.SelectedIndex, txtDifficultyExtraInfo.Text)}")

        If Not String.IsNullOrEmpty(altRestrictions) OrElse Not String.IsNullOrEmpty(baroWarning) Then
            Dim connector As String = If(Not String.IsNullOrEmpty(altRestrictions) AndAlso Not String.IsNullOrEmpty(baroWarning), " & ", String.Empty)
            sb.AppendLine($"> ⚠️ {altRestrictions}{connector}{baroWarning}")
        End If

        If Not fromGroup Then
            sb.AppendLine()
            'Build the credits text based on user to tag
            sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(ProcessCredits(txtCredits.Text),,, 1))
            If chkDPOIncludeCoverImage.Enabled AndAlso chkDPOIncludeCoverImage.Checked AndAlso cboCoverImage.Text.Trim <> String.Empty Then
                'Add cover image
                Dim baseUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}TaskBrowser/Covers/"
                Dim fakeVersion As String = (New Random()).Next(100000, 999999).ToString()
                sb.AppendLine($"[Task Cover]({baseUrl}{_TaskEntrySeqID.ToString}.jpg?v={fakeVersion})")
            End If
            localizer.Language = currentLanguage
        End If

        Return sb.ToString.Trim

    End Function

    Private ReadOnly Property IsDelayedAvailability() As Boolean
        Get
            Return AvailabilityDateTimeToUse > Now()
        End Get
    End Property

    Private ReadOnly Property IsDelayedAvailabilityEnabledAndChecked() As Boolean
        Get
            Return chkDelayedAvailability.Enabled AndAlso chkDelayedAvailability.Checked
        End Get
    End Property

    Function ProcessCredits(ByVal input As String) As String
        Dim pattern As String = "@([\w\.]+)"
        Dim regex As New Regex(pattern)
        Dim output As String = regex.Replace(input, Function(m)
                                                        Dim username As String = m.Groups(1).Value
                                                        If _SF.DiscordNameIDPair.ContainsKey(username) Then
                                                            Return "<@" & _SF.DiscordNameIDPair(username) & ">"
                                                        Else
                                                            Return m.Value ' Leave unchanged if not found
                                                        End If
                                                    End Function)
        Return output
    End Function

    Private Function AddFlagsToTitle() As String
        Dim answer As New StringBuilder

        If lstAllCountries.Items.Count > 0 Then
            For Each country As String In lstAllCountries.Items
                answer.Append($" {_SF.CountryFlagCodes(country).Item1}")
            Next
        End If

        Return answer.ToString

    End Function

    Private Function GetSoaringTypesSelected() As String
        Dim selectedTypes As New List(Of String)
        Dim localizer As LocalizationManager = LocalizationManager.Instance

        If chkSoaringTypeRidge.Checked Then
            selectedTypes.Add(localizer.Format("lifttype.ridge"))
        End If

        If chkSoaringTypeThermal.Checked Then
            selectedTypes.Add(localizer.Format("lifttype.thermal"))
        End If

        If chkSoaringTypeWave.Checked Then
            selectedTypes.Add(localizer.Format("lifttype.wave"))
        End If

        If chkSoaringTypeDynamic.Checked Then
            selectedTypes.Add(localizer.Format("lifttype.dynamic"))
        End If

        ' Join the selected types into a single string, separated by ", "
        Return String.Join(", ", selectedTypes)

    End Function

    Private Sub LoadFlightPlan(filename As String)

        _PossibleElevationUpdateRequired = False
        lblElevationUpdateWarning.Visible = _PossibleElevationUpdateRequired
        lblElevationUpdateWarning.Text = String.Empty

        filename = SupportingFeatures.SanitizeFilePath(filename)

        If String.IsNullOrEmpty(filename) Then
            Return
        End If

        'read file
        txtFlightPlanFile.Text = filename
        _XmlDocFlightPlan.Load(filename)

        _OriginalFlightPlanTitle = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Title").Item(0).FirstChild.Value
        If Not chkTitleLock.Checked Then
            txtTitle.Text = _OriginalFlightPlanTitle
        End If

        If _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Count > 0 AndAlso _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Item(0).FirstChild IsNot Nothing Then
            _OriginalFlightPlanDepRwy = $"Rwy {_XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Item(0).FirstChild.Value}"
            If (Not chkDepartureLock.Checked) Then
                txtDepExtraInfo.Text = _OriginalFlightPlanDepRwy
            End If
        End If

        _OriginalFlightPlanShortDesc = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Descr").Item(0).FirstChild.Value
        If Not chkDescriptionLock.Checked Then
            txtShortDescription.Text = _OriginalFlightPlanShortDesc
        End If

        txtDepartureICAO.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DepartureID").Item(0).FirstChild.Value
        _OriginalFlightPlanDeparture = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DepartureName").Item(0).FirstChild.Value
        If Not chkDepartureLock.Checked Then
            txtDepName.Text = _OriginalFlightPlanDeparture
        End If
        txtArrivalICAO.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DestinationID").Item(0).FirstChild.Value
        _OriginalFlightPlanArrival = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DestinationName").Item(0).FirstChild.Value
        If Not chkArrivalLock.Checked Then
            txtArrivalName.Text = _OriginalFlightPlanArrival
        End If

        txtAltRestrictions.Text = _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, _FlightTotalDistanceInKm, _TaskTotalDistanceInKm, _PossibleElevationUpdateRequired)
        Dim hasBelowGroundRestriction As Boolean = False
        For Each waypoint As ATCWaypoint In _SF.AllWaypoints
            If waypoint.PossibleInvalidRestriction Then
                hasBelowGroundRestriction = True
                Exit For
            End If
        Next

        'Check if there is an AATMinDuration
        If _SF.AATMinDuration > TimeSpan.Zero Then
            txtAATTask.Enabled = True
            txtAATTask.Text = $"AAT with a minimum duration of {SupportingFeatures.FormatTimeSpanAsText(_SF.AATMinDuration)}"
        Else
            txtAATTask.Enabled = False
            txtAATTask.Text = String.Empty
        End If

        Dim warningMessages As New List(Of String)
        If _PossibleElevationUpdateRequired Then
            warningMessages.Add("Possible elevation update required!")
        End If
        If hasBelowGroundRestriction Then
            warningMessages.Add("Altitude restriction below ground!")
        End If
        lblElevationUpdateWarning.Visible = warningMessages.Count > 0
        If warningMessages.Count > 0 Then
            lblElevationUpdateWarning.Text = String.Join(" ", warningMessages)
        End If
        txtDistanceTotal.Text = FormatNumber(_FlightTotalDistanceInKm, 0)

        If _TaskTotalDistanceInKm = 0 Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The task distance is 0! You should open the task in the B21 Online Task Planer and re-download the flight plan again.", "Possible error in flight plan data", vbOKOnly, MessageBoxIcon.Warning)
            End Using
        End If
        txtDistanceTrack.Text = FormatNumber(_TaskTotalDistanceInKm, 0)

        HighlightExpectedFields()

        'BuildFPResults()
        'BuildGroupFlightPost()

        SessionModified(SourceOfChange.TaskTab)

    End Sub

    Private Sub NbrCarsCheckDiscordLimit(lblLabel As Windows.Forms.Label, Optional skipSetHeight As Boolean = False)
        Select Case CInt(lblLabel.Text.Split(" ")(0))
            Case > DiscordLimit
                lblLabel.Font = New Font(lblLabel.Font, lblLabel.Font.Style Or FontStyle.Bold)
                lblLabel.ForeColor = Color.Red
                ToolTip1.SetToolTip(lblLabel, "Caution! Over Discord limit!")
            Case > DiscordLimit - 200
                lblLabel.Font = New Font(lblLabel.Font, lblLabel.Font.Style And Not FontStyle.Bold)
                lblLabel.ForeColor = Color.Red
                ToolTip1.SetToolTip(lblLabel, "Caution! Approaching Discord limit!")
            Case Else
                lblLabel.Font = New Font(lblLabel.Font, lblLabel.Font.Style And Not FontStyle.Bold)
                lblLabel.ForeColor = Color.Black
                ToolTip1.SetToolTip(lblLabel, "Under Discord limit!")
        End Select
    End Sub

    Private Sub UpdateSecondaryWeatherPresetName()
        _secondaryWeatherPresetName = String.Empty

        If String.IsNullOrWhiteSpace(_secondaryWPRFilename) Then
            Return
        End If

        Dim secondaryWeatherFile As String = SupportingFeatures.SanitizeFilePath(_secondaryWPRFilename)
        _secondaryWeatherPresetName = SupportingFeatures.GetWeatherPresetTitleFromFile(secondaryWeatherFile)
    End Sub

    Private Sub LoadWeatherfile(filename As String)
        filename = SupportingFeatures.SanitizeFilePath(filename)

        If String.IsNullOrEmpty(filename) Then
            Return
        End If
        'read file
        txtWeatherFile.Text = filename

        'Fix weather
        If Not _SF.FixWPRFormat(filename, False) Then
        End If

        _XmlDocWeatherPreset.Load(filename)

        _WeatherDetails = Nothing
        _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        BuildWeatherInfoResults()
        'BuildGroupFlightPost()

        If Not (_WeatherDetails Is Nothing) Then
            BuildWeatherCloudLayers()
            BuildWeatherWindLayers()
        End If

        If _WeatherDetails.IsStandardMSLPressure Then
            chkSuppressWarningForBaroPressure.Enabled = False
            txtBaroPressureExtraInfo.Enabled = False
            lblNonStdBaroPressure.Enabled = False

        Else
            chkSuppressWarningForBaroPressure.Enabled = True
            txtBaroPressureExtraInfo.Enabled = True
            lblNonStdBaroPressure.Enabled = True
        End If

        SessionModified(SourceOfChange.TaskTab)

    End Sub

    Private Sub MoveExtraFilesSelectedItems(ByVal direction As Integer, ByVal listContrl As Windows.Forms.ListBox)
        Dim selectedIndices As List(Of Integer) = listContrl.SelectedIndices.Cast(Of Integer).ToList()
        If selectedIndices.Count = 0 Then
            Return
        End If

        Dim minIndex As Integer = selectedIndices.Min()
        Dim maxIndex As Integer = selectedIndices.Max()

        If direction = -1 AndAlso minIndex > 0 Then
            For Each index As Integer In selectedIndices
                Dim item As Object = listContrl.Items(index)
                listContrl.Items.RemoveAt(index)
                listContrl.Items.Insert(index - 1, item)
            Next
            listContrl.ClearSelected()
            For Each index As Integer In selectedIndices
                listContrl.SetSelected(index - 1, True)
            Next
        ElseIf direction = 1 AndAlso maxIndex < listContrl.Items.Count - 1 Then
            For Each index As Integer In selectedIndices.OrderByDescending(Function(i) i)
                Dim item As Object = listContrl.Items(index)
                listContrl.Items.RemoveAt(index)
                listContrl.Items.Insert(index + 1, item)
            Next
            listContrl.ClearSelected()
            For Each index As Integer In selectedIndices
                listContrl.SetSelected(index + 1, True)
            Next
        End If
    End Sub


#Region "Weather sections"

    Private Sub BuildWeatherInfoResults()

        Dim sb As New StringBuilder()

        sb.AppendLine("## 🌡 Weather Basic Information")

        If _WeatherDetails Is Nothing Then
            sb.Append($"- Summary: {SupportingFeatures.ValueToAppendIfNotEmpty(txtWeatherSummary.Text, nbrLineFeed:=1)}")
        Else
            sb.Append($"- Weather file & profile name: ""{Path.GetFileName(txtWeatherFile.Text)}"" ({_WeatherDetails.PresetName}){Environment.NewLine}")
            If Not txtWeatherSummary.Text.Trim = String.Empty Then
                sb.Append($"- Summary: {SupportingFeatures.ValueToAppendIfNotEmpty(txtWeatherSummary.Text)}{Environment.NewLine}")
            End If
            sb.Append($"- Elevation measurement: {_WeatherDetails.AltitudeMeasurement}{Environment.NewLine}")
            sb.Append($"- MSLPressure: {_WeatherDetails.MSLPressure(txtBaroPressureExtraInfo.Text, chkSuppressWarningForBaroPressure.Checked)}{Environment.NewLine}")
            sb.Append($"- MSLTemperature: {_WeatherDetails.MSLTemperature}{Environment.NewLine}")
            sb.Append($"- Aerosol index: {_WeatherDetails.Humidity}")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"{Environment.NewLine}- Precipitations: {_WeatherDetails.Precipitations}")
            End If
            If _WeatherDetails.HasSnowCover Then
                sb.Append($"{Environment.NewLine}- Snow Cover: {_WeatherDetails.SnowCover}")
            End If
            If _WeatherDetails.ThunderstormIntensity > 0 Then
                sb.Append($"{Environment.NewLine}- Lightning Intensity: {_WeatherDetails.ThunderstormIntensity}%")
            End If
        End If

        txtWeatherFirstPart.Text = sb.ToString.TrimEnd

    End Sub

    Private Sub BuildWeatherCloudLayers()
        txtWeatherClouds.Text = $"## 🌥 Cloud Layers{Environment.NewLine}"
        If _WeatherDetails IsNot Nothing Then
            txtWeatherClouds.AppendText(_WeatherDetails.CloudLayersText)
        End If
    End Sub

    Private Sub BuildWeatherWindLayers()
        txtWeatherWinds.Text = $"## 🌬 Wind Layers{Environment.NewLine}"
        If _WeatherDetails IsNot Nothing Then
            txtWeatherWinds.AppendText(_WeatherDetails.WindLayersAsString)
        End If
    End Sub

#End Region

#End Region

#End Region

#Region "Group Flights/Events Tab"

#Region "Event Handlers"

    Private Sub toolStripCurrentDateTime_Click(sender As Object, e As EventArgs) Handles toolStripCurrentDateTime.Click

        _timeStampContextualMenuDateTime = Now()

        GetNowTimeOnlyWithoutSeconds.Text = $"{_timeStampContextualMenuDateTime.ToString("hh:mm tt", _EnglishCulture)} : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        GetNowFullWithDayOfWeek.Text = $"{_timeStampContextualMenuDateTime.ToString("dddd, MMMM d, yyyy h:mm tt", _EnglishCulture)} : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)}"
        GetNowLongDateTime.Text = $"{_timeStampContextualMenuDateTime.ToString("MMMM d, yyyy h:mm tt", _EnglishCulture)} : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.LongDateTime)}"
        GetNowCountdown.Text = $"Countdown format : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.CountDown)}"
        GetNowTimeStampOnly.Text = $"Timestamp only : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.TimeStampOnly)}"

    End Sub

    Private Sub OneMinuteTimer_Tick(sender As Object, e As EventArgs) Handles OneMinuteTimer.Tick
        toolStripCurrentDateTime.Text = Now.ToString("MMMM d, yyyy h:mm tt", _EnglishCulture)
    End Sub

    Private Sub TimeStampContextualMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TimeStampContextualMenu.Opening

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)
        Dim fullSyncFlyDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventSyncFlyDate, dtEventSyncFlyTime, chkDateTimeUTC.Checked)
        Dim fullLaunchDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventLaunchDate, dtEventLaunchTime, chkDateTimeUTC.Checked)
        Dim fullStartTaskDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventStartTaskDate, dtEventStartTaskTime, chkDateTimeUTC.Checked)

        Select Case DirectCast(TimeStampContextualMenu.SourceControl, Windows.Forms.Label).Name
            Case lblMeetTimeResult.Name
                _timeStampContextualMenuDateTime = fullMeetDateTimeLocal
            Case lblSyncTimeResult.Name
                _timeStampContextualMenuDateTime = fullSyncFlyDateTimeLocal
            Case lblLaunchTimeResult.Name
                _timeStampContextualMenuDateTime = fullLaunchDateTimeLocal
            Case lblStartTimeResult.Name
                _timeStampContextualMenuDateTime = fullStartTaskDateTimeLocal
        End Select

        GetTimeStampTimeOnlyWithoutSeconds.Text = $"{_timeStampContextualMenuDateTime.ToString("hh:mm tt", _EnglishCulture)} : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        GetFullWithDayOfWeek.Text = $"{_timeStampContextualMenuDateTime.ToString("dddd, MMMM d, yyyy h:mm tt", _EnglishCulture)} : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)}"
        GetLongDateTime.Text = $"{_timeStampContextualMenuDateTime.ToString("MMMM d, yyyy h:mm tt", _EnglishCulture)} : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.LongDateTime)}"
        GetCountdown.Text = $"Countdown format : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.CountDown)}"
        GetTimeStampOnly.Text = $"Timestamp only : {_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, SupportingFeatures.DiscordTimeStampFormat.TimeStampOnly)}"

    End Sub

    Private Sub TimeStampContextualMenu_Click(sender As Object, e As EventArgs) Handles GetTimeStampTimeOnlyWithoutSeconds.Click,
                                                                                        GetFullWithDayOfWeek.Click,
                                                                                        GetLongDateTime.Click,
                                                                                        GetCountdown.Click,
                                                                                        GetTimeStampOnly.Click,
                                                                                        GetNowTimeOnlyWithoutSeconds.Click,
                                                                                        GetNowFullWithDayOfWeek.Click,
                                                                                        GetNowLongDateTime.Click,
                                                                                        GetNowCountdown.Click,
                                                                                        GetNowTimeStampOnly.Click

        SelectProperTimeStampContextMenu(sender)

    End Sub

    Private Sub SelectProperTimeStampContextMenu(sender As ToolStripMenuItem)
        Dim formatSelected As SupportingFeatures.DiscordTimeStampFormat
        Select Case sender.Name
            Case GetTimeStampTimeOnlyWithoutSeconds.Name, GetNowTimeOnlyWithoutSeconds.Name
                formatSelected = SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds
            Case GetFullWithDayOfWeek.Name, GetNowFullWithDayOfWeek.Name
                formatSelected = SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek
            Case GetLongDateTime.Name, GetNowLongDateTime.Name
                formatSelected = SupportingFeatures.DiscordTimeStampFormat.LongDateTime
            Case GetCountdown.Name, GetNowCountdown.Name
                formatSelected = SupportingFeatures.DiscordTimeStampFormat.CountDown
            Case GetTimeStampOnly.Name, GetNowTimeStampOnly.Name
                formatSelected = SupportingFeatures.DiscordTimeStampFormat.TimeStampOnly
        End Select

        Clipboard.SetText(_SF.GetDiscordTimeStampForDate(_timeStampContextualMenuDateTime, formatSelected))

    End Sub

    Private Sub ClubSelected(sender As Object, e As EventArgs) Handles cboGroupOrClubName.SelectedIndexChanged

        If _isInitiatizing OrElse _loadingFile Then
            Exit Sub
        End If

        chkDateTimeUTC.Enabled = True

        If cboGroupOrClubName.Text <> cboGroupOrClubName.Text.Trim Then
            cboGroupOrClubName.Text = cboGroupOrClubName.Text.Trim
        End If

        SetClubPreset()

        txtClubFullName.Text = String.Empty
        txtTrackerGroup.Text = String.Empty
        lblGroupEmoji.Text = String.Empty

        If _ClubPreset IsNot Nothing Then

            If _ClubPreset.IsCustom Then
                txtClubFullName.ReadOnly = False
                txtClubFullName.Text = "Specify your own club name"
                txtTrackerGroup.Text = String.Empty
                lblGroupEmoji.Text = String.Empty
                lblEventClubNotAuthorized.Visible = False
            Else
                txtClubFullName.ReadOnly = True
                cboGroupOrClubName.Text = _ClubPreset.ClubId
                txtTrackerGroup.Text = _ClubPreset.TrackerGroup
                lblGroupEmoji.Text = _ClubPreset.Emoji
                If _ClubPreset.ClubFullName.Trim.ToUpper.Contains(_ClubPreset.ClubName.Trim.ToUpper) Then
                    txtClubFullName.Text = _ClubPreset.ClubFullName
                Else
                    txtClubFullName.Text = $"{_ClubPreset.ClubFullName} {_ClubPreset.ClubName}"
                End If
            End If

            cboMSFSServer.Text = _ClubPreset.MSFSServer
            cboVoiceChannel.Text = _ClubPreset.VoiceChannel
            CheckAndSetEventAward()
            chkDateTimeUTC.Checked = True
            chkDateTimeUTC.Enabled = False

            'Try with standard time first
            dtEventMeetDate.Value = _SF.FindNextDate(Now, _ClubPreset.EventDayOfWeek, _ClubPreset.ZuluTime)
            dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value, True)
            'Now with the standard date and time, check if summer time applies
            Dim fullEventMeetDate As DateTime = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, False)
            If _ClubPreset.IsSummerTime(fullEventMeetDate) Then
                'Yes, so recheck the proper date and time for summer
                dtEventMeetDate.Value = _SF.FindNextDate(Now, _ClubPreset.SummerEventDayOfWeek, _ClubPreset.SummerZuluTime)
                dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value, False)
            End If

            Application.DoEvents()

            If _ClubPreset.ForceSyncFly AndAlso Not chkUseSyncFly.Checked Then
                chkUseSyncFly.Checked = True
            End If
            dtEventSyncFlyTime.Value = dtEventMeetTime.Value.AddMinutes(_ClubPreset.SyncFlyDelay)
            Application.DoEvents()

            If _ClubPreset.ForceLaunch AndAlso Not chkUseLaunch.Checked Then
                chkUseLaunch.Checked = True
            End If
            dtEventLaunchTime.Value = dtEventSyncFlyTime.Value.AddMinutes(_ClubPreset.LaunchDelay)
            Application.DoEvents()

            If _ClubPreset.ForceStartTask AndAlso Not chkUseStart.Checked Then
                chkUseStart.Checked = True
            End If
            dtEventStartTaskTime.Value = dtEventLaunchTime.Value.AddMinutes(_ClubPreset.StartTaskDelay)

            cboBeginnersGuide.Text = _ClubPreset.BeginnerLink
            If txtEventTitle.Text.Trim = String.Empty Then
                txtEventTitle.Text = TaskTitleMinusClubName()
            End If

            'Manage shared publishers for this club
            If (Not _loadingFile) AndAlso grpDiscordTask.Enabled AndAlso cboTaskOwner.Enabled Then
                'It is possible for the user to post the task and manage the owner, so shared users CAN be selected
                For Each publisherName As String In _ClubPreset.SharedPublishers
                    If publisherName = cboTaskOwner.Text OrElse chkcboSharedWithUsers.GetSelectedItems.Contains(publisherName) Then
                    Else
                        Dim listSingleUser As New List(Of String)
                        listSingleUser.Add(publisherName)
                        chkcboSharedWithUsers.SelectItemsByNames(listSingleUser)
                    End If
                Next
            End If
        End If

        'BuildGroupFlightPost()
        grpDiscordGroupFlight.Enabled = chkActivateEvent.Checked AndAlso IsClubAuthorizedEventOrganizer

        SessionModified(SourceOfChange.EventTab)

    End Sub

    Private Sub SetClubPreset()
        Dim clubExists As Boolean = _SF.DefaultKnownClubEvents.ContainsKey(cboGroupOrClubName.Text.ToUpper)
        If Not clubExists Then
            cboGroupOrClubName.Text = "CUSTOM"
            clubExists = True
            txtClubFullName.ReadOnly = False
            If txtClubFullName.Text.Trim = String.Empty Then
                txtClubFullName.Text = "Specify your own club name"
            End If
            txtTrackerGroup.Text = String.Empty
            lblGroupEmoji.Text = String.Empty
        End If
        If clubExists Then
            _ClubPreset = _SF.DefaultKnownClubEvents(cboGroupOrClubName.Text.ToUpper)
            'Check if user is authorized to publish events for this club
            If _ClubPreset.AuthorizedPublishers.Contains(lblWSGUserName.Text) OrElse _ClubPreset.ClubId = "CUSTOM" Then
                lblEventClubNotAuthorized.Visible = False
            Else
                lblEventClubNotAuthorized.Visible = True
            End If
        Else
            _ClubPreset = Nothing
            lblEventClubNotAuthorized.Visible = False
        End If
        'Correctly assign
        LocalizationManager.Instance.Language = _ClubPreset.Language
    End Sub
    Private Sub btnEventSelectNextWeekClicked(sender As Object, e As EventArgs) Handles btnEventSelectNextWeek.Click

        If _ClubPreset IsNot Nothing Then
            Dim startingDate As DateTime = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, True)
            'Try with standard time first
            dtEventMeetDate.Value = _SF.FindNextDate(startingDate, _ClubPreset.EventDayOfWeek, _ClubPreset.ZuluTime)
            dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value, True)
            'Now with the standard date and time, check if summer time applies
            Dim fullEventMeetDate As DateTime = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, False)
            If _ClubPreset.IsSummerTime(fullEventMeetDate) Then
                'Yes, so recheck the proper date and time for summer
                dtEventMeetDate.Value = _SF.FindNextDate(startingDate, _ClubPreset.SummerEventDayOfWeek, _ClubPreset.SummerZuluTime)
                dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value, False)
            End If
            Application.DoEvents()
        Else
            dtEventMeetDate.Value = dtEventMeetDate.Value.AddDays(7)
        End If

    End Sub

    Private Sub btnEventSelectPrevWeekClicked(sender As Object, e As EventArgs) Handles btnEventSelectPrevWeek.Click
        If _ClubPreset IsNot Nothing Then
            Dim startingDate As DateTime = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, True)
            'Try with standard time first
            dtEventMeetDate.Value = _SF.FindPreviousDate(startingDate, _ClubPreset.EventDayOfWeek, _ClubPreset.ZuluTime)
            dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value, True)
            'Now with the standard date and time, check if summer time applies
            Dim fullEventMeetDate As DateTime = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, False)
            If _ClubPreset.IsSummerTime(fullEventMeetDate) Then
                'Yes, so recheck the proper date and time for summer
                dtEventMeetDate.Value = _SF.FindPreviousDate(startingDate.AddHours(-1), _ClubPreset.SummerEventDayOfWeek, _ClubPreset.SummerZuluTime)
                dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value, False)
            End If
            Application.DoEvents()
        Else
            dtEventMeetDate.Value = dtEventMeetDate.Value.AddDays(-7)
        End If

    End Sub

    Private Sub EventDateChanged(sender As Object, e As EventArgs) Handles dtEventSyncFlyDate.ValueChanged, dtEventStartTaskDate.ValueChanged, dtEventMeetDate.ValueChanged, dtEventLaunchDate.ValueChanged

        If (_isInitiatizing AndAlso Not _loadingFile) OrElse _eventDateTimeChangeInProgress Then
            Exit Sub
        End If

        Dim changedField As DateTimePicker = DirectCast(sender, DateTimePicker)

        _eventDateTimeChangeInProgress = True

        Select Case changedField.Name
            Case dtEventMeetDate.Name
                dtEventSyncFlyDate.Value = dtEventMeetDate.Value
                dtEventLaunchDate.Value = dtEventSyncFlyDate.Value
                dtEventStartTaskDate.Value = dtEventLaunchDate.Value

            Case dtEventSyncFlyDate.Name
                dtEventLaunchDate.Value = dtEventSyncFlyDate.Value
                dtEventStartTaskDate.Value = dtEventLaunchDate.Value

            Case dtEventLaunchDate.Name
                dtEventStartTaskDate.Value = dtEventLaunchDate.Value

        End Select

        _eventDateTimeChangeInProgress = False

        BuildEventDatesTimes()
        SessionModified(SourceOfChange.EventTab)
        CalculateTaskAvailability()
    End Sub

    Private Sub EventTimeChanged(sender As Object, e As EventArgs) Handles dtEventSyncFlyTime.ValueChanged, dtEventStartTaskTime.ValueChanged, dtEventMeetTime.ValueChanged, dtEventLaunchTime.ValueChanged

        Dim changedField As DateTimePicker = DirectCast(sender, DateTimePicker)

        Select Case changedField.Name
            Case dtEventMeetTime.Name
                If _ClubPreset IsNot Nothing Then
                    dtEventSyncFlyTime.Value = dtEventMeetTime.Value.AddMinutes(_ClubPreset.SyncFlyDelay)
                Else
                    dtEventSyncFlyTime.Value = dtEventMeetTime.Value
                End If

            Case dtEventSyncFlyTime.Name
                If _ClubPreset IsNot Nothing Then
                    dtEventLaunchTime.Value = dtEventSyncFlyTime.Value.AddMinutes(_ClubPreset.LaunchDelay)
                Else
                    dtEventLaunchTime.Value = dtEventSyncFlyTime.Value
                End If


            Case dtEventLaunchTime.Name
                If _ClubPreset IsNot Nothing Then
                    dtEventStartTaskTime.Value = dtEventLaunchTime.Value.AddMinutes(_ClubPreset.StartTaskDelay)
                Else
                    dtEventStartTaskTime.Value = dtEventLaunchTime.Value
                End If

        End Select

        BuildEventDatesTimes()
        SessionModified(SourceOfChange.EventTab)

    End Sub

    Private Sub chkDateTimeUTC_CheckedChanged(sender As Object, e As EventArgs) Handles chkDateTimeUTC.CheckedChanged
        BuildEventDatesTimes()
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub GroupFlightFieldLeave(sender As Object, e As EventArgs) Handles chkUseSyncFly.CheckedChanged, chkUseStart.CheckedChanged, chkUseLaunch.CheckedChanged, cboVoiceChannel.SelectedIndexChanged, cboMSFSServer.SelectedIndexChanged, cboEligibleAward.SelectedIndexChanged
        'BuildGroupFlightPost()
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub cboBeginnersGuide_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBeginnersGuide.SelectedIndexChanged
        If cboBeginnersGuide.Text = "Other (provide link below)" Then
            txtOtherBeginnerLink.Enabled = True
            btnPasteBeginnerLink.Enabled = True
        Else
            txtOtherBeginnerLink.Enabled = False
            btnPasteBeginnerLink.Enabled = False
            txtOtherBeginnerLink.Text = String.Empty
        End If
        'BuildGroupFlightPost()
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub btnPasteBeginnerLink_Click(sender As Object, e As EventArgs) Handles btnPasteBeginnerLink.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtOtherBeginnerLink.Text = Clipboard.GetText
        End If
        'BuildGroupFlightPost()
    End Sub

    Private Sub chkIncludeBeginnersHelpLink_CheckedChanged(sender As Object, e As EventArgs)
        'BuildGroupFlightPost()
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub EventTabTextControlLeave(sender As Object, e As EventArgs) Handles txtEventTitle.Leave, txtEventDescription.Leave, txtOtherBeginnerLink.Leave, txtEventTeaserMessage.Leave, txtClubFullName.Leave, txtTrackerGroup.Leave

        'Trim all text boxes!
        If TypeOf sender Is Windows.Forms.TextBox Then
            _SF.RemoveForbiddenPrefixes(sender)
            Dim theTextBox As Windows.Forms.TextBox = DirectCast(sender, Windows.Forms.TextBox)
            If theTextBox.Text <> theTextBox.Text.Trim Then
                theTextBox.Text = theTextBox.Text.Trim
            End If
        End If

        'Trim comboboxes!
        If TypeOf sender Is Windows.Forms.ComboBox Then
            Dim theComboBox As Windows.Forms.ComboBox = DirectCast(sender, Windows.Forms.ComboBox)
            If theComboBox.Text <> theComboBox.Text.Trim Then
                theComboBox.Text = theComboBox.Text.Trim
            End If
        End If

        _SF.RemoveForbiddenPrefixes(sender)
        LeavingTextBox(sender)

    End Sub

    Private Sub chkActivateEvent_CheckedChanged(sender As Object, e As EventArgs) Handles chkActivateEvent.CheckedChanged
        grpGroupEventPost.Enabled = chkActivateEvent.Checked
        grpDiscordGroupFlight.Enabled = chkActivateEvent.Checked AndAlso IsClubAuthorizedEventOrganizer
        chkDelayBasedOnEvent.Enabled = chkActivateEvent.Checked
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub chkEventTeaser_CheckedChanged(sender As Object, e As EventArgs) Handles chkEventTeaser.CheckedChanged
        grpEventTeaser.Enabled = chkEventTeaser.Checked
        If chkEventTeaser.Checked AndAlso (txtEventTeaserAreaMapImage.Text.Trim.Length > 0 OrElse txtEventTeaserMessage.Text.Trim.Length > 0) Then
            chkDGPOTeaser.Checked = True
        End If
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub btnClearEventTeaserAreaMap_Click(sender As Object, e As EventArgs) Handles btnClearEventTeaserAreaMap.Click
        txtEventTeaserAreaMapImage.Text = String.Empty
        SessionModified(SourceOfChange.EventTab)
    End Sub

    Private Sub btnSelectEventTeaserAreaMap_Click(sender As Object, e As EventArgs) Handles btnSelectEventTeaserAreaMap.Click

        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select teaser area map image"
        OpenFileDialog1.Filter = "Image files|*.jpg;*.png"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim selectedImage As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileName)

            SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(selectedImage)
            txtEventTeaserAreaMapImage.Text = selectedImage
            chkDGPOTeaser.Checked = True
            SessionModified(SourceOfChange.EventTab)
        End If

    End Sub

#End Region

#Region "Group Flights/Events tab subs & functions"

    Private Sub CheckAndSetEventAward()

        If (_ClubPreset IsNot Nothing) AndAlso _ClubPreset.EligibleAward AndAlso txtDistanceTrack.Text <> String.Empty Then

            Dim trackDistanceKM As Integer = CInt(txtDistanceTrack.Text)

            lblEventTaskDistance.Text = $"{trackDistanceKM} Km"
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
            If cboEligibleAward.Items.Count > 0 Then
                cboEligibleAward.SelectedIndex = 0
            End If
            lblEventTaskDistance.Visible = False
        End If
    End Sub

    Private Sub BuildEventDatesTimes()

        If _isInitiatizing AndAlso Not _loadingFile Then
            Exit Sub
        End If

        Dim eventDay As DayOfWeek

        Dim theDate As Date

        theDate = New Date(dtEventMeetDate.Value.Year, dtEventMeetDate.Value.Month, dtEventMeetDate.Value.Day, dtEventMeetTime.Value.Hour, dtEventMeetTime.Value.Minute, 0)
        lblMeetTimeResult.Text = _SF.FormatEventDateTime(theDate, eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblMeetTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        'Check if local DST applies for this date
        If _ClubPreset IsNot Nothing Then
            lblLocalDSTWarning.Visible = _ClubPreset.IsSummerTime(theDate)
        Else
            lblLocalDSTWarning.Visible = _SF.DSTAppliesForLocalDate(theDate)
        End If

        lblSyncTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventSyncFlyDate.Value.Year, dtEventSyncFlyDate.Value.Month, dtEventSyncFlyDate.Value.Day, dtEventSyncFlyTime.Value.Hour, dtEventSyncFlyTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblSyncTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        lblLaunchTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventLaunchDate.Value.Year, dtEventLaunchDate.Value.Month, dtEventLaunchDate.Value.Day, dtEventLaunchTime.Value.Hour, dtEventLaunchTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblLaunchTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        lblStartTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventStartTaskDate.Value.Year, dtEventStartTaskDate.Value.Month, dtEventStartTaskDate.Value.Day, dtEventStartTaskTime.Value.Hour, dtEventStartTaskTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblStartTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        _directChange = False
        dtAvailabilityDate.MaxDate = theDate.ToLocalTime
        dtAvailabilityTime.MaxDate = theDate.ToLocalTime
        _directChange = True

        'BuildGroupFlightPost()

    End Sub

#End Region

#End Region

#Region "Discord Tab"

#Region "Discord - Flight Plan event handlers"

    Private Sub chkDPOMainPost_EnabledChanged(sender As Object, e As EventArgs)
        If _sessionModified Then
            btnStartTaskPost.Enabled = _sessionModified
        Else
            btnStartTaskPost.Enabled = grbTaskInfo.Enabled
        End If
    End Sub

    Private Sub txtLastUpdateDescription_EnabledChanged(sender As Object, e As EventArgs) Handles txtLastUpdateDescription.EnabledChanged
        lblUpdateDescription.Enabled = txtLastUpdateDescription.Enabled
    End Sub

    Private Sub lblTaskBrowserIDAndDate_DoubleClick(sender As Object, e As EventArgs) Handles lblTaskBrowserIDAndDate.DoubleClick, btnWSGTaskLink.Click

        WeSimGlideTaskLinkPosting()

    End Sub

    Private Sub btnStartFullPostingWorkflow_Click(sender As Object, e As EventArgs) Handles btnStartFullPostingWorkflow.Click

        GroupEventOrFullWorkflow(False)

    End Sub

    Private Sub btnDPORecallSettings_Click(sender As Object, e As EventArgs) Handles btnDPORecallSettings.Click

        LoadDPOptions()

    End Sub

    Private Sub btnDPORememberSettings_Click(sender As Object, e As EventArgs) Handles btnDPORememberSettings.Click
        SessionSettings.DPO_DPOUseCustomSettings = True
        SessionSettings.DPO_chkDPOIncludeCoverImage = chkDPOIncludeCoverImage.Checked
    End Sub

    Private Sub chkRepost_CheckedChanged(sender As Object, e As EventArgs) Handles chkRepost.CheckedChanged

        dtRepostOriginalDate.Enabled = chkRepost.Checked
        txtRepostOriginalURL.Enabled = chkRepost.Checked
        btnRepostOriginalURLPaste.Enabled = chkRepost.Checked

        If dtRepostOriginalDate.Enabled Then
            dtRepostOriginalDate.Value = Now
        End If

        AllFieldChanges(sender, e)

    End Sub

    Private Sub btnDPOResetToDefault_Click(sender As Object, e As EventArgs) Handles btnDPOResetToDefault.Click

        chkDPOIncludeCoverImage.Checked = True

        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Sub btnStartTaskPost_Click(sender As Object, e As EventArgs) Handles btnStartTaskPost.Click

        If (Not _useTestMode) AndAlso (Not UserCanCreateTask) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "You currently don't have the access rights to create a task on WeSimGlide.org", "Missing rights", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        If chkRemindUserPostOptions.Checked Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Did you make sure to select the correct post options (check boxes) for what you want to post?", "Post Options Reminder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbNo Then
                    Exit Sub
                End If
            End Using
        End If

        Dim autoContinue As Boolean = True

        If txtTaskID.Text.Length = 0 Then
            If Not ValidPostingRequirements() Then
                Exit Sub
            End If
        End If

        If Not PostTaskInLibrary(autoContinue) Then
            Exit Sub
        End If

    End Sub

#End Region

#Region "Discord - Flight Plan Subs & Functions"

    Private Sub WeSimGlideTaskLinkPosting()

        If _TaskEntrySeqID > 0 Then
            Dim msgWeSimGlideLink As String = String.Empty
            msgWeSimGlideLink = $"## :wsg: WeSimGlide.org {Environment.NewLine}[Task #{_TaskEntrySeqID.ToString.Trim} on WeSimGlide.org]({SupportingFeatures.GetWeSimGlideTaskURL(_TaskEntrySeqID)})"
            Clipboard.SetText(msgWeSimGlideLink)

            CopyContent.ShowContent(Me,
                                msgWeSimGlideLink,
                                "Task uploaded to WeSimGlide.org! You can now paste the content of the message anywhere you want to share the WSG link.",
                                "Sharing WeSimGlide.org Task link",
                                "Anywhere you want to share the task link",
                                New List(Of String) From {"^v"})
        End If

    End Sub

    Private Function ValidPostingRequirements(Optional fromGroupOnly As Boolean = False) As Boolean

        Dim dlgResult As DialogResult

        Do While _sessionModified
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, "Latest changes have not been saved! You first need to save the session.", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.OK
                    btnSaveConfig_Click(Nothing, New EventArgs)
                Case DialogResult.Cancel
                    Return False
            End Select
        Loop

        If Not fromGroupOnly Then
            BuildFPResults()
            If HighlightExpectedFields(True) Then
                Return False
            End If
        End If

        Return True

    End Function

    Private Function PostTaskInGroupEvent(autoContinue As Boolean) As Boolean

        Dim msg As String = String.Empty
        Dim language As SupportedLanguage = GetGroupEventLanguage()
        Dim localizer As LocalizationManager = LocalizationManager.Instance
        Dim shouldRestoreFpResults As Boolean = False
        Dim originalFPResults As String = String.Empty
        Dim originalFPDelayed As String = String.Empty
        Dim originalFullDescription As String = String.Empty
        Dim localizedMainContent As String = Nothing
        Dim localizedFullDescription As String = Nothing

        Dim needLocalizedResults As Boolean = ((chkDGPOMainPost.Enabled AndAlso chkDGPOMainPost.Checked AndAlso Not (IsDelayedAvailabilityEnabledAndChecked() AndAlso chkAvailabilityRefly.Checked)) OrElse
                                              (chkDGPOFullDescription.Enabled AndAlso chkDGPOFullDescription.Checked AndAlso (Not IsDelayedAvailability)))

        If needLocalizedResults Then
            originalFPResults = txtFPResults.Text
            originalFPDelayed = txtFPResultsDelayedAvailability.Text
            originalFullDescription = txtFullDescriptionResults.Text

            BuildFPResults(True, False)
            localizedMainContent = If(IsDelayedAvailability, txtFPResultsDelayedAvailability.Text.Trim, txtFPResults.Text.Trim)
            localizedFullDescription = txtFullDescriptionResults.Text.Trim
            shouldRestoreFpResults = True
        End If

        'Event Logistics
        If chkDGPOEventLogistics.Enabled AndAlso chkDGPOEventLogistics.Checked Then
            msg = $"{GroupFlightEventThreadLogistics()}"
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            "Paste the message in the thread and post it.",
                            AppendIsTestMode("Pasting event logistics in group event thread"),
                            AppendIsTestMode("Group Event Thread"),
                            New List(Of String) From {"^v"},,,,
                            SupportingFeatures.GetTaskThreadURLFromGroupURL(txtGroupEventPostURL.Text.Trim))
            If Not autoContinue Then Return False
            msg = String.Empty
        End If

        'Map image
        If (chkDGPOMapImage.Enabled AndAlso chkDGPOMapImage.Checked AndAlso cboBriefingMap.Text.Trim <> String.Empty AndAlso (Not IsDelayedAvailability)) Then
            autoContinue = MapImageCopy(True)
            If Not autoContinue Then
                Return False
            End If
        End If

        'Main post
        If chkDGPOMainPost.Enabled AndAlso chkDGPOMainPost.Checked Then
            If IsDelayedAvailabilityEnabledAndChecked() AndAlso chkAvailabilityRefly.Checked Then
                msg = $"{msg}{localizer.Format("group.event.no_task_details")}"
            Else
                Dim mainContentToAppend As String = localizedMainContent
                If String.IsNullOrEmpty(mainContentToAppend) Then
                    mainContentToAppend = If(IsDelayedAvailability, txtFPResultsDelayedAvailability.Text.Trim, txtFPResults.Text.Trim)
                End If
                msg = $"{msg}{mainContentToAppend}"
            End If
        End If

        If (chkDGPOMapImage.Enabled AndAlso chkDGPOMapImage.Checked AndAlso cboBriefingMap.Text.Trim <> String.Empty AndAlso (Not IsDelayedAvailability)) Then
            'There is a map
            msg = $"{msg}{Environment.NewLine}{localizer.Format("group.event.map_header")}"
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            "Paste the message in the thread and post it.",
                            AppendIsTestMode("Pasting message in group event thread"),
                            AppendIsTestMode("Group Event Thread"),
                            New List(Of String) From {"^v"},,,,
                            SupportingFeatures.GetTaskThreadURLFromGroupURL(txtGroupEventPostURL.Text.Trim))
            msg = String.Empty
        End If

        If Not autoContinue Then Return False

        'Altitude restrictions
        If chkDGPOAltRestrictions.Enabled AndAlso chkDGPOAltRestrictions.Checked AndAlso (Not IsDelayedAvailability) Then
            If msg.Trim.Length > 0 Then
                msg = $"{msg}{Environment.NewLine}"
            End If
            msg = $"{msg}{GetLocalizedAltitudeRestrictions()}"
        End If

        'Full Description
        If chkDGPOFullDescription.Enabled AndAlso chkDGPOFullDescription.Checked AndAlso (Not IsDelayedAvailability) Then
            If msg.Trim.Length > 0 Then
                msg = $"{msg}{Environment.NewLine}"
            End If
            Dim fullDescriptionToAppend As String = If(String.IsNullOrEmpty(localizedFullDescription), txtFullDescriptionResults.Text.Trim, localizedFullDescription)
            msg = $"{msg}{fullDescriptionToAppend}"
        End If

        If shouldRestoreFpResults Then
            txtFPResults.Text = originalFPResults
            txtFPResultsDelayedAvailability.Text = originalFPDelayed
            txtFullDescriptionResults.Text = originalFullDescription
        End If

        If msg.Trim.Length > 0 Then
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            "Paste the message in the thread and post it.",
                            AppendIsTestMode("Pasting message in group event thread"),
                            AppendIsTestMode("Group Event Thread"),
                            New List(Of String) From {"^v"},,,,
                            SupportingFeatures.GetTaskThreadURLFromGroupURL(txtGroupEventPostURL.Text.Trim))
        End If

        Return autoContinue

    End Function

    Private Function PostTaskInLibrary(autoContinue As Boolean) As Boolean

        'Is there an EntrySeqID already?
        If _TaskEntrySeqID > 0 AndAlso (Not _useTestMode) Then

            If _TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation AndAlso
                txtTaskID.Text.Trim.Length = 0 AndAlso
                txtTemporaryTaskID.Text.Trim.Length > 0 Then
                'Scenario 1 - The task is pending creation and has only a temporary TaskID (no TaskID)
                '             We need to resume at posting task on Discord.
                autoContinue = PostTaskOnDiscordFirstTime()

            ElseIf _TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation AndAlso
                txtTaskID.Text.Trim.Length > 0 Then
                'Scenario 2 - The task is pending creation and has a TaskID (the task has been published to Discord but is still incomplete on WSG)
                '             We need to resume at trying to complete the publishing on WSG.
                autoContinue = PrepareCreateWSGTaskPart2()

            ElseIf _TaskStatus > SupportingFeatures.WSGTaskStatus.PendingCreation Then
                'Scenario 3 - The task status is > 10 (it's neither PendingCreation (10) or NotCreated (0))
                '             This is a true Update of an existing active task. - Check User rights for UPDATE!
                If UserCanUpdateTask Then
                    If _hasLeaderBoard AndAlso IsOwnerOrShared() Then
                        Using New Centered_MessageBox(Me)
                            If MessageBox.Show(Me, $"This task has IGC records. Updating some elements of the task could lead to valid IGC results becoming invalid!{Environment.NewLine}{Environment.NewLine}Are you sure you want to proceed?", "Leader Board exists for this task", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                                Return False
                            End If
                        End Using
                    End If
                    If txtLastUpdateDescription.TextLength = 0 Then
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show("Please provide a description for this task update!", "Publishing task update", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                        End Using
                    End If
                    BuildFPResults(False, False)
                    autoContinue = PrepareUpdateWSGTask()

                    'WE'RE DONE - EXIT!
                    Return autoContinue
                End If

            Else
                'Unplanned scenario! We have an EntrySeqID but the status is NotCreated ?!
                'This is a mistake and should never occur
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Error! Task exists on WSG but has invalid status of NotCreated. Cannot proceed.", "Incoherent status", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return False
            End If
        Else
            'NO EntrySeqID, then we go with full CREATE from the start.
            'We first need a temporary inactive TaskID (because we don't have the Discord post yet) to retreive our reserved EntrySeqID on WSG
            Dim taskInfo As AllData = SetAndRetrieveSessionData()
            txtTemporaryTaskID.Text = $"TEMP-{Guid.NewGuid().ToString}"
            If Not _useTestMode Then
                txtTaskID.Text = String.Empty
            ElseIf txtTaskID.Text.Length > 0 Then
                'Ask user
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show(Me, "Because you are testing, I need to know if you want to reset from the start? Did you delete the previous TASK Discord post?", "Test mode reset question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        txtTaskID.Text = String.Empty
                    End If
                End Using
            End If
            _TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation
            taskInfo.TemporaryTaskID = txtTemporaryTaskID.Text
            taskInfo.TaskStatus = _TaskStatus
            If Not _useTestMode Then
                autoContinue = CreateWSGTaskPart1()
            End If

            If autoContinue Then
                'We can now post the task on Discord
                autoContinue = PostTaskOnDiscordFirstTime()
            Else
                'Something went wrong - we cannot continue
                txtTemporaryTaskID.Text = String.Empty
                txtTaskID.Text = String.Empty
                _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
            End If
        End If

        Return autoContinue

    End Function

    Private Function ManualDiscordTaskPost() As Boolean

        Dim autoContinue As Boolean = True
        Dim origin As String = String.Empty
        Dim titleMsg As String = String.Empty

        If Not _taskDiscordPostID = String.Empty Then
            origin = $"WSG was unable to edit the task post. Please check and update it yourself if needed.{Environment.NewLine}You can now EDIT the corresponding task post in the FLIGHTS channel under TASK LIBRARY."
            titleMsg = "Updating main FP post"
        Else
            Dim msgAbove As String = String.Empty
            Dim msgBelow As String = String.Empty
            msgAbove = $"Task Main Post ({If(_useTestMode, "Test Mode", "Task Library")})"
            msgBelow = $"Once your cursor is in the right field {If(_useTestMode, "on the TESTING channel", String.Empty)} (use 'Take me there!'), you can click OK and start posting."
            autoContinue = MsgBoxWithPicture.ShowContent(Me,
                                                 "StartTaskWorkflow.gif",
                                                 msgAbove,
                                                 msgBelow,
                                                 AppendIsTestMode("Instructions to read before starting the post!"),
                                                 SupportingFeatures.TaskLibraryDiscordURL(_useTestMode))

            If Not autoContinue Then
                Return autoContinue
            End If
            origin = $"You can now post the main task message directly in the {If(_useTestMode, "test", "flights")} channel (use 'Take me there!')."
            titleMsg = "Creating main FP post"
        End If

        If IsDelayedAvailability Then
            Clipboard.SetText(txtFPResultsDelayedAvailability.Text)
        Else
            Clipboard.SetText(txtFPResults.Text)
        End If

        autoContinue = CopyContent.ShowContent(Me,
                            IIf(IsDelayedAvailability, txtFPResultsDelayedAvailability.Text, txtFPResults.Text),
                            $"{origin}{Environment.NewLine}Skip (Ok) if already done.",
                            AppendIsTestMode(titleMsg),
                            AppendIsTestMode("Task Main Post"),
                            New List(Of String) From {"^v"},,,, SupportingFeatures.TaskLibraryDiscordURL(_useTestMode))

        If Not autoContinue Then
            Return autoContinue
        End If

        If _taskDiscordPostID = String.Empty Then
            Dim message As String = "Please get the link to the task's post in Discord (""...More menu"" and ""Copy Message Link"")"
            Dim waitingForm As WaitingForURLForm
            Dim answer As DialogResult
            Dim validTaskIDOrCancel As Boolean = False

            Do Until validTaskIDOrCancel
                Clipboard.Clear()
                waitingForm = New WaitingForURLForm(message)
                answer = waitingForm.ShowDialog()

                SupportingFeatures.BringDPHToolToTop(Me.Handle)
                'Check if the clipboard contains a valid URL, which would mean the task's URL has been copied
                If answer = DialogResult.OK Then
                    Dim taskThreadURL As String
                    taskThreadURL = SupportingFeatures.DiscordURL(Clipboard.GetText, False)
                    _taskDiscordPostID = SupportingFeatures.ExtractMessageIDFromDiscordURL(taskThreadURL,, _useTestMode)
                    If _taskDiscordPostID.Trim.Length = 0 Then
                        Using New Centered_MessageBox(Me)
                            If MessageBox.Show(Me, $"Invalid Discord Post ID - If you posted under the Task Library, try again. If not, click Cancel.", "Discord Post ID missing", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                validTaskIDOrCancel = True
                                autoContinue = False
                            End If
                        End Using
                    Else
                        validTaskIDOrCancel = True
                        SaveSession()
                    End If
                Else
                    validTaskIDOrCancel = True
                    autoContinue = False
                End If
                If Not autoContinue Then
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me, $"Discord Post ID is missing - You should be posting your task to the Task Library and get the link to that post!{Environment.NewLine}", "Discord Post ID missingg", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End If
            Loop
        End If

        Return autoContinue

    End Function

    Private Function PostTaskOnDiscordFirstTime() As Boolean
        Dim taskInfo As AllData = SetAndRetrieveSessionData()
        Dim autoContinue As Boolean = False

        If Not _useTestMode Then
            'Retrieve the task details after first step of task creation
            GetTaskDetails(taskInfo.TemporaryTaskID, taskInfo.EntrySeqID)
            'Update taskInfo and save file with latest details
            taskInfo = SetAndRetrieveSessionData()
            SaveSession()
            txtTaskID.Text = $"P{taskInfo.TemporaryTaskID.Substring(4)}"
        Else
            txtTaskID.Text = $"T{taskInfo.TemporaryTaskID.Substring(4)}"
        End If

        'This will now allow us to post the task on Discord with proper links.
        BuildFPResults(False, False)

        If _useTestMode Then
            Dim msgContent As String = String.Empty
            If IsDelayedAvailability Then
                msgContent = txtFPResultsDelayedAvailability.Text
            Else
                msgContent = txtFPResults.Text
            End If
            Dim returnedDiscordPostID As String = String.Empty
            If PostTaskToTestChannel(msgContent, _taskDiscordPostID, False, returnedDiscordPostID) Then
                'Offer to user to go see the output?
                _taskDiscordPostID = returnedDiscordPostID
                Using New Centered_MessageBox
                    If MessageBox.Show(Me, "The task was posted on the Discord test channel. Would you like me to take you there?", "Task posted on test channel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                        'Goto to the discord post ID
                        If Not SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.TaskLibraryDiscordURL(True)}/{_taskDiscordPostID}") Then
                            Using New Centered_MessageBox()
                                MessageBox.Show("An error occured trying to access the task post on Discord.", "Error launching Discord", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Using
                        End If
                    End If
                End Using
            Else
                'The webhook didn't work - do it manually?
                ManualDiscordTaskPost()
            End If
        End If

        If (Not _useTestMode) AndAlso txtTaskID.Text.Trim.Length > 0 AndAlso (Not txtTaskID.Text.StartsWith("T")) Then
            autoContinue = PrepareCreateWSGTaskPart2()
        Else
            'We don't have the official Discord task ID, so the task should stay in Pending creation?
            If _useTestMode Then
                _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
                autoContinue = True
            Else
                _TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation
                autoContinue = False
            End If
            SaveSession()
        End If

        Return autoContinue

    End Function

    Private Function PrepareUpdateWSGTask() As Boolean
        'Update the WSG task
        SetAndRetrieveSessionData()
        SaveSession()
        SetPostingToWSGLabelVisibility(True)
        Dim autoContinue As Boolean = CreateWSGTaskPart2(True)
        SetPostingToWSGLabelVisibility(False)
        If Not autoContinue Then
            'Something went wrong trying to updated the task!!
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "An error occured trying to update the task. Task was not updated on WeSimGlide.org", "Task update error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
        'Retrieve the updated task details
        GetTaskDetails(txtTaskID.Text.Trim, _TaskEntrySeqID)
        Return autoContinue

    End Function

    Private Function PrepareCreateWSGTaskPart2() As Boolean
        'Then we can complete the entry on WSG with the files and correct TaskID
        SetPostingToWSGLabelVisibility(True)
        _TaskStatus = SupportingFeatures.WSGTaskStatus.Active
        SetAndRetrieveSessionData()
        SaveSession()
        Dim autoContinue As Boolean = CreateWSGTaskPart2()
        SetPostingToWSGLabelVisibility(False)
        If Not autoContinue Then
            'Something went wrong on the second part!!
            _TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation
            SaveSession()
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "An error occured trying to perform the second part of task creation.", "Task creation error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End If
        'Retrieve the updated task details
        GetTaskDetails(txtTaskID.Text.Trim, _TaskEntrySeqID)
        Return autoContinue

    End Function

    Private Sub SetPostingToWSGLabelVisibility(visible As Boolean)
        lblPostingToWSGMessage.Visible = visible
        Me.Refresh()
        Application.DoEvents()
    End Sub

    Private Sub SetWarningIconOnPostOptions()

        Dim listOfControlsRemove As New List(Of Windows.Forms.CheckBox)
        Dim listOfControlsAdd As New List(Of Windows.Forms.CheckBox)

        If lblTaskIDAcquired.Visible Then
            'Task
            listOfControlsRemove.Add(chkDPOIncludeCoverImage)
            'Group
            listOfControlsRemove.Add(chkDGPOMainPost)
            listOfControlsRemove.Add(chkDGPOFullDescription)
            listOfControlsRemove.Add(chkDGPOAltRestrictions)
            listOfControlsRemove.Add(chkDGPOMapImage)
            listOfControlsRemove.Add(chkDGPOEventLogistics)
        Else
            'Task
            listOfControlsAdd.Add(chkDPOIncludeCoverImage)
            'Group
            listOfControlsAdd.Add(chkDGPOMainPost)
            listOfControlsAdd.Add(chkDGPOAltRestrictions)
            listOfControlsAdd.Add(chkDGPOFullDescription)
            listOfControlsAdd.Add(chkDGPOMapImage)
            listOfControlsAdd.Add(chkDGPOEventLogistics)
        End If

        RemoveWarningFromCheckBox(listOfControlsRemove)
        AddWarningFromCheckBox(listOfControlsAdd)

    End Sub

    Private Sub RemoveWarningFromCheckBox(optionCheckBoxes As List(Of Windows.Forms.CheckBox))

        Dim warningIcon As String = "⚠️"

        For Each optionCheckBox As Windows.Forms.CheckBox In optionCheckBoxes
            If optionCheckBox.Text.StartsWith(warningIcon) Then
                optionCheckBox.Text = optionCheckBox.Text.Substring(2, optionCheckBox.Text.Length - 2).Trim
                optionCheckBox.ForeColor = Me.ForeColor
            End If
        Next

    End Sub

    Private Sub AddWarningFromCheckBox(optionCheckBoxes As List(Of Windows.Forms.CheckBox))

        Dim warningIcon As String = "⚠️"

        For Each optionCheckBox As Windows.Forms.CheckBox In optionCheckBoxes
            If Not optionCheckBox.Text.StartsWith(warningIcon) Then
                optionCheckBox.Text = $"{warningIcon} {optionCheckBox.Text}"
                optionCheckBox.ForeColor = lblTaskIDNotAcquired.ForeColor
            End If
        Next

    End Sub

    Private Sub CheckTestModes()
        If _userPermissionID = String.Empty Then
            chkTestMode.Visible = False
        Else
            chkTestMode.Visible = True
        End If
        lblTestMode.Visible = False
        If _useTestMode Then
            chkTestMode.Checked = True
            If SupportingFeatures.UsingTestServer Then
                lblTestMode.Text = $"You are in TEST mode{Environment.NewLine}&& WSG Test Server"
            Else
                lblTestMode.Text = "You are in TEST mode"
            End If
            lblTestMode.Visible = True
        ElseIf SupportingFeatures.UsingTestServer Then
            lblTestMode.Text = "Running with WSG Test Server"
            lblTestMode.Visible = True
        End If

        If Not _useTestMode Then
            If txtTaskID.Text.StartsWith("T") Then
                txtTaskID.Text = String.Empty
            End If
        End If

        chkWSGTask.Enabled = Not _useTestMode
        chkWSGTask.Checked = Not _useTestMode
        chkWSGEvent.Enabled = Not _useTestMode
        chkWSGEvent.Checked = Not _useTestMode

        grbWSGExtras.Enabled = Not _useTestMode

    End Sub

    Private Sub CheckWhichOptionsCanBeEnabled()

        'Is there a group selected for event news buttons
        SetEventLabelAndButtons()

        chkDGPOMapImage.Enabled = cboBriefingMap.Text.Trim.Length > 0
        chkDelayBasedOnEvent.Enabled = chkActivateEvent.Checked

        If cboCoverImage.SelectedItem IsNot Nothing AndAlso cboCoverImage.SelectedItem.ToString <> String.Empty Then
            chkDPOIncludeCoverImage.Enabled = True AndAlso grbTaskInfo.Enabled
            chkDGPOCoverImage.Enabled = True
        Else
            chkDPOIncludeCoverImage.Enabled = False
            chkDGPOCoverImage.Enabled = False
        End If

        'Altitude restrictions
        If txtAltRestrictions.Text.Trim.Length = 0 Then
            chkDGPOAltRestrictions.Enabled = False
        Else
            chkDGPOAltRestrictions.Enabled = True
        End If

        If txtLongDescription.Text.Trim.Length = 0 Then
            chkDGPOFullDescription.Enabled = False
        Else
            chkDGPOFullDescription.Enabled = True
        End If

        'Group Event
        If chkEventTeaser.Checked AndAlso (txtEventTeaserAreaMapImage.Text.Trim.Length > 0 OrElse txtEventTeaserMessage.Text.Trim.Length > 0) Then
            chkDGPOTeaser.Enabled = True
        Else
            chkDGPOTeaser.Checked = False
            chkDGPOTeaser.Enabled = False
        End If

        If Not chkDGPOTeaser.Checked AndAlso
           Not chkDGPOMainPost.Checked AndAlso
           Not chkDGPOAltRestrictions.Checked AndAlso
           Not chkDGPOFullDescription.Checked AndAlso
           Not chkDGPOMapImage.Checked AndAlso
           Not chkDGPOEventLogistics.Checked Then
            chkDGPOThreadCreation.Enabled = False
        Else
            chkDGPOThreadCreation.Enabled = True
        End If

        SetWarningIconOnPostOptions()

    End Sub

    Private Sub SetEventLabelAndButtons()

        If chkActivateEvent.Checked AndAlso _ClubPreset IsNot Nothing Then
            Dim key As String = WSGEventID

            If (Not _useTestMode) AndAlso VerifyNewsExists(key) = "success" Then
                lblWSGEventExists.Visible = True
                lblWSGEventAbsent.Visible = False
                btnDeleteEventNews.Enabled = UserCanDeleteEvent
            Else
                lblWSGEventExists.Visible = False
                lblWSGEventAbsent.Visible = True
                btnDeleteEventNews.Enabled = False
            End If
        Else
            lblWSGEventExists.Visible = False
            lblWSGEventAbsent.Visible = True
            btnDeleteEventNews.Enabled = False
        End If

    End Sub

    Private Function AppendIsTestMode(msg As String) As String
        Return $"{msg}{If(_useTestMode, " (Test Mode)", String.Empty)}"
    End Function

    Private Function CoverImage(takeMeThereURL As String, Optional fromGroup As Boolean = False) As Boolean

        Dim autoContinue As Boolean = True

        Clipboard.Clear()
        If cboCoverImage.SelectedItem IsNot Nothing AndAlso cboCoverImage.SelectedItem.ToString <> String.Empty Then
            Dim allFiles As New StringCollection
            If File.Exists(cboCoverImage.SelectedItem) Then
                allFiles.Add(cboCoverImage.SelectedItem)
                Clipboard.SetFileDropList(allFiles)
                autoContinue = CopyContent.ShowContent(Me,
                                    cboCoverImage.SelectedItem,
                                    $"On the Discord app, make sure you are on the proper channel (use 'Take me there!') and message field.{Environment.NewLine}Now paste the copied cover image as message.{Environment.NewLine}Skip (Ok) if already done.",
                                    AppendIsTestMode($"Posting the cover image for the {If(fromGroup, "group event", "task")}."),
                                    AppendIsTestMode($"{If(fromGroup, "Group Event Main Post", "Task Thread")}"),
                                    New List(Of String) From {"^v"},
                                    False,,
                                    Drawing.Image.FromFile(allFiles(0)),
                                    takeMeThereURL)
            Else
                autoContinue = True
            End If
        Else
            autoContinue = True
        End If

        Return autoContinue

    End Function

    Private Function MapImageCopy(Optional postingGroupEvent As Boolean = False) As Boolean

        Dim autoContinue As Boolean = True

        Dim allFiles As New Specialized.StringCollection
        Dim contentForMessage As New StringBuilder

        If cboBriefingMap.Text.Trim <> String.Empty Then
            allFiles.Add(cboBriefingMap.Text)
            contentForMessage.AppendLine(cboBriefingMap.Text)
        End If
        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    contentForMessage.ToString,
                                    $"Make sure you are on the thread's message field.{Environment.NewLine}Now paste the map image as the next message in the thread WITHOUT posting it and come back for the text for this message.",
                                    AppendIsTestMode("Posting the map image in the thread - actual map image first"),
                                    AppendIsTestMode($"{If(postingGroupEvent, "Group Event", "Task")} Thread"),
                                    New List(Of String) From {"^v"},
                                    False,,
                                    Drawing.Image.FromFile(allFiles(0)),
                                    SupportingFeatures.GetTaskThreadURLFromGroupURL(txtGroupEventPostURL.Text.Trim))
        End If

        Return autoContinue

    End Function

    Private Function GetDiscordLinkToTaskThread() As String
        Dim urlToTaskThread As String = String.Empty
        urlToTaskThread = $"{SupportingFeatures.TaskLibraryDiscordURL}/{_taskDiscordPostID}"
        Return urlToTaskThread
    End Function

    Private Function TaskFeatureOnGroupFlight() As String

        Dim sb As New StringBuilder

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)

        sb.AppendLine("## :calendar: Group Flight")
        sb.AppendLine($"This flight will be featured on the {txtClubFullName.Text} group flight of {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time.")

        'check which shared link is available
        If txtGroupEventPostURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtGroupEventPostURL.Text.Trim) Then
            sb.AppendLine($"[{txtClubFullName.Text} - {txtEventTitle.Text} - Group Event Link]({txtGroupEventPostURL.Text})")
        End If

        Return sb.ToString

    End Function

#End Region

#Region "Discord - Group Event event handlers"

    Private Sub btnDGPORecallSettings_Click(sender As Object, e As EventArgs) Handles btnDGPORecallSettings.Click

        LoadDGPOptions()

    End Sub

    Private Sub btnDGPORememberSettings_Click(sender As Object, e As EventArgs) Handles btnDGPORememberSettings.Click

        SessionSettings.DPO_DGPOUseCustomSettings = True
        SessionSettings.DPO_chkDGPOCoverImage = chkDGPOCoverImage.Checked
        SessionSettings.DPO_chkDGPOMainGroupPost = chkDGPOMainGroupPost.Checked
        SessionSettings.DPO_chkDGPOThreadCreation = chkDGPOThreadCreation.Checked
        SessionSettings.DPO_chkDGPOTeaser = chkDGPOTeaser.Checked
        SessionSettings.DPO_chkDGPOFilesWithFullLegend = chkDGPOMapImage.Checked
        SessionSettings.DPO_chkDGPOMainPost = chkDGPOMainPost.Checked
        SessionSettings.DPO_chkDGPOAltRestrictions = chkDGPOAltRestrictions.Checked
        SessionSettings.DPO_chkDGPOFullDescription = chkDGPOFullDescription.Checked
        SessionSettings.DPO_chkDGPOPublishWSGEventNews = chkDGPOPublishWSGEventNews.Checked
        SessionSettings.DPO_chkDGPOEventLogistics = chkDGPOEventLogistics.Checked

    End Sub


    Private Sub grpDiscordGroupFlight_EnabledChanged(sender As Object, e As EventArgs) Handles grpDiscordGroupFlight.EnabledChanged
        btnTaskFeaturedOnGroupFlight.Enabled = grpDiscordGroupFlight.Enabled
        pnlFullWorkflowTaskGroupFlight.Enabled = grpDiscordGroupFlight.Enabled

    End Sub

    Private Sub btnDGPOResetToDefault_Click(sender As Object, e As EventArgs) Handles btnDGPOResetToDefault.Click

        chkDGPOCoverImage.Checked = True
        chkDGPOMainGroupPost.Checked = True
        chkDGPOThreadCreation.Checked = True
        chkDGPOTeaser.Checked = False
        chkDGPOMapImage.Checked = True

        chkDGPOMainPost.Checked = True
        chkDGPOAltRestrictions.Checked = True
        chkDGPOFullDescription.Checked = True
        chkDGPOEventLogistics.Checked = True
        chkDGPOPublishWSGEventNews.Checked = True

        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Sub btnDiscordGroupEventURL_Click(sender As Object, e As EventArgs) Handles btnDiscordGroupEventURL.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtGroupEventPostURL.Text = Clipboard.GetText
        End If
    End Sub

    Private Sub btnDiscordGroupEventURLClear_Click(sender As Object, e As EventArgs) Handles btnDiscordGroupEventURLClear.Click
        txtGroupEventPostURL.Text = String.Empty
    End Sub

    Private Sub btnGroupEventURLGo_Click(sender As Object, e As EventArgs) Handles btnGroupEventURLGo.Click
        If txtGroupEventPostURL.Text.Trim.Length > 0 Then
            SupportingFeatures.LaunchDiscordURL(txtGroupEventPostURL.Text)
            SupportingFeatures.BringDiscordToTop()
        End If
    End Sub

    Private Sub btnRepostOriginalURLPaste_Click(sender As Object, e As EventArgs) Handles btnRepostOriginalURLPaste.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtRepostOriginalURL.Text = Clipboard.GetText
        End If
    End Sub

    Private Sub btnStartGroupEventPost_Click(sender As Object, e As EventArgs) Handles btnStartGroupEventPost.Click

        GroupEventOrFullWorkflow(True)

    End Sub

    Private Sub GroupEventOrFullWorkflow(fromGroupOnly As Boolean)
        Dim autoContinue As Boolean = True

        If chkRemindUserPostOptions.Checked Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Did you make sure to select the correct post options (check boxes) for what you want to post?", "Post Options Reminder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbNo Then
                    Exit Sub
                End If
            End Using
        End If

        If txtGroupEventPostURL.Text.Trim.Length > 0 AndAlso chkDGPOMainGroupPost.Enabled AndAlso chkDGPOMainGroupPost.Checked Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "There is already a group event URL specified for this event, would you like to clear it first?", "Existing group event URL!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                    txtGroupEventPostURL.Text = String.Empty
                    SaveSession()
                End If
            End Using
        End If

        If Not ValidPostingRequirements(fromGroupOnly) Then
            Exit Sub
        End If

        'Check if event is in the past
        _pastEventConfirmed = False
        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)
        If fullMeetDateTimeLocal < Now() Then
            Dim groupEventURLMsg As String = String.Empty
            If txtGroupEventPostURL.Text.Trim.Length = 0 Then
                groupEventURLMsg = $"{Environment.NewLine}The group event URL is also empty! When creating a past event, it is recommended (but not required) to include the link to the past event.{Environment.NewLine}"
            End If
            Dim confirmPastEvent As DialogResult
            Using New Centered_MessageBox(Me)
                confirmPastEvent = MessageBox.Show(Me, $"The group event date is in the past! Do you still want to proceed?{Environment.NewLine}{groupEventURLMsg}{Environment.NewLine}If you proceed, be advised you will not get any feedback confirming any event creation.", "Group event date in the past", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
            End Using
            If confirmPastEvent = DialogResult.No Then
                Exit Sub
            End If
            _pastEventConfirmed = True
        End If

        If _useTestMode AndAlso txtGroupEventPostURL.Text.Trim.Length > 0 Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Because you are testing, I need to know if you want to reset from the start? Did you delete the previous GROUP EVENT Discord post?", "Test mode reset question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    txtGroupEventPostURL.Text = String.Empty
                    SaveSession()
                End If
            End Using
        End If

        If Not _pastEventConfirmed Then
            If Not FirstPartOfGroupPost(autoContinue) Then Exit Sub
        End If

        'Need to go for the task now
        If Not fromGroupOnly OrElse _TaskEntrySeqID > 0 Then
            If _TaskEntrySeqID > 0 Then
                'Since the task already exists - we can grant temporary rights to post the task in the library
                _temporaryRightToUpdateTask = True
                If txtLastUpdateDescription.TextLength = 0 Then
                    txtLastUpdateDescription.Text = "Event"
                End If
            End If
            If Not PostTaskInLibrary(autoContinue) Then
                _temporaryRightToUpdateTask = False
                Exit Sub
            End If
            _temporaryRightToUpdateTask = False
        End If

        If Not _pastEventConfirmed Then
            If (chkDGPOEventLogistics.Enabled AndAlso chkDGPOEventLogistics.Checked) OrElse
           (chkDGPOMainPost.Enabled AndAlso chkDGPOMainPost.Checked) OrElse
           (chkDGPOAltRestrictions.Enabled AndAlso chkDGPOAltRestrictions.Checked) OrElse
           (chkDGPOFullDescription.Enabled AndAlso chkDGPOFullDescription.Checked) OrElse
           (chkDGPOMapImage.Enabled AndAlso chkDGPOMapImage.Checked) Then
                If Not SecondPartOfGroupPost(autoContinue) Then Exit Sub
            End If
        End If

        If Not _useTestMode Then
            'Always publish event News on WSG
            PublishEventNews()
        End If

    End Sub

    Private Sub btnTaskAndGroupEventLinks_Click(sender As Object, e As EventArgs) Handles btnTaskAndGroupEventLinks.Click

        Dim clubName As String = String.Empty

        If Not _ClubPreset Is Nothing Then
            clubName = $" {txtClubFullName.Text} "
        End If

        Dim sb As New StringBuilder

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)

        sb.AppendLine("## :calendar: Group Flight")
        sb.AppendLine($"Links and details are up for the{clubName}group flight of {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time.")

        'WeSimGlide
        sb.AppendLine("## :wsg: WeSimGlide.org")
        If WSGEventID <> String.Empty Then
            sb.AppendLine($"Group: [{txtClubFullName.Text} - {txtEventTitle.Text}](<{WSGEventLink}>)")
        End If
        sb.AppendLine($"Task: [#{_TaskEntrySeqID.ToString.Trim} on WeSimGlide.org](<{SupportingFeatures.GetWeSimGlideTaskURL(_TaskEntrySeqID)}>)")

        'Discord
        sb.AppendLine("## :discord_logo: Discord")
        If txtGroupEventPostURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtGroupEventPostURL.Text.Trim) Then
            sb.AppendLine($"Group: [{txtClubFullName.Text} - {txtEventTitle.Text}]({txtGroupEventPostURL.Text})")
        End If
        sb.AppendLine($"Task: [{txtTitle.Text}]({GetDiscordLinkToTaskThread()})")

        Dim msg As String = sb.ToString
        Clipboard.SetText(msg)
        CopyContent.ShowContent(Me,
                                msg,
                                "You can now paste the content of the message to share links to the task and the group event.",
                                "Sharing Discord Task and Group Event links",
                                "Anywhere you want to share the links",
                                New List(Of String) From {"^v"})

    End Sub

    Private Sub btnTaskFeaturedOnGroupFlight_Click(sender As Object, e As EventArgs) Handles btnTaskFeaturedOnGroupFlight.Click

        If _ClubPreset Is Nothing Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "No club selected For the event!", "Discord Post Helper tool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using
            Exit Sub
        End If

        Dim msg As String = TaskFeatureOnGroupFlight()
        Clipboard.SetText(msg)
        CopyContent.ShowContent(Me,
                                msg,
                                "On the task's thread, paste the content of the message to share the event for this task.",
                                "Sharing Discord Event to Task",
                                "Anywhere you want to share the Featured message",
                                New List(Of String) From {"^v"})

    End Sub

    Private Sub chkDGPOTeaser_CheckedChanged(sender As Object, e As EventArgs) Handles chkDGPOTeaser.CheckedChanged

        If chkDGPOTeaser.Checked Then
            chkDGPOMainPost.Checked = False
            chkDGPOAltRestrictions.Checked = False
            chkDGPOFullDescription.Checked = False
            chkDGPOMapImage.Checked = False
            chkDGPOEventLogistics.Checked = False
        End If

        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Sub chkDGPOAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkDGPOEventLogistics.CheckedChanged,
                                                                                    chkDGPOFullDescription.CheckedChanged,
                                                                                    chkDGPOMainPost.CheckedChanged, chkDGPOAltRestrictions.CheckedChanged

        If _isInitiatizing Then
            Exit Sub
        End If

        If CType(sender, Windows.Forms.CheckBox).Checked Then
            chkDGPOTeaser.Checked = False
        End If

        CheckWhichOptionsCanBeEnabled()

    End Sub

#End Region

#Region "Discord - Group Event Subs & Functions"

    Private Function FirstPartOfGroupPost(autoContinue As Boolean) As Boolean

        'Cover
        If chkDGPOCoverImage.Enabled AndAlso chkDGPOCoverImage.Checked Then
            autoContinue = CoverImage(If(_useTestMode, SupportingFeatures.TestEventsDiscordURL, _ClubPreset.DiscordURL), True)
        End If
        If Not autoContinue Then Return False

        'Group main post
        If chkDGPOMainGroupPost.Enabled AndAlso chkDGPOMainGroupPost.Checked Then
            autoContinue = GroupFlightEventInfoToClipboard(chkDGPOCoverImage.Enabled AndAlso chkDGPOCoverImage.Checked)
        End If
        If Not autoContinue Then Return False

        'Teaser
        If chkDGPOTeaser.Enabled AndAlso chkDGPOTeaser.Checked Then
            If chkDGPOThreadCreation.Enabled AndAlso chkDGPOThreadCreation.Checked Then
                autoContinue = CreateGroupEventThread()
                If Not autoContinue Then Return False
            End If
            autoContinue = GroupFlightEventTeaser()
            Return autoContinue
        End If

        Return True

    End Function

    Private Function SecondPartOfGroupPost(autoContinue As Boolean) As Boolean

        'Must check if task is created on WSG before continuing
        If Not _useTestMode Then
            If Not (_TaskEntrySeqID > 0 AndAlso _TaskStatus = SupportingFeatures.WSGTaskStatus.Active) Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "The task must be published on WeSimGlide.org before continuing group event posting.", "Creating group flight post", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
                Return True
            End If
        End If

        'Thread
        If chkDGPOThreadCreation.Enabled AndAlso chkDGPOThreadCreation.Checked Then
            autoContinue = CreateGroupEventThread()
            If Not autoContinue Then Return False
        End If

        'Second part of Group Flight Event - task part one
        autoContinue = PostTaskInGroupEvent(autoContinue)
        If Not autoContinue Then Return False

        Return True

    End Function

    Private Function GroupFlightEventInfoToClipboard(Optional withCover As Boolean = False) As Boolean

        Dim autoContinue As Boolean

        BuildGroupFlightPost()
        Clipboard.SetText(txtGroupFlightEventPost.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtGroupFlightEventPost.Text,
                                $"In the Discord app and on the proper channel for the club/group (use 'Take me there!'), make sure you are on the new message field to post the group flight event.{Environment.NewLine}Next, you will also be asked to copy the link to that newly created message.",
                                AppendIsTestMode("Creating group flight post"),
                                AppendIsTestMode("Group Event Main Post"),
                                New List(Of String) From {"^v"},
                                True,
                                If(withCover, numWaitSecondsForFiles.Value / 2 * 1000, 0),,
                                If(_useTestMode, SupportingFeatures.TestEventsDiscordURL, _ClubPreset.DiscordURL))

        If Not autoContinue Then Return autoContinue

        If txtGroupEventPostURL.Text = String.Empty Then
            Dim message As String = "Please Get the link to the group event's post in Discord (""...More menu"" and ""Copy Message Link"")"
            Dim waitingForm As New WaitingForURLForm(message, False)
            Dim answer As DialogResult = waitingForm.ShowDialog()

            SupportingFeatures.BringDPHToolToTop(Me.Handle)

            'Check if the clipboard contains a valid URL, which would mean the group event's URL has been copied
            If answer = DialogResult.OK Then
                Dim groupEventPostURL As String
                groupEventPostURL = Clipboard.GetText
                txtGroupEventPostURL.Text = SupportingFeatures.DiscordURL(groupEventPostURL, False)
                SaveSession()
            End If
            If txtGroupEventPostURL.Text = String.Empty Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Take a minute to copy the Discord link to the group flight event you've just created and paste it in the URL field on the Event's tab.", "Creating group flight post", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
                autoContinue = False
            End If
        End If

        Return autoContinue

    End Function

    Private Function CreateGroupEventThread() As Boolean

        Dim autoContinue As Boolean = True

        Dim fpTitle As New StringBuilder
        If txtEventTitle.Text <> String.Empty Then
            fpTitle.AppendLine(txtEventTitle.Text & AddFlagsToTitle())

            Clipboard.SetText(fpTitle.ToString)

            autoContinue = MsgBoxWithPicture.ShowContent(Me,
                                                     "CreateTaskThread.gif",
                                                     $"Group Event Thread {If(_useTestMode, "(Test Mode)", "(Your Discord)")}",
                                                     "ONLY once you've created the thread, pasted its name in THREAD NAME and positionned your cursor on the thread's message field, can you click OK and resume the workflow.",
                                                     AppendIsTestMode("Instructions for the creation of the group event's thread!"),
                                                     txtGroupEventPostURL.Text.Trim)
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "There is no event title so the thread cannot be created!", "Creating group event thread", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            autoContinue = False
        End If

        Return autoContinue

    End Function

    Private Function GroupFlightEventTeaser() As Boolean

        Dim autoContinue As Boolean = True
        Dim language As SupportedLanguage = GetGroupEventLanguage()
        Dim localizer As LocalizationManager = LocalizationManager.Instance

        Dim imagePasted As Boolean = False
        If txtEventTeaserAreaMapImage.Text <> String.Empty AndAlso File.Exists(txtEventTeaserAreaMapImage.Text) Then
            Dim allFiles As New StringCollection
            allFiles.Add(txtEventTeaserAreaMapImage.Text)
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    txtEventTeaserAreaMapImage.Text,
                                    $"Position the cursor on the message field in the group event thread and paste the copied teaser image for your first message.{Environment.NewLine}Skip (Ok) if already done.",
                                    AppendIsTestMode("Pasting teaser area map image"),
                                    AppendIsTestMode("Group Event Thread"),
                                    New List(Of String) From {"^v"},
                                    txtEventTeaserMessage.Text.Trim.Length = 0,
                                    If(txtEventTeaserMessage.Text.Trim.Length = 0, numWaitSecondsForFiles.Value / 2 * 1000, 0),
                                    Drawing.Image.FromFile(allFiles(0)), SupportingFeatures.GetTaskThreadURLFromGroupURL(txtGroupEventPostURL.Text.Trim))
            imagePasted = True
        End If

        If Not autoContinue Then
            Return False
        End If

        'Teaser message
        If txtEventTeaserMessage.Text.Trim <> String.Empty Then
            Dim teaser As New StringBuilder
            teaser.AppendLine(localizer.Format("group.event.teaser_header"))
            teaser.AppendLine(txtEventTeaserMessage.Text.Trim)
            Clipboard.SetText(teaser.ToString)
            autoContinue = CopyContent.ShowContent(Me,
                            teaser.ToString,
                            $"Make sure you are back on the thread's message field.{Environment.NewLine}Then post the teaser message as the first message in the event's thread.",
                            AppendIsTestMode("Posting teaser with text."),
                            AppendIsTestMode("Group Event Thread"),
                            New List(Of String) From {"^v"},
                            True,
                            If(imagePasted, numWaitSecondsForFiles.Value / 2 * 1000, 0),, SupportingFeatures.GetTaskThreadURLFromGroupURL(txtGroupEventPostURL.Text.Trim))
        End If

        Return autoContinue

    End Function

    Private Function GroupFlightEventThreadLogistics() As String

        Dim logisticInstructions As New StringBuilder
        Dim language As SupportedLanguage = GetGroupEventLanguage()
        Dim localizer As LocalizationManager = LocalizationManager.Instance

        logisticInstructions.AppendLine(localizer.Format("group.event.logistics_header"))
        logisticInstructions.AppendLine(localizer.Format("group.event.logistics_wsg", txtEventTitle.Text.Trim, WSGEventLink))
        logisticInstructions.AppendLine(localizer.Format("group.event.logistics_voice", cboVoiceChannel.Text))
        logisticInstructions.AppendLine(localizer.Format("group.event.logistics_server", cboMSFSServer.Text))

        Dim simDateTimeText As String = FormatSimDateTime(language, True)
        Dim simExtraInfo As String = SupportingFeatures.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True)
        logisticInstructions.AppendLine(localizer.Format("group.event.logistics_sim_time", simDateTimeText, simExtraInfo))

        If chkUseSyncFly.Checked Then
            logisticInstructions.AppendLine(localizer.Format("group.event.logistics_sync_warning"))
        End If

        logisticInstructions.AppendLine()

        If _ClubPreset IsNot Nothing AndAlso (_ClubPreset.EventNewsID = "SSCSA" OrElse _ClubPreset.EventNewsID = "SSCWE") Then
            logisticInstructions.AppendLine(localizer.Format("group.event.logistics_reminder_ssc"))
        Else
            logisticInstructions.AppendLine(localizer.Format("group.event.logistics_reminder_default"))
        End If

        Return logisticInstructions.ToString()

    End Function

    Private Function GetGroupEventLanguage() As SupportedLanguage
        'Get the language from the club preset
        If _ClubPreset IsNot Nothing Then
            Return _ClubPreset.Language
        Else
            Return SupportedLanguage.English
        End If

    End Function

    Private Function FormatSimDateTime(language As SupportedLanguage, includeLocalLabel As Boolean) As String
        Dim datePortion As String
        Dim timePortion As String

        If language = SupportedLanguage.French Then
            Dim dateFormat As String = If(chkIncludeYear.Checked, "d MMMM yyyy", "d MMMM")
            datePortion = dtSimDate.Value.ToString(dateFormat, _FrenchCulture)
            datePortion = _FrenchCulture.TextInfo.ToLower(datePortion)
            timePortion = dtSimLocalTime.Value.ToString("HH 'h' mm", _FrenchCulture)
        Else
            Dim dateFormat As String = If(chkIncludeYear.Checked, "MMMM dd, yyyy", "MMMM dd")
            datePortion = dtSimDate.Value.ToString(dateFormat, _EnglishCulture)
            timePortion = dtSimLocalTime.Value.ToString("h:mm tt", _EnglishCulture)
        End If

        Dim formatted As String = $"{datePortion}, {timePortion}"

        If includeLocalLabel Then
            If language = SupportedLanguage.French Then
                formatted &= " heure locale"
            Else
                formatted &= " local"
            End If
        End If

        Return formatted
    End Function

    Private Function FormatMsfsLocalDate(language As SupportedLanguage) As String
        Return FormatSimDateTime(language, False)
    End Function

    Private Sub BuildGroupFlightPost()

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)
        Dim fullSyncFlyDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventSyncFlyDate, dtEventSyncFlyTime, chkDateTimeUTC.Checked)
        Dim fullLaunchDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventLaunchDate, dtEventLaunchTime, chkDateTimeUTC.Checked)
        Dim fullStartTaskDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventStartTaskDate, dtEventStartTaskTime, chkDateTimeUTC.Checked)

        Dim fullMeetDateTimeMSFS As DateTime
        Dim fullSyncFlyDateTimeMSFS As DateTime
        Dim fullLaunchDateTimeMSFS As DateTime
        Dim fullStartTaskDateTimeMSFS As DateTime
        _SF.ExpressEventTimesInMSFSTime(fullMeetDateTimeLocal,
                                        fullSyncFlyDateTimeLocal,
                                        fullLaunchDateTimeLocal,
                                        fullStartTaskDateTimeLocal,
                                        dtSimDate.Value.Date.Add(New TimeSpan(dtSimLocalTime.Value.Hour, dtSimLocalTime.Value.Minute, 0)),
                                        chkUseSyncFly.Checked,
                                        chkUseLaunch.Checked,
                                        fullMeetDateTimeMSFS,
                                        fullSyncFlyDateTimeMSFS,
                                        fullLaunchDateTimeMSFS,
                                        fullStartTaskDateTimeMSFS)

        Dim sb As New StringBuilder
        Dim language As SupportedLanguage = GetGroupEventLanguage()
        Dim localizer As LocalizationManager = LocalizationManager.Instance

        txtGroupFlightEventPost.Text = String.Empty

        If txtEventTitle.Text = String.Empty Then
            txtEventTitle.Text = TaskTitleMinusClubName()
        End If
        If txtEventTitle.Text <> String.Empty Then
            sb.AppendLine($"# {txtEventTitle.Text & AddFlagsToTitle()}")
        End If

        If txtCredits.Text <> String.Empty Then
            sb.AppendLine(GetLocalizedCredits())
            sb.AppendLine()
        End If

        Dim meetFullTimestamp As String = _SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)
        sb.AppendLine(localizer.Format("group.event.local_time_banner", meetFullTimestamp))
        sb.AppendLine()

        sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))

        Dim meetTimestamp As String = _SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)
        Dim meetMessageTemplate As String = If(_ClubPreset IsNot Nothing, _ClubPreset.MeetMessage, String.Empty)
        If String.IsNullOrWhiteSpace(meetMessageTemplate) Then
            sb.AppendLine(localizer.Format("group.event.meet_briefing", meetTimestamp, Environment.NewLine))
            sb.AppendLine()
        Else
            sb.AppendLine(localizer.Format("group.event.meet_label", meetTimestamp))
            sb.AppendLine(meetMessageTemplate)
            sb.AppendLine()
        End If

        If chkUseSyncFly.Checked Then
            Dim syncTimestamp As String = _SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)
            Dim syncMessageTemplate As String = If(_ClubPreset IsNot Nothing, _ClubPreset.SyncMessage, String.Empty)
            If String.IsNullOrWhiteSpace(syncMessageTemplate) Then
                sb.AppendLine(localizer.Format("group.event.sync_fly", syncTimestamp, Environment.NewLine))
            Else
                sb.AppendLine(localizer.Format("group.event.sync_label", syncTimestamp))
                sb.AppendLine(syncMessageTemplate)
            End If
            If chkUseLaunch.Checked AndAlso fullSyncFlyDateTimeLocal = fullLaunchDateTimeLocal Then
                sb.AppendLine()
                sb.AppendLine(localizer.Format("group.event.sync_fly_launch"))
            End If
            sb.AppendLine()
        Else
            Dim noSyncTimestamp As String = _SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)
            Dim noSyncMessageTemplate As String = If(_ClubPreset IsNot Nothing, _ClubPreset.NoSyncMessage, String.Empty)
            If String.IsNullOrWhiteSpace(noSyncMessageTemplate) Then
                sb.AppendLine(localizer.Format("group.event.no_sync", noSyncTimestamp, Environment.NewLine))
            Else
                sb.AppendLine(localizer.Format("group.event.no_sync_label", noSyncTimestamp))
                sb.AppendLine(noSyncMessageTemplate)
            End If
            sb.AppendLine()
        End If
        If chkUseLaunch.Checked AndAlso (fullSyncFlyDateTimeLocal <> fullLaunchDateTimeLocal OrElse Not chkUseSyncFly.Checked) Then
            Dim launchTimestamp As String = _SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)
            Dim launchMessageTemplate As String = If(_ClubPreset IsNot Nothing, _ClubPreset.LaunchMessage, String.Empty)
            If String.IsNullOrWhiteSpace(launchMessageTemplate) Then
                sb.AppendLine(localizer.Format("group.event.launch", launchTimestamp, Environment.NewLine))
            Else
                sb.AppendLine(localizer.Format("group.event.launch_label", launchTimestamp))
                sb.AppendLine(launchMessageTemplate)
            End If
            sb.AppendLine()
        End If

        If chkUseStart.Checked Then
            Dim startTimestamp As String = _SF.GetDiscordTimeStampForDate(fullStartTaskDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)
            Dim startMessageTemplate As String = If(_ClubPreset IsNot Nothing, _ClubPreset.StartMessage, String.Empty)
            If String.IsNullOrWhiteSpace(startMessageTemplate) Then
                sb.AppendLine(localizer.Format("group.event.task_start", startTimestamp, Environment.NewLine))
            Else
                sb.AppendLine(localizer.Format("group.event.task_start_label", startTimestamp))
                sb.AppendLine(startMessageTemplate)
            End If
            sb.AppendLine()
        End If

        sb.AppendLine(GetLocalizedDurationText(True))

        If txtAATTask.Text.Length > 0 Then
            sb.AppendLine($"⚠️ **{txtAATTask.Text}**")
        End If
        sb.AppendLine()

        If cboEligibleAward.SelectedIndex > 0 Then
            sb.AppendLine(localizer.Format("group.Event.badge", cboEligibleAward.Text, cboEligibleAward.Text.ToLower()))
            sb.AppendLine()
        End If

        If (Not IsDelayedAvailability) Then
            sb.AppendLine(localizer.Format("group.Event.files_shared"))
            sb.AppendLine()
        Else
            sb.AppendLine(localizer.Format("group.Event.files_shared_delayed"))
            sb.AppendLine()
        End If

        Dim urlBeginnerGuide As String = String.Empty
        Select Case cboBeginnersGuide.Text
            Case "Other (provide link below)"
                If SupportingFeatures.IsValidURL(txtOtherBeginnerLink.Text.Trim) Then
                    Dim customGuideText As String = localizer.Format("group.Event.custom_beginner_link")
                    urlBeginnerGuide = $"[{customGuideText}]({txtOtherBeginnerLink.Text.Trim})"
                End If
            Case Else
                'Retrieve the link from the club
                For Each club In _SF.DefaultKnownClubEvents.Values
                    If club.BeginnerLink = cboBeginnersGuide.Text Then
                        urlBeginnerGuide = $"[{club.BeginnerLink}]({club.BeginnerLinkURL})"
                    End If
                Next
        End Select
        If Not urlBeginnerGuide = String.Empty Then
            sb.AppendLine(localizer.Format("group.Event.beginner_guide", urlBeginnerGuide))
            sb.AppendLine()
        End If

        txtGroupFlightEventPost.Text = sb.ToString.Trim

    End Sub

#End Region

#End Region

#Region "Briefing tab"

#Region "Briefing tab Event handlers"

    Private Sub cboCoverImage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCoverImage.SelectedIndexChanged

        SessionModified(SourceOfChange.TaskTab)

    End Sub

    Private Sub cboBriefingMap_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBriefingMap.SelectedIndexChanged

        SessionModified(SourceOfChange.TaskTab)

        'Load image
        BriefingControl1.ChangeImage(cboBriefingMap.SelectedItem.ToString)

        HighlightExpectedFields()

    End Sub


#End Region

    Private Sub LoadPossibleImagesInCoverDropdown(Optional coverToSelect As String = "")

        _loadingFile = True

        ' Load up the possible images in the dropdown list
        cboCoverImage.Items.Clear()
        cboCoverImage.Items.Add("")

        For Each item As String In lstAllFiles.Items
            Dim fileExtension As String = Path.GetExtension(item).ToLower
            If fileExtension = ".png" OrElse fileExtension = ".jpg" Then
                cboCoverImage.Items.Add(item)
            End If
        Next

        'Check if coverToSelect is specified and present
        If coverToSelect <> String.Empty AndAlso cboCoverImage.Items.Contains(coverToSelect) Then
            cboCoverImage.SelectedItem = coverToSelect
        Else
            If Not chkLockCoverImage.Checked Then
                For Each item As String In cboCoverImage.Items
                    If item.ToUpper.Contains("COVER") Then
                        cboCoverImage.SelectedItem = item
                        Exit For
                    End If
                Next
            End If
        End If

        _loadingFile = False

    End Sub

    Private Sub LoadPossibleImagesInMapDropdown(Optional mapToSelect As String = "")

        _loadingFile = True

        ' Load up the possible images in the dropdown list
        cboBriefingMap.Items.Clear()
        cboBriefingMap.Items.Add("")
        Dim mapItemSelected As Boolean = False ' Track if an item with "Map" is selected

        For Each item As String In lstAllFiles.Items
            Dim fileExtension As String = Path.GetExtension(item).ToLower
            If fileExtension = ".png" OrElse fileExtension = ".jpg" Then
                cboBriefingMap.Items.Add(item)
            End If
        Next

        'Check if mapToSelect is specified and present
        If mapToSelect <> String.Empty AndAlso cboBriefingMap.Items.Contains(mapToSelect) Then
            cboBriefingMap.SelectedItem = mapToSelect
        Else
            If Not chkLockMapImage.Checked Then
                'Select the first image with "MAP" in it
                For Each item As String In cboBriefingMap.Items
                    If item.ToUpper.Contains("MAP") Then
                        cboBriefingMap.SelectedItem = item
                        mapItemSelected = True
                        Exit For
                    End If
                Next
                If Not mapItemSelected AndAlso cboBriefingMap.Items.Count > 1 Then
                    cboBriefingMap.SelectedItem = 1
                    mapItemSelected = True
                End If
            End If
        End If

        _loadingFile = False

        HighlightExpectedFields()

    End Sub

    Private Sub GenerateBriefing()

        LoadPossibleImagesInMapDropdown(cboBriefingMap.SelectedItem)

        BriefingControl1.FullReset()

        If txtFlightPlanFile.Text = String.Empty OrElse txtWeatherFile.Text = String.Empty Then
            'Can't generate briefing
            BriefingControl1.FullReset()
        Else
            BriefingControl1.GenerateBriefing(_SF, SetAndRetrieveSessionData(), txtFlightPlanFile.Text, txtWeatherFile.Text, _taskDiscordPostID)
        End If

    End Sub

#End Region

#Region "Guide/Wizard"

#Region "Event Handlers"

    Private Sub btnGuideMe_Click(sender As Object, e As EventArgs) Handles toolStripGuideMe.Click

        Dim activateGuide As Boolean = False

        Select Case mainTabControl.SelectedTab.TabIndex
            Case tabFlightPlan.TabIndex
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show(Me, "Do you want To start by resetting everything?", "Starting the Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        ResetForm()
                    End If
                End Using
                _GuideCurrentStep = 1
                activateGuide = True
            Case tabEvent.TabIndex
                _GuideCurrentStep = 60
                activateGuide = True
            Case tabDiscord.TabIndex
                _GuideCurrentStep = 40
                activateGuide = True
            Case tabBriefing.TabIndex
                _GuideCurrentStep = 100
                activateGuide = True
        End Select

        If activateGuide Then
            toolStripStopGuide.Visible = True
            ShowGuide()
        End If

    End Sub

    Private Sub btnGuideNext_Click(sender As Object, e As EventArgs) Handles btnGuideNext.Click, btnEventGuideNext.Click, btnDiscordGuideNext.Click, btnBriefingGuideNext.Click

        _GuideCurrentStep += 1
        ShowGuide()

    End Sub

    Private Sub btnTurnGuideOff_Click(sender As Object, e As EventArgs) Handles toolStripStopGuide.Click

        _GuideCurrentStep = 0
        toolStripStopGuide.Visible = False
        ShowGuide()

    End Sub

    Private Sub Main_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles mainTabControl.KeyDown, Me.KeyDown

        ' Handle F1 for help
        If e.KeyCode = Keys.F1 Then
            Try
                Dim controlTag As Integer = CInt(Me.ActiveControl.Tag)
            Catch ex As Exception
                'Do nothing
            End Try

            If _GuideCurrentStep = CInt(Me.ActiveControl.Tag) Then
                _GuideCurrentStep = 0
            Else
                _GuideCurrentStep = CInt(Me.ActiveControl.Tag)
            End If
            ShowGuide(True)
            e.SuppressKeyPress = True ' This prevents the beep sound
        End If

        ' Handle the CTRL-S key combination (e.g., save the file)
        If e.Control AndAlso e.KeyCode = Keys.S AndAlso _sessionModified Then
            SaveSession()
            e.SuppressKeyPress = True ' This prevents the beep sound
        End If

        ' Handle Ctrl+Backspace for text control
        If e.Control AndAlso e.KeyCode = Keys.Back Then
            If TypeOf Me.ActiveControl Is Windows.Forms.TextBox Then
                Dim tb As Windows.Forms.TextBox = CType(Me.ActiveControl, Windows.Forms.TextBox)
                If Not String.IsNullOrWhiteSpace(tb.Text) AndAlso tb.SelectionStart > 0 Then
                    Dim selStart As Integer = tb.SelectionStart
                    ' Skip over any spaces before the current position
                    While selStart > 0 AndAlso tb.Text.Substring(selStart - 1, 1) = " "
                        selStart -= 1
                    End While

                    Dim prevSpace As Integer = tb.Text.LastIndexOf(" "c, selStart - 1)
                    If prevSpace = -1 Then prevSpace = 0 ' If no space found, go to start of text

                    ' Calculate the start position to delete while retaining the space before the word
                    Dim startPos As Integer = prevSpace
                    If startPos > 0 AndAlso tb.Text(startPos) = " "c Then
                        startPos += 1
                    End If

                    ' Remove the text from the adjusted start position
                    tb.Text = tb.Text.Remove(startPos, tb.SelectionStart - startPos)
                    tb.SelectionStart = startPos
                End If
            ElseIf TypeOf Me.ActiveControl Is Windows.Forms.ComboBox Then
                Dim tb As Windows.Forms.ComboBox = CType(Me.ActiveControl, Windows.Forms.ComboBox)
                If Not String.IsNullOrWhiteSpace(tb.Text) AndAlso tb.SelectionStart > 0 Then
                    Dim selStart As Integer = tb.SelectionStart
                    ' Skip over any spaces before the current position
                    While selStart > 0 AndAlso tb.Text.Substring(selStart - 1, 1) = " "
                        selStart -= 1
                    End While

                    Dim prevSpace As Integer = tb.Text.LastIndexOf(" "c, selStart - 1)
                    If prevSpace = -1 Then prevSpace = 0 ' If no space found, go to start of text

                    ' Calculate the start position to delete while retaining the space before the word
                    Dim startPos As Integer = prevSpace
                    If startPos > 0 AndAlso tb.Text(startPos) = " "c Then
                        startPos += 1
                    End If

                    ' Remove the text from the adjusted start position
                    tb.Text = tb.Text.Remove(startPos, tb.SelectionStart - startPos)
                    tb.SelectionStart = startPos
                End If
            End If
            e.Handled = True  ' Prevent further processing of this key event and the ding sound
            e.SuppressKeyPress = True
        End If

    End Sub

#End Region

    Private Sub ShowGuide(Optional fromF1Key As Boolean = False)

        If _GuideCurrentStep > 0 Then
            toolStripStopGuide.Visible = True
        End If

        Select Case _GuideCurrentStep
            Case 0
                pnlGuide.Visible = False
                pnlWizardEvent.Visible = False
                pnlWizardDiscord.Visible = False
                pnlWizardBriefing.Visible = False
                toolStripStopGuide.Visible = False
                grpGroupEventPost.Width = tabEvent.Width - 15
                FixForDropDownCombos()

            Case 1 'Select flight plan
                mainTabControl.SelectedTab = mainTabControl.TabPages("tabFlightPlan")
                SetGuidePanelToLeft()
                pnlGuide.Top = -3
                lblGuideInstructions.Text = "Click the ""Flight Plan"" button and Select the flight plan to use with this task."
                SetFocusOnField(btnSelectFlightPlan, fromF1Key)
            Case 2 'Title
                SetGuidePanelToLeft()
                pnlGuide.Top = 54
                lblGuideInstructions.Text = "This is the title that was read from the flight plan. Is it ok? If not, change it and use the checkbox to prevent it from being overwritten."
                SetFocusOnField(txtTitle, fromF1Key)
            Case 3 'Select weather file
                SetGuidePanelToLeft()
                pnlGuide.Top = 95
                lblGuideInstructions.Text = "Click the ""Weather Preset"" button and select the weather presets to use with this task."
                SetFocusOnField(btnSelectWeatherFile, fromF1Key)
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
            Case 10 'AAT
                SetGuidePanelToLeft()
                pnlGuide.Top = 371
                lblGuideInstructions.Text = "This field only displays information if the task is an AAT. Nothing to do but to validate the info."
                SetFocusOnField(txtAATTask, fromF1Key)
            Case 11 'Durations
                SetGuidePanelToLeft()
                pnlGuide.Top = 403
                lblGuideInstructions.Text = "You can enter the expected min and max duration of the task as you see fit, and also enter extra information."
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
            Case 17 'Countries
                SetGuidePanelToRight()
                pnlGuide.Top = 28
                lblGuideInstructions.Text = "Countries to show in the topic can be detected from the waypoints, but you can optionally specify them yourself here."
                SetFocusOnField(cboCountryFlag, fromF1Key)
            Case 18 'Weather summary
                SetGuidePanelToRight()
                pnlGuide.Top = 102
                lblGuideInstructions.Text = "Optional weather summary to be added along with the weather basic information."
                SetFocusOnField(txtWeatherSummary, fromF1Key)
            Case 19 'Non standard Barometric Pressure
                SetGuidePanelToRight()
                pnlGuide.Top = 139
                lblGuideInstructions.Text = "If barometric pressure is non-standard, allow you to suppress the warning symbol and set any text you want to display with the barometric pressure."
                SetFocusOnField(chkSuppressWarningForBaroPressure, fromF1Key)
            Case 20 'Recommended add-ons
                SetGuidePanelToRight()
                pnlGuide.Top = 206
                lblGuideInstructions.Text = "You can optionally recommend add-ons that go well with this task. Use this section to add, edit and remove recommended add-ons."
                SetFocusOnField(btnAddRecAddOn, fromF1Key)
            Case 21 'Extra files
                SetGuidePanelToRight()
                pnlGuide.Top = 372
                lblGuideInstructions.Text = "Optionally, use this section to add and remove any extra files you want included with the task post. Maps, XCSoar track files, other images, etc."
                SetFocusOnField(btnAddExtraFile, fromF1Key)
            Case 22 'Map Image
                SetGuidePanelToRight()
                pnlGuide.Top = 497
                lblGuideInstructions.Text = "Select the image that will be used as map on the briefing tab. Any image you add named ""Map"" will be automatically selected."
                SetFocusOnField(cboBriefingMap, fromF1Key)
            Case 23 'Cover image
                SetGuidePanelToRight()
                pnlGuide.Top = 531
                lblGuideInstructions.Text = "You can specify an image that will be used as cover for the flight on Discord. Any image you add named ""Cover"" will be automatically selected."
                SetFocusOnField(txtTaskID, fromF1Key)
            Case 24 'External source of task
                SetGuidePanelToRight()
                pnlGuide.Top = 597
                lblGuideInstructions.Text = "If you'd like to specify where and when this task has been published first, enable this to set the original date the task was published and its URL."
                SetFocusOnField(chkRepost, fromF1Key)
            Case 25 'File drop zone
                SetGuidePanelToRight()
                pnlGuide.Top = 699
                lblGuideInstructions.Text = "This is the file drop zone. You can drag and drop any supported files over this area."
                SetFocusOnField(FileDropZone1, fromF1Key)

            Case 26 To 29 'End of flight plan data
                _GuideCurrentStep = 34
                ShowGuide()

            Case 34 'Briefing review
                mainTabControl.SelectedIndex = 3
                SetBriefingGuidePanel()
                lblBriefingGuideInstructions.Text = "Review the task information on the various briefing tabs here and when you are satisfied, click Next."
                SetFocusOnField(BriefingControl1, fromF1Key)
            Case 35 To 39 'We're done with the briefing
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 40 'WSG Task Create/Update
                mainTabControl.SelectedTab = mainTabControl.TabPages("tabDiscord")
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 0
                lblDiscordGuideInstructions.Text = "Any task creation/update includes updating the task on WeSimGlide.org. This option cannot be unchecked."
                SetFocusOnField(chkWSGTask, fromF1Key)
            Case 41 'Discord Post Options for task
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 58
                lblDiscordGuideInstructions.Text = "These are all the options you can toggle to include the various elements of the post, as you see fit."
                SetFocusOnField(chkDPOIncludeCoverImage, fromF1Key)
            Case 42 'Publisher
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 103
                lblDiscordGuideInstructions.Text = "This is the primary task publisher. Only the publishers can update task information on WeSimGlide.org."
                SetFocusOnField(cboTaskOwner, fromF1Key)
            Case 43 'Shared users
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 142
                lblDiscordGuideInstructions.Text = "The primary publisher can select other publishers to share the edit rights of the task information."
                SetFocusOnField(chkcboSharedWithUsers, fromF1Key)
            Case 44 'Update description
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 205
                lblDiscordGuideInstructions.Text = "All updates to the task on WeSimGlide.org require a brief update description."
                SetFocusOnField(txtLastUpdateDescription, fromF1Key)
            Case 45 'Reset all options to default
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 244
                lblDiscordGuideInstructions.Text = "Use these buttons to recall, save or reset all the options above to your remembered settings or their default values."
                SetFocusOnField(btnDPOResetToDefault, fromF1Key)
            Case 46 'Start Task Posting Workflow
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 284
                lblDiscordGuideInstructions.Text = "Click this button to start the workflow to post your task on Discord using the selected options above."
                SetFocusOnField(btnStartTaskPost, fromF1Key)
            Case 47 'IDs
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 394
                lblDiscordGuideInstructions.Text = "This section shows a status of all proper IDs associated with the task."
                SetFocusOnField(btnDiscordTaskPostIDGo, fromF1Key)
            Case 48 'Delayed Availability
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 475
                lblDiscordGuideInstructions.Text = "Enable this to set a date and time when the task will become available."
                SetFocusOnField(chkDelayedAvailability, fromF1Key)
            Case 49 'Based on group event
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 502
                lblDiscordGuideInstructions.Text = "You can also set the date and time based on the group event meeting time."
                SetFocusOnField(chkDelayBasedOnEvent, fromF1Key)
            Case 50 'Date and time for availability
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 537
                lblDiscordGuideInstructions.Text = "Set the date and time when the task will become available."
                SetFocusOnField(dtAvailabilityDate, fromF1Key)
            Case 51 'Refly
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 569
                lblDiscordGuideInstructions.Text = "Enable this to hide any (existing) task hint information when this is a refly."
                SetFocusOnField(chkAvailabilityRefly, fromF1Key)

            Case 52 To 59 'Next section
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 60 'Event
                'Resume wizard on the Event tab
                mainTabControl.SelectedTab = mainTabControl.TabPages("tabEvent")
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 39
                lblEventGuideInstructions.Text = "Start by selecting the soaring club or known group for which you want to create a new event, if this applies to you."
                SetFocusOnField(cboGroupOrClubName, fromF1Key)

            Case 61 'Group flight URL
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 112
                lblEventGuideInstructions.Text = "This should contain the link to the Discord group flight post. It is auto-populated when using the group event post workflow."
                SetFocusOnField(btnDiscordGroupEventURL, fromF1Key)

            Case 62 'Group flight title / topic
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 156
                lblEventGuideInstructions.Text = "If you would like to specify a different title for the group flight, you can do so now. Otherwise, this is the same as the task's title."
                SetFocusOnField(txtEventTitle, fromF1Key)

            Case 63 'MSFS Server
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 189
                lblEventGuideInstructions.Text = "Specify the MSFS Server to use during the group flight. If you specified a known club, then you should not change this, unless it is incorrect."
                SetFocusOnField(cboMSFSServer, fromF1Key)

            Case 64 'Voice channel
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 223
                lblEventGuideInstructions.Text = "Specify the Discord Voice channel to use during the group flight. If you specified a known club, then you should not change this, unless it is incorrect."
                SetFocusOnField(cboVoiceChannel, fromF1Key)

            Case 65 'UTC Zulu
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 259
                lblEventGuideInstructions.Text = "For the sake of simplicity, leave this checkbox ticked to use UTC (Zulu) entries. Local times are still displayed to the right."
                SetFocusOnField(chkDateTimeUTC, fromF1Key)

            Case 66 'Meet time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 292
                lblEventGuideInstructions.Text = "Specify the meet date and time. This is the time when people will start gathering for the group flight and briefing."
                SetFocusOnField(dtEventMeetDate, fromF1Key)

            Case 67 'Sync
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 323
                lblEventGuideInstructions.Text = "Only if the flight's conditions require some sort of sync, then tick the ""Yes"" checkbox and specify when it will happen."
                SetFocusOnField(chkUseSyncFly, fromF1Key)

            Case 68 'Launch Time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 362
                lblEventGuideInstructions.Text = "If you want to specify the time when people should start to launch from the airfield, tick the ""Yes"" checkbox and specify when it should happen."
                SetFocusOnField(chkUseLaunch, fromF1Key)

            Case 69 'Start Task Time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 396
                lblEventGuideInstructions.Text = "If you want to specify a time for the start of the task (going through the start gate), tick the ""Yes"" checkbox and specify when it should happen."
                SetFocusOnField(chkUseStart, fromF1Key)

            Case 70 'Group flight description
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 490
                lblEventGuideInstructions.Text = "If you would like to specify a description for the group flight, you can do so now."
                SetFocusOnField(txtEventDescription, fromF1Key)

            Case 71 'SSC Award
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 584
                lblEventGuideInstructions.Text = "This is usually set automatically if the club is SSC Saturday and depending on the task's distance. You should leave it alone, unless it's incorrect."
                SetFocusOnField(cboEligibleAward, fromF1Key)

            Case 72 'Beginner's link
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 622
                lblEventGuideInstructions.Text = "You can select from different beginner's guide (or specify a link to a custom one) to include with the group event post."
                SetFocusOnField(cboBeginnersGuide, fromF1Key)

            Case 73 'Teaser section
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 720
                lblEventGuideInstructions.Text = "You can opt to first post a teaser only for your group event. Check this box, select a teaser image and message."
                SetFocusOnField(chkEventTeaser, fromF1Key)

            Case 74 'Briefing review
                mainTabControl.SelectedIndex = 3
                SetBriefingGuidePanel()
                lblBriefingGuideInstructions.Text = "Review the task and event information on the briefing tabs here and when you are satisfied, click Next."
                SetFocusOnField(BriefingControl1, fromF1Key)

            Case 75 To 78 'Next section
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 79 'WSG Create/Update Event
                mainTabControl.SelectedTab = mainTabControl.TabPages("tabDiscord")
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 486
                pnlWizardDiscord.Top = 55
                lblDiscordGuideInstructions.Text = "Any group event creation/update includes updating the event (and possibly task) on WeSimGlide.org. This option cannot be unchecked."
                SetFocusOnField(chkWSGEvent, fromF1Key)

            Case 80 'Discord Post Options
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 486
                pnlWizardDiscord.Top = 371
                lblDiscordGuideInstructions.Text = "These are all the options you can toggle to include the various elements of the post, as you see fit."
                SetFocusOnField(chkDGPOCoverImage, fromF1Key)

            Case 81 'Reset all options
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 585
                pnlWizardDiscord.Top = 412
                lblDiscordGuideInstructions.Text = "Use these buttons to recall, save or reset all the options above to your remembered settings or their default values."
                SetFocusOnField(btnDGPOResetToDefault, fromF1Key)

            Case 82 'Start workflow
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 585
                pnlWizardDiscord.Top = 456
                lblDiscordGuideInstructions.Text = "Click this button to start the workflow to post your group event on Discord using the selected options."
                SetFocusOnField(btnStartGroupEventPost, fromF1Key)

            Case 83 'Delete task from WSG
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 68
                lblDiscordGuideInstructions.Text = "Click this button to delete the task from WeSimGlide.org."
                SetFocusOnField(btnDeleteFromTaskBrowser, fromF1Key)

            Case 84 'Delete event
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 110
                lblDiscordGuideInstructions.Text = "Click this button to remove the group event from WeSimGlide.org."
                SetFocusOnField(btnDeleteEventNews, fromF1Key)

            Case 85 'Task featured on group flight - share
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 184
                lblDiscordGuideInstructions.Text = "Click this button to copy the message to share the group event for the task."
                SetFocusOnField(btnTaskFeaturedOnGroupFlight, fromF1Key)

            Case 86 'Task and Group Event Links
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 224
                lblDiscordGuideInstructions.Text = "Click this button to copy and paste a simple message with both links to task and group event."
                SetFocusOnField(btnTaskAndGroupEventLinks, fromF1Key)

            Case 87 'WSG link to task
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 266
                lblDiscordGuideInstructions.Text = "Click this button to copy and paste a simple message with the link to the task on WeSimGlide.org."
                SetFocusOnField(btnWSGTaskLink, fromF1Key)

            Case 88 'Seconds to wait for files
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 336
                lblDiscordGuideInstructions.Text = "Specify the pause duration when posting files to wait for Discord to complete the upload and continue the workflow."
                SetFocusOnField(numWaitSecondsForFiles, fromF1Key)

            Case 89 'Start workflow full workflow
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 422
                lblDiscordGuideInstructions.Text = "Click this button to start the workflow to post both the task and group flight event full details."
                SetFocusOnField(btnStartFullPostingWorkflow, fromF1Key)

            Case 90 To 99
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 100
                SetBriefingGuidePanel()
                btnBriefingGuideNext.Visible = False
                lblBriefingGuideInstructions.Text = "The briefing offers all of the task and event information in a friendly format and using your preferred units."
                SetFocusOnField(BriefingControl1, fromF1Key)

            Case Else
                _GuideCurrentStep = 0
                pnlGuide.Visible = False
                pnlWizardEvent.Visible = False
                pnlWizardDiscord.Visible = False
                pnlWizardBriefing.Visible = False
                toolStripStopGuide.Visible = False
                grpGroupEventPost.Width = tabEvent.Width - 15
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "The wizard's guidance ends here! You can resume anytime by hitting F1 on any field. Also, if you hover your mouse on any field or button, you will also get a tooltip help displayed!", "Discord Post Helper Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
        End Select
    End Sub

    Private Function AskWhereToGoNext() As Integer
        Dim nextWizard As New WizardNextChoice
        nextWizard.ShowDialog(Me)
        Select Case nextWizard.UserChoice
            Case WizardNextChoice.WhereToGoNext.StopWizard
                Return 999
            Case WizardNextChoice.WhereToGoNext.CreateTask
                Return 1
            Case WizardNextChoice.WhereToGoNext.CreateEvent
                If Not chkActivateEvent.Checked Then
                    chkActivateEvent.Checked = True
                End If
                Return 60
            Case WizardNextChoice.WhereToGoNext.DiscordTask
                Return 40
            Case WizardNextChoice.WhereToGoNext.DiscordEvent
                Return 79
            Case Else
                Return 999
        End Select
    End Function

    Private Sub SetFocusOnField(controlToPutFocus As Windows.Forms.Control, fromF1Key As Boolean)

        If Not fromF1Key Then
            controlToPutFocus.Focus()
        End If

    End Sub

    Private Sub SetGuidePanelToLeft()
        pnlWizardEvent.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.left_arrow
        pnlGuide.Left = 718
        pnlGuide.Visible = True
        pnlArrow.Left = -6
        pnlArrow.Top = 0
        btnGuideNext.Left = 674
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToTopArrowLeftSide()
        pnlWizardEvent.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.up_arrow
        pnlGuide.Top = 0
        pnlGuide.Visible = True
        pnlArrow.Left = -6
        pnlArrow.Top = 0
        btnGuideNext.Left = 674
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToTopArrowRightSide()
        pnlWizardEvent.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.up_arrow
        pnlGuide.Top = 0
        pnlGuide.Visible = True
        pnlArrow.Left = 667
        pnlArrow.Top = 0
        btnGuideNext.Left = 3
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToRight()
        pnlWizardEvent.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        pnlGuide.Left = 0
        pnlGuide.Visible = True
        pnlArrow.Left = 667
        pnlArrow.Top = 0
        btnGuideNext.Left = 3
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetEventGuidePanelToLeft()
        pnlGuide.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlEventArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.left_arrow
        If tabEvent.Width - grpGroupEventPost.Width < pnlWizardEvent.Width Then
            'We need to resize to make room
            grpGroupEventPost.Width = tabEvent.Width - pnlWizardEvent.Width - 20
        End If
        pnlWizardEvent.Left = tabEvent.Width - pnlWizardEvent.Width
        pnlWizardEvent.Visible = True
        pnlEventArrow.Left = -6
        pnlEventArrow.Top = 0
        btnEventGuideNext.Left = 552
        btnEventGuideNext.Top = 3
        FixForDropDownCombos()
    End Sub

    Private Sub SetEventGuidePanelToRight()
        pnlGuide.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlEventArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        pnlWizardEvent.Left = 849
        pnlWizardEvent.Visible = True
        pnlEventArrow.Left = 545
        pnlEventArrow.Top = 0
        btnEventGuideNext.Left = 3
        btnEventGuideNext.Top = 3
    End Sub

    Private Sub SetDiscordGuidePanelToLeft()
        pnlGuide.Visible = False
        pnlWizardEvent.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlDiscordArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.left_arrow
        pnlWizardDiscord.Left = 405
        pnlWizardDiscord.Visible = True
        pnlDiscordArrow.Left = -6
        pnlDiscordArrow.Top = 0
        btnDiscordGuideNext.Left = 674
        btnDiscordGuideNext.Top = 3
    End Sub

    Private Sub SetDiscordGuidePanelToRight()
        pnlGuide.Visible = False
        pnlWizardEvent.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlDiscordArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        pnlWizardDiscord.Left = 90
        pnlWizardDiscord.Visible = True
        pnlDiscordArrow.Left = 667
        pnlDiscordArrow.Top = 0
        btnDiscordGuideNext.Left = 3
        btnDiscordGuideNext.Top = 3
    End Sub

    Private Sub SetDiscordGuidePanelToTopArrowLeftSide()
        pnlGuide.Visible = False
        pnlWizardEvent.Visible = False
        pnlWizardBriefing.Visible = False
        Me.pnlDiscordArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.up_arrow
        pnlWizardDiscord.Left = 709
        pnlWizardDiscord.Visible = True
        pnlDiscordArrow.Left = -6
        pnlDiscordArrow.Top = 0
        btnDiscordGuideNext.Left = 674
        btnDiscordGuideNext.Top = 3

    End Sub


    Private Sub SetBriefingGuidePanel()
        pnlGuide.Visible = False
        pnlWizardEvent.Visible = False
        pnlWizardDiscord.Visible = False
        pnlWizardBriefing.Visible = True

    End Sub

    Private Function GetLocalizedDurationText(Optional fromGroup As Boolean = False) As String
        Dim localizer As LocalizationManager = LocalizationManager.Instance
        Dim result As String = String.Empty
        Dim source As String = String.Empty

        If fromGroup Then
            source = "group.event.duration"
        Else
            source = "task.duration"
        End If
        Dim durationExtra As String = SupportingFeatures.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)
        If localizer.Language = SupportedLanguage.English Then
            result = localizer.Format("group.event.duration", SupportingFeatures.GetDuration(txtDurationMin.Text, txtDurationMax.Text), durationExtra)
        Else
            Dim notSpecifiedTxt As String = localizer.Format("duration.not_specified")
            Dim arroundTxt As String = localizer.Format("duration.arround")
            Dim minutesTxt As String = localizer.Format("duration.minutes")
            Dim toTxt As String = localizer.Format("duration.to")
            result = localizer.Format(source, SupportingFeatures.GetDuration(txtDurationMin.Text, txtDurationMax.Text, notSpecifiedTxt, arroundTxt, minutesTxt, toTxt), durationExtra)
        End If

        Return result
    End Function

    Private Function GetLocalizedCredits() As String
        'Check if txtCredits.Text contains the regular "All credits to @Designer for this task." format. 
        'If not, keep the text as is. If yes, localize it.

        Dim result As String = txtCredits.Text.Trim()
        Dim localizer As LocalizationManager = LocalizationManager.Instance

        'Match pattern like "All credits to @MajorDad for this task."
        Dim pattern As String = "^All credits to @([A-Za-z0-9_]+) for this task\.$"
        Dim m As Match = Regex.Match(result, pattern, RegexOptions.IgnoreCase)

        If m.Success Then
            Dim designer As String = m.Groups(1).Value
            'Assumes your localization key "credits.text" expects something like "{0}"
            result = localizer.Format("credits.text", designer)
        End If

        Return result
    End Function

    Public Function GetLocalizedAltitudeRestrictions() As String
        Dim result As String = txtAltRestrictions.Text.Trim()
        Dim localizer As LocalizationManager = LocalizationManager.Instance

        'Change only the start "## ⚠️ Altitude Restrictions" to its localized equivalent, keeping the rest
        Dim prefix As String = "## ⚠️ Altitude Restrictions"
        If result.StartsWith(prefix) Then
            Dim localizedPrefix As String = localizer.Format("task.details.restrictions")
            result = localizedPrefix & result.Substring(prefix.Length)
        End If

        Return result
    End Function
#End Region

#Region "Call to B21 Online Planner"

    Private Sub btnLoadB21Planner_Click(sender As Object, e As EventArgs) Handles toolStripB21Planner.Click

        If txtFlightPlanFile.Text Is String.Empty Then
            _SF.OpenB21Planner()
        Else
            If _XmlDocWeatherPreset Is Nothing OrElse _XmlDocWeatherPreset.InnerXml = String.Empty Then
                _SF.OpenB21Planner(txtFlightPlanFile.Text, _XmlDocFlightPlan.InnerXml)
            Else
                _SF.OpenB21Planner(txtFlightPlanFile.Text, _XmlDocFlightPlan.InnerXml, txtWeatherFile.Text, _XmlDocWeatherPreset.InnerXml)
            End If

            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "After reviewing or editing the flight plan, did you make any modification and would like to reload the flight plan here?", "Coming back from B21 Planner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    'Reload the flight plan
                    LoadFlightPlan(txtFlightPlanFile.Text)
                End If
            End Using

        End If

    End Sub


#End Region

#Region "Load/Save/Create Package (buttons on top)"

#Region "Event Handlers"

    Private Sub btnLoadConfig_Click(sender As Object, e As EventArgs) Handles toolStripOpen.Click
        If CheckUnsavedAndConfirmAction("load a file") Then
            LoadFileDialog()
        End If
        Select Case mainTabControl.SelectedTab.Name
            Case "tabBriefing"
                GenerateBriefing()
            Case "tabDiscord"
                BuildFPResults()
                BuildWeatherCloudLayers()
                BuildWeatherWindLayers()
                BuildWeatherInfoResults()
        End Select

    End Sub

    Private Sub btnLoadFromWSG(sender As Object, e As EventArgs) Handles toolStripOpenFromWSG.Click
        Dim inputForm As New LoadWSGTaskInputForm()
        If inputForm.ShowDialog() = DialogResult.OK Then
            If inputForm.WSGEntrySeqTaskID <> 0 Then
                'Proceed to download the DPHX file
                'We need to call the script FindTaskUsingEntrySeqID to get the TaskID
                Dim taskTitle As String = String.Empty
                Dim taskID As String = SupportingFeatures.FetchTaskIDUsingEntrySeqID(inputForm.WSGEntrySeqTaskID, taskTitle, Not (_isSuperUser))
                If taskID <> String.Empty Then
                    'Make sure temporary folder is available and empty
                    Dim tempDPHXFromWSGFolder As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempDPHXFromWSG")
                    If Directory.Exists(tempDPHXFromWSGFolder) Then
                        'Check if it contains anything else than DPHX files!
                        If SupportingFeatures.ContainsDifferentExtensions(tempDPHXFromWSGFolder, "dphx") Then
                            Using New Centered_MessageBox(Me)
                                If MessageBox.Show($"The temporary folder is about to be deleted and it contains files! Are you sure you want to proceed?", "Temporary folder warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                                    Exit Sub
                                End If
                            End Using
                        End If
                        'Clean it up
                        SupportingFeatures.DeleteFolderAndFiles(tempDPHXFromWSGFolder, True)
                    End If
                    Directory.CreateDirectory($"{tempDPHXFromWSGFolder}\{taskTitle}")
                    Dim selectedFile As String = SupportingFeatures.DownloadTaskFile(taskID, taskTitle, $"{tempDPHXFromWSGFolder}\{taskTitle}")
                    If selectedFile <> String.Empty Then
                        LoadFile(selectedFile, SessionSettings.LastUsedFileLocation)
                    End If
                Else
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show("Task does not exist or is not available yet!", "Task ID not found or unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Using
                End If
            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("No task ID specified!", "Invalid task ID", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        End If
    End Sub

    Private Sub btnSaveConfig_Click(sender As Object, e As EventArgs) Handles toolStripSave.Click

        SaveSession()

    End Sub

    Private Sub btnClone_Click(sender As Object, e As EventArgs) Handles toolStripClone.Click

        'Remove any IDs from the data
        _loadingFile = True
        txtTaskID.Text = String.Empty
        txtTemporaryTaskID.Text = String.Empty
        _TaskEntrySeqID = 0
        _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
        _taskDiscordPostID = String.Empty

        'Try to rename the task title with the next version number (if any, or else v2)
        Dim t = txtTitle.Text
        Dim m = Regex.Match(t, "\bv(\d+)\s*$", RegexOptions.IgnoreCase)

        If m.Success Then
            Dim n = Integer.Parse(m.Groups(1).Value) + 1
            txtTitle.Text = Regex.Replace(t, "\bv\d+\s*$", $"v{n}", RegexOptions.IgnoreCase)
        Else
            txtTitle.Text = t.TrimEnd() & " v2"
        End If

        _loadingFile = False
        SaveSession()
        Dim currentFile As String = CurrentSessionFile
        ResetForm()

        _loadingFile = True
        CurrentSessionFile = currentFile
        LoadSessionData(currentFile)
        UpdateWeatherPresetButtonState()
        SessionUntouched()
        _loadingFile = False

    End Sub

    Private Sub btnCreateShareablePack_Click(sender As Object, e As EventArgs) Handles toolStripSharePackage.Click

        If CurrentSessionFile <> String.Empty Then
            Dim DPHXFilename As String = $"{Path.GetDirectoryName(CurrentSessionFile)}\{Path.GetFileNameWithoutExtension(CurrentSessionFile)}.dphx"
            If File.Exists(DPHXFilename) Then
                If CheckUnsavedAndConfirmAction("share DPHX package") Then
                    Dim allFiles As New Specialized.StringCollection
                    allFiles.Add(DPHXFilename)
                    Clipboard.SetFileDropList(allFiles)
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me, "The package file (dphx) has been copied to your clipboard.", "Shareable Session Package created", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End If
            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "You need to save your session first!", "Shareable Session Package created", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "You need to save your session first!", "Shareable Session Package created", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

#End Region

    Private Sub LoadFileDialog()

        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If

        OpenFileDialog1.FileName = String.Empty
        OpenFileDialog1.Title = "Select session or package file to load"
        OpenFileDialog1.Filter = "Discord Post Helper|*.dph;*.dphx"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(OpenFileDialog1.FileName)
            LoadFile(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub LoadFile(selectedFilename As String, Optional dphxUnpackFolder As String = "")

        Dim validSessionFile As Boolean = True

        'Check if the selected file is a dph or dphx files
        If Path.GetExtension(selectedFilename) = ".dphx" Then
            'Package - we need to unpack it first
            selectedFilename = _SF.UnpackDPHXFile(selectedFilename, dphxUnpackFolder)

            If selectedFilename = String.Empty Then
                validSessionFile = False
            Else
                validSessionFile = True
                SessionSettings.LastUsedFileLocation = dphxUnpackFolder
            End If
        End If

        If validSessionFile Then
            ResetForm()
            _loadingFile = True
            CurrentSessionFile = selectedFilename
            LoadSessionData(selectedFilename)
            GenerateBriefing()
            _loadingFile = False
            CheckWhichOptionsCanBeEnabled()
            UpdateWeatherPresetButtonState()
        End If

    End Sub

    Private Sub SaveSessionData(filename As String)

        Dim allCurrentData As AllData = SetAndRetrieveSessionData()

        Dim serializer As New XmlSerializer(GetType(AllData))
        Using stream As New FileStream(filename, FileMode.Create)
            serializer.Serialize(stream, allCurrentData)
        End Using

        CurrentSessionFile = filename
        SetFormCaption(filename)
        UpdateWeatherPresetButtonState()

    End Sub

    Private Function SetAndRetrieveSessionData() As AllData

        Dim allCurrentData As New AllData()

        With allCurrentData
            .FlightPlanFilename = txtFlightPlanFile.Text
            .WeatherFilename = txtWeatherFile.Text
            .WeatherFilenameSecondary = _secondaryWPRFilename
            .SSCPresetName = _sscPresetName
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
            .SoaringWaves = chkSoaringTypeWave.Checked
            .SoaringDynamic = chkSoaringTypeDynamic.Checked
            .SoaringExtraInfo = txtSoaringTypeExtraInfo.Text
            .DurationMin = txtDurationMin.Text
            .DurationMax = txtDurationMax.Text
            .DurationExtraInfo = txtDurationExtraInfo.Text
            .RecommendedGliders = cboRecommendedGliders.Text
            .DifficultyRating = cboDifficulty.Text
            .DifficultyExtraInfo = txtDifficultyExtraInfo.Text
            .LockShortDescription = chkDescriptionLock.Checked
            .ShortDescription = txtShortDescription.Text.Replace(Environment.NewLine, "($*$)")
            .Credits = txtCredits.Text
            .LongDescription = txtLongDescription.Text.Replace(Environment.NewLine, "($*$)")
            .WeatherSummary = txtWeatherSummary.Text
            .SuppressBaroPressureWarningSymbol = chkSuppressWarningForBaroPressure.Checked
            .BaroPressureExtraInfo = txtBaroPressureExtraInfo.Text

            For i As Integer = 0 To lstAllFiles.Items.Count - 1
                .ExtraFiles.Add(lstAllFiles.Items(i))
            Next
            For i As Integer = 0 To lstAllCountries.Items.Count - 1
                .Countries.Add(lstAllCountries.Items(i))
            Next
            For i As Integer = 0 To lstAllRecommendedAddOns.Items.Count - 1
                .RecommendedAddOns.Add(lstAllRecommendedAddOns.Items(i))
            Next
            .DPHXPackageFilename = txtDPHXPackageFilename.Text
            .EventEnabled = chkActivateEvent.Checked
            .GroupClubId = cboGroupOrClubName.Text
            Dim clubExists As Boolean = _SF.DefaultKnownClubEvents.ContainsKey(cboGroupOrClubName.Text.ToUpper)
            If clubExists Then
                _ClubPreset = _SF.DefaultKnownClubEvents(cboGroupOrClubName.Text.ToUpper)
                .GroupClubName = txtClubFullName.Text
            Else
                .GroupClubName = String.Empty
            End If
            .TrackerGroup = txtTrackerGroup.Text
            .GroupEmoji = lblGroupEmoji.Text
            .TaskID = txtTaskID.Text
            .TemporaryTaskID = txtTemporaryTaskID.Text
            .EntrySeqID = _TaskEntrySeqID
            .TaskStatus = _TaskStatus
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
            .EventDescription = txtEventDescription.Text.Replace(Environment.NewLine, "($*$)")
            .EligibleAward = cboEligibleAward.SelectedIndex
            .EnableRepostInfo = chkRepost.Checked
            .RepostOriginalDate = dtRepostOriginalDate.Value
            .RepostOriginalURL = txtRepostOriginalURL.Text
            .URLGroupEventPost = txtGroupEventPostURL.Text
            .MapImageSelected = cboBriefingMap.Text
            .LockMapImage = chkLockMapImage.Checked
            .CoverImageSelected = cboCoverImage.Text
            .LockCoverImage = chkLockCoverImage.Checked
            .BeginnersGuide = cboBeginnersGuide.Text
            .BeginnersGuideCustom = txtOtherBeginnerLink.Text
            .GroupEventTeaserEnabled = chkEventTeaser.Checked
            .GroupEventTeaserMessage = txtEventTeaserMessage.Text.Replace(Environment.NewLine, "($*$)")
            .GroupEventTeaserAreaMapImage = txtEventTeaserAreaMapImage.Text
            .DelayedAvailability = chkDelayedAvailability.Checked
            .DelayedBasedOnGroupEvent = chkDelayBasedOnEvent.Checked
            .DelayedAvailabilityUnits = cboDelayUnits.SelectedIndex
            .DelayedAvailabilityNumber = CInt(txtMinutesBeforeMeeting.Text.Trim)
            .DelayedAvailabilityDateTime = dtAvailabilityTime.Value
            .DelayedAvailabilityRefly = chkAvailabilityRefly.Checked
            If _useTestMode Then
                .TestDiscordTaskID = _taskDiscordPostID
            End If
        End With

        Return allCurrentData

    End Function

    Private Sub LoadSessionData(filename As String)
        If File.Exists(filename) Then
            Dim serializer As New XmlSerializer(GetType(AllData))
            Dim allCurrentData As AllData

            On Error Resume Next

            Using stream As New FileStream(filename, FileMode.Open)
                allCurrentData = CType(serializer.Deserialize(stream), AllData)
            End Using

            'Add version to form title
            SetFormCaption(filename)

            'Set all fields
            With allCurrentData
                If File.Exists(.FlightPlanFilename) Then
                Else
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.FlightPlanFilename)}") Then
                        .FlightPlanFilename = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.FlightPlanFilename)}"
                    End If
                End If
                txtFlightPlanFile.Text = .FlightPlanFilename
                Me.Update()
                LoadFlightPlan(txtFlightPlanFile.Text)

                Dim weatherPrimary As String = .WeatherFilename
                Dim weatherSecondary As String = .WeatherFilenameSecondary

                If Not String.IsNullOrEmpty(weatherPrimary) AndAlso Not File.Exists(weatherPrimary) Then
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(weatherPrimary)}") Then
                        weatherPrimary = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(weatherPrimary)}"
                    End If
                End If

                If Not String.IsNullOrEmpty(weatherSecondary) AndAlso Not File.Exists(weatherSecondary) Then
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(weatherSecondary)}") Then
                        weatherSecondary = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(weatherSecondary)}"
                    End If
                End If

                _sscPresetName = If(String.IsNullOrEmpty(.SSCPresetName), String.Empty, .SSCPresetName)
                _primaryWPRFilename = weatherPrimary
                _secondaryWPRFilename = weatherSecondary
                If _secondaryWPRFilename Is Nothing Then
                    _secondaryWPRFilename = String.Empty
                End If
                txtWeatherFile.Text = weatherPrimary
                btnSyncWeatherTitle.Enabled = (_sscPresetName = String.Empty)
                Me.Update()
                LoadWeatherfile(txtWeatherFile.Text)
                UpdateSecondaryWeatherPresetName()

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
                chkSoaringTypeDynamic.Checked = .SoaringDynamic
                chkSoaringTypeThermal.Checked = .SoaringThermals
                chkSoaringTypeWave.Checked = .SoaringWaves
                txtSoaringTypeExtraInfo.Text = .SoaringExtraInfo
                txtDurationMin.Text = .DurationMin
                txtDurationMax.Text = .DurationMax
                txtDurationExtraInfo.Text = .DurationExtraInfo
                cboRecommendedGliders.Text = .RecommendedGliders
                Dim diffRatingIndex As Integer = 0
                Integer.TryParse(.DifficultyRating.Substring(0, 1), diffRatingIndex)
                cboDifficulty.SelectedIndex = diffRatingIndex
                txtDifficultyExtraInfo.Text = .DifficultyExtraInfo
                chkDescriptionLock.Checked = .LockShortDescription
                txtShortDescription.Text = .ShortDescription.Replace("($*$)", Environment.NewLine)
                txtCredits.Text = .Credits
                txtLongDescription.Text = .LongDescription.Replace("($*$)", Environment.NewLine)
                txtWeatherSummary.Text = .WeatherSummary
                chkSuppressWarningForBaroPressure.Checked = .SuppressBaroPressureWarningSymbol
                txtBaroPressureExtraInfo.Text = .BaroPressureExtraInfo

                If .ExtraFiles.Count > 0 Then
                    For i As Integer = 0 To .ExtraFiles.Count - 1

                        If Not File.Exists(.ExtraFiles(i)) Then
                            'Should expect the file to be in the same folder as the .dph file
                            .ExtraFiles(i) = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.ExtraFiles(i))}"
                        End If
                        If File.Exists(.ExtraFiles(i)) _
                           AndAlso _SF.ExtraFileExtensionIsValid(OpenFileDialog1.FileNames(i)) _
                           AndAlso Not lstAllFiles.Items.Contains(OpenFileDialog1.FileNames(i)) Then

                            lstAllFiles.Items.Add(.ExtraFiles(i))

                        End If
                    Next
                End If
                If .Countries.Count > 0 Then
                    For i As Integer = 0 To .Countries.Count - 1
                        If _SF.CountryFlagCodes.ContainsKey(.Countries(i)) AndAlso Not lstAllCountries.Items.Contains(.Countries(i)) Then
                            lstAllCountries.Items.Add(.Countries(i))
                        End If
                    Next
                End If
                If .RecommendedAddOns.Count > 0 Then
                    For i As Integer = 0 To .RecommendedAddOns.Count - 1
                        lstAllRecommendedAddOns.Items.Add(.RecommendedAddOns(i))
                    Next
                End If
                If File.Exists(.DPHXPackageFilename) Then
                    txtDPHXPackageFilename.Text = .DPHXPackageFilename
                Else
                    'Should expect the file to be in the same folder as the .dph file
                    .DPHXPackageFilename = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.DPHXPackageFilename)}"
                    txtDPHXPackageFilename.Text = .DPHXPackageFilename
                End If

                'TaskID
                If .TaskID = String.Empty AndAlso .DiscordTaskID <> String.Empty Then
                    .TaskID = .DiscordTaskID
                ElseIf .TaskID = String.Empty AndAlso .DiscordTaskThreadURL <> String.Empty AndAlso SupportingFeatures.IsValidURL(.DiscordTaskThreadURL) Then
                    .TaskID = SupportingFeatures.ExtractMessageIDFromDiscordURL(.DiscordTaskThreadURL, True)
                End If

                _TaskEntrySeqID = .EntrySeqID
                _TaskStatus = .TaskStatus
                If _TaskEntrySeqID = 0 Then
                    _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
                End If
                txtTaskID.Text = .TaskID
                If txtTaskID.Text.StartsWith("T") OrElse chkTestMode.Checked Then
                    ToggleTestMode(True)
                    _TaskEntrySeqID = 0
                    _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
                    _taskDiscordPostID = .TestDiscordTaskID
                    txtTemporaryTaskID.Text = String.Empty
                    txtTaskID.Text = String.Empty
                End If
                lblCountriesTestModeMsg.Visible = _useTestMode
                If _TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation Then
                    txtTemporaryTaskID.Text = .TemporaryTaskID
                Else
                    txtTemporaryTaskID.Text = String.Empty
                End If
                chkActivateEvent.Checked = .EventEnabled
                cboGroupOrClubName.Text = .GroupClubId
                txtClubFullName.Text = .GroupClubName
                txtTrackerGroup.Text = .TrackerGroup
                lblGroupEmoji.Text = .GroupEmoji
                SetClubPreset()
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
                txtEventDescription.Text = .EventDescription.Replace("($*$)", Environment.NewLine)
                cboEligibleAward.SelectedIndex = .EligibleAward
                chkRepost.Checked = .EnableRepostInfo
                dtRepostOriginalDate.Value = .RepostOriginalDate
                txtRepostOriginalURL.Text = .RepostOriginalURL
                txtGroupEventPostURL.Text = .URLGroupEventPost
                cboBeginnersGuide.Text = .BeginnersGuide
                If cboBeginnersGuide.Text = String.Empty Then
                    cboBeginnersGuide.Text = "None"
                End If
                txtOtherBeginnerLink.Text = .BeginnersGuideCustom
                chkLockMapImage.Checked = .LockMapImage
                chkLockCoverImage.Checked = .LockCoverImage

                chkDelayedAvailability.Checked = .DelayedAvailability
                chkDelayBasedOnEvent.Checked = .DelayedBasedOnGroupEvent
                cboDelayUnits.SelectedIndex = .DelayedAvailabilityUnits
                txtMinutesBeforeMeeting.Text = .DelayedAvailabilityNumber
                dtAvailabilityDate.Value = .DelayedAvailabilityDateTime
                dtAvailabilityTime.Value = .DelayedAvailabilityDateTime
                chkAvailabilityRefly.Checked = .DelayedAvailabilityRefly

                chkEventTeaser.Checked = .GroupEventTeaserEnabled
                txtEventTeaserMessage.Text = .GroupEventTeaserMessage.Replace("($*$)", Environment.NewLine)

                If Not File.Exists(.GroupEventTeaserAreaMapImage) Then
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.GroupEventTeaserAreaMapImage)}") Then
                        .GroupEventTeaserAreaMapImage = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.GroupEventTeaserAreaMapImage)}"
                    End If
                End If
                txtEventTeaserAreaMapImage.Text = .GroupEventTeaserAreaMapImage

                If Not File.Exists(.MapImageSelected) Then
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.MapImageSelected)}") Then
                        .MapImageSelected = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.MapImageSelected)}"
                    End If
                End If
                LoadPossibleImagesInMapDropdown(.MapImageSelected)
                If Not File.Exists(.CoverImageSelected) Then
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.CoverImageSelected)}") Then
                        .CoverImageSelected = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.CoverImageSelected)}"
                    End If
                End If
                LoadPossibleImagesInCoverDropdown(.CoverImageSelected)
            End With

            If _TaskEntrySeqID > 0 Then
                GetTaskDetails(txtTaskID.Text.Trim, _TaskEntrySeqID)
            Else
                SetTBTaskDetailsLabel()
            End If
            'BuildFPResults()
            BuildWeatherInfoResults()
            BuildRecAddOnsText()
            'BuildGroupFlightPost()
            'BuildDiscordEventDescription()

            FixForDropDownCombos()

            _sessionModified = False

        End If

    End Sub

    Private Sub FixForDropDownCombos()
        cboRecommendedGliders.SelectionStart = cboRecommendedGliders.Text.Length
        cboGroupOrClubName.SelectionStart = cboGroupOrClubName.Text.Length
        cboVoiceChannel.SelectionStart = cboVoiceChannel.Text.Length
    End Sub

    Private Sub SessionModified(source As SourceOfChange)

        If _loadingFile Or _isInitiatizing Then
            Exit Sub
        End If

        If source = SourceOfChange.TaskTab AndAlso Not _taskModified Then
            _taskModified = True
        ElseIf source = SourceOfChange.EventTab AndAlso Not _eventModified Then
            _eventModified = True
        ElseIf source = SourceOfChange.DiscordTab AndAlso Not _discordModified Then
            _discordModified = True
        End If

        If (Not _sessionModified) Then
            _sessionModified = True
            SetSaveButtonFont()
        End If

    End Sub

    Private Sub SessionModified(sourceObject As Object)
        If (Not _loadingFile) AndAlso (Not _isInitiatizing) Then
            Select Case FindControlParentTab(sourceObject)
                Case "tabFlightPlan"
                    SessionModified(SourceOfChange.TaskTab)
                Case "tabEvent"
                    SessionModified(SourceOfChange.EventTab)
                Case "tabDiscord"
                    SessionModified(SourceOfChange.DiscordTab)
                Case Else
                    SessionModified(SourceOfChange.TaskTab)
            End Select
        End If
    End Sub

    Private Function FindControlParentTab(obj As Object) As String
        ' Check if the object is a Control
        If TypeOf obj IsNot Control Then
            Return String.Empty
        End If

        ' Cast the object to a Control
        Dim control As Control = DirectCast(obj, Control)

        ' Traverse up the control hierarchy to find the TabControl with the name "mainTabControl"
        While control IsNot Nothing
            ' Check if the parent is a TabPage
            If TypeOf control.Parent Is TabPage Then
                Dim parentTabPage As TabPage = DirectCast(control.Parent, TabPage)

                ' Check if the TabPage belongs to a TabControl named "mainTabControl"
                If TypeOf parentTabPage.Parent Is TabControl Then
                    Dim parentTabControl As TabControl = DirectCast(parentTabPage.Parent, TabControl)
                    If parentTabControl.Name = "mainTabControl" Then
                        ' Return the name of the TabPage (the previous parent)
                        Return parentTabPage.Name
                    End If
                End If
            End If

            ' Move up to the parent control
            control = control.Parent
        End While

        ' If no matching TabControl is found, return Nothing or throw an exception
        Return Nothing
    End Function

    Public Sub SessionUntouched()

        If _sessionModified Then
            _sessionModified = False
            _taskModified = False
            _eventModified = False
            _discordModified = False
            SetSaveButtonFont()
        End If
    End Sub

    Public Sub SetSaveButtonFont()

        If _sessionModified Then
            toolStripSave.Font = New Font(toolStripSave.Font, FontStyle.Bold)
            toolStripSave.ForeColor = Color.Red
            toolStripReload.Visible = True
        Else
            toolStripSave.Font = New Font(toolStripSave.Font, FontStyle.Regular)
            toolStripSave.ForeColor = DefaultForeColor
            toolStripReload.Visible = False
        End If

    End Sub

    Private Sub UpdateWeatherPresetButtonState()
        btnSelectWeatherFile.Enabled = (CurrentSessionFile <> String.Empty)
    End Sub

#End Region

#Region "Task Browser Code"

#Region "Event Handlers"

    Private Sub btnRetrieveEventsFeaturingTask_Click(sender As Object, e As EventArgs) Handles btnRetrieveEventsFeaturingTask.Click
        Dim taskEvents As List(Of String) = FetchTaskEventSummaries(_TaskEntrySeqID)

        lstEventsFeaturingTask.Items.Clear()

        ' Add results
        For Each result As String In taskEvents
            lstEventsFeaturingTask.Items.Add(result)
        Next

        ' Enable horizontal scrollbar
        lstEventsFeaturingTask.HorizontalScrollbar = True

        ' Measure widest string in pixels
        Dim maxWidth As Integer = 0
        Using g As Graphics = lstEventsFeaturingTask.CreateGraphics()
            For Each item As String In lstEventsFeaturingTask.Items
                Dim itemWidth As Integer = CInt(g.MeasureString(item, lstEventsFeaturingTask.Font).Width)
                If itemWidth > maxWidth Then
                    maxWidth = itemWidth
                End If
            Next
        End Using

        ' Apply the horizontal extent so the scrollbar works
        lstEventsFeaturingTask.HorizontalExtent = maxWidth
    End Sub

    Private Sub btnDeleteFromTaskBrowser_Click(sender As Object, e As EventArgs) Handles btnDeleteFromTaskBrowser.Click
        If UserCanDeleteTask Then
            'Ask confirmation
            Dim result As DialogResult
            Using New Centered_MessageBox(Me)
                result = MessageBox.Show($"Are you REALLY sure you want to delete this task from WeSimGlide.org and Discord?", "Removing a task from WeSimGlide.org and Discord", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            End Using
            If result = DialogResult.Yes Then
                Using New Centered_MessageBox(Me)
                    result = MessageBox.Show($"This is your last chance, this operation WILL delete the task entry on WSG and on Discord. Are you still sure?", "Removing a task from WeSimGlide.org and Discord", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                End Using
                If result = DialogResult.Yes Then
                    DeleteTaskFromWSG()
                    SetTBTaskDetailsLabel()
                End If
            End If
        End If
    End Sub


#End Region

#Region "Permissions"
    Private ReadOnly Property IsClubAuthorizedEventOrganizer As Boolean
        Get
            Return (Not lblEventClubNotAuthorized.Enabled)
        End Get
    End Property
    Private ReadOnly Property IsOwnerOrShared As Boolean
        Get
            Return _currentWSGTaskOwner = _userName OrElse _currentWSGSharedWith.Contains(_userName) OrElse _isSuperUser
        End Get
    End Property
    Private ReadOnly Property UserCanCreateTask As Boolean
        Get
            If _userPermissions.ContainsKey("CreateTask") Then
                Return _userPermissions("CreateTask")
            Else
                Return False
            End If
        End Get
    End Property
    Private ReadOnly Property UserCanUpdateTask As Boolean
        Get
            If _temporaryRightToUpdateTask Then
                Return True
            End If
            If (_userPermissions.ContainsKey("UpdateTask") AndAlso IsOwnerOrShared()) Then
                Return _userPermissions("UpdateTask")
            Else
                Return False
            End If
        End Get
    End Property
    Private ReadOnly Property UserCanDeleteTask As Boolean
        Get
            If _userPermissions.ContainsKey("DeleteTask") AndAlso IsOwnerOrShared() Then
                Return _userPermissions("DeleteTask")
            Else
                Return False
            End If
        End Get
    End Property
    Private ReadOnly Property UserCanCreateEvent As Boolean
        Get
            If _userPermissions.ContainsKey("CreateEvent") Then
                Return _userPermissions("CreateEvent")
            Else
                Return False
            End If
        End Get
    End Property
    Private ReadOnly Property UserCanUpdateEvent As Boolean
        Get
            If _userPermissions.ContainsKey("UpdateEvent") Then
                Return _userPermissions("UpdateEvent")
            Else
                Return False
            End If
        End Get
    End Property
    Private ReadOnly Property UserCanDeleteEvent As Boolean
        Get
            If _userPermissions.ContainsKey("DeleteEvent") Then
                Return _userPermissions("DeleteEvent")
            Else
                Return False
            End If
        End Get
    End Property

#End Region

#Region "Tasks Subs"

    Private Sub DeleteTaskFromWSG()

        Dim taskInfo As AllData = SetAndRetrieveSessionData()

        Dim discordDeleted As Boolean = False
        Dim serverMessage As String = String.Empty
        Dim result As Boolean = DeleteTaskFromServer(taskInfo.EntrySeqID, discordDeleted, serverMessage)

        If result Then
            Dim discordTaskIDToRemove As String = _taskDiscordPostID
            _TaskEntrySeqID = 0
            _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
            _taskDiscordPostID = String.Empty
            txtTaskID.Text = String.Empty
            txtTemporaryTaskID.Text = String.Empty
            SetAndRetrieveSessionData()
            SaveSession()
            Using New Centered_MessageBox(Me)
                If String.IsNullOrWhiteSpace(serverMessage) Then
                    serverMessage = "Task deletion request has been queued and will be processed shortly."
                End If
                MessageBox.Show(serverMessage, "Removal Request Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
            'Bring user to Discord to delete task!
            If Not discordDeleted AndAlso discordTaskIDToRemove.Length > 0 Then
                'There is a Discord ID that needs deletion
                If SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.TaskLibraryDiscordURL}/{discordTaskIDToRemove}") Then
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show("I have opened Discord on the correct task. You should now delete it along with its thread (if any).", "Discord removal", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                    SupportingFeatures.BringDiscordToTop()
                Else
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show("You should now go into Discord, find the task and delete it along with its thread (if any).", "Discord removal", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                    SupportingFeatures.BringDiscordToTop()
                End If
            End If

        Else
            Using New Centered_MessageBox(Me)
                Dim failureMessage As String = serverMessage
                If String.IsNullOrWhiteSpace(failureMessage) Then
                    failureMessage = "Failed to submit the task deletion request."
                End If
                MessageBox.Show(failureMessage, "Removal Request Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Public Function DeleteTaskFromServer(entrySeqID As Integer, ByRef discordDeleted As Boolean, ByRef serverMessage As String) As Boolean

        If _useTestMode Then
            Throw New Exception("Test mode is active. WSG connection should not be possible!")
        End If
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}DeleteTask.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "application/x-www-form-urlencoded"

        Dim postData As String = $"EntrySeqID={Uri.EscapeDataString(entrySeqID.ToString)}&UserID={Uri.EscapeDataString(_userPermissionID)}"
        Dim data As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentLength = data.Length

        Try
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.WaitCursor

            Using stream As Stream = request.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    ' Assuming the response is a JSON object with a "status" field
                    Dim result As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
                    If result.ContainsKey("message") AndAlso result("message") IsNot Nothing Then
                        serverMessage = result("message").ToString()
                    End If
                    Dim isSuccess As Boolean = result("status").ToString() = "success"
                    discordDeleted = isSuccess OrElse (Not result.ContainsKey("discordError")) OrElse result("discordError") = String.Empty
                    Return isSuccess
                End Using
            End Using
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default

        Catch ex As Exception
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
            ' Handle the exception
            ' Log the error or display a message
            serverMessage = ex.Message
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error deleting task: {ex.Message}", "Task deletion error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Public Function PostTaskToTestChannel(ByVal content As String, ByVal discordID As String, ByVal deletePost As Boolean, ByRef postID As String) As Boolean

        ' Build the URL to your PHP script.
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}PostingTaskToTestChannel.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "application/x-www-form-urlencoded"

        ' Build POST data.
        ' The PHP script expects:
        '   content: the text to post (or edit)
        '   discordID: the existing Discord post ID (if editing or deleting)
        '   delete: a string "true" if deleting, otherwise "false"
        If discordID Is Nothing Then
            discordID = String.Empty
        End If
        Dim postData As String = $"content={Uri.EscapeDataString(content)}&discordID={Uri.EscapeDataString(discordID)}&delete={Uri.EscapeDataString(deletePost.ToString().ToLower())}"
        Dim data As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentLength = data.Length

        Try
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.WaitCursor

            Using stream As Stream = request.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
                    Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default

                    If deletePost Then
                        ' For deletion, we expect a response with status "success" only.
                        Return result("status").ToString() = "success"
                    Else
                        ' For create or update, if successful, the postID is returned.
                        If result("status").ToString() = "success" Then
                            postID = result("postID").ToString()
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End Using
            End Using

        Catch ex As Exception
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error posting task: {ex.Message}", "Task posting error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Private Function GetFileUpdateUTCDateTime(filePath As String, Optional inUTC As Boolean = True) As DateTime
        Dim fileInfo As New FileInfo(filePath)
        Dim localDateTime As DateTime = fileInfo.LastWriteTime
        If inUTC Then
            Return localDateTime.ToUniversalTime()
        Else
            Return localDateTime
        End If
    End Function

    Private Function ResizeImageAndGetBytes(inputPath As String, maxWidth As Integer, maxHeight As Integer, quality As Long, Optional keepSize As Boolean = False) As Byte()
        ' Load the original image
        Using image As Image = Image.FromFile(inputPath)
            ' If keepSize is True, skip resizing and save the original size
            If keepSize Then
                Using ms As New MemoryStream()
                    Dim jpgEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                    Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                    Dim myEncoderParameters As New EncoderParameters(1)
                    Dim myEncoderParameter As New EncoderParameter(myEncoder, quality)
                    myEncoderParameters.Param(0) = myEncoderParameter

                    ' Save the image to a memory stream in JPEG format
                    image.Save(ms, jpgEncoder, myEncoderParameters)

                    ' Convert the image to a byte array
                    Return ms.ToArray()
                End Using
            Else
                ' Calculate the new size maintaining aspect ratio
                Dim ratioX As Double = maxWidth / image.Width
                Dim ratioY As Double = maxHeight / image.Height
                Dim ratio As Double = Math.Min(ratioX, ratioY)

                Dim newWidth As Integer = CInt(image.Width * ratio)
                Dim newHeight As Integer = CInt(image.Height * ratio)

                ' Create a new Bitmap with the proper dimensions
                Using thumbnail As New Bitmap(newWidth, newHeight)
                    Using graphics As Graphics = Graphics.FromImage(thumbnail)
                        ' High quality settings for better output
                        graphics.CompositingQuality = CompositingQuality.HighQuality
                        graphics.SmoothingMode = SmoothingMode.HighQuality
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic

                        ' Draw the original image onto the thumbnail
                        graphics.DrawImage(image, 0, 0, newWidth, newHeight)
                    End Using

                    ' Image quality settings
                    Using ms As New MemoryStream()
                        Dim jpgEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                        Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                        Dim myEncoderParameters As New EncoderParameters(1)
                        Dim myEncoderParameter As New EncoderParameter(myEncoder, quality)
                        myEncoderParameters.Param(0) = myEncoderParameter

                        ' Save the image to a memory stream in JPEG format
                        thumbnail.Save(ms, jpgEncoder, myEncoderParameters)

                        ' Convert the image to a byte array
                        Return ms.ToArray()
                    End Using
                End Using
            End If
        End Using
    End Function

    Private Function GetEncoder(format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    Private Function CreateWSGTaskPart1() As Boolean

        Dim taskInfo As AllData = SetAndRetrieveSessionData()

        ' Assume these values are computed or retrieved as part of the taskInfo
        Dim latitudeMin As Double
        Dim latitudeMax As Double
        Dim longitudeMin As Double
        Dim longitudeMax As Double
        _SF.GetTaskBoundaries(longitudeMin, longitudeMax, latitudeMin, latitudeMax)

        ' 1) Check for possible duplicates (title OR bbox)
        Dim candidates = FetchPossibleDuplicates(taskInfo.Title, latitudeMin, latitudeMax, longitudeMin, longitudeMax)

        ' 1a) Hard-stop on exact title match (case-insensitive)
        If candidates IsNot Nothing AndAlso candidates.Any(Function(c) String.Equals(c.Title, taskInfo.Title, StringComparison.OrdinalIgnoreCase)) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"Task named ""{taskInfo.Title}"" already exists.{Environment.NewLine}Please change the title before publishing.",
                                "Duplicate title", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return False
        End If

        ' 1b) Soft-stop on identical waypoints (same geometry)
        '     If any candidate has identical waypoints, warn and ask confirmation.
        Dim identical As CandidateTask = Nothing
        Dim currentTaskCandidate As New CandidateTask With {
            .Title = taskInfo.Title,
            .PLNXML = _XmlDocFlightPlan.InnerXml,
            .WPRXML = _XmlDocWeatherPreset.InnerXml,
            .SimDateTime = SupportingFeatures.GetFullEventDateTimeInLocal(taskInfo.SimDate, taskInfo.SimTime, False)
            }
        currentTaskCandidate.CompleteTaskData()

        Dim epsilonMeters As Double = 100.0 ' matches your bbox epsilon of 0.001° (~100 m)
        Dim diffSummary As String = Nothing
        For Each candidate As CandidateTask In candidates
            candidate.CompleteTaskData()

            Dim cmp = CompareTasksForDuplicate(currentTaskCandidate, candidate, epsilonMeters)

            If Not cmp.GeometryEqual Then
                ' hard difference: skip this candidate
                Continue For
            End If

            If cmp.ExactMatch Then
                ' Same geometry + no soft diffs -> reject creation outright
                Using New Centered_MessageBox(Me)
                    MessageBox.Show($"An identical task already exists:{Environment.NewLine}""{candidate.Title}"" ({candidate.EntrySeqID}){Environment.NewLine}Please update the existing task instead.",
                                    "Duplicate task", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Using
                Return False
            Else
                ' Same geometry but with soft differences -> ask for confirmation (keep the first)
                identical = candidate
                diffSummary = String.Join(Environment.NewLine, cmp.Differences)
                Exit For
            End If
        Next

        If identical IsNot Nothing Then
            Using New Centered_MessageBox(Me)
                Dim ask = MessageBox.Show($"A task with the same waypoints already exists:{Environment.NewLine}• {identical.Title} ({identical.EntrySeqID}){Environment.NewLine}{Environment.NewLine}Differences detected:{Environment.NewLine}{diffSummary}{Environment.NewLine}{Environment.NewLine}Do you still want to publish?",
                    "Possible duplicate (waypoints)", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If ask = DialogResult.No Then Return False
            End Using
        End If

        ' Update the taskData dictionary with minimal fields for creation
        Dim taskData As New Dictionary(Of String, Object) From {
            {"TemporaryTaskID", taskInfo.TemporaryTaskID},
            {"Title", taskInfo.Title},
            {"LastUpdate", GetFileUpdateUTCDateTime(CurrentSessionFile).ToString("yyyy-MM-dd HH:mm:ss")},
            {"SimDateTime", SupportingFeatures.GetFullEventDateTimeInLocal(taskInfo.SimDate, taskInfo.SimTime, False)},
            {"IncludeYear", If(taskInfo.IncludeYear, 1, 0)},
            {"SoaringRidge", If(taskInfo.SoaringRidge, 1, 0)},
            {"SoaringThermals", If(taskInfo.SoaringThermals, 1, 0)},
            {"SoaringWaves", If(taskInfo.SoaringWaves, 1, 0)},
            {"SoaringDynamic", If(taskInfo.SoaringDynamic, 1, 0)},
            {"TaskDistance", CInt(_TaskTotalDistanceInKm)},
            {"TotalDistance", CInt(_FlightTotalDistanceInKm)},
            {"DBEntryUpdate", Now.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss")},
            {"RecommendedAddOns", If(taskInfo.RecommendedAddOns Is Nothing OrElse taskInfo.RecommendedAddOns.Count = 0, 0, 1)},
            {"LatMin", latitudeMin},
            {"LatMax", latitudeMax},
            {"LongMin", longitudeMin},
            {"LongMax", longitudeMax},
            {"OwnerName", cboTaskOwner.Text.Trim},
            {"Mode", "CreateTask"}
        }

        Dim result As Boolean = CallScriptToCreateWSGTaskPart1(taskData)

        If result Then
            'Nothing to do
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Failed to upload the task.", "Upload Result", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

        Return result

    End Function

    Private Function CompareTasksForDuplicate(currentTask As CandidateTask,
                                          candidate As CandidateTask,
                                          epsilonMeters As Double) As DuplicateCheckResult
        Dim res As New DuplicateCheckResult()

        Dim currentTaskWayPoints = currentTask.AllWaypoints
        Dim candidateWayPoints = candidate.AllWaypoints

        ' 1) Count must match
        If currentTaskWayPoints.Count <> candidateWayPoints.Count Then
            res.GeometryEqual = False
            res.HardDifference = $"Different number of waypoints ({currentTaskWayPoints.Count} vs {candidateWayPoints.Count})."
            Return res
        End If

        ' 2) Positions must all match within epsilon (sequence order)
        For i = 0 To currentTaskWayPoints.Count - 1
            Dim distanceBetweenWaypoints = SupportingFeatures.HaversineMeters(currentTaskWayPoints(i), candidateWayPoints(i))
            If distanceBetweenWaypoints > epsilonMeters Then
                res.GeometryEqual = False
                res.HardDifference = $"Waypoint #{i + 1} ""{currentTaskWayPoints(i).WaypointName}"" differs by ~{Math.Round(distanceBetweenWaypoints)} m."
                Return res
            End If
        Next
        res.GeometryEqual = True

        ' 3) Soft differences (need confirmation)
        ' 3a) Altitude restrictions (per waypoint)
        For i = 0 To currentTaskWayPoints.Count - 1
            Dim aMin = currentTaskWayPoints(i).MinAltFeet
            Dim aMax = currentTaskWayPoints(i).MaxAltFeet
            Dim bMin = candidateWayPoints(i).MinAltFeet
            Dim bMax = candidateWayPoints(i).MaxAltFeet

            If aMin.GetValueOrDefault(Integer.MinValue) <> bMin.GetValueOrDefault(Integer.MinValue) OrElse aMax.GetValueOrDefault(Integer.MinValue) <> bMax.GetValueOrDefault(Integer.MinValue) Then
                Dim aTxt = If(aMin.HasValue Or aMax.HasValue, $"min {If(aMin, CInt(0))}', max {If(aMax, CInt(0))}'", "none")
                Dim bTxt = If(bMin.HasValue Or bMax.HasValue, $"min {If(bMin, CInt(0))}', max {If(bMax, CInt(0))}'", "none")
                res.Differences.Add($"• Waypoint #{i + 1} ""{currentTaskWayPoints(i).WaypointName}"": altitude restriction differs ({aTxt} vs {bTxt}).")
            End If
        Next

        ' 3b) Diameter differences (per waypoint)
        For i = 0 To currentTaskWayPoints.Count - 1
            If currentTaskWayPoints(i).DiameterMeters <> candidateWayPoints(i).DiameterMeters Then
                res.Differences.Add($"• Waypoint #{i + 1} ""{currentTaskWayPoints(i).WaypointName}"": gate diameter differs ({currentTaskWayPoints(i).DiameterMeters} m vs {candidateWayPoints(i).DiameterMeters} m).")
            End If
        Next

        ' 3c) Weather XML identical?
        Dim wxA = SupportingFeatures.NormalizeXml(currentTask.WPRXML)
        Dim wxB = SupportingFeatures.NormalizeXml(candidate.WPRXML)
        If wxA <> wxB Then
            res.Differences.Add("• Weather preset differs (WPR XML not identical).")
        End If

        ' 3d) Simulator local date/time
        If currentTask.SimDateTime.HasValue OrElse candidate.SimDateTime.HasValue Then
            If Not currentTask.SimDateTime.HasValue OrElse Not candidate.SimDateTime.HasValue Then
                res.Differences.Add("• Sim local date/time differs (missing in one task).")
            ElseIf currentTask.SimDateTime.Value <> candidate.SimDateTime.Value Then
                res.Differences.Add($"• Sim local date/time differs ({currentTask.SimDateTime.Value:yyyy-MM-dd HH:mm} vs {candidate.SimDateTime.Value:yyyy-MM-dd HH:mm}).")
            End If
        End If

        ' 3e) AAT Minimum Duration difference?
        For i = 0 To currentTaskWayPoints.Count - 1
            If currentTaskWayPoints(i).AATMinDuration <> candidateWayPoints(i).AATMinDuration Then
                res.Differences.Add($"• AAT Minimum Duration differs ({SupportingFeatures.FormatTimeSpanAsText(currentTaskWayPoints(i).AATMinDuration)} vs {SupportingFeatures.FormatTimeSpanAsText(candidateWayPoints(i).AATMinDuration)}).")
            End If
        Next

        ' 4) Exact or not
        res.ExactMatch = (res.GeometryEqual AndAlso res.Differences.Count = 0)
        Return res
    End Function

    Private Function FetchPossibleDuplicates(title As String,
                                         latMin As Double, latMax As Double,
                                         lonMin As Double, lonMax As Double,
                                         Optional epsilon As Double = 0.0005) As List(Of CandidateTask)
        If _useTestMode Then
            Throw New Exception("Test mode is active. WSG connection should not be possible!")
        End If

        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}GetPossibleDuplicateTasks.php" &
              $"?Title={Uri.EscapeDataString(title)}" &
              $"&LatMin={latMin.ToString(CultureInfo.InvariantCulture)}" &
              $"&LatMax={latMax.ToString(CultureInfo.InvariantCulture)}" &
              $"&LongMin={lonMin.ToString(CultureInfo.InvariantCulture)}" &
              $"&LongMax={lonMax.ToString(CultureInfo.InvariantCulture)}" &
              $"&Epsilon={epsilon.ToString(CultureInfo.InvariantCulture)}"

        Using http As New Http.HttpClient()
            http.Timeout = TimeSpan.FromSeconds(15)
            Dim json = http.GetStringAsync(apiUrl).GetAwaiter().GetResult()
            Return JsonConvert.DeserializeObject(Of List(Of CandidateTask))(json)
        End Using

    End Function

    Private Function CallScriptToCreateWSGTaskPart1(taskData As Dictionary(Of String, Object)) As Integer
        If _useTestMode Then
            Throw New Exception("Test mode is active. WSG connection should not be possible!")
        End If
        Try
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.WaitCursor

            ' Serialize the task data to JSON
            Dim json As String = JsonConvert.SerializeObject(taskData, New JsonSerializerSettings() With {
            .NullValueHandling = NullValueHandling.Ignore
        })

            ' Prepare the request
            Dim request As HttpWebRequest = CType(WebRequest.Create($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}CreateNewTaskFromDPH.php"), HttpWebRequest)
            request.Method = "POST"

            ' Create boundary for multipart form-data
            Dim boundary As String = "----WebKitFormBoundary" & DateTime.Now.Ticks.ToString("x")
            request.ContentType = "multipart/form-data; boundary=" & boundary
            request.KeepAlive = True
            request.Credentials = CredentialCache.DefaultCredentials

            Using memStream As New MemoryStream()
                Dim boundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & vbCrLf)
                Dim endBoundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                memStream.Write(boundaryBytes, 0, boundaryBytes.Length)

                ' Add task data
                Dim taskDataHeader As String = "Content-Disposition: form-data; name=""task_data""" & vbCrLf & vbCrLf
                Dim taskDataBytes As Byte() = Encoding.UTF8.GetBytes(taskDataHeader & json)
                memStream.Write(taskDataBytes, 0, taskDataBytes.Length)
                memStream.Write(boundaryBytes, 0, boundaryBytes.Length)

                ' Add UserID
                Dim userIDHeader As String = "Content-Disposition: form-data; name=""UserID""" & vbCrLf & vbCrLf
                Dim userIDBytes As Byte() = Encoding.UTF8.GetBytes(userIDHeader & _userPermissionID)
                memStream.Write(userIDBytes, 0, userIDBytes.Length)
                memStream.Write(boundaryBytes, 0, boundaryBytes.Length)

                ' Write the final boundary
                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length)
                request.ContentLength = memStream.Length

                ' Send the request
                Using requestStream As Stream = request.GetRequestStream()
                    memStream.Position = 0
                    Dim tempBuffer As Byte() = New Byte(memStream.Length - 1) {}
                    memStream.Read(tempBuffer, 0, tempBuffer.Length)
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length)
                End Using
            End Using

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                If response.StatusCode = HttpStatusCode.OK Then
                    Using reader As New StreamReader(response.GetResponseStream())
                        Dim result As String = reader.ReadToEnd()
                        ' Parse the response JSON
                        Dim responseJson As JObject = JObject.Parse(result)

                        ' Check if the server returned success
                        If responseJson("status").ToString() = "success" Then
                            ' Return the EntrySeqID from the response
                            Return Convert.ToInt32(responseJson("EntrySeqID"))
                        Else
                            Throw New Exception($"Server error: {responseJson("message")}")
                        End If
                    End Using
                Else
                    Throw New Exception($"Server returned status code: {response.StatusCode}")
                End If
            End Using
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
        Catch ex As Exception
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
            ' Show error message
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error publishing task to WSG (Step 1): {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False ' Return False to indicate failure
        End Try
    End Function

    Private Function CreateWSGTaskPart2(Optional isUpdate As Boolean = False) As Boolean

        Dim taskInfo As AllData = SetAndRetrieveSessionData()

        Dim theMapImage As Byte() = Nothing
        Dim theCoverImage As Byte() = Nothing

        ' Cover and map image
        If taskInfo.MapImageSelected <> String.Empty Then
            theMapImage = ResizeImageAndGetBytes(taskInfo.MapImageSelected, 400, 400, 25)
        End If

        If taskInfo.CoverImageSelected <> String.Empty Then
            theCoverImage = ResizeImageAndGetBytes(taskInfo.CoverImageSelected, 400, 400, 25)
        End If

        ' Assume these values are computed or retrieved as part of the taskInfo
        Dim latitudeMin As Double
        Dim latitudeMax As Double
        Dim longitudeMin As Double
        Dim longitudeMax As Double
        _SF.GetTaskBoundaries(longitudeMin, longitudeMax, latitudeMin, latitudeMax)

        'Set RepostText
        Dim repostText As String = String.Empty
        If chkRepost.Checked Then
            If txtRepostOriginalURL.TextLength > 0 Then
                repostText = $"This task was originally posted on [{SupportingFeatures.ReturnDiscordServer(txtRepostOriginalURL.Text)}]({txtRepostOriginalURL.Text}) on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}"
            Else
                repostText = $"This task was originally posted on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}"
            End If
        End If

        'Set RecommendedAddOns list as JSON
        Dim recommendedAddOnsList As String = "[]" ' Default empty JSON array
        If taskInfo.RecommendedAddOns IsNot Nothing AndAlso taskInfo.RecommendedAddOns.Count > 0 Then
            Dim addOns = taskInfo.RecommendedAddOns.Select(Function(addOn) New With {
            Key .Name = addOn.Name,
            Key .URL = addOn.URL,
            Key .Type = addOn.Type
        }).ToList()
            recommendedAddOnsList = JsonConvert.SerializeObject(addOns)
        End If

        ' Create a new list to store filenames only
        Dim filenames As New List(Of String)
        ' Parse the ExtraFiles list and extract filenames
        If taskInfo.ExtraFiles IsNot Nothing Then
            For Each currentFilePath In taskInfo.ExtraFiles
                filenames.Add(IO.Path.GetFileName(currentFilePath)) ' Extract and add only the filename
            Next
        End If
        ' Serialize the filenames list into JSON
        Dim extraFilesList As String = JsonConvert.SerializeObject(filenames)

        Dim sharedWithList As String = JsonConvert.SerializeObject(chkcboSharedWithUsers.GetSelectedItems)

        Dim availabilityToUse As String = AvailabilityDateTimeToUse.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss")

        Dim secondaryWeatherFilename As String = taskInfo.WeatherFilenameSecondary
        Dim secondaryWeatherPresetName As String = _secondaryWeatherPresetName

        Dim modeRights As String = String.Empty
        If isUpdate Then
            modeRights = "UpdateTask"
        Else
            modeRights = "CreateTask"
        End If
        Dim taskData As New Dictionary(Of String, Object) From {
            {"RealTaskID", taskInfo.TaskID},
            {"EntrySeqID", taskInfo.EntrySeqID},
            {"Title", taskInfo.Title},
            {"LastUpdate", GetFileUpdateUTCDateTime(CurrentSessionFile).ToString("yyyy-MM-dd HH:mm:ss")},
            {"SimDateTime", SupportingFeatures.GetFullEventDateTimeInLocal(taskInfo.SimDate, taskInfo.SimTime, False)},
            {"IncludeYear", If(taskInfo.IncludeYear, 1, 0)},
            {"SimDateTimeExtraInfo", taskInfo.SimDateTimeExtraInfo},
            {"MainAreaPOI", taskInfo.MainAreaPOI},
            {"DepartureName", taskInfo.DepartureName},
            {"DepartureICAO", taskInfo.DepartureICAO},
            {"DepartureExtra", taskInfo.DepartureExtra},
            {"ArrivalName", taskInfo.ArrivalName},
            {"ArrivalICAO", taskInfo.ArrivalICAO},
            {"ArrivalExtra", taskInfo.ArrivalExtra},
            {"SoaringRidge", If(taskInfo.SoaringRidge, 1, 0)},
            {"SoaringThermals", If(taskInfo.SoaringThermals, 1, 0)},
            {"SoaringWaves", If(taskInfo.SoaringWaves, 1, 0)},
            {"SoaringDynamic", If(taskInfo.SoaringDynamic, 1, 0)},
            {"SoaringExtraInfo", taskInfo.SoaringExtraInfo},
            {"DurationMin", taskInfo.DurationMin},
            {"DurationMax", taskInfo.DurationMax},
            {"DurationExtraInfo", taskInfo.DurationExtraInfo},
            {"TaskDistance", CInt(_TaskTotalDistanceInKm)},
            {"TotalDistance", CInt(_FlightTotalDistanceInKm)},
            {"RecommendedGliders", taskInfo.RecommendedGliders},
            {"DifficultyRating", taskInfo.DifficultyRating},
            {"DifficultyExtraInfo", taskInfo.DifficultyExtraInfo},
            {"ShortDescription", taskInfo.ShortDescription},
            {"LongDescription", taskInfo.LongDescription},
            {"WeatherSummary", taskInfo.WeatherSummary},
            {"Credits", taskInfo.Credits},
            {"Countries", String.Join(", ", taskInfo.Countries.Select(Function(country) country.Replace(", ", " - ")))},
            {"RecommendedAddOns", If(taskInfo.RecommendedAddOns Is Nothing OrElse taskInfo.RecommendedAddOns.Count = 0, 0, 1)},
            {"RecommendedAddOnsList", recommendedAddOnsList},
            {"ExtraFilesList", extraFilesList},
            {"MapImage", theMapImage},
            {"CoverImage", theCoverImage},
            {"DBEntryUpdate", Now.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss")},
            {"LatMin", latitudeMin},
            {"LatMax", latitudeMax},
            {"LongMin", longitudeMin},
            {"LongMax", longitudeMax},
            {"PLNFilename", taskInfo.FlightPlanFilename},
            {"PLNXML", _XmlDocFlightPlan.InnerXml},
            {"WPRFilename", taskInfo.WeatherFilename},
            {"WPRXML", _XmlDocWeatherPreset.InnerXml},
            {"WPRSecondaryFilename", secondaryWeatherFilename},
            {"WPRSecondaryName", secondaryWeatherPresetName},
            {"RepostText", repostText},
            {"SuppressBaroPressureWarningSymbol", If(taskInfo.SuppressBaroPressureWarningSymbol, 1, 0)},
            {"BaroPressureExtraInfo", taskInfo.BaroPressureExtraInfo.Trim},
            {"LastUpdateDescription", txtLastUpdateDescription.Text.Trim},
            {"Status", taskInfo.TaskStatus},
            {"OwnerName", cboTaskOwner.Text.Trim},
            {"SharedWith", sharedWithList},
            {"Availability", availabilityToUse},
            {"DiscordPostID", _taskDiscordPostID},
            {"NormalPostContent", txtFPResults.Text},
            {"AvailabilityPostContent", txtFPResultsDelayedAvailability.Text},
            {"Mode", modeRights}
        }

        Dim filePath As String = taskInfo.DPHXPackageFilename

        'keep the TemporaryTaskID in case of an error
        Dim oldTemporaryTaskID As String = taskInfo.TemporaryTaskID
        txtTemporaryTaskID.Text = String.Empty
        taskInfo.TemporaryTaskID = String.Empty

        'One last save of the DPHX that will be published
        SaveSession()

        If Not _useTestMode Then
            Dim discordUpdateSuccess As Boolean = False
            Dim result As Boolean = CallScriptToCreateWSGTaskPart2(discordUpdateSuccess, taskData, filePath, taskInfo.CoverImageSelected)

            If result Then
                'Check if Discord was also a success
                If Not discordUpdateSuccess Then
                    'We want the user to possibly update his Discord message himself
                    ManualDiscordTaskPost()
                End If
            Else
                'Put back the pending values
                taskInfo.TaskStatus = SupportingFeatures.WSGTaskStatus.PendingCreation
                txtTemporaryTaskID.Text = oldTemporaryTaskID
                taskInfo.TemporaryTaskID = oldTemporaryTaskID
                SaveSession()
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Failed to upload the task.", "Upload Result", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
            Return result
        Else
            'Put back the not created
            taskInfo.TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
            txtTemporaryTaskID.Text = oldTemporaryTaskID
            taskInfo.TemporaryTaskID = oldTemporaryTaskID
            SaveSession()
            Return True
        End If

    End Function

    Private Function CallScriptToCreateWSGTaskPart2(ByRef discordUpdateSuccess As Boolean, task As Dictionary(Of String, Object), dphxFilePath As String, Optional coverImagePath As String = "") As Boolean
        If _useTestMode Then
            Throw New Exception("Test mode is active. WSG connection should not be possible!")
        End If
        Try
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.WaitCursor
            ' Serialize the task to JSON
            Dim json As String = JsonConvert.SerializeObject(task, New JsonSerializerSettings() With {
            .NullValueHandling = NullValueHandling.Ignore
        })

            ' Prepare the request
            Dim request As HttpWebRequest = CType(WebRequest.Create($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}CreateNewTaskFromDPH.php"), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "multipart/form-data"

            ' Create boundary
            Dim boundary As String = "----WebKitFormBoundary" & DateTime.Now.Ticks.ToString("x")

            request.ContentType = "multipart/form-data; boundary=" & boundary
            request.KeepAlive = True
            request.Credentials = CredentialCache.DefaultCredentials

            ' Get the weather graph image as BMP
            Dim bmp As Drawing.Image = CopyWeatherGraphToClipboard()

            ' Convert BMP to JPG with specified quality
            Dim weatherChartJpgFilePath As String = Path.Combine(Path.GetTempPath(), "weather_chart.jpg")
            SupportingFeatures.SaveJpegWithQuality(bmp, weatherChartJpgFilePath, 80L)

            ' Convert cover image to JPG (if needed)
            Dim convertedCoverImagePath As String = SupportingFeatures.ConvertImageToJpg(coverImagePath)

            Using memStream As New MemoryStream()
                Dim boundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & vbCrLf)
                Dim endBoundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                memStream.Write(boundaryBytes, 0, boundaryBytes.Length)

                ' Add task data
                Dim taskDataHeader As String = "Content-Disposition: form-data; name=""task_data""" & vbCrLf & vbCrLf
                Dim taskDataBytes As Byte() = Encoding.UTF8.GetBytes(taskDataHeader & json)
                memStream.Write(taskDataBytes, 0, taskDataBytes.Length)
                memStream.Write(boundaryBytes, 0, boundaryBytes.Length)

                ' Add user ID
                Dim userIDHeader As String = "Content-Disposition: form-data; name=""UserID""" & vbCrLf & vbCrLf
                Dim userIDBytes As Byte() = Encoding.UTF8.GetBytes(userIDHeader & _userPermissionID)
                memStream.Write(userIDBytes, 0, userIDBytes.Length)
                memStream.Write(boundaryBytes, 0, boundaryBytes.Length)

                ' Add DPHX file
                AddFileToRequest(memStream, dphxFilePath, "file", boundaryBytes)

                ' Add WeatherChart JPG file
                AddFileToRequest(memStream, weatherChartJpgFilePath, "image", boundaryBytes)

                ' Add Cover Image (if available)
                If Not String.IsNullOrEmpty(convertedCoverImagePath) AndAlso File.Exists(convertedCoverImagePath) Then
                    AddFileToRequest(memStream, convertedCoverImagePath, "cover", boundaryBytes)
                End If

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length)

                request.ContentLength = memStream.Length

                Using requestStream As Stream = request.GetRequestStream()
                    memStream.Position = 0
                    Dim tempBuffer As Byte() = New Byte(memStream.Length - 1) {}
                    memStream.Read(tempBuffer, 0, tempBuffer.Length)
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length)
                End Using
            End Using

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim result As String = reader.ReadToEnd()
                    ' Assuming the server returns a JSON object with a "status" field
                    Dim responseJson As JObject = JObject.Parse(result)
                    If responseJson("discordError").ToString = String.Empty Then
                        discordUpdateSuccess = True
                    End If
                    Return responseJson("status").ToString() = "success"
                End Using
            End Using
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default

        Catch ex As Exception
            Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error uploading task: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Private Sub AddFileToRequest(memStream As MemoryStream, filePath As String, fieldName As String, boundaryBytes As Byte())
        Dim header As String = $"Content-Disposition: form-data; name=""{fieldName}""; filename=""{Path.GetFileName(filePath)}""" & vbCrLf & "Content-Type: application/octet-stream" & vbCrLf & vbCrLf
        Dim headerBytes As Byte() = Encoding.UTF8.GetBytes(header)
        memStream.Write(headerBytes, 0, headerBytes.Length)

        Using fileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
            Dim buffer As Byte() = New Byte(1023) {}
            Dim bytesRead As Integer = fileStream.Read(buffer, 0, buffer.Length)

            While bytesRead <> 0
                memStream.Write(buffer, 0, bytesRead)
                bytesRead = fileStream.Read(buffer, 0, buffer.Length)
            End While
        End Using

        memStream.Write(boundaryBytes, 0, boundaryBytes.Length)
    End Sub


    Private Sub GetTaskDetails(taskID As String, entrySeqID As Integer)
        Try

            Dim taskDetailsUrl As String = String.Empty
            Dim request As HttpWebRequest = Nothing

            If entrySeqID > 0 Then
                'Use EntrySeqID
                taskDetailsUrl = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingEntrySeqID.php"
                request = CType(WebRequest.Create(taskDetailsUrl & "?EntrySeqID=" & entrySeqID.ToString), HttpWebRequest)
            Else
                taskDetailsUrl = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingID.php"
                request = CType(WebRequest.Create(taskDetailsUrl & "?TaskID=" & taskID), HttpWebRequest)
            End If

            ' Create the web request
            request.Method = "GET"
            request.ContentType = "application/json"

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    ' Check the status
                    If result("status").ToString() = "success" Then
                        If entrySeqID = 0 Then
                            _TaskEntrySeqID = result("taskDetails")("EntrySeqID")
                            _taskDiscordPostID = String.Empty
                        Else
                            _taskDiscordPostID = result("taskDetails")("DiscordPostID").ToString()
                        End If
                        Integer.TryParse(result("taskDetails")("Status"), _TaskStatus)
                        ' Specify the format of the datetime string
                        Dim utcFormat As String = "yyyy-MM-dd HH:mm:ss"
                        Dim cultureInfo As CultureInfo = CultureInfo.InvariantCulture
                        ' Convert UTC datetime to local time before assigning to variables
                        _TBTaskDBEntryUpdate = DateTime.ParseExact(result("taskDetails")("DBEntryUpdate").ToString(), utcFormat, cultureInfo, DateTimeStyles.AssumeUniversal).ToLocalTime()
                        _TBTaskLastUpdate = DateTime.ParseExact(result("taskDetails")("LastUpdate").ToString(), utcFormat, cultureInfo, DateTimeStyles.AssumeUniversal).ToLocalTime()
                        ' Retrieve OwnerName and SharedWith info
                        _currentWSGTaskOwner = result("taskDetails")("OwnerName").ToString()
                        cboTaskOwner.SelectedItem = _currentWSGTaskOwner
                        Boolean.TryParse(result("taskDetails")("HasLeaderBoard"), _hasLeaderBoard)

                        ' Check if SharedWith is a JSON string that represents an array
                        Dim sharedWithRaw As String = result("taskDetails")("SharedWith").ToString()
                        If sharedWithRaw.StartsWith("[") AndAlso sharedWithRaw.EndsWith("]") Then
                            ' Parse the string as a JSON array and convert it to a list of strings
                            _currentWSGSharedWith = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of String))(sharedWithRaw)
                        Else
                            ' If it's not a JSON array, treat it as a plain string
                            _currentWSGSharedWith.Add(sharedWithRaw)
                        End If
                        ' Pass the list to the SelectItemsByNames method
                        ' If the Owner is not the user but the user is in the shared list, we must add him to the choices
                        If _currentWSGTaskOwner <> _userName AndAlso _currentWSGSharedWith.Contains(_userName) Then
                            chkcboSharedWithUsers.AddItem(_userName, True)
                        End If
                        chkcboSharedWithUsers.SelectItemsByNames(_currentWSGSharedWith)
                        _currentWSGSharedWith.Sort()

                        ' If Owner is the same as current user, then enable changing the owner. If not, then it must be locked.
                        If cboTaskOwner.Text = _userName OrElse _isSuperUser Then
                            cboTaskOwner.Enabled = True
                            chkcboSharedWithUsers.IsReadOnly = False
                        Else
                            cboTaskOwner.Enabled = False
                            chkcboSharedWithUsers.IsReadOnly = True
                        End If

                    Else
                        _TaskEntrySeqID = 0
                        Throw New Exception("Error retrieving task details: " & result("message").ToString())
                    End If
                End Using
            End Using

            SetTBTaskDetailsLabel()

        Catch ex As Exception
            Throw New Exception("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub SetTBTaskDetailsLabel()

        Dim labelString As String = String.Empty
        Dim dateFormat As String = "yyyy-MM-dd HH:mm:ss"

        txtLastUpdateDescription.Enabled = False
        btnDeleteFromTaskBrowser.Enabled = False
        grbEventsFeaturingTask.Enabled = False
        btnStartTaskPost.Enabled = False

        If _TaskEntrySeqID > 0 Then
            'Verify if local DPH has been changed
            btnStartTaskPost.Text = "Start Task Update Workflow"
            Dim localDPHTime As DateTime = GetFileUpdateUTCDateTime(CurrentSessionFile, False)
            Dim statusMsg As String = String.Empty
            Select Case _TaskStatus
                Case SupportingFeatures.WSGTaskStatus.Active
                    'Nothing
                Case SupportingFeatures.WSGTaskStatus.Inactive
                    statusMsg = "⚠️Inactive: "
                Case SupportingFeatures.WSGTaskStatus.NotCreated
                    'Not possible
                Case SupportingFeatures.WSGTaskStatus.PendingCreation
                    btnStartTaskPost.Text = "Resume Task Creation Workflow"
                    statusMsg = "⚠️Incomplete: "
            End Select
            labelString = $"{statusMsg}#{_TaskEntrySeqID.ToString} - Online ({_TBTaskLastUpdate}) - Local ({localDPHTime})"
            Select Case _TaskStatus
                Case SupportingFeatures.WSGTaskStatus.Active, SupportingFeatures.WSGTaskStatus.Inactive
                    Dim timeDifference As TimeSpan = _TBTaskLastUpdate.Subtract(localDPHTime)
                    If timeDifference.TotalSeconds < -2 Then
                        'Local file is more recent (by more than 2 seconds) - allow change
                        If UserCanUpdateTask Then
                            btnStartTaskPost.Enabled = True
                            txtLastUpdateDescription.Enabled = True
                        Else
                            btnStartTaskPost.Enabled = False
                            txtLastUpdateDescription.Enabled = False
                        End If
                        lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(255, 128, 0)
                    Else
                        lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(0, 192, 0)
                    End If
                Case SupportingFeatures.WSGTaskStatus.PendingCreation
                    If UserCanCreateTask Then
                        btnStartTaskPost.Enabled = True
                    Else
                        btnStartTaskPost.Enabled = False
                    End If
                    lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(255, 128, 0)
            End Select
            tabFlightPlan.Enabled = IsOwnerOrShared
            lblWSGTaskEntrySeqID.Text = $"✅ WeSimGlide.org ID: {_TaskEntrySeqID}"

            btnDeleteFromTaskBrowser.Enabled = UserCanDeleteTask AndAlso IsOwnerOrShared
            grbEventsFeaturingTask.Enabled = True
            toolStripClone.Visible = True
        Else
            lblPrePublishingWarning.Visible = False
            lblWSGTaskEntrySeqID.Text = $"❌ WeSimGlide.org ID: None"
            btnStartTaskPost.Text = "Start Task Creation Workflow"
            labelString = "Task does not exist on WeSimGlide.org"
            lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(255, 128, 0)
            If UserCanCreateTask OrElse _useTestMode Then
                btnStartTaskPost.Enabled = grbTaskInfo.Enabled
            Else
                btnStartTaskPost.Enabled = False
            End If
            toolStripClone.Visible = False
        End If

        If _taskDiscordPostID = String.Empty Then
            lblDiscordTaskPostID.Text = "❌ Discord Post ID"
        Else
            lblDiscordTaskPostID.Text = "✅ Discord Post ID"
        End If

        lblTaskBrowserIDAndDate.Text = labelString

        'Check if task has a leader board and set lblPrePublishingWarning accordingly
        lblPrePublishingWarning.Visible = _hasLeaderBoard AndAlso IsOwnerOrShared()

    End Sub

#End Region

#Region "Events Subs"

    Public Function PublishEventNews(key As String,
                                 title As String,
                                 subtitle As String,
                                 comments As String,
                                 eventDate As DateTime,
                                 published As DateTime,
                                 entrySeqID As Integer,
                                 urlToGo As String,
                                 expiration As DateTime,
                                 trackerGroup As String,
                                 eventMeetDateTime As DateTime,
                                 useEventSyncFly As Boolean,
                                 syncFlyDateTime As DateTime,
                                 useEventLaunch As Boolean,
                                 eventLaunchDateTime As DateTime,
                                 useEventStartTask As Boolean,
                                 eventStartTaskDateTime As DateTime,
                                 eventDescription As String,
                                 groupEventTeaserEnabled As Boolean,
                                 groupEventTeaserMessage As String,
                                 groupEventTeaserImagePath As String,
                                 voiceChannel As String,
                                 msfsServer As String,
                                 eligibleAward As String,
                                 beginnersGuide As String,
                                 notam As String,
                                 availability As String,
                                 refly As Integer,
                                 wsgAnnouncement As String) As Boolean

        If _useTestMode Then
            Throw New Exception("Test mode is active. WSG connection should not be possible!")
        End If

        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}ManageNewsWithRights.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "POST"

        ' Create boundary for multipart/form-data
        Dim boundary As String = "----WebKitFormBoundary" & DateTime.Now.Ticks.ToString("x")
        request.ContentType = "multipart/form-data; boundary=" & boundary

        Try
            Using memStream As New MemoryStream()
                Dim boundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & vbCrLf)
                Dim endBoundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Add form fields
                Dim fields As New Dictionary(Of String, String) From {
                {"action", "CreateEvent"},
                {"Key", key},
                {"Credits", trackerGroup},
                {"Title", title},
                {"Subtitle", subtitle},
                {"Comments", comments},
                {"EventDate", eventDate.ToString("yyyy-MM-dd HH:mm:ss")},
                {"Published", published.ToString("yyyy-MM-dd HH:mm:ss")},
                {"EntrySeqID", entrySeqID.ToString()},
                {"URLToGo", urlToGo},
                {"Expiration", expiration.ToString("yyyy-MM-dd HH:mm:ss")},
                {"EventMeetDateTime", eventMeetDateTime.ToString("yyyy-MM-dd HH:mm:ss")},
                {"UseEventSyncFly", If(useEventSyncFly, "1", "0")},
                {"SyncFlyDateTime", syncFlyDateTime.ToString("yyyy-MM-dd HH:mm:ss")},
                {"UseEventLaunch", If(useEventLaunch, "1", "0")},
                {"EventLaunchDateTime", eventLaunchDateTime.ToString("yyyy-MM-dd HH:mm:ss")},
                {"UseEventStartTask", If(useEventStartTask, "1", "0")},
                {"EventStartTaskDateTime", eventStartTaskDateTime.ToString("yyyy-MM-dd HH:mm:ss")},
                {"EventDescription", eventDescription},
                {"GroupEventTeaserEnabled", If(groupEventTeaserEnabled, "1", "0")},
                {"GroupEventTeaserMessage", groupEventTeaserMessage},
                {"VoiceChannel", voiceChannel},
                {"TrackerGroup", trackerGroup},
                {"MSFSServer", msfsServer},
                {"EligibleAward", eligibleAward},
                {"BeginnersGuide", beginnersGuide},
                {"Notam", notam},
                {"UserID", _userPermissionID},
                {"Availability", availability},
                {"Refly", refly},
                {"WSGAnnouncement", wsgAnnouncement},
                {"PastEventOnly", If(_pastEventConfirmed, "true", "false")}
            }

                For Each field In fields
                    memStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                    Dim fieldData As String = $"Content-Disposition: form-data; name=""{field.Key}""" & vbCrLf & vbCrLf & field.Value
                    Dim fieldBytes As Byte() = Encoding.UTF8.GetBytes(fieldData)
                    memStream.Write(fieldBytes, 0, fieldBytes.Length)
                Next

                ' Add the teaser image file, if present
                If Not String.IsNullOrEmpty(groupEventTeaserImagePath) AndAlso IO.File.Exists(groupEventTeaserImagePath) Then
                    memStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                    Dim header As String = $"Content-Disposition: form-data; name=""GroupEventTeaserImage""; filename=""{Path.GetFileName(groupEventTeaserImagePath)}""" & vbCrLf &
                                       "Content-Type: image/jpeg" & vbCrLf & vbCrLf
                    Dim headerBytes As Byte() = Encoding.UTF8.GetBytes(header)
                    memStream.Write(headerBytes, 0, headerBytes.Length)

                    Dim imageBytes As Byte() = ResizeImageAndGetBytes(groupEventTeaserImagePath, 1024, 768, 85, True)
                    Debug.WriteLine($"Image size (bytes): {imageBytes.Length}")
                    memStream.Write(imageBytes, 0, imageBytes.Length)
                End If

                ' Write the end boundary
                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length)
                request.ContentLength = memStream.Length

                ' Send the request
                Using requestStream As Stream = request.GetRequestStream()
                    memStream.Position = 0
                    memStream.CopyTo(requestStream)
                End Using
            End Using

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    ' Parse response
                    Dim result As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
                    Return result("status").ToString() = "success"
                End Using
            End Using
        Catch ex As Exception
            ' Handle exceptions
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error publishing event news: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Public Function DeleteEventNews(key As String) As Boolean
        If _useTestMode Then
            Throw New Exception("Test mode is active. WSG connection should not be possible!")
        End If
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}ManageNewsWithRights.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "POST"

        ' Create boundary for multipart/form-data
        Dim boundary As String = "----WebKitFormBoundary" & DateTime.Now.Ticks.ToString("x")
        request.ContentType = "multipart/form-data; boundary=" & boundary

        Try
            Using memStream As New MemoryStream()
                Dim boundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & vbCrLf)
                Dim endBoundaryBytes As Byte() = Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)

                ' Add form fields
                Dim fields As New Dictionary(Of String, String) From {
                {"action", "DeleteEvent"},
                {"Key", key},
                {"UserID", _userPermissionID}
            }

                For Each field In fields
                    memStream.Write(boundaryBytes, 0, boundaryBytes.Length)
                    Dim fieldData As String = $"Content-Disposition: form-data; name=""{field.Key}""" & vbCrLf & vbCrLf & field.Value
                    Dim fieldBytes As Byte() = Encoding.UTF8.GetBytes(fieldData)
                    memStream.Write(fieldBytes, 0, fieldBytes.Length)
                Next

                ' Write the end boundary
                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length)
                request.ContentLength = memStream.Length

                ' Send the request
                Using requestStream As Stream = request.GetRequestStream()
                    memStream.Position = 0
                    memStream.CopyTo(requestStream)
                End Using
            End Using

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    ' Parse response
                    Dim result As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
                    If result("status").ToString = "success" Then
                        Return True
                    Else
                        Throw New Exception(result("message".ToString))
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle exceptions
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error deleting event news: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Private Function VerifyNewsExists(key As String) As String

        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}VerifyNewsExists.php?key={key}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()

                    ' Parse the JSON response to extract the status
                    Dim json As JObject = JObject.Parse(jsonResponse)

                    ' Check if the status is "success" or "notfound"
                    If json("status").ToString() = "success" Then
                        Return "success" ' Key exists
                    ElseIf json("status").ToString() = "notfound" Then
                        Return "notfound" ' Key does not exist
                    Else
                        ' Unexpected response
                        Return "error"
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception (log the error, show a message, etc.)
            ' Return "error" if an exception occurs
            Return "error"
        End Try
    End Function

    Private Sub cboTaskOwner_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTaskOwner.SelectedIndexChanged
        If _userInteraction Then
            ' Remove the new selected owner from the checked list
            If cboTaskOwner.Text <> String.Empty Then
                chkcboSharedWithUsers.RemoveItem(cboTaskOwner.Text)
            End If

            ' Add the previous owner back to the checked list if it was valid
            If Not String.IsNullOrEmpty(_previousOwner) AndAlso _previousOwner <> cboTaskOwner.Text Then
                chkcboSharedWithUsers.AddItem(_previousOwner, True)
            End If
        End If

        ' Update the previous owner
        _previousOwner = cboTaskOwner.Text

        CheckOwnerOrUsersHaveChanged()

    End Sub

    Private Sub cboTaskOwner_Enter(sender As Object, e As EventArgs) Handles cboTaskOwner.Enter
        _userInteraction = True
        _previousOwner = cboTaskOwner.Text
    End Sub

    Private Sub cboTaskOwner_Leave(sender As Object, e As EventArgs) Handles cboTaskOwner.Leave
        _userInteraction = False
    End Sub

    Private Sub CheckOwnerOrUsersHaveChanged()
        If _isInitiatizing OrElse _loadingFile Then
            Exit Sub
        End If
        If (Not _discordModified) AndAlso cboTaskOwner.Text <> _currentWSGTaskOwner OrElse (Not AreListsEqual(chkcboSharedWithUsers.GetSelectedItems, _currentWSGSharedWith)) Then
            SessionModified(SourceOfChange.DiscordTab)
        End If
    End Sub

    Public Function AreListsEqual(list1 As List(Of String), list2 As List(Of String)) As Boolean
        ' Check if both lists are the same size
        If list1.Count <> list2.Count Then
            Return False
        End If

        ' Sort and compare the lists
        Return list1.OrderBy(Function(x) x).SequenceEqual(list2.OrderBy(Function(x) x))
    End Function

    Private Sub chkcboSharedWithUsers_SelectedItemsChanged(sender As Object, e As EventArgs) Handles chkcboSharedWithUsers.SelectedItemsChanged
        CheckOwnerOrUsersHaveChanged()
    End Sub

    Private Sub btnStartTaskPost_EnabledChanged(sender As Object, e As EventArgs) Handles btnStartTaskPost.EnabledChanged
        If btnStartTaskPost.Enabled Then
            btnStartTaskPost.Enabled = IsOwnerOrShared
            btnStartFullPostingWorkflow.Enabled = IsOwnerOrShared AndAlso IsClubAuthorizedEventOrganizer
        End If
    End Sub

    Private Sub PrepareDeleteEventNews()
        If UserCanDeleteEvent Then
            Dim key As String = WSGEventID
            Dim result As Boolean = DeleteEventNews(key)
            If result Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("The event news was deleted.", "Deleting event news entry", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End If
            SetEventLabelAndButtons()
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Sorry, you are not allowed to delete an event news.", "Deleting event news entry", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Function GetEventAndTaskWSGAccouncement(eventDate As Date, key As String) As String

        If Not (chkDGPOPublishWSGEventNews.Enabled AndAlso chkDGPOPublishWSGEventNews.Checked) Then
            Return String.Empty
        End If

        Dim roleTaskBrowserID As String = "@&1168038746772471828"
        Dim roleEventHunterID As String = "@&1267461823234441298"

        If SupportingFeatures.useTestServer Then
            roleTaskBrowserID = "TaskBrowsers"
            roleEventHunterID = "EventHunters"
        End If

        Dim msgForEventHunters As String = String.Empty
        Dim groupEmoji As String = String.Empty
        If _SF.DiscordNameIDPair.ContainsKey(lblGroupEmoji.Text.Trim) Then
            groupEmoji = $"<{lblGroupEmoji.Text}{_SF.DiscordNameIDPair(lblGroupEmoji.Text.Trim)}> "
        End If
        If _TaskEntrySeqID > 0 AndAlso (Not IsDelayedAvailability) Then
            msgForEventHunters = $"<{roleTaskBrowserID}> <{roleEventHunterID}> {Environment.NewLine}# {groupEmoji}{_SF.GetDiscordTimeStampForDate(eventDate, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)}{Environment.NewLine}## [{txtClubFullName.Text.Trim} - {txtEventTitle.Text.Trim}](<{SupportingFeatures.GetWeSimGlideEventURL(key)}>){Environment.NewLine}### {WSGEmojiForBot} [Task #{_TaskEntrySeqID.ToString.Trim}](<{SupportingFeatures.GetWeSimGlideTaskURL(_TaskEntrySeqID)}>)"
        Else
            Dim taskUnavailableMsg As String = String.Empty
            If _TaskEntrySeqID > 0 Then
                taskUnavailableMsg = "Please monitor the event as task has been set for later availability."
            Else
                taskUnavailableMsg = "Please monitor the event as task has not been published yet."
            End If
            msgForEventHunters = $"<{roleEventHunterID}> {Environment.NewLine}# {groupEmoji}{_SF.GetDiscordTimeStampForDate(eventDate, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)}{Environment.NewLine}## [{txtClubFullName.Text.Trim} - {txtEventTitle.Text.Trim}](<{SupportingFeatures.GetWeSimGlideEventURL(key)}>){Environment.NewLine}### {taskUnavailableMsg}"
        End If
        Return msgForEventHunters

    End Function

    Private ReadOnly Property WSGEventLink As String
        Get
            If _ClubPreset Is Nothing Then
                Return String.Empty
            End If
            Dim key As String = WSGEventID
            Return SupportingFeatures.GetWeSimGlideEventURL(key)
        End Get
    End Property

    Private Function PublishEventNews() As Boolean

        If UserCanCreateEvent Then
            Dim key As String = WSGEventID
            Dim eventDate As Date
            Dim comments As String = String.Empty

            eventDate = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, chkDateTimeUTC.Checked)
            eventDate = eventDate.ToUniversalTime

            Dim eventSyncDate As Date
            Dim eventLaunchDate As Date
            Dim eventStartDate As Date
            eventSyncDate = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventSyncFlyDate.Value, dtEventSyncFlyTime.Value, chkDateTimeUTC.Checked)
            eventSyncDate = eventSyncDate.ToUniversalTime
            eventLaunchDate = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventLaunchDate.Value, dtEventLaunchTime.Value, chkDateTimeUTC.Checked)
            eventLaunchDate = eventLaunchDate.ToUniversalTime
            eventStartDate = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventStartTaskDate.Value, dtEventStartTaskTime.Value, chkDateTimeUTC.Checked)
            eventStartDate = eventStartDate.ToUniversalTime

            If chkEventTeaser.Checked AndAlso txtEventTeaserMessage.Text.Trim <> String.Empty Then
                comments = txtEventTeaserMessage.Text.Trim
            ElseIf txtEventDescription.Text.Trim <> String.Empty Then
                comments = txtEventDescription.Text.Trim
            Else
                comments = txtShortDescription.Text.Trim
            End If

            Dim urlBeginnerGuide As String = String.Empty
            Select Case cboBeginnersGuide.Text
                Case "Other (provide link below)"
                    If SupportingFeatures.IsValidURL(txtOtherBeginnerLink.Text.Trim) Then
                        urlBeginnerGuide = $"[Link to custom guide]({txtOtherBeginnerLink.Text.Trim})"
                    End If
                Case "The Beginner's Guide to Soaring Events (GotGravel)"
                    urlBeginnerGuide = "[The Beginner's Guide to Soaring Events (GotGravel)](https://discord.com/channels/793376245915189268/1097520643580362753/1097520937701736529)"
                Case "How to join our Group Flights (Sim Soaring Club)"
                    urlBeginnerGuide = "[How to join our Group Flights (Sim Soaring Club)](https://discord.com/channels/876123356385149009/1038819881396744285)"
                Case Else
            End Select

            Dim notam As String = String.Empty

            'Complete notam with notices like barometric pressure and altitude restrictions
            If txtAltRestrictions.Text.Trim.Length > 33 Then
                notam = $"This task contains altitude restrictions!<br>"
            End If
            If txtBaroPressureExtraInfo.Enabled AndAlso txtBaroPressureExtraInfo.Text.Trim <> String.Empty Then
                notam = $"{notam}{txtBaroPressureExtraInfo.Text.Trim}<br>"
            End If
            If notam <> String.Empty Then
                notam = $"<strong>NOTAMs:</strong><br>{notam}"
            End If

            Dim availabilityToUse As String = AvailabilityDateTimeToUse.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss")

            Dim refly As Integer = If(IsDelayedAvailabilityEnabledAndChecked() AndAlso chkAvailabilityRefly.Checked, 1, 0)

            Dim result As Boolean = PublishEventNews(key,
                                                    txtClubFullName.Text.Trim,
                                                    txtEventTitle.Text.Trim,
                                                    comments,
                                                    eventDate,
                                                    Now.ToUniversalTime,
                                                    _TaskEntrySeqID,
                                                    txtGroupEventPostURL.Text.Trim,
                                                    eventDate.AddHours(3),
                                                    txtTrackerGroup.Text,
                                                    eventDate,
                                                    chkUseSyncFly.Checked,
                                                    eventSyncDate,
                                                    chkUseLaunch.Checked,
                                                    eventLaunchDate,
                                                    chkUseStart.Checked,
                                                    eventStartDate,
                                                    txtEventDescription.Text.Trim,
                                                    chkEventTeaser.Checked,
                                                    txtEventTeaserMessage.Text.Trim,
                                                    txtEventTeaserAreaMapImage.Text.Trim,
                                                    cboVoiceChannel.Text.Trim,
                                                    cboMSFSServer.Text.Trim,
                                                    cboEligibleAward.Text.Trim,
                                                    urlBeginnerGuide,
                                                    notam,
                                                    availabilityToUse,
                                                    refly,
                                                    GetEventAndTaskWSGAccouncement(eventDate, key))

            If result Then
                SetEventLabelAndButtons()
            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Failed to publish the event news entry.", "Publishing event news entry", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If

            Return result

        End If

    End Function

    Private Sub btnDeleteEventNews_Click(sender As Object, e As EventArgs) Handles btnDeleteEventNews.Click
        PrepareDeleteEventNews()
    End Sub

    Private Sub chkWSGTask_CheckedChanged(sender As Object, e As EventArgs) Handles chkWSGTask.CheckedChanged
        If chkWSGTask.Enabled Then
            chkWSGTask.Checked = True
        End If
    End Sub

    Private Sub chkWSGEvent_CheckedChanged(sender As Object, e As EventArgs) Handles chkWSGEvent.CheckedChanged
        If chkWSGEvent.Enabled Then
            chkWSGEvent.Checked = True
        End If
    End Sub

    Private Sub chkTestMode_CheckedChanged(sender As Object, e As EventArgs) Handles chkTestMode.CheckedChanged
        If _userPermissionID = String.Empty Then
            chkTestMode.Checked = True
        End If
        If _useTestMode AndAlso Not chkTestMode.Checked Then
            'Going from test mode to real mode - the group event should be emptied if not
            If txtGroupEventPostURL.Text.Trim.Length > 0 Then
                txtGroupEventPostURL.Text = String.Empty
            End If
            If _taskDiscordPostID.Length > 0 Then
                'Try to delete the Discord post
                PostTaskToTestChannel(String.Empty, _taskDiscordPostID, True, _taskDiscordPostID)
            End If
            _taskDiscordPostID = String.Empty
            SetTBTaskDetailsLabel()
        ElseIf Not _useTestMode AndAlso chkTestMode.Checked Then
            'Going from real mode to test mode - reset WSG event and task IDs
            Dim continueSwitch As Boolean = True
            If _TaskEntrySeqID > 0 Then
                'If there was a task, we should warn the user that it will be lost
                continueSwitch = False
                Using New Centered_MessageBox(Me)
                    continueSwitch = (MessageBox.Show("Switching to test mode will reset the WeSimGlide.org task ID and status. You will need to create a new task in test mode.", "Switching to test mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = vbYes)
                End Using
            End If
            If continueSwitch Then
                _TaskEntrySeqID = 0
                _TaskStatus = SupportingFeatures.WSGTaskStatus.NotCreated
                _taskDiscordPostID = String.Empty
                txtTemporaryTaskID.Text = String.Empty
                txtTaskID.Text = String.Empty
                SetTBTaskDetailsLabel()
            Else
                chkTestMode.Checked = False
            End If
        End If
        ToggleTestMode(chkTestMode.Checked)
        CheckTestModes()
        SetEventLabelAndButtons()
    End Sub

    Private Sub chkDelayedAvailability_CheckedChanged(sender As Object, e As EventArgs) Handles chkDelayedAvailability.CheckedChanged

        If _isInitiatizing Then
            Exit Sub
        End If

        grbDelayedPosting.Enabled = IsDelayedAvailabilityEnabledAndChecked()

        If IsDelayedAvailabilityEnabledAndChecked() Then
            chkDelayBasedOnEvent.Enabled = chkActivateEvent.Checked
            CalculateTaskAvailability()
        End If

        SessionModified(SourceOfChange.DiscordTab)

    End Sub

    Private Sub chkDelayBasedOnEvent_CheckedChanged(sender As Object, e As EventArgs) Handles chkDelayBasedOnEvent.CheckedChanged

        pnlDelayBasedOnGroupEvent.Enabled = chkDelayBasedOnEvent.Checked

        If chkDelayBasedOnEvent.Checked Then
            CalculateTaskAvailability()
        End If

        SessionModified(SourceOfChange.DiscordTab)

    End Sub

    Private Sub CalculateTaskAvailability()

        If _isInitiatizing Then
            Exit Sub
        End If

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)
        Try
            If chkActivateEvent.Checked AndAlso fullMeetDateTimeLocal > Now Then
                _directChange = False
                dtAvailabilityDate.MaxDate = fullMeetDateTimeLocal
                dtAvailabilityTime.MaxDate = fullMeetDateTimeLocal
                _directChange = True
            Else
                _directChange = False
                dtAvailabilityDate.MaxDate = "9998-12-31"
                dtAvailabilityTime.MaxDate = "9998-12-31"
                _directChange = True
            End If
        Catch ex As Exception
        End Try

        If (Not chkDelayBasedOnEvent.Checked) OrElse _loadingFile OrElse _isInitiatizing OrElse txtMinutesBeforeMeeting.Text.Trim = String.Empty Then
            SetDelayTimeDifferenceField()
            Exit Sub
        End If

        Dim nbrMinutesToSubstract As Integer

        nbrMinutesToSubstract = CInt(txtMinutesBeforeMeeting.Text.Trim)

        If cboDelayUnits.SelectedIndex = 0 Then
            nbrMinutesToSubstract = nbrMinutesToSubstract * 60
        End If

        Dim availabilityDateTimeLocal As DateTime = fullMeetDateTimeLocal.AddMinutes(0 - nbrMinutesToSubstract)

        lblBeforeMeetingTime.Text = "before meeting time."

        _directChange = False
        dtAvailabilityDate.Value = availabilityDateTimeLocal
        dtAvailabilityTime.Value = availabilityDateTimeLocal
        _directChange = True

    End Sub

    Private Sub txtMinutesBeforeMeeting_Leave(sender As Object, e As EventArgs) Handles txtMinutesBeforeMeeting.Leave
        If txtMinutesBeforeMeeting.Text.Trim = String.Empty Then
            txtMinutesBeforeMeeting.Text = "0"
        End If
        CalculateTaskAvailability()
        GeneralFPTabFieldLeaveDetection(sender, e)
    End Sub

    Private Sub cboDelayUnits_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDelayUnits.SelectedIndexChanged
        If _loadingFile OrElse _isInitiatizing Then
            Exit Sub
        End If
        CalculateTaskAvailability()
        SessionModified(SourceOfChange.DiscordTab)
    End Sub

    Private Sub dtAvailability_ValueChanged(sender As Object, e As EventArgs) Handles dtAvailabilityDate.ValueChanged, dtAvailabilityTime.ValueChanged

        If _loadingFile OrElse _isInitiatizing OrElse (Not _directChange) Then
            Exit Sub
        End If

        If DirectCast(sender, DateTimePicker).Name = "dtAvailabilityDate" Then
            'Set the time field's date to this one
            Dim combinedDate As DateTime = _SF.GetFullEventDateTimeInLocal(dtAvailabilityDate, dtAvailabilityTime, False)
            dtAvailabilityTime.Value = combinedDate
        End If

        SetDelayTimeDifferenceField()
        SessionModified(SourceOfChange.DiscordTab)

    End Sub

#End Region

    Private Sub SetDelayTimeDifferenceField()
        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)

        If chkActivateEvent.Checked Then
            Dim availabilityDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtAvailabilityDate, dtAvailabilityTime, False)
            Dim nbrMinutesDiff As Integer = DateDiff(DateInterval.Minute, fullMeetDateTimeLocal, availabilityDateTimeLocal)

            'If nbrMinutesDiff > 0 Then
            '    dtAvailabilityDate.Value = fullMeetDateTimeLocal
            '    dtAvailabilityTime.Value = fullMeetDateTimeLocal
            '    nbrMinutesDiff = 0
            'End If

            ' Determine delay unit based on the number of minutes difference
            If Math.Abs(nbrMinutesDiff) >= 60 AndAlso nbrMinutesDiff Mod 60 = 0 Then
                cboDelayUnits.SelectedIndex = 0 ' Hours
                txtMinutesBeforeMeeting.Text = Math.Abs(nbrMinutesDiff / 60).ToString()
            Else
                cboDelayUnits.SelectedIndex = 1 ' Minutes
                txtMinutesBeforeMeeting.Text = Math.Abs(nbrMinutesDiff).ToString()
            End If
            If chkDelayBasedOnEvent.Checked Then
                ' Display appropriate warning if the availability time is after the meeting time
                If nbrMinutesDiff <= 0 Then
                    lblBeforeMeetingTime.Text = "before meeting time."
                Else
                    lblBeforeMeetingTime.Text = "⚠️ AFTER meeting time."
                    dtAvailabilityTime.Value = fullMeetDateTimeLocal
                    dtAvailabilityDate.Value = fullMeetDateTimeLocal
                End If
            End If
        End If
    End Sub

    Private ReadOnly Property AvailabilityDateTimeToUse As DateTime
        Get
            Dim availabilityDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtAvailabilityDate, dtAvailabilityTime, False)
            If Not (IsDelayedAvailabilityEnabledAndChecked() AndAlso availabilityDateTimeLocal > Now()) Then
                availabilityDateTimeLocal = Now()
            End If
            Return availabilityDateTimeLocal
        End Get
    End Property

    Private Sub chkDelayBasedOnEvent_EnabledChanged(sender As Object, e As EventArgs) Handles chkDelayBasedOnEvent.EnabledChanged
        chkAvailabilityRefly.Enabled = chkDelayBasedOnEvent.Enabled
    End Sub

    Private Sub chkAvailabilityRefly_CheckedChanged(sender As Object, e As EventArgs) Handles chkAvailabilityRefly.CheckedChanged
        SessionModified(SourceOfChange.DiscordTab)
    End Sub

    Private Sub lblDiscordTaskPostID_TextChanged(sender As Object, e As EventArgs) Handles lblDiscordTaskPostID.TextChanged
        If lblDiscordTaskPostID.Text.StartsWith("✅") Then
            btnDiscordTaskPostIDGo.Enabled = True
        Else
            btnDiscordTaskPostIDGo.Enabled = False
        End If
    End Sub

    Private Sub lblWSGTaskEntrySeqID_TextChanged(sender As Object, e As EventArgs) Handles lblWSGTaskEntrySeqID.TextChanged
        If lblWSGTaskEntrySeqID.Text.StartsWith("✅") Then
            btnDiscordTaskPostIDSet.Enabled = True AndAlso IsOwnerOrShared
        Else
            btnDiscordTaskPostIDSet.Enabled = False
        End If
    End Sub

    Private Sub btnDiscordTaskPostIDGo_Click(sender As Object, e As EventArgs) Handles btnDiscordTaskPostIDGo.Click
        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.TaskLibraryDiscordURL(_useTestMode)}/{_taskDiscordPostID}")
        SupportingFeatures.BringDiscordToTop()
    End Sub

    Private Sub btnDiscordTaskPostIDSet_Click(sender As Object, e As EventArgs) Handles btnDiscordTaskPostIDSet.Click

        If lblDiscordTaskPostID.Text.StartsWith("✅") Then
            'Ask for confirmation
            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, $"Are you REALLY sure you want to change the Discord Post ID associated with this task?", "Confirm Discord Post ID change", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                    Exit Sub
                End If
            End Using
        End If

        SupportingFeatures.BringDiscordToTop()
        SupportingFeatures.BringDPHToolToTop(Me.Handle)

        Dim message As String = "Please get the link to the task's post in Discord (""...More menu"" and ""Copy Message Link"")"
        Dim waitingForm As WaitingForURLForm
        Dim answer As DialogResult
        Dim validTaskIDOrCancel As Boolean = False
        Dim autoContinue As Boolean = True
        Dim newTaskDiscordTaskID As String = String.Empty

        Do Until validTaskIDOrCancel
            Clipboard.Clear()
            waitingForm = New WaitingForURLForm(message)
            answer = waitingForm.ShowDialog()

            SupportingFeatures.BringDPHToolToTop(Me.Handle)
            'Check if the clipboard contains a valid URL, which would mean the task's URL has been copied
            If answer = DialogResult.OK Then
                Dim taskThreadURL As String
                taskThreadURL = SupportingFeatures.DiscordURL(Clipboard.GetText, False)
                newTaskDiscordTaskID = SupportingFeatures.ExtractMessageIDFromDiscordURL(taskThreadURL,, _useTestMode)
                If newTaskDiscordTaskID.Trim.Length = 0 Then
                    Using New Centered_MessageBox(Me)
                        If MessageBox.Show(Me, $"Invalid Discord Post ID - If you posted under the Task Library, try again. If not, click Cancel.", "Discord Post ID missing", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                            validTaskIDOrCancel = True
                            autoContinue = False
                        End If
                    End Using
                Else
                    validTaskIDOrCancel = True
                    SaveSession()
                End If
            Else
                validTaskIDOrCancel = True
                autoContinue = False
            End If
            If autoContinue AndAlso newTaskDiscordTaskID.Trim.Length > 0 AndAlso _taskDiscordPostID <> newTaskDiscordTaskID Then
                _taskDiscordPostID = newTaskDiscordTaskID
                SessionModified(SourceOfChange.DiscordTab)
            End If
        Loop

    End Sub

    Private Sub ToggleTestMode(testMode As Boolean)
        _useTestMode = testMode
        lblCountriesTestModeMsg.Visible = testMode
    End Sub

    Private ReadOnly Property WSGEventID As String
        Get

            If _ClubPreset Is Nothing Then
                Return String.Empty
            End If

            Dim key As String = String.Empty
            Dim eventDate As Date
            eventDate = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, chkDateTimeUTC.Checked)
            eventDate = eventDate.ToUniversalTime
            key = $"E-{_ClubPreset.EventNewsID}{eventDate.ToUniversalTime.ToString("yyyyMMdd")}"

            Return key

        End Get
    End Property

    Private Sub lblEventClubNotAuthorized_VisibleChanged(sender As Object, e As EventArgs) Handles lblEventClubNotAuthorized.VisibleChanged
        lblEventClubNotAuthorized.Enabled = lblEventClubNotAuthorized.Visible
        grpDiscordGroupFlight.Enabled = chkActivateEvent.Checked AndAlso IsClubAuthorizedEventOrganizer
    End Sub

    Public Function FetchTaskEventSummaries(entrySeqID As Integer) As List(Of String)

        Dim results As New List(Of String)

        Try
            Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FetchTaskEventsByEntrySeqID.php"
            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"

            ' POST body
            Dim postData As String = $"EntrySeqID={entrySeqID}"
            Dim postBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(postData)
            request.ContentLength = postBytes.Length
            Using reqStream As Stream = request.GetRequestStream()
                reqStream.Write(postBytes, 0, postBytes.Length)
            End Using

            ' Read the response
            Dim jsonResponse As String
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    jsonResponse = reader.ReadToEnd()
                End Using
            End Using

            ' Parse the JSON
            Dim parsed As Dictionary(Of String, Object) =
            JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)

            If parsed("status").ToString() <> "success" Then
                Throw New Exception(parsed("message").ToString())
            End If

            Dim data = DirectCast(parsed("data"), Newtonsoft.Json.Linq.JArray)
            For Each row As Newtonsoft.Json.Linq.JObject In data
                Dim clubEventNewsID As String = row("ClubEventNewsID")?.ToString()
                Dim eventDateStr As String = row("EventDateTime")?.ToString()

                Dim localDate As DateTime
                If Not DateTime.TryParse(eventDateStr, localDate) Then
                    Continue For
                End If

                ' Try to match with a PresetEvent
                Dim matchedClub As PresetEvent = Nothing
                For Each club As PresetEvent In _SF.DefaultKnownClubEvents.Values
                    If String.Equals(club.EventNewsID, clubEventNewsID, StringComparison.OrdinalIgnoreCase) Then
                        matchedClub = club
                        Exit For
                    End If
                Next

                If matchedClub IsNot Nothing Then
                    ' 1) Parse the Zulu DATE (no time) from TaskEvents
                    Dim eventDateOnly As Date
                    If Not Date.TryParse(eventDateStr, eventDateOnly) Then
                        ' If parsing fails, fallback to raw
                        results.Add($"{clubEventNewsID} of {eventDateStr}")
                        Continue For
                    End If

                    ' 2) Ask the club which Zulu time-of-day applies on that DATE (DST-aware via club timezone)
                    '    GetZuluTimeForDate returns a DateTime whose TIME part we need.
                    Dim zuluTemplate As DateTime = matchedClub.GetZuluTimeForDate(eventDateOnly, forceStandard:=False)

                    ' 3) Build the full UTC timestamp: [DATE from TaskEvents] + [TIME from template], explicitly UTC
                    Dim eventZulu As New DateTime(
                            eventDateOnly.Year, eventDateOnly.Month, eventDateOnly.Day,
                            zuluTemplate.Hour, zuluTemplate.Minute, zuluTemplate.Second,
                            DateTimeKind.Utc
                        )

                    ' 4) Convert to local time (your machine’s local zone; or fix to "America/Toronto" if you prefer)
                    Dim localTz As TimeZoneInfo = TimeZoneInfo.Local
                    ' Dim localTz As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Toronto")
                    Dim localDT As DateTime = TimeZoneInfo.ConvertTimeFromUtc(eventZulu, localTz)

                    ' 5) Build display name: ClubFullName [+ ClubName if not already contained]
                    Dim displayName As String = matchedClub.ClubFullName
                    If Not String.IsNullOrWhiteSpace(matchedClub.ClubName) AndAlso
                           matchedClub.ClubFullName.IndexOf(matchedClub.ClubName, StringComparison.OrdinalIgnoreCase) < 0 Then
                        displayName &= " " & matchedClub.ClubName
                    End If

                    ' 6) Final label (use local DATE part)
                    Dim label As String = $"{displayName} of {localDT:yyyy-MM-dd}"
                    results.Add(label)
                Else
                    ' Fallback: just output the ID and the (parsed) date as given if possible
                    Dim eventDateOnly As Date
                    If Date.TryParse(eventDateStr, eventDateOnly) Then
                        results.Add($"{clubEventNewsID} of {eventDateOnly:yyyy-MM-dd}")
                    Else
                        results.Add($"{clubEventNewsID} of {eventDateStr}")
                    End If
                End If
            Next

        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error fetching TaskEvents: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End Try

        Return results
    End Function

#End Region

End Class
