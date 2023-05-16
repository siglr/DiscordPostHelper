<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.pnlScrollableSurface = New System.Windows.Forms.Panel()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabFlightPlan = New System.Windows.Forms.TabPage()
        Me.pnlGuide = New System.Windows.Forms.Panel()
        Me.btnGuideNext = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlArrow = New System.Windows.Forms.Panel()
        Me.grbTaskInfo = New System.Windows.Forms.GroupBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.chkTitleLock = New System.Windows.Forms.CheckBox()
        Me.chkArrivalLock = New System.Windows.Forms.CheckBox()
        Me.chkDepartureLock = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeThermal = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeRidge = New System.Windows.Forms.CheckBox()
        Me.txtSoaringTypeExtraInfo = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtArrivalExtraInfo = New System.Windows.Forms.TextBox()
        Me.txtArrivalName = New System.Windows.Forms.TextBox()
        Me.txtArrivalICAO = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtDepExtraInfo = New System.Windows.Forms.TextBox()
        Me.txtDepName = New System.Windows.Forms.TextBox()
        Me.txtSimDateTimeExtraInfo = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.dtSimLocalTime = New System.Windows.Forms.DateTimePicker()
        Me.chkIncludeYear = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.dtSimDate = New System.Windows.Forms.DateTimePicker()
        Me.txtDepartureICAO = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtMainArea = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cboSpeedUnits = New System.Windows.Forms.ComboBox()
        Me.txtMinAvgSpeed = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txtMaxAvgSpeed = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.btnSelectWeatherFile = New System.Windows.Forms.Button()
        Me.txtWeatherFile = New System.Windows.Forms.TextBox()
        Me.txtDurationMin = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtDurationMax = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtDurationExtraInfo = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtCredits = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.lblTotalDistanceAndMiles = New System.Windows.Forms.Label()
        Me.lblTrackDistanceAndMiles = New System.Windows.Forms.Label()
        Me.cboDifficulty = New System.Windows.Forms.ComboBox()
        Me.txtDistanceTotal = New System.Windows.Forms.TextBox()
        Me.txtDistanceTrack = New System.Windows.Forms.TextBox()
        Me.cboRecommendedGliders = New System.Windows.Forms.ComboBox()
        Me.txtDifficultyExtraInfo = New System.Windows.Forms.TextBox()
        Me.chkDescriptionLock = New System.Windows.Forms.CheckBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtShortDescription = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.txtLongDescription = New System.Windows.Forms.TextBox()
        Me.txtFlightPlanFile = New System.Windows.Forms.TextBox()
        Me.btnSelectFlightPlan = New System.Windows.Forms.Button()
        Me.grpTaskPart2 = New System.Windows.Forms.GroupBox()
        Me.chkLockCountries = New System.Windows.Forms.CheckBox()
        Me.btnMoveCountryDown = New System.Windows.Forms.Button()
        Me.btnMoveCountryUp = New System.Windows.Forms.Button()
        Me.btnRemoveCountry = New System.Windows.Forms.Button()
        Me.btnAddCountry = New System.Windows.Forms.Button()
        Me.lstAllCountries = New System.Windows.Forms.ListBox()
        Me.cboCountryFlag = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.btnExtraFileDown = New System.Windows.Forms.Button()
        Me.btnExtraFileUp = New System.Windows.Forms.Button()
        Me.btnRemoveExtraFile = New System.Windows.Forms.Button()
        Me.btnAddExtraFile = New System.Windows.Forms.Button()
        Me.lstAllFiles = New System.Windows.Forms.ListBox()
        Me.chkUseOnlyWeatherSummary = New System.Windows.Forms.CheckBox()
        Me.txtWeatherSummary = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.tabEvent = New System.Windows.Forms.TabPage()
        Me.chkActivateEvent = New System.Windows.Forms.CheckBox()
        Me.pnlWizardEvent = New System.Windows.Forms.Panel()
        Me.btnEventGuideNext = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblEventGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlEventArrow = New System.Windows.Forms.Panel()
        Me.grpGroupEventPost = New System.Windows.Forms.GroupBox()
        Me.lblLocalDSTWarning = New System.Windows.Forms.Label()
        Me.chkIncludeGotGravelInvite = New System.Windows.Forms.CheckBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.lblEventTaskDistance = New System.Windows.Forms.Label()
        Me.btnTaskFPURLPaste = New System.Windows.Forms.Button()
        Me.cboGroupOrClubName = New System.Windows.Forms.ComboBox()
        Me.txtEventTitle = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.txtTaskFlightPlanURL = New System.Windows.Forms.TextBox()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.cboEligibleAward = New System.Windows.Forms.ComboBox()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.cboVoiceChannel = New System.Windows.Forms.ComboBox()
        Me.cboMSFSServer = New System.Windows.Forms.ComboBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.txtEventDescription = New System.Windows.Forms.TextBox()
        Me.lblStartTimeResult = New System.Windows.Forms.Label()
        Me.lblLaunchTimeResult = New System.Windows.Forms.Label()
        Me.lblSyncTimeResult = New System.Windows.Forms.Label()
        Me.lblMeetTimeResult = New System.Windows.Forms.Label()
        Me.chkUseStart = New System.Windows.Forms.CheckBox()
        Me.dtEventStartTaskTime = New System.Windows.Forms.DateTimePicker()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.dtEventStartTaskDate = New System.Windows.Forms.DateTimePicker()
        Me.chkUseLaunch = New System.Windows.Forms.CheckBox()
        Me.dtEventLaunchTime = New System.Windows.Forms.DateTimePicker()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.dtEventLaunchDate = New System.Windows.Forms.DateTimePicker()
        Me.chkUseSyncFly = New System.Windows.Forms.CheckBox()
        Me.dtEventSyncFlyTime = New System.Windows.Forms.DateTimePicker()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.dtEventSyncFlyDate = New System.Windows.Forms.DateTimePicker()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.chkDateTimeUTC = New System.Windows.Forms.CheckBox()
        Me.dtEventMeetTime = New System.Windows.Forms.DateTimePicker()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.dtEventMeetDate = New System.Windows.Forms.DateTimePicker()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.tabBriefing = New System.Windows.Forms.TabPage()
        Me.pnlBriefing = New System.Windows.Forms.Panel()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.cboBriefingMap = New System.Windows.Forms.ComboBox()
        Me.tabDiscord = New System.Windows.Forms.TabPage()
        Me.txtAddOnsDetails = New System.Windows.Forms.TextBox()
        Me.txtWaypointsDetails = New System.Windows.Forms.TextBox()
        Me.lblNbrCarsRestrictions = New System.Windows.Forms.Label()
        Me.txtGroupFlightEventPost = New System.Windows.Forms.TextBox()
        Me.grpDiscordTask = New System.Windows.Forms.GroupBox()
        Me.grpDiscordTaskThread = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnFilesCopy = New System.Windows.Forms.Button()
        Me.btnFilesTextCopy = New System.Windows.Forms.Button()
        Me.chkGroupSecondaryPosts = New System.Windows.Forms.CheckBox()
        Me.btnCopyAllSecPosts = New System.Windows.Forms.Button()
        Me.lblAllSecPostsTotalCars = New System.Windows.Forms.Label()
        Me.btnAltRestricCopy = New System.Windows.Forms.Button()
        Me.lblRestrictWeatherTotalCars = New System.Windows.Forms.Label()
        Me.btnFullDescriptionCopy = New System.Windows.Forms.Button()
        Me.lblNbrCarsFullDescResults = New System.Windows.Forms.Label()
        Me.btnWaypointsCopy = New System.Windows.Forms.Button()
        Me.lblWaypointsTotalCars = New System.Windows.Forms.Label()
        Me.btnAddOnsCopy = New System.Windows.Forms.Button()
        Me.lblAddOnsTotalCars = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnFPMainInfoCopy = New System.Windows.Forms.Button()
        Me.lblNbrCarsMainFP = New System.Windows.Forms.Label()
        Me.txtDiscordEventDescription = New System.Windows.Forms.TextBox()
        Me.txtDiscordEventTopic = New System.Windows.Forms.TextBox()
        Me.lblNbrCarsWeatherClouds = New System.Windows.Forms.Label()
        Me.txtFullDescriptionResults = New System.Windows.Forms.TextBox()
        Me.txtWeatherFirstPart = New System.Windows.Forms.TextBox()
        Me.txtFilesText = New System.Windows.Forms.TextBox()
        Me.txtWeatherWinds = New System.Windows.Forms.TextBox()
        Me.txtFPResults = New System.Windows.Forms.TextBox()
        Me.txtWeatherClouds = New System.Windows.Forms.TextBox()
        Me.lblNbrCarsWeatherInfo = New System.Windows.Forms.Label()
        Me.txtAltRestrictions = New System.Windows.Forms.TextBox()
        Me.lblNbrCarsWeatherWinds = New System.Windows.Forms.Label()
        Me.lblNbrCarsFilesText = New System.Windows.Forms.Label()
        Me.grpDiscordGroupFlight = New System.Windows.Forms.GroupBox()
        Me.grpGroupFlightEvent = New System.Windows.Forms.GroupBox()
        Me.btnCopyReqFilesToClipboard = New System.Windows.Forms.Button()
        Me.btnGroupFlightEventInfoToClipboard = New System.Windows.Forms.Button()
        Me.grpDiscordEvent = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnDiscordGroupEventURL = New System.Windows.Forms.Button()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.btnEventDescriptionToClipboard = New System.Windows.Forms.Button()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.lblDiscordPostDateTime = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.btnEventTopicClipboard = New System.Windows.Forms.Button()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.lblDiscordEventVoice = New System.Windows.Forms.Label()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.txtGroupEventPostURL = New System.Windows.Forms.TextBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnReset = New System.Windows.Forms.Button()
        Me.btnLoadConfig = New System.Windows.Forms.Button()
        Me.btnSaveConfig = New System.Windows.Forms.Button()
        Me.btnCreateShareablePack = New System.Windows.Forms.Button()
        Me.btnLoadB21Planner = New System.Windows.Forms.Button()
        Me.btnGuideMe = New System.Windows.Forms.Button()
        Me.btnTurnGuideOff = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.BriefingControl1 = New SIGLR.SoaringTools.CommonLibrary.BriefingControl()
        Me.pnlScrollableSurface.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.tabFlightPlan.SuspendLayout()
        Me.pnlGuide.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.grbTaskInfo.SuspendLayout()
        Me.grpTaskPart2.SuspendLayout()
        Me.tabEvent.SuspendLayout()
        Me.pnlWizardEvent.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.grpGroupEventPost.SuspendLayout()
        Me.tabBriefing.SuspendLayout()
        Me.pnlBriefing.SuspendLayout()
        Me.tabDiscord.SuspendLayout()
        Me.grpDiscordTask.SuspendLayout()
        Me.grpDiscordTaskThread.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.grpDiscordGroupFlight.SuspendLayout()
        Me.grpGroupFlightEvent.SuspendLayout()
        Me.grpDiscordEvent.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlScrollableSurface
        '
        Me.pnlScrollableSurface.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlScrollableSurface.AutoScroll = True
        Me.pnlScrollableSurface.Controls.Add(Me.TabControl1)
        Me.pnlScrollableSurface.Location = New System.Drawing.Point(0, 13)
        Me.pnlScrollableSurface.MinimumSize = New System.Drawing.Size(1024, 768)
        Me.pnlScrollableSurface.Name = "pnlScrollableSurface"
        Me.pnlScrollableSurface.Size = New System.Drawing.Size(1487, 907)
        Me.pnlScrollableSurface.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tabFlightPlan)
        Me.TabControl1.Controls.Add(Me.tabEvent)
        Me.TabControl1.Controls.Add(Me.tabDiscord)
        Me.TabControl1.Controls.Add(Me.tabBriefing)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.ItemSize = New System.Drawing.Size(90, 25)
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1487, 907)
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControl1.TabIndex = 0
        '
        'tabFlightPlan
        '
        Me.tabFlightPlan.AutoScroll = True
        Me.tabFlightPlan.Controls.Add(Me.pnlGuide)
        Me.tabFlightPlan.Controls.Add(Me.grbTaskInfo)
        Me.tabFlightPlan.Controls.Add(Me.txtFlightPlanFile)
        Me.tabFlightPlan.Controls.Add(Me.btnSelectFlightPlan)
        Me.tabFlightPlan.Controls.Add(Me.grpTaskPart2)
        Me.tabFlightPlan.Location = New System.Drawing.Point(4, 29)
        Me.tabFlightPlan.Name = "tabFlightPlan"
        Me.tabFlightPlan.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFlightPlan.Size = New System.Drawing.Size(1479, 874)
        Me.tabFlightPlan.TabIndex = 0
        Me.tabFlightPlan.Text = "Flight Plan"
        Me.tabFlightPlan.UseVisualStyleBackColor = True
        '
        'pnlGuide
        '
        Me.pnlGuide.BackColor = System.Drawing.Color.Gray
        Me.pnlGuide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlGuide.Controls.Add(Me.btnGuideNext)
        Me.pnlGuide.Controls.Add(Me.Panel3)
        Me.pnlGuide.Controls.Add(Me.pnlArrow)
        Me.pnlGuide.Location = New System.Drawing.Point(485, -3)
        Me.pnlGuide.Name = "pnlGuide"
        Me.pnlGuide.Size = New System.Drawing.Size(750, 89)
        Me.pnlGuide.TabIndex = 82
        Me.pnlGuide.Visible = False
        '
        'btnGuideNext
        '
        Me.btnGuideNext.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGuideNext.Location = New System.Drawing.Point(3, 3)
        Me.btnGuideNext.Name = "btnGuideNext"
        Me.btnGuideNext.Size = New System.Drawing.Size(73, 83)
        Me.btnGuideNext.TabIndex = 3
        Me.btnGuideNext.Text = "Next"
        Me.ToolTip1.SetToolTip(Me.btnGuideNext, "Click here to go to the next step in the guide.")
        Me.btnGuideNext.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.Gray
        Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Panel3.Controls.Add(Me.lblGuideInstructions)
        Me.Panel3.Location = New System.Drawing.Point(84, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(586, 89)
        Me.Panel3.TabIndex = 81
        '
        'lblGuideInstructions
        '
        Me.lblGuideInstructions.Font = New System.Drawing.Font("Segoe UI Variable Display Semib", 13.74545!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGuideInstructions.ForeColor = System.Drawing.Color.White
        Me.lblGuideInstructions.Location = New System.Drawing.Point(-1, 0)
        Me.lblGuideInstructions.Name = "lblGuideInstructions"
        Me.lblGuideInstructions.Size = New System.Drawing.Size(584, 89)
        Me.lblGuideInstructions.TabIndex = 0
        Me.lblGuideInstructions.Text = "Click the ""Flight Plan"" button and select the flight plan to use for this task."
        Me.lblGuideInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlArrow
        '
        Me.pnlArrow.BackColor = System.Drawing.Color.Gray
        Me.pnlArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        Me.pnlArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlArrow.Location = New System.Drawing.Point(667, 0)
        Me.pnlArrow.Name = "pnlArrow"
        Me.pnlArrow.Size = New System.Drawing.Size(91, 89)
        Me.pnlArrow.TabIndex = 80
        '
        'grbTaskInfo
        '
        Me.grbTaskInfo.Controls.Add(Me.Label9)
        Me.grbTaskInfo.Controls.Add(Me.chkTitleLock)
        Me.grbTaskInfo.Controls.Add(Me.chkArrivalLock)
        Me.grbTaskInfo.Controls.Add(Me.chkDepartureLock)
        Me.grbTaskInfo.Controls.Add(Me.chkSoaringTypeThermal)
        Me.grbTaskInfo.Controls.Add(Me.chkSoaringTypeRidge)
        Me.grbTaskInfo.Controls.Add(Me.txtSoaringTypeExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.Label8)
        Me.grbTaskInfo.Controls.Add(Me.txtArrivalExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.txtArrivalName)
        Me.grbTaskInfo.Controls.Add(Me.txtArrivalICAO)
        Me.grbTaskInfo.Controls.Add(Me.Label7)
        Me.grbTaskInfo.Controls.Add(Me.txtDepExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.txtDepName)
        Me.grbTaskInfo.Controls.Add(Me.txtSimDateTimeExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.Label5)
        Me.grbTaskInfo.Controls.Add(Me.dtSimLocalTime)
        Me.grbTaskInfo.Controls.Add(Me.chkIncludeYear)
        Me.grbTaskInfo.Controls.Add(Me.Label4)
        Me.grbTaskInfo.Controls.Add(Me.dtSimDate)
        Me.grbTaskInfo.Controls.Add(Me.txtDepartureICAO)
        Me.grbTaskInfo.Controls.Add(Me.Label3)
        Me.grbTaskInfo.Controls.Add(Me.txtMainArea)
        Me.grbTaskInfo.Controls.Add(Me.Label2)
        Me.grbTaskInfo.Controls.Add(Me.txtTitle)
        Me.grbTaskInfo.Controls.Add(Me.Label1)
        Me.grbTaskInfo.Controls.Add(Me.cboSpeedUnits)
        Me.grbTaskInfo.Controls.Add(Me.txtMinAvgSpeed)
        Me.grbTaskInfo.Controls.Add(Me.Label21)
        Me.grbTaskInfo.Controls.Add(Me.txtMaxAvgSpeed)
        Me.grbTaskInfo.Controls.Add(Me.Label22)
        Me.grbTaskInfo.Controls.Add(Me.btnSelectWeatherFile)
        Me.grbTaskInfo.Controls.Add(Me.txtWeatherFile)
        Me.grbTaskInfo.Controls.Add(Me.txtDurationMin)
        Me.grbTaskInfo.Controls.Add(Me.Label12)
        Me.grbTaskInfo.Controls.Add(Me.txtDurationMax)
        Me.grbTaskInfo.Controls.Add(Me.Label13)
        Me.grbTaskInfo.Controls.Add(Me.txtDurationExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.Label14)
        Me.grbTaskInfo.Controls.Add(Me.txtCredits)
        Me.grbTaskInfo.Controls.Add(Me.Label15)
        Me.grbTaskInfo.Controls.Add(Me.Label18)
        Me.grbTaskInfo.Controls.Add(Me.lblTotalDistanceAndMiles)
        Me.grbTaskInfo.Controls.Add(Me.lblTrackDistanceAndMiles)
        Me.grbTaskInfo.Controls.Add(Me.cboDifficulty)
        Me.grbTaskInfo.Controls.Add(Me.txtDistanceTotal)
        Me.grbTaskInfo.Controls.Add(Me.txtDistanceTrack)
        Me.grbTaskInfo.Controls.Add(Me.cboRecommendedGliders)
        Me.grbTaskInfo.Controls.Add(Me.txtDifficultyExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.chkDescriptionLock)
        Me.grbTaskInfo.Controls.Add(Me.Label16)
        Me.grbTaskInfo.Controls.Add(Me.txtShortDescription)
        Me.grbTaskInfo.Controls.Add(Me.Label17)
        Me.grbTaskInfo.Controls.Add(Me.txtLongDescription)
        Me.grbTaskInfo.Enabled = False
        Me.grbTaskInfo.Location = New System.Drawing.Point(8, 57)
        Me.grbTaskInfo.Name = "grbTaskInfo"
        Me.grbTaskInfo.Size = New System.Drawing.Size(729, 803)
        Me.grbTaskInfo.TabIndex = 2
        Me.grbTaskInfo.TabStop = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(4, 307)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(85, 26)
        Me.Label9.TabIndex = 27
        Me.Label9.Text = "Distance"
        '
        'chkTitleLock
        '
        Me.chkTitleLock.AutoSize = True
        Me.chkTitleLock.Location = New System.Drawing.Point(168, 73)
        Me.chkTitleLock.Name = "chkTitleLock"
        Me.chkTitleLock.Size = New System.Drawing.Size(15, 14)
        Me.chkTitleLock.TabIndex = 3
        Me.chkTitleLock.Tag = "3"
        Me.ToolTip1.SetToolTip(Me.chkTitleLock, "When checked, title will not be read from flight plan.")
        Me.chkTitleLock.UseVisualStyleBackColor = True
        '
        'chkArrivalLock
        '
        Me.chkArrivalLock.AutoSize = True
        Me.chkArrivalLock.Location = New System.Drawing.Point(446, 246)
        Me.chkArrivalLock.Name = "chkArrivalLock"
        Me.chkArrivalLock.Size = New System.Drawing.Size(15, 14)
        Me.chkArrivalLock.TabIndex = 21
        Me.chkArrivalLock.Tag = "7"
        Me.ToolTip1.SetToolTip(Me.chkArrivalLock, "When checked, airport name will not be read from flight plan.")
        Me.chkArrivalLock.UseVisualStyleBackColor = True
        '
        'chkDepartureLock
        '
        Me.chkDepartureLock.AutoSize = True
        Me.chkDepartureLock.Location = New System.Drawing.Point(446, 212)
        Me.chkDepartureLock.Name = "chkDepartureLock"
        Me.chkDepartureLock.Size = New System.Drawing.Size(15, 14)
        Me.chkDepartureLock.TabIndex = 16
        Me.chkDepartureLock.Tag = "6"
        Me.ToolTip1.SetToolTip(Me.chkDepartureLock, "When checked, airport name will not be read from flight plan.")
        Me.chkDepartureLock.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeThermal
        '
        Me.chkSoaringTypeThermal.AutoSize = True
        Me.chkSoaringTypeThermal.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeThermal.Location = New System.Drawing.Point(286, 271)
        Me.chkSoaringTypeThermal.Name = "chkSoaringTypeThermal"
        Me.chkSoaringTypeThermal.Size = New System.Drawing.Size(107, 30)
        Me.chkSoaringTypeThermal.TabIndex = 25
        Me.chkSoaringTypeThermal.Tag = "8"
        Me.chkSoaringTypeThermal.Text = "Thermals"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeThermal, "Check if track contains thermal soaring.")
        Me.chkSoaringTypeThermal.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeRidge
        '
        Me.chkSoaringTypeRidge.AutoSize = True
        Me.chkSoaringTypeRidge.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeRidge.Location = New System.Drawing.Point(189, 271)
        Me.chkSoaringTypeRidge.Name = "chkSoaringTypeRidge"
        Me.chkSoaringTypeRidge.Size = New System.Drawing.Size(78, 30)
        Me.chkSoaringTypeRidge.TabIndex = 24
        Me.chkSoaringTypeRidge.Tag = "8"
        Me.chkSoaringTypeRidge.Text = "Ridge"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeRidge, "Check if track contains ridge soaring.")
        Me.chkSoaringTypeRidge.UseVisualStyleBackColor = True
        '
        'txtSoaringTypeExtraInfo
        '
        Me.txtSoaringTypeExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSoaringTypeExtraInfo.Location = New System.Drawing.Point(412, 270)
        Me.txtSoaringTypeExtraInfo.Name = "txtSoaringTypeExtraInfo"
        Me.txtSoaringTypeExtraInfo.Size = New System.Drawing.Size(308, 32)
        Me.txtSoaringTypeExtraInfo.TabIndex = 26
        Me.txtSoaringTypeExtraInfo.Tag = "8"
        Me.ToolTip1.SetToolTip(Me.txtSoaringTypeExtraInfo, "Any extra information to add to the soaring type line.")
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(4, 273)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(120, 26)
        Me.Label8.TabIndex = 23
        Me.Label8.Text = "Soaring Type"
        '
        'txtArrivalExtraInfo
        '
        Me.txtArrivalExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtArrivalExtraInfo.Location = New System.Drawing.Point(467, 236)
        Me.txtArrivalExtraInfo.Name = "txtArrivalExtraInfo"
        Me.txtArrivalExtraInfo.Size = New System.Drawing.Size(253, 32)
        Me.txtArrivalExtraInfo.TabIndex = 22
        Me.txtArrivalExtraInfo.Tag = "7"
        Me.ToolTip1.SetToolTip(Me.txtArrivalExtraInfo, "Any extra information to add to the arrival line.")
        '
        'txtArrivalName
        '
        Me.txtArrivalName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtArrivalName.Location = New System.Drawing.Point(276, 236)
        Me.txtArrivalName.Name = "txtArrivalName"
        Me.txtArrivalName.Size = New System.Drawing.Size(164, 32)
        Me.txtArrivalName.TabIndex = 20
        Me.txtArrivalName.Tag = "7"
        Me.ToolTip1.SetToolTip(Me.txtArrivalName, "Arrival airport name, can be automatic based on ICAO.")
        '
        'txtArrivalICAO
        '
        Me.txtArrivalICAO.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtArrivalICAO.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtArrivalICAO.Location = New System.Drawing.Point(189, 236)
        Me.txtArrivalICAO.Name = "txtArrivalICAO"
        Me.txtArrivalICAO.ReadOnly = True
        Me.txtArrivalICAO.Size = New System.Drawing.Size(81, 32)
        Me.txtArrivalICAO.TabIndex = 19
        Me.txtArrivalICAO.Tag = "7"
        Me.ToolTip1.SetToolTip(Me.txtArrivalICAO, "Arrival airport ICAO, can come from the flight plan file.")
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(4, 239)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(64, 26)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Arrival"
        '
        'txtDepExtraInfo
        '
        Me.txtDepExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDepExtraInfo.Location = New System.Drawing.Point(467, 202)
        Me.txtDepExtraInfo.Name = "txtDepExtraInfo"
        Me.txtDepExtraInfo.Size = New System.Drawing.Size(253, 32)
        Me.txtDepExtraInfo.TabIndex = 17
        Me.txtDepExtraInfo.Tag = "6"
        Me.ToolTip1.SetToolTip(Me.txtDepExtraInfo, "Any extra information to add to the departure line.")
        '
        'txtDepName
        '
        Me.txtDepName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDepName.Location = New System.Drawing.Point(276, 202)
        Me.txtDepName.Name = "txtDepName"
        Me.txtDepName.Size = New System.Drawing.Size(164, 32)
        Me.txtDepName.TabIndex = 15
        Me.txtDepName.Tag = "6"
        Me.ToolTip1.SetToolTip(Me.txtDepName, "Departure airport name, can be automatic based on ICAO.")
        '
        'txtSimDateTimeExtraInfo
        '
        Me.txtSimDateTimeExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSimDateTimeExtraInfo.Location = New System.Drawing.Point(299, 134)
        Me.txtSimDateTimeExtraInfo.Name = "txtSimDateTimeExtraInfo"
        Me.txtSimDateTimeExtraInfo.Size = New System.Drawing.Size(421, 32)
        Me.txtSimDateTimeExtraInfo.TabIndex = 10
        Me.txtSimDateTimeExtraInfo.Tag = "4"
        Me.ToolTip1.SetToolTip(Me.txtSimDateTimeExtraInfo, "Any extra information to add to the date/time line.")
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(4, 140)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(134, 26)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Sim Local Time"
        '
        'dtSimLocalTime
        '
        Me.dtSimLocalTime.CustomFormat = "HH:mm tt"
        Me.dtSimLocalTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtSimLocalTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtSimLocalTime.Location = New System.Drawing.Point(189, 134)
        Me.dtSimLocalTime.Name = "dtSimLocalTime"
        Me.dtSimLocalTime.ShowUpDown = True
        Me.dtSimLocalTime.Size = New System.Drawing.Size(104, 31)
        Me.dtSimLocalTime.TabIndex = 9
        Me.dtSimLocalTime.Tag = "4"
        Me.ToolTip1.SetToolTip(Me.dtSimLocalTime, "Local time to set in MSFS for the flight")
        '
        'chkIncludeYear
        '
        Me.chkIncludeYear.AutoSize = True
        Me.chkIncludeYear.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkIncludeYear.Location = New System.Drawing.Point(395, 100)
        Me.chkIncludeYear.Name = "chkIncludeYear"
        Me.chkIncludeYear.Size = New System.Drawing.Size(132, 30)
        Me.chkIncludeYear.TabIndex = 7
        Me.chkIncludeYear.Tag = "4"
        Me.chkIncludeYear.Text = "Include Year"
        Me.ToolTip1.SetToolTip(Me.chkIncludeYear, "When checked, the year will be included (if important)")
        Me.chkIncludeYear.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(4, 106)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(86, 26)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Sim Date"
        '
        'dtSimDate
        '
        Me.dtSimDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtSimDate.Location = New System.Drawing.Point(189, 100)
        Me.dtSimDate.Name = "dtSimDate"
        Me.dtSimDate.Size = New System.Drawing.Size(200, 31)
        Me.dtSimDate.TabIndex = 6
        Me.dtSimDate.Tag = "4"
        Me.ToolTip1.SetToolTip(Me.dtSimDate, "Date to set in MSFS for the flight")
        '
        'txtDepartureICAO
        '
        Me.txtDepartureICAO.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtDepartureICAO.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDepartureICAO.Location = New System.Drawing.Point(189, 202)
        Me.txtDepartureICAO.Name = "txtDepartureICAO"
        Me.txtDepartureICAO.ReadOnly = True
        Me.txtDepartureICAO.Size = New System.Drawing.Size(81, 32)
        Me.txtDepartureICAO.TabIndex = 14
        Me.txtDepartureICAO.Tag = "6"
        Me.ToolTip1.SetToolTip(Me.txtDepartureICAO, "Departure airport ICAO, can come from the flight plan file.")
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(4, 205)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(97, 26)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Departure"
        '
        'txtMainArea
        '
        Me.txtMainArea.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMainArea.Location = New System.Drawing.Point(189, 168)
        Me.txtMainArea.Name = "txtMainArea"
        Me.txtMainArea.Size = New System.Drawing.Size(531, 32)
        Me.txtMainArea.TabIndex = 12
        Me.txtMainArea.Tag = "5"
        Me.ToolTip1.SetToolTip(Me.txtMainArea, "The main area and/or point of interest of the flight")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(4, 171)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(137, 26)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "Main area / POI"
        '
        'txtTitle
        '
        Me.txtTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTitle.Location = New System.Drawing.Point(189, 66)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(400, 32)
        Me.txtTitle.TabIndex = 4
        Me.txtTitle.Tag = "3"
        Me.ToolTip1.SetToolTip(Me.txtTitle, "Track title - can come from the flight plan's title.")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(4, 70)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(47, 26)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Title"
        '
        'cboSpeedUnits
        '
        Me.cboSpeedUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSpeedUnits.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboSpeedUnits.FormattingEnabled = True
        Me.cboSpeedUnits.Items.AddRange(New Object() {"Km/h", "Miles/h", "Knots"})
        Me.cboSpeedUnits.Location = New System.Drawing.Point(337, 338)
        Me.cboSpeedUnits.Name = "cboSpeedUnits"
        Me.cboSpeedUnits.Size = New System.Drawing.Size(93, 32)
        Me.cboSpeedUnits.TabIndex = 36
        Me.cboSpeedUnits.Tag = "10"
        Me.ToolTip1.SetToolTip(Me.cboSpeedUnits, "Select units to use for average speed input.")
        '
        'txtMinAvgSpeed
        '
        Me.txtMinAvgSpeed.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMinAvgSpeed.Location = New System.Drawing.Point(189, 338)
        Me.txtMinAvgSpeed.Name = "txtMinAvgSpeed"
        Me.txtMinAvgSpeed.Size = New System.Drawing.Size(50, 32)
        Me.txtMinAvgSpeed.TabIndex = 33
        Me.txtMinAvgSpeed.Tag = "10"
        Me.txtMinAvgSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtMinAvgSpeed, "Minimum average speed - used to calculate maximum duration.")
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(245, 341)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(30, 26)
        Me.Label21.TabIndex = 34
        Me.Label21.Text = "to"
        '
        'txtMaxAvgSpeed
        '
        Me.txtMaxAvgSpeed.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMaxAvgSpeed.Location = New System.Drawing.Point(281, 338)
        Me.txtMaxAvgSpeed.Name = "txtMaxAvgSpeed"
        Me.txtMaxAvgSpeed.Size = New System.Drawing.Size(50, 32)
        Me.txtMaxAvgSpeed.TabIndex = 35
        Me.txtMaxAvgSpeed.Tag = "10"
        Me.txtMaxAvgSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtMaxAvgSpeed, "Maximum average speed - used to calculate minimum duration.")
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(4, 341)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(112, 26)
        Me.Label22.TabIndex = 32
        Me.Label22.Text = "Avg. speeds"
        '
        'btnSelectWeatherFile
        '
        Me.btnSelectWeatherFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectWeatherFile.Location = New System.Drawing.Point(8, 21)
        Me.btnSelectWeatherFile.Name = "btnSelectWeatherFile"
        Me.btnSelectWeatherFile.Size = New System.Drawing.Size(175, 35)
        Me.btnSelectWeatherFile.TabIndex = 0
        Me.btnSelectWeatherFile.Tag = "2"
        Me.btnSelectWeatherFile.Text = "Weather file"
        Me.ToolTip1.SetToolTip(Me.btnSelectWeatherFile, "Click to select the weather profile file to use and extract information from.")
        Me.btnSelectWeatherFile.UseVisualStyleBackColor = True
        '
        'txtWeatherFile
        '
        Me.txtWeatherFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherFile.Location = New System.Drawing.Point(189, 24)
        Me.txtWeatherFile.Name = "txtWeatherFile"
        Me.txtWeatherFile.ReadOnly = True
        Me.txtWeatherFile.Size = New System.Drawing.Size(531, 32)
        Me.txtWeatherFile.TabIndex = 1
        Me.txtWeatherFile.TabStop = False
        Me.txtWeatherFile.Tag = "2"
        Me.ToolTip1.SetToolTip(Me.txtWeatherFile, "Current weather file selected.")
        '
        'txtDurationMin
        '
        Me.txtDurationMin.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDurationMin.Location = New System.Drawing.Point(189, 372)
        Me.txtDurationMin.Name = "txtDurationMin"
        Me.txtDurationMin.Size = New System.Drawing.Size(50, 32)
        Me.txtDurationMin.TabIndex = 38
        Me.txtDurationMin.Tag = "11"
        Me.txtDurationMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtDurationMin, "Approximate minimum duration in minutes")
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(4, 375)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(132, 26)
        Me.Label12.TabIndex = 37
        Me.Label12.Text = "Duration (min)"
        '
        'txtDurationMax
        '
        Me.txtDurationMax.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDurationMax.Location = New System.Drawing.Point(281, 372)
        Me.txtDurationMax.Name = "txtDurationMax"
        Me.txtDurationMax.Size = New System.Drawing.Size(50, 32)
        Me.txtDurationMax.TabIndex = 40
        Me.txtDurationMax.Tag = "11"
        Me.txtDurationMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtDurationMax, "Approximate maximum duration in minutes")
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(245, 375)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(30, 26)
        Me.Label13.TabIndex = 39
        Me.Label13.Text = "to"
        '
        'txtDurationExtraInfo
        '
        Me.txtDurationExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDurationExtraInfo.Location = New System.Drawing.Point(337, 372)
        Me.txtDurationExtraInfo.Name = "txtDurationExtraInfo"
        Me.txtDurationExtraInfo.Size = New System.Drawing.Size(383, 32)
        Me.txtDurationExtraInfo.TabIndex = 41
        Me.txtDurationExtraInfo.Tag = "11"
        Me.ToolTip1.SetToolTip(Me.txtDurationExtraInfo, "Any extra information to add on the duration line.")
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(4, 409)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(133, 26)
        Me.Label14.TabIndex = 42
        Me.Label14.Text = "Recom. gliders"
        '
        'txtCredits
        '
        Me.txtCredits.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCredits.Location = New System.Drawing.Point(189, 535)
        Me.txtCredits.Name = "txtCredits"
        Me.txtCredits.Size = New System.Drawing.Size(531, 32)
        Me.txtCredits.TabIndex = 51
        Me.txtCredits.Tag = "15"
        Me.txtCredits.Text = "All credits to @UserName for this task."
        Me.ToolTip1.SetToolTip(Me.txtCredits, "Specify credits for this flight as required.")
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(4, 443)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(144, 26)
        Me.Label15.TabIndex = 44
        Me.Label15.Text = "Difficulty Rating"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.Location = New System.Drawing.Point(4, 538)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(73, 26)
        Me.Label18.TabIndex = 50
        Me.Label18.Text = "Credits"
        '
        'lblTotalDistanceAndMiles
        '
        Me.lblTotalDistanceAndMiles.AutoSize = True
        Me.lblTotalDistanceAndMiles.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalDistanceAndMiles.Location = New System.Drawing.Point(245, 306)
        Me.lblTotalDistanceAndMiles.Name = "lblTotalDistanceAndMiles"
        Me.lblTotalDistanceAndMiles.Size = New System.Drawing.Size(160, 26)
        Me.lblTotalDistanceAndMiles.TabIndex = 29
        Me.lblTotalDistanceAndMiles.Text = "km / 9999 mi Total"
        '
        'lblTrackDistanceAndMiles
        '
        Me.lblTrackDistanceAndMiles.AutoSize = True
        Me.lblTrackDistanceAndMiles.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTrackDistanceAndMiles.Location = New System.Drawing.Point(468, 306)
        Me.lblTrackDistanceAndMiles.Name = "lblTrackDistanceAndMiles"
        Me.lblTrackDistanceAndMiles.Size = New System.Drawing.Size(156, 26)
        Me.lblTrackDistanceAndMiles.TabIndex = 31
        Me.lblTrackDistanceAndMiles.Text = "km / 9999 mi Task"
        '
        'cboDifficulty
        '
        Me.cboDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDifficulty.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboDifficulty.FormattingEnabled = True
        Me.cboDifficulty.Items.AddRange(New Object() {"0. None / Custom", "1. Beginner", "2. Student", "3. Experimented", "4. Professional", "5. Champion"})
        Me.cboDifficulty.Location = New System.Drawing.Point(189, 440)
        Me.cboDifficulty.Name = "cboDifficulty"
        Me.cboDifficulty.Size = New System.Drawing.Size(251, 32)
        Me.cboDifficulty.TabIndex = 45
        Me.cboDifficulty.Tag = "13"
        Me.ToolTip1.SetToolTip(Me.cboDifficulty, "Select standard difficulty rating or None to use your own.")
        '
        'txtDistanceTotal
        '
        Me.txtDistanceTotal.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDistanceTotal.Location = New System.Drawing.Point(189, 304)
        Me.txtDistanceTotal.Name = "txtDistanceTotal"
        Me.txtDistanceTotal.ReadOnly = True
        Me.txtDistanceTotal.Size = New System.Drawing.Size(50, 32)
        Me.txtDistanceTotal.TabIndex = 28
        Me.txtDistanceTotal.Tag = "9"
        Me.txtDistanceTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtDistanceTotal, "Total flight distance (including distance outside the task) read from flight plan" &
        " file.")
        '
        'txtDistanceTrack
        '
        Me.txtDistanceTrack.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDistanceTrack.Location = New System.Drawing.Point(412, 304)
        Me.txtDistanceTrack.Name = "txtDistanceTrack"
        Me.txtDistanceTrack.ReadOnly = True
        Me.txtDistanceTrack.Size = New System.Drawing.Size(50, 32)
        Me.txtDistanceTrack.TabIndex = 30
        Me.txtDistanceTrack.Tag = "9"
        Me.txtDistanceTrack.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtDistanceTrack, "Task distance (this is the distance shown by B21 Planner) read from flight plan f" &
        "ile.")
        '
        'cboRecommendedGliders
        '
        Me.cboRecommendedGliders.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboRecommendedGliders.FormattingEnabled = True
        Me.cboRecommendedGliders.Items.AddRange(New Object() {"Any", "Flapped"})
        Me.cboRecommendedGliders.Location = New System.Drawing.Point(189, 406)
        Me.cboRecommendedGliders.Name = "cboRecommendedGliders"
        Me.cboRecommendedGliders.Size = New System.Drawing.Size(531, 32)
        Me.cboRecommendedGliders.TabIndex = 43
        Me.cboRecommendedGliders.Tag = "12"
        Me.ToolTip1.SetToolTip(Me.cboRecommendedGliders, "Recommended gliders (suggestions in the list or enter your own)")
        '
        'txtDifficultyExtraInfo
        '
        Me.txtDifficultyExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDifficultyExtraInfo.Location = New System.Drawing.Point(446, 440)
        Me.txtDifficultyExtraInfo.Name = "txtDifficultyExtraInfo"
        Me.txtDifficultyExtraInfo.Size = New System.Drawing.Size(274, 32)
        Me.txtDifficultyExtraInfo.TabIndex = 46
        Me.txtDifficultyExtraInfo.Tag = "13"
        Me.ToolTip1.SetToolTip(Me.txtDifficultyExtraInfo, "Any extra information or custom rating to use on the difficulty line.")
        '
        'chkDescriptionLock
        '
        Me.chkDescriptionLock.AutoSize = True
        Me.chkDescriptionLock.Location = New System.Drawing.Point(168, 486)
        Me.chkDescriptionLock.Name = "chkDescriptionLock"
        Me.chkDescriptionLock.Size = New System.Drawing.Size(15, 14)
        Me.chkDescriptionLock.TabIndex = 48
        Me.chkDescriptionLock.Tag = "14"
        Me.ToolTip1.SetToolTip(Me.chkDescriptionLock, "When checked, description will not be read from flight plan.")
        Me.chkDescriptionLock.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(4, 480)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(160, 26)
        Me.Label16.TabIndex = 47
        Me.Label16.Text = "Short Description"
        '
        'txtShortDescription
        '
        Me.txtShortDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtShortDescription.Location = New System.Drawing.Point(189, 476)
        Me.txtShortDescription.Multiline = True
        Me.txtShortDescription.Name = "txtShortDescription"
        Me.txtShortDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtShortDescription.Size = New System.Drawing.Size(531, 53)
        Me.txtShortDescription.TabIndex = 49
        Me.txtShortDescription.Tag = "14"
        Me.ToolTip1.SetToolTip(Me.txtShortDescription, "Short description of the flight, can come from the flight plan file.")
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(4, 572)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(155, 26)
        Me.Label17.TabIndex = 52
        Me.Label17.Text = "Long Description"
        '
        'txtLongDescription
        '
        Me.txtLongDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLongDescription.Location = New System.Drawing.Point(189, 569)
        Me.txtLongDescription.Multiline = True
        Me.txtLongDescription.Name = "txtLongDescription"
        Me.txtLongDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLongDescription.Size = New System.Drawing.Size(531, 224)
        Me.txtLongDescription.TabIndex = 53
        Me.txtLongDescription.Tag = "16"
        Me.ToolTip1.SetToolTip(Me.txtLongDescription, "Full (long) description of the flight.")
        '
        'txtFlightPlanFile
        '
        Me.txtFlightPlanFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFlightPlanFile.Location = New System.Drawing.Point(197, 19)
        Me.txtFlightPlanFile.Name = "txtFlightPlanFile"
        Me.txtFlightPlanFile.ReadOnly = True
        Me.txtFlightPlanFile.Size = New System.Drawing.Size(531, 32)
        Me.txtFlightPlanFile.TabIndex = 1
        Me.txtFlightPlanFile.TabStop = False
        Me.txtFlightPlanFile.Tag = "1"
        Me.ToolTip1.SetToolTip(Me.txtFlightPlanFile, "Current flight plan file selected.")
        '
        'btnSelectFlightPlan
        '
        Me.btnSelectFlightPlan.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectFlightPlan.Location = New System.Drawing.Point(16, 16)
        Me.btnSelectFlightPlan.Name = "btnSelectFlightPlan"
        Me.btnSelectFlightPlan.Size = New System.Drawing.Size(175, 35)
        Me.btnSelectFlightPlan.TabIndex = 0
        Me.btnSelectFlightPlan.Tag = "1"
        Me.btnSelectFlightPlan.Text = "Flight Plan"
        Me.ToolTip1.SetToolTip(Me.btnSelectFlightPlan, "Click to select the flight plan file to use and extract information from.")
        Me.btnSelectFlightPlan.UseVisualStyleBackColor = True
        '
        'grpTaskPart2
        '
        Me.grpTaskPart2.Controls.Add(Me.chkLockCountries)
        Me.grpTaskPart2.Controls.Add(Me.btnMoveCountryDown)
        Me.grpTaskPart2.Controls.Add(Me.btnMoveCountryUp)
        Me.grpTaskPart2.Controls.Add(Me.btnRemoveCountry)
        Me.grpTaskPart2.Controls.Add(Me.btnAddCountry)
        Me.grpTaskPart2.Controls.Add(Me.lstAllCountries)
        Me.grpTaskPart2.Controls.Add(Me.cboCountryFlag)
        Me.grpTaskPart2.Controls.Add(Me.Label11)
        Me.grpTaskPart2.Controls.Add(Me.btnExtraFileDown)
        Me.grpTaskPart2.Controls.Add(Me.btnExtraFileUp)
        Me.grpTaskPart2.Controls.Add(Me.btnRemoveExtraFile)
        Me.grpTaskPart2.Controls.Add(Me.btnAddExtraFile)
        Me.grpTaskPart2.Controls.Add(Me.lstAllFiles)
        Me.grpTaskPart2.Controls.Add(Me.chkUseOnlyWeatherSummary)
        Me.grpTaskPart2.Controls.Add(Me.txtWeatherSummary)
        Me.grpTaskPart2.Controls.Add(Me.Label19)
        Me.grpTaskPart2.Location = New System.Drawing.Point(743, 57)
        Me.grpTaskPart2.Name = "grpTaskPart2"
        Me.grpTaskPart2.Size = New System.Drawing.Size(729, 803)
        Me.grpTaskPart2.TabIndex = 83
        Me.grpTaskPart2.TabStop = False
        '
        'chkLockCountries
        '
        Me.chkLockCountries.AutoSize = True
        Me.chkLockCountries.Location = New System.Drawing.Point(166, 70)
        Me.chkLockCountries.Name = "chkLockCountries"
        Me.chkLockCountries.Size = New System.Drawing.Size(15, 14)
        Me.chkLockCountries.TabIndex = 65
        Me.chkLockCountries.Tag = ""
        Me.ToolTip1.SetToolTip(Me.chkLockCountries, "When checked, countries will not be automatically loaded from flight plan.")
        Me.chkLockCountries.UseVisualStyleBackColor = True
        '
        'btnMoveCountryDown
        '
        Me.btnMoveCountryDown.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMoveCountryDown.Location = New System.Drawing.Point(320, 97)
        Me.btnMoveCountryDown.Name = "btnMoveCountryDown"
        Me.btnMoveCountryDown.Size = New System.Drawing.Size(38, 29)
        Me.btnMoveCountryDown.TabIndex = 66
        Me.btnMoveCountryDown.Tag = ""
        Me.btnMoveCountryDown.Text = "▼"
        Me.ToolTip1.SetToolTip(Me.btnMoveCountryDown, "Click to move the selected countries down in the list.")
        Me.btnMoveCountryDown.UseVisualStyleBackColor = True
        '
        'btnMoveCountryUp
        '
        Me.btnMoveCountryUp.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMoveCountryUp.Location = New System.Drawing.Point(320, 61)
        Me.btnMoveCountryUp.Name = "btnMoveCountryUp"
        Me.btnMoveCountryUp.Size = New System.Drawing.Size(38, 29)
        Me.btnMoveCountryUp.TabIndex = 67
        Me.btnMoveCountryUp.Tag = ""
        Me.btnMoveCountryUp.Text = "▲"
        Me.ToolTip1.SetToolTip(Me.btnMoveCountryUp, "Click to move the selected countries up in the list.")
        Me.btnMoveCountryUp.UseVisualStyleBackColor = True
        '
        'btnRemoveCountry
        '
        Me.btnRemoveCountry.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveCountry.Location = New System.Drawing.Point(189, 97)
        Me.btnRemoveCountry.Name = "btnRemoveCountry"
        Me.btnRemoveCountry.Size = New System.Drawing.Size(125, 29)
        Me.btnRemoveCountry.TabIndex = 68
        Me.btnRemoveCountry.Tag = ""
        Me.btnRemoveCountry.Text = "Remove Country"
        Me.ToolTip1.SetToolTip(Me.btnRemoveCountry, "Remove the selected countries from the list")
        Me.btnRemoveCountry.UseVisualStyleBackColor = True
        '
        'btnAddCountry
        '
        Me.btnAddCountry.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddCountry.Location = New System.Drawing.Point(189, 62)
        Me.btnAddCountry.Name = "btnAddCountry"
        Me.btnAddCountry.Size = New System.Drawing.Size(125, 29)
        Me.btnAddCountry.TabIndex = 69
        Me.btnAddCountry.Tag = ""
        Me.btnAddCountry.Text = "Add Country"
        Me.ToolTip1.SetToolTip(Me.btnAddCountry, "Click to add the selected country to the list")
        Me.btnAddCountry.UseVisualStyleBackColor = True
        '
        'lstAllCountries
        '
        Me.lstAllCountries.FormattingEnabled = True
        Me.lstAllCountries.HorizontalScrollbar = True
        Me.lstAllCountries.ItemHeight = 20
        Me.lstAllCountries.Location = New System.Drawing.Point(364, 62)
        Me.lstAllCountries.Name = "lstAllCountries"
        Me.lstAllCountries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAllCountries.Size = New System.Drawing.Size(356, 64)
        Me.lstAllCountries.TabIndex = 70
        Me.lstAllCountries.Tag = ""
        Me.ToolTip1.SetToolTip(Me.lstAllCountries, "List of the countries which flags are added to the title")
        '
        'cboCountryFlag
        '
        Me.cboCountryFlag.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cboCountryFlag.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboCountryFlag.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboCountryFlag.FormattingEnabled = True
        Me.cboCountryFlag.Location = New System.Drawing.Point(189, 24)
        Me.cboCountryFlag.Name = "cboCountryFlag"
        Me.cboCountryFlag.Size = New System.Drawing.Size(531, 32)
        Me.cboCountryFlag.TabIndex = 71
        Me.cboCountryFlag.Tag = ""
        Me.ToolTip1.SetToolTip(Me.cboCountryFlag, "Select a country to add to the selection (for its flag to be added in the title)")
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(6, 27)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(145, 26)
        Me.Label11.TabIndex = 80
        Me.Label11.Text = "Countries/Flags"
        '
        'btnExtraFileDown
        '
        Me.btnExtraFileDown.Enabled = False
        Me.btnExtraFileDown.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExtraFileDown.Location = New System.Drawing.Point(99, 256)
        Me.btnExtraFileDown.Name = "btnExtraFileDown"
        Me.btnExtraFileDown.Size = New System.Drawing.Size(84, 35)
        Me.btnExtraFileDown.TabIndex = 78
        Me.btnExtraFileDown.Tag = "18"
        Me.btnExtraFileDown.Text = "Down"
        Me.ToolTip1.SetToolTip(Me.btnExtraFileDown, "Click to move the selected file down in the list.")
        Me.btnExtraFileDown.UseVisualStyleBackColor = True
        '
        'btnExtraFileUp
        '
        Me.btnExtraFileUp.Enabled = False
        Me.btnExtraFileUp.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExtraFileUp.Location = New System.Drawing.Point(8, 256)
        Me.btnExtraFileUp.Name = "btnExtraFileUp"
        Me.btnExtraFileUp.Size = New System.Drawing.Size(84, 35)
        Me.btnExtraFileUp.TabIndex = 77
        Me.btnExtraFileUp.Tag = "18"
        Me.btnExtraFileUp.Text = "Up"
        Me.ToolTip1.SetToolTip(Me.btnExtraFileUp, "Click to move the selected file up in the list.")
        Me.btnExtraFileUp.UseVisualStyleBackColor = True
        '
        'btnRemoveExtraFile
        '
        Me.btnRemoveExtraFile.Enabled = False
        Me.btnRemoveExtraFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveExtraFile.Location = New System.Drawing.Point(8, 215)
        Me.btnRemoveExtraFile.Name = "btnRemoveExtraFile"
        Me.btnRemoveExtraFile.Size = New System.Drawing.Size(175, 35)
        Me.btnRemoveExtraFile.TabIndex = 76
        Me.btnRemoveExtraFile.Tag = "18"
        Me.btnRemoveExtraFile.Text = "Remove selected file"
        Me.ToolTip1.SetToolTip(Me.btnRemoveExtraFile, "Click to remove the slelected extra file from the flight plan.")
        Me.btnRemoveExtraFile.UseVisualStyleBackColor = True
        '
        'btnAddExtraFile
        '
        Me.btnAddExtraFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddExtraFile.Location = New System.Drawing.Point(8, 174)
        Me.btnAddExtraFile.Name = "btnAddExtraFile"
        Me.btnAddExtraFile.Size = New System.Drawing.Size(175, 35)
        Me.btnAddExtraFile.TabIndex = 75
        Me.btnAddExtraFile.Tag = "18"
        Me.btnAddExtraFile.Text = "Add extra file"
        Me.ToolTip1.SetToolTip(Me.btnAddExtraFile, "Click to add an extra file to include with the flight plan.")
        Me.btnAddExtraFile.UseVisualStyleBackColor = True
        '
        'lstAllFiles
        '
        Me.lstAllFiles.FormattingEnabled = True
        Me.lstAllFiles.HorizontalScrollbar = True
        Me.lstAllFiles.ItemHeight = 20
        Me.lstAllFiles.Location = New System.Drawing.Point(189, 174)
        Me.lstAllFiles.Name = "lstAllFiles"
        Me.lstAllFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAllFiles.Size = New System.Drawing.Size(531, 124)
        Me.lstAllFiles.TabIndex = 79
        Me.lstAllFiles.Tag = "18"
        Me.ToolTip1.SetToolTip(Me.lstAllFiles, "List of the extra files to include with the flight plan.")
        '
        'chkUseOnlyWeatherSummary
        '
        Me.chkUseOnlyWeatherSummary.AutoSize = True
        Me.chkUseOnlyWeatherSummary.Location = New System.Drawing.Point(168, 149)
        Me.chkUseOnlyWeatherSummary.Name = "chkUseOnlyWeatherSummary"
        Me.chkUseOnlyWeatherSummary.Size = New System.Drawing.Size(15, 14)
        Me.chkUseOnlyWeatherSummary.TabIndex = 73
        Me.chkUseOnlyWeatherSummary.Tag = "17"
        Me.ToolTip1.SetToolTip(Me.chkUseOnlyWeatherSummary, "When checked, only summary will be used for weather information.")
        Me.chkUseOnlyWeatherSummary.UseVisualStyleBackColor = True
        '
        'txtWeatherSummary
        '
        Me.txtWeatherSummary.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherSummary.Location = New System.Drawing.Point(189, 136)
        Me.txtWeatherSummary.Name = "txtWeatherSummary"
        Me.txtWeatherSummary.Size = New System.Drawing.Size(531, 32)
        Me.txtWeatherSummary.TabIndex = 74
        Me.txtWeatherSummary.Tag = "17"
        Me.ToolTip1.SetToolTip(Me.txtWeatherSummary, "Summary of the weather profile.")
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(4, 139)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(166, 26)
        Me.Label19.TabIndex = 72
        Me.Label19.Text = "Weather Summary"
        '
        'tabEvent
        '
        Me.tabEvent.Controls.Add(Me.chkActivateEvent)
        Me.tabEvent.Controls.Add(Me.pnlWizardEvent)
        Me.tabEvent.Controls.Add(Me.grpGroupEventPost)
        Me.tabEvent.Location = New System.Drawing.Point(4, 29)
        Me.tabEvent.Name = "tabEvent"
        Me.tabEvent.Padding = New System.Windows.Forms.Padding(3)
        Me.tabEvent.Size = New System.Drawing.Size(1479, 874)
        Me.tabEvent.TabIndex = 1
        Me.tabEvent.Text = "Event"
        Me.tabEvent.UseVisualStyleBackColor = True
        '
        'chkActivateEvent
        '
        Me.chkActivateEvent.AutoSize = True
        Me.chkActivateEvent.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkActivateEvent.Location = New System.Drawing.Point(13, 6)
        Me.chkActivateEvent.Name = "chkActivateEvent"
        Me.chkActivateEvent.Size = New System.Drawing.Size(232, 25)
        Me.chkActivateEvent.TabIndex = 84
        Me.chkActivateEvent.Text = "Enable Group Event Details"
        Me.ToolTip1.SetToolTip(Me.chkActivateEvent, "Check this to enable the event fields.")
        Me.chkActivateEvent.UseVisualStyleBackColor = True
        '
        'pnlWizardEvent
        '
        Me.pnlWizardEvent.BackColor = System.Drawing.Color.Gray
        Me.pnlWizardEvent.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlWizardEvent.Controls.Add(Me.btnEventGuideNext)
        Me.pnlWizardEvent.Controls.Add(Me.Panel2)
        Me.pnlWizardEvent.Controls.Add(Me.pnlEventArrow)
        Me.pnlWizardEvent.Location = New System.Drawing.Point(725, 680)
        Me.pnlWizardEvent.Name = "pnlWizardEvent"
        Me.pnlWizardEvent.Size = New System.Drawing.Size(750, 89)
        Me.pnlWizardEvent.TabIndex = 83
        Me.pnlWizardEvent.Visible = False
        '
        'btnEventGuideNext
        '
        Me.btnEventGuideNext.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventGuideNext.Location = New System.Drawing.Point(3, 3)
        Me.btnEventGuideNext.Name = "btnEventGuideNext"
        Me.btnEventGuideNext.Size = New System.Drawing.Size(73, 83)
        Me.btnEventGuideNext.TabIndex = 3
        Me.btnEventGuideNext.Text = "Next"
        Me.ToolTip1.SetToolTip(Me.btnEventGuideNext, "Click here to go to the next step in the guide.")
        Me.btnEventGuideNext.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Gray
        Me.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Panel2.Controls.Add(Me.lblEventGuideInstructions)
        Me.Panel2.Location = New System.Drawing.Point(84, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(586, 89)
        Me.Panel2.TabIndex = 81
        '
        'lblEventGuideInstructions
        '
        Me.lblEventGuideInstructions.Font = New System.Drawing.Font("Segoe UI Variable Display Semib", 13.74545!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEventGuideInstructions.ForeColor = System.Drawing.Color.White
        Me.lblEventGuideInstructions.Location = New System.Drawing.Point(-1, 0)
        Me.lblEventGuideInstructions.Name = "lblEventGuideInstructions"
        Me.lblEventGuideInstructions.Size = New System.Drawing.Size(584, 89)
        Me.lblEventGuideInstructions.TabIndex = 0
        Me.lblEventGuideInstructions.Text = "Click the ""Flight Plan"" button and select the flight plan to use for this task."
        Me.lblEventGuideInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlEventArrow
        '
        Me.pnlEventArrow.BackColor = System.Drawing.Color.Gray
        Me.pnlEventArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        Me.pnlEventArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlEventArrow.Location = New System.Drawing.Point(667, 0)
        Me.pnlEventArrow.Name = "pnlEventArrow"
        Me.pnlEventArrow.Size = New System.Drawing.Size(91, 89)
        Me.pnlEventArrow.TabIndex = 80
        '
        'grpGroupEventPost
        '
        Me.grpGroupEventPost.Controls.Add(Me.lblLocalDSTWarning)
        Me.grpGroupEventPost.Controls.Add(Me.chkIncludeGotGravelInvite)
        Me.grpGroupEventPost.Controls.Add(Me.Label48)
        Me.grpGroupEventPost.Controls.Add(Me.lblEventTaskDistance)
        Me.grpGroupEventPost.Controls.Add(Me.btnTaskFPURLPaste)
        Me.grpGroupEventPost.Controls.Add(Me.cboGroupOrClubName)
        Me.grpGroupEventPost.Controls.Add(Me.txtEventTitle)
        Me.grpGroupEventPost.Controls.Add(Me.Label41)
        Me.grpGroupEventPost.Controls.Add(Me.txtTaskFlightPlanURL)
        Me.grpGroupEventPost.Controls.Add(Me.Label37)
        Me.grpGroupEventPost.Controls.Add(Me.cboEligibleAward)
        Me.grpGroupEventPost.Controls.Add(Me.Label36)
        Me.grpGroupEventPost.Controls.Add(Me.Label35)
        Me.grpGroupEventPost.Controls.Add(Me.Label34)
        Me.grpGroupEventPost.Controls.Add(Me.cboVoiceChannel)
        Me.grpGroupEventPost.Controls.Add(Me.cboMSFSServer)
        Me.grpGroupEventPost.Controls.Add(Me.Label33)
        Me.grpGroupEventPost.Controls.Add(Me.Label32)
        Me.grpGroupEventPost.Controls.Add(Me.txtEventDescription)
        Me.grpGroupEventPost.Controls.Add(Me.lblStartTimeResult)
        Me.grpGroupEventPost.Controls.Add(Me.lblLaunchTimeResult)
        Me.grpGroupEventPost.Controls.Add(Me.lblSyncTimeResult)
        Me.grpGroupEventPost.Controls.Add(Me.lblMeetTimeResult)
        Me.grpGroupEventPost.Controls.Add(Me.chkUseStart)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventStartTaskTime)
        Me.grpGroupEventPost.Controls.Add(Me.Label29)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventStartTaskDate)
        Me.grpGroupEventPost.Controls.Add(Me.chkUseLaunch)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventLaunchTime)
        Me.grpGroupEventPost.Controls.Add(Me.Label28)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventLaunchDate)
        Me.grpGroupEventPost.Controls.Add(Me.chkUseSyncFly)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventSyncFlyTime)
        Me.grpGroupEventPost.Controls.Add(Me.Label27)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventSyncFlyDate)
        Me.grpGroupEventPost.Controls.Add(Me.Label25)
        Me.grpGroupEventPost.Controls.Add(Me.chkDateTimeUTC)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventMeetTime)
        Me.grpGroupEventPost.Controls.Add(Me.Label26)
        Me.grpGroupEventPost.Controls.Add(Me.dtEventMeetDate)
        Me.grpGroupEventPost.Controls.Add(Me.Label24)
        Me.grpGroupEventPost.Enabled = False
        Me.grpGroupEventPost.Location = New System.Drawing.Point(6, 6)
        Me.grpGroupEventPost.Name = "grpGroupEventPost"
        Me.grpGroupEventPost.Size = New System.Drawing.Size(848, 667)
        Me.grpGroupEventPost.TabIndex = 0
        Me.grpGroupEventPost.TabStop = False
        '
        'lblLocalDSTWarning
        '
        Me.lblLocalDSTWarning.AutoSize = True
        Me.lblLocalDSTWarning.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLocalDSTWarning.Location = New System.Drawing.Point(579, 236)
        Me.lblLocalDSTWarning.Name = "lblLocalDSTWarning"
        Me.lblLocalDSTWarning.Size = New System.Drawing.Size(217, 26)
        Me.lblLocalDSTWarning.TabIndex = 51
        Me.lblLocalDSTWarning.Text = "⚠️Local DST in effect⚠️"
        Me.lblLocalDSTWarning.Visible = False
        '
        'chkIncludeGotGravelInvite
        '
        Me.chkIncludeGotGravelInvite.AutoSize = True
        Me.chkIncludeGotGravelInvite.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkIncludeGotGravelInvite.Location = New System.Drawing.Point(192, 622)
        Me.chkIncludeGotGravelInvite.Name = "chkIncludeGotGravelInvite"
        Me.chkIncludeGotGravelInvite.Size = New System.Drawing.Size(435, 30)
        Me.chkIncludeGotGravelInvite.TabIndex = 50
        Me.chkIncludeGotGravelInvite.Tag = "39"
        Me.chkIncludeGotGravelInvite.Text = "Include the GotGravel server invite with the post."
        Me.ToolTip1.SetToolTip(Me.chkIncludeGotGravelInvite, "When checked, the invite to the GotGravel Server will be added to the post.")
        Me.chkIncludeGotGravelInvite.UseVisualStyleBackColor = True
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label48.Location = New System.Drawing.Point(7, 28)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(609, 26)
        Me.Label48.TabIndex = 29
        Me.Label48.Text = "On the Flight Plan tab, please load the event's flight plan and weather file."
        '
        'lblEventTaskDistance
        '
        Me.lblEventTaskDistance.AutoSize = True
        Me.lblEventTaskDistance.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEventTaskDistance.Location = New System.Drawing.Point(364, 555)
        Me.lblEventTaskDistance.Name = "lblEventTaskDistance"
        Me.lblEventTaskDistance.Size = New System.Drawing.Size(53, 26)
        Me.lblEventTaskDistance.TabIndex = 44
        Me.lblEventTaskDistance.Text = "0 Km"
        Me.lblEventTaskDistance.Visible = False
        '
        'btnTaskFPURLPaste
        '
        Me.btnTaskFPURLPaste.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTaskFPURLPaste.Location = New System.Drawing.Point(763, 587)
        Me.btnTaskFPURLPaste.Name = "btnTaskFPURLPaste"
        Me.btnTaskFPURLPaste.Size = New System.Drawing.Size(79, 29)
        Me.btnTaskFPURLPaste.TabIndex = 49
        Me.btnTaskFPURLPaste.Tag = "38"
        Me.btnTaskFPURLPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnTaskFPURLPaste, "Click this button to paste the task's flight plan URL from your clipboard")
        Me.btnTaskFPURLPaste.UseVisualStyleBackColor = True
        '
        'cboGroupOrClubName
        '
        Me.cboGroupOrClubName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboGroupOrClubName.FormattingEnabled = True
        Me.cboGroupOrClubName.Items.AddRange(New Object() {"TSC", "FSC", "SSC Saturday", "Aus Tuesdays", "DTS"})
        Me.cboGroupOrClubName.Location = New System.Drawing.Point(192, 95)
        Me.cboGroupOrClubName.Name = "cboGroupOrClubName"
        Me.cboGroupOrClubName.Size = New System.Drawing.Size(650, 32)
        Me.cboGroupOrClubName.TabIndex = 1
        Me.cboGroupOrClubName.Tag = "27"
        Me.ToolTip1.SetToolTip(Me.cboGroupOrClubName, "Select or specify the group or club name related to this event. Leave blank if no" &
        "ne.")
        '
        'txtEventTitle
        '
        Me.txtEventTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventTitle.Location = New System.Drawing.Point(192, 131)
        Me.txtEventTitle.Name = "txtEventTitle"
        Me.txtEventTitle.Size = New System.Drawing.Size(650, 32)
        Me.txtEventTitle.TabIndex = 3
        Me.txtEventTitle.Tag = "28"
        Me.ToolTip1.SetToolTip(Me.txtEventTitle, "Specify the event title (leave blank if none) - comes from the flight plan (title" &
        ") tab if created in the same session.")
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label41.Location = New System.Drawing.Point(7, 135)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(157, 26)
        Me.Label41.TabIndex = 2
        Me.Label41.Text = "Event Title / Topic"
        '
        'txtTaskFlightPlanURL
        '
        Me.txtTaskFlightPlanURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTaskFlightPlanURL.Location = New System.Drawing.Point(192, 588)
        Me.txtTaskFlightPlanURL.Name = "txtTaskFlightPlanURL"
        Me.txtTaskFlightPlanURL.Size = New System.Drawing.Size(565, 32)
        Me.txtTaskFlightPlanURL.TabIndex = 48
        Me.txtTaskFlightPlanURL.Tag = "38"
        Me.ToolTip1.SetToolTip(Me.txtTaskFlightPlanURL, "Enter the URL to the Discord post containing the related task's flight plan.")
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label37.Location = New System.Drawing.Point(7, 592)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(191, 26)
        Me.Label37.TabIndex = 47
        Me.Label37.Text = "URL to task flight plan"
        '
        'cboEligibleAward
        '
        Me.cboEligibleAward.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEligibleAward.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboEligibleAward.FormattingEnabled = True
        Me.cboEligibleAward.Items.AddRange(New Object() {"None", "Bronze", "Silver", "Gold", "Diamond"})
        Me.cboEligibleAward.Location = New System.Drawing.Point(192, 552)
        Me.cboEligibleAward.Name = "cboEligibleAward"
        Me.cboEligibleAward.Size = New System.Drawing.Size(166, 32)
        Me.cboEligibleAward.TabIndex = 43
        Me.cboEligibleAward.Tag = "37"
        Me.ToolTip1.SetToolTip(Me.cboEligibleAward, "Select any eligible award for completing this task succesfully during the event.")
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label36.Location = New System.Drawing.Point(7, 555)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(177, 26)
        Me.Label36.TabIndex = 42
        Me.Label36.Text = "Eligible Award (SSC)"
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label35.Location = New System.Drawing.Point(7, 52)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(670, 26)
        Me.Label35.TabIndex = 45
        Me.Label35.Text = "Then also fill out the Sim local Date and Time, Duration fields and Credits (if a" &
    "ny)."
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label34.Location = New System.Drawing.Point(7, 204)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(128, 26)
        Me.Label34.TabIndex = 6
        Me.Label34.Text = "Voice channel"
        '
        'cboVoiceChannel
        '
        Me.cboVoiceChannel.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboVoiceChannel.FormattingEnabled = True
        Me.cboVoiceChannel.Items.AddRange(New Object() {"Unicom 1", "Unicom 2", "Unicom 3", "Push to talk 1", "Sim Soaring Club (PTT)", "Flight 01", "Flight 02", "Thermal Smashing"})
        Me.cboVoiceChannel.Location = New System.Drawing.Point(192, 201)
        Me.cboVoiceChannel.Name = "cboVoiceChannel"
        Me.cboVoiceChannel.Size = New System.Drawing.Size(650, 32)
        Me.cboVoiceChannel.TabIndex = 7
        Me.cboVoiceChannel.Tag = "30"
        Me.ToolTip1.SetToolTip(Me.cboVoiceChannel, "Select the voice channel to use for the event (from the list or enter your own).")
        '
        'cboMSFSServer
        '
        Me.cboMSFSServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMSFSServer.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboMSFSServer.FormattingEnabled = True
        Me.cboMSFSServer.Items.AddRange(New Object() {"West Europe", "North Europe", "West USA", "East USA", "Southeast Asia"})
        Me.cboMSFSServer.Location = New System.Drawing.Point(192, 165)
        Me.cboMSFSServer.Name = "cboMSFSServer"
        Me.cboMSFSServer.Size = New System.Drawing.Size(200, 32)
        Me.cboMSFSServer.TabIndex = 5
        Me.cboMSFSServer.Tag = "29"
        Me.ToolTip1.SetToolTip(Me.cboMSFSServer, "Select the MSFS Server to use for the event.")
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label33.Location = New System.Drawing.Point(7, 168)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(174, 26)
        Me.Label33.TabIndex = 4
        Me.Label33.Text = "MSFS Server to use"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label32.Location = New System.Drawing.Point(7, 411)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(109, 26)
        Me.Label32.TabIndex = 40
        Me.Label32.Text = "Description"
        '
        'txtEventDescription
        '
        Me.txtEventDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventDescription.Location = New System.Drawing.Point(192, 407)
        Me.txtEventDescription.Multiline = True
        Me.txtEventDescription.Name = "txtEventDescription"
        Me.txtEventDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtEventDescription.Size = New System.Drawing.Size(650, 139)
        Me.txtEventDescription.TabIndex = 41
        Me.txtEventDescription.Tag = "36"
        Me.ToolTip1.SetToolTip(Me.txtEventDescription, "Short description of the flight - comes from the flight plan tab if created in th" &
        "e same session.")
        '
        'lblStartTimeResult
        '
        Me.lblStartTimeResult.AutoSize = True
        Me.lblStartTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStartTimeResult.Location = New System.Drawing.Point(516, 378)
        Me.lblStartTimeResult.Name = "lblStartTimeResult"
        Me.lblStartTimeResult.Size = New System.Drawing.Size(153, 26)
        Me.lblStartTimeResult.TabIndex = 28
        Me.lblStartTimeResult.Text = "start time results"
        '
        'lblLaunchTimeResult
        '
        Me.lblLaunchTimeResult.AutoSize = True
        Me.lblLaunchTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLaunchTimeResult.Location = New System.Drawing.Point(516, 344)
        Me.lblLaunchTimeResult.Name = "lblLaunchTimeResult"
        Me.lblLaunchTimeResult.Size = New System.Drawing.Size(170, 26)
        Me.lblLaunchTimeResult.TabIndex = 23
        Me.lblLaunchTimeResult.Text = "launch time results"
        '
        'lblSyncTimeResult
        '
        Me.lblSyncTimeResult.AutoSize = True
        Me.lblSyncTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSyncTimeResult.Location = New System.Drawing.Point(516, 310)
        Me.lblSyncTimeResult.Name = "lblSyncTimeResult"
        Me.lblSyncTimeResult.Size = New System.Drawing.Size(154, 26)
        Me.lblSyncTimeResult.TabIndex = 18
        Me.lblSyncTimeResult.Text = "sync time results"
        '
        'lblMeetTimeResult
        '
        Me.lblMeetTimeResult.AutoSize = True
        Me.lblMeetTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMeetTimeResult.Location = New System.Drawing.Point(516, 276)
        Me.lblMeetTimeResult.Name = "lblMeetTimeResult"
        Me.lblMeetTimeResult.Size = New System.Drawing.Size(157, 26)
        Me.lblMeetTimeResult.TabIndex = 13
        Me.lblMeetTimeResult.Text = "meet time results"
        '
        'chkUseStart
        '
        Me.chkUseStart.AutoSize = True
        Me.chkUseStart.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseStart.Location = New System.Drawing.Point(125, 376)
        Me.chkUseStart.Name = "chkUseStart"
        Me.chkUseStart.Size = New System.Drawing.Size(59, 30)
        Me.chkUseStart.TabIndex = 25
        Me.chkUseStart.Tag = "35"
        Me.chkUseStart.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseStart, "When checked, a task start time will be specified.")
        Me.chkUseStart.UseVisualStyleBackColor = True
        '
        'dtEventStartTaskTime
        '
        Me.dtEventStartTaskTime.CustomFormat = "HH:mm tt"
        Me.dtEventStartTaskTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventStartTaskTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventStartTaskTime.Location = New System.Drawing.Point(398, 373)
        Me.dtEventStartTaskTime.Name = "dtEventStartTaskTime"
        Me.dtEventStartTaskTime.ShowUpDown = True
        Me.dtEventStartTaskTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventStartTaskTime.TabIndex = 27
        Me.dtEventStartTaskTime.Tag = "35"
        Me.ToolTip1.SetToolTip(Me.dtEventStartTaskTime, "This is the event's task start time in the specified time zone above.")
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label29.Location = New System.Drawing.Point(7, 378)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(90, 26)
        Me.Label29.TabIndex = 24
        Me.Label29.Text = "Start task"
        '
        'dtEventStartTaskDate
        '
        Me.dtEventStartTaskDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventStartTaskDate.Location = New System.Drawing.Point(192, 373)
        Me.dtEventStartTaskDate.Name = "dtEventStartTaskDate"
        Me.dtEventStartTaskDate.Size = New System.Drawing.Size(200, 31)
        Me.dtEventStartTaskDate.TabIndex = 26
        Me.dtEventStartTaskDate.Tag = "35"
        Me.ToolTip1.SetToolTip(Me.dtEventStartTaskDate, "This is the event's task start date in the specified time zone above.")
        '
        'chkUseLaunch
        '
        Me.chkUseLaunch.AutoSize = True
        Me.chkUseLaunch.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseLaunch.Location = New System.Drawing.Point(125, 342)
        Me.chkUseLaunch.Name = "chkUseLaunch"
        Me.chkUseLaunch.Size = New System.Drawing.Size(59, 30)
        Me.chkUseLaunch.TabIndex = 20
        Me.chkUseLaunch.Tag = "34"
        Me.chkUseLaunch.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseLaunch, "When checked, a launch time will be specified.")
        Me.chkUseLaunch.UseVisualStyleBackColor = True
        '
        'dtEventLaunchTime
        '
        Me.dtEventLaunchTime.Cursor = System.Windows.Forms.Cursors.Default
        Me.dtEventLaunchTime.CustomFormat = "HH:mm tt"
        Me.dtEventLaunchTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventLaunchTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventLaunchTime.Location = New System.Drawing.Point(398, 339)
        Me.dtEventLaunchTime.Name = "dtEventLaunchTime"
        Me.dtEventLaunchTime.ShowUpDown = True
        Me.dtEventLaunchTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventLaunchTime.TabIndex = 22
        Me.dtEventLaunchTime.Tag = "34"
        Me.ToolTip1.SetToolTip(Me.dtEventLaunchTime, "This is the event's glider launch time in the specified time zone above.")
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label28.Location = New System.Drawing.Point(7, 344)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(73, 26)
        Me.Label28.TabIndex = 19
        Me.Label28.Text = "Launch"
        '
        'dtEventLaunchDate
        '
        Me.dtEventLaunchDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventLaunchDate.Location = New System.Drawing.Point(192, 339)
        Me.dtEventLaunchDate.Name = "dtEventLaunchDate"
        Me.dtEventLaunchDate.Size = New System.Drawing.Size(200, 31)
        Me.dtEventLaunchDate.TabIndex = 21
        Me.dtEventLaunchDate.Tag = "34"
        Me.ToolTip1.SetToolTip(Me.dtEventLaunchDate, "This is the event's glider launch date in the specified time zone above.")
        '
        'chkUseSyncFly
        '
        Me.chkUseSyncFly.AutoSize = True
        Me.chkUseSyncFly.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseSyncFly.Location = New System.Drawing.Point(125, 308)
        Me.chkUseSyncFly.Name = "chkUseSyncFly"
        Me.chkUseSyncFly.Size = New System.Drawing.Size(59, 30)
        Me.chkUseSyncFly.TabIndex = 15
        Me.chkUseSyncFly.Tag = "33"
        Me.chkUseSyncFly.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseSyncFly, "When checked, a synchronized ""Click Fly"" will be specified.")
        Me.chkUseSyncFly.UseVisualStyleBackColor = True
        '
        'dtEventSyncFlyTime
        '
        Me.dtEventSyncFlyTime.CustomFormat = "HH:mm tt"
        Me.dtEventSyncFlyTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventSyncFlyTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventSyncFlyTime.Location = New System.Drawing.Point(398, 305)
        Me.dtEventSyncFlyTime.Name = "dtEventSyncFlyTime"
        Me.dtEventSyncFlyTime.ShowUpDown = True
        Me.dtEventSyncFlyTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventSyncFlyTime.TabIndex = 17
        Me.dtEventSyncFlyTime.Tag = "33"
        Me.ToolTip1.SetToolTip(Me.dtEventSyncFlyTime, "This is the event's synchronized ""Click Fly"" time in the specified time zone abov" &
        "e.")
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label27.Location = New System.Drawing.Point(7, 310)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(80, 26)
        Me.Label27.TabIndex = 14
        Me.Label27.Text = "Sync Fly"
        '
        'dtEventSyncFlyDate
        '
        Me.dtEventSyncFlyDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventSyncFlyDate.Location = New System.Drawing.Point(192, 305)
        Me.dtEventSyncFlyDate.Name = "dtEventSyncFlyDate"
        Me.dtEventSyncFlyDate.Size = New System.Drawing.Size(200, 31)
        Me.dtEventSyncFlyDate.TabIndex = 16
        Me.dtEventSyncFlyDate.Tag = "33"
        Me.ToolTip1.SetToolTip(Me.dtEventSyncFlyDate, "This is the event's synchronized ""Click Fly"" date in the specified time zone abov" &
        "e.")
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(7, 239)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(155, 26)
        Me.Label25.TabIndex = 8
        Me.Label25.Text = "UTC/Zulu or local"
        '
        'chkDateTimeUTC
        '
        Me.chkDateTimeUTC.AutoSize = True
        Me.chkDateTimeUTC.Checked = True
        Me.chkDateTimeUTC.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDateTimeUTC.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkDateTimeUTC.Location = New System.Drawing.Point(192, 237)
        Me.chkDateTimeUTC.Name = "chkDateTimeUTC"
        Me.chkDateTimeUTC.Size = New System.Drawing.Size(355, 30)
        Me.chkDateTimeUTC.TabIndex = 9
        Me.chkDateTimeUTC.Tag = "31"
        Me.chkDateTimeUTC.Text = "UTC / Zulu (local time if left unchecked)"
        Me.ToolTip1.SetToolTip(Me.chkDateTimeUTC, "When checked, the specified date and time are considered as UTC or Zulu.")
        Me.chkDateTimeUTC.UseVisualStyleBackColor = True
        '
        'dtEventMeetTime
        '
        Me.dtEventMeetTime.CustomFormat = "HH:mm tt"
        Me.dtEventMeetTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventMeetTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventMeetTime.Location = New System.Drawing.Point(398, 271)
        Me.dtEventMeetTime.Name = "dtEventMeetTime"
        Me.dtEventMeetTime.ShowUpDown = True
        Me.dtEventMeetTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventMeetTime.TabIndex = 12
        Me.dtEventMeetTime.Tag = "32"
        Me.ToolTip1.SetToolTip(Me.dtEventMeetTime, "This is the event's meet time in the specified time zone above.")
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(7, 276)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(136, 26)
        Me.Label26.TabIndex = 10
        Me.Label26.Text = "Meet / Briefing"
        '
        'dtEventMeetDate
        '
        Me.dtEventMeetDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventMeetDate.Location = New System.Drawing.Point(192, 271)
        Me.dtEventMeetDate.Name = "dtEventMeetDate"
        Me.dtEventMeetDate.Size = New System.Drawing.Size(200, 31)
        Me.dtEventMeetDate.TabIndex = 11
        Me.dtEventMeetDate.Tag = "32"
        Me.ToolTip1.SetToolTip(Me.dtEventMeetDate, "This is the event's meet date in the specified time zone above.")
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.Location = New System.Drawing.Point(7, 98)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(183, 26)
        Me.Label24.TabIndex = 0
        Me.Label24.Text = "Group or Club Name"
        '
        'tabBriefing
        '
        Me.tabBriefing.Controls.Add(Me.pnlBriefing)
        Me.tabBriefing.Controls.Add(Me.Label20)
        Me.tabBriefing.Controls.Add(Me.cboBriefingMap)
        Me.tabBriefing.Location = New System.Drawing.Point(4, 29)
        Me.tabBriefing.Name = "tabBriefing"
        Me.tabBriefing.Size = New System.Drawing.Size(1479, 874)
        Me.tabBriefing.TabIndex = 2
        Me.tabBriefing.Text = "Briefing"
        Me.tabBriefing.UseVisualStyleBackColor = True
        '
        'pnlBriefing
        '
        Me.pnlBriefing.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlBriefing.Controls.Add(Me.BriefingControl1)
        Me.pnlBriefing.Location = New System.Drawing.Point(3, 43)
        Me.pnlBriefing.Name = "pnlBriefing"
        Me.pnlBriefing.Size = New System.Drawing.Size(1480, 835)
        Me.pnlBriefing.TabIndex = 0
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(20, 12)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(227, 20)
        Me.Label20.TabIndex = 86
        Me.Label20.Text = "Select image for the map display:"
        '
        'cboBriefingMap
        '
        Me.cboBriefingMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBriefingMap.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.cboBriefingMap.FormattingEnabled = True
        Me.cboBriefingMap.Location = New System.Drawing.Point(274, 9)
        Me.cboBriefingMap.Name = "cboBriefingMap"
        Me.cboBriefingMap.Size = New System.Drawing.Size(524, 28)
        Me.cboBriefingMap.TabIndex = 85
        Me.ToolTip1.SetToolTip(Me.cboBriefingMap, "Select the image for the map display")
        Me.cboBriefingMap.Visible = False
        '
        'tabDiscord
        '
        Me.tabDiscord.Controls.Add(Me.txtAddOnsDetails)
        Me.tabDiscord.Controls.Add(Me.txtWaypointsDetails)
        Me.tabDiscord.Controls.Add(Me.lblNbrCarsRestrictions)
        Me.tabDiscord.Controls.Add(Me.txtGroupFlightEventPost)
        Me.tabDiscord.Controls.Add(Me.grpDiscordTask)
        Me.tabDiscord.Controls.Add(Me.txtDiscordEventDescription)
        Me.tabDiscord.Controls.Add(Me.txtDiscordEventTopic)
        Me.tabDiscord.Controls.Add(Me.lblNbrCarsWeatherClouds)
        Me.tabDiscord.Controls.Add(Me.txtFullDescriptionResults)
        Me.tabDiscord.Controls.Add(Me.txtWeatherFirstPart)
        Me.tabDiscord.Controls.Add(Me.txtFilesText)
        Me.tabDiscord.Controls.Add(Me.txtWeatherWinds)
        Me.tabDiscord.Controls.Add(Me.txtFPResults)
        Me.tabDiscord.Controls.Add(Me.txtWeatherClouds)
        Me.tabDiscord.Controls.Add(Me.lblNbrCarsWeatherInfo)
        Me.tabDiscord.Controls.Add(Me.txtAltRestrictions)
        Me.tabDiscord.Controls.Add(Me.lblNbrCarsWeatherWinds)
        Me.tabDiscord.Controls.Add(Me.lblNbrCarsFilesText)
        Me.tabDiscord.Controls.Add(Me.grpDiscordGroupFlight)
        Me.tabDiscord.Location = New System.Drawing.Point(4, 29)
        Me.tabDiscord.Name = "tabDiscord"
        Me.tabDiscord.Size = New System.Drawing.Size(1479, 874)
        Me.tabDiscord.TabIndex = 3
        Me.tabDiscord.Text = "Discord"
        Me.tabDiscord.UseVisualStyleBackColor = True
        '
        'txtAddOnsDetails
        '
        Me.txtAddOnsDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddOnsDetails.Location = New System.Drawing.Point(1141, 246)
        Me.txtAddOnsDetails.Multiline = True
        Me.txtAddOnsDetails.Name = "txtAddOnsDetails"
        Me.txtAddOnsDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtAddOnsDetails.Size = New System.Drawing.Size(59, 23)
        Me.txtAddOnsDetails.TabIndex = 93
        Me.txtAddOnsDetails.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtAddOnsDetails, "This is the full description content for the fourth and last Discord post.")
        '
        'txtWaypointsDetails
        '
        Me.txtWaypointsDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWaypointsDetails.Location = New System.Drawing.Point(1141, 217)
        Me.txtWaypointsDetails.Multiline = True
        Me.txtWaypointsDetails.Name = "txtWaypointsDetails"
        Me.txtWaypointsDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWaypointsDetails.Size = New System.Drawing.Size(59, 23)
        Me.txtWaypointsDetails.TabIndex = 92
        Me.txtWaypointsDetails.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtWaypointsDetails, "This is the full description content for the fourth and last Discord post.")
        '
        'lblNbrCarsRestrictions
        '
        Me.lblNbrCarsRestrictions.AutoSize = True
        Me.lblNbrCarsRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsRestrictions.Location = New System.Drawing.Point(1219, 43)
        Me.lblNbrCarsRestrictions.Name = "lblNbrCarsRestrictions"
        Me.lblNbrCarsRestrictions.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsRestrictions.TabIndex = 71
        Me.lblNbrCarsRestrictions.Text = "0"
        '
        'txtGroupFlightEventPost
        '
        Me.txtGroupFlightEventPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGroupFlightEventPost.Location = New System.Drawing.Point(1141, 295)
        Me.txtGroupFlightEventPost.Multiline = True
        Me.txtGroupFlightEventPost.Name = "txtGroupFlightEventPost"
        Me.txtGroupFlightEventPost.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtGroupFlightEventPost.Size = New System.Drawing.Size(59, 23)
        Me.txtGroupFlightEventPost.TabIndex = 2
        Me.txtGroupFlightEventPost.Tag = "41"
        Me.ToolTip1.SetToolTip(Me.txtGroupFlightEventPost, "This is the content of the Discord group flight event post.")
        '
        'grpDiscordTask
        '
        Me.grpDiscordTask.Controls.Add(Me.grpDiscordTaskThread)
        Me.grpDiscordTask.Controls.Add(Me.GroupBox1)
        Me.grpDiscordTask.Location = New System.Drawing.Point(8, 3)
        Me.grpDiscordTask.Name = "grpDiscordTask"
        Me.grpDiscordTask.Size = New System.Drawing.Size(405, 728)
        Me.grpDiscordTask.TabIndex = 0
        Me.grpDiscordTask.TabStop = False
        Me.grpDiscordTask.Text = "Task"
        '
        'grpDiscordTaskThread
        '
        Me.grpDiscordTaskThread.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpDiscordTaskThread.Controls.Add(Me.FlowLayoutPanel1)
        Me.grpDiscordTaskThread.Location = New System.Drawing.Point(6, 137)
        Me.grpDiscordTaskThread.Name = "grpDiscordTaskThread"
        Me.grpDiscordTaskThread.Size = New System.Drawing.Size(393, 583)
        Me.grpDiscordTaskThread.TabIndex = 1
        Me.grpDiscordTaskThread.TabStop = False
        Me.grpDiscordTaskThread.Text = "Thread"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnFilesCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnFilesTextCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.chkGroupSecondaryPosts)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCopyAllSecPosts)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblAllSecPostsTotalCars)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnAltRestricCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblRestrictWeatherTotalCars)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnFullDescriptionCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblNbrCarsFullDescResults)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnWaypointsCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblWaypointsTotalCars)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnAddOnsCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblAddOnsTotalCars)
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 26)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(393, 557)
        Me.FlowLayoutPanel1.TabIndex = 82
        '
        'btnFilesCopy
        '
        Me.btnFilesCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFilesCopy.Location = New System.Drawing.Point(3, 3)
        Me.btnFilesCopy.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.btnFilesCopy.Name = "btnFilesCopy"
        Me.btnFilesCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnFilesCopy.TabIndex = 0
        Me.btnFilesCopy.Tag = "23"
        Me.btnFilesCopy.Text = "2a. Files to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFilesCopy, "Click this button to put the included files into the clipboard.")
        Me.btnFilesCopy.UseVisualStyleBackColor = True
        '
        'btnFilesTextCopy
        '
        Me.btnFilesTextCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFilesTextCopy.Location = New System.Drawing.Point(3, 57)
        Me.btnFilesTextCopy.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.btnFilesTextCopy.Name = "btnFilesTextCopy"
        Me.btnFilesTextCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnFilesTextCopy.TabIndex = 1
        Me.btnFilesTextCopy.Tag = "24"
        Me.btnFilesTextCopy.Text = "2b. Files info to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFilesTextCopy, "Click this button to put the second post content into the clipboard.")
        Me.btnFilesTextCopy.UseVisualStyleBackColor = True
        '
        'chkGroupSecondaryPosts
        '
        Me.chkGroupSecondaryPosts.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkGroupSecondaryPosts.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkGroupSecondaryPosts.Location = New System.Drawing.Point(3, 111)
        Me.chkGroupSecondaryPosts.Name = "chkGroupSecondaryPosts"
        Me.chkGroupSecondaryPosts.Size = New System.Drawing.Size(384, 28)
        Me.chkGroupSecondaryPosts.TabIndex = 2
        Me.chkGroupSecondaryPosts.Tag = ""
        Me.chkGroupSecondaryPosts.Text = "Group remaining posts"
        Me.ToolTip1.SetToolTip(Me.chkGroupSecondaryPosts, "Check this to group all secondary posts into only one.")
        Me.chkGroupSecondaryPosts.UseVisualStyleBackColor = True
        '
        'btnCopyAllSecPosts
        '
        Me.btnCopyAllSecPosts.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCopyAllSecPosts.Location = New System.Drawing.Point(3, 145)
        Me.btnCopyAllSecPosts.Name = "btnCopyAllSecPosts"
        Me.btnCopyAllSecPosts.Size = New System.Drawing.Size(384, 51)
        Me.btnCopyAllSecPosts.TabIndex = 3
        Me.btnCopyAllSecPosts.Tag = "22"
        Me.btnCopyAllSecPosts.Text = "3. All secondary post's content to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnCopyAllSecPosts, "Click this button to put all remaining content into the clipboard.")
        Me.btnCopyAllSecPosts.UseVisualStyleBackColor = True
        Me.btnCopyAllSecPosts.Visible = False
        '
        'lblAllSecPostsTotalCars
        '
        Me.lblAllSecPostsTotalCars.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAllSecPostsTotalCars.ForeColor = System.Drawing.Color.Red
        Me.lblAllSecPostsTotalCars.Location = New System.Drawing.Point(3, 199)
        Me.lblAllSecPostsTotalCars.Name = "lblAllSecPostsTotalCars"
        Me.lblAllSecPostsTotalCars.Size = New System.Drawing.Size(384, 26)
        Me.lblAllSecPostsTotalCars.TabIndex = 4
        Me.lblAllSecPostsTotalCars.Text = "0"
        Me.lblAllSecPostsTotalCars.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblAllSecPostsTotalCars, "Caution! Approaching Discord limit!")
        '
        'btnAltRestricCopy
        '
        Me.btnAltRestricCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAltRestricCopy.Location = New System.Drawing.Point(3, 228)
        Me.btnAltRestricCopy.Name = "btnAltRestricCopy"
        Me.btnAltRestricCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnAltRestricCopy.TabIndex = 5
        Me.btnAltRestricCopy.Tag = "22"
        Me.btnAltRestricCopy.Text = "3. Restrictions and Weather to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnAltRestricCopy, "Click this button to put the restrictions and weather post content into the clipb" &
        "oard.")
        Me.btnAltRestricCopy.UseVisualStyleBackColor = True
        '
        'lblRestrictWeatherTotalCars
        '
        Me.lblRestrictWeatherTotalCars.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRestrictWeatherTotalCars.ForeColor = System.Drawing.Color.Red
        Me.lblRestrictWeatherTotalCars.Location = New System.Drawing.Point(3, 282)
        Me.lblRestrictWeatherTotalCars.Name = "lblRestrictWeatherTotalCars"
        Me.lblRestrictWeatherTotalCars.Size = New System.Drawing.Size(384, 26)
        Me.lblRestrictWeatherTotalCars.TabIndex = 6
        Me.lblRestrictWeatherTotalCars.Text = "0"
        Me.lblRestrictWeatherTotalCars.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblRestrictWeatherTotalCars, "Caution! Approaching Discord limit!")
        '
        'btnFullDescriptionCopy
        '
        Me.btnFullDescriptionCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFullDescriptionCopy.Location = New System.Drawing.Point(3, 311)
        Me.btnFullDescriptionCopy.Name = "btnFullDescriptionCopy"
        Me.btnFullDescriptionCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnFullDescriptionCopy.TabIndex = 7
        Me.btnFullDescriptionCopy.Tag = "25"
        Me.btnFullDescriptionCopy.Text = "4. Full Description to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFullDescriptionCopy, "Click this button to put the task's full description post content into the clipbo" &
        "ard.")
        Me.btnFullDescriptionCopy.UseVisualStyleBackColor = True
        '
        'lblNbrCarsFullDescResults
        '
        Me.lblNbrCarsFullDescResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsFullDescResults.ForeColor = System.Drawing.Color.Red
        Me.lblNbrCarsFullDescResults.Location = New System.Drawing.Point(3, 365)
        Me.lblNbrCarsFullDescResults.Name = "lblNbrCarsFullDescResults"
        Me.lblNbrCarsFullDescResults.Size = New System.Drawing.Size(384, 26)
        Me.lblNbrCarsFullDescResults.TabIndex = 8
        Me.lblNbrCarsFullDescResults.Text = "0"
        Me.lblNbrCarsFullDescResults.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblNbrCarsFullDescResults, "Caution! Approaching Discord limit!")
        '
        'btnWaypointsCopy
        '
        Me.btnWaypointsCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypointsCopy.Location = New System.Drawing.Point(3, 394)
        Me.btnWaypointsCopy.Name = "btnWaypointsCopy"
        Me.btnWaypointsCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnWaypointsCopy.TabIndex = 9
        Me.btnWaypointsCopy.Tag = "25"
        Me.btnWaypointsCopy.Text = "5. Waypoint details to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnWaypointsCopy, "Click this button to put the waypoints post content into the clipboard.")
        Me.btnWaypointsCopy.UseVisualStyleBackColor = True
        '
        'lblWaypointsTotalCars
        '
        Me.lblWaypointsTotalCars.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWaypointsTotalCars.ForeColor = System.Drawing.Color.Red
        Me.lblWaypointsTotalCars.Location = New System.Drawing.Point(3, 448)
        Me.lblWaypointsTotalCars.Name = "lblWaypointsTotalCars"
        Me.lblWaypointsTotalCars.Size = New System.Drawing.Size(384, 26)
        Me.lblWaypointsTotalCars.TabIndex = 10
        Me.lblWaypointsTotalCars.Text = "0"
        Me.lblWaypointsTotalCars.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblWaypointsTotalCars, "Caution! Approaching Discord limit!")
        '
        'btnAddOnsCopy
        '
        Me.btnAddOnsCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddOnsCopy.Location = New System.Drawing.Point(3, 477)
        Me.btnAddOnsCopy.Name = "btnAddOnsCopy"
        Me.btnAddOnsCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnAddOnsCopy.TabIndex = 11
        Me.btnAddOnsCopy.Tag = "25"
        Me.btnAddOnsCopy.Text = "6. Add-ons details to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnAddOnsCopy, "Click this button to put the add-ons post content into the clipboard.")
        Me.btnAddOnsCopy.UseVisualStyleBackColor = True
        '
        'lblAddOnsTotalCars
        '
        Me.lblAddOnsTotalCars.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAddOnsTotalCars.ForeColor = System.Drawing.Color.Red
        Me.lblAddOnsTotalCars.Location = New System.Drawing.Point(3, 531)
        Me.lblAddOnsTotalCars.Name = "lblAddOnsTotalCars"
        Me.lblAddOnsTotalCars.Size = New System.Drawing.Size(384, 26)
        Me.lblAddOnsTotalCars.TabIndex = 12
        Me.lblAddOnsTotalCars.Text = "0"
        Me.lblAddOnsTotalCars.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblAddOnsTotalCars, "Caution! Approaching Discord limit!")
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnFPMainInfoCopy)
        Me.GroupBox1.Controls.Add(Me.lblNbrCarsMainFP)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 27)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(393, 104)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Main Post"
        '
        'btnFPMainInfoCopy
        '
        Me.btnFPMainInfoCopy.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFPMainInfoCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFPMainInfoCopy.Location = New System.Drawing.Point(6, 26)
        Me.btnFPMainInfoCopy.Name = "btnFPMainInfoCopy"
        Me.btnFPMainInfoCopy.Size = New System.Drawing.Size(381, 51)
        Me.btnFPMainInfoCopy.TabIndex = 0
        Me.btnFPMainInfoCopy.Tag = "21"
        Me.btnFPMainInfoCopy.Text = "1. Main FP post to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFPMainInfoCopy, "Click this button to put the first post content into the clipboard.")
        Me.btnFPMainInfoCopy.UseVisualStyleBackColor = True
        '
        'lblNbrCarsMainFP
        '
        Me.lblNbrCarsMainFP.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblNbrCarsMainFP.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsMainFP.ForeColor = System.Drawing.Color.Red
        Me.lblNbrCarsMainFP.Location = New System.Drawing.Point(0, 77)
        Me.lblNbrCarsMainFP.Name = "lblNbrCarsMainFP"
        Me.lblNbrCarsMainFP.Size = New System.Drawing.Size(393, 26)
        Me.lblNbrCarsMainFP.TabIndex = 1
        Me.lblNbrCarsMainFP.Text = "0"
        Me.lblNbrCarsMainFP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblNbrCarsMainFP, "Caution! Approaching Discord limit!")
        '
        'txtDiscordEventDescription
        '
        Me.txtDiscordEventDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordEventDescription.Location = New System.Drawing.Point(1271, 295)
        Me.txtDiscordEventDescription.Multiline = True
        Me.txtDiscordEventDescription.Name = "txtDiscordEventDescription"
        Me.txtDiscordEventDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDiscordEventDescription.Size = New System.Drawing.Size(59, 23)
        Me.txtDiscordEventDescription.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.txtDiscordEventDescription, "This is the content of the Discord Event description field.")
        '
        'txtDiscordEventTopic
        '
        Me.txtDiscordEventTopic.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordEventTopic.Location = New System.Drawing.Point(1206, 291)
        Me.txtDiscordEventTopic.Name = "txtDiscordEventTopic"
        Me.txtDiscordEventTopic.Size = New System.Drawing.Size(59, 32)
        Me.txtDiscordEventTopic.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.txtDiscordEventTopic, "Specify the event title (leave blank if none) - comes from the flight plan (title" &
        ") tab if created in the same session.")
        '
        'lblNbrCarsWeatherClouds
        '
        Me.lblNbrCarsWeatherClouds.AutoSize = True
        Me.lblNbrCarsWeatherClouds.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsWeatherClouds.Location = New System.Drawing.Point(1219, 127)
        Me.lblNbrCarsWeatherClouds.Name = "lblNbrCarsWeatherClouds"
        Me.lblNbrCarsWeatherClouds.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsWeatherClouds.TabIndex = 14
        Me.lblNbrCarsWeatherClouds.Text = "0"
        '
        'txtFullDescriptionResults
        '
        Me.txtFullDescriptionResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFullDescriptionResults.Location = New System.Drawing.Point(1141, 188)
        Me.txtFullDescriptionResults.Multiline = True
        Me.txtFullDescriptionResults.Name = "txtFullDescriptionResults"
        Me.txtFullDescriptionResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtFullDescriptionResults.Size = New System.Drawing.Size(59, 23)
        Me.txtFullDescriptionResults.TabIndex = 86
        Me.txtFullDescriptionResults.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtFullDescriptionResults, "This is the full description content for the fourth and last Discord post.")
        '
        'txtWeatherFirstPart
        '
        Me.txtWeatherFirstPart.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherFirstPart.Location = New System.Drawing.Point(1141, 75)
        Me.txtWeatherFirstPart.Multiline = True
        Me.txtWeatherFirstPart.Name = "txtWeatherFirstPart"
        Me.txtWeatherFirstPart.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWeatherFirstPart.Size = New System.Drawing.Size(59, 23)
        Me.txtWeatherFirstPart.TabIndex = 1
        Me.txtWeatherFirstPart.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtWeatherFirstPart, "This is the basic weather content for the second Discord post.")
        '
        'txtFilesText
        '
        Me.txtFilesText.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFilesText.Location = New System.Drawing.Point(1141, 162)
        Me.txtFilesText.Multiline = True
        Me.txtFilesText.Name = "txtFilesText"
        Me.txtFilesText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtFilesText.Size = New System.Drawing.Size(59, 23)
        Me.txtFilesText.TabIndex = 83
        Me.txtFilesText.Tag = "23"
        Me.ToolTip1.SetToolTip(Me.txtFilesText, "This is the files content for the third Discord post.")
        '
        'txtWeatherWinds
        '
        Me.txtWeatherWinds.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherWinds.Location = New System.Drawing.Point(1141, 104)
        Me.txtWeatherWinds.Multiline = True
        Me.txtWeatherWinds.Name = "txtWeatherWinds"
        Me.txtWeatherWinds.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWeatherWinds.Size = New System.Drawing.Size(59, 23)
        Me.txtWeatherWinds.TabIndex = 2
        Me.txtWeatherWinds.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtWeatherWinds, "This is the wind layers content for the second Discord post.")
        '
        'txtFPResults
        '
        Me.txtFPResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFPResults.Location = New System.Drawing.Point(1141, 16)
        Me.txtFPResults.Multiline = True
        Me.txtFPResults.Name = "txtFPResults"
        Me.txtFPResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtFPResults.Size = New System.Drawing.Size(59, 23)
        Me.txtFPResults.TabIndex = 79
        Me.txtFPResults.Tag = "21"
        Me.ToolTip1.SetToolTip(Me.txtFPResults, "This is the content of the main Discord post for the flight plan.")
        '
        'txtWeatherClouds
        '
        Me.txtWeatherClouds.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherClouds.Location = New System.Drawing.Point(1141, 133)
        Me.txtWeatherClouds.Multiline = True
        Me.txtWeatherClouds.Name = "txtWeatherClouds"
        Me.txtWeatherClouds.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWeatherClouds.Size = New System.Drawing.Size(59, 23)
        Me.txtWeatherClouds.TabIndex = 3
        Me.txtWeatherClouds.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtWeatherClouds, "This is the cloud layers content for the second Discord post.")
        '
        'lblNbrCarsWeatherInfo
        '
        Me.lblNbrCarsWeatherInfo.AutoSize = True
        Me.lblNbrCarsWeatherInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsWeatherInfo.Location = New System.Drawing.Point(1219, 72)
        Me.lblNbrCarsWeatherInfo.Name = "lblNbrCarsWeatherInfo"
        Me.lblNbrCarsWeatherInfo.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsWeatherInfo.TabIndex = 11
        Me.lblNbrCarsWeatherInfo.Text = "0"
        '
        'txtAltRestrictions
        '
        Me.txtAltRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAltRestrictions.Location = New System.Drawing.Point(1141, 46)
        Me.txtAltRestrictions.Multiline = True
        Me.txtAltRestrictions.Name = "txtAltRestrictions"
        Me.txtAltRestrictions.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtAltRestrictions.Size = New System.Drawing.Size(59, 23)
        Me.txtAltRestrictions.TabIndex = 0
        Me.txtAltRestrictions.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtAltRestrictions, "This is the altitude restrictions content for the second Discord post.")
        '
        'lblNbrCarsWeatherWinds
        '
        Me.lblNbrCarsWeatherWinds.AutoSize = True
        Me.lblNbrCarsWeatherWinds.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsWeatherWinds.Location = New System.Drawing.Point(1219, 101)
        Me.lblNbrCarsWeatherWinds.Name = "lblNbrCarsWeatherWinds"
        Me.lblNbrCarsWeatherWinds.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsWeatherWinds.TabIndex = 12
        Me.lblNbrCarsWeatherWinds.Text = "0"
        '
        'lblNbrCarsFilesText
        '
        Me.lblNbrCarsFilesText.AutoSize = True
        Me.lblNbrCarsFilesText.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsFilesText.Location = New System.Drawing.Point(1219, 156)
        Me.lblNbrCarsFilesText.Name = "lblNbrCarsFilesText"
        Me.lblNbrCarsFilesText.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsFilesText.TabIndex = 76
        Me.lblNbrCarsFilesText.Text = "0"
        '
        'grpDiscordGroupFlight
        '
        Me.grpDiscordGroupFlight.Controls.Add(Me.grpGroupFlightEvent)
        Me.grpDiscordGroupFlight.Controls.Add(Me.grpDiscordEvent)
        Me.grpDiscordGroupFlight.Location = New System.Drawing.Point(419, 3)
        Me.grpDiscordGroupFlight.Name = "grpDiscordGroupFlight"
        Me.grpDiscordGroupFlight.Size = New System.Drawing.Size(685, 561)
        Me.grpDiscordGroupFlight.TabIndex = 1
        Me.grpDiscordGroupFlight.TabStop = False
        Me.grpDiscordGroupFlight.Text = "Group Flight Event"
        '
        'grpGroupFlightEvent
        '
        Me.grpGroupFlightEvent.Controls.Add(Me.btnCopyReqFilesToClipboard)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnGroupFlightEventInfoToClipboard)
        Me.grpGroupFlightEvent.Location = New System.Drawing.Point(6, 27)
        Me.grpGroupFlightEvent.Name = "grpGroupFlightEvent"
        Me.grpGroupFlightEvent.Size = New System.Drawing.Size(673, 147)
        Me.grpGroupFlightEvent.TabIndex = 0
        Me.grpGroupFlightEvent.TabStop = False
        Me.grpGroupFlightEvent.Text = "Step 1: Group Event Post"
        '
        'btnCopyReqFilesToClipboard
        '
        Me.btnCopyReqFilesToClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCopyReqFilesToClipboard.Location = New System.Drawing.Point(6, 26)
        Me.btnCopyReqFilesToClipboard.Name = "btnCopyReqFilesToClipboard"
        Me.btnCopyReqFilesToClipboard.Size = New System.Drawing.Size(661, 51)
        Me.btnCopyReqFilesToClipboard.TabIndex = 0
        Me.btnCopyReqFilesToClipboard.Tag = "40"
        Me.btnCopyReqFilesToClipboard.Text = "Optional - Copy required files to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnCopyReqFilesToClipboard, "If you want to include the required files with the group flight post, click this " &
        "button.")
        Me.btnCopyReqFilesToClipboard.UseVisualStyleBackColor = True
        '
        'btnGroupFlightEventInfoToClipboard
        '
        Me.btnGroupFlightEventInfoToClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGroupFlightEventInfoToClipboard.Location = New System.Drawing.Point(6, 83)
        Me.btnGroupFlightEventInfoToClipboard.Name = "btnGroupFlightEventInfoToClipboard"
        Me.btnGroupFlightEventInfoToClipboard.Size = New System.Drawing.Size(661, 51)
        Me.btnGroupFlightEventInfoToClipboard.TabIndex = 1
        Me.btnGroupFlightEventInfoToClipboard.Tag = "41"
        Me.btnGroupFlightEventInfoToClipboard.Text = "1. Group Flight post info to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnGroupFlightEventInfoToClipboard, "Click this button to put the copy the group flight info to your clipboard.")
        Me.btnGroupFlightEventInfoToClipboard.UseVisualStyleBackColor = True
        '
        'grpDiscordEvent
        '
        Me.grpDiscordEvent.Controls.Add(Me.Label6)
        Me.grpDiscordEvent.Controls.Add(Me.btnDiscordGroupEventURL)
        Me.grpDiscordEvent.Controls.Add(Me.Label46)
        Me.grpDiscordEvent.Controls.Add(Me.Label45)
        Me.grpDiscordEvent.Controls.Add(Me.btnEventDescriptionToClipboard)
        Me.grpDiscordEvent.Controls.Add(Me.Label44)
        Me.grpDiscordEvent.Controls.Add(Me.lblDiscordPostDateTime)
        Me.grpDiscordEvent.Controls.Add(Me.Label43)
        Me.grpDiscordEvent.Controls.Add(Me.btnEventTopicClipboard)
        Me.grpDiscordEvent.Controls.Add(Me.Label42)
        Me.grpDiscordEvent.Controls.Add(Me.lblDiscordEventVoice)
        Me.grpDiscordEvent.Controls.Add(Me.Label39)
        Me.grpDiscordEvent.Controls.Add(Me.txtGroupEventPostURL)
        Me.grpDiscordEvent.Controls.Add(Me.Label38)
        Me.grpDiscordEvent.Location = New System.Drawing.Point(6, 180)
        Me.grpDiscordEvent.Name = "grpDiscordEvent"
        Me.grpDiscordEvent.Size = New System.Drawing.Size(673, 371)
        Me.grpDiscordEvent.TabIndex = 1
        Me.grpDiscordEvent.TabStop = False
        Me.grpDiscordEvent.Text = "Step 2: Discord Event (if applicable)"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(7, 85)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(429, 26)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "1. Create a new Discord Event on the proper server"
        '
        'btnDiscordGroupEventURL
        '
        Me.btnDiscordGroupEventURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordGroupEventURL.Location = New System.Drawing.Point(577, 34)
        Me.btnDiscordGroupEventURL.Name = "btnDiscordGroupEventURL"
        Me.btnDiscordGroupEventURL.Size = New System.Drawing.Size(79, 29)
        Me.btnDiscordGroupEventURL.TabIndex = 2
        Me.btnDiscordGroupEventURL.Tag = "43"
        Me.btnDiscordGroupEventURL.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnDiscordGroupEventURL, "Click this button to paste the group event's post URL from your clipboard")
        Me.btnDiscordGroupEventURL.UseVisualStyleBackColor = True
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label46.Location = New System.Drawing.Point(7, 338)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(231, 26)
        Me.Label46.TabIndex = 13
        Me.Label46.Text = "7. Preview and post event !"
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label45.Location = New System.Drawing.Point(6, 294)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(521, 26)
        Me.Label45.TabIndex = 12
        Me.Label45.Text = "6. Upload optional cover image (min. 800px wide by 320px tall)"
        '
        'btnEventDescriptionToClipboard
        '
        Me.btnEventDescriptionToClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventDescriptionToClipboard.Location = New System.Drawing.Point(234, 250)
        Me.btnEventDescriptionToClipboard.Name = "btnEventDescriptionToClipboard"
        Me.btnEventDescriptionToClipboard.Size = New System.Drawing.Size(337, 29)
        Me.btnEventDescriptionToClipboard.TabIndex = 11
        Me.btnEventDescriptionToClipboard.Tag = "48"
        Me.btnEventDescriptionToClipboard.Text = "Event Description to Clipboard"
        Me.ToolTip1.SetToolTip(Me.btnEventDescriptionToClipboard, "Click this button to copy the event's full description for the Discord Event post" &
        " into the clipboard.")
        Me.btnEventDescriptionToClipboard.UseVisualStyleBackColor = True
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label44.Location = New System.Drawing.Point(7, 252)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(225, 26)
        Me.Label44.TabIndex = 10
        Me.Label44.Text = "5. Enter Event Description"
        '
        'lblDiscordPostDateTime
        '
        Me.lblDiscordPostDateTime.AutoSize = True
        Me.lblDiscordPostDateTime.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordPostDateTime.Location = New System.Drawing.Point(307, 209)
        Me.lblDiscordPostDateTime.Name = "lblDiscordPostDateTime"
        Me.lblDiscordPostDateTime.Size = New System.Drawing.Size(157, 26)
        Me.lblDiscordPostDateTime.TabIndex = 9
        Me.lblDiscordPostDateTime.Text = "meet time results"
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label43.Location = New System.Drawing.Point(7, 209)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(298, 26)
        Me.Label43.TabIndex = 8
        Me.Label43.Text = "4. Specify local start date and time:"
        '
        'btnEventTopicClipboard
        '
        Me.btnEventTopicClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventTopicClipboard.Location = New System.Drawing.Point(234, 167)
        Me.btnEventTopicClipboard.Name = "btnEventTopicClipboard"
        Me.btnEventTopicClipboard.Size = New System.Drawing.Size(337, 29)
        Me.btnEventTopicClipboard.TabIndex = 7
        Me.btnEventTopicClipboard.Tag = "46"
        Me.btnEventTopicClipboard.Text = "Event Topic to Clipboard"
        Me.ToolTip1.SetToolTip(Me.btnEventTopicClipboard, "Click this button to copy the event's topic for the Discord Event post into the c" &
        "lipboard.")
        Me.btnEventTopicClipboard.UseVisualStyleBackColor = True
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label42.Location = New System.Drawing.Point(7, 169)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(172, 26)
        Me.Label42.TabIndex = 6
        Me.Label42.Text = "3. Enter Event Topic"
        '
        'lblDiscordEventVoice
        '
        Me.lblDiscordEventVoice.AutoSize = True
        Me.lblDiscordEventVoice.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordEventVoice.Location = New System.Drawing.Point(350, 128)
        Me.lblDiscordEventVoice.Name = "lblDiscordEventVoice"
        Me.lblDiscordEventVoice.Size = New System.Drawing.Size(126, 26)
        Me.lblDiscordEventVoice.TabIndex = 5
        Me.lblDiscordEventVoice.Text = "voice channel"
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label39.Location = New System.Drawing.Point(7, 128)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(326, 26)
        Me.Label39.TabIndex = 4
        Me.Label39.Text = "2. Select Voice Channel and click next:"
        '
        'txtGroupEventPostURL
        '
        Me.txtGroupEventPostURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGroupEventPostURL.Location = New System.Drawing.Point(192, 33)
        Me.txtGroupEventPostURL.Name = "txtGroupEventPostURL"
        Me.txtGroupEventPostURL.Size = New System.Drawing.Size(379, 32)
        Me.txtGroupEventPostURL.TabIndex = 1
        Me.txtGroupEventPostURL.Tag = "43"
        Me.ToolTip1.SetToolTip(Me.txtGroupEventPostURL, "Enter the URL to the Discord post created above in step 1.")
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label38.Location = New System.Drawing.Point(7, 37)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(173, 26)
        Me.Label38.TabIndex = 0
        Me.Label38.Text = "URL to group event"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnReset
        '
        Me.btnReset.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReset.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReset.Location = New System.Drawing.Point(503, 3)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(118, 35)
        Me.btnReset.TabIndex = 1
        Me.btnReset.Text = "Reset All"
        Me.ToolTip1.SetToolTip(Me.btnReset, "Click to reset ALL of the fiels and start from scratch.")
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'btnLoadConfig
        '
        Me.btnLoadConfig.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnLoadConfig.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLoadConfig.Location = New System.Drawing.Point(627, 3)
        Me.btnLoadConfig.Name = "btnLoadConfig"
        Me.btnLoadConfig.Size = New System.Drawing.Size(118, 35)
        Me.btnLoadConfig.TabIndex = 2
        Me.btnLoadConfig.Tag = "19"
        Me.btnLoadConfig.Text = "Load"
        Me.ToolTip1.SetToolTip(Me.btnLoadConfig, "Click to select and load a configuration file from your PC.")
        Me.btnLoadConfig.UseVisualStyleBackColor = True
        '
        'btnSaveConfig
        '
        Me.btnSaveConfig.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSaveConfig.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveConfig.Location = New System.Drawing.Point(751, 3)
        Me.btnSaveConfig.Name = "btnSaveConfig"
        Me.btnSaveConfig.Size = New System.Drawing.Size(118, 35)
        Me.btnSaveConfig.TabIndex = 3
        Me.btnSaveConfig.Tag = "19"
        Me.btnSaveConfig.Text = "Save"
        Me.ToolTip1.SetToolTip(Me.btnSaveConfig, "Click to save the current configuration to your PC.")
        Me.btnSaveConfig.UseVisualStyleBackColor = True
        '
        'btnCreateShareablePack
        '
        Me.btnCreateShareablePack.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreateShareablePack.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateShareablePack.Location = New System.Drawing.Point(875, 3)
        Me.btnCreateShareablePack.Name = "btnCreateShareablePack"
        Me.btnCreateShareablePack.Size = New System.Drawing.Size(135, 35)
        Me.btnCreateShareablePack.TabIndex = 4
        Me.btnCreateShareablePack.Tag = "20"
        Me.btnCreateShareablePack.Text = "Share package"
        Me.ToolTip1.SetToolTip(Me.btnCreateShareablePack, "Click to create a shareable package with all files.")
        Me.btnCreateShareablePack.UseVisualStyleBackColor = True
        '
        'btnLoadB21Planner
        '
        Me.btnLoadB21Planner.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnLoadB21Planner.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLoadB21Planner.Location = New System.Drawing.Point(1016, 3)
        Me.btnLoadB21Planner.Name = "btnLoadB21Planner"
        Me.btnLoadB21Planner.Size = New System.Drawing.Size(152, 35)
        Me.btnLoadB21Planner.TabIndex = 5
        Me.btnLoadB21Planner.Text = "Open B21 Planner"
        Me.ToolTip1.SetToolTip(Me.btnLoadB21Planner, "Click to open the B21 Planner in your browser.")
        Me.btnLoadB21Planner.UseVisualStyleBackColor = True
        '
        'btnGuideMe
        '
        Me.btnGuideMe.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnGuideMe.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGuideMe.Location = New System.Drawing.Point(1175, 3)
        Me.btnGuideMe.Name = "btnGuideMe"
        Me.btnGuideMe.Size = New System.Drawing.Size(155, 35)
        Me.btnGuideMe.TabIndex = 6
        Me.btnGuideMe.Text = "Guide me please!"
        Me.ToolTip1.SetToolTip(Me.btnGuideMe, "Click to reset ALL of the fiels and start from scratch.")
        Me.btnGuideMe.UseVisualStyleBackColor = True
        '
        'btnTurnGuideOff
        '
        Me.btnTurnGuideOff.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnTurnGuideOff.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTurnGuideOff.Location = New System.Drawing.Point(1336, 3)
        Me.btnTurnGuideOff.Name = "btnTurnGuideOff"
        Me.btnTurnGuideOff.Size = New System.Drawing.Size(139, 35)
        Me.btnTurnGuideOff.TabIndex = 83
        Me.btnTurnGuideOff.Text = "Turn guide off"
        Me.ToolTip1.SetToolTip(Me.btnTurnGuideOff, "Click to reset ALL of the fiels and start from scratch.")
        Me.btnTurnGuideOff.UseVisualStyleBackColor = True
        Me.btnTurnGuideOff.Visible = False
        '
        'BriefingControl1
        '
        Me.BriefingControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BriefingControl1.EventIsEnabled = False
        Me.BriefingControl1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.BriefingControl1.Location = New System.Drawing.Point(0, 0)
        Me.BriefingControl1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BriefingControl1.MinimumSize = New System.Drawing.Size(700, 500)
        Me.BriefingControl1.Name = "BriefingControl1"
        Me.BriefingControl1.Size = New System.Drawing.Size(1480, 835)
        Me.BriefingControl1.TabIndex = 0
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1487, 915)
        Me.Controls.Add(Me.btnTurnGuideOff)
        Me.Controls.Add(Me.btnGuideMe)
        Me.Controls.Add(Me.btnLoadB21Planner)
        Me.Controls.Add(Me.btnCreateShareablePack)
        Me.Controls.Add(Me.btnSaveConfig)
        Me.Controls.Add(Me.btnLoadConfig)
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.pnlScrollableSurface)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(1366, 768)
        Me.Name = "Main"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Discord Post Helper"
        Me.pnlScrollableSurface.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.tabFlightPlan.ResumeLayout(False)
        Me.tabFlightPlan.PerformLayout()
        Me.pnlGuide.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.grbTaskInfo.ResumeLayout(False)
        Me.grbTaskInfo.PerformLayout()
        Me.grpTaskPart2.ResumeLayout(False)
        Me.grpTaskPart2.PerformLayout()
        Me.tabEvent.ResumeLayout(False)
        Me.tabEvent.PerformLayout()
        Me.pnlWizardEvent.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.grpGroupEventPost.ResumeLayout(False)
        Me.grpGroupEventPost.PerformLayout()
        Me.tabBriefing.ResumeLayout(False)
        Me.tabBriefing.PerformLayout()
        Me.pnlBriefing.ResumeLayout(False)
        Me.tabDiscord.ResumeLayout(False)
        Me.tabDiscord.PerformLayout()
        Me.grpDiscordTask.ResumeLayout(False)
        Me.grpDiscordTaskThread.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.grpDiscordGroupFlight.ResumeLayout(False)
        Me.grpGroupFlightEvent.ResumeLayout(False)
        Me.grpDiscordEvent.ResumeLayout(False)
        Me.grpDiscordEvent.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tabFlightPlan As TabPage
    Friend WithEvents grbTaskInfo As GroupBox
    Friend WithEvents Label9 As Label
    Friend WithEvents chkTitleLock As CheckBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents chkArrivalLock As CheckBox
    Friend WithEvents chkDepartureLock As CheckBox
    Friend WithEvents chkSoaringTypeThermal As CheckBox
    Friend WithEvents chkSoaringTypeRidge As CheckBox
    Friend WithEvents txtSoaringTypeExtraInfo As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtArrivalExtraInfo As TextBox
    Friend WithEvents txtArrivalName As TextBox
    Friend WithEvents txtArrivalICAO As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtDepExtraInfo As TextBox
    Friend WithEvents txtDepName As TextBox
    Friend WithEvents txtSimDateTimeExtraInfo As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents dtSimLocalTime As DateTimePicker
    Friend WithEvents chkIncludeYear As CheckBox
    Friend WithEvents Label4 As Label
    Friend WithEvents dtSimDate As DateTimePicker
    Friend WithEvents txtDepartureICAO As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtMainArea As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtTitle As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents cboSpeedUnits As ComboBox
    Friend WithEvents txtMinAvgSpeed As TextBox
    Friend WithEvents Label21 As Label
    Friend WithEvents btnSelectFlightPlan As Button
    Friend WithEvents txtMaxAvgSpeed As TextBox
    Friend WithEvents txtFlightPlanFile As TextBox
    Friend WithEvents Label22 As Label
    Friend WithEvents btnSelectWeatherFile As Button
    Friend WithEvents txtWeatherFile As TextBox
    Friend WithEvents txtDurationMin As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents txtDurationMax As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtDurationExtraInfo As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents txtCredits As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents lblTotalDistanceAndMiles As Label
    Friend WithEvents lblTrackDistanceAndMiles As Label
    Friend WithEvents cboDifficulty As ComboBox
    Friend WithEvents txtDistanceTotal As TextBox
    Friend WithEvents txtDistanceTrack As TextBox
    Friend WithEvents cboRecommendedGliders As ComboBox
    Friend WithEvents txtDifficultyExtraInfo As TextBox
    Friend WithEvents chkDescriptionLock As CheckBox
    Friend WithEvents Label16 As Label
    Friend WithEvents txtShortDescription As TextBox
    Friend WithEvents Label17 As Label
    Friend WithEvents txtLongDescription As TextBox
    Friend WithEvents tabEvent As TabPage
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents btnReset As Button
    Friend WithEvents grpGroupEventPost As GroupBox
    Friend WithEvents dtEventMeetTime As DateTimePicker
    Friend WithEvents Label26 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents chkDateTimeUTC As CheckBox
    Friend WithEvents Label25 As Label
    Friend WithEvents chkUseStart As CheckBox
    Friend WithEvents dtEventStartTaskTime As DateTimePicker
    Friend WithEvents Label29 As Label
    Friend WithEvents dtEventStartTaskDate As DateTimePicker
    Friend WithEvents chkUseLaunch As CheckBox
    Friend WithEvents dtEventLaunchTime As DateTimePicker
    Friend WithEvents Label28 As Label
    Friend WithEvents dtEventLaunchDate As DateTimePicker
    Friend WithEvents dtEventSyncFlyTime As DateTimePicker
    Friend WithEvents Label27 As Label
    Friend WithEvents dtEventSyncFlyDate As DateTimePicker
    Friend WithEvents lblStartTimeResult As Label
    Friend WithEvents lblLaunchTimeResult As Label
    Friend WithEvents lblSyncTimeResult As Label
    Friend WithEvents lblMeetTimeResult As Label
    Friend WithEvents Label32 As Label
    Friend WithEvents txtEventDescription As TextBox
    Friend WithEvents cboMSFSServer As ComboBox
    Friend WithEvents Label33 As Label
    Friend WithEvents Label34 As Label
    Friend WithEvents cboVoiceChannel As ComboBox
    Friend WithEvents Label35 As Label
    Friend WithEvents cboEligibleAward As ComboBox
    Friend WithEvents Label36 As Label
    Friend WithEvents txtTaskFlightPlanURL As TextBox
    Friend WithEvents Label37 As Label
    Friend WithEvents grpDiscordEvent As GroupBox
    Friend WithEvents txtGroupEventPostURL As TextBox
    Friend WithEvents Label38 As Label
    Friend WithEvents lblDiscordEventVoice As Label
    Friend WithEvents Label39 As Label
    Friend WithEvents txtEventTitle As TextBox
    Friend WithEvents Label41 As Label
    Friend WithEvents cboGroupOrClubName As ComboBox
    Friend WithEvents Label42 As Label
    Friend WithEvents Label43 As Label
    Friend WithEvents btnEventTopicClipboard As Button
    Friend WithEvents btnEventDescriptionToClipboard As Button
    Friend WithEvents Label44 As Label
    Friend WithEvents lblDiscordPostDateTime As Label
    Friend WithEvents Label45 As Label
    Friend WithEvents Label46 As Label
    Friend WithEvents btnDiscordGroupEventURL As Button
    Friend WithEvents btnTaskFPURLPaste As Button
    Friend WithEvents btnGroupFlightEventInfoToClipboard As Button
    Friend WithEvents txtGroupFlightEventPost As TextBox
    Friend WithEvents lblEventTaskDistance As Label
    Friend WithEvents Label48 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents txtDiscordEventDescription As TextBox
    Friend WithEvents txtDiscordEventTopic As TextBox
    Friend WithEvents btnLoadConfig As Button
    Friend WithEvents btnSaveConfig As Button
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents btnCreateShareablePack As Button
    Friend WithEvents chkIncludeGotGravelInvite As CheckBox
    Friend WithEvents btnLoadB21Planner As Button
    Friend WithEvents btnCopyReqFilesToClipboard As Button
    Friend WithEvents btnGuideMe As Button
    Friend WithEvents pnlGuide As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents lblGuideInstructions As Label
    Friend WithEvents pnlArrow As Panel
    Friend WithEvents btnGuideNext As Button
    Friend WithEvents btnTurnGuideOff As Button
    Friend WithEvents pnlWizardEvent As Panel
    Friend WithEvents btnEventGuideNext As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblEventGuideInstructions As Label
    Friend WithEvents pnlEventArrow As Panel
    Friend WithEvents dtEventMeetDate As DateTimePicker
    Friend WithEvents chkUseSyncFly As CheckBox
    Friend WithEvents tabBriefing As TabPage
    Friend WithEvents cboBriefingMap As ComboBox
    Friend WithEvents lblLocalDSTWarning As Label
    Friend WithEvents pnlBriefing As Panel
    Friend WithEvents BriefingControl1 As CommonLibrary.BriefingControl
    Friend WithEvents Label20 As Label
    Friend WithEvents pnlScrollableSurface As Panel
    Friend WithEvents chkActivateEvent As CheckBox
    Friend WithEvents tabDiscord As TabPage
    Friend WithEvents chkGroupSecondaryPosts As CheckBox
    Friend WithEvents btnFilesCopy As Button
    Friend WithEvents btnFullDescriptionCopy As Button
    Friend WithEvents btnFilesTextCopy As Button
    Friend WithEvents lblNbrCarsFullDescResults As Label
    Friend WithEvents txtFullDescriptionResults As TextBox
    Friend WithEvents txtFilesText As TextBox
    Friend WithEvents lblNbrCarsMainFP As Label
    Friend WithEvents txtFPResults As TextBox
    Friend WithEvents lblNbrCarsRestrictions As Label
    Friend WithEvents btnCopyAllSecPosts As Button
    Friend WithEvents lblNbrCarsWeatherClouds As Label
    Friend WithEvents lblRestrictWeatherTotalCars As Label
    Friend WithEvents txtWeatherFirstPart As TextBox
    Friend WithEvents txtWeatherWinds As TextBox
    Friend WithEvents txtWeatherClouds As TextBox
    Friend WithEvents lblNbrCarsWeatherInfo As Label
    Friend WithEvents lblNbrCarsWeatherWinds As Label
    Friend WithEvents txtAltRestrictions As TextBox
    Friend WithEvents lblNbrCarsFilesText As Label
    Friend WithEvents btnAltRestricCopy As Button
    Friend WithEvents grpTaskPart2 As GroupBox
    Friend WithEvents chkLockCountries As CheckBox
    Friend WithEvents btnMoveCountryDown As Button
    Friend WithEvents btnMoveCountryUp As Button
    Friend WithEvents btnRemoveCountry As Button
    Friend WithEvents btnAddCountry As Button
    Friend WithEvents lstAllCountries As ListBox
    Friend WithEvents cboCountryFlag As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents btnExtraFileDown As Button
    Friend WithEvents btnExtraFileUp As Button
    Friend WithEvents btnRemoveExtraFile As Button
    Friend WithEvents btnAddExtraFile As Button
    Friend WithEvents lstAllFiles As ListBox
    Friend WithEvents chkUseOnlyWeatherSummary As CheckBox
    Friend WithEvents txtWeatherSummary As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnFPMainInfoCopy As Button
    Friend WithEvents grpDiscordTask As GroupBox
    Friend WithEvents grpDiscordTaskThread As GroupBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents btnWaypointsCopy As Button
    Friend WithEvents btnAddOnsCopy As Button
    Friend WithEvents lblAllSecPostsTotalCars As Label
    Friend WithEvents lblWaypointsTotalCars As Label
    Friend WithEvents lblAddOnsTotalCars As Label
    Friend WithEvents grpDiscordGroupFlight As GroupBox
    Friend WithEvents grpGroupFlightEvent As GroupBox
    Friend WithEvents txtWaypointsDetails As TextBox
    Friend WithEvents txtAddOnsDetails As TextBox
End Class
