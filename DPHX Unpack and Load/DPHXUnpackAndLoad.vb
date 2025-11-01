Imports System.Diagnostics.Eventing
Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports System.Xml.Serialization
Imports CefSharp.DevTools.Page
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary
Imports SIGLR.SoaringTools.ImageViewer

Public Class DPHXUnpackAndLoad

#Region "Constants and other global variables"

    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"
    Private Const READY_EVENT_NAME As String = "WSG_DPHX_READY"

    Private ReadOnly _SF As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _currentFile As String = String.Empty
    Private _abortingFirstRun As Boolean = False
    Private _allDPHData As AllData
    Private _filesToUnpack2020 As New Dictionary(Of String, String)
    Private _filesCurrentlyUnpacked2020 As New Dictionary(Of String, String)
    Private _filesToUnpack2024 As New Dictionary(Of String, String)
    Private _filesCurrentlyUnpacked2024 As New Dictionary(Of String, String)
    Private _currentNewsKeyPublished As New Dictionary(Of String, Date)
    Private _showingPrompt As Boolean = False
    Private _lastLoadSuccess As Boolean = False
    Private _status As New frmStatus()
    Private _taskDiscordPostID As String = String.Empty
    Private _pipeServer As PipeCommandServer
    Private _wsgListenerStartedAsTransient As Boolean = False
    Private _readySignalForListener As EventWaitHandle
    Private _isClosing As Boolean = False

#End Region

#Region "Global exception handler"

    Public Sub New()
        ' Subscribe to global exception handlers
        AddHandler Application.ThreadException, AddressOf Application_ThreadException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException

        'Only one instance can run
        Dim currentProcess As Process = Process.GetCurrentProcess()
        Dim processes() As Process = Process.GetProcessesByName(currentProcess.ProcessName)

        If processes.Length > 1 Then
            ' Another instance is already running, show an error message or take appropriate action
            MessageBox.Show("Another instance of the application is already running.", "Application Already Running", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Application.Exit()
            End
        End If

        ' Initialize the form and other components
        InitializeComponent()
    End Sub

    Private Sub Application_ThreadException(ByVal sender As Object, ByVal e As ThreadExceptionEventArgs)
        ' Handle the exception
        GlobalExceptionHandler(e.Exception)
    End Sub

    Private Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        ' Handle the exception
        GlobalExceptionHandler(e.ExceptionObject)
    End Sub

    Private Sub GlobalExceptionHandler(e As Exception)

        Dim CommonLibraryAssembly As Assembly = GetType(SupportingFeatures).Assembly
        Dim ImageViewerAssembly As Assembly = GetType(ImageBox).Assembly

        Clipboard.SetText($"{Me.GetType.Assembly.GetName.ToString}{vbCrLf}{CommonLibraryAssembly.GetName.ToString}{vbCrLf}{ImageViewerAssembly.GetName.ToString}{vbCrLf}{vbCrLf}{e.Message}{vbCrLf}{vbCrLf}{e.ToString}")
        MessageBox.Show($"An unhandled exception occurred and all details have been added to your clipboard.{vbCrLf}Please paste the content in the 'questions-support' channel on Discord.{vbCrLf}{vbCrLf}{Clipboard.GetText}", "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Application.Exit()
    End Sub
#End Region

#Region "Form events"

    Private Sub DPHXUnpackAndLoad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        msfs2020ToolStrip.Visible = False
        msfs2024ToolStrip.Visible = False

        If Not _SF.CheckRequiredNetFrameworkVersion Then
            MessageBox.Show("This application requires Microsoft .NET Framework 4.8 or later to be present.", "Installation does not meet requirement", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _abortingFirstRun = True
            Application.Exit()
            Exit Sub
        End If

        Dim firstRun As Boolean = Not Settings.SessionSettings.Load()
        SetFormCaption(_currentFile)

        Rescale()

        RestoreMainFormLocationAndSize()

        Me.Show()
        Me.Refresh()
        If firstRun Then

            Do While True
                Select Case OpenSettingsWindow()
                    Case DialogResult.Abort
                        _abortingFirstRun = True
                        Exit Do
                    Case DialogResult.OK
                        Exit Do
                End Select
            Loop

            If _abortingFirstRun Then
                Me.Close()
                Application.Exit()
            End If

        End If

        If Not _abortingFirstRun Then
            Dim createdNew As Boolean
            _readySignalForListener = New EventWaitHandle(False, EventResetMode.ManualReset, READY_EVENT_NAME, createdNew)
            ' Ensure not-ready on startup even if a prior run left it signaled
            _readySignalForListener.Reset()

            ' Start the server
            _pipeServer = New PipeCommandServer()
            AddHandler _pipeServer.CommandReceived, AddressOf OnPipeCommand
            _pipeServer.Start()

            If CheckForNewVersion() Then
                Exit Sub
            End If

            lbl2020AllFilesStatus.Text = String.Empty
            lbl2024AllFilesStatus.Text = String.Empty

            SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder)

            Dim doUnpack As Boolean = False
            Dim ignoreWSGIntegration As Boolean = False

            Dim args = My.Application.CommandLineArgs
            Dim preventWSG As Boolean = args.Any(Function(a) a.Equals("--prevent-wsg", StringComparison.OrdinalIgnoreCase))
            Dim fileArg As String = args.FirstOrDefault(Function(a) Not a.StartsWith("--"))

            If Not String.IsNullOrEmpty(fileArg) Then
                ' Open the file passed as an argument
                _currentFile = My.Application.CommandLineArgs(0)
                doUnpack = True
                ignoreWSGIntegration = Settings.SessionSettings.WSGIgnoreWhenOpeningDPHX
            Else
                ' Check the last file that was opened
                If Not Settings.SessionSettings.LastDPHXOpened = String.Empty AndAlso File.Exists(Settings.SessionSettings.LastDPHXOpened) Then
                    _currentFile = Settings.SessionSettings.LastDPHXOpened
                End If
            End If

            If Not _currentFile = String.Empty AndAlso Path.GetExtension(_currentFile) = ".dphx" Then
                LoadDPHXPackage(_currentFile)
                If Settings.SessionSettings.AutoUnpack AndAlso IsUnpackRed AndAlso doUnpack AndAlso _lastLoadSuccess Then
                    UnpackFiles()
                End If
            End If

            msfs2020ToolStrip.Visible = Settings.SessionSettings.Is2020Installed
            msfs2024ToolStrip.Visible = Settings.SessionSettings.Is2024Installed

            ' Is the listener running?
            Dim listenerStartupResult = MakeSureWSGListenerIsRunning(Not Settings.SessionSettings.WSGListenerAutoStart)

            If listenerStartupResult <> String.Empty Then
                MessageBox.Show(listenerStartupResult, "Error launching WSGListener", MessageBoxButtons.OK, MessageBoxIcon.Error)
                _abortingFirstRun = True
                Application.Exit()
                Exit Sub
            End If

            If Not ignoreWSGIntegration AndAlso Not preventWSG Then
                'Check WSG integration
                Select Case Settings.SessionSettings.WSGIntegration
                    Case AllSettings.WSGIntegrationOptions.None
                    'Do nothing
                    Case AllSettings.WSGIntegrationOptions.OpenHome
                        'Open map in WSG
                        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=home")
                    Case AllSettings.WSGIntegrationOptions.OpenMap
                        'Open map in WSG
                        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=map")
                    Case AllSettings.WSGIntegrationOptions.OpenEvents
                        'Open events in WSG
                        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=events")
                End Select
            End If

            _readySignalForListener.Set()

        End If

    End Sub

    Private Sub DPHXUnpackAndLoad_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        _isClosing = True

        ' Block new commands from the listener
        Try : _readySignalForListener?.Reset() : Catch : End Try

        If Not _abortingFirstRun Then
            ctrlBriefing.Closing()

            ' best-effort temp cleanup
            Dim tries As Integer = 0
            Do While tries < 10 AndAlso Not SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder)
                tries += 1
                Me.Refresh()
                Application.DoEvents()
            Loop

            ' Politely stop the listener if we started it transiently
            If _wsgListenerStartedAsTransient AndAlso Not Settings.SessionSettings.WSGListenerAutoStart Then
                Try : SendCommandToWSG("shutdown") : Catch : End Try
            End If
        End If

        ' Stop pipe server early to avoid commands racing during teardown
        If _pipeServer IsNot Nothing Then
            Try
                RemoveHandler _pipeServer.CommandReceived, AddressOf OnPipeCommand
                _pipeServer.Stop()
            Catch
            Finally
                _pipeServer = Nothing
            End Try
        End If

        ' Persist UI state (best-effort)
        Try
            Settings.SessionSettings.MainFormSize = Me.Size.ToString()
            Settings.SessionSettings.MainFormLocation = Me.Location.ToString()
            Settings.SessionSettings.Save()
        Catch
        End Try

        If _status IsNot Nothing Then
            Try : _status.Close() : Catch : End Try
        End If
    End Sub

    Private Sub DPHXUnpackAndLoad_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' Release the named event
        Try : _readySignalForListener?.Dispose() : Catch : End Try
    End Sub

    Private Sub DPHXUnpackAndLoad_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ctrlBriefing.AdjustRTBoxControls()
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles toolStripSettings.Click

        Dim oldPort As Integer = Integer.Parse(Settings.SessionSettings.LocalWebServerPort)

        OpenSettingsWindow()

        msfs2020ToolStrip.Visible = Settings.SessionSettings.Is2020Installed
        msfs2024ToolStrip.Visible = Settings.SessionSettings.Is2024Installed

        'Recheck files
        If toolStripUnpack.Enabled Then
            EnableUnpackButton()
        End If

        Dim newPort As Integer = Integer.Parse(Settings.SessionSettings.LocalWebServerPort)
        If oldPort <> newPort Then
            'Port has changed, send the set-port command to the WSG Listener
            SendCommandToWSG("set-port", $"port={newPort}", oldPort)
        End If


    End Sub

    Private Sub LoadDPHX_Click(sender As Object, e As EventArgs) Handles toolStripOpen.Click

        lbl2020AllFilesStatus.Text = String.Empty

        If txtPackageName.Text = String.Empty Then
            If Directory.Exists(Settings.SessionSettings.PackagesFolder) Then
                OpenFileDialog1.InitialDirectory = Settings.SessionSettings.PackagesFolder
            Else
                OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            End If
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtPackageName.Text)
        End If

        OpenFileDialog1.FileName = String.Empty
        OpenFileDialog1.Title = "Select DPHX package file to load"
        OpenFileDialog1.Filter = "Discord Post Helper Package|*.dphx"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            LoadDPHXPackage(OpenFileDialog1.FileName)
            If Settings.SessionSettings.AutoUnpack AndAlso _lastLoadSuccess Then
                UnpackFiles()
            End If
        End If

    End Sub

    Private Sub btnCopyFiles_Click(sender As Object, e As EventArgs) Handles toolStripUnpack.Click

        UnpackFiles()

    End Sub

    Private Sub btnCleanup_Click(sender As Object, e As EventArgs) Handles toolStripCleanup.Click

        CleanupFiles()

    End Sub

    Private Sub btnFileBrowser_Click(sender As Object, e As EventArgs) Handles toolStripFileBrowser.Click

        If warningMSFSRunningToolStrip.Visible Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be deleted but weather preset will remain available until MSFS is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    ShowFileBrowserForm()
                End If
            End Using
        Else
            ShowFileBrowserForm()
        End If

        'Recheck files
        If toolStripUnpack.Enabled Then
            EnableUnpackButton()
        End If

    End Sub

    Private Sub ShowFileBrowserForm()
        Dim formCleanup As New CleaningTool
        formCleanup.ShowDialog(Me)
        formCleanup.Dispose()
        formCleanup = Nothing
    End Sub

    Private Sub btnLoadB21_Click(sender As Object, e As EventArgs) Handles toolStripB21Planner.Click

        If _allDPHData Is Nothing Then
            Exit Sub
        End If

        Dim flightplanFilename As String = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename))
        Dim weatherFilename As String = String.Empty

        If Not _allDPHData.WeatherFilename = String.Empty Then
            weatherFilename = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename))
        End If

        If flightplanFilename Is String.Empty Then
        Else
            If weatherFilename = String.Empty OrElse ctrlBriefing.WeatherProfileInnerXML = String.Empty Then
                _SF.OpenB21Planner(flightplanFilename, ctrlBriefing.FlightPlanInnerXML, String.Empty, String.Empty, Settings.SessionSettings.NB21IGCFolder)
            Else
                _SF.OpenB21Planner(flightplanFilename, ctrlBriefing.FlightPlanInnerXML, weatherFilename, ctrlBriefing.WeatherProfileInnerXML, Settings.SessionSettings.NB21IGCFolder)
            End If

        End If

    End Sub

    Private Sub ChkMSFS_Tick(sender As Object, e As EventArgs) Handles ChkMSFS.Tick
        ' look for either the old or the new process name
        Dim running As Boolean = Process.GetProcessesByName("FlightSimulator").Any() _
                          OrElse Process.GetProcessesByName("FlightSimulator2024").Any()

        warningMSFSRunningToolStrip.Visible = running
    End Sub

    Private Sub DiscordInviteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DiscordInviteToolStripMenuItem.Click

        Dim inviteURL As String = "https://discord.gg/aW8YYe3HJF"
        Clipboard.SetText(inviteURL)
        Using New Centered_MessageBox()
            MessageBox.Show("The invite link has been copied to your clipboard. Paste it in the Join Discord Server invite field on Discord.", "Invite link copied", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using

    End Sub

    Private Sub DiscordChannelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DiscordChannelToolStripMenuItem.Click

        SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/1022705603489042472/1101255857683042466")

    End Sub

    Private Sub GoToFeedbackChannelOnDiscordToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoToFeedbackChannelOnDiscordToolStripMenuItem.Click

        SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/1022705603489042472/1101255812883693588")

    End Sub

    Private Sub OnPipeCommand(sender As Object, e As CommandEventArgs)
        If _isClosing OrElse Me.IsDisposed OrElse Not Me.IsHandleCreated Then Return

        Select Case e.Action
            Case "download-task"
                ' Boolean fromEventTab = (e.Source = "event")
                Me.Invoke(Sub() SupportingFeatures.BringWindowToFront(Me))
                Me.Invoke(Sub() RequestReceivedFromWSGListener(e.TaskID, e.Title, e.Source = "event"))
            Case "foreground"
                Me.Invoke(Sub() SupportingFeatures.BringWindowToFront(Me))
            Case "reload-user-info"
                Me.Invoke(Sub() ReloadSettings())
        End Select
    End Sub

    Private Sub RequestReceivedFromWSGListener(taskId As String, taskTitle As String, fromEventTab As Boolean)
        Dim downloaded = SupportingFeatures.DownloadTaskFile(
                      taskId, taskTitle, Settings.SessionSettings.PackagesFolder)

        SupportingFeatures.BringWindowToFront(Me)

        If downloaded <> String.Empty Then
            LoadDPHXPackage(downloaded)
            If Settings.SessionSettings.AutoUnpack AndAlso _currentFile <> "" AndAlso _lastLoadSuccess Then
                UnpackFiles(fromEventTab)
            End If
        End If
    End Sub

#End Region

#Region "Subs and functions"

    Private Sub SetFormCaption(filename As String)

        If filename = String.Empty Then
            filename = "No DPHX package loaded"
        End If

        'Add version to form title
        Me.Text = $"DPHX Unpack and Load v{Me.GetType.Assembly.GetName.Version} - {filename}"

    End Sub

    Private Function CheckForNewVersion() As Boolean
        Dim myVersionInfo As VersionInfo = _SF.GetVersionInfo()
        Dim message As String = String.Empty

        If myVersionInfo IsNot Nothing Then
            If _SF.FormatVersionNumber(myVersionInfo.CurrentLatestVersion) > _SF.FormatVersionNumber(Me.GetType.Assembly.GetName.Version.ToString) Then
                'New version available
                If _SF.ShowVersionForm(myVersionInfo, Me.GetType.Assembly.GetName.Version.ToString) = DialogResult.Yes Then
                    'update
                    'Download the file
                    If _SF.DownloadLatestUpdate(myVersionInfo.CurrentLatestVersion, message) Then
                        Try : SendCommandToWSG("shutdown") : Catch : End Try
                        Application.Exit()
                        Return True
                    Else
                        'Show error updating
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show(Me, $"An error occured during the update process at this step:{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}The update did not complete.", "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using
                    End If
                End If
            End If
        End If

        Return False

    End Function

    Private Function OpenSettingsWindow() As DialogResult
        Dim formSettings As New Settings

        Dim result As DialogResult = formSettings.ShowDialog(Me)

        ' Check if the WSG Listener should be re-started (transient vs auto-start setting)
        If _wsgListenerStartedAsTransient AndAlso Settings.SessionSettings.WSGListenerAutoStart Then
            'Was started as transient, but now auto-start is enabled - shutdown and restart in permanent mode
            SendCommandToWSG("shutdown")
            SupportingFeatures.WaitForProcessExit("WSGListener", 5000)
            MakeSureWSGListenerIsRunning(False)
        ElseIf (Not _wsgListenerStartedAsTransient) AndAlso (Not Settings.SessionSettings.WSGListenerAutoStart) Then
            'Was started as permanent, but now auto-start is disabled - shutdown and restart in transient mode
            SendCommandToWSG("shutdown")
            SupportingFeatures.WaitForProcessExit("WSGListener", 5000)
            MakeSureWSGListenerIsRunning(True)
        End If

        Return result

    End Function

    Private Function TempDPHXUnpackFolder() As String
        Return Path.Combine(Settings.SessionSettings.UnpackingFolder, "TempDPHXUnpack")
    End Function

    Private Sub LoadDPHXPackage(dphxFilename As String)

        Dim newDPHFile As String
        _lastLoadSuccess = False

        ctrlBriefing.FullReset()
        Me.Refresh()
        Application.DoEvents()
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

            Using stream As New FileStream(newDPHFile, FileMode.Open)
                _allDPHData = CType(serializer.Deserialize(stream), AllData)
            End Using

            'Fix the weather file format to be compatible with both MSFS versions
            If _allDPHData.WeatherFilename <> String.Empty Then
                'Weather file present
                Dim weatherFile As String = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename))
                If File.Exists(weatherFile) Then
                    If _SF.FixWPRFormat(weatherFile, False) Then
                        'Success
                    Else
                        'Failure
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show($"Unable to verify and fix the weather file for compatibility with both MSFS versions.", "Fixing WPR file", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End Using
                    End If
                End If
            End If

            'We need to retrieve the DiscordPostID from the task online server
            GetTaskDetails(_allDPHData.TaskID, _allDPHData.EntrySeqID)

            ctrlBriefing.GenerateBriefing(_SF,
                                          _allDPHData,
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename)),
                                          _taskDiscordPostID,
                                          TempDPHXUnpackFolder)

            If newDPHFile = String.Empty Then
                'Invalid file loaded
                txtPackageName.Text = String.Empty
                _currentFile = String.Empty
                DisableUnpackButton()
            Else
                txtPackageName.Text = dphxFilename
                _currentFile = dphxFilename
                _lastLoadSuccess = True
                EnableUnpackButton()
            End If

            Settings.SessionSettings.LastDPHXOpened = _currentFile
        End If

        SetFormCaption(_currentFile)
        packageNameToolStrip.Text = _currentFile

    End Sub

    Private Sub GetTaskDetails(taskID As String, entrySeqID As Integer)
        Try

            Dim taskDetailsUrl As String = String.Empty
            Dim request As HttpWebRequest = Nothing

            If entrySeqID > 0 Then
                'Use EntrySeqID
                taskDetailsUrl = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingEntrySeqID.php"
                request = CType(WebRequest.Create(taskDetailsUrl & "?EntrySeqID=" & entrySeqID.ToString), HttpWebRequest)
            Else
                taskDetailsUrl = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingID.php"
                request = CType(WebRequest.Create(taskDetailsUrl & "?TaskID=" & taskID), HttpWebRequest)
            End If

            ' Create the web request
            request.Method = "GET"
            request.ContentType = "application/json"

            ' Get the response
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    ' Check the status
                    If result("status").ToString() = "success" Then
                        If entrySeqID = 0 Then
                            '_TaskEntrySeqID = result("taskDetails")("EntrySeqID")
                            _taskDiscordPostID = String.Empty
                        Else
                            _taskDiscordPostID = result("taskDetails")("DiscordPostID").ToString()
                        End If
                    Else
                        '_TaskEntrySeqID = 0
                        Throw New Exception("Error retrieving task details: " & result("message").ToString())
                    End If
                End Using
            End Using

        Catch ex As Exception
            Throw New Exception("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub DisableUnpackButton()
        toolStripUnpack.Enabled = False
        toolStripCleanup.Enabled = False
        toolStripB21Planner.Enabled = False
        toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Regular)
    End Sub

    Private Sub SetFilesToUnpack()

        _filesToUnpack2020.Clear()
        _filesCurrentlyUnpacked2020.Clear()
        _filesToUnpack2024.Clear()
        _filesCurrentlyUnpacked2024.Clear()

        'Check if files are already unpacked

        If Settings.SessionSettings.Is2020Installed Then
            'Flight plan
            _filesToUnpack2020.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            If SupportingFeatures.FilesAreEquivalent(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2020FlightPlansFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename))) Then
                _filesCurrentlyUnpacked2020.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            End If

            'Weather file
            _filesToUnpack2020.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            If SupportingFeatures.FilesAreEquivalent(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename))) Then
                _filesCurrentlyUnpacked2020.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            End If
        End If
        If Settings.SessionSettings.Is2024Installed Then
            'Flight plan
            _filesToUnpack2024.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            If SupportingFeatures.FilesAreEquivalent(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2024FlightPlansFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename))) Then
                _filesCurrentlyUnpacked2024.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            End If

            'Weather file
            _filesToUnpack2024.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            If SupportingFeatures.FilesAreEquivalent(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename))) Then
                _filesCurrentlyUnpacked2024.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            End If
        End If

        'XCSoar task
        If Settings.SessionSettings.XCSoarTasksFolder IsNot Nothing Then
            'Look in the other files for xcsoar file
            For Each filepath As String In _allDPHData.ExtraFiles
                If Path.GetExtension(filepath) = ".tsk" Then
                    'XCSoar task
                    _filesToUnpack2020.Add("XCSoar Task", Path.GetFileName(filepath))
                    _filesToUnpack2024.Add("XCSoar Task", Path.GetFileName(filepath))
                    If SupportingFeatures.FilesAreEquivalent(Path.Combine(TempDPHXUnpackFolder,
                                                            Path.GetFileName(filepath)),
                                                            Path.Combine(Settings.SessionSettings.XCSoarTasksFolder,
                                                            Path.GetFileName(filepath))) Then
                        _filesCurrentlyUnpacked2020.Add("XCSoar Task", Path.GetFileName(filepath))
                        _filesCurrentlyUnpacked2024.Add("XCSoar Task", Path.GetFileName(filepath))
                    End If
                End If
            Next
        End If

        'XCSoar maps
        If Settings.SessionSettings.XCSoarMapsFolder IsNot Nothing Then
            'Look in the other files for xcsoar file
            For Each filepath As String In _allDPHData.ExtraFiles
                If Path.GetExtension(filepath) = ".xcm" Then
                    'XCSoar map
                    _filesToUnpack2020.Add("XCSoar Map", Path.GetFileName(filepath))
                    _filesToUnpack2024.Add("XCSoar Map", Path.GetFileName(filepath))
                    If SupportingFeatures.FilesAreEquivalent(Path.Combine(TempDPHXUnpackFolder,
                                                            Path.GetFileName(filepath)),
                                                            Path.Combine(Settings.SessionSettings.XCSoarMapsFolder,
                                                            Path.GetFileName(filepath))) Then
                        _filesCurrentlyUnpacked2020.Add("XCSoar Map", Path.GetFileName(filepath))
                        _filesCurrentlyUnpacked2024.Add("XCSoar Map", Path.GetFileName(filepath))
                    End If
                End If
            Next
        End If

    End Sub

    Private Sub EnableUnpackButton()
        toolStripUnpack.Enabled = True
        toolStripCleanup.Enabled = True
        toolStripB21Planner.Enabled = True

        SetFilesToUnpack()

        tool2020StatusOK.Visible = False
        tool2020StatusWarning.Visible = False
        tool2020StatusStop.Visible = False

        tool2024StatusOK.Visible = False
        tool2024StatusWarning.Visible = False
        tool2024StatusStop.Visible = False

        If Settings.SessionSettings.Is2020Installed Then
            If _filesToUnpack2020.Count <> _filesCurrentlyUnpacked2020.Count Then
                toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Bold)
                toolStripUnpack.ForeColor = Color.Red
                If _filesCurrentlyUnpacked2020.Count = 0 Then
                    lbl2020AllFilesStatus.Text = $"All files ({GetListOfFilesMissing(_filesCurrentlyUnpacked2020, _filesToUnpack2020)}) are MISSING from their respective folder."
                    tool2020StatusStop.Visible = True
                Else
                    lbl2020AllFilesStatus.Text = $"{_filesCurrentlyUnpacked2020.Count} out of {_filesToUnpack2020.Count} files are present: {GetListOfFilesPresent(_filesCurrentlyUnpacked2020)}. MISSING files: {GetListOfFilesMissing(_filesCurrentlyUnpacked2020, _filesToUnpack2020)}"
                    tool2020StatusWarning.Visible = True
                End If
            Else
                toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Regular)
                toolStripUnpack.ForeColor = DefaultForeColor
                lbl2020AllFilesStatus.Text = $"All the files ({GetListOfFilesPresent(_filesCurrentlyUnpacked2020)}) are present in their respective folder."
                tool2020StatusOK.Visible = True
            End If
        End If
        If Settings.SessionSettings.Is2024Installed Then
            If _filesToUnpack2024.Count <> _filesCurrentlyUnpacked2024.Count Then
                toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Bold)
                toolStripUnpack.ForeColor = Color.Red
                If _filesCurrentlyUnpacked2024.Count = 0 Then
                    lbl2024AllFilesStatus.Text = $"All files ({GetListOfFilesMissing(_filesCurrentlyUnpacked2024, _filesToUnpack2024)}) are MISSING from their respective folder."
                    tool2024StatusStop.Visible = True
                Else
                    lbl2024AllFilesStatus.Text = $"{_filesCurrentlyUnpacked2024.Count} out of {_filesToUnpack2024.Count} files are present: {GetListOfFilesPresent(_filesCurrentlyUnpacked2024)}. MISSING files: {GetListOfFilesMissing(_filesCurrentlyUnpacked2024, _filesToUnpack2024)}"
                    tool2024StatusWarning.Visible = True
                End If
            Else
                toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Regular)
                toolStripUnpack.ForeColor = DefaultForeColor
                lbl2024AllFilesStatus.Text = $"All the files ({GetListOfFilesPresent(_filesCurrentlyUnpacked2024)}) are present in their respective folder."
                tool2024StatusOK.Visible = True
            End If
        End If

    End Sub

    Private Function GetListOfFilesPresent(fileCurrentlyUnpacked As Dictionary(Of String, String)) As String

        Dim result As String = String.Empty

        For Each fileType In fileCurrentlyUnpacked.Keys
            If result = String.Empty Then
                result = $"{fileType}"
            Else
                result = $"{result}, {fileType}"
            End If
        Next

        Return result

    End Function

    Private Function GetListOfFilesMissing(fileCurrentlyUnpacked As Dictionary(Of String, String), filesToUnpack As Dictionary(Of String, String)) As String

        Dim result As String = String.Empty

        For Each fileType In filesToUnpack.Keys
            If Not fileCurrentlyUnpacked.Keys.Contains(fileType) Then
                If result = String.Empty Then
                    result = $"{fileType}"
                Else
                    result = $"{result}, {fileType}"
                End If
            End If
        Next

        Return result

    End Function

    Private Sub RestoreMainFormLocationAndSize()
        Dim sizeString As String = Settings.SessionSettings.MainFormSize
        Dim locationString As String = Settings.SessionSettings.MainFormLocation

        ' Restore Size
        If sizeString <> "" Then
            Dim sizeArray As String() = sizeString.TrimStart("{").TrimEnd("}").Split(",")
            Dim width As Integer = CInt(sizeArray(0).Split("=")(1))
            Dim height As Integer = CInt(sizeArray(1).Split("=")(1))
            Me.Size = New Size(width, height)
        End If

        ' Restore Location
        If locationString <> "" Then
            Dim locationArray As String() = locationString.TrimStart("{").TrimEnd("}").Split(",")
            Dim x As Integer = CInt(locationArray(0).Split("=")(1))
            Dim y As Integer = CInt(locationArray(1).Split("=")(1))

            ' Check if the saved position is off-screen or indicates a minimized window
            Dim potentialLocation As New Point(x, y)
            Dim isLocationVisible As Boolean = False

            ' Check if the potential location is within any screen's bounds
            For Each scr As Screen In Screen.AllScreens
                If scr.WorkingArea.Contains(potentialLocation) Then
                    isLocationVisible = True
                    Exit For
                End If
            Next

            If isLocationVisible Then
                Me.Location = potentialLocation
            Else
                ' Center the form on the primary screen if the saved location is not valid
                Me.StartPosition = FormStartPosition.CenterScreen
            End If
        Else
            ' Default to center screen if no location was saved
            Me.StartPosition = FormStartPosition.CenterScreen
        End If
    End Sub

    Private Sub UnpackFiles(Optional fromEvent As Boolean = False)

        _status.StartPosition = FormStartPosition.CenterParent
        _status.Start(Me)

        _status.AppendStatusLine("Unpacking Results:", True)

        ' NB21 auto-start and PLN feeding
        If Settings.SessionSettings.NB21StartAndFeed Then
            Dim NB21LoggerRunning As Boolean
            Dim processList As Process() = Process.GetProcessesByName("NB21_logger")

            NB21LoggerRunning = processList.Length > 0

            If Not NB21LoggerRunning Then
                ' NB21 logger not already running - attempt to start it
                Dim loggerExePath As String = Path.Combine(Settings.SessionSettings.NB21EXEFolder, "NB21_logger.exe")
                If File.Exists(loggerExePath) Then
                    Try
                        Dim loggerProcess As Process = Process.Start(loggerExePath)

                        ' Wait for the process to be ready
                        If loggerProcess.WaitForInputIdle(5000) Then
                            ' Optionally check for network readiness (if applicable)
                            For i As Integer = 1 To 10
                                If IsPortOpen("localhost", Settings.SessionSettings.NB21LocalWSPort, 500) Then
                                    NB21LoggerRunning = True
                                    _status.AppendStatusLine("NB21 Logger successfully started.", False)
                                    Exit For
                                End If
                                Thread.Sleep(500) ' Check every 500ms
                            Next
                        End If

                        If Not NB21LoggerRunning Then
                            _status.AppendStatusLine("NB21 Logger did not become ready within the timeout period.", True)
                        End If
                    Catch ex As Exception
                        _status.AppendStatusLine($"An error occurred trying to launch NB21 Logger: {ex.Message}", True)
                    End Try
                Else
                    _status.AppendStatusLine($"The NB21 Logger's executable file was not found in {Settings.SessionSettings.NB21EXEFolder}", True)
                End If
            Else
                _status.AppendStatusLine("NB21 Logger is already running.", False)
            End If

            If NB21LoggerRunning Then
                'Feed the PLN file to the logger
                SendPLNFileToNB21Logger(Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)))
            End If
        End If

        ' Tracker auto-start and data feeding
        If Settings.SessionSettings.TrackerStartAndFeed Then
            Dim TrackerRunning As Boolean
            Dim processList As Process() = Process.GetProcessesByName("SSC-Tracker")

            TrackerRunning = processList.Length > 0

            If Not TrackerRunning Then
                ' Tracker not already running - attempt to start it
                Dim trackerExePath As String = Path.Combine(Settings.SessionSettings.TrackerEXEFolder, "SSC-Tracker.exe")
                If File.Exists(trackerExePath) Then
                    Try
                        Dim trackerProcess As Process = Process.Start(trackerExePath)

                        ' Wait for the process to be ready
                        If trackerProcess.WaitForInputIdle(5000) Then
                            ' Optionally check for network readiness (if applicable)
                            For i As Integer = 1 To 10
                                If IsPortOpen("localhost", Settings.SessionSettings.TrackerLocalWSPort, 500) Then
                                    TrackerRunning = True
                                    _status.AppendStatusLine("Tracker successfully started.", False)
                                    Exit For
                                End If
                                Thread.Sleep(500) ' Check every 500ms
                            Next
                        End If

                        If Not TrackerRunning Then
                            _status.AppendStatusLine("Tracker did not become ready within the timeout period.", True)
                        End If

                    Catch ex As Exception
                        _status.AppendStatusLine($"An error occurred trying to launch the Tracker: {ex.Message}", True)
                    End Try
                Else
                    _status.AppendStatusLine($"The Tracker's executable file was not found in {Settings.SessionSettings.TrackerEXEFolder}", True)
                End If
            Else
                _status.AppendStatusLine("Tracker is already running.", False)
            End If

            If TrackerRunning Then
                'Feed the data to the tracker
                Dim groupToUse As String = String.Empty
                If _allDPHData.IsFutureOrActiveEvent OrElse fromEvent Then
                    groupToUse = _allDPHData.TrackerGroup
                End If
                SendDataToTracker(groupToUse,
                                  Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                  Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename)),
                                  _allDPHData.URLGroupEventPost
                                 )
            End If
        End If

        If Settings.SessionSettings.Is2020Installed Then
            'Flight plan
            _status.AppendStatusLine(CopyFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2020FlightPlansFolder,
                 "Flight Plan for MSFS 2020"), True)

            'Weather file
            _status.AppendStatusLine(CopyFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                 "Weather Preset for MSFS 2020"), True)

        End If
        If Settings.SessionSettings.Is2024Installed Then
            'Flight plan
            _status.AppendStatusLine(CopyFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2024FlightPlansFolder,
                 "Flight Plan for MSFS 2024"), True)

            If Settings.SessionSettings.EnableEFBFlightPlanCreation Then
                'Copy of flight plan but for the EFB (without extra soaring information on the waypoints)
                _status.AppendStatusLine(CreatePLNForEFB(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2024FlightPlansFolder,
                 "EFB Flight Plan for MSFS 2024"), True)
            End If

            'Weather file
            _status.AppendStatusLine(CopyFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                 "Weather Preset for MSFS 2024"), True)

        End If

        'Look in the other files for xcsoar files
        For Each filepath As String In _allDPHData.ExtraFiles
            If Path.GetExtension(filepath) = ".tsk" Then
                'XCSoar task
                _status.AppendStatusLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarTasksFolder,
                                    "XCSoar Task"), True)
            End If
            If Path.GetExtension(filepath) = ".xcm" Then
                'XCSoar map
                _status.AppendStatusLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarMapsFolder,
                                    "XCSoar Map"), True)
            End If
        Next

        _status.AppendStatusLine("Unpack completed, you can click Close below.", False)
        _status.Done()

        EnableUnpackButton()

    End Sub

    Private Function IsPortOpen(host As String, port As Integer, timeout As Integer) As Boolean
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
            ' Ignore exceptions (e.g., port not open)
        End Try
        Return False
    End Function

    Private ReadOnly Property IsUnpackRed As Boolean
        Get
            Return toolStripUnpack.ForeColor = Color.Red
        End Get
    End Property

    Private Sub SendPLNFileToNB21Logger(plnfilePath As String)

        ' Define the API endpoint and the path to the PLN file
        Dim apiUrl As String = $"http://localhost:{Settings.SessionSettings.NB21LocalWSPort}/pln_set"

        Try
            ' Read the contents of the PLN file
            Dim plnContent As String = File.ReadAllText(plnfilePath)

            ' Use HttpClient to send the POST request
            Using client As New HttpClient()
                Dim content As New StringContent(plnContent, Encoding.UTF8, "application/xml")
                Dim response As HttpResponseMessage = client.PostAsync(apiUrl, content).Result

                If response.IsSuccessStatusCode Then
                    _status.AppendStatusLine("PLN file successfully sent to NB21 Logger.", True)
                Else
                    _status.AppendStatusLine($"Failed to send PLN file to NB21 Logger. HTTP Status: {response.StatusCode}", True)
                End If
            End Using
        Catch ex As Exception
            _status.AppendStatusLine($"An error occurred while sending the PLN file: {ex.Message}", True)
        End Try

    End Sub

    ' Function to send a POST request
    Private Function SendPostRequest(apiUrl As String, jsonPayload As String) As HttpResponseMessage
        Using client As New HttpClient()
            Dim content As New StringContent(jsonPayload, Encoding.UTF8, "application/json")
            Return client.PostAsync(apiUrl, content).Result
        End Using
    End Function

    Private Sub SendDataToTracker(trackerGroup As String, plnfilePath As String, wprfilePath As String, infoURL As String)
        ' Define the API endpoint
        Dim apiUrl As String = $"http://localhost:{Settings.SessionSettings.TrackerLocalWSPort}/settask"

        Try
            ' Read the contents of the PLN and WPR files
            Dim plnContent As String = If(File.Exists(plnfilePath), File.ReadAllText(plnfilePath), "")
            Dim wprContent As String = If(File.Exists(wprfilePath), File.ReadAllText(wprfilePath), "")

            ' Extract filenames without extensions
            Dim extractFilename As Func(Of String, String) =
            Function(filePath)
                If String.IsNullOrEmpty(filePath) Then Return ""
                Dim fullFilename = Path.GetFileName(filePath) ' Get the filename with extension
                Return Path.GetFileNameWithoutExtension(fullFilename) ' Remove the extension
            End Function

            Dim plnFilename As String = extractFilename(plnfilePath)
            Dim wprFilename As String = extractFilename(wprfilePath)

            ' Build the payload as JSON
            Dim payload As New With {
            .CMD = "SET",
            .GN = trackerGroup,
            .TASK = plnFilename,
            .TASKDATA = plnContent,
            .WEATHER = wprFilename,
            .WEATHERDATA = wprContent,
            .TASKINFO = infoURL
            }

            ' Convert payload to JSON
            Dim jsonPayload As String = JsonConvert.SerializeObject(payload)

            Dim response = SendPostRequest(apiUrl, jsonPayload)

            ' Perform the call to tracker
            response = SendPostRequest(apiUrl, jsonPayload)

            If response.IsSuccessStatusCode Then
                _status.AppendStatusLine($"Call to SSC Tracker successful.", True)
            Else
                _status.AppendStatusLine($"Failed to communicate with Tracker on the second call. HTTP Status: {response.StatusCode}", True)
            End If

        Catch ex As Exception
            _status.AppendStatusLine($"An error occurred while communicating with Tracker: {ex.Message}", True)
        End Try
    End Sub

    Private Function CreatePLNForEFB(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String

        Try
            Dim fullSourceFilename = Path.Combine(sourcePath, filename)
            If Not File.Exists(fullSourceFilename) Then
                Return $"❌ Source file not found: {fullSourceFilename}"
            End If

            ' Read the original PLN file as XML (preserve whitespace to avoid reformat surprises)
            Dim xdoc = XDocument.Load(fullSourceFilename, LoadOptions.PreserveWhitespace)

            ' Parse all waypoints and remove the extra information from each ATCWaypoint id
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

            ' Write the new PLN file with _EFB suffix in the same folder
            Dim efbName = Path.GetFileNameWithoutExtension(fullSourceFilename) & "_EFB" & Path.GetExtension(fullSourceFilename)
            Dim efbPath = Path.Combine(sourcePath, efbName)

            Dim settings As New Xml.XmlWriterSettings With {
            .Indent = True,
            .Encoding = New UTF8Encoding(False) ' UTF-8, no BOM
        }
            Using w = Xml.XmlWriter.Create(efbPath, settings)
                xdoc.Save(w)
            End Using

            ' Copy the file to the destination folder (uses your existing CopyFile)
            If Not String.IsNullOrWhiteSpace(destPath) Then
                Return CopyFile(efbName, sourcePath, destPath, msgToAsk)
            End If

            Return "No destination folder set for EFB version of task!"

        Catch ex As Exception
            Return $"❌ CreatePLNForEFB failed: {ex.Message}"
        End Try
    End Function

    Private Function CleanWaypointId(value As String) As String
        If String.IsNullOrEmpty(value) Then Return value
        Dim s = value.Trim()

        ' Remove leading asterisks (e.g., *Start, *Finish)
        s = s.TrimStart("*"c)

        ' Remove everything from the first '+' onward (e.g., +6378|7200x500, +7497x2000, etc.)
        Dim plusIdx = s.IndexOf("+"c)
        If plusIdx >= 0 Then s = s.Substring(0, plusIdx)

        Return s.Trim()
    End Function

    Private Sub AppendMsfsRunningNoteForWpr(ByRef msg As String, fileName As String, isDelete As Boolean)
        If Not warningMSFSRunningToolStrip.Visible Then Exit Sub
        If Not fileName.EndsWith(".wpr", StringComparison.OrdinalIgnoreCase) Then Exit Sub

        If isDelete Then
            msg &= $"{Environment.NewLine}⚠️MSFS is running: the preset may remain visible/usable until MSFS is restarted."
        Else
            msg &= $"{Environment.NewLine}⚠️MSFS is running: the new/updated preset will not appear until MSFS is restarted."
        End If
    End Sub

    Private Function CopyFile(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String
        Dim fullSourceFilename As String
        Dim fullDestFilename As String
        Dim proceed As Boolean = False
        Dim messageToReturn As String = String.Empty

        If Not Directory.Exists(destPath) Then
            Return $"{msgToAsk} ""{filename}"" skipped - destination folder not set or invalid (check settings)"
        End If

        fullSourceFilename = Path.Combine(sourcePath, filename)
        fullDestFilename = Path.Combine(destPath, filename)

        ' We'll copy from this path; default to the incoming package file
        Dim sourceToCopy As String = fullSourceFilename

        ' --- WHITELIST ENFORCEMENT ---
        Dim whitelistDir = Path.Combine(Application.StartupPath, "Whitelist")
        Dim whitelistFile = Path.Combine(whitelistDir, filename)
        If File.Exists(whitelistFile) Then
            ' If dest exists and already matches Whitelist -> skip
            If File.Exists(fullDestFilename) AndAlso SupportingFeatures.FilesAreEquivalent(whitelistFile, fullDestFilename) Then
                Return $"{msgToAsk} ""{filename}"" skipped (protected by Whitelist)"
            End If

            ' Otherwise, enforce Whitelist content (overwrite or create) regardless of policy
            sourceToCopy = whitelistFile
            proceed = True
            If File.Exists(fullDestFilename) Then
                messageToReturn = $"{msgToAsk} ""{filename}"" replaced with Whitelist copy"
            Else
                messageToReturn = $"{msgToAsk} ""{filename}"" copied (from Whitelist)"
            End If
        Else
            ' --- normal logic (identical => skip; different => apply policy) ---
            If File.Exists(fullDestFilename) Then
                Dim srcInfo = New FileInfo(fullSourceFilename)
                Dim dstInfo = New FileInfo(fullDestFilename)

                Dim identical As Boolean = (srcInfo.Exists AndAlso dstInfo.Exists AndAlso srcInfo.Length = dstInfo.Length) _
                                   AndAlso SupportingFeatures.FilesAreEquivalent(fullSourceFilename, fullDestFilename)

                If identical Then
                    proceed = False
                    messageToReturn = $"{msgToAsk} ""{filename}"" skipped (identical - up-to-date)"
                Else
                    Select Case Settings.SessionSettings.AutoOverwriteFiles
                        Case AllSettings.AutoOverwriteOptions.AlwaysOverwrite
                            proceed = True
                            messageToReturn = $"{msgToAsk} ""{filename}"" copied over (different existing file, policy: Overwrite)"
                        Case AllSettings.AutoOverwriteOptions.AlwaysSkip
                            proceed = False
                            messageToReturn = $"{msgToAsk} ""{filename}"" skipped (different file exists, policy: Skip)"
                        Case AllSettings.AutoOverwriteOptions.AlwaysAsk
                            Using New Centered_MessageBox(Me)
                                If MessageBox.Show(
                                $"A different {msgToAsk} file already exists.{Environment.NewLine}{Environment.NewLine}{filename}{Environment.NewLine}{Environment.NewLine}Overwrite it?",
                                "File already exists",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            ) = DialogResult.Yes Then
                                    proceed = True
                                    messageToReturn = $"{msgToAsk} ""{filename}"" copied over (different existing file, policy: Ask)"
                                Else
                                    proceed = False
                                    messageToReturn = $"{msgToAsk} ""{filename}"" skipped by user (different file exists, policy: Ask)"
                                End If
                            End Using
                    End Select
                End If
            Else
                proceed = True
                messageToReturn = $"{msgToAsk} ""{filename}"" copied"
            End If
        End If

        If proceed Then
            Dim destWasReadOnly As Boolean = False
            Dim originalAttrs As FileAttributes = CType(0, FileAttributes)

            Try
                ' If destination exists and is ReadOnly, clear that flag temporarily
                If File.Exists(fullDestFilename) Then
                    originalAttrs = File.GetAttributes(fullDestFilename)
                    If (originalAttrs And FileAttributes.ReadOnly) <> 0 Then
                        destWasReadOnly = True
                        File.SetAttributes(fullDestFilename, originalAttrs And Not FileAttributes.ReadOnly)
                    End If
                End If

                ' Copy from the selected source (Whitelist or package)
                File.Copy(sourceToCopy, fullDestFilename, True)

                ' Optional: restore ReadOnly if it was set before
                If destWasReadOnly AndAlso File.Exists(fullDestFilename) Then
                    Dim newAttrs = File.GetAttributes(fullDestFilename)
                    File.SetAttributes(fullDestFilename, newAttrs Or FileAttributes.ReadOnly)
                End If

                AppendMsfsRunningNoteForWpr(messageToReturn, filename, isDelete:=False)

            Catch ex As UnauthorizedAccessException
                messageToReturn = $"{msgToAsk} ""{filename}"" failed to copy (access denied): {ex.Message}"
                If destWasReadOnly AndAlso File.Exists(fullDestFilename) Then
                    Try : File.SetAttributes(fullDestFilename, originalAttrs) : Catch : End Try
                End If

            Catch ex As Exception
                messageToReturn = $"{msgToAsk} ""{filename}"" failed to copy: {ex.Message}"
                If destWasReadOnly AndAlso File.Exists(fullDestFilename) Then
                    Try : File.SetAttributes(fullDestFilename, originalAttrs) : Catch : End Try
                End If
            End Try
        End If

        Return messageToReturn
    End Function

    Private Function DeleteFile(filename As String, sourcePath As String, msgToAsk As String, excludeFromCleanup As Boolean) As String
        Dim fullSourceFilename As String
        Dim messageToReturn As String = String.Empty

        If Directory.Exists(sourcePath) Then
            fullSourceFilename = Path.Combine(sourcePath, filename)
            If File.Exists(fullSourceFilename) Then
                Try
                    If Not excludeFromCleanup Then
                        File.Delete(fullSourceFilename)
                        messageToReturn = $"{msgToAsk} ""{filename}"" deleted"
                        AppendMsfsRunningNoteForWpr(messageToReturn, filename, isDelete:=True)
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

    Private Sub CleanupFiles()
        ' 1) Build a list of candidates that *would* be deleted
        Dim candidates As New List(Of CleanupCandidate)

        ' Helper: add a candidate only if it truly exists and is not excluded
        Dim addCand = Sub(fileName As String, folder As String, label As String, shortLabel As String, excluded As Boolean)
                          If String.IsNullOrWhiteSpace(fileName) Then Exit Sub
                          If excluded Then Exit Sub
                          If Not Directory.Exists(folder) Then Exit Sub

                          Dim full As String = Path.Combine(folder, fileName)
                          If Not File.Exists(full) Then Exit Sub

                          Dim whitelistDir As String = Path.Combine(Application.StartupPath, "Whitelist")

                          ' ----- Whitelist match? -> start unchecked -----
                          Dim defaultChecked As Boolean = True
                          If Directory.Exists(whitelistDir) Then
                              Dim wlPath As String = Path.Combine(whitelistDir, fileName)
                              If File.Exists(wlPath) Then
                                  ' Uses your XML-aware comparer for PLN/WPR
                                  If SupportingFeatures.FilesAreEquivalent(full, wlPath) Then
                                      defaultChecked = False
                                  End If
                              End If
                          End If

                          Dim disp As String = $"{shortLabel} : {fileName}"
                          Dim isWL As Boolean = Not defaultChecked
                          If Not defaultChecked Then
                              disp &= " (whitelist)"
                          End If

                          candidates.Add(New CleanupCandidate With {
                          .Display = disp,
                          .FileName = fileName,
                          .SourcePath = folder,
                          .Label = label,
                          .DefaultChecked = defaultChecked,
                          .IsWhitelistProtected = isWL
                      })
                      End Sub

        If Settings.SessionSettings.Is2020Installed Then
            addCand(Path.GetFileName(_allDPHData.FlightPlanFilename),
                Settings.SessionSettings.MSFS2020FlightPlansFolder,
                "Flight Plan for MSFS 2020",
                "PLN 2020",
                Settings.SessionSettings.Exclude2020FlightPlanFromCleanup)

            addCand(Path.GetFileName(_allDPHData.WeatherFilename),
                Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                "Weather Preset for MSFS 2020",
                "WPR 2020",
                Settings.SessionSettings.Exclude2020WeatherFileFromCleanup)
        End If

        If Settings.SessionSettings.Is2024Installed Then
            addCand(Path.GetFileName(_allDPHData.FlightPlanFilename),
                Settings.SessionSettings.MSFS2024FlightPlansFolder,
                "Flight Plan for MSFS 2024",
                "PLN 2024",
                Settings.SessionSettings.Exclude2024FlightPlanFromCleanup)

            If Settings.SessionSettings.EnableEFBFlightPlanCreation Then
                Dim nameOnly = Path.GetFileName(_allDPHData.FlightPlanFilename)
                Dim baseName = Path.GetFileNameWithoutExtension(nameOnly)
                Dim ext = Path.GetExtension(nameOnly)
                Dim efbName = If(baseName.EndsWith("_EFB", StringComparison.OrdinalIgnoreCase),
                             baseName & ext,
                             baseName & "_EFB" & ext)

                addCand(efbName,
                    Settings.SessionSettings.MSFS2024FlightPlansFolder,
                    "EFB Flight Plan for MSFS 2024",
                    "EFB 2024",
                    Settings.SessionSettings.Exclude2024FlightPlanFromCleanup)
            End If

            addCand(Path.GetFileName(_allDPHData.WeatherFilename),
                Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                "Weather Preset for MSFS 2024",
                "WPR 2024",
                Settings.SessionSettings.Exclude2024WeatherFileFromCleanup)
        End If

        ' XCSoar (.tsk/.xcm)
        For Each filepath As String In _allDPHData.ExtraFiles
            If String.IsNullOrWhiteSpace(filepath) Then Continue For
            Dim ext = Path.GetExtension(filepath)
            If ext Is Nothing Then Continue For
            If ext.Equals(".tsk", StringComparison.OrdinalIgnoreCase) Then
                addCand(Path.GetFileName(filepath),
                    Settings.SessionSettings.XCSoarTasksFolder,
                    "XCSoar Task",
                    "XCSoar Task",
                    Settings.SessionSettings.ExcludeXCSoarTaskFileFromCleanup)
            ElseIf ext.Equals(".xcm", StringComparison.OrdinalIgnoreCase) Then
                addCand(Path.GetFileName(filepath),
                    Settings.SessionSettings.XCSoarMapsFolder,
                    "XCSoar Map",
                    "XCSoar Map",
                    Settings.SessionSettings.ExcludeXCSoarMapFileFromCleanup)
            End If
        Next

        ' If nothing would actually be deleted, bail early
        If candidates.Count = 0 Then
            Using New Centered_MessageBox()
                MessageBox.Show(Me, "No files eligible for deletion.", "Cleanup", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
            EnableUnpackButton()
            Return
        End If

        ' 2) One dialog to confirm AND show results (it calls DeleteFile internally)
        Using dlg As New CleanupConfirmForm(candidates, AddressOf DeleteFile)
            dlg.ShowDialog(Me)
        End Using

        EnableUnpackButton()
    End Sub

    Private Sub toolStripWSGMap_Click(sender As Object, e As EventArgs) Handles toolStripWSGMap.Click
        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=map")
    End Sub

    Private Sub toolStripWSGHome_Click(sender As Object, e As EventArgs) Handles toolStripWSGHome.Click
        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=home#")
    End Sub

    Private Sub toolStripWSGUploadIGC_Click(sender As Object, e As EventArgs) Handles toolStripWSGUploadIGC.Click

        ' Check that we have the user info on WSG
        If Settings.SessionSettings.WSGUserID = 0 Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"Please log in to WeSimGlide.org first before uploading IGC files!", "Not logged in to WeSimGlide.org", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using
            Exit Sub
        End If

        Dim listOfIGCFiles As List(Of String) = _SF.GetCorrespondingIGCFiles(Nothing, Settings.SessionSettings.NB21IGCFolder)
        If listOfIGCFiles.Count > 0 Then
            'Present the IGCUpload dialog with the list of IGC files
            Dim igcUploadDialog As New IGCFileUpload
            If _allDPHData IsNot Nothing Then
                igcUploadDialog.Display(Me, listOfIGCFiles, _allDPHData.EntrySeqID)
            Else
                igcUploadDialog.Display(Me, listOfIGCFiles, Nothing)
            End If
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"No IGC file found in the NB21 IGC folder ({Settings.SessionSettings.NB21IGCFolder})", "No IGC files found", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
        End If

    End Sub

    Private Function MakeSureWSGListenerIsRunning(isTransient As Boolean) As String
        Dim result As String = String.Empty

        ' Already running?
        Dim procs = Process.GetProcessesByName("WSGListener")
        If procs.Length > 0 Then Return result

        Dim exe = Path.Combine(Application.StartupPath, "WSGListener.exe")
        If Not File.Exists(exe) Then
            Return "WSGListener.exe not found next to DPHX Unpack & Load."
        End If

        Try
            Dim args As String = If(isTransient, "--transient", "")
            Dim psi As New ProcessStartInfo(exe) With {
            .Arguments = args,
            .UseShellExecute = True,
            .CreateNoWindow = True
        }
            Process.Start(psi)

            ' --- SmartScreen / blocked-launch detection ---
            ' Poll for a short time to see if the process truly came up.
            Dim started As Boolean = False
            Dim deadline As Date = Date.UtcNow.AddSeconds(6) ' small grace period

            Do While Date.UtcNow < deadline
                Threading.Thread.Sleep(300)
                If Process.GetProcessesByName("WSGListener").Length > 0 Then
                    If SendCommandToWSG("health") = String.Empty Then
                        started = True
                        Exit Do
                    End If
                End If
            Loop

            If started Then
                _wsgListenerStartedAsTransient = isTransient
                Return result
            End If

            ' If we get here, Windows likely blocked it (SmartScreen on first run).
            Dim msg As String = $"Windows SmartScreen probably blocked the first launch of the WSGListener app.{Environment.NewLine}Please start it once manually so Windows shows the SmartScreen prompt.{Environment.NewLine}{Environment.NewLine}• In the next window, double-click WSGListener.exe{Environment.NewLine}• When SmartScreen appears, choose ""More info"" → ""Run anyway"""

            Dim choice = MessageBox.Show(
                msg,
                "WSGListener needs first-run approval",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            )

            ' Help the user by opening Explorer with the file selected.
            Try
                Process.Start("explorer.exe", "/select,""" & exe & """")
            Catch
                ' non-fatal
            End Try

            ' Wait a bit longer for the user to do the manual launch.
            Dim retryStarted As Boolean = False
            Dim retryDeadline As Date = Date.UtcNow.AddSeconds(30)
            Do While Date.UtcNow < retryDeadline
                Threading.Thread.Sleep(300)
                If Process.GetProcessesByName("WSGListener").Length > 0 Then
                    ' confirm it's answering
                    If SendCommandToWSG("health") = String.Empty Then
                        retryStarted = True
                        Exit Do
                    End If
                End If
            Loop

            If retryStarted Then
                _wsgListenerStartedAsTransient = isTransient
                Return result
            Else
                result = "WSGListener still not detected. Please run it once manually, authorize via SmartScreen, then try again."
            End If

        Catch ex As Exception
            result = $"Could not start WSGListener (transient={isTransient}): {ex.Message}"
        End Try

        Return result
    End Function

    Private Function SendCommandToWSG(endpoint As String, Optional query As String = "", Optional oldPortToUse As Integer = 0) As String
        Dim port As Integer
        If oldPortToUse > 0 Then
            port = oldPortToUse
        ElseIf Not Integer.TryParse(Settings.SessionSettings.LocalWebServerPort, port) Then
            Return "Invalid WSGListener port in settings."
        End If

        Dim url = $"http://localhost:{port}/{endpoint}"
        If Not String.IsNullOrEmpty(query) Then url &= "?" & query  ' ensure caller URL-encodes values

        Try
            Dim req = DirectCast(WebRequest.Create(url), HttpWebRequest)
            req.Method = "GET"
            req.UserAgent = "DPHX Unpack & Load"
            req.Timeout = 3000 ' ms

            Using resp = DirectCast(req.GetResponse(), HttpWebResponse)
                Using sr As New IO.StreamReader(resp.GetResponseStream())
                    Dim body = sr.ReadToEnd()
                    ' Return empty on success to keep your current semantics, OR return body if you prefer:
                    ' Return body
                    Return String.Empty
                End Using
            End Using

        Catch wex As WebException
            Dim statusText As String = wex.Message
            Dim body As String = ""
            Dim code As Integer = -1

            Dim httpResp = TryCast(wex.Response, HttpWebResponse)
            If httpResp IsNot Nothing Then
                code = CInt(httpResp.StatusCode)
                Try
                    Using sr As New IO.StreamReader(httpResp.GetResponseStream())
                        body = sr.ReadToEnd()
                    End Using
                Catch
                End Try
            End If

            Return $"Failed to send '{endpoint}' to WSGListener:{vbCrLf}HTTP {(If(code <> -1, code.ToString(), "n/a"))} {statusText}{If(String.IsNullOrEmpty(body), "", vbCrLf & body)}"

        Catch ex As Exception
            Return $"Failed to send '{endpoint}' to WSGListener:{vbCrLf}{ex.Message}"
        End Try
    End Function

    Private Sub ReloadSettings()
        Settings.SessionSettings.Load()
    End Sub

#End Region

End Class
