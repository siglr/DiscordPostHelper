Imports System.ComponentModel
Imports System.Xml.Serialization

<XmlRoot("AllData")>
Public Class AllData

    <XmlElement("FlightPlanFilename")>
    Public Property FlightPlanFilename As String

    <XmlElement("WeatherFilename")>
    Public Property WeatherFilename As String

    <XmlElement("LockTitle")>
    Public Property LockTitle As Boolean

    <XmlElement("Title")>
    Public Property Title As String

    <XmlElement("SimDate")>
    Public Property SimDate As DateTime

    <XmlElement("IncludeYear")>
    Public Property IncludeYear As Boolean

    <XmlElement("SimTime")>
    Public Property SimTime As DateTime

    <XmlElement("SimDateTimeExtraInfo")>
    Public Property SimDateTimeExtraInfo As String

    <XmlElement("MainAreaPOI")>
    Public Property MainAreaPOI As String

    <XmlElement("DepartureICAO")>
    Public Property DepartureICAO As String

    <XmlElement("DepartureName")>
    Public Property DepartureName As String

    <XmlElement("DepartureLock")>
    Public Property DepartureLock As Boolean

    <XmlElement("DepartureExtra")>
    Public Property DepartureExtra As String

    <XmlElement("ArrivalICAO")>
    Public Property ArrivalICAO As String

    <XmlElement("ArrivalName")>
    Public Property ArrivalName As String

    <XmlElement("ArrivalLock")>
    Public Property ArrivalLock As Boolean

    <XmlElement("ArrivalExtra")>
    Public Property ArrivalExtra As String

    <XmlElement("SoaringRidge")>
    Public Property SoaringRidge As Boolean

    <XmlElement("SoaringThermals")>
    Public Property SoaringThermals As Boolean

    <XmlElement("SoaringWaves")>
    Public Property SoaringWaves As Boolean
    <XmlElement("SoaringExtraInfo")>
    Public Property SoaringExtraInfo As String

    <XmlElement("AvgSpeedsUnit")>
    Public Property AvgSpeedsUnit As Integer

    <XmlElement("AvgMaxSpeed")>
    Public Property AvgMaxSpeed As String

    <XmlElement("AvgMinSpeed")>
    Public Property AvgMinSpeed As String

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

    <XmlElement("LockShortDescription")>
    Public Property LockShortDescription As Boolean

    <XmlElement("ShortDescription")>
    Public Property ShortDescription As String

    <XmlElement("Credits")>
    Public Property Credits As String

    <XmlElement("LongDescription")>
    Public Property LongDescription As String

    <XmlElement("CountryFlag")>
    Public Property CountryFlag As String

    <XmlElement("WeatherSummaryOnly")>
    Public Property WeatherSummaryOnly As Boolean

    <XmlElement("WeatherSummary")>
    Public Property WeatherSummary As String

    <XmlElement("RecommendedAddOns")>
    Public Property RecommendedAddOns As New List(Of RecommendedAddOn)

    <XmlElement("ExtraFiles")>
    Public Property ExtraFiles As List(Of String) = New List(Of String)

    <XmlElement("LockCountries")>
    Public Property LockCountries As Boolean

    <XmlElement("Countries")>
    Public Property Countries As List(Of String) = New List(Of String)

    <XmlElement("IncludeDPHXPackage")>
    Public Property IncludeDPHXPackage As Boolean

    <XmlElement("DPHXPackageFilename")>
    Public Property DPHXPackageFilename As String

    <XmlElement("EventEnabled")>
    Public Property EventEnabled As Boolean

    <XmlElement("GroupClubId")>
    Public Property GroupClubId As String

    <XmlElement("GroupClubName")>
    Public Property GroupClubName As String

    <XmlElement("EventTopic")>
    Public Property EventTopic As String

    <XmlElement("MSFSServer")>
    Public Property MSFSServer As Integer

    <XmlElement("VoiceChannel")>
    Public Property VoiceChannel As String

    <XmlElement("UTCSelected")>
    Public Property UTCSelected As Boolean

    <XmlElement("EventMeetDate")>
    Public Property EventMeetDate As DateTime

    <XmlElement("EventMeetTime")>
    Public Property EventMeetTime As DateTime

    <XmlElement("UseEventSyncFly")>
    Public Property UseEventSyncFly As Boolean

    <XmlElement("EventSyncFlyDate")>
    Public Property EventSyncFlyDate As DateTime

    <XmlElement("EventSyncFlyTime")>
    Public Property EventSyncFlyTime As DateTime

    <XmlElement("UseEventLaunch")>
    Public Property UseEventLaunch As Boolean

    <XmlElement("EventLaunchDate")>
    Public Property EventLaunchDate As DateTime

    <XmlElement("EventLaunchTime")>
    Public Property EventLaunchTime As DateTime

    <XmlElement("UseEventStartTask")>
    Public Property UseEventStartTask As Boolean

    <XmlElement("EventStartTaskDate")>
    Public Property EventStartTaskDate As DateTime

    <XmlElement("EventStartTaskTime")>
    Public Property EventStartTaskTime As DateTime

    <XmlElement("EventDescription")>
    Public Property EventDescription As String

    <XmlElement("EligibleAward")>
    Public Property EligibleAward As Integer

    <XmlElement("URLFlightPlanPost")>
    Public Property URLFlightPlanPost As String

    <XmlElement("URLGroupEventPost")>
    Public Property URLGroupEventPost As String

    <XmlElement("URLDiscordEventInvite")>
    Public Property URLDiscordEventInvite As String

    <XmlElement("IncludeGGServerInvite")>
    Public Property IncludeGGServerInvite As Boolean

    <XmlElement("MapImageSelected")>
    Public Property MapImageSelected As String

    <XmlElement("MapImageHorizontalScrollValue")>
    Public Property MapImageHorizontalScrollValue As String

    <XmlElement("MapImageVerticalScrollValue")>
    Public Property MapImageVerticalScrollValue As String

    <XmlElement("MapImageWidth")>
    Public Property MapImageWidth As Integer

    <XmlElement("MapImageHeight")>
    Public Property MapImageHeight As Integer

    <XmlElement("BeginnersGuide")>
    Public Property BeginnersGuide As String

    <XmlElement("BeginnersGuideCustom")>
    Public Property BeginnersGuideCustom As String

    <XmlElement("DiscordTaskThreadURL")>
    Public Property DiscordTaskThreadURL As String

    <XmlElement("DiscordEventThreadURL")>
    Public Property DiscordEventThreadURL As String

    Public Sub New()

    End Sub

    Public ReadOnly Property SimLocalDateTime As Date
        Get
            Return SupportingFeatures.GetFullEventDateTimeInLocal(SimDate, SimTime, False)
        End Get
    End Property

    Public ReadOnly Property MeetLocalDateTime As Date
        Get
            Return SupportingFeatures.GetFullEventDateTimeInLocal(EventMeetDate, EventMeetTime, True)
        End Get
    End Property

    Public ReadOnly Property SyncFlyLocalDateTime As Date
        Get
            Return SupportingFeatures.GetFullEventDateTimeInLocal(EventSyncFlyDate, EventSyncFlyTime, True)
        End Get
    End Property

    Public ReadOnly Property LaunchLocalDateTime As Date
        Get
            Return SupportingFeatures.GetFullEventDateTimeInLocal(EventLaunchDate, EventLaunchTime, True)
        End Get
    End Property

    Public ReadOnly Property StartTaskLocalDateTime As Date
        Get
            Return SupportingFeatures.GetFullEventDateTimeInLocal(EventStartTaskDate, EventStartTaskTime, True)
        End Get
    End Property

End Class
