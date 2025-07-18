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
        If _isRunning Then
            Exit Sub  ' Already running
        End If

        _listener = New HttpListener()
        _listener.Prefixes.Add(String.Format("http://localhost:{0}/", _port))
        _listener.Start()
        _isRunning = True

        ' Asynchronously wait for requests
        _listener.BeginGetContext(AddressOf HandleRequest, Nothing)
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
        ' If we've stopped or the listener is disposed, return
        If (Not _isRunning) OrElse (_listener Is Nothing) Then
            Return
        End If

        Dim context As HttpListenerContext = Nothing

        Try
            ' Complete the context retrieval
            context = _listener.EndGetContext(ar)

            ' Immediately begin waiting for the next request again
            If _isRunning Then
                _listener.BeginGetContext(AddressOf HandleRequest, Nothing)
            End If

            ' Raise the event so subscribers can handle the request
            RaiseEvent RequestReceived(Me, New RequestReceivedEventArgs(context))

        Catch ex As Exception
            ' If the listener is closed or there's an error, it may throw here.
            ' You could log the exception if needed.
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

        ' If you need CORS, uncomment or customize:
        response.Headers.Add("Access-Control-Allow-Origin", "*")
        ' response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,OPTIONS")

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

    Public Sub New(context As HttpListenerContext)
        Me.Context = context
    End Sub
End Class
