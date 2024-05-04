Imports System.Web.ModelBinding
Imports System.Web.Profile
Imports System.Xml.Serialization

<XmlRoot("TBTaskData")>
Public Class TBTaskData

    <XmlIgnore()>
    Private Shared ReadOnly Property PrefUnits As New PreferredUnits

    <XmlElement("TaskID")>
    Public Property TaskID As String

    <XmlIgnore()>
    Public Property DPHXFilename As String

    <XmlIgnore()>
    Public Property IsUpdate As Boolean

    <XmlElement("Title")>
    Public Property Title As String

    <XmlElement("LastUpdate")>
    Public Property LastUpdate As DateTime

    <XmlElement("SimDateTime")>
    Public Property SimDateTime As DateTime

    <XmlElement("IncludeYear")>
    Public Property IncludeYear As Boolean

    <XmlElement("SimDateTimeExtraInfo")>
    Public Property SimDateTimeExtraInfo As String

    <XmlElement("MainAreaPOI")>
    Public Property MainAreaPOI As String

    <XmlElement("DepartureName")>
    Public Property DepartureName As String

    <XmlElement("DepartureICAO")>
    Public Property DepartureICAO As String

    <XmlElement("DepartureExtra")>
    Public Property DepartureExtra As String

    <XmlElement("ArrivalName")>
    Public Property ArrivalName As String

    <XmlElement("ArrivalICAO")>
    Public Property ArrivalICAO As String

    <XmlElement("ArrivalExtra")>
    Public Property ArrivalExtra As String

    <XmlElement("SoaringRidge")>
    Public Property SoaringRidge As Boolean

    <XmlElement("SoaringThermals")>
    Public Property SoaringThermals As Boolean

    <XmlElement("SoaringWaves")>
    Public Property SoaringWaves As Boolean

    <XmlElement("SoaringDynamic")>
    Public Property SoaringDynamic As Boolean

    <XmlElement("SoaringExtraInfo")>
    Public Property SoaringExtraInfo As String

    <XmlElement("DurationMin")>
    Public Property DurationMin As String

    <XmlElement("DurationMax")>
    Public Property DurationMax As String

    <XmlIgnore()>
    Public ReadOnly Property DurationConcat As String
        Get
            Return SupportingFeatures.GetDuration(DurationMin, DurationMax)
        End Get
    End Property

    <XmlElement("DurationExtraInfo")>
    Public Property DurationExtraInfo As String

    <XmlElement("TaskDistance")>
    Public Property TaskDistance As String

    <XmlElement("TotalDistance")>
    Public Property TotalDistance As String

    <XmlIgnore()>
    Public ReadOnly Property DistancesConcat As String
        Get
            Return SupportingFeatures.GetDistance(TotalDistance, TaskDistance, PrefUnits)
        End Get
    End Property

    <XmlElement("RecommendedGliders")>
    Public Property RecommendedGliders As String

    <XmlElement("DifficultyRating")>
    Public Property DifficultyRating As String

    <XmlElement("DifficultyExtraInfo")>
    Public Property DifficultyExtraInfo As String

    <XmlIgnore()>
    Public ReadOnly Property DifficultyConcat As String
        Get
            Dim diffIndex As Integer = CInt(DifficultyRating.Substring(0, 1))
            Return SupportingFeatures.GetDifficulty(diffIndex, DifficultyExtraInfo)
        End Get
    End Property

    <XmlElement("ShortDescription")>
    Public Property ShortDescription As String

    <XmlElement("LongDescription")>
    Public Property LongDescription As String

    <XmlElement("WeatherSummary")>
    Public Property WeatherSummary As String

    <XmlElement("Credits")>
    Public Property Credits As String

    <XmlElement("Countries")>
    Public Property Countries As String

    <XmlElement("RecommendedAddOns")>
    Public Property RecommendedAddOns As Boolean

    Public Sub New()

    End Sub


End Class


