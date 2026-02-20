Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json.Linq

Public Module GliderMatcher

    Private Const DefaultBaseUrl As String = "https://siglr.com/DiscordPostHelper/"
    Private Const RulesEndpoint As String = "RetrieveGliderMatchRules.php"
    Private ReadOnly _syncRoot As New Object()
    Private _baseUrlResolver As Func(Of String) = Function() DefaultBaseUrl
    Private _rulesLoaded As Boolean = False
    Private _rules As List(Of GliderRule)

    Public Property BaseUrlResolver As Func(Of String)
        Get
            Return _baseUrlResolver
        End Get
        Set(value As Func(Of String))
            _baseUrlResolver = If(value, Function() DefaultBaseUrl)
        End Set
    End Property

    Private Class GliderRule
        Public Property RuleID As Integer
        Public Property RuleType As String
        Public Property Pattern As String
        Public Property Priority As Integer
        Public Property GliderKey As String
        Public Property DisplayName As String
    End Class

    Public Sub EnsureRulesLoaded(Optional forceReload As Boolean = False)
        If _rulesLoaded AndAlso Not forceReload Then
            Return
        End If

        SyncLock _syncRoot
            If _rulesLoaded AndAlso Not forceReload Then
                Return
            End If

            Dim loadedRules = LoadRulesFromServer()
            _rules = loadedRules.OrderBy(Function(r) r.Priority).ThenBy(Function(r) r.RuleID).ToList()
            _rulesLoaded = True
        End SyncLock
    End Sub

    Public Function MatchGliderKey(rawGliderType As String) As String
        EnsureRulesLoaded()

        Dim normalized As String = If(rawGliderType, String.Empty).ToLowerInvariant().Trim()
        If String.IsNullOrEmpty(normalized) Then
            Return "Unknown"
        End If

        For Each rule In _rules
            Dim pattern = If(rule.Pattern, String.Empty)
            Select Case If(rule.RuleType, String.Empty).ToUpperInvariant()
                Case "EXACT"
                    If normalized = pattern Then
                        Return rule.GliderKey
                    End If
                Case "CONTAINS"
                    If normalized.Contains(pattern) Then
                        Return rule.GliderKey
                    End If
                Case "REGEX"
                    If Regex.IsMatch(normalized, pattern, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant) Then
                        Return rule.GliderKey
                    End If
            End Select
        Next

        Return "Unknown"
    End Function

    Private Function LoadRulesFromServer() As List(Of GliderRule)
        Dim baseUrl As String = If(BaseUrlResolver?.Invoke(), String.Empty).Trim()
        If String.IsNullOrWhiteSpace(baseUrl) Then
            baseUrl = DefaultBaseUrl
        End If

        If Not baseUrl.EndsWith("/", StringComparison.Ordinal) Then
            baseUrl &= "/"
        End If

        Dim endpointUrl As String = $"{baseUrl}{RulesEndpoint}"

        Using client As New HttpClient()
            Dim response = client.GetAsync(endpointUrl).GetAwaiter().GetResult()
            If Not response.IsSuccessStatusCode Then
                Throw New Exception($"Unable to retrieve glider rules ({CInt(response.StatusCode)} {response.ReasonPhrase}).")
            End If

            Dim payload As String = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            Dim json As JToken = JToken.Parse(payload)
            Dim status As String = json.Value(Of String)("status")
            If Not String.Equals(status, "success", StringComparison.OrdinalIgnoreCase) Then
                Dim serverMessage As String = json.Value(Of String)("message")
                Throw New Exception($"Glider rules endpoint returned an error: {serverMessage}")
            End If

            Dim rulesArray = TryCast(json("rules"), JArray)
            If rulesArray Is Nothing Then
                Throw New Exception("Glider rules endpoint returned an invalid payload.")
            End If

            Dim loadedRules As New List(Of GliderRule)
            For Each item In rulesArray
                Dim rule As New GliderRule With {
                    .RuleID = item.Value(Of Integer?)("RuleID").GetValueOrDefault(),
                    .RuleType = If(item.Value(Of String)("RuleType"), String.Empty),
                    .Pattern = If(item.Value(Of String)("Pattern"), String.Empty),
                    .Priority = item.Value(Of Integer?)("Priority").GetValueOrDefault(),
                    .GliderKey = If(item.Value(Of String)("GliderKey"), "Unknown"),
                    .DisplayName = If(item.Value(Of String)("DisplayName"), String.Empty)
                }
                loadedRules.Add(rule)
            Next

            Return loadedRules
        End Using
    End Function

End Module
