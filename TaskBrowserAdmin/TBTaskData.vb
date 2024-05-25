Imports SIGLR.SoaringTools.CommonLibrary
Imports Newtonsoft.Json

Public Class TBTaskData

    <JsonIgnore>
    Private Shared ReadOnly Property PrefUnits As New PreferredUnits

    Public Property EntrySeqID As Integer

    Public Property TaskID As String

    <JsonIgnore>
    Public Property DPHXFilename As String

    <JsonIgnore>
    Public Property IsUpdate As Boolean

    Public Property Title As String

    Public Property LastUpdate As String

    Public Property SimDateTime As String

    Public Property IncludeYear As Integer

    Public Property SimDateTimeExtraInfo As String

    Public Property MainAreaPOI As String

    Public Property DepartureName As String

    Public Property DepartureICAO As String

    Public Property DepartureExtra As String

    Public Property ArrivalName As String

    Public Property ArrivalICAO As String

    Public Property ArrivalExtra As String

    Public Property SoaringRidge As Integer

    Public Property SoaringThermals As Integer

    Public Property SoaringWaves As Integer

    Public Property SoaringDynamic As Integer

    Public Property SoaringExtraInfo As String

    Public Property DurationMin As Integer

    Public Property DurationMax As Integer

    <JsonIgnore>
    Public ReadOnly Property DurationConcat As String
        Get
            Return SupportingFeatures.GetDuration(DurationMin.ToString, DurationMax.ToString)
        End Get
    End Property

    Public Property DurationExtraInfo As String

    Public Property TaskDistance As Integer

    Public Property TotalDistance As Integer

    Public Property MapImage As Byte()

    Public Property CoverImage As Byte()

    <JsonIgnore>
    Public ReadOnly Property DistancesConcat As String
        Get
            Return SupportingFeatures.GetDistance(TotalDistance.ToString, TaskDistance.ToString, PrefUnits)
        End Get
    End Property

    Public Property RecommendedGliders As String

    Public Property DifficultyRating As String

    Public Property DifficultyExtraInfo As String

    <JsonIgnore>
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

    Public Property RecommendedAddOns As Integer

    Public Property TotDownloads As Integer

    Public Property LastDownloadUpdate As String

    Public Property DBEntryUpdate As String


    Public Sub New()

    End Sub

End Class


