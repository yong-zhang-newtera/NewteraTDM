using System;
using System.Resources;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// Summary description for SearchExprBuilder.
	/// </summary>
	public class SearchExprBuilder : System.Windows.Forms.UserControl
	{
        public event EventHandler PopupClosed;

		private DataViewModel _dataView;
		private int _index;
		private BinaryExpr _currentBinaryExpr;
		private IDataViewElement _currentExpr;
        private Label _msgLabel;
        private bool _isGoodForLikeOperator;

		private System.Windows.Forms.Label undo;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public SearchExprBuilder()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			_dataView = null;
			_index = 0;
			_currentBinaryExpr = null;
			_currentExpr = null;
            _msgLabel = null;
		}

		/// <summary>
		/// Gets or sets the data view that contains a search expression.
		/// </summary>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;

				if (_dataView != null)
				{
					ShowExpression(); // show expression in data view
				}
			}
		}

        /// <summary>
        /// Gets or sets the label to display a message.
        /// </summary>
        public Label MessageLable
        {
            get
            {
                return _msgLabel;
            }
            set
            {
                _msgLabel = value;
            }
        }

		/// <summary>
		/// Display the serach expression of a dataview
		/// </summary>
		public void DisplaySearchExpression()
		{
			if (_dataView != null)
			{
				ShowExpression();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchExprBuilder));
            this.undo = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // undo
            // 
            resources.ApplyResources(this.undo, "undo");
            this.undo.Name = "undo";
            this.toolTip.SetToolTip(this.undo, resources.GetString("undo.ToolTip"));
            this.undo.Click += new System.EventHandler(this.undo_Click);
            // 
            // SearchExprBuilder
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "SearchExprBuilder";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.SizeChanged += new System.EventHandler(this.SearchExprBuilder_SizeChanged);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Show the existing expression in the data view
		/// </summary>
		private void ShowExpression()
		{
			if (_dataView != null)
			{
                if (_msgLabel != null)
                {
                    this._msgLabel.Text = ""; // clear error message
                }

				Control[] controls;
				DataViewElementCollection exprItems = _dataView.FlattenedSearchFilters;

				this.SuspendLayout();

				this.Controls.Clear();

				foreach (IDataViewElement item in exprItems)
				{
					controls = GetControls(item);

					if (controls != null)
					{
						this.Controls.AddRange(controls);
					}
				}

				// add an Append link at the end
				LinkLabel link = CreateLinkLabel("...");
				link.Links[0].LinkData = "AddExpr";
				this.Controls.Add(link);
                link.Focus(); // so that the dialog won't lose the focus

				// add a undo image to delete an expression
				if (exprItems.Count > 0)
				{
					this.Controls.Add(undo);
				}

				RelocateControls(); // relocate the controls to fit in the client area

				this.ResumeLayout();
			}
		}

		/// <summary>
		/// Create UI control(s) representing an expression item
		/// </summary>
		/// <param name="exprItem">An search expression item</param>
		/// <returns>A Control object</returns>
		private Control[] GetControls(IDataViewElement exprItem)
		{
			Control[] controls = null;
			LinkLabel link;
			Label label;

			switch (exprItem.ElementType)
			{
				case ElementType.And:
				case ElementType.Or:
					link = CreateLinkLabel(((LogicalExpr) exprItem).Operator);
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;

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
                    link = CreateLinkLabel(((RelationalExpr)exprItem).Operator);
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;
					break;
				case ElementType.In:
				case ElementType.NotIn:
					link = CreateLinkLabel(((InExpr) exprItem).Operator);
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;					
					break;
				case ElementType.LeftEmptyOperand:
					link = CreateLinkLabel("?");
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;					
					break;
				case ElementType.RightEmptyOperand:
					label = new Label();
					label.Text = "?";
					label.AutoSize = true;
					controls = new Control[1];
					controls[0] = label;
					break;
				case ElementType.Parameter:
					Parameter parameter = (Parameter) exprItem;
					if (parameter.ParameterValue != null && parameter.ParameterValue.Length > 0)
					{
						if (parameter.DataType != DataType.String)
						{
							link = CreateLinkLabel(parameter.ParameterValue);
						}
						else
						{
							link = CreateLinkLabel("\"" + parameter.ParameterValue + "\"");
						}
					}
					else
					{
						link = CreateLinkLabel("?");
					}
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;
					break;
				case ElementType.SimpleAttribute:
					label = new Label();
					label.Text = exprItem.Caption;
					label.AutoSize = true;
					controls = new Control[1];
					controls[0] = label;
					break;
                case ElementType.ArrayAttribute:
                    label = new Label();
                    label.Text = exprItem.Caption;
                    label.AutoSize = true;
                    controls = new Control[1];
                    controls[0] = label;
                    break;
				case ElementType.RelationshipAttribute:
                    label = new Label();
                    label.Text = exprItem.Caption;
                    label.AutoSize = true;
                    controls = new Control[1];
                    controls[0] = label;
					break;
				case ElementType.LeftParenthesis:
					label = new Label();
					label.Name = "lable" + _index++;
					label.AutoSize = true;
					label.Text = "(";
					controls = new Control[1];
					controls[0] = label;
					break;
				case ElementType.RightParenthesis:
					link = CreateLinkLabel("...");
					link.Links[0].LinkData = exprItem;
					controls = new Control[2];
					controls[0] = link;
					label = new Label();
					label.Name = "lable" + _index++;
					label.AutoSize = true;
					label.Text = ")";
					controls[1] = label;
					break;
				case ElementType.Comma:
					label = new Label();
					label.Name = "lable" + _index++;
					label.AutoSize = true;
					label.Text = ",";
					controls = new Control[1];
					controls[0] = label;
					break;
                case ElementType.WFState:
                    label = new Label();
                    label.Text = exprItem.Caption;
                    label.AutoSize = true;
                    controls = new Control[1];
                    controls[0] = label;
                    break;
                case ElementType.Before:
                    controls = new Control[3];
                    label = new Label();
                    label.Name = "lable" + _index++;
                    label.Text = exprItem.Caption + "(";
                    label.AutoSize = true;
                    controls[0] = label;
                    if (((IFunctionElement)exprItem).AttributeName != null)
                    {
                        // attribute has been selected, show the attribute caption
                        label = new Label();
                        label.Text = ((IFunctionElement)exprItem).AttributeCaption;
                        label.AutoSize = true;
                        controls[1] = label;
                    }
                    else
                    {
                        // attribute has not been selected, show a question mark
                        link = CreateLinkLabel("?");
                        link.Links[0].LinkData = exprItem;
                        controls[1] = link;
                    }
                    label = new Label();
                    label.Name = "lable" + _index++;
                    label.AutoSize = true;
                    label.Text = ")";
                    controls[2] = label;
                    break;
			}

			return controls;
		}

		/// <summary>
		/// Relocate the controls to fit the client area for display.
		/// </summary>
		private void RelocateControls()
		{
			int lineWidth = this.ClientSize.Width;
			int margin = 5;
			int x = 0;
			int y = 0;

			foreach (Control control in this.Controls)
			{
				control.Location = new Point(x, y);
				
				// decide a location for the next control
				if (x + control.Size.Width + margin > lineWidth)
				{
					// start a new line
					x = 0;
					y += control.Size.Height + 2;
				}
				else
				{
					x += control.Size.Width;
				}
			}
		}

		private LinkLabel CreateLinkLabel(string text)
		{
			LinkLabel link = new LinkLabel();
			link.Name = "linkLabel" + _index++;

			// Configure the appearance.
			link.DisabledLinkColor = System.Drawing.Color.Silver;
			link.VisitedLinkColor = System.Drawing.Color.Blue;
			link.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			link.LinkColor = System.Drawing.Color.Navy;
			link.TabIndex = 0;
			link.TabStop = true;
			link.Text = text;
			link.AutoSize = true;

			// Add an event handler to do something when the links are clicked.
			link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);

			// Identify what the first Link is.
			link.LinkArea = new LinkArea(0, text.Length);

			return link;
		}

		private void linkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			IExprPopup popup;

			// Display the appropriate popup based on the value of the 
			// LinkData property of the Link object.
			string target = e.Link.LinkData as string;
			IDataViewElement element = e.Link.LinkData as IDataViewElement;

			if (target != null)
			{
				if (target == "AddExpr")
				{
					popup = new AddExprPopup();
					popup.Location = GetWindowCoordinate((Control) sender);
					popup.Accept +=new EventHandler(this.Popup_Accept);
					popup.Show();
				}
			}
			else if (element != null)
			{
				switch (element.ElementType)
				{
					case ElementType.LeftEmptyOperand:
						_currentBinaryExpr = element.ParentElement as BinaryExpr;
						popup = new ChooseSearchAttributePopup();
						popup.DataView = _dataView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ElementType.And:
					case ElementType.Or:
						_currentBinaryExpr = (BinaryExpr) element;
						popup = new ChooseOperatorPopup();
						((ChooseOperatorPopup) popup).OperatorType = element.ElementType;
						popup.DataView = _dataView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ElementType.Equals:
					case ElementType.NotEquals:
					case ElementType.LessThan:
					case ElementType.LessThanEquals:
					case ElementType.GreaterThan:
					case ElementType.GreaterThanEquals:
                    case ElementType.Like:
                    case ElementType.IsNull:
                    case ElementType.IsNotNull:
						_currentBinaryExpr = (BinaryExpr) element;
						popup = new ChooseOperatorPopup();
						((ChooseOperatorPopup) popup).OperatorType = element.ElementType;
						popup.DataView = _dataView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ElementType.In:
					case ElementType.NotIn:
						_currentBinaryExpr = (BinaryExpr) element;
						popup = new ChooseOperatorPopup();
						((ChooseOperatorPopup) popup).OperatorType = element.ElementType;
						popup.DataView = _dataView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ElementType.Parameter:
						_currentExpr = element;
						SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) ((Parameter) element).GetSchemaModelElement();
						popup = GetParameterValuePopup(((Parameter) element).DataType, schemaModelElement);
						popup.Expression = ((Parameter) element).ParameterValue;
						popup.DataView = _dataView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ElementType.RightParenthesis:
						_currentExpr = element.ParentElement;
						if (((RightParenthesis) element).ParentElement is ParameterCollection)
						{
							// add a new parameter to the collection
                            ParameterCollection parameters = (ParameterCollection)((RightParenthesis)element).ParentElement;
							if (parameters.Count > 0)
							{
								Parameter param = (Parameter) parameters[0];

								parameters.Add((Parameter) param.Clone());

								ShowExpression();
							}
						}
                        else if (((RightParenthesis)element).ParentElement is ParenthesizedExpr)
						{
							popup = new AddExprPopup();
							popup.Location = GetWindowCoordinate((Control) sender);
							popup.Accept +=new EventHandler(this.Popup_Accept);
							popup.Show();
						}

						break;
                    case ElementType.Before:
                        _currentBinaryExpr = element.ParentElement as BinaryExpr;
                        _currentExpr = element;
                        popup = new ChooseSearchAttributePopup();
                        popup.DataView = _dataView;
                        popup.Location = GetWindowCoordinate((Control)sender);
                        popup.Accept += new EventHandler(this.Popup_Accept);
                        popup.Show();
                        break;
				}
			}
		}

		/// <summary>
		/// Get the coordinates of a popup related to the window
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		private Point GetWindowCoordinate(Control control)
		{
			int x = 0;
			int y = control.Size.Height * 2;
			
			while (control != null)
			{
				x += control.Location.X;
				y += control.Location.Y;

				control = control.Parent;
			}

			return new Point(x, y);
		}

		/// <summary>
		/// Handle the accept event from popups
		/// </summary>
		/// <param name="sender">The popup object</param>
		/// <param name="e"></param>
		private void Popup_Accept(object sender, EventArgs e)
		{
			IDataViewElement expr;

			if (sender is AddExprPopup)
			{
				expr = (IDataViewElement) ((IExprPopup) sender).Expression;

				if (_currentExpr != null && _currentExpr is ParenthesizedExpr)
				{
					// adding an expression to a Parenthesized expression
					((ParenthesizedExpr) _currentExpr).AddSearchExpr(expr, ElementType.And);
					_currentExpr = null;
				}
				else
				{
					_dataView.AddSearchExpr(expr, ElementType.And);
				}

				ShowExpression();
			}
			else if (sender is ChooseSearchAttributePopup)
			{
				expr = (IDataViewElement) ((IExprPopup) sender).Expression;
                Parameter parameter = null;

				if (_currentBinaryExpr != null)
				{
                    _isGoodForLikeOperator = false;
                    if (expr is IFunctionElement)
                    {
                        _currentBinaryExpr.Left = expr;

                        // expr is a function element
                        parameter = new Parameter(expr.Name, null, ((IFunctionElement)expr).DataType);
                    }
                    else if (_currentExpr != null && _currentExpr is IFunctionElement)
                    {
                        // the selected attribute is used as a parameter of a function
                        ((IFunctionElement)_currentExpr).AttributeName = expr.Name;
                        ((IFunctionElement)_currentExpr).AttributeCaption = expr.Caption;
                        
                        parameter = CreateParameter(expr);

                        // change the data type of the function element to that of the parameter
                        ((IFunctionElement)_currentExpr).DataType = parameter.DataType;

                        // the right is a single parameter
                        _currentBinaryExpr.Right = parameter;

                        _currentExpr = null;
                    }
                    else
                    {
                        _currentBinaryExpr.Left = expr;

                        parameter = CreateParameter(expr);
                    }

                    if (_currentBinaryExpr is RelationalExpr &&
                        _currentBinaryExpr.ElementType == ElementType.Like)
                    {
                        if (!_isGoodForLikeOperator)
                        {
                            if (_msgLabel != null)
                            {
                                _msgLabel.Text = MessageResourceManager.GetString("WindowsControl.InvalidLikeAttribute");
                            }

                            _currentBinaryExpr.Left = null;
                            return;
                        }
                        else
                        {
                            // the right is a single parameter
                            _currentBinaryExpr.Right = parameter;
                        }
                    }
					else if (_currentBinaryExpr is InExpr)
					{
						// the right operand is a collection of parameters
						ParameterCollection parameters = new ParameterCollection();
						parameters.Add(parameter);
						_currentBinaryExpr.Right = parameters;
					}
                    else if (_currentBinaryExpr.ElementType != ElementType.IsNull &&
                        _currentBinaryExpr.ElementType != ElementType.IsNotNull)
                    {
                        // the right is a single parameter
                        _currentBinaryExpr.Right = parameter;
                    }
                    else
                    {
                        _currentBinaryExpr.Right = null;
                    }

					_currentBinaryExpr = null;
				}

				ShowExpression();
			}
			else if (sender is ChooseOperatorPopup)
			{
				string theOperator = (string) ((IExprPopup) sender).Expression;

				if (_currentBinaryExpr != null)
				{
					_currentBinaryExpr.Operator = theOperator;
				}

				ShowExpression();
			}
			else if (sender is ChooseBooleanValuePopup ||
					 sender is ChooseEnumValuePopup ||
					 sender is ChooseDateTimePopup ||
					 sender is EnterValuePopup)
			{
				string theValue = (string) ((IExprPopup) sender).Expression;

				if (_currentExpr != null && _currentExpr is Parameter)
				{
                    Parameter parameter = (Parameter)_currentExpr;

                    if (sender is EnterValuePopup && !IsParameterValueValid(parameter, theValue))
                    {
                        string msg = string.Format(MessageResourceManager.GetString("WindowsControl.InvalidParameterValue"), parameter.DataType.ToString());
                        if (_msgLabel != null)
                        {
                            _msgLabel.Text = msg;
                        }
                        return;
                    }

					parameter.ParameterValue = theValue;

					_currentExpr = null;
				}

				ShowExpression();
			}

            if (PopupClosed != null)
            {
                // Raise the event if the popup is closed
                PopupClosed(this, null);
            }
		}

		/// <summary>
		/// Gets an appropriate popup for inputing a value of a parameter of a certain data type.
		/// </summary>
        /// <param name="dataType">One of DataType enum values</param>
		/// <param name="schemaModelElement"></param>
		/// <returns></returns>
		private IExprPopup GetParameterValuePopup(DataType dataType, SimpleAttributeElement schemaModelElement)
		{
			IExprPopup popup;

            if (dataType == DataType.Boolean && schemaModelElement != null)
			{
				Type booleanEnumType = EnumTypeFactory.Instance.Create(schemaModelElement);

				ChooseBooleanValuePopup boolPopup = new ChooseBooleanValuePopup();
				boolPopup.EnumType = booleanEnumType;
				popup = boolPopup;
			}
            else if (dataType == DataType.Date || dataType == DataType.DateTime)
			{
				ChooseDateTimePopup dateTimePopup = new ChooseDateTimePopup();
				popup = dateTimePopup;
			}
            else if (schemaModelElement != null && schemaModelElement.Constraint != null &&
			     schemaModelElement.Constraint is EnumElement &&
				 schemaModelElement.ConstraintUsage == ConstraintUsage.Restriction)
			{
				Type enumType = EnumTypeFactory.Instance.Create(schemaModelElement);

				ChooseEnumValuePopup enumPopup = new ChooseEnumValuePopup();
				enumPopup.EnumType = enumType;
				popup = enumPopup;
			}
			else
			{
				popup = new EnterValuePopup();
			}

			return popup;
		}

        /// <summary>
        /// Gets the information indicating whether the parameter value meets the data type
        /// requirements
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <param name="val">The value</param>
        /// <returns></returns>
        private bool IsParameterValueValid(Parameter parameter, string val)
        {
            bool status = true;

            if (!string.IsNullOrEmpty(val))
            {
                // if the val enclosed with { }, then it is a variable, do not validate the value
                if (val.StartsWith("{") && val.EndsWith("}"))
                {
                    return status;
                }

                // we don't need to check certain data types, such as bool, date, datetime
                // since the value are selected instead of being entered. we do not need to
                // worry about string type either.
                switch (parameter.DataType)
                {
                    case DataType.Byte:
                        try
                        {
                            byte.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case DataType.Decimal:
                        try
                        {
                            decimal.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case DataType.Double:
                        try
                        {
                            double.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case DataType.Float:
                        try
                        {
                            float.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case DataType.Integer:
                        try
                        {
                            int.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case DataType.BigInteger:
                        try
                        {
                            long.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;
                }
            }

            return status;
        }

        private Parameter CreateParameter(IDataViewElement expr)
        {
            Parameter parameter = null;

            // expr is an attribute element
            AttributeElementBase schemaModelElement = (AttributeElementBase)expr.GetSchemaModelElement();

            // selected attribute could be a simple attribute or relationship attribute
            if (schemaModelElement is SimpleAttributeElement)
            {
                SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)schemaModelElement;

                if (simpleAttribute.IsMultipleChoice)
                {
                    // specially make the parameter a string type for attribute with multiple choice
                    parameter = new Parameter(expr.Name, ((DataSimpleAttribute)expr).OwnerClassAlias,
                        DataType.String);
                }
                else
                {
                    parameter = new Parameter(expr.Name, ((DataSimpleAttribute)expr).OwnerClassAlias,
                        schemaModelElement.DataType);

                    // only the simple attribute with String type and without any
                    // constrain can be used for like operator
                    if (simpleAttribute.DataType == DataType.String &&
                        simpleAttribute.Constraint == null)
                    {
                        _isGoodForLikeOperator = true;
                    }
                }
            }
            else if (schemaModelElement is ArrayAttributeElement)
            {
                ArrayAttributeElement arrayAttribute = (ArrayAttributeElement)schemaModelElement;

                parameter = new Parameter(expr.Name, ((DataArrayAttribute)expr).OwnerClassAlias,
                    schemaModelElement.DataType);
            }
            else if (schemaModelElement is RelationshipAttributeElement)
            {
                parameter = new Parameter(expr.Name, ((DataRelationshipAttribute)expr).OwnerClassAlias,
                    schemaModelElement.DataType);
            }

            return parameter;
        }

		#endregion

		private void SearchExprBuilder_SizeChanged(object sender, System.EventArgs e)
		{
			RelocateControls();	
		}

		private void undo_Click(object sender, System.EventArgs e)
		{
			_dataView.RemoveLastSearchExpr();
			ShowExpression();	
		}
	}
}
