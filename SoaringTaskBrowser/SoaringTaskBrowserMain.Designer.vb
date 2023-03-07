<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSoaringTaskBrowserMain
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
        Me.tabCtrlMain = New System.Windows.Forms.TabControl()
        Me.tabMain = New System.Windows.Forms.TabPage()
        Me.tabSettings = New System.Windows.Forms.TabPage()
        Me.grpFileTypesAndFolders = New System.Windows.Forms.GroupBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.tabCtrlMain.SuspendLayout()
        Me.tabSettings.SuspendLayout()
        Me.grpFileTypesAndFolders.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabCtrlMain
        '
        Me.tabCtrlMain.Controls.Add(Me.tabMain)
        Me.tabCtrlMain.Controls.Add(Me.tabSettings)
        Me.tabCtrlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabCtrlMain.Location = New System.Drawing.Point(0, 0)
        Me.tabCtrlMain.Name = "tabCtrlMain"
        Me.tabCtrlMain.SelectedIndex = 0
        Me.tabCtrlMain.Size = New System.Drawing.Size(1158, 823)
        Me.tabCtrlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabCtrlMain.TabIndex = 0
        '
        'tabMain
        '
        Me.tabMain.AutoScroll = True
        Me.tabMain.AutoScrollMinSize = New System.Drawing.Size(640, 480)
        Me.tabMain.Location = New System.Drawing.Point(4, 29)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.Padding = New System.Windows.Forms.Padding(3)
        Me.tabMain.Size = New System.Drawing.Size(1150, 790)
        Me.tabMain.TabIndex = 0
        Me.tabMain.Text = "Main"
        Me.tabMain.UseVisualStyleBackColor = True
        '
        'tabSettings
        '
        Me.tabSettings.AutoScroll = True
        Me.tabSettings.AutoScrollMinSize = New System.Drawing.Size(640, 480)
        Me.tabSettings.Controls.Add(Me.grpFileTypesAndFolders)
        Me.tabSettings.Location = New System.Drawing.Point(4, 29)
        Me.tabSettings.Name = "tabSettings"
        Me.tabSettings.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSettings.Size = New System.Drawing.Size(1150, 790)
        Me.tabSettings.TabIndex = 1
        Me.tabSettings.Text = "Settings"
        Me.tabSettings.UseVisualStyleBackColor = True
        '
        'grpFileTypesAndFolders
        '
        Me.grpFileTypesAndFolders.Controls.Add(Me.SplitContainer1)
        Me.grpFileTypesAndFolders.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpFileTypesAndFolders.Location = New System.Drawing.Point(3, 3)
        Me.grpFileTypesAndFolders.Name = "grpFileTypesAndFolders"
        Me.grpFileTypesAndFolders.Size = New System.Drawing.Size(1144, 289)
        Me.grpFileTypesAndFolders.TabIndex = 0
        Me.grpFileTypesAndFolders.TabStop = False
        Me.grpFileTypesAndFolders.Text = "File types and destinations"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 23)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Button1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.AutoScroll = True
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListBox1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1138, 263)
        Me.SplitContainer1.SplitterDistance = 250
        Me.SplitContainer1.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(3, 14)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(244, 34)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ListBox1
        '
        Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.ItemHeight = 20
        Me.ListBox1.Items.AddRange(New Object() {"Test" & Global.Microsoft.VisualBasic.ChrW(9) & "Hello", "Test2" & Global.Microsoft.VisualBasic.ChrW(9) & "Nono"})
        Me.ListBox1.Location = New System.Drawing.Point(0, 0)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(884, 263)
        Me.ListBox1.TabIndex = 0
        '
        'frmSoaringTaskBrowserMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1158, 823)
        Me.Controls.Add(Me.tabCtrlMain)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmSoaringTaskBrowserMain"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Soaring Task Browser"
        Me.tabCtrlMain.ResumeLayout(False)
        Me.tabSettings.ResumeLayout(False)
        Me.grpFileTypesAndFolders.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabCtrlMain As TabControl
    Friend WithEvents tabMain As TabPage
    Friend WithEvents tabSettings As TabPage
    Friend WithEvents grpFileTypesAndFolders As GroupBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents Button1 As Button
    Friend WithEvents ListBox1 As ListBox
End Class
