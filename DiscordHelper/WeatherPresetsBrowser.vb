Imports System.IO
Imports System.Windows.Controls
Imports SIGLR.SoaringTools.CommonLibrary

Public Class WeatherPresetsBrowser

    Private _processingDisplayChanges As Boolean = False
    Private _allDefaultWeatherPresets As New Dictionary(Of Integer, WeatherPreset)
    Private _allUserDefinedWeatherPresets As New Dictionary(Of Integer, WeatherPreset)
    Private _weatherPresetsBaseDirectory As String

    Private ReadOnly Property PrefUnits As New PreferredUnits

#Region "Events"
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub WeatherPresetsBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim appDirectory As String = AppDomain.CurrentDomain.BaseDirectory
        _weatherPresetsBaseDirectory = Path.Combine(appDirectory, "WeatherPresets")
        Dim seq As Integer = 0

        'Load all presets
        'Look up all Default first
        LoadUpAllPresetsInFolder($"{Path.Combine(_weatherPresetsBaseDirectory, "Default")}", False, seq)
        'Look up all UserDefined second
        LoadUpAllPresetsInFolder($"{Path.Combine(_weatherPresetsBaseDirectory, "UserDefined")}", True, seq)

        'Load the DataGrid
        Dim dt As New DataTable()
        dt.Columns.Add("ID", GetType(Integer))          '0
        dt.Columns.Add("Source", GetType(String))       '1
        dt.Columns.Add("Title", GetType(String))        '2
        dt.Columns.Add("Types", GetType(String))        '3
        dt.Columns.Add("Rating", GetType(String))       '4
        dt.Columns.Add("Strength", GetType(String))     '5
        dt.Columns.Add("Primary", GetType(String))      '6
        'Default
        For Each preset As KeyValuePair(Of Integer, WeatherPreset) In _allDefaultWeatherPresets
            If Not preset.Value.UserData.Hidden Then
                dt.Rows.Add(preset.Key,
                        "Default",
                        preset.Value.BuiltInData.Title,
                        preset.Value.BuiltInData.AllSoaringTypes,
                        preset.Value.UserData.Rating,
                        preset.Value.UserData.OverallStrengthScale,
                        preset.Value.UserData.AllSoaringTypes)
            End If
        Next
        'User Defined
        For Each preset As KeyValuePair(Of Integer, WeatherPreset) In _allUserDefinedWeatherPresets
            If Not preset.Value.UserData.Hidden Then
                dt.Rows.Add(preset.Key,
                        "User",
                        preset.Value.BuiltInData.Title,
                        preset.Value.BuiltInData.AllSoaringTypes,
                        preset.Value.UserData.Rating,
                        preset.Value.UserData.OverallStrengthScale,
                        preset.Value.UserData.AllSoaringTypes)
            End If
        Next
        weatherProfilesDataGrid.DataSource = dt
        weatherProfilesDataGrid.Font = New Font(weatherProfilesDataGrid.Font.FontFamily, 10)
        weatherProfilesDataGrid.RowTemplate.Height = 28
        weatherProfilesDataGrid.RowHeadersVisible = True
        weatherProfilesDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        weatherProfilesDataGrid.Columns(0).Visible = False
        weatherProfilesDataGrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        weatherProfilesDataGrid.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        weatherProfilesDataGrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        weatherProfilesDataGrid.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        weatherProfilesDataGrid.Columns(5).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
        weatherProfilesDataGrid.Columns(6).AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader

    End Sub

    Private Sub visibilityOptions_CheckedChanged(sender As Object, e As EventArgs) Handles optGridOnly.CheckedChanged,
                                                                                        optGridDetails.CheckedChanged,
                                                                                        optDetailsOnly.CheckedChanged

        If optGridOnly.Checked Then
            splitContainerWeatherPresetManager.Panel1Collapsed = False
            splitContainerWeatherPresetManager.Panel2Collapsed = True
        ElseIf optDetailsOnly.Checked Then
            splitContainerWeatherPresetManager.Panel2Collapsed = False
            splitContainerWeatherPresetManager.Panel1Collapsed = True
        Else
            splitContainerWeatherPresetManager.Panel1Collapsed = False
            splitContainerWeatherPresetManager.Panel2Collapsed = False
        End If

    End Sub

    Private Sub weatherProfilesDataGrid_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles weatherProfilesDataGrid.RowEnter

        Dim idPreset As Integer = weatherProfilesDataGrid.Rows(e.RowIndex).Cells(0).Value
        Dim source As String = weatherProfilesDataGrid.Rows(e.RowIndex).Cells(1).Value
        Dim preset As WeatherPreset = Nothing

        Select Case source
            Case "Default"
                preset = _allDefaultWeatherPresets(idPreset)
                txtPresetTitle.ReadOnly = True
                txtPresetDescription.ReadOnly = True
                chkSoaringTypeRidge.Enabled = False
                chkSoaringTypeThermal.Enabled = False
                chkSoaringTypeWave.Enabled = False
                chkSoaringTypeDynamic.Enabled = False
            Case "User"
                preset = _allUserDefinedWeatherPresets(idPreset)
                txtPresetTitle.ReadOnly = False
                txtPresetDescription.ReadOnly = False
                chkSoaringTypeRidge.Enabled = True
                chkSoaringTypeThermal.Enabled = True
                chkSoaringTypeWave.Enabled = True
                chkSoaringTypeDynamic.Enabled = True
        End Select

        'Preset builtin data
        txtPresetTitle.Text = preset.BuiltInData.Title
        txtPresetDescription.Text = preset.BuiltInData.Description
        chkSoaringTypeRidge.Checked = preset.BuiltInData.SoaringTypeRidge
        chkSoaringTypeThermal.Checked = preset.BuiltInData.SoaringTypeThermal
        chkSoaringTypeWave.Checked = preset.BuiltInData.SoaringTypeWave
        chkSoaringTypeDynamic.Checked = preset.BuiltInData.SoaringTypeDynamic

        'Preset user data
        trkRating.Value = preset.UserData.Rating
        trkStrength.Value = preset.UserData.OverallStrengthScale
        txtComments.Text = preset.UserData.Comments
        chkPrimaryRidge.Checked = preset.UserData.SoaringTypeRidge
        chkPrimaryThermal.Checked = preset.UserData.SoaringTypeThermal
        chkPrimaryWave.Checked = preset.UserData.SoaringTypeWave
        chkPrimaryDynamic.Checked = preset.UserData.SoaringTypeDynamic

        'Graph
        FullWeatherGraphPanel1.SetWeatherInfo(preset.ProfileDetails, PrefUnits, String.Empty)

        'Load the DataGrid
        Dim dt As New DataTable()
        dt.Columns.Add("Name", GetType(String))       '0
        dt.Columns.Add("Value", GetType(String))       '1
        dt.Rows.Add("Name", preset.ProfileDetails.PresetName)
        dt.Rows.Add("Altitude Measurement", preset.ProfileDetails.AltitudeMeasurement)
        dt.Rows.Add("Humidity Index", preset.ProfileDetails.Humidity)
        dt.Rows.Add("MSL Barometric Pressure", preset.ProfileDetails.MSLPressure("Non standard", False, PrefUnits, True))
        dt.Rows.Add("MSL Temperature", preset.ProfileDetails.MSLTemperature(PrefUnits))
        dt.Rows.Add("Precipitations", preset.ProfileDetails.Precipitations)
        dt.Rows.Add("Snow Cover", preset.ProfileDetails.SnowCover)
        dt.Rows.Add("Thunderstorm Intensity", preset.ProfileDetails.ThunderstormIntensity)
        weatherDetailsDataGrid.DataSource = dt
        weatherDetailsDataGrid.Font = New Font(weatherDetailsDataGrid.Font.FontFamily, 10)
        weatherDetailsDataGrid.RowTemplate.Height = 28
        weatherDetailsDataGrid.RowHeadersVisible = False
        weatherDetailsDataGrid.ColumnHeadersVisible = False
        weatherDetailsDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        weatherDetailsDataGrid.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        weatherDetailsDataGrid.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        weatherDetailsDataGrid.BackgroundColor = weatherDetailsDataGrid.DefaultCellStyle.BackColor

        SupportingFeatures.BuildCloudAndWindLayersDatagrids(preset.ProfileDetails, windLayersDatagrid, cloudLayersDatagrid, PrefUnits)

        AdjustLastRowHeight(weatherDetailsDataGrid)
        AdjustLastRowHeight(windLayersDatagrid)
        AdjustLastRowHeight(cloudLayersDatagrid)

        SetWindLayers3DDisplay(preset)
        trkCameraTrack_Scroll(Me, New EventArgs)

        If preset.ProfileDetails.MultiDirections Then
            lblWindDirectionMulti.Enabled = True
            trkWindDirectionMulti.Enabled = True
            lblWindDirectionMultiLabel.Enabled = True
            lblWindDirectionSingle.Enabled = False
            trkWindDirectionSingle.Enabled = False
            lblWindDirectionSingleLabel.Enabled = False
            trkWindDirectionSingle.Value = 0
            trkWindDirectionMulti.Value = 0
            btnWindDirN.Enabled = False
            btnWindDirNE.Enabled = False
            btnWindDirE.Enabled = False
            btnWindDirSE.Enabled = False
            btnWindDirS.Enabled = False
            btnWindDirSW.Enabled = False
            btnWindDirW.Enabled = False
            btnWindDirNW.Enabled = False
        Else
            lblWindDirectionMulti.Enabled = False
            trkWindDirectionMulti.Enabled = False
            lblWindDirectionMultiLabel.Enabled = False
            lblWindDirectionSingle.Enabled = True
            trkWindDirectionSingle.Enabled = True
            lblWindDirectionSingleLabel.Enabled = True
            trkWindDirectionMulti.Value = 0
            trkWindDirectionSingle.Value = preset.ProfileDetails.WindLayers(0).Angle
            btnWindDirN.Enabled = True
            btnWindDirNE.Enabled = True
            btnWindDirE.Enabled = True
            btnWindDirSE.Enabled = True
            btnWindDirS.Enabled = True
            btnWindDirSW.Enabled = True
            btnWindDirW.Enabled = True
            btnWindDirNW.Enabled = True
        End If
        lblWindDirectionSingle.Text = trkWindDirectionSingle.Value.ToString
        lblWindDirectionSingle.Tag = trkWindDirectionSingle.Value
        lblWindDirectionSingleLabel.ForeColor = GroupBox2.ForeColor

        lblWindDirectionMulti.Text = trkWindDirectionMulti.Value.ToString
        lblWindDirectionMultiLabel.ForeColor = GroupBox2.ForeColor

        trkHumidityIndex.Value = preset.ProfileDetails.Humidity * 100
        lblHumidityIndex.Text = (trkHumidityIndex.Value / 100).ToString("0.00")
        lblHumidityIndex.Tag = trkHumidityIndex.Value
        lblHumidityIndexLabel.ForeColor = GroupBox2.ForeColor

        trkBaroPressure.Value = preset.ProfileDetails._MSLPressureInPa * 1000
        lblBarometricPressure.Text = SupportingFeatures.GetMSLPressure(trkBaroPressure.Value / 1000, PrefUnits)
        lblBarometricPressure.Tag = trkBaroPressure.Value
        lblBarometricPressureLabel.ForeColor = GroupBox2.ForeColor

        trkTemperature.Value = preset.ProfileDetails._MSLTempKelvin * 1000
        lblTemperature.Text = SupportingFeatures.MSLTemperature(trkTemperature.Value / 1000, PrefUnits)
        lblTemperature.Tag = trkTemperature.Value
        lblTemperatureLabel.ForeColor = GroupBox2.ForeColor

        trkPrecipitations.Value = preset.ProfileDetails._Precipitations * 1000
        lblPrecipitations.Text = $"{(trkPrecipitations.Value / 1000)} mm/h"
        lblPrecipitations.Tag = trkPrecipitations.Value
        lblPrecipitationsLabel.ForeColor = GroupBox2.ForeColor

        trkSnowCover.Value = preset.ProfileDetails._SnowCover * 1000
        lblSnowCover.Text = SupportingFeatures.GetSnowCover(trkSnowCover.Value / 1000)
        lblSnowCover.Tag = trkSnowCover.Value
        lblSnowCoverLabel.ForeColor = GroupBox2.ForeColor

        trkThunderstormIntensity.Value = preset.ProfileDetails.ThunderstormIntensity
        lblThunderstormIntensity.Text = $"{(trkThunderstormIntensity.Value)} %"
        lblThunderstormIntensity.Tag = trkThunderstormIntensity.Value
        lblThunderstormIntensityLabel.ForeColor = GroupBox2.ForeColor

    End Sub

    Private Sub GridResize(sender As Object, e As EventArgs) Handles weatherDetailsDataGrid.Resize,
                                                                     windLayersDatagrid.Resize,
                                                                     cloudLayersDatagrid.Resize

        AdjustLastRowHeight(sender)

    End Sub

    Private Sub trkWindDirectionSingle_Scroll(sender As Object, e As EventArgs) Handles trkWindDirectionSingle.Scroll
        WindLayers3DControl1.SetAllLayersDirection(trkWindDirectionSingle.Value)
        lblWindDirectionSingle.Text = trkWindDirectionSingle.Value.ToString

        If trkWindDirectionSingle.Value <> lblWindDirectionSingle.Tag Then
            lblWindDirectionSingleLabel.ForeColor = Color.Red
        Else
            lblWindDirectionSingleLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblWindDirectionSingleLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblWindDirectionSingleLabel.DoubleClick
        trkWindDirectionSingle.Value = lblWindDirectionSingle.Tag
        trkWindDirectionSingle_Scroll(sender, e)
    End Sub

    Private Sub trkWindDirectionMulti_Scroll(sender As Object, e As EventArgs) Handles trkWindDirectionMulti.Scroll
        WindLayers3DControl1.ShiftAllLayersDirection(-trkWindDirectionMulti.Value)
        lblWindDirectionMulti.Text = trkWindDirectionMulti.Value.ToString

        If trkWindDirectionMulti.Value <> 0 Then
            lblWindDirectionMultiLabel.ForeColor = Color.Red
        Else
            lblWindDirectionMultiLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblWindDirectionMultiLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblWindDirectionMultiLabel.DoubleClick
        trkWindDirectionMulti.Value = 0
        trkWindDirectionMulti_Scroll(sender, e)
    End Sub

    Private Sub trkHumidityIndex_Scroll(sender As Object, e As EventArgs) Handles trkHumidityIndex.Scroll
        lblHumidityIndex.Text = (trkHumidityIndex.Value / 100).ToString("0.00")

        If trkHumidityIndex.Value <> lblHumidityIndex.Tag Then
            lblHumidityIndexLabel.ForeColor = Color.Red
        Else
            lblHumidityIndexLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblHumidityIndexLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblHumidityIndexLabel.DoubleClick
        trkHumidityIndex.Value = lblHumidityIndex.Tag
        trkHumidityIndex_Scroll(sender, e)
    End Sub

    Private Sub trkBaroPressure_Scroll(sender As Object, e As EventArgs) Handles trkBaroPressure.Scroll
        lblBarometricPressure.Text = SupportingFeatures.GetMSLPressure(trkBaroPressure.Value / 1000, PrefUnits)

        If trkBaroPressure.Value <> lblBarometricPressure.Tag Then
            lblBarometricPressureLabel.ForeColor = Color.Red
        Else
            lblBarometricPressureLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblBarometricPressureLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblBarometricPressureLabel.DoubleClick
        trkBaroPressure.Value = lblBarometricPressure.Tag
        trkBaroPressure_Scroll(sender, e)
    End Sub

    Private Sub lblBarometricPressure_DoubleClick(sender As Object, e As EventArgs) Handles lblBarometricPressure.DoubleClick
        trkBaroPressure.Value = 101320750
        lblBarometricPressure.Text = SupportingFeatures.GetMSLPressure(trkBaroPressure.Value / 1000, PrefUnits)
    End Sub

    Private Sub trkTemperature_Scroll(sender As Object, e As EventArgs) Handles trkTemperature.Scroll
        lblTemperature.Text = SupportingFeatures.MSLTemperature(trkTemperature.Value / 1000, PrefUnits)

        If trkTemperature.Value <> lblTemperature.Tag Then
            lblTemperatureLabel.ForeColor = Color.Red
        Else
            lblTemperatureLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblTemperatureLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblTemperatureLabel.DoubleClick
        trkTemperature.Value = lblTemperature.Tag
        trkTemperature_Scroll(sender, e)
    End Sub

    Private Sub trkSnowCover_Scroll(sender As Object, e As EventArgs) Handles trkSnowCover.Scroll
        lblSnowCover.Text = SupportingFeatures.GetSnowCover(trkSnowCover.Value / 1000)

        If trkSnowCover.Value <> lblSnowCover.Tag Then
            lblSnowCoverLabel.ForeColor = Color.Red
        Else
            lblSnowCoverLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblSnowCoverLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblSnowCoverLabel.DoubleClick
        trkSnowCover.Value = lblSnowCover.Tag
        trkSnowCover_Scroll(sender, e)
    End Sub

    Private Sub trkPrecipitations_Scroll(sender As Object, e As EventArgs) Handles trkPrecipitations.Scroll
        lblPrecipitations.Text = $"{(trkPrecipitations.Value / 1000)} mm/h"

        If trkPrecipitations.Value <> lblPrecipitations.Tag Then
            lblPrecipitationsLabel.ForeColor = Color.Red
        Else
            lblPrecipitationsLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblPrecipitationsLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblPrecipitationsLabel.DoubleClick
        trkPrecipitations.Value = lblPrecipitations.Tag
        trkPrecipitations_Scroll(sender, e)
    End Sub

    Private Sub trkThunderstormIntensity_Scroll(sender As Object, e As EventArgs) Handles trkThunderstormIntensity.Scroll
        lblThunderstormIntensity.Text = $"{(trkThunderstormIntensity.Value)} %"

        If trkThunderstormIntensity.Value <> lblThunderstormIntensity.Tag Then
            lblThunderstormIntensityLabel.ForeColor = Color.Red
        Else
            lblThunderstormIntensityLabel.ForeColor = GroupBox2.ForeColor
        End If

    End Sub

    Private Sub lblThunderstormIntensityLabel_DoubleClick(sender As Object, e As EventArgs) Handles lblThunderstormIntensityLabel.DoubleClick
        trkThunderstormIntensity.Value = lblThunderstormIntensity.Tag
        trkThunderstormIntensity_Scroll(sender, e)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged

        Select Case TabControl1.SelectedIndex
            Case 0 'Preset Data
            Case 1 'Profile Details
                AdjustLastRowHeight(weatherDetailsDataGrid)
                AdjustLastRowHeight(windLayersDatagrid)
                AdjustLastRowHeight(cloudLayersDatagrid)
            Case 2 'Graph
            Case 3 'Customization
        End Select

    End Sub

    Private Sub trkRating_ValueChanged(sender As Object, e As EventArgs) Handles trkRating.ValueChanged
        txtRating.Text = trkRating.Value.ToString
    End Sub

    Private Sub trkStrength_ValueChanged(sender As Object, e As EventArgs) Handles trkStrength.ValueChanged
        txtStrength.Text = trkStrength.Value.ToString
    End Sub

    Private Sub trkCameraTrack_Scroll(sender As Object, e As EventArgs) Handles trkCameraTrack.Scroll

        _processingDisplayChanges = True

        Dim sliderValue As Integer = trkCameraTrack.Value

        ' Values at slider position 0 'H, T, D, FOV
        Dim startValues As Double() = {139, -100, 1, 46} '{139, -100, 1, 46}

        ' Values at slider position 49
        Dim endValues As Double() = {139, -100, 200, 19} '{139, -100, 200, 19}

        ' Calculate the mapped values for the current slider position
        trkViewHeight.Value = MapValue(sliderValue, trkCameraTrack.Maximum, startValues(0), endValues(0))
        trkViewTilt.Value = MapValue(sliderValue, trkCameraTrack.Maximum, startValues(1), endValues(1))
        trkViewDistance.Value = MapValue(sliderValue, trkCameraTrack.Maximum, startValues(2), endValues(2))
        trkViewFOV.Value = MapValue(sliderValue, trkCameraTrack.Maximum, startValues(3), endValues(3))

        ' Use the mapped values for whatever you need in your application
        WindLayers3DControl1.SetCameraPositionAndDirection(trkViewRotation.Value,
                                                           trkViewHeight.Value / 100,
                                                           trkViewTilt.Value / 100,
                                                           trkViewDistance.Value / 100,
                                                           trkViewFOV.Value)

        lblAngleValue.Text = trkViewRotation.Value.ToString
        lblHeightValue.Text = trkViewHeight.Value.ToString
        lblTiltValue.Text = trkViewTilt.Value.ToString
        lblDistanceValue.Text = trkViewDistance.Value.ToString
        lblFOVValue.Text = trkViewFOV.Value.ToString

        _processingDisplayChanges = False

    End Sub

    Private Sub ViewTrackBars_Scroll(sender As Object, e As EventArgs) Handles trkViewRotation.Scroll,
                                                                           trkViewHeight.Scroll,
                                                                           trkViewTilt.Scroll,
                                                                           trkViewDistance.Scroll,
                                                                           trkViewFOV.Scroll

        If Not _processingDisplayChanges Then

            lblAngleValue.Text = trkViewRotation.Value.ToString
            lblHeightValue.Text = trkViewHeight.Value.ToString
            lblTiltValue.Text = trkViewTilt.Value.ToString
            lblDistanceValue.Text = trkViewDistance.Value.ToString
            lblFOVValue.Text = trkViewFOV.Value.ToString

            WindLayers3DControl1.SetCameraPositionAndDirection(trkViewRotation.Value,
                                                           trkViewHeight.Value / 100,
                                                           trkViewTilt.Value / 100,
                                                           trkViewDistance.Value / 100,
                                                           trkViewFOV.Value)
        End If

    End Sub

    Private Sub btnWindDir_Click(sender As Object, e As EventArgs) Handles btnWindDirW.Click, btnWindDirSW.Click, btnWindDirSE.Click, btnWindDirS.Click, btnWindDirNW.Click, btnWindDirNE.Click, btnWindDirN.Click, btnWindDirE.Click

        trkWindDirectionSingle.Value = CType(sender, System.Windows.Forms.Button).Tag
        trkWindDirectionSingle_Scroll(trkWindDirectionSingle, e)

    End Sub

    Private Sub btnActionSave_Click(sender As Object, e As EventArgs) Handles btnActionSave.Click

        'Save data depending on rights
        Dim idPreset As Integer = weatherProfilesDataGrid.SelectedRows(0).Cells(0).Value
        Dim source As String = weatherProfilesDataGrid.SelectedRows(0).Cells(1).Value
        Dim preset As WeatherPreset = Nothing

        Select Case source
            Case "Default"
                preset = _allDefaultWeatherPresets(idPreset)
            Case "User"
                preset = _allUserDefinedWeatherPresets(idPreset)
                'Has the title changed?
                If Not preset.BuiltInData.Title.Trim = txtPresetTitle.Text.Trim Then
                    'Title has changed
                    'Make sure it's not already in use
                    Dim titleAlreadyInUse As Boolean = False
                    'Check title with all other presets - default ones first
                    For Each id As Integer In _allDefaultWeatherPresets.Keys
                        If id = idPreset Then
                            'This is the current preset - so skip
                        Else
                            If _allDefaultWeatherPresets(id).BuiltInData.Title = txtPresetTitle.Text.Trim Then
                                titleAlreadyInUse = True
                                Exit For
                            End If
                        End If
                    Next
                    If Not titleAlreadyInUse Then
                        For Each id As Integer In _allUserDefinedWeatherPresets.Keys
                            If id = idPreset Then
                                'This is the current preset - so skip
                            Else
                                If _allUserDefinedWeatherPresets(id).BuiltInData.Title = txtPresetTitle.Text.Trim Then
                                    titleAlreadyInUse = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                    If titleAlreadyInUse Then
                        Using New Centered_MessageBox(Me)
                            MessageBox.Show(Me, $"This title already exists for another preset, it cannot be used!", "Existing title", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Using
                        Exit Sub
                    End If
                End If
                preset.BuiltInData.Title = txtPresetTitle.Text.Trim
                preset.BuiltInData.Description = txtPresetDescription.Text.Trim
                preset.BuiltInData.SoaringTypeRidge = chkSoaringTypeRidge.Checked
                preset.BuiltInData.SoaringTypeThermal = chkSoaringTypeThermal.Checked
                preset.BuiltInData.SoaringTypeWave = chkSoaringTypeWave.Checked
                preset.BuiltInData.SoaringTypeDynamic = chkSoaringTypeDynamic.Checked
                preset.BuiltInData.SavePreset()
        End Select

        'Preset user data
        preset.UserData.Rating = trkRating.Value
        preset.UserData.OverallStrengthScale = trkStrength.Value
        preset.UserData.Comments = txtComments.Text.Trim
        preset.UserData.SoaringTypeRidge = chkPrimaryRidge.Checked
        preset.UserData.SoaringTypeThermal = chkPrimaryThermal.Checked
        preset.UserData.SoaringTypeWave = chkPrimaryWave.Checked
        preset.UserData.SoaringTypeDynamic = chkPrimaryDynamic.Checked
        preset.UserData.SavePreset()

        weatherProfilesDataGrid.SelectedRows(0).Cells(2).Value = preset.BuiltInData.Title
        weatherProfilesDataGrid.SelectedRows(0).Cells(3).Value = preset.BuiltInData.AllSoaringTypes
        weatherProfilesDataGrid.SelectedRows(0).Cells(4).Value = preset.UserData.Rating
        weatherProfilesDataGrid.SelectedRows(0).Cells(5).Value = preset.UserData.OverallStrengthScale
        weatherProfilesDataGrid.SelectedRows(0).Cells(6).Value = preset.UserData.AllSoaringTypes

    End Sub

    Private Sub btnActionDelete_Click(sender As Object, e As EventArgs) Handles btnActionDelete.Click

        Dim idPreset As Integer = weatherProfilesDataGrid.SelectedRows(0).Cells(0).Value
        Dim source As String = weatherProfilesDataGrid.SelectedRows(0).Cells(1).Value
        Dim preset As WeatherPreset = Nothing

        'Confirm deletion
        Dim msg As String = String.Empty
        Select Case source
            Case "Default"
                msg = "This is a default preset, it will be hidden from view instead of a full deletion. Do you want to proceed?"
            Case "User"
                msg = "This is a user preset, it will be fully deleted and unretrievable! Do you want to proceed?"
        End Select
        Using New Centered_MessageBox(Me)
            If MessageBox.Show(Me, msg, "Confirm removal/deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If
        End Using

        Select Case source
            Case "Default"
                preset = _allDefaultWeatherPresets(idPreset)
                preset.UserData.Hidden = True
                preset.Save()

            Case "User"
                'Delete!
                preset = _allUserDefinedWeatherPresets(idPreset)
                If Not SupportingFeatures.DeleteFolderAndFiles(Path.Combine(_weatherPresetsBaseDirectory, "UserDefined", preset.Name)) Then
                    MessageBox.Show(Me, "An error occured during deletion of the preset.", "Error deleting preset", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

        End Select

        WeatherPresetsBrowser_Load(btnActionDelete, e)

    End Sub

    Private Sub btnActionNew_Click(sender As Object, e As EventArgs) Handles btnActionNew.Click

        'Creating a new preset requires a weather file and a name (not title) ??

    End Sub

#End Region

#Region "Subs"

    Private Sub LoadUpAllPresetsInFolder(baseFolder As String, userDefined As Boolean, ByRef seq As Integer)

        Dim subFolderName As String

        'Does the base folder exist?
        If Not Directory.Exists(baseFolder) Then
            Exit Sub
        End If

        If userDefined Then
            _allUserDefinedWeatherPresets.Clear()
        Else
            _allDefaultWeatherPresets.Clear()
        End If

        For Each subfolder As String In Directory.GetDirectories(baseFolder)

            subFolderName = Path.GetFileName(subfolder)

            'Check if folder contains the required files first (Weather WPR and Built-in designer data)
            If File.Exists(Path.Combine(baseFolder, subFolderName, $"{subFolderName}.wpr")) AndAlso
                File.Exists(Path.Combine(baseFolder, subFolderName, "BuiltInData.xml")) Then

                seq += 1
                If userDefined Then
                    _allUserDefinedWeatherPresets.Add(seq, New WeatherPreset(subFolderName, True))
                Else
                    _allDefaultWeatherPresets.Add(seq, New WeatherPreset(subFolderName, False))
                End If

            End If

        Next

    End Sub


    Private Sub AdjustLastRowHeight(theGrid As DataGridView)
        If theGrid.Rows.Count > 1 Then
            Dim totalHeight As Integer = If(theGrid.ColumnHeadersVisible, theGrid.ColumnHeadersHeight, 0)
            For i As Integer = 0 To theGrid.Rows.Count - 2 ' Exclude the last row
                totalHeight += theGrid.Rows(i).Height
            Next

            Dim remainingSpace As Integer = theGrid.Height - totalHeight ' - SystemInformation.HorizontalScrollBarHeight ' Subtract scrollbar height if visible
            Dim minimumHeight As Integer = 30 ' Minimum height for the last row, adjust as necessary

            If remainingSpace > minimumHeight Then
                theGrid.Rows(theGrid.Rows.Count - 1).Height = remainingSpace
                theGrid.ScrollBars = ScrollBars.None
            Else
                theGrid.Rows(theGrid.Rows.Count - 1).Height = minimumHeight
            End If
            AdjustScrollBars(theGrid)
        End If
    End Sub

    Private Sub AdjustScrollBars(theGrid As DataGridView)
        ' Calculate the total height of all the rows including the header.
        Dim totalHeight As Integer = If(theGrid.ColumnHeadersVisible, theGrid.ColumnHeadersHeight, 0)
        For Each row As DataGridViewRow In theGrid.Rows
            totalHeight += row.Height
        Next

        ' Compare with the client area of the DataGridView.
        If totalHeight <= theGrid.ClientRectangle.Height Then
            ' All rows are displayed, so we can hide the scroll bars.
            theGrid.ScrollBars = ScrollBars.Horizontal
        Else
            ' Not all rows are displayed, so we need to show the scroll bars.
            theGrid.ScrollBars = ScrollBars.Both
        End If
    End Sub

    Private Function MapValue(sliderValue As Integer, sliderMax As Integer, startValue As Double, endValue As Double) As Double
        ' Calculate the percentage of the slider position
        Dim percentage As Double = sliderValue / sliderMax

        ' Determine the change needed for the current slider position
        Dim change As Double = (endValue - startValue) * percentage

        ' Map the slider position to the value
        Dim mappedValue As Double = startValue + change

        Return mappedValue
    End Function


    Private Sub SetWindLayers3DDisplay(preset As WeatherPreset)

        WindLayers3DControl1.ResetViewport()

        WindLayers3DControl1.AddCentralCylinder(0, preset.ProfileDetails.HighestWindLayerAlt)

        '0.80000 represents the maximum altitude possible (the very maximum being 60000 feet)
        '0.00001 still show above the compass rose (so the span could be between 1 and 80k as value span between lowest and highest altitudes)
        For Each windlayr As WindLayer In preset.ProfileDetails.WindLayers
            WindLayers3DControl1.AddWindLayer(windlayr.Altitude, windlayr.Angle, windlayr.Speed, preset.ProfileDetails.LowestWindLayerAlt, preset.ProfileDetails.HighestWindLayerAlt)
        Next

    End Sub

#End Region

End Class