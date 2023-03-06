Public Class ATCWaypoint

    Private ReadOnly _MinAlt As Nullable(Of Integer) = Nothing
    Private ReadOnly _MaxAlt As Nullable(Of Integer) = Nothing

    Public ReadOnly WPName As String = String.Empty
    Public IsTaskStart As Boolean = False
    Public IsTaskEnd As Boolean = False

    Private Shared _TaskStartFound As Boolean = False

    Public Sub New(strFullATCId As String,
                   strWorldPosition As String,
                   intSeq As Integer)

        If intSeq = 0 Then
            _TaskStartFound = False
        End If

        Me.FullATCId = strFullATCId

        CheckTaskStartOrEnd()

        SetLatitudeAndLongitude(strWorldPosition, Latitude, Longitude)

        'Set name of waypoint
        If strFullATCId.Contains("+") OrElse strFullATCId.Contains("|") Then
            WPName = strFullATCId.Substring(0, strFullATCId.IndexOfAny("+|"))
            strFullATCId = strFullATCId.Substring(WPName.Length)
            If WPName.StartsWith("*") Then
                WPName = WPName.Substring(1, WPName.Length - 1)
            End If
        Else
            'No encoding
            WPName = strFullATCId
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

    Private Sub SetLatitudeAndLongitude(strWorldPosition As String, ByRef pLatitude As Double, ByRef pLongitude As Double)
        Dim strParts As String() = strWorldPosition.Split(",")
        pLatitude = Conversions.ConvertToLatitude(strParts(0))
        pLongitude = Conversions.ConvertToLongitude(strParts(1))
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

    Public ReadOnly Property Restrictions As String
        Get
            Dim strRestrictions As String = String.Empty

            If _MinAlt IsNot Nothing AndAlso _MaxAlt IsNot Nothing Then
                strRestrictions = String.Format("{0}: Between {1}' and {2}' ({3}m and {4}m)",
                                                WPName,
                                                _MinAlt,
                                                _MaxAlt,
                                                Int(Conversions.FeetToMeters(_MinAlt)),
                                                Int(Conversions.FeetToMeters(_MaxAlt))
                                                )
            ElseIf _MinAlt IsNot Nothing Then
                strRestrictions = String.Format("{0}: MIN {1}' ({2}m)",
                                                WPName,
                                                _MinAlt,
                                                Int(Conversions.FeetToMeters(_MinAlt))
                                                )
            ElseIf _MaxAlt IsNot Nothing Then
                strRestrictions = String.Format("{0}: MAX {1}' ({2}m)",
                                                WPName,
                                                _MaxAlt,
                                                Int(Conversions.FeetToMeters(_MaxAlt))
                                                )
            End If

            Return strRestrictions

        End Get
    End Property

End Class