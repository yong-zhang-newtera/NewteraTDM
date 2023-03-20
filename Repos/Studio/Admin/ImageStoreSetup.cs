using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ImageStoreSetup.
	/// </summary>
	public class ImageStoreSetup : System.Windows.Forms.Form
	{
		private const string IMAGE_RELATIVE_PATH = @"images\items\";

		private WizardButtonTag _buttonTag;
		private DBSetupWizard _wizard;
		private AdminServiceStub _service;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.TextBox basePathTextBox;
		private System.Windows.Forms.TextBox baseURLTextBox;
        private IContainer components;

        public ImageStoreSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_buttonTag = WizardButtonTag.None;
			_service = new AdminServiceStub();
		}

		~ImageStoreSetup()
		{
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
		/// Get the tag of a button that is clicked
		/// </summary>
		/// <value>One of WizardButtonTag values</value>
		internal WizardButtonTag ButtonTag
		{
			get
			{
				return this._buttonTag;
			}
		}

		/// <summary>
		/// Gets or sets the DB Setup Wizard controller
		/// </summary>
		internal DBSetupWizard Wizard
		{
			get
			{
				return _wizard;
			}
			set
			{
				_wizard = value;
			}
		}

		/// <summary>
		/// Gets the Image Base URL
		/// </summary>
		public string ImageBaseURL
		{
			get
			{
				return this.baseURLTextBox.Text;
			}
		}

		/// <summary>
		/// Gets the Image Base Path
		/// </summary>
		public string ImageBasePath
		{
			get
			{
				return this.basePathTextBox.Text;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageStoreSetup));
            this.panel1 = new System.Windows.Forms.Panel();
            this.basePathTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.baseURLTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.basePathTextBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.baseURLTextBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // basePathTextBox
            // 
            resources.ApplyResources(this.basePathTextBox, "basePathTextBox");
            this.basePathTextBox.Name = "basePathTextBox";
            this.basePathTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.basePathTextBox_Validating);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // baseURLTextBox
            // 
            resources.ApplyResources(this.baseURLTextBox, "baseURLTextBox");
            this.baseURLTextBox.Name = "baseURLTextBox";
            this.baseURLTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.baseURLTextBox_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HighlightText;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Tag = "1";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // backButton
            // 
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.CausesValidation = false;
            this.backButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.backButton.Name = "backButton";
            this.backButton.Tag = "2";
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            resources.ApplyResources(this.nextButton, "nextButton");
            this.nextButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.nextButton.Name = "nextButton";
            this.nextButton.Tag = "3";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // finishButton
            // 
            resources.ApplyResources(this.finishButton, "finishButton");
            this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.finishButton.Name = "finishButton";
            this.finishButton.Tag = "4";
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // ImageStoreSetup
            // 
            this.AcceptButton = this.nextButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.finishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImageStoreSetup";
            this.Load += new System.EventHandler(this.ImageStoreSetup_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Cancel;
		}

		private void backButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Back;
		}

		private void nextButton_Click(object sender, System.EventArgs e)
		{
			// validate the text in baseURLTextBox
			this.baseURLTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			// validate the text in basePathTextBox
			this.basePathTextBox.Focus();

			if (!this.Validate())
			{
				this.DialogResult = DialogResult.None;
				return;
			}

			this._buttonTag = WizardButtonTag.Next;
			this._wizard.ImageBaseURL = ImageBaseURL;
			this._wizard.ImageBasePath = ImageBasePath;

			this.DialogResult = DialogResult.OK;
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			this._buttonTag = WizardButtonTag.Finish;		
		}

		private void baseURLTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the base url cannot be null
			if (this.baseURLTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.baseURLTextBox, "Please enter an image base URL.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}

		private void basePathTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// the base path cannot be null
			if (this.basePathTextBox.Text.Length == 0)
			{
				this.errorProvider.SetError(this.basePathTextBox, "Please enter an image base path.");
				e.Cancel = true;
			}
			else
			{
				this.errorProvider.SetError((Control) sender, null);
			}		
		}

		private void ImageStoreSetup_Load(object sender, System.EventArgs e)
		{
			try
			{
				string server = "localhost/Newtera"; // default server name
                string urlSetting = System.Configuration.ConfigurationManager.AppSettings["Studio.AdminWebService.AdminService"];
				if (urlSetting != null)
				{
					// url is in form of http://hostname/appname/..."
					int pos = -1;
					int count = 0;
					// start after http:// find the second "/" in the url
					for (int i = 7; i < urlSetting.Length; i++)
					{
						if (urlSetting[i] == '/')
						{
							if (count == 0)
							{
								count++;
							}
							else
							{
								count++;
								pos = i;
								break;
							}
						}
					}

					if (pos > 0 && count == 2)
					{
						server = urlSetting.Substring(7, pos - 7);
					}
				}

				this.baseURLTextBox.Text ="http://" + server + "/images/items/";

				string appHome = _service.GetAppHomeDir();
				appHome = appHome.Trim();
				if (!appHome.EndsWith(@"\"))
				{
					appHome += @"\";
				}

				this.basePathTextBox.Text = appHome + IMAGE_RELATIVE_PATH;	
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}
	}
}
