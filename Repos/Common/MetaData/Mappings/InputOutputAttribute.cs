/*
* @(#)InputOutputAttribute.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Mappings.Transform;

	/// <summary>
	/// The class represents a single source attribute or a destination attribute
	/// that is an input or output of MultiAttributeMapping instance.
	/// </summary>
	/// <version>1.0.0 15 Nov 2004</version>
	/// <author> Yong Zhang </author>
	public class InputOutputAttribute : MappingNodeBase
	{
		private SetterType _setterType;
		private GetterType _getterType;
		private string _value = null; // run-time use only
		private string _attributeName;
		private int _rowIndex = -1; // obtained from destination attribute name
		private int _colIndex = -1; // obtained from destination attribute name
		private string _relationshipAttributeName = null; // obtained dynamically
		
		/// <summary>
		/// Initiate an instance of InputOutputAttribute class.
		/// </summary>
		/// <param name="attributeName">Input or output attribute name.</param>
		public InputOutputAttribute(string attributeName) : base()
		{
			_attributeName = attributeName;
			_setterType = SetterType.Unknown;
			_getterType = GetterType.SimpleAttributeGetter; // default
		}

		/// <summary>
		/// Initiating an instance of InputOutputAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal InputOutputAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets name of the destination attribute associated with a InputOutputAttribute
		/// </summary>
		public virtual string AttributeName
		{
			get
			{
				return _attributeName;
			}
			set
			{
				_attributeName = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets value of the destination attribute associated with a InputOutputAttribute
		/// </summary>
		public virtual string AttributeValue
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
		/// Gets or sets the type of attribute setter.
		/// </summary>
		/// <value>One of SetterType enum values</value>
		public SetterType SetterType
		{
			get
			{
				return _setterType;
			}
			set
			{
				_setterType = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of attribute getter.
		/// </summary>
		/// <value>One of GetterType enum values</value>
		public GetterType GetterType
		{
			get
			{
				return _getterType;
			}
			set
			{
				_getterType = value;
			}
		}

		/// <summary>
		/// Gets the row index of the array data cell
		/// </summary>
		public int RowIndex
		{
			get
			{
				if (_rowIndex < 0)
				{
					GetIndex();
				}

				return _rowIndex;
			}
		}

		/// <summary>
		/// Gets the column index of the array data cell
		/// </summary>
		public int ColIndex
		{
			get
			{
				if (_colIndex < 0)
				{
					GetIndex();
				}

				return _colIndex;
			}
		}

		/// <summary>
		/// Gets name of the relationship attribute associated with a PrimaryKeyMapping
		/// </summary>
		public string RelationshipAttributeName
		{
			get
			{
				if (_relationshipAttributeName == null)
				{
					int pos = _attributeName.IndexOf("_");
					if (pos > 0)
					{
						_relationshipAttributeName = _attributeName.Substring(0, pos);
					}
					else
					{
						_relationshipAttributeName = _attributeName;
					}
				}

				return _relationshipAttributeName;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.InputOutputAttribute;
			}
		}

		/// <summary>
		/// create an InputOutputAttribute from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Name");
			if (str != null && str.Length > 0)
			{
				_attributeName = str;
			}
			else
			{
				_attributeName = null;
			}

			str = parent.GetAttribute("SetterType");
			if (str != null && str.Length > 0)
			{
				_setterType = (SetterType) Enum.Parse(typeof(SetterType), str);
			}
			else
			{
				_setterType = SetterType.Unknown;
			}

			str = parent.GetAttribute("GetterType");
			if (str != null && str.Length > 0)
			{
				_getterType = (GetterType) Enum.Parse(typeof(GetterType), str);
			}
			else
			{
				_getterType = GetterType.SimpleAttributeGetter; // default value
			}
		}

		/// <summary>
		/// write InputOutputAttribute to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _attributeName
			if (_attributeName != null && _attributeName.Length > 0)
			{
				parent.SetAttribute("Name", _attributeName);
			}

			if (_setterType != SetterType.Unknown)
			{
				parent.SetAttribute("SetterType", Enum.GetName(typeof(SetterType), _setterType));
			}

			if (_getterType != GetterType.SimpleAttributeGetter)
			{
				parent.SetAttribute("GetterType", Enum.GetName(typeof(GetterType), _getterType));
			}
		}

		/// <summary>
		/// Parse the row and column index which is part of the destination attribute
		/// name.
		/// </summary>
		private void GetIndex()
		{
			int pos = _attributeName.IndexOf("_");
			if (pos > 0)
			{
				// the index str is rowIndex_colIndex, for example 9_12
				string indexStr = _attributeName.Substring(pos + 1);
				pos = indexStr.IndexOf("_");
				if (pos > 0 && (pos + 1) < indexStr.Length)
				{
					_rowIndex = int.Parse(indexStr.Substring(0, pos));
					_colIndex = int.Parse(indexStr.Substring(pos + 1));
				}
			}
		}
	}
}