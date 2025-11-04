using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NB21Logger.Sample.Properties;

namespace NB21Logger.Sample;

partial class MainForm
{
    private void InitializeComponent()
    {
        this.components = new Container();
        this.tabControl1 = new TabControl();
        this.tabPage_Home = new TabPage();
        this.ui_task_button = new Button();
        this.ui_file_icon = new PictureBox();
        this.ui_min_max = new LinkLabel();
        this.ui_message_bar = new Label();
        this.view_tracklogs_button = new Button();
        this.ui_simtime_label = new Label();
        this.ui_recording_time = new Label();
        this.ui_sim_rate = new Label();
        this.ui_task = new Label();
        this.ui_aircraft = new Label();
        this.ui_pilot = new Label();
        this.ui_aircraft_label = new Label();
        this.ui_pilot_label = new Label();
        this.ui_local_time = new Label();
        this.ui_local_date = new Label();
        this.ui_conn_status = new Label();
        this.pictureBox_statusImage = new PictureBox();
        this.pln_drop_outline = new Label();
        this.tabPage_Settings = new TabPage();
        this.ui_web_urls = new LinkLabel();
        this.label3 = new Label();
        this.button1 = new Button();
        this.ui_settings_igc_path = new TextBox();
        this.label2 = new Label();
        this.ui_settings_pilot_id = new TextBox();
        this.label1 = new Label();
        this.ui_settings_pilot_name = new TextBox();
        this.ui_auto_start_label1 = new Label();
        this.ui_auto_start_checkbox = new CheckBox();
        this.tabControl1.SuspendLayout();
        this.tabPage_Home.SuspendLayout();
        ((ISupportInitialize)(this.ui_file_icon)).BeginInit();
        ((ISupportInitialize)(this.pictureBox_statusImage)).BeginInit();
        this.tabPage_Settings.SuspendLayout();
        this.SuspendLayout();
        // 
        // tabControl1
        // 
        this.tabControl1.Alignment = TabAlignment.Left;
        this.tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.tabControl1.Controls.Add(this.tabPage_Home);
        this.tabControl1.Controls.Add(this.tabPage_Settings);
        this.tabControl1.ItemSize = new Size(115, 60);
        this.tabControl1.Location = new Point(2, 0);
        this.tabControl1.Margin = new Padding(0);
        this.tabControl1.Multiline = true;
        this.tabControl1.Name = "tabControl1";
        this.tabControl1.Padding = new Point(0, 0);
        this.tabControl1.SelectedIndex = 0;
        this.tabControl1.Size = new Size(592, 244);
        this.tabControl1.SizeMode = TabSizeMode.Fixed;
        this.tabControl1.TabIndex = 1;
        this.tabControl1.Click += new System.EventHandler(this.ui_tab_click);
        // 
        // tabPage_Home
        // 
        this.tabPage_Home.AllowDrop = true;
        this.tabPage_Home.BackColor = Color.LightCyan;
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
        this.tabPage_Home.Location = new Point(64, 4);
        this.tabPage_Home.Margin = new Padding(3, 2, 3, 2);
        this.tabPage_Home.Name = "tabPage_Home";
        this.tabPage_Home.Padding = new Padding(3, 2, 3, 2);
        this.tabPage_Home.Size = new Size(524, 236);
        this.tabPage_Home.TabIndex = 0;
        this.tabPage_Home.DragOver += new DragEventHandler(this.ui_home_dragover);
        this.tabPage_Home.Enter += new System.EventHandler(this.ui_home_select);
        // 
        // ui_task_button
        // 
        this.ui_task_button.AllowDrop = true;
        this.ui_task_button.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        this.ui_task_button.Location = new Point(22, 148);
        this.ui_task_button.Name = "ui_task_button";
        this.ui_task_button.Size = new Size(97, 30);
        this.ui_task_button.TabIndex = 17;
        this.ui_task_button.Text = "Task:";
        this.ui_task_button.TextAlign = ContentAlignment.MiddleRight;
        this.ui_task_button.UseVisualStyleBackColor = true;
        this.ui_task_button.Click += new System.EventHandler(this.ui_task_click);
        this.ui_task_button.DragDrop += new DragEventHandler(this.ui_task_DragDrop);
        this.ui_task_button.DragEnter += new DragEventHandler(this.ui_task_DragEnter);
        // 
        // ui_file_icon
        // 
        this.ui_file_icon.Image = Properties.Resources.file_icon;
        this.ui_file_icon.Location = new Point(476, 174);
        this.ui_file_icon.Name = "ui_file_icon";
        this.ui_file_icon.Size = new Size(38, 57);
        this.ui_file_icon.TabIndex = 15;
        this.ui_file_icon.TabStop = false;
        this.ui_file_icon.Visible = false;
        this.ui_file_icon.MouseDown += new MouseEventHandler(this.ui_file_drag);
        // 
        // ui_min_max
        // 
        this.ui_min_max.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.ui_min_max.AutoSize = true;
        this.ui_min_max.Location = new Point(458, 5);
        this.ui_min_max.Name = "ui_min_max";
        this.ui_min_max.Size = new Size(56, 15);
        this.ui_min_max.TabIndex = 16;
        this.ui_min_max.TabStop = true;
        this.ui_min_max.Text = "Compact";
        this.ui_min_max.LinkClicked += new LinkLabelLinkClickedEventHandler(this.ui_min_max_LinkClicked);
        // 
        // ui_message_bar
        // 
        this.ui_message_bar.Font = new Font("Segoe UI", 15.75F);
        this.ui_message_bar.Location = new Point(140, 183);
        this.ui_message_bar.Name = "ui_message_bar";
        this.ui_message_bar.Size = new Size(293, 30);
        this.ui_message_bar.TabIndex = 14;
        this.ui_message_bar.TextAlign = ContentAlignment.MiddleRight;
        // 
        // view_tracklogs_button
        // 
        this.view_tracklogs_button.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.view_tracklogs_button.Location = new Point(14, 187);
        this.view_tracklogs_button.Name = "view_tracklogs_button";
        this.view_tracklogs_button.Size = new Size(111, 31);
        this.view_tracklogs_button.TabIndex = 13;
        this.view_tracklogs_button.Text = "Browse Files";
        this.view_tracklogs_button.UseVisualStyleBackColor = true;
        this.view_tracklogs_button.Visible = false;
        this.view_tracklogs_button.Click += new System.EventHandler(this.ui_view_tracklogs);
        // 
        // ui_simtime_label
        // 
        this.ui_simtime_label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_simtime_label.Location = new Point(3, 94);
        this.ui_simtime_label.Name = "ui_simtime_label";
        this.ui_simtime_label.Size = new Size(113, 26);
        this.ui_simtime_label.TabIndex = 12;
        this.ui_simtime_label.Text = "Sim Time:";
        this.ui_simtime_label.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_recording_time
        // 
        this.ui_recording_time.BackColor = Color.Transparent;
        this.ui_recording_time.Font = new Font("Lucida Console", 12F, FontStyle.Bold);
        this.ui_recording_time.Location = new Point(250, 28);
        this.ui_recording_time.MinimumSize = new Size(50, 24);
        this.ui_recording_time.Name = "ui_recording_time";
        this.ui_recording_time.Size = new Size(131, 24);
        this.ui_recording_time.TabIndex = 11;
        this.ui_recording_time.Text = "00:00:00";
        // 
        // ui_sim_rate
        // 
        this.ui_sim_rate.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_sim_rate.Location = new Point(388, 28);
        this.ui_sim_rate.Name = "ui_sim_rate";
        this.ui_sim_rate.Size = new Size(130, 24);
        this.ui_sim_rate.TabIndex = 10;
        this.ui_sim_rate.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_task
        // 
        this.ui_task.AllowDrop = true;
        this.ui_task.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_task.Location = new Point(125, 148);
        this.ui_task.Name = "ui_task";
        this.ui_task.Size = new Size(393, 26);
        this.ui_task.TabIndex = 9;
        this.ui_task.TextAlign = ContentAlignment.MiddleLeft;
        this.ui_task.AllowDrop = true;
        this.ui_task.DragDrop += new DragEventHandler(this.ui_task_DragDrop);
        this.ui_task.DragEnter += new DragEventHandler(this.ui_task_DragEnter);
        // 
        // ui_aircraft
        // 
        this.ui_aircraft.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_aircraft.Location = new Point(125, 122);
        this.ui_aircraft.Name = "ui_aircraft";
        this.ui_aircraft.Size = new Size(393, 26);
        this.ui_aircraft.TabIndex = 8;
        this.ui_aircraft.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // ui_pilot
        // 
        this.ui_pilot.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_pilot.Location = new Point(125, 67);
        this.ui_pilot.Name = "ui_pilot";
        this.ui_pilot.Size = new Size(393, 26);
        this.ui_pilot.TabIndex = 7;
        this.ui_pilot.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // ui_aircraft_label
        // 
        this.ui_aircraft_label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_aircraft_label.Location = new Point(3, 122);
        this.ui_aircraft_label.Name = "ui_aircraft_label";
        this.ui_aircraft_label.Size = new Size(113, 26);
        this.ui_aircraft_label.TabIndex = 6;
        this.ui_aircraft_label.Text = "Aircraft:";
        this.ui_aircraft_label.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_pilot_label
        // 
        this.ui_pilot_label.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_pilot_label.Location = new Point(3, 67);
        this.ui_pilot_label.Name = "ui_pilot_label";
        this.ui_pilot_label.Size = new Size(113, 26);
        this.ui_pilot_label.TabIndex = 5;
        this.ui_pilot_label.Text = "Pilot:";
        this.ui_pilot_label.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_local_time
        // 
        this.ui_local_time.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_local_time.Location = new Point(125, 94);
        this.ui_local_time.Name = "ui_local_time";
        this.ui_local_time.Size = new Size(180, 26);
        this.ui_local_time.TabIndex = 4;
        this.ui_local_time.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // ui_local_date
        // 
        this.ui_local_date.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_local_date.Location = new Point(311, 94);
        this.ui_local_date.Name = "ui_local_date";
        this.ui_local_date.Size = new Size(207, 26);
        this.ui_local_date.TabIndex = 3;
        this.ui_local_date.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // ui_conn_status
        // 
        this.ui_conn_status.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.ui_conn_status.Location = new Point(125, 28);
        this.ui_conn_status.Name = "ui_conn_status";
        this.ui_conn_status.Size = new Size(393, 24);
        this.ui_conn_status.TabIndex = 2;
        this.ui_conn_status.Text = "Waiting for MSFS.";
        this.ui_conn_status.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pictureBox_statusImage
        // 
        this.pictureBox_statusImage.Location = new Point(22, 12);
        this.pictureBox_statusImage.Name = "pictureBox_statusImage";
        this.pictureBox_statusImage.Size = new Size(96, 51);
        this.pictureBox_statusImage.SizeMode = PictureBoxSizeMode.StretchImage;
        this.pictureBox_statusImage.TabIndex = 1;
        this.pictureBox_statusImage.TabStop = false;
        // 
        // pln_drop_outline
        // 
        this.pln_drop_outline.AllowDrop = true;
        this.pln_drop_outline.BackColor = Color.Transparent;
        this.pln_drop_outline.Location = new Point(6, 144);
        this.pln_drop_outline.Name = "pln_drop_outline";
        this.pln_drop_outline.Size = new Size(514, 42);
        this.pln_drop_outline.TabIndex = 18;
        this.pln_drop_outline.DragDrop += new DragEventHandler(this.ui_task_DragDrop);
        this.pln_drop_outline.DragEnter += new DragEventHandler(this.ui_task_DragEnter);
        // 
        // tabPage_Settings
        // 
        this.tabPage_Settings.BackColor = Color.LightYellow;
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
        this.tabPage_Settings.Location = new Point(64, 4);
        this.tabPage_Settings.Margin = new Padding(3, 2, 3, 2);
        this.tabPage_Settings.Name = "tabPage_Settings";
        this.tabPage_Settings.Padding = new Padding(3, 2, 3, 2);
        this.tabPage_Settings.Size = new Size(524, 236);
        this.tabPage_Settings.TabIndex = 1;
        // 
        // ui_web_urls
        // 
        this.ui_web_urls.AutoSize = true;
        this.ui_web_urls.Location = new Point(15, 200);
        this.ui_web_urls.Name = "ui_web_urls";
        this.ui_web_urls.Size = new Size(0, 15);
        this.ui_web_urls.TabIndex = 9;
        this.ui_web_urls.LinkClicked += new LinkLabelLinkClickedEventHandler(this.ui_web_urls_LinkClicked);
        // 
        // label3
        // 
        this.label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.label3.Location = new Point(15, 123);
        this.label3.Name = "label3";
        this.label3.Size = new Size(145, 23);
        this.label3.TabIndex = 8;
        this.label3.Text = "Tracklogs folder:";
        this.label3.TextAlign = ContentAlignment.MiddleRight;
        // 
        // button1
        // 
        this.button1.Location = new Point(411, 122);
        this.button1.Name = "button1";
        this.button1.Size = new Size(75, 23);
        this.button1.TabIndex = 7;
        this.button1.Text = "Browse";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.ui_browse_tracklogs);
        // 
        // ui_settings_igc_path
        // 
        this.ui_settings_igc_path.Location = new Point(166, 122);
        this.ui_settings_igc_path.Name = "ui_settings_igc_path";
        this.ui_settings_igc_path.Size = new Size(239, 23);
        this.ui_settings_igc_path.TabIndex = 6;
        this.ui_settings_igc_path.Leave += new System.EventHandler(this.ui_settings_igc_path_Leave);
        // 
        // label2
        // 
        this.label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.label2.Location = new Point(15, 76);
        this.label2.Name = "label2";
        this.label2.Size = new Size(145, 23);
        this.label2.TabIndex = 5;
        this.label2.Text = "Pilot ID:";
        this.label2.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_settings_pilot_id
        // 
        this.ui_settings_pilot_id.Location = new Point(166, 75);
        this.ui_settings_pilot_id.Name = "ui_settings_pilot_id";
        this.ui_settings_pilot_id.Size = new Size(239, 23);
        this.ui_settings_pilot_id.TabIndex = 4;
        this.ui_settings_pilot_id.Leave += new System.EventHandler(this.ui_settings_pilot_id_Leave);
        // 
        // label1
        // 
        this.label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.label1.Location = new Point(15, 29);
        this.label1.Name = "label1";
        this.label1.Size = new Size(145, 23);
        this.label1.TabIndex = 3;
        this.label1.Text = "Pilot name:";
        this.label1.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_settings_pilot_name
        // 
        this.ui_settings_pilot_name.Location = new Point(166, 28);
        this.ui_settings_pilot_name.Name = "ui_settings_pilot_name";
        this.ui_settings_pilot_name.Size = new Size(239, 23);
        this.ui_settings_pilot_name.TabIndex = 2;
        this.ui_settings_pilot_name.Leave += new System.EventHandler(this.ui_settings_pilot_name_Leave);
        // 
        // ui_auto_start_label1
        // 
        this.ui_auto_start_label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.ui_auto_start_label1.Location = new Point(15, 166);
        this.ui_auto_start_label1.Name = "ui_auto_start_label1";
        this.ui_auto_start_label1.Size = new Size(145, 23);
        this.ui_auto_start_label1.TabIndex = 1;
        this.ui_auto_start_label1.Text = "Auto start:";
        this.ui_auto_start_label1.TextAlign = ContentAlignment.MiddleRight;
        // 
        // ui_auto_start_checkbox
        // 
        this.ui_auto_start_checkbox.AutoSize = true;
        this.ui_auto_start_checkbox.Location = new Point(166, 170);
        this.ui_auto_start_checkbox.Name = "ui_auto_start_checkbox";
        this.ui_auto_start_checkbox.Size = new Size(63, 19);
        this.ui_auto_start_checkbox.TabIndex = 0;
        this.ui_auto_start_checkbox.Text = "Enable";
        this.ui_auto_start_checkbox.UseVisualStyleBackColor = true;
        this.ui_auto_start_checkbox.CheckedChanged += new System.EventHandler(this.ui_auto_start_checkbox_CheckedChanged);
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(593, 240);
        this.Controls.Add(this.tabControl1);
        this.Font = new Font("Segoe UI", 9F);
        this.Icon = Resources.app_icon;
        this.Margin = new Padding(3, 2, 3, 2);
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.Text = "NB21 Logger";
        this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
        this.Load += new System.EventHandler(this.MainForm_Load);
        this.tabControl1.ResumeLayout(false);
        this.tabPage_Home.ResumeLayout(false);
        this.tabPage_Home.PerformLayout();
        ((ISupportInitialize)(this.ui_file_icon)).EndInit();
        ((ISupportInitialize)(this.pictureBox_statusImage)).EndInit();
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
