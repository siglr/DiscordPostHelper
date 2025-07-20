Imports System.IO
Imports System.IO.Pipes
Imports System.Text
Imports System.Threading
Imports System.Web.Script.Serialization  ' requires reference to System.Web.Extensions

Public Class PipeCommandServer
    Private Const PIPE_NAME As String = "DPHXPipe"
    Private _thread As Thread
    Private _running As Boolean

    Public Event CommandReceived As EventHandler(Of CommandEventArgs)

    Public Sub Start()
        If _running Then Return
        _running = True
        _thread = New Thread(AddressOf ListenLoop) With {.IsBackground = True}
        _thread.Start()
    End Sub

    Public Sub [Stop]()
        _running = False
        Try
            ' connect once to unblock WaitForConnection
            Using temp = New NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out)
                temp.Connect(100)
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ListenLoop()
        While _running
            Using server = New NamedPipeServerStream(PIPE_NAME, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous)
                Try
                    server.WaitForConnection()
                    If Not _running Then Exit While

                    Using reader As New StreamReader(server, Encoding.UTF8)
                        Dim payload = reader.ReadToEnd()
                        If Not String.IsNullOrWhiteSpace(payload) Then
                            Dim cmd = ParseCommand(payload)
                            RaiseEvent CommandReceived(Me, New CommandEventArgs(cmd.Action, cmd.TaskID, cmd.Title, cmd.Source))
                        End If
                    End Using
                Catch ex As IOException
                    ' pipe closed early
                End Try
            End Using
        End While
    End Sub

    Private Function ParseCommand(json As String) _
      As (Action As String, TaskID As String, Title As String, Source As String)

        Try
            Dim js = New JavaScriptSerializer()
            Dim dict = js.Deserialize(Of Dictionary(Of String, String))(json)

            Dim act = If(dict.ContainsKey("action"), dict("action"), "")
            Dim id = If(dict.ContainsKey("taskID"), dict("taskID"), "")
            Dim title = If(dict.ContainsKey("title"), dict("title"), "")
            Dim source = If(dict.ContainsKey("source"), dict("source"), "")

            Return (act, id, title, source)
        Catch
            Return ("", "", "", "")
        End Try
    End Function

End Class

Public Class CommandEventArgs
    Inherits EventArgs

    Public ReadOnly Property Action As String
    Public ReadOnly Property TaskID As String
    Public ReadOnly Property Title As String
    Public ReadOnly Property Source As String

    Public Sub New(action As String, taskID As String, title As String, source As String)
        Me.Action = action
        Me.TaskID = taskID
        Me.Title = title
        Me.Source = source
    End Sub
End Class