Public Class IGCCacheTaskObject
    Public Property EntrySeqID As Integer
    Public Property PLNXML As String
    Public Property MSFSLocalDateTime As DateTime

    Public Sub New(entrySeqID As Integer, plnXML As String, msfsLocalDateTime As DateTime)
        Me.EntrySeqID = entrySeqID
        Me.PLNXML = plnXML
        Me.MSFSLocalDateTime = msfsLocalDateTime
    End Sub

End Class
