Imports System.Data.SQLite
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports SIGLR.SoaringTools.CommonLibrary

Module ParseTasksToCreateWorldMapInfo

    Private _sf As New SupportingFeatures(SupportingFeatures.ClientApp.TaskBrowserAdmin)
    Private _localTasksDatabaseFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SupportingFeatures.TasksDatabase)
    Private _currentTaskDBEntries As DataTable
    Private _localUnpackFolder As String = Path.GetDirectoryName(Application.ExecutablePath)
    Private _localDPHXFileName As String
    Private _plnFilename As String
    Private _wprFilename As String
    Private TempDPHXUnpackFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TempUnpack")
    Private plnXMLDocument As XmlDocument
    Private wprXMLDocument As XmlDocument

    Public Sub Main()

        Console.WriteLine("Reading the tasks from the local database")
        LoadTasksFromDatabase()

        Console.WriteLine($"{_currentTaskDBEntries.Rows.Count} tasks read.")
        Console.WriteLine("Press enter to begin processing.")
        Console.ReadLine()

        Dim totalDistance As Single = 0
        Dim taskDistance As Single = 0
        Dim possibleElevUpdate As Boolean = False
        Dim LatitudeMin As Double
        Dim Latitudemax As Double
        Dim LongitudeMin As Double
        Dim LongitudeMax As Double
        Dim EntrySeqID As Integer
        Dim TaskID As String

        Try
            'Read each taskID
            For Each row As DataRow In _currentTaskDBEntries.Rows
                EntrySeqID = row("EntrySeqID")
                TaskID = row("TaskID")
                Console.WriteLine($"Processing task {EntrySeqID}")
                _localDPHXFileName = Path.Combine(_localUnpackFolder, TaskID & ".dphx")

                'Download the DPHX file
                Console.WriteLine(DownloadTaskFile(TaskID))

                'Unzip the PLN and WPR files
                Console.WriteLine("Unzipping files")
                LoadDPHXPackage(_localDPHXFileName)

                'Build the AllWaypoints list
                Console.WriteLine("Building waypoints")
                _sf.BuildAltitudeRestrictions(plnXMLDocument, totalDistance, taskDistance, possibleElevUpdate)

                'Retrieve the task boundary
                Console.WriteLine("Getting task boundaries")
                _sf.GetTaskBoundaries(LongitudeMin, LongitudeMax, LatitudeMin, Latitudemax)

                'Submit to the PHP script to create / update the WorldMapInfo online for that task
                Console.WriteLine("Calling the PHP script online")
                ' Call the asynchronous method to update or insert the entry
                Console.WriteLine(UpdateWorldMapInfo(EntrySeqID, TaskID, _plnFilename, plnXMLDocument.InnerXml, _wprFilename, wprXMLDocument.InnerXml, LatitudeMin, Latitudemax, LongitudeMin, LongitudeMax))

                'Delete the PLN and WPR files
                Console.WriteLine("Cleaning up")
                SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder)
                'Delete the DPHX file
                File.Delete(_localDPHXFileName)

                'Next
                Console.WriteLine("")
            Next

        Catch ex As Exception
            Console.WriteLine(ex.ToString)
        End Try

        Console.WriteLine("Press enter to finish.")
        Console.ReadLine()

    End Sub

    Private Sub LoadTasksFromDatabase()
        Dim connectionString As String = $"Data Source={_localTasksDatabaseFilePath};Version=3;"
        _currentTaskDBEntries = New DataTable()

        Using conn As New SQLiteConnection(connectionString)
            conn.Open()
            Using cmd As New SQLiteCommand("SELECT Tasks.EntrySeqID, 
                                                   Tasks.TaskID 
                                            FROM Tasks", conn)
                Using adapter As New SQLiteDataAdapter(cmd)
                    adapter.Fill(_currentTaskDBEntries)
                End Using
            End Using
        End Using
    End Sub

    Private Function DownloadTaskFile(taskID As String) As String
        Dim baseUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}TaskBrowser/Tasks/"
        Dim remoteFileName As String = taskID & ".dphx"
        Dim localFileName As String = taskID & ".dphx"
        Dim remoteFileUrl As String = baseUrl & remoteFileName

        Try

            ' Download the file
            Using client As New WebClient()
                client.DownloadFile(remoteFileUrl, _localDPHXFileName)
            End Using

            ' Check if the file exists
            If File.Exists(_localDPHXFileName) Then
                Return _localDPHXFileName
            Else
                Throw New Exception("Failed to download the task file.")
            End If

        Catch ex As Exception
            ' Handle the exception (e.g., log the error)
            Using New Centered_MessageBox()
                MessageBox.Show($"An error occurred while downloading the task file:{Environment.NewLine}{ex.Message}", "Downloading task", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return String.Empty
        End Try
    End Function

    Private Sub LoadDPHXPackage(dphxFilename As String)
        Dim newDPHFile As String
        Dim nbrTries As Integer = 0
        Do Until nbrTries = 10
            nbrTries += 1
            If SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder) Then
                nbrTries = 10
            Else
                Application.DoEvents()
            End If
        Loop

        newDPHFile = _sf.UnpackDPHXFileToTempFolder(dphxFilename, TempDPHXUnpackFolder)

        If File.Exists(newDPHFile) Then
            Dim serializer As New XmlSerializer(GetType(AllData))

            Dim _allDPHData As AllData
            Using stream As New FileStream(newDPHFile, FileMode.Open)
                _allDPHData = CType(serializer.Deserialize(stream), AllData)
            End Using

            If newDPHFile = String.Empty Then
                Console.WriteLine($"The DPHX file {dphxFilename} contains an invalid DPH file.")
                Exit Sub
            End If

            If _allDPHData.DiscordTaskID = String.Empty AndAlso _allDPHData.DiscordTaskThreadURL <> String.Empty AndAlso SupportingFeatures.IsValidURL(_allDPHData.DiscordTaskThreadURL) Then
                _allDPHData.DiscordTaskID = SupportingFeatures.ExtractMessageIDFromDiscordURL(_allDPHData.DiscordTaskThreadURL, True)
            End If

            If _allDPHData.DiscordTaskID = String.Empty Then
                Console.WriteLine($"The DPHX file {dphxFilename} contains an invalid or missing task ID.")
            Else
                Dim newEntry As TBTaskData = CreateDPHFileEntry(_allDPHData, dphxFilename, newDPHFile)
            End If
        End If
    End Sub

    Private Function CreateDPHFileEntry(DPHData As AllData, dphxFilename As String, DPHFilename As String) As TBTaskData
        Dim newEntry As New TBTaskData

        With newEntry
            .TaskID = DPHData.DiscordTaskID

            ' Load flight plan
            _plnFilename = Path.GetFileName(DPHData.FlightPlanFilename)
            plnXMLDocument = New XmlDocument()
            plnXMLDocument.Load(Path.Combine(Path.GetDirectoryName(DPHFilename), _plnFilename))

            ' Load weather file
            _wprFilename = Path.GetFileName(DPHData.WeatherFilename)
            wprXMLDocument = New XmlDocument()
            wprXMLDocument.Load(Path.Combine(Path.GetDirectoryName(DPHFilename), _wprFilename))

        End With

        Return newEntry
    End Function

    Public Function UpdateWorldMapInfo(entrySeqID As Integer, taskID As String, plnFilename As String, plnXML As String, wprFilename As String, wprXML As String, latMin As Double, latMax As Double, longMin As Double, longMax As Double) As String
        Using client As New HttpClient()
            Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"EntrySeqID", entrySeqID.ToString()},
                {"TaskID", taskID},
                {"PLNFilename", plnFilename},
                {"PLNXML", plnXML},
                {"WPRFilename", wprFilename},
                {"WPRXML", wprXML},
                {"LatMin", latMin.ToString()},
                {"LatMax", latMax.ToString()},
                {"LongMin", longMin.ToString()},
                {"LongMax", longMax.ToString()}
            })

            ' Execute the request synchronously
            Dim response As HttpResponseMessage = client.PostAsync($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}UpdateWorldMapInfo.php", content).Result
            response.EnsureSuccessStatusCode()

            Return response.Content.ReadAsStringAsync().Result

        End Using
    End Function

End Module
