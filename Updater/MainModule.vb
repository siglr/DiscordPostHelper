Imports System.IO
Imports System.IO.Compression

Module MainModule

    Sub Main()
        Dim argNo As Integer = 0

        Dim zipFilename As String = String.Empty
        Dim processID As Integer = 0
        Dim programToStart As String = String.Empty

        Console.WriteLine($"Number of arguments: {My.Application.CommandLineArgs.Count}")
        For Each arg In My.Application.CommandLineArgs
            argNo += 1
            Select Case argNo
                Case 1
                    'Zip file
                    zipFilename = arg
                Case 2
                    'Process ID
                    processID = CInt(arg)
                Case 3
                    'Program to start after update
                    programToStart = arg
            End Select
            Console.WriteLine($"Argument #{argNo.ToString}: {arg}")
        Next

        Try
            Console.WriteLine($"Trying to get process ID {processID}")
            Dim callingProcess As Process = Process.GetProcessById(processID)
            Console.WriteLine($"Please close the calling process: {callingProcess.MainModule.FileName}")
            callingProcess.WaitForExit()

        Catch ex As Exception
            'Process is already closed
        End Try

        Console.WriteLine($"Calling process ended.")
        Console.WriteLine($"Checking for other related processes running.")

        'Discord Post Helper
        Console.WriteLine($"Checking for conflicting instances of Discord Post Helper running.")
        For Each relatedProcess In Process.GetProcessesByName("DiscordPostHelper")
            If $"{Path.GetDirectoryName(relatedProcess.MainModule.FileName)}\" = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory) Then
                Console.WriteLine($"Please close this process: {relatedProcess.MainModule.FileName}")
                relatedProcess.WaitForExit()
            End If
        Next

        'Soaring Task Browsers
        Console.WriteLine($"Checking for conflicting instances of Soaring Task Browser running.")
        For Each relatedProcess In Process.GetProcessesByName("SoaringTaskBrowser")
            If $"{Path.GetDirectoryName(relatedProcess.MainModule.FileName)}\" = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory) Then
                Console.WriteLine($"Please close this process: {relatedProcess.MainModule.FileName}")
                relatedProcess.WaitForExit()
            End If
        Next

        Console.WriteLine($"All related processes are closed - Starting the file updates.")

        Try
            Using archive As ZipArchive = ZipFile.OpenRead(zipFilename)
                For Each entry As ZipArchiveEntry In archive.Entries
                    If entry.Name <> "Updater.exe" Then
                        Console.WriteLine($"Extracting {AppDomain.CurrentDomain.BaseDirectory}{entry.Name}")
                        entry.ExtractToFile($"{AppDomain.CurrentDomain.BaseDirectory}{entry.Name}", True)
                    End If
                Next
            End Using

            Console.WriteLine($"Update completed - Launching the calling process back.")
            Console.WriteLine("")
            Console.WriteLine("Press ENTER to continue")
            Console.ReadLine()
            Dim startInfo As New ProcessStartInfo(programToStart)
            Process.Start(startInfo)

        Catch ex As Exception
            Console.WriteLine("An error occured! Incomplete update! You should get the latest release by yourself and copy the files over manually!")
            Console.WriteLine(ex.Message.ToString)
            Console.WriteLine("")
            Console.WriteLine("Press ENTER to continue")
            Console.ReadLine()

        End Try

    End Sub

End Module
