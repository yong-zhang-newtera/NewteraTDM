using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;

namespace WorkflowStudio
{
    public partial class SelectSubjectDialog : Form
    {
        private EnumValueCollection _allUsers = null;
        private int _selectedIndex = -1;

        public SelectSubjectDialog()
        {
            InitializeComponent();
        }

        public string SelectedSubjectName
        {
            get
            {
                return _allUsers[_selectedIndex].Value;
            }
        }

        public string SelectedSubjectDisplayText
        {
            get
            {
                return _allUsers[_selectedIndex].DisplayText;
            }
        }

        private void SelectSubjectDialog_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                WindowClientUserManager userManager = new WindowClientUserManager();
                UsersListHandler userListHandler = new UsersListHandler();

                _allUsers = userListHandler.GetValues(userManager);

                this.listBox1.BeginUpdate();

                this.listBox1.Items.Clear();

                foreach (EnumValue usr in this._allUsers)
                {
                    this.listBox1.Items.Add(usr.Value + " (" + usr.DisplayText + ")");
                }

                this.listBox1.EndUpdate();

                if (this.listBox1.Items.Count > 0)
                {
                    // select the first user by default
                    this.listBox1.SelectedIndex = 0;
                }
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedIndex = listBox1.SelectedIndex;
        }
    }
}