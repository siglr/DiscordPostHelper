Imports System.Net
Imports System.Text

''' <summary>
''' Encapsulates an HttpListener that listens on localhost at a configurable port.
''' Raises an event when a request is received.
''' </summary>
Public Class DPHXLocalWS
    Implements IDisposable

    Private _listener As HttpListener
    Private _isRunning As Boolean
    Private _port As Integer

    ''' <summary>
    ''' Event fired whenever a request is received.
    ''' Passes an instance of RequestReceivedEventArgs with the HttpListenerContext.
    ''' </summary>
    Public Event RequestReceived As EventHandler(Of RequestReceivedEventArgs)

    ''' <summary>
    ''' Gets or sets the port for the local HTTP server.
    ''' You can only change this when the server is not running.
    ''' To change the port at runtime, call Stop() first, set Port, then call Start() again.
    ''' </summary>
    Public Property Port As Integer
        Get
            Return _port
        End Get
        Set(value As Integer)
            If _isRunning Then
                Throw New InvalidOperationException("Cannot change port while the server is running. " &
                                                    "Call Stop() first, then set Port, then Start().")
            End If
            _port = value
        End Set
    End Property

    ''' <summary>
    ''' Indicates whether the server is currently running.
    ''' </summary>
    Public ReadOnly Property IsRunning As Boolean
        Get
            Return _isRunning
        End Get
    End Property

    ''' <summary>
    ''' Creates a new instance of LocalHttpServer, optionally specifying the listening port.
    ''' </summary>
    ''' <param name="port">The TCP port to listen on. Defaults to 54513.</param>
    Public Sub New(Optional port As Integer = 54513)
        _port = port
    End Sub

    ''' <summary>
    ''' Starts the HTTP listener on the specified port (localhost only).
    ''' </summary>
    Public Sub Start()
        If _isRunning Then Exit Sub

        _listener = New HttpListener()
        _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous
        _listener.IgnoreWriteExceptions = True

        ' CHANGED: listen on all loopback variants so any browser target works
        _listener.Prefixes.Add($"http://localhost:{_port}/")
        Try : _listener.Prefixes.Add($"http://127.0.0.1:{_port}/") : Catch : End Try
        Try : _listener.Prefixes.Add($"http://[::1]:{_port}/") : Catch : End Try

        _listener.Start()
        _isRunning = True

        StartAccept() ' CHANGED: use helper
    End Sub

    Private Sub StartAccept()
        If _isRunning AndAlso _listener IsNot Nothing Then
            _listener.BeginGetContext(AddressOf HandleRequest, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Stops the HTTP listener if running.
    ''' </summary>
    Public Sub [Stop]()
        If Not _isRunning Then
            Exit Sub
        End If

        _isRunning = False
        If _listener IsNot Nothing Then
            _listener.Close()
            _listener = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Internal callback for handling inbound HTTP requests asynchronously.
    ''' </summary>
    Private Sub HandleRequest(ar As IAsyncResult)
        If Not _isRunning OrElse _listener Is Nothing Then Return

        Dim context As HttpListenerContext = Nothing
        Try
            context = _listener.EndGetContext(ar)
        Catch ex As HttpListenerException
            ' transient (unsupported/aborted). Re-arm and bail.
            StartAccept()
            Exit Sub
        Catch ex As ObjectDisposedException
            Exit Sub
        End Try

        ' Re-arm immediately so we don't miss the next request
        StartAccept()

        ' Preflight / quick no-content reply
        If String.Equals(context.Request.HttpMethod, "OPTIONS", StringComparison.OrdinalIgnoreCase) Then
            SendNoContent(context)
            Exit Sub
        End If

        ' Hand off to subscribers (they should reply quickly)
        Dim args = New RequestReceivedEventArgs(context)
        RaiseEvent RequestReceived(Me, args)

        If Not args.Handled Then
            SendNoContent(context)
        End If

    End Sub

    Private Sub SendNoContent(context As HttpListenerContext)
        If context Is Nothing Then Exit Sub
        Dim r = context.Response
        Try
            ' Set only what is safe; avoid ContentLength64 entirely.
            r.StatusCode = 204
            r.Headers("Access-Control-Allow-Origin") = "*"
            r.Headers("Access-Control-Allow-Methods") = "GET, OPTIONS"
            r.Headers("Access-Control-Allow-Headers") = "Content-Type"
            r.Headers("Access-Control-Allow-Private-Network") = "true"
            r.KeepAlive = False
            r.Headers("Connection") = "close"
            ' DO NOT set r.ContentLength64 here.
        Catch
            ' ignore – best-effort headers
        End Try
        Try
            r.OutputStream.Close()   ' clean, will commit if not already committed
        Catch
            Try : r.Abort() : Catch : End Try   ' last resort; guarantees the socket closes
        End Try
    End Sub

    ''' <summary>
    ''' Helper method for sending a string response back to the browser/client.
    ''' </summary>
    ''' <param name="context">The HttpListenerContext to respond to.</param>
    ''' <param name="message">The response string.</param>
    ''' <param name="statusCode">Optional HTTP status code (default 200).</param>
    Public Sub SendResponse(context As HttpListenerContext,
                            message As String,
                            Optional statusCode As Integer = 200)

        Dim response = context.Response
        response.StatusCode = statusCode
        response.Headers("Access-Control-Allow-Origin") = "*"
        response.Headers("Access-Control-Allow-Methods") = "GET, OPTIONS"
        response.Headers("Access-Control-Allow-Headers") = "Content-Type"
        response.Headers("Access-Control-Allow-Private-Network") = "true"
        response.Headers("Connection") = "close"

        Dim buffer As Byte() = Encoding.UTF8.GetBytes(message)
        response.ContentLength64 = buffer.Length

        Using output = response.OutputStream
            output.Write(buffer, 0, buffer.Length)
        End Using
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Dispose managed state (managed objects).
                [Stop]() ' Ensure the listener is properly closed.
            End If
            ' Free unmanaged resources (unmanaged objects) here, if needed.
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

''' <summary>
''' Custom event arguments, carrying the HttpListenerContext.
''' </summary>
Public Class RequestReceivedEventArgs
    Inherits EventArgs
    Public ReadOnly Property Context As HttpListenerContext
    Public Property Handled As Boolean
    Public Sub New(context As HttpListenerContext)
        Me.Context = context
    End Sub
End Class
