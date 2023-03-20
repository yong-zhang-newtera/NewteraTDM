using System;
using System.Resources;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.DataView;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// Summary description for ChooseEnumValuePopup.
	/// </summary>
	public class ChooseEnumValuePopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private string _value;
		private DataViewModel _dataView;
		private Type _enumType;

		private System.Windows.Forms.ListBox listBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseEnumValuePopup()
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
		/// Gets or sets the enum type of boolean values
		/// </summary>
		/// <remarks>The boolean enum type is dynamically generated.</remarks>
		public Type EnumType
		{
			get
			{
				return _enumType;
			}
			set
			{
				_enumType = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseEnumValuePopup));
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            resources.ApplyResources(this.listBox, "listBox");
            this.listBox.Name = "listBox";
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // ChooseEnumValuePopup
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.listBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChooseEnumValuePopup";
            this.Deactivate += new System.EventHandler(this.ChooseEnumValuePopup_Deactivate);
            this.Load += new System.EventHandler(this.ChooseEnumValuePopup_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_value = (string) this.listBox.SelectedItem;
		
			// set the value to empty string if value is none
			ResourceManager resources = new ResourceManager(typeof(LocaleInfo));

			if (_value == resources.GetString("None"))
			{
				_value = "";
			}

			// fire the accept event
			if (Accept != null)
			{
				Accept(this, EventArgs.Empty);
			}

			this.Close();
		}

		private void ChooseEnumValuePopup_Load(object sender, System.EventArgs e)
		{
			this.listBox.Items.Clear();

			if (_enumType != null)
			{
				foreach (string val in Enum.GetNames(_enumType))
				{
					this.listBox.Items.Add(val);
				}
			}
		}

		private void ChooseEnumValuePopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
