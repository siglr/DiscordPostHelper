Public Class DuplicateCheckResult
    Public Property GeometryEqual As Boolean         ' same # and positions (within epsilon)
    Public Property ExactMatch As Boolean            ' geometry equal AND no soft diffs
    Public Property HardDifference As String         ' reason if geometry NOT equal
    Public Property Differences As New List(Of String) ' soft diffs list (for confirmation)
End Class
