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
        Me.toolStripFileBrowser = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripDiscordTaskLibrary = New System.Windows.Forms.ToolStripButton()
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
        Me.ToolStrip2 = New System.Windows.Forms.ToolStrip()
        Me.toolStatusOK = New System.Windows.Forms.ToolStripButton()
        Me.toolStatusStop = New System.Windows.Forms.ToolStripButton()
        Me.toolStatusWarning = New System.Windows.Forms.ToolStripButton()
        Me.lblAllFilesStatus = New System.Windows.Forms.ToolStripTextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.warningMSFSRunningToolStrip = New System.Windows.Forms.ToolStripStatusLabel()
        Me.packageNameToolStrip = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ChkMSFS = New System.Windows.Forms.Timer(Me.components)
        Me.ctrlBriefing = New SIGLR.SoaringTools.CommonLibrary.BriefingControl()
        Me.flowNewsPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.RetrieveNewsList = New System.Windows.Forms.Timer(Me.components)
        Me.chkNewsRetrieval = New System.Windows.Forms.CheckBox()
        Me.pnlToolbar.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.pnlDPHFile.SuspendLayout()
        Me.ToolStrip2.SuspendLayout()
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
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripOpen, Me.toolStripUnpack, Me.toolStripCleanup, Me.toolStripFileBrowser, Me.ToolStripSeparator1, Me.toolStripDiscordTaskLibrary, Me.ToolStripSeparator4, Me.toolStripB21Planner, Me.ToolStripSeparator2, Me.ToolStripDropDownButton1, Me.toolStripSettings})
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
        'toolStripFileBrowser
        '
        Me.toolStripFileBrowser.Image = CType(resources.GetObject("toolStripFileBrowser.Image"), System.Drawing.Image)
        Me.toolStripFileBrowser.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStripFileBrowser.Name = "toolStripFileBrowser"
        Me.toolStripFileBrowser.Size = New System.Drawing.Size(117, 25)
        Me.toolStripFileBrowser.Text = "File Browser"
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
        Me.toolStripDiscordTaskLibrary.ToolTipText = "Click here to open the Task Library Browser."
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
        Me.txtPackageName.Location = New System.Drawing.Point(793, 55)
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
        Me.pnlDPHFile.Controls.Add(Me.ToolStrip2)
        Me.pnlDPHFile.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlDPHFile.Location = New System.Drawing.Point(0, 45)
        Me.pnlDPHFile.Name = "pnlDPHFile"
        Me.pnlDPHFile.Padding = New System.Windows.Forms.Padding(5)
        Me.pnlDPHFile.Size = New System.Drawing.Size(1006, 32)
        Me.pnlDPHFile.TabIndex = 2
        '
        'ToolStrip2
        '
        Me.ToolStrip2.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip2.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.ToolStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStatusOK, Me.toolStatusStop, Me.toolStatusWarning, Me.lblAllFilesStatus})
        Me.ToolStrip2.Location = New System.Drawing.Point(5, 5)
        Me.ToolStrip2.Name = "ToolStrip2"
        Me.ToolStrip2.Size = New System.Drawing.Size(996, 25)
        Me.ToolStrip2.TabIndex = 0
        Me.ToolStrip2.Text = "ToolStrip2"
        '
        'toolStatusOK
        '
        Me.toolStatusOK.AutoToolTip = False
        Me.toolStatusOK.BackgroundImage = CType(resources.GetObject("toolStatusOK.BackgroundImage"), System.Drawing.Image)
        Me.toolStatusOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.toolStatusOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.toolStatusOK.Enabled = False
        Me.toolStatusOK.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStatusOK.Name = "toolStatusOK"
        Me.toolStatusOK.Size = New System.Drawing.Size(26, 22)
        Me.toolStatusOK.ToolTipText = "All ready to go!"
        Me.toolStatusOK.Visible = False
        '
        'toolStatusStop
        '
        Me.toolStatusStop.AutoToolTip = False
        Me.toolStatusStop.BackgroundImage = CType(resources.GetObject("toolStatusStop.BackgroundImage"), System.Drawing.Image)
        Me.toolStatusStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.toolStatusStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.toolStatusStop.Enabled = False
        Me.toolStatusStop.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStatusStop.Name = "toolStatusStop"
        Me.toolStatusStop.Size = New System.Drawing.Size(26, 22)
        Me.toolStatusStop.ToolTipText = "Stop right there or you won't get far!"
        Me.toolStatusStop.Visible = False
        '
        'toolStatusWarning
        '
        Me.toolStatusWarning.AutoToolTip = False
        Me.toolStatusWarning.BackgroundImage = CType(resources.GetObject("toolStatusWarning.BackgroundImage"), System.Drawing.Image)
        Me.toolStatusWarning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.toolStatusWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.toolStatusWarning.Enabled = False
        Me.toolStatusWarning.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.toolStatusWarning.Name = "toolStatusWarning"
        Me.toolStatusWarning.Size = New System.Drawing.Size(26, 22)
        Me.toolStatusWarning.ToolTipText = "Warning! Check your files!"
        Me.toolStatusWarning.Visible = False
        '
        'lblAllFilesStatus
        '
        Me.lblAllFilesStatus.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.lblAllFilesStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lblAllFilesStatus.Font = New System.Drawing.Font("Segoe UI", 9.163636!)
        Me.lblAllFilesStatus.Margin = New System.Windows.Forms.Padding(5, 0, 1, 0)
        Me.lblAllFilesStatus.Name = "lblAllFilesStatus"
        Me.lblAllFilesStatus.Size = New System.Drawing.Size(800, 25)
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
        Me.warningMSFSRunningToolStrip.Size = New System.Drawing.Size(179, 19)
        Me.warningMSFSRunningToolStrip.Text = "⚠️MSFS 2020 Running⚠️"
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
        Me.ctrlBriefing.Location = New System.Drawing.Point(0, 77)
        Me.ctrlBriefing.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ctrlBriefing.MinimumSize = New System.Drawing.Size(700, 500)
        Me.ctrlBriefing.Name = "ctrlBriefing"
        Me.ctrlBriefing.Size = New System.Drawing.Size(737, 633)
        Me.ctrlBriefing.TabIndex = 3
        '
        'flowNewsPanel
        '
        Me.flowNewsPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.flowNewsPanel.AutoScroll = True
        Me.flowNewsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flowNewsPanel.Location = New System.Drawing.Point(728, 112)
        Me.flowNewsPanel.Name = "flowNewsPanel"
        Me.flowNewsPanel.Size = New System.Drawing.Size(278, 589)
        Me.flowNewsPanel.TabIndex = 6
        Me.flowNewsPanel.WrapContents = False
        '
        'RetrieveNewsList
        '
        Me.RetrieveNewsList.Enabled = True
        Me.RetrieveNewsList.Interval = 5000
        '
        'chkNewsRetrieval
        '
        Me.chkNewsRetrieval.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkNewsRetrieval.AutoSize = True
        Me.chkNewsRetrieval.Location = New System.Drawing.Point(757, 88)
        Me.chkNewsRetrieval.Name = "chkNewsRetrieval"
        Me.chkNewsRetrieval.Size = New System.Drawing.Size(237, 24)
        Me.chkNewsRetrieval.TabIndex = 7
        Me.chkNewsRetrieval.Text = "Disable automatic news retrieval"
        Me.ToolTip1.SetToolTip(Me.chkNewsRetrieval, "Check this to disable the automatic retrieval of news.")
        Me.chkNewsRetrieval.UseVisualStyleBackColor = True
        '
        'DPHXUnpackAndLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1006, 724)
        Me.Controls.Add(Me.chkNewsRetrieval)
        Me.Controls.Add(Me.flowNewsPanel)
        Me.Controls.Add(Me.txtPackageName)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ctrlBriefing)
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
        Me.ToolStrip2.ResumeLayout(False)
        Me.ToolStrip2.PerformLayout()
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
    Friend WithEvents toolStripDiscordTaskLibrary As ToolStripButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents toolStripB21Planner As ToolStripButton
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents DiscordChannelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GoToFeedbackChannelOnDiscordToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DiscordInviteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripSettings As ToolStripButton
    Friend WithEvents txtPackageName As TextBox
    Friend WithEvents ToolStrip2 As ToolStrip
    Friend WithEvents lblAllFilesStatus As ToolStripTextBox
    Friend WithEvents toolStatusWarning As ToolStripButton
    Friend WithEvents toolStatusOK As ToolStripButton
    Friend WithEvents toolStatusStop As ToolStripButton
    Friend WithEvents ToolStrip1 As ToolStripExtensions.ToolStripExtended
    Friend WithEvents toolStripFileBrowser As ToolStripButton
    Friend WithEvents flowNewsPanel As FlowLayoutPanel
    Friend WithEvents RetrieveNewsList As Timer
    Friend WithEvents chkNewsRetrieval As CheckBox
End Class
