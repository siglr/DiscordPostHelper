Imports System.Data.SQLite
Imports SIGLR.SoaringTools.CommonLibrary
Imports System.IO

Public Class DatabaseUpdate

    Public Const ExpectedDatabaseVersion As Integer = 0
    Private Shared _localTasksDatabaseFilePath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), SupportingFeatures.TasksDatabase)
    Private Shared _updateScriptsDatabaseFilePath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DBUpdates.db")

    Public Shared Sub CheckAndUpdateDatabase()
        Dim currentDbVersion As Integer = GetDatabaseVersion()

        If currentDbVersion < ExpectedDatabaseVersion Then
            ApplyDatabaseUpdates(currentDbVersion)
        End If
    End Sub

    Private Shared Function GetDatabaseVersion() As Integer
        Dim version As Integer = 0
        Dim connectionString As String = $"Data Source={_localTasksDatabaseFilePath};Version=3;"
        Using conn As New SQLiteConnection(connectionString)
            conn.Open()
            Using cmd As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='Version'", conn)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Using versionCmd As New SQLiteCommand("SELECT MAX(VersionNumber) FROM Version", conn)
                        Dim versionResult = versionCmd.ExecuteScalar()
                        If versionResult IsNot DBNull.Value Then
                            version = Convert.ToInt32(versionResult)
                        End If
                    End Using
                Else
                    ' Version table does not exist, so we assume it's version 0
                    version = 0
                End If
            End Using
        End Using
        Return version
    End Function

    Private Shared Sub ApplyDatabaseUpdates(currentVersion As Integer)
        Dim mainConnectionString As String = $"Data Source={_localTasksDatabaseFilePath};Version=3;"
        Dim updateConnectionString As String = $"Data Source={_updateScriptsDatabaseFilePath};Version=3;"

        Using mainConn As New SQLiteConnection(mainConnectionString)
            mainConn.Open()
            ' Read update scripts from the DBUpdates.db
            Using updateConn As New SQLiteConnection(updateConnectionString)
                updateConn.Open()
                Using cmd As New SQLiteCommand("SELECT VersionNumber, Script FROM UpdateScripts WHERE VersionNumber > @CurrentVersion ORDER BY VersionNumber ASC", updateConn)
                    cmd.Parameters.AddWithValue("@CurrentVersion", currentVersion)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim versionNumber = Convert.ToInt32(reader("VersionNumber"))
                            Dim script = reader("Script").ToString()

                            Try
                                ' Execute the script on the main database
                                Using scriptCmd As New SQLiteCommand(script, mainConn)
                                    scriptCmd.ExecuteNonQuery()
                                End Using

                                ' Update the version table in the main database
                                UpdateDatabaseVersion(mainConn, versionNumber)
                            Catch ex As Exception
                                ' Handle the error appropriately (e.g., log it)
                                Throw New Exception($"Error applying update script for version {versionNumber}: {ex.Message}", ex)
                            End Try
                        End While
                    End Using
                End Using
            End Using
        End Using
    End Sub

    Private Shared Sub UpdateDatabaseVersion(conn As SQLiteConnection, versionNumber As Integer)
        Using cmd As New SQLiteCommand("INSERT INTO Version (VersionNumber, AppliedOn) VALUES (@VersionNumber, @AppliedOn)", conn)
            cmd.Parameters.AddWithValue("@VersionNumber", versionNumber)
            cmd.Parameters.AddWithValue("@AppliedOn", DateTime.Now)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

End Class
