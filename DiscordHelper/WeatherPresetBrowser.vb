Imports System.IO
Imports System.Xml
Imports Microsoft.Win32
Imports SIGLR.SoaringTools.CommonLibrary
Imports System.IO.Compression

Public Class WeatherPresetBrowser

    Shared _sscWeatherPresets As Dictionary(Of String, SSCWeatherPreset) = Nothing

    Private _parent As Main = Nothing
    Private _presetTypeBuffer As String = ""
    Private _presetTypeTimer As Timer

    Public SSCPresetName As String = String.Empty
    Public Filename2024 As String = String.Empty
    Public Filename2020 As String = String.Empty


    Public Function ShowForm(parent As Main, p_sscPresetName As String, p_filename2024 As String, p_filename2020 As String) As DialogResult

        _parent = parent

        If _sscWeatherPresets Is Nothing Then
            _sscWeatherPresets = SSCWeatherPreset.LoadSSCWeatherPresets()
        End If

        SSCPresetName = p_sscPresetName
        Filename2024 = p_filename2024
        Filename2020 = p_filename2020

        'Populate SSC Presets dropdown list
        cboSSCPresetList.Items.Clear()
        For Each preset As KeyValuePair(Of String, SSCWeatherPreset) In _sscWeatherPresets
            cboSSCPresetList.Items.Add(preset.Key)
        Next

        Select Case True
            Case SSCPresetName = String.Empty AndAlso Filename2024 = String.Empty AndAlso Filename2020 = String.Empty
                'No preset selected
                optSSCPreset.Checked = True
                grpSSCPresets.Enabled = True
                grpCustomPresets.Enabled = False
                cboSSCPresetList.SelectedIndex = -1
            Case SSCPresetName = String.Empty AndAlso (Filename2024 <> String.Empty OrElse Filename2020 <> String.Empty)
                'Custom preset selected
                optCustomPreset.Checked = True
                grpSSCPresets.Enabled = False
                grpCustomPresets.Enabled = True
                cboSSCPresetList.SelectedIndex = -1
                SetCustomPreset(True, Filename2024)
                SetCustomPreset(False, Filename2020)
            Case Else
                'SSC preset selected
                If Not _sscWeatherPresets.ContainsKey(SSCPresetName) Then
                    'The SSC preset no longer exists - revert to custom preset with old files - TODO: show message?
                    SSCPresetName = String.Empty
                    optCustomPreset.Checked = True
                    grpSSCPresets.Enabled = False
                    grpCustomPresets.Enabled = True
                    cboSSCPresetList.SelectedIndex = -1
                    SetCustomPreset(True, Filename2024)
                    SetCustomPreset(False, Filename2020)
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

            Dim oldLocal2024 As String = LocalPresetPathFromServerPath(oldPreset.PresetFile2024, _parent.CurrentSessionFile)
            Dim oldLocal2020 As String = LocalPresetPathFromServerPath(oldPreset.PresetFile2020, _parent.CurrentSessionFile)

            If File.Exists(oldLocal2024) Then File.Delete(oldLocal2024)
            If File.Exists(oldLocal2020) Then File.Delete(oldLocal2020)
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
            Dim newLocal2024 As String = LocalPresetPathFromServerPath(newPreset.PresetFile2024, _parent.CurrentSessionFile)
            Dim newLocal2020 As String = LocalPresetPathFromServerPath(newPreset.PresetFile2020, _parent.CurrentSessionFile)
            Filename2024 = newLocal2024
            Filename2020 = newLocal2020
        Else
            SSCPresetName = String.Empty
            Filename2024 = lblWeatherPresetFilename2024.Tag.ToString()
            Filename2020 = lblWeatherPresetFilename2020.Tag.ToString()
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

                Dim oldLocal2024 As String = LocalPresetPathFromServerPath(oldPreset.PresetFile2024, _parent.CurrentSessionFile)
                Dim oldLocal2020 As String = LocalPresetPathFromServerPath(oldPreset.PresetFile2020, _parent.CurrentSessionFile)

                If File.Exists(oldLocal2024) Then File.Delete(oldLocal2024)
                If File.Exists(oldLocal2020) Then File.Delete(oldLocal2020)
            End If
            ClearOutCustomPreset(True)
            ClearOutCustomPreset(False)
            Exit Sub
        End If

        'Set SSC Preset details and filename labels based on selected preset
        Dim sscPreset As SSCWeatherPreset = _sscWeatherPresets(cboSSCPresetList.Items(cboSSCPresetList.SelectedIndex))

        If sscPreset IsNot Nothing Then
            SetCustomPreset(True, sscPreset.PresetFile2024, sscPreset.PresetMSFSTitle2024)
            SetCustomPreset(False, sscPreset.PresetFile2020, sscPreset.PresetMSFSTitle2020)
        End If

    End Sub

    Private Sub btnPreset2024Browse_Click(sender As Object, e As EventArgs) Handles btnPreset2024Browse.Click

        'Browse for 2024 preset file
        If _parent.txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = Main.SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(_parent.txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select weather file for MSFS 2024"
        OpenFileDialog1.Filter = "Weather preset file|*.wpr"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim selectedWeatherFile As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileName)
            SetCustomPreset(True, selectedWeatherFile)
            If selectedWeatherFile = lblWeatherPresetFilename2020.Tag Then
                ' Don't allow same file for both versions
                lblWeatherPresetFilename2020.Text = String.Empty
                lblWeatherPresetFilename2020.Tag = String.Empty
                lblWeatherPresetTitle2020.Text = String.Empty
            End If
        End If

    End Sub

    Private Sub btnPreset2020Browse_Click(sender As Object, e As EventArgs) Handles btnPreset2020Browse.Click

        'Browse for 2020 preset file
        If _parent.txtFlightPlanFile.Text = String.Empty Then
            OpenFileDialog1.InitialDirectory = Main.SessionSettings.LastUsedFileLocation
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(_parent.txtFlightPlanFile.Text)
        End If
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Title = "Select weather file for MSFS 2024"
        OpenFileDialog1.Filter = "Weather preset file|*.wpr"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            Dim selectedWeatherFile As String = SupportingFeatures.SanitizeFilePath(OpenFileDialog1.FileName)
            If selectedWeatherFile = lblWeatherPresetFilename2024.Tag Then
                ' Don't allow same file for both versions
            Else
                SetCustomPreset(False, selectedWeatherFile)
            End If
        End If

    End Sub

    Private Sub SetCustomPreset(defaultPreset As Boolean, selectedWeatherFile As String, Optional presetTitle As String = "")

        Dim filenameOnly As String = Path.GetFileName(selectedWeatherFile)

        If defaultPreset Then
            lblWeatherPresetFilename2024.Text = filenameOnly
            lblWeatherPresetFilename2024.Tag = selectedWeatherFile
            ToolTip1.SetToolTip(lblWeatherPresetFilename2024, selectedWeatherFile)
            Main.SessionSettings.LastUsedFileLocation = Path.GetDirectoryName(selectedWeatherFile)
            If presetTitle = String.Empty Then
                presetTitle = GetWeatherPresetTitleFromFile(selectedWeatherFile)
            End If
            lblWeatherPresetTitle2024.Text = presetTitle
        Else
            lblWeatherPresetFilename2020.Text = filenameOnly
            lblWeatherPresetFilename2020.Tag = selectedWeatherFile
            ToolTip1.SetToolTip(lblWeatherPresetFilename2020, selectedWeatherFile)
            If presetTitle = String.Empty Then
                presetTitle = GetWeatherPresetTitleFromFile(selectedWeatherFile)
            End If
            lblWeatherPresetTitle2020.Text = presetTitle
        End If

    End Sub

    Private Function GetWeatherPresetTitleFromFile(weatherFilePath As String) As String

        If weatherFilePath = String.Empty Then
            Return String.Empty
        End If

        Dim presetTitle As String = String.Empty
        Dim xmlDocWeatherPreset As New XmlDocument()
        xmlDocWeatherPreset.Load(weatherFilePath)
        Dim weatherDetails As WeatherDetails = New WeatherDetails(xmlDocWeatherPreset)
        presetTitle = weatherDetails.PresetName

        weatherDetails = Nothing
        xmlDocWeatherPreset = Nothing

        Return presetTitle

    End Function

    Private Sub ClearOutCustomPreset(primary As Boolean)
        If primary Then
            lblWeatherPresetFilename2024.Text = String.Empty
            lblWeatherPresetFilename2024.Tag = String.Empty
            ToolTip1.SetToolTip(lblWeatherPresetFilename2024, String.Empty)
            lblWeatherPresetTitle2024.Text = String.Empty
        Else
            lblWeatherPresetFilename2020.Text = String.Empty
            lblWeatherPresetFilename2020.Tag = String.Empty
            ToolTip1.SetToolTip(lblWeatherPresetFilename2020, String.Empty)
            lblWeatherPresetTitle2020.Text = String.Empty
        End If
    End Sub
    Private Sub lblWeatherPresetFilename2024_TextChanged(sender As Object, e As EventArgs) Handles lblWeatherPresetFilename2024.TextChanged

        If lblWeatherPresetFilename2024.Text = String.Empty Then
            btnPreset2020Browse.Enabled = False
            lblWeatherPresetFilename2020.Text = String.Empty
            lblWeatherPresetTitle2020.Text = String.Empty
            btnSave.Enabled = False
        Else
            btnPreset2020Browse.Enabled = True
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