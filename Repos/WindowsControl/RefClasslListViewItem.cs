/*
* @(#)RefClassListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	
	/// <summary>
	/// Represents a ListView item for a Linked class
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class RefClassListViewItem : ListViewItem
	{
		private DataClass _referencedClass;

		/// <summary>
		/// Initializes a new instance of the RefClassListViewItem class.
		/// </summary>
		/// <param name="referencedClass">The referenced class that the item represents</param>
		public RefClassListViewItem(string text, DataClass referencedClass) : base(text)
		{
			_referencedClass = referencedClass;
		}

		/// <summary> 
		/// Gets the referenced class.
		/// </summary>
		/// <value>A DataClass</value>
		public DataClass ReferencedClass
		{
			get
			{
				return _referencedClass;
			}
		}
	}
}