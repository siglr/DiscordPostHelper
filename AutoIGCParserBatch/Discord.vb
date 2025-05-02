Imports CefSharp
Imports CefSharp.WinForms
Imports System.IO
Imports System.Diagnostics
Imports System.Net.Http

Public Class frmDiscord

    Private listOfIGCUrls As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    Private Async Sub btnGo_Click(sender As Object, e As EventArgs) Handles btnGo.Click

        txtDiscordThreadURL.Text = Clipboard.GetText

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

    Private Async Sub ScrapeIgcUrlsFromDiscordThreadAsync()
        txtLog.AppendText("→ Scanning IGC URLs in thread…" & vbCrLf)

        ' helper JS: find scrollable parent of the message list
        Const findAndScrollJs As String = "
      (function(dir){
        const list = document.querySelector('ol[data-list-id=""chat-messages""]');
        if(!list) return false;
        let el = list;
        // walk up until we find an element that's actually scrollable
        while(el && !(el.scrollHeight > el.clientHeight)) el = el.parentElement;
        if(!el) return false;
        // scroll down or up
        el.scrollTop = (dir === 'down' ? el.scrollHeight : 0);
        return true;
      })(DIR);
    "

        ' 1) Scroll DOWN in a loop, scraping after each jump
        txtLog.AppendText("→ Scrolling DOWN to newest…" & vbCrLf)
        For i = 1 To 20
            Dim js = findAndScrollJs.Replace("DIR", "'down'")
            Dim r = Await browser.EvaluateScriptAsync(js)
            If Not (r.Success AndAlso CBool(r.Result)) Then
                txtLog.AppendText($"    ⚠ Could not find scroll container on pass {i}" & vbCrLf)
                Exit For
            End If
            Await Task.Delay(500)
            Await ExtractIgcLinksAsync()
        Next
        txtLog.AppendText("✔ Bottom region reached." & vbCrLf)

        ' 2) Scroll UP in a loop, scraping after each jump
        txtLog.AppendText("→ Scrolling UP to oldest…" & vbCrLf)
        For i = 1 To 50
            Dim js = findAndScrollJs.Replace("DIR", "'up'")
            Dim r = Await browser.EvaluateScriptAsync(js)
            If Not (r.Success AndAlso CBool(r.Result)) Then
                txtLog.AppendText($"    ⚠ Could not find scroll container on pass {i}" & vbCrLf)
                Exit For
            End If
            Await Task.Delay(500)
            Await ExtractIgcLinksAsync()
        Next
        txtLog.AppendText("✔ Top region reached." & vbCrLf)
        txtLog.AppendText($"🎯 Total unique IGC files: {listOfIGCUrls.Count}{vbCrLf}")

        Await DownloadAllIgcFilesAsync()

    End Sub

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
        Using client As New HttpClient()
            For Each kvp In listOfIGCUrls
                Dim name = kvp.Key    ' e.g. "Fliege_…igc"
                Dim url = kvp.Value  ' full URL
                Dim dest = Path.Combine(tempFolder, name)

                Try
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
    End Function

    Private Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim uploadForm As New Form1
        uploadForm.ShowDialog()
        uploadForm.Dispose()
        uploadForm = Nothing
    End Sub
End Class
