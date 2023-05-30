<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RecommendedAddOnsForm
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
        Me.txtAddOnName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.radioTypeFreeware = New System.Windows.Forms.RadioButton()
        Me.radioTypePayware = New System.Windows.Forms.RadioButton()
        Me.txtAddOnURL = New System.Windows.Forms.TextBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnURLPaste = New System.Windows.Forms.Button()
        Me.btnNamePaste = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtAddOnName
        '
        Me.txtAddOnName.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddOnName.Location = New System.Drawing.Point(116, 13)
        Me.txtAddOnName.Name = "txtAddOnName"
        Me.txtAddOnName.Size = New System.Drawing.Size(400, 32)
        Me.txtAddOnName.TabIndex = 1
        Me.txtAddOnName.Tag = "3"
        Me.ToolTip1.SetToolTip(Me.txtAddOnName, "Specify the name or title of the add-on")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(10, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Name"
        '
        'radioTypeFreeware
        '
        Me.radioTypeFreeware.AutoSize = True
        Me.radioTypeFreeware.Location = New System.Drawing.Point(116, 51)
        Me.radioTypeFreeware.Name = "radioTypeFreeware"
        Me.radioTypeFreeware.Size = New System.Drawing.Size(86, 24)
        Me.radioTypeFreeware.TabIndex = 4
        Me.radioTypeFreeware.TabStop = True
        Me.radioTypeFreeware.Text = "Freeware"
        Me.ToolTip1.SetToolTip(Me.radioTypeFreeware, "Select this if the add-on is a freeware")
        Me.radioTypeFreeware.UseVisualStyleBackColor = True
        '
        'radioTypePayware
        '
        Me.radioTypePayware.AutoSize = True
        Me.radioTypePayware.Location = New System.Drawing.Point(116, 81)
        Me.radioTypePayware.Name = "radioTypePayware"
        Me.radioTypePayware.Size = New System.Drawing.Size(80, 24)
        Me.radioTypePayware.TabIndex = 5
        Me.radioTypePayware.TabStop = True
        Me.radioTypePayware.Text = "Payware"
        Me.ToolTip1.SetToolTip(Me.radioTypePayware, "Select this if the add-on is a payware")
        Me.radioTypePayware.UseVisualStyleBackColor = True
        '
        'txtAddOnURL
        '
        Me.txtAddOnURL.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddOnURL.Location = New System.Drawing.Point(116, 111)
        Me.txtAddOnURL.Name = "txtAddOnURL"
        Me.txtAddOnURL.Size = New System.Drawing.Size(400, 32)
        Me.txtAddOnURL.TabIndex = 7
        Me.txtAddOnURL.Tag = "3"
        Me.ToolTip1.SetToolTip(Me.txtAddOnURL, "Specify the URL of the add-on")
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSave.Location = New System.Drawing.Point(95, 3)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(84, 35)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Tag = "18"
        Me.btnSave.Text = "Save"
        Me.ToolTip1.SetToolTip(Me.btnSave, "Click to save the add-on")
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnURLPaste
        '
        Me.btnURLPaste.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnURLPaste.Location = New System.Drawing.Point(522, 111)
        Me.btnURLPaste.Name = "btnURLPaste"
        Me.btnURLPaste.Size = New System.Drawing.Size(84, 32)
        Me.btnURLPaste.TabIndex = 8
        Me.btnURLPaste.Tag = "18"
        Me.btnURLPaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnURLPaste, "Click to paste the URL from the clipboard")
        Me.btnURLPaste.UseVisualStyleBackColor = True
        '
        'btnNamePaste
        '
        Me.btnNamePaste.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnNamePaste.Location = New System.Drawing.Point(522, 13)
        Me.btnNamePaste.Name = "btnNamePaste"
        Me.btnNamePaste.Size = New System.Drawing.Size(84, 32)
        Me.btnNamePaste.TabIndex = 2
        Me.btnNamePaste.Tag = "18"
        Me.btnNamePaste.Text = "Paste"
        Me.ToolTip1.SetToolTip(Me.btnNamePaste, "Click to paste the name from the clipboard")
        Me.btnNamePaste.UseVisualStyleBackColor = True
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
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(10, 49)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 26)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Type"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(10, 114)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(45, 26)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "URL"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCancel)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnSave)
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(337, 159)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(272, 43)
        Me.FlowLayoutPanel1.TabIndex = 9
        '
        'RecommendedAddOnsForm
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(617, 214)
        Me.ControlBox = False
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.btnNamePaste)
        Me.Controls.Add(Me.btnURLPaste)
        Me.Controls.Add(Me.txtAddOnURL)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.radioTypePayware)
        Me.Controls.Add(Me.radioTypeFreeware)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtAddOnName)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "RecommendedAddOnsForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Recommended Add-On"
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtAddOnName As TextBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents radioTypeFreeware As RadioButton
    Friend WithEvents radioTypePayware As RadioButton
    Friend WithEvents txtAddOnURL As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents btnSave As Button
    Friend WithEvents btnURLPaste As Button
    Friend WithEvents btnNamePaste As Button
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents btnCancel As Button
End Class
