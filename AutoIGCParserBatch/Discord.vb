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

        ' then call your downloader if you like
        Await DownloadAllIgcFilesAsync()
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

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        txtLog.Clear()
        listOfIGCUrls.Clear()
        btnStop.Enabled = True
        btnStart.Enabled = False
        btnUpload.Enabled = False
        btnGo.Enabled = False
        forcedTaskConfirmed = False

        If txtForcedTask.Text.Trim <> String.Empty Then
            If MsgBox($"Are you sure you want to force match these IGC to task #{txtForcedTask.Text.Trim} ?", vbYesNo, "Forced task specified") = vbNo Then
                Exit Sub
            End If
            forcedTaskConfirmed = True
        End If

        scrapingUp = True
        txtLog.AppendText("▶️  Starting auto-scroll up…" & vbCrLf)
        ScrapeIgcUrlsFromDiscordThreadAsync()
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
End Class
