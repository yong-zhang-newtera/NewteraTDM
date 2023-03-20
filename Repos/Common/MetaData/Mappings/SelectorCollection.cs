/*
* @(#)SelectorCollection.cs
*
* Copyright (c) 2003-2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of Selector instances.
	/// </summary>
	/// <version>1.0.0 09 Jan 2005</version>
	/// <author>Yong Zhang</author>
	public class SelectorCollection : MappingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of SelectorCollection class
		/// </summary>
		public SelectorCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of SelectorCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SelectorCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.SelectorCollection;
			}
		}
	}
}