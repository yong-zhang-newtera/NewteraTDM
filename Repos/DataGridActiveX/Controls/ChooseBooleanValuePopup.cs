using System;
using System.Resources;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.DataGridView;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// Summary description for ChooseBooleanValuePopup.
	/// </summary>
	public class ChooseBooleanValuePopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private string _value;
		private DataGridViewModel _dataGridView;

		private System.Windows.Forms.ListBox listBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseBooleanValuePopup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_value = null;
		}

		#region IExprPopup

		/// <summary>
		/// Gets the expression created by the popup
		/// </summary>
		public object Expression
		{
			get
			{
				return _value;
			}
			set
			{
				_value = (string) value;
			}
		}

		/// <summary>
		/// Gets or sets the data view to which a search expression is built
		/// </summary>
		/// <value>A DataVieModel instance</value>
		public DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;
			}
		}

		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the
		/// popup relative to the window.
		/// </summary>
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		public new void Show()
		{
			base.Show();
		}

		#endregion


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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChooseBooleanValuePopup));
			this.listBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// listBox
			// 
			this.listBox.AccessibleDescription = resources.GetString("listBox.AccessibleDescription");
			this.listBox.AccessibleName = resources.GetString("listBox.AccessibleName");
			this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("listBox.Anchor")));
			this.listBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listBox.BackgroundImage")));
			this.listBox.ColumnWidth = ((int)(resources.GetObject("listBox.ColumnWidth")));
			this.listBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("listBox.Dock")));
			this.listBox.Enabled = ((bool)(resources.GetObject("listBox.Enabled")));
			this.listBox.Font = ((System.Drawing.Font)(resources.GetObject("listBox.Font")));
			this.listBox.HorizontalExtent = ((int)(resources.GetObject("listBox.HorizontalExtent")));
			this.listBox.HorizontalScrollbar = ((bool)(resources.GetObject("listBox.HorizontalScrollbar")));
			this.listBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("listBox.ImeMode")));
			this.listBox.IntegralHeight = ((bool)(resources.GetObject("listBox.IntegralHeight")));
			this.listBox.ItemHeight = ((int)(resources.GetObject("listBox.ItemHeight")));
			this.listBox.Location = ((System.Drawing.Point)(resources.GetObject("listBox.Location")));
			this.listBox.Name = "listBox";
			this.listBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("listBox.RightToLeft")));
			this.listBox.ScrollAlwaysVisible = ((bool)(resources.GetObject("listBox.ScrollAlwaysVisible")));
			this.listBox.Size = ((System.Drawing.Size)(resources.GetObject("listBox.Size")));
			this.listBox.TabIndex = ((int)(resources.GetObject("listBox.TabIndex")));
			this.listBox.Visible = ((bool)(resources.GetObject("listBox.Visible")));
			this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
			// 
			// ChooseBooleanValuePopup
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.listBox);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "ChooseBooleanValuePopup";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.ChooseBooleanValuePopup_Load);
			this.Deactivate += new System.EventHandler(this.ChooseBooleanValuePopup_Deactivate);
			this.ResumeLayout(false);

		}
		#endregion

		private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_value = (string) this.listBox.SelectedItem;
		
			// fire the accept event
			if (Accept != null)
			{
				Accept(this, EventArgs.Empty);
			}

			this.Close();
		}

		private void ChooseBooleanValuePopup_Load(object sender, System.EventArgs e)
		{
			this.listBox.Items.Clear();

			this.listBox.Items.Add("");
			this.listBox.Items.Add("true");
			this.listBox.Items.Add("false");
		}

		private void ChooseBooleanValuePopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
