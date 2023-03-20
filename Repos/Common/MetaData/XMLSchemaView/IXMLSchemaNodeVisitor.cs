/*
* @(#)IXMLSchemaNodeVisitor.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in a xml schema view model.
	/// </summary>
    /// <version>  	1.0.0 10 Aug 2014</version>
	public interface IXMLSchemaNodeVisitor
	{
		/// <summary>
		/// Viste a xml schema model.
		/// </summary>
		/// <param name="element">A XMLSchemaModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitXMLSchemaModel(XMLSchemaModel element);

        /// <summary>
        /// Viste a xml complex type.
        /// </summary>
        /// <param name="element">A XMLSchemaComplexType instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitXMLSchemaComplexType(XMLSchemaComplexType element);

        /// <summary>
        /// Viste a xml element.
        /// </summary>
        /// <param name="element">A XMLSchemaElement instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitXMLSchemaElement(XMLSchemaElement element);
	}
}