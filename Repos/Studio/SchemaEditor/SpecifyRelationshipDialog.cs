using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for SpecifyRelationshipDialog.
	/// </summary>
	public class SpecifyRelationshipDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private RelationshipAttributeElement _relationshipAttribute;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ImageList treeImageList;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TabPage backwardRelationshipTabPage;
		private System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox brNameTextBox;
		private System.Windows.Forms.TextBox brCaptionTextBox;
		private System.Windows.Forms.Button nextStepButton;
		private System.Windows.Forms.Button prevStepButton;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ErrorProvider infoProvider;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.TabPage forwardRelationshipTabPage;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox frNameTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox frCaptionTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox frOwnerTextBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox frTypeComboBox;
		private System.Windows.Forms.CheckBox frJoinManagerCheckBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox frDescriptionTextBox;
		private System.Windows.Forms.TextBox brLinkedClassTextBox;
		private System.Windows.Forms.TextBox brTypeTextBox;
		private System.Windows.Forms.TextBox brOwnerClassTextBox;
		private System.Windows.Forms.TextBox frLinkedClassTextBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox brJoinManagerCheckBox;
		private System.Windows.Forms.TextBox brDescriptionTextBox;
		private System.Windows.Forms.ComboBox frOwnershipComboBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox brOwnershipComboBox;
		private System.ComponentModel.IContainer components;

		public SpecifyRelationshipDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_relationshipAttribute = null;
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
			}
		}

		/// <summary>
		/// Gets or sets the relationship attribute element that the linked class
		/// is being chosen.
		/// </summary>
		public RelationshipAttributeElement RelationshipAttribute
		{
			get
			{
				return _relationshipAttribute;
			}
			set
			{
				_relationshipAttribute = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecifyRelationshipDialog));
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.nextStepButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.forwardRelationshipTabPage = new System.Windows.Forms.TabPage();
            this.frOwnershipComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.frDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.frJoinManagerCheckBox = new System.Windows.Forms.CheckBox();
            this.frTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.frOwnerTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.frCaptionTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.frNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.frLinkedClassTextBox = new System.Windows.Forms.TextBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.backwardRelationshipTabPage = new System.Windows.Forms.TabPage();
            this.brOwnershipComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.brJoinManagerCheckBox = new System.Windows.Forms.CheckBox();
            this.brDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.brLinkedClassTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.brTypeTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.brOwnerClassTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.brCaptionTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.brNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.doneButton = new System.Windows.Forms.Button();
            this.prevStepButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.infoProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.forwardRelationshipTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.backwardRelationshipTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // treeImageList
            // 
            this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImageList.Images.SetKeyName(0, "");
            this.treeImageList.Images.SetKeyName(1, "");
            this.treeImageList.Images.SetKeyName(2, "");
            this.treeImageList.Images.SetKeyName(3, "");
            // 
            // nextStepButton
            // 
            this.nextStepButton.AccessibleDescription = null;
            this.nextStepButton.AccessibleName = null;
            resources.ApplyResources(this.nextStepButton, "nextStepButton");
            this.nextStepButton.BackgroundImage = null;
            this.infoProvider.SetError(this.nextStepButton, resources.GetString("nextStepButton.Error"));
            this.errorProvider.SetError(this.nextStepButton, resources.GetString("nextStepButton.Error1"));
            this.nextStepButton.Font = null;
            this.infoProvider.SetIconAlignment(this.nextStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("nextStepButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.nextStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("nextStepButton.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.nextStepButton, ((int)(resources.GetObject("nextStepButton.IconPadding"))));
            this.errorProvider.SetIconPadding(this.nextStepButton, ((int)(resources.GetObject("nextStepButton.IconPadding1"))));
            this.nextStepButton.Name = "nextStepButton";
            this.toolTip.SetToolTip(this.nextStepButton, resources.GetString("nextStepButton.ToolTip"));
            this.nextStepButton.Click += new System.EventHandler(this.nextStepButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.infoProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error"));
            this.errorProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error1"));
            this.cancelButton.Font = null;
            this.errorProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding"))));
            this.errorProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding1"))));
            this.cancelButton.Name = "cancelButton";
            this.toolTip.SetToolTip(this.cancelButton, resources.GetString("cancelButton.ToolTip"));
            // 
            // tabControl1
            // 
            this.tabControl1.AccessibleDescription = null;
            this.tabControl1.AccessibleName = null;
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.BackgroundImage = null;
            this.tabControl1.Controls.Add(this.forwardRelationshipTabPage);
            this.tabControl1.Controls.Add(this.backwardRelationshipTabPage);
            this.infoProvider.SetError(this.tabControl1, resources.GetString("tabControl1.Error"));
            this.errorProvider.SetError(this.tabControl1, resources.GetString("tabControl1.Error1"));
            this.tabControl1.Font = null;
            this.errorProvider.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.tabControl1, ((int)(resources.GetObject("tabControl1.IconPadding"))));
            this.infoProvider.SetIconPadding(this.tabControl1, ((int)(resources.GetObject("tabControl1.IconPadding1"))));
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // forwardRelationshipTabPage
            // 
            this.forwardRelationshipTabPage.AccessibleDescription = null;
            this.forwardRelationshipTabPage.AccessibleName = null;
            resources.ApplyResources(this.forwardRelationshipTabPage, "forwardRelationshipTabPage");
            this.forwardRelationshipTabPage.BackgroundImage = null;
            this.forwardRelationshipTabPage.Controls.Add(this.frOwnershipComboBox);
            this.forwardRelationshipTabPage.Controls.Add(this.label12);
            this.forwardRelationshipTabPage.Controls.Add(this.frDescriptionTextBox);
            this.forwardRelationshipTabPage.Controls.Add(this.label11);
            this.forwardRelationshipTabPage.Controls.Add(this.frJoinManagerCheckBox);
            this.forwardRelationshipTabPage.Controls.Add(this.frTypeComboBox);
            this.forwardRelationshipTabPage.Controls.Add(this.label10);
            this.forwardRelationshipTabPage.Controls.Add(this.frOwnerTextBox);
            this.forwardRelationshipTabPage.Controls.Add(this.label6);
            this.forwardRelationshipTabPage.Controls.Add(this.frCaptionTextBox);
            this.forwardRelationshipTabPage.Controls.Add(this.label5);
            this.forwardRelationshipTabPage.Controls.Add(this.frNameTextBox);
            this.forwardRelationshipTabPage.Controls.Add(this.label4);
            this.forwardRelationshipTabPage.Controls.Add(this.groupBox1);
            this.infoProvider.SetError(this.forwardRelationshipTabPage, resources.GetString("forwardRelationshipTabPage.Error"));
            this.errorProvider.SetError(this.forwardRelationshipTabPage, resources.GetString("forwardRelationshipTabPage.Error1"));
            this.forwardRelationshipTabPage.Font = null;
            this.errorProvider.SetIconAlignment(this.forwardRelationshipTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("forwardRelationshipTabPage.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.forwardRelationshipTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("forwardRelationshipTabPage.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.forwardRelationshipTabPage, ((int)(resources.GetObject("forwardRelationshipTabPage.IconPadding"))));
            this.infoProvider.SetIconPadding(this.forwardRelationshipTabPage, ((int)(resources.GetObject("forwardRelationshipTabPage.IconPadding1"))));
            this.forwardRelationshipTabPage.Name = "forwardRelationshipTabPage";
            this.toolTip.SetToolTip(this.forwardRelationshipTabPage, resources.GetString("forwardRelationshipTabPage.ToolTip"));
            // 
            // frOwnershipComboBox
            // 
            this.frOwnershipComboBox.AccessibleDescription = null;
            this.frOwnershipComboBox.AccessibleName = null;
            resources.ApplyResources(this.frOwnershipComboBox, "frOwnershipComboBox");
            this.frOwnershipComboBox.BackgroundImage = null;
            this.errorProvider.SetError(this.frOwnershipComboBox, resources.GetString("frOwnershipComboBox.Error"));
            this.infoProvider.SetError(this.frOwnershipComboBox, resources.GetString("frOwnershipComboBox.Error1"));
            this.frOwnershipComboBox.Font = null;
            this.errorProvider.SetIconAlignment(this.frOwnershipComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frOwnershipComboBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.frOwnershipComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frOwnershipComboBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.frOwnershipComboBox, ((int)(resources.GetObject("frOwnershipComboBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.frOwnershipComboBox, ((int)(resources.GetObject("frOwnershipComboBox.IconPadding1"))));
            this.frOwnershipComboBox.Name = "frOwnershipComboBox";
            this.toolTip.SetToolTip(this.frOwnershipComboBox, resources.GetString("frOwnershipComboBox.ToolTip"));
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.infoProvider.SetError(this.label12, resources.GetString("label12.Error"));
            this.errorProvider.SetError(this.label12, resources.GetString("label12.Error1"));
            this.label12.Font = null;
            this.infoProvider.SetIconAlignment(this.label12, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label12.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label12, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label12.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label12, ((int)(resources.GetObject("label12.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label12, ((int)(resources.GetObject("label12.IconPadding1"))));
            this.label12.Name = "label12";
            this.toolTip.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // frDescriptionTextBox
            // 
            this.frDescriptionTextBox.AccessibleDescription = null;
            this.frDescriptionTextBox.AccessibleName = null;
            resources.ApplyResources(this.frDescriptionTextBox, "frDescriptionTextBox");
            this.frDescriptionTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.frDescriptionTextBox, resources.GetString("frDescriptionTextBox.Error"));
            this.errorProvider.SetError(this.frDescriptionTextBox, resources.GetString("frDescriptionTextBox.Error1"));
            this.frDescriptionTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.frDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frDescriptionTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.frDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frDescriptionTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.frDescriptionTextBox, ((int)(resources.GetObject("frDescriptionTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.frDescriptionTextBox, ((int)(resources.GetObject("frDescriptionTextBox.IconPadding1"))));
            this.frDescriptionTextBox.Name = "frDescriptionTextBox";
            this.toolTip.SetToolTip(this.frDescriptionTextBox, resources.GetString("frDescriptionTextBox.ToolTip"));
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.infoProvider.SetError(this.label11, resources.GetString("label11.Error"));
            this.errorProvider.SetError(this.label11, resources.GetString("label11.Error1"));
            this.label11.Font = null;
            this.infoProvider.SetIconAlignment(this.label11, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label11.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label11, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label11.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label11, ((int)(resources.GetObject("label11.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label11, ((int)(resources.GetObject("label11.IconPadding1"))));
            this.label11.Name = "label11";
            this.toolTip.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // frJoinManagerCheckBox
            // 
            this.frJoinManagerCheckBox.AccessibleDescription = null;
            this.frJoinManagerCheckBox.AccessibleName = null;
            resources.ApplyResources(this.frJoinManagerCheckBox, "frJoinManagerCheckBox");
            this.frJoinManagerCheckBox.BackgroundImage = null;
            this.infoProvider.SetError(this.frJoinManagerCheckBox, resources.GetString("frJoinManagerCheckBox.Error"));
            this.errorProvider.SetError(this.frJoinManagerCheckBox, resources.GetString("frJoinManagerCheckBox.Error1"));
            this.frJoinManagerCheckBox.Font = null;
            this.infoProvider.SetIconAlignment(this.frJoinManagerCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frJoinManagerCheckBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.frJoinManagerCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frJoinManagerCheckBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.frJoinManagerCheckBox, ((int)(resources.GetObject("frJoinManagerCheckBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.frJoinManagerCheckBox, ((int)(resources.GetObject("frJoinManagerCheckBox.IconPadding1"))));
            this.frJoinManagerCheckBox.Name = "frJoinManagerCheckBox";
            this.toolTip.SetToolTip(this.frJoinManagerCheckBox, resources.GetString("frJoinManagerCheckBox.ToolTip"));
            // 
            // frTypeComboBox
            // 
            this.frTypeComboBox.AccessibleDescription = null;
            this.frTypeComboBox.AccessibleName = null;
            resources.ApplyResources(this.frTypeComboBox, "frTypeComboBox");
            this.frTypeComboBox.BackgroundImage = null;
            this.errorProvider.SetError(this.frTypeComboBox, resources.GetString("frTypeComboBox.Error"));
            this.infoProvider.SetError(this.frTypeComboBox, resources.GetString("frTypeComboBox.Error1"));
            this.frTypeComboBox.Font = null;
            this.errorProvider.SetIconAlignment(this.frTypeComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frTypeComboBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.frTypeComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frTypeComboBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.frTypeComboBox, ((int)(resources.GetObject("frTypeComboBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.frTypeComboBox, ((int)(resources.GetObject("frTypeComboBox.IconPadding1"))));
            this.frTypeComboBox.Name = "frTypeComboBox";
            this.toolTip.SetToolTip(this.frTypeComboBox, resources.GetString("frTypeComboBox.ToolTip"));
            this.frTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.frTypeComboBox_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AccessibleDescription = null;
            this.label10.AccessibleName = null;
            resources.ApplyResources(this.label10, "label10");
            this.infoProvider.SetError(this.label10, resources.GetString("label10.Error"));
            this.errorProvider.SetError(this.label10, resources.GetString("label10.Error1"));
            this.label10.Font = null;
            this.infoProvider.SetIconAlignment(this.label10, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label10.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label10, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label10.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label10, ((int)(resources.GetObject("label10.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label10, ((int)(resources.GetObject("label10.IconPadding1"))));
            this.label10.Name = "label10";
            this.toolTip.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // frOwnerTextBox
            // 
            this.frOwnerTextBox.AccessibleDescription = null;
            this.frOwnerTextBox.AccessibleName = null;
            resources.ApplyResources(this.frOwnerTextBox, "frOwnerTextBox");
            this.frOwnerTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.frOwnerTextBox, resources.GetString("frOwnerTextBox.Error"));
            this.errorProvider.SetError(this.frOwnerTextBox, resources.GetString("frOwnerTextBox.Error1"));
            this.frOwnerTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.frOwnerTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frOwnerTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.frOwnerTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frOwnerTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.frOwnerTextBox, ((int)(resources.GetObject("frOwnerTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.frOwnerTextBox, ((int)(resources.GetObject("frOwnerTextBox.IconPadding1"))));
            this.frOwnerTextBox.Name = "frOwnerTextBox";
            this.frOwnerTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.frOwnerTextBox, resources.GetString("frOwnerTextBox.ToolTip"));
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.infoProvider.SetError(this.label6, resources.GetString("label6.Error"));
            this.errorProvider.SetError(this.label6, resources.GetString("label6.Error1"));
            this.label6.Font = null;
            this.infoProvider.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding1"))));
            this.label6.Name = "label6";
            this.toolTip.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // frCaptionTextBox
            // 
            this.frCaptionTextBox.AccessibleDescription = null;
            this.frCaptionTextBox.AccessibleName = null;
            resources.ApplyResources(this.frCaptionTextBox, "frCaptionTextBox");
            this.frCaptionTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.frCaptionTextBox, resources.GetString("frCaptionTextBox.Error"));
            this.errorProvider.SetError(this.frCaptionTextBox, resources.GetString("frCaptionTextBox.Error1"));
            this.frCaptionTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.frCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frCaptionTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.frCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frCaptionTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.frCaptionTextBox, ((int)(resources.GetObject("frCaptionTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.frCaptionTextBox, ((int)(resources.GetObject("frCaptionTextBox.IconPadding1"))));
            this.frCaptionTextBox.Name = "frCaptionTextBox";
            this.toolTip.SetToolTip(this.frCaptionTextBox, resources.GetString("frCaptionTextBox.ToolTip"));
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.infoProvider.SetError(this.label5, resources.GetString("label5.Error"));
            this.errorProvider.SetError(this.label5, resources.GetString("label5.Error1"));
            this.label5.Font = null;
            this.infoProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding1"))));
            this.label5.Name = "label5";
            this.toolTip.SetToolTip(this.label5, resources.GetString("label5.ToolTip"));
            // 
            // frNameTextBox
            // 
            this.frNameTextBox.AccessibleDescription = null;
            this.frNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.frNameTextBox, "frNameTextBox");
            this.frNameTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.frNameTextBox, resources.GetString("frNameTextBox.Error"));
            this.errorProvider.SetError(this.frNameTextBox, resources.GetString("frNameTextBox.Error1"));
            this.frNameTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.frNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frNameTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.frNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frNameTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.frNameTextBox, ((int)(resources.GetObject("frNameTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.frNameTextBox, ((int)(resources.GetObject("frNameTextBox.IconPadding1"))));
            this.frNameTextBox.Name = "frNameTextBox";
            this.frNameTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.frNameTextBox, resources.GetString("frNameTextBox.ToolTip"));
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.infoProvider.SetError(this.label4, resources.GetString("label4.Error"));
            this.errorProvider.SetError(this.label4, resources.GetString("label4.Error1"));
            this.label4.Font = null;
            this.infoProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding1"))));
            this.label4.Name = "label4";
            this.toolTip.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.frLinkedClassTextBox);
            this.groupBox1.Controls.Add(this.treeView);
            this.infoProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this.errorProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error1"));
            this.groupBox1.Font = null;
            this.infoProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.errorProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding1"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // frLinkedClassTextBox
            // 
            this.frLinkedClassTextBox.AccessibleDescription = null;
            this.frLinkedClassTextBox.AccessibleName = null;
            resources.ApplyResources(this.frLinkedClassTextBox, "frLinkedClassTextBox");
            this.frLinkedClassTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.frLinkedClassTextBox, resources.GetString("frLinkedClassTextBox.Error"));
            this.errorProvider.SetError(this.frLinkedClassTextBox, resources.GetString("frLinkedClassTextBox.Error1"));
            this.frLinkedClassTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.frLinkedClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frLinkedClassTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.frLinkedClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("frLinkedClassTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.frLinkedClassTextBox, ((int)(resources.GetObject("frLinkedClassTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.frLinkedClassTextBox, ((int)(resources.GetObject("frLinkedClassTextBox.IconPadding1"))));
            this.frLinkedClassTextBox.Name = "frLinkedClassTextBox";
            this.frLinkedClassTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.frLinkedClassTextBox, resources.GetString("frLinkedClassTextBox.ToolTip"));
            // 
            // treeView
            // 
            this.treeView.AccessibleDescription = null;
            this.treeView.AccessibleName = null;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.BackgroundImage = null;
            this.errorProvider.SetError(this.treeView, resources.GetString("treeView.Error"));
            this.infoProvider.SetError(this.treeView, resources.GetString("treeView.Error1"));
            this.treeView.Font = null;
            this.treeView.HideSelection = false;
            this.infoProvider.SetIconAlignment(this.treeView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("treeView.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.treeView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("treeView.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.treeView, ((int)(resources.GetObject("treeView.IconPadding"))));
            this.infoProvider.SetIconPadding(this.treeView, ((int)(resources.GetObject("treeView.IconPadding1"))));
            this.treeView.ImageList = this.treeImageList;
            this.treeView.ItemHeight = 16;
            this.treeView.Name = "treeView";
            this.toolTip.SetToolTip(this.treeView, resources.GetString("treeView.ToolTip"));
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // backwardRelationshipTabPage
            // 
            this.backwardRelationshipTabPage.AccessibleDescription = null;
            this.backwardRelationshipTabPage.AccessibleName = null;
            resources.ApplyResources(this.backwardRelationshipTabPage, "backwardRelationshipTabPage");
            this.backwardRelationshipTabPage.BackgroundImage = null;
            this.backwardRelationshipTabPage.Controls.Add(this.brOwnershipComboBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label13);
            this.backwardRelationshipTabPage.Controls.Add(this.brJoinManagerCheckBox);
            this.backwardRelationshipTabPage.Controls.Add(this.brDescriptionTextBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label9);
            this.backwardRelationshipTabPage.Controls.Add(this.brLinkedClassTextBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label8);
            this.backwardRelationshipTabPage.Controls.Add(this.brTypeTextBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label7);
            this.backwardRelationshipTabPage.Controls.Add(this.brOwnerClassTextBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label3);
            this.backwardRelationshipTabPage.Controls.Add(this.brCaptionTextBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label2);
            this.backwardRelationshipTabPage.Controls.Add(this.brNameTextBox);
            this.backwardRelationshipTabPage.Controls.Add(this.label1);
            this.infoProvider.SetError(this.backwardRelationshipTabPage, resources.GetString("backwardRelationshipTabPage.Error"));
            this.errorProvider.SetError(this.backwardRelationshipTabPage, resources.GetString("backwardRelationshipTabPage.Error1"));
            this.backwardRelationshipTabPage.Font = null;
            this.errorProvider.SetIconAlignment(this.backwardRelationshipTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("backwardRelationshipTabPage.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.backwardRelationshipTabPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("backwardRelationshipTabPage.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.backwardRelationshipTabPage, ((int)(resources.GetObject("backwardRelationshipTabPage.IconPadding"))));
            this.infoProvider.SetIconPadding(this.backwardRelationshipTabPage, ((int)(resources.GetObject("backwardRelationshipTabPage.IconPadding1"))));
            this.backwardRelationshipTabPage.Name = "backwardRelationshipTabPage";
            this.toolTip.SetToolTip(this.backwardRelationshipTabPage, resources.GetString("backwardRelationshipTabPage.ToolTip"));
            // 
            // brOwnershipComboBox
            // 
            this.brOwnershipComboBox.AccessibleDescription = null;
            this.brOwnershipComboBox.AccessibleName = null;
            resources.ApplyResources(this.brOwnershipComboBox, "brOwnershipComboBox");
            this.brOwnershipComboBox.BackgroundImage = null;
            this.errorProvider.SetError(this.brOwnershipComboBox, resources.GetString("brOwnershipComboBox.Error"));
            this.infoProvider.SetError(this.brOwnershipComboBox, resources.GetString("brOwnershipComboBox.Error1"));
            this.brOwnershipComboBox.Font = null;
            this.errorProvider.SetIconAlignment(this.brOwnershipComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brOwnershipComboBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.brOwnershipComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brOwnershipComboBox.IconAlignment1"))));
            this.errorProvider.SetIconPadding(this.brOwnershipComboBox, ((int)(resources.GetObject("brOwnershipComboBox.IconPadding"))));
            this.infoProvider.SetIconPadding(this.brOwnershipComboBox, ((int)(resources.GetObject("brOwnershipComboBox.IconPadding1"))));
            this.brOwnershipComboBox.Name = "brOwnershipComboBox";
            this.toolTip.SetToolTip(this.brOwnershipComboBox, resources.GetString("brOwnershipComboBox.ToolTip"));
            // 
            // label13
            // 
            this.label13.AccessibleDescription = null;
            this.label13.AccessibleName = null;
            resources.ApplyResources(this.label13, "label13");
            this.errorProvider.SetError(this.label13, resources.GetString("label13.Error"));
            this.infoProvider.SetError(this.label13, resources.GetString("label13.Error1"));
            this.label13.Font = null;
            this.infoProvider.SetIconAlignment(this.label13, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label13.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label13, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label13.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label13, ((int)(resources.GetObject("label13.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label13, ((int)(resources.GetObject("label13.IconPadding1"))));
            this.label13.Name = "label13";
            this.toolTip.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // brJoinManagerCheckBox
            // 
            this.brJoinManagerCheckBox.AccessibleDescription = null;
            this.brJoinManagerCheckBox.AccessibleName = null;
            resources.ApplyResources(this.brJoinManagerCheckBox, "brJoinManagerCheckBox");
            this.brJoinManagerCheckBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brJoinManagerCheckBox, resources.GetString("brJoinManagerCheckBox.Error"));
            this.errorProvider.SetError(this.brJoinManagerCheckBox, resources.GetString("brJoinManagerCheckBox.Error1"));
            this.brJoinManagerCheckBox.Font = null;
            this.infoProvider.SetIconAlignment(this.brJoinManagerCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brJoinManagerCheckBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.brJoinManagerCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brJoinManagerCheckBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brJoinManagerCheckBox, ((int)(resources.GetObject("brJoinManagerCheckBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brJoinManagerCheckBox, ((int)(resources.GetObject("brJoinManagerCheckBox.IconPadding1"))));
            this.brJoinManagerCheckBox.Name = "brJoinManagerCheckBox";
            this.toolTip.SetToolTip(this.brJoinManagerCheckBox, resources.GetString("brJoinManagerCheckBox.ToolTip"));
            // 
            // brDescriptionTextBox
            // 
            this.brDescriptionTextBox.AccessibleDescription = null;
            this.brDescriptionTextBox.AccessibleName = null;
            resources.ApplyResources(this.brDescriptionTextBox, "brDescriptionTextBox");
            this.brDescriptionTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brDescriptionTextBox, resources.GetString("brDescriptionTextBox.Error"));
            this.errorProvider.SetError(this.brDescriptionTextBox, resources.GetString("brDescriptionTextBox.Error1"));
            this.brDescriptionTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.brDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brDescriptionTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.brDescriptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brDescriptionTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brDescriptionTextBox, ((int)(resources.GetObject("brDescriptionTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brDescriptionTextBox, ((int)(resources.GetObject("brDescriptionTextBox.IconPadding1"))));
            this.brDescriptionTextBox.Name = "brDescriptionTextBox";
            this.toolTip.SetToolTip(this.brDescriptionTextBox, resources.GetString("brDescriptionTextBox.ToolTip"));
            // 
            // label9
            // 
            this.label9.AccessibleDescription = null;
            this.label9.AccessibleName = null;
            resources.ApplyResources(this.label9, "label9");
            this.infoProvider.SetError(this.label9, resources.GetString("label9.Error"));
            this.errorProvider.SetError(this.label9, resources.GetString("label9.Error1"));
            this.label9.Font = null;
            this.infoProvider.SetIconAlignment(this.label9, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label9.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label9, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label9.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label9, ((int)(resources.GetObject("label9.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label9, ((int)(resources.GetObject("label9.IconPadding1"))));
            this.label9.Name = "label9";
            this.toolTip.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
            // 
            // brLinkedClassTextBox
            // 
            this.brLinkedClassTextBox.AccessibleDescription = null;
            this.brLinkedClassTextBox.AccessibleName = null;
            resources.ApplyResources(this.brLinkedClassTextBox, "brLinkedClassTextBox");
            this.brLinkedClassTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brLinkedClassTextBox, resources.GetString("brLinkedClassTextBox.Error"));
            this.errorProvider.SetError(this.brLinkedClassTextBox, resources.GetString("brLinkedClassTextBox.Error1"));
            this.brLinkedClassTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.brLinkedClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brLinkedClassTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.brLinkedClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brLinkedClassTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brLinkedClassTextBox, ((int)(resources.GetObject("brLinkedClassTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brLinkedClassTextBox, ((int)(resources.GetObject("brLinkedClassTextBox.IconPadding1"))));
            this.brLinkedClassTextBox.Name = "brLinkedClassTextBox";
            this.brLinkedClassTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.brLinkedClassTextBox, resources.GetString("brLinkedClassTextBox.ToolTip"));
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.errorProvider.SetError(this.label8, resources.GetString("label8.Error"));
            this.infoProvider.SetError(this.label8, resources.GetString("label8.Error1"));
            this.label8.Font = null;
            this.infoProvider.SetIconAlignment(this.label8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label8.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label8.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label8, ((int)(resources.GetObject("label8.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label8, ((int)(resources.GetObject("label8.IconPadding1"))));
            this.label8.Name = "label8";
            this.toolTip.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
            // 
            // brTypeTextBox
            // 
            this.brTypeTextBox.AccessibleDescription = null;
            this.brTypeTextBox.AccessibleName = null;
            resources.ApplyResources(this.brTypeTextBox, "brTypeTextBox");
            this.brTypeTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brTypeTextBox, resources.GetString("brTypeTextBox.Error"));
            this.errorProvider.SetError(this.brTypeTextBox, resources.GetString("brTypeTextBox.Error1"));
            this.brTypeTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.brTypeTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brTypeTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.brTypeTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brTypeTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brTypeTextBox, ((int)(resources.GetObject("brTypeTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brTypeTextBox, ((int)(resources.GetObject("brTypeTextBox.IconPadding1"))));
            this.brTypeTextBox.Name = "brTypeTextBox";
            this.brTypeTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.brTypeTextBox, resources.GetString("brTypeTextBox.ToolTip"));
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.infoProvider.SetError(this.label7, resources.GetString("label7.Error"));
            this.errorProvider.SetError(this.label7, resources.GetString("label7.Error1"));
            this.label7.Font = null;
            this.infoProvider.SetIconAlignment(this.label7, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label7.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label7, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label7.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label7, ((int)(resources.GetObject("label7.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label7, ((int)(resources.GetObject("label7.IconPadding1"))));
            this.label7.Name = "label7";
            this.toolTip.SetToolTip(this.label7, resources.GetString("label7.ToolTip"));
            // 
            // brOwnerClassTextBox
            // 
            this.brOwnerClassTextBox.AccessibleDescription = null;
            this.brOwnerClassTextBox.AccessibleName = null;
            resources.ApplyResources(this.brOwnerClassTextBox, "brOwnerClassTextBox");
            this.brOwnerClassTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brOwnerClassTextBox, resources.GetString("brOwnerClassTextBox.Error"));
            this.errorProvider.SetError(this.brOwnerClassTextBox, resources.GetString("brOwnerClassTextBox.Error1"));
            this.brOwnerClassTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.brOwnerClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brOwnerClassTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.brOwnerClassTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brOwnerClassTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brOwnerClassTextBox, ((int)(resources.GetObject("brOwnerClassTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brOwnerClassTextBox, ((int)(resources.GetObject("brOwnerClassTextBox.IconPadding1"))));
            this.brOwnerClassTextBox.Name = "brOwnerClassTextBox";
            this.brOwnerClassTextBox.ReadOnly = true;
            this.toolTip.SetToolTip(this.brOwnerClassTextBox, resources.GetString("brOwnerClassTextBox.ToolTip"));
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.infoProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error1"));
            this.label3.Font = null;
            this.infoProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding1"))));
            this.label3.Name = "label3";
            this.toolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // brCaptionTextBox
            // 
            this.brCaptionTextBox.AccessibleDescription = null;
            this.brCaptionTextBox.AccessibleName = null;
            resources.ApplyResources(this.brCaptionTextBox, "brCaptionTextBox");
            this.brCaptionTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brCaptionTextBox, resources.GetString("brCaptionTextBox.Error"));
            this.errorProvider.SetError(this.brCaptionTextBox, resources.GetString("brCaptionTextBox.Error1"));
            this.brCaptionTextBox.Font = null;
            this.errorProvider.SetIconAlignment(this.brCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brCaptionTextBox.IconAlignment"))));
            this.infoProvider.SetIconAlignment(this.brCaptionTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brCaptionTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brCaptionTextBox, ((int)(resources.GetObject("brCaptionTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brCaptionTextBox, ((int)(resources.GetObject("brCaptionTextBox.IconPadding1"))));
            this.brCaptionTextBox.Name = "brCaptionTextBox";
            this.toolTip.SetToolTip(this.brCaptionTextBox, resources.GetString("brCaptionTextBox.ToolTip"));
            this.brCaptionTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.brCaptionTextBox_Validating);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.infoProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error1"));
            this.label2.Font = null;
            this.infoProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding1"))));
            this.label2.Name = "label2";
            this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // brNameTextBox
            // 
            this.brNameTextBox.AccessibleDescription = null;
            this.brNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.brNameTextBox, "brNameTextBox");
            this.brNameTextBox.BackgroundImage = null;
            this.infoProvider.SetError(this.brNameTextBox, resources.GetString("brNameTextBox.Error"));
            this.errorProvider.SetError(this.brNameTextBox, resources.GetString("brNameTextBox.Error1"));
            this.brNameTextBox.Font = null;
            this.infoProvider.SetIconAlignment(this.brNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brNameTextBox.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.brNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brNameTextBox.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.brNameTextBox, ((int)(resources.GetObject("brNameTextBox.IconPadding"))));
            this.errorProvider.SetIconPadding(this.brNameTextBox, ((int)(resources.GetObject("brNameTextBox.IconPadding1"))));
            this.brNameTextBox.Name = "brNameTextBox";
            this.toolTip.SetToolTip(this.brNameTextBox, resources.GetString("brNameTextBox.ToolTip"));
            this.brNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.brNameTextBox_Validating);
            this.brNameTextBox.TextChanged += new System.EventHandler(this.brNameTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.infoProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error1"));
            this.label1.Font = null;
            this.infoProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding1"))));
            this.label1.Name = "label1";
            this.toolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // doneButton
            // 
            this.doneButton.AccessibleDescription = null;
            this.doneButton.AccessibleName = null;
            resources.ApplyResources(this.doneButton, "doneButton");
            this.doneButton.BackgroundImage = null;
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.infoProvider.SetError(this.doneButton, resources.GetString("doneButton.Error"));
            this.errorProvider.SetError(this.doneButton, resources.GetString("doneButton.Error1"));
            this.doneButton.Font = null;
            this.infoProvider.SetIconAlignment(this.doneButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("doneButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.doneButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("doneButton.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.doneButton, ((int)(resources.GetObject("doneButton.IconPadding"))));
            this.errorProvider.SetIconPadding(this.doneButton, ((int)(resources.GetObject("doneButton.IconPadding1"))));
            this.doneButton.Name = "doneButton";
            this.toolTip.SetToolTip(this.doneButton, resources.GetString("doneButton.ToolTip"));
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // prevStepButton
            // 
            this.prevStepButton.AccessibleDescription = null;
            this.prevStepButton.AccessibleName = null;
            resources.ApplyResources(this.prevStepButton, "prevStepButton");
            this.prevStepButton.BackgroundImage = null;
            this.infoProvider.SetError(this.prevStepButton, resources.GetString("prevStepButton.Error"));
            this.errorProvider.SetError(this.prevStepButton, resources.GetString("prevStepButton.Error1"));
            this.prevStepButton.Font = null;
            this.infoProvider.SetIconAlignment(this.prevStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("prevStepButton.IconAlignment"))));
            this.errorProvider.SetIconAlignment(this.prevStepButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("prevStepButton.IconAlignment1"))));
            this.infoProvider.SetIconPadding(this.prevStepButton, ((int)(resources.GetObject("prevStepButton.IconPadding"))));
            this.errorProvider.SetIconPadding(this.prevStepButton, ((int)(resources.GetObject("prevStepButton.IconPadding1"))));
            this.prevStepButton.Name = "prevStepButton";
            this.toolTip.SetToolTip(this.prevStepButton, resources.GetString("prevStepButton.ToolTip"));
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
            // SpecifyRelationshipDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.prevStepButton);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.nextStepButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = null;
            this.Name = "SpecifyRelationshipDialog";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.SpecifyRelationshipDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.forwardRelationshipTabPage.ResumeLayout(false);
            this.forwardRelationshipTabPage.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.backwardRelationshipTabPage.ResumeLayout(false);
            this.backwardRelationshipTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private void brNameTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the name cannot be null and has to be unique
			if (this.brNameTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.brNameTextBox, MessageResourceManager.GetString("SchemaEditor.EnterName"));
				this.infoProvider.SetError(this.brNameTextBox, null);
				e.Cancel = true;
			}
            else if (!ValidateNameString(this.brNameTextBox.Text))
            {
                this.errorProvider.SetError(this.brNameTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidName"));
                this.infoProvider.SetError(this.brNameTextBox, null);
                e.Cancel = true;
            }
			else if (!ValidateNameUniqueness(this.brNameTextBox.Text))
			{
				this.infoProvider.SetError(this.brNameTextBox, null);
				e.Cancel = true;
			}
            else if (IsReservedName(this.brNameTextBox.Text))
            {
                string msg = String.Format(MessageResourceManager.GetString("SchemaEditor.ReservedWord"), this.brNameTextBox.Text);
                this.errorProvider.SetError(this.brNameTextBox, msg);
                this.infoProvider.SetError(this.brNameTextBox, null);
                e.Cancel = true;
            }
            else if (IsClassName(_relationshipAttribute.LinkedClass, this.brNameTextBox.Text))
            {
                this.errorProvider.SetError(this.brNameTextBox, MessageResourceManager.GetString("SchemaEditor.NameTakenByClass"));
                this.infoProvider.SetError(this.brNameTextBox, null);
                e.Cancel = true;
            }
			else
			{
				string tip = this.toolTip.GetToolTip((Control) sender);
				// show the info when there is text in text box
				this.errorProvider.SetError((Control) sender, null);
				this.infoProvider.SetError((Control) sender, tip);
			}
		}

		private void brCaptionTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the caption cannot be null
			if (this.brCaptionTextBox.Text.Length == 0)
			{
				e.Cancel = true;

                this.errorProvider.SetError(this.brCaptionTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidCaption"));
			}		
		}

		private bool ValidateNameUniqueness(string name)
		{
			bool status = true;

			if (_relationshipAttribute.LinkedClass.FindAttribute(name, SearchDirection.TwoWay) != null)
			{
				status = false;
                this.errorProvider.SetError(this.brNameTextBox, MessageResourceManager.GetString("SchemaEditor.DulicateRelationshipName"));
			}
            else if (_relationshipAttribute.Name == name)
            {
                status = false;
                this.errorProvider.SetError(this.brNameTextBox, MessageResourceManager.GetString("SchemaEditor.InvalidRelationshipName"));
            }

			return status;
		}

        private bool ValidateNameString(string name)
        {
            Regex regex = new Regex(@"^[a-zA-Z]+[0-9]*[a-zA-Z]*[0-9]*$");

            bool status = regex.IsMatch(name);

            return status;
        }

        private bool IsReservedName(string name)
        {
            bool status = false;

            if (name == "type" || name == "obj_id" || name == "attachments" || name == "permission")
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the given name is used as class name.
        /// </summary>
        /// <param name="ownerClass">The owner class</param>
        /// <param name="name">The name</param>
        /// <returns>true if it is a class name, false otherwise</returns>
        private bool IsClassName(ClassElement ownerClass, string name)
        {
            bool status = false;

            ClassElement theClass = ownerClass;
            while (theClass != null)
            {
                if (theClass.Name == name)
                {
                    status = true;
                    break;
                }

                theClass = theClass.ParentClass;
            }

            return status;
        }

		/// <summary>
		/// Gets the relationship type of the backward relationship type according to
		/// the forward relationship type
		/// </summary>
		/// <param name="relationshipType">The forward relationship type</param>
		/// <returns>The backward relationship type</returns>
		private RelationshipType GetBackwardRelationshipType(RelationshipType relationshipType)
		{
			RelationshipType backwardRelationType = RelationshipType.ManyToOne;

			switch (relationshipType)
			{
				case RelationshipType.OneToOne:
					backwardRelationType = RelationshipType.OneToOne;
					break;
				case RelationshipType.OneToMany:
					backwardRelationType = RelationshipType.ManyToOne;
					break;
				case RelationshipType.ManyToOne:
					backwardRelationType = RelationshipType.OneToMany;
					break;
			}

			return backwardRelationType;
		}

		/// <summary>
		/// Gets the values from the text boxes of the forward relationship tab
		/// </summary>
		private void GetForwardRelationshipValues()
		{
			_relationshipAttribute.Caption = this.frCaptionTextBox.Text;
			_relationshipAttribute.Type = (RelationshipType) Enum.Parse(typeof(RelationshipType), (string) this.frTypeComboBox.SelectedItem);
			_relationshipAttribute.IsJoinManager = this.frJoinManagerCheckBox.Checked;
			if (this.frOwnershipComboBox.SelectedIndex >= 0)
			{
				_relationshipAttribute.Ownership = (RelationshipOwnership) Enum.Parse(typeof(RelationshipOwnership), (string) this.frOwnershipComboBox.SelectedItem);
			}
			_relationshipAttribute.Description = this.frDescriptionTextBox.Text;

			// Note linked class is set at treeView_AfterSelect handler
		}

		/// <summary>
		/// Fill the text boxes of the backward relationship tab with known values
		/// </summary>
		private void SetBackwardRelationshipValues()
		{
			// show known info of the backward relationship
			this.brOwnerClassTextBox.Text = _relationshipAttribute.LinkedClass.Caption;
			this.brLinkedClassTextBox.Text = _relationshipAttribute.OwnerClass.Caption;
			RelationshipType backwardRelationType = GetBackwardRelationshipType(_relationshipAttribute.Type);
			this.brTypeTextBox.Text = Enum.GetName(typeof(RelationshipType), backwardRelationType);
			this.brJoinManagerCheckBox.Checked = (_relationshipAttribute.IsJoinManager? false : true);
			if (this.brJoinManagerCheckBox.Checked)
			{
				this.brOwnershipComboBox.Enabled = true;
				this.brOwnershipComboBox.SelectedItem = Enum.GetName(typeof(RelationshipOwnership), RelationshipOwnership.LooselyReferenced);
			}
			else
			{
				this.brOwnershipComboBox.Enabled = false;
			}
		}

		#endregion

		private MetaDataTreeNode FindTreeNode(MetaDataTreeNode root, ClassElement classElement)
		{
			foreach (MetaDataTreeNode node in root.Nodes)
			{
				if (classElement == node.MetaDataElement)
				{
					return node;
				}
				else
				{
					MetaDataTreeNode found = FindTreeNode(node, classElement);
					if (found != null)
					{
						return found;
					}
				}
			}

			return null;
		}

		private void SpecifyRelationshipDialog_Load(object sender, System.EventArgs e)
		{
			if (_metaData != null)
			{
				MetaDataTreeBuilder builder = new MetaDataTreeBuilder();

				// do not show attributes and constraints nodes
				builder.IsAttributesShown = false;
				builder.IsConstraintsShown = false;
				builder.IsTaxonomiesShown = false;
				builder.IsDataViewsShown = false;

				MetaDataTreeNode root = (MetaDataTreeNode) builder.BuildTree(_metaData);

				treeView.BeginUpdate();
				treeView.Nodes.Clear();
				treeView.Nodes.Add(root);

				if (_relationshipAttribute.LinkedClass != null)
				{
					// make the initial selection
					TreeNode selectedNode = FindTreeNode(root, _relationshipAttribute.LinkedClass);
					if (selectedNode != null)
					{
						treeView.SelectedNode = selectedNode;
					}
				}

				treeView.EndUpdate();

				// display help providers to some text boxes
				string tip = toolTip.GetToolTip(this.frTypeComboBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.frTypeComboBox, tip);
				}

				tip = toolTip.GetToolTip(this.frJoinManagerCheckBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.frJoinManagerCheckBox, tip);
					infoProvider.SetError(this.brJoinManagerCheckBox, tip);
				}

				tip = toolTip.GetToolTip(this.frOwnershipComboBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.frOwnershipComboBox, tip);
					infoProvider.SetError(this.brOwnershipComboBox, tip);
				}

				tip = toolTip.GetToolTip(this.brNameTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.brNameTextBox, tip);
				}

				tip = toolTip.GetToolTip(this.brCaptionTextBox);
				if (tip.Length > 0)
				{
					infoProvider.SetError(this.brCaptionTextBox, tip);
				}
			}

			if (_relationshipAttribute != null)
			{
				this.frNameTextBox.Text = _relationshipAttribute.Name;
				this.frCaptionTextBox.Text = _relationshipAttribute.Caption;
				this.frOwnerTextBox.Text = _relationshipAttribute.OwnerClass.Caption;
				this.frJoinManagerCheckBox.Checked = false;
				this.frDescriptionTextBox.Text = _relationshipAttribute.Description;

				// display the realtionship type through reflection
				Type type = typeof(RelationshipType);
				foreach (string enumValue in Enum.GetNames(type))
				{
			        this.frTypeComboBox.Items.Add(enumValue);
				}
				this.frTypeComboBox.SelectedIndex = 0;

				// display the ownership enum values
				type = typeof(RelationshipOwnership);
				foreach (string enumValue in Enum.GetNames(type))
				{
					this.frOwnershipComboBox.Items.Add(enumValue);
					this.brOwnershipComboBox.Items.Add(enumValue);
				}
				this.frTypeComboBox.SelectedIndex = 0;
			}
		}

		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Get the selected node
			MetaDataTreeNode node = (MetaDataTreeNode) e.Node;
			
			ClassElement linkedClass = node.MetaDataElement as ClassElement;

            if (linkedClass != null)
            {
                this.frLinkedClassTextBox.Text = linkedClass.Caption;
                _relationshipAttribute.LinkedClass = linkedClass;

                this.nextStepButton.Enabled = true; // allow to go to next step
            }
            else
            {
                this.frLinkedClassTextBox.Text = null;
                _relationshipAttribute.LinkedClass = null;
                this.nextStepButton.Enabled = false; // do not allow to go to next step
            }
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int tabIndex = this.tabControl1.SelectedIndex;

			// do not allow switching to new tab if the linked class is not selected
            // or relationship type is unknown
			if (_relationshipAttribute.LinkedClass != null &&
                frTypeComboBox.SelectedIndex > 0)
			{
				if (tabIndex > 0)
				{
					// it is in the backward relationship tab page
					// sets the values for the forward relationship
					// and sets the values for the backward relationship.
					GetForwardRelationshipValues();
					SetBackwardRelationshipValues();

					this.prevStepButton.Enabled = true;
					this.nextStepButton.Enabled = false;
				}
				else
				{
					this.prevStepButton.Enabled = false;
					this.nextStepButton.Enabled = true;
				}
			}
			else if (tabIndex > 0)
			{
				// stay in the first tab page
				this.tabControl1.SelectedIndex = 0;
			}
		}

		private void nextStepButton_Click(object sender, System.EventArgs e)
		{
            if (frTypeComboBox.SelectedIndex > 0)
            {
                int tabIndex = this.tabControl1.SelectedIndex;
                if (tabIndex < 1)
                {
                    // go to the next step
                    this.tabControl1.SelectedIndex = tabIndex + 1;
                }
            }
            else
            {
                // relationship type is unknown
                MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.UnknownRelationship"),
                    "Error Dialog", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
		}

		private void brNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			// keep the caption text in sync with the name
			this.brCaptionTextBox.Text = this.brNameTextBox.Text;

			if (this.brNameTextBox.Text.Length > 0)
			{
				this.doneButton.Enabled = true;
			}
		}

		private void prevStepButton_Click(object sender, System.EventArgs e)
		{
			int tabIndex = this.tabControl1.SelectedIndex;
			if (tabIndex > 0)
			{
				this.tabControl1.SelectedIndex = tabIndex - 1;
			}
		}

		private void doneButton_Click(object sender, System.EventArgs e)
		{
			// Create a backward relationship based on the data user selected, and
			// add it to the linked class.
			RelationshipAttributeElement backwardRelationship = new RelationshipAttributeElement(this.brNameTextBox.Text);
			backwardRelationship.Caption = this.brCaptionTextBox.Text;
			backwardRelationship.LinkedClass = _relationshipAttribute.OwnerClass;
			backwardRelationship.BackwardRelationship = _relationshipAttribute;
			backwardRelationship.Type = (RelationshipType) Enum.Parse(typeof(RelationshipType), this.brTypeTextBox.Text);
            backwardRelationship.IsJoinManager = this.brJoinManagerCheckBox.Checked;
			if (backwardRelationship.IsJoinManager)
			{
				backwardRelationship.Ownership = (RelationshipOwnership) Enum.Parse(typeof(RelationshipOwnership), (string) this.brOwnershipComboBox.SelectedItem);
			}

			backwardRelationship.Description = this.brDescriptionTextBox.Text;

            // create an index on the foreign key as default
            if (backwardRelationship.IsForeignKeyRequired)
            {
                backwardRelationship.IsIndexed = true;
            }
            else if (_relationshipAttribute.IsForeignKeyRequired)
            {
                _relationshipAttribute.IsIndexed = true;
            }

			if (VerifyRelationship())
			{
				_relationshipAttribute.LinkedClass.AddRelationshipAttribute(backwardRelationship);

				_relationshipAttribute.BackwardRelationship = backwardRelationship;
			}
			else
			{
				string classCaption;
				
				if (_relationshipAttribute.IsForeignKeyRequired)
				{
					classCaption = _relationshipAttribute.LinkedClass.Caption;
				}
				else
				{
					classCaption = _relationshipAttribute.OwnerClass.Caption;
				}

				// stay in this step
				MessageBox.Show(classCaption + MessageResourceManager.GetString("SchemaEditor.NoPrimaryKeys"),
					"Error Dialog", MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				// keep the SpecifyRelationshipDialog open
				this.DialogResult = DialogResult.None;
			}
		}

		private void frTypeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RelationshipType selectedType = (RelationshipType) Enum.Parse(typeof(RelationshipType), (string) this.frTypeComboBox.SelectedItem);

			switch (selectedType)
			{
                case RelationshipType.Unknown:
                    this.frJoinManagerCheckBox.Enabled = false;
                    this.frJoinManagerCheckBox.Checked = false;
                    this.frOwnershipComboBox.Enabled = false;
                    this.frOwnershipComboBox.SelectedItem = Enum.GetName(typeof(RelationshipOwnership), RelationshipOwnership.LooselyReferenced);
                    break;
				case RelationshipType.OneToOne:
					// user has to set data for IsJoinManager and Ownership
					this.frJoinManagerCheckBox.Enabled = true;
					this.frJoinManagerCheckBox.Checked = false;
					this.frOwnershipComboBox.Enabled = true;
					this.frOwnershipComboBox.SelectedItem = Enum.GetName(typeof(RelationshipOwnership), RelationshipOwnership.LooselyReferenced);
					break;
				case RelationshipType.ManyToOne:
					// by default, many to one relationship can not be a join manager
					this.frJoinManagerCheckBox.Checked = false;
					this.frJoinManagerCheckBox.Enabled = false;
					this.frOwnershipComboBox.Enabled = false;
					this.frOwnershipComboBox.SelectedItem = "";
					break;
				case RelationshipType.OneToMany:
					// by default, many to one relationship has to be a join manager
					this.frJoinManagerCheckBox.Checked = true;
					this.frJoinManagerCheckBox.Enabled = false;
					this.frOwnershipComboBox.Enabled = true;
					this.frOwnershipComboBox.SelectedItem = Enum.GetName(typeof(RelationshipOwnership), RelationshipOwnership.LooselyReferenced);;
					break;
			}
		}

		/// <summary>
		/// Verify if one side of classes in a relationship has primary keys defined.
		/// if the required primary key(s) are missing, it popups an error dialog and
		/// stay in the first tab.
		/// </summary>
		/// <returns>true if it is a good relationship, false otherwise.</returns>
		private bool VerifyRelationship()
		{
			bool isGoodRelationship = true;

			if (_relationshipAttribute.IsForeignKeyRequired)
			{
				// The linked class must has primary keys
				if (_relationshipAttribute.LinkedClass.RootClass.PrimaryKeys.Count == 0)
				{
					isGoodRelationship = false;
				}
			}
			else
			{
				// the owner class doesn't primary key(s)
				if (_relationshipAttribute.OwnerClass.RootClass.PrimaryKeys.Count == 0)
				{
					isGoodRelationship = false;
				}
			}

			return isGoodRelationship;
		}
	}
}
