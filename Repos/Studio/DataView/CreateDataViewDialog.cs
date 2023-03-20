using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Studio.UserControls;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for CreateDataViewDialog.
	/// </summary>
	public class CreateDataViewDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private DataViewModel _dataView;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.Button nextStepButton;
		private System.Windows.Forms.Button prevStepButton;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ErrorProvider infoProvider;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TabPage basicInfoTabPage;
		private System.Windows.Forms.TextBox baseClassTextBox;
		private System.Windows.Forms.TextBox dataViewCaptionTextBox;
		private System.Windows.Forms.TextBox dataViewNameTextBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.GroupBox searchGroupBox;
		private System.Windows.Forms.Button baseClassButton;
		private System.Windows.Forms.TextBox dataViewDescriptionTextBox;
        private System.Windows.Forms.ImageList smallIconImageList;
		private System.Windows.Forms.TabPage searchTabPage;
		private System.Windows.Forms.TabPage resultTabPage;
		private System.Windows.Forms.TabControl dataViewTabControl;
		private Newtera.Studio.UserControls.DataViewResultBuilder dataViewResultBuilder;
        private System.Windows.Forms.Button addRefClassesButton;
        private Panel panel1;
        private Newtera.WindowsControl.SearchExprBuilder searchExprBuilder;
        private TabPage sortTabPage;
        private DataViewSortControl dataViewSortControl;
        private Label msgLabel;
		private System.ComponentModel.IContainer components;

		public CreateDataViewDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            this.searchExprBuilder.MessageLable = this.msgLabel;
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
		/// Gets or sets the meta data model from which to get all
		/// available information needed to specify a relationship
		/// </summary>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
				if (_dataView == null)
				{
					_dataView = new DataViewModel("", _metaData.SchemaInfo, _metaData.SchemaModel);
					this.searchExprBuilder.DataView = _dataView;
					this.dataViewResultBuilder.DataView = _dataView;
                    this.dataViewSortControl.DataView = _dataView;
				}
			}
		}

		/// <summary>
		/// Gets or sets the relationship attribute element that the linked class
		/// is being chosen.
		/// </summary>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				if (value != null)
				{
					_dataView = value;
					this.searchExprBuilder.DataView = value;
					this.dataViewResultBuilder.DataView = value;
                    this.dataViewSortControl.DataView = value;
				}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateDataViewDialog));
            this.nextStepButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.dataViewTabControl = new System.Windows.Forms.TabControl();
            this.basicInfoTabPage = new System.Windows.Forms.TabPage();
            this.baseClassButton = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.dataViewDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.baseClassTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dataViewCaptionTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dataViewNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.searchTabPage = new System.Windows.Forms.TabPage();
            this.searchGroupBox = new System.Windows.Forms.GroupBox();
            this.msgLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchExprBuilder = new Newtera.WindowsControl.SearchExprBuilder();
            this.addRefClassesButton = new System.Windows.Forms.Button();
            this.resultTabPage = new System.Windows.Forms.TabPage();
            this.dataViewResultBuilder = new Newtera.Studio.UserControls.DataViewResultBuilder();
            this.sortTabPage = new System.Windows.Forms.TabPage();
            this.dataViewSortControl = new Newtera.Studio.UserControls.DataViewSortControl();
            this.smallIconImageList = new System.Windows.Forms.ImageList(this.components);
            this.doneButton = new System.Windows.Forms.Button();
            this.prevStepButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.dataViewTabControl.SuspendLayout();
            this.basicInfoTabPage.SuspendLayout();
            this.searchTabPage.SuspendLayout();
            this.searchGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.resultTabPage.SuspendLayout();
            this.sortTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // nextStepButton
            // 
            resources.ApplyResources(this.nextStepButton, "nextStepButton");
            this.nextStepButton.Name = "nextStepButton";
            this.nextStepButton.Click += new System.EventHandler(this.nextStepButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // dataViewTabControl
            // 
            resources.ApplyResources(this.dataViewTabControl, "dataViewTabControl");
            this.dataViewTabControl.Controls.Add(this.basicInfoTabPage);
            this.dataViewTabControl.Controls.Add(this.searchTabPage);
            this.dataViewTabControl.Controls.Add(this.resultTabPage);
            this.dataViewTabControl.Controls.Add(this.sortTabPage);
            this.dataViewTabControl.Name = "dataViewTabControl";
            this.dataViewTabControl.SelectedIndex = 0;
            this.toolTip.SetToolTip(this.dataViewTabControl, resources.GetString("dataViewTabControl.ToolTip"));
            this.dataViewTabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // basicInfoTabPage
            // 
            this.basicInfoTabPage.Controls.Add(this.baseClassButton);
            this.basicInfoTabPage.Controls.Add(this.label11);
            this.basicInfoTabPage.Controls.Add(this.dataViewDescriptionTextBox);
            this.basicInfoTabPage.Controls.Add(this.baseClassTextBox);
            this.basicInfoTabPage.Controls.Add(this.label6);
            this.basicInfoTabPage.Controls.Add(this.dataViewCaptionTextBox);
            this.basicInfoTabPage.Controls.Add(this.label5);
            this.basicInfoTabPage.Controls.Add(this.dataViewNameTextBox);
            this.basicInfoTabPage.Controls.Add(this.label4);
            resources.ApplyResources(this.basicInfoTabPage, "basicInfoTabPage");
            this.basicInfoTabPage.Name = "basicInfoTabPage";
            // 
            // baseClassButton
            // 
            resources.ApplyResources(this.baseClassButton, "baseClassButton");
            this.baseClassButton.Name = "baseClassButton";
            this.baseClassButton.Click += new System.EventHandler(this.baseClassButton_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // dataViewDescriptionTextBox
            // 
            resources.ApplyResources(this.dataViewDescriptionTextBox, "dataViewDescriptionTextBox");
            this.dataViewDescriptionTextBox.Name = "dataViewDescriptionTextBox";
            // 
            // baseClassTextBox
            // 
            resources.ApplyResources(this.baseClassTextBox, "baseClassTextBox");
            this.baseClassTextBox.Name = "baseClassTextBox";
            this.baseClassTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.baseClassTextBox, resources.GetString("baseClassTextBox.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // dataViewCaptionTextBox
            // 
            resources.ApplyResources(this.dataViewCaptionTextBox, "dataViewCaptionTextBox");
            this.dataViewCaptionTextBox.Name = "dataViewCaptionTextBox";
            this.toolTip.SetToolTip(this.dataViewCaptionTextBox, resources.GetString("dataViewCaptionTextBox.ToolTip"));
            this.dataViewCaptionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dataViewCaptionTextBox_Validating);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // dataViewNameTextBox
            // 
            resources.ApplyResources(this.dataViewNameTextBox, "dataViewNameTextBox");
            this.dataViewNameTextBox.Name = "dataViewNameTextBox";
            this.toolTip.SetToolTip(this.dataViewNameTextBox, resources.GetString("dataViewNameTextBox.ToolTip"));
            this.dataViewNameTextBox.TextChanged += new System.EventHandler(this.dataViewNameTextBox_TextChanged);
            this.dataViewNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dataViewNameTextBox_Validating);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // searchTabPage
            // 
            this.searchTabPage.Controls.Add(this.searchGroupBox);
            resources.ApplyResources(this.searchTabPage, "searchTabPage");
            this.searchTabPage.Name = "searchTabPage";
            // 
            // searchGroupBox
            // 
            this.searchGroupBox.Controls.Add(this.msgLabel);
            this.searchGroupBox.Controls.Add(this.panel1);
            this.searchGroupBox.Controls.Add(this.addRefClassesButton);
            resources.ApplyResources(this.searchGroupBox, "searchGroupBox");
            this.searchGroupBox.Name = "searchGroupBox";
            this.searchGroupBox.TabStop = false;
            this.toolTip.SetToolTip(this.searchGroupBox, resources.GetString("searchGroupBox.ToolTip"));
            // 
            // msgLabel
            // 
            resources.ApplyResources(this.msgLabel, "msgLabel");
            this.msgLabel.ForeColor = System.Drawing.Color.Red;
            this.msgLabel.Name = "msgLabel";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.searchExprBuilder);
            this.panel1.Name = "panel1";
            // 
            // searchExprBuilder
            // 
            this.searchExprBuilder.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.searchExprBuilder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.searchExprBuilder.DataView = null;
            resources.ApplyResources(this.searchExprBuilder, "searchExprBuilder");
            this.searchExprBuilder.MessageLable = null;
            this.searchExprBuilder.Name = "searchExprBuilder";
            this.searchExprBuilder.PopupClosed += new System.EventHandler(this.searchExprBuilder_PopupClosed);
            // 
            // addRefClassesButton
            // 
            resources.ApplyResources(this.addRefClassesButton, "addRefClassesButton");
            this.addRefClassesButton.Name = "addRefClassesButton";
            this.addRefClassesButton.Click += new System.EventHandler(this.addRefClassesButton_Click);
            // 
            // resultTabPage
            // 
            this.resultTabPage.Controls.Add(this.dataViewResultBuilder);
            resources.ApplyResources(this.resultTabPage, "resultTabPage");
            this.resultTabPage.Name = "resultTabPage";
            // 
            // dataViewResultBuilder
            // 
            this.dataViewResultBuilder.DataView = null;
            resources.ApplyResources(this.dataViewResultBuilder, "dataViewResultBuilder");
            this.dataViewResultBuilder.Name = "dataViewResultBuilder";
            this.dataViewResultBuilder.ResultAttributesChanged += new System.EventHandler(this.dataViewResultBuilder_ResultAttributesChanged);
            // 
            // sortTabPage
            // 
            this.sortTabPage.Controls.Add(this.dataViewSortControl);
            resources.ApplyResources(this.sortTabPage, "sortTabPage");
            this.sortTabPage.Name = "sortTabPage";
            this.toolTip.SetToolTip(this.sortTabPage, resources.GetString("sortTabPage.ToolTip"));
            this.sortTabPage.UseVisualStyleBackColor = true;
            // 
            // dataViewSortControl
            // 
            this.dataViewSortControl.DataView = null;
            resources.ApplyResources(this.dataViewSortControl, "dataViewSortControl");
            this.dataViewSortControl.Name = "dataViewSortControl";
            // 
            // smallIconImageList
            // 
            this.smallIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallIconImageList.ImageStream")));
            this.smallIconImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallIconImageList.Images.SetKeyName(0, "");
            this.smallIconImageList.Images.SetKeyName(1, "");
            this.smallIconImageList.Images.SetKeyName(2, "");
            this.smallIconImageList.Images.SetKeyName(3, "");
            // 
            // doneButton
            // 
            resources.ApplyResources(this.doneButton, "doneButton");
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.doneButton.Name = "doneButton";
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // prevStepButton
            // 
            resources.ApplyResources(this.prevStepButton, "prevStepButton");
            this.prevStepButton.Name = "prevStepButton";
            this.prevStepButton.Click += new System.EventHandler(this.prevStepButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // infoProvider
            // 
            this.infoProvider.ContainerControl = this;
            resources.ApplyResources(this.infoProvider, "infoProvider");
            // 
            // CreateDataViewDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.prevStepButton);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.dataViewTabControl);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nextStepButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CreateDataViewDialog";
            this.Load += new System.EventHandler(this.CreateDataViewDialog_Load);
            this.dataViewTabControl.ResumeLayout(false);
            this.basicInfoTabPage.ResumeLayout(false);
            this.basicInfoTabPage.PerformLayout();
            this.searchTabPage.ResumeLayout(false);
            this.searchGroupBox.ResumeLayout(false);
            this.searchGroupBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.resultTabPage.ResumeLayout(false);
            this.sortTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private void dataViewNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique
			if (this.dataViewNameTextBox.Text.Length == 0)
			{
                this.errorProvider.SetError(this.dataViewNameTextBox, MessageResourceManager.GetString("SchemaEditor.EnterName"));
				this.infoProvider.SetError(this.dataViewNameTextBox, null);
				e.Cancel = true;
			}
			else if (!ValidateNameUniqueness(this.dataViewNameTextBox.Text))
			{
				this.infoProvider.SetError(this.dataViewNameTextBox, null);
				e.Cancel = true;
			}
			else
			{
				// data binding did not work properly for name and caption
				// box, so we set the values manually
				_dataView.Name = this.dataViewNameTextBox.Text;
				_dataView.Caption = this.dataViewCaptionTextBox.Text;

				string tip = this.toolTip.GetToolTip((Control) sender);
				// show the info when there is text in text box
				this.errorProvider.SetError((Control) sender, null);
				this.infoProvider.SetError((Control) sender, tip);
			}
		}

		private void dataViewCaptionTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the caption cannot be null
			if (this.dataViewCaptionTextBox.Text.Length == 0)
			{
				e.Cancel = true;

                this.errorProvider.SetError(this.dataViewCaptionTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidCaption"));
			}		
		}

		private bool ValidateNameUniqueness(string name)
		{
			bool status = true;

			if (_metaData.DataViews[name] != null)
			{
				status = false;
				this.errorProvider.SetError(this.dataViewNameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateDataViewName"));
			}

			return status;
		}

		#endregion

		private void CreateDataViewDialog_Load(object sender, System.EventArgs e)
		{
			if (_dataView != null)
			{
				// set the data bindings
				this.dataViewNameTextBox.DataBindings.Add("Text", _dataView, "Name");
				this.dataViewCaptionTextBox.DataBindings.Add("Text", _dataView, "Caption");
				this.dataViewDescriptionTextBox.DataBindings.Add("Text", _dataView, "Description");

				// if the base class exist, then this is an existing data view
				// do not allow change the name of the data view
				if (_dataView.BaseClass != null)
				{
					this.dataViewNameTextBox.ReadOnly = true;
					this.dataViewNameTextBox.Validating -= new System.ComponentModel.CancelEventHandler(this.dataViewNameTextBox_Validating);
				}

				// display help providers to some text boxes
				string tip = toolTip.GetToolTip(this.dataViewNameTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.dataViewNameTextBox, tip);
				}

				tip = toolTip.GetToolTip(this.dataViewCaptionTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.dataViewCaptionTextBox, tip);
				}

				tip = toolTip.GetToolTip(this.baseClassTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.baseClassTextBox, tip);
				}

				if (_dataView.BaseClass != null)
				{
					this.baseClassTextBox.Text = _dataView.BaseClass.Caption;
					this.nextStepButton.Enabled = true;
				}

				// display search expression and result attributes of the data view
				this.searchExprBuilder.DisplaySearchExpression();
				this.dataViewResultBuilder.DisplayResultAttributes();
                this.dataViewSortControl.DisplaySortAttributes();
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int tabIndex = this.dataViewTabControl.SelectedIndex;

			// do not allow switching to new tab if the linase class is not selected
			if (_dataView.BaseClass != null)
			{
				if (tabIndex == 0)
				{
					this.prevStepButton.Enabled = false;
					this.nextStepButton.Enabled = true;
				}
				else if (tabIndex == 1 || tabIndex == 2)
				{
					this.prevStepButton.Enabled = true;
					this.nextStepButton.Enabled = true;
				}
				else
				{
					this.prevStepButton.Enabled = true;
					this.nextStepButton.Enabled = false;
				}
			}
			else if (tabIndex > 0)
			{
				// stay in the first tab page
				this.dataViewTabControl.SelectedIndex = 0;
			}
		}

		private void nextStepButton_Click(object sender, System.EventArgs e)
		{
			int tabIndex = this.dataViewTabControl.SelectedIndex;
			if (tabIndex < 3)
			{
				this.dataViewTabControl.SelectedIndex = tabIndex + 1;	
			}
		}

		private void dataViewNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// keep the caption text in sync with the name
			this.dataViewCaptionTextBox.Text = this.dataViewNameTextBox.Text;
		}

		private void prevStepButton_Click(object sender, System.EventArgs e)
		{
			int tabIndex = this.dataViewTabControl.SelectedIndex;
			if (tabIndex > 0)
			{
				this.dataViewTabControl.SelectedIndex = tabIndex - 1;
			}
		}

		private void doneButton_Click(object sender, System.EventArgs e)
		{
		}

		private void baseClassButton_Click(object sender, System.EventArgs e)
		{
			ChooseClassDialog dialog = new ChooseClassDialog();
			dialog.RootClass = "ALL";
			dialog.MetaData = _metaData;
			if (_dataView.BaseClass != null)
			{
				dialog.SelectedClass = (ClassElement) _dataView.BaseClass.GetSchemaModelElement();
			}

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				ClassElement baseClassElement = dialog.SelectedClass;
				DataClass baseClass = new DataClass(baseClassElement.Name,
													baseClassElement.Name,
													DataClassType.BaseClass);
				baseClass.Caption = baseClassElement.Caption;

				if (_dataView.BaseClass == null ||
					_dataView.BaseClass.ClassName != baseClass.ClassName)
				{
					// clear old info for a new base class
					_dataView.ReferencedClasses.Clear();
					_dataView.ResultAttributes.Clear();
					_dataView.ClearSearchExpression();

					// clear the display of search and result panes
					this.searchExprBuilder.DisplaySearchExpression();
					this.dataViewResultBuilder.DisplayResultAttributes();
                    this.dataViewSortControl.DisplaySortAttributes();

					_dataView.BaseClass = baseClass;
					
					this.baseClassTextBox.Text = baseClass.Caption;

					this.nextStepButton.Enabled = true;
				}
			}
		}

		private void dataViewResultBuilder_ResultAttributesChanged(object sender, System.EventArgs e)
		{
			if (_dataView.ResultAttributes.Count > 0)
			{
				this.doneButton.Enabled = true;
			}
			else
			{
				this.doneButton.Enabled = false;
			}
		}

		private void addRefClassesButton_Click(object sender, System.EventArgs e)
		{
			ChooseReferencedClassDialog dialog = new ChooseReferencedClassDialog();
			dialog.DataView = this._dataView;
			dialog.ShowDialog();
		}

        private void searchExprBuilder_PopupClosed(object sender, EventArgs e)
        {
            this.cancelButton.Focus();
        }
	}
}
