/*
* @(#)ConnectLineAction.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// A action that connects two components with a line
	/// </summary>
	/// <version> 1.0.0 10 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class ConnectLineAction : MapActionBase
	{
		private IMapComponent _src = null;
		private IMapComponent _dst = null;
		private IMapComponent _line = null;
		private string _errMessage = "";

		/// <summary>
		/// Initiate an instance of ConnectLineAction
		/// </summary>
		public ConnectLineAction(MapPanel panel) : base(panel)
		{
		}

		/// <summary>
		/// Gets or sets the component represents the source of the connection
		/// </summary>
		/// <value>IMapComponent</value>
		public IMapComponent SrcComponent
		{
			get
			{
				return _src;
			}
			set
			{
				_src = value;

				CreateLine(); // create a line
			}
		}

		/// <summary>
		/// Gets or sets the component represents the destination of the connection
		/// </summary>
		/// <value>IMapComponent</value>
		public IMapComponent DstComponent
		{
			get
			{
				return _dst;
			}
			set
			{
				_dst = value;

				CreateLine(); // create a line
			}
		}

		/// <summary>
		/// Gets the information indicating whether the connection from a source component
		/// to a destination connection is a valid or not?
		/// </summary>
		/// <value>true if it is a valid connection, false otherwise.</value>
		public bool IsValid
		{
			get
			{
				if (_src != null && _dst != null)
				{
                    if (_line == null)
                    {
                        _errMessage = ImportExportResourceManager.GetString("Error.InvalidMapping");

                        return false;
                    }
					else if (!_line.IsValid)
					{
						_errMessage = ImportExportResourceManager.GetString("Error.InvalidMapping");

						return false;
					}
					else if (_line.IsMappingExist())
					{
						_errMessage = ImportExportResourceManager.GetString("Error.MappingExist");

						return false;
					}
					else if (_line is InputLine &&
						_dst is InputDot &&
						((InputDot) _dst).MappingType == MapComponentType.OneToMany &&
						((InputDot) _dst).InputLines.Count > 0)
					{
						_errMessage = ImportExportResourceManager.GetString("Error.InputLimit");

						return false;
					}
					else if (_line is OutputLine &&
						_src is OutputDot &&
						((OutputDot) _src).MappingType == MapComponentType.ManyToOne &&
						((OutputDot) _src).OutputLines.Count > 0)
					{
						_errMessage = ImportExportResourceManager.GetString("Error.OutputLimit");

						return false;
					}

					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Gets the error message as result of validating the connection
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage
		{
			get
			{
				return this._errMessage;
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
				return MapActionType.Connect;
			}
		}

		/// <summary>
		/// Perform the action
		/// </summary>
		public override void Do()
		{
			if (_line != null)
			{
				if (!_line.IsMappingExist())
				{
					_line.IsSelected = IsSelected;
					_line.CreateMapping();
				}
			}
		}

		#endregion

		/// <summary>
		/// Create a IMapComponent instance represent the line
		/// </summary>
		private void CreateLine()
		{
			if (_src != null && _dst != null)
			{
				if (_src.ComponentType == MapComponentType.SourceEnd)
				{
					if (_dst.ComponentType == MapComponentType.DestinationEnd)
					{
						_line = new MapLine(_panel, (SrcGridItemDot) _src,
							(DstGridItemDot) _dst);
					}
					else if (_dst.ComponentType == MapComponentType.InputEnd)
					{
						_line = new InputLine(_panel, (SrcGridItemDot) _src,
							(InputDot) _dst);
					}
				}
				else if (_src.ComponentType == MapComponentType.OutputEnd &&
					_dst.ComponentType == MapComponentType.DestinationEnd)
				{
					_line = new OutputLine(_panel, (OutputDot) _src,
						(DstGridItemDot) _dst);
				}
			}
		}
	}
}
