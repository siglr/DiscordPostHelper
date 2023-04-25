Imports System.Configuration
Imports System.IO

Public Class AllSettings

    Public Enum AutoOverwriteOptions As Integer
        AlwaysOverwrite = 0
        AlwaysSkip = 1
        AlwaysAsk = 2
    End Enum

    Private _MSFSWeatherPresetsFolder As String
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

    Private _UnpackingFolder As String
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

    Public Property AutoOverwriteFiles As AutoOverwriteOptions


    Public Sub Save()
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)

        If Not config.AppSettings.Settings.AllKeys.Contains("MSFSWeatherPresetsFolder") Then
            config.AppSettings.Settings.Add("MSFSWeatherPresetsFolder", MSFSWeatherPresetsFolder)
        Else
            config.AppSettings.Settings("MSFSWeatherPresetsFolder").Value = MSFSWeatherPresetsFolder
        End If

        If Not config.AppSettings.Settings.AllKeys.Contains("FlightPlansFolder") Then
            config.AppSettings.Settings.Add("FlightPlansFolder", FlightPlansFolder)
        Else
            config.AppSettings.Settings("FlightPlansFolder").Value = FlightPlansFolder
        End If

        If Not config.AppSettings.Settings.AllKeys.Contains("UnpackingFolder") Then
            config.AppSettings.Settings.Add("UnpackingFolder", UnpackingFolder)
        Else
            config.AppSettings.Settings("UnpackingFolder").Value = UnpackingFolder
        End If

        If Not config.AppSettings.Settings.AllKeys.Contains("PackagesFolder") Then
            config.AppSettings.Settings.Add("PackagesFolder", PackagesFolder)
        Else
            config.AppSettings.Settings("PackagesFolder").Value = PackagesFolder
        End If

        If Not config.AppSettings.Settings.AllKeys.Contains("AutoOverwriteFiles") Then
            config.AppSettings.Settings.Add("AutoOverwriteFiles", CStr(CInt(AutoOverwriteFiles)))
        Else
            config.AppSettings.Settings("AutoOverwriteFiles").Value = CStr(CInt(AutoOverwriteFiles))
        End If

        config.Save(ConfigurationSaveMode.Modified)
        ConfigurationManager.RefreshSection("appSettings")
    End Sub

    Public Sub Load()
        Try
            MSFSWeatherPresetsFolder = ConfigurationManager.AppSettings("MSFSWeatherPresetsFolder")
            FlightPlansFolder = ConfigurationManager.AppSettings("FlightPlansFolder")
            UnpackingFolder = ConfigurationManager.AppSettings("UnpackingFolder")
            PackagesFolder = ConfigurationManager.AppSettings("PackagesFolder")
            AutoOverwriteFiles = CType(Integer.Parse(ConfigurationManager.AppSettings("AutoOverwriteFiles")), AutoOverwriteOptions)
        Catch ex As Exception

        End Try
    End Sub

End Class
