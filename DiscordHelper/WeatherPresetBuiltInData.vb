Imports System.IO
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

<XmlRoot("WeatherPresetBuiltInData")>
Public Class WeatherPresetBuiltInData

    Private _Exists As Boolean
    Private _FullFilename As String

    Private _Title As String
    <XmlElement("Title")>
    Public Property Title As String
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
        End Set
    End Property

    Private _Description As String
    <XmlElement("Description")>
    Public Property Description As String
        Get
            Return _Description
        End Get
        Set(value As String)
            _Description = value
        End Set
    End Property

    Private _SoaringTypeRidge As Boolean
    <XmlElement("SoaringTypeRidge")>
    Public Property SoaringTypeRidge As Boolean
        Get
            Return _SoaringTypeRidge
        End Get
        Set(value As Boolean)
            _SoaringTypeRidge = value
        End Set
    End Property

    Private _SoaringTypeThermal As Boolean
    <XmlElement("SoaringTypeThermal")>
    Public Property SoaringTypeThermal As Boolean
        Get
            Return _SoaringTypeThermal
        End Get
        Set(value As Boolean)
            _SoaringTypeThermal = value
        End Set
    End Property

    Private _SoaringTypeWave As Boolean
    <XmlElement("SoaringTypeWave")>
    Public Property SoaringTypeWave As Boolean
        Get
            Return _SoaringTypeWave
        End Get
        Set(value As Boolean)
            _SoaringTypeWave = value
        End Set
    End Property

    Private _SoaringTypeDynamic As Boolean
    <XmlElement("SoaringTypeDynamic")>
    Public Property SoaringTypeDynamic As Boolean
        Get
            Return _SoaringTypeDynamic
        End Get
        Set(value As Boolean)
            _SoaringTypeDynamic = value
        End Set
    End Property

    Public ReadOnly Property AllSoaringTypes As String
        Get
            Dim result As String = String.Empty
            If _SoaringTypeRidge Then
                result = $"{result}R"
            End If
            If _SoaringTypeThermal Then
                result = $"{result}T"
            End If
            If _SoaringTypeWave Then
                result = $"{result}W"
            End If
            If _SoaringTypeDynamic Then
                result = $"{result}D"
            End If
            Return result
        End Get
    End Property

    Public Sub New()

    End Sub
    Public Sub New(presetPath As String)

        _FullFilename = GetFullFilename(presetPath)

        'Check if file exists
        If File.Exists(_FullFilename) Then
            _Exists = True
            ReloadData()
        Else
            _Exists = False
        End If

    End Sub

    Public Sub ReloadData()
        If Not Load() Then
            'TODO: Error ?
        End If
    End Sub

    Public Sub SavePreset()
        Save()
    End Sub

    Private Function GetFullFilename(weatherFilename As String) As String
        Return $"{weatherFilename}\BuiltInData.xml"
    End Function

    Private Sub Save()
        Dim serializer As New XmlSerializer(GetType(WeatherPresetBuiltInData))
        Using stream As New FileStream(_FullFilename, FileMode.Create)
            serializer.Serialize(stream, Me)
        End Using
    End Sub

    Private Function Load() As Boolean

        Dim fileFound As Boolean = True

        If File.Exists(_FullFilename) Then
            Dim serializer As New XmlSerializer(GetType(WeatherPresetBuiltInData))
            Dim settingsInFile As WeatherPresetBuiltInData

            'On Error Resume Next

            Using stream As New FileStream(_FullFilename, FileMode.Open)
                settingsInFile = CType(serializer.Deserialize(stream), WeatherPresetBuiltInData)
            End Using

            With settingsInFile
                _Title = .Title
                _SoaringTypeRidge = .SoaringTypeRidge
                _SoaringTypeThermal = .SoaringTypeThermal
                _SoaringTypeWave = .SoaringTypeWave
                _SoaringTypeDynamic = .SoaringTypeDynamic
                _Description = .Description
            End With

        Else
            fileFound = False
        End If

        Return fileFound

    End Function

End Class

