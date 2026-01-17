Imports System.IO
Imports Microsoft.Win32
Imports SIGLR.SoaringTools.CommonLibrary
Imports System.IO.Compression

Public Class WeatherPresetBrowser

    Shared _sscWeatherPresets As Dictionary(Of String, SSCWeatherPreset) = Nothing

    Private _parent As Main = Nothing
    Private _presetTypeBuffer As String = ""
    Private _presetTypeTimer As Timer

    Public SSCPresetName As String = String.Empty
    Public PrimaryWPRFilename As String = String.Empty
    Public SecondaryWPRFilename As String = String.Empty


    Public Function ShowForm(parent As Main, p_sscPresetName As String, p_primaryWPRFilename As String, p_secondaryWPRFilename As String) As DialogResult

        _parent = parent

        If _sscWeatherPresets Is Nothing Then
            _sscWeatherPresets = SSCWeatherPreset.LoadSSCWeatherPresets()
        End If

        SSCPresetName = p_sscPresetName
        PrimaryWPRFilename = p_primaryWPRFilename
        SecondaryWPRFilename = p_secondaryWPRFilename

        'Populate SSC Presets dropdown list
        cboSSCPresetList.Items.Clear()
        For Each preset As KeyValuePair(Of String, SSCWeatherPreset) In _sscWeatherPresets
            cboSSCPresetList.Items.Add(preset.Key)
        Next

        Select Case True
            Case SSCPresetName = String.Empty AndAlso PrimaryWPRFilename = String.Empty AndAlso SecondaryWPRFilename = String.Empty
                'No preset selected
                optSSCPreset.Checked = True
                grpSSCPresets.Enabled = True
                grpCustomPresets.Enabled = False
                cboSSCPresetList.SelectedIndex = -1
            Case SSCPresetName = String.Empty AndAlso (PrimaryWPRFilename <> String.Empty OrElse SecondaryWPRFilename <> String.Empty)
                'Custom preset selected
                optCustomPreset.Checked = True
                grpSSCPresets.Enabled = False
                grpCustomPresets.Enabled = True
                cboSSCPresetList.SelectedIndex = -1
                SetCustomPreset(True, PrimaryWPRFilename)
                SetCustomPreset(False, SecondaryWPRFilename)
            Case Else
                'SSC preset selected
                If Not _sscWeatherPresets.ContainsKey(SSCPresetName) Then
                    'The SSC preset no longer exists - revert to custom preset with old files - TODO: show message?
                    SSCPresetName = String.Empty
                    optCustomPreset.Checked = True
                    grpSSCPresets.Enabled = False
                    grpCustomPresets.Enabled = True
                    cboSSCPresetList.SelectedIndex = -1
                    SetCustomPreset(True, PrimaryWPRFilename)
                    SetCustomPreset(False, SecondaryWPRFilename)
                End If
                optSSCPreset.Checked = True
                grpSSCPresets.Enabled = True
                grpCustomPresets.Enabled = False
                cboSSCPresetList.Text = SSCPresetName
        End Select

        Me.ShowDialog(parent)

        Return Me.DialogResult

    End Function

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        'If an SSC preset was previously selected and Custom was then selected, we need to restore it and redownload the files
        If SSCPresetName <> String.Empty AndAlso optCustomPreset.Checked Then
            Try
                DownloadAndExtractZipPresets(SSCPresetName)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show($"An error occurent while re-downloading the presets: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
            End Try
        End If

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        ' If SSC preset changed, delete previous extracted local files based on the OLD preset filenames
        If SSCPresetName <> String.Empty AndAlso SSCPresetName <> cboSSCPresetList.Text Then

            Dim oldPreset As SSCWeatherPreset = _sscWeatherPresets(SSCPresetName)

            Dim oldLocalPrimary As String = LocalPresetPathFromServerPath(oldPreset.PresetPrimaryWPRFilename, _parent.CurrentSessionFile)
            Dim oldLocalSecondary As String = LocalPresetPathFromServerPath(oldPreset.PresetSecondaryWPRFilename, _parent.CurrentSessionFile)

            If File.Exists(oldLocalPrimary) Then File.Delete(oldLocalPrimary)
            If File.Exists(oldLocalSecondary) Then File.Delete(oldLocalSecondary)
        End If

        'If SSC preset selected, download and extract the files
        If optSSCPreset.Checked Then
            Try
                DownloadAndExtractZipPresets(cboSSCPresetList.Text)

            Catch ex As Exception
                Using New Centered_MessageBox(Me)
                    MessageBox.Show($"An error occurent while downloading the presets: {Environment.NewLine}{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End Try

            ' Save selections to return to parent form
            SSCPresetName = cboSSCPresetList.Text

            ' IMPORTANT: store the *local* filenames (or full local paths) derived from the NEW preset
            Dim newPreset As SSCWeatherPreset = _sscWeatherPresets(SSCPresetName)
            Dim newLocalPrimary As String = LocalPresetPathFromServerPath(newPreset.PresetPrimaryWPRFilename, _parent.CurrentSessionFile)
            Dim newLocalSecondary As String = LocalPresetPathFromServerPath(newPreset.PresetSecondaryWPRFilename, _parent.CurrentSessionFile)
            PrimaryWPRFilename = newLocalPrimary
            SecondaryWPRFilename = newLocalSecondary
        Else
            SSCPresetName = String.Empty
            PrimaryWPRFilename = lblWeatherPresetPrimaryFilename.Tag.ToString()
            SecondaryWPRFilename = lblWeatherPresetSecondaryFilename.Tag.ToString()
        End If


        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub DownloadAndExtractZipPresets(presetTitle As String)
        ' Download ZIP into the session folder
        Dim sessionFolder As String = Path.GetDirectoryName(_parent.CurrentSessionFile)
        Dim zipPath As String = SSCWeatherPreset.DownloadSSCWeatherPresetZip(presetTitle, sessionFolder)

        ' Extract ZIP to session folder (keep exact filenames from ZIP)
        Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
            For Each entry As ZipArchiveEntry In archive.Entries
                If String.IsNullOrWhiteSpace(entry.Name) Then Continue For
                If Not entry.Name.EndsWith(".WPR", StringComparison.OrdinalIgnoreCase) Then Continue For

                Dim outPath As String = Path.Combine(sessionFolder, entry.Name)
                entry.ExtractToFile(outPath, True)
            Next
        End Using

        ' Delete ZIP after successful extraction attempt
        Try : File.Delete(zipPath) : Catch : End Try

    End Sub

    Private Shared Function LocalPresetPathFromServerPath(serverRelativePath As String, sessionFilePath As String) As String
        Dim folder As String = Path.GetDirectoryName(sessionFilePath)
        Dim fileName As String = Path.GetFileName(serverRelativePath) 'works even if server path uses /
        Return Path.Combine(folder, fileName)
    End Function

    Private Sub WeatherPresetBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        _presetTypeTimer = New Timer() With {.Interval = 900} ' reset after ~0.9s idle
        AddHandler _presetTypeTimer.Tick,
        Sub()
            _presetTypeTimer.Stop()
            _presetTypeBuffer = ""
        End Sub

        AddHandler cboSSCPresetList.KeyPress, AddressOf cboSSCPresetList_KeyPress

    End Sub

    Private Sub optPreset_CheckedChanged(sender As Object, e As EventArgs) Handles optCustomPreset.CheckedChanged, optSSCPreset.CheckedChanged

        Select Case True
            Case optSSCPreset.Checked
                grpSSCPresets.Enabled = True
                grpCustomPresets.Enabled = False
            Case optCustomPreset.Checked
                grpSSCPresets.Enabled = False
                grpCustomPresets.Enabled = True
                cboSSCPresetList.SelectedIndex = -1
        End Select
    End Sub

    Private Sub cboSSCPresetList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSSCPresetList.SelectedIndexChanged

        If cboSSCPresetList.SelectedIndex = -1 Then
            If SSCPresetName <> String.Empty Then
                Dim oldPreset As SSCWeatherPreset = _sscWeatherPresets(SSCPresetName)

                Dim oldLocalPrimary As String = LocalPresetPathFromServerPath(oldPreset.PresetPrimaryWPRFilename, _parent.CurrentSessionFile)
                Dim oldLocalSecondary As String = LocalPresetPathFromServerPath(oldPreset.PresetSecondaryWPRFilename, _parent.CurrentSessionFile)

                If File.Exists(oldLocalPrimary) Then File.Delete(oldLocalPrimary)
                If File.Exists(oldLocalSecondary) Then File.Delete(oldLocalSecondary)
            End If
            ClearOutCustomPreset(True)
            ClearOutCustomPreset(False)
            Exit Sub
        End If

        'Set SSC Preset details and filename labels based on selected preset
        Dim sscPreset As SSCWeatherPreset = _sscWeatherPresets(cboSSCPresetList.Items(cboSSCPresetList.SelectedIndex))

        If sscPreset IsNot Nothing Then
            SetCustomPreset(True, sscPreset.PresetPrimaryWPRFilename, sscPreset.PresetMSFSTitlePrimary)
            SetCustomPreset(False, sscPreset.PresetSecondaryWPRFilename, sscPreset.PresetMSFSTitleSecondary)
        End If

    End Sub

    Private Sub btnPresetPrimaryBrowse_Click(sender As Object, e As EventArgs) Handles btnPresetPrimaryBrowse.Click

        'Browse for primary preset file
        If _parent.txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = Main.SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(_parent.txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select primary weather file"
        OpenFileDialog1.Filter = "Weather preset file|*.wpr"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim selectedWeatherFile As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileName)
            SetCustomPreset(True, selectedWeatherFile)
            If selectedWeatherFile = lblWeatherPresetSecondaryFilename.Tag Then
                ' Don't allow same file for both versions
                lblWeatherPresetSecondaryFilename.Text = String.Empty
                lblWeatherPresetSecondaryFilename.Tag = String.Empty
                lblWeatherPresetTitleSecondary.Text = String.Empty
            End If
        End If

    End Sub

    Private Sub btnPresetSecondaryBrowse_Click(sender As Object, e As EventArgs) Handles btnPresetSecondaryBrowse.Click

        'Browse for secondary preset file
        If _parent.txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = Main.SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(_parent.txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select secondary weather file"
        OpenFileDialog1.Filter = "Weather preset file|*.wpr"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim selectedWeatherFile As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileName)
            If selectedWeatherFile = lblWeatherPresetPrimaryFilename.Tag Then
                ' Don't allow same file for both versions
            Else
                SetCustomPreset(False, selectedWeatherFile)
            End If
        End If

    End Sub

    Private Sub SetCustomPreset(defaultPreset As Boolean, selectedWeatherFile As String, Optional presetTitle As String = "")

        Dim filenameOnly As String = Path.GetFileName(selectedWeatherFile)

        If defaultPreset Then
            lblWeatherPresetPrimaryFilename.Text = filenameOnly
            lblWeatherPresetPrimaryFilename.Tag = selectedWeatherFile
            ToolTip1.SetToolTip(lblWeatherPresetPrimaryFilename, selectedWeatherFile)
            Main.SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(selectedWeatherFile)
            If presetTitle = String.Empty Then
                presetTitle = SupportingFeatures.GetWeatherPresetTitleFromFile(selectedWeatherFile)
            End If
            lblWeatherPresetTitle2024.Text = presetTitle
        Else
            lblWeatherPresetSecondaryFilename.Text = filenameOnly
            lblWeatherPresetSecondaryFilename.Tag = selectedWeatherFile
            ToolTip1.SetToolTip(lblWeatherPresetSecondaryFilename, selectedWeatherFile)
            If presetTitle = String.Empty Then
                presetTitle = SupportingFeatures.GetWeatherPresetTitleFromFile(selectedWeatherFile)
            End If
            lblWeatherPresetTitleSecondary.Text = presetTitle
        End If

    End Sub

    Private Sub ClearOutCustomPreset(primary As Boolean)
        If primary Then
            lblWeatherPresetPrimaryFilename.Text = String.Empty
            lblWeatherPresetPrimaryFilename.Tag = String.Empty
            ToolTip1.SetToolTip(lblWeatherPresetPrimaryFilename, String.Empty)
            lblWeatherPresetTitle2024.Text = String.Empty
        Else
            lblWeatherPresetSecondaryFilename.Text = String.Empty
            lblWeatherPresetSecondaryFilename.Tag = String.Empty
            ToolTip1.SetToolTip(lblWeatherPresetSecondaryFilename, String.Empty)
            lblWeatherPresetTitleSecondary.Text = String.Empty
        End If
    End Sub
    Private Sub lblWeatherPresetFilename2024_TextChanged(sender As Object, e As EventArgs) Handles lblWeatherPresetPrimaryFilename.TextChanged

        If lblWeatherPresetPrimaryFilename.Text = String.Empty Then
            btnPresetSecondaryBrowse.Enabled = False
            lblWeatherPresetSecondaryFilename.Text = String.Empty
            lblWeatherPresetTitleSecondary.Text = String.Empty
            btnSave.Enabled = False
        Else
            btnPresetSecondaryBrowse.Enabled = True
            btnSave.Enabled = True
        End If
    End Sub

    Private Sub cboSSCPresetList_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Only allow digits + Backspace
        If Char.IsDigit(e.KeyChar) Then
            e.Handled = True

            _presetTypeTimer.Stop()
            _presetTypeTimer.Start()

            _presetTypeBuffer &= e.KeyChar

            ' Keep buffer to 3 digits since your presets are 001-206
            If _presetTypeBuffer.Length > 3 Then
                _presetTypeBuffer = _presetTypeBuffer.Substring(_presetTypeBuffer.Length - 3)
            End If

            JumpToPresetByPrefix(_presetTypeBuffer)
            Return
        End If

        If e.KeyChar = ChrW(Keys.Back) Then
            e.Handled = True
            If _presetTypeBuffer.Length > 0 Then
                _presetTypeBuffer = _presetTypeBuffer.Substring(0, _presetTypeBuffer.Length - 1)
                JumpToPresetByPrefix(_presetTypeBuffer)
            End If
            Return
        End If

        ' Block everything else
        e.Handled = True
    End Sub

    Private Sub JumpToPresetByPrefix(prefix As String)
        If String.IsNullOrWhiteSpace(prefix) Then Return

        Dim idx As Integer = -1

        ' Assumes items are strings starting with "001", "002", ...
        For i As Integer = 0 To cboSSCPresetList.Items.Count - 1
            Dim s As String = Convert.ToString(cboSSCPresetList.Items(i))
            If s IsNot Nothing AndAlso s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                idx = i
                Exit For
            End If
        Next

        If idx >= 0 Then
            cboSSCPresetList.SelectedIndex = idx

            ' Open dropdown so user sees where they landed
            If Not cboSSCPresetList.DroppedDown Then
                cboSSCPresetList.DroppedDown = True
            End If

        End If
    End Sub

End Class
