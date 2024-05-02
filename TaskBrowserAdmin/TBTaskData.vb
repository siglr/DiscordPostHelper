Imports System.Xml.Serialization

<XmlRoot("TBTaskData")>
Public Class TBTaskData
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

    <XmlElement("DepartureICAO")>
    Public Property DepartureICAO As String

    <XmlElement("DepartureName")>
    Public Property DepartureName As String

    <XmlElement("DepartureExtra")>
    Public Property DepartureExtra As String

    <XmlElement("ArrivalICAO")>
    Public Property ArrivalICAO As String

    <XmlElement("ArrivalName")>
    Public Property ArrivalName As String

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

    <XmlElement("DurationExtraInfo")>
    Public Property DurationExtraInfo As String

    <XmlElement("RecommendedGliders")>
    Public Property RecommendedGliders As String

    <XmlElement("DifficultyRating")>
    Public Property DifficultyRating As String

    <XmlElement("DifficultyExtraInfo")>
    Public Property DifficultyExtraInfo As String

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

    Public Sub New()

    End Sub


End Class
