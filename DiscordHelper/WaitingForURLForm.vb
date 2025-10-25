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
        Try
            Dim clipboardText As String = Clipboard.GetText()

            If clipboardText <> String.Empty AndAlso SupportingFeatures.IsValidURL(clipboardText) Then

                Dim lowerText As String = clipboardText.ToLowerInvariant()

                If lowerText.Contains("discord.com/channels/") OrElse
                   lowerText.Contains("discordapp.com/channels/") Then

                    If _AcceptMSFSSoaringToolsOnly Then
                        Dim guildId As String = SupportingFeatures.GetMSFSSoaringToolsDiscordID()

                        If lowerText.Contains($"discord.com/channels/{guildId}/") OrElse
                           lowerText.Contains($"discordapp.com/channels/{guildId}/") Then

                            Timer1.Enabled = False
                            Me.DialogResult = DialogResult.OK
                            Me.Close()
                        End If
                    Else
                        Timer1.Enabled = False
                        Me.DialogResult = DialogResult.OK
                        Me.Close()
                    End If
                End If
            End If

        Catch ex As Exception
#If DEBUG Then
            Debug.WriteLine("Timer1_Tick clipboard check failed: " & ex.Message)
#End If
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class