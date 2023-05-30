Imports SIGLR.SoaringTools.CommonLibrary

Public Class WizardNextChoice

    Public Enum WhereToGoNext
        StopWizard = 0
        CreateTask = 1
        CreateEvent = 2
        DiscordTask = 3
        DiscordEvent = 4
    End Enum

    Public Property UserChoice As WhereToGoNext

    Private Sub btnCreateTask_Click(sender As Object, e As EventArgs) Handles btnCreateTask.Click
        UserChoice = WhereToGoNext.CreateTask
        Me.Close()
    End Sub

    Private Sub btnCreateGroupFlight_Click(sender As Object, e As EventArgs) Handles btnCreateGroupFlight.Click
        UserChoice = WhereToGoNext.CreateEvent
        Me.Close()
    End Sub

    Private Sub btnPostTask_Click(sender As Object, e As EventArgs) Handles btnPostTask.Click
        UserChoice = WhereToGoNext.DiscordTask
        Me.Close()
    End Sub

    Private Sub btnPostGroupFlight_Click(sender As Object, e As EventArgs) Handles btnPostGroupFlight.Click
        UserChoice = WhereToGoNext.DiscordEvent
        Me.Close()
    End Sub

    Private Sub btnStopWizard_Click(sender As Object, e As EventArgs) Handles btnStopWizard.Click
        UserChoice = WhereToGoNext.StopWizard
        Me.Close()
    End Sub

End Class