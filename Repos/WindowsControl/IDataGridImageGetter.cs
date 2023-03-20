/*
* @(#) IDataGridImageGetter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Drawing;

	/// <summary>
	/// A interface for getting an image based on a cell value for DataGridImageColumn.
	/// </summary>
	/// <version> 	1.0.0 31 May 2007 </version>
	public interface IDataGridImageGetter
	{
		/// <summary>
		/// Gets an image for a cell val
		/// </summary>
        /// <param name="val">A cell value</param>
        /// <returns>An Image object for the cell value, can be null</returns>
		Image GetImage(object val);
	}
}