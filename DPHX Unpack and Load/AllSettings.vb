Imports System.Configuration
Imports System.IO
Imports System.Linq.Expressions
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

<XmlRoot("DPHXUnpackLoadSettings")>
Public Class AllSettings

    Public Enum AutoOverwriteOptions As Integer
        AlwaysOverwrite = 0
        AlwaysSkip = 1
        AlwaysAsk = 2
    End Enum

    Private _MSFSWeatherPresetsFolder As String
    <XmlElement("MSFSWeatherPresetsFolder")>
    Public Property MSFSWeatherPresetsFolder As String
        Get
            Return _MSFSWeatherPresetsFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _MSFSWeatherPresetsFolder = value
            End If
        End Set
    End Property

    Private _FlightPlansFolder As String
    <XmlElement("FlightPlansFolder")>
    Public Property FlightPlansFolder As String
        Get
            Return _FlightPlansFolder
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _FlightPlansFolder = value
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

    <XmlElement("ExcludeFlightPlanFromCleanup")>
    Public Property ExcludeFlightPlanFromCleanup As Boolean

    <XmlElement("ExcludeWeatherFileFromCleanup")>
    Public Property ExcludeWeatherFileFromCleanup As Boolean

    <XmlElement("ExcludeXCSoarTaskFileFromCleanup")>
    Public Property ExcludeXCSoarTaskFileFromCleanup As Boolean

    <XmlElement("ExcludeXCSoarMapFileFromCleanup")>
    Public Property ExcludeXCSoarMapFileFromCleanup As Boolean

    <XmlElement("LocalDBTimestamp")>
    Public Property LocalDBTimestamp As String

    <XmlElement("TaskLibrarySortColumn")>
    Public Property TaskLibrarySortColumn As String

    <XmlElement("TaskLibrarySortAsc")>
    Public Property TaskLibrarySortAsc As Boolean

    <XmlElement("TaskLibrarySplitterLocation")>
    Public Property TaskLibrarySplitterLocation As Integer

    <XmlElement("TaskLibraryRightPartSplitterLocation")>
    Public Property TaskLibraryRightPartSplitterLocation As Integer

    <XmlElement("TaskLibraryDetailsZoomLevel")>
    Public Property TaskLibraryDetailsZoomLevel As Single

    <XmlArray("TBColumnsSettings")>
    <XmlArrayItem("Column")>
    Public Property TBColumnsSettings As List(Of TBColumnSetting)

    Public Sub New()

        ' Initialize the list
        TBColumnsSettings = New List(Of TBColumnSetting)

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

            _MSFSWeatherPresetsFolder = settingsInFile.MSFSWeatherPresetsFolder
            _FlightPlansFolder = settingsInFile.FlightPlansFolder
            _XCSoarTasksFolder = settingsInFile.XCSoarTasksFolder
            _XCSoarMapsFolder = settingsInFile.XCSoarMapsFolder
            _UnpackingFolder = settingsInFile.UnpackingFolder
            _PackagesFolder = settingsInFile.PackagesFolder
            _NB21IGCFolder = settingsInFile.NB21IGCFolder
            MainFormLocation = settingsInFile.MainFormLocation
            MainFormSize = settingsInFile.MainFormSize
            AutoOverwriteFiles = settingsInFile.AutoOverwriteFiles
            AutoUnpack = settingsInFile.AutoUnpack
            LastDPHXOpened = settingsInFile.LastDPHXOpened
            ExcludeFlightPlanFromCleanup = settingsInFile.ExcludeFlightPlanFromCleanup
            ExcludeWeatherFileFromCleanup = settingsInFile.ExcludeWeatherFileFromCleanup
            ExcludeXCSoarTaskFileFromCleanup = settingsInFile.ExcludeXCSoarTaskFileFromCleanup
            ExcludeXCSoarMapFileFromCleanup = settingsInFile.ExcludeXCSoarMapFileFromCleanup
            LocalDBTimestamp = settingsInFile.LocalDBTimestamp
            TaskLibrarySortColumn = settingsInFile.TaskLibrarySortColumn
            TaskLibrarySortAsc = settingsInFile.TaskLibrarySortAsc
            TaskLibrarySplitterLocation = settingsInFile.TaskLibrarySplitterLocation
            If TaskLibrarySplitterLocation = 0 Then
                TaskLibrarySplitterLocation = 60
            End If
            TaskLibraryRightPartSplitterLocation = settingsInFile.TaskLibraryRightPartSplitterLocation
            If TaskLibraryRightPartSplitterLocation = 0 Then
                TaskLibraryRightPartSplitterLocation = 50
            End If
            TaskLibraryDetailsZoomLevel = settingsInFile.TaskLibraryDetailsZoomLevel
            If TaskLibraryDetailsZoomLevel = 0 Then
                TaskLibraryDetailsZoomLevel = 1.5
            End If
            If TaskLibrarySortColumn = String.Empty Then
                TaskLibrarySortColumn = "LastUpdate"
                TaskLibrarySortAsc = True
            End If
            TBColumnsSettings = settingsInFile.TBColumnsSettings
            If LocalDBTimestamp = String.Empty Then
                LocalDBTimestamp = "None"
            End If

            'Check if valid folder
            If Not Directory.Exists(_FlightPlansFolder) Then
                settingsFound = False
            End If
            If Not Directory.Exists(_MSFSWeatherPresetsFolder) Then
                settingsFound = False
            End If
            If Not Directory.Exists(_UnpackingFolder) Then
                settingsFound = False
            End If
            If Not Directory.Exists(_PackagesFolder) Then
                settingsFound = False
            End If

        Else
                settingsFound = False

            'No settings found - try to auto locate the MSFS default folders
            Dim folderPathToCheck As String
            'Weather Presets
            folderPathToCheck = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalState\Weather\Presets"

            If Directory.Exists(folderPathToCheck) Then
                _MSFSWeatherPresetsFolder = folderPathToCheck
            End If
            'Flight plans
            folderPathToCheck = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalState"
            If Directory.Exists(folderPathToCheck) Then
                _FlightPlansFolder = folderPathToCheck
            End If

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

End Class

Public Class TBColumnSetting
    Public Property Name As String
    Public Property DisplayIndex As Integer
    Public Property Visible As Boolean
    Public Property ColumnWidth As Integer

    Public Sub New()
    End Sub

    Public Sub New(name As String, displayIndex As Integer, visible As Boolean, columnWidth As Integer)
        Me.Name = name
        Me.DisplayIndex = displayIndex
        Me.Visible = visible
        Me.ColumnWidth = columnWidth
    End Sub
End Class
