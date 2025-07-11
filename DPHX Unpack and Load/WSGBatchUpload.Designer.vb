<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WSGBatchUpload
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.browser = New CefSharp.WinForms.ChromiumWebBrowser()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'browser
        '
        Me.browser.ActivateBrowserOnCreation = False
        Me.browser.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.browser.Location = New System.Drawing.Point(9, 8)
        Me.browser.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.browser.Name = "browser"
        Me.browser.Size = New System.Drawing.Size(1112, 645)
        Me.browser.TabIndex = 0
        '
        'txtLog
        '
        Me.txtLog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLog.Location = New System.Drawing.Point(1126, 8)
        Me.txtLog.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtLog.Size = New System.Drawing.Size(374, 646)
        Me.txtLog.TabIndex = 2
        '
        'WSGBatchUpload
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1508, 662)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.browser)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "WSGBatchUpload"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "WSGBatchUpload"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents browser As CefSharp.WinForms.ChromiumWebBrowser
    Friend WithEvents txtLog As TextBox

End Class
