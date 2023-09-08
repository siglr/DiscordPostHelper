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

        _SF.CleanupDPHXTempFolder(TempDPHXUnpackFolder)

        If My.Application.CommandLineArgs.Count > 0 Then
            ' Open the file passed as an argument
            _currentFile = My.Application.CommandLineArgs(0)
            'Check if the selected file is a dph or dphx files
            If Path.GetExtension(_currentFile) = ".dphx" Then
                LoadDPHXPackage(_currentFile)
                If Settings.SessionSettings.AutoUnpack Then
                    UnpackFiles()
                End If
            End If
        End If

    End Sub

    Private Sub DPHXUnpackAndLoad_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not _abortingFirstRun Then
            Dim nbrTries As Integer = 0
            Do Until nbrTries = 10
                nbrTries += 1
                If _SF.CleanupDPHXTempFolder(TempDPHXUnpackFolder) Then
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

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click

        OpenSettingsWindow()

    End Sub

    Private Sub LoadDPHX_Click(sender As Object, e As EventArgs) Handles LoadDPHX.Click

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
        OpenFileDialog1.Filter = "Discord Post Helper Pacakge|*.dphx"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            LoadDPHXPackage(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub btnCopyFiles_Click(sender As Object, e As EventArgs) Handles btnCopyFiles.Click

        If warningMSFSRunningToolStrip.Visible Then
            If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be copied but weather preset will not be available in MSFS until it is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                UnpackFiles()
            End If
        Else
            UnpackFiles()
        End If
    End Sub

    Private Sub btnCleanup_Click(sender As Object, e As EventArgs) Handles btnCleanup.Click

        If warningMSFSRunningToolStrip.Visible Then
            If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be deleted but weather preset will remain available until MSFS is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                CleanupFiles()
            End If
        Else
            CleanupFiles()
        End If

    End Sub

    Private Sub btnLoadB21_Click(sender As Object, e As EventArgs) Handles btnLoadB21.Click

        Dim flightplanFilename As String = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.FlightPlanFilename))
        Dim weatherFilename As String = String.Empty

        If Not _allDPHData.WeatherFilename = String.Empty Then
            weatherFilename = Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(_allDPHData.WeatherFilename))
        End If

        If flightplanFilename Is String.Empty Then
        Else
            If weatherFilename = String.Empty OrElse ctrlBriefing.WeatherProfileInnerXML = String.Empty Then
                _SF.OpenB21Planner(flightplanFilename, ctrlBriefing.FlightPlanInnerXML)
            Else
                _SF.OpenB21Planner(flightplanFilename, ctrlBriefing.FlightPlanInnerXML, weatherFilename, ctrlBriefing.WeatherProfileInnerXML)
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
                        MessageBox.Show(Me, $"An error occured during the update process at this step:{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}The update did not complete.", "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            If _SF.CleanupDPHXTempFolder(TempDPHXUnpackFolder) Then
                nbrTries = 10
            Else
                Me.Refresh()
                Application.DoEvents()
            End If
        Loop

        newDPHFile = _SF.UnpackDPHXFileToTempFolder(dphxFilename, TempDPHXUnpackFolder)

        If newDPHFile = String.Empty Then
            'Invalid file loaded
            txtPackageName.Text = String.Empty
            _currentFile = String.Empty
            DisableUnpackButton()
        Else
            txtPackageName.Text = dphxFilename
            _currentFile = dphxFilename
            txtDPHFilename.Text = newDPHFile
            EnableUnpackButton(True)
        End If
        SetFormCaption(_currentFile)
        packageNameToolStrip.Text = _currentFile

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
        End If

    End Sub

    Private Sub DisableUnpackButton()
        btnCopyFiles.Enabled = False
        btnCleanup.Enabled = False
        btnLoadB21.Enabled = False
        pnlUnpackBtn.BackColor = SystemColors.Control
        btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Regular)
    End Sub

    Private Sub EnableUnpackButton(emphasize As Boolean)
        btnCopyFiles.Enabled = True
        btnCleanup.Enabled = True
        btnLoadB21.Enabled = True
        If emphasize Then
            pnlUnpackBtn.BackColor = Color.Red
            btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Bold)
        Else
            pnlUnpackBtn.BackColor = SystemColors.Control
            btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Regular)
        End If
    End Sub

    Private Sub RestoreMainFormLocationAndSize()
        Dim sizeString As String = Settings.SessionSettings.MainFormSize
        Dim locationString As String = Settings.SessionSettings.MainFormLocation

        If sizeString <> "" Then
            Dim sizeArray As String() = sizeString.TrimStart("{").TrimEnd("}").Split(",")
            Dim width As Integer = CInt(sizeArray(0).Split("=")(1))
            Dim height As Integer = CInt(sizeArray(1).Split("=")(1))
            Me.Size = New Size(width, height)
        End If

        If locationString <> "" Then
            Dim locationArray As String() = locationString.TrimStart("{").TrimEnd("}").Split(",")
            Dim x As Integer = CInt(locationArray(0).Split("=")(1))
            Dim y As Integer = CInt(locationArray(1).Split("=")(1))
            Me.Location = New Point(x, y)
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

        'Look in the other files for xcsoar file
        For Each filepath As String In _allDPHData.ExtraFiles
            If Path.GetExtension(filepath) = ".tsk" Then
                'XCSoar file
                sb.AppendLine(CopyFile(Path.GetFileName(filepath),
                                    TempDPHXUnpackFolder,
                                    Settings.SessionSettings.XCSoarTasksFolder,
                                    "XCSoar Task"))
            End If
        Next

        MessageBox.Show(sb.ToString, "Unpacking results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        EnableUnpackButton(False)

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
                        If MessageBox.Show($"The {msgToAsk} file already exists.{Environment.NewLine}{Environment.NewLine}{filename}{Environment.NewLine}{Environment.NewLine}Do you want to overwrite it?", "File already exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            proceed = True
                            messageToReturn = $"{msgToAsk} ""{filename}"" copied over existing one"
                        Else
                            proceed = False
                            messageToReturn = $"{msgToAsk} ""{filename}"" skipped by user - already exists"
                        End If
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

    Private Function DeleteFile(filename As String, sourcePath As String, msgToAsk As String) As String
        Dim fullSourceFilename As String
        Dim messageToReturn As String = String.Empty

        If Directory.Exists(sourcePath) Then
            fullSourceFilename = Path.Combine(sourcePath, filename)
            If File.Exists(fullSourceFilename) Then
                Try
                    File.Delete(fullSourceFilename)
                    messageToReturn = $"{msgToAsk} ""{filename}"" deleted"
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
                 "Flight Plan"))
        sb.AppendLine()

        'Weather file
        sb.AppendLine(DeleteFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 Settings.SessionSettings.MSFSWeatherPresetsFolder,
                 "Weather Preset"))
        sb.AppendLine()

        'Look in the other files for xcsoar file
        For Each filepath As String In _allDPHData.ExtraFiles
            If Path.GetExtension(filepath) = ".tsk" Then
                'XCSoar file
                sb.AppendLine(DeleteFile(Path.GetFileName(filepath),
                                    Settings.SessionSettings.XCSoarTasksFolder,
                                    "XCSoar Task"))
            End If
        Next

        MessageBox.Show(sb.ToString, "Cleanup results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        EnableUnpackButton(True)

    End Sub

#End Region

End Class
