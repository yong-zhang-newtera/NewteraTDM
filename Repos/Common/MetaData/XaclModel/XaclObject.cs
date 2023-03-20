/*
* @(#)XaclObject.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;

	/// <summary>
	/// The class represents the object in a Xacl definition.
	/// </summary>
	/// <version> 1.0.0 11 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class XaclObject : XaclNodeBase
	{
		// the href attribute of the object element
		private string _href = null;
		
		/// <summary>
		/// Initiate an instance of XaclObject
		/// </summary>
		/// <param name="href">The href of the object element.</param>
		public XaclObject(string href) : base()
		{
			_href = href;
		}

		/// <summary>
		/// Initiating an instance of XaclObject class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal XaclObject(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the href attribute of the XaclObject.
		/// </summary>
		/// <returns> the href attribute.</returns>
		public string Href
		{
			get
			{
				return _href;
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
				return NodeType.Object;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the XaclObject represent a class attribute
        /// </summary>
        public bool IsClassAttribute
        {
            get
            {
                bool status = false;

                if (!string.IsNullOrEmpty(_href) &&
                    _href.EndsWith(NewteraNameSpace.ATTRIBUTE_SUFFIX))
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Accept a visitor of IXaclNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IXaclNodeVisitor visitor)
        {
            visitor.VisitXaclObject(this);
        }

		/// <summary>
		/// create an XaclObject from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_href = parent.GetAttribute("href");
		}

		/// <summary>
		/// write policy to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _href member
			parent.SetAttribute("href", _href);
		}
	}
}