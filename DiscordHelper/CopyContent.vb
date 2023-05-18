Public Class CopyContent

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub

    Public Sub ShowContent(parent As Form, contentCopied As String, message As String, title As String)

        Me.Text = title
        lblMessage.Text = message
        txtCopiedContent.Text = contentCopied

        Me.ShowDialog(parent)

    End Sub

End Class