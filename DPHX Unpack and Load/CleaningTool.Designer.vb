<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CleaningTool
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.tabCtrlCleaningTool = New System.Windows.Forms.TabControl()
        Me.tabFlights = New System.Windows.Forms.TabPage()
        Me.btnFlightsSelectAll = New System.Windows.Forms.Button()
        Me.btnFlightsDelete = New System.Windows.Forms.Button()
        Me.lstFlights = New System.Windows.Forms.ListBox()
        Me.lblFlightsFolderPath = New System.Windows.Forms.Label()
        Me.btnFlightsRefresh = New System.Windows.Forms.Button()
        Me.tabWeather = New System.Windows.Forms.TabPage()
        Me.btnWeatherSelectAll = New System.Windows.Forms.Button()
        Me.btnWeatherDelete = New System.Windows.Forms.Button()
        Me.lstWeather = New System.Windows.Forms.ListBox()
        Me.lblWeatherFolderPath = New System.Windows.Forms.Label()
        Me.btnWeatherRefresh = New System.Windows.Forms.Button()
        Me.tabPackages = New System.Windows.Forms.TabPage()
        Me.btnPackagesSelectAll = New System.Windows.Forms.Button()
        Me.btnPackagesDelete = New System.Windows.Forms.Button()
        Me.lstPackages = New System.Windows.Forms.ListBox()
        Me.lblPackagesFolderPath = New System.Windows.Forms.Label()
        Me.btnPackagesRefresh = New System.Windows.Forms.Button()
        Me.tabNB21Logs = New System.Windows.Forms.TabPage()
        Me.btnNB21LogsSelectAll = New System.Windows.Forms.Button()
        Me.btnNB21LogsDelete = New System.Windows.Forms.Button()
        Me.lstNB21Logs = New System.Windows.Forms.ListBox()
        Me.lblNB21LogsFolderPath = New System.Windows.Forms.Label()
        Me.btnNB21LogsRefresh = New System.Windows.Forms.Button()
        Me.tabXCSoarTasks = New System.Windows.Forms.TabPage()
        Me.btnXCSoarTasksSelectAll = New System.Windows.Forms.Button()
        Me.btnXCSoarTasksDelete = New System.Windows.Forms.Button()
        Me.lstXCSoarTasks = New System.Windows.Forms.ListBox()
        Me.lblXCSoarTasksFolderPath = New System.Windows.Forms.Label()
        Me.btnXCSoarTasksRefresh = New System.Windows.Forms.Button()
        Me.tabXCSoarMaps = New System.Windows.Forms.TabPage()
        Me.btnXCSoarMapsSelectAll = New System.Windows.Forms.Button()
        Me.btnXCSoarMapsDelete = New System.Windows.Forms.Button()
        Me.lstXCSoarMaps = New System.Windows.Forms.ListBox()
        Me.lblXCSoarMapsFolderPath = New System.Windows.Forms.Label()
        Me.btnXCSoarMapsRefresh = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tabCtrlCleaningTool.SuspendLayout()
        Me.tabFlights.SuspendLayout()
        Me.tabWeather.SuspendLayout()
        Me.tabPackages.SuspendLayout()
        Me.tabNB21Logs.SuspendLayout()
        Me.tabXCSoarTasks.SuspendLayout()
        Me.tabXCSoarMaps.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabCtrlCleaningTool
        '
        Me.tabCtrlCleaningTool.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabFlights)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabWeather)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabPackages)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabNB21Logs)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabXCSoarTasks)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabXCSoarMaps)
        Me.tabCtrlCleaningTool.Location = New System.Drawing.Point(12, 12)
        Me.tabCtrlCleaningTool.Name = "tabCtrlCleaningTool"
        Me.tabCtrlCleaningTool.SelectedIndex = 0
        Me.tabCtrlCleaningTool.Size = New System.Drawing.Size(685, 518)
        Me.tabCtrlCleaningTool.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.tabCtrlCleaningTool, "Select the category of files to browse")
        '
        'tabFlights
        '
        Me.tabFlights.Controls.Add(Me.btnFlightsSelectAll)
        Me.tabFlights.Controls.Add(Me.btnFlightsDelete)
        Me.tabFlights.Controls.Add(Me.lstFlights)
        Me.tabFlights.Controls.Add(Me.lblFlightsFolderPath)
        Me.tabFlights.Controls.Add(Me.btnFlightsRefresh)
        Me.tabFlights.Location = New System.Drawing.Point(4, 29)
        Me.tabFlights.Name = "tabFlights"
        Me.tabFlights.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFlights.Size = New System.Drawing.Size(677, 485)
        Me.tabFlights.TabIndex = 0
        Me.tabFlights.Text = "Flight plans"
        Me.tabFlights.UseVisualStyleBackColor = True
        '
        'btnFlightsSelectAll
        '
        Me.btnFlightsSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlightsSelectAll.Location = New System.Drawing.Point(561, 77)
        Me.btnFlightsSelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlightsSelectAll.Name = "btnFlightsSelectAll"
        Me.btnFlightsSelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnFlightsSelectAll.TabIndex = 3
        Me.btnFlightsSelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnFlightsSelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnFlightsDelete
        '
        Me.btnFlightsDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlightsDelete.Location = New System.Drawing.Point(561, 122)
        Me.btnFlightsDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlightsDelete.Name = "btnFlightsDelete"
        Me.btnFlightsDelete.Size = New System.Drawing.Size(109, 35)
        Me.btnFlightsDelete.TabIndex = 4
        Me.btnFlightsDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnFlightsDelete, "Click to delete all selected files.")
        '
        'lstFlights
        '
        Me.lstFlights.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstFlights.FormattingEnabled = True
        Me.lstFlights.ItemHeight = 20
        Me.lstFlights.Location = New System.Drawing.Point(6, 33)
        Me.lstFlights.Name = "lstFlights"
        Me.lstFlights.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstFlights.Size = New System.Drawing.Size(548, 444)
        Me.lstFlights.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstFlights, "List of flight plan files currently in your folder.")
        '
        'lblFlightsFolderPath
        '
        Me.lblFlightsFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFlightsFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblFlightsFolderPath.Name = "lblFlightsFolderPath"
        Me.lblFlightsFolderPath.Size = New System.Drawing.Size(665, 23)
        Me.lblFlightsFolderPath.TabIndex = 0
        Me.lblFlightsFolderPath.Text = "The flights folder path"
        Me.ToolTip1.SetToolTip(Me.lblFlightsFolderPath, "This is the folder that you've set for your flight files (.pln).")
        '
        'btnFlightsRefresh
        '
        Me.btnFlightsRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlightsRefresh.Location = New System.Drawing.Point(561, 32)
        Me.btnFlightsRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlightsRefresh.Name = "btnFlightsRefresh"
        Me.btnFlightsRefresh.Size = New System.Drawing.Size(109, 35)
        Me.btnFlightsRefresh.TabIndex = 2
        Me.btnFlightsRefresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnFlightsRefresh, "Click to refresh the list.")
        '
        'tabWeather
        '
        Me.tabWeather.Controls.Add(Me.btnWeatherSelectAll)
        Me.tabWeather.Controls.Add(Me.btnWeatherDelete)
        Me.tabWeather.Controls.Add(Me.lstWeather)
        Me.tabWeather.Controls.Add(Me.lblWeatherFolderPath)
        Me.tabWeather.Controls.Add(Me.btnWeatherRefresh)
        Me.tabWeather.Location = New System.Drawing.Point(4, 29)
        Me.tabWeather.Name = "tabWeather"
        Me.tabWeather.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWeather.Size = New System.Drawing.Size(677, 485)
        Me.tabWeather.TabIndex = 1
        Me.tabWeather.Text = "Weather"
        Me.tabWeather.UseVisualStyleBackColor = True
        '
        'btnWeatherSelectAll
        '
        Me.btnWeatherSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeatherSelectAll.Location = New System.Drawing.Point(561, 77)
        Me.btnWeatherSelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeatherSelectAll.Name = "btnWeatherSelectAll"
        Me.btnWeatherSelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnWeatherSelectAll.TabIndex = 3
        Me.btnWeatherSelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnWeatherSelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnWeatherDelete
        '
        Me.btnWeatherDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeatherDelete.Location = New System.Drawing.Point(561, 122)
        Me.btnWeatherDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeatherDelete.Name = "btnWeatherDelete"
        Me.btnWeatherDelete.Size = New System.Drawing.Size(109, 35)
        Me.btnWeatherDelete.TabIndex = 4
        Me.btnWeatherDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnWeatherDelete, "Click to delete all selected files.")
        '
        'lstWeather
        '
        Me.lstWeather.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstWeather.FormattingEnabled = True
        Me.lstWeather.ItemHeight = 20
        Me.lstWeather.Location = New System.Drawing.Point(6, 33)
        Me.lstWeather.Name = "lstWeather"
        Me.lstWeather.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstWeather.Size = New System.Drawing.Size(548, 444)
        Me.lstWeather.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstWeather, "List of weather files currently in your folder.")
        '
        'lblWeatherFolderPath
        '
        Me.lblWeatherFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeatherFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblWeatherFolderPath.Name = "lblWeatherFolderPath"
        Me.lblWeatherFolderPath.Size = New System.Drawing.Size(665, 23)
        Me.lblWeatherFolderPath.TabIndex = 0
        Me.lblWeatherFolderPath.Text = "The weather files folder path"
        Me.ToolTip1.SetToolTip(Me.lblWeatherFolderPath, "This is the folder that you've set for your weather files (.wpr).")
        '
        'btnWeatherRefresh
        '
        Me.btnWeatherRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeatherRefresh.Location = New System.Drawing.Point(561, 32)
        Me.btnWeatherRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeatherRefresh.Name = "btnWeatherRefresh"
        Me.btnWeatherRefresh.Size = New System.Drawing.Size(109, 35)
        Me.btnWeatherRefresh.TabIndex = 2
        Me.btnWeatherRefresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnWeatherRefresh, "Click to refresh the list.")
        '
        'tabPackages
        '
        Me.tabPackages.Controls.Add(Me.btnPackagesSelectAll)
        Me.tabPackages.Controls.Add(Me.btnPackagesDelete)
        Me.tabPackages.Controls.Add(Me.lstPackages)
        Me.tabPackages.Controls.Add(Me.lblPackagesFolderPath)
        Me.tabPackages.Controls.Add(Me.btnPackagesRefresh)
        Me.tabPackages.Location = New System.Drawing.Point(4, 29)
        Me.tabPackages.Name = "tabPackages"
        Me.tabPackages.Size = New System.Drawing.Size(677, 485)
        Me.tabPackages.TabIndex = 4
        Me.tabPackages.Text = "Packages"
        Me.tabPackages.UseVisualStyleBackColor = True
        '
        'btnPackagesSelectAll
        '
        Me.btnPackagesSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesSelectAll.Location = New System.Drawing.Point(561, 77)
        Me.btnPackagesSelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnPackagesSelectAll.Name = "btnPackagesSelectAll"
        Me.btnPackagesSelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnPackagesSelectAll.TabIndex = 3
        Me.btnPackagesSelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnPackagesSelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnPackagesDelete
        '
        Me.btnPackagesDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesDelete.Location = New System.Drawing.Point(561, 122)
        Me.btnPackagesDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnPackagesDelete.Name = "btnPackagesDelete"
        Me.btnPackagesDelete.Size = New System.Drawing.Size(109, 35)
        Me.btnPackagesDelete.TabIndex = 4
        Me.btnPackagesDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnPackagesDelete, "Click to delete all selected files.")
        '
        'lstPackages
        '
        Me.lstPackages.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstPackages.FormattingEnabled = True
        Me.lstPackages.ItemHeight = 20
        Me.lstPackages.Location = New System.Drawing.Point(6, 33)
        Me.lstPackages.Name = "lstPackages"
        Me.lstPackages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstPackages.Size = New System.Drawing.Size(548, 444)
        Me.lstPackages.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstPackages, "List of package files currently in your folder.")
        '
        'lblPackagesFolderPath
        '
        Me.lblPackagesFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPackagesFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblPackagesFolderPath.Name = "lblPackagesFolderPath"
        Me.lblPackagesFolderPath.Size = New System.Drawing.Size(665, 23)
        Me.lblPackagesFolderPath.TabIndex = 0
        Me.lblPackagesFolderPath.Text = "The DPHX packages folder path"
        Me.ToolTip1.SetToolTip(Me.lblPackagesFolderPath, "This is the folder that you've set for your package files (.dphx).")
        '
        'btnPackagesRefresh
        '
        Me.btnPackagesRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesRefresh.Location = New System.Drawing.Point(561, 32)
        Me.btnPackagesRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnPackagesRefresh.Name = "btnPackagesRefresh"
        Me.btnPackagesRefresh.Size = New System.Drawing.Size(109, 35)
        Me.btnPackagesRefresh.TabIndex = 2
        Me.btnPackagesRefresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnPackagesRefresh, "Click to refresh the list.")
        '
        'tabNB21Logs
        '
        Me.tabNB21Logs.Controls.Add(Me.btnNB21LogsSelectAll)
        Me.tabNB21Logs.Controls.Add(Me.btnNB21LogsDelete)
        Me.tabNB21Logs.Controls.Add(Me.lstNB21Logs)
        Me.tabNB21Logs.Controls.Add(Me.lblNB21LogsFolderPath)
        Me.tabNB21Logs.Controls.Add(Me.btnNB21LogsRefresh)
        Me.tabNB21Logs.Location = New System.Drawing.Point(4, 29)
        Me.tabNB21Logs.Name = "tabNB21Logs"
        Me.tabNB21Logs.Size = New System.Drawing.Size(677, 485)
        Me.tabNB21Logs.TabIndex = 5
        Me.tabNB21Logs.Text = "NB21 Logs"
        Me.tabNB21Logs.UseVisualStyleBackColor = True
        '
        'btnNB21LogsSelectAll
        '
        Me.btnNB21LogsSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNB21LogsSelectAll.Location = New System.Drawing.Point(561, 77)
        Me.btnNB21LogsSelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnNB21LogsSelectAll.Name = "btnNB21LogsSelectAll"
        Me.btnNB21LogsSelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnNB21LogsSelectAll.TabIndex = 3
        Me.btnNB21LogsSelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnNB21LogsSelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnNB21LogsDelete
        '
        Me.btnNB21LogsDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNB21LogsDelete.Location = New System.Drawing.Point(561, 122)
        Me.btnNB21LogsDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnNB21LogsDelete.Name = "btnNB21LogsDelete"
        Me.btnNB21LogsDelete.Size = New System.Drawing.Size(109, 35)
        Me.btnNB21LogsDelete.TabIndex = 4
        Me.btnNB21LogsDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnNB21LogsDelete, "Click to delete all selected files.")
        '
        'lstNB21Logs
        '
        Me.lstNB21Logs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstNB21Logs.FormattingEnabled = True
        Me.lstNB21Logs.ItemHeight = 20
        Me.lstNB21Logs.Location = New System.Drawing.Point(6, 33)
        Me.lstNB21Logs.Name = "lstNB21Logs"
        Me.lstNB21Logs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstNB21Logs.Size = New System.Drawing.Size(548, 444)
        Me.lstNB21Logs.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstNB21Logs, "List of log files currently in your folder.")
        '
        'lblNB21LogsFolderPath
        '
        Me.lblNB21LogsFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblNB21LogsFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblNB21LogsFolderPath.Name = "lblNB21LogsFolderPath"
        Me.lblNB21LogsFolderPath.Size = New System.Drawing.Size(665, 23)
        Me.lblNB21LogsFolderPath.TabIndex = 0
        Me.lblNB21LogsFolderPath.Text = "The NB21 Logs folder path"
        Me.ToolTip1.SetToolTip(Me.lblNB21LogsFolderPath, "This is the folder that you've set for your NB21 log files (.igc).")
        '
        'btnNB21LogsRefresh
        '
        Me.btnNB21LogsRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNB21LogsRefresh.Location = New System.Drawing.Point(561, 32)
        Me.btnNB21LogsRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnNB21LogsRefresh.Name = "btnNB21LogsRefresh"
        Me.btnNB21LogsRefresh.Size = New System.Drawing.Size(109, 35)
        Me.btnNB21LogsRefresh.TabIndex = 2
        Me.btnNB21LogsRefresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnNB21LogsRefresh, "Click to refresh the list.")
        '
        'tabXCSoarTasks
        '
        Me.tabXCSoarTasks.Controls.Add(Me.btnXCSoarTasksSelectAll)
        Me.tabXCSoarTasks.Controls.Add(Me.btnXCSoarTasksDelete)
        Me.tabXCSoarTasks.Controls.Add(Me.lstXCSoarTasks)
        Me.tabXCSoarTasks.Controls.Add(Me.lblXCSoarTasksFolderPath)
        Me.tabXCSoarTasks.Controls.Add(Me.btnXCSoarTasksRefresh)
        Me.tabXCSoarTasks.Location = New System.Drawing.Point(4, 29)
        Me.tabXCSoarTasks.Name = "tabXCSoarTasks"
        Me.tabXCSoarTasks.Size = New System.Drawing.Size(677, 485)
        Me.tabXCSoarTasks.TabIndex = 2
        Me.tabXCSoarTasks.Text = "XC Soar Tasks"
        Me.tabXCSoarTasks.UseVisualStyleBackColor = True
        '
        'btnXCSoarTasksSelectAll
        '
        Me.btnXCSoarTasksSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarTasksSelectAll.Location = New System.Drawing.Point(561, 77)
        Me.btnXCSoarTasksSelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnXCSoarTasksSelectAll.Name = "btnXCSoarTasksSelectAll"
        Me.btnXCSoarTasksSelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnXCSoarTasksSelectAll.TabIndex = 3
        Me.btnXCSoarTasksSelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnXCSoarTasksSelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnXCSoarTasksDelete
        '
        Me.btnXCSoarTasksDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarTasksDelete.Location = New System.Drawing.Point(561, 122)
        Me.btnXCSoarTasksDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnXCSoarTasksDelete.Name = "btnXCSoarTasksDelete"
        Me.btnXCSoarTasksDelete.Size = New System.Drawing.Size(109, 35)
        Me.btnXCSoarTasksDelete.TabIndex = 4
        Me.btnXCSoarTasksDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnXCSoarTasksDelete, "Click to delete all selected files.")
        '
        'lstXCSoarTasks
        '
        Me.lstXCSoarTasks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstXCSoarTasks.FormattingEnabled = True
        Me.lstXCSoarTasks.ItemHeight = 20
        Me.lstXCSoarTasks.Location = New System.Drawing.Point(6, 33)
        Me.lstXCSoarTasks.Name = "lstXCSoarTasks"
        Me.lstXCSoarTasks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstXCSoarTasks.Size = New System.Drawing.Size(548, 444)
        Me.lstXCSoarTasks.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstXCSoarTasks, "List of XC Soar task files currently in your folder.")
        '
        'lblXCSoarTasksFolderPath
        '
        Me.lblXCSoarTasksFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblXCSoarTasksFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblXCSoarTasksFolderPath.Name = "lblXCSoarTasksFolderPath"
        Me.lblXCSoarTasksFolderPath.Size = New System.Drawing.Size(665, 23)
        Me.lblXCSoarTasksFolderPath.TabIndex = 0
        Me.lblXCSoarTasksFolderPath.Text = "The XCSoar Tasks folder path"
        Me.ToolTip1.SetToolTip(Me.lblXCSoarTasksFolderPath, "This is the folder that you've set for your XC Soar Task files (.tsk).")
        '
        'btnXCSoarTasksRefresh
        '
        Me.btnXCSoarTasksRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarTasksRefresh.Location = New System.Drawing.Point(561, 32)
        Me.btnXCSoarTasksRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnXCSoarTasksRefresh.Name = "btnXCSoarTasksRefresh"
        Me.btnXCSoarTasksRefresh.Size = New System.Drawing.Size(109, 35)
        Me.btnXCSoarTasksRefresh.TabIndex = 2
        Me.btnXCSoarTasksRefresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnXCSoarTasksRefresh, "Click to refresh the list.")
        '
        'tabXCSoarMaps
        '
        Me.tabXCSoarMaps.Controls.Add(Me.btnXCSoarMapsSelectAll)
        Me.tabXCSoarMaps.Controls.Add(Me.btnXCSoarMapsDelete)
        Me.tabXCSoarMaps.Controls.Add(Me.lstXCSoarMaps)
        Me.tabXCSoarMaps.Controls.Add(Me.lblXCSoarMapsFolderPath)
        Me.tabXCSoarMaps.Controls.Add(Me.btnXCSoarMapsRefresh)
        Me.tabXCSoarMaps.Location = New System.Drawing.Point(4, 29)
        Me.tabXCSoarMaps.Name = "tabXCSoarMaps"
        Me.tabXCSoarMaps.Size = New System.Drawing.Size(677, 485)
        Me.tabXCSoarMaps.TabIndex = 3
        Me.tabXCSoarMaps.Text = "XC Soar Maps"
        Me.tabXCSoarMaps.UseVisualStyleBackColor = True
        '
        'btnXCSoarMapsSelectAll
        '
        Me.btnXCSoarMapsSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarMapsSelectAll.Location = New System.Drawing.Point(561, 77)
        Me.btnXCSoarMapsSelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnXCSoarMapsSelectAll.Name = "btnXCSoarMapsSelectAll"
        Me.btnXCSoarMapsSelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnXCSoarMapsSelectAll.TabIndex = 3
        Me.btnXCSoarMapsSelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnXCSoarMapsSelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnXCSoarMapsDelete
        '
        Me.btnXCSoarMapsDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarMapsDelete.Location = New System.Drawing.Point(561, 122)
        Me.btnXCSoarMapsDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnXCSoarMapsDelete.Name = "btnXCSoarMapsDelete"
        Me.btnXCSoarMapsDelete.Size = New System.Drawing.Size(109, 35)
        Me.btnXCSoarMapsDelete.TabIndex = 4
        Me.btnXCSoarMapsDelete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnXCSoarMapsDelete, "Click to delete all selected files.")
        '
        'lstXCSoarMaps
        '
        Me.lstXCSoarMaps.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstXCSoarMaps.FormattingEnabled = True
        Me.lstXCSoarMaps.ItemHeight = 20
        Me.lstXCSoarMaps.Location = New System.Drawing.Point(6, 33)
        Me.lstXCSoarMaps.Name = "lstXCSoarMaps"
        Me.lstXCSoarMaps.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstXCSoarMaps.Size = New System.Drawing.Size(548, 444)
        Me.lstXCSoarMaps.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstXCSoarMaps, "List of XC Soar map files currently in your folder.")
        '
        'lblXCSoarMapsFolderPath
        '
        Me.lblXCSoarMapsFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblXCSoarMapsFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblXCSoarMapsFolderPath.Name = "lblXCSoarMapsFolderPath"
        Me.lblXCSoarMapsFolderPath.Size = New System.Drawing.Size(665, 23)
        Me.lblXCSoarMapsFolderPath.TabIndex = 0
        Me.lblXCSoarMapsFolderPath.Text = "The XCSoar Maps folder path"
        Me.ToolTip1.SetToolTip(Me.lblXCSoarMapsFolderPath, "This is the folder that you've set for your XC Soar Map files (.xcm).")
        '
        'btnXCSoarMapsRefresh
        '
        Me.btnXCSoarMapsRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarMapsRefresh.Location = New System.Drawing.Point(561, 32)
        Me.btnXCSoarMapsRefresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnXCSoarMapsRefresh.Name = "btnXCSoarMapsRefresh"
        Me.btnXCSoarMapsRefresh.Size = New System.Drawing.Size(109, 35)
        Me.btnXCSoarMapsRefresh.TabIndex = 2
        Me.btnXCSoarMapsRefresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnXCSoarMapsRefresh, "Click to refresh the list.")
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.OK_Button)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 560)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(709, 44)
        Me.Panel1.TabIndex = 1
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Location = New System.Drawing.Point(596, 4)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(109, 35)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        Me.ToolTip1.SetToolTip(Me.OK_Button, "Click to close window.")
        '
        'CleaningTool
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.OK_Button
        Me.ClientSize = New System.Drawing.Size(709, 604)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.tabCtrlCleaningTool)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(727, 622)
        Me.Name = "CleaningTool"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "CleaningTool"
        Me.tabCtrlCleaningTool.ResumeLayout(False)
        Me.tabFlights.ResumeLayout(False)
        Me.tabWeather.ResumeLayout(False)
        Me.tabPackages.ResumeLayout(False)
        Me.tabNB21Logs.ResumeLayout(False)
        Me.tabXCSoarTasks.ResumeLayout(False)
        Me.tabXCSoarMaps.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabCtrlCleaningTool As TabControl
    Friend WithEvents tabFlights As TabPage
    Friend WithEvents tabWeather As TabPage
    Friend WithEvents OK_Button As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents tabPackages As TabPage
    Friend WithEvents tabNB21Logs As TabPage
    Friend WithEvents tabXCSoarTasks As TabPage
    Friend WithEvents tabXCSoarMaps As TabPage
    Friend WithEvents btnFlightsRefresh As Button
    Friend WithEvents lstFlights As ListBox
    Friend WithEvents lblFlightsFolderPath As Label
    Friend WithEvents btnFlightsDelete As Button
    Friend WithEvents btnFlightsSelectAll As Button
    Friend WithEvents btnWeatherSelectAll As Button
    Friend WithEvents btnWeatherDelete As Button
    Friend WithEvents lstWeather As ListBox
    Friend WithEvents lblWeatherFolderPath As Label
    Friend WithEvents btnWeatherRefresh As Button
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents btnPackagesSelectAll As Button
    Friend WithEvents btnPackagesDelete As Button
    Friend WithEvents lstPackages As ListBox
    Friend WithEvents lblPackagesFolderPath As Label
    Friend WithEvents btnPackagesRefresh As Button
    Friend WithEvents btnNB21LogsSelectAll As Button
    Friend WithEvents btnNB21LogsDelete As Button
    Friend WithEvents lstNB21Logs As ListBox
    Friend WithEvents lblNB21LogsFolderPath As Label
    Friend WithEvents btnNB21LogsRefresh As Button
    Friend WithEvents btnXCSoarTasksSelectAll As Button
    Friend WithEvents btnXCSoarTasksDelete As Button
    Friend WithEvents lstXCSoarTasks As ListBox
    Friend WithEvents lblXCSoarTasksFolderPath As Label
    Friend WithEvents btnXCSoarTasksRefresh As Button
    Friend WithEvents btnXCSoarMapsSelectAll As Button
    Friend WithEvents btnXCSoarMapsDelete As Button
    Friend WithEvents lstXCSoarMaps As ListBox
    Friend WithEvents lblXCSoarMapsFolderPath As Label
    Friend WithEvents btnXCSoarMapsRefresh As Button
    Friend WithEvents ToolTip1 As ToolTip
End Class
