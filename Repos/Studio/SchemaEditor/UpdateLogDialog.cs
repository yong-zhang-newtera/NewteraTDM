using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for UpdateLogDialog.
	/// </summary>
	public class UpdateLogDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.RichTextBox logTextBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public UpdateLogDialog()
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

		internal void SetUpdateLog(string log)
		{
			this.logTextBox.Text = log;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(UpdateLogDialog));
			this.cancelButton = new System.Windows.Forms.Button();
			this.logTextBox = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
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
			// logTextBox
			// 
			this.logTextBox.AccessibleDescription = resources.GetString("logTextBox.AccessibleDescription");
			this.logTextBox.AccessibleName = resources.GetString("logTextBox.AccessibleName");
			this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("logTextBox.Anchor")));
			this.logTextBox.AutoSize = ((bool)(resources.GetObject("logTextBox.AutoSize")));
			this.logTextBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("logTextBox.BackgroundImage")));
			this.logTextBox.BulletIndent = ((int)(resources.GetObject("logTextBox.BulletIndent")));
			this.logTextBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("logTextBox.Dock")));
			this.logTextBox.Enabled = ((bool)(resources.GetObject("logTextBox.Enabled")));
			this.logTextBox.Font = ((System.Drawing.Font)(resources.GetObject("logTextBox.Font")));
			this.logTextBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("logTextBox.ImeMode")));
			this.logTextBox.Location = ((System.Drawing.Point)(resources.GetObject("logTextBox.Location")));
			this.logTextBox.MaxLength = ((int)(resources.GetObject("logTextBox.MaxLength")));
			this.logTextBox.Multiline = ((bool)(resources.GetObject("logTextBox.Multiline")));
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ReadOnly = true;
			this.logTextBox.RightMargin = ((int)(resources.GetObject("logTextBox.RightMargin")));
			this.logTextBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("logTextBox.RightToLeft")));
			this.logTextBox.ScrollBars = ((System.Windows.Forms.RichTextBoxScrollBars)(resources.GetObject("logTextBox.ScrollBars")));
			this.logTextBox.Size = ((System.Drawing.Size)(resources.GetObject("logTextBox.Size")));
			this.logTextBox.TabIndex = ((int)(resources.GetObject("logTextBox.TabIndex")));
			this.logTextBox.Text = resources.GetString("logTextBox.Text");
			this.logTextBox.Visible = ((bool)(resources.GetObject("logTextBox.Visible")));
			this.logTextBox.WordWrap = ((bool)(resources.GetObject("logTextBox.WordWrap")));
			this.logTextBox.ZoomFactor = ((System.Single)(resources.GetObject("logTextBox.ZoomFactor")));
			// 
			// UpdateLogDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.logTextBox);
			this.Controls.Add(this.cancelButton);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "UpdateLogDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);

		}
		#endregion

	}
}
