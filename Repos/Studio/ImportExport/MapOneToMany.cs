/*
* @(#)MapOneToMany.cs
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
	/// Representing a UI component for many to one mapping.
	/// </summary>
	/// <version> 1.0.0 08 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class MapOneToMany : MapRect
	{
		/// <summary>
		/// Initiate an instance of MapOneToMany
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		public MapOneToMany(MapPanel panel) : base(panel)
		{
		}

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>OneToMany</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.OneToMany;
			}
		}

		/// <summary>
		/// Create a IMappingNode instance represent this UI map component
		/// </summary>
		public override void CreateMapping()
		{
			_mapping = new OneToManyMapping();
			_mapping.X = this.X;
			_mapping.Y = this.Y;

			_panel.AttributeMappings.Add(_mapping);	
		}
	}
}
