/*
* @(#)MoveRectAction.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	/// <summary>
	/// A action that move a rectangle in the panel
	/// </summary>
	/// <version> 1.0.0 10 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class MoveRectAction : MapActionBase
	{
		private MapRect _rect;

		/// <summary>
		/// Initiate an instance of MoveRectAction
		/// </summary>
		public MoveRectAction(MapPanel panel, MapRect rect) : base(panel)
		{
			_rect = rect;
		}


		/// <summary>
		/// Draw a dashed rectangle to show the current position
		/// </summary>
		public void Draw()
		{
			int x = X - (_rect.HitX - _rect.X);
			int y = Y - (_rect.HitY - _rect.Y);

			// do not go out of bound
			x = _panel.CheckXBound(x);
			y = _panel.CheckYBound(y);

			using (Graphics g = _panel.CreateGraphics())
			{
				using (Pen pen = new Pen(Color.Black))
				{
					pen.DashStyle = DashStyle.Dash;

					// a regular rectangle
					g.DrawRectangle(pen,
						x, /*start x*/ y, /*start Y*/
						MapRect.RectWidth, /* width */
						MapRect.RectHeight /* height */);
				}
			}
		}

		#region IMapComponent

		/// <summary>
		/// Gets the type of the action
		/// </summary>
		/// <value>One of MapActionTypeEnum</value>
		public override MapActionType ActionType
		{
			get
			{
				return MapActionType.Move;
			}
		}


		/// <summary>
		/// Perform the action
		/// </summary>
		public override void Do()
		{
			int x = X - (_rect.HitX - _rect.X);
			int y = Y - (_rect.HitY - _rect.Y);

			x = _panel.CheckXBound(x);
			y = _panel.CheckYBound(y);

			_rect.X = x;
			_rect.Y = y;

			// select the rectangle
			IMapComponent groupSelection = _rect.GetSelectionGroup();

			// adjust the lines that connect to the rectangle
			groupSelection.AdjustPosition();

			_panel.SelectComponent(groupSelection); // select the rect and its associates

		}

		#endregion
	}
}
