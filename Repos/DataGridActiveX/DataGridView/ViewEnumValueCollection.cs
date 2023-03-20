/*
* @(#)ViewEnumValueCollection.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
    using System.Xml;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle ViewEnumValue when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 31 May 2007 </version>
    public class ViewEnumValueCollection : DataGridViewElementCollection
	{
		/// <summary>
		///  Initializes a new instance of the ViewEnumValueCollection class.
		/// </summary>
		public ViewEnumValueCollection() : base()
		{
		}

        /// <summary>
        /// Initiating an instance of ViewEnumValueCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal ViewEnumValueCollection(XmlElement xmlElement)
            : base(xmlElement)
		{
		}

        /// <summary>
        /// Gets the type of element
        /// </summary>
        /// <value>One of ViewElementType values</value>
        public override ViewElementType ElementType
        {
            get
            {
                return ViewElementType.EnumValues;
            }
        }

        /// <summary>
        /// Accept a visitor of IDataGridViewElementVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IDataGridViewElementVisitor visitor)
        {
            if (visitor.VisitEnumValues(this))
            {
                foreach (IDataGridViewElement element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}