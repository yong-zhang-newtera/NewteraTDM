/*
* @(#)MapManyToOne.cs
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
	public class MapManyToOne : MapRect
	{
		/// <summary>
		/// Initiate an instance of MapManyToOne
		/// </summary>
		/// <param name="panel">The panel on which to draw lines</param>
		public MapManyToOne(MapPanel panel) : base(panel)
		{
		}

		/// <summary>
		/// Gets the type of the componnet
		/// </summary>
		/// <value>ManyToOne</value>
		public override MapComponentType ComponentType
		{
			get
			{
				return MapComponentType.ManyToOne;
			}
		}

		/// <summary>
		/// Create a IMappingNode instance represent this UI map component
		/// </summary>
		public override void CreateMapping()
		{
			_mapping = new ManyToOneMapping();
			_mapping.X = this.X;
			_mapping.Y = this.Y;

			_panel.AttributeMappings.Add(_mapping);	
		}
	}
}
