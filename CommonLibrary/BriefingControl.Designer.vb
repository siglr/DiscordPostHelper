<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BriefingControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.pnlTaskBriefing = New System.Windows.Forms.Panel()
        Me.tabsBriefing = New System.Windows.Forms.TabControl()
        Me.tbpgMainTaskInfo = New System.Windows.Forms.TabPage()
        Me.countryFlagsLayoutPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.txtBriefing = New System.Windows.Forms.RichTextBox()
        Me.tbpgMap = New System.Windows.Forms.TabPage()
        Me.mapSplitterUpDown = New System.Windows.Forms.SplitContainer()
        Me.mapAndWindLayersSplitter = New System.Windows.Forms.SplitContainer()
        Me.imageViewer = New SIGLR.SoaringTools.ImageViewer.ImageViewerControl()
        Me.windLayersFlowLayoutPnl = New System.Windows.Forms.FlowLayoutPanel()
        Me.mapSplitterLeftRight = New System.Windows.Forms.SplitContainer()
        Me.txtFullDescription = New System.Windows.Forms.RichTextBox()
        Me.restrictionsDataGrid = New System.Windows.Forms.DataGridView()
        Me.tbpgEventInfo = New System.Windows.Forms.TabPage()
        Me.eventInfoSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.txtEventInfo = New System.Windows.Forms.RichTextBox()
        Me.btnTestAudioCueVolume = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.trackAudioCueVolume = New System.Windows.Forms.TrackBar()
        Me.lblInsideOutside60Minutes = New System.Windows.Forms.Label()
        Me.msfsLocalTimeToSet = New System.Windows.Forms.Label()
        Me.msfsLocalDateToSet = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbpgImages = New System.Windows.Forms.TabPage()
        Me.imagesTabDivider = New System.Windows.Forms.SplitContainer()
        Me.imagesTabViewerControl = New SIGLR.SoaringTools.ImageViewer.ImageViewerControl()
        Me.imagesListView = New System.Windows.Forms.ListView()
        Me.tbpgXBOX = New System.Windows.Forms.TabPage()
        Me.chkWPEnableLatLonColumns = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cboWayPointDistances = New System.Windows.Forms.ComboBox()
        Me.waypointCoordinatesDataGrid = New System.Windows.Forms.DataGridView()
        Me.tbpgWeather = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.cloudLayersDatagrid = New System.Windows.Forms.DataGridView()
        Me.windLayersDatagrid = New System.Windows.Forms.DataGridView()
        Me.tbpgAddOns = New System.Windows.Forms.TabPage()
        Me.AddOnsDataGrid = New System.Windows.Forms.DataGridView()
        Me.tabUnits = New System.Windows.Forms.TabPage()
        Me.lblPrefUnitsMessage = New System.Windows.Forms.Label()
        Me.grbTemperature = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel6 = New System.Windows.Forms.FlowLayoutPanel()
        Me.radioTemperatureCelsius = New System.Windows.Forms.RadioButton()
        Me.radioTemperatureFarenheit = New System.Windows.Forms.RadioButton()
        Me.radioTemperatureBoth = New System.Windows.Forms.RadioButton()
        Me.grbWindSpeed = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel5 = New System.Windows.Forms.FlowLayoutPanel()
        Me.radioWindSpeedMps = New System.Windows.Forms.RadioButton()
        Me.radioWindSpeedKnots = New System.Windows.Forms.RadioButton()
        Me.radioWindSpeedBoth = New System.Windows.Forms.RadioButton()
        Me.grbGateDiameter = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel4 = New System.Windows.Forms.FlowLayoutPanel()
        Me.radioGateDiameterMetric = New System.Windows.Forms.RadioButton()
        Me.radioGateDiameterImperial = New System.Windows.Forms.RadioButton()
        Me.radioGateDiameterBoth = New System.Windows.Forms.RadioButton()
        Me.grbDistances = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.radioDistanceMetric = New System.Windows.Forms.RadioButton()
        Me.radioDistanceImperial = New System.Windows.Forms.RadioButton()
        Me.radioDistanceBoth = New System.Windows.Forms.RadioButton()
        Me.grbBarometric = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.radioBaroHPa = New System.Windows.Forms.RadioButton()
        Me.radioBaroInHg = New System.Windows.Forms.RadioButton()
        Me.radioBaroBoth = New System.Windows.Forms.RadioButton()
        Me.grbAltitudes = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel3 = New System.Windows.Forms.FlowLayoutPanel()
        Me.radioAltitudeMeters = New System.Windows.Forms.RadioButton()
        Me.radioAltitudeFeet = New System.Windows.Forms.RadioButton()
        Me.radioAltitudeBoth = New System.Windows.Forms.RadioButton()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.countDownTaskStart = New SIGLR.SoaringTools.CommonLibrary.Countdown()
        Me.countDownToLaunch = New SIGLR.SoaringTools.CommonLibrary.Countdown()
        Me.countDownToSyncFly = New SIGLR.SoaringTools.CommonLibrary.Countdown()
        Me.countDownToMeet = New SIGLR.SoaringTools.CommonLibrary.Countdown()
        Me.pnlTaskBriefing.SuspendLayout()
        Me.tabsBriefing.SuspendLayout()
        Me.tbpgMainTaskInfo.SuspendLayout()
        Me.tbpgMap.SuspendLayout()
        CType(Me.mapSplitterUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mapSplitterUpDown.Panel1.SuspendLayout()
        Me.mapSplitterUpDown.Panel2.SuspendLayout()
        Me.mapSplitterUpDown.SuspendLayout()
        CType(Me.mapAndWindLayersSplitter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mapAndWindLayersSplitter.Panel1.SuspendLayout()
        Me.mapAndWindLayersSplitter.Panel2.SuspendLayout()
        Me.mapAndWindLayersSplitter.SuspendLayout()
        CType(Me.mapSplitterLeftRight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mapSplitterLeftRight.Panel1.SuspendLayout()
        Me.mapSplitterLeftRight.Panel2.SuspendLayout()
        Me.mapSplitterLeftRight.SuspendLayout()
        CType(Me.restrictionsDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpgEventInfo.SuspendLayout()
        CType(Me.eventInfoSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.eventInfoSplitContainer.Panel1.SuspendLayout()
        Me.eventInfoSplitContainer.Panel2.SuspendLayout()
        Me.eventInfoSplitContainer.SuspendLayout()
        CType(Me.trackAudioCueVolume, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpgImages.SuspendLayout()
        CType(Me.imagesTabDivider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.imagesTabDivider.Panel1.SuspendLayout()
        Me.imagesTabDivider.Panel2.SuspendLayout()
        Me.imagesTabDivider.SuspendLayout()
        Me.tbpgXBOX.SuspendLayout()
        CType(Me.waypointCoordinatesDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpgWeather.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.cloudLayersDatagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.windLayersDatagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpgAddOns.SuspendLayout()
        CType(Me.AddOnsDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabUnits.SuspendLayout()
        Me.grbTemperature.SuspendLayout()
        Me.FlowLayoutPanel6.SuspendLayout()
        Me.grbWindSpeed.SuspendLayout()
        Me.FlowLayoutPanel5.SuspendLayout()
        Me.grbGateDiameter.SuspendLayout()
        Me.FlowLayoutPanel4.SuspendLayout()
        Me.grbDistances.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.grbBarometric.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.grbAltitudes.SuspendLayout()
        Me.FlowLayoutPanel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTaskBriefing
        '
        Me.pnlTaskBriefing.Controls.Add(Me.tabsBriefing)
        Me.pnlTaskBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlTaskBriefing.Location = New System.Drawing.Point(0, 0)
        Me.pnlTaskBriefing.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlTaskBriefing.Name = "pnlTaskBriefing"
        Me.pnlTaskBriefing.Padding = New System.Windows.Forms.Padding(7, 8, 7, 8)
        Me.pnlTaskBriefing.Size = New System.Drawing.Size(1004, 759)
        Me.pnlTaskBriefing.TabIndex = 4
        '
        'tabsBriefing
        '
        Me.tabsBriefing.Controls.Add(Me.tbpgMainTaskInfo)
        Me.tabsBriefing.Controls.Add(Me.tbpgMap)
        Me.tabsBriefing.Controls.Add(Me.tbpgEventInfo)
        Me.tabsBriefing.Controls.Add(Me.tbpgImages)
        Me.tabsBriefing.Controls.Add(Me.tbpgXBOX)
        Me.tabsBriefing.Controls.Add(Me.tbpgWeather)
        Me.tabsBriefing.Controls.Add(Me.tbpgAddOns)
        Me.tabsBriefing.Controls.Add(Me.tabUnits)
        Me.tabsBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabsBriefing.ItemSize = New System.Drawing.Size(100, 25)
        Me.tabsBriefing.Location = New System.Drawing.Point(7, 8)
        Me.tabsBriefing.Name = "tabsBriefing"
        Me.tabsBriefing.SelectedIndex = 0
        Me.tabsBriefing.Size = New System.Drawing.Size(990, 743)
        Me.tabsBriefing.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabsBriefing.TabIndex = 0
        '
        'tbpgMainTaskInfo
        '
        Me.tbpgMainTaskInfo.AutoScroll = True
        Me.tbpgMainTaskInfo.Controls.Add(Me.countryFlagsLayoutPanel)
        Me.tbpgMainTaskInfo.Controls.Add(Me.txtBriefing)
        Me.tbpgMainTaskInfo.Location = New System.Drawing.Point(4, 29)
        Me.tbpgMainTaskInfo.Name = "tbpgMainTaskInfo"
        Me.tbpgMainTaskInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpgMainTaskInfo.Size = New System.Drawing.Size(982, 710)
        Me.tbpgMainTaskInfo.TabIndex = 0
        Me.tbpgMainTaskInfo.Text = "Main Task Info"
        Me.tbpgMainTaskInfo.UseVisualStyleBackColor = True
        '
        'countryFlagsLayoutPanel
        '
        Me.countryFlagsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top
        Me.countryFlagsLayoutPanel.Location = New System.Drawing.Point(3, 3)
        Me.countryFlagsLayoutPanel.Name = "countryFlagsLayoutPanel"
        Me.countryFlagsLayoutPanel.Size = New System.Drawing.Size(976, 45)
        Me.countryFlagsLayoutPanel.TabIndex = 5
        '
        'txtBriefing
        '
        Me.txtBriefing.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtBriefing.Font = New System.Drawing.Font("Segoe UI Emoji", 15.70909!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBriefing.Location = New System.Drawing.Point(3, 54)
        Me.txtBriefing.Name = "txtBriefing"
        Me.txtBriefing.ReadOnly = True
        Me.txtBriefing.Size = New System.Drawing.Size(976, 653)
        Me.txtBriefing.TabIndex = 4
        Me.txtBriefing.Text = "⌚"
        Me.ToolTip1.SetToolTip(Me.txtBriefing, "Use CTRL-MouseWheel to make the content smaller or larger.")
        '
        'tbpgMap
        '
        Me.tbpgMap.AutoScroll = True
        Me.tbpgMap.Controls.Add(Me.mapSplitterUpDown)
        Me.tbpgMap.Location = New System.Drawing.Point(4, 29)
        Me.tbpgMap.Name = "tbpgMap"
        Me.tbpgMap.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpgMap.Size = New System.Drawing.Size(982, 710)
        Me.tbpgMap.TabIndex = 1
        Me.tbpgMap.Text = "Map"
        Me.tbpgMap.UseVisualStyleBackColor = True
        '
        'mapSplitterUpDown
        '
        Me.mapSplitterUpDown.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mapSplitterUpDown.Location = New System.Drawing.Point(3, 3)
        Me.mapSplitterUpDown.Name = "mapSplitterUpDown"
        Me.mapSplitterUpDown.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'mapSplitterUpDown.Panel1
        '
        Me.mapSplitterUpDown.Panel1.AutoScroll = True
        Me.mapSplitterUpDown.Panel1.Controls.Add(Me.mapAndWindLayersSplitter)
        '
        'mapSplitterUpDown.Panel2
        '
        Me.mapSplitterUpDown.Panel2.Controls.Add(Me.mapSplitterLeftRight)
        Me.mapSplitterUpDown.Size = New System.Drawing.Size(976, 704)
        Me.mapSplitterUpDown.SplitterDistance = 323
        Me.mapSplitterUpDown.TabIndex = 0
        '
        'mapAndWindLayersSplitter
        '
        Me.mapAndWindLayersSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mapAndWindLayersSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.mapAndWindLayersSplitter.IsSplitterFixed = True
        Me.mapAndWindLayersSplitter.Location = New System.Drawing.Point(0, 0)
        Me.mapAndWindLayersSplitter.Name = "mapAndWindLayersSplitter"
        '
        'mapAndWindLayersSplitter.Panel1
        '
        Me.mapAndWindLayersSplitter.Panel1.Controls.Add(Me.imageViewer)
        Me.mapAndWindLayersSplitter.Panel1MinSize = 500
        '
        'mapAndWindLayersSplitter.Panel2
        '
        Me.mapAndWindLayersSplitter.Panel2.Controls.Add(Me.windLayersFlowLayoutPnl)
        Me.mapAndWindLayersSplitter.Panel2MinSize = 100
        Me.mapAndWindLayersSplitter.Size = New System.Drawing.Size(976, 323)
        Me.mapAndWindLayersSplitter.SplitterDistance = 715
        Me.mapAndWindLayersSplitter.TabIndex = 0
        '
        'imageViewer
        '
        Me.imageViewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imageViewer.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.imageViewer.Location = New System.Drawing.Point(0, 0)
        Me.imageViewer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.imageViewer.Name = "imageViewer"
        Me.imageViewer.Size = New System.Drawing.Size(715, 323)
        Me.imageViewer.TabIndex = 1
        '
        'windLayersFlowLayoutPnl
        '
        Me.windLayersFlowLayoutPnl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.windLayersFlowLayoutPnl.AutoScroll = True
        Me.windLayersFlowLayoutPnl.Location = New System.Drawing.Point(0, 0)
        Me.windLayersFlowLayoutPnl.Name = "windLayersFlowLayoutPnl"
        Me.windLayersFlowLayoutPnl.Size = New System.Drawing.Size(257, 323)
        Me.windLayersFlowLayoutPnl.TabIndex = 0
        '
        'mapSplitterLeftRight
        '
        Me.mapSplitterLeftRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mapSplitterLeftRight.Location = New System.Drawing.Point(0, 0)
        Me.mapSplitterLeftRight.Name = "mapSplitterLeftRight"
        '
        'mapSplitterLeftRight.Panel1
        '
        Me.mapSplitterLeftRight.Panel1.Controls.Add(Me.txtFullDescription)
        '
        'mapSplitterLeftRight.Panel2
        '
        Me.mapSplitterLeftRight.Panel2.Controls.Add(Me.restrictionsDataGrid)
        Me.mapSplitterLeftRight.Size = New System.Drawing.Size(976, 377)
        Me.mapSplitterLeftRight.SplitterDistance = 650
        Me.mapSplitterLeftRight.TabIndex = 0
        '
        'txtFullDescription
        '
        Me.txtFullDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtFullDescription.Font = New System.Drawing.Font("Segoe UI Emoji", 9.818182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFullDescription.Location = New System.Drawing.Point(0, 0)
        Me.txtFullDescription.Name = "txtFullDescription"
        Me.txtFullDescription.ReadOnly = True
        Me.txtFullDescription.Size = New System.Drawing.Size(650, 377)
        Me.txtFullDescription.TabIndex = 0
        Me.txtFullDescription.Text = ""
        '
        'restrictionsDataGrid
        '
        Me.restrictionsDataGrid.AllowUserToAddRows = False
        Me.restrictionsDataGrid.AllowUserToDeleteRows = False
        Me.restrictionsDataGrid.AllowUserToResizeRows = False
        Me.restrictionsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.restrictionsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.restrictionsDataGrid.ColumnHeadersVisible = False
        Me.restrictionsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.restrictionsDataGrid.Location = New System.Drawing.Point(0, 0)
        Me.restrictionsDataGrid.Name = "restrictionsDataGrid"
        Me.restrictionsDataGrid.ReadOnly = True
        Me.restrictionsDataGrid.RowHeadersVisible = False
        Me.restrictionsDataGrid.RowHeadersWidth = 47
        Me.restrictionsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.restrictionsDataGrid.Size = New System.Drawing.Size(322, 377)
        Me.restrictionsDataGrid.TabIndex = 1
        '
        'tbpgEventInfo
        '
        Me.tbpgEventInfo.AutoScroll = True
        Me.tbpgEventInfo.Controls.Add(Me.eventInfoSplitContainer)
        Me.tbpgEventInfo.Location = New System.Drawing.Point(4, 29)
        Me.tbpgEventInfo.Name = "tbpgEventInfo"
        Me.tbpgEventInfo.Size = New System.Drawing.Size(982, 710)
        Me.tbpgEventInfo.TabIndex = 3
        Me.tbpgEventInfo.Text = "Event Info"
        Me.tbpgEventInfo.UseVisualStyleBackColor = True
        '
        'eventInfoSplitContainer
        '
        Me.eventInfoSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.eventInfoSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.eventInfoSplitContainer.IsSplitterFixed = True
        Me.eventInfoSplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.eventInfoSplitContainer.Name = "eventInfoSplitContainer"
        '
        'eventInfoSplitContainer.Panel1
        '
        Me.eventInfoSplitContainer.Panel1.Controls.Add(Me.txtEventInfo)
        '
        'eventInfoSplitContainer.Panel2
        '
        Me.eventInfoSplitContainer.Panel2.AutoScroll = True
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.btnTestAudioCueVolume)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.Label6)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.trackAudioCueVolume)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.lblInsideOutside60Minutes)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.msfsLocalTimeToSet)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.msfsLocalDateToSet)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.Label5)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.countDownTaskStart)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.Label4)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.countDownToLaunch)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.Label3)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.countDownToSyncFly)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.Label1)
        Me.eventInfoSplitContainer.Panel2.Controls.Add(Me.countDownToMeet)
        Me.eventInfoSplitContainer.Size = New System.Drawing.Size(982, 710)
        Me.eventInfoSplitContainer.SplitterDistance = 790
        Me.eventInfoSplitContainer.TabIndex = 0
        '
        'txtEventInfo
        '
        Me.txtEventInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtEventInfo.Font = New System.Drawing.Font("Segoe UI Emoji", 15.70909!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventInfo.Location = New System.Drawing.Point(0, 0)
        Me.txtEventInfo.Name = "txtEventInfo"
        Me.txtEventInfo.ReadOnly = True
        Me.txtEventInfo.Size = New System.Drawing.Size(790, 710)
        Me.txtEventInfo.TabIndex = 6
        Me.txtEventInfo.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtEventInfo, "Use CTRL-MouseWheel to make the content smaller or larger.")
        '
        'btnTestAudioCueVolume
        '
        Me.btnTestAudioCueVolume.Location = New System.Drawing.Point(136, 432)
        Me.btnTestAudioCueVolume.Name = "btnTestAudioCueVolume"
        Me.btnTestAudioCueVolume.Size = New System.Drawing.Size(42, 30)
        Me.btnTestAudioCueVolume.TabIndex = 15
        Me.btnTestAudioCueVolume.Text = "🔈"
        Me.ToolTip1.SetToolTip(Me.btnTestAudioCueVolume, "Click to test volume output")
        Me.btnTestAudioCueVolume.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(4, 382)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(181, 30)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Audio Cues Volume"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'trackAudioCueVolume
        '
        Me.trackAudioCueVolume.BackColor = System.Drawing.SystemColors.Control
        Me.trackAudioCueVolume.Location = New System.Drawing.Point(8, 412)
        Me.trackAudioCueVolume.Maximum = 100
        Me.trackAudioCueVolume.Name = "trackAudioCueVolume"
        Me.trackAudioCueVolume.Size = New System.Drawing.Size(170, 50)
        Me.trackAudioCueVolume.TabIndex = 13
        Me.trackAudioCueVolume.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trackAudioCueVolume, "Adjust the output volume of the audio cues")
        '
        'lblInsideOutside60Minutes
        '
        Me.lblInsideOutside60Minutes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblInsideOutside60Minutes.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblInsideOutside60Minutes.Location = New System.Drawing.Point(10, 469)
        Me.lblInsideOutside60Minutes.Name = "lblInsideOutside60Minutes"
        Me.lblInsideOutside60Minutes.Size = New System.Drawing.Size(168, 119)
        Me.lblInsideOutside60Minutes.TabIndex = 10
        Me.lblInsideOutside60Minutes.Text = "Within 60 minutes of the event's time." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "If clicking Fly now, MSFS local time sh" &
    "ould be:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.lblInsideOutside60Minutes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'msfsLocalTimeToSet
        '
        Me.msfsLocalTimeToSet.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.msfsLocalTimeToSet.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Bold)
        Me.msfsLocalTimeToSet.Location = New System.Drawing.Point(10, 615)
        Me.msfsLocalTimeToSet.Name = "msfsLocalTimeToSet"
        Me.msfsLocalTimeToSet.Size = New System.Drawing.Size(168, 32)
        Me.msfsLocalTimeToSet.TabIndex = 9
        Me.msfsLocalTimeToSet.Text = "12:00 PM"
        Me.msfsLocalTimeToSet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'msfsLocalDateToSet
        '
        Me.msfsLocalDateToSet.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.msfsLocalDateToSet.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Bold)
        Me.msfsLocalDateToSet.Location = New System.Drawing.Point(10, 587)
        Me.msfsLocalDateToSet.Name = "msfsLocalDateToSet"
        Me.msfsLocalDateToSet.Size = New System.Drawing.Size(168, 32)
        Me.msfsLocalDateToSet.TabIndex = 8
        Me.msfsLocalDateToSet.Text = "September 31, 2014"
        Me.msfsLocalDateToSet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(4, 281)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(181, 30)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Start task in"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(4, 189)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(181, 30)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "Launch in"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(4, 97)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(181, 30)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Sync Fly in"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(4, 4)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(181, 30)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Meet in"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tbpgImages
        '
        Me.tbpgImages.AutoScroll = True
        Me.tbpgImages.Controls.Add(Me.imagesTabDivider)
        Me.tbpgImages.Location = New System.Drawing.Point(4, 29)
        Me.tbpgImages.Name = "tbpgImages"
        Me.tbpgImages.Size = New System.Drawing.Size(982, 710)
        Me.tbpgImages.TabIndex = 2
        Me.tbpgImages.Text = "Images"
        Me.tbpgImages.UseVisualStyleBackColor = True
        '
        'imagesTabDivider
        '
        Me.imagesTabDivider.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imagesTabDivider.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.imagesTabDivider.IsSplitterFixed = True
        Me.imagesTabDivider.Location = New System.Drawing.Point(0, 0)
        Me.imagesTabDivider.Name = "imagesTabDivider"
        '
        'imagesTabDivider.Panel1
        '
        Me.imagesTabDivider.Panel1.Controls.Add(Me.imagesTabViewerControl)
        '
        'imagesTabDivider.Panel2
        '
        Me.imagesTabDivider.Panel2.Controls.Add(Me.imagesListView)
        Me.imagesTabDivider.Panel2MinSize = 100
        Me.imagesTabDivider.Size = New System.Drawing.Size(982, 710)
        Me.imagesTabDivider.SplitterDistance = 814
        Me.imagesTabDivider.TabIndex = 0
        '
        'imagesTabViewerControl
        '
        Me.imagesTabViewerControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imagesTabViewerControl.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.imagesTabViewerControl.Location = New System.Drawing.Point(0, 0)
        Me.imagesTabViewerControl.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.imagesTabViewerControl.Name = "imagesTabViewerControl"
        Me.imagesTabViewerControl.Size = New System.Drawing.Size(814, 710)
        Me.imagesTabViewerControl.TabIndex = 0
        '
        'imagesListView
        '
        Me.imagesListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imagesListView.FullRowSelect = True
        Me.imagesListView.HideSelection = False
        Me.imagesListView.Location = New System.Drawing.Point(0, 0)
        Me.imagesListView.MultiSelect = False
        Me.imagesListView.Name = "imagesListView"
        Me.imagesListView.Size = New System.Drawing.Size(164, 710)
        Me.imagesListView.TabIndex = 0
        Me.imagesListView.UseCompatibleStateImageBehavior = False
        '
        'tbpgXBOX
        '
        Me.tbpgXBOX.Controls.Add(Me.chkWPEnableLatLonColumns)
        Me.tbpgXBOX.Controls.Add(Me.Label2)
        Me.tbpgXBOX.Controls.Add(Me.cboWayPointDistances)
        Me.tbpgXBOX.Controls.Add(Me.waypointCoordinatesDataGrid)
        Me.tbpgXBOX.Location = New System.Drawing.Point(4, 29)
        Me.tbpgXBOX.Name = "tbpgXBOX"
        Me.tbpgXBOX.Size = New System.Drawing.Size(982, 710)
        Me.tbpgXBOX.TabIndex = 4
        Me.tbpgXBOX.Text = "All waypoints"
        Me.tbpgXBOX.UseVisualStyleBackColor = True
        '
        'chkWPEnableLatLonColumns
        '
        Me.chkWPEnableLatLonColumns.AutoSize = True
        Me.chkWPEnableLatLonColumns.Checked = True
        Me.chkWPEnableLatLonColumns.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkWPEnableLatLonColumns.Location = New System.Drawing.Point(253, 9)
        Me.chkWPEnableLatLonColumns.Name = "chkWPEnableLatLonColumns"
        Me.chkWPEnableLatLonColumns.Size = New System.Drawing.Size(218, 24)
        Me.chkWPEnableLatLonColumns.TabIndex = 3
        Me.chkWPEnableLatLonColumns.Text = "Show Latitude and Longitude"
        Me.ToolTip1.SetToolTip(Me.chkWPEnableLatLonColumns, "You can select to show or hide the latitude and longitude columns")
        Me.chkWPEnableLatLonColumns.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 10)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(65, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Distance"
        '
        'cboWayPointDistances
        '
        Me.cboWayPointDistances.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboWayPointDistances.FormattingEnabled = True
        Me.cboWayPointDistances.Items.AddRange(New Object() {"None", "Kilometers", "Miles"})
        Me.cboWayPointDistances.Location = New System.Drawing.Point(82, 7)
        Me.cboWayPointDistances.Name = "cboWayPointDistances"
        Me.cboWayPointDistances.Size = New System.Drawing.Size(165, 28)
        Me.cboWayPointDistances.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cboWayPointDistances, "You can select to display the distances columns")
        '
        'waypointCoordinatesDataGrid
        '
        Me.waypointCoordinatesDataGrid.AllowUserToAddRows = False
        Me.waypointCoordinatesDataGrid.AllowUserToDeleteRows = False
        Me.waypointCoordinatesDataGrid.AllowUserToResizeRows = False
        Me.waypointCoordinatesDataGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.waypointCoordinatesDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.waypointCoordinatesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.waypointCoordinatesDataGrid.Location = New System.Drawing.Point(0, 41)
        Me.waypointCoordinatesDataGrid.Name = "waypointCoordinatesDataGrid"
        Me.waypointCoordinatesDataGrid.RowHeadersWidth = 47
        Me.waypointCoordinatesDataGrid.Size = New System.Drawing.Size(982, 669)
        Me.waypointCoordinatesDataGrid.TabIndex = 0
        '
        'tbpgWeather
        '
        Me.tbpgWeather.Controls.Add(Me.SplitContainer1)
        Me.tbpgWeather.Location = New System.Drawing.Point(4, 29)
        Me.tbpgWeather.Name = "tbpgWeather"
        Me.tbpgWeather.Size = New System.Drawing.Size(982, 710)
        Me.tbpgWeather.TabIndex = 7
        Me.tbpgWeather.Text = "Weather"
        Me.tbpgWeather.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.cloudLayersDatagrid)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.windLayersDatagrid)
        Me.SplitContainer1.Size = New System.Drawing.Size(982, 710)
        Me.SplitContainer1.SplitterDistance = 327
        Me.SplitContainer1.TabIndex = 0
        '
        'cloudLayersDatagrid
        '
        Me.cloudLayersDatagrid.AllowUserToAddRows = False
        Me.cloudLayersDatagrid.AllowUserToDeleteRows = False
        Me.cloudLayersDatagrid.AllowUserToResizeRows = False
        Me.cloudLayersDatagrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.cloudLayersDatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.cloudLayersDatagrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cloudLayersDatagrid.Location = New System.Drawing.Point(0, 0)
        Me.cloudLayersDatagrid.Name = "cloudLayersDatagrid"
        Me.cloudLayersDatagrid.ReadOnly = True
        Me.cloudLayersDatagrid.RowHeadersWidth = 47
        Me.cloudLayersDatagrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.cloudLayersDatagrid.Size = New System.Drawing.Size(982, 327)
        Me.cloudLayersDatagrid.TabIndex = 1
        '
        'windLayersDatagrid
        '
        Me.windLayersDatagrid.AllowUserToAddRows = False
        Me.windLayersDatagrid.AllowUserToDeleteRows = False
        Me.windLayersDatagrid.AllowUserToResizeRows = False
        Me.windLayersDatagrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.windLayersDatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.windLayersDatagrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.windLayersDatagrid.Location = New System.Drawing.Point(0, 0)
        Me.windLayersDatagrid.Name = "windLayersDatagrid"
        Me.windLayersDatagrid.ReadOnly = True
        Me.windLayersDatagrid.RowHeadersWidth = 47
        Me.windLayersDatagrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.windLayersDatagrid.Size = New System.Drawing.Size(982, 379)
        Me.windLayersDatagrid.TabIndex = 2
        '
        'tbpgAddOns
        '
        Me.tbpgAddOns.Controls.Add(Me.AddOnsDataGrid)
        Me.tbpgAddOns.Location = New System.Drawing.Point(4, 29)
        Me.tbpgAddOns.Name = "tbpgAddOns"
        Me.tbpgAddOns.Size = New System.Drawing.Size(982, 710)
        Me.tbpgAddOns.TabIndex = 5
        Me.tbpgAddOns.Text = "Add-ons"
        Me.tbpgAddOns.UseVisualStyleBackColor = True
        '
        'AddOnsDataGrid
        '
        Me.AddOnsDataGrid.AllowUserToAddRows = False
        Me.AddOnsDataGrid.AllowUserToDeleteRows = False
        Me.AddOnsDataGrid.AllowUserToResizeRows = False
        Me.AddOnsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.AddOnsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AddOnsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AddOnsDataGrid.Location = New System.Drawing.Point(0, 0)
        Me.AddOnsDataGrid.Name = "AddOnsDataGrid"
        Me.AddOnsDataGrid.ReadOnly = True
        Me.AddOnsDataGrid.RowHeadersVisible = False
        Me.AddOnsDataGrid.RowHeadersWidth = 47
        Me.AddOnsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.AddOnsDataGrid.Size = New System.Drawing.Size(982, 710)
        Me.AddOnsDataGrid.TabIndex = 2
        '
        'tabUnits
        '
        Me.tabUnits.Controls.Add(Me.lblPrefUnitsMessage)
        Me.tabUnits.Controls.Add(Me.grbTemperature)
        Me.tabUnits.Controls.Add(Me.grbWindSpeed)
        Me.tabUnits.Controls.Add(Me.grbGateDiameter)
        Me.tabUnits.Controls.Add(Me.grbDistances)
        Me.tabUnits.Controls.Add(Me.grbBarometric)
        Me.tabUnits.Controls.Add(Me.grbAltitudes)
        Me.tabUnits.Location = New System.Drawing.Point(4, 29)
        Me.tabUnits.Name = "tabUnits"
        Me.tabUnits.Size = New System.Drawing.Size(982, 710)
        Me.tabUnits.TabIndex = 6
        Me.tabUnits.Text = "Units"
        Me.tabUnits.UseVisualStyleBackColor = True
        '
        'lblPrefUnitsMessage
        '
        Me.lblPrefUnitsMessage.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblPrefUnitsMessage.Location = New System.Drawing.Point(0, 606)
        Me.lblPrefUnitsMessage.Name = "lblPrefUnitsMessage"
        Me.lblPrefUnitsMessage.Size = New System.Drawing.Size(982, 104)
        Me.lblPrefUnitsMessage.TabIndex = 9
        Me.lblPrefUnitsMessage.Text = "Units selected here are only used for YOUR briefing tabs." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Any data specified in " &
    "description fields is excluded and will appear as it is."
        Me.lblPrefUnitsMessage.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'grbTemperature
        '
        Me.grbTemperature.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbTemperature.Controls.Add(Me.FlowLayoutPanel6)
        Me.grbTemperature.Location = New System.Drawing.Point(3, 318)
        Me.grbTemperature.Name = "grbTemperature"
        Me.grbTemperature.Size = New System.Drawing.Size(976, 57)
        Me.grbTemperature.TabIndex = 8
        Me.grbTemperature.TabStop = False
        Me.grbTemperature.Text = "Temperature"
        '
        'FlowLayoutPanel6
        '
        Me.FlowLayoutPanel6.Controls.Add(Me.radioTemperatureCelsius)
        Me.FlowLayoutPanel6.Controls.Add(Me.radioTemperatureFarenheit)
        Me.FlowLayoutPanel6.Controls.Add(Me.radioTemperatureBoth)
        Me.FlowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel6.Location = New System.Drawing.Point(3, 23)
        Me.FlowLayoutPanel6.Name = "FlowLayoutPanel6"
        Me.FlowLayoutPanel6.Size = New System.Drawing.Size(970, 31)
        Me.FlowLayoutPanel6.TabIndex = 0
        '
        'radioTemperatureCelsius
        '
        Me.radioTemperatureCelsius.AutoSize = True
        Me.radioTemperatureCelsius.Location = New System.Drawing.Point(3, 3)
        Me.radioTemperatureCelsius.Name = "radioTemperatureCelsius"
        Me.radioTemperatureCelsius.Size = New System.Drawing.Size(74, 24)
        Me.radioTemperatureCelsius.TabIndex = 1
        Me.radioTemperatureCelsius.Tag = "0"
        Me.radioTemperatureCelsius.Text = "Celsius"
        Me.radioTemperatureCelsius.UseVisualStyleBackColor = True
        '
        'radioTemperatureFarenheit
        '
        Me.radioTemperatureFarenheit.AutoSize = True
        Me.radioTemperatureFarenheit.Location = New System.Drawing.Point(83, 3)
        Me.radioTemperatureFarenheit.Name = "radioTemperatureFarenheit"
        Me.radioTemperatureFarenheit.Size = New System.Drawing.Size(85, 24)
        Me.radioTemperatureFarenheit.TabIndex = 0
        Me.radioTemperatureFarenheit.Tag = "1"
        Me.radioTemperatureFarenheit.Text = "Farenheit"
        Me.radioTemperatureFarenheit.UseVisualStyleBackColor = True
        '
        'radioTemperatureBoth
        '
        Me.radioTemperatureBoth.AutoSize = True
        Me.radioTemperatureBoth.Location = New System.Drawing.Point(174, 3)
        Me.radioTemperatureBoth.Name = "radioTemperatureBoth"
        Me.radioTemperatureBoth.Size = New System.Drawing.Size(57, 24)
        Me.radioTemperatureBoth.TabIndex = 2
        Me.radioTemperatureBoth.Tag = "2"
        Me.radioTemperatureBoth.Text = "Both"
        Me.radioTemperatureBoth.UseVisualStyleBackColor = True
        '
        'grbWindSpeed
        '
        Me.grbWindSpeed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbWindSpeed.Controls.Add(Me.FlowLayoutPanel5)
        Me.grbWindSpeed.Location = New System.Drawing.Point(3, 192)
        Me.grbWindSpeed.Name = "grbWindSpeed"
        Me.grbWindSpeed.Size = New System.Drawing.Size(976, 57)
        Me.grbWindSpeed.TabIndex = 7
        Me.grbWindSpeed.TabStop = False
        Me.grbWindSpeed.Text = "Wind Speed"
        '
        'FlowLayoutPanel5
        '
        Me.FlowLayoutPanel5.Controls.Add(Me.radioWindSpeedMps)
        Me.FlowLayoutPanel5.Controls.Add(Me.radioWindSpeedKnots)
        Me.FlowLayoutPanel5.Controls.Add(Me.radioWindSpeedBoth)
        Me.FlowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel5.Location = New System.Drawing.Point(3, 23)
        Me.FlowLayoutPanel5.Name = "FlowLayoutPanel5"
        Me.FlowLayoutPanel5.Size = New System.Drawing.Size(970, 31)
        Me.FlowLayoutPanel5.TabIndex = 0
        '
        'radioWindSpeedMps
        '
        Me.radioWindSpeedMps.AutoSize = True
        Me.radioWindSpeedMps.Location = New System.Drawing.Point(3, 3)
        Me.radioWindSpeedMps.Name = "radioWindSpeedMps"
        Me.radioWindSpeedMps.Size = New System.Drawing.Size(152, 24)
        Me.radioWindSpeedMps.TabIndex = 0
        Me.radioWindSpeedMps.Tag = "0"
        Me.radioWindSpeedMps.Text = "Meters per second"
        Me.radioWindSpeedMps.UseVisualStyleBackColor = True
        '
        'radioWindSpeedKnots
        '
        Me.radioWindSpeedKnots.AutoSize = True
        Me.radioWindSpeedKnots.Location = New System.Drawing.Point(161, 3)
        Me.radioWindSpeedKnots.Name = "radioWindSpeedKnots"
        Me.radioWindSpeedKnots.Size = New System.Drawing.Size(65, 24)
        Me.radioWindSpeedKnots.TabIndex = 1
        Me.radioWindSpeedKnots.Tag = "1"
        Me.radioWindSpeedKnots.Text = "Knots"
        Me.radioWindSpeedKnots.UseVisualStyleBackColor = True
        '
        'radioWindSpeedBoth
        '
        Me.radioWindSpeedBoth.AutoSize = True
        Me.radioWindSpeedBoth.Location = New System.Drawing.Point(232, 3)
        Me.radioWindSpeedBoth.Name = "radioWindSpeedBoth"
        Me.radioWindSpeedBoth.Size = New System.Drawing.Size(57, 24)
        Me.radioWindSpeedBoth.TabIndex = 2
        Me.radioWindSpeedBoth.Tag = "2"
        Me.radioWindSpeedBoth.Text = "Both"
        Me.radioWindSpeedBoth.UseVisualStyleBackColor = True
        '
        'grbGateDiameter
        '
        Me.grbGateDiameter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbGateDiameter.Controls.Add(Me.FlowLayoutPanel4)
        Me.grbGateDiameter.Location = New System.Drawing.Point(3, 129)
        Me.grbGateDiameter.Name = "grbGateDiameter"
        Me.grbGateDiameter.Size = New System.Drawing.Size(976, 57)
        Me.grbGateDiameter.TabIndex = 6
        Me.grbGateDiameter.TabStop = False
        Me.grbGateDiameter.Text = "Gate Diameter"
        '
        'FlowLayoutPanel4
        '
        Me.FlowLayoutPanel4.Controls.Add(Me.radioGateDiameterMetric)
        Me.FlowLayoutPanel4.Controls.Add(Me.radioGateDiameterImperial)
        Me.FlowLayoutPanel4.Controls.Add(Me.radioGateDiameterBoth)
        Me.FlowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel4.Location = New System.Drawing.Point(3, 23)
        Me.FlowLayoutPanel4.Name = "FlowLayoutPanel4"
        Me.FlowLayoutPanel4.Size = New System.Drawing.Size(970, 31)
        Me.FlowLayoutPanel4.TabIndex = 0
        '
        'radioGateDiameterMetric
        '
        Me.radioGateDiameterMetric.AutoSize = True
        Me.radioGateDiameterMetric.Location = New System.Drawing.Point(3, 3)
        Me.radioGateDiameterMetric.Name = "radioGateDiameterMetric"
        Me.radioGateDiameterMetric.Size = New System.Drawing.Size(155, 24)
        Me.radioGateDiameterMetric.TabIndex = 0
        Me.radioGateDiameterMetric.Tag = "0"
        Me.radioGateDiameterMetric.Text = "Metric (Meters/Km)"
        Me.radioGateDiameterMetric.UseVisualStyleBackColor = True
        '
        'radioGateDiameterImperial
        '
        Me.radioGateDiameterImperial.AutoSize = True
        Me.radioGateDiameterImperial.Location = New System.Drawing.Point(164, 3)
        Me.radioGateDiameterImperial.Name = "radioGateDiameterImperial"
        Me.radioGateDiameterImperial.Size = New System.Drawing.Size(159, 24)
        Me.radioGateDiameterImperial.TabIndex = 1
        Me.radioGateDiameterImperial.Tag = "1"
        Me.radioGateDiameterImperial.Text = "Imperial (Feet/Miles)"
        Me.radioGateDiameterImperial.UseVisualStyleBackColor = True
        '
        'radioGateDiameterBoth
        '
        Me.radioGateDiameterBoth.AutoSize = True
        Me.radioGateDiameterBoth.Location = New System.Drawing.Point(329, 3)
        Me.radioGateDiameterBoth.Name = "radioGateDiameterBoth"
        Me.radioGateDiameterBoth.Size = New System.Drawing.Size(57, 24)
        Me.radioGateDiameterBoth.TabIndex = 2
        Me.radioGateDiameterBoth.Tag = "2"
        Me.radioGateDiameterBoth.Text = "Both"
        Me.radioGateDiameterBoth.UseVisualStyleBackColor = True
        '
        'grbDistances
        '
        Me.grbDistances.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbDistances.Controls.Add(Me.FlowLayoutPanel2)
        Me.grbDistances.Location = New System.Drawing.Point(3, 66)
        Me.grbDistances.Name = "grbDistances"
        Me.grbDistances.Size = New System.Drawing.Size(976, 57)
        Me.grbDistances.TabIndex = 5
        Me.grbDistances.TabStop = False
        Me.grbDistances.Text = "Distance"
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Controls.Add(Me.radioDistanceMetric)
        Me.FlowLayoutPanel2.Controls.Add(Me.radioDistanceImperial)
        Me.FlowLayoutPanel2.Controls.Add(Me.radioDistanceBoth)
        Me.FlowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(3, 23)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(970, 31)
        Me.FlowLayoutPanel2.TabIndex = 0
        '
        'radioDistanceMetric
        '
        Me.radioDistanceMetric.AutoSize = True
        Me.radioDistanceMetric.Location = New System.Drawing.Point(3, 3)
        Me.radioDistanceMetric.Name = "radioDistanceMetric"
        Me.radioDistanceMetric.Size = New System.Drawing.Size(151, 24)
        Me.radioDistanceMetric.TabIndex = 0
        Me.radioDistanceMetric.Tag = "0"
        Me.radioDistanceMetric.Text = "Metric (Kilometers)"
        Me.radioDistanceMetric.UseVisualStyleBackColor = True
        '
        'radioDistanceImperial
        '
        Me.radioDistanceImperial.AutoSize = True
        Me.radioDistanceImperial.Location = New System.Drawing.Point(160, 3)
        Me.radioDistanceImperial.Name = "radioDistanceImperial"
        Me.radioDistanceImperial.Size = New System.Drawing.Size(125, 24)
        Me.radioDistanceImperial.TabIndex = 1
        Me.radioDistanceImperial.Tag = "1"
        Me.radioDistanceImperial.Text = "Imperial (Miles)"
        Me.radioDistanceImperial.UseVisualStyleBackColor = True
        '
        'radioDistanceBoth
        '
        Me.radioDistanceBoth.AutoSize = True
        Me.radioDistanceBoth.Location = New System.Drawing.Point(291, 3)
        Me.radioDistanceBoth.Name = "radioDistanceBoth"
        Me.radioDistanceBoth.Size = New System.Drawing.Size(57, 24)
        Me.radioDistanceBoth.TabIndex = 2
        Me.radioDistanceBoth.Tag = "2"
        Me.radioDistanceBoth.Text = "Both"
        Me.radioDistanceBoth.UseVisualStyleBackColor = True
        '
        'grbBarometric
        '
        Me.grbBarometric.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbBarometric.Controls.Add(Me.FlowLayoutPanel1)
        Me.grbBarometric.Location = New System.Drawing.Point(3, 255)
        Me.grbBarometric.Name = "grbBarometric"
        Me.grbBarometric.Size = New System.Drawing.Size(976, 57)
        Me.grbBarometric.TabIndex = 4
        Me.grbBarometric.TabStop = False
        Me.grbBarometric.Text = "Barometric Pressure"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.radioBaroHPa)
        Me.FlowLayoutPanel1.Controls.Add(Me.radioBaroInHg)
        Me.FlowLayoutPanel1.Controls.Add(Me.radioBaroBoth)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 23)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(970, 31)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'radioBaroHPa
        '
        Me.radioBaroHPa.AutoSize = True
        Me.radioBaroHPa.Location = New System.Drawing.Point(3, 3)
        Me.radioBaroHPa.Name = "radioBaroHPa"
        Me.radioBaroHPa.Size = New System.Drawing.Size(49, 24)
        Me.radioBaroHPa.TabIndex = 1
        Me.radioBaroHPa.Tag = "0"
        Me.radioBaroHPa.Text = "hPa"
        Me.radioBaroHPa.UseVisualStyleBackColor = True
        '
        'radioBaroInHg
        '
        Me.radioBaroInHg.AutoSize = True
        Me.radioBaroInHg.Location = New System.Drawing.Point(58, 3)
        Me.radioBaroInHg.Name = "radioBaroInHg"
        Me.radioBaroInHg.Size = New System.Drawing.Size(58, 24)
        Me.radioBaroInHg.TabIndex = 0
        Me.radioBaroInHg.Tag = "1"
        Me.radioBaroInHg.Text = "inHg"
        Me.radioBaroInHg.UseVisualStyleBackColor = True
        '
        'radioBaroBoth
        '
        Me.radioBaroBoth.AutoSize = True
        Me.radioBaroBoth.Location = New System.Drawing.Point(122, 3)
        Me.radioBaroBoth.Name = "radioBaroBoth"
        Me.radioBaroBoth.Size = New System.Drawing.Size(57, 24)
        Me.radioBaroBoth.TabIndex = 2
        Me.radioBaroBoth.Tag = "2"
        Me.radioBaroBoth.Text = "Both"
        Me.radioBaroBoth.UseVisualStyleBackColor = True
        '
        'grbAltitudes
        '
        Me.grbAltitudes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grbAltitudes.Controls.Add(Me.FlowLayoutPanel3)
        Me.grbAltitudes.Location = New System.Drawing.Point(3, 3)
        Me.grbAltitudes.Name = "grbAltitudes"
        Me.grbAltitudes.Size = New System.Drawing.Size(976, 57)
        Me.grbAltitudes.TabIndex = 3
        Me.grbAltitudes.TabStop = False
        Me.grbAltitudes.Text = "Altitude"
        '
        'FlowLayoutPanel3
        '
        Me.FlowLayoutPanel3.Controls.Add(Me.radioAltitudeMeters)
        Me.FlowLayoutPanel3.Controls.Add(Me.radioAltitudeFeet)
        Me.FlowLayoutPanel3.Controls.Add(Me.radioAltitudeBoth)
        Me.FlowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel3.Location = New System.Drawing.Point(3, 23)
        Me.FlowLayoutPanel3.Name = "FlowLayoutPanel3"
        Me.FlowLayoutPanel3.Size = New System.Drawing.Size(970, 31)
        Me.FlowLayoutPanel3.TabIndex = 0
        '
        'radioAltitudeMeters
        '
        Me.radioAltitudeMeters.AutoSize = True
        Me.radioAltitudeMeters.Location = New System.Drawing.Point(3, 3)
        Me.radioAltitudeMeters.Name = "radioAltitudeMeters"
        Me.radioAltitudeMeters.Size = New System.Drawing.Size(127, 24)
        Me.radioAltitudeMeters.TabIndex = 1
        Me.radioAltitudeMeters.Tag = "0"
        Me.radioAltitudeMeters.Text = "Metric (Meters)"
        Me.radioAltitudeMeters.UseVisualStyleBackColor = True
        '
        'radioAltitudeFeet
        '
        Me.radioAltitudeFeet.AutoSize = True
        Me.radioAltitudeFeet.Location = New System.Drawing.Point(136, 3)
        Me.radioAltitudeFeet.Name = "radioAltitudeFeet"
        Me.radioAltitudeFeet.Size = New System.Drawing.Size(119, 24)
        Me.radioAltitudeFeet.TabIndex = 0
        Me.radioAltitudeFeet.Tag = "1"
        Me.radioAltitudeFeet.Text = "Imperial (Feet)"
        Me.radioAltitudeFeet.UseVisualStyleBackColor = True
        '
        'radioAltitudeBoth
        '
        Me.radioAltitudeBoth.AutoSize = True
        Me.radioAltitudeBoth.Location = New System.Drawing.Point(261, 3)
        Me.radioAltitudeBoth.Name = "radioAltitudeBoth"
        Me.radioAltitudeBoth.Size = New System.Drawing.Size(57, 24)
        Me.radioAltitudeBoth.TabIndex = 2
        Me.radioAltitudeBoth.Tag = "2"
        Me.radioAltitudeBoth.Text = "Both"
        Me.radioAltitudeBoth.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'countDownTaskStart
        '
        Me.countDownTaskStart.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.countDownTaskStart.Location = New System.Drawing.Point(7, 316)
        Me.countDownTaskStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.countDownTaskStart.Name = "countDownTaskStart"
        Me.countDownTaskStart.PlayAudioCues = False
        Me.countDownTaskStart.Size = New System.Drawing.Size(173, 52)
        Me.countDownTaskStart.TabIndex = 6
        Me.countDownTaskStart.ZoomFactor = 2.0!
        '
        'countDownToLaunch
        '
        Me.countDownToLaunch.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.countDownToLaunch.Location = New System.Drawing.Point(7, 224)
        Me.countDownToLaunch.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.countDownToLaunch.Name = "countDownToLaunch"
        Me.countDownToLaunch.PlayAudioCues = False
        Me.countDownToLaunch.Size = New System.Drawing.Size(173, 52)
        Me.countDownToLaunch.TabIndex = 4
        Me.countDownToLaunch.ZoomFactor = 2.0!
        '
        'countDownToSyncFly
        '
        Me.countDownToSyncFly.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.countDownToSyncFly.Location = New System.Drawing.Point(7, 132)
        Me.countDownToSyncFly.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.countDownToSyncFly.Name = "countDownToSyncFly"
        Me.countDownToSyncFly.PlayAudioCues = False
        Me.countDownToSyncFly.Size = New System.Drawing.Size(173, 52)
        Me.countDownToSyncFly.TabIndex = 2
        Me.countDownToSyncFly.ZoomFactor = 2.0!
        '
        'countDownToMeet
        '
        Me.countDownToMeet.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.countDownToMeet.Location = New System.Drawing.Point(7, 39)
        Me.countDownToMeet.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.countDownToMeet.Name = "countDownToMeet"
        Me.countDownToMeet.PlayAudioCues = False
        Me.countDownToMeet.Size = New System.Drawing.Size(173, 52)
        Me.countDownToMeet.TabIndex = 0
        Me.countDownToMeet.ZoomFactor = 2.0!
        '
        'BriefingControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlTaskBriefing)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(700, 500)
        Me.Name = "BriefingControl"
        Me.Size = New System.Drawing.Size(1004, 759)
        Me.pnlTaskBriefing.ResumeLayout(False)
        Me.tabsBriefing.ResumeLayout(False)
        Me.tbpgMainTaskInfo.ResumeLayout(False)
        Me.tbpgMap.ResumeLayout(False)
        Me.mapSplitterUpDown.Panel1.ResumeLayout(False)
        Me.mapSplitterUpDown.Panel2.ResumeLayout(False)
        CType(Me.mapSplitterUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mapSplitterUpDown.ResumeLayout(False)
        Me.mapAndWindLayersSplitter.Panel1.ResumeLayout(False)
        Me.mapAndWindLayersSplitter.Panel2.ResumeLayout(False)
        CType(Me.mapAndWindLayersSplitter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mapAndWindLayersSplitter.ResumeLayout(False)
        Me.mapSplitterLeftRight.Panel1.ResumeLayout(False)
        Me.mapSplitterLeftRight.Panel2.ResumeLayout(False)
        CType(Me.mapSplitterLeftRight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mapSplitterLeftRight.ResumeLayout(False)
        CType(Me.restrictionsDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpgEventInfo.ResumeLayout(False)
        Me.eventInfoSplitContainer.Panel1.ResumeLayout(False)
        Me.eventInfoSplitContainer.Panel2.ResumeLayout(False)
        Me.eventInfoSplitContainer.Panel2.PerformLayout()
        CType(Me.eventInfoSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.eventInfoSplitContainer.ResumeLayout(False)
        CType(Me.trackAudioCueVolume, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpgImages.ResumeLayout(False)
        Me.imagesTabDivider.Panel1.ResumeLayout(False)
        Me.imagesTabDivider.Panel2.ResumeLayout(False)
        CType(Me.imagesTabDivider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.imagesTabDivider.ResumeLayout(False)
        Me.tbpgXBOX.ResumeLayout(False)
        Me.tbpgXBOX.PerformLayout()
        CType(Me.waypointCoordinatesDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpgWeather.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.cloudLayersDatagrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.windLayersDatagrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpgAddOns.ResumeLayout(False)
        CType(Me.AddOnsDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabUnits.ResumeLayout(False)
        Me.grbTemperature.ResumeLayout(False)
        Me.FlowLayoutPanel6.ResumeLayout(False)
        Me.FlowLayoutPanel6.PerformLayout()
        Me.grbWindSpeed.ResumeLayout(False)
        Me.FlowLayoutPanel5.ResumeLayout(False)
        Me.FlowLayoutPanel5.PerformLayout()
        Me.grbGateDiameter.ResumeLayout(False)
        Me.FlowLayoutPanel4.ResumeLayout(False)
        Me.FlowLayoutPanel4.PerformLayout()
        Me.grbDistances.ResumeLayout(False)
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.FlowLayoutPanel2.PerformLayout()
        Me.grbBarometric.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.grbAltitudes.ResumeLayout(False)
        Me.FlowLayoutPanel3.ResumeLayout(False)
        Me.FlowLayoutPanel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTaskBriefing As Windows.Forms.Panel
    Friend WithEvents tabsBriefing As Windows.Forms.TabControl
    Friend WithEvents tbpgMainTaskInfo As Windows.Forms.TabPage
    Friend WithEvents tbpgMap As Windows.Forms.TabPage
    Friend WithEvents tbpgEventInfo As Windows.Forms.TabPage
    Friend WithEvents tbpgImages As Windows.Forms.TabPage
    Friend WithEvents txtBriefing As Windows.Forms.RichTextBox
    Friend WithEvents ToolTip1 As Windows.Forms.ToolTip
    Friend WithEvents mapSplitterUpDown As Windows.Forms.SplitContainer
    Friend WithEvents mapSplitterLeftRight As Windows.Forms.SplitContainer
    Friend WithEvents txtFullDescription As Windows.Forms.RichTextBox
    Friend WithEvents tbpgXBOX As Windows.Forms.TabPage
    Friend WithEvents waypointCoordinatesDataGrid As Windows.Forms.DataGridView
    Friend WithEvents restrictionsDataGrid As Windows.Forms.DataGridView
    Friend WithEvents imagesTabDivider As Windows.Forms.SplitContainer
    Friend WithEvents imagesListView As Windows.Forms.ListView
    Friend WithEvents imagesTabViewerControl As ImageViewerControl
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents cboWayPointDistances As Windows.Forms.ComboBox
    Friend WithEvents chkWPEnableLatLonColumns As Windows.Forms.CheckBox
    Friend WithEvents eventInfoSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents txtEventInfo As Windows.Forms.RichTextBox
    Friend WithEvents countDownToMeet As Countdown
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents countDownToLaunch As Countdown
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents countDownToSyncFly As Countdown
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents countDownTaskStart As Countdown
    Friend WithEvents msfsLocalTimeToSet As Windows.Forms.Label
    Friend WithEvents msfsLocalDateToSet As Windows.Forms.Label
    Friend WithEvents Timer1 As Windows.Forms.Timer
    Friend WithEvents lblInsideOutside60Minutes As Windows.Forms.Label
    Friend WithEvents mapAndWindLayersSplitter As Windows.Forms.SplitContainer
    Friend WithEvents imageViewer As ImageViewerControl
    Friend WithEvents windLayersFlowLayoutPnl As Windows.Forms.FlowLayoutPanel
    Friend WithEvents tbpgAddOns As Windows.Forms.TabPage
    Friend WithEvents AddOnsDataGrid As Windows.Forms.DataGridView
    Friend WithEvents tabUnits As Windows.Forms.TabPage
    Friend WithEvents grbAltitudes As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel3 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents radioAltitudeFeet As Windows.Forms.RadioButton
    Friend WithEvents radioAltitudeMeters As Windows.Forms.RadioButton
    Friend WithEvents grbBarometric As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel1 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents radioBaroInHg As Windows.Forms.RadioButton
    Friend WithEvents radioBaroHPa As Windows.Forms.RadioButton
    Friend WithEvents radioBaroBoth As Windows.Forms.RadioButton
    Friend WithEvents radioAltitudeBoth As Windows.Forms.RadioButton
    Friend WithEvents grbDistances As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel2 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents radioDistanceMetric As Windows.Forms.RadioButton
    Friend WithEvents radioDistanceImperial As Windows.Forms.RadioButton
    Friend WithEvents radioDistanceBoth As Windows.Forms.RadioButton
    Friend WithEvents grbGateDiameter As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel4 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents radioGateDiameterMetric As Windows.Forms.RadioButton
    Friend WithEvents radioGateDiameterImperial As Windows.Forms.RadioButton
    Friend WithEvents radioGateDiameterBoth As Windows.Forms.RadioButton
    Friend WithEvents grbWindSpeed As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel5 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents radioWindSpeedMps As Windows.Forms.RadioButton
    Friend WithEvents radioWindSpeedKnots As Windows.Forms.RadioButton
    Friend WithEvents radioWindSpeedBoth As Windows.Forms.RadioButton
    Friend WithEvents grbTemperature As Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel6 As Windows.Forms.FlowLayoutPanel
    Friend WithEvents radioTemperatureCelsius As Windows.Forms.RadioButton
    Friend WithEvents radioTemperatureFarenheit As Windows.Forms.RadioButton
    Friend WithEvents radioTemperatureBoth As Windows.Forms.RadioButton
    Friend WithEvents countryFlagsLayoutPanel As Windows.Forms.FlowLayoutPanel
    Friend WithEvents tbpgWeather As Windows.Forms.TabPage
    Friend WithEvents SplitContainer1 As Windows.Forms.SplitContainer
    Friend WithEvents cloudLayersDatagrid As Windows.Forms.DataGridView
    Friend WithEvents windLayersDatagrid As Windows.Forms.DataGridView
    Friend WithEvents lblPrefUnitsMessage As Windows.Forms.Label
    Friend WithEvents trackAudioCueVolume As Windows.Forms.TrackBar
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents btnTestAudioCueVolume As Windows.Forms.Button
End Class
