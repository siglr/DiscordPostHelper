Public Class IGCCacheTaskObject
    Public Property EntrySeqID As Integer
    Public Property PLNXML As String
    Public Property MSFSLocalDateTime As DateTime
    Public Property TaskTitle As String

    Public Sub New(entrySeqID As Integer, plnXML As String, msfsLocalDateTime As DateTime, taskTitle As String)
        Me.EntrySeqID = entrySeqID
        Me.PLNXML = plnXML
        Me.MSFSLocalDateTime = msfsLocalDateTime
        Me.TaskTitle = taskTitle
    End Sub

End Class
