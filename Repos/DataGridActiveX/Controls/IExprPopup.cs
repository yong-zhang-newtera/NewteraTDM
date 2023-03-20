/*
* @(#) IExprPopup.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Drawing;

	using Newtera.DataGridActiveX.DataGridView;

	/// <summary>
	/// A common interface for popups is search expression builder.
	/// </summary>
	/// <version> 	1.0.0 24 May 2006 </version>
	public interface IExprPopup
	{
		event EventHandler Accept;

		/// <summary>
		/// Gets the expression that users pick from the popup
		/// </summary>
		/// <value>A object for the expression</value>
		object Expression
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the data view to which a search expression is built
		/// </summary>
		/// <value>A DataVieModel instance</value>
		DataGridViewModel DataGridView
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the
		/// popup relative to the window.
		/// </summary>
		Point Location
		{
			get;
			set;
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		void Show();
	}
}