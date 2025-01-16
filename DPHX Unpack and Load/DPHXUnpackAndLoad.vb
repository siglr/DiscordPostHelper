Imports System.Configuration
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary
Imports SIGLR.SoaringTools.ImageViewer

Public Class DPHXUnpackAndLoad

#Region "Constants and other global variables"

    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"

    Private ReadOnly _SF As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _currentFile As String = String.Empty
    Private _abortingFirstRun As Boolean = False
    Private _allDPHData As AllData
    Private _filesToUnpack2020 As New Dictionary(Of String, String)
    Private _filesCurrentlyUnpacked2020 As New Dictionary(Of String, String)
    Private _filesToUnpack2024 As New Dictionary(Of String, String)
    Private _filesCurrentlyUnpacked2024 As New Dictionary(Of String, String)
    Private _currentNewsKeyPublished As New Dictionary(Of String, Date)
    Private _groupEventNewsEntries As New Dictionary(Of String, NewsEntry)
    Private _showingPrompt As Boolean = False
    Private _lastLoadSuccess As Boolean = False
    Private WithEvents _DPHXWS As DPHXLocalWS

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
        chkNewsRetrieval.Left = 3
        newsSplitContainer.SplitterDistance = 750

        If Not _SF.CheckRequiredNetFrameworkVersion Then
            MessageBox.Show("This application requires Microsoft .NET Framework 4.8 or later to be present.", "Installation does not meet requirement", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _abortingFirstRun = True
            Application.Exit()
            Exit Sub
        End If

        Dim firstRun As Boolean = Not Settings.SessionSettings.Load()
        SetFormCaption(_currentFile)

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
            CheckForNewVersion()

            lbl2020AllFilesStatus.Text = String.Empty
            lbl2024AllFilesStatus.Text = String.Empty

            SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder)

            Dim doUnpack As Boolean = False
            If My.Application.CommandLineArgs.Count > 0 Then
                ' Open the file passed as an argument
                _currentFile = My.Application.CommandLineArgs(0)
                doUnpack = True
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

            DatabaseUpdate.CheckAndUpdateDatabase()

            RetrieveNewsList_Tick(sender, e)

            ' Start the server
            _DPHXWS = New DPHXLocalWS(Settings.SessionSettings.LocalWebServerPort)
            _DPHXWS.Start()

        End If

    End Sub

    Private Sub DPHXUnpackAndLoad_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not _abortingFirstRun Then
            ctrlBriefing.Closing()
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
            If _DPHXWS IsNot Nothing Then
                _DPHXWS.Stop()
                _DPHXWS.Dispose()
            End If
            Settings.SessionSettings.MainFormSize = Me.Size.ToString()
            Settings.SessionSettings.MainFormLocation = Me.Location.ToString()
            Settings.SessionSettings.Save()
        End If

    End Sub

    Private Sub DPHXUnpackAndLoad_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ctrlBriefing.AdjustRTBoxControls()
    End Sub

    Private Sub chkNewsRetrieval_CheckedChanged(sender As Object, e As EventArgs) Handles chkNewsRetrieval.CheckedChanged
        RetrieveNewsList.Enabled = Not chkNewsRetrieval.Checked
        If Not RetrieveNewsList.Enabled Then
            RetrieveNewsList.Stop()
        End If
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles toolStripSettings.Click

        OpenSettingsWindow()

        msfs2020ToolStrip.Visible = Settings.SessionSettings.Is2020Installed
        msfs2024ToolStrip.Visible = Settings.SessionSettings.Is2024Installed

        'Recheck files
        If toolStripUnpack.Enabled Then
            EnableUnpackButton()
        End If

        'Restart local web server
        _DPHXWS.Stop()
        _DPHXWS.Port = Settings.SessionSettings.LocalWebServerPort
        _DPHXWS.Start()

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

        If warningMSFSRunningToolStrip.Visible Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be copied but weather preset will not be available in MSFS until it is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    UnpackFiles()
                End If
            End Using
        Else
            UnpackFiles()
        End If
    End Sub

    Private Sub btnCleanup_Click(sender As Object, e As EventArgs) Handles toolStripCleanup.Click

        If warningMSFSRunningToolStrip.Visible Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be deleted but weather preset will remain available until MSFS is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    CleanupFiles()
                End If
            End Using
        Else
            CleanupFiles()
        End If

    End Sub

    Private Sub btnFileBrowser_Click(sender As Object, e As EventArgs) Handles toolStripFileBrowser.Click

        If warningMSFSRunningToolStrip.Visible Then
            Using New Centered_MessageBox(Me)
                If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be deleted but weather preset will remain available until MSFS is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    CleaningTool.ShowDialog(Me)
                End If
            End Using
        Else
            CleaningTool.ShowDialog(Me)
        End If

        'Recheck files
        If toolStripUnpack.Enabled Then
            EnableUnpackButton()
        End If

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
        Dim processList As Process() = Process.GetProcessesByName("FlightSimulator")
        If processList.Length > 0 Then
            RetrieveNewsList.Enabled = False
            warningMSFSRunningToolStrip.Visible = True
        Else
            RetrieveNewsList.Enabled = Not chkNewsRetrieval.Checked
            warningMSFSRunningToolStrip.Visible = False
        End If

    End Sub

    Private Sub RetrieveNewsList_Tick(sender As Object, e As EventArgs) Handles RetrieveNewsList.Tick

        RetrieveNewsList.Enabled = False

        FetchNewsEntries()

        If Not chkNewsRetrieval.Enabled Then
            RetrieveNewsList.Enabled = True
        Else
            RetrieveNewsList.Enabled = False
        End If

        If Not _showingPrompt Then
            'Check if there is an group event that is within 2 hours and user has not provided an answer
            Dim userParticipatingInEvent As Boolean = False
            For Each groupEventNews As NewsEntry In _groupEventNewsEntries.Values
                If groupEventNews.IsWithin2HoursOfEvent AndAlso Not groupEventNews.UserHasAnswered Then
                    'Ask user!
                    Dim msgPrompt As String = $"The following group event is starting soon. Will you be participating?{Environment.NewLine}{Environment.NewLine}{groupEventNews.Title}{Environment.NewLine}{groupEventNews.Subtitle}{Environment.NewLine}{_SF.GetFullEventDateTimeInLocal(groupEventNews.EventDate, groupEventNews.EventDate, True).ToString}"
                    _showingPrompt = True
                    Using New Centered_MessageBox()
                        If MessageBox.Show(msgPrompt, "Group Event Starting Soon", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbYes Then
                            'User will be participating
                            userParticipatingInEvent = True
                        Else
                            'User will not be participating - do nothing
                        End If
                        groupEventNews.UserHasAnswered = True
                    End Using
                    _showingPrompt = False
                End If
                If userParticipatingInEvent Then
                    SupportingFeatures.LaunchDiscordURL(groupEventNews.URLToGo)
                    If groupEventNews.EntrySeqID > 0 Then
                        DownloadAndOpenTaskUsingNewsEntry(groupEventNews)
                    Else
                        _showingPrompt = True
                        Using New Centered_MessageBox()
                            MessageBox.Show("The task has not yet been published for the event!", "Group Event Starting Soon", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End Using
                        _showingPrompt = False
                    End If
                    Exit For
                End If

            Next
        End If

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

    Private Sub toolStripDiscordTaskLibrary_Click(sender As Object, e As EventArgs) Handles toolStripDiscordTaskLibrary.Click

        OpenTaskLibraryBrowser()

    End Sub

    Private Sub _DPHXWS_RequestReceived(sender As Object, e As RequestReceivedEventArgs) _
    Handles _DPHXWS.RequestReceived

        Dim context = e.Context
        Dim request = context.Request

        ' 1. Check if it's a preflight OPTIONS request
        If request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase) Then
            ' Send an appropriate CORS response if needed
            context.Response.AddHeader("Access-Control-Allow-Origin", "*")  ' or specific domain
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST,OPTIONS")
            context.Response.StatusCode = 204 ' No Content
            context.Response.OutputStream.Close()
            Return
        End If

        ' 2. If it's a GET, handle your logic
        If request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) Then
            Dim taskId As String = request.QueryString("taskID")
            Dim taskTitle As String = request.QueryString("title")
            Dim fromEventTab As Boolean = False
            If request.QueryString("source") IsNot Nothing AndAlso request.QueryString("source") <> String.Empty Then
                fromEventTab = (request.QueryString("source") = "event")
            End If

            Console.WriteLine($"Incoming GET request for taskId = {taskId}, title = {taskTitle}")

            Dim DownloadedFilePath As String = SupportingFeatures.DownloadTaskFile(taskId, taskTitle, Settings.SessionSettings.PackagesFolder)
            If DownloadedFilePath <> String.Empty Then
                _DPHXWS.SendResponse(context, $"Task received: {taskId}, Title: {taskTitle}", 200)
                ' Because LoadDPHXPackage likely does UI-related work,
                ' we must call it via Invoke on the main form:
                Me.Invoke(Sub()
                              LoadDPHXPackage(DownloadedFilePath)
                              If Settings.SessionSettings.AutoUnpack AndAlso _currentFile <> String.Empty AndAlso _lastLoadSuccess Then
                                  UnpackFiles(fromEventTab)
                              End If
                          End Sub)
            End If
        Else
            ' Optional: handle other methods or return 405 (Method Not Allowed)
            context.Response.StatusCode = 405
            context.Response.OutputStream.Close()
        End If

    End Sub

#End Region

#Region "Subs and functions"

    Private Sub OpenTaskLibraryBrowser(Optional entrySeqID As Integer = 0)

        RetrieveNewsList.Enabled = False

        Using taskBrowserForm As New TaskBrowser()
            taskBrowserForm.OpenWithEntrySeqID = entrySeqID
            taskBrowserForm.ShowDialog(Me)

            Dim selectedFile As String = taskBrowserForm.DownloadedFilePath
            If selectedFile <> String.Empty Then
                LoadDPHXPackage(selectedFile)
                If Settings.SessionSettings.AutoUnpack AndAlso _lastLoadSuccess Then
                    UnpackFiles()
                End If
            End If
        End Using

        RetrieveNewsList.Enabled = Not chkNewsRetrieval.Checked

    End Sub

    Private Sub SetFormCaption(filename As String)

        If filename = String.Empty Then
            filename = "No DPHX package loaded"
        End If

        'Add version to form title
        Me.Text = $"DPHX Unpack and Load v{Me.GetType.Assembly.GetName.Version} - {filename}"

    End Sub

    Private Sub CheckForNewVersion()
        Dim myVersionInfo As VersionInfo = _SF.GetVersionInfo()
        Dim message As String = String.Empty

        If myVersionInfo IsNot Nothing Then
            If _SF.FormatVersionNumber(myVersionInfo.CurrentLatestVersion) > _SF.FormatVersionNumber(Me.GetType.Assembly.GetName.Version.ToString) Then
                'New version available
                If _SF.ShowVersionForm(myVersionInfo, Me.GetType.Assembly.GetName.Version.ToString) = DialogResult.Yes Then
                    'update
                    'Download the file
                    If _SF.DownloadLatestUpdate(myVersionInfo.CurrentLatestVersion, message) Then
                        Application.Exit()
                    Else
                        'Show error updating
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show(Me, $"An error occured during the update process at this step:{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}The update did not complete.", "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using
                    End If
                End If
            End If
        End If

    End Sub

    Private Function OpenSettingsWindow() As DialogResult
        Dim formSettings As New Settings

        Return formSettings.ShowDialog(Me)

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

            ctrlBriefing.GenerateBriefing(_SF,
                                          _allDPHData,
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename)),
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
            If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2020FlightPlansFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename))) Then
                _filesCurrentlyUnpacked2020.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            End If

            'Weather file
            _filesToUnpack2020.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename))) Then
                _filesCurrentlyUnpacked2020.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            End If
        End If
        If Settings.SessionSettings.Is2024Installed Then
            'Flight plan
            _filesToUnpack2024.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFS2024FlightPlansFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename))) Then
                _filesCurrentlyUnpacked2024.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
            End If

            'Weather file
            _filesToUnpack2024.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
            If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
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
                    If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
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
                    If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
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

        Dim sb As New StringBuilder

        sb.AppendLine("Unpacking Results:")
        sb.AppendLine()

        If Settings.SessionSettings.Is2020Installed Then
            'Flight plan
            sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2020FlightPlansFolder,
                 "Flight Plan for MSFS 2020"))
            sb.AppendLine()

            'Weather file
            sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                 "Weather Preset for MSFS 2020"))

            sb.AppendLine()
        End If
        If Settings.SessionSettings.Is2024Installed Then
            'Flight plan
            sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2024FlightPlansFolder,
                 "Flight Plan for MSFS 2024"))
            sb.AppendLine()

            'Weather file
            sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                 "Weather Preset for MSFS 2024"))

            sb.AppendLine()
        End If

        'Look in the other files for xcsoar files
        For Each filepath As String In _allDPHData.ExtraFiles
            If Path.GetExtension(filepath) = ".tsk" Then
                'XCSoar task
                sb.AppendLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarTasksFolder,
                                    "XCSoar Task"))
                sb.AppendLine()
            End If
            If Path.GetExtension(filepath) = ".xcm" Then
                'XCSoar map
                sb.AppendLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarMapsFolder,
                                    "XCSoar Map"))
                sb.AppendLine()
            End If
        Next

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
                                    sb.AppendLine("NB21 Logger successfully started.")
                                    Exit For
                                End If
                                Thread.Sleep(500) ' Check every 500ms
                            Next
                        End If

                        If Not NB21LoggerRunning Then
                            sb.AppendLine("NB21 Logger did not become ready within the timeout period.")
                        End If
                    Catch ex As Exception
                        sb.AppendLine($"An error occurred trying to launch NB21 Logger: {ex.Message}")
                    End Try
                Else
                    sb.AppendLine($"The NB21 Logger's executable file was not found in {Settings.SessionSettings.NB21EXEFolder}")
                End If
            Else
                sb.AppendLine("NB21 Logger is already running.")
            End If

            If NB21LoggerRunning Then
                'Feed the PLN file to the logger
                SendPLNFileToNB21Logger(sb, Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)))
            End If
            sb.AppendLine()
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
                                    sb.AppendLine("Tracker successfully started.")
                                    Exit For
                                End If
                                Thread.Sleep(500) ' Check every 500ms
                            Next
                        End If

                        If Not TrackerRunning Then
                            sb.AppendLine("Tracker did not become ready within the timeout period.")
                        End If

                    Catch ex As Exception
                        sb.AppendLine($"An error occurred trying to launch the Tracker: {ex.Message}")
                    End Try
                Else
                    sb.AppendLine($"The Tracker's executable file was not found in {Settings.SessionSettings.TrackerEXEFolder}")
                End If
            Else
                sb.AppendLine("Tracker is already running.")
            End If

            If TrackerRunning Then
                'Feed the data to the tracker
                Dim groupToUse As String = String.Empty
                If _allDPHData.IsFutureEvent AndAlso fromEvent Then
                    groupToUse = _allDPHData.TrackerGroup
                End If
                SendDataToTracker(sb,
                                  groupToUse,
                                  Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                  Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename)),
                                  _allDPHData.URLGroupEventPost
                                 )
            End If
            sb.AppendLine()
        End If

        Using New Centered_MessageBox(Me)
            MessageBox.Show(sb.ToString, "Unpacking results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using

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

    Private Sub SendPLNFileToNB21Logger(sb As StringBuilder, plnfilePath As String)

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
                    sb.AppendLine("PLN file successfully sent to NB21 Logger.")
                Else
                    sb.AppendLine($"Failed to send PLN file to NB21 Logger. HTTP Status: {response.StatusCode}")
                End If
            End Using
        Catch ex As Exception
            sb.AppendLine($"An error occurred while sending the PLN file: {ex.Message}")
        End Try

    End Sub

    ' Function to send a POST request
    Private Function SendPostRequest(apiUrl As String, jsonPayload As String) As HttpResponseMessage
        Using client As New HttpClient()
            Dim content As New StringContent(jsonPayload, Encoding.UTF8, "application/json")
            Return client.PostAsync(apiUrl, content).Result
        End Using
    End Function

    Private Sub SendDataToTracker(sb As StringBuilder, trackerGroup As String, plnfilePath As String, wprfilePath As String, infoURL As String)
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

            ' Perform the first call
            Dim response = SendPostRequest(apiUrl, jsonPayload)

            If response.IsSuccessStatusCode Then
                sb.AppendLine("First call to SSC Tracker was successful.")
            Else
                sb.AppendLine($"Failed to communicate with Tracker. HTTP Status: {response.StatusCode}")
                Exit Sub ' Stop if the first call fails
            End If

            If trackerGroup = String.Empty Then
                ' No group set, we don't need the second call
                Exit Sub
            End If

            ' Wait for 5 seconds before the second call
            Threading.Thread.Sleep(5000)

            ' Perform the second call
            response = SendPostRequest(apiUrl, jsonPayload)

            If response.IsSuccessStatusCode Then
                sb.AppendLine("Second call to SSC Tracker was successful.")
            Else
                sb.AppendLine($"Failed to communicate with Tracker on the second call. HTTP Status: {response.StatusCode}")
            End If

        Catch ex As Exception
            sb.AppendLine($"An error occurred while communicating with Tracker: {ex.Message}")
        End Try
    End Sub

    Private Function CopyFile(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String
        Dim fullSourceFilename As String
        Dim fullDestFilename As String
        Dim proceed As Boolean = False
        Dim messageToReturn As String = String.Empty

        If Directory.Exists(destPath) Then
            fullSourceFilename = Path.Combine(sourcePath, filename)
            fullDestFilename = Path.Combine(destPath, filename)
            If File.Exists(fullDestFilename) Then
                'Check what to do
                Select Case Settings.SessionSettings.AutoOverwriteFiles
                    Case AllSettings.AutoOverwriteOptions.AlwaysOverwrite
                        proceed = True
                        messageToReturn = $"{msgToAsk} ""{filename}"" copied over existing one"
                    Case AllSettings.AutoOverwriteOptions.AlwaysSkip
                        proceed = False
                        messageToReturn = $"{msgToAsk} ""{filename}"" skipped - already exists"
                    Case AllSettings.AutoOverwriteOptions.AlwaysAsk
                        Using New Centered_MessageBox(Me)
                            If MessageBox.Show($"The {msgToAsk} file already exists.{Environment.NewLine}{Environment.NewLine}{filename}{Environment.NewLine}{Environment.NewLine}Do you want to overwrite it?", "File already exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                                proceed = True
                                messageToReturn = $"{msgToAsk} ""{filename}"" copied over existing one"
                            Else
                                proceed = False
                                messageToReturn = $"{msgToAsk} ""{filename}"" skipped by user - already exists"
                            End If
                        End Using
                End Select
            Else
                proceed = True
                messageToReturn = $"{msgToAsk} ""{filename}"" copied"
            End If
            If proceed Then
                File.Copy(fullSourceFilename, fullDestFilename, True)
            End If
        Else
            messageToReturn = $"{msgToAsk} ""{filename}"" skipped - destination folder not set or invalid (check settings)"
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
                    Else
                        messageToReturn = $"{msgToAsk} ""{filename}"" excluded from cleanup"
                    End If
                Catch ex As Exception
                    messageToReturn = $"{msgToAsk} ""{filename}"" found but error trying to deleted it:{Environment.NewLine}{ex.Message}"
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

        Dim sb As New StringBuilder

        sb.AppendLine("Cleanup Results:")
        sb.AppendLine()

        If Settings.SessionSettings.Is2020Installed Then
            'Flight plan
            sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 Settings.SessionSettings.MSFS2020FlightPlansFolder,
                 "Flight Plan for MSFS 2020",
                 Settings.SessionSettings.Exclude2020FlightPlanFromCleanup))
            sb.AppendLine()

            'Weather file
            sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                 "Weather Preset for MSFS 2020",
                 Settings.SessionSettings.Exclude2020WeatherFileFromCleanup))
            sb.AppendLine()

        End If
        If Settings.SessionSettings.Is2024Installed Then
            'Flight plan
            sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 Settings.SessionSettings.MSFS2024FlightPlansFolder,
                 "Flight Plan for MSFS 2024",
                 Settings.SessionSettings.Exclude2024FlightPlanFromCleanup))
            sb.AppendLine()

            'Weather file
            sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                 "Weather Preset for MSFS 2024",
                 Settings.SessionSettings.Exclude2024WeatherFileFromCleanup))
            sb.AppendLine()

        End If

        'Look in the other files for xcsoar file
        For Each filepath As String In _allDPHData.ExtraFiles
            If Path.GetExtension(filepath) = ".tsk" Then
                'XCSoar task
                sb.AppendLine(DeleteFile(Path.GetFileName(filepath),
                                    Settings.SessionSettings.XCSoarTasksFolder,
                                    "XCSoar Task",
                                    Settings.SessionSettings.ExcludeXCSoarTaskFileFromCleanup))
            End If
            If Path.GetExtension(filepath) = ".xcm" Then
                'XCSoar map
                sb.AppendLine(DeleteFile(Path.GetFileName(filepath),
                                    Settings.SessionSettings.XCSoarMapsFolder,
                                    "XCSoar Map",
                                    Settings.SessionSettings.ExcludeXCSoarMapFileFromCleanup))
            End If
        Next

        Using New Centered_MessageBox()
            MessageBox.Show(Me, sb.ToString, "Cleanup results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
        EnableUnpackButton()

    End Sub

    Public Sub FetchNewsEntries()
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveNews.php"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim newsEntries As List(Of NewsEntry) = ConvertJsonToDataTable(jsonResponse)

                    ' Determine if there are new or updated entries
                    Dim newOrUpdatedEntries As Boolean = False
                    Dim fetchedKeys As New Dictionary(Of String, DateTime)

                    ' Populate fetchedKeys with new entries' keys and Published dates
                    For Each newsEntry In newsEntries
                        Dim publishedDate As DateTime = DateTime.Parse(newsEntry.Published)
                        fetchedKeys.Add(newsEntry.Key, publishedDate)

                        ' Check if the entry is new or updated
                        If Not _currentNewsKeyPublished.ContainsKey(newsEntry.Key) Then
                            ' New entry
                            newOrUpdatedEntries = True
                            If newsEntry.NewsType + 1 = TaskEventNews.NewsTypeEnum.Event Then
                                _groupEventNewsEntries.Add(newsEntry.Key, newsEntry)
                            End If
                        ElseIf _currentNewsKeyPublished(newsEntry.Key) < publishedDate Then
                            ' Updated entry
                            newOrUpdatedEntries = True
                            If newsEntry.NewsType + 1 = TaskEventNews.NewsTypeEnum.Event Then
                                If _groupEventNewsEntries.ContainsKey(newsEntry.Key) Then
                                    newsEntry.UserHasAnswered = _groupEventNewsEntries(newsEntry.Key).UserHasAnswered
                                    _groupEventNewsEntries(newsEntry.Key) = newsEntry
                                End If
                            End If
                        End If
                    Next

                    ' Determine if there are deletions by comparing keys
                    Dim deletions As Boolean = _currentNewsKeyPublished.Keys.Except(fetchedKeys.Keys).Any()
                    For Each deletedItem As String In _currentNewsKeyPublished.Keys.Except(fetchedKeys.Keys)
                        'Delete the news entry from the global dictionary
                        If _groupEventNewsEntries.ContainsKey(deletedItem) Then
                            If _groupEventNewsEntries(deletedItem).NewsType = TaskEventNews.NewsTypeEnum.Event Then
                                _groupEventNewsEntries.Remove(deletedItem)
                            End If
                        End If
                    Next

                    ' If there are new/updated entries or deletions, rebuild the news panel
                    If newOrUpdatedEntries Or deletions Then
                        _currentNewsKeyPublished.Clear()

                        ' Clean up existing news panels
                        For Each newsPanel As TaskEventNews In flowNewsPanel.Controls
                            RemoveHandler newsPanel.NewsClicked, AddressOf NewsPanelClicked
                            newsPanel.Dispose()
                            newsPanel = Nothing
                        Next
                        flowNewsPanel.Controls.Clear()

                        ' Add new news panels
                        For Each newsEntry In newsEntries
                            Dim newsPanel = New TaskEventNews(newsEntry)
                            If newsEntry.NewsType + 1 <> TaskEventNews.NewsTypeEnum.News Then
                                newsPanel.ContextMenuStrip = contextGroupEventNews
                            End If
                            AddHandler newsPanel.NewsClicked, AddressOf NewsPanelClicked
                            _currentNewsKeyPublished.Add(newsEntry.Key, DateTime.Parse(newsEntry.Published))
                            flowNewsPanel.Controls.Add(newsPanel)
                        Next

                        ' Update button color
                        If newOrUpdatedEntries Then
                            If newsSplitContainer.Panel2Collapsed Then
                                btnNewsPanelCollapse.BackColor = Color.Yellow
                            End If
                        End If
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception
            ' Log the error or display a message
            Throw
        End Try
    End Sub

    Private Sub NewsPanelClicked(sender As Object, e As EventArgs)

        Dim theNewsEntry As TaskEventNews = CType(sender, TaskEventNews)

        Select Case theNewsEntry.NewsType
            Case TaskEventNews.NewsTypeEnum.Task
                'Open the library with a specific task
                OpenTaskLibraryBrowser(theNewsEntry.TaskEntrySeqID)

            Case TaskEventNews.NewsTypeEnum.Event
                'Open the Discord event
                SupportingFeatures.LaunchDiscordURL(theNewsEntry.URLToGo)
                'Open the library with the task
                If theNewsEntry.TaskEntrySeqID > 0 Then
                    OpenTaskLibraryBrowser(theNewsEntry.TaskEntrySeqID)
                Else
                    Using New Centered_MessageBox()
                        MessageBox.Show(Me, "The associated task has not yet been published! Stay tuned!", "Task for group event", vbOKOnly, vbInformation)
                    End Using
                End If

            Case TaskEventNews.NewsTypeEnum.News
                'Launch URL
                SupportingFeatures.LaunchDiscordURL(theNewsEntry.URLToGo)

        End Select

    End Sub

    Private Function ConvertJsonToDataTable(jsonResponse As String) As List(Of NewsEntry)

        Dim newsEntries As New List(Of NewsEntry)

        Dim jsonResponseObject = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonResponse)
        If jsonResponseObject("status").ToString() = "success" Then
            newsEntries = JsonConvert.DeserializeObject(Of List(Of NewsEntry))(jsonResponseObject("data").ToString())

        Else
            Throw New Exception(jsonResponseObject("message").ToString())
        End If

        Return newsEntries
    End Function

    Private Sub btnNewsPanelCollapse_Click(sender As Object, e As EventArgs) Handles btnNewsPanelCollapse.Click
        newsSplitContainer.Panel2Collapsed = Not newsSplitContainer.Panel2Collapsed
        btnNewsPanelCollapse.Text = If(newsSplitContainer.Panel2Collapsed, "<", ">")

        If Not newsSplitContainer.Panel2Collapsed Then
            btnNewsPanelCollapse.BackColor = SystemColors.Control
        End If

    End Sub

    Private Sub contextGroupEventNews_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles contextGroupEventNews.Opening

        Dim newsPanel As TaskEventNews = contextGroupEventNews.SourceControl

        Select Case newsPanel.NewsType
            Case TaskEventNews.NewsTypeEnum.Task
                ctxtNewsDownloadAndOpenTask.Visible = True
                ctxtNewsViewTaskInLibrary.Visible = True
                ctxtNewsDownloadAndOpenTaskAndVisitGroupEvent.Visible = False
                ctxtNewsViewTaskInLibraryVisitGroupEvent.Visible = False
                ctxtNewsVisitGroupEvent.Visible = False
            Case TaskEventNews.NewsTypeEnum.Event
                If newsPanel.TaskEntrySeqID > 0 Then
                    ctxtNewsDownloadAndOpenTask.Visible = True
                    ctxtNewsViewTaskInLibrary.Visible = True
                    If newsPanel.URLToGo <> String.Empty Then
                        ctxtNewsDownloadAndOpenTaskAndVisitGroupEvent.Visible = True
                        ctxtNewsViewTaskInLibraryVisitGroupEvent.Visible = True
                        ctxtNewsVisitGroupEvent.Visible = True
                    Else
                        ctxtNewsDownloadAndOpenTaskAndVisitGroupEvent.Visible = False
                        ctxtNewsViewTaskInLibraryVisitGroupEvent.Visible = False
                        ctxtNewsVisitGroupEvent.Visible = False
                    End If
                Else
                    ctxtNewsDownloadAndOpenTask.Visible = False
                    ctxtNewsViewTaskInLibrary.Visible = False
                    ctxtNewsDownloadAndOpenTaskAndVisitGroupEvent.Visible = False
                    ctxtNewsViewTaskInLibraryVisitGroupEvent.Visible = False
                    If newsPanel.URLToGo <> String.Empty Then
                        ctxtNewsVisitGroupEvent.Visible = True
                    Else
                        ctxtNewsVisitGroupEvent.Visible = False
                        e.Cancel = True
                    End If
                End If
        End Select

    End Sub

    Private Sub ctxtNewsViewTaskInLibrary_Click(sender As Object, e As EventArgs) Handles ctxtNewsViewTaskInLibrary.Click

        Dim theNewsEntry As TaskEventNews = contextGroupEventNews.SourceControl
        OpenTaskLibraryBrowser(theNewsEntry.TaskEntrySeqID)

    End Sub

    Private Sub ctxtNewsDownloadAndOpenTask_Click(sender As Object, e As EventArgs) Handles ctxtNewsDownloadAndOpenTask.Click

        Dim theNewsEntry As TaskEventNews = contextGroupEventNews.SourceControl
        DownloadAndOpenTaskUsingNewsEntry(_groupEventNewsEntries(theNewsEntry.Key))

    End Sub

    Private Sub ctxtNewsVisitGroupEvent_Click(sender As Object, e As EventArgs) Handles ctxtNewsVisitGroupEvent.Click

        Dim theNewsEntry As TaskEventNews = contextGroupEventNews.SourceControl
        SupportingFeatures.LaunchDiscordURL(theNewsEntry.URLToGo)

    End Sub

    Private Sub ctxtNewsDownloadAndOpenTaskAndVisitGroupEvent_Click(sender As Object, e As EventArgs) Handles ctxtNewsDownloadAndOpenTaskAndVisitGroupEvent.Click

        Dim theNewsEntry As TaskEventNews = contextGroupEventNews.SourceControl
        SupportingFeatures.LaunchDiscordURL(theNewsEntry.URLToGo)
        DownloadAndOpenTaskUsingNewsEntry(_groupEventNewsEntries(theNewsEntry.Key))

    End Sub

    Private Sub ctxtNewsViewTaskInLibraryVisitGroupEvent_Click(sender As Object, e As EventArgs) Handles ctxtNewsViewTaskInLibraryVisitGroupEvent.Click

        Dim theNewsEntry As TaskEventNews = contextGroupEventNews.SourceControl
        SupportingFeatures.LaunchDiscordURL(theNewsEntry.URLToGo)
        OpenTaskLibraryBrowser(theNewsEntry.TaskEntrySeqID)

    End Sub

    Private Function FetchTaskIDUsingEntrySeqID(entrySeqID As String) As String
        Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}FindTaskUsingEntrySeqID.php?EntrySeqID={entrySeqID}"
        Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
        request.Method = "GET"

        Try
            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()

                    ' Parse the JSON response to extract the TaskID
                    Dim json As JObject = JObject.Parse(jsonResponse)

                    ' Check if the status is "success" and return the TaskID, else return "0"
                    If json("status").ToString() = "success" Then
                        Return json("taskDetails")("TaskID").ToString()
                    Else
                        ' Return Empty string if no task was found
                        Return String.Empty
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Handle the exception (you could log this or handle it differently based on your needs)
            Return String.Empty ' Return Empty string if an exception occurs
        End Try
    End Function

    Private Sub DownloadAndOpenTaskUsingNewsEntry(theNewsEntry As NewsEntry)

        'We need to call the script FindTaskUsingEntrySeqID to get the TaskID
        Dim taskID As String = FetchTaskIDUsingEntrySeqID(theNewsEntry.EntrySeqID)

        If taskID <> String.Empty Then
            Dim selectedFile As String = SupportingFeatures.DownloadTaskFile(taskID, theNewsEntry.Subtitle, Settings.SessionSettings.PackagesFolder)
            If selectedFile <> String.Empty Then
                LoadDPHXPackage(selectedFile)
                If Settings.SessionSettings.AutoUnpack AndAlso _lastLoadSuccess Then
                    UnpackFiles()
                End If
            End If
        End If
    End Sub

#End Region

End Class
