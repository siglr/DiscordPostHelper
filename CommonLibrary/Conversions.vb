Imports System.Globalization

Public Class Conversions

    Private Const constMeterToFeet As Double = 3.2808398950131
    Private Const constKmToMiles As Double = 0.6213711922
    Private Const constMilesToKm As Double = 1.609344
    Private Const constPaToInHg As Double = 0.000295301
    Private Const constKelvinTo As Decimal = 273.15
    Private Const constMeterToInches As Double = 39.370078740157
    Private Const constKnotsToKmh As Single = 1.852
    Private Const constEarthRadius As Double = 6371 ' Earth's radius in kilometers

    Private Shared Function TryParseDecimal(value As String, ByRef parsedValue As Decimal) As Boolean
        Dim style As NumberStyles = NumberStyles.Float Or NumberStyles.AllowThousands
        If String.IsNullOrWhiteSpace(value) Then
            Return False
        End If
        Dim trimmedValue As String = value.Trim()
        Dim currentCulture As CultureInfo = CultureInfo.CurrentCulture
        If Decimal.TryParse(trimmedValue, style, currentCulture, parsedValue) Then
            Return True
        End If
        Dim normalizedValue As String = NormalizeNumberString(trimmedValue, currentCulture.NumberFormat)
        If Not String.Equals(trimmedValue, normalizedValue, StringComparison.Ordinal) AndAlso Decimal.TryParse(normalizedValue, style, currentCulture, parsedValue) Then
            Return True
        End If
        Return Decimal.TryParse(trimmedValue, style, CultureInfo.InvariantCulture, parsedValue)
    End Function

    Private Shared Function TryParseSingle(value As String, ByRef parsedValue As Single) As Boolean
        Dim style As NumberStyles = NumberStyles.Float Or NumberStyles.AllowThousands
        If String.IsNullOrWhiteSpace(value) Then
            Return False
        End If
        Dim trimmedValue As String = value.Trim()
        Dim currentCulture As CultureInfo = CultureInfo.CurrentCulture
        If Single.TryParse(trimmedValue, style, currentCulture, parsedValue) Then
            Return True
        End If
        Dim normalizedValue As String = NormalizeNumberString(trimmedValue, currentCulture.NumberFormat)
        If Not String.Equals(trimmedValue, normalizedValue, StringComparison.Ordinal) AndAlso Single.TryParse(normalizedValue, style, currentCulture, parsedValue) Then
            Return True
        End If
        Return Single.TryParse(trimmedValue, style, CultureInfo.InvariantCulture, parsedValue)
    End Function

    Private Shared Function NormalizeNumberString(value As String, numberFormat As NumberFormatInfo) As String
        Dim trimmedValue As String = value.Trim()
        Dim lastDotIndex As Integer = trimmedValue.LastIndexOf("."c)
        Dim lastCommaIndex As Integer = trimmedValue.LastIndexOf(","c)
        If lastDotIndex = -1 AndAlso lastCommaIndex = -1 Then
            Return trimmedValue
        End If
        Dim decimalSymbol As Char = If(lastDotIndex > lastCommaIndex, "."c, ","c)
        Dim groupSymbol As Char = If(decimalSymbol = "."c, ","c, "."c)
        Dim normalizedValue As String = trimmedValue.Replace(groupSymbol.ToString(), String.Empty)
        Dim decimalSeparator As String = numberFormat.NumberDecimalSeparator
        If decimalSeparator <> decimalSymbol.ToString() Then
            normalizedValue = normalizedValue.Replace(decimalSymbol.ToString(), decimalSeparator)
        End If
        Return normalizedValue
    End Function

    Public Shared Function FeetToMeters(feet As Decimal) As Decimal

        Return feet / constMeterToFeet

    End Function

    Public Shared Function FeetToMeters(feet As String) As Decimal
        Dim feetValue As Decimal
        If Not TryParseDecimal(feet, feetValue) Then
            Return 0
        End If
        Return FeetToMeters(feetValue)
    End Function

    Public Shared Function KmToMiles(km As Decimal) As Decimal

        Return km * constKmToMiles

    End Function

    Public Shared Function KmToMiles(km As String) As Decimal
        Dim kmValue As Decimal
        If Not TryParseDecimal(km, kmValue) Then
            Return 0
        End If
        Return KmToMiles(kmValue)
    End Function

    Public Shared Function MilesToKm(miles As Decimal) As Decimal

        Return miles * constMilesToKm

    End Function

    Public Shared Function MilesToKm(miles As String) As Decimal
        Dim milesValue As Decimal
        If Not TryParseDecimal(miles, milesValue) Then
            Return 0
        End If
        Return MilesToKm(milesValue)
    End Function

    Public Shared Function PaToInHg(pa As Single) As Decimal
        Return pa * constPaToInHg
    End Function

    Public Shared Function PaToInHg(pa As String) As Decimal
        Dim paValue As Single
        If Not TryParseSingle(pa, paValue) Then
            Return 0
        End If
        Return PaToInHg(paValue)
    End Function

    Public Shared Function KelvinToCelsius(kelvin As Single) As Decimal
        Return kelvin - constKelvinTo
    End Function

    Public Shared Function KelvinToCelsius(kelvin As String) As Decimal
        Dim kelvinValue As Single
        If Not TryParseSingle(kelvin, kelvinValue) Then
            Return 0
        End If
        Return KelvinToCelsius(kelvinValue)
    End Function

    Public Shared Function KelvinToFarenheit(kelvin As Single) As Decimal
        Return (kelvin - 273.15) * (9 / 5) + 32
    End Function

    Public Shared Function KelvinToFarenheit(kelvin As String) As Decimal
        Dim kelvinValue As Single
        If Not TryParseSingle(kelvin, kelvinValue) Then
            Return 0
        End If
        Return KelvinToFarenheit(kelvinValue)
    End Function

    Public Shared Function MeterToInches(meters As Single) As Decimal
        Return meters * constMeterToInches
    End Function

    Public Shared Function MeterToInches(meters As String) As Decimal
        Dim meterValue As Single
        If Not TryParseSingle(meters, meterValue) Then
            Return 0
        End If
        Return MeterToInches(meterValue)
    End Function

    Public Shared Function MeterToFeet(meters As Single) As Decimal
        Return meters * constMeterToFeet
    End Function

    Public Shared Function MeterToFeet(meters As String) As Decimal
        Dim meterValue As Single
        If Not TryParseSingle(meters, meterValue) Then
            Return 0
        End If
        Return MeterToFeet(meterValue)
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
        Dim degrees As Double = Double.Parse(parts(0), CultureInfo.InvariantCulture)
        Dim minutes As Double = Double.Parse(parts(2), CultureInfo.InvariantCulture)
        Dim seconds As Double = Double.Parse(parts(4).Substring(0, parts(4).Length - 1), CultureInfo.InvariantCulture)
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
        Dim degrees As Double = Double.Parse(parts(0), CultureInfo.InvariantCulture)
        Dim minutes As Double = Double.Parse(parts(2), CultureInfo.InvariantCulture)
        Dim seconds As Double = Double.Parse(parts(4).Substring(0, parts(4).Length - 1), CultureInfo.InvariantCulture)
        Dim decimalDegrees As Double = degrees + (minutes / 60) + (seconds / 3600)
        If isNegative Then
            decimalDegrees *= -1
        End If
        Return decimalDegrees
    End Function

    Public Shared Function KnotsToKmh(knots As Integer) As Single
        Return knots * constKnotsToKmh
    End Function

    ''' <summary>
    ''' Converts a speed string in “km/h” into miles per hour.
    ''' Returns 0 on parse failure.
    ''' </summary>
    Public Shared Function KmhToMph(kmhString As String) As Single
        Dim kmh As Single
        ' TryParse will use the current culture’s decimal separator
        If TryParseSingle(kmhString, kmh) Then
            ' 1 km/h ≈ 0.621371192 mph
            Return kmh * 0.6213712F
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Converts a speed string in “km/h” into knots.
    ''' Returns 0 on parse failure.
    ''' </summary>
    Public Shared Function KmhToKnots(kmhString As String) As Single
        Dim kmh As Single
        If TryParseSingle(kmhString, kmh) Then
            ' 1 knot = 1.852 km/h  ⇒  knots = km/h ÷ 1.852
            Return kmh / constKnotsToKmh
        End If
        Return 0
    End Function


    Public Shared Function ConvertDateToUnixTimestamp(dateToConvert As DateTime) As Long
        Dim unixTimestamp As Long = CType((dateToConvert.ToUniversalTime() - New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, Long)
        Return unixTimestamp
    End Function

    Public Shared Function ConvertLocalToUTC(dateToConvert As DateTime) As DateTime
        Dim localTimeZone As TimeZoneInfo = TimeZoneInfo.Local
        Dim utcTime As DateTime = TimeZoneInfo.ConvertTimeToUtc(dateToConvert, localTimeZone)

        Return utcTime

    End Function

    Public Shared Function ConvertUTCToLocal(dateToConvert As DateTime) As DateTime
        Dim localTimeZone As TimeZoneInfo = TimeZoneInfo.Local
        Dim localTime As DateTime = TimeZoneInfo.ConvertTimeFromUtc(dateToConvert, localTimeZone)

        Return localTime

    End Function

    Public Shared Function KnotsToMps(knots As Integer) As Single
        Dim kmh As Single = KnotsToKmh(knots)
        Dim mps As Single = kmh * 1000 / 3600
        Return mps
    End Function

End Class
