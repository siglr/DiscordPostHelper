﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.pnlFlightPlanFilesFolder = New System.Windows.Forms.Panel()
        Me.btnFlightPlansFolderPaste = New System.Windows.Forms.Button()
        Me.btnFlightPlanFilesFolder = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnWeatherPresetsFolder = New System.Windows.Forms.Button()
        Me.btnUnpackingFolder = New System.Windows.Forms.Button()
        Me.optOverwriteAlwaysOverwrite = New System.Windows.Forms.RadioButton()
        Me.optOverwriteAlwaysSkip = New System.Windows.Forms.RadioButton()
        Me.optOverwriteAlwaysAsk = New System.Windows.Forms.RadioButton()
        Me.btnWeatherPresetsFolderPaste = New System.Windows.Forms.Button()
        Me.btnTempFolderPaste = New System.Windows.Forms.Button()
        Me.btnPackagesFolderPaste = New System.Windows.Forms.Button()
        Me.btnPackagesFolder = New System.Windows.Forms.Button()
        Me.pblWeatherPresetsFolder = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.pnlUnpackingFolder = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.pnlPackagesFolder = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnlAutoOverwrite = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.pnlFlightPlanFilesFolder.SuspendLayout()
        Me.pblWeatherPresetsFolder.SuspendLayout()
        Me.pnlUnpackingFolder.SuspendLayout()
        Me.pnlPackagesFolder.SuspendLayout()
        Me.pnlAutoOverwrite.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(487, 247)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(195, 45)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(4, 5)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(89, 35)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.Location = New System.Drawing.Point(101, 5)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(89, 35)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'pnlFlightPlanFilesFolder
        '
        Me.pnlFlightPlanFilesFolder.Controls.Add(Me.btnFlightPlansFolderPaste)
        Me.pnlFlightPlanFilesFolder.Controls.Add(Me.btnFlightPlanFilesFolder)
        Me.pnlFlightPlanFilesFolder.Controls.Add(Me.Label1)
        Me.pnlFlightPlanFilesFolder.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFlightPlanFilesFolder.Location = New System.Drawing.Point(0, 0)
        Me.pnlFlightPlanFilesFolder.Name = "pnlFlightPlanFilesFolder"
        Me.pnlFlightPlanFilesFolder.Padding = New System.Windows.Forms.Padding(2)
        Me.pnlFlightPlanFilesFolder.Size = New System.Drawing.Size(682, 45)
        Me.pnlFlightPlanFilesFolder.TabIndex = 0
        '
        'btnFlightPlansFolderPaste
        '
        Me.btnFlightPlansFolderPaste.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlightPlansFolderPaste.Location = New System.Drawing.Point(604, 4)
        Me.btnFlightPlansFolderPaste.Name = "btnFlightPlansFolderPaste"
        Me.btnFlightPlansFolderPaste.Size = New System.Drawing.Size(75, 37)
        Me.btnFlightPlansFolderPaste.TabIndex = 2
        Me.btnFlightPlansFolderPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnFlightPlansFolderPaste, "Click this button to paste a folder from your clipboard")
        Me.btnFlightPlansFolderPaste.UseVisualStyleBackColor = True
        '
        'btnFlightPlanFilesFolder
        '
        Me.btnFlightPlanFilesFolder.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlightPlanFilesFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFlightPlanFilesFolder.Location = New System.Drawing.Point(197, 4)
        Me.btnFlightPlanFilesFolder.Name = "btnFlightPlanFilesFolder"
        Me.btnFlightPlanFilesFolder.Size = New System.Drawing.Size(401, 37)
        Me.btnFlightPlanFilesFolder.TabIndex = 1
        Me.btnFlightPlanFilesFolder.Text = "Select the folder for the flight plan files (.pln)"
        Me.btnFlightPlanFilesFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.btnFlightPlanFilesFolder, "Select the folder for the flight plan files (.pln)")
        Me.btnFlightPlanFilesFolder.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(154, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Flight Plan Files Folder:"
        '
        'btnWeatherPresetsFolder
        '
        Me.btnWeatherPresetsFolder.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeatherPresetsFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnWeatherPresetsFolder.Location = New System.Drawing.Point(197, 4)
        Me.btnWeatherPresetsFolder.Name = "btnWeatherPresetsFolder"
        Me.btnWeatherPresetsFolder.Size = New System.Drawing.Size(401, 37)
        Me.btnWeatherPresetsFolder.TabIndex = 1
        Me.btnWeatherPresetsFolder.Text = "Select the MSFS folder containing weather presets (.wpr)"
        Me.btnWeatherPresetsFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.btnWeatherPresetsFolder, "Select the MSFS folder containing weather presets (.wpr)")
        Me.btnWeatherPresetsFolder.UseVisualStyleBackColor = True
        '
        'btnUnpackingFolder
        '
        Me.btnUnpackingFolder.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUnpackingFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnUnpackingFolder.Location = New System.Drawing.Point(197, 4)
        Me.btnUnpackingFolder.Name = "btnUnpackingFolder"
        Me.btnUnpackingFolder.Size = New System.Drawing.Size(401, 37)
        Me.btnUnpackingFolder.TabIndex = 1
        Me.btnUnpackingFolder.Text = "Select the folder where to temporary unpack DPHX packages"
        Me.btnUnpackingFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.btnUnpackingFolder, "Select the folder for the flight plan files (.pln)")
        Me.btnUnpackingFolder.UseVisualStyleBackColor = True
        '
        'optOverwriteAlwaysOverwrite
        '
        Me.optOverwriteAlwaysOverwrite.AutoSize = True
        Me.optOverwriteAlwaysOverwrite.Location = New System.Drawing.Point(197, 10)
        Me.optOverwriteAlwaysOverwrite.Name = "optOverwriteAlwaysOverwrite"
        Me.optOverwriteAlwaysOverwrite.Size = New System.Drawing.Size(138, 24)
        Me.optOverwriteAlwaysOverwrite.TabIndex = 1
        Me.optOverwriteAlwaysOverwrite.Text = "Always overwrite"
        Me.ToolTip1.SetToolTip(Me.optOverwriteAlwaysOverwrite, "Files with the same names will be overwritten")
        Me.optOverwriteAlwaysOverwrite.UseVisualStyleBackColor = True
        '
        'optOverwriteAlwaysSkip
        '
        Me.optOverwriteAlwaysSkip.AutoSize = True
        Me.optOverwriteAlwaysSkip.Location = New System.Drawing.Point(341, 10)
        Me.optOverwriteAlwaysSkip.Name = "optOverwriteAlwaysSkip"
        Me.optOverwriteAlwaysSkip.Size = New System.Drawing.Size(103, 24)
        Me.optOverwriteAlwaysSkip.TabIndex = 2
        Me.optOverwriteAlwaysSkip.Text = "Always skip"
        Me.ToolTip1.SetToolTip(Me.optOverwriteAlwaysSkip, "Files with the same names will be skipped")
        Me.optOverwriteAlwaysSkip.UseVisualStyleBackColor = True
        '
        'optOverwriteAlwaysAsk
        '
        Me.optOverwriteAlwaysAsk.AutoSize = True
        Me.optOverwriteAlwaysAsk.Checked = True
        Me.optOverwriteAlwaysAsk.Location = New System.Drawing.Point(450, 10)
        Me.optOverwriteAlwaysAsk.Name = "optOverwriteAlwaysAsk"
        Me.optOverwriteAlwaysAsk.Size = New System.Drawing.Size(98, 24)
        Me.optOverwriteAlwaysAsk.TabIndex = 3
        Me.optOverwriteAlwaysAsk.TabStop = True
        Me.optOverwriteAlwaysAsk.Text = "Always ask"
        Me.ToolTip1.SetToolTip(Me.optOverwriteAlwaysAsk, "Files with the same names will result in a dialog asking you each time.")
        Me.optOverwriteAlwaysAsk.UseVisualStyleBackColor = True
        '
        'btnWeatherPresetsFolderPaste
        '
        Me.btnWeatherPresetsFolderPaste.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeatherPresetsFolderPaste.Location = New System.Drawing.Point(604, 4)
        Me.btnWeatherPresetsFolderPaste.Name = "btnWeatherPresetsFolderPaste"
        Me.btnWeatherPresetsFolderPaste.Size = New System.Drawing.Size(75, 37)
        Me.btnWeatherPresetsFolderPaste.TabIndex = 2
        Me.btnWeatherPresetsFolderPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnWeatherPresetsFolderPaste, "Click this button to paste a folder from your clipboard")
        Me.btnWeatherPresetsFolderPaste.UseVisualStyleBackColor = True
        '
        'btnTempFolderPaste
        '
        Me.btnTempFolderPaste.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnTempFolderPaste.Location = New System.Drawing.Point(604, 4)
        Me.btnTempFolderPaste.Name = "btnTempFolderPaste"
        Me.btnTempFolderPaste.Size = New System.Drawing.Size(75, 37)
        Me.btnTempFolderPaste.TabIndex = 2
        Me.btnTempFolderPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnTempFolderPaste, "Click this button to paste a folder from your clipboard")
        Me.btnTempFolderPaste.UseVisualStyleBackColor = True
        '
        'btnPackagesFolderPaste
        '
        Me.btnPackagesFolderPaste.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesFolderPaste.Location = New System.Drawing.Point(604, 4)
        Me.btnPackagesFolderPaste.Name = "btnPackagesFolderPaste"
        Me.btnPackagesFolderPaste.Size = New System.Drawing.Size(75, 37)
        Me.btnPackagesFolderPaste.TabIndex = 2
        Me.btnPackagesFolderPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnPackagesFolderPaste, "Click this button to paste a folder from your clipboard")
        Me.btnPackagesFolderPaste.UseVisualStyleBackColor = True
        '
        'btnPackagesFolder
        '
        Me.btnPackagesFolder.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesFolder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnPackagesFolder.Location = New System.Drawing.Point(197, 4)
        Me.btnPackagesFolder.Name = "btnPackagesFolder"
        Me.btnPackagesFolder.Size = New System.Drawing.Size(401, 37)
        Me.btnPackagesFolder.TabIndex = 1
        Me.btnPackagesFolder.Text = "Select the folder where your DPHX packages are stored"
        Me.btnPackagesFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolTip1.SetToolTip(Me.btnPackagesFolder, "Select the folder where your DPHX packages are stored")
        Me.btnPackagesFolder.UseVisualStyleBackColor = True
        '
        'pblWeatherPresetsFolder
        '
        Me.pblWeatherPresetsFolder.Controls.Add(Me.btnWeatherPresetsFolderPaste)
        Me.pblWeatherPresetsFolder.Controls.Add(Me.btnWeatherPresetsFolder)
        Me.pblWeatherPresetsFolder.Controls.Add(Me.Label2)
        Me.pblWeatherPresetsFolder.Dock = System.Windows.Forms.DockStyle.Top
        Me.pblWeatherPresetsFolder.Location = New System.Drawing.Point(0, 45)
        Me.pblWeatherPresetsFolder.Name = "pblWeatherPresetsFolder"
        Me.pblWeatherPresetsFolder.Padding = New System.Windows.Forms.Padding(2)
        Me.pblWeatherPresetsFolder.Size = New System.Drawing.Size(682, 45)
        Me.pblWeatherPresetsFolder.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(5, 12)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(163, 20)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Weather Presets Folder:"
        '
        'pnlUnpackingFolder
        '
        Me.pnlUnpackingFolder.Controls.Add(Me.btnTempFolderPaste)
        Me.pnlUnpackingFolder.Controls.Add(Me.btnUnpackingFolder)
        Me.pnlUnpackingFolder.Controls.Add(Me.Label3)
        Me.pnlUnpackingFolder.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlUnpackingFolder.Location = New System.Drawing.Point(0, 90)
        Me.pnlUnpackingFolder.Name = "pnlUnpackingFolder"
        Me.pnlUnpackingFolder.Padding = New System.Windows.Forms.Padding(2)
        Me.pnlUnpackingFolder.Size = New System.Drawing.Size(682, 45)
        Me.pnlUnpackingFolder.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(5, 12)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(167, 20)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Unpacking Temp Folder:"
        '
        'pnlPackagesFolder
        '
        Me.pnlPackagesFolder.Controls.Add(Me.btnPackagesFolderPaste)
        Me.pnlPackagesFolder.Controls.Add(Me.btnPackagesFolder)
        Me.pnlPackagesFolder.Controls.Add(Me.Label5)
        Me.pnlPackagesFolder.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlPackagesFolder.Location = New System.Drawing.Point(0, 135)
        Me.pnlPackagesFolder.Name = "pnlPackagesFolder"
        Me.pnlPackagesFolder.Padding = New System.Windows.Forms.Padding(2)
        Me.pnlPackagesFolder.Size = New System.Drawing.Size(682, 45)
        Me.pnlPackagesFolder.TabIndex = 5
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(5, 12)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(159, 20)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "DPHX Packages Folder:"
        '
        'pnlAutoOverwrite
        '
        Me.pnlAutoOverwrite.Controls.Add(Me.optOverwriteAlwaysAsk)
        Me.pnlAutoOverwrite.Controls.Add(Me.optOverwriteAlwaysSkip)
        Me.pnlAutoOverwrite.Controls.Add(Me.optOverwriteAlwaysOverwrite)
        Me.pnlAutoOverwrite.Controls.Add(Me.Label4)
        Me.pnlAutoOverwrite.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlAutoOverwrite.Location = New System.Drawing.Point(0, 180)
        Me.pnlAutoOverwrite.Name = "pnlAutoOverwrite"
        Me.pnlAutoOverwrite.Padding = New System.Windows.Forms.Padding(2)
        Me.pnlAutoOverwrite.Size = New System.Drawing.Size(682, 45)
        Me.pnlAutoOverwrite.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(5, 12)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(147, 20)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Automatic Overwrite:"
        '
        'Settings
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(682, 306)
        Me.ControlBox = False
        Me.Controls.Add(Me.pnlAutoOverwrite)
        Me.Controls.Add(Me.pnlPackagesFolder)
        Me.Controls.Add(Me.pnlUnpackingFolder)
        Me.Controls.Add(Me.pblWeatherPresetsFolder)
        Me.Controls.Add(Me.pnlFlightPlanFilesFolder)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(5000, 350)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(700, 350)
        Me.Name = "Settings"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "DPHX Unpack and Load - Settings"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.pnlFlightPlanFilesFolder.ResumeLayout(False)
        Me.pnlFlightPlanFilesFolder.PerformLayout()
        Me.pblWeatherPresetsFolder.ResumeLayout(False)
        Me.pblWeatherPresetsFolder.PerformLayout()
        Me.pnlUnpackingFolder.ResumeLayout(False)
        Me.pnlUnpackingFolder.PerformLayout()
        Me.pnlPackagesFolder.ResumeLayout(False)
        Me.pnlPackagesFolder.PerformLayout()
        Me.pnlAutoOverwrite.ResumeLayout(False)
        Me.pnlAutoOverwrite.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents pnlFlightPlanFilesFolder As Panel
    Friend WithEvents btnFlightPlanFilesFolder As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Label1 As Label
    Friend WithEvents pblWeatherPresetsFolder As Panel
    Friend WithEvents btnWeatherPresetsFolder As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents pnlUnpackingFolder As Panel
    Friend WithEvents btnUnpackingFolder As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents pnlAutoOverwrite As Panel
    Friend WithEvents optOverwriteAlwaysSkip As RadioButton
    Friend WithEvents optOverwriteAlwaysOverwrite As RadioButton
    Friend WithEvents Label4 As Label
    Friend WithEvents optOverwriteAlwaysAsk As RadioButton
    Friend WithEvents btnFlightPlansFolderPaste As Button
    Friend WithEvents btnWeatherPresetsFolderPaste As Button
    Friend WithEvents btnTempFolderPaste As Button
    Friend WithEvents pnlPackagesFolder As Panel
    Friend WithEvents btnPackagesFolderPaste As Button
    Friend WithEvents btnPackagesFolder As Button
    Friend WithEvents Label5 As Label
End Class