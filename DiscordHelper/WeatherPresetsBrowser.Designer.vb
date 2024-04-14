<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WeatherPresetsBrowser
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.weatherProfilesDataGrid = New System.Windows.Forms.DataGridView()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.txtPresetTitle = New System.Windows.Forms.TextBox()
        Me.chkSoaringTypeDynamic = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeWave = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeThermal = New System.Windows.Forms.CheckBox()
        Me.chkSoaringTypeRidge = New System.Windows.Forms.CheckBox()
        Me.txtPresetDescription = New System.Windows.Forms.TextBox()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.btnActionNew = New System.Windows.Forms.Button()
        Me.btnActionSave = New System.Windows.Forms.Button()
        Me.btnActionDelete = New System.Windows.Forms.Button()
        Me.btnActionImport = New System.Windows.Forms.Button()
        Me.lblBarometricPressure = New System.Windows.Forms.Label()
        Me.trkBaroPressure = New System.Windows.Forms.TrackBar()
        Me.lblHumidityIndex = New System.Windows.Forms.Label()
        Me.trkHumidityIndex = New System.Windows.Forms.TrackBar()
        Me.lblWindDirectionMulti = New System.Windows.Forms.Label()
        Me.trkWindDirectionMulti = New System.Windows.Forms.TrackBar()
        Me.lblWindDirectionSingle = New System.Windows.Forms.Label()
        Me.trkWindDirectionSingle = New System.Windows.Forms.TrackBar()
        Me.trkCameraTrack = New System.Windows.Forms.TrackBar()
        Me.lblFOVValue = New System.Windows.Forms.Label()
        Me.trkViewFOV = New System.Windows.Forms.TrackBar()
        Me.lblDistanceValue = New System.Windows.Forms.Label()
        Me.trkViewDistance = New System.Windows.Forms.TrackBar()
        Me.lblTiltValue = New System.Windows.Forms.Label()
        Me.trkViewTilt = New System.Windows.Forms.TrackBar()
        Me.lblHeightValue = New System.Windows.Forms.Label()
        Me.trkViewHeight = New System.Windows.Forms.TrackBar()
        Me.lblAngleValue = New System.Windows.Forms.Label()
        Me.trkViewRotation = New System.Windows.Forms.TrackBar()
        Me.lblTemperature = New System.Windows.Forms.Label()
        Me.trkTemperature = New System.Windows.Forms.TrackBar()
        Me.lblPrecipitations = New System.Windows.Forms.Label()
        Me.trkPrecipitations = New System.Windows.Forms.TrackBar()
        Me.lblSnowCover = New System.Windows.Forms.Label()
        Me.trkSnowCover = New System.Windows.Forms.TrackBar()
        Me.lblThunderstormIntensity = New System.Windows.Forms.Label()
        Me.trkThunderstormIntensity = New System.Windows.Forms.TrackBar()
        Me.chkPrimaryDynamic = New System.Windows.Forms.CheckBox()
        Me.chkPrimaryWave = New System.Windows.Forms.CheckBox()
        Me.chkPrimaryThermal = New System.Windows.Forms.CheckBox()
        Me.chkPrimaryRidge = New System.Windows.Forms.CheckBox()
        Me.splitContainerWeatherPresetManager = New System.Windows.Forms.SplitContainer()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tbdgPresetDetails = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlPresetActions = New System.Windows.Forms.Panel()
        Me.pnlPresetUserData = New System.Windows.Forms.Panel()
        Me.grpUserData = New System.Windows.Forms.GroupBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtStrength = New System.Windows.Forms.TextBox()
        Me.txtRating = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.trkStrength = New System.Windows.Forms.TrackBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.trkRating = New System.Windows.Forms.TrackBar()
        Me.pnlPresetBuildInData = New System.Windows.Forms.Panel()
        Me.grpPresetBuiltInData = New System.Windows.Forms.GroupBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.lblSoaringType = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbpgProfileDetails = New System.Windows.Forms.TabPage()
        Me.weatherDetailsDataGrid = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.cloudLayersDatagrid = New System.Windows.Forms.DataGridView()
        Me.windLayersDatagrid = New System.Windows.Forms.DataGridView()
        Me.tbpgLayers = New System.Windows.Forms.TabPage()
        Me.FullWeatherGraphPanel1 = New SIGLR.SoaringTools.CommonLibrary.FullWeatherGraphPanel()
        Me.tbpgCustom = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblThunderstormIntensityLabel = New System.Windows.Forms.Label()
        Me.lblSnowCoverLabel = New System.Windows.Forms.Label()
        Me.lblPrecipitationsLabel = New System.Windows.Forms.Label()
        Me.lblTemperatureLabel = New System.Windows.Forms.Label()
        Me.lblBarometricPressureLabel = New System.Windows.Forms.Label()
        Me.lblHumidityIndexLabel = New System.Windows.Forms.Label()
        Me.btnWindDirNW = New System.Windows.Forms.Button()
        Me.btnWindDirSW = New System.Windows.Forms.Button()
        Me.btnWindDirSE = New System.Windows.Forms.Button()
        Me.btnWindDirNE = New System.Windows.Forms.Button()
        Me.btnWindDirW = New System.Windows.Forms.Button()
        Me.btnWindDirE = New System.Windows.Forms.Button()
        Me.btnWindDirS = New System.Windows.Forms.Button()
        Me.btnWindDirN = New System.Windows.Forms.Button()
        Me.lblWindDirectionMultiLabel = New System.Windows.Forms.Label()
        Me.lblWindDirectionSingleLabel = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ElementHost1 = New System.Windows.Forms.Integration.ElementHost()
        Me.WindLayers3DControl1 = New WindLayers3DDisplay.WindLayers3DDisplay.WindLayers3DControl()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.optGridDetails = New System.Windows.Forms.RadioButton()
        Me.optGridOnly = New System.Windows.Forms.RadioButton()
        Me.optDetailsOnly = New System.Windows.Forms.RadioButton()
        CType(Me.weatherProfilesDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkBaroPressure, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkHumidityIndex, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkWindDirectionMulti, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkWindDirectionSingle, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkCameraTrack, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkViewFOV, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkViewDistance, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkViewTilt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkViewHeight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkViewRotation, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkTemperature, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkPrecipitations, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkSnowCover, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkThunderstormIntensity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.splitContainerWeatherPresetManager, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainerWeatherPresetManager.Panel1.SuspendLayout()
        Me.splitContainerWeatherPresetManager.Panel2.SuspendLayout()
        Me.splitContainerWeatherPresetManager.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.tbdgPresetDetails.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.pnlPresetActions.SuspendLayout()
        Me.pnlPresetUserData.SuspendLayout()
        Me.grpUserData.SuspendLayout()
        CType(Me.trkStrength, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkRating, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlPresetBuildInData.SuspendLayout()
        Me.grpPresetBuiltInData.SuspendLayout()
        Me.tbpgProfileDetails.SuspendLayout()
        CType(Me.weatherDetailsDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.cloudLayersDatagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.windLayersDatagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tbpgLayers.SuspendLayout()
        Me.tbpgCustom.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'weatherProfilesDataGrid
        '
        Me.weatherProfilesDataGrid.AllowUserToAddRows = False
        Me.weatherProfilesDataGrid.AllowUserToDeleteRows = False
        Me.weatherProfilesDataGrid.AllowUserToResizeRows = False
        Me.weatherProfilesDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.weatherProfilesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.weatherProfilesDataGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.weatherProfilesDataGrid.Location = New System.Drawing.Point(0, 0)
        Me.weatherProfilesDataGrid.MultiSelect = False
        Me.weatherProfilesDataGrid.Name = "weatherProfilesDataGrid"
        Me.weatherProfilesDataGrid.ReadOnly = True
        Me.weatherProfilesDataGrid.RowHeadersVisible = False
        Me.weatherProfilesDataGrid.RowHeadersWidth = 25
        Me.weatherProfilesDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.weatherProfilesDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.weatherProfilesDataGrid.Size = New System.Drawing.Size(597, 911)
        Me.weatherProfilesDataGrid.TabIndex = 0
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(1335, 917)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(147, 51)
        Me.btnClose.TabIndex = 1
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'txtPresetTitle
        '
        Me.txtPresetTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPresetTitle.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.txtPresetTitle.Location = New System.Drawing.Point(133, 26)
        Me.txtPresetTitle.Name = "txtPresetTitle"
        Me.txtPresetTitle.Size = New System.Drawing.Size(734, 27)
        Me.txtPresetTitle.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.txtPresetTitle, "The weather preset's title, as defined by the designer.")
        '
        'chkSoaringTypeDynamic
        '
        Me.chkSoaringTypeDynamic.AutoSize = True
        Me.chkSoaringTypeDynamic.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkSoaringTypeDynamic.Location = New System.Drawing.Point(282, 59)
        Me.chkSoaringTypeDynamic.Name = "chkSoaringTypeDynamic"
        Me.chkSoaringTypeDynamic.Size = New System.Drawing.Size(38, 24)
        Me.chkSoaringTypeDynamic.TabIndex = 33
        Me.chkSoaringTypeDynamic.Tag = "8"
        Me.chkSoaringTypeDynamic.Text = "D"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeDynamic, "Check if weather can provide good dynamic soaring.")
        Me.chkSoaringTypeDynamic.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeWave
        '
        Me.chkSoaringTypeWave.AutoSize = True
        Me.chkSoaringTypeWave.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkSoaringTypeWave.Location = New System.Drawing.Point(228, 59)
        Me.chkSoaringTypeWave.Name = "chkSoaringTypeWave"
        Me.chkSoaringTypeWave.Size = New System.Drawing.Size(42, 24)
        Me.chkSoaringTypeWave.TabIndex = 32
        Me.chkSoaringTypeWave.Tag = "8"
        Me.chkSoaringTypeWave.Text = "W"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeWave, "Check if weather can provide good wave soaring.")
        Me.chkSoaringTypeWave.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeThermal
        '
        Me.chkSoaringTypeThermal.AutoSize = True
        Me.chkSoaringTypeThermal.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkSoaringTypeThermal.Location = New System.Drawing.Point(181, 59)
        Me.chkSoaringTypeThermal.Name = "chkSoaringTypeThermal"
        Me.chkSoaringTypeThermal.Size = New System.Drawing.Size(36, 24)
        Me.chkSoaringTypeThermal.TabIndex = 31
        Me.chkSoaringTypeThermal.Tag = "8"
        Me.chkSoaringTypeThermal.Text = "T"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeThermal, "Check if weather can provide good thermal soaring.")
        Me.chkSoaringTypeThermal.UseVisualStyleBackColor = True
        '
        'chkSoaringTypeRidge
        '
        Me.chkSoaringTypeRidge.AutoSize = True
        Me.chkSoaringTypeRidge.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkSoaringTypeRidge.Location = New System.Drawing.Point(133, 59)
        Me.chkSoaringTypeRidge.Name = "chkSoaringTypeRidge"
        Me.chkSoaringTypeRidge.Size = New System.Drawing.Size(37, 24)
        Me.chkSoaringTypeRidge.TabIndex = 30
        Me.chkSoaringTypeRidge.Tag = "8"
        Me.chkSoaringTypeRidge.Text = "R"
        Me.ToolTip1.SetToolTip(Me.chkSoaringTypeRidge, "Check if weather can provide good ridge soaring.")
        Me.chkSoaringTypeRidge.UseVisualStyleBackColor = True
        '
        'txtPresetDescription
        '
        Me.txtPresetDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPresetDescription.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.txtPresetDescription.Location = New System.Drawing.Point(133, 89)
        Me.txtPresetDescription.Multiline = True
        Me.txtPresetDescription.Name = "txtPresetDescription"
        Me.txtPresetDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtPresetDescription.Size = New System.Drawing.Size(734, 307)
        Me.txtPresetDescription.TabIndex = 57
        Me.txtPresetDescription.Tag = "16"
        Me.ToolTip1.SetToolTip(Me.txtPresetDescription, "Weather preset's description, as provided by the designer.")
        '
        'txtComments
        '
        Me.txtComments.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtComments.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.txtComments.Location = New System.Drawing.Point(133, 144)
        Me.txtComments.Multiline = True
        Me.txtComments.Name = "txtComments"
        Me.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtComments.Size = New System.Drawing.Size(734, 252)
        Me.txtComments.TabIndex = 59
        Me.txtComments.Tag = "16"
        Me.ToolTip1.SetToolTip(Me.txtComments, "Weather preset's description, as provided by the designer.")
        '
        'btnActionNew
        '
        Me.btnActionNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnActionNew.Location = New System.Drawing.Point(0, 3)
        Me.btnActionNew.Name = "btnActionNew"
        Me.btnActionNew.Size = New System.Drawing.Size(147, 44)
        Me.btnActionNew.TabIndex = 4
        Me.btnActionNew.Text = "New"
        Me.ToolTip1.SetToolTip(Me.btnActionNew, "Click to create a new weather preset.")
        Me.btnActionNew.UseVisualStyleBackColor = True
        '
        'btnActionSave
        '
        Me.btnActionSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnActionSave.Location = New System.Drawing.Point(153, 3)
        Me.btnActionSave.Name = "btnActionSave"
        Me.btnActionSave.Size = New System.Drawing.Size(147, 44)
        Me.btnActionSave.TabIndex = 5
        Me.btnActionSave.Text = "Save"
        Me.ToolTip1.SetToolTip(Me.btnActionSave, "Click to save changes to the selected weather preset.")
        Me.btnActionSave.UseVisualStyleBackColor = True
        '
        'btnActionDelete
        '
        Me.btnActionDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnActionDelete.Location = New System.Drawing.Point(306, 3)
        Me.btnActionDelete.Name = "btnActionDelete"
        Me.btnActionDelete.Size = New System.Drawing.Size(147, 44)
        Me.btnActionDelete.TabIndex = 6
        Me.btnActionDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnActionDelete, "Click to delete the selected weather preset.")
        Me.btnActionDelete.UseVisualStyleBackColor = True
        '
        'btnActionImport
        '
        Me.btnActionImport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnActionImport.Location = New System.Drawing.Point(1182, 917)
        Me.btnActionImport.Name = "btnActionImport"
        Me.btnActionImport.Size = New System.Drawing.Size(147, 51)
        Me.btnActionImport.TabIndex = 0
        Me.btnActionImport.Text = "Use for task"
        Me.ToolTip1.SetToolTip(Me.btnActionImport, "Click to use this customized preset with the current task.")
        Me.btnActionImport.UseVisualStyleBackColor = True
        '
        'lblBarometricPressure
        '
        Me.lblBarometricPressure.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblBarometricPressure.Location = New System.Drawing.Point(152, 205)
        Me.lblBarometricPressure.Name = "lblBarometricPressure"
        Me.lblBarometricPressure.Size = New System.Drawing.Size(188, 20)
        Me.lblBarometricPressure.TabIndex = 15
        Me.lblBarometricPressure.Text = "20.00"
        Me.lblBarometricPressure.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblBarometricPressure, "Current barometric pressure (double-click to set to standard)")
        '
        'trkBaroPressure
        '
        Me.trkBaroPressure.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkBaroPressure.AutoSize = False
        Me.trkBaroPressure.LargeChange = 100000
        Me.trkBaroPressure.Location = New System.Drawing.Point(6, 228)
        Me.trkBaroPressure.Maximum = 108379367
        Me.trkBaroPressure.Minimum = 94800000
        Me.trkBaroPressure.Name = "trkBaroPressure"
        Me.trkBaroPressure.Size = New System.Drawing.Size(334, 25)
        Me.trkBaroPressure.SmallChange = 10000
        Me.trkBaroPressure.TabIndex = 16
        Me.trkBaroPressure.TickFrequency = 15
        Me.trkBaroPressure.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkBaroPressure, "Move the slider to set customized barometric pressure")
        Me.trkBaroPressure.Value = 94800000
        '
        'lblHumidityIndex
        '
        Me.lblHumidityIndex.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHumidityIndex.Location = New System.Drawing.Point(233, 154)
        Me.lblHumidityIndex.Name = "lblHumidityIndex"
        Me.lblHumidityIndex.Size = New System.Drawing.Size(107, 20)
        Me.lblHumidityIndex.TabIndex = 12
        Me.lblHumidityIndex.Text = "20.00"
        Me.lblHumidityIndex.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblHumidityIndex, "Current humidity index")
        '
        'trkHumidityIndex
        '
        Me.trkHumidityIndex.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkHumidityIndex.AutoSize = False
        Me.trkHumidityIndex.LargeChange = 10
        Me.trkHumidityIndex.Location = New System.Drawing.Point(6, 177)
        Me.trkHumidityIndex.Maximum = 2000
        Me.trkHumidityIndex.Minimum = 100
        Me.trkHumidityIndex.Name = "trkHumidityIndex"
        Me.trkHumidityIndex.Size = New System.Drawing.Size(334, 25)
        Me.trkHumidityIndex.TabIndex = 13
        Me.trkHumidityIndex.TickFrequency = 15
        Me.trkHumidityIndex.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkHumidityIndex, "Move the slider to set customized humidity index")
        Me.trkHumidityIndex.Value = 100
        '
        'lblWindDirectionMulti
        '
        Me.lblWindDirectionMulti.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWindDirectionMulti.Location = New System.Drawing.Point(297, 103)
        Me.lblWindDirectionMulti.Name = "lblWindDirectionMulti"
        Me.lblWindDirectionMulti.Size = New System.Drawing.Size(43, 20)
        Me.lblWindDirectionMulti.TabIndex = 9
        Me.lblWindDirectionMulti.Text = "0"
        Me.lblWindDirectionMulti.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblWindDirectionMulti, "Current wind direction shift")
        '
        'trkWindDirectionMulti
        '
        Me.trkWindDirectionMulti.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkWindDirectionMulti.AutoSize = False
        Me.trkWindDirectionMulti.Location = New System.Drawing.Point(6, 126)
        Me.trkWindDirectionMulti.Maximum = 180
        Me.trkWindDirectionMulti.Minimum = -180
        Me.trkWindDirectionMulti.Name = "trkWindDirectionMulti"
        Me.trkWindDirectionMulti.Size = New System.Drawing.Size(334, 25)
        Me.trkWindDirectionMulti.TabIndex = 10
        Me.trkWindDirectionMulti.TickFrequency = 15
        Me.ToolTip1.SetToolTip(Me.trkWindDirectionMulti, "Move the slider to set customized wind direction shift")
        Me.trkWindDirectionMulti.Value = -180
        '
        'lblWindDirectionSingle
        '
        Me.lblWindDirectionSingle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWindDirectionSingle.Location = New System.Drawing.Point(297, 23)
        Me.lblWindDirectionSingle.Name = "lblWindDirectionSingle"
        Me.lblWindDirectionSingle.Size = New System.Drawing.Size(43, 20)
        Me.lblWindDirectionSingle.TabIndex = 1
        Me.lblWindDirectionSingle.Text = "0"
        Me.lblWindDirectionSingle.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblWindDirectionSingle, "Current wind direction")
        '
        'trkWindDirectionSingle
        '
        Me.trkWindDirectionSingle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkWindDirectionSingle.AutoSize = False
        Me.trkWindDirectionSingle.Location = New System.Drawing.Point(6, 46)
        Me.trkWindDirectionSingle.Maximum = 359
        Me.trkWindDirectionSingle.Name = "trkWindDirectionSingle"
        Me.trkWindDirectionSingle.Size = New System.Drawing.Size(334, 25)
        Me.trkWindDirectionSingle.TabIndex = 2
        Me.trkWindDirectionSingle.TickFrequency = 15
        Me.ToolTip1.SetToolTip(Me.trkWindDirectionSingle, "Move the slider to set customized wind direction")
        Me.trkWindDirectionSingle.Value = 270
        '
        'trkCameraTrack
        '
        Me.trkCameraTrack.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkCameraTrack.AutoSize = False
        Me.trkCameraTrack.LargeChange = 1
        Me.trkCameraTrack.Location = New System.Drawing.Point(10, 46)
        Me.trkCameraTrack.Maximum = 27
        Me.trkCameraTrack.Name = "trkCameraTrack"
        Me.trkCameraTrack.Size = New System.Drawing.Size(330, 25)
        Me.trkCameraTrack.TabIndex = 1
        Me.trkCameraTrack.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkCameraTrack, "Move the slider to change to a different default camera position.")
        Me.trkCameraTrack.Value = 20
        '
        'lblFOVValue
        '
        Me.lblFOVValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFOVValue.Location = New System.Drawing.Point(297, 278)
        Me.lblFOVValue.Name = "lblFOVValue"
        Me.lblFOVValue.Size = New System.Drawing.Size(43, 20)
        Me.lblFOVValue.TabIndex = 15
        Me.lblFOVValue.Text = "0"
        Me.lblFOVValue.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblFOVValue, "Current camera field of view")
        '
        'trkViewFOV
        '
        Me.trkViewFOV.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkViewFOV.AutoSize = False
        Me.trkViewFOV.LargeChange = 1
        Me.trkViewFOV.Location = New System.Drawing.Point(10, 301)
        Me.trkViewFOV.Maximum = 90
        Me.trkViewFOV.Minimum = 10
        Me.trkViewFOV.Name = "trkViewFOV"
        Me.trkViewFOV.Size = New System.Drawing.Size(330, 25)
        Me.trkViewFOV.TabIndex = 16
        Me.trkViewFOV.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkViewFOV, "Move the slider to change the camera's field of view")
        Me.trkViewFOV.Value = 38
        '
        'lblDistanceValue
        '
        Me.lblDistanceValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDistanceValue.Location = New System.Drawing.Point(297, 176)
        Me.lblDistanceValue.Name = "lblDistanceValue"
        Me.lblDistanceValue.Size = New System.Drawing.Size(43, 20)
        Me.lblDistanceValue.TabIndex = 9
        Me.lblDistanceValue.Text = "0"
        Me.lblDistanceValue.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblDistanceValue, "Current camera distance")
        '
        'trkViewDistance
        '
        Me.trkViewDistance.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkViewDistance.AutoSize = False
        Me.trkViewDistance.LargeChange = 1
        Me.trkViewDistance.Location = New System.Drawing.Point(10, 199)
        Me.trkViewDistance.Maximum = 200
        Me.trkViewDistance.Minimum = 1
        Me.trkViewDistance.Name = "trkViewDistance"
        Me.trkViewDistance.Size = New System.Drawing.Size(330, 25)
        Me.trkViewDistance.TabIndex = 10
        Me.trkViewDistance.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkViewDistance, "Move the slider to change the camera's distance")
        Me.trkViewDistance.Value = 90
        '
        'lblTiltValue
        '
        Me.lblTiltValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTiltValue.Location = New System.Drawing.Point(297, 227)
        Me.lblTiltValue.Name = "lblTiltValue"
        Me.lblTiltValue.Size = New System.Drawing.Size(43, 20)
        Me.lblTiltValue.TabIndex = 12
        Me.lblTiltValue.Text = "0"
        Me.lblTiltValue.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblTiltValue, "Current camera tilt angle")
        '
        'trkViewTilt
        '
        Me.trkViewTilt.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkViewTilt.AutoSize = False
        Me.trkViewTilt.LargeChange = 1
        Me.trkViewTilt.Location = New System.Drawing.Point(10, 250)
        Me.trkViewTilt.Maximum = 100
        Me.trkViewTilt.Minimum = -100
        Me.trkViewTilt.Name = "trkViewTilt"
        Me.trkViewTilt.Size = New System.Drawing.Size(330, 25)
        Me.trkViewTilt.TabIndex = 13
        Me.trkViewTilt.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkViewTilt, "Move the slider to change the camera's tilt angle")
        Me.trkViewTilt.Value = 70
        '
        'lblHeightValue
        '
        Me.lblHeightValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHeightValue.Location = New System.Drawing.Point(297, 125)
        Me.lblHeightValue.Name = "lblHeightValue"
        Me.lblHeightValue.Size = New System.Drawing.Size(43, 20)
        Me.lblHeightValue.TabIndex = 6
        Me.lblHeightValue.Text = "0"
        Me.lblHeightValue.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblHeightValue, "Current camera height")
        '
        'trkViewHeight
        '
        Me.trkViewHeight.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkViewHeight.AutoSize = False
        Me.trkViewHeight.LargeChange = 1
        Me.trkViewHeight.Location = New System.Drawing.Point(10, 148)
        Me.trkViewHeight.Maximum = 200
        Me.trkViewHeight.Name = "trkViewHeight"
        Me.trkViewHeight.Size = New System.Drawing.Size(330, 25)
        Me.trkViewHeight.TabIndex = 7
        Me.trkViewHeight.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkViewHeight, "Move the slider to change the camera's height")
        Me.trkViewHeight.Value = 90
        '
        'lblAngleValue
        '
        Me.lblAngleValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblAngleValue.Location = New System.Drawing.Point(297, 74)
        Me.lblAngleValue.Name = "lblAngleValue"
        Me.lblAngleValue.Size = New System.Drawing.Size(43, 20)
        Me.lblAngleValue.TabIndex = 3
        Me.lblAngleValue.Text = "15"
        Me.lblAngleValue.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblAngleValue, "Current camera position on the compass rose.")
        '
        'trkViewRotation
        '
        Me.trkViewRotation.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkViewRotation.AutoSize = False
        Me.trkViewRotation.Location = New System.Drawing.Point(10, 97)
        Me.trkViewRotation.Maximum = 179
        Me.trkViewRotation.Minimum = -179
        Me.trkViewRotation.Name = "trkViewRotation"
        Me.trkViewRotation.Size = New System.Drawing.Size(330, 25)
        Me.trkViewRotation.TabIndex = 4
        Me.trkViewRotation.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkViewRotation, "Move the slider to change the placement of the camera along the compass axis.")
        Me.trkViewRotation.Value = 15
        '
        'lblTemperature
        '
        Me.lblTemperature.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTemperature.Location = New System.Drawing.Point(152, 256)
        Me.lblTemperature.Name = "lblTemperature"
        Me.lblTemperature.Size = New System.Drawing.Size(188, 20)
        Me.lblTemperature.TabIndex = 18
        Me.lblTemperature.Text = "20.00"
        Me.lblTemperature.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblTemperature, "Current temperature")
        '
        'trkTemperature
        '
        Me.trkTemperature.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkTemperature.AutoSize = False
        Me.trkTemperature.LargeChange = 1000
        Me.trkTemperature.Location = New System.Drawing.Point(6, 279)
        Me.trkTemperature.Maximum = 333150
        Me.trkTemperature.Minimum = 183150
        Me.trkTemperature.Name = "trkTemperature"
        Me.trkTemperature.Size = New System.Drawing.Size(334, 25)
        Me.trkTemperature.SmallChange = 250
        Me.trkTemperature.TabIndex = 19
        Me.trkTemperature.TickFrequency = 15
        Me.trkTemperature.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkTemperature, "Move the slider to set customized temperature")
        Me.trkTemperature.Value = 333150
        '
        'lblPrecipitations
        '
        Me.lblPrecipitations.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPrecipitations.Location = New System.Drawing.Point(152, 307)
        Me.lblPrecipitations.Name = "lblPrecipitations"
        Me.lblPrecipitations.Size = New System.Drawing.Size(188, 20)
        Me.lblPrecipitations.TabIndex = 21
        Me.lblPrecipitations.Text = "20.00"
        Me.lblPrecipitations.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblPrecipitations, "Current precipitations")
        '
        'trkPrecipitations
        '
        Me.trkPrecipitations.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkPrecipitations.AutoSize = False
        Me.trkPrecipitations.LargeChange = 1000
        Me.trkPrecipitations.Location = New System.Drawing.Point(6, 330)
        Me.trkPrecipitations.Maximum = 29972
        Me.trkPrecipitations.Name = "trkPrecipitations"
        Me.trkPrecipitations.Size = New System.Drawing.Size(334, 25)
        Me.trkPrecipitations.SmallChange = 100
        Me.trkPrecipitations.TabIndex = 22
        Me.trkPrecipitations.TickFrequency = 15
        Me.trkPrecipitations.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkPrecipitations, "Move the slider to set customized precipitations")
        Me.trkPrecipitations.Value = 29972
        '
        'lblSnowCover
        '
        Me.lblSnowCover.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSnowCover.Location = New System.Drawing.Point(152, 358)
        Me.lblSnowCover.Name = "lblSnowCover"
        Me.lblSnowCover.Size = New System.Drawing.Size(188, 20)
        Me.lblSnowCover.TabIndex = 24
        Me.lblSnowCover.Text = "20.00"
        Me.lblSnowCover.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblSnowCover, "Current snow cover")
        '
        'trkSnowCover
        '
        Me.trkSnowCover.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkSnowCover.AutoSize = False
        Me.trkSnowCover.LargeChange = 50
        Me.trkSnowCover.Location = New System.Drawing.Point(6, 381)
        Me.trkSnowCover.Maximum = 750
        Me.trkSnowCover.Name = "trkSnowCover"
        Me.trkSnowCover.Size = New System.Drawing.Size(334, 25)
        Me.trkSnowCover.SmallChange = 10
        Me.trkSnowCover.TabIndex = 25
        Me.trkSnowCover.TickFrequency = 15
        Me.trkSnowCover.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkSnowCover, "Move the slider to set customized snow cover")
        Me.trkSnowCover.Value = 750
        '
        'lblThunderstormIntensity
        '
        Me.lblThunderstormIntensity.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblThunderstormIntensity.Location = New System.Drawing.Point(152, 409)
        Me.lblThunderstormIntensity.Name = "lblThunderstormIntensity"
        Me.lblThunderstormIntensity.Size = New System.Drawing.Size(188, 20)
        Me.lblThunderstormIntensity.TabIndex = 27
        Me.lblThunderstormIntensity.Text = "20.00"
        Me.lblThunderstormIntensity.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTip1.SetToolTip(Me.lblThunderstormIntensity, "Current thunderstom intensity")
        '
        'trkThunderstormIntensity
        '
        Me.trkThunderstormIntensity.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkThunderstormIntensity.AutoSize = False
        Me.trkThunderstormIntensity.LargeChange = 10
        Me.trkThunderstormIntensity.Location = New System.Drawing.Point(6, 432)
        Me.trkThunderstormIntensity.Maximum = 100
        Me.trkThunderstormIntensity.Name = "trkThunderstormIntensity"
        Me.trkThunderstormIntensity.Size = New System.Drawing.Size(334, 25)
        Me.trkThunderstormIntensity.TabIndex = 28
        Me.trkThunderstormIntensity.TickFrequency = 15
        Me.trkThunderstormIntensity.TickStyle = System.Windows.Forms.TickStyle.None
        Me.ToolTip1.SetToolTip(Me.trkThunderstormIntensity, "Move the slider to set customized thunderstorm intensity")
        Me.trkThunderstormIntensity.Value = 100
        '
        'chkPrimaryDynamic
        '
        Me.chkPrimaryDynamic.AutoSize = True
        Me.chkPrimaryDynamic.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkPrimaryDynamic.Location = New System.Drawing.Point(282, 31)
        Me.chkPrimaryDynamic.Name = "chkPrimaryDynamic"
        Me.chkPrimaryDynamic.Size = New System.Drawing.Size(38, 24)
        Me.chkPrimaryDynamic.TabIndex = 64
        Me.chkPrimaryDynamic.Tag = "8"
        Me.chkPrimaryDynamic.Text = "D"
        Me.ToolTip1.SetToolTip(Me.chkPrimaryDynamic, "Check if primary usage is Dynamic soaring")
        Me.chkPrimaryDynamic.UseVisualStyleBackColor = True
        '
        'chkPrimaryWave
        '
        Me.chkPrimaryWave.AccessibleDescription = ""
        Me.chkPrimaryWave.AutoSize = True
        Me.chkPrimaryWave.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkPrimaryWave.Location = New System.Drawing.Point(228, 31)
        Me.chkPrimaryWave.Name = "chkPrimaryWave"
        Me.chkPrimaryWave.Size = New System.Drawing.Size(42, 24)
        Me.chkPrimaryWave.TabIndex = 63
        Me.chkPrimaryWave.Tag = "8"
        Me.chkPrimaryWave.Text = "W"
        Me.ToolTip1.SetToolTip(Me.chkPrimaryWave, "Check if primary usage is Wave soaring")
        Me.chkPrimaryWave.UseVisualStyleBackColor = True
        '
        'chkPrimaryThermal
        '
        Me.chkPrimaryThermal.AutoSize = True
        Me.chkPrimaryThermal.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkPrimaryThermal.Location = New System.Drawing.Point(181, 31)
        Me.chkPrimaryThermal.Name = "chkPrimaryThermal"
        Me.chkPrimaryThermal.Size = New System.Drawing.Size(36, 24)
        Me.chkPrimaryThermal.TabIndex = 62
        Me.chkPrimaryThermal.Tag = "8"
        Me.chkPrimaryThermal.Text = "T"
        Me.ToolTip1.SetToolTip(Me.chkPrimaryThermal, "Check if primary usage is Thermal soaring")
        Me.chkPrimaryThermal.UseVisualStyleBackColor = True
        '
        'chkPrimaryRidge
        '
        Me.chkPrimaryRidge.AutoSize = True
        Me.chkPrimaryRidge.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.chkPrimaryRidge.Location = New System.Drawing.Point(133, 31)
        Me.chkPrimaryRidge.Name = "chkPrimaryRidge"
        Me.chkPrimaryRidge.Size = New System.Drawing.Size(37, 24)
        Me.chkPrimaryRidge.TabIndex = 61
        Me.chkPrimaryRidge.Tag = "8"
        Me.chkPrimaryRidge.Text = "R"
        Me.ToolTip1.SetToolTip(Me.chkPrimaryRidge, "Check if primary usage is Ridge soaring")
        Me.chkPrimaryRidge.UseVisualStyleBackColor = True
        '
        'splitContainerWeatherPresetManager
        '
        Me.splitContainerWeatherPresetManager.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.splitContainerWeatherPresetManager.Location = New System.Drawing.Point(0, 0)
        Me.splitContainerWeatherPresetManager.Name = "splitContainerWeatherPresetManager"
        '
        'splitContainerWeatherPresetManager.Panel1
        '
        Me.splitContainerWeatherPresetManager.Panel1.Controls.Add(Me.weatherProfilesDataGrid)
        '
        'splitContainerWeatherPresetManager.Panel2
        '
        Me.splitContainerWeatherPresetManager.Panel2.Controls.Add(Me.TabControl1)
        Me.splitContainerWeatherPresetManager.Size = New System.Drawing.Size(1494, 911)
        Me.splitContainerWeatherPresetManager.SplitterDistance = 597
        Me.splitContainerWeatherPresetManager.TabIndex = 4
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tbdgPresetDetails)
        Me.TabControl1.Controls.Add(Me.tbpgProfileDetails)
        Me.TabControl1.Controls.Add(Me.tbpgLayers)
        Me.TabControl1.Controls.Add(Me.tbpgCustom)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(893, 911)
        Me.TabControl1.TabIndex = 0
        '
        'tbdgPresetDetails
        '
        Me.tbdgPresetDetails.Controls.Add(Me.TableLayoutPanel1)
        Me.tbdgPresetDetails.Location = New System.Drawing.Point(4, 29)
        Me.tbdgPresetDetails.Name = "tbdgPresetDetails"
        Me.tbdgPresetDetails.Padding = New System.Windows.Forms.Padding(3)
        Me.tbdgPresetDetails.Size = New System.Drawing.Size(885, 878)
        Me.tbdgPresetDetails.TabIndex = 0
        Me.tbdgPresetDetails.Text = "Preset Data"
        Me.tbdgPresetDetails.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.pnlPresetActions, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.pnlPresetUserData, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.pnlPresetBuildInData, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(879, 872)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'pnlPresetActions
        '
        Me.pnlPresetActions.AutoScroll = True
        Me.pnlPresetActions.Controls.Add(Me.btnActionDelete)
        Me.pnlPresetActions.Controls.Add(Me.btnActionSave)
        Me.pnlPresetActions.Controls.Add(Me.btnActionNew)
        Me.pnlPresetActions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlPresetActions.Location = New System.Drawing.Point(3, 819)
        Me.pnlPresetActions.Name = "pnlPresetActions"
        Me.pnlPresetActions.Size = New System.Drawing.Size(873, 50)
        Me.pnlPresetActions.TabIndex = 2
        '
        'pnlPresetUserData
        '
        Me.pnlPresetUserData.AutoScroll = True
        Me.pnlPresetUserData.Controls.Add(Me.grpUserData)
        Me.pnlPresetUserData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlPresetUserData.Location = New System.Drawing.Point(3, 411)
        Me.pnlPresetUserData.Name = "pnlPresetUserData"
        Me.pnlPresetUserData.Size = New System.Drawing.Size(873, 402)
        Me.pnlPresetUserData.TabIndex = 1
        '
        'grpUserData
        '
        Me.grpUserData.Controls.Add(Me.chkPrimaryDynamic)
        Me.grpUserData.Controls.Add(Me.chkPrimaryWave)
        Me.grpUserData.Controls.Add(Me.chkPrimaryThermal)
        Me.grpUserData.Controls.Add(Me.chkPrimaryRidge)
        Me.grpUserData.Controls.Add(Me.Label18)
        Me.grpUserData.Controls.Add(Me.Label4)
        Me.grpUserData.Controls.Add(Me.txtComments)
        Me.grpUserData.Controls.Add(Me.txtStrength)
        Me.grpUserData.Controls.Add(Me.txtRating)
        Me.grpUserData.Controls.Add(Me.Label3)
        Me.grpUserData.Controls.Add(Me.trkStrength)
        Me.grpUserData.Controls.Add(Me.Label2)
        Me.grpUserData.Controls.Add(Me.trkRating)
        Me.grpUserData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpUserData.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.grpUserData.Location = New System.Drawing.Point(0, 0)
        Me.grpUserData.Name = "grpUserData"
        Me.grpUserData.Size = New System.Drawing.Size(873, 402)
        Me.grpUserData.TabIndex = 1
        Me.grpUserData.TabStop = False
        Me.grpUserData.Text = "Your own data / appreciation"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Label18.Location = New System.Drawing.Point(6, 32)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(101, 20)
        Me.Label18.TabIndex = 60
        Me.Label18.Text = "Primary usage"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Label4.Location = New System.Drawing.Point(6, 147)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(83, 20)
        Me.Label4.TabIndex = 58
        Me.Label4.Text = "Comments"
        '
        'txtStrength
        '
        Me.txtStrength.Font = New System.Drawing.Font("Segoe UI Variable Display", 15.0!, System.Drawing.FontStyle.Bold)
        Me.txtStrength.Location = New System.Drawing.Point(366, 100)
        Me.txtStrength.Name = "txtStrength"
        Me.txtStrength.ReadOnly = True
        Me.txtStrength.Size = New System.Drawing.Size(73, 38)
        Me.txtStrength.TabIndex = 6
        Me.txtStrength.Text = "0"
        Me.txtStrength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtRating
        '
        Me.txtRating.Font = New System.Drawing.Font("Segoe UI Variable Display", 15.0!, System.Drawing.FontStyle.Bold)
        Me.txtRating.Location = New System.Drawing.Point(366, 59)
        Me.txtRating.Name = "txtRating"
        Me.txtRating.ReadOnly = True
        Me.txtRating.Size = New System.Drawing.Size(73, 38)
        Me.txtRating.TabIndex = 5
        Me.txtRating.Text = "0"
        Me.txtRating.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Label3.Location = New System.Drawing.Point(6, 112)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(113, 20)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Overall Strength"
        '
        'trkStrength
        '
        Me.trkStrength.AutoSize = False
        Me.trkStrength.LargeChange = 1
        Me.trkStrength.Location = New System.Drawing.Point(133, 102)
        Me.trkStrength.Name = "trkStrength"
        Me.trkStrength.Size = New System.Drawing.Size(227, 35)
        Me.trkStrength.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Label2.Location = New System.Drawing.Point(6, 71)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(50, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Rating"
        '
        'trkRating
        '
        Me.trkRating.AutoSize = False
        Me.trkRating.LargeChange = 1
        Me.trkRating.Location = New System.Drawing.Point(133, 61)
        Me.trkRating.Maximum = 5
        Me.trkRating.Name = "trkRating"
        Me.trkRating.Size = New System.Drawing.Size(227, 35)
        Me.trkRating.TabIndex = 0
        '
        'pnlPresetBuildInData
        '
        Me.pnlPresetBuildInData.AutoScroll = True
        Me.pnlPresetBuildInData.Controls.Add(Me.grpPresetBuiltInData)
        Me.pnlPresetBuildInData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlPresetBuildInData.Location = New System.Drawing.Point(3, 3)
        Me.pnlPresetBuildInData.Name = "pnlPresetBuildInData"
        Me.pnlPresetBuildInData.Size = New System.Drawing.Size(873, 402)
        Me.pnlPresetBuildInData.TabIndex = 0
        '
        'grpPresetBuiltInData
        '
        Me.grpPresetBuiltInData.Controls.Add(Me.Label17)
        Me.grpPresetBuiltInData.Controls.Add(Me.txtPresetDescription)
        Me.grpPresetBuiltInData.Controls.Add(Me.chkSoaringTypeDynamic)
        Me.grpPresetBuiltInData.Controls.Add(Me.chkSoaringTypeWave)
        Me.grpPresetBuiltInData.Controls.Add(Me.chkSoaringTypeThermal)
        Me.grpPresetBuiltInData.Controls.Add(Me.chkSoaringTypeRidge)
        Me.grpPresetBuiltInData.Controls.Add(Me.lblSoaringType)
        Me.grpPresetBuiltInData.Controls.Add(Me.Label1)
        Me.grpPresetBuiltInData.Controls.Add(Me.txtPresetTitle)
        Me.grpPresetBuiltInData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpPresetBuiltInData.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.grpPresetBuiltInData.Location = New System.Drawing.Point(0, 0)
        Me.grpPresetBuiltInData.Name = "grpPresetBuiltInData"
        Me.grpPresetBuiltInData.Size = New System.Drawing.Size(873, 402)
        Me.grpPresetBuiltInData.TabIndex = 0
        Me.grpPresetBuiltInData.TabStop = False
        Me.grpPresetBuiltInData.Text = "Designer provided data"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Label17.Location = New System.Drawing.Point(6, 92)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(84, 20)
        Me.Label17.TabIndex = 56
        Me.Label17.Text = "Description"
        '
        'lblSoaringType
        '
        Me.lblSoaringType.AutoSize = True
        Me.lblSoaringType.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.lblSoaringType.Location = New System.Drawing.Point(6, 60)
        Me.lblSoaringType.Name = "lblSoaringType"
        Me.lblSoaringType.Size = New System.Drawing.Size(94, 20)
        Me.lblSoaringType.TabIndex = 29
        Me.lblSoaringType.Text = "Soaring Type"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Label1.Location = New System.Drawing.Point(6, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Title"
        '
        'tbpgProfileDetails
        '
        Me.tbpgProfileDetails.Controls.Add(Me.weatherDetailsDataGrid)
        Me.tbpgProfileDetails.Controls.Add(Me.SplitContainer1)
        Me.tbpgProfileDetails.Location = New System.Drawing.Point(4, 29)
        Me.tbpgProfileDetails.Name = "tbpgProfileDetails"
        Me.tbpgProfileDetails.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpgProfileDetails.Size = New System.Drawing.Size(885, 878)
        Me.tbpgProfileDetails.TabIndex = 1
        Me.tbpgProfileDetails.Text = "Profile Details"
        Me.tbpgProfileDetails.UseVisualStyleBackColor = True
        '
        'weatherDetailsDataGrid
        '
        Me.weatherDetailsDataGrid.AllowUserToAddRows = False
        Me.weatherDetailsDataGrid.AllowUserToDeleteRows = False
        Me.weatherDetailsDataGrid.AllowUserToResizeRows = False
        Me.weatherDetailsDataGrid.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.weatherDetailsDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.weatherDetailsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.weatherDetailsDataGrid.DefaultCellStyle = DataGridViewCellStyle1
        Me.weatherDetailsDataGrid.Location = New System.Drawing.Point(6, 6)
        Me.weatherDetailsDataGrid.MultiSelect = False
        Me.weatherDetailsDataGrid.Name = "weatherDetailsDataGrid"
        Me.weatherDetailsDataGrid.ReadOnly = True
        Me.weatherDetailsDataGrid.RowHeadersVisible = False
        Me.weatherDetailsDataGrid.RowHeadersWidth = 25
        Me.weatherDetailsDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.weatherDetailsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.weatherDetailsDataGrid.Size = New System.Drawing.Size(873, 248)
        Me.weatherDetailsDataGrid.TabIndex = 6
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(6, 260)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.cloudLayersDatagrid)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.windLayersDatagrid)
        Me.SplitContainer1.Size = New System.Drawing.Size(873, 612)
        Me.SplitContainer1.SplitterDistance = 306
        Me.SplitContainer1.TabIndex = 7
        '
        'cloudLayersDatagrid
        '
        Me.cloudLayersDatagrid.AllowUserToAddRows = False
        Me.cloudLayersDatagrid.AllowUserToDeleteRows = False
        Me.cloudLayersDatagrid.AllowUserToResizeRows = False
        Me.cloudLayersDatagrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.cloudLayersDatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.cloudLayersDatagrid.DefaultCellStyle = DataGridViewCellStyle2
        Me.cloudLayersDatagrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cloudLayersDatagrid.Location = New System.Drawing.Point(0, 0)
        Me.cloudLayersDatagrid.Name = "cloudLayersDatagrid"
        Me.cloudLayersDatagrid.ReadOnly = True
        Me.cloudLayersDatagrid.RowHeadersWidth = 47
        Me.cloudLayersDatagrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.cloudLayersDatagrid.Size = New System.Drawing.Size(873, 306)
        Me.cloudLayersDatagrid.TabIndex = 8
        '
        'windLayersDatagrid
        '
        Me.windLayersDatagrid.AllowUserToAddRows = False
        Me.windLayersDatagrid.AllowUserToDeleteRows = False
        Me.windLayersDatagrid.AllowUserToResizeRows = False
        Me.windLayersDatagrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.windLayersDatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.windLayersDatagrid.DefaultCellStyle = DataGridViewCellStyle3
        Me.windLayersDatagrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.windLayersDatagrid.Location = New System.Drawing.Point(0, 0)
        Me.windLayersDatagrid.Name = "windLayersDatagrid"
        Me.windLayersDatagrid.ReadOnly = True
        Me.windLayersDatagrid.RowHeadersWidth = 47
        Me.windLayersDatagrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.windLayersDatagrid.Size = New System.Drawing.Size(873, 302)
        Me.windLayersDatagrid.TabIndex = 9
        '
        'tbpgLayers
        '
        Me.tbpgLayers.Controls.Add(Me.FullWeatherGraphPanel1)
        Me.tbpgLayers.Location = New System.Drawing.Point(4, 29)
        Me.tbpgLayers.Margin = New System.Windows.Forms.Padding(0)
        Me.tbpgLayers.Name = "tbpgLayers"
        Me.tbpgLayers.Size = New System.Drawing.Size(885, 878)
        Me.tbpgLayers.TabIndex = 8
        Me.tbpgLayers.Text = "Graph"
        Me.tbpgLayers.UseVisualStyleBackColor = True
        '
        'FullWeatherGraphPanel1
        '
        Me.FullWeatherGraphPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FullWeatherGraphPanel1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FullWeatherGraphPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FullWeatherGraphPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FullWeatherGraphPanel1.Name = "FullWeatherGraphPanel1"
        Me.FullWeatherGraphPanel1.Size = New System.Drawing.Size(885, 878)
        Me.FullWeatherGraphPanel1.TabIndex = 6
        '
        'tbpgCustom
        '
        Me.tbpgCustom.Controls.Add(Me.GroupBox2)
        Me.tbpgCustom.Controls.Add(Me.GroupBox1)
        Me.tbpgCustom.Controls.Add(Me.ElementHost1)
        Me.tbpgCustom.Location = New System.Drawing.Point(4, 29)
        Me.tbpgCustom.Name = "tbpgCustom"
        Me.tbpgCustom.Size = New System.Drawing.Size(885, 878)
        Me.tbpgCustom.TabIndex = 9
        Me.tbpgCustom.Text = "Customization"
        Me.tbpgCustom.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.lblThunderstormIntensityLabel)
        Me.GroupBox2.Controls.Add(Me.lblThunderstormIntensity)
        Me.GroupBox2.Controls.Add(Me.trkThunderstormIntensity)
        Me.GroupBox2.Controls.Add(Me.lblSnowCoverLabel)
        Me.GroupBox2.Controls.Add(Me.lblSnowCover)
        Me.GroupBox2.Controls.Add(Me.trkSnowCover)
        Me.GroupBox2.Controls.Add(Me.lblPrecipitationsLabel)
        Me.GroupBox2.Controls.Add(Me.lblPrecipitations)
        Me.GroupBox2.Controls.Add(Me.trkPrecipitations)
        Me.GroupBox2.Controls.Add(Me.lblTemperatureLabel)
        Me.GroupBox2.Controls.Add(Me.lblTemperature)
        Me.GroupBox2.Controls.Add(Me.trkTemperature)
        Me.GroupBox2.Controls.Add(Me.lblBarometricPressureLabel)
        Me.GroupBox2.Controls.Add(Me.lblBarometricPressure)
        Me.GroupBox2.Controls.Add(Me.trkBaroPressure)
        Me.GroupBox2.Controls.Add(Me.lblHumidityIndexLabel)
        Me.GroupBox2.Controls.Add(Me.lblHumidityIndex)
        Me.GroupBox2.Controls.Add(Me.trkHumidityIndex)
        Me.GroupBox2.Controls.Add(Me.btnWindDirNW)
        Me.GroupBox2.Controls.Add(Me.btnWindDirSW)
        Me.GroupBox2.Controls.Add(Me.btnWindDirSE)
        Me.GroupBox2.Controls.Add(Me.btnWindDirNE)
        Me.GroupBox2.Controls.Add(Me.btnWindDirW)
        Me.GroupBox2.Controls.Add(Me.btnWindDirE)
        Me.GroupBox2.Controls.Add(Me.btnWindDirS)
        Me.GroupBox2.Controls.Add(Me.btnWindDirN)
        Me.GroupBox2.Controls.Add(Me.lblWindDirectionMultiLabel)
        Me.GroupBox2.Controls.Add(Me.lblWindDirectionMulti)
        Me.GroupBox2.Controls.Add(Me.trkWindDirectionMulti)
        Me.GroupBox2.Controls.Add(Me.lblWindDirectionSingleLabel)
        Me.GroupBox2.Controls.Add(Me.lblWindDirectionSingle)
        Me.GroupBox2.Controls.Add(Me.trkWindDirectionSingle)
        Me.GroupBox2.Location = New System.Drawing.Point(536, 346)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(346, 467)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Modifications"
        '
        'lblThunderstormIntensityLabel
        '
        Me.lblThunderstormIntensityLabel.AutoSize = True
        Me.lblThunderstormIntensityLabel.Location = New System.Drawing.Point(6, 409)
        Me.lblThunderstormIntensityLabel.Name = "lblThunderstormIntensityLabel"
        Me.lblThunderstormIntensityLabel.Size = New System.Drawing.Size(162, 20)
        Me.lblThunderstormIntensityLabel.TabIndex = 26
        Me.lblThunderstormIntensityLabel.Text = "Thunderstorm Intensity"
        '
        'lblSnowCoverLabel
        '
        Me.lblSnowCoverLabel.AutoSize = True
        Me.lblSnowCoverLabel.Location = New System.Drawing.Point(6, 358)
        Me.lblSnowCoverLabel.Name = "lblSnowCoverLabel"
        Me.lblSnowCoverLabel.Size = New System.Drawing.Size(89, 20)
        Me.lblSnowCoverLabel.TabIndex = 23
        Me.lblSnowCoverLabel.Text = "Snow Cover"
        '
        'lblPrecipitationsLabel
        '
        Me.lblPrecipitationsLabel.AutoSize = True
        Me.lblPrecipitationsLabel.Location = New System.Drawing.Point(6, 307)
        Me.lblPrecipitationsLabel.Name = "lblPrecipitationsLabel"
        Me.lblPrecipitationsLabel.Size = New System.Drawing.Size(97, 20)
        Me.lblPrecipitationsLabel.TabIndex = 20
        Me.lblPrecipitationsLabel.Text = "Precipitations"
        '
        'lblTemperatureLabel
        '
        Me.lblTemperatureLabel.AutoSize = True
        Me.lblTemperatureLabel.Location = New System.Drawing.Point(6, 256)
        Me.lblTemperatureLabel.Name = "lblTemperatureLabel"
        Me.lblTemperatureLabel.Size = New System.Drawing.Size(92, 20)
        Me.lblTemperatureLabel.TabIndex = 17
        Me.lblTemperatureLabel.Text = "Temperature"
        '
        'lblBarometricPressureLabel
        '
        Me.lblBarometricPressureLabel.AutoSize = True
        Me.lblBarometricPressureLabel.Location = New System.Drawing.Point(6, 205)
        Me.lblBarometricPressureLabel.Name = "lblBarometricPressureLabel"
        Me.lblBarometricPressureLabel.Size = New System.Drawing.Size(140, 20)
        Me.lblBarometricPressureLabel.TabIndex = 14
        Me.lblBarometricPressureLabel.Text = "Barometric Pressure"
        '
        'lblHumidityIndexLabel
        '
        Me.lblHumidityIndexLabel.AutoSize = True
        Me.lblHumidityIndexLabel.Location = New System.Drawing.Point(6, 154)
        Me.lblHumidityIndexLabel.Name = "lblHumidityIndexLabel"
        Me.lblHumidityIndexLabel.Size = New System.Drawing.Size(109, 20)
        Me.lblHumidityIndexLabel.TabIndex = 11
        Me.lblHumidityIndexLabel.Text = "Humidity Index"
        '
        'btnWindDirNW
        '
        Me.btnWindDirNW.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirNW.Location = New System.Drawing.Point(271, 74)
        Me.btnWindDirNW.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirNW.Name = "btnWindDirNW"
        Me.btnWindDirNW.Size = New System.Drawing.Size(38, 23)
        Me.btnWindDirNW.TabIndex = 7
        Me.btnWindDirNW.Tag = "315"
        Me.btnWindDirNW.Text = "NW"
        Me.btnWindDirNW.UseVisualStyleBackColor = True
        '
        'btnWindDirSW
        '
        Me.btnWindDirSW.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirSW.Location = New System.Drawing.Point(194, 74)
        Me.btnWindDirSW.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirSW.Name = "btnWindDirSW"
        Me.btnWindDirSW.Size = New System.Drawing.Size(38, 23)
        Me.btnWindDirSW.TabIndex = 6
        Me.btnWindDirSW.Tag = "225"
        Me.btnWindDirSW.Text = "SW"
        Me.btnWindDirSW.UseVisualStyleBackColor = True
        '
        'btnWindDirSE
        '
        Me.btnWindDirSE.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirSE.Location = New System.Drawing.Point(116, 74)
        Me.btnWindDirSE.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirSE.Name = "btnWindDirSE"
        Me.btnWindDirSE.Size = New System.Drawing.Size(38, 23)
        Me.btnWindDirSE.TabIndex = 5
        Me.btnWindDirSE.Tag = "135"
        Me.btnWindDirSE.Text = "SE"
        Me.btnWindDirSE.UseVisualStyleBackColor = True
        '
        'btnWindDirNE
        '
        Me.btnWindDirNE.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirNE.Location = New System.Drawing.Point(38, 74)
        Me.btnWindDirNE.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirNE.Name = "btnWindDirNE"
        Me.btnWindDirNE.Size = New System.Drawing.Size(38, 23)
        Me.btnWindDirNE.TabIndex = 4
        Me.btnWindDirNE.Tag = "45"
        Me.btnWindDirNE.Text = "NE"
        Me.btnWindDirNE.UseVisualStyleBackColor = True
        '
        'btnWindDirW
        '
        Me.btnWindDirW.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirW.Location = New System.Drawing.Point(236, 74)
        Me.btnWindDirW.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirW.Name = "btnWindDirW"
        Me.btnWindDirW.Size = New System.Drawing.Size(28, 23)
        Me.btnWindDirW.TabIndex = 36
        Me.btnWindDirW.Tag = "270"
        Me.btnWindDirW.Text = "W"
        Me.btnWindDirW.UseVisualStyleBackColor = True
        '
        'btnWindDirE
        '
        Me.btnWindDirE.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirE.Location = New System.Drawing.Point(81, 74)
        Me.btnWindDirE.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirE.Name = "btnWindDirE"
        Me.btnWindDirE.Size = New System.Drawing.Size(28, 23)
        Me.btnWindDirE.TabIndex = 35
        Me.btnWindDirE.Tag = "90"
        Me.btnWindDirE.Text = "E"
        Me.btnWindDirE.UseVisualStyleBackColor = True
        '
        'btnWindDirS
        '
        Me.btnWindDirS.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirS.Location = New System.Drawing.Point(159, 74)
        Me.btnWindDirS.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirS.Name = "btnWindDirS"
        Me.btnWindDirS.Size = New System.Drawing.Size(28, 23)
        Me.btnWindDirS.TabIndex = 34
        Me.btnWindDirS.Tag = "180"
        Me.btnWindDirS.Text = "S"
        Me.btnWindDirS.UseVisualStyleBackColor = True
        '
        'btnWindDirN
        '
        Me.btnWindDirN.Font = New System.Drawing.Font("Segoe UI Variable Display", 8.0!)
        Me.btnWindDirN.Location = New System.Drawing.Point(4, 74)
        Me.btnWindDirN.Margin = New System.Windows.Forms.Padding(0)
        Me.btnWindDirN.Name = "btnWindDirN"
        Me.btnWindDirN.Size = New System.Drawing.Size(28, 23)
        Me.btnWindDirN.TabIndex = 3
        Me.btnWindDirN.Tag = "0"
        Me.btnWindDirN.Text = "N"
        Me.btnWindDirN.UseVisualStyleBackColor = True
        '
        'lblWindDirectionMultiLabel
        '
        Me.lblWindDirectionMultiLabel.AutoSize = True
        Me.lblWindDirectionMultiLabel.Location = New System.Drawing.Point(6, 103)
        Me.lblWindDirectionMultiLabel.Name = "lblWindDirectionMultiLabel"
        Me.lblWindDirectionMultiLabel.Size = New System.Drawing.Size(137, 20)
        Me.lblWindDirectionMultiLabel.TabIndex = 8
        Me.lblWindDirectionMultiLabel.Text = "Wind direction shift"
        '
        'lblWindDirectionSingleLabel
        '
        Me.lblWindDirectionSingleLabel.AutoSize = True
        Me.lblWindDirectionSingleLabel.Location = New System.Drawing.Point(6, 23)
        Me.lblWindDirectionSingleLabel.Name = "lblWindDirectionSingleLabel"
        Me.lblWindDirectionSingleLabel.Size = New System.Drawing.Size(105, 20)
        Me.lblWindDirectionSingleLabel.TabIndex = 0
        Me.lblWindDirectionSingleLabel.Text = "Wind direction"
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.trkCameraTrack)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.lblFOVValue)
        Me.GroupBox1.Controls.Add(Me.trkViewFOV)
        Me.GroupBox1.Controls.Add(Me.lblDistanceValue)
        Me.GroupBox1.Controls.Add(Me.trkViewDistance)
        Me.GroupBox1.Controls.Add(Me.lblTiltValue)
        Me.GroupBox1.Controls.Add(Me.trkViewTilt)
        Me.GroupBox1.Controls.Add(Me.lblHeightValue)
        Me.GroupBox1.Controls.Add(Me.trkViewHeight)
        Me.GroupBox1.Controls.Add(Me.lblAngleValue)
        Me.GroupBox1.Controls.Add(Me.trkViewRotation)
        Me.GroupBox1.Location = New System.Drawing.Point(536, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(346, 336)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "View Settings"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(6, 23)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(189, 20)
        Me.Label10.TabIndex = 0
        Me.Label10.Text = "Default camera movements"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(6, 74)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(64, 20)
        Me.Label9.TabIndex = 2
        Me.Label9.Text = "Rotation"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(6, 125)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(53, 20)
        Me.Label8.TabIndex = 5
        Me.Label8.Text = "Height"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 227)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(28, 20)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "Tilt"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 176)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 20)
        Me.Label6.TabIndex = 8
        Me.Label6.Text = "Distance"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 278)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(36, 20)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "FOV"
        '
        'ElementHost1
        '
        Me.ElementHost1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ElementHost1.BackColor = System.Drawing.Color.FromArgb(CType(CType(225, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.ElementHost1.Location = New System.Drawing.Point(0, 0)
        Me.ElementHost1.Name = "ElementHost1"
        Me.ElementHost1.Size = New System.Drawing.Size(530, 875)
        Me.ElementHost1.TabIndex = 0
        Me.ElementHost1.Text = "ElementHost1"
        Me.ElementHost1.Child = Me.WindLayers3DControl1
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.Controls.Add(Me.optGridDetails)
        Me.FlowLayoutPanel1.Controls.Add(Me.optGridOnly)
        Me.FlowLayoutPanel1.Controls.Add(Me.optDetailsOnly)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 919)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(597, 60)
        Me.FlowLayoutPanel1.TabIndex = 5
        '
        'optGridDetails
        '
        Me.optGridDetails.AutoSize = True
        Me.optGridDetails.Checked = True
        Me.optGridDetails.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.optGridDetails.Location = New System.Drawing.Point(3, 3)
        Me.optGridDetails.Name = "optGridDetails"
        Me.optGridDetails.Size = New System.Drawing.Size(131, 26)
        Me.optGridDetails.TabIndex = 0
        Me.optGridDetails.TabStop = True
        Me.optGridDetails.Text = "Grid && Details"
        Me.optGridDetails.UseVisualStyleBackColor = True
        '
        'optGridOnly
        '
        Me.optGridOnly.AutoSize = True
        Me.optGridOnly.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.optGridOnly.Location = New System.Drawing.Point(140, 3)
        Me.optGridOnly.Name = "optGridOnly"
        Me.optGridOnly.Size = New System.Drawing.Size(99, 26)
        Me.optGridOnly.TabIndex = 1
        Me.optGridOnly.Text = "Grid Only"
        Me.optGridOnly.UseVisualStyleBackColor = True
        '
        'optDetailsOnly
        '
        Me.optDetailsOnly.AutoSize = True
        Me.optDetailsOnly.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!)
        Me.optDetailsOnly.Location = New System.Drawing.Point(245, 3)
        Me.optDetailsOnly.Name = "optDetailsOnly"
        Me.optDetailsOnly.Size = New System.Drawing.Size(118, 26)
        Me.optDetailsOnly.TabIndex = 2
        Me.optDetailsOnly.Text = "Details Only"
        Me.optDetailsOnly.UseVisualStyleBackColor = True
        '
        'WeatherPresetsBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 22.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(1494, 1032)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnActionImport)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.splitContainerWeatherPresetManager)
        Me.Controls.Add(Me.btnClose)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 11.12727!, System.Drawing.FontStyle.Bold)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MinimumSize = New System.Drawing.Size(1512, 1050)
        Me.Name = "WeatherPresetsBrowser"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Weather Presets Management"
        CType(Me.weatherProfilesDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkBaroPressure, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkHumidityIndex, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkWindDirectionMulti, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkWindDirectionSingle, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkCameraTrack, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkViewFOV, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkViewDistance, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkViewTilt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkViewHeight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkViewRotation, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkTemperature, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkPrecipitations, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkSnowCover, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkThunderstormIntensity, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainerWeatherPresetManager.Panel1.ResumeLayout(False)
        Me.splitContainerWeatherPresetManager.Panel2.ResumeLayout(False)
        CType(Me.splitContainerWeatherPresetManager, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainerWeatherPresetManager.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.tbdgPresetDetails.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.pnlPresetActions.ResumeLayout(False)
        Me.pnlPresetUserData.ResumeLayout(False)
        Me.grpUserData.ResumeLayout(False)
        Me.grpUserData.PerformLayout()
        CType(Me.trkStrength, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkRating, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlPresetBuildInData.ResumeLayout(False)
        Me.grpPresetBuiltInData.ResumeLayout(False)
        Me.grpPresetBuiltInData.PerformLayout()
        Me.tbpgProfileDetails.ResumeLayout(False)
        CType(Me.weatherDetailsDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.cloudLayersDatagrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.windLayersDatagrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tbpgLayers.ResumeLayout(False)
        Me.tbpgCustom.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents weatherProfilesDataGrid As DataGridView
    Friend WithEvents btnClose As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents splitContainerWeatherPresetManager As SplitContainer
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents tbdgPresetDetails As TabPage
    Friend WithEvents tbpgProfileDetails As TabPage
    Friend WithEvents tbpgLayers As TabPage
    Friend WithEvents FullWeatherGraphPanel1 As CommonLibrary.FullWeatherGraphPanel
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents optGridDetails As RadioButton
    Friend WithEvents optGridOnly As RadioButton
    Friend WithEvents optDetailsOnly As RadioButton
    Friend WithEvents weatherDetailsDataGrid As DataGridView
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents cloudLayersDatagrid As DataGridView
    Friend WithEvents windLayersDatagrid As DataGridView
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents pnlPresetActions As Panel
    Friend WithEvents pnlPresetUserData As Panel
    Friend WithEvents pnlPresetBuildInData As Panel
    Friend WithEvents grpUserData As GroupBox
    Friend WithEvents grpPresetBuiltInData As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txtPresetTitle As TextBox
    Friend WithEvents chkSoaringTypeDynamic As CheckBox
    Friend WithEvents chkSoaringTypeWave As CheckBox
    Friend WithEvents chkSoaringTypeThermal As CheckBox
    Friend WithEvents chkSoaringTypeRidge As CheckBox
    Friend WithEvents lblSoaringType As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents txtPresetDescription As TextBox
    Friend WithEvents txtRating As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents trkStrength As TrackBar
    Friend WithEvents Label2 As Label
    Friend WithEvents trkRating As TrackBar
    Friend WithEvents Label4 As Label
    Friend WithEvents txtComments As TextBox
    Friend WithEvents txtStrength As TextBox
    Friend WithEvents tbpgCustom As TabPage
    Friend WithEvents ElementHost1 As Integration.ElementHost
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label10 As Label
    Friend WithEvents trkCameraTrack As TrackBar
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents lblFOVValue As Label
    Friend WithEvents trkViewFOV As TrackBar
    Friend WithEvents lblDistanceValue As Label
    Friend WithEvents trkViewDistance As TrackBar
    Friend WithEvents lblTiltValue As Label
    Friend WithEvents trkViewTilt As TrackBar
    Friend WithEvents lblHeightValue As Label
    Friend WithEvents trkViewHeight As TrackBar
    Friend WithEvents lblAngleValue As Label
    Friend WithEvents trkViewRotation As TrackBar
    Friend WindLayers3DControl1 As WindLayers3DDisplay.WindLayers3DDisplay.WindLayers3DControl
    Friend WithEvents trkWindDirectionSingle As TrackBar
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents lblWindDirectionSingleLabel As Label
    Friend WithEvents lblWindDirectionSingle As Label
    Friend WithEvents lblWindDirectionMultiLabel As Label
    Friend WithEvents lblWindDirectionMulti As Label
    Friend WithEvents trkWindDirectionMulti As TrackBar
    Friend WithEvents btnWindDirN As Button
    Friend WithEvents btnWindDirS As Button
    Friend WithEvents btnWindDirW As Button
    Friend WithEvents btnWindDirE As Button
    Friend WithEvents btnWindDirNW As Button
    Friend WithEvents btnWindDirSW As Button
    Friend WithEvents btnWindDirSE As Button
    Friend WithEvents btnWindDirNE As Button
    Friend WithEvents btnActionSave As Button
    Friend WithEvents btnActionNew As Button
    Friend WithEvents btnActionDelete As Button
    Friend WithEvents btnActionImport As Button
    Friend WithEvents lblHumidityIndexLabel As Label
    Friend WithEvents lblHumidityIndex As Label
    Friend WithEvents trkHumidityIndex As TrackBar
    Friend WithEvents lblBarometricPressureLabel As Label
    Friend WithEvents lblBarometricPressure As Label
    Friend WithEvents trkBaroPressure As TrackBar
    Friend WithEvents lblTemperatureLabel As Label
    Friend WithEvents lblTemperature As Label
    Friend WithEvents trkTemperature As TrackBar
    Friend WithEvents lblPrecipitationsLabel As Label
    Friend WithEvents lblPrecipitations As Label
    Friend WithEvents trkPrecipitations As TrackBar
    Friend WithEvents lblSnowCoverLabel As Label
    Friend WithEvents lblSnowCover As Label
    Friend WithEvents trkSnowCover As TrackBar
    Friend WithEvents lblThunderstormIntensityLabel As Label
    Friend WithEvents lblThunderstormIntensity As Label
    Friend WithEvents trkThunderstormIntensity As TrackBar
    Friend WithEvents chkPrimaryDynamic As CheckBox
    Friend WithEvents chkPrimaryWave As CheckBox
    Friend WithEvents chkPrimaryThermal As CheckBox
    Friend WithEvents chkPrimaryRidge As CheckBox
    Friend WithEvents Label18 As Label
End Class
