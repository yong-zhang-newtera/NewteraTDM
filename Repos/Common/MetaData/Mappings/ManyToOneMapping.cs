/*
* @(#)ManyToOneMapping.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// The class represents a mapping between many source attributes and
	/// many destination attributes
	/// </summary>
	/// <version>1.0.0 15 Nov 2004</version>
	/// <author> Yong Zhang </author>
	public class ManyToOneMapping : MultiAttributeMappingBase
	{		
		/// <summary>
		/// Initiate an instance of ManyToOneMapping class.
		/// </summary>
		public ManyToOneMapping() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of ManyToOneMapping class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ManyToOneMapping(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.ManyToOneMapping;
			}
		}

		/// <summary>
		/// create an ManyToOneMapping from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// write ManyToOneMapping to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}
	}
}