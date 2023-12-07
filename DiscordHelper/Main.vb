Imports System.Xml
Imports System.IO
Imports System.Xml.Serialization
Imports System.Text
Imports System.Threading
Imports System.Web.UI.WebControls
Imports System.Globalization
Imports SIGLR.SoaringTools.CommonLibrary
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports SIGLR.SoaringTools.ImageViewer
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar
Imports System.Linq.Expressions
Imports System.Web.UI
Imports System.Web
Imports System.Collections.Specialized

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

        _SF.FillCountryFlagList(cboCountryFlag.Items)

        ResetForm()

        SetTimePickerFormat()

        SessionSettings.Load()

        'Adjust some button position
        btnCopyAllSecPosts.Top = btnAltRestricCopy.Top

        RestoreMainFormLocationAndSize()

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

        chkExpertMode.Checked = SessionSettings.ExpertMode
        chkGroupSecondaryPosts.Checked = SessionSettings.MergeSecondaryPosts

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

        _XmlDocFlightPlan = New XmlDocument
        _XmlDocWeatherPreset = New XmlDocument
        _WeatherDetails = Nothing
        _FlightTotalDistanceInKm = 0
        _TaskTotalDistanceInKm = 0
        _PossibleElevationUpdateRequired = False
        lblElevationUpdateWarning.Visible = _PossibleElevationUpdateRequired

        cboSpeedUnits.SelectedIndex = 0
        cboDifficulty.SelectedIndex = 0
        cboVoiceChannel.Items.Clear()
        cboVoiceChannel.Items.AddRange(_SF.GetVoiceChannels.ToArray)
        cboMSFSServer.Items.Clear()
        cboMSFSServer.Items.AddRange(_SF.GetMSFSServers.ToArray)
        'cboRecommendedGliders.Text = String.Empty
        cboRecommendedGliders.Text = cboRecommendedGliders.Items(0)
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
        txtMaxAvgSpeed.Text = String.Empty
        txtMinAvgSpeed.Text = String.Empty
        txtDurationExtraInfo.Text = String.Empty
        txtDurationMin.Text = String.Empty
        txtDurationMax.Text = String.Empty
        txtDifficultyExtraInfo.Text = String.Empty
        txtShortDescription.Text = String.Empty
        chkDescriptionLock.Checked = False
        chkSuppressWarningForBaroPressure.Checked = False
        txtBaroPressureExtraInfo.Text = "Non standard: Set your altimeter! (Press ""B"" once in your glider)"
        txtCredits.Text = "All credits to @UserName for this task."
        txtLongDescription.Text = String.Empty
        chkLockCountries.Checked = False
        chkUseOnlyWeatherSummary.Checked = False
        txtWeatherSummary.Text = String.Empty
        txtAltRestrictions.Text = String.Empty
        txtWeatherFirstPart.Text = String.Empty
        txtWeatherWinds.Text = String.Empty
        txtWeatherClouds.Text = String.Empty
        txtFullDescriptionResults.Text = String.Empty
        cboGroupOrClubName.SelectedIndex = -1
        lblClubFullName.Text = String.Empty
        cboMSFSServer.SelectedIndex = -1
        cboVoiceChannel.SelectedIndex = -1
        chkDateTimeUTC.Checked = True
        chkUseSyncFly.Checked = False
        chkUseLaunch.Checked = False
        chkUseStart.Checked = False
        txtEventDescription.Text = String.Empty
        cboEligibleAward.SelectedIndex = -1
        txtGroupEventPostURL.Text = String.Empty
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

        _SF.PopulateSoaringClubList(cboGroupOrClubName.Items)
        _SF.AllWaypoints.Clear()

        SetVisibilityForSecPosts()
        'BuildFPResults()
        'BuildGroupFlightPost()
        SetFormCaption(String.Empty)

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
            SessionSettings.LastFileLoaded = _CurrentSessionFile
            SessionSettings.FlightPlanTabSplitterLocation = FlightPlanTabSplitter.SplitPosition
            SessionSettings.Save()
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
                    SetDiscordTaskThreadHeight()
            End Select
        End If

    End Sub

    Private Sub EnterTextBox(sender As Object, e As EventArgs) Handles txtWeatherWinds.Enter, txtWeatherSummary.Enter, txtWeatherFirstPart.Enter, txtWeatherClouds.Enter, txtTitle.Enter, txtSoaringTypeExtraInfo.Enter, txtSimDateTimeExtraInfo.Enter, txtShortDescription.Enter, txtMinAvgSpeed.Enter, txtMaxAvgSpeed.Enter, txtMainArea.Enter, txtLongDescription.Enter, txtGroupFlightEventPost.Enter, txtFullDescriptionResults.Enter, txtFPResults.Enter, txtFilesText.Enter, txtEventTitle.Enter, txtEventDescription.Enter, txtDurationMin.Enter, txtDurationMax.Enter, txtDurationExtraInfo.Enter, txtDiscordEventTopic.Enter, txtDiscordEventDescription.Enter, txtDifficultyExtraInfo.Enter, txtDepName.Enter, txtDepExtraInfo.Enter, txtCredits.Enter, txtArrivalName.Enter, txtArrivalExtraInfo.Enter, txtAltRestrictions.Enter, txtBaroPressureExtraInfo.Enter, txtOtherBeginnerLink.Enter, txtEventTeaserMessage.Enter
        SupportingFeatures.EnteringTextBox(sender)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Select Case TabControl1.SelectedTab.Name
            Case "tabBriefing"
                GenerateBriefing()
            Case "tabDiscord"
                BuildFPResults()
                BuildWeatherCloudLayers()
                BuildWeatherWindLayers()
                BuildWeatherInfoResults()
                SetDiscordTaskThreadHeight()
                BuildGroupFlightPost()
        End Select
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

#End Region


#End Region

#Region "Flight Plan Tab"

#Region "Event Handlers"

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
            Else
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "The URL you copied does not contain a valid ID for the task. The URL must come from a task published in the Task Library on Discord.", "Error extracting task ID", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        End If
    End Sub

    Private Sub btnDeleteDiscordID_Click(sender As Object, e As EventArgs) Handles btnDeleteDiscordID.Click
        Using New Centered_MessageBox(Me)
            If MessageBox.Show(Me, "Are you sure you want to clear the Discord ID from this task ?", "Please confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                txtDiscordTaskID.Text = String.Empty
            End If
        End Using
    End Sub

    Private Sub AllFieldChanges(sender As Object, e As EventArgs) Handles chkTitleLock.CheckedChanged,
                                                                          chkDepartureLock.CheckedChanged,
                                                                          chkArrivalLock.CheckedChanged,
                                                                          chkDescriptionLock.CheckedChanged,
                                                                          chkLockCountries.CheckedChanged,
                                                                          chkIncludeYear.CheckedChanged,
                                                                          chkSoaringTypeRidge.CheckedChanged,
                                                                          chkUseOnlyWeatherSummary.CheckedChanged,
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
                                                                          txtMinAvgSpeed.TextChanged,
                                                                          txtMaxAvgSpeed.TextChanged,
                                                                          txtDifficultyExtraInfo.TextChanged,
                                                                          txtGroupEventPostURL.TextChanged,
                                                                          txtDiscordEventShareURL.TextChanged,
                                                                          txtDiscordTaskID.TextChanged,
                                                                          txtBaroPressureExtraInfo.TextChanged,
                                                                          dtSimDate.ValueChanged,
                                                                          dtSimLocalTime.ValueChanged,
                                                                          cboRecommendedGliders.TextChanged,
                                                                          cboRecommendedGliders.SelectedIndexChanged,
                                                                          cboDifficulty.TextChanged,
                                                                          cboDifficulty.SelectedIndexChanged, txtOtherBeginnerLink.TextChanged, chkSoaringTypeDynamic.CheckedChanged, txtEventTeaserMessage.TextChanged

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

        SessionModified()

        'Some fields have an impact on the events tab
        If sender Is dtSimDate OrElse
           sender Is dtSimLocalTime OrElse
           sender Is chkIncludeYear Then
            CopyToEventFields(sender, e)
        End If

        If sender Is chkUseOnlyWeatherSummary Then
            WeatherFieldChangeDetection()
        End If

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
                                                                                           txtWeatherSummary.Leave, txtBaroPressureExtraInfo.Leave

        'BuildFPResults()

        'Some fields need to be copied to the Event tab
        If sender Is txtTitle OrElse sender Is txtShortDescription Then
            CopyToEventFields(sender, e)
        End If

        If sender Is txtWeatherSummary Then
            WeatherFieldChangeDetection()
        End If

        'For text box, make sure to display the value from the start
        If TypeOf sender Is Windows.Forms.TextBox Then
            _SF.RemoveForbiddenPrefixes(sender)
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

    Private Sub DurationNumberValidation(ByVal sender As Windows.Forms.TextBox, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtDurationMin.KeyPress, txtDurationMax.KeyPress, txtMaxAvgSpeed.KeyPress, txtMinAvgSpeed.KeyPress
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
            LoadFlightPlan(OpenFileDialog1.FileName)
        End If

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
        SessionModified()
        CalculateDuration()
    End Sub

    Private Sub NbrCarsCheckDiscordLimitEvent(sender As Object, e As EventArgs) Handles lblNbrCarsMainFP.TextChanged, lblNbrCarsFullDescResults.TextChanged, lblAllSecPostsTotalCars.TextChanged

        NbrCarsCheckDiscordLimit(DirectCast(sender, Windows.Forms.Label))

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

    Private Sub txtWaypointsDetails_TextChanged(sender As Object, e As EventArgs) Handles txtWaypointsDetails.TextChanged
        CalculateTotalNbrCars()
    End Sub

    Private Sub txtAddOnsDetails_TextChanged(sender As Object, e As EventArgs) Handles txtAddOnsDetails.TextChanged
        CalculateTotalNbrCars()
    End Sub

    Private Sub txtFullDescriptionResults_TextChanged(sender As Object, e As EventArgs) Handles txtFullDescriptionResults.TextChanged
        lblNbrCarsFullDescResults.Text = txtFullDescriptionResults.Text.Length
    End Sub

    Private Sub lblNbrCarsWeatherInfo_TextChanged(sender As Object, e As EventArgs) Handles lblNbrCarsWeatherWinds.TextChanged, lblNbrCarsWeatherInfo.TextChanged, lblNbrCarsWeatherClouds.TextChanged, lblNbrCarsRestrictions.TextChanged

        CalculateTotalNbrCars()

    End Sub

    Private Sub CopyToEventFields(sender As Object, e As EventArgs)

        If sender Is txtTitle Then
            txtEventTitle.Text = txtTitle.Text
        End If

        If sender Is txtShortDescription Then
            txtEventDescription.Text = txtShortDescription.Text
        End If

        'BuildGroupFlightPost()

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

#Region "Clipboard buttons on the Flight Plan Tab"

    Private Sub chkRepost_CheckedChanged(sender As Object, e As EventArgs) Handles chkRepost.CheckedChanged

        dtRepostOriginalDate.Enabled = chkRepost.Checked

    End Sub

    Private Sub chkExpertMode_CheckedChanged(sender As Object, e As EventArgs) Handles chkExpertMode.CheckedChanged
        SessionSettings.ExpertMode = chkExpertMode.Checked
    End Sub

    Private Sub btnFPMainInfoCopy_Click(sender As Object, e As EventArgs) Handles btnFPMainInfoCopy.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        BuildFPResults()

        'Sanity Check
        If HighlightExpectedFields(True) Then
            Return
        End If

        Clipboard.SetText(txtFPResults.Text)

        autoContinue = CopyContent.ShowContent(Me,
                                txtFPResults.Text,
                                $"You can now post the main flight plan message directly in the tasks/plans channel. Then get the link to that newly created post in Discord.{Environment.NewLine}Skip (Ok) if already done.", "Step 1 - Creating main FP post",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If Not autoContinue Then Exit Sub

        If txtDiscordTaskID.Text = String.Empty Then
            Dim message As String = "Please get the link to the task's post in Discord (""...More menu"" and ""Copy Message Link"")"
            Dim waitingForm As New WaitingForURLForm(message)
            Dim answer As DialogResult = waitingForm.ShowDialog()

            SupportingFeatures.BringDPHToolToTop(Me.Handle)

            'Check if the clipboard contains a valid URL, which would mean the task's URL has been copied
            If answer = DialogResult.OK Then
                Dim taskThreadURL As String
                taskThreadURL = Clipboard.GetText
                txtDiscordTaskID.Text = SupportingFeatures.ExtractMessageIDFromDiscordURL(taskThreadURL)
                SaveSession()
            End If
            If txtDiscordTaskID.Text = String.Empty Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "Take a minute to copy the Discord link to the task's you've just created and use the paste button on the Flight Plan tab.", "Copy URL to Task", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
                autoContinue = False
            End If
        End If

        Dim fpTitle As String = $"{txtTitle.Text}{AddFlagsToTitle()}"
        Clipboard.SetText(fpTitle)
        autoContinue = CopyContent.ShowContent(Me,
                            fpTitle,
                            "Now create a thread and position the cursor on the thread name field.", "Step 1 - Creating main FP post",
                            New List(Of String) From {"^v"},
                            SessionSettings.ExpertMode, False)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If
        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnFullDescriptionCopy_Click(sender, e)
        End If
    End Sub

    Private Sub btnFilesCopy_Click(sender As Object, e As EventArgs) Handles btnFilesCopy.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode
        Dim dlgResult As DialogResult

        Do While _sessionModified
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, "Latest changes have not been saved! You first need to save the session.", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.OK
                    btnSaveConfig_Click(btnFilesCopy, e)
                Case DialogResult.Cancel
                    Return
            End Select
        Loop

        Dim allFiles As New Specialized.StringCollection
        Dim contentForMessage As New StringBuilder
        GetAllFilesForMessage(allFiles, contentForMessage)

        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    contentForMessage.ToString,
                                    $"Make sure you are back on the thread's message field.{Environment.NewLine}Now paste the copied files as the second message in the thread WITHOUT posting it and come back for the text info (button 3b).",
                                    "Step 3a - Creating the files post in the thread - actual files first",
                                    New List(Of String) From {"^v"},
                                    SessionSettings.ExpertMode,
                                    False)
            If _GuideCurrentStep <> 0 Then
                _GuideCurrentStep += 1
                ShowGuide()
            End If
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "No files to copy!", "Step 3a - Creating the files post in the thread", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using
            autoContinue = False
        End If
        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnFilesTextCopy_Click(sender, e)
        End If

    End Sub

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

    Private Sub btnFilesTextCopy_Click(sender As Object, e As EventArgs) Handles btnFilesTextCopy.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        BuildFileInfoText()
        Clipboard.SetText(txtFilesText.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFilesText.Text,
                                "Now enter the file info in the second message in the thread and post it. Also pin this message in the thread.",
                                "Step 3b - Creating the files post in the thread - file info",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If
        If autoContinue AndAlso SessionSettings.ExpertMode Then
            If chkGroupSecondaryPosts.Checked Then
                btnCopyAllSecPosts_Click(sender, e)
            Else
                btnAltRestricCopy_Click(sender, e)
            End If
        End If

    End Sub

    Private Sub BuildFileInfoText()
        Dim sb As New StringBuilder
        sb.AppendLine("## 📁 **Files**")

        'Check if the DPHX package is included
        If File.Exists(txtDPHXPackageFilename.Text) Then
            sb.AppendLine("### DPHX Unpack & Load")
            sb.AppendLine("> Simply download the included **.DPHX** package and double-click it.")
            sb.AppendLine("> *To get and install the tool [click this link](https://flightsim.to/file/62573/msfs-soaring-task-tools-dphx-unpack-load)*")
            sb.AppendLine("> ")
            sb.AppendLine("> Otherwise, you must download the required files and put them in the proper folders.")
        Else
            sb.AppendLine("You must download the required files and put them in the proper folders.")
        End If

        sb.AppendLine("### Required")
        sb.AppendLine("> Flight plan (.pln)")
        sb.AppendLine("> Weather preset (.wpr)")

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

        txtFilesText.Text = sb.ToString.Trim
        sb.Clear()
    End Sub

    Private Sub btnAltRestricCopy_Click(sender As Object, e As EventArgs) Handles btnAltRestricCopy.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        BuildFPResults()
        BuildWeatherCloudLayers()
        BuildWeatherWindLayers()
        BuildWeatherInfoResults()

        Dim msg As String = txtAltRestrictions.Text & vbCrLf & vbCrLf & txtWeatherFirstPart.Text & vbCrLf & vbCrLf & txtWeatherWinds.Text & vbCrLf & vbCrLf & txtWeatherClouds.Text & vbCrLf

        If msg.Trim = String.Empty Then
            If sender Is btnAltRestricCopy Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "No restriction to post!", "Step 4 - Creating post for restrictions in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        Else
            Clipboard.SetText(msg)
            autoContinue = CopyContent.ShowContent(Me,
                                msg,
                                "Now paste the restrictions and weather content as the next message in the thread!",
                                "Step 4 - Creating post for restrictions in the thread.",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)
        End If

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnWaypointsCopy_Click(sender, e)
        End If

    End Sub

    Private Sub btnFullDescriptionCopy_Click(sender As Object, e As EventArgs) Handles btnFullDescriptionCopy.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        BuildFPResults()
        BuildWeatherCloudLayers()
        BuildWeatherWindLayers()
        BuildWeatherInfoResults()

        'Cover?
        If cboCoverImage.SelectedItem IsNot Nothing AndAlso cboCoverImage.SelectedItem.ToString <> String.Empty Then
            Dim allFiles As New Specialized.StringCollection
            If File.Exists(cboCoverImage.SelectedItem) Then
                allFiles.Add(cboCoverImage.SelectedItem)
                Clipboard.SetFileDropList(allFiles)
                autoContinue = CopyContent.ShowContent(Me,
                                    cboCoverImage.SelectedItem,
                                    $"Make sure you are back on the thread's message field.{Environment.NewLine}Now paste the copied cover image as the very first message in the task's thread.{Environment.NewLine}Skip (Ok) if already done.",
                                    "Step 2 - Posting the cover image for the task in the thread.",
                                    New List(Of String) From {"^v"},
                                    SessionSettings.ExpertMode)
            Else
                autoContinue = True
            End If
        Else
            autoContinue = True
        End If

        If Not autoContinue Then Exit Sub

        Clipboard.SetText(txtFullDescriptionResults.Text.Trim)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFullDescriptionResults.Text.Trim,
                                $"Make sure you are back on the thread's message field.{Environment.NewLine}Then post the full description as the first message in the task's thread.",
                                "Step 2 - Creating full description post in the thread.",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If
        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnFilesCopy_Click(sender, e)
        End If

    End Sub

    Private Sub btnWaypointsCopy_Click(sender As Object, e As EventArgs) Handles btnWaypointsCopy.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        BuildFPResults()
        BuildWeatherCloudLayers()
        BuildWeatherWindLayers()
        BuildWeatherInfoResults()

        If txtWaypointsDetails.Text.Length = 0 Then
            If sender Is btnWaypointsCopy Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "No waypoint to post!", "Step 5 - Creating waypoints post in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        Else
            Clipboard.SetText(txtWaypointsDetails.Text)
            autoContinue = CopyContent.ShowContent(Me,
                                txtWaypointsDetails.Text,
                                "Now post the waypoints details as the next message in the thread.",
                                "Step 5 - Creating waypoints post in the thread.",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)
        End If

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If
        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnAddOnsCopy_Click(sender, e)
        End If
    End Sub

    Private Sub btnAddOnsCopy_Click(sender As Object, e As EventArgs) Handles btnAddOnsCopy.Click

        BuildFPResults()
        BuildWeatherCloudLayers()
        BuildWeatherWindLayers()
        BuildWeatherInfoResults()

        If txtAddOnsDetails.Text.Length = 0 Then
            If sender Is btnAddOnsCopy Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show(Me, "No add-ons to post!", "Step 6 - Creating add-ons post in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End If
        Else
            Clipboard.SetText(txtAddOnsDetails.Text)
            CopyContent.ShowContent(Me,
                                txtAddOnsDetails.Text,
                                "Now post the add-ons details as the last message in the thread.",
                                "Step 6 - Creating add-ons post in the thread.",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)
            If _GuideCurrentStep <> 0 Then
                _GuideCurrentStep += 1
                ShowGuide()
            End If
        End If
    End Sub

    Private Sub chkGroupSecondaryPosts_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupSecondaryPosts.CheckedChanged

        SessionSettings.MergeSecondaryPosts = chkGroupSecondaryPosts.Checked
        SetVisibilityForSecPosts()

    End Sub

    Private Sub btnCopyAllSecPosts_Click(sender As Object, e As EventArgs) Handles btnCopyAllSecPosts.Click
        BuildFPResults()
        BuildWeatherCloudLayers()
        BuildWeatherWindLayers()
        BuildWeatherInfoResults()

        Dim msg As String = _SF.ValueToAppendIfNotEmpty(txtAltRestrictions.Text,,, 2) &
                          _SF.ValueToAppendIfNotEmpty(txtWeatherFirstPart.Text,,, 2) &
                          _SF.ValueToAppendIfNotEmpty(txtWeatherWinds.Text,,, 2) &
                          _SF.ValueToAppendIfNotEmpty(txtWeatherClouds.Text,,, 2) &
                          _SF.ValueToAppendIfNotEmpty(txtWaypointsDetails.Text,,, 1) &
                          _SF.ValueToAppendIfNotEmpty(txtAddOnsDetails.Text)

        If Not txtDiscordTaskID.Text.Trim = String.Empty Then
            msg = $"{msg}## 🏁 Results{Environment.NewLine}Feel free to share your task results in this thread, creating a central spot for everyone's achievements."
        End If

        If msg.Trim = String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "Nothing to post!", "Step 4 - Creating remaining content post in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        Else
            Clipboard.SetText(msg)

            CopyContent.ShowContent(Me,
                                msg,
                                "Now paste all remaining content as the next message in the thread!",
                                "Step 4 - Creating remaining content post in the thread.",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)
        End If

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If


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
        If Not (chkUseOnlyWeatherSummary.Checked Or _WeatherDetails Is Nothing) Then
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

    Private Sub CalculateTotalNbrCars()

        Dim intNbrTotal As Integer = 0

        intNbrTotal += _SF.GetIntegerFromString(lblNbrCarsRestrictions.Text)
        intNbrTotal += _SF.GetIntegerFromString(lblNbrCarsWeatherInfo.Text)
        intNbrTotal += _SF.GetIntegerFromString(lblNbrCarsWeatherWinds.Text)
        intNbrTotal += _SF.GetIntegerFromString(lblNbrCarsWeatherClouds.Text)
        intNbrTotal += _SF.GetIntegerFromString(txtWaypointsDetails.TextLength)
        intNbrTotal += _SF.GetIntegerFromString(txtAddOnsDetails.TextLength)

        lblAllSecPostsTotalCars.Text = (intNbrTotal).ToString

    End Sub

    Private Sub BuildFPResults()

        Dim sb As New StringBuilder()

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        sb.AppendLine($"# {txtTitle.Text}{AddFlagsToTitle()}")
        If chkRepost.Checked Then
            sb.AppendLine($"This task was originally posted on {dtRepostOriginalDate.Value.ToString("MMMM dd, yyyy", _EnglishCulture)}")
        End If
        sb.AppendLine()
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtShortDescription.Text,,, 2))
        If txtMainArea.Text.Trim.Length > 0 Then
            sb.AppendLine("> 🗺 " & _SF.ValueToAppendIfNotEmpty(txtMainArea.Text))
        End If
        sb.AppendLine($"> 🛫 {_SF.ValueToAppendIfNotEmpty(txtDepartureICAO.Text)}{_SF.ValueToAppendIfNotEmpty(txtDepName.Text, True)}{_SF.ValueToAppendIfNotEmpty(txtDepExtraInfo.Text, True, True)}")
        sb.AppendLine($"> 🛬 {_SF.ValueToAppendIfNotEmpty(txtArrivalICAO.Text)}{_SF.ValueToAppendIfNotEmpty(txtArrivalName.Text, True)}{_SF.ValueToAppendIfNotEmpty(txtArrivalExtraInfo.Text, True, True)}")
        sb.AppendLine($"> ⌚ {dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local in MSFS{_SF.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text.Trim, True, True)}")
        sb.AppendLine($"> ↗️ {GetSoaringTypesSelected()}{_SF.ValueToAppendIfNotEmpty(txtSoaringTypeExtraInfo.Text, True, True)}")
        sb.AppendLine($"> 📏 {_SF.GetDistance(txtDistanceTotal.Text, txtDistanceTrack.Text)}")
        sb.AppendLine($"> ⏳ {_SF.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{_SF.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}")
        sb.AppendLine($"> ✈️ {_SF.ValueToAppendIfNotEmpty(cboRecommendedGliders.Text)}")
        sb.AppendLine($"> 🎚 {_SF.GetDifficulty(cboDifficulty.SelectedIndex, txtDifficultyExtraInfo.Text)}")
        sb.AppendLine()
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtCredits.Text,,, 1))
        sb.Append("### See inside thread for most up-to-date files and more information.")
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
                txtDurationMax.Text = FormatNumber(_SF.RoundTo15Minutes((totalDistanceInKm / minAvgspeedInKmh) * 60), 0)
            End If
            If maxAvgspeedInKmh > 0 Then
                txtDurationMin.Text = FormatNumber(_SF.RoundTo15Minutes((totalDistanceInKm / maxAvgspeedInKmh) * 60), 0)
            End If
        End If

        'BuildFPResults()

    End Sub

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

    Private Sub SetVisibilityForSecPosts()

        If chkGroupSecondaryPosts.Checked Then
            btnCopyAllSecPosts.Visible = True
            btnAltRestricCopy.Visible = False
            btnWaypointsCopy.Visible = False
            btnAddOnsCopy.Visible = False
            If pnlWizardDiscord.Visible AndAlso _GuideCurrentStep >= 45 AndAlso _GuideCurrentStep < 59 Then
                _GuideCurrentStep = 45
                ShowGuide()
            End If
        Else
            btnCopyAllSecPosts.Visible = False
            btnAltRestricCopy.Visible = True
            btnWaypointsCopy.Visible = True
            btnAddOnsCopy.Visible = True
            If pnlWizardDiscord.Visible AndAlso _GuideCurrentStep = 48 Then
                _GuideCurrentStep = 45
                ShowGuide()
            End If
        End If

        SetDiscordTaskThreadHeight()

        NbrCarsCheckDiscordLimit(lblAllSecPostsTotalCars, True)

    End Sub

    Private Sub NbrCarsCheckDiscordLimit(lblLabel As Windows.Forms.Label, Optional skipSetHeight As Boolean = False)
        Select Case CInt(lblLabel.Text)
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

    Private Sub SetDiscordTaskThreadHeight()

        Dim height As Integer = 0

        If chkGroupSecondaryPosts.Checked Then
            height = 120
            If lblAllSecPostsTotalCars.Visible Then
                height += 26
            End If
        Else
            height = 57 * 4
            If lblNbrCarsFullDescResults.Visible Then
                height += 26
            End If
        End If

        grpDiscordTaskThread.Height = height + 175
        'grpDiscordTask.Height = grpDiscordTaskThread.Height + 214

    End Sub

    Private Sub LoadWeatherfile(filename As String)
        'read file
        txtWeatherFile.Text = filename
        _XmlDocWeatherPreset.Load(filename)

        _WeatherDetails = Nothing
        _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        BuildWeatherInfoResults()
        'BuildGroupFlightPost()

        If Not (chkUseOnlyWeatherSummary.Checked Or _WeatherDetails Is Nothing) Then
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

        If chkUseOnlyWeatherSummary.Checked Or _WeatherDetails Is Nothing Then
            sb.Append($"- Summary: {_SF.ValueToAppendIfNotEmpty(txtWeatherSummary.Text, nbrLineFeed:=1)}")
        Else
            sb.Append($"- Weather file & profile name: ""{Path.GetFileName(txtWeatherFile.Text)}"" ({_WeatherDetails.PresetName}){Environment.NewLine}")
            If Not txtWeatherSummary.Text.Trim = String.Empty Then
                sb.Append($"- Summary: {_SF.ValueToAppendIfNotEmpty(txtWeatherSummary.Text)}{Environment.NewLine}")
            End If
            sb.Append($"- Elevation measurement: {_WeatherDetails.AltitudeMeasurement}{Environment.NewLine}")
            sb.Append($"- MSLPressure: {_WeatherDetails.MSLPressure(txtBaroPressureExtraInfo.Text, chkSuppressWarningForBaroPressure.Checked)}{Environment.NewLine}")
            sb.Append($"- MSLTemperature: {_WeatherDetails.MSLTemperature}{Environment.NewLine}")
            sb.Append($"- Humidity: {_WeatherDetails.Humidity}")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"{Environment.NewLine}- Precipitations: {_WeatherDetails.Precipitations}")
            End If
            If _WeatherDetails.HasSnowCover Then
                sb.Append($"{Environment.NewLine}- Snow Cover: {_WeatherDetails.SnowCover}")
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

    Private Sub ClubSelected(sender As Object, e As EventArgs) Handles cboGroupOrClubName.SelectedIndexChanged, cboGroupOrClubName.TextChanged

        Dim clubExists As Boolean = _SF.DefaultKnownClubEvents.ContainsKey(cboGroupOrClubName.Text.ToUpper)
        lblClubFullName.Text = String.Empty

        If clubExists Then
            _ClubPreset = _SF.DefaultKnownClubEvents(cboGroupOrClubName.Text.ToUpper)
            cboGroupOrClubName.Text = _ClubPreset.ClubId
            lblClubFullName.Text = _ClubPreset.ClubFullName
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

    Private Sub btnDiscordGroupEventURL_Click(sender As Object, e As EventArgs) Handles btnDiscordGroupEventURL.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtGroupEventPostURL.Text = Clipboard.GetText
        End If
        'BuildDiscordEventDescription()
    End Sub

    Private Sub btnDiscordSharedEventURL_Click(sender As Object, e As EventArgs) Handles btnDiscordSharedEventURL.Click
        If SupportingFeatures.IsValidURL(Clipboard.GetText) Then
            txtDiscordEventShareURL.Text = Clipboard.GetText
        End If
    End Sub

    Private Sub btnGroupFlightEventInfoToClipboard_Click(sender As Object, e As EventArgs) Handles btnGroupFlightEventInfoToClipboard.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        If cboCoverImage.SelectedItem IsNot Nothing AndAlso cboCoverImage.SelectedItem.ToString <> String.Empty Then
            Dim allFiles As New Specialized.StringCollection
            If File.Exists(cboCoverImage.SelectedItem) Then
                allFiles.Add(cboCoverImage.SelectedItem)
                Clipboard.SetFileDropList(allFiles)
                autoContinue = CopyContent.ShowContent(Me,
                                    cboCoverImage.SelectedItem,
                                    $"You will start by just pasting the copied cover image For your New group flight Event.{Environment.NewLine}Skip (Ok) If already done.",
                                    "Creating group flight post",
                                    New List(Of String) From {"^v"},
                                    SessionSettings.ExpertMode,
                                    False)
            Else
                autoContinue = True
            End If
        Else
            autoContinue = True
        End If

        If Not autoContinue Then Exit Sub

        BuildGroupFlightPost()
        Clipboard.SetText(txtGroupFlightEventPost.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtGroupFlightEventPost.Text,
                                $"You can now post the group flight Event In the proper Discord channel For the club/group.{Environment.NewLine}Then copy the link To that newly created message.{Environment.NewLine}Finally, paste the link In the URL field just below For Discord Event.",
                                "Creating group flight post",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If Not autoContinue Then Exit Sub

        If txtGroupEventPostURL.Text = String.Empty Then
            Dim message As String = "Please Get the link To the group Event's post in Discord (""...More menu"" and ""Copy Message Link"")"
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

        Dim fpTitle As New StringBuilder
        If txtEventTitle.Text <> String.Empty Then
            If cboGroupOrClubName.SelectedIndex > -1 Then
                fpTitle.Append($"{_ClubPreset.ClubName} - ")
            Else
                fpTitle.Append($"")
            End If
            fpTitle.AppendLine(txtEventTitle.Text & AddFlagsToTitle())
        End If
        Clipboard.SetText(fpTitle.ToString)
        autoContinue = CopyContent.ShowContent(Me,
                            fpTitle.ToString,
                            "Now create a thread and position the cursor on the thread name field.", "Creating group flight thread",
                            New List(Of String) From {"^v"},
                            SessionSettings.ExpertMode, False)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

        If autoContinue AndAlso SessionSettings.ExpertMode Then
            If btnGroupFlightEventTeaser.Enabled Then
                btnGroupFlightEventTeaser_Click(sender, e)
            Else

            End If
        End If
    End Sub
    Private Sub btnGroupFlightEventTeaser_Click(sender As Object, e As EventArgs) Handles btnGroupFlightEventTeaser.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        If txtEventTeaserAreaMapImage.Text <> String.Empty AndAlso File.Exists(txtEventTeaserAreaMapImage.Text) Then
            Dim allFiles As New Specialized.StringCollection
            allFiles.Add(txtEventTeaserAreaMapImage.Text)
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    txtEventTeaserAreaMapImage.Text,
                                    $"Position the cursor on the message field in the group event thread and paste the copied teaser image for your first message.{Environment.NewLine}Skip (Ok) if already done.",
                                    "Pasting teaser area map image",
                                    New List(Of String) From {"^v"},
                                    SessionSettings.ExpertMode,
                                    False)
        Else
            autoContinue = True
        End If

        If Not autoContinue Then Exit Sub

        'Teaser message
        If txtEventTeaserMessage.Text.Trim <> String.Empty Then
            Dim teaser As New StringBuilder
            teaser.AppendLine("# 🤐 Teaser")
            teaser.AppendLine(txtEventTeaserMessage.Text.Trim)
            Clipboard.SetText(teaser.ToString)
            autoContinue = CopyContent.ShowContent(Me,
                            teaser.ToString,
                            $"Make sure you are back on the thread's message field.{Environment.NewLine}Then post the teaser message as the first message in the event's thread.",
                            "Posting teaser.",
                            New List(Of String) From {"^v"},
                            SessionSettings.ExpertMode)
        End If

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

    Private Sub btnGroupFlightEventThreadLogistics_Click(sender As Object, e As EventArgs) Handles btnGroupFlightEventThreadLogistics.Click

        Dim logisticInstructions As New StringBuilder

        logisticInstructions.AppendLine("## 🗣 Event Logistics")
        logisticInstructions.AppendLine("**Use this thread only to discuss logistics for this event!**")
        logisticInstructions.AppendLine("> Focus on:")
        logisticInstructions.AppendLine("> - Event logistics, such as meet-up times and locations")
        logisticInstructions.AppendLine("> - Providing feedback on the event's organization and coordination")
        logisticInstructions.AppendLine("## :octagonal_sign: Reports, screenshots, and feedback on the task itself should go in the task's thread please!")
        logisticInstructions.AppendLine($"⏩ [{txtEventTitle.Text.Trim}](https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{txtDiscordTaskID.Text})")

        Clipboard.SetText(logisticInstructions.ToString)
        CopyContent.ShowContent(Me,
                                logisticInstructions.ToString,
                                "Now paste the message content into the thread and post it.",
                                "Creating group flight post",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If


    End Sub

    Private Sub btnEventTopicClipboard_Click(sender As Object, e As EventArgs) Handles btnEventTopicClipboard.Click
        BuildDiscordEventDescription()
        If txtDiscordEventTopic.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventTopic.Text)
            CopyContent.ShowContent(Me,
                                txtDiscordEventTopic.Text,
                                "Paste the topic into the Event Topic field on Discord.",
                                "Creating Discord Event",
                                New List(Of String) From {"^v"},
                                True)

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
                                New List(Of String) From {"^v"},
                                True)

        End If
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

    Private Sub chkIncludeBeginnersHelpLink_CheckedChanged(sender As Object, e As EventArgs)
        'BuildGroupFlightPost()
        SessionModified()
    End Sub

    Private Sub bbtnEventFilesAndFilesInfo_Click(sender As Object, e As EventArgs) Handles btnEventFilesAndFilesInfo.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        Dim dlgResult As DialogResult

        Do While _sessionModified
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, "Latest changes have not been saved! You first need to save the session.", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.OK
                    btnSaveConfig_Click(btnFilesCopy, e)
                Case DialogResult.Cancel
                    Return
            End Select
        Loop

        Dim allFiles As New Specialized.StringCollection
        Dim contentForMessage As New StringBuilder

        GetAllFilesForMessage(allFiles, contentForMessage)

        If allFiles.Count > 0 Then
            Clipboard.SetFileDropList(allFiles)
            autoContinue = CopyContent.ShowContent(Me,
                                    contentForMessage.ToString,
                                    "Now paste the copied files in a new post under the group event's thread and come back for the text info (coming next).",
                                    "Including the required files in the group flight thread",
                                    New List(Of String) From {"^v"},
                                    SessionSettings.ExpertMode,
                                    False)
        End If

        If Not autoContinue Then Exit Sub

        BuildFileInfoText()
        Clipboard.SetText(txtFilesText.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFilesText.Text,
                                "Now enter the file info in the second message in the thread and post it. Also pin this message in the thread.",
                                "Step 3b - Creating the files post in the thread - file info",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnEventTaskDetails_Click(sender, e)
        End If

    End Sub

    Private Sub btnEventDPHXAndLinkOnly_Click(sender As Object, e As EventArgs) Handles btnEventDPHXAndLinkOnly.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        Dim dlgResult As DialogResult

        Do While _sessionModified
            Using New Centered_MessageBox(Me)
                dlgResult = MessageBox.Show(Me, "Latest changes have not been saved! You first need to save the session.", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            End Using
            Select Case dlgResult
                Case DialogResult.OK
                    btnSaveConfig_Click(btnFilesCopy, e)
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
                                    "Including the required files in the group flight thread",
                                    New List(Of String) From {"^v"},
                                    SessionSettings.ExpertMode,
                                    False)
        End If

        If Not autoContinue Then Exit Sub

        txtFilesText.Text = $"**DPHX file** for people using it and [even more details here]({$"https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{SupportingFeatures.GetMSFSSoaringToolsLibraryID}/{txtDiscordTaskID.Text}"})."
        Clipboard.SetText(txtFilesText.Text)
        autoContinue = CopyContent.ShowContent(Me,
                                txtFilesText.Text,
                                "Now enter the file info in the second message in the thread and post it. Also pin this message in the thread.",
                                "Step 3b - Creating the files post in the thread - file info",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnEventTaskDetails_Click(sender, e)
        End If

    End Sub

    Private Sub btnEventTaskDetails_Click(sender As Object, e As EventArgs) Handles btnEventTaskDetails.Click

        Dim autoContinue As Boolean = SessionSettings.ExpertMode

        Dim taskDetails As String = BuildLightTaskDetailsForEventPost()
        Clipboard.SetText(taskDetails)
        autoContinue = CopyContent.ShowContent(Me,
                                taskDetails,
                                "Now paste the remaining and relevant task details for the group flight event.",
                                "Pasting remaining task info",
                                New List(Of String) From {"^v"},
                                SessionSettings.ExpertMode)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

        If autoContinue AndAlso SessionSettings.ExpertMode Then
            btnGroupFlightEventThreadLogistics_Click(sender, e)
        End If

    End Sub
    Private Sub EventTabTextControlLeave(sender As Object, e As EventArgs) Handles txtGroupFlightEventPost.Leave, txtEventTitle.Leave, txtEventDescription.Leave, txtDiscordEventTopic.Leave, txtDiscordEventDescription.Leave, txtOtherBeginnerLink.Leave, txtEventTeaserMessage.Leave

        _SF.RemoveForbiddenPrefixes(sender)
        LeavingTextBox(sender)
        'BuildGroupFlightPost()
        'BuildDiscordEventDescription()
    End Sub

    Private Sub chkActivateEvent_CheckedChanged(sender As Object, e As EventArgs) Handles chkActivateEvent.CheckedChanged
        grpGroupEventPost.Enabled = chkActivateEvent.Checked
        grpDiscordGroupFlight.Enabled = chkActivateEvent.Checked
        SessionModified()
    End Sub

    Private Sub chkEventTeaser_CheckedChanged(sender As Object, e As EventArgs) Handles chkEventTeaser.CheckedChanged
        grpEventTeaser.Enabled = chkEventTeaser.Checked
        btnGroupFlightEventTeaser.Enabled = chkEventTeaser.Checked
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
            SessionModified()
        End If

    End Sub

    Private Sub btnTaskFeaturedOnGroupFlight_Click(sender As Object, e As EventArgs) Handles btnTaskFeaturedOnGroupFlight.Click

        If _ClubPreset Is Nothing Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "No club selected for the event!", "Discord Post Helper tool", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using
            Exit Sub
        End If

        Dim fullMeetDateTimeLocal As DateTime = _SF.GetFullEventDateTimeInLocal(dtEventMeetDate, dtEventMeetTime, chkDateTimeUTC.Checked)

        Dim sb As New StringBuilder

        sb.AppendLine("## :calendar: Group Flight")
        sb.AppendLine($"This flight will be featured on the {_ClubPreset.ClubFullName} group flight of {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time.")

        'check which shared link is available
        If txtDiscordEventShareURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtDiscordEventShareURL.Text.Trim) Then
            sb.AppendLine($"{txtDiscordEventShareURL.Text}")
        ElseIf txtGroupEventPostURL.Text.Trim <> String.Empty AndAlso SupportingFeatures.IsValidURL(txtGroupEventPostURL.Text.Trim) Then
            sb.AppendLine($"[{_ClubPreset.ClubFullName} - Group Event Link]({txtGroupEventPostURL.Text})")
        End If

        Clipboard.SetText(sb.ToString)
        CopyContent.ShowContent(Me,
                                sb.ToString,
                                "On the task's thread, paste the content of the message to share the event for this task.",
                                "Sharing Discord Event to Task",
                                New List(Of String) From {"^v"},
                                True)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
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

    Private Function BuildLightTaskDetailsForEventPost() As String

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        If Not txtDiscordTaskID.Text = String.Empty Then
            sb.AppendLine("## ❗ Final task details and reminders")
            sb.AppendLine($"> 🔗 [Link to complete task details, including full briefing, restrictions, weather and more]({$"https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{SupportingFeatures.GetMSFSSoaringToolsLibraryID}/{txtDiscordTaskID.Text}"})")
            sb.AppendLine("> *If you did not join MSFS Soaring Task Tools already, you will need this [invite link](https://discord.gg/aW8YYe3HJF) first*")
            sb.AppendLine("> ")
        End If

        If Not txtFlightPlanFile.Text = String.Empty Then
            sb.AppendLine($"> 📁 Flight plan file: **""{Path.GetFileName(txtFlightPlanFile.Text)}""**")
        End If
        If txtWeatherFile.Text <> String.Empty AndAlso (_WeatherDetails IsNot Nothing) Then
            sb.AppendLine("> 🌤 Weather file & profile name: **""" & Path.GetFileName(txtWeatherFile.Text) & """ (" & _WeatherDetails.PresetName & ")**")
        End If
        sb.AppendLine("> ")

        sb.AppendLine($"> 📆 Sim date and time: **{dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local** {_SF.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True)}")

        If chkUseSyncFly.Checked Then
            sb.AppendLine("### :octagonal_sign: Stay on the world map to synchronize weather :octagonal_sign:")
        End If

        sb.AppendLine()
        sb.AppendLine($"*Don't forget to review the details for this group flight event (first post of the thread).*")

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

        sb.AppendLine($"## {Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("dddd, MMMM dd", _EnglishCulture)}, {Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time")
        sb.AppendLine()

        If txtEventTitle.Text <> String.Empty Then
            If cboGroupOrClubName.SelectedIndex > -1 Then
                sb.Append($"# {_ClubPreset.ClubName} - ")
            Else
                sb.Append($"# ")
            End If
            sb.AppendLine(txtEventTitle.Text & AddFlagsToTitle())
        End If
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))

        sb.AppendLine($"## 💼 Meet/Briefing{Environment.NewLine}> **{Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("dddd, MMMM dd", _EnglishCulture)}, {Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time**{Environment.NewLine}> *At this time we meet in the voice chat and get ready.*")
        sb.AppendLine("> ")
        sb.AppendLine($"> 🗣 Voice: **{cboVoiceChannel.Text}**")

        If chkEventTeaser.Checked Then
            sb.AppendLine("> ")
            sb.AppendLine($"> 📁 All files will be shared inside the thread below, a few hours before the actual event takes place")
        Else
            sb.AppendLine("> ")
            sb.AppendLine($"> 📁 All files are shared inside the thread below")
        End If
        sb.AppendLine("> ")
        sb.AppendLine($"> 🌐 Server: **{cboMSFSServer.Text}**")

        Dim theLocalTime As String = String.Empty
        If chkUseSyncFly.Checked Then
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        ElseIf chkUseLaunch.Checked Then
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        Else
            theLocalTime = $"{_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)}"
        End If
        sb.AppendLine($"> 📆 Sim date and time: **{dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local **(when it is {theLocalTime} in your own local time){_SF.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True)}")

        sb.AppendLine("> ")

        If chkUseSyncFly.Checked Then
            sb.AppendLine("### :octagonal_sign: Stay on the world map to synchronize weather :octagonal_sign:")
            sb.AppendLine($"## ⏱️ Sync Fly: {Environment.NewLine}> **{Conversions.ConvertLocalToUTC(fullSyncFlyDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)} your local time ({fullSyncFlyDateTimeMSFS.ToString("hh:mm tt", _EnglishCulture)} in MSFS)**{Environment.NewLine}> *At this time we simultaneously click fly to sync our weather.*")
            If chkUseLaunch.Checked AndAlso fullSyncFlyDateTimeLocal = fullLaunchDateTimeLocal Then
                sb.AppendLine("> *At this time we can also start launching from the airfield.*")
            End If
            sb.AppendLine("> ")
        End If

        If chkUseLaunch.Checked AndAlso (fullSyncFlyDateTimeLocal <> fullLaunchDateTimeLocal OrElse Not chkUseSyncFly.Checked) Then
            sb.AppendLine($"## 🚀 Launch:{Environment.NewLine}> **{Conversions.ConvertLocalToUTC(fullLaunchDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)} your local time ({fullLaunchDateTimeMSFS.ToString("hh:mm tt", _EnglishCulture)} in MSFS)**{Environment.NewLine}> *At this time we can start launching from the airfield.*")
            sb.AppendLine("> ")
        End If

        If chkUseStart.Checked Then
            sb.AppendLine($"## 🟢 Task Start:{Environment.NewLine}> **{Conversions.ConvertLocalToUTC(fullStartTaskDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullStartTaskDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)} your local time ({fullStartTaskDateTimeMSFS.ToString("hh:mm tt", _EnglishCulture)} in MSFS)**{Environment.NewLine}> *At this time we cross the starting line and start the task.*")
            sb.AppendLine("> ")
        End If

        sb.AppendLine()
        sb.AppendLine($"⏳ Duration: **{_SF.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{_SF.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}**")

        If cboEligibleAward.SelectedIndex > 0 Then
            sb.AppendLine()
            sb.AppendLine($"🏆 Pilots who finish this task successfully during the event will be eligible to apply for the **{cboEligibleAward.Text} Soaring Badge** :{cboEligibleAward.Text.ToLower()}:")
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
            sb.AppendLine()
            sb.AppendLine($"‍:student: If it's your first time flying with us, please make sure to read the following guide: {urlBeginnerGuide}")
        End If

        If txtCredits.Text <> String.Empty Then
            sb.AppendLine()
            sb.AppendLine(txtCredits.Text)
        End If

        If SupportingFeatures.IsValidURL(txtDiscordEventShareURL.Text) Then
            sb.AppendLine()
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
        sb.AppendLine($"**Duration:** {_SF.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{_SF.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}")
        sb.AppendLine()
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))
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

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles TabControl1.KeyDown, Me.KeyDown
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
        If e.Control AndAlso e.KeyCode = Keys.S AndAlso _sessionModified Then
            ' Handle the CTRL-S key combination (e.g., save the file)
            SaveSession()
            e.SuppressKeyPress = True ' This prevents the beep sound
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
            Case 17 'Countries
                SetGuidePanelToRight()
                pnlGuide.Top = 28
                lblGuideInstructions.Text = "Countries to show in the topic should be read from the waypoints, but you can optionally specify them yourself here."
                SetFocusOnField(cboCountryFlag, fromF1Key)
            Case 18 'Weather summary
                SetGuidePanelToRight()
                pnlGuide.Top = 102
                lblGuideInstructions.Text = "Optional weather summary. If you don't want the full weather details to be included, tick the checkbox to the left. Only the summary will then be shown."
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
                lblDiscordGuideInstructions.Text = "If this is a repost on an existing task, enable this to set the original date the task was published."
                SetFocusOnField(chkRepost, fromF1Key)
            Case 41 'Create FP post
                TabControl1.SelectedTab = TabControl1.TabPages("tabDiscord")
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 110
                lblDiscordGuideInstructions.Text = "You are now ready to create the task's primary post in Discord. Click this button to copy the content to your clipboard and receive instructions."
                SetFocusOnField(btnFPMainInfoCopy, fromF1Key)
            Case 42 'Copy Description
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 223
                lblDiscordGuideInstructions.Text = "Click this button to copy the full description to your clipboard and receive instructions."
                SetFocusOnField(btnFullDescriptionCopy, fromF1Key)
            Case 43 'Copy Files
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 307
                lblDiscordGuideInstructions.Text = "Once you've created the primary post and thread on Discord, click this button to put the files into your clipboard and receive instructions."
                SetFocusOnField(btnFilesCopy, fromF1Key)
            Case 44 'Copy Files Legend
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 360
                lblDiscordGuideInstructions.Text = "Once you've pasted the files in Discord, click this button to put the standard legend into your clipboard and receive instructions."
                SetFocusOnField(btnFilesTextCopy, fromF1Key)
            Case 45 'Merge remaining content
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 405
                lblDiscordGuideInstructions.Text = "You can select to merge all remaining content in a single post, depending also on the size. Or, you can do individual posts in the thread."
                SetFocusOnField(chkGroupSecondaryPosts, fromF1Key)
            Case 46 'Remaining content OR Restrictions & Weather
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = 449
                If chkGroupSecondaryPosts.Checked Then
                    lblDiscordGuideInstructions.Text = "You can now create the last post with all remaining task information. Watch out for Discord's post size limit!"
                    SetFocusOnField(btnCopyAllSecPosts, fromF1Key)
                    _GuideCurrentStep = 48

                Else
                    lblDiscordGuideInstructions.Text = "Altitude Restrictions and Weather info - You can click this button and receive instructions for this next post."
                    SetFocusOnField(btnAltRestricCopy, fromF1Key)
                End If

            Case 47 'Waypoints
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = btnWaypointsCopy.Top + 150
                lblDiscordGuideInstructions.Text = "Click this button to copy the waypoints details (very useful for xBox users) to your clipboard and receive instructions."
                SetFocusOnField(btnWaypointsCopy, fromF1Key)
            Case 48 'Recommended add-ons
                SetDiscordGuidePanelToLeft()
                pnlWizardDiscord.Top = btnAddOnsCopy.Top + 150
                lblDiscordGuideInstructions.Text = "Finally, click this button to copy the recommended add-ons to your clipboard and receive instructions."
                SetFocusOnField(btnAddOnsCopy, fromF1Key)

            Case 49 To 59 'Next section
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
                lblEventGuideInstructions.Text = "If you would like to specify a different description for the group flight, you can do so now. Otherwise, this is the same as the task's short description."
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

            Case 80 'Create Group Event Flight post
                TabControl1.SelectedTab = TabControl1.TabPages("tabDiscord")
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 94
                lblDiscordGuideInstructions.Text = "You are now ready to create the group flight post in Discord. Click this button to copy the group flight's post content and receive instructions."
                SetFocusOnField(btnGroupFlightEventInfoToClipboard, fromF1Key)

            Case 81 'Group flight URL
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 134
                lblDiscordGuideInstructions.Text = "From Discord, copy the link to the group flight post you just created above, and click ""Paste"" here."
                SetFocusOnField(btnDiscordGroupEventURL, fromF1Key)

            Case 82 'Group flight Teaser message
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 174
                lblDiscordGuideInstructions.Text = "You have selected to post a teaser. Click this button to copy the teaser's post content and receive instructions."
                SetFocusOnField(btnGroupFlightEventTeaser, fromF1Key)

            Case 83 'Task files and info
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 217
                lblDiscordGuideInstructions.Text = "Click and follow the instructions to post all task files and the file notice information."
                SetFocusOnField(btnEventFilesAndFilesInfo, fromF1Key)

            Case 84 'Relevant task details
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 259
                lblDiscordGuideInstructions.Text = "Click this button to copy the relevant task details for the group event and receive instructions."
                SetFocusOnField(btnEventTaskDetails, fromF1Key)

            Case 85 'Group flight thread logistic instructions
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 303
                lblDiscordGuideInstructions.Text = "Click this button to copy the group flight's thread logistic information and and receive instructions."
                SetFocusOnField(btnGroupFlightEventTeaser, fromF1Key)

            Case 86 'Discord Event
                Using New Centered_MessageBox(Me)
                    If MessageBox.Show("Do you have the access rights to create Discord Event on the target Discord Server? Click No if you don't know.", "Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        _GuideCurrentStep += 1
                    Else
                        _GuideCurrentStep = AskWhereToGoNext()
                    End If
                End Using
                ShowGuide()

            Case 87 'Create Discord Event
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 382
                lblDiscordGuideInstructions.Text = "In Discord and in the proper Discord Server, start the creation of a new Event (Create Event). If you don't know how to do this, ask for help!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 88 'Select voice channel for event
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 420
                lblDiscordGuideInstructions.Text = "On the new event window, under ""Where is your event"", choose ""Voice Channel"" and select this voice channel. Then click ""Next"" on the event window."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 89 'Topic name
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 458
                lblDiscordGuideInstructions.Text = "Click this button to copy the event topic and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventTopicClipboard, fromF1Key)

            Case 90 'Event date & time
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 490
                lblDiscordGuideInstructions.Text = "On the Discord event window, specify the date and time displayed here - these are all local times you have to use!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 91 'Event description
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 529
                lblDiscordGuideInstructions.Text = "Click this button to copy the event description and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventDescriptionToClipboard, fromF1Key)

            Case 92 'Cover image
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 566
                lblDiscordGuideInstructions.Text = "In the Discord event window, you can also upload a cover image for your event. This is optional."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 93 'Preview and publish
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 603
                lblDiscordGuideInstructions.Text = "In the Discord event window, click Next to review your event information and publish it."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 94 'Paste link to Discord Event
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 649
                lblDiscordGuideInstructions.Text = "From the Discord Event published window, copy the URL to share to and invite participants and click ""Paste"" here."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 95 'Share the Discord Event on the task
                SetDiscordGuidePanelToTopArrowLeftSide()
                pnlWizardDiscord.Top = 742
                lblDiscordGuideInstructions.Text = "Click this button to copy the message to post on the task and receive instructions to paste it in the Discord."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 96 To 99
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
                SetDiscordTaskThreadHeight()
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
            LoadSessionData(selectedFilename)
            _CurrentSessionFile = selectedFilename
            GenerateBriefing()
            _loadingFile = False
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
            .ShortDescription = txtShortDescription.Text.Replace(Environment.NewLine, "($*$)")
            .Credits = txtCredits.Text
            .LongDescription = txtLongDescription.Text.Replace(Environment.NewLine, "($*$)")
            .WeatherSummaryOnly = chkUseOnlyWeatherSummary.Checked
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
                .GroupClubName = _ClubPreset.ClubName
            Else
                .GroupClubName = String.Empty
            End If
            .DiscordTaskID = txtDiscordTaskID.Text
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
                cboSpeedUnits.SelectedIndex = .AvgSpeedsUnit
                txtMinAvgSpeed.Text = .AvgMinSpeed
                txtMaxAvgSpeed.Text = .AvgMaxSpeed
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
                chkUseOnlyWeatherSummary.Checked = .WeatherSummaryOnly
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
                chkActivateEvent.Checked = .EventEnabled
                cboGroupOrClubName.Text = .GroupClubId
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

            _sessionModified = False

        End If

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
                    SetDiscordTaskThreadHeight()
            End Select
        End If

    End Sub

#End Region

End Class



