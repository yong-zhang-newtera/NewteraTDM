/*
* @(#)XaclCondition.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// This class represents the condition of a rule
	/// </summary>
	/// <version>  	1.0.0 25 Nov 2003 </version>
	/// <author>  	Yong Zhang </author>
	public class XaclCondition : XaclNodeBase
	{		
		// the condition string
		private string _conditionStr;
		
		/// <summary>
		/// Initiate an instance of XaclCondition class
		/// </summary>
		public XaclCondition() : base()
		{
			_conditionStr = null;
		}

		/// <summary>
		/// Initiate an instance of XaclCondition class
		/// </summary>
		/// <param name="condition">   condition expression </param>
		public XaclCondition(string condition) : base()
		{
			_conditionStr = condition;
		}

		/// <summary>
		/// Initiating an instance of XaclCondition class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XaclCondition(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the condition string
		/// </summary>
		/// <value>  condition string</value>
		public string Condition
		{
			get
			{
				if (_conditionStr == null)
				{
					return "";
				}
				else
				{
					return _conditionStr;
				}
			}
			set
			{
				_conditionStr = value;
				FireValueChangedEvent(value);
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
				return NodeType.Condition;
			}
		}

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
            visitor.VisitXaclCondition(this);
        }

		/// <summary>
		/// create an XaclCondition from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_conditionStr = parent.InnerText;
		}

		/// <summary>
		/// write XaclCondition to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _conditionStr member
			if (_conditionStr != null && _conditionStr.Length > 0)
			{
				parent.InnerText = _conditionStr;
			}
		}
	}
}