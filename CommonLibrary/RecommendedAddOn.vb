Imports System.Xml.Serialization

<XmlRoot("RecommendedAddOn")>
Public Class RecommendedAddOn

    Public Enum Types
        Freeware = 0
        Payware = 1
    End Enum

    <XmlElement("Name")>
    Public Property Name As String

    <XmlElement("URL")>
    Public Property URL As String

    <XmlElement("Type")>
    Public Property Type As Types

    Public Sub New()

    End Sub

    Public Overrides Function ToString() As String
        Return $"{Name} - {Type.ToString}"
    End Function

End Class
