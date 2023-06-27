Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Module MainModule

    Public Sub Main()
        Dim argNo As Integer = 0

        Dim zipFilename As String = String.Empty
        Dim processID As Integer = 0
        Dim programToStart As String = String.Empty

        Dim updateForm As New UpdaterForm

        updateForm.Show()
        updateForm.Refresh()

        updateForm.txtParamCount.Text = My.Application.CommandLineArgs.Count.ToString

        For Each arg In My.Application.CommandLineArgs
            argNo += 1
            Select Case argNo
                Case 1
                    'Zip file
                    zipFilename = arg
                    updateForm.txtZipFile.Text = zipFilename
                Case 2
                    'Process ID
                    processID = CInt(arg)
                    updateForm.txtProcessID.Text = processID.ToString
                Case 3
                    'Program to start after update
                    programToStart = arg
                    updateForm.txtApplication.Text = programToStart
            End Select
        Next

        updateForm.Refresh()

        Try
            Dim callingProcess As Process = Process.GetProcessById(processID)
            If updateForm.ShowWaitingForProcess(callingProcess) Then
                'Exited cleanly
            Else
                'Aborted
                Exit Sub
            End If

        Catch ex As Exception
            'Process is already closed
        End Try
        updateForm.CallerIsTerminated()

        'Discord Post Helper
        For Each relatedProcess In Process.GetProcessesByName("DiscordPostHelper")
            If $"{Path.GetDirectoryName(relatedProcess.MainModule.FileName)}\" = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory) Then
                If updateForm.ShowWaitingForProcess(relatedProcess) Then
                    'Exited cleanly
                Else
                    'Aborted
                    Exit Sub
                End If
            End If
        Next
        'DPHX Unpack & Load
        For Each relatedProcess In Process.GetProcessesByName("DPHX Unpack and Load")
            If $"{Path.GetDirectoryName(relatedProcess.MainModule.FileName)}\" = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory) Then
                If updateForm.ShowWaitingForProcess(relatedProcess) Then
                    'Exited cleanly
                Else
                    'Aborted
                    Exit Sub
                End If
            End If
        Next
        updateForm.OtherProcessesTerminated()

        Try
            Using archive As ZipArchive = ZipFile.OpenRead(zipFilename)
                For Each entry As ZipArchiveEntry In archive.Entries
                    If entry.Name <> "Updater.exe" Then
                        updateForm.AddUnzippedFile($"Extracting {AppDomain.CurrentDomain.BaseDirectory}{entry.Name}")
                        updateForm.Refresh()
                        Application.DoEvents()
                        entry.ExtractToFile($"{AppDomain.CurrentDomain.BaseDirectory}{entry.Name}", True)
                    End If
                Next
            End Using
            updateForm.AllFilesUpdated()

            If File.Exists(zipFilename) Then
                File.Delete(zipFilename)
            End If
            updateForm.ZipFileDeleted()

            updateForm.btnUpdateCompleted.Visible = True

            While updateForm.Visible
                Application.DoEvents() ' Allow the form to update and respond to events
            End While

        Catch ex As Exception

            Dim sb As New StringBuilder()
            sb.appendline("An error occured! Incomplete update! You should get the latest release by yourself and copy the files over manually!")
            sb.AppendLine()
            sb.AppendLine(ex.Message.ToString)
            MessageBox.Show(sb.ToString, "MSFS Soaring Task Tools Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub

        End Try

        Try
            Dim startInfo As New ProcessStartInfo(programToStart)
            Process.Start(startInfo)

        Catch ex As Exception
            Dim sb As New StringBuilder()
            sb.AppendLine("An error occured while trying to restart the program!")
            sb.AppendLine(programToStart)
            sb.AppendLine()
            sb.AppendLine(ex.Message.ToString)
            MessageBox.Show(sb.ToString, "MSFS Soaring Task Tools Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

End Module
