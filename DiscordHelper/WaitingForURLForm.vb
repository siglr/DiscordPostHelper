Imports SIGLR.SoaringTools.CommonLibrary

Public Class WaitingForURLForm

    Private _AcceptMSFSSoaringToolsOnly As Boolean = True

    Public Sub New(message As String, Optional onlyFromMSFSSoaringTools As Boolean = True)
        InitializeComponent()

        ' Set the label text to the provided message
        lblWaiting.Text = message

        _AcceptMSFSSoaringToolsOnly = onlyFromMSFSSoaringTools

    End Sub

    Private Sub WaitingForURLForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' Check if the clipboard contains the desired URL
        Dim clipboardText As String
        Try
            clipboardText = Clipboard.GetText
            If (Not clipboardText = String.Empty) AndAlso SupportingFeatures.IsValidURL(clipboardText) AndAlso clipboardText.Contains($"discord.com/channels/") Then
                If _AcceptMSFSSoaringToolsOnly Then
                    If clipboardText.Contains($"discord.com/channels/{SupportingFeatures.GetMSFSSoaringToolsDiscordID}/") Then
                        Me.DialogResult = DialogResult.OK
                        Me.Close()
                    End If
                Else
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class