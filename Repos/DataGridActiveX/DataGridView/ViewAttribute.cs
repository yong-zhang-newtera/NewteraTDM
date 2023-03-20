/*
* @(#)ViewAttribute.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Collections.Specialized;
	using System.Xml;

	/// <summary>
	/// A ViewAttribute instance represents an attribute, including simple or array, in a class view.
	/// </summary>
	/// <version>1.0.1 28 May 2006</version>
	public abstract class ViewAttribute : DataGridViewElementBase
	{
		private string _ownerClassAlias;
		private string _value;
		private ViewDataType _dataType;
		private bool _visible;

		/// <summary>
		/// Initiating an instance of ViewAttribute class
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		/// <param name="ownerClassAlias">Owner class alias</param>
		public ViewAttribute(string name, string ownerClassAlias) : this(name, ViewDataType.Unknown, ownerClassAlias)
		{
		}

		/// <summary>
		/// Initiating an instance of ViewAttribute class
		/// </summary>
		/// <param name="name">Name of the attribute</param>
		/// <param name="dataType">Data Type</param>
		/// <param name="ownerClassAlias">The class alias</param>
		public ViewAttribute(string name, ViewDataType dataType, string ownerClassAlias) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_dataType = dataType;
			_value = null; // run-time use only
			_visible = true;
		}

		/// <summary>
		/// Initiating an instance of ViewAttribute class
		/// </summary>
		internal ViewAttribute() : base()
		{
            _ownerClassAlias = null;
            _dataType = ViewDataType.Unknown;
            _visible = true;
			_value = null; // run-time use only
		}

		/// <summary>
		/// Gets alias of class that owns this attribute
		/// </summary>
		/// <value>Owner class alias</value>
		public string OwnerClassAlias
		{
			get
			{
				return _ownerClassAlias;
			}
		}

		/// <summary>
		/// Gets the data type of a parameter
		/// </summary>
		/// <value>One of ViewDataType values</value>
		public ViewDataType DataType
		{
			get
			{
				return _dataType;
			}
			set
			{
				_dataType = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the attribute has an value
		/// </summary>
		/// <value>true if it has, false, otherwise</value>
		public bool HasValue
		{
			get
			{
				if (_value != null && _value.Length > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the data type of attribute is
		/// numeric, including integer, decimal, float, double, etc.
		/// </summary>
		/// <value>true if it is a numeric attribute, false otherwise.</value>
		public bool IsNumeric
		{
			get
			{
				bool status = false;
				
				if (_dataType == ViewDataType.Decimal ||
					_dataType == ViewDataType.Double ||
					_dataType == ViewDataType.Float ||
					_dataType == ViewDataType.Integer)
				{
					status = true;
				}

				return status;
			}
		}

		/// <summary>
		/// Gets or sets the value of an attribute.
		/// </summary>
		/// <value>The attribute value</value>
		/// <remarks> Run-time use only, no need to write to class view xml</remarks>
		public string AttributeValue
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the attribute is visible in datagrid.
		/// </summary>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of _ownerClassAlias member
			_ownerClassAlias = parent.GetAttribute("OwnerAlias");

			// set the data type
            string dataTypeStr = parent.GetAttribute("Type");
            if (!string.IsNullOrEmpty(dataTypeStr))
            {
                _dataType = (ViewDataType)Enum.Parse(typeof(ViewDataType), dataTypeStr);
            }
            else
            {
                // default to string
                _dataType = ViewDataType.String;
            }

			string str = parent.GetAttribute("visible");
			if (str != null && str == "true")
			{
				this._visible = true;
			}
			else
			{
				this._visible = false;
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _ownerName member
			parent.SetAttribute("OwnerAlias", _ownerClassAlias);

			parent.SetAttribute("Type", Enum.GetName(typeof(ViewDataType), _dataType));

			if (this._visible)
			{
				parent.SetAttribute("visible", "true");
			}
		}
	}
}