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
        Me.tabFlights2020 = New System.Windows.Forms.TabPage()
        Me.btnFlights2020SelectAll = New System.Windows.Forms.Button()
        Me.btnFlights2020Delete = New System.Windows.Forms.Button()
        Me.lstFlights2020 = New System.Windows.Forms.ListBox()
        Me.lblFlights2020FolderPath = New System.Windows.Forms.Label()
        Me.btnFlights2020Refresh = New System.Windows.Forms.Button()
        Me.tabWeather2020 = New System.Windows.Forms.TabPage()
        Me.btnWeather2020SelectAll = New System.Windows.Forms.Button()
        Me.btnWeather2020Delete = New System.Windows.Forms.Button()
        Me.lstWeather2020 = New System.Windows.Forms.ListBox()
        Me.lblWeather2020FolderPath = New System.Windows.Forms.Label()
        Me.btnWeather2020Refresh = New System.Windows.Forms.Button()
        Me.tabFlights2024 = New System.Windows.Forms.TabPage()
        Me.btnFlights2024SelectAll = New System.Windows.Forms.Button()
        Me.btnFlights2024Delete = New System.Windows.Forms.Button()
        Me.lstFlights2024 = New System.Windows.Forms.ListBox()
        Me.lblFlights2024FolderPath = New System.Windows.Forms.Label()
        Me.btnFlights2024Refresh = New System.Windows.Forms.Button()
        Me.tabWeather2024 = New System.Windows.Forms.TabPage()
        Me.btnWeather2024SelectAll = New System.Windows.Forms.Button()
        Me.btnWeather2024Delete = New System.Windows.Forms.Button()
        Me.lstWeather2024 = New System.Windows.Forms.ListBox()
        Me.lblWeather2024FolderPath = New System.Windows.Forms.Label()
        Me.btnWeather2024Refresh = New System.Windows.Forms.Button()
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
        Me.tabFlights2020.SuspendLayout()
        Me.tabWeather2020.SuspendLayout()
        Me.tabFlights2024.SuspendLayout()
        Me.tabWeather2024.SuspendLayout()
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
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabFlights2020)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabWeather2020)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabFlights2024)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabWeather2024)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabPackages)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabNB21Logs)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabXCSoarTasks)
        Me.tabCtrlCleaningTool.Controls.Add(Me.tabXCSoarMaps)
        Me.tabCtrlCleaningTool.Location = New System.Drawing.Point(12, 12)
        Me.tabCtrlCleaningTool.Name = "tabCtrlCleaningTool"
        Me.tabCtrlCleaningTool.SelectedIndex = 0
        Me.tabCtrlCleaningTool.Size = New System.Drawing.Size(853, 492)
        Me.tabCtrlCleaningTool.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.tabCtrlCleaningTool, "Select the category of files to browse")
        '
        'tabFlights2020
        '
        Me.tabFlights2020.Controls.Add(Me.btnFlights2020SelectAll)
        Me.tabFlights2020.Controls.Add(Me.btnFlights2020Delete)
        Me.tabFlights2020.Controls.Add(Me.lstFlights2020)
        Me.tabFlights2020.Controls.Add(Me.lblFlights2020FolderPath)
        Me.tabFlights2020.Controls.Add(Me.btnFlights2020Refresh)
        Me.tabFlights2020.Location = New System.Drawing.Point(4, 29)
        Me.tabFlights2020.Name = "tabFlights2020"
        Me.tabFlights2020.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFlights2020.Size = New System.Drawing.Size(845, 459)
        Me.tabFlights2020.TabIndex = 0
        Me.tabFlights2020.Text = "Flight plans 2020"
        Me.tabFlights2020.UseVisualStyleBackColor = True
        '
        'btnFlights2020SelectAll
        '
        Me.btnFlights2020SelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlights2020SelectAll.Location = New System.Drawing.Point(729, 77)
        Me.btnFlights2020SelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlights2020SelectAll.Name = "btnFlights2020SelectAll"
        Me.btnFlights2020SelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnFlights2020SelectAll.TabIndex = 3
        Me.btnFlights2020SelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnFlights2020SelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnFlights2020Delete
        '
        Me.btnFlights2020Delete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlights2020Delete.Location = New System.Drawing.Point(729, 122)
        Me.btnFlights2020Delete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlights2020Delete.Name = "btnFlights2020Delete"
        Me.btnFlights2020Delete.Size = New System.Drawing.Size(109, 35)
        Me.btnFlights2020Delete.TabIndex = 4
        Me.btnFlights2020Delete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnFlights2020Delete, "Click to delete all selected files.")
        '
        'lstFlights2020
        '
        Me.lstFlights2020.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstFlights2020.FormattingEnabled = True
        Me.lstFlights2020.ItemHeight = 20
        Me.lstFlights2020.Location = New System.Drawing.Point(6, 33)
        Me.lstFlights2020.Name = "lstFlights2020"
        Me.lstFlights2020.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstFlights2020.Size = New System.Drawing.Size(716, 404)
        Me.lstFlights2020.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstFlights2020, "List of flight plan files currently in your folder.")
        '
        'lblFlights2020FolderPath
        '
        Me.lblFlights2020FolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFlights2020FolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblFlights2020FolderPath.Name = "lblFlights2020FolderPath"
        Me.lblFlights2020FolderPath.Size = New System.Drawing.Size(833, 23)
        Me.lblFlights2020FolderPath.TabIndex = 0
        Me.lblFlights2020FolderPath.Text = "The flights folder path"
        Me.ToolTip1.SetToolTip(Me.lblFlights2020FolderPath, "This is the folder that you've set for your flight files (.pln).")
        '
        'btnFlights2020Refresh
        '
        Me.btnFlights2020Refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlights2020Refresh.Location = New System.Drawing.Point(729, 32)
        Me.btnFlights2020Refresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlights2020Refresh.Name = "btnFlights2020Refresh"
        Me.btnFlights2020Refresh.Size = New System.Drawing.Size(109, 35)
        Me.btnFlights2020Refresh.TabIndex = 2
        Me.btnFlights2020Refresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnFlights2020Refresh, "Click to refresh the list.")
        '
        'tabWeather2020
        '
        Me.tabWeather2020.Controls.Add(Me.btnWeather2020SelectAll)
        Me.tabWeather2020.Controls.Add(Me.btnWeather2020Delete)
        Me.tabWeather2020.Controls.Add(Me.lstWeather2020)
        Me.tabWeather2020.Controls.Add(Me.lblWeather2020FolderPath)
        Me.tabWeather2020.Controls.Add(Me.btnWeather2020Refresh)
        Me.tabWeather2020.Location = New System.Drawing.Point(4, 29)
        Me.tabWeather2020.Name = "tabWeather2020"
        Me.tabWeather2020.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWeather2020.Size = New System.Drawing.Size(845, 459)
        Me.tabWeather2020.TabIndex = 1
        Me.tabWeather2020.Text = "Weather 2020"
        Me.tabWeather2020.UseVisualStyleBackColor = True
        '
        'btnWeather2020SelectAll
        '
        Me.btnWeather2020SelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeather2020SelectAll.Location = New System.Drawing.Point(729, 77)
        Me.btnWeather2020SelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeather2020SelectAll.Name = "btnWeather2020SelectAll"
        Me.btnWeather2020SelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnWeather2020SelectAll.TabIndex = 3
        Me.btnWeather2020SelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnWeather2020SelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnWeather2020Delete
        '
        Me.btnWeather2020Delete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeather2020Delete.Location = New System.Drawing.Point(729, 122)
        Me.btnWeather2020Delete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeather2020Delete.Name = "btnWeather2020Delete"
        Me.btnWeather2020Delete.Size = New System.Drawing.Size(109, 35)
        Me.btnWeather2020Delete.TabIndex = 4
        Me.btnWeather2020Delete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnWeather2020Delete, "Click to delete all selected files.")
        '
        'lstWeather2020
        '
        Me.lstWeather2020.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstWeather2020.FormattingEnabled = True
        Me.lstWeather2020.ItemHeight = 20
        Me.lstWeather2020.Location = New System.Drawing.Point(6, 33)
        Me.lstWeather2020.Name = "lstWeather2020"
        Me.lstWeather2020.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstWeather2020.Size = New System.Drawing.Size(716, 404)
        Me.lstWeather2020.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstWeather2020, "List of weather files currently in your folder.")
        '
        'lblWeather2020FolderPath
        '
        Me.lblWeather2020FolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeather2020FolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblWeather2020FolderPath.Name = "lblWeather2020FolderPath"
        Me.lblWeather2020FolderPath.Size = New System.Drawing.Size(833, 23)
        Me.lblWeather2020FolderPath.TabIndex = 0
        Me.lblWeather2020FolderPath.Text = "The weather files folder path"
        Me.ToolTip1.SetToolTip(Me.lblWeather2020FolderPath, "This is the folder that you've set for your weather files (.wpr).")
        '
        'btnWeather2020Refresh
        '
        Me.btnWeather2020Refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeather2020Refresh.Location = New System.Drawing.Point(729, 32)
        Me.btnWeather2020Refresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeather2020Refresh.Name = "btnWeather2020Refresh"
        Me.btnWeather2020Refresh.Size = New System.Drawing.Size(109, 35)
        Me.btnWeather2020Refresh.TabIndex = 2
        Me.btnWeather2020Refresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnWeather2020Refresh, "Click to refresh the list.")
        '
        'tabFlights2024
        '
        Me.tabFlights2024.Controls.Add(Me.btnFlights2024SelectAll)
        Me.tabFlights2024.Controls.Add(Me.btnFlights2024Delete)
        Me.tabFlights2024.Controls.Add(Me.lstFlights2024)
        Me.tabFlights2024.Controls.Add(Me.lblFlights2024FolderPath)
        Me.tabFlights2024.Controls.Add(Me.btnFlights2024Refresh)
        Me.tabFlights2024.Location = New System.Drawing.Point(4, 29)
        Me.tabFlights2024.Name = "tabFlights2024"
        Me.tabFlights2024.Size = New System.Drawing.Size(845, 459)
        Me.tabFlights2024.TabIndex = 6
        Me.tabFlights2024.Text = "Flight plans 2024"
        Me.tabFlights2024.UseVisualStyleBackColor = True
        '
        'btnFlights2024SelectAll
        '
        Me.btnFlights2024SelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlights2024SelectAll.Location = New System.Drawing.Point(729, 77)
        Me.btnFlights2024SelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlights2024SelectAll.Name = "btnFlights2024SelectAll"
        Me.btnFlights2024SelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnFlights2024SelectAll.TabIndex = 8
        Me.btnFlights2024SelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnFlights2024SelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnFlights2024Delete
        '
        Me.btnFlights2024Delete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlights2024Delete.Location = New System.Drawing.Point(729, 122)
        Me.btnFlights2024Delete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlights2024Delete.Name = "btnFlights2024Delete"
        Me.btnFlights2024Delete.Size = New System.Drawing.Size(109, 35)
        Me.btnFlights2024Delete.TabIndex = 9
        Me.btnFlights2024Delete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnFlights2024Delete, "Click to delete all selected files.")
        '
        'lstFlights2024
        '
        Me.lstFlights2024.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstFlights2024.FormattingEnabled = True
        Me.lstFlights2024.ItemHeight = 20
        Me.lstFlights2024.Location = New System.Drawing.Point(6, 33)
        Me.lstFlights2024.Name = "lstFlights2024"
        Me.lstFlights2024.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstFlights2024.Size = New System.Drawing.Size(716, 404)
        Me.lstFlights2024.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.lstFlights2024, "List of flight plan files currently in your folder.")
        '
        'lblFlights2024FolderPath
        '
        Me.lblFlights2024FolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFlights2024FolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblFlights2024FolderPath.Name = "lblFlights2024FolderPath"
        Me.lblFlights2024FolderPath.Size = New System.Drawing.Size(833, 23)
        Me.lblFlights2024FolderPath.TabIndex = 5
        Me.lblFlights2024FolderPath.Text = "The flights folder path"
        Me.ToolTip1.SetToolTip(Me.lblFlights2024FolderPath, "This is the folder that you've set for your flight files (.pln).")
        '
        'btnFlights2024Refresh
        '
        Me.btnFlights2024Refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFlights2024Refresh.Location = New System.Drawing.Point(729, 32)
        Me.btnFlights2024Refresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnFlights2024Refresh.Name = "btnFlights2024Refresh"
        Me.btnFlights2024Refresh.Size = New System.Drawing.Size(109, 35)
        Me.btnFlights2024Refresh.TabIndex = 7
        Me.btnFlights2024Refresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnFlights2024Refresh, "Click to refresh the list.")
        '
        'tabWeather2024
        '
        Me.tabWeather2024.Controls.Add(Me.btnWeather2024SelectAll)
        Me.tabWeather2024.Controls.Add(Me.btnWeather2024Delete)
        Me.tabWeather2024.Controls.Add(Me.lstWeather2024)
        Me.tabWeather2024.Controls.Add(Me.lblWeather2024FolderPath)
        Me.tabWeather2024.Controls.Add(Me.btnWeather2024Refresh)
        Me.tabWeather2024.Location = New System.Drawing.Point(4, 29)
        Me.tabWeather2024.Name = "tabWeather2024"
        Me.tabWeather2024.Size = New System.Drawing.Size(845, 459)
        Me.tabWeather2024.TabIndex = 7
        Me.tabWeather2024.Text = "Weather 2024"
        Me.tabWeather2024.UseVisualStyleBackColor = True
        '
        'btnWeather2024SelectAll
        '
        Me.btnWeather2024SelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeather2024SelectAll.Location = New System.Drawing.Point(729, 77)
        Me.btnWeather2024SelectAll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeather2024SelectAll.Name = "btnWeather2024SelectAll"
        Me.btnWeather2024SelectAll.Size = New System.Drawing.Size(109, 35)
        Me.btnWeather2024SelectAll.TabIndex = 8
        Me.btnWeather2024SelectAll.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.btnWeather2024SelectAll, "Click to toggle selection of all items in the list.")
        '
        'btnWeather2024Delete
        '
        Me.btnWeather2024Delete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeather2024Delete.Location = New System.Drawing.Point(729, 122)
        Me.btnWeather2024Delete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeather2024Delete.Name = "btnWeather2024Delete"
        Me.btnWeather2024Delete.Size = New System.Drawing.Size(109, 35)
        Me.btnWeather2024Delete.TabIndex = 9
        Me.btnWeather2024Delete.Text = "Delete"
        Me.ToolTip1.SetToolTip(Me.btnWeather2024Delete, "Click to delete all selected files.")
        '
        'lstWeather2024
        '
        Me.lstWeather2024.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstWeather2024.FormattingEnabled = True
        Me.lstWeather2024.ItemHeight = 20
        Me.lstWeather2024.Location = New System.Drawing.Point(6, 33)
        Me.lstWeather2024.Name = "lstWeather2024"
        Me.lstWeather2024.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lstWeather2024.Size = New System.Drawing.Size(716, 404)
        Me.lstWeather2024.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.lstWeather2024, "List of weather files currently in your folder.")
        '
        'lblWeather2024FolderPath
        '
        Me.lblWeather2024FolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWeather2024FolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblWeather2024FolderPath.Name = "lblWeather2024FolderPath"
        Me.lblWeather2024FolderPath.Size = New System.Drawing.Size(833, 23)
        Me.lblWeather2024FolderPath.TabIndex = 5
        Me.lblWeather2024FolderPath.Text = "The weather files folder path"
        Me.ToolTip1.SetToolTip(Me.lblWeather2024FolderPath, "This is the folder that you've set for your weather files (.wpr).")
        '
        'btnWeather2024Refresh
        '
        Me.btnWeather2024Refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWeather2024Refresh.Location = New System.Drawing.Point(729, 32)
        Me.btnWeather2024Refresh.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnWeather2024Refresh.Name = "btnWeather2024Refresh"
        Me.btnWeather2024Refresh.Size = New System.Drawing.Size(109, 35)
        Me.btnWeather2024Refresh.TabIndex = 7
        Me.btnWeather2024Refresh.Text = "Refresh"
        Me.ToolTip1.SetToolTip(Me.btnWeather2024Refresh, "Click to refresh the list.")
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
        Me.tabPackages.Size = New System.Drawing.Size(845, 459)
        Me.tabPackages.TabIndex = 4
        Me.tabPackages.Text = "Packages"
        Me.tabPackages.UseVisualStyleBackColor = True
        '
        'btnPackagesSelectAll
        '
        Me.btnPackagesSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesSelectAll.Location = New System.Drawing.Point(742, 77)
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
        Me.btnPackagesDelete.Location = New System.Drawing.Point(742, 122)
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
        Me.lstPackages.Size = New System.Drawing.Size(729, 444)
        Me.lstPackages.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstPackages, "List of package files currently in your folder.")
        '
        'lblPackagesFolderPath
        '
        Me.lblPackagesFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPackagesFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblPackagesFolderPath.Name = "lblPackagesFolderPath"
        Me.lblPackagesFolderPath.Size = New System.Drawing.Size(846, 23)
        Me.lblPackagesFolderPath.TabIndex = 0
        Me.lblPackagesFolderPath.Text = "The DPHX packages folder path"
        Me.ToolTip1.SetToolTip(Me.lblPackagesFolderPath, "This is the folder that you've set for your package files (.dphx).")
        '
        'btnPackagesRefresh
        '
        Me.btnPackagesRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPackagesRefresh.Location = New System.Drawing.Point(742, 32)
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
        Me.tabNB21Logs.Size = New System.Drawing.Size(845, 459)
        Me.tabNB21Logs.TabIndex = 5
        Me.tabNB21Logs.Text = "NB21 Logs"
        Me.tabNB21Logs.UseVisualStyleBackColor = True
        '
        'btnNB21LogsSelectAll
        '
        Me.btnNB21LogsSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNB21LogsSelectAll.Location = New System.Drawing.Point(742, 77)
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
        Me.btnNB21LogsDelete.Location = New System.Drawing.Point(742, 122)
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
        Me.lstNB21Logs.Size = New System.Drawing.Size(729, 444)
        Me.lstNB21Logs.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstNB21Logs, "List of log files currently in your folder.")
        '
        'lblNB21LogsFolderPath
        '
        Me.lblNB21LogsFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblNB21LogsFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblNB21LogsFolderPath.Name = "lblNB21LogsFolderPath"
        Me.lblNB21LogsFolderPath.Size = New System.Drawing.Size(846, 23)
        Me.lblNB21LogsFolderPath.TabIndex = 0
        Me.lblNB21LogsFolderPath.Text = "The NB21 Logs folder path"
        Me.ToolTip1.SetToolTip(Me.lblNB21LogsFolderPath, "This is the folder that you've set for your NB21 log files (.igc).")
        '
        'btnNB21LogsRefresh
        '
        Me.btnNB21LogsRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNB21LogsRefresh.Location = New System.Drawing.Point(742, 32)
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
        Me.tabXCSoarTasks.Size = New System.Drawing.Size(845, 459)
        Me.tabXCSoarTasks.TabIndex = 2
        Me.tabXCSoarTasks.Text = "XC Soar Tasks"
        Me.tabXCSoarTasks.UseVisualStyleBackColor = True
        '
        'btnXCSoarTasksSelectAll
        '
        Me.btnXCSoarTasksSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarTasksSelectAll.Location = New System.Drawing.Point(742, 77)
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
        Me.btnXCSoarTasksDelete.Location = New System.Drawing.Point(742, 122)
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
        Me.lstXCSoarTasks.Size = New System.Drawing.Size(729, 444)
        Me.lstXCSoarTasks.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstXCSoarTasks, "List of XC Soar task files currently in your folder.")
        '
        'lblXCSoarTasksFolderPath
        '
        Me.lblXCSoarTasksFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblXCSoarTasksFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblXCSoarTasksFolderPath.Name = "lblXCSoarTasksFolderPath"
        Me.lblXCSoarTasksFolderPath.Size = New System.Drawing.Size(846, 23)
        Me.lblXCSoarTasksFolderPath.TabIndex = 0
        Me.lblXCSoarTasksFolderPath.Text = "The XCSoar Tasks folder path"
        Me.ToolTip1.SetToolTip(Me.lblXCSoarTasksFolderPath, "This is the folder that you've set for your XC Soar Task files (.tsk).")
        '
        'btnXCSoarTasksRefresh
        '
        Me.btnXCSoarTasksRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarTasksRefresh.Location = New System.Drawing.Point(742, 32)
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
        Me.tabXCSoarMaps.Size = New System.Drawing.Size(845, 459)
        Me.tabXCSoarMaps.TabIndex = 3
        Me.tabXCSoarMaps.Text = "XC Soar Maps"
        Me.tabXCSoarMaps.UseVisualStyleBackColor = True
        '
        'btnXCSoarMapsSelectAll
        '
        Me.btnXCSoarMapsSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarMapsSelectAll.Location = New System.Drawing.Point(742, 77)
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
        Me.btnXCSoarMapsDelete.Location = New System.Drawing.Point(742, 122)
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
        Me.lstXCSoarMaps.Size = New System.Drawing.Size(729, 444)
        Me.lstXCSoarMaps.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.lstXCSoarMaps, "List of XC Soar map files currently in your folder.")
        '
        'lblXCSoarMapsFolderPath
        '
        Me.lblXCSoarMapsFolderPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblXCSoarMapsFolderPath.Location = New System.Drawing.Point(6, 7)
        Me.lblXCSoarMapsFolderPath.Name = "lblXCSoarMapsFolderPath"
        Me.lblXCSoarMapsFolderPath.Size = New System.Drawing.Size(846, 23)
        Me.lblXCSoarMapsFolderPath.TabIndex = 0
        Me.lblXCSoarMapsFolderPath.Text = "The XCSoar Maps folder path"
        Me.ToolTip1.SetToolTip(Me.lblXCSoarMapsFolderPath, "This is the folder that you've set for your XC Soar Map files (.xcm).")
        '
        'btnXCSoarMapsRefresh
        '
        Me.btnXCSoarMapsRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnXCSoarMapsRefresh.Location = New System.Drawing.Point(742, 32)
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
        Me.Panel1.Size = New System.Drawing.Size(877, 44)
        Me.Panel1.TabIndex = 1
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OK_Button.Location = New System.Drawing.Point(764, 4)
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
        Me.ClientSize = New System.Drawing.Size(877, 604)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.tabCtrlCleaningTool)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(895, 622)
        Me.Name = "CleaningTool"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Cleaning Tool"
        Me.tabCtrlCleaningTool.ResumeLayout(False)
        Me.tabFlights2020.ResumeLayout(False)
        Me.tabWeather2020.ResumeLayout(False)
        Me.tabFlights2024.ResumeLayout(False)
        Me.tabWeather2024.ResumeLayout(False)
        Me.tabPackages.ResumeLayout(False)
        Me.tabNB21Logs.ResumeLayout(False)
        Me.tabXCSoarTasks.ResumeLayout(False)
        Me.tabXCSoarMaps.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabCtrlCleaningTool As TabControl
    Friend WithEvents tabFlights2020 As TabPage
    Friend WithEvents tabWeather2020 As TabPage
    Friend WithEvents OK_Button As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents tabPackages As TabPage
    Friend WithEvents tabNB21Logs As TabPage
    Friend WithEvents tabXCSoarTasks As TabPage
    Friend WithEvents tabXCSoarMaps As TabPage
    Friend WithEvents btnFlights2020Refresh As Button
    Friend WithEvents lstFlights2020 As ListBox
    Friend WithEvents lblFlights2020FolderPath As Label
    Friend WithEvents btnFlights2020Delete As Button
    Friend WithEvents btnFlights2020SelectAll As Button
    Friend WithEvents btnWeather2020SelectAll As Button
    Friend WithEvents btnWeather2020Delete As Button
    Friend WithEvents lstWeather2020 As ListBox
    Friend WithEvents lblWeather2020FolderPath As Label
    Friend WithEvents btnWeather2020Refresh As Button
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
    Friend WithEvents tabFlights2024 As TabPage
    Friend WithEvents tabWeather2024 As TabPage
    Friend WithEvents btnFlights2024SelectAll As Button
    Friend WithEvents btnFlights2024Delete As Button
    Friend WithEvents lstFlights2024 As ListBox
    Friend WithEvents lblFlights2024FolderPath As Label
    Friend WithEvents btnFlights2024Refresh As Button
    Friend WithEvents btnWeather2024SelectAll As Button
    Friend WithEvents btnWeather2024Delete As Button
    Friend WithEvents lstWeather2024 As ListBox
    Friend WithEvents lblWeather2024FolderPath As Label
    Friend WithEvents btnWeather2024Refresh As Button
End Class
