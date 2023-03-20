using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Newtera.Studio
{
    public partial class ChooseCategoryDialog : Form
    {
        private string _category;
        private StringCollection _existingCategories;

        public ChooseCategoryDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the selected category
        /// </summary>
        public string Category
        {
            get
            {
                return _category;
            }
        }

        /// <summary>
        /// Gets or sets the existing categories
        /// </summary>
        public StringCollection ExistingCategories
        {
            get
            {
                return _existingCategories;
            }
            set
            {
                _existingCategories = value;
            }
        }

        private void ChooseCategoryDialog_Load(object sender, EventArgs e)
        {
            if (_existingCategories != null)
            {
                foreach (string category in _existingCategories)
                {
                    this.categoryListBox.Items.Add(category);
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _category = this.categoryTextBox.Text;
        }

        private void categoryListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.categoryListBox.SelectedIndex >= 0)
            {
                this.categoryTextBox.Text = (string) this.categoryListBox.SelectedItem;
            }
        }
    }
}