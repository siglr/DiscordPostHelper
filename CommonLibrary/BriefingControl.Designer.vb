﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BriefingControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.pnlTaskBriefing = New System.Windows.Forms.Panel()
        Me.tabsBriefing = New System.Windows.Forms.TabControl()
        Me.tbpgMainTaskInfo = New System.Windows.Forms.TabPage()
        Me.txtBriefing = New System.Windows.Forms.RichTextBox()
        Me.tbpgMap = New System.Windows.Forms.TabPage()
        Me.mapSplitterUpDown = New System.Windows.Forms.SplitContainer()
        Me.imageViewer = New SIGLR.SoaringTools.ImageViewer.ImageViewerControl()
        Me.mapSplitterLeftRight = New System.Windows.Forms.SplitContainer()
        Me.txtFullDescription = New System.Windows.Forms.RichTextBox()
        Me.txtAltitudeRestrictions = New System.Windows.Forms.RichTextBox()
        Me.tbpgEventInfo = New System.Windows.Forms.TabPage()
        Me.tbpgImages = New System.Windows.Forms.TabPage()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tbpgXBOX = New System.Windows.Forms.TabPage()
        Me.waypointCoordinatesDataGrid = New System.Windows.Forms.DataGridView()
        Me.pnlTaskBriefing.SuspendLayout()
        Me.tabsBriefing.SuspendLayout()
        Me.tbpgMainTaskInfo.SuspendLayout()
        Me.tbpgMap.SuspendLayout()
        CType(Me.mapSplitterUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mapSplitterUpDown.Panel1.SuspendLayout()
        Me.mapSplitterUpDown.Panel2.SuspendLayout()
        Me.mapSplitterUpDown.SuspendLayout()
        CType(Me.mapSplitterLeftRight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mapSplitterLeftRight.Panel1.SuspendLayout()
        Me.mapSplitterLeftRight.Panel2.SuspendLayout()
        Me.mapSplitterLeftRight.SuspendLayout()
        Me.tbpgXBOX.SuspendLayout()
        CType(Me.waypointCoordinatesDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlTaskBriefing
        '
        Me.pnlTaskBriefing.Controls.Add(Me.tabsBriefing)
        Me.pnlTaskBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlTaskBriefing.Location = New System.Drawing.Point(0, 0)
        Me.pnlTaskBriefing.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlTaskBriefing.Name = "pnlTaskBriefing"
        Me.pnlTaskBriefing.Padding = New System.Windows.Forms.Padding(7, 8, 7, 8)
        Me.pnlTaskBriefing.Size = New System.Drawing.Size(1004, 759)
        Me.pnlTaskBriefing.TabIndex = 4
        '
        'tabsBriefing
        '
        Me.tabsBriefing.Controls.Add(Me.tbpgMainTaskInfo)
        Me.tabsBriefing.Controls.Add(Me.tbpgMap)
        Me.tabsBriefing.Controls.Add(Me.tbpgEventInfo)
        Me.tabsBriefing.Controls.Add(Me.tbpgImages)
        Me.tabsBriefing.Controls.Add(Me.tbpgXBOX)
        Me.tabsBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabsBriefing.Location = New System.Drawing.Point(7, 8)
        Me.tabsBriefing.Name = "tabsBriefing"
        Me.tabsBriefing.SelectedIndex = 0
        Me.tabsBriefing.Size = New System.Drawing.Size(990, 743)
        Me.tabsBriefing.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabsBriefing.TabIndex = 0
        '
        'tbpgMainTaskInfo
        '
        Me.tbpgMainTaskInfo.AutoScroll = True
        Me.tbpgMainTaskInfo.Controls.Add(Me.txtBriefing)
        Me.tbpgMainTaskInfo.Location = New System.Drawing.Point(4, 29)
        Me.tbpgMainTaskInfo.Name = "tbpgMainTaskInfo"
        Me.tbpgMainTaskInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpgMainTaskInfo.Size = New System.Drawing.Size(982, 710)
        Me.tbpgMainTaskInfo.TabIndex = 0
        Me.tbpgMainTaskInfo.Text = "Main Task Info"
        Me.tbpgMainTaskInfo.UseVisualStyleBackColor = True
        '
        'txtBriefing
        '
        Me.txtBriefing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBriefing.Font = New System.Drawing.Font("Segoe UI Variable Display", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtBriefing.Location = New System.Drawing.Point(3, 3)
        Me.txtBriefing.Name = "txtBriefing"
        Me.txtBriefing.ReadOnly = True
        Me.txtBriefing.Size = New System.Drawing.Size(976, 704)
        Me.txtBriefing.TabIndex = 4
        Me.txtBriefing.Text = ""
        Me.ToolTip1.SetToolTip(Me.txtBriefing, "Use CTRL-MouseWheel to make the content smaller or larger.")
        '
        'tbpgMap
        '
        Me.tbpgMap.AutoScroll = True
        Me.tbpgMap.Controls.Add(Me.mapSplitterUpDown)
        Me.tbpgMap.Location = New System.Drawing.Point(4, 29)
        Me.tbpgMap.Name = "tbpgMap"
        Me.tbpgMap.Padding = New System.Windows.Forms.Padding(3)
        Me.tbpgMap.Size = New System.Drawing.Size(982, 710)
        Me.tbpgMap.TabIndex = 1
        Me.tbpgMap.Text = "Map"
        Me.tbpgMap.UseVisualStyleBackColor = True
        '
        'mapSplitterUpDown
        '
        Me.mapSplitterUpDown.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mapSplitterUpDown.Location = New System.Drawing.Point(3, 3)
        Me.mapSplitterUpDown.Name = "mapSplitterUpDown"
        Me.mapSplitterUpDown.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'mapSplitterUpDown.Panel1
        '
        Me.mapSplitterUpDown.Panel1.AutoScroll = True
        Me.mapSplitterUpDown.Panel1.Controls.Add(Me.imageViewer)
        '
        'mapSplitterUpDown.Panel2
        '
        Me.mapSplitterUpDown.Panel2.Controls.Add(Me.mapSplitterLeftRight)
        Me.mapSplitterUpDown.Size = New System.Drawing.Size(976, 704)
        Me.mapSplitterUpDown.SplitterDistance = 325
        Me.mapSplitterUpDown.TabIndex = 0
        '
        'imageViewer
        '
        Me.imageViewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.imageViewer.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.imageViewer.Location = New System.Drawing.Point(0, 0)
        Me.imageViewer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.imageViewer.Name = "imageViewer"
        Me.imageViewer.Size = New System.Drawing.Size(976, 325)
        Me.imageViewer.TabIndex = 0
        '
        'mapSplitterLeftRight
        '
        Me.mapSplitterLeftRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.mapSplitterLeftRight.Location = New System.Drawing.Point(0, 0)
        Me.mapSplitterLeftRight.Name = "mapSplitterLeftRight"
        '
        'mapSplitterLeftRight.Panel1
        '
        Me.mapSplitterLeftRight.Panel1.Controls.Add(Me.txtFullDescription)
        '
        'mapSplitterLeftRight.Panel2
        '
        Me.mapSplitterLeftRight.Panel2.Controls.Add(Me.txtAltitudeRestrictions)
        Me.mapSplitterLeftRight.Size = New System.Drawing.Size(976, 375)
        Me.mapSplitterLeftRight.SplitterDistance = 650
        Me.mapSplitterLeftRight.TabIndex = 0
        '
        'txtFullDescription
        '
        Me.txtFullDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtFullDescription.Location = New System.Drawing.Point(0, 0)
        Me.txtFullDescription.Name = "txtFullDescription"
        Me.txtFullDescription.ReadOnly = True
        Me.txtFullDescription.Size = New System.Drawing.Size(650, 375)
        Me.txtFullDescription.TabIndex = 0
        Me.txtFullDescription.Text = ""
        '
        'txtAltitudeRestrictions
        '
        Me.txtAltitudeRestrictions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtAltitudeRestrictions.Location = New System.Drawing.Point(0, 0)
        Me.txtAltitudeRestrictions.Name = "txtAltitudeRestrictions"
        Me.txtAltitudeRestrictions.ReadOnly = True
        Me.txtAltitudeRestrictions.Size = New System.Drawing.Size(322, 375)
        Me.txtAltitudeRestrictions.TabIndex = 1
        Me.txtAltitudeRestrictions.Text = ""
        '
        'tbpgEventInfo
        '
        Me.tbpgEventInfo.AutoScroll = True
        Me.tbpgEventInfo.Location = New System.Drawing.Point(4, 29)
        Me.tbpgEventInfo.Name = "tbpgEventInfo"
        Me.tbpgEventInfo.Size = New System.Drawing.Size(982, 710)
        Me.tbpgEventInfo.TabIndex = 3
        Me.tbpgEventInfo.Text = "Event Info"
        Me.tbpgEventInfo.UseVisualStyleBackColor = True
        '
        'tbpgImages
        '
        Me.tbpgImages.AutoScroll = True
        Me.tbpgImages.Location = New System.Drawing.Point(4, 29)
        Me.tbpgImages.Name = "tbpgImages"
        Me.tbpgImages.Size = New System.Drawing.Size(982, 710)
        Me.tbpgImages.TabIndex = 2
        Me.tbpgImages.Text = "Images"
        Me.tbpgImages.UseVisualStyleBackColor = True
        '
        'tbpgXBOX
        '
        Me.tbpgXBOX.Controls.Add(Me.waypointCoordinatesDataGrid)
        Me.tbpgXBOX.Location = New System.Drawing.Point(4, 29)
        Me.tbpgXBOX.Name = "tbpgXBOX"
        Me.tbpgXBOX.Size = New System.Drawing.Size(982, 710)
        Me.tbpgXBOX.TabIndex = 4
        Me.tbpgXBOX.Text = "XBOX"
        Me.tbpgXBOX.UseVisualStyleBackColor = True
        '
        'waypointCoordinatesDataGrid
        '
        Me.waypointCoordinatesDataGrid.AllowUserToAddRows = False
        Me.waypointCoordinatesDataGrid.AllowUserToDeleteRows = False
        Me.waypointCoordinatesDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Segoe UI Variable Display", 14.0!, System.Drawing.FontStyle.Bold)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.waypointCoordinatesDataGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.waypointCoordinatesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Segoe UI Variable Display", 12.0!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.waypointCoordinatesDataGrid.DefaultCellStyle = DataGridViewCellStyle2
        Me.waypointCoordinatesDataGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.waypointCoordinatesDataGrid.Location = New System.Drawing.Point(0, 0)
        Me.waypointCoordinatesDataGrid.Name = "waypointCoordinatesDataGrid"
        Me.waypointCoordinatesDataGrid.RowHeadersWidth = 47
        Me.waypointCoordinatesDataGrid.Size = New System.Drawing.Size(982, 710)
        Me.waypointCoordinatesDataGrid.TabIndex = 0
        '
        'BriefingControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlTaskBriefing)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(700, 500)
        Me.Name = "BriefingControl"
        Me.Size = New System.Drawing.Size(1004, 759)
        Me.pnlTaskBriefing.ResumeLayout(False)
        Me.tabsBriefing.ResumeLayout(False)
        Me.tbpgMainTaskInfo.ResumeLayout(False)
        Me.tbpgMap.ResumeLayout(False)
        Me.mapSplitterUpDown.Panel1.ResumeLayout(False)
        Me.mapSplitterUpDown.Panel2.ResumeLayout(False)
        CType(Me.mapSplitterUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mapSplitterUpDown.ResumeLayout(False)
        Me.mapSplitterLeftRight.Panel1.ResumeLayout(False)
        Me.mapSplitterLeftRight.Panel2.ResumeLayout(False)
        CType(Me.mapSplitterLeftRight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mapSplitterLeftRight.ResumeLayout(False)
        Me.tbpgXBOX.ResumeLayout(False)
        CType(Me.waypointCoordinatesDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTaskBriefing As Windows.Forms.Panel
    Friend WithEvents tabsBriefing As Windows.Forms.TabControl
    Friend WithEvents tbpgMainTaskInfo As Windows.Forms.TabPage
    Friend WithEvents tbpgMap As Windows.Forms.TabPage
    Friend WithEvents tbpgEventInfo As Windows.Forms.TabPage
    Friend WithEvents tbpgImages As Windows.Forms.TabPage
    Friend WithEvents txtBriefing As Windows.Forms.RichTextBox
    Friend WithEvents ToolTip1 As Windows.Forms.ToolTip
    Friend WithEvents mapSplitterUpDown As Windows.Forms.SplitContainer
    Friend WithEvents mapSplitterLeftRight As Windows.Forms.SplitContainer
    Friend WithEvents imageViewer As ImageViewerControl
    Friend WithEvents txtFullDescription As Windows.Forms.RichTextBox
    Friend WithEvents txtAltitudeRestrictions As Windows.Forms.RichTextBox
    Friend WithEvents tbpgXBOX As Windows.Forms.TabPage
    Friend WithEvents waypointCoordinatesDataGrid As Windows.Forms.DataGridView
End Class