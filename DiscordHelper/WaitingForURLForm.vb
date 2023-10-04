Imports SIGLR.SoaringTools.CommonLibrary

Public Class WaitingForURLForm

    Public Sub New(message As String)
        InitializeComponent()

        ' Set the label text to the provided message
        lblWaiting.Text = message
    End Sub

    Private Sub WaitingForURLForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' Check if the clipboard contains the desired URL
        Dim clipboardText As String
        Try
            clipboardText = Clipboard.GetText
            If (Not clipboardText = String.Empty) AndAlso SupportingFeatures.IsValidURL(clipboardText) AndAlso clipboardText.Contains("discord.com/channels") Then
                Me.Close()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class