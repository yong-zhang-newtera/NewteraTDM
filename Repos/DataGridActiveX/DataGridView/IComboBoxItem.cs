/*
* @(#)IComboBoxItem.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents a common interface for the items used in InlineComboBox control.
	/// </summary>
	/// <version>  	1.0.0 25 Jul 2008</version>
	public interface IComboBoxItem
	{
        /// <summary>
        /// Gets or sets value of a combo box item
        /// </summary>
		string Value {get; set;}

        /// <summary>
        /// Gets or sets the display text of a combo box item
        /// </summary>
        string DisplayText { get; set;}
	}
}