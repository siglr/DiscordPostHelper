<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class IGCFileUpload
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IGCFileUpload))
        Me.browser = New CefSharp.WinForms.ChromiumWebBrowser()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.pnlResults = New System.Windows.Forms.Panel()
        Me.txtFlags = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtTaskPlannerStatus = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtWSGStatus = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtRecordDate = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtSim = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtNB21 = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtGlider = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCompID = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtPilot = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstbxIGCFiles = New System.Windows.Forms.ListBox()
        Me.lblProcessing = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtLocalDateTime = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtTaskLocalDateTime = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtSpeed = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtTime = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtDistance = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtTPVersion = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btnRecalculate = New System.Windows.Forms.Button()
        Me.btnUpload = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlResults.SuspendLayout()
        Me.SuspendLayout()
        '
        'browser
        '
        Me.browser.ActivateBrowserOnCreation = False
        Me.browser.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.browser.Location = New System.Drawing.Point(3, 3)
        Me.browser.Name = "browser"
        Me.browser.Size = New System.Drawing.Size(734, 688)
        Me.browser.TabIndex = 0
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnClose)
        Me.SplitContainer1.Panel1.Controls.Add(Me.pnlResults)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtTaskPlannerStatus)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label8)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtWSGStatus)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label7)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtRecordDate)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label6)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtSim)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label5)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtNB21)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label4)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtGlider)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label3)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtCompID)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.txtPilot)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1.Controls.Add(Me.lstbxIGCFiles)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblProcessing)
        Me.SplitContainer1.Panel2.Controls.Add(Me.browser)
        Me.SplitContainer1.Size = New System.Drawing.Size(980, 696)
        Me.SplitContainer1.SplitterDistance = 228
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 1
        '
        'pnlResults
        '
        Me.pnlResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlResults.Controls.Add(Me.btnUpload)
        Me.pnlResults.Controls.Add(Me.btnRecalculate)
        Me.pnlResults.Controls.Add(Me.txtTPVersion)
        Me.pnlResults.Controls.Add(Me.Label16)
        Me.pnlResults.Controls.Add(Me.txtDistance)
        Me.pnlResults.Controls.Add(Me.Label15)
        Me.pnlResults.Controls.Add(Me.txtTime)
        Me.pnlResults.Controls.Add(Me.Label14)
        Me.pnlResults.Controls.Add(Me.txtSpeed)
        Me.pnlResults.Controls.Add(Me.Label13)
        Me.pnlResults.Controls.Add(Me.txtTaskLocalDateTime)
        Me.pnlResults.Controls.Add(Me.Label12)
        Me.pnlResults.Controls.Add(Me.txtLocalDateTime)
        Me.pnlResults.Controls.Add(Me.Label11)
        Me.pnlResults.Controls.Add(Me.Label10)
        Me.pnlResults.Controls.Add(Me.txtFlags)
        Me.pnlResults.Controls.Add(Me.Label9)
        Me.pnlResults.Location = New System.Drawing.Point(0, 359)
        Me.pnlResults.Name = "pnlResults"
        Me.pnlResults.Size = New System.Drawing.Size(227, 282)
        Me.pnlResults.TabIndex = 19
        Me.pnlResults.Visible = False
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
        Me.txtFlags.Size = New System.Drawing.Size(122, 20)
        Me.txtFlags.TabIndex = 18
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(3, 25)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(42, 20)
        Me.Label9.TabIndex = 17
        Me.Label9.Text = "Flags"
        '
        'txtTaskPlannerStatus
        '
        Me.txtTaskPlannerStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTaskPlannerStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtTaskPlannerStatus.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtTaskPlannerStatus.Location = New System.Drawing.Point(99, 336)
        Me.txtTaskPlannerStatus.Name = "txtTaskPlannerStatus"
        Me.txtTaskPlannerStatus.ReadOnly = True
        Me.txtTaskPlannerStatus.Size = New System.Drawing.Size(122, 20)
        Me.txtTaskPlannerStatus.TabIndex = 16
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(3, 336)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(69, 20)
        Me.Label8.TabIndex = 15
        Me.Label8.Text = "TP Status"
        '
        'txtWSGStatus
        '
        Me.txtWSGStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtWSGStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtWSGStatus.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtWSGStatus.Location = New System.Drawing.Point(99, 310)
        Me.txtWSGStatus.Name = "txtWSGStatus"
        Me.txtWSGStatus.ReadOnly = True
        Me.txtWSGStatus.Size = New System.Drawing.Size(122, 20)
        Me.txtWSGStatus.TabIndex = 14
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(3, 310)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(86, 20)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "WSG Status"
        '
        'txtRecordDate
        '
        Me.txtRecordDate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRecordDate.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtRecordDate.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtRecordDate.Location = New System.Drawing.Point(99, 284)
        Me.txtRecordDate.Name = "txtRecordDate"
        Me.txtRecordDate.ReadOnly = True
        Me.txtRecordDate.Size = New System.Drawing.Size(122, 20)
        Me.txtRecordDate.TabIndex = 12
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(3, 284)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(90, 20)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Record date"
        '
        'txtSim
        '
        Me.txtSim.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSim.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtSim.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtSim.Location = New System.Drawing.Point(99, 258)
        Me.txtSim.Name = "txtSim"
        Me.txtSim.ReadOnly = True
        Me.txtSim.Size = New System.Drawing.Size(122, 20)
        Me.txtSim.TabIndex = 10
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 258)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(33, 20)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Sim"
        '
        'txtNB21
        '
        Me.txtNB21.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNB21.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtNB21.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtNB21.Location = New System.Drawing.Point(99, 232)
        Me.txtNB21.Name = "txtNB21"
        Me.txtNB21.ReadOnly = True
        Me.txtNB21.Size = New System.Drawing.Size(122, 20)
        Me.txtNB21.TabIndex = 8
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 232)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 20)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "NB21"
        '
        'txtGlider
        '
        Me.txtGlider.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtGlider.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtGlider.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtGlider.Location = New System.Drawing.Point(99, 206)
        Me.txtGlider.Name = "txtGlider"
        Me.txtGlider.ReadOnly = True
        Me.txtGlider.Size = New System.Drawing.Size(122, 20)
        Me.txtGlider.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 206)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(48, 20)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Glider"
        '
        'txtCompID
        '
        Me.txtCompID.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCompID.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtCompID.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtCompID.Location = New System.Drawing.Point(99, 180)
        Me.txtCompID.Name = "txtCompID"
        Me.txtCompID.ReadOnly = True
        Me.txtCompID.Size = New System.Drawing.Size(122, 20)
        Me.txtCompID.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 180)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(72, 20)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Comp. ID"
        '
        'txtPilot
        '
        Me.txtPilot.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPilot.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtPilot.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtPilot.Location = New System.Drawing.Point(99, 154)
        Me.txtPilot.Name = "txtPilot"
        Me.txtPilot.ReadOnly = True
        Me.txtPilot.Size = New System.Drawing.Size(122, 20)
        Me.txtPilot.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 154)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(37, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Pilot"
        '
        'lstbxIGCFiles
        '
        Me.lstbxIGCFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstbxIGCFiles.FormattingEnabled = True
        Me.lstbxIGCFiles.ItemHeight = 20
        Me.lstbxIGCFiles.Location = New System.Drawing.Point(4, 5)
        Me.lstbxIGCFiles.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lstbxIGCFiles.Name = "lstbxIGCFiles"
        Me.lstbxIGCFiles.Size = New System.Drawing.Size(217, 144)
        Me.lstbxIGCFiles.TabIndex = 0
        '
        'lblProcessing
        '
        Me.lblProcessing.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblProcessing.Font = New System.Drawing.Font("Segoe UI Variable Display", 20.29091!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProcessing.Location = New System.Drawing.Point(0, 0)
        Me.lblProcessing.Name = "lblProcessing"
        Me.lblProcessing.Size = New System.Drawing.Size(745, 694)
        Me.lblProcessing.TabIndex = 1
        Me.lblProcessing.Text = "Processing"
        Me.lblProcessing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.Label10.Location = New System.Drawing.Point(3, 3)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(60, 20)
        Me.Label10.TabIndex = 19
        Me.Label10.Text = "Results"
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
        Me.txtLocalDateTime.Size = New System.Drawing.Size(122, 20)
        Me.txtLocalDateTime.TabIndex = 21
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(3, 51)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(76, 20)
        Me.Label11.TabIndex = 20
        Me.Label11.Text = "Local time"
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
        Me.txtTaskLocalDateTime.Size = New System.Drawing.Size(122, 20)
        Me.txtTaskLocalDateTime.TabIndex = 23
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(3, 77)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(77, 20)
        Me.Label12.TabIndex = 22
        Me.Label12.Text = "(task local)"
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
        Me.txtSpeed.Size = New System.Drawing.Size(122, 20)
        Me.txtSpeed.TabIndex = 25
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(3, 103)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(51, 20)
        Me.Label13.TabIndex = 24
        Me.Label13.Text = "Speed"
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
        Me.txtTime.Size = New System.Drawing.Size(122, 20)
        Me.txtTime.TabIndex = 27
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(3, 129)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(41, 20)
        Me.Label14.TabIndex = 26
        Me.Label14.Text = "Time"
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
        Me.txtDistance.Size = New System.Drawing.Size(122, 20)
        Me.txtDistance.TabIndex = 29
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(3, 155)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(65, 20)
        Me.Label15.TabIndex = 28
        Me.Label15.Text = "Distance"
        '
        'txtTPVersion
        '
        Me.txtTPVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTPVersion.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtTPVersion.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtTPVersion.Location = New System.Drawing.Point(99, 181)
        Me.txtTPVersion.Name = "txtTPVersion"
        Me.txtTPVersion.ReadOnly = True
        Me.txtTPVersion.Size = New System.Drawing.Size(122, 20)
        Me.txtTPVersion.TabIndex = 31
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(3, 181)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(77, 20)
        Me.Label16.TabIndex = 30
        Me.Label16.Text = "TP Version"
        '
        'btnRecalculate
        '
        Me.btnRecalculate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRecalculate.Location = New System.Drawing.Point(7, 207)
        Me.btnRecalculate.Name = "btnRecalculate"
        Me.btnRecalculate.Size = New System.Drawing.Size(214, 32)
        Me.btnRecalculate.TabIndex = 32
        Me.btnRecalculate.Text = "Extract Again"
        Me.btnRecalculate.UseVisualStyleBackColor = True
        '
        'btnUpload
        '
        Me.btnUpload.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUpload.Location = New System.Drawing.Point(7, 245)
        Me.btnUpload.Name = "btnUpload"
        Me.btnUpload.Size = New System.Drawing.Size(214, 32)
        Me.btnUpload.TabIndex = 33
        Me.btnUpload.Text = "Upload to WSG"
        Me.btnUpload.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(7, 659)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(214, 32)
        Me.btnClose.TabIndex = 34
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
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
    Friend WithEvents txtNB21 As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtSim As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtRecordDate As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents txtWSGStatus As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents lblProcessing As Label
    Friend WithEvents txtTaskPlannerStatus As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents txtFlags As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents pnlResults As Panel
    Friend WithEvents Label10 As Label
    Friend WithEvents txtLocalDateTime As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents txtTaskLocalDateTime As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents txtSpeed As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents txtDistance As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents txtTime As TextBox
    Friend WithEvents Label14 As Label
    Friend WithEvents txtTPVersion As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents btnRecalculate As Button
    Friend WithEvents btnUpload As Button
    Friend WithEvents btnClose As Button
End Class
