Imports CefSharp
Imports CefSharp.WinForms
Imports System.IO
Imports System.Diagnostics
Imports System.Net.Http
Imports System.DirectoryServices.ActiveDirectory

Public Class frmDiscord

    Private listOfIGCUrls As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private scrapingUp As Boolean = False
    Private forcedTaskConfirmed As Boolean = False
    Private browserRequestContext As RequestContext
    Private browser As ChromiumWebBrowser

    Public Sub New()
        InitializeComponent()
        InitializeBrowser()
    End Sub

    Private Sub InitializeBrowser()
        Dim cachePath = Startup.CefCachePath
        Dim requestContextSettings = New RequestContextSettings() With {
            .CachePath = cachePath,
            .PersistSessionCookies = True
        }
        browserRequestContext = New RequestContext(requestContextSettings)

        Dim newBrowser = New ChromiumWebBrowser("https://discord.com/app", browserRequestContext) With {
            .ActivateBrowserOnCreation = False,
            .Dock = DockStyle.Fill,
            .Name = "browser"
        }

        If browser IsNot Nothing Then
            browser.Dispose()
        End If

        pnlBrowserHost.Controls.Clear()
        browser = newBrowser
        pnlBrowserHost.Controls.Add(browser)

        Startup.LogCefDiagnostic($"RequestContext CachePath = {cachePath}")
        Startup.LogCefDiagnostic($"CookieDbExists = {File.Exists(Startup.CookieDbPath)}")
    End Sub

    Private Async Sub btnGo_Click(sender As Object, e As EventArgs) Handles btnGo.Click

        txtDiscordThreadURL.Text = Clipboard.GetText

        txtLog.Clear()
        listOfIGCUrls.Clear()

        Dim url = txtDiscordThreadURL.Text.Trim()
        If String.IsNullOrEmpty(url) Then
            txtLog.AppendText("❌ Please enter a thread URL." & vbCrLf)
            Return
        End If

        txtLog.AppendText("→ Loading thread…" & vbCrLf)
        browser.Load(url)
        Await WaitForConditionAsync("document.readyState === 'complete'", 20)

        txtLog.AppendText("→ Waiting for messages to render…" & vbCrLf)
        Dim ready = Await WaitForConditionAsync(
            "(() => {
                const list = document.querySelector('ol[data-list-id=""chat-messages""]');
                return list && list.childElementCount > 0;
             })()", 20)
        If Not ready Then
            txtLog.AppendText("❌ Messages didn’t load in time." & vbCrLf)
        Else
            txtLog.AppendText("✔ Thread loaded and messages are visible." & vbCrLf)
        End If

    End Sub

    Private Sub frmDiscord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        browser.Load("https://discord.com/channels/@me")
    End Sub

    Private Sub frmDiscord_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If browser IsNot Nothing Then
            browser.Dispose()
            browser = Nothing
        End If

        If browserRequestContext IsNot Nothing Then
            browserRequestContext.Dispose()
            browserRequestContext = Nothing
        End If
    End Sub

    Private Async Function ScrapeIgcUrlsFromDiscordThreadAsync() As Task
        ' assume user has already loaded & manually scrolled to bottom…
        txtLog.AppendText($"🎯 Starting harvest at bottom: {listOfIGCUrls.Count} links so far." & vbCrLf)

        Dim findAndScrollUpJs = "
      (function(){
        const list = document.querySelector('ol[data-list-id=""chat-messages""]');
        if(!list) return false;
        let el = list;
        while(el && !(el.scrollHeight > el.clientHeight)) el = el.parentElement;
        if(!el) return false;
        el.scrollTop = 0;
        return true;
      })();
    "

        Dim oldCount As Integer = 0
        Dim nbrTries As Integer = 0
        Do While scrapingUp
            ' scroll up
            Dim r = Await browser.EvaluateScriptAsync(findAndScrollUpJs)
            If Not (r.Success AndAlso Convert.ToBoolean(r.Result)) Then
                txtLog.AppendText("    ⚠ Cannot find scroll container, aborting." & vbCrLf)
                Exit Do
            End If

            Await Task.Delay(400)

            ' harvest whatever's now in view
            oldCount = listOfIGCUrls.Count
            Await ExtractIgcLinksAsync()
            txtLog.AppendText($"    ⬆️  Scrolled up — total so far: {listOfIGCUrls.Count}" & vbCrLf)
            If listOfIGCUrls.Count = oldCount Then
                nbrTries = nbrTries + 1
                If nbrTries > 4 AndAlso Not chkManualStop.Checked Then
                    btnStop_Click(btnStop, Nothing)
                End If
            End If
        Loop

        txtLog.AppendText($"✔️  Harvest loop ended. {listOfIGCUrls.Count} total IGC URLs collected." & vbCrLf)
        scrapingUp = False

    End Function

    ''' <summary>
    ''' Pulls all .igc links currently in view into the field dictionary.
    ''' </summary>
    Private Async Function ExtractIgcLinksAsync() As Task
        Dim script = "
      Array.from(document.querySelectorAll('a'))
           .map(a => a.href)
           .filter(h => /\.igc($|\?)/i.test(h));
    "
        Dim resp = Await browser.EvaluateScriptAsync(script)
        If Not resp.Success OrElse resp.Result Is Nothing Then Return

        For Each o In DirectCast(resp.Result, IEnumerable(Of Object))
            Dim url = o.ToString()
            Dim uri = New Uri(url)
            ' strip off any query string so we get a clean file name
            Dim name = Path.GetFileName(uri.LocalPath)
            If Not listOfIGCUrls.ContainsKey(name) Then
                listOfIGCUrls.Add(name, url)
                lblProgress.Text = $"0 / {listOfIGCUrls.Count}"
                txtLog.AppendText($"  ➕ Found {name}" & vbCrLf)
            End If
        Next
    End Function

    ''' <summary>
    ''' Returns the scrollHeight of Discord’s message container, or zero.
    ''' </summary>
    Private Async Function GetDiscordScrollHeightAsync() As Task(Of Long)
        Dim js = "
        const msgList = document.querySelector('ol[data-list-id=""chat-messages""]');
        const sc = msgList?.closest('div[role=""group""][data-jump-section=""global""]');
        sc ? sc.scrollHeight : 0
    "
        Dim res = Await browser.EvaluateScriptAsync(js)
        If res.Success AndAlso res.Result IsNot Nothing Then
            Return Convert.ToInt64(res.Result)
        End If
        Return 0L
    End Function

    ''' <summary>
    ''' Polls a JS expression until it returns true or the timeout elapses.
    ''' </summary>
    Private Async Function WaitForConditionAsync(expr As String,
                                                Optional timeoutSeconds As Integer = 15) _
                                                As Task(Of Boolean)

        Dim sw = Stopwatch.StartNew()
        Do
            Dim resp = Await browser.EvaluateScriptAsync(expr)
            If resp.Success AndAlso Convert.ToBoolean(resp.Result) Then
                Return True
            ElseIf Not resp.Success Then
                txtLog.AppendText($"    ⚠ JS error on '{expr}': {resp.Message}{Environment.NewLine}")
                Return False
            End If
            Await Task.Delay(300)
        Loop While sw.Elapsed.TotalSeconds < timeoutSeconds

        Return False
    End Function

    Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        txtLog.Clear()
        listOfIGCUrls.Clear()
        btnStop.Enabled = True
        btnStart.Enabled = False
        btnUpload.Enabled = False
        btnGo.Enabled = False
        forcedTaskConfirmed = False

        Await TryJumpToPresentAsync()

        scrapingUp = True
        txtLog.AppendText("▶️  Starting auto-scroll up…" & vbCrLf)
        Await ScrapeIgcUrlsFromDiscordThreadAsync()
        If txtForcedTask.Text.Trim <> String.Empty Then
            If MsgBox($"Are you sure you want to force match these IGC to task #{txtForcedTask.Text.Trim} ?", vbYesNo, "Forced task specified") = vbNo Then
                Exit Sub
            End If
            forcedTaskConfirmed = True
        End If
        Await DownloadAllIgcFilesAsync()

    End Sub

    Private Async Function DownloadAllIgcFilesAsync() As Task
        ' 1) Compute “IGCTemp” next to your exe
        Dim exeFolder = Path.GetDirectoryName(Application.ExecutablePath)
        Dim tempFolder = Path.Combine(exeFolder, "IGCTemp")

        ' 2) Clear or recreate the folder
        If Directory.Exists(tempFolder) Then
            Directory.Delete(tempFolder, recursive:=True)
        End If
        Directory.CreateDirectory(tempFolder)

        ' 3) Download each file
        Dim indexCount As Integer = 0
        Using client As New HttpClient()
            For Each kvp In listOfIGCUrls
                Dim name = kvp.Key    ' e.g. "Fliege_…igc"
                Dim url = kvp.Value  ' full URL
                Dim dest = Path.Combine(tempFolder, name)

                Try
                    indexCount += 1
                    lblProgress.Text = $"{indexCount} / {listOfIGCUrls.Count}"
                    txtLog.AppendText($"→ Downloading {name}…{vbCrLf}")
                    Dim data = Await client.GetByteArrayAsync(url)
                    File.WriteAllBytes(dest, data)
                    txtLog.AppendText($"✔ Saved to {dest}{vbCrLf}")
                Catch ex As Exception
                    txtLog.AppendText($"❌ Error downloading {name}: {ex.Message}{vbCrLf}")
                End Try
            Next

        End Using

        txtLog.AppendText($"🏁 All downloads complete. Files in: {tempFolder}{vbCrLf}")

        If indexCount = listOfIGCUrls.Count Then
            btnUpload_Click(btnUpload, Nothing)
            forcedTaskConfirmed = False
        End If

    End Function

    Private Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        If txtForcedTask.Text.Trim <> String.Empty AndAlso Not forcedTaskConfirmed Then
            If MsgBox($"Are you sure you want to force match these IGC to task #{txtForcedTask.Text.Trim} ?", vbYesNo, "Forced task specified") = vbNo Then
                Exit Sub
            End If
        End If
        Dim uploadForm As New WSGBatchUpload
        If txtForcedTask.Text.Trim <> String.Empty Then
            uploadForm.ForcedTaskId = CInt(txtForcedTask.Text)
        End If
        uploadForm.ShowDialog()
        uploadForm.Dispose()
        uploadForm = Nothing
        txtForcedTask.Text = String.Empty
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        scrapingUp = False
        btnStop.Enabled = False
        btnStart.Enabled = True
        btnUpload.Enabled = True
        btnGo.Enabled = True
        txtLog.AppendText("⏹  Stopped by user." & vbCrLf)
    End Sub

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

    Private Async Function TryJumpToPresentAsync() As Task
        txtLog.AppendText("→ Waiting up to 5s for ‘Jump To Present’ button…" & vbCrLf)

        Dim found = Await WaitForConditionAsync(
        "Array.from(document.querySelectorAll('button'))" &
        ".some(b => b.textContent.trim() === 'Jump To Present')",
        timeoutSeconds:=5
    )

        If Not found Then
            txtLog.AppendText("→ ‘Jump To Present’ never appeared—continuing without it." & vbCrLf)
            Return
        End If

        txtLog.AppendText("✔ ‘Jump To Present’ is visible; assigning id…" & vbCrLf)
        Dim prep = Await browser.EvaluateScriptAsync("
        (()=> {
          const btn = Array.from(document.querySelectorAll('button'))
                            .find(b => b.textContent.trim() === 'Jump To Present');
          if (btn) btn.id = 'jumpPresentBtn';
          return !!btn;
        })();
    ")
        If prep.Success AndAlso Convert.ToBoolean(prep.Result) Then
            txtLog.AppendText("✔ id=jumpPresentBtn assigned; clicking…" & vbCrLf)
            Await ClickElementByIdAsync("jumpPresentBtn")
            Await Task.Delay(1500)
            txtLog.AppendText("✔ Jumped to present." & vbCrLf)
        Else
            txtLog.AppendText("⚠ Button vanished before we could click it." & vbCrLf)
        End If
    End Function

End Class
