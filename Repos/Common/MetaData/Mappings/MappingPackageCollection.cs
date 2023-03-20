/*
* @(#)MappingPackageCollection.cs
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
	/// Represents a collection of MappingPackage instances.
	/// </summary>
	/// <version>1.0.0 04 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class MappingPackageCollection : MappingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of MappingPackageCollection class
		/// </summary>
		public MappingPackageCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of MappingPackageCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal MappingPackageCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.MappingPackageCollection;
			}
		}
	}
}