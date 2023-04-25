Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports SIGLR.SoaringTools.CommonLibrary

Public Class DPHXUnpackAndLoad

    Private ReadOnly _SF As New SupportingFeatures(SupportingFeatures.ClientApp.DPHXUnpackAndLoad)
    Private _currentFile As String = String.Empty
    Private _XmlDocFlightPlan As XmlDocument
    Private _XmlDocWeatherPreset As XmlDocument
    Private _WeatherDetails As WeatherDetails = Nothing

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
            btnCopyFiles.Enabled = False
        Else
            txtPackageName.Text = dphxFilename
            _currentFile = dphxFilename
            btnCopyFiles.Enabled = True
            txtDPHFilename.Text = newDPHFile
        End If
        SetFormCaption(_currentFile)

        If File.Exists(newDPHFile) Then
            Dim serializer As New XmlSerializer(GetType(AllData))
            Dim allCurrentData As AllData

            On Error Resume Next

            Using stream As New FileStream(newDPHFile, FileMode.Open)
                allCurrentData = CType(serializer.Deserialize(stream), AllData)
            End Using

            'Load flight plan
            _XmlDocFlightPlan = New XmlDocument
            _XmlDocFlightPlan.Load(Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(allCurrentData.FlightPlanFilename)))
            Dim totalDistance As Integer
            Dim trackDistance As Integer
            Dim altitudeRestrictions As String = _SF.BuildAltitudeRestrictions(_XmlDocFlightPlan, totalDistance, trackDistance)

            'Load weather info
            _XmlDocWeatherPreset = New XmlDocument
            _XmlDocWeatherPreset.Load(Path.Combine(TempDPHXUnpackFolder, Path.GetFileName(allCurrentData.WeatherFilename)))
            _WeatherDetails = Nothing
            _WeatherDetails = New WeatherDetails(_XmlDocWeatherPreset)

        End If

    End Sub

End Class
