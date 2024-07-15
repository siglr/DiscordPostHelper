Imports System.Data.SQLite
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports SIGLR.SoaringTools.CommonLibrary

Module ParseTasksToCreateWorldMapInfo

    Private Const StartingFrom As Integer = 509

    Private _sf As New SupportingFeatures(SupportingFeatures.ClientApp.TaskBrowserAdmin)
    Private _localTasksDatabaseFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SupportingFeatures.TasksDatabase)
    Private _currentTaskDBEntries As DataTable
    Private _localUnpackFolder As String = Path.GetDirectoryName(Application.ExecutablePath)
    Private _localDPHXFileName As String
    Private _plnFilename As String
    Private _wprFilename As String
    Private TempDPHXUnpackFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TempUnpack")
    Private ProcessedWeatherCharts = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ProcessedWeatherCharts")
    Private plnXMLDocument As XmlDocument
    Private wprXMLDocument As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing

    ' Set the image quality
    private qualityParam As New System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L)
    private jpegCodec As System.Drawing.Imaging.ImageCodecInfo = GetEncoderInfo("image/jpeg")
    private encoderParams As New System.Drawing.Imaging.EncoderParameters(1)

    Public Sub Main()

        encoderParams.Param(0) = qualityParam

        Console.WriteLine("Reading the tasks from the local database")
        LoadTasksFromDatabase()

        Console.WriteLine($"{_currentTaskDBEntries.Rows.Count} tasks read.")
        Console.WriteLine("Press enter to begin processing.")
        Console.ReadLine()

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

                'Load the weather file
                _WeatherDetails = Nothing
                _WeatherDetails = New WeatherDetails(wprXMLDocument)

                SaveWeatherGraphToFile(EntrySeqID, row("SimDateTime"), If(row("IncludeYear") = 1, True, False))

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
            Using cmd As New SQLiteCommand($"SELECT Tasks.EntrySeqID, 
                                                   Tasks.TaskID,
                                                   Tasks.SimDateTime,
                                                   Tasks.IncludeYear
                                            FROM Tasks Where EntrySeqID > {StartingFrom} Order By EntrySeqID ASC", conn)
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

    Private Sub SaveWeatherGraphToFile(taskEntrySeqID As Integer, simDateTime As String, blnIncludeYear As Boolean)

        Dim control = New FullWeatherGraphPanel
        Dim imageWidth As Integer = 1333
        Dim imageHeight As Integer = 1000
        Dim tempUnits As New PreferredUnits
        tempUnits.Altitude = PreferredUnits.AltitudeUnits.Both
        tempUnits.WindSpeed = PreferredUnits.WindSpeedUnits.Both
        tempUnits.Temperature = PreferredUnits.TemperatureUnits.Both
        tempUnits.Barometric = PreferredUnits.BarometricUnits.Both

        Dim format As String = "yyyy-MM-dd HH:mm:ss"
        Dim parsedDate As DateTime = DateTime.ParseExact(simDateTime, format, System.Globalization.CultureInfo.InvariantCulture)

        control.SetWeatherInfo(_WeatherDetails, tempUnits, SupportingFeatures.GetEnUSFormattedDate(parsedDate, parsedDate, blnIncludeYear))

        ' Create a bitmap with the specified size
        Dim bmp As New Bitmap(imageWidth, imageHeight)

        ' Scale the drawing to the specified size
        control.Width = imageWidth
        control.Height = imageHeight

        ' Create a graphics object to draw the control's image
        Using g As Graphics = Graphics.FromImage(bmp)
            ' Draw the control onto the graphics object
            control.DrawToBitmap(bmp, New Rectangle(0, 0, imageWidth, imageHeight))
        End Using

        ' Define the file path and name
        Dim filePath As String = $"{ProcessedWeatherCharts}\{taskEntrySeqID}.jpg"

        ' Save the bitmap as a JPG file with the specified quality
        bmp.Save(filePath, jpegCodec, encoderParams)

        bmp.Dispose()
        bmp = Nothing

    End Sub

    Private Function GetEncoderInfo(mimeType As String) As System.Drawing.Imaging.ImageCodecInfo
        Dim codecs() As System.Drawing.Imaging.ImageCodecInfo = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
        For Each codec As System.Drawing.Imaging.ImageCodecInfo In codecs
            If codec.MimeType = mimeType Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

End Module
