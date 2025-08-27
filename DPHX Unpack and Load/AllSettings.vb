Imports System.IO
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

<XmlRoot("DPHXUnpackLoadSettings")>
Public Class AllSettings

    Public Enum AutoOverwriteOptions As Integer
        AlwaysOverwrite = 0
        AlwaysSkip = 1
        AlwaysAsk = 2
    End Enum

    Public Enum WSGIntegrationOptions As Integer
        None = 0
        OpenHome = 1
        OpenMap = 2
        OpenEvents = 3
    End Enum

    <XmlElement("MSFS2020Steam")>
    Public Property MSFS2020Steam As Boolean

    <XmlElement("MSFS2020Microsoft")>
    Public Property MSFS2020Microsoft As Boolean


    <XmlElement("MSFS2024Steam")>
    Public Property MSFS2024Steam As Boolean

    <XmlElement("MSFS2024Microsoft")>
    Public Property MSFS2024Microsoft As Boolean

    Private _MSFS2020WeatherPresetsFolder As String
    <XmlElement("MSFSWeatherPresetsFolder")>
    Public Property MSFS2020WeatherPresetsFolder As String
        Get
            Return _MSFS2020WeatherPresetsFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _MSFS2020WeatherPresetsFolder = value
            End If
        End Set
    End Property

    Private _MSFS2020FlightPlansFolder As String
    <XmlElement("FlightPlansFolder")>
    Public Property MSFS2020FlightPlansFolder As String
        Get
            Return _MSFS2020FlightPlansFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _MSFS2020FlightPlansFolder = value
            End If
        End Set
    End Property

    Private _MSFS2024WeatherPresetsFolder As String
    <XmlElement("2024WeatherPresetsFolder")>
    Public Property MSFS2024WeatherPresetsFolder As String
        Get
            Return _MSFS2024WeatherPresetsFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _MSFS2024WeatherPresetsFolder = value
            End If
        End Set
    End Property

    Private _MSFS2024FlightPlansFolder As String
    <XmlElement("2024FlightPlansFolder")>
    Public Property MSFS2024FlightPlansFolder As String
        Get
            Return _MSFS2024FlightPlansFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _MSFS2024FlightPlansFolder = value
            End If
        End Set
    End Property

    Private _XCSoarTasksFolder As String
    <XmlElement("XCSoarTasksFolder")>
    Public Property XCSoarTasksFolder As String
        Get
            Return _XCSoarTasksFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _XCSoarTasksFolder = value
            End If
        End Set
    End Property

    Private _XCSoarMapsFolder As String
    <XmlElement("XCSoarMapsFolder")>
    Public Property XCSoarMapsFolder As String
        Get
            Return _XCSoarMapsFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _XCSoarMapsFolder = value
            End If
        End Set
    End Property

    Private _UnpackingFolder As String
    <XmlElement("UnpackingFolder")>
    Public Property UnpackingFolder As String
        Get
            Return _UnpackingFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _UnpackingFolder = value
            End If
        End Set
    End Property

    Private _PackagesFolder As String
    <XmlElement("PackagesFolder")>
    Public Property PackagesFolder As String
        Get
            Return _PackagesFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _PackagesFolder = value
            End If
        End Set
    End Property

    Private _NB21IGCFolder As String
    <XmlElement("NB21IGCFolder")>
    Public Property NB21IGCFolder As String
        Get
            Return _NB21IGCFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _NB21IGCFolder = value
            End If
        End Set
    End Property

    <XmlElement("NB21StartAndFeed")>
    Public Property NB21StartAndFeed As Boolean

    Private _NB21EXEFolder As String
    <XmlElement("NB21EXEFolder")>
    Public Property NB21EXEFolder As String
        Get
            Return _NB21EXEFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _NB21EXEFolder = value
            End If
        End Set
    End Property

    Private _NB21LocalWSPort As String
    <XmlElement("NB21LocalWSPort")>
    Public Property NB21LocalWSPort As String
        Get
            Return _NB21LocalWSPort
        End Get
        Set(value As String)
            Dim port As Integer
            If Integer.TryParse(value, port) AndAlso port >= 0 AndAlso port <= 65535 AndAlso port <> _LocalWebServerPort Then
                _NB21LocalWSPort = value
            End If
        End Set
    End Property

    <XmlElement("TrackerStartAndFeed")>
    Public Property TrackerStartAndFeed As Boolean

    Private _TrackerEXEFolder As String
    <XmlElement("TrackerEXEFolder")>
    Public Property TrackerEXEFolder As String
        Get
            Return _TrackerEXEFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _TrackerEXEFolder = value
            End If
        End Set
    End Property

    Private _TrackerLocalWSPort As String
    <XmlElement("TrackerLocalWSPort")>
    Public Property TrackerLocalWSPort As String
        Get
            Return _TrackerLocalWSPort
        End Get
        Set(value As String)
            Dim port As Integer
            If Integer.TryParse(value, port) AndAlso port >= 0 AndAlso port <= 65535 AndAlso port <> _LocalWebServerPort Then
                _TrackerLocalWSPort = value
            End If
        End Set
    End Property

    <XmlElement("AutoOverwriteFiles")>
    Public Property AutoOverwriteFiles As AutoOverwriteOptions

    <XmlElement("MainFormSize")>
    Public Property MainFormSize As String

    <XmlElement("MainFormLocation")>
    Public Property MainFormLocation As String

    <XmlElement("LastDPHXOpened")>
    Public Property LastDPHXOpened As String

    <XmlElement("AutoUnpack")>
    Public Property AutoUnpack As Boolean

    <XmlElement("EnableEFBFlightPlanCreation")>
    Public Property EnableEFBFlightPlanCreation As Boolean

    <XmlElement("ExcludeFlightPlanFromCleanup")>
    Public Property Exclude2020FlightPlanFromCleanup As Boolean

    <XmlElement("ExcludeWeatherFileFromCleanup")>
    Public Property Exclude2020WeatherFileFromCleanup As Boolean

    <XmlElement("Exclude2024FlightPlanFromCleanup")>
    Public Property Exclude2024FlightPlanFromCleanup As Boolean

    <XmlElement("Exclude2024WeatherFileFromCleanup")>
    Public Property Exclude2024WeatherFileFromCleanup As Boolean

    <XmlElement("ExcludeXCSoarTaskFileFromCleanup")>
    Public Property ExcludeXCSoarTaskFileFromCleanup As Boolean

    <XmlElement("ExcludeXCSoarMapFileFromCleanup")>
    Public Property ExcludeXCSoarMapFileFromCleanup As Boolean

    Private _LocalWebServerPort As String
    <XmlElement("LocalWebServerPort")>
    Public Property LocalWebServerPort As String
        Get
            Return _LocalWebServerPort
        End Get
        Set(value As String)
            Dim port As Integer
            If Integer.TryParse(value, port) AndAlso port >= 0 AndAlso port <= 65535 AndAlso port <> _NB21LocalWSPort Then
                _LocalWebServerPort = value
            End If
        End Set
    End Property

    <XmlElement("WSGIntegration")>
    Public Property WSGIntegration As WSGIntegrationOptions

    <XmlElement("WSGIgnoreWhenOpeningDPHX")>
    Public Property WSGIgnoreWhenOpeningDPHX As Boolean

    <XmlElement("WSGListenerAutoStart")>
    Public Property WSGListenerAutoStart As Boolean

    Public ReadOnly Property Is2020Installed As Boolean
        Get
            Return MSFS2020Microsoft OrElse MSFS2020Steam
        End Get
    End Property
    Public ReadOnly Property Is2024Installed As Boolean
        Get
            Return MSFS2024Microsoft OrElse MSFS2024Steam
        End Get
    End Property


    Public Sub New()

    End Sub

    Public Sub Save()
        Dim serializer As New XmlSerializer(GetType(AllSettings))
        Using stream As New FileStream($"{Application.StartupPath}\DPHXUnpackLoadSettings.xml", FileMode.Create)
            serializer.Serialize(stream, Me)
        End Using
    End Sub

    Public Function Load() As Boolean

        Dim settingsFound As Boolean = True

        If File.Exists($"{Application.StartupPath}\DPHXUnpackLoadSettings.xml") Then
            Dim serializer As New XmlSerializer(GetType(AllSettings))
            Dim settingsInFile As AllSettings

            On Error Resume Next

            Using stream As New FileStream($"{Application.StartupPath}\DPHXUnpackLoadSettings.xml", FileMode.Open)
                settingsInFile = CType(serializer.Deserialize(stream), AllSettings)
            End Using

            MSFS2020Microsoft = settingsInFile.MSFS2020Microsoft
            MSFS2020Steam = settingsInFile.MSFS2020Steam
            MSFS2024Microsoft = settingsInFile.MSFS2024Microsoft
            MSFS2024Steam = settingsInFile.MSFS2024Steam
            _MSFS2020WeatherPresetsFolder = settingsInFile.MSFS2020WeatherPresetsFolder
            _MSFS2020FlightPlansFolder = settingsInFile.MSFS2020FlightPlansFolder
            _MSFS2024WeatherPresetsFolder = settingsInFile.MSFS2024WeatherPresetsFolder
            _MSFS2024FlightPlansFolder = settingsInFile.MSFS2024FlightPlansFolder
            _XCSoarTasksFolder = settingsInFile.XCSoarTasksFolder
            _XCSoarMapsFolder = settingsInFile.XCSoarMapsFolder
            _UnpackingFolder = settingsInFile.UnpackingFolder
            _PackagesFolder = settingsInFile.PackagesFolder
            _NB21IGCFolder = settingsInFile.NB21IGCFolder
            _NB21EXEFolder = settingsInFile.NB21EXEFolder
            _NB21LocalWSPort = settingsInFile.NB21LocalWSPort
            If _NB21LocalWSPort = 0 Then
                _NB21LocalWSPort = 54178
            End If
            NB21StartAndFeed = settingsInFile.NB21StartAndFeed
            _TrackerEXEFolder = settingsInFile.TrackerEXEFolder
            _TrackerLocalWSPort = settingsInFile.TrackerLocalWSPort
            If _TrackerLocalWSPort = 0 Then
                _TrackerLocalWSPort = 55055
            End If
            TrackerStartAndFeed = settingsInFile.TrackerStartAndFeed
            _LocalWebServerPort = settingsInFile.LocalWebServerPort
            If _LocalWebServerPort = 0 Then
                _LocalWebServerPort = 54513
            End If
            MainFormLocation = settingsInFile.MainFormLocation
            MainFormSize = settingsInFile.MainFormSize
            AutoOverwriteFiles = settingsInFile.AutoOverwriteFiles
            AutoUnpack = settingsInFile.AutoUnpack
            EnableEFBFlightPlanCreation = settingsInFile.EnableEFBFlightPlanCreation
            LastDPHXOpened = settingsInFile.LastDPHXOpened
            Exclude2020FlightPlanFromCleanup = settingsInFile.Exclude2020FlightPlanFromCleanup
            Exclude2020WeatherFileFromCleanup = settingsInFile.Exclude2020WeatherFileFromCleanup
            Exclude2024FlightPlanFromCleanup = settingsInFile.Exclude2024FlightPlanFromCleanup
            Exclude2024WeatherFileFromCleanup = settingsInFile.Exclude2024WeatherFileFromCleanup
            ExcludeXCSoarTaskFileFromCleanup = settingsInFile.ExcludeXCSoarTaskFileFromCleanup
            ExcludeXCSoarMapFileFromCleanup = settingsInFile.ExcludeXCSoarMapFileFromCleanup
            WSGIntegration = settingsInFile.WSGIntegration
            WSGIgnoreWhenOpeningDPHX = settingsInFile.WSGIgnoreWhenOpeningDPHX
            WSGListenerAutoStart = settingsInFile.WSGListenerAutoStart

            'Check if at least one installation
            If Not (MSFS2020Microsoft OrElse MSFS2020Steam OrElse MSFS2024Microsoft OrElse MSFS2024Steam) Then
                settingsFound = False
            End If

            'Check if valid folder
            If MSFS2020Microsoft OrElse MSFS2020Steam Then
                If Not Directory.Exists(_MSFS2020FlightPlansFolder) Then
                    settingsFound = False
                End If
                If Not Directory.Exists(_MSFS2020WeatherPresetsFolder) Then
                    settingsFound = False
                End If
            End If
            If MSFS2024Microsoft OrElse MSFS2024Steam Then
                If Not Directory.Exists(_MSFS2024FlightPlansFolder) Then
                    settingsFound = False
                End If
                If Not Directory.Exists(_MSFS2024WeatherPresetsFolder) Then
                    settingsFound = False
                End If
            End If
            If Not Directory.Exists(_UnpackingFolder) Then
                settingsFound = False
            End If
            If Not Directory.Exists(_PackagesFolder) Then
                settingsFound = False
            End If

        Else
            settingsFound = False
        End If

        Return settingsFound

    End Function

    Public Sub ClearXCSoarTasks()
        _XCSoarTasksFolder = Nothing
    End Sub

    Public Sub ClearXCSoarMaps()
        _XCSoarMapsFolder = Nothing
    End Sub

    Public Sub ClearNB21IGCFolder()
        _NB21IGCFolder = Nothing
    End Sub

    Public Sub ClearNB21EXEFolder()
        _NB21EXEFolder = Nothing
    End Sub

    Public Sub ClearTrackerEXEFolder()
        _TrackerEXEFolder = Nothing
    End Sub
End Class
