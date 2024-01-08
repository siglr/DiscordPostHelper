Imports System.Xml
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

<Serializable()>
Public Class CloudLayer
    Public ReadOnly Property Density As Single
    Public ReadOnly Property Coverage As Single
    Public ReadOnly Property Scattering As Single
    Public ReadOnly Property AltitudeTop As Single
    Public ReadOnly Property AltitudeBottom As Single

    Public Sub New(xmlCloudNode As XmlNode)

        AltitudeBottom = XmlConvert.ToSingle(xmlCloudNode.SelectNodes("CloudLayerAltitudeBot").Item(0).Attributes("Value").Value)
        AltitudeTop = XmlConvert.ToSingle(xmlCloudNode.SelectNodes("CloudLayerAltitudeTop").Item(0).Attributes("Value").Value)
        Density = XmlConvert.ToSingle(xmlCloudNode.SelectNodes("CloudLayerDensity").Item(0).Attributes("Value").Value)
        Coverage = XmlConvert.ToSingle(xmlCloudNode.SelectNodes("CloudLayerCoverage").Item(0).Attributes("Value").Value) * 100
        Scattering = XmlConvert.ToSingle(xmlCloudNode.SelectNodes("CloudLayerScattering").Item(0).Attributes("Value").Value) * 100

    End Sub

    Public ReadOnly Property IsValidCloudLayer As Boolean
        Get
            Dim blnAnswer As Boolean = True
            If Coverage = 0 Or AltitudeBottom = AltitudeTop Then
                blnAnswer = False
            End If
            Return blnAnswer
        End Get
    End Property

    Public ReadOnly Property CloudLayerText(Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            Dim strResults As String = String.Empty
            If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                strResults = String.Format("From {0}' to {1}' / {2} m to {3} m, {4}% coverage, {5} density, {6}% scattering",
                                                  FormatNumber(Conversions.MeterToFeet(AltitudeBottom), 0,,, TriState.False),
                                                  FormatNumber(Conversions.MeterToFeet(AltitudeTop), 0,,, TriState.False),
                                                  FormatNumber(AltitudeBottom, 0,,, TriState.False),
                                                  FormatNumber(AltitudeTop, 0,,, TriState.False),
                                                  FormatNumber(Coverage, 0),
                                                  FormatNumber(Density, 3),
                                                  FormatNumber(Scattering, 0))
            Else
                Select Case prefUnits.Altitude
                    Case AltitudeUnits.Metric
                        strResults = String.Format("From {0} m to {1} m, {2}% coverage, {3} density, {4}% scattering",
                                                  FormatNumber(AltitudeBottom, 0,,, TriState.False),
                                                  FormatNumber(AltitudeTop, 0,,, TriState.False),
                                                  FormatNumber(Coverage, 0),
                                                  FormatNumber(Density, 3),
                                                  FormatNumber(Scattering, 0))
                    Case AltitudeUnits.Imperial
                        strResults = String.Format("From {0}' to {1}', {2}% coverage, {3} density, {4}% scattering",
                                                  FormatNumber(Conversions.MeterToFeet(AltitudeBottom), 0,,, TriState.False),
                                                  FormatNumber(Conversions.MeterToFeet(AltitudeTop), 0,,, TriState.False),
                                                  FormatNumber(Coverage, 0),
                                                  FormatNumber(Density, 3),
                                                  FormatNumber(Scattering, 0))
                End Select
            End If

            Return strResults
        End Get
    End Property

    Public ReadOnly Property AltitudeBottomCorrectUnit(prefUnits As PreferredUnits) As String
        Get
            If prefUnits.Altitude = AltitudeUnits.Both Then
                Return String.Format("{0:N0} / {1:N0}",
                                Conversions.MeterToFeet(_AltitudeBottom),
                                _AltitudeBottom)
            Else
                Select Case prefUnits.Altitude
                    Case AltitudeUnits.Metric
                        Return String.Format("{0:N0}", _AltitudeBottom)
                    Case AltitudeUnits.Imperial
                        Return String.Format("{0:N0}", Conversions.MeterToFeet(_AltitudeBottom))
                End Select
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property AltitudeTopCorrectUnit(prefUnits As PreferredUnits) As String
        Get
            If prefUnits.Altitude = AltitudeUnits.Both Then
                Return String.Format("{0:N0} / {1:N0}",
                                Conversions.MeterToFeet(AltitudeTop),
                                AltitudeTop)
            Else
                Select Case prefUnits.Altitude
                    Case AltitudeUnits.Metric
                        Return String.Format("{0:N0}", AltitudeTop)
                    Case AltitudeUnits.Imperial
                        Return String.Format("{0:N0}", Conversions.MeterToFeet(AltitudeTop))
                End Select
            End If
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property CoverageForGrid() As String
        Get
            Return FormatNumber(Coverage, 0)
        End Get
    End Property

    Public ReadOnly Property DensityForGrid() As String
        Get
            Return FormatNumber(Density, 3)
        End Get
    End Property

    Public ReadOnly Property ScatteringForGrid() As String
        Get
            Return FormatNumber(Scattering, 0)
        End Get
    End Property

End Class
