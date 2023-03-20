/*
* @(#)AttributeElementListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Represents a ListView item for a schema model attribute element
	/// </summary>
	/// <version>  1.0.1 17 Nov 2003</version>
	/// <author>  Yong Zhang</author>
	public class AttributeElementListViewItem : ListViewItem
	{
		private AttributeElementBase _attributeElement;

		/// <summary>
		/// Initializes a new instance of the AttributeElementListViewItem class.
		/// </summary>
		/// <param name="attributeElement">The schema model element that the item represents</param>
		public AttributeElementListViewItem(string text, AttributeElementBase attributeElement) : base(text)
		{
			_attributeElement = attributeElement;
		}

		/// <summary> 
		/// Gets the attribute element.
		/// </summary>
		/// <value>A AttributeElementBase</value>
		public AttributeElementBase AttributeElement
		{
			get
			{
				return _attributeElement;
			}
		}
	}
}