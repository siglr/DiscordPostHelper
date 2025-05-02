Public Class IGCLookupDetails
    ' — from your matching step —
    Public Property EntrySeqID As Integer
    Public Property AlreadyUploaded As Boolean
    Public Property TaskTitle As String

    ' — constructed key/filename —
    Public ReadOnly Property IGCKeyFilename As String
        Get
            Return $"{EntrySeqID}_{CompetitionID}_{GliderType}_{IGCRecordDateTimeUTC}"
        End Get
    End Property

    ' — parsed from the IGC headers —
    Public Property CompetitionID As String
    Public Property CompetitionClass As String
    Public Property Pilot As String
    Public Property GliderType As String
    Public Property GliderID As String
    Public Property NB21Version As String
    Public Property Sim As String

    ' — timing fields —
    Public Property IGCRecordDateTimeUTC As String
    Public Property IGCUploadDateTimeUTC As String
    Public Property LocalDate As String
    Public Property LocalTime As String
    Public Property BeginTimeUTC As String

    ' — waypoints payload —
    Public Property IGCWaypoints As List(Of IGCWaypoint)

    ' — where the local .igc lives —
    Public Property IGCLocalFilePath As String
End Class

Public Class IGCWaypoint
    Public Property Id As String
    Public Property Coord As String
End Class
