Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports CefSharp
Imports HtmlAgilityPack
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SIGLR.SoaringTools.CommonLibrary

Public Class IGCFileUpload

    Private uploadHandler As UploadFileDialogHandler
    Private igcFiles As List(Of String)
    Private currentDPHXEntrySeqID As Integer = 0
    Private alreadyUploaded As Boolean = False
    Private dictIGCDetails As New Dictionary(Of String, IGCLookupDetails)
    Private igcDetails As IGCLookupDetails = Nothing
    Private taskCache As New Dictionary(Of String, IGCCacheTaskObject)
    Private _tabpgRatingsVisited As Boolean = False

    Public Sub Display(parentForm As Form, pIGCFiles As List(Of String), pEntrySeqID As Integer)

        Rescale()

        currentDPHXEntrySeqID = pEntrySeqID
        igcFiles = pIGCFiles

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
                dictIGCDetails.Add(thisIGCDetails.IGCFileName, thisIGCDetails)
                lstbxIGCFiles.Items.Add(thisIGCDetails.IGCFileName)
            Next
        End If
        'Subscribe for when the main frame finishes loading
        AddHandler browser.FrameLoadEnd, AddressOf OnBrowserFrameLoadEnd
        Me.Width = parentForm.Width
        Me.Height = parentForm.Height
        SupportingFeatures.CenterFormOnOwner(parentForm, Me)
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
        lblProcessing.Text = String.Empty
        pnlResults.Visible = False
        tabIGCTabs.SelectedTab = tabpgResults
        grpIGCUserComment.Enabled = False
        grpTaskUserData.Enabled = False

        If igcDetails.CacheKey Is Nothing OrElse igcDetails.CacheKey = String.Empty Then
            'CacheKey is not set, so look for the EntrySeqID for this IGC file
            Dim matchErr As String = Await GetOrFetchEntrySeqID()
        End If
        If (igcDetails.MatchedTask Is Nothing) OrElse igcDetails.MatchedTask.EntrySeqID = 0 Then
            'No task matched!
            txtWSGStatus.Text = "No task found!"
            txtIGCEntrySeqID.Text = "Task not found"
            lblProcessing.Text = "No task found!"
            Return
        Else
            'Check if the task is the same as the current DPHX task
            If (igcDetails.MatchedTask.EntrySeqID <> currentDPHXEntrySeqID) And currentDPHXEntrySeqID > 0 Then
                'Set a different color for the EntrySeqID - red if different
                txtIGCEntrySeqID.Text = $"{igcDetails.MatchedTask.EntrySeqID.ToString} <> current DPHX"
            Else
                'Set a different color for the EntrySeqID - WindowText if same
                txtIGCEntrySeqID.Text = igcDetails.MatchedTask.EntrySeqID.ToString
            End If
        End If

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

        If String.IsNullOrEmpty(txtWSGStatus.Text) Then
            If igcDetails.AlreadyUploaded Then
                txtWSGStatus.Text = "Already uploaded"
                lblProcessing.Text = "Already uploaded"
                Return
            Else
                Dim userInfo As String = String.Empty
                If igcDetails.WSGUserID = Settings.SessionSettings.WSGUserID Then
                    userInfo = $"You ({Settings.SessionSettings.WSGPilotName})"
                    igcDetails.IsOwnedByCurrentUser = True
                Else
                    userInfo = $"Not you ({igcDetails.WSGUserID})"
                    igcDetails.IsOwnedByCurrentUser = False
                End If
                txtWSGStatus.Text = $"Can upload - WSG User: {userInfo}"
            End If
        End If

        If igcDetails.Results Is Nothing Then
            Await ExtractionFromTaskPlanner()
        Else
            lblProcessing.Text = "Results already available"
        End If
        If igcDetails.Results Is Nothing Then
            'Still nothing - error processing?
        Else
            Await FillResultsPanel()
        End If

    End Function

    Private Async Function FillResultsPanel() As Task

        If SupportingFeatures.PrefUnits Is Nothing Then
            SupportingFeatures.PrefUnits = New PreferredUnits
        End If

        'Fill results
        pnlResults.Visible = True
        tabIGCTabs.SelectedTab = tabpgResults
        grpIGCUserComment.Enabled = True AndAlso igcDetails.IsOwnedByCurrentUser
        grpTaskUserData.Enabled = True AndAlso igcDetails.IsOwnedByCurrentUser
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
        If SupportingFeatures.LocalDateTimeMatch(igcDetails.LocalDate, igcDetails.LocalTime, igcDetails.MatchedTask.MSFSLocalDateTime) Then
            flagDateTime = "⌚"
        End If
        If Not igcDetails.Results.Penalties Then
            flagPenalties = "✅"
        End If
        txtFlags.Text = $"{flagValid}{flagCompleted}{flagDateTime}{flagPenalties}"
        txtLocalDateTime.Text = SupportingFeatures.FormatDateWithoutYearSecondsAndWeekday(igcDetails.LocalDate, igcDetails.LocalTime)
        txtTaskLocalDateTime.Text = SupportingFeatures.FormatDateWithoutYearSecondsAndWeekday(igcDetails.MatchedTask.MSFSLocalDateTime)

        If igcDetails.Results.Speed Is Nothing Then
            txtSpeed.Text = String.Empty
        Else
            Select Case SupportingFeatures.PrefUnits.Speed
                Case PreferredUnits.SpeedUnits.Imperial
                    txtSpeed.Text = String.Format("{0:N1} mph", Conversions.KmhToMph(igcDetails.Results.Speed))
                Case PreferredUnits.SpeedUnits.Knots
                    txtSpeed.Text = String.Format("{0:N1} kts", Conversions.KmhToKnots(igcDetails.Results.Speed))
                Case PreferredUnits.SpeedUnits.Metric
                    txtSpeed.Text = $"{igcDetails.Results.Speed} km/h"
            End Select
        End If

        If igcDetails.Results.Distance Is Nothing Then
            txtDistance.Text = String.Empty
        Else
            Select Case SupportingFeatures.PrefUnits.Distance
                Case PreferredUnits.DistanceUnits.Metric
                    txtDistance.Text = $"{igcDetails.Results.Distance} km"
                Case PreferredUnits.DistanceUnits.Imperial
                    txtDistance.Text = String.Format("{0:N1} mi", Conversions.KmToMiles(igcDetails.Results.Distance))
                Case PreferredUnits.DistanceUnits.Both
                    txtDistance.Text = String.Format($"{igcDetails.Results.Distance} km / {Conversions.KmToMiles(igcDetails.Results.Distance):N1} mi")
            End Select
        End If

        txtTime.Text = igcDetails.Results.Duration

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
            lblProcessing.Text = "Results extracted"
        Else
            'Error
            lblProcessing.Visible = True
            lblProcessing.Text = resultExctractMsg
        End If

    End Function

    Private Async Function UploadIGCResults() As Task

        ' Upload the IGC results and file to the server
        Dim saveResults As String = Await SaveIgcRecordForBatchAsync()

        If String.IsNullOrEmpty(saveResults) Then

            ' Finish with the post‐upload steps
            txtWSGStatus.Text = "Uploaded"
            lblProcessing.Text = "Uploaded"
            igcDetails.AlreadyUploaded = True
            igcDetails.Results = Nothing
            pnlResults.Visible = False
            tabIGCTabs.SelectedTab = tabpgResults
            grpIGCUserComment.Enabled = False
            grpTaskUserData.Enabled = False
            lblProcessing.Visible = True

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

        'Reset the planner and make sure the units are set to metric
        Await ResetTaskPlanner()

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
        igcDetails.IGCWaypoints = New List(Of IGCWaypoint)()

        Dim wps = TryCast(root("igcWaypoints"), JArray)
        If wps IsNot Nothing Then
            For Each item As JObject In wps
                Dim wp As New IGCWaypoint With {
            .Id = item("id")?.ToString(),
            .Coord = item("coord")?.ToString()
        }
                igcDetails.IGCWaypoints.Add(wp)
            Next
        End If

        igcDetails.IsParsed = True

    End Function

    Private Async Function ProcessIGCFileStep2TaskPlanner() As Task(Of String)

        'Load the flight plan first - we need to save the content from igcDetails.PLNXML to disk in a temporary location first
        ' 1) Build a temp .pln filename based on the EntrySeqID
        Dim plnFileName = $"{igcDetails.MatchedTask.EntrySeqID}.pln"
        Dim tempPlnPath = Path.Combine(Path.GetTempPath(), plnFileName)

        ' 2) Write out the PLNXML only if it isn’t already on disk
        If Not File.Exists(tempPlnPath) Then
            File.WriteAllText(tempPlnPath, igcDetails.MatchedTask.PLNXML, Encoding.UTF8)
        End If

        ' 3) Point the upload handler at that file
        uploadHandler.FileToUpload = tempPlnPath

        ' 4) Fire the JS click to open the file dialog
        Await ClickElementByIdAsync("drop_zone_choose_button")

        ' Wait for the file input to be populated
        Dim gotPlnFile = Await WaitForConditionAsync("document.getElementById('drop_zone_choose_input').files.length > 0", timeoutSeconds:=5)
        ' Cleanup: delete the temp file once the click has been issued
        If File.Exists(tempPlnPath) Then
            Try
                File.Delete(tempPlnPath)
            Catch
                ' ignore any delete errors
            End Try
        End If
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
            Dim m As Match = Regex.Match(resultText.Trim(), "([\d\.]+)\s*km", RegexOptions.IgnoreCase)
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

    ''' <summary>
    ''' Finds the first <button> whose visible text matches exactly the given text
    ''' and sends a real mouse click to it.
    ''' </summary>
    Private Async Function ClickButtonByTextAsync(buttonText As String) As Task
    Dim rectScript = $"
      (function(){{
        var buttons = document.querySelectorAll('button');
        for (var i=0;i<buttons.length;i++) {{
          if (buttons[i].textContent.trim() === '{buttonText}') {{
            var r = buttons[i].getBoundingClientRect();
            return {{ x:r.left + r.width/2, y:r.top + r.height/2 }};
          }}
        }}
        return null;
      }})();
    "

    Dim rectResp = Await browser.EvaluateScriptAsync(rectScript)
    If Not rectResp.Success OrElse rectResp.Result Is Nothing Then Return

    Dim dict = DirectCast(rectResp.Result, IDictionary(Of String, Object))
    Dim x = Convert.ToDouble(dict("x"))
    Dim y = Convert.ToDouble(dict("y"))

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
                {"EntrySeqID", igcDetails.MatchedTask.EntrySeqID.ToString()},
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
                client.DefaultRequestHeaders.ExpectContinue = False

                Using content As New MultipartFormDataContent()
                    ' Helper to avoid passing Nothing into StringContent
                    Dim safe As Func(Of String, String) = Function(s) If(s, String.Empty)

                    ' --- IGC metadata (never Nothing) ---
                    content.Add(New StringContent(safe(igcDetails.IGCKeyFilename)), "IGCKey")
                    content.Add(New StringContent(igcDetails.MatchedTask.EntrySeqID.ToString()), "EntrySeqID")
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
                    content.Add(New StringContent(safe(If(String.IsNullOrWhiteSpace(igcDetails.IGCUserComment), "", igcDetails.IGCUserComment))), "IGCUserComment")

                    ' --- parsed results ---
                    content.Add(New StringContent(If(igcDetails.Results.TaskCompleted, "1", "0")), "TaskCompleted")
                    content.Add(New StringContent(If(igcDetails.Results.Penalties, "1", "0")), "Penalties")
                    content.Add(New StringContent(igcDetails.Results.DurationInSeconds.ToString()), "Duration")
                    content.Add(New StringContent(safe(igcDetails.Results.Distance)), "Distance")
                    content.Add(New StringContent(safe(igcDetails.Results.Speed)), "Speed")
                    content.Add(New StringContent(If(igcDetails.Results.IGCValid, "1", "0")), "IGCValid")
                    content.Add(New StringContent(safe(igcDetails.Results.TPVersion)), "TPVersion")

                    ' --- UsersTasks (only meaningful if user is known) ---
                    Dim userIdVal As Integer
                    Integer.TryParse(igcDetails.WSGUserID, userIdVal)

                    If userIdVal > 0 Then
                        content.Add(New StringContent("1"), "UT_InfoFetched")
                        content.Add(New StringContent(If(String.IsNullOrWhiteSpace(igcDetails.UT_MarkedFlyNextUTC), "", igcDetails.UT_MarkedFlyNextUTC)), "UT_MarkedFlyNextUTC")
                        content.Add(New StringContent(If(String.IsNullOrWhiteSpace(igcDetails.UT_MarkedFavoritesUTC), "", igcDetails.UT_MarkedFavoritesUTC)), "UT_MarkedFavoritesUTC")
                        content.Add(New StringContent(If(igcDetails.UT_DifficultyRating > 0, igcDetails.UT_DifficultyRating.ToString(), "0")), "UT_DifficultyRating")
                        content.Add(New StringContent(If(igcDetails.UT_QualityRating > 0, igcDetails.UT_QualityRating.ToString(), "0")), "UT_QualityRating")
                        content.Add(New StringContent(safe(If(String.IsNullOrWhiteSpace(igcDetails.UT_PrivateNote), "", igcDetails.UT_PrivateNote))), "UT_PrivateNote")
                        content.Add(New StringContent(safe(If(String.IsNullOrWhiteSpace(igcDetails.UT_PublicNote), "", igcDetails.UT_PublicNote))), "UT_PublicNote")
                    Else
                        content.Add(New StringContent("0"), "UT_InfoFetched")
                    End If

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
                _tabpgRatingsVisited = False
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

    Private Async Function ResetTaskPlanner() As Task
        Dim currentZoomLevel As Double = Await browser.GetZoomLevelAsync()
        browser.SetZoomLevel(0.0)
        Await browser.EvaluateScriptAsync("b21_task_planner.reset_all_button()")
        Await ClickElementAsync("button[title=""Choose units for distance, elevation etc.""]")
        Await ClickElementByIdAsync("setting_speed_units_kph")
        Await ClickElementByIdAsync("setting_distance_units_km")
        Await ClickButtonByTextAsync("Close Settings")
        browser.SetZoomLevel(currentZoomLevel)
    End Function

    Private Async Sub btnRecalculate_Click(sender As Object, e As EventArgs) Handles btnRecalculate.Click
        Try
            If lstbxIGCFiles.SelectedIndex >= 0 Then
                lstbxIGCFiles.Enabled = False
                Await ResetTaskPlanner()
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

        'If nothing's been entered in the Ratings & Comments tab, ask the user to confirm
        If igcDetails.IsOwnedByCurrentUser AndAlso
           (txtUserIGCComment.Text.Trim = String.Empty AndAlso Not _tabpgRatingsVisited) OrElse
           (txtTaskPublicFeedback.Text.Trim = String.Empty AndAlso
           txtTaskPrivateNotes.Text.Trim = String.Empty AndAlso
           Not chkFlyNext.Checked AndAlso
           Not chkFavorites.Checked AndAlso
           cboDifficulty.SelectedIndex = 0 AndAlso
           cboQuality.SelectedIndex = 0 AndAlso Not _tabpgRatingsVisited) Then
            Using New Centered_MessageBox(Me)
                tabIGCTabs.SelectTab(tabpgRatings.TabIndex)
                If MessageBox.Show("You haven't entered any comments or ratings. Are you sure you want to proceed with the upload?", "No Comments or Ratings", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Return
                End If
            End Using
        End If
        Await UploadIGCResults()

    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click

        'Ask confirmation and delete the selected IGC file from the list, from the dictionary and from the disk
        Using New Centered_MessageBox(Me)
            If MessageBox.Show($"Are you sure you want to delete the selected IGC file?{Environment.NewLine}WARNING: THIS IS PERMANENT!", "Delete IGC File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
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

    Private Sub btnMoveIGCToProcessed_Click(sender As Object, e As EventArgs) Handles btnMoveIGCToProcessed.Click
        ' Ask confirmation and “move” the selected IGC file into a subfolder "Processed"
        If lstbxIGCFiles.SelectedIndex < 0 Then
            Return
        End If

        ' Grab file name & path
        Dim igcFileName = igcDetails.IGCFileName
        Dim sourcePath = igcDetails.IGCLocalFilePath

        ' Ensure the "Processed" subfolder exists
        Dim processedFolder = Path.Combine(
            Path.GetDirectoryName(sourcePath),
            "Processed"
        )
        If Not Directory.Exists(processedFolder) Then
            Directory.CreateDirectory(processedFolder)
        End If

        ' Compute destination path and move
        Dim destPath = Path.Combine(processedFolder, $"{igcFileName}.igc")
        File.Move(sourcePath, destPath)

        ' Remove from our collections and UI (same as delete)
        dictIGCDetails.Remove(igcFileName)
        igcFiles.Remove(igcFileName)
        lstbxIGCFiles.Items.Remove(igcFileName)

        ' Select next or close
        If lstbxIGCFiles.Items.Count > 0 Then
            lstbxIGCFiles.SelectedIndex = 0
        Else
            Me.Close()
        End If
    End Sub

    ''' <summary>
    ''' Reads all C-lines, filters between first/last “*”, then joins with “|”
    ''' to form a stable cache key.
    ''' </summary>
    Private Function ExtractIgcRecordCacheKey(filePath As String) As String
        Dim raw = File.ReadAllLines(filePath) _
                   .Where(Function(l) l.StartsWith("C"c)) _
                   .Select(Function(l) l.Trim()) _
                   .ToList()

        ' strip before/after the * markers
        Dim firstStar = raw.FindIndex(Function(r) r.Contains("*"))
        Dim lastStar = raw.FindLastIndex(Function(r) r.Contains("*"))
        If firstStar < 0 OrElse lastStar < 0 OrElse lastStar < firstStar Then
            Return String.Join("|", raw)
        End If
        Dim slice = raw.Skip(firstStar).Take(lastStar - firstStar + 1)
        Return String.Join("|", slice)
    End Function

    Private Async Function FetchEntrySeqIDFromServer() As Task(Of String)

        Dim response As String = String.Empty

        Try

            Dim form = New Dictionary(Of String, String) From {
              {"igcTitle", igcDetails.TaskTitle},
              {"igcWaypoints", JsonConvert.SerializeObject(
                                   igcDetails.IGCWaypoints.Select(
                                     Function(wp) New With {wp.Id, wp.Coord}
                                   ).ToList()
                                 )}
          }
            Using client As New HttpClient()
                client.BaseAddress = New Uri(SupportingFeatures.SIGLRDiscordPostHelperFolder)
                Dim resp = Await client.PostAsync("MatchIGCToTask.php",
                                                New FormUrlEncodedContent(form))

                If resp.IsSuccessStatusCode Then
                    Dim body = Await resp.Content.ReadAsStringAsync()
                    Dim j = JObject.Parse(body)
                    If j("status")?.ToString() = "found" Then
                        ' Found a match, extract the EntrySeqID and store it into a new IGCCacheTaskObject and then in the current igcDetails
                        Dim entrySeqID As Integer = 0
                        Dim plnXML As String = String.Empty
                        Dim simDateTime As DateTime = DateTime.MinValue

                        Integer.TryParse(j("EntrySeqID")?.ToString(), entrySeqID)
                        plnXML = j("PLNXML")?.ToString()
                        Dim simDTstr = j("SimDateTime")?.ToString()
                        Dim taskTitle = j("Title")?.ToString()
                        If Not String.IsNullOrEmpty(simDTstr) Then
                            DateTime.TryParseExact(simDTstr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, simDateTime)
                        End If
                        igcDetails.MatchedTask = New IGCCacheTaskObject(entrySeqID, plnXML, simDateTime, taskTitle)
                    End If
                End If
            End Using
        Catch ex As Exception
            response = $"Error fetching EntrySeqID: {ex.Message}"
        End Try

        Return response

    End Function

    ''' <summary>
    ''' Returns either the forced ID, the cached ID, or—on cache miss—the
    ''' server-matched ID (and stores it in cache).
    ''' </summary>
    Private Async Function GetOrFetchEntrySeqID() As Task(Of String)

        Dim response As String = String.Empty

        ' Compute cache key
        igcDetails.CacheKey = ExtractIgcRecordCacheKey(igcDetails.IGCLocalFilePath)

        ' Cache hit?
        Dim matchedTask As IGCCacheTaskObject = Nothing
        If taskCache.TryGetValue(igcDetails.CacheKey, matchedTask) Then
            ' Cache hit → The EntrySeqID was set by the cache lookup
            igcDetails.MatchedTask = matchedTask
        Else
            ' Cache miss → server lookup
            response = Await FetchEntrySeqIDFromServer()
            If String.IsNullOrEmpty(response) Then
                taskCache.Add(igcDetails.CacheKey, igcDetails.MatchedTask)
            End If
        End If

        Return response

    End Function

    Private Sub MakeAvatarCircular(pb As PictureBox)
        Dim gp As New GraphicsPath()
        gp.AddEllipse(0, 0, pb.Width - 1, pb.Height - 1)
        pb.Region = New Region(gp)
    End Sub

    Private Sub tabIGCTabs_Selected(sender As Object, e As TabControlEventArgs) Handles tabIGCTabs.Selected
        If tabIGCTabs.SelectedTab Is tabpgRatings Then
            ' Reset all fields to default values
            cboDifficulty.SelectedIndex = 0
            cboQuality.SelectedIndex = 0
            txtUserIGCComment.Text = String.Empty
            txtTaskPrivateNotes.Text = String.Empty
            txtTaskPublicFeedback.Text = String.Empty
            chkFavorites.Checked = False
            chkFlyNext.Checked = False
            lblFlyNextDateTime.Text = String.Empty
            lblFavoritesDateTime.Text = String.Empty
            lblTaskIDAndTitle.Text = String.Empty
            _tabpgRatingsVisited = True

            ' WSG User info
            lblCompID.Text = Settings.SessionSettings.WSGCompID
            lblPilotName.Text = Settings.SessionSettings.WSGPilotName
            lblDisplayName.Text = Settings.SessionSettings.WSGDisplayName
            lblUserID.Text = Settings.SessionSettings.WSGUserID
            Dim url As String = Settings.SessionSettings.WSGAvatar
            If Not String.IsNullOrWhiteSpace(url) Then
                Using wc As New WebClient()
                    Using ms As New MemoryStream(wc.DownloadData(url))
                        imgAvatar.Image = Image.FromStream(ms)
                        MakeAvatarCircular(imgAvatar)
                    End Using
                End Using
            Else
                imgAvatar.Image = Nothing
            End If

            If grpTaskUserData.Enabled Then
                If Not igcDetails.UT_InfoFetched Then
                    ' If the User/Task info have not been retrieved, fetch them.
                    igcDetails.UT_InfoFetched = GetUsersTask(igcDetails.WSGUserID, igcDetails.MatchedTask.EntrySeqID)
                End If
                lblTaskIDAndTitle.Text = $"{igcDetails.MatchedTask.TaskTitle} (#{igcDetails.MatchedTask.EntrySeqID.ToString()})"
                If igcDetails.UT_MarkedFavoritesUTC = String.Empty Then
                    lblFavoritesDateTime.Text = String.Empty
                    chkFavorites.Checked = False
                Else
                    lblFavoritesDateTime.Text = SupportingFeatures.FormatUserDateTime(igcDetails.UT_MarkedFavoritesUTC, True)
                    chkFavorites.Checked = True
                End If
                If igcDetails.UT_MarkedFlyNextUTC = String.Empty Then
                    lblFlyNextDateTime.Text = String.Empty
                    chkFlyNext.Checked = False
                Else
                    lblFlyNextDateTime.Text = SupportingFeatures.FormatUserDateTime(igcDetails.UT_MarkedFlyNextUTC, True)
                    chkFlyNext.Checked = True
                End If
                cboDifficulty.SelectedIndex = igcDetails.UT_DifficultyRating
                cboQuality.SelectedIndex = igcDetails.UT_QualityRating
                txtUserIGCComment.Text = igcDetails.IGCUserComment
                txtTaskPrivateNotes.Text = igcDetails.UT_PrivateNote
                txtTaskPublicFeedback.Text = igcDetails.UT_PublicNote

            End If

        End If
    End Sub

    Private Function GetUsersTask(wsgUserId As Integer, entrySeqID As Integer) As Boolean
        Try
            Dim usersTaskUrl As String =
            $"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}GetUserTaskInfo.php?WSGUserID={wsgUserId}&EntrySeqID={entrySeqID}"

            Dim request As HttpWebRequest = CType(WebRequest.Create(usersTaskUrl), HttpWebRequest)
            request.Method = "GET"
            request.ContentType = "application/json"

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    Dim jsonResponse As String = reader.ReadToEnd()
                    Dim result As JObject = JObject.Parse(jsonResponse)

                    If result("status").ToString() = "success" Then
                        Dim usersTask As JObject = result("usersTask")

                        ' Example: pull fields from the JSON
                        Dim privateNotes As String = usersTask.Value(Of String)("PrivateNotes")
                        Dim tags As String = usersTask.Value(Of String)("Tags")
                        Dim difficulty As Integer = usersTask.Value(Of Integer?)("DifficultyRating").GetValueOrDefault(0)
                        Dim quality As Integer = usersTask.Value(Of Integer?)("QualityRating").GetValueOrDefault(0)
                        Dim flyNextDate As String = usersTask.Value(Of String)("MarkedFlyNextUTC")
                        Dim favoritesDate As String = usersTask.Value(Of String)("MarkedFavoritesUTC")
                        Dim publicFeedback As String = usersTask.Value(Of String)("PublicFeedback")

                        igcDetails.UT_DifficultyRating = difficulty
                        igcDetails.UT_QualityRating = quality
                        igcDetails.UT_MarkedFlyNextUTC = flyNextDate
                        igcDetails.UT_MarkedFavoritesUTC = favoritesDate
                        igcDetails.UT_PrivateNote = privateNotes
                        igcDetails.UT_PublicNote = publicFeedback

                        Return True

                    ElseIf result("message").ToString().Contains("not found") Then
                        ' Normal case: record does not exist
                        Return False
                    Else
                        ' Unexpected error message → treat as exception
                        Throw New Exception("Error retrieving user task: " & result("message").ToString())
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Only unexpected issues (connection, parse, etc.) end up here
            Throw New Exception("Error: " & ex.Message)
        End Try
    End Function

    Private Sub chkFlyNext_CheckedChanged(sender As Object, e As EventArgs) Handles chkFlyNext.CheckedChanged
        If chkFlyNext.Checked Then
            If igcDetails.UT_MarkedFlyNextUTC = String.Empty Then
                igcDetails.UT_MarkedFlyNextUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            End If
        Else
            igcDetails.UT_MarkedFlyNextUTC = String.Empty
        End If
        lblFlyNextDateTime.Text = If(chkFlyNext.Checked, SupportingFeatures.FormatUserDateTime(igcDetails.UT_MarkedFlyNextUTC, True), "")
    End Sub

    Private Sub chkFavorites_CheckedChanged(sender As Object, e As EventArgs) Handles chkFavorites.CheckedChanged
        If chkFavorites.Checked Then
            If igcDetails.UT_MarkedFavoritesUTC = String.Empty Then
                igcDetails.UT_MarkedFavoritesUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            End If
        Else
            igcDetails.UT_MarkedFavoritesUTC = String.Empty
        End If
        lblFavoritesDateTime.Text = If(chkFavorites.Checked, SupportingFeatures.FormatUserDateTime(igcDetails.UT_MarkedFavoritesUTC, True), "")
    End Sub

    Private Sub txtUserIGCComment_Leave(sender As Object, e As EventArgs) Handles txtUserIGCComment.Leave
        igcDetails.IGCUserComment = txtUserIGCComment.Text.Trim()
    End Sub

    Private Sub txtTaskPublicFeedback_Leave(sender As Object, e As EventArgs) Handles txtTaskPublicFeedback.Leave
        igcDetails.UT_PublicNote = txtTaskPublicFeedback.Text.Trim()
    End Sub

    Private Sub txtTaskPrivateNotes_Leave(sender As Object, e As EventArgs) Handles txtTaskPrivateNotes.Leave
        igcDetails.UT_PrivateNote = txtTaskPrivateNotes.Text.Trim()
    End Sub

    Private Sub cboDifficulty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDifficulty.SelectedIndexChanged
        igcDetails.UT_DifficultyRating = cboDifficulty.SelectedIndex
    End Sub

    Private Sub cboQuality_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboQuality.SelectedIndexChanged
        igcDetails.UT_QualityRating = cboQuality.SelectedIndex
    End Sub

    Private Sub lblTaskIDAndTitle_Click(sender As Object, e As EventArgs) Handles lblTaskIDAndTitle.Click
        If lblTaskIDAndTitle.Text = String.Empty Then
            Return
        End If
        SupportingFeatures.LaunchDiscordURL($"{SupportingFeatures.WeSimGlide}index.html?task={igcDetails.MatchedTask.EntrySeqID.ToString()}")

    End Sub

    Private Sub btnCopyToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyToClipboard.Click
        ' Put the selected IGC file into the clipboard as a file drop list
        If lstbxIGCFiles.SelectedIndex < 0 Then Return

        Dim sourcePath As String = igcDetails.IGCLocalFilePath
        If String.IsNullOrWhiteSpace(sourcePath) OrElse Not File.Exists(sourcePath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Selected IGC file not found on disk.", "Copy to Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return
        End If

        Try
            Dim files As New System.Collections.Specialized.StringCollection()
            files.Add(sourcePath)

            ' Use a DataObject so we can request persistent clipboard
            Dim data As New DataObject()
            data.SetFileDropList(files)

            ' True = keep data after app closes
            Clipboard.SetDataObject(data, True)
            Using New Centered_MessageBox(Me)
                MessageBox.Show("IGC file copied to clipboard.", "Copy to Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Couldn't copy the file to clipboard:" & Environment.NewLine & ex.Message, "Copy to Clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Using
        End Try
    End Sub

    Private Sub btnConvertToOtherFormat_Click(sender As Object, e As EventArgs) Handles btnConvertToOtherFormat.Click

        ShowConvertToFormatMenu()

    End Sub

    Private Sub ShowConvertToFormatMenu()

        If convertToFormatMenu Is Nothing Then Return

        convertToFormatMenu.AutoSize = False
        convertToFormatMenu.MinimumSize = New Size(btnConvertToOtherFormat.Width, 0)
        convertToFormatMenu.Width = btnConvertToOtherFormat.Width

        For Each item As ToolStripItem In convertToFormatMenu.Items
            item.AutoSize = False
            item.Width = btnConvertToOtherFormat.Width
        Next

        Dim showPoint As New Point(0, btnConvertToOtherFormat.Height)
        convertToFormatMenu.Show(btnConvertToOtherFormat, showPoint)

    End Sub

    Private Sub convertToGpxMenuItem_Click(sender As Object, e As EventArgs) Handles convertToGpxMenuItem.Click

        ConvertSelectedIgcToGpx()

    End Sub

    Private Sub convertToKmlMenuItem_Click(sender As Object, e As EventArgs) Handles convertToKmlMenuItem.Click

        ConvertSelectedIgcToKml()

    End Sub

    Private Function GetSelectedIgcSourcePath(actionTitle As String) As String

        If lstbxIGCFiles.SelectedIndex < 0 Then Return String.Empty

        Dim sourcePath As String = igcDetails.IGCLocalFilePath
        If String.IsNullOrWhiteSpace(sourcePath) OrElse Not File.Exists(sourcePath) Then
            Using New Centered_MessageBox(Me)
                MessageBox.Show("Selected IGC file not found on disk.", actionTitle,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
            Return String.Empty
        End If

        Return sourcePath

    End Function

    Private Function PromptForDestinationFile(sourcePath As String, defaultExtension As String, filter As String, title As String) As String

        Using sfd As New SaveFileDialog()
            sfd.Title = title
            sfd.Filter = filter
            sfd.DefaultExt = defaultExtension
            sfd.AddExtension = True

            Try
                Dim defaultFolder As String = Path.GetDirectoryName(sourcePath)
                Dim defaultName As String = Path.GetFileNameWithoutExtension(sourcePath) & $".{defaultExtension}"

                If Not String.IsNullOrWhiteSpace(defaultFolder) AndAlso Directory.Exists(defaultFolder) Then
                    sfd.InitialDirectory = defaultFolder
                End If

                sfd.FileName = defaultName
            Catch
                ' If anything goes wrong with paths, just let dialog use its own defaults
            End Try

            If sfd.ShowDialog(Me) <> DialogResult.OK Then
                Return String.Empty
            End If

            Return sfd.FileName

        End Using

    End Function

    Private Sub ConvertSelectedIgcToGpx()

        Dim sourcePath As String = GetSelectedIgcSourcePath("Convert IGC to GPX file")
        If String.IsNullOrWhiteSpace(sourcePath) Then Return

        Dim gpxFileDestination As String = PromptForDestinationFile(sourcePath, "gpx", "GPX files (*.gpx)|*.gpx|All files (*.*)|*.*", "Save GPX file")
        If String.IsNullOrWhiteSpace(gpxFileDestination) Then Return

        Try
            IgcToGpxConverter.Convert(sourcePath, gpxFileDestination)

            Using New Centered_MessageBox(Me)
                MessageBox.Show($"GPX file created successfully at:{Environment.NewLine}{gpxFileDestination}",
                            "Convert IGC to GPX file",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"An error occurred converting the file:{Environment.NewLine}{ex.Message}",
                            "Convert IGC to GPX file",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
        End Try

    End Sub

    Private Sub ConvertSelectedIgcToKml()

        Dim sourcePath As String = GetSelectedIgcSourcePath("Convert IGC to KML file")
        If String.IsNullOrWhiteSpace(sourcePath) Then Return

        Dim kmlFileDestination As String = PromptForDestinationFile(sourcePath, "kml", "KML files (*.kml)|*.kml|All files (*.*)|*.*", "Save KML file")
        If String.IsNullOrWhiteSpace(kmlFileDestination) Then Return

        Try
            IgcToKmlConverter.Convert(sourcePath, kmlFileDestination)

            Using New Centered_MessageBox(Me)
                MessageBox.Show($"KML file created successfully at:{Environment.NewLine}{kmlFileDestination}",
                            "Convert IGC to KML file",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using

        Catch ex As Exception
            Using New Centered_MessageBox(Me)
                MessageBox.Show($"An error occurred converting the file:{Environment.NewLine}{ex.Message}",
                            "Convert IGC to KML file",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Using
        End Try

    End Sub

End Class
