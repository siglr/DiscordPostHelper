Imports System.Diagnostics.Eventing
Imports System.Diagnostics.Eventing.Reader
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Net.Mime
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Xml.Linq
Imports System.Xml.Serialization
Imports CefSharp.DevTools.Page
Imports Microsoft.Win32
Imports NB21_logger
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
    Private _loggerForm As Logger
    Private _manualFallbackFlightPlanPath As String = String.Empty
    Private _manualFallbackWeatherPrimaryPath As String = String.Empty
    Private _manualFallbackWeatherSecondaryPath As String = String.Empty
    Private _manualFallbackSSCPresetName As String = String.Empty
    Private _manualFallbackTrackerGroup As String = String.Empty
    Private _isManualMode As Boolean = False
    Private _upcomingEventCheckAttempted As Boolean = False
    Private _launchedWithDPHXArgument As Boolean = False
    Private _taskFetchRequestedByListener As Boolean = False
    Private _pendingWSGIntegration As Nullable(Of AllSettings.WSGIntegrationOptions) = Nothing
    Private _awaitingUpcomingEventDecision As Boolean = False

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
        UpdateBriefingRenderContext()
        SetFormCaption(_currentFile)

        Rescale()

        RestoreMainFormLocationAndSize()

        Me.Show()
        Me.Refresh()
        If firstRun Then

            Do While True
                Select Case OpenSettingsWindow(True)
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

            SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder, True)

            Dim doUnpack As Boolean = False
            Dim ignoreWSGIntegration As Boolean = False
            Dim shouldCheckForUpcomingEvent As Boolean = True

            Dim args = My.Application.CommandLineArgs
            Dim preventWSG As Boolean = args.Any(Function(a) a.Equals("--prevent-wsg", StringComparison.OrdinalIgnoreCase))
            Dim fileArg As String = args.FirstOrDefault(Function(a) Not a.StartsWith("--"))

            If Not String.IsNullOrEmpty(fileArg) Then
                ' Open the file passed as an argument
                _currentFile = fileArg
                doUnpack = True
                ignoreWSGIntegration = Settings.SessionSettings.WSGIgnoreWhenOpeningDPHX
                _launchedWithDPHXArgument = String.Equals(Path.GetExtension(fileArg), ".dphx", StringComparison.OrdinalIgnoreCase)
                If _launchedWithDPHXArgument Then
                    shouldCheckForUpcomingEvent = False
                End If
            Else
                _launchedWithDPHXArgument = False
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

            If Not ignoreWSGIntegration AndAlso Not preventWSG AndAlso Not _taskFetchRequestedByListener Then
                Dim integrationOption = Settings.SessionSettings.WSGIntegration
                If integrationOption <> AllSettings.WSGIntegrationOptions.None Then
                    _pendingWSGIntegration = integrationOption
                Else
                    _pendingWSGIntegration = Nothing
                End If
            Else
                _pendingWSGIntegration = Nothing
            End If

            _readySignalForListener.Set()

            If shouldCheckForUpcomingEvent Then
                _awaitingUpcomingEventDecision = True
                CheckForUpcomingEventAsync()
            Else
                TryLaunchWSGIntegration()
            End If

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
            Do While tries < 10 AndAlso Not SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder, True)
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

        UpdateBriefingRenderContext()
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
        RegenerateBriefingIfLoaded()

    End Sub

    Private Sub LoadDPHX_Click(sender As Object, e As EventArgs) Handles DPHXPackageToolStripMenuItem.Click

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

    Private Sub ManualModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManualModeToolStripMenuItem.Click

        Dim manualResult = PromptForManualSelection(_manualFallbackFlightPlanPath,
                                                    _manualFallbackWeatherPrimaryPath,
                                                    _manualFallbackWeatherSecondaryPath,
                                                    _manualFallbackSSCPresetName,
                                                    _manualFallbackTrackerGroup)

        If manualResult IsNot Nothing Then
            Dim sourceLabel = Path.GetFileName(manualResult.FlightPlanPath)
            If String.IsNullOrWhiteSpace(sourceLabel) Then
                sourceLabel = "Manual selection"
            End If

            LoadManualSelection(manualResult.FlightPlanPath,
                                manualResult.PrimaryWeatherLocalPath,
                                manualResult.SecondaryWeatherLocalPath,
                                manualResult.TrackerGroupName,
                                manualResult.SSCPresetName,
                                Nothing,
                                sourceLabel)
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
                _taskFetchRequestedByListener = True
                _pendingWSGIntegration = Nothing
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
        _taskFetchRequestedByListener = True
        _pendingWSGIntegration = Nothing

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

    Private Sub TryLaunchWSGIntegration()
        If Not _pendingWSGIntegration.HasValue Then
            Return
        End If

        If _awaitingUpcomingEventDecision OrElse _taskFetchRequestedByListener Then
            Return
        End If

        If _isClosing OrElse Me.IsDisposed Then
            _pendingWSGIntegration = Nothing
            Return
        End If

        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() TryLaunchWSGIntegration())
            Return
        End If

        Dim integrationOption = _pendingWSGIntegration.Value
        _pendingWSGIntegration = Nothing

        Select Case integrationOption
            Case AllSettings.WSGIntegrationOptions.OpenHome
                SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=home")
            Case AllSettings.WSGIntegrationOptions.OpenMap
                SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=map")
            Case AllSettings.WSGIntegrationOptions.OpenEvents
                SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?tab=events")
        End Select
    End Sub

    Private Sub CompleteUpcomingEventDecision(joinedUpcomingEvent As Boolean)
        If joinedUpcomingEvent Then
            _pendingWSGIntegration = Nothing
        End If

        _awaitingUpcomingEventDecision = False

        TryLaunchWSGIntegration()
    End Sub

    Private Async Sub CheckForUpcomingEventAsync()
        If _upcomingEventCheckAttempted Then
            Return
        End If

        _upcomingEventCheckAttempted = True

        Try
            Await Task.Delay(TimeSpan.FromSeconds(1))

            If _isClosing OrElse Me.IsDisposed Then
                CompleteUpcomingEventDecision(False)
                Return
            End If

            If ShouldSuppressUpcomingEventPrompt() Then
                CompleteUpcomingEventDecision(False)
                Return
            End If

            Dim upcoming = Await FetchUpcomingEventAsync()

            If upcoming Is Nothing Then
                CompleteUpcomingEventDecision(False)
                Return
            End If

            If _isClosing OrElse Me.IsDisposed Then
                CompleteUpcomingEventDecision(False)
                Return
            End If

            If ShouldSuppressUpcomingEventPrompt() Then
                CompleteUpcomingEventDecision(False)
                Return
            End If

            If Not Me.IsHandleCreated Then
                CompleteUpcomingEventDecision(False)
                Return
            End If

            Me.BeginInvoke(Sub()
                               If _isClosing OrElse Me.IsDisposed Then
                                   CompleteUpcomingEventDecision(False)
                                   Return
                               End If

                               If ShouldSuppressUpcomingEventPrompt() Then
                                   CompleteUpcomingEventDecision(False)
                                   Return
                               End If

                               Dim joined = False

                               Using prompt As New UpcomingEventPrompt(upcoming)
                                   If prompt.ShowDialog(Me) = DialogResult.OK Then
                                       joined = True
                                       JoinUpcomingEvent(upcoming)
                                   End If
                               End Using

                               CompleteUpcomingEventDecision(joined)
                           End Sub)
        Catch
            CompleteUpcomingEventDecision(False)
        End Try
    End Sub

    Private Function ShouldSuppressUpcomingEventPrompt() As Boolean
        Return _launchedWithDPHXArgument OrElse _taskFetchRequestedByListener
    End Function

    Private Async Function FetchUpcomingEventAsync() As Task(Of UpcomingEventInfo)
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveNews.php?fullText=true")
                response.EnsureSuccessStatusCode()

                Dim content = Await response.Content.ReadAsStringAsync()
                Dim root As JObject = JObject.Parse(content)

                If Not String.Equals(root("status")?.ToString(), "success", StringComparison.OrdinalIgnoreCase) Then
                    Return Nothing
                End If

                Dim data = TryCast(root("data"), JArray)
                If data Is Nothing OrElse data.Count = 0 Then
                    Return Nothing
                End If

                Dim nowUtc = DateTime.UtcNow

                Dim candidates As New List(Of UpcomingEventInfo)()

                For Each item As JObject In data.OfType(Of JObject)()
                    Dim newsTypeToken = item("NewsType")
                    Dim newsType As Integer
                    If newsTypeToken Is Nothing OrElse Not Integer.TryParse(newsTypeToken.ToString(), newsType) OrElse newsType <> 1 Then
                        Continue For
                    End If

                    Dim entrySeqToken = item("EntrySeqID")?.ToString()
                    Dim entrySeqId As Integer
                    If String.IsNullOrWhiteSpace(entrySeqToken) OrElse Not Integer.TryParse(entrySeqToken, entrySeqId) OrElse entrySeqId <= 0 OrElse entrySeqId = _allDPHData.EntrySeqID Then
                        Continue For
                    End If

                    Dim eventDateStr = item("EventDate")?.ToString()
                    If String.IsNullOrWhiteSpace(eventDateStr) Then
                        Continue For
                    End If

                    Dim eventDateUtc As DateTime
                    If Not DateTime.TryParseExact(eventDateStr,
                                                 "yyyy-MM-dd HH:mm:ss",
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal,
                                                 eventDateUtc) Then
                        If Not DateTime.TryParse(eventDateStr,
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal,
                                                 eventDateUtc) Then
                            Continue For
                        End If
                    End If
                    Dim windowStart = eventDateUtc.AddMinutes(-60)
                    Dim windowEnd = eventDateUtc.AddMinutes(30)

                    If nowUtc < windowStart OrElse nowUtc > windowEnd Then
                        Continue For
                    End If

                    Dim info As New UpcomingEventInfo With {
                        .Title = item("Title")?.ToString(),
                        .Subtitle = item("Subtitle")?.ToString(),
                        .Comments = item("Comments")?.ToString(),
                        .EventDateUtc = eventDateUtc,
                        .EntrySeqID = entrySeqId,
                        .TaskID = item("TaskID")?.ToString()
                    }

                    candidates.Add(info)
                Next

                If candidates.Count = 0 Then
                    Return Nothing
                End If

                Return candidates.OrderBy(Function(c) Math.Abs((c.EventDateUtc - nowUtc).TotalMinutes)).ThenBy(Function(c) c.EventDateUtc).FirstOrDefault()
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    Private Sub JoinUpcomingEvent(info As UpcomingEventInfo)
        If info Is Nothing Then
            Return
        End If

        Dim taskTitle As String = String.Empty
        Dim taskId As String = SupportingFeatures.FetchTaskIDUsingEntrySeqID(info.EntrySeqID.ToString(), taskTitle)

        If String.IsNullOrEmpty(taskId) AndAlso Not String.IsNullOrEmpty(info.TaskID) Then
            taskId = info.TaskID
            If String.IsNullOrEmpty(taskTitle) Then
                taskTitle = info.Title
            End If
        End If

        If String.IsNullOrEmpty(taskId) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me,
                                "Unable to retrieve the task associated with this event at the moment.",
                                "Join Event",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information)
            End Using
            Return
        End If

        Dim downloaded = SupportingFeatures.DownloadTaskFile(taskId, taskTitle, Settings.SessionSettings.PackagesFolder)

        If downloaded = String.Empty Then
            Return
        End If

        SupportingFeatures.BringWindowToFront(Me)

        LoadDPHXPackage(downloaded)

        If Settings.SessionSettings.AutoUnpack AndAlso _currentFile <> String.Empty AndAlso _lastLoadSuccess Then
            UnpackFiles(True)
        End If
    End Sub

    Private Function OpenSettingsWindow(Optional firstRun As Boolean = False) As DialogResult
        Dim formSettings As New Settings
        formSettings.IsFirstRun = firstRun

        Dim result As DialogResult = formSettings.ShowDialog(Me)

        If result = DialogResult.Abort Then
            Return result
        End If
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
        _isManualMode = False

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

            EnsureSessionDataDefaults(_allDPHData)

            'Fix the weather file format to be compatible with both MSFS versions
            FixWeatherFileFormat(_allDPHData.WeatherFilename)
            FixWeatherFileFormat(_allDPHData.WeatherFilenameSecondary)

            'We need to retrieve the DiscordPostID from the task online server
            If Not String.IsNullOrWhiteSpace(_allDPHData.TaskID) OrElse _allDPHData.EntrySeqID > 0 Then
                GetTaskDetails(_allDPHData.TaskID, _allDPHData.EntrySeqID)
            End If

            ctrlBriefing.GenerateBriefing(_SF,
                                          _allDPHData,
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(If(Not String.IsNullOrWhiteSpace(_allDPHData.WeatherFilename),
                                                                                              _allDPHData.WeatherFilename,
                                                                                              _allDPHData.WeatherFilenameSecondary))),
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
            Dim weather2020 = GetWeatherFilenameForSim(True)
            If Not String.IsNullOrWhiteSpace(weather2020) Then
                Dim sourceName = Path.GetFileName(weather2020)
                Dim installedName = TaskFileHelper.GetInstalledWeatherPresetFilename(sourceName)
                _filesToUnpack2020.Add("Weather File", installedName)
                If File.Exists(Path.Combine(Settings.SessionSettings.MSFS2020WeatherPresetsFolder, installedName)) Then
                    _filesCurrentlyUnpacked2020.Add("Weather File", installedName)
                End If
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
            Dim weather2024 = GetWeatherFilenameForSim(False)
            If Not String.IsNullOrWhiteSpace(weather2024) Then
                Dim sourceName = Path.GetFileName(weather2024)
                Dim installedName = TaskFileHelper.GetInstalledWeatherPresetFilename(sourceName)
                _filesToUnpack2024.Add("Weather File", installedName)
                If File.Exists(Path.Combine(Settings.SessionSettings.MSFS2024WeatherPresetsFolder, installedName)) Then
                    _filesCurrentlyUnpacked2024.Add("Weather File", installedName)
                End If
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

        Dim loggerFlightPlanPath As String = GetCurrentFlightPlanPath()
        Dim primaryWeather As String = NullToEmpty(_allDPHData.WeatherFilename)
        Dim secondaryWeather As String = NullToEmpty(_allDPHData.WeatherFilenameSecondary)
        Dim weatherPresence As String
        If Not String.IsNullOrWhiteSpace(primaryWeather) AndAlso Not String.IsNullOrWhiteSpace(secondaryWeather) Then
            weatherPresence = "Primary + Secondary"
        ElseIf Not String.IsNullOrWhiteSpace(primaryWeather) Then
            weatherPresence = "Primary only"
        ElseIf Not String.IsNullOrWhiteSpace(secondaryWeather) Then
            weatherPresence = "Secondary only"
        Else
            weatherPresence = "None"
        End If

        _status.AppendStatusLine($"SSCPresetName: {If(String.IsNullOrWhiteSpace(_allDPHData.SSCPresetName), "(none)", _allDPHData.SSCPresetName)}", False)
        _status.AppendStatusLine($"Weather preset availability: {weatherPresence}", False)
        If Settings.SessionSettings.Is2020Installed Then
            Dim weather2020Selection = GetWeatherFilenameForSim(True)
            _status.AppendStatusLine($"Weather file selected for MSFS 2020: {FormatWeatherFilenameForLog(weather2020Selection)}", False)
        End If
        If Settings.SessionSettings.Is2024Installed Then
            Dim weather2024Selection = GetWeatherFilenameForSim(False)
            _status.AppendStatusLine($"Weather file selected for MSFS 2024: {FormatWeatherFilenameForLog(weather2024Selection)}", False)
        End If

        Dim internalLoggerInUse As Boolean = EnsureInternalLoggerInUse(_status)

        ' NB21 auto-start and PLN feeding
        If Settings.SessionSettings.NB21StartAndFeed AndAlso (Not internalLoggerInUse) Then
            Dim NB21LoggerRunning As Boolean = TaskFileHelper.EnsureNb21Running(_status)
            If NB21LoggerRunning AndAlso Not String.IsNullOrWhiteSpace(loggerFlightPlanPath) Then
                TaskFileHelper.SendPlnFileToNB21Logger(loggerFlightPlanPath, _status)
            End If
        End If

        ' Tracker auto-start and data feeding
        If Settings.SessionSettings.TrackerStartAndFeed Then
            Dim TrackerRunning As Boolean = TaskFileHelper.EnsureTrackerRunning(_status)

            If TrackerRunning Then
                'Feed the data to the tracker
                Dim groupToUse As String = ""
                Dim trackerGroupValue As String = NullToEmpty(_allDPHData.TrackerGroup)

                If _allDPHData.IsFutureOrActiveEvent OrElse fromEvent Then
                    groupToUse = trackerGroupValue
                ElseIf _isManualMode Then
                    If Not String.IsNullOrWhiteSpace(trackerGroupValue) Then
                        groupToUse = trackerGroupValue
                    End If
                End If

                Dim flightPlanFilePath As String = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename))
                Dim flightPlanTitle As String = SupportingFeatures.GetFlightPlanTitleFromPln(flightPlanFilePath)
                Dim trackerWeatherFilename As String = String.Empty

                If Settings.SessionSettings.Is2024Installed Then
                    trackerWeatherFilename = GetWeatherFilenameForSim(False)
                ElseIf Settings.SessionSettings.Is2020Installed Then
                    trackerWeatherFilename = GetWeatherFilenameForSim(True)
                Else
                    trackerWeatherFilename = _allDPHData.WeatherFilename
                End If
                Dim trackerWeatherPath As String = String.Empty
                If Not String.IsNullOrWhiteSpace(trackerWeatherFilename) Then
                    trackerWeatherPath = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(trackerWeatherFilename))
                End If

                TaskFileHelper.SendDataToTracker(groupToUse,
                                  flightPlanFilePath,
                                  trackerWeatherPath,
                              _allDPHData.URLGroupEventPost,
                              flightPlanTitle,
                              _status
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
            Dim weather2020 = GetWeatherFilenameForSim(True)
            If Not String.IsNullOrWhiteSpace(weather2020) Then
                _status.AppendStatusLine(CopyWeatherPresetFile(Path.GetFileName(weather2020),
                     TempDPHXUnpackFolder,
                     Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                     "Weather Preset for MSFS 2020"), True)
            Else
                _status.AppendStatusLine("Weather Preset for MSFS 2020 skipped - no weather preset specified.", True)
            End If

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
            Dim weather2024 = GetWeatherFilenameForSim(False)
            If Not String.IsNullOrWhiteSpace(weather2024) Then
                _status.AppendStatusLine(CopyWeatherPresetFile(Path.GetFileName(weather2024),
                     TempDPHXUnpackFolder,
                     Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                     "Weather Preset for MSFS 2024"), True)
            Else
                _status.AppendStatusLine("Weather Preset for MSFS 2024 skipped - no weather preset specified.", True)
            End If

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

    Private ReadOnly Property IsUnpackRed As Boolean
        Get
            Return toolStripUnpack.ForeColor = Color.Red
        End Get
    End Property

    Private Function CreatePLNForEFB(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String
        Return TaskFileHelper.CreatePlnForEfb(filename, sourcePath, destPath, msgToAsk, Me, warningMSFSRunningToolStrip.Visible)
    End Function


    Private Function CopyFile(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String
        Return TaskFileHelper.CopyTaskFile(filename, sourcePath, destPath, msgToAsk, Me, warningMSFSRunningToolStrip.Visible)
    End Function

    Private Function CopyWeatherPresetFile(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String
        Return TaskFileHelper.CopyWeatherPresetToMsfs(filename, sourcePath, destPath, msgToAsk, Me, warningMSFSRunningToolStrip.Visible)
    End Function

    Private Function DeleteFile(filename As String, sourcePath As String, msgToAsk As String, excludeFromCleanup As Boolean) As String
        Return TaskFileHelper.DeleteTaskFile(filename, sourcePath, msgToAsk, excludeFromCleanup, warningMSFSRunningToolStrip.Visible)
    End Function

    Private Sub CleanupFiles()
        ' 1) Build a list of candidates that *would* be deleted
        Dim candidates As New List(Of CleanupCandidate)
        Dim additionalCleanupSteps As List(Of System.Func(Of String)) = Nothing
        If Settings.SessionSettings.TrackerStartAndFeed Then
            additionalCleanupSteps = New List(Of System.Func(Of String)) From {
                New System.Func(Of String)(AddressOf TaskFileHelper.ExecuteTrackerTaskFolderCleanup)
            }
        End If

        ' Helper: add a candidate only if it truly exists and is not excluded
        Dim addCand = Sub(fileName As String, folder As String, label As String, shortLabel As String, excluded As Boolean)
                          If String.IsNullOrWhiteSpace(fileName) Then Exit Sub
                          If excluded Then Exit Sub
                          If Not Directory.Exists(folder) Then Exit Sub

                          Dim full As String = Path.Combine(folder, fileName)
                          If Not File.Exists(full) Then Exit Sub

                          Dim disp As String = $"{shortLabel} : {fileName}"

                          candidates.Add(New CleanupCandidate With {
                          .Display = disp,
                          .FileName = fileName,
                          .SourcePath = folder,
                          .Label = label,
                          .DefaultChecked = True
                      })
                      End Sub

        If Settings.SessionSettings.Is2020Installed Then
            addCand(Path.GetFileName(_allDPHData.FlightPlanFilename),
                Settings.SessionSettings.MSFS2020FlightPlansFolder,
                "Flight Plan for MSFS 2020",
                "PLN 2020",
                Settings.SessionSettings.Exclude2020FlightPlanFromCleanup)

            Dim weather2020 = GetWeatherFilenameForSim(True)
            Dim weather2020Source = Path.GetFileName(weather2020)
            Dim weather2020Installed = TaskFileHelper.GetInstalledWeatherPresetFilename(weather2020Source)
            addCand(weather2020Installed,
                Settings.SessionSettings.MSFS2020WeatherPresetsFolder,
                $"Weather Preset for MSFS 2020 (source: {weather2020Source}, installed: {weather2020Installed})",
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

            Dim weather2024 = GetWeatherFilenameForSim(False)
            Dim weather2024Source = Path.GetFileName(weather2024)
            Dim weather2024Installed = TaskFileHelper.GetInstalledWeatherPresetFilename(weather2024Source)
            addCand(weather2024Installed,
                Settings.SessionSettings.MSFS2024WeatherPresetsFolder,
                $"Weather Preset for MSFS 2024 (source: {weather2024Source}, installed: {weather2024Installed})",
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
        Using dlg As New CleanupConfirmForm(candidates, AddressOf DeleteFile, additionalCleanupSteps)
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
        UpdateBriefingRenderContext()
        RegenerateBriefingIfLoaded()
    End Sub

    Friend Function EnsureInternalLoggerInUse(status As frmStatus) As Boolean
        Dim internalLoggerInUse As Boolean = False

        Dim requestFile As String = Path.Combine(Application.StartupPath, "GiveMeTheLogger.Please")
        If Not File.Exists(requestFile) Then
            Return False
        End If

        Dim processList As Process() = Process.GetProcessesByName("NB21_logger")
        Dim NB21LoggerRunning As Boolean = processList.Length > 0
        If NB21LoggerRunning Then
            If status IsNot Nothing Then
                status.AppendStatusLine("Internal NB21 Logger requested but external already running.", False)
            End If
            Return False
        End If

        OpenLoggerForm()

        If _loggerForm IsNot Nothing AndAlso _loggerForm.IsReady Then
            If status IsNot Nothing Then
                status.AppendStatusLine("Internal NB21 Logger successfully started and setup.", True)
            End If
            internalLoggerInUse = True
        Else
            If status IsNot Nothing Then
                status.AppendStatusLine("Internal NB21 Logger is not ready, possibly reverting to external.", True)
            End If
        End If

        Return internalLoggerInUse
    End Function

    Friend Sub UpdateManualFallbackPaths(flightPlanPath As String, primaryWeatherPath As String, secondaryWeatherPath As String, Optional trackerGroup As String = Nothing, Optional sscPresetName As String = Nothing)
        _manualFallbackFlightPlanPath = If(String.IsNullOrWhiteSpace(flightPlanPath), String.Empty, flightPlanPath)
        _manualFallbackWeatherPrimaryPath = If(String.IsNullOrWhiteSpace(primaryWeatherPath), String.Empty, primaryWeatherPath)
        _manualFallbackWeatherSecondaryPath = If(String.IsNullOrWhiteSpace(secondaryWeatherPath), String.Empty, secondaryWeatherPath)

        If trackerGroup IsNot Nothing Then
            _manualFallbackTrackerGroup = trackerGroup
        End If

        If sscPresetName IsNot Nothing Then
            _manualFallbackSSCPresetName = sscPresetName
        End If
    End Sub

    Friend Sub UpdateManualFallbackFlightPlanPath(flightPlanPath As String)
        UpdateManualFallbackPaths(flightPlanPath, _manualFallbackWeatherPrimaryPath, _manualFallbackWeatherSecondaryPath)
    End Sub

    Private Sub OpenLoggerForm()

        ' Check that we have the user info on WSG - if not, can't use the logger
        If Settings.SessionSettings.WSGUserID = 0 Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"Please log in to WeSimGlide.org first before using the logger!", "Not logged in to WeSimGlide.org", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Using
            Exit Sub
        End If

        Dim pilotName As String = If(Settings.SessionSettings.WSGPilotName, String.Empty)
        Dim competitionId As String = If(Settings.SessionSettings.WSGCompID, String.Empty)
        Dim tracklogsPath As String = If(Settings.SessionSettings.NB21IGCFolder, String.Empty)
        Dim flightPlanPath As String = GetCurrentFlightPlanPath()

        If _loggerForm Is Nothing OrElse _loggerForm.IsDisposed OrElse (Not _loggerForm.Visible) Then
            If _loggerForm IsNot Nothing Then
                _loggerForm.Dispose()
                _loggerForm = Nothing
            End If

            _loggerForm = New Logger(pilotName,
                                      competitionId,
                                      tracklogsPath,
                                      flightPlanPath)
            _loggerForm.Show(Me)
        Else
            _loggerForm.UpdateConfigurationFromCaller(pilotName,
                                                      competitionId,
                                                      tracklogsPath,
                                                      flightPlanPath)
        End If

        'Make sure the form is visible, restored if minimized and on top
        _loggerForm.WindowState = FormWindowState.Normal
        _loggerForm.BringToFront()

    End Sub

    Private Function GetCurrentFlightPlanPath() As String
        Dim manualPath = GetManualFallbackFlightPlanPath()
        If Not String.IsNullOrEmpty(manualPath) Then
            Return manualPath
        End If

        If _allDPHData Is Nothing Then
            Return String.Empty
        End If

        If String.IsNullOrWhiteSpace(_allDPHData.FlightPlanFilename) Then
            Return String.Empty
        End If

        Dim filenameOnly As String = Path.GetFileName(_allDPHData.FlightPlanFilename)
        Dim tempPath As String = Path.Combine(TempDPHXUnpackFolder, filenameOnly)

        If File.Exists(tempPath) Then
            Return tempPath
        End If

        If File.Exists(_allDPHData.FlightPlanFilename) Then
            Return _allDPHData.FlightPlanFilename
        End If

        Return String.Empty
    End Function

    Private Function GetManualFallbackFlightPlanPath() As String
        If String.IsNullOrWhiteSpace(_manualFallbackFlightPlanPath) Then
            Return String.Empty
        End If

        If File.Exists(_manualFallbackFlightPlanPath) Then
            Return _manualFallbackFlightPlanPath
        End If

        _manualFallbackFlightPlanPath = String.Empty
        Return String.Empty
    End Function

    Private Function LoadAllDataFromFile(dataFile As String) As AllData
        Try
            If String.IsNullOrWhiteSpace(dataFile) OrElse Not File.Exists(dataFile) Then
                Return Nothing
            End If

            Dim serializer As New XmlSerializer(GetType(AllData))
            Using stream As New FileStream(dataFile, FileMode.Open, FileAccess.Read)
                Return CType(serializer.Deserialize(stream), AllData)
            End Using

        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me,
                                $"Unable to read the task data file ({Path.GetFileName(dataFile)}): {ex.Message}",
                                "Task data",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
            End Using
            Return Nothing
        End Try
    End Function

    Private Function ExtractFlightPlanMetadata(flightPlanPath As String) As FlightPlanMetadata
        Dim metadata As New FlightPlanMetadata With {
            .Title = Path.GetFileNameWithoutExtension(flightPlanPath)
        }

        Try
            Dim doc = XDocument.Load(flightPlanPath)
            Dim titleElement = doc.Descendants("Title").FirstOrDefault()
            If titleElement IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(titleElement.Value) Then
                metadata.Title = titleElement.Value.Trim()
            End If

            Dim departureId = doc.Descendants("DepartureID").FirstOrDefault()
            If departureId IsNot Nothing Then
                metadata.DepartureId = departureId.Value.Trim()
            End If

            Dim departureName = doc.Descendants("DepartureName").FirstOrDefault()
            If departureName IsNot Nothing Then
                metadata.DepartureName = departureName.Value.Trim()
            End If

            Dim arrivalId = doc.Descendants("DestinationID").FirstOrDefault()
            If arrivalId IsNot Nothing Then
                metadata.ArrivalId = arrivalId.Value.Trim()
            End If

            Dim arrivalName = doc.Descendants("DestinationName").FirstOrDefault()
            If arrivalName IsNot Nothing Then
                metadata.ArrivalName = arrivalName.Value.Trim()
            End If

        Catch
            'Non-fatal: fall back to defaults
        End Try

        Return metadata
    End Function

    Private Function BuildSessionDataFromManualSources(flightPlanPath As String, primaryWeatherPath As String, secondaryWeatherPath As String, trackerGroup As String, sscPresetName As String, taskData As AllData) As AllData
        Dim session = If(taskData IsNot Nothing, taskData, New AllData())

        EnsureSessionDataDefaults(session)

        Dim metadata = ExtractFlightPlanMetadata(flightPlanPath)

        session.FlightPlanFilename = flightPlanPath
        session.WeatherFilename = primaryWeatherPath
        session.WeatherFilenameSecondary = secondaryWeatherPath
        session.SSCPresetName = If(sscPresetName, String.Empty)

        If String.IsNullOrWhiteSpace(session.Title) Then
            session.Title = metadata.Title
        End If

        If String.IsNullOrWhiteSpace(session.DepartureICAO) Then
            session.DepartureICAO = metadata.DepartureId
        End If

        If String.IsNullOrWhiteSpace(session.DepartureName) Then
            session.DepartureName = metadata.DepartureName
        End If

        If String.IsNullOrWhiteSpace(session.ArrivalICAO) Then
            session.ArrivalICAO = metadata.ArrivalId
        End If

        If String.IsNullOrWhiteSpace(session.ArrivalName) Then
            session.ArrivalName = metadata.ArrivalName
        End If

        If taskData IsNot Nothing Then
            If session.SimDate = Date.MinValue Then
                session.SimDate = Date.Today
            End If

            If session.SimTime = Date.MinValue Then
                session.SimTime = Date.Now
            End If
        End If

        If Not String.IsNullOrWhiteSpace(trackerGroup) Then
            session.TrackerGroup = trackerGroup
        End If

        session.ExtraFiles = session.ExtraFiles.Where(Function(f) Not String.IsNullOrWhiteSpace(f) AndAlso File.Exists(f)).ToList()

        If Not String.IsNullOrWhiteSpace(session.MapImageSelected) AndAlso Not File.Exists(session.MapImageSelected) Then
            session.MapImageSelected = String.Empty
        End If

        If Not String.IsNullOrWhiteSpace(session.GroupEventTeaserAreaMapImage) AndAlso Not File.Exists(session.GroupEventTeaserAreaMapImage) Then
            session.GroupEventTeaserAreaMapImage = String.Empty
        End If

        If Not String.IsNullOrWhiteSpace(session.TrackerGroup) Then
            _manualFallbackTrackerGroup = session.TrackerGroup
        End If

        Return session
    End Function

    Private Sub EnsureSessionDataDefaults(session As AllData)
        If session Is Nothing Then
            Return
        End If

        session.ExtraFiles = If(session.ExtraFiles, New List(Of String)())
        session.Countries = If(session.Countries, New List(Of String)())
        session.RecommendedAddOns = If(session.RecommendedAddOns, New List(Of RecommendedAddOn)())

        session.TaskID = NullToEmpty(session.TaskID)
        session.DiscordTaskID = NullToEmpty(session.DiscordTaskID)
        session.DiscordTaskThreadURL = NullToEmpty(session.DiscordTaskThreadURL)
        session.WeatherFilename = NullToEmpty(session.WeatherFilename)
        session.WeatherFilenameSecondary = NullToEmpty(session.WeatherFilenameSecondary)
        session.SSCPresetName = NullToEmpty(session.SSCPresetName)
        session.Title = NullToEmpty(session.Title)
        session.SimDateTimeExtraInfo = NullToEmpty(session.SimDateTimeExtraInfo)
        session.DepartureICAO = NullToEmpty(session.DepartureICAO)
        session.DepartureName = NullToEmpty(session.DepartureName)
        session.DepartureExtra = NullToEmpty(session.DepartureExtra)
        session.ArrivalICAO = NullToEmpty(session.ArrivalICAO)
        session.ArrivalName = NullToEmpty(session.ArrivalName)
        session.ArrivalExtra = NullToEmpty(session.ArrivalExtra)
        session.MainAreaPOI = NullToEmpty(session.MainAreaPOI)
        session.ShortDescription = NullToEmpty(session.ShortDescription)
        session.SoaringExtraInfo = NullToEmpty(session.SoaringExtraInfo)
        session.RecommendedGliders = NullToEmpty(session.RecommendedGliders)
        session.DifficultyRating = NullToEmpty(session.DifficultyRating)
        session.DifficultyExtraInfo = NullToEmpty(session.DifficultyExtraInfo)
        session.WeatherSummary = NullToEmpty(session.WeatherSummary)
        session.BaroPressureExtraInfo = NullToEmpty(session.BaroPressureExtraInfo)
        session.Credits = NullToEmpty(session.Credits)
        session.LongDescription = NullToEmpty(session.LongDescription)
        session.TrackerGroup = NullToEmpty(session.TrackerGroup)
        session.GroupClubName = NullToEmpty(session.GroupClubName)
        session.GroupEmoji = NullToEmpty(session.GroupEmoji)
        session.EventTopic = NullToEmpty(session.EventTopic)
        session.URLGroupEventPost = NullToEmpty(session.URLGroupEventPost)
    End Sub

    Private Function NullToEmpty(value As String) As String
        Return If(value, String.Empty)
    End Function

    Private Function GetWeatherFilenameForSim(isMsfs2020 As Boolean) As String
        Dim primary As String = NullToEmpty(_allDPHData.WeatherFilename)
        Dim secondary As String = NullToEmpty(_allDPHData.WeatherFilenameSecondary)
        Dim hasPrimary As Boolean = Not String.IsNullOrWhiteSpace(primary)
        Dim hasSecondary As Boolean = Not String.IsNullOrWhiteSpace(secondary)

        If hasPrimary AndAlso hasSecondary Then
            Return If(isMsfs2020, secondary, primary)
        End If

        If hasPrimary Then
            Return primary
        End If

        Return secondary
    End Function

    Private Function GetInstalledSimFlags() As InstalledSimFlags
        Dim flags As InstalledSimFlags = InstalledSimFlags.None

        If Settings.SessionSettings.Is2020Installed Then
            flags = flags Or InstalledSimFlags.MSFS2020
        End If

        If Settings.SessionSettings.Is2024Installed Then
            flags = flags Or InstalledSimFlags.MSFS2024
        End If

        Return flags
    End Function

    Private Sub UpdateBriefingRenderContext()
        Dim briefingContext As New BriefingRenderContext() With {
            .HostMode = BriefingHostMode.EndUser,
            .InstalledSims = GetInstalledSimFlags(),
            .PresetNameDisplayMode = PresetNameDisplayMode.Exact
        }
        ctrlBriefing.RenderContext = briefingContext
    End Sub

    Private Sub RegenerateBriefingIfLoaded()
        If _allDPHData Is Nothing OrElse Not _lastLoadSuccess Then
            Return
        End If

        Dim flightPlanPath = GetBriefingFilePath(_allDPHData.FlightPlanFilename)
        Dim weatherPath = GetBriefingWeatherFilePath(_allDPHData.WeatherFilename, _allDPHData.WeatherFilenameSecondary)

        If String.IsNullOrWhiteSpace(flightPlanPath) OrElse String.IsNullOrWhiteSpace(weatherPath) Then
            Return
        End If

        If Not File.Exists(flightPlanPath) OrElse Not File.Exists(weatherPath) Then
            Return
        End If

        ctrlBriefing.GenerateBriefing(_SF,
                                      _allDPHData,
                                      flightPlanPath,
                                      weatherPath,
                                      _taskDiscordPostID,
                                      TempDPHXUnpackFolder,
                                      _isManualMode)
    End Sub

    Private Function GetBriefingFilePath(filename As String) As String
        If String.IsNullOrWhiteSpace(filename) Then
            Return String.Empty
        End If

        Return Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(filename))
    End Function

    Private Function GetBriefingWeatherFilePath(primaryFilename As String, secondaryFilename As String) As String
        Dim filename = If(Not String.IsNullOrWhiteSpace(primaryFilename), primaryFilename, secondaryFilename)

        If String.IsNullOrWhiteSpace(filename) Then
            Return String.Empty
        End If

        Return Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(filename))
    End Function

    Private Function FormatWeatherFilenameForLog(weatherFilename As String) As String
        If String.IsNullOrWhiteSpace(weatherFilename) Then
            Return "(none)"
        End If

        Return Path.GetFileName(weatherFilename)
    End Function

    Private Sub FixWeatherFileFormat(weatherFilename As String)
        If String.IsNullOrWhiteSpace(weatherFilename) Then
            Return
        End If

        Dim weatherFile As String = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(weatherFilename))
        If File.Exists(weatherFile) Then
            If _SF.FixWPRFormat(weatherFile, False) Then
                Return
            End If

            Using New Centered_MessageBox(Me)
                MessageBox.Show($"Unable to verify and fix the weather file for compatibility with both MSFS versions: {Path.GetFileName(weatherFilename)}", "Fixing WPR file", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
        End If
    End Sub

    Private Function StageManualFiles(flightPlanPath As String, primaryWeatherPath As String, secondaryWeatherPath As String) As Tuple(Of String, String, String)
        Dim planInsideTemp = IsPathInsideFolder(flightPlanPath, TempDPHXUnpackFolder)
        Dim weatherPrimaryInsideTemp = IsPathInsideFolder(primaryWeatherPath, TempDPHXUnpackFolder)
        Dim weatherSecondaryInsideTemp = IsPathInsideFolder(secondaryWeatherPath, TempDPHXUnpackFolder)

        If Not planInsideTemp AndAlso Not weatherPrimaryInsideTemp AndAlso Not weatherSecondaryInsideTemp Then
            SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder)
        End If

        If Not Directory.Exists(TempDPHXUnpackFolder) Then
            Directory.CreateDirectory(TempDPHXUnpackFolder)
        End If

        Dim stagedPln = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(flightPlanPath))
        Dim stagedPrimaryWpr As String = String.Empty
        Dim stagedSecondaryWpr As String = String.Empty

        If Not String.Equals(flightPlanPath, stagedPln, StringComparison.OrdinalIgnoreCase) Then
            File.Copy(flightPlanPath, stagedPln, True)
        Else
            If Not File.Exists(stagedPln) Then
                Throw New FileNotFoundException("Flight plan not found", stagedPln)
            End If
        End If

        If Not String.IsNullOrWhiteSpace(primaryWeatherPath) Then
            stagedPrimaryWpr = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(primaryWeatherPath))
            If Not String.Equals(primaryWeatherPath, stagedPrimaryWpr, StringComparison.OrdinalIgnoreCase) Then
                File.Copy(primaryWeatherPath, stagedPrimaryWpr, True)
            ElseIf Not File.Exists(stagedPrimaryWpr) Then
                Throw New FileNotFoundException("Primary weather preset not found", stagedPrimaryWpr)
            End If
        End If

        If Not String.IsNullOrWhiteSpace(secondaryWeatherPath) Then
            stagedSecondaryWpr = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(secondaryWeatherPath))
            If Not String.Equals(secondaryWeatherPath, stagedSecondaryWpr, StringComparison.OrdinalIgnoreCase) Then
                File.Copy(secondaryWeatherPath, stagedSecondaryWpr, True)
            ElseIf Not File.Exists(stagedSecondaryWpr) Then
                Throw New FileNotFoundException("Secondary weather preset not found", stagedSecondaryWpr)
            End If
        End If

        Return Tuple.Create(stagedPln, stagedPrimaryWpr, stagedSecondaryWpr)
    End Function

    Private Function IsPathInsideFolder(targetPath As String, folderPath As String) As Boolean
        If String.IsNullOrWhiteSpace(targetPath) OrElse String.IsNullOrWhiteSpace(folderPath) Then
            Return False
        End If

        Try
            Dim fullPath = System.IO.Path.GetFullPath(targetPath).TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
            Dim fullFolder = System.IO.Path.GetFullPath(folderPath).TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)

            Return fullPath.StartsWith(fullFolder, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

    Private Sub LoadManualSelection(flightPlanPath As String, primaryWeatherPath As String, secondaryWeatherPath As String, trackerGroup As String, sscPresetName As String, taskData As AllData, sourceLabel As String)

        _lastLoadSuccess = False
        _isManualMode = True

        If Not File.Exists(flightPlanPath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The selected flight plan could not be found.", "Manual selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return
        End If

        If String.IsNullOrWhiteSpace(primaryWeatherPath) AndAlso String.IsNullOrWhiteSpace(secondaryWeatherPath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The selected weather preset could not be found.", "Manual selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return
        End If

        If Not String.IsNullOrWhiteSpace(primaryWeatherPath) AndAlso Not File.Exists(primaryWeatherPath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The selected primary weather preset could not be found.", "Manual selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return
        End If

        If Not String.IsNullOrWhiteSpace(secondaryWeatherPath) AndAlso Not File.Exists(secondaryWeatherPath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "The selected secondary weather preset could not be found.", "Manual selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return
        End If

        Try
            ctrlBriefing.FullReset()
            Dim stagedFiles = StageManualFiles(flightPlanPath, primaryWeatherPath, secondaryWeatherPath)
            Dim stagedFlightPlan = stagedFiles.Item1
            Dim stagedPrimaryWeather = stagedFiles.Item2
            Dim stagedSecondaryWeather = stagedFiles.Item3

            Dim sessionData = BuildSessionDataFromManualSources(stagedFlightPlan, stagedPrimaryWeather, stagedSecondaryWeather, trackerGroup, sscPresetName, taskData)

            _allDPHData = sessionData
            _taskDiscordPostID = NullToEmpty(sessionData.DiscordTaskID)

            If Not String.IsNullOrWhiteSpace(stagedPrimaryWeather) Then
                Try
                    _SF.FixWPRFormat(stagedPrimaryWeather, False)
                Catch ex As Exception
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me, $"Unable to normalize the weather preset: {ex.Message}", "Weather preset", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Using
                End Try
            End If

            If Not String.IsNullOrWhiteSpace(stagedSecondaryWeather) Then
                Try
                    _SF.FixWPRFormat(stagedSecondaryWeather, False)
                Catch ex As Exception
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me, $"Unable to normalize the secondary weather preset: {ex.Message}", "Weather preset", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Using
                End Try
            End If

            If Not String.IsNullOrWhiteSpace(_allDPHData.TaskID) OrElse _allDPHData.EntrySeqID > 0 Then
                GetTaskDetails(_allDPHData.TaskID, _allDPHData.EntrySeqID)
            End If

            ctrlBriefing.GenerateBriefing(_SF,
                                          _allDPHData,
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(stagedFlightPlan)),
                                          Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(If(Not String.IsNullOrWhiteSpace(stagedPrimaryWeather),
                                                                                              stagedPrimaryWeather,
                                                                                              stagedSecondaryWeather))),
                                          _taskDiscordPostID,
                                          TempDPHXUnpackFolder, _isManualMode)

            _currentFile = sourceLabel
            txtPackageName.Text = sourceLabel
            _lastLoadSuccess = True
            UpdateManualFallbackPaths(stagedFlightPlan, stagedPrimaryWeather, stagedSecondaryWeather, trackerGroup, sscPresetName)

            EnableUnpackButton()
            SetFormCaption(_currentFile)
            packageNameToolStrip.Text = _currentFile

        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me,
                                $"Unable to load the selected files: {ex.Message}",
                                "Manual selection",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            End Using
        End Try

    End Sub

    Private Function PromptForManualSelection(initialPln As String, initialPrimaryWpr As String, initialSecondaryWpr As String, initialSscPresetName As String, trackerGroup As String) As ManualFallbackMode.ManualSelectionResult
        Using dialog As New ManualFallbackMode()
            dialog.InitialPlnPath = initialPln
            dialog.InitialPrimaryWprPath = initialPrimaryWpr
            dialog.InitialSecondaryWprPath = initialSecondaryWpr
            dialog.InitialSSCPresetName = initialSscPresetName
            dialog.InitialTrackerGroup = trackerGroup

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                Return dialog.SelectionResult
            End If
        End Using

        Return Nothing
    End Function

    Private Sub HandleManualFileCombination(files As IEnumerable(Of String))
        Dim plnPath As String = files.FirstOrDefault(Function(f) HasExtension(f, ".pln"))
        Dim wprPath As String = files.FirstOrDefault(Function(f) HasExtension(f, ".wpr"))
        Dim dphPath As String = files.FirstOrDefault(Function(f) HasExtension(f, ".dph"))

        If String.IsNullOrWhiteSpace(plnPath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "A flight plan (.pln) is required to continue.", "Missing flight plan", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
            Return
        End If

        Dim taskData As AllData = Nothing
        If Not String.IsNullOrWhiteSpace(dphPath) Then
            taskData = LoadAllDataFromFile(dphPath)
        End If

        Dim trackerGroupFromTask As String = If(taskData IsNot Nothing, NullToEmpty(taskData.TrackerGroup), String.Empty)
        Dim trackerGroup As String = If(Not String.IsNullOrWhiteSpace(_manualFallbackTrackerGroup), _manualFallbackTrackerGroup, trackerGroupFromTask)

        If String.IsNullOrWhiteSpace(wprPath) Then
            BeginInvoke(New Action(Sub()
                                       Dim manualResult = PromptForManualSelection(plnPath, String.Empty, String.Empty, String.Empty, trackerGroup)
                                       If manualResult Is Nothing Then
                                           Return
                                       End If

                                       CompleteManualSelection(manualResult.FlightPlanPath,
                                                               manualResult.PrimaryWeatherLocalPath,
                                                               manualResult.SecondaryWeatherLocalPath,
                                                               manualResult.TrackerGroupName,
                                                               manualResult.SSCPresetName,
                                                               taskData,
                                                               Path.GetFileName(manualResult.FlightPlanPath))
                                   End Sub))
            Return
        End If

        Dim primaryWeather As String = String.Empty
        Dim secondaryWeather As String = String.Empty
        If Settings.SessionSettings.Is2024Installed AndAlso Settings.SessionSettings.Is2020Installed Then
            primaryWeather = wprPath
        ElseIf Settings.SessionSettings.Is2024Installed Then
            primaryWeather = wprPath
        ElseIf Settings.SessionSettings.Is2020Installed Then
            secondaryWeather = wprPath
        Else
            primaryWeather = wprPath
        End If

        CompleteManualSelection(plnPath, primaryWeather, secondaryWeather, trackerGroup, String.Empty, taskData, Path.GetFileName(plnPath))
    End Sub

    Private Sub CompleteManualSelection(plnPath As String, primaryWprPath As String, secondaryWprPath As String, trackerGroup As String, sscPresetName As String, taskData As AllData, sourceLabel As String)
        If String.IsNullOrWhiteSpace(plnPath) OrElse (String.IsNullOrWhiteSpace(primaryWprPath) AndAlso String.IsNullOrWhiteSpace(secondaryWprPath)) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, "A flight plan and weather preset are required to continue.", "Manual selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
            Return
        End If

        LoadManualSelection(plnPath, primaryWprPath, secondaryWprPath, trackerGroup, sscPresetName, taskData, sourceLabel)
    End Sub

    Private Sub HandleZipDrop(zipPath As String)
        Dim extractionFolder = Path.Combine(Settings.SessionSettings.UnpackingFolder, "DroppedZip")

        If Directory.Exists(extractionFolder) Then
            Directory.Delete(extractionFolder, True)
        End If

        Directory.CreateDirectory(extractionFolder)

        Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
            For Each entry As ZipArchiveEntry In archive.Entries
                If String.IsNullOrEmpty(entry.Name) Then
                    Continue For
                End If

                Dim extension = Path.GetExtension(entry.Name)
                If HasExtension(extension, ".pln") OrElse HasExtension(extension, ".wpr") OrElse HasExtension(extension, ".dph") OrElse HasExtension(extension, ".dphx") Then
                    Dim destination = Path.Combine(extractionFolder, Path.GetFileName(entry.Name))
                    entry.ExtractToFile(destination, True)
                End If
            Next
        End Using

        Dim extractedFiles = Directory.GetFiles(extractionFolder, "*", SearchOption.AllDirectories)

        Dim dphxFile = extractedFiles.FirstOrDefault(Function(f) HasExtension(f, ".dphx"))
        If Not String.IsNullOrWhiteSpace(dphxFile) Then
            LoadDPHXPackage(dphxFile)
            If Settings.SessionSettings.AutoUnpack AndAlso _lastLoadSuccess Then
                UnpackFiles()
            End If
            Try
                Directory.Delete(extractionFolder, True)
            Catch
            End Try
            Return
        End If

        HandleManualFileCombination(extractedFiles)

        Try
            Directory.Delete(extractionFolder, True)
        Catch
        End Try
    End Sub

    Private Sub ProcessDroppedFiles(files As IReadOnlyList(Of String))
        If ContainsWeSimGlideLink(files) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me,
                                "When using the DPHX Unpack & Load, simply click on the DPHX link instead of trying to drag and drop links to WeSimGlide.org",
                                "Drag and drop",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information)
            End Using
            Return
        End If

        If files Is Nothing OrElse files.Count = 0 Then
            Return
        End If

        Dim normalizedFiles = NormalizeDroppedFiles(files)
        If normalizedFiles.Count = 0 Then
            Return
        End If

        If normalizedFiles.Count = 1 Then
            Dim singleFile = normalizedFiles(0)
            If HasExtension(singleFile, ".dphx") Then
                LoadDPHXPackage(singleFile)
                If Settings.SessionSettings.AutoUnpack AndAlso _lastLoadSuccess Then
                    UnpackFiles()
                End If
                Return
            End If

            If HasExtension(singleFile, ".zip") Then
                HandleZipDrop(singleFile)
                Return
            End If
        End If

        HandleManualFileCombination(normalizedFiles)
    End Sub

    Private Shared Function ContainsWeSimGlideLink(entries As IEnumerable(Of String)) As Boolean
        If entries Is Nothing Then
            Return False
        End If

        Return entries.Any(Function(entry) Not String.IsNullOrWhiteSpace(entry) AndAlso entry.IndexOf("wesimglide.org", StringComparison.OrdinalIgnoreCase) >= 0)
    End Function

    Private Shared Function HasExtension(filePath As String, extension As String) As Boolean
        If String.IsNullOrEmpty(filePath) OrElse String.IsNullOrEmpty(extension) Then
            Return False
        End If

        Dim extToCheck As String = filePath
        If Not filePath.StartsWith(".") OrElse filePath.Contains(Path.DirectorySeparatorChar) OrElse filePath.Contains(Path.AltDirectorySeparatorChar) Then
            extToCheck = Path.GetExtension(filePath)
        End If

        Return extToCheck.Equals(extension, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function NormalizeDroppedFiles(files As IEnumerable(Of String)) As List(Of String)
        Dim normalized As New List(Of String)()

        For Each entry In files
            If String.IsNullOrWhiteSpace(entry) Then
                Continue For
            End If

            Dim resolved = entry
            If ShouldDownloadFromUrl(entry) Then
                Try
                    resolved = DownloadDroppedFile(entry)
                Catch ex As Exception
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me,
                                        $"Unable to download the dropped link: {ex.Message}",
                                        "Drag and drop",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning)
                    End Using
                    Continue For
                End Try
            Else
                If Not File.Exists(resolved) Then
                    Using New Centered_MessageBox(Me)
                        MessageBox.Show(Me,
                                        $"The dropped file could not be found: {resolved}",
                                        "Drag and drop",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning)
                    End Using
                    Continue For
                End If
            End If

            normalized.Add(resolved)
        Next

        Return normalized
    End Function

    Private Function ShouldDownloadFromUrl(entry As String) As Boolean
        If String.IsNullOrWhiteSpace(entry) Then
            Return False
        End If

        Dim uri As Uri = Nothing
        If Not Uri.TryCreate(entry.Trim(), UriKind.Absolute, uri) Then
            Return False
        End If

        If Not (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps) Then
            Return False
        End If

        Dim ext = Path.GetExtension(uri.AbsolutePath)
        Return HasExtension(ext, ".pln") OrElse HasExtension(ext, ".wpr")
    End Function

    Private Function DownloadDroppedFile(entry As String) As String
        Dim uri As New Uri(entry.Trim())
        Dim downloadFolder = Path.Combine(Path.GetTempPath(), "DPHXDropped")
        If Not Directory.Exists(downloadFolder) Then
            Directory.CreateDirectory(downloadFolder)
        End If

        Using client As New WebClient()
            Using stream = client.OpenRead(uri)
                If stream Is Nothing Then
                    Throw New InvalidOperationException("Unable to download the dropped file.")
                End If

                Dim fileName = ResolveDroppedFileName(uri, client.ResponseHeaders)
                Dim targetPath = Path.Combine(downloadFolder, fileName)

                Using output As New FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None)
                    stream.CopyTo(output)
                End Using

                Return targetPath
            End Using
        End Using
    End Function

    Private Function ResolveDroppedFileName(uri As Uri, headers As WebHeaderCollection) As String
        Dim contentDisposition As String = Nothing
        If headers IsNot Nothing Then
            contentDisposition = headers("Content-Disposition")
        End If

        Dim fileNameFromHeader = SupportingFeatures.TryResolveContentDispositionFileName(contentDisposition)
        If Not String.IsNullOrWhiteSpace(fileNameFromHeader) Then
            Return fileNameFromHeader
        End If

        Dim fileName = Path.GetFileName(uri.LocalPath)
        If String.IsNullOrWhiteSpace(fileName) Then
            fileName = $"dropped_{Guid.NewGuid():N}{Path.GetExtension(uri.LocalPath)}"
        End If

        Return fileName
    End Function

    Private Class FlightPlanMetadata
        Public Property Title As String
        Public Property DepartureId As String
        Public Property DepartureName As String
        Public Property ArrivalId As String
        Public Property ArrivalName As String
    End Class

    Private Sub ctrlBriefing_ValidFilesDragActiveChanged(sender As Object, e As ValidFilesDragActiveChangedEventArgs) Handles ctrlBriefing.ValidFilesDragActiveChanged
        If e.IsActive Then
            Me.dragNdropToolStrip.Visible = True
        Else
            Me.dragNdropToolStrip.Visible = False
        End If
    End Sub

    Private Sub ctrlBriefing_FilesDropped(sender As Object, e As FilesDroppedEventArgs) Handles ctrlBriefing.FilesDropped
        Try
            ProcessDroppedFiles(e.Files)
        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me,
                                $"Unable to process the dropped files: {ex.Message}",
                                "Drag and drop",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            End Using
        End Try

    End Sub

#End Region

End Class
