Imports System.Globalization
Imports SIGLR.SoaringTools.CommonLibrary.PreferredUnits

Public Class ATCWaypoint

    Private ReadOnly _MinAlt As Nullable(Of Integer) = Nothing
    Private ReadOnly _MaxAlt As Nullable(Of Integer) = Nothing
    Private ReadOnly _Diameter As Integer = 0

    Public ReadOnly Property Sequence As Integer = 0
    Public ReadOnly Property WaypointName As String = String.Empty
    Public ReadOnly Property ICAO As String = String.Empty
    Public ReadOnly Property Gate As String
        Get
            Dim result As String = String.Empty
            Dim diameter As String = String.Empty

            If _Diameter > 0 Then
                Dim convImperial As Single
                convImperial = Conversions.KmToMiles(_Diameter / 1000)
                If SupportingFeatures.PrefUnits Is Nothing OrElse SupportingFeatures.PrefUnits.GateDiameter = GateDiameterUnits.Both Then
                    If convImperial < 1 Then
                        convImperial = Conversions.MeterToFeet(_Diameter)
                        diameter = $"{Math.Round(convImperial, 0)}' ({_Diameter} m)"
                    Else
                        diameter = $"{String.Format("{0:0.0}", Math.Round(convImperial, 1))} mi ({_Diameter} m)"
                    End If
                Else
                    Select Case SupportingFeatures.PrefUnits.GateDiameter
                        Case GateDiameterUnits.Metric
                            diameter = $"{_Diameter} m"
                        Case GateDiameterUnits.Imperial
                            If convImperial < 1 Then
                                convImperial = Conversions.MeterToFeet(_Diameter)
                                diameter = $"{Math.Round(convImperial, 0)}'"
                            Else
                                diameter = $"{String.Format("{0:0.0}", Math.Round(convImperial, 1))} mi"
                            End If
                    End Select
                End If
            End If

            If IsTaskStart Then
                result = ($"Start - {diameter}").Trim
            ElseIf IsTaskEnd Then
                result = $"Finish - {diameter}".Trim
            Else
                result = diameter.Trim
            End If

            Return result

        End Get
    End Property

    Public IsTaskStart As Boolean = False
    Public IsTaskEnd As Boolean = False
    Public Country As String = String.Empty

    Private Shared _TaskStartFound As Boolean = False

    Public Sub New(strFullATCId As String,
                   strWorldPosition As String,
                   intSeq As Integer,
                   icaoCode As String)

        Sequence = intSeq + 1
        ICAO = icaoCode

        If intSeq = 0 Then
            _TaskStartFound = False
        End If

        Me.FullATCId = strFullATCId

        CheckTaskStartOrEnd()

        SetLatitudeAndLongitude(strWorldPosition, Latitude, Longitude, _wpelevation)

        If SupportingFeatures.ClientRunning = SupportingFeatures.ClientApp.DiscordPostHelper Then
            Country = CountryGeo.GetCountryFromCoordinates(Latitude, Longitude)
        End If

        'Set name of waypoint
        If strFullATCId.Contains("+") OrElse strFullATCId.Contains("|") Then
            WaypointName = strFullATCId.Substring(0, strFullATCId.IndexOfAny("+|"))
            strFullATCId = strFullATCId.Substring(WaypointName.Length)
            If WaypointName.StartsWith("*") Then
                WaypointName = WaypointName.Substring(1, WaypointName.Length - 1)
            End If
        Else
            'No encoding
            WaypointName = strFullATCId
        End If

        'Read the radius part if any
        If Not (strFullATCId.Contains("x") AndAlso Integer.TryParse(strFullATCId.Substring(strFullATCId.IndexOf("x") + 1), _Diameter)) Then
            If ICAO = String.Empty Then
                If IsTaskStart Then
                    _Diameter = 5000
                End If
                If IsTaskEnd Then
                    _Diameter = 4000
                End If
                If Not IsTaskStart And Not IsTaskEnd Then
                    _Diameter = 1000
                End If
            End If
        End If

        'Remove radius part (when only radius is specified)
        If strFullATCId.Contains("|x") Then
            strFullATCId = strFullATCId.Substring(0, strFullATCId.IndexOf("|"))
        End If
        'Remove radius part
        If strFullATCId.Contains("x") Then
            strFullATCId = strFullATCId.Substring(0, strFullATCId.IndexOf("x"))
        End If

        'Check if waypoint contains encoding valid restriction data
        If strFullATCId.Contains("|") OrElse strFullATCId.Contains("/") Then

            ContainsRestriction = True

            'Remove elevation part
            If strFullATCId.Contains("+") Then
                strFullATCId = strFullATCId.Substring(strFullATCId.IndexOfAny("/|"))
            End If

            Dim tempValue As Integer = 0
            If strFullATCId.Contains("|") Then
                'Contains maximum
                If strFullATCId.Contains("/") Then
                    If Integer.TryParse(strFullATCId.Substring(strFullATCId.IndexOf("|") + 1, strFullATCId.IndexOfAny("/") - 1), tempValue) Then
                        _MaxAlt = tempValue
                    End If
                Else
                    If Integer.TryParse(strFullATCId.Substring(strFullATCId.IndexOf("|") + 1), tempValue) Then
                        _MaxAlt = tempValue
                    End If
                End If
            End If
            If strFullATCId.Contains("/") Then
                'Contains minimum
                If Integer.TryParse(strFullATCId.Substring(strFullATCId.IndexOf("/") + 1), tempValue) Then
                    _MinAlt = tempValue
                End If
            End If

        End If
    End Sub

    Private Sub SetLatitudeAndLongitude(strWorldPosition As String, ByRef pLatitude As Double, ByRef pLongitude As Double, ByRef pElevation As Double)
        Dim strParts As String() = strWorldPosition.Split(",")
        pLatitude = Conversions.ConvertToLatitude(strParts(0))
        pLongitude = Conversions.ConvertToLongitude(strParts(1))
        pElevation = Double.Parse(strParts(2), CultureInfo.InvariantCulture)
    End Sub

    Private Sub CheckTaskStartOrEnd()
        If Me.FullATCId.StartsWith("*") Then
            If Not _TaskStartFound Then
                _TaskStartFound = True
                IsTaskStart = True
            Else
                IsTaskEnd = True
            End If
        End If
    End Sub


    Public ReadOnly Property FullATCId As String = String.Empty

    Public ReadOnly Property ContainsRestriction As Boolean = False

    Public ReadOnly Property Latitude As Double = 0

    Public ReadOnly Property Longitude As Double = 0

    Private ReadOnly _wpelevation As Double = 0

    Public ReadOnly Property Restrictions(prefUnits As PreferredUnits) As String
        Get
            Return Restrictions(False, prefUnits)
        End Get
    End Property

    Public ReadOnly Property Restrictions(includeName As Boolean, Optional prefUnits As PreferredUnits = Nothing) As String
        Get
            Dim strRestrictions As String = String.Empty

            Dim prefix As String = String.Empty
            If includeName Then
                prefix = String.Format("{0}: ", WaypointName)
            End If

            If _MinAlt IsNot Nothing AndAlso _MaxAlt IsNot Nothing Then
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    strRestrictions = String.Format("{0}Between {1}' and {2}' ({3}m and {4}m)",
                                                prefix,
                                                _MinAlt,
                                                _MaxAlt,
                                                Math.Round(Conversions.FeetToMeters(_MinAlt), 0),
                                                Math.Round(Conversions.FeetToMeters(_MaxAlt), 0)
                                                )
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            strRestrictions = String.Format("{0}Between {1}m and {2}m",
                                                prefix,
                                                Math.Round(Conversions.FeetToMeters(_MinAlt), 0),
                                                Math.Round(Conversions.FeetToMeters(_MaxAlt), 0)
                                                )
                        Case AltitudeUnits.Imperial
                            strRestrictions = String.Format("{0}Between {1}' and {2}')",
                                                prefix,
                                                _MinAlt,
                                                _MaxAlt
                                                )
                    End Select
                End If
            ElseIf _MinAlt IsNot Nothing Then
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    strRestrictions = String.Format("{0}MIN {1}' ({2}m)",
                                                prefix,
                                                _MinAlt,
                                                Math.Round(Conversions.FeetToMeters(_MinAlt), 0)
                                                )
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            strRestrictions = String.Format("{0}MIN {1}m",
                                                prefix,
                                                Math.Round(Conversions.FeetToMeters(_MinAlt), 0)
                                                )
                        Case AltitudeUnits.Imperial
                            strRestrictions = String.Format("{0}MIN {1}'",
                                                prefix,
                                                _MinAlt
                                                )
                    End Select
                End If
            ElseIf _MaxAlt IsNot Nothing Then
                If prefUnits Is Nothing OrElse prefUnits.Altitude = AltitudeUnits.Both Then
                    strRestrictions = String.Format("{0}MAX {1}' ({2}m)",
                                                prefix,
                                                _MaxAlt,
                                                Math.Round(Conversions.FeetToMeters(_MaxAlt), 0)
                                                )
                Else
                    Select Case prefUnits.Altitude
                        Case AltitudeUnits.Metric
                            strRestrictions = String.Format("{0}MAX {1}m",
                                                prefix,
                                                Math.Round(Conversions.FeetToMeters(_MaxAlt), 0)
                                                )
                        Case AltitudeUnits.Imperial
                            strRestrictions = String.Format("{0}MAX {1}'",
                                                prefix,
                                                _MaxAlt
                                                )
                    End Select
                End If
            End If

            Return strRestrictions

        End Get
    End Property
    Public ReadOnly Property Elevation As String
        Get
            If SupportingFeatures.PrefUnits Is Nothing OrElse SupportingFeatures.PrefUnits.Altitude = AltitudeUnits.Both Then
                Return String.Format("{0}' ({1}m)", CInt(_wpelevation), Math.Round(Conversions.FeetToMeters(_wpelevation)), 0)
            Else
                Select Case SupportingFeatures.PrefUnits.Altitude
                    Case AltitudeUnits.Metric
                        Return String.Format("{0}m", Math.Round(Conversions.FeetToMeters(_wpelevation)), 0)
                    Case AltitudeUnits.Imperial
                        Return String.Format("{0}'", CInt(_wpelevation), 0)
                End Select
            End If
            Return String.Empty
        End Get
    End Property

    Public Property DistanceFromPreviousKM As Single = 0
    Public Property DistanceFromTaskStartKM As Single = 0
    Public Property DistanceFromDepartureKM As Single = 0

    Public ReadOnly Property DistanceFromPreviousMi As Single
        Get
            Return Conversions.KmToMiles(DistanceFromPreviousKM)
        End Get
    End Property

    Public ReadOnly Property DistanceFromTaskStartMi As Single
        Get
            Return Conversions.KmToMiles(DistanceFromTaskStartKM)
        End Get
    End Property

    Public ReadOnly Property DistanceFromDepartureMi As Single
        Get
            Return Conversions.KmToMiles(DistanceFromDepartureKM)
        End Get
    End Property

End Class