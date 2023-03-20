/*
* @(#) IExprPopup.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Drawing;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// A common interface for popups is search expression builder.
	/// </summary>
	/// <version> 	1.0.0 24 Nov 2003 </version>
	/// <author> Yong Zhang </author>
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
		DataViewModel DataView
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