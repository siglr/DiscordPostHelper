Imports System.Xml

Public Class CloudLayer

    Private sglAltitudeBottom As Single
    Private sglAltitudeTop As Single
    Private sglDensity As Single
    Private sglCoverage As Single
    Private sglScattering As Single

    Public Sub New(cloudNode As XmlNode)

        sglAltitudeBottom = cloudNode.SelectNodes("CloudLayerAltitudeBot").Item(0).Attributes("Value").Value
        sglAltitudeTop = cloudNode.SelectNodes("CloudLayerAltitudeTop").Item(0).Attributes("Value").Value
        sglDensity = cloudNode.SelectNodes("CloudLayerDensity").Item(0).Attributes("Value").Value
        sglCoverage = cloudNode.SelectNodes("CloudLayerCoverage").Item(0).Attributes("Value").Value * 100
        sglScattering = cloudNode.SelectNodes("CloudLayerScattering").Item(0).Attributes("Value").Value * 100

    End Sub

    Public ReadOnly Property IsValidCloudLayer As Boolean
        Get
            Dim answer As Boolean = True
            If sglDensity = 0 Or sglCoverage = 0 Or sglAltitudeBottom = sglAltitudeTop Then
                answer = False
            End If
            Return answer
        End Get
    End Property

    Public ReadOnly Property CloudLayerText() As String
        Get
            Dim results As String
            results = "From " & FormatNumber(Conversions.MeterToFeet(sglAltitudeBottom), 0,,, TriState.False) & "' / " & FormatNumber(sglAltitudeBottom, 0,,, TriState.False) & " m " &
                "to " & FormatNumber(Conversions.MeterToFeet(sglAltitudeTop), 0,,, TriState.False) & "' / " & FormatNumber(sglAltitudeTop, 0,,, TriState.False) & " m, " &
                FormatNumber(sglCoverage, 0) & "% coverage, " & FormatNumber(sglDensity, 3) & " density, " & FormatNumber(sglScattering, 0) & "% scattering"

            Return results
        End Get
    End Property

    Public ReadOnly Property AltitudeBottom As Single
        Get
            Return sglAltitudeBottom
        End Get
    End Property
End Class
