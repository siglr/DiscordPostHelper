Public Class Conversions

    Private Const constMeterToFeet As Double = 3.2808398950131
    Private Const constKmToMiles As Double = 0.6213711922
    Private Const constMilesToKm As Double = 1.609344
    Private Const constPaToInHg As Double = 0.000295301
    Private Const constKelvinTo As Decimal = 273.15
    Private Const constMeterToInches As Double = 39.370078740157
    Private Const constKnotsToKmh As Single = 1.852
    Private Const constEarthRadius As Double = 6371 ' Earth's radius in kilometers


    Public Shared Function FeetToMeters(feet As Decimal) As Decimal

        Return feet / constMeterToFeet

    End Function

    Public Shared Function KmToMiles(km As Decimal) As Decimal

        Return km * constKmToMiles

    End Function

    Public Shared Function MilesToKm(miles As Decimal) As Decimal

        Return miles * constMilesToKm

    End Function

    Public Shared Function PaToInHg(pa As Single) As Decimal
        Return pa * constPaToInHg
    End Function

    Public Shared Function KelvinToCelsius(kelvin As Single) As Decimal
        Return kelvin - constKelvinTo
    End Function

    Public Shared Function KelvinToFarenheit(kelvin As Single) As Decimal
        Return (kelvin - 273.15) * (9 / 5) + 32
    End Function

    Public Shared Function MeterToInches(meters As Single) As Decimal
        Return meters * constMeterToInches
    End Function

    Public Shared Function MeterToFeet(meters As Single) As Decimal
        Return meters * constMeterToFeet
    End Function

    Public Shared Function GetDistanceInKm(latitude1 As Double, longitude1 As Double, latitude2 As Double, longitude2 As Double) As Double

        Dim latRad1 As Double = latitude1 * (Math.PI / 180)
        Dim latRad2 As Double = latitude2 * (Math.PI / 180)
        Dim lonRad1 As Double = longitude1 * (Math.PI / 180)
        Dim lonRad2 As Double = longitude2 * (Math.PI / 180)

        Dim deltaLat As Double = (latitude2 - latitude1) * (Math.PI / 180)
        Dim deltaLon As Double = (longitude2 - longitude1) * (Math.PI / 180)

        Dim a As Double = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2) * Math.Cos(latRad1) * Math.Cos(latRad2)
        Dim c As Double = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a))
        Dim distanceInKm As Double = constEarthRadius * c

        Return distanceInKm
    End Function

    Public Shared Function ConvertToLatitude(coord As String) As Double
        Dim isNegative As Boolean = False
        If coord.StartsWith("S") Then
            isNegative = True
            coord = coord.Replace("S", "")
        ElseIf coord.StartsWith("N") Then
            coord = coord.Replace("N", "")
        End If
        coord = coord.Replace("°", " ") ' replace the degree symbol with a space
        coord = coord.Replace("'", " ") ' replace the minute symbol with a space
        Dim parts As String() = coord.Split(" "c)
        Dim degrees As Double = Double.Parse(parts(0))
        Dim minutes As Double = Double.Parse(parts(2))
        Dim seconds As Double = Double.Parse(parts(4).Substring(0, parts(4).Length - 1))
        Dim decimalDegrees As Double = degrees + (minutes / 60) + (seconds / 3600)
        If isNegative Then
            decimalDegrees *= -1
        End If
        Return decimalDegrees
    End Function

    Public Shared Function ConvertToLongitude(ByVal coord As String) As Double
        Dim isNegative As Boolean = False
        If coord.StartsWith("W") Then
            isNegative = True
            coord = coord.Replace("W", "")
        ElseIf coord.StartsWith("E") Then
            coord = coord.Replace("E", "")
        End If
        coord = coord.Replace("°", " ") ' replace the degree symbol with a space
        coord = coord.Replace("'", " ") ' replace the minute symbol with a space
        Dim parts As String() = coord.Split(" "c)
        Dim degrees As Double = Double.Parse(parts(0))
        Dim minutes As Double = Double.Parse(parts(2))
        Dim seconds As Double = Double.Parse(parts(4).Substring(0, parts(4).Length - 1))
        Dim decimalDegrees As Double = degrees + (minutes / 60) + (seconds / 3600)
        If isNegative Then
            decimalDegrees *= -1
        End If
        Return decimalDegrees
    End Function

    Public Shared Function KnotsToKmh(knots As Integer) As Single
        Return knots * constKnotsToKmh
    End Function

    Public Shared Function ConvertDateToUnixTimestamp(dateToConvert As DateTime) As Long
        Dim unixTimestamp As Long = CType((dateToConvert.ToUniversalTime() - New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, Long)
        Return unixTimestamp
    End Function

    Public Shared Function ConvertLocalToUTC(dateToConvert As DateTime) As DateTime
        Return TimeZoneInfo.ConvertTimeToUtc(dateToConvert, TimeZoneInfo.Local)
    End Function

    Public Shared Function ConvertUTCToLocal(dateToConvert As DateTime) As DateTime
        Dim localTimeZone As TimeZoneInfo = TimeZoneInfo.Local
        Dim localTime As DateTime = TimeZoneInfo.ConvertTimeFromUtc(dateToConvert, localTimeZone)

        ' Check if the local time zone is currently observing daylight saving time
        Dim isDst As Boolean = localTimeZone.IsDaylightSavingTime(localTime)

        ' If the local time zone is observing daylight saving time, subtract an hour from the local time
        If isDst Then
            Return localTime.AddHours(-1)
        Else
            Return localTime
        End If
    End Function

End Class
