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
	/// A list that is designed for Drop-Down Checkbox UI Type Editor
	/// </summary>
	public class DropDownCheckListControl : System.Windows.Forms.UserControl
	{
        private IWindowsFormsEditorService _editorService = null;
        private CheckedListBox checkedListBox1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DropDownCheckListControl(IWindowsFormsEditorService editorService)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			_editorService = editorService;
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
		/// Gets the selected index
		/// </summary>
        public CheckedListBox.CheckedIndexCollection SelectedIndices
		{
			get
			{
                return this.checkedListBox1.CheckedIndices;
			}
		}

		/// <summary>
		/// Add item with checked status
		/// </summary>
		public void AddItem(string text, bool isChecked)
		{
            this.checkedListBox1.Items.Add(text, isChecked);
		}
		
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(122, 180);
            this.checkedListBox1.TabIndex = 0;
            // 
            // DropDownCheckListControl
            // 
            this.Controls.Add(this.checkedListBox1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DropDownCheckListControl";
            this.Size = new System.Drawing.Size(122, 188);
            this.ResumeLayout(false);

		}
		#endregion

	}
}
