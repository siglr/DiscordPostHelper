Imports System.Xml
Imports System.IO
Imports System.Xml.Serialization
Imports System.Text
Imports System.Threading
Imports System.Globalization
Imports SIGLR.SoaringTools.CommonLibrary
Imports System.Reflection
Imports SIGLR.SoaringTools.ImageViewer
Imports System.Collections.Specialized
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports System.ComponentModel
Imports System.Data.SqlTypes
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Class Main

#Region "Members, Constants and Enums"

    Public Shared SessionSettings As New AllSettings

    Private Const DiscordLimit As Integer = 2000
    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"
    Private Const DefaultMapImageWidth As Integer = 1647
    Private Const DefaultMapImageHeight As Integer = 639

    Private ReadOnly _SF As New SupportingFeatures
    Private ReadOnly _CurrentDistanceUnit As Integer
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _ClubPreset As PresetEvent = Nothing
    Private _GuideCurrentStep As Integer = 0
    Private _FlightTotalDistanceInKm As Double = 0
    Private _TaskTotalDistanceInKm As Double = 0
    Private _CurrentSessionFile As String = String.Empty
    Private _loadingFile As Boolean = False
    Private _sessionModified As Boolean = False
    Private _PossibleElevationUpdateRequired As Boolean = False
    Private _timeStampContextualMenuDateTime As DateTime
    Private _userPermissionID As String
    Private _userPermissions As New Dictionary(Of String, Boolean)
    Private _TBTaskEntrySeqID As Integer
    Private _TBTaskDBEntryUpdate As DateTime
    Private _TBTaskLastUpdate As DateTime

    Private _OriginalFlightPlanTitle As String = String.Empty
    Private _OriginalFlightPlanDeparture As String = String.Empty
    Private _OriginalFlightPlanDepRwy As String = String.Empty
    Private _OriginalFlightPlanArrival As String = String.Empty
    Private _OriginalFlightPlanShortDesc As String = String.Empty

    Private _taskThreadFirstPostID As String = String.Empty

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

        _SF.FillCountryFlagList(cboCountryFlag.Items)

        ResetForm()

        SetTimePickerFormat()

        SessionSettings.Load()

        RestoreMainFormLocationAndSize()

        If SessionSettings.WaitSecondsForFiles >= 1 AndAlso SessionSettings.WaitSecondsForFiles <= 10 Then
            numWaitSecondsForFiles.Value = SessionSettings.WaitSecondsForFiles
        End If
        chkDPOExpertMode.Checked = SessionSettings.AutomaticPostingProgression

        LoadDPOptions()
        LoadDGPOptions()

        'Get the permission ID
        _userPermissionID = GetUserIDFromPermissionsFile()

        If My.Application.CommandLineArgs.Count > 0 Then
            ' Open the file passed as an argument
            _CurrentSessionFile = My.Application.CommandLineArgs(0)
            'Check if the selected file is a dph or dphx files
            If Path.GetExtension(_CurrentSessionFile) = ".dphx" Then
                'Package - we need to unpack it first
                OpenFileDialog1.FileName = _SF.UnpackDPHXFile(_CurrentSessionFile)

                If OpenFileDialog1.FileName = String.Empty Then
                    _CurrentSessionFile = String.Empty
                Else
                    _CurrentSessionFile = OpenFileDialog1.FileName
                End If
            End If
        Else
            'Load previous session data
            If Not SessionSettings.LastFileLoaded = String.Empty Then
                _CurrentSessionFile = SessionSettings.LastFileLoaded
            End If
        End If
        Me.Refresh()
        _loadingFile = True
        LoadSessionData(_CurrentSessionFile)
        _loadingFile = False
        CheckForNewVersion()

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
            chkDPOMainPost.Checked = SessionSettings.DPO_chkDPOMainPost
            chkDPOThreadCreation.Checked = SessionSettings.DPO_chkDPOThreadCreation
            chkDPOIncludeCoverImage.Checked = SessionSettings.DPO_chkDPOIncludeCoverImage
            chkDPOFullDescription.Checked = SessionSettings.DPO_chkDPOFullDescription
            chkDPOFilesWithDescription.Checked = SessionSettings.DPO_chkDPOFilesWithDescription
            chkDPOFilesAlone.Checked = SessionSettings.DPO_chkDPOFilesAlone
            chkDPOAltRestrictions.Checked = SessionSettings.DPO_chkDPOAltRestrictions
            chkDPOWeatherInfo.Checked = SessionSettings.DPO_chkDPOWeatherInfo
            chkDPOWeatherChart.Checked = SessionSettings.DPO_chkDPOWeatherChart
            chkDPOWaypoints.Checked = SessionSettings.DPO_chkDPOWaypoints
            chkDPOAddOns.Checked = SessionSettings.DPO_chkDPOAddOns
            chkDPOResultsInvitation.Checked = SessionSettings.DPO_chkDPOResultsInvitation
            chkDPOFeaturedOnGroupFlight.Checked = SessionSettings.DPO_chkDPOFeaturedOnGroupFlight
        End If

    End Sub
    Private Sub LoadDGPOptions()

        If SessionSettings.DPO_DGPOUseCustomSettings Then
            chkDGPOCoverImage.Checked = SessionSettings.DPO_chkDGPOCoverImage
            chkDGPOMainGroupPost.Checked = SessionSettings.DPO_chkDGPOMainGroupPost
            chkDGPOThreadCreation.Checked = SessionSettings.DPO_chkDGPOThreadCreation
            chkDGPOTeaser.Checked = SessionSettings.DPO_chkDGPOTeaser
            chkDGPOFilesWithFullLegend.Checked = SessionSettings.DPO_chkDGPOFilesWithFullLegend
            chkDGPOFilesWithoutLegend.Checked = SessionSettings.DPO_chkDGPOFilesWithoutLegend
            chkDGPODPHXOnly.Checked = SessionSettings.DPO_chkDGPODPHXOnly
            chkDGPOMainPost.Checked = SessionSettings.DPO_chkDGPOMainPost
            chkDGPOFullDescription.Checked = SessionSettings.DPO_chkDGPOFullDescription
            chkDGPOAltRestrictions.Checked = SessionSettings.DPO_chkDGPOAltRestrictions
            chkDGPOWeatherInfo.Checked = SessionSettings.DPO_chkDGPOWeatherInfo
            chkDGPOWeatherChart.Checked = SessionSettings.DPO_chkDGPOWeatherChart
            chkDGPOWaypoints.Checked = SessionSettings.DPO_chkDGPOWaypoints
            chkDGPOAddOns.Checked = SessionSettings.DPO_chkDGPOAddOns
            chkDGPORelevantTaskDetails.Checked = SessionSettings.DPO_chkDGPORelevantTaskDetails
            chkDGPOEventLogistics.Checked = SessionSettings.DPO_chkDGPOEventLogistics
        End If

    End Sub

    Private Sub RestoreMainFormLocationAndSize()
        Dim sizeString As String = SessionSettings.MainFormSize
        Dim locationString As String = SessionSettings.MainFormLocation

        If sizeString <> "" Then
            Dim sizeArray As String() = sizeString.TrimStart("{").TrimEnd("}").Split(",")
            Dim width As Integer = CInt(sizeArray(0).Split("=")(1))
            Dim height As Integer = CInt(sizeArray(1).Split("=")(1))
            Me.Size = New Size(width, height)
        End If

        If locationString <> "" Then
            Dim locationArray As String() = locationString.TrimStart("{").TrimEnd("}").Split(",")
            Dim x As Integer = CInt(locationArray(0).Split("=")(1))
            Dim y As Integer = CInt(locationArray(1).Split("=")(1))
            Me.Location = New Point(x, y)
        End If

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

    End Sub

    Private Sub ResetForm()

        _loadingFile = True
        _CurrentSessionFile = String.Empty

        BriefingControl1.FullReset()

        lblThread1stMsgIDNotAcquired.Visible = True
        lblThread1stMsgIDAcquired.Visible = False

        _XmlDocFlightPlan = New XmlDocument
        _XmlDocWeatherPreset = New XmlDocument
        _WeatherDetails = Nothing
        _FlightTotalDistanceInKm = 0
        _TaskTotalDistanceInKm = 0
        _PossibleElevationUpdateRequired = False
        lblElevationUpdateWarning.Visible = _PossibleElevationUpdateRequired

        cboDifficulty.SelectedIndex = 0
        cboVoiceChannel.Items.Clear()
        cboVoiceChannel.Items.AddRange(_SF.GetVoiceChannels.ToArray)
        cboMSFSServer.Items.Clear()
        cboMSFSServer.Items.AddRange(_SF.GetMSFSServers.ToArray)
        cboRecommendedGliders.SelectedIndex = 0
        lstAllFiles.Items.Clear()
        lstAllCountries.Items.Clear()
        lstAllRecommendedAddOns.Items.Clear()

        txtFilesText.Text = String.Empty

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
        chkLockCountries.Checked = False
        txtWeatherSummary.Text = String.Empty
        txtAltRestrictions.Text = String.Empty
        txtWeatherFirstPart.Text = String.Empty
        txtWeatherWinds.Text = String.Empty
        txtWeatherClouds.Text = String.Empty
        txtFullDescriptionResults.Text = String.Empty
        cboGroupOrClubName.SelectedIndex = -1
        txtClubFullName.Text = String.Empty
        txtClubFullName.ReadOnly = True
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
        txtDiscordEventShareURL.Text = String.Empty
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
        txtDiscordTaskID.Text = String.Empty
        _taskThreadFirstPostID = String.Empty
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

        _SF.PopulateSoaringClubList(cboGroupOrClubName.Items)
        _SF.PopulateKnownDesignersList(cboKnownTaskDesigners.Items)
        _SF.AllWaypoints.Clear()
        _TBTaskEntrySeqID = 0

        'BuildFPResults()
        'BuildGroupFlightPost()
        SetFormCaption(String.Empty)
        FixForDropDownCombos()

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
            SessionSettings.MainFormSize = Me.Size.ToString()
            SessionSettings.MainFormLocation = Me.Location.ToString()
            SessionSettings.WaitSecondsForFiles = numWaitSecondsForFiles.Value
            SessionSettings.LastFileLoaded = _CurrentSessionFile
            SessionSettings.FlightPlanTabSplitterLocation = FlightPlanTabSplitter.SplitPosition
            SessionSettings.AutomaticPostingProgression = chkDPOExpertMode.Checked
            SessionSettings.Save()
            BriefingControl1.Closing()
        End If

    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles toolStripResetAll.Click
        If CheckUnsavedAndConfirmAction("reset all") Then
            ResetForm()
            TabControl1.SelectTab(0)
            Select Case TabControl1.SelectedTab.Name
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

    Private Sub EnterTextBox(sender As Object, e As EventArgs) Handles txtWeatherWinds.Enter, txtWeatherSummary.Enter, txtWeatherFirstPart.Enter, txtWeatherClouds.Enter, txtTitle.Enter, txtSoaringTypeExtraInfo.Enter, txtSimDateTimeExtraInfo.Enter, txtShortDescription.Enter, txtMainArea.Enter, txtLongDescription.Enter, txtGroupFlightEventPost.Enter, txtFullDescriptionResults.Enter, txtFPResults.Enter, txtFilesText.Enter, txtEventTitle.Enter, txtEventDescription.Enter, txtDurationMin.Enter, txtDurationMax.Enter, txtDurationExtraInfo.Enter, txtDiscordEventTopic.Enter, txtDiscordEventDescription.Enter, txtDifficultyExtraInfo.Enter, txtDepName.Enter, txtDepExtraInfo.Enter, txtCredits.Enter, txtArrivalName.Enter, txtArrivalExtraInfo.Enter, txtAltRestrictions.Enter, txtBaroPressureExtraInfo.Enter, txtOtherBeginnerLink.Enter, txtEventTeaserMessage.Enter, txtClubFullName.Enter
        SupportingFeatures.EnteringTextBox(sender)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        FixForDropDownCombos()
        Select Case TabControl1.SelectedTab.Name
            Case "tabBriefing"
                GenerateBriefing()
            Case "tabDiscord"
                BuildFPResults()
                BuildWeatherCloudLayers()
                BuildWeatherWindLayers()
                BuildWeatherInfoResults()
                BuildGroupFlightPost()
                CheckWhichOptionsCanBeEnabled()
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
            Dim currentFile As String = _CurrentSessionFile
            ResetForm()
            LoadFile(currentFile)
            TabControl1.SelectTab(0)
            Select Case TabControl1.SelectedTab.Name
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
                MessageBox.Show(Me, "A title is required before saving!", "Title required", vbOKOnly, vbCritical)
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
            DPHXFilename = $"{Path.GetDirectoryName(_CurrentSessionFile)}\{DPHXFilename}.dphx"
            DPHFilename = $"{Path.GetDirectoryName(_CurrentSessionFile)}\{DPHFilename}.dph"
        Else
            If txtFlightPlanFile.Text = String.Empty Then
                SaveFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
            Else
                SaveFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
            End If
            SaveFileDialog1.FileName = txtTitle.Text
            SaveFileDialog1.Title = "Select session file to save"
            SaveFileDialog1.Filter = "Discord Post Helper Session|*.dph"
            result = SaveFileDialog1.ShowDialog()
            If result = DialogResult.OK Then
                DPHFilename = SaveFileDialog1.FileName
                DPHXFilename = $"{Path.GetDirectoryName(SaveFileDialog1.FileName)}\{Path.GetFileNameWithoutExtension(SaveFileDialog1.FileName)}.dphx"
            End If
        End If

        If saveWithoutAsking OrElse result = DialogResult.OK Then
            txtDPHXPackageFilename.Text = DPHXFilename

            SaveSessionData(DPHFilename)
            _CurrentSessionFile = DPHFilename

            'Then save the DPHX as well
            'Check if file already exists and delete it
            If File.Exists(DPHXFilename) Then
                File.Delete(DPHXFilename)
            End If

            ' Zip the selected files using the ZipFiles method
            Dim filesToInclude As New List(Of String)()
            filesToInclude.Add(_CurrentSessionFile)
            If File.Exists(txtFlightPlanFile.Text) Then
                filesToInclude.Add(txtFlightPlanFile.Text)
            End If
            If File.Exists(txtWeatherFile.Text) Then
                filesToInclude.Add(txtWeatherFile.Text)
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

        Dim designerSelected As String = cboKnownTaskDesigners.Text.Substring(0, cboKnownTaskDesigners.Text.LastIndexOf("#")).Trim

        txtCredits.Text = $"All credits to @{designerSelected} for this task."

    End Sub

    Private Sub FileDropZone1_FilesDropped(sender As Object, e As FilesDroppedEventArgs) Handles FileDropZone1.FilesDropped

        Dim droppedFlightPlan As String = String.Empty
        Dim droppedWeather As String = String.Empty
        Dim droppedDPH As String = String.Empty
        Dim droppedDPHX As String = String.Empty
        Dim droppedOtherFiles As List(Of String) = New List(Of String)

        ' Iterate through the array of dropped file paths
        For Each filePath As String In e.DroppedFiles
            ' Process each file
            Select Case Path.GetExtension(filePath).ToUpper
                Case ".PLN"
                    droppedFlightPlan = filePath
                Case ".WPR"
                    droppedWeather = filePath
                Case ".DPH"
                    droppedDPH = filePath
                Case ".DPHX"
                    droppedDPHX = filePath
                Case Else
                    droppedOtherFiles.Add(filePath)
            End Select
        Next

        'Handle all invalid cases
        Dim invalidMessage As String = String.Empty
        If droppedDPH <> String.Empty AndAlso droppedDPHX <> String.Empty Then
            invalidMessage = "Either one DPH or one DPHX files can be dropped here, not both."
        End If
        If (droppedDPH <> String.Empty OrElse droppedDPHX <> String.Empty) AndAlso
                (droppedFlightPlan <> String.Empty OrElse
                droppedWeather <> String.Empty OrElse
                droppedOtherFiles.Count > 0) Then
            invalidMessage = "A DPH or DPHX file cannot be dropped along with any other files."
        End If
        If Not grbTaskInfo.Enabled AndAlso droppedFlightPlan = String.Empty AndAlso (droppedWeather <> String.Empty OrElse droppedOtherFiles.Count > 0) Then
            invalidMessage = "There is no flight plan loaded or dropped to load, so no other files can be specified in an empty session."
        End If
        If invalidMessage <> String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, invalidMessage, "Invalid file drop", vbOKOnly, MessageBoxIcon.Error)
            End Using
            Return
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
            LoadFlightPlan(droppedFlightPlan)
        End If
        If droppedWeather <> String.Empty Then
            LoadWeatherfile(droppedWeather)
        End If
        For Each otherFile As String In droppedOtherFiles
            If lstAllFiles.Items.Count = 7 Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Discord does not allow more than 10 files!", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Exit For
            End If
            AddExtraFile(otherFile)
        Next
        LoadPossibleImagesInMapDropdown(cboBriefingMap.SelectedItem)
        LoadPossibleImagesInCoverDropdown(cboCoverImage.SelectedItem)

    End Sub

    Private Sub AddExtraFile(otherFile As String)
        If _SF.ExtraFileExtensionIsValid(otherFile) Then
            If Not lstAllFiles.Items.Contains(otherFile) Then
                lstAllFiles.Items.Add(otherFile)
                SessionModified()
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

    Private Sub btnDiscordTaskThreadURLPaste_Click(sender As Object, e As EventArgs) Handles btnDiscordTaskThreadURLPaste.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            Dim extractedID As String = SupportingFeatures.ExtractMessageIDFromDiscordURL(Clipboard.GetText)
            If extractedID <> String.Empty Then
                txtDiscordTaskID.Text = extractedID
                _taskThreadFirstPostID = String.Empty
            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "The URL you copied does not contain a valid ID for the task. The URL must come from a task published in the Task Library on Discord.", "Error extracting task ID", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        End If
    End Sub

    Private Sub btnDeleteDiscordID_Click(sender As Object, e As EventArgs) Handles btnDeleteDiscordID.Click

        If txtDiscordTaskID.Text.Trim <> String.Empty Then
            'TODO: If the task exists online - do you want to delete it?

            Using New Centered_MessageBox(Me)
                If MessageBox.Show(Me, "Are you sure you want to clear the Discord ID from this task ?", "Please confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    txtDiscordTaskID.Text = String.Empty
                    _taskThreadFirstPostID = String.Empty
                    lblThread1stMsgIDNotAcquired.Visible = True
                    lblThread1stMsgIDAcquired.Visible = False
                End If
            End Using
        End If
    End Sub

    Private Sub txtDiscordTaskID_TextChanged(sender As Object, e As EventArgs) Handles txtDiscordTaskID.TextChanged
        If txtDiscordTaskID.Text.Trim = String.Empty Then
            lblTaskLibraryIDNotAcquired.Visible = True
            lblTaskLibraryIDAcquired.Visible = False
            'TODO: TaskID got deleted - do we need to do something?
        Else
            lblTaskLibraryIDAcquired.Visible = True
            lblTaskLibraryIDNotAcquired.Visible = False
            'Try to retrieve the task EntrySeqID online
            Try
                GetTaskDetails(txtDiscordTaskID.Text.Trim)
            Catch ex As Exception
                'Do nothing - it means the task has not been pushed to the online database
            End Try
        End If
        SetTBTaskDetailsLabel()
        AllFieldChanges(sender, e)
    End Sub

    Private Sub AllFieldChanges(sender As Object, e As EventArgs) Handles chkTitleLock.CheckedChanged,
                                                                          chkDepartureLock.CheckedChanged,
                                                                          chkArrivalLock.CheckedChanged,
                                                                          chkDescriptionLock.CheckedChanged,
                                                                          chkLockCountries.CheckedChanged,
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
                                                                          txtDiscordEventShareURL.TextChanged,
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
                                                                          dtRepostOriginalDate.ValueChanged

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
        SessionModified()

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
                                                                                           txtDiscordEventShareURL.Leave,
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

    Private Sub SelectFlightPlan_Click(sender As Object, e As EventArgs) Handles btnSelectFlightPlan.Click

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
            If txtFlightPlanFile.Text.Trim.Length > 0 AndAlso OpenFileDialog1.FileName <> txtFlightPlanFile.Text Then
                'User has selected a different flight plan than the current one - ask to reset first?
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show(Me, "You have selected a different flight plan file. Do you want to reset first ?", "Selecting a new flight plan file", vbYesNo, MessageBoxIcon.Question) = vbYes Then
                        'Reset first
                        ResetForm()
                    End If
                End Using
            End If
            LoadFlightPlan(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub txtFPResults_TextChanged(sender As Object, e As EventArgs) Handles txtFPResults.TextChanged
        lblNbrCarsMainFP.Text = $"{txtFPResults.Text.Length} chars"
    End Sub

    Private Sub txtAltRestrictions_TextChanged(sender As Object, e As EventArgs) Handles txtAltRestrictions.TextChanged
        lblNbrCarsRestrictions.Text = $"{txtAltRestrictions.Text.Length} chars"
    End Sub

    Private Sub NbrCarsCheckDiscordLimitEvent(sender As Object, e As EventArgs) Handles lblNbrCarsMainFP.TextChanged, lblNbrCarsFullDescResults.TextChanged, lblNbrCarsFullDescResults.TextChanged, lblNbrCarsWeather.TextChanged, lblNbrCarsRestrictions.TextChanged, lblNbrCarsWaypoints.TextChanged

        NbrCarsCheckDiscordLimit(DirectCast(sender, Windows.Forms.Label))

    End Sub

    Private Sub NbrCarsChangedOnWeatherDetails(sender As Object, e As EventArgs) Handles lblNbrCarsWeatherInfo.TextChanged,
                                                                                         lblNbrCarsWeatherWinds.TextChanged,
                                                                                         lblNbrCarsWeatherClouds.TextChanged

        Dim totalCars As Integer = 0
        Try
            totalCars += CInt(lblNbrCarsWeatherInfo.Text)
            totalCars += CInt(lblNbrCarsWeatherWinds.Text)
            totalCars += CInt(lblNbrCarsWeatherClouds.Text)

        Catch ex As Exception
        End Try

        lblNbrCarsWeather.Text = $"{totalCars.ToString} chars"

    End Sub

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

    Private Sub txtFullDescriptionResults_TextChanged(sender As Object, e As EventArgs) Handles txtFullDescriptionResults.TextChanged
        lblNbrCarsFullDescResults.Text = $"{txtFullDescriptionResults.Text.Length} chars"
    End Sub

    Private Sub txtWaypointsDetails_TextChanged(sender As Object, e As EventArgs) Handles txtWaypointsDetails.TextChanged
        lblNbrCarsWaypoints.Text = $"{txtWaypointsDetails.Text.Length} chars"
    End Sub

    Private Sub txtAddOnsDetails_TextChanged(sender As Object, e As EventArgs) Handles txtAddOnsDetails.TextChanged
        lblNbrCarsAddOns.Text = $"{txtAddOnsDetails.Text.Length} chars"
    End Sub

    Private Sub CopyToEventFields(sender As Object, e As EventArgs)

        If sender Is txtTitle Then
            txtEventTitle.Text = txtTitle.Text
        End If

    End Sub

    Private Sub txtFlightPlanFile_TextChanged(sender As Object, e As EventArgs) Handles txtFlightPlanFile.TextChanged

        If txtFlightPlanFile.Text = String.Empty Then
            grbTaskInfo.Enabled = False
            grbTaskPart2.Enabled = False
            grbTaskDiscord.Enabled = False
        Else
            grbTaskInfo.Enabled = True
            grbTaskPart2.Enabled = True
            grbTaskDiscord.Enabled = True
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

    Private Sub btnWeatherBrowser_Click(sender As Object, e As EventArgs) Handles btnWeatherBrowser.Click

        WeatherPresetsBrowser.ShowDialog(Me)

    End Sub

    Private Sub btnSyncTitles_Click(sender As Object, e As EventArgs) Handles btnSyncTitles.Click

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
            For i As Integer = 0 To OpenFileDialog1.FileNames.Count - 1
                'Check if one of the files is selected flight plan or weather file to exclude them
                If OpenFileDialog1.FileNames(i) <> txtFlightPlanFile.Text And OpenFileDialog1.FileNames(i) <> txtWeatherFile.Text Then
                    If lstAllFiles.Items.Count = 7 Then
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show(Me, "Discord does not allow more than 10 files!", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using
                        Exit For
                    End If
                    AddExtraFile(OpenFileDialog1.FileNames(i))
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
            SessionModified()
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
        SessionModified()

    End Sub

    Private Sub btnExtraFileDown_Click(sender As Object, e As EventArgs) Handles btnExtraFileDown.Click

        MoveExtraFilesSelectedItems(1, lstAllFiles)
        btnExtraFileDown.Focus()
        SessionModified()

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
            SessionModified()
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
                SessionModified()
            Case DialogResult.Cancel
                'Cancel
        End Select

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

    End Sub

    Private Sub btnRemoveSelectedAddOns_Click(sender As Object, e As EventArgs) Handles btnRemoveSelectedAddOns.Click

        For i As Integer = lstAllRecommendedAddOns.SelectedIndices.Count - 1 To 0 Step -1
            lstAllRecommendedAddOns.Items.RemoveAt(lstAllRecommendedAddOns.SelectedIndices(i))
            SessionModified()
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

        SessionModified()

    End Sub

    Private Sub btnAddOnDown_Click(sender As Object, e As EventArgs) Handles btnAddOnDown.Click

        MoveExtraFilesSelectedItems(1, lstAllRecommendedAddOns)
        btnAddOnDown.Focus()

        'Update recommended add-ons textbox
        BuildRecAddOnsText()

        SessionModified()

    End Sub

#End Region

#Region "Country controls events"

    Private Sub btnAddCountry_Click(sender As Object, e As EventArgs) Handles btnAddCountry.Click
        If cboCountryFlag.SelectedIndex > 0 AndAlso Not lstAllCountries.Items.Contains(cboCountryFlag.Text) Then
            lstAllCountries.Items.Add(cboCountryFlag.Text)
            'BuildFPResults()
            SessionModified()
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
            SessionModified()
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
        SessionModified()
    End Sub

    Private Sub btnMoveCountryDown_Click(sender As Object, e As EventArgs) Handles btnMoveCountryDown.Click
        MoveExtraFilesSelectedItems(1, lstAllCountries)
        'BuildFPResults()
        SessionModified()
    End Sub

#End Region

#Region "Flight Plan tab Subs & Functions"

    Private Function HighlightExpectedFields(Optional showMessageBox As Boolean = False) As Boolean

        Dim messageText As New StringBuilder
        Dim requiredText As New StringBuilder
        Dim cannotContinue As Boolean = False

        If txtTitle.Text.Trim = String.Empty Then
            SetLabelFormat(lblTitle, LabelFormat.BoldRed, requiredText, "A title is required!")
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

    Private Sub BuildFPResults(Optional fromGroup As Boolean = False)

        Dim sb As New StringBuilder()

        If fromGroup Then
            sb.AppendLine($"## Task Details")
        Else
            sb.AppendLine($"# {txtTitle.Text}{AddFlagsToTitle()}")
            If chkRepost.Checked Then
                If txtRepostOriginalURL.TextLength > 0 Then
                    sb.AppendLine($"This task was originally posted on [{SupportingFeatures.ReturnDiscordServer(txtRepostOriginalURL.Text)}]({txtRepostOriginalURL.Text}) on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}")
                Else
                    sb.AppendLine($"This task was originally posted on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}")
                End If
            End If
        End If
        sb.AppendLine()
        sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(txtShortDescription.Text,,, 2))
        If txtMainArea.Text.Trim.Length > 0 Then
            sb.AppendLine("> 🗺 " & SupportingFeatures.ValueToAppendIfNotEmpty(txtMainArea.Text))
        End If
        sb.AppendLine($"> 🛫 {SupportingFeatures.ValueToAppendIfNotEmpty(txtDepartureICAO.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDepName.Text, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDepExtraInfo.Text, True, True)}")
        sb.AppendLine($"> 🛬 {SupportingFeatures.ValueToAppendIfNotEmpty(txtArrivalICAO.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtArrivalName.Text, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtArrivalExtraInfo.Text, True, True)}")
        sb.AppendLine($"> ⌚ {SupportingFeatures.GetEnUSFormattedDate(dtSimDate.Value, dtSimLocalTime.Value, chkIncludeYear.Checked)} local in MSFS{SupportingFeatures.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text.Trim, True, True)}")
        sb.AppendLine($"> ↗️ {GetSoaringTypesSelected()}{SupportingFeatures.ValueToAppendIfNotEmpty(txtSoaringTypeExtraInfo.Text, True, True)}")
        sb.AppendLine($"> 📏 {SupportingFeatures.GetDistance(txtDistanceTotal.Text, txtDistanceTrack.Text)}")
        sb.AppendLine($"> ⏳ {SupportingFeatures.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}")
        If txtAATTask.Text.Length > 0 Then
            sb.AppendLine($"> ⚠️ {txtAATTask.Text}")
        End If
        sb.AppendLine($"> ✈️ {SupportingFeatures.ValueToAppendIfNotEmpty(cboRecommendedGliders.Text)}")
        sb.AppendLine($"> 🎚 {SupportingFeatures.GetDifficulty(cboDifficulty.SelectedIndex, txtDifficultyExtraInfo.Text)}")

        If Not fromGroup Then
            sb.AppendLine()
            sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(txtCredits.Text,,, 1))
            sb.Append("### See inside thread for most up-to-date files and more information.")
        End If

        txtFPResults.Text = sb.ToString.Trim

        If txtLongDescription.Text.Trim.Length > 0 Then
            txtFullDescriptionResults.Text = $"## 📖 Full Description{Environment.NewLine}{txtLongDescription.Text.Trim}"
        Else
            txtFullDescriptionResults.Text = $"## 📖 Full Description{Environment.NewLine}None provided"
        End If

        txtWaypointsDetails.Text = _SF.GetAllWPCoordinates()

    End Sub

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

        If chkSoaringTypeRidge.Checked Then
            selectedTypes.Add("Ridge")
        End If

        If chkSoaringTypeThermal.Checked Then
            selectedTypes.Add("Thermal")
        End If

        If chkSoaringTypeWave.Checked Then
            selectedTypes.Add("Wave")
        End If

        If chkSoaringTypeDynamic.Checked Then
            selectedTypes.Add("Dynamic")
        End If

        ' Join the selected types into a single string, separated by ", "
        Return String.Join(", ", selectedTypes)

    End Function

    Private Sub LoadFlightPlan(filename As String)

        _PossibleElevationUpdateRequired = False

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

        'Check if there is an AATMinDuration
        If _SF.AATMinDuration > TimeSpan.Zero Then
            txtAATTask.Enabled = True
            txtAATTask.Text = $"AAT with a minimum duration of {SupportingFeatures.FormatTimeSpanAsText(_SF.AATMinDuration)}"
        Else
            txtAATTask.Enabled = False
            txtAATTask.Text = String.Empty
        End If

        lblElevationUpdateWarning.Visible = _PossibleElevationUpdateRequired
        txtDistanceTotal.Text = FormatNumber(_FlightTotalDistanceInKm, 0)

        If _TaskTotalDistanceInKm = 0 Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The task distance is 0! You should open the task in the B21 Online Task Planer and re-download the flight plan again.", "Possible error in flight plan data", vbOKOnly, MessageBoxIcon.Warning)
            End Using
        End If
        txtDistanceTrack.Text = FormatNumber(_TaskTotalDistanceInKm, 0)

        'Build countries
        If Not chkLockCountries.Checked Then
            lstAllCountries.Items.Clear()
            For Each waypoint As ATCWaypoint In _SF.AllWaypoints
                If _SF.CountryFlagCodes.ContainsKey(waypoint.Country) AndAlso Not lstAllCountries.Items.Contains(waypoint.Country) Then
                    lstAllCountries.Items.Add(waypoint.Country)
                End If
            Next
        End If
        HighlightExpectedFields()

        'BuildFPResults()
        'BuildGroupFlightPost()

        SessionModified()

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

    Private Sub LoadWeatherfile(filename As String)
        'read file
        txtWeatherFile.Text = filename
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

        SessionModified()

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

        If cboGroupOrClubName.Text <> cboGroupOrClubName.Text.Trim Then
            cboGroupOrClubName.Text = cboGroupOrClubName.Text.Trim
        End If

        Dim clubExists As Boolean = _SF.DefaultKnownClubEvents.ContainsKey(cboGroupOrClubName.Text.ToUpper)
        txtClubFullName.Text = String.Empty

        If clubExists Then
            _ClubPreset = _SF.DefaultKnownClubEvents(cboGroupOrClubName.Text.ToUpper)

            If _ClubPreset.IsCustom Then
                txtClubFullName.ReadOnly = False
                txtClubFullName.Text = "Specify your own club name"
            Else
                txtClubFullName.ReadOnly = True
                cboGroupOrClubName.Text = _ClubPreset.ClubId
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

            dtEventMeetDate.Value = _SF.FindNextDate(Now, _ClubPreset.EventDayOfWeek, _ClubPreset.ZuluTime)
            dtEventMeetTime.Value = _ClubPreset.GetZuluTimeForDate(dtEventMeetDate.Value)
            Application.DoEvents()
            dtEventSyncFlyTime.Value = dtEventMeetTime.Value.AddMinutes(_ClubPreset.SyncFlyDelay)
            Application.DoEvents()
            dtEventLaunchTime.Value = dtEventSyncFlyTime.Value.AddMinutes(_ClubPreset.LaunchDelay)
            Application.DoEvents()
            dtEventStartTaskTime.Value = dtEventLaunchTime.Value.AddMinutes(_ClubPreset.StartTaskDelay)

            cboBeginnersGuide.Text = _ClubPreset.BeginnerLink

        Else
            _ClubPreset = Nothing
        End If

        'BuildGroupFlightPost()

        SessionModified()

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
        SessionModified()
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
        SessionModified()

    End Sub

    Private Sub chkDateTimeUTC_CheckedChanged(sender As Object, e As EventArgs) Handles chkDateTimeUTC.CheckedChanged
        BuildEventDatesTimes()
        SessionModified()
    End Sub

    Private Sub GroupFlightFieldLeave(sender As Object, e As EventArgs) Handles chkUseSyncFly.CheckedChanged, chkUseStart.CheckedChanged, chkUseLaunch.CheckedChanged, cboVoiceChannel.SelectedIndexChanged, cboMSFSServer.SelectedIndexChanged, cboEligibleAward.SelectedIndexChanged
        'BuildGroupFlightPost()
        SessionModified()
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
        SessionModified()
    End Sub

    Private Sub btnPasteBeginnerLink_Click(sender As Object, e As EventArgs) Handles btnPasteBeginnerLink.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtOtherBeginnerLink.Text = Clipboard.GetText
        End If
        'BuildGroupFlightPost()
    End Sub

    Private Sub chkIncludeBeginnersHelpLink_CheckedChanged(sender As Object, e As EventArgs)
        'BuildGroupFlightPost()
        SessionModified()
    End Sub

    Private Sub EventTabTextControlLeave(sender As Object, e As EventArgs) Handles txtGroupFlightEventPost.Leave, txtEventTitle.Leave, txtEventDescription.Leave, txtDiscordEventTopic.Leave, txtDiscordEventDescription.Leave, txtOtherBeginnerLink.Leave, txtEventTeaserMessage.Leave, txtClubFullName.Leave

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
        grpDiscordGroupFlight.Enabled = chkActivateEvent.Checked
        SessionModified()
    End Sub

    Private Sub chkEventTeaser_CheckedChanged(sender As Object, e As EventArgs) Handles chkEventTeaser.CheckedChanged
        grpEventTeaser.Enabled = chkEventTeaser.Checked
        If chkEventTeaser.Checked AndAlso (txtEventTeaserAreaMapImage.Text.Trim.Length > 0 OrElse txtEventTeaserMessage.Text.Trim.Length > 0) Then
            chkDGPOTeaser.Checked = True
        End If
        SessionModified()
    End Sub

    Private Sub btnClearEventTeaserAreaMap_Click(sender As Object, e As EventArgs) Handles btnClearEventTeaserAreaMap.Click
        txtEventTeaserAreaMapImage.Text = String.Empty
        SessionModified()
    End Sub

    Private Sub btnSelectEventTeaserAreaMap_Click(sender As Object, e As EventArgs) Handles btnSelectEventTeaserAreaMap.Click

        If txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select teaser area map image"
        OpenFileDialog1.Filter = "Image files|*.jpg;*.png"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            txtEventTeaserAreaMapImage.Text = OpenFileDialog1.FileName
            chkDGPOTeaser.Checked = True
            SessionModified()
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

        Dim eventDay As DayOfWeek

        Dim theDate As Date

        theDate = New Date(dtEventMeetDate.Value.Year, dtEventMeetDate.Value.Month, dtEventMeetDate.Value.Day, dtEventMeetTime.Value.Hour, dtEventMeetTime.Value.Minute, 0)
        lblMeetTimeResult.Text = _SF.FormatEventDateTime(theDate, eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblMeetTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        'Check if local DST applies for this date
        lblLocalDSTWarning.Visible = _SF.DSTAppliesForLocalDate(theDate)

        lblSyncTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventSyncFlyDate.Value.Year, dtEventSyncFlyDate.Value.Month, dtEventSyncFlyDate.Value.Day, dtEventSyncFlyTime.Value.Hour, dtEventSyncFlyTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblSyncTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        lblLaunchTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventLaunchDate.Value.Year, dtEventLaunchDate.Value.Month, dtEventLaunchDate.Value.Day, dtEventLaunchTime.Value.Hour, dtEventLaunchTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblLaunchTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        lblStartTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventStartTaskDate.Value.Year, dtEventStartTaskDate.Value.Month, dtEventStartTaskDate.Value.Day, dtEventStartTaskTime.Value.Hour, dtEventStartTaskTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblStartTimeResult, $"{eventDay.ToString} - Right click for UNIX timestamp options")

        'BuildGroupFlightPost()

    End Sub

#End Region

#End Region

#Region "Discord Tab"

#Region "Discord - Flight Plan event handlers"

    Private Sub lblTaskBrowserIDAndDate_DoubleClick(sender As Object, e As EventArgs) Handles lblTaskBrowserIDAndDate.DoubleClick

        WeSimGlideTaskLinkPosting()

    End Sub

    Private Sub btnStartFullPostingWorkflow_Click(sender As Object, e As EventArgs) Handles btnStartFullPostingWorkflow.Click

        Dim enforceTaskLibrary As Boolean = True
        Dim autoContinue As Boolean = True

        If Not ValidPostingRequirements() Then
            Exit Sub
        End If

        'First part of Group Flight Event
        If Not FirstPartOfGroupPost(autoContinue) Then Exit Sub

        'Task in Task Library
        If Not PostTaskInLibrary(autoContinue) Then
            Exit Sub
        End If

        If Not SecondPartOfGroupPost(autoContinue) Then Exit Sub

    End Sub

    Private Sub btnDPORecallSettings_Click(sender As Object, e As EventArgs) Handles btnDPORecallSettings.Click

        LoadDPOptions()

    End Sub

    Private Sub btnDPORememberSettings_Click(sender As Object, e As EventArgs) Handles btnDPORememberSettings.Click
        SessionSettings.DPO_DPOUseCustomSettings = True
        SessionSettings.DPO_chkDPOMainPost = chkDPOMainPost.Checked
        SessionSettings.DPO_chkDPOThreadCreation = chkDPOThreadCreation.Checked
        SessionSettings.DPO_chkDPOIncludeCoverImage = chkDPOIncludeCoverImage.Checked
        SessionSettings.DPO_chkDPOFullDescription = chkDPOFullDescription.Checked
        SessionSettings.DPO_chkDPOFilesWithDescription = chkDPOFilesWithDescription.Checked
        SessionSettings.DPO_chkDPOFilesAlone = chkDPOFilesAlone.Checked
        SessionSettings.DPO_chkDPOAltRestrictions = chkDPOAltRestrictions.Checked
        SessionSettings.DPO_chkDPOWeatherInfo = chkDPOWeatherInfo.Checked
        SessionSettings.DPO_chkDPOWeatherChart = chkDPOWeatherChart.Checked
        SessionSettings.DPO_chkDPOWaypoints = chkDPOWaypoints.Checked
        SessionSettings.DPO_chkDPOAddOns = chkDPOAddOns.Checked
        SessionSettings.DPO_chkDPOResultsInvitation = chkDPOResultsInvitation.Checked
        SessionSettings.DPO_chkDPOFeaturedOnGroupFlight = chkDPOFeaturedOnGroupFlight.Checked
    End Sub

    Private Sub chkDPOFilesWithDescription_CheckedChanged(sender As Object, e As EventArgs) Handles chkDPOFilesWithDescription.CheckedChanged

        If chkDPOFilesWithDescription.Checked Then
            chkDPOFilesAlone.Checked = False
        End If

    End Sub

    Private Sub chkDPOFilesAlone_CheckedChanged(sender As Object, e As EventArgs) Handles chkDPOFilesAlone.CheckedChanged

        If chkDPOFilesAlone.Checked Then
            chkDPOFilesWithDescription.Checked = False
        End If

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

        chkDPOMainPost.Checked = True
        chkDPOThreadCreation.Checked = True
        chkDPOIncludeCoverImage.Checked = True
        chkDPOFullDescription.Checked = True
        chkDPOFilesWithDescription.Checked = True
        chkDPOFilesAlone.Checked = False
        chkDPOAltRestrictions.Checked = True
        chkDPOWeatherInfo.Checked = True
        chkDPOWeatherChart.Checked = True
        chkDPOWaypoints.Checked = True
        chkDPOAddOns.Checked = True
        chkDPOResultsInvitation.Checked = True
        chkDPOFeaturedOnGroupFlight.Checked = True

        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Sub btnStartTaskPost_Click(sender As Object, e As EventArgs) Handles btnStartTaskPost.Click

        Dim enforceTaskLibrary As Boolean = True
        Dim autoContinue As Boolean = True

        If Not ValidPostingRequirements() Then
            Exit Sub
        End If

        If Not PostTaskInLibrary(autoContinue) Then
            Exit Sub
        End If

    End Sub

#End Region

#Region "Discord - Flight Plan Subs & Functions"

    Private Sub WeSimGlideTaskLinkPosting()

        If _TBTaskEntrySeqID > 0 Then
            Dim msgWeSimGlideLink As String = String.Empty
            msgWeSimGlideLink = $"## 🌐 WeSimGlide.org {Environment.NewLine}[Task #{_TBTaskEntrySeqID.ToString.Trim} on WeSimGlide.org]({SupportingFeatures.GetWeSimGlideTaskURL(_TBTaskEntrySeqID)})"
            Clipboard.SetText(msgWeSimGlideLink)

            CopyContent.ShowContent(Me,
                                msgWeSimGlideLink,
                                "Task uploaded and database updated successfully! You can now paste the content of the message in the task's thread to share the WSG link.",
                                "Sharing WeSimGlide.org Task link",
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

        If chkDPOExpertMode.Checked Then
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, $"Automatic progression is enabled!{Environment.NewLine}{Environment.NewLine}This is still an experimental feature, several post actions will happen in succession without the possibility to stop them.{Environment.NewLine}{Environment.NewLine}Do you want to keep it enabled?", "Automatic progression confirmation request", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.Yes
                    'Nothing to do
                Case DialogResult.No
                    chkDPOExpertMode.Checked = False
                Case DialogResult.Cancel
                    Return False
            End Select
        End If

        If Not fromGroupOnly Then
            BuildFPResults()
            If HighlightExpectedFields(True) Then
                Return False
            End If
        End If

        Return True

    End Function

    Private Function PostTaskInGroupEventPartOne(autoContinue As Boolean) As Boolean

        Dim msg As String = String.Empty

        'Event Logistics
        If chkDGPOEventLogistics.Enabled AndAlso chkDGPOEventLogistics.Checked Then
            msg = $"{GroupFlightEventThreadLogistics()}"
        End If

        If chkDGPORelevantTaskDetails.Enabled AndAlso chkDGPORelevantTaskDetails.Checked Then

            'Other task options after files
            Dim altRestrictions As String = String.Empty
            If chkDGPOAltRestrictions.Enabled AndAlso chkDGPOAltRestrictions.Checked Then
                altRestrictions = txtAltRestrictions.Text.Trim
            End If
            Dim addOns As String = String.Empty
            If chkDGPOAddOns.Enabled AndAlso chkDGPOAddOns.Checked Then
                addOns = txtAddOnsDetails.Text.Trim
            End If

            msg = $"{msg}{BuildLightTaskDetailsForEventPost(altRestrictions.Length = 0,
                                                    addOns.Length = 0,
                                                    chkDGPOFilesWithFullLegend.Checked AndAlso (Not chkDGPODPHXOnly.Checked),
                                                    chkDGPOMainPost.Enabled AndAlso chkDGPOMainPost.Checked)}{Environment.NewLine}"
        End If

        'Main post
        If chkDGPOMainPost.Enabled AndAlso chkDGPOMainPost.Checked Then
            BuildFPResults(True)
            msg = $"{msg}{txtFPResults.Text.Trim}"
        End If

        Clipboard.SetText(msg)
        autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            "Paste the first message in the thread and post it.",
                            "Pasting first message in group event thread",
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked)

        If Not autoContinue Then Return False

        'Full Description
        autoContinue = PostTaskFullDescription(autoContinue, chkDGPOFullDescription, True)
        If Not autoContinue Then Return False

        Return True

    End Function

    Private Function PostTaskInGroupEventPartTwo(autoContinue As Boolean) As Boolean

        'Other task options after files
        Dim altRestrictions As String = String.Empty
        If chkDGPOAltRestrictions.Enabled AndAlso chkDGPOAltRestrictions.Checked Then
            altRestrictions = txtAltRestrictions.Text.Trim
        End If
        If altRestrictions.Length > 0 Then
            altRestrictions = $"{altRestrictions}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim completeWeather As String = String.Empty
        If chkDGPOWeatherInfo.Enabled AndAlso chkDGPOWeatherInfo.Checked Then
            completeWeather = txtWeatherFirstPart.Text.Trim & vbCrLf & vbCrLf & txtWeatherWinds.Text.Trim & vbCrLf & vbCrLf & txtWeatherClouds.Text.Trim
        End If
        If completeWeather.Length > 0 Then
            completeWeather = $"{completeWeather}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim waypoints As String = String.Empty
        If chkDGPOWaypoints.Enabled AndAlso chkDGPOWaypoints.Checked Then
            waypoints = txtWaypointsDetails.Text.Trim
        End If
        If waypoints.Length > 0 Then
            waypoints = $"{waypoints}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim addOns As String = String.Empty
        If chkDGPOAddOns.Enabled AndAlso chkDGPOAddOns.Checked Then
            addOns = txtAddOnsDetails.Text.Trim
        End If
        If addOns.Length > 0 Then
            addOns = $"{addOns}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim msg As String = String.Empty
        'If the weather chart is included, do Restrictions and Weather together then chart, then rest of details together.
        'If weather chart is not included, do everything together.
        If chkDGPOWeatherChart.Enabled AndAlso chkDGPOWeatherChart.Checked Then
            msg = $"{altRestrictions}{completeWeather}"
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                                msg,
                                $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the content of your clipboard as the next message in the group event's thread.",
                                "Creating altitude restrictions and weather details post in the thread.",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked)
            If Not autoContinue Then
                Return False
            End If
            'Weather Chart
            Dim chartImage As Drawing.Image = CopyWeatherGraphToClipboard()
            autoContinue = CopyContent.ShowContent(Me,
                                "Weather chart",
                                $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the image of the weather chart as the next message in the group event's thread.",
                                "Creating weather chart post in the thread.",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked,
                                True,
                                numWaitSecondsForFiles.Value / 2 * 1000,
                                chartImage)
            If Not autoContinue Then
                Return False
            End If
        End If

        If addOns.Trim.Length > 0 Then
            'There are add-ons, we must post them without the rest so the embeds are created right under
            If msg.Trim.Length = 0 Then
                msg = $"{altRestrictions}{completeWeather}{waypoints}{addOns}"
            Else
                msg = $"{waypoints}{addOns}"
            End If
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the content of your clipboard as the next message in the group event's thread.",
                            "Creating remaining details up to add-ons to post in the thread.",
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked)
            If Not autoContinue Then
                Return False
            End If
            msg = String.Empty
        Else
            'No add-ons, we can post everything remaining all at once
            If msg.Trim.Length = 0 Then
                msg = $"{altRestrictions}{completeWeather}{waypoints}{addOns}"
            Else
                msg = $"{waypoints}{addOns}"
            End If
        End If

        'Remaining details
        If msg.Trim.Length > 0 Then
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the content of your clipboard as the next message in the group event's thread.",
                            "Creating all remaining details post in the thread.",
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked)
        End If
        If Not autoContinue Then
            Return False
        End If

        Return True

    End Function

    Private Function PostTaskInLibrary(autoContinue As Boolean, Optional enforceTaskLibrary As Boolean = True) As Boolean

        'Task Main Post
        If chkDPOMainPost.Enabled AndAlso chkDPOMainPost.Checked Then
            autoContinue = FlightPlanMainInfoCopy()
        End If
        If Not autoContinue Then
            Return False
        End If

        'Are we enforcing the posting on the Task Library only?
        If txtDiscordTaskID.Text = String.Empty Then
            If enforceTaskLibrary Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Task ID is missing - You must post the task to the Task Library!", "Task ID missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return False
            Else
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show(Me, "Task ID is missing - Are you sure you want to proceed?", "Task ID missing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Else
                        Return False
                    End If
                End Using
            End If
        End If

        'Thread Creation
        If chkDPOThreadCreation.Enabled AndAlso chkDPOThreadCreation.Checked Then
            autoContinue = CreateTaskThread()
        End If
        If Not autoContinue Then
            Return False
        End If

        'Do we have both IDs before we can continue?
        If lblThread1stMsgIDNotAcquired.Visible Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "ID of the task thread or first message in the thread is missing - You must post the task to the Task Library!", "Thread ID missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End If

        'Cover Image
        If chkDPOIncludeCoverImage.Enabled AndAlso chkDPOIncludeCoverImage.Checked Then
            autoContinue = CoverImage()
        End If
        If Not autoContinue Then
            Return False
        End If

        'Full Description
        If Not PostTaskFullDescription(autoContinue, chkDPOFullDescription, False) Then
            Return False
        End If

        'Files
        If (chkDPOFilesWithDescription.Enabled AndAlso chkDPOFilesWithDescription.Checked) OrElse (chkDPOFilesAlone.Enabled AndAlso chkDPOFilesAlone.Checked) Then
            autoContinue = FilesCopy()
            If autoContinue Then
                'Files text (description or simple Files heading)
                autoContinue = FilesTextCopy(chkDPOFilesWithDescription.Checked)
            Else
                Return False
            End If
        End If

        Dim altRestrictions As String = String.Empty
        If chkDPOAltRestrictions.Enabled AndAlso chkDPOAltRestrictions.Checked Then
            altRestrictions = txtAltRestrictions.Text.Trim
        End If
        If altRestrictions.Length > 0 Then
            altRestrictions = $"{altRestrictions}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim completeWeather As String = String.Empty
        If chkDPOWeatherInfo.Enabled AndAlso chkDPOWeatherInfo.Checked Then
            completeWeather = txtWeatherFirstPart.Text.Trim & vbCrLf & vbCrLf & txtWeatherWinds.Text.Trim & vbCrLf & vbCrLf & txtWeatherClouds.Text.Trim
        End If
        If completeWeather.Length > 0 Then
            completeWeather = $"{completeWeather}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim waypoints As String = String.Empty
        If chkDPOWaypoints.Enabled AndAlso chkDPOWaypoints.Checked Then
            waypoints = txtWaypointsDetails.Text.Trim
        End If
        If waypoints.Length > 0 Then
            waypoints = $"{waypoints}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim addOns As String = String.Empty
        If chkDPOAddOns.Enabled AndAlso chkDPOAddOns.Checked Then
            addOns = txtAddOnsDetails.Text.Trim
        End If
        If addOns.Length > 0 Then
            addOns = $"{addOns}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim invitation As String = String.Empty
        If chkDPOResultsInvitation.Enabled AndAlso chkDPOResultsInvitation.Checked Then
            invitation = ResultsCopy()
        End If
        If invitation.Length > 0 Then
            invitation = $"{invitation}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim taskFeatured As String = String.Empty
        If chkDPOFeaturedOnGroupFlight.Enabled AndAlso chkDPOFeaturedOnGroupFlight.Checked Then
            taskFeatured = TaskFeatureOnGroupFlight()
        End If
        If taskFeatured.Length > 0 Then
            taskFeatured = $"{taskFeatured}{Environment.NewLine}{Environment.NewLine}"
        End If

        Dim msg As String = String.Empty
        'If the weather chart is included, do Restrictions and Weather together then chart, then rest of details together.
        'If weather chart is not included, do everything together.
        If (chkDPOWeatherChart.Enabled AndAlso chkDPOWeatherChart.Checked) Then
            If (altRestrictions.Length + completeWeather.Length > 0) Then
                msg = $"{altRestrictions}{completeWeather}"
                Clipboard.SetText(msg)
                autoContinue = CopyContent.ShowContent(Me,
                                msg,
                                $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the content of your clipboard as the next message in the task's thread.",
                                "Creating altitude restrictions and weather details post in the thread.",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked)
                If Not autoContinue Then
                    Return False
                End If
            End If
            'Weather Chart
            Dim chartImage As Drawing.Image = CopyWeatherGraphToClipboard()
            autoContinue = CopyContent.ShowContent(Me,
                                "Weather chart",
                                $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the image of the weather chart as the next message in the task's thread.",
                                "Creating weather chart post in the thread.",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked,
                                True,
                                numWaitSecondsForFiles.Value / 2 * 1000,
                                chartImage)
            If Not autoContinue Then
                Return False
            End If
        End If

        If addOns.Trim.Length > 0 Then
            'There are add-ons, we must post them without the rest so the embeds are created right under
            If msg.Trim.Length = 0 Then
                msg = $"{altRestrictions}{completeWeather}{waypoints}{addOns}"
            Else
                msg = $"{waypoints}{addOns}"
            End If
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the content of your clipboard as the next message in the task's thread.",
                            "Creating remaining details up to add-ons to post in the thread.",
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked)
            If Not autoContinue Then
                Return False
            End If
            msg = $"{invitation}{taskFeatured}"
        Else
            'No add-ons, we can post everything remaining all at once
            If msg.Trim.Length = 0 Then
                msg = $"{altRestrictions}{completeWeather}{waypoints}{addOns}{invitation}{taskFeatured}"
            Else
                msg = $"{waypoints}{addOns}{invitation}{taskFeatured}"
            End If
        End If

        'Remaining details
        If msg.Trim.Length > 0 Then
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                            msg,
                            $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the content of your clipboard as the next message in the task's thread.",
                            "Creating all remaining details post in the thread.",
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked)
        End If
        If Not autoContinue Then
            Return False
        End If

        Return True

    End Function

    Private Function PostTaskFullDescription(autoContinue As Boolean, checkOptionFullDescription As Windows.Forms.CheckBox, fromGroup As Boolean) As Boolean

        If checkOptionFullDescription.Enabled AndAlso checkOptionFullDescription.Checked Then
            autoContinue = FullDescriptionCopy(fromGroup)
        End If
        If Not autoContinue Then
            Return False
        End If

        Return True

    End Function

    Private Sub SetWarningIconOnPostOptions()

        Dim listOfControlsRemove As New List(Of Windows.Forms.CheckBox)
        Dim listOfControlsAdd As New List(Of Windows.Forms.CheckBox)

        If lblTaskLibraryIDAcquired.Visible Then
            listOfControlsRemove.Add(chkDPOThreadCreation)
            listOfControlsRemove.Add(chkDPOIncludeCoverImage)
            listOfControlsRemove.Add(chkDPOFullDescription)
            If lblThread1stMsgIDAcquired.Visible Then
                'Task
                listOfControlsRemove.Add(chkDPOFilesWithDescription)
                listOfControlsRemove.Add(chkDPOFilesAlone)
                listOfControlsRemove.Add(chkDPOAltRestrictions)
                listOfControlsRemove.Add(chkDPOWeatherInfo)
                listOfControlsRemove.Add(chkDPOWeatherChart)
                listOfControlsRemove.Add(chkDPOWaypoints)
                listOfControlsRemove.Add(chkDPOAddOns)
                listOfControlsRemove.Add(chkDPOResultsInvitation)
                listOfControlsRemove.Add(chkDPOFeaturedOnGroupFlight)
                'Group
                listOfControlsRemove.Add(chkDGPOMainPost)
                listOfControlsRemove.Add(chkDGPOFullDescription)
                listOfControlsRemove.Add(chkDGPOFilesWithFullLegend)
                listOfControlsRemove.Add(chkDGPODPHXOnly)
                listOfControlsRemove.Add(chkDGPOFilesWithoutLegend)
                listOfControlsRemove.Add(chkDGPOAltRestrictions)
                listOfControlsRemove.Add(chkDGPOWeatherInfo)
                listOfControlsRemove.Add(chkDGPOWeatherChart)
                listOfControlsRemove.Add(chkDGPOWaypoints)
                listOfControlsRemove.Add(chkDGPOAddOns)
                listOfControlsRemove.Add(chkDGPORelevantTaskDetails)
                listOfControlsRemove.Add(chkDGPOEventLogistics)
            Else
                'Task
                listOfControlsAdd.Add(chkDPOFilesWithDescription)
                listOfControlsAdd.Add(chkDPOFilesAlone)
                listOfControlsAdd.Add(chkDPOAltRestrictions)
                listOfControlsAdd.Add(chkDPOWeatherInfo)
                listOfControlsAdd.Add(chkDPOWeatherChart)
                listOfControlsAdd.Add(chkDPOWaypoints)
                listOfControlsAdd.Add(chkDPOAddOns)
                listOfControlsAdd.Add(chkDPOResultsInvitation)
                listOfControlsAdd.Add(chkDPOFeaturedOnGroupFlight)
                'Group
                listOfControlsAdd.Add(chkDGPOMainPost)
                listOfControlsAdd.Add(chkDGPOFullDescription)
                listOfControlsAdd.Add(chkDGPOFilesWithFullLegend)
                listOfControlsAdd.Add(chkDGPODPHXOnly)
                listOfControlsAdd.Add(chkDGPOFilesWithoutLegend)
                listOfControlsAdd.Add(chkDGPOAltRestrictions)
                listOfControlsAdd.Add(chkDGPOWeatherInfo)
                listOfControlsAdd.Add(chkDGPOWeatherChart)
                listOfControlsAdd.Add(chkDGPOWaypoints)
                listOfControlsAdd.Add(chkDGPOAddOns)
                listOfControlsAdd.Add(chkDGPORelevantTaskDetails)
                listOfControlsAdd.Add(chkDGPOEventLogistics)
            End If
        Else
            'Task
            listOfControlsAdd.Add(chkDPOThreadCreation)
            listOfControlsAdd.Add(chkDPOIncludeCoverImage)
            listOfControlsAdd.Add(chkDPOFullDescription)
            listOfControlsAdd.Add(chkDPOFilesWithDescription)
            listOfControlsAdd.Add(chkDPOFilesAlone)
            listOfControlsAdd.Add(chkDPOAltRestrictions)
            listOfControlsAdd.Add(chkDPOWeatherInfo)
            listOfControlsAdd.Add(chkDPOWeatherChart)
            listOfControlsAdd.Add(chkDPOWaypoints)
            listOfControlsAdd.Add(chkDPOAddOns)
            listOfControlsAdd.Add(chkDPOResultsInvitation)
            listOfControlsAdd.Add(chkDPOFeaturedOnGroupFlight)
            'Group
            listOfControlsAdd.Add(chkDGPOMainPost)
            listOfControlsAdd.Add(chkDGPOFullDescription)
            listOfControlsAdd.Add(chkDGPOFilesWithFullLegend)
            listOfControlsAdd.Add(chkDGPODPHXOnly)
            listOfControlsAdd.Add(chkDGPOFilesWithoutLegend)
            listOfControlsAdd.Add(chkDGPOAltRestrictions)
            listOfControlsAdd.Add(chkDGPOWeatherInfo)
            listOfControlsAdd.Add(chkDGPOWeatherChart)
            listOfControlsAdd.Add(chkDGPOWaypoints)
            listOfControlsAdd.Add(chkDGPOAddOns)
            listOfControlsAdd.Add(chkDGPORelevantTaskDetails)
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
                optionCheckBox.ForeColor = lblTaskLibraryIDNotAcquired.ForeColor
            End If
        Next

    End Sub

    Private Sub CheckWhichOptionsCanBeEnabled()

        'Is there a group selected for event news buttons
        btnPublishEventNews.Enabled = False
        btnDeleteEventNews.Enabled = False
        If txtClubFullName.Text.Trim <> String.Empty Then
            btnPublishEventNews.Enabled = UserCanCreateEvent
            btnDeleteEventNews.Enabled = UserCanDeleteEvent
        End If

        If txtTitle.Text.Trim.Length > 0 Then
            btnEventDPHXAndLinkOnly.Enabled = True
        Else
            btnEventDPHXAndLinkOnly.Enabled = False
        End If

        chkDPOMainPost.Enabled = grbTaskInfo.Enabled
        chkDPOThreadCreation.Enabled = grbTaskInfo.Enabled
        chkDPOFilesAlone.Enabled = grbTaskInfo.Enabled
        chkDPOFilesWithDescription.Enabled = grbTaskInfo.Enabled
        chkDPOWaypoints.Enabled = grbTaskInfo.Enabled

        If cboCoverImage.SelectedItem IsNot Nothing AndAlso cboCoverImage.SelectedItem.ToString <> String.Empty Then
            chkDPOIncludeCoverImage.Enabled = True
            chkDGPOCoverImage.Enabled = True
        Else
            chkDPOIncludeCoverImage.Enabled = False
            chkDGPOCoverImage.Enabled = False
        End If

        If txtLongDescription.Text.Trim.Length = 0 Then
            chkDPOFullDescription.Enabled = False
        Else
            chkDPOFullDescription.Enabled = True
        End If

        If txtAltRestrictions.Text.Trim.Length = 0 Then
            chkDPOAltRestrictions.Enabled = False
        Else
            chkDPOAltRestrictions.Enabled = True AndAlso grbTaskInfo.Enabled
        End If

        If txtWeatherFile.Text.Trim.Length + txtWeatherSummary.Text.Trim.Length = 0 Then
            chkDPOWeatherInfo.Enabled = False
        Else
            chkDPOWeatherInfo.Enabled = True AndAlso grbTaskInfo.Enabled
        End If

        If txtWeatherFile.Text.Trim.Length = 0 Then
            chkDPOWeatherChart.Enabled = False
        Else
            chkDPOWeatherChart.Enabled = True AndAlso grbTaskInfo.Enabled
        End If

        If txtAddOnsDetails.Text.Trim.Length = 0 Then
            chkDPOAddOns.Enabled = False
        Else
            chkDPOAddOns.Enabled = True AndAlso grbTaskInfo.Enabled
        End If

        If txtDiscordTaskID.Text.Trim = String.Empty Then
            chkDPOResultsInvitation.Enabled = False
        Else
            chkDPOResultsInvitation.Enabled = True AndAlso grbTaskInfo.Enabled
        End If

        If _ClubPreset IsNot Nothing AndAlso
            txtDiscordEventShareURL.Text.Trim.Length + txtGroupEventPostURL.Text.Trim.Length > 0 Then
            chkDPOFeaturedOnGroupFlight.Enabled = True AndAlso grbTaskInfo.Enabled
        Else
            chkDPOFeaturedOnGroupFlight.Enabled = False
        End If

        'Group Event
        If chkEventTeaser.Checked AndAlso (txtEventTeaserAreaMapImage.Text.Trim.Length > 0 OrElse txtEventTeaserMessage.Text.Trim.Length > 0) Then
            chkDGPOTeaser.Enabled = True
        Else
            chkDGPOTeaser.Checked = False
            chkDGPOTeaser.Enabled = False
        End If
        chkDGPOFilesWithoutLegend.Enabled = chkDPOFilesAlone.Enabled
        chkDGPOFilesWithFullLegend.Enabled = chkDPOFilesWithDescription.Enabled
        chkDGPOMainPost.Enabled = grbTaskInfo.Enabled
        chkDGPOFullDescription.Enabled = chkDPOFullDescription.Enabled
        chkDGPOAltRestrictions.Enabled = chkDPOAltRestrictions.Enabled
        chkDGPOWeatherInfo.Enabled = chkDPOWeatherInfo.Enabled
        chkDGPOWeatherChart.Enabled = chkDPOWeatherChart.Enabled
        chkDGPOAddOns.Enabled = chkDPOAddOns.Enabled
        chkDGPOWaypoints.Enabled = chkDPOWaypoints.Enabled

        chkDGPORelevantTaskDetails.Enabled = grbTaskInfo.Enabled

        chkDGPODPHXOnly.Enabled = (chkDGPOFilesWithoutLegend.Checked Or chkDGPOFilesWithFullLegend.Checked)

        If Not chkDGPOTeaser.Checked AndAlso
           Not chkDGPOMainPost.Checked AndAlso
           Not chkDGPOFullDescription.Checked AndAlso
           Not chkDGPOAltRestrictions.Checked AndAlso
           Not chkDGPOWeatherInfo.Checked AndAlso
           Not chkDGPOWeatherChart.Checked AndAlso
           Not chkDGPOAddOns.Checked AndAlso
           Not chkDGPOWaypoints.Checked AndAlso
           Not chkDGPOFilesWithFullLegend.Checked AndAlso
           Not chkDGPOFilesWithoutLegend.Checked AndAlso
           Not chkDGPORelevantTaskDetails.Checked AndAlso
           Not chkDGPOEventLogistics.Checked Then
            chkDGPOThreadCreation.Enabled = False
        Else
            chkDGPOThreadCreation.Enabled = True
        End If

        SetWarningIconOnPostOptions()

    End Sub

    Private Function FlightPlanMainInfoCopy(Optional fromGroup As Boolean = False) As Boolean

        Dim autoContinue As Boolean = True
        Dim origin As String = String.Empty
        Dim titleMsg As String = String.Empty

        BuildFPResults(fromGroup)

        If Not fromGroup Then
            autoContinue = MsgBoxWithPicture.ShowContent(Me,
                                                     "StartTaskWorkflow.gif",
                                                     "Please open the Discord app and position your cursor as shown below",
                                                     "Once your cursor is in the right field, you can click OK and start posting.",
                                                     "Instructions to read before starting the post!")

            If Not autoContinue Then
                Return autoContinue
            End If
            origin = "You can now post the main flight plan message directly in the FLIGHTS channel under TASK LIBRARY."
            titleMsg = "Creating main FP post"
        Else
            origin = "You can now post the main flight plan message under the group event's thread."
            titleMsg = "Creating FP post for group"
        End If

        Clipboard.SetText(txtFPResults.Text)

        autoContinue = CopyContent.ShowContent(Me,
                            txtFPResults.Text,
                            $"{origin}{Environment.NewLine}Skip (Ok) if already done.",
                            titleMsg,
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked)

        If Not autoContinue OrElse fromGroup Then
            Return autoContinue
        End If

        If txtDiscordTaskID.Text = String.Empty Then
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
                    taskThreadURL = Clipboard.GetText
                    txtDiscordTaskID.Text = SupportingFeatures.ExtractMessageIDFromDiscordURL(taskThreadURL)
                    If txtDiscordTaskID.Text.Trim.Length = 0 Then
                        Using New Centered_MessageBox(Me)
                            If MessageBox.Show(Me, $"Invalid task ID - If you posted under the Task Library, try again. If not, click Cancel.", "Task ID missing", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                validTaskIDOrCancel = True
                                autoContinue = False
                            End If
                        End Using
                    Else
                        _taskThreadFirstPostID = String.Empty
                        validTaskIDOrCancel = True
                        SaveSession()
                    End If
                Else
                    validTaskIDOrCancel = True
                    autoContinue = False
                End If
                If Not autoContinue Then
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me, $"Task ID is missing - You should be posting your task to the Task Library and get the link to that post!{Environment.NewLine}", "Task ID missing", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End If
            Loop
        End If

        Return autoContinue

    End Function

    Private Sub GetTaskThreadFirstPostID()

        If txtDiscordTaskID.Text = String.Empty Then
            Exit Sub
        End If

        If _taskThreadFirstPostID = String.Empty Then
            Dim message As String = "Please get the link to the anchor message from the task thread in Discord (""...More menu"" and ""Copy Message Link"")"
            Dim waitingForm As WaitingForURLForm
            Dim answer As DialogResult
            Dim validTaskThreadFirstPostIDOrCancel As Boolean = False

            Do Until validTaskThreadFirstPostIDOrCancel
                Clipboard.Clear()
                waitingForm = New WaitingForURLForm(message)
                answer = waitingForm.ShowDialog()

                SupportingFeatures.BringDPHToolToTop(Me.Handle)
                'Check if the clipboard contains a valid URL, which would mean the task's URL has been copied
                If answer = DialogResult.OK Then
                    Dim threadFirstPostID As String
                    threadFirstPostID = Clipboard.GetText
                    _taskThreadFirstPostID = SupportingFeatures.ExtractMessageIDFromDiscordURL(threadFirstPostID, False, txtDiscordTaskID.Text)
                    If _taskThreadFirstPostID.Trim.Length = 0 Then
                        Using New Centered_MessageBox(Me)
                            If MessageBox.Show(Me, $"Invalid message ID - You must get the ID to a message inside the task's thread! To skip and link to the thread instead, click Cancel.", "Message ID missing", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                _taskThreadFirstPostID = txtDiscordTaskID.Text
                                validTaskThreadFirstPostIDOrCancel = True
                            End If
                        End Using
                    Else
                        validTaskThreadFirstPostIDOrCancel = True
                        SaveSession()
                    End If
                Else
                    validTaskThreadFirstPostIDOrCancel = True
                    Using New Centered_MessageBox(Me)
                        _taskThreadFirstPostID = txtDiscordTaskID.Text
                        MessageBox.Show(Me, $"Thread anchor message ID is missing - Redirection will be done directly to the thread instead!", "Thread anchor message ID missing", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End If
            Loop
        End If

        If _taskThreadFirstPostID = String.Empty Then
            lblThread1stMsgIDNotAcquired.Visible = True
            lblThread1stMsgIDAcquired.Visible = False
        Else
            lblThread1stMsgIDAcquired.Visible = True
            lblThread1stMsgIDNotAcquired.Visible = False
        End If
        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Function CreateTaskThread() As Boolean

        Dim autoContinue As Boolean = True

        Dim fpTitle As String = $"{txtTitle.Text}{AddFlagsToTitle()}"
        Clipboard.SetText(fpTitle)

        autoContinue = MsgBoxWithPicture.ShowContent(Me,
                                                     "CreateTaskThread.gif",
                                                     "Follow the instructions as shown below to create the task's thread.",
                                                     "ONLY once you've created the thread, pasted its name in THREAD NAME and positionned your cursor on the thread's message field, can you click OK and resume the workflow.",
                                                     "Instructions for the creation of the task's thread!")

        If autoContinue Then
            Dim msg As String = $"## 🧵 Task Details Thread{Environment.NewLine}Refer to this thread for comprehensive information, resources, and updates related to this task. This first message serves as your anchor point for all details in this thread."
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                                msg,
                                $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the thread anchor as the very first message in the thread.",
                                "Creating thread anchor post.",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked)

            If autoContinue Then
                _taskThreadFirstPostID = String.Empty
                lblThread1stMsgIDNotAcquired.Visible = True
                lblThread1stMsgIDAcquired.Visible = False
                GetTaskThreadFirstPostID()
            End If

        End If

        Return autoContinue

    End Function

    Private Function CoverImage(Optional postAfterPasting As Boolean = True) As Boolean

        Dim autoContinue As Boolean = True

        If cboCoverImage.SelectedItem IsNot Nothing AndAlso cboCoverImage.SelectedItem.ToString <> String.Empty Then
            Dim allFiles As New StringCollection
            If File.Exists(cboCoverImage.SelectedItem) Then
                allFiles.Add(cboCoverImage.SelectedItem)
                Clipboard.SetFileDropList(allFiles)
                autoContinue = CopyContent.ShowContent(Me,
                                    cboCoverImage.SelectedItem,
                                    $"On the Discord app, make sure you are on the proper channel and message field.{Environment.NewLine}Now paste the copied cover image as message.{Environment.NewLine}Skip (Ok) if already done.",
                                    "Posting the cover image for the task.",
                                    New List(Of String) From {"^v"},
                                    chkDPOExpertMode.Checked,
                                    postAfterPasting,
                                    If(postAfterPasting, numWaitSecondsForFiles.Value / 2 * 1000, 0),
                                    Drawing.Image.FromFile(allFiles(0)))
            Else
                autoContinue = True
            End If
        Else
            autoContinue = True
        End If

        Return autoContinue

    End Function

    Private Function FullDescriptionCopy(fromGroup As Boolean) As Boolean

        Dim autoContinue As Boolean = True

        Dim origin As String
        If fromGroup Then
            origin = "group event"
        Else
            origin = "task"
        End If

        Clipboard.SetText(txtFullDescriptionResults.Text.Trim)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFullDescriptionResults.Text.Trim,
                                $"Make sure you are on the thread's message field.{Environment.NewLine}Then post the full description as the next message in the {origin}'s thread.",
                                "Creating full description post in the thread.",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked)
        If fromGroup Then
            Return autoContinue
        End If

        Return autoContinue

    End Function

    Private Function FilesCopy(Optional postingGroupEvent As Boolean = False) As Boolean

        Dim autoContinue As Boolean = True
        Dim dlgResult As DialogResult

        Do While _sessionModified
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, "Latest changes have not been saved! You first need to save the session.", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.OK
                    Dim e As New EventArgs
                    btnSaveConfig_Click(Nothing, e)
                Case DialogResult.Cancel
                    Return False
            End Select
        Loop

        Dim allFiles As New Specialized.StringCollection
        Dim contentForMessage As New StringBuilder
        If postingGroupEvent Then
            GetAllFilesForMessage(allFiles, contentForMessage, chkDGPODPHXOnly.Checked)
        Else
            GetAllFilesForMessage(allFiles, contentForMessage)
        End If

        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    contentForMessage.ToString,
                                    $"Make sure you are on the thread's message field.{Environment.NewLine}Now paste the copied files as the next message in the thread WITHOUT posting it and come back for the text for this message.",
                                    "Creating the files post in the thread - actual files first",
                                    New List(Of String) From {"^v"},
                                    chkDPOExpertMode.Checked,
                                    False)
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "No files to copy!", "Creating the files post in the thread - actual files first", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using
            autoContinue = False
        End If

        Return autoContinue

    End Function

    Private Function GetDiscordLinkToTaskThread() As String
        Dim urlToTaskThread As String = String.Empty
        If _taskThreadFirstPostID = String.Empty OrElse _taskThreadFirstPostID = txtDiscordTaskID.Text Then
            urlToTaskThread = $"https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{txtDiscordTaskID.Text}"
        Else
            urlToTaskThread = $"https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{txtDiscordTaskID.Text}/{_taskThreadFirstPostID}"
        End If
        Return urlToTaskThread
    End Function

    Private Sub GetAllFilesForMessage(allFiles As StringCollection, contentForMessage As StringBuilder, Optional DPHXOnly As Boolean = False)
        contentForMessage.AppendLine("FILES")
        If File.Exists(txtDPHXPackageFilename.Text) Then
            allFiles.Add(txtDPHXPackageFilename.Text)
            contentForMessage.AppendLine(txtDPHXPackageFilename.Text)
        End If
        If Not DPHXOnly Then

            If File.Exists(txtFlightPlanFile.Text) Then
                allFiles.Add(txtFlightPlanFile.Text)
                contentForMessage.AppendLine(txtFlightPlanFile.Text)
            End If
            If File.Exists(txtWeatherFile.Text) Then
                allFiles.Add(txtWeatherFile.Text)
                contentForMessage.AppendLine(txtWeatherFile.Text)
            End If

            For i = 0 To lstAllFiles.Items.Count() - 1
                If File.Exists(lstAllFiles.Items(i)) AndAlso lstAllFiles.Items(i) <> cboCoverImage.Text Then
                    allFiles.Add(lstAllFiles.Items(i))
                    contentForMessage.AppendLine(lstAllFiles.Items(i))
                End If
            Next
        End If

    End Sub

    Private Function FilesTextCopy(fullLegend As Boolean, Optional postingGroupEvent As Boolean = False) As Boolean

        Dim autoContinue As Boolean = True

        If postingGroupEvent Then
            BuildFileInfoText(fullLegend, chkDGPODPHXOnly.Checked)
        Else
            BuildFileInfoText(fullLegend)
        End If
        Clipboard.SetText(txtFilesText.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFilesText.Text,
                                "Now enter the file info in the current message in the thread and post it.",
                                "Creating the files post in the thread - file info",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked,
                                True,
                                numWaitSecondsForFiles.Value * 1000)

        Return autoContinue

    End Function

    Private Sub BuildFileInfoText(fullLegend As Boolean, Optional DPHXOnly As Boolean = False)
        Dim sb As New StringBuilder
        sb.AppendLine("## 📁 **Files**")

        If Not fullLegend Then
            txtFilesText.Text = sb.ToString.Trim
            sb.Clear()
            Exit Sub
        End If

        'Check if the DPHX package is included
        If File.Exists(txtDPHXPackageFilename.Text) Then
            sb.AppendLine("### DPHX Unpack & Load")
            sb.AppendLine("> Simply download the included **.DPHX** package and double-click it.")
            sb.AppendLine("> *To get and install the tool [click this link](https://flightsim.to/file/62573/msfs-soaring-task-tools-dphx-unpack-load)*")
            sb.AppendLine("> ")
            If DPHXOnly Then
                sb.AppendLine("> Otherwise, visit the task's thread and download the required files and put them in the proper folders.")
            Else
                sb.AppendLine("> Otherwise, you must download the required files and put them in the proper folders.")
            End If
        Else
            sb.AppendLine("You must download the required files and put them in the proper folders.")
        End If

        If Not DPHXOnly Then

            sb.AppendLine("### Required")
            sb.AppendLine($"> Flight plan: **""{Path.GetFileName(txtFlightPlanFile.Text)}""**")
            sb.AppendLine($"> Weather file & profile name: **""{Path.GetFileName(txtWeatherFile.Text)}"" ({_WeatherDetails.PresetName})**")

            'Check if there is a tsk file in the files
            Dim optionalAdded As Boolean = False
            For i = 0 To lstAllFiles.Items.Count() - 1
                If File.Exists(lstAllFiles.Items(i)) AndAlso Path.GetExtension(lstAllFiles.Items(i)) = ".tsk" Then
                    If Not optionalAdded Then
                        sb.AppendLine("### XCSoar Files - Optional")
                        sb.AppendLine("> *Only if you use the XCSoar program.*")
                        optionalAdded = True
                    End If
                    sb.AppendLine("> XCSoar Task (.tsk)")
                ElseIf File.Exists(lstAllFiles.Items(i)) AndAlso Path.GetExtension(lstAllFiles.Items(i)) = ".xcm" Then
                    If Not optionalAdded Then
                        sb.AppendLine("### XCSoar Files - Optional")
                        sb.AppendLine("> *Only if you use the XCSoar program.*")
                        optionalAdded = True
                    End If
                    sb.AppendLine("> XCSoar Map (.xcm)")
                End If
            Next
        End If

        txtFilesText.Text = sb.ToString.Trim
        sb.Clear()
    End Sub

    Private Function ResultsCopy() As String

        Dim msg As String = String.Empty

        'Results
        If Not txtDiscordTaskID.Text.Trim = String.Empty Then
            msg = $"{msg}## 🏁 Results{Environment.NewLine}Feel free to share your task results in this thread, creating a central spot for everyone's achievements."
        End If

        Return msg

    End Function

    Private Function TaskFeatureOnGroupFlight() As String

        Dim sb As New StringBuilder

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)

        sb.AppendLine("## :calendar: Group Flight")
        sb.AppendLine($"This flight will be featured on the {txtClubFullName.Text} group flight of {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time.")

        'check which shared link is available
        If txtDiscordEventShareURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtDiscordEventShareURL.Text.Trim) Then
            sb.AppendLine($"{txtDiscordEventShareURL.Text}")
        ElseIf txtGroupEventPostURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtGroupEventPostURL.Text.Trim) Then
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
        SessionSettings.DPO_chkDGPOFilesWithFullLegend = chkDGPOFilesWithFullLegend.Checked
        SessionSettings.DPO_chkDGPOFilesWithoutLegend = chkDGPOFilesWithoutLegend.Checked
        SessionSettings.DPO_chkDGPODPHXOnly = chkDGPODPHXOnly.Checked
        SessionSettings.DPO_chkDGPOMainPost = chkDGPOMainPost.Checked
        SessionSettings.DPO_chkDGPOFullDescription = chkDGPOFullDescription.Checked
        SessionSettings.DPO_chkDGPOAltRestrictions = chkDGPOAltRestrictions.Checked
        SessionSettings.DPO_chkDGPOWeatherInfo = chkDGPOWeatherInfo.Checked
        SessionSettings.DPO_chkDGPOWeatherChart = chkDGPOWeatherChart.Checked
        SessionSettings.DPO_chkDGPOWaypoints = chkDGPOWaypoints.Checked
        SessionSettings.DPO_chkDGPOAddOns = chkDGPOAddOns.Checked
        SessionSettings.DPO_chkDGPORelevantTaskDetails = chkDGPORelevantTaskDetails.Checked
        SessionSettings.DPO_chkDGPOEventLogistics = chkDGPOEventLogistics.Checked

    End Sub


    Private Sub grpDiscordGroupFlight_EnabledChanged(sender As Object, e As EventArgs) Handles grpDiscordGroupFlight.EnabledChanged
        btnTaskFeaturedOnGroupFlight.Enabled = grpDiscordGroupFlight.Enabled
    End Sub

    Private Sub btnDGPOResetToDefault_Click(sender As Object, e As EventArgs) Handles btnDGPOResetToDefault.Click

        chkDGPOCoverImage.Checked = True
        chkDGPOMainGroupPost.Checked = True
        chkDGPOThreadCreation.Checked = True
        chkDGPOTeaser.Checked = False
        chkDGPOFilesWithFullLegend.Checked = True
        chkDGPOFilesWithoutLegend.Checked = False

        chkDGPOMainPost.Checked = False
        chkDGPOFullDescription.Checked = False
        chkDGPOAltRestrictions.Checked = False
        chkDGPOWeatherInfo.Checked = False
        chkDGPOWeatherChart.Checked = False
        chkDGPOWaypoints.Checked = False
        chkDGPOAddOns.Checked = False

        chkDGPORelevantTaskDetails.Checked = True
        chkDGPOEventLogistics.Checked = True

        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Sub btnDiscordGroupEventURL_Click(sender As Object, e As EventArgs) Handles btnDiscordGroupEventURL.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtGroupEventPostURL.Text = Clipboard.GetText
        End If
    End Sub

    Private Sub btnRepostOriginalURLPaste_Click(sender As Object, e As EventArgs) Handles btnRepostOriginalURLPaste.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtRepostOriginalURL.Text = Clipboard.GetText
        End If
    End Sub

    Private Sub btnDiscordSharedEventURL_Click(sender As Object, e As EventArgs) Handles btnDiscordSharedEventURL.Click

        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtDiscordEventShareURL.Text = Clipboard.GetText
        End If

    End Sub


    Private Sub btnStartGroupEventPost_Click(sender As Object, e As EventArgs) Handles btnStartGroupEventPost.Click

        Dim autoContinue As Boolean = True

        If Not ValidPostingRequirements(True) Then
            Exit Sub
        End If

        If Not FirstPartOfGroupPost(autoContinue) Then Exit Sub

        If Not SecondPartOfGroupPost(autoContinue) Then Exit Sub

    End Sub


    Private Sub btnEventTopicClipboard_Click(sender As Object, e As EventArgs) Handles btnEventTopicClipboard.Click
        BuildDiscordEventDescription()
        If txtDiscordEventTopic.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventTopic.Text)
            CopyContent.ShowContent(Me,
                                txtDiscordEventTopic.Text,
                                "Paste the topic into the Event Topic field on Discord.",
                                "Creating Discord Event",
                                New List(Of String) From {"^v"})

        End If
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

    Private Sub btnEventDescriptionToClipboard_Click(sender As Object, e As EventArgs) Handles btnEventDescriptionToClipboard.Click
        BuildDiscordEventDescription()
        If txtDiscordEventDescription.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventDescription.Text)
            CopyContent.ShowContent(Me,
                                txtDiscordEventDescription.Text,
                                "Paste the description into the Event Description field on Discord.",
                                "Creating Discord Event",
                                New List(Of String) From {"^v"})

        End If
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
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

        'Task
        sb.AppendLine()
        sb.AppendLine($"Task thread link: [{txtTitle.Text}]({GetDiscordLinkToTaskThread()})")

        'Group Event link
        If txtGroupEventPostURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtGroupEventPostURL.Text.Trim) Then
            sb.AppendLine($"Group event link: [{txtClubFullName.Text} - {txtEventTitle.Text}]({txtGroupEventPostURL.Text})")
        End If

        'Discord Event link
        If txtDiscordEventShareURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtDiscordEventShareURL.Text.Trim) Then
            sb.AppendLine()
            sb.AppendLine($"Discord event link: {txtDiscordEventShareURL.Text}")
        End If

        Dim msg As String = sb.ToString
        Clipboard.SetText(msg)
        CopyContent.ShowContent(Me,
                                msg,
                                "You can now paste the content of the message to share links to the task and the group event.",
                                "Sharing Discord Task and Group Event links",
                                New List(Of String) From {"^v"})

    End Sub

    Private Sub btnEventDPHXAndLinkOnly_Click(sender As Object, e As EventArgs) Handles btnEventDPHXAndLinkOnly.Click

        If txtDiscordTaskID.Text.Trim.Length = 0 Then
            Exit Sub
        End If

        Dim autoContinue As Boolean = True

        Dim dlgResult As DialogResult

        Do While _sessionModified
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, "Latest changes have not been saved! You first need to save the session.", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.OK
                    btnSaveConfig_Click(btnEventDPHXAndLinkOnly, e)
                Case DialogResult.Cancel
                    Return
            End Select
        Loop

        Dim allFiles As New Specialized.StringCollection
        Dim contentForMessage As New StringBuilder

        GetAllFilesForMessage(allFiles, contentForMessage, True)

        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    contentForMessage.ToString,
                                    "Now paste the copied files in a new post under the group event's thread and come back for the text info (coming next).",
                                    "Posting the DPHX file only",
                                    New List(Of String) From {"^v"},
                                    True,
                                    False)
        End If

        If Not autoContinue Then Exit Sub

        If _TBTaskEntrySeqID > 0 Then
            txtFilesText.Text = $"**DPHX file** for people using it and [complete task and weather details here]({GetDiscordLinkToTaskThread()})" &
                            $"{Environment.NewLine}[Task #{_TBTaskEntrySeqID}]({SupportingFeatures.GetWeSimGlideTaskURL(_TBTaskEntrySeqID)}) and event have been published to WeSimGlide.org so are also directly accessible through the tool."
        Else
            txtFilesText.Text = $"**DPHX file** for people using it and [complete task and weather details here]({GetDiscordLinkToTaskThread()})"
        End If
        Clipboard.SetText(txtFilesText.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFilesText.Text,
                                "Now enter the file info In the second message In the thread And post it.",
                                "Posting the DPHX file only",
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
                                New List(Of String) From {"^v"})

    End Sub

    Private Sub chkDGPOTeaser_CheckedChanged(sender As Object, e As EventArgs) Handles chkDGPOTeaser.CheckedChanged

        If chkDGPOTeaser.Checked Then
            chkDGPOMainPost.Checked = False
            chkDGPOFullDescription.Checked = False
            chkDGPOFilesWithoutLegend.Checked = False
            chkDGPOFilesWithFullLegend.Checked = False
            chkDGPOAltRestrictions.Checked = False
            chkDGPOWeatherInfo.Checked = False
            chkDGPOWeatherChart.Checked = False
            chkDGPOWaypoints.Checked = False
            chkDGPOAddOns.Checked = False
            chkDGPORelevantTaskDetails.Checked = False
            chkDGPOEventLogistics.Checked = False
        End If

        CheckWhichOptionsCanBeEnabled()

    End Sub

    Private Sub chkDGPOFilesWithFullLegend_CheckedChanged(sender As Object, e As EventArgs) Handles chkDGPOFilesWithFullLegend.CheckedChanged
        If chkDGPOFilesWithFullLegend.Checked Then
            chkDGPOFilesWithoutLegend.Checked = False
        End If
        chkDGPOAll_CheckedChanged(sender, e)
    End Sub

    Private Sub chkDGPOFilesWithoutLegend_CheckedChanged(sender As Object, e As EventArgs) Handles chkDGPOFilesWithoutLegend.CheckedChanged
        If chkDGPOFilesWithoutLegend.Checked Then
            chkDGPOFilesWithFullLegend.Checked = False
        End If
        chkDGPOAll_CheckedChanged(sender, e)
    End Sub

    Private Sub chkDGPOAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkDGPOEventLogistics.CheckedChanged,
                                                                                    chkDGPORelevantTaskDetails.CheckedChanged,
                                                                                    chkDGPOFullDescription.CheckedChanged,
                                                                                    chkDGPOMainPost.CheckedChanged, chkDGPOWeatherInfo.CheckedChanged, chkDGPOWeatherChart.CheckedChanged, chkDGPOWaypoints.CheckedChanged, chkDGPOAltRestrictions.CheckedChanged, chkDGPOAddOns.CheckedChanged

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
            autoContinue = CoverImage(False)
        End If
        If Not autoContinue Then Return False

        'Group main post
        If chkDGPOMainGroupPost.Enabled AndAlso chkDGPOMainGroupPost.Checked Then
            autoContinue = GroupFlightEventInfoToClipboard(chkDGPOCoverImage.Enabled AndAlso chkDGPOCoverImage.Checked)
        End If
        If Not autoContinue Then Return False

        Return True

    End Function

    Private Function SecondPartOfGroupPost(autoContinue As Boolean) As Boolean

        Dim msg As String = String.Empty

        'Teaser
        If chkDGPOTeaser.Enabled AndAlso chkDGPOTeaser.Checked Then
            If chkDGPOThreadCreation.Enabled AndAlso chkDGPOThreadCreation.Checked Then
                autoContinue = CreateGroupEventThread()
                If Not autoContinue Then Return False
            End If
            autoContinue = GroupFlightEventTeaser()
            Return autoContinue
        End If

        'We must have both Discord IDs for the task to continue
        If lblThread1stMsgIDNotAcquired.Visible Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The task must be fully posted to the Task Library first before continuing.", "Task not fully posted to library", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End Using
            Return False
        End If

        'Thread
        If chkDGPOThreadCreation.Enabled AndAlso chkDGPOThreadCreation.Checked Then
            autoContinue = CreateGroupEventThread()
            If Not autoContinue Then Return False
        End If

        'Second part of Group Flight Event - task part one
        autoContinue = PostTaskInGroupEventPartOne(autoContinue)
        If Not autoContinue Then Return False

        'Files
        If (chkDGPOFilesWithFullLegend.Enabled AndAlso chkDGPOFilesWithFullLegend.Checked) OrElse (chkDGPOFilesWithoutLegend.Enabled AndAlso chkDGPOFilesWithoutLegend.Checked) Then
            autoContinue = FilesCopy(True)
            If autoContinue Then
                'Files text (description or simple Files heading)
                autoContinue = FilesTextCopy(chkDGPOFilesWithFullLegend.Checked, True)
                If Not autoContinue Then
                    Return False
                End If
            Else
                Return False
            End If
        End If

        If Not autoContinue Then Return False

        'Second part of Group Flight Event - task part two
        autoContinue = PostTaskInGroupEventPartTwo(autoContinue)
        If Not autoContinue Then Return False


        Return True

    End Function

    Private Function GroupFlightEventInfoToClipboard(Optional withCover As Boolean = False) As Boolean

        Dim autoContinue As Boolean

        BuildGroupFlightPost()
        Clipboard.SetText(txtGroupFlightEventPost.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtGroupFlightEventPost.Text,
                                $"In the Discord app and on the proper channel for the club/group, make sure you are on the new message field to post the group flight event.{Environment.NewLine}Next, you will also be asked to copy the link To that newly created message.",
                                "Creating group flight post",
                                New List(Of String) From {"^v"},
                                chkDPOExpertMode.Checked,
                                True,
                                If(withCover, numWaitSecondsForFiles.Value / 2 * 1000, 0))

        If Not autoContinue Then Return autoContinue

        If txtGroupEventPostURL.Text = String.Empty Then
            Dim message As String = "Please Get the link To the group event's post in Discord (""...More menu"" and ""Copy Message Link"")"
            Dim waitingForm As New WaitingForURLForm(message, False)
            Dim answer As DialogResult = waitingForm.ShowDialog()

            SupportingFeatures.BringDPHToolToTop(Me.Handle)

            'Check if the clipboard contains a valid URL, which would mean the group event's URL has been copied
            If answer = DialogResult.OK Then
                Dim groupEventPostURL As String
                groupEventPostURL = Clipboard.GetText
                txtGroupEventPostURL.Text = groupEventPostURL
                SaveSession()
            End If
            If txtGroupEventPostURL.Text = String.Empty Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Take a minute to copy the Discord link to the group flight event you've just created and paste it below in the URL field.", "Creating group flight post", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
                                                     "Follow the instructions as shown below to create the group event's thread.",
                                                     "ONLY once you've created the thread, pasted its name in THREAD NAME and positionned your cursor on the thread's message field, can you click OK and resume the workflow.",
                                                     "Instructions for the creation of the group event's thread!")
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

        Dim imagePasted As Boolean = False
        If txtEventTeaserAreaMapImage.Text <> String.Empty AndAlso File.Exists(txtEventTeaserAreaMapImage.Text) Then
            Dim allFiles As New StringCollection
            allFiles.Add(txtEventTeaserAreaMapImage.Text)
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    txtEventTeaserAreaMapImage.Text,
                                    $"Position the cursor on the message field in the group event thread and paste the copied teaser image for your first message.{Environment.NewLine}Skip (Ok) if already done.",
                                    "Pasting teaser area map image",
                                    New List(Of String) From {"^v"},
                                    chkDPOExpertMode.Checked,
                                    txtEventTeaserMessage.Text.Trim.Length = 0,
                                    If(txtEventTeaserMessage.Text.Trim.Length = 0, numWaitSecondsForFiles.Value / 2 * 1000, 0),
                                    Drawing.Image.FromFile(allFiles(0)))
            imagePasted = True
        End If

        If Not autoContinue Then
            Return False
        End If

        'Teaser message
        If txtEventTeaserMessage.Text.Trim <> String.Empty Then
            Dim teaser As New StringBuilder
            teaser.AppendLine("## 🤐 Teaser")
            teaser.AppendLine(txtEventTeaserMessage.Text.Trim)
            Clipboard.SetText(teaser.ToString)
            autoContinue = CopyContent.ShowContent(Me,
                            teaser.ToString,
                            $"Make sure you are back on the thread's message field.{Environment.NewLine}Then post the teaser message as the first message in the event's thread.",
                            "Posting teaser with text.",
                            New List(Of String) From {"^v"},
                            chkDPOExpertMode.Checked,
                            True,
                            If(imagePasted, numWaitSecondsForFiles.Value / 2 * 1000, 0))
        End If

        Return autoContinue

    End Function

    Private Function GroupFlightEventThreadLogistics() As String

        Dim logisticInstructions As New StringBuilder

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)
        Dim fullSyncFlyDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventSyncFlyDate, dtEventSyncFlyTime, chkDateTimeUTC.Checked)
        Dim fullLaunchDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventLaunchDate, dtEventLaunchTime, chkDateTimeUTC.Checked)
        Dim fullStartTaskDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventStartTaskDate, dtEventStartTaskTime, chkDateTimeUTC.Checked)

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If
        Dim theLocalTime As String = String.Empty
        If chkUseSyncFly.Checked Then
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        ElseIf chkUseLaunch.Checked Then
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        Else
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        End If

        logisticInstructions.AppendLine("## Event Logistics")
        logisticInstructions.AppendLine($"🗣 Voice: **{cboVoiceChannel.Text}**")
        logisticInstructions.AppendLine($"🌐 Server: **{cboMSFSServer.Text}**")
        logisticInstructions.AppendLine($"📆 Sim date and time: **{dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local **(when it is {theLocalTime} in your own local time){SupportingFeatures.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True)}")

        If chkUseSyncFly.Checked Then
            logisticInstructions.AppendLine("🛑 Stay on the world map to synchronize weather 🛑")
        End If
        logisticInstructions.AppendLine()
        logisticInstructions.AppendLine($"*Don't forget to review the details for this group flight event (first post of the thread).*")
        logisticInstructions.AppendLine()
        logisticInstructions.AppendLine("**Use this thread only to discuss logistics for this event!**")
        logisticInstructions.AppendLine("> Please focus on:")
        logisticInstructions.AppendLine("> - Event logistics, such as meet-up times and locations")
        logisticInstructions.AppendLine("> - Asking for help to join and participate to this event")
        logisticInstructions.AppendLine("> - Providing feedback on the **event's** organization and coordination")
        If Not txtDiscordTaskID.Text = String.Empty Then
            logisticInstructions.AppendLine("### 🏁 For posting results, screenshots, and task feedback, please head over to the task's thread!")
            logisticInstructions.AppendLine($"🧵 [{txtEventTitle.Text.Trim} - Task thread](https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{txtDiscordTaskID.Text})")
        End If

        Return logisticInstructions.ToString()

    End Function

    Private Function BuildLightTaskDetailsForEventPost(altRestrictionsMsg As Boolean, addOnsMsg As Boolean, allFilesDetailsPosted As Boolean, fltPlanPosted As Boolean) As String

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        If Not txtDiscordTaskID.Text = String.Empty Then
            sb.AppendLine("## Relevant task details summary")
            If altRestrictionsMsg AndAlso txtAltRestrictions.Text.Trim.Length > 33 Then
                sb.AppendLine("⚠️ There are altitude restrictions on this task")
            End If
            If addOnsMsg AndAlso lstAllRecommendedAddOns.Items.Count > 0 Then
                sb.AppendLine("📀 There are recommended add-ons with this task")
            End If
            sb.AppendLine($"🔗 [Click here for the official task details in the Task Library]({GetDiscordLinkToTaskThread()})")
            sb.AppendLine("*If you did not join MSFS Soaring Task Tools already, you will need this [invite link](https://discord.gg/aW8YYe3HJF) first*")
            sb.AppendLine()
        End If

        If (Not allFilesDetailsPosted) AndAlso (Not txtFlightPlanFile.Text = String.Empty) Then
            sb.AppendLine($"📁 Flight plan file: **""{Path.GetFileName(txtFlightPlanFile.Text)}""**")
        End If
        If (Not allFilesDetailsPosted) AndAlso txtWeatherFile.Text <> String.Empty AndAlso (_WeatherDetails IsNot Nothing) Then
            sb.AppendLine($"🌤 Weather file & profile name: **""{Path.GetFileName(txtWeatherFile.Text)}"" ({_WeatherDetails.PresetName})**")
        End If
        If (Not fltPlanPosted) AndAlso (Not chkDGPOEventLogistics.Checked) Then
            sb.AppendLine($"📆 Sim date and time: **{dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local** {SupportingFeatures.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True)}")
        End If

        Return sb.ToString.Trim

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

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        lblDiscordPostDateTime.Text = $"{fullMeetDateTimeLocal:dddd, MMMM dd}, {fullMeetDateTimeLocal:hh:mm tt}"
        lblDiscordEventVoice.Text = SupportingFeatures.ReturnTextFromURLMarkdown(cboVoiceChannel.Text)

        txtGroupFlightEventPost.Text = String.Empty

        If txtEventTitle.Text = String.Empty Then
            txtEventTitle.Text = txtTitle.Text
        End If
        If txtEventTitle.Text <> String.Empty Then
            'If cboGroupOrClubName.SelectedIndex > -1 Then
            'sb.Append($"# {_ClubPreset.ClubName} - ")
            'Else
            'sb.Append($"# ")
            'End If
            sb.AppendLine($"# {txtEventTitle.Text & AddFlagsToTitle()}")
        End If

        If txtCredits.Text <> String.Empty Then
            sb.AppendLine(txtCredits.Text)
            sb.AppendLine()
        End If

        sb.AppendLine($"**{_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time**")
        sb.AppendLine()

        sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))

        Dim theLocalTime As String = String.Empty
        If chkUseSyncFly.Checked Then
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        ElseIf chkUseLaunch.Checked Then
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        Else
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        End If

        sb.AppendLine($"💼 Meet/Briefing: **{_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}**{Environment.NewLine}At this time we meet in the voice chat and get ready.")
        sb.AppendLine()

        If chkUseSyncFly.Checked Then
            sb.AppendLine($"⏱️ Sync Fly: **{_SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}**{Environment.NewLine}At this time we simultaneously click fly to sync our weather.")
            If chkUseLaunch.Checked AndAlso fullSyncFlyDateTimeLocal = fullLaunchDateTimeLocal Then
                sb.AppendLine("At this time we can also start launching from the airfield.")
            End If
            sb.AppendLine()
        End If
        If chkUseLaunch.Checked AndAlso (fullSyncFlyDateTimeLocal <> fullLaunchDateTimeLocal OrElse Not chkUseSyncFly.Checked) Then
            sb.AppendLine($"🚀 Launch: **{_SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}**{Environment.NewLine}At this time we can start launching from the airfield.")
            sb.AppendLine()
        End If

        If chkUseStart.Checked Then
            sb.AppendLine($"🟢 Task Start: **{_SF.GetDiscordTimeStampForDate(fullStartTaskDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}**{Environment.NewLine}At this time we cross the starting line and start the task.")
            sb.AppendLine()
        End If

        sb.AppendLine($"⏳ Duration: **{SupportingFeatures.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}**")
        If txtAATTask.Text.Length > 0 Then
            sb.AppendLine($"⚠️ **{txtAATTask.Text}**")
        End If
        sb.AppendLine()

        If cboEligibleAward.SelectedIndex > 0 Then
            sb.AppendLine($"🏆 Pilots who finish this task successfully during the event will be eligible to apply for the **{cboEligibleAward.Text} Soaring Badge** :{cboEligibleAward.Text.ToLower()}:")
            sb.AppendLine()
        End If

        If chkDGPOFilesWithFullLegend.Checked OrElse chkDGPOFilesWithoutLegend.Checked Then
            sb.AppendLine($"📁 All files are shared inside the thread below")
            sb.AppendLine()
        Else
            sb.AppendLine($"📁 All files will be shared inside the thread below, a few hours before the actual event takes place")
            sb.AppendLine()
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
        If Not urlBeginnerGuide = String.Empty Then
            sb.AppendLine($"‍:student: If it's your first time flying with us, please make sure to read the following guide: {urlBeginnerGuide}")
            sb.AppendLine()
        End If

        If SupportingFeatures.IsValidURL(txtDiscordEventShareURL.Text) Then
            sb.AppendLine(txtDiscordEventShareURL.Text)
        End If

        txtGroupFlightEventPost.Text = sb.ToString.Trim

    End Sub

    Private Sub BuildDiscordEventDescription()

        txtDiscordEventTopic.Text = String.Empty
        If Not txtEventTitle.Text = String.Empty Then
            If cboGroupOrClubName.SelectedIndex > -1 Then
                txtDiscordEventTopic.AppendText($"{_ClubPreset.ClubName} - ")
            End If
            txtDiscordEventTopic.AppendText(txtEventTitle.Text)
        End If

        Dim sb As New StringBuilder

        sb.AppendLine($"**Server:** {cboMSFSServer.Text}")
        sb.AppendLine($"**Duration:** {SupportingFeatures.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{SupportingFeatures.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}")
        sb.AppendLine()
        sb.Append(SupportingFeatures.ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))
        sb.AppendLine("**More Information on this group flight event:**")
        sb.AppendLine(txtGroupEventPostURL.Text)

        txtDiscordEventDescription.Text = sb.ToString.Trim

    End Sub

#End Region

#End Region

#Region "Briefing tab"

#Region "Briefing tab event handlers"

    Private Sub cboCoverImage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCoverImage.SelectedIndexChanged

        SessionModified()

    End Sub

    Private Sub cboBriefingMap_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBriefingMap.SelectedIndexChanged

        SessionModified()

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
            BriefingControl1.GenerateBriefing(_SF, SetAndRetrieveSessionData(), txtFlightPlanFile.Text, txtWeatherFile.Text)
        End If

    End Sub

#End Region

#Region "Guide/Wizard"

#Region "Event Handlers"

    Private Sub btnGuideMe_Click(sender As Object, e As EventArgs) Handles toolStripGuideMe.Click

        Dim activateGuide As Boolean = False

        Select Case TabControl1.SelectedTab.TabIndex
            Case tabFlightPlan.TabIndex
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show(Me, "Do you want to start by resetting everything?", "Starting the Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
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

    Private Sub Main_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles TabControl1.KeyDown, Me.KeyDown

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
                TabControl1.SelectedTab = TabControl1.TabPages("tabFlightPlan")
                SetGuidePanelToLeft()
                pnlGuide.Top = -3
                lblGuideInstructions.Text = "Click the ""Flight Plan"" button And Select the flight plan To use With this task."
                SetFocusOnField(btnSelectFlightPlan, fromF1Key)
            Case 2 'Select weather file
                SetGuidePanelToLeft()
                pnlGuide.Top = 54
                lblGuideInstructions.Text = "Click the ""Weather file"" button And Select the weather file To use With this task."
                SetFocusOnField(btnSelectWeatherFile, fromF1Key)
            Case 3 'Title
                SetGuidePanelToLeft()
                pnlGuide.Top = 95
                lblGuideInstructions.Text = "This Is the title that was read from the flight plan. Is it ok? If Not, change it And use the checkbox to prevent it from being overwritten."
                SetFocusOnField(txtTitle, fromF1Key)
            Case 4 'Sim date & time
                SetGuidePanelToLeft()
                pnlGuide.Top = 139
                lblGuideInstructions.Text = "Specify the appropriate local date And time to set inside MSFS. You can also add extra information in the text box if you want."
                SetFocusOnField(dtSimDate, fromF1Key)
            Case 5 'Main area
                SetGuidePanelToLeft()
                pnlGuide.Top = 194
                lblGuideInstructions.Text = "Do you want to specify the main area And/Or point of interest for this task? It's only optionnal."
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
            Case 10 'AAT TODO: Complete
                SetGuidePanelToLeft()
                pnlGuide.Top = 371
                lblGuideInstructions.Text = "This field only displays information if the task is an AAT. Nothing to do but to validate the info."
                SetFocusOnField(txtAATTask, fromF1Key)
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
            Case 17 'Countries
                SetGuidePanelToRight()
                pnlGuide.Top = 28
                lblGuideInstructions.Text = "Countries to show in the topic should be read from the waypoints, but you can optionally specify them yourself here."
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
                SetFocusOnField(txtDiscordTaskID, fromF1Key)
            Case 24 'Task ID
                SetGuidePanelToRight()
                pnlGuide.Top = 615
                lblGuideInstructions.Text = "Once you've posted the task on Discord and before posting the files, copy the task URL and paste it here."
                SetFocusOnField(txtDiscordTaskID, fromF1Key)

            Case 25 To 29 'End of flight plan data
                _GuideCurrentStep = 34
                ShowGuide()

            Case 34 'Briefing review
                TabControl1.SelectedIndex = 3
                SetBriefingGuidePanel()
                lblBriefingGuideInstructions.Text = "Review the task information on the various briefing tabs here and when you are satisfied, click Next."
                SetFocusOnField(BriefingControl1, fromF1Key)
            Case 35 To 39 'We're done with the briefing
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 40 'Repost checkbox and date
                TabControl1.SelectedTab = TabControl1.TabPages("tabDiscord")
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 26
                lblDiscordGuideInstructions.Text = "If you'd like to specify where and when this task has been published first, enable this to set the original date the task was published and its URL."
                SetFocusOnField(chkRepost, fromF1Key)
            Case 41 'Discord Post Options for task
                TabControl1.SelectedTab = TabControl1.TabPages("tabDiscord")
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 220
                lblDiscordGuideInstructions.Text = "These are all the options you can toggle to include the various elements of the post, as you see fit."
                SetFocusOnField(chkDPOMainPost, fromF1Key)
            Case 42 'Reset all options to default
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 601
                lblDiscordGuideInstructions.Text = "Use these buttons to recall, save or reset all the options above to your remembered settings or their default values."
                SetFocusOnField(btnDPOResetToDefault, fromF1Key)
            Case 43 'Start Task Posting Workflow
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 644
                lblDiscordGuideInstructions.Text = "Click this button to start the workflow to post your task on Discord using the selected options above."
                SetFocusOnField(btnStartTaskPost, fromF1Key)

            Case 44 To 59 'Next section
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 60 'Event
                'Resume wizard on the Event tab
                TabControl1.SelectedTab = TabControl1.TabPages("tabEvent")
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 69
                lblEventGuideInstructions.Text = "Start by selecting the soaring club or known group for which you want to create a new event, if this applies to you."
                SetFocusOnField(cboGroupOrClubName, fromF1Key)

            Case 61 'Group flight title / topic
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 141
                lblEventGuideInstructions.Text = "If you would like to specify a different title for the group flight, you can do so now. Otherwise, this is the same as the task's title."
                SetFocusOnField(txtEventTitle, fromF1Key)

            Case 62 'MSFS Server
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 176
                lblEventGuideInstructions.Text = "Specify the MSFS Server to use during the group flight. If you specified a known club, then you should not change this, unless it is incorrect."
                SetFocusOnField(cboMSFSServer, fromF1Key)

            Case 63 'Voice channel
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 212
                lblEventGuideInstructions.Text = "Specify the Discord Voice channel to use during the group flight. If you specified a known club, then you should not change this, unless it is incorrect."
                SetFocusOnField(cboVoiceChannel, fromF1Key)

            Case 64 'UTC Zulu
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 251
                lblEventGuideInstructions.Text = "For the sake of simplicity, leave this checkbox ticked to use UTC (Zulu) entries. Local times are still displayed to the right."
                SetFocusOnField(chkDateTimeUTC, fromF1Key)

            Case 65 'Meet time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 280
                lblEventGuideInstructions.Text = "Specify the meet date and time. This is the time when people will start gathering for the group flight and briefing."
                SetFocusOnField(dtEventMeetDate, fromF1Key)

            Case 66 'Sync Fly
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 315
                lblEventGuideInstructions.Text = "Only if the flight's conditions require a synchronized click ""Fly"", then tick the ""Yes"" checkbox and specify when it will happen."
                SetFocusOnField(chkUseSyncFly, fromF1Key)

            Case 67 'Launch Time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 349
                lblEventGuideInstructions.Text = "If you want to specify the time when people should start to launch from the airfield, tick the ""Yes"" checkbox and specify when it should happen."
                SetFocusOnField(chkUseLaunch, fromF1Key)

            Case 68 'Start Task Time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 394
                lblEventGuideInstructions.Text = "If you want to specify a time for the start of the task (going through the start gate), tick the ""Yes"" checkbox and specify when it should happen."
                SetFocusOnField(chkUseStart, fromF1Key)

            Case 69 'Group flight description
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 470
                lblEventGuideInstructions.Text = "If you would like to specify a description for the group flight, you can do so now."
                SetFocusOnField(txtEventDescription, fromF1Key)

            Case 70 'SSC Award
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 563
                lblEventGuideInstructions.Text = "This is usually set automatically if the club is SSC Saturday and depending on the task's distance. You should leave it alone, unless it's incorrect."
                SetFocusOnField(cboEligibleAward, fromF1Key)

            Case 71 'Beginner's link
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 622
                lblEventGuideInstructions.Text = "You can select from different beginner's guide (or specify a link to a custom one) to include with the group event post."
                SetFocusOnField(cboBeginnersGuide, fromF1Key)

            Case 72 'Teaser section
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 702
                lblEventGuideInstructions.Text = "You can opt to first post a teaser only for your group event. Check this box, select a teaser image and message."
                SetFocusOnField(chkEventTeaser, fromF1Key)

            Case 73 'Briefing review
                TabControl1.SelectedIndex = 3
                SetBriefingGuidePanel()
                lblBriefingGuideInstructions.Text = "Review the task and event information on the briefing tabs here and when you are satisfied, click Next."
                SetFocusOnField(BriefingControl1, fromF1Key)

            Case 74 To 79 'Next section
                _GuideCurrentStep = AskWhereToGoNext()
                ShowGuide()

            Case 80 'Discord Post Options
                TabControl1.SelectedTab = TabControl1.TabPages("tabDiscord")
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 591
                pnlWizardDiscord.Top = 557
                lblDiscordGuideInstructions.Text = "These are all the options you can toggle to include the various elements of the post, as you see fit."
                SetFocusOnField(chkDGPOCoverImage, fromF1Key)

            Case 81 'Group flight URL
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 591
                pnlWizardDiscord.Top = 624
                lblDiscordGuideInstructions.Text = "From Discord, copy the link to the group flight post you just created above, and click ""Paste"" here."
                SetFocusOnField(btnDiscordGroupEventURL, fromF1Key)

            Case 82 'Reset all options
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 591
                pnlWizardDiscord.Top = 664
                lblDiscordGuideInstructions.Text = "Use these buttons to recall, save or reset all the options above to your remembered settings or their default values."
                SetFocusOnField(btnDGPOResetToDefault, fromF1Key)

            Case 83 'Start workflow
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Left = 591
                pnlWizardDiscord.Top = 707
                lblDiscordGuideInstructions.Text = "Click this button to start the workflow to post your group event on Discord using the selected options."
                SetFocusOnField(btnStartGroupEventPost, fromF1Key)

            Case 84 'Seconds to wait for files
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 0
                lblDiscordGuideInstructions.Text = "Specify the pause duration when posting files to wait for Discord to complete the upload and continue the workflow."
                SetFocusOnField(numWaitSecondsForFiles, fromF1Key)

            Case 85 'Automatic workflow progression
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 27
                lblDiscordGuideInstructions.Text = "When you enable this, the workflow will try to progress automatically step after step when possible."
                SetFocusOnField(chkDPOExpertMode, fromF1Key)

            Case 86 'Start full workflow
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 105
                lblDiscordGuideInstructions.Text = "Click this button to start the workflow to post both the task and group flight event full details."
                SetFocusOnField(btnStartFullPostingWorkflow, fromF1Key)

            Case 87 'Share the Discord Event on the task
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 183
                lblDiscordGuideInstructions.Text = "Click this button to copy the message to share the group event for the task."
                SetFocusOnField(btnTaskFeaturedOnGroupFlight, fromF1Key)

            Case 88 'DPHX only
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 225
                lblDiscordGuideInstructions.Text = "Click this button to copy and paste a simple message with only the DPHX file and link to task post."
                SetFocusOnField(btnEventDPHXAndLinkOnly, fromF1Key)

            Case 89 'Task and Group Event Links
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 269
                lblDiscordGuideInstructions.Text = "Click this button to copy and paste a simple message with both links to task and group event."
                SetFocusOnField(btnTaskAndGroupEventLinks, fromF1Key)

            Case 90 'Discord Event
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show("Do you have the access rights to create Discord Event on the target Discord Server? Click No if you don't know.", "Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        _GuideCurrentStep += 1
                    Else
                        _GuideCurrentStep = AskWhereToGoNext()
                    End If
                End Using
                ShowGuide()

            Case 91 'Create Discord Event
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 345
                lblDiscordGuideInstructions.Text = "In Discord and in the proper Discord Server, start the creation of a new Event (Create Event). If you don't know how to do this, ask for help!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 92 'Select voice channel for event
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 377
                lblDiscordGuideInstructions.Text = "On the new event window, under ""Where is your event"", choose ""Voice Channel"" and select this voice channel. Then click ""Next"" on the event window."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 93 'Topic name
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 410
                lblDiscordGuideInstructions.Text = "Click this button to copy the event topic and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventTopicClipboard, fromF1Key)

            Case 94 'Event date & time
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 445
                lblDiscordGuideInstructions.Text = "On the Discord event window, specify the date and time displayed here - these are all local times you have to use!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 95 'Event description
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 476
                lblDiscordGuideInstructions.Text = "Click this button to copy the event description and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventDescriptionToClipboard, fromF1Key)

            Case 96 'Cover image
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 509
                lblDiscordGuideInstructions.Text = "In the Discord event window, you can also upload a cover image for your event. This is optional."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 97 'Preview and publish
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 541
                lblDiscordGuideInstructions.Text = "In the Discord event window, click Next to review your event information and publish it."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 98 'Paste link to Discord Event
                SetDiscordGuidePanelToRight()
                pnlWizardDiscord.Top = 604
                lblDiscordGuideInstructions.Text = "From the Discord Event published window, copy the URL to share to and invite participants and click ""Paste"" here."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 99 To 99
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
                Return 80
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
        Select Case TabControl1.SelectedTab.Name
            Case "tabBriefing"
                GenerateBriefing()
            Case "tabDiscord"
                BuildFPResults()
                BuildWeatherCloudLayers()
                BuildWeatherWindLayers()
                BuildWeatherInfoResults()
        End Select

    End Sub

    Private Sub btnSaveConfig_Click(sender As Object, e As EventArgs) Handles toolStripSave.Click

        SaveSession()

    End Sub

    Private Sub btnCreateShareablePack_Click(sender As Object, e As EventArgs) Handles toolStripSharePackage.Click

        If _CurrentSessionFile <> String.Empty Then
            Dim DPHXFilename As String = $"{Path.GetDirectoryName(_CurrentSessionFile)}\{Path.GetFileNameWithoutExtension(_CurrentSessionFile)}.dphx"
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
            OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If

        OpenFileDialog1.FileName = String.Empty
        OpenFileDialog1.Title = "Select session or package file to load"
        OpenFileDialog1.Filter = "Discord Post Helper|*.dph;*.dphx"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            LoadFile(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub LoadFile(selectedFilename As String)

        Dim validSessionFile As Boolean = True

        'Check if the selected file is a dph or dphx files
        If Path.GetExtension(selectedFilename) = ".dphx" Then
            'Package - we need to unpack it first
            selectedFilename = _SF.UnpackDPHXFile(selectedFilename)

            If selectedFilename = String.Empty Then
                validSessionFile = False
            Else
                validSessionFile = True
            End If
        End If

        If validSessionFile Then
            ResetForm()
            _loadingFile = True
            _CurrentSessionFile = selectedFilename
            LoadSessionData(selectedFilename)
            GenerateBriefing()
            _loadingFile = False
            CheckWhichOptionsCanBeEnabled()
        End If

    End Sub

    Private Sub SaveSessionData(filename As String)

        Dim allCurrentData As AllData = SetAndRetrieveSessionData()

        Dim serializer As New XmlSerializer(GetType(AllData))
        Using stream As New FileStream(filename, FileMode.Create)
            serializer.Serialize(stream, allCurrentData)
        End Using

        _CurrentSessionFile = filename
        SetFormCaption(filename)

    End Sub

    Private Function SetAndRetrieveSessionData() As AllData

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
            .LockCountries = chkLockCountries.Checked
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
            .DiscordTaskID = txtDiscordTaskID.Text
            .TaskThreadFirstPostID = _taskThreadFirstPostID
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
            .URLDiscordEventInvite = txtDiscordEventShareURL.Text
            .MapImageSelected = cboBriefingMap.Text
            .LockMapImage = chkLockMapImage.Checked
            .CoverImageSelected = cboCoverImage.Text
            .LockCoverImage = chkLockCoverImage.Checked
            .BeginnersGuide = cboBeginnersGuide.Text
            .BeginnersGuideCustom = txtOtherBeginnerLink.Text
            .GroupEventTeaserEnabled = chkEventTeaser.Checked
            .GroupEventTeaserMessage = txtEventTeaserMessage.Text.Replace(Environment.NewLine, "($*$)")
            .GroupEventTeaserAreaMapImage = txtEventTeaserAreaMapImage.Text

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
                chkLockCountries.Checked = .LockCountries
                LoadFlightPlan(txtFlightPlanFile.Text)

                If File.Exists(.WeatherFilename) Then
                Else
                    'Should expect the file to be in the same folder as the .dph file
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.WeatherFilename)}") Then
                        .WeatherFilename = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.WeatherFilename)}"
                    End If
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
                    If File.Exists($"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.DPHXPackageFilename)}") Then
                        .DPHXPackageFilename = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.DPHXPackageFilename)}"
                        txtDPHXPackageFilename.Text = .DPHXPackageFilename
                    End If
                End If
                If .DiscordTaskID = String.Empty AndAlso .DiscordTaskThreadURL <> String.Empty AndAlso SupportingFeatures.IsValidURL(.DiscordTaskThreadURL) Then
                    .DiscordTaskID = SupportingFeatures.ExtractMessageIDFromDiscordURL(.DiscordTaskThreadURL, True)
                End If
                txtDiscordTaskID.Text = .DiscordTaskID
                _taskThreadFirstPostID = .TaskThreadFirstPostID
                chkActivateEvent.Checked = .EventEnabled
                cboGroupOrClubName.Text = .GroupClubId
                txtClubFullName.Text = .GroupClubName
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
                txtDiscordEventShareURL.Text = .URLDiscordEventInvite
                cboBeginnersGuide.Text = .BeginnersGuide
                If cboBeginnersGuide.Text = String.Empty Then
                    cboBeginnersGuide.Text = "None"
                End If
                txtOtherBeginnerLink.Text = .BeginnersGuideCustom
                chkLockMapImage.Checked = .LockMapImage
                chkLockCoverImage.Checked = .LockCoverImage
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

            'BuildFPResults()
            BuildWeatherInfoResults()
            BuildRecAddOnsText()
            'BuildGroupFlightPost()
            'BuildDiscordEventDescription()

            FixForDropDownCombos()

            If _taskThreadFirstPostID = String.Empty Then
                lblThread1stMsgIDNotAcquired.Visible = True
                lblThread1stMsgIDAcquired.Visible = False
            Else
                lblThread1stMsgIDAcquired.Visible = True
                lblThread1stMsgIDNotAcquired.Visible = False
            End If

            _sessionModified = False

        End If

    End Sub

    Private Sub FixForDropDownCombos()
        cboRecommendedGliders.SelectionStart = cboRecommendedGliders.Text.Length
        cboGroupOrClubName.SelectionStart = cboGroupOrClubName.Text.Length
        cboVoiceChannel.SelectionStart = cboVoiceChannel.Text.Length
    End Sub

    Public Sub SessionModified()

        If (Not _sessionModified) AndAlso (Not _loadingFile) Then
            _sessionModified = True
            SetSaveButtonFont()
        End If

    End Sub

    Public Sub SessionUntouched()

        If _sessionModified Then
            _sessionModified = False
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

#End Region

#Region "Task Browser Code"

#Region "Event Handlers"
    Private Sub btnCreateInTaskBrowser_Click(sender As Object, e As EventArgs) Handles btnCreateInTaskBrowser.Click
        If UserCanCreateTask Then
            UploadToTaskBrowser()
            GetTaskDetails(txtDiscordTaskID.Text.Trim)
            SetTBTaskDetailsLabel()
            WeSimGlideTaskLinkPosting()
        End If
    End Sub

    Private Sub btnUpdateInTaskBrowser_Click(sender As Object, e As EventArgs) Handles btnUpdateInTaskBrowser.Click
        If UserCanUpdateTask Then
            'Check if an update description is present
            If txtLastUpdateDescription.TextLength = 0 Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Please provide a description for this task update!", "Publishing task update", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Using
            End If
            UploadToTaskBrowser()
            GetTaskDetails(txtDiscordTaskID.Text.Trim)
            SetTBTaskDetailsLabel()
        End If
    End Sub

    Private Sub btnDeleteFromTaskBrowser_Click(sender As Object, e As EventArgs) Handles btnDeleteFromTaskBrowser.Click
        If UserCanDeleteTask Then
            DeleteTaskFromBrowser()
            SetTBTaskDetailsLabel()
        End If
    End Sub

    Private Sub btnPublishEventNews_Click(sender As Object, e As EventArgs) Handles btnPublishEventNews.Click
        If UserCanCreateEvent Then
            Dim key As String
            Dim eventDate As Date
            Dim comments As String = String.Empty

            eventDate = SupportingFeatures.GetFullEventDateTimeInLocal(dtEventMeetDate.Value, dtEventMeetTime.Value, chkDateTimeUTC.Checked)

            eventDate = eventDate.ToUniversalTime
            key = $"E-{_ClubPreset.EventNewsID}{eventDate.ToUniversalTime.ToString("yyyyMMdd")}"

            If chkEventTeaser.Checked AndAlso txtEventTeaserMessage.Text.Trim <> String.Empty Then
                comments = txtEventTeaserMessage.Text.Trim
            ElseIf txtEventDescription.Text.Trim <> String.Empty Then
                comments = txtEventDescription.Text.Trim
            Else
                comments = txtShortDescription.Text.Trim
            End If
            Dim result As Boolean = PublishEventNews(key,
                                                    txtClubFullName.Text.Trim,
                                                    txtEventTitle.Text.Trim,
                                                    comments,
                                                    eventDate,
                                                    Now.ToUniversalTime,
                                                    _TBTaskEntrySeqID,
                                                    txtGroupEventPostURL.Text.Trim,
                                                    eventDate.AddHours(3)
)
            If result Then
                Dim msgForEventHunters As String = String.Empty
                If _TBTaskEntrySeqID > 0 Then
                    msgForEventHunters = $"@TasksBrowser @EventHunter {Environment.NewLine}[{txtClubFullName.Text.Trim} - {txtEventTitle.Text.Trim}]({SupportingFeatures.GetWeSimGlideEventURL(key)}){Environment.NewLine}[Task #{_TBTaskEntrySeqID.ToString.Trim}]({SupportingFeatures.GetWeSimGlideTaskURL(_TBTaskEntrySeqID)})"
                Else
                    msgForEventHunters = $"@TasksBrowser @EventHunter {Environment.NewLine}[{txtClubFullName.Text.Trim} - {txtEventTitle.Text.Trim}]({SupportingFeatures.GetWeSimGlideEventURL(key)}){Environment.NewLine}Please monitor the original event as task has not been published yet.)"
                End If
                Clipboard.SetText(msgForEventHunters)

                CopyContent.ShowContent(Me,
                                msgForEventHunters,
                                "Event news published! You can now paste the content of the message into the 'wsg-accouncements' channel to share WSG event and task links.",
                                "Sharing WeSimGlide.org Task and Group Event links",
                                New List(Of String) From {"^v"})

            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Failed publish the event news entry.", "Publishing event news entry", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If

        End If

    End Sub

#End Region

#Region "Permissions"
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
            If _userPermissions.ContainsKey("UpdateTask") Then
                Return _userPermissions("UpdateTask")
            Else
                Return False
            End If
        End Get
    End Property
    Private ReadOnly Property UserCanDeleteTask As Boolean
        Get
            If _userPermissions.ContainsKey("DeleteTask") Then
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

    Private Sub DeleteTaskFromBrowser()

        Dim taskInfo As AllData = SetAndRetrieveSessionData()

        Dim result As Boolean = DeleteTaskFromServer(taskInfo.DiscordTaskID)

        If result Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Task removed from database successfully.", "Removal Result", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Failed to remove the task.", "Removal Result", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Public Function DeleteTaskFromServer(taskID As String) As Boolean
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}DeleteTask.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "application/x-www-form-urlencoded"

        Dim postData As String = $"TaskID={Uri.EscapeDataString(taskID)}&UserID={Uri.EscapeDataString(_userPermissionID)}"
        Dim data As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentLength = data.Length

        Try
            Using stream As Stream = request.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    ' Assuming the response is a JSON object with a "status" field
                    Dim result As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
                    Return result("status").ToString() = "success"
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error deleting task: {ex.Message}", "Task deletion error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Private Sub UploadToTaskBrowser()

        Dim taskInfo As AllData = SetAndRetrieveSessionData()

        Dim theMapImage As Byte()
        Dim theCoverImage As Byte()

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

        ' Update the taskData dictionary to include WorldMapInfo fields
        Dim taskData As New Dictionary(Of String, Object) From {
        {"TaskID", taskInfo.DiscordTaskID},
        {"Title", taskInfo.Title},
        {"LastUpdate", GetFileUpdateUTCDateTime(_CurrentSessionFile).ToString("yyyy-MM-dd HH:mm:ss")},
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
        {"RepostText", repostText},
        {"LastUpdateDescription", txtLastUpdateDescription.Text.Trim}
    }

        Dim filePath As String = taskInfo.DPHXPackageFilename
        Dim result As Boolean = UploadTaskToServer(taskData, filePath)

        If result Then
            'Nothing to do
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Failed to upload the task.", "Upload Result", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Function GetFileUpdateUTCDateTime(filePath As String, Optional inUTC As Boolean = True) As DateTime
        Dim fileInfo As New FileInfo(filePath)
        Dim localDateTime As DateTime = fileInfo.LastWriteTime
        If inUTC Then
            Return localDateTime.ToUniversalTime()
        Else
            Return localDateTime
        End If
    End Function

    Private Function ResizeImageAndGetBytes(inputPath As String, maxWidth As Integer, maxHeight As Integer, quality As Long) As Byte()
        ' Load the original image
        Using image As Image = Image.FromFile(inputPath)
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

    Private Function UploadTaskToServer(task As Dictionary(Of String, Object), dphxFilePath As String) As Boolean
        Try
            ' Serialize the task to JSON
            Dim json As String = JsonConvert.SerializeObject(task, New JsonSerializerSettings() With {
            .NullValueHandling = NullValueHandling.Ignore
        })

            ' Prepare the request
            Dim request As HttpWebRequest = CType(WebRequest.Create($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}CreateUpdateTaskFromDPHTool.php"), HttpWebRequest)
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
            Dim jpgFilePath As String = Path.Combine(Path.GetTempPath(), "weather_chart.jpg")
            SaveJpegWithQuality(bmp, jpgFilePath, 80L)

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

                ' Add JPG file
                AddFileToRequest(memStream, jpgFilePath, "image", boundaryBytes)

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
                    Return responseJson("status").ToString() = "success"
                End Using
            End Using
        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error uploading task: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

    Private Sub SaveJpegWithQuality(image As Drawing.Image, filePath As String, quality As Long)
        Dim qualityParam As New System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality)
        Dim jpegCodec As System.Drawing.Imaging.ImageCodecInfo = GetEncoderInfo("image/jpeg")
        Dim encoderParams As New System.Drawing.Imaging.EncoderParameters(1)
        encoderParams.Param(0) = qualityParam

        image.Save(filePath, jpegCodec, encoderParams)
    End Sub

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

    Private Function GetEncoderInfo(mimeType As String) As Imaging.ImageCodecInfo
        Dim codecs As Imaging.ImageCodecInfo() = Imaging.ImageCodecInfo.GetImageEncoders()
        For Each codec As Imaging.ImageCodecInfo In codecs
            If codec.MimeType = mimeType Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    Private Sub GetTaskDetails(taskID As String)
        Try
            Dim taskDetailsUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingID.php"

            ' Create the web request
            Dim request As HttpWebRequest = CType(WebRequest.Create(taskDetailsUrl & "?TaskID=" & taskID), HttpWebRequest)
            request.Method = "GET"
            request.ContentType = "application/json"

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    ' Check the status
                    If result("status").ToString() = "success" Then
                        _TBTaskEntrySeqID = result("taskDetails")("EntrySeqID")
                        ' Specify the format of the datetime string
                        Dim utcFormat As String = "yyyy-MM-dd HH:mm:ss"
                        Dim cultureInfo As CultureInfo = CultureInfo.InvariantCulture
                        ' Convert UTC datetime to local time before assigning to variables
                        _TBTaskDBEntryUpdate = DateTime.ParseExact(result("taskDetails")("DBEntryUpdate").ToString(), utcFormat, cultureInfo, DateTimeStyles.AssumeUniversal).ToLocalTime()
                        _TBTaskLastUpdate = DateTime.ParseExact(result("taskDetails")("LastUpdate").ToString(), utcFormat, cultureInfo, DateTimeStyles.AssumeUniversal).ToLocalTime()
                    Else
                        _TBTaskEntrySeqID = 0
                        Throw New Exception("Error retrieving task details: " & result("message").ToString())
                    End If
                End Using
            End Using

        Catch ex As Exception
            Throw New Exception("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub SetTBTaskDetailsLabel()

        Dim labelString As String = String.Empty
        Dim dateFormat As String = "yyyy-MM-dd HH:mm:ss"

        btnCreateInTaskBrowser.Enabled = False
        btnUpdateInTaskBrowser.Enabled = False
        txtLastUpdateDescription.Enabled = False
        btnDeleteFromTaskBrowser.Enabled = False

        If txtDiscordTaskID.Text.Trim = String.Empty Then
            lblTaskBrowserIDAndDate.Visible = False
            Exit Sub
        End If
        lblTaskBrowserIDAndDate.Visible = True

        If _TBTaskEntrySeqID > 0 Then
            'Verify if local DPH has been changed
            Dim localDPHTime As DateTime = GetFileUpdateUTCDateTime(_CurrentSessionFile, False)
            labelString = $"#{_TBTaskEntrySeqID.ToString} - Online ({_TBTaskLastUpdate}) - Local ({localDPHTime})"
            If _TBTaskLastUpdate.ToString(dateFormat) < localDPHTime.ToString(dateFormat) Then
                'Local file is more recent - allow change
                If UserCanUpdateTask Then
                    btnUpdateInTaskBrowser.Enabled = True
                    txtLastUpdateDescription.Enabled = True
                End If
                lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(255, 128, 0)
            Else
                lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(0, 192, 0)
            End If
            If UserCanDeleteTask Then
                btnDeleteFromTaskBrowser.Enabled = True
            End If
        Else
            labelString = "Task does not exist for the task browser yet"
            lblTaskBrowserIDAndDate.ForeColor = Color.FromArgb(255, 128, 0)
            If UserCanCreateTask Then
                btnCreateInTaskBrowser.Enabled = True
            End If
        End If

        lblTaskBrowserIDAndDate.Text = labelString

    End Sub

#End Region

#Region "Events Subs"

    Public Function PublishEventNews(key As String, title As String, subtitle As String, comments As String, eventDate As DateTime, published As DateTime, entrySeqID As Integer, urlToGo As String, expiration As DateTime) As Boolean
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}ManageNews.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "POST"
        request.ContentType = "application/x-www-form-urlencoded"

        Dim postData As String = $"action=CreateEvent&Key={Uri.EscapeDataString(key)}&Title={Uri.EscapeDataString(title)}&Subtitle={Uri.EscapeDataString(subtitle)}&Comments={Uri.EscapeDataString(comments)}&EventDate={Uri.EscapeDataString(eventDate.ToString("yyyy-MM-dd HH:mm:ss"))}&Published={Uri.EscapeDataString(published.ToString("yyyy-MM-dd HH:mm:ss"))}&EntrySeqID={entrySeqID}&URLToGo={Uri.EscapeDataString(urlToGo)}&Expiration={Uri.EscapeDataString(expiration.ToString("yyyy-MM-dd HH:mm:ss"))}"

        Dim data As Byte() = System.Text.Encoding.UTF8.GetBytes(postData)
        request.ContentLength = data.Length

        Try
            Using stream As Stream = request.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    ' Assuming the response is a JSON object with a "status" field
                    Dim result As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
                    Return result("status").ToString() = "success"
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"Error publishing news entry: {ex.Message}", "News publishing error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try
    End Function

#End Region

#End Region

End Class



