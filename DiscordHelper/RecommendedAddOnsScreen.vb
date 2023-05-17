Imports SIGLR.SoaringTools.CommonLibrary

Public Class RecommendedAddOnsForm

    Private _editMode As Boolean = False
    Private _addOn As RecommendedAddOn
    Private _freshlyLoaded As Boolean = True

    Public Function ShowForm(parent As Form, addOn As RecommendedAddOn, editMode As Boolean) As DialogResult

        _editMode = editMode
        _addOn = addOn

        txtAddOnName.Text = _addOn.Name
        txtAddOnURL.Text = _addOn.URL

        Select Case _addOn.Type
            Case RecommendedAddOn.Types.Freeware
                radioTypeFreeware.Checked = True
            Case RecommendedAddOn.Types.Payware
                radioTypePayware.Checked = True
        End Select

        _freshlyLoaded = True
        Me.ShowDialog(parent)

        Return Me.DialogResult

    End Function

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click

        _addOn.Name = txtAddOnName.Text
        _addOn.URL = txtAddOnURL.Text
        If radioTypeFreeware.Checked Then
            _addOn.Type = RecommendedAddOn.Types.Freeware
        End If
        If radioTypePayware.Checked Then
            _addOn.Type = RecommendedAddOn.Types.Payware
        End If
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnNamePaste_Click(sender As Object, e As EventArgs) Handles btnNamePaste.Click

        txtAddOnName.Text = Clipboard.GetText()

    End Sub

    Private Sub btnURLPaste_Click(sender As Object, e As EventArgs) Handles btnURLPaste.Click

        txtAddOnURL.Text = Clipboard.GetText()

    End Sub

    Private Sub txtAddOnName_TextChanged(sender As Object, e As EventArgs) Handles txtAddOnURL.TextChanged, txtAddOnName.TextChanged

        If (Not txtAddOnName.Text = String.Empty) AndAlso (Not txtAddOnURL.Text = String.Empty) Then
            btnSave.Enabled = True
        Else
            btnSave.Enabled = False
        End If
    End Sub

    Private Sub txtAddOnName_Enter(sender As Object, e As EventArgs) Handles txtAddOnURL.Enter, txtAddOnName.Enter
        SupportingFeatures.EnteringTextBox(sender)
    End Sub

    Private Sub RecommendedAddOnsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtAddOnName.Focus()
    End Sub

    Private Sub RecommendedAddOnsForm_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        If _freshlyLoaded Then
            txtAddOnName.Focus()
        End If
        _freshlyLoaded = False
    End Sub

End Class