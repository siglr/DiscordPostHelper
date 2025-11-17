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

End Class
