Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.Win32
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Public Class Settings

    Public Shared SessionSettings As New AllSettings

    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Rescale()
        SupportingFeatures.CenterFormOnOwner(Owner, Me)

        okCancelPanel.Top = Me.Height - 103

        If SessionSettings.MSFS2020Microsoft OrElse SessionSettings.MSFS2020Steam Then
            chkMSFS2020.Checked = True
            opt2020Steam.Checked = SessionSettings.MSFS2020Steam
            opt2020Microsoft.Checked = SessionSettings.MSFS2020Microsoft
        End If
        If SessionSettings.MSFS2024Microsoft OrElse SessionSettings.MSFS2024Steam Then
            chkMSFS2024.Checked = True
            opt2024Steam.Checked = SessionSettings.MSFS2024Steam
            opt2024Microsoft.Checked = SessionSettings.MSFS2024Microsoft
        End If

        If Directory.Exists(SessionSettings.MSFS2020FlightPlansFolder) Then
            btnMSFS2020FlightPlanFilesFolder.Text = SessionSettings.MSFS2020FlightPlansFolder
            ToolTip1.SetToolTip(btnMSFS2020FlightPlanFilesFolder, SessionSettings.MSFS2020FlightPlansFolder)
        End If
        If Directory.Exists(SessionSettings.MSFS2020WeatherPresetsFolder) Then
            btnMSFS2020WeatherPresetsFolder.Text = SessionSettings.MSFS2020WeatherPresetsFolder
            ToolTip1.SetToolTip(btnMSFS2020WeatherPresetsFolder, SessionSettings.MSFS2020WeatherPresetsFolder)
        End If
        If Directory.Exists(SessionSettings.MSFS2024FlightPlansFolder) Then
            btnMSFS2024FlightPlanFilesFolder.Text = SessionSettings.MSFS2024FlightPlansFolder
            ToolTip1.SetToolTip(btnMSFS2024FlightPlanFilesFolder, SessionSettings.MSFS2024FlightPlansFolder)
        End If
        If Directory.Exists(SessionSettings.MSFS2024WeatherPresetsFolder) Then
            btnMSFS2024WeatherPresetsFolder.Text = SessionSettings.MSFS2024WeatherPresetsFolder
            ToolTip1.SetToolTip(btnMSFS2024WeatherPresetsFolder, SessionSettings.MSFS2024WeatherPresetsFolder)
        End If
        If Directory.Exists(SessionSettings.UnpackingFolder) Then
            btnUnpackingFolder.Text = SessionSettings.UnpackingFolder
            ToolTip1.SetToolTip(btnUnpackingFolder, SessionSettings.UnpackingFolder)
        End If
        If Directory.Exists(SessionSettings.PackagesFolder) Then
            btnPackagesFolder.Text = SessionSettings.PackagesFolder
            ToolTip1.SetToolTip(btnPackagesFolder, SessionSettings.PackagesFolder)
        End If
        If Directory.Exists(SessionSettings.NB21IGCFolder) Then
            btnNB21IGCFolder.Text = SessionSettings.NB21IGCFolder
            ToolTip1.SetToolTip(btnNB21IGCFolder, SessionSettings.NB21IGCFolder)
        End If
        If Directory.Exists(SessionSettings.NB21EXEFolder) Then
            btnNB21EXEFolder.Text = SessionSettings.NB21EXEFolder
            ToolTip1.SetToolTip(btnNB21EXEFolder, SessionSettings.NB21EXEFolder)
        End If
        If Directory.Exists(SessionSettings.TrackerEXEFolder) Then
            btnTrackerEXEFolder.Text = SessionSettings.TrackerEXEFolder
            ToolTip1.SetToolTip(btnTrackerEXEFolder, SessionSettings.TrackerEXEFolder)
        End If
        If Directory.Exists(SessionSettings.XCSoarTasksFolder) Then
            btnXCSoarTasksFolder.Text = SessionSettings.XCSoarTasksFolder
            ToolTip1.SetToolTip(btnXCSoarTasksFolder, SessionSettings.XCSoarTasksFolder)
        End If
        If Directory.Exists(SessionSettings.XCSoarMapsFolder) Then
            btnXCSoarMapsFolder.Text = SessionSettings.XCSoarMapsFolder
            ToolTip1.SetToolTip(btnXCSoarMapsFolder, SessionSettings.XCSoarMapsFolder)
        End If

        Dim NB21port As Integer
        If Integer.TryParse(SessionSettings.NB21LocalWSPort, NB21port) AndAlso NB21port >= 0 AndAlso NB21port <= 65535 Then
            txtNB21LocalWSPort.Text = SessionSettings.NB21LocalWSPort
        End If
        chkEnableNB21StartAndFeed.Checked = SessionSettings.NB21StartAndFeed

        Dim Trackerport As Integer
        If Integer.TryParse(SessionSettings.TrackerLocalWSPort, Trackerport) AndAlso Trackerport >= 0 AndAlso Trackerport <= 65535 Then
            txtTrackerLocalWSPort.Text = SessionSettings.TrackerLocalWSPort
        End If
        chkEnableTrackerStartAndFeed.Checked = SessionSettings.TrackerStartAndFeed

        Dim DPHXport As Integer
        If Integer.TryParse(SessionSettings.LocalWebServerPort, DPHXport) AndAlso DPHXport >= 0 AndAlso DPHXport <= 65535 Then
            txtDPHXLocalPort.Text = SessionSettings.LocalWebServerPort
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
        chkExclude2020FlightPlanFromCleanup.Checked = SessionSettings.Exclude2020FlightPlanFromCleanup
        chkExclude2020WeatherFileFromCleanup.Checked = SessionSettings.Exclude2020WeatherFileFromCleanup
        chkExclude2024FlightPlanFromCleanup.Checked = SessionSettings.Exclude2024FlightPlanFromCleanup
        chkExclude2024WeatherFileFromCleanup.Checked = SessionSettings.Exclude2024WeatherFileFromCleanup
        chkExcludeXCSoarTaskFileFromCleanup.Checked = SessionSettings.ExcludeXCSoarTaskFileFromCleanup
        chkExcludeXCSoarMapFileFromCleanup.Checked = SessionSettings.ExcludeXCSoarMapFileFromCleanup

        cboWSGIntegration.SelectedIndex = SessionSettings.WSGIntegration
        chkWSGExceptOpeningDPHX.Checked = SessionSettings.WSGIgnoreWhenOpeningDPHX
        chkWSGListenerAutoStart.Checked = SessionSettings.WSGListenerAutoStart

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        Dim validSettings As Boolean = True
        Dim sbMsg As New StringBuilder

        'Check at least one installation
        If (chkMSFS2020.Checked AndAlso (opt2020Microsoft.Checked OrElse opt2020Steam.Checked)) OrElse
           (chkMSFS2024.Checked AndAlso (opt2024Microsoft.Checked OrElse opt2024Steam.Checked)) Then
            'OK
            If chkMSFS2020.Checked AndAlso (Not opt2020Microsoft.Checked) AndAlso (Not opt2020Steam.Checked) Then
                'Missing source for 2020
                validSettings = False
                sbMsg.AppendLine("Select the source for MSFS 2020 installation")
            End If
            If chkMSFS2024.Checked AndAlso (Not opt2024Microsoft.Checked) AndAlso (Not opt2024Steam.Checked) Then
                'Missing source for 2024
                validSettings = False
                sbMsg.AppendLine("Select the source for MSFS 2024 installation")
            End If
        Else
            'No installation at all
            validSettings = False
            sbMsg.AppendLine("Select at least one MSFS installation")
        End If

        If chkMSFS2020.Checked Then
            If Not Directory.Exists(btnMSFS2020FlightPlanFilesFolder.Text) Then
                validSettings = False
                sbMsg.AppendLine("Invalid folder path for 2020 Flight Plans")
            End If
            If Not Directory.Exists(btnMSFS2020WeatherPresetsFolder.Text) Then
                validSettings = False
                sbMsg.AppendLine("Invalid folder path for 2020 Weather Presets")
            End If
        End If

        If chkMSFS2024.Checked Then
            If Not Directory.Exists(btnMSFS2024FlightPlanFilesFolder.Text) Then
                validSettings = False
                sbMsg.AppendLine("Invalid folder path for 2024 Flight Plans")
            End If
            If Not Directory.Exists(btnMSFS2024WeatherPresetsFolder.Text) Then
                validSettings = False
                sbMsg.AppendLine("Invalid folder path for 2024 Weather Presets")
            End If
        End If

        'Check if valid folder
        If Not Directory.Exists(btnUnpackingFolder.Text) Then
            validSettings = False
            sbMsg.AppendLine("Invalid folder path for unpacking DPHX files")
        End If
        If Not Directory.Exists(btnPackagesFolder.Text) Then
            validSettings = False
            sbMsg.AppendLine("Invalid folder path for DPHX files")
        End If

        'check that temporary and DPHX packages are not the same folder
        If Directory.Exists(btnUnpackingFolder.Text) AndAlso Directory.Exists(btnPackagesFolder.Text) Then
            If btnUnpackingFolder.Text.Trim = btnPackagesFolder.Text.Trim Then
                validSettings = False
                sbMsg.AppendLine("The unpacking and DPHX packages folders must not be the same")
            End If
        End If

        'If NB21StartAndFeed enabled, a valid path must be provided for the EXE
        If chkEnableNB21StartAndFeed.Checked Then
            If Not File.Exists(Path.Combine(btnNB21EXEFolder.Text, "NB21_logger.exe")) Then
                validSettings = False
                sbMsg.AppendLine("Invalid folder path for the NB21 Logger executable")
            End If
        End If

        'If TrackerStartAndFeed enabled, a valid path must be provided for the EXE
        If chkEnableTrackerStartAndFeed.Checked Then
            If Not File.Exists(Path.Combine(btnTrackerEXEFolder.Text, "SSC-Tracker.exe")) Then
                validSettings = False
                sbMsg.AppendLine("Invalid folder path for the Tracker executable")
            End If
        End If

        'Check for valid NB21 port value
        Dim NB21Port As Integer
        If Not (Integer.TryParse(txtNB21LocalWSPort.Text, NB21Port) AndAlso NB21Port >= 0 AndAlso NB21Port <= 65535) Then
            validSettings = False
            sbMsg.AppendLine("Invalid port value for the NB21 Logger's local web server")
        End If

        'Check for valid Tracker port value
        Dim TrackerPort As Integer
        If Not (Integer.TryParse(txtTrackerLocalWSPort.Text, TrackerPort) AndAlso TrackerPort >= 0 AndAlso TrackerPort <= 65535) Then
            validSettings = False
            sbMsg.AppendLine("Invalid port value for the Tracker's local web server")
        End If

        Dim DPHXPort As Integer
        If Not (Integer.TryParse(txtDPHXLocalPort.Text, DPHXPort) AndAlso DPHXPort >= 0 AndAlso DPHXPort <= 65535) Then
            validSettings = False
            sbMsg.AppendLine("Invalid port value for the DPHX local web server")
        End If
        If (DPHXPort = NB21Port) OrElse (DPHXPort = TrackerPort) OrElse (NB21Port = TrackerPort) Then
            validSettings = False
            sbMsg.AppendLine("Ports for DPHX, NB21 and Tracker must be different")
        End If

        If Not validSettings Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show(sbMsg.ToString, "Cannot save settings", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        Else
            'Save settings
            SessionSettings.MSFS2020Microsoft = opt2020Microsoft.Checked
            SessionSettings.MSFS2020Steam = opt2020Steam.Checked
            SessionSettings.MSFS2024Microsoft = opt2024Microsoft.Checked
            SessionSettings.MSFS2024Steam = opt2024Steam.Checked
            SessionSettings.MSFS2020FlightPlansFolder = btnMSFS2020FlightPlanFilesFolder.Text
            SessionSettings.MSFS2020WeatherPresetsFolder = btnMSFS2020WeatherPresetsFolder.Text
            SessionSettings.MSFS2024FlightPlansFolder = btnMSFS2024FlightPlanFilesFolder.Text
            SessionSettings.MSFS2024WeatherPresetsFolder = btnMSFS2024WeatherPresetsFolder.Text
            SessionSettings.XCSoarTasksFolder = btnXCSoarTasksFolder.Text
            SessionSettings.XCSoarMapsFolder = btnXCSoarMapsFolder.Text
            SessionSettings.UnpackingFolder = btnUnpackingFolder.Text
            SessionSettings.PackagesFolder = btnPackagesFolder.Text
            SessionSettings.NB21IGCFolder = btnNB21IGCFolder.Text
            SessionSettings.NB21EXEFolder = btnNB21EXEFolder.Text
            SessionSettings.NB21LocalWSPort = txtNB21LocalWSPort.Text
            SessionSettings.NB21StartAndFeed = chkEnableNB21StartAndFeed.Checked
            SessionSettings.TrackerEXEFolder = btnTrackerEXEFolder.Text
            SessionSettings.TrackerLocalWSPort = txtTrackerLocalWSPort.Text
            SessionSettings.TrackerStartAndFeed = chkEnableTrackerStartAndFeed.Checked
            SessionSettings.LocalWebServerPort = txtDPHXLocalPort.Text
            SessionSettings.AutoUnpack = chkEnableAutoUnpack.Checked
            SessionSettings.Exclude2020FlightPlanFromCleanup = chkExclude2020FlightPlanFromCleanup.Checked
            SessionSettings.Exclude2020WeatherFileFromCleanup = chkExclude2020WeatherFileFromCleanup.Checked
            SessionSettings.Exclude2024FlightPlanFromCleanup = chkExclude2024FlightPlanFromCleanup.Checked
            SessionSettings.Exclude2024WeatherFileFromCleanup = chkExclude2024WeatherFileFromCleanup.Checked
            SessionSettings.ExcludeXCSoarTaskFileFromCleanup = chkExcludeXCSoarTaskFileFromCleanup.Checked
            SessionSettings.ExcludeXCSoarMapFileFromCleanup = chkExcludeXCSoarMapFileFromCleanup.Checked
            If cboWSGIntegration.SelectedIndex = -1 Then
                cboWSGIntegration.SelectedIndex = 0 'Default to None if no selection made   
            End If
            SessionSettings.WSGIntegration = cboWSGIntegration.SelectedIndex
            SessionSettings.WSGIgnoreWhenOpeningDPHX = chkWSGExceptOpeningDPHX.Checked

            'If the auto-start setting has changed, we need to add or remove the listener from the auto-run with Windows
            If chkWSGListenerAutoStart.Checked AndAlso Not SessionSettings.WSGListenerAutoStart Then
                ' The Auto-Start has been enabled, make sure we set the listener to auto-start with Windows
                SetWSGListenerAutoStart(True)
            ElseIf Not chkWSGListenerAutoStart.Checked AndAlso SessionSettings.WSGListenerAutoStart Then
                ' The Auto-Start has been disabled, make sure we remove the listener from auto-start with Windows
                SetWSGListenerAutoStart(False)
            End If
            SessionSettings.WSGListenerAutoStart = chkWSGListenerAutoStart.Checked

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
                If MessageBox.Show("You must select your MSFS installation And save valid paths. Click Cancel to exit app.", "Cannot cancel with invalid settings.", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) = DialogResult.Cancel Then
                    Me.DialogResult = DialogResult.Abort
                    Me.Close()
                End If
            End Using
        End If

    End Sub

    Private Sub btn2020FlightPlanFilesFolder_Click(sender As Object, e As EventArgs) Handles btnMSFS2020FlightPlanFilesFolder.Click

        FolderBrowserDialog1.Description = "Please select a folder where to put flight plan files for MSFS 2020 (.pln)"
        FolderBrowserDialog1.ShowNewFolderButton = True

        If Directory.Exists(btnMSFS2020FlightPlanFilesFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnMSFS2020FlightPlanFilesFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnMSFS2020FlightPlanFilesFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnMSFS2020FlightPlanFilesFolder, FolderBrowserDialog1.SelectedPath)
        End If
    End Sub

    Private Sub btn2024FlightPlanFilesFolder_Click(sender As Object, e As EventArgs) Handles btnMSFS2024FlightPlanFilesFolder.Click

        FolderBrowserDialog1.Description = "Please Select a folder where To put flight plan files For MSFS 2024 (.pln) Then"
        FolderBrowserDialog1.ShowNewFolderButton = True

        If Directory.Exists(btnMSFS2024FlightPlanFilesFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnMSFS2024FlightPlanFilesFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnMSFS2024FlightPlanFilesFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnMSFS2024FlightPlanFilesFolder, FolderBrowserDialog1.SelectedPath)
        End If
    End Sub

    Private Sub btn2020WeatherPresetsFolder_Click(sender As Object, e As EventArgs) Handles btnMSFS2020WeatherPresetsFolder.Click
        FolderBrowserDialog1.Description = "Please Select a folder where MSFS 2020 weather presets are located (.wpr)"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnMSFS2020WeatherPresetsFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnMSFS2020WeatherPresetsFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnMSFS2020WeatherPresetsFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnMSFS2020WeatherPresetsFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btn2024WeatherPresetsFolder_Click(sender As Object, e As EventArgs) Handles btnMSFS2024WeatherPresetsFolder.Click
        FolderBrowserDialog1.Description = "Please Select a folder where MSFS 2024 weather presets are located (.wpr)"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnMSFS2024WeatherPresetsFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnMSFS2024WeatherPresetsFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnMSFS2024WeatherPresetsFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnMSFS2024WeatherPresetsFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnUnpackingFolder_Click(sender As Object, e As EventArgs) Handles btnUnpackingFolder.Click
        FolderBrowserDialog1.Description = "Please Select a temporary folder where To unpack the DPHX files"
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
        FolderBrowserDialog1.Description = "Please Select the folder where your DPHX packages are stored"
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

    Private Sub btnNB21IGCFolder_Click(sender As Object, e As EventArgs) Handles btnNB21IGCFolder.Click
        FolderBrowserDialog1.Description = "Please Select the folder where your NB21 Logger puts the IGC log files"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnNB21IGCFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnNB21IGCFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnNB21IGCFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnNB21IGCFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnNB21EXEFolder_Click(sender As Object, e As EventArgs) Handles btnNB21EXEFolder.Click
        FolderBrowserDialog1.Description = "Please Select the folder where your NB21 Logger executable (EXE) file is"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnNB21EXEFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnNB21EXEFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnNB21EXEFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnNB21EXEFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnTrackerEXEFolder_Click(sender As Object, e As EventArgs) Handles btnTrackerEXEFolder.Click
        FolderBrowserDialog1.Description = "Please Select the folder where your Tracker executable (EXE) file is"
        FolderBrowserDialog1.ShowNewFolderButton = True
        If Directory.Exists(btnTrackerEXEFolder.Text) Then
            FolderBrowserDialog1.SelectedPath = btnTrackerEXEFolder.Text
        Else
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        End If

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' User selected a folder and clicked OK
            btnTrackerEXEFolder.Text = FolderBrowserDialog1.SelectedPath
            ToolTip1.SetToolTip(btnTrackerEXEFolder, FolderBrowserDialog1.SelectedPath)
        End If

    End Sub

    Private Sub btnXCSoarMapsFolder_Click(sender As Object, e As EventArgs) Handles btnXCSoarMapsFolder.Click
        FolderBrowserDialog1.Description = "Please Select the XCSoar folder where the .xcm files are located"
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
        FolderBrowserDialog1.Description = "Please Select the XCSoar folder where the .tsk files are located"
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
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub btn2020FlightPlansFolderPaste_Click(sender As Object, e As EventArgs) Handles btn2020FlightPlansFolderPaste.Click

        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnMSFS2020FlightPlanFilesFolder.Text = folderPath
            ToolTip1.SetToolTip(btnMSFS2020FlightPlanFilesFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub btn2024FlightPlansFolderPaste_Click(sender As Object, e As EventArgs) Handles btn2024FlightPlansFolderPaste.Click

        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnMSFS2024FlightPlanFilesFolder.Text = folderPath
            ToolTip1.SetToolTip(btnMSFS2024FlightPlanFilesFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If
    End Sub

    Private Sub btn2020WeatherPresetsFolderPaste_Click(sender As Object, e As EventArgs) Handles btn2020WeatherPresetsFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnMSFS2020WeatherPresetsFolder.Text = folderPath
            ToolTip1.SetToolTip(btnMSFS2020WeatherPresetsFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btn2024WeatherPresetsFolderPaste_Click(sender As Object, e As EventArgs) Handles btn2024WeatherPresetsFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnMSFS2024WeatherPresetsFolder.Text = folderPath
            ToolTip1.SetToolTip(btnMSFS2024WeatherPresetsFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btnNB21IGCFolderPaste_Click(sender As Object, e As EventArgs) Handles btnNB21IGCFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnNB21IGCFolder.Text = folderPath
            ToolTip1.SetToolTip(btnNB21IGCFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path In the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btnNB21EXEFolderPaste_Click(sender As Object, e As EventArgs) Handles btnNB21EXEFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnNB21EXEFolder.Text = folderPath
            ToolTip1.SetToolTip(btnNB21EXEFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btnTrackerEXEFolderPaste_Click(sender As Object, e As EventArgs) Handles btnTrackerEXEFolderPaste.Click
        Dim folderPath As String = Clipboard.GetText()
        If Directory.Exists(folderPath) Then
            ' folderPath is a valid folder
            btnTrackerEXEFolder.Text = folderPath
            ToolTip1.SetToolTip(btnTrackerEXEFolder, folderPath)
        Else
            ' folderPath is not a valid folder
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Invalid folder path in the clipboard", "Cannot paste", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Private Sub btnPaths_MouseUp(sender As Object, e As MouseEventArgs) Handles btnMSFS2020FlightPlanFilesFolder.MouseUp, btnMSFS2020WeatherPresetsFolder.MouseUp, btnUnpackingFolder.MouseUp, btnPackagesFolder.MouseUp, btnXCSoarTasksFolder.MouseUp, btnXCSoarMapsFolder.MouseUp, btnNB21IGCFolder.MouseUp, btnNB21EXEFolder.MouseUp, btnTrackerEXEFolder.MouseUp, btnMSFS2024WeatherPresetsFolder.MouseUp, btnMSFS2024FlightPlanFilesFolder.MouseUp
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

    Private Sub btnXCSoarTasksFolderClear_Click(sender As Object, e As EventArgs) Handles btnXCSoarTasksFolderClear.Click
        Settings.SessionSettings.ClearXCSoarTasks()
        btnXCSoarTasksFolder.Text = "Select the folder containing XCSoar tasks (.tsk) (Optional)"
    End Sub

    Private Sub btnXCSoarMapsFolderClear_Click(sender As Object, e As EventArgs) Handles btnXCSoarMapsFolderClear.Click
        Settings.SessionSettings.ClearXCSoarMaps()
        btnXCSoarMapsFolder.Text = "Select the folder containing XCSoar maps (.xcm) (Optional)"
    End Sub

    Private Sub btnNB21IGCFolderClear_Click(sender As Object, e As EventArgs) Handles btnNB21IGCFolderClear.Click
        Settings.SessionSettings.ClearNB21IGCFolder()
        btnNB21IGCFolder.Text = "Select the folder containing the logger flights IGC files (Optional)"
    End Sub

    Private Sub btnNB21EXEFolderClear_Click(sender As Object, e As EventArgs) Handles btnNB21EXEFolderClear.Click
        Settings.SessionSettings.ClearNB21EXEFolder()
        btnNB21EXEFolder.Text = "Select the folder containing the logger's EXE file (optional)"
    End Sub

    Private Sub btnNB21ResetPort_Click(sender As Object, e As EventArgs) Handles btnNB21ResetPort.Click
        txtNB21LocalWSPort.Text = "54178"
    End Sub

    Private Sub btnTrackerEXEFolderClear_Click(sender As Object, e As EventArgs) Handles btnTrackerEXEFolderClear.Click
        Settings.SessionSettings.ClearTrackerEXEFolder()
        btnTrackerEXEFolder.Text = "Select the folder containing the Tracker's EXE file (optional)"
    End Sub

    Private Sub btnTrackerResetPort_Click(sender As Object, e As EventArgs) Handles btnTrackerResetPort.Click
        txtTrackerLocalWSPort.Text = "55055"
    End Sub

    Private Sub btnResetDPHXLocalPort_Click(sender As Object, e As EventArgs) Handles btnResetDPHXLocalPort.Click
        txtDPHXLocalPort.Text = "54513"
    End Sub

    Private Sub chkMSFS_CheckedChanged(sender As Object, e As EventArgs) Handles chkMSFS2020.CheckedChanged, chkMSFS2024.CheckedChanged

        If chkMSFS2020.Checked Then
            pnl2020Options.Enabled = True
            pnlMSFS2020FlightPlanFilesFolder.Enabled = True
            pnlMSFS2020WeatherPresetsFolder.Enabled = True
        Else
            pnl2020Options.Enabled = False
            opt2020Microsoft.Checked = False
            opt2020Steam.Checked = False
            pnlMSFS2020FlightPlanFilesFolder.Enabled = False
            pnlMSFS2020WeatherPresetsFolder.Enabled = False
        End If

        If chkMSFS2024.Checked Then
            pnl2024Options.Enabled = True
            pnlMSFS2024FlightPlanFilesFolder.Enabled = True
            pnlMSFS2024WeatherPresetsFolder.Enabled = True
        Else
            pnl2024Options.Enabled = False
            opt2024Microsoft.Checked = False
            opt2024Steam.Checked = False
            pnlMSFS2024FlightPlanFilesFolder.Enabled = False
            pnlMSFS2024WeatherPresetsFolder.Enabled = False
        End If

        If chkMSFS2020.Checked OrElse chkMSFS2024.Checked Then
            btnDetectMSFSFolders.Enabled = True
        Else
            btnDetectMSFSFolders.Enabled = False
        End If

        CheckIfDetectFoldersCanBeEnabled()

    End Sub

    Private Sub optSteamMicrosfot_CheckedChanged(sender As Object, e As EventArgs) Handles opt2020Steam.CheckedChanged, opt2020Microsoft.CheckedChanged, opt2024Steam.CheckedChanged, opt2024Microsoft.CheckedChanged
        CheckIfDetectFoldersCanBeEnabled()
    End Sub

    Private Sub CheckIfDetectFoldersCanBeEnabled()
        If (chkMSFS2020.Checked AndAlso (opt2020Steam.Checked OrElse opt2020Microsoft.Checked)) OrElse
           (chkMSFS2024.Checked AndAlso (opt2024Steam.Checked OrElse opt2024Microsoft.Checked)) Then
            btnDetectMSFSFolders.Enabled = True
        Else
            btnDetectMSFSFolders.Enabled = False
        End If
    End Sub

    Private Sub btnDetectMSFSFolders_Click(sender As Object, e As EventArgs) Handles btnDetectMSFSFolders.Click

        Dim folderPathToCheck As String
        Dim basePath As String
        Dim errorMessages As String = String.Empty

        If opt2020Microsoft.Checked Then
            'Base path
            basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalState"
            'Flight plans folders
            folderPathToCheck = basePath
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2020FlightPlanFilesFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2020FlightPlanFilesFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2020 (Microsoft Store) flight plans folder.{Environment.NewLine}"
            End If
            'Weather presets folders
            folderPathToCheck = $"{basePath}\Weather\Presets"
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2020WeatherPresetsFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2020WeatherPresetsFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2020 (Microsoft Store) weather profiles folder.{Environment.NewLine}"
            End If
        End If
        If opt2020Steam.Checked Then
            'Base path
            basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Microsoft Flight Simulator"
            'Flight plans folders
            folderPathToCheck = basePath
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2020FlightPlanFilesFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2020FlightPlanFilesFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2020 (Steam) flight plans folder.{Environment.NewLine}"
            End If
            'Weather presets folders
            folderPathToCheck = $"{basePath}\Weather\Presets"
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2020WeatherPresetsFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2020WeatherPresetsFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2020 (Steam) weather profiles folder.{Environment.NewLine}"
            End If
        End If

        If opt2024Microsoft.Checked Then
            'Base path
            basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\Microsoft.Limitless_8wekyb3d8bbwe\LocalState"
            'Flight plans folders
            folderPathToCheck = basePath
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2024FlightPlanFilesFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2024FlightPlanFilesFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2024 (Microsoft Store) flight plans folder.{Environment.NewLine}"
            End If
            'Weather presets folders
            folderPathToCheck = $"{basePath}\Weather\Presets"
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2024WeatherPresetsFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2024WeatherPresetsFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2024 (Microsoft Store) weather profiles folder.{Environment.NewLine}"
            End If
        End If
        If opt2024Steam.Checked Then
            'Base path
            basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Microsoft Flight Simulator 2024"
            'Flight plans folders
            folderPathToCheck = basePath
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2024FlightPlanFilesFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2024FlightPlanFilesFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2024 (Steam) flight plans folder.{Environment.NewLine}"
            End If
            'Weather presets folders
            folderPathToCheck = $"{basePath}\Weather\Presets"
            If Directory.Exists(folderPathToCheck) Then
                btnMSFS2024WeatherPresetsFolder.Text = folderPathToCheck
                ToolTip1.SetToolTip(btnMSFS2024WeatherPresetsFolder, folderPathToCheck)
            Else
                errorMessages = $"{errorMessages}Could not find MSFS 2024 (Steam) weather profiles folder.{Environment.NewLine}"
            End If
        End If

        If errorMessages.Length > 0 Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"The following error(s) occured:{Environment.NewLine}{Environment.NewLine}{errorMessages}", "Detecting MSFS folders", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    ''' <summary>
    ''' Toggles WSGListener.exe auto-start in HKCU\...\Run
    ''' </summary>
    ''' <param name="enable">True to add; False to remove.</param>
    Private Sub SetWSGListenerAutoStart(enable As Boolean)
        Const runKeyPath As String = "Software\Microsoft\Windows\CurrentVersion\Run"
        Dim exePath = Path.Combine(Application.StartupPath, "WSGListener.exe")
        Dim regValue = $"""{exePath}"""

        Using runKey = Registry.CurrentUser.OpenSubKey(runKeyPath, writable:=True)
            If runKey Is Nothing Then
                Throw New InvalidOperationException("Could not open registry key for auto-start.")
            End If

            If enable Then
                runKey.SetValue("WSGListener", regValue)
            Else
                runKey.DeleteValue("WSGListener", throwOnMissingValue:=False)
            End If
        End Using
    End Sub

End Class
