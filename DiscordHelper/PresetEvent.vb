Public Class PresetEvent

    Private strClubName As String
    Private strMSFSServer As String
    Private strVoiceChannel As String
    Private dowEventDayOfWeek As DayOfWeek
    Private timeZuluTime As DateTime
    Private intSyncFlyTimeDelay As Integer
    Private intLaunchDelay As Integer
    Private intStartTaskDelay As Integer
    Private blnEligibleAward As Boolean

    Public Sub New(pClubName As String,
                   pMSFSServer As String,
                   pVoiceChannel As String,
                   pZuluDayOfWeek As DayOfWeek,
                   pZuluTime As DateTime,
                   pSyncFlyDelay As Integer,
                   pLaunchTDelay As Integer,
                   pStartTaskDelay As Integer,
                   pEligibleAward As Boolean)

        strClubName = pClubName
        strMSFSServer = pMSFSServer
        strVoiceChannel = pVoiceChannel
        dowEventDayOfWeek = pZuluDayOfWeek
        timeZuluTime = pZuluTime
        intSyncFlyTimeDelay = pSyncFlyDelay
        intLaunchDelay = pLaunchTDelay
        intStartTaskDelay = pStartTaskDelay
        blnEligibleAward = pEligibleAward

    End Sub

    Public ReadOnly Property ClubName As String
        Get
            Return strClubName
        End Get
    End Property

    Public ReadOnly Property MSFSServer As String
        Get
            Return strMSFSServer
        End Get
    End Property

    Public ReadOnly Property VoiceChannel As String
        Get
            Return strVoiceChannel
        End Get
    End Property

    Public ReadOnly Property EventDayOfWeek As DayOfWeek
        Get
            Return dowEventDayOfWeek
        End Get
    End Property

    Public ReadOnly Property ZuluTime As DateTime
        Get
            Return timeZuluTime
        End Get
    End Property

    Public ReadOnly Property SyncFlyDelay As Integer
        Get
            Return intSyncFlyTimeDelay
        End Get
    End Property

    Public ReadOnly Property LaunchDelay As Integer
        Get
            Return intLaunchDelay
        End Get
    End Property

    Public ReadOnly Property StartTaskDelay As Integer
        Get
            Return intStartTaskDelay
        End Get
    End Property

    Public ReadOnly Property EligibleAward As Boolean
        Get
            Return blnEligibleAward
        End Get
    End Property

End Class
