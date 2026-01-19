Imports System
Imports System.Collections.Generic

Public Class CleanupConfirmForm
    Private ReadOnly _candidates As List(Of CleanupCandidate)
    Private ReadOnly _deleteFunc As Func(Of String, String, String, Boolean, String)
    Private ReadOnly _additionalActions As List(Of Func(Of String))
    Private _ran As Boolean = False

    ' New ctor: pass candidates and your DeleteFile delegate
    Public Sub New(candidates As List(Of CleanupCandidate),
                   deleteFunc As Func(Of String, String, String, Boolean, String),
                   Optional additionalActions As IEnumerable(Of Func(Of String)) = Nothing)
        InitializeComponent()
        _candidates = candidates
        _deleteFunc = deleteFunc
        If additionalActions IsNot Nothing Then
            _additionalActions = New List(Of Func(Of String))(additionalActions)
        Else
            _additionalActions = New List(Of Func(Of String))()
        End If

        clbFiles.CheckOnClick = True
        clbFiles.Items.Clear()
        For Each c In _candidates
            clbFiles.Items.Add(c.Display, c.DefaultChecked) ' checked by default
        Next

        txtResults.Clear()
    End Sub

    Private Sub CleanupConfirmForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Rescale()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If Not _ran Then
            ' 1) Run deletions for checked items, print results in txtResults
            Dim checked As New System.Collections.Generic.HashSet(Of String)(
                StringComparer.OrdinalIgnoreCase)
            For Each o In clbFiles.CheckedItems
                checked.Add(o.ToString())
            Next

            Dim sb As New Text.StringBuilder()
            If checked.Count = 0 Then
                sb.AppendLine("No files selected. Nothing was deleted.")
            Else
                For Each c In _candidates
                    If checked.Contains(c.Display) Then
                        ' Exclude flag is False here because candidates were pre-filtered
                        sb.AppendLine(_deleteFunc(c.FileName, c.SourcePath, c.Label, False))
                    Else
                        sb.AppendLine($"{c.Label} ""{c.FileName}"" skipped by user")
                    End If
                    sb.AppendLine()
                Next
            End If

            If _additionalActions.Count > 0 Then
                For Each action In _additionalActions
                    Dim line As String
                    Try
                        line = action()
                    Catch ex As Exception
                        line = $"Additional cleanup step failed: {ex.Message}"
                    End Try

                    If String.IsNullOrWhiteSpace(line) Then
                        line = "Additional cleanup step completed."
                    End If

                    sb.AppendLine(line)
                Next
            End If

            txtResults.Text = sb.ToString()

            ' 2) Lock UI and turn Delete into Close
            clbFiles.Enabled = False
            btnCancel.Enabled = False
            btnDelete.Text = "Close"
            _ran = True
        Else
            ' Close after showing results
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
