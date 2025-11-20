Imports System
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Windows.Forms
Imports System.Xml.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Partial Public Class ManualFallbackMode
    Inherits ZoomForm

    Private ReadOnly _sf As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _selectedPln As ManualFileSelection
    Private _selectedWpr As ManualFileSelection
    Private _whitelistPresets As New List(Of WeatherPresetOption)
    Private _plnGroupDefaultColor As Color
    Private _weatherGroupDefaultColor As Color
    Private ReadOnly _dragHighlightColor As Color = ControlPaint.LightLight(SystemColors.Control)
    Private _selectionResult As ManualSelectionResult

    Public Property InitialPlnPath As String
    Public Property InitialWprPath As String
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
        _weatherGroupDefaultColor = grpWeather.BackColor
    End Sub

    Private Sub ManualFallbackMode_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadWhitelistPresets()
        Reset()
        ApplyInitialSelection()

        Me.AcceptButton = btnCopyGoFly
        Me.CancelButton = btnClearFiles
        btnClose.Visible = False
        btnClose.Enabled = False

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
        _selectionResult = Nothing

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

        If Not String.IsNullOrWhiteSpace(InitialWprPath) AndAlso File.Exists(InitialWprPath) Then
            Dim selection = LoadWprSelection(InitialWprPath, False)
            If selection IsNot Nothing Then
                _selectedWpr = selection
                lblWPRFile.Text = selection.FileName
                lblWPRName.Text = selection.DisplayName
            End If
        End If

        If Not String.IsNullOrWhiteSpace(InitialTrackerGroup) Then
            txtTrackerGroupName.Text = InitialTrackerGroup
        End If
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

        If _selectedWpr Is Nothing Then
            ShowCenteredMessage("A weather preset is required before continuing.", "Weather preset missing")
            Return
        End If

        _selectionResult = New ManualSelectionResult With {
            .FlightPlanPath = _selectedPln.FullPath,
            .WeatherPath = _selectedWpr.FullPath,
            .TrackerGroupName = txtTrackerGroupName.Text.Trim()
        }

        Dim unpackForm = GetUnpackAndLoadForm()
        If unpackForm IsNot Nothing Then
            unpackForm.UpdateManualFallbackPaths(_selectedPln.FullPath, _selectedWpr.FullPath, _selectionResult.TrackerGroupName)
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnClearFiles_Click(sender As Object, e As EventArgs) Handles btnClearFiles.Click

        _selectionResult = Nothing
        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

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

    Private Sub grpPLN_DragEnter(sender As Object, e As DragEventArgs) Handles grpPLN.DragEnter
        HandleGroupDragEnter(e, grpPLN)
    End Sub

    Private Sub grpPLN_DragOver(sender As Object, e As DragEventArgs) Handles grpPLN.DragOver
        HandleGroupDragOver(e, grpPLN)
    End Sub

    Private Sub grpPLN_DragLeave(sender As Object, e As EventArgs) Handles grpPLN.DragLeave
        ClearAllGroupHighlights()
    End Sub

    Private Sub grpPLN_DragDrop(sender As Object, e As DragEventArgs) Handles grpPLN.DragDrop
        HandleGroupDragDrop(e, grpPLN)
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

    Private Sub grpWeather_DragEnter(sender As Object, e As DragEventArgs) Handles grpWeather.DragEnter
        HandleGroupDragEnter(e, grpWeather)
    End Sub

    Private Sub grpWeather_DragOver(sender As Object, e As DragEventArgs) Handles grpWeather.DragOver
        HandleGroupDragOver(e, grpWeather)
    End Sub

    Private Sub grpWeather_DragLeave(sender As Object, e As EventArgs) Handles grpWeather.DragLeave
        ClearAllGroupHighlights()
    End Sub

    Private Sub grpWeather_DragDrop(sender As Object, e As DragEventArgs) Handles grpWeather.DragDrop
        HandleGroupDragDrop(e, grpWeather)
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

    Private Function GetUnpackAndLoadForm() As DPHXUnpackAndLoad
        Dim unpackForm = TryCast(Me.Owner, DPHXUnpackAndLoad)
        If unpackForm Is Nothing Then
            unpackForm = Application.OpenForms.OfType(Of DPHXUnpackAndLoad)().FirstOrDefault()
        End If

        Return unpackForm
    End Function
    Public Class ManualSelectionResult
        Public Property FlightPlanPath As String
        Public Property WeatherPath As String
        Public Property TrackerGroupName As String
    End Class

    Private Class ManualFileSelection
        Public Property FullPath As String
        Public Property DisplayName As String

        Public ReadOnly Property FileName As String
            Get
                Return If(String.IsNullOrEmpty(FullPath), String.Empty, Path.GetFileName(FullPath))
            End Get
        End Property
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

    Private Sub HandleGroupDragEnter(e As DragEventArgs, targetGroup As GroupBox)
        Dim draggedFiles = GetDraggedFilesInfo(e, False)
        UpdateGroupHighlights(draggedFiles)

        If ShouldAcceptDrop(targetGroup, draggedFiles) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub HandleGroupDragOver(e As DragEventArgs, targetGroup As GroupBox)
        Dim draggedFiles = GetDraggedFilesInfo(e, False)
        UpdateGroupHighlights(draggedFiles)

        If ShouldAcceptDrop(targetGroup, draggedFiles) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub HandleGroupDragDrop(e As DragEventArgs, targetGroup As GroupBox)
        Dim draggedFiles = GetDraggedFilesInfo(e, True)
        ClearAllGroupHighlights()

        If Not ShouldAcceptDrop(targetGroup, draggedFiles) Then
            Return
        End If

        If draggedFiles.HasPln Then
            Dim selection = LoadPlnSelection(draggedFiles.PlnPath)
            If selection IsNot Nothing Then
                _selectedPln = selection
                lblPLNFile.Text = selection.FileName
                lblPLNTitle.Text = selection.DisplayName
            End If
        End If

        If draggedFiles.HasWpr Then
            Dim selection = LoadWprSelection(draggedFiles.WprPath, True)
            If selection IsNot Nothing Then
                _selectedWpr = selection
                lblWPRFile.Text = selection.FileName
                lblWPRName.Text = selection.DisplayName
                cboWhitelistPresets.SelectedIndex = -1
            End If
        End If
    End Sub

    Private Function ShouldAcceptDrop(targetGroup As GroupBox, draggedFiles As DraggedFilesInfo) As Boolean
        If draggedFiles Is Nothing Then
            Return False
        End If

        If targetGroup Is grpPLN Then
            Return draggedFiles.HasPln
        End If

        If targetGroup Is grpWeather Then
            Return draggedFiles.HasWpr
        End If

        Return False
    End Function

    Private Function GetDraggedFilesInfo(e As DragEventArgs, resolveDownloads As Boolean) As DraggedFilesInfo
        Dim info As New DraggedFilesInfo()

        If e Is Nothing OrElse e.Data Is Nothing Then
            Return info
        End If

        Dim entries = ExtractDragEntries(e.Data)
        If entries.Count = 0 Then
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

        Dim fileName = Path.GetFileName(uri.LocalPath)
        If String.IsNullOrWhiteSpace(fileName) Then
            fileName = $"dropped_{Guid.NewGuid():N}{Path.GetExtension(uri.LocalPath)}"
        End If

        Dim targetPath = Path.Combine(downloadFolder, fileName)
        If File.Exists(targetPath) Then
            targetPath = Path.Combine(downloadFolder, $"{Path.GetFileNameWithoutExtension(fileName)}{Path.GetExtension(fileName)}")
        End If

        Using client As New WebClient()
            client.DownloadFile(uri, targetPath)
        End Using

        Return targetPath
    End Function

    Private Sub UpdateGroupHighlights(draggedFiles As DraggedFilesInfo)
        Dim hasPln = draggedFiles IsNot Nothing AndAlso draggedFiles.HasPln
        Dim hasWpr = draggedFiles IsNot Nothing AndAlso draggedFiles.HasWpr

        SetGroupHighlight(grpPLN, hasPln)
        SetGroupHighlight(grpWeather, hasWpr)
    End Sub

    Private Sub ClearAllGroupHighlights()
        SetGroupHighlight(grpPLN, False)
        SetGroupHighlight(grpWeather, False)
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
        If targetGroup Is grpWeather Then
            Return _weatherGroupDefaultColor
        End If

        Return _plnGroupDefaultColor
    End Function

End Class
