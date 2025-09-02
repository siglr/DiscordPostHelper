Public Class CleanupConfirmForm
    Private Sub CleanupConfirmForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Rescale()
    End Sub

    Public Sub New(items As IEnumerable(Of String))
        InitializeComponent()
        ' Disable built-in click toggling to avoid double-toggles
        clbFiles.CheckOnClick = True
        clbFiles.Items.Clear()
        For Each s In items
            clbFiles.Items.Add(s, True) ' checked by default
        Next
    End Sub

    Public Function GetApprovedItems() As List(Of String)
        Dim res As New List(Of String)
        For Each i In clbFiles.CheckedItems
            res.Add(i.ToString())
        Next
        Return res
    End Function

End Class