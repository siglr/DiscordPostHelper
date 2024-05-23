Imports System.Configuration
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports System.Xml
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary
Imports SIGLR.SoaringTools.ImageViewer

Public Class DPHXUnpackAndLoad

#Region "Constants and other global variables"

    Private Const B21PlannerURL As String = "https://xp-soaring.github.io/tasks/b21_task_planner/index.html"

    Private ReadOnly _SF As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _currentFile As String = String.Empty
    Private _abortingFirstRun As Boolean = False
    Private _allDPHData As AllData
    Private _filesToUnpack As New Dictionary(Of String, String)
    Private _filesCurrentlyUnpacked As New Dictionary(Of String, String)

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

        CheckForNewVersion()

        lblAllFilesStatus.Text = String.Empty

        SupportingFeatures.CleanupDPHXTempFolder(TempDPHXUnpackFolder)

        If My.Application.CommandLineArgs.Count > 0 Then
            ' Open the file passed as an argument
            _currentFile = My.Application.CommandLineArgs(0)
        Else
            ' Check the last file that was opened
            If Not Settings.SessionSettings.LastDPHXOpened = String.Empty AndAlso File.Exists(Settings.SessionSettings.LastDPHXOpened) Then
                _currentFile = Settings.SessionSettings.LastDPHXOpened
            End If
        End If

        If Not _currentFile = String.Empty AndAlso Path.GetExtension(_currentFile) = ".dphx" Then
            LoadDPHXPackage(_currentFile)
            If Settings.SessionSettings.AutoUnpack Then
                UnpackFiles()
            End If
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
            Settings.SessionSettings.MainFormSize = Me.Size.ToString()
            Settings.SessionSettings.MainFormLocation = Me.Location.ToString()
            Settings.SessionSettings.Save()
        End If

    End Sub

    Private Sub DPHXUnpackAndLoad_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ctrlBriefing.AdjustRTBoxControls()
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles toolStripSettings.Click

        OpenSettingsWindow()
        'Recheck files
        If toolStripUnpack.Enabled Then
            EnableUnpackButton()
        End If

    End Sub

    Private Sub LoadDPHX_Click(sender As Object, e As EventArgs) Handles toolStripOpen.Click

        lblAllFilesStatus.Text = String.Empty

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
            warningMSFSRunningToolStrip.Visible = True
        Else
            warningMSFSRunningToolStrip.Visible = False
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

        TaskBrowser.ShowDialog(Me)
        TaskBrowser.Dispose()
        TaskBrowser = Nothing

        'Old button to go to the Discord library
        'SupportingFeatures.LaunchDiscordURL($"https://discord.com/channels/1022705603489042472/1155511739799060552")

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

        _filesToUnpack.Clear()
        _filesCurrentlyUnpacked.Clear()

        'Check if files are already unpacked
        'Flight plan
        _filesToUnpack.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
        If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename)),
                                                Path.Combine(Settings.SessionSettings.FlightPlansFolder,
                                                Path.GetFileName(_allDPHData.FlightPlanFilename))) Then
            _filesCurrentlyUnpacked.Add("Flight Plan", Path.GetFileName(_allDPHData.FlightPlanFilename))
        End If

        'Weather file
        _filesToUnpack.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
        If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename)),
                                                Path.Combine(Settings.SessionSettings.MSFSWeatherPresetsFolder,
                                                Path.GetFileName(_allDPHData.WeatherFilename))) Then
            _filesCurrentlyUnpacked.Add("Weather File", Path.GetFileName(_allDPHData.WeatherFilename))
        End If

        'XCSoar task
        If Settings.SessionSettings.XCSoarTasksFolder IsNot Nothing Then
            'Look in the other files for xcsoar file
            For Each filepath As String In _allDPHData.ExtraFiles
                If Path.GetExtension(filepath) = ".tsk" Then
                    'XCSoar task
                    _filesToUnpack.Add("XCSoar Task", Path.GetFileName(filepath))
                    If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                            Path.GetFileName(filepath)),
                                                            Path.Combine(Settings.SessionSettings.XCSoarTasksFolder,
                                                            Path.GetFileName(filepath))) Then
                        _filesCurrentlyUnpacked.Add("XCSoar Task", Path.GetFileName(filepath))
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
                    _filesToUnpack.Add("XCSoar Map", Path.GetFileName(filepath))
                    If SupportingFeatures.AreFilesIdentical(Path.Combine(TempDPHXUnpackFolder,
                                                            Path.GetFileName(filepath)),
                                                            Path.Combine(Settings.SessionSettings.XCSoarMapsFolder,
                                                            Path.GetFileName(filepath))) Then
                        _filesCurrentlyUnpacked.Add("XCSoar Map", Path.GetFileName(filepath))
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

        toolStatusOK.Visible = False
        toolStatusWarning.Visible = False
        toolStatusStop.Visible = False

        If _filesToUnpack.Count <> _filesCurrentlyUnpacked.Count Then
            toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Bold)
            toolStripUnpack.ForeColor = Color.Red
            If _filesCurrentlyUnpacked.Count = 0 Then
                lblAllFilesStatus.Text = $"All files ({GetListOfFilesMissing()}) are MISSING from their respective folder."
                toolStatusStop.Visible = True
            Else
                lblAllFilesStatus.Text = $"{_filesCurrentlyUnpacked.Count} out of {_filesToUnpack.Count} files are present: {GetListOfFilesPresent()}. MISSING files: {GetListOfFilesMissing()}"
                toolStatusWarning.Visible = True
            End If
        Else
            toolStripUnpack.Font = New Font(toolStripUnpack.Font, FontStyle.Regular)
            toolStripUnpack.ForeColor = DefaultForeColor
            lblAllFilesStatus.Text = $"All the files ({GetListOfFilesPresent()}) are present in their respective folder."
            toolStatusOK.Visible = True
        End If
    End Sub

    Private Function GetListOfFilesPresent() As String

        Dim result As String = String.Empty

        For Each fileType In _filesCurrentlyUnpacked.Keys
            If result = String.Empty Then
                result = $"{fileType}"
            Else
                result = $"{result}, {fileType}"
            End If
        Next

        Return result

    End Function

    Private Function GetListOfFilesMissing() As String

        Dim result As String = String.Empty

        For Each fileType In _filesToUnpack.Keys
            If Not _filesCurrentlyUnpacked.Keys.Contains(fileType) Then
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

    Private Sub UnpackFiles()

        Dim sb As New StringBuilder

        sb.AppendLine("Unpacking Results:")
        sb.AppendLine()

        'Flight plan
        sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.FlightPlansFolder,
                 "Flight Plan"))
        sb.AppendLine()

        'Weather file
        sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFSWeatherPresetsFolder,
                 "Weather Preset"))

        sb.AppendLine()

        'Look in the other files for xcsoar files
        For Each filepath As String In _allDPHData.ExtraFiles
            If Path.GetExtension(filepath) = ".tsk" Then
                'XCSoar task
                sb.AppendLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarTasksFolder,
                                    "XCSoar Task"))
            End If
            If Path.GetExtension(filepath) = ".xcm" Then
                'XCSoar map
                sb.AppendLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarMapsFolder,
                                    "XCSoar Map"))
            End If
        Next

        Using New Centered_MessageBox(Me)
            MessageBox.Show(sb.ToString, "Unpacking results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using

        EnableUnpackButton()

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

        'Flight plan
        sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 Settings.SessionSettings.FlightPlansFolder,
                 "Flight Plan",
                 Settings.SessionSettings.ExcludeFlightPlanFromCleanup))
        sb.AppendLine()

        'Weather file
        sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 Settings.SessionSettings.MSFSWeatherPresetsFolder,
                 "Weather Preset",
                 Settings.SessionSettings.ExcludeWeatherFileFromCleanup))
        sb.AppendLine()

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


#End Region

End Class
