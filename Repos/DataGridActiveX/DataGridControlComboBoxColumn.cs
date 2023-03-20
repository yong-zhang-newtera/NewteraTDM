/*
* @(#)DataGridControlComboBoxColumn.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
    using System;
    using System.Data;
    using System.Collections;
    using System.Drawing;
    using System.Windows.Forms;

    using Newtera.DataGridActiveX.DataGridView;

    public class DataGridControlComboBoxColumn : DataGridTextBoxColumn
    {
        private InlineComboBox _inlineComboBox;
        private System.Windows.Forms.CurrencyManager _currencyManager;
        private int _rowNum;
        private bool _Editing;

        // Constructor, create our own customized Combobox
        public DataGridControlComboBoxColumn()
        {
            _currencyManager = null;
            _Editing = false;

            // Create our own customized Combobox, which is used in the DataGrid
            // DropDownList: The user cannot directly edit the text portion.
            //               The user must click the arrow button to display the
            //               list portion.
            //     DropDown: The text portion is editable. The user must click
            //               the arrow button to display the list portion.
            //       Simple: The text portion is editable. The list portion is
            //               always visible.
            _inlineComboBox = new InlineComboBox();
            _inlineComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // My own Combobox subscribes to the Leave Event. It occurs when the
            // input focus leaves the ComboBox.
            this._inlineComboBox.Leave +=
                new System.EventHandler(LeaveComboBox);

            // My own Combobox subscribes to the SelectionChangeCommitted Event.
            // It occurs when the selected item has changed and that change
            // is committed (save the changed data to the DataGrid TextBox).
            this._inlineComboBox.SelectionChangeCommitted +=
                new System.EventHandler(SelectionChangeCommit);
        }

        public InlineComboBox ComboBoxControl
        {
            get
            {
                return _inlineComboBox;
            }
        }

        // Make current Combobox invisible when user scrolls
        // the DataGrid control using the ScrollBar.
        private void HandleScroll(Object sender, EventArgs e)
        {
            if (_inlineComboBox.Visible)
            {
                _inlineComboBox.Hide();
            }
        }

        // The ColumnStartedEditing method allows the DataGrid
        // to show a pencil in the row header indicating the row
        // is being edited. (base is the parent DataGridTextBoxColumn)
        private void SelectionChangeCommit(Object sender, EventArgs e)
        {
            _Editing = true;
            base.ColumnStartedEditing((System.Windows.Forms.Control)sender);
        }

        // Handle Combobox Behaviour when Focus leaves the Combobox.
        private void LeaveComboBox(Object sender, EventArgs e)
        {
            if (_Editing)
            {
                // Set the Combobox ValueMember to the current RowColumn
                // when the Focus leaves the Combobox.
                SetColumnValueAtRow(_currencyManager, _rowNum, _inlineComboBox.Text);
                _Editing = false;

                // Redraws the column
                Invalidate();
            }
            // Hide the current Combobox when Focus on Combobox is loosen
            _inlineComboBox.Hide();

            // Let current Combobox visible when user scrolls
            // the DataGrid control using the ScrollBar.
            this.DataGridTableStyle.DataGrid.Scroll += new System.EventHandler(HandleScroll);
        }

        // The SetColumnValueAtRow method updates the bound
        // DataTable "Titles" with the ValueMember
        // for a given DisplayMember = _inlineComboBox.Text from the Combobox.
        protected override void SetColumnValueAtRow
            (CurrencyManager source, int rowNum, Object value)
        {
            Object tbDisplay = value;
            IList items = (IList)this._inlineComboBox.DataSource;
            int rowCount = items.Count;
            int i = 0;
            Object cbDisplay;
            Object cbValue;

            // Loop through the Combobox DisplayMember values
            // until you find the selected value, then read the
            // ValueMember from the Combobox and update it in the
            // DataTable "Titles"
            while (i < rowCount)
            {
                cbDisplay = ((IComboBoxItem)items[i]).DisplayText;

                if ((cbDisplay != DBNull.Value) &&
                    (tbDisplay.Equals(cbDisplay)))
                {
                    break;
                }
                i += 1;
            }
            if (i < rowCount)
            {
                cbValue = ((IComboBoxItem)items[i]).DisplayText;
            }
            else
            {
                cbValue = DBNull.Value;
            }
            base.SetColumnValueAtRow(source, rowNum, cbValue);
        }

        // The GetColumnValueAtRow method updates the bound
        // Combobox with the DisplayMember
        // for a given Row Number = rowNum from the DataTable "Titles".
        protected override Object GetColumnValueAtRow
            (CurrencyManager source, int rowNum)
        {
            // Get the ValueMember from the DataTable "Titles"
            Object tbValue = base.GetColumnValueAtRow(source, rowNum);

            // Associate a IList to the Combox, so we can search
            // the DisplayMember in the Combox corresponding to the
            // ValueMember from the DataTable "Titles"
            IList items = (IList)this._inlineComboBox.DataSource;
            int rowCount = items.Count;
            int i = 0;
            Object cbValue;

            // Loop through the Combox Entries and search the DisplayMember
            while (i < rowCount)
            {
                cbValue = ((IComboBoxItem)items[i]).DisplayText;
                if ((cbValue != DBNull.Value) &&
                    (tbValue != DBNull.Value) &&
                    (tbValue.Equals(cbValue)))
                {
                    break; // We found the DisplayMember - exit the Loop
                }
                i += 1;
            }

            // If we are within the Combox Entries, return now the DisplayMember
            // for the found ValueMember above. If we are at the End of the Combox
            // Entries, return NULL
            if (i < rowCount)
            {
                return ((IComboBoxItem)items[i]).DisplayText;
            }
            else
            {
                return DBNull.Value;
            }
        }

        // The Edit event is raised when the user sets the focus to the cell
        // containing the combobox. In this event the dimensions of the combobox
        // are set and an event handler is assigned to handle scrolling of the combobox.
        protected override void Edit(
            CurrencyManager source,
            int rowNum,
            Rectangle bounds,
            bool readOnly,
            string instantText,
            bool cellIsVisible)
        {
            base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);

            // Set current Rownum and Postion Manager
            _rowNum = rowNum;
            _currencyManager = source;

            // Calculate Location of the Combox relative to the TextBox
            // of the DataGrid which have the Focus
            Point NewLoc;
            NewLoc = this.TextBox.Location;
            NewLoc.X -= 3;
            NewLoc.Y -= 3;
            _inlineComboBox.Location = NewLoc;

            // Attach the Combobox to the same Parent Control
            // as the TextBox of the DataGrid
            _inlineComboBox.Parent = this.TextBox.Parent;

            // Position the Combox at the same Location as the TextBox
            _inlineComboBox.Size = new Size(this.TextBox.Size.Width + 3, _inlineComboBox.Size.Height);

            // Select the Entry in the Combobox corresponding to the Text in
            // in the TextBox.
            _inlineComboBox.SelectedIndex = _inlineComboBox.FindStringExact(this.TextBox.Text);
            // _inlineComboBox.Text = this.TextBox.Text;

            // Make the TextBox invisible and then show the Combobox
            this.TextBox.Visible = false;
            _inlineComboBox.Visible = true;
            _inlineComboBox.BringToFront();
            _inlineComboBox.Focus();

            // Make Combobox invisible id User scrolls uo or down the DataGrid
            this.DataGridTableStyle.DataGrid.Scroll += new System.EventHandler(HandleScroll);
        }

        // The Commit method can be used to put the Combomox ValueMember
        // into the TextBox ValueMember. This can be handled in the
        // LeaveComboBox EventHandler as well.
        protected override bool Commit(
            System.Windows.Forms.CurrencyManager dataSource, int rowNum)
        {
            if (_Editing)
            {
                _Editing = false;
                SetColumnValueAtRow(dataSource, rowNum, _inlineComboBox.Text);
            }
            return true;
        }
    }

    public class InlineComboBox : System.Windows.Forms.ComboBox
    {
        private const int WM_KEYUP = 0x101;

        // The WndProc method corresponds exactly to the Windows WindowProc function.
        // For more information about processing Windows messages, see the WindowProc
        // function documentation in the Windows Platform SDK reference located in
        // the MSDN Library.
        protected override void WndProc(ref System.Windows.Forms.Message theMessage)
        {
            // Ignore KeyUp event to avoid problem with tabbing the dropdown.
            if (theMessage.Msg == WM_KEYUP)
            {
                return;
            }
            else
            {
                base.WndProc(ref theMessage);
            }
        }
    }
}