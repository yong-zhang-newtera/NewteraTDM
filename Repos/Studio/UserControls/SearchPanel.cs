using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio.UserControls
{
	/// <summary>
	/// Summary description for SearchPanel.
	/// </summary>
	public class SearchPanel : System.Windows.Forms.Panel
	{
		private System.Resources.ResourceManager resources;

		private const int LeftMargin = 3; // in pixals
		private const int TopMargin = 3; // in pixals
		private const int BetweenMargin = 7; // in pixals
		private const int RowHeight = 34; // in pixals
		private const int RelationalOperatorWidth = 40; // in pixals
		private const int ValueColumnWidth = 180; // in pixals
		private const int LogicalOperatorWidth = 60; // in pixals
		private const int Identation = 20; // in pixals

		private DataViewModel _dataView;
		private int _tabIndex = 0;
		private int _controlIndex = 0;
		private int _row = 0; // zero-based index
		private int _column = 0;
		private int _maxWidth = 30;
		private bool _isIndent = false;
		private SimpleAttributeElement _currentSimpleAttribute = null;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SearchPanel()
		{
			resources = new System.Resources.ResourceManager(typeof(SearchPanel));

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary>
		/// Gets or sets the currently selected data view of the search panel.
		/// </summary>
		/// <value>The DataViewModel instance.</value>
		public DataViewModel SelectedDataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
				Clear();
				if (_dataView != null)
				{
					Draw();
				}
			}
		}

		/// <summary>
		/// Reset the search panel to its original condition
		/// </summary>
		public void Reset()
		{
			foreach (Control control in this.Controls)
			{
				if (control is TextBox)
				{
					((TextBox) control).Text = "";
				}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SearchPanel
			// 
			this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));

		}
		#endregion

		#region Customized code

		/// <summary>
		/// Clear the controls existed in the search panel
		/// </summary>
		private void Clear()
		{
			_tabIndex = 0;
			_controlIndex = 0;
			_row = 0;
			_column = 0;
			this.Controls.Clear();
		}

		/// <summary>
		/// Draw the controls in the search panel vertically to reflect the search expressions
		/// contained by the currently selected data view.
		/// </summary>
		private void Draw()
		{
			DataViewElementCollection searchExprs = _dataView.FlattenedSearchFilters;

			this.SuspendLayout();

			_maxWidth = CalculateMaxAttributeNameWidth(searchExprs);

			foreach (IDataViewElement searchExpr in searchExprs)
			{
				Control control = GetControl(searchExpr);

				if (control != null)
				{
					this.Controls.Add(control);
				}
			}

			this.ResumeLayout(false);
		}

		/// <summary>
		/// Create a suitable control corresponding to a search expression
		/// </summary>
		/// <param name="searchExpr">The search expression</param>
		/// <returns>A UI control</returns>
		private Control GetControl(IDataViewElement searchExpr)
		{
			Control control = null;
			ComboBox comboBox;
			Label label;
			TextBox textBox;

			switch (searchExpr.ElementType)
			{
				case ElementType.And:
				case ElementType.Or:
					label = new Label();
					label.Text = resources.GetString(searchExpr.Name + ".Text");
					label.Name = "lable" + _controlIndex++;
					label.TextAlign = ContentAlignment.MiddleCenter;
					label.Location = GetLocation(_row, _column);
					label.Size = GetSize(_column);
					label.Visible = true;
					control = label;
					// start a new row
					_row++;
					_column = 0;
					
					// reset the member
					_currentSimpleAttribute = null;
					break;
				case ElementType.Equals:
				case ElementType.NotEquals:
				case ElementType.LessThan:
				case ElementType.GreaterThan:
				case ElementType.LessThanEquals:
				case ElementType.GreaterThanEquals:
                case ElementType.Like:
                case ElementType.IsNull:
                case ElementType.IsNotNull:
					if (!IsStaticOperator)
					{
						comboBox = new ComboBox();
						comboBox.Name = "comboBox" + _controlIndex++;
						comboBox.TabIndex = _tabIndex++;
						comboBox.Items.AddRange(RelationalExpr.Operators);
						comboBox.SelectedItem = ((RelationalExpr) searchExpr).Operator;
						comboBox.Location = GetLocation(_row, _column);
						comboBox.Size = GetSize(_column);
						comboBox.Visible = true;

						// Add a data binding between the combo box control and
						// Operator property of the expression
						comboBox.DataBindings.Add("Text", searchExpr, "Operator");
						control = comboBox;
					}
					else
					{
						label = new Label();
						label.Name = "label" + _controlIndex++;
						label.Text = ((RelationalExpr) searchExpr).Operator;
						label.Location = GetLocation(_row, _column);
						label.Size = GetSize(_column);
						label.Visible = true;
						control = label;
					}

					_column++;
					break;
				case ElementType.In:
				case ElementType.NotIn:
					comboBox = new ComboBox();
					comboBox.Name = "comboBox" + _controlIndex++;
					comboBox.TabIndex = _tabIndex++;
					comboBox.Items.AddRange(InExpr.Operators);
					comboBox.SelectedItem = ((InExpr) searchExpr).Operator;
					comboBox.Location = GetLocation(_row, _column);
					comboBox.Size = GetSize(_column);
					comboBox.Visible = true;

					// Add a data binding between the combo box control and
					// Operator property of the expression
					comboBox.DataBindings.Add("Text", searchExpr, "Operator");

					control = comboBox;
					_column++;
					break;
				case ElementType.Parameter:
					textBox = new TextBox();
					textBox.Name = "textBox" + _controlIndex++;
					textBox.Text = "";
					textBox.TabIndex = _tabIndex++;
					textBox.Location = GetLocation(_row, _column);
					textBox.Size = GetSize(_column);
					textBox.Visible = true;

					// Add a data binding between the text box control and
					// ParameterValue property of the parameter
					textBox.DataBindings.Add("Text", searchExpr, "ParameterValue");
					control = textBox;
					_column++;
					break;
				case ElementType.SimpleAttribute:
					label = new Label();
					label.Name = "lable" + _controlIndex++;
					label.Text = searchExpr.Caption;
					label.TextAlign = ContentAlignment.MiddleRight;
					label.Location = GetLocation(_row, _column);
					label.Size = GetSize(_column);
					label.Visible = true;
					control = label;

					// Get the corresponding schema model element
					string classAlias = ((DataSimpleAttribute) searchExpr).OwnerClassAlias;
					_currentSimpleAttribute = GetSimpleAttributeElement(classAlias, searchExpr.Name);
					_column++;
					break;
				case ElementType.RelationshipAttribute:
					label = new Label();
					label.Name = "lable" + _controlIndex++;
					label.Text = searchExpr.Caption;
					label.TextAlign = ContentAlignment.MiddleRight;
					label.Location = GetLocation(_row, _column);
					label.Size = GetSize(_column);
					label.Visible = true;
					control = label;
					_column++;
					break;
				case ElementType.RelationshipBegin:
					label = new Label();
					label.Name = "lable" + _controlIndex++;
					label.Text = searchExpr.Caption;
					label.TextAlign = ContentAlignment.MiddleRight;
					label.Location = GetLocation(_row, _column);
					label.Size = GetSize(_column);
					label.Visible = true;
					control = label;
					// start a new indented row
					_row++;
					_column = 0;
					_isIndent = true;
					break;
				case ElementType.RelationshipEnd:
					// cancel the indent
					_isIndent = false;
					break;
				case ElementType.LeftParenthesis:
					label = new Label();
					label.Name = "lable" + _controlIndex++;
					label.Text = "(";
					label.TextAlign = ContentAlignment.MiddleLeft;
					label.Location = GetLocation(_row, _column);
					label.Size = GetSize(_column);
					label.Visible = true;
					control = label;
					// Start a new row
					_row++;
					_column = 0;
					break;
				case ElementType.RightParenthesis:
					label = new Label();
					label.Name = "lable" + _controlIndex++;
					label.Text = ")";
					label.TextAlign = ContentAlignment.MiddleLeft;
					label.Location = GetLocation(_row, _column);
					label.Size = GetSize(_column);
					label.Visible = true;
					control = label;
					// start a new row
					_row++;
					_column = 0;
					break;
			}

			return control;
		}

		/// <summary>
		/// Gets a top-left point of a control
		/// </summary>
		/// <param name="row">The current row number</param>
		/// <param name="column">The current column number</param>
		/// <returns></returns>
		private Point GetLocation(int row, int column)
		{
			int x, y;

			if (_isIndent)
			{
				x = LeftMargin + Identation;
			}
			else
			{
				x = LeftMargin;
			}

			switch (column)
			{
				case 0:
					break;
				case 1:
					x += _maxWidth + BetweenMargin;
					break;
				case 2:
					x = x + _maxWidth + RelationalOperatorWidth + BetweenMargin * 2;
					break;
				case 3:
					x = x + _maxWidth + RelationalOperatorWidth + ValueColumnWidth + BetweenMargin * 3;
					break;
			}

			y = 10 + TopMargin * (row + 1) + row * RowHeight;

			return new Point(x, y);
		}

		/// <summary>
		/// Gets the size of a control
		/// </summary>
		/// <param name="column">The column number</param>
		/// <returns></returns>
		private Size GetSize(int column)
		{
			int height = RowHeight;
			int width = 100;

			switch (column)
			{
				case 0:
					width = _maxWidth;
					break;
				case 1:
					width = RelationalOperatorWidth;
					break;
				case 2:
					width = ValueColumnWidth;
					break;
				case 3:
					width = LogicalOperatorWidth;
					break;
			}

			return new Size(width, height);
		}

		/// <summary>
		/// Get the maximum width of attribute names.
		/// </summary>
		/// <param name="searchExprs">The search expressions</param>
		/// <returns>The maximun width</returns>
		private int CalculateMaxAttributeNameWidth(DataViewElementCollection searchExprs)
		{
			int maxWidth = this._maxWidth;

			using (Graphics g = this.CreateGraphics())
			{
				foreach(IDataViewElement expr in searchExprs)
				{
					if (expr.ElementType == ElementType.SimpleAttribute ||
						expr.ElementType == ElementType.RelationshipAttribute ||
						expr.ElementType == ElementType.RelationshipBegin)
					{
						SizeF size = g.MeasureString(expr.Caption, this.Font);

						if (size.Width > maxWidth)
						{
							maxWidth = (int) size.Width + 1;
						}
					}
				}
			}

			return maxWidth;
		}

		/// <summary>
		/// Get the schema model simple attribute
		/// </summary>
		/// <param name="classAlias">The class alias that identifies the class to which the attribute belongs.</param>
		/// <param name="simpleAttributeName">The name of the simple attribute</param>
		/// <returns>The corresponding SimpleAttributeElement</returns>
		private SimpleAttributeElement GetSimpleAttributeElement(string classAlias, string simpleAttributeName)
		{
			DataClass dataClass = _dataView.FindClass(classAlias);

			ClassElement classElement = _dataView.SchemaModel.FindClass(dataClass.ClassName);

			SimpleAttributeElement simpleAttributeElement = classElement.FindInheritedSimpleAttribute(simpleAttributeName);

			return simpleAttributeElement;
		}

		/// <summary>
		/// Gets the information indicating whether to create a static relational
		/// operator or a dynamic one.
		/// </summary>
		/// <value>True to create static operator, false to create dynamic one.</value>
		private bool IsStaticOperator
		{
			get
			{
				bool status = false;

				if (_currentSimpleAttribute != null)
				{
					if (_currentSimpleAttribute.DataType == DataType.Boolean)
					{
						status = true;
					}
				}

				return status;
			}
		}

		#endregion
	}
}
