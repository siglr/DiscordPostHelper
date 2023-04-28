namespace SIGLR.SoaringTools.ImageViewer
{
  public partial class ImageViewerControl
    {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.zoomLevel = new System.Windows.Forms.TrackBar();
            this.chkFitToWindow = new System.Windows.Forms.CheckBox();
            this.imageBox = new SIGLR.SoaringTools.ImageViewer.ImageBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.positionToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageSizeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.zoomToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageFilenameStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.showImageRegionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.showSourceImageRegionToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomLevel)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.imageBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
            this.splitContainer1.Panel2Collapsed = true;
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(868, 594);
            this.splitContainer1.SplitterDistance = 587;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.zoomLevel);
            this.panel1.Controls.Add(this.chkFitToWindow);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(868, 39);
            this.panel1.TabIndex = 1;
            // 
            // zoomLevel
            // 
            this.zoomLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomLevel.LargeChange = 20;
            this.zoomLevel.Location = new System.Drawing.Point(176, 9);
            this.zoomLevel.Maximum = 500;
            this.zoomLevel.Minimum = 10;
            this.zoomLevel.Name = "zoomLevel";
            this.zoomLevel.Size = new System.Drawing.Size(675, 50);
            this.zoomLevel.SmallChange = 5;
            this.zoomLevel.TabIndex = 2;
            this.zoomLevel.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zoomLevel.Value = 10;
            this.zoomLevel.ValueChanged += new System.EventHandler(this.zoomLevel_ValueChanged);
            // 
            // chkFitToWindow
            // 
            this.chkFitToWindow.AutoSize = true;
            this.chkFitToWindow.Location = new System.Drawing.Point(12, 7);
            this.chkFitToWindow.Name = "chkFitToWindow";
            this.chkFitToWindow.Size = new System.Drawing.Size(144, 24);
            this.chkFitToWindow.TabIndex = 0;
            this.chkFitToWindow.Text = "Fit to window size";
            this.toolTip1.SetToolTip(this.chkFitToWindow, "Enable to fit the image to the available window size");
            this.chkFitToWindow.UseVisualStyleBackColor = true;
            this.chkFitToWindow.CheckedChanged += new System.EventHandler(this.chkFitToWindow_CheckedChanged);
            // 
            // imageBox
            // 
            this.imageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageBox.AutoScroll = true;
            this.imageBox.AutoSize = false;
            this.imageBox.GridDisplayMode = SIGLR.SoaringTools.ImageViewer.ImageBoxGridDisplayMode.None;
            this.imageBox.Location = new System.Drawing.Point(0, 39);
            this.imageBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(868, 555);
            this.imageBox.TabIndex = 0;
            this.imageBox.SizeToFitChanged += new System.EventHandler(this.imageBox_SizeToFitChanged);
            this.imageBox.ZoomChanged += new System.EventHandler(this.imageBox_ZoomChanged);
            this.imageBox.Scroll += new System.Windows.Forms.ScrollEventHandler(this.imageBox_Scroll);
            this.imageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imageBox_Paint);
            this.imageBox.Resize += new System.EventHandler(this.imageBox_Resize);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(175, 198);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.SelectedObject = this.imageBox;
            this.propertyGrid.Size = new System.Drawing.Size(204, 349);
            this.propertyGrid.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.positionToolStripStatusLabel,
            this.imageSizeToolStripStatusLabel,
            this.zoomToolStripStatusLabel,
            this.imageFilenameStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 594);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(868, 27);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 1;
            // 
            // positionToolStripStatusLabel
            // 
            this.positionToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.positionToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.positionToolStripStatusLabel.Image = global::SIGLR.SoaringTools.ImageViewer.Properties.Resources.Object_Position;
            this.positionToolStripStatusLabel.Name = "positionToolStripStatusLabel";
            this.positionToolStripStatusLabel.Size = new System.Drawing.Size(22, 22);
            this.positionToolStripStatusLabel.Visible = false;
            // 
            // imageSizeToolStripStatusLabel
            // 
            this.imageSizeToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.imageSizeToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.imageSizeToolStripStatusLabel.Image = global::SIGLR.SoaringTools.ImageViewer.Properties.Resources.Object_Size;
            this.imageSizeToolStripStatusLabel.Name = "imageSizeToolStripStatusLabel";
            this.imageSizeToolStripStatusLabel.Size = new System.Drawing.Size(22, 22);
            this.imageSizeToolStripStatusLabel.Visible = false;
            // 
            // zoomToolStripStatusLabel
            // 
            this.zoomToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.zoomToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.zoomToolStripStatusLabel.Image = global::SIGLR.SoaringTools.ImageViewer.Properties.Resources.magnifier_zoom;
            this.zoomToolStripStatusLabel.Name = "zoomToolStripStatusLabel";
            this.zoomToolStripStatusLabel.Size = new System.Drawing.Size(22, 22);
            // 
            // imageFilenameStatusLabel
            // 
            this.imageFilenameStatusLabel.Name = "imageFilenameStatusLabel";
            this.imageFilenameStatusLabel.Size = new System.Drawing.Size(113, 22);
            this.imageFilenameStatusLabel.Text = "No image loaded";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showImageRegionToolStripButton,
            this.showSourceImageRegionToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(868, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // showImageRegionToolStripButton
            // 
            this.showImageRegionToolStripButton.CheckOnClick = true;
            this.showImageRegionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showImageRegionToolStripButton.Image = global::SIGLR.SoaringTools.ImageViewer.Properties.Resources.zone;
            this.showImageRegionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showImageRegionToolStripButton.Name = "showImageRegionToolStripButton";
            this.showImageRegionToolStripButton.Size = new System.Drawing.Size(26, 22);
            this.showImageRegionToolStripButton.Text = "Show Image Region";
            this.showImageRegionToolStripButton.Click += new System.EventHandler(this.showImageRegionToolStripButton_Click);
            // 
            // showSourceImageRegionToolStripButton
            // 
            this.showSourceImageRegionToolStripButton.CheckOnClick = true;
            this.showSourceImageRegionToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showSourceImageRegionToolStripButton.Image = global::SIGLR.SoaringTools.ImageViewer.Properties.Resources.zone;
            this.showSourceImageRegionToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showSourceImageRegionToolStripButton.Name = "showSourceImageRegionToolStripButton";
            this.showSourceImageRegionToolStripButton.Size = new System.Drawing.Size(26, 22);
            this.showSourceImageRegionToolStripButton.Text = "Show Source Image Region";
            this.showSourceImageRegionToolStripButton.Click += new System.EventHandler(this.showImageRegionToolStripButton_Click);
            // 
            // ImageViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI Variable Display", 9.818182F);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ImageViewerControl";
            this.Size = new System.Drawing.Size(868, 621);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomLevel)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.PropertyGrid propertyGrid;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel positionToolStripStatusLabel;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton showImageRegionToolStripButton;
    private System.Windows.Forms.ToolStripButton showSourceImageRegionToolStripButton;
    private System.Windows.Forms.ToolStripStatusLabel imageSizeToolStripStatusLabel;
    private System.Windows.Forms.ToolStripStatusLabel zoomToolStripStatusLabel;
        private SIGLR.SoaringTools.ImageViewer.ImageBox imageBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkFitToWindow;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TrackBar zoomLevel;
        private System.Windows.Forms.ToolStripStatusLabel imageFilenameStatusLabel;
    }
}

