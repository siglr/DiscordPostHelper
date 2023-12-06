Imports SIGLR.SoaringTools.CommonLibrary

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
        Me.lblElevationUpdateWarning = New System.Windows.Forms.Label()
        Me.pnlGuide = New System.Windows.Forms.Panel()
        Me.btnGuideNext = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlArrow = New System.Windows.Forms.Panel()
        Me.grbTaskInfo = New System.Windows.Forms.GroupBox()
        Me.chkSoaringTypeDynamic = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeWave = New System.Windows.Forms.CheckBox()
        Me.btnPasteUsernameCredits = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.chkTitleLock = New System.Windows.Forms.CheckBox()
        Me.chkArrivalLock = New System.Windows.Forms.CheckBox()
        Me.chkDepartureLock = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeThermal = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeRidge = New System.Windows.Forms.CheckBox()
        Me.txtSoaringTypeExtraInfo = New System.Windows.Forms.TextBox()
        Me.lblSoaringType = New System.Windows.Forms.Label()
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
        Me.lblDeparture = New System.Windows.Forms.Label()
        Me.txtMainArea = New System.Windows.Forms.TextBox()
        Me.lblMainAreaPOI = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.cboSpeedUnits = New System.Windows.Forms.ComboBox()
        Me.txtMinAvgSpeed = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txtMaxAvgSpeed = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.btnSelectWeatherFile = New System.Windows.Forms.Button()
        Me.txtWeatherFile = New System.Windows.Forms.TextBox()
        Me.txtDurationMin = New System.Windows.Forms.TextBox()
        Me.lblDuration = New System.Windows.Forms.Label()
        Me.txtDurationMax = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtDurationExtraInfo = New System.Windows.Forms.TextBox()
        Me.lblRecommendedGliders = New System.Windows.Forms.Label()
        Me.txtCredits = New System.Windows.Forms.TextBox()
        Me.lblDifficultyRating = New System.Windows.Forms.Label()
        Me.lblCredits = New System.Windows.Forms.Label()
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
        Me.grbTaskPart2 = New System.Windows.Forms.GroupBox()
        Me.chkSuppressWarningForBaroPressure = New System.Windows.Forms.CheckBox()
        Me.txtBaroPressureExtraInfo = New System.Windows.Forms.TextBox()
        Me.lblNonStdBaroPressure = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.chkLockCoverImage = New System.Windows.Forms.CheckBox()
        Me.chkLockMapImage = New System.Windows.Forms.CheckBox()
        Me.cboCoverImage = New System.Windows.Forms.ComboBox()
        Me.lblMap = New System.Windows.Forms.Label()
        Me.cboBriefingMap = New System.Windows.Forms.ComboBox()
        Me.lstAllFiles = New System.Windows.Forms.ListBox()
        Me.btnAddExtraFile = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.btnRemoveExtraFile = New System.Windows.Forms.Button()
        Me.btnExtraFileUp = New System.Windows.Forms.Button()
        Me.btnExtraFileDown = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnRemoveSelectedAddOns = New System.Windows.Forms.Button()
        Me.lstAllRecommendedAddOns = New System.Windows.Forms.ListBox()
        Me.btnAddOnDown = New System.Windows.Forms.Button()
        Me.btnAddRecAddOn = New System.Windows.Forms.Button()
        Me.btnAddOnUp = New System.Windows.Forms.Button()
        Me.btnEditSelectedAddOn = New System.Windows.Forms.Button()
        Me.chkLockCountries = New System.Windows.Forms.CheckBox()
        Me.btnMoveCountryDown = New System.Windows.Forms.Button()
        Me.btnMoveCountryUp = New System.Windows.Forms.Button()
        Me.txtDPHXPackageFilename = New System.Windows.Forms.TextBox()
        Me.btnRemoveCountry = New System.Windows.Forms.Button()
        Me.btnAddCountry = New System.Windows.Forms.Button()
        Me.lstAllCountries = New System.Windows.Forms.ListBox()
        Me.cboCountryFlag = New System.Windows.Forms.ComboBox()
        Me.lblCountries = New System.Windows.Forms.Label()
        Me.chkUseOnlyWeatherSummary = New System.Windows.Forms.CheckBox()
        Me.txtWeatherSummary = New System.Windows.Forms.TextBox()
        Me.lblWeatherSummary = New System.Windows.Forms.Label()
        Me.grbTaskDiscord = New System.Windows.Forms.GroupBox()
        Me.btnDeleteDiscordID = New System.Windows.Forms.Button()
        Me.btnDiscordTaskThreadURLPaste = New System.Windows.Forms.Button()
        Me.txtDiscordTaskID = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.FileDropZone1 = New SIGLR.SoaringTools.CommonLibrary.FileDropZone()
        Me.tabEvent = New System.Windows.Forms.TabPage()
        Me.chkActivateEvent = New System.Windows.Forms.CheckBox()
        Me.pnlWizardEvent = New System.Windows.Forms.Panel()
        Me.btnEventGuideNext = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblEventGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlEventArrow = New System.Windows.Forms.Panel()
        Me.grpGroupEventPost = New System.Windows.Forms.GroupBox()
        Me.chkEventTeaser = New System.Windows.Forms.CheckBox()
        Me.grpEventTeaser = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtEventTeaserMessage = New System.Windows.Forms.TextBox()
        Me.btnSelectEventTeaserAreaMap = New System.Windows.Forms.Button()
        Me.btnClearEventTeaserAreaMap = New System.Windows.Forms.Button()
        Me.txtEventTeaserAreaMapImage = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblClubFullName = New System.Windows.Forms.Label()
        Me.btnPasteBeginnerLink = New System.Windows.Forms.Button()
        Me.txtOtherBeginnerLink = New System.Windows.Forms.TextBox()
        Me.cboBeginnersGuide = New System.Windows.Forms.ComboBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.lblLocalDSTWarning = New System.Windows.Forms.Label()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.lblEventTaskDistance = New System.Windows.Forms.Label()
        Me.cboGroupOrClubName = New System.Windows.Forms.ComboBox()
        Me.txtEventTitle = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
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
        Me.TimeStampContextualMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GetTimeStampTimeOnlyWithoutSeconds = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetFullWithDayOfWeek = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetLongDateTime = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetCountdown = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetTimeStampOnly = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.tabDiscord = New System.Windows.Forms.TabPage()
        Me.lblAllSecPostsTotalCars = New System.Windows.Forms.Label()
        Me.pnlWizardDiscord = New System.Windows.Forms.Panel()
        Me.btnDiscordGuideNext = New System.Windows.Forms.Button()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.lblDiscordGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlDiscordArrow = New System.Windows.Forms.Panel()
        Me.txtAddOnsDetails = New System.Windows.Forms.TextBox()
        Me.txtWaypointsDetails = New System.Windows.Forms.TextBox()
        Me.lblNbrCarsRestrictions = New System.Windows.Forms.Label()
        Me.txtGroupFlightEventPost = New System.Windows.Forms.TextBox()
        Me.grpDiscordTask = New System.Windows.Forms.GroupBox()
        Me.grpRepost = New System.Windows.Forms.GroupBox()
        Me.dtRepostOriginalDate = New System.Windows.Forms.DateTimePicker()
        Me.chkRepost = New System.Windows.Forms.CheckBox()
        Me.grpDiscordTaskThread = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnFullDescriptionCopy = New System.Windows.Forms.Button()
        Me.lblNbrCarsFullDescResults = New System.Windows.Forms.Label()
        Me.btnFilesCopy = New System.Windows.Forms.Button()
        Me.btnFilesTextCopy = New System.Windows.Forms.Button()
        Me.chkGroupSecondaryPosts = New System.Windows.Forms.CheckBox()
        Me.btnCopyAllSecPosts = New System.Windows.Forms.Button()
        Me.btnAltRestricCopy = New System.Windows.Forms.Button()
        Me.btnWaypointsCopy = New System.Windows.Forms.Button()
        Me.btnAddOnsCopy = New System.Windows.Forms.Button()
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
        Me.grpTaskFeatured = New System.Windows.Forms.GroupBox()
        Me.btnTaskFeaturedOnGroupFlight = New System.Windows.Forms.Button()
        Me.grpGroupFlightEvent = New System.Windows.Forms.GroupBox()
        Me.btnEventDPHXAndLinkOnly = New System.Windows.Forms.Button()
        Me.btnEventTaskDetails = New System.Windows.Forms.Button()
        Me.btnGroupFlightEventThreadLogistics = New System.Windows.Forms.Button()
        Me.btnGroupFlightEventTeaser = New System.Windows.Forms.Button()
        Me.btnDiscordGroupEventURL = New System.Windows.Forms.Button()
        Me.txtGroupEventPostURL = New System.Windows.Forms.TextBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.btnEventFilesAndFilesInfo = New System.Windows.Forms.Button()
        Me.btnGroupFlightEventInfoToClipboard = New System.Windows.Forms.Button()
        Me.grpDiscordEvent = New System.Windows.Forms.GroupBox()
        Me.btnDiscordSharedEventURL = New System.Windows.Forms.Button()
        Me.txtDiscordEventShareURL = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
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
        Me.chkExpertMode = New System.Windows.Forms.CheckBox()
        Me.tabBriefing = New System.Windows.Forms.TabPage()
        Me.pnlBriefing = New System.Windows.Forms.Panel()
        Me.pnlWizardBriefing = New System.Windows.Forms.Panel()
        Me.lblBriefingGuideInstructions = New System.Windows.Forms.Label()
        Me.btnBriefingGuideNext = New System.Windows.Forms.Button()
        Me.BriefingControl1 = New SIGLR.SoaringTools.CommonLibrary.BriefingControl()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.ToolStrip1 = New SIGLR.SoaringTools.CommonLibrary.ToolStripExtensions.ToolStripExtended()
        Me.toolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.toolStripSave = New System.Windows.Forms.ToolStripButton()
        Me.toolStripReload = New System.Windows.Forms.ToolStripButton()
        Me.toolStripResetAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripDiscordTaskLibrary = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripB21Planner = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripSharePackage = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripGuideMe = New System.Windows.Forms.ToolStripButton()
        Me.toolStripStopGuide = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.DiscordChannelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoToFeedbackChannelOnDiscordToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DiscordInviteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCurrentDateTime = New System.Windows.Forms.ToolStripDropDownButton()
        Me.GetNowTimeOnlyWithoutSeconds = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowFullWithDayOfWeek = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowLongDateTime = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowCountdown = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowTimeStampOnly = New System.Windows.Forms.ToolStripMenuItem()
        Me.OneMinuteTimer = New System.Windows.Forms.Timer(Me.components)
        Me.pnlEventDateTimeControls = New System.Windows.Forms.Panel()
        Me.pnlScrollableSurface.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.tabFlightPlan.SuspendLayout()
        Me.pnlGuide.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.grbTaskInfo.SuspendLayout()
        Me.grbTaskPart2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.grbTaskDiscord.SuspendLayout()
        Me.tabEvent.SuspendLayout()
        Me.pnlWizardEvent.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.grpGroupEventPost.SuspendLayout()
        Me.grpEventTeaser.SuspendLayout()
        Me.TimeStampContextualMenu.SuspendLayout()
        Me.tabDiscord.SuspendLayout()
        Me.pnlWizardDiscord.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.grpDiscordTask.SuspendLayout()
        Me.grpRepost.SuspendLayout()
        Me.grpDiscordTaskThread.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.grpDiscordGroupFlight.SuspendLayout()
        Me.grpTaskFeatured.SuspendLayout()
        Me.grpGroupFlightEvent.SuspendLayout()
        Me.grpDiscordEvent.SuspendLayout()
        Me.tabBriefing.SuspendLayout()
        Me.pnlBriefing.SuspendLayout()
        Me.pnlWizardBriefing.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.pnlEventDateTimeControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlScrollableSurface
        '
        Me.pnlScrollableSurface.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlScrollableSurface.AutoScroll = True
        Me.pnlScrollableSurface.Controls.Add(Me.TabControl1)
        Me.pnlScrollableSurface.Location = New System.Drawing.Point(0, 28)
        Me.pnlScrollableSurface.MinimumSize = New System.Drawing.Size(1489, 892)
        Me.pnlScrollableSurface.Name = "pnlScrollableSurface"
        Me.pnlScrollableSurface.Size = New System.Drawing.Size(1489, 892)
        Me.pnlScrollableSurface.TabIndex = 0
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.tabFlightPlan)
        Me.TabControl1.Controls.Add(Me.tabEvent)
        Me.TabControl1.Controls.Add(Me.tabDiscord)
        Me.TabControl1.Controls.Add(Me.tabBriefing)
        Me.TabControl1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.ItemSize = New System.Drawing.Size(90, 25)
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1489, 892)
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControl1.TabIndex = 0
        '
        'tabFlightPlan
        '
        Me.tabFlightPlan.AutoScroll = True
        Me.tabFlightPlan.BackColor = System.Drawing.Color.Transparent
        Me.tabFlightPlan.Controls.Add(Me.lblElevationUpdateWarning)
        Me.tabFlightPlan.Controls.Add(Me.pnlGuide)
        Me.tabFlightPlan.Controls.Add(Me.grbTaskInfo)
        Me.tabFlightPlan.Controls.Add(Me.txtFlightPlanFile)
        Me.tabFlightPlan.Controls.Add(Me.btnSelectFlightPlan)
        Me.tabFlightPlan.Controls.Add(Me.grbTaskPart2)
        Me.tabFlightPlan.Controls.Add(Me.grbTaskDiscord)
        Me.tabFlightPlan.Controls.Add(Me.FileDropZone1)
        Me.tabFlightPlan.Location = New System.Drawing.Point(4, 29)
        Me.tabFlightPlan.Name = "tabFlightPlan"
        Me.tabFlightPlan.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFlightPlan.Size = New System.Drawing.Size(1481, 859)
        Me.tabFlightPlan.TabIndex = 0
        Me.tabFlightPlan.Text = "Flight Plan"
        '
        'lblElevationUpdateWarning
        '
        Me.lblElevationUpdateWarning.AutoSize = True
        Me.lblElevationUpdateWarning.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblElevationUpdateWarning.ForeColor = System.Drawing.Color.Red
        Me.lblElevationUpdateWarning.Location = New System.Drawing.Point(71, 51)
        Me.lblElevationUpdateWarning.Name = "lblElevationUpdateWarning"
        Me.lblElevationUpdateWarning.Size = New System.Drawing.Size(659, 21)
        Me.lblElevationUpdateWarning.TabIndex = 83
        Me.lblElevationUpdateWarning.Text = "One or more waypoints have their elevation set to 1500' - Possible elevation upda" &
    "te required!"
        Me.ToolTip1.SetToolTip(Me.lblElevationUpdateWarning, "Open the flight plan on the B21 Planner and make sure to update all elevations. O" &
        "therwise, you can dismiss this warning.")
        Me.lblElevationUpdateWarning.Visible = False
        '
        'pnlGuide
        '
        Me.pnlGuide.BackColor = System.Drawing.Color.Gray
        Me.pnlGuide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlGuide.Controls.Add(Me.btnGuideNext)
        Me.pnlGuide.Controls.Add(Me.Panel3)
        Me.pnlGuide.Controls.Add(Me.pnlArrow)
        Me.pnlGuide.Location = New System.Drawing.Point(0, 705)
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
        Me.btnGuideNext.TabIndex = 0
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
        Me.grbTaskInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.grbTaskInfo.Controls.Add(Me.chkSoaringTypeDynamic)
        Me.grbTaskInfo.Controls.Add(Me.chkSoaringTypeWave)
        Me.grbTaskInfo.Controls.Add(Me.btnPasteUsernameCredits)
        Me.grbTaskInfo.Controls.Add(Me.Label9)
        Me.grbTaskInfo.Controls.Add(Me.chkTitleLock)
        Me.grbTaskInfo.Controls.Add(Me.chkArrivalLock)
        Me.grbTaskInfo.Controls.Add(Me.chkDepartureLock)
        Me.grbTaskInfo.Controls.Add(Me.chkSoaringTypeThermal)
        Me.grbTaskInfo.Controls.Add(Me.chkSoaringTypeRidge)
        Me.grbTaskInfo.Controls.Add(Me.txtSoaringTypeExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.lblSoaringType)
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
        Me.grbTaskInfo.Controls.Add(Me.lblDeparture)
        Me.grbTaskInfo.Controls.Add(Me.txtMainArea)
        Me.grbTaskInfo.Controls.Add(Me.lblMainAreaPOI)
        Me.grbTaskInfo.Controls.Add(Me.txtTitle)
        Me.grbTaskInfo.Controls.Add(Me.lblTitle)
        Me.grbTaskInfo.Controls.Add(Me.cboSpeedUnits)
        Me.grbTaskInfo.Controls.Add(Me.txtMinAvgSpeed)
        Me.grbTaskInfo.Controls.Add(Me.Label21)
        Me.grbTaskInfo.Controls.Add(Me.txtMaxAvgSpeed)
        Me.grbTaskInfo.Controls.Add(Me.Label22)
        Me.grbTaskInfo.Controls.Add(Me.btnSelectWeatherFile)
        Me.grbTaskInfo.Controls.Add(Me.txtWeatherFile)
        Me.grbTaskInfo.Controls.Add(Me.txtDurationMin)
        Me.grbTaskInfo.Controls.Add(Me.lblDuration)
        Me.grbTaskInfo.Controls.Add(Me.txtDurationMax)
        Me.grbTaskInfo.Controls.Add(Me.Label13)
        Me.grbTaskInfo.Controls.Add(Me.txtDurationExtraInfo)
        Me.grbTaskInfo.Controls.Add(Me.lblRecommendedGliders)
        Me.grbTaskInfo.Controls.Add(Me.txtCredits)
        Me.grbTaskInfo.Controls.Add(Me.lblDifficultyRating)
        Me.grbTaskInfo.Controls.Add(Me.lblCredits)
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
        Me.grbTaskInfo.Size = New System.Drawing.Size(729, 796)
        Me.grbTaskInfo.TabIndex = 2
        Me.grbTaskInfo.TabStop = False
        '
        'chkSoaringTypeDynamic
        '
        Me.chkSoaringTypeDynamic.AutoSize = True
        Me.chkSoaringTypeDynamic.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeDynamic.Location = New System.Drawing.Point(338, 271)
        Me.chkSoaringTypeDynamic.Name = "chkSoaringTypeDynamic"
        Me.chkSoaringTypeDynamic.Size = New System.Drawing.Size(44, 30)
        Me.chkSoaringTypeDynamic.TabIndex = 56
        Me.chkSoaringTypeDynamic.Tag = "8"
        Me.chkSoaringTypeDynamic.Text = "D"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeDynamic, "Check if track involves dynamic soaring.")
        Me.chkSoaringTypeDynamic.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeWave
        '
        Me.chkSoaringTypeWave.AutoSize = True
        Me.chkSoaringTypeWave.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeWave.Location = New System.Drawing.Point(284, 271)
        Me.chkSoaringTypeWave.Name = "chkSoaringTypeWave"
        Me.chkSoaringTypeWave.Size = New System.Drawing.Size(48, 30)
        Me.chkSoaringTypeWave.TabIndex = 26
        Me.chkSoaringTypeWave.Tag = "8"
        Me.chkSoaringTypeWave.Text = "W"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeWave, "Check if track involves wave soaring.")
        Me.chkSoaringTypeWave.UseVisualStyleBackColor = True
        '
        'btnPasteUsernameCredits
        '
        Me.btnPasteUsernameCredits.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPasteUsernameCredits.Location = New System.Drawing.Point(641, 536)
        Me.btnPasteUsernameCredits.Name = "btnPasteUsernameCredits"
        Me.btnPasteUsernameCredits.Size = New System.Drawing.Size(79, 29)
        Me.btnPasteUsernameCredits.TabIndex = 53
        Me.btnPasteUsernameCredits.Tag = "22"
        Me.btnPasteUsernameCredits.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnPasteUsernameCredits, "Click this button to set the credits field along with the username in the clipboa" &
        "rd.")
        Me.btnPasteUsernameCredits.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(4, 307)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(85, 26)
        Me.Label9.TabIndex = 28
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
        Me.chkSoaringTypeThermal.Location = New System.Drawing.Point(237, 271)
        Me.chkSoaringTypeThermal.Name = "chkSoaringTypeThermal"
        Me.chkSoaringTypeThermal.Size = New System.Drawing.Size(41, 30)
        Me.chkSoaringTypeThermal.TabIndex = 25
        Me.chkSoaringTypeThermal.Tag = "8"
        Me.chkSoaringTypeThermal.Text = "T"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeThermal, "Check if track involves thermal soaring.")
        Me.chkSoaringTypeThermal.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeRidge
        '
        Me.chkSoaringTypeRidge.AutoSize = True
        Me.chkSoaringTypeRidge.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeRidge.Location = New System.Drawing.Point(189, 271)
        Me.chkSoaringTypeRidge.Name = "chkSoaringTypeRidge"
        Me.chkSoaringTypeRidge.Size = New System.Drawing.Size(42, 30)
        Me.chkSoaringTypeRidge.TabIndex = 24
        Me.chkSoaringTypeRidge.Tag = "8"
        Me.chkSoaringTypeRidge.Text = "R"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeRidge, "Check if track involves ridge soaring.")
        Me.chkSoaringTypeRidge.UseVisualStyleBackColor = True
        '
        'txtSoaringTypeExtraInfo
        '
        Me.txtSoaringTypeExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSoaringTypeExtraInfo.Location = New System.Drawing.Point(388, 270)
        Me.txtSoaringTypeExtraInfo.Name = "txtSoaringTypeExtraInfo"
        Me.txtSoaringTypeExtraInfo.Size = New System.Drawing.Size(332, 32)
        Me.txtSoaringTypeExtraInfo.TabIndex = 27
        Me.txtSoaringTypeExtraInfo.Tag = "8"
        Me.ToolTip1.SetToolTip(Me.txtSoaringTypeExtraInfo, "Any extra information to add to the soaring type line.")
        '
        'lblSoaringType
        '
        Me.lblSoaringType.AutoSize = True
        Me.lblSoaringType.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSoaringType.Location = New System.Drawing.Point(4, 273)
        Me.lblSoaringType.Name = "lblSoaringType"
        Me.lblSoaringType.Size = New System.Drawing.Size(120, 26)
        Me.lblSoaringType.TabIndex = 23
        Me.lblSoaringType.Text = "Soaring Type"
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
        'lblDeparture
        '
        Me.lblDeparture.AutoSize = True
        Me.lblDeparture.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDeparture.Location = New System.Drawing.Point(4, 205)
        Me.lblDeparture.Name = "lblDeparture"
        Me.lblDeparture.Size = New System.Drawing.Size(97, 26)
        Me.lblDeparture.TabIndex = 13
        Me.lblDeparture.Text = "Departure"
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
        'lblMainAreaPOI
        '
        Me.lblMainAreaPOI.AutoSize = True
        Me.lblMainAreaPOI.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMainAreaPOI.Location = New System.Drawing.Point(4, 171)
        Me.lblMainAreaPOI.Name = "lblMainAreaPOI"
        Me.lblMainAreaPOI.Size = New System.Drawing.Size(137, 26)
        Me.lblMainAreaPOI.TabIndex = 11
        Me.lblMainAreaPOI.Text = "Main area / POI"
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
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(4, 70)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(47, 26)
        Me.lblTitle.TabIndex = 2
        Me.lblTitle.Text = "Title"
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
        Me.cboSpeedUnits.TabIndex = 37
        Me.cboSpeedUnits.Tag = "10"
        Me.ToolTip1.SetToolTip(Me.cboSpeedUnits, "Select units to use for average speed input.")
        '
        'txtMinAvgSpeed
        '
        Me.txtMinAvgSpeed.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMinAvgSpeed.Location = New System.Drawing.Point(189, 338)
        Me.txtMinAvgSpeed.Name = "txtMinAvgSpeed"
        Me.txtMinAvgSpeed.Size = New System.Drawing.Size(50, 32)
        Me.txtMinAvgSpeed.TabIndex = 34
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
        Me.Label21.TabIndex = 35
        Me.Label21.Text = "to"
        '
        'txtMaxAvgSpeed
        '
        Me.txtMaxAvgSpeed.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMaxAvgSpeed.Location = New System.Drawing.Point(281, 338)
        Me.txtMaxAvgSpeed.Name = "txtMaxAvgSpeed"
        Me.txtMaxAvgSpeed.Size = New System.Drawing.Size(50, 32)
        Me.txtMaxAvgSpeed.TabIndex = 36
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
        Me.Label22.TabIndex = 33
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
        Me.txtDurationMin.TabIndex = 39
        Me.txtDurationMin.Tag = "11"
        Me.txtDurationMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtDurationMin, "Approximate minimum duration in minutes")
        '
        'lblDuration
        '
        Me.lblDuration.AutoSize = True
        Me.lblDuration.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDuration.Location = New System.Drawing.Point(4, 375)
        Me.lblDuration.Name = "lblDuration"
        Me.lblDuration.Size = New System.Drawing.Size(132, 26)
        Me.lblDuration.TabIndex = 38
        Me.lblDuration.Text = "Duration (min)"
        '
        'txtDurationMax
        '
        Me.txtDurationMax.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDurationMax.Location = New System.Drawing.Point(281, 372)
        Me.txtDurationMax.Name = "txtDurationMax"
        Me.txtDurationMax.Size = New System.Drawing.Size(50, 32)
        Me.txtDurationMax.TabIndex = 41
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
        Me.Label13.TabIndex = 40
        Me.Label13.Text = "to"
        '
        'txtDurationExtraInfo
        '
        Me.txtDurationExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDurationExtraInfo.Location = New System.Drawing.Point(337, 372)
        Me.txtDurationExtraInfo.Name = "txtDurationExtraInfo"
        Me.txtDurationExtraInfo.Size = New System.Drawing.Size(383, 32)
        Me.txtDurationExtraInfo.TabIndex = 42
        Me.txtDurationExtraInfo.Tag = "11"
        Me.ToolTip1.SetToolTip(Me.txtDurationExtraInfo, "Any extra information to add on the duration line.")
        '
        'lblRecommendedGliders
        '
        Me.lblRecommendedGliders.AutoSize = True
        Me.lblRecommendedGliders.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRecommendedGliders.Location = New System.Drawing.Point(4, 409)
        Me.lblRecommendedGliders.Name = "lblRecommendedGliders"
        Me.lblRecommendedGliders.Size = New System.Drawing.Size(133, 26)
        Me.lblRecommendedGliders.TabIndex = 43
        Me.lblRecommendedGliders.Text = "Recom. gliders"
        '
        'txtCredits
        '
        Me.txtCredits.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCredits.Location = New System.Drawing.Point(189, 535)
        Me.txtCredits.Name = "txtCredits"
        Me.txtCredits.Size = New System.Drawing.Size(446, 32)
        Me.txtCredits.TabIndex = 52
        Me.txtCredits.Tag = "15"
        Me.txtCredits.Text = "All credits to @UserName for this task."
        Me.ToolTip1.SetToolTip(Me.txtCredits, "Specify credits for this flight as required.")
        '
        'lblDifficultyRating
        '
        Me.lblDifficultyRating.AutoSize = True
        Me.lblDifficultyRating.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDifficultyRating.Location = New System.Drawing.Point(4, 443)
        Me.lblDifficultyRating.Name = "lblDifficultyRating"
        Me.lblDifficultyRating.Size = New System.Drawing.Size(144, 26)
        Me.lblDifficultyRating.TabIndex = 45
        Me.lblDifficultyRating.Text = "Difficulty Rating"
        '
        'lblCredits
        '
        Me.lblCredits.AutoSize = True
        Me.lblCredits.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCredits.Location = New System.Drawing.Point(4, 538)
        Me.lblCredits.Name = "lblCredits"
        Me.lblCredits.Size = New System.Drawing.Size(73, 26)
        Me.lblCredits.TabIndex = 51
        Me.lblCredits.Text = "Credits"
        '
        'lblTotalDistanceAndMiles
        '
        Me.lblTotalDistanceAndMiles.AutoSize = True
        Me.lblTotalDistanceAndMiles.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalDistanceAndMiles.Location = New System.Drawing.Point(245, 306)
        Me.lblTotalDistanceAndMiles.Name = "lblTotalDistanceAndMiles"
        Me.lblTotalDistanceAndMiles.Size = New System.Drawing.Size(160, 26)
        Me.lblTotalDistanceAndMiles.TabIndex = 30
        Me.lblTotalDistanceAndMiles.Text = "km / 9999 mi Total"
        '
        'lblTrackDistanceAndMiles
        '
        Me.lblTrackDistanceAndMiles.AutoSize = True
        Me.lblTrackDistanceAndMiles.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTrackDistanceAndMiles.Location = New System.Drawing.Point(468, 306)
        Me.lblTrackDistanceAndMiles.Name = "lblTrackDistanceAndMiles"
        Me.lblTrackDistanceAndMiles.Size = New System.Drawing.Size(156, 26)
        Me.lblTrackDistanceAndMiles.TabIndex = 32
        Me.lblTrackDistanceAndMiles.Text = "km / 9999 mi Task"
        '
        'cboDifficulty
        '
        Me.cboDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDifficulty.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboDifficulty.FormattingEnabled = True
        Me.cboDifficulty.Items.AddRange(New Object() {"0. None / Custom", "1. Beginner", "2. Student", "3. Experienced", "4. Professional", "5. Champion"})
        Me.cboDifficulty.Location = New System.Drawing.Point(189, 440)
        Me.cboDifficulty.Name = "cboDifficulty"
        Me.cboDifficulty.Size = New System.Drawing.Size(251, 32)
        Me.cboDifficulty.TabIndex = 46
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
        Me.txtDistanceTotal.TabIndex = 29
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
        Me.txtDistanceTrack.TabIndex = 31
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
        Me.cboRecommendedGliders.TabIndex = 44
        Me.cboRecommendedGliders.Tag = "12"
        Me.ToolTip1.SetToolTip(Me.cboRecommendedGliders, "Recommended gliders (suggestions in the list or enter your own)")
        '
        'txtDifficultyExtraInfo
        '
        Me.txtDifficultyExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDifficultyExtraInfo.Location = New System.Drawing.Point(446, 440)
        Me.txtDifficultyExtraInfo.Name = "txtDifficultyExtraInfo"
        Me.txtDifficultyExtraInfo.Size = New System.Drawing.Size(274, 32)
        Me.txtDifficultyExtraInfo.TabIndex = 47
        Me.txtDifficultyExtraInfo.Tag = "13"
        Me.ToolTip1.SetToolTip(Me.txtDifficultyExtraInfo, "Any extra information or custom rating to use on the difficulty line.")
        '
        'chkDescriptionLock
        '
        Me.chkDescriptionLock.AutoSize = True
        Me.chkDescriptionLock.Location = New System.Drawing.Point(168, 486)
        Me.chkDescriptionLock.Name = "chkDescriptionLock"
        Me.chkDescriptionLock.Size = New System.Drawing.Size(15, 14)
        Me.chkDescriptionLock.TabIndex = 49
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
        Me.Label16.TabIndex = 48
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
        Me.txtShortDescription.TabIndex = 50
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
        Me.Label17.TabIndex = 54
        Me.Label17.Text = "Long Description"
        '
        'txtLongDescription
        '
        Me.txtLongDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtLongDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLongDescription.Location = New System.Drawing.Point(189, 569)
        Me.txtLongDescription.Multiline = True
        Me.txtLongDescription.Name = "txtLongDescription"
        Me.txtLongDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLongDescription.Size = New System.Drawing.Size(531, 217)
        Me.txtLongDescription.TabIndex = 55
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
        'grbTaskPart2
        '
        Me.grbTaskPart2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbTaskPart2.Controls.Add(Me.chkSuppressWarningForBaroPressure)
        Me.grbTaskPart2.Controls.Add(Me.txtBaroPressureExtraInfo)
        Me.grbTaskPart2.Controls.Add(Me.lblNonStdBaroPressure)
        Me.grbTaskPart2.Controls.Add(Me.GroupBox3)
        Me.grbTaskPart2.Controls.Add(Me.GroupBox2)
        Me.grbTaskPart2.Controls.Add(Me.chkLockCountries)
        Me.grbTaskPart2.Controls.Add(Me.btnMoveCountryDown)
        Me.grbTaskPart2.Controls.Add(Me.btnMoveCountryUp)
        Me.grbTaskPart2.Controls.Add(Me.txtDPHXPackageFilename)
        Me.grbTaskPart2.Controls.Add(Me.btnRemoveCountry)
        Me.grbTaskPart2.Controls.Add(Me.btnAddCountry)
        Me.grbTaskPart2.Controls.Add(Me.lstAllCountries)
        Me.grbTaskPart2.Controls.Add(Me.cboCountryFlag)
        Me.grbTaskPart2.Controls.Add(Me.lblCountries)
        Me.grbTaskPart2.Controls.Add(Me.chkUseOnlyWeatherSummary)
        Me.grbTaskPart2.Controls.Add(Me.txtWeatherSummary)
        Me.grbTaskPart2.Controls.Add(Me.lblWeatherSummary)
        Me.grbTaskPart2.Enabled = False
        Me.grbTaskPart2.Location = New System.Drawing.Point(743, -5)
        Me.grbTaskPart2.Name = "grbTaskPart2"
        Me.grbTaskPart2.Size = New System.Drawing.Size(729, 613)
        Me.grbTaskPart2.TabIndex = 3
        Me.grbTaskPart2.TabStop = False
        Me.grbTaskPart2.Tag = "17"
        '
        'chkSuppressWarningForBaroPressure
        '
        Me.chkSuppressWarningForBaroPressure.AutoSize = True
        Me.chkSuppressWarningForBaroPressure.Location = New System.Drawing.Point(189, 177)
        Me.chkSuppressWarningForBaroPressure.Name = "chkSuppressWarningForBaroPressure"
        Me.chkSuppressWarningForBaroPressure.Size = New System.Drawing.Size(164, 24)
        Me.chkSuppressWarningForBaroPressure.TabIndex = 12
        Me.chkSuppressWarningForBaroPressure.Tag = "19"
        Me.chkSuppressWarningForBaroPressure.Text = "Suppress ⚠️Symbol"
        Me.ToolTip1.SetToolTip(Me.chkSuppressWarningForBaroPressure, "When checked, the warning symbol will not be added next to a non-standard baromet" &
        "ric pressure.")
        Me.chkSuppressWarningForBaroPressure.UseVisualStyleBackColor = True
        '
        'txtBaroPressureExtraInfo
        '
        Me.txtBaroPressureExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtBaroPressureExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBaroPressureExtraInfo.Location = New System.Drawing.Point(359, 172)
        Me.txtBaroPressureExtraInfo.Name = "txtBaroPressureExtraInfo"
        Me.txtBaroPressureExtraInfo.Size = New System.Drawing.Size(361, 32)
        Me.txtBaroPressureExtraInfo.TabIndex = 13
        Me.txtBaroPressureExtraInfo.Tag = "19"
        Me.ToolTip1.SetToolTip(Me.txtBaroPressureExtraInfo, "Any extra information to add to a non-standard barometric pressure.")
        '
        'lblNonStdBaroPressure
        '
        Me.lblNonStdBaroPressure.AutoSize = True
        Me.lblNonStdBaroPressure.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNonStdBaroPressure.Location = New System.Drawing.Point(6, 175)
        Me.lblNonStdBaroPressure.Name = "lblNonStdBaroPressure"
        Me.lblNonStdBaroPressure.Size = New System.Drawing.Size(148, 26)
        Me.lblNonStdBaroPressure.TabIndex = 11
        Me.lblNonStdBaroPressure.Text = "Barom. pressure"
        '
        'GroupBox3
        '
        Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox3.Controls.Add(Me.chkLockCoverImage)
        Me.GroupBox3.Controls.Add(Me.chkLockMapImage)
        Me.GroupBox3.Controls.Add(Me.cboCoverImage)
        Me.GroupBox3.Controls.Add(Me.lblMap)
        Me.GroupBox3.Controls.Add(Me.cboBriefingMap)
        Me.GroupBox3.Controls.Add(Me.lstAllFiles)
        Me.GroupBox3.Controls.Add(Me.btnAddExtraFile)
        Me.GroupBox3.Controls.Add(Me.Label10)
        Me.GroupBox3.Controls.Add(Me.btnRemoveExtraFile)
        Me.GroupBox3.Controls.Add(Me.btnExtraFileUp)
        Me.GroupBox3.Controls.Add(Me.btnExtraFileDown)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 376)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(717, 229)
        Me.GroupBox3.TabIndex = 15
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Extra files"
        '
        'chkLockCoverImage
        '
        Me.chkLockCoverImage.AutoSize = True
        Me.chkLockCoverImage.Location = New System.Drawing.Point(162, 197)
        Me.chkLockCoverImage.Name = "chkLockCoverImage"
        Me.chkLockCoverImage.Size = New System.Drawing.Size(15, 14)
        Me.chkLockCoverImage.TabIndex = 11
        Me.chkLockCoverImage.Tag = "23"
        Me.ToolTip1.SetToolTip(Me.chkLockCoverImage, "When checked, cover image will not be auto selected")
        Me.chkLockCoverImage.UseVisualStyleBackColor = True
        '
        'chkLockMapImage
        '
        Me.chkLockMapImage.AutoSize = True
        Me.chkLockMapImage.Location = New System.Drawing.Point(162, 166)
        Me.chkLockMapImage.Name = "chkLockMapImage"
        Me.chkLockMapImage.Size = New System.Drawing.Size(15, 14)
        Me.chkLockMapImage.TabIndex = 10
        Me.chkLockMapImage.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.chkLockMapImage, "When checked, map will not be auto selected")
        Me.chkLockMapImage.UseVisualStyleBackColor = True
        '
        'cboCoverImage
        '
        Me.cboCoverImage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboCoverImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCoverImage.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.cboCoverImage.FormattingEnabled = True
        Me.cboCoverImage.Location = New System.Drawing.Point(183, 190)
        Me.cboCoverImage.Name = "cboCoverImage"
        Me.cboCoverImage.Size = New System.Drawing.Size(531, 28)
        Me.cboCoverImage.TabIndex = 8
        Me.cboCoverImage.Tag = "23"
        Me.ToolTip1.SetToolTip(Me.cboCoverImage, "Select the image to post as cover for the flight/task (any image file named ""Cove" &
        "r"" will be selected automatically)")
        '
        'lblMap
        '
        Me.lblMap.AutoSize = True
        Me.lblMap.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMap.Location = New System.Drawing.Point(0, 156)
        Me.lblMap.Name = "lblMap"
        Me.lblMap.Size = New System.Drawing.Size(104, 26)
        Me.lblMap.TabIndex = 5
        Me.lblMap.Text = "Map Image"
        '
        'cboBriefingMap
        '
        Me.cboBriefingMap.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboBriefingMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBriefingMap.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.cboBriefingMap.FormattingEnabled = True
        Me.cboBriefingMap.Location = New System.Drawing.Point(183, 156)
        Me.cboBriefingMap.Name = "cboBriefingMap"
        Me.cboBriefingMap.Size = New System.Drawing.Size(531, 28)
        Me.cboBriefingMap.TabIndex = 6
        Me.cboBriefingMap.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.cboBriefingMap, "Select the image for the map display (any image file named ""Map"" will be selected" &
        " automatically)")
        '
        'lstAllFiles
        '
        Me.lstAllFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstAllFiles.FormattingEnabled = True
        Me.lstAllFiles.HorizontalScrollbar = True
        Me.lstAllFiles.ItemHeight = 20
        Me.lstAllFiles.Location = New System.Drawing.Point(183, 26)
        Me.lstAllFiles.Name = "lstAllFiles"
        Me.lstAllFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAllFiles.Size = New System.Drawing.Size(531, 124)
        Me.lstAllFiles.TabIndex = 1
        Me.lstAllFiles.Tag = "21"
        Me.ToolTip1.SetToolTip(Me.lstAllFiles, "List of the extra files to include with the flight plan.")
        '
        'btnAddExtraFile
        '
        Me.btnAddExtraFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddExtraFile.Location = New System.Drawing.Point(2, 26)
        Me.btnAddExtraFile.Name = "btnAddExtraFile"
        Me.btnAddExtraFile.Size = New System.Drawing.Size(175, 35)
        Me.btnAddExtraFile.TabIndex = 0
        Me.btnAddExtraFile.Tag = "21"
        Me.btnAddExtraFile.Text = "Add extra file"
        Me.ToolTip1.SetToolTip(Me.btnAddExtraFile, "Click to add an extra file to include with the flight plan.")
        Me.btnAddExtraFile.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(0, 190)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(117, 26)
        Me.Label10.TabIndex = 7
        Me.Label10.Text = "Cover Image"
        '
        'btnRemoveExtraFile
        '
        Me.btnRemoveExtraFile.Enabled = False
        Me.btnRemoveExtraFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveExtraFile.Location = New System.Drawing.Point(2, 67)
        Me.btnRemoveExtraFile.Name = "btnRemoveExtraFile"
        Me.btnRemoveExtraFile.Size = New System.Drawing.Size(175, 35)
        Me.btnRemoveExtraFile.TabIndex = 2
        Me.btnRemoveExtraFile.Tag = "21"
        Me.btnRemoveExtraFile.Text = "Remove selected file"
        Me.ToolTip1.SetToolTip(Me.btnRemoveExtraFile, "Click to remove the selected extra file from the flight plan.")
        Me.btnRemoveExtraFile.UseVisualStyleBackColor = True
        '
        'btnExtraFileUp
        '
        Me.btnExtraFileUp.Enabled = False
        Me.btnExtraFileUp.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!)
        Me.btnExtraFileUp.Location = New System.Drawing.Point(2, 108)
        Me.btnExtraFileUp.Name = "btnExtraFileUp"
        Me.btnExtraFileUp.Size = New System.Drawing.Size(84, 35)
        Me.btnExtraFileUp.TabIndex = 3
        Me.btnExtraFileUp.Tag = "21"
        Me.btnExtraFileUp.Text = "▲"
        Me.ToolTip1.SetToolTip(Me.btnExtraFileUp, "Click to move the selected file up in the list.")
        Me.btnExtraFileUp.UseVisualStyleBackColor = True
        '
        'btnExtraFileDown
        '
        Me.btnExtraFileDown.Enabled = False
        Me.btnExtraFileDown.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!)
        Me.btnExtraFileDown.Location = New System.Drawing.Point(93, 108)
        Me.btnExtraFileDown.Name = "btnExtraFileDown"
        Me.btnExtraFileDown.Size = New System.Drawing.Size(84, 35)
        Me.btnExtraFileDown.TabIndex = 4
        Me.btnExtraFileDown.Tag = "21"
        Me.btnExtraFileDown.Text = "▼"
        Me.ToolTip1.SetToolTip(Me.btnExtraFileDown, "Click to move the selected file down in the list.")
        Me.btnExtraFileDown.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.btnRemoveSelectedAddOns)
        Me.GroupBox2.Controls.Add(Me.lstAllRecommendedAddOns)
        Me.GroupBox2.Controls.Add(Me.btnAddOnDown)
        Me.GroupBox2.Controls.Add(Me.btnAddRecAddOn)
        Me.GroupBox2.Controls.Add(Me.btnAddOnUp)
        Me.GroupBox2.Controls.Add(Me.btnEditSelectedAddOn)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 210)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(717, 160)
        Me.GroupBox2.TabIndex = 14
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Recommended Add-Ons"
        '
        'btnRemoveSelectedAddOns
        '
        Me.btnRemoveSelectedAddOns.Enabled = False
        Me.btnRemoveSelectedAddOns.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveSelectedAddOns.Location = New System.Drawing.Point(93, 67)
        Me.btnRemoveSelectedAddOns.Name = "btnRemoveSelectedAddOns"
        Me.btnRemoveSelectedAddOns.Size = New System.Drawing.Size(84, 35)
        Me.btnRemoveSelectedAddOns.TabIndex = 2
        Me.btnRemoveSelectedAddOns.Tag = "20"
        Me.btnRemoveSelectedAddOns.Text = "Remove"
        Me.ToolTip1.SetToolTip(Me.btnRemoveSelectedAddOns, "Click to remove any selected add-on(s).")
        Me.btnRemoveSelectedAddOns.UseVisualStyleBackColor = True
        '
        'lstAllRecommendedAddOns
        '
        Me.lstAllRecommendedAddOns.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstAllRecommendedAddOns.FormattingEnabled = True
        Me.lstAllRecommendedAddOns.HorizontalScrollbar = True
        Me.lstAllRecommendedAddOns.ItemHeight = 20
        Me.lstAllRecommendedAddOns.Location = New System.Drawing.Point(183, 26)
        Me.lstAllRecommendedAddOns.Name = "lstAllRecommendedAddOns"
        Me.lstAllRecommendedAddOns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAllRecommendedAddOns.Size = New System.Drawing.Size(531, 124)
        Me.lstAllRecommendedAddOns.TabIndex = 5
        Me.lstAllRecommendedAddOns.Tag = "20"
        Me.ToolTip1.SetToolTip(Me.lstAllRecommendedAddOns, "List of the recommended add-ons for this task.")
        '
        'btnAddOnDown
        '
        Me.btnAddOnDown.Enabled = False
        Me.btnAddOnDown.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!)
        Me.btnAddOnDown.Location = New System.Drawing.Point(93, 108)
        Me.btnAddOnDown.Name = "btnAddOnDown"
        Me.btnAddOnDown.Size = New System.Drawing.Size(84, 35)
        Me.btnAddOnDown.TabIndex = 4
        Me.btnAddOnDown.Tag = "20"
        Me.btnAddOnDown.Text = "▼"
        Me.ToolTip1.SetToolTip(Me.btnAddOnDown, "Click to move the selected add-on down in the list.")
        Me.btnAddOnDown.UseVisualStyleBackColor = True
        '
        'btnAddRecAddOn
        '
        Me.btnAddRecAddOn.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddRecAddOn.Location = New System.Drawing.Point(2, 26)
        Me.btnAddRecAddOn.Name = "btnAddRecAddOn"
        Me.btnAddRecAddOn.Size = New System.Drawing.Size(175, 35)
        Me.btnAddRecAddOn.TabIndex = 0
        Me.btnAddRecAddOn.Tag = "20"
        Me.btnAddRecAddOn.Text = "Add new add-on"
        Me.ToolTip1.SetToolTip(Me.btnAddRecAddOn, "Click to add a recommended add-on to the list")
        Me.btnAddRecAddOn.UseVisualStyleBackColor = True
        '
        'btnAddOnUp
        '
        Me.btnAddOnUp.Enabled = False
        Me.btnAddOnUp.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!)
        Me.btnAddOnUp.Location = New System.Drawing.Point(2, 108)
        Me.btnAddOnUp.Name = "btnAddOnUp"
        Me.btnAddOnUp.Size = New System.Drawing.Size(84, 35)
        Me.btnAddOnUp.TabIndex = 3
        Me.btnAddOnUp.Tag = "20"
        Me.btnAddOnUp.Text = "▲"
        Me.ToolTip1.SetToolTip(Me.btnAddOnUp, "Click to move the selected add-on up in the list.")
        Me.btnAddOnUp.UseVisualStyleBackColor = True
        '
        'btnEditSelectedAddOn
        '
        Me.btnEditSelectedAddOn.Enabled = False
        Me.btnEditSelectedAddOn.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEditSelectedAddOn.Location = New System.Drawing.Point(2, 67)
        Me.btnEditSelectedAddOn.Name = "btnEditSelectedAddOn"
        Me.btnEditSelectedAddOn.Size = New System.Drawing.Size(84, 35)
        Me.btnEditSelectedAddOn.TabIndex = 1
        Me.btnEditSelectedAddOn.Tag = "20"
        Me.btnEditSelectedAddOn.Text = "Edit"
        Me.ToolTip1.SetToolTip(Me.btnEditSelectedAddOn, "Click to edit the selected add-on.")
        Me.btnEditSelectedAddOn.UseVisualStyleBackColor = True
        '
        'chkLockCountries
        '
        Me.chkLockCountries.AutoSize = True
        Me.chkLockCountries.Location = New System.Drawing.Point(168, 70)
        Me.chkLockCountries.Name = "chkLockCountries"
        Me.chkLockCountries.Size = New System.Drawing.Size(15, 14)
        Me.chkLockCountries.TabIndex = 2
        Me.chkLockCountries.Tag = ""
        Me.ToolTip1.SetToolTip(Me.chkLockCountries, "When checked, countries will not be automatically loaded from flight plan.")
        Me.chkLockCountries.UseVisualStyleBackColor = True
        '
        'btnMoveCountryDown
        '
        Me.btnMoveCountryDown.Enabled = False
        Me.btnMoveCountryDown.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMoveCountryDown.Location = New System.Drawing.Point(320, 97)
        Me.btnMoveCountryDown.Name = "btnMoveCountryDown"
        Me.btnMoveCountryDown.Size = New System.Drawing.Size(38, 29)
        Me.btnMoveCountryDown.TabIndex = 7
        Me.btnMoveCountryDown.Tag = "17"
        Me.btnMoveCountryDown.Text = "▼"
        Me.ToolTip1.SetToolTip(Me.btnMoveCountryDown, "Click to move the selected countries down in the list.")
        Me.btnMoveCountryDown.UseVisualStyleBackColor = True
        '
        'btnMoveCountryUp
        '
        Me.btnMoveCountryUp.Enabled = False
        Me.btnMoveCountryUp.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnMoveCountryUp.Location = New System.Drawing.Point(320, 61)
        Me.btnMoveCountryUp.Name = "btnMoveCountryUp"
        Me.btnMoveCountryUp.Size = New System.Drawing.Size(38, 29)
        Me.btnMoveCountryUp.TabIndex = 4
        Me.btnMoveCountryUp.Tag = "17"
        Me.btnMoveCountryUp.Text = "▲"
        Me.ToolTip1.SetToolTip(Me.btnMoveCountryUp, "Click to move the selected countries up in the list.")
        Me.btnMoveCountryUp.UseVisualStyleBackColor = True
        '
        'txtDPHXPackageFilename
        '
        Me.txtDPHXPackageFilename.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDPHXPackageFilename.Location = New System.Drawing.Point(11, 83)
        Me.txtDPHXPackageFilename.Name = "txtDPHXPackageFilename"
        Me.txtDPHXPackageFilename.ReadOnly = True
        Me.txtDPHXPackageFilename.Size = New System.Drawing.Size(43, 32)
        Me.txtDPHXPackageFilename.TabIndex = 9
        Me.txtDPHXPackageFilename.TabStop = False
        Me.txtDPHXPackageFilename.Tag = ""
        Me.ToolTip1.SetToolTip(Me.txtDPHXPackageFilename, "Current DPHX package file selected.")
        Me.txtDPHXPackageFilename.Visible = False
        '
        'btnRemoveCountry
        '
        Me.btnRemoveCountry.Enabled = False
        Me.btnRemoveCountry.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveCountry.Location = New System.Drawing.Point(189, 97)
        Me.btnRemoveCountry.Name = "btnRemoveCountry"
        Me.btnRemoveCountry.Size = New System.Drawing.Size(125, 29)
        Me.btnRemoveCountry.TabIndex = 6
        Me.btnRemoveCountry.Tag = "17"
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
        Me.btnAddCountry.TabIndex = 3
        Me.btnAddCountry.Tag = "17"
        Me.btnAddCountry.Text = "Add Country"
        Me.ToolTip1.SetToolTip(Me.btnAddCountry, "Click to add the selected country to the list")
        Me.btnAddCountry.UseVisualStyleBackColor = True
        '
        'lstAllCountries
        '
        Me.lstAllCountries.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstAllCountries.FormattingEnabled = True
        Me.lstAllCountries.HorizontalScrollbar = True
        Me.lstAllCountries.ItemHeight = 20
        Me.lstAllCountries.Location = New System.Drawing.Point(364, 62)
        Me.lstAllCountries.Name = "lstAllCountries"
        Me.lstAllCountries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAllCountries.Size = New System.Drawing.Size(356, 64)
        Me.lstAllCountries.TabIndex = 5
        Me.lstAllCountries.Tag = "17"
        Me.ToolTip1.SetToolTip(Me.lstAllCountries, "List of the countries which flags are added to the title")
        '
        'cboCountryFlag
        '
        Me.cboCountryFlag.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboCountryFlag.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cboCountryFlag.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboCountryFlag.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboCountryFlag.FormattingEnabled = True
        Me.cboCountryFlag.Location = New System.Drawing.Point(189, 24)
        Me.cboCountryFlag.Name = "cboCountryFlag"
        Me.cboCountryFlag.Size = New System.Drawing.Size(531, 32)
        Me.cboCountryFlag.TabIndex = 1
        Me.cboCountryFlag.Tag = "17"
        Me.ToolTip1.SetToolTip(Me.cboCountryFlag, "Select a country to add to the selection (for its flag to be added in the title)")
        '
        'lblCountries
        '
        Me.lblCountries.AutoSize = True
        Me.lblCountries.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCountries.Location = New System.Drawing.Point(6, 27)
        Me.lblCountries.Name = "lblCountries"
        Me.lblCountries.Size = New System.Drawing.Size(145, 26)
        Me.lblCountries.TabIndex = 0
        Me.lblCountries.Text = "Countries/Flags"
        '
        'chkUseOnlyWeatherSummary
        '
        Me.chkUseOnlyWeatherSummary.AutoSize = True
        Me.chkUseOnlyWeatherSummary.Location = New System.Drawing.Point(168, 147)
        Me.chkUseOnlyWeatherSummary.Name = "chkUseOnlyWeatherSummary"
        Me.chkUseOnlyWeatherSummary.Size = New System.Drawing.Size(15, 14)
        Me.chkUseOnlyWeatherSummary.TabIndex = 9
        Me.chkUseOnlyWeatherSummary.Tag = "18"
        Me.ToolTip1.SetToolTip(Me.chkUseOnlyWeatherSummary, "When checked, only summary will be used for weather information.")
        Me.chkUseOnlyWeatherSummary.UseVisualStyleBackColor = True
        '
        'txtWeatherSummary
        '
        Me.txtWeatherSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWeatherSummary.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherSummary.Location = New System.Drawing.Point(189, 134)
        Me.txtWeatherSummary.Name = "txtWeatherSummary"
        Me.txtWeatherSummary.Size = New System.Drawing.Size(531, 32)
        Me.txtWeatherSummary.TabIndex = 10
        Me.txtWeatherSummary.Tag = "18"
        Me.ToolTip1.SetToolTip(Me.txtWeatherSummary, "Summary of the weather profile.")
        '
        'lblWeatherSummary
        '
        Me.lblWeatherSummary.AutoSize = True
        Me.lblWeatherSummary.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWeatherSummary.Location = New System.Drawing.Point(6, 137)
        Me.lblWeatherSummary.Name = "lblWeatherSummary"
        Me.lblWeatherSummary.Size = New System.Drawing.Size(166, 26)
        Me.lblWeatherSummary.TabIndex = 8
        Me.lblWeatherSummary.Text = "Weather Summary"
        '
        'grbTaskDiscord
        '
        Me.grbTaskDiscord.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbTaskDiscord.Controls.Add(Me.btnDeleteDiscordID)
        Me.grbTaskDiscord.Controls.Add(Me.btnDiscordTaskThreadURLPaste)
        Me.grbTaskDiscord.Controls.Add(Me.txtDiscordTaskID)
        Me.grbTaskDiscord.Controls.Add(Me.Label31)
        Me.grbTaskDiscord.Enabled = False
        Me.grbTaskDiscord.Location = New System.Drawing.Point(743, 614)
        Me.grbTaskDiscord.Name = "grbTaskDiscord"
        Me.grbTaskDiscord.Size = New System.Drawing.Size(729, 72)
        Me.grbTaskDiscord.TabIndex = 4
        Me.grbTaskDiscord.TabStop = False
        Me.grbTaskDiscord.Text = "Discord / Task ID"
        '
        'btnDeleteDiscordID
        '
        Me.btnDeleteDiscordID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDeleteDiscordID.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDeleteDiscordID.Location = New System.Drawing.Point(641, 27)
        Me.btnDeleteDiscordID.Name = "btnDeleteDiscordID"
        Me.btnDeleteDiscordID.Size = New System.Drawing.Size(79, 29)
        Me.btnDeleteDiscordID.TabIndex = 3
        Me.btnDeleteDiscordID.Tag = "24"
        Me.btnDeleteDiscordID.Text = "Clear"
        Me.ToolTip1.SetToolTip(Me.btnDeleteDiscordID, "Click this button to clear the Discord ID linked with this task")
        Me.btnDeleteDiscordID.UseVisualStyleBackColor = True
        '
        'btnDiscordTaskThreadURLPaste
        '
        Me.btnDiscordTaskThreadURLPaste.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDiscordTaskThreadURLPaste.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordTaskThreadURLPaste.Location = New System.Drawing.Point(556, 27)
        Me.btnDiscordTaskThreadURLPaste.Name = "btnDiscordTaskThreadURLPaste"
        Me.btnDiscordTaskThreadURLPaste.Size = New System.Drawing.Size(79, 29)
        Me.btnDiscordTaskThreadURLPaste.TabIndex = 2
        Me.btnDiscordTaskThreadURLPaste.Tag = "24"
        Me.btnDiscordTaskThreadURLPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnDiscordTaskThreadURLPaste, "Click this button to paste the task ID from your clipboard")
        Me.btnDiscordTaskThreadURLPaste.UseVisualStyleBackColor = True
        '
        'txtDiscordTaskID
        '
        Me.txtDiscordTaskID.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDiscordTaskID.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordTaskID.Location = New System.Drawing.Point(189, 27)
        Me.txtDiscordTaskID.Name = "txtDiscordTaskID"
        Me.txtDiscordTaskID.ReadOnly = True
        Me.txtDiscordTaskID.Size = New System.Drawing.Size(361, 32)
        Me.txtDiscordTaskID.TabIndex = 1
        Me.txtDiscordTaskID.Tag = "24"
        Me.ToolTip1.SetToolTip(Me.txtDiscordTaskID, "The ID of the task on Discord, i.e., where to post results among other things")
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label31.Location = New System.Drawing.Point(6, 30)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(109, 26)
        Me.Label31.TabIndex = 0
        Me.Label31.Text = "Task Post ID"
        '
        'FileDropZone1
        '
        Me.FileDropZone1.AllowDrop = True
        Me.FileDropZone1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileDropZone1.Location = New System.Drawing.Point(743, 692)
        Me.FileDropZone1.Name = "FileDropZone1"
        Me.FileDropZone1.Size = New System.Drawing.Size(728, 161)
        Me.FileDropZone1.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.FileDropZone1, "Drag files here to automatically process them depending on their type")
        '
        'tabEvent
        '
        Me.tabEvent.Controls.Add(Me.chkActivateEvent)
        Me.tabEvent.Controls.Add(Me.pnlWizardEvent)
        Me.tabEvent.Controls.Add(Me.grpGroupEventPost)
        Me.tabEvent.Location = New System.Drawing.Point(4, 29)
        Me.tabEvent.Name = "tabEvent"
        Me.tabEvent.Padding = New System.Windows.Forms.Padding(3)
        Me.tabEvent.Size = New System.Drawing.Size(1481, 859)
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
        Me.chkActivateEvent.TabIndex = 1
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
        Me.pnlWizardEvent.Location = New System.Drawing.Point(849, 702)
        Me.pnlWizardEvent.Name = "pnlWizardEvent"
        Me.pnlWizardEvent.Size = New System.Drawing.Size(627, 89)
        Me.pnlWizardEvent.TabIndex = 83
        Me.pnlWizardEvent.Visible = False
        '
        'btnEventGuideNext
        '
        Me.btnEventGuideNext.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventGuideNext.Location = New System.Drawing.Point(3, 3)
        Me.btnEventGuideNext.Name = "btnEventGuideNext"
        Me.btnEventGuideNext.Size = New System.Drawing.Size(73, 83)
        Me.btnEventGuideNext.TabIndex = 0
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
        Me.Panel2.Size = New System.Drawing.Size(453, 89)
        Me.Panel2.TabIndex = 81
        '
        'lblEventGuideInstructions
        '
        Me.lblEventGuideInstructions.Font = New System.Drawing.Font("Segoe UI Variable Display Semib", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEventGuideInstructions.ForeColor = System.Drawing.Color.White
        Me.lblEventGuideInstructions.Location = New System.Drawing.Point(-1, 0)
        Me.lblEventGuideInstructions.Name = "lblEventGuideInstructions"
        Me.lblEventGuideInstructions.Size = New System.Drawing.Size(451, 89)
        Me.lblEventGuideInstructions.TabIndex = 0
        Me.lblEventGuideInstructions.Text = "Click the ""Flight Plan"" button and select the flight plan to use for this task."
        Me.lblEventGuideInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlEventArrow
        '
        Me.pnlEventArrow.BackColor = System.Drawing.Color.Gray
        Me.pnlEventArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        Me.pnlEventArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlEventArrow.Location = New System.Drawing.Point(535, 0)
        Me.pnlEventArrow.Name = "pnlEventArrow"
        Me.pnlEventArrow.Size = New System.Drawing.Size(91, 89)
        Me.pnlEventArrow.TabIndex = 80
        '
        'grpGroupEventPost
        '
        Me.grpGroupEventPost.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpGroupEventPost.Controls.Add(Me.chkEventTeaser)
        Me.grpGroupEventPost.Controls.Add(Me.grpEventTeaser)
        Me.grpGroupEventPost.Controls.Add(Me.lblClubFullName)
        Me.grpGroupEventPost.Controls.Add(Me.btnPasteBeginnerLink)
        Me.grpGroupEventPost.Controls.Add(Me.txtOtherBeginnerLink)
        Me.grpGroupEventPost.Controls.Add(Me.cboBeginnersGuide)
        Me.grpGroupEventPost.Controls.Add(Me.Label30)
        Me.grpGroupEventPost.Controls.Add(Me.lblLocalDSTWarning)
        Me.grpGroupEventPost.Controls.Add(Me.Label48)
        Me.grpGroupEventPost.Controls.Add(Me.lblEventTaskDistance)
        Me.grpGroupEventPost.Controls.Add(Me.cboGroupOrClubName)
        Me.grpGroupEventPost.Controls.Add(Me.txtEventTitle)
        Me.grpGroupEventPost.Controls.Add(Me.Label41)
        Me.grpGroupEventPost.Controls.Add(Me.cboEligibleAward)
        Me.grpGroupEventPost.Controls.Add(Me.Label36)
        Me.grpGroupEventPost.Controls.Add(Me.Label35)
        Me.grpGroupEventPost.Controls.Add(Me.Label34)
        Me.grpGroupEventPost.Controls.Add(Me.cboVoiceChannel)
        Me.grpGroupEventPost.Controls.Add(Me.cboMSFSServer)
        Me.grpGroupEventPost.Controls.Add(Me.Label33)
        Me.grpGroupEventPost.Controls.Add(Me.Label32)
        Me.grpGroupEventPost.Controls.Add(Me.txtEventDescription)
        Me.grpGroupEventPost.Controls.Add(Me.chkUseStart)
        Me.grpGroupEventPost.Controls.Add(Me.Label29)
        Me.grpGroupEventPost.Controls.Add(Me.chkUseLaunch)
        Me.grpGroupEventPost.Controls.Add(Me.Label28)
        Me.grpGroupEventPost.Controls.Add(Me.chkUseSyncFly)
        Me.grpGroupEventPost.Controls.Add(Me.Label27)
        Me.grpGroupEventPost.Controls.Add(Me.Label25)
        Me.grpGroupEventPost.Controls.Add(Me.chkDateTimeUTC)
        Me.grpGroupEventPost.Controls.Add(Me.Label26)
        Me.grpGroupEventPost.Controls.Add(Me.Label24)
        Me.grpGroupEventPost.Controls.Add(Me.pnlEventDateTimeControls)
        Me.grpGroupEventPost.Enabled = False
        Me.grpGroupEventPost.Location = New System.Drawing.Point(6, 6)
        Me.grpGroupEventPost.MinimumSize = New System.Drawing.Size(848, 850)
        Me.grpGroupEventPost.Name = "grpGroupEventPost"
        Me.grpGroupEventPost.Size = New System.Drawing.Size(1466, 850)
        Me.grpGroupEventPost.TabIndex = 0
        Me.grpGroupEventPost.TabStop = False
        '
        'chkEventTeaser
        '
        Me.chkEventTeaser.AutoSize = True
        Me.chkEventTeaser.BackColor = System.Drawing.SystemColors.Control
        Me.chkEventTeaser.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkEventTeaser.Location = New System.Drawing.Point(15, 697)
        Me.chkEventTeaser.Name = "chkEventTeaser"
        Me.chkEventTeaser.Size = New System.Drawing.Size(109, 24)
        Me.chkEventTeaser.TabIndex = 85
        Me.chkEventTeaser.Tag = "72"
        Me.chkEventTeaser.Text = "Event Teaser"
        Me.ToolTip1.SetToolTip(Me.chkEventTeaser, "Check this if you will be posting a group event teaser")
        Me.chkEventTeaser.UseVisualStyleBackColor = False
        '
        'grpEventTeaser
        '
        Me.grpEventTeaser.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpEventTeaser.Controls.Add(Me.Label2)
        Me.grpEventTeaser.Controls.Add(Me.txtEventTeaserMessage)
        Me.grpEventTeaser.Controls.Add(Me.btnSelectEventTeaserAreaMap)
        Me.grpEventTeaser.Controls.Add(Me.btnClearEventTeaserAreaMap)
        Me.grpEventTeaser.Controls.Add(Me.txtEventTeaserAreaMapImage)
        Me.grpEventTeaser.Controls.Add(Me.Label1)
        Me.grpEventTeaser.Location = New System.Drawing.Point(7, 698)
        Me.grpEventTeaser.Name = "grpEventTeaser"
        Me.grpEventTeaser.Size = New System.Drawing.Size(1454, 146)
        Me.grpEventTeaser.TabIndex = 50
        Me.grpEventTeaser.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(6, 67)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(87, 26)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Message"
        '
        'txtEventTeaserMessage
        '
        Me.txtEventTeaserMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEventTeaserMessage.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventTeaserMessage.Location = New System.Drawing.Point(185, 64)
        Me.txtEventTeaserMessage.Multiline = True
        Me.txtEventTeaserMessage.Name = "txtEventTeaserMessage"
        Me.txtEventTeaserMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtEventTeaserMessage.Size = New System.Drawing.Size(1263, 76)
        Me.txtEventTeaserMessage.TabIndex = 5
        Me.txtEventTeaserMessage.Tag = "72"
        Me.ToolTip1.SetToolTip(Me.txtEventTeaserMessage, "Specify any message you want to be posted with the teaser")
        '
        'btnSelectEventTeaserAreaMap
        '
        Me.btnSelectEventTeaserAreaMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectEventTeaserAreaMap.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectEventTeaserAreaMap.Location = New System.Drawing.Point(1284, 27)
        Me.btnSelectEventTeaserAreaMap.Name = "btnSelectEventTeaserAreaMap"
        Me.btnSelectEventTeaserAreaMap.Size = New System.Drawing.Size(79, 29)
        Me.btnSelectEventTeaserAreaMap.TabIndex = 2
        Me.btnSelectEventTeaserAreaMap.Tag = "72"
        Me.btnSelectEventTeaserAreaMap.Text = "Select"
        Me.ToolTip1.SetToolTip(Me.btnSelectEventTeaserAreaMap, "Click this button to browse and select the teaser area map")
        Me.btnSelectEventTeaserAreaMap.UseVisualStyleBackColor = True
        '
        'btnClearEventTeaserAreaMap
        '
        Me.btnClearEventTeaserAreaMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearEventTeaserAreaMap.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClearEventTeaserAreaMap.Location = New System.Drawing.Point(1369, 26)
        Me.btnClearEventTeaserAreaMap.Name = "btnClearEventTeaserAreaMap"
        Me.btnClearEventTeaserAreaMap.Size = New System.Drawing.Size(79, 29)
        Me.btnClearEventTeaserAreaMap.TabIndex = 3
        Me.btnClearEventTeaserAreaMap.Tag = "72"
        Me.btnClearEventTeaserAreaMap.Text = "Clear"
        Me.ToolTip1.SetToolTip(Me.btnClearEventTeaserAreaMap, "Click this button to clear the teaser area map")
        Me.btnClearEventTeaserAreaMap.UseVisualStyleBackColor = True
        '
        'txtEventTeaserAreaMapImage
        '
        Me.txtEventTeaserAreaMapImage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEventTeaserAreaMapImage.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventTeaserAreaMapImage.Location = New System.Drawing.Point(185, 26)
        Me.txtEventTeaserAreaMapImage.Name = "txtEventTeaserAreaMapImage"
        Me.txtEventTeaserAreaMapImage.ReadOnly = True
        Me.txtEventTeaserAreaMapImage.Size = New System.Drawing.Size(1093, 32)
        Me.txtEventTeaserAreaMapImage.TabIndex = 1
        Me.txtEventTeaserAreaMapImage.Tag = "72"
        Me.ToolTip1.SetToolTip(Me.txtEventTeaserAreaMapImage, "Specify the event title (leave blank if none) - comes from the flight plan (title" &
        ") tab if created in the same session.")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(6, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(146, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Area Map Image"
        '
        'lblClubFullName
        '
        Me.lblClubFullName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblClubFullName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblClubFullName.Location = New System.Drawing.Point(187, 130)
        Me.lblClubFullName.Name = "lblClubFullName"
        Me.lblClubFullName.Size = New System.Drawing.Size(1273, 26)
        Me.lblClubFullName.TabIndex = 45
        '
        'btnPasteBeginnerLink
        '
        Me.btnPasteBeginnerLink.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPasteBeginnerLink.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPasteBeginnerLink.Location = New System.Drawing.Point(1381, 663)
        Me.btnPasteBeginnerLink.Name = "btnPasteBeginnerLink"
        Me.btnPasteBeginnerLink.Size = New System.Drawing.Size(79, 29)
        Me.btnPasteBeginnerLink.TabIndex = 44
        Me.btnPasteBeginnerLink.Tag = "71"
        Me.btnPasteBeginnerLink.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnPasteBeginnerLink, "Click this button to paste the beginner's link from your clipboard")
        Me.btnPasteBeginnerLink.UseVisualStyleBackColor = True
        '
        'txtOtherBeginnerLink
        '
        Me.txtOtherBeginnerLink.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOtherBeginnerLink.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOtherBeginnerLink.Location = New System.Drawing.Point(192, 660)
        Me.txtOtherBeginnerLink.Name = "txtOtherBeginnerLink"
        Me.txtOtherBeginnerLink.Size = New System.Drawing.Size(1183, 32)
        Me.txtOtherBeginnerLink.TabIndex = 43
        Me.txtOtherBeginnerLink.Tag = "71"
        Me.ToolTip1.SetToolTip(Me.txtOtherBeginnerLink, "Specify the URL (link) to the guide you want to include")
        '
        'cboBeginnersGuide
        '
        Me.cboBeginnersGuide.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboBeginnersGuide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBeginnersGuide.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboBeginnersGuide.FormattingEnabled = True
        Me.cboBeginnersGuide.Items.AddRange(New Object() {"None", "The Beginner's Guide to Soaring Events (GotGravel)", "How to join our Group Flights (Sim Soaring Club)", "Other (provide link below)"})
        Me.cboBeginnersGuide.Location = New System.Drawing.Point(192, 622)
        Me.cboBeginnersGuide.Name = "cboBeginnersGuide"
        Me.cboBeginnersGuide.Size = New System.Drawing.Size(1268, 32)
        Me.cboBeginnersGuide.TabIndex = 42
        Me.cboBeginnersGuide.Tag = "71"
        Me.ToolTip1.SetToolTip(Me.cboBeginnersGuide, "You can select a link to guide beginners into soaring group flights")
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(7, 625)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(127, 26)
        Me.Label30.TabIndex = 41
        Me.Label30.Text = "For beginners"
        '
        'lblLocalDSTWarning
        '
        Me.lblLocalDSTWarning.AutoSize = True
        Me.lblLocalDSTWarning.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLocalDSTWarning.Location = New System.Drawing.Point(553, 271)
        Me.lblLocalDSTWarning.Name = "lblLocalDSTWarning"
        Me.lblLocalDSTWarning.Size = New System.Drawing.Size(217, 26)
        Me.lblLocalDSTWarning.TabIndex = 12
        Me.lblLocalDSTWarning.Text = "⚠️Local DST in effect⚠️"
        Me.lblLocalDSTWarning.Visible = False
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label48.Location = New System.Drawing.Point(7, 28)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(609, 26)
        Me.Label48.TabIndex = 0
        Me.Label48.Text = "On the Flight Plan tab, please load the event's flight plan and weather file."
        '
        'lblEventTaskDistance
        '
        Me.lblEventTaskDistance.AutoSize = True
        Me.lblEventTaskDistance.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEventTaskDistance.Location = New System.Drawing.Point(481, 587)
        Me.lblEventTaskDistance.Name = "lblEventTaskDistance"
        Me.lblEventTaskDistance.Size = New System.Drawing.Size(53, 26)
        Me.lblEventTaskDistance.TabIndex = 36
        Me.lblEventTaskDistance.Text = "0 Km"
        Me.lblEventTaskDistance.Visible = False
        '
        'cboGroupOrClubName
        '
        Me.cboGroupOrClubName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboGroupOrClubName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboGroupOrClubName.FormattingEnabled = True
        Me.cboGroupOrClubName.Items.AddRange(New Object() {"TSC", "FSC", "SSC Saturday", "Aus Tuesdays", "DTS"})
        Me.cboGroupOrClubName.Location = New System.Drawing.Point(192, 95)
        Me.cboGroupOrClubName.Name = "cboGroupOrClubName"
        Me.cboGroupOrClubName.Size = New System.Drawing.Size(1268, 32)
        Me.cboGroupOrClubName.TabIndex = 3
        Me.cboGroupOrClubName.Tag = "60"
        Me.ToolTip1.SetToolTip(Me.cboGroupOrClubName, "Select or specify the group or club name related to this event. Leave blank if no" &
        "ne.")
        '
        'txtEventTitle
        '
        Me.txtEventTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEventTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventTitle.Location = New System.Drawing.Point(192, 163)
        Me.txtEventTitle.Name = "txtEventTitle"
        Me.txtEventTitle.Size = New System.Drawing.Size(1268, 32)
        Me.txtEventTitle.TabIndex = 5
        Me.txtEventTitle.Tag = "61"
        Me.ToolTip1.SetToolTip(Me.txtEventTitle, "Specify the event title (leave blank if none) - comes from the flight plan (title" &
        ") tab if created in the same session.")
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label41.Location = New System.Drawing.Point(7, 167)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(157, 26)
        Me.Label41.TabIndex = 4
        Me.Label41.Text = "Event Title / Topic"
        '
        'cboEligibleAward
        '
        Me.cboEligibleAward.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEligibleAward.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboEligibleAward.FormattingEnabled = True
        Me.cboEligibleAward.Items.AddRange(New Object() {"None", "Bronze", "Silver", "Gold", "Diamond", "Cloud Surfer"})
        Me.cboEligibleAward.Location = New System.Drawing.Point(192, 584)
        Me.cboEligibleAward.Name = "cboEligibleAward"
        Me.cboEligibleAward.Size = New System.Drawing.Size(283, 32)
        Me.cboEligibleAward.TabIndex = 35
        Me.cboEligibleAward.Tag = "70"
        Me.ToolTip1.SetToolTip(Me.cboEligibleAward, "Select any eligible award for completing this task succesfully during the event.")
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label36.Location = New System.Drawing.Point(7, 587)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(177, 26)
        Me.Label36.TabIndex = 34
        Me.Label36.Text = "Eligible Award (SSC)"
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label35.Location = New System.Drawing.Point(7, 52)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(670, 26)
        Me.Label35.TabIndex = 1
        Me.Label35.Text = "Then also fill out the Sim local Date and Time, Duration fields and Credits (if a" &
    "ny)."
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label34.Location = New System.Drawing.Point(7, 236)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(128, 26)
        Me.Label34.TabIndex = 8
        Me.Label34.Text = "Voice channel"
        '
        'cboVoiceChannel
        '
        Me.cboVoiceChannel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboVoiceChannel.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboVoiceChannel.FormattingEnabled = True
        Me.cboVoiceChannel.Items.AddRange(New Object() {"[Unicom 1](https://discord.com/channels/793376245915189268/793378730750771210)", "[Unicom 2](https://discord.com/channels/793376245915189268/793379061237284874)", "[Unicom 3](https://discord.com/channels/793376245915189268/793437043861487626)", "[Sim Soaring Club (PTT)](https://discord.com/channels/876123356385149009/87639782" &
                "5934626836)", "[Flight 01](https://discord.com/channels/876123356385149009/876123356385149015)", "[Flight 02](https://discord.com/channels/876123356385149009/876130658513203230)", "[General](https://discord.com/channels/325227457445625856/448551355712274435)"})
        Me.cboVoiceChannel.Location = New System.Drawing.Point(192, 233)
        Me.cboVoiceChannel.Name = "cboVoiceChannel"
        Me.cboVoiceChannel.Size = New System.Drawing.Size(1268, 32)
        Me.cboVoiceChannel.TabIndex = 9
        Me.cboVoiceChannel.Tag = "63"
        Me.ToolTip1.SetToolTip(Me.cboVoiceChannel, "Select the voice channel to use for the event (from the list or enter your own).")
        '
        'cboMSFSServer
        '
        Me.cboMSFSServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMSFSServer.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboMSFSServer.FormattingEnabled = True
        Me.cboMSFSServer.Items.AddRange(New Object() {"West Europe", "North Europe", "West USA", "East USA", "Southeast Asia"})
        Me.cboMSFSServer.Location = New System.Drawing.Point(192, 197)
        Me.cboMSFSServer.Name = "cboMSFSServer"
        Me.cboMSFSServer.Size = New System.Drawing.Size(234, 32)
        Me.cboMSFSServer.TabIndex = 7
        Me.cboMSFSServer.Tag = "62"
        Me.ToolTip1.SetToolTip(Me.cboMSFSServer, "Select the MSFS Server to use for the event.")
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label33.Location = New System.Drawing.Point(7, 200)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(174, 26)
        Me.Label33.TabIndex = 6
        Me.Label33.Text = "MSFS Server to use"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label32.Location = New System.Drawing.Point(7, 443)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(109, 26)
        Me.Label32.TabIndex = 32
        Me.Label32.Text = "Description"
        '
        'txtEventDescription
        '
        Me.txtEventDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEventDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventDescription.Location = New System.Drawing.Point(192, 439)
        Me.txtEventDescription.Multiline = True
        Me.txtEventDescription.Name = "txtEventDescription"
        Me.txtEventDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtEventDescription.Size = New System.Drawing.Size(1268, 139)
        Me.txtEventDescription.TabIndex = 33
        Me.txtEventDescription.Tag = "69"
        Me.ToolTip1.SetToolTip(Me.txtEventDescription, "Short description of the flight - comes from the flight plan tab if created in th" &
        "e same session.")
        '
        'lblStartTimeResult
        '
        Me.lblStartTimeResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStartTimeResult.AutoSize = True
        Me.lblStartTimeResult.ContextMenuStrip = Me.TimeStampContextualMenu
        Me.lblStartTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStartTimeResult.Location = New System.Drawing.Point(338, 111)
        Me.lblStartTimeResult.Name = "lblStartTimeResult"
        Me.lblStartTimeResult.Size = New System.Drawing.Size(153, 26)
        Me.lblStartTimeResult.TabIndex = 31
        Me.lblStartTimeResult.Text = "start time results"
        '
        'TimeStampContextualMenu
        '
        Me.TimeStampContextualMenu.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.TimeStampContextualMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetTimeStampTimeOnlyWithoutSeconds, Me.GetFullWithDayOfWeek, Me.GetLongDateTime, Me.GetCountdown, Me.GetTimeStampOnly})
        Me.TimeStampContextualMenu.Name = "TimeStampContextualMenu"
        Me.TimeStampContextualMenu.ShowImageMargin = False
        Me.TimeStampContextualMenu.Size = New System.Drawing.Size(224, 124)
        '
        'GetTimeStampTimeOnlyWithoutSeconds
        '
        Me.GetTimeStampTimeOnlyWithoutSeconds.Name = "GetTimeStampTimeOnlyWithoutSeconds"
        Me.GetTimeStampTimeOnlyWithoutSeconds.Size = New System.Drawing.Size(223, 24)
        Me.GetTimeStampTimeOnlyWithoutSeconds.Text = "Time Only Without Seconds"
        '
        'GetFullWithDayOfWeek
        '
        Me.GetFullWithDayOfWeek.Name = "GetFullWithDayOfWeek"
        Me.GetFullWithDayOfWeek.Size = New System.Drawing.Size(223, 24)
        Me.GetFullWithDayOfWeek.Text = "Full With Day of Week"
        '
        'GetLongDateTime
        '
        Me.GetLongDateTime.Name = "GetLongDateTime"
        Me.GetLongDateTime.Size = New System.Drawing.Size(223, 24)
        Me.GetLongDateTime.Text = "Long Date Time"
        '
        'GetCountdown
        '
        Me.GetCountdown.Name = "GetCountdown"
        Me.GetCountdown.Size = New System.Drawing.Size(223, 24)
        Me.GetCountdown.Text = "Countdown"
        '
        'GetTimeStampOnly
        '
        Me.GetTimeStampOnly.Name = "GetTimeStampOnly"
        Me.GetTimeStampOnly.Size = New System.Drawing.Size(223, 24)
        Me.GetTimeStampOnly.Text = "TimeStampOnly"
        '
        'lblLaunchTimeResult
        '
        Me.lblLaunchTimeResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLaunchTimeResult.AutoSize = True
        Me.lblLaunchTimeResult.ContextMenuStrip = Me.TimeStampContextualMenu
        Me.lblLaunchTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLaunchTimeResult.Location = New System.Drawing.Point(338, 77)
        Me.lblLaunchTimeResult.Name = "lblLaunchTimeResult"
        Me.lblLaunchTimeResult.Size = New System.Drawing.Size(170, 26)
        Me.lblLaunchTimeResult.TabIndex = 26
        Me.lblLaunchTimeResult.Text = "launch time results"
        '
        'lblSyncTimeResult
        '
        Me.lblSyncTimeResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSyncTimeResult.AutoSize = True
        Me.lblSyncTimeResult.ContextMenuStrip = Me.TimeStampContextualMenu
        Me.lblSyncTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSyncTimeResult.Location = New System.Drawing.Point(338, 43)
        Me.lblSyncTimeResult.Name = "lblSyncTimeResult"
        Me.lblSyncTimeResult.Size = New System.Drawing.Size(154, 26)
        Me.lblSyncTimeResult.TabIndex = 21
        Me.lblSyncTimeResult.Text = "sync time results"
        '
        'lblMeetTimeResult
        '
        Me.lblMeetTimeResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMeetTimeResult.AutoSize = True
        Me.lblMeetTimeResult.ContextMenuStrip = Me.TimeStampContextualMenu
        Me.lblMeetTimeResult.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMeetTimeResult.Location = New System.Drawing.Point(338, 9)
        Me.lblMeetTimeResult.Name = "lblMeetTimeResult"
        Me.lblMeetTimeResult.Size = New System.Drawing.Size(157, 26)
        Me.lblMeetTimeResult.TabIndex = 16
        Me.lblMeetTimeResult.Text = "meet time results"
        '
        'chkUseStart
        '
        Me.chkUseStart.AutoSize = True
        Me.chkUseStart.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseStart.Location = New System.Drawing.Point(125, 408)
        Me.chkUseStart.Name = "chkUseStart"
        Me.chkUseStart.Size = New System.Drawing.Size(59, 30)
        Me.chkUseStart.TabIndex = 28
        Me.chkUseStart.Tag = "68"
        Me.chkUseStart.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseStart, "When checked, a task start time will be specified.")
        Me.chkUseStart.UseVisualStyleBackColor = True
        '
        'dtEventStartTaskTime
        '
        Me.dtEventStartTaskTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventStartTaskTime.CustomFormat = "HH:mm tt"
        Me.dtEventStartTaskTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventStartTaskTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventStartTaskTime.Location = New System.Drawing.Point(220, 106)
        Me.dtEventStartTaskTime.Name = "dtEventStartTaskTime"
        Me.dtEventStartTaskTime.ShowUpDown = True
        Me.dtEventStartTaskTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventStartTaskTime.TabIndex = 30
        Me.dtEventStartTaskTime.Tag = "68"
        Me.ToolTip1.SetToolTip(Me.dtEventStartTaskTime, "This is the event's task start time in the specified time zone above.")
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label29.Location = New System.Drawing.Point(7, 410)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(90, 26)
        Me.Label29.TabIndex = 27
        Me.Label29.Text = "Start task"
        '
        'dtEventStartTaskDate
        '
        Me.dtEventStartTaskDate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventStartTaskDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventStartTaskDate.Location = New System.Drawing.Point(3, 106)
        Me.dtEventStartTaskDate.Name = "dtEventStartTaskDate"
        Me.dtEventStartTaskDate.Size = New System.Drawing.Size(211, 31)
        Me.dtEventStartTaskDate.TabIndex = 29
        Me.dtEventStartTaskDate.Tag = "68"
        Me.ToolTip1.SetToolTip(Me.dtEventStartTaskDate, "This is the event's task start date in the specified time zone above.")
        '
        'chkUseLaunch
        '
        Me.chkUseLaunch.AutoSize = True
        Me.chkUseLaunch.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseLaunch.Location = New System.Drawing.Point(125, 374)
        Me.chkUseLaunch.Name = "chkUseLaunch"
        Me.chkUseLaunch.Size = New System.Drawing.Size(59, 30)
        Me.chkUseLaunch.TabIndex = 23
        Me.chkUseLaunch.Tag = "67"
        Me.chkUseLaunch.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseLaunch, "When checked, a launch time will be specified.")
        Me.chkUseLaunch.UseVisualStyleBackColor = True
        '
        'dtEventLaunchTime
        '
        Me.dtEventLaunchTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventLaunchTime.Cursor = System.Windows.Forms.Cursors.Default
        Me.dtEventLaunchTime.CustomFormat = "HH:mm tt"
        Me.dtEventLaunchTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventLaunchTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventLaunchTime.Location = New System.Drawing.Point(220, 72)
        Me.dtEventLaunchTime.Name = "dtEventLaunchTime"
        Me.dtEventLaunchTime.ShowUpDown = True
        Me.dtEventLaunchTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventLaunchTime.TabIndex = 25
        Me.dtEventLaunchTime.Tag = "67"
        Me.ToolTip1.SetToolTip(Me.dtEventLaunchTime, "This is the event's glider launch time in the specified time zone above.")
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label28.Location = New System.Drawing.Point(7, 376)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(73, 26)
        Me.Label28.TabIndex = 22
        Me.Label28.Text = "Launch"
        '
        'dtEventLaunchDate
        '
        Me.dtEventLaunchDate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventLaunchDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventLaunchDate.Location = New System.Drawing.Point(3, 72)
        Me.dtEventLaunchDate.Name = "dtEventLaunchDate"
        Me.dtEventLaunchDate.Size = New System.Drawing.Size(211, 31)
        Me.dtEventLaunchDate.TabIndex = 24
        Me.dtEventLaunchDate.Tag = "67"
        Me.ToolTip1.SetToolTip(Me.dtEventLaunchDate, "This is the event's glider launch date in the specified time zone above.")
        '
        'chkUseSyncFly
        '
        Me.chkUseSyncFly.AutoSize = True
        Me.chkUseSyncFly.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseSyncFly.Location = New System.Drawing.Point(125, 340)
        Me.chkUseSyncFly.Name = "chkUseSyncFly"
        Me.chkUseSyncFly.Size = New System.Drawing.Size(59, 30)
        Me.chkUseSyncFly.TabIndex = 18
        Me.chkUseSyncFly.Tag = "66"
        Me.chkUseSyncFly.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseSyncFly, "When checked, a synchronized ""Click Fly"" will be specified.")
        Me.chkUseSyncFly.UseVisualStyleBackColor = True
        '
        'dtEventSyncFlyTime
        '
        Me.dtEventSyncFlyTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventSyncFlyTime.CustomFormat = "HH:mm tt"
        Me.dtEventSyncFlyTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventSyncFlyTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventSyncFlyTime.Location = New System.Drawing.Point(220, 38)
        Me.dtEventSyncFlyTime.Name = "dtEventSyncFlyTime"
        Me.dtEventSyncFlyTime.ShowUpDown = True
        Me.dtEventSyncFlyTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventSyncFlyTime.TabIndex = 20
        Me.dtEventSyncFlyTime.Tag = "66"
        Me.ToolTip1.SetToolTip(Me.dtEventSyncFlyTime, "This is the event's synchronized ""Click Fly"" time in the specified time zone abov" &
        "e.")
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label27.Location = New System.Drawing.Point(7, 342)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(80, 26)
        Me.Label27.TabIndex = 17
        Me.Label27.Text = "Sync Fly"
        '
        'dtEventSyncFlyDate
        '
        Me.dtEventSyncFlyDate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventSyncFlyDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventSyncFlyDate.Location = New System.Drawing.Point(3, 38)
        Me.dtEventSyncFlyDate.Name = "dtEventSyncFlyDate"
        Me.dtEventSyncFlyDate.Size = New System.Drawing.Size(211, 31)
        Me.dtEventSyncFlyDate.TabIndex = 19
        Me.dtEventSyncFlyDate.Tag = "66"
        Me.ToolTip1.SetToolTip(Me.dtEventSyncFlyDate, "This is the event's synchronized ""Click Fly"" date in the specified time zone abov" &
        "e.")
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(7, 271)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(155, 26)
        Me.Label25.TabIndex = 10
        Me.Label25.Text = "UTC/Zulu or local"
        '
        'chkDateTimeUTC
        '
        Me.chkDateTimeUTC.AutoSize = True
        Me.chkDateTimeUTC.Checked = True
        Me.chkDateTimeUTC.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDateTimeUTC.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkDateTimeUTC.Location = New System.Drawing.Point(192, 269)
        Me.chkDateTimeUTC.Name = "chkDateTimeUTC"
        Me.chkDateTimeUTC.Size = New System.Drawing.Size(355, 30)
        Me.chkDateTimeUTC.TabIndex = 11
        Me.chkDateTimeUTC.Tag = "64"
        Me.chkDateTimeUTC.Text = "UTC / Zulu (local time if left unchecked)"
        Me.ToolTip1.SetToolTip(Me.chkDateTimeUTC, "When checked, the specified date and time are considered as UTC or Zulu.")
        Me.chkDateTimeUTC.UseVisualStyleBackColor = True
        '
        'dtEventMeetTime
        '
        Me.dtEventMeetTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventMeetTime.CustomFormat = "HH:mm tt"
        Me.dtEventMeetTime.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventMeetTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtEventMeetTime.Location = New System.Drawing.Point(220, 4)
        Me.dtEventMeetTime.Name = "dtEventMeetTime"
        Me.dtEventMeetTime.ShowUpDown = True
        Me.dtEventMeetTime.Size = New System.Drawing.Size(104, 31)
        Me.dtEventMeetTime.TabIndex = 15
        Me.dtEventMeetTime.Tag = "65"
        Me.ToolTip1.SetToolTip(Me.dtEventMeetTime, "This is the event's meet time in the specified time zone above.")
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(7, 308)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(136, 26)
        Me.Label26.TabIndex = 13
        Me.Label26.Text = "Meet / Briefing"
        '
        'dtEventMeetDate
        '
        Me.dtEventMeetDate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtEventMeetDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtEventMeetDate.Location = New System.Drawing.Point(3, 4)
        Me.dtEventMeetDate.Name = "dtEventMeetDate"
        Me.dtEventMeetDate.Size = New System.Drawing.Size(211, 31)
        Me.dtEventMeetDate.TabIndex = 14
        Me.dtEventMeetDate.Tag = "65"
        Me.ToolTip1.SetToolTip(Me.dtEventMeetDate, "This is the event's meet date in the specified time zone above.")
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.Location = New System.Drawing.Point(7, 98)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(183, 26)
        Me.Label24.TabIndex = 2
        Me.Label24.Text = "Group or Club Name"
        '
        'tabDiscord
        '
        Me.tabDiscord.Controls.Add(Me.lblAllSecPostsTotalCars)
        Me.tabDiscord.Controls.Add(Me.pnlWizardDiscord)
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
        Me.tabDiscord.Controls.Add(Me.chkExpertMode)
        Me.tabDiscord.Location = New System.Drawing.Point(4, 29)
        Me.tabDiscord.Name = "tabDiscord"
        Me.tabDiscord.Size = New System.Drawing.Size(1481, 859)
        Me.tabDiscord.TabIndex = 3
        Me.tabDiscord.Text = "Discord"
        Me.tabDiscord.UseVisualStyleBackColor = True
        '
        'lblAllSecPostsTotalCars
        '
        Me.lblAllSecPostsTotalCars.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAllSecPostsTotalCars.ForeColor = System.Drawing.Color.Red
        Me.lblAllSecPostsTotalCars.Location = New System.Drawing.Point(311, 434)
        Me.lblAllSecPostsTotalCars.Name = "lblAllSecPostsTotalCars"
        Me.lblAllSecPostsTotalCars.Size = New System.Drawing.Size(90, 26)
        Me.lblAllSecPostsTotalCars.TabIndex = 4
        Me.lblAllSecPostsTotalCars.Text = "0"
        Me.lblAllSecPostsTotalCars.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlWizardDiscord
        '
        Me.pnlWizardDiscord.BackColor = System.Drawing.Color.Gray
        Me.pnlWizardDiscord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlWizardDiscord.Controls.Add(Me.btnDiscordGuideNext)
        Me.pnlWizardDiscord.Controls.Add(Me.Panel4)
        Me.pnlWizardDiscord.Controls.Add(Me.pnlDiscordArrow)
        Me.pnlWizardDiscord.Location = New System.Drawing.Point(411, 750)
        Me.pnlWizardDiscord.Name = "pnlWizardDiscord"
        Me.pnlWizardDiscord.Size = New System.Drawing.Size(750, 89)
        Me.pnlWizardDiscord.TabIndex = 94
        Me.pnlWizardDiscord.Visible = False
        '
        'btnDiscordGuideNext
        '
        Me.btnDiscordGuideNext.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordGuideNext.Location = New System.Drawing.Point(3, 3)
        Me.btnDiscordGuideNext.Name = "btnDiscordGuideNext"
        Me.btnDiscordGuideNext.Size = New System.Drawing.Size(73, 83)
        Me.btnDiscordGuideNext.TabIndex = 0
        Me.btnDiscordGuideNext.Text = "Next"
        Me.ToolTip1.SetToolTip(Me.btnDiscordGuideNext, "Click here to go to the next step in the guide.")
        Me.btnDiscordGuideNext.UseVisualStyleBackColor = True
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.Gray
        Me.Panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Panel4.Controls.Add(Me.lblDiscordGuideInstructions)
        Me.Panel4.Location = New System.Drawing.Point(84, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(586, 89)
        Me.Panel4.TabIndex = 81
        '
        'lblDiscordGuideInstructions
        '
        Me.lblDiscordGuideInstructions.Font = New System.Drawing.Font("Segoe UI Variable Display Semib", 13.74545!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordGuideInstructions.ForeColor = System.Drawing.Color.White
        Me.lblDiscordGuideInstructions.Location = New System.Drawing.Point(-1, 0)
        Me.lblDiscordGuideInstructions.Name = "lblDiscordGuideInstructions"
        Me.lblDiscordGuideInstructions.Size = New System.Drawing.Size(584, 89)
        Me.lblDiscordGuideInstructions.TabIndex = 0
        Me.lblDiscordGuideInstructions.Text = "Click the ""Flight Plan"" button and select the flight plan to use for this task."
        Me.lblDiscordGuideInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlDiscordArrow
        '
        Me.pnlDiscordArrow.BackColor = System.Drawing.Color.Gray
        Me.pnlDiscordArrow.BackgroundImage = Global.SIGLR.SoaringTools.DiscordPostHelper.My.Resources.Resources.right_arrow
        Me.pnlDiscordArrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlDiscordArrow.Location = New System.Drawing.Point(667, 0)
        Me.pnlDiscordArrow.Name = "pnlDiscordArrow"
        Me.pnlDiscordArrow.Size = New System.Drawing.Size(91, 89)
        Me.pnlDiscordArrow.TabIndex = 80
        '
        'txtAddOnsDetails
        '
        Me.txtAddOnsDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddOnsDetails.Location = New System.Drawing.Point(1124, 545)
        Me.txtAddOnsDetails.Multiline = True
        Me.txtAddOnsDetails.Name = "txtAddOnsDetails"
        Me.txtAddOnsDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtAddOnsDetails.Size = New System.Drawing.Size(59, 23)
        Me.txtAddOnsDetails.TabIndex = 93
        Me.txtAddOnsDetails.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtAddOnsDetails, "This is the full description content for the fourth and last Discord post.")
        Me.txtAddOnsDetails.Visible = False
        '
        'txtWaypointsDetails
        '
        Me.txtWaypointsDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWaypointsDetails.Location = New System.Drawing.Point(1124, 516)
        Me.txtWaypointsDetails.Multiline = True
        Me.txtWaypointsDetails.Name = "txtWaypointsDetails"
        Me.txtWaypointsDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWaypointsDetails.Size = New System.Drawing.Size(59, 23)
        Me.txtWaypointsDetails.TabIndex = 92
        Me.txtWaypointsDetails.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtWaypointsDetails, "This is the full description content for the fourth and last Discord post.")
        Me.txtWaypointsDetails.Visible = False
        '
        'lblNbrCarsRestrictions
        '
        Me.lblNbrCarsRestrictions.AutoSize = True
        Me.lblNbrCarsRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsRestrictions.Location = New System.Drawing.Point(1202, 342)
        Me.lblNbrCarsRestrictions.Name = "lblNbrCarsRestrictions"
        Me.lblNbrCarsRestrictions.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsRestrictions.TabIndex = 71
        Me.lblNbrCarsRestrictions.Text = "0"
        Me.lblNbrCarsRestrictions.Visible = False
        '
        'txtGroupFlightEventPost
        '
        Me.txtGroupFlightEventPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGroupFlightEventPost.Location = New System.Drawing.Point(1124, 594)
        Me.txtGroupFlightEventPost.Multiline = True
        Me.txtGroupFlightEventPost.Name = "txtGroupFlightEventPost"
        Me.txtGroupFlightEventPost.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtGroupFlightEventPost.Size = New System.Drawing.Size(59, 23)
        Me.txtGroupFlightEventPost.TabIndex = 2
        Me.txtGroupFlightEventPost.Tag = "41"
        Me.ToolTip1.SetToolTip(Me.txtGroupFlightEventPost, "This is the content of the Discord group flight event post.")
        Me.txtGroupFlightEventPost.Visible = False
        '
        'grpDiscordTask
        '
        Me.grpDiscordTask.Controls.Add(Me.grpRepost)
        Me.grpDiscordTask.Controls.Add(Me.grpDiscordTaskThread)
        Me.grpDiscordTask.Controls.Add(Me.GroupBox1)
        Me.grpDiscordTask.Location = New System.Drawing.Point(8, 3)
        Me.grpDiscordTask.Name = "grpDiscordTask"
        Me.grpDiscordTask.Size = New System.Drawing.Size(405, 755)
        Me.grpDiscordTask.TabIndex = 0
        Me.grpDiscordTask.TabStop = False
        Me.grpDiscordTask.Text = "Task"
        '
        'grpRepost
        '
        Me.grpRepost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpRepost.Controls.Add(Me.dtRepostOriginalDate)
        Me.grpRepost.Controls.Add(Me.chkRepost)
        Me.grpRepost.Location = New System.Drawing.Point(6, 26)
        Me.grpRepost.Name = "grpRepost"
        Me.grpRepost.Size = New System.Drawing.Size(393, 68)
        Me.grpRepost.TabIndex = 0
        Me.grpRepost.TabStop = False
        '
        'dtRepostOriginalDate
        '
        Me.dtRepostOriginalDate.Enabled = False
        Me.dtRepostOriginalDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtRepostOriginalDate.Location = New System.Drawing.Point(6, 26)
        Me.dtRepostOriginalDate.Name = "dtRepostOriginalDate"
        Me.dtRepostOriginalDate.Size = New System.Drawing.Size(381, 31)
        Me.dtRepostOriginalDate.TabIndex = 1
        Me.dtRepostOriginalDate.Tag = "4"
        Me.ToolTip1.SetToolTip(Me.dtRepostOriginalDate, "Date to set in MSFS for the flight")
        '
        'chkRepost
        '
        Me.chkRepost.AutoSize = True
        Me.chkRepost.BackColor = System.Drawing.SystemColors.Control
        Me.chkRepost.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkRepost.Location = New System.Drawing.Point(8, 0)
        Me.chkRepost.Name = "chkRepost"
        Me.chkRepost.Size = New System.Drawing.Size(126, 24)
        Me.chkRepost.TabIndex = 0
        Me.chkRepost.Text = "This is a repost"
        Me.ToolTip1.SetToolTip(Me.chkRepost, "Check this if you are reposting a pre-existing flight")
        Me.chkRepost.UseVisualStyleBackColor = False
        '
        'grpDiscordTaskThread
        '
        Me.grpDiscordTaskThread.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpDiscordTaskThread.Controls.Add(Me.FlowLayoutPanel1)
        Me.grpDiscordTaskThread.Location = New System.Drawing.Point(6, 210)
        Me.grpDiscordTaskThread.Name = "grpDiscordTaskThread"
        Me.grpDiscordTaskThread.Size = New System.Drawing.Size(393, 489)
        Me.grpDiscordTaskThread.TabIndex = 2
        Me.grpDiscordTaskThread.TabStop = False
        Me.grpDiscordTaskThread.Text = "Thread"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnFullDescriptionCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblNbrCarsFullDescResults)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnFilesCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnFilesTextCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.chkGroupSecondaryPosts)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCopyAllSecPosts)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnAltRestricCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnWaypointsCopy)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnAddOnsCopy)
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 26)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(393, 463)
        Me.FlowLayoutPanel1.TabIndex = 82
        '
        'btnFullDescriptionCopy
        '
        Me.btnFullDescriptionCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFullDescriptionCopy.Location = New System.Drawing.Point(3, 3)
        Me.btnFullDescriptionCopy.Name = "btnFullDescriptionCopy"
        Me.btnFullDescriptionCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnFullDescriptionCopy.TabIndex = 0
        Me.btnFullDescriptionCopy.Tag = "42"
        Me.btnFullDescriptionCopy.Text = "2. Thread - Full Description to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFullDescriptionCopy, "Click this button to put the task's full description post content into the clipbo" &
        "ard.")
        Me.btnFullDescriptionCopy.UseVisualStyleBackColor = True
        '
        'lblNbrCarsFullDescResults
        '
        Me.lblNbrCarsFullDescResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsFullDescResults.ForeColor = System.Drawing.Color.Red
        Me.lblNbrCarsFullDescResults.Location = New System.Drawing.Point(3, 57)
        Me.lblNbrCarsFullDescResults.Name = "lblNbrCarsFullDescResults"
        Me.lblNbrCarsFullDescResults.Size = New System.Drawing.Size(384, 26)
        Me.lblNbrCarsFullDescResults.TabIndex = 8
        Me.lblNbrCarsFullDescResults.Text = "0"
        Me.lblNbrCarsFullDescResults.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnFilesCopy
        '
        Me.btnFilesCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFilesCopy.Location = New System.Drawing.Point(3, 86)
        Me.btnFilesCopy.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.btnFilesCopy.Name = "btnFilesCopy"
        Me.btnFilesCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnFilesCopy.TabIndex = 1
        Me.btnFilesCopy.Tag = "43"
        Me.btnFilesCopy.Text = "3a. Thread - Files to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFilesCopy, "Click this button to put the included files into the clipboard.")
        Me.btnFilesCopy.UseVisualStyleBackColor = True
        '
        'btnFilesTextCopy
        '
        Me.btnFilesTextCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFilesTextCopy.Location = New System.Drawing.Point(3, 140)
        Me.btnFilesTextCopy.Margin = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.btnFilesTextCopy.Name = "btnFilesTextCopy"
        Me.btnFilesTextCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnFilesTextCopy.TabIndex = 2
        Me.btnFilesTextCopy.Tag = "44"
        Me.btnFilesTextCopy.Text = "3b. Thread - Files info to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnFilesTextCopy, "Click this button to put the second post content into the clipboard.")
        Me.btnFilesTextCopy.UseVisualStyleBackColor = True
        '
        'chkGroupSecondaryPosts
        '
        Me.chkGroupSecondaryPosts.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkGroupSecondaryPosts.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkGroupSecondaryPosts.Location = New System.Drawing.Point(3, 194)
        Me.chkGroupSecondaryPosts.Name = "chkGroupSecondaryPosts"
        Me.chkGroupSecondaryPosts.Size = New System.Drawing.Size(384, 28)
        Me.chkGroupSecondaryPosts.TabIndex = 3
        Me.chkGroupSecondaryPosts.Tag = "45"
        Me.chkGroupSecondaryPosts.Text = "Merge remaining posts"
        Me.ToolTip1.SetToolTip(Me.chkGroupSecondaryPosts, "Check this to merge all secondary posts into only one.")
        Me.chkGroupSecondaryPosts.UseVisualStyleBackColor = True
        '
        'btnCopyAllSecPosts
        '
        Me.btnCopyAllSecPosts.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCopyAllSecPosts.Location = New System.Drawing.Point(3, 228)
        Me.btnCopyAllSecPosts.Name = "btnCopyAllSecPosts"
        Me.btnCopyAllSecPosts.Size = New System.Drawing.Size(384, 51)
        Me.btnCopyAllSecPosts.TabIndex = 4
        Me.btnCopyAllSecPosts.Tag = "46"
        Me.btnCopyAllSecPosts.Text = "4. Thread - All secondary post's content to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnCopyAllSecPosts, "Click this button to put all remaining content into the clipboard.")
        Me.btnCopyAllSecPosts.UseVisualStyleBackColor = True
        Me.btnCopyAllSecPosts.Visible = False
        '
        'btnAltRestricCopy
        '
        Me.btnAltRestricCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAltRestricCopy.Location = New System.Drawing.Point(3, 285)
        Me.btnAltRestricCopy.Name = "btnAltRestricCopy"
        Me.btnAltRestricCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnAltRestricCopy.TabIndex = 5
        Me.btnAltRestricCopy.Tag = "46"
        Me.btnAltRestricCopy.Text = "4. Thread - Restrictions and Weather to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnAltRestricCopy, "Click this button to put the restrictions and weather post content into the clipb" &
        "oard.")
        Me.btnAltRestricCopy.UseVisualStyleBackColor = True
        '
        'btnWaypointsCopy
        '
        Me.btnWaypointsCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypointsCopy.Location = New System.Drawing.Point(3, 342)
        Me.btnWaypointsCopy.Name = "btnWaypointsCopy"
        Me.btnWaypointsCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnWaypointsCopy.TabIndex = 6
        Me.btnWaypointsCopy.Tag = "47"
        Me.btnWaypointsCopy.Text = "5. Thread - Waypoint details to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnWaypointsCopy, "Click this button to put the waypoints post content into the clipboard.")
        Me.btnWaypointsCopy.UseVisualStyleBackColor = True
        '
        'btnAddOnsCopy
        '
        Me.btnAddOnsCopy.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddOnsCopy.Location = New System.Drawing.Point(3, 399)
        Me.btnAddOnsCopy.Name = "btnAddOnsCopy"
        Me.btnAddOnsCopy.Size = New System.Drawing.Size(384, 51)
        Me.btnAddOnsCopy.TabIndex = 7
        Me.btnAddOnsCopy.Tag = "48"
        Me.btnAddOnsCopy.Text = "6. Thread - Add-ons details to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnAddOnsCopy, "Click this button to put the add-ons post content into the clipboard.")
        Me.btnAddOnsCopy.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnFPMainInfoCopy)
        Me.GroupBox1.Controls.Add(Me.lblNbrCarsMainFP)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 100)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(393, 104)
        Me.GroupBox1.TabIndex = 1
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
        Me.btnFPMainInfoCopy.Tag = "41"
        Me.btnFPMainInfoCopy.Text = "1. Main FP post to clipboard then thread title"
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
        '
        'txtDiscordEventDescription
        '
        Me.txtDiscordEventDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordEventDescription.Location = New System.Drawing.Point(1254, 594)
        Me.txtDiscordEventDescription.Multiline = True
        Me.txtDiscordEventDescription.Name = "txtDiscordEventDescription"
        Me.txtDiscordEventDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDiscordEventDescription.Size = New System.Drawing.Size(59, 23)
        Me.txtDiscordEventDescription.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.txtDiscordEventDescription, "This is the content of the Discord Event description field.")
        Me.txtDiscordEventDescription.Visible = False
        '
        'txtDiscordEventTopic
        '
        Me.txtDiscordEventTopic.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordEventTopic.Location = New System.Drawing.Point(1189, 590)
        Me.txtDiscordEventTopic.Name = "txtDiscordEventTopic"
        Me.txtDiscordEventTopic.Size = New System.Drawing.Size(59, 32)
        Me.txtDiscordEventTopic.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.txtDiscordEventTopic, "Specify the event title (leave blank if none) - comes from the flight plan (title" &
        ") tab if created in the same session.")
        Me.txtDiscordEventTopic.Visible = False
        '
        'lblNbrCarsWeatherClouds
        '
        Me.lblNbrCarsWeatherClouds.AutoSize = True
        Me.lblNbrCarsWeatherClouds.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsWeatherClouds.Location = New System.Drawing.Point(1202, 426)
        Me.lblNbrCarsWeatherClouds.Name = "lblNbrCarsWeatherClouds"
        Me.lblNbrCarsWeatherClouds.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsWeatherClouds.TabIndex = 14
        Me.lblNbrCarsWeatherClouds.Text = "0"
        Me.lblNbrCarsWeatherClouds.Visible = False
        '
        'txtFullDescriptionResults
        '
        Me.txtFullDescriptionResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFullDescriptionResults.Location = New System.Drawing.Point(1124, 487)
        Me.txtFullDescriptionResults.Multiline = True
        Me.txtFullDescriptionResults.Name = "txtFullDescriptionResults"
        Me.txtFullDescriptionResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtFullDescriptionResults.Size = New System.Drawing.Size(59, 23)
        Me.txtFullDescriptionResults.TabIndex = 86
        Me.txtFullDescriptionResults.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtFullDescriptionResults, "This is the full description content for the fourth and last Discord post.")
        Me.txtFullDescriptionResults.Visible = False
        '
        'txtWeatherFirstPart
        '
        Me.txtWeatherFirstPart.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherFirstPart.Location = New System.Drawing.Point(1124, 374)
        Me.txtWeatherFirstPart.Multiline = True
        Me.txtWeatherFirstPart.Name = "txtWeatherFirstPart"
        Me.txtWeatherFirstPart.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWeatherFirstPart.Size = New System.Drawing.Size(59, 23)
        Me.txtWeatherFirstPart.TabIndex = 1
        Me.txtWeatherFirstPart.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtWeatherFirstPart, "This is the basic weather content for the second Discord post.")
        Me.txtWeatherFirstPart.Visible = False
        '
        'txtFilesText
        '
        Me.txtFilesText.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFilesText.Location = New System.Drawing.Point(1124, 461)
        Me.txtFilesText.Multiline = True
        Me.txtFilesText.Name = "txtFilesText"
        Me.txtFilesText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtFilesText.Size = New System.Drawing.Size(59, 23)
        Me.txtFilesText.TabIndex = 83
        Me.txtFilesText.Tag = "23"
        Me.ToolTip1.SetToolTip(Me.txtFilesText, "This is the files content for the third Discord post.")
        Me.txtFilesText.Visible = False
        '
        'txtWeatherWinds
        '
        Me.txtWeatherWinds.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherWinds.Location = New System.Drawing.Point(1124, 403)
        Me.txtWeatherWinds.Multiline = True
        Me.txtWeatherWinds.Name = "txtWeatherWinds"
        Me.txtWeatherWinds.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWeatherWinds.Size = New System.Drawing.Size(59, 23)
        Me.txtWeatherWinds.TabIndex = 2
        Me.txtWeatherWinds.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtWeatherWinds, "This is the wind layers content for the second Discord post.")
        Me.txtWeatherWinds.Visible = False
        '
        'txtFPResults
        '
        Me.txtFPResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFPResults.Location = New System.Drawing.Point(1124, 315)
        Me.txtFPResults.Multiline = True
        Me.txtFPResults.Name = "txtFPResults"
        Me.txtFPResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtFPResults.Size = New System.Drawing.Size(59, 23)
        Me.txtFPResults.TabIndex = 79
        Me.txtFPResults.Tag = "21"
        Me.ToolTip1.SetToolTip(Me.txtFPResults, "This is the content of the main Discord post for the flight plan.")
        Me.txtFPResults.Visible = False
        '
        'txtWeatherClouds
        '
        Me.txtWeatherClouds.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherClouds.Location = New System.Drawing.Point(1124, 432)
        Me.txtWeatherClouds.Multiline = True
        Me.txtWeatherClouds.Name = "txtWeatherClouds"
        Me.txtWeatherClouds.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWeatherClouds.Size = New System.Drawing.Size(59, 23)
        Me.txtWeatherClouds.TabIndex = 3
        Me.txtWeatherClouds.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtWeatherClouds, "This is the cloud layers content for the second Discord post.")
        Me.txtWeatherClouds.Visible = False
        '
        'lblNbrCarsWeatherInfo
        '
        Me.lblNbrCarsWeatherInfo.AutoSize = True
        Me.lblNbrCarsWeatherInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsWeatherInfo.Location = New System.Drawing.Point(1202, 371)
        Me.lblNbrCarsWeatherInfo.Name = "lblNbrCarsWeatherInfo"
        Me.lblNbrCarsWeatherInfo.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsWeatherInfo.TabIndex = 11
        Me.lblNbrCarsWeatherInfo.Text = "0"
        Me.lblNbrCarsWeatherInfo.Visible = False
        '
        'txtAltRestrictions
        '
        Me.txtAltRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAltRestrictions.Location = New System.Drawing.Point(1124, 345)
        Me.txtAltRestrictions.Multiline = True
        Me.txtAltRestrictions.Name = "txtAltRestrictions"
        Me.txtAltRestrictions.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtAltRestrictions.Size = New System.Drawing.Size(59, 23)
        Me.txtAltRestrictions.TabIndex = 0
        Me.txtAltRestrictions.Tag = "22"
        Me.ToolTip1.SetToolTip(Me.txtAltRestrictions, "This is the altitude restrictions content for the second Discord post.")
        Me.txtAltRestrictions.Visible = False
        '
        'lblNbrCarsWeatherWinds
        '
        Me.lblNbrCarsWeatherWinds.AutoSize = True
        Me.lblNbrCarsWeatherWinds.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsWeatherWinds.Location = New System.Drawing.Point(1202, 400)
        Me.lblNbrCarsWeatherWinds.Name = "lblNbrCarsWeatherWinds"
        Me.lblNbrCarsWeatherWinds.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsWeatherWinds.TabIndex = 12
        Me.lblNbrCarsWeatherWinds.Text = "0"
        Me.lblNbrCarsWeatherWinds.Visible = False
        '
        'lblNbrCarsFilesText
        '
        Me.lblNbrCarsFilesText.AutoSize = True
        Me.lblNbrCarsFilesText.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNbrCarsFilesText.Location = New System.Drawing.Point(1202, 455)
        Me.lblNbrCarsFilesText.Name = "lblNbrCarsFilesText"
        Me.lblNbrCarsFilesText.Size = New System.Drawing.Size(22, 26)
        Me.lblNbrCarsFilesText.TabIndex = 76
        Me.lblNbrCarsFilesText.Text = "0"
        Me.lblNbrCarsFilesText.Visible = False
        '
        'grpDiscordGroupFlight
        '
        Me.grpDiscordGroupFlight.Controls.Add(Me.grpTaskFeatured)
        Me.grpDiscordGroupFlight.Controls.Add(Me.grpGroupFlightEvent)
        Me.grpDiscordGroupFlight.Controls.Add(Me.grpDiscordEvent)
        Me.grpDiscordGroupFlight.Location = New System.Drawing.Point(419, 3)
        Me.grpDiscordGroupFlight.Name = "grpDiscordGroupFlight"
        Me.grpDiscordGroupFlight.Size = New System.Drawing.Size(685, 755)
        Me.grpDiscordGroupFlight.TabIndex = 1
        Me.grpDiscordGroupFlight.TabStop = False
        Me.grpDiscordGroupFlight.Text = "Group Flight Event"
        '
        'grpTaskFeatured
        '
        Me.grpTaskFeatured.Controls.Add(Me.btnTaskFeaturedOnGroupFlight)
        Me.grpTaskFeatured.Location = New System.Drawing.Point(6, 663)
        Me.grpTaskFeatured.Name = "grpTaskFeatured"
        Me.grpTaskFeatured.Size = New System.Drawing.Size(673, 84)
        Me.grpTaskFeatured.TabIndex = 2
        Me.grpTaskFeatured.TabStop = False
        Me.grpTaskFeatured.Text = "Step 3: Sharing the event"
        '
        'btnTaskFeaturedOnGroupFlight
        '
        Me.btnTaskFeaturedOnGroupFlight.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTaskFeaturedOnGroupFlight.Location = New System.Drawing.Point(6, 26)
        Me.btnTaskFeaturedOnGroupFlight.Name = "btnTaskFeaturedOnGroupFlight"
        Me.btnTaskFeaturedOnGroupFlight.Size = New System.Drawing.Size(661, 51)
        Me.btnTaskFeaturedOnGroupFlight.TabIndex = 2
        Me.btnTaskFeaturedOnGroupFlight.Tag = "95"
        Me.btnTaskFeaturedOnGroupFlight.Text = "1. ""Task featured on group flight"" to clipboard"
        Me.ToolTip1.SetToolTip(Me.btnTaskFeaturedOnGroupFlight, "Click this button to copy the message to post on the task and receive instruction" &
        "s to paste it in the Discord.")
        Me.btnTaskFeaturedOnGroupFlight.UseVisualStyleBackColor = True
        '
        'grpGroupFlightEvent
        '
        Me.grpGroupFlightEvent.Controls.Add(Me.btnEventDPHXAndLinkOnly)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnEventTaskDetails)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnGroupFlightEventThreadLogistics)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnGroupFlightEventTeaser)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnDiscordGroupEventURL)
        Me.grpGroupFlightEvent.Controls.Add(Me.txtGroupEventPostURL)
        Me.grpGroupFlightEvent.Controls.Add(Me.Label38)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnEventFilesAndFilesInfo)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnGroupFlightEventInfoToClipboard)
        Me.grpGroupFlightEvent.Location = New System.Drawing.Point(6, 27)
        Me.grpGroupFlightEvent.Name = "grpGroupFlightEvent"
        Me.grpGroupFlightEvent.Size = New System.Drawing.Size(673, 293)
        Me.grpGroupFlightEvent.TabIndex = 0
        Me.grpGroupFlightEvent.TabStop = False
        Me.grpGroupFlightEvent.Text = "Step 1: Group Event Post"
        '
        'btnEventDPHXAndLinkOnly
        '
        Me.btnEventDPHXAndLinkOnly.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventDPHXAndLinkOnly.Location = New System.Drawing.Point(436, 150)
        Me.btnEventDPHXAndLinkOnly.Name = "btnEventDPHXAndLinkOnly"
        Me.btnEventDPHXAndLinkOnly.Size = New System.Drawing.Size(231, 37)
        Me.btnEventDPHXAndLinkOnly.TabIndex = 7
        Me.btnEventDPHXAndLinkOnly.Tag = "83"
        Me.btnEventDPHXAndLinkOnly.Text = "3b. Thread - DPHX and link"
        Me.ToolTip1.SetToolTip(Me.btnEventDPHXAndLinkOnly, "Click this button to only include the DPHX file and link to task post.")
        Me.btnEventDPHXAndLinkOnly.UseVisualStyleBackColor = True
        '
        'btnEventTaskDetails
        '
        Me.btnEventTaskDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventTaskDetails.Location = New System.Drawing.Point(6, 192)
        Me.btnEventTaskDetails.Name = "btnEventTaskDetails"
        Me.btnEventTaskDetails.Size = New System.Drawing.Size(661, 37)
        Me.btnEventTaskDetails.TabIndex = 8
        Me.btnEventTaskDetails.Tag = "84"
        Me.btnEventTaskDetails.Text = "4. Thread - Relevant task details"
        Me.ToolTip1.SetToolTip(Me.btnEventTaskDetails, "Click this button to copy the relevant task information to be posted next.")
        Me.btnEventTaskDetails.UseVisualStyleBackColor = True
        '
        'btnGroupFlightEventThreadLogistics
        '
        Me.btnGroupFlightEventThreadLogistics.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGroupFlightEventThreadLogistics.Location = New System.Drawing.Point(6, 235)
        Me.btnGroupFlightEventThreadLogistics.Name = "btnGroupFlightEventThreadLogistics"
        Me.btnGroupFlightEventThreadLogistics.Size = New System.Drawing.Size(661, 37)
        Me.btnGroupFlightEventThreadLogistics.TabIndex = 9
        Me.btnGroupFlightEventThreadLogistics.Tag = "85"
        Me.btnGroupFlightEventThreadLogistics.Text = "5. Thread - Logistic instructions"
        Me.ToolTip1.SetToolTip(Me.btnGroupFlightEventThreadLogistics, "Click this button to copy the thread logistic instructions to your clipboard.")
        Me.btnGroupFlightEventThreadLogistics.UseVisualStyleBackColor = True
        '
        'btnGroupFlightEventTeaser
        '
        Me.btnGroupFlightEventTeaser.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGroupFlightEventTeaser.Location = New System.Drawing.Point(6, 106)
        Me.btnGroupFlightEventTeaser.Name = "btnGroupFlightEventTeaser"
        Me.btnGroupFlightEventTeaser.Size = New System.Drawing.Size(661, 37)
        Me.btnGroupFlightEventTeaser.TabIndex = 5
        Me.btnGroupFlightEventTeaser.Tag = "82"
        Me.btnGroupFlightEventTeaser.Text = "2. Thread - Group event Teaser section"
        Me.ToolTip1.SetToolTip(Me.btnGroupFlightEventTeaser, "Click this button to post the teaser section.")
        Me.btnGroupFlightEventTeaser.UseVisualStyleBackColor = True
        '
        'btnDiscordGroupEventURL
        '
        Me.btnDiscordGroupEventURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordGroupEventURL.Location = New System.Drawing.Point(587, 69)
        Me.btnDiscordGroupEventURL.Name = "btnDiscordGroupEventURL"
        Me.btnDiscordGroupEventURL.Size = New System.Drawing.Size(80, 29)
        Me.btnDiscordGroupEventURL.TabIndex = 4
        Me.btnDiscordGroupEventURL.Tag = "81"
        Me.btnDiscordGroupEventURL.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnDiscordGroupEventURL, "Click this button to paste the group event's post URL from your clipboard")
        Me.btnDiscordGroupEventURL.UseVisualStyleBackColor = True
        '
        'txtGroupEventPostURL
        '
        Me.txtGroupEventPostURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGroupEventPostURL.Location = New System.Drawing.Point(192, 68)
        Me.txtGroupEventPostURL.Name = "txtGroupEventPostURL"
        Me.txtGroupEventPostURL.Size = New System.Drawing.Size(389, 32)
        Me.txtGroupEventPostURL.TabIndex = 3
        Me.txtGroupEventPostURL.Tag = "81"
        Me.ToolTip1.SetToolTip(Me.txtGroupEventPostURL, "Enter the URL to the Discord post created above in step 1.")
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label38.Location = New System.Drawing.Point(6, 71)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(173, 26)
        Me.Label38.TabIndex = 2
        Me.Label38.Text = "URL to group event"
        '
        'btnEventFilesAndFilesInfo
        '
        Me.btnEventFilesAndFilesInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventFilesAndFilesInfo.Location = New System.Drawing.Point(6, 149)
        Me.btnEventFilesAndFilesInfo.Name = "btnEventFilesAndFilesInfo"
        Me.btnEventFilesAndFilesInfo.Size = New System.Drawing.Size(424, 37)
        Me.btnEventFilesAndFilesInfo.TabIndex = 6
        Me.btnEventFilesAndFilesInfo.Tag = "83"
        Me.btnEventFilesAndFilesInfo.Text = "3. Thread - Files and files info"
        Me.ToolTip1.SetToolTip(Me.btnEventFilesAndFilesInfo, "Click this button to first paste the files and then files legend information.")
        Me.btnEventFilesAndFilesInfo.UseVisualStyleBackColor = True
        '
        'btnGroupFlightEventInfoToClipboard
        '
        Me.btnGroupFlightEventInfoToClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGroupFlightEventInfoToClipboard.Location = New System.Drawing.Point(6, 25)
        Me.btnGroupFlightEventInfoToClipboard.Name = "btnGroupFlightEventInfoToClipboard"
        Me.btnGroupFlightEventInfoToClipboard.Size = New System.Drawing.Size(661, 37)
        Me.btnGroupFlightEventInfoToClipboard.TabIndex = 0
        Me.btnGroupFlightEventInfoToClipboard.Tag = "80"
        Me.btnGroupFlightEventInfoToClipboard.Text = "1. Group event post info to clipboard then thread title"
        Me.ToolTip1.SetToolTip(Me.btnGroupFlightEventInfoToClipboard, "Click this button to put the group flight info into your clipboard.")
        Me.btnGroupFlightEventInfoToClipboard.UseVisualStyleBackColor = True
        '
        'grpDiscordEvent
        '
        Me.grpDiscordEvent.Controls.Add(Me.btnDiscordSharedEventURL)
        Me.grpDiscordEvent.Controls.Add(Me.txtDiscordEventShareURL)
        Me.grpDiscordEvent.Controls.Add(Me.Label20)
        Me.grpDiscordEvent.Controls.Add(Me.Label6)
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
        Me.grpDiscordEvent.Location = New System.Drawing.Point(6, 326)
        Me.grpDiscordEvent.Name = "grpDiscordEvent"
        Me.grpDiscordEvent.Size = New System.Drawing.Size(673, 331)
        Me.grpDiscordEvent.TabIndex = 1
        Me.grpDiscordEvent.TabStop = False
        Me.grpDiscordEvent.Text = "Step 2: Discord Event (if applicable)"
        '
        'btnDiscordSharedEventURL
        '
        Me.btnDiscordSharedEventURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordSharedEventURL.Location = New System.Drawing.Point(587, 289)
        Me.btnDiscordSharedEventURL.Name = "btnDiscordSharedEventURL"
        Me.btnDiscordSharedEventURL.Size = New System.Drawing.Size(80, 29)
        Me.btnDiscordSharedEventURL.TabIndex = 16
        Me.btnDiscordSharedEventURL.Tag = "91"
        Me.btnDiscordSharedEventURL.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnDiscordSharedEventURL, "Click this button to paste the group event's post URL from your clipboard")
        Me.btnDiscordSharedEventURL.UseVisualStyleBackColor = True
        '
        'txtDiscordEventShareURL
        '
        Me.txtDiscordEventShareURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordEventShareURL.Location = New System.Drawing.Point(160, 288)
        Me.txtDiscordEventShareURL.Name = "txtDiscordEventShareURL"
        Me.txtDiscordEventShareURL.Size = New System.Drawing.Size(421, 32)
        Me.txtDiscordEventShareURL.TabIndex = 15
        Me.txtDiscordEventShareURL.Tag = "91"
        Me.ToolTip1.SetToolTip(Me.txtDiscordEventShareURL, "Enter the URL to the Discord post created above in step 1.")
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(1, 291)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(153, 26)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "URL link to share:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(1, 26)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(429, 26)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "1. Create a new Discord Event on the proper server"
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label46.Location = New System.Drawing.Point(1, 249)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(562, 26)
        Me.Label46.TabIndex = 13
        Me.Label46.Text = "7. Preview and post your Discord Event! And copy the link to share."
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label45.Location = New System.Drawing.Point(1, 210)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(521, 26)
        Me.Label45.TabIndex = 12
        Me.Label45.Text = "6. Upload optional cover image (min. 800px wide by 320px tall)"
        '
        'btnEventDescriptionToClipboard
        '
        Me.btnEventDescriptionToClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventDescriptionToClipboard.Location = New System.Drawing.Point(234, 171)
        Me.btnEventDescriptionToClipboard.Name = "btnEventDescriptionToClipboard"
        Me.btnEventDescriptionToClipboard.Size = New System.Drawing.Size(337, 29)
        Me.btnEventDescriptionToClipboard.TabIndex = 11
        Me.btnEventDescriptionToClipboard.Tag = "88"
        Me.btnEventDescriptionToClipboard.Text = "Event Description to Clipboard"
        Me.ToolTip1.SetToolTip(Me.btnEventDescriptionToClipboard, "Click this button to copy the event's full description for the Discord Event post" &
        " into the clipboard.")
        Me.btnEventDescriptionToClipboard.UseVisualStyleBackColor = True
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label44.Location = New System.Drawing.Point(1, 173)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(225, 26)
        Me.Label44.TabIndex = 10
        Me.Label44.Text = "5. Enter Event Description"
        '
        'lblDiscordPostDateTime
        '
        Me.lblDiscordPostDateTime.AutoSize = True
        Me.lblDiscordPostDateTime.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordPostDateTime.Location = New System.Drawing.Point(307, 135)
        Me.lblDiscordPostDateTime.Name = "lblDiscordPostDateTime"
        Me.lblDiscordPostDateTime.Size = New System.Drawing.Size(157, 26)
        Me.lblDiscordPostDateTime.TabIndex = 9
        Me.lblDiscordPostDateTime.Text = "meet time results"
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label43.Location = New System.Drawing.Point(1, 135)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(298, 26)
        Me.Label43.TabIndex = 8
        Me.Label43.Text = "4. Specify local start date and time:"
        '
        'btnEventTopicClipboard
        '
        Me.btnEventTopicClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventTopicClipboard.Location = New System.Drawing.Point(234, 98)
        Me.btnEventTopicClipboard.Name = "btnEventTopicClipboard"
        Me.btnEventTopicClipboard.Size = New System.Drawing.Size(337, 29)
        Me.btnEventTopicClipboard.TabIndex = 7
        Me.btnEventTopicClipboard.Tag = "87"
        Me.btnEventTopicClipboard.Text = "Event Topic to Clipboard"
        Me.ToolTip1.SetToolTip(Me.btnEventTopicClipboard, "Click this button to copy the event's topic for the Discord Event post into the c" &
        "lipboard.")
        Me.btnEventTopicClipboard.UseVisualStyleBackColor = True
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label42.Location = New System.Drawing.Point(1, 100)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(172, 26)
        Me.Label42.TabIndex = 6
        Me.Label42.Text = "3. Enter Event Topic"
        '
        'lblDiscordEventVoice
        '
        Me.lblDiscordEventVoice.AutoSize = True
        Me.lblDiscordEventVoice.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordEventVoice.Location = New System.Drawing.Point(350, 64)
        Me.lblDiscordEventVoice.Name = "lblDiscordEventVoice"
        Me.lblDiscordEventVoice.Size = New System.Drawing.Size(126, 26)
        Me.lblDiscordEventVoice.TabIndex = 5
        Me.lblDiscordEventVoice.Text = "voice channel"
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label39.Location = New System.Drawing.Point(1, 64)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(326, 26)
        Me.Label39.TabIndex = 4
        Me.Label39.Text = "2. Select Voice Channel and click next:"
        '
        'chkExpertMode
        '
        Me.chkExpertMode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkExpertMode.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkExpertMode.Location = New System.Drawing.Point(1110, 3)
        Me.chkExpertMode.Name = "chkExpertMode"
        Me.chkExpertMode.Size = New System.Drawing.Size(373, 52)
        Me.chkExpertMode.TabIndex = 95
        Me.chkExpertMode.Tag = "44"
        Me.chkExpertMode.Text = "Expert Mode (auto click next step)"
        Me.ToolTip1.SetToolTip(Me.chkExpertMode, "Check this if you want the app to auto click on the next step (faster)")
        Me.chkExpertMode.UseVisualStyleBackColor = True
        '
        'tabBriefing
        '
        Me.tabBriefing.Controls.Add(Me.pnlBriefing)
        Me.tabBriefing.Location = New System.Drawing.Point(4, 29)
        Me.tabBriefing.Name = "tabBriefing"
        Me.tabBriefing.Size = New System.Drawing.Size(1481, 859)
        Me.tabBriefing.TabIndex = 2
        Me.tabBriefing.Text = "Briefing"
        Me.tabBriefing.UseVisualStyleBackColor = True
        '
        'pnlBriefing
        '
        Me.pnlBriefing.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlBriefing.Controls.Add(Me.pnlWizardBriefing)
        Me.pnlBriefing.Controls.Add(Me.BriefingControl1)
        Me.pnlBriefing.Location = New System.Drawing.Point(0, 0)
        Me.pnlBriefing.Name = "pnlBriefing"
        Me.pnlBriefing.Size = New System.Drawing.Size(1481, 867)
        Me.pnlBriefing.TabIndex = 0
        '
        'pnlWizardBriefing
        '
        Me.pnlWizardBriefing.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlWizardBriefing.BackColor = System.Drawing.Color.Gray
        Me.pnlWizardBriefing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlWizardBriefing.Controls.Add(Me.lblBriefingGuideInstructions)
        Me.pnlWizardBriefing.Controls.Add(Me.btnBriefingGuideNext)
        Me.pnlWizardBriefing.Location = New System.Drawing.Point(982, 0)
        Me.pnlWizardBriefing.Name = "pnlWizardBriefing"
        Me.pnlWizardBriefing.Size = New System.Drawing.Size(498, 46)
        Me.pnlWizardBriefing.TabIndex = 95
        Me.pnlWizardBriefing.Visible = False
        '
        'lblBriefingGuideInstructions
        '
        Me.lblBriefingGuideInstructions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBriefingGuideInstructions.Font = New System.Drawing.Font("Segoe UI Variable Display Semib", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBriefingGuideInstructions.ForeColor = System.Drawing.Color.White
        Me.lblBriefingGuideInstructions.Location = New System.Drawing.Point(0, 0)
        Me.lblBriefingGuideInstructions.Name = "lblBriefingGuideInstructions"
        Me.lblBriefingGuideInstructions.Size = New System.Drawing.Size(425, 46)
        Me.lblBriefingGuideInstructions.TabIndex = 1
        Me.lblBriefingGuideInstructions.Text = "Review the task and event information on the briefing tabs here and when you are " &
    "satisfied, click Next."
        Me.lblBriefingGuideInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnBriefingGuideNext
        '
        Me.btnBriefingGuideNext.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnBriefingGuideNext.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBriefingGuideNext.Location = New System.Drawing.Point(425, 0)
        Me.btnBriefingGuideNext.Name = "btnBriefingGuideNext"
        Me.btnBriefingGuideNext.Size = New System.Drawing.Size(73, 46)
        Me.btnBriefingGuideNext.TabIndex = 0
        Me.btnBriefingGuideNext.Text = "Next"
        Me.ToolTip1.SetToolTip(Me.btnBriefingGuideNext, "Click here to go to the next step in the guide.")
        Me.btnBriefingGuideNext.UseVisualStyleBackColor = True
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
        Me.BriefingControl1.Size = New System.Drawing.Size(1481, 867)
        Me.BriefingControl1.TabIndex = 0
        Me.BriefingControl1.Tag = "100"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ClickThrough = True
        Me.ToolStrip1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripOpen, Me.toolStripSave, Me.toolStripReload, Me.toolStripResetAll, Me.ToolStripSeparator1, Me.toolStripDiscordTaskLibrary, Me.ToolStripSeparator4, Me.toolStripB21Planner, Me.ToolStripSeparator2, Me.toolStripSharePackage, Me.ToolStripSeparator3, Me.toolStripGuideMe, Me.toolStripStopGuide, Me.ToolStripDropDownButton1, Me.toolStripCurrentDateTime})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1494, 28)
        Me.ToolStrip1.SuppressHighlighting = False
        Me.ToolStrip1.TabIndex = 7
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'toolStripOpen
        '
        Me.toolStripOpen.Image = CType(resources.GetObject("toolStripOpen.Image"), System.Drawing.Image)
        Me.toolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripOpen.Name = "toolStripOpen"
        Me.toolStripOpen.Size = New System.Drawing.Size(70, 25)
        Me.toolStripOpen.Text = "&Open"
        Me.toolStripOpen.ToolTipText = "Click to select and load a DPH session file from your PC."
        '
        'toolStripSave
        '
        Me.toolStripSave.Image = CType(resources.GetObject("toolStripSave.Image"), System.Drawing.Image)
        Me.toolStripSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripSave.Name = "toolStripSave"
        Me.toolStripSave.Size = New System.Drawing.Size(65, 25)
        Me.toolStripSave.Text = "&Save"
        Me.toolStripSave.ToolTipText = "Click to save the current DPH session to your PC."
        '
        'toolStripReload
        '
        Me.toolStripReload.Image = CType(resources.GetObject("toolStripReload.Image"), System.Drawing.Image)
        Me.toolStripReload.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripReload.Name = "toolStripReload"
        Me.toolStripReload.Size = New System.Drawing.Size(84, 25)
        Me.toolStripReload.Text = "Discard"
        Me.toolStripReload.ToolTipText = "Click here to discard changes and reload the current DPH session file."
        Me.toolStripReload.Visible = False
        '
        'toolStripResetAll
        '
        Me.toolStripResetAll.Image = CType(resources.GetObject("toolStripResetAll.Image"), System.Drawing.Image)
        Me.toolStripResetAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripResetAll.Name = "toolStripResetAll"
        Me.toolStripResetAll.Size = New System.Drawing.Size(92, 25)
        Me.toolStripResetAll.Text = "&Reset All"
        Me.toolStripResetAll.ToolTipText = "Click to reset ALL of the fiels and start from scratch."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 28)
        '
        'toolStripDiscordTaskLibrary
        '
        Me.toolStripDiscordTaskLibrary.Image = CType(resources.GetObject("toolStripDiscordTaskLibrary.Image"), System.Drawing.Image)
        Me.toolStripDiscordTaskLibrary.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripDiscordTaskLibrary.Name = "toolStripDiscordTaskLibrary"
        Me.toolStripDiscordTaskLibrary.Size = New System.Drawing.Size(114, 25)
        Me.toolStripDiscordTaskLibrary.Text = "Task &Library"
        Me.toolStripDiscordTaskLibrary.ToolTipText = "Click here to open the Task Library on Discord."
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 28)
        '
        'toolStripB21Planner
        '
        Me.toolStripB21Planner.Image = CType(resources.GetObject("toolStripB21Planner.Image"), System.Drawing.Image)
        Me.toolStripB21Planner.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripB21Planner.Name = "toolStripB21Planner"
        Me.toolStripB21Planner.Size = New System.Drawing.Size(116, 25)
        Me.toolStripB21Planner.Text = "&B21 Planner"
        Me.toolStripB21Planner.ToolTipText = "Click to open the B21 Planner in your browser."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 28)
        '
        'toolStripSharePackage
        '
        Me.toolStripSharePackage.Image = CType(resources.GetObject("toolStripSharePackage.Image"), System.Drawing.Image)
        Me.toolStripSharePackage.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripSharePackage.Name = "toolStripSharePackage"
        Me.toolStripSharePackage.Size = New System.Drawing.Size(132, 25)
        Me.toolStripSharePackage.Text = "Share &Package"
        Me.toolStripSharePackage.ToolTipText = "Click to create a shareable package with all files."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 28)
        '
        'toolStripGuideMe
        '
        Me.toolStripGuideMe.Image = CType(resources.GetObject("toolStripGuideMe.Image"), System.Drawing.Image)
        Me.toolStripGuideMe.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripGuideMe.Name = "toolStripGuideMe"
        Me.toolStripGuideMe.Size = New System.Drawing.Size(183, 25)
        Me.toolStripGuideMe.Text = "&Guide me please! (F1)"
        Me.toolStripGuideMe.ToolTipText = "Click to activate wizard"
        '
        'toolStripStopGuide
        '
        Me.toolStripStopGuide.Image = CType(resources.GetObject("toolStripStopGuide.Image"), System.Drawing.Image)
        Me.toolStripStopGuide.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripStopGuide.Name = "toolStripStopGuide"
        Me.toolStripStopGuide.Size = New System.Drawing.Size(130, 25)
        Me.toolStripStopGuide.Text = "&Turn guide off"
        Me.toolStripStopGuide.ToolTipText = "Click to disable wizard"
        Me.toolStripStopGuide.Visible = False
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DiscordChannelToolStripMenuItem, Me.GoToFeedbackChannelOnDiscordToolStripMenuItem, Me.DiscordInviteToolStripMenuItem})
        Me.ToolStripDropDownButton1.Image = CType(resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(147, 25)
        Me.ToolStripDropDownButton1.Text = "&I need support!"
        Me.ToolStripDropDownButton1.ToolTipText = "Click here to view all support options"
        '
        'DiscordChannelToolStripMenuItem
        '
        Me.DiscordChannelToolStripMenuItem.Name = "DiscordChannelToolStripMenuItem"
        Me.DiscordChannelToolStripMenuItem.Size = New System.Drawing.Size(345, 26)
        Me.DiscordChannelToolStripMenuItem.Text = "&1. Go to support channel on Discord"
        '
        'GoToFeedbackChannelOnDiscordToolStripMenuItem
        '
        Me.GoToFeedbackChannelOnDiscordToolStripMenuItem.Name = "GoToFeedbackChannelOnDiscordToolStripMenuItem"
        Me.GoToFeedbackChannelOnDiscordToolStripMenuItem.Size = New System.Drawing.Size(345, 26)
        Me.GoToFeedbackChannelOnDiscordToolStripMenuItem.Text = "&2. Go to feedback channel on Discord"
        '
        'DiscordInviteToolStripMenuItem
        '
        Me.DiscordInviteToolStripMenuItem.Name = "DiscordInviteToolStripMenuItem"
        Me.DiscordInviteToolStripMenuItem.Size = New System.Drawing.Size(345, 26)
        Me.DiscordInviteToolStripMenuItem.Text = "&3. Copy Discord invite link"
        '
        'toolStripCurrentDateTime
        '
        Me.toolStripCurrentDateTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.toolStripCurrentDateTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.toolStripCurrentDateTime.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetNowTimeOnlyWithoutSeconds, Me.GetNowFullWithDayOfWeek, Me.GetNowLongDateTime, Me.GetNowCountdown, Me.GetNowTimeStampOnly})
        Me.toolStripCurrentDateTime.Image = CType(resources.GetObject("toolStripCurrentDateTime.Image"), System.Drawing.Image)
        Me.toolStripCurrentDateTime.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripCurrentDateTime.Name = "toolStripCurrentDateTime"
        Me.toolStripCurrentDateTime.Size = New System.Drawing.Size(143, 25)
        Me.toolStripCurrentDateTime.Text = "CurrentDateTime"
        Me.toolStripCurrentDateTime.ToolTipText = "Click for UNIX timestamp options"
        '
        'GetNowTimeOnlyWithoutSeconds
        '
        Me.GetNowTimeOnlyWithoutSeconds.Name = "GetNowTimeOnlyWithoutSeconds"
        Me.GetNowTimeOnlyWithoutSeconds.Size = New System.Drawing.Size(269, 26)
        Me.GetNowTimeOnlyWithoutSeconds.Text = "TimeOnlyWithoutSeconds"
        '
        'GetNowFullWithDayOfWeek
        '
        Me.GetNowFullWithDayOfWeek.Name = "GetNowFullWithDayOfWeek"
        Me.GetNowFullWithDayOfWeek.Size = New System.Drawing.Size(269, 26)
        Me.GetNowFullWithDayOfWeek.Text = "FullWithDayOfWeek"
        '
        'GetNowLongDateTime
        '
        Me.GetNowLongDateTime.Name = "GetNowLongDateTime"
        Me.GetNowLongDateTime.Size = New System.Drawing.Size(269, 26)
        Me.GetNowLongDateTime.Text = "LongDateTime"
        '
        'GetNowCountdown
        '
        Me.GetNowCountdown.Name = "GetNowCountdown"
        Me.GetNowCountdown.Size = New System.Drawing.Size(269, 26)
        Me.GetNowCountdown.Text = "Countdown"
        '
        'GetNowTimeStampOnly
        '
        Me.GetNowTimeStampOnly.Name = "GetNowTimeStampOnly"
        Me.GetNowTimeStampOnly.Size = New System.Drawing.Size(269, 26)
        Me.GetNowTimeStampOnly.Text = "TimestampOnly"
        '
        'OneMinuteTimer
        '
        Me.OneMinuteTimer.Enabled = True
        Me.OneMinuteTimer.Interval = 1000
        '
        'pnlEventDateTimeControls
        '
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventMeetDate)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventMeetTime)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventSyncFlyDate)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventSyncFlyTime)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventLaunchDate)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventLaunchTime)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventStartTaskDate)
        Me.pnlEventDateTimeControls.Controls.Add(Me.dtEventStartTaskTime)
        Me.pnlEventDateTimeControls.Controls.Add(Me.lblMeetTimeResult)
        Me.pnlEventDateTimeControls.Controls.Add(Me.lblSyncTimeResult)
        Me.pnlEventDateTimeControls.Controls.Add(Me.lblLaunchTimeResult)
        Me.pnlEventDateTimeControls.Controls.Add(Me.lblStartTimeResult)
        Me.pnlEventDateTimeControls.Location = New System.Drawing.Point(190, 299)
        Me.pnlEventDateTimeControls.MaximumSize = New System.Drawing.Size(663, 139)
        Me.pnlEventDateTimeControls.MinimumSize = New System.Drawing.Size(663, 139)
        Me.pnlEventDateTimeControls.Name = "pnlEventDateTimeControls"
        Me.pnlEventDateTimeControls.Size = New System.Drawing.Size(663, 139)
        Me.pnlEventDateTimeControls.TabIndex = 86
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1494, 923)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.pnlScrollableSurface)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(1512, 967)
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
        Me.grbTaskPart2.ResumeLayout(False)
        Me.grbTaskPart2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.grbTaskDiscord.ResumeLayout(False)
        Me.grbTaskDiscord.PerformLayout()
        Me.tabEvent.ResumeLayout(False)
        Me.tabEvent.PerformLayout()
        Me.pnlWizardEvent.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.grpGroupEventPost.ResumeLayout(False)
        Me.grpGroupEventPost.PerformLayout()
        Me.grpEventTeaser.ResumeLayout(False)
        Me.grpEventTeaser.PerformLayout()
        Me.TimeStampContextualMenu.ResumeLayout(False)
        Me.tabDiscord.ResumeLayout(False)
        Me.tabDiscord.PerformLayout()
        Me.pnlWizardDiscord.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.grpDiscordTask.ResumeLayout(False)
        Me.grpRepost.ResumeLayout(False)
        Me.grpRepost.PerformLayout()
        Me.grpDiscordTaskThread.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.grpDiscordGroupFlight.ResumeLayout(False)
        Me.grpTaskFeatured.ResumeLayout(False)
        Me.grpGroupFlightEvent.ResumeLayout(False)
        Me.grpGroupFlightEvent.PerformLayout()
        Me.grpDiscordEvent.ResumeLayout(False)
        Me.grpDiscordEvent.PerformLayout()
        Me.tabBriefing.ResumeLayout(False)
        Me.pnlBriefing.ResumeLayout(False)
        Me.pnlWizardBriefing.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.pnlEventDateTimeControls.ResumeLayout(False)
        Me.pnlEventDateTimeControls.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

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
    Friend WithEvents lblSoaringType As Label
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
    Friend WithEvents lblDeparture As Label
    Friend WithEvents txtMainArea As TextBox
    Friend WithEvents lblMainAreaPOI As Label
    Friend WithEvents txtTitle As TextBox
    Friend WithEvents lblTitle As Label
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
    Friend WithEvents lblDuration As Label
    Friend WithEvents txtDurationMax As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtDurationExtraInfo As TextBox
    Friend WithEvents lblRecommendedGliders As Label
    Friend WithEvents txtCredits As TextBox
    Friend WithEvents lblDifficultyRating As Label
    Friend WithEvents lblCredits As Label
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
    Friend WithEvents grpDiscordEvent As GroupBox
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
    Friend WithEvents btnGroupFlightEventInfoToClipboard As Button
    Friend WithEvents txtGroupFlightEventPost As TextBox
    Friend WithEvents lblEventTaskDistance As Label
    Friend WithEvents Label48 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents txtDiscordEventDescription As TextBox
    Friend WithEvents txtDiscordEventTopic As TextBox
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents btnEventFilesAndFilesInfo As Button
    Friend WithEvents pnlGuide As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents lblGuideInstructions As Label
    Friend WithEvents pnlArrow As Panel
    Friend WithEvents btnGuideNext As Button
    Friend WithEvents pnlWizardEvent As Panel
    Friend WithEvents btnEventGuideNext As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblEventGuideInstructions As Label
    Friend WithEvents pnlEventArrow As Panel
    Friend WithEvents dtEventMeetDate As DateTimePicker
    Friend WithEvents chkUseSyncFly As CheckBox
    Friend WithEvents tabBriefing As TabPage
    Friend WithEvents lblLocalDSTWarning As Label
    Friend WithEvents pnlBriefing As Panel
    Friend WithEvents BriefingControl1 As CommonLibrary.BriefingControl
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
    Friend WithEvents txtWeatherFirstPart As TextBox
    Friend WithEvents txtWeatherWinds As TextBox
    Friend WithEvents txtWeatherClouds As TextBox
    Friend WithEvents lblNbrCarsWeatherInfo As Label
    Friend WithEvents lblNbrCarsWeatherWinds As Label
    Friend WithEvents txtAltRestrictions As TextBox
    Friend WithEvents lblNbrCarsFilesText As Label
    Friend WithEvents btnAltRestricCopy As Button
    Friend WithEvents grbTaskPart2 As GroupBox
    Friend WithEvents chkLockCountries As CheckBox
    Friend WithEvents btnMoveCountryDown As Button
    Friend WithEvents btnMoveCountryUp As Button
    Friend WithEvents btnRemoveCountry As Button
    Friend WithEvents btnAddCountry As Button
    Friend WithEvents lstAllCountries As ListBox
    Friend WithEvents cboCountryFlag As ComboBox
    Friend WithEvents lblCountries As Label
    Friend WithEvents btnExtraFileDown As Button
    Friend WithEvents btnExtraFileUp As Button
    Friend WithEvents btnRemoveExtraFile As Button
    Friend WithEvents btnAddExtraFile As Button
    Friend WithEvents lstAllFiles As ListBox
    Friend WithEvents chkUseOnlyWeatherSummary As CheckBox
    Friend WithEvents txtWeatherSummary As TextBox
    Friend WithEvents lblWeatherSummary As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnFPMainInfoCopy As Button
    Friend WithEvents grpDiscordTask As GroupBox
    Friend WithEvents grpDiscordTaskThread As GroupBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents btnWaypointsCopy As Button
    Friend WithEvents btnAddOnsCopy As Button
    Friend WithEvents lblAllSecPostsTotalCars As Label
    Friend WithEvents grpDiscordGroupFlight As GroupBox
    Friend WithEvents grpGroupFlightEvent As GroupBox
    Friend WithEvents txtWaypointsDetails As TextBox
    Friend WithEvents txtAddOnsDetails As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents txtDPHXPackageFilename As TextBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lstAllRecommendedAddOns As ListBox
    Friend WithEvents btnAddOnDown As Button
    Friend WithEvents btnAddRecAddOn As Button
    Friend WithEvents btnAddOnUp As Button
    Friend WithEvents btnEditSelectedAddOn As Button
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents btnRemoveSelectedAddOns As Button
    Friend WithEvents pnlWizardDiscord As Panel
    Friend WithEvents btnDiscordGuideNext As Button
    Friend WithEvents Panel4 As Panel
    Friend WithEvents lblDiscordGuideInstructions As Label
    Friend WithEvents pnlDiscordArrow As Panel
    Friend WithEvents pnlWizardBriefing As Panel
    Friend WithEvents btnBriefingGuideNext As Button
    Friend WithEvents cboBriefingMap As ComboBox
    Friend WithEvents lblMap As Label
    Friend WithEvents lblBriefingGuideInstructions As Label
    Friend WithEvents btnDiscordSharedEventURL As Button
    Friend WithEvents txtDiscordEventShareURL As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents grpTaskFeatured As GroupBox
    Friend WithEvents btnTaskFeaturedOnGroupFlight As Button
    Friend WithEvents btnDiscordGroupEventURL As Button
    Friend WithEvents txtGroupEventPostURL As TextBox
    Friend WithEvents Label38 As Label
    Friend WithEvents Label30 As Label
    Friend WithEvents txtOtherBeginnerLink As TextBox
    Friend WithEvents cboBeginnersGuide As ComboBox
    Friend WithEvents btnPasteBeginnerLink As Button
    Friend WithEvents lblClubFullName As Label
    Friend WithEvents grbTaskDiscord As GroupBox
    Friend WithEvents txtDiscordTaskID As TextBox
    Friend WithEvents Label31 As Label
    Friend WithEvents btnDiscordTaskThreadURLPaste As Button
    Friend WithEvents btnPasteUsernameCredits As Button
    Friend WithEvents chkExpertMode As CheckBox
    Friend WithEvents FileDropZone1 As CommonLibrary.FileDropZone
    Friend WithEvents chkSoaringTypeWave As CheckBox
    Friend WithEvents chkSuppressWarningForBaroPressure As CheckBox
    Friend WithEvents txtBaroPressureExtraInfo As TextBox
    Friend WithEvents lblNonStdBaroPressure As Label
    Friend WithEvents btnGroupFlightEventThreadLogistics As Button
    Friend WithEvents btnGroupFlightEventTeaser As Button
    Friend WithEvents cboCoverImage As ComboBox
    Friend WithEvents chkLockCoverImage As CheckBox
    Friend WithEvents chkLockMapImage As CheckBox
    Friend WithEvents grpRepost As GroupBox
    Friend WithEvents chkRepost As CheckBox
    Friend WithEvents dtRepostOriginalDate As DateTimePicker
    Friend WithEvents chkSoaringTypeDynamic As CheckBox
    Friend WithEvents btnDeleteDiscordID As Button
    Friend WithEvents grpEventTeaser As GroupBox
    Friend WithEvents btnSelectEventTeaserAreaMap As Button
    Friend WithEvents btnClearEventTeaserAreaMap As Button
    Friend WithEvents txtEventTeaserAreaMapImage As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtEventTeaserMessage As TextBox
    Friend WithEvents chkEventTeaser As CheckBox
    Friend WithEvents btnEventTaskDetails As Button
    Friend WithEvents toolStripOpen As ToolStripButton
    Friend WithEvents toolStripSave As ToolStripButton
    Friend WithEvents toolStripResetAll As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents toolStripB21Planner As ToolStripButton
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents toolStripSharePackage As ToolStripButton
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents toolStripGuideMe As ToolStripButton
    Friend WithEvents toolStripStopGuide As ToolStripButton
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents DiscordChannelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DiscordInviteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GoToFeedbackChannelOnDiscordToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripDiscordTaskLibrary As ToolStripButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents toolStripReload As ToolStripButton
    Friend WithEvents ToolStrip1 As ToolStripExtensions.ToolStripExtended
    Friend WithEvents btnEventDPHXAndLinkOnly As Button
    Friend WithEvents lblElevationUpdateWarning As Label
    Friend WithEvents TimeStampContextualMenu As ContextMenuStrip
    Friend WithEvents GetTimeStampTimeOnlyWithoutSeconds As ToolStripMenuItem
    Friend WithEvents GetFullWithDayOfWeek As ToolStripMenuItem
    Friend WithEvents GetLongDateTime As ToolStripMenuItem
    Friend WithEvents GetCountdown As ToolStripMenuItem
    Friend WithEvents GetTimeStampOnly As ToolStripMenuItem
    Friend WithEvents OneMinuteTimer As Timer
    Friend WithEvents toolStripCurrentDateTime As ToolStripDropDownButton
    Friend WithEvents GetNowTimeOnlyWithoutSeconds As ToolStripMenuItem
    Friend WithEvents GetNowFullWithDayOfWeek As ToolStripMenuItem
    Friend WithEvents GetNowLongDateTime As ToolStripMenuItem
    Friend WithEvents GetNowCountdown As ToolStripMenuItem
    Friend WithEvents GetNowTimeStampOnly As ToolStripMenuItem
    Friend WithEvents pnlEventDateTimeControls As Panel
End Class
