Public Class PresetEvent

    Public ReadOnly Property ClubId As String
    Public ReadOnly Property ClubName As String
    Public ReadOnly Property ClubFullName As String
    Public ReadOnly Property MSFSServer As String
    Public ReadOnly Property VoiceChannel As String
    Public ReadOnly Property EventDayOfWeek As DayOfWeek
    Public ReadOnly Property ZuluTime As DateTime
    Public ReadOnly Property SyncFlyDelay As Integer
    Public ReadOnly Property LaunchDelay As Integer
    Public ReadOnly Property StartTaskDelay As Integer
    Public ReadOnly Property EligibleAward As Boolean
    Public ReadOnly Property BeginnerLink As String


    Public Sub New(pClubId As String,
                   pClubName As String,
                   pClubFullName As String,
                   pMSFSServer As String,
                   pVoiceChannel As String,
                   pZuluDayOfWeek As DayOfWeek,
                   pZuluTime As DateTime,
                   pSyncFlyDelay As Integer,
                   pLaunchTDelay As Integer,
                   pStartTaskDelay As Integer,
                   pEligibleAward As Boolean,
                   pBeginnerLink As String)

        ClubId = pClubId
        ClubName = pClubName
        ClubFullName = pClubFullName
        MSFSServer = pMSFSServer
        VoiceChannel = pVoiceChannel
        EventDayOfWeek = pZuluDayOfWeek
        ZuluTime = pZuluTime
        SyncFlyDelay = pSyncFlyDelay
        LaunchDelay = pLaunchTDelay
        StartTaskDelay = pStartTaskDelay
        EligibleAward = pEligibleAward
        BeginnerLink = pBeginnerLink

    End Sub

    Public ReadOnly Property IsCustom As Boolean
        Get
            Return ClubId = "CUSTOM"
        End Get
    End Property

    Public Function GetZuluTimeForDate(selectedDate As Date) As DateTime

        'Going back to always returning ZuluTime without checking daylight savings
        Return ZuluTime

        'Dim localTimeZone As TimeZoneInfo = TimeZoneInfo.Local
        '' Check if the local time zone is currently observing daylight saving time
        'Dim isDst As Boolean = localTimeZone.IsDaylightSavingTime(selectedDate)
        'If isDst Then
        '    Return ZuluTime.AddHours(-1)
        'Else
        '    Return ZuluTime
        'End If

    End Function

End Class
