Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports SIGLR.SoaringTools.CommonLibrary

Public Class Settings

    Public Shared SessionSettings As New AllSettings

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        Dim validSettings As Boolean = True
        Dim sbMsg As New StringBuilder

        'Check if valid folder
        If Not Directory.Exists(btnFlightPlanFilesFolder.Text) Then
            validSettings = False
            sbMsg.AppendLine("Invalid folder path for Flight Plans")
        End If
        If Not Directory.Exists(btnWeatherPresetsFolder.Text) Then
            validSettings = False
            sbMsg.AppendLine("Invalid folder path for Weather Presets")
        End If
        If Not Directory.Exists(btnUnpackingFolder.Text) Then
            validSettings = False
            sbMsg.AppendLine("Invalid folder path for unpacking DPHX files")
        End If
        If Not Directory.Exists(btnPackagesFolder.Text) Then
            validSettings = False
            sbMsg.AppendLine("Invalid folder path for DPHX files")
        End If

        If Not validSettings Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(sbMsg.ToString, "Cannot save settings", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        Else
            'Save settings
            SessionSettings.FlightPlansFolder = btnFlightPlanFilesFolder.Text
            SessionSettings.MSFSWeatherPresetsFolder = btnWeatherPresetsFolder.Text
            SessionSettings.XCSoarTasksFolder = btnXCSoarTasksFolder.Text
            SessionSettings.XCSoarMapsFolder = btnXCSoarMapsFolder.Text
            SessionSettings.UnpackingFolder = btnUnpackingFolder.Text
            SessionSettings.PackagesFolder = btnPackagesFolder.Text
            SessionSettings.AutoUnpack = chkEnableAutoUnpack.Checked
            SessionSettings.ExcludeFlightPlanFromCleanup = chkExcludeFlightPlanFromCleanup.Checked
            SessionSettings.ExcludeWeatherFileFromCleanup = chkExcludeWeatherFileFromCleanup.Checked
            SessionSettings.ExcludeXCSoarTaskFileFromCleanup = chkExcludeXCSoarTaskFileFromCleanup.Checked
            SessionSettings.ExcludeXCSoarMapFileFromCleanup = chkExcludeXCSoarMapFileFromCleanup.Checked

            If optOverwriteAlwaysOverwrite.Checked Then
                SessionSettings.AutoOverwriteFiles = AllSettings.AutoOverwriteOptions.AlwaysOverwrite
            End If
            If optOverwriteAlwaysSkip.Checked Then
                SessionSettings.AutoOverwriteFiles = AllSettings.AutoOverwriteOptions.AlwaysSkip
            End If
            If optOverwriteAlwaysAsk.Checked Then
                SessionSettings.AutoOverwriteFiles = AllSettings.AutoOverwriteOptions.AlwaysAsk
            End If
            SessionSettings.Save()
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click

        'Discard all changes
        If SessionSettings.Load() Then
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        Else
            Using New Centered_MessageBox(Me)
                If MessageBox.Show("You must save valid paths on the first run. Click Cancel to exit app.", "Cannot cancel on first run.", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) = DialogResult.Cancel Then
                    Me.DialogResult = DialogResult.Abort
                    Me.Close()
                End If
            End Using
        End If

    End Sub

    Private Sub btnFlightPlanFilesFolder_Click(sender As Object, e As EventArgs) Handles btnFlightPlanFilesFolder.Click

        FolderBrowserDialog1.Description = "Please select a folder where to put flight plan files (.pln)"
        FolderBrowserDialog1.ShowNewFolderButton = True

        If Directory.Exists(btnFlightPlanFilesFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnFlightPlanFilesFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnFlightPlanFilesFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnFlightPlanFilesFolder, FolderBrowserDialog1.SelectedPath)
        End If
    End Sub

    Private Sub btnWeatherPresetsFolder_Click(sender As Object, e As EventArgs) Handles btnWeatherPresetsFolder.Click
        FolderBrowserDialog1.Description = "Please select a folder where MSFS weather presets are located (.wpr)"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnWeatherPresetsFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnWeatherPresetsFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnWeatherPresetsFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnWeatherPresetsFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnUnpackingFolder_Click(sender As Object, e As EventArgs) Handles btnUnpackingFolder.Click
        FolderBrowserDialog1.Description = "Please select a temporary folder where to unpack the DPHX files"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnUnpackingFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnUnpackingFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnUnpackingFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnUnpackingFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnPackagesFolder_Click(sender As Object, e As EventArgs) Handles btnPackagesFolder.Click
        FolderBrowserDialog1.Description = "Please select the folder where to your DPHX packages are stored"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnPackagesFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnPackagesFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnPackagesFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnPackagesFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnXCSoarMapsFolder_Click(sender As Object, e As EventArgs) Handles btnXCSoarMapsFolder.Click
        FolderBrowserDialog1.Description = "Please select the XCSoar folder where the .xcm files are located"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnXCSoarMapsFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnXCSoarMapsFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnXCSoarMapsFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnXCSoarMapsFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnXCSoarTasksFolder_Click(sender As Object, e As EventArgs) Handles btnXCSoarTasksFolder.Click
        FolderBrowserDialog1.Description = "Please select the XCSoar folder where the .tsk files are located"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnXCSoarTasksFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnXCSoarTasksFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnXCSoarTasksFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnXCSoarTasksFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnXCSoarMapsFolderPaste_Click(sender As Object, e As EventArgs) Handles btnXCSoarMapsFolderPaste.Click

        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnXCSoarMapsFolder.Text = folderPath
            ToolTip1.SetToolTip(btnXCSoarMapsFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub btnXCSoarFilesFolderPaste_Click(sender As Object, e As EventArgs) Handles btnXCSoarTasksFolderPaste.Click

        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnXCSoarTasksFolder.Text = folderPath
            ToolTip1.SetToolTip(btnXCSoarTasksFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub btnFlightPlansFolderPaste_Click(sender As Object, e As EventArgs) Handles btnFlightPlansFolderPaste.Click

        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnFlightPlanFilesFolder.Text = folderPath
            ToolTip1.SetToolTip(btnFlightPlanFilesFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub btnWeatherPresetsFolderPaste_Click(sender As Object, e As EventArgs) Handles btnWeatherPresetsFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnWeatherPresetsFolder.Text = folderPath
            ToolTip1.SetToolTip(btnWeatherPresetsFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btnTempFolderPaste_Click(sender As Object, e As EventArgs) Handles btnTempFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnUnpackingFolder.Text = folderPath
            ToolTip1.SetToolTip(btnUnpackingFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btnPackagesFolderPaste_Click(sender As Object, e As EventArgs) Handles btnPackagesFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnPackagesFolder.Text = folderPath
            ToolTip1.SetToolTip(btnPackagesFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        okCancelPanel.Top = Me.Height - 103

        If Directory.Exists(SessionSettings.FlightPlansFolder) Then
            btnFlightPlanFilesFolder.Text = SessionSettings.FlightPlansFolder
            ToolTip1.SetToolTip(btnFlightPlanFilesFolder, SessionSettings.FlightPlansFolder)
        End If
        If Directory.Exists(SessionSettings.MSFSWeatherPresetsFolder) Then
            btnWeatherPresetsFolder.Text = SessionSettings.MSFSWeatherPresetsFolder
            ToolTip1.SetToolTip(btnWeatherPresetsFolder, SessionSettings.MSFSWeatherPresetsFolder)
        End If
        If Directory.Exists(SessionSettings.UnpackingFolder) Then
            btnUnpackingFolder.Text = SessionSettings.UnpackingFolder
            ToolTip1.SetToolTip(btnUnpackingFolder, SessionSettings.UnpackingFolder)
        End If
        If Directory.Exists(SessionSettings.PackagesFolder) Then
            btnPackagesFolder.Text = SessionSettings.PackagesFolder
            ToolTip1.SetToolTip(btnPackagesFolder, SessionSettings.PackagesFolder)
        End If
        If Directory.Exists(SessionSettings.XCSoarTasksFolder) Then
            btnXCSoarTasksFolder.Text = SessionSettings.XCSoarTasksFolder
            ToolTip1.SetToolTip(btnXCSoarTasksFolder, SessionSettings.XCSoarTasksFolder)
        End If
        If Directory.Exists(SessionSettings.XCSoarMapsFolder) Then
            btnXCSoarMapsFolder.Text = SessionSettings.XCSoarMapsFolder
            ToolTip1.SetToolTip(btnXCSoarMapsFolder, SessionSettings.XCSoarMapsFolder)
        End If

        Select Case SessionSettings.AutoOverwriteFiles
            Case AllSettings.AutoOverwriteOptions.AlwaysOverwrite
                optOverwriteAlwaysOverwrite.Checked = True
            Case AllSettings.AutoOverwriteOptions.AlwaysSkip
                optOverwriteAlwaysSkip.Checked = True
            Case AllSettings.AutoOverwriteOptions.AlwaysAsk
                optOverwriteAlwaysAsk.Checked = True
        End Select

        chkEnableAutoUnpack.Checked = SessionSettings.AutoUnpack
        chkExcludeFlightPlanFromCleanup.Checked = SessionSettings.ExcludeFlightPlanFromCleanup
        chkExcludeWeatherFileFromCleanup.Checked = SessionSettings.ExcludeWeatherFileFromCleanup
        chkExcludeXCSoarTaskFileFromCleanup.Checked = SessionSettings.ExcludeXCSoarTaskFileFromCleanup
        chkExcludeXCSoarMapFileFromCleanup.Checked = SessionSettings.ExcludeXCSoarMapFileFromCleanup

    End Sub

    Private Sub btnPaths_MouseUp(sender As Object, e As MouseEventArgs) Handles btnFlightPlanFilesFolder.MouseUp, btnWeatherPresetsFolder.MouseUp, btnUnpackingFolder.MouseUp, btnPackagesFolder.MouseUp, btnXCSoarTasksFolder.MouseUp, btnXCSoarMapsFolder.MouseUp
        Select Case e.Button
            Case MouseButtons.Right
                RightClickOnPathButton(sender)
        End Select
    End Sub

    Private Sub RightClickOnPathButton(theButton As Button)

        If Directory.Exists(theButton.Text) Then
            Process.Start("explorer.exe", theButton.Text)
        End If

    End Sub

End Class
