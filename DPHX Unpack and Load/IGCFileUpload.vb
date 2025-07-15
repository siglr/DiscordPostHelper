Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text.RegularExpressions
Imports CefSharp
Imports HtmlAgilityPack
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Public Class IGCFileUpload

    Private uploadHandler As UploadFileDialogHandler
    Private igcFiles As List(Of String)
    Private entrySeqID As Integer = 0
    Private plnFilePath As String = Nothing
    Private simLocalDateTime As DateTime = Nothing
    Private alreadyUploaded As Boolean = False
    Private dictIGCDetails As New Dictionary(Of String, IGCLookupDetails)
    Private igcDetails As IGCLookupDetails = Nothing

    Public Sub Display(parentForm As Form, pIGCFiles As List(Of String), pPLNFilePath As String, pEntrySeqID As Integer, pSimLocalDateTime As DateTime)
        entrySeqID = pEntrySeqID
        igcFiles = pIGCFiles
        plnFilePath = pPLNFilePath
        simLocalDateTime = pSimLocalDateTime

        uploadHandler = New UploadFileDialogHandler()
        browser.DialogHandler = uploadHandler
        browser.Load("https://xp-soaring.github.io/tasks/b21_task_planner/index.html")

        lstbxIGCFiles.Items.Clear()
        dictIGCDetails.Clear()
        Dim thisIGCDetails As IGCLookupDetails = Nothing
        If igcFiles IsNot Nothing AndAlso igcFiles.Count > 0 Then
            For Each igcFile As String In igcFiles
                thisIGCDetails = New IGCLookupDetails
                thisIGCDetails.IGCLocalFilePath = igcFile
                thisIGCDetails.EntrySeqID = entrySeqID
                dictIGCDetails.Add(thisIGCDetails.IGCFileName, thisIGCDetails)
                lstbxIGCFiles.Items.Add(thisIGCDetails.IGCFileName)
            Next
        End If
        'Subscribe for when the main frame finishes loading
        AddHandler browser.FrameLoadEnd, AddressOf OnBrowserFrameLoadEnd
        Me.Width = parentForm.Width
        Me.Height = parentForm.Height
        Me.ShowDialog(parentForm)

    End Sub

    Private Sub OnBrowserFrameLoadEnd(sender As Object, e As CefSharp.FrameLoadEndEventArgs)
        ' only once, and only for the main frame
        If Not e.Frame.IsMain Then Return

        ' we’re on CEF’s render thread—marshal back to WinForms
        Me.Invoke(Sub()
                      If lstbxIGCFiles.Items.Count > 0 Then
                          lstbxIGCFiles.SelectedIndex = 0
                      End If
                  End Sub)

        ' unsubscribe, so it only runs once
        RemoveHandler browser.FrameLoadEnd, AddressOf OnBrowserFrameLoadEnd
    End Sub

    Private Async Function ProcessSelectedIGCFile() As Task

        lblProcessing.Visible = True
        lblProcessing.Text = String.Empty

        'Parse the IGC file
        If igcDetails.IsParsed = False Then
            Await ProcessIGCFileStep1Parsing()
        End If

        'Display the IGC details
        txtPilot.Text = igcDetails.Pilot
        txtCompID.Text = igcDetails.CompetitionID
        txtGlider.Text = igcDetails.GliderType
        txtNB21.Text = igcDetails.NB21Version
        txtSim.Text = igcDetails.Sim

        Dim rawUtc = igcDetails.IGCRecordDateTimeUTC
        ' Parse into a DateTime (yyMMddHHmmss)
        Dim dtUtc As DateTime
        If DateTime.TryParseExact(rawUtc,
                                  "yyMMddHHmmss",
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.AssumeUniversal,
                                  dtUtc) Then

            ' Convert to local and format: short date + long time
            Dim dtLocal = dtUtc.ToLocalTime()
            txtRecordDate.Text = $"{dtLocal:d} {dtLocal:T}"

        Else
            ' Fallback if something’s wrong
            txtRecordDate.Text = rawUtc
        End If

        txtWSGStatus.Text = String.Empty
        txtTaskPlannerStatus.Text = String.Empty

        If Not igcDetails.AlreadyUploadedChecked Then
            'Check if already uploaded
            Dim resultCheckIGCAlreadyUploaded As String = Await CheckIGCAlreadyUploaded()
            If Not String.IsNullOrEmpty(resultCheckIGCAlreadyUploaded) Then
                'Error checking if IGC already uploaded
                txtWSGStatus.Text = $"Error: {resultCheckIGCAlreadyUploaded}"
            Else
                igcDetails.AlreadyUploadedChecked = True
            End If
        End If

        pnlResults.Visible = False

        If String.IsNullOrEmpty(txtWSGStatus.Text) Then
            If igcDetails.AlreadyUploaded Then
                txtWSGStatus.Text = "Already uploaded"
                txtTaskPlannerStatus.Text = "N/A"
                Return
            Else
                txtWSGStatus.Text = $"Can upload - WSG User: {igcDetails.WSGUserID}"
            End If
        End If

        If igcDetails.Results Is Nothing Then
            Await ExtractionFromTaskPlanner()
        Else
            txtTaskPlannerStatus.Text = "Results already available"
        End If
        If igcDetails.Results Is Nothing Then
            'Still nothing - error processing?
        Else
            Await FillResultsPanel()
        End If

    End Function

    Private Async Function FillResultsPanel() As Task

        'Fill results
        pnlResults.Visible = True
        Dim flagValid As String = "❗"
        Dim flagCompleted As String = "❌"
        Dim flagDateTime As String = "❌"
        Dim flagPenalties As String = "👮"
        If igcDetails.Results.IGCValid Then
            flagValid = "🔒"
        End If
        If igcDetails.Results.TaskCompleted Then
            flagCompleted = "🏁"
        End If
        If SupportingFeatures.LocalDateTimeMatch(igcDetails.LocalDate, igcDetails.LocalTime, simLocalDateTime) Then
            flagDateTime = "⌚"
        End If
        If igcDetails.Results.Penalties Then
            flagPenalties = "✅"
        End If
        txtFlags.Text = $"{flagValid}{flagCompleted}{flagDateTime}{flagPenalties}"
        txtLocalDateTime.Text = SupportingFeatures.FormatDateWithoutYearSecondsAndWeekday(igcDetails.LocalDate, igcDetails.LocalTime)
        txtTaskLocalDateTime.Text = SupportingFeatures.FormatDateWithoutYearSecondsAndWeekday(simLocalDateTime)

        Select Case SupportingFeatures.PrefUnits.Speed
            Case PreferredUnits.SpeedUnits.Imperial
                txtSpeed.Text = String.Format("{0:N1} mph", Conversions.KmhToMph(igcDetails.Results.Speed))
            Case PreferredUnits.SpeedUnits.Knots
                txtSpeed.Text = String.Format("{0:N1} kts", Conversions.KmhToKnots(igcDetails.Results.Speed))
            Case PreferredUnits.SpeedUnits.Metric
                txtSpeed.Text = $"{igcDetails.Results.Speed} km/h"
        End Select

        Select Case SupportingFeatures.PrefUnits.Distance
            Case PreferredUnits.DistanceUnits.Metric
                txtDistance.Text = $"{igcDetails.Results.Distance} km"
            Case PreferredUnits.DistanceUnits.Imperial
                txtDistance.Text = String.Format("{0:N1} mi", Conversions.KmToMiles(igcDetails.Results.Distance))
            Case PreferredUnits.DistanceUnits.Both
                txtDistance.Text = String.Format($"{igcDetails.Results.Distance} km / {Conversions.KmToMiles(igcDetails.Results.Distance):N1} mi")
        End Select

        txtTime.Text = igcDetails.Results.Duration
        txtTPVersion.Text = igcDetails.Results.TPVersion

    End Function

    Private Async Function ExtractionFromTaskPlanner() As Task

        Dim currentZoomLevel As Double = Await browser.GetZoomLevelAsync()
        browser.SetZoomLevel(0.0)
        browser.Enabled = False
        lblProcessing.Visible = True
        lblProcessing.Text = "Extracting results, please wait..."
        Dim resultExctractMsg As String = Await ProcessIGCFileStep2TaskPlanner()
        browser.SetZoomLevel(currentZoomLevel)
        If String.IsNullOrEmpty(resultExctractMsg) Then
            'All fine
            lblProcessing.Visible = False
            browser.Enabled = True
            txtTaskPlannerStatus.Text = "Results extracted"
        Else
            'Error
            lblProcessing.Visible = True
            txtTaskPlannerStatus.Text = resultExctractMsg
        End If

    End Function

    Private Async Function UploadIGCResults() As Task

        ' Upload the IGC results and file to the server
        Dim saveResults As String = Await SaveIgcRecordForBatchAsync()

        If String.IsNullOrEmpty(saveResults) Then

            ' Finish with the post‐upload steps
            txtWSGStatus.Text = "Already uploaded"
            txtTaskPlannerStatus.Text = "N/A"
            igcDetails.AlreadyUploaded = True
            igcDetails.Results = Nothing
            pnlResults.Visible = False
            lblProcessing.Visible = True
            lblProcessing.Text = String.Empty

            FinishUp()

        Else
            'Handle the error
            Using New Centered_MessageBox(Me)
                MessageBox.Show(saveResults, "Error uploading IGC", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End If

    End Function

    Private Async Sub FinishUp()

        'Call the PHP script that reassigns the IGC records to a proper user if possible
        Await CallSetUnassignedIGCRecordUserAsync()

        'Call the PHP script that updates the latest IGC leader records JSON file
        Await CallUpdateLatestIGCLeadersAsync()

    End Sub

    Private Async Function ProcessIGCFileStep1Parsing() As Task

        Await browser.EvaluateScriptAsync("b21_task_planner.reset_all_button()")

        'Parse everything out of the .igc
        Dim doc As JToken = IgcParser.ParseIgcFile(igcDetails.IGCLocalFilePath)
        If doc Is Nothing Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Error parsing IGC file.", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return
        End If

        Dim root As JObject = CType(doc, JObject)

        igcDetails.TaskTitle = root.Value(Of String)("igcTitle")
        igcDetails.Pilot = root.Value(Of String)("pilot")
        igcDetails.GliderID = root.Value(Of String)("gliderID")
        igcDetails.CompetitionID = root.Value(Of String)("competitionID")
        igcDetails.CompetitionClass = root.Value(Of String)("competitionClass")
        igcDetails.GliderType = root.Value(Of String)("gliderType")
        igcDetails.NB21Version = root.Value(Of String)("NB21Version")
        igcDetails.Sim = root.Value(Of String)("Sim")

        If igcDetails.NB21Version Is Nothing OrElse igcDetails.NB21Version.Trim = String.Empty Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("IGC file not coming from NB21 Logger.", "Error processing IGC file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
            Return
        End If

        igcDetails.IGCRecordDateTimeUTC = root.Value(Of String)("IGCRecordDateTimeUTC")
        igcDetails.IGCUploadDateTimeUTC = root.Value(Of String)("IGCUploadDateTimeUTC")
        igcDetails.LocalDate = root.Value(Of String)("LocalDate")
        igcDetails.LocalTime = root.Value(Of String)("LocalTime")
        igcDetails.BeginTimeUTC = root.Value(Of String)("BeginTimeUTC")

        ' Waypoints
        igcDetails.IGCWaypoints = root("igcWaypoints") _
        .OfType(Of JArray)() _
        .FirstOrDefault() _
        ?.Select(Function(el) New IGCWaypoint With {
            .Id = el.Value(Of String)("id"),
            .Coord = el.Value(Of String)("coord")
        }) _
        .ToList()

        igcDetails.IsParsed = True

    End Function

    Private Async Function ProcessIGCFileStep2TaskPlanner() As Task(Of String)

        'Load the flight plan first
        ' Tell our handler which file to feed in
        uploadHandler.FileToUpload = $"{plnFilePath}"

        ' Fire the JS “Choose file(s)” click
        Await ClickElementByIdAsync("drop_zone_choose_button")

        ' Wait for the file input to be populated
        Dim gotPlnFile = Await WaitForConditionAsync("document.getElementById('drop_zone_choose_input').files.length > 0", timeoutSeconds:=5)
        If Not gotPlnFile Then
            Return "Could not load the PLN file!"
        End If

        'Then the IGC file
        ' Tell our handler which IGC file to feed in
        uploadHandler.FileToUpload = igcDetails.IGCLocalFilePath

        ' Fire the JS “Choose file(s)” click
        Await ClickElementByIdAsync("drop_zone_choose_button")

        ' Wait for the file input to be populated
        Dim gotFile = Await WaitForConditionAsync("document.getElementById('drop_zone_choose_input').files.length > 0", timeoutSeconds:=5)
        If Not gotFile Then
            Return "Could not load the IGC file!"
        End If

        ' Now wait for at least one tracklogs entry
        Dim hasRows = Await WaitForConditionAsync("document.querySelectorAll('#tracklogs_table tr.tracklogs_entry_current').length > 0", timeoutSeconds:=10)
        If Not hasRows Then
            Return "No tracklogs entries detected!"
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
            Return "Tracklogs tab never re-appeared!"
        End If

        'Read tracklogs
        Dim res = Await browser.EvaluateScriptAsync("document.querySelector('#tracklogs_table')?.outerHTML")
        If Not res.Success OrElse res.Result Is Nothing Then
            Return "Could not read tracklogs HTML."
        End If
        Dim tracklogsHtml As String = res.Result.ToString()

        'Load it into HtmlAgilityPack
        Dim doc As New HtmlDocument()
        doc.LoadHtml(tracklogsHtml)

        'Find the “current” row (fallback to any row if needed)
        Dim row As HtmlNode = doc.DocumentNode.SelectSingleNode("//tr[contains(@class,'tracklogs_entry_current')]")
        If row Is Nothing Then
            row = doc.DocumentNode.SelectSingleNode("//tr[contains(@class,'tracklogs_entry')]")
        End If
        If row Is Nothing Then
            Return "No tracklogs row found in parsed HTML."
        End If

        'Within that row, locate the info cell
        Dim infoTd As HtmlNode = row.SelectSingleNode(".//td[contains(@class,'tracklogs_entry_info')]")
        If infoTd Is Nothing Then
            Return "Could not find info cell."
        End If

        'Pull out the name div for IGCValid & raw text
        Dim nameDiv As HtmlNode = infoTd.SelectSingleNode(".//div[contains(@class,'tracklogs_entry_name')]")
        Dim rawName As String = If(nameDiv?.InnerText.Trim(), "")
        Dim igcValid As Boolean = rawName.StartsWith("🔒")

        'Pull out the result‐status div
        Dim resultDiv As HtmlNode = nameDiv.SelectSingleNode(".//div[contains(@class,'tracklogs_entry_finished')]")
        Dim cssClass As String = If(resultDiv?.GetAttributeValue("class", ""), "")
        Dim taskCompleted As Boolean = cssClass.Contains("tracklogs_entry_finished_ok")
        Dim penalties As Boolean = cssClass.Contains("penalties")

        'Extract the “XXh YYkm ZZkph” (or similar) text
        Dim span As HtmlNode = resultDiv.SelectSingleNode(".//span")
        Dim resultText As String = If(span?.InnerText.Trim(), "")

        'Parse duration / distance / speed
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

        'Fetch the planner version
        Dim verRes As JavascriptResponse = Await browser.EvaluateScriptAsync("document.querySelector('#b21_task_planner_version')?.innerText")
        Dim plannerVersion As String = If(verRes.Success, verRes.Result?.ToString(), "")

        'Build the IGC results object
        igcDetails.Results = New IGCResults(taskCompleted, penalties, duration, distance, speed, igcValid, plannerVersion)

        Return String.Empty

    End Function

#Region "Browser Related Stuff"

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
                client.BaseAddress = New Uri(SupportingFeatures.SIGLRDiscordPostHelperFolder)

                ' Build form values, including the new PilotName and CompID
                Dim values As New Dictionary(Of String, String) From {
                {"IGCKey", igcDetails.IGCKeyFilename},
                {"EntrySeqID", igcDetails.EntrySeqID.ToString()},
                {"PilotName", igcDetails.Pilot},
                {"CompID", igcDetails.CompetitionID}
            }
                Dim chkForm As New FormUrlEncodedContent(values)

                Dim chkResp = Await client.PostAsync("CheckIgcUploaded.php", chkForm)
                If Not chkResp.IsSuccessStatusCode Then
                    Return $"{CInt(chkResp.StatusCode)} {chkResp.ReasonPhrase}"
                End If

                Dim chkBody = Await chkResp.Content.ReadAsStringAsync()
                Dim chkDoc = JToken.Parse(chkBody)
                Dim status As String = chkDoc("status")?.ToString()

                If status = "exists" Then
                    igcDetails.AlreadyUploaded = True
                    Return String.Empty
                End If

                ' Not found → read inferred WSGUserID (0 if absent)
                Dim wsgToken As JToken = chkDoc("wsgUserID")
                If wsgToken IsNot Nothing Then
                    Dim wsgID As Integer
                    If Integer.TryParse(wsgToken.ToString(), wsgID) Then
                        igcDetails.WSGUserID = wsgID
                    End If
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
    Private Async Function SaveIgcRecordForBatchAsync() As Task(Of String)

        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri(SupportingFeatures.SIGLRDiscordPostHelperFolder)

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
                    content.Add(New StringContent(safe(igcDetails.WSGUserID)), "WSGUserID")

                    ' --- parsed results ---
                    content.Add(New StringContent(If(igcDetails.Results.TaskCompleted, "1", "0")), "TaskCompleted")
                    content.Add(New StringContent(If(igcDetails.Results.Penalties, "1", "0")), "Penalties")
                    content.Add(New StringContent(igcDetails.Results.DurationInSeconds.ToString()), "Duration")
                    content.Add(New StringContent(safe(igcDetails.Results.Distance)), "Distance")
                    content.Add(New StringContent(safe(igcDetails.Results.Speed)), "Speed")
                    content.Add(New StringContent(If(igcDetails.Results.IGCValid, "1", "0")), "IGCValid")
                    content.Add(New StringContent(safe(igcDetails.Results.TPVersion)), "TPVersion")

                    ' --- attach the .igc file ---
                    Using fs = File.OpenRead(igcDetails.IGCLocalFilePath)
                        Dim fileContent = New StreamContent(fs)
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream")
                        content.Add(fileContent, "igcFile", Path.GetFileName(igcDetails.IGCLocalFilePath))

                        ' --- POST it ---
                        Dim resp = Await client.PostAsync("SaveIGCRecordForBatch.php", content)
                        If Not resp.IsSuccessStatusCode Then
                            Return $"Save PHP error {(CInt(resp.StatusCode))} {resp.ReasonPhrase}"
                        End If

                        ' parse JSON response
                        Dim body As String = Await resp.Content.ReadAsStringAsync()
                        Dim jdoc As JToken = JToken.Parse(body)
                        Dim status As String = jdoc("status")?.ToString()

                        If status = "success" Then
                            Return String.Empty
                        Else
                            Dim msg As String = ""
                            ' if there’s a "message" property, grab its text
                            If jdoc("message") IsNot Nothing Then
                                msg = jdoc("message")?.ToString()
                            End If
                            Return $"Save failed: {msg}"
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            Return ($"HTTP error saving IGC: {ex.Message}")
        End Try
    End Function

    ''' <summary>
    ''' Calls the admin script to reassign any IGCRecords with WSGUserID=0.
    ''' </summary>
    Private Async Function CallSetUnassignedIGCRecordUserAsync() As Task
        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri(SupportingFeatures.SIGLRDiscordPostHelperFolder)
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
                client.BaseAddress = New Uri($"{SupportingFeatures.WeSimGlide}php/")
                ' If your script accepts POST instead, swap to PostAsync with an empty FormUrlEncodedContent.
                Dim resp = Await client.GetAsync("UpdateLatestIGCLeaders.php")
                Dim body = Await resp.Content.ReadAsStringAsync()

            End Using
        Catch ex As Exception
        End Try
    End Function

    Private Async Sub lstbxIGCFiles_SelectedIndexChangedAsync(sender As Object, e As EventArgs) Handles lstbxIGCFiles.SelectedIndexChanged

        Try
            If lstbxIGCFiles.SelectedIndex >= 0 Then
                lstbxIGCFiles.Enabled = False
                SetCurrentIGCDetails()
                Me.Text = $"IGC File Upload - {igcDetails.IGCFileName}"
                Await ProcessSelectedIGCFile()
                lstbxIGCFiles.Enabled = True
                lstbxIGCFiles.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show($"Error processing IGC file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub SetCurrentIGCDetails()
        If lstbxIGCFiles.SelectedIndex >= 0 Then
            igcDetails = dictIGCDetails(lstbxIGCFiles.SelectedItem.ToString())
        End If
    End Sub

    Private Async Sub btnRecalculate_Click(sender As Object, e As EventArgs) Handles btnRecalculate.Click
        Try
            If lstbxIGCFiles.SelectedIndex >= 0 Then
                lstbxIGCFiles.Enabled = False
                Await browser.EvaluateScriptAsync("b21_task_planner.reset_all_button()")
                Await ExtractionFromTaskPlanner()
                Await FillResultsPanel()
                lstbxIGCFiles.Enabled = True
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Async Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click

        Await UploadIGCResults()

    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        'Ask confirmation and delete the selected IGC file from the list, from the dictionary and from the disk
        Using New Centered_MessageBox(Me)
            If MessageBox.Show("Are you sure you want to delete the selected IGC file?", "Delete IGC File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If lstbxIGCFiles.SelectedIndex >= 0 Then
                    Dim igcFileName As String = igcDetails.IGCFileName
                    File.Delete(igcDetails.IGCLocalFilePath)
                    dictIGCDetails.Remove(igcFileName)
                    igcFiles.Remove(igcFileName)
                    lstbxIGCFiles.Items.Remove(igcFileName)
                    If lstbxIGCFiles.Items.Count > 0 Then
                        lstbxIGCFiles.SelectedIndex = 0
                    Else
                        Me.Close()
                    End If
                End If
            End If
        End Using
    End Sub

End Class
