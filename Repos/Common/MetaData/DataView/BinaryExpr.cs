/*
* @(#)BinaryExpr.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.MetaData;

	/// <summary>
	/// Represents a base class for all binary expressions in search filter 
	/// which has left and right operands.
	/// </summary>
	/// <version> 1.0.0 28 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public abstract class BinaryExpr : DataViewElementBase, IQueryElement
	{
		internal IDataViewElement _left = null;
		internal IDataViewElement _right = null;
		internal ElementType _type;
        private IDataViewElement _substituteExpr = null; // run-time use

		/// <summary>
		/// Initiate an instance of BinaryExpr class.
		/// </summary>
		public BinaryExpr() : base()
		{
			_left = null;
			_right = null;
            _substituteExpr = null;
		}

		/// <summary>
		/// Initiate an instance of BinaryExpr class.
		/// </summary>
		public BinaryExpr(IDataViewElement left, IDataViewElement right) : base()
		{
			_left = left;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (_left != null)
                {
                    _left.ValueChanged += new EventHandler(ValueChangedHandler);

                }
            }
			_right = right;
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (_right != null)
                {
                    _right.ValueChanged += new EventHandler(ValueChangedHandler);
                }
            }
		}

		/// <summary>
		/// Initiate an instance of BinaryExpr class.
		/// </summary>
		/// <param name="parent">The xml element represents the expr</param>
		internal BinaryExpr(XmlElement parent) : base()
		{
			Unmarshal(parent);
		}

		/// <summary>
		/// Gets or sets the DataViewModel that owns this element
		/// </summary>
		/// <value>DataViewModel object</value>
		public override DataViewModel DataView
		{
			get
			{
				return base.DataView;
			}
			set
			{
				base.DataView = value;
				if (_left != null && value != null)
				{
					_left.DataView = value;
				}

				if (_right != null && value != null)
				{
					_right.DataView = value;
				}
			}
		}

		/// <summary>
		/// Gets the element type
		/// </summary>
		/// <value>One of the ElementType values</value>
		public override ElementType ElementType
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the left operand of the binary expression
		/// </summary>
		public IDataViewElement Left
		{
			get
			{				
				return _left;
			}
			set
			{
				_left = value;

				if (_left != null && DataView != null)
				{
					_left.DataView = DataView;
                    if (GlobalSettings.Instance.IsWindowClient)
                    {
                        _left.ValueChanged += new EventHandler(this.ValueChangedHandler);
                    }
				}

                if (GlobalSettings.Instance.IsWindowClient)
                {
                    FireValueChangedEvent(value);
                }
			}
		}

		/// <summary>
		/// Gets the right operand of the binary expression
		/// </summary>
		public IDataViewElement Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;

				if (_right != null && DataView != null)
				{
					_right.DataView = DataView;
                    if (GlobalSettings.Instance.IsWindowClient)
                    {
                        _right.ValueChanged += new EventHandler(this.ValueChangedHandler);
                    }
				}

                if (GlobalSettings.Instance.IsWindowClient)
                {
                    FireValueChangedEvent(value);
                }
			}
		}

        /// <summary>
        /// Gets the information indicating whether the binary expression has a value(s) as
        /// the right expression of the operator.
        /// </summary>
        public virtual bool HasValue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the substitute expression for the binary expression, used mainly
        /// in case of binary expression that consists of relationship attribute
        /// </summary>
        public IDataViewElement SubstituteExpression
        {
            get
            {
                return _substituteExpr;
            }
            set
            {
                _substituteExpr = value;
            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
            if (_left != null)
            {
                _left.Accept(visitor);
            }

            visitor.VisitBinaryExpr(this);

            if (_right != null)
            {
                _right.Accept(visitor);
            }
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set the type memember
			_type = ElementFactory.ConvertStringToType(parent.Name);

			// the first child element is left operand element
			_left = ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (_left != null)
                {
                    _left.ValueChanged += new EventHandler(this.ValueChangedHandler);
                }
            }

			// The second child element is right operand element
			_right = ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                if (_right != null)
                {
                    _right.ValueChanged += new EventHandler(this.ValueChangedHandler);
                }
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			XmlElement child;
			base.Marshal(parent);

			if (_left != null)
			{
				// write the _left operand member
				child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_left.ElementType));
				_left.Marshal(child);
				parent.AppendChild(child);
			}

			if (_right != null)
			{
				// write the _right operand member
				child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_right.ElementType));
				_right.Marshal(child);
				parent.AppendChild(child);
			}
		}

		/// <summary>
		/// Gets or sets the binary expression's operator.
		/// </summary>
		/// <value>An operator symbol</value>
		public abstract string Operator {get; set;}


		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public abstract string ToXQuery();

		#endregion
	}
}