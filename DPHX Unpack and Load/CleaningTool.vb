Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports SIGLR.SoaringTools.CommonLibrary

Public Class CleaningTool

    Private Sub CleaningTool_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        tabCtrlCleaningTool.SelectTab(tabFlights)

    End Sub

    Private Sub tabCtrlCleaningTool_Selected(sender As Object, e As TabControlEventArgs) Handles tabCtrlCleaningTool.Selected

        TabSelected(e.TabPage)

    End Sub

    Private Sub TabSelected(tabPageSelected As TabPage)

        Select Case tabPageSelected.Name
            Case tabFlights.Name
                lblFlightsFolderPath.Text = Settings.SessionSettings.FlightPlansFolder

            Case tabWeather.Name
                lblWeatherFolderPath.Text = Settings.SessionSettings.MSFSWeatherPresetsFolder

            Case tabPackages.Name
                lblPackagesFolderPath.Text = Settings.SessionSettings.PackagesFolder

            Case tabNB21Logs.Name
                lblNB21LogsFolderPath.Text = Settings.SessionSettings.NB21IGCFolder

            Case tabXCSoarTasks.Name
                lblXCSoarTasksFolderPath.Text = Settings.SessionSettings.XCSoarTasksFolder

            Case tabXCSoarMaps.Name
                lblXCSoarMapsFolderPath.Text = Settings.SessionSettings.XCSoarMapsFolder

        End Select

        LoadListBox(tabPageSelected)

    End Sub

    Private Sub LoadListBox(tabPageSelected As TabPage)

        Select Case tabPageSelected.Name
            Case tabFlights.Name
                LoadFlightPlans()

            Case tabWeather.Name
                LoadWeatherProfiles()

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

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnFlightsSelectAll.Click,
                                                                             btnWeatherSelectAll.Click,
                                                                             btnPackagesSelectAll.Click,
                                                                             btnNB21LogsSelectAll.Click,
                                                                             btnXCSoarTasksSelectAll.Click,
                                                                             btnXCSoarMapsSelectAll.Click

        Dim theListBox As ListBox = Nothing

        Select Case sender.name
            Case btnFlightsSelectAll.Name
                theListBox = lstFlights
            Case btnWeatherSelectAll.Name
                theListBox = lstWeather
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

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnFlightsRefresh.Click,
                                                                           btnWeatherRefresh.Click,
                                                                           btnPackagesRefresh.Click,
                                                                           btnNB21LogsRefresh.Click,
                                                                           btnXCSoarTasksRefresh.Click,
                                                                           btnXCSoarMapsRefresh.Click

        Select Case sender.name
            Case btnFlightsRefresh.Name
                TabSelected(tabFlights)
            Case btnWeatherRefresh.Name
                TabSelected(tabFlights)
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

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnFlightsDelete.Click,
                                                                          btnWeatherDelete.Click,
                                                                          btnPackagesDelete.Click,
                                                                          btnNB21LogsDelete.Click,
                                                                          btnXCSoarTasksDelete.Click,
                                                                          btnXCSoarMapsDelete.Click

        Dim theList As ListBox = Nothing

        'Check if files are selected
        Select Case sender.name
            Case btnFlightsDelete.Name
                theList = lstFlights
            Case btnWeatherDelete.Name
                theList = lstWeather
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
            Case btnFlightsDelete.Name
                DeleteSelectedFiles(lstFlights, lblFlightsFolderPath.Text)
                TabSelected(tabFlights)
            Case btnWeatherDelete.Name
                DeleteSelectedFiles(lstWeather, lblWeatherFolderPath.Text)
                TabSelected(tabWeather)
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
    End Sub


#Region "Flights"

    Private Sub LoadFlightPlans()
        Dim folderPath As String = lblFlightsFolderPath.Text

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
            lstFlights.Items.Clear()

            ' Process each .pln file.
            For Each filePath In plnFiles
                ' Load the XML content of the .pln file.
                Dim doc As XDocument = XDocument.Load(filePath)

                ' Extract the <Title> element value.
                Dim title As String = doc.Descendants("Title").First().Value

                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)

                ' Add the filename and title to the ListBox.
                lstFlights.Items.Add($"{filename} : ""{title}""")
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
    Private Sub LoadWeatherProfiles()
        Dim folderPath As String = lblWeatherFolderPath.Text

        'Check if path is legal
        Try
            If Not Directory.Exists(folderPath) Then
                Return
            End If

        Catch ex As Exception
            Return
        End Try

        Try
            ' Get all .wpr files from the specified folder.
            Dim wprFiles As String() = Directory.GetFiles(folderPath, "*.wpr")

            ' Clear existing items in the ListBox.
            lstWeather.Items.Clear()

            ' Process each .wpr file.
            For Each filePath In wprFiles
                ' Load the XML content of the .wpr file.
                Dim doc As XDocument = XDocument.Load(filePath)

                ' Extract the <Name> element value.
                Dim name As String = doc.Descendants("Name").First().Value

                ' Get the filename without the path.
                Dim filename As String = Path.GetFileName(filePath)

                ' Add the filename and name to the ListBox.
                lstWeather.Items.Add($"{filename} : ""{name}""")
            Next
        Catch ex As Exception
            ' Handle any errors that might occur.
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


#End Region

End Class