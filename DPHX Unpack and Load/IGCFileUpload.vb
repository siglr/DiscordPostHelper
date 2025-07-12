Imports System.IO
Imports CefSharp
Imports HtmlAgilityPack
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Net.Http.Headers
Imports System.Collections.Specialized
Imports System.Net
Imports SIGLR.SoaringTools.CommonLibrary

Public Class IGCFileUpload

    Private uploadHandler As UploadFileDialogHandler
    Private igcFiles As List(Of String)
    Private igcDetails As IGCLookupDetails = Nothing
    Private entrySeqID As Integer = 0
    Private plnFilePath As String = Nothing
    Private alreadyUploaded As Boolean = False
    Private igcResults As IGCResults = Nothing

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        uploadHandler = New UploadFileDialogHandler()
        browser.DialogHandler = uploadHandler
        browser.Load("https://xp-soaring.github.io/tasks/b21_task_planner/index.html")

    End Sub

    Public Sub Display(pIGCFiles As List(Of String), pPLNFilePath As String, pEntrySeqID As Integer)
        entrySeqID = pEntrySeqID
        igcFiles = pIGCFiles
        plnFilePath = pPLNFilePath

    End Sub

    Private Sub ProcessSelectedIGCFile()

        'TODO: Select the IGC file to process
        ProcessIGCFileStep1Async(Nothing)

    End Sub

    Private Async Sub UploadIGCResults()

        ' Upload the IGC results and file to the server
        If Await SaveIgcRecordForBatchAsync(
                        igcDetails,
                        igcResults.TaskCompleted,
                        igcResults.Penalties,
                        igcResults.DurationInSeconds,
                        igcResults.Distance,
                        igcResults.Speed,
                        igcResults.IGCValid,
                        igcResults.TPVersion
                        ) Then

            ' Finish with the post‐upload steps
            FinishUp()

        Else
            'TODO: Handle the error
        End If

    End Sub

    Private Async Sub FinishUp()

        'Call the PHP script that reassigns the IGC records to a proper user if possible
        Await CallSetUnassignedIGCRecordUserAsync()

        'Call the PHP script that updates the latest IGC leader records JSON file
        Await CallUpdateLatestIGCLeadersAsync()

    End Sub

    Private Async Sub ProcessIGCFileStep1Async(igcFile As String)

        Await browser.EvaluateScriptAsync("b21_task_planner.reset_all_button()")

        igcDetails = New IGCLookupDetails

        igcDetails.IGCLocalFilePath = igcFile
        igcDetails.EntrySeqID = entrySeqID

        'Parse everything out of the .igc
        Dim doc = IgcParser.ParseIgcFile(igcDetails.IGCLocalFilePath)
        If doc Is Nothing Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Error parsing IGC file.", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
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

        If igcDetails.NB21Version Is Nothing OrElse igcDetails.NB21Version.Trim = String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("IGC file not coming from NB21 Logger.", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return
        End If

        igcDetails.IGCRecordDateTimeUTC = root.GetProperty("IGCRecordDateTimeUTC").GetString()
        igcDetails.IGCUploadDateTimeUTC = root.GetProperty("IGCUploadDateTimeUTC").GetString()
        igcDetails.LocalDate = root.GetProperty("LocalDate").GetString()
        igcDetails.LocalTime = root.GetProperty("LocalTime").GetString()
        igcDetails.BeginTimeUTC = root.GetProperty("BeginTimeUTC").GetString()

        ' Waypoints
        igcDetails.IGCWaypoints = root.GetProperty("igcWaypoints") _
        .EnumerateArray() _
        .Select(Function(el) New IGCWaypoint With {
            .Id = el.GetProperty("id").GetString(),
            .Coord = el.GetProperty("coord").GetString()
        }) _
        .ToList()

    End Sub

    Private Async Sub ProcessIGCFileStep2Async()

        'Check if already uploaded
        Dim resultCheckIGCAlreadyUploaded As String = Await CheckIGCAlreadyUploaded()
        If String.IsNullOrEmpty(resultCheckIGCAlreadyUploaded) Then
            'Error checking if IGC already uploaded
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"Error checking if IGC already uploaded: {resultCheckIGCAlreadyUploaded}", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return
        End If

        If igcDetails.AlreadyUploaded Then
            ' TODO: Mark the IFC file as already uploaded

        Else
            'Load the flight plan first
            ' Tell our handler which file to feed in
            uploadHandler.FileToUpload = $"{plnFilePath}"

            ' Fire the JS “Choose file(s)” click
            Await ClickElementByIdAsync("drop_zone_choose_button")

            ' Wait for the file input to be populated
            Dim gotPlnFile = Await WaitForConditionAsync("document.getElementById('drop_zone_choose_input').files.length > 0", timeoutSeconds:=5)
            If Not gotPlnFile Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Could not load the PLN file!", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return
            End If

            'Then the IGC file
            ' Tell our handler which IGC file to feed in
            uploadHandler.FileToUpload = igcDetails.IGCLocalFilePath

            ' Fire the JS “Choose file(s)” click
            Await ClickElementByIdAsync("drop_zone_choose_button")

            ' Wait for the file input to be populated
            Dim gotFile = Await WaitForConditionAsync("document.getElementById('drop_zone_choose_input').files.length > 0", timeoutSeconds:=5)
            If Not gotFile Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Could not load the IGC file!", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return
            End If

            ' Now wait for at least one tracklogs entry
            Dim hasRows = Await WaitForConditionAsync("document.querySelectorAll('#tracklogs_table tr.tracklogs_entry_current').length > 0", timeoutSeconds:=10)
            If Not hasRows Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("No tracklogs entries detected!", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return
            End If

            ' Click the first tracklog
            Await ClickElementAsync("#tracklogs_table tr.tracklogs_entry_current")

            ' Click Load Task
            Await ClickElementAsync("#tracklog_info_load_task")

            ' Switch back to the Tracklogs tab (the Load Task button never hides)
            Await browser.EvaluateScriptAsync("b21_task_planner.tab_tracklogs_click()")

            ' Wait for the Tracklogs panel to be visible again
            Dim tabVisible = Await WaitForConditionAsync("document.getElementById('tracklogs').style.display != 'none'", timeoutSeconds:=5)

            If Not tabVisible Then
                Using New Centered_MessageBox(Me)
                    MessageBox.Show("Tracklogs tab never re-appeared!", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Using
                Return
            End If

            ' Extract IGCDetails
            Await ExtractResults()

        End If

    End Sub

#Region "Browser Related Stuff"

    ''' <summary>
    ''' Parses the tracklogs table and planner version,  
    ''' builds the IGCDetails object & writes its JSON to txtLog.
    ''' </summary>
    Private Async Function ExtractResults() As Task(Of Boolean)

        ' 1) Get the raw tracklogs HTML
        Dim res = Await browser.EvaluateScriptAsync("document.querySelector('#tracklogs_table')?.outerHTML")
        If Not res.Success OrElse res.Result Is Nothing Then
            'txtLog.AppendText("⚠ Could not read tracklogs HTML." & vbCrLf)
            Return False
        End If
        Dim tracklogsHtml As String = res.Result.ToString()

        ' 2) Load it into HtmlAgilityPack
        Dim doc As New HtmlDocument()
        doc.LoadHtml(tracklogsHtml)

        ' 3) Find the “current” row (fallback to any row if needed)
        Dim row As HtmlNode = doc.DocumentNode.SelectSingleNode("//tr[contains(@class,'tracklogs_entry_current')]")

        If row Is Nothing Then
            row = doc.DocumentNode.SelectSingleNode("//tr[contains(@class,'tracklogs_entry')]")
        End If

        If row Is Nothing Then
            'txtLog.AppendText("❌ No tracklogs row found in parsed HTML." & vbCrLf)
            Return False
        End If

        ' 4) Within that row, locate the info cell
        Dim infoTd As HtmlNode = row.SelectSingleNode(".//td[contains(@class,'tracklogs_entry_info')]")
        If infoTd Is Nothing Then
            'txtLog.AppendText("❌ Could not find info cell." & vbCrLf)
            Return False
        End If

        ' 5) Pull out the name div for IGCValid & raw text
        Dim nameDiv As HtmlNode = infoTd.SelectSingleNode(".//div[contains(@class,'tracklogs_entry_name')]")
        Dim rawName As String = If(nameDiv?.InnerText.Trim(), "")
        Dim igcValid As Boolean = rawName.StartsWith("🔒")

        ' 6) Pull out the result‐status div
        Dim resultDiv As HtmlNode = nameDiv.SelectSingleNode(".//div[contains(@class,'tracklogs_entry_finished')]")
        Dim cssClass As String = If(resultDiv?.GetAttributeValue("class", ""), "")
        Dim taskCompleted As Boolean = cssClass.Contains("tracklogs_entry_finished_ok")
        Dim penalties As Boolean = cssClass.Contains("penalties")

        ' 7) Extract the “XXh YYkm ZZkph” (or similar) text
        Dim span As HtmlNode = resultDiv.SelectSingleNode(".//span")
        Dim resultText As String = If(span?.InnerText.Trim(), "")

        ' 8) Parse duration / distance / speed
        Dim duration As String = Nothing
        Dim distance As String = Nothing
        Dim speed As String = Nothing

        If taskCompleted Then
            ' split on whitespace
            Dim parts As String() = resultText.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
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
            Dim m As Match = Regex.Match(resultText, "([\d\.]+)km")
            If m.Success Then distance = m.Groups(1).Value
        End If

        ' 9) Fetch the planner version
        Dim verRes As JavascriptResponse = Await browser.EvaluateScriptAsync("document.querySelector('#b21_task_planner_version')?.innerText")
        Dim plannerVersion As String = If(verRes.Success, verRes.Result?.ToString(), "")

        ' 10) Build the IGC results object
        igcResults = New IGCResults(taskCompleted, penalties, duration, distance, speed, igcValid, plannerVersion)

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
                'txtLog.AppendText($"    ⚠ JS error on '{expr}': {resp.Message}{Environment.NewLine}")
                Return False
            End If

            Await Task.Delay(500)
        Loop While sw.Elapsed.TotalSeconds < timeoutSeconds

        'txtLog.AppendText($"    ❌ Timeout waiting for: {expr}{Environment.NewLine}")
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
            'txtLog.AppendText($"⚠ Could not locate element #{id}{Environment.NewLine}")
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
            'txtLog.AppendText($"⚠ Could not locate element '{selector}'{Environment.NewLine}")
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

    Private Async Function CheckIGCAlreadyUploaded() As Task(Of String)

        igcDetails.AlreadyUploaded = False

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
                    Return $"Check IGC already uploaded PHP error {(CInt(chkResp.StatusCode))} {chkResp.ReasonPhrase}"
                End If
                Dim chkBody = Await chkResp.Content.ReadAsStringAsync()
                Dim chkDoc = JsonDocument.Parse(chkBody)
                Dim chkStatus = chkDoc.RootElement.GetProperty("status").GetString()
                If chkStatus = "exists" Then
                    igcDetails.AlreadyUploaded = True
                    Return String.Empty
                End If
            End Using
        Catch ex As Exception
            Return $"Error checking upload: {ex.Message}"
        End Try

        Return String.Empty

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
                            'txtLog.AppendText($"❌ Save PHP error {(CInt(resp.StatusCode))} {resp.ReasonPhrase}{vbCrLf}")
                            Return False
                        End If

                        ' parse JSON response
                        Dim body = Await resp.Content.ReadAsStringAsync()
                        Dim doc = JsonDocument.Parse(body)
                        Dim status = doc.RootElement.GetProperty("status").GetString()

                        If status = "success" Then
                            'txtLog.AppendText($"✔ Saved IGCKey {igcDetails.IGCKeyFilename}{vbCrLf}")
                            Return True
                        Else
                            Dim msg = ""
                            If doc.RootElement.TryGetProperty("message", Nothing) Then
                                msg = doc.RootElement.GetProperty("message").GetString()
                            End If
                            'txtLog.AppendText($"❌ Save failed: {msg}{vbCrLf}")
                            Return False
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            'txtLog.AppendText($"❌ HTTP error saving IGC: {ex.Message}{vbCrLf}")
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

            End Using
        Catch ex As Exception
        End Try
    End Function

    ''' <summary>
    ''' Calls the admin script to update the JSON files that contains the latest leader IGC records
    ''' </summary>
    Private Async Function CallUpdateLatestIGCLeadersAsync() As Task
        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri("https://wesimglide.org/php/")
                ' If your script accepts POST instead, swap to PostAsync with an empty FormUrlEncodedContent.
                Dim resp = Await client.GetAsync("UpdateLatestIGCLeaders.php")
                Dim body = Await resp.Content.ReadAsStringAsync()

            End Using
        Catch ex As Exception
        End Try
    End Function

End Class
