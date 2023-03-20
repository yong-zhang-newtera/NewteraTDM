using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChooseConstraintDialog.
	/// </summary>
	public class ChooseConstraintDialog : System.Windows.Forms.Form
	{
		private SchemaModel _schemaModel = null;
		private ConstraintElementBase _constraint = null;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button createButton;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage enumPage;
		private System.Windows.Forms.TabPage rangePage;
		private System.Windows.Forms.TabPage patternPage;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox enumValueListBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox patternTextBox;
		private System.Windows.Forms.TextBox rangeTypeTextBox;
		private System.Windows.Forms.TextBox rangeMinTextBox;
		private System.Windows.Forms.TextBox rangeMaxTextBox;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TabPage listPage;
		private System.Windows.Forms.Label listHandlerLlabel;
		private System.Windows.Forms.TextBox handlerTextBox;
		private System.Windows.Forms.Label label8;
		private System.ComponentModel.IContainer components;

		public ChooseConstraintDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
		/// Gets or sets the schema model from which to get all
		/// available constraints
		/// </summary>
		public SchemaModel SchemaModel
		{
			get
			{
				return _schemaModel;
			}
			set
			{
				_schemaModel = value;
			}
		}

		/// <summary>
		/// Gets or sets the selected constraint instance
		/// </summary>
		public ConstraintElementBase SelectedConstraint
		{
			get
			{
				return _constraint;
			}
			set
			{
				_constraint = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseConstraintDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.createButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.enumPage = new System.Windows.Forms.TabPage();
            this.enumValueListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rangePage = new System.Windows.Forms.TabPage();
            this.rangeMaxTextBox = new System.Windows.Forms.TextBox();
            this.rangeMinTextBox = new System.Windows.Forms.TextBox();
            this.rangeTypeTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.patternPage = new System.Windows.Forms.TabPage();
            this.patternTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.listPage = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.handlerTextBox = new System.Windows.Forms.TextBox();
            this.listHandlerLlabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1.SuspendLayout();
            this.enumPage.SuspendLayout();
            this.rangePage.SuspendLayout();
            this.patternPage.SuspendLayout();
            this.listPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comboBox1
            // 
            this.comboBox1.AccessibleDescription = null;
            this.comboBox1.AccessibleName = null;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.BackgroundImage = null;
            this.comboBox1.Font = null;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4")});
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            // 
            // createButton
            // 
            this.createButton.AccessibleDescription = null;
            this.createButton.AccessibleName = null;
            resources.ApplyResources(this.createButton, "createButton");
            this.createButton.BackgroundImage = null;
            this.createButton.Font = null;
            this.createButton.Name = "createButton";
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.enumPage);
            this.tabControl1.Controls.Add(this.rangePage);
            this.tabControl1.Controls.Add(this.patternPage);
            this.tabControl1.Controls.Add(this.listPage);
            this.tabControl1.Font = null;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // enumPage
            // 
            this.enumPage.AccessibleDescription = null;
            this.enumPage.AccessibleName = null;
            resources.ApplyResources(this.enumPage, "enumPage");
            this.enumPage.BackgroundImage = null;
            this.enumPage.Controls.Add(this.enumValueListBox);
            this.enumPage.Controls.Add(this.label3);
            this.enumPage.Font = null;
            this.enumPage.Name = "enumPage";
            // 
            // enumValueListBox
            // 
            this.enumValueListBox.AccessibleDescription = null;
            this.enumValueListBox.AccessibleName = null;
            resources.ApplyResources(this.enumValueListBox, "enumValueListBox");
            this.enumValueListBox.BackgroundImage = null;
            this.enumValueListBox.Font = null;
            this.enumValueListBox.Name = "enumValueListBox";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // rangePage
            // 
            this.rangePage.AccessibleDescription = null;
            this.rangePage.AccessibleName = null;
            resources.ApplyResources(this.rangePage, "rangePage");
            this.rangePage.BackgroundImage = null;
            this.rangePage.Controls.Add(this.rangeMaxTextBox);
            this.rangePage.Controls.Add(this.rangeMinTextBox);
            this.rangePage.Controls.Add(this.rangeTypeTextBox);
            this.rangePage.Controls.Add(this.label6);
            this.rangePage.Controls.Add(this.label5);
            this.rangePage.Controls.Add(this.label4);
            this.rangePage.Font = null;
            this.rangePage.Name = "rangePage";
            // 
            // rangeMaxTextBox
            // 
            this.rangeMaxTextBox.AccessibleDescription = null;
            this.rangeMaxTextBox.AccessibleName = null;
            resources.ApplyResources(this.rangeMaxTextBox, "rangeMaxTextBox");
            this.rangeMaxTextBox.BackgroundImage = null;
            this.rangeMaxTextBox.Font = null;
            this.rangeMaxTextBox.Name = "rangeMaxTextBox";
            this.rangeMaxTextBox.ReadOnly = true;
            // 
            // rangeMinTextBox
            // 
            this.rangeMinTextBox.AccessibleDescription = null;
            this.rangeMinTextBox.AccessibleName = null;
            resources.ApplyResources(this.rangeMinTextBox, "rangeMinTextBox");
            this.rangeMinTextBox.BackgroundImage = null;
            this.rangeMinTextBox.Font = null;
            this.rangeMinTextBox.Name = "rangeMinTextBox";
            this.rangeMinTextBox.ReadOnly = true;
            // 
            // rangeTypeTextBox
            // 
            this.rangeTypeTextBox.AccessibleDescription = null;
            this.rangeTypeTextBox.AccessibleName = null;
            resources.ApplyResources(this.rangeTypeTextBox, "rangeTypeTextBox");
            this.rangeTypeTextBox.BackgroundImage = null;
            this.rangeTypeTextBox.Font = null;
            this.rangeTypeTextBox.Name = "rangeTypeTextBox";
            this.rangeTypeTextBox.ReadOnly = true;
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // patternPage
            // 
            this.patternPage.AccessibleDescription = null;
            this.patternPage.AccessibleName = null;
            resources.ApplyResources(this.patternPage, "patternPage");
            this.patternPage.BackgroundImage = null;
            this.patternPage.Controls.Add(this.patternTextBox);
            this.patternPage.Controls.Add(this.label7);
            this.patternPage.Font = null;
            this.patternPage.Name = "patternPage";
            // 
            // patternTextBox
            // 
            this.patternTextBox.AccessibleDescription = null;
            this.patternTextBox.AccessibleName = null;
            resources.ApplyResources(this.patternTextBox, "patternTextBox");
            this.patternTextBox.BackgroundImage = null;
            this.patternTextBox.Font = null;
            this.patternTextBox.Name = "patternTextBox";
            this.patternTextBox.ReadOnly = true;
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // listPage
            // 
            this.listPage.AccessibleDescription = null;
            this.listPage.AccessibleName = null;
            resources.ApplyResources(this.listPage, "listPage");
            this.listPage.BackgroundImage = null;
            this.listPage.Controls.Add(this.label8);
            this.listPage.Controls.Add(this.handlerTextBox);
            this.listPage.Controls.Add(this.listHandlerLlabel);
            this.listPage.Font = null;
            this.listPage.Name = "listPage";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // handlerTextBox
            // 
            this.handlerTextBox.AccessibleDescription = null;
            this.handlerTextBox.AccessibleName = null;
            resources.ApplyResources(this.handlerTextBox, "handlerTextBox");
            this.handlerTextBox.BackgroundImage = null;
            this.handlerTextBox.Font = null;
            this.handlerTextBox.Name = "handlerTextBox";
            // 
            // listHandlerLlabel
            // 
            this.listHandlerLlabel.AccessibleDescription = null;
            this.listHandlerLlabel.AccessibleName = null;
            resources.ApplyResources(this.listHandlerLlabel, "listHandlerLlabel");
            this.listHandlerLlabel.Font = null;
            this.listHandlerLlabel.Name = "listHandlerLlabel";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // listView1
            // 
            this.listView1.AccessibleDescription = null;
            this.listView1.AccessibleName = null;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.BackgroundImage = null;
            this.listView1.Font = null;
            this.listView1.HideSelection = false;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            // 
            // ChooseConstraintDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "ChooseConstraintDialog";
            this.Load += new System.EventHandler(this.ChooseConstraintDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.enumPage.ResumeLayout(false);
            this.rangePage.ResumeLayout(false);
            this.rangePage.PerformLayout();
            this.patternPage.ResumeLayout(false);
            this.patternPage.PerformLayout();
            this.listPage.ResumeLayout(false);
            this.listPage.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Display the constraints in the list view.
		/// </summary>
		private void ShowConstraints()
		{
			SchemaModelElementCollection constraints;
			ListViewItem item;

			this.listView1.BeginUpdate();

			// clear the list view
			this.listView1.Items.Clear();

			switch (this.comboBox1.SelectedIndex)
			{
				case 0 : // show all constraints
					constraints = _schemaModel.EnumConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 0);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					constraints = _schemaModel.RangeConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 1);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					constraints = _schemaModel.PatternConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 2);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					constraints = _schemaModel.ListConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 3);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					break;
				case 1: // show enum constraints
					constraints = _schemaModel.EnumConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 0);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					break;
				case 2: // show range constraints
					constraints = _schemaModel.RangeConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 1);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					break;
				case 3: // show pattern constraints
					constraints = _schemaModel.PatternConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 2);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					break;

				case 4: // show list constraints
					constraints = _schemaModel.ListConstraints;
					foreach (ConstraintElementBase constraint in constraints)
					{
						item = this.listView1.Items.Add(constraint.Caption, 0);
						item.SubItems.Add(constraint.Name);
						if (_constraint != null && _constraint.Name == constraint.Name)
						{
							// show the item as selected
							item.Selected = true;
						}
					}

					break;
			}

			this.listView1.EndUpdate();
		}

		private void ShowConstraintDetail(ConstraintElementBase constraint)
		{
			if (constraint is EnumElement)
			{
				this.tabControl1.SelectedTab = this.enumPage;
				this.enumValueListBox.DataSource = ((EnumElement) constraint).Values;
				this.enumValueListBox.ValueMember = "Value";
				this.enumValueListBox.DisplayMember = "DisplayText";
			}
			else if (constraint is RangeElement)
			{
				this.tabControl1.SelectedTab = this.rangePage;
				this.rangeTypeTextBox.Text = DataTypeConverter.ConvertToTypeString(((RangeElement) constraint).DataType);
				this.rangeMinTextBox.Text = ((RangeElement) constraint).MinValue;
				this.rangeMaxTextBox.Text = ((RangeElement) constraint).MaxValue;			
			}
			else if (constraint is PatternElement)
			{
				this.tabControl1.SelectedTab = this.patternPage;
				this.patternTextBox.Text = ((PatternElement) constraint).PatternValue;
			}
			else if (constraint is ListElement)
			{
				this.tabControl1.SelectedTab = this.listPage;
				this.handlerTextBox.Text = ((ListElement) constraint).ListHandlerName;
			}
		}

		private void ChooseConstraintDialog_Load(object sender, System.EventArgs e)
		{
			this.comboBox1.SelectedIndex = 0; // show all constraints

			if (_schemaModel != null)
			{
				ShowConstraints();
			}
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_schemaModel != null)
			{
				ListView.SelectedListViewItemCollection items = this.listView1.SelectedItems;

				if (items.Count == 1)
				{
					ListViewItem item = (ListViewItem) items[0];
					
					_constraint = _schemaModel.FindConstraint(item.SubItems[1].Text);

					if (_constraint != null)
					{
						ShowConstraintDetail(_constraint);
					}
				}		
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_schemaModel != null)
			{
				ShowConstraints();
			}		
		}
	}
}
