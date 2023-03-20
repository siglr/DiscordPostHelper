Imports System.Xml.Serialization

<Serializable>
<XmlRoot("VersionInfo")>
Public Class VersionInfo

    <XmlElement("CurrentLatestVersion")>
    Public Property CurrentLatestVersion As String

    <XmlArray("Releases")>
    <XmlArrayItem("Release")>
    Public Property Releases As List(Of Release)

    Public Sub New()

    End Sub

End Class

<Serializable>
Public Class Release
    <XmlElement("ReleaseVersion")>
    Public Property ReleaseVersion As String

    <XmlElement("ReleaseTitle")>
    Public Property ReleaseTitle As String

    <XmlElement("ReleaseNotes")>
    <XmlText>
    Public Property ReleaseNotes As String

    Public Sub New()

    End Sub

End Class



