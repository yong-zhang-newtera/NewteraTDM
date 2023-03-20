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
	/// Summary description for MultipleLineTextControl.
	/// </summary>
	public class MultipleLineTextControl : System.Windows.Forms.UserControl
	{
		private IWindowsFormsEditorService _editorService = null;
		private Type _propertyType;
		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.TextBox textBox;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MultipleLineTextControl(IWindowsFormsEditorService editorService,
			Type propertyType)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_editorService = editorService;
			_propertyType = propertyType;
		}

		/// <summary>
		/// Gets or sets the value of the text box with multiple lines
		/// </summary>
		public string PropertyText
		{
			get
			{
				return textBox.Text;
			}
			set
			{
				textBox.Text = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultipleLineTextControl));
            this.CloseButton = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // textBox
            // 
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.Name = "textBox";
            // 
            // MultipleLineTextControl
            // 
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.CloseButton);
            resources.ApplyResources(this, "$this");
            this.Name = "MultipleLineTextControl";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		private void CloseButton_Click(object sender, System.EventArgs e)
		{
			// close the UI editor control
			_editorService.CloseDropDown();
        }
	}
}
