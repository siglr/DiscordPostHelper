Imports SIGLR.SoaringTools.CommonLibrary

Public Class CleanupConfirmForm
    Private ReadOnly _candidates As List(Of CleanupCandidate)
    Private ReadOnly _deleteFunc As Func(Of String, String, String, Boolean, String)
    Private _ran As Boolean = False

    ' New ctor: pass candidates and your DeleteFile delegate
    Public Sub New(candidates As List(Of CleanupCandidate),
                   deleteFunc As Func(Of String, String, String, Boolean, String))
        InitializeComponent()
        _candidates = candidates
        _deleteFunc = deleteFunc

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

    Private Sub clbFiles_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbFiles.ItemCheck
        ' Fires BEFORE the check state changes; use e.NewValue to gate
        If e.NewValue = CheckState.Checked Then
            Dim cand As CleanupCandidate = _candidates(e.Index)
            If cand IsNot Nothing AndAlso cand.IsWhitelistProtected Then
                Using New Centered_MessageBox(Me)
                    Dim res = MessageBox.Show(Me,
                        $"{cand.FileName} matches a Whitelist-protected copy and was left unchecked by default.{Environment.NewLine}{Environment.NewLine}Delete it anyway?",
                    "Whitelist protection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning)
                    If res = DialogResult.No Then
                        e.NewValue = CheckState.Unchecked   ' cancel the check
                    End If
                End Using
            End If
        End If
    End Sub

End Class
