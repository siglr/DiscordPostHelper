<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TaskVersionSelector
    Inherits ZoomForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Me.dgvCandidates = New System.Windows.Forms.DataGridView()
        Me.colEntrySeqId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTitle = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colSimTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWeather = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        CType(Me.dgvCandidates, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvCandidates
        '
        Me.dgvCandidates.AllowUserToAddRows = False
        Me.dgvCandidates.AllowUserToDeleteRows = False
        Me.dgvCandidates.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvCandidates.AutoGenerateColumns = False
        Me.dgvCandidates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.dgvCandidates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCandidates.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colEntrySeqId, Me.colTitle, Me.colSimTime, Me.colWeather})
        Me.dgvCandidates.Location = New System.Drawing.Point(10, 10)
        Me.dgvCandidates.MultiSelect = False
        Me.dgvCandidates.Name = "dgvCandidates"
        Me.dgvCandidates.ReadOnly = True
        Me.dgvCandidates.RowHeadersVisible = False
        Me.dgvCandidates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvCandidates.Size = New System.Drawing.Size(760, 300)
        Me.dgvCandidates.TabIndex = 0
        '
        'colEntrySeqId
        '
        Me.colEntrySeqId.DataPropertyName = "EntrySeqID"
        Me.colEntrySeqId.HeaderText = "EntrySeqID"
        Me.colEntrySeqId.MinimumWidth = 6
        Me.colEntrySeqId.Name = "colEntrySeqId"
        Me.colEntrySeqId.ReadOnly = True
        Me.colEntrySeqId.Width = 98
        '
        'colTitle
        '
        Me.colTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.colTitle.DataPropertyName = "Title"
        Me.colTitle.HeaderText = "Title"
        Me.colTitle.MinimumWidth = 6
        Me.colTitle.Name = "colTitle"
        Me.colTitle.ReadOnly = True
        '
        'colSimTime
        '
        Me.colSimTime.DataPropertyName = "SimTime"
        Me.colSimTime.HeaderText = "Sim Local Date/Time"
        Me.colSimTime.MinimumWidth = 6
        Me.colSimTime.Name = "colSimTime"
        Me.colSimTime.ReadOnly = True
        Me.colSimTime.Width = 179
        '
        'colWeather
        '
        Me.colWeather.DataPropertyName = "Weather"
        Me.colWeather.HeaderText = "Weather Preset"
        Me.colWeather.MinimumWidth = 6
        Me.colWeather.Name = "colWeather"
        Me.colWeather.ReadOnly = True
        Me.colWeather.Width = 143
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOk.Location = New System.Drawing.Point(600, 320)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(80, 30)
        Me.btnOk.TabIndex = 1
        Me.btnOk.Text = "OK"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(690, 320)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'TaskVersionSelector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(800, 360)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.dgvCandidates)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TaskVersionSelector"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Select Task Version"
        CType(Me.dgvCandidates, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents dgvCandidates As System.Windows.Forms.DataGridView
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents colEntrySeqId As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colTitle As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colSimTime As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colWeather As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
