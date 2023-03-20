using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;


namespace Newtera.Studio.UserControls.Wizards
{
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(Newtera.Studio.UserControls.Wizards.WizardForm), "Newtera.Studio.UserControls.Wizards.WizardForm.bmp")]
  [Designer(typeof(WizardFormDesigner))]
  public class WizardForm : System.Windows.Forms.UserControl
  {
    #region Class Constants and Enumerators
    /// <summary>
    /// Enum configure how much buttons will be shown by wizard 
    /// and what state buttons will have
    /// </summary>
    [Flags]
      public enum TWizardsButtons
    {
      [Browsable(false)]
      None      = 0x0000,
      [Description("Enable or Disable Previous/Continue Button")]
      Previous  = 0x0001,
      [Description("Enable or Disable Next/Finish button")]
      Next      = 0x0002,
      [Description("If checked then Next button become a Finish Button")]
      Finish    = 0x0004,
      [Description("Show or not Help button")]
      Help      = 0x0010,
      [Description("Enable or Disable Cancel button")]
      Cancel    = 0x0020,
      
      [Browsable(false)]
      All                     = Previous | Next | Cancel | Help,
      [Browsable(false)]
      AllShort                = Previous | Next | Cancel,
      [Browsable(false)]
      AllShortFinishDisabled  = Previous | Finish | Cancel,
      [Browsable(false)]
      AllShortFinishEnabled   = Previous | Next | Finish | Cancel,
      [Browsable(false)]
      AllFinishEnabled        = Previous | Next | Finish | Cancel | Help,
      [Browsable(false)]
      AllFinishDisabled       = Previous | Finish | Help | Cancel
    }

    /// <summary>
    /// Enum configure Help Button Position
    /// </summary>
    public enum THelpPosition
    {
      AlignLeft,
      AlignRight
    }

    // static constants of Text displayed on Buttons. This values can be 
    // replaced by Wizard user in case of localization
    public static string DEF_FINISH = "&Finish";
    public static string DEF_NEXT = "&Next >";
    public static string DEF_BACK = "< &Back";
    public static string DEF_HELP = "&Help";
    public static string DEF_CANCEL = "&Cancel";
    public static string DEF_CLOSE = "Do you want to close Wizard and loose all Data entered on Wizard Pages?";
    public static string DEF_WARNING = "Warning";
    #endregion

    #region Class Internal Classes/Collection/EventArgs/Delegates
    /// <summary>
    /// Class Used to sort collection items by Index property
    /// </summary>
    internal class CollectionComapre : IComparer
    {
      public int Compare( object x, object y )
      {
        WizardPageBase page1 = ( WizardPageBase )x;
        WizardPageBase page2 = ( WizardPageBase )y;

        return ( page1.Index > page2.Index ) ? 1 : 
          ( ( page1.Index < page2.Index ) ? -1 : 0 );
      }
    }


    /// <summary>
    /// Custom collection which we use to work with Wizard Pages. Some
    /// methods of IList interface are overrided to make collection 
    /// type safe. Also Collection support "Changed" event and sorting 
    /// by Index property.
    /// </summary>
    public class WizardPagesCollection : CollectionBase
    {
      #region Class Members
      private WizardForm m_parent;
      #endregion

      #region Class events
      public event EventHandler Changed;
      #endregion

      #region Class Initialize methods
      public WizardPagesCollection( WizardForm parent )
      {
        m_parent = parent;
      }
      #endregion

      #region IList interface implementation
      public int Add( WizardPageBase value )
      {
        if( Contains( value ) == true )
        {
          return IndexOf( value );
        }

        value.WizardPageParent = m_parent;
        int index = InnerList.Add( value );
        
        if( value.Index < 0 ) 
          value.Index = index;

        RaiseChanged( value );

        return index;
      }

      public void AddRange( WizardPageBase[] array )
      {
        foreach( WizardPageBase page in array )
        {
          Add( page );
        }
      }

      public bool Contains( WizardPageBase value )
      {
        return InnerList.Contains( value );
      }

      public int IndexOf( WizardPageBase value )
      {
        return InnerList.IndexOf( value );
      }

      public void Insert( int index, WizardPageBase value )
      {
        value.WizardPageParent = m_parent;
        InnerList.Insert( index, value );
        
        if( value.Index < 0 )
          value.Index = index;

        RaiseChanged( value );
      }

      public void Remove( WizardPageBase value )
      {
        InnerList.Remove( value );
        RaiseChanged();
      }
      #endregion

      #region IList Properties
      public WizardPageBase this[int index]
      {
        get
        {
          return InnerList[index] as WizardPageBase;
        }
        set
        {
          value.WizardPageParent = m_parent;
          value.Index = index;
          InnerList[index] = value;
          RaiseChanged( value );
        }
      }
      #endregion

      #region Class Helper methods
      protected void RaiseChanged( WizardPageBase value )
      {
        if( Changed != null )
        {
          Changed( value, EventArgs.Empty );
        }
      }

      protected void RaiseChanged()
      {
        if( Changed != null )
        {
          Changed( this, EventArgs.Empty );
        }
      }

      public void Sort()
      {
        InnerList.Sort( new CollectionComapre() );
      }
      #endregion

      #region Class overrides
      protected override void OnClearComplete()
      {
        // raise Changed Event when collection cleared
        RaiseChanged();
      }
      
      protected override void OnSetComplete(int index, object oldValue, object newValue)
      {
        WizardPageBase page = ( WizardPageBase )newValue;
        page.WizardPageParent = m_parent;
        if( page.Index < 0 ) page.Index = index;

        base.OnSetComplete( index, oldValue, page );
        
        RaiseChanged( page );
      }

      protected override void OnInsertComplete(int index, object value)
      {
        WizardPageBase page = ( WizardPageBase )value;
        page.WizardPageParent = m_parent;
        if( page.Index < 0 ) page.Index = index;

        base.OnInsertComplete( index, page );
        
        RaiseChanged( page );
      }

      protected override void OnRemoveComplete(int index, object value)
      {
        RaiseChanged();
      }
      #endregion
    }

    /// <summary>
    /// Special Parameters send to user on Cancel Event
    /// </summary>
    public class EventWizardCancelArgs : EventArgs
    {
      private string m_strMessage;
      private string m_strTitle;

      public string Message
      {
        get
        {
          return m_strMessage;
        }
        set
        {
          m_strMessage = value;
        }
      }

      public string Title
      {
        get
        {
          return m_strTitle;
        }
        set
        {
          m_strTitle = value;
        }
      }

      // hide default constructor
      private EventWizardCancelArgs()
      {
      }

      public EventWizardCancelArgs( string title, string message ) : base()
      {
        m_strTitle = title;
        m_strMessage = message;
      }
    }

    /// <summary>
    /// Event parameters by which usr can control logic of page show
    /// </summary>
    public class EventNextArgs : EventArgs
    {
      private WizardPageBase m_page;
      private int m_iStep = 1;

      public WizardPageBase CurrentPage
      {
        get
        {
          return m_page;
        }
      }

      public int Step
      {
        get
        {
          return m_iStep;
        }
        set
        {
          m_iStep = value;
        }
      }

      private EventNextArgs()
      {
      }

      public EventNextArgs( WizardPageBase page )
      {
        m_page = page;
      }

      public EventNextArgs( WizardPageBase page, int step ) : this( page )
      {
        m_iStep = step;
      }
    }


    public delegate void WizardCancelEventHandler( object sender, EventWizardCancelArgs e );
    public delegate void WizardNextEventHandler( object sender, EventNextArgs e );
    #endregion

    #region Class Members
    private TWizardsButtons m_eButtons = TWizardsButtons.All;          // wizard buttons
    private THelpPosition   m_eHelpPosition = THelpPosition.AlignLeft; // wizard Help Button state
    
    private bool m_bFireEvents = true;      // QUIET Mode indicator
    private bool m_bShowConfirm = true;     // Does control show Confirmation Message on Cancel press
    private bool m_bFinishCalled;           // indeicate is Finish event was called or not
    private bool m_bCancelCalled;           // indeicate is Cancel event was called or not
    
    private WizardPagesCollection m_pages;  // Wizard Pages Collection
    private int m_iShowIndex = 0;           // Index of Shown Wizard page
    private int m_iLastShown = -1;
    private WizardPageBase m_CurrentPage;   // Currently Shown Wizard Page
    #endregion
    
    #region Form controls
    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.PictureBox imgHeader;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblDescription;
    private Newtera.Studio.UserControls.Wizards.LineFrame lineDownLimit;
    private System.Windows.Forms.Panel pnlBody;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnNextFinish;
    private System.Windows.Forms.Button btnBack;
    private System.Windows.Forms.Button btnHelp;
    private System.Windows.Forms.Panel pnlMain;

    private System.ComponentModel.Container components = null;
    #endregion

    #region Class Events
    [Category( "Property Changed" )]
    public event EventHandler WizardButtonsChanged;

    [Category( "Property Changed" )]
    public event EventHandler HelpAlignChanged;

    [Category( "Wizard" )]
    public event WizardNextEventHandler Next;

    [Category( "Wizard" )]
    public event WizardNextEventHandler Finish;

    [Category( "Wizard" )]
    public event WizardNextEventHandler Back;

	[Category( "Wizard" )]
	public event WizardNextEventHandler Continue;

    [Category( "Wizard" )]
    public event WizardCancelEventHandler Cancel;

    [Category( "Wizard" )]
    public event WizardNextEventHandler PageShown;

    [Category( "Wizard" )]
    public event EventHandler Help;

    [Category( "Wizard" )]
    public event EventHandler WizardClosed;
    #endregion

    #region Class Properties
    [Browsable( true )]
    [Category( "Wizard" )]
    [Description( "Gets/Sets buttons states in wizard control for current Wizard Page" )]
    [DefaultValue( typeof( WizardForm.TWizardsButtons ), "All" )]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Editor( "Newtera.Studio.UserControls.Wizards.FlagsEditor, Newtera.Studio.UserControls.Wizards, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d", 
       typeof( UITypeEditor ) )]
    public TWizardsButtons WizardButtons
    {
      get
      {
        return m_eButtons;
      }
      set
      {
        if( value != m_eButtons )
        {
          m_eButtons = value;
          OnWizardButtonChanged();
        }
      }
    }

    [Browsable( true )]
    [Category("Wizard")]
    [Description("Gets/Sets Help button align on wizard Control")]
    [DefaultValue( typeof( WizardForm.THelpPosition ), "AlignLeft" )]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public THelpPosition HelpAlign
    {
      get
      {
        return m_eHelpPosition;
      }
      set
      {
        if( value != m_eHelpPosition )
        {
          m_eHelpPosition = value;
          OnHelpAlignChanged();
        }
      }
    }

    [Browsable(true)]
    [Category("Wizard")]
    [Description("Gets collection of wizard pages")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public WizardPagesCollection Pages
    {
      get{ return m_pages; }
    }

    [Browsable(false)]
    [Description( "Gets/Sets QUIET Mode of control" )]
    [DefaultValue(false)]
    public bool QuietMode
    {
      get
      {
        return !m_bFireEvents;
      }
      set
      {
        m_bFireEvents = !value;
      }
    }

    [Browsable( true )]
    [Category( "Wizard" )]
    [Description( "Gets/Sets Does Wizard Show confirmation dialog on Cancel button Press" )]
    [DefaultValue( true )]
    public bool ShowCancelConfirm
    {
      get
      {
        return m_bShowConfirm;
      }
      set
      {
        m_bShowConfirm = value;
      }
    }
    
    [Browsable( true )]
    [Category( "Wizard" )]
    [Description( "Gets/Sets Current page of wizard" )]
    public int PageIndex
    {
      get
      {
        return m_iShowIndex;
      }
      set
      {
        if( value != m_iShowIndex && value < m_pages.Count && value >= 0 )
        {
          m_iShowIndex = value;
          ShowWizardPage();
        }
      }
    }
    #endregion

    #region Class Initialize/Finilize methods
    /// <summary>
    /// Default Constructor
    /// </summary>
    public WizardForm()
    {
      InitializeComponent();
      
      m_pages = new WizardPagesCollection( this );
      m_pages.Changed += new EventHandler( PagesCollectionChanged );
    }

    /// <summary>
    /// Custom resources cleaning
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose( bool disposing )
    {
      if( disposing && components != null )
      {
        components.Dispose();
      }

      base.Dispose( disposing );
    }
    #endregion

    #region Windows Form Designer generated code
    /// <summary>
    /// Control Initialization and Designer View Code
    /// </summary>
    private void InitializeComponent()
    {
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardForm));
		this.btnCancel = new System.Windows.Forms.Button();
		this.pnlHeader = new System.Windows.Forms.Panel();
		this.lblTitle = new System.Windows.Forms.Label();
		this.lblDescription = new System.Windows.Forms.Label();
		this.imgHeader = new System.Windows.Forms.PictureBox();
		this.btnNextFinish = new System.Windows.Forms.Button();
		this.btnBack = new System.Windows.Forms.Button();
		this.lineDownLimit = new Newtera.Studio.UserControls.Wizards.LineFrame();
		this.pnlBody = new System.Windows.Forms.Panel();
		this.btnHelp = new System.Windows.Forms.Button();
		this.pnlMain = new System.Windows.Forms.Panel();
		this.pnlHeader.SuspendLayout();
		this.pnlMain.SuspendLayout();
		this.SuspendLayout();
		// 
		// btnCancel
		// 
		this.btnCancel.AccessibleDescription = resources.GetString("btnCancel.AccessibleDescription");
		this.btnCancel.AccessibleName = resources.GetString("btnCancel.AccessibleName");
		this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnCancel.Anchor")));
		this.btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.BackgroundImage")));
		this.btnCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnCancel.Dock")));
		this.btnCancel.Enabled = ((bool)(resources.GetObject("btnCancel.Enabled")));
		this.btnCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnCancel.FlatStyle")));
		this.btnCancel.Font = ((System.Drawing.Font)(resources.GetObject("btnCancel.Font")));
		this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
		this.btnCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.ImageAlign")));
		this.btnCancel.ImageIndex = ((int)(resources.GetObject("btnCancel.ImageIndex")));
		this.btnCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnCancel.ImeMode")));
		this.btnCancel.Location = ((System.Drawing.Point)(resources.GetObject("btnCancel.Location")));
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnCancel.RightToLeft")));
		this.btnCancel.Size = ((System.Drawing.Size)(resources.GetObject("btnCancel.Size")));
		this.btnCancel.TabIndex = ((int)(resources.GetObject("btnCancel.TabIndex")));
		this.btnCancel.Text = resources.GetString("btnCancel.Text");
		this.btnCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.TextAlign")));
		this.btnCancel.Visible = ((bool)(resources.GetObject("btnCancel.Visible")));
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnCancel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WizardForm_KeyDown);
		// 
		// pnlHeader
		// 
		this.pnlHeader.AccessibleDescription = resources.GetString("pnlHeader.AccessibleDescription");
		this.pnlHeader.AccessibleName = resources.GetString("pnlHeader.AccessibleName");
		this.pnlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pnlHeader.Anchor")));
		this.pnlHeader.AutoScroll = ((bool)(resources.GetObject("pnlHeader.AutoScroll")));
		this.pnlHeader.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("pnlHeader.AutoScrollMargin")));
		this.pnlHeader.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("pnlHeader.AutoScrollMinSize")));
		this.pnlHeader.BackColor = System.Drawing.Color.White;
		this.pnlHeader.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlHeader.BackgroundImage")));
		this.pnlHeader.Controls.Add(this.lblTitle);
		this.pnlHeader.Controls.Add(this.lblDescription);
		this.pnlHeader.Controls.Add(this.imgHeader);
		this.pnlHeader.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pnlHeader.Dock")));
		this.pnlHeader.Enabled = ((bool)(resources.GetObject("pnlHeader.Enabled")));
		this.pnlHeader.Font = ((System.Drawing.Font)(resources.GetObject("pnlHeader.Font")));
		this.pnlHeader.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pnlHeader.ImeMode")));
		this.pnlHeader.Location = ((System.Drawing.Point)(resources.GetObject("pnlHeader.Location")));
		this.pnlHeader.Name = "pnlHeader";
		this.pnlHeader.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pnlHeader.RightToLeft")));
		this.pnlHeader.Size = ((System.Drawing.Size)(resources.GetObject("pnlHeader.Size")));
		this.pnlHeader.TabIndex = ((int)(resources.GetObject("pnlHeader.TabIndex")));
		this.pnlHeader.Text = resources.GetString("pnlHeader.Text");
		this.pnlHeader.Visible = ((bool)(resources.GetObject("pnlHeader.Visible")));
		// 
		// lblTitle
		// 
		this.lblTitle.AccessibleDescription = resources.GetString("lblTitle.AccessibleDescription");
		this.lblTitle.AccessibleName = resources.GetString("lblTitle.AccessibleName");
		this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblTitle.Anchor")));
		this.lblTitle.AutoSize = ((bool)(resources.GetObject("lblTitle.AutoSize")));
		this.lblTitle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblTitle.Dock")));
		this.lblTitle.Enabled = ((bool)(resources.GetObject("lblTitle.Enabled")));
		this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
		this.lblTitle.Font = ((System.Drawing.Font)(resources.GetObject("lblTitle.Font")));
		this.lblTitle.Image = ((System.Drawing.Image)(resources.GetObject("lblTitle.Image")));
		this.lblTitle.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTitle.ImageAlign")));
		this.lblTitle.ImageIndex = ((int)(resources.GetObject("lblTitle.ImageIndex")));
		this.lblTitle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblTitle.ImeMode")));
		this.lblTitle.Location = ((System.Drawing.Point)(resources.GetObject("lblTitle.Location")));
		this.lblTitle.Name = "lblTitle";
		this.lblTitle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblTitle.RightToLeft")));
		this.lblTitle.Size = ((System.Drawing.Size)(resources.GetObject("lblTitle.Size")));
		this.lblTitle.TabIndex = ((int)(resources.GetObject("lblTitle.TabIndex")));
		this.lblTitle.Text = resources.GetString("lblTitle.Text");
		this.lblTitle.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblTitle.TextAlign")));
		this.lblTitle.Visible = ((bool)(resources.GetObject("lblTitle.Visible")));
		// 
		// lblDescription
		// 
		this.lblDescription.AccessibleDescription = resources.GetString("lblDescription.AccessibleDescription");
		this.lblDescription.AccessibleName = resources.GetString("lblDescription.AccessibleName");
		this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblDescription.Anchor")));
		this.lblDescription.AutoSize = ((bool)(resources.GetObject("lblDescription.AutoSize")));
		this.lblDescription.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblDescription.Dock")));
		this.lblDescription.Enabled = ((bool)(resources.GetObject("lblDescription.Enabled")));
		this.lblDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
		// imgHeader
		// 
		this.imgHeader.AccessibleDescription = resources.GetString("imgHeader.AccessibleDescription");
		this.imgHeader.AccessibleName = resources.GetString("imgHeader.AccessibleName");
		this.imgHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("imgHeader.Anchor")));
		this.imgHeader.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imgHeader.BackgroundImage")));
		this.imgHeader.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("imgHeader.Dock")));
		this.imgHeader.Enabled = ((bool)(resources.GetObject("imgHeader.Enabled")));
		this.imgHeader.Font = ((System.Drawing.Font)(resources.GetObject("imgHeader.Font")));
		this.imgHeader.Image = ((System.Drawing.Image)(resources.GetObject("imgHeader.Image")));
		this.imgHeader.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("imgHeader.ImeMode")));
		this.imgHeader.Location = ((System.Drawing.Point)(resources.GetObject("imgHeader.Location")));
		this.imgHeader.Name = "imgHeader";
		this.imgHeader.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("imgHeader.RightToLeft")));
		this.imgHeader.Size = ((System.Drawing.Size)(resources.GetObject("imgHeader.Size")));
		this.imgHeader.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("imgHeader.SizeMode")));
		this.imgHeader.TabIndex = ((int)(resources.GetObject("imgHeader.TabIndex")));
		this.imgHeader.TabStop = false;
		this.imgHeader.Text = resources.GetString("imgHeader.Text");
		this.imgHeader.Visible = ((bool)(resources.GetObject("imgHeader.Visible")));
		// 
		// btnNextFinish
		// 
		this.btnNextFinish.AccessibleDescription = resources.GetString("btnNextFinish.AccessibleDescription");
		this.btnNextFinish.AccessibleName = resources.GetString("btnNextFinish.AccessibleName");
		this.btnNextFinish.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnNextFinish.Anchor")));
		this.btnNextFinish.BackColor = System.Drawing.SystemColors.Control;
		this.btnNextFinish.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnNextFinish.BackgroundImage")));
		this.btnNextFinish.Cursor = System.Windows.Forms.Cursors.Default;
		this.btnNextFinish.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnNextFinish.Dock")));
		this.btnNextFinish.Enabled = ((bool)(resources.GetObject("btnNextFinish.Enabled")));
		this.btnNextFinish.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnNextFinish.FlatStyle")));
		this.btnNextFinish.Font = ((System.Drawing.Font)(resources.GetObject("btnNextFinish.Font")));
		this.btnNextFinish.Image = ((System.Drawing.Image)(resources.GetObject("btnNextFinish.Image")));
		this.btnNextFinish.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnNextFinish.ImageAlign")));
		this.btnNextFinish.ImageIndex = ((int)(resources.GetObject("btnNextFinish.ImageIndex")));
		this.btnNextFinish.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnNextFinish.ImeMode")));
		this.btnNextFinish.Location = ((System.Drawing.Point)(resources.GetObject("btnNextFinish.Location")));
		this.btnNextFinish.Name = "btnNextFinish";
		this.btnNextFinish.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnNextFinish.RightToLeft")));
		this.btnNextFinish.Size = ((System.Drawing.Size)(resources.GetObject("btnNextFinish.Size")));
		this.btnNextFinish.TabIndex = ((int)(resources.GetObject("btnNextFinish.TabIndex")));
		this.btnNextFinish.Text = resources.GetString("btnNextFinish.Text");
		this.btnNextFinish.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnNextFinish.TextAlign")));
		this.btnNextFinish.Visible = ((bool)(resources.GetObject("btnNextFinish.Visible")));
		this.btnNextFinish.Click += new System.EventHandler(this.btnNextFinish_Click);
		this.btnNextFinish.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WizardForm_KeyDown);
		// 
		// btnBack
		// 
		this.btnBack.AccessibleDescription = resources.GetString("btnBack.AccessibleDescription");
		this.btnBack.AccessibleName = resources.GetString("btnBack.AccessibleName");
		this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnBack.Anchor")));
		this.btnBack.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBack.BackgroundImage")));
		this.btnBack.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnBack.Dock")));
		this.btnBack.Enabled = ((bool)(resources.GetObject("btnBack.Enabled")));
		this.btnBack.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnBack.FlatStyle")));
		this.btnBack.Font = ((System.Drawing.Font)(resources.GetObject("btnBack.Font")));
		this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
		this.btnBack.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnBack.ImageAlign")));
		this.btnBack.ImageIndex = ((int)(resources.GetObject("btnBack.ImageIndex")));
		this.btnBack.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnBack.ImeMode")));
		this.btnBack.Location = ((System.Drawing.Point)(resources.GetObject("btnBack.Location")));
		this.btnBack.Name = "btnBack";
		this.btnBack.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnBack.RightToLeft")));
		this.btnBack.Size = ((System.Drawing.Size)(resources.GetObject("btnBack.Size")));
		this.btnBack.TabIndex = ((int)(resources.GetObject("btnBack.TabIndex")));
		this.btnBack.Text = resources.GetString("btnBack.Text");
		this.btnBack.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnBack.TextAlign")));
		this.btnBack.Visible = ((bool)(resources.GetObject("btnBack.Visible")));
		this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
		this.btnBack.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WizardForm_KeyDown);
		// 
		// lineDownLimit
		// 
		this.lineDownLimit.AccessibleDescription = resources.GetString("lineDownLimit.AccessibleDescription");
		this.lineDownLimit.AccessibleName = resources.GetString("lineDownLimit.AccessibleName");
		this.lineDownLimit.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lineDownLimit.Anchor")));
		this.lineDownLimit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lineDownLimit.BackgroundImage")));
		this.lineDownLimit.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lineDownLimit.Dock")));
		this.lineDownLimit.Enabled = ((bool)(resources.GetObject("lineDownLimit.Enabled")));
		this.lineDownLimit.Font = ((System.Drawing.Font)(resources.GetObject("lineDownLimit.Font")));
		this.lineDownLimit.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lineDownLimit.ImeMode")));
		this.lineDownLimit.Location = ((System.Drawing.Point)(resources.GetObject("lineDownLimit.Location")));
		this.lineDownLimit.Name = "lineDownLimit";
		this.lineDownLimit.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lineDownLimit.RightToLeft")));
		this.lineDownLimit.Size = ((System.Drawing.Size)(resources.GetObject("lineDownLimit.Size")));
		this.lineDownLimit.TabIndex = ((int)(resources.GetObject("lineDownLimit.TabIndex")));
		this.lineDownLimit.TabStop = false;
		this.lineDownLimit.Text = resources.GetString("lineDownLimit.Text");
		this.lineDownLimit.Visible = ((bool)(resources.GetObject("lineDownLimit.Visible")));
		// 
		// pnlBody
		// 
		this.pnlBody.AccessibleDescription = resources.GetString("pnlBody.AccessibleDescription");
		this.pnlBody.AccessibleName = resources.GetString("pnlBody.AccessibleName");
		this.pnlBody.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pnlBody.Anchor")));
		this.pnlBody.AutoScroll = ((bool)(resources.GetObject("pnlBody.AutoScroll")));
		this.pnlBody.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("pnlBody.AutoScrollMargin")));
		this.pnlBody.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("pnlBody.AutoScrollMinSize")));
		this.pnlBody.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlBody.BackgroundImage")));
		this.pnlBody.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pnlBody.Dock")));
		this.pnlBody.Enabled = ((bool)(resources.GetObject("pnlBody.Enabled")));
		this.pnlBody.Font = ((System.Drawing.Font)(resources.GetObject("pnlBody.Font")));
		this.pnlBody.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pnlBody.ImeMode")));
		this.pnlBody.Location = ((System.Drawing.Point)(resources.GetObject("pnlBody.Location")));
		this.pnlBody.Name = "pnlBody";
		this.pnlBody.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pnlBody.RightToLeft")));
		this.pnlBody.Size = ((System.Drawing.Size)(resources.GetObject("pnlBody.Size")));
		this.pnlBody.TabIndex = ((int)(resources.GetObject("pnlBody.TabIndex")));
		this.pnlBody.Text = resources.GetString("pnlBody.Text");
		this.pnlBody.Visible = ((bool)(resources.GetObject("pnlBody.Visible")));
		// 
		// btnHelp
		// 
		this.btnHelp.AccessibleDescription = resources.GetString("btnHelp.AccessibleDescription");
		this.btnHelp.AccessibleName = resources.GetString("btnHelp.AccessibleName");
		this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnHelp.Anchor")));
		this.btnHelp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnHelp.BackgroundImage")));
		this.btnHelp.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnHelp.Dock")));
		this.btnHelp.Enabled = ((bool)(resources.GetObject("btnHelp.Enabled")));
		this.btnHelp.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnHelp.FlatStyle")));
		this.btnHelp.Font = ((System.Drawing.Font)(resources.GetObject("btnHelp.Font")));
		this.btnHelp.Image = ((System.Drawing.Image)(resources.GetObject("btnHelp.Image")));
		this.btnHelp.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnHelp.ImageAlign")));
		this.btnHelp.ImageIndex = ((int)(resources.GetObject("btnHelp.ImageIndex")));
		this.btnHelp.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnHelp.ImeMode")));
		this.btnHelp.Location = ((System.Drawing.Point)(resources.GetObject("btnHelp.Location")));
		this.btnHelp.Name = "btnHelp";
		this.btnHelp.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnHelp.RightToLeft")));
		this.btnHelp.Size = ((System.Drawing.Size)(resources.GetObject("btnHelp.Size")));
		this.btnHelp.TabIndex = ((int)(resources.GetObject("btnHelp.TabIndex")));
		this.btnHelp.Text = resources.GetString("btnHelp.Text");
		this.btnHelp.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnHelp.TextAlign")));
		this.btnHelp.Visible = ((bool)(resources.GetObject("btnHelp.Visible")));
		this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
		this.btnHelp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WizardForm_KeyDown);
		// 
		// pnlMain
		// 
		this.pnlMain.AccessibleDescription = resources.GetString("pnlMain.AccessibleDescription");
		this.pnlMain.AccessibleName = resources.GetString("pnlMain.AccessibleName");
		this.pnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pnlMain.Anchor")));
		this.pnlMain.AutoScroll = ((bool)(resources.GetObject("pnlMain.AutoScroll")));
		this.pnlMain.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("pnlMain.AutoScrollMargin")));
		this.pnlMain.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("pnlMain.AutoScrollMinSize")));
		this.pnlMain.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlMain.BackgroundImage")));
		this.pnlMain.Controls.Add(this.pnlBody);
		this.pnlMain.Controls.Add(this.pnlHeader);
		this.pnlMain.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pnlMain.Dock")));
		this.pnlMain.Enabled = ((bool)(resources.GetObject("pnlMain.Enabled")));
		this.pnlMain.Font = ((System.Drawing.Font)(resources.GetObject("pnlMain.Font")));
		this.pnlMain.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pnlMain.ImeMode")));
		this.pnlMain.Location = ((System.Drawing.Point)(resources.GetObject("pnlMain.Location")));
		this.pnlMain.Name = "pnlMain";
		this.pnlMain.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pnlMain.RightToLeft")));
		this.pnlMain.Size = ((System.Drawing.Size)(resources.GetObject("pnlMain.Size")));
		this.pnlMain.TabIndex = ((int)(resources.GetObject("pnlMain.TabIndex")));
		this.pnlMain.Text = resources.GetString("pnlMain.Text");
		this.pnlMain.Visible = ((bool)(resources.GetObject("pnlMain.Visible")));
		// 
		// WizardForm
		// 
		this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
		this.AccessibleName = resources.GetString("$this.AccessibleName");
		this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
		this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
		this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
		this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
		this.Controls.Add(this.pnlMain);
		this.Controls.Add(this.lineDownLimit);
		this.Controls.Add(this.btnBack);
		this.Controls.Add(this.btnNextFinish);
		this.Controls.Add(this.btnCancel);
		this.Controls.Add(this.btnHelp);
		this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
		this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
		this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
		this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
		this.Name = "WizardForm";
		this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
		this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
		this.Load += new System.EventHandler(this.WizardForm_Load);
		this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WizardForm_KeyDown);
		this.pnlHeader.ResumeLayout(false);
		this.pnlMain.ResumeLayout(false);
		this.ResumeLayout(false);

	}
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Show Wizard Page. m_iShowIndex is identifier of page which 
    /// Wizard control must show
    /// </summary>
    protected void ShowWizardPage()
    {
      // do not show one page twice
      if( m_iShowIndex == m_iLastShown ) return;

      pnlMain.SuspendLayout();
      pnlBody.SuspendLayout();
      
      pnlMain.Controls.Clear();
      pnlBody.Controls.Clear();

      // check if control has any pages
      if( m_pages.Count > 0 )
      {
        m_pages.Sort();

        // check m_iShowIndex
        if( m_iShowIndex < 0 ) m_iShowIndex = 0;
        if( m_iShowIndex >= m_pages.Count ) m_iShowIndex = m_pages.Count - 1;

        m_CurrentPage = m_pages[ m_iLastShown = m_iShowIndex ];
        
        // set page parent correctly in case when is something wrong
        if( m_CurrentPage.WizardPageParent != this )
        {
          m_CurrentPage.WizardPageParent = this;
        }
        
        // show page custom data
        if( m_CurrentPage.HeaderImage != null )
        {
          imgHeader.Image = m_CurrentPage.HeaderImage;
        }
        else
        {
          if( m_CurrentPage.ImageList != null && 
            m_CurrentPage.ImageIndex >= 0 && 
            m_CurrentPage.ImageIndex < m_CurrentPage.ImageList.Images.Count )
          {
            imgHeader.Image = m_CurrentPage.ImageList.Images[ m_CurrentPage.ImageIndex ];
          }
          else
          {
            imgHeader.Image = null;
          }
        }

        lblTitle.Text = m_CurrentPage.Title;
        lblDescription.Text = m_CurrentPage.Description;

        m_CurrentPage.Changed += new EventHandler( WizardPageAttributesChanged );

        // if Welcome page then link it ot pnlMain otherwise to pnlBody
        if( m_CurrentPage.WelcomePage == false )
        {
          pnlMain.Controls.AddRange( new System.Windows.Forms.Control[]{ pnlBody, pnlHeader } );
          
          // fix width and height of body panel
          pnlBody.Width = pnlMain.Width - 16;     
          pnlBody.Height = pnlMain.Height - pnlHeader.Height - 8;

          // fix page control resize
          if( m_CurrentPage.Width != pnlBody.Width || m_CurrentPage.Height != pnlBody.Height )
          {
            m_CurrentPage.Width = pnlBody.Width;
            m_CurrentPage.Height = pnlBody.Height;
          }

          pnlBody.Controls.Add( m_CurrentPage );
          pnlBody.Controls[0].Focus();
        }
        else
        {
          // fix page control resize
          if( m_CurrentPage.Width != pnlMain.Width || m_CurrentPage.Height != pnlMain.Height )
          {
            m_CurrentPage.Size = pnlMain.Size;
          }

          pnlMain.Controls.Add( m_CurrentPage );
          pnlMain.Controls[0].Focus();
        }
        
        // if current page has indicator that it is a Finish Page then Show 
        // Finish Button
        if( m_CurrentPage.FinishPage == true )
        {
          m_eButtons |= TWizardsButtons.Finish;
        }
        else // otherwise do not show Finish Button
        {
          m_eButtons &= (~TWizardsButtons.Finish);
        }
        
        // modify buttons states
        ChangeButtonsStates();
      }
      else // in case of empty Collection show standard "work page" view of control
      {
        pnlMain.Controls.AddRange( new System.Windows.Forms.Control[]{ pnlBody, pnlHeader } );
      }

      pnlBody.ResumeLayout();
      pnlMain.ResumeLayout();

      OnPageShown();
    }

    /// <summary>
    /// According to a state of control change Buttons states and position
    /// </summary>
    protected void ChangeButtonsStates()
    {
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardForm));
      int yPos = btnNextFinish.Top;
      int iNewWidth = Width;

      bool bHelp = ( ( m_eButtons & TWizardsButtons.Help ) == TWizardsButtons.Help );
      bool bCancel = ( ( m_eButtons & TWizardsButtons.Cancel ) == TWizardsButtons.Cancel );
      bool bNext = ( ( m_eButtons & TWizardsButtons.Next ) == TWizardsButtons.Next );
      bool bPrev = ( ( m_eButtons & TWizardsButtons.Previous ) == TWizardsButtons.Previous );
      bool bFinish = ( ( m_eButtons & TWizardsButtons.Finish ) == TWizardsButtons.Finish );

      // if we have Help Button
      btnHelp.Visible = bHelp;
      btnHelp.Enabled = bHelp;

      if( bHelp )
      {
        if( m_eHelpPosition == THelpPosition.AlignLeft ) // help button on default position
        {
          btnHelp.Left = 8;
          btnHelp.Top = yPos;

          btnHelp.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        }
        else
        {
          btnHelp.Left = Width - btnHelp.Width - 8;
          iNewWidth = btnHelp.Left;

          btnHelp.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        }

        if( m_CurrentPage != null )
        {
          btnHelp.Enabled = m_CurrentPage.HelpAvailable;
        }
      }

      // Cancel Button
      btnCancel.Left = iNewWidth - btnCancel.Width - 8;
      btnCancel.Top = yPos;
      btnCancel.Enabled = bCancel;
      iNewWidth = btnCancel.Left;

      // Next or Finish Button
      btnNextFinish.Left = iNewWidth - btnNextFinish.Width - 8;
      btnNextFinish.Top = yPos;
      btnNextFinish.Enabled = bNext;
      btnNextFinish.Text = ( bFinish == true ) ? resources.GetString("btnNextFinish.Finish") : resources.GetString("btnNextFinish.Text");
      btnNextFinish.DialogResult = ( bFinish == true ) ? DialogResult.OK : DialogResult.None;
      iNewWidth = btnNextFinish.Left;

      // Back button
      btnBack.Left = iNewWidth - btnBack.Width;
	  btnBack.Text = ( bFinish == true ) ? resources.GetString("btnBackFinish.Continue") : resources.GetString("btnBack.Text");
      btnBack.Top = yPos;
      btnBack.Enabled = bPrev;
      
      if( m_CurrentPage != null )
      {
        if( m_CurrentPage.Index == 0 && btnBack.Enabled == true )
        {
          btnBack.Enabled = false;
        }
      }
    }
    #endregion

    #region Designer Helper functions
    /// <summary>
    /// Special Method Used only In Designer Control Mode
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal void WizardPageAttributesChanged( object sender, EventArgs e )
    {
      WizardPageBase page = ( WizardPageBase )sender;
      
      // remove designer event first
      page.Changed -= new EventHandler( WizardPageAttributesChanged );
      
      m_iLastShown = -1;
      ShowWizardPage();
    }
    
    /// <summary>
    /// Method Indicate is Point on "Back" or "Next" Buttons. Method used only by Designer
    /// </summary>
    /// <param name="p">Point coordinates to check</param>
    /// <returns>TRUE - poin found on Butons</returns>
    public bool OnDesignedWizardButtons( Point p )
    {
      Rectangle rect1 = new Rectangle( btnNextFinish.Location, btnNextFinish.Size ); 
      Rectangle rect2 = new Rectangle( btnBack.Location, btnBack.Size ); 

      return ( rect1.Contains( p ) || rect2.Contains( p ) );
    }

    public Rectangle GetCurrentPageRect()
    {
      if( m_CurrentPage != null )
      {
        if( m_CurrentPage.WelcomePage == true )
        {
          return pnlMain.ClientRectangle;
        }
        else
        {
          return pnlBody.ClientRectangle;
        }
      }

      return Rectangle.Empty;
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Set Next Wizard Page
    /// </summary>
    public void MoveNextStep()
    {
      m_iShowIndex++;
      ShowWizardPage();
    }

    /// <summary>
    /// Set Pervious Wizard Page
    /// </summary>
    public void MoveBackStep()
    {
      m_iShowIndex--;
      ShowWizardPage();
    }

    /// <summary>
    /// Step Forward in Wizard on N pages
    /// </summary>
    /// <param name="count">Number of pages which must be skipped</param>
    public void StepNext( int count )
    {
      if( count == 0 ) return;
      m_iShowIndex += count;
      ShowWizardPage();
    }

    /// <summary>
    /// Step in Back direction on N pages 
    /// </summary>
    /// <param name="count">Number of pages which must be skipped</param>
    public void StepBack( int count )
    {
      if( count == 0 ) return;
      m_iShowIndex -= count;
      ShowWizardPage();
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// Function detach and then Attach keyPress event to all wizard pages controls 
    /// </summary>
    protected void AttachControlEvents( object sender, ControlEventArgs e )
    {
      e.Control.KeyDown += new KeyEventHandler( WizardForm_KeyDown );
    }

    protected void DetachControlEvents( object sender, ControlEventArgs e )
    {
      e.Control.KeyDown -= new KeyEventHandler( WizardForm_KeyDown );
    }

    /// <summary>
    /// Event Handler which raised on Pages Collection changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void PagesCollectionChanged( object sender, EventArgs e )
    {
      if( m_pages.Count > 0 )
      {
        if( sender is WizardPageBase )
        {
          WizardPageBase page = ( WizardPageBase )sender;
          
          // remove old handlers
          page.ControlAdded -= new ControlEventHandler( AttachControlEvents );
          page.ControlRemoved -= new ControlEventHandler( DetachControlEvents );

          // add them again
          page.ControlAdded += new ControlEventHandler( AttachControlEvents );
          page.ControlRemoved += new ControlEventHandler( DetachControlEvents );
        }

        m_iLastShown = -1;
        ShowWizardPage();
      }
    }

    /// <summary>
    /// Method Raise "WizardButtonChanged" event
    /// </summary>
    protected virtual void OnWizardButtonChanged()
    {
      if( WizardButtonsChanged != null && m_bFireEvents == true )
      {
        WizardButtonsChanged( this, EventArgs.Empty );
      }
      
      ChangeButtonsStates();
    }

    /// <summary>
    /// Method Raise "HelpAlignChanged" event
    /// </summary>
    protected virtual void OnHelpAlignChanged()
    {
      if( HelpAlignChanged != null && m_bFireEvents == true )
      {
        HelpAlignChanged( this, EventArgs.Empty );
      }
      
      ChangeButtonsStates();
    }

    /// <summary>
    /// Method Raise "Next" event and then Set Wizard Control to Next Page.
    /// </summary>
    protected virtual void OnNextPressed()
    {
      EventNextArgs e = new EventNextArgs( m_CurrentPage );

      if( Next != null && m_bFireEvents == true )
      {
        Next( m_CurrentPage, e );
      }

      StepNext( e.Step );
    }

    /// <summary>
    /// Method Raise "Finish" event. Event Raised when user press Finish Button.
    /// </summary>
    protected virtual void OnFinishPressed()
    {
      EventNextArgs e = new EventNextArgs( m_CurrentPage );

      if( Finish != null && m_bFireEvents == true )
      {
        m_bFinishCalled = true;
        Finish( m_CurrentPage, e );
      }
    }

    /// <summary>
    /// Method Raise "Back" event and then change Wizard control page to Previous Page
    /// </summary>
    protected virtual void OnBackPressed()
    {
      EventNextArgs e = new EventNextArgs( m_CurrentPage );

      if( Back != null && m_bFireEvents == true )
      {
        Back( m_CurrentPage, e );
      }    
      
      StepBack( e.Step );
    }

	/// <summary>
	/// Method Raise "Back" event and then change Wizard control page to Second Page
	/// </summary>
	protected virtual void OnContinuePressed()
	{
        // step back seven steps to the select file page
		EventNextArgs e = new EventNextArgs( m_CurrentPage, 8 );

		if( Continue != null && m_bFireEvents == true )
		{
			Continue( m_CurrentPage, e );
		}    
    
		StepBack( e.Step );
	}

    /// <summary>
    /// Method Raise "Cancel" event and then if property ShowConfirm is set, show
    /// Confirmation Dialog. If user Choose Yes on Confirmation then "WizardClosed"
    /// event thrown to user. In Event Handler user can chage message and title of 
    /// Confirmation Message Box. 
    /// </summary>
    protected virtual DialogResult OnCancelPressed()
    {
		System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardForm));

      EventWizardCancelArgs e = new EventWizardCancelArgs( DEF_WARNING, DEF_CLOSE );

      if( Cancel != null && m_bFireEvents == true )
      {
        Cancel( this, e );
      }

      if( m_bShowConfirm == true )
      {
		  string msg = resources.GetString("Warning.CloseWizard");
        DialogResult result = MessageBox.Show( this, msg, e.Title, MessageBoxButtons.YesNo );

        if( result == DialogResult.Yes )
        {
          m_bCancelCalled = true;
          OnWizardClosed();
          return DialogResult.Yes;
        }
      }

      return DialogResult.No;
    }

    /// <summary>
    /// Raise "Help" event when User press Help button
    /// </summary>
    protected virtual void OnHelpPressed()
    {
      if( Help != null && m_bFireEvents == true )
      {
        Help( m_CurrentPage, EventArgs.Empty );
      }    
    }

    /// <summary>
    /// Raise "WizardClosed" event.
    /// </summary>
    protected virtual void OnWizardClosed()
    {
      if( WizardClosed != null && m_bFireEvents == true )
      {
        WizardClosed( m_CurrentPage, EventArgs.Empty );
      }
    }
    /// <summary>
    /// Raise "PageShown" event.
    /// </summary>
    protected virtual void OnPageShown()
    {
      EventNextArgs e = new EventNextArgs( m_CurrentPage, m_pages.IndexOf( m_CurrentPage ) );

      if( PageShown != null && m_bFireEvents == true )
      {
        PageShown( m_CurrentPage, e );
      }
    }
    #endregion

    #region Class Events Handlers
    private void btnCancel_Click( object sender, EventArgs e )
    {
      if( btnCancel.Visible == true && btnCancel.Enabled == true )
      {
		  DialogResult result = OnCancelPressed();
      
		  // if user select Yes in confirmation dialog then close the form
		  if( result == DialogResult.Yes )
		  {
			  this.FindForm().Close();
		  }
      }
    }

    private void btnNextFinish_Click( object sender, EventArgs e )
    {
      if( btnNextFinish.Visible == true && btnNextFinish.Enabled == true )
      {
        if( m_CurrentPage != null )
        {
          if( m_CurrentPage.FinishPage == true && DesignMode == false )
          {
            OnFinishPressed();
          }
          else
          {
            OnNextPressed();
          }
        }
        else
        {
          btnNextFinish.Enabled = false;
        }
      }
    }

    private void btnBack_Click( object sender, EventArgs e )
    {
      if( btnBack.Visible == true && btnBack.Enabled == true )
      {
		  if (m_CurrentPage != null &&
			  m_CurrentPage.FinishPage == true &&
			  DesignMode == false )
		  {
			  OnContinuePressed();
		  }
		  else
		  {
			  OnBackPressed();
		  }
      }
    }

    private void btnHelp_Click( object sender, EventArgs e )
    {
      if( btnHelp.Visible == true && btnHelp.Enabled == true )
      {
        OnHelpPressed();
      }
    }

    private void WizardForm_KeyDown( object sender, KeyEventArgs e )
    {
      if( ( e.Modifiers & Keys.Alt ) == Keys.Alt )
      {
        if( e.KeyCode == Keys.N && btnNextFinish.Enabled == true )
        {
          btnNextFinish.Focus();
        }
        else if( e.KeyCode == Keys.B && btnBack.Enabled == true )
        {
          btnBack.Focus();
        }
        else if( e.KeyCode == Keys.C && btnCancel.Enabled == true )
        {
          btnCancel.Focus();
        }
        else if( e.KeyCode == Keys.H && btnHelp.Enabled == true )
        {
          btnHelp.Focus();
        }
      }

      if( ( e.Modifiers & ( Keys.Alt | Keys.Shift | Keys.Control ) ) == 0 )
      {
        if( e.KeyCode == Keys.Escape )
        {
          if( sender is ComboBox == true && true == ((ComboBox)sender).DroppedDown )
          {
            return;
          }

          btnCancel_Click( sender, e );
        }
        else if( e.KeyCode == Keys.Enter )
        {
          if( sender.Equals( btnCancel ) )
          {
            btnCancel_Click( sender, e );
          }
          else if( sender.Equals( btnBack ) )
          {
            btnBack_Click( sender, e );
          }
          else if( sender.Equals( btnHelp ) )
          {
            btnHelp_Click( sender, e );
          }
          else if( sender is TextBoxBase == true ) // skip enter key for multiline controls
          {
            TextBoxBase ctrl = (TextBoxBase)sender;
          
            if( ctrl.Multiline == false )
            {
              btnNextFinish_Click( sender, e );
            }
          }
          else if( sender is ComboBox == true )
          {
            ComboBox box = (ComboBox)sender;
          
            if( false == box.DroppedDown )
            {
              btnNextFinish_Click( sender, e );
            }
          }
          else
          {
            btnNextFinish_Click( sender, e );
          }
        }
      }
    }

    private void WizardForm_Closing( object sender, CancelEventArgs e )
    {
      /// if user press Finish Button and in Finish event handler user 
      /// call Close() method, we must accept closing without any questions.
      if( m_bFinishCalled == true || m_bCancelCalled == true )
      {
        // reset flag to return control to default state
        m_bFinishCalled = m_bCancelCalled = false;
        return;
      }

      if( btnCancel.Visible == true && btnCancel.Enabled == true )
      {
        DialogResult result = OnCancelPressed();
      
        // if user select No in confirmation dialog then cancel closing process
        if( result == DialogResult.No )
        {
          e.Cancel = true;
        }
      }
    }

    private void WizardForm_Load( object sender, EventArgs e )
    {
      this.FindForm().KeyDown += new KeyEventHandler( WizardForm_KeyDown );
      this.FindForm().Closing += new CancelEventHandler( WizardForm_Closing );
      
      // make our NextFinish Button default to a form
      //this.FindForm().AcceptButton = btnNextFinish;
    }
    #endregion
  }
}
