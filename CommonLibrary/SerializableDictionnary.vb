Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Xml
Imports System.Xml.Schema

<XmlRoot("Dictionary")>
Public Class SerializableDictionary(Of TKey, TValue)
    Inherits Dictionary(Of TKey, TValue)
    Implements IXmlSerializable

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function GetSchema() As XMLSchema Implements IXmlSerializable.GetSchema
        Return Nothing
    End Function

    Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
        Dim keySerializer As New XmlSerializer(GetType(TKey))
        Dim valueSerializer As New XmlSerializer(GetType(TValue))
        Dim wasEmpty As Boolean = reader.IsEmptyElement
        reader.Read()

        If wasEmpty Then
            Return
        End If

        While reader.NodeType <> XmlNodeType.EndElement
            reader.ReadStartElement("item")

            reader.ReadStartElement("key")
            Dim key As TKey = CType(keySerializer.Deserialize(reader), TKey)
            reader.ReadEndElement()

            reader.ReadStartElement("value")
            Dim value As TValue = CType(valueSerializer.Deserialize(reader), TValue)
            reader.ReadEndElement()

            Me.Add(key, value)
            reader.ReadEndElement()
            reader.MoveToContent()
        End While
        reader.ReadEndElement()
    End Sub

    Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
        Dim keySerializer As New XmlSerializer(GetType(TKey))
        Dim valueSerializer As New XmlSerializer(GetType(TValue))
        For Each key As TKey In Me.Keys
            writer.WriteStartElement("item")

            writer.WriteStartElement("key")
            keySerializer.Serialize(writer, key)
            writer.WriteEndElement()

            writer.WriteStartElement("value")
            valueSerializer.Serialize(writer, Me(key))
            writer.WriteEndElement()

            writer.WriteEndElement()
        Next
    End Sub
End Class
