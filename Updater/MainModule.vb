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
        ' Define the list of files to be skipped if they already exist
        Dim filesToSkip As New List(Of String) From {"TasksDatabase.db"}

        Dim updateForm As New UpdaterForm

        updateForm.Show()
        updateForm.Refresh()

        updateForm.txtParamCount.Text = My.Application.CommandLineArgs.Count.ToString

        If My.Application.CommandLineArgs.Count = 3 Then
            updateForm.lblChkMarkParameters.Visible = True
        Else
            Dim sb As New StringBuilder()
            sb.AppendLine("An error occurred! Incomplete update! You should get the latest release by yourself and copy the files over manually!")
            sb.AppendLine()
            sb.AppendLine("Incorrect number of parameters passed to the Updater process.")
            MessageBox.Show(sb.ToString, "MSFS Soaring Task Tools Updater", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        For Each arg In My.Application.CommandLineArgs
            argNo += 1
            Select Case argNo
                Case 1
                    ' Zip file
                    zipFilename = arg
                    updateForm.txtZipFile.Text = zipFilename
                Case 2
                    ' Process ID
                    processID = CInt(arg)
                    updateForm.txtProcessID.Text = processID.ToString
                Case 3
                    ' Program to start after update
                    programToStart = arg
                    updateForm.txtApplication.Text = programToStart
            End Select
        Next

        updateForm.Refresh()

        ' Wait for the main process to exit
        If processID <> 0 Then
            Try
                Dim callingProcess As Process = Process.GetProcessById(processID)
                If Not updateForm.ShowWaitingForProcess(callingProcess) Then
                    Exit Sub
                End If
            Catch ex As Exception
                ' Already exited
            End Try
        End If
        updateForm.CallerIsTerminated()

        ' Wait for DiscordPostHelper
        For Each relatedProcess In Process.GetProcessesByName("DiscordPostHelper")
            If $"{Path.GetDirectoryName(relatedProcess.MainModule.FileName)}\" = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory) Then
                If Not updateForm.ShowWaitingForProcess(relatedProcess) Then
                    Exit Sub
                End If
            End If
        Next

        ' Wait for DPHX Unpack and Load
        For Each relatedProcess In Process.GetProcessesByName("DPHX Unpack and Load")
            If $"{Path.GetDirectoryName(relatedProcess.MainModule.FileName)}\" = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory) Then
                If Not updateForm.ShowWaitingForProcess(relatedProcess) Then
                    Exit Sub
                End If
            End If
        Next

        updateForm.OtherProcessesTerminated()

        Try
            ' --- 1) Build normalized base directory path ---
            Dim baseDir As String = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory)
            If Not baseDir.EndsWith(Path.DirectorySeparatorChar) Then
                baseDir &= Path.DirectorySeparatorChar
            End If

            ' --- 2) Read cleanup list from the ZIP ---
            Dim toRemove As New List(Of String)
            Using archive As ZipArchive = ZipFile.OpenRead(zipFilename)
                Dim cleanupEntry = archive.GetEntry("_FilesToRemove.txt")
                If cleanupEntry IsNot Nothing Then
                    Using sr As New StreamReader(cleanupEntry.Open())
                        While Not sr.EndOfStream
                            Dim line = sr.ReadLine().Trim()
                            If line <> String.Empty Then
                                toRemove.Add(line)
                            End If
                        End While
                    End Using
                End If
            End Using

            ' --- 3) Delete files/folders safely ---
            For Each relPath In toRemove
                ' Skip empty or absolute paths
                If String.IsNullOrWhiteSpace(relPath) OrElse Path.IsPathRooted(relPath) Then
                    updateForm.AddUnzippedFile($"Skipping invalid removal entry `{relPath}`")
                    Continue For
                End If

                ' Resolve to full path
                Dim candidate As String = Path.GetFullPath(Path.Combine(baseDir, relPath))

                ' Verify it’s still under baseDir
                If Not candidate.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase) Then
                    updateForm.AddUnzippedFile($"Unsafe removal entry skipped: `{relPath}`")
                    Continue For
                End If

                ' Perform deletion
                If File.Exists(candidate) Then
                    File.Delete(candidate)
                    updateForm.AddUnzippedFile($"Deleted file {relPath}")
                ElseIf Directory.Exists(candidate) Then
                    Directory.Delete(candidate, recursive:=True)
                    updateForm.AddUnzippedFile($"Deleted folder {relPath}")
                End If

                updateForm.Refresh()
                Application.DoEvents()
            Next

            ' --- 4) Extract ZIP (skipping Updater.exe and respecting filesToSkip) ---
            Using archive As ZipArchive = ZipFile.OpenRead(zipFilename)
                For Each entry As ZipArchiveEntry In archive.Entries
                    If entry.Name <> "Updater.exe" AndAlso entry.Name <> "_FilesToRemove.txt" Then
                        Dim entryFullName As String = entry.FullName
                        Dim entryDirectory As String = Path.GetDirectoryName(entryFullName.Replace("/", "\"))

                        ' Combine the base directory with the entry's path to get the destination path
                        Dim destinationPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, entryFullName.Replace("/", "\"))

                        ' Check if the file is in the list of files to be skipped and already exists
                        If filesToSkip.Contains(entry.Name) AndAlso File.Exists(destinationPath) Then
                            updateForm.AddUnzippedFile($"Skipping existing file {destinationPath}")
                            updateForm.Refresh()
                            Application.DoEvents()
                            Continue For
                        End If

                        If entry.Name = String.Empty Then
                            ' Create the directory if it doesn't exist
                            If entryDirectory <> String.Empty And Not Directory.Exists(entryDirectory) Then
                                Directory.CreateDirectory(entryDirectory)
                                updateForm.AddUnzippedFile($"Creating folder {entryDirectory}")
                                updateForm.Refresh()
                                Application.DoEvents()
                            End If
                        Else
                            updateForm.AddUnzippedFile($"Extracting {destinationPath}")
                            updateForm.Refresh()
                            Application.DoEvents()
                            ' Extract the entry to the specified destination path
                            entry.ExtractToFile(destinationPath, True)
                        End If

                    End If
                Next
            End Using
            updateForm.AllFilesUpdated()

            Dim cleanupOnDisk = Path.Combine(baseDir, "_FilesToRemove.txt")
            If File.Exists(cleanupOnDisk) Then
                File.Delete(cleanupOnDisk)
                updateForm.AddUnzippedFile("Deleted cleanup definition file _FilesToRemove.txt")
                updateForm.Refresh()
                Application.DoEvents()
            End If

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
            sb.AppendLine("An error occured! Incomplete update! You should get the latest release by yourself and copy the files over manually!")
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
