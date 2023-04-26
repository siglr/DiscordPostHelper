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
        Me.topButtonsArea = New System.Windows.Forms.Panel()
        Me.pnlUnpackBtn = New System.Windows.Forms.Panel()
        Me.btnCopyFiles = New System.Windows.Forms.Button()
        Me.LoadDPHX = New System.Windows.Forms.Button()
        Me.btnSettings = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtPackageName = New System.Windows.Forms.TextBox()
        Me.txtDPHFilename = New System.Windows.Forms.TextBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.pnlPackageFileName = New System.Windows.Forms.Panel()
        Me.pnlDPHFile = New System.Windows.Forms.Panel()
        Me.ctrlBriefing = New SIGLR.SoaringTools.CommonLibrary.BriefingControl()
        Me.topButtonsArea.SuspendLayout()
        Me.pnlUnpackBtn.SuspendLayout()
        Me.pnlPackageFileName.SuspendLayout()
        Me.pnlDPHFile.SuspendLayout()
        Me.SuspendLayout()
        '
        'topButtonsArea
        '
        Me.topButtonsArea.Controls.Add(Me.pnlUnpackBtn)
        Me.topButtonsArea.Controls.Add(Me.LoadDPHX)
        Me.topButtonsArea.Controls.Add(Me.btnSettings)
        Me.topButtonsArea.Dock = System.Windows.Forms.DockStyle.Top
        Me.topButtonsArea.Location = New System.Drawing.Point(0, 0)
        Me.topButtonsArea.Name = "topButtonsArea"
        Me.topButtonsArea.Padding = New System.Windows.Forms.Padding(5, 5, 5, 10)
        Me.topButtonsArea.Size = New System.Drawing.Size(1077, 45)
        Me.topButtonsArea.TabIndex = 0
        '
        'pnlUnpackBtn
        '
        Me.pnlUnpackBtn.BackColor = System.Drawing.SystemColors.Control
        Me.pnlUnpackBtn.Controls.Add(Me.btnCopyFiles)
        Me.pnlUnpackBtn.Location = New System.Drawing.Point(133, 2)
        Me.pnlUnpackBtn.Name = "pnlUnpackBtn"
        Me.pnlUnpackBtn.Padding = New System.Windows.Forms.Padding(2)
        Me.pnlUnpackBtn.Size = New System.Drawing.Size(128, 38)
        Me.pnlUnpackBtn.TabIndex = 3
        '
        'btnCopyFiles
        '
        Me.btnCopyFiles.Dock = System.Windows.Forms.DockStyle.Left
        Me.btnCopyFiles.Enabled = False
        Me.btnCopyFiles.Location = New System.Drawing.Point(2, 2)
        Me.btnCopyFiles.Name = "btnCopyFiles"
        Me.btnCopyFiles.Size = New System.Drawing.Size(123, 34)
        Me.btnCopyFiles.TabIndex = 2
        Me.btnCopyFiles.Text = "Unpack!"
        Me.ToolTip1.SetToolTip(Me.btnCopyFiles, "Click to unpack the files to their right locations")
        Me.btnCopyFiles.UseVisualStyleBackColor = True
        '
        'LoadDPHX
        '
        Me.LoadDPHX.Dock = System.Windows.Forms.DockStyle.Left
        Me.LoadDPHX.Location = New System.Drawing.Point(5, 5)
        Me.LoadDPHX.Name = "LoadDPHX"
        Me.LoadDPHX.Size = New System.Drawing.Size(123, 30)
        Me.LoadDPHX.TabIndex = 0
        Me.LoadDPHX.Text = "Load DPHX"
        Me.ToolTip1.SetToolTip(Me.LoadDPHX, "Click to select and open a DPHX package file")
        Me.LoadDPHX.UseVisualStyleBackColor = True
        '
        'btnSettings
        '
        Me.btnSettings.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnSettings.Location = New System.Drawing.Point(949, 5)
        Me.btnSettings.Name = "btnSettings"
        Me.btnSettings.Size = New System.Drawing.Size(123, 30)
        Me.btnSettings.TabIndex = 2
        Me.btnSettings.Text = "Settings"
        Me.ToolTip1.SetToolTip(Me.btnSettings, "Click to open the Settings windows")
        Me.btnSettings.UseVisualStyleBackColor = True
        '
        'txtPackageName
        '
        Me.txtPackageName.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPackageName.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtPackageName.Location = New System.Drawing.Point(8, 1)
        Me.txtPackageName.Name = "txtPackageName"
        Me.txtPackageName.ReadOnly = True
        Me.txtPackageName.Size = New System.Drawing.Size(1061, 27)
        Me.txtPackageName.TabIndex = 0
        Me.txtPackageName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ToolTip1.SetToolTip(Me.txtPackageName, "The currently loaded DPHX package file")
        '
        'txtDPHFilename
        '
        Me.txtDPHFilename.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDPHFilename.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.txtDPHFilename.Location = New System.Drawing.Point(8, 1)
        Me.txtDPHFilename.Name = "txtDPHFilename"
        Me.txtDPHFilename.ReadOnly = True
        Me.txtDPHFilename.Size = New System.Drawing.Size(1061, 27)
        Me.txtDPHFilename.TabIndex = 0
        Me.txtDPHFilename.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ToolTip1.SetToolTip(Me.txtDPHFilename, "The currently loaded DPH file")
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'pnlPackageFileName
        '
        Me.pnlPackageFileName.Controls.Add(Me.txtPackageName)
        Me.pnlPackageFileName.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlPackageFileName.Location = New System.Drawing.Point(0, 45)
        Me.pnlPackageFileName.Name = "pnlPackageFileName"
        Me.pnlPackageFileName.Padding = New System.Windows.Forms.Padding(5)
        Me.pnlPackageFileName.Size = New System.Drawing.Size(1077, 32)
        Me.pnlPackageFileName.TabIndex = 1
        '
        'pnlDPHFile
        '
        Me.pnlDPHFile.Controls.Add(Me.txtDPHFilename)
        Me.pnlDPHFile.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlDPHFile.Location = New System.Drawing.Point(0, 77)
        Me.pnlDPHFile.Name = "pnlDPHFile"
        Me.pnlDPHFile.Padding = New System.Windows.Forms.Padding(5)
        Me.pnlDPHFile.Size = New System.Drawing.Size(1077, 32)
        Me.pnlDPHFile.TabIndex = 2
        '
        'ctrlBriefing
        '
        Me.ctrlBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ctrlBriefing.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.ctrlBriefing.Location = New System.Drawing.Point(0, 109)
        Me.ctrlBriefing.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ctrlBriefing.MinimumSize = New System.Drawing.Size(700, 500)
        Me.ctrlBriefing.Name = "ctrlBriefing"
        Me.ctrlBriefing.Size = New System.Drawing.Size(1077, 649)
        Me.ctrlBriefing.TabIndex = 3
        '
        'DPHXUnpackAndLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1077, 758)
        Me.Controls.Add(Me.ctrlBriefing)
        Me.Controls.Add(Me.pnlDPHFile)
        Me.Controls.Add(Me.pnlPackageFileName)
        Me.Controls.Add(Me.topButtonsArea)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "DPHXUnpackAndLoad"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.topButtonsArea.ResumeLayout(False)
        Me.pnlUnpackBtn.ResumeLayout(False)
        Me.pnlPackageFileName.ResumeLayout(False)
        Me.pnlPackageFileName.PerformLayout()
        Me.pnlDPHFile.ResumeLayout(False)
        Me.pnlDPHFile.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents topButtonsArea As Panel
    Friend WithEvents btnSettings As Button
    Friend WithEvents LoadDPHX As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents pnlPackageFileName As Panel
    Friend WithEvents txtPackageName As TextBox
    Friend WithEvents pnlDPHFile As Panel
    Friend WithEvents txtDPHFilename As TextBox
    Friend WithEvents pnlUnpackBtn As Panel
    Friend WithEvents btnCopyFiles As Button
    Friend WithEvents ctrlBriefing As CommonLibrary.BriefingControl
End Class
