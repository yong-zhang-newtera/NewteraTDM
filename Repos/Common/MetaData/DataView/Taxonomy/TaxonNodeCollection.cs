/*
* @(#)TaxonNodeCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.Text;
	using System.Xml;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Represents a collection of Taxon nodes.
	/// </summary>
	/// <version>1.0.1 23 Feb 2004</version>
	/// <author>Yong Zhang</author>
	public class TaxonNodeCollection : DataViewElementCollection
	{
		/// <summary>
		/// Initiating an instance of TaxonNodeCollection class
		/// </summary>
		public TaxonNodeCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of TaxonNodeCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal TaxonNodeCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.TaxonNodes;
			}
		}
	}
}