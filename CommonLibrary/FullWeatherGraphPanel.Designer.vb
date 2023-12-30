<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FullWeatherGraphPanel
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
        Me.WindCloudDisplay1 = New SIGLR.SoaringTools.CommonLibrary.WindCloudDisplay()
        Me.splitWeatherLegend = New System.Windows.Forms.SplitContainer()
        CType(Me.splitWeatherLegend, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitWeatherLegend.SuspendLayout()
        Me.SuspendLayout()
        '
        'WindCloudDisplay1
        '
        Me.WindCloudDisplay1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.WindCloudDisplay1.Location = New System.Drawing.Point(0, 47)
        Me.WindCloudDisplay1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.WindCloudDisplay1.Name = "WindCloudDisplay1"
        Me.WindCloudDisplay1.Size = New System.Drawing.Size(1024, 721)
        Me.WindCloudDisplay1.TabIndex = 5
        Me.WindCloudDisplay1.Text = "WindCloudDisplay1"
        Me.WindCloudDisplay1.Visible = False
        '
        'splitWeatherLegend
        '
        Me.splitWeatherLegend.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.splitWeatherLegend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.splitWeatherLegend.IsSplitterFixed = True
        Me.splitWeatherLegend.Location = New System.Drawing.Point(0, 0)
        Me.splitWeatherLegend.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.splitWeatherLegend.Name = "splitWeatherLegend"
        Me.splitWeatherLegend.Size = New System.Drawing.Size(1024, 46)
        Me.splitWeatherLegend.SplitterDistance = 512
        Me.splitWeatherLegend.SplitterWidth = 5
        Me.splitWeatherLegend.TabIndex = 4
        '
        'FullWeatherGraphPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.WindCloudDisplay1)
        Me.Controls.Add(Me.splitWeatherLegend)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "FullWeatherGraphPanel"
        Me.Size = New System.Drawing.Size(1024, 768)
        CType(Me.splitWeatherLegend, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitWeatherLegend.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents WindCloudDisplay1 As WindCloudDisplay
    Friend WithEvents splitWeatherLegend As Windows.Forms.SplitContainer
End Class
