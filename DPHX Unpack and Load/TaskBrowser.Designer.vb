<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TaskBrowser
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.btnUpdateLocalDB = New System.Windows.Forms.Button()
        Me.lblOnlineDBTimestamp = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnRetrieveOnlineDBTimestamp = New System.Windows.Forms.Button()
        Me.lblLocalDBTimestamp = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.gridCurrentDatabase = New System.Windows.Forms.DataGridView()
        Me.TasksGridContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.txtBriefing = New System.Windows.Forms.RichTextBox()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.gridCurrentDatabase, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TasksGridContextMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.OK_Button)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 648)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1067, 44)
        Me.Panel1.TabIndex = 2
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Location = New System.Drawing.Point(966, 4)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(97, 35)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1067, 48)
        Me.Panel2.TabIndex = 3
        '
        'Panel3
        '
        Me.Panel3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel3.Controls.Add(Me.btnUpdateLocalDB)
        Me.Panel3.Controls.Add(Me.lblOnlineDBTimestamp)
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.btnRetrieveOnlineDBTimestamp)
        Me.Panel3.Controls.Add(Me.lblLocalDBTimestamp)
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Location = New System.Drawing.Point(620, 3)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(443, 41)
        Me.Panel3.TabIndex = 6
        '
        'btnUpdateLocalDB
        '
        Me.btnUpdateLocalDB.Enabled = False
        Me.btnUpdateLocalDB.Location = New System.Drawing.Point(333, 3)
        Me.btnUpdateLocalDB.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnUpdateLocalDB.Name = "btnUpdateLocalDB"
        Me.btnUpdateLocalDB.Size = New System.Drawing.Size(107, 35)
        Me.btnUpdateLocalDB.TabIndex = 11
        Me.btnUpdateLocalDB.Text = "Download DB"
        '
        'lblOnlineDBTimestamp
        '
        Me.lblOnlineDBTimestamp.Location = New System.Drawing.Point(62, 20)
        Me.lblOnlineDBTimestamp.Name = "lblOnlineDBTimestamp"
        Me.lblOnlineDBTimestamp.Size = New System.Drawing.Size(159, 20)
        Me.lblOnlineDBTimestamp.TabIndex = 10
        Me.lblOnlineDBTimestamp.Text = "None"
        Me.lblOnlineDBTimestamp.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 20)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Online:"
        '
        'btnRetrieveOnlineDBTimestamp
        '
        Me.btnRetrieveOnlineDBTimestamp.Location = New System.Drawing.Point(228, 3)
        Me.btnRetrieveOnlineDBTimestamp.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnRetrieveOnlineDBTimestamp.Name = "btnRetrieveOnlineDBTimestamp"
        Me.btnRetrieveOnlineDBTimestamp.Size = New System.Drawing.Size(97, 35)
        Me.btnRetrieveOnlineDBTimestamp.TabIndex = 8
        Me.btnRetrieveOnlineDBTimestamp.Text = "Check"
        '
        'lblLocalDBTimestamp
        '
        Me.lblLocalDBTimestamp.Location = New System.Drawing.Point(62, 0)
        Me.lblLocalDBTimestamp.Name = "lblLocalDBTimestamp"
        Me.lblLocalDBTimestamp.Size = New System.Drawing.Size(159, 20)
        Me.lblLocalDBTimestamp.TabIndex = 7
        Me.lblLocalDBTimestamp.Text = "None"
        Me.lblLocalDBTimestamp.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(46, 20)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Local:"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 48)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.gridCurrentDatabase)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtBriefing)
        Me.SplitContainer1.Size = New System.Drawing.Size(1067, 600)
        Me.SplitContainer1.SplitterDistance = 600
        Me.SplitContainer1.TabIndex = 4
        '
        'gridCurrentDatabase
        '
        Me.gridCurrentDatabase.AllowUserToAddRows = False
        Me.gridCurrentDatabase.AllowUserToDeleteRows = False
        Me.gridCurrentDatabase.AllowUserToOrderColumns = True
        Me.gridCurrentDatabase.AllowUserToResizeRows = False
        Me.gridCurrentDatabase.ColumnHeadersHeight = 35
        Me.gridCurrentDatabase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.gridCurrentDatabase.ContextMenuStrip = Me.TasksGridContextMenu
        Me.gridCurrentDatabase.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridCurrentDatabase.Location = New System.Drawing.Point(0, 0)
        Me.gridCurrentDatabase.MultiSelect = False
        Me.gridCurrentDatabase.Name = "gridCurrentDatabase"
        Me.gridCurrentDatabase.RowHeadersVisible = False
        Me.gridCurrentDatabase.RowHeadersWidth = 47
        Me.gridCurrentDatabase.RowTemplate.Height = 35
        Me.gridCurrentDatabase.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.gridCurrentDatabase.Size = New System.Drawing.Size(600, 600)
        Me.gridCurrentDatabase.TabIndex = 0
        '
        'TasksGridContextMenu
        '
        Me.TasksGridContextMenu.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.TasksGridContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem2, Me.ToolStripSeparator1})
        Me.TasksGridContextMenu.Name = "TasksGridContextMenu"
        Me.TasksGridContextMenu.Size = New System.Drawing.Size(176, 34)
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(175, 24)
        Me.ToolStripMenuItem2.Text = "Reset (Show All)"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(172, 6)
        '
        'txtBriefing
        '
        Me.txtBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBriefing.Font = New System.Drawing.Font("Segoe UI Emoji", 15.7!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBriefing.Location = New System.Drawing.Point(0, 0)
        Me.txtBriefing.Name = "txtBriefing"
        Me.txtBriefing.ReadOnly = True
        Me.txtBriefing.Size = New System.Drawing.Size(463, 600)
        Me.txtBriefing.TabIndex = 5
        Me.txtBriefing.Text = ""
        Me.txtBriefing.ZoomFactor = 2.0!
        '
        'TaskBrowser
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.OK_Button
        Me.ClientSize = New System.Drawing.Size(1067, 692)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(648, 622)
        Me.Name = "TaskBrowser"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Task Browser"
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.gridCurrentDatabase, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TasksGridContextMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents OK_Button As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents btnUpdateLocalDB As Button
    Friend WithEvents lblOnlineDBTimestamp As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents btnRetrieveOnlineDBTimestamp As Button
    Friend WithEvents lblLocalDBTimestamp As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents gridCurrentDatabase As DataGridView
    Friend WithEvents txtBriefing As RichTextBox
    Friend WithEvents TasksGridContextMenu As ContextMenuStrip
    Friend WithEvents ToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
End Class
