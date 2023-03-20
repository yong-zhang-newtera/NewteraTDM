/*
* @(#)AttributeMappingCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of AttributeMapping instances.
	/// </summary>
	/// <version>1.0.0 02 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class AttributeMappingCollection : MappingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of AttributeMappingCollection class
		/// </summary>
		public AttributeMappingCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of AttributeMappingCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AttributeMappingCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.AttributeMappingCollection;
			}
		}
	}
}