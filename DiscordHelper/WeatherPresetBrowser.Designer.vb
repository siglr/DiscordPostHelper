<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WeatherPresetBrowser
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
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.optSSCPreset = New System.Windows.Forms.RadioButton()
        Me.optCustomPreset = New System.Windows.Forms.RadioButton()
        Me.cboSSCPresetList = New System.Windows.Forms.ComboBox()
        Me.btnPresetPrimaryBrowse = New System.Windows.Forms.Button()
        Me.lblWeatherPresetTitle2024 = New System.Windows.Forms.Label()
        Me.lblWeatherPresetTitleSecondary = New System.Windows.Forms.Label()
        Me.btnPresetSecondaryBrowse = New System.Windows.Forms.Button()
        Me.lblWeatherPresetPrimaryFilename = New System.Windows.Forms.Label()
        Me.lblWeatherPresetSecondaryFilename = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.grpSSCPresets = New System.Windows.Forms.GroupBox()
        Me.grpCustomPresets = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.grpSSCPresets.SuspendLayout()
        Me.grpCustomPresets.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSave.Location = New System.Drawing.Point(61, 3)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(118, 35)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Tag = "18"
        Me.btnSave.Text = "Set && Save"
        Me.ToolTip1.SetToolTip(Me.btnSave, "Click to set and save the weather profiles for the task")
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCancel.Location = New System.Drawing.Point(185, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(84, 35)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Tag = "18"
        Me.btnCancel.Text = "Cancel"
        Me.ToolTip1.SetToolTip(Me.btnCancel, "Click to cancel any change and go back to the main screen")
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'optSSCPreset
        '
        Me.optSSCPreset.AutoSize = True
        Me.optSSCPreset.Location = New System.Drawing.Point(12, 12)
        Me.optSSCPreset.Name = "optSSCPreset"
        Me.optSSCPreset.Size = New System.Drawing.Size(351, 24)
        Me.optSSCPreset.TabIndex = 0
        Me.optSSCPreset.TabStop = True
        Me.optSSCPreset.Text = "SSC Standard Preset (primary preset titles shown)"
        Me.ToolTip1.SetToolTip(Me.optSSCPreset, "Click to use standard SSC weather presets")
        Me.optSSCPreset.UseVisualStyleBackColor = True
        '
        'optCustomPreset
        '
        Me.optCustomPreset.AutoSize = True
        Me.optCustomPreset.Location = New System.Drawing.Point(12, 99)
        Me.optCustomPreset.Name = "optCustomPreset"
        Me.optCustomPreset.Size = New System.Drawing.Size(125, 24)
        Me.optCustomPreset.TabIndex = 2
        Me.optCustomPreset.TabStop = True
        Me.optCustomPreset.Text = "Custom Preset"
        Me.ToolTip1.SetToolTip(Me.optCustomPreset, "Click to use your own custom weather presets")
        Me.optCustomPreset.UseVisualStyleBackColor = True
        '
        'cboSSCPresetList
        '
        Me.cboSSCPresetList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboSSCPresetList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSSCPresetList.FormattingEnabled = True
        Me.cboSSCPresetList.Location = New System.Drawing.Point(6, 30)
        Me.cboSSCPresetList.Name = "cboSSCPresetList"
        Me.cboSSCPresetList.Size = New System.Drawing.Size(581, 28)
        Me.cboSSCPresetList.Sorted = True
        Me.cboSSCPresetList.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.cboSSCPresetList, "Select the SSC weather preset from the list (primary preset titles shown)")
        '
        'btnPresetPrimaryBrowse
        '
        Me.btnPresetPrimaryBrowse.Location = New System.Drawing.Point(512, 43)
        Me.btnPresetPrimaryBrowse.Name = "btnPresetPrimaryBrowse"
        Me.btnPresetPrimaryBrowse.Size = New System.Drawing.Size(75, 35)
        Me.btnPresetPrimaryBrowse.TabIndex = 2
        Me.btnPresetPrimaryBrowse.Text = "Browse"
        Me.ToolTip1.SetToolTip(Me.btnPresetPrimaryBrowse, "Click to browse and select the primary weather preset file")
        Me.btnPresetPrimaryBrowse.UseVisualStyleBackColor = True
        '
        'lblWeatherPresetTitle2024
        '
        Me.lblWeatherPresetTitle2024.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeatherPresetTitle2024.Location = New System.Drawing.Point(6, 74)
        Me.lblWeatherPresetTitle2024.Name = "lblWeatherPresetTitle2024"
        Me.lblWeatherPresetTitle2024.Size = New System.Drawing.Size(500, 27)
        Me.lblWeatherPresetTitle2024.TabIndex = 3
        Me.lblWeatherPresetTitle2024.Text = "- no preset selected -"
        Me.lblWeatherPresetTitle2024.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.lblWeatherPresetTitle2024, "This is the primary weather preset title")
        '
        'lblWeatherPresetTitleSecondary
        '
        Me.lblWeatherPresetTitleSecondary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeatherPresetTitleSecondary.Location = New System.Drawing.Point(6, 151)
        Me.lblWeatherPresetTitleSecondary.Name = "lblWeatherPresetTitleSecondary"
        Me.lblWeatherPresetTitleSecondary.Size = New System.Drawing.Size(500, 27)
        Me.lblWeatherPresetTitleSecondary.TabIndex = 7
        Me.lblWeatherPresetTitleSecondary.Text = "- no preset selected -"
        Me.lblWeatherPresetTitleSecondary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.lblWeatherPresetTitleSecondary, "This is the optional secondary weather preset title")
        '
        'btnPresetSecondaryBrowse
        '
        Me.btnPresetSecondaryBrowse.Enabled = False
        Me.btnPresetSecondaryBrowse.Location = New System.Drawing.Point(512, 120)
        Me.btnPresetSecondaryBrowse.Name = "btnPresetSecondaryBrowse"
        Me.btnPresetSecondaryBrowse.Size = New System.Drawing.Size(75, 35)
        Me.btnPresetSecondaryBrowse.TabIndex = 6
        Me.btnPresetSecondaryBrowse.Text = "Browse"
        Me.ToolTip1.SetToolTip(Me.btnPresetSecondaryBrowse, "Click to browse and select the optional secondary weather preset file")
        Me.btnPresetSecondaryBrowse.UseVisualStyleBackColor = True
        '
        'lblWeatherPresetPrimaryFilename
        '
        Me.lblWeatherPresetPrimaryFilename.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeatherPresetPrimaryFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblWeatherPresetPrimaryFilename.Location = New System.Drawing.Point(6, 47)
        Me.lblWeatherPresetPrimaryFilename.Name = "lblWeatherPresetPrimaryFilename"
        Me.lblWeatherPresetPrimaryFilename.Size = New System.Drawing.Size(500, 27)
        Me.lblWeatherPresetPrimaryFilename.TabIndex = 1
        Me.lblWeatherPresetPrimaryFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblWeatherPresetSecondaryFilename
        '
        Me.lblWeatherPresetSecondaryFilename.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeatherPresetSecondaryFilename.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblWeatherPresetSecondaryFilename.Location = New System.Drawing.Point(6, 124)
        Me.lblWeatherPresetSecondaryFilename.Name = "lblWeatherPresetSecondaryFilename"
        Me.lblWeatherPresetSecondaryFilename.Size = New System.Drawing.Size(500, 27)
        Me.lblWeatherPresetSecondaryFilename.TabIndex = 5
        Me.lblWeatherPresetSecondaryFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCancel)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnSave)
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(337, 296)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(272, 43)
        Me.FlowLayoutPanel1.TabIndex = 4
        '
        'grpSSCPresets
        '
        Me.grpSSCPresets.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpSSCPresets.Controls.Add(Me.cboSSCPresetList)
        Me.grpSSCPresets.Location = New System.Drawing.Point(12, 12)
        Me.grpSSCPresets.Name = "grpSSCPresets"
        Me.grpSSCPresets.Size = New System.Drawing.Size(593, 81)
        Me.grpSSCPresets.TabIndex = 1
        Me.grpSSCPresets.TabStop = False
        '
        'grpCustomPresets
        '
        Me.grpCustomPresets.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpCustomPresets.Controls.Add(Me.lblWeatherPresetTitleSecondary)
        Me.grpCustomPresets.Controls.Add(Me.btnPresetSecondaryBrowse)
        Me.grpCustomPresets.Controls.Add(Me.Label3)
        Me.grpCustomPresets.Controls.Add(Me.lblWeatherPresetSecondaryFilename)
        Me.grpCustomPresets.Controls.Add(Me.lblWeatherPresetTitle2024)
        Me.grpCustomPresets.Controls.Add(Me.btnPresetPrimaryBrowse)
        Me.grpCustomPresets.Controls.Add(Me.Label2)
        Me.grpCustomPresets.Controls.Add(Me.lblWeatherPresetPrimaryFilename)
        Me.grpCustomPresets.Location = New System.Drawing.Point(12, 99)
        Me.grpCustomPresets.Name = "grpCustomPresets"
        Me.grpCustomPresets.Size = New System.Drawing.Size(593, 191)
        Me.grpCustomPresets.TabIndex = 3
        Me.grpCustomPresets.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(2, 104)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(146, 20)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Secondary (Optional)"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(2, 27)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(129, 20)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Primary (Required)"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'WeatherPresetBrowser
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(617, 351)
        Me.ControlBox = False
        Me.Controls.Add(Me.optCustomPreset)
        Me.Controls.Add(Me.grpCustomPresets)
        Me.Controls.Add(Me.optSSCPreset)
        Me.Controls.Add(Me.grpSSCPresets)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "WeatherPresetBrowser"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Weather Profiles Selection"
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.grpSSCPresets.ResumeLayout(False)
        Me.grpCustomPresets.ResumeLayout(False)
        Me.grpCustomPresets.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents btnSave As Button
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents btnCancel As Button
    Friend WithEvents optSSCPreset As RadioButton
    Friend WithEvents grpSSCPresets As GroupBox
    Friend WithEvents optCustomPreset As RadioButton
    Friend WithEvents grpCustomPresets As GroupBox
    Friend WithEvents cboSSCPresetList As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents lblWeatherPresetPrimaryFilename As Label
    Friend WithEvents btnPresetPrimaryBrowse As Button
    Friend WithEvents lblWeatherPresetTitleSecondary As Label
    Friend WithEvents btnPresetSecondaryBrowse As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents lblWeatherPresetSecondaryFilename As Label
    Friend WithEvents lblWeatherPresetTitle2024 As Label
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
End Class
