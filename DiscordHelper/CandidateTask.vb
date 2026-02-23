Imports System.Xml
Imports SIGLR.SoaringTools.CommonLibrary

Public Class CandidateTask
    Public Property EntrySeqID As Integer
    Public Property TaskID As String
    Public Property Title As String
    Public Property LastUpdate As String
    Public Property OwnerName As String
    Public Property PLNXML As String
    Public Property WPRXML As String
    Public Property SimDateTime As Date?

    Private _allWaypoints As List(Of ATCWaypoint)
    Public ReadOnly Property AllWaypoints As List(Of ATCWaypoint)
        Get
            Return _allWaypoints
        End Get
    End Property

    Private _altRestrictions As String
    Public ReadOnly Property AltRestrictions As String
        Get
            Return _altRestrictions
        End Get
    End Property

    Private _totalDistance As Integer
    Public ReadOnly Property TotalDistance As Integer
        Get
            Return _totalDistance
        End Get
    End Property

    Private _trackDistance As Integer
    Public ReadOnly Property TrackDistance As Integer
        Get
            Return _trackDistance
        End Get
    End Property

    Public Sub CompleteTaskData()
        Dim sf As New SupportingFeatures
        Dim elevUpdate As Boolean
        Dim taskXMLDoc As New XmlDocument
        taskXMLDoc.LoadXml(PLNXML)
        _altRestrictions = sf.BuildAltitudeRestrictions(taskXMLDoc, _totalDistance, _trackDistance, elevUpdate)
        _allWaypoints = sf.AllWaypoints
        sf = Nothing
    End Sub

End Class
