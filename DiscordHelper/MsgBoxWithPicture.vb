Imports System.Runtime.InteropServices
Imports SIGLR.SoaringTools.CommonLibrary

Public Class MsgBoxWithPicture

    Private _Continue As Boolean = True

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub

    Private Sub btnStopExpert_Click(sender As Object, e As EventArgs) Handles btnStopExpert.Click
        _Continue = False
        Me.Close()
    End Sub



    Public Function ShowContent(parent As Form,
                                imageName As String,
                                messageAbove As String,
                                messageBelow As String,
                                title As String) As Boolean

        _Continue = True

        Me.Text = title
        lblMessageAbove.Text = messageAbove
        lblMessageBelow.Text = messageBelow
        PictureBox1.Image = Image.FromFile($"{Application.StartupPath}\{imageName}")

        btnStopExpert.Visible = True

        Me.CancelButton = btnStopExpert
        Me.AcceptButton = btnOk
        Me.ActiveControl = btnOk

        Me.ShowDialog(parent)

        Return _Continue

    End Function

End Class