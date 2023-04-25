Imports System.IO

Public Class DPHXUnpackAndLoad

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
                OpenFileDialog1.InitialDirectory = "H:\MSFS WIP Flight plans\"
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

            Dim validSessionFile As Boolean = True

            'Check if the selected file is a dph or dphx files
            If Path.GetExtension(OpenFileDialog1.FileName) = ".dphx" Then
                'Package - we need to unpack it first
                'OpenFileDialog1.FileName = _SF.UnpackDPHXFile(OpenFileDialog1.FileName)

                If OpenFileDialog1.FileName = String.Empty Then
                    validSessionFile = False
                Else
                    validSessionFile = True
                End If
            End If

            If validSessionFile Then
            End If
        End If

    End Sub
End Class
