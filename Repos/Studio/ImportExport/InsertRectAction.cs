/*
* @(#)InsertRectAction.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Studio.ImportExport
{
	using System;
	using System.Windows.Forms;

	/// <summary>
	/// A action that inserts a component in the panel
	/// </summary>
	/// <version> 1.0.0 10 Nov 2004 </version>
	/// <author> Yong Zhang</author>
	public class InsertRectAction : MapActionBase
	{
		private MapComponentType _type;

		/// <summary>
		/// Initiate an instance of InsertRectAction
		/// </summary>
		public InsertRectAction(MapPanel panel, MapComponentType type) : base(panel)
		{
			_type = type;
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
				return MapActionType.Insert;
			}
		}


		/// <summary>
		/// Perform the action
		/// </summary>
		public override void Do()
		{
			MapRect rect = _panel.CreateRectAt(_type, X, Y);

			if (rect != null)
			{
				_panel.Components.Add(rect._inputDot);
				_panel.Components.Add(rect);
				_panel.Components.Add(rect._outputDot);

				rect.CreateMapping();

				// select the rectangle
				IMapComponent groupSelection = rect.GetSelectionGroup();
				_panel.SelectComponent(groupSelection); // select the rect and its associates
			}
		}

		#endregion
	}
}
