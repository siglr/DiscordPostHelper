<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class IGCFileUpload
    Inherits ZoomForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IGCFileUpload))
        Me.browser = New CefSharp.WinForms.ChromiumWebBrowser()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.txtIGCEntrySeqID = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnMoveIGCToProcessed = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.pnlResults = New System.Windows.Forms.Panel()
        Me.txtTaskLocalDateTime = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.btnUpload = New System.Windows.Forms.Button()
        Me.btnRecalculate = New System.Windows.Forms.Button()
        Me.txtDistance = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtTime = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtSpeed = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtLocalDateTime = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtFlags = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtWSGStatus = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtRecordDate = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtSim = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtGlider = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCompID = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtPilot = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstbxIGCFiles = New System.Windows.Forms.ListBox()
        Me.tabIGCTabs = New System.Windows.Forms.TabControl()
        Me.tabpgResults = New System.Windows.Forms.TabPage()
        Me.lblProcessing = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tabpgRatings = New System.Windows.Forms.TabPage()
        Me.lblTaskIDAndTitle = New System.Windows.Forms.Label()
        Me.grpTaskUserData = New System.Windows.Forms.GroupBox()
        Me.txtTaskPrivateNotes = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.txtTaskPublicFeedback = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.cboQuality = New System.Windows.Forms.ComboBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.lblFavoritesDateTime = New System.Windows.Forms.Label()
        Me.lblFlyNextDateTime = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.cboDifficulty = New System.Windows.Forms.ComboBox()
        Me.chkFavorites = New System.Windows.Forms.CheckBox()
        Me.chkFlyNext = New System.Windows.Forms.CheckBox()
        Me.grpIGCUserComment = New System.Windows.Forms.GroupBox()
        Me.txtUserIGCComment = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlResults.SuspendLayout()
        Me.tabIGCTabs.SuspendLayout()
        Me.tabpgResults.SuspendLayout()
        Me.tabpgRatings.SuspendLayout()
        Me.grpTaskUserData.SuspendLayout()
        Me.grpIGCUserComment.SuspendLayout()
        Me.SuspendLayout()
        '
        'browser
        '
        Me.browser.ActivateBrowserOnCreation = False
        Me.browser.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.browser.Location = New System.Drawing.Point(0, 0)
        Me.browser.Name = "browser"
        Me.browser.Size = New System.Drawing.Size(716, 636)
        Me.browser.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.browser, "You can browse this page like a normal web page. Use CTRL and mouse wheel to set " &
        "zoom level.")
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer1.Location = New System.Drawing.Point(13, 14)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtIGCEntrySeqID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label8)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnMoveIGCToProcessed)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnDelete)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnClose)
        Me.SplitContainer1.Panel1.Controls.Add(Me.pnlResults)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtWSGStatus)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label7)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtRecordDate)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label6)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtSim)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label5)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtGlider)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCompID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtPilot)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lstbxIGCFiles)
        Me.SplitContainer1.Panel1MinSize = 250
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.tabIGCTabs)
        Me.SplitContainer1.Size = New System.Drawing.Size(980, 696)
        Me.SplitContainer1.SplitterDistance = 250
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 0
        '
        'txtIGCEntrySeqID
        '
        Me.txtIGCEntrySeqID.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIGCEntrySeqID.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtIGCEntrySeqID.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtIGCEntrySeqID.Location = New System.Drawing.Point(99, 304)
        Me.txtIGCEntrySeqID.Name = "txtIGCEntrySeqID"
        Me.txtIGCEntrySeqID.ReadOnly = True
        Me.txtIGCEntrySeqID.Size = New System.Drawing.Size(144, 20)
        Me.txtIGCEntrySeqID.TabIndex = 12
        Me.ToolTip1.SetToolTip(Me.txtIGCEntrySeqID, "The task ID from WeSimGlide.org")
        '
        'Label8
        '
        Me.Label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(3, 304)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(49, 20)
        Me.Label8.TabIndex = 11
        Me.Label8.Text = "Task #"
        '
        'btnMoveIGCToProcessed
        '
        Me.btnMoveIGCToProcessed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnMoveIGCToProcessed.Location = New System.Drawing.Point(4, 582)
        Me.btnMoveIGCToProcessed.Name = "btnMoveIGCToProcessed"
        Me.btnMoveIGCToProcessed.Size = New System.Drawing.Size(239, 32)
        Me.btnMoveIGCToProcessed.TabIndex = 16
        Me.btnMoveIGCToProcessed.Text = "Move to Processed"
        Me.ToolTip1.SetToolTip(Me.btnMoveIGCToProcessed, "Click to move this IGC file to the ""Processed"" folder.")
        Me.btnMoveIGCToProcessed.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDelete.Location = New System.Drawing.Point(4, 620)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(239, 32)
        Me.btnDelete.TabIndex = 17
        Me.btnDelete.Text = "☠️ Delete IGC File ☠️"
        Me.ToolTip1.SetToolTip(Me.btnDelete, "Click to delete this IGC file - WARNING: the deletion is permanent!")
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(4, 659)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(239, 32)
        Me.btnClose.TabIndex = 18
        Me.btnClose.Text = "Close"
        Me.ToolTip1.SetToolTip(Me.btnClose, "Click to close this form.")
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'pnlResults
        '
        Me.pnlResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlResults.Controls.Add(Me.txtTaskLocalDateTime)
        Me.pnlResults.Controls.Add(Me.Label12)
        Me.pnlResults.Controls.Add(Me.btnUpload)
        Me.pnlResults.Controls.Add(Me.btnRecalculate)
        Me.pnlResults.Controls.Add(Me.txtDistance)
        Me.pnlResults.Controls.Add(Me.Label15)
        Me.pnlResults.Controls.Add(Me.txtTime)
        Me.pnlResults.Controls.Add(Me.Label14)
        Me.pnlResults.Controls.Add(Me.txtSpeed)
        Me.pnlResults.Controls.Add(Me.Label13)
        Me.pnlResults.Controls.Add(Me.txtLocalDateTime)
        Me.pnlResults.Controls.Add(Me.Label11)
        Me.pnlResults.Controls.Add(Me.Label10)
        Me.pnlResults.Controls.Add(Me.txtFlags)
        Me.pnlResults.Controls.Add(Me.Label9)
        Me.pnlResults.Location = New System.Drawing.Point(0, 358)
        Me.pnlResults.Name = "pnlResults"
        Me.pnlResults.Size = New System.Drawing.Size(249, 221)
        Me.pnlResults.TabIndex = 15
        Me.pnlResults.Visible = False
        '
        'txtTaskLocalDateTime
        '
        Me.txtTaskLocalDateTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTaskLocalDateTime.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtTaskLocalDateTime.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtTaskLocalDateTime.Location = New System.Drawing.Point(99, 77)
        Me.txtTaskLocalDateTime.Name = "txtTaskLocalDateTime"
        Me.txtTaskLocalDateTime.ReadOnly = True
        Me.txtTaskLocalDateTime.Size = New System.Drawing.Size(144, 20)
        Me.txtTaskLocalDateTime.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.txtTaskLocalDateTime, "Local date and time specified by the task.")
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(3, 77)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(47, 20)
        Me.Label12.TabIndex = 6
        Me.Label12.Text = " (task)"
        '
        'btnUpload
        '
        Me.btnUpload.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUpload.Location = New System.Drawing.Point(4, 183)
        Me.btnUpload.Name = "btnUpload"
        Me.btnUpload.Size = New System.Drawing.Size(239, 32)
        Me.btnUpload.TabIndex = 14
        Me.btnUpload.Text = "Upload to WSG"
        Me.ToolTip1.SetToolTip(Me.btnUpload, "Click to upload this IGC file to WeSimGlide.org")
        Me.btnUpload.UseVisualStyleBackColor = True
        '
        'btnRecalculate
        '
        Me.btnRecalculate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRecalculate.BackgroundImage = CType(resources.GetObject("btnRecalculate.BackgroundImage"), System.Drawing.Image)
        Me.btnRecalculate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnRecalculate.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.163636!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRecalculate.Location = New System.Drawing.Point(206, 0)
        Me.btnRecalculate.Name = "btnRecalculate"
        Me.btnRecalculate.Size = New System.Drawing.Size(37, 37)
        Me.btnRecalculate.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.btnRecalculate, "Click to re-extract the results from this IGC file.")
        Me.btnRecalculate.UseVisualStyleBackColor = True
        '
        'txtDistance
        '
        Me.txtDistance.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDistance.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtDistance.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtDistance.Location = New System.Drawing.Point(99, 155)
        Me.txtDistance.Name = "txtDistance"
        Me.txtDistance.ReadOnly = True
        Me.txtDistance.Size = New System.Drawing.Size(144, 20)
        Me.txtDistance.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.txtDistance, "Calculated distance achieved extracted from the results.")
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(3, 155)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(65, 20)
        Me.Label15.TabIndex = 12
        Me.Label15.Text = "Distance"
        '
        'txtTime
        '
        Me.txtTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTime.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtTime.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtTime.Location = New System.Drawing.Point(99, 129)
        Me.txtTime.Name = "txtTime"
        Me.txtTime.ReadOnly = True
        Me.txtTime.Size = New System.Drawing.Size(144, 20)
        Me.txtTime.TabIndex = 11
        Me.ToolTip1.SetToolTip(Me.txtTime, "Calculated time extracted from the results.")
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(3, 129)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(41, 20)
        Me.Label14.TabIndex = 10
        Me.Label14.Text = "Time"
        '
        'txtSpeed
        '
        Me.txtSpeed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSpeed.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtSpeed.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtSpeed.Location = New System.Drawing.Point(99, 103)
        Me.txtSpeed.Name = "txtSpeed"
        Me.txtSpeed.ReadOnly = True
        Me.txtSpeed.Size = New System.Drawing.Size(144, 20)
        Me.txtSpeed.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.txtSpeed, "Calculated speed extracted from the results.")
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(3, 103)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(51, 20)
        Me.Label13.TabIndex = 8
        Me.Label13.Text = "Speed"
        '
        'txtLocalDateTime
        '
        Me.txtLocalDateTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLocalDateTime.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtLocalDateTime.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtLocalDateTime.Location = New System.Drawing.Point(99, 51)
        Me.txtLocalDateTime.Name = "txtLocalDateTime"
        Me.txtLocalDateTime.ReadOnly = True
        Me.txtLocalDateTime.Size = New System.Drawing.Size(144, 20)
        Me.txtLocalDateTime.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.txtLocalDateTime, "Local date and time extracted from the results in the IGC file.")
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(3, 51)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(76, 20)
        Me.Label11.TabIndex = 4
        Me.Label11.Text = "Local time"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.Label10.Location = New System.Drawing.Point(3, 3)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(60, 20)
        Me.Label10.TabIndex = 0
        Me.Label10.Text = "Results"
        '
        'txtFlags
        '
        Me.txtFlags.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFlags.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtFlags.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtFlags.Location = New System.Drawing.Point(99, 25)
        Me.txtFlags.Name = "txtFlags"
        Me.txtFlags.ReadOnly = True
        Me.txtFlags.Size = New System.Drawing.Size(144, 20)
        Me.txtFlags.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtFlags, "All flags for the results in this IGC file.")
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(3, 25)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(42, 20)
        Me.Label9.TabIndex = 2
        Me.Label9.Text = "Flags"
        '
        'txtWSGStatus
        '
        Me.txtWSGStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWSGStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtWSGStatus.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtWSGStatus.Location = New System.Drawing.Point(99, 330)
        Me.txtWSGStatus.Name = "txtWSGStatus"
        Me.txtWSGStatus.ReadOnly = True
        Me.txtWSGStatus.Size = New System.Drawing.Size(144, 20)
        Me.txtWSGStatus.TabIndex = 14
        Me.ToolTip1.SetToolTip(Me.txtWSGStatus, "Status of this IGC file on WeSimGlide.org")
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(3, 330)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(86, 20)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "WSG Status"
        '
        'txtRecordDate
        '
        Me.txtRecordDate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRecordDate.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtRecordDate.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtRecordDate.Location = New System.Drawing.Point(99, 278)
        Me.txtRecordDate.Name = "txtRecordDate"
        Me.txtRecordDate.ReadOnly = True
        Me.txtRecordDate.Size = New System.Drawing.Size(144, 20)
        Me.txtRecordDate.TabIndex = 10
        Me.ToolTip1.SetToolTip(Me.txtRecordDate, "Date and time the IGC file was recorded.")
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(3, 278)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(90, 20)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Record date"
        '
        'txtSim
        '
        Me.txtSim.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSim.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtSim.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtSim.Location = New System.Drawing.Point(99, 252)
        Me.txtSim.Name = "txtSim"
        Me.txtSim.ReadOnly = True
        Me.txtSim.Size = New System.Drawing.Size(144, 20)
        Me.txtSim.TabIndex = 8
        Me.ToolTip1.SetToolTip(Me.txtSim, "Sim version extracted from IGC file")
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 252)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(33, 20)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Sim"
        '
        'txtGlider
        '
        Me.txtGlider.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtGlider.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtGlider.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtGlider.Location = New System.Drawing.Point(99, 226)
        Me.txtGlider.Name = "txtGlider"
        Me.txtGlider.ReadOnly = True
        Me.txtGlider.Size = New System.Drawing.Size(144, 20)
        Me.txtGlider.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.txtGlider, "Glider type extracted from IGC file")
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 226)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(48, 20)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Glider"
        '
        'txtCompID
        '
        Me.txtCompID.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCompID.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtCompID.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtCompID.Location = New System.Drawing.Point(99, 200)
        Me.txtCompID.Name = "txtCompID"
        Me.txtCompID.ReadOnly = True
        Me.txtCompID.Size = New System.Drawing.Size(144, 20)
        Me.txtCompID.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.txtCompID, "Competition ID extracted from IGC file")
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 200)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(72, 20)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Comp. ID"
        '
        'txtPilot
        '
        Me.txtPilot.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPilot.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtPilot.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtPilot.Location = New System.Drawing.Point(99, 174)
        Me.txtPilot.Name = "txtPilot"
        Me.txtPilot.ReadOnly = True
        Me.txtPilot.Size = New System.Drawing.Size(144, 20)
        Me.txtPilot.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtPilot, "Pilot name extracted from IGC file")
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 174)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(37, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Pilot"
        '
        'lstbxIGCFiles
        '
        Me.lstbxIGCFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstbxIGCFiles.FormattingEnabled = True
        Me.lstbxIGCFiles.IntegralHeight = False
        Me.lstbxIGCFiles.ItemHeight = 20
        Me.lstbxIGCFiles.Location = New System.Drawing.Point(4, 5)
        Me.lstbxIGCFiles.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lstbxIGCFiles.Name = "lstbxIGCFiles"
        Me.lstbxIGCFiles.Size = New System.Drawing.Size(239, 161)
        Me.lstbxIGCFiles.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.lstbxIGCFiles, "Select an IGC file from the list.")
        '
        'tabIGCTabs
        '
        Me.tabIGCTabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabIGCTabs.Controls.Add(Me.tabpgResults)
        Me.tabIGCTabs.Controls.Add(Me.tabpgRatings)
        Me.tabIGCTabs.Location = New System.Drawing.Point(0, 0)
        Me.tabIGCTabs.Name = "tabIGCTabs"
        Me.tabIGCTabs.SelectedIndex = 0
        Me.tabIGCTabs.Size = New System.Drawing.Size(706, 695)
        Me.tabIGCTabs.TabIndex = 2
        '
        'tabpgResults
        '
        Me.tabpgResults.Controls.Add(Me.lblProcessing)
        Me.tabpgResults.Controls.Add(Me.Label4)
        Me.tabpgResults.Controls.Add(Me.browser)
        Me.tabpgResults.Location = New System.Drawing.Point(4, 29)
        Me.tabpgResults.Name = "tabpgResults"
        Me.tabpgResults.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpgResults.Size = New System.Drawing.Size(698, 662)
        Me.tabpgResults.TabIndex = 0
        Me.tabpgResults.Text = "Results"
        Me.tabpgResults.UseVisualStyleBackColor = True
        '
        'lblProcessing
        '
        Me.lblProcessing.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblProcessing.Font = New System.Drawing.Font("Segoe UI Variable Display", 20.29091!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProcessing.Location = New System.Drawing.Point(0, 0)
        Me.lblProcessing.Name = "lblProcessing"
        Me.lblProcessing.Size = New System.Drawing.Size(716, 662)
        Me.lblProcessing.TabIndex = 0
        Me.lblProcessing.Text = "Processing"
        Me.lblProcessing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.Location = New System.Drawing.Point(3, 639)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(710, 20)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "You can browse this page like a normal web page. Use CTRL and mouse wheel to set " &
    "zoom level."
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'tabpgRatings
        '
        Me.tabpgRatings.Controls.Add(Me.lblTaskIDAndTitle)
        Me.tabpgRatings.Controls.Add(Me.grpTaskUserData)
        Me.tabpgRatings.Controls.Add(Me.grpIGCUserComment)
        Me.tabpgRatings.Location = New System.Drawing.Point(4, 29)
        Me.tabpgRatings.Name = "tabpgRatings"
        Me.tabpgRatings.Padding = New System.Windows.Forms.Padding(3)
        Me.tabpgRatings.Size = New System.Drawing.Size(698, 662)
        Me.tabpgRatings.TabIndex = 1
        Me.tabpgRatings.Text = "Ratings & Comments"
        Me.tabpgRatings.UseVisualStyleBackColor = True
        '
        'lblTaskIDAndTitle
        '
        Me.lblTaskIDAndTitle.AutoSize = True
        Me.lblTaskIDAndTitle.Cursor = System.Windows.Forms.Cursors.Hand
        Me.lblTaskIDAndTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTaskIDAndTitle.Location = New System.Drawing.Point(3, 3)
        Me.lblTaskIDAndTitle.Name = "lblTaskIDAndTitle"
        Me.lblTaskIDAndTitle.Size = New System.Drawing.Size(16, 26)
        Me.lblTaskIDAndTitle.TabIndex = 2
        Me.lblTaskIDAndTitle.Text = " "
        Me.ToolTip1.SetToolTip(Me.lblTaskIDAndTitle, "Click to open this task on WeSimGlide.org")
        '
        'grpTaskUserData
        '
        Me.grpTaskUserData.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpTaskUserData.Controls.Add(Me.txtTaskPrivateNotes)
        Me.grpTaskUserData.Controls.Add(Me.Label19)
        Me.grpTaskUserData.Controls.Add(Me.txtTaskPublicFeedback)
        Me.grpTaskUserData.Controls.Add(Me.Label18)
        Me.grpTaskUserData.Controls.Add(Me.cboQuality)
        Me.grpTaskUserData.Controls.Add(Me.Label17)
        Me.grpTaskUserData.Controls.Add(Me.lblFavoritesDateTime)
        Me.grpTaskUserData.Controls.Add(Me.lblFlyNextDateTime)
        Me.grpTaskUserData.Controls.Add(Me.Label16)
        Me.grpTaskUserData.Controls.Add(Me.cboDifficulty)
        Me.grpTaskUserData.Controls.Add(Me.chkFavorites)
        Me.grpTaskUserData.Controls.Add(Me.chkFlyNext)
        Me.grpTaskUserData.Location = New System.Drawing.Point(6, 109)
        Me.grpTaskUserData.Name = "grpTaskUserData"
        Me.grpTaskUserData.Size = New System.Drawing.Size(686, 402)
        Me.grpTaskUserData.TabIndex = 1
        Me.grpTaskUserData.TabStop = False
        Me.grpTaskUserData.Text = "My Stuff for this task"
        '
        'txtTaskPrivateNotes
        '
        Me.txtTaskPrivateNotes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTaskPrivateNotes.Location = New System.Drawing.Point(6, 300)
        Me.txtTaskPrivateNotes.Multiline = True
        Me.txtTaskPrivateNotes.Name = "txtTaskPrivateNotes"
        Me.txtTaskPrivateNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtTaskPrivateNotes.Size = New System.Drawing.Size(674, 91)
        Me.txtTaskPrivateNotes.TabIndex = 11
        Me.ToolTip1.SetToolTip(Me.txtTaskPrivateNotes, "You can save a private note about this task")
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(3, 277)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(96, 20)
        Me.Label19.TabIndex = 10
        Me.Label19.Text = "Private Notes"
        '
        'txtTaskPublicFeedback
        '
        Me.txtTaskPublicFeedback.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTaskPublicFeedback.Location = New System.Drawing.Point(6, 181)
        Me.txtTaskPublicFeedback.Multiline = True
        Me.txtTaskPublicFeedback.Name = "txtTaskPublicFeedback"
        Me.txtTaskPublicFeedback.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtTaskPublicFeedback.Size = New System.Drawing.Size(674, 91)
        Me.txtTaskPublicFeedback.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.txtTaskPublicFeedback, "Provide public feedback for this task")
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(3, 158)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(115, 20)
        Me.Label18.TabIndex = 8
        Me.Label18.Text = "Public Feedback"
        '
        'cboQuality
        '
        Me.cboQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboQuality.FormattingEnabled = True
        Me.cboQuality.Items.AddRange(New Object() {"Not set", "★", "★★", "★★★", "★★★★", "★★★★★"})
        Me.cboQuality.Location = New System.Drawing.Point(82, 117)
        Me.cboQuality.Name = "cboQuality"
        Me.cboQuality.Size = New System.Drawing.Size(108, 28)
        Me.cboQuality.TabIndex = 7
        Me.ToolTip1.SetToolTip(Me.cboQuality, "Rate the quality and fun factor of this task")
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(2, 120)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(57, 20)
        Me.Label17.TabIndex = 6
        Me.Label17.Text = "Quality:"
        '
        'lblFavoritesDateTime
        '
        Me.lblFavoritesDateTime.AutoSize = True
        Me.lblFavoritesDateTime.Location = New System.Drawing.Point(119, 57)
        Me.lblFavoritesDateTime.Name = "lblFavoritesDateTime"
        Me.lblFavoritesDateTime.Size = New System.Drawing.Size(13, 20)
        Me.lblFavoritesDateTime.TabIndex = 3
        Me.lblFavoritesDateTime.Text = " "
        '
        'lblFlyNextDateTime
        '
        Me.lblFlyNextDateTime.AutoSize = True
        Me.lblFlyNextDateTime.Location = New System.Drawing.Point(119, 27)
        Me.lblFlyNextDateTime.Name = "lblFlyNextDateTime"
        Me.lblFlyNextDateTime.Size = New System.Drawing.Size(13, 20)
        Me.lblFlyNextDateTime.TabIndex = 1
        Me.lblFlyNextDateTime.Text = " "
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(2, 86)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(70, 20)
        Me.Label16.TabIndex = 4
        Me.Label16.Text = "Difficulty:"
        '
        'cboDifficulty
        '
        Me.cboDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDifficulty.FormattingEnabled = True
        Me.cboDifficulty.Items.AddRange(New Object() {"0. Not set", "1. Beginner", "2. Student", "3. Experienced", "4. Professional", "5. Champion"})
        Me.cboDifficulty.Location = New System.Drawing.Point(82, 83)
        Me.cboDifficulty.Name = "cboDifficulty"
        Me.cboDifficulty.Size = New System.Drawing.Size(125, 28)
        Me.cboDifficulty.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.cboDifficulty, "Rate the difficulty level of this task")
        '
        'chkFavorites
        '
        Me.chkFavorites.AutoSize = True
        Me.chkFavorites.Location = New System.Drawing.Point(7, 56)
        Me.chkFavorites.Name = "chkFavorites"
        Me.chkFavorites.Size = New System.Drawing.Size(114, 24)
        Me.chkFavorites.TabIndex = 2
        Me.chkFavorites.Text = "🌟 Favorites "
        Me.ToolTip1.SetToolTip(Me.chkFavorites, "Check this box to add this task to your list of Favorites")
        Me.chkFavorites.UseVisualStyleBackColor = True
        '
        'chkFlyNext
        '
        Me.chkFlyNext.AutoSize = True
        Me.chkFlyNext.Location = New System.Drawing.Point(7, 26)
        Me.chkFlyNext.Name = "chkFlyNext"
        Me.chkFlyNext.Size = New System.Drawing.Size(106, 24)
        Me.chkFlyNext.TabIndex = 0
        Me.chkFlyNext.Text = "🔜 Fly Next"
        Me.ToolTip1.SetToolTip(Me.chkFlyNext, "Check this box to add this task to your ""Fly Next"" list")
        Me.chkFlyNext.UseVisualStyleBackColor = True
        '
        'grpIGCUserComment
        '
        Me.grpIGCUserComment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpIGCUserComment.Controls.Add(Me.txtUserIGCComment)
        Me.grpIGCUserComment.Location = New System.Drawing.Point(6, 35)
        Me.grpIGCUserComment.Name = "grpIGCUserComment"
        Me.grpIGCUserComment.Size = New System.Drawing.Size(686, 68)
        Me.grpIGCUserComment.TabIndex = 0
        Me.grpIGCUserComment.TabStop = False
        Me.grpIGCUserComment.Text = "IGC Comment"
        '
        'txtUserIGCComment
        '
        Me.txtUserIGCComment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtUserIGCComment.Location = New System.Drawing.Point(6, 26)
        Me.txtUserIGCComment.Name = "txtUserIGCComment"
        Me.txtUserIGCComment.Size = New System.Drawing.Size(674, 27)
        Me.txtUserIGCComment.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.txtUserIGCComment, "Provide comments for this specific IGC flight")
        '
        'IGCFileUpload
        '
        Me.AcceptButton = Me.btnClose
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(1006, 724)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(1024, 768)
        Me.Name = "IGCFileUpload"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "IGC File Upload"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlResults.ResumeLayout(False)
        Me.pnlResults.PerformLayout()
        Me.tabIGCTabs.ResumeLayout(False)
        Me.tabpgResults.ResumeLayout(False)
        Me.tabpgRatings.ResumeLayout(False)
        Me.tabpgRatings.PerformLayout()
        Me.grpTaskUserData.ResumeLayout(False)
        Me.grpTaskUserData.PerformLayout()
        Me.grpIGCUserComment.ResumeLayout(False)
        Me.grpIGCUserComment.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents browser As CefSharp.WinForms.ChromiumWebBrowser
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents lstbxIGCFiles As ListBox
    Friend WithEvents txtPilot As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txtCompID As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtGlider As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtSim As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtRecordDate As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtWSGStatus As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents lblProcessing As Label
    Friend WithEvents txtFlags As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents pnlResults As Panel
    Friend WithEvents Label10 As Label
    Friend WithEvents txtLocalDateTime As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents txtSpeed As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtDistance As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents txtTime As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents btnRecalculate As Button
    Friend WithEvents btnUpload As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents btnMoveIGCToProcessed As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents txtIGCEntrySeqID As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtTaskLocalDateTime As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents tabIGCTabs As TabControl
    Friend WithEvents tabpgResults As TabPage
    Friend WithEvents tabpgRatings As TabPage
    Friend WithEvents txtUserIGCComment As TextBox
    Friend WithEvents grpIGCUserComment As GroupBox
    Friend WithEvents grpTaskUserData As GroupBox
    Friend WithEvents chkFlyNext As CheckBox
    Friend WithEvents Label16 As Label
    Friend WithEvents cboDifficulty As ComboBox
    Friend WithEvents chkFavorites As CheckBox
    Friend WithEvents lblFavoritesDateTime As Label
    Friend WithEvents lblFlyNextDateTime As Label
    Friend WithEvents cboQuality As ComboBox
    Friend WithEvents Label17 As Label
    Friend WithEvents txtTaskPrivateNotes As TextBox
    Friend WithEvents Label19 As Label
    Friend WithEvents txtTaskPublicFeedback As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents lblTaskIDAndTitle As Label
End Class
