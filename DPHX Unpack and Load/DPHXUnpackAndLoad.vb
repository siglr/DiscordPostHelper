﻿Imports System.Configuration
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

Public Class DPHXUnpackAndLoad

    Private ReadOnly _SF As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _currentFile As String = String.Empty
    Private _abortingFirstRun As Boolean = False
    Private _allDPHData As AllData

    Public Sub SetFormCaption(filename As String)

        If filename = String.Empty Then
            filename = "No DPHX package loaded"
        End If

        'Add version to form title
        Me.Text = $"DPHX Unpack and Load v{Me.GetType.Assembly.GetName.Version} - {filename}"

    End Sub

    Private Sub DPHXUnpackAndLoad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim firstRun As Boolean = Not Settings.SessionSettings.Load()
        SetFormCaption(_currentFile)

        RestoreMainFormLocationAndSize()

        Me.Show()
        Me.Refresh()
        If firstRun Then

            Do While True
                Select Case OpenSettingsWindow()
                    Case DialogResult.Abort
                        _abortingFirstRun = True
                        Exit Do
                    Case DialogResult.OK
                        Exit Do
                End Select
            Loop

            If _abortingFirstRun Then
                Me.Close()
                Application.Exit()
            End If

        End If

    End Sub

    Private Function OpenSettingsWindow() As DialogResult
        Dim formSettings As New Settings

        Return formSettings.ShowDialog(Me)

    End Function

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click

        OpenSettingsWindow()

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
            EnableUnpackButton(True)
        End If
        SetFormCaption(_currentFile)
        packageNameToolStrip.Text = _currentFile

        If File.Exists(newDPHFile) Then
            Dim serializer As New XmlSerializer(GetType(AllData))

            On Error Resume Next

            Using stream As New FileStream(newDPHFile, FileMode.Open)
                _allDPHData = CType(serializer.Deserialize(stream), AllData)
            End Using

            ctrlBriefing.GenerateBriefing(_SF, _allDPHData, TempDPHXUnpackFolder)
        End If

    End Sub

    Private Sub DisableUnpackButton()
        btnCopyFiles.Enabled = False
        pnlUnpackBtn.BackColor = SystemColors.Control
        btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Regular)
    End Sub

    Private Sub EnableUnpackButton(emphasize As Boolean)
        btnCopyFiles.Enabled = True
        If emphasize Then
            pnlUnpackBtn.BackColor = Color.Red
            btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Bold)
        Else
            pnlUnpackBtn.BackColor = SystemColors.Control
            btnCopyFiles.Font = New Font(btnCopyFiles.Font, FontStyle.Regular)
        End If
    End Sub

    Private Sub DPHXUnpackAndLoad_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not _abortingFirstRun Then
            Settings.SessionSettings.MainFormSize = Me.Size.ToString()
            Settings.SessionSettings.MainFormLocation = Me.Location.ToString()
            Settings.SessionSettings.Save()
        End If

    End Sub

    Private Sub RestoreMainFormLocationAndSize()
        Dim sizeString As String = Settings.SessionSettings.MainFormSize
        Dim locationString As String = Settings.SessionSettings.MainFormLocation

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


    Private Sub DPHXUnpackAndLoad_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ctrlBriefing.AdjustRTBoxControls()
    End Sub

    Private Sub ChkMSFS_Tick(sender As Object, e As EventArgs) Handles ChkMSFS.Tick
        Dim processList As Process() = Process.GetProcessesByName("FlightSimulator")
        If processList.Length > 0 Then
            warningMSFSRunningToolStrip.Visible = True
        Else
            warningMSFSRunningToolStrip.Visible = False
        End If
    End Sub

    Private Sub btnCopyFiles_Click(sender As Object, e As EventArgs) Handles btnCopyFiles.Click

        If warningMSFSRunningToolStrip.Visible Then
            If MessageBox.Show($"{warningMSFSRunningToolStrip.Text}{Environment.NewLine}{Environment.NewLine}Files can be copied but weather preset will not be available in MSFS until it is restarted.{Environment.NewLine}{Environment.NewLine}Do you still want to proceed?", "MSFS is running", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                UnpackFiles()
            End If
        Else
            UnpackFiles()
        End If
    End Sub

    Private Sub UnpackFiles()

        Dim sb As New StringBuilder

        sb.AppendLine("Unpacking Results:")
        sb.AppendLine()

        'Flight plan
        sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.FlightPlanFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.FlightPlansFolder,
                 "Flight Plan"))
        sb.AppendLine()

        'Weather file
        sb.AppendLine(CopyFile(Path.GetFileName(_allDPHData.WeatherFilename),
                 TempDPHXUnpackFolder,
                 Settings.SessionSettings.MSFSWeatherPresetsFolder,
                 "Weather Preset"))

        MessageBox.Show(sb.ToString, "Unpacking results", MessageBoxButtons.OK, MessageBoxIcon.Information)
        EnableUnpackButton(False)

    End Sub

    Private Function CopyFile(filename As String, sourcePath As String, destPath As String, msgToAsk As String) As String
        Dim fullSourceFilename As String
        Dim fullDestFilename As String
        Dim proceed As Boolean = False
        Dim messageToReturn As String = String.Empty

        fullSourceFilename = Path.Combine(sourcePath, filename)
        fullDestFilename = Path.Combine(destPath, filename)
        If File.Exists(fullDestFilename) Then
            'Check what to do
            Select Case Settings.SessionSettings.AutoOverwriteFiles
                Case AllSettings.AutoOverwriteOptions.AlwaysOverwrite
                    proceed = True
                    messageToReturn = $"{msgToAsk} ""{filename}"" copied over existing one"
                Case AllSettings.AutoOverwriteOptions.AlwaysSkip
                    proceed = False
                    messageToReturn = $"{msgToAsk} ""{filename}"" skipped - already exists"
                Case AllSettings.AutoOverwriteOptions.AlwaysAsk
                    If MessageBox.Show($"The {msgToAsk} file already exists.{Environment.NewLine}{Environment.NewLine}{filename}{Environment.NewLine}{Environment.NewLine}Do you want to overwrite it?", "File already exists!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        proceed = True
                        messageToReturn = $"{msgToAsk} ""{filename}"" copied over existing one"
                    Else
                        proceed = False
                        messageToReturn = $"{msgToAsk} ""{filename}"" skipped by user - already exists"
                    End If
            End Select
        Else
            proceed = True
            messageToReturn = $"{msgToAsk} ""{filename}"" copied"
        End If
        If proceed Then
            File.Copy(fullSourceFilename, fullDestFilename, True)
        End If

        Return messageToReturn

    End Function
End Class