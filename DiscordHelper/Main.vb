Imports System.Xml
Imports System.IO
Imports System.Xml.Serialization
Imports System.Text
Imports System.Threading
Imports System.Web.UI.WebControls
Imports System.Globalization
Imports SIGLR.SoaringTools.CommonLibrary
Imports System.Text.RegularExpressions

Public Class Main

#Region "Members, Constants and Enums"

    Private Const DiscordLimit As Integer = 2000
    Private Const LimitMsg As String = "Caution! Discord Characters Limit: "
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

#End Region

#Region "Startup"

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        _SF.FillCountryFlagList(cboCountryFlag.Items)

        ResetForm()

        SetTimePickerFormat()

        'Adjust some button position
        btnCopyAllSecPosts.Top = btnAltRestricCopy.Top

        'Adjust form based on screen size available
        If Screen.PrimaryScreen.Bounds.Height > Me.Height Then
        Else
            Me.Height = Screen.PrimaryScreen.Bounds.Height - 20
        End If

        If My.Application.CommandLineArgs.Count > 0 Then
            ' Open the file passed as an argument
            _CurrentSessionFile = My.Application.CommandLineArgs(0)
            'Check if the selected file is a dph or dphx files
            If Path.GetExtension(_CurrentSessionFile) = ".dphx" Then
                'Package - we need to unpack it first
                OpenFileDialog1.FileName = _SF.UnpackDPHXFile(_CurrentSessionFile)

                If OpenFileDialog1.FileName = String.Empty Then
                    _CurrentSessionFile = $"{Application.StartupPath}\LastSession.dph"
                Else
                    _CurrentSessionFile = OpenFileDialog1.FileName
                End If
            End If
        Else
            'Load previous session data
            _CurrentSessionFile = $"{Application.StartupPath}\LastSession.dph"
        End If
        Me.Refresh()
        LoadSessionData(_CurrentSessionFile)
        CheckForNewVersion()

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

        BriefingControl1.FullReset()

        _XmlDocFlightPlan = New XmlDocument
        _XmlDocWeatherPreset = New XmlDocument
        _WeatherDetails = Nothing
        _FlightTotalDistanceInKm = 0
        _TaskTotalDistanceInKm = 0

        cboSpeedUnits.SelectedIndex = 0
        cboDifficulty.SelectedIndex = 0
        cboRecommendedGliders.SelectedIndex = 0
        lstAllFiles.Items.Clear()
        lstAllCountries.Items.Clear()

        Dim sb As New StringBuilder
        sb.AppendLine("**Files**")
        sb.AppendLine("Required: Flight plan (.pln) and Weather preset (.wpr)")
        sb.AppendLine("Optional: XCSoar Track (.trk)")
        txtFilesText.Text = sb.ToString.Trim
        sb.Clear()

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
        txtCredits.Text = "All credits to @UserName for this task."
        txtLongDescription.Text = String.Empty
        chkAddWPCoords.Checked = False
        chkLockCountries.Checked = False
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
        cboBriefingMap.Items.Clear()

        btnRemoveExtraFile.Enabled = False
        btnExtraFileDown.Enabled = False
        btnExtraFileUp.Enabled = False

        _SF.PopulateSoaringClubList(cboGroupOrClubName.Items)

        SetVisibilityForSecPosts()
        BuildFPResults()
        BuildGroupFlightPost()
        SetFormCaption(String.Empty)

    End Sub

    Public Sub CheckForNewVersion()
        Dim myVersionInfo As VersionInfo = _SF.GetVersionInfo()
        Dim message As String = String.Empty

        If _SF.FormatVersionNumber(myVersionInfo.CurrentLatestVersion) > _SF.FormatVersionNumber(Me.GetType.Assembly.GetName.Version.ToString) Then
            'New version available
            If _SF.ShowVersionForm(myVersionInfo, Me.GetType.Assembly.GetName.Version.ToString) = DialogResult.Yes Then
                'update
                'Download the file
                If _SF.DownloadLatestUpdate(myVersionInfo.CurrentLatestVersion, message) Then
                    Application.Exit()
                Else
                    'Show error updating
                    MessageBox.Show(Me, $"An error occured during the update process at this step:{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}The update did not complete.", "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End If

    End Sub

#End Region

#Region "Global form"

#Region "Event handlers"

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        SaveSessionData($"{Application.StartupPath}\LastSession.dph")

    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        ResetForm()
    End Sub

    Private Sub EnterTextBox(sender As Object, e As EventArgs) Handles txtWeatherWinds.Enter, txtWeatherSummary.Enter, txtWeatherFirstPart.Enter, txtWeatherClouds.Enter, txtTitle.Enter, txtTaskFlightPlanURL.Enter, txtSoaringTypeExtraInfo.Enter, txtSimDateTimeExtraInfo.Enter, txtShortDescription.Enter, txtMinAvgSpeed.Enter, txtMaxAvgSpeed.Enter, txtMainArea.Enter, txtLongDescription.Enter, txtGroupFlightEventPost.Enter, txtGroupEventPostURL.Enter, txtFullDescriptionResults.Enter, txtFPResults.Enter, txtFilesText.Enter, txtEventTitle.Enter, txtEventDescription.Enter, txtDurationMin.Enter, txtDurationMax.Enter, txtDurationExtraInfo.Enter, txtDiscordEventTopic.Enter, txtDiscordEventDescription.Enter, txtDifficultyExtraInfo.Enter, txtDepName.Enter, txtDepExtraInfo.Enter, txtDepartureICAO.Enter, txtCredits.Enter, txtArrivalName.Enter, txtArrivalICAO.Enter, txtArrivalExtraInfo.Enter, txtAltRestrictions.Enter
        EnteringTextBox(sender)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        SetBriefingControlsVisiblity()
        If TabControl1.SelectedTab.Name = "tabBriefing" Then
            GenerateBriefing()
        End If
    End Sub

#End Region

#Region "Global form subs & functions"

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

    Private Sub EnteringTextBox(txtbox As Windows.Forms.TextBox)
        txtbox.SelectAll()
        txtbox.SelectionStart = 0
    End Sub

#End Region


#End Region

#Region "Flight Plan Tab"

#Region "Event Handlers"

    Private Sub GeneralFPTabFieldChangeDetection(sender As Object, e As EventArgs) Handles txtTitle.Leave, txtSoaringTypeExtraInfo.Leave, txtSimDateTimeExtraInfo.Leave, txtShortDescription.Leave, txtMainArea.Leave, txtDurationMin.Leave, txtDurationMax.Leave, txtDurationExtraInfo.Leave, txtDifficultyExtraInfo.Leave, txtDepName.Leave, txtDepExtraInfo.Leave, txtDepartureICAO.Leave, txtCredits.Leave, txtArrivalName.Leave, txtArrivalICAO.Leave, txtArrivalExtraInfo.Leave, dtSimLocalTime.ValueChanged, dtSimLocalTime.Leave, dtSimDate.ValueChanged, dtSimDate.Leave, chkSoaringTypeThermal.CheckedChanged, chkSoaringTypeRidge.CheckedChanged, chkIncludeYear.CheckedChanged, cboRecommendedGliders.SelectedIndexChanged, cboRecommendedGliders.Leave, cboDifficulty.SelectedIndexChanged, chkAddWPCoords.CheckedChanged

        BuildFPResults()

        'Some fields need to be copied to the Event tab
        If sender Is dtSimDate Or sender Is dtSimLocalTime Or sender Is chkIncludeYear Then
            CopyToEventFields(sender, e)
        End If

        'For text box, make sure to display the value from the start
        If TypeOf sender Is Windows.Forms.TextBox Then
            LeavingTextBox(sender)
        End If

    End Sub

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
        CalculateDuration()
    End Sub

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

    Private Sub txtLongDescription_Leave(sender As Object, e As EventArgs) Handles txtLongDescription.Leave
        BuildFPResults()
        LeavingTextBox(sender)
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

    Private Sub WeatherFieldChangeDetection(sender As Object, e As EventArgs) Handles txtWeatherSummary.Leave, chkUseOnlyWeatherSummary.CheckedChanged
        BuildWeatherInfoResults()
        If Not (chkUseOnlyWeatherSummary.Checked Or _WeatherDetails Is Nothing) Then
            BuildWeatherCloudLayers()
            BuildWeatherWindLayers()
        Else
            txtWeatherClouds.Text = String.Empty
            txtWeatherWinds.Text = String.Empty
        End If
        If TypeOf (sender) Is TextBox Then
            LeavingTextBox(sender)
        End If
    End Sub

    Private Sub CopyToEventFields(sender As Object, e As EventArgs) Handles txtTitle.TextChanged, txtSimDateTimeExtraInfo.TextChanged, txtShortDescription.TextChanged, txtDurationMin.TextChanged, txtDurationMax.TextChanged, txtDurationExtraInfo.TextChanged, txtCredits.TextChanged

        If sender Is txtTitle Then
            txtEventTitle.Text = txtTitle.Text
        End If

        If sender Is txtShortDescription Then
            txtEventDescription.Text = txtShortDescription.Text
        End If

        BuildGroupFlightPost()

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
            lblTrackDistanceAndMiles.Text = "km / " & FormatNumber(Conversions.KmToMiles(Decimal.Parse(txtDistanceTrack.Text)), 0) & " mi Task"
        End If

        CheckAndSetEventAward()

    End Sub

#Region "Clipboard buttons on the Flight Plan Tab"
    Private Sub btnFPMainInfoCopy_Click(sender As Object, e As EventArgs) Handles btnFPMainInfoCopy.Click
        Clipboard.SetText(txtFPResults.Text)
        MessageBox.Show(Me, "You can now post the main flight plan message directly in the tasks/plans channel, then create a thread (make sure the name is the same as the title) where we will put the other informations.", "Step 1 - Creating main FP post", vbOKOnly, MessageBoxIcon.Information)
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If
    End Sub

    Private Sub btnAltRestricCopy_Click(sender As Object, e As EventArgs) Handles btnAltRestricCopy.Click
        Clipboard.SetText(txtAltRestrictions.Text & vbCrLf & vbCrLf & txtWeatherFirstPart.Text & vbCrLf & vbCrLf & txtWeatherWinds.Text & vbCrLf & vbCrLf & txtWeatherClouds.Text & vbCrLf & ".")
        MessageBox.Show(Me, "Now paste the content as the second message in the thread!", "Step 2 - Creating secondary post for weather in the thread.", vbOKOnly, MessageBoxIcon.Information)
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
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
                MessageBox.Show(Me, "Now paste the copied files as the final message.", "Step 3 - Inserting the files in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

            Else
                MessageBox.Show(Me, "Now paste the copied files as the third message without posting it and come back for the text info (button 3b).", "Step 3a - Creating the files post in the thread - actual files first", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            If _GuideCurrentStep <> 0 Then
                _GuideCurrentStep += 1
                ShowGuide()
            End If
        Else
            MessageBox.Show(Me, "No files to copy!", "Step 3a - Creating the files post in the thread - actual files first", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End If

    End Sub

    Private Sub btnFilesTextCopy_Click(sender As Object, e As EventArgs) Handles btnFilesTextCopy.Click
        Clipboard.SetText(txtFilesText.Text)
        MessageBox.Show(Me, "Now enter the info (legend) in the third message and post it. Also pin this message in the thread.", "Step 3b - Creating the files post in the thread - file info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If
    End Sub

    Private Sub btnFullDescriptionCopy_Click(sender As Object, e As EventArgs) Handles btnFullDescriptionCopy.Click

        If txtFullDescriptionResults.Text.Length = 0 Then
            MessageBox.Show(Me, "The last message (Full Description) is empty. Cannot proceed!", "Step 4 - Creating full description post in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Clipboard.SetText(txtFullDescriptionResults.Text)
            MessageBox.Show(Me, "Now post the last message in the thread to complete your flight plan entry.", "Step 4 - Creating full description post in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Information)
            If _GuideCurrentStep <> 0 Then
                _GuideCurrentStep += 1
                ShowGuide()
            End If
        End If

    End Sub

    Private Sub chkGroupSecondaryPosts_CheckedChanged(sender As Object, e As EventArgs) Handles chkGroupSecondaryPosts.CheckedChanged

        SetVisibilityForSecPosts()

    End Sub

    Private Sub btnCopyAllSecPosts_Click(sender As Object, e As EventArgs) Handles btnCopyAllSecPosts.Click

        Clipboard.SetText(txtAltRestrictions.Text & vbCrLf & vbCrLf &
                          txtWeatherFirstPart.Text & vbCrLf & vbCrLf &
                          txtWeatherWinds.Text & vbCrLf & vbCrLf &
                          txtWeatherClouds.Text & vbCrLf & vbCrLf &
                          txtFullDescriptionResults.Text & vbCrLf & vbCrLf &
                          txtFilesText.Text & vbCrLf)
        MessageBox.Show(Me, "Now paste the content as the second message in the thread!", "Step 2 - Creating secondary post in the thread.", MessageBoxButtons.OK, MessageBoxIcon.Information)
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

#End Region

#Region "Extra files Controls events"
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
                        MessageBox.Show(Me, "Discord does not allow more than 10 files!", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                    If _SF.ExtraFileExtensionIsValid(OpenFileDialog1.FileNames(i)) Then
                        If Not lstAllFiles.Items.Contains(OpenFileDialog1.FileNames(i)) Then
                            lstAllFiles.Items.Add(OpenFileDialog1.FileNames(i))
                        Else
                            MessageBox.Show(Me, "This file already exists in the list.", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        MessageBox.Show(Me, "This file type cannot be added as it may be unsafe.", "Error adding extra file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
            Next
        End If

        LoadPossibleImagesInMapDropdown()

    End Sub

    Private Sub btnRemoveExtraFile_Click(sender As Object, e As EventArgs) Handles btnRemoveExtraFile.Click

        For i As Integer = lstAllFiles.SelectedIndices.Count - 1 To 0 Step -1
            lstAllFiles.Items.RemoveAt(lstAllFiles.SelectedIndices(i))
        Next

        LoadPossibleImagesInMapDropdown()

    End Sub

    Private Sub btnExtraFileUp_Click(sender As Object, e As EventArgs) Handles btnExtraFileUp.Click

        MoveExtraFilesSelectedItems(-1, lstAllFiles)
        btnExtraFileUp.Focus()

    End Sub

    Private Sub btnExtraFileDown_Click(sender As Object, e As EventArgs) Handles btnExtraFileDown.Click

        MoveExtraFilesSelectedItems(1, lstAllFiles)
        btnExtraFileDown.Focus()

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

#End Region

#Region "Country controls events"

    Private Sub btnAddCountry_Click(sender As Object, e As EventArgs) Handles btnAddCountry.Click
        If cboCountryFlag.SelectedIndex > 0 AndAlso Not lstAllCountries.Items.Contains(cboCountryFlag.Text) Then
            lstAllCountries.Items.Add(cboCountryFlag.Text)
            BuildFPResults()
        End If
    End Sub

    Private Sub btnRemoveCountry_Click(sender As Object, e As EventArgs) Handles btnRemoveCountry.Click
        For i As Integer = lstAllCountries.SelectedIndices.Count - 1 To 0 Step -1
            lstAllCountries.Items.RemoveAt(lstAllCountries.SelectedIndices(i))
        Next
        BuildFPResults()
    End Sub

    Private Sub btnMoveCountryUp_Click(sender As Object, e As EventArgs) Handles btnMoveCountryUp.Click
        MoveExtraFilesSelectedItems(-1, lstAllCountries)
        BuildFPResults()
    End Sub

    Private Sub btnMoveCountryDown_Click(sender As Object, e As EventArgs) Handles btnMoveCountryDown.Click
        MoveExtraFilesSelectedItems(1, lstAllCountries)
        BuildFPResults()
    End Sub

#End Region

#End Region

#Region "Flight Plan tab Subs & Functions"

    Private Sub BuildFPResults()

        Dim sb As New StringBuilder()

        Dim dateFormat As String
        If chkIncludeYear.Checked Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        sb.AppendLine($"**{txtTitle.Text}**{AddFlagsToTitle()}")
        sb.AppendLine()
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtShortDescription.Text,,, 2))
        If txtMainArea.Text.Trim.Length > 0 Then
            sb.AppendLine("**Main area/POI:** " & _SF.ValueToAppendIfNotEmpty(txtMainArea.Text))
        End If
        sb.AppendLine($"**Flight plan file:** ""{Path.GetFileName(txtFlightPlanFile.Text)}""")
        sb.AppendLine($"**Departure:** {_SF.ValueToAppendIfNotEmpty(txtDepartureICAO.Text)}{_SF.ValueToAppendIfNotEmpty(txtDepName.Text, True)}{_SF.ValueToAppendIfNotEmpty(txtDepExtraInfo.Text, True, True)}")
        sb.AppendLine($"**Arrival:** {_SF.ValueToAppendIfNotEmpty(txtArrivalICAO.Text)}{_SF.ValueToAppendIfNotEmpty(txtArrivalName.Text, True)}{_SF.ValueToAppendIfNotEmpty(txtArrivalExtraInfo.Text, True, True)}")
        sb.AppendLine($"**Sim Date & Time:** {dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local{_SF.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text.Trim, True, True)}")
        sb.AppendLine($"**Soaring Type:** {GetSoaringTypesSelected()}{_SF.ValueToAppendIfNotEmpty(txtSoaringTypeExtraInfo.Text, True, True)}")
        sb.AppendLine($"**Distance:** {_SF.GetDistance(txtDistanceTotal.Text, txtDistanceTrack.Text)}")
        sb.AppendLine($"**Duration:** {_SF.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{_SF.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}")
        sb.AppendLine($"**Recommended gliders:** {_SF.ValueToAppendIfNotEmpty(cboRecommendedGliders.Text)}")
        sb.AppendLine($"**Difficulty:** {_SF.GetDifficulty(cboDifficulty.SelectedIndex, txtDifficultyExtraInfo.Text)}")
        sb.AppendLine()
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtCredits.Text,,, 2))
        sb.Append("See inside thread for most up-to-date files And more information.")
        txtFPResults.Text = sb.ToString.Trim

        If txtLongDescription.Text.Trim.Length > 0 Then
            txtFullDescriptionResults.Text = $"**Full Description**{Environment.NewLine}{txtLongDescription.Text.Trim}"
        Else
            txtFullDescriptionResults.Text = String.Empty
        End If

        If chkAddWPCoords.Checked Then
            'Add waypoints information to the description
            If txtLongDescription.Text.Trim.Length > 0 Then
                txtFullDescriptionResults.AppendText($"{Environment.NewLine}{Environment.NewLine}")
            End If
            txtFullDescriptionResults.AppendText(_SF.GetAllWPCoordinates())
        End If

    End Sub

    Private Function AddFlagsToTitle() As String
        Dim answer As New StringBuilder

        If lstAllCountries.Items.Count > 0 Then
            For Each country As String In lstAllCountries.Items
                answer.Append($" {_SF.CountryFlagCodes(country)}")
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

        BuildFPResults()

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

    Private Sub LoadFlightPlan(filename As String)

        'read file
        txtFlightPlanFile.Text = filename
        _XmlDocFlightPlan.Load(filename)

        If Not chkTitleLock.Checked Then
            txtTitle.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Title").Item(0).FirstChild.Value
        End If

        If _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Count > 0 AndAlso (Not chkDepartureLock.Checked) Then
            txtDepExtraInfo.Text = $"Rwy {_XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DeparturePosition").Item(0).FirstChild.Value}"
        End If

        If Not chkDescriptionLock.Checked Then
            txtShortDescription.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/Descr").Item(0).FirstChild.Value
        End If

        txtDepartureICAO.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DepartureID").Item(0).FirstChild.Value
        If Not chkDepartureLock.Checked Then
            txtDepName.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DepartureName").Item(0).FirstChild.Value
        End If
        txtArrivalICAO.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DestinationID").Item(0).FirstChild.Value
        If Not chkArrivalLock.Checked Then
            txtArrivalName.Text = _XmlDocFlightPlan.DocumentElement.SelectNodes("FlightPlan.FlightPlan/DestinationName").Item(0).FirstChild.Value
        End If

        txtAltRestrictions.Text = _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, _FlightTotalDistanceInKm, _TaskTotalDistanceInKm)
        txtDistanceTotal.Text = FormatNumber(_FlightTotalDistanceInKm, 0)
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

        BuildFPResults()
        BuildGroupFlightPost()

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

    Private Sub LoadWeatherfile(filename As String)
        'read file
        txtWeatherFile.Text = filename
        _XmlDocWeatherPreset.Load(filename)

        _WeatherDetails = Nothing
        _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        BuildWeatherInfoResults()
        BuildGroupFlightPost()

        If Not (chkUseOnlyWeatherSummary.Checked Or _WeatherDetails Is Nothing) Then
            BuildWeatherCloudLayers()
            BuildWeatherWindLayers()
        End If

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

        sb.AppendLine("**Weather Basic Information**")

        If chkUseOnlyWeatherSummary.Checked Or _WeatherDetails Is Nothing Then
            sb.Append($"Summary: {_SF.ValueToAppendIfNotEmpty(txtWeatherSummary.Text, nbrLineFeed:=1)}")
        Else
            sb.Append($"Weather file & profile name: ""{Path.GetFileName(txtWeatherFile.Text)}"" ({_WeatherDetails.PresetName}){Environment.NewLine}")
            If Not txtWeatherSummary.Text.Trim = String.Empty Then
                sb.Append($"Summary: {_SF.ValueToAppendIfNotEmpty(txtWeatherSummary.Text)}{Environment.NewLine}")
            End If
            sb.Append($"Elevation measurement: {_WeatherDetails.AltitudeMeasurement}{Environment.NewLine}")
            sb.Append($"MSLPressure: {_WeatherDetails.MSLPressure}{Environment.NewLine}")
            sb.Append($"MSLTemperature: {_WeatherDetails.MSLTemperature}{Environment.NewLine}")
            sb.Append($"Humidity: {_WeatherDetails.Humidity}")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"{Environment.NewLine}Precipitations: {_WeatherDetails.Precipitations}")
            End If
            If _WeatherDetails.HasSnowCover Then
                sb.Append($"{Environment.NewLine}Snow Cover: {_WeatherDetails.SnowCover}")
            End If
        End If

        txtWeatherFirstPart.Text = sb.ToString.TrimEnd

    End Sub

    Private Sub BuildWeatherCloudLayers()
        txtWeatherClouds.Text = $"**Cloud Layers**{Environment.NewLine}"
        txtWeatherClouds.AppendText(_WeatherDetails.CloudLayers)
    End Sub

    Private Sub BuildWeatherWindLayers()
        txtWeatherWinds.Text = $"**Wind Layers**{Environment.NewLine}"
        txtWeatherWinds.AppendText(_WeatherDetails.WindLayers)
    End Sub

#End Region

#End Region

#End Region

#Region "Group Flights/Events Tab"

#Region "Event Handlers"

    Private Sub ClubSelected(sender As Object, e As EventArgs) Handles cboGroupOrClubName.SelectedIndexChanged, cboGroupOrClubName.Leave

        Dim clubExists As Boolean = _SF.DefaultKnownClubEvents.ContainsKey(cboGroupOrClubName.Text.ToUpper)

        If clubExists Then
            _ClubPreset = _SF.DefaultKnownClubEvents(cboGroupOrClubName.Text.ToUpper)
            cboGroupOrClubName.Text = _ClubPreset.ClubId
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
        Else
            _ClubPreset = Nothing
        End If

        BuildGroupFlightPost()

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

    End Sub

    Private Sub chkDateTimeUTC_CheckedChanged(sender As Object, e As EventArgs) Handles chkDateTimeUTC.CheckedChanged
        BuildEventDatesTimes()
    End Sub

    Private Sub GroupFlightFieldLeave(sender As Object, e As EventArgs) Handles chkUseSyncFly.CheckedChanged, chkUseStart.CheckedChanged, chkUseLaunch.CheckedChanged, cboVoiceChannel.SelectedIndexChanged, cboVoiceChannel.Leave, cboMSFSServer.SelectedIndexChanged, cboMSFSServer.Leave, cboEligibleAward.SelectedIndexChanged, cboEligibleAward.Leave
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
        MessageBox.Show(Me, $"You can now post the group flight event in the proper Discord channel for the club/group.{Environment.NewLine}Then copy the link to that newly created message.{Environment.NewLine}Finally, paste the link in the URL field on section 2 for Discord Event.",
                        "Creating group flight post",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

    Private Sub btnEventTopicClipboard_Click(sender As Object, e As EventArgs) Handles btnEventTopicClipboard.Click
        If txtDiscordEventTopic.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventTopic.Text)
            MessageBox.Show(Me, "Paste the topic into the Event Topic field on Discord.", "Creating Discord Event", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

    Private Sub btnEventDescriptionToClipboard_Click(sender As Object, e As EventArgs) Handles btnEventDescriptionToClipboard.Click
        If txtDiscordEventDescription.Text <> String.Empty Then
            Clipboard.SetText(txtDiscordEventDescription.Text)
            MessageBox.Show(Me, "Paste the description into the Event Description field on Discord.", "Creating Discord Event", MessageBoxButtons.OK, MessageBoxIcon.Information)

        End If
        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
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
            MessageBox.Show(Me,
                            "Now paste the copied files in a new post in the proper Discord channel for the club/group and come back for the text info (button 1 below).",
                            "Optional - Including the required files in the group flight post",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
        End If

        If _GuideCurrentStep <> 0 Then
            _GuideCurrentStep += 1
            ShowGuide()
        End If

    End Sub

    Private Sub EventTabTextControlLeave(sender As Object, e As EventArgs) Handles txtTaskFlightPlanURL.Leave, txtGroupFlightEventPost.Leave, txtGroupEventPostURL.Leave, txtEventTitle.Leave, txtEventDescription.Leave, txtDiscordEventTopic.Leave, txtDiscordEventDescription.Leave
        LeavingTextBox(sender)
        BuildGroupFlightPost()
        BuildDiscordEventDescription()
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
        ToolTip1.SetToolTip(lblMeetTimeResult, eventDay.ToString)

        'Check if local DST applies for this date
        lblLocalDSTWarning.Visible = _SF.DSTAppliesForLocalDate(theDate)

        lblSyncTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventSyncFlyDate.Value.Year, dtEventSyncFlyDate.Value.Month, dtEventSyncFlyDate.Value.Day, dtEventSyncFlyTime.Value.Hour, dtEventSyncFlyTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblSyncTimeResult, eventDay.ToString)

        lblLaunchTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventLaunchDate.Value.Year, dtEventLaunchDate.Value.Month, dtEventLaunchDate.Value.Day, dtEventLaunchTime.Value.Hour, dtEventLaunchTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblLaunchTimeResult, eventDay.ToString)

        lblStartTimeResult.Text = _SF.FormatEventDateTime(New Date(dtEventStartTaskDate.Value.Year, dtEventStartTaskDate.Value.Month, dtEventStartTaskDate.Value.Day, dtEventStartTaskTime.Value.Hour, dtEventStartTaskTime.Value.Minute, 0), eventDay, chkDateTimeUTC.Checked)
        ToolTip1.SetToolTip(lblStartTimeResult, eventDay.ToString)

        BuildGroupFlightPost()

    End Sub

    Private Sub BuildGroupFlightPost()

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

        Dim sb As New StringBuilder

        lblDiscordPostDateTime.Text = $"{fullMeetDateTimeLocal:dddd, MMMM dd}, {fullMeetDateTimeLocal:hh:mm tt}"
        lblDiscordEventVoice.Text = cboVoiceChannel.Text

        txtGroupFlightEventPost.Text = String.Empty

        sb.AppendLine($"**{Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("dddd, MMMM dd", _EnglishCulture)}, {Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time**")
        sb.AppendLine()

        If txtEventTitle.Text <> String.Empty Then
            If cboGroupOrClubName.SelectedIndex > -1 Then
                sb.Append($"{_ClubPreset.ClubName} - ")
            End If
            sb.AppendLine(txtEventTitle.Text & AddFlagsToTitle())
            sb.AppendLine()
        End If
        sb.Append(_SF.ValueToAppendIfNotEmpty(txtEventDescription.Text,,, 2))
        sb.AppendLine($"**Server:** {cboMSFSServer.Text}")
        sb.AppendLine($"**Voice:** {cboVoiceChannel.Text}")
        sb.AppendLine()

        sb.AppendLine($"**Meet/Briefing:** {Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("dddd, MMMM dd", _EnglishCulture)}, {Conversions.ConvertLocalToUTC(fullMeetDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullMeetDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.FullDateTimeWithDayOfWeek)} your local time{Environment.NewLine}At this time we meet in the voice chat and get ready.")

        If Not txtTaskFlightPlanURL.Text = String.Empty Then
            sb.AppendLine()
            sb.AppendLine($"**Flight Plan Details, Weather and files**{Environment.NewLine}{txtTaskFlightPlanURL.Text}")
            sb.AppendLine()
            If chkIncludeGotGravelInvite.Checked AndAlso chkIncludeGotGravelInvite.Enabled Then
                sb.AppendLine("If you did not join Got Gravel already, you will need this invite link first: https://discord.gg/BqUcbvDP69")
                sb.AppendLine()
            End If
        Else
            sb.AppendLine()
        End If
        sb.AppendLine($"**Sim date And time:** {dtSimDate.Value.ToString(dateFormat, _EnglishCulture)}, {dtSimLocalTime.Value.ToString("hh:mm tt", _EnglishCulture)} local{_SF.ValueToAppendIfNotEmpty(txtSimDateTimeExtraInfo.Text, True, True)}")

        If Not txtFlightPlanFile.Text = String.Empty Then
            sb.AppendLine($"**Flight plan file:** ""{Path.GetFileName(txtFlightPlanFile.Text)}""")
        End If
        If txtWeatherFile.Text <> String.Empty AndAlso (_WeatherDetails IsNot Nothing) Then
            sb.AppendLine("**Weather file & profile name:** """ & Path.GetFileName(txtWeatherFile.Text) & """ (" & _WeatherDetails.PresetName & ")")
        End If
        sb.AppendLine()

        If chkUseSyncFly.Checked Then
            sb.AppendLine($"**Synchronized Fly:** {Conversions.ConvertLocalToUTC(fullSyncFlyDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullSyncFlyDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)} your local time{Environment.NewLine}At this time we simultaneously click fly to sync our weather.")
            If chkUseLaunch.Checked AndAlso fullSyncFlyDateTimeLocal = fullLaunchDateTimeLocal Then
                sb.AppendLine("At this time we can also start launching from the airfield.")
                sb.AppendLine()
            Else
                sb.AppendLine()
            End If
        End If

        If chkUseLaunch.Checked AndAlso (fullSyncFlyDateTimeLocal <> fullLaunchDateTimeLocal OrElse Not chkUseSyncFly.Checked) Then
            sb.AppendLine($"**Launch:** {Conversions.ConvertLocalToUTC(fullLaunchDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullLaunchDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)} your local time{Environment.NewLine}At this time we can start launching from the airfield.")
            sb.AppendLine()
        End If

        If chkUseStart.Checked Then
            sb.AppendLine($"**Task Start:** {Conversions.ConvertLocalToUTC(fullStartTaskDateTimeLocal).ToString("hh:mm tt", _EnglishCulture)} Zulu / {_SF.GetDiscordTimeStampForDate(fullStartTaskDateTimeLocal, SupportingFeatures.DiscordTimeStampFormat.TimeOnlyWithoutSeconds)} your local time{Environment.NewLine}At this time we cross the starting line and start the task.")
            sb.AppendLine()
        End If

        sb.AppendLine($"**Duration:** {_SF.GetDuration(txtDurationMin.Text, txtDurationMax.Text)}{_SF.ValueToAppendIfNotEmpty(txtDurationExtraInfo.Text, True, True)}")

        If cboEligibleAward.SelectedIndex > 0 Then
            sb.AppendLine()
            sb.AppendLine($"Pilots who finish this task successfully during the event will be eligible to apply for the {cboEligibleAward.Text} Soaring Badge :{cboEligibleAward.Text.ToLower()}:")
        End If

        If txtCredits.Text <> String.Empty Then
            sb.AppendLine()
            sb.AppendLine(txtCredits.Text)
        End If

        txtGroupFlightEventPost.Text = sb.ToString.Trim

        BuildDiscordEventDescription()

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
    Private Sub tabBriefingControl_SelectedIndexChanged(sender As Object, e As EventArgs)

        SetBriefingControlsVisiblity()

    End Sub

    Private Sub cboBriefingMap_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBriefingMap.SelectedIndexChanged

        'Load image
        BriefingControl1.ChangeImage(cboBriefingMap.SelectedItem.ToString)

    End Sub


#End Region

    Private Sub SetBriefingControlsVisiblity()

        If TabControl1.SelectedTab.Name = "tabBriefing" Then
            cboBriefingMap.Visible = True
        Else
            cboBriefingMap.Visible = False
        End If

    End Sub

    Private Sub LoadPossibleImagesInMapDropdown(Optional mapToSelect As String = "")

        Dim currentImage As String = String.Empty

        'Load up the possible images in the dropdown list
        If mapToSelect = "" Then
            currentImage = cboBriefingMap.Text
        Else
            currentImage = mapToSelect
        End If
        cboBriefingMap.Items.Clear()
        For Each item As String In lstAllFiles.Items
            Dim fileExtension As String = Path.GetExtension(item)
            If fileExtension = ".png" OrElse fileExtension = ".jpg" OrElse fileExtension = ".bmp" Then
                cboBriefingMap.Items.Add(item)
                If item = currentImage Then
                    cboBriefingMap.SelectedItem = item
                End If
            End If
        Next
        If cboBriefingMap.SelectedIndex = -1 Then
        End If

    End Sub
    Private Sub GenerateBriefing()

        LoadPossibleImagesInMapDropdown()

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

    Private Sub btnGuideMe_Click(sender As Object, e As EventArgs) Handles btnGuideMe.Click

        If MessageBox.Show(Me, "Do you want To start by resetting everything?", "Starting the Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ResetForm()
        End If

        TabControl1.SelectedTab = TabControl1.TabPages("tabFlightPlan")
        _GuideCurrentStep = 1
        btnTurnGuideOff.Visible = True
        ShowGuide()

    End Sub

    Private Sub btnGuideNext_Click(sender As Object, e As EventArgs) Handles btnGuideNext.Click, btnEventGuideNext.Click

        _GuideCurrentStep += 1
        ShowGuide()

    End Sub

    Private Sub btnTurnGuideOff_Click(sender As Object, e As EventArgs) Handles btnTurnGuideOff.Click

        _GuideCurrentStep = 0
        btnTurnGuideOff.Visible = False
        ShowGuide()

    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles TabControl1.KeyDown, MyBase.KeyDown
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
        End If

    End Sub

#End Region

    Private Sub ShowGuide(Optional fromF1Key As Boolean = False)

        If _GuideCurrentStep > 0 Then
            btnTurnGuideOff.Visible = True
        End If

        Select Case _GuideCurrentStep
            Case 0
                pnlGuide.Visible = False
                pnlWizardEvent.Visible = False
                btnTurnGuideOff.Visible = False
            Case 1 'Select flight plan
                SetGuidePanelToLeft()
                pnlGuide.Top = -9
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
                lblGuideInstructions.Text = "Optionally, you should provide a more detailed description of the task. Context, history, hints, tips, tricks around waypoints, etc. Also possible to add waypoint coordinates."
                SetFocusOnField(txtLongDescription, fromF1Key)
            Case 17 'Weather summary
                SetGuidePanelToLeft()
                pnlGuide.Top = 942
                lblGuideInstructions.Text = "Optional weather summary. If you don't want the full weather details to be included, tick the checkbox to the left. Only the summary will then be shown."
                SetFocusOnField(txtWeatherSummary, fromF1Key)
            Case 18 'Extra files
                SetGuidePanelToLeft()
                Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.left_arrow
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
                    _GuideCurrentStep += 1
                    ShowGuide()
                End If

            Case 25 'Copy Description
                If Not chkGroupSecondaryPosts.Checked Then
                    SetGuidePanelToRight()
                    pnlGuide.Top = 1027
                    lblGuideInstructions.Text = "One last step, click this button to copy the full description to your clipboard and receive instructions."
                    SetFocusOnField(btnFullDescriptionCopy, fromF1Key)
                Else
                    _GuideCurrentStep += 1
                    ShowGuide()
                End If

            Case 26 'Event
                If MessageBox.Show(Me, "The task's details are all posted. Are you also creating the group flight post on Discord?", "Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    _GuideCurrentStep += 1
                Else
                    _GuideCurrentStep = 999
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
                If MessageBox.Show("Do you have the access rights to create Discord Event on the target Discord Server? Click No if you don't know.", "Discord Post Helper Wizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    _GuideCurrentStep += 1
                Else
                    _GuideCurrentStep = 999
                End If
                ShowGuide()

            Case 43 'Group flight post
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 705
                lblEventGuideInstructions.Text = "From Discord, copy the link to the group flight post you just created above, and click ""Paste"" here."
                SetFocusOnField(btnDiscordGroupEventURL, fromF1Key)

            Case 44 'Create Discord Event
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 750
                lblEventGuideInstructions.Text = "In Discord and in the proper Discord Server, start the creation of a new Event (Create Event). If you don't know how to do this, ask for help!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 45 'Select voice channel for event
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 792
                lblEventGuideInstructions.Text = "On the new event window, under ""Where is your event"", choose ""Voice Channel"" and select this voice channel. Then click ""Next"" on the event window."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 46 'Topic name
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 832
                lblEventGuideInstructions.Text = "Click this button to copy the event topic and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventTopicClipboard, fromF1Key)

            Case 47 'Event date & time
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 875
                lblEventGuideInstructions.Text = "On the Discord event window, specify the date and time displayed here - these are all local times you have to use!"
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 48 'Event description
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 918
                lblEventGuideInstructions.Text = "Click this button to copy the event description and receive instructions to paste it in the Discord event window."
                SetFocusOnField(btnEventDescriptionToClipboard, fromF1Key)

            Case 49 'Cover image
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 958
                lblEventGuideInstructions.Text = "In the Discord event window, you can also upload a cover image for your event. This is optional."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case 50 'Cover image
                SetEventGuidePanelToLeft()
                pnlWizardEvent.Top = 1003
                lblEventGuideInstructions.Text = "In the Discord event window, click Next to review your event information and publish it."
                SetFocusOnField(btnEventGuideNext, fromF1Key)

            Case Else
                _GuideCurrentStep = 0
                pnlGuide.Visible = False
                pnlWizardEvent.Visible = False
                btnTurnGuideOff.Visible = False
                MessageBox.Show(Me, "The wizard's guidance ends here! If you hover your mouse on any field or button, you will also get a tooltip help displayed!", "Discord Post Helper Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Select
    End Sub

    Private Sub SetFocusOnField(controlToPutFocus As Windows.Forms.Control, fromF1Key As Boolean)

        If Not fromF1Key Then
            controlToPutFocus.Focus()
        End If

    End Sub

    Private Sub SetGuidePanelToLeft()
        pnlWizardEvent.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.left_arrow
        pnlGuide.Left = 737
        pnlGuide.Visible = True
        pnlArrow.Left = -6
        pnlArrow.Top = 0
        btnGuideNext.Left = 674
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToTop()
        pnlWizardEvent.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.up_arrow
        pnlGuide.Top = 0
        pnlGuide.Visible = True
        pnlArrow.Left = -6
        pnlArrow.Top = 0
        btnGuideNext.Left = 674
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetGuidePanelToRight()
        pnlWizardEvent.Visible = False
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        pnlGuide.Left = 737
        pnlGuide.Visible = True
        pnlArrow.Left = 667
        pnlArrow.Top = 0
        btnGuideNext.Left = 3
        btnGuideNext.Top = 3
    End Sub

    Private Sub SetEventGuidePanelToLeft()
        pnlGuide.Visible = False
        Me.pnlEventArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.left_arrow
        pnlWizardEvent.Left = 852
        pnlWizardEvent.Visible = True
        pnlEventArrow.Left = -6
        pnlEventArrow.Top = 0
        btnEventGuideNext.Left = 674
        btnEventGuideNext.Top = 3
    End Sub

    Private Sub SetEventGuidePanelToRight()
        pnlGuide.Visible = False
        Me.pnlEventArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        pnlWizardEvent.Left = 740
        pnlWizardEvent.Visible = True
        pnlEventArrow.Left = 667
        pnlEventArrow.Top = 0
        btnEventGuideNext.Left = 3
        btnEventGuideNext.Top = 3
    End Sub


#End Region

#Region "Call to B21 Online Planner"

    Private Sub btnLoadB21Planner_Click(sender As Object, e As EventArgs) Handles btnLoadB21Planner.Click

        If txtFlightPlanFile.Text Is String.Empty Then
            Process.Start(B21PlannerURL)
        Else
            Dim tempFolderName As String = _SF.GenerateRandomFileName()
            Dim flightPlanName As String = Path.GetFileNameWithoutExtension(txtFlightPlanFile.Text)

            _SF.UploadFile(tempFolderName, flightPlanName, _XmlDocFlightPlan.InnerXml)

            Process.Start(B21PlannerURL & "?pln=siglr.com/DiscordPostHelper/FlightPlans/" & tempFolderName & "/" & flightPlanName & ".pln")

            'Wait 5 seconds
            Thread.Sleep(5000)
            _SF.DeleteTempFile(tempFolderName)

            If MessageBox.Show(Me, "After reviewing or editing the flight plan, did you make any modification and would like to reload the flight plan here?", "Coming back from B21 Planner", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                'Reload the flight plan
                LoadFlightPlan(txtFlightPlanFile.Text)
            End If

        End If

    End Sub


#End Region

#Region "Load/Save/Create Package (buttons on top)"

#Region "Event Handlers"

    Private Sub btnLoadConfig_Click(sender As Object, e As EventArgs) Handles btnLoadConfig.Click
        LoadFile()
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
            _CurrentSessionFile = SaveFileDialog1.FileName
        End If

    End Sub

    Private Sub btnCreateShareablePack_Click(sender As Object, e As EventArgs) Handles btnCreateShareablePack.Click

        If _CurrentSessionFile.EndsWith("LastSession.dph") Then
            MessageBox.Show("You first need to save or load your session!", "Package creation error - cannot create from last session!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        SaveFileDialog1.RestoreDirectory = True
        If txtFlightPlanFile.Text = String.Empty Then
            SaveFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
        Else
            SaveFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFlightPlanFile.Text)
        End If
        SaveFileDialog1.FileName = txtTitle.Text
        SaveFileDialog1.Title = "Select package file to save"
        SaveFileDialog1.Filter = "Discord Post Helper package files (*.dphx)|*.dphx"

        Dim result As DialogResult = SaveFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            'Check if file already exists and delete it
            If File.Exists(SaveFileDialog1.FileName) Then
                File.Delete(SaveFileDialog1.FileName)
            End If

            ' Zip the selected files using the ZipFiles method
            Dim filesToInclude As New List(Of String)()
            filesToInclude.Add(_CurrentSessionFile)
            filesToInclude.Add(txtFlightPlanFile.Text)
            filesToInclude.Add(txtWeatherFile.Text)
            For i As Integer = 0 To lstAllFiles.Items.Count - 1
                filesToInclude.Add(lstAllFiles.Items(i))
            Next

            _SF.CreateDPHXFile(SaveFileDialog1.FileName, filesToInclude)

            Dim allFiles As New Specialized.StringCollection
            allFiles.Add(SaveFileDialog1.FileName)
            Clipboard.SetFileDropList(allFiles)
            MessageBox.Show(Me, "The package file (dphx) has been copied to your clipboard.", "Shareable Session Package created", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

#End Region

    Private Sub LoadFile()

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

            Dim validSessionFile As Boolean = True

            'Check if the selected file is a dph or dphx files
            If Path.GetExtension(OpenFileDialog1.FileName) = ".dphx" Then
                'Package - we need to unpack it first
                OpenFileDialog1.FileName = _SF.UnpackDPHXFile(OpenFileDialog1.FileName)

                If OpenFileDialog1.FileName = String.Empty Then
                    validSessionFile = False
                Else
                    validSessionFile = True
                End If
            End If

            If validSessionFile Then
                ResetForm()
                LoadSessionData(OpenFileDialog1.FileName)
                _CurrentSessionFile = OpenFileDialog1.FileName
                GenerateBriefing()
            End If
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
            .AddWPCoordinates = chkAddWPCoords.Checked
            .WeatherSummaryOnly = chkUseOnlyWeatherSummary.Checked
            .WeatherSummary = txtWeatherSummary.Text
            For i As Integer = 0 To lstAllFiles.Items.Count - 1
                .ExtraFiles.Add(lstAllFiles.Items(i))
            Next
            .LockCountries = chkLockCountries.Checked
            For i As Integer = 0 To lstAllCountries.Items.Count - 1
                .Countries.Add(lstAllCountries.Items(i))
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
            .EventDescription = txtEventDescription.Text.Replace(Environment.NewLine, "($*$)")
            .EligibleAward = cboEligibleAward.SelectedIndex
            .URLFlightPlanPost = txtTaskFlightPlanURL.Text
            .URLGroupEventPost = txtGroupEventPostURL.Text
            .IncludeGGServerInvite = chkIncludeGotGravelInvite.Checked
            .MapImageSelected = cboBriefingMap.Text

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
                    .FlightPlanFilename = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.FlightPlanFilename)}"
                End If
                txtFlightPlanFile.Text = .FlightPlanFilename
                Me.Update()
                chkLockCountries.Checked = .LockCountries
                LoadFlightPlan(txtFlightPlanFile.Text)

                If File.Exists(.WeatherFilename) Then
                Else
                    'Should expect the file to be in the same folder as the .dph file
                    .WeatherFilename = $"{Path.GetDirectoryName(filename)}\{Path.GetFileName(.WeatherFilename)}"
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
                txtShortDescription.Text = .ShortDescription.Replace("($*$)", Environment.NewLine)
                txtCredits.Text = .Credits
                txtLongDescription.Text = .LongDescription.Replace("($*$)", Environment.NewLine)
                chkAddWPCoords.Checked = .AddWPCoordinates
                chkUseOnlyWeatherSummary.Checked = .WeatherSummaryOnly
                txtWeatherSummary.Text = .WeatherSummary
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
                txtEventDescription.Text = .EventDescription.Replace("($*$)", Environment.NewLine)
                cboEligibleAward.SelectedIndex = .EligibleAward
                txtTaskFlightPlanURL.Text = .URLFlightPlanPost
                txtGroupEventPostURL.Text = .URLGroupEventPost
                chkIncludeGotGravelInvite.Checked = .IncludeGGServerInvite
                LoadPossibleImagesInMapDropdown(.MapImageSelected)
            End With

            BuildFPResults()
            BuildWeatherInfoResults()
            BuildGroupFlightPost()
            BuildDiscordEventDescription()

        End If

    End Sub

#End Region

End Class



