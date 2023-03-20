/*
* @(#)InputLine.cs
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
	/// Representing a line showing a mapping between source and destination
	/// attributes.
	/// </summary>
	/// <version> 1.0.0 21 Sep 2004 </version>
	/// <author> Yong Zhang</author>
	public class InputLine : MapComponentBase
	{
		private SrcGridItemDot _srcDot;
		private InputDot _inputDot;
		private int _srcY;
		private int _dstY;

		/// <summary>
		/// Initiate an instance of InputLine
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		/// <param name="srcDot">The name of source attribute.</param>
		/// <param name="inputDot">The name of destination attribute.</param>
		public InputLine(MapPanel panel, SrcGridItemDot srcDot, InputDot inputDot) : base(panel)
		{
			_srcDot = srcDot;
			_inputDot = inputDot;

			_srcY = _srcDot.YPoint;
			_dstY = _inputDot.YCoordinate + (InputDot.HEIGHT / 2); // middle of InputDot height
		}

		/// <summary>
		/// Gets name of the source attribute.
		/// </summary>
		public string SourceAttributeName
		{
			get
			{
				return _srcDot.AttributeName;
			}
		}

		/// <summary>
		/// Gets the InputDot instance associated with the input line
		/// </summary>
		/// <value>OutputDot instance</value>
		public InputDot InputDot
		{
			get
			{
				return _inputDot;
			}
		}

		/// <summary>
		/// Gets the SrcGridItemDot instance associated with the intput line
		/// </summary>
		/// <value>SrcGridItemDot instance</value>
		public SrcGridItemDot SrcDot
		{
			get
			{
				return _srcDot;
			}
		}

		#region IMapComponent

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>OneToOne</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.InputLine;
			}
		}

		/// <summary>
		///  Gets the information indicating whether the mapping represented by
		///  this component has existed.
		/// </summary>
		/// <returns>True if it existed, false otherwise.</returns>
		public override bool IsMappingExist()
		{
			bool status = false;

			foreach (IMapComponent component in _panel.Components)
			{
				if (component is InputLine)
				{
					InputLine inputLine = (InputLine) component;
					if (inputLine.SrcDot == this.SrcDot &&
						inputLine.InputDot == this.InputDot)
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

		/// <summary>
		/// Create a IMappingNode instance represent this UI map component
		/// </summary>
		public override void CreateMapping()
		{
			_panel.Components.Add(this);

			// add the line to the rectangle
			_inputDot.AddInputLine(this);

			IMapComponent selectionGroup = GetSelectionGroup();

			_panel.SelectComponent(selectionGroup); // select the newly added line
		}

		/// <summary>
		/// Get the group of components that are supposed to be selected together
		/// </summary>
		/// <returns>IMapComponent</returns>
		public override IMapComponent GetSelectionGroup()
		{
			MapComposite composite = new MapComposite(this._panel, this);

			composite.Components.Add(this);

			// add the source and input dots connected by the line
			AddConnectedDots(composite);

			return composite;
		}

		/// <summary>
		/// Adjust the position of the line in the map panel
		/// </summary>
		public override void AdjustPosition()
		{
			_srcY = _srcDot.YPoint;
			_dstY = _inputDot.YCoordinate + (InputDot.HEIGHT / 2); // middle of InputDot height
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
			_panel.Components.Remove(this);

			_inputDot.RemoveInputLine(this);
		}

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// line
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public override bool Hits(int x, int y)
		{
			float ratio = ((_dstY - _srcY) * 1.0f) / (_inputDot.XCoordinate - SrcGridItemDot.WIDTH);
			float fResult = ratio * (x - SrcGridItemDot.WIDTH) + _srcY;
			float delta;
			if (y > fResult)
			{
				delta = y - fResult;
			}
			else
			{
				delta = fResult - y;
			}

			// allow an offset
			if (delta < 4.0f)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Draw a line on the panel to show the mapping
		/// </summary>
		public override void Draw()
		{
			if (IsVisible)
			{
				// TODO, only draw the lines that are visible completely or partially
				using (Graphics g = this._panel.CreateGraphics())
				{
					if (!IsSelected)
					{
						// regular line
						g.DrawLine(Pens.DarkGray, SrcGridItemDot.WIDTH, /*start x*/ _srcY, /*start Y*/
							_inputDot.XCoordinate, /* end X */
							_dstY /* end Y */);
					}
					else
					{
						// highlighted line
						g.DrawLine(Pens.Black, SrcGridItemDot.WIDTH, /*start x*/ _srcY, /*start Y*/
							_inputDot.XCoordinate, /* end X */
							_dstY /* end Y */);
					}
				}
			}
		}

		/// <summary>
		/// Erase a line on the panel
		/// </summary>
		public override void Erase()
		{
			if (IsVisible)
			{
				// TODO, only erase the lines that are visible completely or partially
				using (Graphics g = this._panel.CreateGraphics())
				{
					// erase the line
					// highlighted line
					g.DrawLine(Pens.White, SrcGridItemDot.WIDTH, /*start x*/ _srcY, /*start Y*/
						_inputDot.XCoordinate, /* end X */
						_dstY /* end Y */);
				}
			}
		}

		#endregion

		/// <summary>
		/// Add the connected dots to the composite component
		/// </summary>
		/// <param name="composite">The composite</param>
		/// <param name="mapLine">The line that connects the dots.</param>
		private void AddConnectedDots(MapComposite composite)
		{
			composite.Components.Add(_srcDot);

			_inputDot.IsTightlyCoupled = false;
			composite.Components.Add(_inputDot);
		}
	}
}
