/*
* @(#)NodeType.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WinClientCommon
{
	/// <summary>
	/// Specify the types of tree nodes
	/// </summary>
	public enum NodeType
	{
		Unknown = 0,
		ClassNode,
		SimpleAttributeNode,
		RelationshipAttributeNode,
		ArrayAttributeNode,
        VirtualAttributeNode,
        ImageAttributeNode,
		EnumConstraintNode,
		RangeConstraintNode,
		PatternConstraintNode,
		ListConstraintNode,
		SchemaInfoNode,
		ClassesFolder,
		ConstraintsFolder,
		DataViewsFolder,
		DataViewNode,
		TaxonomiesFolder,
		TaxonomyNode,
		TaxonNode,
		SelectorsFolder,
		SelectorNode,
        CategoryFolder,
        XMLSchemaViewsFolder,
        XMLSchemaView,
        XMLSchemaComplexType,
        XMLSchemaElement
	}
}