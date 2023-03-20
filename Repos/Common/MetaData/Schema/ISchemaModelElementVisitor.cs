/*
* @(#)ISchemaModelElementVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in a schema model.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public interface ISchemaModelElementVisitor
	{
		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		void VisitClassElement(ClassElement element);

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		void VisitSimpleAttributeElement(SimpleAttributeElement element);

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		void VisitRelationshipAttributeElement(RelationshipAttributeElement element);

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">An ArrayAttributeElement instance</param>
		void VisitArrayAttributeElement(ArrayAttributeElement element);

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        void VisitVirtualAttributeElement(VirtualAttributeElement element);

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">An ImageAttributeElement instance</param>
        void VisitImageAttributeElement(ImageAttributeElement element);

        /// <summary>
        /// Viste a custom page element.
        /// </summary>
        /// <param name="element">An CustomPageElement instance</param>
        void VisitCustomPageElement(CustomPageElement element);

		/// <summary>
		/// Viste a schema info element.
		/// </summary>
		/// <param name="element">A SchemaInfoElement instance</param>
		void VisitSchemaInfoElement(SchemaInfoElement element);

		/// <summary>
		/// Viste an enum constraint element.
		/// </summary>
		/// <param name="element">A EnumElement instance</param>
		void VisitEnumElement(EnumElement element);

		/// <summary>
		/// Viste a range constraint element.
		/// </summary>
		/// <param name="element">A RangeElement instance</param>
		void VisitRangeElement(RangeElement element);

		/// <summary>
		/// Viste a pattern constraint element.
		/// </summary>
		/// <param name="element">A PatternElement instance</param>
		void VisitPatternElement(PatternElement element);

		/// <summary>
		/// Viste a list constraint element.
		/// </summary>
		/// <param name="element">A ListElement instance</param>
		void VisitListElement(ListElement element);
	}
}