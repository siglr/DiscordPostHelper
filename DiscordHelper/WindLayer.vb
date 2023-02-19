Imports System.Text
Imports System.Xml

Public Class WindLayer
    Private ReadOnly _Angle As Integer
    Private ReadOnly _Speed As Integer
    Private ReadOnly _GustAngle As Integer
    Private ReadOnly _GustSpeed As Integer
    Private ReadOnly _GustInterval As Integer
    Private ReadOnly _GustDuration As Integer
    Private ReadOnly _HasGust As Boolean

    Public IncludeSurfaceWind As Boolean = False
    Public ReadOnly Property Altitude As Single
    Public ReadOnly Property IsGround As Boolean

    Public Sub New(windNode As XmlNode)

        Altitude = windNode.SelectNodes("WindLayerAltitude").Item(0).Attributes("Value").Value
        IsGround = Altitude <= 0
        _Angle = windNode.SelectNodes("WindLayerAngle").Item(0).Attributes("Value").Value
        _Speed = windNode.SelectNodes("WindLayerSpeed").Item(0).Attributes("Value").Value
        Try
            _GustAngle = windNode.SelectNodes("GustWave/GustAngle").Item(0).Attributes("Value").Value
            _GustSpeed = windNode.SelectNodes("GustWave/GustWaveSpeed").Item(0).Attributes("Value").Value
            _GustInterval = windNode.SelectNodes("GustWave/GustWaveInterval").Item(0).Attributes("Value").Value
            _GustDuration = windNode.SelectNodes("GustWave/GustWaveDuration").Item(0).Attributes("Value").Value
            _HasGust = _GustSpeed <> 0
        Catch ex As Exception
            _HasGust = False
        End Try

    End Sub
    Public ReadOnly Property IsValidWindLayer As Boolean
        Get
            Dim answer As Boolean = True
            'Nothing to check yet

            Return answer
        End Get
    End Property

    Public ReadOnly Property WindLayerText As String
        Get
            Dim sb As New StringBuilder()
            Dim divider As Integer = 1

            sb.Append("Altitude: ")
            If Altitude <= 0 Then
                sb.Append("Ground ")
                divider = 2
            Else
                sb.AppendFormat("{0:N0}' / {1:N0} m ",
                                Conversions.MeterToFeet(Altitude),
                                Altitude)
            End If

            If divider = 1 Then
                sb.AppendFormat("{0}° @ {1} kts",
                                _Angle.ToString(),
                                _Speed.ToString())
                If IncludeSurfaceWind Then
                    sb.AppendFormat(" (Expect {0:N1} kts on surface)",
                                    _Speed / 2)
                End If
                sb.AppendFormat(" - {0}", GetGustText())
            Else
                sb.AppendFormat("{0}° @ {1} kts (raising to {2} kts around 1000' / 300 m) - {3}",
                                _Angle.ToString(),
                                (_Speed / divider).ToString(),
                                _Speed.ToString(),
                                GetGustText())
            End If

            Return sb.ToString().Trim

        End Get
    End Property

    Private Function GetGustText() As String
        Dim results As String
        If _HasGust Then
            results = String.Format("Gust {0}° @ {1} kts every {2} seconds for {3} seconds :warning:*Gusts formats are currently bugged in MSFS*",
                                    _GustAngle,
                                    _GustSpeed,
                                    _GustInterval,
                                    _GustDuration)
        Else
            results = "No gust"
        End If

        Return results
    End Function

End Class
