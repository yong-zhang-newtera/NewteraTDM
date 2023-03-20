using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace Newtera.Studio.UserControls.Wizards
{
  [ToolboxItem(false)]
  [Designer(typeof(WizardPageDesigner))]
  public class WizardFinalPage : Newtera.Studio.UserControls.Wizards.WizardPageBase
  {
    #region Page controls

    private System.Windows.Forms.Label lblFinishHint;
    private System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.Label lblDescription2;
    private System.Windows.Forms.PictureBox imgWelcome;
    private System.Windows.Forms.Label lblWizardName;
    private System.ComponentModel.IContainer components = null;
    #endregion

    #region Class Properties
    protected override System.Drawing.Size DefaultSize
    {
      get
      {
        return new Size( 498, 328 );
      }
    }
    [Browsable(true)]
    [Category("Wizard Page")]
	[Localizable(true)]
    [Description("Gets/Sets wizard page second description Text. This description used only by welocme and final pages")]
    public string Description2
    {
      get
      {
        return lblDescription2.Text;
      }
      set
      {
        if( value != lblDescription2.Text )
        {
          lblDescription2.Text = value;
          OnDescription2Changed();
        }
      }
    }
    #endregion    

    #region Class Constructor/Finilize methods
    public WizardFinalPage()
    {
      InitializeComponent();
      
      this.Size = new Size( 498, 328 );
      this.Name = "wizFinalPage";
      base.WelcomePage = true;

      base.Title       = lblWizardName.Text;
      base.Description = lblDescription.Text;
      base.HeaderImage = imgWelcome.Image;
      base.FinishPage  = true;
    }

    protected override void Dispose( bool disposing )
    {
      if( disposing && components != null )
      {
        components.Dispose();
      }

      base.Dispose( disposing );
    }
    #endregion

    #region Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardFinalPage));
		this.lblDescription = new System.Windows.Forms.Label();
		this.lblWizardName = new System.Windows.Forms.Label();
		this.imgWelcome = new System.Windows.Forms.PictureBox();
		this.lblFinishHint = new System.Windows.Forms.Label();
		this.lblDescription2 = new System.Windows.Forms.Label();
		this.SuspendLayout();
		// 
		// lblDescription
		// 
		this.lblDescription.AccessibleDescription = resources.GetString("lblDescription.AccessibleDescription");
		this.lblDescription.AccessibleName = resources.GetString("lblDescription.AccessibleName");
		this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDescription.Anchor")));
		this.lblDescription.AutoSize = ((bool)(resources.GetObject("lblDescription.AutoSize")));
		this.lblDescription.BackColor = System.Drawing.Color.Transparent;
		this.lblDescription.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDescription.Dock")));
		this.lblDescription.Enabled = ((bool)(resources.GetObject("lblDescription.Enabled")));
		this.lblDescription.Font = ((System.Drawing.Font)(resources.GetObject("lblDescription.Font")));
		this.lblDescription.Image = ((System.Drawing.Image)(resources.GetObject("lblDescription.Image")));
		this.lblDescription.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDescription.ImageAlign")));
		this.lblDescription.ImageIndex = ((int)(resources.GetObject("lblDescription.ImageIndex")));
		this.lblDescription.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblDescription.ImeMode")));
		this.lblDescription.Location = ((System.Drawing.Point)(resources.GetObject("lblDescription.Location")));
		this.lblDescription.Name = "lblDescription";
		this.lblDescription.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblDescription.RightToLeft")));
		this.lblDescription.Size = ((System.Drawing.Size)(resources.GetObject("lblDescription.Size")));
		this.lblDescription.TabIndex = ((int)(resources.GetObject("lblDescription.TabIndex")));
		this.lblDescription.Text = resources.GetString("lblDescription.Text");
		this.lblDescription.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDescription.TextAlign")));
		this.lblDescription.Visible = ((bool)(resources.GetObject("lblDescription.Visible")));
		// 
		// lblWizardName
		// 
		this.lblWizardName.AccessibleDescription = resources.GetString("lblWizardName.AccessibleDescription");
		this.lblWizardName.AccessibleName = resources.GetString("lblWizardName.AccessibleName");
		this.lblWizardName.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblWizardName.Anchor")));
		this.lblWizardName.AutoSize = ((bool)(resources.GetObject("lblWizardName.AutoSize")));
		this.lblWizardName.BackColor = System.Drawing.Color.Transparent;
		this.lblWizardName.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblWizardName.Dock")));
		this.lblWizardName.Enabled = ((bool)(resources.GetObject("lblWizardName.Enabled")));
		this.lblWizardName.Font = ((System.Drawing.Font)(resources.GetObject("lblWizardName.Font")));
		this.lblWizardName.Image = ((System.Drawing.Image)(resources.GetObject("lblWizardName.Image")));
		this.lblWizardName.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblWizardName.ImageAlign")));
		this.lblWizardName.ImageIndex = ((int)(resources.GetObject("lblWizardName.ImageIndex")));
		this.lblWizardName.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblWizardName.ImeMode")));
		this.lblWizardName.Location = ((System.Drawing.Point)(resources.GetObject("lblWizardName.Location")));
		this.lblWizardName.Name = "lblWizardName";
		this.lblWizardName.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblWizardName.RightToLeft")));
		this.lblWizardName.Size = ((System.Drawing.Size)(resources.GetObject("lblWizardName.Size")));
		this.lblWizardName.TabIndex = ((int)(resources.GetObject("lblWizardName.TabIndex")));
		this.lblWizardName.Text = resources.GetString("lblWizardName.Text");
		this.lblWizardName.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblWizardName.TextAlign")));
		this.lblWizardName.Visible = ((bool)(resources.GetObject("lblWizardName.Visible")));
		// 
		// imgWelcome
		// 
		this.imgWelcome.AccessibleDescription = resources.GetString("imgWelcome.AccessibleDescription");
		this.imgWelcome.AccessibleName = resources.GetString("imgWelcome.AccessibleName");
		this.imgWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("imgWelcome.Anchor")));
		this.imgWelcome.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imgWelcome.BackgroundImage")));
		this.imgWelcome.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("imgWelcome.Dock")));
		this.imgWelcome.Enabled = ((bool)(resources.GetObject("imgWelcome.Enabled")));
		this.imgWelcome.Font = ((System.Drawing.Font)(resources.GetObject("imgWelcome.Font")));
		this.imgWelcome.Image = ((System.Drawing.Image)(resources.GetObject("imgWelcome.Image")));
		this.imgWelcome.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("imgWelcome.ImeMode")));
		this.imgWelcome.Location = ((System.Drawing.Point)(resources.GetObject("imgWelcome.Location")));
		this.imgWelcome.Name = "imgWelcome";
		this.imgWelcome.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("imgWelcome.RightToLeft")));
		this.imgWelcome.Size = ((System.Drawing.Size)(resources.GetObject("imgWelcome.Size")));
		this.imgWelcome.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("imgWelcome.SizeMode")));
		this.imgWelcome.TabIndex = ((int)(resources.GetObject("imgWelcome.TabIndex")));
		this.imgWelcome.TabStop = false;
		this.imgWelcome.Text = resources.GetString("imgWelcome.Text");
		this.imgWelcome.Visible = ((bool)(resources.GetObject("imgWelcome.Visible")));
		// 
		// lblFinishHint
		// 
		this.lblFinishHint.AccessibleDescription = resources.GetString("lblFinishHint.AccessibleDescription");
		this.lblFinishHint.AccessibleName = resources.GetString("lblFinishHint.AccessibleName");
		this.lblFinishHint.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblFinishHint.Anchor")));
		this.lblFinishHint.AutoSize = ((bool)(resources.GetObject("lblFinishHint.AutoSize")));
		this.lblFinishHint.BackColor = System.Drawing.Color.Transparent;
		this.lblFinishHint.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblFinishHint.Dock")));
		this.lblFinishHint.Enabled = ((bool)(resources.GetObject("lblFinishHint.Enabled")));
		this.lblFinishHint.Font = ((System.Drawing.Font)(resources.GetObject("lblFinishHint.Font")));
		this.lblFinishHint.Image = ((System.Drawing.Image)(resources.GetObject("lblFinishHint.Image")));
		this.lblFinishHint.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblFinishHint.ImageAlign")));
		this.lblFinishHint.ImageIndex = ((int)(resources.GetObject("lblFinishHint.ImageIndex")));
		this.lblFinishHint.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblFinishHint.ImeMode")));
		this.lblFinishHint.Location = ((System.Drawing.Point)(resources.GetObject("lblFinishHint.Location")));
		this.lblFinishHint.Name = "lblFinishHint";
		this.lblFinishHint.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblFinishHint.RightToLeft")));
		this.lblFinishHint.Size = ((System.Drawing.Size)(resources.GetObject("lblFinishHint.Size")));
		this.lblFinishHint.TabIndex = ((int)(resources.GetObject("lblFinishHint.TabIndex")));
		this.lblFinishHint.Text = resources.GetString("lblFinishHint.Text");
		this.lblFinishHint.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblFinishHint.TextAlign")));
		this.lblFinishHint.Visible = ((bool)(resources.GetObject("lblFinishHint.Visible")));
		// 
		// lblDescription2
		// 
		this.lblDescription2.AccessibleDescription = resources.GetString("lblDescription2.AccessibleDescription");
		this.lblDescription2.AccessibleName = resources.GetString("lblDescription2.AccessibleName");
		this.lblDescription2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDescription2.Anchor")));
		this.lblDescription2.AutoSize = ((bool)(resources.GetObject("lblDescription2.AutoSize")));
		this.lblDescription2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDescription2.Dock")));
		this.lblDescription2.Enabled = ((bool)(resources.GetObject("lblDescription2.Enabled")));
		this.lblDescription2.Font = ((System.Drawing.Font)(resources.GetObject("lblDescription2.Font")));
		this.lblDescription2.Image = ((System.Drawing.Image)(resources.GetObject("lblDescription2.Image")));
		this.lblDescription2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDescription2.ImageAlign")));
		this.lblDescription2.ImageIndex = ((int)(resources.GetObject("lblDescription2.ImageIndex")));
		this.lblDescription2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblDescription2.ImeMode")));
		this.lblDescription2.Location = ((System.Drawing.Point)(resources.GetObject("lblDescription2.Location")));
		this.lblDescription2.Name = "lblDescription2";
		this.lblDescription2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblDescription2.RightToLeft")));
		this.lblDescription2.Size = ((System.Drawing.Size)(resources.GetObject("lblDescription2.Size")));
		this.lblDescription2.TabIndex = ((int)(resources.GetObject("lblDescription2.TabIndex")));
		this.lblDescription2.Text = resources.GetString("lblDescription2.Text");
		this.lblDescription2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblDescription2.TextAlign")));
		this.lblDescription2.Visible = ((bool)(resources.GetObject("lblDescription2.Visible")));
		// 
		// WizardFinalPage
		// 
		this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
		this.AccessibleName = resources.GetString("$this.AccessibleName");
		this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
		this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
		this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
		this.BackColor = System.Drawing.Color.White;
		this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
		this.Controls.Add(this.lblDescription2);
		this.Controls.Add(this.lblDescription);
		this.Controls.Add(this.lblWizardName);
		this.Controls.Add(this.imgWelcome);
		this.Controls.Add(this.lblFinishHint);
		this.Description = resources.GetString("$this.Description");
		this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
		this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
		this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
		this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
		this.Name = "WizardFinalPage";
		this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
		this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
		this.Title = resources.GetString("$this.Title");
		this.ResumeLayout(false);

	}
    #endregion

    #region Class overrides

    protected override void OnHeaderImageChanged()
    {
      imgWelcome.Image = base.HeaderImage;
    }

    protected override void OnTitleChanged()
    {
      lblWizardName.Text = base.Title;
    }

    protected override void OnDescriptionChanged()
    {
      lblDescription.Text = base.Description;
    }
    
    protected virtual  void OnDescription2Changed()
    {
    }
    protected override void OnLoad(System.EventArgs e)
    {
      OnHeaderImageChanged();
      base.OnLoad(e);
    }
    #endregion
  }
}

