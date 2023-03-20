/*
* @(#)AutoClassifyLevelCollection.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.Text;
	using System.Xml;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
    /// Represents a collection of AutoHierarchyLevel objects.
	/// </summary>
	/// <version>1.0.1 12 June 2008</version>
	public class AutoClassifyLevelCollection : DataViewElementCollection
	{
		/// <summary>
		/// Initiating an instance of AutoClassifyLevelCollection class
		/// </summary>
		public AutoClassifyLevelCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of AutoClassifyLevelCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AutoClassifyLevelCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.AutoClassifyLevels;
			}
		}
	}
}