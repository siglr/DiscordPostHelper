<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpdaterForm
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtProcessID = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtApplication = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtZipFile = New System.Windows.Forms.TextBox()
        Me.txtParamCount = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.chkZipFileDeleted = New System.Windows.Forms.CheckBox()
        Me.chkAllFilesUpdated = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtUpdatedFiles = New System.Windows.Forms.TextBox()
        Me.chkOtherSharedProcessesTerminated = New System.Windows.Forms.CheckBox()
        Me.chkCallerIsTerminated = New System.Windows.Forms.CheckBox()
        Me.btnUpdateCompleted = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.txtProcessID)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.txtApplication)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.txtZipFile)
        Me.GroupBox1.Controls.Add(Me.txtParamCount)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(728, 167)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Parameters"
        '
        'txtProcessID
        '
        Me.txtProcessID.Location = New System.Drawing.Point(109, 57)
        Me.txtProcessID.Name = "txtProcessID"
        Me.txtProcessID.ReadOnly = True
        Me.txtProcessID.Size = New System.Drawing.Size(68, 27)
        Me.txtProcessID.TabIndex = 7
        Me.txtProcessID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 60)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(79, 20)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Process ID"
        '
        'txtApplication
        '
        Me.txtApplication.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtApplication.Location = New System.Drawing.Point(109, 123)
        Me.txtApplication.Name = "txtApplication"
        Me.txtApplication.ReadOnly = True
        Me.txtApplication.Size = New System.Drawing.Size(613, 27)
        Me.txtApplication.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(7, 126)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 20)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Application"
        '
        'txtZipFile
        '
        Me.txtZipFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtZipFile.Location = New System.Drawing.Point(109, 90)
        Me.txtZipFile.Name = "txtZipFile"
        Me.txtZipFile.ReadOnly = True
        Me.txtZipFile.Size = New System.Drawing.Size(613, 27)
        Me.txtZipFile.TabIndex = 3
        '
        'txtParamCount
        '
        Me.txtParamCount.Location = New System.Drawing.Point(109, 24)
        Me.txtParamCount.Name = "txtParamCount"
        Me.txtParamCount.ReadOnly = True
        Me.txtParamCount.Size = New System.Drawing.Size(68, 27)
        Me.txtParamCount.TabIndex = 2
        Me.txtParamCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 93)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Zip file"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 27)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Count"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.chkZipFileDeleted)
        Me.GroupBox2.Controls.Add(Me.chkAllFilesUpdated)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.txtUpdatedFiles)
        Me.GroupBox2.Controls.Add(Me.chkOtherSharedProcessesTerminated)
        Me.GroupBox2.Controls.Add(Me.chkCallerIsTerminated)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 186)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(728, 337)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Update Steps"
        '
        'chkZipFileDeleted
        '
        Me.chkZipFileDeleted.AutoSize = True
        Me.chkZipFileDeleted.Location = New System.Drawing.Point(11, 302)
        Me.chkZipFileDeleted.Name = "chkZipFileDeleted"
        Me.chkZipFileDeleted.Size = New System.Drawing.Size(242, 24)
        Me.chkZipFileDeleted.TabIndex = 11
        Me.chkZipFileDeleted.Text = "Update package (zip file) deleted"
        Me.chkZipFileDeleted.UseVisualStyleBackColor = True
        '
        'chkAllFilesUpdated
        '
        Me.chkAllFilesUpdated.AutoSize = True
        Me.chkAllFilesUpdated.Location = New System.Drawing.Point(11, 272)
        Me.chkAllFilesUpdated.Name = "chkAllFilesUpdated"
        Me.chkAllFilesUpdated.Size = New System.Drawing.Size(133, 24)
        Me.chkAllFilesUpdated.TabIndex = 10
        Me.chkAllFilesUpdated.Text = "All files updated"
        Me.chkAllFilesUpdated.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 93)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(99, 20)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Updated files:"
        '
        'txtUpdatedFiles
        '
        Me.txtUpdatedFiles.Location = New System.Drawing.Point(6, 124)
        Me.txtUpdatedFiles.Multiline = True
        Me.txtUpdatedFiles.Name = "txtUpdatedFiles"
        Me.txtUpdatedFiles.ReadOnly = True
        Me.txtUpdatedFiles.Size = New System.Drawing.Size(716, 142)
        Me.txtUpdatedFiles.TabIndex = 2
        '
        'chkOtherSharedProcessesTerminated
        '
        Me.chkOtherSharedProcessesTerminated.AutoSize = True
        Me.chkOtherSharedProcessesTerminated.Location = New System.Drawing.Point(11, 66)
        Me.chkOtherSharedProcessesTerminated.Name = "chkOtherSharedProcessesTerminated"
        Me.chkOtherSharedProcessesTerminated.Size = New System.Drawing.Size(360, 24)
        Me.chkOtherSharedProcessesTerminated.TabIndex = 1
        Me.chkOtherSharedProcessesTerminated.Text = "Other processes sharing resources are not running"
        Me.chkOtherSharedProcessesTerminated.UseVisualStyleBackColor = True
        '
        'chkCallerIsTerminated
        '
        Me.chkCallerIsTerminated.AutoSize = True
        Me.chkCallerIsTerminated.Location = New System.Drawing.Point(11, 36)
        Me.chkCallerIsTerminated.Name = "chkCallerIsTerminated"
        Me.chkCallerIsTerminated.Size = New System.Drawing.Size(215, 24)
        Me.chkCallerIsTerminated.TabIndex = 0
        Me.chkCallerIsTerminated.Text = "Caller process is not running"
        Me.chkCallerIsTerminated.UseVisualStyleBackColor = True
        '
        'btnUpdateCompleted
        '
        Me.btnUpdateCompleted.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUpdateCompleted.Location = New System.Drawing.Point(12, 529)
        Me.btnUpdateCompleted.Name = "btnUpdateCompleted"
        Me.btnUpdateCompleted.Size = New System.Drawing.Size(728, 39)
        Me.btnUpdateCompleted.TabIndex = 2
        Me.btnUpdateCompleted.Text = "Update completed! Click here to resume."
        Me.btnUpdateCompleted.UseVisualStyleBackColor = True
        Me.btnUpdateCompleted.Visible = False
        '
        'UpdaterForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(752, 582)
        Me.Controls.Add(Me.btnUpdateCompleted)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UpdaterForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MSFS Soaring Task Tools Updater"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents txtZipFile As Windows.Forms.TextBox
    Friend WithEvents txtParamCount As Windows.Forms.TextBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents txtApplication As Windows.Forms.TextBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents txtProcessID As Windows.Forms.TextBox
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents chkCallerIsTerminated As Windows.Forms.CheckBox
    Friend WithEvents chkOtherSharedProcessesTerminated As Windows.Forms.CheckBox
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents txtUpdatedFiles As Windows.Forms.TextBox
    Friend WithEvents chkAllFilesUpdated As Windows.Forms.CheckBox
    Friend WithEvents chkZipFileDeleted As Windows.Forms.CheckBox
    Friend WithEvents btnUpdateCompleted As Windows.Forms.Button
End Class
