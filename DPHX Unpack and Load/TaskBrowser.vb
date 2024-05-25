Imports System.ComponentModel
Imports System.Data.SQLite
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Public Class TaskBrowser

#Region "Constants and variables"

    Public DownloadedFilePath As String = String.Empty
    Private _localTasksDatabaseFilePath As String
    Private _currentTaskDBEntries As DataTable
    Private _dataGridViewAllSet As Boolean = False
    Private _selectedTaskRow As DataRow
    Private _filteredDataTable As DataTable
    Private _searchTerms As New List(Of String)
    Private scrollIndex As Integer
    Private selectedRowIndex As Integer
    Private horizontalScrollIndex As Integer
    Private Shared _processingChange As Boolean = False

    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private Shared ReadOnly Property PrefUnits As New PreferredUnits

#End Region

#Region "Events"

    Private Sub TaskBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        lblCurrentSelection.Text = String.Empty

        _localTasksDatabaseFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TasksDatabase.db")

        splitMain.SplitterDistance = splitMain.Width * (Settings.SessionSettings.TaskLibrarySplitterLocation / 100)
        splitRightPart.SplitterDistance = splitRightPart.Height * (Settings.SessionSettings.TaskLibraryRightPartSplitterLocation / 100)
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
        AddFieldWithTextCriteria("global", "Everywhere")
        AddFieldWithNumbersCriteria("duration", "Duration")
        AddFieldWithNumbersCriteria("distance", "Distance (in KM)")
        AddFieldWithNumbersCriteria("totdown", "Total downloads")

        BuildFavoritesMenu()

        CheckForUpdates(False)

    End Sub

    Private Sub btnDownloadOpen_Click(sender As Object, e As EventArgs) Handles btnDownloadOpen.Click

        If btnDownloadOpen.Text = "Open" Then
            'Already exists locally
            DownloadedFilePath = LocalDPHXFileName
        Else
            'Call the script to increment the download count by 1
            If IncrementDownloadForTask(_selectedTaskRow("EntrySeqID")) Then
                'Success
            End If
            'Download
            DownloadedFilePath = DownloadTaskFile(_selectedTaskRow("TaskID").ToString(), _selectedTaskRow("Title").ToString(), Settings.SessionSettings.PackagesFolder)
        End If

        If Not String.IsNullOrEmpty(DownloadedFilePath) Then
            Me.Close()
        End If

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
                                                                                         AddOnsToolStripMenuItem.Click
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

        gridCurrentDatabase.DataSource = _currentTaskDBEntries
        _filteredDataTable = Nothing
        lblSearchTerms.Text = String.Empty
        txtSearch.Text = String.Empty
        _searchTerms.Clear()

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
    End Sub

    Private Sub gridCurrentDatabase_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles gridCurrentDatabase.RowEnter

        Dim selectedEntrySeqID As Integer = gridCurrentDatabase.Rows(e.RowIndex).Cells("EntrySeqID").Value
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

    Private Sub gridCurrentDatabase_Resize(sender As Object, e As EventArgs) Handles gridCurrentDatabase.Resize
        CheckColumnSizes()
    End Sub

    Private Sub gridCurrentDatabase_DataSourceChanged(sender As Object, e As EventArgs) Handles gridCurrentDatabase.DataSourceChanged
        SetupDataGridView()
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
#End Region

#Region "Subs and functions"

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
            Using cmd As New SQLiteCommand("SELECT * FROM Tasks", conn)
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
        _currentTaskDBEntries.Columns.Add(includeYearColumn)
        _currentTaskDBEntries.Columns.Add(soaringRidgeColumn)
        _currentTaskDBEntries.Columns.Add(soaringThermalsColumn)
        _currentTaskDBEntries.Columns.Add(soaringWavesColumn)
        _currentTaskDBEntries.Columns.Add(soaringDynamicColumn)
        _currentTaskDBEntries.Columns.Add(recommendedAddOnsColumn)

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
        Next
    End Sub

    Private Sub SaveDataGridViewSettings()
        If gridCurrentDatabase.SortedColumn IsNot Nothing Then
            Settings.SessionSettings.TaskLibrarySortColumn = gridCurrentDatabase.SortedColumn.Name
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
        gridCurrentDatabase.Columns("TaskID").Visible = False
        gridCurrentDatabase.Columns("SimDateTime").Visible = False
        gridCurrentDatabase.Columns("IncludeYear").Visible = False
        gridCurrentDatabase.Columns("IncludeYearBool").Visible = False
        gridCurrentDatabase.Columns("SimDateTimeExtraInfo").Visible = False
        gridCurrentDatabase.Columns("DepartureExtra").Visible = False
        gridCurrentDatabase.Columns("ArrivalExtra").Visible = False
        gridCurrentDatabase.Columns("DurationMin").Visible = False
        gridCurrentDatabase.Columns("DurationMax").Visible = False
        gridCurrentDatabase.Columns("TaskDistance").Visible = False
        gridCurrentDatabase.Columns("TotalDistance").Visible = False
        gridCurrentDatabase.Columns("DifficultyRating").Visible = False
        gridCurrentDatabase.Columns("DifficultyExtraInfo").Visible = False
        gridCurrentDatabase.Columns("ShortDescription").Visible = False
        gridCurrentDatabase.Columns("LongDescription").Visible = False
        gridCurrentDatabase.Columns("MapImage").Visible = False
        gridCurrentDatabase.Columns("CoverImage").Visible = False
        gridCurrentDatabase.Columns("SoaringRidge").Visible = False
        gridCurrentDatabase.Columns("SoaringThermals").Visible = False
        gridCurrentDatabase.Columns("SoaringWaves").Visible = False
        gridCurrentDatabase.Columns("SoaringDynamic").Visible = False
        gridCurrentDatabase.Columns("RecommendedAddOns").Visible = False
        gridCurrentDatabase.Columns("LastDownloadUpdate").Visible = False
        gridCurrentDatabase.Columns("DBEntryUpdate").Visible = False

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

        If gridCurrentDatabase IsNot Nothing Then
            For Each setting In Settings.SessionSettings.TBColumnsSettings
                If gridCurrentDatabase.Columns.Contains(setting.Name) Then
                    gridCurrentDatabase.Columns(setting.Name).Visible = setting.Visible
                    gridCurrentDatabase.Columns(setting.Name).DisplayIndex = setting.DisplayIndex
                    If setting.ColumnWidth > 0 Then
                        gridCurrentDatabase.Columns(setting.Name).Width = setting.ColumnWidth
                    Else

                    End If
                    Select Case setting.Name
                        Case "DurationConcat", "DistancesConcat", "TotDownloads"
                            gridCurrentDatabase.Columns(setting.Name).SortMode = DataGridViewColumnSortMode.Programmatic
                        Case Else
                            gridCurrentDatabase.Columns(setting.Name).SortMode = DataGridViewColumnSortMode.Automatic
                    End Select
                    gridCurrentDatabase.Columns(setting.Name).Tag = setting.Name
                End If
            Next
        End If

        _dataGridViewAllSet = True

    End Sub

    Private Sub BuildTaskData()

        Dim dateFormat As String
        If _selectedTaskRow("IncludeYear") Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        'Title
        sb.Append($"\b {_selectedTaskRow("Title")}\b0\line ")
        If _selectedTaskRow("MainAreaPOI").ToString().Trim <> String.Empty Then
            sb.Append($"{_selectedTaskRow("MainAreaPOI")}\line ")
            sb.Append("\line ")
        End If
        If _selectedTaskRow("ShortDescription").ToString().Trim <> String.Empty Then
            sb.Append($"{_selectedTaskRow("ShortDescription")}\line ")
            sb.Append("\line ")
        End If

        'Credits
        sb.Append($"{_selectedTaskRow("Credits")}\line ")
        sb.Append("\line ")

        'Local MSFS date and time 
        sb.Append($"MSFS Local date & time is \b {Convert.ToDateTime(_selectedTaskRow("SimDateTime")).ToString(dateFormat, _EnglishCulture)}, {Convert.ToDateTime(_selectedTaskRow("SimDateTime")).ToString("hh:mm tt", _EnglishCulture)} {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("SimDateTimeExtraInfo").ToString().Trim, True, True)}\b0\line ")

        'Departure airfield And runway
        sb.Append($"You will depart from \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DepartureICAO").ToString())}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DepartureName").ToString(), True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DepartureExtra").ToString(), True, True)}\b0\line ")

        'Arrival airfield And expected runway
        sb.Append($"You will land at \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("ArrivalICAO").ToString())}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("ArrivalName").ToString(), True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("ArrivalExtra").ToString(), True, True)}\b0\line ")

        'Type of soaring
        Dim soaringType As String = SupportingFeatures.GetSoaringTypesSelected(Convert.ToBoolean(_selectedTaskRow("SoaringRidge")), Convert.ToBoolean(_selectedTaskRow("SoaringThermals")), Convert.ToBoolean(_selectedTaskRow("SoaringWaves")), Convert.ToBoolean(_selectedTaskRow("SoaringDynamic")))
        If soaringType.Trim <> String.Empty OrElse _selectedTaskRow("SoaringExtraInfo").ToString() <> String.Empty Then
            sb.Append($"Soaring Type is \b {soaringType}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("SoaringExtraInfo").ToString(), True, True)}\b0\line ")
        End If

        'Task distance And total distance
        sb.Append($"Distance are \b {_selectedTaskRow("DistancesConcat")}\b0\line ")

        'Approx. duration
        sb.Append($"Approx. duration should be \b {_selectedTaskRow("DurationConcat")} {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("DurationExtraInfo").ToString(), True, True)}\b0\line ")

        'Recommended gliders
        If _selectedTaskRow("RecommendedGliders").ToString().Trim <> String.Empty Then
            sb.Append($"Recommended gliders: \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("RecommendedGliders").ToString())}\b0\line ")
        End If

        'Difficulty rating
        If _selectedTaskRow("DifficultyRating").ToString().Trim <> String.Empty OrElse _selectedTaskRow("DifficultyExtraInfo").ToString().Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as \b {SupportingFeatures.GetDifficulty(CInt(_selectedTaskRow("DifficultyRating").ToString().Substring(0, 1)), _selectedTaskRow("DifficultyExtraInfo").ToString(), True)}\b0\line ")
        End If

        sb.Append("\line ")

        If _selectedTaskRow("WeatherSummary").ToString() <> String.Empty Then
            sb.Append($"Weather summary: \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTaskRow("WeatherSummary").ToString())}\b0\line ")
            sb.Append("\line ")
        End If

        'Build full description
        If _selectedTaskRow("LongDescription").ToString() <> String.Empty Then
            sb.AppendLine("**Full Description**($*$)")
            sb.AppendLine(_selectedTaskRow("LongDescription").ToString())
        End If

        sb.Append("}")

        Dim oldZoomFactor As Single = txtBriefing.ZoomFactor
        'If oldZoomFactor = 1 Then
        'oldZoomFactor = 1.1
        'End If
        txtBriefing.ZoomFactor = 1
        SupportingFeatures.FormatMarkdownToRTF(sb.ToString(), txtBriefing)
        txtBriefing.ZoomFactor = oldZoomFactor

        ' Set images to PictureBox controls
        If Not IsDBNull(_selectedTaskRow("MapImage")) Then
            Dim mapImageBytes As Byte() = CType(_selectedTaskRow("MapImage"), Byte())
            imgMap.Image = ByteArrayToImage(mapImageBytes)
        Else
            imgMap.Image = Nothing
        End If

        If Not IsDBNull(_selectedTaskRow("CoverImage")) Then
            Dim coverImageBytes As Byte() = CType(_selectedTaskRow("CoverImage"), Byte())
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
                Case "ridge", "thermals", "waves", "dynamic", "addons"
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
                Case "duration", "distance", "totdown"
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

        If filteredRows.Any() Then
            _filteredDataTable = filteredRows.CopyToDataTable()
            gridCurrentDatabase.DataSource = _filteredDataTable
        Else
            gridCurrentDatabase.DataSource = _currentTaskDBEntries.Clone() ' Return an empty DataTable with the same schema
        End If

        Return validSearch

    End Function

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
        AddHandler addToSearchItem.Click, AddressOf FavoriteAddToSearchItem_Click
        AddHandler renameItem.Click, AddressOf FavoriteRenameItem_Click
        AddHandler deleteItem.Click, AddressOf FavoriteDeleteItem_Click

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
        AddHandler startsWithItem.Click, AddressOf TextCriteriaMenuItem_Click
        AddHandler endsWithItem.Click, AddressOf TextCriteriaMenuItem_Click
        AddHandler exactlyItem.Click, AddressOf TextCriteriaMenuItem_Click

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

        ' Add the field menu item to the main TextCriteriaToolStripMenuItem
        NumbersCriteriaToolStripMenuItem.DropDownItems.Add(fieldMenuItem)
    End Sub

    Private Function FetchUpdatedTasks(lastDBEntryUpdate As String) As DataTable
        Dim apiUrl As String = $"https://siglr.com/DiscordPostHelper/GetUpdatedTasks.php?DBEntryUpdate={lastDBEntryUpdate}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            Using reader As New IO.StreamReader(response.GetResponseStream())
                Dim jsonResponse As String = reader.ReadToEnd()
                Return ConvertJsonToDataTable(jsonResponse)
            End Using
        End Using
    End Function

    Private Function FetchUpdatedDownloads(lastDownloadUpdate As String) As DataTable
        Dim apiUrl As String = $"https://siglr.com/DiscordPostHelper/GetUpdatedDownloads.php?lastDownloadUpdate={lastDownloadUpdate}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            Using reader As New IO.StreamReader(response.GetResponseStream())
                Dim jsonResponse As String = reader.ReadToEnd()
                Return ConvertJsonToDataTable(jsonResponse)
            End Using
        End Using
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

        Dim updatedTasks As DataTable = FetchUpdatedTasks(lastDBEntryUpdate)
        Dim updatedDownloads As DataTable = FetchUpdatedDownloads(lastDownloadUpdate)

        Dim updateResult As UpdateResult = UpdateLocalDatabase(updatedTasks, updatedDownloads)

        Dim msgResults As String = String.Empty
        If updateResult.Success Then
            If updateResult.RecordsAdded > 0 OrElse updateResult.RecordsUpdated > 0 Then
                'Reopen the datatable
                UpdateCurrentDBGrid()
                msgResults = $"Database update successful.{Environment.NewLine}{updateResult.RecordsAdded} new or updated tasks{Environment.NewLine}{updateResult.RecordsUpdated} updated download counts"
            Else
                msgResults = $"Database update check successful - No new or updated task."
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

    Private Function UpdateLocalDatabase(updatedTasks As DataTable, updatedDownloads As DataTable) As UpdateResult
        Dim result As New UpdateResult
        Dim recordsAdded As Integer = 0
        Dim recordsUpdated As Integer = 0

        Try
            Using conn As New SQLiteConnection($"Data Source={_localTasksDatabaseFilePath};Version=3;")
                conn.Open()
                Using transaction = conn.BeginTransaction()

                    ' Update or insert tasks
                    For Each row As DataRow In updatedTasks.Rows
                        Dim cmd As New SQLiteCommand("INSERT OR REPLACE INTO Tasks (EntrySeqID, TaskID, Title, LastUpdate, SimDateTime, IncludeYear, SimDateTimeExtraInfo, MainAreaPOI, DepartureName, DepartureICAO, DepartureExtra, ArrivalName, ArrivalICAO, ArrivalExtra, SoaringRidge, SoaringThermals, SoaringWaves, SoaringDynamic, SoaringExtraInfo, DurationMin, DurationMax, DurationExtraInfo, TaskDistance, TotalDistance, RecommendedGliders, DifficultyRating, DifficultyExtraInfo, ShortDescription, LongDescription, WeatherSummary, Credits, Countries, RecommendedAddOns, MapImage, CoverImage, DBEntryUpdate) VALUES (@EntrySeqID, @TaskID, @Title, @LastUpdate, @SimDateTime, @IncludeYear, @SimDateTimeExtraInfo, @MainAreaPOI, @DepartureName, @DepartureICAO, @DepartureExtra, @ArrivalName, @ArrivalICAO, @ArrivalExtra, @SoaringRidge, @SoaringThermals, @SoaringWaves, @SoaringDynamic, @SoaringExtraInfo, @DurationMin, @DurationMax, @DurationExtraInfo, @TaskDistance, @TotalDistance, @RecommendedGliders, @DifficultyRating, @DifficultyExtraInfo, @ShortDescription, @LongDescription, @WeatherSummary, @Credits, @Countries, @RecommendedAddOns, @MapImage, @CoverImage, @DBEntryUpdate)", conn)

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
                        cmd.Parameters.AddWithValue("@MapImage", If(row("MapImage") Is Nothing OrElse row("MapImage") = String.Empty OrElse IsDBNull(row("MapImage")), DBNull.Value, Convert.FromBase64String(row("MapImage").ToString())))
                        cmd.Parameters.AddWithValue("@CoverImage", If(row("CoverImage") Is Nothing OrElse row("CoverImage") = String.Empty OrElse IsDBNull(row("CoverImage")), DBNull.Value, Convert.FromBase64String(row("CoverImage").ToString())))
                        cmd.Parameters.AddWithValue("@DBEntryUpdate", row("DBEntryUpdate"))


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

                    transaction.Commit()
                End Using
            End Using

            result.Success = True
            result.RecordsAdded = recordsAdded
            result.RecordsUpdated = recordsUpdated
        Catch ex As Exception
            result.Success = False
            result.ErrorMessage = ex.Message
        End Try

        Return result
    End Function

    Private Function IncrementDownloadForTask(entrySeqID As String) As Boolean
        Dim apiUrl As String = $"https://siglr.com/DiscordPostHelper/IncrementDownloadForTask.php?EntrySeqID={entrySeqID}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            Using reader As New IO.StreamReader(response.GetResponseStream())
                Dim jsonResponse As String = reader.ReadToEnd()
                Return UpdateLocalDatabaseWithResponse(jsonResponse, entrySeqID)
            End Using
        End Using
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
        Dim baseUrl As String = "https://siglr.com/DiscordPostHelper/TaskBrowser/Tasks/"
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
            "DPHXFilename",
            "IsUpdate",
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
            "DBEntryUpdate"}

    End Function
#End Region

End Class

Public Class UpdateResult
    Public Property Success As Boolean
    Public Property RecordsAdded As Integer
    Public Property RecordsUpdated As Integer
    Public Property ErrorMessage As String
End Class
