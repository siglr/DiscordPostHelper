Public Class TBTaskData

    Private Shared ReadOnly Property PrefUnits As New PreferredUnits
    Public Property EntrySeqID As Integer

    Public Property TaskID As String

    Public Property DPHXFilename As String

    Public Property IsUpdate As Boolean

    Public Property Title As String

    Public Property LastUpdate As DateTime

    Public Property SimDateTime As DateTime

    Public Property IncludeYear As Boolean

    Public Property SimDateTimeExtraInfo As String

    Public Property MainAreaPOI As String

    Public Property DepartureName As String

    Public Property DepartureICAO As String

    Public Property DepartureExtra As String

    Public Property ArrivalName As String

    Public Property ArrivalICAO As String

    Public Property ArrivalExtra As String

    Public Property SoaringRidge As Boolean

    Public Property SoaringThermals As Boolean

    Public Property SoaringWaves As Boolean

    Public Property SoaringDynamic As Boolean

    Public Property SoaringExtraInfo As String

    Public Property DurationMin As String

    Public Property DurationMax As String

    Public ReadOnly Property DurationConcat As String
        Get
            Return SupportingFeatures.GetDuration(DurationMin, DurationMax)
        End Get
    End Property

    Public Property DurationExtraInfo As String

    Public Property TaskDistance As String

    Public Property TotalDistance As String

    Public Property MapImage As Byte()

    Public Property CoverImage As Byte()

    Public ReadOnly Property DistancesConcat As String
        Get
            Return SupportingFeatures.GetDistance(TotalDistance, TaskDistance, PrefUnits)
        End Get
    End Property

    Public Property RecommendedGliders As String

    Public Property DifficultyRating As String

    Public Property DifficultyExtraInfo As String

    Public ReadOnly Property DifficultyConcat As String
        Get
            Dim diffIndex As Integer = CInt(DifficultyRating.Substring(0, 1))
            Return SupportingFeatures.GetDifficulty(diffIndex, DifficultyExtraInfo)
        End Get
    End Property

    Public Property ShortDescription As String

    Public Property LongDescription As String

    Public Property WeatherSummary As String

    Public Property Credits As String

    Public Property Countries As String

    Public Property RecommendedAddOns As Boolean

    Public Property TotDownloads As Integer

    Public Property LastDownloadUpdate As String


    Public Sub New()

    End Sub

End Class


