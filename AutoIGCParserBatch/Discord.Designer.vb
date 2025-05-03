<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDiscord
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
        txtLog = New TextBox()
        browser = New CefSharp.WinForms.ChromiumWebBrowser()
        txtDiscordThreadURL = New TextBox()
        btnGo = New Button()
        btnStart = New Button()
        btnUpload = New Button()
        btnStop = New Button()
        lblProgress = New Label()
        SuspendLayout()
        ' 
        ' txtLog
        ' 
        txtLog.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Right
        txtLog.Location = New Point(1752, 12)
        txtLog.Multiline = True
        txtLog.Name = "txtLog"
        txtLog.ReadOnly = True
        txtLog.ScrollBars = ScrollBars.Both
        txtLog.Size = New Size(319, 1084)
        txtLog.TabIndex = 4
        ' 
        ' browser
        ' 
        browser.ActivateBrowserOnCreation = False
        browser.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        browser.Location = New Point(12, 43)
        browser.Name = "browser"
        browser.Size = New Size(1734, 1053)
        browser.TabIndex = 3
        ' 
        ' txtDiscordThreadURL
        ' 
        txtDiscordThreadURL.Location = New Point(12, 11)
        txtDiscordThreadURL.Name = "txtDiscordThreadURL"
        txtDiscordThreadURL.Size = New Size(428, 26)
        txtDiscordThreadURL.TabIndex = 5
        ' 
        ' btnGo
        ' 
        btnGo.Location = New Point(446, 10)
        btnGo.Name = "btnGo"
        btnGo.Size = New Size(132, 26)
        btnGo.TabIndex = 6
        btnGo.Text = "Paste && Go"
        btnGo.UseVisualStyleBackColor = True
        ' 
        ' btnStart
        ' 
        btnStart.Location = New Point(584, 10)
        btnStart.Name = "btnStart"
        btnStart.Size = New Size(86, 26)
        btnStart.TabIndex = 7
        btnStart.Text = "Get IGCs"
        btnStart.UseVisualStyleBackColor = True
        ' 
        ' btnUpload
        ' 
        btnUpload.Location = New Point(789, 10)
        btnUpload.Name = "btnUpload"
        btnUpload.Size = New Size(155, 26)
        btnUpload.TabIndex = 8
        btnUpload.Text = "Process && Upload"
        btnUpload.UseVisualStyleBackColor = True
        ' 
        ' btnStop
        ' 
        btnStop.Enabled = False
        btnStop.Location = New Point(676, 10)
        btnStop.Name = "btnStop"
        btnStop.Size = New Size(107, 26)
        btnStop.TabIndex = 9
        btnStop.Text = "Stop scraping"
        btnStop.UseVisualStyleBackColor = True
        ' 
        ' lblProgress
        ' 
        lblProgress.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        lblProgress.AutoSize = True
        lblProgress.BackColor = Color.White
        lblProgress.BorderStyle = BorderStyle.FixedSingle
        lblProgress.Font = New Font("Segoe UI", 40F, FontStyle.Bold)
        lblProgress.Location = New Point(30, 990)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(71, 86)
        lblProgress.TabIndex = 10
        lblProgress.Tag = " "
        lblProgress.Text = "  "
        lblProgress.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' frmDiscord
        ' 
        AutoScaleDimensions = New SizeF(8F, 19F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(2083, 1108)
        Controls.Add(lblProgress)
        Controls.Add(btnStop)
        Controls.Add(btnUpload)
        Controls.Add(btnStart)
        Controls.Add(btnGo)
        Controls.Add(txtDiscordThreadURL)
        Controls.Add(txtLog)
        Controls.Add(browser)
        Name = "frmDiscord"
        Text = "frmDiscord"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtLog As TextBox
    Friend WithEvents browser As CefSharp.WinForms.ChromiumWebBrowser
    Friend WithEvents txtDiscordThreadURL As TextBox
    Friend WithEvents btnGo As Button
    Friend WithEvents btnStart As Button
    Friend WithEvents btnUpload As Button
    Friend WithEvents btnStop As Button
    Friend WithEvents lblProgress As Label
End Class
