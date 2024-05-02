Imports System.IO
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

Public Class AdminScreen

    Private _SF As New SupportingFeatures(SupportingFeatures.ClientApp.TaskBrowserAdmin)
    Private TempDPHXUnpackFolder As String
    Private ProcessedDPHXFilesFolder As String
    Private TBTaskDatabaseXMLFile As String
    Private _currentTaskDBEntries As New Dictionary(Of String, TBTaskData)
    Private _incomingTaskDBEntries As Dictionary(Of String, TBTaskData)

    Private Sub AdminScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Set temporary unpack folder
        TempDPHXUnpackFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TempUnpack")
        ProcessedDPHXFilesFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ProcessedDPHXFiles")
        TBTaskDatabaseXMLFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TBTaskDatabase.xml")

        'Read the current database file
        DeserializeTaskDataList()
        UpdateCurrentDBGrid()

    End Sub

    Private Sub UpdateCurrentDBGrid()

        'Map the dictionnary as datasource to the current database grid
        gridCurrentDatabase.DataSource = _currentTaskDBEntries.Values.ToList
        gridCurrentDatabase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader
        gridCurrentDatabase.ReadOnly = True
        gridCurrentDatabase.Columns(1).Visible = False
        gridCurrentDatabase.Columns(2).Visible = False

    End Sub

    Private Sub UpdateIncomingDBGrid()

        'Map the dictionnary as datasource to the incoming grid
        gridIncomingDPHXFilesData.DataSource = _incomingTaskDBEntries.Values.ToList
        gridIncomingDPHXFilesData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader
        gridIncomingDPHXFilesData.ReadOnly = True

    End Sub

    Private Sub btnSelectDPHXFiles_Click(sender As Object, e As EventArgs) Handles btnSelectDPHXFiles.Click

        OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"

        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select DPHX files"
        OpenFileDialog1.Filter = "DPHX|*.dphx"
        OpenFileDialog1.Multiselect = True

        Dim result As DialogResult = OpenFileDialog1.ShowDialog(Me)

        If result = DialogResult.OK Then
            _incomingTaskDBEntries = New Dictionary(Of String, TBTaskData)

            For Each selectedfile As String In OpenFileDialog1.FileNames
                LoadDPHXPackage(selectedfile)
            Next
        End If

        UpdateIncomingDBGrid()

    End Sub

    Private Sub LoadDPHXPackage(dphxFilename As String)

        Dim newDPHFile As String

        Dim nbrTries As Integer = 0
        Do Until nbrTries = 10
            nbrTries += 1
            If SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder) Then
                nbrTries = 10
            Else
                Me.Refresh()
                Application.DoEvents()
            End If
        Loop

        newDPHFile = _SF.UnpackDPHXFileToTempFolder(dphxFilename, TempDPHXUnpackFolder)

        If File.Exists(newDPHFile) Then
            Dim serializer As New XmlSerializer(GetType(AllData))

            On Error Resume Next

            Dim _allDPHData As AllData

            Using stream As New FileStream(newDPHFile, FileMode.Open)
                _allDPHData = CType(serializer.Deserialize(stream), AllData)
            End Using

            If newDPHFile = String.Empty OrElse _allDPHData.DiscordTaskID = String.Empty Then
                'Invalid file loaded
                MessageBox.Show(Me, $"The DPHX file {dphxFilename} contains an invalid DPH file or the task ID is missing.", "Loading DPHX file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                'Process the DPH file
                Dim newEntry As TBTaskData = CreateDPHFileEntry(_allDPHData, dphxFilename, newDPHFile)
                If newEntry IsNot Nothing Then
                    _incomingTaskDBEntries.Add(_allDPHData.DiscordTaskID, newEntry)
                Else
                    MessageBox.Show(Me, $"The DPHX file {dphxFilename} already exists and is the same in the current database.", "Loading DPHX file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If

        End If

    End Sub

    Private Function CreateDPHFileEntry(DPHData As AllData, dphxFilename As String, DPHFilename As String) As TBTaskData

        Dim newEntry As New TBTaskData

        With newEntry
            .TaskID = DPHData.DiscordTaskID
            .Title = DPHData.Title
            .LastUpdate = GetFileUpdateDateTime(DPHFilename)
            .SimDateTime = SupportingFeatures.GetFullEventDateTimeInLocal(DPHData.SimDate, DPHData.SimTime, False)
            .IncludeYear = DPHData.IncludeYear
            .SimDateTimeExtraInfo = DPHData.SimDateTimeExtraInfo
            .MainAreaPOI = DPHData.MainAreaPOI
            .DepartureICAO = DPHData.DepartureICAO
            .DepartureName = DPHData.DepartureName
            .DepartureExtra = DPHData.DepartureExtra
            .ArrivalICAO = DPHData.ArrivalICAO
            .ArrivalName = DPHData.ArrivalName
            .ArrivalExtra = DPHData.ArrivalExtra
            .SoaringRidge = DPHData.SoaringRidge
            .SoaringThermals = DPHData.SoaringThermals
            .SoaringWaves = DPHData.SoaringWaves
            .SoaringDynamic = DPHData.SoaringDynamic
            .SoaringExtraInfo = DPHData.SoaringExtraInfo
            .DurationMin = DPHData.DurationMin
            .DurationMax = DPHData.DurationMax
            .DurationExtraInfo = DPHData.DurationExtraInfo
            .RecommendedGliders = DPHData.RecommendedGliders
            .DifficultyRating = DPHData.DifficultyRating
            .DifficultyExtraInfo = DPHData.DifficultyExtraInfo
            .ShortDescription = DPHData.ShortDescription
            .LongDescription = DPHData.LongDescription
            .WeatherSummary = DPHData.WeatherSummary
            .Credits = DPHData.Credits
            .Countries = String.Join(", ", DPHData.Countries)
            .DPHXFilename = dphxFilename
        End With

        'Lookup the task ID in the current database to check if it's an update or an addition
        If _currentTaskDBEntries.ContainsKey(newEntry.TaskID) Then
            newEntry.IsUpdate = True
            'Check the update date to see if something has changed
            If _currentTaskDBEntries(newEntry.TaskID).LastUpdate = newEntry.LastUpdate Then
                newEntry = Nothing
            End If
        End If

        Return newEntry

    End Function

    Private Function GetFileUpdateDateTime(filePath As String) As DateTime

        Dim fileInfo As New FileInfo(filePath)

        ' Get the last write time
        Return fileInfo.LastWriteTime

    End Function

    Private Sub SerializeTaskDataList()

        Dim tasks As List(Of TBTaskData) = _currentTaskDBEntries.Values.ToList

        ' Create an XmlSerializer for the List(Of TBTaskData)
        Dim serializer As New XmlSerializer(GetType(List(Of TBTaskData)), New XmlRootAttribute("TBTaskDatabase"))

        ' Use a StreamWriter to write the XML to file
        Using writer As New StreamWriter(TBTaskDatabaseXMLFile)
            serializer.Serialize(writer, tasks)
        End Using

    End Sub

    Public Sub DeserializeTaskDataList()

        Dim theList As List(Of TBTaskData)

        ' Create an XmlSerializer for the List(Of TBTaskData)
        Dim serializer As New XmlSerializer(GetType(List(Of TBTaskData)), New XmlRootAttribute("TBTaskDatabase"))

        ' Use a StreamReader to read the XML from file
        Using reader As New StreamReader(TBTaskDatabaseXMLFile)
            ' Deserialize the XML to a list of TBTaskData
            theList = CType(serializer.Deserialize(reader), List(Of TBTaskData))
        End Using

        For Each task In theList
            _currentTaskDBEntries.Add(task.TaskID, task)
        Next

    End Sub

    Private Sub btnSaveDatabase_Click(sender As Object, e As EventArgs) Handles btnSaveDatabase.Click

        If _incomingTaskDBEntries IsNot Nothing Then
            For Each task In _incomingTaskDBEntries.Values
                If task.IsUpdate Then
                    'replace the task
                    _currentTaskDBEntries(task.TaskID) = task
                Else
                    _currentTaskDBEntries.Add(task.TaskID, task)
                End If
                'Copy the DPHXFile and rename it using the task ID
                File.Copy(task.DPHXFilename, Path.Combine(ProcessedDPHXFilesFolder, $"{task.TaskID}.dphx"), True)

            Next
            SerializeTaskDataList()
            UpdateCurrentDBGrid()
            _incomingTaskDBEntries.Clear()
            UpdateIncomingDBGrid()
        End If

    End Sub
End Class
