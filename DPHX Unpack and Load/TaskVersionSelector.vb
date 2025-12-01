Imports System.Windows.Forms
Imports System.Globalization
Imports System.Linq
Imports System.Diagnostics
Imports SIGLR.SoaringTools.CommonLibrary

Public Partial Class TaskVersionSelector
    Inherits ZoomForm

    Private ReadOnly _candidates As List(Of IGCCacheTaskObject)

    Public Property SelectedTask As IGCCacheTaskObject

    Public Sub New(candidates As List(Of IGCCacheTaskObject))
        _candidates = candidates
        InitializeComponent()
        PopulateGrid()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Rescale()
    End Sub

    Private Sub PopulateGrid()
        Dim displayList = _candidates.Select(Function(c) New With {
            .EntrySeqID = c.EntrySeqID,
            .Title = c.TaskTitle,
            .SimTime = FormatSimTime(c.MSFSLocalDateTime),
            .Weather = If(String.IsNullOrWhiteSpace(c.WeatherPresetName), String.Empty, c.WeatherPresetName),
            .LastUpdate = FormatLastUpdate(c.LastUpdate)
        }).ToList()

        dgvCandidates.DataSource = displayList
    End Sub

    Private Function FormatSimTime(dt As DateTime) As String
        If dt = DateTime.MinValue Then Return String.Empty
        Dim pattern = CultureInfo.CurrentCulture.DateTimeFormat
        Return dt.ToString("MMM dd, HH:mm", pattern)
    End Function

    Private Function FormatLastUpdate(dt As DateTime) As String
        If dt = DateTime.MinValue Then Return String.Empty
        Dim pattern = CultureInfo.CurrentCulture
        Return dt.ToString("g", pattern)
    End Function

    Private Sub DgvCandidates_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCandidates.CellContentClick
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colEntrySeqId.Index Then
            Return
        End If

        Dim entrySeqIdValue = dgvCandidates.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        Dim entrySeqId As Integer
        If entrySeqIdValue Is Nothing OrElse Not Integer.TryParse(entrySeqIdValue.ToString(), entrySeqId) Then
            Return
        End If

        Try
            Process.Start(New ProcessStartInfo(SupportingFeatures.GetWeSimGlideTaskURL(entrySeqId)) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show("Unable to open task on WeSimGlide.org.", "Open task", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub BtnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        If dgvCandidates.SelectedRows.Count = 0 Then
            DialogResult = DialogResult.Cancel
            Return
        End If

        Dim entrySeqId As Integer = CInt(dgvCandidates.SelectedRows(0).Cells(0).Value)
        SelectedTask = _candidates.FirstOrDefault(Function(c) c.EntrySeqID = entrySeqId)
        DialogResult = If(SelectedTask IsNot Nothing, DialogResult.OK, DialogResult.Cancel)
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub GridCellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCandidates.CellDoubleClick
        If e.RowIndex >= 0 Then
            BtnOk_Click(sender, e)
        End If
    End Sub
End Class
