Imports System.Collections.Generic
Imports CefSharp
Imports CefSharp.Handler

''' <summary>
''' Feeds our chosen IGC file into the JS <input type="file">
''' whenever the planner page does a file‐pick.
''' </summary>
Public Class UploadFileDialogHandler
    Inherits DialogHandler

    ''' <summary>
    ''' Set this to the full path of the IGC you want to “upload”
    ''' just before you fire the JS click().
    ''' </summary>
    Public Property FileToUpload As String

    ''' <summary>
    ''' Matches CefSharp.Example’s TempFileDialogHandler override exactly:
    ''' https://github.com/cefsharp/CefSharp/blob/master/CefSharp.Example/Handlers/TempFileDialogHandler.cs :contentReference[oaicite:0]{index=0}
    ''' </summary>
    Protected Overrides Function OnFileDialog(
            chromiumWebBrowser As IWebBrowser,
            browser As IBrowser,
            mode As CefFileDialogMode,
            title As String,
            defaultFilePath As String,
            acceptFilters As IReadOnlyCollection(Of String),
            acceptExtensions As IReadOnlyCollection(Of String),
            acceptDescriptions As IReadOnlyCollection(Of String),
            callback As IFileDialogCallback
        ) As Boolean

        If Not String.IsNullOrEmpty(FileToUpload) Then
            ' Feed our single IGC path back into the page.
            callback.Continue(New List(Of String) From {FileToUpload})
            Return True
        End If

        ' Otherwise fall back to the default behavior.
        Return MyBase.OnFileDialog(
            chromiumWebBrowser, browser, mode,
            title, defaultFilePath,
            acceptFilters, acceptExtensions, acceptDescriptions,
            callback)
    End Function

End Class
