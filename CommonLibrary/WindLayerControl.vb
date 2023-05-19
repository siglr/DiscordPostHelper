Imports System.Drawing
Imports System.Windows.Forms

Public Class WindLayerControl
    Inherits UserControl

    Private _windDirection As Double = 0
    Private Property WindLayer As WindLayer

    Public Sub New(windLayer As WindLayer, Optional prefUnits As PreferredUnits = Nothing)
        InitializeComponent()
        Me.WindLayer = windLayer
        WindDirection = Me.WindLayer.Angle
        lblAltitude.Text = Me.WindLayer.WindLayerTextWithoutDirection(prefUnits)
    End Sub

    Public Property WindDirection As Double
        Get
            Return _windDirection
        End Get
        Set(value As Double)
            _windDirection = value
            picWindDirection.Refresh()
        End Set
    End Property

    Private Sub picWindDirection_Paint(sender As Object, e As PaintEventArgs) Handles picWindDirection.Paint
        Dim center As New Point(picWindDirection.Width / 2, picWindDirection.Height / 2)
        Dim radiusFullLine As Integer = 78 'Math.Min(center.X, center.Y) - 10
        Dim directionRadians As Double = WindDirection * Math.PI / 180
        Dim lineStartX As Integer = center.X + CInt(radiusFullLine * (Math.Sin(directionRadians)))
        Dim lineStartY As Integer = center.Y - CInt(radiusFullLine * (Math.Cos(directionRadians)))
        Dim lineStart As New Point(lineStartX, lineStartY)
        Dim lineEnd As New Point(center.X, center.Y)

        Dim pen As New Pen(Color.Red, 3)

        ' Draw line
        e.Graphics.DrawLine(pen, lineStart, lineEnd)

        ' Draw arrow
        Dim arrowSize As Integer = 10 + (Me.WindLayer.Speed)
        If arrowSize > 70 Then
            arrowSize = 70
        End If
        Dim radiusArrow As Integer = 62 - (arrowSize - 10) 'Math.Min(center.X, center.Y) - 10
        Dim lineStartArrowX As Integer = center.X + CInt(radiusArrow * (Math.Sin(directionRadians)))
        Dim lineStartArrowY As Integer = center.Y - CInt(radiusArrow * (Math.Cos(directionRadians)))
        Dim lineStartArrow As New Point(lineStartArrowX, lineStartArrowY)
        Dim arrowAngle As Double = Math.PI / 8 ' 22.5 degrees
        Dim arrowPoints As Point() = {
        New Point(lineStartArrow.X + CInt(arrowSize * Math.Sin(directionRadians + arrowAngle)), lineStartArrow.Y - CInt(arrowSize * Math.Cos(directionRadians + arrowAngle))),
        New Point(lineStartArrow.X + CInt(arrowSize * Math.Sin(directionRadians - arrowAngle)), lineStartArrow.Y - CInt(arrowSize * Math.Cos(directionRadians - arrowAngle))),
        lineStartArrow
    }
        e.Graphics.FillPolygon(Brushes.Red, arrowPoints)
    End Sub

End Class
