<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MsgBoxWithPicture
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
        Me.lblMessageAbove = New System.Windows.Forms.Label()
        Me.lblMessageBelow = New System.Windows.Forms.Label()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnStopExpert = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnGoURL = New System.Windows.Forms.Button()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblMessageAbove
        '
        Me.lblMessageAbove.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.lblMessageAbove.Location = New System.Drawing.Point(16, 6)
        Me.lblMessageAbove.Name = "lblMessageAbove"
        Me.lblMessageAbove.Size = New System.Drawing.Size(659, 60)
        Me.lblMessageAbove.TabIndex = 1
        Me.lblMessageAbove.Text = "Content copied to your clipboard:"
        Me.lblMessageAbove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblMessageBelow
        '
        Me.lblMessageBelow.Location = New System.Drawing.Point(12, 456)
        Me.lblMessageBelow.Name = "lblMessageBelow"
        Me.lblMessageBelow.Size = New System.Drawing.Size(663, 111)
        Me.lblMessageBelow.TabIndex = 3
        Me.lblMessageBelow.Text = "Content copied to your clipboard:"
        Me.lblMessageBelow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnOk
        '
        Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnOk.Location = New System.Drawing.Point(219, 570)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(104, 34)
        Me.btnOk.TabIndex = 0
        Me.btnOk.Text = "Ok"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'btnStopExpert
        '
        Me.btnStopExpert.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnStopExpert.Location = New System.Drawing.Point(329, 570)
        Me.btnStopExpert.Name = "btnStopExpert"
        Me.btnStopExpert.Size = New System.Drawing.Size(133, 34)
        Me.btnStopExpert.TabIndex = 4
        Me.btnStopExpert.Text = "Stop Workflow"
        Me.btnStopExpert.UseVisualStyleBackColor = True
        Me.btnStopExpert.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(16, 69)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(659, 384)
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'btnGoURL
        '
        Me.btnGoURL.Location = New System.Drawing.Point(16, 570)
        Me.btnGoURL.Name = "btnGoURL"
        Me.btnGoURL.Size = New System.Drawing.Size(120, 34)
        Me.btnGoURL.TabIndex = 6
        Me.btnGoURL.Text = "Take me there!"
        Me.btnGoURL.UseVisualStyleBackColor = True
        Me.btnGoURL.Visible = False
        '
        'MsgBoxWithPicture
        '
        Me.AcceptButton = Me.btnOk
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnStopExpert
        Me.ClientSize = New System.Drawing.Size(687, 624)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnGoURL)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnStopExpert)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblMessageBelow)
        Me.Controls.Add(Me.lblMessageAbove)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MsgBoxWithPicture"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Instructions to read before proceeding"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblMessageAbove As Label
    Friend WithEvents lblMessageBelow As Label
    Friend WithEvents btnOk As Button
    Friend WithEvents btnStopExpert As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents btnGoURL As Button
End Class
