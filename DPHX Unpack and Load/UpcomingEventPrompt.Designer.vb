<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UpcomingEventPrompt

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
        Me.btnJoin = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.FlowLayoutPanelButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblDateValue = New System.Windows.Forms.Label()
        Me.lblSubtitleValue = New System.Windows.Forms.Label()
        Me.lblTitleValue = New System.Windows.Forms.Label()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.FlowLayoutPanelButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnJoin
        '
        Me.btnJoin.AutoSize = True
        Me.btnJoin.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnJoin.Location = New System.Drawing.Point(92, 5)
        Me.btnJoin.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnJoin.Name = "btnJoin"
        Me.btnJoin.Size = New System.Drawing.Size(80, 38)
        Me.btnJoin.TabIndex = 0
        Me.btnJoin.Text = "Join"
        Me.ToolTip1.SetToolTip(Me.btnJoin, "Click to download the DPHX file for this group event.")
        Me.btnJoin.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(4, 5)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 38)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        Me.ToolTip1.SetToolTip(Me.btnCancel, "Click to close this prompt without further action.")
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanelButtons
        '
        Me.FlowLayoutPanelButtons.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanelButtons.AutoSize = True
        Me.FlowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanelButtons.Controls.Add(Me.btnCancel)
        Me.FlowLayoutPanelButtons.Controls.Add(Me.btnJoin)
        Me.FlowLayoutPanelButtons.Location = New System.Drawing.Point(437, 293)
        Me.FlowLayoutPanelButtons.Margin = New System.Windows.Forms.Padding(0, 15, 0, 0)
        Me.FlowLayoutPanelButtons.Name = "FlowLayoutPanelButtons"
        Me.FlowLayoutPanelButtons.Size = New System.Drawing.Size(176, 48)
        Me.FlowLayoutPanelButtons.TabIndex = 8
        '
        'lblDateValue
        '
        Me.lblDateValue.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.lblDateValue.Location = New System.Drawing.Point(9, 47)
        Me.lblDateValue.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.lblDateValue.Name = "lblDateValue"
        Me.lblDateValue.Size = New System.Drawing.Size(604, 31)
        Me.lblDateValue.TabIndex = 7
        Me.lblDateValue.UseMnemonic = False
        '
        'lblSubtitleValue
        '
        Me.lblSubtitleValue.Font = New System.Drawing.Font("Segoe UI", 9.818182!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSubtitleValue.Location = New System.Drawing.Point(9, 78)
        Me.lblSubtitleValue.Margin = New System.Windows.Forms.Padding(0)
        Me.lblSubtitleValue.Name = "lblSubtitleValue"
        Me.lblSubtitleValue.Size = New System.Drawing.Size(604, 31)
        Me.lblSubtitleValue.TabIndex = 3
        Me.lblSubtitleValue.UseMnemonic = False
        '
        'lblTitleValue
        '
        Me.lblTitleValue.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitleValue.Location = New System.Drawing.Point(9, 9)
        Me.lblTitleValue.Margin = New System.Windows.Forms.Padding(0)
        Me.lblTitleValue.Name = "lblTitleValue"
        Me.lblTitleValue.Size = New System.Drawing.Size(604, 35)
        Me.lblTitleValue.TabIndex = 1
        Me.lblTitleValue.UseMnemonic = False
        '
        'txtComments
        '
        Me.txtComments.BackColor = System.Drawing.SystemColors.Window
        Me.txtComments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtComments.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.txtComments.Location = New System.Drawing.Point(9, 112)
        Me.txtComments.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.txtComments.Multiline = True
        Me.txtComments.Name = "txtComments"
        Me.txtComments.ReadOnly = True
        Me.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtComments.Size = New System.Drawing.Size(604, 177)
        Me.txtComments.TabIndex = 5
        Me.txtComments.TabStop = False
        '
        'UpcomingEventPrompt
        '
        Me.AcceptButton = Me.btnJoin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(621, 345)
        Me.Controls.Add(Me.lblTitleValue)
        Me.Controls.Add(Me.lblSubtitleValue)
        Me.Controls.Add(Me.lblDateValue)
        Me.Controls.Add(Me.FlowLayoutPanelButtons)
        Me.Controls.Add(Me.txtComments)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UpcomingEventPrompt"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Do you want to join this upcoming group event ?"
        Me.FlowLayoutPanelButtons.ResumeLayout(False)
        Me.FlowLayoutPanelButtons.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnJoin As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents FlowLayoutPanelButtons As FlowLayoutPanel
    Friend WithEvents lblDateValue As Label
    Friend WithEvents lblSubtitleValue As Label
    Friend WithEvents lblTitleValue As Label
    Friend WithEvents txtComments As TextBox
    Friend WithEvents ToolTip1 As ToolTip
End Class
