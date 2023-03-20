/*
* @(#)MapRect.cs
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
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Transform;

	/// <summary>
	/// Representing a rectangle component that serves as a base class for
	/// OneToMany, ManyToOne, and ManyToMany mapping components.
	/// </summary>
	/// <version> 1.0.0 8 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public abstract class MapRect : MapComponentBase
	{
		internal const int RectWidth = 22;
		internal const int RectHeight = 22;
		internal const int DotWidth = 9;
		internal const int DotHeight = 9;
		internal IMapComponent _inputDot;
		internal IMapComponent _outputDot;

		private int _x;
		private int _y;

		private MapComponentCollection _inputLines;
		private MapComponentCollection _outputLines;

		private Image _image;

		protected MultiAttributeMappingBase _mapping;

		/// <summary>
		/// Initiate an instance of MapRect
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		public MapRect(MapPanel panel) : base(panel)
		{
			_x = 0;
			_y = 0;
			_mapping = null;

			_inputLines = new MapComponentCollection();
			_outputLines = new MapComponentCollection();
			_inputDot = new InputDot(_panel, this);
			_outputDot = new OutputDot(_panel, this);
		}

		/// <summary>
		/// Gets or sets the starting x point for the rectangle
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

				if (_mapping != null)
				{
					_mapping.X = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the starting y point for the rectangle
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

				if (_mapping != null)
				{
					_mapping.Y = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the Image object of the component
		/// </summary>
		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		/// <summary>
		/// Gets a collection of input lines
		/// </summary>
		/// <value>A MapComponentCollection instance</value>
		internal MapComponentCollection InputLines
		{
			get
			{
				return this._inputLines;
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
				return this._outputLines;
			}
		}

		/// <summary>
		/// Add an input line to a rectangle
		/// </summary>
		/// <param name="line">IMapComponent</param>
		public void AddInputLine(IMapComponent line)
		{
			InputLine inputLine = (InputLine) line;

			_inputLines.Add(inputLine);

			InputOutputAttribute inputAttribute = new InputOutputAttribute(inputLine.SourceAttributeName);
			// Set a Getter type accroding to the GridItem in source property grid
			if (inputLine.SrcDot.GridItem.PropertyDescriptor is InstanceAttributePropertyDescriptor)
			{
				InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor) inputLine.SrcDot.GridItem.PropertyDescriptor;
				if (!pd.IsPrimaryKey)
				{
					inputAttribute.GetterType = GetterType.SimpleAttributeGetter;
				}
				else
				{
					inputAttribute.GetterType = GetterType.PrimaryKeyGetter;
				}
			}
			else if (inputLine.SrcDot.GridItem.PropertyDescriptor is ArrayDataCellPropertyDescriptor)
			{
				inputAttribute.GetterType = GetterType.ArrayDataCellGetter;
			}

			_mapping.InputAttributes.Add(inputAttribute);
		}


		/// <summary>
		/// Add an output line to a rectangle
		/// </summary>
		/// <param name="line">IMapComponent</param>
		public void AddOutputLine(IMapComponent line)
		{
			OutputLine outputLine = (OutputLine) line;

			_outputLines.Add(line);

			InputOutputAttribute outputAttribute = new InputOutputAttribute(outputLine.DestinationAttributeName);

			// Set a Setter type accroding to the GridItem in destination property
			// grid
			if (outputLine.DstDot.GridItem.PropertyDescriptor is InstanceAttributePropertyDescriptor)
			{
				InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor) outputLine.DstDot.GridItem.PropertyDescriptor;
				if (!pd.IsPrimaryKey)
				{
					outputAttribute.SetterType = SetterType.SimpleAttributeSetter;
				}
				else
				{
					outputAttribute.SetterType = SetterType.PrimaryKeySetter;
				}
			}
			else if (outputLine.DstDot.GridItem.PropertyDescriptor is ArrayDataCellPropertyDescriptor)
			{
				outputAttribute.SetterType = SetterType.ArrayDataCellSetter;
			}

			_mapping.OutputAttributes.Add(outputAttribute);
		}

		/// <summary>
		/// Remove an input line from a rectangle
		/// </summary>
		/// <param name="line">IMapComponent</param>
		public void RemoveInputLine(IMapComponent line)
		{
			_inputLines.Remove(line);

			string attrName = ((InputLine) line).SourceAttributeName;

			foreach (InputOutputAttribute attribute in _mapping.InputAttributes)
			{
				if (attribute.AttributeName == attrName)
				{
					_mapping.InputAttributes.Remove(attribute);
					break;
				}
			}
		}

		/// <summary>
		/// Remove an output line from a rectangle
		/// </summary>
		/// <param name="line">IMapComponent</param>
		public void RemoveOutputLine(IMapComponent line)
		{
			_outputLines.Remove(line);

			string attrName = ((OutputLine) line).DestinationAttributeName;

			foreach (InputOutputAttribute attribute in _mapping.OutputAttributes)
			{
				if (attribute.AttributeName == attrName)
				{
					_mapping.OutputAttributes.Remove(attribute);
					break;
				}
			}
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
				return _mapping;
			}
			set
			{
				if (value is MultiAttributeMappingBase)
				{
					_mapping = (MultiAttributeMappingBase) value;

					foreach (InputOutputAttribute inputAttribute in _mapping.InputAttributes)
					{
						
						InputLine line = new InputLine(this._panel,
							_panel.GetSrcDot(inputAttribute.AttributeName),
							(InputDot) _inputDot);

						_inputLines.Add(line);
					}

					foreach (InputOutputAttribute outputAttribute in _mapping.OutputAttributes)
					{
						
						OutputLine line = new OutputLine(this._panel,
							(OutputDot) _outputDot,
							_panel.GetDstDot(outputAttribute.AttributeName));

						_outputLines.Add(line);
					}
				}
			}
		}

		/// <summary>
		///  Gets the information indicating whether the mapping represented by
		///  this component has existed.
		/// </summary>
		/// <returns>True if it existed, false otherwise.</returns>
		public override bool IsMappingExist()
		{
			return false;
		}

		/// <summary>
		/// Show the mapping(s) on the map panel
		/// </summary>
		public override void ShowMapping()
		{
			_panel.Components.Add(_inputDot);
			_panel.Components.Add(this);
			_panel.Components.Add(_outputDot);

			foreach (IMapComponent line in this._inputLines)
			{
				_panel.Components.Add(line);
			}

			foreach (IMapComponent line in this._outputLines)
			{
				_panel.Components.Add(line);
			}
		}

		/// <summary>
		/// Get the group of components that are supposed to be selected together
		/// </summary>
		/// <returns>IMapComponent</returns>
		public override IMapComponent GetSelectionGroup()
		{
			MapComposite composite = new MapComposite(_panel, this);

			composite.Components.Add(this);

			// add the input and output dots connected by the rectangle
			AddInAndOutDots(composite);

			return composite;
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
			_panel.Components.Remove(this);

			if (_mapping != null)
			{
				_panel.AttributeMappings.Remove(_mapping);
			}
		}

		/// <summary>
		/// Gets the information indicating whether a given point hits on the
		/// rectangle
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public override bool Hits(int x, int y)
		{
			bool status = false;

			if (x >= _x && x < (_x + MapRect.RectWidth) &&
				y >= _y && y < (_y + MapRect.RectHeight))
			{
				status = true;
			}

			return status;
		}

		/// <summary>
		/// Draw a line on the panel to show the mapping
		/// </summary>
		public override void Draw()
		{
			if (IsVisible)
			{
				using (Graphics g = this._panel.CreateGraphics())
				{
					if (!IsSelected)
					{
						using (Pen pen = new Pen(Color.DarkGray, 2.0f))
						{
							// a regular rectangle
							g.DrawRectangle(pen,
								_x, /*start x*/ _y, /*start Y*/
								MapRect.RectWidth, /* width */
								MapRect.RectHeight /* height */);
						}

						g.DrawImageUnscaled(_image, _x + 3, _y + 3);
					}
					else
					{
						using (Pen pen = new Pen(Color.Black, 2.0f))
						{
							// a highlighted rectangle
							g.DrawRectangle(pen,
								_x, /*start x*/ _y, /*start Y*/
								MapRect.RectWidth, /* width */
								MapRect.RectHeight /* height */);
						}

						g.DrawImageUnscaled(_image, _x + 3, _y + 3);
					}
				}
			}
		}

		/// <summary>
		/// Erase a rect on the panel
		/// </summary>
		public override void Erase()
		{
			if (IsVisible)
			{
				// TODO, only erase the dot that is visible completely or partially
				using (Graphics g = this._panel.CreateGraphics())
				{
					// fill the rectangle with white color
					g.FillRectangle(Brushes.White,
						_x, /*start x*/ _y, /*start Y*/
						MapRect.RectWidth, /* width */
						MapRect.RectHeight /* height */);
				}
			}
		}

		#endregion

		/// <summary>
		/// Add input and output dots and lines to the rectangle as a whole
		/// </summary>
		/// <param name="composite">The composites</param>
		private void AddInAndOutDots(MapComposite composite)
		{
			_inputDot.IsTightlyCoupled = true;
			composite.Components.Add(_inputDot);

			_outputDot.IsTightlyCoupled = true;
			composite.Components.Add(_outputDot);

			foreach (IMapComponent line in this._inputLines)
			{
				composite.Components.Add(line);
			}

			foreach (IMapComponent line in this._outputLines)
			{
				composite.Components.Add(line);
			}
		}
	}
}
