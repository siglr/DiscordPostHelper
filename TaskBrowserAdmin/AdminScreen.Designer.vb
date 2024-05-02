<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AdminScreen
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
        Me.btnSelectDPHXFiles = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.gridIncomingDPHXFilesData = New System.Windows.Forms.DataGridView()
        Me.gridCurrentDatabase = New System.Windows.Forms.DataGridView()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnSaveDatabase = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.gridIncomingDPHXFilesData, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridCurrentDatabase, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnSelectDPHXFiles
        '
        Me.btnSelectDPHXFiles.Location = New System.Drawing.Point(12, 13)
        Me.btnSelectDPHXFiles.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSelectDPHXFiles.Name = "btnSelectDPHXFiles"
        Me.btnSelectDPHXFiles.Size = New System.Drawing.Size(176, 40)
        Me.btnSelectDPHXFiles.TabIndex = 0
        Me.btnSelectDPHXFiles.Text = "Select DPHX files"
        Me.btnSelectDPHXFiles.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(12, 60)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.gridIncomingDPHXFilesData)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.gridCurrentDatabase)
        Me.SplitContainer1.Size = New System.Drawing.Size(2044, 1128)
        Me.SplitContainer1.SplitterDistance = 564
        Me.SplitContainer1.TabIndex = 2
        '
        'gridIncomingDPHXFilesData
        '
        Me.gridIncomingDPHXFilesData.AllowUserToAddRows = False
        Me.gridIncomingDPHXFilesData.AllowUserToDeleteRows = False
        Me.gridIncomingDPHXFilesData.AllowUserToOrderColumns = True
        Me.gridIncomingDPHXFilesData.AllowUserToResizeRows = False
        Me.gridIncomingDPHXFilesData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gridIncomingDPHXFilesData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridIncomingDPHXFilesData.Location = New System.Drawing.Point(0, 0)
        Me.gridIncomingDPHXFilesData.Name = "gridIncomingDPHXFilesData"
        Me.gridIncomingDPHXFilesData.RowHeadersWidth = 47
        Me.gridIncomingDPHXFilesData.Size = New System.Drawing.Size(2044, 564)
        Me.gridIncomingDPHXFilesData.TabIndex = 2
        '
        'gridCurrentDatabase
        '
        Me.gridCurrentDatabase.AllowUserToAddRows = False
        Me.gridCurrentDatabase.AllowUserToDeleteRows = False
        Me.gridCurrentDatabase.AllowUserToOrderColumns = True
        Me.gridCurrentDatabase.AllowUserToResizeRows = False
        Me.gridCurrentDatabase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.gridCurrentDatabase.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridCurrentDatabase.Location = New System.Drawing.Point(0, 0)
        Me.gridCurrentDatabase.Name = "gridCurrentDatabase"
        Me.gridCurrentDatabase.RowHeadersWidth = 47
        Me.gridCurrentDatabase.Size = New System.Drawing.Size(2044, 560)
        Me.gridCurrentDatabase.TabIndex = 3
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        Me.OpenFileDialog1.Multiselect = True
        '
        'btnSaveDatabase
        '
        Me.btnSaveDatabase.Location = New System.Drawing.Point(287, 13)
        Me.btnSaveDatabase.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSaveDatabase.Name = "btnSaveDatabase"
        Me.btnSaveDatabase.Size = New System.Drawing.Size(343, 40)
        Me.btnSaveDatabase.TabIndex = 3
        Me.btnSaveDatabase.Text = "Add/Update Tasks && Save Database"
        Me.btnSaveDatabase.UseVisualStyleBackColor = True
        '
        'AdminScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(2068, 1200)
        Me.Controls.Add(Me.btnSaveDatabase)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.btnSelectDPHXFiles)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "AdminScreen"
        Me.Text = "Task Browser Admin"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.gridIncomingDPHXFilesData, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridCurrentDatabase, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnSelectDPHXFiles As Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents gridIncomingDPHXFilesData As DataGridView
    Friend WithEvents gridCurrentDatabase As DataGridView
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents btnSaveDatabase As Button
End Class
