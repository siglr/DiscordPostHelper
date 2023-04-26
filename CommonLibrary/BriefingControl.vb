Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web.SessionState
Imports System.Windows.Forms
Imports System.Xml

Public Class BriefingControl

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _SF As SupportingFeatures
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")

    Public Sub GenerateBriefing(supportFeat As SupportingFeatures, sessionData As AllData, unpackFolder As String)

        _SF = supportFeat

        'Load flight plan
        _XmlDocFlightPlan = New XmlDocument
        _XmlDocFlightPlan.Load(Path.Combine(unpackFolder, Path.GetFileName(sessionData.FlightPlanFilename)))
        Dim totalDistance As Integer
        Dim trackDistance As Integer
        Dim altitudeRestrictions As String = _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, trackDistance)

        'Load weather info
        _XmlDocWeatherPreset = New XmlDocument
        _XmlDocWeatherPreset.Load(Path.Combine(unpackFolder, Path.GetFileName(sessionData.WeatherFilename)))
        _WeatherDetails = Nothing
        _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        Dim dateFormat As String
        If sessionData.IncludeYear Then
            dateFormat = "MMMM dd, yyyy"
        Else
            dateFormat = "MMMM dd"
        End If

        Dim sb As New StringBuilder
        sb.Append("{\rtf1\ansi ")

        'Title
        sb.Append($"\b {sessionData.Title}\b0\line ")
        sb.Append("\line ")

        'Local MSFS date and time 
        sb.Append($"MSFS Local date & time is \b {sessionData.SimDate.ToString(dateFormat, _EnglishCulture)}, {sessionData.SimTime.ToString("hh:mm tt", _EnglishCulture)} {_SF.ValueToAppendIfNotEmpty(sessionData.SimDateTimeExtraInfo.Trim, True, True)}\b0\line ")

        'Flight plan
        sb.Append($"The flight plan to load is \b {Path.GetFileName(sessionData.FlightPlanFilename)}\b0\line ")

        sb.Append("\line ")

        'Departure airfield And runway
        sb.Append($"You will depart from \b {_SF.ValueToAppendIfNotEmpty(sessionData.DepartureICAO)}{_SF.ValueToAppendIfNotEmpty(sessionData.DepartureName, True)}{_SF.ValueToAppendIfNotEmpty(sessionData.DepartureExtra, True, True)}\b0\line ")

        'Arrival airfield And expected runway
        sb.Append($"You will land at \b {_SF.ValueToAppendIfNotEmpty(sessionData.ArrivalICAO)}{_SF.ValueToAppendIfNotEmpty(sessionData.ArrivalName, True)}{_SF.ValueToAppendIfNotEmpty(sessionData.ArrivalExtra, True, True)}\b0\line ")

        'Type of soaring
        Dim soaringType As String = GetSoaringTypesSelected(sessionData)
        If soaringType.Trim <> String.Empty OrElse sessionData.SoaringExtraInfo <> String.Empty Then
            sb.Append($"Soaring Type is \b {soaringType}{_SF.ValueToAppendIfNotEmpty(sessionData.SoaringExtraInfo, True, True)}\b0\line ")
        End If

        'Task distance And total distance
        sb.Append($"Distances are \b {_SF.GetDistance(totalDistance.ToString, trackDistance.ToString)}\b0\line ")

        'Approx. duration
        sb.Append($"Approx. duration should be \b {_SF.GetDuration(sessionData.DurationMin, sessionData.DurationMax)}{_SF.ValueToAppendIfNotEmpty(sessionData.DurationExtraInfo, True, True)}\b0\line ")

        'Recommended gliders
        If sessionData.RecommendedGliders.Trim <> String.Empty Then
            sb.Append($"Recommended gliders: \b {_SF.ValueToAppendIfNotEmpty(sessionData.RecommendedGliders)}\b0\line ")
        End If

        'Difficulty rating
        If sessionData.DifficultyRating.Trim <> String.Empty OrElse sessionData.DifficultyExtraInfo.Trim <> String.Empty Then
            sb.Append($"The difficulty is rated as \b {_SF.GetDifficulty(CInt(sessionData.DifficultyRating.Substring(0, 1)), sessionData.DifficultyExtraInfo, True)}\b0\line ")
        End If

        sb.Append("\line ")

        If _WeatherDetails IsNot Nothing Then
            'Weather info (temperature, baro pressure, precipitations)
            sb.Append($"The weather profile to load is \b {_WeatherDetails.PresetName}\b0\line ")
            If sessionData.WeatherSummary <> String.Empty Then
                sb.Append($"Weather summary: \b {_SF.ValueToAppendIfNotEmpty(sessionData.WeatherSummary)}\b0\line ")
            End If
            sb.Append($"The elevation measurement used is \b {_WeatherDetails.AltitudeMeasurement}\b0\line ")
            sb.Append($"The barometric pressure is \b {_WeatherDetails.MSLPressure}\b0\line ")
            sb.Append($"The temperature is \b {_WeatherDetails.MSLTemperature}\b0\line ")
            sb.Append($"The humidity index is \b {_WeatherDetails.Humidity}\b0\line ")
            If _WeatherDetails.HasPrecipitations Then
                sb.Append($"Precipitations: \b {_WeatherDetails.Precipitations}\b0\line ")
            End If

            sb.Append("\line ")

            'Winds
            sb.Append($"\b Wind Layers\b0\line ")
            Dim lines As String() = _WeatherDetails.WindLayers.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}\line ")
            Next

            sb.Append(" \line ")

            'Clouds
            sb.Append($"\b Cloud Layers\b0\line ")
            lines = _WeatherDetails.CloudLayers.Split(New String() {Environment.NewLine}, StringSplitOptions.None)
            For Each line In lines
                sb.Append($"{line}\line ")
            Next

        End If
        sb.Append("}")
        txtBriefing.Rtf = sb.ToString()

        sb.Clear()

        imgMap.Image = System.Drawing.Image.FromFile(Path.Combine(unpackFolder, Path.GetFileName(sessionData.MapImageSelected)))

    End Sub

    Private Function GetSoaringTypesSelected(sessionData As AllData) As String
        Dim types As String = String.Empty

        If sessionData.SoaringRidge And sessionData.SoaringThermals Then
            types = "Ridge and Thermals"
        ElseIf sessionData.SoaringRidge Then
            types = "Ridge"
        ElseIf sessionData.SoaringThermals Then
            types = "Thermals"
        End If

        Return types

    End Function

    Private Sub btnMapZoomIn_Click(sender As Object, e As EventArgs) 
        Dim ratio As Integer = imgMap.Size.Width * 100 / mapSplitterUpDown.Panel1.Size.Width
        If ratio <= 200 Then
            ' Increase the size of the picture box
            imgMap.Size = New Size(imgMap.Size.Width * 1.1, imgMap.Size.Height * 1.1)
            imgMap.SizeMode = PictureBoxSizeMode.Zoom
        End If

    End Sub

    Private Sub btnMapZoomOut_Click(sender As Object, e As EventArgs) 
        Dim ratio As Integer = imgMap.Size.Width * 100 / mapSplitterUpDown.Panel1.Size.Width
        If ratio >= 100 Then
            ' Decrease the size of the picture box
            imgMap.Size = New Size(imgMap.Size.Width * 0.9, imgMap.Size.Height * 0.9)
            imgMap.SizeMode = PictureBoxSizeMode.Zoom
        End If

    End Sub

End Class
