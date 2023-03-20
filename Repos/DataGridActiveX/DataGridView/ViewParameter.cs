/*
* @(#)ViewParameter.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a parameter appeared in search filters.
	/// </summary>
	/// <version>1.0.1 29 May 2006</version>
	
	public class ViewParameter : DataGridViewElementBase
	{
		private string _ownerClassAlias;
		private ViewDataType _dataType;
		private string _value;

		/// <summary>
		/// Initiating an instance of ViewParameter class
		/// </summary>
		/// <param name="name">Name of an attribute that parameter is associated with</param>
		/// <param name="ownerClassAlias">The unique alias of DataClass element that owns the attribute</param>
		/// <param name="dataType">ViewParameter's data type. One of ViewDataType values</param>
		public ViewParameter(string name, string ownerClassAlias, ViewDataType dataType) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_dataType = dataType;
			_value = null;
		}

		/// <summary>
		/// Initiating an instance of ViewParameter class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewParameter(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets an alias of the class that owns this parameter
		/// </summary>
		public string ClassAlias
		{
			get
			{
				return _ownerClassAlias;
			}
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.Parameter;
			}
		}

		/// <summary>
		/// Gets or sets the value of ViewParameter
		/// </summary>
		/// <value>A value string.</value>
		public string ParameterValue
		{
			get
			{
				if (_value != null)
				{
					return _value;
				}
				else
				{
					return "";
				}
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Gets the attribute that is associated with the parameter
		/// </summary>
		/// <returns>ViewAttribute object</returns>
		public ViewAttribute GetAttribute()
		{
			return (ViewAttribute) this.DataGridView.ResultAttributes[this.Name];
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
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			visitor.VisitParameter(this);
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

			// set value of _value member
			_value = parent.GetAttribute("Value");

			// set the data type
			_dataType = (ViewDataType) Enum.Parse(typeof(ViewDataType), parent.GetAttribute("Type"));
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

			// write the value member
			if (_value != null && _value.Length > 0)
			{
				parent.SetAttribute("Value", _value);
			}

			parent.SetAttribute("Type", Enum.GetName(typeof(ViewDataType), _dataType));
		}

		/// <summary>
		/// Clone a paramter with a empty value
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ViewParameter cloned = null;

			// first convert the expr into xml
			XmlDocument doc = new XmlDocument();

			string elementName = Enum.GetName(typeof(ViewElementType), this.ElementType);

			XmlElement xmlElement = doc.CreateElement(elementName);

			doc.AppendChild(xmlElement);

			this.Marshal(xmlElement); // created a xml element tree

			// convert xml to a new ViewAttribute
			cloned = (ViewParameter) ViewElementFactory.Instance.Create(xmlElement);

			return cloned;
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			if (_value != null)
			{
				return _value;
			}
			else
			{
				return "";
			}
		}
	}
}