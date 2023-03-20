/*
* @(#)ClassMappingCollection.cs
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
	/// Represents a collection of ClassMapping instances.
	/// </summary>
	/// <version>1.0.0 03 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class ClassMappingCollection : MappingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of ClassMappingCollection class
		/// </summary>
		public ClassMappingCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ClassMappingCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ClassMappingCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.ClassMappingCollection;
			}
		}
	}
}