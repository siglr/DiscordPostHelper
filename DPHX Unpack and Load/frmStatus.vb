Public Class frmStatus

    Private _formToEnable As Form
    Private Shared _done As Boolean = False

    ' ---- Append lines of text to txtStatus ----
    Public Sub AppendStatusLine(message As String, addBlankLine As Boolean)
        ' Safely update the textbox from any thread.
        If txtStatus.InvokeRequired Then
            txtStatus.Invoke(Sub() AppendStatusLine(message, addBlankLine))
        Else
            txtStatus.AppendText(message & Environment.NewLine)
            If addBlankLine Then
                txtStatus.AppendText(Environment.NewLine)
            End If
        End If
    End Sub

    Public Sub Start(formToEnable As Form)
        txtStatus.Text = String.Empty
        Clear()
        _formToEnable = formToEnable
        _formToEnable.Enabled = False
        btnClose.Enabled = False
        _done = False

        ' Manually center this form over the parent
        Me.StartPosition = FormStartPosition.Manual
        Dim newX As Integer = formToEnable.Left + (formToEnable.Width - Me.Width) \ 2
        Dim newY As Integer = formToEnable.Top + (formToEnable.Height - Me.Height) \ 2
        Me.Location = New Point(newX, newY)

        If Not Me.Visible Then
            Me.Show(formToEnable)
        End If

    End Sub

    Private Sub Clear()
        ' Safely update the textbox from any thread.
        If txtStatus.InvokeRequired Then
            txtStatus.Invoke(Sub() Clear())
        Else
            txtStatus.Text = String.Empty
        End If
    End Sub

    ' ---- Enable the Close button so the user can dismiss the form ----
    Public Sub Done()
        _done = True
        btnClose.Enabled = True
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If _done Then
            CloseForm()
        End If
    End Sub

    Private Sub CloseForm()
        If _formToEnable IsNot Nothing Then
            _formToEnable.Enabled = True
        End If
        Me.Hide()
    End Sub

End Class
