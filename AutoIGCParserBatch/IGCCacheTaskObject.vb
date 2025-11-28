Public Class IGCCacheTaskObject
    Public Property EntrySeqID As Integer
    Public Property PLNXML As String
    Public Property MSFSLocalDateTime As DateTime
    Public Property TaskTitle As String
    Public Property WPRXML As String
    Public Property WeatherPresetName As String

    Public Sub New(entrySeqID As Integer, plnXML As String, msfsLocalDateTime As DateTime, taskTitle As String,
                   Optional wprXml As String = "", Optional weatherPresetName As String = "")
        Me.EntrySeqID = entrySeqID
        Me.PLNXML = plnXML
        Me.MSFSLocalDateTime = msfsLocalDateTime
        Me.TaskTitle = taskTitle
        Me.WPRXML = wprXml
        Me.WeatherPresetName = weatherPresetName
    End Sub

End Class
