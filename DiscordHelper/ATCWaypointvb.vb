Public Class ATCWaypoint

    Private strName As String = String.Empty
    Private strData As String = String.Empty
    Private strFullATCId As String = String.Empty
    Private intMinAlt As Nullable(Of Integer) = Nothing
    Private intMaxAlt As Nullable(Of Integer) = Nothing
    Private dblLatitude As Double = 0
    Private dblLongitude As Double = 0
    Private intSequence As Integer = 0
    Private blnContainsRestriction As Boolean = False

    Public IsTaskStart As Boolean = False
    Public IsTaskEnd As Boolean = False

    Private Shared blnTaskStartFound As Boolean = False

    Public Sub New(fullATCId As String, WorldPosition As String, seq As Integer)

        If seq = 0 Then
            blnTaskStartFound = False
        End If

        strFullATCId = fullATCId

        If strFullATCId.StartsWith("*") Then
            If Not blnTaskStartFound Then
                blnTaskStartFound = True
                IsTaskStart = True
            Else
                IsTaskEnd = True
            End If
        End If

        Dim parts As String() = WorldPosition.Split(",")
        dblLatitude = Conversions.ConvertToLatitude(parts(0))
        dblLongitude = Conversions.ConvertToLongitude(parts(1))

        If fullATCId.Contains("+") Or fullATCId.Contains("|") Then
            strName = fullATCId.Substring(0, fullATCId.IndexOfAny("+|"))
            fullATCId = fullATCId.Substring(strName.Length)
            If strName.StartsWith("*") Then
                strName = strName.Substring(1, strName.Length - 1)
            End If
        Else
            'No encoding
        End If

        'Remove radius part (when only radius is specified)
        If fullATCId.Contains("|x") Then
            fullATCId = fullATCId.Substring(0, fullATCId.IndexOf("|"))
        End If
        'Remove radius part
        If fullATCId.Contains("x") Then
            fullATCId = fullATCId.Substring(0, fullATCId.IndexOf("x"))
        End If

        'Check if waypoint contains encoding valid restriction data
        If fullATCId.Contains("|") Or fullATCId.Contains("/") Then

            blnContainsRestriction = True

            'Remove elevation part
            If fullATCId.Contains("+") Then
                fullATCId = fullATCId.Substring(fullATCId.IndexOfAny("/|"))
            End If

            If fullATCId.Contains("|") Then
                'Contains maximum
                If fullATCId.Contains("/") Then
                    intMaxAlt = CInt(fullATCId.Substring(fullATCId.IndexOf("|") + 1, fullATCId.IndexOfAny("/") - 1))
                Else
                    intMaxAlt = CInt(fullATCId.Substring(fullATCId.IndexOf("|") + 1))
                End If
            End If
            If fullATCId.Contains("/") Then
                'Contains minimum
                intMinAlt = CInt(fullATCId.Substring(fullATCId.IndexOf("/") + 1))
            End If

        End If
    End Sub

    Public ReadOnly Property FullATCId As String
        Get
            Return strFullATCId
        End Get
    End Property

    Public ReadOnly Property ContainsRestriction As Boolean
        Get
            Return blnContainsRestriction
        End Get
    End Property

    Public ReadOnly Property Latitude As Double
        Get
            Return dblLatitude
        End Get
    End Property

    Public ReadOnly Property Longitude As Double
        Get
            Return dblLongitude
        End Get
    End Property

    Public ReadOnly Property Restrictions As String
        Get
            Dim strRestrictions As String = String.Empty

            If intMinAlt Is Nothing And intMaxAlt Is Nothing Then
                'Do nothing
            ElseIf (Not intMinAlt Is Nothing) And intMaxAlt Is Nothing Then
                'Minimum only
                strRestrictions = strName & ": MIN " & intMinAlt.ToString() & "' (" & Int(Conversions.FeetToMeters(intMinAlt)).ToString & "m)"
            ElseIf (Not intMaxAlt Is Nothing) And intMinAlt Is Nothing Then
                'Maximum only
                strRestrictions = strName & ": MAX " & intMaxAlt.ToString() & "' (" & Int(Conversions.FeetToMeters(intMaxAlt)).ToString & "m)"
            Else
                'Both minimum and maximum
                strRestrictions = strName & ": Between " & intMinAlt.ToString() & "' (" & Int(Conversions.FeetToMeters(intMinAlt)).ToString & "m) and " & intMaxAlt.ToString() & "' (" & Int(Conversions.FeetToMeters(intMaxAlt)).ToString & "m)"
            End If

            Return strRestrictions

        End Get
    End Property

End Class