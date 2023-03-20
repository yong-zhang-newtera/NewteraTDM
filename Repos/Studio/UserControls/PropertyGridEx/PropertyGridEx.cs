using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Newtera.Studio.UserControls.PropertyGridEx
{
	///	<summary>
	///	Extend PropertyGrid to allow them to be used in class attribute mapping purpose 
	///	</summary>
	public class PropertyGridEx	: System.Windows.Forms.PropertyGrid
	{
		public const int WM_LBUTTONDOWN = 0x0201;
		public const int WM_LBUTTONUP = 0x0202;
		public const int WM_RBUTTONDOWN = 0x0204;
		public const int WM_RBUTTONUP = 0x0205;

		private const int PropertyGridRowHeight = 17;
		private GridItem _selectedItem = null;
		private Hashtable _gridItemPositionTable = new Hashtable();

		/// <summary>
		/// Event Handler for GridItemClicked event
		/// </summary>
		public delegate void GridItemClickEventHandler(object sender, GridItemEventArgs e);
		
		public delegate void GridItemExpandedEventHandler(object sender, GridItemEventArgs e);

		public delegate void GridItemCollapsedEventHandler(object sender, GridItemEventArgs e);

		///	<summary>
		///	Required designer variable.
		///	</summary>
		private	System.ComponentModel.Container	components = null;

		public event GridItemClickEventHandler GridItemClicked;

		public event GridItemExpandedEventHandler GridItemExpanded;

		public event GridItemCollapsedEventHandler GridItemCollapsed;


		/// <summary>
		/// Initiate an instance of PropertyGrid
		/// </summary>
		public PropertyGridEx() : base()
		{
			// This	call is	required by	the	Windows.Forms Form Designer.
			InitializeComponent();

			this._selectedItem = this.SelectedGridItem;
		}

		/// <summary>
		/// Get an iterator that iterates through all grid items in property grid
		/// </summary>
		/// <returns>An iterator of GridItemIterator type</returns>
		public GridItemIterator AllGridItems
		{
			get
			{
				GridItem root = this.GetRootItem();

				GridItemIterator iterator = new GridItemIterator(root);

				return iterator;
			}
		}

		/// <summary>
		/// Get the information indicating whether a GridItem is visible or not.
		/// </summary>
		/// <param name="item">The GridItem instance</param>
		/// <returns>true if item is visible, false otherwise.</returns>
		public bool IsItemVisible(GridItem item)
		{
			bool visible= true;

			if (item.GridItemType != GridItemType.Root)
			{
				GridItem current = item;

				while (current.Parent.GridItemType != GridItemType.Root)
				{
					// if any of parent items is not expanded, the item is invisible
					if (current.Parent.Expanded == false)
					{
						visible = false;
						break;
					}

					current = current.Parent;
				}
			}
			else
			{
				visible = false;
			}

			return visible;
		}

		/// <summary>
		/// Calculate the actual height of a property grid based on
		/// number of visible rows
		/// </summary>
		/// <returns>The height of PropertyGrid in pixels</returns>
		public int GetActualHeight()
		{
			int rowCount = GetRowNumbers();

			return rowCount * (PropertyGridRowHeight + 3); // add 3 pixels for cell margin
		}

		/// <summary>
		/// Gets the Y coordinate for the given GridItem in the PropertyGrid
		/// </summary>
		/// <param name="gridItem">The given GridItem</param>
		/// <returns>The Y coordinate of the GridItem</returns>
		/// <remarks>If the grid item is invisible, returns Y coodinate of its
		/// first visible parent.</remarks>
		public int GetYCoordinate(GridItem gridItem)
		{			
			// Get display position of the gridItem.
			int pos = GetItemDisplayPosition(gridItem);

			return (pos - 1) * PropertyGridRowHeight + PropertyGridRowHeight / 2; // get middle point of a row
		}

		/// <summary>
		/// Gets the Y coordinate for the GridItem of given name in the PropertyGrid
		/// </summary>
		/// <param name="name">The name of a GridItem of Property type</param>
		/// <returns>The Y coordinate of the matched GridItem, 0 if not matched GridItem found.</returns>
		/// <remarks>If the grid item is invisible, returns Y coodinate of its
		/// first visible parent.</remarks>
		public int GetYCoordinate(string name)
		{
			GridItem gridItem = GetGridItem(name);

			if (gridItem != null)
			{
				int pos = GetItemDisplayPosition(gridItem);

				return (pos - 1) * PropertyGridRowHeight + PropertyGridRowHeight / 2; // get middle point of a row
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Get a GridItem of Property type by name
		/// </summary>
		/// <param name="name">The given name.</param>
		/// <returns>An GridItem, null if there is no GridItem of Property Type with the given name.</returns>
		public GridItem GetGridItem(string name)
		{
			GridItem root = GetRootItem();

			return GetGridItemFromCollection(name, root.GridItems);
		}

		public void CalculateGridItemPositions()
		{
            bool selected = false;
			this._gridItemPositionTable.Clear();

			int count = 0;
			GridItem root = this.GetRootItem();

			GridItemIterator iterator = new GridItemIterator(root);

			GridItem current;

			while ((current = iterator.Next()) != null)
			{
				if (IsItemVisible(current))
				{
					count++;
                    if (!selected && current.GridItemType == GridItemType.Property &&
                        !current.Expandable)
                    {
                        current.Select();
                        selected = true;
                    }
				}

				this._gridItemPositionTable.Add(current, count);
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

		#region Component	Designer generated code

		///	<summary>
		///	Required method	for	Designer support - do not modify 
		///	the	contents of	this method	with the code editor.
		///	</summary>
		private	void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion


		#region Controllers

		/// <summary>
		/// Get the root grid item of the PropertyGrid
		/// </summary>
		/// <returns>The GridItem instance that is a root</returns>
		private GridItem GetRootItem()
		{
			GridItem root = this.SelectedGridItem;

			if (root != null)
			{
				while (root.GridItemType != GridItemType.Root)
				{
					root = root.Parent;
				}
			}

			return root;
		}

		/// <summary>
		/// Get number of rows currently visible in a PropertyGrid
		/// </summary>
		/// <returns>A number of rows currently visible</returns>
		/// <remarks>The number of visible rows can be different from the number of
		/// Grid items in a PropertyGrid, because some of the Grid Items may be
		/// invisible. </remarks>
		private int GetRowNumbers()
		{
			GridItem rootItem = GetRootItem();

			if (rootItem != null)
			{
				return CountRows(rootItem.GridItems) + 1; // 1 is for row displaying root item
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Count the number of rows visible for a collection of GridItem instances
		/// </summary>
		/// <param name="gridItems">The collection of GridItem instances</param>
		/// <returns>The number of rows visible</returns>
		private int CountRows(GridItemCollection gridItems)
		{
			int count = 0;

			foreach (GridItem item in gridItems)
			{
				if (item.Expandable == true)
				{
					if (item.Expandable == true)
					{
						count += CountRows(item.GridItems);
					}
				}
				else
				{
					count ++;
				}
			}

			return count;
		}

		/// <summary>
		/// Given a name, try to get a GridItem of Property type from a collection of
		/// GridItems. This method can be called recursively.
		/// </summary>
		/// <returns>The macthed GridItem instance, null if there is no matched GridItem of Property Type.</returns>
		private GridItem GetGridItemFromCollection(string name, GridItemCollection gridItems)
		{
			GridItem found = null;

			foreach (GridItem item in gridItems)
			{
				if (item.GridItemType == GridItemType.Property &&
					item.PropertyDescriptor.Name == name)
				{
					found = item;
					break; // break the loop
				}

				if (item.Expandable == true)
				{
					// search the grid items from the child grid items
					found = GetGridItemFromCollection(name, item.GridItems);
					if (found != null)
					{
						break;
					}
				}

			}

			return found;
		}

		/// <summary>
		/// Gets the display position of the grid item
		/// </summary>
		/// <param name="gridItem">The given item</param>
		/// <returns>The display position in the PropertyGrid</returns>
		private int GetItemDisplayPosition(GridItem gridItem)
		{
			object val = this._gridItemPositionTable[gridItem];
			if (val == null)
			{
				throw new Exception("Unable to find the grid item in the position table");
			}
			
			return (int) val;
		}

		#endregion

		protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
		{
			if (this._selectedItem == null)
			{
				// ignore the first SelectedGridItemChanged event, for it is generated
				// by the PropertyGrid instantiation by default
				this._selectedItem = this.SelectedGridItem;
				return;
			}

			base.OnSelectedGridItemChanged(e);

			if (SelectedGridItem.Expandable)
			{
				if (SelectedGridItem.Expanded)
				{
					// trigger the event
					if (GridItemExpanded != null)
					{
						GridItemExpanded(this, new GridItemEventArgs(SelectedGridItem));
					}
				}
				else
				{
					if (GridItemCollapsed != null)
					{
						GridItemCollapsed(this, new GridItemEventArgs(SelectedGridItem));
					}
				}

				// set the root as the selected grid item so that it will generate
				// SelectedGridItemChanged event when user click on the grid item
				// again.
				bool status = GetRootItem().Select();
			}
			else if (SelectedGridItem.GridItemType == GridItemType.Property)
			{
				if (GridItemClicked != null)
				{
					GridItemClicked(this, new GridItemEventArgs(SelectedGridItem));
				}
			}
		}
	}

	/// <summary>
	/// The iterator that traverse the hierarchy of grid items in order of top-left.
	/// </summary>
	public class GridItemIterator
	{
		private GridItem _currentItem;
		private GridItem _endItem;

		/// <summary>
		/// Initiate an instance of GridItemIterator
		/// </summary>
		/// <param name="rootItem">The root item that iterator starts from</param>
		public GridItemIterator(GridItem rootItem)
		{
			_currentItem = rootItem;
			_endItem = null;
		}

		/// <summary>
		/// Initiate an instance of GridItemIterator
		/// </summary>
		/// <param name="rootItem">The root item that iterator starts from</param>
		/// <param name="endItem">The end item that iterator stops</param>
		public GridItemIterator(GridItem rootItem, GridItem endItem)
		{
			_currentItem = rootItem;
			_endItem = endItem;
		}

		/// <summary>
		/// Get the next GridItem of traverse
		/// </summary>
		/// <returns>An GridItem instance, null if the iterator reaches the end item.</returns>
		public GridItem Next()
		{
			GridItem item;

			if (_currentItem == null)
			{
				item = null;
			}
			else if (_endItem != null && IsSameItem(_currentItem, _endItem))
			{
				item = null; // reach the end item
			}
			else
			{
				item = _currentItem;

				if (_currentItem.GridItems.Count > 0)
				{
					// make the first child item as the current item
					_currentItem = _currentItem.GridItems[0];
				}
				else
				{
					// go to the next sibling item.
					_currentItem = NextSiblingItem(_currentItem);
				}
			}

			return item;
		}

		/// <summary>
		/// Get the next sibling item of the current item.
		/// </summary>
		/// <param name="currentItem">The current item</param>
		/// <returns>A GridItem, null if there isn't any sibling item available.</returns>
		private GridItem NextSiblingItem(GridItem currentItem)
		{
			GridItem nextItem = null;

			if (currentItem.Parent != null)
			{
				// go to the sibling if any
				GridItemCollection items = currentItem.Parent.GridItems;
				int index = 0;
				foreach (GridItem gridItem in items)
				{
					if (IsSameItem(currentItem, gridItem))
					{
						break;
					}

					index ++;
				}

				if (index + 1 < items.Count)
				{
					// there is a next sibling item
					nextItem = items[index + 1];
				}
				else
				{
					// it is the end of list, go up one level
					nextItem = NextSiblingItem(currentItem.Parent);
				}
			}

			return nextItem;
		}

		/// <summary>
		/// Gets the information indicating whether two GridItem instance are
		/// equals.
		/// </summary>
		/// <param name="first">The first GridItem instance</param>
		/// <param name="second">The second GridItem instance</param>
		/// <returns>true if they are equal, false otherwise.</returns>
		private bool IsSameItem(GridItem first, GridItem second)
		{
			bool status = false;

			if (first.GridItemType == GridItemType.Category &&
				second.GridItemType == GridItemType.Category &&
				first.Label == second.Label)
			{
				status = true;
			}
			else if (first.GridItemType == GridItemType.Property &&
				second.GridItemType == GridItemType.Property &&
				first.PropertyDescriptor.Name == second.PropertyDescriptor.Name)
			{
				status = true;
			}

			return status;
		}
	}

	/// <summary>
	/// Event Args for GridItemClicked, GridItemExpanded, and GridItemCollapsed events
	/// </summary>
	public class GridItemEventArgs : EventArgs
	{
		private GridItem _item = null;

		/// <summary>
		/// Initiate an instance of GridItemEventArgs
		/// </summary>
		/// <param name="item">The GridItem being clicked.</param>
		public GridItemEventArgs(GridItem item)
		{
			_item = item;
		}

		/// <summary>
		/// Gets the GridItem instance
		/// </summary>
		public GridItem Item
		{
			get
			{
				return _item; 
			}
		}
	}
}
