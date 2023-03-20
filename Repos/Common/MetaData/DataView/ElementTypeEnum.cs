/*
* @(#)ElementTypeEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	/// <summary>
	/// Specify the types of elements in data view model.
	/// </summary>
	public enum ElementType
	{
		/// <summary>
		/// Unknown element
		/// </summary>
		Unknown,
		/// <summary>
		/// View element
		/// </summary>
		View,
		/// <summary>
		/// Class element
		/// </summary>
		Class,
		/// <summary>
		/// Filter element
		/// </summary>
		Filter,
		/// <summary>
		/// Simple attribute element
		/// </summary>
		SimpleAttribute,
		/// <summary>
		/// Array element
		/// </summary>
		ArrayAttribute,
        /// <summary>
        /// Virtual element
        /// </summary>
        VirtualAttribute,
        /// <summary>
        /// Image element
        /// </summary>
        ImageAttribute,
		/// <summary>
		/// Relationship element
		/// </summary>
		RelationshipAttribute,
		/// <summary>
		/// And element
		/// </summary>
		And,
		/// <summary>
		/// Or element
		/// </summary>
		Or,
		/// <summary>
		/// Equals element
		/// </summary>
		Equals,
		/// <summary>
		/// Not Equals element
		/// </summary>
		NotEquals,
        /// <summary>
        /// IsNull element
        /// </summary>
        IsNull,
        /// <summary>
        /// IsNotNull element
        /// </summary>
        IsNotNull,
		/// <summary>
		/// To element
		/// </summary>
		To,
		/// <summary>
		/// Less Than element
		/// </summary>
		LessThan,
		/// <summary>
		/// Greater Than element
		/// </summary>
		GreaterThan,
		/// <summary>
		/// Less Than Equals element
		/// </summary>
		LessThanEquals,
		/// <summary>
		/// Greater Than Equals element
		/// </summary>
		GreaterThanEquals,
        /// <summary>
        /// Like element
        /// </summary>
        Like,
		/// <summary>
		/// In element
		/// </summary>
		In,
		/// <summary>
		/// Not In element
		/// </summary>
		NotIn,
		/// <summary>
		/// Parameter element
		/// </summary>
		Parameter,
		/// <summary>
		/// Parenthesized Expr element
		/// </summary>
		ParenthesizedExpr,
		/// <summary>
		/// Left Parenthesis
		/// </summary>
		LeftParenthesis,
		/// <summary>
		/// Right Parenthesis
		/// </summary>
		RightParenthesis,
		/// <summary>
		/// Left Empty Operand
		/// </summary>
		LeftEmptyOperand,
		/// <summary>
		/// Right Empty Operand
		/// </summary>
		RightEmptyOperand,
		/// <summary>
		/// Relationship Begin
		/// </summary>
		RelationshipBegin,
		/// <summary>
		/// Relationship End
		/// </summary>
		RelationshipEnd,
		/// <summary>
		/// Collection
		/// </summary>
		Collection,
		/// <summary>
		/// Result Attributes
		/// </summary>
		ResultAttributes,
		/// <summary>
		/// Referenced Classes
		/// </summary>
		ReferencedClasses,
		/// <summary>
		/// Parameters
		/// </summary>
		Parameters,
		/// <summary>
		/// Contains
		/// </summary>
		Contains,
		/// <summary>
		/// DataViews
		/// </summary>
		DataViews,
		/// <summary>
		/// Comma
		/// </summary>
		Comma,
		/// <summary>
		/// SortBy
		/// </summary>
		SortBy,
        /// <summary>
        /// SortAttribute
        /// </summary>
        SortAttribute,
		/// <summary>
		/// Taxonomy
		/// </summary>
		Taxonomy,
		/// <summary>
		/// Taxonomies
		/// </summary>
		Taxonomies,
		/// <summary>
		/// TaxonNode
		/// </summary>
		TaxonNode,
		/// <summary>
		/// TaxonNodes
		/// </summary>
		TaxonNodes,
        /// <summary>
        /// AutoClassifyDef
        /// </summary>
        AutoClassifyDef,
        /// <summary>
        /// AutoClassifyLevel
        /// </summary>
        AutoClassifyLevel,
        /// <summary>
        /// AutoClassifyLevels
        /// </summary>
        AutoClassifyLevels,
        /// <summary>
        /// WFState
        /// </summary>
        WFState,
        /// <summary>
        /// Before
        /// </summary>
        Before,
        /// <summary>
        /// Error
        /// </summary>
        Error
	}
}