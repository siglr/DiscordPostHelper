Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq

Friend Module WeatherCommunityPackageHelper

    Friend Enum PackageEnsureResult
        Ready
        NotConfigured
        NeedsFolderChange
        Cancelled
        Failed
    End Enum

    Private Const PackageFolderName As String = "0_DPHX-Weather"
    Private Const WeatherPresetsFolderName As String = "WeatherPresets"
    Private Const ContentInfoFolderName As String = "ContentInfo"
    Private Const ManifestFileName As String = "manifest.json"
    Private Const LayoutFileName As String = "layout.json"
    Private Const ThumbnailSourceName As String = "DPHXWeatherPackThumbail.jpg"
    Private Const ThumbnailFileName As String = "Thumbnail.jpg"
    Private Const MinimumGameVersion As String = "1.0.0"
    Private Const MinimumCompatibilityVersion As String = "1.0.0"

    Friend Function EnsureWeatherCommunityPackage(simLabel As String,
                                                 communityFolder As String,
                                                 owner As IWin32Window) As PackageEnsureResult
        If String.IsNullOrWhiteSpace(communityFolder) Then
            LogPackageEvent($"{simLabel}: Community folder not configured.")
            Return PackageEnsureResult.NotConfigured
        End If

        If Not Directory.Exists(communityFolder) Then
            LogPackageEvent($"{simLabel}: Community folder does not exist: {communityFolder}", TraceEventType.Warning)
            Return PackageEnsureResult.Failed
        End If

        Dim missingItems As List(Of String) = Nothing
        If IsPackageComplete(communityFolder, missingItems) Then
            Return PackageEnsureResult.Ready
        End If

        LogPackageEvent($"{simLabel}: Missing/incomplete package items: {String.Join(", ", missingItems)}")

        Dim messageBuilder As New StringBuilder()
        messageBuilder.AppendLine($"The required DPHX Community package (0_DPHX-Weather) for {simLabel} is missing or incomplete.")
        messageBuilder.AppendLine()
        messageBuilder.AppendLine("Community folder:")
        messageBuilder.AppendLine(communityFolder)
        messageBuilder.AppendLine()
        messageBuilder.AppendLine("Missing items:")
        For Each item As String In missingItems
            messageBuilder.AppendLine($"- {item}")
        Next
        messageBuilder.AppendLine()
        messageBuilder.AppendLine("Choose Yes to Create/Repair now, No to change folder, or Cancel to exit.")

        Dim response As DialogResult
        Using New Centered_MessageBox(owner)
            response = MessageBox.Show(owner,
                                       messageBuilder.ToString(),
                                       "DPHX Community Package Required",
                                       MessageBoxButtons.YesNoCancel,
                                       MessageBoxIcon.Warning)
        End Using

        Select Case response
            Case DialogResult.Yes
                If CreateOrRepairPackage(simLabel, communityFolder) Then
                    Return PackageEnsureResult.Ready
                End If
                Return PackageEnsureResult.Failed
            Case DialogResult.No
                Return PackageEnsureResult.NeedsFolderChange
            Case Else
                Return PackageEnsureResult.Cancelled
        End Select
    End Function

    Private Function IsPackageComplete(communityFolder As String,
                                       ByRef missingItems As List(Of String)) As Boolean
        Dim packageRoot = Path.Combine(communityFolder, PackageFolderName)
        Dim manifestPath = Path.Combine(packageRoot, ManifestFileName)
        Dim layoutPath = Path.Combine(packageRoot, LayoutFileName)
        Dim weatherPresetsPath = Path.Combine(packageRoot, WeatherPresetsFolderName)
        Dim thumbnailPath = Path.Combine(packageRoot, ContentInfoFolderName, ThumbnailFileName)

        missingItems = New List(Of String)

        If Not Directory.Exists(packageRoot) Then
            missingItems.Add(PackageFolderName)
        End If
        If Not Directory.Exists(weatherPresetsPath) Then
            missingItems.Add($"{PackageFolderName}\{WeatherPresetsFolderName}")
        End If
        If Not File.Exists(manifestPath) OrElse Not IsValidJsonFile(manifestPath) Then
            missingItems.Add($"{PackageFolderName}\{ManifestFileName}")
        End If
        If Not File.Exists(layoutPath) OrElse Not IsValidLayoutFile(layoutPath, thumbnailPath) Then
            missingItems.Add($"{PackageFolderName}\{LayoutFileName}")
        End If
        If Not File.Exists(thumbnailPath) Then
            missingItems.Add($"{PackageFolderName}\{ContentInfoFolderName}\{ThumbnailFileName}")
        End If

        Return missingItems.Count = 0
    End Function

    Private Function CreateOrRepairPackage(simLabel As String, communityFolder As String) As Boolean
        Dim packageRoot = Path.Combine(communityFolder, PackageFolderName)
        Dim weatherPresetsPath = Path.Combine(packageRoot, WeatherPresetsFolderName)
        Dim contentInfoPath = Path.Combine(packageRoot, ContentInfoFolderName)
        Dim manifestPath = Path.Combine(packageRoot, ManifestFileName)
        Dim layoutPath = Path.Combine(packageRoot, LayoutFileName)
        Dim thumbnailTarget = Path.Combine(contentInfoPath, ThumbnailFileName)

        Try
            If Directory.Exists(packageRoot) Then
                Directory.Delete(packageRoot, recursive:=True)
                LogPackageEvent($"{simLabel}: Existing package removed at {packageRoot}.")
            End If

            Directory.CreateDirectory(packageRoot)
            Directory.CreateDirectory(weatherPresetsPath)
            Directory.CreateDirectory(contentInfoPath)

            If Not WriteThumbnail(thumbnailTarget) Then
                Return False
            End If

            Dim manifest = BuildManifest()
            WriteJsonFile(manifestPath, manifest)
            LogPackageEvent($"{simLabel}: manifest.json generated at {manifestPath}")

            Dim layout = BuildLayout(thumbnailTarget)
            WriteJsonFile(layoutPath, layout)
            LogPackageEvent($"{simLabel}: layout.json generated at {layoutPath}")

            LogPackageEvent($"{simLabel}: DPHX Community package ensured at {packageRoot}.")
            Return True
        Catch ex As UnauthorizedAccessException
            LogPackageEvent($"{simLabel}: Access denied while creating package: {ex.Message}", TraceEventType.Error)
            ShowWriteFailureMessage()
            Return False
        Catch ex As Exception
            LogPackageEvent($"{simLabel}: Error creating package: {ex.Message}", TraceEventType.Error)
            ShowWriteFailureMessage(ex.Message)
            Return False
        End Try
    End Function

    Private Function WriteThumbnail(targetPath As String) As Boolean
        Dim sourcePath = Path.Combine(Application.StartupPath, ThumbnailSourceName)
        If Not File.Exists(sourcePath) Then
            LogPackageEvent($"Thumbnail source missing: {sourcePath}", TraceEventType.Error)
            ShowWriteFailureMessage($"Missing thumbnail source file: {sourcePath}")
            Return False
        End If

        Dim bytes = File.ReadAllBytes(sourcePath)
        WriteBytesAtomically(targetPath, bytes)
        LogPackageEvent($"Thumbnail written to {targetPath}")
        Return True
    End Function

    Private Function BuildManifest() As JObject
        Dim version = Assembly.GetExecutingAssembly().GetName().Version.ToString()

        Dim manifest = New JObject From {
            {"dependencies", New JArray()},
            {"content_type", "WEATHER"},
            {"title", "DPHX – Dynamic Weather Preset"},
            {"manufacturer", "WeSimGlide"},
            {"creator", "MajorDad"},
            {"package_version", version},
            {"minimum_game_version", MinimumGameVersion},
            {"minimum_compatibility_version", MinimumCompatibilityVersion},
            {"export_type", "Community"},
            {"builder", "Microsoft Flight Simulator 2024"},
            {"package_order_hint", "CUSTOM_SIMOBJECTS"}
        }

        Return manifest
    End Function

    Private Function BuildLayout(thumbnailPath As String) As JObject
        Dim contentItems As New JArray()
        If File.Exists(thumbnailPath) Then
            Dim info As New FileInfo(thumbnailPath)
            contentItems.Add(New JObject From {
                {"path", $"{ContentInfoFolderName}/{ThumbnailFileName}"},
                {"size", info.Length},
                {"date", DateTime.UtcNow.ToFileTimeUtc()}
            })
        End If

        Return New JObject From {
            {"content", contentItems}
        }
    End Function

    Private Function IsValidJsonFile(filePath As String) As Boolean
        Try
            Dim content = File.ReadAllText(filePath)
            Dim token = JToken.Parse(content)
            Return TypeOf token Is JObject
        Catch
            Return False
        End Try
    End Function

    Private Function IsValidLayoutFile(filePath As String, thumbnailPath As String) As Boolean
        Try
            Dim content = File.ReadAllText(filePath)
            Dim token = JToken.Parse(content)
            Dim layout = TryCast(token, JObject)
            If layout Is Nothing Then
                Return False
            End If

            Dim contentArray = TryCast(layout("content"), JArray)
            If contentArray Is Nothing Then
                Return False
            End If

            Dim thumbnailSize As Long = 0
            If File.Exists(thumbnailPath) Then
                thumbnailSize = New FileInfo(thumbnailPath).Length
            End If

            For Each entry As JObject In contentArray.OfType(Of JObject)()
                Dim pathValue = entry.Value(Of String)("path")
                If String.Equals(pathValue, $"{ContentInfoFolderName}/{ThumbnailFileName}", StringComparison.OrdinalIgnoreCase) Then
                    Dim sizeValue = entry.Value(Of Long?)("size")
                    If sizeValue.HasValue AndAlso sizeValue.Value = thumbnailSize Then
                        Return True
                    End If
                End If
            Next

            Return False
        Catch
            Return False
        End Try
    End Function

    Private Sub WriteJsonFile(targetPath As String, json As JObject)
        Dim content = json.ToString(Newtonsoft.Json.Formatting.Indented)
        WriteTextAtomically(targetPath, content)
    End Sub

    Private Sub WriteTextAtomically(targetPath As String, content As String)
        Dim tempPath = $"{targetPath}.tmp"
        File.WriteAllText(tempPath, content, Encoding.UTF8)
        ReplaceFile(tempPath, targetPath)
    End Sub

    Private Sub WriteBytesAtomically(targetPath As String, data As Byte())
        Dim tempPath = $"{targetPath}.tmp"
        File.WriteAllBytes(tempPath, data)
        ReplaceFile(tempPath, targetPath)
    End Sub

    Private Sub ReplaceFile(tempPath As String, targetPath As String)
        If File.Exists(targetPath) Then
            File.Replace(tempPath, targetPath, Nothing)
        Else
            File.Move(tempPath, targetPath)
        End If
    End Sub

    Private Sub ShowWriteFailureMessage(Optional details As String = Nothing)
        Dim message = "Cannot write to Community folder; choose a different folder or run with appropriate permissions."
        If Not String.IsNullOrWhiteSpace(details) Then
            message &= $"{Environment.NewLine}{Environment.NewLine}{details}"
        End If

        MessageBox.Show(message, "DPHX Community Package", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Private Sub LogPackageEvent(message As String, Optional severity As TraceEventType = TraceEventType.Information)
        My.Application.Log.WriteEntry(message, severity)
    End Sub

End Module
