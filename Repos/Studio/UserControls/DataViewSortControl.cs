using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.WindowsControl;

namespace Newtera.Studio.UserControls
{
    public partial class DataViewSortControl : UserControl
    {
        private DataViewModel _dataView;

        public event EventHandler SortAttributesChanged;

        public DataViewSortControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data view model
        /// </summary>
        public DataViewModel DataView
        {
            get
            {
                return _dataView;
            }
            set
            {
                _dataView = value;
            }
        }

        /// <summary>
        /// Display the sort attributes of a dataview
        /// </summary>
        public void DisplaySortAttributes()
        {
            if (_dataView != null)
            {
                ShowSortAttributes();
            }
        }

        #region private method

        /// <summary>
        /// Fire an event for number of sort attributes change
        /// </summary>
        private void FireSortAttributeChangedEvent()
        {
            if (SortAttributesChanged != null)
            {
                SortAttributesChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Display the sort attributes in the list view
        /// </summary>
        private void ShowSortAttributes()
        {
            string ownerClassCaption = "";
            if (_dataView.BaseClass != null)
            {
                ownerClassCaption = _dataView.BaseClass.Caption;
            }
            ResultAttributeListViewItem item;
            this.sortAttributesListView.SuspendLayout();
            this.sortAttributesListView.Items.Clear();

            bool selected = false;
            foreach (SortAttribute sortAttribute in _dataView.SortBy.SortAttributes)
            {
                item = new ResultAttributeListViewItem(sortAttribute.Caption, sortAttribute);
                item.ImageIndex = 1;
                item.SubItems.Add(sortAttribute.Name);

                DataClass dataClass = FindDataClass(_dataView, sortAttribute.OwnerClassAlias);
                if (dataClass != null)
                {
                    ownerClassCaption = dataClass.Caption;
                }
                item.SubItems.Add(ownerClassCaption);

                if (!selected)
                {
                    item.Selected = true;
                    selected = true;
                }

                this.sortAttributesListView.Items.Add(item);
            }

            this.sortAttributesListView.ResumeLayout();

            FireSortAttributeChangedEvent();
        }

        private DataClass FindDataClass(DataViewModel dataView, string classAlias)
        {
            DataClass dataClass = null;

            if (dataView.BaseClass.Alias == classAlias)
            {
                dataClass = dataView.BaseClass;
            }
            else
            {
                foreach (DataClass refClass in dataView.ReferencedClasses)
                {
                    if (refClass.Alias == classAlias)
                    {
                        dataClass = refClass;
                        break;
                    }
                }
            }

            return dataClass;
        }

        private void ClearSortDisplay()
        {
            this.descriptionTextBox.Text = "";
            this.captionTextBox.Text = "";
        }

        #endregion

        private void sortAttributesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.sortAttributesListView.SelectedItems.Count == 1)
            {
                this.removeSortButton.Enabled = true;
                ResultAttributeListViewItem item = (ResultAttributeListViewItem)this.sortAttributesListView.SelectedItems[0];

                this.captionTextBox.DataBindings.Clear();
                this.captionTextBox.DataBindings.Add("Text", item.ResultAttribute, "Caption");
                this.descriptionTextBox.DataBindings.Clear();
                this.descriptionTextBox.DataBindings.Add("Text", item.ResultAttribute, "Description");
                if (((SortAttribute)item.ResultAttribute).SortDirection == SortDirection.Ascending)
                {
                    this.sortDirectionComboBox.SelectedIndex = 0;
                }
                else
                {
                    this.sortDirectionComboBox.SelectedIndex = 1;
                }
            }
            else
            {
                this.removeSortButton.Enabled = false;
                ClearSortDisplay();
            }
        }

        private void addSortButton_Click(object sender, EventArgs e)
        {
            ChooseSortAttributeDialog dialog = new ChooseSortAttributeDialog();
            dialog.DataView = _dataView;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _dataView.ClearSortBy();

                foreach (SortAttribute sortAttribute in dialog.SortAttributes)
                {
                    _dataView.SortBy.SortAttributes.Add(sortAttribute);
                }

                ShowSortAttributes();
            }
        }

        private void removeSortButton_Click(object sender, EventArgs e)
        {
            if (this.sortAttributesListView.SelectedItems.Count == 1)
            {
                ResultAttributeListViewItem item = (ResultAttributeListViewItem)this.sortAttributesListView.SelectedItems[0];

                _dataView.SortBy.SortAttributes.Remove(item.ResultAttribute);

                this.sortAttributesListView.Items.Remove(item);

                this.removeSortButton.Enabled = false;

                FireSortAttributeChangedEvent();
            }
        }

        private void sortDirectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.sortAttributesListView.SelectedItems.Count == 1)
            {
                ResultAttributeListViewItem item = (ResultAttributeListViewItem)this.sortAttributesListView.SelectedItems[0];

                SortAttribute sortAttribute = (SortAttribute)item.ResultAttribute;

                if (this.sortDirectionComboBox.SelectedIndex == 0)
                {
                    if (sortAttribute.SortDirection != SortDirection.Ascending)
                    {
                        sortAttribute.SortDirection = SortDirection.Ascending;

                        FireSortAttributeChangedEvent();
                    }
                }
                else if (this.sortDirectionComboBox.SelectedIndex == 1)
                {
                    if (sortAttribute.SortDirection != SortDirection.Descending)
                    {
                        sortAttribute.SortDirection = SortDirection.Descending;

                        FireSortAttributeChangedEvent();
                    }
                }
            }
        }
    }
}
