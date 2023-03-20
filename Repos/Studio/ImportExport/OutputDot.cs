/*
* @(#)OutputDot.cs
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

	/// <summary>
	/// A UI component representing an output dot of a rectangle component. It can be connected to
	/// a destination grid item using a line.
	/// </summary>
	/// <version> 1.0.0 10 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class OutputDot : MapComponentBase
	{
		internal const int WIDTH = 9;
		internal const int HEIGHT = 9;

		private MapRect _rect;

		/// <summary>
		/// Initiate an instance of OutputDot
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		public OutputDot(MapPanel panel, MapRect rect) : base(panel)
		{
			_rect = rect;
		}

		/// <summary>
		/// Gets the X coordinate of the dot
		/// </summary>
		internal int XCoordinate
		{
			get
			{
				return _rect.X + MapRect.RectWidth;
			}
		}

		/// <summary>
		/// Gets the Y coordinate of the dot
		/// </summary>
		internal int YCoordinate
		{
			get
			{
				return _rect.Y + (MapRect.RectHeight - OutputDot.HEIGHT) / 2;
			}
		}

		/// <summary>
		/// Gets mapping type to which the output dot is associated with
		/// </summary>
		/// <value>One of ManyToMany, OneToMany, or ManyToOne</value>
		internal MapComponentType MappingType
		{
			get
			{
				return _rect.ComponentType;
			}
		}

		/// <summary>
		/// Gets a collection of output lines
		/// </summary>
		/// <value>A MapComponentCollection instance</value>
		internal MapComponentCollection OutputLines
		{
			get
			{
				return _rect.OutputLines;
			}
		}

		/// <summary>
		/// Add an output line to a rectangle
		/// </summary>
		/// <param name="line">IMapComponent</param>
		public void AddOutputLine(IMapComponent line)
		{
			_rect.AddOutputLine(line);
		}

		/// <summary>
		/// Remove an output line from a rectangle
		/// </summary>
		/// <param name="line">IMapComponent</param>
		public void RemoveOutputLine(IMapComponent line)
		{
			_rect.RemoveOutputLine(line);
		}

		#region IMapComponent

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>DestinationEnd</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.OutputEnd;
			}
		}

		/// <summary>
		/// Adjust the position of the components in the map panel
		/// </summary>
		/// <remarks> Can be overrided by the subclasses</remarks>
		public override void AdjustPosition()
		{
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
			if (IsTightlyCoupled)
			{
				_panel.Components.Remove(this);
			}
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

			if (x >= XCoordinate && x < XCoordinate + OutputDot.WIDTH &&
				y >= YCoordinate && y < (YCoordinate + OutputDot.HEIGHT))
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
							OutputDot.WIDTH, /* width */
							OutputDot.HEIGHT /* height */);
					}
					else
					{
						// a highlighted rectangle
						g.FillRectangle(Brushes.Black,
							XCoordinate, /*start x*/ YCoordinate, /*start Y*/
							OutputDot.WIDTH, /* width */
							OutputDot.HEIGHT /* height */);
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
						OutputDot.WIDTH, /* width */
						OutputDot.HEIGHT /* height */);
				}
			}
		}

		#endregion

	}
}
