Imports System.IO
Imports CefSharp
Imports HtmlAgilityPack
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Net.Http.Headers

Public Class WSGBatchUpload

    Private taskCache As New Dictionary(Of String, Integer)
    Private uploadHandler As UploadFileDialogHandler
    Private igcFiles As List(Of String)
    Private currentIdx As Integer
    Private igcDetails As IGCLookupDetails = Nothing
    Private tempFolder As String = Nothing

    Public Property ForcedTaskId As Integer = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        uploadHandler = New UploadFileDialogHandler()
        browser.DialogHandler = uploadHandler
        browser.Load("https://xp-soaring.github.io/tasks/b21_task_planner/index.html")

        Dim exeFolder = Path.GetDirectoryName(Application.ExecutablePath)
        tempFolder = Path.Combine(exeFolder, "IGCTemp")

        'Let's go!
        igcFiles = IO.Directory.GetFiles(tempFolder, "*.igc").ToList()

        ' Clear the multiline TextBox instead of ListBox
        txtLog.Clear()

        If igcFiles.Count = 0 Then
            txtLog.AppendText("No .igc files found." & Environment.NewLine)
            Return
        End If

        txtLog.AppendText($"Found {igcFiles.Count} files. Starting…{Environment.NewLine}")
        currentIdx = 0
        ProcessNextFileAsync()


    End Sub

    Private Async Sub ProcessNextFileAsync()

        lblProgress.Text = $"{currentIdx} / {igcFiles.Count}"

        If currentIdx >= igcFiles.Count Then
            txtLog.AppendText("✅ All done." & Environment.NewLine)
            txtLog.AppendText("✅ Clearing folder!" & Environment.NewLine)
            'Clear the folder
            If Directory.Exists(tempFolder) Then
                Directory.Delete(tempFolder, recursive:=True)
            End If
            Directory.CreateDirectory(tempFolder)

            'Call the PHP script that reassigns the IGC records to a proper user if possible
            Await CallSetUnassignedIGCRecordUserAsync()

            Return
        End If

        igcDetails = New IGCLookupDetails

        igcDetails.IGCLocalFilePath = igcFiles(currentIdx)
        txtLog.AppendText($"Uploading: {IO.Path.GetFileName(igcDetails.IGCLocalFilePath)}{Environment.NewLine}")

        ' 1) Parse everything out of the .igc
        Dim doc = IgcParser.ParseIgcFile(igcDetails.IGCLocalFilePath)
        If doc Is Nothing Then
            txtLog.AppendText("❌ Error parsing IGC file." & Environment.NewLine)
            currentIdx += 1
            ProcessNextFileAsync()
            Return
        End If

        Dim root = doc.RootElement

        igcDetails.TaskTitle = root.GetProperty("igcTitle").GetString()
        igcDetails.Pilot = root.GetProperty("pilot").GetString()
        igcDetails.GliderID = root.GetProperty("gliderID").GetString()
        igcDetails.CompetitionID = root.GetProperty("competitionID").GetString()
        igcDetails.CompetitionClass = root.GetProperty("competitionClass").GetString()
        igcDetails.GliderType = root.GetProperty("gliderType").GetString()
        igcDetails.NB21Version = root.GetProperty("NB21Version").GetString()
        igcDetails.Sim = root.GetProperty("Sim").GetString()

        igcDetails.IGCRecordDateTimeUTC = root.GetProperty("IGCRecordDateTimeUTC").GetString()
        igcDetails.IGCUploadDateTimeUTC = root.GetProperty("IGCUploadDateTimeUTC").GetString()
        igcDetails.LocalDate = root.GetProperty("LocalDate").GetString()
        igcDetails.LocalTime = root.GetProperty("LocalTime").GetString()
        igcDetails.BeginTimeUTC = root.GetProperty("BeginTimeUTC").GetString()

        ' 2) Waypoints
        igcDetails.IGCWaypoints = root.GetProperty("igcWaypoints") _
        .EnumerateArray() _
        .Select(Function(el) New IGCWaypoint With {
            .Id = el.GetProperty("id").GetString(),
            .Coord = el.GetProperty("coord").GetString()
        }) _
        .ToList()

        igcDetails.EntrySeqID = Await GetOrFetchEntrySeqID()

        If igcDetails.EntrySeqID = 0 Then
            txtLog.AppendText($"❌ No matching task for IGC: {Path.GetFileName(igcDetails.IGCLocalFilePath)}. Skipping." & vbCrLf)
            currentIdx += 1
            ProcessNextFileAsync()
            Return
        End If

        txtLog.AppendText($"✔ Found EntrySeqID {igcDetails.EntrySeqID} for this IGC." & vbCrLf)

        'Check if already uploaded
        Dim alreadyUploaded As Boolean = Await ParseIGCFileAndCheckIfAlreadyUploaded()

        If Not alreadyUploaded Then
            ' 1) Tell our handler which file to feed in
            uploadHandler.FileToUpload = igcDetails.IGCLocalFilePath

            ' 2) Fire the JS “Choose file(s)” click
            Await ClickElementByIdAsync("drop_zone_choose_button")

            ' 3) Wait for the file input to be populated
            Dim gotFile = Await WaitForConditionAsync(
      "document.getElementById('drop_zone_choose_input').files.length > 0",
      timeoutSeconds:=5
    )
            If Not gotFile Then
                txtLog.AppendText("  ❌ File never made it into the input!" & Environment.NewLine)
                Return
            End If

            ' 4) Now wait for at least one tracklogs entry
            Dim hasRows = Await WaitForConditionAsync("document.querySelectorAll('#tracklogs_table tr.tracklogs_entry_current').length > 0", timeoutSeconds:=10)
            If Not hasRows Then
                txtLog.AppendText("  ❌ No tracklogs entries detected!" & Environment.NewLine)
                Return
            End If

            ' 5) Click the first tracklog
            Await ClickElementAsync("#tracklogs_table tr.tracklogs_entry_current")

            ' 6) Click Load Task
            Await ClickElementAsync("#tracklog_info_load_task")

            ' 7) Switch back to the Tracklogs tab (the Load Task button never hides)
            Await browser.EvaluateScriptAsync("b21_task_planner.tab_tracklogs_click()")

            ' 8) Wait for the Tracklogs panel to be visible again
            Dim tabVisible = Await WaitForConditionAsync(
      "document.getElementById('tracklogs').style.display != 'none'",
      timeoutSeconds:=5)

            If Not tabVisible Then
                txtLog.AppendText("  ❌ Tracklogs tab never re-appeared!" & Environment.NewLine)
                Return
            End If

            ' 9) Extract IGCDetails
            Await ExtractResults()

            ' 10) Click Reset
            Await browser.EvaluateScriptAsync("b21_task_planner.reset_all_button()")

        End If

        ' 11) Move on to the next file
        currentIdx += 1
        ProcessNextFileAsync()

    End Sub

#Region "Browser Related Stuff"

    ''' <summary>
    ''' Parses the tracklogs table and planner version,  
    ''' builds the IGCDetails object & writes its JSON to txtLog.
    ''' </summary>
    Private Async Function ExtractResults() As Task(Of Boolean)
        ' 1) Get the raw tracklogs HTML
        Dim res = Await browser.EvaluateScriptAsync(
      "document.querySelector('#tracklogs_table')?.outerHTML"
    )
        If Not res.Success OrElse res.Result Is Nothing Then
            txtLog.AppendText("⚠ Could not read tracklogs HTML." & vbCrLf)
            Return False
        End If
        Dim tracklogsHtml As String = res.Result.ToString()

        ' 2) Load it into HtmlAgilityPack
        Dim doc As New HtmlDocument()
        doc.LoadHtml(tracklogsHtml)

        ' 3) Find the “current” row (fallback to any row if needed)
        Dim row As HtmlAgilityPack.HtmlNode =
    doc.DocumentNode.SelectSingleNode("//tr[contains(@class,'tracklogs_entry_current')]")

        If row Is Nothing Then
            row = doc.DocumentNode.SelectSingleNode("//tr[contains(@class,'tracklogs_entry')]")
        End If

        If row Is Nothing Then
            txtLog.AppendText("❌ No tracklogs row found in parsed HTML." & vbCrLf)
            Return False
        End If

        ' 4) Within that row, locate the info cell
        Dim infoTd = row.SelectSingleNode(
      ".//td[contains(@class,'tracklogs_entry_info')]")
        If infoTd Is Nothing Then
            txtLog.AppendText("❌ Could not find info cell." & vbCrLf)
            Return False
        End If

        ' 5) Pull out the name div for IGCValid & raw text
        Dim nameDiv = infoTd.SelectSingleNode(
      ".//div[contains(@class,'tracklogs_entry_name')]")
        Dim rawName = If(nameDiv?.InnerText.Trim(), "")
        Dim igcValid = rawName.StartsWith("🔒")

        ' 6) Pull out the result‐status div
        Dim resultDiv = nameDiv.SelectSingleNode(
      ".//div[contains(@class,'tracklogs_entry_finished')]")
        Dim cssClass = If(resultDiv?.GetAttributeValue("class", ""), "")
        Dim taskCompleted = cssClass.Contains("tracklogs_entry_finished_ok")
        Dim penalties = cssClass.Contains("penalties")

        ' 7) Extract the “XXh YYkm ZZkph” (or similar) text
        Dim span = resultDiv.SelectSingleNode(".//span")
        Dim resultText = If(span?.InnerText.Trim(), "")

        ' 8) Parse duration / distance / speed
        Dim duration As String = Nothing
        Dim distance As String = Nothing
        Dim speed As String = Nothing

        If taskCompleted Then
            ' split on whitespace
            Dim parts = resultText.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length >= 3 AndAlso parts(1).Contains("km") Then
                duration = parts(0)
                distance = parts(1).Replace("km", "")
                speed = parts(2).Replace("kph", "")
            ElseIf parts.Length >= 2 Then
                duration = parts(0)
                speed = parts(1).Replace("kph", "")
            End If
        Else
            ' incomplete: only distance is shown
            Dim m = System.Text.RegularExpressions.Regex.Match(resultText, "([\d\.]+)km")
            If m.Success Then distance = m.Groups(1).Value
        End If

        ' 9) Fetch the planner version
        Dim verRes = Await browser.EvaluateScriptAsync(
      "document.querySelector('#b21_task_planner_version')?.innerText"
    )
        Dim plannerVersion = If(verRes.Success, verRes.Result?.ToString(), "")

        ' 10) Build the anonymous result object
        Dim parsed = New With {
      .TaskCompleted = taskCompleted,
      .Penalties = penalties,
      .Duration = duration,
      .Distance = distance,
      .Speed = speed,
      .IGCValid = igcValid,
      .TPVersion = plannerVersion
    }

        ' 11) Serialize to JSON (indented) and write to txtLog
        Dim opts = New JsonSerializerOptions() With {
      .WriteIndented = True,
      .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    }
        Dim json = JsonSerializer.Serialize(parsed, opts)
        txtLog.AppendText("🎯 Parsed Results JSON:" & vbCrLf)
        txtLog.AppendText(json & vbCrLf & vbCrLf)

        ' We submit only completed tasks
        If parsed.TaskCompleted Then
            ' compute duration in seconds
            Dim durSec As Integer = 0
            If Not String.IsNullOrEmpty(parsed.Duration) Then
                Dim p = parsed.Duration.Split(":"c)
                If p.Length = 3 Then
                    durSec = CInt(p(0)) * 3600 + CInt(p(1)) * 60 + CInt(p(2))
                End If
            End If

            Return Await SaveIgcRecordForBatchAsync(
                            igcDetails:=igcDetails,
                            taskCompleted:=parsed.TaskCompleted,
                            penalties:=parsed.Penalties,
                            durationSeconds:=durSec,
                            distance:=parsed.Distance,
                            speed:=parsed.Speed,
                            igcValid:=parsed.IGCValid,
                            tpVersion:=parsed.TPVersion
                        )
        Else
            txtLog.AppendText("❌ Incomplete task, skipping." & vbCrLf)
        End If


        Return True
    End Function


    ''' <summary>
    ''' Polls a JS expression until it returns true or the timeout elapses.
    ''' </summary>
    Private Async Function WaitForConditionAsync(expr As String,
                                            Optional timeoutSeconds As Integer = 15) _
                                            As Task(Of Boolean)

        Dim sw As Stopwatch = Stopwatch.StartNew()
        Do
            Dim resp = Await browser.EvaluateScriptAsync(expr)
            If resp.Success Then
                If Convert.ToBoolean(resp.Result) Then
                    Return True
                End If
            Else
                txtLog.AppendText($"    ⚠ JS error on '{expr}': {resp.Message}{Environment.NewLine}")
                Return False
            End If

            Await Task.Delay(500)
        Loop While sw.Elapsed.TotalSeconds < timeoutSeconds

        txtLog.AppendText($"    ❌ Timeout waiting for: {expr}{Environment.NewLine}")
        Return False
    End Function

    ''' <summary>
    ''' Finds the center of the element with the given id and sends
    ''' a real mouse‐down / up to that point.
    ''' </summary>
    Private Async Function ClickElementByIdAsync(id As String) As Task
        ' 1) Get the element’s bounding rect from JS
        Dim rectScript = $"
      (function() {{
        var e = document.getElementById('{id}');
        if (!e) return null;
        var r = e.getBoundingClientRect();
        return {{ x: r.left + r.width/2, y: r.top + r.height/2 }};
      }})();
    "
        Dim rectResp = Await browser.EvaluateScriptAsync(rectScript)
        If Not rectResp.Success OrElse rectResp.Result Is Nothing Then
            txtLog.AppendText($"⚠ Could not locate element #{id}{Environment.NewLine}")
            Return
        End If

        ' 2) Extract coordinates
        Dim dict = DirectCast(rectResp.Result, IDictionary(Of String, Object))
        Dim x = Convert.ToDouble(dict("x"))
        Dim y = Convert.ToDouble(dict("y"))

        ' 3) Send a real mouse‐down + mouse‐up
        Dim host = browser.GetBrowser().GetHost()

        ' Mouse down with no modifiers:
        host.SendMouseClickEvent(CInt(x), CInt(y), MouseButtonType.Left, False, 1, CefEventFlags.None)
        ' Mouse up:
        host.SendMouseClickEvent(CInt(x), CInt(y), MouseButtonType.Left, True, 1, CefEventFlags.None)

    End Function

    ''' <summary>
    ''' Finds the center of the first element matching the CSS selector
    ''' and sends a real mouse‐down/up so CEF treats it as a genuine click.
    ''' </summary>
    Private Async Function ClickElementAsync(selector As String) As Task
        ' 1) Grab bounding rect
        Dim rectScript = $"
      (function(){{
        var e = document.querySelector('{selector}');
        if(!e) return null;
        var r = e.getBoundingClientRect();
        return {{ x:r.left + r.width/2, y:r.top + r.height/2 }};
      }})();
    "
        Dim rectResp = Await browser.EvaluateScriptAsync(rectScript)
        If Not rectResp.Success OrElse rectResp.Result Is Nothing Then
            txtLog.AppendText($"⚠ Could not locate element '{selector}'{Environment.NewLine}")
            Return
        End If

        Dim dict = DirectCast(rectResp.Result, IDictionary(Of String, Object))
        Dim x = Convert.ToDouble(dict("x"))
        Dim y = Convert.ToDouble(dict("y"))

        ' 2) Send a real mouse click
        Dim host = browser.GetBrowser().GetHost()
        Dim mevt As New MouseEvent(CInt(x), CInt(y), CefEventFlags.None)
        host.SendMouseClickEvent(mevt, MouseButtonType.Left, False, 1)
        host.SendMouseClickEvent(mevt, MouseButtonType.Left, True, 1)

    End Function

#End Region

    ''' <summary>
    ''' Reads all the raw C-records from an IGC file.
    ''' </summary>
    Private Function ReadCRecords(filePath As String) As List(Of String)
        Return File.ReadAllLines(filePath) _
               .Where(Function(l) l.StartsWith("C")) _
               .Select(Function(l) l.Trim()) _
               .ToList()
    End Function

    ''' <summary>
    ''' Discards records before first “*” and after last “*”.
    ''' Includes both the first/last starred lines themselves.
    ''' </summary>
    Private Function FilterBetweenMarkers(records As List(Of String)) As List(Of String)
        Dim firstStar = records.FindIndex(Function(r) r.Contains("*"))
        Dim lastStar = records.FindLastIndex(Function(r) r.Contains("*"))

        If firstStar < 0 OrElse lastStar < 0 OrElse lastStar < firstStar Then
            ' no markers → return all
            Return New List(Of String)(records)
        End If

        Return records.Skip(firstStar) _
                  .Take(lastStar - firstStar + 1) _
                  .ToList()
    End Function

    ''' <summary>
    ''' Joins the filtered records into a single cache-cacheKey string.
    ''' We use “|” as a delimiter to avoid accidental collisions.
    ''' </summary>
    Private Function BuildRecordCacheKey(filtered As List(Of String)) As String
        Return String.Join("|", filtered)
    End Function

    ''' <summary>
    ''' Full pipeline: read raw C-records, filter them, build the cacheKey.
    ''' </summary>
    Private Function ExtractIgcRecordCacheKey(filePath As String) As String
        Dim raw = ReadCRecords(filePath)
        Dim filtered = FilterBetweenMarkers(raw)
        Return BuildRecordCacheKey(filtered)
    End Function

    ''' <summary>
    ''' Checks our in-memory cache for this IGC file’s cacheKey.
    ''' If missing, calls the PHP search and stores the result.
    ''' </summary>
    Private Async Function GetOrFetchEntrySeqID() As Task(Of Integer)
        Dim cacheKey = ExtractIgcRecordCacheKey(igcDetails.IGCLocalFilePath)

        If taskCache.ContainsKey(cacheKey) Then
            Return taskCache(cacheKey)
        Else
            ' Cache miss → call your async PHP search
            Dim entrySeqID As Integer = Await IGCFlightPlanMatchedTaskID()

            ' Store the result (0 = not found, too)
            taskCache.Add(cacheKey, entrySeqID)
            Return entrySeqID
        End If
    End Function

    Private Async Function IGCFlightPlanMatchedTaskID() As Task(Of Integer)

        Dim entrySeqID As Integer = 0

        ' 3) Call MatchIGCToTask.php to get EntrySeqID
        Dim taskTitle = igcDetails.TaskTitle

        ' build the JSON array of { id, coord } from your IGCWaypoints
        Dim wpObjects = igcDetails.IGCWaypoints _
                        .Select(Function(wp) New With {
                            Key .id = wp.Id,
                            Key .coord = wp.Coord
                        }) _
                        .ToList()

        Dim igcWaypointsJson = JsonSerializer.Serialize(wpObjects)
        Dim form = New Dictionary(Of String, String) From {
                        {"igcTitle", taskTitle},
                        {"igcWaypoints", igcWaypointsJson}
                    }

        Using client As New HttpClient()
            client.BaseAddress = New Uri("https://siglr.com/DiscordPostHelper/")
            Dim resp = Await client.PostAsync("MatchIGCToTask.php", New FormUrlEncodedContent(form))
            If Not resp.IsSuccessStatusCode Then
                txtLog.AppendText($"❌ PHP error {(CInt(resp.StatusCode))} {resp.ReasonPhrase}" & vbCrLf)
                Return entrySeqID
            End If
            Dim body = Await resp.Content.ReadAsStringAsync()
            Dim doc = JsonDocument.Parse(body)
            Dim status = doc.RootElement.GetProperty("status").GetString()
            If status <> "found" Then
                txtLog.AppendText($"❌ No match (status={status})" & vbCrLf)
                Return entrySeqID
            End If
            entrySeqID = doc.RootElement.GetProperty("EntrySeqID").GetInt32()
            txtLog.AppendText($"✔ Matched EntrySeqID {entrySeqID}" & vbCrLf)
        End Using

        Return entrySeqID

    End Function

    Private Async Function ParseIGCFileAndCheckIfAlreadyUploaded() As Task(Of Boolean)

        igcDetails.AlreadyUploaded = True

        ' 6) Call CheckIgcUploaded.php
        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri("https://siglr.com/DiscordPostHelper/")
                Dim chkForm = New FormUrlEncodedContent(
                New Dictionary(Of String, String) From {
                    {"IGCKey", igcDetails.IGCKeyFilename},
                    {"EntrySeqID", igcDetails.EntrySeqID.ToString()}
                }
            )
                Dim chkResp = Await client.PostAsync("CheckIgcUploaded.php", chkForm)
                If Not chkResp.IsSuccessStatusCode Then
                    txtLog.AppendText(
                  $"❌ Check-upload PHP error {(CInt(chkResp.StatusCode))} {chkResp.ReasonPhrase}" & vbCrLf)
                    Return igcDetails.AlreadyUploaded
                End If
                Dim chkBody = Await chkResp.Content.ReadAsStringAsync()
                Dim chkDoc = JsonDocument.Parse(chkBody)
                Dim chkStatus = chkDoc.RootElement.GetProperty("status").GetString()
                If chkStatus = "exists" Then
                    txtLog.AppendText($"⚠ IGCKey already uploaded. Skipping." & vbCrLf)
                    Return igcDetails.AlreadyUploaded
                End If
            End Using
        Catch ex As Exception
            txtLog.AppendText($"❌ Error checking upload: {ex.Message}" & vbCrLf)
            Return igcDetails.AlreadyUploaded
        End Try

        ' 7) Not found, proceed
        igcDetails.AlreadyUploaded = False
        Return igcDetails.AlreadyUploaded

    End Function

    ''' <summary>
    ''' Sends all of your IGC metadata, parsed‐results flags & metrics, 
    ''' plus the .igc file itself, to SaveIGCRecordForBatch.php.
    ''' Returns True on success, False otherwise.
    ''' </summary>
    Private Async Function SaveIgcRecordForBatchAsync(
    igcDetails As IGCLookupDetails,
    taskCompleted As Boolean,
    penalties As Boolean,
    durationSeconds As Integer,
    distance As String,
    speed As String,
    igcValid As Boolean,
    tpVersion As String
) As Task(Of Boolean)

        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri("https://siglr.com/DiscordPostHelper/")

                Using content As New MultipartFormDataContent()
                    ' Helper to avoid passing Nothing into StringContent
                    Dim safe As Func(Of String, String) = Function(s) If(s, String.Empty)

                    ' --- IGC metadata (never Nothing) ---
                    content.Add(New StringContent(safe(igcDetails.IGCKeyFilename)), "IGCKey")
                    content.Add(New StringContent(igcDetails.EntrySeqID.ToString()), "EntrySeqID")
                    content.Add(New StringContent(safe(igcDetails.IGCRecordDateTimeUTC)), "IGCRecordDateTimeUTC")
                    content.Add(New StringContent(safe(igcDetails.IGCUploadDateTimeUTC)), "IGCUploadDateTimeUTC")
                    content.Add(New StringContent(safe(igcDetails.LocalDate)), "LocalDate")
                    content.Add(New StringContent(safe(igcDetails.LocalTime)), "LocalTime")
                    content.Add(New StringContent(safe(igcDetails.BeginTimeUTC)), "BeginTimeUTC")
                    content.Add(New StringContent(safe(igcDetails.Pilot)), "Pilot")
                    content.Add(New StringContent(safe(igcDetails.GliderType)), "GliderType")
                    content.Add(New StringContent(safe(igcDetails.GliderID)), "GliderID")
                    content.Add(New StringContent(safe(igcDetails.CompetitionID)), "CompetitionID")
                    content.Add(New StringContent(safe(igcDetails.CompetitionClass)), "CompetitionClass")
                    content.Add(New StringContent(safe(igcDetails.NB21Version)), "NB21Version")
                    content.Add(New StringContent(safe(igcDetails.Sim)), "Sim")
                    content.Add(New StringContent("0"), "WSGUserID")

                    ' --- parsed results ---
                    content.Add(New StringContent(If(taskCompleted, "1", "0")), "TaskCompleted")
                    content.Add(New StringContent(If(penalties, "1", "0")), "Penalties")
                    content.Add(New StringContent(durationSeconds.ToString()), "Duration")
                    content.Add(New StringContent(safe(distance)), "Distance")
                    content.Add(New StringContent(safe(speed)), "Speed")
                    content.Add(New StringContent(If(igcValid, "1", "0")), "IGCValid")
                    content.Add(New StringContent(safe(tpVersion)), "TPVersion")

                    ' --- attach the .igc file ---
                    Using fs = File.OpenRead(igcDetails.IGCLocalFilePath)
                        Dim fileContent = New StreamContent(fs)
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream")
                        content.Add(fileContent, "igcFile", Path.GetFileName(igcDetails.IGCLocalFilePath))

                        ' --- POST it ---
                        Dim resp = Await client.PostAsync("SaveIGCRecordForBatch.php", content)
                        If Not resp.IsSuccessStatusCode Then
                            txtLog.AppendText($"❌ Save PHP error {(CInt(resp.StatusCode))} {resp.ReasonPhrase}{vbCrLf}")
                            Return False
                        End If

                        ' parse JSON response
                        Dim body = Await resp.Content.ReadAsStringAsync()
                        Dim doc = JsonDocument.Parse(body)
                        Dim status = doc.RootElement.GetProperty("status").GetString()

                        If status = "success" Then
                            txtLog.AppendText($"✔ Saved IGCKey {igcDetails.IGCKeyFilename}{vbCrLf}")
                            Return True
                        Else
                            Dim msg = ""
                            If doc.RootElement.TryGetProperty("message", Nothing) Then
                                msg = doc.RootElement.GetProperty("message").GetString()
                            End If
                            txtLog.AppendText($"❌ Save failed: {msg}{vbCrLf}")
                            Return False
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            txtLog.AppendText($"❌ HTTP error saving IGC: {ex.Message}{vbCrLf}")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Calls the admin script to reassign any IGCRecords with WSGUserID=0.
    ''' </summary>
    Private Async Function CallSetUnassignedIGCRecordUserAsync() As Task
        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri("https://siglr.com/DiscordPostHelper/")
                ' If your script accepts POST instead, swap to PostAsync with an empty FormUrlEncodedContent.
                Dim resp = Await client.GetAsync("adm_SetUnassignedIGCRecordUser.php")
                Dim body = Await resp.Content.ReadAsStringAsync()

                If resp.IsSuccessStatusCode Then
                    txtLog.AppendText($"✔ Reassigned unassigned IGC records: {body}{Environment.NewLine}")
                Else
                    txtLog.AppendText($"❌ Error reassigning IGC records: HTTP {CInt(resp.StatusCode)} – {resp.ReasonPhrase}{Environment.NewLine}")
                    txtLog.AppendText(body & Environment.NewLine)
                End If
            End Using
        Catch ex As Exception
            txtLog.AppendText($"❌ Exception calling adm_SetUnassignedIGCRecordUser.php: {ex.Message}{Environment.NewLine}")
        End Try
    End Function

End Class
