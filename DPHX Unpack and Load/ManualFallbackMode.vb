Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports System.Xml.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Partial Public Class ManualFallbackMode
    Inherits ZoomForm

    Private ReadOnly _sf As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _selectedPln As ManualFileSelection
    Private _selectedWpr As ManualFileSelection
    Private ReadOnly _copiedFiles As New List(Of CopiedFileRecord)
    Private _whitelistPresets As New List(Of WeatherPresetOption)

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Rescale()
    End Sub

    Private Sub ManualFallbackMode_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadWhitelistPresets()
        Reset()

    End Sub

    Private Sub Reset()

        cboWhitelistPresets.SelectedIndex = -1
        lblPLNFile.Text = String.Empty
        lblPLNTitle.Text = String.Empty
        lblWPRFile.Text = String.Empty
        lblWPRName.Text = String.Empty
        txtTrackerGroupName.Text = String.Empty
        _selectedPln = Nothing
        _selectedWpr = Nothing
        _copiedFiles.Clear()

    End Sub

    Private Sub cboWhitelistPresets_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboWhitelistPresets.SelectionChangeCommitted

        Dim selectedOption = TryCast(cboWhitelistPresets.SelectedItem, WeatherPresetOption)
        If selectedOption Is Nothing Then
            Return
        End If

        _selectedWpr = New ManualFileSelection With {
            .FullPath = selectedOption.FilePath,
            .DisplayName = selectedOption.PresetName
        }

        lblWPRFile.Text = Path.GetFileName(selectedOption.FilePath)
        lblWPRName.Text = selectedOption.PresetName

        'Clear any previously selected custom file
        If _selectedWpr IsNot Nothing Then
            'Nothing else to do, the whitelist file is authoritative
        End If

    End Sub

    Private Sub btnCopyGoFly_Click(sender As Object, e As EventArgs) Handles btnCopyGoFly.Click

        If _selectedPln Is Nothing Then
            ShowCenteredMessage("Please select at least a flight plan before continuing.", "No files selected")
            Return
        End If

        Dim workingFolder As String
        Try
            workingFolder = PrepareManualWorkingFolder()
        Catch ex As Exception
            ShowCenteredMessage(ex.Message, "Manual fallback mode")
            Return
        End Try

        Dim status As New frmStatus()
        status.StartPosition = FormStartPosition.CenterParent
        status.Start(Me)
        status.AppendStatusLine("Manual copy results:", True)

        Dim msfsRunning As Boolean = IsMsfsRunning()
        Dim stagedPln As String = StageSelectedFile(_selectedPln, workingFolder, status, "flight plan")
        Dim stagedWpr As String = StageSelectedFile(_selectedWpr, workingFolder, status, "weather preset")

        If Not String.IsNullOrEmpty(stagedWpr) Then
            Try
                _sf.FixWPRFormat(stagedWpr, False)
            Catch ex As Exception
                status.AppendStatusLine($"⚠️ Unable to verify the WPR file for compatibility: {ex.Message}", True)
            End Try
        End If

        Dim internalLoggerInUse As Boolean = False
        Dim unpackForm = TryCast(Me.Owner, DPHXUnpackAndLoad)
        If unpackForm Is Nothing Then
            unpackForm = Application.OpenForms.OfType(Of DPHXUnpackAndLoad)().FirstOrDefault()
        End If

        If unpackForm IsNot Nothing Then
            internalLoggerInUse = unpackForm.EnsureInternalLoggerInUse(status)
        ElseIf File.Exists(Path.Combine(Application.StartupPath, "GiveMeTheLogger.Please")) Then
            status.AppendStatusLine("Internal NB21 Logger requested but the main window is unavailable.", True)
        End If

        Try
            If Settings.SessionSettings.NB21StartAndFeed AndAlso Not internalLoggerInUse AndAlso Not String.IsNullOrWhiteSpace(stagedPln) Then
                Dim nb21Running = TaskFileHelper.EnsureNb21Running(status)
                If nb21Running Then
                    TaskFileHelper.SendPlnFileToNB21Logger(stagedPln, status)
                End If
            End If

            If Settings.SessionSettings.TrackerStartAndFeed Then
                Dim trackerRunning = TaskFileHelper.EnsureTrackerRunning(status)
                If trackerRunning Then
                    Dim trackerGroup = txtTrackerGroupName.Text.Trim()
                    TaskFileHelper.SendDataToTracker(trackerGroup, stagedPln, stagedWpr, String.Empty, status)
                End If
            End If

            If Settings.SessionSettings.Is2020Installed Then
                If Not String.IsNullOrEmpty(stagedPln) Then
                    Dim result = TaskFileHelper.CopyTaskFile(Path.GetFileName(stagedPln), workingFolder, Settings.SessionSettings.MSFS2020FlightPlansFolder, "Flight Plan for MSFS 2020", Me, msfsRunning)
                    status.AppendStatusLine(result, True)
                    TrackCopyResult(result, Path.GetFileName(stagedPln), Settings.SessionSettings.MSFS2020FlightPlansFolder, "Flight Plan for MSFS 2020")
                End If

                If Not String.IsNullOrEmpty(stagedWpr) Then
                    Dim result = TaskFileHelper.CopyTaskFile(Path.GetFileName(stagedWpr), workingFolder, Settings.SessionSettings.MSFS2020WeatherPresetsFolder, "Weather Preset for MSFS 2020", Me, msfsRunning)
                    status.AppendStatusLine(result, True)
                    TrackCopyResult(result, Path.GetFileName(stagedWpr), Settings.SessionSettings.MSFS2020WeatherPresetsFolder, "Weather Preset for MSFS 2020")
                End If
            End If

            If Settings.SessionSettings.Is2024Installed Then
                If Not String.IsNullOrEmpty(stagedPln) Then
                    Dim result = TaskFileHelper.CopyTaskFile(Path.GetFileName(stagedPln), workingFolder, Settings.SessionSettings.MSFS2024FlightPlansFolder, "Flight Plan for MSFS 2024", Me, msfsRunning)
                    status.AppendStatusLine(result, True)
                    TrackCopyResult(result, Path.GetFileName(stagedPln), Settings.SessionSettings.MSFS2024FlightPlansFolder, "Flight Plan for MSFS 2024")

                    If Settings.SessionSettings.EnableEFBFlightPlanCreation Then
                        Dim efbResult = TaskFileHelper.CreatePlnForEfb(Path.GetFileName(stagedPln), workingFolder, Settings.SessionSettings.MSFS2024FlightPlansFolder, "EFB Flight Plan for MSFS 2024", Me, msfsRunning)
                        status.AppendStatusLine(efbResult, True)
                        TrackCopyResult(efbResult, $"{Path.GetFileNameWithoutExtension(stagedPln)}_EFB{Path.GetExtension(stagedPln)}", Settings.SessionSettings.MSFS2024FlightPlansFolder, "EFB Flight Plan for MSFS 2024")
                    End If
                End If

                If Not String.IsNullOrEmpty(stagedWpr) Then
                    Dim result = TaskFileHelper.CopyTaskFile(Path.GetFileName(stagedWpr), workingFolder, Settings.SessionSettings.MSFS2024WeatherPresetsFolder, "Weather Preset for MSFS 2024", Me, msfsRunning)
                    status.AppendStatusLine(result, True)
                    TrackCopyResult(result, Path.GetFileName(stagedWpr), Settings.SessionSettings.MSFS2024WeatherPresetsFolder, "Weather Preset for MSFS 2024")
                End If
            End If

            status.AppendStatusLine("Manual fallback copy completed. Close this window or clean up when done.", False)
        Finally
            status.Done()
        End Try

    End Sub

    Private Sub btnClearFiles_Click(sender As Object, e As EventArgs) Handles btnClearFiles.Click

        If _copiedFiles.Count = 0 Then
            ShowCenteredMessage("No files copied during this manual session. There is nothing to clean up.", "Cleanup")
            Return
        End If

        Dim status As New frmStatus()
        status.StartPosition = FormStartPosition.CenterParent
        status.Start(Me)
        status.AppendStatusLine("Manual cleanup results:", True)

        Dim msfsRunning = IsMsfsRunning()

        For Each record In _copiedFiles
            Dim result = TaskFileHelper.DeleteTaskFile(record.FileName, record.DestinationFolder, record.Label, False, msfsRunning)
            status.AppendStatusLine(result, True)
        Next

        If Settings.SessionSettings.TrackerStartAndFeed Then
            status.AppendStatusLine(TaskFileHelper.ExecuteTrackerTaskFolderCleanup(), True)
        End If

        status.AppendStatusLine("Cleanup completed.", False)
        status.Done()

        _copiedFiles.Clear()

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

        Me.Close()

    End Sub

    Private Sub txtTrackerGroupName_Leave(sender As Object, e As EventArgs) Handles txtTrackerGroupName.Leave

        txtTrackerGroupName.Text = txtTrackerGroupName.Text.Trim()

    End Sub

    Private Sub btnSelectPLN_Click(sender As Object, e As EventArgs) Handles btnSelectPLN.Click

        Using ofd As New OpenFileDialog()
            ofd.Filter = "Flight plan (*.pln)|*.pln"
            ofd.Title = "Select a Flight Plan"
            ofd.Multiselect = False

            If ofd.ShowDialog(Me) = DialogResult.OK Then
                Dim selection = LoadPlnSelection(ofd.FileName)
                If selection IsNot Nothing Then
                    _selectedPln = selection
                    lblPLNFile.Text = selection.FileName
                    lblPLNTitle.Text = selection.DisplayName
                End If
            End If
        End Using

    End Sub

    Private Sub btnSelectWPR_Click(sender As Object, e As EventArgs) Handles btnSelectWPR.Click

        Using ofd As New OpenFileDialog()
            ofd.Filter = "Weather preset (*.wpr)|*.wpr"
            ofd.Title = "Select a Weather Preset"
            ofd.Multiselect = False

            If ofd.ShowDialog(Me) = DialogResult.OK Then
                Dim selection = LoadWprSelection(ofd.FileName, True)
                If selection IsNot Nothing Then
                    _selectedWpr = selection
                    lblWPRFile.Text = selection.FileName
                    lblWPRName.Text = selection.DisplayName
                    cboWhitelistPresets.SelectedIndex = -1
                End If
            End If
        End Using

    End Sub

    Private Sub LoadWhitelistPresets()
        _whitelistPresets = New List(Of WeatherPresetOption)()
        Dim whitelistFolder = Path.Combine(Application.StartupPath, "Whitelist")

        If Directory.Exists(whitelistFolder) Then
            For Each filePath In Directory.GetFiles(whitelistFolder, "*.wpr", SearchOption.TopDirectoryOnly)
                Dim selection = LoadWprSelection(filePath, False)
                If selection IsNot Nothing Then
                    _whitelistPresets.Add(New WeatherPresetOption With {
                        .FilePath = filePath,
                        .PresetName = selection.DisplayName
                    })
                End If
            Next
        End If

        cboWhitelistPresets.DataSource = Nothing
        If _whitelistPresets.Count > 0 Then
            cboWhitelistPresets.DataSource = _whitelistPresets
            cboWhitelistPresets.DisplayMember = "DisplayText"
            cboWhitelistPresets.ValueMember = "FilePath"
            cboWhitelistPresets.Enabled = True
        Else
            cboWhitelistPresets.Enabled = False
        End If
    End Sub

    Private Function LoadPlnSelection(filePath As String) As ManualFileSelection
        Try
            Dim doc = XDocument.Load(filePath)
            Dim titleElement = doc.Descendants("Title").FirstOrDefault()
            Dim title = If(titleElement IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(titleElement.Value), titleElement.Value.Trim(), Path.GetFileNameWithoutExtension(filePath))

            Return New ManualFileSelection With {
                .FullPath = filePath,
                .DisplayName = title
            }
        Catch ex As Exception
            ShowCenteredMessage($"Unable to read the flight plan file: {ex.Message}", "Flight plan error")
            Return Nothing
        End Try
    End Function

    Private Function LoadWprSelection(filePath As String, showErrors As Boolean) As ManualFileSelection
        Try
            Dim doc = XDocument.Load(filePath)
            Dim nameElement = doc.Descendants("Name").FirstOrDefault()
            Dim presetName = If(nameElement IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(nameElement.Value), nameElement.Value.Trim(), Path.GetFileNameWithoutExtension(filePath))

            Return New ManualFileSelection With {
                .FullPath = filePath,
                .DisplayName = presetName
            }
        Catch ex As Exception
            If showErrors Then
                ShowCenteredMessage($"Unable to read the weather preset file: {ex.Message}", "Weather preset error")
            End If
            Return Nothing
        End Try
    End Function

    Private Sub ShowCenteredMessage(message As String, caption As String)
        Using New Centered_MessageBox(Me)
            MessageBox.Show(Me, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    Private Function PrepareManualWorkingFolder() As String
        Dim folder = Path.Combine(Settings.SessionSettings.UnpackingFolder, "ManualFallback")

        Try
            If Directory.Exists(folder) Then
                For Each theFile As String In Directory.GetFiles(folder)
                    File.Delete(theFile)
                Next
                For Each subfolder As String In Directory.GetDirectories(folder)
                    Directory.Delete(subfolder, True)
                Next
            Else
                Directory.CreateDirectory(folder)
            End If
        Catch ex As Exception
            Throw New InvalidOperationException($"Unable to prepare the manual working folder (""{folder}"")): {ex.Message}", ex)
        End Try

        Return folder
    End Function

    Private Function StageSelectedFile(selection As ManualFileSelection, workingFolder As String, status As frmStatus, label As String) As String
        If selection Is Nothing OrElse String.IsNullOrWhiteSpace(selection.FullPath) Then
            Return String.Empty
        End If

        Try
            Dim destination = Path.Combine(workingFolder, Path.GetFileName(selection.FullPath))
            File.Copy(selection.FullPath, destination, True)
            Return destination
        Catch ex As Exception
            status.AppendStatusLine($"❌ Unable to stage the {label}: {ex.Message}", True)
            Return String.Empty
        End Try
    End Function

    Private Sub TrackCopyResult(message As String, fileName As String, destinationFolder As String, label As String)
        If String.IsNullOrWhiteSpace(message) OrElse String.IsNullOrWhiteSpace(fileName) OrElse String.IsNullOrWhiteSpace(destinationFolder) Then
            Return
        End If

        Dim normalized = message.ToLowerInvariant()
        If normalized.Contains("copied") OrElse normalized.Contains("replaced") Then
            Dim alreadyTracked = _copiedFiles.Any(Function(c) c.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase) _
                                                    AndAlso c.DestinationFolder.Equals(destinationFolder, StringComparison.OrdinalIgnoreCase))
            If Not alreadyTracked Then
                _copiedFiles.Add(New CopiedFileRecord With {
                    .FileName = fileName,
                    .DestinationFolder = destinationFolder,
                    .Label = label
                })
            End If
        End If
    End Sub

    Private Function IsMsfsRunning() As Boolean
        Return Process.GetProcessesByName("FlightSimulator").Any() OrElse Process.GetProcessesByName("FlightSimulator2024").Any()
    End Function
    Private Class ManualFileSelection
        Public Property FullPath As String
        Public Property DisplayName As String

        Public ReadOnly Property FileName As String
            Get
                Return If(String.IsNullOrEmpty(FullPath), String.Empty, Path.GetFileName(FullPath))
            End Get
        End Property
    End Class

    Private Class CopiedFileRecord
        Public Property FileName As String
        Public Property DestinationFolder As String
        Public Property Label As String
    End Class

    Private Class WeatherPresetOption
        Public Property FilePath As String
        Public Property PresetName As String

        Public ReadOnly Property DisplayText As String
            Get
                Dim fileName = Path.GetFileName(FilePath)
                If String.IsNullOrWhiteSpace(PresetName) Then
                    Return fileName
                End If
                Return $"{PresetName} — {fileName}"
            End Get
        End Property
    End Class
End Class
