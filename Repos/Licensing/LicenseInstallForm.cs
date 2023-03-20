#region File Header
//
//      FILE:   LicenseInstallForm.cs.
//
//    AUTHOR:   Grant Frisken
//
// COPYRIGHT:   Copyright 2004 
//              Infralution
//              6 Bruce St 
//              Mitcham Australia
//
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Infralution.Licensing
{
	/// <summary>
	/// Provides a basic form for installing Infralution Encrypted Licenses that can be extended or modified using 
	/// visual inheritance
	/// </summary>
	/// <seealso cref="EncryptedLicenseProvider"/>
#if PUBLIC_LICENSE_CLASS  // if true allows class to be visible outside library  
    public
#endif
    class LicenseInstallForm : System.Windows.Forms.Form
	{
        #region Member Variables

        /// <summary>The OK Button</summary>
        protected System.Windows.Forms.Button _okButton;

        /// <summary>Displays the main message of the form</summary>
        protected System.Windows.Forms.Label _msgLabel;

        /// <summary>Displays text to the left of the LicenseKey entry box</summary>
        protected System.Windows.Forms.Label _keyLabel;
 
        /// <summary>Provides link to company website</summary>
        protected System.Windows.Forms.LinkLabel _linkLabel;

        /// <summary>Displays text to the left of the website link</summary>
        protected System.Windows.Forms.Label _linkLabelLabel;

        /// <summary>Allows the user to enter a license key for the product</summary>
        protected System.Windows.Forms.TextBox _keyText;

 
        /// <summary>The license installed by the form (if any)</summary>
        private EncryptedLicense _license;
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Label _invalidKeyTitle;
        private System.Windows.Forms.Label _invalidKeyMsg;
        private System.Windows.Forms.Label _errorMsg;
        private System.Windows.Forms.Label _errorTitle;
        private System.Type _licenseType;

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LicenseInstallForm));
            this._okButton = new System.Windows.Forms.Button();
            this._msgLabel = new System.Windows.Forms.Label();
            this._keyText = new System.Windows.Forms.TextBox();
            this._keyLabel = new System.Windows.Forms.Label();
            this._linkLabel = new System.Windows.Forms.LinkLabel();
            this._linkLabelLabel = new System.Windows.Forms.Label();
            this._invalidKeyTitle = new System.Windows.Forms.Label();
            this._invalidKeyMsg = new System.Windows.Forms.Label();
            this._errorMsg = new System.Windows.Forms.Label();
            this._errorTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._okButton.Location = new System.Drawing.Point(280, 104);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(80, 24);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "OK";
            this._okButton.Click += new System.EventHandler(this.OnOkButtonClick);
            // 
            // _msgLabel
            // 
            this._msgLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._msgLabel.Location = new System.Drawing.Point(16, 8);
            this._msgLabel.Name = "_msgLabel";
            this._msgLabel.Size = new System.Drawing.Size(344, 64);
            this._msgLabel.TabIndex = 1;
            this._msgLabel.Text = "{0} is currently not licensed.  To install a license, enter the key you received " +
                "on purchasing the product and click OK.    You may continue to evaluate the prod" +
                "uct under the terms of the evaluation license by leaving the License Key blank. " +
                "";
            // 
            // _keyText
            // 
            this._keyText.Location = new System.Drawing.Point(104, 72);
            this._keyText.Multiline = true;
            this._keyText.Name = "_keyText";
            this._keyText.Size = new System.Drawing.Size(256, 20);
            this._keyText.TabIndex = 2;
            this._keyText.Text = "";
            // 
            // _keyLabel
            // 
            this._keyLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._keyLabel.Location = new System.Drawing.Point(8, 74);
            this._keyLabel.Name = "_keyLabel";
            this._keyLabel.Size = new System.Drawing.Size(88, 16);
            this._keyLabel.TabIndex = 3;
            this._keyLabel.Text = "License Key";
            this._keyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _linkLabel
            // 
            this._linkLabel.AutoSize = true;
            this._linkLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._linkLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 100);
            this._linkLabel.Location = new System.Drawing.Point(104, 112);
            this._linkLabel.Name = "_linkLabel";
            this._linkLabel.Size = new System.Drawing.Size(18, 16);
            this._linkLabel.TabIndex = 4;
            this._linkLabel.TabStop = true;
            this._linkLabel.Text = "{0}";
            this._linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkLabel_LinkClicked);
            // 
            // _linkLabelLabel
            // 
            this._linkLabelLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._linkLabelLabel.Location = new System.Drawing.Point(8, 112);
            this._linkLabelLabel.Name = "_linkLabelLabel";
            this._linkLabelLabel.Size = new System.Drawing.Size(88, 16);
            this._linkLabelLabel.TabIndex = 5;
            this._linkLabelLabel.Text = "Visit us at:";
            this._linkLabelLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _invalidKeyTitle
            // 
            this._invalidKeyTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._invalidKeyTitle.Location = new System.Drawing.Point(8, 160);
            this._invalidKeyTitle.Name = "_invalidKeyTitle";
            this._invalidKeyTitle.Size = new System.Drawing.Size(224, 16);
            this._invalidKeyTitle.TabIndex = 6;
            this._invalidKeyTitle.Text = "Invalid License Key";
            // 
            // _invalidKeyMsg
            // 
            this._invalidKeyMsg.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._invalidKeyMsg.Location = new System.Drawing.Point(8, 176);
            this._invalidKeyMsg.Name = "_invalidKeyMsg";
            this._invalidKeyMsg.Size = new System.Drawing.Size(328, 40);
            this._invalidKeyMsg.TabIndex = 7;
            this._invalidKeyMsg.Text = "The key entered was not a valid license key for this product.";
            // 
            // _errorMsg
            // 
            this._errorMsg.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._errorMsg.Location = new System.Drawing.Point(8, 232);
            this._errorMsg.Name = "_errorMsg";
            this._errorMsg.Size = new System.Drawing.Size(328, 40);
            this._errorMsg.TabIndex = 9;
            this._errorMsg.Text = "An error occurred while installing the license.  Ensure you have sufficient privi" +
                "leges to install the license.";
            // 
            // _errorTitle
            // 
            this._errorTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._errorTitle.Location = new System.Drawing.Point(8, 216);
            this._errorTitle.Name = "_errorTitle";
            this._errorTitle.Size = new System.Drawing.Size(224, 16);
            this._errorTitle.TabIndex = 10;
            this._errorTitle.Text = "License Install Error";
            // 
            // LicenseInstallForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(376, 142);
            this.Controls.Add(this._errorTitle);
            this.Controls.Add(this._errorMsg);
            this.Controls.Add(this._invalidKeyMsg);
            this.Controls.Add(this._invalidKeyTitle);
            this.Controls.Add(this._linkLabelLabel);
            this.Controls.Add(this._linkLabel);
            this.Controls.Add(this._keyText);
            this.Controls.Add(this._keyLabel);
            this.Controls.Add(this._msgLabel);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseInstallForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Install {0} License";
            this.ResumeLayout(false);

        }
        #endregion

        #region Public Interface

        /// <summary>
        /// Initialize a new instance of the form
        /// </summary>
        public LicenseInstallForm()
		{
			InitializeComponent();
		}

        /// <summary>
        /// The type of the component/control being licensed
        /// </summary>
        public Type TypeToLicense
        {
            get { return _licenseType; }
            set { _licenseType = value; }
        }

        /// <summary>
        /// The license installed by this form (if any)
        /// </summary>
        public EncryptedLicense InstalledLicense
        {
            get { return _license; }
        }

        /// <summary>
        /// Display the InstallLicense Dialog
        /// </summary>
        /// <param name="productName">The name of the product being licensed</param>
        /// <param name="companyWebsite">The company website</param>
        /// <param name="typeToLicense">The type of the component being licensed</param>
        /// <returns>The newly installed license (if any)</returns>
        public EncryptedLicense ShowDialog(string productName, string companyWebsite, Type typeToLicense)
        {
            this.Text = string.Format(Text, productName);
            _msgLabel.Text = string.Format(_msgLabel.Text, productName);
            _linkLabel.Text = companyWebsite;
            _licenseType = typeToLicense;
            this.ShowDialog();
            return _license;
        }

        #endregion

        #region Local Methods and Overrides

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
        /// Return the license provider to use
        /// </summary>
        /// <returns>The license provider to use for installing licensing</returns>
        protected virtual EncryptedLicenseProvider GetLicenseProvider()
        {
            return new EncryptedLicenseProvider();
        }

        /// <summary>
        /// Install the license key entered by the user
        /// </summary>
        /// <param name="key">The key to install</param>
        /// <returns>True if the license was installed successfully</returns>
        protected virtual bool InstallLicenseKey(string key)
        {
            try
            {
                _license = GetLicenseProvider().InstallLicense(_licenseType, key);
                if (_license == null)
                {
                    MessageBox.Show(_invalidKeyMsg.Text, _invalidKeyTitle.Text, 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return (_license != null);
            }
            catch 
            {
                MessageBox.Show(_errorMsg.Text, _errorTitle.Text, 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// Handle Click event for the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnOkButtonClick(object sender, System.EventArgs e)
        {

            if (_keyText.Text == null || _keyText.Text.Trim().Length == 0)
            {
                this.Close();
            }
            else 
            {
                if (InstallLicenseKey(_keyText.Text))
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Handle click on the link label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void _linkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            string address = @"http://" + _linkLabel.Text;
            _linkLabel.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start(address);      
            }
            catch {}
        }

        #endregion

	}
}
