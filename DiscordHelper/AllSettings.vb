Imports System.IO
Imports System.Xml.Serialization

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

    <XmlElement("DPO_DPOUseCustomSettings")>
    Public Property DPO_DPOUseCustomSettings As Boolean

    <XmlElement("DPO_chkDPOIncludeCoverImage")>
    Public Property DPO_chkDPOIncludeCoverImage As Boolean

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

    <XmlElement("DPO_chkDGPOMainPost")>
    Public Property DPO_chkDGPOMainPost As Boolean

    <XmlElement("DPO_chkDGPOAltRestrictions")>
    Public Property DPO_chkDGPOAltRestrictions As Boolean

    <XmlElement("DPO_chkDGPOFullDescription")>
    Public Property DPO_chkDGPOFullDescription As Boolean

    <XmlElement("DPO_chkDGPOPublishWSGEventNews")>
    Public Property DPO_chkDGPOPublishWSGEventNews As Boolean

    <XmlElement("DPO_chkDGPOEventLogistics")>
    Public Property DPO_chkDGPOEventLogistics As Boolean

    <XmlElement("TaskDescriptionTemplate")>
    Public Property TaskDescriptionTemplate As String

    <XmlElement("EventDescriptionTemplate")>
    Public Property EventDescriptionTemplate As String

    <XmlElement("RemindUserPostOptions")>
    Public Property RemindUserPostOptions As Boolean?

    <XmlElement("LastUsedFileLocation")>
    Public Property LastUsedFileLocation As String
        Get
            If String.IsNullOrEmpty(_lastUsedFileLocation) OrElse Not Directory.Exists(_lastUsedFileLocation) Then
                ' Trim up one level at a time until we find an existing directory
                Do While Not String.IsNullOrEmpty(_lastUsedFileLocation) AndAlso Not Directory.Exists(_lastUsedFileLocation)
                    _lastUsedFileLocation = Directory.GetParent(_lastUsedFileLocation)?.FullName
                Loop
                ' If everything was invalid, default to the application's base directory
                If String.IsNullOrEmpty(_lastUsedFileLocation) OrElse Not Directory.Exists(_lastUsedFileLocation) Then
                    _lastUsedFileLocation = AppDomain.CurrentDomain.BaseDirectory
                End If
            End If
            Return _lastUsedFileLocation
        End Get
        Set(value As String)
            If Directory.Exists(value) Then
                _lastUsedFileLocation = value
            End If
        End Set
    End Property

    Private _lastUsedFileLocation As String

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
            DPO_DPOUseCustomSettings = settingsInFile.DPO_DPOUseCustomSettings
            DPO_chkDPOIncludeCoverImage = settingsInFile.DPO_chkDPOIncludeCoverImage
            DPO_DGPOUseCustomSettings = settingsInFile.DPO_DGPOUseCustomSettings
            DPO_chkDGPOCoverImage = settingsInFile.DPO_chkDGPOCoverImage
            DPO_chkDGPOMainGroupPost = settingsInFile.DPO_chkDGPOMainGroupPost
            DPO_chkDGPOThreadCreation = settingsInFile.DPO_chkDGPOThreadCreation
            DPO_chkDGPOTeaser = settingsInFile.DPO_chkDGPOTeaser
            DPO_chkDGPOFilesWithFullLegend = settingsInFile.DPO_chkDGPOFilesWithFullLegend
            DPO_chkDGPOMainPost = settingsInFile.DPO_chkDGPOMainPost
            DPO_chkDGPOFullDescription = settingsInFile.DPO_chkDGPOFullDescription
            DPO_chkDGPOAltRestrictions = settingsInFile.DPO_chkDGPOAltRestrictions
            DPO_chkDGPOPublishWSGEventNews = settingsInFile.DPO_chkDGPOPublishWSGEventNews
            DPO_chkDGPOEventLogistics = settingsInFile.DPO_chkDGPOEventLogistics
            TaskDescriptionTemplate = settingsInFile.TaskDescriptionTemplate
            EventDescriptionTemplate = settingsInFile.EventDescriptionTemplate
            RemindUserPostOptions = If(settingsInFile.RemindUserPostOptions.HasValue, settingsInFile.RemindUserPostOptions.Value, True)
            LastUsedFileLocation = settingsInFile.LastUsedFileLocation
        Else
            settingsFound = False
        End If

        Return settingsFound

    End Function

End Class
