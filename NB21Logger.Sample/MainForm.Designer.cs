using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NB21Logger.Sample.Properties;

namespace NB21Logger.Sample;

partial class MainForm
{
    private void InitializeComponent()
    {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Home = new System.Windows.Forms.TabPage();
            this.ui_task_button = new System.Windows.Forms.Button();
            this.ui_file_icon = new System.Windows.Forms.PictureBox();
            this.ui_min_max = new System.Windows.Forms.LinkLabel();
            this.ui_message_bar = new System.Windows.Forms.Label();
            this.view_tracklogs_button = new System.Windows.Forms.Button();
            this.ui_simtime_label = new System.Windows.Forms.Label();
            this.ui_recording_time = new System.Windows.Forms.Label();
            this.ui_sim_rate = new System.Windows.Forms.Label();
            this.ui_task = new System.Windows.Forms.Label();
            this.ui_aircraft = new System.Windows.Forms.Label();
            this.ui_pilot = new System.Windows.Forms.Label();
            this.ui_aircraft_label = new System.Windows.Forms.Label();
            this.ui_pilot_label = new System.Windows.Forms.Label();
            this.ui_local_time = new System.Windows.Forms.Label();
            this.ui_local_date = new System.Windows.Forms.Label();
            this.ui_conn_status = new System.Windows.Forms.Label();
            this.pictureBox_statusImage = new System.Windows.Forms.PictureBox();
            this.pln_drop_outline = new System.Windows.Forms.Label();
            this.tabPage_Settings = new System.Windows.Forms.TabPage();
            this.ui_web_urls = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ui_settings_igc_path = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ui_settings_pilot_id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ui_settings_pilot_name = new System.Windows.Forms.TextBox();
            this.ui_auto_start_label1 = new System.Windows.Forms.Label();
            this.ui_auto_start_checkbox = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage_Home.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ui_file_icon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_statusImage)).BeginInit();
            this.tabPage_Settings.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage_Home);
            this.tabControl1.Controls.Add(this.tabPage_Settings);
            this.tabControl1.ItemSize = new System.Drawing.Size(115, 60);
            this.tabControl1.Location = new System.Drawing.Point(2, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(592, 244);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Click += new System.EventHandler(this.ui_tab_click);
            // 
            // tabPage_Home
            // 
            this.tabPage_Home.AllowDrop = true;
            this.tabPage_Home.BackColor = System.Drawing.Color.LightCyan;
            this.tabPage_Home.Controls.Add(this.ui_task_button);
            this.tabPage_Home.Controls.Add(this.ui_file_icon);
            this.tabPage_Home.Controls.Add(this.ui_min_max);
            this.tabPage_Home.Controls.Add(this.ui_message_bar);
            this.tabPage_Home.Controls.Add(this.view_tracklogs_button);
            this.tabPage_Home.Controls.Add(this.ui_simtime_label);
            this.tabPage_Home.Controls.Add(this.ui_recording_time);
            this.tabPage_Home.Controls.Add(this.ui_sim_rate);
            this.tabPage_Home.Controls.Add(this.ui_task);
            this.tabPage_Home.Controls.Add(this.ui_aircraft);
            this.tabPage_Home.Controls.Add(this.ui_pilot);
            this.tabPage_Home.Controls.Add(this.ui_aircraft_label);
            this.tabPage_Home.Controls.Add(this.ui_pilot_label);
            this.tabPage_Home.Controls.Add(this.ui_local_time);
            this.tabPage_Home.Controls.Add(this.ui_local_date);
            this.tabPage_Home.Controls.Add(this.ui_conn_status);
            this.tabPage_Home.Controls.Add(this.pictureBox_statusImage);
            this.tabPage_Home.Controls.Add(this.pln_drop_outline);
            this.tabPage_Home.Location = new System.Drawing.Point(64, 4);
            this.tabPage_Home.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_Home.Name = "tabPage_Home";
            this.tabPage_Home.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_Home.Size = new System.Drawing.Size(524, 236);
            this.tabPage_Home.TabIndex = 0;
            this.tabPage_Home.DragOver += new System.Windows.Forms.DragEventHandler(this.ui_home_dragover);
            this.tabPage_Home.Enter += new System.EventHandler(this.ui_home_select);
            // 
            // ui_task_button
            // 
            this.ui_task_button.AllowDrop = true;
            this.ui_task_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ui_task_button.Location = new System.Drawing.Point(22, 148);
            this.ui_task_button.Name = "ui_task_button";
            this.ui_task_button.Size = new System.Drawing.Size(97, 30);
            this.ui_task_button.TabIndex = 17;
            this.ui_task_button.Text = "Task:";
            this.ui_task_button.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ui_task_button.UseVisualStyleBackColor = true;
            this.ui_task_button.Click += new System.EventHandler(this.ui_task_click);
            this.ui_task_button.DragDrop += new System.Windows.Forms.DragEventHandler(this.ui_task_DragDrop);
            this.ui_task_button.DragEnter += new System.Windows.Forms.DragEventHandler(this.ui_task_DragEnter);
            // 
            // ui_file_icon
            // 
            this.ui_file_icon.Image = global::NB21Logger.Sample.Properties.Resources.file_icon;
            this.ui_file_icon.Location = new System.Drawing.Point(476, 174);
            this.ui_file_icon.Name = "ui_file_icon";
            this.ui_file_icon.Size = new System.Drawing.Size(38, 57);
            this.ui_file_icon.TabIndex = 15;
            this.ui_file_icon.TabStop = false;
            this.ui_file_icon.Visible = false;
            this.ui_file_icon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ui_file_drag);
            // 
            // ui_min_max
            // 
            this.ui_min_max.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ui_min_max.AutoSize = true;
            this.ui_min_max.Location = new System.Drawing.Point(458, 5);
            this.ui_min_max.Name = "ui_min_max";
            this.ui_min_max.Size = new System.Drawing.Size(64, 19);
            this.ui_min_max.TabIndex = 16;
            this.ui_min_max.TabStop = true;
            this.ui_min_max.Text = "Compact";
            this.ui_min_max.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ui_min_max_LinkClicked);
            // 
            // ui_message_bar
            // 
            this.ui_message_bar.Font = new System.Drawing.Font("Segoe UI", 15.75F);
            this.ui_message_bar.Location = new System.Drawing.Point(140, 183);
            this.ui_message_bar.Name = "ui_message_bar";
            this.ui_message_bar.Size = new System.Drawing.Size(293, 30);
            this.ui_message_bar.TabIndex = 14;
            this.ui_message_bar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // view_tracklogs_button
            // 
            this.view_tracklogs_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.view_tracklogs_button.Location = new System.Drawing.Point(14, 187);
            this.view_tracklogs_button.Name = "view_tracklogs_button";
            this.view_tracklogs_button.Size = new System.Drawing.Size(111, 31);
            this.view_tracklogs_button.TabIndex = 13;
            this.view_tracklogs_button.Text = "Browse Files";
            this.view_tracklogs_button.UseVisualStyleBackColor = true;
            this.view_tracklogs_button.Visible = false;
            this.view_tracklogs_button.Click += new System.EventHandler(this.ui_view_tracklogs);
            // 
            // ui_simtime_label
            // 
            this.ui_simtime_label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_simtime_label.Location = new System.Drawing.Point(3, 94);
            this.ui_simtime_label.Name = "ui_simtime_label";
            this.ui_simtime_label.Size = new System.Drawing.Size(113, 26);
            this.ui_simtime_label.TabIndex = 12;
            this.ui_simtime_label.Text = "Sim Time:";
            this.ui_simtime_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_recording_time
            // 
            this.ui_recording_time.BackColor = System.Drawing.Color.Transparent;
            this.ui_recording_time.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Bold);
            this.ui_recording_time.Location = new System.Drawing.Point(415, 24);
            this.ui_recording_time.MinimumSize = new System.Drawing.Size(50, 24);
            this.ui_recording_time.Name = "ui_recording_time";
            this.ui_recording_time.Size = new System.Drawing.Size(105, 24);
            this.ui_recording_time.TabIndex = 11;
            this.ui_recording_time.Text = "00:00:00";
            this.ui_recording_time.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_sim_rate
            // 
            this.ui_sim_rate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_sim_rate.Location = new System.Drawing.Point(388, 28);
            this.ui_sim_rate.Name = "ui_sim_rate";
            this.ui_sim_rate.Size = new System.Drawing.Size(130, 24);
            this.ui_sim_rate.TabIndex = 10;
            this.ui_sim_rate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_task
            // 
            this.ui_task.AllowDrop = true;
            this.ui_task.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_task.Location = new System.Drawing.Point(125, 148);
            this.ui_task.Name = "ui_task";
            this.ui_task.Size = new System.Drawing.Size(393, 26);
            this.ui_task.TabIndex = 9;
            this.ui_task.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ui_task.DragDrop += new System.Windows.Forms.DragEventHandler(this.ui_task_DragDrop);
            this.ui_task.DragEnter += new System.Windows.Forms.DragEventHandler(this.ui_task_DragEnter);
            // 
            // ui_aircraft
            // 
            this.ui_aircraft.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_aircraft.Location = new System.Drawing.Point(125, 122);
            this.ui_aircraft.Name = "ui_aircraft";
            this.ui_aircraft.Size = new System.Drawing.Size(393, 26);
            this.ui_aircraft.TabIndex = 8;
            this.ui_aircraft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ui_pilot
            // 
            this.ui_pilot.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_pilot.Location = new System.Drawing.Point(125, 67);
            this.ui_pilot.Name = "ui_pilot";
            this.ui_pilot.Size = new System.Drawing.Size(393, 26);
            this.ui_pilot.TabIndex = 7;
            this.ui_pilot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ui_aircraft_label
            // 
            this.ui_aircraft_label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_aircraft_label.Location = new System.Drawing.Point(3, 122);
            this.ui_aircraft_label.Name = "ui_aircraft_label";
            this.ui_aircraft_label.Size = new System.Drawing.Size(113, 26);
            this.ui_aircraft_label.TabIndex = 6;
            this.ui_aircraft_label.Text = "Aircraft:";
            this.ui_aircraft_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_pilot_label
            // 
            this.ui_pilot_label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_pilot_label.Location = new System.Drawing.Point(3, 67);
            this.ui_pilot_label.Name = "ui_pilot_label";
            this.ui_pilot_label.Size = new System.Drawing.Size(113, 26);
            this.ui_pilot_label.TabIndex = 5;
            this.ui_pilot_label.Text = "Pilot:";
            this.ui_pilot_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_local_time
            // 
            this.ui_local_time.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_local_time.Location = new System.Drawing.Point(125, 94);
            this.ui_local_time.Name = "ui_local_time";
            this.ui_local_time.Size = new System.Drawing.Size(180, 26);
            this.ui_local_time.TabIndex = 4;
            this.ui_local_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ui_local_date
            // 
            this.ui_local_date.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_local_date.Location = new System.Drawing.Point(311, 94);
            this.ui_local_date.Name = "ui_local_date";
            this.ui_local_date.Size = new System.Drawing.Size(207, 26);
            this.ui_local_date.TabIndex = 3;
            this.ui_local_date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ui_conn_status
            // 
            this.ui_conn_status.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.ui_conn_status.Location = new System.Drawing.Point(78, 24);
            this.ui_conn_status.Name = "ui_conn_status";
            this.ui_conn_status.Size = new System.Drawing.Size(331, 24);
            this.ui_conn_status.TabIndex = 2;
            this.ui_conn_status.Text = "Waiting for MSFS.";
            this.ui_conn_status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox_statusImage
            // 
            this.pictureBox_statusImage.Location = new System.Drawing.Point(22, 12);
            this.pictureBox_statusImage.Name = "pictureBox_statusImage";
            this.pictureBox_statusImage.Size = new System.Drawing.Size(50, 51);
            this.pictureBox_statusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_statusImage.TabIndex = 1;
            this.pictureBox_statusImage.TabStop = false;
            // 
            // pln_drop_outline
            // 
            this.pln_drop_outline.AllowDrop = true;
            this.pln_drop_outline.BackColor = System.Drawing.Color.Transparent;
            this.pln_drop_outline.Location = new System.Drawing.Point(6, 144);
            this.pln_drop_outline.Name = "pln_drop_outline";
            this.pln_drop_outline.Size = new System.Drawing.Size(514, 42);
            this.pln_drop_outline.TabIndex = 18;
            this.pln_drop_outline.DragDrop += new System.Windows.Forms.DragEventHandler(this.ui_task_DragDrop);
            this.pln_drop_outline.DragEnter += new System.Windows.Forms.DragEventHandler(this.ui_task_DragEnter);
            // 
            // tabPage_Settings
            // 
            this.tabPage_Settings.BackColor = System.Drawing.Color.LightYellow;
            this.tabPage_Settings.Controls.Add(this.ui_web_urls);
            this.tabPage_Settings.Controls.Add(this.label3);
            this.tabPage_Settings.Controls.Add(this.button1);
            this.tabPage_Settings.Controls.Add(this.ui_settings_igc_path);
            this.tabPage_Settings.Controls.Add(this.label2);
            this.tabPage_Settings.Controls.Add(this.ui_settings_pilot_id);
            this.tabPage_Settings.Controls.Add(this.label1);
            this.tabPage_Settings.Controls.Add(this.ui_settings_pilot_name);
            this.tabPage_Settings.Controls.Add(this.ui_auto_start_label1);
            this.tabPage_Settings.Controls.Add(this.ui_auto_start_checkbox);
            this.tabPage_Settings.Location = new System.Drawing.Point(64, 4);
            this.tabPage_Settings.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_Settings.Name = "tabPage_Settings";
            this.tabPage_Settings.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage_Settings.Size = new System.Drawing.Size(524, 236);
            this.tabPage_Settings.TabIndex = 1;
            // 
            // ui_web_urls
            // 
            this.ui_web_urls.AutoSize = true;
            this.ui_web_urls.Location = new System.Drawing.Point(15, 200);
            this.ui_web_urls.Name = "ui_web_urls";
            this.ui_web_urls.Size = new System.Drawing.Size(0, 19);
            this.ui_web_urls.TabIndex = 9;
            this.ui_web_urls.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ui_web_urls_LinkClicked);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(15, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "Tracklogs folder:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(411, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ui_browse_tracklogs);
            // 
            // ui_settings_igc_path
            // 
            this.ui_settings_igc_path.Location = new System.Drawing.Point(166, 122);
            this.ui_settings_igc_path.Name = "ui_settings_igc_path";
            this.ui_settings_igc_path.Size = new System.Drawing.Size(239, 26);
            this.ui_settings_igc_path.TabIndex = 6;
            this.ui_settings_igc_path.Leave += new System.EventHandler(this.ui_settings_igc_path_Leave);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(15, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Pilot ID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_settings_pilot_id
            // 
            this.ui_settings_pilot_id.Location = new System.Drawing.Point(166, 75);
            this.ui_settings_pilot_id.Name = "ui_settings_pilot_id";
            this.ui_settings_pilot_id.Size = new System.Drawing.Size(239, 26);
            this.ui_settings_pilot_id.TabIndex = 4;
            this.ui_settings_pilot_id.Leave += new System.EventHandler(this.ui_settings_pilot_id_Leave);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(15, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pilot name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_settings_pilot_name
            // 
            this.ui_settings_pilot_name.Location = new System.Drawing.Point(166, 28);
            this.ui_settings_pilot_name.Name = "ui_settings_pilot_name";
            this.ui_settings_pilot_name.Size = new System.Drawing.Size(239, 26);
            this.ui_settings_pilot_name.TabIndex = 2;
            this.ui_settings_pilot_name.Leave += new System.EventHandler(this.ui_settings_pilot_name_Leave);
            // 
            // ui_auto_start_label1
            // 
            this.ui_auto_start_label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.ui_auto_start_label1.Location = new System.Drawing.Point(15, 166);
            this.ui_auto_start_label1.Name = "ui_auto_start_label1";
            this.ui_auto_start_label1.Size = new System.Drawing.Size(145, 23);
            this.ui_auto_start_label1.TabIndex = 1;
            this.ui_auto_start_label1.Text = "Auto start:";
            this.ui_auto_start_label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ui_auto_start_checkbox
            // 
            this.ui_auto_start_checkbox.AutoSize = true;
            this.ui_auto_start_checkbox.Location = new System.Drawing.Point(166, 170);
            this.ui_auto_start_checkbox.Name = "ui_auto_start_checkbox";
            this.ui_auto_start_checkbox.Size = new System.Drawing.Size(68, 23);
            this.ui_auto_start_checkbox.TabIndex = 0;
            this.ui_auto_start_checkbox.Text = "Enable";
            this.ui_auto_start_checkbox.UseVisualStyleBackColor = true;
            this.ui_auto_start_checkbox.CheckedChanged += new System.EventHandler(this.ui_auto_start_checkbox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 240);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = global::NB21Logger.Sample.Properties.Resources.app_icon;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "NB21 Logger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_Home.ResumeLayout(false);
            this.tabPage_Home.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ui_file_icon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_statusImage)).EndInit();
            this.tabPage_Settings.ResumeLayout(false);
            this.tabPage_Settings.PerformLayout();
            this.ResumeLayout(false);

    }

    private System.ComponentModel.IContainer components;
    private TabControl tabControl1;
    private TabPage tabPage_Home;
    private Button ui_task_button;
    private PictureBox ui_file_icon;
    private LinkLabel ui_min_max;
    private Label ui_message_bar;
    private Button view_tracklogs_button;
    private Label ui_simtime_label;
    private Label ui_recording_time;
    private Label ui_sim_rate;
    private Label ui_task;
    private Label ui_aircraft;
    private Label ui_pilot;
    private Label ui_aircraft_label;
    private Label ui_pilot_label;
    private Label ui_local_time;
    private Label ui_local_date;
    private Label ui_conn_status;
    private PictureBox pictureBox_statusImage;
    private Label pln_drop_outline;
    private TabPage tabPage_Settings;
    private LinkLabel ui_web_urls;
    private Label label3;
    private Button button1;
    private TextBox ui_settings_igc_path;
    private Label label2;
    private TextBox ui_settings_pilot_id;
    private Label label1;
    private TextBox ui_settings_pilot_name;
    private Label ui_auto_start_label1;
    private CheckBox ui_auto_start_checkbox;
}
