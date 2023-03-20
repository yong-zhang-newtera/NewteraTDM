/*
* @(#)ITaxonomy.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a common interface for the nodes in Taxonomy name space.
	/// </summary>
	/// <version>  	1.0.0 14 Feb 2004</version>
	/// <author> Yong Zhang </author>
	public interface ITaxonomy : IXaclObject
	{
		/// <summary>
		/// Gets the meta data model that owns the ITaxonomy object
		/// </summary>
		MetaDataModel MetaDataModel { get; }

		/// <summary>
		/// Gets or sets the class name for this node
		/// </summary>
		string ClassName { get; set;}

		/// <summary>
		/// Gets or sets the data view name for this node
		/// </summary>
		string DataViewName { get; set;}

		/// <summary>
		/// Gets or sets the parent node of this node
		/// </summary>
		/// <value>A IDataViewElement object.</value>
		ITaxonomy ParentNode {get; set;}

		/// <summary>
		/// Gets the children nodes of this node
		/// </summary>
		/// <value>A TaxonNodeCollection</value>
		TaxonNodeCollection ChildrenNodes {get;}

        /// <summary>
        /// Gets or sets the definition for auto-generated hierarchy.
        /// </summary>
        /// <value>A AutoClassifyDef object</value>
        AutoClassifyDef AutoClassifyDef { get; set;}

		/// <summary>
		/// Gets the DataViewModel for the ITaxonomy object
		/// </summary>
		/// <param name="sectionString">Specify the sections whose attributes are included
		/// in the result list of the generated data view, or null to include all attributes.</param>		
		DataViewModel GetDataView(string sectionString);
	}
}