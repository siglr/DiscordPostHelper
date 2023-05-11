<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WindLayerControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.picWindDirection = New System.Windows.Forms.PictureBox()
        Me.lblAltitude = New System.Windows.Forms.Label()
        CType(Me.picWindDirection, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picWindDirection
        '
        Me.picWindDirection.Dock = System.Windows.Forms.DockStyle.Top
        Me.picWindDirection.Image = Global.SIGLR.SoaringTools.CommonLibrary.My.Resources.Resources.CompassRoseBack
        Me.picWindDirection.Location = New System.Drawing.Point(0, 0)
        Me.picWindDirection.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.picWindDirection.Name = "picWindDirection"
        Me.picWindDirection.Size = New System.Drawing.Size(222, 212)
        Me.picWindDirection.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picWindDirection.TabIndex = 0
        Me.picWindDirection.TabStop = False
        '
        'lblAltitude
        '
        Me.lblAltitude.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblAltitude.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.0!)
        Me.lblAltitude.Location = New System.Drawing.Point(0, 212)
        Me.lblAltitude.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblAltitude.Name = "lblAltitude"
        Me.lblAltitude.Size = New System.Drawing.Size(222, 42)
        Me.lblAltitude.TabIndex = 1
        Me.lblAltitude.Text = "label1"
        Me.lblAltitude.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'WindLayerControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblAltitude)
        Me.Controls.Add(Me.picWindDirection)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "WindLayerControl"
        Me.Size = New System.Drawing.Size(222, 254)
        CType(Me.picWindDirection, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents picWindDirection As Windows.Forms.PictureBox
    Friend WithEvents lblAltitude As Windows.Forms.Label
End Class
