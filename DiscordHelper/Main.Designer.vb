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
        Me.pnlGuide = New System.Windows.Forms.Panel()
        Me.btnGuideNext = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlArrow = New System.Windows.Forms.Panel()
        Me.pnlFlightPlanRightSide = New System.Windows.Forms.Panel()
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
        Me.txtWeatherSummary = New System.Windows.Forms.TextBox()
        Me.lblWeatherSummary = New System.Windows.Forms.Label()
        Me.grbTaskDiscord = New System.Windows.Forms.GroupBox()
        Me.btnDeleteDiscordID = New System.Windows.Forms.Button()
        Me.btnDiscordTaskThreadURLPaste = New System.Windows.Forms.Button()
        Me.txtDiscordTaskID = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.FlightPlanTabSplitter = New System.Windows.Forms.Splitter()
        Me.pnlFlightPlanLeftSide = New System.Windows.Forms.Panel()
        Me.lblElevationUpdateWarning = New System.Windows.Forms.Label()
        Me.grbTaskInfo = New System.Windows.Forms.GroupBox()
        Me.txtAATTask = New System.Windows.Forms.TextBox()
        Me.btnRecallTaskDescriptionTemplate = New System.Windows.Forms.Button()
        Me.btnSaveDescriptionTemplate = New System.Windows.Forms.Button()
        Me.btnWeatherBrowser = New System.Windows.Forms.Button()
        Me.btnSyncTitles = New System.Windows.Forms.Button()
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
        Me.cboKnownTaskDesigners = New System.Windows.Forms.ComboBox()
        Me.btnSelectFlightPlan = New System.Windows.Forms.Button()
        Me.txtFlightPlanFile = New System.Windows.Forms.TextBox()
        Me.tabEvent = New System.Windows.Forms.TabPage()
        Me.chkActivateEvent = New System.Windows.Forms.CheckBox()
        Me.pnlWizardEvent = New System.Windows.Forms.Panel()
        Me.btnEventGuideNext = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblEventGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlEventArrow = New System.Windows.Forms.Panel()
        Me.grpGroupEventPost = New System.Windows.Forms.GroupBox()
        Me.txtTrackerGroup = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.btnLoadEventDescriptionTemplate = New System.Windows.Forms.Button()
        Me.btnSaveEventDescriptionTemplate = New System.Windows.Forms.Button()
        Me.txtClubFullName = New System.Windows.Forms.TextBox()
        Me.chkEventTeaser = New System.Windows.Forms.CheckBox()
        Me.grpEventTeaser = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtEventTeaserMessage = New System.Windows.Forms.TextBox()
        Me.btnSelectEventTeaserAreaMap = New System.Windows.Forms.Button()
        Me.btnClearEventTeaserAreaMap = New System.Windows.Forms.Button()
        Me.txtEventTeaserAreaMapImage = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
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
        Me.chkUseStart = New System.Windows.Forms.CheckBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.chkUseLaunch = New System.Windows.Forms.CheckBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.chkUseSyncFly = New System.Windows.Forms.CheckBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.chkDateTimeUTC = New System.Windows.Forms.CheckBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.pnlEventDateTimeControls = New System.Windows.Forms.Panel()
        Me.dtEventMeetDate = New System.Windows.Forms.DateTimePicker()
        Me.dtEventMeetTime = New System.Windows.Forms.DateTimePicker()
        Me.dtEventSyncFlyDate = New System.Windows.Forms.DateTimePicker()
        Me.dtEventSyncFlyTime = New System.Windows.Forms.DateTimePicker()
        Me.dtEventLaunchDate = New System.Windows.Forms.DateTimePicker()
        Me.dtEventLaunchTime = New System.Windows.Forms.DateTimePicker()
        Me.dtEventStartTaskDate = New System.Windows.Forms.DateTimePicker()
        Me.dtEventStartTaskTime = New System.Windows.Forms.DateTimePicker()
        Me.lblMeetTimeResult = New System.Windows.Forms.Label()
        Me.TimeStampContextualMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GetFullWithDayOfWeek = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetLongDateTime = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetTimeStampTimeOnlyWithoutSeconds = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetCountdown = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetTimeStampOnly = New System.Windows.Forms.ToolStripMenuItem()
        Me.lblSyncTimeResult = New System.Windows.Forms.Label()
        Me.lblLaunchTimeResult = New System.Windows.Forms.Label()
        Me.lblStartTimeResult = New System.Windows.Forms.Label()
        Me.tabDiscord = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.txtLastUpdateDescription = New System.Windows.Forms.TextBox()
        Me.btnDeleteEventNews = New System.Windows.Forms.Button()
        Me.btnPublishEventNews = New System.Windows.Forms.Button()
        Me.btnDeleteFromTaskBrowser = New System.Windows.Forms.Button()
        Me.btnUpdateInTaskBrowser = New System.Windows.Forms.Button()
        Me.btnCreateInTaskBrowser = New System.Windows.Forms.Button()
        Me.pnlWizardDiscord = New System.Windows.Forms.Panel()
        Me.btnDiscordGuideNext = New System.Windows.Forms.Button()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.lblDiscordGuideInstructions = New System.Windows.Forms.Label()
        Me.pnlDiscordArrow = New System.Windows.Forms.Panel()
        Me.pnlFullWorkflowTaskGroupFlight = New System.Windows.Forms.GroupBox()
        Me.btnStartFullPostingWorkflow = New System.Windows.Forms.Button()
        Me.grpDiscordEvent = New System.Windows.Forms.GroupBox()
        Me.Label19 = New System.Windows.Forms.Label()
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
        Me.grpDiscordOthers = New System.Windows.Forms.GroupBox()
        Me.btnTaskAndGroupEventLinks = New System.Windows.Forms.Button()
        Me.btnTaskFeaturedOnGroupFlight = New System.Windows.Forms.Button()
        Me.btnEventDPHXAndLinkOnly = New System.Windows.Forms.Button()
        Me.txtAddOnsDetails = New System.Windows.Forms.TextBox()
        Me.txtWaypointsDetails = New System.Windows.Forms.TextBox()
        Me.txtGroupFlightEventPost = New System.Windows.Forms.TextBox()
        Me.grpDiscordTask = New System.Windows.Forms.GroupBox()
        Me.grpRepost = New System.Windows.Forms.GroupBox()
        Me.btnRepostOriginalURLPaste = New System.Windows.Forms.Button()
        Me.txtRepostOriginalURL = New System.Windows.Forms.TextBox()
        Me.dtRepostOriginalDate = New System.Windows.Forms.DateTimePicker()
        Me.chkRepost = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnDPORecallSettings = New System.Windows.Forms.Button()
        Me.btnDPORememberSettings = New System.Windows.Forms.Button()
        Me.flpDiscordPostOptions = New System.Windows.Forms.FlowLayoutPanel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOMainPost = New System.Windows.Forms.CheckBox()
        Me.lblNbrCarsMainFP = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOThreadCreation = New System.Windows.Forms.CheckBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel4 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOIncludeCoverImage = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel3 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOFullDescription = New System.Windows.Forms.CheckBox()
        Me.lblNbrCarsFullDescResults = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel5 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOFilesWithDescription = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel6 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOFilesAlone = New System.Windows.Forms.CheckBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel7 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOAltRestrictions = New System.Windows.Forms.CheckBox()
        Me.lblNbrCarsRestrictions = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel8 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOWeatherInfo = New System.Windows.Forms.CheckBox()
        Me.lblNbrCarsWeather = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel9 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOWeatherChart = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel10 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOWaypoints = New System.Windows.Forms.CheckBox()
        Me.lblNbrCarsWaypoints = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel11 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOAddOns = New System.Windows.Forms.CheckBox()
        Me.lblNbrCarsAddOns = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel12 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOResultsInvitation = New System.Windows.Forms.CheckBox()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel13 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDPOFeaturedOnGroupFlight = New System.Windows.Forms.CheckBox()
        Me.btnDPOResetToDefault = New System.Windows.Forms.Button()
        Me.btnStartTaskPost = New System.Windows.Forms.Button()
        Me.lblTaskLibraryIDNotAcquired = New System.Windows.Forms.Label()
        Me.lblTaskLibraryIDAcquired = New System.Windows.Forms.Label()
        Me.lblThread1stMsgIDNotAcquired = New System.Windows.Forms.Label()
        Me.lblThread1stMsgIDAcquired = New System.Windows.Forms.Label()
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
        Me.btnDGPORecallSettings = New System.Windows.Forms.Button()
        Me.btnDGPORememberSettings = New System.Windows.Forms.Button()
        Me.btnStartGroupEventPost = New System.Windows.Forms.Button()
        Me.btnDGPOResetToDefault = New System.Windows.Forms.Button()
        Me.flpDiscordGroupPostOptions = New System.Windows.Forms.FlowLayoutPanel()
        Me.FlowLayoutPanel16 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOCoverImage = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel17 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOMainGroupPost = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel18 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOThreadCreation = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel19 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOTeaser = New System.Windows.Forms.CheckBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel23 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOEventLogistics = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel22 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPORelevantTaskDetails = New System.Windows.Forms.CheckBox()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel24 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOMainPost = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel25 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOFullDescription = New System.Windows.Forms.CheckBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel20 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOFilesWithFullLegend = New System.Windows.Forms.CheckBox()
        Me.chkDGPODPHXOnly = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel21 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOFilesWithoutLegend = New System.Windows.Forms.CheckBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel26 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOAltRestrictions = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel27 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOWeatherInfo = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel28 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOWeatherChart = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel29 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOWaypoints = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel30 = New System.Windows.Forms.FlowLayoutPanel()
        Me.chkDGPOAddOns = New System.Windows.Forms.CheckBox()
        Me.btnDiscordGroupEventURL = New System.Windows.Forms.Button()
        Me.txtGroupEventPostURL = New System.Windows.Forms.TextBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.chkDPOExpertMode = New System.Windows.Forms.CheckBox()
        Me.numWaitSecondsForFiles = New System.Windows.Forms.NumericUpDown()
        Me.lblTaskBrowserIDAndDate = New System.Windows.Forms.Label()
        Me.tabBriefing = New System.Windows.Forms.TabPage()
        Me.pnlBriefing = New System.Windows.Forms.Panel()
        Me.pnlWizardBriefing = New System.Windows.Forms.Panel()
        Me.lblBriefingGuideInstructions = New System.Windows.Forms.Label()
        Me.btnBriefingGuideNext = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.OneMinuteTimer = New System.Windows.Forms.Timer(Me.components)
        Me.lblGroupEmoji = New System.Windows.Forms.Label()
        Me.ToolStrip1 = New SIGLR.SoaringTools.CommonLibrary.ToolStripExtensions.ToolStripExtended()
        Me.toolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.toolStripSave = New System.Windows.Forms.ToolStripButton()
        Me.toolStripResetAll = New System.Windows.Forms.ToolStripButton()
        Me.toolStripReload = New System.Windows.Forms.ToolStripButton()
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
        Me.GetNowFullWithDayOfWeek = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowLongDateTime = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowTimeOnlyWithoutSeconds = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowCountdown = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetNowTimeStampOnly = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileDropZone1 = New SIGLR.SoaringTools.CommonLibrary.FileDropZone()
        Me.BriefingControl1 = New SIGLR.SoaringTools.CommonLibrary.BriefingControl()
        Me.pnlScrollableSurface.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.tabFlightPlan.SuspendLayout()
        Me.pnlGuide.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.pnlFlightPlanRightSide.SuspendLayout()
        Me.grbTaskPart2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.grbTaskDiscord.SuspendLayout()
        Me.pnlFlightPlanLeftSide.SuspendLayout()
        Me.grbTaskInfo.SuspendLayout()
        Me.tabEvent.SuspendLayout()
        Me.pnlWizardEvent.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.grpGroupEventPost.SuspendLayout()
        Me.grpEventTeaser.SuspendLayout()
        Me.pnlEventDateTimeControls.SuspendLayout()
        Me.TimeStampContextualMenu.SuspendLayout()
        Me.tabDiscord.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.pnlWizardDiscord.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.pnlFullWorkflowTaskGroupFlight.SuspendLayout()
        Me.grpDiscordEvent.SuspendLayout()
        Me.grpDiscordOthers.SuspendLayout()
        Me.grpDiscordTask.SuspendLayout()
        Me.grpRepost.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.flpDiscordPostOptions.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.FlowLayoutPanel4.SuspendLayout()
        Me.FlowLayoutPanel3.SuspendLayout()
        Me.FlowLayoutPanel5.SuspendLayout()
        Me.FlowLayoutPanel6.SuspendLayout()
        Me.FlowLayoutPanel7.SuspendLayout()
        Me.FlowLayoutPanel8.SuspendLayout()
        Me.FlowLayoutPanel9.SuspendLayout()
        Me.FlowLayoutPanel10.SuspendLayout()
        Me.FlowLayoutPanel11.SuspendLayout()
        Me.FlowLayoutPanel12.SuspendLayout()
        Me.FlowLayoutPanel13.SuspendLayout()
        Me.grpDiscordGroupFlight.SuspendLayout()
        Me.grpGroupFlightEvent.SuspendLayout()
        Me.flpDiscordGroupPostOptions.SuspendLayout()
        Me.FlowLayoutPanel16.SuspendLayout()
        Me.FlowLayoutPanel17.SuspendLayout()
        Me.FlowLayoutPanel18.SuspendLayout()
        Me.FlowLayoutPanel19.SuspendLayout()
        Me.FlowLayoutPanel23.SuspendLayout()
        Me.FlowLayoutPanel22.SuspendLayout()
        Me.FlowLayoutPanel24.SuspendLayout()
        Me.FlowLayoutPanel25.SuspendLayout()
        Me.FlowLayoutPanel20.SuspendLayout()
        Me.FlowLayoutPanel21.SuspendLayout()
        Me.FlowLayoutPanel26.SuspendLayout()
        Me.FlowLayoutPanel27.SuspendLayout()
        Me.FlowLayoutPanel28.SuspendLayout()
        Me.FlowLayoutPanel29.SuspendLayout()
        Me.FlowLayoutPanel30.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        CType(Me.numWaitSecondsForFiles, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabBriefing.SuspendLayout()
        Me.pnlBriefing.SuspendLayout()
        Me.pnlWizardBriefing.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlScrollableSurface
        '
        Me.pnlScrollableSurface.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlScrollableSurface.AutoScroll = True
        Me.pnlScrollableSurface.AutoScrollMargin = New System.Drawing.Size(0, 15)
        Me.pnlScrollableSurface.AutoScrollMinSize = New System.Drawing.Size(1490, 878)
        Me.pnlScrollableSurface.Controls.Add(Me.TabControl1)
        Me.pnlScrollableSurface.Location = New System.Drawing.Point(0, 28)
        Me.pnlScrollableSurface.MinimumSize = New System.Drawing.Size(1490, 650)
        Me.pnlScrollableSurface.Name = "pnlScrollableSurface"
        Me.pnlScrollableSurface.Size = New System.Drawing.Size(1490, 893)
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
        Me.TabControl1.ItemSize = New System.Drawing.Size(155, 25)
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.MinimumSize = New System.Drawing.Size(1475, 892)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1475, 893)
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControl1.TabIndex = 0
        '
        'tabFlightPlan
        '
        Me.tabFlightPlan.AutoScroll = True
        Me.tabFlightPlan.BackColor = System.Drawing.Color.Transparent
        Me.tabFlightPlan.Controls.Add(Me.pnlGuide)
        Me.tabFlightPlan.Controls.Add(Me.pnlFlightPlanRightSide)
        Me.tabFlightPlan.Controls.Add(Me.FlightPlanTabSplitter)
        Me.tabFlightPlan.Controls.Add(Me.pnlFlightPlanLeftSide)
        Me.tabFlightPlan.Location = New System.Drawing.Point(4, 29)
        Me.tabFlightPlan.Name = "tabFlightPlan"
        Me.tabFlightPlan.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFlightPlan.Size = New System.Drawing.Size(1467, 860)
        Me.tabFlightPlan.TabIndex = 0
        Me.tabFlightPlan.Text = "Flight Plan"
        '
        'pnlGuide
        '
        Me.pnlGuide.BackColor = System.Drawing.Color.Gray
        Me.pnlGuide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlGuide.Controls.Add(Me.btnGuideNext)
        Me.pnlGuide.Controls.Add(Me.Panel3)
        Me.pnlGuide.Controls.Add(Me.pnlArrow)
        Me.pnlGuide.Location = New System.Drawing.Point(19, 760)
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
        'pnlFlightPlanRightSide
        '
        Me.pnlFlightPlanRightSide.Controls.Add(Me.grbTaskPart2)
        Me.pnlFlightPlanRightSide.Controls.Add(Me.FileDropZone1)
        Me.pnlFlightPlanRightSide.Controls.Add(Me.grbTaskDiscord)
        Me.pnlFlightPlanRightSide.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlFlightPlanRightSide.Location = New System.Drawing.Point(753, 3)
        Me.pnlFlightPlanRightSide.MinimumSize = New System.Drawing.Size(715, 854)
        Me.pnlFlightPlanRightSide.Name = "pnlFlightPlanRightSide"
        Me.pnlFlightPlanRightSide.Size = New System.Drawing.Size(715, 854)
        Me.pnlFlightPlanRightSide.TabIndex = 16
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
        Me.grbTaskPart2.Controls.Add(Me.txtWeatherSummary)
        Me.grbTaskPart2.Controls.Add(Me.lblWeatherSummary)
        Me.grbTaskPart2.Enabled = False
        Me.grbTaskPart2.Location = New System.Drawing.Point(4, -5)
        Me.grbTaskPart2.Name = "grbTaskPart2"
        Me.grbTaskPart2.Size = New System.Drawing.Size(711, 613)
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
        Me.txtBaroPressureExtraInfo.Size = New System.Drawing.Size(340, 32)
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
        Me.GroupBox3.Size = New System.Drawing.Size(699, 229)
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
        Me.cboCoverImage.Size = New System.Drawing.Size(510, 28)
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
        Me.cboBriefingMap.Size = New System.Drawing.Size(510, 28)
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
        Me.lstAllFiles.Size = New System.Drawing.Size(510, 124)
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
        Me.GroupBox2.Size = New System.Drawing.Size(699, 160)
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
        Me.lstAllRecommendedAddOns.Size = New System.Drawing.Size(510, 124)
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
        Me.lstAllCountries.Location = New System.Drawing.Point(360, 62)
        Me.lstAllCountries.Name = "lstAllCountries"
        Me.lstAllCountries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstAllCountries.Size = New System.Drawing.Size(339, 64)
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
        Me.cboCountryFlag.Size = New System.Drawing.Size(510, 32)
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
        'txtWeatherSummary
        '
        Me.txtWeatherSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWeatherSummary.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherSummary.Location = New System.Drawing.Point(189, 134)
        Me.txtWeatherSummary.Name = "txtWeatherSummary"
        Me.txtWeatherSummary.Size = New System.Drawing.Size(510, 32)
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
        Me.grbTaskDiscord.Location = New System.Drawing.Point(4, 614)
        Me.grbTaskDiscord.Name = "grbTaskDiscord"
        Me.grbTaskDiscord.Size = New System.Drawing.Size(711, 72)
        Me.grbTaskDiscord.TabIndex = 4
        Me.grbTaskDiscord.TabStop = False
        Me.grbTaskDiscord.Text = "Discord / Task ID"
        '
        'btnDeleteDiscordID
        '
        Me.btnDeleteDiscordID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDeleteDiscordID.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDeleteDiscordID.Location = New System.Drawing.Point(629, 27)
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
        Me.btnDiscordTaskThreadURLPaste.Location = New System.Drawing.Point(544, 27)
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
        Me.txtDiscordTaskID.Size = New System.Drawing.Size(349, 32)
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
        'FlightPlanTabSplitter
        '
        Me.FlightPlanTabSplitter.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.FlightPlanTabSplitter.Location = New System.Drawing.Point(743, 3)
        Me.FlightPlanTabSplitter.MinExtra = 715
        Me.FlightPlanTabSplitter.MinSize = 740
        Me.FlightPlanTabSplitter.Name = "FlightPlanTabSplitter"
        Me.FlightPlanTabSplitter.Size = New System.Drawing.Size(10, 854)
        Me.FlightPlanTabSplitter.TabIndex = 84
        Me.FlightPlanTabSplitter.TabStop = False
        '
        'pnlFlightPlanLeftSide
        '
        Me.pnlFlightPlanLeftSide.Controls.Add(Me.lblElevationUpdateWarning)
        Me.pnlFlightPlanLeftSide.Controls.Add(Me.grbTaskInfo)
        Me.pnlFlightPlanLeftSide.Controls.Add(Me.btnSelectFlightPlan)
        Me.pnlFlightPlanLeftSide.Controls.Add(Me.txtFlightPlanFile)
        Me.pnlFlightPlanLeftSide.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlFlightPlanLeftSide.Location = New System.Drawing.Point(3, 3)
        Me.pnlFlightPlanLeftSide.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlFlightPlanLeftSide.MinimumSize = New System.Drawing.Size(740, 854)
        Me.pnlFlightPlanLeftSide.Name = "pnlFlightPlanLeftSide"
        Me.pnlFlightPlanLeftSide.Size = New System.Drawing.Size(740, 854)
        Me.pnlFlightPlanLeftSide.TabIndex = 0
        '
        'lblElevationUpdateWarning
        '
        Me.lblElevationUpdateWarning.AutoSize = True
        Me.lblElevationUpdateWarning.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblElevationUpdateWarning.ForeColor = System.Drawing.Color.Red
        Me.lblElevationUpdateWarning.Location = New System.Drawing.Point(69, 50)
        Me.lblElevationUpdateWarning.Name = "lblElevationUpdateWarning"
        Me.lblElevationUpdateWarning.Size = New System.Drawing.Size(659, 21)
        Me.lblElevationUpdateWarning.TabIndex = 83
        Me.lblElevationUpdateWarning.Text = "One or more waypoints have their elevation set to 1500' - Possible elevation upda" &
    "te required!"
        Me.ToolTip1.SetToolTip(Me.lblElevationUpdateWarning, "Open the flight plan on the B21 Planner and make sure to update all elevations. O" &
        "therwise, you can dismiss this warning.")
        Me.lblElevationUpdateWarning.Visible = False
        '
        'grbTaskInfo
        '
        Me.grbTaskInfo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbTaskInfo.Controls.Add(Me.txtAATTask)
        Me.grbTaskInfo.Controls.Add(Me.btnRecallTaskDescriptionTemplate)
        Me.grbTaskInfo.Controls.Add(Me.btnSaveDescriptionTemplate)
        Me.grbTaskInfo.Controls.Add(Me.btnWeatherBrowser)
        Me.grbTaskInfo.Controls.Add(Me.btnSyncTitles)
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
        Me.grbTaskInfo.Controls.Add(Me.cboKnownTaskDesigners)
        Me.grbTaskInfo.Enabled = False
        Me.grbTaskInfo.Location = New System.Drawing.Point(6, 56)
        Me.grbTaskInfo.MinimumSize = New System.Drawing.Size(700, 796)
        Me.grbTaskInfo.Name = "grbTaskInfo"
        Me.grbTaskInfo.Size = New System.Drawing.Size(729, 796)
        Me.grbTaskInfo.TabIndex = 2
        Me.grbTaskInfo.TabStop = False
        '
        'txtAATTask
        '
        Me.txtAATTask.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAATTask.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAATTask.Location = New System.Drawing.Point(189, 338)
        Me.txtAATTask.Name = "txtAATTask"
        Me.txtAATTask.ReadOnly = True
        Me.txtAATTask.Size = New System.Drawing.Size(534, 32)
        Me.txtAATTask.TabIndex = 37
        Me.txtAATTask.Tag = "10"
        Me.ToolTip1.SetToolTip(Me.txtAATTask, "AAT Task with minimum duration information")
        '
        'btnRecallTaskDescriptionTemplate
        '
        Me.btnRecallTaskDescriptionTemplate.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRecallTaskDescriptionTemplate.Location = New System.Drawing.Point(6, 644)
        Me.btnRecallTaskDescriptionTemplate.Name = "btnRecallTaskDescriptionTemplate"
        Me.btnRecallTaskDescriptionTemplate.Size = New System.Drawing.Size(177, 37)
        Me.btnRecallTaskDescriptionTemplate.TabIndex = 58
        Me.btnRecallTaskDescriptionTemplate.Tag = "22"
        Me.btnRecallTaskDescriptionTemplate.Text = "Load template"
        Me.ToolTip1.SetToolTip(Me.btnRecallTaskDescriptionTemplate, "Click this button to replace the current description with your saved template.")
        Me.btnRecallTaskDescriptionTemplate.UseVisualStyleBackColor = True
        '
        'btnSaveDescriptionTemplate
        '
        Me.btnSaveDescriptionTemplate.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveDescriptionTemplate.Location = New System.Drawing.Point(6, 601)
        Me.btnSaveDescriptionTemplate.Name = "btnSaveDescriptionTemplate"
        Me.btnSaveDescriptionTemplate.Size = New System.Drawing.Size(177, 37)
        Me.btnSaveDescriptionTemplate.TabIndex = 57
        Me.btnSaveDescriptionTemplate.Tag = "22"
        Me.btnSaveDescriptionTemplate.Text = "Save template"
        Me.ToolTip1.SetToolTip(Me.btnSaveDescriptionTemplate, "Click this button to save the current description to be used by default as templa" &
        "te.")
        Me.btnSaveDescriptionTemplate.UseVisualStyleBackColor = True
        '
        'btnWeatherBrowser
        '
        Me.btnWeatherBrowser.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeatherBrowser.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWeatherBrowser.Location = New System.Drawing.Point(635, 24)
        Me.btnWeatherBrowser.Name = "btnWeatherBrowser"
        Me.btnWeatherBrowser.Size = New System.Drawing.Size(88, 32)
        Me.btnWeatherBrowser.TabIndex = 2
        Me.btnWeatherBrowser.Tag = "22"
        Me.btnWeatherBrowser.Text = "Browser"
        Me.ToolTip1.SetToolTip(Me.btnWeatherBrowser, "Click this button to open the weather profile browser / manager.")
        Me.btnWeatherBrowser.UseVisualStyleBackColor = True
        Me.btnWeatherBrowser.Visible = False
        '
        'btnSyncTitles
        '
        Me.btnSyncTitles.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSyncTitles.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSyncTitles.Location = New System.Drawing.Point(656, 66)
        Me.btnSyncTitles.Name = "btnSyncTitles"
        Me.btnSyncTitles.Size = New System.Drawing.Size(66, 32)
        Me.btnSyncTitles.TabIndex = 6
        Me.btnSyncTitles.Tag = "22"
        Me.btnSyncTitles.Text = "Sync"
        Me.ToolTip1.SetToolTip(Me.btnSyncTitles, "Click this button to harmonize all titles and filenames for flight plan and weath" &
        "er file.")
        Me.btnSyncTitles.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeDynamic
        '
        Me.chkSoaringTypeDynamic.AutoSize = True
        Me.chkSoaringTypeDynamic.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeDynamic.Location = New System.Drawing.Point(338, 271)
        Me.chkSoaringTypeDynamic.Name = "chkSoaringTypeDynamic"
        Me.chkSoaringTypeDynamic.Size = New System.Drawing.Size(44, 30)
        Me.chkSoaringTypeDynamic.TabIndex = 29
        Me.chkSoaringTypeDynamic.Tag = "8"
        Me.chkSoaringTypeDynamic.Text = "D"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeDynamic, "Check if task involves dynamic soaring.")
        Me.chkSoaringTypeDynamic.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeWave
        '
        Me.chkSoaringTypeWave.AutoSize = True
        Me.chkSoaringTypeWave.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeWave.Location = New System.Drawing.Point(284, 271)
        Me.chkSoaringTypeWave.Name = "chkSoaringTypeWave"
        Me.chkSoaringTypeWave.Size = New System.Drawing.Size(48, 30)
        Me.chkSoaringTypeWave.TabIndex = 28
        Me.chkSoaringTypeWave.Tag = "8"
        Me.chkSoaringTypeWave.Text = "W"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeWave, "Check if task involves wave soaring.")
        Me.chkSoaringTypeWave.UseVisualStyleBackColor = True
        '
        'btnPasteUsernameCredits
        '
        Me.btnPasteUsernameCredits.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPasteUsernameCredits.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPasteUsernameCredits.Location = New System.Drawing.Point(644, 538)
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
        Me.Label9.TabIndex = 31
        Me.Label9.Text = "Distance"
        '
        'chkTitleLock
        '
        Me.chkTitleLock.AutoSize = True
        Me.chkTitleLock.Location = New System.Drawing.Point(168, 73)
        Me.chkTitleLock.Name = "chkTitleLock"
        Me.chkTitleLock.Size = New System.Drawing.Size(15, 14)
        Me.chkTitleLock.TabIndex = 4
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
        Me.chkArrivalLock.TabIndex = 23
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
        Me.chkDepartureLock.TabIndex = 18
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
        Me.chkSoaringTypeThermal.TabIndex = 27
        Me.chkSoaringTypeThermal.Tag = "8"
        Me.chkSoaringTypeThermal.Text = "T"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeThermal, "Check if task involves thermal soaring.")
        Me.chkSoaringTypeThermal.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeRidge
        '
        Me.chkSoaringTypeRidge.AutoSize = True
        Me.chkSoaringTypeRidge.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSoaringTypeRidge.Location = New System.Drawing.Point(189, 271)
        Me.chkSoaringTypeRidge.Name = "chkSoaringTypeRidge"
        Me.chkSoaringTypeRidge.Size = New System.Drawing.Size(42, 30)
        Me.chkSoaringTypeRidge.TabIndex = 26
        Me.chkSoaringTypeRidge.Tag = "8"
        Me.chkSoaringTypeRidge.Text = "R"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeRidge, "Check if task involves ridge soaring.")
        Me.chkSoaringTypeRidge.UseVisualStyleBackColor = True
        '
        'txtSoaringTypeExtraInfo
        '
        Me.txtSoaringTypeExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSoaringTypeExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSoaringTypeExtraInfo.Location = New System.Drawing.Point(388, 270)
        Me.txtSoaringTypeExtraInfo.Name = "txtSoaringTypeExtraInfo"
        Me.txtSoaringTypeExtraInfo.Size = New System.Drawing.Size(335, 32)
        Me.txtSoaringTypeExtraInfo.TabIndex = 30
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
        Me.lblSoaringType.TabIndex = 25
        Me.lblSoaringType.Text = "Soaring Type"
        '
        'txtArrivalExtraInfo
        '
        Me.txtArrivalExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtArrivalExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtArrivalExtraInfo.Location = New System.Drawing.Point(467, 236)
        Me.txtArrivalExtraInfo.Name = "txtArrivalExtraInfo"
        Me.txtArrivalExtraInfo.Size = New System.Drawing.Size(256, 32)
        Me.txtArrivalExtraInfo.TabIndex = 24
        Me.txtArrivalExtraInfo.Tag = "7"
        Me.ToolTip1.SetToolTip(Me.txtArrivalExtraInfo, "Any extra information to add to the arrival line.")
        '
        'txtArrivalName
        '
        Me.txtArrivalName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtArrivalName.Location = New System.Drawing.Point(276, 236)
        Me.txtArrivalName.Name = "txtArrivalName"
        Me.txtArrivalName.Size = New System.Drawing.Size(164, 32)
        Me.txtArrivalName.TabIndex = 22
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
        Me.txtArrivalICAO.TabIndex = 21
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
        Me.Label7.TabIndex = 20
        Me.Label7.Text = "Arrival"
        '
        'txtDepExtraInfo
        '
        Me.txtDepExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDepExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDepExtraInfo.Location = New System.Drawing.Point(467, 202)
        Me.txtDepExtraInfo.Name = "txtDepExtraInfo"
        Me.txtDepExtraInfo.Size = New System.Drawing.Size(256, 32)
        Me.txtDepExtraInfo.TabIndex = 19
        Me.txtDepExtraInfo.Tag = "6"
        Me.ToolTip1.SetToolTip(Me.txtDepExtraInfo, "Any extra information to add to the departure line.")
        '
        'txtDepName
        '
        Me.txtDepName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDepName.Location = New System.Drawing.Point(276, 202)
        Me.txtDepName.Name = "txtDepName"
        Me.txtDepName.Size = New System.Drawing.Size(164, 32)
        Me.txtDepName.TabIndex = 17
        Me.txtDepName.Tag = "6"
        Me.ToolTip1.SetToolTip(Me.txtDepName, "Departure airport name, can be automatic based on ICAO.")
        '
        'txtSimDateTimeExtraInfo
        '
        Me.txtSimDateTimeExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSimDateTimeExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSimDateTimeExtraInfo.Location = New System.Drawing.Point(299, 134)
        Me.txtSimDateTimeExtraInfo.Name = "txtSimDateTimeExtraInfo"
        Me.txtSimDateTimeExtraInfo.Size = New System.Drawing.Size(424, 32)
        Me.txtSimDateTimeExtraInfo.TabIndex = 12
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
        Me.Label5.TabIndex = 10
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
        Me.dtSimLocalTime.TabIndex = 11
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
        Me.chkIncludeYear.TabIndex = 9
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
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Sim Date"
        '
        'dtSimDate
        '
        Me.dtSimDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtSimDate.Location = New System.Drawing.Point(189, 100)
        Me.dtSimDate.Name = "dtSimDate"
        Me.dtSimDate.Size = New System.Drawing.Size(200, 31)
        Me.dtSimDate.TabIndex = 8
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
        Me.txtDepartureICAO.TabIndex = 16
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
        Me.lblDeparture.TabIndex = 15
        Me.lblDeparture.Text = "Departure"
        '
        'txtMainArea
        '
        Me.txtMainArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtMainArea.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMainArea.Location = New System.Drawing.Point(189, 168)
        Me.txtMainArea.Name = "txtMainArea"
        Me.txtMainArea.Size = New System.Drawing.Size(534, 32)
        Me.txtMainArea.TabIndex = 14
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
        Me.lblMainAreaPOI.TabIndex = 13
        Me.lblMainAreaPOI.Text = "Main area / POI"
        '
        'txtTitle
        '
        Me.txtTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTitle.Location = New System.Drawing.Point(189, 66)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(461, 32)
        Me.txtTitle.TabIndex = 5
        Me.txtTitle.Tag = "3"
        Me.ToolTip1.SetToolTip(Me.txtTitle, "Task title - can come from the flight plan's title.")
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(4, 70)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(47, 26)
        Me.lblTitle.TabIndex = 3
        Me.lblTitle.Text = "Title"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(4, 341)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(83, 26)
        Me.Label22.TabIndex = 36
        Me.Label22.Text = "AAT Task"
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
        Me.txtWeatherFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWeatherFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtWeatherFile.Location = New System.Drawing.Point(189, 24)
        Me.txtWeatherFile.Name = "txtWeatherFile"
        Me.txtWeatherFile.ReadOnly = True
        Me.txtWeatherFile.Size = New System.Drawing.Size(534, 32)
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
        Me.txtDurationMin.TabIndex = 40
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
        Me.txtDurationExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDurationExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDurationExtraInfo.Location = New System.Drawing.Point(337, 372)
        Me.txtDurationExtraInfo.Name = "txtDurationExtraInfo"
        Me.txtDurationExtraInfo.Size = New System.Drawing.Size(386, 32)
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
        Me.txtCredits.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCredits.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCredits.Location = New System.Drawing.Point(189, 535)
        Me.txtCredits.Name = "txtCredits"
        Me.txtCredits.Size = New System.Drawing.Size(421, 32)
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
        Me.lblTotalDistanceAndMiles.TabIndex = 33
        Me.lblTotalDistanceAndMiles.Text = "km / 9999 mi Total"
        '
        'lblTrackDistanceAndMiles
        '
        Me.lblTrackDistanceAndMiles.AutoSize = True
        Me.lblTrackDistanceAndMiles.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTrackDistanceAndMiles.Location = New System.Drawing.Point(468, 306)
        Me.lblTrackDistanceAndMiles.Name = "lblTrackDistanceAndMiles"
        Me.lblTrackDistanceAndMiles.Size = New System.Drawing.Size(156, 26)
        Me.lblTrackDistanceAndMiles.TabIndex = 35
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
        Me.txtDistanceTotal.TabIndex = 32
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
        Me.txtDistanceTrack.TabIndex = 34
        Me.txtDistanceTrack.Tag = "9"
        Me.txtDistanceTrack.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ToolTip1.SetToolTip(Me.txtDistanceTrack, "Task distance (this is the distance shown by B21 Planner) read from flight plan f" &
        "ile.")
        '
        'cboRecommendedGliders
        '
        Me.cboRecommendedGliders.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboRecommendedGliders.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboRecommendedGliders.FormattingEnabled = True
        Me.cboRecommendedGliders.Items.AddRange(New Object() {"Any", "Flapped", "AS 33 / JS3 18m Class", "Discus 2c / JS3 15m Class", "AS 33 Me (Schleicher)", "DG-1001e Neo (DG-Flugzeugbau)", "DG808s (DG-Flugzeugbau)", "Discus 2c (Schempp-Hirth)", "JS3 Rapture 15m (Jonker)", "JS3 Rapture 18m (Jonker)", "ASK 21 (Schleicher)", "K7 (Schleicher)", "LS4 (Rolladen-Schneider)", "LS8-18 (Rolladen-Schneider)", "SG 38 (DFS Schulgleiter)", "Stemme S12-G", "Pipistrel Taurus-M", "SZD-30 Pirat (PZL)"})
        Me.cboRecommendedGliders.Location = New System.Drawing.Point(189, 406)
        Me.cboRecommendedGliders.Name = "cboRecommendedGliders"
        Me.cboRecommendedGliders.Size = New System.Drawing.Size(534, 32)
        Me.cboRecommendedGliders.TabIndex = 44
        Me.cboRecommendedGliders.Tag = "12"
        Me.ToolTip1.SetToolTip(Me.cboRecommendedGliders, "Recommended gliders (suggestions in the list or enter your own)")
        '
        'txtDifficultyExtraInfo
        '
        Me.txtDifficultyExtraInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDifficultyExtraInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDifficultyExtraInfo.Location = New System.Drawing.Point(446, 440)
        Me.txtDifficultyExtraInfo.Name = "txtDifficultyExtraInfo"
        Me.txtDifficultyExtraInfo.Size = New System.Drawing.Size(277, 32)
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
        Me.txtShortDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtShortDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtShortDescription.Location = New System.Drawing.Point(189, 476)
        Me.txtShortDescription.Multiline = True
        Me.txtShortDescription.Name = "txtShortDescription"
        Me.txtShortDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtShortDescription.Size = New System.Drawing.Size(534, 53)
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
        Me.txtLongDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLongDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLongDescription.Location = New System.Drawing.Point(189, 569)
        Me.txtLongDescription.Multiline = True
        Me.txtLongDescription.Name = "txtLongDescription"
        Me.txtLongDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLongDescription.Size = New System.Drawing.Size(534, 217)
        Me.txtLongDescription.TabIndex = 55
        Me.txtLongDescription.Tag = "16"
        Me.ToolTip1.SetToolTip(Me.txtLongDescription, "Full (long) description of the flight.")
        '
        'cboKnownTaskDesigners
        '
        Me.cboKnownTaskDesigners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboKnownTaskDesigners.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboKnownTaskDesigners.FormattingEnabled = True
        Me.cboKnownTaskDesigners.Location = New System.Drawing.Point(189, 535)
        Me.cboKnownTaskDesigners.Name = "cboKnownTaskDesigners"
        Me.cboKnownTaskDesigners.Size = New System.Drawing.Size(449, 32)
        Me.cboKnownTaskDesigners.Sorted = True
        Me.cboKnownTaskDesigners.TabIndex = 53
        Me.cboKnownTaskDesigners.TabStop = False
        Me.cboKnownTaskDesigners.Tag = "15"
        Me.ToolTip1.SetToolTip(Me.cboKnownTaskDesigners, "Click to help selecting a designer for credits.")
        '
        'btnSelectFlightPlan
        '
        Me.btnSelectFlightPlan.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectFlightPlan.Location = New System.Drawing.Point(14, 15)
        Me.btnSelectFlightPlan.Name = "btnSelectFlightPlan"
        Me.btnSelectFlightPlan.Size = New System.Drawing.Size(175, 35)
        Me.btnSelectFlightPlan.TabIndex = 0
        Me.btnSelectFlightPlan.Tag = "1"
        Me.btnSelectFlightPlan.Text = "Flight Plan"
        Me.ToolTip1.SetToolTip(Me.btnSelectFlightPlan, "Click to select the flight plan file to use and extract information from.")
        Me.btnSelectFlightPlan.UseVisualStyleBackColor = True
        '
        'txtFlightPlanFile
        '
        Me.txtFlightPlanFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFlightPlanFile.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFlightPlanFile.Location = New System.Drawing.Point(195, 18)
        Me.txtFlightPlanFile.Name = "txtFlightPlanFile"
        Me.txtFlightPlanFile.ReadOnly = True
        Me.txtFlightPlanFile.Size = New System.Drawing.Size(534, 32)
        Me.txtFlightPlanFile.TabIndex = 1
        Me.txtFlightPlanFile.TabStop = False
        Me.txtFlightPlanFile.Tag = "1"
        Me.ToolTip1.SetToolTip(Me.txtFlightPlanFile, "Current flight plan file selected.")
        '
        'tabEvent
        '
        Me.tabEvent.Controls.Add(Me.chkActivateEvent)
        Me.tabEvent.Controls.Add(Me.pnlWizardEvent)
        Me.tabEvent.Controls.Add(Me.grpGroupEventPost)
        Me.tabEvent.Location = New System.Drawing.Point(4, 29)
        Me.tabEvent.Name = "tabEvent"
        Me.tabEvent.Padding = New System.Windows.Forms.Padding(3)
        Me.tabEvent.Size = New System.Drawing.Size(1467, 860)
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
        Me.chkActivateEvent.TabIndex = 0
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
        Me.pnlWizardEvent.Location = New System.Drawing.Point(849, 546)
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
        Me.grpGroupEventPost.Controls.Add(Me.lblGroupEmoji)
        Me.grpGroupEventPost.Controls.Add(Me.txtTrackerGroup)
        Me.grpGroupEventPost.Controls.Add(Me.Label14)
        Me.grpGroupEventPost.Controls.Add(Me.btnLoadEventDescriptionTemplate)
        Me.grpGroupEventPost.Controls.Add(Me.btnSaveEventDescriptionTemplate)
        Me.grpGroupEventPost.Controls.Add(Me.txtClubFullName)
        Me.grpGroupEventPost.Controls.Add(Me.chkEventTeaser)
        Me.grpGroupEventPost.Controls.Add(Me.grpEventTeaser)
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
        Me.grpGroupEventPost.Size = New System.Drawing.Size(1455, 850)
        Me.grpGroupEventPost.TabIndex = 0
        Me.grpGroupEventPost.TabStop = False
        '
        'txtTrackerGroup
        '
        Me.txtTrackerGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTrackerGroup.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtTrackerGroup.Location = New System.Drawing.Point(655, 89)
        Me.txtTrackerGroup.Name = "txtTrackerGroup"
        Me.txtTrackerGroup.Size = New System.Drawing.Size(210, 32)
        Me.txtTrackerGroup.TabIndex = 87
        Me.txtTrackerGroup.Tag = "61"
        Me.ToolTip1.SetToolTip(Me.txtTrackerGroup, "You can specify the club's group name used in the tracker utility.")
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(520, 92)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(129, 26)
        Me.Label14.TabIndex = 86
        Me.Label14.Text = "Tracker Group"
        '
        'btnLoadEventDescriptionTemplate
        '
        Me.btnLoadEventDescriptionTemplate.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLoadEventDescriptionTemplate.Location = New System.Drawing.Point(12, 515)
        Me.btnLoadEventDescriptionTemplate.Name = "btnLoadEventDescriptionTemplate"
        Me.btnLoadEventDescriptionTemplate.Size = New System.Drawing.Size(172, 37)
        Me.btnLoadEventDescriptionTemplate.TabIndex = 25
        Me.btnLoadEventDescriptionTemplate.Tag = "22"
        Me.btnLoadEventDescriptionTemplate.Text = "Load template"
        Me.ToolTip1.SetToolTip(Me.btnLoadEventDescriptionTemplate, "Click this button to replace the current description with your saved template.")
        Me.btnLoadEventDescriptionTemplate.UseVisualStyleBackColor = True
        '
        'btnSaveEventDescriptionTemplate
        '
        Me.btnSaveEventDescriptionTemplate.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveEventDescriptionTemplate.Location = New System.Drawing.Point(12, 472)
        Me.btnSaveEventDescriptionTemplate.Name = "btnSaveEventDescriptionTemplate"
        Me.btnSaveEventDescriptionTemplate.Size = New System.Drawing.Size(174, 37)
        Me.btnSaveEventDescriptionTemplate.TabIndex = 24
        Me.btnSaveEventDescriptionTemplate.Tag = "22"
        Me.btnSaveEventDescriptionTemplate.Text = "Save template"
        Me.ToolTip1.SetToolTip(Me.btnSaveEventDescriptionTemplate, "Click this button to save the current description to be used by default as templa" &
        "te.")
        Me.btnSaveEventDescriptionTemplate.UseVisualStyleBackColor = True
        '
        'txtClubFullName
        '
        Me.txtClubFullName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtClubFullName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtClubFullName.Location = New System.Drawing.Point(192, 125)
        Me.txtClubFullName.Name = "txtClubFullName"
        Me.txtClubFullName.ReadOnly = True
        Me.txtClubFullName.Size = New System.Drawing.Size(1257, 32)
        Me.txtClubFullName.TabIndex = 4
        Me.txtClubFullName.Tag = "61"
        Me.ToolTip1.SetToolTip(Me.txtClubFullName, "The soaring club event's full ""official"" name.")
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
        Me.grpEventTeaser.Size = New System.Drawing.Size(1443, 146)
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
        Me.txtEventTeaserMessage.Size = New System.Drawing.Size(1252, 76)
        Me.txtEventTeaserMessage.TabIndex = 5
        Me.txtEventTeaserMessage.Tag = "72"
        Me.ToolTip1.SetToolTip(Me.txtEventTeaserMessage, "Specify any message you want to be posted with the teaser")
        '
        'btnSelectEventTeaserAreaMap
        '
        Me.btnSelectEventTeaserAreaMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectEventTeaserAreaMap.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectEventTeaserAreaMap.Location = New System.Drawing.Point(1273, 27)
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
        Me.btnClearEventTeaserAreaMap.Location = New System.Drawing.Point(1358, 26)
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
        Me.txtEventTeaserAreaMapImage.Size = New System.Drawing.Size(1082, 32)
        Me.txtEventTeaserAreaMapImage.TabIndex = 1
        Me.txtEventTeaserAreaMapImage.Tag = "72"
        Me.ToolTip1.SetToolTip(Me.txtEventTeaserAreaMapImage, "Select an image to use in the Teaser post for the group event.")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(6, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(120, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Teaser Image"
        '
        'btnPasteBeginnerLink
        '
        Me.btnPasteBeginnerLink.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPasteBeginnerLink.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPasteBeginnerLink.Location = New System.Drawing.Point(1370, 663)
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
        Me.txtOtherBeginnerLink.Size = New System.Drawing.Size(1172, 32)
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
        Me.cboBeginnersGuide.Size = New System.Drawing.Size(1257, 32)
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
        Me.lblLocalDSTWarning.TabIndex = 13
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
        Me.cboGroupOrClubName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboGroupOrClubName.FormattingEnabled = True
        Me.cboGroupOrClubName.Items.AddRange(New Object() {"TSC", "FSC", "SSC Saturday", "Aus Tuesdays", "DTS"})
        Me.cboGroupOrClubName.Location = New System.Drawing.Point(192, 89)
        Me.cboGroupOrClubName.Name = "cboGroupOrClubName"
        Me.cboGroupOrClubName.Size = New System.Drawing.Size(322, 32)
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
        Me.txtEventTitle.Size = New System.Drawing.Size(1257, 32)
        Me.txtEventTitle.TabIndex = 6
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
        Me.Label41.TabIndex = 5
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
        Me.Label34.TabIndex = 9
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
        Me.cboVoiceChannel.Size = New System.Drawing.Size(1257, 32)
        Me.cboVoiceChannel.TabIndex = 10
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
        Me.cboMSFSServer.TabIndex = 8
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
        Me.Label33.TabIndex = 7
        Me.Label33.Text = "MSFS Server to use"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label32.Location = New System.Drawing.Point(7, 443)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(159, 26)
        Me.Label32.TabIndex = 22
        Me.Label32.Text = "Event Description"
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
        Me.txtEventDescription.Size = New System.Drawing.Size(1257, 139)
        Me.txtEventDescription.TabIndex = 23
        Me.txtEventDescription.Tag = "69"
        Me.ToolTip1.SetToolTip(Me.txtEventDescription, "Short description of the event")
        '
        'chkUseStart
        '
        Me.chkUseStart.AutoSize = True
        Me.chkUseStart.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseStart.Location = New System.Drawing.Point(125, 408)
        Me.chkUseStart.Name = "chkUseStart"
        Me.chkUseStart.Size = New System.Drawing.Size(59, 30)
        Me.chkUseStart.TabIndex = 21
        Me.chkUseStart.Tag = "68"
        Me.chkUseStart.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseStart, "When checked, a task start time will be specified.")
        Me.chkUseStart.UseVisualStyleBackColor = True
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label29.Location = New System.Drawing.Point(7, 410)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(90, 26)
        Me.Label29.TabIndex = 20
        Me.Label29.Text = "Start task"
        '
        'chkUseLaunch
        '
        Me.chkUseLaunch.AutoSize = True
        Me.chkUseLaunch.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseLaunch.Location = New System.Drawing.Point(125, 374)
        Me.chkUseLaunch.Name = "chkUseLaunch"
        Me.chkUseLaunch.Size = New System.Drawing.Size(59, 30)
        Me.chkUseLaunch.TabIndex = 19
        Me.chkUseLaunch.Tag = "67"
        Me.chkUseLaunch.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseLaunch, "When checked, a launch time will be specified.")
        Me.chkUseLaunch.UseVisualStyleBackColor = True
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label28.Location = New System.Drawing.Point(7, 376)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(73, 26)
        Me.Label28.TabIndex = 18
        Me.Label28.Text = "Launch"
        '
        'chkUseSyncFly
        '
        Me.chkUseSyncFly.AutoSize = True
        Me.chkUseSyncFly.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkUseSyncFly.Location = New System.Drawing.Point(125, 340)
        Me.chkUseSyncFly.Name = "chkUseSyncFly"
        Me.chkUseSyncFly.Size = New System.Drawing.Size(59, 30)
        Me.chkUseSyncFly.TabIndex = 17
        Me.chkUseSyncFly.Tag = "66"
        Me.chkUseSyncFly.Text = "Yes"
        Me.ToolTip1.SetToolTip(Me.chkUseSyncFly, "When checked, a synchronized ""Click Fly"" will be specified.")
        Me.chkUseSyncFly.UseVisualStyleBackColor = True
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label27.Location = New System.Drawing.Point(7, 342)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(80, 26)
        Me.Label27.TabIndex = 16
        Me.Label27.Text = "Sync Fly"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(7, 271)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(155, 26)
        Me.Label25.TabIndex = 11
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
        Me.chkDateTimeUTC.TabIndex = 12
        Me.chkDateTimeUTC.Tag = "64"
        Me.chkDateTimeUTC.Text = "UTC / Zulu (local time if left unchecked)"
        Me.ToolTip1.SetToolTip(Me.chkDateTimeUTC, "When checked, the specified date and time are considered as UTC or Zulu.")
        Me.chkDateTimeUTC.UseVisualStyleBackColor = True
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(7, 308)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(136, 26)
        Me.Label26.TabIndex = 14
        Me.Label26.Text = "Meet / Briefing"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label24.Location = New System.Drawing.Point(7, 92)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(183, 26)
        Me.Label24.TabIndex = 2
        Me.Label24.Text = "Group or Club Name"
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
        Me.pnlEventDateTimeControls.TabIndex = 15
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
        'TimeStampContextualMenu
        '
        Me.TimeStampContextualMenu.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.TimeStampContextualMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetFullWithDayOfWeek, Me.GetLongDateTime, Me.GetTimeStampTimeOnlyWithoutSeconds, Me.GetCountdown, Me.GetTimeStampOnly})
        Me.TimeStampContextualMenu.Name = "TimeStampContextualMenu"
        Me.TimeStampContextualMenu.ShowImageMargin = False
        Me.TimeStampContextualMenu.Size = New System.Drawing.Size(224, 124)
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
        'GetTimeStampTimeOnlyWithoutSeconds
        '
        Me.GetTimeStampTimeOnlyWithoutSeconds.Name = "GetTimeStampTimeOnlyWithoutSeconds"
        Me.GetTimeStampTimeOnlyWithoutSeconds.Size = New System.Drawing.Size(223, 24)
        Me.GetTimeStampTimeOnlyWithoutSeconds.Text = "Time Only Without Seconds"
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
        'tabDiscord
        '
        Me.tabDiscord.Controls.Add(Me.GroupBox6)
        Me.tabDiscord.Controls.Add(Me.pnlWizardDiscord)
        Me.tabDiscord.Controls.Add(Me.pnlFullWorkflowTaskGroupFlight)
        Me.tabDiscord.Controls.Add(Me.grpDiscordEvent)
        Me.tabDiscord.Controls.Add(Me.grpDiscordOthers)
        Me.tabDiscord.Controls.Add(Me.txtAddOnsDetails)
        Me.tabDiscord.Controls.Add(Me.txtWaypointsDetails)
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
        Me.tabDiscord.Controls.Add(Me.GroupBox4)
        Me.tabDiscord.Controls.Add(Me.lblTaskBrowserIDAndDate)
        Me.tabDiscord.Location = New System.Drawing.Point(4, 29)
        Me.tabDiscord.Name = "tabDiscord"
        Me.tabDiscord.Size = New System.Drawing.Size(1467, 860)
        Me.tabDiscord.TabIndex = 3
        Me.tabDiscord.Text = "Discord and Browser"
        Me.tabDiscord.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.txtLastUpdateDescription)
        Me.GroupBox6.Controls.Add(Me.btnDeleteEventNews)
        Me.GroupBox6.Controls.Add(Me.btnPublishEventNews)
        Me.GroupBox6.Controls.Add(Me.btnDeleteFromTaskBrowser)
        Me.GroupBox6.Controls.Add(Me.btnUpdateInTaskBrowser)
        Me.GroupBox6.Controls.Add(Me.btnCreateInTaskBrowser)
        Me.GroupBox6.Location = New System.Drawing.Point(1240, 3)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(224, 342)
        Me.GroupBox6.TabIndex = 96
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Task Browser"
        '
        'txtLastUpdateDescription
        '
        Me.txtLastUpdateDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.txtLastUpdateDescription.Location = New System.Drawing.Point(6, 70)
        Me.txtLastUpdateDescription.Multiline = True
        Me.txtLastUpdateDescription.Name = "txtLastUpdateDescription"
        Me.txtLastUpdateDescription.Size = New System.Drawing.Size(212, 86)
        Me.txtLastUpdateDescription.TabIndex = 100
        Me.txtLastUpdateDescription.Tag = "87"
        Me.ToolTip1.SetToolTip(Me.txtLastUpdateDescription, "Enter a description for this task update")
        '
        'btnDeleteEventNews
        '
        Me.btnDeleteEventNews.Enabled = False
        Me.btnDeleteEventNews.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDeleteEventNews.Location = New System.Drawing.Point(6, 291)
        Me.btnDeleteEventNews.Name = "btnDeleteEventNews"
        Me.btnDeleteEventNews.Size = New System.Drawing.Size(212, 37)
        Me.btnDeleteEventNews.TabIndex = 99
        Me.btnDeleteEventNews.Tag = "87"
        Me.btnDeleteEventNews.Text = "Remove event News"
        Me.ToolTip1.SetToolTip(Me.btnDeleteEventNews, "Click this button to remove this group event's news entry.")
        Me.btnDeleteEventNews.UseVisualStyleBackColor = True
        '
        'btnPublishEventNews
        '
        Me.btnPublishEventNews.Enabled = False
        Me.btnPublishEventNews.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPublishEventNews.Location = New System.Drawing.Point(6, 248)
        Me.btnPublishEventNews.Name = "btnPublishEventNews"
        Me.btnPublishEventNews.Size = New System.Drawing.Size(212, 37)
        Me.btnPublishEventNews.TabIndex = 98
        Me.btnPublishEventNews.Tag = "87"
        Me.btnPublishEventNews.Text = "Publish event News"
        Me.ToolTip1.SetToolTip(Me.btnPublishEventNews, "Click this button to create or update this group event's news entry.")
        Me.btnPublishEventNews.UseVisualStyleBackColor = True
        '
        'btnDeleteFromTaskBrowser
        '
        Me.btnDeleteFromTaskBrowser.Enabled = False
        Me.btnDeleteFromTaskBrowser.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDeleteFromTaskBrowser.Location = New System.Drawing.Point(6, 205)
        Me.btnDeleteFromTaskBrowser.Name = "btnDeleteFromTaskBrowser"
        Me.btnDeleteFromTaskBrowser.Size = New System.Drawing.Size(212, 37)
        Me.btnDeleteFromTaskBrowser.TabIndex = 97
        Me.btnDeleteFromTaskBrowser.Tag = "87"
        Me.btnDeleteFromTaskBrowser.Text = "Remove task"
        Me.ToolTip1.SetToolTip(Me.btnDeleteFromTaskBrowser, "Click this button to remove this task for the online task browser.")
        Me.btnDeleteFromTaskBrowser.UseVisualStyleBackColor = True
        '
        'btnUpdateInTaskBrowser
        '
        Me.btnUpdateInTaskBrowser.Enabled = False
        Me.btnUpdateInTaskBrowser.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnUpdateInTaskBrowser.Location = New System.Drawing.Point(6, 162)
        Me.btnUpdateInTaskBrowser.Name = "btnUpdateInTaskBrowser"
        Me.btnUpdateInTaskBrowser.Size = New System.Drawing.Size(212, 37)
        Me.btnUpdateInTaskBrowser.TabIndex = 96
        Me.btnUpdateInTaskBrowser.Tag = "87"
        Me.btnUpdateInTaskBrowser.Text = "Update task"
        Me.ToolTip1.SetToolTip(Me.btnUpdateInTaskBrowser, "Click this button to update this task for the online task browser.")
        Me.btnUpdateInTaskBrowser.UseVisualStyleBackColor = True
        '
        'btnCreateInTaskBrowser
        '
        Me.btnCreateInTaskBrowser.Enabled = False
        Me.btnCreateInTaskBrowser.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateInTaskBrowser.Location = New System.Drawing.Point(6, 27)
        Me.btnCreateInTaskBrowser.Name = "btnCreateInTaskBrowser"
        Me.btnCreateInTaskBrowser.Size = New System.Drawing.Size(212, 37)
        Me.btnCreateInTaskBrowser.TabIndex = 95
        Me.btnCreateInTaskBrowser.Tag = "87"
        Me.btnCreateInTaskBrowser.Text = "Create task"
        Me.ToolTip1.SetToolTip(Me.btnCreateInTaskBrowser, "Click this button to create this task for the online task browser.")
        Me.btnCreateInTaskBrowser.UseVisualStyleBackColor = True
        '
        'pnlWizardDiscord
        '
        Me.pnlWizardDiscord.BackColor = System.Drawing.Color.Gray
        Me.pnlWizardDiscord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlWizardDiscord.Controls.Add(Me.btnDiscordGuideNext)
        Me.pnlWizardDiscord.Controls.Add(Me.Panel4)
        Me.pnlWizardDiscord.Controls.Add(Me.pnlDiscordArrow)
        Me.pnlWizardDiscord.Location = New System.Drawing.Point(90, 726)
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
        'pnlFullWorkflowTaskGroupFlight
        '
        Me.pnlFullWorkflowTaskGroupFlight.Controls.Add(Me.btnStartFullPostingWorkflow)
        Me.pnlFullWorkflowTaskGroupFlight.Location = New System.Drawing.Point(841, 104)
        Me.pnlFullWorkflowTaskGroupFlight.Name = "pnlFullWorkflowTaskGroupFlight"
        Me.pnlFullWorkflowTaskGroupFlight.Size = New System.Drawing.Size(393, 72)
        Me.pnlFullWorkflowTaskGroupFlight.TabIndex = 3
        Me.pnlFullWorkflowTaskGroupFlight.TabStop = False
        Me.pnlFullWorkflowTaskGroupFlight.Text = "Full Task and Group Flight Event"
        '
        'btnStartFullPostingWorkflow
        '
        Me.btnStartFullPostingWorkflow.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStartFullPostingWorkflow.Location = New System.Drawing.Point(6, 26)
        Me.btnStartFullPostingWorkflow.Name = "btnStartFullPostingWorkflow"
        Me.btnStartFullPostingWorkflow.Size = New System.Drawing.Size(381, 37)
        Me.btnStartFullPostingWorkflow.TabIndex = 0
        Me.btnStartFullPostingWorkflow.Tag = "86"
        Me.btnStartFullPostingWorkflow.Text = "Start Full Workflow for Group Flight and Task"
        Me.ToolTip1.SetToolTip(Me.btnStartFullPostingWorkflow, "Click this button to begin posting the entire details for the group flight and ta" &
        "sk.")
        Me.btnStartFullPostingWorkflow.UseVisualStyleBackColor = True
        '
        'grpDiscordEvent
        '
        Me.grpDiscordEvent.Controls.Add(Me.Label19)
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
        Me.grpDiscordEvent.Location = New System.Drawing.Point(841, 351)
        Me.grpDiscordEvent.Name = "grpDiscordEvent"
        Me.grpDiscordEvent.Size = New System.Drawing.Size(623, 329)
        Me.grpDiscordEvent.TabIndex = 5
        Me.grpDiscordEvent.TabStop = False
        Me.grpDiscordEvent.Text = "Official Discord Event (if applicable)"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(239, 191)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(237, 22)
        Me.Label19.TabIndex = 10
        Me.Label19.Text = "(min. 800px wide by 320px tall)"
        '
        'btnDiscordSharedEventURL
        '
        Me.btnDiscordSharedEventURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordSharedEventURL.Location = New System.Drawing.Point(536, 282)
        Me.btnDiscordSharedEventURL.Name = "btnDiscordSharedEventURL"
        Me.btnDiscordSharedEventURL.Size = New System.Drawing.Size(81, 29)
        Me.btnDiscordSharedEventURL.TabIndex = 14
        Me.btnDiscordSharedEventURL.Tag = "98"
        Me.btnDiscordSharedEventURL.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnDiscordSharedEventURL, "Click this button to paste the group event's post URL from your clipboard")
        Me.btnDiscordSharedEventURL.UseVisualStyleBackColor = True
        '
        'txtDiscordEventShareURL
        '
        Me.txtDiscordEventShareURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.txtDiscordEventShareURL.Location = New System.Drawing.Point(23, 281)
        Me.txtDiscordEventShareURL.Name = "txtDiscordEventShareURL"
        Me.txtDiscordEventShareURL.Size = New System.Drawing.Size(507, 30)
        Me.txtDiscordEventShareURL.TabIndex = 13
        Me.txtDiscordEventShareURL.Tag = "98"
        Me.ToolTip1.SetToolTip(Me.txtDiscordEventShareURL, "Enter the URL to the Discord post created above in step 1.")
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(19, 249)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(252, 22)
        Me.Label20.TabIndex = 12
        Me.Label20.Text = "Copy and paste the link to share:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(1, 26)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(227, 22)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "1. Create a new Discord Event"
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label46.Location = New System.Drawing.Point(2, 223)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(303, 22)
        Me.Label46.TabIndex = 11
        Me.Label46.Text = "7. Preview and post your Discord Event!"
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label45.Location = New System.Drawing.Point(2, 191)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(241, 22)
        Me.Label45.TabIndex = 9
        Me.Label45.Text = "6. Upload optional cover image"
        '
        'btnEventDescriptionToClipboard
        '
        Me.btnEventDescriptionToClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventDescriptionToClipboard.Location = New System.Drawing.Point(203, 155)
        Me.btnEventDescriptionToClipboard.Name = "btnEventDescriptionToClipboard"
        Me.btnEventDescriptionToClipboard.Size = New System.Drawing.Size(302, 29)
        Me.btnEventDescriptionToClipboard.TabIndex = 8
        Me.btnEventDescriptionToClipboard.Tag = "95"
        Me.btnEventDescriptionToClipboard.Text = "Event Description to Clipboard"
        Me.ToolTip1.SetToolTip(Me.btnEventDescriptionToClipboard, "Click this button to copy the event's full description for the Discord Event post" &
        " into the clipboard.")
        Me.btnEventDescriptionToClipboard.UseVisualStyleBackColor = True
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label44.Location = New System.Drawing.Point(1, 158)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(200, 22)
        Me.Label44.TabIndex = 7
        Me.Label44.Text = "5. Enter Event Description"
        '
        'lblDiscordPostDateTime
        '
        Me.lblDiscordPostDateTime.AutoSize = True
        Me.lblDiscordPostDateTime.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordPostDateTime.Location = New System.Drawing.Point(276, 126)
        Me.lblDiscordPostDateTime.Name = "lblDiscordPostDateTime"
        Me.lblDiscordPostDateTime.Size = New System.Drawing.Size(139, 22)
        Me.lblDiscordPostDateTime.TabIndex = 6
        Me.lblDiscordPostDateTime.Text = "meet time results"
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label43.Location = New System.Drawing.Point(2, 126)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(268, 22)
        Me.Label43.TabIndex = 5
        Me.Label43.Text = "4. Specify local start date and time:"
        '
        'btnEventTopicClipboard
        '
        Me.btnEventTopicClipboard.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventTopicClipboard.Location = New System.Drawing.Point(203, 90)
        Me.btnEventTopicClipboard.Name = "btnEventTopicClipboard"
        Me.btnEventTopicClipboard.Size = New System.Drawing.Size(302, 29)
        Me.btnEventTopicClipboard.TabIndex = 4
        Me.btnEventTopicClipboard.Tag = "93"
        Me.btnEventTopicClipboard.Text = "Event Topic to Clipboard"
        Me.ToolTip1.SetToolTip(Me.btnEventTopicClipboard, "Click this button to copy the event's topic for the Discord Event post into the c" &
        "lipboard.")
        Me.btnEventTopicClipboard.UseVisualStyleBackColor = True
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label42.Location = New System.Drawing.Point(1, 93)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(153, 22)
        Me.Label42.TabIndex = 3
        Me.Label42.Text = "3. Enter Event Topic"
        '
        'lblDiscordEventVoice
        '
        Me.lblDiscordEventVoice.AutoSize = True
        Me.lblDiscordEventVoice.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDiscordEventVoice.Location = New System.Drawing.Point(193, 59)
        Me.lblDiscordEventVoice.Name = "lblDiscordEventVoice"
        Me.lblDiscordEventVoice.Size = New System.Drawing.Size(111, 22)
        Me.lblDiscordEventVoice.TabIndex = 2
        Me.lblDiscordEventVoice.Text = "voice channel"
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label39.Location = New System.Drawing.Point(1, 59)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(187, 22)
        Me.Label39.TabIndex = 1
        Me.Label39.Text = "2. Select Voice Channel:"
        '
        'grpDiscordOthers
        '
        Me.grpDiscordOthers.Controls.Add(Me.btnTaskAndGroupEventLinks)
        Me.grpDiscordOthers.Controls.Add(Me.btnTaskFeaturedOnGroupFlight)
        Me.grpDiscordOthers.Controls.Add(Me.btnEventDPHXAndLinkOnly)
        Me.grpDiscordOthers.Location = New System.Drawing.Point(841, 182)
        Me.grpDiscordOthers.Name = "grpDiscordOthers"
        Me.grpDiscordOthers.Size = New System.Drawing.Size(393, 163)
        Me.grpDiscordOthers.TabIndex = 4
        Me.grpDiscordOthers.TabStop = False
        Me.grpDiscordOthers.Text = "Other individual posts"
        '
        'btnTaskAndGroupEventLinks
        '
        Me.btnTaskAndGroupEventLinks.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTaskAndGroupEventLinks.Location = New System.Drawing.Point(6, 112)
        Me.btnTaskAndGroupEventLinks.Name = "btnTaskAndGroupEventLinks"
        Me.btnTaskAndGroupEventLinks.Size = New System.Drawing.Size(381, 37)
        Me.btnTaskAndGroupEventLinks.TabIndex = 2
        Me.btnTaskAndGroupEventLinks.Tag = "89"
        Me.btnTaskAndGroupEventLinks.Text = "Task and Group Event links"
        Me.ToolTip1.SetToolTip(Me.btnTaskAndGroupEventLinks, "Click here to get a message with links to the task and group flight event posts.")
        Me.btnTaskAndGroupEventLinks.UseVisualStyleBackColor = True
        '
        'btnTaskFeaturedOnGroupFlight
        '
        Me.btnTaskFeaturedOnGroupFlight.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTaskFeaturedOnGroupFlight.Location = New System.Drawing.Point(6, 26)
        Me.btnTaskFeaturedOnGroupFlight.Name = "btnTaskFeaturedOnGroupFlight"
        Me.btnTaskFeaturedOnGroupFlight.Size = New System.Drawing.Size(381, 37)
        Me.btnTaskFeaturedOnGroupFlight.TabIndex = 0
        Me.btnTaskFeaturedOnGroupFlight.Tag = "87"
        Me.btnTaskFeaturedOnGroupFlight.Text = "Task featured on group flight"
        Me.ToolTip1.SetToolTip(Me.btnTaskFeaturedOnGroupFlight, "Click this button to copy the message to post on the task and receive instruction" &
        "s to paste it in the Discord.")
        Me.btnTaskFeaturedOnGroupFlight.UseVisualStyleBackColor = True
        '
        'btnEventDPHXAndLinkOnly
        '
        Me.btnEventDPHXAndLinkOnly.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEventDPHXAndLinkOnly.Location = New System.Drawing.Point(6, 69)
        Me.btnEventDPHXAndLinkOnly.Name = "btnEventDPHXAndLinkOnly"
        Me.btnEventDPHXAndLinkOnly.Size = New System.Drawing.Size(381, 37)
        Me.btnEventDPHXAndLinkOnly.TabIndex = 1
        Me.btnEventDPHXAndLinkOnly.Tag = "88"
        Me.btnEventDPHXAndLinkOnly.Text = "DPHX and link only"
        Me.ToolTip1.SetToolTip(Me.btnEventDPHXAndLinkOnly, "Click this button to only include the DPHX file and link to task post.")
        Me.btnEventDPHXAndLinkOnly.UseVisualStyleBackColor = True
        '
        'txtAddOnsDetails
        '
        Me.txtAddOnsDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddOnsDetails.Location = New System.Drawing.Point(1201, 814)
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
        Me.txtWaypointsDetails.Location = New System.Drawing.Point(1136, 819)
        Me.txtWaypointsDetails.Multiline = True
        Me.txtWaypointsDetails.Name = "txtWaypointsDetails"
        Me.txtWaypointsDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtWaypointsDetails.Size = New System.Drawing.Size(59, 23)
        Me.txtWaypointsDetails.TabIndex = 92
        Me.txtWaypointsDetails.Tag = "25"
        Me.ToolTip1.SetToolTip(Me.txtWaypointsDetails, "This is the full description content for the fourth and last Discord post.")
        Me.txtWaypointsDetails.Visible = False
        '
        'txtGroupFlightEventPost
        '
        Me.txtGroupFlightEventPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGroupFlightEventPost.Location = New System.Drawing.Point(1266, 814)
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
        Me.grpDiscordTask.Controls.Add(Me.GroupBox1)
        Me.grpDiscordTask.Location = New System.Drawing.Point(8, 3)
        Me.grpDiscordTask.Name = "grpDiscordTask"
        Me.grpDiscordTask.Size = New System.Drawing.Size(405, 719)
        Me.grpDiscordTask.TabIndex = 0
        Me.grpDiscordTask.TabStop = False
        Me.grpDiscordTask.Text = "Task (for Task Library)"
        '
        'grpRepost
        '
        Me.grpRepost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpRepost.Controls.Add(Me.btnRepostOriginalURLPaste)
        Me.grpRepost.Controls.Add(Me.txtRepostOriginalURL)
        Me.grpRepost.Controls.Add(Me.dtRepostOriginalDate)
        Me.grpRepost.Controls.Add(Me.chkRepost)
        Me.grpRepost.Location = New System.Drawing.Point(6, 23)
        Me.grpRepost.Name = "grpRepost"
        Me.grpRepost.Size = New System.Drawing.Size(393, 92)
        Me.grpRepost.TabIndex = 0
        Me.grpRepost.TabStop = False
        '
        'btnRepostOriginalURLPaste
        '
        Me.btnRepostOriginalURLPaste.Enabled = False
        Me.btnRepostOriginalURLPaste.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRepostOriginalURLPaste.Location = New System.Drawing.Point(313, 57)
        Me.btnRepostOriginalURLPaste.Name = "btnRepostOriginalURLPaste"
        Me.btnRepostOriginalURLPaste.Size = New System.Drawing.Size(74, 29)
        Me.btnRepostOriginalURLPaste.TabIndex = 3
        Me.btnRepostOriginalURLPaste.Tag = "40"
        Me.btnRepostOriginalURLPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnRepostOriginalURLPaste, "Click this button to paste the original task post URL from your clipboard")
        Me.btnRepostOriginalURLPaste.UseVisualStyleBackColor = True
        '
        'txtRepostOriginalURL
        '
        Me.txtRepostOriginalURL.Enabled = False
        Me.txtRepostOriginalURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.txtRepostOriginalURL.Location = New System.Drawing.Point(6, 55)
        Me.txtRepostOriginalURL.Name = "txtRepostOriginalURL"
        Me.txtRepostOriginalURL.Size = New System.Drawing.Size(301, 30)
        Me.txtRepostOriginalURL.TabIndex = 2
        Me.txtRepostOriginalURL.Tag = "40"
        Me.ToolTip1.SetToolTip(Me.txtRepostOriginalURL, "Enter the URL to the original task post.")
        '
        'dtRepostOriginalDate
        '
        Me.dtRepostOriginalDate.Enabled = False
        Me.dtRepostOriginalDate.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtRepostOriginalDate.Location = New System.Drawing.Point(6, 23)
        Me.dtRepostOriginalDate.Name = "dtRepostOriginalDate"
        Me.dtRepostOriginalDate.Size = New System.Drawing.Size(381, 31)
        Me.dtRepostOriginalDate.TabIndex = 1
        Me.dtRepostOriginalDate.Tag = "40"
        Me.ToolTip1.SetToolTip(Me.dtRepostOriginalDate, "Date of original posting (source)")
        '
        'chkRepost
        '
        Me.chkRepost.AutoSize = True
        Me.chkRepost.BackColor = System.Drawing.SystemColors.Control
        Me.chkRepost.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkRepost.Location = New System.Drawing.Point(8, -3)
        Me.chkRepost.Name = "chkRepost"
        Me.chkRepost.Size = New System.Drawing.Size(299, 24)
        Me.chkRepost.TabIndex = 0
        Me.chkRepost.Tag = "40"
        Me.chkRepost.Text = "External task original date and source URL"
        Me.ToolTip1.SetToolTip(Me.chkRepost, "Check this if you are reposting a previous task published somewhere else (not the" &
        " Task Library)")
        Me.chkRepost.UseVisualStyleBackColor = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.btnDPORecallSettings)
        Me.GroupBox1.Controls.Add(Me.btnDPORememberSettings)
        Me.GroupBox1.Controls.Add(Me.flpDiscordPostOptions)
        Me.GroupBox1.Controls.Add(Me.btnDPOResetToDefault)
        Me.GroupBox1.Controls.Add(Me.btnStartTaskPost)
        Me.GroupBox1.Controls.Add(Me.lblTaskLibraryIDNotAcquired)
        Me.GroupBox1.Controls.Add(Me.lblTaskLibraryIDAcquired)
        Me.GroupBox1.Controls.Add(Me.lblThread1stMsgIDNotAcquired)
        Me.GroupBox1.Controls.Add(Me.lblThread1stMsgIDAcquired)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 116)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(393, 595)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Discord Post Options"
        '
        'btnDPORecallSettings
        '
        Me.btnDPORecallSettings.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.btnDPORecallSettings.Location = New System.Drawing.Point(4, 510)
        Me.btnDPORecallSettings.Name = "btnDPORecallSettings"
        Me.btnDPORecallSettings.Size = New System.Drawing.Size(124, 35)
        Me.btnDPORecallSettings.TabIndex = 2
        Me.btnDPORecallSettings.Tag = "42"
        Me.btnDPORecallSettings.Text = "Recall"
        Me.ToolTip1.SetToolTip(Me.btnDPORecallSettings, "Click to recall the remembered set of options.")
        Me.btnDPORecallSettings.UseVisualStyleBackColor = True
        '
        'btnDPORememberSettings
        '
        Me.btnDPORememberSettings.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.btnDPORememberSettings.Location = New System.Drawing.Point(134, 510)
        Me.btnDPORememberSettings.Name = "btnDPORememberSettings"
        Me.btnDPORememberSettings.Size = New System.Drawing.Size(124, 35)
        Me.btnDPORememberSettings.TabIndex = 3
        Me.btnDPORememberSettings.Tag = "42"
        Me.btnDPORememberSettings.Text = "Remember"
        Me.ToolTip1.SetToolTip(Me.btnDPORememberSettings, "Click to remember (save) this set of options for future posts.")
        Me.btnDPORememberSettings.UseVisualStyleBackColor = True
        '
        'flpDiscordPostOptions
        '
        Me.flpDiscordPostOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel1)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel2)
        Me.flpDiscordPostOptions.Controls.Add(Me.Label3)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel4)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel3)
        Me.flpDiscordPostOptions.Controls.Add(Me.Label8)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel5)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel6)
        Me.flpDiscordPostOptions.Controls.Add(Me.Label12)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel7)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel8)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel9)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel10)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel11)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel12)
        Me.flpDiscordPostOptions.Controls.Add(Me.Label37)
        Me.flpDiscordPostOptions.Controls.Add(Me.FlowLayoutPanel13)
        Me.flpDiscordPostOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpDiscordPostOptions.Location = New System.Drawing.Point(0, 26)
        Me.flpDiscordPostOptions.Name = "flpDiscordPostOptions"
        Me.flpDiscordPostOptions.Size = New System.Drawing.Size(393, 420)
        Me.flpDiscordPostOptions.TabIndex = 0
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.chkDPOMainPost)
        Me.FlowLayoutPanel1.Controls.Add(Me.lblNbrCarsMainFP)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 0)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'chkDPOMainPost
        '
        Me.chkDPOMainPost.AutoSize = True
        Me.chkDPOMainPost.Checked = True
        Me.chkDPOMainPost.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOMainPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOMainPost.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOMainPost.Name = "chkDPOMainPost"
        Me.chkDPOMainPost.Size = New System.Drawing.Size(103, 26)
        Me.chkDPOMainPost.TabIndex = 0
        Me.chkDPOMainPost.Tag = "41"
        Me.chkDPOMainPost.Text = "Main post"
        Me.ToolTip1.SetToolTip(Me.chkDPOMainPost, "Select if you want to include the task's main information.")
        Me.chkDPOMainPost.UseVisualStyleBackColor = True
        '
        'lblNbrCarsMainFP
        '
        Me.lblNbrCarsMainFP.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblNbrCarsMainFP.AutoSize = True
        Me.lblNbrCarsMainFP.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.lblNbrCarsMainFP.ForeColor = System.Drawing.Color.Red
        Me.lblNbrCarsMainFP.Location = New System.Drawing.Point(112, 4)
        Me.lblNbrCarsMainFP.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsMainFP.Name = "lblNbrCarsMainFP"
        Me.lblNbrCarsMainFP.Size = New System.Drawing.Size(19, 22)
        Me.lblNbrCarsMainFP.TabIndex = 1
        Me.lblNbrCarsMainFP.Text = "0"
        Me.lblNbrCarsMainFP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.lblNbrCarsMainFP, "Number of characters for the task's main information post.")
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel2.Controls.Add(Me.chkDPOThreadCreation)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(3, 26)
        Me.FlowLayoutPanel2.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel2.TabIndex = 1
        '
        'chkDPOThreadCreation
        '
        Me.chkDPOThreadCreation.AutoSize = True
        Me.chkDPOThreadCreation.Checked = True
        Me.chkDPOThreadCreation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOThreadCreation.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOThreadCreation.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOThreadCreation.Name = "chkDPOThreadCreation"
        Me.chkDPOThreadCreation.Size = New System.Drawing.Size(145, 26)
        Me.chkDPOThreadCreation.TabIndex = 0
        Me.chkDPOThreadCreation.Tag = "41"
        Me.chkDPOThreadCreation.Text = "Thread creation"
        Me.ToolTip1.SetToolTip(Me.chkDPOThreadCreation, "Select if you want to include the creation of the task's thread.")
        Me.chkDPOThreadCreation.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.Enabled = False
        Me.Label3.Location = New System.Drawing.Point(3, 52)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(227, 20)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = " "
        '
        'FlowLayoutPanel4
        '
        Me.FlowLayoutPanel4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel4.Controls.Add(Me.chkDPOIncludeCoverImage)
        Me.FlowLayoutPanel4.Location = New System.Drawing.Point(3, 72)
        Me.FlowLayoutPanel4.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel4.Name = "FlowLayoutPanel4"
        Me.FlowLayoutPanel4.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel4.TabIndex = 2
        '
        'chkDPOIncludeCoverImage
        '
        Me.chkDPOIncludeCoverImage.AutoSize = True
        Me.chkDPOIncludeCoverImage.Checked = True
        Me.chkDPOIncludeCoverImage.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOIncludeCoverImage.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOIncludeCoverImage.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOIncludeCoverImage.Name = "chkDPOIncludeCoverImage"
        Me.chkDPOIncludeCoverImage.Size = New System.Drawing.Size(123, 26)
        Me.chkDPOIncludeCoverImage.TabIndex = 0
        Me.chkDPOIncludeCoverImage.Tag = "41"
        Me.chkDPOIncludeCoverImage.Text = "Cover image"
        Me.ToolTip1.SetToolTip(Me.chkDPOIncludeCoverImage, "Select if you want to include the cover image.")
        Me.chkDPOIncludeCoverImage.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel3
        '
        Me.FlowLayoutPanel3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel3.Controls.Add(Me.chkDPOFullDescription)
        Me.FlowLayoutPanel3.Controls.Add(Me.lblNbrCarsFullDescResults)
        Me.FlowLayoutPanel3.Location = New System.Drawing.Point(3, 98)
        Me.FlowLayoutPanel3.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel3.Name = "FlowLayoutPanel3"
        Me.FlowLayoutPanel3.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel3.TabIndex = 3
        '
        'chkDPOFullDescription
        '
        Me.chkDPOFullDescription.AutoSize = True
        Me.chkDPOFullDescription.Checked = True
        Me.chkDPOFullDescription.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOFullDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOFullDescription.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOFullDescription.Name = "chkDPOFullDescription"
        Me.chkDPOFullDescription.Size = New System.Drawing.Size(143, 26)
        Me.chkDPOFullDescription.TabIndex = 0
        Me.chkDPOFullDescription.Tag = "41"
        Me.chkDPOFullDescription.Text = "Full description"
        Me.ToolTip1.SetToolTip(Me.chkDPOFullDescription, "Select if you want to include the full description.")
        Me.chkDPOFullDescription.UseVisualStyleBackColor = True
        '
        'lblNbrCarsFullDescResults
        '
        Me.lblNbrCarsFullDescResults.AutoSize = True
        Me.lblNbrCarsFullDescResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.lblNbrCarsFullDescResults.ForeColor = System.Drawing.Color.Red
        Me.lblNbrCarsFullDescResults.Location = New System.Drawing.Point(152, 4)
        Me.lblNbrCarsFullDescResults.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsFullDescResults.Name = "lblNbrCarsFullDescResults"
        Me.lblNbrCarsFullDescResults.Size = New System.Drawing.Size(19, 22)
        Me.lblNbrCarsFullDescResults.TabIndex = 1
        Me.lblNbrCarsFullDescResults.Text = "0"
        Me.lblNbrCarsFullDescResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label8
        '
        Me.Label8.Enabled = False
        Me.Label8.Location = New System.Drawing.Point(3, 124)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(390, 20)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = " "
        '
        'FlowLayoutPanel5
        '
        Me.FlowLayoutPanel5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel5.Controls.Add(Me.chkDPOFilesWithDescription)
        Me.FlowLayoutPanel5.Location = New System.Drawing.Point(3, 144)
        Me.FlowLayoutPanel5.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel5.Name = "FlowLayoutPanel5"
        Me.FlowLayoutPanel5.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel5.TabIndex = 4
        '
        'chkDPOFilesWithDescription
        '
        Me.chkDPOFilesWithDescription.AutoSize = True
        Me.chkDPOFilesWithDescription.Checked = True
        Me.chkDPOFilesWithDescription.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOFilesWithDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOFilesWithDescription.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOFilesWithDescription.Name = "chkDPOFilesWithDescription"
        Me.chkDPOFilesWithDescription.Size = New System.Drawing.Size(252, 26)
        Me.chkDPOFilesWithDescription.TabIndex = 0
        Me.chkDPOFilesWithDescription.Tag = "41"
        Me.chkDPOFilesWithDescription.Text = "Files with description / legend"
        Me.ToolTip1.SetToolTip(Me.chkDPOFilesWithDescription, "Select if you want to include the file description along with the files.")
        Me.chkDPOFilesWithDescription.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel6
        '
        Me.FlowLayoutPanel6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel6.Controls.Add(Me.chkDPOFilesAlone)
        Me.FlowLayoutPanel6.Location = New System.Drawing.Point(3, 170)
        Me.FlowLayoutPanel6.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel6.Name = "FlowLayoutPanel6"
        Me.FlowLayoutPanel6.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel6.TabIndex = 5
        '
        'chkDPOFilesAlone
        '
        Me.chkDPOFilesAlone.AutoSize = True
        Me.chkDPOFilesAlone.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOFilesAlone.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOFilesAlone.Name = "chkDPOFilesAlone"
        Me.chkDPOFilesAlone.Size = New System.Drawing.Size(277, 26)
        Me.chkDPOFilesAlone.TabIndex = 0
        Me.chkDPOFilesAlone.Tag = "41"
        Me.chkDPOFilesAlone.Text = "Files without description / legend"
        Me.ToolTip1.SetToolTip(Me.chkDPOFilesAlone, "Select if you want to post only the files without their description.")
        Me.chkDPOFilesAlone.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.Enabled = False
        Me.Label12.Location = New System.Drawing.Point(3, 196)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(390, 20)
        Me.Label12.TabIndex = 2
        Me.Label12.Text = " "
        '
        'FlowLayoutPanel7
        '
        Me.FlowLayoutPanel7.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel7.Controls.Add(Me.chkDPOAltRestrictions)
        Me.FlowLayoutPanel7.Controls.Add(Me.lblNbrCarsRestrictions)
        Me.FlowLayoutPanel7.Location = New System.Drawing.Point(3, 216)
        Me.FlowLayoutPanel7.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel7.Name = "FlowLayoutPanel7"
        Me.FlowLayoutPanel7.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel7.TabIndex = 7
        '
        'chkDPOAltRestrictions
        '
        Me.chkDPOAltRestrictions.AutoSize = True
        Me.chkDPOAltRestrictions.Checked = True
        Me.chkDPOAltRestrictions.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOAltRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOAltRestrictions.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOAltRestrictions.Name = "chkDPOAltRestrictions"
        Me.chkDPOAltRestrictions.Size = New System.Drawing.Size(177, 26)
        Me.chkDPOAltRestrictions.TabIndex = 0
        Me.chkDPOAltRestrictions.Tag = "41"
        Me.chkDPOAltRestrictions.Text = "Altitude restrictions"
        Me.ToolTip1.SetToolTip(Me.chkDPOAltRestrictions, "Select if you want to include the altitude restrictions.")
        Me.chkDPOAltRestrictions.UseVisualStyleBackColor = True
        '
        'lblNbrCarsRestrictions
        '
        Me.lblNbrCarsRestrictions.AutoSize = True
        Me.lblNbrCarsRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.lblNbrCarsRestrictions.Location = New System.Drawing.Point(186, 4)
        Me.lblNbrCarsRestrictions.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsRestrictions.Name = "lblNbrCarsRestrictions"
        Me.lblNbrCarsRestrictions.Size = New System.Drawing.Size(19, 22)
        Me.lblNbrCarsRestrictions.TabIndex = 1
        Me.lblNbrCarsRestrictions.Text = "0"
        '
        'FlowLayoutPanel8
        '
        Me.FlowLayoutPanel8.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel8.Controls.Add(Me.chkDPOWeatherInfo)
        Me.FlowLayoutPanel8.Controls.Add(Me.lblNbrCarsWeather)
        Me.FlowLayoutPanel8.Location = New System.Drawing.Point(3, 242)
        Me.FlowLayoutPanel8.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel8.Name = "FlowLayoutPanel8"
        Me.FlowLayoutPanel8.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel8.TabIndex = 8
        '
        'chkDPOWeatherInfo
        '
        Me.chkDPOWeatherInfo.AutoSize = True
        Me.chkDPOWeatherInfo.Checked = True
        Me.chkDPOWeatherInfo.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOWeatherInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOWeatherInfo.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOWeatherInfo.Name = "chkDPOWeatherInfo"
        Me.chkDPOWeatherInfo.Size = New System.Drawing.Size(143, 26)
        Me.chkDPOWeatherInfo.TabIndex = 0
        Me.chkDPOWeatherInfo.Tag = "41"
        Me.chkDPOWeatherInfo.Text = "Weather details"
        Me.ToolTip1.SetToolTip(Me.chkDPOWeatherInfo, "Select if you want to include the weather details.")
        Me.chkDPOWeatherInfo.UseVisualStyleBackColor = True
        '
        'lblNbrCarsWeather
        '
        Me.lblNbrCarsWeather.AutoSize = True
        Me.lblNbrCarsWeather.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.lblNbrCarsWeather.Location = New System.Drawing.Point(152, 4)
        Me.lblNbrCarsWeather.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsWeather.Name = "lblNbrCarsWeather"
        Me.lblNbrCarsWeather.Size = New System.Drawing.Size(19, 22)
        Me.lblNbrCarsWeather.TabIndex = 1
        Me.lblNbrCarsWeather.Text = "0"
        '
        'FlowLayoutPanel9
        '
        Me.FlowLayoutPanel9.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel9.Controls.Add(Me.chkDPOWeatherChart)
        Me.FlowLayoutPanel9.Location = New System.Drawing.Point(3, 268)
        Me.FlowLayoutPanel9.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel9.Name = "FlowLayoutPanel9"
        Me.FlowLayoutPanel9.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel9.TabIndex = 9
        '
        'chkDPOWeatherChart
        '
        Me.chkDPOWeatherChart.AutoSize = True
        Me.chkDPOWeatherChart.Checked = True
        Me.chkDPOWeatherChart.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOWeatherChart.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOWeatherChart.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOWeatherChart.Name = "chkDPOWeatherChart"
        Me.chkDPOWeatherChart.Size = New System.Drawing.Size(132, 26)
        Me.chkDPOWeatherChart.TabIndex = 0
        Me.chkDPOWeatherChart.Tag = "41"
        Me.chkDPOWeatherChart.Text = "Weather chart"
        Me.ToolTip1.SetToolTip(Me.chkDPOWeatherChart, "Select if you want to include the weather chart.")
        Me.chkDPOWeatherChart.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel10
        '
        Me.FlowLayoutPanel10.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel10.Controls.Add(Me.chkDPOWaypoints)
        Me.FlowLayoutPanel10.Controls.Add(Me.lblNbrCarsWaypoints)
        Me.FlowLayoutPanel10.Location = New System.Drawing.Point(3, 294)
        Me.FlowLayoutPanel10.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel10.Name = "FlowLayoutPanel10"
        Me.FlowLayoutPanel10.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel10.TabIndex = 10
        '
        'chkDPOWaypoints
        '
        Me.chkDPOWaypoints.AutoSize = True
        Me.chkDPOWaypoints.Checked = True
        Me.chkDPOWaypoints.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOWaypoints.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOWaypoints.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOWaypoints.Name = "chkDPOWaypoints"
        Me.chkDPOWaypoints.Size = New System.Drawing.Size(107, 26)
        Me.chkDPOWaypoints.TabIndex = 0
        Me.chkDPOWaypoints.Tag = "41"
        Me.chkDPOWaypoints.Text = "Waypoints"
        Me.ToolTip1.SetToolTip(Me.chkDPOWaypoints, "Select if you want to include the waypoints details.")
        Me.chkDPOWaypoints.UseVisualStyleBackColor = True
        '
        'lblNbrCarsWaypoints
        '
        Me.lblNbrCarsWaypoints.AutoSize = True
        Me.lblNbrCarsWaypoints.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.lblNbrCarsWaypoints.Location = New System.Drawing.Point(116, 4)
        Me.lblNbrCarsWaypoints.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsWaypoints.Name = "lblNbrCarsWaypoints"
        Me.lblNbrCarsWaypoints.Size = New System.Drawing.Size(19, 22)
        Me.lblNbrCarsWaypoints.TabIndex = 1
        Me.lblNbrCarsWaypoints.Text = "0"
        '
        'FlowLayoutPanel11
        '
        Me.FlowLayoutPanel11.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel11.Controls.Add(Me.chkDPOAddOns)
        Me.FlowLayoutPanel11.Controls.Add(Me.lblNbrCarsAddOns)
        Me.FlowLayoutPanel11.Location = New System.Drawing.Point(3, 320)
        Me.FlowLayoutPanel11.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel11.Name = "FlowLayoutPanel11"
        Me.FlowLayoutPanel11.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel11.TabIndex = 11
        '
        'chkDPOAddOns
        '
        Me.chkDPOAddOns.AutoSize = True
        Me.chkDPOAddOns.Checked = True
        Me.chkDPOAddOns.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOAddOns.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOAddOns.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOAddOns.Name = "chkDPOAddOns"
        Me.chkDPOAddOns.Size = New System.Drawing.Size(208, 26)
        Me.chkDPOAddOns.TabIndex = 0
        Me.chkDPOAddOns.Tag = "41"
        Me.chkDPOAddOns.Text = "Recommended add-ons"
        Me.ToolTip1.SetToolTip(Me.chkDPOAddOns, "Select if you want to include the recommended add-ons.")
        Me.chkDPOAddOns.UseVisualStyleBackColor = True
        '
        'lblNbrCarsAddOns
        '
        Me.lblNbrCarsAddOns.AutoSize = True
        Me.lblNbrCarsAddOns.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.lblNbrCarsAddOns.Location = New System.Drawing.Point(217, 4)
        Me.lblNbrCarsAddOns.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsAddOns.Name = "lblNbrCarsAddOns"
        Me.lblNbrCarsAddOns.Size = New System.Drawing.Size(19, 22)
        Me.lblNbrCarsAddOns.TabIndex = 1
        Me.lblNbrCarsAddOns.Text = "0"
        '
        'FlowLayoutPanel12
        '
        Me.FlowLayoutPanel12.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel12.Controls.Add(Me.chkDPOResultsInvitation)
        Me.FlowLayoutPanel12.Location = New System.Drawing.Point(3, 346)
        Me.FlowLayoutPanel12.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel12.Name = "FlowLayoutPanel12"
        Me.FlowLayoutPanel12.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel12.TabIndex = 12
        '
        'chkDPOResultsInvitation
        '
        Me.chkDPOResultsInvitation.AutoSize = True
        Me.chkDPOResultsInvitation.Checked = True
        Me.chkDPOResultsInvitation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOResultsInvitation.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOResultsInvitation.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOResultsInvitation.Name = "chkDPOResultsInvitation"
        Me.chkDPOResultsInvitation.Size = New System.Drawing.Size(209, 26)
        Me.chkDPOResultsInvitation.TabIndex = 0
        Me.chkDPOResultsInvitation.Tag = "41"
        Me.chkDPOResultsInvitation.Text = "Invitation to post results"
        Me.ToolTip1.SetToolTip(Me.chkDPOResultsInvitation, "Select if you want to include an invitation to post results in the thread (only w" &
        "hen task has been published on the Task Library).")
        Me.chkDPOResultsInvitation.UseVisualStyleBackColor = True
        '
        'Label37
        '
        Me.Label37.Enabled = False
        Me.Label37.Location = New System.Drawing.Point(3, 372)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(390, 20)
        Me.Label37.TabIndex = 3
        Me.Label37.Text = " "
        '
        'FlowLayoutPanel13
        '
        Me.FlowLayoutPanel13.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel13.Controls.Add(Me.chkDPOFeaturedOnGroupFlight)
        Me.FlowLayoutPanel13.Location = New System.Drawing.Point(3, 392)
        Me.FlowLayoutPanel13.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel13.Name = "FlowLayoutPanel13"
        Me.FlowLayoutPanel13.Size = New System.Drawing.Size(393, 26)
        Me.FlowLayoutPanel13.TabIndex = 13
        '
        'chkDPOFeaturedOnGroupFlight
        '
        Me.chkDPOFeaturedOnGroupFlight.AutoSize = True
        Me.chkDPOFeaturedOnGroupFlight.Checked = True
        Me.chkDPOFeaturedOnGroupFlight.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDPOFeaturedOnGroupFlight.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOFeaturedOnGroupFlight.Location = New System.Drawing.Point(3, 3)
        Me.chkDPOFeaturedOnGroupFlight.Name = "chkDPOFeaturedOnGroupFlight"
        Me.chkDPOFeaturedOnGroupFlight.Size = New System.Drawing.Size(285, 26)
        Me.chkDPOFeaturedOnGroupFlight.TabIndex = 0
        Me.chkDPOFeaturedOnGroupFlight.Tag = "41"
        Me.chkDPOFeaturedOnGroupFlight.Text = "Task featured on group flight event"
        Me.ToolTip1.SetToolTip(Me.chkDPOFeaturedOnGroupFlight, "Select if you want to include the task featured on group flight notice (only when" &
        " link is available).")
        Me.chkDPOFeaturedOnGroupFlight.UseVisualStyleBackColor = True
        '
        'btnDPOResetToDefault
        '
        Me.btnDPOResetToDefault.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.btnDPOResetToDefault.Location = New System.Drawing.Point(264, 510)
        Me.btnDPOResetToDefault.Name = "btnDPOResetToDefault"
        Me.btnDPOResetToDefault.Size = New System.Drawing.Size(124, 35)
        Me.btnDPOResetToDefault.TabIndex = 4
        Me.btnDPOResetToDefault.Tag = "42"
        Me.btnDPOResetToDefault.Text = "Reset all"
        Me.ToolTip1.SetToolTip(Me.btnDPOResetToDefault, "Click to reset all options to the default values.")
        Me.btnDPOResetToDefault.UseVisualStyleBackColor = True
        '
        'btnStartTaskPost
        '
        Me.btnStartTaskPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStartTaskPost.Location = New System.Drawing.Point(4, 551)
        Me.btnStartTaskPost.Name = "btnStartTaskPost"
        Me.btnStartTaskPost.Size = New System.Drawing.Size(384, 37)
        Me.btnStartTaskPost.TabIndex = 5
        Me.btnStartTaskPost.Tag = "43"
        Me.btnStartTaskPost.Text = "Start Task Post Workflow"
        Me.ToolTip1.SetToolTip(Me.btnStartTaskPost, "Click this button to begin posting the task's details.")
        Me.btnStartTaskPost.UseVisualStyleBackColor = True
        '
        'lblTaskLibraryIDNotAcquired
        '
        Me.lblTaskLibraryIDNotAcquired.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTaskLibraryIDNotAcquired.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblTaskLibraryIDNotAcquired.Location = New System.Drawing.Point(6, 448)
        Me.lblTaskLibraryIDNotAcquired.Name = "lblTaskLibraryIDNotAcquired"
        Me.lblTaskLibraryIDNotAcquired.Size = New System.Drawing.Size(381, 33)
        Me.lblTaskLibraryIDNotAcquired.TabIndex = 0
        Me.lblTaskLibraryIDNotAcquired.Text = "⚠️ = TASK LIBRARY ID NOT ACQUIRED"
        Me.lblTaskLibraryIDNotAcquired.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTaskLibraryIDAcquired
        '
        Me.lblTaskLibraryIDAcquired.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTaskLibraryIDAcquired.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblTaskLibraryIDAcquired.Location = New System.Drawing.Point(6, 448)
        Me.lblTaskLibraryIDAcquired.Name = "lblTaskLibraryIDAcquired"
        Me.lblTaskLibraryIDAcquired.Size = New System.Drawing.Size(381, 33)
        Me.lblTaskLibraryIDAcquired.TabIndex = 17
        Me.lblTaskLibraryIDAcquired.Text = "TASK LIBRARY ID ACQUIRED"
        Me.lblTaskLibraryIDAcquired.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblTaskLibraryIDAcquired.Visible = False
        '
        'lblThread1stMsgIDNotAcquired
        '
        Me.lblThread1stMsgIDNotAcquired.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblThread1stMsgIDNotAcquired.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblThread1stMsgIDNotAcquired.Location = New System.Drawing.Point(6, 474)
        Me.lblThread1stMsgIDNotAcquired.Name = "lblThread1stMsgIDNotAcquired"
        Me.lblThread1stMsgIDNotAcquired.Size = New System.Drawing.Size(381, 33)
        Me.lblThread1stMsgIDNotAcquired.TabIndex = 1
        Me.lblThread1stMsgIDNotAcquired.Text = "⚠️ = Thread First Msg ID NOT Acquired"
        Me.lblThread1stMsgIDNotAcquired.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblThread1stMsgIDAcquired
        '
        Me.lblThread1stMsgIDAcquired.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblThread1stMsgIDAcquired.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblThread1stMsgIDAcquired.Location = New System.Drawing.Point(6, 474)
        Me.lblThread1stMsgIDAcquired.Name = "lblThread1stMsgIDAcquired"
        Me.lblThread1stMsgIDAcquired.Size = New System.Drawing.Size(381, 33)
        Me.lblThread1stMsgIDAcquired.TabIndex = 21
        Me.lblThread1stMsgIDAcquired.Text = "Thread First Msg ID Acquired"
        Me.lblThread1stMsgIDAcquired.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblThread1stMsgIDAcquired.Visible = False
        '
        'txtDiscordEventDescription
        '
        Me.txtDiscordEventDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDiscordEventDescription.Location = New System.Drawing.Point(1396, 814)
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
        Me.txtDiscordEventTopic.Location = New System.Drawing.Point(1331, 810)
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
        Me.lblNbrCarsWeatherClouds.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblNbrCarsWeatherClouds.Location = New System.Drawing.Point(634, 823)
        Me.lblNbrCarsWeatherClouds.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsWeatherClouds.Name = "lblNbrCarsWeatherClouds"
        Me.lblNbrCarsWeatherClouds.Size = New System.Drawing.Size(17, 20)
        Me.lblNbrCarsWeatherClouds.TabIndex = 14
        Me.lblNbrCarsWeatherClouds.Text = "0"
        Me.lblNbrCarsWeatherClouds.Visible = False
        '
        'txtFullDescriptionResults
        '
        Me.txtFullDescriptionResults.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFullDescriptionResults.Location = New System.Drawing.Point(1071, 819)
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
        Me.txtWeatherFirstPart.Location = New System.Drawing.Point(811, 820)
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
        Me.txtFilesText.Location = New System.Drawing.Point(1006, 819)
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
        Me.txtWeatherWinds.Location = New System.Drawing.Point(876, 819)
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
        Me.txtFPResults.Location = New System.Drawing.Point(681, 819)
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
        Me.txtWeatherClouds.Location = New System.Drawing.Point(941, 819)
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
        Me.lblNbrCarsWeatherInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblNbrCarsWeatherInfo.Location = New System.Drawing.Point(588, 823)
        Me.lblNbrCarsWeatherInfo.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsWeatherInfo.Name = "lblNbrCarsWeatherInfo"
        Me.lblNbrCarsWeatherInfo.Size = New System.Drawing.Size(17, 20)
        Me.lblNbrCarsWeatherInfo.TabIndex = 11
        Me.lblNbrCarsWeatherInfo.Text = "0"
        Me.lblNbrCarsWeatherInfo.Visible = False
        '
        'txtAltRestrictions
        '
        Me.txtAltRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAltRestrictions.Location = New System.Drawing.Point(746, 819)
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
        Me.lblNbrCarsWeatherWinds.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblNbrCarsWeatherWinds.Location = New System.Drawing.Point(611, 823)
        Me.lblNbrCarsWeatherWinds.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsWeatherWinds.Name = "lblNbrCarsWeatherWinds"
        Me.lblNbrCarsWeatherWinds.Size = New System.Drawing.Size(17, 20)
        Me.lblNbrCarsWeatherWinds.TabIndex = 12
        Me.lblNbrCarsWeatherWinds.Text = "0"
        Me.lblNbrCarsWeatherWinds.Visible = False
        '
        'lblNbrCarsFilesText
        '
        Me.lblNbrCarsFilesText.AutoSize = True
        Me.lblNbrCarsFilesText.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblNbrCarsFilesText.Location = New System.Drawing.Point(655, 823)
        Me.lblNbrCarsFilesText.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.lblNbrCarsFilesText.Name = "lblNbrCarsFilesText"
        Me.lblNbrCarsFilesText.Size = New System.Drawing.Size(17, 20)
        Me.lblNbrCarsFilesText.TabIndex = 76
        Me.lblNbrCarsFilesText.Text = "0"
        Me.lblNbrCarsFilesText.Visible = False
        '
        'grpDiscordGroupFlight
        '
        Me.grpDiscordGroupFlight.Controls.Add(Me.grpGroupFlightEvent)
        Me.grpDiscordGroupFlight.Location = New System.Drawing.Point(424, 3)
        Me.grpDiscordGroupFlight.Name = "grpDiscordGroupFlight"
        Me.grpDiscordGroupFlight.Size = New System.Drawing.Size(405, 719)
        Me.grpDiscordGroupFlight.TabIndex = 1
        Me.grpDiscordGroupFlight.TabStop = False
        Me.grpDiscordGroupFlight.Text = "Group Flight Event"
        '
        'grpGroupFlightEvent
        '
        Me.grpGroupFlightEvent.Controls.Add(Me.btnDGPORecallSettings)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnDGPORememberSettings)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnStartGroupEventPost)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnDGPOResetToDefault)
        Me.grpGroupFlightEvent.Controls.Add(Me.flpDiscordGroupPostOptions)
        Me.grpGroupFlightEvent.Controls.Add(Me.btnDiscordGroupEventURL)
        Me.grpGroupFlightEvent.Controls.Add(Me.txtGroupEventPostURL)
        Me.grpGroupFlightEvent.Controls.Add(Me.Label38)
        Me.grpGroupFlightEvent.Location = New System.Drawing.Point(6, 27)
        Me.grpGroupFlightEvent.Name = "grpGroupFlightEvent"
        Me.grpGroupFlightEvent.Size = New System.Drawing.Size(393, 684)
        Me.grpGroupFlightEvent.TabIndex = 0
        Me.grpGroupFlightEvent.TabStop = False
        Me.grpGroupFlightEvent.Text = "Discord Post Options"
        '
        'btnDGPORecallSettings
        '
        Me.btnDGPORecallSettings.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.btnDGPORecallSettings.Location = New System.Drawing.Point(4, 599)
        Me.btnDGPORecallSettings.Name = "btnDGPORecallSettings"
        Me.btnDGPORecallSettings.Size = New System.Drawing.Size(124, 35)
        Me.btnDGPORecallSettings.TabIndex = 4
        Me.btnDGPORecallSettings.Tag = "82"
        Me.btnDGPORecallSettings.Text = "Recall"
        Me.ToolTip1.SetToolTip(Me.btnDGPORecallSettings, "Click to recall the remembered set of options.")
        Me.btnDGPORecallSettings.UseVisualStyleBackColor = True
        '
        'btnDGPORememberSettings
        '
        Me.btnDGPORememberSettings.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.btnDGPORememberSettings.Location = New System.Drawing.Point(134, 599)
        Me.btnDGPORememberSettings.Name = "btnDGPORememberSettings"
        Me.btnDGPORememberSettings.Size = New System.Drawing.Size(124, 35)
        Me.btnDGPORememberSettings.TabIndex = 5
        Me.btnDGPORememberSettings.Tag = "82"
        Me.btnDGPORememberSettings.Text = "Remember"
        Me.ToolTip1.SetToolTip(Me.btnDGPORememberSettings, "Click to remember (save) this set of options for future posts.")
        Me.btnDGPORememberSettings.UseVisualStyleBackColor = True
        '
        'btnStartGroupEventPost
        '
        Me.btnStartGroupEventPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStartGroupEventPost.Location = New System.Drawing.Point(4, 640)
        Me.btnStartGroupEventPost.Name = "btnStartGroupEventPost"
        Me.btnStartGroupEventPost.Size = New System.Drawing.Size(384, 37)
        Me.btnStartGroupEventPost.TabIndex = 7
        Me.btnStartGroupEventPost.Tag = "83"
        Me.btnStartGroupEventPost.Text = "Start Group Flight Event Post Workflow"
        Me.ToolTip1.SetToolTip(Me.btnStartGroupEventPost, "Click this button to begin posting the group flight event's details.")
        Me.btnStartGroupEventPost.UseVisualStyleBackColor = True
        '
        'btnDGPOResetToDefault
        '
        Me.btnDGPOResetToDefault.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.btnDGPOResetToDefault.Location = New System.Drawing.Point(264, 597)
        Me.btnDGPOResetToDefault.Name = "btnDGPOResetToDefault"
        Me.btnDGPOResetToDefault.Size = New System.Drawing.Size(124, 35)
        Me.btnDGPOResetToDefault.TabIndex = 6
        Me.btnDGPOResetToDefault.Tag = "82"
        Me.btnDGPOResetToDefault.Text = "Reset all"
        Me.ToolTip1.SetToolTip(Me.btnDGPOResetToDefault, "Click to reset all options to the default values.")
        Me.btnDGPOResetToDefault.UseVisualStyleBackColor = True
        '
        'flpDiscordGroupPostOptions
        '
        Me.flpDiscordGroupPostOptions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel16)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel17)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel18)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel19)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.Label11)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel23)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel22)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.Label49)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel24)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel25)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.Label23)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel20)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel21)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.Label18)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel26)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel27)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel28)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel29)
        Me.flpDiscordGroupPostOptions.Controls.Add(Me.FlowLayoutPanel30)
        Me.flpDiscordGroupPostOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpDiscordGroupPostOptions.Location = New System.Drawing.Point(6, 25)
        Me.flpDiscordGroupPostOptions.Name = "flpDiscordGroupPostOptions"
        Me.flpDiscordGroupPostOptions.Size = New System.Drawing.Size(379, 504)
        Me.flpDiscordGroupPostOptions.TabIndex = 0
        '
        'FlowLayoutPanel16
        '
        Me.FlowLayoutPanel16.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel16.Controls.Add(Me.chkDGPOCoverImage)
        Me.FlowLayoutPanel16.Location = New System.Drawing.Point(3, 0)
        Me.FlowLayoutPanel16.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel16.Name = "FlowLayoutPanel16"
        Me.FlowLayoutPanel16.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel16.TabIndex = 0
        '
        'chkDGPOCoverImage
        '
        Me.chkDGPOCoverImage.AutoSize = True
        Me.chkDGPOCoverImage.Checked = True
        Me.chkDGPOCoverImage.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPOCoverImage.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOCoverImage.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOCoverImage.Name = "chkDGPOCoverImage"
        Me.chkDGPOCoverImage.Size = New System.Drawing.Size(123, 26)
        Me.chkDGPOCoverImage.TabIndex = 0
        Me.chkDGPOCoverImage.Tag = "80"
        Me.chkDGPOCoverImage.Text = "Cover image"
        Me.ToolTip1.SetToolTip(Me.chkDGPOCoverImage, "Select if you want to include the cover image.")
        Me.chkDGPOCoverImage.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel17
        '
        Me.FlowLayoutPanel17.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel17.Controls.Add(Me.chkDGPOMainGroupPost)
        Me.FlowLayoutPanel17.Location = New System.Drawing.Point(3, 26)
        Me.FlowLayoutPanel17.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel17.Name = "FlowLayoutPanel17"
        Me.FlowLayoutPanel17.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel17.TabIndex = 1
        '
        'chkDGPOMainGroupPost
        '
        Me.chkDGPOMainGroupPost.AutoSize = True
        Me.chkDGPOMainGroupPost.Checked = True
        Me.chkDGPOMainGroupPost.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPOMainGroupPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOMainGroupPost.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOMainGroupPost.Name = "chkDGPOMainGroupPost"
        Me.chkDGPOMainGroupPost.Size = New System.Drawing.Size(197, 26)
        Me.chkDGPOMainGroupPost.TabIndex = 0
        Me.chkDGPOMainGroupPost.Tag = "80"
        Me.chkDGPOMainGroupPost.Text = "Main group event post"
        Me.ToolTip1.SetToolTip(Me.chkDGPOMainGroupPost, "Select if you want to include the group event's main information.")
        Me.chkDGPOMainGroupPost.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel18
        '
        Me.FlowLayoutPanel18.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel18.Controls.Add(Me.chkDGPOThreadCreation)
        Me.FlowLayoutPanel18.Location = New System.Drawing.Point(3, 52)
        Me.FlowLayoutPanel18.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel18.Name = "FlowLayoutPanel18"
        Me.FlowLayoutPanel18.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel18.TabIndex = 2
        '
        'chkDGPOThreadCreation
        '
        Me.chkDGPOThreadCreation.AutoSize = True
        Me.chkDGPOThreadCreation.Checked = True
        Me.chkDGPOThreadCreation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPOThreadCreation.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOThreadCreation.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOThreadCreation.Name = "chkDGPOThreadCreation"
        Me.chkDGPOThreadCreation.Size = New System.Drawing.Size(145, 26)
        Me.chkDGPOThreadCreation.TabIndex = 0
        Me.chkDGPOThreadCreation.Tag = "80"
        Me.chkDGPOThreadCreation.Text = "Thread creation"
        Me.ToolTip1.SetToolTip(Me.chkDGPOThreadCreation, "Select if you want to include the creation of the group event's thread.")
        Me.chkDGPOThreadCreation.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel19
        '
        Me.FlowLayoutPanel19.Controls.Add(Me.chkDGPOTeaser)
        Me.FlowLayoutPanel19.Location = New System.Drawing.Point(3, 78)
        Me.FlowLayoutPanel19.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel19.Name = "FlowLayoutPanel19"
        Me.FlowLayoutPanel19.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel19.TabIndex = 3
        '
        'chkDGPOTeaser
        '
        Me.chkDGPOTeaser.AutoSize = True
        Me.chkDGPOTeaser.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOTeaser.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOTeaser.Name = "chkDGPOTeaser"
        Me.chkDGPOTeaser.Size = New System.Drawing.Size(76, 26)
        Me.chkDGPOTeaser.TabIndex = 0
        Me.chkDGPOTeaser.Tag = "80"
        Me.chkDGPOTeaser.Text = "Teaser"
        Me.ToolTip1.SetToolTip(Me.chkDGPOTeaser, "Select if you want to only post a teaser in the group event's thread.")
        Me.chkDGPOTeaser.UseVisualStyleBackColor = True
        '
        'Label11
        '
        Me.Label11.Enabled = False
        Me.Label11.Location = New System.Drawing.Point(3, 104)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(227, 20)
        Me.Label11.TabIndex = 4
        Me.Label11.Text = " "
        '
        'FlowLayoutPanel23
        '
        Me.FlowLayoutPanel23.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel23.Controls.Add(Me.chkDGPOEventLogistics)
        Me.FlowLayoutPanel23.Location = New System.Drawing.Point(3, 124)
        Me.FlowLayoutPanel23.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel23.Name = "FlowLayoutPanel23"
        Me.FlowLayoutPanel23.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel23.TabIndex = 5
        '
        'chkDGPOEventLogistics
        '
        Me.chkDGPOEventLogistics.AutoSize = True
        Me.chkDGPOEventLogistics.Checked = True
        Me.chkDGPOEventLogistics.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPOEventLogistics.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOEventLogistics.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOEventLogistics.Name = "chkDGPOEventLogistics"
        Me.chkDGPOEventLogistics.Size = New System.Drawing.Size(136, 26)
        Me.chkDGPOEventLogistics.TabIndex = 0
        Me.chkDGPOEventLogistics.Tag = "80"
        Me.chkDGPOEventLogistics.Text = "Event logistics"
        Me.ToolTip1.SetToolTip(Me.chkDGPOEventLogistics, "Select if you want to include the message about event logistics in the group even" &
        "t thread.")
        Me.chkDGPOEventLogistics.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel22
        '
        Me.FlowLayoutPanel22.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel22.Controls.Add(Me.chkDGPORelevantTaskDetails)
        Me.FlowLayoutPanel22.Location = New System.Drawing.Point(3, 150)
        Me.FlowLayoutPanel22.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel22.Name = "FlowLayoutPanel22"
        Me.FlowLayoutPanel22.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel22.TabIndex = 6
        '
        'chkDGPORelevantTaskDetails
        '
        Me.chkDGPORelevantTaskDetails.AutoSize = True
        Me.chkDGPORelevantTaskDetails.Checked = True
        Me.chkDGPORelevantTaskDetails.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPORelevantTaskDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPORelevantTaskDetails.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPORelevantTaskDetails.Name = "chkDGPORelevantTaskDetails"
        Me.chkDGPORelevantTaskDetails.Size = New System.Drawing.Size(179, 26)
        Me.chkDGPORelevantTaskDetails.TabIndex = 0
        Me.chkDGPORelevantTaskDetails.Tag = "80"
        Me.chkDGPORelevantTaskDetails.Text = "Relevant task details"
        Me.ToolTip1.SetToolTip(Me.chkDGPORelevantTaskDetails, "Select if you want to include the relevant information about the task in the grou" &
        "p event thread.")
        Me.chkDGPORelevantTaskDetails.UseVisualStyleBackColor = True
        '
        'Label49
        '
        Me.Label49.Enabled = False
        Me.Label49.Location = New System.Drawing.Point(3, 176)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(227, 20)
        Me.Label49.TabIndex = 7
        Me.Label49.Text = " "
        '
        'FlowLayoutPanel24
        '
        Me.FlowLayoutPanel24.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel24.Controls.Add(Me.chkDGPOMainPost)
        Me.FlowLayoutPanel24.Location = New System.Drawing.Point(3, 196)
        Me.FlowLayoutPanel24.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel24.Name = "FlowLayoutPanel24"
        Me.FlowLayoutPanel24.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel24.TabIndex = 8
        '
        'chkDGPOMainPost
        '
        Me.chkDGPOMainPost.AutoSize = True
        Me.chkDGPOMainPost.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOMainPost.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOMainPost.Name = "chkDGPOMainPost"
        Me.chkDGPOMainPost.Size = New System.Drawing.Size(152, 26)
        Me.chkDGPOMainPost.TabIndex = 0
        Me.chkDGPOMainPost.Tag = "80"
        Me.chkDGPOMainPost.Text = "Task main details"
        Me.ToolTip1.SetToolTip(Me.chkDGPOMainPost, "Select if you want to include the task's main information.")
        Me.chkDGPOMainPost.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel25
        '
        Me.FlowLayoutPanel25.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel25.Controls.Add(Me.chkDGPOFullDescription)
        Me.FlowLayoutPanel25.Location = New System.Drawing.Point(3, 222)
        Me.FlowLayoutPanel25.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel25.Name = "FlowLayoutPanel25"
        Me.FlowLayoutPanel25.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel25.TabIndex = 9
        '
        'chkDGPOFullDescription
        '
        Me.chkDGPOFullDescription.AutoSize = True
        Me.chkDGPOFullDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOFullDescription.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOFullDescription.Name = "chkDGPOFullDescription"
        Me.chkDGPOFullDescription.Size = New System.Drawing.Size(143, 26)
        Me.chkDGPOFullDescription.TabIndex = 0
        Me.chkDGPOFullDescription.Tag = "80"
        Me.chkDGPOFullDescription.Text = "Full description"
        Me.ToolTip1.SetToolTip(Me.chkDGPOFullDescription, "Select if you want to include the full description.")
        Me.chkDGPOFullDescription.UseVisualStyleBackColor = True
        '
        'Label23
        '
        Me.Label23.Enabled = False
        Me.Label23.Location = New System.Drawing.Point(3, 248)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(227, 20)
        Me.Label23.TabIndex = 10
        Me.Label23.Text = " "
        '
        'FlowLayoutPanel20
        '
        Me.FlowLayoutPanel20.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel20.Controls.Add(Me.chkDGPOFilesWithFullLegend)
        Me.FlowLayoutPanel20.Controls.Add(Me.chkDGPODPHXOnly)
        Me.FlowLayoutPanel20.Location = New System.Drawing.Point(3, 268)
        Me.FlowLayoutPanel20.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel20.Name = "FlowLayoutPanel20"
        Me.FlowLayoutPanel20.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel20.TabIndex = 11
        '
        'chkDGPOFilesWithFullLegend
        '
        Me.chkDGPOFilesWithFullLegend.AutoSize = True
        Me.chkDGPOFilesWithFullLegend.Checked = True
        Me.chkDGPOFilesWithFullLegend.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPOFilesWithFullLegend.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOFilesWithFullLegend.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOFilesWithFullLegend.Name = "chkDGPOFilesWithFullLegend"
        Me.chkDGPOFilesWithFullLegend.Size = New System.Drawing.Size(148, 26)
        Me.chkDGPOFilesWithFullLegend.TabIndex = 0
        Me.chkDGPOFilesWithFullLegend.Tag = "80"
        Me.chkDGPOFilesWithFullLegend.Text = "Files and legend"
        Me.ToolTip1.SetToolTip(Me.chkDGPOFilesWithFullLegend, "Select if you want to include the file description along with the files.")
        Me.chkDGPOFilesWithFullLegend.UseVisualStyleBackColor = True
        '
        'chkDGPODPHXOnly
        '
        Me.chkDGPODPHXOnly.AutoSize = True
        Me.chkDGPODPHXOnly.Checked = True
        Me.chkDGPODPHXOnly.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkDGPODPHXOnly.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPODPHXOnly.Location = New System.Drawing.Point(157, 3)
        Me.chkDGPODPHXOnly.Name = "chkDGPODPHXOnly"
        Me.chkDGPODPHXOnly.Size = New System.Drawing.Size(111, 26)
        Me.chkDGPODPHXOnly.TabIndex = 1
        Me.chkDGPODPHXOnly.Tag = "80"
        Me.chkDGPODPHXOnly.Text = "DPHX Only"
        Me.ToolTip1.SetToolTip(Me.chkDGPODPHXOnly, "Select if you want to include the file description along with the files.")
        Me.chkDGPODPHXOnly.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel21
        '
        Me.FlowLayoutPanel21.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel21.Controls.Add(Me.chkDGPOFilesWithoutLegend)
        Me.FlowLayoutPanel21.Location = New System.Drawing.Point(3, 294)
        Me.FlowLayoutPanel21.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel21.Name = "FlowLayoutPanel21"
        Me.FlowLayoutPanel21.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel21.TabIndex = 12
        '
        'chkDGPOFilesWithoutLegend
        '
        Me.chkDGPOFilesWithoutLegend.AutoSize = True
        Me.chkDGPOFilesWithoutLegend.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOFilesWithoutLegend.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOFilesWithoutLegend.Name = "chkDGPOFilesWithoutLegend"
        Me.chkDGPOFilesWithoutLegend.Size = New System.Drawing.Size(177, 26)
        Me.chkDGPOFilesWithoutLegend.TabIndex = 0
        Me.chkDGPOFilesWithoutLegend.Tag = "80"
        Me.chkDGPOFilesWithoutLegend.Text = "Files without legend"
        Me.ToolTip1.SetToolTip(Me.chkDGPOFilesWithoutLegend, "Select if you want to post only the files without their description.")
        Me.chkDGPOFilesWithoutLegend.UseVisualStyleBackColor = True
        '
        'Label18
        '
        Me.Label18.Enabled = False
        Me.Label18.Location = New System.Drawing.Point(3, 320)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(227, 20)
        Me.Label18.TabIndex = 13
        Me.Label18.Text = " "
        '
        'FlowLayoutPanel26
        '
        Me.FlowLayoutPanel26.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel26.Controls.Add(Me.chkDGPOAltRestrictions)
        Me.FlowLayoutPanel26.Location = New System.Drawing.Point(3, 340)
        Me.FlowLayoutPanel26.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel26.Name = "FlowLayoutPanel26"
        Me.FlowLayoutPanel26.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel26.TabIndex = 14
        '
        'chkDGPOAltRestrictions
        '
        Me.chkDGPOAltRestrictions.AutoSize = True
        Me.chkDGPOAltRestrictions.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOAltRestrictions.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOAltRestrictions.Name = "chkDGPOAltRestrictions"
        Me.chkDGPOAltRestrictions.Size = New System.Drawing.Size(177, 26)
        Me.chkDGPOAltRestrictions.TabIndex = 0
        Me.chkDGPOAltRestrictions.Tag = "80"
        Me.chkDGPOAltRestrictions.Text = "Altitude restrictions"
        Me.ToolTip1.SetToolTip(Me.chkDGPOAltRestrictions, "Select if you want to include the altitude restrictions.")
        Me.chkDGPOAltRestrictions.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel27
        '
        Me.FlowLayoutPanel27.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel27.Controls.Add(Me.chkDGPOWeatherInfo)
        Me.FlowLayoutPanel27.Location = New System.Drawing.Point(3, 366)
        Me.FlowLayoutPanel27.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel27.Name = "FlowLayoutPanel27"
        Me.FlowLayoutPanel27.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel27.TabIndex = 15
        '
        'chkDGPOWeatherInfo
        '
        Me.chkDGPOWeatherInfo.AutoSize = True
        Me.chkDGPOWeatherInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOWeatherInfo.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOWeatherInfo.Name = "chkDGPOWeatherInfo"
        Me.chkDGPOWeatherInfo.Size = New System.Drawing.Size(143, 26)
        Me.chkDGPOWeatherInfo.TabIndex = 0
        Me.chkDGPOWeatherInfo.Tag = "80"
        Me.chkDGPOWeatherInfo.Text = "Weather details"
        Me.ToolTip1.SetToolTip(Me.chkDGPOWeatherInfo, "Select if you want to include the weather details.")
        Me.chkDGPOWeatherInfo.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel28
        '
        Me.FlowLayoutPanel28.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel28.Controls.Add(Me.chkDGPOWeatherChart)
        Me.FlowLayoutPanel28.Location = New System.Drawing.Point(3, 392)
        Me.FlowLayoutPanel28.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel28.Name = "FlowLayoutPanel28"
        Me.FlowLayoutPanel28.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel28.TabIndex = 16
        '
        'chkDGPOWeatherChart
        '
        Me.chkDGPOWeatherChart.AutoSize = True
        Me.chkDGPOWeatherChart.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOWeatherChart.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOWeatherChart.Name = "chkDGPOWeatherChart"
        Me.chkDGPOWeatherChart.Size = New System.Drawing.Size(132, 26)
        Me.chkDGPOWeatherChart.TabIndex = 0
        Me.chkDGPOWeatherChart.Tag = "80"
        Me.chkDGPOWeatherChart.Text = "Weather chart"
        Me.ToolTip1.SetToolTip(Me.chkDGPOWeatherChart, "Select if you want to include the weather chart.")
        Me.chkDGPOWeatherChart.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel29
        '
        Me.FlowLayoutPanel29.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel29.Controls.Add(Me.chkDGPOWaypoints)
        Me.FlowLayoutPanel29.Location = New System.Drawing.Point(3, 418)
        Me.FlowLayoutPanel29.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel29.Name = "FlowLayoutPanel29"
        Me.FlowLayoutPanel29.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel29.TabIndex = 17
        '
        'chkDGPOWaypoints
        '
        Me.chkDGPOWaypoints.AutoSize = True
        Me.chkDGPOWaypoints.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOWaypoints.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOWaypoints.Name = "chkDGPOWaypoints"
        Me.chkDGPOWaypoints.Size = New System.Drawing.Size(107, 26)
        Me.chkDGPOWaypoints.TabIndex = 0
        Me.chkDGPOWaypoints.Tag = "80"
        Me.chkDGPOWaypoints.Text = "Waypoints"
        Me.ToolTip1.SetToolTip(Me.chkDGPOWaypoints, "Select if you want to include the waypoints details.")
        Me.chkDGPOWaypoints.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel30
        '
        Me.FlowLayoutPanel30.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel30.Controls.Add(Me.chkDGPOAddOns)
        Me.FlowLayoutPanel30.Location = New System.Drawing.Point(3, 444)
        Me.FlowLayoutPanel30.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.FlowLayoutPanel30.Name = "FlowLayoutPanel30"
        Me.FlowLayoutPanel30.Size = New System.Drawing.Size(376, 26)
        Me.FlowLayoutPanel30.TabIndex = 18
        '
        'chkDGPOAddOns
        '
        Me.chkDGPOAddOns.AutoSize = True
        Me.chkDGPOAddOns.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDGPOAddOns.Location = New System.Drawing.Point(3, 3)
        Me.chkDGPOAddOns.Name = "chkDGPOAddOns"
        Me.chkDGPOAddOns.Size = New System.Drawing.Size(208, 26)
        Me.chkDGPOAddOns.TabIndex = 0
        Me.chkDGPOAddOns.Tag = "80"
        Me.chkDGPOAddOns.Text = "Recommended add-ons"
        Me.ToolTip1.SetToolTip(Me.chkDGPOAddOns, "Select if you want to include the recommended add-ons.")
        Me.chkDGPOAddOns.UseVisualStyleBackColor = True
        '
        'btnDiscordGroupEventURL
        '
        Me.btnDiscordGroupEventURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDiscordGroupEventURL.Location = New System.Drawing.Point(314, 562)
        Me.btnDiscordGroupEventURL.Name = "btnDiscordGroupEventURL"
        Me.btnDiscordGroupEventURL.Size = New System.Drawing.Size(74, 29)
        Me.btnDiscordGroupEventURL.TabIndex = 3
        Me.btnDiscordGroupEventURL.Tag = "81"
        Me.btnDiscordGroupEventURL.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnDiscordGroupEventURL, "Click this button to paste the group event's post URL from your clipboard")
        Me.btnDiscordGroupEventURL.UseVisualStyleBackColor = True
        '
        'txtGroupEventPostURL
        '
        Me.txtGroupEventPostURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.txtGroupEventPostURL.Location = New System.Drawing.Point(4, 561)
        Me.txtGroupEventPostURL.Name = "txtGroupEventPostURL"
        Me.txtGroupEventPostURL.Size = New System.Drawing.Size(304, 30)
        Me.txtGroupEventPostURL.TabIndex = 2
        Me.txtGroupEventPostURL.Tag = "81"
        Me.ToolTip1.SetToolTip(Me.txtGroupEventPostURL, "Enter the URL to the Discord post created above in step 1.")
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label38.Location = New System.Drawing.Point(0, 534)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(158, 22)
        Me.Label38.TabIndex = 1
        Me.Label38.Text = "URL to group event:"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label15)
        Me.GroupBox4.Controls.Add(Me.chkDPOExpertMode)
        Me.GroupBox4.Controls.Add(Me.numWaitSecondsForFiles)
        Me.GroupBox4.Location = New System.Drawing.Point(841, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(393, 92)
        Me.GroupBox4.TabIndex = 2
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "General options"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.Label15.Location = New System.Drawing.Point(6, 27)
        Me.Label15.Margin = New System.Windows.Forms.Padding(3, 5, 3, 3)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(191, 22)
        Me.Label15.TabIndex = 0
        Me.Label15.Text = "Seconds to wait for files:"
        '
        'chkDPOExpertMode
        '
        Me.chkDPOExpertMode.AutoSize = True
        Me.chkDPOExpertMode.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.chkDPOExpertMode.Location = New System.Drawing.Point(6, 55)
        Me.chkDPOExpertMode.Name = "chkDPOExpertMode"
        Me.chkDPOExpertMode.Size = New System.Drawing.Size(310, 26)
        Me.chkDPOExpertMode.TabIndex = 2
        Me.chkDPOExpertMode.Tag = "85"
        Me.chkDPOExpertMode.Text = "Automatic progression (experimental)"
        Me.ToolTip1.SetToolTip(Me.chkDPOExpertMode, "Select if you want the workflow to progress automatically whenever possible.")
        Me.chkDPOExpertMode.UseVisualStyleBackColor = True
        '
        'numWaitSecondsForFiles
        '
        Me.numWaitSecondsForFiles.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.numWaitSecondsForFiles.Location = New System.Drawing.Point(203, 25)
        Me.numWaitSecondsForFiles.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numWaitSecondsForFiles.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numWaitSecondsForFiles.Name = "numWaitSecondsForFiles"
        Me.numWaitSecondsForFiles.Size = New System.Drawing.Size(55, 30)
        Me.numWaitSecondsForFiles.TabIndex = 1
        Me.numWaitSecondsForFiles.Tag = "84"
        Me.numWaitSecondsForFiles.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.numWaitSecondsForFiles.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'lblTaskBrowserIDAndDate
        '
        Me.lblTaskBrowserIDAndDate.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTaskBrowserIDAndDate.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblTaskBrowserIDAndDate.Location = New System.Drawing.Point(841, 683)
        Me.lblTaskBrowserIDAndDate.Name = "lblTaskBrowserIDAndDate"
        Me.lblTaskBrowserIDAndDate.Size = New System.Drawing.Size(623, 33)
        Me.lblTaskBrowserIDAndDate.TabIndex = 97
        Me.lblTaskBrowserIDAndDate.Text = "This task does not exist in the Task Library Browser"
        Me.lblTaskBrowserIDAndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblTaskBrowserIDAndDate.Visible = False
        '
        'tabBriefing
        '
        Me.tabBriefing.Controls.Add(Me.pnlBriefing)
        Me.tabBriefing.Location = New System.Drawing.Point(4, 29)
        Me.tabBriefing.Name = "tabBriefing"
        Me.tabBriefing.Size = New System.Drawing.Size(1467, 860)
        Me.tabBriefing.TabIndex = 2
        Me.tabBriefing.Text = "Briefing"
        Me.tabBriefing.UseVisualStyleBackColor = True
        '
        'pnlBriefing
        '
        Me.pnlBriefing.Controls.Add(Me.pnlWizardBriefing)
        Me.pnlBriefing.Controls.Add(Me.BriefingControl1)
        Me.pnlBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBriefing.Location = New System.Drawing.Point(0, 0)
        Me.pnlBriefing.Name = "pnlBriefing"
        Me.pnlBriefing.Size = New System.Drawing.Size(1467, 860)
        Me.pnlBriefing.TabIndex = 0
        '
        'pnlWizardBriefing
        '
        Me.pnlWizardBriefing.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlWizardBriefing.BackColor = System.Drawing.Color.Gray
        Me.pnlWizardBriefing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.pnlWizardBriefing.Controls.Add(Me.lblBriefingGuideInstructions)
        Me.pnlWizardBriefing.Controls.Add(Me.btnBriefingGuideNext)
        Me.pnlWizardBriefing.Location = New System.Drawing.Point(968, 0)
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
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'OneMinuteTimer
        '
        Me.OneMinuteTimer.Enabled = True
        Me.OneMinuteTimer.Interval = 1000
        '
        'lblGroupEmoji
        '
        Me.lblGroupEmoji.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblGroupEmoji.AutoSize = True
        Me.lblGroupEmoji.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblGroupEmoji.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGroupEmoji.Location = New System.Drawing.Point(871, 92)
        Me.lblGroupEmoji.Name = "lblGroupEmoji"
        Me.lblGroupEmoji.Size = New System.Drawing.Size(18, 28)
        Me.lblGroupEmoji.TabIndex = 88
        Me.lblGroupEmoji.Text = " "
        Me.lblGroupEmoji.Visible = False
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ClickThrough = True
        Me.ToolStrip1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripOpen, Me.toolStripSave, Me.toolStripResetAll, Me.toolStripReload, Me.ToolStripSeparator1, Me.toolStripDiscordTaskLibrary, Me.ToolStripSeparator4, Me.toolStripB21Planner, Me.ToolStripSeparator2, Me.toolStripSharePackage, Me.ToolStripSeparator3, Me.toolStripGuideMe, Me.toolStripStopGuide, Me.ToolStripDropDownButton1, Me.toolStripCurrentDateTime})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1494, 29)
        Me.ToolStrip1.SuppressHighlighting = False
        Me.ToolStrip1.TabIndex = 7
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'toolStripOpen
        '
        Me.toolStripOpen.Image = CType(resources.GetObject("toolStripOpen.Image"), System.Drawing.Image)
        Me.toolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripOpen.Name = "toolStripOpen"
        Me.toolStripOpen.Size = New System.Drawing.Size(70, 26)
        Me.toolStripOpen.Text = "&Open"
        Me.toolStripOpen.ToolTipText = "Click to select and load a DPH session file from your PC."
        '
        'toolStripSave
        '
        Me.toolStripSave.Image = CType(resources.GetObject("toolStripSave.Image"), System.Drawing.Image)
        Me.toolStripSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripSave.Name = "toolStripSave"
        Me.toolStripSave.Size = New System.Drawing.Size(65, 26)
        Me.toolStripSave.Text = "&Save"
        Me.toolStripSave.ToolTipText = "Click to save the current DPH session to your PC."
        '
        'toolStripResetAll
        '
        Me.toolStripResetAll.Image = CType(resources.GetObject("toolStripResetAll.Image"), System.Drawing.Image)
        Me.toolStripResetAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripResetAll.Name = "toolStripResetAll"
        Me.toolStripResetAll.Size = New System.Drawing.Size(92, 26)
        Me.toolStripResetAll.Text = "&Reset All"
        Me.toolStripResetAll.ToolTipText = "Click to reset ALL of the fiels and start from scratch."
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
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 29)
        '
        'toolStripDiscordTaskLibrary
        '
        Me.toolStripDiscordTaskLibrary.Image = CType(resources.GetObject("toolStripDiscordTaskLibrary.Image"), System.Drawing.Image)
        Me.toolStripDiscordTaskLibrary.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripDiscordTaskLibrary.Name = "toolStripDiscordTaskLibrary"
        Me.toolStripDiscordTaskLibrary.Size = New System.Drawing.Size(114, 26)
        Me.toolStripDiscordTaskLibrary.Text = "Task &Library"
        Me.toolStripDiscordTaskLibrary.ToolTipText = "Click here to open the Task Library on Discord."
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 29)
        '
        'toolStripB21Planner
        '
        Me.toolStripB21Planner.Image = CType(resources.GetObject("toolStripB21Planner.Image"), System.Drawing.Image)
        Me.toolStripB21Planner.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripB21Planner.Name = "toolStripB21Planner"
        Me.toolStripB21Planner.Size = New System.Drawing.Size(116, 26)
        Me.toolStripB21Planner.Text = "&B21 Planner"
        Me.toolStripB21Planner.ToolTipText = "Click to open the B21 Planner in your browser."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 29)
        '
        'toolStripSharePackage
        '
        Me.toolStripSharePackage.Image = CType(resources.GetObject("toolStripSharePackage.Image"), System.Drawing.Image)
        Me.toolStripSharePackage.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripSharePackage.Name = "toolStripSharePackage"
        Me.toolStripSharePackage.Size = New System.Drawing.Size(132, 26)
        Me.toolStripSharePackage.Text = "Share &Package"
        Me.toolStripSharePackage.ToolTipText = "Click to create a shareable package with all files."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 29)
        '
        'toolStripGuideMe
        '
        Me.toolStripGuideMe.Image = CType(resources.GetObject("toolStripGuideMe.Image"), System.Drawing.Image)
        Me.toolStripGuideMe.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripGuideMe.Name = "toolStripGuideMe"
        Me.toolStripGuideMe.Size = New System.Drawing.Size(183, 26)
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
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(147, 26)
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
        Me.toolStripCurrentDateTime.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetNowFullWithDayOfWeek, Me.GetNowLongDateTime, Me.GetNowTimeOnlyWithoutSeconds, Me.GetNowCountdown, Me.GetNowTimeStampOnly})
        Me.toolStripCurrentDateTime.Image = CType(resources.GetObject("toolStripCurrentDateTime.Image"), System.Drawing.Image)
        Me.toolStripCurrentDateTime.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripCurrentDateTime.Name = "toolStripCurrentDateTime"
        Me.toolStripCurrentDateTime.Size = New System.Drawing.Size(143, 26)
        Me.toolStripCurrentDateTime.Text = "CurrentDateTime"
        Me.toolStripCurrentDateTime.ToolTipText = "Click for UNIX timestamp options"
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
        'GetNowTimeOnlyWithoutSeconds
        '
        Me.GetNowTimeOnlyWithoutSeconds.Name = "GetNowTimeOnlyWithoutSeconds"
        Me.GetNowTimeOnlyWithoutSeconds.Size = New System.Drawing.Size(269, 26)
        Me.GetNowTimeOnlyWithoutSeconds.Text = "TimeOnlyWithoutSeconds"
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
        'FileDropZone1
        '
        Me.FileDropZone1.AllowDrop = True
        Me.FileDropZone1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileDropZone1.Location = New System.Drawing.Point(3, 690)
        Me.FileDropZone1.MinimumSize = New System.Drawing.Size(700, 161)
        Me.FileDropZone1.Name = "FileDropZone1"
        Me.FileDropZone1.Size = New System.Drawing.Size(709, 161)
        Me.FileDropZone1.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.FileDropZone1, "Drag files here to automatically process them depending on their type")
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
        Me.BriefingControl1.Size = New System.Drawing.Size(1467, 860)
        Me.BriefingControl1.TabIndex = 0
        Me.BriefingControl1.Tag = "100"
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
        Me.MinimumSize = New System.Drawing.Size(1512, 700)
        Me.Name = "Main"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Discord Post Helper"
        Me.pnlScrollableSurface.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.tabFlightPlan.ResumeLayout(False)
        Me.pnlGuide.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.pnlFlightPlanRightSide.ResumeLayout(False)
        Me.grbTaskPart2.ResumeLayout(False)
        Me.grbTaskPart2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.grbTaskDiscord.ResumeLayout(False)
        Me.grbTaskDiscord.PerformLayout()
        Me.pnlFlightPlanLeftSide.ResumeLayout(False)
        Me.pnlFlightPlanLeftSide.PerformLayout()
        Me.grbTaskInfo.ResumeLayout(False)
        Me.grbTaskInfo.PerformLayout()
        Me.tabEvent.ResumeLayout(False)
        Me.tabEvent.PerformLayout()
        Me.pnlWizardEvent.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.grpGroupEventPost.ResumeLayout(False)
        Me.grpGroupEventPost.PerformLayout()
        Me.grpEventTeaser.ResumeLayout(False)
        Me.grpEventTeaser.PerformLayout()
        Me.pnlEventDateTimeControls.ResumeLayout(False)
        Me.pnlEventDateTimeControls.PerformLayout()
        Me.TimeStampContextualMenu.ResumeLayout(False)
        Me.tabDiscord.ResumeLayout(False)
        Me.tabDiscord.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.pnlWizardDiscord.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.pnlFullWorkflowTaskGroupFlight.ResumeLayout(False)
        Me.grpDiscordEvent.ResumeLayout(False)
        Me.grpDiscordEvent.PerformLayout()
        Me.grpDiscordOthers.ResumeLayout(False)
        Me.grpDiscordTask.ResumeLayout(False)
        Me.grpRepost.ResumeLayout(False)
        Me.grpRepost.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.flpDiscordPostOptions.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.FlowLayoutPanel2.PerformLayout()
        Me.FlowLayoutPanel4.ResumeLayout(False)
        Me.FlowLayoutPanel4.PerformLayout()
        Me.FlowLayoutPanel3.ResumeLayout(False)
        Me.FlowLayoutPanel3.PerformLayout()
        Me.FlowLayoutPanel5.ResumeLayout(False)
        Me.FlowLayoutPanel5.PerformLayout()
        Me.FlowLayoutPanel6.ResumeLayout(False)
        Me.FlowLayoutPanel6.PerformLayout()
        Me.FlowLayoutPanel7.ResumeLayout(False)
        Me.FlowLayoutPanel7.PerformLayout()
        Me.FlowLayoutPanel8.ResumeLayout(False)
        Me.FlowLayoutPanel8.PerformLayout()
        Me.FlowLayoutPanel9.ResumeLayout(False)
        Me.FlowLayoutPanel9.PerformLayout()
        Me.FlowLayoutPanel10.ResumeLayout(False)
        Me.FlowLayoutPanel10.PerformLayout()
        Me.FlowLayoutPanel11.ResumeLayout(False)
        Me.FlowLayoutPanel11.PerformLayout()
        Me.FlowLayoutPanel12.ResumeLayout(False)
        Me.FlowLayoutPanel12.PerformLayout()
        Me.FlowLayoutPanel13.ResumeLayout(False)
        Me.FlowLayoutPanel13.PerformLayout()
        Me.grpDiscordGroupFlight.ResumeLayout(False)
        Me.grpGroupFlightEvent.ResumeLayout(False)
        Me.grpGroupFlightEvent.PerformLayout()
        Me.flpDiscordGroupPostOptions.ResumeLayout(False)
        Me.FlowLayoutPanel16.ResumeLayout(False)
        Me.FlowLayoutPanel16.PerformLayout()
        Me.FlowLayoutPanel17.ResumeLayout(False)
        Me.FlowLayoutPanel17.PerformLayout()
        Me.FlowLayoutPanel18.ResumeLayout(False)
        Me.FlowLayoutPanel18.PerformLayout()
        Me.FlowLayoutPanel19.ResumeLayout(False)
        Me.FlowLayoutPanel19.PerformLayout()
        Me.FlowLayoutPanel23.ResumeLayout(False)
        Me.FlowLayoutPanel23.PerformLayout()
        Me.FlowLayoutPanel22.ResumeLayout(False)
        Me.FlowLayoutPanel22.PerformLayout()
        Me.FlowLayoutPanel24.ResumeLayout(False)
        Me.FlowLayoutPanel24.PerformLayout()
        Me.FlowLayoutPanel25.ResumeLayout(False)
        Me.FlowLayoutPanel25.PerformLayout()
        Me.FlowLayoutPanel20.ResumeLayout(False)
        Me.FlowLayoutPanel20.PerformLayout()
        Me.FlowLayoutPanel21.ResumeLayout(False)
        Me.FlowLayoutPanel21.PerformLayout()
        Me.FlowLayoutPanel26.ResumeLayout(False)
        Me.FlowLayoutPanel26.PerformLayout()
        Me.FlowLayoutPanel27.ResumeLayout(False)
        Me.FlowLayoutPanel27.PerformLayout()
        Me.FlowLayoutPanel28.ResumeLayout(False)
        Me.FlowLayoutPanel28.PerformLayout()
        Me.FlowLayoutPanel29.ResumeLayout(False)
        Me.FlowLayoutPanel29.PerformLayout()
        Me.FlowLayoutPanel30.ResumeLayout(False)
        Me.FlowLayoutPanel30.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.numWaitSecondsForFiles, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabBriefing.ResumeLayout(False)
        Me.pnlBriefing.ResumeLayout(False)
        Me.pnlWizardBriefing.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
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
    Friend WithEvents btnSelectFlightPlan As Button
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
    Friend WithEvents txtGroupFlightEventPost As TextBox
    Friend WithEvents lblEventTaskDistance As Label
    Friend WithEvents Label48 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents txtDiscordEventDescription As TextBox
    Friend WithEvents txtDiscordEventTopic As TextBox
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
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
    Friend WithEvents txtFullDescriptionResults As TextBox
    Friend WithEvents txtFilesText As TextBox
    Friend WithEvents lblNbrCarsMainFP As Label
    Friend WithEvents txtFPResults As TextBox
    Friend WithEvents lblNbrCarsRestrictions As Label
    Friend WithEvents lblNbrCarsWeatherClouds As Label
    Friend WithEvents txtWeatherFirstPart As TextBox
    Friend WithEvents txtWeatherWinds As TextBox
    Friend WithEvents txtWeatherClouds As TextBox
    Friend WithEvents lblNbrCarsWeatherInfo As Label
    Friend WithEvents lblNbrCarsWeatherWinds As Label
    Friend WithEvents txtAltRestrictions As TextBox
    Friend WithEvents lblNbrCarsFilesText As Label
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
    Friend WithEvents txtWeatherSummary As TextBox
    Friend WithEvents lblWeatherSummary As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents grpDiscordTask As GroupBox
    Friend WithEvents flpDiscordPostOptions As FlowLayoutPanel
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
    Friend WithEvents btnTaskFeaturedOnGroupFlight As Button
    Friend WithEvents btnDiscordGroupEventURL As Button
    Friend WithEvents txtGroupEventPostURL As TextBox
    Friend WithEvents Label38 As Label
    Friend WithEvents Label30 As Label
    Friend WithEvents txtOtherBeginnerLink As TextBox
    Friend WithEvents cboBeginnersGuide As ComboBox
    Friend WithEvents btnPasteBeginnerLink As Button
    Friend WithEvents grbTaskDiscord As GroupBox
    Friend WithEvents txtDiscordTaskID As TextBox
    Friend WithEvents Label31 As Label
    Friend WithEvents btnDiscordTaskThreadURLPaste As Button
    Friend WithEvents btnPasteUsernameCredits As Button
    Friend WithEvents FileDropZone1 As CommonLibrary.FileDropZone
    Friend WithEvents chkSoaringTypeWave As CheckBox
    Friend WithEvents chkSuppressWarningForBaroPressure As CheckBox
    Friend WithEvents txtBaroPressureExtraInfo As TextBox
    Friend WithEvents lblNonStdBaroPressure As Label
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
    Friend WithEvents pnlFlightPlanLeftSide As Panel
    Friend WithEvents pnlFlightPlanRightSide As Panel
    Friend WithEvents FlightPlanTabSplitter As Splitter
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents chkDPOMainPost As CheckBox
    Friend WithEvents btnStartTaskPost As Button
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents chkDPOThreadCreation As CheckBox
    Friend WithEvents FlowLayoutPanel3 As FlowLayoutPanel
    Friend WithEvents chkDPOFullDescription As CheckBox
    Friend WithEvents lblNbrCarsFullDescResults As Label
    Friend WithEvents FlowLayoutPanel4 As FlowLayoutPanel
    Friend WithEvents chkDPOIncludeCoverImage As CheckBox
    Friend WithEvents FlowLayoutPanel5 As FlowLayoutPanel
    Friend WithEvents chkDPOFilesWithDescription As CheckBox
    Friend WithEvents FlowLayoutPanel6 As FlowLayoutPanel
    Friend WithEvents chkDPOFilesAlone As CheckBox
    Friend WithEvents btnDPOResetToDefault As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents FlowLayoutPanel7 As FlowLayoutPanel
    Friend WithEvents chkDPOAltRestrictions As CheckBox
    Friend WithEvents FlowLayoutPanel8 As FlowLayoutPanel
    Friend WithEvents chkDPOWeatherInfo As CheckBox
    Friend WithEvents lblNbrCarsWeather As Label
    Friend WithEvents FlowLayoutPanel9 As FlowLayoutPanel
    Friend WithEvents chkDPOWeatherChart As CheckBox
    Friend WithEvents FlowLayoutPanel10 As FlowLayoutPanel
    Friend WithEvents chkDPOWaypoints As CheckBox
    Friend WithEvents lblNbrCarsWaypoints As Label
    Friend WithEvents FlowLayoutPanel11 As FlowLayoutPanel
    Friend WithEvents chkDPOAddOns As CheckBox
    Friend WithEvents lblNbrCarsAddOns As Label
    Friend WithEvents FlowLayoutPanel12 As FlowLayoutPanel
    Friend WithEvents chkDPOResultsInvitation As CheckBox
    Friend WithEvents FlowLayoutPanel13 As FlowLayoutPanel
    Friend WithEvents chkDPOFeaturedOnGroupFlight As CheckBox
    Friend WithEvents chkDPOExpertMode As CheckBox
    Friend WithEvents Label15 As Label
    Friend WithEvents numWaitSecondsForFiles As NumericUpDown
    Friend WithEvents flpDiscordGroupPostOptions As FlowLayoutPanel
    Friend WithEvents FlowLayoutPanel16 As FlowLayoutPanel
    Friend WithEvents chkDGPOCoverImage As CheckBox
    Friend WithEvents btnStartGroupEventPost As Button
    Friend WithEvents btnDGPOResetToDefault As Button
    Friend WithEvents FlowLayoutPanel17 As FlowLayoutPanel
    Friend WithEvents chkDGPOMainGroupPost As CheckBox
    Friend WithEvents FlowLayoutPanel18 As FlowLayoutPanel
    Friend WithEvents chkDGPOThreadCreation As CheckBox
    Friend WithEvents Label11 As Label
    Friend WithEvents FlowLayoutPanel19 As FlowLayoutPanel
    Friend WithEvents chkDGPOTeaser As CheckBox
    Friend WithEvents FlowLayoutPanel20 As FlowLayoutPanel
    Friend WithEvents chkDGPOFilesWithFullLegend As CheckBox
    Friend WithEvents FlowLayoutPanel21 As FlowLayoutPanel
    Friend WithEvents chkDGPOFilesWithoutLegend As CheckBox
    Friend WithEvents FlowLayoutPanel22 As FlowLayoutPanel
    Friend WithEvents chkDGPORelevantTaskDetails As CheckBox
    Friend WithEvents FlowLayoutPanel23 As FlowLayoutPanel
    Friend WithEvents chkDGPOEventLogistics As CheckBox
    Friend WithEvents Label18 As Label
    Friend WithEvents grpDiscordOthers As GroupBox
    Friend WithEvents btnSyncTitles As Button
    Friend WithEvents txtClubFullName As TextBox
    Friend WithEvents chkDGPODPHXOnly As CheckBox
    Friend WithEvents Label19 As Label
    Friend WithEvents FlowLayoutPanel24 As FlowLayoutPanel
    Friend WithEvents chkDGPOMainPost As CheckBox
    Friend WithEvents FlowLayoutPanel25 As FlowLayoutPanel
    Friend WithEvents chkDGPOFullDescription As CheckBox
    Friend WithEvents Label23 As Label
    Friend WithEvents FlowLayoutPanel26 As FlowLayoutPanel
    Friend WithEvents chkDGPOAltRestrictions As CheckBox
    Friend WithEvents FlowLayoutPanel27 As FlowLayoutPanel
    Friend WithEvents chkDGPOWeatherInfo As CheckBox
    Friend WithEvents FlowLayoutPanel28 As FlowLayoutPanel
    Friend WithEvents chkDGPOWeatherChart As CheckBox
    Friend WithEvents FlowLayoutPanel29 As FlowLayoutPanel
    Friend WithEvents chkDGPOWaypoints As CheckBox
    Friend WithEvents FlowLayoutPanel30 As FlowLayoutPanel
    Friend WithEvents chkDGPOAddOns As CheckBox
    Friend WithEvents Label49 As Label
    Friend WithEvents lblTaskLibraryIDNotAcquired As Label
    Friend WithEvents lblTaskLibraryIDAcquired As Label
    Friend WithEvents btnDPORecallSettings As Button
    Friend WithEvents btnDPORememberSettings As Button
    Friend WithEvents btnDGPORecallSettings As Button
    Friend WithEvents btnDGPORememberSettings As Button
    Friend WithEvents Label12 As Label
    Friend WithEvents Label37 As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents lblThread1stMsgIDAcquired As Label
    Friend WithEvents lblThread1stMsgIDNotAcquired As Label
    Friend WithEvents pnlFullWorkflowTaskGroupFlight As GroupBox
    Friend WithEvents btnStartFullPostingWorkflow As Button
    Friend WithEvents btnTaskAndGroupEventLinks As Button
    Friend WithEvents btnWeatherBrowser As Button
    Friend WithEvents btnSaveDescriptionTemplate As Button
    Friend WithEvents btnRecallTaskDescriptionTemplate As Button
    Friend WithEvents btnLoadEventDescriptionTemplate As Button
    Friend WithEvents btnSaveEventDescriptionTemplate As Button
    Friend WithEvents btnCreateInTaskBrowser As Button
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents btnDeleteFromTaskBrowser As Button
    Friend WithEvents btnUpdateInTaskBrowser As Button
    Friend WithEvents lblTaskBrowserIDAndDate As Label
    Friend WithEvents btnDeleteEventNews As Button
    Friend WithEvents btnPublishEventNews As Button
    Friend WithEvents btnRepostOriginalURLPaste As Button
    Friend WithEvents txtRepostOriginalURL As TextBox
    Friend WithEvents txtLastUpdateDescription As TextBox
    Friend WithEvents txtAATTask As TextBox
    Friend WithEvents cboKnownTaskDesigners As ComboBox
    Friend WithEvents txtTrackerGroup As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents lblGroupEmoji As Label
End Class
