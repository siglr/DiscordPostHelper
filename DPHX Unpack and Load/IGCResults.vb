Public Class IGCResults

    Public Property TaskCompleted As Boolean
    Public Property Penalties As Boolean
    Public Property Duration As String
    Public Property Distance As String
    Public Property Speed As String
    Public Property IGCValid As Boolean
    Public Property TPVersion As String

    Public Sub New(pTaskCompleted As Boolean,
                   pPenalties As Boolean,
                   pDuration As String,
                   pDistance As String,
                   pSpeed As String,
                   pIGCValid As Boolean,
                   pTPVersion As String
                   )

        TaskCompleted = pTaskCompleted
        Penalties = pPenalties
        Duration = pDuration
        Distance = pDistance
        Speed = pSpeed
        IGCValid = pIGCValid
        TPVersion = pTPVersion

    End Sub

    Public ReadOnly Property DurationInSeconds As Integer
        Get
            If Not String.IsNullOrEmpty(Duration) Then
                Dim p As String() = Duration.Split(":"c)
                If p.Length = 3 Then
                    Return CInt(p(0)) * 3600 + CInt(p(1)) * 60 + CInt(p(2))
                End If
            End If
            Return 0
        End Get
    End Property

End Class
