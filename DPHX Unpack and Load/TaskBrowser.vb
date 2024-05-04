Imports System.ComponentModel
Imports System.Diagnostics.Eventing.Reader
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

Public Class TaskBrowser

#Region "Constants and variables"

    Private _localXmlFilePath As String
    Private _currentTaskDBEntries As Dictionary(Of String, TBTaskData)
    Private _dataGridViewAllSet As Boolean = False
    Private _selectedTask As TBTaskData
    Private _currentSortCol As String = "Title"
    Private _currentSortAsc As Boolean = True
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")

#End Region

#Region "Events"

    Private Sub TaskBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        _currentTaskDBEntries = New Dictionary(Of String, TBTaskData)

        _localXmlFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TBTaskDatabase.xml")

        UpdateCurrentDBGrid()

        If File.Exists(_localXmlFilePath) Then
            'Get local DB timestamp
            lblLocalDBTimestamp.Text = Settings.SessionSettings.LocalDBTimestamp
        Else
            lblLocalDBTimestamp.Text = "None"
        End If

        RetrieveOnlineDBTimestamp()

        ' Set this form's location and size to match the parent form's
        If Me.Owner IsNot Nothing Then
            Me.Size = Me.Owner.Size
            Me.Location = Me.Owner.Location
        End If
        AddColumnsVisibilityOptions()

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
        Dim excludeColumns As New List(Of String) From {"TaskID",
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
            "LongDescription"}

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

    Private Sub btnRetrieveOnlineDBTimestamp_Click(sender As Object, e As EventArgs) Handles btnRetrieveOnlineDBTimestamp.Click
        RetrieveOnlineDBTimestamp()
    End Sub

    Private Sub btnUpdateLocalDB_Click(sender As Object, e As EventArgs) Handles btnUpdateLocalDB.Click

        If DownloadOnlineDB() Then

        End If

    End Sub

    Private Sub TaskBrowser_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SaveDataGridViewSettings()
    End Sub

    Private Sub gridCurrentDatabase_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles gridCurrentDatabase.RowEnter

        Dim taskID As String = gridCurrentDatabase.Rows(e.RowIndex).Cells(0).Value
        _selectedTask = _currentTaskDBEntries(taskID)
        BuildTaskData()

    End Sub

    Private Sub gridCurrentDatabase_Resize(sender As Object, e As EventArgs) Handles gridCurrentDatabase.Resize
        CheckColumnSizes()
    End Sub

    Private Sub gridCurrentDatabase_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles gridCurrentDatabase.ColumnHeaderMouseClick

        ' Get the name of the column that was clicked
        Dim clickedColumn As String = gridCurrentDatabase.Columns(e.ColumnIndex).Name

        ' Remove sort indicators from all column headers
        For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
            col.HeaderText = col.HeaderText.Replace(" ↑", "").Replace(" ↓", "")
        Next

        ' Check if the clicked column is the same as the current sorting column
        If clickedColumn = _currentSortCol Then
            ' Same column clicked, toggle the sorting order
            _currentSortAsc = Not _currentSortAsc
        Else
            ' Different column clicked, change the sorting column and set order to ascending
            _currentSortCol = clickedColumn
            _currentSortAsc = True
        End If

        'Remember which task was selected
        Dim currentlySelectedTaskID = gridCurrentDatabase.CurrentRow.Cells("TaskID").Value

        ' Reload the data source sorted by the new column and order
        gridCurrentDatabase.DataSource = LoadAndSortXML(_currentSortCol, _currentSortAsc)

        'Reselect the task that was selected prior to the new sort
        For Each row As DataGridViewRow In gridCurrentDatabase.Rows
            If Convert.ToString(row.Cells("TaskID").Value) = currentlySelectedTaskID Then
                gridCurrentDatabase.CurrentCell = row.Cells(_currentSortCol) ' Selects the first cell in the matching row
                gridCurrentDatabase.Rows(row.Index).Selected = True
                Exit For
            End If
        Next

    End Sub

    Private Sub gridCurrentDatabase_ColumnDisplayIndexChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles gridCurrentDatabase.ColumnDisplayIndexChanged
    End Sub

#End Region

#Region "Subs and functions"

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
        Dim excludeColumns As New List(Of String) From {"TaskID",
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
            "LongDescription"}

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
        For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
            If Not excludeColumns.Contains(col.Name) Then
                Dim menuItem As New ToolStripMenuItem With
                            {
                            .Text = col.HeaderText.Replace(" ↑", "").Replace(" ↓", ""),
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

    Private Function DownloadOnlineDB() As Boolean

        Dim cleanResponseString As String = String.Empty

        Dim url As String = $"https://siglr.com/DiscordPostHelper/TaskBrowser/TBTaskDatabase.xml"
        Dim client As New WebClient()
        Dim responseBytes As Byte() = Nothing

        Try
            responseBytes = client.DownloadData(url)
        Catch ex As WebException
            Using New Centered_MessageBox()
                MessageBox.Show("It appears it is impossible to download the task database.", "Downloading online task database", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try

        Dim responseString As String = Encoding.UTF8.GetString(responseBytes)

        ' Remove ZWNBSP character using regular expression pattern
        cleanResponseString = Regex.Replace(responseString, "^\uFEFF", String.Empty)

        ' Write the clean response string to the local XML file
        Try
            File.WriteAllText(_localXmlFilePath, cleanResponseString)

        Catch ioEx As IOException
            Using New Centered_MessageBox()
                MessageBox.Show("Failed to write the local task database file: " & ioEx.Message, "File Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return False
        End Try

        ' Update timestamp
        lblLocalDBTimestamp.Text = lblOnlineDBTimestamp.Text
        Settings.SessionSettings.LocalDBTimestamp = lblLocalDBTimestamp.Text
        btnUpdateLocalDB.Enabled = False
        RetrieveOnlineDBTimestamp()

        Return True

    End Function

    Private Sub RetrieveOnlineDBTimestamp()

        Dim apiUrl As String = "https://siglr.com/DiscordPostHelper/TBGetDatabaseTimestamp.php"  ' Update this URL to the actual URL of your PHP script
        Using client As New HttpClient()
            Try
                ' Make the synchronous GET request to the PHP script
                Dim response As HttpResponseMessage = client.GetAsync(apiUrl).Result  ' Use .Result to wait synchronously
                response.EnsureSuccessStatusCode()

                ' Read the response as a string
                lblOnlineDBTimestamp.Text = response.Content.ReadAsStringAsync().Result  ' Use .Result to wait synchronously

            Catch ex As Exception
                Using New Centered_MessageBox()
                    MessageBox.Show($"Error retrieving online database timestamp: {ex.Message}")
                End Using

            End Try
        End Using

        Dim downloadDB As Boolean = False
        If lblOnlineDBTimestamp.Text = "None" Then
            'Cannot download DB
        Else
            If lblLocalDBTimestamp.Text = "None" Then
                'Download DB
                downloadDB = True
            Else
                'Compare timestamp
                If lblLocalDBTimestamp.Text < lblOnlineDBTimestamp.Text Then
                    downloadDB = True
                End If
            End If
        End If
        If downloadDB Then
            btnUpdateLocalDB.Enabled = True
            lblLocalDBTimestamp.ForeColor = Color.Red
            lblOnlineDBTimestamp.ForeColor = Color.Green
        Else
            btnUpdateLocalDB.Enabled = False
            lblLocalDBTimestamp.ForeColor = Color.Green
            lblOnlineDBTimestamp.ForeColor = Color.Green
        End If

    End Sub

    Private Sub UpdateCurrentDBGrid()

        SupportingFeatures.DeserializeTaskDataList(_localXmlFilePath, _currentTaskDBEntries)
        gridCurrentDatabase.DataSource = LoadAndSortXML(_currentSortCol, _currentSortAsc)

        If Not _dataGridViewAllSet Then
            SetupDataGridView()
        End If

    End Sub

    Public Function LoadAndSortXML(sortBy As String, ascending As Boolean) As List(Of TBTaskData)
        Dim doc As XDocument = XDocument.Load(_localXmlFilePath)

        Dim tasks = From task In doc.Descendants("TBTaskData")
                    Select New TBTaskData With {
                .TaskID = task.Element("TaskID")?.Value,
                .Title = task.Element("Title")?.Value,
                .LastUpdate = If(DateTime.TryParse(task.Element("LastUpdate")?.Value, New DateTime()), DateTime.Parse(task.Element("LastUpdate")?.Value), Nothing),
                .SimDateTime = If(DateTime.TryParse(task.Element("SimDateTime")?.Value, New DateTime()), DateTime.Parse(task.Element("SimDateTime")?.Value), Nothing),
                .IncludeYear = If(Boolean.TryParse(task.Element("IncludeYear")?.Value, False), Boolean.Parse(task.Element("IncludeYear")?.Value), False),
                .SimDateTimeExtraInfo = task.Element("SimDateTimeExtraInfo")?.Value,
                .MainAreaPOI = task.Element("MainAreaPOI")?.Value,
                .DepartureName = task.Element("DepartureName")?.Value,
                .DepartureICAO = task.Element("DepartureICAO")?.Value,
                .DepartureExtra = task.Element("DepartureExtra")?.Value,
                .ArrivalName = task.Element("ArrivalName")?.Value,
                .ArrivalICAO = task.Element("ArrivalICAO")?.Value,
                .ArrivalExtra = task.Element("ArrivalExtra")?.Value,
                .SoaringRidge = If(Boolean.TryParse(task.Element("SoaringRidge")?.Value, False), Boolean.Parse(task.Element("SoaringRidge")?.Value), False),
                .SoaringThermals = If(Boolean.TryParse(task.Element("SoaringThermals")?.Value, False), Boolean.Parse(task.Element("SoaringThermals")?.Value), False),
                .SoaringWaves = If(Boolean.TryParse(task.Element("SoaringWaves")?.Value, False), Boolean.Parse(task.Element("SoaringWaves")?.Value), False),
                .SoaringDynamic = If(Boolean.TryParse(task.Element("SoaringDynamic")?.Value, False), Boolean.Parse(task.Element("SoaringDynamic")?.Value), False),
                .SoaringExtraInfo = task.Element("SoaringExtraInfo")?.Value,
                .DurationMin = task.Element("DurationMin")?.Value,
                .DurationMax = task.Element("DurationMax")?.Value,
                .DurationExtraInfo = task.Element("DurationExtraInfo")?.Value,
                .TaskDistance = task.Element("TaskDistance")?.Value,
                .TotalDistance = task.Element("TotalDistance")?.Value,
                .RecommendedGliders = task.Element("RecommendedGliders")?.Value,
                .DifficultyRating = task.Element("DifficultyRating")?.Value,
                .DifficultyExtraInfo = task.Element("DifficultyExtraInfo")?.Value,
                .ShortDescription = task.Element("ShortDescription")?.Value,
                .LongDescription = task.Element("LongDescription")?.Value,
                .WeatherSummary = task.Element("WeatherSummary")?.Value,
                .Credits = task.Element("Credits")?.Value,
                .Countries = task.Element("Countries")?.Value,
                .RecommendedAddOns = If(Boolean.TryParse(task.Element("RecommendedAddOns")?.Value, False), Boolean.Parse(task.Element("RecommendedAddOns")?.Value), False)
            }

        ' Sorting the data based on a specified field and order
        If ascending Then
            tasks = tasks.OrderBy(Function(t) t.GetType().GetProperty(sortBy).GetValue(t, Nothing))
        Else
            tasks = tasks.OrderByDescending(Function(t) t.GetType().GetProperty(sortBy).GetValue(t, Nothing))
        End If

        ' Update the header text with a sort indicator
        If gridCurrentDatabase.Columns.GetColumnCount(False) > 0 Then
            If _currentSortAsc Then
                gridCurrentDatabase.Columns(sortBy).HeaderText = $"{gridCurrentDatabase.Columns(sortBy).HeaderText.Replace(" ↑", "").Replace(" ↓", "")} ↑"
            Else
                gridCurrentDatabase.Columns(sortBy).HeaderText = $"{gridCurrentDatabase.Columns(sortBy).HeaderText.Replace(" ↑", "").Replace(" ↓", "")} ↓"
            End If
        End If

        Return tasks.ToList()

    End Function

    Private Sub SaveDataGridViewSettings()
        Settings.SessionSettings.TBColumnsSettings.Clear()
        For Each col As DataGridViewColumn In gridCurrentDatabase.Columns
            Settings.SessionSettings.TBColumnsSettings.Add(New TBColumnSetting(col.Name, col.DisplayIndex, col.Visible))
        Next
    End Sub

    Private Sub SetupDataGridView()

        gridCurrentDatabase.Font = New Font(gridCurrentDatabase.Font.FontFamily, 12)
        gridCurrentDatabase.RowTemplate.Height = 35
        gridCurrentDatabase.ColumnHeadersDefaultCellStyle.Font = New Font(gridCurrentDatabase.Font, FontStyle.Bold)

        If _currentSortAsc Then
            gridCurrentDatabase.Columns(_currentSortCol).HeaderText &= " ↑"
        Else
            gridCurrentDatabase.Columns(_currentSortCol).HeaderText &= " ↓"
        End If

        If gridCurrentDatabase IsNot Nothing Then
            For Each setting In Settings.SessionSettings.TBColumnsSettings
                If gridCurrentDatabase.Columns.Contains(setting.Name) Then
                    gridCurrentDatabase.Columns(setting.Name).Visible = setting.Visible
                    gridCurrentDatabase.Columns(setting.Name).DisplayIndex = setting.DisplayIndex
                    gridCurrentDatabase.Columns(setting.Name).SortMode = DataGridViewColumnSortMode.Automatic
                    gridCurrentDatabase.Columns(setting.Name).Tag = setting.Name
                End If
            Next
        End If

        gridCurrentDatabase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader
        gridCurrentDatabase.ReadOnly = True
        gridCurrentDatabase.Columns("TaskID").Visible = False
        gridCurrentDatabase.Columns("DPHXFilename").Visible = False
        gridCurrentDatabase.Columns("IsUpdate").Visible = False
        gridCurrentDatabase.Columns("SimDateTime").Visible = False
        gridCurrentDatabase.Columns("IncludeYear").Visible = False
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

        gridCurrentDatabase.Columns("Title").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("LastUpdate").HeaderText = "Updated"
        gridCurrentDatabase.Columns("LastUpdate").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("MainAreaPOI").HeaderText = "Main Area / POI"
        gridCurrentDatabase.Columns("MainAreaPOI").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DepartureName").HeaderText = "From Name"
        gridCurrentDatabase.Columns("DepartureName").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DepartureICAO").HeaderText = "ICAO"
        gridCurrentDatabase.Columns("DepartureICAO").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("ArrivalName").HeaderText = "To Name"
        gridCurrentDatabase.Columns("ArrivalName").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("ArrivalICAO").HeaderText = "ICAO"
        gridCurrentDatabase.Columns("ArrivalICAO").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringRidge").HeaderText = "R"
        gridCurrentDatabase.Columns("SoaringRidge").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringThermals").HeaderText = "T"
        gridCurrentDatabase.Columns("SoaringThermals").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringWaves").HeaderText = "W"
        gridCurrentDatabase.Columns("SoaringWaves").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringDynamic").HeaderText = "D"
        gridCurrentDatabase.Columns("SoaringDynamic").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("SoaringExtraInfo").HeaderText = "Soaring Extra"
        gridCurrentDatabase.Columns("SoaringExtraInfo").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DurationConcat").HeaderText = "Duration"
        gridCurrentDatabase.Columns("DurationConcat").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DurationExtraInfo").HeaderText = "Duration Extra"
        gridCurrentDatabase.Columns("DurationExtraInfo").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DistancesConcat").HeaderText = "Distances"
        gridCurrentDatabase.Columns("DistancesConcat").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("RecommendedGliders").HeaderText = "Gliders"
        gridCurrentDatabase.Columns("RecommendedGliders").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("DifficultyConcat").HeaderText = "Difficulty"
        gridCurrentDatabase.Columns("DifficultyConcat").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("WeatherSummary").HeaderText = "Weather Summary"
        gridCurrentDatabase.Columns("WeatherSummary").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("Credits").HeaderText = "Credits"
        gridCurrentDatabase.Columns("Credits").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("Countries").HeaderText = "Countries"
        gridCurrentDatabase.Columns("Countries").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.Columns("RecommendedAddons").HeaderText = "AddOns"
        gridCurrentDatabase.Columns("RecommendedAddons").AutoSizeMode = DataGridViewAutoSizeColumnsMode.AllCells

        _dataGridViewAllSet = True

    End Sub

    Private Sub BuildTaskData()

        Dim dateFormat As String
        If _selectedTask.IncludeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder

        'Title
        sb.Append($"\b {_selectedTask.Title}\b0\line ")
        If _selectedTask.MainAreaPOI.Trim <> String.Empty Then
            sb.Append($"{_selectedTask.MainAreaPOI}\line ")
            sb.Append("\line ")
        End If
        If _selectedTask.ShortDescription.Trim <> String.Empty Then
            sb.Append($"{_selectedTask.ShortDescription}\line ")
            sb.Append("\line ")
        End If

        'Credits
        sb.Append($"{_selectedTask.Credits}\line ")
        sb.Append("\line ")

        'Local MSFS date and time 
        sb.Append($"MSFS Local date & time is \b {_selectedTask.SimDateTime.ToString(dateFormat, _EnglishCulture)}, {_selectedTask.SimDateTime.ToString("hh:mm tt", _EnglishCulture)} {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.SimDateTimeExtraInfo.Trim, True, True)}\b0\line ")

        'Departure airfield And runway
        sb.Append($"You will depart from \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.DepartureICAO)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.DepartureName, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.DepartureExtra, True, True)}\b0\line ")

        'Arrival airfield And expected runway
        sb.Append($"You will land at \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.ArrivalICAO)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.ArrivalName, True)}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.ArrivalExtra, True, True)}\b0\line ")

        'Type of soaring
        Dim soaringType As String = SupportingFeatures.GetSoaringTypesSelected(_selectedTask.SoaringRidge, _selectedTask.SoaringThermals, _selectedTask.SoaringWaves, _selectedTask.SoaringDynamic)
        If soaringType.Trim <> String.Empty OrElse _selectedTask.SoaringExtraInfo <> String.Empty Then
            sb.Append($"Soaring Type is \b {soaringType}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.SoaringExtraInfo, True, True)}\b0\line ")
        End If

        'Task distance And total distance
        sb.Append($"Distance are \b {_selectedTask.DistancesConcat}\b0\line ")

        'Approx. duration
        sb.Append($"Approx. duration should be \b {_selectedTask.DurationConcat}{SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.DurationExtraInfo, True, True)}\b0\line ")

        'Recommended gliders
        If _selectedTask.RecommendedGliders.Trim <> String.Empty Then
            sb.Append($"Recommended gliders: \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.RecommendedGliders)}\b0\line ")
        End If

        'Difficulty rating
        If _selectedTask.DifficultyRating.Trim <> String.Empty OrElse _selectedTask.DifficultyExtraInfo.Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as \b {SupportingFeatures.GetDifficulty(CInt(_selectedTask.DifficultyRating.Substring(0, 1)), _selectedTask.DifficultyExtraInfo, True)}\b0\line ")
        End If

        sb.Append("\line ")

        If _selectedTask.WeatherSummary <> String.Empty Then
            sb.Append($"Weather summary: \b {SupportingFeatures.ValueToAppendIfNotEmpty(_selectedTask.WeatherSummary)}\b0\line ")
        End If

        'Build full description
        If _selectedTask.LongDescription <> String.Empty Then
            sb.AppendLine("**Full Description**($*$)")
            sb.AppendLine(_selectedTask.LongDescription)
        End If

        sb.Append("}")

        Dim oldZoomFactor As Single = txtBriefing.ZoomFactor
        If oldZoomFactor = 1 Then
            oldZoomFactor = 1.5
        End If
        txtBriefing.ZoomFactor = 1
        SupportingFeatures.FormatMarkdownToRTF(sb.ToString(), txtBriefing)
        txtBriefing.ZoomFactor = oldZoomFactor

    End Sub

#End Region

End Class