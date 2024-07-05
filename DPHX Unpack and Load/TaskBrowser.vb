Imports System.ComponentModel
Imports System.Data.SQLite
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles
Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Public Class TaskBrowser

#Region "Constants and variables"

    Public DownloadedFilePath As String = String.Empty
    Public OpenWithEntrySeqID As Integer = 0
    Private _localTasksDatabaseFilePath As String
    Private _currentTaskDBEntries As DataTable
    Private _dataGridViewAllSet As Boolean = False
    Private _selectedTaskRow As DataRow
    Private _filteredDataTable As DataTable
    Private _searchTerms As New List(Of String)
    Private scrollIndex As Integer
    Private selectedRowIndex As Integer
    Private horizontalScrollIndex As Integer
    Private prevSortColumn As String
    Private prevSortDirectionAsc As Boolean
    Private initializing As Boolean = True

    Private Shared _processingChange As Boolean = False

    Private Shared ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private Shared ReadOnly Property PrefUnits As New PreferredUnits

    Private _handlerFavoriteReplaceSearchItem_Click As New List(Of ToolStripMenuItem)
    Private _handlerFavoriteAddToSearchItem_Click As New List(Of ToolStripMenuItem)
    Private _handlerFavoriteRenameItem_Click As New List(Of ToolStripMenuItem)
    Private _handlerFavoriteDeleteItem_Click As New List(Of ToolStripMenuItem)
    Private _handlerColumnVisibilityToggle As New List(Of ToolStripMenuItem)
    Private _handlerTextCriteriaMenuItem_Click As New List(Of ToolStripMenuItem)
    Private _handlerNumberCriteriaMenuItem_Click As New List(Of ToolStripMenuItem)

#End Region

#Region "Events"

    Private Sub TaskBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        initializing = True

        InitializeWebView2()

        lblCurrentSelection.Text = String.Empty

        _localTasksDatabaseFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SupportingFeatures.TasksDatabase)

        splitMain.SplitterDistance = splitMain.Width * (Settings.SessionSettings.TaskLibrarySplitterLocation / 100)
        splitRightPart.SplitterDistance = splitRightPart.Height * (Settings.SessionSettings.TaskLibraryRightPartSplitterLocation / 100)
        If Settings.SessionSettings.TaskLibraryDetailsZoomLevel <= 0.015625 OrElse Settings.SessionSettings.TaskLibraryDetailsZoomLevel >= 64 Then
            Settings.SessionSettings.TaskLibraryDetailsZoomLevel = 1.5
        End If
        txtBriefing.ZoomFactor = Settings.SessionSettings.TaskLibraryDetailsZoomLevel

        UpdateCurrentDBGrid()

        If Settings.SessionSettings.TaskLibrarySortAsc Then
            gridCurrentDatabase.Sort(gridCurrentDatabase.Columns(Settings.SessionSettings.TaskLibrarySortColumn), ListSortDirection.Ascending)
        Else
            gridCurrentDatabase.Sort(gridCurrentDatabase.Columns(Settings.SessionSettings.TaskLibrarySortColumn), ListSortDirection.Descending)
        End If

        ' Set this form's location and size to match the parent form's
        If Me.Owner IsNot Nothing Then
            Me.Size = Me.Owner.Size
            Me.Location = Me.Owner.Location
        End If
        AddColumnsVisibilityOptions()

        AddFieldWithTextCriteria("title", "Title")
        AddFieldWithTextCriteria("mainarea", "MainAreaPOI")
        AddFieldWithTextCriteria("description", "Description")
        AddFieldWithTextCriteria("depname", "Departure name")
        AddFieldWithTextCriteria("depicao", "Departure ICAO")
        AddFieldWithTextCriteria("arrname", "Arrival name")
        AddFieldWithTextCriteria("arricao", "Arrival ICAO")
        AddFieldWithTextCriteria("deparrname", "Departure or Arrival name")
        AddFieldWithTextCriteria("deparricao", "Departure or Arrival ICAO")
        AddFieldWithTextCriteria("gliders", "Recommended gliders")
        AddFieldWithTextCriteria("diff", "Difficulty rating")
        AddFieldWithTextCriteria("countries", "Countries")
        AddFieldWithTextCriteria("weather", "Weather")
        AddFieldWithTextCriteria("credits", "Credits")
        AddFieldWithTextCriteria("description", "Descriptions")
        AddFieldWithTextCriteria("tags", "Tags")
        AddFieldWithTextCriteria("comment", "Comment")
        AddFieldWithTextCriteria("global", "Everywhere")
        AddFieldWithNumbersCriteria("duration", "Duration")
        AddFieldWithNumbersCriteria("distance", "Distance (in KM)")
        AddFieldWithNumbersCriteria("totdown", "Total downloads")
        AddFieldWithNumbersCriteria("yq", "Your quality rating")
        AddFieldWithNumbersCriteria("yd", "Your difficulty rating")

        BuildFavoritesMenu()

        CheckForUpdates(False)

        If OpenWithEntrySeqID <> 0 Then
            PerformSearch($"%nbr({OpenWithEntrySeqID.ToString})")
        End If

        initializing = False

        'Set focus on grid
        Me.ActiveControl = gridCurrentDatabase

    End Sub

    Private Sub btnSmallerText_Click(sender As Object, e As EventArgs) Handles btnSmallerText.Click
        If Not (txtBriefing.ZoomFactor - 0.1) <= 0.015625 Then
            txtBriefing.ZoomFactor = txtBriefing.ZoomFactor - 0.1
        End If
    End Sub
    Private Sub btnBiggerText_Click(sender As Object, e As EventArgs) Handles btnBiggerText.Click
        If Not (txtBriefing.ZoomFactor + 0.1) >= 64 Then
            txtBriefing.ZoomFactor = txtBriefing.ZoomFactor + 0.1
        End If
    End Sub

    Private Sub btnDownloadOpen_Click(sender As Object, e As EventArgs) Handles btnDownloadOpen.Click

        If btnDownloadOpen.Text = "Open" Then
            'Already exists locally
            DownloadedFilePath = LocalDPHXFileName
        Else
            'Download
            Dim selTaskSeqID As String = _selectedTaskRow("EntrySeqID")
            DownloadedFilePath = DownloadTaskFile(_selectedTaskRow("TaskID").ToString(), _selectedTaskRow("Title").ToString(), Settings.SessionSettings.PackagesFolder)
            'Call the script to increment the download count by 1
            If IncrementDownloadForTask(selTaskSeqID) Then
                'Success
            End If
        End If

        If Not String.IsNullOrEmpty(DownloadedFilePath) Then
            Me.Close()
        End If

    End Sub

    Private Sub btnCopyLinkToWeSimGlide_Click(sender As Object, e As EventArgs) Handles btnCopyLinkToWeSimGlide.Click
        Dim textToCopy As String = String.Empty
        textToCopy = $"{SupportingFeatures.GetWeSimGlideTaskURL(_selectedTaskRow("EntrySeqID"))}"
        Clipboard.SetText(textToCopy)
        Using New Centered_MessageBox()
            MessageBox.Show(Me, "The link to this task has been copied to your clipboard!", "Sharing task link", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    Private Sub AddNewFavoriteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewFavoriteToolStripMenuItem.Click

        txtNewFavoriteTitle.Text = txtNewFavoriteTitle.Text.Replace("&", "")

        If txtNewFavoriteTitle.Text.Trim = String.Empty Then
            Using New Centered_MessageBox()
                MessageBox.Show(Me, "No title specified for the favorite!", "Adding new search favorite", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        If _searchTerms.Count = 0 Then
            Using New Centered_MessageBox()
                MessageBox.Show(Me, "Please build your search criteria first!", "Adding new search favorite", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        'check if title already exists
        If Settings.SessionSettings.FavoriteSearches.Keys.Contains(txtNewFavoriteTitle.Text.Trim) Then
            Using New Centered_MessageBox()
                If MessageBox.Show(Me, "This title already exists in your favorites! Do you want to replace it?", "Adding new search favorite", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    txtNewFavoriteTitle.Text = String.Empty
                    Exit Sub
                End If
            End Using
            'Replace the existing favorite
            Settings.SessionSettings.FavoriteSearches(txtNewFavoriteTitle.Text.Trim).Clear()
            For Each term As String In _searchTerms
                Settings.SessionSettings.FavoriteSearches(txtNewFavoriteTitle.Text.Trim).Add(term)
            Next
        Else
            'Add new favorite
            Settings.SessionSettings.FavoriteSearches.Add(txtNewFavoriteTitle.Text.Trim, New List(Of String))
            For Each term As String In _searchTerms
                Settings.SessionSettings.FavoriteSearches(txtNewFavoriteTitle.Text.Trim).Add(term)
            Next
            'Add to the contextual menu
            BuildFavoritesMenu()
            txtNewFavoriteTitle.Text = String.Empty
        End If

    End Sub

    Private Sub NumberCriteriaMenuItem_Click(sender As Object, e As EventArgs)

        If numbersCriteriaFromTo.Text.Trim = String.Empty Then
            Exit Sub
        End If

        Dim menuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim fieldName As String = menuItem.Tag.ToString().ToLower
        Dim criteria As String = menuItem.Text
        Dim fieldSymbol As String = "%"

        ' Handle the criteria selection here
        Dim searchTerm As String = $"({numbersCriteriaFromTo.Text.Trim.ToLower})"

        If chkNot.Checked Then
            txtSearch.Text = $"!{fieldSymbol}{fieldName}{searchTerm}"
        Else
            txtSearch.Text = $"{fieldSymbol}{fieldName}{searchTerm}"
        End If
        ProcessSearch(txtSearch.Text.Trim, True)
        txtSearch.Text = String.Empty
        numbersCriteriaFromTo.Text = String.Empty

    End Sub

    Private Sub FavoriteReplaceSearchItem_Click(sender As Object, e As EventArgs)

        btnResetSearch_Click(sender, e)
        FavoriteAddToSearchItem_Click(sender, e)

    End Sub

    Private Sub FavoriteAddToSearchItem_Click(sender As Object, e As EventArgs)

        Dim txtFavoriteTitle As String = sender.Tag

        For Each termToAdd As String In Settings.SessionSettings.FavoriteSearches(txtFavoriteTitle)
            ProcessSearch(termToAdd, True)
        Next

    End Sub

    Private Sub FavoriteRenameItem_Click(sender As Object, e As EventArgs)

        Dim txtRenameFavorite As ToolStripTextBox = sender.tag
        Dim txtCurrentFavoriteTitle As String = txtRenameFavorite.Tag

        If txtRenameFavorite.Text.Trim = String.Empty Then
            Using New Centered_MessageBox()
                MessageBox.Show(Me, "No new title specified for the favorite!", "Renaming favorite", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Exit Sub
        End If

        txtRenameFavorite.Text = txtRenameFavorite.Text.Replace("&", "")

        'check if title already exists
        If Settings.SessionSettings.FavoriteSearches.Keys.Contains(txtRenameFavorite.Text.Trim) Then
            Using New Centered_MessageBox()
                MessageBox.Show(Me, "This title already exists in your favorites!", "Renaming favorite", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtRenameFavorite.Text = String.Empty
                Exit Sub
            End Using
        End If

        'Rename favorite
        Settings.SessionSettings.FavoriteSearches.Add(txtRenameFavorite.Text.Trim, Settings.SessionSettings.FavoriteSearches(txtCurrentFavoriteTitle))
        Settings.SessionSettings.FavoriteSearches.Remove(txtCurrentFavoriteTitle)
        txtRenameFavorite.Text = String.Empty

        BuildFavoritesMenu()

    End Sub

    Private Sub FavoriteDeleteItem_Click(sender As Object, e As EventArgs)

        Dim txtFavoriteTitle As String = sender.Tag

        Using New Centered_MessageBox()
            If MessageBox.Show(Me, $"Are you sure you want to delete your favorite ""{txtFavoriteTitle}""?", "Deleting favorite", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                'Delete the favorite
                Settings.SessionSettings.FavoriteSearches.Remove(txtFavoriteTitle)
                BuildFavoritesMenu()
            End If
        End Using

    End Sub

    Private Sub TextCriteriaMenuItem_Click(sender As Object, e As EventArgs)

        If textCriteriaWords.Text.Trim = String.Empty Then
            Exit Sub
        End If

        Dim menuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim fieldName As String = menuItem.Tag.ToString().ToLower
        Dim criteria As String = menuItem.Text
        Dim fieldSymbol As String = "%"

        ' Handle the criteria selection here
        Dim searchTerm As String = String.Empty
        Select Case criteria
            Case "Containing"
                searchTerm = $"*{textCriteriaWords.Text.Trim.ToLower}*"
            Case "Starts with"
                searchTerm = $"{textCriteriaWords.Text.Trim.ToLower}*"
            Case "Ends with"
                searchTerm = $"*{textCriteriaWords.Text.Trim.ToLower}"
            Case "Exactly"
                searchTerm = $"{textCriteriaWords.Text.Trim.ToLower}"
        End Select

        If fieldName = "global" Then
            fieldName = String.Empty
            fieldSymbol = String.Empty
        Else
            searchTerm = $"({searchTerm})"
        End If

        If chkNot.Checked Then
            txtSearch.Text = $"!{fieldSymbol}{fieldName}{searchTerm}"
        Else
            txtSearch.Text = $"{fieldSymbol}{fieldName}{searchTerm}"
        End If
        ProcessSearch(txtSearch.Text.Trim, True)
        txtSearch.Text = String.Empty
        textCriteriaWords.Text = String.Empty

    End Sub

    Private Sub BooleanToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RidgeToolStripMenuItem.Click,
                                                                                         ThermalsToolStripMenuItem.Click,
                                                                                         WavesToolStripMenuItem.Click,
                                                                                         DynamicToolStripMenuItem.Click,
                                                                                         AddOnsToolStripMenuItem.Click, ToFlyToolStripMenuItem.Click, TaskFlownToolStripMenuItem.Click
        If chkNot.Checked Then
            txtSearch.Text = $"!%{CType(sender, ToolStripMenuItem).Text.ToLower}"
        Else
            txtSearch.Text = $"%{CType(sender, ToolStripMenuItem).Text.ToLower}"
        End If
        ProcessSearch(txtSearch.Text.Trim, True)
        txtSearch.Text = String.Empty

    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        If _processingChange Then
            Exit Sub
        Else
            _processingChange = True
        End If
        If txtSearch.Text.StartsWith("!") AndAlso Not chkNot.Checked Then
            chkNot.Checked = True
        ElseIf chkNot.Checked Then
            chkNot.Checked = False
        End If
        textCriteriaWords.Text = txtSearch.Text.Replace("!", "")
        numbersCriteriaFromTo.Text = txtSearch.Text.Replace("!", "")

        _processingChange = False

    End Sub

    Private Sub chkNot_CheckedChanged(sender As Object, e As EventArgs) Handles chkNot.CheckedChanged
        If _processingChange Then
            Exit Sub
        Else
            _processingChange = True
        End If
        If chkNot.Checked Then
            If txtSearch.Text.StartsWith("!") Then
                'nothing to do
            Else
                'add the !
                txtSearch.Text = $"!{txtSearch.Text}"
            End If
        Else
            If txtSearch.Text.StartsWith("!") Then
                'remove the !
                txtSearch.Text = txtSearch.Text.Substring(1)
            Else
                'nothing to do 
            End If
        End If
        _processingChange = False
    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True ' Prevent the "ding" sound on Enter key press
            ProcessSearch(txtSearch.Text.Trim, True)
            txtSearch.Text = String.Empty
        End If
    End Sub

    Private Sub btnResetSearch_Click(sender As Object, e As EventArgs) Handles btnResetSearch.Click

        SaveCurrentPosition()
        SaveOrReapplySort(True)
        gridCurrentDatabase.DataSource = _currentTaskDBEntries
        SaveOrReapplySort(False)
        _filteredDataTable = Nothing
        lblSearchTerms.Text = String.Empty
        txtSearch.Text = String.Empty
        _searchTerms.Clear()
        RestorePosition()
        SelectTaskOnMap(_selectedTaskRow("EntrySeqID"))

    End Sub

    Private Sub btnSearchBack_Click(sender As Object, e As EventArgs) Handles btnSearchBack.Click

        Select Case _searchTerms.Count
            Case 0
                'Do nothing
            Case 1
                'Reset
                btnResetSearch_Click(sender, e)
            Case Else
                'Remove the last entry and rebuild the search with all remaining entries
                gridCurrentDatabase.DataSource = _currentTaskDBEntries
                _filteredDataTable = Nothing
                lblSearchTerms.Text = String.Empty
                txtSearch.Text = String.Empty
                _searchTerms.RemoveAt(_searchTerms.Count - 1)
                For Each searchTerm As String In _searchTerms
                    ProcessSearch(searchTerm, False)
                Next
        End Select

    End Sub

    Private Sub btnViewInLibrary_Click(sender As Object, e As EventArgs) Handles btnViewInLibrary.Click

        'Call the script to increment the Discord thread access by 1
        If IncrementThreadAccessForTask(_selectedTaskRow("EntrySeqID")) Then
            'Success
        End If

        If Not SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/{_selectedTaskRow("TaskID").ToString.Trim}") Then
            Using New Centered_MessageBox()
                MessageBox.Show("Invalid URL provided! Please specify a valid URL.", "Error launching Discord", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub ColumnVisibilityToggle(sender As Object, e As EventArgs)
        Dim menuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim linkedColumn As DataGridViewColumn = CType(menuItem.Tag, DataGridViewColumn)
        For Each column As DataGridViewColumn In gridCurrentDatabase.Columns
            If column.Tag = linkedColumn.Tag Then
                column.Visible = Not column.Visible
                menuItem.Checked = column.Visible
                Exit For
            End If
        Next
        CheckColumnSizes()
    End Sub

    Private Sub ResetShowAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click

        ' List of column names to exclude from the visibility toggle
        Dim excludeColumns As List(Of String) = GetListOfExcludedColumnNames()

        ' Set all columns visible
        For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
            If Not excludeColumns.Contains(col.Name) Then
                col.Visible = True
            End If
        Next

        ' Update the check state of each dynamic menu item
        For i As Integer = TasksGridContextMenu.Items.Count - 1 To 2 Step -1
            Dim item As ToolStripMenuItem = TasksGridContextMenu.Items(i)
            If TypeOf item Is ToolStripMenuItem AndAlso CType(item, ToolStripMenuItem).Name.StartsWith("dynamicCol_") Then
                item.Checked = True
            End If
        Next

        CheckColumnSizes()

    End Sub

    Private Sub btnUpdateDB_Click(sender As Object, e As EventArgs) Handles btnUpdateDB.Click

        CheckForUpdates(True)

    End Sub

    Private Sub TaskBrowser_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SaveDataGridViewSettings()
        RemoveEventHandlers()

        If gridCurrentDatabase IsNot Nothing Then
            gridCurrentDatabase.DataSource = Nothing
            gridCurrentDatabase.Columns.Clear()
            gridCurrentDatabase.Dispose()
            gridCurrentDatabase = Nothing
        End If

        ' Dispose of DataTables
        If _currentTaskDBEntries IsNot Nothing Then
            _currentTaskDBEntries.Clear()
            _currentTaskDBEntries.Dispose()
            _currentTaskDBEntries = Nothing
        End If

        If _filteredDataTable IsNot Nothing Then
            _filteredDataTable.Clear()
            _filteredDataTable.Dispose()
            _filteredDataTable = Nothing
        End If

        ' Dispose of images
        If imgMap.Image IsNot Nothing Then
            imgMap.Image.Dispose()
            imgMap.Image = Nothing
        End If

        If imgCover.Image IsNot Nothing Then
            imgCover.Image.Dispose()
            imgCover.Image = Nothing
        End If

        _searchTerms.Clear()

        ' Force garbage collection
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

    End Sub

    Private Sub gridCurrentDatabase_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles gridCurrentDatabase.RowEnter

        If initializing Then
            Exit Sub
        End If
        Dim selectedEntrySeqID As Integer = gridCurrentDatabase.Rows(e.RowIndex).Cells("EntrySeqID").Value
        RowEnterInGrid(selectedEntrySeqID)
    End Sub

    Private Sub gridCurrentDatabase_Resize(sender As Object, e As EventArgs) Handles gridCurrentDatabase.Resize
        CheckColumnSizes()
    End Sub

    Private Sub gridCurrentDatabase_DataSourceChanged(sender As Object, e As EventArgs) Handles gridCurrentDatabase.DataSourceChanged
        SetupDataGridView()
        Me.Text = $"Task Library Browser - {gridCurrentDatabase.Rows.Count.ToString} tasks displayed"

        If tabGridAndMap.SelectedTab Is tabMap Then
            ApplyTaskFilter()
        End If

    End Sub

    Private Sub gridCurrentDatabase_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles gridCurrentDatabase.ColumnHeaderMouseClick
        ' Get the name of the clicked column
        Dim clickedColumn As DataGridViewColumn = gridCurrentDatabase.Columns(e.ColumnIndex)

        ' Determine if the column requires custom sorting
        If clickedColumn.SortMode = DataGridViewColumnSortMode.Programmatic Then
            ' Toggle the sort order for the clicked column
            Dim sortOrder As ListSortDirection
            If clickedColumn.HeaderCell.SortGlyphDirection = Windows.Forms.SortOrder.Ascending OrElse clickedColumn.HeaderCell.SortGlyphDirection = Windows.Forms.SortOrder.None Then
                sortOrder = ListSortDirection.Descending
            Else
                sortOrder = ListSortDirection.Ascending
            End If

            SaveCurrentPosition()

            ' Perform the custom sorting
            SortGrid(clickedColumn.Name, sortOrder)

            RestorePosition()

            ' Update the sort glyph
            gridCurrentDatabase.Columns(clickedColumn.Name).HeaderCell.SortGlyphDirection = If(sortOrder = ListSortDirection.Ascending, Windows.Forms.SortOrder.Ascending, Windows.Forms.SortOrder.Descending)

        End If
    End Sub

    Private Sub chkTaskFlown_CheckedChanged(sender As Object, e As EventArgs) Handles chkTaskFlown.CheckedChanged

        pnlAllUserDataFields.Enabled = chkTaskFlown.Checked

    End Sub

    Private Sub trkQualityRating_Scroll(sender As Object, e As EventArgs) Handles trkQualityRating.ValueChanged

        lblQualityRating.Text = trkQualityRating.Value.ToString

    End Sub

    Private Sub trkDifficultyRating_Scroll(sender As Object, e As EventArgs) Handles trkDifficultyRating.ValueChanged

        lblDifficultyRating.Text = trkDifficultyRating.Value.ToString

    End Sub

    Private Sub btnUserDataSave_Click(sender As Object, e As EventArgs) Handles btnUserDataSave.Click
        SaveUserData()
    End Sub

    Private Sub webView_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs) Handles webView.WebMessageReceived
        Dim message = e.WebMessageAsJson
        If Not String.IsNullOrEmpty(message) Then
            Dim json = JObject.Parse(message)
            Dim action = json("action").ToString()
            Select Case action
                Case "selectTask"
                    Dim entrySeqID = json("entrySeqID").ToString()
                    SelectTaskOnGrid(entrySeqID)
                Case "escapeKeyPressed"
                    OK_Button.PerformClick()
            End Select
        End If
    End Sub

    Private Sub btnInstallWebView_Click(sender As Object, e As EventArgs) Handles btnInstallWebView.Click
        InitializeWebView2()
    End Sub

    Private Sub tabGridAndMap_Selected(sender As Object, e As TabControlEventArgs) Handles tabGridAndMap.Selected

        If tabGridAndMap.SelectedTab Is tabMap Then
            'Apply filter
            ApplyTaskFilter()

            'Select current task
            SelectTaskOnMap(_selectedTaskRow("EntrySeqID"))
        End If

    End Sub

    Private Sub gridCurrentDatabase_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles gridCurrentDatabase.CellMouseDoubleClick

        Select Case gridCurrentDatabase.Columns(e.ColumnIndex).Name
            Case "TaskFlownBool"
                chkTaskFlown.Checked = Not chkTaskFlown.Checked
                SaveUserData()
            Case "ToFlyBool"
                chkToFly.Checked = Not chkToFly.Checked
                SaveUserData()
        End Select

    End Sub

    Private Sub txtBriefing_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles txtBriefing.LinkClicked
        Process.Start(e.LinkText)
    End Sub

#End Region

#Region "Subs and functions"

    Private Sub ClearTaskFilterOnMap()
        If webView.CoreWebView2 IsNot Nothing Then
            webView.CoreWebView2.ExecuteScriptAsync("window.clearFilter();")
        End If
    End Sub

    Private Sub FilterTasksOnMap(entrySeqIDs As List(Of String))
        If webView.CoreWebView2 IsNot Nothing Then
            Dim script As String = $"window.filterTasks({Newtonsoft.Json.JsonConvert.SerializeObject(entrySeqIDs)});"
            webView.CoreWebView2.ExecuteScriptAsync(script)
        End If
    End Sub

    Private Sub ApplyTaskFilter()
        If _currentTaskDBEntries.Rows.Count = gridCurrentDatabase.Rows.Count Then
            ClearTaskFilterOnMap()
        Else
            Dim entrySeqIDs As New List(Of String)

            For Each row As DataGridViewRow In gridCurrentDatabase.Rows
                entrySeqIDs.Add(row.Cells("EntrySeqID").Value)
            Next

            FilterTasksOnMap(entrySeqIDs)
        End If
    End Sub

    Private Sub SelectTaskOnGrid(entrySeqID As String)
        ' Ensure the DataGridView is bound to a data source
        If gridCurrentDatabase.DataSource IsNot Nothing Then
            For i As Integer = 0 To gridCurrentDatabase.Rows.Count - 1
                Dim row As DataGridViewRow = gridCurrentDatabase.Rows(i)
                If row.DataBoundItem IsNot Nothing Then
                    ' Assuming the data item has an EntrySeqID property
                    If row.Cells("EntrySeqID").Value = entrySeqID Then
                        ' Select the row and scroll it into view
                        gridCurrentDatabase.ClearSelection()
                        row.Selected = True
                        gridCurrentDatabase.FirstDisplayedScrollingRowIndex = row.Index
                        RowEnterInGrid(entrySeqID)
                        Exit For
                    End If
                End If
            Next
        End If
    End Sub

    Public Sub SelectTaskOnMap(entrySeqID As String)

        If Not tabGridAndMap.SelectedTab Is tabMap Then
            Exit Sub
        End If

        Dim script As String = $"window.selectTask('{entrySeqID}');"
        Try
            If webView.CoreWebView2 IsNot Nothing Then
                webView.CoreWebView2.ExecuteScriptAsync(script)
            Else
                Application.DoEvents()
                webView.Refresh()
                webView.CoreWebView2.ExecuteScriptAsync(script)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub RowEnterInGrid(selectedEntrySeqID As Integer)
        _selectedTaskRow = _currentTaskDBEntries.Select($"EntrySeqID = {selectedEntrySeqID}").FirstOrDefault()
        lblCurrentSelection.Text = $"{_selectedTaskRow("EntrySeqID")} - {_selectedTaskRow("TaskID")} - {_selectedTaskRow("Title")}"
        BuildTaskData()

        'Check if the file is already present locally
        If File.Exists(LocalDPHXFileName) Then
            'Check inside to get the timestamp of the DPH file
            Dim dphUpdateStamp As String = SupportingFeatures.GetDPHLastUpdateFromDPHXFile(LocalDPHXFileName)
            If dphUpdateStamp < _selectedTaskRow("LastUpdate") Then
                btnDownloadOpen.Text = "Download && Open"
            Else
                btnDownloadOpen.Text = "Open"
            End If
        Else
            btnDownloadOpen.Text = "Download && Open"
        End If

    End Sub

    Private Sub BuildFavoritesMenu()

        'Remove all elements
        For i As Integer = FavoritesToolStripMenuItem.DropDownItems.Count - 1 To 2 Step -1
            Dim item As ToolStripItem = FavoritesToolStripMenuItem.DropDownItems(i)
            If TypeOf item Is ToolStripMenuItem Then
                FavoritesToolStripMenuItem.DropDownItems.RemoveAt(i)
            End If
        Next

        ' Get the sorted list of favorite titles
        Dim sortedFavoriteTitles = Settings.SessionSettings.FavoriteSearches.Keys.ToList()
        sortedFavoriteTitles.Sort()

        ' Add the sorted favorite titles to the menu
        For Each favTitle As String In sortedFavoriteTitles
            AddFavoriteSearch(favTitle)
        Next

    End Sub

    Private Sub SortGrid(columnName As String, direction As ListSortDirection)
        ' Create a DataView from the DataTable
        Dim dataTable As DataTable = CType(gridCurrentDatabase.DataSource, DataTable)
        Dim sortedTable As DataTable = dataTable.Clone() ' Clone the schema of the original table

        ' Sort based on the column name
        Select Case columnName
            Case "DurationConcat"
                ' Create a list of DataRows with adjusted duration values
                Dim rows = dataTable.AsEnumerable().Select(Function(row)
                                                               Dim durationMin = If(IsDBNull(row("DurationMin")), 0, Convert.ToInt32(row("DurationMin")))
                                                               Dim durationMax = If(IsDBNull(row("DurationMax")), 0, Convert.ToInt32(row("DurationMax")))

                                                               ' If only one value is present, use it for both fields
                                                               If durationMin = 0 AndAlso durationMax > 0 Then
                                                                   durationMin = durationMax
                                                               ElseIf durationMax = 0 AndAlso durationMin > 0 Then
                                                                   durationMax = durationMin
                                                               End If

                                                               Return New With {
                                                              .Row = row,
                                                              .DurationMin = durationMin,
                                                              .DurationMax = durationMax
                                                          }
                                                           End Function)

                ' Sort the rows based on DurationMin and then DurationMax
                If direction = ListSortDirection.Ascending Then
                    rows = rows.OrderBy(Function(x) x.DurationMin).ThenBy(Function(x) x.DurationMax)
                Else
                    rows = rows.OrderByDescending(Function(x) x.DurationMin).ThenByDescending(Function(x) x.DurationMax)
                End If

                ' Populate the sorted table
                For Each item In rows
                    sortedTable.ImportRow(item.Row)
                Next

            Case "DistancesConcat"
                ' Similar logic can be applied for DistancesConcat if needed
                ' Example: Sort based on TotalDistance
                Dim rows = dataTable.AsEnumerable().Select(Function(row)
                                                               Dim totalDistance = If(IsDBNull(row("TotalDistance")), 0, Convert.ToInt32(row("TotalDistance")))
                                                               Return New With {
                                                              .Row = row,
                                                              .TotalDistance = totalDistance
                                                          }
                                                           End Function)

                ' Sort the rows based on the total distance
                If direction = ListSortDirection.Ascending Then
                    rows = rows.OrderBy(Function(x) x.TotalDistance)
                Else
                    rows = rows.OrderByDescending(Function(x) x.TotalDistance)
                End If

                ' Populate the sorted table
                For Each item In rows
                    sortedTable.ImportRow(item.Row)
                Next
            Case "TotDownloads"
                Dim rows = dataTable.AsEnumerable().Select(Function(row)
                                                               Dim totalDownloads = If(IsDBNull(row("TotDownloads")), 0, Convert.ToInt32(row("TotDownloads")))
                                                               Return New With {
                                                              .Row = row,
                                                              .TotDownloads = totalDownloads
                                                          }
                                                           End Function)

                ' Sort the rows based on the total distance
                If direction = ListSortDirection.Ascending Then
                    rows = rows.OrderBy(Function(x) x.TotDownloads)
                Else
                    rows = rows.OrderByDescending(Function(x) x.TotDownloads)
                End If

                ' Populate the sorted table
                For Each item In rows
                    sortedTable.ImportRow(item.Row)
                Next
        End Select

        ' Rebind the DataGridView to the sorted DataTable
        gridCurrentDatabase.DataSource = sortedTable
    End Sub

    Private Sub CheckColumnSizes()
        Dim totalColumnWidth As Integer = 0
        For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
            If col.Visible Then
                totalColumnWidth += col.Width
            End If
        Next

        If gridCurrentDatabase.Columns.GetColumnCount(False) > 0 Then
            ' Check if there's extra space
            If totalColumnWidth < gridCurrentDatabase.Width Then
                ' Set the last column or a specific column to fill the remaining space
                gridCurrentDatabase.Columns("Title").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Else
                ' Optional: Reset the AutoSizeMode if no extra space
                gridCurrentDatabase.Columns("Title").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            End If
        End If
    End Sub

    Private Sub AddColumnsVisibilityOptions()

        ' List of column names to exclude from the visibility toggle
        Dim excludeColumns As List(Of String) = GetListOfExcludedColumnNames()

        ' Clear only dynamic items (those added for column visibility)
        For i As Integer = TasksGridContextMenu.Items.Count - 1 To 2 Step -1
            Dim item As ToolStripItem = TasksGridContextMenu.Items(i)
            If TypeOf item Is ToolStripMenuItem AndAlso CType(item, ToolStripMenuItem).Name.StartsWith("dynamicCol_") Then
                TasksGridContextMenu.Items.RemoveAt(i)
            End If
        Next

        ' Add sorted columns to the menu
        Dim sortedColumns = From col In gridCurrentDatabase.Columns.Cast(Of DataGridViewColumn)()
                            Order By col.DisplayIndex
                            Select col

        ' Add a menu item for each column in the DataGridView
        Dim index As Integer = 2 ' Start adding items at the start or find the right index if needed
        For Each col As DataGridViewColumn In sortedColumns
            If Not excludeColumns.Contains(col.Name) Then
                Dim menuItem As New ToolStripMenuItem With
                            {
                            .Text = col.HeaderText,
                            .ToolTipText = "Toggle visibility of the column.",
                            .Checked = col.Visible,
                            .Tag = col,
                            .Name = "dynamicCol_" & col.Tag  ' Tagging the item as dynamic
                            }
                AddHandler menuItem.Click, AddressOf ColumnVisibilityToggle
                _handlerColumnVisibilityToggle.Add(menuItem)
                ' Insert at specific index if needed, or add directly
                TasksGridContextMenu.Items.Insert(index, menuItem)
                index += 1
            End If
        Next

    End Sub

    Private Sub UpdateCurrentDBGrid()

        LoadTasksFromDatabase()
        gridCurrentDatabase.DataSource = _currentTaskDBEntries.DefaultView.ToTable()

    End Sub

    Private Sub LoadTasksFromDatabase()
        Dim connectionString As String = $"Data Source={_localTasksDatabaseFilePath};Version=3;"
        _currentTaskDBEntries = New DataTable()

        Using conn As New SQLiteConnection(connectionString)
            conn.Open()
            Using cmd As New SQLiteCommand("SELECT Tasks.EntrySeqID, 
                                               Tasks.TaskID, 
                                               Tasks.Title, 
                                               Tasks.LastUpdate, 
                                               Tasks.SimDateTime, 
                                               Tasks.IncludeYear, 
                                               Tasks.SimDateTimeExtraInfo, 
                                               Tasks.MainAreaPOI,
                                               Tasks.DepartureName, 
                                               Tasks.DepartureICAO, 
                                               Tasks.DepartureExtra, 
                                               Tasks.ArrivalName, 
                                               Tasks.ArrivalICAO, 
                                               Tasks.ArrivalExtra, 
                                               Tasks.SoaringRidge, 
                                               Tasks.SoaringThermals, 
                                               Tasks.SoaringWaves, 
                                               Tasks.SoaringDynamic, 
                                               Tasks.SoaringExtraInfo, 
                                               Tasks.DurationMin, 
                                               Tasks.DurationMax, 
                                               Tasks.DurationExtraInfo, 
                                               Tasks.TaskDistance, 
                                               Tasks.TotalDistance, 
                                               Tasks.RecommendedGliders, 
                                               Tasks.DifficultyRating, 
                                               Tasks.DifficultyExtraInfo, 
                                               Tasks.ShortDescription, 
                                               Tasks.LongDescription, 
                                               Tasks.WeatherSummary, 
                                               Tasks.Credits, 
                                               Tasks.Countries, 
                                               Tasks.RecommendedAddOns, 
                                               Tasks.DBEntryUpdate, 
                                               Tasks.TotDownloads,
                                               Tasks.LastDownloadUpdate,
                                               Tasks.ThreadAccess,
                                               Tasks.RepostText,
                                               Tasks.LastUpdateDescription,
                                        	   COALESCE(UserData.TaskQualityRating, 0) AS TaskQualityRating,
                                               COALESCE(UserData.TaskDifficultyRating, 0) AS TaskDifficultyRating,
                                               COALESCE(UserData.Comment, '') AS Comment,
                                               COALESCE(UserData.Tags, '') AS Tags,
                                               COALESCE(UserData.TaskFlown, 0) AS TaskFlown,
                                               COALESCE(UserData.ToFly, 0) AS ToFly
                                        FROM Tasks
                                        LEFT JOIN UserData ON Tasks.EntrySeqID = UserData.EntrySeqID", conn)
                Using adapter As New SQLiteDataAdapter(cmd)
                    adapter.Fill(_currentTaskDBEntries)
                End Using
            End Using
        End Using

        ' Add calculated columns
        Dim durationConcatColumn As New DataColumn("DurationConcat", GetType(String))
        Dim distancesConcatColumn As New DataColumn("DistancesConcat", GetType(String))
        Dim difficultyConcatColumn As New DataColumn("DifficultyConcat", GetType(String))
        _currentTaskDBEntries.Columns.Add(durationConcatColumn)
        _currentTaskDBEntries.Columns.Add(distancesConcatColumn)
        _currentTaskDBEntries.Columns.Add(difficultyConcatColumn)

        ' Add boolean columns for checkboxes
        Dim includeYearColumn As New DataColumn("IncludeYearBool", GetType(Boolean))
        Dim soaringRidgeColumn As New DataColumn("SoaringRidgeBool", GetType(Boolean))
        Dim soaringThermalsColumn As New DataColumn("SoaringThermalsBool", GetType(Boolean))
        Dim soaringWavesColumn As New DataColumn("SoaringWavesBool", GetType(Boolean))
        Dim soaringDynamicColumn As New DataColumn("SoaringDynamicBool", GetType(Boolean))
        Dim recommendedAddOnsColumn As New DataColumn("RecommendedAddOnsBool", GetType(Boolean))
        Dim taskFlownColumn As New DataColumn("TaskFlownBool", GetType(Boolean))
        Dim toFlyColumn As New DataColumn("ToFlyBool", GetType(Boolean))
        _currentTaskDBEntries.Columns.Add(includeYearColumn)
        _currentTaskDBEntries.Columns.Add(soaringRidgeColumn)
        _currentTaskDBEntries.Columns.Add(soaringThermalsColumn)
        _currentTaskDBEntries.Columns.Add(soaringWavesColumn)
        _currentTaskDBEntries.Columns.Add(soaringDynamicColumn)
        _currentTaskDBEntries.Columns.Add(recommendedAddOnsColumn)
        _currentTaskDBEntries.Columns.Add(taskFlownColumn)
        _currentTaskDBEntries.Columns.Add(toFlyColumn)

        ' Calculate values for the calculated and boolean columns
        For Each row As DataRow In _currentTaskDBEntries.Rows
            row("DurationConcat") = SupportingFeatures.GetDuration(row("DurationMin"), row("DurationMax"))
            row("DistancesConcat") = SupportingFeatures.GetDistance(Convert.ToInt32(row("TotalDistance")), Convert.ToInt32(row("TaskDistance")), PrefUnits)
            Dim diffIndex As Integer = CInt(row("DifficultyRating").Substring(0, 1))
            row("DifficultyConcat") = SupportingFeatures.GetDifficulty(diffIndex, row("DifficultyExtraInfo"))

            ' Convert integer values to boolean
            row("IncludeYearBool") = (CInt(row("IncludeYear")) = 1)
            row("SoaringRidgeBool") = (CInt(row("SoaringRidge")) = 1)
            row("SoaringThermalsBool") = (CInt(row("SoaringThermals")) = 1)
            row("SoaringWavesBool") = (CInt(row("SoaringWaves")) = 1)
            row("SoaringDynamicBool") = (CInt(row("SoaringDynamic")) = 1)
            row("RecommendedAddOnsBool") = (CInt(row("RecommendedAddOns")) = 1)
            row("TaskFlownBool") = If(IsDBNull(row("TaskFlown")), False, (CInt(row("TaskFlown")) = 1))
            row("ToFlyBool") = If(IsDBNull(row("ToFly")), False, (CInt(row("ToFly")) = 1))
        Next
    End Sub

    Private Sub SaveDataGridViewSettings()
        If gridCurrentDatabase.SortedColumn IsNot Nothing Then
            Settings.SessionSettings.TaskLibrarySortColumn = gridCurrentDatabase.SortedColumn.Name
        Else
            Settings.SessionSettings.TaskLibrarySortColumn = "EntrySeqID"
        End If
        Settings.SessionSettings.TaskLibrarySortAsc = (gridCurrentDatabase.SortOrder = SortOrder.Ascending)
        Settings.SessionSettings.TaskLibrarySplitterLocation = CInt(splitMain.SplitterDistance / splitMain.Width * 100)
        Settings.SessionSettings.TaskLibraryRightPartSplitterLocation = CInt(splitRightPart.SplitterDistance / splitRightPart.Height * 100)
        Settings.SessionSettings.TaskLibraryDetailsZoomLevel = txtBriefing.ZoomFactor

        Settings.SessionSettings.TBColumnsSettings.Clear()
        For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
            Settings.SessionSettings.TBColumnsSettings.Add(New TBColumnSetting(col.Name, col.DisplayIndex, col.Visible, col.Width))
        Next
    End Sub

    Private Sub SetupDataGridView()

        gridCurrentDatabase.Font = New Font(gridCurrentDatabase.Font.FontFamily, 12)
        gridCurrentDatabase.RowTemplate.Height = 35
        gridCurrentDatabase.ColumnHeadersDefaultCellStyle.Font = New Font(gridCurrentDatabase.Font, FontStyle.Bold)

        gridCurrentDatabase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader
        gridCurrentDatabase.ReadOnly = True

        gridCurrentDatabase.Columns("EntrySeqID").HeaderText = "#"
        gridCurrentDatabase.Columns("EntrySeqID").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("EntrySeqID").DisplayIndex = 0

        gridCurrentDatabase.Columns("Title").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("Title").DisplayIndex = 1

        gridCurrentDatabase.Columns("LastUpdate").HeaderText = "Updated"
        gridCurrentDatabase.Columns("LastUpdate").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("LastUpdate").DisplayIndex = 2

        gridCurrentDatabase.Columns("MainAreaPOI").HeaderText = "Main Area / POI"
        gridCurrentDatabase.Columns("MainAreaPOI").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("MainAreaPOI").DisplayIndex = 3
        gridCurrentDatabase.Columns("MainAreaPOI").Width = 560

        gridCurrentDatabase.Columns("DepartureName").HeaderText = "From Name"
        gridCurrentDatabase.Columns("DepartureName").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("DepartureName").DisplayIndex = 4
        gridCurrentDatabase.Columns("DepartureName").Width = 225

        gridCurrentDatabase.Columns("DepartureICAO").HeaderText = "ICAO"
        gridCurrentDatabase.Columns("DepartureICAO").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DepartureICAO").DisplayIndex = 5

        gridCurrentDatabase.Columns("ArrivalName").HeaderText = "To Name"
        gridCurrentDatabase.Columns("ArrivalName").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("ArrivalName").DisplayIndex = 6
        gridCurrentDatabase.Columns("ArrivalName").Width = 225

        gridCurrentDatabase.Columns("ArrivalICAO").HeaderText = "ICAO"
        gridCurrentDatabase.Columns("ArrivalICAO").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("ArrivalICAO").DisplayIndex = 7

        gridCurrentDatabase.Columns("SoaringRidgeBool").HeaderText = "R"
        gridCurrentDatabase.Columns("SoaringRidgeBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringRidgeBool").DisplayIndex = 8

        gridCurrentDatabase.Columns("SoaringThermalsBool").HeaderText = "T"
        gridCurrentDatabase.Columns("SoaringThermalsBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringThermalsBool").DisplayIndex = 9

        gridCurrentDatabase.Columns("SoaringWavesBool").HeaderText = "W"
        gridCurrentDatabase.Columns("SoaringWavesBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringWavesBool").DisplayIndex = 10

        gridCurrentDatabase.Columns("SoaringDynamicBool").HeaderText = "D"
        gridCurrentDatabase.Columns("SoaringDynamicBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringDynamicBool").DisplayIndex = 11

        gridCurrentDatabase.Columns("SoaringExtraInfo").HeaderText = "Soaring Extra"
        gridCurrentDatabase.Columns("SoaringExtraInfo").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("SoaringExtraInfo").DisplayIndex = 12
        gridCurrentDatabase.Columns("SoaringExtraInfo").Width = 225

        gridCurrentDatabase.Columns("DurationConcat").HeaderText = "Duration"
        gridCurrentDatabase.Columns("DurationConcat").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DurationConcat").DisplayIndex = 13

        gridCurrentDatabase.Columns("DurationExtraInfo").HeaderText = "Duration Extra"
        gridCurrentDatabase.Columns("DurationExtraInfo").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("DurationExtraInfo").DisplayIndex = 14
        gridCurrentDatabase.Columns("DurationExtraInfo").Width = 290

        gridCurrentDatabase.Columns("DistancesConcat").HeaderText = "Distances"
        gridCurrentDatabase.Columns("DistancesConcat").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DistancesConcat").DisplayIndex = 15

        gridCurrentDatabase.Columns("RecommendedGliders").HeaderText = "Gliders"
        gridCurrentDatabase.Columns("RecommendedGliders").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("RecommendedGliders").DisplayIndex = 16
        gridCurrentDatabase.Columns("RecommendedGliders").Width = 300

        gridCurrentDatabase.Columns("DifficultyConcat").HeaderText = "Difficulty"
        gridCurrentDatabase.Columns("DifficultyConcat").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("DifficultyConcat").DisplayIndex = 17
        gridCurrentDatabase.Columns("DifficultyConcat").Width = 305

        gridCurrentDatabase.Columns("WeatherSummary").HeaderText = "Weather Summary"
        gridCurrentDatabase.Columns("WeatherSummary").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("WeatherSummary").DisplayIndex = 18
        gridCurrentDatabase.Columns("WeatherSummary").Width = 250

        gridCurrentDatabase.Columns("Credits").HeaderText = "Credits"
        gridCurrentDatabase.Columns("Credits").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("Credits").DisplayIndex = 19
        gridCurrentDatabase.Columns("Credits").Width = 450

        gridCurrentDatabase.Columns("Countries").HeaderText = "Countries"
        gridCurrentDatabase.Columns("Countries").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("Countries").DisplayIndex = 20
        gridCurrentDatabase.Columns("Countries").Width = 295

        gridCurrentDatabase.Columns("RecommendedAddonsBool").HeaderText = "AddOns"
        gridCurrentDatabase.Columns("RecommendedAddonsBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("RecommendedAddonsBool").DisplayIndex = 21

        gridCurrentDatabase.Columns("TotDownloads").HeaderText = "Downloads"
        gridCurrentDatabase.Columns("TotDownloads").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("TotDownloads").DisplayIndex = 22
        gridCurrentDatabase.Columns("TotDownloads").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        gridCurrentDatabase.Columns("TaskQualityRating").HeaderText = "YQ"
        gridCurrentDatabase.Columns("TaskQualityRating").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("TaskQualityRating").DisplayIndex = 23
        gridCurrentDatabase.Columns("TaskQualityRating").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        gridCurrentDatabase.Columns("TaskDifficultyRating").HeaderText = "YD"
        gridCurrentDatabase.Columns("TaskDifficultyRating").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("TaskDifficultyRating").DisplayIndex = 24
        gridCurrentDatabase.Columns("TaskDifficultyRating").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

        gridCurrentDatabase.Columns("TaskFlownBool").HeaderText = "Flown"
        gridCurrentDatabase.Columns("TaskFlownBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("TaskFlownBool").DisplayIndex = 25

        gridCurrentDatabase.Columns("ToFlyBool").HeaderText = "To fly"
        gridCurrentDatabase.Columns("ToFlyBool").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("ToFlyBool").DisplayIndex = 26

        gridCurrentDatabase.Columns("Tags").HeaderText = "Tags"
        gridCurrentDatabase.Columns("Tags").AutoSizeMode = DataGridViewAutoSizeColumnsMode.None
        gridCurrentDatabase.Columns("Tags").DisplayIndex = 27
        gridCurrentDatabase.Columns("Tags").Width = 80

        ' List of column names to exclude from the visibility toggle
        Dim excludeColumns As List(Of String) = GetListOfExcludedColumnNames()

        If gridCurrentDatabase IsNot Nothing Then
            If Settings.SessionSettings.TBColumnsSettings.Count <> gridCurrentDatabase.Columns.Count Then
                'Check which column setting is missing
                Dim columnNames As New List(Of String)
                For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
                    columnNames.Add(col.Name)
                Next
                For Each setting In Settings.SessionSettings.TBColumnsSettings
                    If columnNames.Contains(setting.Name) Then
                        columnNames.Remove(setting.Name)
                    End If
                Next
                For Each colName As String In columnNames
                    Dim col As DataGridViewColumn = gridCurrentDatabase.Columns(colName)
                    Settings.SessionSettings.TBColumnsSettings.Add(New TBColumnSetting(col.Name, col.DisplayIndex, col.Visible, col.Width))
                Next
            End If
            For Each setting In Settings.SessionSettings.TBColumnsSettings
                If gridCurrentDatabase.Columns.Contains(setting.Name) Then
                    If excludeColumns.Contains(setting.Name) Then
                        gridCurrentDatabase.Columns(setting.Name).Visible = False
                        gridCurrentDatabase.Columns(setting.Name).DisplayIndex = setting.DisplayIndex
                    Else
                        gridCurrentDatabase.Columns(setting.Name).Visible = setting.Visible
                        gridCurrentDatabase.Columns(setting.Name).DisplayIndex = setting.DisplayIndex
                        If setting.ColumnWidth > 0 Then
                            gridCurrentDatabase.Columns(setting.Name).Width = setting.ColumnWidth
                        End If
                        Select Case setting.Name
                            Case "DurationConcat", "DistancesConcat", "TotDownloads"
                                gridCurrentDatabase.Columns(setting.Name).SortMode = DataGridViewColumnSortMode.Programmatic
                            Case Else
                                gridCurrentDatabase.Columns(setting.Name).SortMode = DataGridViewColumnSortMode.Automatic
                        End Select
                        gridCurrentDatabase.Columns(setting.Name).Tag = setting.Name
                    End If
                End If
            Next
        End If

        _dataGridViewAllSet = True

    End Sub

    Private Sub BuildTaskData()

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat
        Dim dateFormat As String
        If _selectedTaskRow("IncludeYear") Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        'Title
        sb.Append($"**{_selectedTaskRow("Title")}**($*$)")
        If _selectedTaskRow("MainAreaPOI").ToString().Trim <> String.Empty Then
            sb.Append($"{_selectedTaskRow("MainAreaPOI")}($*$)")
            sb.Append("($*$)")
        End If
        If _selectedTaskRow("ShortDescription").ToString().Trim <> String.Empty Then
            sb.Append($"{_selectedTaskRow("ShortDescription")}($*$)")
            sb.Append("($*$)")
        End If

        'Credits
        sb.Append($"{_selectedTaskRow("Credits")}($*$)")
        If _selectedTaskRow("RepostText").ToString().Trim <> String.Empty Then
            Dim discordURL As String = _selectedTaskRow("RepostText").ToString().Replace("http://discord.com", "discord://discord.com")
            discordURL = discordURL.Replace("https://discord.com", "discord://discord.com")
            sb.Append($"{discordURL}($*$)")
        End If
        sb.Append("($*$)")

        'Local MSFS date and time 
        sb.Append($"MSFS Local date & time is **{Convert.ToDateTime(_selectedTaskRow("SimDateTime")).ToString(dateFormat, _EnglishCulture)}, {Convert.ToDateTime(_selectedTaskRow("SimDateTime")).ToString(dateTimeFormat.ShortTimePattern, CultureInfo.CurrentCulture)} {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("SimDateTimeExtraInfo").ToString().Trim, True, True)}**($*$)")

        'Departure airfield And runway
        sb.Append($"You will depart from **{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DepartureICAO").ToString())}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DepartureName").ToString(), True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DepartureExtra").ToString(), True, True)}**($*$)")

        'Arrival airfield And expected runway
        sb.Append($"You will land at **{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("ArrivalICAO").ToString())}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("ArrivalName").ToString(), True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("ArrivalExtra").ToString(), True, True)}**($*$)")

        'Type of soaring
        Dim soaringType As String = SupportingFeatures.GetSoaringTypesSelected(Convert.ToBoolean(_selectedTaskRow("SoaringRidge")), Convert.ToBoolean(_selectedTaskRow("SoaringThermals")), Convert.ToBoolean(_selectedTaskRow("SoaringWaves")), Convert.ToBoolean(_selectedTaskRow("SoaringDynamic")))
        If soaringType.Trim <> String.Empty OrElse _selectedTaskRow("SoaringExtraInfo").ToString() <> String.Empty Then
            sb.Append($"Soaring Type is **{soaringType}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("SoaringExtraInfo").ToString(), True, True)}**($*$)")
        End If

        'Task distance And total distance
        sb.Append($"Distance are **{_selectedTaskRow("DistancesConcat")}**($*$)")

        'Approx. duration
        sb.Append($"Approx. duration should be **{_selectedTaskRow("DurationConcat")} {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DurationExtraInfo").ToString(), True, True)}**($*$)")

        'Recommended gliders
        If _selectedTaskRow("RecommendedGliders").ToString().Trim <> String.Empty Then
            sb.Append($"Recommended gliders: **{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("RecommendedGliders").ToString())}**($*$)")
        End If

        'Difficulty rating
        If _selectedTaskRow("DifficultyRating").ToString().Trim <> String.Empty OrElse _selectedTaskRow("DifficultyExtraInfo").ToString().Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as **{SupportingFeatures.GetDifficulty(CInt(_selectedTaskRow("DifficultyRating").ToString().Substring(0, 1)), _selectedTaskRow("DifficultyExtraInfo").ToString(), True)}**($*$)")
        End If

        sb.Append("($*$)")

        If _selectedTaskRow("WeatherSummary").ToString() <> String.Empty Then
            sb.Append($"Weather summary: **{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("WeatherSummary").ToString())}**($*$)")
            sb.Append("($*$)")
        End If

        'Build full description
        If _selectedTaskRow("LongDescription").ToString() <> String.Empty Then
            sb.Append("**Full Description**($*$)")
            sb.Append(_selectedTaskRow("LongDescription").ToString())
        End If

        Dim oldZoomFactor As Single = txtBriefing.ZoomFactor
        'If oldZoomFactor = 1 Then
        'oldZoomFactor = 1.1
        'End If
        txtBriefing.ZoomFactor = 1
        txtBriefing.Rtf = SupportingFeatures.ConvertMarkdownToRTF(sb.ToString.Trim)
        'SupportingFeatures.FormatMarkdownToRTF(sb.ToString(), txtBriefing)
        txtBriefing.ZoomFactor = oldZoomFactor

        ' Dispose existing images
        If imgMap.Image IsNot Nothing Then
            imgMap.Image.Dispose()
            imgMap.Image = Nothing
        End If

        If imgCover.Image IsNot Nothing Then
            imgCover.Image.Dispose()
            imgCover.Image = Nothing
        End If

        Dim connectionString As String = $"Data Source={_localTasksDatabaseFilePath};Version=3;"
        Dim imgMapObject As Object = Nothing
        Dim imgCoverObject As Object = Nothing
        Using conn As New SQLiteConnection(connectionString)
            conn.Open()
            Using cmd As New SQLiteCommand("SELECT MapImage, CoverImage FROM Tasks WHERE EntrySeqID = @EntrySeqID", conn)
                cmd.Parameters.AddWithValue("@EntrySeqID", _selectedTaskRow("EntrySeqID"))
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        imgMapObject = If(reader.IsDBNull(reader.GetOrdinal("MapImage")), Nothing, reader("MapImage"))
                        imgCoverObject = If(reader.IsDBNull(reader.GetOrdinal("CoverImage")), Nothing, reader("CoverImage"))
                    End If
                End Using
            End Using
        End Using

        ' Set images to PictureBox controls
        If imgMapObject IsNot Nothing AndAlso Not IsDBNull(imgMapObject) Then
            Dim mapImageBytes As Byte() = CType(imgMapObject, Byte())
            imgMap.Image = ByteArrayToImage(mapImageBytes)
        Else
            imgMap.Image = Nothing
        End If

        If imgCoverObject IsNot Nothing AndAlso Not IsDBNull(imgCoverObject) Then
            Dim coverImageBytes As Byte() = CType(imgCoverObject, Byte())
            imgCover.Image = ByteArrayToImage(coverImageBytes)
        Else
            imgCover.Image = Nothing
        End If

        If imgMap.Image IsNot Nothing AndAlso imgCover.Image IsNot Nothing Then
            splitImages.Panel1Collapsed = False
            splitImages.Panel2Collapsed = False
        ElseIf imgMap.Image IsNot Nothing Then
            splitImages.Panel1Collapsed = False
            splitImages.Panel2Collapsed = True
        Else
            splitImages.Panel1Collapsed = True
            splitImages.Panel2Collapsed = False
        End If

        'User Your Data
        chkTaskFlown.Checked = _selectedTaskRow("TaskFlown")
        chkToFly.Checked = _selectedTaskRow("ToFly")
        trkDifficultyRating.Value = _selectedTaskRow("TaskDifficultyRating")
        trkQualityRating.Value = _selectedTaskRow("TaskQualityRating")
        txtComment.Text = _selectedTaskRow("Comment")
        txtTags.Text = _selectedTaskRow("Tags")

    End Sub

    Private Function ByteArrayToImage(ByVal byteArray As Byte()) As Image
        Using ms As New MemoryStream(byteArray)
            Return Image.FromStream(ms)
        End Using
    End Function

    Private Sub ProcessSearch(searchTerm As String, addSearchTermToList As Boolean)
        SaveCurrentPosition()

        If PerformSearch(searchTerm) Then
            If addSearchTermToList Then
                _searchTerms.Add(searchTerm)
            End If
            If lblSearchTerms.Text = String.Empty Then
                lblSearchTerms.Text = $"{searchTerm}"
            Else
                lblSearchTerms.Text = $"{lblSearchTerms.Text} - {searchTerm}"
            End If
            RestorePosition()
            SelectTaskOnMap(_selectedTaskRow("EntrySeqID"))
        End If
    End Sub

    Private Function PerformSearch(searchTerm As String) As Boolean

        Dim validSearch As Boolean = True

        Dim filteredRows As EnumerableRowCollection(Of DataRow) = Nothing
        Dim sourceTable As DataTable
        Dim specificSearch As Boolean = False
        Dim specificField As String = String.Empty
        Dim negationCondition As Boolean = False
        Dim specificFieldWord As String = String.Empty

        Dim acceptableSpecialTerms As New Dictionary(Of String, String)

        If _filteredDataTable Is Nothing Then
            sourceTable = _currentTaskDBEntries
        Else
            sourceTable = _filteredDataTable
        End If

        'Search
        'Check for negation first
        If searchTerm.StartsWith("!") Then
            negationCondition = True
            searchTerm = searchTerm.Substring(1).ToLower
        Else
            negationCondition = False
        End If

        If searchTerm.StartsWith("%") Then
            specificSearch = True
            searchTerm = searchTerm.Substring(1).ToLower
            If searchTerm.StartsWith("!") Then
                negationCondition = True
                searchTerm = searchTerm.Substring(1)
            End If

            If searchTerm.Contains("(") And searchTerm.Contains(")") Then
                ' Isolate the field and the value to search
                specificField = searchTerm.Substring(0, searchTerm.IndexOf("(")).Trim()
                specificFieldWord = searchTerm.Substring(searchTerm.IndexOf("(") + 1, searchTerm.IndexOf(")") - searchTerm.IndexOf("(") - 1)
            Else
                specificField = searchTerm
            End If

            acceptableSpecialTerms.Add("nbr", "EntrySeqID")
            acceptableSpecialTerms.Add("ridge", "SoaringRidge")
            acceptableSpecialTerms.Add("thermals", "SoaringThermals")
            acceptableSpecialTerms.Add("waves", "SoaringWaves")
            acceptableSpecialTerms.Add("dynamic", "SoaringDynamic")
            acceptableSpecialTerms.Add("addons", "RecommendedAddOns")
            acceptableSpecialTerms.Add("title", "Title")
            acceptableSpecialTerms.Add("mainarea", "MainAreaPOI")
            acceptableSpecialTerms.Add("depname", "DepartureName")
            acceptableSpecialTerms.Add("arrname", "ArrivalName")
            acceptableSpecialTerms.Add("deparrname", "")
            acceptableSpecialTerms.Add("depicao", "DepartureICAO")
            acceptableSpecialTerms.Add("arricao", "ArrivalICAO")
            acceptableSpecialTerms.Add("deparricao", "")
            acceptableSpecialTerms.Add("gliders", "RecommendedGliders")
            acceptableSpecialTerms.Add("diff", "DifficultyConcat")
            acceptableSpecialTerms.Add("countries", "Countries")
            acceptableSpecialTerms.Add("weather", "WeatherSummary")
            acceptableSpecialTerms.Add("credits", "Credits")
            acceptableSpecialTerms.Add("duration", "")
            acceptableSpecialTerms.Add("distance", "TotalDistance")
            acceptableSpecialTerms.Add("description", "")
            acceptableSpecialTerms.Add("totdown", "TotDownloads")
            acceptableSpecialTerms.Add("comment", "Comment")
            acceptableSpecialTerms.Add("tags", "Tags")
            acceptableSpecialTerms.Add("yq", "YQ")
            acceptableSpecialTerms.Add("yd", "YD")
            acceptableSpecialTerms.Add("flown", "TaskFlown")
            acceptableSpecialTerms.Add("tofly", "ToFly")

            If Not acceptableSpecialTerms.Keys.Contains(specificField) Then
                Using New Centered_MessageBox()
                    MessageBox.Show(Me, "This search term Is Not valid.", "Search tasks", vbOKOnly, MessageBoxIcon.Error)
                End Using
                Return False
            End If
        End If

        If specificSearch Then
            ' Process special search term
            Select Case specificField
                Case "ridge", "thermals", "waves", "dynamic", "addons", "flown", "tofly"
                    filteredRows = sourceTable.AsEnumerable().Where(Function(row) Convert.ToBoolean(row(acceptableSpecialTerms(specificField))) = Not negationCondition)
                Case "description"
                    If negationCondition Then
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (WildcardMatch(row("ShortDescription").ToString(), specificFieldWord) OrElse WildcardMatch(row("LongDescription").ToString(), specificFieldWord)))
                    Else
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) WildcardMatch(row("ShortDescription").ToString(), specificFieldWord) OrElse WildcardMatch(row("LongDescription").ToString(), specificFieldWord))
                    End If
                Case "deparrname"
                    If negationCondition Then
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (WildcardMatch(row("DepartureName").ToString(), specificFieldWord) OrElse WildcardMatch(row("ArrivalName").ToString(), specificFieldWord)))
                    Else
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) WildcardMatch(row("DepartureName").ToString(), specificFieldWord) OrElse WildcardMatch(row("ArrivalName").ToString(), specificFieldWord))
                    End If
                Case "deparricao"
                    If negationCondition Then
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (WildcardMatch(row("DepartureICAO").ToString(), specificFieldWord) OrElse WildcardMatch(row("ArrivalICAO").ToString(), specificFieldWord)))
                    Else
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) WildcardMatch(row("DepartureICAO").ToString(), specificFieldWord) OrElse WildcardMatch(row("ArrivalICAO").ToString(), specificFieldWord))
                    End If
                Case "duration", "distance", "totdown", "yq", "yd"
                    Dim ranges() As String = specificFieldWord.Split("-"c)
                    Dim minValue As Integer
                    Dim maxValue As Integer
                    If ranges.Length <> 2 OrElse Not Integer.TryParse(ranges(0), minValue) OrElse Not Integer.TryParse(ranges(1), maxValue) Then
                        Using New Centered_MessageBox()
                            MessageBox.Show(Me, "Invalid range format. Please use 'min-max' format.", "Search tasks", vbOKOnly, MessageBoxIcon.Error)
                        End Using
                        Return False
                    End If
                    Select Case specificField
                        Case "duration"
                            If negationCondition Then
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (
                                (row("DurationMin") <> 0 AndAlso row("DurationMax") <> 0 AndAlso Integer.Parse(row("DurationMin")) >= minValue AndAlso Integer.Parse(row("DurationMax")) <= maxValue) OrElse
                                (row("DurationMin") <> 0 AndAlso row("DurationMax") = 0 AndAlso Integer.Parse(row("DurationMin")) >= minValue AndAlso Integer.Parse(row("DurationMin")) <= maxValue) OrElse
                                (row("DurationMin") = 0 AndAlso row("DurationMax") <> 0 AndAlso Integer.Parse(row("DurationMax")) >= minValue AndAlso Integer.Parse(row("DurationMax")) <= maxValue)
                            ))
                            Else
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) (
                                (row("DurationMin") <> 0 AndAlso row("DurationMax") <> 0 AndAlso Integer.Parse(row("DurationMin")) >= minValue AndAlso Integer.Parse(row("DurationMax")) <= maxValue) OrElse
                                (row("DurationMin") <> 0 AndAlso row("DurationMax") = 0 AndAlso Integer.Parse(row("DurationMin")) >= minValue AndAlso Integer.Parse(row("DurationMin")) <= maxValue) OrElse
                                (row("DurationMin") = 0 AndAlso row("DurationMax") <> 0 AndAlso Integer.Parse(row("DurationMax")) >= minValue AndAlso Integer.Parse(row("DurationMax")) <= maxValue)
                            ))
                            End If
                        Case "distance"
                            If negationCondition Then
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (
                                    (row("TotalDistance") <> 0 AndAlso Integer.Parse(row("TotalDistance")) >= minValue AndAlso Integer.Parse(row("TotalDistance")) <= maxValue)
                                ))
                            Else
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) (
                                    (row("TotalDistance") <> 0 AndAlso Integer.Parse(row("TotalDistance")) >= minValue AndAlso Integer.Parse(row("TotalDistance")) <= maxValue)
                                ))
                            End If
                        Case "totdown"
                            If negationCondition Then
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (
                                    (row("TotDownloads") <> 0 AndAlso Integer.Parse(row("TotDownloads")) >= minValue AndAlso Integer.Parse(row("TotDownloads")) <= maxValue)
                                ))
                            Else
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) (
                                    (row("TotDownloads") <> 0 AndAlso Integer.Parse(row("TotDownloads")) >= minValue AndAlso Integer.Parse(row("TotDownloads")) <= maxValue)
                                ))
                            End If
                        Case "yq"
                            If negationCondition Then
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (
                                    (row("TaskQualityRating") <> 0 AndAlso Integer.Parse(row("TaskQualityRating")) >= minValue AndAlso Integer.Parse(row("TaskQualityRating")) <= maxValue)
                                ))
                            Else
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) (
                                    (row("TaskQualityRating") <> 0 AndAlso Integer.Parse(row("TaskQualityRating")) >= minValue AndAlso Integer.Parse(row("TaskQualityRating")) <= maxValue)
                                ))
                            End If
                        Case "yd"
                            If negationCondition Then
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not (
                                    (row("TaskDifficultyRating") <> 0 AndAlso Integer.Parse(row("TaskDifficultyRating")) >= minValue AndAlso Integer.Parse(row("TaskDifficultyRating")) <= maxValue)
                                ))
                            Else
                                filteredRows = sourceTable.AsEnumerable().Where(Function(row) (
                                    (row("TaskDifficultyRating") <> 0 AndAlso Integer.Parse(row("TaskDifficultyRating")) >= minValue AndAlso Integer.Parse(row("TaskDifficultyRating")) <= maxValue)
                                ))
                            End If
                    End Select
                Case Else
                    'The specific term represents a non-boolean column
                    Dim exactFieldName As String = String.Empty
                    If negationCondition Then
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not WildcardMatch(row(acceptableSpecialTerms(specificField)).ToString(), specificFieldWord))
                    Else
                        filteredRows = sourceTable.AsEnumerable().Where(Function(row) WildcardMatch(row(acceptableSpecialTerms(specificField)).ToString(), specificFieldWord))
                    End If
            End Select
        Else
            ' Perform full-text search
            If negationCondition Then
                filteredRows = sourceTable.AsEnumerable().Where(Function(row) Not row.ItemArray.Any(Function(field) WildcardMatch(field.ToString(), searchTerm)))
            Else
                filteredRows = sourceTable.AsEnumerable().Where(Function(row) row.ItemArray.Any(Function(field) WildcardMatch(field.ToString(), searchTerm)))
            End If
        End If

        If _dataGridViewAllSet Then
            SaveDataGridViewSettings()
        End If

        'save sort
        SaveOrReapplySort(True)
        If filteredRows.Any() Then
            _filteredDataTable = filteredRows.CopyToDataTable()
            gridCurrentDatabase.DataSource = _filteredDataTable
        Else
            gridCurrentDatabase.DataSource = _currentTaskDBEntries.Clone() ' Return an empty DataTable with the same schema
        End If
        'reapply sort
        SaveOrReapplySort(False)

        Return validSearch

    End Function

    Private Sub SaveOrReapplySort(save As Boolean)

        If save Then
            If gridCurrentDatabase.SortedColumn IsNot Nothing Then
                prevSortColumn = gridCurrentDatabase.SortedColumn.Name
            Else
                prevSortColumn = String.Empty
            End If
            prevSortDirectionAsc = gridCurrentDatabase.SortOrder = SortOrder.Ascending
        Else
            If prevSortColumn = String.Empty Then
                prevSortColumn = "EntrySeqID"
            End If
            gridCurrentDatabase.Sort(gridCurrentDatabase.Columns(prevSortColumn), If(prevSortDirectionAsc, ListSortDirection.Ascending, ListSortDirection.Descending))
        End If

    End Sub

    Private Function WildcardMatch(input As String, pattern As String) As Boolean
        ' Convert wildcard pattern to regex pattern
        Dim regexPattern As String = "^" & Regex.Escape(pattern).Replace("\*", ".*").Replace("\?", ".") & "$"
        Return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase)
    End Function

    Private Sub SaveCurrentPosition()
        If gridCurrentDatabase.Rows.Count > 0 Then
            scrollIndex = gridCurrentDatabase.FirstDisplayedScrollingRowIndex
            If gridCurrentDatabase.SelectedRows.Count > 0 Then
                selectedRowIndex = gridCurrentDatabase.SelectedRows(0).Index
            End If
            horizontalScrollIndex = gridCurrentDatabase.FirstDisplayedScrollingColumnIndex
        End If
    End Sub

    Private Sub RestorePosition()
        If gridCurrentDatabase.Rows.Count > 0 Then
            If selectedRowIndex >= 0 AndAlso selectedRowIndex < gridCurrentDatabase.Rows.Count Then
                gridCurrentDatabase.ClearSelection()
                gridCurrentDatabase.Rows(selectedRowIndex).Selected = True
                gridCurrentDatabase.CurrentCell = gridCurrentDatabase.Rows(selectedRowIndex).Cells(0)
            End If
            If scrollIndex >= 0 AndAlso scrollIndex < gridCurrentDatabase.Rows.Count Then
                gridCurrentDatabase.FirstDisplayedScrollingRowIndex = scrollIndex
            End If
            If horizontalScrollIndex >= 0 AndAlso horizontalScrollIndex < gridCurrentDatabase.Columns.Count Then
                gridCurrentDatabase.FirstDisplayedScrollingColumnIndex = horizontalScrollIndex
            End If
        End If
    End Sub

    Private Sub AddFavoriteSearch(favoriteTitle As String)
        ' Create the main menu item for the field
        Dim favoriteMenuItem As New ToolStripMenuItem(favoriteTitle)

        ' Create sub-menu items
        Dim replaceSearchItem As New ToolStripMenuItem("Replace search")
        Dim addToSearchItem As New ToolStripMenuItem("Add to search")
        Dim separator As New ToolStripSeparator
        Dim txtRenameTitle As New ToolStripTextBox("txtRenameFavorite")
        Dim renameItem As New ToolStripMenuItem("Rename favorite")
        Dim deleteItem As New ToolStripMenuItem("Delete favorite")

        ' Set the Tag property to the field name
        replaceSearchItem.Tag = favoriteTitle
        replaceSearchItem.ToolTipText = "Replace the current search filter with this favorite."
        addToSearchItem.Tag = favoriteTitle
        addToSearchItem.ToolTipText = "Add the filter from this favorite to current search filter."
        txtRenameTitle.Tag = favoriteTitle
        txtRenameTitle.ToolTipText = "Specify the new name for your favorite."
        renameItem.Tag = txtRenameTitle
        renameItem.ToolTipText = "Using the name specified above, rename this favorite."
        deleteItem.Tag = favoriteTitle
        deleteItem.ToolTipText = "Delete this favorite search filter."

        ' Add event handlers
        AddHandler replaceSearchItem.Click, AddressOf FavoriteReplaceSearchItem_Click
        _handlerFavoriteReplaceSearchItem_Click.Add(replaceSearchItem)

        AddHandler addToSearchItem.Click, AddressOf FavoriteAddToSearchItem_Click
        _handlerFavoriteAddToSearchItem_Click.Add(addToSearchItem)

        AddHandler renameItem.Click, AddressOf FavoriteRenameItem_Click
        _handlerFavoriteRenameItem_Click.Add(renameItem)

        AddHandler deleteItem.Click, AddressOf FavoriteDeleteItem_Click
        _handlerFavoriteDeleteItem_Click.Add(deleteItem)


        ' Add sub-menu items to the field menu item
        favoriteMenuItem.DropDownItems.Add(replaceSearchItem)
        favoriteMenuItem.DropDownItems.Add(addToSearchItem)
        favoriteMenuItem.DropDownItems.Add(separator)
        favoriteMenuItem.DropDownItems.Add(txtRenameTitle)
        favoriteMenuItem.DropDownItems.Add(renameItem)
        favoriteMenuItem.DropDownItems.Add(deleteItem)

        ' Add the field menu item to the main TextCriteriaToolStripMenuItem
        FavoritesToolStripMenuItem.DropDownItems.Add(favoriteMenuItem)
    End Sub

    Private Sub AddFieldWithTextCriteria(fieldName As String, fieldLabel As String)
        ' Create the main menu item for the field
        Dim fieldMenuItem As New ToolStripMenuItem(fieldLabel)

        ' Create sub-menu items
        Dim containingItem As New ToolStripMenuItem("Containing")
        Dim startsWithItem As New ToolStripMenuItem("Starts with")
        Dim endsWithItem As New ToolStripMenuItem("Ends with")
        Dim exactlyItem As New ToolStripMenuItem("Exactly")

        ' Set the Tag property to the field name
        containingItem.Tag = fieldName
        containingItem.ToolTipText = $"Search in {fieldLabel} for tasks containing specified text."
        startsWithItem.Tag = fieldName
        startsWithItem.ToolTipText = $"Search in {fieldLabel} for tasks that start with specified text."
        endsWithItem.Tag = fieldName
        endsWithItem.ToolTipText = $"Search in {fieldLabel} for tasks that end with specified text."
        exactlyItem.Tag = fieldName
        exactlyItem.ToolTipText = $"Search in {fieldLabel} for tasks that have exactly the specified text."

        ' Add event handlers
        AddHandler containingItem.Click, AddressOf TextCriteriaMenuItem_Click
        _handlerTextCriteriaMenuItem_Click.Add(containingItem)

        AddHandler startsWithItem.Click, AddressOf TextCriteriaMenuItem_Click
        _handlerTextCriteriaMenuItem_Click.Add(startsWithItem)

        AddHandler endsWithItem.Click, AddressOf TextCriteriaMenuItem_Click
        _handlerTextCriteriaMenuItem_Click.Add(endsWithItem)

        AddHandler exactlyItem.Click, AddressOf TextCriteriaMenuItem_Click
        _handlerTextCriteriaMenuItem_Click.Add(exactlyItem)

        ' Add sub-menu items to the field menu item
        fieldMenuItem.DropDownItems.Add(containingItem)
        fieldMenuItem.DropDownItems.Add(startsWithItem)
        fieldMenuItem.DropDownItems.Add(endsWithItem)
        fieldMenuItem.DropDownItems.Add(exactlyItem)

        ' Add the field menu item to the main TextCriteriaToolStripMenuItem
        TextCriteriaToolStripMenuItem.DropDownItems.Add(fieldMenuItem)
    End Sub

    Private Sub AddFieldWithNumbersCriteria(fieldName As String, fieldLabel As String)
        ' Create the main menu item for the field
        Dim fieldMenuItem As New ToolStripMenuItem(fieldLabel)
        fieldMenuItem.Tag = fieldName
        fieldMenuItem.ToolTipText = $"Search {fieldLabel} for values that are in the range specified in the text field above."

        ' Add event handlers
        AddHandler fieldMenuItem.Click, AddressOf NumberCriteriaMenuItem_Click
        _handlerNumberCriteriaMenuItem_Click.Add(fieldMenuItem)

        ' Add the field menu item to the main TextCriteriaToolStripMenuItem
        NumbersCriteriaToolStripMenuItem.DropDownItems.Add(fieldMenuItem)
    End Sub

    Private Function FetchUpdatedTasks(lastDBEntryUpdate As String) As DataTable
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}GetUpdatedTasks.php?DBEntryUpdate={lastDBEntryUpdate}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"
        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Return ConvertJsonToDataTable(jsonResponse)
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Throw
        End Try
    End Function

    Private Function FetchUpdatedDownloads(lastDownloadUpdate As String) As DataTable
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}GetUpdatedDownloads.php?lastDownloadUpdate={lastDownloadUpdate}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"
        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Return ConvertJsonToDataTable(jsonResponse)
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Throw
        End Try
    End Function

    Public Function FetchDeletedTasks() As List(Of Integer)
        Dim deletedTasks As New List(Of Integer)

        Try
            Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveDeletedTasks.php"
            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "GET"
            request.ContentType = "application/json"

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim responseObject As JObject = JObject.Parse(jsonResponse)

                    If responseObject("status").ToString() = "success" Then
                        For Each task As JObject In responseObject("deletedTasks")
                            deletedTasks.Add(CInt(task("EntrySeqID").ToString()))
                        Next
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(Me, $"Error retrieving deleted tasks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return deletedTasks
    End Function

    Public Function ConvertJsonToDataTable(json As String) As DataTable
        If String.IsNullOrEmpty(json) OrElse json.Trim() = "[]" Then
            Return New DataTable()
        End If
        Return JsonConvert.DeserializeObject(Of DataTable)(json)
    End Function

    Private Sub CheckForUpdates(showSuccessResults As Boolean)
        Dim lastDBEntryUpdate As String = GetMaxLastDBEntryUpdateFromLocalDatabase()
        Dim lastDownloadUpdate As String = GetMaxLastDownloadUpdateFromLocalDatabase()

        Dim deletedTasks As List(Of Integer) = FetchDeletedTasks()

        Using updatedTasks As DataTable = FetchUpdatedTasks(lastDBEntryUpdate)
            Using updatedDownloads As DataTable = FetchUpdatedDownloads(lastDownloadUpdate)
                Dim updateResult As UpdateResult = UpdateLocalDatabase(updatedTasks, updatedDownloads, deletedTasks)

                Dim msgResults As String = String.Empty
                If updateResult.Success Then
                    If updateResult.RecordsAdded > 0 OrElse updateResult.RecordsUpdated > 0 OrElse updateResult.RecordsDeleted > 0 Then
                        ' Reopen the datatable
                        SaveOrReapplySort(True)
                        UpdateCurrentDBGrid()
                        SaveOrReapplySort(False)
                        msgResults = $"Database update successful.{Environment.NewLine}{updateResult.RecordsAdded} new or updated tasks{Environment.NewLine}{updateResult.RecordsUpdated} updated download counts{Environment.NewLine}{updateResult.RecordsDeleted} tasks deleted"
                    Else
                        msgResults = "Database update check successful - No new, updated, or deleted tasks."
                    End If
                    If showSuccessResults Then
                        Using New Centered_MessageBox()
                            MessageBox.Show(Me, msgResults, "Database update", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End Using
                    End If
                Else
                    msgResults = $"Database update check unsuccessful.{Environment.NewLine}{updateResult.ErrorMessage}"
                    Using New Centered_MessageBox()
                        MessageBox.Show(Me, msgResults, "Database update", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Using
                End If
            End Using
        End Using
    End Sub

    Private Function GetMaxLastDBEntryUpdateFromLocalDatabase() As String
        If _currentTaskDBEntries Is Nothing OrElse _currentTaskDBEntries.Rows.Count = 0 Then
            Return "1970-01-01 00:00:00"
        End If

        Dim maxLastUpdate As String = _currentTaskDBEntries.AsEnumerable().Max(Function(row) row.Field(Of String)("DBEntryUpdate"))
        Return maxLastUpdate
    End Function

    Private Function GetMaxLastDownloadUpdateFromLocalDatabase() As String
        If _currentTaskDBEntries Is Nothing OrElse _currentTaskDBEntries.Rows.Count = 0 Then
            Return "1970-01-01 00:00:00"
        End If

        Dim maxLastDownloadUpdate As String = _currentTaskDBEntries.AsEnumerable().Max(Function(row) row.Field(Of String)("LastDownloadUpdate"))
        Return maxLastDownloadUpdate
    End Function

    Private Function UpdateLocalDatabase(updatedTasks As DataTable, updatedDownloads As DataTable, deletedTasks As List(Of Integer)) As UpdateResult
        Dim result As New UpdateResult
        Dim recordsAdded As Integer = 0
        Dim recordsUpdated As Integer = 0
        Dim recordsDeleted As Integer = 0

        Try
            Using conn As New SQLiteConnection($"Data Source={_localTasksDatabaseFilePath};Version=3;")
                conn.Open()
                Using transaction = conn.BeginTransaction()

                    ' Update or insert tasks
                    For Each row As DataRow In updatedTasks.Rows
                        Dim cmd As New SQLiteCommand("INSERT OR REPLACE INTO Tasks (EntrySeqID, 
                                                                                    TaskID, 
                                                                                    Title, 
                                                                                    LastUpdate, 
                                                                                    SimDateTime, 
                                                                                    IncludeYear, 
                                                                                    SimDateTimeExtraInfo,  
                                                                                    MainAreaPOI,  
                                                                                    DepartureName,  
                                                                                    DepartureICAO,  
                                                                                    DepartureExtra,  
                                                                                    ArrivalName,  
                                                                                    ArrivalICAO,  
                                                                                    ArrivalExtra,  
                                                                                    SoaringRidge,  
                                                                                    SoaringThermals,  
                                                                                    SoaringWaves,  
                                                                                    SoaringDynamic,  
                                                                                    SoaringExtraInfo,  
                                                                                    DurationMin,  
                                                                                    DurationMax,  
                                                                                    DurationExtraInfo,  
                                                                                    TaskDistance,  
                                                                                    TotalDistance,  
                                                                                    RecommendedGliders,  
                                                                                    DifficultyRating,  
                                                                                    DifficultyExtraInfo,  
                                                                                    ShortDescription,  
                                                                                    LongDescription,  
                                                                                    WeatherSummary,  
                                                                                    Credits,  
                                                                                    Countries,  
                                                                                    RecommendedAddOns,  
                                                                                    MapImage,  
                                                                                    CoverImage,  
                                                                                    DBEntryUpdate,
                                                                                    RepostText,
                                                                                    LastUpdateDescription) 
                                                        VALUES (@EntrySeqID,  
                                                                @TaskID, 
                                                                @Title, 
                                                                @LastUpdate, 
                                                                @SimDateTime, 
                                                                @IncludeYear, 
                                                                @SimDateTimeExtraInfo, 
                                                                @MainAreaPOI, 
                                                                @DepartureName, 
                                                                @DepartureICAO, 
                                                                @DepartureExtra, 
                                                                @ArrivalName, 
                                                                @ArrivalICAO, 
                                                                @ArrivalExtra, 
                                                                @SoaringRidge, 
                                                                @SoaringThermals, 
                                                                @SoaringWaves, 
                                                                @SoaringDynamic, 
                                                                @SoaringExtraInfo, 
                                                                @DurationMin, 
                                                                @DurationMax, 
                                                                @DurationExtraInfo, 
                                                                @TaskDistance, 
                                                                @TotalDistance, 
                                                                @RecommendedGliders, 
                                                                @DifficultyRating, 
                                                                @DifficultyExtraInfo, 
                                                                @ShortDescription, 
                                                                @LongDescription, 
                                                                @WeatherSummary, 
                                                                @Credits, 
                                                                @Countries, 
                                                                @RecommendedAddOns, 
                                                                @MapImage, 
                                                                @CoverImage, 
                                                                @DBEntryUpdate,
                                                                @RepostText,
                                                                @LastUpdateDescription)", conn)

                        ' Add parameters and set their values
                        cmd.Parameters.AddWithValue("@EntrySeqID", row("EntrySeqID"))
                        cmd.Parameters.AddWithValue("@TaskID", row("TaskID"))
                        cmd.Parameters.AddWithValue("@Title", row("Title"))
                        cmd.Parameters.AddWithValue("@LastUpdate", row("LastUpdate"))
                        cmd.Parameters.AddWithValue("@SimDateTime", row("SimDateTime"))
                        cmd.Parameters.AddWithValue("@IncludeYear", row("IncludeYear"))
                        cmd.Parameters.AddWithValue("@SimDateTimeExtraInfo", If(IsDBNull(row("SimDateTimeExtraInfo")), DBNull.Value, row("SimDateTimeExtraInfo")))
                        cmd.Parameters.AddWithValue("@MainAreaPOI", If(IsDBNull(row("MainAreaPOI")), DBNull.Value, row("MainAreaPOI")))
                        cmd.Parameters.AddWithValue("@DepartureName", If(IsDBNull(row("DepartureName")), DBNull.Value, row("DepartureName")))
                        cmd.Parameters.AddWithValue("@DepartureICAO", If(IsDBNull(row("DepartureICAO")), DBNull.Value, row("DepartureICAO")))
                        cmd.Parameters.AddWithValue("@DepartureExtra", If(IsDBNull(row("DepartureExtra")), DBNull.Value, row("DepartureExtra")))
                        cmd.Parameters.AddWithValue("@ArrivalName", If(IsDBNull(row("ArrivalName")), DBNull.Value, row("ArrivalName")))
                        cmd.Parameters.AddWithValue("@ArrivalICAO", If(IsDBNull(row("ArrivalICAO")), DBNull.Value, row("ArrivalICAO")))
                        cmd.Parameters.AddWithValue("@ArrivalExtra", If(IsDBNull(row("ArrivalExtra")), DBNull.Value, row("ArrivalExtra")))
                        cmd.Parameters.AddWithValue("@SoaringRidge", row("SoaringRidge"))
                        cmd.Parameters.AddWithValue("@SoaringThermals", row("SoaringThermals"))
                        cmd.Parameters.AddWithValue("@SoaringWaves", row("SoaringWaves"))
                        cmd.Parameters.AddWithValue("@SoaringDynamic", row("SoaringDynamic"))
                        cmd.Parameters.AddWithValue("@SoaringExtraInfo", If(IsDBNull(row("SoaringExtraInfo")), DBNull.Value, row("SoaringExtraInfo")))
                        cmd.Parameters.AddWithValue("@DurationMin", If(IsDBNull(row("DurationMin")), DBNull.Value, row("DurationMin")))
                        cmd.Parameters.AddWithValue("@DurationMax", If(IsDBNull(row("DurationMax")), DBNull.Value, row("DurationMax")))
                        cmd.Parameters.AddWithValue("@DurationExtraInfo", If(IsDBNull(row("DurationExtraInfo")), DBNull.Value, row("DurationExtraInfo")))
                        cmd.Parameters.AddWithValue("@TaskDistance", row("TaskDistance"))
                        cmd.Parameters.AddWithValue("@TotalDistance", row("TotalDistance"))
                        cmd.Parameters.AddWithValue("@RecommendedGliders", If(IsDBNull(row("RecommendedGliders")), DBNull.Value, row("RecommendedGliders")))
                        cmd.Parameters.AddWithValue("@DifficultyRating", If(IsDBNull(row("DifficultyRating")), DBNull.Value, row("DifficultyRating")))
                        cmd.Parameters.AddWithValue("@DifficultyExtraInfo", If(IsDBNull(row("DifficultyExtraInfo")), DBNull.Value, row("DifficultyExtraInfo")))
                        cmd.Parameters.AddWithValue("@ShortDescription", If(IsDBNull(row("ShortDescription")), DBNull.Value, row("ShortDescription")))
                        cmd.Parameters.AddWithValue("@LongDescription", If(IsDBNull(row("LongDescription")), DBNull.Value, row("LongDescription")))
                        cmd.Parameters.AddWithValue("@WeatherSummary", If(IsDBNull(row("WeatherSummary")), DBNull.Value, row("WeatherSummary")))
                        cmd.Parameters.AddWithValue("@Credits", If(IsDBNull(row("Credits")), DBNull.Value, row("Credits")))
                        cmd.Parameters.AddWithValue("@Countries", If(IsDBNull(row("Countries")), DBNull.Value, row("Countries")))
                        cmd.Parameters.AddWithValue("@RecommendedAddOns", row("RecommendedAddOns"))
                        cmd.Parameters.AddWithValue("@MapImage", If(row("MapImage") Is Nothing OrElse IsDBNull(row("MapImage")) OrElse row("MapImage") = String.Empty, DBNull.Value, Convert.FromBase64String(row("MapImage").ToString())))
                        cmd.Parameters.AddWithValue("@CoverImage", If(row("CoverImage") Is Nothing OrElse IsDBNull(row("CoverImage")) OrElse row("CoverImage") = String.Empty, DBNull.Value, Convert.FromBase64String(row("CoverImage").ToString())))
                        cmd.Parameters.AddWithValue("@DBEntryUpdate", row("DBEntryUpdate"))
                        cmd.Parameters.AddWithValue("@RepostText", row("RepostText"))
                        cmd.Parameters.AddWithValue("@LastUpdateDescription", row("LastUpdateDescription"))

                        Dim rowsAffected = cmd.ExecuteNonQuery()
                        If rowsAffected = 1 Then
                            If row.RowState = DataRowState.Added Then
                                recordsAdded += 1
                            Else
                                recordsUpdated += 1
                            End If
                        End If
                    Next

                    ' Update download counts
                    For Each row As DataRow In updatedDownloads.Rows
                        Dim cmd As New SQLiteCommand("UPDATE Tasks SET TotDownloads = @TotDownloads, LastDownloadUpdate = @LastDownloadUpdate WHERE EntrySeqID = @EntrySeqID", conn)
                        cmd.Parameters.AddWithValue("@TotDownloads", row("TotDownloads"))
                        cmd.Parameters.AddWithValue("@LastDownloadUpdate", row("LastDownloadUpdate"))
                        cmd.Parameters.AddWithValue("@EntrySeqID", row("EntrySeqID"))

                        Dim rowsAffected = cmd.ExecuteNonQuery()
                        If rowsAffected = 1 Then
                            recordsUpdated += 1
                        End If
                    Next

                    ' Process deleted tasks
                    For Each entrySeqID In deletedTasks
                        Dim cmd As New SQLiteCommand("DELETE FROM Tasks WHERE EntrySeqID = @EntrySeqID", conn)
                        cmd.Parameters.AddWithValue("@EntrySeqID", entrySeqID)

                        Dim rowsAffected = cmd.ExecuteNonQuery()
                        If rowsAffected = 1 Then
                            recordsDeleted += 1
                        End If
                    Next

                    transaction.Commit()
                End Using
            End Using

            result.Success = True
            result.RecordsAdded = recordsAdded
            result.RecordsUpdated = recordsUpdated
            result.RecordsDeleted = recordsDeleted
        Catch ex As Exception
            result.Success = False
            result.ErrorMessage = ex.Message
        End Try

        Return result
    End Function

    Private Function IncrementDownloadForTask(entrySeqID As String) As Boolean
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}IncrementDownloadForTask.php?EntrySeqID={entrySeqID}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"
        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Return UpdateLocalDatabaseWithResponse(jsonResponse, entrySeqID)
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Throw
        End Try
    End Function

    Private Function IncrementThreadAccessForTask(entrySeqID As String) As Boolean
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}IncrementThreadAccessForTask.php?EntrySeqID={entrySeqID}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"
        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Return True
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Throw
        End Try
    End Function

    Private Function UpdateLocalDatabaseWithResponse(jsonResponse As String, entrySeqID As String) As Boolean
        Dim status As String = "failure"
        Dim totDownloads As Integer = 0
        Dim lastDownloadUpdate As String = ""

        Try
            Dim json As JObject = JObject.Parse(jsonResponse)
            If json("status") IsNot Nothing AndAlso json("status").ToString() = "success" Then
                status = "success"
                totDownloads = Integer.Parse(json("TotDownloads").ToString())
                lastDownloadUpdate = json("LastDownloadUpdate").ToString()

                ' Update the local database
                Using conn As New SQLiteConnection($"Data Source={_localTasksDatabaseFilePath};Version=3;")
                    conn.Open()
                    Dim query As String = "UPDATE Tasks SET TotDownloads = @TotDownloads, LastDownloadUpdate = @LastDownloadUpdate WHERE EntrySeqID = @EntrySeqID"
                    Using cmd As New SQLiteCommand(query, conn)
                        cmd.Parameters.AddWithValue("@TotDownloads", totDownloads)
                        cmd.Parameters.AddWithValue("@LastDownloadUpdate", lastDownloadUpdate)
                        cmd.Parameters.AddWithValue("@EntrySeqID", entrySeqID)
                        cmd.ExecuteNonQuery()
                    End Using
                    conn.Close()
                End Using

                SaveCurrentPosition()
                ' Find the row in the DataTable and update it
                Dim sourceDataTable As DataTable = CType(gridCurrentDatabase.DataSource, DataTable)
                For Each row As DataRow In sourceDataTable.Rows
                    If row("EntrySeqID").ToString() = entrySeqID Then
                        row.BeginEdit()
                        row("TotDownloads") = totDownloads
                        row("LastDownloadUpdate") = lastDownloadUpdate
                        row.EndEdit()
                        ' Notify that row has been changed
                        sourceDataTable.AcceptChanges()
                        Exit For
                    End If
                Next
                Dim bindingSource As BindingSource = CType(gridCurrentDatabase.DataSource, BindingSource)
                bindingSource.ResetBindings(False)
                RestorePosition()

                Return True
            End If
        Catch ex As Exception
            ' Handle the exception (e.g., log the error)
            status = "failure"
        End Try

        Return False
    End Function

    Private Function DownloadTaskFile(taskID As String, taskTitle As String, localFolder As String) As String
        Dim baseUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}TaskBrowser/Tasks/"
        Dim remoteFileName As String = taskID & ".dphx"
        Dim localFileName As String = taskTitle & ".dphx"
        Dim remoteFileUrl As String = baseUrl & remoteFileName
        Dim localFilePath As String = Path.Combine(localFolder, localFileName)

        Try
            ' Create the directory if it doesn't exist
            If Not Directory.Exists(localFolder) Then
                Directory.CreateDirectory(localFolder)
            End If

            ' Download the file
            Using client As New WebClient()
                client.DownloadFile(remoteFileUrl, localFilePath)
            End Using

            ' Check if the file exists
            If File.Exists(localFilePath) Then
                Return localFilePath
            Else
                Throw New Exception("Failed to download the task file.")
            End If

        Catch ex As Exception
            ' Handle the exception (e.g., log the error)
            Using New Centered_MessageBox()
                MessageBox.Show(Me, $"An error occurred while downloading the task file:{Environment.NewLine}{ex.Message}", "Downloading task", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return String.Empty
        End Try
    End Function

    Private ReadOnly Property LocalDPHXFileName As String
        Get
            If _selectedTaskRow IsNot Nothing Then
                Return Path.Combine(Settings.SessionSettings.PackagesFolder, _selectedTaskRow("Title").ToString() & ".dphx")
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private Function GetListOfExcludedColumnNames() As List(Of String)
        ' List of column names to exclude from the visibility toggle
        Return New List(Of String) From {
            "TaskID",
            "SimDateTime",
            "IncludeYear",
            "SimDateTimeExtraInfo",
            "DepartureExtra",
            "ArrivalExtra",
            "DurationMin",
            "DurationMax",
            "TaskDistance",
            "TotalDistance",
            "DifficultyRating",
            "DifficultyExtraInfo",
            "ShortDescription",
            "LongDescription",
            "LastDownloadUpdate",
            "SoaringRidge",
            "SoaringThermals",
            "SoaringWaves",
            "SoaringDynamic",
            "RecommendedAddOns",
            "MapImage",
            "CoverImage",
            "IncludeYearBool",
            "DBEntryUpdate",
            "ThreadAccess",
            "Comment",
            "TaskFlown",
            "ToFly",
            "RepostText",
            "LastUpdateDescription"}

    End Function

    Private Sub RemoveEventHandlers()

        For Each tmi As ToolStripMenuItem In _handlerFavoriteReplaceSearchItem_Click
            RemoveHandler tmi.Click, AddressOf FavoriteReplaceSearchItem_Click
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerFavoriteReplaceSearchItem_Click.Clear()

        For Each tmi As ToolStripMenuItem In _handlerFavoriteAddToSearchItem_Click
            RemoveHandler tmi.Click, AddressOf FavoriteAddToSearchItem_Click
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerFavoriteAddToSearchItem_Click.Clear()

        For Each tmi As ToolStripMenuItem In _handlerFavoriteRenameItem_Click
            RemoveHandler tmi.Click, AddressOf FavoriteRenameItem_Click
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerFavoriteRenameItem_Click.Clear()

        For Each tmi As ToolStripMenuItem In _handlerFavoriteDeleteItem_Click
            RemoveHandler tmi.Click, AddressOf FavoriteDeleteItem_Click
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerFavoriteDeleteItem_Click.Clear()

        For Each tmi As ToolStripMenuItem In _handlerColumnVisibilityToggle
            RemoveHandler tmi.Click, AddressOf ColumnVisibilityToggle
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerColumnVisibilityToggle.Clear()

        For Each tmi As ToolStripMenuItem In _handlerTextCriteriaMenuItem_Click
            RemoveHandler tmi.Click, AddressOf TextCriteriaMenuItem_Click
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerTextCriteriaMenuItem_Click.Clear()

        For Each tmi As ToolStripMenuItem In _handlerNumberCriteriaMenuItem_Click
            RemoveHandler tmi.Click, AddressOf NumberCriteriaMenuItem_Click
            tmi.Dispose()
            tmi = Nothing
        Next
        _handlerNumberCriteriaMenuItem_Click.Clear()

        RemoveHandler btnSmallerText.Click, AddressOf btnSmallerText_Click
        RemoveHandler btnBiggerText.Click, AddressOf btnBiggerText_Click
        RemoveHandler btnDownloadOpen.Click, AddressOf btnDownloadOpen_Click
        RemoveHandler AddNewFavoriteToolStripMenuItem.Click, AddressOf AddNewFavoriteToolStripMenuItem_Click
        RemoveHandler txtSearch.TextChanged, AddressOf txtSearch_TextChanged
        RemoveHandler chkNot.CheckedChanged, AddressOf chkNot_CheckedChanged
        RemoveHandler txtSearch.KeyDown, AddressOf txtSearch_KeyDown
        RemoveHandler btnResetSearch.Click, AddressOf btnResetSearch_Click
        RemoveHandler btnSearchBack.Click, AddressOf btnSearchBack_Click
        RemoveHandler btnViewInLibrary.Click, AddressOf btnViewInLibrary_Click
        RemoveHandler btnUpdateDB.Click, AddressOf btnUpdateDB_Click
        RemoveHandler gridCurrentDatabase.RowEnter, AddressOf gridCurrentDatabase_RowEnter
        RemoveHandler gridCurrentDatabase.Resize, AddressOf gridCurrentDatabase_Resize
        RemoveHandler gridCurrentDatabase.DataSourceChanged, AddressOf gridCurrentDatabase_DataSourceChanged
        RemoveHandler gridCurrentDatabase.ColumnHeaderMouseClick, AddressOf gridCurrentDatabase_ColumnHeaderMouseClick
        RemoveHandler Me.FormClosing, AddressOf TaskBrowser_FormClosing

    End Sub

    Private Sub SaveUserData()
        Try
            ' Update the local database
            Using conn As New SQLiteConnection($"Data Source={_localTasksDatabaseFilePath};Version=3;")
                conn.Open()
                Dim query As String = "
                INSERT OR REPLACE INTO UserData (EntrySeqID, TaskQualityRating, TaskDifficultyRating, Comment, Tags, TaskFlown, ToFly)
                VALUES (@EntrySeqID, @TaskQualityRating, @TaskDifficultyRating, @Comment, @Tags, @TaskFlown, @ToFly)"
                Using cmd As New SQLiteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@EntrySeqID", _selectedTaskRow("EntrySeqID"))
                    cmd.Parameters.AddWithValue("@TaskQualityRating", trkQualityRating.Value)
                    cmd.Parameters.AddWithValue("@TaskDifficultyRating", trkDifficultyRating.Value)
                    cmd.Parameters.AddWithValue("@Comment", txtComment.Text.Trim)
                    cmd.Parameters.AddWithValue("@Tags", txtTags.Text.Trim)
                    cmd.Parameters.AddWithValue("@TaskFlown", If(chkTaskFlown.Checked, 1, 0))
                    cmd.Parameters.AddWithValue("@ToFly", If(chkToFly.Checked, 1, 0))
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            _selectedTaskRow("TaskQualityRating") = trkQualityRating.Value
            _selectedTaskRow("TaskDifficultyRating") = trkDifficultyRating.Value
            _selectedTaskRow("Comment") = txtComment.Text.Trim
            _selectedTaskRow("Tags") = txtTags.Text.Trim
            _selectedTaskRow("TaskFlown") = chkTaskFlown.Checked
            _selectedTaskRow("ToFly") = chkToFly.Checked

            'As it stands now, we don't have a choice but to reset the grid for the changes to be visible

            'Save position and sort
            SaveCurrentPosition()
            SaveOrReapplySort(True)

            'Reload the datasource and grid
            UpdateCurrentDBGrid()
            _filteredDataTable = Nothing

            'reapply sort and restore position
            SaveOrReapplySort(False)
            RestorePosition()

            If _searchTerms.Count > 0 Then
                Using New Centered_MessageBox()
                    If MessageBox.Show(Me, "Would you like to reapply the search?", "Reapply search", vbYesNo, vbQuestion) = vbYes Then
                        For Each searchTerm As String In _searchTerms
                            PerformSearch(searchTerm)
                        Next
                    Else
                        lblSearchTerms.Text = String.Empty
                        txtSearch.Text = String.Empty
                        _searchTerms.Clear()
                    End If
                End Using
            End If

        Catch ex As Exception
            ' Handle the exception (e.g., log the error)
            Throw
        End Try
    End Sub

    Private Async Sub InitializeWebView2()

        If Not IsWebView2RuntimeInstalled() Then
            Dim result = MessageBox.Show("WebView2 Runtime is not installed. Would you like to install it now?", "WebView2 Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                InstallWebView2Runtime()
            Else
                btnInstallWebView.Visible = True
                Exit Sub
            End If
        End If

        btnInstallWebView.Visible = False

        Try
            ' Clear WebView2 cache before initialization
            ClearWebView2Cache()

            ' Attempt to create the WebView2 environment
            Dim env As CoreWebView2Environment = Await CoreWebView2Environment.CreateAsync()
            Await webView.EnsureCoreWebView2Async(env)
            webView.Source = New Uri("https://wesimglide.org/integrated.html?appContext=true")
        Catch ex As Exception
            MessageBox.Show("Error initializing the map view: " & ex.Message)
        End Try
    End Sub

    Private Sub ClearWebView2Cache()
        ' Get the default user data folder location for WebView2
        Dim userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "WebView2", "User Data")

        ' Delete the user data folder if it exists
        If Directory.Exists(userDataFolder) Then
            Try
                Directory.Delete(userDataFolder, True)
            Catch ex As Exception
                MessageBox.Show("Error clearing WebView2 cache: " & ex.Message)
            End Try
        End If
    End Sub

    Private Function IsWebView2RuntimeInstalled() As Boolean
        Try
            Dim env = CoreWebView2Environment.GetAvailableBrowserVersionString()
            Return Not String.IsNullOrEmpty(env)
        Catch ex As COMException
            Return False
        End Try
    End Function

    Private Sub InstallWebView2Runtime()
        Dim bootstrapperLink As String = "https://go.microsoft.com/fwlink/p/?LinkId=2124703"
        Dim bootstrapperPath As String = IO.Path.Combine(Application.StartupPath, "MicrosoftEdgeWebView2Setup.exe")

        ' Download the bootstrapper
        Using client As New WebClient()
            Try
                client.DownloadFile(bootstrapperLink, bootstrapperPath)
                ' Run the downloaded bootstrapper
                Dim psi As New ProcessStartInfo(bootstrapperPath) With {
                    .UseShellExecute = True
                }
                Dim process As Process = Process.Start(psi)
                process.WaitForExit() ' Wait for the installer to finish

                ' Try to reinitialize WebView2 after installation
                InitializeWebView2()
            Catch ex As Exception
                MessageBox.Show("Failed to download or run WebView2 bootstrapper. Please install it from: " & bootstrapperLink)
            End Try
        End Using
    End Sub

#End Region

End Class

Public Class UpdateResult
    Public Property Success As Boolean
    Public Property RecordsAdded As Integer
    Public Property RecordsUpdated As Integer
    Public Property RecordsDeleted As Integer
    Public Property ErrorMessage As String
End Class
