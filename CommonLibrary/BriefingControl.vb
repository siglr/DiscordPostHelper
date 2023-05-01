Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml

Public Class BriefingControl

    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing
    Private _SF As SupportingFeatures
    Private ReadOnly _EnglishCulture As New CultureInfo("en-US")
    Private _sessionData As AllData
    Private _unpackFolder As String = String.Empty

    Public Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    Public Declare Function GetSystemMetrics Lib "user32.dll" (ByVal nIndex As Integer) As Integer
    Public Const GWL_STYLE As Integer = (-16)
    Public Const WS_VSCROLL As Integer = &H200000
    Public Const WS_HSCROLL As Integer = &H100000

    Public Sub FullReset()
        txtBriefing.Clear()
        imageViewer.Visible = False
        txtFullDescription.Clear()
        restrictionsDataGrid.DataSource = Nothing
        waypointCoordinatesDataGrid.DataSource = Nothing
        cboWayPointDistances.SelectedIndex = 0
        imagesListView.Clear()
    End Sub

    Public Sub ChangeImage(imgFilename As String)
        imageViewer.LoadImage(imgFilename)
    End Sub

    Public Sub GenerateBriefing(supportFeat As SupportingFeatures,
                                sessionData As AllData,
                                flightplanfile As String,
                                weatherfile As String,
                                Optional unpackFolder As String = "NONE")

        _SF = supportFeat
        _sessionData = sessionData
        If unpackFolder = "NONE" Then
            _unpackFolder = String.Empty
        Else
            _unpackFolder = unpackFolder
        End If

        'Load flight plan
        _XmlDocFlightPlan = New XmlDocument
        _XmlDocFlightPlan.Load(flightplanfile)
        Dim totalDistance As Integer
        Dim trackDistance As Integer
        Dim altitudeRestrictions As String = _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, trackDistance, False)

        'Load weather info
        _XmlDocWeatherPreset = New XmlDocument
        _XmlDocWeatherPreset.Load(weatherfile)
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
        sb.Append($"{sessionData.Credits}\line ")
        sb.Append("\line ")

        'Credits

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

        imageViewer.Visible = True
        If sessionData.MapImageSelected = String.Empty Then
            imageViewer.Enabled = False
        Else
            Dim filename As String = sessionData.MapImageSelected
            If _unpackFolder <> String.Empty Then
                filename = Path.Combine(_unpackFolder, Path.GetFileName(filename))
            End If
            imageViewer.LoadImage(filename)
        End If

        'Build full description
        If sessionData.LongDescription <> String.Empty Then
            sb.AppendLine("**Full Description**")
            sb.AppendLine(sessionData.LongDescription.Replace("($*$)", Environment.NewLine))
            _SF.FormatMarkdownToRTF(sb.ToString.Trim, txtFullDescription)

        End If

        'Build altitude restrictions
        Dim dt As New DataTable()
        dt.Columns.Add("Waypoint Name", GetType(String))
        dt.Columns.Add("Restrictions", GetType(String))
        For Each waypoint In _SF.AllWaypoints.Where(Function(x) x.ContainsRestriction)
            dt.Rows.Add(waypoint.WaypointName, waypoint.Restrictions)
        Next
        restrictionsDataGrid.DataSource = dt
        restrictionsDataGrid.Font = New Font(restrictionsDataGrid.Font.FontFamily, 12)
        restrictionsDataGrid.RowTemplate.Height = 28
        restrictionsDataGrid.RowHeadersVisible = False
        restrictionsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        'XBOX Coordinates
        waypointCoordinatesDataGrid.Font = New Font(waypointCoordinatesDataGrid.Font.FontFamily, 12)
        waypointCoordinatesDataGrid.RowTemplate.Height = 30
        waypointCoordinatesDataGrid.DataSource = _SF.AllWaypoints
        waypointCoordinatesDataGrid.Columns("Latitude").DefaultCellStyle.Format = "N6"
        waypointCoordinatesDataGrid.Columns("Longitude").DefaultCellStyle.Format = "N6"
        waypointCoordinatesDataGrid.ColumnHeadersDefaultCellStyle.Font = New Font(waypointCoordinatesDataGrid.Font, FontStyle.Bold)
        waypointCoordinatesDataGrid.Columns(0).HeaderText = "#"
        For i As Integer = 0 To waypointCoordinatesDataGrid.Columns.Count - 2
            waypointCoordinatesDataGrid.Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
        Next
        waypointCoordinatesDataGrid.RowHeadersWidth = 15
        'waypointCoordinatesDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        waypointCoordinatesDataGrid.AllowUserToResizeColumns = True
        waypointCoordinatesDataGrid.Columns("FullATCId").Visible = False
        waypointCoordinatesDataGrid.Columns("ContainsRestriction").Visible = False
        waypointCoordinatesDataGrid.Columns("Gate").HeaderText = "Gate diameter"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").HeaderText = "Previous (km)"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").HeaderText = "Total (km)"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").HeaderText = "Task (km)"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").HeaderText = "Previous (mi)"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").HeaderText = "Total (mi)"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").DefaultCellStyle.Format = "N1"
        waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").HeaderText = "Task (mi)"
        SetWPGridColumnsVisibility()

        'Images tab
        LoadImagesTabListView()

    End Sub

    Private Sub LoadImagesTabListView()

        Dim imageFilenameList As New List(Of String)

        Dim imgList As New ImageList
        For Each filename As String In _sessionData.ExtraFiles
            If Path.GetExtension(filename.ToLower) = ".png" OrElse Path.GetExtension(filename.ToLower) = ".jpg" Then
                If _unpackFolder <> String.Empty Then
                    filename = Path.Combine(_unpackFolder, Path.GetFileName(filename))
                End If
                imgList.Images.Add(Image.FromFile(filename))
                imageFilenameList.Add(filename)
            End If
        Next
        imgList.ImageSize = New Size(64, 64) 'set the size of the icons

        imagesListView.View = View.LargeIcon
        imagesListView.LargeImageList = imgList

        For i = 0 To imgList.Images.Count - 1
            Dim item1 As New ListViewItem($"")
            item1.ImageIndex = i
            item1.Text = i + 1.ToString
            item1.Tag = imageFilenameList(i)
            imagesListView.Items.Add(item1)
        Next

        If imagesListView.Items.Count > 0 Then
            imagesListView.Items(0).Selected = True
        End If

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
        End Select

    End Sub

    Private Sub mapSplitterLeftRight_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles mapSplitterLeftRight.SplitterMoved, mapSplitterUpDown.SplitterMoved
        AdjustRTBoxControls()
    End Sub

    Private Sub imagesListView_SelectedIndexChanged(sender As Object, e As EventArgs) Handles imagesListView.SelectedIndexChanged


        If imagesListView.SelectedItems.Count > 0 AndAlso imagesListView.SelectedItems(0).Tag <> String.Empty Then
            imagesTabViewerControl.LoadImage(imagesListView.SelectedItems(0).Tag)
        End If

    End Sub

    Private Sub cboWayPointDistances_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboWayPointDistances.SelectedIndexChanged
        SetWPGridColumnsVisibility()
    End Sub

    Private Sub SetWPGridColumnsVisibility()

        Try
            'Distances
            Select Case cboWayPointDistances.SelectedIndex
                Case 0 'None
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").Visible = False
                Case 1 'Kilometers
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").Visible = False
                Case 2 'Miles
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartKM").Visible = False
                    waypointCoordinatesDataGrid.Columns("DistanceFromPreviousMi").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromDepartureMi").Visible = True
                    waypointCoordinatesDataGrid.Columns("DistanceFromTaskStartMi").Visible = True

            End Select

            'Lat and Lon columns
            If chkWPEnableLatLonColumns.Checked Then
                waypointCoordinatesDataGrid.Columns("Latitude").Visible = True
                waypointCoordinatesDataGrid.Columns("Longitude").Visible = True
            Else
                waypointCoordinatesDataGrid.Columns("Latitude").Visible = False
                waypointCoordinatesDataGrid.Columns("Longitude").Visible = False
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub chkWPEnableLatLonColumns_CheckedChanged(sender As Object, e As EventArgs) Handles chkWPEnableLatLonColumns.CheckedChanged
        SetWPGridColumnsVisibility()
    End Sub
End Class
