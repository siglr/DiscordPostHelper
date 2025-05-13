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
        browser = New CefSharp.WinForms.ChromiumWebBrowser()
        txtLog = New TextBox()
        lblProgress = New Label()
        SuspendLayout()
        ' 
        ' browser
        ' 
        browser.ActivateBrowserOnCreation = False
        browser.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        browser.Location = New Point(12, 12)
        browser.Name = "browser"
        browser.Size = New Size(1483, 943)
        browser.TabIndex = 0
        ' 
        ' txtLog
        ' 
        txtLog.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Right
        txtLog.Location = New Point(1501, 12)
        txtLog.Multiline = True
        txtLog.Name = "txtLog"
        txtLog.ReadOnly = True
        txtLog.ScrollBars = ScrollBars.Both
        txtLog.Size = New Size(497, 943)
        txtLog.TabIndex = 2
        ' 
        ' lblProgress
        ' 
        lblProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        lblProgress.AutoSize = True
        lblProgress.BackColor = Color.White
        lblProgress.BorderStyle = BorderStyle.FixedSingle
        lblProgress.Font = New Font("Segoe UI", 40F, FontStyle.Bold)
        lblProgress.Location = New Point(32, 850)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(172, 86)
        lblProgress.TabIndex = 3
        lblProgress.Text = "1/10"
        lblProgress.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' WSGBatchUpload
        ' 
        AutoScaleDimensions = New SizeF(8F, 19F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(2010, 967)
        Controls.Add(lblProgress)
        Controls.Add(txtLog)
        Controls.Add(browser)
        Name = "WSGBatchUpload"
        StartPosition = FormStartPosition.CenterParent
        Text = "WSGBatchUpload"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents browser As CefSharp.WinForms.ChromiumWebBrowser
    Friend WithEvents txtLog As TextBox
    Friend WithEvents lblProgress As Label

End Class
