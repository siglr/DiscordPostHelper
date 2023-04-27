Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Linq.Expressions
Imports System.Text
Imports System.Web.SessionState
Imports System.Web.UI.WebControls
Imports System.Windows.Forms
Imports System.Xml

Public Class BriefingControl

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _SF As SupportingFeatures
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private _sessionData As AllData

    Public Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    Public Declare Function GetSystemMetrics Lib "user32.dll" (ByVal nIndex As Integer) As Integer
    Public Const GWL_STYLE As Integer = (-16)
    Public Const WS_VSCROLL As Integer = &H200000
    Public Const WS_HSCROLL As Integer = &H100000

    Public Sub GenerateBriefing(supportFeat As SupportingFeatures, sessionData As AllData, unpackFolder As String)

        _SF = supportFeat
        _sessionData = sessionData

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
        Dim soaringType As String = GetSoaringTypesSelected()
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
        SetZoomFactorOfRichTextBox(txtBriefing)

        sb.Clear()

        If sessionData.MapImageSelected = String.Empty Then
            imageViewer.Enabled = False
        Else
            imageViewer.LoadImage(sessionData.MapImageSelected)
        End If

        'Build full description
        If sessionData.LongDescription <> String.Empty Then
            sb.AppendLine("**Full Description**")
            sb.AppendLine(sessionData.LongDescription.Replace("($*$)", Environment.NewLine))
            _SF.FormatMarkdownToRTF(sb.ToString.Trim, txtFullDescription)

        End If

        'Build altitude restrictions
        sb.Clear()
        If altitudeRestrictions <> String.Empty Then
            sb.AppendLine(altitudeRestrictions)
            _SF.FormatMarkdownToRTF(sb.ToString, txtAltitudeRestrictions)
        End If

        'XBOX Coordinates
        waypointCoordinatesDataGrid.Font = New Font(waypointCoordinatesDataGrid.Font.FontFamily, 12)
        waypointCoordinatesDataGrid.RowTemplate.Height = 30
        waypointCoordinatesDataGrid.DataSource = _SF.AllWaypoints
        Dim gridColumn As DataGridViewColumn = waypointCoordinatesDataGrid.Columns("Latitude")
        gridColumn.DefaultCellStyle.Format = "N6"
        gridColumn = waypointCoordinatesDataGrid.Columns("Longitude")
        gridColumn.DefaultCellStyle.Format = "N6"
        waypointCoordinatesDataGrid.ColumnHeadersDefaultCellStyle.Font = New Font(waypointCoordinatesDataGrid.Font, FontStyle.Bold)

    End Sub

    Private Sub SetZoomFactorOfRichTextBox(rtfControl As RichTextBox)

        If rtfControl.Text.Trim = String.Empty Then
            Exit Sub
        End If

        Dim bVScrollBar As Boolean
        bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
        Select Case bVScrollBar
            Case True
                'Scrollbar is visible - Make it smaller
                Do
                    If (rtfControl.ZoomFactor) - 0.01 <= 0.015625 Then
                        Exit Do
                    End If
                    rtfControl.ZoomFactor = rtfControl.ZoomFactor - 0.01
                    bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
                    'If the scrollbar is no longer visible we are done!
                    If bVScrollBar = False Then Exit Do
                Loop
            Case False
                'Scrollbar is not visible - Make it bigger
                Do
                    If (rtfControl.ZoomFactor + 0.01) >= 64 Then
                        Exit Do
                    End If
                    rtfControl.ZoomFactor = rtfControl.ZoomFactor + 0.01
                    bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
                    If bVScrollBar = True Then
                        Do
                            'Found the scrollbar, make smaller until bar is not visible
                            If (rtfControl.ZoomFactor) - 0.01 <= 0.015625 Then
                                Exit Do
                            End If
                            rtfControl.ZoomFactor = rtfControl.ZoomFactor - 0.01
                            bVScrollBar = ((GetWindowLong(rtfControl.Handle, GWL_STYLE) And WS_VSCROLL) = WS_VSCROLL)
                            'If the scrollbar is no longer visible we are done!
                            If bVScrollBar = False Then Exit Do
                        Loop
                        Exit Do
                    End If
                Loop
        End Select

    End Sub

    Private Function GetSoaringTypesSelected() As String
        Dim types As String = String.Empty

        If _sessionData.SoaringRidge And _sessionData.SoaringThermals Then
            types = "Ridge and Thermals"
        ElseIf _sessionData.SoaringRidge Then
            types = "Ridge"
        ElseIf _sessionData.SoaringThermals Then
            types = "Thermals"
        End If

        Return types

    End Function

    Private Sub mapPictureBox_DoubleClick(sender As Object, e As EventArgs)

        'Launch the ImageViewer program
        Dim startInfo As New ProcessStartInfo($"{Application.StartupPath}\ImageViewer.exe", $"""{_sessionData.MapImageSelected}""")

        Process.Start(startInfo)


    End Sub

    Private Sub tabsBriefing_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabsBriefing.SelectedIndexChanged

        AdjustRTBoxControls()

    End Sub

    Public Sub AdjustRTBoxControls()

        Select Case tabsBriefing.SelectedIndex
            Case 0
                SetZoomFactorOfRichTextBox(txtBriefing)
            Case 1
                SetZoomFactorOfRichTextBox(txtFullDescription)
                SetZoomFactorOfRichTextBox(txtAltitudeRestrictions)
        End Select

    End Sub

    Private Sub mapSplitterLeftRight_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles mapSplitterLeftRight.SplitterMoved, mapSplitterUpDown.SplitterMoved
        AdjustRTBoxControls()
    End Sub
End Class
