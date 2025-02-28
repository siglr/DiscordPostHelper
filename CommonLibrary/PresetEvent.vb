Public Class PresetEvent

    Public ReadOnly Property ClubId As String
    Public ReadOnly Property ClubName As String
    Public ReadOnly Property ClubFullName As String
    Public ReadOnly Property TrackerGroup As String
    Public ReadOnly Property EventNewsID As String
    Public ReadOnly Property MSFSServer As String
    Public ReadOnly Property VoiceChannel As String
    Public ReadOnly Property TimeZoneID As String
    Public ReadOnly Property EventDayOfWeek As DayOfWeek
    Public ReadOnly Property ZuluTime As DateTime
    Public ReadOnly Property SummerEventDayOfWeek As DayOfWeek
    Public ReadOnly Property SummerZuluTime As DateTime
    Public ReadOnly Property SyncFlyDelay As Integer
    Public ReadOnly Property LaunchDelay As Integer
    Public ReadOnly Property StartTaskDelay As Integer
    Public ReadOnly Property EligibleAward As Boolean
    Public ReadOnly Property BeginnerLink As String

    Public ReadOnly Property ForceSyncFly As Boolean
    Public ReadOnly Property ForceLaunch As Boolean
    Public ReadOnly Property ForceStartTask As Boolean
    Public ReadOnly Property DiscordURL As String
    Public ReadOnly Property SharedPublishers As List(Of String)
    Public ReadOnly Property EmojiID As String

    Private _emoji As String
    Public ReadOnly Property Emoji As String
        Get
            Return $":{_emoji}:"
        End Get
    End Property

    Public Sub New(pClubId As String,
                   pClubName As String,
                   pClubFullName As String,
                   pTrackerGroup As String,
                   pEmoji As String,
                   pEmojiID As String,
                   pEventNewsID As String,
                   pMSFSServer As String,
                   pVoiceChannel As String,
                   pTimeZoneID As String,
                   pZuluDayOfWeek As DayOfWeek,
                   pZuluTime As DateTime,
                   pSummerZuluDayOfWeek As DayOfWeek,
                   pSummerZuluTime As DateTime,
                   pSyncFlyDelay As Integer,
                   pLaunchTDelay As Integer,
                   pStartTaskDelay As Integer,
                   pEligibleAward As Boolean,
                   pBeginnerLink As String,
                   pForceSyncFly As Boolean,
                   pForceLaunch As Boolean,
                   pForceStartTask As Boolean,
                   pDiscordURL As String,
                   pSharedPublishers As List(Of String))

        ClubId = pClubId
        ClubName = pClubName
        ClubFullName = pClubFullName
        TrackerGroup = pTrackerGroup
        EventNewsID = pEventNewsID
        _emoji = pEmoji
        EmojiID = pEmojiID
        MSFSServer = pMSFSServer
        VoiceChannel = pVoiceChannel
        TimeZoneID = pTimeZoneID
        EventDayOfWeek = pZuluDayOfWeek
        ZuluTime = pZuluTime
        SummerEventDayOfWeek = pSummerZuluDayOfWeek
        SummerZuluTime = pSummerZuluTime
        SyncFlyDelay = pSyncFlyDelay
        LaunchDelay = pLaunchTDelay
        StartTaskDelay = pStartTaskDelay
        EligibleAward = pEligibleAward
        BeginnerLink = pBeginnerLink
        ForceSyncFly = pForceSyncFly
        ForceLaunch = pForceLaunch
        ForceStartTask = pForceStartTask
        DiscordURL = $"https://discord.com/channels/{pDiscordURL}"
        SharedPublishers = pSharedPublishers

    End Sub

    Public ReadOnly Property IsCustom As Boolean
        Get
            Return ClubId = "CUSTOM"
        End Get
    End Property

    Public Function GetZuluTimeForDate(selectedDate As Date, forceStandard As Boolean) As DateTime

        If forceStandard Then
            Return ZuluTime
        End If

        Dim clubTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID)
        Dim isDST As Boolean = clubTimeZone.IsDaylightSavingTime(selectedDate)

        If isDST Then
            Return SummerZuluTime
        Else
            Return ZuluTime
        End If

    End Function

    Public Function GetZuluTimeEventDayOfWeekForDate(selectedDate As Date) As DayOfWeek

        Dim clubTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID)
        Dim isDST As Boolean = clubTimeZone.IsDaylightSavingTime(selectedDate)

        If isDST Then
            Return SummerEventDayOfWeek
        Else
            Return EventDayOfWeek
        End If

    End Function

    Public Function IsSummerTime(selectedDateTime As DateTime) As Boolean
        Dim clubTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID)
        Return clubTimeZone.IsDaylightSavingTime(selectedDateTime)
    End Function

End Class
