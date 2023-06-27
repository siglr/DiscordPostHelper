Imports System.Drawing
Imports System.Windows.Forms

Public Class WaitingToCloseApp

    Private _processToWaitFor As Process

    Public Sub New(ProcessToWaitFor As Process)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _processToWaitFor = ProcessToWaitFor

    End Sub

    Private Sub btnAbort_Click(sender As Object, e As EventArgs) Handles btnAbort.Click
        Timer1.Enabled = False
        Me.DialogResult = DialogResult.Abort
        Me.Close()
    End Sub

    Private Sub WaitingToCloseApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If _processToWaitFor IsNot Nothing AndAlso Not _processToWaitFor.HasExited Then
            ' Process is still running
        Else
            Timer1.Enabled = False
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If

    End Sub
End Class