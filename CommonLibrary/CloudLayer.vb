Imports System.Xml

Public Class CloudLayer
    Private ReadOnly _AltitudeTop As Single
    Private ReadOnly _Density As Single
    Private ReadOnly _Coverage As Single
    Private ReadOnly _Scattering As Single
    Public ReadOnly Property AltitudeBottom As Single

    Public Sub New(xmlCloudNode As XmlNode)

        AltitudeBottom = xmlCloudNode.SelectNodes("CloudLayerAltitudeBot").Item(0).Attributes("Value").Value
        _AltitudeTop = xmlCloudNode.SelectNodes("CloudLayerAltitudeTop").Item(0).Attributes("Value").Value
        _Density = xmlCloudNode.SelectNodes("CloudLayerDensity").Item(0).Attributes("Value").Value
        _Coverage = xmlCloudNode.SelectNodes("CloudLayerCoverage").Item(0).Attributes("Value").Value * 100
        _Scattering = xmlCloudNode.SelectNodes("CloudLayerScattering").Item(0).Attributes("Value").Value * 100

    End Sub

    Public ReadOnly Property IsValidCloudLayer As Boolean
        Get
            Dim blnAnswer As Boolean = True
            If _Density = 0 Or _Coverage = 0 Or AltitudeBottom = _AltitudeTop Then
                blnAnswer = False
            End If
            Return blnAnswer
        End Get
    End Property

    Public ReadOnly Property CloudLayerText() As String
        Get
            Dim strResults As String = String.Format("From {2}' to {3}' / {0} m to {1} m, {4}% coverage, {5} density, {6}% scattering",
                                                  FormatNumber(AltitudeBottom, 0,,, TriState.False),
                                                  FormatNumber(_AltitudeTop, 0,,, TriState.False),
                                                  FormatNumber(Conversions.MeterToFeet(AltitudeBottom), 0,,, TriState.False),
                                                  FormatNumber(Conversions.MeterToFeet(_AltitudeTop), 0,,, TriState.False),
                                                  FormatNumber(_Coverage, 0),
                                                  FormatNumber(_Density, 3),
                                                  FormatNumber(_Scattering, 0))
            Return strResults
        End Get
    End Property

End Class
