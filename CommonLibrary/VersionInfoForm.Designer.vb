<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Public Class VersionInfoForm
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
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnDoNotUpdate = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblLocalVersion = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblLatestVersion = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtReleaseNotes = New System.Windows.Forms.TextBox()
        Me.lblReleaseTitle = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtReleaseHistory = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'btnUpdate
        '
        Me.btnUpdate.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.btnUpdate.Location = New System.Drawing.Point(539, 597)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(151, 53)
        Me.btnUpdate.TabIndex = 0
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnDoNotUpdate
        '
        Me.btnDoNotUpdate.DialogResult = System.Windows.Forms.DialogResult.No
        Me.btnDoNotUpdate.Location = New System.Drawing.Point(696, 597)
        Me.btnDoNotUpdate.Name = "btnDoNotUpdate"
        Me.btnDoNotUpdate.Size = New System.Drawing.Size(207, 53)
        Me.btnDoNotUpdate.TabIndex = 1
        Me.btnDoNotUpdate.Text = "Don't update for now"
        Me.btnDoNotUpdate.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(147, 26)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Current version:"
        '
        'lblLocalVersion
        '
        Me.lblLocalVersion.AutoSize = True
        Me.lblLocalVersion.Location = New System.Drawing.Point(166, 13)
        Me.lblLocalVersion.Name = "lblLocalVersion"
        Me.lblLocalVersion.Size = New System.Drawing.Size(65, 26)
        Me.lblLocalVersion.TabIndex = 3
        Me.lblLocalVersion.Text = "Label2"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(13, 39)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(152, 26)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Version available:"
        '
        'lblLatestVersion
        '
        Me.lblLatestVersion.AutoSize = True
        Me.lblLatestVersion.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLatestVersion.Location = New System.Drawing.Point(171, 39)
        Me.lblLatestVersion.Name = "lblLatestVersion"
        Me.lblLatestVersion.Size = New System.Drawing.Size(70, 26)
        Me.lblLatestVersion.TabIndex = 5
        Me.lblLatestVersion.Text = "Label2"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(13, 79)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(130, 26)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Release notes:"
        '
        'txtReleaseNotes
        '
        Me.txtReleaseNotes.Location = New System.Drawing.Point(12, 134)
        Me.txtReleaseNotes.Multiline = True
        Me.txtReleaseNotes.Name = "txtReleaseNotes"
        Me.txtReleaseNotes.ReadOnly = True
        Me.txtReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtReleaseNotes.Size = New System.Drawing.Size(885, 190)
        Me.txtReleaseNotes.TabIndex = 7
        '
        'lblReleaseTitle
        '
        Me.lblReleaseTitle.AutoSize = True
        Me.lblReleaseTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblReleaseTitle.Location = New System.Drawing.Point(12, 105)
        Me.lblReleaseTitle.Name = "lblReleaseTitle"
        Me.lblReleaseTitle.Size = New System.Drawing.Size(166, 26)
        Me.lblReleaseTitle.TabIndex = 8
        Me.lblReleaseTitle.Text = "Version available:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(13, 327)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(141, 26)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Release history:"
        '
        'txtReleaseHistory
        '
        Me.txtReleaseHistory.Location = New System.Drawing.Point(12, 356)
        Me.txtReleaseHistory.Multiline = True
        Me.txtReleaseHistory.Name = "txtReleaseHistory"
        Me.txtReleaseHistory.ReadOnly = True
        Me.txtReleaseHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtReleaseHistory.Size = New System.Drawing.Size(885, 235)
        Me.txtReleaseHistory.TabIndex = 10
        '
        'VersionInfoForm
        '
        Me.AcceptButton = Me.btnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnDoNotUpdate
        Me.ClientSize = New System.Drawing.Size(915, 662)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtReleaseHistory)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblReleaseTitle)
        Me.Controls.Add(Me.txtReleaseNotes)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblLatestVersion)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblLocalVersion)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnDoNotUpdate)
        Me.Controls.Add(Me.btnUpdate)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.Name = "VersionInfoForm"
        Me.Text = "Update Available!"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnUpdate As Windows.Forms.Button
    Friend WithEvents btnDoNotUpdate As Windows.Forms.Button
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents lblLocalVersion As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents lblLatestVersion As Windows.Forms.Label
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents txtReleaseNotes As Windows.Forms.TextBox
    Friend WithEvents lblReleaseTitle As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents txtReleaseHistory As Windows.Forms.TextBox
End Class
