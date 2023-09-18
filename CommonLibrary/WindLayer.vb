Imports System.Text
Imports System.Web.ModelBinding
Imports System.Xml
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

Public Class WindLayer
    Private ReadOnly _GustAngle As Integer
    Private ReadOnly _GustSpeed As Integer
    Private ReadOnly _GustInterval As Integer
    Private ReadOnly _GustDuration As Integer
    Private ReadOnly _HasGust As Boolean
    Private ReadOnly _AltitudeMeasurement As String

    Public IncludeSurfaceWind As Boolean = False
    Public ReadOnly Property Altitude As Single
    Public ReadOnly Property Angle As Integer
    Public ReadOnly Property Speed As Integer

    Public ReadOnly Property IsGround As Boolean

    Public Sub New(windNode As XmlNode, altMeasurement As String)

        _AltitudeMeasurement = altMeasurement

        Altitude = XmlConvert.ToSingle(windNode.SelectNodes("WindLayerAltitude").Item(0).Attributes("Value").Value)
        IsGround = Altitude <= 0
        Angle = XmlConvert.ToSingle(windNode.SelectNodes("WindLayerAngle").Item(0).Attributes("Value").Value)
        Speed = XmlConvert.ToSingle(windNode.SelectNodes("WindLayerSpeed").Item(0).Attributes("Value").Value)
        Try
            _GustAngle = XmlConvert.ToSingle(windNode.SelectNodes("GustWave/GustAngle").Item(0).Attributes("Value").Value)
            _GustSpeed = XmlConvert.ToSingle(windNode.SelectNodes("GustWave/GustWaveSpeed").Item(0).Attributes("Value").Value)
            _GustInterval = XmlConvert.ToSingle(windNode.SelectNodes("GustWave/GustWaveInterval").Item(0).Attributes("Value").Value)
            _GustDuration = XmlConvert.ToSingle(windNode.SelectNodes("GustWave/GustWaveDuration").Item(0).Attributes("Value").Value)
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

    Public ReadOnly Property WindLayerText(Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            Dim sb As New StringBuilder()
            Dim divider As Integer = 1
            Dim aroundAltString As String = String.Empty

            If Altitude <= 0 Then
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    aroundAltString = "1000' / 300 m"
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            aroundAltString = "300 m"
                        Case AltitudeUnits.Imperial
                            aroundAltString = "1000'"
                    End Select
                End If
                sb.Append("Ground ")
                divider = 2
            Else
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    sb.AppendFormat("{0:N0}' / {1:N0} m ",
                                Conversions.MeterToFeet(Altitude),
                                Altitude)
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            sb.AppendFormat("{0:N0} m ", Altitude)
                            aroundAltString = "300 m"
                        Case AltitudeUnits.Imperial
                            sb.AppendFormat("{0:N0}' ", Conversions.MeterToFeet(Altitude))
                            aroundAltString = "1000'"
                    End Select
                End If
            End If

            If divider = 1 Then
                If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                    sb.AppendFormat("{0}° @ {1} kts ({2:N1} m/s)",
                                Angle.ToString(),
                                Speed.ToString(),
                                Conversions.KnotsToMps(Speed))
                Else
                    Select Case prefUnits.WindSpeed
                        Case WindSpeedUnits.MeterPerSecond
                            sb.AppendFormat("{0}° @ {1:N1} m/s",
                                Angle.ToString(),
                                Conversions.KnotsToMps(Speed))
                        Case WindSpeedUnits.Knots
                            sb.AppendFormat("{0}° @ {1} kts",
                                Angle.ToString(),
                                Speed.ToString())
                    End Select
                End If
                If IncludeSurfaceWind Then
                    If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                        sb.AppendFormat(" (Expect {0:N1} kts ({1:N1} m/s) on surface)",
                                    Speed / 2,
                                    Conversions.KnotsToMps(Speed / 2))
                    Else
                        Select Case prefUnits.WindSpeed
                            Case WindSpeedUnits.MeterPerSecond
                                sb.AppendFormat(" (Expect {0:N1} m/s on surface)", Conversions.KnotsToMps(Speed / 2))
                            Case WindSpeedUnits.Knots
                                sb.AppendFormat(" (Expect {0:N1} kts on surface)", Speed / 2)
                        End Select
                    End If
                End If
                sb.AppendFormat("{0}", GetGustText(prefUnits, True))
            Else
                If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                    sb.AppendFormat("{0}° @ {1} kts ({2:N1} m/s) (raising to {3} kts ({4:N1} m/s) around {5}){6}",
                                Angle.ToString(),
                                (Speed / divider).ToString(),
                                Conversions.KnotsToMps(Speed / divider),
                                Speed.ToString(),
                                Conversions.KnotsToMps(Speed),
                                aroundAltString,
                                GetGustText(prefUnits, True))
                Else
                    Select Case prefUnits.WindSpeed
                        Case WindSpeedUnits.MeterPerSecond
                            sb.AppendFormat("{0}° @ {1:N1} m/s (raising to {2:N1} m/s around {3}){4}",
                                Angle.ToString(),
                                Conversions.KnotsToMps(Speed / divider),
                                Conversions.KnotsToMps(Speed),
                                aroundAltString,
                                GetGustText(prefUnits, True))
                        Case WindSpeedUnits.Knots
                            sb.AppendFormat("{0}° @ {1} kts (raising to {2} kts around {3}){4}",
                                Angle.ToString(),
                                (Speed / divider).ToString(),
                                Speed.ToString(),
                                aroundAltString,
                                GetGustText(prefUnits, True))
                    End Select
                End If
            End If

            Return sb.ToString().Trim

        End Get
    End Property

    Public ReadOnly Property WindLayerTextWithoutDirection(Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            Dim sb As New StringBuilder()
            Dim divider As Integer = 1
            Dim aroundAltString As String = String.Empty

            If Altitude <= 0 Then
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    aroundAltString = "1000' / 300 m"
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            aroundAltString = "300 m"
                        Case AltitudeUnits.Imperial
                            aroundAltString = "1000'"
                    End Select
                End If
                sb.Append("Ground ")
                divider = 2
            Else
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    sb.AppendFormat("{0} {1:N0}' / {2:N0} m ",
                                _AltitudeMeasurement,
                                Conversions.MeterToFeet(Altitude),
                                Altitude)
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            sb.AppendFormat("{0} {1:N0} m ",
                                _AltitudeMeasurement,
                                Altitude)
                            aroundAltString = "300 m"
                        Case AltitudeUnits.Imperial
                            sb.AppendFormat("{0} {1:N0}' ",
                                _AltitudeMeasurement,
                                Conversions.MeterToFeet(Altitude))
                            aroundAltString = "1000'"
                    End Select
                End If
            End If

            If divider = 1 Then
                If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                    sb.AppendFormat("@ {0} kts ({1:N1} m/s)",
                                Speed.ToString(),
                                Conversions.KnotsToMps(Speed))
                Else
                    Select Case prefUnits.WindSpeed
                        Case WindSpeedUnits.MeterPerSecond
                            sb.AppendFormat("@ {0:N1} m/s", Conversions.KnotsToMps(Speed))
                        Case WindSpeedUnits.Knots
                            sb.AppendFormat("@ {0} kts", Speed.ToString())
                    End Select
                End If
                If IncludeSurfaceWind Then
                    If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                        sb.AppendFormat(" (Expect {0:N1} kts ({1:N1} m/s) on surface)",
                                    Speed / 2,
                                    Conversions.KnotsToMps(Speed / 2))
                    Else
                        Select Case prefUnits.WindSpeed
                            Case WindSpeedUnits.MeterPerSecond
                                sb.AppendFormat(" (Expect {0:N1} m/s on surface)", Conversions.KnotsToMps(Speed / 2))
                            Case WindSpeedUnits.Knots
                                sb.AppendFormat(" (Expect {0:N1} kts on surface)", Speed / 2)
                        End Select
                    End If
                End If
            Else
                If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                    sb.AppendFormat("@ {0} kts ({1:N1} m/s) (raising to {2} kts ({3:N1} m/s) around {4})",
                                (Speed / divider).ToString(),
                                Conversions.KnotsToMps(Speed / divider),
                                Speed.ToString(),
                                Conversions.KnotsToMps(Speed),
                                aroundAltString)
                Else
                    Select Case prefUnits.WindSpeed
                        Case WindSpeedUnits.MeterPerSecond
                            sb.AppendFormat("@ {0:N1} m/s (raising to {1:N1} m/s around {2})",
                                Conversions.KnotsToMps(Speed / divider),
                                Conversions.KnotsToMps(Speed), aroundAltString)
                        Case WindSpeedUnits.Knots
                            sb.AppendFormat("@ {0} kts (raising to {1} kts around {2})",
                                (Speed / divider).ToString(),
                                Speed.ToString(), aroundAltString)
                    End Select
                End If
            End If

            Return sb.ToString().Trim

        End Get
    End Property

    Public Function GetGustText(Optional prefUnits As PreferredUnits = Nothing, Optional includeHyphen As Boolean = False) As String

        Dim results As String = String.Empty
        Dim prefix As String = String.Empty

        If includeHyphen Then
            prefix = " - "
        End If

        If _HasGust Then
            If prefUnits Is Nothing OrElse prefUnits.WindSpeed = WindSpeedUnits.Both Then
                results = String.Format(prefix & "Gust {0}° @ {1} kts ({2:N1} m/s) every {3} seconds for {4} seconds :warning:*Gusts formats are currently bugged in MSFS*",
                                    _GustAngle,
                                    _GustSpeed,
                                    Conversions.KnotsToMps(_GustSpeed),
                                    _GustInterval,
                                    _GustDuration)
            Else
                Select Case prefUnits.WindSpeed
                    Case WindSpeedUnits.MeterPerSecond
                        results = String.Format("prefix & Gust {0}° @ {1:N1} m/s every {2} seconds for {3} seconds :warning:*Gusts formats are currently bugged in MSFS*",
                                    _GustAngle,
                                    Conversions.KnotsToMps(_GustSpeed),
                                    _GustInterval,
                                    _GustDuration)
                    Case WindSpeedUnits.Knots
                        results = String.Format("prefix & Gust {0}° @ {1} kts every {2} seconds for {3} seconds :warning:*Gusts formats are currently bugged in MSFS*",
                                    _GustAngle,
                                    _GustSpeed,
                                    _GustInterval,
                                    _GustDuration)
                End Select
            End If
        Else
            results = String.Empty
        End If

        Return results
    End Function

    Public ReadOnly Property AltitudeCorrectUnit(prefUnits As PreferredUnits) As String
        Get
            If prefUnits.Altitude = AltitudeUnits.Both Then
                Return String.Format("{0:N0} / {1:N0}",
                                Conversions.MeterToFeet(Altitude),
                                Altitude)
            Else
                Select Case prefUnits.Altitude
                    Case AltitudeUnits.Metric
                        Return String.Format("{0:N0}", Altitude)
                    Case AltitudeUnits.Imperial
                        Return String.Format("{0:N0}", Conversions.MeterToFeet(Altitude))
                End Select
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property SpeedCorrectUnit(prefUnits As PreferredUnits) As String
        Get
            If prefUnits.WindSpeed = WindSpeedUnits.Both Then
                Return String.Format("{0} / {1:N1}",
                                Speed.ToString(),
                                Conversions.KnotsToMps(Speed))
            Else
                Select Case prefUnits.WindSpeed
                    Case WindSpeedUnits.MeterPerSecond
                        Return String.Format("{0:N1}",
                                Conversions.KnotsToMps(Speed))
                    Case WindSpeedUnits.Knots
                        Return String.Format("{0}",
                                Speed.ToString())
                End Select
            End If
            Return String.Empty
        End Get
    End Property
End Class
