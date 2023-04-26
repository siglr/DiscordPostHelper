Imports System.Configuration
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

Public Class DPHXUnpackAndLoad

    Private ReadOnly _SF As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _currentFile As String = String.Empty

    Public Sub SetFormCaption(filename As String)

        If filename = String.Empty Then
            filename = "No DPHX package loaded"
        End If

        'Add version to form title
        Me.Text = $"DPHX Unpack and Load v{Me.GetType.Assembly.GetName.Version} - {filename}"

    End Sub

    Private Sub DPHXUnpackAndLoad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Settings.SessionSettings.Load()

        SetFormCaption(_currentFile)

        RestoreMainFormLocationAndSize()

    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click

        Dim formSettings As New Settings

        If formSettings.ShowDialog() = DialogResult.OK Then
            'Settings may have changed
        End If

    End Sub

    Private Sub LoadDPHX_Click(sender As Object, e As EventArgs) Handles LoadDPHX.Click

        If txtPackageName.Text = String.Empty Then
            If Directory.Exists(Settings.SessionSettings.PackagesFolder) Then
                OpenFileDialog1.InitialDirectory = Settings.SessionSettings.PackagesFolder
            Else
                OpenFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            End If
        Else
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(txtPackageName.Text)
        End If

        OpenFileDialog1.FileName = String.Empty
        OpenFileDialog1.Title = "Select DPHX package file to load"
        OpenFileDialog1.Filter = "Discord Post Helper Pacakge|*.dphx"
        OpenFileDialog1.Multiselect = False

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = DialogResult.OK Then
            LoadDPHXPackage(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Function TempDPHXUnpackFolder() As String
        Return Path.Combine(Settings.SessionSettings.UnpackingFolder, "TempDPHXUnpack")
    End Function

    Private Sub LoadDPHXPackage(dphxFilename As String)

        Dim newDPHFile As String

        newDPHFile = _SF.UnpackDPHXFileToTempFolder(dphxFilename, TempDPHXUnpackFolder)

        If newDPHFile = String.Empty Then
            'Invalid file loaded
            txtPackageName.Text = String.Empty
            _currentFile = String.Empty
            DisableUnpackButton()
        Else
            txtPackageName.Text = dphxFilename
            _currentFile = dphxFilename
            txtDPHFilename.Text = newDPHFile
            EnableUnpackButton()
        End If
        SetFormCaption(_currentFile)

        If File.Exists(newDPHFile) Then
            Dim serializer As New XmlSerializer(GetType(AllData))
            Dim allCurrentData As AllData

            On Error Resume Next

            Using stream As New FileStream(newDPHFile, FileMode.Open)
                allCurrentData = CType(serializer.Deserialize(stream), AllData)
            End Using

            ctrlBriefing.GenerateBriefing(_SF, allCurrentData, TempDPHXUnpackFolder)
        End If

    End Sub

    Private Sub DisableUnpackButton()
        btnCopyFiles.Enabled = False
        pnlUnpackBtn.BackColor = SystemColors.Control
        btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Regular)
    End Sub

    Private Sub EnableUnpackButton()
        btnCopyFiles.Enabled = True
        pnlUnpackBtn.BackColor = Color.Red
        btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Bold)
    End Sub

    Private Sub DPHXUnpackAndLoad_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)

        If Not config.AppSettings.Settings.AllKeys.Contains("MainFormSize") Then
            config.AppSettings.Settings.Add("MainFormSize", Me.Size.ToString())
        Else
            config.AppSettings.Settings("MainFormSize").Value = Me.Size.ToString()
        End If

        If Not config.AppSettings.Settings.AllKeys.Contains("MainFormLocation") Then
            config.AppSettings.Settings.Add("MainFormLocation", Me.Location.ToString())
        Else
            config.AppSettings.Settings("MainFormLocation").Value = Me.Location.ToString()
        End If

        config.Save(ConfigurationSaveMode.Modified)
        ConfigurationManager.RefreshSection("appSettings")
    End Sub

    Private Sub RestoreMainFormLocationAndSize()
        Dim sizeString As String = ConfigurationManager.AppSettings("MainFormSize")
        Dim locationString As String = ConfigurationManager.AppSettings("MainFormLocation")

        If sizeString <> "" Then
            Dim sizeArray As String() = sizeString.TrimStart("{").TrimEnd("}").Split(",")
            Dim width As Integer = CInt(sizeArray(0).Split("=")(1))
            Dim height As Integer = CInt(sizeArray(1).Split("=")(1))
            Me.Size = New Size(width, height)
        End If

        If locationString <> "" Then
            Dim locationArray As String() = locationString.TrimStart("{").TrimEnd("}").Split(",")
            Dim x As Integer = CInt(locationArray(0).Split("=")(1))
            Dim y As Integer = CInt(locationArray(1).Split("=")(1))
            Me.Location = New Point(x, y)
        End If
    End Sub
End Class
