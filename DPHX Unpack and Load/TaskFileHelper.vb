Imports System.Diagnostics
Imports System.IO
Imports System.Net.Http
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports System.Xml.Linq
Imports Microsoft.Win32
Imports Newtonsoft.Json
Imports SIGLR.SoaringTools.CommonLibrary

Friend Module TaskFileHelper

    ' Reusable HttpClient for Tracker JSON polling
    Private ReadOnly TrackerHttpClient As New HttpClient()
    Private Const WeatherPresetPrefix As String = "0_"

    Friend Function CopyTaskFile(filename As String,
                                 sourcePath As String,
                                 destPath As String,
                                 msgToAsk As String,
                                 owner As IWin32Window,
                                 msfsWarningVisible As Boolean) As String
        Return CopyTaskFileInternal(filename, sourcePath, destPath, msgToAsk, owner, msfsWarningVisible)
    End Function

    Friend Function CopyWeatherPresetToMsfs(filename As String,
                                            sourcePath As String,
                                            destPath As String,
                                            msgToAsk As String,
                                            owner As IWin32Window,
                                            msfsWarningVisible As Boolean) As String
        Dim sourceName = Path.GetFileName(filename)
        Dim installedName = GetInstalledWeatherPresetFilename(sourceName)
        Dim label = $"{msgToAsk} (source: {sourceName}, installed: {installedName})"
        Dim copyMessage = CopyTaskFileInternal(sourceName, sourcePath, destPath, label, owner, msfsWarningVisible, installedName)
        Dim xmlMessage = UpdateWeatherPresetDisplayName(Path.Combine(destPath, installedName))

        If Not String.IsNullOrWhiteSpace(xmlMessage) Then
            copyMessage &= $"{Environment.NewLine}{xmlMessage}"
        End If

        Return copyMessage
    End Function

    Friend Function GetInstalledWeatherPresetFilename(originalFilename As String) As String
        If String.IsNullOrWhiteSpace(originalFilename) Then
            Return originalFilename
        End If

        Dim nameOnly = Path.GetFileName(originalFilename)
        If nameOnly.StartsWith(WeatherPresetPrefix, StringComparison.OrdinalIgnoreCase) Then
            Return nameOnly
        End If

        Return WeatherPresetPrefix & nameOnly
    End Function

    Private Function CopyTaskFileInternal(filename As String,
                                          sourcePath As String,
                                          destPath As String,
                                          msgToAsk As String,
                                          owner As IWin32Window,
                                          msfsWarningVisible As Boolean,
                                          Optional destFilename As String = Nothing) As String
        Dim fullSourceFilename As String
        Dim fullDestFilename As String
        Dim proceed As Boolean = False
        Dim messageToReturn As String = String.Empty
        Dim destName = If(String.IsNullOrWhiteSpace(destFilename), filename, destFilename)

        If Not Directory.Exists(destPath) Then
            Return $"{msgToAsk} ""{destName}"" skipped - destination folder not set or invalid (check settings)"
        End If

        fullSourceFilename = Path.Combine(sourcePath, filename)
        fullDestFilename = Path.Combine(destPath, destName)

        Dim sourceToCopy As String = fullSourceFilename
        If File.Exists(fullDestFilename) Then
            Dim srcInfo = New FileInfo(fullSourceFilename)
            Dim dstInfo = New FileInfo(fullDestFilename)

            Dim identical As Boolean = (srcInfo.Exists AndAlso dstInfo.Exists AndAlso srcInfo.Length = dstInfo.Length) _
                                   AndAlso SupportingFeatures.FilesAreEquivalent(fullSourceFilename, fullDestFilename)

            If identical Then
                proceed = False
                messageToReturn = $"{msgToAsk} ""{destName}"" skipped (identical - up-to-date)"
            Else
                Select Case Settings.SessionSettings.AutoOverwriteFiles
                    Case AllSettings.AutoOverwriteOptions.AlwaysOverwrite
                        proceed = True
                        messageToReturn = $"{msgToAsk} ""{destName}"" copied over (different existing file, policy: Overwrite)"
                    Case AllSettings.AutoOverwriteOptions.AlwaysSkip
                        proceed = False
                        messageToReturn = $"{msgToAsk} ""{destName}"" skipped (different file exists, policy: Skip)"
                    Case AllSettings.AutoOverwriteOptions.AlwaysAsk
                        Dim ownerWindow As IWin32Window = owner
                        Using New Centered_MessageBox(ownerWindow)
                            If MessageBox.Show(ownerWindow,
                                               $"A different {msgToAsk} file already exists.{Environment.NewLine}{Environment.NewLine}{destName}{Environment.NewLine}{Environment.NewLine}Overwrite it?",
                                               "File already exists",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question) = DialogResult.Yes Then
                                proceed = True
                                messageToReturn = $"{msgToAsk} ""{destName}"" copied over (different existing file, policy: Ask)"
                            Else
                                proceed = False
                                messageToReturn = $"{msgToAsk} ""{destName}"" skipped by user (different file exists, policy: Ask)"
                            End If
                        End Using
                End Select
            End If
        Else
            proceed = True
            messageToReturn = $"{msgToAsk} ""{destName}"" copied"
        End If

        If proceed Then
            Dim destWasReadOnly As Boolean = False
            Dim originalAttrs As FileAttributes = CType(0, FileAttributes)

            Try
                If File.Exists(fullDestFilename) Then
                    originalAttrs = File.GetAttributes(fullDestFilename)
                    If (originalAttrs And FileAttributes.ReadOnly) <> 0 Then
                        destWasReadOnly = True
                        File.SetAttributes(fullDestFilename, originalAttrs And Not FileAttributes.ReadOnly)
                    End If
                End If

                File.Copy(sourceToCopy, fullDestFilename, True)

                If destWasReadOnly AndAlso File.Exists(fullDestFilename) Then
                    Dim newAttrs = File.GetAttributes(fullDestFilename)
                    File.SetAttributes(fullDestFilename, newAttrs Or FileAttributes.ReadOnly)
                End If

                AppendMsfsRunningNoteForWpr(messageToReturn, destName, isDelete:=False, msfsWarningVisible:=msfsWarningVisible)

            Catch ex As UnauthorizedAccessException
                messageToReturn = $"{msgToAsk} ""{destName}"" failed to copy (access denied): {ex.Message}"
                If destWasReadOnly AndAlso File.Exists(fullDestFilename) Then
                    Try : File.SetAttributes(fullDestFilename, originalAttrs) : Catch : End Try
                End If

            Catch ex As Exception
                messageToReturn = $"{msgToAsk} ""{destName}"" failed to copy: {ex.Message}"
                If destWasReadOnly AndAlso File.Exists(fullDestFilename) Then
                    Try : File.SetAttributes(fullDestFilename, originalAttrs) : Catch : End Try
                End If
            End Try
        End If

        Return messageToReturn
    End Function

    Private Function UpdateWeatherPresetDisplayName(filePath As String) As String
        If String.IsNullOrWhiteSpace(filePath) Then
            Return "Preset name update skipped - installed file path missing."
        End If

        If Not File.Exists(filePath) Then
            Return "Preset name update skipped - installed file not found."
        End If

        Try
            Dim doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace)
            Dim nameElement = doc.Descendants("Name").FirstOrDefault()
            Dim currentName = If(nameElement IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(nameElement.Value),
                                 nameElement.Value.Trim(),
                                 Path.GetFileNameWithoutExtension(filePath))

            If String.IsNullOrWhiteSpace(currentName) Then
                Return "Preset name update skipped - preset name missing."
            End If

            Dim newName = currentName
            Dim updated = False
            If Not currentName.StartsWith(WeatherPresetPrefix, StringComparison.OrdinalIgnoreCase) Then
                newName = WeatherPresetPrefix & currentName
                updated = True
                If nameElement Is Nothing Then
                    If doc.Root IsNot Nothing Then
                        doc.Root.AddFirst(New XElement("Name", newName))
                    End If
                Else
                    nameElement.Value = newName
                End If

                Dim settings As New Xml.XmlWriterSettings With {
                    .Indent = True,
                    .Encoding = New UTF8Encoding(False)
                }
                Using writer = Xml.XmlWriter.Create(filePath, settings)
                    doc.Save(writer)
                End Using
            End If

            If updated Then
                Return $"Preset name updated to ""{newName}""."
            End If

            Return $"Preset name already prefixed: ""{newName}""."
        Catch ex As Exception
            Return $"Preset name update failed: {ex.Message}"
        End Try
    End Function

    Friend Function DeleteTaskFile(filename As String,
                                   sourcePath As String,
                                   msgToAsk As String,
                                   excludeFromCleanup As Boolean,
                                   msfsWarningVisible As Boolean) As String
        Dim fullSourceFilename As String
        Dim messageToReturn As String = String.Empty

        If Directory.Exists(sourcePath) Then
            fullSourceFilename = Path.Combine(sourcePath, filename)
            If File.Exists(fullSourceFilename) Then
                Try
                    If Not excludeFromCleanup Then
                        File.Delete(fullSourceFilename)
                        messageToReturn = $"{msgToAsk} ""{filename}"" deleted"
                        AppendMsfsRunningNoteForWpr(messageToReturn, filename, isDelete:=True, msfsWarningVisible:=msfsWarningVisible)
                    Else
                        messageToReturn = $"{msgToAsk} ""{filename}"" excluded from cleanup"
                    End If
                Catch ex As Exception
                    messageToReturn = $"{msgToAsk} ""{filename}"" found but error trying to delete it:{Environment.NewLine}{ex.Message}"
                End Try
            Else
                messageToReturn = $"{msgToAsk} ""{filename}"" not found"
            End If
        Else
            messageToReturn = $"{msgToAsk} ""{filename}"" skipped - destination folder not set or invalid (check settings)"
        End If

        Return messageToReturn
    End Function

    Friend Sub AppendMsfsRunningNoteForWpr(ByRef msg As String,
                                           fileName As String,
                                           isDelete As Boolean,
                                           msfsWarningVisible As Boolean)
        If Not msfsWarningVisible Then Exit Sub
        If Not fileName.EndsWith(".wpr", StringComparison.OrdinalIgnoreCase) Then Exit Sub

        If isDelete Then
            msg &= $"{Environment.NewLine}⚠️MSFS is running: the preset may remain visible/usable until MSFS is restarted."
        Else
            msg &= $"{Environment.NewLine}⚠️MSFS is running: the new/updated preset will not appear until MSFS is restarted."
        End If
    End Sub

    Friend Function CreatePlnForEfb(filename As String,
                                     sourcePath As String,
                                     destPath As String,
                                     msgToAsk As String,
                                     owner As IWin32Window,
                                     msfsWarningVisible As Boolean) As String
        Try
            Dim fullSourceFilename = Path.Combine(sourcePath, filename)
            If Not File.Exists(fullSourceFilename) Then
                Return $"❌ Source file not found: {fullSourceFilename}"
            End If

            Dim xdoc = XDocument.Load(fullSourceFilename, LoadOptions.PreserveWhitespace)

            Dim changed As Integer = 0
            For Each wp In xdoc.Descendants("ATCWaypoint")
                Dim idAttr = wp.Attribute("id")
                If idAttr Is Nothing Then Continue For
                Dim original = idAttr.Value
                Dim cleaned = CleanWaypointId(original)
                If cleaned <> original Then
                    idAttr.Value = cleaned
                    changed += 1
                End If
            Next

            Dim efbName = Path.GetFileNameWithoutExtension(fullSourceFilename) & "_EFB" & Path.GetExtension(fullSourceFilename)
            Dim efbPath = Path.Combine(sourcePath, efbName)

            Dim settings As New Xml.XmlWriterSettings With {
                .Indent = True,
                .Encoding = New UTF8Encoding(False)
            }
            Using w = Xml.XmlWriter.Create(efbPath, settings)
                xdoc.Save(w)
            End Using

            If Not String.IsNullOrWhiteSpace(destPath) Then
                Return CopyTaskFile(efbName, sourcePath, destPath, msgToAsk, owner, msfsWarningVisible)
            End If

            Return "No destination folder set for EFB version of task!"

        Catch ex As Exception
            Return $"❌ CreatePLNForEFB failed: {ex.Message}"
        End Try
    End Function

    Private Function CleanWaypointId(value As String) As String
        If String.IsNullOrEmpty(value) Then Return value
        Dim s = value.Trim()
        s = s.TrimStart("*"c)
        Dim plusIdx = s.IndexOf("+"c)
        If plusIdx >= 0 Then s = s.Substring(0, plusIdx)
        Return s.Trim()
    End Function

    Friend Sub SendPlnFileToNB21Logger(plnfilePath As String, status As frmStatus)
        Dim apiUrl As String = $"http://localhost:{Settings.SessionSettings.NB21LocalWSPort}/pln_set"

        Try
            Dim plnContent As String = File.ReadAllText(plnfilePath)

            Using client As New HttpClient()
                Dim content As New StringContent(plnContent, Encoding.UTF8, "application/xml")
                Dim response As HttpResponseMessage = client.PostAsync(apiUrl, content).Result

                If response.IsSuccessStatusCode Then
                    status?.AppendStatusLine("PLN file successfully sent to external NB21 Logger.", True)
                Else
                    status?.AppendStatusLine($"Failed to send PLN file to external NB21 Logger. HTTP Status: {response.StatusCode}", True)
                End If
            End Using
        Catch ex As Exception
            status?.AppendStatusLine($"An error occurred while sending the PLN file: {ex.Message}", True)
        End Try
    End Sub

    ' =========================
    '  SSC-Tracker integration
    ' =========================

    Friend Sub SendDataToTracker(trackerGroup As String,
                                 plnfilePath As String,
                                 wprfilePath As String,
                                 infoURL As String,
                                 expectedPlnTitle As String,
                                 status As frmStatus)

        Dim port = Settings.SessionSettings.TrackerLocalWSPort
        Dim setTaskUrl As String = $"http://localhost:{port}/settask"
        Dim jsonUrl As String = $"http://localhost:{port}/json"

        Try
            Dim plnContent As String = If(File.Exists(plnfilePath), File.ReadAllText(plnfilePath), "")
            Dim wprContent As String = If(File.Exists(wprfilePath), File.ReadAllText(wprfilePath), "")

            Dim extractFilename As Func(Of String, String) =
                Function(filePath)
                    If String.IsNullOrEmpty(filePath) Then Return ""
                    Dim fullFilename = Path.GetFileName(filePath)
                    Return Path.GetFileNameWithoutExtension(fullFilename)
                End Function

            Dim payload As New With {
                .CMD = "SET",
                .GN = trackerGroup,
                .TASK = extractFilename(plnfilePath),  ' keep existing behavior
                .TASKDATA = plnContent,
                .WEATHER = extractFilename(wprfilePath),
                .WEATHERDATA = wprContent,
                .TASKINFO = infoURL
            }

            Dim jsonPayload As String = JsonConvert.SerializeObject(payload)

            Const maxAttempts As Integer = 3
            Const delayMs As Integer = 3000

            For attempt As Integer = 1 To maxAttempts
                status?.AppendStatusLine($"Sending task to Tracker (attempt {attempt}/{maxAttempts})...", True)

                Dim response = SendPostRequest(setTaskUrl, jsonPayload)

                If Not response.IsSuccessStatusCode Then
                    status?.AppendStatusLine(
                        $"Tracker Set Task failed. HTTP {(CInt(response.StatusCode))} – {response.ReasonPhrase}", True)
                    ' Still continue to retry – tracker might be starting up
                Else
                    ' Give SSC-Tracker time to apply the new task
                    Thread.Sleep(delayMs)

                    Dim trackerState = GetTrackerState(jsonUrl, status)

                    If trackerState IsNot Nothing AndAlso
                       TrackerStateLooksOk(trackerState, expectedPlnTitle, status) Then

                        status?.AppendStatusLine("Tracker confirmed updated task successfully.", True)
                        Exit Sub
                    Else
                        status?.AppendStatusLine("Tracker state does not match expected flight plan yet – retrying.", True)
                    End If
                End If
            Next

            status?.AppendStatusLine(
                "Failed to reliably apply task to Tracker after several attempts.", True)

        Catch ex As Exception
            status?.AppendStatusLine($"An error occurred while communicating with Tracker: {ex.Message}", True)
        End Try
    End Sub

    Private Function SendPostRequest(apiUrl As String, jsonPayload As String) As HttpResponseMessage
        Using client As New HttpClient()
            Dim content As New StringContent(jsonPayload, Encoding.UTF8, "application/json")
            Return client.PostAsync(apiUrl, content).Result
        End Using
    End Function

    Private Function GetTrackerState(jsonUrl As String, status As frmStatus) As TrackerItems
        Try
            Dim response = TrackerHttpClient.GetAsync(jsonUrl).Result
            If Not response.IsSuccessStatusCode Then
                status?.AppendStatusLine(
                    $"Unable to get Tracker state. HTTP {(CInt(response.StatusCode))}.",
                    True)
                Return Nothing
            End If

            Dim json = response.Content.ReadAsStringAsync().Result
            Dim state = JsonConvert.DeserializeObject(Of TrackerItems)(json)
            Return state

        Catch ex As Exception
            status?.AppendStatusLine($"Error while querying Tracker /json: {ex.Message}", True)
            Return Nothing
        End Try
    End Function

    Private Function TrackerStateLooksOk(state As TrackerItems,
                                         expectedPlnTitle As String,
                                         status As frmStatus) As Boolean
        Dim trackerTaskTitle As String = If(state?.TASK, String.Empty)

        status?.AppendStatusLine(
            $"Tracker state → TASK='{trackerTaskTitle}'",
            True)

        If String.IsNullOrWhiteSpace(expectedPlnTitle) Then
            ' If for some reason we don't have a title, don't block on this check
            Return True
        End If

        Return String.Equals(trackerTaskTitle?.Trim(),
                             expectedPlnTitle.Trim(),
                             StringComparison.OrdinalIgnoreCase)
    End Function

    Friend Function ExecuteTrackerTaskFolderCleanup() As String
        Try
            Dim taskFolder As String = Nothing
            Using key = Registry.CurrentUser.OpenSubKey("Software\\SSC")
                If key IsNot Nothing Then
                    taskFolder = TryCast(key.GetValue("TaskFolder"), String)
                End If
            End Using

            If String.IsNullOrWhiteSpace(taskFolder) Then
                Return "Tracker temporary task folder cleanup skipped (registry value not found)."
            End If

            taskFolder = taskFolder.Trim()
            If taskFolder.Length = 0 Then
                Return "Tracker temporary task folder cleanup skipped (registry value not found)."
            End If

            If Not Directory.Exists(taskFolder) Then
                Return $"Tracker temporary task folder cleanup skipped (""{taskFolder}"" not found)."
            End If

            Dim trackerFolder As New DirectoryInfo(taskFolder)
            Dim errors As New List(Of String)()

            For Each fileInfo In trackerFolder.GetFiles()
                Try
                    fileInfo.Attributes = FileAttributes.Normal
                    fileInfo.Delete()
                Catch ex As Exception
                    errors.Add($"File '{fileInfo.Name}': {ex.Message}")
                End Try
            Next

            For Each dirInfo In trackerFolder.GetDirectories()
                Try
                    ResetDirectoryAttributes(dirInfo)
                    dirInfo.Delete(True)
                Catch ex As Exception
                    errors.Add($"Folder '{dirInfo.Name}': {ex.Message}")
                End Try
            Next

            If errors.Count > 0 Then
                Return $"Tracker temporary task folder cleanup failed: {String.Join("; ", errors)}"
            End If

            Return "Cleaned up the Tracker's temporary tasks folder."
        Catch ex As Exception
            Return $"Tracker temporary task folder cleanup failed: {ex.Message}"
        End Try
    End Function

    Private Sub ResetDirectoryAttributes(directory As DirectoryInfo)
        For Each subDir In directory.GetDirectories()
            ResetDirectoryAttributes(subDir)
        Next

        For Each fileInfo In directory.GetFiles()
            fileInfo.Attributes = FileAttributes.Normal
        Next

        directory.Attributes = FileAttributes.Normal
    End Sub

    Friend Function EnsureNb21Running(status As frmStatus) As Boolean
        Return EnsureExternalCompanion(
            processName:="NB21_logger",
            exeFolder:=Settings.SessionSettings.NB21EXEFolder,
            exeName:="NB21_logger.exe",
            port:=Settings.SessionSettings.NB21LocalWSPort,
            status:=status,
            startSuccessMessage:="External NB21 Logger successfully started.",
            alreadyRunningMessage:="External NB21 Logger is already running.",
            missingExeMessage:=$"The NB21 Logger's executable file was not found in {Settings.SessionSettings.NB21EXEFolder}",
            notReadyMessage:="NB21 Logger did not become ready within the timeout period.",
            errorPrefix:="An error occurred trying to launch NB21 Logger")
    End Function

    Friend Function EnsureTrackerRunning(status As frmStatus) As Boolean

        'Make sure SSC is set to not copy task files to the simulator folder
        Using sscKey As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\SSC", True)
            If sscKey Is Nothing Then
                Throw New InvalidOperationException("Unable to create the registry key HKCU\\Software\\SSC.")
            End If
            sscKey.SetValue("CopyTaskFilesToSim", 0, RegistryValueKind.DWord)
        End Using

        Return EnsureExternalCompanion(
            processName:="SSC-Tracker",
            exeFolder:=Settings.SessionSettings.TrackerEXEFolder,
            exeName:="SSC-Tracker.exe",
            port:=Settings.SessionSettings.TrackerLocalWSPort,
            status:=status,
            startSuccessMessage:="Tracker successfully started.",
            alreadyRunningMessage:="Tracker is already running.",
            missingExeMessage:=$"The Tracker's executable file was not found in {Settings.SessionSettings.TrackerEXEFolder}",
            notReadyMessage:="Tracker did not become ready within the timeout period.",
            errorPrefix:="An error occurred trying to launch the Tracker")
    End Function

    Private Function EnsureExternalCompanion(processName As String,
                                             exeFolder As String,
                                             exeName As String,
                                             port As Integer?,
                                             status As frmStatus,
                                             startSuccessMessage As String,
                                             alreadyRunningMessage As String,
                                             missingExeMessage As String,
                                             notReadyMessage As String,
                                             errorPrefix As String) As Boolean
        Dim processList As Process() = Process.GetProcessesByName(processName)
        Dim running As Boolean = processList.Length > 0

        If running Then
            status?.AppendStatusLine(alreadyRunningMessage, False)
            Return True
        End If

        Dim exePath As String = Path.Combine(exeFolder, exeName)
        If Not File.Exists(exePath) Then
            status?.AppendStatusLine(missingExeMessage, True)
            Return False
        End If

        Try
            Dim launchedProcess As Process = Process.Start(exePath)
            Dim ready As Boolean = True

            If port.HasValue Then
                ready = False
                If launchedProcess IsNot Nothing AndAlso launchedProcess.WaitForInputIdle(5000) Then
                    For i As Integer = 1 To 10
                        If IsPortOpen("localhost", port.Value, 500) Then
                            ready = True
                            Exit For
                        End If
                        Thread.Sleep(500)
                    Next
                End If
            End If

            If ready Then
                status?.AppendStatusLine(startSuccessMessage, False)
                Return True
            Else
                status?.AppendStatusLine(notReadyMessage, True)
            End If

        Catch ex As Exception
            status?.AppendStatusLine($"{errorPrefix}: {ex.Message}", True)
        End Try

        Return False
    End Function

    Friend Function IsPortOpen(host As String, port As Integer, timeout As Integer) As Boolean
        Try
            Using client As New Net.Sockets.TcpClient()
                Dim result = client.BeginConnect(host, port, Nothing, Nothing)
                Dim success = result.AsyncWaitHandle.WaitOne(timeout)
                If success Then
                    client.EndConnect(result)
                    Return True
                End If
            End Using
        Catch ex As Exception
        End Try
        Return False
    End Function

End Module

' Classes for SSC-Tracker /json response
Friend Class TrackerItems
    Public Property GN As String = ""
    Public Property TASK As String = ""
    Public Property WEATHER As String = ""
    Public Property ID As String = ""
    Public Property CONFIG As Integer = 0
    Public Property ITEMS As List(Of TrackerItem) = New List(Of TrackerItem)()
End Class

Friend Class TrackerItem
    ' Details not needed for this check yet
End Class
