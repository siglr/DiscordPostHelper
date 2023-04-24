Imports System.Net
Imports System.Xml

Public Class CountryGeo

    Private Shared ReadOnly CountriesCache As New Dictionary(Of String, String)

    Public Shared Function GetCountryFromCoordinates(ByVal lat As Double, ByVal lon As Double) As String

        'Check if the country is in cache
        Dim key As String = lat.ToString & lon.ToString
        If CountriesCache.Keys.Contains(key) Then
            Return CountriesCache(key)
        End If

        Dim url As String = $"http://dev.virtualearth.net/REST/v1/Locations/{lat},{lon}?includeEntityTypes=countryRegion&o=xml&key=u55CcFPOKv1c2SL4A4CT~I9NvEguTlf-h2ykmuGRMKw~ApGdr1VVqULcZjF-BUbsyb2VXD52pcbpvfA34-hHmJwP2_qzmD9fNp7TxLx0ePGZ"
        Dim client As New WebClient()
        Dim data As String = ""
        Dim attempts As Integer = 0
        While attempts < 10
            Try
                data = client.DownloadString(url)
                Exit While
            Catch ex As WebException
                If ex.Message.Contains("(429) Too Many Requests") Then
                    attempts += 1
                    System.Threading.Thread.Sleep(1000) ' Wait for 1 second
                Else
                    Throw ex
                End If
            End Try
        End While

        If attempts = 10 Then
            Throw New Exception("Too many attempts to retrieve data from the API")
        End If

        Dim xmlDoc As New XmlDocument()
        xmlDoc.LoadXml(data)

        Dim country As String = xmlDoc.GetElementsByTagName("CountryRegion")(0).InnerText
        CountriesCache.Add(key, country)

        Return country

    End Function

End Class
