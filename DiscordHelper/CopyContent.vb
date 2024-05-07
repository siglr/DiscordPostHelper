Imports System.IO
Imports System.Runtime.InteropServices
Imports SIGLR.SoaringTools.CommonLibrary

Public Class CopyContent

    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    Private _Continue As Boolean = True
    Private _AutoPost As Boolean = True
    Private _keySequences As List(Of String) = Nothing
    Private _msToWaitAfterPost As Integer
    Private _expertMode As Boolean

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub

    Private Sub btnStopExpert_Click(sender As Object, e As EventArgs) Handles btnStopExpert.Click
        _Continue = False
        Me.Close()
    End Sub

    Private Sub btnAutoPaste_Click(sender As Object, e As EventArgs) Handles btnAutoPaste.Click

        If Clipboard.ContainsText Then
            Clipboard.SetText(txtCopiedContent.Text)
            Threading.Thread.Sleep(250)
            Application.DoEvents()
        End If

        Dim discordProcess As IntPtr = SupportingFeatures.BringWindowToTopWithPartialTitle(" - Discord")
        If Not discordProcess = IntPtr.Zero Then
            For Each keySequence As String In _keySequences
                My.Computer.Keyboard.SendKeys(keySequence, True)
            Next
            If _AutoPost Then
                My.Computer.Keyboard.SendKeys("{ENTER}", True)
            End If
            Me.Cursor = Cursors.WaitCursor
            lblMessage.Text = "Waiting a bit for Discord to complete posting."
            Application.DoEvents()
            Threading.Thread.Sleep(500)
            Threading.Thread.Sleep(_msToWaitAfterPost)
            Me.Cursor = Cursors.Default
            SupportingFeatures.BringDPHToolToTop(Me.Handle)
            Me.Close()
        Else
            Using New Centered_MessageBox(Me)
                MessageBox.Show(Me, $"The Discord app can't be found / is not running.{Environment.NewLine}{Environment.NewLine}If you use Discord on a browser, you will have to paste and post manually.", "Discord app not running", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Sub

    Public Function ShowContent(parent As Form,
                                contentCopied As String,
                                message As String,
                                title As String,
                                Optional keySequences As List(Of String) = Nothing,
                                Optional expertMode As Boolean = False,
                                Optional autoPastePost As Boolean = True,
                                Optional msToWaitAfterPost As Integer = 500,
                                Optional imageToDisplay As Image = Nothing) As Boolean

        _Continue = True
        _AutoPost = autoPastePost
        _keySequences = keySequences
        _msToWaitAfterPost = msToWaitAfterPost
        _expertMode = expertMode

        Me.Text = title
        lblMessage.Text = message

        txtCopiedContent.Text = contentCopied
        imgImageInClipboard.Visible = False
        If imageToDisplay IsNot Nothing Then
            imgImageInClipboard.Visible = True
            imgImageInClipboard.Image = imageToDisplay
        End If

        btnStopExpert.Visible = True

        Me.CancelButton = btnStopExpert
        Me.AcceptButton = btnAutoPaste
        btnAutoPaste.Focus()
        Me.ActiveControl = btnAutoPaste

        Me.ShowDialog(parent)

        Return _Continue

    End Function

    Private Sub CopyContent_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        If _expertMode Then
            btnAutoPaste_Click(sender, e)
        End If

    End Sub
End Class