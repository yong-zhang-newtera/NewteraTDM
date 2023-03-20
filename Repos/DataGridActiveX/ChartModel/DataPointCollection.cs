/*
* @(#)DataPointCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of ChartDef instances.
	/// </summary>
	/// <version>1.0.0 24 Apr 2006</version>
	public class DataPointCollection : ChartNodeCollection
	{
		/// <summary>
		/// Initiating an instance of DataPointCollection class
		/// </summary>
		public DataPointCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of DataPointCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataPointCollection(XmlElement xmlElement) : base(xmlElement)
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
				return NodeType.DataPointCollection;
			}
		}
	}
}