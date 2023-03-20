/*
* @(#)ConditionalFunctionComboBoxItem.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData;
	
	/// <summary>
	/// Represents a ComboBox item for a function used in an condition expression
	/// </summary>
	/// <version>  1.0.0 13 Oct 2007</version>
	public class ConditionalFunctionComboBoxItem
	{
		/// <summary>
		/// Initializes a new instance of the ConditionalFunctionComboBoxItem class.
		/// </summary>
		/// <param name="referencedClass">A data class that the item represents</param>
		public ConditionalFunctionComboBoxItem()
		{
		}

		public override string ToString()
		{
			return MessageResourceManager.GetString("DesignStudio.Functions");
		}
	}
}