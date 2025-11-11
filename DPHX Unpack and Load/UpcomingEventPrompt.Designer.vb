<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpcomingEventPrompt

    <System.Diagnostics.DebuggerNonUserCode()> _
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

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
        Me.lblTitleCaption = New System.Windows.Forms.Label()
        Me.lblTitleValue = New System.Windows.Forms.Label()
        Me.lblSubtitleCaption = New System.Windows.Forms.Label()
        Me.lblSubtitleValue = New System.Windows.Forms.Label()
        Me.lblCommentsCaption = New System.Windows.Forms.Label()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.lblDateCaption = New System.Windows.Forms.Label()
        Me.lblDateValue = New System.Windows.Forms.Label()
        Me.FlowLayoutPanelButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnJoin = New System.Windows.Forms.Button()
        Me.TableLayoutPanelMain.SuspendLayout()
        Me.FlowLayoutPanelButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanelMain
        '
        Me.TableLayoutPanelMain.ColumnCount = 1
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelMain.Controls.Add(Me.lblTitleCaption, 0, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.lblTitleValue, 0, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.lblSubtitleCaption, 0, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.lblSubtitleValue, 0, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.lblCommentsCaption, 0, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.txtComments, 0, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.lblDateCaption, 0, 6)
        Me.TableLayoutPanelMain.Controls.Add(Me.lblDateValue, 0, 7)
        Me.TableLayoutPanelMain.Controls.Add(Me.FlowLayoutPanelButtons, 0, 8)
        Me.TableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanelMain.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanelMain.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanelMain.Name = "TableLayoutPanelMain"
        Me.TableLayoutPanelMain.Padding = New System.Windows.Forms.Padding(12)
        Me.TableLayoutPanelMain.RowCount = 9
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize))
        Me.TableLayoutPanelMain.Size = New System.Drawing.Size(480, 360)
        Me.TableLayoutPanelMain.TabIndex = 0
        '
        'lblTitleCaption
        '
        Me.lblTitleCaption.AutoSize = True
        Me.lblTitleCaption.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblTitleCaption.Location = New System.Drawing.Point(12, 12)
        Me.lblTitleCaption.Margin = New System.Windows.Forms.Padding(0)
        Me.lblTitleCaption.Name = "lblTitleCaption"
        Me.lblTitleCaption.Size = New System.Drawing.Size(31, 15)
        Me.lblTitleCaption.TabIndex = 0
        Me.lblTitleCaption.Text = "Title"
        Me.lblTitleCaption.UseMnemonic = False
        '
        'lblTitleValue
        '
        Me.lblTitleValue.AutoSize = True
        Me.lblTitleValue.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblTitleValue.Location = New System.Drawing.Point(12, 27)
        Me.lblTitleValue.Margin = New System.Windows.Forms.Padding(0)
        Me.lblTitleValue.Name = "lblTitleValue"
        Me.lblTitleValue.Size = New System.Drawing.Size(0, 20)
        Me.lblTitleValue.TabIndex = 1
        Me.lblTitleValue.UseMnemonic = False
        '
        'lblSubtitleCaption
        '
        Me.lblSubtitleCaption.AutoSize = True
        Me.lblSubtitleCaption.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblSubtitleCaption.Location = New System.Drawing.Point(12, 47)
        Me.lblSubtitleCaption.Margin = New System.Windows.Forms.Padding(0, 0, 0, 2)
        Me.lblSubtitleCaption.Name = "lblSubtitleCaption"
        Me.lblSubtitleCaption.Size = New System.Drawing.Size(53, 15)
        Me.lblSubtitleCaption.TabIndex = 2
        Me.lblSubtitleCaption.Text = "Subtitle"
        Me.lblSubtitleCaption.UseMnemonic = False
        '
        'lblSubtitleValue
        '
        Me.lblSubtitleValue.AutoSize = True
        Me.lblSubtitleValue.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.lblSubtitleValue.Location = New System.Drawing.Point(12, 64)
        Me.lblSubtitleValue.Margin = New System.Windows.Forms.Padding(0)
        Me.lblSubtitleValue.Name = "lblSubtitleValue"
        Me.lblSubtitleValue.Size = New System.Drawing.Size(0, 17)
        Me.lblSubtitleValue.TabIndex = 3
        Me.lblSubtitleValue.UseMnemonic = False
        '
        'lblCommentsCaption
        '
        Me.lblCommentsCaption.AutoSize = True
        Me.lblCommentsCaption.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblCommentsCaption.Location = New System.Drawing.Point(12, 81)
        Me.lblCommentsCaption.Margin = New System.Windows.Forms.Padding(0, 0, 0, 2)
        Me.lblCommentsCaption.Name = "lblCommentsCaption"
        Me.lblCommentsCaption.Size = New System.Drawing.Size(64, 15)
        Me.lblCommentsCaption.TabIndex = 4
        Me.lblCommentsCaption.Text = "Comments"
        Me.lblCommentsCaption.UseMnemonic = False
        '
        'txtComments
        '
        Me.txtComments.BackColor = System.Drawing.SystemColors.Window
        Me.txtComments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtComments.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtComments.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.txtComments.Location = New System.Drawing.Point(12, 98)
        Me.txtComments.Margin = New System.Windows.Forms.Padding(0, 2, 0, 0)
        Me.txtComments.Multiline = True
        Me.txtComments.Name = "txtComments"
        Me.txtComments.ReadOnly = True
        Me.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtComments.Size = New System.Drawing.Size(456, 186)
        Me.txtComments.TabIndex = 5
        Me.txtComments.TabStop = False
        '
        'lblDateCaption
        '
        Me.lblDateCaption.AutoSize = True
        Me.lblDateCaption.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.lblDateCaption.Location = New System.Drawing.Point(12, 284)
        Me.lblDateCaption.Margin = New System.Windows.Forms.Padding(0, 8, 0, 0)
        Me.lblDateCaption.Name = "lblDateCaption"
        Me.lblDateCaption.Size = New System.Drawing.Size(69, 15)
        Me.lblDateCaption.TabIndex = 6
        Me.lblDateCaption.Text = "Event time"
        Me.lblDateCaption.UseMnemonic = False
        '
        'lblDateValue
        '
        Me.lblDateValue.AutoSize = True
        Me.lblDateValue.Font = New System.Drawing.Font("Segoe UI", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.lblDateValue.Location = New System.Drawing.Point(12, 301)
        Me.lblDateValue.Margin = New System.Windows.Forms.Padding(0, 2, 0, 0)
        Me.lblDateValue.Name = "lblDateValue"
        Me.lblDateValue.Size = New System.Drawing.Size(0, 17)
        Me.lblDateValue.TabIndex = 7
        Me.lblDateValue.UseMnemonic = False
        '
        'FlowLayoutPanelButtons
        '
        Me.FlowLayoutPanelButtons.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.FlowLayoutPanelButtons.AutoSize = True
        Me.FlowLayoutPanelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanelButtons.Controls.Add(Me.btnCancel)
        Me.FlowLayoutPanelButtons.Controls.Add(Me.btnJoin)
        Me.FlowLayoutPanelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.FlowLayoutPanelButtons.Location = New System.Drawing.Point(323, 320)
        Me.FlowLayoutPanelButtons.Margin = New System.Windows.Forms.Padding(0, 10, 0, 0)
        Me.FlowLayoutPanelButtons.Name = "FlowLayoutPanelButtons"
        Me.FlowLayoutPanelButtons.Size = New System.Drawing.Size(145, 28)
        Me.FlowLayoutPanelButtons.TabIndex = 8
        '
        'btnCancel
        '
        Me.btnCancel.AutoSize = True
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(82, 3)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(60, 25)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnJoin
        '
        Me.btnJoin.AutoSize = True
        Me.btnJoin.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnJoin.Location = New System.Drawing.Point(16, 3)
        Me.btnJoin.Name = "btnJoin"
        Me.btnJoin.Size = New System.Drawing.Size(60, 25)
        Me.btnJoin.TabIndex = 0
        Me.btnJoin.Text = "Join"
        Me.btnJoin.UseVisualStyleBackColor = True
        '
        'UpcomingEventPrompt
        '
        Me.AcceptButton = Me.btnJoin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(480, 360)
        Me.Controls.Add(Me.TableLayoutPanelMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UpcomingEventPrompt"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Upcoming Event"
        Me.TableLayoutPanelMain.ResumeLayout(False)
        Me.TableLayoutPanelMain.PerformLayout()
        Me.FlowLayoutPanelButtons.ResumeLayout(False)
        Me.FlowLayoutPanelButtons.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanelMain As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblTitleCaption As System.Windows.Forms.Label
    Friend WithEvents lblTitleValue As System.Windows.Forms.Label
    Friend WithEvents lblSubtitleCaption As System.Windows.Forms.Label
    Friend WithEvents lblSubtitleValue As System.Windows.Forms.Label
    Friend WithEvents lblCommentsCaption As System.Windows.Forms.Label
    Friend WithEvents txtComments As System.Windows.Forms.TextBox
    Friend WithEvents lblDateCaption As System.Windows.Forms.Label
    Friend WithEvents lblDateValue As System.Windows.Forms.Label
    Friend WithEvents FlowLayoutPanelButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnJoin As System.Windows.Forms.Button
End Class
