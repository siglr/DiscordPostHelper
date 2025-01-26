Imports System.ComponentModel

Public Class CheckedListComboBox

    Private _dropDownForm As Form
    Public TheCheckedListBox As CheckedListBox
    Private btnAll As Button
    Private btnNone As Button
    Private _maxVisibleItems As Integer = 5
    Private _selectedItemsTextFormat As String = "{0} item(s) selected"
    Private _isReadOnly As Boolean = False
    Private _fromUserChange As Boolean = True
    Private Shared _shownOnce As Boolean = False

    Public Property LockedValueFromUser As String

    Public Sub New()
        InitializeComponent()

        ' Initialize the dropdown form and TheCheckedListBox
        _dropDownForm = New Form With {
            .FormBorderStyle = FormBorderStyle.None,
            .ShowInTaskbar = False,
            .StartPosition = FormStartPosition.Manual,
            .Font = Me.Font,
            .TopMost = True,
            .KeyPreview = True
        }

        btnAll = New Button With {
            .Text = "All",
            .Dock = DockStyle.Left,
            .Width = Me.Width / 2 - 5
        }
        AddHandler btnAll.Click, AddressOf SelectAllItems

        btnNone = New Button With {
            .Text = "None",
            .Dock = DockStyle.Right,
            .Width = Me.Width / 2 - 3
        }
        AddHandler btnNone.Click, AddressOf DeselectAllItems

        Dim buttonPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 30
        }
        buttonPanel.Controls.Add(btnAll)
        buttonPanel.Controls.Add(btnNone)

        TheCheckedListBox = New CheckedListBox With {
            .Dock = DockStyle.Fill,
            .IntegralHeight = False,
            .Sorted = True
        }
        ' Handle ItemCheck event for TheCheckedListBox to update txtDisplay
        AddHandler TheCheckedListBox.ItemCheck, AddressOf PreventUserInteraction

        _dropDownForm.Controls.Add(TheCheckedListBox)
        _dropDownForm.Controls.Add(buttonPanel)

        ' Handle the Deactivate event to hide the dropdown when it loses focus
        AddHandler _dropDownForm.Deactivate, AddressOf DropDownForm_Deactivate

        ' Handle the KeyDown event for the dropdown form
        AddHandler _dropDownForm.KeyDown, AddressOf DropDownForm_KeyDown
        ' Handle KeyDown events for txtDisplay and btnDropdown
        AddHandler txtDisplay.KeyDown, AddressOf HandleControlKeyDown
        AddHandler btnDropdown.KeyDown, AddressOf HandleControlKeyDown

    End Sub

    ' Property to lock the CheckedListBox from user interaction
    Public Property IsReadOnly As Boolean
        Get
            Return _isReadOnly
        End Get
        Set(value As Boolean)
            _isReadOnly = value
        End Set
    End Property

    ' Prevent user from interacting with CheckedListBox if IsReadOnly is true
    Private Sub PreventUserInteraction(sender As Object, e As ItemCheckEventArgs)
        If _fromUserChange AndAlso (_isReadOnly OrElse TheCheckedListBox.Items(e.Index).ToString = LockedValueFromUser) Then
            e.NewValue = e.CurrentValue
        End If
        UpdateCheckedItemsDisplayDirect()
    End Sub

    ' Property to set the maximum number of visible items in the dropdown
    Public Property MaxVisibleItems As Integer
        Get
            Return _maxVisibleItems
        End Get
        Set(value As Integer)
            _maxVisibleItems = value
            UpdateDropDownFormHeight()
        End Set
    End Property

    ' Method to select items by a list of strings
    Public Sub SelectItemsByNames(itemNames As List(Of String))
        _fromUserChange = False
        For i As Integer = 0 To TheCheckedListBox.Items.Count - 1
            If itemNames.Contains(TheCheckedListBox.Items(i).ToString()) Then
                TheCheckedListBox.SetItemChecked(i, True)
            End If
        Next
        _fromUserChange = True
        UpdateCheckedItemsDisplayDirect()
    End Sub

    ' Property to set the format of the selected items text
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property SelectedItemsTextFormat As String
        Get
            Return _selectedItemsTextFormat
        End Get
        Set(value As String)
            _selectedItemsTextFormat = value
            UpdateCheckedItemsDisplayDirect()
        End Set
    End Property

    ' Expose the Items property of TheCheckedListBox for design-time and runtime modifications
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Public ReadOnly Property Items As CheckedListBox.ObjectCollection
        Get
            Return TheCheckedListBox.Items
        End Get
    End Property

    ' Add methods for runtime item management
    Public Sub AddItem(item As Object, Optional isChecked As Boolean = False)
        _fromUserChange = False
        TheCheckedListBox.Items.Add(item, isChecked)
        _fromUserChange = True
        UpdateCheckedItemsDisplayDirect()
    End Sub

    Public Sub RemoveItem(item As Object)
        TheCheckedListBox.Items.Remove(item)
        UpdateCheckedItemsDisplayDirect()
    End Sub

    Public Sub ClearItems()
        TheCheckedListBox.Items.Clear()
        UpdateCheckedItemsDisplayDirect()
    End Sub

    ' Method to deselect all items (Select None)
    Public Sub SelectNone()
        For i As Integer = 0 To TheCheckedListBox.Items.Count - 1
            TheCheckedListBox.SetItemChecked(i, False)
        Next
        UpdateCheckedItemsDisplayDirect()
    End Sub

    ' Method to retrieve all selected items
    Public Function GetSelectedItems() As List(Of String)
        Dim selectedItems As New List(Of String)
        For Each item As String In TheCheckedListBox.CheckedItems
            selectedItems.Add(item)
        Next
        Return selectedItems
    End Function

    ' Update the height of the dropdown form based on the number of visible items
    Private Sub UpdateDropDownFormHeight()
        Dim itemHeight As Integer = TextRenderer.MeasureText("Sample", TheCheckedListBox.Font).Height
        Dim visibleItemCount As Integer = Math.Min(_maxVisibleItems, TheCheckedListBox.Items.Count)

        ' Account for additional margins, borders, and padding
        Dim extraHeight As Integer = SystemInformation.BorderSize.Height * 2 + TheCheckedListBox.Margin.Vertical + 4 + 30 ' 30 for the buttons panel
        _dropDownForm.Height = (visibleItemCount * itemHeight) + extraHeight
    End Sub

    ' Handle the dropdown button click to show or hide the dropdown
    Private Sub btnDropdown_Click(sender As Object, e As EventArgs) Handles btnDropdown.Click
        ToggleDropdown()
    End Sub

    ' Toggle the dropdown visibility
    Private Sub ToggleDropdown()
        If _dropDownForm.Visible Then
            _dropDownForm.Hide()
            btnDropdown.Enabled = True
            Timer1.Stop()
        Else
            UpdateDropDownFormHeight()
            Dim screenPosition = txtDisplay.PointToScreen(New Point(0, txtDisplay.Height))
            _dropDownForm.Location = screenPosition
            _dropDownForm.Width = Me.Width - 5
            btnAll.Width = Me.Width / 2 - 5
            btnNone.Width = Me.Width / 2 - 5
            _dropDownForm.Show()
            If Not _shownOnce Then
                _shownOnce = True
                Application.DoEvents()
                ToggleDropdown()
                Application.DoEvents()
                ToggleDropdown()
                Application.DoEvents()
            End If
            Timer1.Start()
        End If
    End Sub

    ' Hide the dropdown when the form loses focus
    Private Sub DropDownForm_Deactivate(sender As Object, e As EventArgs)
        _dropDownForm.Hide()
    End Sub

    ' Handle the KeyDown event for the dropdown form
    Private Sub DropDownForm_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Escape OrElse (e.KeyCode = Keys.Up AndAlso e.Modifiers = Keys.Alt) OrElse (e.KeyCode = Keys.Down AndAlso e.Modifiers = Keys.Alt) Then
            _dropDownForm.Hide()
            e.Handled = True
        End If
    End Sub

    ' Handle KeyDown event for txtDisplay and btnDropdown
    Private Sub HandleControlKeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Down AndAlso e.Modifiers = Keys.Alt Then
            If Not _dropDownForm.Visible Then
                ToggleDropdown()
            End If
            e.Handled = True
        ElseIf e.KeyCode = Keys.Up AndAlso e.Modifiers = Keys.Alt Then
            If _dropDownForm.Visible Then
                _dropDownForm.Hide()
            End If
            e.Handled = True
        End If
    End Sub

    ' Select all items in the CheckedListBox
    Private Sub SelectAllItems(sender As Object, e As EventArgs)
        For i As Integer = 0 To TheCheckedListBox.Items.Count - 1
            TheCheckedListBox.SetItemChecked(i, True)
        Next
        UpdateCheckedItemsDisplayDirect()
    End Sub

    ' Deselect all items in the CheckedListBox
    Private Sub DeselectAllItems(sender As Object, e As EventArgs)
        For i As Integer = 0 To TheCheckedListBox.Items.Count - 1
            TheCheckedListBox.SetItemChecked(i, False)
        Next
        UpdateCheckedItemsDisplayDirect()
    End Sub

    ' Update txtDisplay directly for Add/Remove/Clear actions
    Private Sub UpdateCheckedItemsDisplayDirect()
        txtDisplay.Text = String.Format(_selectedItemsTextFormat, TheCheckedListBox.CheckedItems.Count)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        btnDropdown.Enabled = Not _dropDownForm.Visible
        UpdateCheckedItemsDisplayDirect()
    End Sub
End Class
