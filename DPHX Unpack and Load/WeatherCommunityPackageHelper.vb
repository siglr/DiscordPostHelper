Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

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
    Private Const ThumbnailSourceName As String = "Thumbnail.jpg"
    Private Const ThumbnailFileName As String = "Thumbnail.jpg"
    Private Const ThumbnailLayoutSize As Long = 100045
    Private Const ThumbnailLayoutDate As Long = 134104361600000000
    Private Const PackageVersion As String = "2026.01.21"
    Private Const MinimumGameVersionMsfs2024 As String = "1.5.27"
    Private Const MinimumCompatibilityVersionMsfs2024 As String = "5.27.0.112"
    Private Const MinimumGameVersionMsfs2020 As String = "1.37.18"
    Private Const MinimumCompatibilityVersionMsfs2020 As String = "3.20.0.00"

    Friend Function GetDphxWeatherPackageRoot(isMsfs2020 As Boolean) As String
        Dim communityFolder = If(isMsfs2020, Settings.SessionSettings.MSFS2020WeatherPresetsFolder, Settings.SessionSettings.MSFS2024WeatherPresetsFolder)
        If String.IsNullOrWhiteSpace(communityFolder) Then
            Return String.Empty
        End If

        Return Path.Combine(communityFolder, PackageFolderName)
    End Function

    Friend Function GetDphxWeatherPresetsDir(isMsfs2020 As Boolean) As String
        Dim packageRoot = GetDphxWeatherPackageRoot(isMsfs2020)
        If String.IsNullOrWhiteSpace(packageRoot) Then
            Return String.Empty
        End If

        Return Path.Combine(packageRoot, WeatherPresetsFolderName)
    End Function

    Friend Function GetDphxWeatherLayoutPath(isMsfs2020 As Boolean) As String
        Dim packageRoot = GetDphxWeatherPackageRoot(isMsfs2020)
        If String.IsNullOrWhiteSpace(packageRoot) Then
            Return String.Empty
        End If

        Return Path.Combine(packageRoot, LayoutFileName)
    End Function

    Friend Function GetDphxWeatherThumbnailPath(isMsfs2020 As Boolean) As String
        Dim packageRoot = GetDphxWeatherPackageRoot(isMsfs2020)
        If String.IsNullOrWhiteSpace(packageRoot) Then
            Return String.Empty
        End If

        Return Path.Combine(packageRoot, ContentInfoFolderName, PackageFolderName, ThumbnailFileName)
    End Function

    Friend Function BuildWeatherPresetLayoutPath(fileName As String) As String
        If String.IsNullOrWhiteSpace(fileName) Then
            Return String.Empty
        End If

        Return NormalizeLayoutPath($"{WeatherPresetsFolderName}/{fileName}")
    End Function

    Friend Function LoadLayout(layoutPath As String, Optional thumbnailPath As String = Nothing) As JObject
        If Not String.IsNullOrWhiteSpace(layoutPath) AndAlso File.Exists(layoutPath) Then
            Try
                Dim content = File.ReadAllText(layoutPath)
                Dim token = JToken.Parse(content)
                Dim layout = TryCast(token, JObject)
                If layout IsNot Nothing Then
                    EnsureContentArray(layout)
                    If Not String.IsNullOrWhiteSpace(thumbnailPath) Then
                        EnsureThumbnailEntry(layout, thumbnailPath)
                    End If
                    Return layout
                End If
            Catch
            End Try
        End If

        Dim emptyLayout As New JObject From {
            {"content", New JArray()}
        }

        If Not String.IsNullOrWhiteSpace(thumbnailPath) Then
            EnsureThumbnailEntry(emptyLayout, thumbnailPath)
        End If

        Return emptyLayout
    End Function

    Friend Sub SaveLayout(layoutPath As String, layout As JObject)
        If String.IsNullOrWhiteSpace(layoutPath) OrElse layout Is Nothing Then
            Exit Sub
        End If

        WriteJsonFile(layoutPath, layout)
    End Sub

    Friend Function LayoutHasEntry(layout As JObject, relativePath As String) As Boolean
        If layout Is Nothing Then
            Return False
        End If

        Dim normalized = NormalizeLayoutPath(relativePath)
        If String.IsNullOrWhiteSpace(normalized) Then
            Return False
        End If

        Dim contentArray = EnsureContentArray(layout)
        For Each entry As JObject In contentArray.OfType(Of JObject)()
            Dim pathValue = NormalizeLayoutPath(entry.Value(Of String)("path"))
            If String.Equals(pathValue, normalized, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Friend Function UpsertLayoutEntryForFile(layout As JObject, relativePath As String, fileFullPath As String) As Boolean
        If layout Is Nothing OrElse String.IsNullOrWhiteSpace(relativePath) OrElse String.IsNullOrWhiteSpace(fileFullPath) Then
            Return False
        End If

        Dim normalized = NormalizeLayoutPath(relativePath)
        Dim contentArray = EnsureContentArray(layout)
        Dim entry = contentArray.OfType(Of JObject)().
            FirstOrDefault(Function(obj) String.Equals(NormalizeLayoutPath(obj.Value(Of String)("path")),
                                                       normalized,
                                                       StringComparison.OrdinalIgnoreCase))

        Dim info As New FileInfo(fileFullPath)
        Dim sizeValue As Long = If(info.Exists, info.Length, 0)
        Dim dateValue As Long = DateTime.UtcNow.ToFileTimeUtc()

        Dim updated As Boolean = False
        If entry Is Nothing Then
            contentArray.Add(New JObject From {
                {"path", normalized},
                {"size", sizeValue},
                {"date", dateValue}
            })
            updated = True
        Else
            entry("size") = sizeValue
            entry("date") = dateValue
            updated = True
        End If

        Return updated
    End Function

    Friend Function RemoveLayoutEntry(layout As JObject, relativePath As String) As Boolean
        If layout Is Nothing OrElse String.IsNullOrWhiteSpace(relativePath) Then
            Return False
        End If

        Dim normalized = NormalizeLayoutPath(relativePath)
        Dim contentArray = EnsureContentArray(layout)

        For i As Integer = contentArray.Count - 1 To 0 Step -1
            Dim entry = TryCast(contentArray(i), JObject)
            If entry Is Nothing Then
                Continue For
            End If

            Dim pathValue = NormalizeLayoutPath(entry.Value(Of String)("path"))
            If String.Equals(pathValue, normalized, StringComparison.OrdinalIgnoreCase) Then
                contentArray.RemoveAt(i)
                Return True
            End If
        Next

        Return False
    End Function

    Friend Function SyncLayoutWithDisk(layout As JObject, presetsDir As String, Optional addMissing As Boolean = True) As Boolean
        If layout Is Nothing OrElse String.IsNullOrWhiteSpace(presetsDir) Then
            Return False
        End If

        Dim contentArray = EnsureContentArray(layout)
        Dim changed As Boolean = False

        Dim expected As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If Directory.Exists(presetsDir) Then
            For Each filePath As String In Directory.EnumerateFiles(presetsDir, "*.wpr")
                Dim fileName = Path.GetFileName(filePath)
                Dim relPath = BuildWeatherPresetLayoutPath(fileName)
                If String.IsNullOrWhiteSpace(relPath) Then
                    Continue For
                End If

                expected.Add(relPath)

                If addMissing AndAlso Not LayoutHasEntry(layout, relPath) Then
                    If UpsertLayoutEntryForFile(layout, relPath, filePath) Then
                        changed = True
                    End If
                End If
            Next
        End If

        For i As Integer = contentArray.Count - 1 To 0 Step -1
            Dim entry = TryCast(contentArray(i), JObject)
            If entry Is Nothing Then
                Continue For
            End If

            Dim pathValue = NormalizeLayoutPath(entry.Value(Of String)("path"))
            If pathValue.StartsWith($"{WeatherPresetsFolderName}/", StringComparison.OrdinalIgnoreCase) Then
                If Not expected.Contains(pathValue) Then
                    contentArray.RemoveAt(i)
                    changed = True
                End If
            End If
        Next

        Return changed
    End Function

    Friend Function EnsureWeatherCommunityPackage(simLabel As String,
                                                 communityFolder As String,
                                                 isMsfs2020 As Boolean,
                                                 owner As IWin32Window,
                                                 Optional silentCreate As Boolean = False) As PackageEnsureResult
        If String.IsNullOrWhiteSpace(communityFolder) Then
            LogPackageEvent($"{simLabel}: Community folder not configured.")
            Return PackageEnsureResult.NotConfigured
        End If

        If Not Directory.Exists(communityFolder) Then
            LogPackageEvent($"{simLabel}: Community folder does not exist: {communityFolder}", TraceEventType.Warning)
            Return PackageEnsureResult.Failed
        End If

        Dim missingItems As List(Of String) = Nothing
        If IsPackageComplete(communityFolder, isMsfs2020, missingItems) Then
            Return PackageEnsureResult.Ready
        End If

        LogPackageEvent($"{simLabel}: Missing/incomplete package items: {String.Join(", ", missingItems)}")

        If silentCreate Then
            If NeedsCommunityFolderConfirmation(simLabel, communityFolder, owner) Then
                Return PackageEnsureResult.NeedsFolderChange
            End If
            If CreateOrRepairPackage(simLabel, communityFolder, isMsfs2020) Then
                Return PackageEnsureResult.Ready
            End If
            Return PackageEnsureResult.Failed
        End If

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
        messageBuilder.AppendLine("Choose Yes to Create/Repair now in the current folder, No to change folder, or Cancel to exit.")

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
                If NeedsCommunityFolderConfirmation(simLabel, communityFolder, owner) Then
                    Return PackageEnsureResult.NeedsFolderChange
                End If
                If CreateOrRepairPackage(simLabel, communityFolder, isMsfs2020) Then
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
                                       isMsfs2020 As Boolean,
                                       ByRef missingItems As List(Of String)) As Boolean
        Dim packageRoot = Path.Combine(communityFolder, PackageFolderName)
        Dim manifestPath = Path.Combine(packageRoot, ManifestFileName)
        Dim layoutPath = Path.Combine(packageRoot, LayoutFileName)
        Dim weatherPresetsPath = Path.Combine(packageRoot, WeatherPresetsFolderName)
        Dim thumbnailPath = Path.Combine(packageRoot, ContentInfoFolderName, PackageFolderName, ThumbnailFileName)

        missingItems = New List(Of String)

        If Not Directory.Exists(packageRoot) Then
            missingItems.Add(PackageFolderName)
        End If
        If Not Directory.Exists(weatherPresetsPath) Then
            missingItems.Add($"{PackageFolderName}\{WeatherPresetsFolderName}")
        End If
        If Not File.Exists(manifestPath) Then
            missingItems.Add($"{PackageFolderName}\{ManifestFileName}")
        Else
            Dim manifestToken As JToken = Nothing
            Dim hasUtf8Bom As Boolean = False
            Try
                Dim manifestBytes = File.ReadAllBytes(manifestPath)
                hasUtf8Bom = manifestBytes.Length >= 3 AndAlso
                    manifestBytes(0) = &HEF AndAlso
                    manifestBytes(1) = &HBB AndAlso
                    manifestBytes(2) = &HBF
                Dim manifestContent = Encoding.UTF8.GetString(manifestBytes)
                manifestToken = JToken.Parse(manifestContent)
            Catch
                missingItems.Add($"{PackageFolderName}\{ManifestFileName}")
            End Try

            Dim manifestObject = TryCast(manifestToken, JObject)
            If manifestObject Is Nothing Then
                missingItems.Add($"{PackageFolderName}\{ManifestFileName}")
            Else
                Dim expectedManifest = BuildManifest(isMsfs2020)
                If hasUtf8Bom OrElse Not JToken.DeepEquals(manifestObject, expectedManifest) Then
                    WriteJsonFile(manifestPath, expectedManifest)
                    LogPackageEvent($"Manifest updated to expected content at {manifestPath}.")
                End If
            End If
        End If
        If Not File.Exists(layoutPath) OrElse Not IsValidLayoutFile(layoutPath, thumbnailPath) Then
            missingItems.Add($"{PackageFolderName}\{LayoutFileName}")
        End If
        If Not File.Exists(thumbnailPath) Then
            missingItems.Add($"{PackageFolderName}\{ContentInfoFolderName}\{PackageFolderName}\{ThumbnailFileName}")
        End If

        Return missingItems.Count = 0
    End Function

    Private Function CreateOrRepairPackage(simLabel As String, communityFolder As String, isMsfs2020 As Boolean) As Boolean
        Dim packageRoot = Path.Combine(communityFolder, PackageFolderName)
        Dim weatherPresetsPath = Path.Combine(packageRoot, WeatherPresetsFolderName)
        Dim contentInfoPath = Path.Combine(packageRoot, ContentInfoFolderName)
        Dim contentInfoPackagePath = Path.Combine(contentInfoPath, PackageFolderName)
        Dim manifestPath = Path.Combine(packageRoot, ManifestFileName)
        Dim layoutPath = Path.Combine(packageRoot, LayoutFileName)
        Dim thumbnailTarget = Path.Combine(contentInfoPackagePath, ThumbnailFileName)

        Try
            If Directory.Exists(packageRoot) Then
                Directory.Delete(packageRoot, recursive:=True)
                LogPackageEvent($"{simLabel}: Existing package removed at {packageRoot}.")
            End If

            Directory.CreateDirectory(packageRoot)
            Directory.CreateDirectory(weatherPresetsPath)
            Directory.CreateDirectory(contentInfoPath)
            Directory.CreateDirectory(contentInfoPackagePath)

            If Not WriteThumbnail(thumbnailTarget) Then
                Return False
            End If

            Dim manifest = BuildManifest(isMsfs2020)
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

    Private Function BuildManifest(isMsfs2020 As Boolean) As JObject
        Dim manifest = New JObject From {
            {"dependencies", New JArray()},
            {"content_type", "WEATHER"},
            {"title", "DPHX – Dynamic Weather Preset"},
            {"manufacturer", "WeSimGlide"},
            {"creator", "MajorDad"},
            {"package_version", PackageVersion},
            {"minimum_game_version", If(isMsfs2020, MinimumGameVersionMsfs2020, MinimumGameVersionMsfs2024)},
            {"minimum_compatibility_version", If(isMsfs2020, MinimumCompatibilityVersionMsfs2020, MinimumCompatibilityVersionMsfs2024)},
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
                {"path", $"{ContentInfoFolderName}/{PackageFolderName}/{ThumbnailFileName}"},
                {"size", ThumbnailLayoutSize},
                {"date", ThumbnailLayoutDate}
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
                If String.Equals(pathValue, $"{ContentInfoFolderName}/{PackageFolderName}/{ThumbnailFileName}", StringComparison.OrdinalIgnoreCase) Then
                    Dim sizeValue = entry.Value(Of Long?)("size")
                    Dim dateValue = entry.Value(Of Long?)("date")
                    If sizeValue.HasValue AndAlso sizeValue.Value = ThumbnailLayoutSize AndAlso
                        dateValue.HasValue AndAlso dateValue.Value = ThumbnailLayoutDate AndAlso
                        thumbnailSize = ThumbnailLayoutSize Then
                        Return True
                    End If
                End If
            Next

            Return False
        Catch
            Return False
        End Try
    End Function

    Private Function EnsureContentArray(layout As JObject) As JArray
        Dim contentArray = TryCast(layout("content"), JArray)
        If contentArray Is Nothing Then
            contentArray = New JArray()
            layout("content") = contentArray
        End If
        Return contentArray
    End Function

    Private Sub EnsureThumbnailEntry(layout As JObject, thumbnailPath As String)
        If layout Is Nothing OrElse String.IsNullOrWhiteSpace(thumbnailPath) OrElse Not File.Exists(thumbnailPath) Then
            Exit Sub
        End If

        Dim contentArray = EnsureContentArray(layout)
        Dim thumbnailEntry = contentArray.OfType(Of JObject)().
            FirstOrDefault(Function(obj) String.Equals(NormalizeLayoutPath(obj.Value(Of String)("path")),
                                                       $"{ContentInfoFolderName}/{PackageFolderName}/{ThumbnailFileName}",
                                                       StringComparison.OrdinalIgnoreCase))
        If thumbnailEntry Is Nothing Then
            contentArray.Add(New JObject From {
                {"path", $"{ContentInfoFolderName}/{PackageFolderName}/{ThumbnailFileName}"},
                {"size", ThumbnailLayoutSize},
                {"date", ThumbnailLayoutDate}
            })
        End If
    End Sub

    Private Function NormalizeLayoutPath(pathValue As String) As String
        If String.IsNullOrWhiteSpace(pathValue) Then
            Return String.Empty
        End If

        Return pathValue.Replace("\", "/")
    End Function

    Private Sub WriteJsonFile(targetPath As String, json As JObject)
        Dim content = json.ToString(Newtonsoft.Json.Formatting.Indented)
        WriteTextAtomically(targetPath, content)
    End Sub

    Private Sub WriteTextAtomically(targetPath As String, content As String)
        Dim tempPath = $"{targetPath}.tmp"
        Dim utf8WithoutBom As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
        File.WriteAllText(tempPath, content, utf8WithoutBom)
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

    Friend Function ConfirmCommunityFolderSelection(simLabel As String,
                                                    communityFolder As String,
                                                    owner As IWin32Window) As Boolean
        If String.IsNullOrWhiteSpace(communityFolder) Then
            Return True
        End If

        If Not Directory.Exists(communityFolder) Then
            Return False
        End If

        Return Not NeedsCommunityFolderConfirmation(simLabel, communityFolder, owner)
    End Function

    Private Function NeedsCommunityFolderConfirmation(simLabel As String,
                                                      communityFolder As String,
                                                      owner As IWin32Window) As Boolean
        Dim subfolders As String() = Array.Empty(Of String)()
        Try
            subfolders = Directory.GetDirectories(communityFolder)
        Catch ex As Exception
            LogPackageEvent($"{simLabel}: Unable to enumerate community folder subdirectories: {ex.Message}", TraceEventType.Warning)
            Return False
        End Try

        If subfolders.Length > 0 Then
            Return False
        End If

        Dim message = $"The selected Community folder has no subfolders, which is unusual and may indicate the wrong location.{Environment.NewLine}{Environment.NewLine}Are you sure you want to create the 0_DPHX-Weather package here?{Environment.NewLine}{Environment.NewLine}{communityFolder}"
        Dim confirmation As DialogResult
        Using New Centered_MessageBox(owner)
            confirmation = MessageBox.Show(owner,
                                           message,
                                           "Confirm Community Folder",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning)
        End Using

        Return confirmation = DialogResult.No
    End Function

End Module
