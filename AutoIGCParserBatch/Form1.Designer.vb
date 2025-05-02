<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
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
        btnSelectFolder = New Button()
        txtLog = New TextBox()
        SuspendLayout()
        ' 
        ' browser
        ' 
        browser.ActivateBrowserOnCreation = False
        browser.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        browser.Location = New Point(12, 44)
        browser.Name = "browser"
        browser.Size = New Size(1483, 911)
        browser.TabIndex = 0
        ' 
        ' btnSelectFolder
        ' 
        btnSelectFolder.Location = New Point(12, 12)
        btnSelectFolder.Name = "btnSelectFolder"
        btnSelectFolder.Size = New Size(131, 26)
        btnSelectFolder.TabIndex = 1
        btnSelectFolder.Text = "Select Folder"
        btnSelectFolder.UseVisualStyleBackColor = True
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
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 19F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(2010, 967)
        Controls.Add(txtLog)
        Controls.Add(btnSelectFolder)
        Controls.Add(browser)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents browser As CefSharp.WinForms.ChromiumWebBrowser
    Friend WithEvents btnSelectFolder As Button
    Friend WithEvents txtLog As TextBox

End Class
