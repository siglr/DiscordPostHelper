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

            _MSFSWeatherPresetsFolder = settingsInFile.MSFSWeatherPresetsFolder
            _FlightPlansFolder = settingsInFile.FlightPlansFolder
            _XCSoarTasksFolder = settingsInFile.XCSoarTasksFolder
            _UnpackingFolder = settingsInFile.UnpackingFolder
            _PackagesFolder = settingsInFile.PackagesFolder
            MainFormLocation = settingsInFile.MainFormLocation
            MainFormSize = settingsInFile.MainFormSize
            AutoOverwriteFiles = settingsInFile.AutoOverwriteFiles
            AutoUnpack = settingsInFile.AutoUnpack
            LastDPHXOpened = settingsInFile.LastDPHXOpened

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

End Class
