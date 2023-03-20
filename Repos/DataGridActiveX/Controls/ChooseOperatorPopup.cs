using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.DataGridActiveX.DataGridView;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// Summary description for ChooseOperatorPopup.
	/// </summary>
	public class ChooseOperatorPopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private string _operator;
		private DataGridViewModel _dataGridView;
		private ViewElementType _operatorType;
		private System.Windows.Forms.ListBox listBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseOperatorPopup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_operatorType = ViewElementType.Unknown;
			_operator = null;
		}

		/// <summary>
		/// Gets the expression created by the popup
		/// </summary>
		public object Expression
		{
			get
			{
				return _operator;
			}
			set
			{
				_operator = (string) value;
			}
		}

		/// <summary>
		/// Gets or sets the data view to which a search expression is built
		/// </summary>
		/// <value>A DataVieModel instance</value>
		public DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;
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
		/// Gets or sets the type of the operator
		/// </summary>
		public ViewElementType OperatorType
		{
			get
			{
				return _operatorType;
			}
			set
			{
				_operatorType = value;
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseOperatorPopup));
            this.listBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox
            // 
            resources.ApplyResources(this.listBox, "listBox");
            this.listBox.Name = "listBox";
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // ChooseOperatorPopup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.listBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChooseOperatorPopup";
            this.Deactivate += new System.EventHandler(this.ChooseOperatorPopup_Deactivate);
            this.Load += new System.EventHandler(this.ChooseOperatorPopup_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ChooseOperatorPopup_Load(object sender, System.EventArgs e)
		{
			this.listBox.Items.Clear();

			switch (_operatorType)
			{
				case ViewElementType.And:
				case ViewElementType.Or:
					this.listBox.Items.AddRange(ViewLogicalExpr.Operators);
					break;
				case ViewElementType.Equals:
				case ViewElementType.NotEquals:
				case ViewElementType.LessThan:
				case ViewElementType.LessThanEquals:
				case ViewElementType.GreaterThan:
				case ViewElementType.GreaterThanEquals:
                case ViewElementType.Like:
                case ViewElementType.IsNull:
                case ViewElementType.IsNotNull:
					this.listBox.Items.AddRange(ViewRelationalExpr.Operators);
					break;
				case ViewElementType.In:
				case ViewElementType.NotIn:
					this.listBox.Items.AddRange(ViewInExpr.Operators);
					break;
			}
		}

		private void listBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_operator = (string) this.listBox.SelectedItem;
		
			// fire the accept event
			if (Accept != null)
			{
				Accept(this, EventArgs.Empty);
			}

			this.Close();
		}

		private void ChooseOperatorPopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
