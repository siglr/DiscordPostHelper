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
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.tbpgAddOns = New System.Windows.Forms.TabPage()
        Me.AddOnsDataGrid = New System.Windows.Forms.DataGridView()
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
        Me.tbpgImages.SuspendLayout()
        CType(Me.imagesTabDivider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.imagesTabDivider.Panel1.SuspendLayout()
        Me.imagesTabDivider.Panel2.SuspendLayout()
        Me.imagesTabDivider.SuspendLayout()
        Me.tbpgXBOX.SuspendLayout()
        CType(Me.waypointCoordinatesDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpgAddOns.SuspendLayout()
        CType(Me.AddOnsDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.tabsBriefing.Controls.Add(Me.tbpgAddOns)
        Me.tabsBriefing.Dock = System.Windows.Forms.DockStyle.Fill
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
        Me.tbpgMainTaskInfo.Controls.Add(Me.txtBriefing)
        Me.tbpgMainTaskInfo.Location = New System.Drawing.Point(4, 29)
        Me.tbpgMainTaskInfo.Name = "tbpgMainTaskInfo"
        Me.tbpgMainTaskInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpgMainTaskInfo.Size = New System.Drawing.Size(982, 710)
        Me.tbpgMainTaskInfo.TabIndex = 0
        Me.tbpgMainTaskInfo.Text = "Main Task Info"
        Me.tbpgMainTaskInfo.UseVisualStyleBackColor = True
        '
        'txtBriefing
        '
        Me.txtBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBriefing.Font = New System.Drawing.Font("Segoe UI Variable Display", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBriefing.Location = New System.Drawing.Point(3, 3)
        Me.txtBriefing.Name = "txtBriefing"
        Me.txtBriefing.ReadOnly = True
        Me.txtBriefing.Size = New System.Drawing.Size(976, 704)
        Me.txtBriefing.TabIndex = 4
        Me.txtBriefing.Text = ""
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
        Me.mapSplitterUpDown.Size = New System.Drawing.Size(976, 711)
        Me.mapSplitterUpDown.SplitterDistance = 327
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
        Me.mapAndWindLayersSplitter.Size = New System.Drawing.Size(976, 327)
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
        Me.imageViewer.Size = New System.Drawing.Size(715, 327)
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
        Me.windLayersFlowLayoutPnl.Size = New System.Drawing.Size(257, 327)
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
        Me.mapSplitterLeftRight.Size = New System.Drawing.Size(976, 380)
        Me.mapSplitterLeftRight.SplitterDistance = 650
        Me.mapSplitterLeftRight.TabIndex = 0
        '
        'txtFullDescription
        '
        Me.txtFullDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtFullDescription.Location = New System.Drawing.Point(0, 0)
        Me.txtFullDescription.Name = "txtFullDescription"
        Me.txtFullDescription.ReadOnly = True
        Me.txtFullDescription.Size = New System.Drawing.Size(650, 380)
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
        Me.restrictionsDataGrid.Size = New System.Drawing.Size(322, 380)
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
        Me.eventInfoSplitContainer.Size = New System.Drawing.Size(982, 717)
        Me.eventInfoSplitContainer.SplitterDistance = 790
        Me.eventInfoSplitContainer.TabIndex = 0
        '
        'txtEventInfo
        '
        Me.txtEventInfo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtEventInfo.Font = New System.Drawing.Font("Segoe UI Variable Display", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEventInfo.Location = New System.Drawing.Point(0, 0)
        Me.txtEventInfo.Name = "txtEventInfo"
        Me.txtEventInfo.ReadOnly = True
        Me.txtEventInfo.Size = New System.Drawing.Size(790, 717)
        Me.txtEventInfo.TabIndex = 6
        Me.txtEventInfo.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtEventInfo, "Use CTRL-MouseWheel to make the content smaller or larger.")
        '
        'lblInsideOutside60Minutes
        '
        Me.lblInsideOutside60Minutes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblInsideOutside60Minutes.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblInsideOutside60Minutes.Location = New System.Drawing.Point(2, 382)
        Me.lblInsideOutside60Minutes.Name = "lblInsideOutside60Minutes"
        Me.lblInsideOutside60Minutes.Size = New System.Drawing.Size(183, 119)
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
        Me.msfsLocalTimeToSet.Location = New System.Drawing.Point(2, 528)
        Me.msfsLocalTimeToSet.Name = "msfsLocalTimeToSet"
        Me.msfsLocalTimeToSet.Size = New System.Drawing.Size(183, 32)
        Me.msfsLocalTimeToSet.TabIndex = 9
        Me.msfsLocalTimeToSet.Text = "12:00 PM"
        Me.msfsLocalTimeToSet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'msfsLocalDateToSet
        '
        Me.msfsLocalDateToSet.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.msfsLocalDateToSet.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Bold)
        Me.msfsLocalDateToSet.Location = New System.Drawing.Point(2, 500)
        Me.msfsLocalDateToSet.Name = "msfsLocalDateToSet"
        Me.msfsLocalDateToSet.Size = New System.Drawing.Size(183, 32)
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
        Me.imagesTabDivider.Size = New System.Drawing.Size(982, 717)
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
        Me.imagesTabViewerControl.Size = New System.Drawing.Size(814, 717)
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
        Me.imagesListView.Size = New System.Drawing.Size(164, 717)
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
        Me.Label2.Size = New System.Drawing.Size(72, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Distances"
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
        'Timer1
        '
        Me.Timer1.Interval = 1000
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
        'countDownTaskStart
        '
        Me.countDownTaskStart.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.countDownTaskStart.Location = New System.Drawing.Point(7, 316)
        Me.countDownTaskStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.countDownTaskStart.Name = "countDownTaskStart"
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
        CType(Me.eventInfoSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.eventInfoSplitContainer.ResumeLayout(False)
        Me.tbpgImages.ResumeLayout(False)
        Me.imagesTabDivider.Panel1.ResumeLayout(False)
        Me.imagesTabDivider.Panel2.ResumeLayout(False)
        CType(Me.imagesTabDivider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.imagesTabDivider.ResumeLayout(False)
        Me.tbpgXBOX.ResumeLayout(False)
        Me.tbpgXBOX.PerformLayout()
        CType(Me.waypointCoordinatesDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpgAddOns.ResumeLayout(False)
        CType(Me.AddOnsDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
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
End Class
