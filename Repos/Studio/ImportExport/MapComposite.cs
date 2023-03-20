/*
* @(#)MapComposite.cs
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
	/// Representing a group of map components that can be selected as a whole
	/// </summary>
	/// <version> 1.0.0 8 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class MapComposite : MapComponentBase
	{
		private MapComponentCollection _components;
		private IMapComponent _principal;

		/// <summary>
		/// Initiate an instance of MapComposite
		/// </summary>
		/// <param name="panel">The map panel</param>
		public MapComposite(MapPanel panel, IMapComponent principal) : base(panel)
		{
			_components = new MapComponentCollection();
			_principal = principal;
		}


		/// <summary>
		/// Gets the collection of map components in the composite
		/// </summary>
		public MapComponentCollection Components
		{
			get
			{
				return _components;
			}
		}


		#region IMapComponent

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>Composite</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.Composite;
			}
		}

		/// <summary>
		/// Gets the principal component when it represents a group of
		/// components.
		/// </summary>
		public override IMapComponent Principal
		{
			get
			{
				return _principal;
			}
		}

		/// <summary>
		/// Adjust the position of the components in the map panel
		/// </summary>
		/// <remarks> Can be overrided by the subclasses</remarks>
		public override void AdjustPosition()
		{
			foreach (IMapComponent component in this._components)
			{
				component.AdjustPosition();
			}
		}

		/// <summary>
		/// Perform necessary actions that removes the component from the map.
		/// </summary>
		public override void Delete()
		{
			foreach (IMapComponent component in this._components)
			{
				component.Delete();
			}
		}

		/// <summary>
		/// Select all components in the composit
		/// </summary>
		public override void Select()
		{
			base.Select();

			foreach (IMapComponent component in this._components)
			{
				component.Select();
			}
		}

		/// <summary>
		/// Deselect all components in the composit
		/// </summary>
		public override void Deselect()
		{
			base.Deselect();

			foreach (IMapComponent component in this._components)
			{
				component.Deselect();
			}
		}

		/// <summary>
		/// Gets the information indicating whether a given point hits on
		/// one of the components in the collection
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>True if it hits the line, false, otherwise.</returns>
		public override bool Hits(int x, int y)
		{
			bool status = false;

			foreach (IMapComponent component in this._components)
			{
				if (component.Hits(x, y))
				{
					status = true;
					break;
				}
			}

			return status;
		}

		/// <summary>
		/// Draw all components in the composite
		/// </summary>
		public override void Draw()
		{
			foreach (IMapComponent component in this._components)
			{
				component.Draw();
			}
		}

		/// <summary>
		/// Erase a rect on the panel
		/// </summary>
		public override void Erase()
		{
			foreach (IMapComponent component in this._components)
			{
				component.Erase();
			}
		}

		#endregion
	}
}
