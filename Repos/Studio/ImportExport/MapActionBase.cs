/*
* @(#)MapActionBase.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Windows.Forms;

	/// <summary>
	/// A base class for all actions performed in the map panel
	/// </summary>
	/// <version> 1.0.0 10 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public abstract class MapActionBase : IMapAction
	{
		private int _x;
		private int _y;
		private bool _isSelected;

		protected MapPanel _panel;

		/// <summary>
		/// Initiate an instance of MapActionBase
		/// </summary>
		public MapActionBase(MapPanel panel)
		{
			_panel = panel;
			_isSelected = true;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the mapping created by
		/// the action will be selected by default
		/// </summary>
		/// <value>true if it is selected, false otherwise.</value>
		internal bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				_isSelected = value;
			}
		}

		#region IMapComponent

		/// <summary>
		/// Gets the type of the action
		/// </summary>
		/// <value>One of MapActionTypeEnum</value>
		public abstract MapActionType ActionType
		{
			get;
		}

		/// <summary>
		/// X of mouse point
		/// </summary>
		public int X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		/// <summary>
		/// Y of mouse point
		/// </summary>
		public int Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		/// <summary>
		/// Perform the action
		/// </summary>
		public abstract void Do();

		#endregion
	}
}
