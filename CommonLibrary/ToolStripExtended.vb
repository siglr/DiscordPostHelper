Imports System
Imports System.Windows.Forms

Namespace ToolStripExtensions
    Public Class WinConst
        Public Const WM_MOUSEMOVE As UInteger = &H200
        Public Const WM_MOUSEACTIVATE As UInteger = &H21
        Public Const MA_ACTIVATE As UInteger = 1
        Public Const MA_ACTIVATEANDEAT As UInteger = 2
        Public Const MA_NOACTIVATE As UInteger = 3
        Public Const MA_NOACTIVATEANDEAT As UInteger = 4
    End Class

    Public Class ToolStripExtended
        Inherits ToolStrip

        Private _clickThrough As Boolean = True
        Private _suppressHighlighting As Boolean = False

        Public Sub New()
            MyBase.New
        End Sub

        Public Property ClickThrough As Boolean
            Get
                Return Me._clickThrough
            End Get
            Set(ByVal value As Boolean)
                Me._clickThrough = value
            End Set
        End Property

        Public Property SuppressHighlighting As Boolean
            Get
                Return Me._suppressHighlighting
            End Get
            Set(ByVal value As Boolean)
                Me._suppressHighlighting = value
            End Set
        End Property

        Protected Overrides Sub WndProc(ByRef m As Message)
            If m.Msg = WinConst.WM_MOUSEMOVE AndAlso Me._suppressHighlighting AndAlso Not Me.TopLevelControl.ContainsFocus Then
                Return
            Else
                MyBase.WndProc(m)
            End If

            If m.Msg = WinConst.WM_MOUSEACTIVATE AndAlso Me._clickThrough AndAlso m.Result = CType(WinConst.MA_ACTIVATEANDEAT, IntPtr) Then m.Result = CType(WinConst.MA_ACTIVATE, IntPtr)
        End Sub
    End Class
End Namespace
