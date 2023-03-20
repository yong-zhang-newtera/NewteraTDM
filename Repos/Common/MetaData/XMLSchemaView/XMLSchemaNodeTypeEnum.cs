/*
* @(#)ElementTypeEnum.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	/// <summary>
	/// Specify the types of elements in xml schema view model.
	/// </summary>
	public enum XMLSchemaNodeType
	{
		/// <summary>
		/// Unknown element
		/// </summary>
		Unknown,
		/// <summary>
		/// XMLSchemaViewModel element
		/// </summary>
        XMLSchemaView,
        /// <summary>
        /// Collection
        /// </summary>
        Collection,
        /// <summary>
        /// XMLSchemaViewModel collection
        /// </summary>
        XMLSchemaViews,
        /// <summary>
        /// XMLSchemaComplexType
        /// </summary>
        XMLSchemaComplexType,
        /// <summary>
        /// XMLSchemaComplexTypes
        /// </summary>
        XMLSchemaComplexTypes,
        /// <summary>
        /// XMLSchemaElement
        /// </summary>
        XMLSchemaElement,
        /// <summary>
        /// XMLSchemaElements
        /// </summary>
        XMLSchemaElements
	}

    public enum MachineLearningCategory
    {
        None,
        Feature,
        Label
    }
}