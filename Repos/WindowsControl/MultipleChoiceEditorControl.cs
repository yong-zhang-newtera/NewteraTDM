using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Text.RegularExpressions;

using Newtera.Common.MetaData.Schema;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// Summary description for MultipleChoiceEditorControl.
	/// </summary>
	public class MultipleChoiceEditorControl : System.Windows.Forms.UserControl
	{
		private IWindowsFormsEditorService _editorService = null;
		private Type _propertyType;
		private System.Windows.Forms.CheckedListBox checkedListBox;
		private System.Windows.Forms.Button CloseButton;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MultipleChoiceEditorControl(IWindowsFormsEditorService editorService,
			Type propertyType)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_editorService = editorService;
			_propertyType = propertyType;

			// display the check boxes for enum values
			InitializeCheckBoxes();
		}

		/// <summary>
		/// Gets or sets the values of checked items
		/// </summary>
		public object[] CheckedValues
		{
			get
			{
				return GetCheckedValues();
			}
			set
			{
				SetCheckboxStatus(value);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MultipleChoiceEditorControl));
			this.checkedListBox = new System.Windows.Forms.CheckedListBox();
			this.CloseButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkedListBox
			// 
			this.checkedListBox.AccessibleDescription = resources.GetString("checkedListBox.AccessibleDescription");
			this.checkedListBox.AccessibleName = resources.GetString("checkedListBox.AccessibleName");
			this.checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkedListBox.Anchor")));
			this.checkedListBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkedListBox.BackgroundImage")));
			this.checkedListBox.CheckOnClick = true;
			this.checkedListBox.ColumnWidth = ((int)(resources.GetObject("checkedListBox.ColumnWidth")));
			this.checkedListBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkedListBox.Dock")));
			this.checkedListBox.Enabled = ((bool)(resources.GetObject("checkedListBox.Enabled")));
			this.checkedListBox.Font = ((System.Drawing.Font)(resources.GetObject("checkedListBox.Font")));
			this.checkedListBox.HorizontalExtent = ((int)(resources.GetObject("checkedListBox.HorizontalExtent")));
			this.checkedListBox.HorizontalScrollbar = ((bool)(resources.GetObject("checkedListBox.HorizontalScrollbar")));
			this.checkedListBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkedListBox.ImeMode")));
			this.checkedListBox.IntegralHeight = ((bool)(resources.GetObject("checkedListBox.IntegralHeight")));
			this.checkedListBox.Location = ((System.Drawing.Point)(resources.GetObject("checkedListBox.Location")));
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkedListBox.RightToLeft")));
			this.checkedListBox.ScrollAlwaysVisible = ((bool)(resources.GetObject("checkedListBox.ScrollAlwaysVisible")));
			this.checkedListBox.Size = ((System.Drawing.Size)(resources.GetObject("checkedListBox.Size")));
			this.checkedListBox.TabIndex = ((int)(resources.GetObject("checkedListBox.TabIndex")));
			this.checkedListBox.Visible = ((bool)(resources.GetObject("checkedListBox.Visible")));
			// 
			// CloseButton
			// 
			this.CloseButton.AccessibleDescription = resources.GetString("CloseButton.AccessibleDescription");
			this.CloseButton.AccessibleName = resources.GetString("CloseButton.AccessibleName");
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("CloseButton.Anchor")));
			this.CloseButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("CloseButton.BackgroundImage")));
			this.CloseButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("CloseButton.Dock")));
			this.CloseButton.Enabled = ((bool)(resources.GetObject("CloseButton.Enabled")));
			this.CloseButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("CloseButton.FlatStyle")));
			this.CloseButton.Font = ((System.Drawing.Font)(resources.GetObject("CloseButton.Font")));
			this.CloseButton.Image = ((System.Drawing.Image)(resources.GetObject("CloseButton.Image")));
			this.CloseButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("CloseButton.ImageAlign")));
			this.CloseButton.ImageIndex = ((int)(resources.GetObject("CloseButton.ImageIndex")));
			this.CloseButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("CloseButton.ImeMode")));
			this.CloseButton.Location = ((System.Drawing.Point)(resources.GetObject("CloseButton.Location")));
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("CloseButton.RightToLeft")));
			this.CloseButton.Size = ((System.Drawing.Size)(resources.GetObject("CloseButton.Size")));
			this.CloseButton.TabIndex = ((int)(resources.GetObject("CloseButton.TabIndex")));
			this.CloseButton.Text = resources.GetString("CloseButton.Text");
			this.CloseButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("CloseButton.TextAlign")));
			this.CloseButton.Visible = ((bool)(resources.GetObject("CloseButton.Visible")));
			this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
			// 
			// MultipleChoiceEditorControl
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Controls.Add(this.CloseButton);
			this.Controls.Add(this.checkedListBox);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "MultipleChoiceEditorControl";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Display the enum values as checkbox items
		/// </summary>
		private void InitializeCheckBoxes()
		{
			string[] names = Enum.GetNames(_propertyType);
			// skip the first enum "None"
			for (int i = 1; i < names.Length; i++)
			{
				this.checkedListBox.Items.Add(names[i]);
			}
		}

		private void SetCheckboxStatus(object[] values)
		{
			for (int i = 0; i < checkedListBox.Items.Count; i++)
			{
				if (Contains((string) checkedListBox.Items[i], values))
				{
					checkedListBox.SetItemChecked(i, true);
				}
				else
				{
					checkedListBox.SetItemChecked(i, false);
				}
			}
		}

		/// <summary>
		/// Gets a string that combined the check enum values separated with ;
		/// </summary>
		/// <returns>A combined string</returns>
		private object[] GetCheckedValues()
		{
			object[] checkedValues = new object[checkedListBox.CheckedItems.Count];

			for (int i = 0; i < checkedListBox.CheckedItems.Count; i++)
			{
				checkedValues[i] = Enum.Parse(_propertyType, (string) checkedListBox.CheckedItems[i]);
			}

			return checkedValues;
		}

		/// <summary>
		/// Gets the information indicating whether a enum value exists in an array of enums.
		/// </summary>
		/// <param name="val">The value</param>
		/// <param name="values">An array of strings</param>
		/// <returns></returns>
		private bool Contains(string val, object[] values)
		{
			bool status = false;

			object enumValue = Enum.Parse(_propertyType, val);
			for (int i = 0; i < values.Length; i++)
			{
				if (Enum.Equals(enumValue, values[i]))
				{
					status = true;
					break;
				}
			}

			return status;
		}

		private void CloseButton_Click(object sender, System.EventArgs e)
		{
			// close the UI editor control
			_editorService.CloseDropDown();
		}
	}
}
