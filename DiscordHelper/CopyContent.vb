Imports System.Runtime.InteropServices
Imports SIGLR.SoaringTools.CommonLibrary

Public Class CopyContent

    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    Private _Continue As Boolean = True
    Private _AutoPost As Boolean = True
    Private _WaitForAction As String = String.Empty
    Private _keySequences As List(Of String) = Nothing

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
        End If

        Dim discordProcess As Process = SupportingFeatures.BringWindowToTopWithExe("Discord", "Discord.exe")
        If discordProcess IsNot Nothing Then
            For Each keySequence As String In _keySequences
                My.Computer.Keyboard.SendKeys(keySequence, True)
            Next
            If _AutoPost Then
                My.Computer.Keyboard.SendKeys("{ENTER}", True)
            End If
        End If
        Me.Focus()
        SupportingFeatures.BringDPHToolToTop(Me.Handle)

        If Not _WaitForAction = String.Empty Then

        End If

    End Sub

    Public Function ShowContent(parent As Form,
                                contentCopied As String,
                                message As String,
                                title As String,
                                Optional keySequences As List(Of String) = Nothing,
                                Optional expertMode As Boolean = False,
                                Optional autoPastePost As Boolean = True,
                                Optional waitForAction As String = "") As Boolean

        _Continue = True
        _AutoPost = autoPastePost
        _WaitForAction = waitForAction
        _keySequences = keySequences

        Me.Text = title
        lblMessage.Text = message
        txtCopiedContent.Text = contentCopied

        btnStopExpert.Visible = False
        btnStopExpert.Visible = expertMode
        Me.AcceptButton = btnOk
        If expertMode Then
            Me.CancelButton = btnStopExpert
        Else
            Me.CancelButton = btnOk
        End If

        btnOk.Focus()
        Me.ActiveControl = btnOk

        Me.ShowDialog(parent)

        Return _Continue

    End Function

End Class