<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CopyContent
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
        Me.txtCopiedContent = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnStopExpert = New System.Windows.Forms.Button()
        Me.btnAutoPaste = New System.Windows.Forms.Button()
        Me.imgImageInClipboard = New System.Windows.Forms.PictureBox()
        Me.btnGoURL = New System.Windows.Forms.Button()
        Me.lblLocation = New System.Windows.Forms.Label()
        CType(Me.imgImageInClipboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtCopiedContent
        '
        Me.txtCopiedContent.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCopiedContent.Location = New System.Drawing.Point(12, 32)
        Me.txtCopiedContent.Multiline = True
        Me.txtCopiedContent.Name = "txtCopiedContent"
        Me.txtCopiedContent.ReadOnly = True
        Me.txtCopiedContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtCopiedContent.Size = New System.Drawing.Size(663, 417)
        Me.txtCopiedContent.TabIndex = 2
        Me.txtCopiedContent.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(12, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(247, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Content copied to your clipboard:"
        '
        'lblMessage
        '
        Me.lblMessage.Location = New System.Drawing.Point(12, 495)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(663, 114)
        Me.lblMessage.TabIndex = 3
        Me.lblMessage.Text = "Content copied to your clipboard:"
        Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnOk
        '
        Me.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnOk.Location = New System.Drawing.Point(287, 612)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(104, 34)
        Me.btnOk.TabIndex = 0
        Me.btnOk.Text = "Ok / Next"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'btnStopExpert
        '
        Me.btnStopExpert.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnStopExpert.Location = New System.Drawing.Point(397, 612)
        Me.btnStopExpert.Name = "btnStopExpert"
        Me.btnStopExpert.Size = New System.Drawing.Size(133, 34)
        Me.btnStopExpert.TabIndex = 4
        Me.btnStopExpert.Text = "Stop Workflow"
        Me.btnStopExpert.UseVisualStyleBackColor = True
        Me.btnStopExpert.Visible = False
        '
        'btnAutoPaste
        '
        Me.btnAutoPaste.Location = New System.Drawing.Point(148, 612)
        Me.btnAutoPaste.Name = "btnAutoPaste"
        Me.btnAutoPaste.Size = New System.Drawing.Size(133, 34)
        Me.btnAutoPaste.TabIndex = 5
        Me.btnAutoPaste.Text = "Auto paste"
        Me.btnAutoPaste.UseVisualStyleBackColor = True
        '
        'imgImageInClipboard
        '
        Me.imgImageInClipboard.Location = New System.Drawing.Point(12, 32)
        Me.imgImageInClipboard.Name = "imgImageInClipboard"
        Me.imgImageInClipboard.Size = New System.Drawing.Size(663, 417)
        Me.imgImageInClipboard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.imgImageInClipboard.TabIndex = 6
        Me.imgImageInClipboard.TabStop = False
        Me.imgImageInClipboard.Visible = False
        '
        'btnGoURL
        '
        Me.btnGoURL.Location = New System.Drawing.Point(12, 612)
        Me.btnGoURL.Name = "btnGoURL"
        Me.btnGoURL.Size = New System.Drawing.Size(130, 34)
        Me.btnGoURL.TabIndex = 7
        Me.btnGoURL.Text = "Take me there!"
        Me.btnGoURL.UseVisualStyleBackColor = True
        '
        'lblLocation
        '
        Me.lblLocation.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLocation.Font = New System.Drawing.Font("Segoe UI Variable Display", 15.70909!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLocation.Location = New System.Drawing.Point(12, 452)
        Me.lblLocation.Name = "lblLocation"
        Me.lblLocation.Size = New System.Drawing.Size(663, 43)
        Me.lblLocation.TabIndex = 8
        Me.lblLocation.Text = "Location"
        Me.lblLocation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CopyContent
        '
        Me.AcceptButton = Me.btnOk
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnOk
        Me.ClientSize = New System.Drawing.Size(687, 658)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblLocation)
        Me.Controls.Add(Me.btnGoURL)
        Me.Controls.Add(Me.imgImageInClipboard)
        Me.Controls.Add(Me.btnAutoPaste)
        Me.Controls.Add(Me.btnStopExpert)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtCopiedContent)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "CopyContent"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "CopyContent"
        CType(Me.imgImageInClipboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtCopiedContent As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents lblMessage As Label
    Friend WithEvents btnOk As Button
    Friend WithEvents btnStopExpert As Button
    Friend WithEvents btnAutoPaste As Button
    Friend WithEvents imgImageInClipboard As PictureBox
    Friend WithEvents btnGoURL As Button
    Friend WithEvents lblLocation As Label
End Class
