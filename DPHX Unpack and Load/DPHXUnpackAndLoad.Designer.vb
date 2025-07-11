Imports SIGLR.SoaringTools.CommonLibrary

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DPHXUnpackAndLoad
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DPHXUnpackAndLoad))
        Me.pnlToolbar = New System.Windows.Forms.Panel()
        Me.ToolStrip1 = New SIGLR.SoaringTools.CommonLibrary.ToolStripExtensions.ToolStripExtended()
        Me.toolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.toolStripUnpack = New System.Windows.Forms.ToolStripButton()
        Me.toolStripCleanup = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripFileBrowser = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripWSGMap = New System.Windows.Forms.ToolStripButton()
        Me.toolStripWSGEvents = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripB21Planner = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.DiscordChannelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoToFeedbackChannelOnDiscordToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DiscordInviteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSettings = New System.Windows.Forms.ToolStripButton()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtPackageName = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.pnlDPHFile = New System.Windows.Forms.Panel()
        Me.msfs2024ToolStrip = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel2 = New System.Windows.Forms.ToolStripLabel()
        Me.tool2024StatusOK = New System.Windows.Forms.ToolStripButton()
        Me.tool2024StatusStop = New System.Windows.Forms.ToolStripButton()
        Me.tool2024StatusWarning = New System.Windows.Forms.ToolStripButton()
        Me.lbl2024AllFilesStatus = New System.Windows.Forms.ToolStripTextBox()
        Me.msfs2020ToolStrip = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.tool2020StatusOK = New System.Windows.Forms.ToolStripButton()
        Me.tool2020StatusStop = New System.Windows.Forms.ToolStripButton()
        Me.tool2020StatusWarning = New System.Windows.Forms.ToolStripButton()
        Me.lbl2020AllFilesStatus = New System.Windows.Forms.ToolStripTextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.warningMSFSRunningToolStrip = New System.Windows.Forms.ToolStripStatusLabel()
        Me.packageNameToolStrip = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ChkMSFS = New System.Windows.Forms.Timer(Me.components)
        Me.ctrlBriefing = New SIGLR.SoaringTools.CommonLibrary.BriefingControl()
        Me.pnlToolbar.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.pnlDPHFile.SuspendLayout()
        Me.msfs2024ToolStrip.SuspendLayout()
        Me.msfs2020ToolStrip.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlToolbar
        '
        Me.pnlToolbar.Controls.Add(Me.ToolStrip1)
        Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlToolbar.Location = New System.Drawing.Point(0, 0)
        Me.pnlToolbar.Name = "pnlToolbar"
        Me.pnlToolbar.Padding = New System.Windows.Forms.Padding(5, 5, 5, 10)
        Me.pnlToolbar.Size = New System.Drawing.Size(1006, 45)
        Me.pnlToolbar.TabIndex = 0
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ClickThrough = True
        Me.ToolStrip1.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripOpen, Me.toolStripUnpack, Me.toolStripCleanup, Me.ToolStripSeparator3, Me.toolStripFileBrowser, Me.ToolStripSeparator1, Me.toolStripWSGMap, Me.toolStripWSGEvents, Me.ToolStripSeparator4, Me.toolStripB21Planner, Me.ToolStripSeparator2, Me.ToolStripDropDownButton1, Me.toolStripSettings})
        Me.ToolStrip1.Location = New System.Drawing.Point(5, 5)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(996, 28)
        Me.ToolStrip1.SuppressHighlighting = False
        Me.ToolStrip1.TabIndex = 8
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'toolStripOpen
        '
        Me.toolStripOpen.Image = CType(resources.GetObject("toolStripOpen.Image"), System.Drawing.Image)
        Me.toolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripOpen.Name = "toolStripOpen"
        Me.toolStripOpen.Size = New System.Drawing.Size(70, 25)
        Me.toolStripOpen.Text = "&Open"
        Me.toolStripOpen.ToolTipText = "Click to select and load a DPHX pacage from your PC."
        '
        'toolStripUnpack
        '
        Me.toolStripUnpack.Enabled = False
        Me.toolStripUnpack.Image = CType(resources.GetObject("toolStripUnpack.Image"), System.Drawing.Image)
        Me.toolStripUnpack.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripUnpack.Name = "toolStripUnpack"
        Me.toolStripUnpack.Size = New System.Drawing.Size(89, 25)
        Me.toolStripUnpack.Text = "&Unpack!"
        Me.toolStripUnpack.ToolTipText = "Click to unpack the files to their proper locations"
        '
        'toolStripCleanup
        '
        Me.toolStripCleanup.Enabled = False
        Me.toolStripCleanup.Image = CType(resources.GetObject("toolStripCleanup.Image"), System.Drawing.Image)
        Me.toolStripCleanup.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripCleanup.Name = "toolStripCleanup"
        Me.toolStripCleanup.Size = New System.Drawing.Size(89, 25)
        Me.toolStripCleanup.Text = "&Cleanup"
        Me.toolStripCleanup.ToolTipText = "Click to reset ALL of the fiels and start from scratch."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 28)
        '
        'toolStripFileBrowser
        '
        Me.toolStripFileBrowser.Image = CType(resources.GetObject("toolStripFileBrowser.Image"), System.Drawing.Image)
        Me.toolStripFileBrowser.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripFileBrowser.Name = "toolStripFileBrowser"
        Me.toolStripFileBrowser.Size = New System.Drawing.Size(117, 25)
        Me.toolStripFileBrowser.Text = "File Browser"
        Me.toolStripFileBrowser.ToolTipText = "Click to open the File Browser"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 28)
        '
        'toolStripWSGMap
        '
        Me.toolStripWSGMap.Image = CType(resources.GetObject("toolStripWSGMap.Image"), System.Drawing.Image)
        Me.toolStripWSGMap.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripWSGMap.Name = "toolStripWSGMap"
        Me.toolStripWSGMap.Size = New System.Drawing.Size(63, 25)
        Me.toolStripWSGMap.Text = "Map"
        Me.toolStripWSGMap.ToolTipText = "Click to open WeSimGlide Map"
        '
        'toolStripWSGEvents
        '
        Me.toolStripWSGEvents.Image = CType(resources.GetObject("toolStripWSGEvents.Image"), System.Drawing.Image)
        Me.toolStripWSGEvents.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripWSGEvents.Name = "toolStripWSGEvents"
        Me.toolStripWSGEvents.Size = New System.Drawing.Size(77, 25)
        Me.toolStripWSGEvents.Text = "Events"
        Me.toolStripWSGEvents.ToolTipText = "Click to open WeSimGlide Events"
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
        'toolStripSettings
        '
        Me.toolStripSettings.Image = CType(resources.GetObject("toolStripSettings.Image"), System.Drawing.Image)
        Me.toolStripSettings.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripSettings.Name = "toolStripSettings"
        Me.toolStripSettings.Size = New System.Drawing.Size(88, 25)
        Me.toolStripSettings.Text = "&Settings"
        Me.toolStripSettings.ToolTipText = "Click to open the Settings windows"
        '
        'txtPackageName
        '
        Me.txtPackageName.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPackageName.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtPackageName.Location = New System.Drawing.Point(735, 11)
        Me.txtPackageName.Name = "txtPackageName"
        Me.txtPackageName.ReadOnly = True
        Me.txtPackageName.Size = New System.Drawing.Size(176, 27)
        Me.txtPackageName.TabIndex = 5
        Me.txtPackageName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ToolTip1.SetToolTip(Me.txtPackageName, "The currently loaded DPHX package file")
        Me.txtPackageName.Visible = False
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'pnlDPHFile
        '
        Me.pnlDPHFile.AutoSize = True
        Me.pnlDPHFile.Controls.Add(Me.msfs2024ToolStrip)
        Me.pnlDPHFile.Controls.Add(Me.msfs2020ToolStrip)
        Me.pnlDPHFile.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlDPHFile.Location = New System.Drawing.Point(0, 45)
        Me.pnlDPHFile.Name = "pnlDPHFile"
        Me.pnlDPHFile.Padding = New System.Windows.Forms.Padding(5)
        Me.pnlDPHFile.Size = New System.Drawing.Size(1006, 60)
        Me.pnlDPHFile.TabIndex = 2
        '
        'msfs2024ToolStrip
        '
        Me.msfs2024ToolStrip.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.msfs2024ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.msfs2024ToolStrip.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.msfs2024ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel2, Me.tool2024StatusOK, Me.tool2024StatusStop, Me.tool2024StatusWarning, Me.lbl2024AllFilesStatus})
        Me.msfs2024ToolStrip.Location = New System.Drawing.Point(5, 30)
        Me.msfs2024ToolStrip.Name = "msfs2024ToolStrip"
        Me.msfs2024ToolStrip.Size = New System.Drawing.Size(996, 25)
        Me.msfs2024ToolStrip.TabIndex = 1
        Me.msfs2024ToolStrip.Text = "ToolStrip2"
        '
        'ToolStripLabel2
        '
        Me.ToolStripLabel2.Name = "ToolStripLabel2"
        Me.ToolStripLabel2.Size = New System.Drawing.Size(82, 22)
        Me.ToolStripLabel2.Text = "MSFS 2024:"
        '
        'tool2024StatusOK
        '
        Me.tool2024StatusOK.AutoToolTip = False
        Me.tool2024StatusOK.BackgroundImage = CType(resources.GetObject("tool2024StatusOK.BackgroundImage"), System.Drawing.Image)
        Me.tool2024StatusOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.tool2024StatusOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tool2024StatusOK.Enabled = False
        Me.tool2024StatusOK.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tool2024StatusOK.Name = "tool2024StatusOK"
        Me.tool2024StatusOK.Size = New System.Drawing.Size(26, 22)
        Me.tool2024StatusOK.ToolTipText = "All ready to go!"
        Me.tool2024StatusOK.Visible = False
        '
        'tool2024StatusStop
        '
        Me.tool2024StatusStop.AutoToolTip = False
        Me.tool2024StatusStop.BackgroundImage = CType(resources.GetObject("tool2024StatusStop.BackgroundImage"), System.Drawing.Image)
        Me.tool2024StatusStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.tool2024StatusStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tool2024StatusStop.Enabled = False
        Me.tool2024StatusStop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tool2024StatusStop.Name = "tool2024StatusStop"
        Me.tool2024StatusStop.Size = New System.Drawing.Size(26, 22)
        Me.tool2024StatusStop.ToolTipText = "Stop right there or you won't get far!"
        Me.tool2024StatusStop.Visible = False
        '
        'tool2024StatusWarning
        '
        Me.tool2024StatusWarning.AutoToolTip = False
        Me.tool2024StatusWarning.BackgroundImage = CType(resources.GetObject("tool2024StatusWarning.BackgroundImage"), System.Drawing.Image)
        Me.tool2024StatusWarning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.tool2024StatusWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tool2024StatusWarning.Enabled = False
        Me.tool2024StatusWarning.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tool2024StatusWarning.Name = "tool2024StatusWarning"
        Me.tool2024StatusWarning.Size = New System.Drawing.Size(26, 22)
        Me.tool2024StatusWarning.ToolTipText = "Warning! Check your files!"
        Me.tool2024StatusWarning.Visible = False
        '
        'lbl2024AllFilesStatus
        '
        Me.lbl2024AllFilesStatus.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.lbl2024AllFilesStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lbl2024AllFilesStatus.Font = New System.Drawing.Font("Segoe UI", 9.163636!)
        Me.lbl2024AllFilesStatus.Margin = New System.Windows.Forms.Padding(5, 0, 1, 0)
        Me.lbl2024AllFilesStatus.Name = "lbl2024AllFilesStatus"
        Me.lbl2024AllFilesStatus.Size = New System.Drawing.Size(700, 25)
        '
        'msfs2020ToolStrip
        '
        Me.msfs2020ToolStrip.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.msfs2020ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.msfs2020ToolStrip.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.msfs2020ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.tool2020StatusOK, Me.tool2020StatusStop, Me.tool2020StatusWarning, Me.lbl2020AllFilesStatus})
        Me.msfs2020ToolStrip.Location = New System.Drawing.Point(5, 5)
        Me.msfs2020ToolStrip.Name = "msfs2020ToolStrip"
        Me.msfs2020ToolStrip.Size = New System.Drawing.Size(996, 25)
        Me.msfs2020ToolStrip.TabIndex = 0
        Me.msfs2020ToolStrip.Text = "ToolStrip2"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(82, 22)
        Me.ToolStripLabel1.Text = "MSFS 2020:"
        '
        'tool2020StatusOK
        '
        Me.tool2020StatusOK.AutoToolTip = False
        Me.tool2020StatusOK.BackgroundImage = CType(resources.GetObject("tool2020StatusOK.BackgroundImage"), System.Drawing.Image)
        Me.tool2020StatusOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.tool2020StatusOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tool2020StatusOK.Enabled = False
        Me.tool2020StatusOK.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tool2020StatusOK.Name = "tool2020StatusOK"
        Me.tool2020StatusOK.Size = New System.Drawing.Size(26, 22)
        Me.tool2020StatusOK.ToolTipText = "All ready to go!"
        Me.tool2020StatusOK.Visible = False
        '
        'tool2020StatusStop
        '
        Me.tool2020StatusStop.AutoToolTip = False
        Me.tool2020StatusStop.BackgroundImage = CType(resources.GetObject("tool2020StatusStop.BackgroundImage"), System.Drawing.Image)
        Me.tool2020StatusStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.tool2020StatusStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tool2020StatusStop.Enabled = False
        Me.tool2020StatusStop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tool2020StatusStop.Name = "tool2020StatusStop"
        Me.tool2020StatusStop.Size = New System.Drawing.Size(26, 22)
        Me.tool2020StatusStop.ToolTipText = "Stop right there or you won't get far!"
        Me.tool2020StatusStop.Visible = False
        '
        'tool2020StatusWarning
        '
        Me.tool2020StatusWarning.AutoToolTip = False
        Me.tool2020StatusWarning.BackgroundImage = CType(resources.GetObject("tool2020StatusWarning.BackgroundImage"), System.Drawing.Image)
        Me.tool2020StatusWarning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.tool2020StatusWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tool2020StatusWarning.Enabled = False
        Me.tool2020StatusWarning.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tool2020StatusWarning.Name = "tool2020StatusWarning"
        Me.tool2020StatusWarning.Size = New System.Drawing.Size(26, 22)
        Me.tool2020StatusWarning.ToolTipText = "Warning! Check your files!"
        Me.tool2020StatusWarning.Visible = False
        '
        'lbl2020AllFilesStatus
        '
        Me.lbl2020AllFilesStatus.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.lbl2020AllFilesStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lbl2020AllFilesStatus.Font = New System.Drawing.Font("Segoe UI", 9.163636!)
        Me.lbl2020AllFilesStatus.Margin = New System.Windows.Forms.Padding(5, 0, 1, 0)
        Me.lbl2020AllFilesStatus.Name = "lbl2020AllFilesStatus"
        Me.lbl2020AllFilesStatus.Size = New System.Drawing.Size(700, 25)
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.warningMSFSRunningToolStrip, Me.packageNameToolStrip})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 700)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1006, 24)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'warningMSFSRunningToolStrip
        '
        Me.warningMSFSRunningToolStrip.Font = New System.Drawing.Font("Segoe UI", 9.163636!, System.Drawing.FontStyle.Bold)
        Me.warningMSFSRunningToolStrip.ForeColor = System.Drawing.Color.Red
        Me.warningMSFSRunningToolStrip.Name = "warningMSFSRunningToolStrip"
        Me.warningMSFSRunningToolStrip.Size = New System.Drawing.Size(143, 19)
        Me.warningMSFSRunningToolStrip.Text = "⚠️MSFS Running⚠️"
        Me.warningMSFSRunningToolStrip.ToolTipText = "MSFS needs to be restarted to see new weather presets after unpacking."
        Me.warningMSFSRunningToolStrip.Visible = False
        '
        'packageNameToolStrip
        '
        Me.packageNameToolStrip.Name = "packageNameToolStrip"
        Me.packageNameToolStrip.Size = New System.Drawing.Size(93, 19)
        Me.packageNameToolStrip.Text = "No file loaded"
        '
        'ChkMSFS
        '
        Me.ChkMSFS.Enabled = True
        Me.ChkMSFS.Interval = 5000
        '
        'ctrlBriefing
        '
        Me.ctrlBriefing.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ctrlBriefing.EventIsEnabled = False
        Me.ctrlBriefing.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.ctrlBriefing.Location = New System.Drawing.Point(0, 105)
        Me.ctrlBriefing.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ctrlBriefing.MinimumSize = New System.Drawing.Size(700, 500)
        Me.ctrlBriefing.Name = "ctrlBriefing"
        Me.ctrlBriefing.Size = New System.Drawing.Size(1006, 597)
        Me.ctrlBriefing.TabIndex = 3
        '
        'DPHXUnpackAndLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1006, 724)
        Me.Controls.Add(Me.ctrlBriefing)
        Me.Controls.Add(Me.txtPackageName)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.pnlDPHFile)
        Me.Controls.Add(Me.pnlToolbar)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(1024, 768)
        Me.Name = "DPHXUnpackAndLoad"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.pnlToolbar.ResumeLayout(False)
        Me.pnlToolbar.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.pnlDPHFile.ResumeLayout(False)
        Me.pnlDPHFile.PerformLayout()
        Me.msfs2024ToolStrip.ResumeLayout(False)
        Me.msfs2024ToolStrip.PerformLayout()
        Me.msfs2020ToolStrip.ResumeLayout(False)
        Me.msfs2020ToolStrip.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents pnlDPHFile As Panel
    Friend WithEvents ctrlBriefing As CommonLibrary.BriefingControl
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents packageNameToolStrip As ToolStripStatusLabel
    Friend WithEvents ChkMSFS As Timer
    Friend WithEvents warningMSFSRunningToolStrip As ToolStripStatusLabel
    Friend WithEvents pnlToolbar As Panel
    Friend WithEvents toolStripOpen As ToolStripButton
    Friend WithEvents toolStripUnpack As ToolStripButton
    Friend WithEvents toolStripCleanup As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents toolStripB21Planner As ToolStripButton
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents DiscordChannelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GoToFeedbackChannelOnDiscordToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DiscordInviteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSettings As ToolStripButton
    Friend WithEvents txtPackageName As TextBox
    Friend WithEvents msfs2020ToolStrip As ToolStrip
    Friend WithEvents lbl2020AllFilesStatus As ToolStripTextBox
    Friend WithEvents tool2020StatusWarning As ToolStripButton
    Friend WithEvents tool2020StatusOK As ToolStripButton
    Friend WithEvents tool2020StatusStop As ToolStripButton
    Friend WithEvents ToolStrip1 As ToolStripExtensions.ToolStripExtended
    Friend WithEvents toolStripFileBrowser As ToolStripButton
    Friend WithEvents newsSplitContainer As SplitContainer
    Friend WithEvents msfs2024ToolStrip As ToolStrip
    Friend WithEvents tool2024StatusOK As ToolStripButton
    Friend WithEvents tool2024StatusStop As ToolStripButton
    Friend WithEvents tool2024StatusWarning As ToolStripButton
    Friend WithEvents lbl2024AllFilesStatus As ToolStripTextBox
    Friend WithEvents ToolStripLabel2 As ToolStripLabel
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents toolStripWSGMap As ToolStripButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents toolStripWSGEvents As ToolStripButton
End Class
