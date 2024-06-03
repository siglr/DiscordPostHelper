Imports System.Net.Http
Imports Newtonsoft.Json
Imports SIGLR.SoaringTools.CommonLibrary

Public Class NewsManagement
    Private newsEntries As List(Of Dictionary(Of String, String))

    Private Sub NewsManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler cmbAction.SelectedIndexChanged, AddressOf cmbAction_SelectedIndexChanged
        AddHandler lstNewsEntries.SelectedIndexChanged, AddressOf lstNewsEntries_SelectedIndexChanged

        btnFetchNews_Click(sender, e)

    End Sub

    Private Sub cmbAction_SelectedIndexChanged(sender As Object, e As EventArgs)
        ' Reset all fields
        txtTaskID.Enabled = False
        txtKey.Enabled = False
        txtTitle.Enabled = False
        txtSubtitle.Enabled = False
        txtComments.Enabled = False
        txtCredits.Enabled = False
        txtEventDate.Enabled = False
        txtPublished.Enabled = False
        txtNews.Enabled = False
        txtEntrySeqID.Enabled = False
        txtURLToGo.Enabled = False
        txtExpiration.Enabled = False

        Select Case cmbAction.SelectedItem.ToString()
            Case "CreateTask", "UpdateTask"
                txtTaskID.Enabled = True
                txtTitle.Enabled = True
                txtSubtitle.Enabled = True
                txtComments.Enabled = True
                txtCredits.Enabled = True
                txtEntrySeqID.Enabled = True

            Case "DeleteTask"
                txtTaskID.Enabled = True

            Case "CreateEvent"
                txtKey.Enabled = True
                txtTitle.Enabled = True
                txtSubtitle.Enabled = True
                txtComments.Enabled = True
                txtEventDate.Enabled = True
                txtEntrySeqID.Enabled = True
                txtURLToGo.Enabled = True
                txtExpiration.Enabled = True

            Case "DeleteEvent"
                txtKey.Enabled = True

            Case "CreateNews"
                txtKey.Enabled = True
                txtTitle.Enabled = True
                txtSubtitle.Enabled = True
                txtComments.Enabled = True
                txtNews.Enabled = True
                txtURLToGo.Enabled = True
                txtExpiration.Enabled = True

            Case "DeleteNews"
                txtKey.Enabled = True
        End Select
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Validate input fields
        If Not ValidateFields() Then
            MessageBox.Show("Please ensure all required fields are filled out correctly.")
            Return
        End If

        Dim action As String = cmbAction.SelectedItem.ToString()
        Dim parameters As New Dictionary(Of String, String) From {
            {"action", action},
            {"TaskID", txtTaskID.Text},
            {"Key", txtKey.Text},
            {"Title", txtTitle.Text},
            {"Subtitle", txtSubtitle.Text},
            {"Comments", txtComments.Text},
            {"Credits", txtCredits.Text},
            {"EventDate", txtEventDate.Text},
            {"Published", DateTime.Now.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss")},
            {"News", txtNews.Text},
            {"EntrySeqID", txtEntrySeqID.Text},
            {"URLToGo", txtURLToGo.Text},
            {"Expiration", txtExpiration.Text}
        }

        Dim content As New FormUrlEncodedContent(parameters)
        Dim response As String = Await PostDataAsync($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}ManageNews.php", content)

        MessageBox.Show(response)

        btnFetchNews_Click(sender, e)

    End Sub

    Private Async Sub btnFetchNews_Click(sender As Object, e As EventArgs) Handles btnFetchNews.Click
        Dim response As String = Await GetDataAsync($"{SupportingFeatures.SIGLRDiscordPostHelperFolder()}RetrieveNews.php")
        Dim result = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(response)

        If result("status").ToString() = "success" Then
            newsEntries = JsonConvert.DeserializeObject(Of List(Of Dictionary(Of String, String)))(result("data").ToString())
            lstNewsEntries.Items.Clear()
            For Each entry In newsEntries
                lstNewsEntries.Items.Add(entry("Key"))
            Next
        Else
            MessageBox.Show("Error fetching news entries: " & result("message").ToString())
        End If
    End Sub

    Private Sub lstNewsEntries_SelectedIndexChanged(sender As Object, e As EventArgs)
        If lstNewsEntries.SelectedIndex <> -1 Then
            Dim selectedEntry = newsEntries(lstNewsEntries.SelectedIndex)
            txtTaskID.Text = selectedEntry("TaskID")
            txtKey.Text = selectedEntry("Key")
            txtTitle.Text = selectedEntry("Title")
            txtSubtitle.Text = selectedEntry("Subtitle")
            txtComments.Text = selectedEntry("Comments")
            txtCredits.Text = selectedEntry("Credits")
            txtEventDate.Text = selectedEntry("EventDate")
            txtPublished.Text = selectedEntry("Published")
            txtNews.Text = selectedEntry("News")
            txtEntrySeqID.Text = selectedEntry("EntrySeqID")
            txtURLToGo.Text = selectedEntry("URLToGo")
            txtExpiration.Text = selectedEntry("Expiration")
        End If
    End Sub

    Private Async Function PostDataAsync(url As String, content As FormUrlEncodedContent) As Task(Of String)
        Using client As New HttpClient()
            Dim response As HttpResponseMessage = Await client.PostAsync(url, content)
            response.EnsureSuccessStatusCode()
            Return Await response.Content.ReadAsStringAsync()
        End Using
    End Function

    Private Async Function GetDataAsync(url As String) As Task(Of String)
        Using client As New HttpClient()
            Dim response As HttpResponseMessage = Await client.GetAsync(url)
            response.EnsureSuccessStatusCode()
            Return Await response.Content.ReadAsStringAsync()
        End Using
    End Function

    Private Function ValidateFields() As Boolean
        ' Check required fields based on action
        If cmbAction.SelectedItem Is Nothing Then Return False

        Select Case cmbAction.SelectedItem.ToString()
            Case "CreateTask", "UpdateTask"
                If txtTaskID.Text = String.Empty OrElse txtTitle.Text = String.Empty OrElse txtSubtitle.Text = String.Empty OrElse txtComments.Text = String.Empty Then Return False
                If txtEntrySeqID.Text <> String.Empty AndAlso Not Integer.TryParse(txtEntrySeqID.Text, Nothing) Then Return False

            Case "DeleteTask"
                If txtTaskID.Text = String.Empty Then Return False

            Case "CreateEvent"
                If txtKey.Text = String.Empty OrElse txtTitle.Text = String.Empty OrElse txtSubtitle.Text = String.Empty OrElse txtComments.Text = String.Empty OrElse txtEventDate.Text = String.Empty Then Return False
                If txtEntrySeqID.Text <> String.Empty AndAlso Not Integer.TryParse(txtEntrySeqID.Text, Nothing) Then Return False
                If txtEventDate.Text <> String.Empty AndAlso Not DateTime.TryParse(txtEventDate.Text, Nothing) Then Return False
                If txtExpiration.Text <> String.Empty AndAlso Not DateTime.TryParse(txtExpiration.Text, Nothing) Then Return False

            Case "DeleteEvent", "DeleteNews"
                If txtKey.Text = String.Empty Then Return False

            Case "CreateNews"
                If txtKey.Text = String.Empty OrElse txtTitle.Text = String.Empty OrElse txtSubtitle.Text = String.Empty OrElse txtComments.Text = String.Empty OrElse txtNews.Text = String.Empty Then Return False
                If txtExpiration.Text <> String.Empty AndAlso Not DateTime.TryParse(txtExpiration.Text, Nothing) Then Return False
        End Select

        Return True
    End Function
End Class
