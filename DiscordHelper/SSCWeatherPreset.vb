Public Class SSCWeatherPreset

    Public Property PresetDescriptiveName As String
    Public Property PresetMSFSTitle As String
    Public Property PresetID As String
    Public Property PresetFile2020 As String
    Public Property PresetFile2024 As String

    Public Shared Function LoadSSCWeatherPresets() As Dictionary(Of String, SSCWeatherPreset)
        Dim presets As New Dictionary(Of String, SSCWeatherPreset)
        Try
            'TODO: Fetch the SSC Weather Presets from the server and populate the dictionary with key as PresetDescriptiveName and value as SSCWeatherPreset object
        Catch ex As Exception
            ' Handle exceptions (e.g., log error)
        End Try
        Return presets
    End Function

End Class
