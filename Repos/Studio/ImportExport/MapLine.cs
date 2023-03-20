/*
* @(#)MapLine.cs
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
	using Newtera.Common.MetaData.Mappings.Transform;
	using Newtera.Studio.UserControls.PropertyGridEx;

	/// <summary>
	/// Representing a line showing a mapping between source and destination
	/// attributes.
	/// </summary>
	/// <version> 1.0.0 21 Sep 2004 </version>
	/// <author> Yong Zhang</author>
	public class MapLine : MapComponentBase
	{
		private SrcGridItemDot _srcDot;
		private DstGridItemDot _dstDot;
		private int _srcY;
		private int _dstY;
		private AttributeMapping _attributeMapping;

		/// <summary>
		/// Initiate an instance of MapLine
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		public MapLine(MapPanel panel) : base(panel)
		{
			_srcDot = null;
			_dstDot = null;
			_attributeMapping = null;

			_srcY = 0;
			_dstY = 0;
		}

		/// <summary>
		/// Initiate an instance of MapLine
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		/// <param name="srcDot">The SrcGridItemDot instance</param>
		/// <param name="dstDot">The DstGridItemDot instance</param>
		public MapLine(MapPanel panel, SrcGridItemDot srcDot,
			DstGridItemDot dstDot) : base(panel)
		{
			_srcDot = srcDot;
			_dstDot = dstDot;
			_attributeMapping = null;

			_srcY = _srcDot.YPoint;
			_dstY = _dstDot.YPoint;
		}

		#region IMapComponent

		/// <summary>
		/// Gets or sets the IMappingNode associated with the mapping UI componnet
		/// </summary>
		/// <value>The associated IMappingNode instance, it could be null</value>
		public override IMappingNode MappingNode
		{
			get
			{
				return _attributeMapping;
			}
			set
			{
				if (value is AttributeMapping)
				{
					_attributeMapping = (AttributeMapping) value;

					_srcDot = (SrcGridItemDot) _panel.GetSrcDot(_attributeMapping.SourceAttributeName);
					if (_srcDot == null)
					{
						throw new Exception(ImportExportResourceManager.GetString("ImportWizard.MissingSrcAttribute") + " " + _attributeMapping.SourceAttributeName);
					}

					_dstDot = (DstGridItemDot) _panel.GetDstDot(_attributeMapping.DestinationAttributeName);
					if (_dstDot != null)
					{
                        _srcY = _srcDot.YPoint;
                        _dstY = _dstDot.YPoint;
					}
                    else
                    {
                        // Destination attribute in the mapping may have been removed in the destination class, igonre it
                        //throw new Exception(ImportExportResourceManager.GetString("ImportWizard.MissingDstAttribute") + " " + _attributeMapping.DestinationAttributeName);
                    }
                }
			}
		}

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>OneToOne</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.OneToOne;
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

			foreach (IMappingNode mapping in _panel.AttributeMappings)
			{
				if (mapping is AttributeMapping)
				{
					AttributeMapping attrMapping = (AttributeMapping) mapping;
					if (attrMapping.SourceAttributeName == _srcDot.GridItem.PropertyDescriptor.Name &&
						attrMapping.DestinationAttributeName == _dstDot.GridItem.PropertyDescriptor.Name)
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
			// create an AttributeMapping or ArrayDataCellMapping instances
			if (_dstDot.GridItem.PropertyDescriptor is InstanceAttributePropertyDescriptor)
			{
				InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor) _dstDot.GridItem.PropertyDescriptor;
				if (!pd.IsPrimaryKey)
				{
					_attributeMapping = new AttributeMapping(_srcDot.GridItem.PropertyDescriptor.Name,
						_dstDot.GridItem.PropertyDescriptor.Name);
					_attributeMapping.GetterType = GetGetterType();
					_panel.AttributeMappings.Add(_attributeMapping);
				}
				else
				{
					_attributeMapping = new PrimaryKeyMapping(_srcDot.GridItem.PropertyDescriptor.Name,
						_dstDot.GridItem.PropertyDescriptor.Name);
					_attributeMapping.GetterType = GetGetterType();
					_panel.AttributeMappings.Add(_attributeMapping);
				}
			}
			else if (_dstDot.GridItem.PropertyDescriptor is ArrayDataCellPropertyDescriptor)
			{
				_attributeMapping = new ArrayDataCellMapping(_srcDot.GridItem.PropertyDescriptor.Name,
					_dstDot.GridItem.PropertyDescriptor.Name);
				_attributeMapping.GetterType = GetGetterType();
				_panel.AttributeMappings.Add(_attributeMapping);
			}

			// Add the line to show the mapping
			if (_attributeMapping != null)
			{
				_panel.Components.Add(this);

				if (IsSelected)
				{
					IMapComponent selectionGroup = this.GetSelectionGroup();

					_panel.SelectComponent(selectionGroup); // select the newly added line
				}
			}
		}

		/// <summary>
		/// Show the mapping(s) on the map panel
		/// </summary>
		public override void ShowMapping()
		{
			_panel.Components.Add(this);
		}

		/// <summary>
		/// Get the group of components that are supposed to be selected together
		/// </summary>
		/// <returns>IMapComponent</returns>
		public override IMapComponent GetSelectionGroup()
		{
			MapComposite composite = new MapComposite(this._panel, this);

			composite.Components.Add(this);

			// add the source and destination dots connected by the line
			AddConnectedDots(composite);

			return composite;
		}

		/// <summary>
		/// Adjust the position of the line in the map panel
		/// </summary>
		public override void AdjustPosition()
		{
			_srcY = _srcDot.YPoint;
			_dstY = _dstDot.YPoint;
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
			_panel.Components.Remove(this);

			_panel.AttributeMappings.Remove(_attributeMapping);
		}

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// line
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public override bool Hits(int x, int y)
		{
			float ratio = ((_dstY - _srcY) * 1.0f) / (_panel.DisplayRectangle.Width - SrcGridItemDot.WIDTH * 2);
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
							_panel.DisplayRectangle.Width - SrcGridItemDot.WIDTH, /* end X */
							_dstY /* end Y */);
					}
					else
					{
						// selected line
						g.DrawLine(Pens.Black, SrcGridItemDot.WIDTH, /*start x*/ _srcY, /*start Y*/
							_panel.DisplayRectangle.Width - SrcGridItemDot.WIDTH, /* end X */
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
					g.DrawLine(Pens.White, SrcGridItemDot.WIDTH, /*start x*/ _srcY, /*start Y*/
						_panel.DisplayRectangle.Width - SrcGridItemDot.WIDTH, /* end X */
						_dstY /* end Y */);
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the component is
		/// visible or not on the panel
		/// </summary>
		/// <value>True if it visible, false otherwise.</value>
		public override bool IsVisible
		{
			get
			{
				bool isVisible = true;

				Rectangle rect = this._panel.ClientRectangle;
				Rectangle rect2 = this._panel.DisplayRectangle;
				return isVisible;
			}
			set
			{
				base.IsVisible = value;
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

			composite.Components.Add(_dstDot);
		}

		/// <summary>
		/// Get the type of source item
		/// </summary>
		/// <returns>One of the GetterType enum</returns>
		private GetterType GetGetterType()
		{
			GetterType getterType = GetterType.SimpleAttributeGetter; // default

			if (_srcDot.GridItem.PropertyDescriptor is InstanceAttributePropertyDescriptor)
			{
				InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor)_srcDot.GridItem.PropertyDescriptor;
				if (pd.IsPrimaryKey)
				{
					getterType = GetterType.PrimaryKeyGetter;
				}
			}
			else if (_srcDot.GridItem.PropertyDescriptor is ArrayDataCellPropertyDescriptor)
			{
				getterType = GetterType.ArrayDataCellGetter;
			}

			return getterType;
		}
	}
}
