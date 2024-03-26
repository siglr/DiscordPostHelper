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

    <XmlElement("DPO_DPOUseCustomSettings")>
    Public Property DPO_DPOUseCustomSettings As Boolean

    <XmlElement("DPO_chkDPOMainPost")>
    Public Property DPO_chkDPOMainPost As Boolean

    <XmlElement("DPO_chkDPOThreadCreation")>
    Public Property DPO_chkDPOThreadCreation As Boolean

    <XmlElement("DPO_chkDPOIncludeCoverImage")>
    Public Property DPO_chkDPOIncludeCoverImage As Boolean

    <XmlElement("DPO_chkDPOFullDescription")>
    Public Property DPO_chkDPOFullDescription As Boolean

    <XmlElement("DPO_chkDPOFilesWithDescription")>
    Public Property DPO_chkDPOFilesWithDescription As Boolean

    <XmlElement("DPO_chkDPOFilesAlone")>
    Public Property DPO_chkDPOFilesAlone As Boolean

    <XmlElement("DPO_chkDPOAltRestrictions")>
    Public Property DPO_chkDPOAltRestrictions As Boolean

    <XmlElement("DPO_chkDPOWeatherInfo")>
    Public Property DPO_chkDPOWeatherInfo As Boolean

    <XmlElement("DPO_chkDPOWeatherChart")>
    Public Property DPO_chkDPOWeatherChart As Boolean

    <XmlElement("DPO_chkDPOWaypoints")>
    Public Property DPO_chkDPOWaypoints As Boolean

    <XmlElement("DPO_chkDPOAddOns")>
    Public Property DPO_chkDPOAddOns As Boolean

    <XmlElement("DPO_chkDPOResultsInvitation")>
    Public Property DPO_chkDPOResultsInvitation As Boolean

    <XmlElement("DPO_chkDPOFeaturedOnGroupFlight")>
    Public Property DPO_chkDPOFeaturedOnGroupFlight As Boolean

    <XmlElement("DPO_DGPOUseCustomSettings")>
    Public Property DPO_DGPOUseCustomSettings As Boolean

    <XmlElement("DPO_chkDGPOCoverImage")>
    Public Property DPO_chkDGPOCoverImage As Boolean

    <XmlElement("DPO_chkDGPOMainGroupPost")>
    Public Property DPO_chkDGPOMainGroupPost As Boolean

    <XmlElement("DPO_chkDGPOThreadCreation")>
    Public Property DPO_chkDGPOThreadCreation As Boolean

    <XmlElement("DPO_chkDGPOTeaser")>
    Public Property DPO_chkDGPOTeaser As Boolean

    <XmlElement("DPO_chkDGPOFilesWithFullLegend")>
    Public Property DPO_chkDGPOFilesWithFullLegend As Boolean

    <XmlElement("DPO_chkDGPOFilesWithoutLegend")>
    Public Property DPO_chkDGPOFilesWithoutLegend As Boolean

    <XmlElement("DPO_chkDGPODPHXOnly")>
    Public Property DPO_chkDGPODPHXOnly As Boolean

    <XmlElement("DPO_chkDGPOMainPost")>
    Public Property DPO_chkDGPOMainPost As Boolean

    <XmlElement("DPO_chkDGPOFullDescription")>
    Public Property DPO_chkDGPOFullDescription As Boolean

    <XmlElement("DPO_chkDGPOAltRestrictions")>
    Public Property DPO_chkDGPOAltRestrictions As Boolean

    <XmlElement("DPO_chkDGPOWeatherInfo")>
    Public Property DPO_chkDGPOWeatherInfo As Boolean

    <XmlElement("DPO_chkDGPOWeatherChart")>
    Public Property DPO_chkDGPOWeatherChart As Boolean

    <XmlElement("DPO_chkDGPOWaypoints")>
    Public Property DPO_chkDGPOWaypoints As Boolean

    <XmlElement("DPO_chkDGPOAddOns")>
    Public Property DPO_chkDGPOAddOns As Boolean

    <XmlElement("DPO_chkDGPORelevantTaskDetails")>
    Public Property DPO_chkDGPORelevantTaskDetails As Boolean

    <XmlElement("DPO_chkDGPOEventLogistics")>
    Public Property DPO_chkDGPOEventLogistics As Boolean

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
            DPO_DPOUseCustomSettings = settingsInFile.DPO_DPOUseCustomSettings
            DPO_chkDPOMainPost = settingsInFile.DPO_chkDPOMainPost
            DPO_chkDPOThreadCreation = settingsInFile.DPO_chkDPOThreadCreation
            DPO_chkDPOIncludeCoverImage = settingsInFile.DPO_chkDPOIncludeCoverImage
            DPO_chkDPOFullDescription = settingsInFile.DPO_chkDPOFullDescription
            DPO_chkDPOFilesWithDescription = settingsInFile.DPO_chkDPOFilesWithDescription
            DPO_chkDPOFilesAlone = settingsInFile.DPO_chkDPOFilesAlone
            DPO_chkDPOAltRestrictions = settingsInFile.DPO_chkDPOAltRestrictions
            DPO_chkDPOWeatherInfo = settingsInFile.DPO_chkDPOWeatherInfo
            DPO_chkDPOWeatherChart = settingsInFile.DPO_chkDPOWeatherChart
            DPO_chkDPOWaypoints = settingsInFile.DPO_chkDPOWaypoints
            DPO_chkDPOAddOns = settingsInFile.DPO_chkDPOAddOns
            DPO_chkDPOResultsInvitation = settingsInFile.DPO_chkDPOResultsInvitation
            DPO_chkDPOFeaturedOnGroupFlight = settingsInFile.DPO_chkDPOFeaturedOnGroupFlight
            DPO_DGPOUseCustomSettings = settingsInFile.DPO_DGPOUseCustomSettings
            DPO_chkDGPOCoverImage = settingsInFile.DPO_chkDGPOCoverImage
            DPO_chkDGPOMainGroupPost = settingsInFile.DPO_chkDGPOMainGroupPost
            DPO_chkDGPOThreadCreation = settingsInFile.DPO_chkDGPOThreadCreation
            DPO_chkDGPOTeaser = settingsInFile.DPO_chkDGPOTeaser
            DPO_chkDGPOFilesWithFullLegend = settingsInFile.DPO_chkDGPOFilesWithFullLegend
            DPO_chkDGPOFilesWithoutLegend = settingsInFile.DPO_chkDGPOFilesWithoutLegend
            DPO_chkDGPODPHXOnly = settingsInFile.DPO_chkDGPODPHXOnly
            DPO_chkDGPOMainPost = settingsInFile.DPO_chkDGPOMainPost
            DPO_chkDGPOFullDescription = settingsInFile.DPO_chkDGPOFullDescription
            DPO_chkDGPOAltRestrictions = settingsInFile.DPO_chkDGPOAltRestrictions
            DPO_chkDGPOWeatherInfo = settingsInFile.DPO_chkDGPOWeatherInfo
            DPO_chkDGPOWeatherChart = settingsInFile.DPO_chkDGPOWeatherChart
            DPO_chkDGPOWaypoints = settingsInFile.DPO_chkDGPOWaypoints
            DPO_chkDGPOAddOns = settingsInFile.DPO_chkDGPOAddOns
            DPO_chkDGPORelevantTaskDetails = settingsInFile.DPO_chkDGPORelevantTaskDetails
            DPO_chkDGPOEventLogistics = settingsInFile.DPO_chkDGPOEventLogistics
        Else
            settingsFound = False
        End If

        Return settingsFound

    End Function

End Class
