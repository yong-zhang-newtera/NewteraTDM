using System;
using System.Resources;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Newtera.DataGridActiveX.DataGridView;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// Summary description for SearchExprBuilder.
	/// </summary>
	public class SearchExprBuilder : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Popup closed handler
		/// </summary>
		public event EventHandler PopupClosed;

		private DataGridViewModel _dataGridView;
		private int _index;
		private ViewBinaryExpr _currentBinaryExpr;
		private IDataGridViewElement _currentExpr;
        private Label _msgLabel;

		private System.Windows.Forms.Label undo;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public SearchExprBuilder()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			_dataGridView = null;
			_index = 0;
			_currentBinaryExpr = null;
			_currentExpr = null;
            _msgLabel = null;
		}

		/// <summary>
		/// Gets or sets the data view that contains a search expression.
		/// </summary>
		public DataGridViewModel DataGridView
		{
			get
			{
				return _dataGridView;
			}
			set
			{
				_dataGridView = value;

				if (_dataGridView != null)
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
			if (_dataGridView != null)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SearchExprBuilder));
			this.undo = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			// 
			// undo
			// 
			this.undo.AccessibleDescription = resources.GetString("undo.AccessibleDescription");
			this.undo.AccessibleName = resources.GetString("undo.AccessibleName");
			this.undo.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("undo.Anchor")));
			this.undo.AutoSize = ((bool)(resources.GetObject("undo.AutoSize")));
			this.undo.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("undo.Dock")));
			this.undo.Enabled = ((bool)(resources.GetObject("undo.Enabled")));
			this.undo.Font = ((System.Drawing.Font)(resources.GetObject("undo.Font")));
			this.undo.Image = ((System.Drawing.Image)(resources.GetObject("undo.Image")));
			this.undo.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("undo.ImageAlign")));
			this.undo.ImageIndex = ((int)(resources.GetObject("undo.ImageIndex")));
			this.undo.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("undo.ImeMode")));
			this.undo.Location = ((System.Drawing.Point)(resources.GetObject("undo.Location")));
			this.undo.Name = "undo";
			this.undo.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("undo.RightToLeft")));
			this.undo.Size = ((System.Drawing.Size)(resources.GetObject("undo.Size")));
			this.undo.TabIndex = ((int)(resources.GetObject("undo.TabIndex")));
			this.undo.Text = resources.GetString("undo.Text");
			this.undo.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("undo.TextAlign")));
			this.toolTip.SetToolTip(this.undo, resources.GetString("undo.ToolTip"));
			this.undo.Visible = ((bool)(resources.GetObject("undo.Visible")));
			this.undo.Click += new System.EventHandler(this.undo_Click);
			// 
			// SearchExprBuilder
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "SearchExprBuilder";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
			this.SizeChanged += new System.EventHandler(this.SearchExprBuilder_SizeChanged);

		}
		#endregion

		#region Controller code

		/// <summary>
		/// Show the existing expression in the data view
		/// </summary>
		private void ShowExpression()
		{
			if (_dataGridView != null)
			{
                if (_msgLabel != null)
                {
                    this._msgLabel.Text = ""; // clear error message
                }

				Control[] controls;
				DataGridViewElementCollection exprItems = _dataGridView.FlattenedSearchFilters;

				this.SuspendLayout();

				this.Controls.Clear();

				foreach (IDataGridViewElement item in exprItems)
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
				link.Focus();

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
		private Control[] GetControls(IDataGridViewElement exprItem)
		{
			Control[] controls = null;
			LinkLabel link;
			Label label;

			switch (exprItem.ElementType)
			{
				case ViewElementType.And:
				case ViewElementType.Or:
					link = CreateLinkLabel(((ViewLogicalExpr) exprItem).Operator);
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;

					break;
				case ViewElementType.Equals:
				case ViewElementType.NotEquals:
				case ViewElementType.LessThan:
				case ViewElementType.GreaterThan:
				case ViewElementType.LessThanEquals:
				case ViewElementType.GreaterThanEquals:
                case ViewElementType.Like:
                case ViewElementType.IsNull:
                case ViewElementType.IsNotNull:
					link = CreateLinkLabel(((ViewRelationalExpr) exprItem).Operator);
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;
					break;
				case ViewElementType.In:
				case ViewElementType.NotIn:
					link = CreateLinkLabel(((ViewInExpr) exprItem).Operator);
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;					
					break;
				case ViewElementType.LeftEmptyOperand:
					link = CreateLinkLabel("?");
					link.Links[0].LinkData = exprItem;
					controls = new Control[1];
					controls[0] = link;					
					break;
				case ViewElementType.RightEmptyOperand:
					label = new Label();
					label.Text = "?";
					label.AutoSize = true;
					controls = new Control[1];
					controls[0] = label;
					break;
				case ViewElementType.Parameter:
					ViewParameter parameter = (ViewParameter) exprItem;
					if (parameter.ParameterValue != null && parameter.ParameterValue.Length > 0)
					{
						if (parameter.DataType != ViewDataType.String)
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
				case ViewElementType.SimpleAttribute:
					label = new Label();
					label.Text = exprItem.Caption;
					label.AutoSize = true;
					controls = new Control[1];
					controls[0] = label;
					break;
                case ViewElementType.RelationshipAttribute:
                    label = new Label();
                    label.Text = exprItem.Caption;
                    label.AutoSize = true;
                    controls = new Control[1];
                    controls[0] = label;
                    break;
				case ViewElementType.LeftParenthesis:
					label = new Label();
					label.Name = "lable" + _index++;
					label.AutoSize = true;
					label.Text = "(";
					controls = new Control[1];
					controls[0] = label;
					break;
				case ViewElementType.RightParenthesis:
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
				case ViewElementType.Comma:
					label = new Label();
					label.Name = "lable" + _index++;
					label.AutoSize = true;
					label.Text = ",";
					controls = new Control[1];
					controls[0] = label;
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
			IDataGridViewElement element = e.Link.LinkData as IDataGridViewElement;

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
					case ViewElementType.LeftEmptyOperand:
						_currentBinaryExpr = ((ViewLeftEmptyOperand) element).Parent;
						popup = new ChooseSearchAttributePopup();
						popup.DataGridView = _dataGridView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ViewElementType.And:
					case ViewElementType.Or:
						_currentBinaryExpr = (ViewBinaryExpr) element;
						popup = new ChooseOperatorPopup();
						((ChooseOperatorPopup) popup).OperatorType = element.ElementType;
						popup.DataGridView = _dataGridView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
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
						_currentBinaryExpr = (ViewBinaryExpr) element;
						popup = new ChooseOperatorPopup();
						((ChooseOperatorPopup) popup).OperatorType = element.ElementType;
						popup.DataGridView = _dataGridView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ViewElementType.In:
					case ViewElementType.NotIn:
						_currentBinaryExpr = (ViewBinaryExpr) element;
						popup = new ChooseOperatorPopup();
						((ChooseOperatorPopup) popup).OperatorType = element.ElementType;
						popup.DataGridView = _dataGridView;
						popup.Location = GetWindowCoordinate((Control) sender);
						popup.Accept += new EventHandler(this.Popup_Accept);
						popup.Show();
						break;
					case ViewElementType.Parameter:
						_currentExpr = element;
                        popup = null;
						ViewAttribute attribute = ((ViewParameter) element).GetAttribute();
                        if (attribute is ViewSimpleAttribute)
                        {
                            popup = GetParameterValuePopup((ViewSimpleAttribute) attribute);

                        }
                        else if (attribute is ViewRelationshipAttribute)
                        {
                            popup = new EnterValuePopup();
                        }

                        if (popup != null)
                        {
                            popup.Expression = ((ViewParameter)element).ParameterValue;
                            popup.DataGridView = _dataGridView;
                            popup.Location = GetWindowCoordinate((Control)sender);
                            popup.Accept += new EventHandler(this.Popup_Accept);
                            popup.Show();
                        }
						break;
					case ViewElementType.RightParenthesis:
						_currentExpr = ((ViewRightParenthesis) element).Parent;
						if (((ViewRightParenthesis) element).Parent is ViewParameterCollection)
						{
							// add a new parameter to the collection
							ViewParameterCollection parameters = (ViewParameterCollection) ((ViewRightParenthesis) element).Parent;
							if (parameters.Count > 0)
							{
								ViewParameter param = (ViewParameter) parameters[0];

								parameters.Add((ViewParameter) param.Clone());

								ShowExpression();
							}
						}
						else if (((ViewRightParenthesis) element).Parent is ViewParenthesizedExpr)
						{
							popup = new AddExprPopup();
							popup.Location = GetWindowCoordinate((Control) sender);
							popup.Accept +=new EventHandler(this.Popup_Accept);
							popup.Show();
						}

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
			IDataGridViewElement expr;

			if (sender is AddExprPopup)
			{
				expr = (IDataGridViewElement) ((IExprPopup) sender).Expression;

				if (_currentExpr != null && _currentExpr is ViewParenthesizedExpr)
				{
					// adding an expression to a Parenthesized expression
					((ViewParenthesizedExpr) _currentExpr).AddSearchExpr(expr, ViewElementType.And);
					_currentExpr = null;
				}
				else
				{
					_dataGridView.AddSearchExpr(expr, ViewElementType.And);
				}

				ShowExpression();
			}
			else if (sender is ChooseSearchAttributePopup)
			{
                ViewSimpleAttribute simpleAttribute = ((IExprPopup)sender).Expression as ViewSimpleAttribute;
                ViewRelationshipAttribute relationshipAttribute = ((IExprPopup)sender).Expression as ViewRelationshipAttribute;
                ViewParameter parameter = null;

				if (_currentBinaryExpr != null)
				{
                    bool isGoodForLikeOperator = false;

                    if (simpleAttribute != null)
                    {
                        _currentBinaryExpr.Left = simpleAttribute;
                        if (simpleAttribute.IsMultipleChoice)
                        {
                            // specially make the parameter a string type for attribute with multiple choice
                            parameter = new ViewParameter(simpleAttribute.Name, simpleAttribute.OwnerClassAlias, ViewDataType.String);
                        }
                        else
                        {
                            parameter = new ViewParameter(simpleAttribute.Name, simpleAttribute.OwnerClassAlias,
                                simpleAttribute.DataType);

                            // only the simple attribute with String type and without any
                            // constrain can be used for like operator
                            if (simpleAttribute.DataType == ViewDataType.String &&
                                !simpleAttribute.HasEnumConstraint)
                            {
                                isGoodForLikeOperator = true;
                            }
                        }
                    }
                    else if (relationshipAttribute != null)
                    {
                        _currentBinaryExpr.Left = relationshipAttribute;

                        parameter = new ViewParameter(relationshipAttribute.Name, relationshipAttribute.OwnerClassAlias,
                                                        relationshipAttribute.DataType);
                    }

                    if (_currentBinaryExpr is ViewRelationalExpr &&
                        _currentBinaryExpr.ElementType == ViewElementType.Like)
                    {
                        if (!isGoodForLikeOperator)
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
					if (_currentBinaryExpr is ViewInExpr)
					{
						// the right operand is a collection of parameters
						ViewParameterCollection parameters = new ViewParameterCollection();
						parameters.Add(parameter);
						_currentBinaryExpr.Right = parameters;
					}
                    else if (_currentBinaryExpr.ElementType != ViewElementType.IsNull &&
                        _currentBinaryExpr.ElementType !=ViewElementType.IsNotNull)
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

				if (_currentExpr != null && _currentExpr is ViewParameter)
				{
                    ViewParameter parameter = (ViewParameter)_currentExpr;
                    if (sender is EnterValuePopup && !IsParameterValueValid(parameter, theValue))
                    {
                        string msg = string.Format(MessageResourceManager.GetString("ActiveXControl.InvalidParameterValue"), parameter.DataType.ToString());
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
		/// Gets an appropriate popup for inputing a value of a parameter. The type
		/// of popup depends on the information provided by the schema model element.
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		private IExprPopup GetParameterValuePopup(ViewSimpleAttribute attribute)
		{
			IExprPopup popup;

			if (attribute.DataType == ViewDataType.Boolean)
			{
				ChooseBooleanValuePopup boolPopup = new ChooseBooleanValuePopup();
				popup = boolPopup;
			}
			else if (attribute.DataType == ViewDataType.Date ||
				attribute.DataType == ViewDataType.DateTime)
			{
				ChooseDateTimePopup dateTimePopup = new ChooseDateTimePopup();
				popup = dateTimePopup;
			}
			else if (attribute.HasEnumConstraint)
			{
				ChooseEnumValuePopup enumPopup = new ChooseEnumValuePopup();
				enumPopup.EnumValues = attribute.EnumConstraint.DisplayTexts;
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
        private bool IsParameterValueValid(ViewParameter parameter, string val)
        {
            bool status = true;

            if (!string.IsNullOrEmpty(val))
            {
                // we don't need to check certain data types, such as bool, date, datetime
                // since the value are selected instead of being entered. we do not need to
                // worry about string type either.
                switch (parameter.DataType)
                {
                    case ViewDataType.Byte:
                        try
                        {
                            byte.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case ViewDataType.Decimal:
                        try
                        {
                            decimal.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case ViewDataType.Double:
                        try
                        {
                            double.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case ViewDataType.Float:
                        try
                        {
                            float.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case ViewDataType.Integer:
                        try
                        {
                            int.Parse(val);
                        }
                        catch (Exception)
                        {
                            status = false;
                        }

                        break;

                    case ViewDataType.BigInteger:
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

		#endregion

		private void SearchExprBuilder_SizeChanged(object sender, System.EventArgs e)
		{
			RelocateControls();	
		}

		private void undo_Click(object sender, System.EventArgs e)
		{
			_dataGridView.RemoveLastSearchExpr();
			ShowExpression();	
		}
	}
}
