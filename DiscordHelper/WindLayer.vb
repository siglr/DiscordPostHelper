Imports System.Xml

Public Class WindLayer

    Private sglAltitude As Single
    Private sglAngle As Integer
    Private sglSpeed As Integer
    Private sglGustAngle As Integer
    Private sglGustSpeed As Integer
    Private sglGustInterval As Integer
    Private sglGustDuration As Integer
    Private blnIsGround As Boolean
    Private blnHasGust As Boolean
    Private blnIncludeSurfaceWind As Boolean = False

    Public Sub New(windNode As XmlNode)

        sglAltitude = windNode.SelectNodes("WindLayerAltitude").Item(0).Attributes("Value").Value
        If sglAltitude <= 0 Then
            blnIsGround = True
        Else
            blnIsGround = False
        End If
        sglAngle = windNode.SelectNodes("WindLayerAngle").Item(0).Attributes("Value").Value
        sglSpeed = windNode.SelectNodes("WindLayerSpeed").Item(0).Attributes("Value").Value
        Try
            sglGustAngle = windNode.SelectNodes("GustWave/GustAngle").Item(0).Attributes("Value").Value
            sglGustSpeed = windNode.SelectNodes("GustWave/GustWaveSpeed").Item(0).Attributes("Value").Value
            sglGustInterval = windNode.SelectNodes("GustWave/GustWaveInterval").Item(0).Attributes("Value").Value
            sglGustDuration = windNode.SelectNodes("GustWave/GustWaveDuration").Item(0).Attributes("Value").Value
            If sglGustSpeed = 0 Then
                blnHasGust = False
            Else
                blnHasGust = True
            End If
        Catch ex As Exception
            blnHasGust = False
        End Try

    End Sub
    Public ReadOnly Property IsValidWindLayer As Boolean
        Get
            Dim answer As Boolean = True
            Return answer
        End Get
    End Property

    Public ReadOnly Property WindLayerText As String
        Get
            Dim results As String
            Dim divider As Integer = 1

            results = "Altitude: "

            If sglAltitude <= 0 Then
                results = results & "Ground "
                divider = 2
            Else
                results = results & FormatNumber(Conversions.MeterToFeet(sglAltitude), 0,,, TriState.False) & "' / " & FormatNumber(sglAltitude, 0,,, TriState.False) & " m "
            End If

            If divider = 1 Then
                results = results & sglAngle.ToString() & "° @ " & sglSpeed.ToString() & " kts"
                If blnIncludeSurfaceWind Then
                    results = results & " (Expect " & FormatNumber(sglSpeed / 2, 1) & " kts on surface)"
                End If
                results = results & " - " & GustText()
            Else
                results = results & sglAngle.ToString() & "° @ " & (sglSpeed / divider).ToString() & " kts (raising to " & sglSpeed.ToString() & " kts around 1000' / 300 m) - " & GustText()
            End If

            Return results
        End Get
    End Property

    Public ReadOnly Property Altitude As Single
        Get
            Return sglAltitude
        End Get
    End Property

    Public ReadOnly Property IsGround As Boolean
        Get
            Return blnIsGround
        End Get
    End Property

    Private Function GustText() As String
        Dim results As String
        If blnHasGust Then
            results = "Gust " & sglGustAngle.ToString() & "° @ " & sglGustSpeed.ToString() & " every " & sglGustInterval.ToString() & " seconds for " & sglGustDuration.ToString() & " seconds"
        Else
            results = "No gust"
        End If

        Return results
    End Function

    Public Sub IncludeSurfaceWind()
        blnIncludeSurfaceWind = True
    End Sub
End Class
