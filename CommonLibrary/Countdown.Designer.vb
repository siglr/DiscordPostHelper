<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Countdown
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
        Me.RemainingTimeLabel = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'RemainingTimeLabel
        '
        Me.RemainingTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RemainingTimeLabel.Location = New System.Drawing.Point(0, 0)
        Me.RemainingTimeLabel.Name = "RemainingTimeLabel"
        Me.RemainingTimeLabel.ReadOnly = True
        Me.RemainingTimeLabel.Size = New System.Drawing.Size(173, 52)
        Me.RemainingTimeLabel.TabIndex = 0
        Me.RemainingTimeLabel.Text = "000 00:00:00"
        Me.RemainingTimeLabel.ZoomFactor = 2.0!
        '
        'Countdown
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.RemainingTimeLabel)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "Countdown"
        Me.Size = New System.Drawing.Size(173, 52)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents RemainingTimeLabel As Windows.Forms.RichTextBox
End Class
