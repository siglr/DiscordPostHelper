Imports System.Text
Imports System.Xml
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

Public Class WeatherDetails
    Private ReadOnly _MSLPressureInPa As Single
    Private ReadOnly _MSLTempKelvin As Single
    Private ReadOnly _AerosolDensity As Single
    Private ReadOnly _Precipitations As Single
    Private ReadOnly _PrecipitationType As String
    Private ReadOnly _SnowCover As Single
    Private ReadOnly _ThunderstormIntensity As Single

    Public ReadOnly CloudLayers As New List(Of CloudLayer)
    Public ReadOnly WindLayers As New List(Of WindLayer)

    Public Sub New(xmlWeatherXMLDoc As XmlDocument)

        Dim blnHasGround As Boolean = False

        PresetName = xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/Name").Item(0).FirstChild.Value
        _MSLPressureInPa = XmlConvert.ToSingle(xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/MSLPressure").Item(0).Attributes("Value").Value)
        _MSLTempKelvin = XmlConvert.ToSingle(xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/MSLTemperature").Item(0).Attributes("Value").Value)
        _AerosolDensity = XmlConvert.ToSingle(xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/AerosolDensity").Item(0).Attributes("Value").Value)
        _Precipitations = XmlConvert.ToSingle(xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/Precipitations").Item(0).Attributes("Value").Value)
        _SnowCover = XmlConvert.ToSingle(xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/SnowCover").Item(0).Attributes("Value").Value)
        _ThunderstormIntensity = XmlConvert.ToSingle(xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/ThunderstormIntensity").Item(0).Attributes("Value").Value)

        If xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/IsAltitudeAMGL").Item(0).FirstChild.Value = "True" Then
            AltitudeMeasurement = "AGL"
        Else
            AltitudeMeasurement = "AMSL"
        End If

        Try
            _PrecipitationType = xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/PrecipitationType").Item(0).Attributes("Value").Value.ToLower
        Catch ex As Exception
            _PrecipitationType = "rain"
        End Try

        For Each xmlCloudNode As XmlNode In xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/CloudLayer")
            CloudLayers.Add(New CloudLayer(xmlCloudNode))
        Next

        CloudLayers = CloudLayers.OrderBy(Function(x) x.AltitudeBottom).ToList()

        For Each xmlWindNode As XmlNode In xmlWeatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/WindLayer")
            Me.WindLayers.Add(New WindLayer(xmlWindNode, AltitudeMeasurement))
            If Me.WindLayers.Last.IsGround Then
                blnHasGround = True
            End If
        Next

        Me.WindLayers = Me.WindLayers.OrderBy(Function(x) x.Altitude).ToList()

        'if there is no ground layer, the first layer should indicate surface wind
        If Not blnHasGround Then
            Me.WindLayers.First.IncludeSurfaceWind = True
        End If

    End Sub

    Public ReadOnly Property PresetName() As String

    Public ReadOnly Property MSLTemperature(Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            If prefUnits Is Nothing OrElse prefUnits.Temperature = TemperatureUnits.Both Then
                Return String.Format("{0:N0}°C / {1:N0}°F", Conversions.KelvinToCelsius(_MSLTempKelvin), Conversions.KelvinToFarenheit(_MSLTempKelvin))
            Else
                Select Case prefUnits.Temperature
                    Case TemperatureUnits.Celsius
                        Return String.Format("{0:N0}°C", Conversions.KelvinToCelsius(_MSLTempKelvin))
                    Case TemperatureUnits.Fahrenheit
                        Return String.Format("{0:N0}°F", Conversions.KelvinToFarenheit(_MSLTempKelvin))
                End Select
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property IsStandardMSLPressure As Boolean
        Get
            Dim roundedPaToInHg As Double = 0
            Try
                roundedPaToInHg = CDec(String.Format("{0:F2}", Conversions.PaToInHg(_MSLPressureInPa)))
            Catch ex As Exception
            End Try

            Return roundedPaToInHg = 29.92
        End Get
    End Property

    Public ReadOnly Property MSLPressure(textForNonStandard As String, suppressNonStandardWarning As Boolean, Optional prefUnits As PreferredUnits = Nothing, Optional useEmoji As Boolean = True) As String
        Get

            Dim notStdBaro As String = String.Empty

            If Not IsStandardMSLPressure Then
                notStdBaro = If(suppressNonStandardWarning, " ", If(useEmoji, " ⚠️ ", " * ")) & textForNonStandard
            End If

            If prefUnits Is Nothing OrElse prefUnits.Barometric = BarometricUnits.Both Then
                Return String.Format("{0:F2} inHg / {1:N0} hPa{2}", Conversions.PaToInHg(_MSLPressureInPa), _MSLPressureInPa / 100, notStdBaro)
            Else
                Select Case prefUnits.Barometric
                    Case BarometricUnits.hPa
                        Return String.Format("{0:N0} hPa{1}", _MSLPressureInPa / 100, notStdBaro)
                    Case BarometricUnits.inHg
                        Return String.Format("{0:F2} inHg{1}", Conversions.PaToInHg(_MSLPressureInPa), notStdBaro)
                End Select
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Humidity As String
        Get
            Return FormatNumber(_AerosolDensity, 2)
        End Get
    End Property

    Public ReadOnly Property ThunderstormIntensity As Integer
        Get
            Return FormatNumber(_ThunderstormIntensity * 100, 0)
        End Get
    End Property

    Public ReadOnly Property SnowCover As String
        Get
            Dim cover As String
            If _SnowCover = 0 Then
                cover = "0"
            Else
                cover = String.Format("{0:N0} inches / {1:N2} m", Conversions.MeterToInches(_SnowCover), _SnowCover)
            End If

            Return cover
        End Get
    End Property

    Public ReadOnly Property Precipitations As String
        Get
            Dim strSummary As String = String.Empty

            Select Case _Precipitations
                Case 0
                    strSummary = "None"
                Case < 0.5
                    strSummary = String.Format("Slight {0} ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
                Case < 2
                    strSummary = String.Format("Slight {0} shower ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
                Case < 4
                    strSummary = String.Format("Moderate {0} shower ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
                Case < 8
                    strSummary = String.Format("Heavy {0} ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
                Case < 10
                    strSummary = String.Format("Very heavy {0} ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
                Case < 50
                    strSummary = String.Format("Heavy {0} shower ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
                Case Else
                    strSummary = String.Format("Violent {0} shower ({1:N1} mm/h)", _PrecipitationType, _Precipitations)
            End Select
            Return strSummary
        End Get
    End Property

    Public ReadOnly Property HasSnowCover As Boolean
        Get
            Return _SnowCover > 0
        End Get
    End Property

    Public ReadOnly Property AltitudeMeasurement As String

    Public ReadOnly Property HasPrecipitations As Boolean
        Get
            Return _Precipitations > 0
        End Get
    End Property

    Public ReadOnly Property CloudLayersText(Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            Dim results As New StringBuilder()
            Dim countLayer As Integer = 0

            For Each layer As CloudLayer In CloudLayers
                If layer.IsValidCloudLayer Then
                    countLayer = ++1
                    results.AppendLine($"- {layer.CloudLayerText(prefUnits)}")
                End If
            Next

            If countLayer = 0 Then
                results.AppendLine("None")
            End If

            Return results.ToString.Trim

        End Get
    End Property

    Public ReadOnly Property WindLayersAsString(Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            Dim results As New StringBuilder()
            Dim countLayer As Integer = 0

            For Each layer As WindLayer In Me.WindLayers
                If layer.IsValidWindLayer Then
                    countLayer = ++1
                    results.AppendLine($"- {layer.WindLayerText(prefUnits)}")
                End If
            Next

            If countLayer = 0 Then
                results.AppendLine("None")
            End If

            Return results.ToString.Trim

        End Get
    End Property
End Class

