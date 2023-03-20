using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// Summary description for FullScreenForm.
	/// </summary>
	public class FullScreenForm : System.Windows.Forms.Form
	{
		private DataGridControl _dataGridControl;
		private bool _isMaximized = false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FullScreenForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public DataGridControl DataGrid
		{
			get
			{
				return _dataGridControl;
			}
			set
			{
				_dataGridControl = value;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// FullScreenForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(350, 286);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.KeyPreview = true;
			this.Name = "FullScreenForm";
			this.Text = "FullScreenForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FullScreenForm_Closing);
			this.SizeChanged += new System.EventHandler(this.FullScreenForm_SizeChanged);

		}
		#endregion

		private void FullScreenForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_isMaximized && DataGrid != null)
			{
				DataGrid.ShowRegularScreen();
			}
		}

		private void FullScreenForm_SizeChanged(object sender, System.EventArgs e)
		{
			if (_isMaximized && DataGrid != null)
			{
				DataGrid.ShowRegularScreen();
				_isMaximized = false;
				this.Close(); // close the full-screen
			}
			else
			{
				_isMaximized = true;
			}
		}
	}
}
