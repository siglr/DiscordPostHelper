Imports System.Configuration
Imports System.IO
Imports System.Linq.Expressions
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

<XmlRoot("DPHSettings")>
Public Class AllSettings

    Private _LastFileLoaded As String
    <XmlElement("LastFileLoaded")>
    Public Property LastFileLoaded As String
        Get
            Return _LastFileLoaded
        End Get
        Set(value As String)
            If value = String.Empty OrElse File.Exists(value) Then
                _LastFileLoaded = value
            End If
        End Set
    End Property

    <XmlElement("FlightPlanTabSplitterLocation")>
    Public Property FlightPlanTabSplitterLocation As Integer

    <XmlElement("MainFormSize")>
    Public Property MainFormSize As String

    <XmlElement("MainFormLocation")>
    Public Property MainFormLocation As String

    <XmlElement("WaitSecondsForFiles")>
    Public Property WaitSecondsForFiles As Integer

    <XmlElement("AutomaticPostingProgression")>
    Public Property AutomaticPostingProgression As Boolean

    Public Sub New()

    End Sub

    Public Sub Save()
        Dim serializer As New XmlSerializer(GetType(AllSettings))
        Using stream As New FileStream($"{Application.StartupPath}\DPHSettings.xml", FileMode.Create)
            serializer.Serialize(stream, Me)
        End Using
    End Sub

    Public Function Load() As Boolean

        Dim settingsFound As Boolean = True

        If File.Exists($"{Application.StartupPath}\DPHSettings.xml") Then
            Dim serializer As New XmlSerializer(GetType(AllSettings))
            Dim settingsInFile As AllSettings

            On Error Resume Next

            Using stream As New FileStream($"{Application.StartupPath}\DPHSettings.xml", FileMode.Open)
                settingsInFile = CType(serializer.Deserialize(stream), AllSettings)
            End Using

            _LastFileLoaded = settingsInFile.LastFileLoaded
            MainFormLocation = settingsInFile.MainFormLocation
            MainFormSize = settingsInFile.MainFormSize
            FlightPlanTabSplitterLocation = settingsInFile.FlightPlanTabSplitterLocation
            WaitSecondsForFiles = settingsInFile.WaitSecondsForFiles
            AutomaticPostingProgression = settingsInFile.AutomaticPostingProgression

        Else
            settingsFound = False
        End If

        Return settingsFound

    End Function

End Class
