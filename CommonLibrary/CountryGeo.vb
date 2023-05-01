Imports System.Globalization
Imports System.Net
Imports System.Windows.Forms
Imports System.Xml
Imports Microsoft.Win32

Public Class CountryGeo

    Private Shared ReadOnly CountriesCache As New Dictionary(Of String, String)
    Private Shared NotSupported As Boolean = False

    Public Shared Function GetCountryFromCoordinates(ByVal lat As Double, ByVal lon As Double) As String

        If NotSupported Then
            Return String.Empty
        End If

        'Check if the country is in cache
        Dim key As String = lat.ToString & lon.ToString
        If CountriesCache.Keys.Contains(key) Then
            Return CountriesCache(key)
        End If

        Dim url As String = $"https://dev.virtualearth.net/REST/v1/Locations/{lat.ToString("G17", CultureInfo.InvariantCulture)},{lon.ToString("G17", CultureInfo.InvariantCulture)}?includeEntityTypes=countryRegion&o=xml&key=u55CcFPOKv1c2SL4A4CT~I9NvEguTlf-h2ykmuGRMKw~ApGdr1VVqULcZjF-BUbsyb2VXD52pcbpvfA34-hHmJwP2_qzmD9fNp7TxLx0ePGZ"
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
                    NotSupported = True
                    MessageBox.Show("It appears the retrieval of country information is not supported right now.", "Country info retrieval error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return String.Empty
                End If
            End Try
        End While

        If attempts = 10 Then
            NotSupported = True
            MessageBox.Show("It appears the retrieval of country information is not supported or unavailable right now from your location.", "Country info retrieval error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return String.Empty
        End If

        Dim xmlDoc As New XmlDocument()
        xmlDoc.LoadXml(data)

        If xmlDoc.GetElementsByTagName("CountryRegion").Count > 0 Then
            Dim country As String = xmlDoc.GetElementsByTagName("CountryRegion")(0).InnerText
            CountriesCache.Add(key, country)
            Return country
        Else
            Return String.Empty
        End If

    End Function

End Class
