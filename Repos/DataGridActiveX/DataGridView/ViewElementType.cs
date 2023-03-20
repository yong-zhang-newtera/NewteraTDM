/*
* @(#)ElementTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	/// <summary>
	/// Specify the types of elements in class view model.
	/// </summary>
	public enum ViewElementType
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
		/// ViewFilter element
		/// </summary>
		Filter,
		/// <summary>
		/// Simple attribute element
		/// </summary>
		SimpleAttribute,
        /// <summary>
        /// Array attribute element
        /// </summary>
        ArrayAttribute,
        /// <summary>
        /// Virtual attribute element
        /// </summary>
        VirtualAttribute,
        /// <summary>
        /// Relationship attribute element
        /// </summary>
        RelationshipAttribute,
        /// <summary>
        /// Enum constraint
        /// </summary>
        EnumElement,
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
        /// IsNull element
        /// </summary>
        IsNull,
        /// <summary>
        /// IsNotNull element
        /// </summary>
        IsNotNull,
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
		/// Collection
		/// </summary>
		Collection,
		/// <summary>
		/// Result Attributes
		/// </summary>
		ResultAttributes,
		/// <summary>
		/// Parameters
		/// </summary>
		Parameters,
		/// <summary>
		/// Contains
		/// </summary>
		Contains,
		/// <summary>
		/// ViewComma
		/// </summary>
		Comma,
		/// <summary>
		/// SortBy
		/// </summary>
		SortBy,
        /// <summary>
        /// EnumValues
        /// </summary>
        EnumValues,
        /// <summary>
        /// EnumValue
        /// </summary>
        EnumValue,
        /// <summary>
        /// LocaleInfo
        /// </summary>
        LocaleInfo
	}
}