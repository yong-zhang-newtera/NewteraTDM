/*
* @(#)CleanupSchemaVisitor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WebApi.Utils
{
	using System;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Traverse the schema model and cleanup the schema model to have a fresh start.
	/// </summary>
	/// <version> 1.0.0 11 Jan 2013 </version>
	public class CleanupSchemaVisitor : ISchemaModelElementVisitor
	{
		/// <summary>
		/// Instantiate an instance of CleanupSchemaVisitor class
		/// </summary>
		public CleanupSchemaVisitor()
		{
		}

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
			element.TableName = null;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
			element.ColumnName = null;
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
			element.ColumnName = null;
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
			element.ColumnName = null;
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        public void VisitVirtualAttributeElement(VirtualAttributeElement element)
        {
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A ImageAttributeElement instance</param>
        public void VisitImageAttributeElement(ImageAttributeElement element)
        {
            element.ColumnName = null;
        }

        /// <summary>
        /// Viste an custom page element.
        /// </summary>
        /// <param name="element">A CustomPageElement instance</param>
        public void VisitCustomPageElement(CustomPageElement element)
        {
        }

		/// <summary>
		/// Viste a schema info element.
		/// </summary>
		/// <param name="element">A SchemaInfoElement instance</param>
		public void VisitSchemaInfoElement(SchemaInfoElement element)
		{
		}

		/// <summary>
		/// Viste an enum constraint element.
		/// </summary>
		/// <param name="element">A EnumElement instance</param>
		public void VisitEnumElement(EnumElement element)
		{
		}

		/// <summary>
		/// Viste a range constraint element.
		/// </summary>
		/// <param name="element">A RangeElement instance</param>
		public void VisitRangeElement(RangeElement element)
		{
		}

		/// <summary>
		/// Viste a pattern constraint element.
		/// </summary>
		/// <param name="element">A PatternElement instance</param>
		public void VisitPatternElement(PatternElement element)
		{
		}

		/// <summary>
		/// Viste a list constraint element.
		/// </summary>
		/// <param name="element">A ListElement instance</param>
		public void VisitListElement(ListElement element)
		{
		}
	}
}