<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TaskBrowser
    Inherits System.Windows.Forms.Form

    ' Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            ' Add custom dispose logic here
            If disposing Then
                RemoveHandlers()
                DisposeManagedResources()
            End If

        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub DisposeManagedResources()
        ' Dispose managed resources
        If _currentTaskDBEntries IsNot Nothing Then
            _currentTaskDBEntries.Dispose()
            _currentTaskDBEntries = Nothing
        End If
        If _filteredDataTable IsNot Nothing Then
            _filteredDataTable.Dispose()
            _filteredDataTable = Nothing
        End If
        If _searchTerms IsNot Nothing Then
            _searchTerms.Clear()
        End If
    End Sub
    ' Add a method to remove event handlers
    Private Sub RemoveHandlers()
        ' Remove event handlers
        RemoveHandler Me.Load, AddressOf TaskBrowser_Load
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
        Me.lblCurrentSelection = New System.Windows.Forms.Label()
        Me.btnDownloadOpen = New System.Windows.Forms.Button()
        Me.btnViewInLibrary = New System.Windows.Forms.Button()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.btnSmallerText = New System.Windows.Forms.Button()
        Me.btnBiggerText = New System.Windows.Forms.Button()
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
        Me.TextCriteriaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.textCriteriaWords = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.NumbersCriteriaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.numbersCriteriaFromTo = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.FavoritesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddCurrentAsFavoriteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.txtNewFavoriteTitle = New System.Windows.Forms.ToolStripTextBox()
        Me.AddNewFavoriteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.btnUpdateDB = New System.Windows.Forms.Button()
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
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel5.SuspendLayout()
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
        Me.Panel1.Controls.Add(Me.lblCurrentSelection)
        Me.Panel1.Controls.Add(Me.btnDownloadOpen)
        Me.Panel1.Controls.Add(Me.btnViewInLibrary)
        Me.Panel1.Controls.Add(Me.OK_Button)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 723)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1118, 44)
        Me.Panel1.TabIndex = 0
        '
        'lblCurrentSelection
        '
        Me.lblCurrentSelection.AutoSize = True
        Me.lblCurrentSelection.Location = New System.Drawing.Point(2, 11)
        Me.lblCurrentSelection.Name = "lblCurrentSelection"
        Me.lblCurrentSelection.Size = New System.Drawing.Size(114, 20)
        Me.lblCurrentSelection.TabIndex = 0
        Me.lblCurrentSelection.Text = "CurentSelection"
        Me.ToolTip1.SetToolTip(Me.lblCurrentSelection, "This is the currently selected task's number, Discord ID and title.")
        '
        'btnDownloadOpen
        '
        Me.btnDownloadOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDownloadOpen.Location = New System.Drawing.Point(715, 4)
        Me.btnDownloadOpen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnDownloadOpen.Name = "btnDownloadOpen"
        Me.btnDownloadOpen.Size = New System.Drawing.Size(160, 35)
        Me.btnDownloadOpen.TabIndex = 1
        Me.btnDownloadOpen.Text = "Download && Open"
        Me.ToolTip1.SetToolTip(Me.btnDownloadOpen, "Click to download (if missing locally) and open this task.")
        '
        'btnViewInLibrary
        '
        Me.btnViewInLibrary.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnViewInLibrary.Location = New System.Drawing.Point(883, 4)
        Me.btnViewInLibrary.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnViewInLibrary.Name = "btnViewInLibrary"
        Me.btnViewInLibrary.Size = New System.Drawing.Size(126, 35)
        Me.btnViewInLibrary.TabIndex = 2
        Me.btnViewInLibrary.Text = "Library thread"
        Me.ToolTip1.SetToolTip(Me.btnViewInLibrary, "Click to go to this task's thread on Discord.")
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Location = New System.Drawing.Point(1017, 4)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(97, 35)
        Me.OK_Button.TabIndex = 3
        Me.OK_Button.Text = "Close"
        Me.ToolTip1.SetToolTip(Me.OK_Button, "Click to close the Task Library browser without selecting a task.")
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Panel5)
        Me.Panel2.Controls.Add(Me.Panel4)
        Me.Panel2.Controls.Add(Me.Panel3)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1118, 47)
        Me.Panel2.TabIndex = 3
        '
        'Panel5
        '
        Me.Panel5.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel5.Controls.Add(Me.btnSmallerText)
        Me.Panel5.Controls.Add(Me.btnBiggerText)
        Me.Panel5.Location = New System.Drawing.Point(804, 3)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(202, 41)
        Me.Panel5.TabIndex = 1
        '
        'btnSmallerText
        '
        Me.btnSmallerText.Location = New System.Drawing.Point(50, 2)
        Me.btnSmallerText.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnSmallerText.Name = "btnSmallerText"
        Me.btnSmallerText.Size = New System.Drawing.Size(72, 35)
        Me.btnSmallerText.TabIndex = 0
        Me.btnSmallerText.Text = "Smaller"
        Me.ToolTip1.SetToolTip(Me.btnSmallerText, "Click to make the task details smaller.")
        '
        'btnBiggerText
        '
        Me.btnBiggerText.Location = New System.Drawing.Point(126, 2)
        Me.btnBiggerText.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnBiggerText.Name = "btnBiggerText"
        Me.btnBiggerText.Size = New System.Drawing.Size(72, 35)
        Me.btnBiggerText.TabIndex = 1
        Me.btnBiggerText.Text = "Larger"
        Me.ToolTip1.SetToolTip(Me.btnBiggerText, "Click to make the task details larger.")
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
        Me.Panel4.Size = New System.Drawing.Size(800, 44)
        Me.Panel4.TabIndex = 0
        '
        'chkNot
        '
        Me.chkNot.AutoSize = True
        Me.chkNot.Location = New System.Drawing.Point(164, 11)
        Me.chkNot.Name = "chkNot"
        Me.chkNot.Size = New System.Drawing.Size(53, 24)
        Me.chkNot.TabIndex = 2
        Me.chkNot.Text = "Not"
        Me.ToolTip1.SetToolTip(Me.chkNot, "Tasks that DO NOT have the next criteria applied will be shown.")
        Me.chkNot.UseVisualStyleBackColor = True
        '
        'btnSearchBack
        '
        Me.btnSearchBack.Location = New System.Drawing.Point(85, 5)
        Me.btnSearchBack.Name = "btnSearchBack"
        Me.btnSearchBack.Size = New System.Drawing.Size(73, 35)
        Me.btnSearchBack.TabIndex = 1
        Me.btnSearchBack.Text = "Back"
        Me.ToolTip1.SetToolTip(Me.btnSearchBack, "Click to remove the last search/filter that was added.")
        Me.btnSearchBack.UseVisualStyleBackColor = True
        '
        'btnResetSearch
        '
        Me.btnResetSearch.Location = New System.Drawing.Point(6, 5)
        Me.btnResetSearch.Name = "btnResetSearch"
        Me.btnResetSearch.Size = New System.Drawing.Size(73, 35)
        Me.btnResetSearch.TabIndex = 0
        Me.btnResetSearch.Text = "Reset"
        Me.ToolTip1.SetToolTip(Me.btnResetSearch, "Click to reset all search/filtering and show all tasks.")
        Me.btnResetSearch.UseVisualStyleBackColor = True
        '
        'lblSearchTerms
        '
        Me.lblSearchTerms.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSearchTerms.Location = New System.Drawing.Point(353, 12)
        Me.lblSearchTerms.Name = "lblSearchTerms"
        Me.lblSearchTerms.Size = New System.Drawing.Size(444, 24)
        Me.lblSearchTerms.TabIndex = 4
        '
        'txtSearch
        '
        Me.txtSearch.ContextMenuStrip = Me.FilterBoxContextMenu
        Me.txtSearch.Location = New System.Drawing.Point(223, 9)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(124, 27)
        Me.txtSearch.TabIndex = 3
        Me.ToolTip1.SetToolTip(Me.txtSearch, "Right click for a contextual menu to build your next search criteria or acces you" &
        "r favorite filters.")
        '
        'FilterBoxContextMenu
        '
        Me.FilterBoxContextMenu.ImageScalingSize = New System.Drawing.Size(18, 18)
        Me.FilterBoxContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.TextCriteriaToolStripMenuItem, Me.NumbersCriteriaToolStripMenuItem, Me.ToolStripSeparator4, Me.FavoritesToolStripMenuItem})
        Me.FilterBoxContextMenu.Name = "FilterBoxContextMenu"
        Me.FilterBoxContextMenu.Size = New System.Drawing.Size(180, 106)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RidgeToolStripMenuItem, Me.ThermalsToolStripMenuItem, Me.WavesToolStripMenuItem, Me.DynamicToolStripMenuItem, Me.AddOnsToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(179, 24)
        Me.ToolStripMenuItem1.Text = "Boolean"
        Me.ToolStripMenuItem1.ToolTipText = "Menu for all true/false fields."
        '
        'RidgeToolStripMenuItem
        '
        Me.RidgeToolStripMenuItem.Name = "RidgeToolStripMenuItem"
        Me.RidgeToolStripMenuItem.Size = New System.Drawing.Size(142, 24)
        Me.RidgeToolStripMenuItem.Tag = ""
        Me.RidgeToolStripMenuItem.Text = "Ridge"
        Me.RidgeToolStripMenuItem.ToolTipText = "Select to filter based on Ridge soaring."
        '
        'ThermalsToolStripMenuItem
        '
        Me.ThermalsToolStripMenuItem.Name = "ThermalsToolStripMenuItem"
        Me.ThermalsToolStripMenuItem.Size = New System.Drawing.Size(142, 24)
        Me.ThermalsToolStripMenuItem.Text = "Thermals"
        Me.ThermalsToolStripMenuItem.ToolTipText = "Select to filter based on Thermals soaring."
        '
        'WavesToolStripMenuItem
        '
        Me.WavesToolStripMenuItem.Name = "WavesToolStripMenuItem"
        Me.WavesToolStripMenuItem.Size = New System.Drawing.Size(142, 24)
        Me.WavesToolStripMenuItem.Text = "Waves"
        Me.WavesToolStripMenuItem.ToolTipText = "Select to filter based on Waves soaring."
        '
        'DynamicToolStripMenuItem
        '
        Me.DynamicToolStripMenuItem.Name = "DynamicToolStripMenuItem"
        Me.DynamicToolStripMenuItem.Size = New System.Drawing.Size(142, 24)
        Me.DynamicToolStripMenuItem.Text = "Dynamic"
        Me.DynamicToolStripMenuItem.ToolTipText = "Select to filter based on Dynamic soaring."
        '
        'AddOnsToolStripMenuItem
        '
        Me.AddOnsToolStripMenuItem.Name = "AddOnsToolStripMenuItem"
        Me.AddOnsToolStripMenuItem.Size = New System.Drawing.Size(142, 24)
        Me.AddOnsToolStripMenuItem.Text = "Add-Ons"
        Me.AddOnsToolStripMenuItem.ToolTipText = "Select to filter based on the task having recommended add-ons or not."
        '
        'TextCriteriaToolStripMenuItem
        '
        Me.TextCriteriaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.textCriteriaWords, Me.ToolStripSeparator2})
        Me.TextCriteriaToolStripMenuItem.Name = "TextCriteriaToolStripMenuItem"
        Me.TextCriteriaToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.TextCriteriaToolStripMenuItem.Text = "Text criteria"
        Me.TextCriteriaToolStripMenuItem.ToolTipText = "Menu for all text fields."
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
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(165, 6)
        '
        'NumbersCriteriaToolStripMenuItem
        '
        Me.NumbersCriteriaToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.numbersCriteriaFromTo, Me.ToolStripSeparator3})
        Me.NumbersCriteriaToolStripMenuItem.Name = "NumbersCriteriaToolStripMenuItem"
        Me.NumbersCriteriaToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.NumbersCriteriaToolStripMenuItem.Text = "Numbers criteria"
        Me.NumbersCriteriaToolStripMenuItem.ToolTipText = "Menu for all numbers fields."
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
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(165, 6)
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(176, 6)
        '
        'FavoritesToolStripMenuItem
        '
        Me.FavoritesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddCurrentAsFavoriteToolStripMenuItem, Me.ToolStripSeparator5})
        Me.FavoritesToolStripMenuItem.Name = "FavoritesToolStripMenuItem"
        Me.FavoritesToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.FavoritesToolStripMenuItem.Text = "Favorites"
        Me.FavoritesToolStripMenuItem.ToolTipText = "Menu to select/manage your favorite filters."
        '
        'AddCurrentAsFavoriteToolStripMenuItem
        '
        Me.AddCurrentAsFavoriteToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.txtNewFavoriteTitle, Me.AddNewFavoriteToolStripMenuItem})
        Me.AddCurrentAsFavoriteToolStripMenuItem.Name = "AddCurrentAsFavoriteToolStripMenuItem"
        Me.AddCurrentAsFavoriteToolStripMenuItem.Size = New System.Drawing.Size(226, 24)
        Me.AddCurrentAsFavoriteToolStripMenuItem.Text = "Add current as favorite"
        Me.AddCurrentAsFavoriteToolStripMenuItem.ToolTipText = "Menu to add the current filter to your favorites."
        '
        'txtNewFavoriteTitle
        '
        Me.txtNewFavoriteTitle.Font = New System.Drawing.Font("Segoe UI", 9.163636!)
        Me.txtNewFavoriteTitle.Name = "txtNewFavoriteTitle"
        Me.txtNewFavoriteTitle.Size = New System.Drawing.Size(100, 26)
        '
        'AddNewFavoriteToolStripMenuItem
        '
        Me.AddNewFavoriteToolStripMenuItem.Name = "AddNewFavoriteToolStripMenuItem"
        Me.AddNewFavoriteToolStripMenuItem.Size = New System.Drawing.Size(223, 24)
        Me.AddNewFavoriteToolStripMenuItem.Text = "Add (enter title above)"
        Me.AddNewFavoriteToolStripMenuItem.ToolTipText = "Specify a name for your favorite in the text field above and click to add."
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(223, 6)
        '
        'Panel3
        '
        Me.Panel3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel3.Controls.Add(Me.btnUpdateDB)
        Me.Panel3.Location = New System.Drawing.Point(1008, 3)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(106, 41)
        Me.Panel3.TabIndex = 2
        '
        'btnUpdateDB
        '
        Me.btnUpdateDB.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnUpdateDB.Location = New System.Drawing.Point(5, 2)
        Me.btnUpdateDB.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnUpdateDB.Name = "btnUpdateDB"
        Me.btnUpdateDB.Size = New System.Drawing.Size(97, 35)
        Me.btnUpdateDB.TabIndex = 0
        Me.btnUpdateDB.Text = "Update DB"
        Me.ToolTip1.SetToolTip(Me.btnUpdateDB, "Click to force an online update of your local database.")
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
        Me.gridCurrentDatabase.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
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
        Me.ToolStripMenuItem2.ToolTipText = "Click to unhide all columns."
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
        Me.txtBriefing.TabIndex = 0
        Me.txtBriefing.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtBriefing, "Use CTRL-MouseWheel to make the content smaller or larger.")
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
        Me.ToolTip1.SetToolTip(Me.imgMap, "Map image included with the task (if any).")
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
        Me.ToolTip1.SetToolTip(Me.imgCover, "Cover image included with the task (if any).")
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
        Me.Text = "Task Library Browser"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.FilterBoxContextMenu.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
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
    Friend WithEvents btnUpdateDB As Button
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
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents FavoritesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddCurrentAsFavoriteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents txtNewFavoriteTitle As ToolStripTextBox
    Friend WithEvents AddNewFavoriteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnDownloadOpen As Button
    Friend WithEvents lblCurrentSelection As Label
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Panel5 As Panel
    Friend WithEvents btnBiggerText As Button
    Friend WithEvents btnSmallerText As Button
End Class
