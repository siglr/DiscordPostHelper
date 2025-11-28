Imports System.Windows.Forms
Imports System.Globalization
Imports System.Linq
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
            .Weather = If(String.IsNullOrWhiteSpace(c.WeatherPresetName), String.Empty, c.WeatherPresetName)
        }).ToList()

        dgvCandidates.DataSource = displayList
    End Sub

    Private Function FormatSimTime(dt As DateTime) As String
        If dt = DateTime.MinValue Then Return String.Empty
        Dim pattern = CultureInfo.CurrentCulture.DateTimeFormat
        Return dt.ToString("MMM dd, HH:mm", pattern)
    End Function

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
