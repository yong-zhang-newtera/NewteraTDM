/*
* @(#)ResultAttributeListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	
	/// <summary>
	/// Represents a ListView item for a result attribute in a data view
	/// </summary>
	/// <version>  1.0.1 17 Nov 2003</version>
	/// <author>  Yong Zhang</author>
	public class ResultAttributeListViewItem : ListViewItem
	{
		private IDataViewElement _resultAttribute;

		/// <summary>
		/// Initializes a new instance of the ResultAttributeListViewItem class.
		/// </summary>
		/// <param name="resultAttribute">The result attribute that the item represents</param>
		public ResultAttributeListViewItem(string text, IDataViewElement resultAttribute) : base(text)
		{
			_resultAttribute = resultAttribute;
		}

		/// <summary> 
		/// Gets the attribute element.
		/// </summary>
		/// <value>A IDataViewElement</value>
		public IDataViewElement ResultAttribute
		{
			get
			{
				return _resultAttribute;
			}
		}
	}
}