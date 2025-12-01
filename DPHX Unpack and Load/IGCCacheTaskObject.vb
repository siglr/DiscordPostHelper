Public Class IGCCacheTaskObject
    Public Property EntrySeqID As Integer
    Public Property PLNXML As String
    Public Property WPRXML As String
    Public Property MSFSLocalDateTime As DateTime
    Public Property TaskTitle As String
    Public Property WeatherPresetName As String
    Public Property LastUpdate As DateTime

    Public Sub New(entrySeqID As Integer, plnXML As String, msfsLocalDateTime As DateTime, taskTitle As String, lastUpdate As DateTime)
        Me.EntrySeqID = entrySeqID
        Me.PLNXML = plnXML
        Me.MSFSLocalDateTime = msfsLocalDateTime
        Me.TaskTitle = taskTitle
        Me.LastUpdate = lastUpdate
    End Sub

    Public Sub New(entrySeqID As Integer, plnXML As String, wprXML As String, msfsLocalDateTime As DateTime, taskTitle As String, weatherPresetName As String, lastUpdate As DateTime)
        Me.EntrySeqID = entrySeqID
        Me.PLNXML = plnXML
        Me.WPRXML = wprXML
        Me.MSFSLocalDateTime = msfsLocalDateTime
        Me.TaskTitle = taskTitle
        Me.WeatherPresetName = weatherPresetName
        Me.LastUpdate = lastUpdate
    End Sub
End Class
