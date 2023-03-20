using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace WorkflowStudio
{
    public partial class ServerTraceLogDialog : Form
    {
        public ServerTraceLogDialog()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                this.logRichTextBox.Text = "";
                AdminServiceStub adminService = new AdminServiceStub();
                adminService.ClearTraceLog();
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void ServerTraceLogDialog_Load(object sender, EventArgs e)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                AdminServiceStub adminService = new AdminServiceStub();
                string text = adminService.GetServerTraceLog();
                this.logRichTextBox.Text = text;
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                AdminServiceStub adminService = new AdminServiceStub();
                string text = adminService.GetServerTraceLog();
                this.logRichTextBox.Text = text;
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }
    }
}