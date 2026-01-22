Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports SIGLR.SoaringTools.CommonLibrary

Public Class CleaningTool


    Private Sub tabCtrlCleaningTool_Selected(sender As Object, e As TabControlEventArgs) Handles tabCtrlCleaningTool.Selected

        TabSelected(e.TabPage)

    End Sub

    Private Sub TabSelected(tabPageSelected As TabPage)

        Select Case tabPageSelected.Name
            Case tabFlights2020.Name
                lblFlights2020FolderPath.Text = If(Settings.SessionSettings.Is2020Installed, Settings.SessionSettings.MSFS2020FlightPlansFolder, "Not installed")

            Case tabFlights2024.Name
                lblFlights2024FolderPath.Text = If(Settings.SessionSettings.Is2024Installed, Settings.SessionSettings.MSFS2024FlightPlansFolder, "Not installed")

            Case tabWeather2020.Name
                lblWeather2020FolderPath.Text = If(Settings.SessionSettings.Is2020Installed, WeatherCommunityPackageHelper.GetDphxWeatherPresetsDir(True), "Not installed")
                If Settings.SessionSettings.Is2020Installed Then
                    tabWeather2020.Enabled = EnsureWeatherPackageReady(True)
                    If Not tabWeather2020.Enabled Then
                        lblWeather2020FolderPath.Text = "DPHX Community package missing - open Settings to repair"
                    End If
                End If

            Case tabWeather2024.Name
                lblWeather2024FolderPath.Text = If(Settings.SessionSettings.Is2024Installed, WeatherCommunityPackageHelper.GetDphxWeatherPresetsDir(False), "Not installed")
                If Settings.SessionSettings.Is2024Installed Then
                    tabWeather2024.Enabled = EnsureWeatherPackageReady(False)
                    If Not tabWeather2024.Enabled Then
                        lblWeather2024FolderPath.Text = "DPHX Community package missing - open Settings to repair"
                    End If
                End If

            Case tabPackages.Name
                lblPackagesFolderPath.Text = Settings.SessionSettings.PackagesFolder

            Case tabNB21Logs.Name
                lblNB21LogsFolderPath.Text = Settings.SessionSettings.NB21IGCFolder
                If lblNB21LogsFolderPath.Text.Trim = String.Empty Then
                    lblNB21LogsFolderPath.Text = "No path selected"
                    tabNB21Logs.Enabled = False
                Else
                    tabNB21Logs.Enabled = True
                End If

            Case tabXCSoarTasks.Name
                lblXCSoarTasksFolderPath.Text = Settings.SessionSettings.XCSoarTasksFolder
                If lblXCSoarTasksFolderPath.Text.Trim = String.Empty Then
                    lblXCSoarTasksFolderPath.Text = "No path selected"
                    tabXCSoarTasks.Enabled = False
                Else
                    tabXCSoarTasks.Enabled = True
                End If

            Case tabXCSoarMaps.Name
                lblXCSoarMapsFolderPath.Text = Settings.SessionSettings.XCSoarMapsFolder
                If lblXCSoarMapsFolderPath.Text.Trim = String.Empty Then
                    lblXCSoarMapsFolderPath.Text = "No path selected"
                    tabXCSoarMaps.Enabled = False
                Else
                    tabXCSoarMaps.Enabled = True
                End If

        End Select

        If tabPageSelected.Enabled Then
            LoadListBox(tabPageSelected)
        End If

    End Sub

    Private Sub LoadListBox(tabPageSelected As TabPage)

        Select Case tabPageSelected.Name
            Case tabFlights2020.Name
                If tabFlights2020.Enabled Then
                    LoadFlightPlans2020()
                End If

            Case tabFlights2024.Name
                If tabFlights2024.Enabled Then
                    LoadFlightPlans2024()
                End If

            Case tabWeather2020.Name
                If tabWeather2020.Enabled Then
                    LoadWeatherProfiles2020()
                End If

            Case tabWeather2024.Name
                If tabWeather2024.Enabled Then
                    LoadWeatherProfiles2024()
                End If

            Case tabPackages.Name
                LoadPackages()

            Case tabNB21Logs.Name
                LoadNB21Logs()

            Case tabXCSoarTasks.Name
                LoadXCSoarTasks()

            Case tabXCSoarMaps.Name
                LoadXCSoarMaps()

        End Select

    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnFlights2020SelectAll.Click,
                                                                             btnWeather2020SelectAll.Click,
                                                                             btnPackagesSelectAll.Click,
                                                                             btnNB21LogsSelectAll.Click,
                                                                             btnXCSoarTasksSelectAll.Click,
                                                                             btnXCSoarMapsSelectAll.Click,
                                                                             btnFlights2024SelectAll.Click,
                                                                             btnWeather2024SelectAll.Click

        Dim theListBox As ListBox = Nothing

        Select Case sender.name
            Case btnFlights2020SelectAll.Name
                theListBox = lstFlights2020
            Case btnWeather2020SelectAll.Name
                theListBox = lstWeather2020
            Case btnFlights2024SelectAll.Name
                theListBox = lstFlights2024
            Case btnWeather2024SelectAll.Name
                theListBox = lstWeather2024
            Case btnPackagesSelectAll.Name
                theListBox = lstPackages
            Case btnNB21LogsSelectAll.Name
                theListBox = lstNB21Logs
            Case btnXCSoarTasksSelectAll.Name
                theListBox = lstXCSoarTasks
            Case btnXCSoarMapsSelectAll.Name
                theListBox = lstXCSoarMaps

        End Select

        If theListBox IsNot Nothing Then
            ' Check if the ListBox is not already fully selected.
            If theListBox.SelectedItems.Count < theListBox.Items.Count Then
                ' Select all items.
                For i As Integer = 0 To theListBox.Items.Count - 1
                    theListBox.SetSelected(i, True)
                Next
            Else
                ' Deselect all items if they are all selected.
                For i As Integer = 0 To theListBox.Items.Count - 1
                    theListBox.SetSelected(i, False)
                Next
            End If
        End If

    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnFlights2020Refresh.Click,
                                                                           btnWeather2020Refresh.Click,
                                                                           btnPackagesRefresh.Click,
                                                                           btnNB21LogsRefresh.Click,
                                                                           btnXCSoarTasksRefresh.Click,
                                                                           btnXCSoarMapsRefresh.Click,
                                                                           btnFlights2024Refresh.Click,
                                                                           btnWeather2024Refresh.Click

        Select Case sender.name
            Case btnFlights2020Refresh.Name
                TabSelected(tabFlights2020)
            Case btnWeather2020Refresh.Name
                TabSelected(tabWeather2020)
            Case btnFlights2024Refresh.Name
                TabSelected(tabFlights2024)
            Case btnWeather2024Refresh.Name
                TabSelected(tabWeather2024)
            Case btnPackagesRefresh.Name
                TabSelected(tabPackages)
            Case btnNB21LogsRefresh.Name
                TabSelected(tabNB21Logs)
            Case btnXCSoarTasksRefresh.Name
                TabSelected(tabXCSoarTasks)
            Case btnXCSoarMapsRefresh.Name
                TabSelected(tabXCSoarMaps)
        End Select

    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnFlights2020Delete.Click,
                                                                          btnWeather2020Delete.Click,
                                                                          btnPackagesDelete.Click,
                                                                          btnNB21LogsDelete.Click,
                                                                          btnXCSoarTasksDelete.Click,
                                                                          btnXCSoarMapsDelete.Click,
                                                                          btnWeather2024Delete.Click, btnFlights2024Delete.Click

        Dim theList As ListBox = Nothing

        'Check if files are selected
        Select Case sender.name
            Case btnFlights2020Delete.Name
                theList = lstFlights2020
            Case btnWeather2020Delete.Name
                theList = lstWeather2020
            Case btnFlights2024Delete.Name
                theList = lstFlights2024
            Case btnWeather2024Delete.Name
                theList = lstWeather2024
            Case btnPackagesDelete.Name
                theList = lstPackages
            Case btnNB21LogsDelete.Name
                theList = lstNB21Logs
            Case btnXCSoarTasksDelete.Name
                theList = lstXCSoarTasks
            Case btnXCSoarMapsDelete.Name
                theList = lstXCSoarMaps
        End Select
        If theList Is Nothing OrElse theList.SelectedItems.Count = 0 Then
            Return
        End If

        ' Step 1: Ask for confirmation
        Using New Centered_MessageBox()
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete the selected files?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.No Then
                ' If the user selects 'No', exit the sub
                Return
            End If
        End Using

        Select Case sender.name
            Case btnFlights2020Delete.Name
                DeleteSelectedFiles(lstFlights2020, lblFlights2020FolderPath.Text)
                TabSelected(tabFlights2020)
            Case btnWeather2020Delete.Name
                DeleteSelectedFiles(lstWeather2020, lblWeather2020FolderPath.Text)
                TabSelected(tabWeather2020)
            Case btnFlights2024Delete.Name
                DeleteSelectedFiles(lstFlights2024, lblFlights2024FolderPath.Text)
                TabSelected(tabFlights2024)
            Case btnWeather2024Delete.Name
                DeleteSelectedFiles(lstWeather2024, lblWeather2024FolderPath.Text)
                TabSelected(tabWeather2024)
            Case btnPackagesDelete.Name
                DeleteSelectedFiles(lstPackages, lblPackagesFolderPath.Text)
                TabSelected(tabPackages)
            Case btnNB21LogsDelete.Name
                DeleteSelectedFiles(lstNB21Logs, lblNB21LogsFolderPath.Text)
                TabSelected(tabNB21Logs)
            Case btnXCSoarTasksDelete.Name
                DeleteSelectedFiles(lstXCSoarTasks, lblXCSoarTasksFolderPath.Text)
                TabSelected(tabXCSoarTasks)
            Case btnXCSoarMapsDelete.Name
                DeleteSelectedFiles(lstXCSoarMaps, lblXCSoarMapsFolderPath.Text)
                TabSelected(tabXCSoarMaps)

        End Select

    End Sub

    Private Sub DeleteSelectedFiles(theListBox As ListBox, folderPath As String)
        Dim layoutPath As String = GetWeatherLayoutPathForFolder(folderPath)

        ' Step 2: Build a list of selected filenames
        Dim filesToDelete As New List(Of String)()
        For Each item As String In theListBox.SelectedItems
            ' Assuming the format is "filename : title", split and get the filename part
            Dim parts As String() = item.Split(New String() {" : "}, StringSplitOptions.None)
            If parts.Length > 0 Then
                If parts.Length = 1 OrElse parts(1).Trim() <> "currently in use, cannot be deleted!" Then
                    filesToDelete.Add(parts(0).Trim()) ' Add only the filename to the list
                End If
            End If
        Next

        ' Step 3: Delete each file
        For Each filename In filesToDelete
            Dim fullPath As String = Path.Combine(folderPath, filename)
            Try
                If File.Exists(fullPath) Then
                    File.Delete(fullPath)
                End If
            Catch ex As Exception
                Using New Centered_MessageBox()
                    MessageBox.Show($"Error deleting file '{filename}': {ex.Message}")
                End Using
            End Try
        Next

        If Not String.IsNullOrWhiteSpace(layoutPath) Then
            SyncWeatherLayoutWithDisk(folderPath, layoutPath)
        End If
    End Sub


#Region "Flights"

    Private Sub LoadFlightPlans2020()
        Dim folderPath As String = lblFlights2020FolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .pln files from the specified folder.
            Dim plnFiles As String() = Directory.GetFiles(folderPath, "*.pln")

            ' Clear existing items in the ListBox.
            lstFlights2020.Items.Clear()

            ' Process each .pln file.
            For Each filePath In plnFiles
                ' Load the XML content of the .pln file.
                Dim doc As XDocument = XDocument.Load(filePath)

                ' Extract the <Title> element value.
                Dim title As String = doc.Descendants("Title").First().Value

                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)

                ' Add the filename and title to the ListBox.
                lstFlights2020.Items.Add($"{filename} : ""{title}""")
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

    Private Sub LoadFlightPlans2024()
        Dim folderPath As String = lblFlights2024FolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .pln files from the specified folder.
            Dim plnFiles As String() = Directory.GetFiles(folderPath, "*.pln")

            ' Clear existing items in the ListBox.
            lstFlights2024.Items.Clear()

            ' Process each .pln file.
            For Each filePath In plnFiles
                ' Load the XML content of the .pln file.
                Dim doc As XDocument = XDocument.Load(filePath)

                ' Extract the <Title> element value.
                Dim title As String = doc.Descendants("Title").First().Value

                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)

                ' Add the filename and title to the ListBox.
                lstFlights2024.Items.Add($"{filename} : ""{title}""")
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

#End Region

#Region "Weather"
    Private Sub LoadWeatherProfiles2020()
        Dim folderPath As String = lblWeather2020FolderPath.Text

        Try
            If Not Directory.Exists(folderPath) Then Return
        Catch : Return
        End Try

        Try
            SyncWeatherLayoutWithDisk(folderPath, WeatherCommunityPackageHelper.GetDphxWeatherLayoutPath(True))
            Dim wprFiles As String() = Directory.GetFiles(folderPath, "*.wpr")
            lstWeather2020.Items.Clear()

            For Each filePath In wprFiles
                Dim filename As String = System.IO.Path.GetFileName(filePath)

                ' Load the display name from the weather preset
                Dim doc As XDocument = XDocument.Load(filePath)
                Dim nameElem = doc.Descendants("Name").FirstOrDefault()
                Dim name As String = If(nameElem IsNot Nothing, nameElem.Value, "")
                lstWeather2020.Items.Add($"{filename} : ""{name}""")
            Next
        Catch ex As Exception
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

    Private Sub LoadWeatherProfiles2024()
        Dim folderPath As String = lblWeather2024FolderPath.Text

        Try
            If Not Directory.Exists(folderPath) Then Return
        Catch : Return
        End Try

        Try
            SyncWeatherLayoutWithDisk(folderPath, WeatherCommunityPackageHelper.GetDphxWeatherLayoutPath(False))
            Dim wprFiles As String() = Directory.GetFiles(folderPath, "*.wpr")
            lstWeather2024.Items.Clear()

            For Each filePath In wprFiles
                Dim filename As String = System.IO.Path.GetFileName(filePath)

                Dim doc As XDocument = XDocument.Load(filePath)
                Dim nameElem = doc.Descendants("Name").FirstOrDefault()
                Dim name As String = If(nameElem IsNot Nothing, nameElem.Value, "")
                lstWeather2024.Items.Add($"{filename} : ""{name}""")
            Next
        Catch ex As Exception
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

#End Region

#Region "Packages"
    Private Sub LoadPackages()
        Dim folderPath As String = lblPackagesFolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .dphx files from the specified folder.
            Dim dphxFiles As String() = Directory.GetFiles(folderPath, "*.dphx")

            ' Clear existing items in the ListBox.
            lstPackages.Items.Clear()

            ' Process each .dphx file.
            For Each filePath In dphxFiles
                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)

                ' Add the filename and name to the ListBox.
                If DPHXUnpackAndLoad.packageNameToolStrip.Text = filePath Then
                    lstPackages.Items.Add($"{filename} : currently in use, cannot be deleted!")
                Else
                    lstPackages.Items.Add($"{filename}")
                End If
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

#End Region

#Region "NB21 Logs"
    Private Sub LoadNB21Logs()
        Dim folderPath As String = lblNB21LogsFolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .igc files from the specified folder.
            Dim nb21LogFiles As String() = Directory.GetFiles(folderPath, "*.igc")

            ' Clear existing items in the ListBox.
            lstNB21Logs.Items.Clear()

            ' Process each .igc file.
            For Each filePath In nb21LogFiles
                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)
                ' Add the filename and name to the ListBox.
                lstNB21Logs.Items.Add($"{filename}")
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

#End Region

#Region "XCSoar Tasks"
    Private Sub LoadXCSoarTasks()
        Dim folderPath As String = lblXCSoarTasksFolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .tsk files from the specified folder.
            Dim XCSoarTasksFiles As String() = Directory.GetFiles(folderPath, "*.tsk")

            ' Clear existing items in the ListBox.
            lstXCSoarTasks.Items.Clear()

            ' Process each .igc file.
            For Each filePath In XCSoarTasksFiles
                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)
                ' Add the filename and name to the ListBox.
                lstXCSoarTasks.Items.Add($"{filename}")
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub


#End Region

#Region "XCSoar Maps"
    Private Sub LoadXCSoarMaps()
        Dim folderPath As String = lblXCSoarMapsFolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .xcm files from the specified folder.
            Dim XCSoarMapsFiles As String() = Directory.GetFiles(folderPath, "*.xcm")

            ' Clear existing items in the ListBox.
            lstXCSoarMaps.Items.Clear()

            ' Process each .xcm file.
            For Each filePath In XCSoarMapsFiles
                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)
                ' Add the filename and name to the ListBox.
                lstXCSoarMaps.Items.Add($"{filename}")
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred: {ex.Message}")
            End Using
        End Try
    End Sub

    Private Sub CleaningTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Rescale()
        SupportingFeatures.CenterFormOnOwner(Owner, Me)

        lstFlights2020.Width = lstWeather2020.Width
        lstFlights2020.Height = lstWeather2020.Height
        btnFlights2020Refresh.Left = btnWeather2020Refresh.Left
        btnFlights2020SelectAll.Left = btnWeather2020SelectAll.Left
        btnFlights2020Delete.Left = btnWeather2020Delete.Left

        If Settings.SessionSettings.Is2024Installed Then
            tabFlights2024.Enabled = True
            tabWeather2024.Enabled = True
            tabCtrlCleaningTool.SelectTab(tabFlights2024)
            TabSelected(tabFlights2024)
        Else
            tabFlights2024.Enabled = False
            tabWeather2024.Enabled = False
        End If

        If Settings.SessionSettings.Is2020Installed Then
            tabFlights2020.Enabled = True
            tabWeather2020.Enabled = True
            tabCtrlCleaningTool.SelectTab(tabFlights2020)
            TabSelected(tabFlights2020)
        Else
            tabFlights2020.Enabled = False
            tabWeather2020.Enabled = False
        End If
    End Sub

    Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click
        Close()
    End Sub

#End Region

    Private Function EnsureWeatherPackageReady(isMsfs2020 As Boolean) As Boolean
        Dim simLabel = If(isMsfs2020, "MSFS 2020", "MSFS 2024")
        Dim communityFolder = If(isMsfs2020, Settings.SessionSettings.MSFS2020WeatherPresetsFolder, Settings.SessionSettings.MSFS2024WeatherPresetsFolder)

        Dim ensureResult = WeatherCommunityPackageHelper.EnsureWeatherCommunityPackage(simLabel, communityFolder, isMsfs2020, Me)
        If ensureResult = WeatherCommunityPackageHelper.PackageEnsureResult.Ready Then
            Return True
        End If

        Dim message As String
        Select Case ensureResult
            Case WeatherCommunityPackageHelper.PackageEnsureResult.NotConfigured
                message = $"DPHX Community package not configured for {simLabel}. Please update the Community folder in Settings."
            Case WeatherCommunityPackageHelper.PackageEnsureResult.NeedsFolderChange
                message = $"DPHX Community package requires a different folder for {simLabel}. Please update the Community folder in Settings."
            Case WeatherCommunityPackageHelper.PackageEnsureResult.Cancelled
                message = $"DPHX Community package setup was cancelled for {simLabel}."
            Case WeatherCommunityPackageHelper.PackageEnsureResult.Failed
                message = $"DPHX Community package is missing or incomplete for {simLabel}. Please open Settings to repair it."
            Case Else
                message = $"DPHX Community package is not ready for {simLabel}."
        End Select

        Using New Centered_MessageBox(Me)
            MessageBox.Show(message, "DPHX Community Package", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Using

        Return False
    End Function

    Private Function GetWeatherLayoutPathForFolder(folderPath As String) As String
        If String.IsNullOrWhiteSpace(folderPath) Then
            Return String.Empty
        End If

        Dim normalized = NormalizeFolderPath(folderPath)
        Dim presets2020 = NormalizeFolderPath(WeatherCommunityPackageHelper.GetDphxWeatherPresetsDir(True))
        Dim presets2024 = NormalizeFolderPath(WeatherCommunityPackageHelper.GetDphxWeatherPresetsDir(False))

        If Not String.IsNullOrWhiteSpace(presets2020) AndAlso String.Equals(normalized, presets2020, StringComparison.OrdinalIgnoreCase) Then
            Return WeatherCommunityPackageHelper.GetDphxWeatherLayoutPath(True)
        End If

        If Not String.IsNullOrWhiteSpace(presets2024) AndAlso String.Equals(normalized, presets2024, StringComparison.OrdinalIgnoreCase) Then
            Return WeatherCommunityPackageHelper.GetDphxWeatherLayoutPath(False)
        End If

        Return String.Empty
    End Function

    Private Sub SyncWeatherLayoutWithDisk(presetsDir As String, layoutPath As String)
        If String.IsNullOrWhiteSpace(presetsDir) OrElse String.IsNullOrWhiteSpace(layoutPath) Then
            Return
        End If

        Dim isMsfs2020 As Boolean
        Dim presets2020 = NormalizeFolderPath(WeatherCommunityPackageHelper.GetDphxWeatherPresetsDir(True))
        Dim presets2024 = NormalizeFolderPath(WeatherCommunityPackageHelper.GetDphxWeatherPresetsDir(False))
        Dim normalized = NormalizeFolderPath(presetsDir)

        If String.IsNullOrWhiteSpace(normalized) Then
            Return
        End If

        If String.Equals(normalized, presets2020, StringComparison.OrdinalIgnoreCase) Then
            isMsfs2020 = True
        ElseIf String.Equals(normalized, presets2024, StringComparison.OrdinalIgnoreCase) Then
            isMsfs2020 = False
        Else
            Return
        End If

        Dim thumbnailPath = WeatherCommunityPackageHelper.GetDphxWeatherThumbnailPath(isMsfs2020)

        Try
            Dim layout = WeatherCommunityPackageHelper.LoadLayout(layoutPath, thumbnailPath)
            Dim changed = WeatherCommunityPackageHelper.SyncLayoutWithDisk(layout, presetsDir)
            If changed Then
                WeatherCommunityPackageHelper.SaveLayout(layoutPath, layout)
            End If
        Catch ex As Exception
            Using New Centered_MessageBox()
                MessageBox.Show($"Unable to sync layout.json with weather presets: {ex.Message}")
            End Using
        End Try
    End Sub

    Private Function NormalizeFolderPath(folderPath As String) As String
        If String.IsNullOrWhiteSpace(folderPath) Then
            Return String.Empty
        End If

        Return Path.GetFullPath(folderPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    End Function

End Class
