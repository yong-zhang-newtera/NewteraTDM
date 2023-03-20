using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Newtera.Studio.UserControls.PropertyGridEx;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Mappings;

namespace Newtera.Studio.ImportExport
{
	///	<summary>
	///	Extend Panel to draw lines representing mappings between source attributes
	///	and destination attributes. 
	///	</summary>
	public class MapPanel : System.Windows.Forms.Panel
	{
		private Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx _sourcePropertyGrid;
		private Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx _destinationPropertyGrid;
		private ClassMapping _classMapping;

		private Hashtable _srcDots;
		private Hashtable _dstDots;

		internal MapComponentCollection _components;

		internal AttributeMappingCollection _attributeMappings;

		private IMapComponent _selectedComponent = null;

		private ImageList _imageList;

		private IMapAction _action = null;

		///	<summary>
		///	Required designer variable.
		///	</summary>
		private	System.ComponentModel.Container	components = null;

		/// <summary>
		/// Event Handler for SelectedComponentChanged event
		/// </summary>
		public delegate void SelectedComponentChangedEventHandler(object sender, MapComponentEventArgs e);
		
		/// <summary>
		/// SelectedComponentChanged event handler
		/// </summary>
		public event SelectedComponentChangedEventHandler SelectedComponentChanged;

		/// <summary>
		/// Initiate an instance of MapPanel
		/// </summary>
		public MapPanel() : base()
		{
			// This	call is	required by	the	Windows.Forms Form Designer.
			InitializeComponent();

			_components = new MapComponentCollection();

			_attributeMappings = new AttributeMappingCollection();

			_srcDots = new Hashtable();
			_dstDots = new Hashtable();

			//this.SetStyle(ControlStyles.DoubleBuffer, true);
			//this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			// Needed for controls that are double-buffered
			//this.SetStyle(ControlStyles.UserPaint, true);
			
			// redraw when resized
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		/// <summary>
		/// Gets or sets the ImageList that contains icons for
		/// drawing map components.
		/// </summary>
		public ImageList ImageList
		{
			get
			{
				return _imageList;
			}
			set
			{
				_imageList = value;
			}
		}

		/// <summary>
		/// Gets or sets the PropertyGridEx instance for the source data.
		/// </summary>
		public PropertyGridEx SourcePropertyGrid
		{
			get
			{
				return _sourcePropertyGrid;
			}
			set
			{
				_sourcePropertyGrid = value;
			}
		}

		/// <summary>
		/// Gets or sets the PropertyGridEx instance for the destination data.
		/// </summary>
		public PropertyGridEx DestinationPropertyGrid
		{
			get
			{
				return _destinationPropertyGrid;
			}
			set
			{
				_destinationPropertyGrid = value;
			}
		}

		/// <summary>
		/// Gets the map components in the map panel
		/// </summary>
		public MapComponentCollection Components
		{
			get
			{
				return _components;
			}
		}

		/// <summary>
		/// Gets the currently selected component.
		/// </summary>
		/// <value>The currently selected component</value>
		public IMapComponent SelectedComponent
		{
			get
			{
				return _selectedComponent;
			}
		}

		/// <summary>
		/// Gets or sets the action being performed
		/// </summary>
		/// <value>IMapAction object</value>
		public IMapAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		/// <summary>
		/// Gets or sets the ClassMapping instance.
		/// </summary>
		public ClassMapping ClassMapping
		{
			get
			{
				return _classMapping;
			}
			set
			{
				_classMapping = value;

				// clear the existing components
				ClearMappings();

				if (value != null)
				{
					// show the attribute mappings in the class mapping
					ShowMappings(value);
				}
			}
		}

		/// <summary>
		/// Gets the newly created attribute mappings
		/// </summary>
		/// <value> A AttributeMappingCollection instance</value>
		public AttributeMappingCollection AttributeMappings
		{
			get
			{
				return _attributeMappings;
			}
		}

		/// <summary>
		/// Create a new collection for keeping attribute mappings
		/// </summary>
		public void InitAttributeCollection()
		{
			_attributeMappings = new AttributeMappingCollection();
		}

		/// <summary>
		/// Clear the any selections on UI components
		/// </summary>
		public void ClearSelection()
		{
			if (_selectedComponent != null)
			{
				_selectedComponent.Deselect();
			}

			this.Refresh();
		}

		/// <summary>
		/// Delete the given component from the map
		/// </summary>
		/// <param name="lineIndex">The line index.</param>
		public void DeleteComponent(IMapComponent component)
		{
			if (component.IsSelected)
			{
				component.Deselect();

				// fire SelectedComponentChanged event
				if (SelectedComponentChanged != null)
				{
					// fire the selected compoent changed event
					SelectedComponentChanged(this, new MapComponentEventArgs(null));
				}
			}

			component.Delete();

			this.Refresh();
		}

		/// <summary>
		/// Redraw the UI components in the panel in the case of GridItem's
		/// expanding or collapsing events
		/// </summary>
		public void Redraw()
		{
			try
			{
				this.RefreshDstDots();

				foreach (IMapComponent component in this._components)
				{
					component.AdjustPosition();
				}

				Refresh();
			}
			catch (Exception ex)
			{
				throw new MappingException(ex.Message);
			}
		}

		/// <summary>
		/// Clear all mappings
		/// </summary>
		public void ClearMappings()
		{
			_selectedComponent = null;

			_components.Clear();

			_srcDots.Clear();

			_dstDots.Clear();

			_attributeMappings.Clear();

			// fire SelectedComponentChanged event
			if (SelectedComponentChanged != null)
			{
				// fire the selected component changed event
				SelectedComponentChanged(this, new MapComponentEventArgs(null));
			}

			// recreate the src and dst dots
			this.RefreshSrcDots();
			this.RefreshDstDots();

			this.Refresh();
		}

		/// <summary>
		/// Draw the components in the map panel
		/// </summary>
		/// <remarks></remarks>
		public void DrawComponents()
		{
			foreach (IMapComponent component in _components)
			{
				component.Draw();
			}
		}


		///	<summary>
		///	Clean up any resources being used.
		///	</summary>
		protected override void	Dispose( bool disposing	)
		{
			if(	disposing )
			{
				if(	components != null )
					components.Dispose();
			}
			base.Dispose( disposing	);
		}

		/// <summary>
		/// Draw the mapping lines
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			DrawComponents();
		}

		/// <summary>
		/// Handle the mouse down event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			IMapComponent component = GetComponentAt(e.X, e.Y);
			if (component != null)
			{
				// get a component contains all related components to be selected
				IMapComponent groupComponent = component.GetSelectionGroup();
				SelectComponent(groupComponent);
			}
			else
			{
				// clear selection and fires component selection change event if nothing is hits
				this.ClearSelection();

				if (this.SelectedComponentChanged != null)
				{
					SelectedComponentChanged(this, new MapComponentEventArgs(null));
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (_action != null)
			{
				_action.X = e.X;
				_action.Y = e.Y;

				_action.Do();
				
				_action = null;
			}

			if (this.Cursor == Cursors.SizeAll)
			{
				this.Cursor = Cursors.Default;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);

			if (_action != null &&
				_action.ActionType == MapActionType.Move)
			{
				this.Refresh();
				
				_action.X = e.X;
				_action.Y = e.Y;
				((MoveRectAction) _action).Draw();
			}
		}


		#region Component	Designer generated code

		///	<summary>
		///	Required method	for	Designer support - do not modify 
		///	the	contents of	this method	with the code editor.
		///	</summary>
		private	void InitializeComponent()
		{

		}

		#endregion


		/// <summary>
		/// Build one-to-one mappings between source and destination attributes
		/// that have the same dsuplay name.
		/// </summary>
		internal void AutoMappings()
		{
			this.Cursor = Cursors.Hand;

			try
			{
				ConnectLineAction action;
				DstGridItemDot foundDstDot;
				int matchLength;
				bool isPerfectMatch;
				ArrayList mappedDstDots = new ArrayList();
				ArrayList postponedActions = new ArrayList();

				foreach (SrcGridItemDot srcDot in _srcDots.Values)
				{
					// search for a destination property grid item that has the
					// lable that is the same as that of source grid item.
					foundDstDot = null;
					matchLength = 0;
					isPerfectMatch = false;

					foreach (DstGridItemDot dstDot in _dstDots.Values)
					{
						if (dstDot.GridItem.GridItemType == GridItemType.Property)
						{
							string srcLabel = srcDot.GridItem.Label;
							string dstLabel = dstDot.GridItem.Label;
							if (srcLabel.Length == dstLabel.Length)
							{
								if (srcLabel == dstLabel)
								{
									foundDstDot = dstDot;
									isPerfectMatch = true;
									break;
								}
							}
							else if (srcLabel.Length > dstLabel.Length)
							{
								if (srcLabel.StartsWith(dstLabel))
								{
									if (dstLabel.Length > matchLength)
									{
										// best partial match, remember the match and continue searching
										foundDstDot = dstDot;
										matchLength = dstLabel.Length;
									}
								}
							}
							else if (dstLabel.Length > srcLabel.Length)
							{
								if (dstLabel.StartsWith(srcLabel))
								{
									if (srcLabel.Length > matchLength)
									{
										// best partial match, remember the match and continue searching
										foundDstDot = dstDot;
										matchLength = srcLabel.Length;
									}
								}
							}
						}
					}

					if (foundDstDot != null)
					{
						action = new ConnectLineAction(this);

						action.SrcComponent = srcDot;
						action.DstComponent = foundDstDot;
						action.IsSelected = false; // do not select the line

						// create a line between src and dst dots if
						// it is a perfect match with names.
						// if it is a partial match, put the action in the postponed list
						if (isPerfectMatch)
						{
							// perform the action if it passes validation
							if (action.IsValid)
							{
								action.Do();

								mappedDstDots.Add(foundDstDot); // remember the mapped dst dot
							}
						}
						else
						{
							postponedActions.Add(action);
						}
					}
				}

				// process each postponed action to see if a line has already created
				// for the destination dot, if not, perform the action
				foreach (ConnectLineAction act in postponedActions)
				{
					if (!mappedDstDots.Contains(act.DstComponent))
					{
						if (act.IsValid)
						{
							act.Do(); // create the mapping

							mappedDstDots.Add(act.DstComponent); // remember the mapped dst dot
						}
					}
				}

				this.Refresh();
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// Create a rectangle according to the given type and insert it to
		/// the panel at the mouse point.
		/// </summary>
		/// <param name="type">One of MapComponentType enum</param>
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>The created rectangle component</returns>
		internal MapRect CreateRectAt(MapComponentType type, int x, int y)
		{
			MapRect component = null;

			x = CheckXBound(x);
			y = CheckYBound(y);

			switch (type)
			{
				case MapComponentType.ManyToOne:
					component = new MapManyToOne(this);
					component.X = x;
					component.Y = y;
					component.Image = this.ImageList.Images[2];
					break;
				case MapComponentType.OneToMany:
					component = new MapOneToMany(this);
					component.X = x;
					component.Y = y;
					component.Image = this.ImageList.Images[3];
					break;
				case MapComponentType.ManyToMany:
					component = new MapManyToMany(this);
					component.X = x;
					component.Y = y;
					component.Image = this.ImageList.Images[4];
					break;
			}

			return component;
		}

		/// <summary>
		/// Check the boundary of map area and make sure x does not go out
		/// of bound.
		/// </summary>
		/// <param name="x">x</param>
		/// <returns>x winthin the bound</returns>
		internal int CheckXBound(int x)
		{
			if (x < SrcGridItemDot.WIDTH)
			{
				x = SrcGridItemDot.WIDTH;
			}
			else if (x > (this.ClientRectangle.Width - DstGridItemDot.WIDTH - MapRect.RectWidth))
			{
				x = this.ClientRectangle.Width - DstGridItemDot.WIDTH - MapRect.RectWidth;
			}

			return x;
		}

		/// <summary>
		/// Check the boundary of map area and make sure y does not go out
		/// of bound.
		/// </summary>
		/// <param name="y">y</param>
		/// <returns>y winthin the bound</returns>
		internal int CheckYBound(int y)
		{
			if (y < 0)
			{
				y = 0;
			}
			else if (y > this.ClientRectangle.Height - MapRect.RectHeight)
			{
				y = this.ClientRectangle.Height - MapRect.RectHeight;
			}

			return y;
		}

		/// <summary>
		/// Show the attribute mappings in a ClassMapping instance.
		/// </summary>
		/// <param name="classMapping">A ClassMapping instance</param>
		private void ShowMappings(ClassMapping classMapping)
		{
			IMappingNode nodeCopy = null;
			MapRect rect;
			MultiAttributeMappingBase multiMapping;

			foreach (IMappingNode mapping in classMapping.AttributeMappings)
			{
				// Keep a copy of the IMappingNode instance
				nodeCopy = mapping.Copy();
				_attributeMappings.Add(nodeCopy);

				switch (mapping.NodeType)
				{
					case Newtera.Common.MetaData.Mappings.NodeType.ArrayDataCellMapping:
					case Newtera.Common.MetaData.Mappings.NodeType.AttributeMapping:
                    case Newtera.Common.MetaData.Mappings.NodeType.PrimaryKeyMapping:
						
						AttributeMapping attrMapping = (AttributeMapping) mapping;

						MapLine line = new MapLine(this);

						line.MappingNode = nodeCopy;

						line.ShowMapping();

						break;

					case Newtera.Common.MetaData.Mappings.NodeType.OneToManyMapping:

						multiMapping = (MultiAttributeMappingBase) mapping;

						rect = CreateRectAt(MapComponentType.OneToMany,
							multiMapping.X, multiMapping.Y);

						rect.MappingNode = nodeCopy;

						rect.ShowMapping();

						break;

					case Newtera.Common.MetaData.Mappings.NodeType.ManyToOneMapping:

						multiMapping = (MultiAttributeMappingBase) mapping;

						rect = CreateRectAt(MapComponentType.ManyToOne,
							multiMapping.X, multiMapping.Y);

						rect.MappingNode = nodeCopy;

						rect.ShowMapping();

						break;

					case Newtera.Common.MetaData.Mappings.NodeType.ManyToManyMapping:

						multiMapping = (MultiAttributeMappingBase) mapping;

						rect = CreateRectAt(MapComponentType.ManyToMany,
							multiMapping.X, multiMapping.Y);

						rect.MappingNode= nodeCopy;

						rect.ShowMapping();

						break;
				}
			}

			_attributeMappings.IsAltered = false; // reset the flag
		}

		/// <summary>
		/// Select the given component and fire the SelectedComponentChanged event.
		/// </summary>
		/// <param name="component">The component to be selected.</param>
		internal void SelectComponent(IMapComponent component)
		{
			// deselect the previous selection
			this.ClearSelection();

			// select the component
			component.Select();
			_selectedComponent = component;

			if (SelectedComponentChanged != null)
			{
				if (component is IGridItemProxy)
				{
					SelectedComponentChanged(this, new MapComponentEventArgs(component,
						((IGridItemProxy) component).GridItem));
				}
				else
				{
					// fire the selected component changed event
					SelectedComponentChanged(this, new MapComponentEventArgs(component));
				}
			}

			this.Refresh(); // refresh the panle to draw selection
		}

		/// <summary>
		/// Gets the SrcGridItemDot instance by a key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>SrcGridItemDot instance</returns>
		internal SrcGridItemDot GetSrcDot(string key)
		{
			return (SrcGridItemDot) _srcDots[key];
		}

		/// <summary>
		/// Gets the DstGridItemDot instance by a key
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>DstGridItemDot instance</returns>
		internal DstGridItemDot GetDstDot(string key)
		{
			return (DstGridItemDot) _dstDots[key];
		}

		/// <summary>
		/// Get the IMapComponent object that is a mouse point
		/// </summary>
		/// <param name="x">The x coordinate</param>
		/// <param name="y">The y coordinate</param>
		/// <returns>The cliecked SrcGridItemDot object, null if none is clicked.</returns>
		private IMapComponent GetComponentAt(int x, int y)
		{
			IMapComponent found = null;

			foreach (IMapComponent component in _components)
			{
				if (component.Hits(x, y))
				{
					found = component;
					found.HitX = x;
					found.HitY = y;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// Refresh dot components to reflect changes in grid items in src property
		/// grid
		/// </summary>
		private void RefreshSrcDots()
		{
			if (_sourcePropertyGrid != null)
			{
				GridItemIterator iterator = _sourcePropertyGrid.AllGridItems;

				GridItem current;
				SrcGridItemDot srcDot;

				while ((current = iterator.Next()) != null)
				{
					if (current.GridItemType != GridItemType.Root)
					{
						srcDot = new SrcGridItemDot(this,
							this._sourcePropertyGrid, current);

						_components.Add(srcDot);

						if (srcDot.AttributeName != null)
						{
							_srcDots[srcDot.AttributeName] = srcDot;
						}
					}
				}
			}
		}

		/// <summary>
		/// Refresh dot components to reflect the changes of grid items in destination property
		/// grid
		/// </summary>
		private void RefreshDstDots()
		{
			if (_destinationPropertyGrid != null)
			{
				GridItemIterator iterator = _destinationPropertyGrid.AllGridItems;

				GridItem current;
				DstGridItemDot dstDot;

				while ((current = iterator.Next()) != null)
				{
					// create an DstGridItemDot instance if it has not been created
					if (current.GridItemType != GridItemType.Root)
					{
						string key;
						if (current.GridItemType == GridItemType.Property)
						{
							key = current.PropertyDescriptor.Name;
						}
						else
						{
							key = current.Label;
						}

						// make sure that the dst dot has not been created before
						if (_dstDots[key] == null)
						{
							dstDot = new DstGridItemDot(this,
								DestinationPropertyGrid,
								current);

							_components.Add(dstDot);

							if (dstDot.AttributeName != null)
							{
								_dstDots[dstDot.AttributeName] = dstDot;
							}
							else
							{
								_dstDots[current.Label] = dstDot;
							}
						}
						else
						{
							// found an existing DstGridItem instance,
							// replace the GridItem in the DstGridItem since
							// the previousely stored GridItem may be invalid
							dstDot = (DstGridItemDot) _dstDots[key];
							dstDot.GridItem = current;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Event Args for SelectedComponentChanged event
	/// </summary>
	public class MapComponentEventArgs : EventArgs
	{
		private IMapComponent _selectedComponent;
		private GridItem _gridItem;

		/// <summary>
		/// Initiate an instance of MapComponentEventArgs
		/// </summary>
		/// <param name="selectedComponent">The selected component.</param>
		public MapComponentEventArgs(IMapComponent selectedComponent)
		{
			_selectedComponent = selectedComponent;
			_gridItem = null;
		}

		/// <summary>
		/// Initiate an instance of MapComponentEventArgs
		/// </summary>
		/// <param name="selectedComponent">The selected component.</param>
		/// <param name="gridItem">The GridItem object associated with the selected component.</param>
		public MapComponentEventArgs(IMapComponent selectedComponent,
			GridItem gridItem)
		{
			_selectedComponent = selectedComponent;
			_gridItem = gridItem;
		}

		/// <summary>
		/// Gets the selected component.
		/// </summary>
		/// <value>Null if a component is deselected</value>
		public IMapComponent SelectedComponent
		{
			get
			{
				return _selectedComponent; 
			}
		}

		/// <summary>
		/// Gets the grid item associated with the component if exists.
		/// </summary>
		/// <value>Can be null</value>
		public GridItem GridItem
		{
			get
			{
				return _gridItem; 
			}
		}
	}
}
