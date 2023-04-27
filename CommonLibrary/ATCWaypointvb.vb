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
                Dim convImperial As Single = Conversions.KmToMiles(_Diameter / 1000)
                If convImperial < 1 Then
                    convImperial = Conversions.MeterToFeet(_Diameter)
                    diameter = $"{CInt(convImperial)}' ({_Diameter} m)"
                Else
                    diameter = $"{String.Format("{0:0.0}", convImperial)} mi ({_Diameter} m)"
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
        If strFullATCId.Contains("x") Then
            _Diameter = CInt(strFullATCId.Substring(strFullATCId.IndexOf("x") + 1))
        Else
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

            If strFullATCId.Contains("|") Then
                'Contains maximum
                If strFullATCId.Contains("/") Then
                    _MaxAlt = CInt(strFullATCId.Substring(strFullATCId.IndexOf("|") + 1, strFullATCId.IndexOfAny("/") - 1))
                Else
                    _MaxAlt = CInt(strFullATCId.Substring(strFullATCId.IndexOf("|") + 1))
                End If
            End If
            If strFullATCId.Contains("/") Then
                'Contains minimum
                _MinAlt = CInt(strFullATCId.Substring(strFullATCId.IndexOf("/") + 1))
            End If

        End If
    End Sub

    Private Sub SetLatitudeAndLongitude(strWorldPosition As String, ByRef pLatitude As Double, ByRef pLongitude As Double, ByRef pElevation As Double)
        Dim strParts As String() = strWorldPosition.Split(",")
        pLatitude = Conversions.ConvertToLatitude(strParts(0))
        pLongitude = Conversions.ConvertToLongitude(strParts(1))
        pElevation = CDbl(strParts(2))
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

    Public ReadOnly Property Restrictions As String
        Get
            Return Restrictions(False)
        End Get
    End Property

    Public ReadOnly Property Restrictions(includeName As Boolean) As String
        Get
            Dim strRestrictions As String = String.Empty

            Dim prefix As String = String.Empty
            If includeName Then
                prefix = String.Format("{0}: ", WaypointName)
            End If

            If _MinAlt IsNot Nothing AndAlso _MaxAlt IsNot Nothing Then
                strRestrictions = String.Format("{0}Between {1}' and {2}' ({3}m and {4}m)",
                                                prefix,
                                                _MinAlt,
                                                _MaxAlt,
                                                Int(Conversions.FeetToMeters(_MinAlt)),
                                                Int(Conversions.FeetToMeters(_MaxAlt))
                                                )
            ElseIf _MinAlt IsNot Nothing Then
                strRestrictions = String.Format("{0}MIN {1}' ({2}m)",
                                                prefix,
                                                _MinAlt,
                                                Int(Conversions.FeetToMeters(_MinAlt))
                                                )
            ElseIf _MaxAlt IsNot Nothing Then
                strRestrictions = String.Format("{0}MAX {1}' ({2}m)",
                                                prefix,
                                                _MaxAlt,
                                                Int(Conversions.FeetToMeters(_MaxAlt))
                                                )
            End If

            Return strRestrictions

        End Get
    End Property
    Public ReadOnly Property Elevation As String
        Get
            Return String.Format("{0}' ({1}m)", CInt(_wpelevation), Int(Conversions.FeetToMeters(_wpelevation)))
        End Get
    End Property

End Class