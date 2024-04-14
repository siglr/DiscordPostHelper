Imports System.IO
Imports SIGLR.SoaringTools.CommonLibrary

Public Class WeatherPreset

    Public Property Name As String
    Public ReadOnly Property UserDefined As Boolean
    Public Property ProfileDetails As WeatherDetails
    Public Property BuiltInData As WeatherPresetBuiltInData
    Public Property UserData As WeatherPresetUserData

    Dim _WeatherProfileXML As Xml.XmlDocument

    Public Sub New(pName As String, pUserDefined As Boolean)

        Name = pName
        UserDefined = pUserDefined

        Dim presetPath As String = GetPath()
        Dim filenameWithPath As String = $"{presetPath}\{Name}.wpr"

        'Check if weather file exists
        If File.Exists(filenameWithPath) Then
            _WeatherProfileXML = New Xml.XmlDocument()
            _WeatherProfileXML.Load(filenameWithPath)
            ProfileDetails = New WeatherDetails(_WeatherProfileXML)
            BuiltInData = New WeatherPresetBuiltInData(presetPath)
            UserData = New WeatherPresetUserData(presetPath)
        End If

    End Sub

    Public Sub Reload()
        BuiltInData.ReloadData()
        UserData.ReloadData()
    End Sub

    Public Sub Save()
        BuiltInData.SavePreset()
        UserData.SavePreset()
    End Sub

    Public Function GetPath()

        Dim appDirectory As String = AppDomain.CurrentDomain.BaseDirectory
        Dim weatherPresetsBaseDirectory As String = Path.Combine(appDirectory, "WeatherPresets")
        Dim weatherPresetFolder As String = Path.Combine(appDirectory, "WeatherPresets")

        If UserDefined Then
            weatherPresetFolder = $"{Path.Combine(weatherPresetsBaseDirectory, "UserDefined")}\{Name}"
        Else
            weatherPresetFolder = $"{Path.Combine(weatherPresetsBaseDirectory, "Default")}\{Name}"
        End If

        Return weatherPresetFolder

    End Function
End Class
