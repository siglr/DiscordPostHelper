Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Drawing

Public Class ZoomForm
    Inherits System.Windows.Forms.Form

    <DllImport("user32.dll")>
    Private Shared Function GetDpiForWindow(hWnd As IntPtr) As UInteger
    End Function

    Private Const DESIGN_DPI As Single = 96.0F * 1.15F  ' =110.4dpi

    Private _originalClientSize As Size
    Private _originalFormSize As Size
    Private _originalMinSize As Size
    Private _originalMaxSize As Size
    Private _designDpi As Single = 0

    Public Sub New()
        Me.AutoScaleMode = AutoScaleMode.None
        ' designer will still call InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If _designDpi = 0 Then
            ' 1) capture your design metrics once
            _designDpi = DESIGN_DPI
            _originalClientSize = Me.ClientSize
            _originalFormSize = Me.Size
            _originalMinSize = Me.MinimumSize
            _originalMaxSize = Me.MaximumSize
        End If

        ' 2) measure actual DPI & compute scale
        Dim actualDpi = GetDpiForWindow(Me.Handle)
        Dim factor = actualDpi / _designDpi

        If factor <= 1.0F Then
            Return
        End If

        ' 3) zoom the client and controls
        Me.SuspendLayout()
        Me.Scale(New SizeF(factor, factor))
        Dim newClientW = CInt(Math.Round(_originalClientSize.Width * factor))
        Dim newClientH = CInt(Math.Round(_originalClientSize.Height * factor))
        Me.ClientSize = New Size(newClientW, newClientH)
        Me.ResumeLayout()

        ' 4) now adjust min/max constraints *only* if they were set
        Dim newMinW = If(_originalMinSize.Width > 0, CInt(Math.Round(_originalMinSize.Width * factor)), 0)
        Dim newMinH = If(_originalMinSize.Height > 0, CInt(Math.Round(_originalMinSize.Height * factor)), 0)
        Dim newMaxW = If(_originalMaxSize.Width > 0, CInt(Math.Round(_originalMaxSize.Width * factor)), 0)
        Dim newMaxH = If(_originalMaxSize.Height > 0, CInt(Math.Round(_originalMaxSize.Height * factor)), 0)

        If _originalMinSize.Width > 0 OrElse _originalMinSize.Height > 0 Then
            Me.MinimumSize = New Size(newMinW, newMinH)
        End If
        If _originalMaxSize.Width > 0 OrElse _originalMaxSize.Height > 0 Then
            Me.MaximumSize = New Size(newMaxW, newMaxH)
        End If
    End Sub

End Class
