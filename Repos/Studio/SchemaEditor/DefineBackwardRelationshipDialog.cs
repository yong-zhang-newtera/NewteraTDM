using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for DefineBackwardRelationshipDialog.
	/// </summary>
	public class DefineBackwardRelationshipDialog : System.Windows.Forms.Form
	{
		private SchemaModel _schemaModel = null;
		private RelationshipAttributeElement _backwardRelationship = null;

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox linkedClassTextBox;
		private System.Windows.Forms.TextBox typeTextBox;
		private System.Windows.Forms.TextBox ownerClassTextBox;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.TextBox captionTextBox;
		private System.Windows.Forms.TextBox descriptionTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DefineBackwardRelationshipDialog()
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
		/// Gets or sets the schema model
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
		/// Gets or sets the backward relationship instance
		/// </summary>
		public RelationshipAttributeElement BackwardRelationship
		{
			get
			{
				return _backwardRelationship;
			}
			set
			{
				_backwardRelationship = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DefineBackwardRelationshipDialog));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.descriptionTextBox = new System.Windows.Forms.TextBox();
			this.captionTextBox = new System.Windows.Forms.TextBox();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.ownerClassTextBox = new System.Windows.Forms.TextBox();
			this.typeTextBox = new System.Windows.Forms.TextBox();
			this.linkedClassTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.AccessibleDescription = resources.GetString("groupBox1.AccessibleDescription");
			this.groupBox1.AccessibleName = resources.GetString("groupBox1.AccessibleName");
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("groupBox1.Anchor")));
			this.groupBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox1.BackgroundImage")));
			this.groupBox1.Controls.Add(this.descriptionTextBox);
			this.groupBox1.Controls.Add(this.captionTextBox);
			this.groupBox1.Controls.Add(this.nameTextBox);
			this.groupBox1.Controls.Add(this.ownerClassTextBox);
			this.groupBox1.Controls.Add(this.typeTextBox);
			this.groupBox1.Controls.Add(this.linkedClassTextBox);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox1.Dock")));
			this.groupBox1.Enabled = ((bool)(resources.GetObject("groupBox1.Enabled")));
			this.groupBox1.Font = ((System.Drawing.Font)(resources.GetObject("groupBox1.Font")));
			this.groupBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox1.ImeMode")));
			this.groupBox1.Location = ((System.Drawing.Point)(resources.GetObject("groupBox1.Location")));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox1.RightToLeft")));
			this.groupBox1.Size = ((System.Drawing.Size)(resources.GetObject("groupBox1.Size")));
			this.groupBox1.TabIndex = ((int)(resources.GetObject("groupBox1.TabIndex")));
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = resources.GetString("groupBox1.Text");
			this.groupBox1.Visible = ((bool)(resources.GetObject("groupBox1.Visible")));
			// 
			// descriptionTextBox
			// 
			this.descriptionTextBox.AccessibleDescription = resources.GetString("descriptionTextBox.AccessibleDescription");
			this.descriptionTextBox.AccessibleName = resources.GetString("descriptionTextBox.AccessibleName");
			this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("descriptionTextBox.Anchor")));
			this.descriptionTextBox.AutoSize = ((bool)(resources.GetObject("descriptionTextBox.AutoSize")));
			this.descriptionTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("descriptionTextBox.BackgroundImage")));
			this.descriptionTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("descriptionTextBox.Dock")));
			this.descriptionTextBox.Enabled = ((bool)(resources.GetObject("descriptionTextBox.Enabled")));
			this.descriptionTextBox.Font = ((System.Drawing.Font)(resources.GetObject("descriptionTextBox.Font")));
			this.descriptionTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("descriptionTextBox.ImeMode")));
			this.descriptionTextBox.Location = ((System.Drawing.Point)(resources.GetObject("descriptionTextBox.Location")));
			this.descriptionTextBox.MaxLength = ((int)(resources.GetObject("descriptionTextBox.MaxLength")));
			this.descriptionTextBox.Multiline = ((bool)(resources.GetObject("descriptionTextBox.Multiline")));
			this.descriptionTextBox.Name = "descriptionTextBox";
			this.descriptionTextBox.PasswordChar = ((char)(resources.GetObject("descriptionTextBox.PasswordChar")));
			this.descriptionTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("descriptionTextBox.RightToLeft")));
			this.descriptionTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("descriptionTextBox.ScrollBars")));
			this.descriptionTextBox.Size = ((System.Drawing.Size)(resources.GetObject("descriptionTextBox.Size")));
			this.descriptionTextBox.TabIndex = ((int)(resources.GetObject("descriptionTextBox.TabIndex")));
			this.descriptionTextBox.Text = resources.GetString("descriptionTextBox.Text");
			this.descriptionTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("descriptionTextBox.TextAlign")));
			this.descriptionTextBox.Visible = ((bool)(resources.GetObject("descriptionTextBox.Visible")));
			this.descriptionTextBox.WordWrap = ((bool)(resources.GetObject("descriptionTextBox.WordWrap")));
			// 
			// captionTextBox
			// 
			this.captionTextBox.AccessibleDescription = resources.GetString("captionTextBox.AccessibleDescription");
			this.captionTextBox.AccessibleName = resources.GetString("captionTextBox.AccessibleName");
			this.captionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("captionTextBox.Anchor")));
			this.captionTextBox.AutoSize = ((bool)(resources.GetObject("captionTextBox.AutoSize")));
			this.captionTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("captionTextBox.BackgroundImage")));
			this.captionTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("captionTextBox.Dock")));
			this.captionTextBox.Enabled = ((bool)(resources.GetObject("captionTextBox.Enabled")));
			this.captionTextBox.Font = ((System.Drawing.Font)(resources.GetObject("captionTextBox.Font")));
			this.captionTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("captionTextBox.ImeMode")));
			this.captionTextBox.Location = ((System.Drawing.Point)(resources.GetObject("captionTextBox.Location")));
			this.captionTextBox.MaxLength = ((int)(resources.GetObject("captionTextBox.MaxLength")));
			this.captionTextBox.Multiline = ((bool)(resources.GetObject("captionTextBox.Multiline")));
			this.captionTextBox.Name = "captionTextBox";
			this.captionTextBox.PasswordChar = ((char)(resources.GetObject("captionTextBox.PasswordChar")));
			this.captionTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("captionTextBox.RightToLeft")));
			this.captionTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("captionTextBox.ScrollBars")));
			this.captionTextBox.Size = ((System.Drawing.Size)(resources.GetObject("captionTextBox.Size")));
			this.captionTextBox.TabIndex = ((int)(resources.GetObject("captionTextBox.TabIndex")));
			this.captionTextBox.Text = resources.GetString("captionTextBox.Text");
			this.captionTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("captionTextBox.TextAlign")));
			this.captionTextBox.Visible = ((bool)(resources.GetObject("captionTextBox.Visible")));
			this.captionTextBox.WordWrap = ((bool)(resources.GetObject("captionTextBox.WordWrap")));
			// 
			// nameTextBox
			// 
			this.nameTextBox.AccessibleDescription = resources.GetString("nameTextBox.AccessibleDescription");
			this.nameTextBox.AccessibleName = resources.GetString("nameTextBox.AccessibleName");
			this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("nameTextBox.Anchor")));
			this.nameTextBox.AutoSize = ((bool)(resources.GetObject("nameTextBox.AutoSize")));
			this.nameTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("nameTextBox.BackgroundImage")));
			this.nameTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("nameTextBox.Dock")));
			this.nameTextBox.Enabled = ((bool)(resources.GetObject("nameTextBox.Enabled")));
			this.nameTextBox.Font = ((System.Drawing.Font)(resources.GetObject("nameTextBox.Font")));
			this.nameTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("nameTextBox.ImeMode")));
			this.nameTextBox.Location = ((System.Drawing.Point)(resources.GetObject("nameTextBox.Location")));
			this.nameTextBox.MaxLength = ((int)(resources.GetObject("nameTextBox.MaxLength")));
			this.nameTextBox.Multiline = ((bool)(resources.GetObject("nameTextBox.Multiline")));
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.PasswordChar = ((char)(resources.GetObject("nameTextBox.PasswordChar")));
			this.nameTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("nameTextBox.RightToLeft")));
			this.nameTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("nameTextBox.ScrollBars")));
			this.nameTextBox.Size = ((System.Drawing.Size)(resources.GetObject("nameTextBox.Size")));
			this.nameTextBox.TabIndex = ((int)(resources.GetObject("nameTextBox.TabIndex")));
			this.nameTextBox.Text = resources.GetString("nameTextBox.Text");
			this.nameTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("nameTextBox.TextAlign")));
			this.nameTextBox.Visible = ((bool)(resources.GetObject("nameTextBox.Visible")));
			this.nameTextBox.WordWrap = ((bool)(resources.GetObject("nameTextBox.WordWrap")));
			// 
			// ownerClassTextBox
			// 
			this.ownerClassTextBox.AccessibleDescription = resources.GetString("ownerClassTextBox.AccessibleDescription");
			this.ownerClassTextBox.AccessibleName = resources.GetString("ownerClassTextBox.AccessibleName");
			this.ownerClassTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("ownerClassTextBox.Anchor")));
			this.ownerClassTextBox.AutoSize = ((bool)(resources.GetObject("ownerClassTextBox.AutoSize")));
			this.ownerClassTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ownerClassTextBox.BackgroundImage")));
			this.ownerClassTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("ownerClassTextBox.Dock")));
			this.ownerClassTextBox.Enabled = ((bool)(resources.GetObject("ownerClassTextBox.Enabled")));
			this.ownerClassTextBox.Font = ((System.Drawing.Font)(resources.GetObject("ownerClassTextBox.Font")));
			this.ownerClassTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("ownerClassTextBox.ImeMode")));
			this.ownerClassTextBox.Location = ((System.Drawing.Point)(resources.GetObject("ownerClassTextBox.Location")));
			this.ownerClassTextBox.MaxLength = ((int)(resources.GetObject("ownerClassTextBox.MaxLength")));
			this.ownerClassTextBox.Multiline = ((bool)(resources.GetObject("ownerClassTextBox.Multiline")));
			this.ownerClassTextBox.Name = "ownerClassTextBox";
			this.ownerClassTextBox.PasswordChar = ((char)(resources.GetObject("ownerClassTextBox.PasswordChar")));
			this.ownerClassTextBox.ReadOnly = true;
			this.ownerClassTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("ownerClassTextBox.RightToLeft")));
			this.ownerClassTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("ownerClassTextBox.ScrollBars")));
			this.ownerClassTextBox.Size = ((System.Drawing.Size)(resources.GetObject("ownerClassTextBox.Size")));
			this.ownerClassTextBox.TabIndex = ((int)(resources.GetObject("ownerClassTextBox.TabIndex")));
			this.ownerClassTextBox.Text = resources.GetString("ownerClassTextBox.Text");
			this.ownerClassTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("ownerClassTextBox.TextAlign")));
			this.ownerClassTextBox.Visible = ((bool)(resources.GetObject("ownerClassTextBox.Visible")));
			this.ownerClassTextBox.WordWrap = ((bool)(resources.GetObject("ownerClassTextBox.WordWrap")));
			// 
			// typeTextBox
			// 
			this.typeTextBox.AccessibleDescription = resources.GetString("typeTextBox.AccessibleDescription");
			this.typeTextBox.AccessibleName = resources.GetString("typeTextBox.AccessibleName");
			this.typeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("typeTextBox.Anchor")));
			this.typeTextBox.AutoSize = ((bool)(resources.GetObject("typeTextBox.AutoSize")));
			this.typeTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("typeTextBox.BackgroundImage")));
			this.typeTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("typeTextBox.Dock")));
			this.typeTextBox.Enabled = ((bool)(resources.GetObject("typeTextBox.Enabled")));
			this.typeTextBox.Font = ((System.Drawing.Font)(resources.GetObject("typeTextBox.Font")));
			this.typeTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("typeTextBox.ImeMode")));
			this.typeTextBox.Location = ((System.Drawing.Point)(resources.GetObject("typeTextBox.Location")));
			this.typeTextBox.MaxLength = ((int)(resources.GetObject("typeTextBox.MaxLength")));
			this.typeTextBox.Multiline = ((bool)(resources.GetObject("typeTextBox.Multiline")));
			this.typeTextBox.Name = "typeTextBox";
			this.typeTextBox.PasswordChar = ((char)(resources.GetObject("typeTextBox.PasswordChar")));
			this.typeTextBox.ReadOnly = true;
			this.typeTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("typeTextBox.RightToLeft")));
			this.typeTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("typeTextBox.ScrollBars")));
			this.typeTextBox.Size = ((System.Drawing.Size)(resources.GetObject("typeTextBox.Size")));
			this.typeTextBox.TabIndex = ((int)(resources.GetObject("typeTextBox.TabIndex")));
			this.typeTextBox.Text = resources.GetString("typeTextBox.Text");
			this.typeTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("typeTextBox.TextAlign")));
			this.typeTextBox.Visible = ((bool)(resources.GetObject("typeTextBox.Visible")));
			this.typeTextBox.WordWrap = ((bool)(resources.GetObject("typeTextBox.WordWrap")));
			// 
			// linkedClassTextBox
			// 
			this.linkedClassTextBox.AccessibleDescription = resources.GetString("linkedClassTextBox.AccessibleDescription");
			this.linkedClassTextBox.AccessibleName = resources.GetString("linkedClassTextBox.AccessibleName");
			this.linkedClassTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("linkedClassTextBox.Anchor")));
			this.linkedClassTextBox.AutoSize = ((bool)(resources.GetObject("linkedClassTextBox.AutoSize")));
			this.linkedClassTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("linkedClassTextBox.BackgroundImage")));
			this.linkedClassTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("linkedClassTextBox.Dock")));
			this.linkedClassTextBox.Enabled = ((bool)(resources.GetObject("linkedClassTextBox.Enabled")));
			this.linkedClassTextBox.Font = ((System.Drawing.Font)(resources.GetObject("linkedClassTextBox.Font")));
			this.linkedClassTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("linkedClassTextBox.ImeMode")));
			this.linkedClassTextBox.Location = ((System.Drawing.Point)(resources.GetObject("linkedClassTextBox.Location")));
			this.linkedClassTextBox.MaxLength = ((int)(resources.GetObject("linkedClassTextBox.MaxLength")));
			this.linkedClassTextBox.Multiline = ((bool)(resources.GetObject("linkedClassTextBox.Multiline")));
			this.linkedClassTextBox.Name = "linkedClassTextBox";
			this.linkedClassTextBox.PasswordChar = ((char)(resources.GetObject("linkedClassTextBox.PasswordChar")));
			this.linkedClassTextBox.ReadOnly = true;
			this.linkedClassTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("linkedClassTextBox.RightToLeft")));
			this.linkedClassTextBox.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("linkedClassTextBox.ScrollBars")));
			this.linkedClassTextBox.Size = ((System.Drawing.Size)(resources.GetObject("linkedClassTextBox.Size")));
			this.linkedClassTextBox.TabIndex = ((int)(resources.GetObject("linkedClassTextBox.TabIndex")));
			this.linkedClassTextBox.Text = resources.GetString("linkedClassTextBox.Text");
			this.linkedClassTextBox.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("linkedClassTextBox.TextAlign")));
			this.linkedClassTextBox.Visible = ((bool)(resources.GetObject("linkedClassTextBox.Visible")));
			this.linkedClassTextBox.WordWrap = ((bool)(resources.GetObject("linkedClassTextBox.WordWrap")));
			// 
			// label6
			// 
			this.label6.AccessibleDescription = resources.GetString("label6.AccessibleDescription");
			this.label6.AccessibleName = resources.GetString("label6.AccessibleName");
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label6.Anchor")));
			this.label6.AutoSize = ((bool)(resources.GetObject("label6.AutoSize")));
			this.label6.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label6.Dock")));
			this.label6.Enabled = ((bool)(resources.GetObject("label6.Enabled")));
			this.label6.Font = ((System.Drawing.Font)(resources.GetObject("label6.Font")));
			this.label6.Image = ((System.Drawing.Image)(resources.GetObject("label6.Image")));
			this.label6.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.ImageAlign")));
			this.label6.ImageIndex = ((int)(resources.GetObject("label6.ImageIndex")));
			this.label6.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label6.ImeMode")));
			this.label6.Location = ((System.Drawing.Point)(resources.GetObject("label6.Location")));
			this.label6.Name = "label6";
			this.label6.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label6.RightToLeft")));
			this.label6.Size = ((System.Drawing.Size)(resources.GetObject("label6.Size")));
			this.label6.TabIndex = ((int)(resources.GetObject("label6.TabIndex")));
			this.label6.Text = resources.GetString("label6.Text");
			this.label6.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label6.TextAlign")));
			this.label6.Visible = ((bool)(resources.GetObject("label6.Visible")));
			// 
			// label5
			// 
			this.label5.AccessibleDescription = resources.GetString("label5.AccessibleDescription");
			this.label5.AccessibleName = resources.GetString("label5.AccessibleName");
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label5.Anchor")));
			this.label5.AutoSize = ((bool)(resources.GetObject("label5.AutoSize")));
			this.label5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label5.Dock")));
			this.label5.Enabled = ((bool)(resources.GetObject("label5.Enabled")));
			this.label5.Font = ((System.Drawing.Font)(resources.GetObject("label5.Font")));
			this.label5.Image = ((System.Drawing.Image)(resources.GetObject("label5.Image")));
			this.label5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.ImageAlign")));
			this.label5.ImageIndex = ((int)(resources.GetObject("label5.ImageIndex")));
			this.label5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label5.ImeMode")));
			this.label5.Location = ((System.Drawing.Point)(resources.GetObject("label5.Location")));
			this.label5.Name = "label5";
			this.label5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label5.RightToLeft")));
			this.label5.Size = ((System.Drawing.Size)(resources.GetObject("label5.Size")));
			this.label5.TabIndex = ((int)(resources.GetObject("label5.TabIndex")));
			this.label5.Text = resources.GetString("label5.Text");
			this.label5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label5.TextAlign")));
			this.label5.Visible = ((bool)(resources.GetObject("label5.Visible")));
			// 
			// label4
			// 
			this.label4.AccessibleDescription = resources.GetString("label4.AccessibleDescription");
			this.label4.AccessibleName = resources.GetString("label4.AccessibleName");
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label4.Anchor")));
			this.label4.AutoSize = ((bool)(resources.GetObject("label4.AutoSize")));
			this.label4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label4.Dock")));
			this.label4.Enabled = ((bool)(resources.GetObject("label4.Enabled")));
			this.label4.Font = ((System.Drawing.Font)(resources.GetObject("label4.Font")));
			this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
			this.label4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.ImageAlign")));
			this.label4.ImageIndex = ((int)(resources.GetObject("label4.ImageIndex")));
			this.label4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label4.ImeMode")));
			this.label4.Location = ((System.Drawing.Point)(resources.GetObject("label4.Location")));
			this.label4.Name = "label4";
			this.label4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label4.RightToLeft")));
			this.label4.Size = ((System.Drawing.Size)(resources.GetObject("label4.Size")));
			this.label4.TabIndex = ((int)(resources.GetObject("label4.TabIndex")));
			this.label4.Text = resources.GetString("label4.Text");
			this.label4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label4.TextAlign")));
			this.label4.Visible = ((bool)(resources.GetObject("label4.Visible")));
			// 
			// label3
			// 
			this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
			this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
			this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
			this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
			this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
			this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
			this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
			this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
			this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
			this.label3.Name = "label3";
			this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
			this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
			this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
			this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// okButton
			// 
			this.okButton.AccessibleDescription = resources.GetString("okButton.AccessibleDescription");
			this.okButton.AccessibleName = resources.GetString("okButton.AccessibleName");
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("okButton.Anchor")));
			this.okButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("okButton.BackgroundImage")));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("okButton.Dock")));
			this.okButton.Enabled = ((bool)(resources.GetObject("okButton.Enabled")));
			this.okButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("okButton.FlatStyle")));
			this.okButton.Font = ((System.Drawing.Font)(resources.GetObject("okButton.Font")));
			this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
			this.okButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.ImageAlign")));
			this.okButton.ImageIndex = ((int)(resources.GetObject("okButton.ImageIndex")));
			this.okButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("okButton.ImeMode")));
			this.okButton.Location = ((System.Drawing.Point)(resources.GetObject("okButton.Location")));
			this.okButton.Name = "okButton";
			this.okButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("okButton.RightToLeft")));
			this.okButton.Size = ((System.Drawing.Size)(resources.GetObject("okButton.Size")));
			this.okButton.TabIndex = ((int)(resources.GetObject("okButton.TabIndex")));
			this.okButton.Text = resources.GetString("okButton.Text");
			this.okButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.TextAlign")));
			this.okButton.Visible = ((bool)(resources.GetObject("okButton.Visible")));
			// 
			// cancelButton
			// 
			this.cancelButton.AccessibleDescription = resources.GetString("cancelButton.AccessibleDescription");
			this.cancelButton.AccessibleName = resources.GetString("cancelButton.AccessibleName");
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("cancelButton.Anchor")));
			this.cancelButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cancelButton.BackgroundImage")));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("cancelButton.Dock")));
			this.cancelButton.Enabled = ((bool)(resources.GetObject("cancelButton.Enabled")));
			this.cancelButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("cancelButton.FlatStyle")));
			this.cancelButton.Font = ((System.Drawing.Font)(resources.GetObject("cancelButton.Font")));
			this.cancelButton.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton.Image")));
			this.cancelButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("cancelButton.ImageAlign")));
			this.cancelButton.ImageIndex = ((int)(resources.GetObject("cancelButton.ImageIndex")));
			this.cancelButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("cancelButton.ImeMode")));
			this.cancelButton.Location = ((System.Drawing.Point)(resources.GetObject("cancelButton.Location")));
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("cancelButton.RightToLeft")));
			this.cancelButton.Size = ((System.Drawing.Size)(resources.GetObject("cancelButton.Size")));
			this.cancelButton.TabIndex = ((int)(resources.GetObject("cancelButton.TabIndex")));
			this.cancelButton.Text = resources.GetString("cancelButton.Text");
			this.cancelButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("cancelButton.TextAlign")));
			this.cancelButton.Visible = ((bool)(resources.GetObject("cancelButton.Visible")));
			// 
			// DefineBackwardRelationshipDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.groupBox1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "DefineBackwardRelationshipDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.DefineBackwardRelationshipDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void DefineBackwardRelationshipDialog_Load(object sender, System.EventArgs e)
		{
			if (_backwardRelationship != null)
			{
				this.nameTextBox.DataBindings.Add("Text", _backwardRelationship, "Name");
				this.captionTextBox.DataBindings.Add("Text", _backwardRelationship, "Caption");
				this.linkedClassTextBox.DataBindings.Add("Text", _backwardRelationship, "LinkedClassName");
				this.descriptionTextBox.DataBindings.Add("Text", _backwardRelationship, "Description");
				this.ownerClassTextBox.Text = _backwardRelationship.OwnerClass.Name;
				this.typeTextBox.Text = _backwardRelationship.Type + "";
			}
		}
	}
}
