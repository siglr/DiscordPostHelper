Imports System
Imports System.Drawing
Imports System.Windows.Forms

Partial Public Class ManualFallbackMode
    Inherits ZoomForm

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Rescale()
    End Sub

    Private Sub ManualFallbackMode_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'TODO: Load weather presets (PLN) from the Whitelist folder into the combo box

        Reset()

    End Sub

    Private Sub Reset()

        cboWhitelistPresets.SelectedIndex = -1
        lblPLNFile.Text = String.Empty
        lblPLNTitle.Text = String.Empty
        lblWPRFile.Text = String.Empty
        lblWPRName.Text = String.Empty
        txtTrackerGroupName.Text = String.Empty

    End Sub

    Private Sub cboWhitelistPresets_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboWhitelistPresets.SelectionChangeCommitted

        'TODO: Load the selected whitelist preset details into the labels - filename and weather preset name inside the WPR XML file
        lblWPRFile.Text = "FILENAME SELECTED"
        lblWPRName.Text = "PRESET NAME INSIDE XML"

        'Example:
        '<WeatherPreset.Preset>
        '    <Name>Omarama 2024 NW20</Name>

    End Sub

    Private Sub btnCopyGoFly_Click(sender As Object, e As EventArgs) Handles btnCopyGoFly.Click

        'TODO: Check which files are selected and copy them to the appropriate MSFS folders
        'TODO: Depending on Settings, launch NB21 Logger and Tracker with the specified PLN, WPR, and Tracker Group Name
        'TODO: Give proper feedback using frmStatus and similar technique as when unpacking regular DPHX files

    End Sub

    Private Sub btnClearFiles_Click(sender As Object, e As EventArgs) Handles btnClearFiles.Click

        'TODO: Cleanup copied files from MSFS folders
        'TODO: Give proper feedback using frmStatus and similar technique as when cleaning up regular DPHX files

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

        Me.Close()

    End Sub

    Private Sub txtTrackerGroupName_Leave(sender As Object, e As EventArgs) Handles txtTrackerGroupName.Leave

        txtTrackerGroupName.Text = txtTrackerGroupName.Text.Trim()

    End Sub

    Private Sub btnSelectPLN_Click(sender As Object, e As EventArgs) Handles btnSelectPLN.Click

        'TODO: Open file dialog to select a PLN file

        'TODO: Load selected PLN file details into the labels - filename and title inside the PLN XML file

        'Example:
        '<FlightPlan.FlightPlan>
        '    <Title>Flight Title</Title>

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'TODO: Open file dialog to select a WPR file

        'TODO: Load selected WPR file details into the labels - filename and preset name inside the WPR XML file

        'Example:
        '<WeatherPreset.Preset>
        '    <Name>Omarama 2024 NW20</Name>

    End Sub
End Class
