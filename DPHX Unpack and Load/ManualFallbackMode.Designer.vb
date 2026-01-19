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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ManualFallbackMode))
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnSelectPLN = New System.Windows.Forms.Button()
        Me.btnCopyGoFly = New System.Windows.Forms.Button()
        Me.btnClearFiles = New System.Windows.Forms.Button()
        Me.txtTrackerGroupName = New System.Windows.Forms.TextBox()
        Me.lblPLNFile = New System.Windows.Forms.Label()
        Me.lblPLNTitle = New System.Windows.Forms.Label()
        Me.grpPLN = New System.Windows.Forms.GroupBox()
        Me.grpWeather = New System.Windows.Forms.GroupBox()
        Me.grpCustomPresets = New System.Windows.Forms.GroupBox()
        Me.grpSecondaryWeather = New System.Windows.Forms.GroupBox()
        Me.btnSecondaryBrowse = New System.Windows.Forms.Button()
        Me.lblSecondaryName = New System.Windows.Forms.Label()
        Me.lblSecondaryFile = New System.Windows.Forms.Label()
        Me.grpPrimaryWeather = New System.Windows.Forms.GroupBox()
        Me.btnPrimaryBrowse = New System.Windows.Forms.Button()
        Me.lblPrimaryName = New System.Windows.Forms.Label()
        Me.lblPrimaryFile = New System.Windows.Forms.Label()
        Me.optCustomPreset = New System.Windows.Forms.RadioButton()
        Me.optSSCPreset = New System.Windows.Forms.RadioButton()
        Me.cboSSCPresetList = New System.Windows.Forms.ComboBox()
        Me.grpTracker = New System.Windows.Forms.GroupBox()
        Me.grpPLN.SuspendLayout()
        Me.grpWeather.SuspendLayout()
        Me.grpCustomPresets.SuspendLayout()
        Me.grpSecondaryWeather.SuspendLayout()
        Me.grpPrimaryWeather.SuspendLayout()
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
        'btnCopyGoFly
        '
        Me.btnCopyGoFly.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCopyGoFly.Location = New System.Drawing.Point(12, 541)
        Me.btnCopyGoFly.Name = "btnCopyGoFly"
        Me.btnCopyGoFly.Size = New System.Drawing.Size(458, 33)
        Me.btnCopyGoFly.TabIndex = 3
        Me.btnCopyGoFly.Text = "Confirm"
        Me.ToolTip1.SetToolTip(Me.btnCopyGoFly, "Confirm the selected flight plan and weather preset")
        Me.btnCopyGoFly.UseVisualStyleBackColor = True
        '
        'btnClearFiles
        '
        Me.btnClearFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearFiles.Location = New System.Drawing.Point(12, 580)
        Me.btnClearFiles.Name = "btnClearFiles"
        Me.btnClearFiles.Size = New System.Drawing.Size(458, 33)
        Me.btnClearFiles.TabIndex = 4
        Me.btnClearFiles.Text = "Cancel"
        Me.ToolTip1.SetToolTip(Me.btnClearFiles, "Cancel and return to the main window")
        Me.btnClearFiles.UseVisualStyleBackColor = True
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
        Me.grpPLN.AllowDrop = True
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
        Me.grpWeather.Controls.Add(Me.grpCustomPresets)
        Me.grpWeather.Controls.Add(Me.optCustomPreset)
        Me.grpWeather.Controls.Add(Me.optSSCPreset)
        Me.grpWeather.Controls.Add(Me.cboSSCPresetList)
        Me.grpWeather.Location = New System.Drawing.Point(12, 118)
        Me.grpWeather.Name = "grpWeather"
        Me.grpWeather.Size = New System.Drawing.Size(458, 324)
        Me.grpWeather.TabIndex = 1
        Me.grpWeather.TabStop = False
        Me.grpWeather.Text = "Weather Preset"
        '
        'grpCustomPresets
        '
        Me.grpCustomPresets.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right) _
            Or System.Windows.Forms.AnchorStyles.Bottom), System.Windows.Forms.AnchorStyles)
        Me.grpCustomPresets.Controls.Add(Me.grpSecondaryWeather)
        Me.grpCustomPresets.Controls.Add(Me.grpPrimaryWeather)
        Me.grpCustomPresets.Location = New System.Drawing.Point(6, 88)
        Me.grpCustomPresets.Name = "grpCustomPresets"
        Me.grpCustomPresets.Size = New System.Drawing.Size(446, 230)
        Me.grpCustomPresets.TabIndex = 3
        Me.grpCustomPresets.TabStop = False
        Me.grpCustomPresets.Text = "Custom Preset"
        '
        'grpSecondaryWeather
        '
        Me.grpSecondaryWeather.AllowDrop = True
        Me.grpSecondaryWeather.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpSecondaryWeather.Controls.Add(Me.btnSecondaryBrowse)
        Me.grpSecondaryWeather.Controls.Add(Me.lblSecondaryName)
        Me.grpSecondaryWeather.Controls.Add(Me.lblSecondaryFile)
        Me.grpSecondaryWeather.Location = New System.Drawing.Point(6, 126)
        Me.grpSecondaryWeather.Name = "grpSecondaryWeather"
        Me.grpSecondaryWeather.Size = New System.Drawing.Size(434, 90)
        Me.grpSecondaryWeather.TabIndex = 1
        Me.grpSecondaryWeather.TabStop = False
        Me.grpSecondaryWeather.Text = "Secondary (MSFS 2020)"
        '
        'btnSecondaryBrowse
        '
        Me.btnSecondaryBrowse.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnSecondaryBrowse.Location = New System.Drawing.Point(6, 24)
        Me.btnSecondaryBrowse.Name = "btnSecondaryBrowse"
        Me.btnSecondaryBrowse.Size = New System.Drawing.Size(82, 55)
        Me.btnSecondaryBrowse.TabIndex = 0
        Me.btnSecondaryBrowse.Text = "Browse"
        Me.ToolTip1.SetToolTip(Me.btnSecondaryBrowse, "Select a custom secondary (2020) weather preset (.wpr)")
        Me.btnSecondaryBrowse.UseVisualStyleBackColor = True
        '
        'lblSecondaryName
        '
        Me.lblSecondaryName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSecondaryName.Location = New System.Drawing.Point(94, 54)
        Me.lblSecondaryName.Name = "lblSecondaryName"
        Me.lblSecondaryName.Size = New System.Drawing.Size(334, 25)
        Me.lblSecondaryName.TabIndex = 2
        Me.lblSecondaryName.Text = "Preset title"
        '
        'lblSecondaryFile
        '
        Me.lblSecondaryFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSecondaryFile.Location = New System.Drawing.Point(94, 29)
        Me.lblSecondaryFile.Name = "lblSecondaryFile"
        Me.lblSecondaryFile.Size = New System.Drawing.Size(334, 25)
        Me.lblSecondaryFile.TabIndex = 1
        Me.lblSecondaryFile.Text = "Weather preset file"
        '
        'grpPrimaryWeather
        '
        Me.grpPrimaryWeather.AllowDrop = True
        Me.grpPrimaryWeather.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpPrimaryWeather.Controls.Add(Me.btnPrimaryBrowse)
        Me.grpPrimaryWeather.Controls.Add(Me.lblPrimaryName)
        Me.grpPrimaryWeather.Controls.Add(Me.lblPrimaryFile)
        Me.grpPrimaryWeather.Location = New System.Drawing.Point(6, 26)
        Me.grpPrimaryWeather.Name = "grpPrimaryWeather"
        Me.grpPrimaryWeather.Size = New System.Drawing.Size(434, 90)
        Me.grpPrimaryWeather.TabIndex = 0
        Me.grpPrimaryWeather.TabStop = False
        Me.grpPrimaryWeather.Text = "Primary (MSFS 2024)"
        '
        'btnPrimaryBrowse
        '
        Me.btnPrimaryBrowse.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnPrimaryBrowse.Location = New System.Drawing.Point(6, 24)
        Me.btnPrimaryBrowse.Name = "btnPrimaryBrowse"
        Me.btnPrimaryBrowse.Size = New System.Drawing.Size(82, 55)
        Me.btnPrimaryBrowse.TabIndex = 0
        Me.btnPrimaryBrowse.Text = "Browse"
        Me.ToolTip1.SetToolTip(Me.btnPrimaryBrowse, "Select a custom primary (2024) weather preset (.wpr)")
        Me.btnPrimaryBrowse.UseVisualStyleBackColor = True
        '
        'lblPrimaryName
        '
        Me.lblPrimaryName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPrimaryName.Location = New System.Drawing.Point(94, 54)
        Me.lblPrimaryName.Name = "lblPrimaryName"
        Me.lblPrimaryName.Size = New System.Drawing.Size(334, 25)
        Me.lblPrimaryName.TabIndex = 2
        Me.lblPrimaryName.Text = "Preset title"
        '
        'lblPrimaryFile
        '
        Me.lblPrimaryFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPrimaryFile.Location = New System.Drawing.Point(94, 29)
        Me.lblPrimaryFile.Name = "lblPrimaryFile"
        Me.lblPrimaryFile.Size = New System.Drawing.Size(334, 25)
        Me.lblPrimaryFile.TabIndex = 1
        Me.lblPrimaryFile.Text = "Weather preset file"
        '
        'optCustomPreset
        '
        Me.optCustomPreset.AutoSize = True
        Me.optCustomPreset.Location = New System.Drawing.Point(10, 58)
        Me.optCustomPreset.Name = "optCustomPreset"
        Me.optCustomPreset.Size = New System.Drawing.Size(127, 24)
        Me.optCustomPreset.TabIndex = 2
        Me.optCustomPreset.TabStop = True
        Me.optCustomPreset.Text = "Custom Preset"
        Me.optCustomPreset.UseVisualStyleBackColor = True
        '
        'optSSCPreset
        '
        Me.optSSCPreset.AutoSize = True
        Me.optSSCPreset.Location = New System.Drawing.Point(10, 28)
        Me.optSSCPreset.Name = "optSSCPreset"
        Me.optSSCPreset.Size = New System.Drawing.Size(166, 24)
        Me.optSSCPreset.TabIndex = 0
        Me.optSSCPreset.TabStop = True
        Me.optSSCPreset.Text = "SSC Standard Preset"
        Me.optSSCPreset.UseVisualStyleBackColor = True
        '
        'cboSSCPresetList
        '
        Me.cboSSCPresetList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboSSCPresetList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSSCPresetList.FormattingEnabled = True
        Me.cboSSCPresetList.Location = New System.Drawing.Point(182, 27)
        Me.cboSSCPresetList.Name = "cboSSCPresetList"
        Me.cboSSCPresetList.Size = New System.Drawing.Size(270, 28)
        Me.cboSSCPresetList.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.cboSSCPresetList, "Select an SSC standard preset")
        '
        'grpTracker
        '
        Me.grpTracker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpTracker.Controls.Add(Me.txtTrackerGroupName)
        Me.grpTracker.Location = New System.Drawing.Point(12, 448)
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
        Me.ClientSize = New System.Drawing.Size(482, 650)
        Me.Controls.Add(Me.grpTracker)
        Me.Controls.Add(Me.btnClearFiles)
        Me.Controls.Add(Me.btnCopyGoFly)
        Me.Controls.Add(Me.grpWeather)
        Me.Controls.Add(Me.grpPLN)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(5000, 650)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(500, 650)
        Me.Name = "ManualFallbackMode"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Manual Fallback Mode - File Selection"
        Me.grpPLN.ResumeLayout(False)
        Me.grpWeather.ResumeLayout(False)
        Me.grpWeather.PerformLayout()
        Me.grpCustomPresets.ResumeLayout(False)
        Me.grpSecondaryWeather.ResumeLayout(False)
        Me.grpPrimaryWeather.ResumeLayout(False)
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
    Friend WithEvents btnCopyGoFly As Button
    Friend WithEvents btnClearFiles As Button
    Friend WithEvents grpTracker As GroupBox
    Friend WithEvents txtTrackerGroupName As TextBox
    Friend WithEvents optCustomPreset As RadioButton
    Friend WithEvents optSSCPreset As RadioButton
    Friend WithEvents cboSSCPresetList As ComboBox
    Friend WithEvents grpCustomPresets As GroupBox
    Friend WithEvents grpSecondaryWeather As GroupBox
    Friend WithEvents btnSecondaryBrowse As Button
    Friend WithEvents lblSecondaryName As Label
    Friend WithEvents lblSecondaryFile As Label
    Friend WithEvents grpPrimaryWeather As GroupBox
    Friend WithEvents btnPrimaryBrowse As Button
    Friend WithEvents lblPrimaryName As Label
    Friend WithEvents lblPrimaryFile As Label
End Class
