/*
* @(#)FunctionListViewItem.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.DataView;
	
	/// <summary>
	/// Represents a ListView item for a function
	/// </summary>
	/// <version>  1.0.0 14 Oct 2007</version>
	public class FunctionListViewItem : ListViewItem
	{
        private IFunctionElement _functionElement;

		/// <summary>
		/// Initializes a new instance of the FunctionListViewItem class.
		/// </summary>
		public FunctionListViewItem(string text, IFunctionElement functionElement) : base(text)
		{
            _functionElement = functionElement;
		}

        public IFunctionElement FunctionElement
        {
            get
            {
                return _functionElement;
            }
        }
	}
}