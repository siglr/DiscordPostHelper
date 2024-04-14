Imports System.Text
Imports System.Xml
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

Public Class WeatherDetails
    Public ReadOnly _MSLPressureInPa As Single
    Public ReadOnly _MSLTempKelvin As Single
    Private ReadOnly _AerosolDensity As Single
    Public ReadOnly _Precipitations As Single
    Private ReadOnly _PrecipitationType As String
    Public ReadOnly _SnowCover As Single
    Private ReadOnly _ThunderstormIntensity As Single
    Private ReadOnly _LastWindDirection As Single = 0

    Public ReadOnly CloudLayers As New List(Of CloudLayer)
    Public ReadOnly WindLayers As New List(Of WindLayer)
    Public ReadOnly LowestWindLayerAlt As Integer
    Public ReadOnly HighestWindLayerAlt As Integer
    Public ReadOnly MultiDirections As Boolean = False

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
            AltitudeMeasurement = "AMGL"
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
            LowestWindLayerAlt = Math.Min(LowestWindLayerAlt, Me.WindLayers.Last.Altitude)
            HighestWindLayerAlt = Math.Max(HighestWindLayerAlt, Me.WindLayers.Last.Altitude)
            If Not MultiDirections Then
                MultiDirections = (_LastWindDirection <> Me.WindLayers.Last.Angle)
                If Not MultiDirections Then
                    _LastWindDirection = Me.WindLayers.Last.Angle
                End If
            End If
        Next

        Me.WindLayers = Me.WindLayers.OrderBy(Function(x) x.Altitude).ToList()

        'if there is no ground layer, the first layer should indicate surface wind
        If Not blnHasGround Then
            Me.WindLayers.First.IncludeSurfaceWind = True
        End If

    End Sub

    Public ReadOnly Property PresetName() As String

    Public ReadOnly Property MSLTemperature(Optional prefUnits As PreferredUnits = Nothing, Optional forceF As Boolean = False, Optional forceC As Boolean = False) As String
        Get
            Return SupportingFeatures.MSLTemperature(_MSLTempKelvin, prefUnits, forceF, forceC)
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

    Public ReadOnly Property MSLPressure(textForNonStandard As String, suppressNonStandardWarning As Boolean, Optional prefUnits As PreferredUnits = Nothing, Optional useEmoji As Boolean = True, Optional forceinHg As Boolean = False, Optional forcehPa As Boolean = False) As String
        Get

            Dim notStdBaro As String = String.Empty

            If Not IsStandardMSLPressure Then
                notStdBaro = If(suppressNonStandardWarning, " ", If(useEmoji, " ⚠️ ", " * ")) & textForNonStandard
            End If

            Return $"{SupportingFeatures.GetMSLPressure(_MSLPressureInPa, prefUnits, forceinHg, forcehPa)}{notStdBaro}"

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
            Return SupportingFeatures.GetSnowCover(_SnowCover)
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

