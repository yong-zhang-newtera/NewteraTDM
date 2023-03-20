/*
* @(#)DstGridItemDot.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Data;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Mappings;
	using Newtera.Studio.UserControls.PropertyGridEx;

	/// <summary>
	/// A UI component representing a destination grid item. It can be connected to
	/// a destination grid item using a line.
	/// </summary>
	/// <version> 1.0.0 06 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class DstGridItemDot : MapComponentBase, IGridItemProxy
	{
		internal const int WIDTH = 9;
		internal const int HEIGHT = 9;

		private GridItem _gridItem;
		private int _dstY;
		private PropertyGridEx _propertyGrid;

		/// <summary>
		/// Initiate an instance of DstGridItemDot
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		/// <param name="srcGridItem">The Grid Item the dot represents</param>
		public DstGridItemDot(MapPanel panel, PropertyGridEx propertyGrid,
			GridItem gridItem) : base(panel)
		{
			_gridItem = gridItem;
			_propertyGrid = propertyGrid;

			YPoint = _propertyGrid.GetYCoordinate(gridItem);

			if (_propertyGrid.IsItemVisible(gridItem))
			{
				IsVisible = true;
			}
			else
			{
				IsVisible = false;
			}
		}

		/// <summary>
		/// Gets the name of destination attribute associated with the grid item
		/// </summary>
		/// <value>The name of destination attribute, null if grid item is not associated
		/// with a destination attribute.</value>
		public string AttributeName
		{
			get
			{
				string name = null;
				if (_gridItem.GridItemType == GridItemType.Property)
				{
					name = _gridItem.PropertyDescriptor.Name;
				}

				return name;
			}
		}

		/// <summary>
		/// Gets or sets the middle Y coordinate for destination grid item.
		/// </summary>
		public int YPoint
		{
			get
			{
				return _dstY;
			}
			set
			{
				_dstY = value;
			}
		}

		#region IGridItemProxy

		/// <summary>
		/// Gets or sets the grid item in destination property grid.
		/// </summary>
		public GridItem GridItem
		{
			get
			{
				return _gridItem;
			}
			set
			{
				_gridItem = value;
			}
		}

		#endregion IGridItemProxy

		#region IMapComponent

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>DestinationEnd</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.DestinationEnd;
			}
		}

		/// <summary>
		/// Adjust the position of the components in the map panel
		/// </summary>
		/// <remarks> Can be overrided by the subclasses</remarks>
		public override void AdjustPosition()
		{
			this.YPoint = _propertyGrid.GetYCoordinate(GridItem);
			this.IsVisible = _propertyGrid.IsItemVisible(GridItem);
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
		}

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// dot.
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public override bool Hits(int x, int y)
		{
			bool status = false;

			if (x >= XCoordinate && x < XCoordinate + DstGridItemDot.WIDTH &&
				y >= YCoordinate && y < (YCoordinate + DstGridItemDot.HEIGHT))
			{
				status = true;
			}

			return status;
		}

		/// <summary>
		/// Draw a dot on right-most side of the panel to represent a
		/// destination attribute.
		/// </summary>
		public override void Draw()
		{
			if (IsVisible)
			{
				using (Graphics g = this._panel.CreateGraphics())
				{
					if (!IsSelected)
					{
						// a regular rectangle
						g.FillRectangle(Brushes.DarkGray,
							XCoordinate, /*start x*/ YCoordinate, /*start Y*/
							DstGridItemDot.WIDTH, /* width */
							DstGridItemDot.HEIGHT /* height */);
					}
					else
					{
						// a highlighted rectangle
						g.FillRectangle(Brushes.Black,
							XCoordinate, /*start x*/ YCoordinate, /*start Y*/
							DstGridItemDot.WIDTH, /* width */
							DstGridItemDot.HEIGHT /* height */);
					}
				}
			}
		}

		/// <summary>
		/// Erase a dot on the panel
		/// </summary>
		public override void Erase()
		{
			if (IsVisible)
			{
				// TODO, only erase the dot that is visible completely or partially
				using (Graphics g = this._panel.CreateGraphics())
				{
					g.FillRectangle(Brushes.White,
						XCoordinate, /*start x*/ YCoordinate, /*start Y*/
						DstGridItemDot.WIDTH, /* width */
						DstGridItemDot.HEIGHT /* height */);
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the X coordinate of the rectangle
		/// </summary>
		private int XCoordinate
		{
			get
			{
				return _panel.DisplayRectangle.Width - DstGridItemDot.WIDTH;
			}
		}

		/// <summary>
		/// Gets the Y coordinate of the rectangle
		/// </summary>
		private int YCoordinate
		{
			get
			{
				return _dstY - DstGridItemDot.HEIGHT/2 + 1;
			}
		}
	}
}
