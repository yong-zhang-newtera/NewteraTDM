using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// Summary description for AddExprPopup.
	/// </summary>
	public class AddExprPopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private IDataViewElement _expr;
		private DataViewModel _dataView;

		private System.Windows.Forms.ListBox listBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddExprPopup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_expr = null;
		}

		/// <summary>
		/// Gets the expression created by the popup
		/// </summary>
		public object Expression
		{
			get
			{
				return _expr;
			}
			set
			{
				_expr = (IDataViewElement) value;
			}
		}

		/// <summary>
		/// Gets or sets the data view to which a search expression is built
		/// </summary>
		/// <value>A DataVieModel instance</value>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
			}
		}

		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the
		/// popup relative to the window.
		/// </summary>
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		public new void Show()
		{
			base.Show();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddExprPopup));
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            resources.ApplyResources(this.listBox, "listBox");
            this.listBox.Name = "listBox";
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // AddExprPopup
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.listBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AddExprPopup";
            this.Deactivate += new System.EventHandler(this.AddExprPopup_Deactivate);
            this.Load += new System.EventHandler(this.AddBinaryExprPopup_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string selectedItem = (String) this.listBox.SelectedItem;
			ElementType type = RelationalExpr.ConvertToElementType(selectedItem);
			if (type == ElementType.Unknown)
			{
				type = InExpr.ConvertToElementType(selectedItem);
			}

			if (type == ElementType.Unknown)
			{
				type = ParenthesizedExpr.ConvertToElementType(selectedItem);
			}

			switch (type)
			{
				case ElementType.Equals:
				case ElementType.NotEquals:
				case ElementType.LessThan:
				case ElementType.LessThanEquals:
				case ElementType.GreaterThan:
				case ElementType.GreaterThanEquals:
                case ElementType.Like:
                case ElementType.IsNull:
                case ElementType.IsNotNull:
					_expr = new RelationalExpr(type);
					break;
				case ElementType.In:
				case ElementType.NotIn:
					_expr = new InExpr(type);
					break;
				case ElementType.ParenthesizedExpr:
					_expr = new ParenthesizedExpr();
					break;
			}

			// fire the accept event
			if (Accept != null)
			{
				Accept(this, EventArgs.Empty);
			}

			this.Close();
		}

		private void AddBinaryExprPopup_Load(object sender, System.EventArgs e)
		{
			this.listBox.Items.AddRange(RelationalExpr.Operators);
			this.listBox.Items.AddRange(InExpr.Operators);
			this.listBox.Items.AddRange(ParenthesizedExpr.Operators);

			this.Focus();
		}

		private void AddExprPopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
