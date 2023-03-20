/*
* @(#)DefaultValue.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// The class represents a default value of a destination attribute.
	/// </summary>
	/// <version>1.0.0 28 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class DefaultValue : MappingNodeBase
	{
		private string _destinationAttribute;
		private string _value;
        private string _destinationClassName = null; // run-time value, do not persists
        private int _appliedRowIndex = -1; // run-time value, do not persisits
		
		/// <summary>
		/// Initiate an instance of DefaultValue class.
		/// </summary>
		public DefaultValue(string destinationAttribute, string val) : base()
		{
			_destinationAttribute = destinationAttribute;
			_value = val;
		}

		/// <summary>
		/// Initiating an instance of DefaultValue class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DefaultValue(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets name of the destination attribute associated with a DefaultValue
		/// </summary>
		public string DestinationAttributeName
		{
			get
			{
				return _destinationAttribute;
			}
			set
			{
				_destinationAttribute = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets default value of destination attribute.
		/// </summary>
		/// <value> The default value</value>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;

				FireValueChangedEvent(value);
			}
		}

        /// <summary>
        /// Gets name of the destination class associated with a DefaultValue
        /// </summary>
        /// <remarks>This property does not need to be serialized, for run-time use</remarks>
        public string DestinationClassName
        {
            get
            {
                return _destinationClassName;
            }
            set
            {
                _destinationClassName = value;
            }
        }

        /// <summary>
        /// Gets or set the index of a data row to which the default value applies
        /// </summary>
        /// <remarks>When < 0, the deafult value applied to all row, otherwise applied to the specified row</remarks>
        public int AppliedRowIndex
        {
            get
            {
                return _appliedRowIndex;
            }
            set
            {
                _appliedRowIndex = value;
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
				return NodeType.DefaultValue;
			}
		}

		/// <summary>
		/// create an DefaultValue from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Destination");
			if (str != null && str.Length > 0)
			{
				_destinationAttribute = str;
			}
			else
			{
				_destinationAttribute = null;
			}

			str = parent.GetAttribute("Value");
			if (str != null && str.Length > 0)
			{
				_value = str;
			}
			else
			{
				_value = null;
			}
		}

		/// <summary>
		/// write DefaultValue to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		
			// write the _destinationAttribute
			if (_destinationAttribute != null && _destinationAttribute.Length > 0)
			{
				parent.SetAttribute("Destination", _destinationAttribute);
			}

			if (_value != null && _value.Length > 0)
			{
				parent.SetAttribute("Value", _value);
			}
		}
	}
}