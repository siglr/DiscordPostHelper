<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class NewsManagement
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
        Me.cmbAction = New System.Windows.Forms.ComboBox()
        Me.txtTaskID = New System.Windows.Forms.TextBox()
        Me.txtKey = New System.Windows.Forms.TextBox()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.txtSubtitle = New System.Windows.Forms.TextBox()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.txtCredits = New System.Windows.Forms.TextBox()
        Me.txtEventDate = New System.Windows.Forms.TextBox()
        Me.txtPublished = New System.Windows.Forms.TextBox()
        Me.txtNews = New System.Windows.Forms.TextBox()
        Me.txtEntrySeqID = New System.Windows.Forms.TextBox()
        Me.txtURLToGo = New System.Windows.Forms.TextBox()
        Me.txtExpiration = New System.Windows.Forms.TextBox()
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.lblAction = New System.Windows.Forms.Label()
        Me.lblTaskID = New System.Windows.Forms.Label()
        Me.lblKey = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.lblComments = New System.Windows.Forms.Label()
        Me.lblCredits = New System.Windows.Forms.Label()
        Me.lblEventDate = New System.Windows.Forms.Label()
        Me.lblPublished = New System.Windows.Forms.Label()
        Me.lblNews = New System.Windows.Forms.Label()
        Me.lblEntrySeqID = New System.Windows.Forms.Label()
        Me.lblURLToGo = New System.Windows.Forms.Label()
        Me.lblExpiration = New System.Windows.Forms.Label()
        Me.lstNewsEntries = New System.Windows.Forms.ListBox()
        Me.btnFetchNews = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmbAction
        '
        Me.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAction.FormattingEnabled = True
        Me.cmbAction.Items.AddRange(New Object() {"CreateTask", "UpdateTask", "DeleteTask", "CreateEvent", "DeleteEvent", "CreateNews", "DeleteNews"})
        Me.cmbAction.Location = New System.Drawing.Point(503, 16)
        Me.cmbAction.Name = "cmbAction"
        Me.cmbAction.Size = New System.Drawing.Size(388, 26)
        Me.cmbAction.TabIndex = 0
        '
        'txtTaskID
        '
        Me.txtTaskID.Location = New System.Drawing.Point(503, 56)
        Me.txtTaskID.Name = "txtTaskID"
        Me.txtTaskID.Size = New System.Drawing.Size(388, 25)
        Me.txtTaskID.TabIndex = 1
        '
        'txtKey
        '
        Me.txtKey.Location = New System.Drawing.Point(503, 96)
        Me.txtKey.Name = "txtKey"
        Me.txtKey.Size = New System.Drawing.Size(388, 25)
        Me.txtKey.TabIndex = 2
        '
        'txtTitle
        '
        Me.txtTitle.Location = New System.Drawing.Point(503, 136)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(388, 25)
        Me.txtTitle.TabIndex = 3
        '
        'txtSubtitle
        '
        Me.txtSubtitle.Location = New System.Drawing.Point(503, 176)
        Me.txtSubtitle.Name = "txtSubtitle"
        Me.txtSubtitle.Size = New System.Drawing.Size(388, 25)
        Me.txtSubtitle.TabIndex = 4
        '
        'txtComments
        '
        Me.txtComments.Location = New System.Drawing.Point(503, 216)
        Me.txtComments.Name = "txtComments"
        Me.txtComments.Size = New System.Drawing.Size(388, 25)
        Me.txtComments.TabIndex = 5
        '
        'txtCredits
        '
        Me.txtCredits.Location = New System.Drawing.Point(503, 256)
        Me.txtCredits.Name = "txtCredits"
        Me.txtCredits.Size = New System.Drawing.Size(388, 25)
        Me.txtCredits.TabIndex = 6
        '
        'txtEventDate
        '
        Me.txtEventDate.Location = New System.Drawing.Point(503, 296)
        Me.txtEventDate.Name = "txtEventDate"
        Me.txtEventDate.Size = New System.Drawing.Size(388, 25)
        Me.txtEventDate.TabIndex = 7
        '
        'txtPublished
        '
        Me.txtPublished.Location = New System.Drawing.Point(503, 336)
        Me.txtPublished.Name = "txtPublished"
        Me.txtPublished.Size = New System.Drawing.Size(388, 25)
        Me.txtPublished.TabIndex = 8
        '
        'txtNews
        '
        Me.txtNews.Location = New System.Drawing.Point(503, 376)
        Me.txtNews.Name = "txtNews"
        Me.txtNews.Size = New System.Drawing.Size(388, 25)
        Me.txtNews.TabIndex = 9
        '
        'txtEntrySeqID
        '
        Me.txtEntrySeqID.Location = New System.Drawing.Point(503, 416)
        Me.txtEntrySeqID.Name = "txtEntrySeqID"
        Me.txtEntrySeqID.Size = New System.Drawing.Size(388, 25)
        Me.txtEntrySeqID.TabIndex = 10
        '
        'txtURLToGo
        '
        Me.txtURLToGo.Location = New System.Drawing.Point(503, 456)
        Me.txtURLToGo.Name = "txtURLToGo"
        Me.txtURLToGo.Size = New System.Drawing.Size(388, 25)
        Me.txtURLToGo.TabIndex = 11
        '
        'txtExpiration
        '
        Me.txtExpiration.Location = New System.Drawing.Point(503, 496)
        Me.txtExpiration.Name = "txtExpiration"
        Me.txtExpiration.Size = New System.Drawing.Size(388, 25)
        Me.txtExpiration.TabIndex = 12
        '
        'btnSubmit
        '
        Me.btnSubmit.Location = New System.Drawing.Point(503, 536)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(85, 30)
        Me.btnSubmit.TabIndex = 13
        Me.btnSubmit.Text = "Submit"
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'lblAction
        '
        Me.lblAction.Location = New System.Drawing.Point(386, 19)
        Me.lblAction.Name = "lblAction"
        Me.lblAction.Size = New System.Drawing.Size(100, 23)
        Me.lblAction.TabIndex = 14
        Me.lblAction.Text = "Action"
        '
        'lblTaskID
        '
        Me.lblTaskID.Location = New System.Drawing.Point(386, 59)
        Me.lblTaskID.Name = "lblTaskID"
        Me.lblTaskID.Size = New System.Drawing.Size(100, 23)
        Me.lblTaskID.TabIndex = 15
        Me.lblTaskID.Text = "Task ID"
        '
        'lblKey
        '
        Me.lblKey.Location = New System.Drawing.Point(386, 99)
        Me.lblKey.Name = "lblKey"
        Me.lblKey.Size = New System.Drawing.Size(100, 23)
        Me.lblKey.TabIndex = 16
        Me.lblKey.Text = "Key"
        '
        'lblTitle
        '
        Me.lblTitle.Location = New System.Drawing.Point(386, 139)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(100, 23)
        Me.lblTitle.TabIndex = 17
        Me.lblTitle.Text = "Title"
        '
        'lblSubtitle
        '
        Me.lblSubtitle.Location = New System.Drawing.Point(386, 179)
        Me.lblSubtitle.Name = "lblSubtitle"
        Me.lblSubtitle.Size = New System.Drawing.Size(100, 23)
        Me.lblSubtitle.TabIndex = 18
        Me.lblSubtitle.Text = "Subtitle"
        '
        'lblComments
        '
        Me.lblComments.Location = New System.Drawing.Point(386, 219)
        Me.lblComments.Name = "lblComments"
        Me.lblComments.Size = New System.Drawing.Size(100, 23)
        Me.lblComments.TabIndex = 19
        Me.lblComments.Text = "Comments"
        '
        'lblCredits
        '
        Me.lblCredits.Location = New System.Drawing.Point(386, 259)
        Me.lblCredits.Name = "lblCredits"
        Me.lblCredits.Size = New System.Drawing.Size(100, 23)
        Me.lblCredits.TabIndex = 20
        Me.lblCredits.Text = "Credits"
        '
        'lblEventDate
        '
        Me.lblEventDate.Location = New System.Drawing.Point(386, 299)
        Me.lblEventDate.Name = "lblEventDate"
        Me.lblEventDate.Size = New System.Drawing.Size(100, 23)
        Me.lblEventDate.TabIndex = 21
        Me.lblEventDate.Text = "Event Date"
        '
        'lblPublished
        '
        Me.lblPublished.Location = New System.Drawing.Point(386, 339)
        Me.lblPublished.Name = "lblPublished"
        Me.lblPublished.Size = New System.Drawing.Size(100, 23)
        Me.lblPublished.TabIndex = 22
        Me.lblPublished.Text = "Published"
        '
        'lblNews
        '
        Me.lblNews.Location = New System.Drawing.Point(386, 379)
        Me.lblNews.Name = "lblNews"
        Me.lblNews.Size = New System.Drawing.Size(100, 23)
        Me.lblNews.TabIndex = 23
        Me.lblNews.Text = "News"
        '
        'lblEntrySeqID
        '
        Me.lblEntrySeqID.Location = New System.Drawing.Point(386, 419)
        Me.lblEntrySeqID.Name = "lblEntrySeqID"
        Me.lblEntrySeqID.Size = New System.Drawing.Size(100, 23)
        Me.lblEntrySeqID.TabIndex = 24
        Me.lblEntrySeqID.Text = "Entry Seq ID"
        '
        'lblURLToGo
        '
        Me.lblURLToGo.Location = New System.Drawing.Point(386, 459)
        Me.lblURLToGo.Name = "lblURLToGo"
        Me.lblURLToGo.Size = New System.Drawing.Size(100, 23)
        Me.lblURLToGo.TabIndex = 25
        Me.lblURLToGo.Text = "URL To Go"
        '
        'lblExpiration
        '
        Me.lblExpiration.Location = New System.Drawing.Point(386, 499)
        Me.lblExpiration.Name = "lblExpiration"
        Me.lblExpiration.Size = New System.Drawing.Size(100, 23)
        Me.lblExpiration.TabIndex = 26
        Me.lblExpiration.Text = "Expiration"
        '
        'lstNewsEntries
        '
        Me.lstNewsEntries.FormattingEnabled = True
        Me.lstNewsEntries.ItemHeight = 18
        Me.lstNewsEntries.Location = New System.Drawing.Point(12, 38)
        Me.lstNewsEntries.Name = "lstNewsEntries"
        Me.lstNewsEntries.Size = New System.Drawing.Size(358, 490)
        Me.lstNewsEntries.TabIndex = 27
        '
        'btnFetchNews
        '
        Me.btnFetchNews.Location = New System.Drawing.Point(13, 5)
        Me.btnFetchNews.Name = "btnFetchNews"
        Me.btnFetchNews.Size = New System.Drawing.Size(174, 31)
        Me.btnFetchNews.TabIndex = 28
        Me.btnFetchNews.Text = "Update"
        Me.btnFetchNews.UseVisualStyleBackColor = True
        '
        'NewsManagement
        '
        Me.ClientSize = New System.Drawing.Size(920, 580)
        Me.Controls.Add(Me.btnFetchNews)
        Me.Controls.Add(Me.lstNewsEntries)
        Me.Controls.Add(Me.cmbAction)
        Me.Controls.Add(Me.txtTaskID)
        Me.Controls.Add(Me.txtKey)
        Me.Controls.Add(Me.txtTitle)
        Me.Controls.Add(Me.txtSubtitle)
        Me.Controls.Add(Me.txtComments)
        Me.Controls.Add(Me.txtCredits)
        Me.Controls.Add(Me.txtEventDate)
        Me.Controls.Add(Me.txtPublished)
        Me.Controls.Add(Me.txtNews)
        Me.Controls.Add(Me.txtEntrySeqID)
        Me.Controls.Add(Me.txtURLToGo)
        Me.Controls.Add(Me.txtExpiration)
        Me.Controls.Add(Me.btnSubmit)
        Me.Controls.Add(Me.lblAction)
        Me.Controls.Add(Me.lblTaskID)
        Me.Controls.Add(Me.lblKey)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblSubtitle)
        Me.Controls.Add(Me.lblComments)
        Me.Controls.Add(Me.lblCredits)
        Me.Controls.Add(Me.lblEventDate)
        Me.Controls.Add(Me.lblPublished)
        Me.Controls.Add(Me.lblNews)
        Me.Controls.Add(Me.lblEntrySeqID)
        Me.Controls.Add(Me.lblURLToGo)
        Me.Controls.Add(Me.lblExpiration)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.Name = "NewsManagement"
        Me.Text = "Task Manager"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents cmbAction As ComboBox
    Private WithEvents txtTaskID As TextBox
    Private WithEvents txtKey As TextBox
    Private WithEvents txtTitle As TextBox
    Private WithEvents txtSubtitle As TextBox
    Private WithEvents txtComments As TextBox
    Private WithEvents txtCredits As TextBox
    Private WithEvents txtEventDate As TextBox
    Private WithEvents txtPublished As TextBox
    Private WithEvents txtNews As TextBox
    Private WithEvents txtEntrySeqID As TextBox
    Private WithEvents txtURLToGo As TextBox
    Private WithEvents txtExpiration As TextBox
    Private WithEvents btnSubmit As Button
    Friend WithEvents lblAction As Label
    Friend WithEvents lblTaskID As Label
    Friend WithEvents lblKey As Label
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubtitle As Label
    Friend WithEvents lblComments As Label
    Friend WithEvents lblCredits As Label
    Friend WithEvents lblEventDate As Label
    Friend WithEvents lblPublished As Label
    Friend WithEvents lblNews As Label
    Friend WithEvents lblEntrySeqID As Label
    Friend WithEvents lblURLToGo As Label
    Friend WithEvents lblExpiration As Label
    Friend WithEvents lstNewsEntries As ListBox
    Friend WithEvents btnFetchNews As Button
End Class
