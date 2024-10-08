Public Class NewsEntry
    Public Property Key As String
    Public Property Published As DateTime
    Public Property Title As String
    Public Property Subtitle As String
    Public Property Comments As String
    Public Property Credits As String
    Public Property EventDate As DateTime?
    Public Property News As String
    Public Property NewsType As Integer?
    Public Property TaskID As String
    Public Property EntrySeqID As Integer?
    Public Property URLToGo As String
    Public Property UserHasAnswered As Boolean

    Public ReadOnly Property IsWithin2HoursOfEvent As Boolean
        Get
            Dim currentTimeUtc As DateTime = DateTime.UtcNow

            ' Calculate the time difference
            Dim timeDifference As TimeSpan = EventDate - currentTimeUtc

            ' Check if the difference is within 2 hours and up to 30 minutes into the event (for those who are late)
            If Math.Abs(timeDifference.TotalHours) <= 2 AndAlso (timeDifference.TotalMinutes) >= -30 Then
                Return True
            Else
                Return False
            End If

        End Get
    End Property

End Class
