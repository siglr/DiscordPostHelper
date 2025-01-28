Imports System.Runtime.InteropServices
Imports SIGLR.SoaringTools.CommonLibrary

Public Class MsgBoxWithPicture

    Private _Continue As Boolean = True
    Private _takeMeThereURL As String = String.Empty

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
                                title As String,
                                Optional takeMeThereURL As String = "") As Boolean

        _Continue = True

        Me.Text = title
        lblMessageAbove.Text = messageAbove
        lblMessageBelow.Text = messageBelow
        PictureBox1.Image = Image.FromFile($"{Application.StartupPath}\{imageName}")

        If takeMeThereURL.Trim.Length > 0 Then
            Me.btnGoURL.Visible = True
            _takeMeThereURL = takeMeThereURL
        Else
            Me.btnGoURL.Visible = False
        End If

        btnStopExpert.Visible = True

        Me.CancelButton = btnStopExpert
        Me.AcceptButton = btnOk
        Me.ActiveControl = btnOk

        Me.ShowDialog(parent)

        Return _Continue

    End Function

    Private Sub btnGoURL_Click(sender As Object, e As EventArgs) Handles btnGoURL.Click
        SupportingFeatures.LaunchDiscordURL(_takeMeThereURL)
        SupportingFeatures.BringDPHToolToTop(Me.Handle)
    End Sub
End Class