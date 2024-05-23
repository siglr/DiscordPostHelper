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
        Me.btnViewInLibrary = New System.Windows.Forms.Button()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.chkNot = New System.Windows.Forms.CheckBox()
        Me.btnSearchBack = New System.Windows.Forms.Button()
        Me.btnResetSearch = New System.Windows.Forms.Button()
        Me.lblSearchTerms = New System.Windows.Forms.Label()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.FilterBoxContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.RidgeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ThermalsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WavesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DynamicToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddOnsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.btnUpdateLocalDB = New System.Windows.Forms.Button()
        Me.lblOnlineDBTimestamp = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnRetrieveOnlineDBTimestamp = New System.Windows.Forms.Button()
        Me.lblLocalDBTimestamp = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.splitMain = New System.Windows.Forms.SplitContainer()
        Me.gridCurrentDatabase = New System.Windows.Forms.DataGridView()
        Me.TasksGridContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.splitRightPart = New System.Windows.Forms.SplitContainer()
        Me.txtBriefing = New System.Windows.Forms.RichTextBox()
        Me.splitImages = New System.Windows.Forms.SplitContainer()
        Me.imgMap = New System.Windows.Forms.PictureBox()
        Me.imgCover = New System.Windows.Forms.PictureBox()
        Me.TextCriteriaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.textCriteriaWords = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.NumbersCriteriaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.numbersCriteriaFromTo = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.FilterBoxContextMenu.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitMain.Panel1.SuspendLayout()
        Me.splitMain.Panel2.SuspendLayout()
        Me.splitMain.SuspendLayout()
        CType(Me.gridCurrentDatabase, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TasksGridContextMenu.SuspendLayout()
        CType(Me.splitRightPart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitRightPart.Panel1.SuspendLayout()
        Me.splitRightPart.Panel2.SuspendLayout()
        Me.splitRightPart.SuspendLayout()
        CType(Me.splitImages, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitImages.Panel1.SuspendLayout()
        Me.splitImages.Panel2.SuspendLayout()
        Me.splitImages.SuspendLayout()
        CType(Me.imgMap, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.imgCover, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnViewInLibrary)
        Me.Panel1.Controls.Add(Me.OK_Button)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 723)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1118, 44)
        Me.Panel1.TabIndex = 2
        '
        'btnViewInLibrary
        '
        Me.btnViewInLibrary.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnViewInLibrary.Location = New System.Drawing.Point(883, 4)
        Me.btnViewInLibrary.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnViewInLibrary.Name = "btnViewInLibrary"
        Me.btnViewInLibrary.Size = New System.Drawing.Size(126, 35)
        Me.btnViewInLibrary.TabIndex = 1
        Me.btnViewInLibrary.Text = "Library thread"
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Location = New System.Drawing.Point(1017, 4)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(97, 35)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "Close"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Panel4)
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1118, 47)
        Me.Panel2.TabIndex = 3
        '
        'Panel4
        '
        Me.Panel4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel4.Controls.Add(Me.chkNot)
        Me.Panel4.Controls.Add(Me.btnSearchBack)
        Me.Panel4.Controls.Add(Me.btnResetSearch)
        Me.Panel4.Controls.Add(Me.lblSearchTerms)
        Me.Panel4.Controls.Add(Me.txtSearch)
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(665, 44)
        Me.Panel4.TabIndex = 10
        '
        'chkNot
        '
        Me.chkNot.AutoSize = True
        Me.chkNot.Location = New System.Drawing.Point(164, 11)
        Me.chkNot.Name = "chkNot"
        Me.chkNot.Size = New System.Drawing.Size(53, 24)
        Me.chkNot.TabIndex = 11
        Me.chkNot.Text = "Not"
        Me.chkNot.UseVisualStyleBackColor = True
        '
        'btnSearchBack
        '
        Me.btnSearchBack.Location = New System.Drawing.Point(85, 5)
        Me.btnSearchBack.Name = "btnSearchBack"
        Me.btnSearchBack.Size = New System.Drawing.Size(73, 35)
        Me.btnSearchBack.TabIndex = 10
        Me.btnSearchBack.Text = "Back"
        Me.btnSearchBack.UseVisualStyleBackColor = True
        '
        'btnResetSearch
        '
        Me.btnResetSearch.Location = New System.Drawing.Point(6, 5)
        Me.btnResetSearch.Name = "btnResetSearch"
        Me.btnResetSearch.Size = New System.Drawing.Size(73, 35)
        Me.btnResetSearch.TabIndex = 7
        Me.btnResetSearch.Text = "Reset"
        Me.btnResetSearch.UseVisualStyleBackColor = True
        '
        'lblSearchTerms
        '
        Me.lblSearchTerms.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSearchTerms.Location = New System.Drawing.Point(353, 12)
        Me.lblSearchTerms.Name = "lblSearchTerms"
        Me.lblSearchTerms.Size = New System.Drawing.Size(303, 24)
        Me.lblSearchTerms.TabIndex = 9
        '
        'txtSearch
        '
        Me.txtSearch.ContextMenuStrip = Me.FilterBoxContextMenu
        Me.txtSearch.Location = New System.Drawing.Point(223, 9)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(124, 27)
        Me.txtSearch.TabIndex = 8
        '
        'FilterBoxContextMenu
        '
        Me.FilterBoxContextMenu.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.FilterBoxContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.TextCriteriaToolStripMenuItem, Me.NumbersCriteriaToolStripMenuItem})
        Me.FilterBoxContextMenu.Name = "FilterBoxContextMenu"
        Me.FilterBoxContextMenu.Size = New System.Drawing.Size(199, 101)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RidgeToolStripMenuItem, Me.ThermalsToolStripMenuItem, Me.WavesToolStripMenuItem, Me.DynamicToolStripMenuItem, Me.AddOnsToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(198, 24)
        Me.ToolStripMenuItem1.Text = "Boolean"
        '
        'RidgeToolStripMenuItem
        '
        Me.RidgeToolStripMenuItem.Name = "RidgeToolStripMenuItem"
        Me.RidgeToolStripMenuItem.Size = New System.Drawing.Size(206, 24)
        Me.RidgeToolStripMenuItem.Tag = ""
        Me.RidgeToolStripMenuItem.Text = "Ridge"
        '
        'ThermalsToolStripMenuItem
        '
        Me.ThermalsToolStripMenuItem.Name = "ThermalsToolStripMenuItem"
        Me.ThermalsToolStripMenuItem.Size = New System.Drawing.Size(206, 24)
        Me.ThermalsToolStripMenuItem.Text = "Thermals"
        '
        'WavesToolStripMenuItem
        '
        Me.WavesToolStripMenuItem.Name = "WavesToolStripMenuItem"
        Me.WavesToolStripMenuItem.Size = New System.Drawing.Size(206, 24)
        Me.WavesToolStripMenuItem.Text = "Waves"
        '
        'DynamicToolStripMenuItem
        '
        Me.DynamicToolStripMenuItem.Name = "DynamicToolStripMenuItem"
        Me.DynamicToolStripMenuItem.Size = New System.Drawing.Size(206, 24)
        Me.DynamicToolStripMenuItem.Text = "Dynamic"
        '
        'AddOnsToolStripMenuItem
        '
        Me.AddOnsToolStripMenuItem.Name = "AddOnsToolStripMenuItem"
        Me.AddOnsToolStripMenuItem.Size = New System.Drawing.Size(206, 24)
        Me.AddOnsToolStripMenuItem.Text = "Add-Ons"
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
        Me.Panel3.Location = New System.Drawing.Point(671, 3)
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
        'splitMain
        '
        Me.splitMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitMain.Location = New System.Drawing.Point(0, 47)
        Me.splitMain.Name = "splitMain"
        '
        'splitMain.Panel1
        '
        Me.splitMain.Panel1.Controls.Add(Me.gridCurrentDatabase)
        '
        'splitMain.Panel2
        '
        Me.splitMain.Panel2.Controls.Add(Me.splitRightPart)
        Me.splitMain.Size = New System.Drawing.Size(1118, 676)
        Me.splitMain.SplitterDistance = 800
        Me.splitMain.TabIndex = 4
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
        Me.gridCurrentDatabase.Size = New System.Drawing.Size(800, 676)
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
        'splitRightPart
        '
        Me.splitRightPart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitRightPart.Location = New System.Drawing.Point(0, 0)
        Me.splitRightPart.Name = "splitRightPart"
        Me.splitRightPart.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitRightPart.Panel1
        '
        Me.splitRightPart.Panel1.Controls.Add(Me.txtBriefing)
        '
        'splitRightPart.Panel2
        '
        Me.splitRightPart.Panel2.Controls.Add(Me.splitImages)
        Me.splitRightPart.Size = New System.Drawing.Size(314, 676)
        Me.splitRightPart.SplitterDistance = 270
        Me.splitRightPart.TabIndex = 0
        '
        'txtBriefing
        '
        Me.txtBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBriefing.Font = New System.Drawing.Font("Segoe UI Emoji", 15.7!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBriefing.Location = New System.Drawing.Point(0, 0)
        Me.txtBriefing.Name = "txtBriefing"
        Me.txtBriefing.ReadOnly = True
        Me.txtBriefing.Size = New System.Drawing.Size(314, 270)
        Me.txtBriefing.TabIndex = 5
        Me.txtBriefing.Text = ""
        Me.txtBriefing.ZoomFactor = 2.0!
        '
        'splitImages
        '
        Me.splitImages.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitImages.IsSplitterFixed = True
        Me.splitImages.Location = New System.Drawing.Point(0, 0)
        Me.splitImages.Name = "splitImages"
        Me.splitImages.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitImages.Panel1
        '
        Me.splitImages.Panel1.Controls.Add(Me.imgMap)
        '
        'splitImages.Panel2
        '
        Me.splitImages.Panel2.Controls.Add(Me.imgCover)
        Me.splitImages.Size = New System.Drawing.Size(314, 402)
        Me.splitImages.SplitterDistance = 201
        Me.splitImages.SplitterWidth = 1
        Me.splitImages.TabIndex = 0
        '
        'imgMap
        '
        Me.imgMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.imgMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imgMap.Location = New System.Drawing.Point(0, 0)
        Me.imgMap.Margin = New System.Windows.Forms.Padding(0)
        Me.imgMap.Name = "imgMap"
        Me.imgMap.Size = New System.Drawing.Size(314, 201)
        Me.imgMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.imgMap.TabIndex = 1
        Me.imgMap.TabStop = False
        '
        'imgCover
        '
        Me.imgCover.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.imgCover.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imgCover.Location = New System.Drawing.Point(0, 0)
        Me.imgCover.Margin = New System.Windows.Forms.Padding(0)
        Me.imgCover.Name = "imgCover"
        Me.imgCover.Size = New System.Drawing.Size(314, 200)
        Me.imgCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.imgCover.TabIndex = 2
        Me.imgCover.TabStop = False
        '
        'TextCriteriaToolStripMenuItem
        '
        Me.TextCriteriaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.textCriteriaWords, Me.ToolStripSeparator2})
        Me.TextCriteriaToolStripMenuItem.Name = "TextCriteriaToolStripMenuItem"
        Me.TextCriteriaToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.TextCriteriaToolStripMenuItem.Text = "Text criteria"
        '
        'textCriteriaWords
        '
        Me.textCriteriaWords.Font = New System.Drawing.Font("Segoe UI", 9.163636!)
        Me.textCriteriaWords.Name = "textCriteriaWords"
        Me.textCriteriaWords.Size = New System.Drawing.Size(100, 26)
        Me.textCriteriaWords.ToolTipText = "Specify the text value to search for"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(203, 6)
        '
        'NumbersCriteriaToolStripMenuItem
        '
        Me.NumbersCriteriaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.numbersCriteriaFromTo, Me.ToolStripSeparator3})
        Me.NumbersCriteriaToolStripMenuItem.Name = "NumbersCriteriaToolStripMenuItem"
        Me.NumbersCriteriaToolStripMenuItem.Size = New System.Drawing.Size(198, 24)
        Me.NumbersCriteriaToolStripMenuItem.Text = "Numbers criteria"
        '
        'numbersCriteriaFromTo
        '
        Me.numbersCriteriaFromTo.Font = New System.Drawing.Font("Segoe UI", 9.163636!)
        Me.numbersCriteriaFromTo.Name = "numbersCriteriaFromTo"
        Me.numbersCriteriaFromTo.Size = New System.Drawing.Size(100, 26)
        Me.numbersCriteriaFromTo.ToolTipText = "Specify the minimum and maximum values from-to"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(203, 6)
        '
        'TaskBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.OK_Button
        Me.ClientSize = New System.Drawing.Size(1118, 767)
        Me.Controls.Add(Me.splitMain)
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
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.FilterBoxContextMenu.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.splitMain.Panel1.ResumeLayout(False)
        Me.splitMain.Panel2.ResumeLayout(False)
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitMain.ResumeLayout(False)
        CType(Me.gridCurrentDatabase, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TasksGridContextMenu.ResumeLayout(False)
        Me.splitRightPart.Panel1.ResumeLayout(False)
        Me.splitRightPart.Panel2.ResumeLayout(False)
        CType(Me.splitRightPart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitRightPart.ResumeLayout(False)
        Me.splitImages.Panel1.ResumeLayout(False)
        Me.splitImages.Panel2.ResumeLayout(False)
        CType(Me.splitImages, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitImages.ResumeLayout(False)
        CType(Me.imgMap, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.imgCover, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents OK_Button As Button
    Friend WithEvents Panel3 As Panel
    Friend WithEvents btnUpdateLocalDB As Button
    Friend WithEvents lblOnlineDBTimestamp As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents btnRetrieveOnlineDBTimestamp As Button
    Friend WithEvents lblLocalDBTimestamp As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents splitMain As SplitContainer
    Friend WithEvents gridCurrentDatabase As DataGridView
    Friend WithEvents TasksGridContextMenu As ContextMenuStrip
    Friend WithEvents ToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents splitRightPart As SplitContainer
    Friend WithEvents txtBriefing As RichTextBox
    Friend WithEvents splitImages As SplitContainer
    Friend WithEvents imgMap As PictureBox
    Friend WithEvents imgCover As PictureBox
    Friend WithEvents btnViewInLibrary As Button
    Friend WithEvents Panel4 As Panel
    Friend WithEvents btnResetSearch As Button
    Friend WithEvents lblSearchTerms As Label
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents btnSearchBack As Button
    Friend WithEvents FilterBoxContextMenu As ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents RidgeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ThermalsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents WavesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DynamicToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddOnsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkNot As CheckBox
    Friend WithEvents TextCriteriaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents textCriteriaWords As ToolStripTextBox
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents NumbersCriteriaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents numbersCriteriaFromTo As ToolStripTextBox
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
End Class
