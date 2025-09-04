Imports System.IO
Imports System.IO.Pipes
Imports System.Text
Imports System.Threading
Imports Newtonsoft.Json.Linq   ' add NuGet: Newtonsoft.Json

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
            Using temp = New NamedPipeClientStream(".", PIPE_NAME, PipeDirection.Out)
                temp.Connect(100) ' unblock WaitForConnection
            End Using
        Catch
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
                            Dim act As String = ""
                            Dim data As JObject = Nothing
                            If TryParse(payload, act, data) Then
                                RaiseEvent CommandReceived(Me, New CommandEventArgs(act, data))
                            End If
                        End If
                    End Using
                Catch ex As IOException
                    ' pipe closed early
                End Try
            End Using
        End While
    End Sub

    Private Function TryParse(json As String, ByRef action As String, ByRef data As JObject) As Boolean
        Try
            data = JObject.Parse(json)
            action = If(data.Value(Of String)("action"), String.Empty)
            Return Not String.IsNullOrEmpty(action)
        Catch
            action = ""
            data = Nothing
            Return False
        End Try
    End Function

End Class

Public Class CommandEventArgs
    Inherits EventArgs

    Public ReadOnly Property Action As String
    Public ReadOnly Property Data As JObject

    ' --- Convenience (keeps current callers working for download-task) ---
    Public ReadOnly Property TaskID As String
        Get
            Return Data?.Value(Of String)("taskID")
        End Get
    End Property

    Public ReadOnly Property Title As String
        Get
            Return Data?.Value(Of String)("title")
        End Get
    End Property

    Public ReadOnly Property Source As String
        Get
            Return Data?.Value(Of String)("source")
        End Get
    End Property

    ' Optional convenience for user-info payloads:
    Public ReadOnly Property User As JObject
        Get
            Return TryCast(Data?("user"), JObject)
        End Get
    End Property

    Public Sub New(action As String, data As JObject)
        Me.Action = action
        Me.Data = data
    End Sub
End Class
