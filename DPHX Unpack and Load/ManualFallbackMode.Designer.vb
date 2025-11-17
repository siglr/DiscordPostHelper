<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ManualFallbackMode
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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnSelectPLN = New System.Windows.Forms.Button()
        Me.btnSelectWPR = New System.Windows.Forms.Button()
        Me.cboWhitelistPresets = New System.Windows.Forms.ComboBox()
        Me.btnCopyGoFly = New System.Windows.Forms.Button()
        Me.btnClearFiles = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.txtTrackerGroupName = New System.Windows.Forms.TextBox()
        Me.lblPLNFile = New System.Windows.Forms.Label()
        Me.lblPLNTitle = New System.Windows.Forms.Label()
        Me.grpPLN = New System.Windows.Forms.GroupBox()
        Me.grpWeather = New System.Windows.Forms.GroupBox()
        Me.lblWPRName = New System.Windows.Forms.Label()
        Me.lblWPRFile = New System.Windows.Forms.Label()
        Me.grpTracker = New System.Windows.Forms.GroupBox()
        Me.grpPLN.SuspendLayout()
        Me.grpWeather.SuspendLayout()
        Me.grpTracker.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSelectPLN
        '
        Me.btnSelectPLN.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSelectPLN.Location = New System.Drawing.Point(6, 26)
        Me.btnSelectPLN.Name = "btnSelectPLN"
        Me.btnSelectPLN.Size = New System.Drawing.Size(82, 68)
        Me.btnSelectPLN.TabIndex = 0
        Me.btnSelectPLN.Text = "Select File"
        Me.ToolTip1.SetToolTip(Me.btnSelectPLN, "Click to select the flight plan (PLN) file from your computer")
        Me.btnSelectPLN.UseVisualStyleBackColor = True
        '
        'btnSelectWPR
        '
        Me.btnSelectWPR.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSelectWPR.Location = New System.Drawing.Point(6, 26)
        Me.btnSelectWPR.Name = "btnSelectWPR"
        Me.btnSelectWPR.Size = New System.Drawing.Size(82, 87)
        Me.btnSelectWPR.TabIndex = 0
        Me.btnSelectWPR.Text = "Select File"
        Me.ToolTip1.SetToolTip(Me.btnSelectWPR, "Click to select the weather (WPR) file from your computer")
        Me.btnSelectWPR.UseVisualStyleBackColor = True
        '
        'cboWhitelistPresets
        '
        Me.cboWhitelistPresets.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboWhitelistPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboWhitelistPresets.FormattingEnabled = True
        Me.cboWhitelistPresets.Location = New System.Drawing.Point(94, 28)
        Me.cboWhitelistPresets.Name = "cboWhitelistPresets"
        Me.cboWhitelistPresets.Size = New System.Drawing.Size(358, 28)
        Me.cboWhitelistPresets.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cboWhitelistPresets, "Select a preset from the whitelist folder")
        '
        'btnCopyGoFly
        '
        Me.btnCopyGoFly.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCopyGoFly.Location = New System.Drawing.Point(12, 315)
        Me.btnCopyGoFly.Name = "btnCopyGoFly"
        Me.btnCopyGoFly.Size = New System.Drawing.Size(458, 33)
        Me.btnCopyGoFly.TabIndex = 3
        Me.btnCopyGoFly.Text = "Copy / Send and Go Fly !"
        Me.ToolTip1.SetToolTip(Me.btnCopyGoFly, "Click to copy the selected files to the proper folders and tools")
        Me.btnCopyGoFly.UseVisualStyleBackColor = True
        '
        'btnClearFiles
        '
        Me.btnClearFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearFiles.Location = New System.Drawing.Point(12, 354)
        Me.btnClearFiles.Name = "btnClearFiles"
        Me.btnClearFiles.Size = New System.Drawing.Size(458, 33)
        Me.btnClearFiles.TabIndex = 4
        Me.btnClearFiles.Text = "Cleanup from folders"
        Me.ToolTip1.SetToolTip(Me.btnClearFiles, "Click to cleanup copied files")
        Me.btnClearFiles.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(12, 393)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(458, 33)
        Me.btnClose.TabIndex = 5
        Me.btnClose.Text = "Close"
        Me.ToolTip1.SetToolTip(Me.btnClose, "Click to close this window and go back to regular DPHX operations")
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'txtTrackerGroupName
        '
        Me.txtTrackerGroupName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTrackerGroupName.Location = New System.Drawing.Point(6, 26)
        Me.txtTrackerGroupName.Name = "txtTrackerGroupName"
        Me.txtTrackerGroupName.Size = New System.Drawing.Size(446, 27)
        Me.txtTrackerGroupName.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.txtTrackerGroupName, "Enter the SSC Tracker group name (if any)")
        '
        'lblPLNFile
        '
        Me.lblPLNFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPLNFile.Location = New System.Drawing.Point(94, 36)
        Me.lblPLNFile.Name = "lblPLNFile"
        Me.lblPLNFile.Size = New System.Drawing.Size(358, 25)
        Me.lblPLNFile.TabIndex = 1
        Me.lblPLNFile.Text = "Flight plan file"
        '
        'lblPLNTitle
        '
        Me.lblPLNTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPLNTitle.Location = New System.Drawing.Point(94, 61)
        Me.lblPLNTitle.Name = "lblPLNTitle"
        Me.lblPLNTitle.Size = New System.Drawing.Size(358, 25)
        Me.lblPLNTitle.TabIndex = 2
        Me.lblPLNTitle.Text = "Flight plan title"
        '
        'grpPLN
        '
        Me.grpPLN.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpPLN.Controls.Add(Me.btnSelectPLN)
        Me.grpPLN.Controls.Add(Me.lblPLNTitle)
        Me.grpPLN.Controls.Add(Me.lblPLNFile)
        Me.grpPLN.Location = New System.Drawing.Point(12, 12)
        Me.grpPLN.Name = "grpPLN"
        Me.grpPLN.Size = New System.Drawing.Size(458, 100)
        Me.grpPLN.TabIndex = 0
        Me.grpPLN.TabStop = False
        Me.grpPLN.Text = "Flight Plan"
        '
        'grpWeather
        '
        Me.grpWeather.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpWeather.Controls.Add(Me.cboWhitelistPresets)
        Me.grpWeather.Controls.Add(Me.btnSelectWPR)
        Me.grpWeather.Controls.Add(Me.lblWPRName)
        Me.grpWeather.Controls.Add(Me.lblWPRFile)
        Me.grpWeather.Location = New System.Drawing.Point(12, 118)
        Me.grpWeather.Name = "grpWeather"
        Me.grpWeather.Size = New System.Drawing.Size(458, 119)
        Me.grpWeather.TabIndex = 1
        Me.grpWeather.TabStop = False
        Me.grpWeather.Text = "Weather Preset"
        '
        'lblWPRName
        '
        Me.lblWPRName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWPRName.Location = New System.Drawing.Point(94, 88)
        Me.lblWPRName.Name = "lblWPRName"
        Me.lblWPRName.Size = New System.Drawing.Size(358, 25)
        Me.lblWPRName.TabIndex = 3
        Me.lblWPRName.Text = "Weather preset name"
        '
        'lblWPRFile
        '
        Me.lblWPRFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWPRFile.Location = New System.Drawing.Point(94, 63)
        Me.lblWPRFile.Name = "lblWPRFile"
        Me.lblWPRFile.Size = New System.Drawing.Size(358, 25)
        Me.lblWPRFile.TabIndex = 2
        Me.lblWPRFile.Text = "Weather preset file"
        '
        'grpTracker
        '
        Me.grpTracker.Controls.Add(Me.txtTrackerGroupName)
        Me.grpTracker.Location = New System.Drawing.Point(12, 243)
        Me.grpTracker.Name = "grpTracker"
        Me.grpTracker.Size = New System.Drawing.Size(458, 64)
        Me.grpTracker.TabIndex = 2
        Me.grpTracker.TabStop = False
        Me.grpTracker.Text = "Tracker Group"
        '
        'ManualFallbackMode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(482, 437)
        Me.Controls.Add(Me.grpTracker)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnClearFiles)
        Me.Controls.Add(Me.btnCopyGoFly)
        Me.Controls.Add(Me.grpWeather)
        Me.Controls.Add(Me.grpPLN)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(5000, 481)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(500, 481)
        Me.Name = "ManualFallbackMode"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Manual Fallback Mode - File Selection"
        Me.grpPLN.ResumeLayout(False)
        Me.grpWeather.ResumeLayout(False)
        Me.grpTracker.ResumeLayout(False)
        Me.grpTracker.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents btnSelectPLN As Button
    Friend WithEvents lblPLNFile As Label
    Friend WithEvents lblPLNTitle As Label
    Friend WithEvents grpPLN As GroupBox
    Friend WithEvents grpWeather As GroupBox
    Friend WithEvents btnSelectWPR As Button
    Friend WithEvents lblWPRName As Label
    Friend WithEvents lblWPRFile As Label
    Friend WithEvents cboWhitelistPresets As ComboBox
    Friend WithEvents btnCopyGoFly As Button
    Friend WithEvents btnClearFiles As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents grpTracker As GroupBox
    Friend WithEvents txtTrackerGroupName As TextBox
End Class
