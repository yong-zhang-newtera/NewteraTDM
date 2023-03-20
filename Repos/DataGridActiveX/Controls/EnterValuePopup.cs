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
	/// Summary description for EnterValuePopup.
	/// </summary>
	public class EnterValuePopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private string _value;
		private DataGridViewModel _dataGridView;
		private System.Windows.Forms.TextBox textBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EnterValuePopup()
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

		/// <summary>
		/// Gets or sets the expression created by the popup
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EnterValuePopup));
			this.textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.AccessibleDescription = resources.GetString("textBox.AccessibleDescription");
			this.textBox.AccessibleName = resources.GetString("textBox.AccessibleName");
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("textBox.Anchor")));
			this.textBox.AutoSize = ((bool)(resources.GetObject("textBox.AutoSize")));
			this.textBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textBox.BackgroundImage")));
			this.textBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("textBox.Dock")));
			this.textBox.Enabled = ((bool)(resources.GetObject("textBox.Enabled")));
			this.textBox.Font = ((System.Drawing.Font)(resources.GetObject("textBox.Font")));
			this.textBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("textBox.ImeMode")));
			this.textBox.Location = ((System.Drawing.Point)(resources.GetObject("textBox.Location")));
			this.textBox.MaxLength = ((int)(resources.GetObject("textBox.MaxLength")));
			this.textBox.Multiline = ((bool)(resources.GetObject("textBox.Multiline")));
			this.textBox.Name = "textBox";
			this.textBox.PasswordChar = ((char)(resources.GetObject("textBox.PasswordChar")));
			this.textBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("textBox.RightToLeft")));
			this.textBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("textBox.ScrollBars")));
			this.textBox.Size = ((System.Drawing.Size)(resources.GetObject("textBox.Size")));
			this.textBox.TabIndex = ((int)(resources.GetObject("textBox.TabIndex")));
			this.textBox.Text = resources.GetString("textBox.Text");
			this.textBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("textBox.TextAlign")));
			this.textBox.Visible = ((bool)(resources.GetObject("textBox.Visible")));
			this.textBox.WordWrap = ((bool)(resources.GetObject("textBox.WordWrap")));
			this.textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
			// 
			// EnterValuePopup
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.textBox);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "EnterValuePopup";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.EnterValuePopup_Load);
			this.Deactivate += new System.EventHandler(this.EnterValuePopup_Deactivate);
			this.ResumeLayout(false);

		}
		#endregion


		private void EnterValuePopup_Load(object sender, System.EventArgs e)
		{
			if (_value != null && _value.Length > 0)
			{
				this.textBox.Text = _value;
			}
		}

		private void EnterValuePopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			// If the ENTER key is pressed, fire the Accept event and close the
			// popup
			if(e.KeyChar == (char)13)
			{
				_value = this.textBox.Text.Trim();

				// fire the accept event
				if (Accept != null)
				{
					Accept(this, EventArgs.Empty);
				}

				this.Close();
			}
		}
	}
}
