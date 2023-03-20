/*
* @(#)OutputLine.cs
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
	/// Representing a line showing a mapping between source and destination
	/// attributes.
	/// </summary>
	/// <version> 1.0.0 21 Sep 2004 </version>
	/// <author> Yong Zhang</author>
	public class OutputLine : MapComponentBase
	{
		private DstGridItemDot _dstDot;
		private OutputDot _outputDot;
		private int _srcX;
		private int _srcY;
		private int _dstY;

		/// <summary>
		/// Initiate an instance of OutputLine
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		/// <param name="srcDot">An OutputDot instance</param>
		/// <param name="dstDot">An DstGridItemDot instance</param>
		public OutputLine(MapPanel panel, OutputDot srcDot, DstGridItemDot dstDot) : base(panel)
		{
			_outputDot = srcDot;
			_dstDot = dstDot;

			_srcX = _outputDot.XCoordinate + OutputDot.WIDTH;
			_srcY = _outputDot.YCoordinate + (OutputDot.HEIGHT / 2); // middle of OutputDot height
			_dstY = _dstDot.YPoint;
		}

		/// <summary>
		/// Gets the OutputDot instance associated with the output line
		/// </summary>
		/// <value>OutputDot instance</value>
		public OutputDot OutputDot
		{
			get
			{
				return _outputDot;
			}
		}

		/// <summary>
		/// Gets the DstGridItemDot instance associated with the output line
		/// </summary>
		/// <value>DstGridItemDot instance</value>
		public DstGridItemDot DstDot
		{
			get
			{
				return _dstDot;
			}
		}

		/// <summary>
		/// Gets name of the source attribute.
		/// </summary>
		public string DestinationAttributeName
		{
			get
			{
				return _dstDot.AttributeName;
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
				return MapComponentType.OutputLine;
			}
		}

		/// <summary>
		/// Gets the information indicate whether the mapping represented by
		/// the IMapComponent is valid or not.
		/// </summary>
		/// <value>True if it is valid, false otherwise.</value>
		public override bool IsValid
		{
			get
			{
				bool status = false;

				InstanceAttributePropertyDescriptor ipd = _dstDot.GridItem.PropertyDescriptor as InstanceAttributePropertyDescriptor;
				ArrayDataCellPropertyDescriptor acpd = _dstDot.GridItem.PropertyDescriptor as ArrayDataCellPropertyDescriptor;

				if (_dstDot.GridItem.GridItemType == GridItemType.Property)
				{
					if (ipd != null && !ipd.IsRelationship)
					{
						status = true;
					}
					else if (acpd != null)
					{
						status = true;
					}
				}

				return status;
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
				if (component is OutputLine)
				{
					OutputLine outputLine = (OutputLine) component;
					if (outputLine.OutputDot == this.OutputDot &&
						outputLine.DstDot == this.DstDot)
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
			_outputDot.AddOutputLine(this);

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

			// add the output and destination dots connected by the line
			AddConnectedDots(composite);

			return composite;
		}

		/// <summary>
		/// Adjust the position of the line in the map panel
		/// </summary>
		public override void AdjustPosition()
		{
			_srcX = _outputDot.XCoordinate + OutputDot.WIDTH;
			_srcY = _outputDot.YCoordinate + (OutputDot.HEIGHT / 2); // middle of OutputDot height
			_dstY = _dstDot.YPoint;
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
			_panel.Components.Remove(this);

			_outputDot.RemoveOutputLine(this);
		}

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// line
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public override bool Hits(int x, int y)
		{
			float ratio = ((_dstY - _srcY) * 1.0f) / (_panel.ClientRectangle.Width - DstGridItemDot.WIDTH - _srcX);
			float fResult = ratio * (x - _srcX) + _srcY;
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
						g.DrawLine(Pens.DarkGray, _srcX, /*start x*/ _srcY, /*start Y*/
							_panel.ClientRectangle.Width - DstGridItemDot.WIDTH, /* end X */
							_dstY /* end Y */);
					}
					else
					{
						// highlighted line
						g.DrawLine(Pens.Black, _srcX, /*start x*/ _srcY, /*start Y*/
							_panel.ClientRectangle.Width - DstGridItemDot.WIDTH, /* end X */
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
					g.DrawLine(Pens.White, _srcX, /*start x*/ _srcY, /*start Y*/
						_panel.ClientRectangle.Width - DstGridItemDot.WIDTH, /* end X */
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
			composite.Components.Add(_dstDot);

			_outputDot.IsTightlyCoupled = false;
			composite.Components.Add(_outputDot);
		}
	}
}
