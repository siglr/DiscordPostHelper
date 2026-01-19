Imports System
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Net
Imports System.Windows.Forms
Imports System.Xml.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Partial Public Class ManualFallbackMode
    Inherits ZoomForm

    Private _selectedPln As ManualFileSelection
    Private _selectedPrimaryWpr As ManualFileSelection
    Private _selectedSecondaryWpr As ManualFileSelection
    Private _plnGroupDefaultColor As Color
    Private _primaryGroupDefaultColor As Color
    Private _secondaryGroupDefaultColor As Color
    Private ReadOnly _dragHighlightColor As Color = ControlPaint.LightLight(SystemColors.Control)
    Private _selectionResult As ManualSelectionResult
    Private _sscWeatherPresets As Dictionary(Of String, SSCWeatherPreset)
    Private _msfs2020Enabled As Boolean
    Private _msfs2024Enabled As Boolean
    Private _layoutInitialized As Boolean
    Private _baseFormHeight As Integer
    Private _baseCustomGroupHeight As Integer
    Private _baseWeatherGroupHeight As Integer
    Private _baseTrackerTop As Integer
    Private _baseConfirmTop As Integer
    Private _baseCancelTop As Integer
    Private _customGroupSpacing As Integer
    Private _basePrimaryTop As Integer

    Public Property InitialPlnPath As String
    Public Property InitialPrimaryWprPath As String
    Public Property InitialSecondaryWprPath As String
    Public Property InitialSSCPresetName As String
    Public Property InitialTrackerGroup As String

    Public ReadOnly Property SelectionResult As ManualSelectionResult
        Get
            Return _selectionResult
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Rescale()
        _plnGroupDefaultColor = grpPLN.BackColor
        _primaryGroupDefaultColor = grpPrimaryWeather.BackColor
        _secondaryGroupDefaultColor = grpSecondaryWeather.BackColor
        InitializeLayoutSizing()
    End Sub

    Private Sub ManualFallbackMode_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _msfs2020Enabled = Settings.SessionSettings.Is2020Installed
        _msfs2024Enabled = Settings.SessionSettings.Is2024Installed

        LoadSscPresets()
        Reset()
        ApplyInitialSelection()
        UpdateWeatherLayout()
        UpdateWeatherModeControls()

        Me.AcceptButton = btnCopyGoFly
        Me.CancelButton = btnClearFiles
    End Sub

    Private Sub Reset()
        cboSSCPresetList.SelectedIndex = -1
        lblPLNFile.Text = String.Empty
        lblPLNTitle.Text = String.Empty
        lblPrimaryFile.Text = String.Empty
        lblPrimaryName.Text = String.Empty
        lblSecondaryFile.Text = String.Empty
        lblSecondaryName.Text = String.Empty
        ToolTip1.SetToolTip(lblPrimaryFile, String.Empty)
        ToolTip1.SetToolTip(lblSecondaryFile, String.Empty)
        _selectedPln = Nothing
        _selectedPrimaryWpr = Nothing
        _selectedSecondaryWpr = Nothing
        _selectionResult = Nothing

        If optSSCPreset.Enabled Then
            optSSCPreset.Checked = True
        Else
            optCustomPreset.Checked = True
        End If
    End Sub

    Private Sub ApplyInitialSelection()
        If Not String.IsNullOrWhiteSpace(InitialPlnPath) AndAlso File.Exists(InitialPlnPath) Then
            Dim selection = LoadPlnSelection(InitialPlnPath)
            If selection IsNot Nothing Then
                _selectedPln = selection
                lblPLNFile.Text = selection.FileName
                lblPLNTitle.Text = selection.DisplayName
            End If
        End If

        If Not String.IsNullOrWhiteSpace(InitialPrimaryWprPath) AndAlso File.Exists(InitialPrimaryWprPath) Then
            Dim selection = LoadWprSelection(InitialPrimaryWprPath, False)
            If selection IsNot Nothing Then
                _selectedPrimaryWpr = selection
            End If
        End If

        If Not String.IsNullOrWhiteSpace(InitialSecondaryWprPath) AndAlso File.Exists(InitialSecondaryWprPath) Then
            Dim selection = LoadWprSelection(InitialSecondaryWprPath, False)
            If selection IsNot Nothing Then
                _selectedSecondaryWpr = selection
            End If
        End If

        Dim hasCustomSelection = _selectedPrimaryWpr IsNot Nothing OrElse _selectedSecondaryWpr IsNot Nothing
        If Not String.IsNullOrWhiteSpace(InitialSSCPresetName) AndAlso _sscWeatherPresets IsNot Nothing AndAlso _sscWeatherPresets.ContainsKey(InitialSSCPresetName) Then
            optSSCPreset.Checked = True
            cboSSCPresetList.Text = InitialSSCPresetName
            DisplaySscPresetDetails(_sscWeatherPresets(InitialSSCPresetName))
        ElseIf hasCustomSelection Then
            optCustomPreset.Checked = True
            SyncCustomSelectionToLabels()
        End If

        If Not String.IsNullOrWhiteSpace(InitialTrackerGroup) Then
            txtTrackerGroupName.Text = InitialTrackerGroup
        End If
    End Sub

    Private Sub LoadSscPresets()
        cboSSCPresetList.Items.Clear()
        _sscWeatherPresets = Nothing

        Try
            _sscWeatherPresets = SSCWeatherPreset.LoadSSCWeatherPresets()

            If _sscWeatherPresets IsNot Nothing Then
                For Each preset As KeyValuePair(Of String, SSCWeatherPreset) In _sscWeatherPresets
                    cboSSCPresetList.Items.Add(preset.Key)
                Next
            End If
        Catch ex As Exception
            ShowCenteredMessage($"Unable to load SSC presets: {ex.Message}", "SSC presets")
        End Try

        Dim hasPresets As Boolean = _sscWeatherPresets IsNot Nothing AndAlso _sscWeatherPresets.Count > 0
        optSSCPreset.Enabled = hasPresets
        cboSSCPresetList.Enabled = hasPresets
        If Not hasPresets Then
            optCustomPreset.Checked = True
        End If
    End Sub

    Private Sub UpdateWeatherLayout()
        Dim has2020 = _msfs2020Enabled
        Dim has2024 = _msfs2024Enabled

        If has2020 AndAlso has2024 Then
            grpPrimaryWeather.Visible = True
            grpSecondaryWeather.Visible = True
            grpPrimaryWeather.Text = "Primary (MSFS 2024) - Required"
            grpSecondaryWeather.Text = If(optSSCPreset.Checked, "Secondary (MSFS 2020) - Required", "Secondary (MSFS 2020) - Optional")
        ElseIf has2024 Then
            grpPrimaryWeather.Visible = True
            grpSecondaryWeather.Visible = False
            grpPrimaryWeather.Text = "Primary (MSFS 2024) - Required"
        ElseIf has2020 Then
            grpPrimaryWeather.Visible = False
            grpSecondaryWeather.Visible = True
            grpSecondaryWeather.Text = "Weather Preset (MSFS 2020) - Required"
        Else
            grpPrimaryWeather.Visible = True
            grpSecondaryWeather.Visible = True
            grpPrimaryWeather.Text = "Primary (MSFS 2024)"
            grpSecondaryWeather.Text = "Secondary (MSFS 2020)"
        End If

        AdjustLayoutForSimConfiguration()
    End Sub

    Private Sub InitializeLayoutSizing()
        If _layoutInitialized Then
            Return
        End If

        _baseFormHeight = 650
        _baseCustomGroupHeight = grpCustomPresets.Height
        _baseWeatherGroupHeight = grpWeather.Height
        _baseTrackerTop = grpTracker.Top
        _baseConfirmTop = btnCopyGoFly.Top
        _baseCancelTop = btnClearFiles.Top
        _basePrimaryTop = grpPrimaryWeather.Top

        Dim primaryBottom = grpPrimaryWeather.Top + grpPrimaryWeather.Height
        _customGroupSpacing = Math.Max(0, grpSecondaryWeather.Top - primaryBottom)

        _layoutInitialized = True
    End Sub

    Private Sub AdjustLayoutForSimConfiguration()
        If Not _layoutInitialized Then
            InitializeLayoutSizing()
        End If

        Dim showPrimary = grpPrimaryWeather.Visible
        Dim showSecondary = grpSecondaryWeather.Visible
        Dim sectionsVisible = 0
        If showPrimary Then sectionsVisible += 1
        If showSecondary Then sectionsVisible += 1

        Dim shrinkBy As Integer = 0
        If sectionsVisible <= 1 Then
            shrinkBy = grpSecondaryWeather.Height + _customGroupSpacing
        End If

        If showSecondary AndAlso Not showPrimary Then
            grpSecondaryWeather.Top = _basePrimaryTop
        Else
            grpSecondaryWeather.Top = _basePrimaryTop + grpPrimaryWeather.Height + _customGroupSpacing
        End If

        grpCustomPresets.Height = _baseCustomGroupHeight - shrinkBy
        grpWeather.Height = _baseWeatherGroupHeight - shrinkBy

        grpTracker.Top = _baseTrackerTop - shrinkBy
        btnCopyGoFly.Top = _baseConfirmTop - shrinkBy
        btnClearFiles.Top = _baseCancelTop - shrinkBy

        Dim targetHeight = _baseFormHeight - shrinkBy
        Dim newWidth = Me.Width
        Dim minWidth = Me.MinimumSize.Width
        Dim maxWidth = Me.MaximumSize.Width

        Me.MinimumSize = New Size(minWidth, targetHeight)
        Me.MaximumSize = New Size(maxWidth, targetHeight)
        Me.Size = New Size(newWidth, targetHeight)
    End Sub

    Private Sub UpdateWeatherModeControls()
        grpCustomPresets.Enabled = optCustomPreset.Checked
        cboSSCPresetList.Enabled = optSSCPreset.Checked AndAlso optSSCPreset.Enabled
        UpdateWeatherLayout()

        If optSSCPreset.Checked Then
            Dim selectedPreset = GetSelectedSscPreset()
            If selectedPreset IsNot Nothing Then
                DisplaySscPresetDetails(selectedPreset)
            End If
        Else
            SyncCustomSelectionToLabels()
        End If
    End Sub

    Private Function GetSelectedSscPreset() As SSCWeatherPreset
        If _sscWeatherPresets Is Nothing Then
            Return Nothing
        End If

        Dim selectedName = cboSSCPresetList.Text
        If String.IsNullOrWhiteSpace(selectedName) Then
            Return Nothing
        End If

        If _sscWeatherPresets.ContainsKey(selectedName) Then
            Return _sscWeatherPresets(selectedName)
        End If

        Return Nothing
    End Function

    Private Sub optPreset_CheckedChanged(sender As Object, e As EventArgs) Handles optCustomPreset.CheckedChanged, optSSCPreset.CheckedChanged
        UpdateWeatherModeControls()
    End Sub

    Private Sub cboSSCPresetList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSSCPresetList.SelectedIndexChanged
        If cboSSCPresetList.SelectedIndex = -1 Then
            Return
        End If

        Dim selectedPreset = GetSelectedSscPreset()
        If selectedPreset IsNot Nothing Then
            DisplaySscPresetDetails(selectedPreset)
        End If
    End Sub

    Private Sub DisplaySscPresetDetails(preset As SSCWeatherPreset)
        If preset Is Nothing Then
            Return
        End If

        lblPrimaryFile.Text = Path.GetFileName(preset.PresetPrimaryWPRFilename)
        lblPrimaryName.Text = preset.PresetMSFSTitlePrimary
        lblSecondaryFile.Text = Path.GetFileName(preset.PresetSecondaryWPRFilename)
        lblSecondaryName.Text = preset.PresetMSFSTitleSecondary
        ToolTip1.SetToolTip(lblPrimaryFile, preset.PresetPrimaryWPRFilename)
        ToolTip1.SetToolTip(lblSecondaryFile, preset.PresetSecondaryWPRFilename)
    End Sub

    Private Sub SyncCustomSelectionToLabels()
        UpdateCustomPresetLabels(True, _selectedPrimaryWpr)
        UpdateCustomPresetLabels(False, _selectedSecondaryWpr)
    End Sub

    Private Sub UpdateCustomPresetLabels(isPrimary As Boolean, selection As ManualFileSelection)
        If isPrimary Then
            If selection Is Nothing Then
                lblPrimaryFile.Text = String.Empty
                lblPrimaryName.Text = String.Empty
                lblPrimaryFile.Tag = String.Empty
                ToolTip1.SetToolTip(lblPrimaryFile, String.Empty)
            Else
                lblPrimaryFile.Text = selection.FileName
                lblPrimaryName.Text = selection.DisplayName
                lblPrimaryFile.Tag = selection.FullPath
                ToolTip1.SetToolTip(lblPrimaryFile, selection.FullPath)
            End If
        Else
            If selection Is Nothing Then
                lblSecondaryFile.Text = String.Empty
                lblSecondaryName.Text = String.Empty
                lblSecondaryFile.Tag = String.Empty
                ToolTip1.SetToolTip(lblSecondaryFile, String.Empty)
            Else
                lblSecondaryFile.Text = selection.FileName
                lblSecondaryName.Text = selection.DisplayName
                lblSecondaryFile.Tag = selection.FullPath
                ToolTip1.SetToolTip(lblSecondaryFile, selection.FullPath)
            End If
        End If
    End Sub

    Private Sub btnCopyGoFly_Click(sender As Object, e As EventArgs) Handles btnCopyGoFly.Click
        If _selectedPln Is Nothing Then
            ShowCenteredMessage("Please select at least a flight plan before continuing.", "No files selected")
            Return
        End If

        Dim primaryPath As String = String.Empty
        Dim secondaryPath As String = String.Empty
        Dim primaryName As String = String.Empty
        Dim secondaryName As String = String.Empty
        Dim presetName As String = String.Empty
        Dim mode As ManualWeatherSelectionMode

        If optSSCPreset.Checked Then
            Dim selectedPreset = GetSelectedSscPreset()
            If selectedPreset Is Nothing Then
                ShowCenteredMessage("Please select an SSC standard preset before continuing.", "SSC preset missing")
                Return
            End If

            Try
                Dim downloadResult = DownloadAndExtractSscPresets(selectedPreset)
                primaryPath = downloadResult.Item1
                secondaryPath = downloadResult.Item2
                primaryName = selectedPreset.PresetMSFSTitlePrimary
                secondaryName = selectedPreset.PresetMSFSTitleSecondary
                presetName = selectedPreset.PresetDescriptiveName
                mode = ManualWeatherSelectionMode.SSC_STANDARD
            Catch ex As Exception
                ShowCenteredMessage($"An error occurred while downloading the SSC preset: {ex.Message}", "SSC preset")
                Return
            End Try
        Else
            mode = ManualWeatherSelectionMode.CUSTOM
            If _selectedPrimaryWpr IsNot Nothing Then
                primaryPath = _selectedPrimaryWpr.FullPath
                primaryName = _selectedPrimaryWpr.DisplayName
            End If

            If _selectedSecondaryWpr IsNot Nothing Then
                secondaryPath = _selectedSecondaryWpr.FullPath
                secondaryName = _selectedSecondaryWpr.DisplayName
            End If
        End If

        If Not ValidateWeatherSelection(mode, primaryPath, secondaryPath) Then
            Return
        End If

        _selectionResult = New ManualSelectionResult With {
            .FlightPlanPath = _selectedPln.FullPath,
            .PrimaryWeatherLocalPath = primaryPath,
            .SecondaryWeatherLocalPath = secondaryPath,
            .DisplayNamePrimary = primaryName,
            .DisplayNameSecondary = secondaryName,
            .SSCPresetName = presetName,
            .WeatherMode = mode,
            .TrackerGroupName = txtTrackerGroupName.Text.Trim()
        }

        Dim unpackForm = GetUnpackAndLoadForm()
        If unpackForm IsNot Nothing Then
            unpackForm.UpdateManualFallbackPaths(_selectedPln.FullPath, primaryPath, secondaryPath, _selectionResult.TrackerGroupName, presetName)
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Function ValidateWeatherSelection(mode As ManualWeatherSelectionMode, primaryPath As String, secondaryPath As String) As Boolean
        If mode = ManualWeatherSelectionMode.SSC_STANDARD Then
            If _msfs2024Enabled AndAlso (String.IsNullOrWhiteSpace(primaryPath) OrElse Not File.Exists(primaryPath)) Then
                ShowCenteredMessage("A primary (MSFS 2024) weather preset is required.", "Weather preset missing")
                Return False
            End If
            If _msfs2020Enabled AndAlso (String.IsNullOrWhiteSpace(secondaryPath) OrElse Not File.Exists(secondaryPath)) Then
                ShowCenteredMessage("A secondary (MSFS 2020) weather preset is required.", "Weather preset missing")
                Return False
            End If
            Return True
        End If

        If _msfs2024Enabled AndAlso _msfs2020Enabled Then
            If String.IsNullOrWhiteSpace(primaryPath) Then
                ShowCenteredMessage("A primary weather preset is required for MSFS 2024.", "Weather preset missing")
                Return False
            End If
        ElseIf _msfs2024Enabled Then
            If String.IsNullOrWhiteSpace(primaryPath) Then
                ShowCenteredMessage("A primary weather preset is required for MSFS 2024.", "Weather preset missing")
                Return False
            End If
        ElseIf _msfs2020Enabled Then
            If String.IsNullOrWhiteSpace(secondaryPath) Then
                ShowCenteredMessage("A weather preset is required for MSFS 2020.", "Weather preset missing")
                Return False
            End If
        Else
            If String.IsNullOrWhiteSpace(primaryPath) AndAlso String.IsNullOrWhiteSpace(secondaryPath) Then
                ShowCenteredMessage("A weather preset is required before continuing.", "Weather preset missing")
                Return False
            End If
        End If

        If Not String.IsNullOrWhiteSpace(primaryPath) AndAlso Not File.Exists(primaryPath) Then
            ShowCenteredMessage("The selected primary weather preset could not be found.", "Weather preset missing")
            Return False
        End If

        If Not String.IsNullOrWhiteSpace(secondaryPath) AndAlso Not File.Exists(secondaryPath) Then
            ShowCenteredMessage("The selected secondary weather preset could not be found.", "Weather preset missing")
            Return False
        End If

        Return True
    End Function

    Private Function DownloadAndExtractSscPresets(preset As SSCWeatherPreset) As Tuple(Of String, String)
        Dim targetFolder = GetPresetDownloadFolder()
        Dim zipPath = SSCWeatherPreset.DownloadSSCWeatherPresetZipByID(preset.PresetID, targetFolder)

        Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
            For Each entry As ZipArchiveEntry In archive.Entries
                If String.IsNullOrWhiteSpace(entry.Name) Then
                    Continue For
                End If
                If Not entry.Name.EndsWith(".WPR", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Dim outPath As String = Path.Combine(targetFolder, entry.Name)
                entry.ExtractToFile(outPath, True)
            Next
        End Using

        Try
            File.Delete(zipPath)
        Catch
        End Try

        Dim localPrimary As String = LocalPresetPathFromServerPath(preset.PresetPrimaryWPRFilename, targetFolder)
        Dim localSecondary As String = LocalPresetPathFromServerPath(preset.PresetSecondaryWPRFilename, targetFolder)

        Return Tuple.Create(localPrimary, localSecondary)
    End Function

    Private Function GetPresetDownloadFolder() As String
        Dim baseFolder As String = Settings.SessionSettings.UnpackingFolder
        If String.IsNullOrWhiteSpace(baseFolder) OrElse Not Directory.Exists(baseFolder) Then
            baseFolder = Path.GetTempPath()
        End If

        Dim folder As String = Path.Combine(baseFolder, "ManualFallbackPresets")
        If Not Directory.Exists(folder) Then
            Directory.CreateDirectory(folder)
        End If

        Return folder
    End Function

    Private Shared Function LocalPresetPathFromServerPath(serverRelativePath As String, targetFolder As String) As String
        Dim fileName As String = Path.GetFileName(serverRelativePath)
        Return Path.Combine(targetFolder, fileName)
    End Function

    Private Sub btnClearFiles_Click(sender As Object, e As EventArgs) Handles btnClearFiles.Click
        _selectionResult = Nothing
        Me.DialogResult = DialogResult.Cancel
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

    Private Sub btnPrimaryBrowse_Click(sender As Object, e As EventArgs) Handles btnPrimaryBrowse.Click
        Dim selection = BrowseForWeatherPreset("Select primary weather file")
        If selection IsNot Nothing Then
            _selectedPrimaryWpr = selection
            SyncCustomSelectionToLabels()
            optCustomPreset.Checked = True
        End If
    End Sub

    Private Sub btnSecondaryBrowse_Click(sender As Object, e As EventArgs) Handles btnSecondaryBrowse.Click
        Dim selection = BrowseForWeatherPreset("Select secondary weather file")
        If selection IsNot Nothing Then
            If _selectedPrimaryWpr IsNot Nothing AndAlso String.Equals(selection.FullPath, _selectedPrimaryWpr.FullPath, StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            _selectedSecondaryWpr = selection
            SyncCustomSelectionToLabels()
            optCustomPreset.Checked = True
        End If
    End Sub

    Private Function BrowseForWeatherPreset(title As String) As ManualFileSelection
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Weather preset (*.wpr)|*.wpr"
            ofd.Title = title
            ofd.Multiselect = False

            If ofd.ShowDialog(Me) = DialogResult.OK Then
                Return LoadWprSelection(ofd.FileName, True)
            End If
        End Using

        Return Nothing
    End Function

    Private Sub grpPLN_DragEnter(sender As Object, e As DragEventArgs) Handles grpPLN.DragEnter
        HandleGroupDragEnter(e, grpPLN, True, False)
    End Sub

    Private Sub grpPLN_DragOver(sender As Object, e As DragEventArgs) Handles grpPLN.DragOver
        HandleGroupDragEnter(e, grpPLN, True, False)
    End Sub

    Private Sub grpPLN_DragLeave(sender As Object, e As EventArgs) Handles grpPLN.DragLeave
        ClearAllGroupHighlights()
    End Sub

    Private Sub grpPLN_DragDrop(sender As Object, e As DragEventArgs) Handles grpPLN.DragDrop
        HandleGroupDragDrop(e, grpPLN, True, False)
    End Sub

    Private Sub grpPrimaryWeather_DragEnter(sender As Object, e As DragEventArgs) Handles grpPrimaryWeather.DragEnter
        HandleGroupDragEnter(e, grpPrimaryWeather, False, True)
    End Sub

    Private Sub grpPrimaryWeather_DragOver(sender As Object, e As DragEventArgs) Handles grpPrimaryWeather.DragOver
        HandleGroupDragEnter(e, grpPrimaryWeather, False, True)
    End Sub

    Private Sub grpPrimaryWeather_DragLeave(sender As Object, e As EventArgs) Handles grpPrimaryWeather.DragLeave
        ClearAllGroupHighlights()
    End Sub

    Private Sub grpPrimaryWeather_DragDrop(sender As Object, e As DragEventArgs) Handles grpPrimaryWeather.DragDrop
        HandleGroupDragDrop(e, grpPrimaryWeather, False, True)
    End Sub

    Private Sub grpSecondaryWeather_DragEnter(sender As Object, e As DragEventArgs) Handles grpSecondaryWeather.DragEnter
        HandleGroupDragEnter(e, grpSecondaryWeather, False, True)
    End Sub

    Private Sub grpSecondaryWeather_DragOver(sender As Object, e As DragEventArgs) Handles grpSecondaryWeather.DragOver
        HandleGroupDragEnter(e, grpSecondaryWeather, False, True)
    End Sub

    Private Sub grpSecondaryWeather_DragLeave(sender As Object, e As EventArgs) Handles grpSecondaryWeather.DragLeave
        ClearAllGroupHighlights()
    End Sub

    Private Sub grpSecondaryWeather_DragDrop(sender As Object, e As DragEventArgs) Handles grpSecondaryWeather.DragDrop
        HandleGroupDragDrop(e, grpSecondaryWeather, False, True)
    End Sub

    Private Sub HandleGroupDragEnter(e As DragEventArgs, targetGroup As GroupBox, acceptPln As Boolean, acceptWpr As Boolean)
        Dim entries = ExtractDragEntries(e.Data)
        If ContainsWeSimGlideLink(entries) Then
            ClearAllGroupHighlights()
            e.Effect = DragDropEffects.Copy
            Return
        End If

        Dim draggedFiles = GetDraggedFilesInfo(entries, False)
        ClearAllGroupHighlights()

        Dim canAccept = (acceptPln AndAlso draggedFiles.HasPln) OrElse (acceptWpr AndAlso draggedFiles.HasWpr)
        If canAccept Then
            SetGroupHighlight(targetGroup, True)
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub HandleGroupDragDrop(e As DragEventArgs, targetGroup As GroupBox, acceptPln As Boolean, acceptWpr As Boolean)
        Dim entries = ExtractDragEntries(e.Data)

        If ContainsWeSimGlideLink(entries) Then
            ClearAllGroupHighlights()
            ShowCenteredMessage("When using the DPHX Unpack & Load, simply click on the DPHX link instead of trying to drag and drop links to WeSimGlide.org", "Drag and drop")
            Return
        End If

        Dim draggedFiles = GetDraggedFilesInfo(entries, True)
        ClearAllGroupHighlights()

        If acceptPln AndAlso draggedFiles.HasPln Then
            Dim selection = LoadPlnSelection(draggedFiles.PlnPath)
            If selection IsNot Nothing Then
                _selectedPln = selection
                lblPLNFile.Text = selection.FileName
                lblPLNTitle.Text = selection.DisplayName
            End If
        End If

        If acceptWpr AndAlso draggedFiles.HasWpr Then
            Dim selection = LoadWprSelection(draggedFiles.WprPath, True)
            If selection IsNot Nothing Then
                optCustomPreset.Checked = True
                AssignDroppedWeather(selection, targetGroup)
                SyncCustomSelectionToLabels()
            End If
        End If
    End Sub

    Private Sub AssignDroppedWeather(selection As ManualFileSelection, targetGroup As GroupBox)
        If selection Is Nothing Then
            Return
        End If

        Dim showPrimary = grpPrimaryWeather.Visible
        Dim showSecondary = grpSecondaryWeather.Visible

        If optSSCPreset.Checked Then
            If showPrimary AndAlso _selectedPrimaryWpr Is Nothing Then
                _selectedPrimaryWpr = selection
                Return
            End If

            If showSecondary AndAlso _selectedSecondaryWpr Is Nothing Then
                _selectedSecondaryWpr = selection
                Return
            End If

            If showPrimary Then
                _selectedPrimaryWpr = selection
            ElseIf showSecondary Then
                _selectedSecondaryWpr = selection
            End If
            Return
        End If

        If targetGroup Is grpPrimaryWeather Then
            _selectedPrimaryWpr = selection
        ElseIf targetGroup Is grpSecondaryWeather Then
            _selectedSecondaryWpr = selection
        ElseIf showPrimary Then
            _selectedPrimaryWpr = selection
        ElseIf showSecondary Then
            _selectedSecondaryWpr = selection
        End If
    End Sub

    Private Function GetDraggedFilesInfo(entries As List(Of String), resolveDownloads As Boolean) As DraggedFilesInfo
        Dim info As New DraggedFilesInfo()

        If entries Is Nothing OrElse entries.Count = 0 Then
            Return info
        End If

        For Each entry In entries
            Dim extension = GetEntryExtension(entry)
            If String.IsNullOrEmpty(extension) Then
                Continue For
            End If

            If extension.Equals(".pln", StringComparison.OrdinalIgnoreCase) AndAlso String.IsNullOrEmpty(info.PlnPath) Then
                info.PlnPath = If(resolveDownloads, ResolveDroppedEntry(entry, ".pln"), entry)
            ElseIf extension.Equals(".wpr", StringComparison.OrdinalIgnoreCase) AndAlso String.IsNullOrEmpty(info.WprPath) Then
                info.WprPath = If(resolveDownloads, ResolveDroppedEntry(entry, ".wpr"), entry)
            End If

            If info.HasPln AndAlso info.HasWpr Then
                Exit For
            End If
        Next

        Return info
    End Function

    Private Function ExtractDragEntries(data As IDataObject) As List(Of String)
        Dim entries As New List(Of String)()

        If data Is Nothing Then
            Return entries
        End If

        If data.GetDataPresent(DataFormats.FileDrop) Then
            Dim files = TryCast(data.GetData(DataFormats.FileDrop), String())
            If files IsNot Nothing Then
                entries.AddRange(files.Where(Function(f) Not String.IsNullOrWhiteSpace(f)))
            End If
        ElseIf data.GetDataPresent(DataFormats.UnicodeText) OrElse data.GetDataPresent(DataFormats.Text) Then
            Dim textValue As String = Nothing
            If data.GetDataPresent(DataFormats.UnicodeText) Then
                textValue = TryCast(data.GetData(DataFormats.UnicodeText), String)
            Else
                textValue = TryCast(data.GetData(DataFormats.Text), String)
            End If

            If Not String.IsNullOrWhiteSpace(textValue) Then
                entries.AddRange(textValue.Split({Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select(Function(t) t.Trim()).Where(Function(t) Not String.IsNullOrWhiteSpace(t)))
            End If
        End If

        Return entries
    End Function

    Private Shared Function ContainsWeSimGlideLink(entries As IEnumerable(Of String)) As Boolean
        If entries Is Nothing Then
            Return False
        End If

        Return entries.Any(Function(entry) Not String.IsNullOrWhiteSpace(entry) AndAlso entry.IndexOf("wesimglide.org", StringComparison.OrdinalIgnoreCase) >= 0)
    End Function

    Private Function GetEntryExtension(entry As String) As String
        If String.IsNullOrWhiteSpace(entry) Then
            Return String.Empty
        End If

        Dim uri As Uri = Nothing
        If Uri.TryCreate(entry.Trim(), UriKind.Absolute, uri) AndAlso (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps) Then
            Return Path.GetExtension(uri.AbsolutePath)
        End If

        Return Path.GetExtension(entry)
    End Function

    Private Function ResolveDroppedEntry(entry As String, expectedExtension As String) As String
        Dim uri As Uri = Nothing
        If Uri.TryCreate(entry.Trim(), UriKind.Absolute, uri) AndAlso (uri.Scheme = Uri.UriSchemeHttp OrElse uri.Scheme = Uri.UriSchemeHttps) Then
            If Not Path.GetExtension(uri.AbsolutePath).Equals(expectedExtension, StringComparison.OrdinalIgnoreCase) Then
                Return String.Empty
            End If

            Try
                Return DownloadDroppedFile(uri)
            Catch ex As Exception
                ShowCenteredMessage($"Unable to download the dropped file: {ex.Message}", "Download error")
                Return String.Empty
            End Try
        End If

        If Not Path.GetExtension(entry).Equals(expectedExtension, StringComparison.OrdinalIgnoreCase) Then
            Return String.Empty
        End If

        If File.Exists(entry) Then
            Return entry
        End If

        ShowCenteredMessage($"The dropped file could not be found: {entry}", "File missing")
        Return String.Empty
    End Function

    Private Function DownloadDroppedFile(uri As Uri) As String
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

    Private Sub ClearAllGroupHighlights()
        SetGroupHighlight(grpPLN, False)
        SetGroupHighlight(grpPrimaryWeather, False)
        SetGroupHighlight(grpSecondaryWeather, False)
    End Sub

    Private Sub SetGroupHighlight(targetGroup As GroupBox, highlighted As Boolean)
        If targetGroup Is Nothing Then
            Return
        End If

        If highlighted Then
            targetGroup.BackColor = _dragHighlightColor
        Else
            targetGroup.BackColor = GetGroupDefaultColor(targetGroup)
        End If
    End Sub

    Private Function GetGroupDefaultColor(targetGroup As GroupBox) As Color
        If targetGroup Is grpPrimaryWeather Then
            Return _primaryGroupDefaultColor
        End If

        If targetGroup Is grpSecondaryWeather Then
            Return _secondaryGroupDefaultColor
        End If

        Return _plnGroupDefaultColor
    End Function

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
        If String.IsNullOrWhiteSpace(filePath) Then
            Return Nothing
        End If

        If Not File.Exists(filePath) Then
            If showErrors Then
                ShowCenteredMessage($"The selected weather preset could not be found: {filePath}", "Weather preset error")
            End If
            Return Nothing
        End If

        If Not Path.GetExtension(filePath).Equals(".wpr", StringComparison.OrdinalIgnoreCase) Then
            If showErrors Then
                ShowCenteredMessage("The selected file is not a valid .WPR weather preset.", "Weather preset error")
            End If
            Return Nothing
        End If

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

    Private Function GetUnpackAndLoadForm() As DPHXUnpackAndLoad
        Dim unpackForm = TryCast(Me.Owner, DPHXUnpackAndLoad)
        If unpackForm Is Nothing Then
            unpackForm = Application.OpenForms.OfType(Of DPHXUnpackAndLoad)().FirstOrDefault()
        End If

        Return unpackForm
    End Function

    Public Class ManualSelectionResult
        Public Property FlightPlanPath As String
        Public Property PrimaryWeatherLocalPath As String
        Public Property SecondaryWeatherLocalPath As String
        Public Property DisplayNamePrimary As String
        Public Property DisplayNameSecondary As String
        Public Property SSCPresetName As String
        Public Property WeatherMode As ManualWeatherSelectionMode
        Public Property TrackerGroupName As String
    End Class

    Public Enum ManualWeatherSelectionMode
        SSC_STANDARD
        CUSTOM
    End Enum

    Private Class ManualFileSelection
        Public Property FullPath As String
        Public Property DisplayName As String

        Public ReadOnly Property FileName As String
            Get
                Return If(String.IsNullOrEmpty(FullPath), String.Empty, Path.GetFileName(FullPath))
            End Get
        End Property
    End Class

    Private Class DraggedFilesInfo
        Public Property PlnPath As String
        Public Property WprPath As String

        Public ReadOnly Property HasPln As Boolean
            Get
                Return Not String.IsNullOrEmpty(PlnPath)
            End Get
        End Property

        Public ReadOnly Property HasWpr As Boolean
            Get
                Return Not String.IsNullOrEmpty(WprPath)
            End Get
        End Property
    End Class

    Private Sub ManualFallbackMode_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Me.DialogResult <> DialogResult.OK Then
            Reset()
        End If
    End Sub

End Class
