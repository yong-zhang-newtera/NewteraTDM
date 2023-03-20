/*
* @(#)AttributeElementListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Windows.Forms;

	using Newtera.DataGridActiveX.DataGridView;
	
	/// <summary>
	/// Represents a ListView item for a schema model attribute element
	/// </summary>
	/// <version>  1.0.1 17 May 2006</version>
	public class AttributeElementListViewItem : ListViewItem
	{
		private ViewAttribute _attributeElement;

		/// <summary>
		/// Initializes a new instance of the AttributeElementListViewItem class.
		/// </summary>
		/// <param name="attributeElement">The schema model element that the item represents</param>
		public AttributeElementListViewItem(string text, ViewAttribute attributeElement) : base(text)
		{
			_attributeElement = attributeElement;
		}

		/// <summary> 
		/// Gets the attribute element.
		/// </summary>
		/// <value>A ViewAttribute</value>
		public ViewAttribute AttributeElement
		{
			get
			{
				return _attributeElement;
			}
		}
	}
}