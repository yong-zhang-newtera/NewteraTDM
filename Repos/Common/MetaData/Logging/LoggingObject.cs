/*
* @(#)LoggingObject.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;

	/// <summary>
	/// The class represents the object that may require logging info.
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
	public class LoggingObject : LoggingNodeBase
	{
		// the href attribute of the object element
		private string _href = null;
		
		/// <summary>
		/// Initiate an instance of LoggingObject
		/// </summary>
		/// <param name="href">The href of the object element.</param>
		public LoggingObject(string href) : base()
		{
			_href = href;
		}

		/// <summary>
		/// Initiating an instance of LoggingObject class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingObject(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the href attribute of the LoggingObject.
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
        /// Gets the information indicating whether the LoggingObject represent a class attribute
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
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
            visitor.VisitLoggingObject(this);
        }

		/// <summary>
		/// create an LoggingObject from a xml document.
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