using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Newtera.WinClientCommon
{
	/// <summary>
	/// A list that is designed for Drop-Down UI Type Editor
	/// </summary>
	public class DropDownListControl : System.Windows.Forms.UserControl
	{
		private object _selectedItem;
		private IWindowsFormsEditorService _editorService = null;

		private System.Windows.Forms.ListBox listBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DropDownListControl(IWindowsFormsEditorService editorService)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			_editorService = editorService;
			_selectedItem = null;
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

		/// <summary>
		/// Gets the selected item
		/// </summary>
		public object SelectedItem
		{
			get
			{
				return _selectedItem;
			}
		}

		/// <summary>
		/// Gets the selected index
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				return this.listBox.SelectedIndex;
			}
		}

		/// <summary>
		/// Gets or sets the data source of the control
		/// </summary>
		public object DataSource
		{
			get
			{
				return this.listBox.DataSource;
			}
			set
			{
				this.listBox.DataSource = value;
			}
		}

		/// <summary>
		/// Gets or sets a string that specifies the data member in the data source
		/// to be displayed.
		/// </summary>
		public string DisplayMember
		{
			get
			{
				return this.listBox.DisplayMember;
			}
			set
			{
				this.listBox.DisplayMember = value;
			}
		}

		/// <summary>
		/// Gets or sets a string that specifies the value member in the data source.
		/// </summary>
		public string ValueMember
		{
			get
			{
				return this.listBox.ValueMember;
			}
			set
			{
				this.listBox.ValueMember = value;
			}
		}
		
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// listBox
			// 
			this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.listBox.ItemHeight = 15;
			this.listBox.Location = new System.Drawing.Point(8, 8);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(96, 109);
			this.listBox.TabIndex = 0;
			this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
			// 
			// DropDownListControl
			// 
			this.Controls.Add(this.listBox);
			this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "DropDownListControl";
			this.Size = new System.Drawing.Size(112, 136);
			this.ResumeLayout(false);

		}
		#endregion

		private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.listBox.SelectedIndex >= 0)
			{
				_selectedItem = this.listBox.SelectedItem;
			}

			// Close the UI editor control upon value selection
			_editorService.CloseDropDown();
		}
	}
}
