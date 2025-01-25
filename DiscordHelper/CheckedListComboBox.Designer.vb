<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CheckedListComboBox
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.txtDisplay = New System.Windows.Forms.TextBox()
        Me.btnDropdown = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'txtDisplay
        '
        Me.txtDisplay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDisplay.Location = New System.Drawing.Point(0, 0)
        Me.txtDisplay.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.txtDisplay.Name = "txtDisplay"
        Me.txtDisplay.ReadOnly = True
        Me.txtDisplay.Size = New System.Drawing.Size(238, 30)
        Me.txtDisplay.TabIndex = 0
        '
        'btnDropdown
        '
        Me.btnDropdown.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDropdown.Font = New System.Drawing.Font("Segoe UI Variable Display", 7.854546!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDropdown.Location = New System.Drawing.Point(234, 0)
        Me.btnDropdown.Name = "btnDropdown"
        Me.btnDropdown.Size = New System.Drawing.Size(36, 30)
        Me.btnDropdown.TabIndex = 1
        Me.btnDropdown.Text = "▼"
        Me.btnDropdown.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 200
        '
        'CheckedListComboBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 22.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDropdown)
        Me.Controls.Add(Me.txtDisplay)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.Name = "CheckedListComboBox"
        Me.Size = New System.Drawing.Size(269, 30)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtDisplay As TextBox
    Friend WithEvents btnDropdown As Button
    Friend WithEvents Timer1 As Timer
End Class
