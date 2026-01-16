Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary
Public Class SSCWeatherPreset

    Public Property PresetDescriptiveName As String
    Public Property PresetMSFSTitle As String
    Public Property PresetID As String
    Public Property PresetFile2020 As String
    Public Property PresetFile2024 As String

    Public Shared Function LoadSSCWeatherPresets() As Dictionary(Of String, SSCWeatherPreset)
        Dim presets As New Dictionary(Of String, SSCWeatherPreset)(StringComparer.OrdinalIgnoreCase)

        Try
            Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveSSCWeatherPresets.php"

            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "GET"
            request.Timeout = 30000 ' 30s (optional)

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    If result("status") IsNot Nothing AndAlso result("status").ToString() = "success" Then
                        Dim arr As JArray = JArray.Parse(result("presets").ToString())

                        For Each p As JObject In arr
                            Dim preset As New SSCWeatherPreset With {
                                .PresetID = p("PresetID").ToString(),
                                .PresetDescriptiveName = p("PresetDescriptiveName").ToString(),
                                .PresetMSFSTitle = p("PresetMSFSTitle").ToString(),
                                .PresetFile2020 = p("PresetFile2020").ToString(),
                                .PresetFile2024 = p("PresetFile2024").ToString()
                            }

                            ' Key = PresetDescriptiveName (as requested)
                            ' If duplicates ever happen, last one wins
                            presets(preset.PresetDescriptiveName) = preset
                        Next
                    Else
                        Dim msg As String = If(result("message") IsNot Nothing, result("message").ToString(), "Unknown error")
                        Throw New Exception("Error retrieving SSC weather presets: " & msg)
                    End If
                End Using
            End Using

        Catch ex As Exception
            Throw New Exception("Failed to retrieve SSC weather presets: " & ex.Message)
        End Try

        Return presets
    End Function

End Class
