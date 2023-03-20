/*
* @(#)IMapAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Xml;
	using System.Windows.Forms;

	/// <summary>
	/// Represents a common interface for actions performed in map panel.
	/// </summary>
	/// <version>1.0.0 11 Nov 2004</version>
	/// <author>Yong Zhang </author>
	public interface IMapAction
	{
		/// <summary>
		/// Gets the type of the action
		/// </summary>
		/// <value>One of MapActionTypeEnum</value>
		MapActionType ActionType
		{
			get;
		}

		/// <summary>
		/// X of mouse point
		/// </summary>
		int X
		{
			get; set;
		}

		/// <summary>
		/// Y of mouse point
		/// </summary>
		int Y
		{
			get; set;
		}

		/// <summary>
		/// Perform the action
		/// </summary>
		void Do();
	}
}