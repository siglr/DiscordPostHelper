Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Public Class SSCWeatherPreset

    Public Property PresetDescriptiveName As String
    Public Property PresetMSFSTitle2024 As String
    Public Property PresetMSFSTitle2020 As String
    Public Property PresetID As String
    Public Property PresetFile2020 As String
    Public Property PresetFile2024 As String

    Public Shared Function LoadSSCWeatherPresets() As Dictionary(Of String, SSCWeatherPreset)
        Dim presets As New Dictionary(Of String, SSCWeatherPreset)(StringComparer.OrdinalIgnoreCase)

        Try
            Dim apiUrl As String = $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveSSCWeatherPresets.php"

            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "GET"
            request.Timeout = 30000

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    If result("status") IsNot Nothing AndAlso result("status").ToString() = "success" Then
                        Dim arr As JArray = CType(result("presets"), JArray)

                        For Each p As JObject In arr
                            Dim preset As New SSCWeatherPreset With {
                                .PresetID = SafeToken(p, "PresetID"),
                                .PresetDescriptiveName = SafeToken(p, "PresetDescriptiveName"),
                                .PresetMSFSTitle2024 = SafeToken(p, "PresetMSFSTitle2024"),
                                .PresetMSFSTitle2020 = SafeToken(p, "PresetMSFSTitle2020"),
                                .PresetFile2024 = SafeToken(p, "PresetFile2024"),
                                .PresetFile2020 = SafeToken(p, "PresetFile2020")
                            }

                            ' Key = PresetDescriptiveName (primary = 2024)
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

    ' Helper: safe JSON field read (avoids NullReference crashes)
    Private Shared Function SafeToken(obj As JObject, key As String) As String
        Dim t As JToken = obj(key)
        If t Is Nothing OrElse t.Type = JTokenType.Null Then Return ""
        Return t.ToString()
    End Function

    ' --- Preferred overload: pass PresetID directly (string or numeric) ---
    Public Shared Function DownloadSSCWeatherPresetZipByID(presetId As String, targetFolder As String) As String
        If String.IsNullOrWhiteSpace(presetId) OrElse Not Integer.TryParse(presetId.Trim(), Nothing) Then
            Throw New Exception("Invalid presetId.")
        End If

        If Not Directory.Exists(targetFolder) Then
            Directory.CreateDirectory(targetFolder)
        End If

        Dim apiUrl As String =
            $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}DownloadSSCWeatherPresetZip.php?presetId={presetId.Trim()}"

        Dim zipFileName As String = $"SSCWeatherPreset_{presetId.Trim()}.zip"
        Dim zipFullPath As String = Path.Combine(targetFolder, zipFileName)

        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "GET"
            request.Timeout = 60000

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                If response.StatusCode <> HttpStatusCode.OK Then
                    Throw New Exception($"HTTP error: {response.StatusCode}")
                End If

                Using responseStream As Stream = response.GetResponseStream()
                    Using fileStream As New FileStream(zipFullPath, FileMode.Create, FileAccess.Write)
                        responseStream.CopyTo(fileStream)
                    End Using
                End Using
            End Using

            If Not File.Exists(zipFullPath) Then
                Throw New Exception("ZIP file was not downloaded.")
            End If

            Dim fileInfo As New FileInfo(zipFullPath)
            If fileInfo.Length = 0 Then
                File.Delete(zipFullPath)
                Throw New Exception("Downloaded ZIP file is empty.")
            End If

            Return zipFullPath

        Catch ex As WebException
            Throw New Exception("Failed to download SSC weather preset ZIP: " & ex.Message & ReadWebExceptionBody(ex))
        Catch ex As Exception
            Throw New Exception("Error downloading SSC weather preset ZIP: " & ex.Message)
        End Try
    End Function

    ' --- Backward-compatible overload: keep taking a "presetName" but extract ID safely ---
    ' If you pass the descriptive name (starts with "001 ..."), this still works.
    ' But preferred is to call DownloadSSCWeatherPresetZipByID(preset.PresetID, ...)
    Public Shared Function DownloadSSCWeatherPresetZip(presetName As String, targetFolder As String) As String
        If String.IsNullOrWhiteSpace(presetName) OrElse presetName.Length < 3 Then
            Throw New Exception("Invalid preset name.")
        End If

        Dim presetIdPart As String = presetName.Substring(0, 3)
        If Not Integer.TryParse(presetIdPart, Nothing) Then
            Throw New Exception("Preset name does not start with a valid numeric ID.")
        End If

        Return DownloadSSCWeatherPresetZipByID(presetIdPart, targetFolder)
    End Function

    Private Shared Function ReadWebExceptionBody(ex As WebException) As String
        Try
            Dim resp = TryCast(ex.Response, HttpWebResponse)
            If resp Is Nothing Then Return ""

            Using s = resp.GetResponseStream()
                If s Is Nothing Then Return ""
                Using r As New StreamReader(s)
                    Dim body As String = r.ReadToEnd()
                    If String.IsNullOrWhiteSpace(body) Then Return ""
                    Return " | " & body
                End Using
            End Using
        Catch
            Return ""
        End Try
    End Function

End Class
