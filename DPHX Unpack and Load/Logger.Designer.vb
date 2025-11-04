<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Logger
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Logger))
        Me.ui_simtime_label = New System.Windows.Forms.Label()
        Me.ui_recording_time = New System.Windows.Forms.Label()
        Me.ui_task = New System.Windows.Forms.Label()
        Me.ui_aircraft = New System.Windows.Forms.Label()
        Me.ui_pilot = New System.Windows.Forms.Label()
        Me.ui_aircraft_label = New System.Windows.Forms.Label()
        Me.ui_pilot_label = New System.Windows.Forms.Label()
        Me.ui_local_time = New System.Windows.Forms.Label()
        Me.ui_conn_status = New System.Windows.Forms.Label()
        Me.pictureBox_statusImage = New System.Windows.Forms.PictureBox()
        Me.ui_min_max = New System.Windows.Forms.LinkLabel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.view_tracklogs_button = New System.Windows.Forms.Button()
        Me.ui_message_bar = New System.Windows.Forms.Label()
        CType(Me.pictureBox_statusImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ui_simtime_label
        '
        Me.ui_simtime_label.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_simtime_label.Location = New System.Drawing.Point(13, 107)
        Me.ui_simtime_label.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_simtime_label.Name = "ui_simtime_label"
        Me.ui_simtime_label.Size = New System.Drawing.Size(109, 31)
        Me.ui_simtime_label.TabIndex = 22
        Me.ui_simtime_label.Text = "Sim Time:"
        Me.ui_simtime_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ui_recording_time
        '
        Me.ui_recording_time.BackColor = System.Drawing.Color.Transparent
        Me.ui_recording_time.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_recording_time.Location = New System.Drawing.Point(498, 27)
        Me.ui_recording_time.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_recording_time.MinimumSize = New System.Drawing.Size(67, 37)
        Me.ui_recording_time.Name = "ui_recording_time"
        Me.ui_recording_time.Size = New System.Drawing.Size(112, 37)
        Me.ui_recording_time.TabIndex = 21
        Me.ui_recording_time.Text = "00:00:00"
        Me.ui_recording_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ui_task
        '
        Me.ui_task.AllowDrop = False
        Me.ui_task.Cursor = System.Windows.Forms.Cursors.Default
        Me.ui_task.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_task.Location = New System.Drawing.Point(130, 169)
        Me.ui_task.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_task.Name = "ui_task"
        Me.ui_task.Size = New System.Drawing.Size(480, 31)
        Me.ui_task.TabIndex = 20
        Me.ui_task.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ui_aircraft
        '
        Me.ui_aircraft.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_aircraft.Location = New System.Drawing.Point(130, 138)
        Me.ui_aircraft.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_aircraft.Name = "ui_aircraft"
        Me.ui_aircraft.Size = New System.Drawing.Size(480, 31)
        Me.ui_aircraft.TabIndex = 19
        Me.ui_aircraft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ui_pilot
        '
        Me.ui_pilot.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_pilot.Location = New System.Drawing.Point(130, 76)
        Me.ui_pilot.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_pilot.Name = "ui_pilot"
        Me.ui_pilot.Size = New System.Drawing.Size(480, 31)
        Me.ui_pilot.TabIndex = 18
        Me.ui_pilot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ui_aircraft_label
        '
        Me.ui_aircraft_label.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_aircraft_label.Location = New System.Drawing.Point(13, 138)
        Me.ui_aircraft_label.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_aircraft_label.Name = "ui_aircraft_label"
        Me.ui_aircraft_label.Size = New System.Drawing.Size(109, 31)
        Me.ui_aircraft_label.TabIndex = 17
        Me.ui_aircraft_label.Text = "Aircraft:"
        Me.ui_aircraft_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ui_pilot_label
        '
        Me.ui_pilot_label.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_pilot_label.Location = New System.Drawing.Point(13, 76)
        Me.ui_pilot_label.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_pilot_label.Name = "ui_pilot_label"
        Me.ui_pilot_label.Size = New System.Drawing.Size(109, 31)
        Me.ui_pilot_label.TabIndex = 16
        Me.ui_pilot_label.Text = "Pilot:"
        Me.ui_pilot_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ui_local_time
        '
        Me.ui_local_time.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_local_time.Location = New System.Drawing.Point(130, 107)
        Me.ui_local_time.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_local_time.Name = "ui_local_time"
        Me.ui_local_time.Size = New System.Drawing.Size(240, 31)
        Me.ui_local_time.TabIndex = 15
        Me.ui_local_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ui_conn_status
        '
        Me.ui_conn_status.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.ui_conn_status.Location = New System.Drawing.Point(88, 27)
        Me.ui_conn_status.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_conn_status.Name = "ui_conn_status"
        Me.ui_conn_status.Size = New System.Drawing.Size(446, 37)
        Me.ui_conn_status.TabIndex = 14
        Me.ui_conn_status.Text = "Waiting for MSFS."
        Me.ui_conn_status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pictureBox_statusImage
        '
        Me.pictureBox_statusImage.Location = New System.Drawing.Point(13, 14)
        Me.pictureBox_statusImage.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pictureBox_statusImage.Name = "pictureBox_statusImage"
        Me.pictureBox_statusImage.Size = New System.Drawing.Size(61, 60)
        Me.pictureBox_statusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pictureBox_statusImage.TabIndex = 13
        Me.pictureBox_statusImage.TabStop = False
        '
        'ui_min_max
        '
        Me.ui_min_max.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ui_min_max.Location = New System.Drawing.Point(535, 7)
        Me.ui_min_max.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.ui_min_max.Name = "ui_min_max"
        Me.ui_min_max.Size = New System.Drawing.Size(71, 20)
        Me.ui_min_max.TabIndex = 23
        Me.ui_min_max.TabStop = True
        Me.ui_min_max.Text = "Compact"
        Me.ui_min_max.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(13, 169)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(109, 31)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "Task:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'view_tracklogs_button
        '
        Me.view_tracklogs_button.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.view_tracklogs_button.Location = New System.Drawing.Point(13, 203)
        Me.view_tracklogs_button.Name = "view_tracklogs_button"
        Me.view_tracklogs_button.Size = New System.Drawing.Size(111, 36)
        Me.view_tracklogs_button.TabIndex = 25
        Me.view_tracklogs_button.Text = "Browse Files"
        Me.view_tracklogs_button.UseVisualStyleBackColor = True
        '
        'ui_message_bar
        '
        Me.ui_message_bar.Font = New System.Drawing.Font("Segoe UI", 11.78182!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ui_message_bar.Location = New System.Drawing.Point(12, 242)
        Me.ui_message_bar.Name = "ui_message_bar"
        Me.ui_message_bar.Size = New System.Drawing.Size(594, 31)
        Me.ui_message_bar.TabIndex = 26
        Me.ui_message_bar.Tag = ""
        Me.ui_message_bar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Logger
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.LightCyan
        Me.ClientSize = New System.Drawing.Size(616, 280)
        Me.Controls.Add(Me.ui_message_bar)
        Me.Controls.Add(Me.view_tracklogs_button)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ui_min_max)
        Me.Controls.Add(Me.ui_simtime_label)
        Me.Controls.Add(Me.ui_recording_time)
        Me.Controls.Add(Me.ui_task)
        Me.Controls.Add(Me.ui_aircraft)
        Me.Controls.Add(Me.ui_pilot)
        Me.Controls.Add(Me.ui_aircraft_label)
        Me.Controls.Add(Me.ui_pilot_label)
        Me.Controls.Add(Me.ui_local_time)
        Me.Controls.Add(Me.ui_conn_status)
        Me.Controls.Add(Me.pictureBox_statusImage)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.Name = "Logger"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "NB21 Logger"
        CType(Me.pictureBox_statusImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents ui_simtime_label As Label
    Private WithEvents ui_recording_time As Label
    Private WithEvents ui_task As Label
    Private WithEvents ui_aircraft As Label
    Private WithEvents ui_pilot As Label
    Private WithEvents ui_aircraft_label As Label
    Private WithEvents ui_pilot_label As Label
    Private WithEvents ui_local_time As Label
    Private WithEvents ui_conn_status As Label
    Private WithEvents pictureBox_statusImage As PictureBox
    Private WithEvents ui_min_max As LinkLabel
    Private WithEvents Label1 As Label
    Private WithEvents view_tracklogs_button As Button
    Private WithEvents ui_message_bar As Label
End Class
