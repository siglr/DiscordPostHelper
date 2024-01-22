Imports System.Drawing
Imports System.Windows.Forms

Public Class FullWeatherGraphPanel

    Private _WeatherInfo As WeatherDetails = Nothing
    Private _prefUnits As PreferredUnits

#Region "Events"


#End Region

#Region "Public Methods"

    Public Sub ResetGraph()
        WindCloudDisplay1.ResetGraph()
    End Sub

    Public Sub SetWeatherInfo(thisWeatherInfo As WeatherDetails, prefUnits As PreferredUnits, simDateTime As String)
        _WeatherInfo = thisWeatherInfo
        _prefUnits = prefUnits
        WindCloudDisplay1.SetWeatherInfo(_WeatherInfo, _prefUnits, simDateTime)
        BuildGradientsLegend()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub BuildGradientsLegend()

        splitWeatherLegend.Panel1.Controls.Clear()

        Dim myWindGradientControl As New GradientLegendControl
        myWindGradientControl.Dock = DockStyle.Fill

        Select Case _prefUnits.WindSpeed
            Case PreferredUnits.WindSpeedUnits.Knots
                myWindGradientControl.FirstValue = "26 kts"
            Case PreferredUnits.WindSpeedUnits.MeterPerSecond
                myWindGradientControl.FirstValue = $"{Conversions.KnotsToMps(26):N1} m/s"
            Case PreferredUnits.WindSpeedUnits.Both
                myWindGradientControl.FirstValue = $"26 kts ({Conversions.KnotsToMps(26):N1} m/s)"
        End Select

        myWindGradientControl.LastValue = "0"
        myWindGradientControl.GradientPalette = WindCloudDisplay1.BlueGradientPalette

        ' Add the control to your form or container
        splitWeatherLegend.Panel1.Controls.Add(myWindGradientControl)

        splitWeatherLegend.Panel2.Controls.Clear()

        Dim myCloudGradientControl As New GradientLegendControl
        myCloudGradientControl.Dock = DockStyle.Fill
        myCloudGradientControl.FirstValue = "0.0"
        myCloudGradientControl.LastValue = "5.0"
        myCloudGradientControl.GradientPalette = WindCloudDisplay1.GreyGradientPalette

        ' Add the control to your form or container
        splitWeatherLegend.Panel2.Controls.Add(myCloudGradientControl)

    End Sub

#End Region

End Class
