﻿Imports System.Data.SQLite
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class AdminScreen

    Private _SF As New SupportingFeatures(SupportingFeatures.ClientApp.TaskBrowserAdmin)
    Private TempDPHXUnpackFolder As String
    Private ProcessedDPHXFilesFolder As String
    Private SQLiteDBFile As String
    Private _currentTaskDBEntries As New Dictionary(Of String, TBTaskData)
    Private _incomingTaskDBEntries As Dictionary(Of String, TBTaskData)
    Private _XmlDocFlightPlan As XmlDocument

    Private Sub AdminScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Set temporary unpack folder
        TempDPHXUnpackFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "TempUnpack")
        ProcessedDPHXFilesFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ProcessedDPHXFiles")
        SQLiteDBFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SupportingFeatures.TasksDatabase)

        ' Read the current database file
        LoadCurrentTasksFromDatabase()
        UpdateCurrentDBGrid()
    End Sub

    Private Sub LoadCurrentTasksFromDatabase()
        Dim connectionString As String = $"Data Source={SQLiteDBFile};Version=3;"
        Using conn As New SQLiteConnection(connectionString)
            conn.Open()
            Using cmd As New SQLiteCommand("SELECT * FROM Tasks order by DBEntryUpdate DESC, EntrySeqID DESC", conn)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim task As New TBTaskData With {
                        .EntrySeqID = Convert.ToInt32(reader("EntrySeqID")),
                        .TaskID = reader("TaskID").ToString(),
                        .DBEntryUpdate = reader("DBEntryUpdate").ToString(),
                        .Title = reader("Title").ToString(),
                        .LastUpdate = DateTime.Parse(reader("LastUpdate").ToString()),
                        .SimDateTime = DateTime.Parse(reader("SimDateTime").ToString()),
                        .IncludeYear = If(Convert.ToBoolean(reader("IncludeYear")), 1, 0),
                        .SimDateTimeExtraInfo = reader("SimDateTimeExtraInfo").ToString(),
                        .MainAreaPOI = reader("MainAreaPOI").ToString(),
                        .DepartureName = reader("DepartureName").ToString(),
                        .DepartureICAO = reader("DepartureICAO").ToString(),
                        .DepartureExtra = reader("DepartureExtra").ToString(),
                        .ArrivalName = reader("ArrivalName").ToString(),
                        .ArrivalICAO = reader("ArrivalICAO").ToString(),
                        .ArrivalExtra = reader("ArrivalExtra").ToString(),
                        .SoaringRidge = If(Convert.ToBoolean(reader("SoaringRidge")), 1, 0),
                        .SoaringThermals = If(Convert.ToBoolean(reader("SoaringThermals")), 1, 0),
                        .SoaringWaves = If(Convert.ToBoolean(reader("SoaringWaves")), 1, 0),
                        .SoaringDynamic = If(Convert.ToBoolean(reader("SoaringDynamic")), 1, 0),
                        .SoaringExtraInfo = reader("SoaringExtraInfo").ToString(),
                        .DurationMin = Convert.ToInt32(reader("DurationMin")),
                        .DurationMax = Convert.ToInt32(reader("DurationMax")),
                        .DurationExtraInfo = reader("DurationExtraInfo").ToString(),
                        .TaskDistance = Convert.ToInt32(reader("TaskDistance")),
                        .TotalDistance = Convert.ToInt32(reader("TotalDistance")),
                        .RecommendedGliders = reader("RecommendedGliders").ToString(),
                        .DifficultyRating = reader("DifficultyRating").ToString(),
                        .DifficultyExtraInfo = reader("DifficultyExtraInfo").ToString(),
                        .WeatherSummary = reader("WeatherSummary").ToString(),
                        .Credits = reader("Credits").ToString(),
                        .Countries = reader("Countries").ToString(),
                        .RecommendedAddOns = If(Convert.ToBoolean(reader("RecommendedAddOns")), 1, 0),
                        .TotDownloads = Convert.ToInt32(reader("TotDownloads")),
                        .LastDownloadUpdate = reader("LastDownloadUpdate").ToString(),
                        .ShortDescription = reader("ShortDescription").ToString(),
                        .LongDescription = reader("LongDescription").ToString()
                    }

                        ' Load the MapImage
                        If Not IsDBNull(reader("MapImage")) Then
                            task.MapImage = DirectCast(reader("MapImage"), Byte())
                        End If

                        ' Load the CoverImage
                        If Not IsDBNull(reader("CoverImage")) Then
                            task.CoverImage = DirectCast(reader("CoverImage"), Byte())
                        End If

                        _currentTaskDBEntries.Add(task.TaskID, task)
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub UpdateCurrentDBGrid()
        gridCurrentDatabase.DataSource = _currentTaskDBEntries.Values.ToList()
        gridCurrentDatabase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridCurrentDatabase.ReadOnly = True
        gridCurrentDatabase.Columns(1).Visible = False
        gridCurrentDatabase.Columns(2).Visible = False
    End Sub

    Private Sub UpdateIncomingDBGrid()
        gridIncomingDPHXFilesData.DataSource = _incomingTaskDBEntries.Values.ToList()
        gridIncomingDPHXFilesData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        gridIncomingDPHXFilesData.ReadOnly = True
    End Sub

    Private Sub btnSelectDPHXFiles_Click(sender As Object, e As EventArgs) Handles btnSelectDPHXFiles.Click
        OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\Fly Now\TestBrowserAdmin"
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
                If _incomingTaskDBEntries.ContainsKey(_allDPHData.DiscordTaskID) Then
                    Console.WriteLine($"Error: Duplicate DiscordTaskID {_allDPHData.DiscordTaskID} found in file {dphxFilename}.")
                Else
                    Dim newEntry As TBTaskData = CreateDPHFileEntry(_allDPHData, dphxFilename, newDPHFile)
                    If newEntry IsNot Nothing Then
                        _incomingTaskDBEntries.Add(_allDPHData.DiscordTaskID, newEntry)
                    Else
                        MessageBox.Show(Me, $"The DPHX file {dphxFilename} already exists and is the same in the current database.", "Loading DPHX file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
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
            .IncludeYear = If(DPHData.IncludeYear, 1, 0)
            .SimDateTimeExtraInfo = DPHData.SimDateTimeExtraInfo
            .MainAreaPOI = DPHData.MainAreaPOI
            .DepartureICAO = DPHData.DepartureICAO
            .DepartureName = DPHData.DepartureName
            .DepartureExtra = DPHData.DepartureExtra
            .ArrivalICAO = DPHData.ArrivalICAO
            .ArrivalName = DPHData.ArrivalName
            .ArrivalExtra = DPHData.ArrivalExtra
            .SoaringRidge = If(DPHData.SoaringRidge, 1, 0)
            .SoaringThermals = If(DPHData.SoaringThermals, 1, 0)
            .SoaringWaves = If(DPHData.SoaringWaves, 1, 0)
            .SoaringDynamic = If(DPHData.SoaringDynamic, 1, 0)
            .SoaringExtraInfo = DPHData.SoaringExtraInfo
            .DurationMin = If(DPHData.DurationMin <> String.Empty, CInt(DPHData.DurationMin), 0)
            .DurationMax = If(DPHData.DurationMax <> String.Empty, CInt(DPHData.DurationMax), 0)
            .DurationExtraInfo = DPHData.DurationExtraInfo
            .RecommendedGliders = DPHData.RecommendedGliders
            .DifficultyRating = DPHData.DifficultyRating
            .DifficultyExtraInfo = DPHData.DifficultyExtraInfo
            .ShortDescription = DPHData.ShortDescription
            .LongDescription = DPHData.LongDescription
            .WeatherSummary = DPHData.WeatherSummary
            .Credits = DPHData.Credits
            .Countries = String.Join(", ", DPHData.Countries)
            If DPHData.RecommendedAddOns Is Nothing OrElse DPHData.RecommendedAddOns.Count = 0 Then
                .RecommendedAddOns = 0
            Else
                .RecommendedAddOns = 1
            End If
            .DPHXFilename = dphxFilename

            ' Load flight plan
            Dim totalDistance As Integer
            Dim taskDistance As Integer
            Dim possibleElevationUpdateRequired As Boolean = False

            _XmlDocFlightPlan = New XmlDocument()
            _XmlDocFlightPlan.Load(Path.Combine(Path.GetDirectoryName(DPHFilename), Path.GetFileName(DPHData.FlightPlanFilename)))

            _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, taskDistance, possibleElevationUpdateRequired, False)
            .TaskDistance = taskDistance
            .TotalDistance = totalDistance

            ' Cover and map image
            If DPHData.MapImageSelected <> String.Empty Then
                .MapImage = ResizeImageAndGetBytes(Path.Combine(Path.GetDirectoryName(DPHFilename), Path.GetFileName(DPHData.MapImageSelected)), 400, 400, 25)
            End If

            If DPHData.CoverImageSelected <> String.Empty Then
                .CoverImage = ResizeImageAndGetBytes(Path.Combine(Path.GetDirectoryName(DPHFilename), Path.GetFileName(DPHData.CoverImageSelected)), 400, 400, 25)
            End If
            .TotDownloads = 0
            .LastDownloadUpdate = "2000-01-01 23:59:00"
            .DBEntryUpdate = Now.ToString
        End With

        If _currentTaskDBEntries.ContainsKey(newEntry.TaskID) Then
            newEntry.IsUpdate = True
            newEntry.EntrySeqID = _currentTaskDBEntries(newEntry.TaskID).EntrySeqID
            If _currentTaskDBEntries(newEntry.TaskID).LastUpdate = newEntry.LastUpdate Then
                newEntry = Nothing
            End If
        End If

        Return newEntry
    End Function

    Private Function GetFileUpdateDateTime(filePath As String) As DateTime
        Dim fileInfo As New FileInfo(filePath)
        Return fileInfo.LastWriteTime
    End Function

    Private Sub btnSaveDatabase_Click(sender As Object, e As EventArgs) Handles btnSaveDatabase.Click
        If _incomingTaskDBEntries IsNot Nothing Then
            For Each task In _incomingTaskDBEntries.Values
                If task.IsUpdate Then
                    _currentTaskDBEntries(task.TaskID) = task
                Else
                    _currentTaskDBEntries.Add(task.TaskID, task)
                End If
                File.Copy(task.DPHXFilename, Path.Combine(ProcessedDPHXFilesFolder, $"{task.TaskID}.dphx"), True)
            Next

            MessageBox.Show(Me, "Upload new/updated DPHX files first.", "Waiting for new and updated DPHX files upload", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Call the new function to send the _incomingTaskDBEntries.Values containing new/updated tasks
            Dim uploadResult As Boolean = UploadTasksToServer(_incomingTaskDBEntries.Values.ToList())

            If uploadResult Then
                'To review:
                'SaveTasksToDatabase()
                'UpdateCurrentDBGrid()
                _incomingTaskDBEntries.Clear()
                UpdateIncomingDBGrid()
            Else
                MessageBox.Show(Me, "Failed to upload tasks to server.", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        End If
    End Sub

    Private Sub SaveTasksToDatabase()
        Dim connectionString As String = $"Data Source={SQLiteDBFile};Version=3;"
        Using conn As New SQLiteConnection(connectionString)
            conn.Open()
            For Each task As TBTaskData In _currentTaskDBEntries.Values
                Using cmd As New SQLiteCommand(conn)
                    cmd.CommandText = "INSERT OR REPLACE INTO Tasks (TaskID, Title, LastUpdate, SimDateTime, IncludeYear, SimDateTimeExtraInfo, MainAreaPOI, DepartureName, DepartureICAO, DepartureExtra, ArrivalName, ArrivalICAO, ArrivalExtra, SoaringRidge, SoaringThermals, SoaringWaves, SoaringDynamic, SoaringExtraInfo, DurationMin, DurationMax, DurationExtraInfo, TaskDistance, TotalDistance, RecommendedGliders, DifficultyRating, DifficultyExtraInfo, ShortDescription, LongDescription, WeatherSummary, Credits, Countries, RecommendedAddOns, MapImage, CoverImage, TotDownloads, LastDownloadUpdate) VALUES (@TaskID, @Title, @LastUpdate, @SimDateTime, @IncludeYear, @SimDateTimeExtraInfo, @MainAreaPOI, @DepartureName, @DepartureICAO, @DepartureExtra, @ArrivalName, @ArrivalICAO, @ArrivalExtra, @SoaringRidge, @SoaringThermals, @SoaringWaves, @SoaringDynamic, @SoaringExtraInfo, @DurationMin, @DurationMax, @DurationExtraInfo, @TaskDistance, @TotalDistance, @RecommendedGliders, @DifficultyRating, @DifficultyExtraInfo, @ShortDescription, @LongDescription, @WeatherSummary, @Credits, @Countries, @RecommendedAddOns, @MapImage, @CoverImage, @TotDownloads, @LastDownloadUpdate)"

                    cmd.Parameters.AddWithValue("@TaskID", task.TaskID)
                    cmd.Parameters.AddWithValue("@Title", task.Title)
                    cmd.Parameters.AddWithValue("@LastUpdate", task.LastUpdate)
                    cmd.Parameters.AddWithValue("@SimDateTime", task.SimDateTime)
                    cmd.Parameters.AddWithValue("@IncludeYear", task.IncludeYear)
                    cmd.Parameters.AddWithValue("@SimDateTimeExtraInfo", task.SimDateTimeExtraInfo)
                    cmd.Parameters.AddWithValue("@MainAreaPOI", task.MainAreaPOI)
                    cmd.Parameters.AddWithValue("@DepartureName", task.DepartureName)
                    cmd.Parameters.AddWithValue("@DepartureICAO", task.DepartureICAO)
                    cmd.Parameters.AddWithValue("@DepartureExtra", task.DepartureExtra)
                    cmd.Parameters.AddWithValue("@ArrivalName", task.ArrivalName)
                    cmd.Parameters.AddWithValue("@ArrivalICAO", task.ArrivalICAO)
                    cmd.Parameters.AddWithValue("@ArrivalExtra", task.ArrivalExtra)
                    cmd.Parameters.AddWithValue("@SoaringRidge", task.SoaringRidge)
                    cmd.Parameters.AddWithValue("@SoaringThermals", task.SoaringThermals)
                    cmd.Parameters.AddWithValue("@SoaringWaves", task.SoaringWaves)
                    cmd.Parameters.AddWithValue("@SoaringDynamic", task.SoaringDynamic)
                    cmd.Parameters.AddWithValue("@SoaringExtraInfo", task.SoaringExtraInfo)
                    cmd.Parameters.AddWithValue("@DurationMin", task.DurationMin)
                    cmd.Parameters.AddWithValue("@DurationMax", task.DurationMax)
                    cmd.Parameters.AddWithValue("@DurationExtraInfo", task.DurationExtraInfo)
                    cmd.Parameters.AddWithValue("@TaskDistance", task.TaskDistance)
                    cmd.Parameters.AddWithValue("@TotalDistance", task.TotalDistance)
                    cmd.Parameters.AddWithValue("@RecommendedGliders", task.RecommendedGliders)
                    cmd.Parameters.AddWithValue("@DifficultyRating", task.DifficultyRating)
                    cmd.Parameters.AddWithValue("@DifficultyExtraInfo", task.DifficultyExtraInfo)
                    cmd.Parameters.AddWithValue("@ShortDescription", task.ShortDescription)
                    cmd.Parameters.AddWithValue("@LongDescription", task.LongDescription)
                    cmd.Parameters.AddWithValue("@WeatherSummary", task.WeatherSummary)
                    cmd.Parameters.AddWithValue("@Credits", task.Credits)
                    cmd.Parameters.AddWithValue("@Countries", task.Countries)
                    cmd.Parameters.AddWithValue("@RecommendedAddOns", task.RecommendedAddOns)
                    cmd.Parameters.AddWithValue("@TotDownloads", task.TotDownloads)
                    cmd.Parameters.AddWithValue("@LastDownloadUpdate", task.LastDownloadUpdate)
                    cmd.Parameters.AddWithValue("@DBEntryUpdate", task.DBEntryUpdate)

                    ' Handle BLOB fields
                    If task.MapImage IsNot Nothing Then
                        cmd.Parameters.Add("@MapImage", DbType.Binary).Value = task.MapImage
                    Else
                        cmd.Parameters.Add("@MapImage", DbType.Binary).Value = DBNull.Value
                    End If

                    If task.CoverImage IsNot Nothing Then
                        cmd.Parameters.Add("@CoverImage", DbType.Binary).Value = task.CoverImage
                    Else
                        cmd.Parameters.Add("@CoverImage", DbType.Binary).Value = DBNull.Value
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
    End Sub

    Private Function ResizeImageAndGetBytes(inputPath As String, maxWidth As Integer, maxHeight As Integer, quality As Long) As Byte()
        ' Load the original image
        Using image As Image = Image.FromFile(inputPath)
            ' Calculate the new size maintaining aspect ratio
            Dim ratioX As Double = maxWidth / image.Width
            Dim ratioY As Double = maxHeight / image.Height
            Dim ratio As Double = Math.Min(ratioX, ratioY)

            Dim newWidth As Integer = CInt(image.Width * ratio)
            Dim newHeight As Integer = CInt(image.Height * ratio)

            ' Create a new Bitmap with the proper dimensions
            Using thumbnail As New Bitmap(newWidth, newHeight)
                Using graphics As Graphics = Graphics.FromImage(thumbnail)
                    ' High quality settings for better output
                    graphics.CompositingQuality = CompositingQuality.HighQuality
                    graphics.SmoothingMode = SmoothingMode.HighQuality
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic

                    ' Draw the original image onto the thumbnail
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight)
                End Using

                ' Image quality settings
                Using ms As New MemoryStream()
                    Dim jpgEncoder As ImageCodecInfo = GetEncoder(ImageFormat.Jpeg)
                    Dim myEncoder As System.Drawing.Imaging.Encoder = System.Drawing.Imaging.Encoder.Quality
                    Dim myEncoderParameters As New EncoderParameters(1)
                    Dim myEncoderParameter As New EncoderParameter(myEncoder, quality)
                    myEncoderParameters.Param(0) = myEncoderParameter

                    ' Save the image to a memory stream in JPEG format
                    thumbnail.Save(ms, jpgEncoder, myEncoderParameters)

                    ' Convert the image to a byte array
                    Return ms.ToArray()
                End Using
            End Using
        End Using
    End Function

    Private Function GetEncoder(format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next
        Return Nothing
    End Function

    Private Function UploadTasksToServer(tasks As List(Of TBTaskData)) As Boolean
        Try
            ' Serialize the tasks to JSON
            Dim json As String = JsonConvert.SerializeObject(tasks, New JsonSerializerSettings() With {
                .NullValueHandling = NullValueHandling.Ignore
            })

            ' Prepare the request
            Dim request As HttpWebRequest = CType(WebRequest.Create($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}UpdateTasks.php"), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"

            ' Write the JSON to the request stream
            Using writer As New StreamWriter(request.GetRequestStream())
                writer.Write(json)
            End Using

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim result As String = reader.ReadToEnd()
                    ' Assuming the server returns a JSON object with a "status" field
                    Dim responseJson As JObject = JObject.Parse(result)
                    Return responseJson("status").ToString() = "success"
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show(Me, $"Error uploading tasks: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

End Class
