Public Class PresetEvent

    Public ReadOnly Property ClubName As String
    Public ReadOnly Property MSFSServer As String
    Public ReadOnly Property VoiceChannel As String
    Public ReadOnly Property EventDayOfWeek As DayOfWeek
    Public ReadOnly Property ZuluTime As DateTime
    Public ReadOnly Property SyncFlyDelay As Integer
    Public ReadOnly Property LaunchDelay As Integer
    Public ReadOnly Property StartTaskDelay As Integer
    Public ReadOnly Property EligibleAward As Boolean


    Public Sub New(pClubName As String,
                   pMSFSServer As String,
                   pVoiceChannel As String,
                   pZuluDayOfWeek As DayOfWeek,
                   pZuluTime As DateTime,
                   pSyncFlyDelay As Integer,
                   pLaunchTDelay As Integer,
                   pStartTaskDelay As Integer,
                   pEligibleAward As Boolean)

        ClubName = pClubName
        MSFSServer = pMSFSServer
        VoiceChannel = pVoiceChannel
        EventDayOfWeek = pZuluDayOfWeek
        ZuluTime = pZuluTime
        SyncFlyDelay = pSyncFlyDelay
        LaunchDelay = pLaunchTDelay
        StartTaskDelay = pStartTaskDelay
        EligibleAward = pEligibleAward

    End Sub

End Class
