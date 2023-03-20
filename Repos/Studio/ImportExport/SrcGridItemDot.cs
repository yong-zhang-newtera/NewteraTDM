/*
* @(#)SrcGridItemDot.cs
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

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings;
	using Newtera.Studio.UserControls.PropertyGridEx;

	/// <summary>
	/// A UI component representing a source grid item. It can be connected to
	/// a destination grid item using a line.
	/// </summary>
	/// <version> 1.0.0 06 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class SrcGridItemDot : MapComponentBase, IGridItemProxy
	{
		internal const int WIDTH = 9;
		internal const int HEIGHT = 9;

		private GridItem _gridItem;
		private int _srcY;
		private PropertyGridEx _propertyGrid;

		/// <summary>
		/// Initiate an instance of SrcGridItemDot
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		/// <param name="propertyGrid">The property grid</param>
		/// <param name="gridItem">The Grid Item the dot represents</param>
		public SrcGridItemDot(MapPanel panel, PropertyGridEx propertyGrid,
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
		/// Gets the name of source attribute associated with the grid item
		/// </summary>
		/// <value>The name of source attribute, null if grid item is not associated
		/// with a source attribute.</value>
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
		/// Gets or sets the Y coordinate for source grid item.
		/// </summary>
		public int YPoint
		{
			get
			{
				return _srcY;
			}
			set
			{
				_srcY = value;
			}
		}

		#region IGridItemProxy

		/// <summary>
		/// Gets or sets the grid item in source property grid.
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
		/// <value>SourceEnd</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.SourceEnd;
			}
		}

		/// <summary>
		/// Gets the information indicate whether the selection represented by
		/// the IMapComponent is valid or not.
		/// </summary>
		/// <value>True if it is valid, false otherwise.</value>
		public override bool IsValid
		{
			get
			{
				bool status = true;

				if (GridItem.PropertyDescriptor is InstanceAttributePropertyDescriptor)
				{
					InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor) GridItem.PropertyDescriptor;
					if (pd.IsArray || pd.IsRelationship)
					{
						// select on relationship or array attributes are not allowed
						status = false;
					}
				}
				else if (GridItem.PropertyDescriptor is ArrayDataRowPropertyDescriptor)
				{
					status = false;
				}

				return status;
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

			if (x >= 0 && x < SrcGridItemDot.WIDTH &&
				y >= YCoordinate && y < (YCoordinate + SrcGridItemDot.HEIGHT))
			{
				status = true;
			}

			return status;
		}

		/// <summary>
		/// Draw a dot on left-most side of the panel to represent a
		/// source attribute.
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
							0, /*start x*/ YCoordinate, /*start Y*/
							SrcGridItemDot.WIDTH, /* width */
							SrcGridItemDot.HEIGHT /* height */);
					}
					else
					{
						// a highlighted rectangle
						g.FillRectangle(Brushes.Black,
							0, /*start x*/ YCoordinate, /*start Y*/
							SrcGridItemDot.WIDTH, /* width */
							SrcGridItemDot.HEIGHT /* height */);
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
					// a regular rectangle
					g.FillRectangle(Brushes.White,
						0, /*start x*/ YCoordinate, /*start Y*/
						SrcGridItemDot.WIDTH, /* width */
						SrcGridItemDot.HEIGHT /* height */);
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the Y coordinate of the rectangle
		/// </summary>
		private int YCoordinate
		{
			get
			{
				return _srcY - SrcGridItemDot.HEIGHT/2 + 1;
			}
		}
	}
}
