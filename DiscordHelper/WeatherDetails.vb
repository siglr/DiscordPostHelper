Imports System.Xml

Public Class WeatherDetails

    Private strPresetName As String
    Private dblMSLPressureInPa As Single
    Private dblMSLTempKelvin As Single
    Private dblAerosolDensity As Single
    Private dblPrecipitations As Single
    Private strPrecipitationType As String
    Private dblSnowCover As Single
    Private strAltitudeMeasurement As String

    Private lstCloudLayers As New List(Of CloudLayer)
    Private lstWindLayers As New List(Of WindLayer)

    Public Sub New(weatherXMLDoc As XmlDocument)

        Dim HasGround As Boolean = False

        strPresetName = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/Name").Item(0).FirstChild.Value
        dblMSLPressureInPa = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/MSLPressure").Item(0).Attributes("Value").Value
        dblMSLTempKelvin = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/MSLTemperature").Item(0).Attributes("Value").Value
        dblAerosolDensity = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/AerosolDensity").Item(0).Attributes("Value").Value
        dblPrecipitations = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/Precipitations").Item(0).Attributes("Value").Value
        dblSnowCover = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/SnowCover").Item(0).Attributes("Value").Value

        If weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/IsAltitudeAMGL").Item(0).FirstChild.Value = "True" Then
            strAltitudeMeasurement = "AMGL"
        Else
            strAltitudeMeasurement = "AMSL"
        End If

        Try
            strPrecipitationType = weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/PrecipitationType").Item(0).Attributes("Value").Value.ToLower
        Catch ex As Exception
            strPrecipitationType = "rain"
        End Try

        For Each cloudNode As XmlNode In weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/CloudLayer")
            lstCloudLayers.Add(New CloudLayer(cloudNode))
        Next

        lstCloudLayers = lstCloudLayers.OrderBy(Function(x) x.AltitudeBottom).ToList()

        For Each windNode As XmlNode In weatherXMLDoc.DocumentElement.SelectNodes("WeatherPreset.Preset/WindLayer")
            lstWindLayers.Add(New WindLayer(windNode))
            If lstWindLayers.Last.IsGround Then
                HasGround = True
            End If
        Next

        lstWindLayers = lstWindLayers.OrderBy(Function(x) x.Altitude).ToList()

        'if there is no ground layer, the first layer should indicate surface wind
        If Not HasGround Then
            lstWindLayers.First.IncludeSurfaceWind()
        End If

    End Sub

    Public ReadOnly Property PresetName() As String
        Get
            Return strPresetName
        End Get
    End Property

    Public ReadOnly Property MSLPressure As String
        Get
            Return FormatNumber(Conversions.PaToInHg(dblMSLPressureInPa), 2) & " inHg / " & FormatNumber(dblMSLPressureInPa / 100, 0,,, TriState.False) & " hpa"
        End Get
    End Property

    Public ReadOnly Property MSLTemperature As String
        Get
            Return FormatNumber(Conversions.KelvinToCelsius(dblMSLTempKelvin), 0) & "°C / " & FormatNumber(Conversions.KelvinToFarenheit(dblMSLTempKelvin), 0) & "°F"
        End Get
    End Property

    Public ReadOnly Property Humidity As String
        Get
            Return FormatNumber(dblAerosolDensity, 2)
        End Get
    End Property

    Public ReadOnly Property SnowCover As String
        Get
            Dim cover As String
            If dblSnowCover = 0 Then
                cover = "0"
            Else
                cover = FormatNumber(Conversions.MeterToInches(dblSnowCover), 0) & " inches / " & FormatNumber(dblSnowCover, 2) & " m"
            End If

            Return cover
        End Get
    End Property

    Public ReadOnly Property Precipitations As String
        Get
            Dim strSummary As String = String.Empty

            Select Case dblPrecipitations
                Case 0
                    strSummary = "None"
                Case < 0.5
                    strSummary = "Slight " & strPrecipitationType & " (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
                Case < 2
                    strSummary = "Slight " & strPrecipitationType & " shower (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
                Case < 4
                    strSummary = "Moderate " & strPrecipitationType & " shower (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
                Case < 8
                    strSummary = "Heavy " & strPrecipitationType & " (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
                Case < 10
                    strSummary = "Very heavy " & strPrecipitationType & " (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
                Case < 50
                    strSummary = "Heavy " & strPrecipitationType & " shower (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
                Case Else
                    strSummary = "Violent " & strPrecipitationType & " shower (" & FormatNumber(dblPrecipitations, 1) & " mm/h)"
            End Select
            Return strSummary
        End Get
    End Property

    Public ReadOnly Property HasSnowCover As Boolean
        Get
            Dim answer As Boolean = False

            If dblSnowCover > 0 Then
                answer = True
            End If

            Return answer
        End Get
    End Property

    Public ReadOnly Property AltitudeMeasurement As String
        Get
            Return strAltitudeMeasurement
        End Get
    End Property

    Public ReadOnly Property HasPrecipitations As Boolean
        Get
            Dim answer As Boolean = False

            If dblPrecipitations > 0 Then
                answer = True
            End If

            Return answer
        End Get
    End Property

    Public ReadOnly Property CloudLayers As String
        Get
            Dim results As String = String.Empty
            Dim countLayer As Integer = 0

            For Each layer As CloudLayer In lstCloudLayers
                If layer.IsValidCloudLayer Then
                    countLayer = ++1
                    results = results & vbCrLf & layer.CloudLayerText
                End If
            Next

            If countLayer = 0 Then
                results = vbCrLf & "None"
            End If

            Return results

        End Get
    End Property

    Public ReadOnly Property WindLayers As String
        Get
            Dim results As String = String.Empty
            Dim countLayer As Integer = 0

            For Each layer As WindLayer In lstWindLayers
                If layer.IsValidWindLayer Then
                    countLayer = ++1
                    results = results & vbCrLf & layer.WindLayerText
                End If
            Next

            If countLayer = 0 Then
                results = "None"
            End If

            Return results

        End Get
    End Property
End Class

