Imports System.Drawing
Imports System.Windows.Forms

Public Class UpdaterForm

    Public Property AllowCheckChange As Boolean = False

    Private Sub UpdaterForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CheckBoxes_CheckedChanged(sender As Object, e As EventArgs) Handles chkCallerIsTerminated.CheckedChanged,
                                                                                    chkOtherSharedProcessesTerminated.CheckedChanged,
                                                                                    chkAllFilesUpdated.CheckedChanged,
                                                                                    chkZipFileDeleted.CheckedChanged
        If AllowCheckChange Then
            ' Handle the logic for programmatic change here
            AllowCheckChange = False
        Else
            ' Reset the checkbox state back to the original value
            AllowCheckChange = True
            DirectCast(sender, CheckBox).Checked = Not DirectCast(sender, CheckBox).Checked
        End If
    End Sub

    Public Sub OtherProcessesTerminated()
        AllowCheckChange = True
        chkOtherSharedProcessesTerminated.Checked = True
    End Sub

    Public Sub CallerIsTerminated()
        AllowCheckChange = True
        chkCallerIsTerminated.Checked = True
    End Sub

    Public Sub AllFilesUpdated()
        AllowCheckChange = True
        chkAllFilesUpdated.Checked = True
    End Sub

    Public Sub ZipFileDeleted()
        AllowCheckChange = True
        chkZipFileDeleted.Checked = True
    End Sub

    Public Function ShowWaitingForProcess(processToWaitFor As Process) As Boolean

        Dim waitToClose As New WaitingToCloseApp(processToWaitFor)

        waitToClose.Label1.Text = $"Please close the process:{Environment.NewLine}{processToWaitFor.MainModule.FileName}"
        If waitToClose.ShowDialog(Me) = DialogResult.OK Then
            'Process closed
            Return True
        Else
            'Aborted
            Return False
        End If

    End Function

    Public Sub AddUnzippedFile(text As String)

        txtUpdatedFiles.Text = ($"{txtUpdatedFiles.Text}{text}{Environment.NewLine}")
        Me.Refresh()

    End Sub

    Private Sub btnUpdateCompleted_Click(sender As Object, e As EventArgs) Handles btnUpdateCompleted.Click
        Me.Close()

    End Sub
End Class