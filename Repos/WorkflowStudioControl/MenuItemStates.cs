/*
* @(#)MenuItemStates.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowStudioControl
{
	using System;
	using System.Collections;
	using System.Windows.Forms;
	
	/// <summary>
	/// A central place to keep the enabling states of all memnu items. When state of
	/// a memu item changes, it will notify the menu items in different places,
	/// including main menu, context menu, and toolbar to change the enabling
	/// states accordingly.
	/// </summary>
	/// <version>  1.0.1 13 Dec 2006</version>
	public class MenuItemStates
	{
		public event EventHandler StateChanged;

		private Hashtable _states;

		/// <summary>
		/// Create an instance of MenuItemStates
		/// </summary>
		public MenuItemStates()
		{
			_states = new Hashtable();
		}

		/// <summary>
		/// Enable or disable a menu item.
		/// </summary>
		/// <param name="id">Id of menu item</param>
		/// <param name="state">true to enable the menu item, false to disable</param>
		public void SetState(MenuItemID id, bool state)
		{
			_states[id] = state;

			// Raise the event for state change
			if (StateChanged != null)
			{
				StateChanged(this, new StateChangedEventArgs(id, state));
			}
		}

		/// <summary>
		/// Gets the state of a menu item of given id
		/// </summary>
		/// <param name="id">menu item id</param>
		/// <returns>the state</returns>
		public bool GetState(MenuItemID id)
		{
			bool state = true;

			if (_states[id] != null)
			{
				state = (bool) _states[id];
			}

			return state;
		}

		/// <summary>
		/// Restore the existing menu states
		/// </summary>
		public void RestoreMenuStates()
		{
			Array menuIDs = Enum.GetValues(typeof(MenuItemID));
			bool state;

			foreach (MenuItemID menuId in menuIDs)
			{
				state = false; // default menu state is false

				if (_states[menuId] != null)
				{
					state = (bool) _states[menuId];
				}

				// Raise the event for state change
				if (StateChanged != null)
				{
					StateChanged(this, new StateChangedEventArgs(menuId, state));
				}			
			}
		}
	}

	public class StateChangedEventArgs : EventArgs
	{
		public MenuItemID ID;
		public bool State;

		public StateChangedEventArgs(MenuItemID id, bool state) 
		{
			this.ID = id;
			this.State = state;
		}
	}

	/// <summary>
	/// Specify the ID of menu item
	/// </summary>
	public enum MenuItemID
	{
		FileNew = 0,
		FileOpenFile,
		FileOpenDatabase,
		FileSaveFile,
		FileSaveAsFile,
		FileSaveDatabase,
        FileSaveDatabaseAs,
		FileSetup,
		FilePreview,
        FilePrint,
		FileExit,
		EditCut,
		EditCopy,
		EditPaste,
		EditAdd,
		EditDelete,
        EditRename,
		ViewProject,
		ViewProperties,
		ViewActivities,
        ViewWorkflowMonitor,
		WorkflowExpand,
        WorkflowCollapse,
		WorkflowZoomLevel,
        WorkflowTool,
        WorkflowStart,
        WorkflowValidate,
		ToolLock,
		ToolUnlock,
        ToolAccessControl
	}
}