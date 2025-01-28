<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class WizardNextChoice
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
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnCreateTask = New System.Windows.Forms.Button()
        Me.btnCreateGroupFlight = New System.Windows.Forms.Button()
        Me.btnPostTask = New System.Windows.Forms.Button()
        Me.btnPostGroupFlight = New System.Windows.Forms.Button()
        Me.btnStopWizard = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.Label1)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCreateTask)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnCreateGroupFlight)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnPostTask)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnPostGroupFlight)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnStopWizard)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(712, 276)
        Me.FlowLayoutPanel1.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(707, 35)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Select where the wizard should guide you next"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnCreateTask
        '
        Me.btnCreateTask.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreateTask.Location = New System.Drawing.Point(3, 38)
        Me.btnCreateTask.Name = "btnCreateTask"
        Me.btnCreateTask.Size = New System.Drawing.Size(707, 41)
        Me.btnCreateTask.TabIndex = 1
        Me.btnCreateTask.Text = "1. I want help creating the task (this is usually the first step)"
        Me.btnCreateTask.UseVisualStyleBackColor = True
        '
        'btnCreateGroupFlight
        '
        Me.btnCreateGroupFlight.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreateGroupFlight.Location = New System.Drawing.Point(3, 85)
        Me.btnCreateGroupFlight.Name = "btnCreateGroupFlight"
        Me.btnCreateGroupFlight.Size = New System.Drawing.Size(707, 41)
        Me.btnCreateGroupFlight.TabIndex = 2
        Me.btnCreateGroupFlight.Text = "2. I want help creating the group flight (only if applicable, select this before " &
    "posting the task)"
        Me.btnCreateGroupFlight.UseVisualStyleBackColor = True
        '
        'btnPostTask
        '
        Me.btnPostTask.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPostTask.Location = New System.Drawing.Point(3, 132)
        Me.btnPostTask.Name = "btnPostTask"
        Me.btnPostTask.Size = New System.Drawing.Size(707, 41)
        Me.btnPostTask.TabIndex = 4
        Me.btnPostTask.Text = "3. I want help to post the task on WSG and Discord (when the previous steps are d" &
    "one)"
        Me.btnPostTask.UseVisualStyleBackColor = True
        '
        'btnPostGroupFlight
        '
        Me.btnPostGroupFlight.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnPostGroupFlight.Location = New System.Drawing.Point(3, 179)
        Me.btnPostGroupFlight.Name = "btnPostGroupFlight"
        Me.btnPostGroupFlight.Size = New System.Drawing.Size(707, 41)
        Me.btnPostGroupFlight.TabIndex = 5
        Me.btnPostGroupFlight.Text = "4. I want help to post the group flight on WSG and Discord (when everything else " &
    "is done)"
        Me.btnPostGroupFlight.UseVisualStyleBackColor = True
        '
        'btnStopWizard
        '
        Me.btnStopWizard.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStopWizard.Location = New System.Drawing.Point(3, 226)
        Me.btnStopWizard.Name = "btnStopWizard"
        Me.btnStopWizard.Size = New System.Drawing.Size(707, 41)
        Me.btnStopWizard.TabIndex = 6
        Me.btnStopWizard.Text = "I'm ok, stop the wizard now"
        Me.btnStopWizard.UseVisualStyleBackColor = True
        '
        'WizardNextChoice
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(712, 276)
        Me.ControlBox = False
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Font = New System.Drawing.Font("Segoe UI Variable Display", 9.818182!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "WizardNextChoice"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "What should you to do next ?"
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents btnCreateTask As Button
    Friend WithEvents btnCreateGroupFlight As Button
    Friend WithEvents btnPostTask As Button
    Friend WithEvents btnPostGroupFlight As Button
    Friend WithEvents btnStopWizard As Button
End Class
