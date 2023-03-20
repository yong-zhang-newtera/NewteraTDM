/*
* @(#)FindSchemaModelElementByXPathVisitor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Validate
{
	using System;
	using System.Resources;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Traverse a schema model and find a schema model element of given xpath.
	/// </summary>
	/// <version> 1.0.0 20 Apr 2007 </version>
	public class FindSchemaModelElementByXPathVisitor : ISchemaModelElementVisitor
	{
		private string _xpath;
		private SchemaModelElement _schemaModelElement;

		/// <summary>
		/// Instantiate an instance of FindSchemaModelElementByXPathVisitor class
		/// </summary>
        /// <param name="xpath">The xpath.</param>
		public FindSchemaModelElementByXPathVisitor(string xpath)
		{
            _xpath = xpath;
            _schemaModelElement = null;
		}

		/// <summary>
		/// Gets a schema model element found.
		/// </summary>
		/// <value>The found element, null if not found</value>
        public SchemaModelElement SchemaModelElement
		{
			get
			{
                return _schemaModelElement;
			}
		}

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        public void VisitVirtualAttributeElement(VirtualAttributeElement element)
        {
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A ImageAttributeElement instance</param>
        public void VisitImageAttributeElement(ImageAttributeElement element)
        {
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
        }

        /// <summary>
        /// Viste an custom page element.
        /// </summary>
        /// <param name="element">A CustomPageElement instance</param>
        public void VisitCustomPageElement(CustomPageElement element)
        {
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
        }

		/// <summary>
		/// Viste a schema info element.
		/// </summary>
		/// <param name="element">A SchemaInfoElement instance</param>
		public void VisitSchemaInfoElement(SchemaInfoElement element)
		{
            if (element.ToXPath() == _xpath)
            {
                _schemaModelElement = element;
            }
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