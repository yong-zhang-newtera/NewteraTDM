/*
* @(#)MapComponentBase.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Studio.ImportExport
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using System.Runtime.InteropServices;

	using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// A base class for all UI components in the map panel
	/// </summary>
	/// <version> 1.0.0 05 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public abstract class MapComponentBase : IMapComponent
	{
		private bool _isSelected;
		private bool _isVisible;
		private int _hitX;
		private int _hitY;
		bool _isTightlyCoupled;

		internal MapPanel _panel;

		/// <summary>
		/// Initiate an instance of MapComponentBase
		/// </summary>
		/// <param name="panel">The panel on which to draw a component</param>
		public MapComponentBase(MapPanel panel)
		{
			_panel = panel;
			_isSelected = false;
			_isVisible = true;
			_isTightlyCoupled = true;
		}

		#region IMapComponent

		/// <summary>
		/// Gets or sets the IMappingNode associated with the mapping UI componnet
		/// </summary>
		/// <value>The associated IMappingNode instance, it could be null</value>
		public virtual IMappingNode MappingNode
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the component is
		/// selected.
		/// </summary>
		/// <value>True if it selected, false otherwise.</value>
		public virtual bool IsSelected
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

		/// <summary>
		/// Gets or sets the information indicating whether the component is
		/// visible or not on the panel
		/// </summary>
		/// <value>True if it visible, false otherwise.</value>
		public virtual bool IsVisible
		{
			get
			{
				return _isVisible;
			}
			set
			{
				_isVisible = value;
			}
		}

		/// <summary>
		/// Gets or sets the x where the component is hit by mouse
		/// </summary>
		public int HitX
		{
			get
			{
				return _hitX;
			}
			set
			{
				_hitX = value;
			}
		}

		/// <summary>
		/// Gets or sets the y where the component is hit by mouse
		/// </summary>
		public int HitY
		{
			get
			{
				return _hitY;
			}
			set
			{
				_hitY = value;
			}
		}

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>One of MapComponentTypeEnum</value>
		public abstract MapComponentType ComponentType
		{
			get;
		}

		/// <summary>
		/// Gets the principal component when it represents a group of
		/// components.
		/// </summary>
		public virtual IMapComponent Principal
		{
			get
			{
				return this; // itself
			}
		}
		
		/// <summary>
		/// Get the group of components that are supposed to be selected together
		/// </summary>
		/// <returns>IMapComponent</returns>
		/// <remarks>By default, the component itself is selected. It can be
		/// overrided by the subclasses to return a MapComposite component.</remarks>
		public virtual IMapComponent GetSelectionGroup()
		{
			return this;
		}

		/// <summary>
		/// Gets the information indicate whether the mapping represented by
		/// the IMapComponent is valid or not.
		/// </summary>
		/// <value>True if it is valid, false otherwise.</value>
		public virtual bool IsValid
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Indicate whether the component is tightly coupled in a selection group.
		/// If it is tightly coupled, it is deleted when the group is deleted.
		/// </summary>
		/// <value>True if it is tightly coupled, false otherwise.By default, true</value>
		public bool IsTightlyCoupled
		{
			get
			{
				return _isTightlyCoupled;
			}
			set
			{
				_isTightlyCoupled = value;
			}
		}

		/// <summary>
		///  Gets the information indicating whether the mapping represented by
		///  this component has existed.
		/// </summary>
		/// <returns>True if it existed, false otherwise.</returns>
		public virtual bool IsMappingExist()
		{
			return false;
		}

		/// <summary>
		/// Create a IMappingNode instance represent this UI map component
		/// </summary>
		public virtual void CreateMapping()
		{
		}

		/// <summary>
		/// Show the mapping(s) on the map panel
		/// </summary>
		public virtual void ShowMapping()
		{
		}

		/// <summary>
		/// Adjust the position of the components in the map panel
		/// </summary>
		/// <remarks> Can be overrided by the subclasses</remarks>
		public virtual void AdjustPosition()
		{
		}

		/// <summary>
		/// Select the line.
		/// </summary>
		public virtual void Select()
		{
			_isSelected = true;
		}

		/// <summary>
		/// Deselect the line.
		/// </summary>
		public virtual void Deselect()
		{
			_isSelected = false;
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public abstract void Delete();

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// component
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public abstract bool Hits(int x, int y);

		/// <summary>
		/// Draw the component
		/// </summary>
		public abstract void Draw();

		/// <summary>
		/// Erase the component
		/// </summary>
		public abstract void Erase();

		#endregion
	}
}
