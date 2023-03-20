using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// DefineEnumValuesDialog 的摘要说明。
	/// </summary>
	public class DefineEnumValuesDialog : System.Windows.Forms.Form
	{
		private EnumValueCollection _values;
		private EnumElement _enumElement;

		private System.Windows.Forms.ListBox enumValueListBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button delButton;
		private Newtera.WindowsControl.EnterTextBox valueEnterTextBox;
		private System.Windows.Forms.Label label1;
        private Newtera.WindowsControl.EnterTextBox textEnterTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ErrorProvider infoProvider;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.TextBox valueTextBox;
        private Button imageButton;
        private Label label3;
        private TextBox imageNameTextBox;
        private Label imageLabel;
        private Button upButton;
        private Button downButton;
		private System.ComponentModel.IContainer components;

		public DefineEnumValuesDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_values = null;
			_enumElement = null;
		}

		/// <summary>
		/// Gets or sets the enum value collection
		/// </summary>
		public EnumValueCollection Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;
			}
		}

		/// <summary>
		/// Gets or sets the enum element
		/// </summary>
		public EnumElement EnumElement
		{
			get
			{
				return _enumElement;
			}
			set
			{
				_enumElement = value;
			}
		}

		/// <summary>
		/// 清理所有正在使用的资源。
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

		private bool IsEnumValueExist(string val, int excludeIndex)
		{
			bool status = false;

			if (this._values != null && val != null)
			{
				int index = 0;
				foreach (EnumValue enumValue in _values)
				{
					if (enumValue.Value == val && index != excludeIndex)
					{
						status = true;
						break;
					}

					index++;
				}
			}

			return status;
		}

		private bool IsEnumTextExist(string text, int excludeIndex)
		{
			bool status = false;

			if (this._values != null && text != null)
			{
				int index = 0;
				foreach (EnumValue enumValue in _values)
				{
					if (enumValue.DisplayText == text && index != excludeIndex)
					{
						status = true;
						break;
					}

					index++;
				}
			}

			return status;
		}

		private void AddEnumValue()
		{
            string msg;
			if (this.valueEnterTextBox.Text == null ||
				this.valueEnterTextBox.Text.Length == 0)
			{
				MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.NullEnumText"),
					"Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
            else if (IsEnumValueExist(this.valueEnterTextBox.Text, -1))
            {
                msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.EnumValueExist"), this.valueEnterTextBox.Text);
                MessageBox.Show(msg,
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                EnumValue enumValue = new EnumValue();
                enumValue.Value = this.valueEnterTextBox.Text;
                this._values.Add(enumValue);

                ShowEnumValues();

                this.enumValueListBox.SelectedIndex = this._values.Count - 1;

                this.valueEnterTextBox.Text = null;
            }
		}

        /// <summary>
        /// Due to a .net constriant, the enum display text can't be used by any of the enum values except itself
        /// </summary>
        /// <param name="enumText">The enum text</param>
        /// <returns>true if it is a valid value, false otherwise</returns>
        private bool IsEnumValueValid(string enumText, int excludeIndex)
        {
            bool status = true;

            if (this._values != null && enumText != null)
            {
                int index = 0;
                foreach (EnumValue enumValue in _values)
                {
                    if (enumValue.Value == enumText && index != excludeIndex)
                    {
                        status = false;
                        break;
                    }

                    index++;
                }
            }

            return status;
        }

        /// <summary>
        /// Due to a .net constraint, the enum display text can't be zero
        /// </summary>
        /// <param name="enumText">The enum text</param>
        /// <returns>true if it is a legal value, false otherwise</returns>
        private bool IsEnumValueLegal(string enumText)
        {
            bool status = true;

            try
            {
                int number = int.Parse(enumText);

                if (number == 0)
                {
                    status = false;
                }
            }
            catch (Exception)
            {
            }

            return status;
        }

		private void ShowEnumValues()
		{
			this.enumValueListBox.Items.Clear();
			if (this._values != null)
			{
				foreach (EnumValue enumValue in this._values)
				{
					this.enumValueListBox.Items.Add(enumValue.DisplayText);
				}
			}
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefineEnumValuesDialog));
            this.enumValueListBox = new System.Windows.Forms.ListBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.delButton = new System.Windows.Forms.Button();
            this.valueEnterTextBox = new Newtera.WindowsControl.EnterTextBox();
            this.textEnterTextBox = new Newtera.WindowsControl.EnterTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.imageNameTextBox = new System.Windows.Forms.TextBox();
            this.imageLabel = new System.Windows.Forms.Label();
            this.imageButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // enumValueListBox
            // 
            this.enumValueListBox.AccessibleDescription = null;
            this.enumValueListBox.AccessibleName = null;
            resources.ApplyResources(this.enumValueListBox, "enumValueListBox");
            this.enumValueListBox.BackgroundImage = null;
            this.infoProvider.SetError(this.enumValueListBox, resources.GetString("enumValueListBox.Error"));
            this.enumValueListBox.Font = null;
            this.infoProvider.SetIconAlignment(this.enumValueListBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("enumValueListBox.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.enumValueListBox, ((int)(resources.GetObject("enumValueListBox.IconPadding"))));
            this.enumValueListBox.Name = "enumValueListBox";
            this.toolTip.SetToolTip(this.enumValueListBox, resources.GetString("enumValueListBox.ToolTip"));
            this.enumValueListBox.SelectedIndexChanged += new System.EventHandler(this.enumValueListBox_SelectedIndexChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.infoProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error"));
            this.cancelButton.Font = null;
            this.infoProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding"))));
            this.cancelButton.Name = "cancelButton";
            this.toolTip.SetToolTip(this.cancelButton, resources.GetString("cancelButton.ToolTip"));
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.infoProvider.SetError(this.okButton, resources.GetString("okButton.Error"));
            this.okButton.Font = null;
            this.infoProvider.SetIconAlignment(this.okButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("okButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.okButton, ((int)(resources.GetObject("okButton.IconPadding"))));
            this.okButton.Name = "okButton";
            this.toolTip.SetToolTip(this.okButton, resources.GetString("okButton.ToolTip"));
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // addButton
            // 
            this.addButton.AccessibleDescription = null;
            this.addButton.AccessibleName = null;
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.BackgroundImage = null;
            this.infoProvider.SetError(this.addButton, resources.GetString("addButton.Error"));
            this.addButton.Font = null;
            this.infoProvider.SetIconAlignment(this.addButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("addButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.addButton, ((int)(resources.GetObject("addButton.IconPadding"))));
            this.addButton.Name = "addButton";
            this.toolTip.SetToolTip(this.addButton, resources.GetString("addButton.ToolTip"));
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // delButton
            // 
            this.delButton.AccessibleDescription = null;
            this.delButton.AccessibleName = null;
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.BackgroundImage = null;
            this.infoProvider.SetError(this.delButton, resources.GetString("delButton.Error"));
            this.delButton.Font = null;
            this.infoProvider.SetIconAlignment(this.delButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("delButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.delButton, ((int)(resources.GetObject("delButton.IconPadding"))));
            this.delButton.Name = "delButton";
            this.toolTip.SetToolTip(this.delButton, resources.GetString("delButton.ToolTip"));
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // valueEnterTextBox
            // 
            this.valueEnterTextBox.AccessibleDescription = null;
            this.valueEnterTextBox.AccessibleName = null;
            resources.ApplyResources(this.valueEnterTextBox, "valueEnterTextBox");
            this.valueEnterTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.valueEnterTextBox, resources.GetString("valueEnterTextBox.Error"));
            this.valueEnterTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.valueEnterTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("valueEnterTextBox.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.valueEnterTextBox, ((int)(resources.GetObject("valueEnterTextBox.IconPadding"))));
            this.valueEnterTextBox.Name = "valueEnterTextBox";
            this.toolTip.SetToolTip(this.valueEnterTextBox, resources.GetString("valueEnterTextBox.ToolTip"));
            this.valueEnterTextBox.TextChanged += new System.EventHandler(this.valueEnterTextBox_TextChanged);
            this.valueEnterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.valueEnterTextBox_KeyDown);
            // 
            // textEnterTextBox
            // 
            this.textEnterTextBox.AccessibleDescription = null;
            this.textEnterTextBox.AccessibleName = null;
            resources.ApplyResources(this.textEnterTextBox, "textEnterTextBox");
            this.textEnterTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.textEnterTextBox, resources.GetString("textEnterTextBox.Error"));
            this.textEnterTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.textEnterTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textEnterTextBox.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.textEnterTextBox, ((int)(resources.GetObject("textEnterTextBox.IconPadding"))));
            this.textEnterTextBox.Name = "textEnterTextBox";
            this.toolTip.SetToolTip(this.textEnterTextBox, resources.GetString("textEnterTextBox.ToolTip"));
            this.textEnterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textEnterTextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.infoProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.label1.Font = null;
            this.infoProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.infoProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.label2.Font = null;
            this.infoProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.imageNameTextBox);
            this.groupBox1.Controls.Add(this.imageLabel);
            this.groupBox1.Controls.Add(this.imageButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textEnterTextBox);
            this.groupBox1.Controls.Add(this.valueTextBox);
            this.infoProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this.groupBox1.Font = null;
            this.infoProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // imageNameTextBox
            // 
            this.imageNameTextBox.AccessibleDescription = null;
            this.imageNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.imageNameTextBox, "imageNameTextBox");
            this.imageNameTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.imageNameTextBox, resources.GetString("imageNameTextBox.Error"));
            this.imageNameTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.imageNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("imageNameTextBox.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.imageNameTextBox, ((int)(resources.GetObject("imageNameTextBox.IconPadding"))));
            this.imageNameTextBox.Name = "imageNameTextBox";
            this.toolTip.SetToolTip(this.imageNameTextBox, resources.GetString("imageNameTextBox.ToolTip"));
            // 
            // imageLabel
            // 
            this.imageLabel.AccessibleDescription = null;
            this.imageLabel.AccessibleName = null;
            resources.ApplyResources(this.imageLabel, "imageLabel");
            this.infoProvider.SetError(this.imageLabel, resources.GetString("imageLabel.Error"));
            this.imageLabel.Font = null;
            this.infoProvider.SetIconAlignment(this.imageLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("imageLabel.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.imageLabel, ((int)(resources.GetObject("imageLabel.IconPadding"))));
            this.imageLabel.Name = "imageLabel";
            this.toolTip.SetToolTip(this.imageLabel, resources.GetString("imageLabel.ToolTip"));
            // 
            // imageButton
            // 
            this.imageButton.AccessibleDescription = null;
            this.imageButton.AccessibleName = null;
            resources.ApplyResources(this.imageButton, "imageButton");
            this.imageButton.BackgroundImage = null;
            this.infoProvider.SetError(this.imageButton, resources.GetString("imageButton.Error"));
            this.imageButton.Font = null;
            this.infoProvider.SetIconAlignment(this.imageButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("imageButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.imageButton, ((int)(resources.GetObject("imageButton.IconPadding"))));
            this.imageButton.Name = "imageButton";
            this.toolTip.SetToolTip(this.imageButton, resources.GetString("imageButton.ToolTip"));
            this.imageButton.UseVisualStyleBackColor = true;
            this.imageButton.Click += new System.EventHandler(this.imageButton_Click);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.infoProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.label3.Font = null;
            this.infoProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // valueTextBox
            // 
            this.valueTextBox.AccessibleDescription = null;
            this.valueTextBox.AccessibleName = null;
            resources.ApplyResources(this.valueTextBox, "valueTextBox");
            this.valueTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.valueTextBox, resources.GetString("valueTextBox.Error"));
            this.valueTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.valueTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("valueTextBox.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.valueTextBox, ((int)(resources.GetObject("valueTextBox.IconPadding"))));
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.valueTextBox, resources.GetString("valueTextBox.ToolTip"));
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            resources.ApplyResources(this.infoProvider, "infoProvider");
            // 
            // upButton
            // 
            this.upButton.AccessibleDescription = null;
            this.upButton.AccessibleName = null;
            resources.ApplyResources(this.upButton, "upButton");
            this.upButton.BackgroundImage = null;
            this.infoProvider.SetError(this.upButton, resources.GetString("upButton.Error"));
            this.upButton.Font = null;
            this.infoProvider.SetIconAlignment(this.upButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("upButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.upButton, ((int)(resources.GetObject("upButton.IconPadding"))));
            this.upButton.Name = "upButton";
            this.toolTip.SetToolTip(this.upButton, resources.GetString("upButton.ToolTip"));
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.AccessibleDescription = null;
            this.downButton.AccessibleName = null;
            resources.ApplyResources(this.downButton, "downButton");
            this.downButton.BackgroundImage = null;
            this.infoProvider.SetError(this.downButton, resources.GetString("downButton.Error"));
            this.downButton.Font = null;
            this.infoProvider.SetIconAlignment(this.downButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("downButton.IconAlignment"))));
            this.infoProvider.SetIconPadding(this.downButton, ((int)(resources.GetObject("downButton.IconPadding"))));
            this.downButton.Name = "downButton";
            this.toolTip.SetToolTip(this.downButton, resources.GetString("downButton.ToolTip"));
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // DefineEnumValuesDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.enumValueListBox);
            this.Controls.Add(this.valueEnterTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.Name = "DefineEnumValuesDialog";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.DefineEnumValuesDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		private void addButton_Click(object sender, System.EventArgs e)
		{
			AddEnumValue();
		}

		private void delButton_Click(object sender, System.EventArgs e)
		{
			// ask user to confirm of deletion if the constraint is referenced
            ClassElement refClass;
            AttributeElementBase refAttribute;
            if (this._enumElement.SchemaModel.IsConstraintReferenced(_enumElement, out refClass, out refAttribute))
			{
                string msg = Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.ConfirmDeleteEnum");
                if (MessageBox.Show(msg,
					"Confirm Dialog", MessageBoxButtons.YesNo,
					MessageBoxIcon.Information) == DialogResult.No)
				{
					return; // do not delete
				}
			}

			if (this.enumValueListBox.SelectedIndex >= 0)
			{
				this._values.RemoveAt(this.enumValueListBox.SelectedIndex);

				ShowEnumValues();
			}

			if (this._values.Count > 0)
			{
				this.enumValueListBox.SelectedIndex = 0; // select the first item
			}
			else
			{
				this.valueTextBox.Text = null;
				this.textEnterTextBox.Text = null;
			}
		}

		private void DefineEnumValuesDialog_Load(object sender, System.EventArgs e)
		{
			if (this._values != null)
			{
				ShowEnumValues();

				if (this._values.Count > 0)
				{
					this.enumValueListBox.SelectedIndex = 0;
				}
			}

			// display help providers to text boxes
			string tip = toolTip.GetToolTip(this.valueEnterTextBox);
			if (tip.Length > 0)
			{
				infoProvider.SetError(this.valueEnterTextBox, tip);
			}

			tip = toolTip.GetToolTip(this.textEnterTextBox);
			if (tip.Length > 0)
			{
				infoProvider.SetError(this.textEnterTextBox, tip);
			}

			this.valueEnterTextBox.Focus();
		}

		private void enumValueListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.enumValueListBox.SelectedIndex >= 0)
			{
				EnumValue selectedEnumValue = _values[this.enumValueListBox.SelectedIndex];
				this.valueTextBox.Text = selectedEnumValue.Value;
				this.textEnterTextBox.Text = selectedEnumValue.DisplayText;
                this.imageNameTextBox.Text = selectedEnumValue.ImageName;
                if (!string.IsNullOrEmpty(selectedEnumValue.ImageName))
                {
                    ImageInfo imageInfo = ImageInfoCache.Instance.GetImageInfo(selectedEnumValue.ImageName);
                    this.imageLabel.Image = imageInfo.Image;
                }
                else
                {
                    this.imageLabel.Image = null;
                }

                this.upButton.Enabled = true;
                this.downButton.Enabled = true;


                if (this.enumValueListBox.SelectedIndex == 0)
                {
                    this.upButton.Enabled = false;
                }

                if (this.enumValueListBox.SelectedIndex == (this.enumValueListBox.Items.Count - 1))
                {
                    this.downButton.Enabled = false;
                }
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
            string msg;

			// make sure that there are no duplicated enum values
			if (_values != null)
			{
				EnumValue enumValue;
				for (int i = 0; i < _values.Count; i++)
				{
					enumValue = _values[i];
					if (this.IsEnumTextExist(enumValue.DisplayText, i))
					{
                        msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.EnumTextExist"), enumValue.DisplayText);
						MessageBox.Show(msg,
							"Error Dialog", MessageBoxButtons.OK,
							MessageBoxIcon.Error);

						this.DialogResult = DialogResult.None;
						return;
					}
                    else if (!IsEnumValueValid(enumValue.DisplayText, i))
                    {
                        msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InvalidEnumValue"), enumValue.DisplayText);

                        MessageBox.Show(msg,
                            "Error Dialog", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    else if (!IsEnumValueLegal(enumValue.DisplayText))
                    {
                        msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.IllegalEnumValue"), enumValue.DisplayText);

                        MessageBox.Show(msg,
                            "Error Dialog", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        this.DialogResult = DialogResult.None;
                        return;
                    }
				}
			}
		}

		private void valueEnterTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.valueEnterTextBox.Text.Length > 0)
			{
				this.addButton.Enabled = true;
			}
			else
			{
				this.addButton.Enabled = false;
			}
		}

		private void valueEnterTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) 
			{
				AddEnumValue();
			}
		}

		private void textEnterTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) 
			{
				if (this.enumValueListBox.SelectedIndex >= 0)
				{
					if (this.textEnterTextBox.Text == null ||
						this.textEnterTextBox.Text.Length == 0)
					{
						MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.NullEnumText"),
							"Error Dialog", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
					else if (IsEnumTextExist(this.textEnterTextBox.Text, -1))
					{
                        string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.EnumTextExist"),
                            this.textEnterTextBox.Text);
                        MessageBox.Show(msg,
							"Error Dialog", MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
					else
					{
						int currentIndex = this.enumValueListBox.SelectedIndex;
						EnumValue selectedEnumValue = this._values[this.enumValueListBox.SelectedIndex];
						selectedEnumValue.DisplayText = this.textEnterTextBox.Text;

						ShowEnumValues();

						this.enumValueListBox.SelectedIndex = currentIndex;
					}
				}
			}
		}

        private void imageButton_Click(object sender, EventArgs e)
        {
            ChooseImageDialog dialog = new ChooseImageDialog();

            dialog.ImageName = this.imageNameTextBox.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (this.enumValueListBox.SelectedIndex >= 0)
                {
                    int currentIndex = this.enumValueListBox.SelectedIndex;
                    EnumValue selectedEnumValue = this._values[this.enumValueListBox.SelectedIndex];
                    selectedEnumValue.ImageName = dialog.SelectedImageInfo.Name;
                    this.imageNameTextBox.Text = dialog.SelectedImageInfo.Name;
                    this.imageLabel.Image = dialog.SelectedImageInfo.Image;
                }
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (this.enumValueListBox.SelectedIndex > 0)
            {
                EnumValue selectedEnumValue = _values[this.enumValueListBox.SelectedIndex];

                _values.RemoveAt(this.enumValueListBox.SelectedIndex);

                _values.Insert(this.enumValueListBox.SelectedIndex - 1, selectedEnumValue);

                int selectedIndex = this.enumValueListBox.SelectedIndex - 1;

                ShowEnumValues();

                this.enumValueListBox.SelectedIndex = selectedIndex;
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (this.enumValueListBox.SelectedIndex < (this.enumValueListBox.Items.Count - 1))
            {
                EnumValue selectedEnumValue = _values[this.enumValueListBox.SelectedIndex];

                _values.RemoveAt(this.enumValueListBox.SelectedIndex);

                _values.Insert(this.enumValueListBox.SelectedIndex + 1, selectedEnumValue);

                int selectedIndex = this.enumValueListBox.SelectedIndex + 1;

                ShowEnumValues();

                this.enumValueListBox.SelectedIndex = selectedIndex;
            }
        }
	}
}
