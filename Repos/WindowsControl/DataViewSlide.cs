/*
* @(#)DataViewSlide.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Data;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;
	
	/// <summary>
	/// Represents a slide of data view for the data grid
	/// </summary>
	/// <version>  1.0.1 05 Nov 2003</version>
	/// <author>  Yong Zhang</author>
	public class DataViewSlide
	{
		private DataViewModel _dataView;
		private DataSet _dataSet;
		private InstanceView _rowView;
		private int _selectedRow;
		private bool _isRowExpanded;
		private DataViewModel _instanceDataView;
		private InstanceView _instanceView;
		private string _attachmentViewObjId;
		private int _pageIndex;
		private int _pageSize;
		private int _totalCount;
		private MenuItemStates _menuItemStates;
        private bool _goodForInlineEdit = false;


		/// <summary>
		/// Initializes a new instance of the DataViewSlide class.
		/// </summary>
		/// <param name="menuItemStates">The manager of menu item states</param>
		public DataViewSlide(MenuItemStates menuItemStates)
		{
			_menuItemStates = menuItemStates;
			_dataView = null;
			_dataSet = null;
			_rowView = null;
			_selectedRow = 0;
			_isRowExpanded = false;
			_instanceDataView = null;
			_instanceView = null;
			_attachmentViewObjId = null;
			_totalCount = -1; // indicate that total count is not available.
			_pageSize = DataViewModel.DEFAULT_PAGE_SIZE;
			PageIndex = 0; // Set page navigation buttion states initially
            _goodForInlineEdit = false;
		}

		/// <summary>
		/// Gets or sets the data view of the slide
		/// </summary>
		/// <value>The DataViewModel instance</value>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
			}
		}

		/// <summary>
		/// Gets or sets the data set of the slide
		/// </summary>
		/// <value>The DataSet instance</value>
		public DataSet DataSet
		{
			get
			{
				return _dataSet;
			}
			set
			{
				_dataSet = value;

				if (_rowView != null)
				{
					_rowView.DataSet = value;
				}
				
				int rowCount = RowCount;
				if (RowCount < PageSize)
				{
					// it is the last page
					_menuItemStates.SetState(MenuItemID.ViewNextPage, false);

					// now we know the total count of instance
					_totalCount = _pageIndex * _pageSize + rowCount;
				}
				else if (TotalInstanceCount < 0)
				{
					// we got full page and there is no instance count available
					// assuming there is more instances in the next page
					_menuItemStates.SetState(MenuItemID.ViewNextPage, true);
				}
			}
		}

		/// <summary>
		/// Gets the Row View of the slide
		/// </summary>
		/// <value>An InstanceView instance</value>
		public InstanceView RowView
		{
			get
			{
				if (_rowView == null)
				{
					_rowView = new InstanceView(_dataView, _dataSet);
					_rowView.ShowRelationshipAttributes = false;
				}

				return _rowView;
			}
		}

		/// <summary>
		/// Gets the Data View of the selected instance based on the instance's
		/// bottom class.
		/// </summary>
		/// <value>An DataViewModel</value>
		public DataViewModel InstanceDataView
		{
			get
			{
				return _instanceDataView;
			}
			set
			{
				_instanceDataView = value;
			}
		}

		/// <summary>
		/// Gets the InstanceView of the selected instance
		/// </summary>
		/// <value>An DataViewModel</value>
		public InstanceView InstanceView
		{
			get
			{
				return _instanceView;
			}
			set
			{
				_instanceView = value;
			}
		}

		/// <summary>
		/// Gets or sets the selected row in the data set
		/// </summary>
		/// <value>An integer represents the selected row</value>
		public int SelectedRowIndex
		{
			get
			{
				return _selectedRow;
			}
			set
			{
				_selectedRow = value;

				if (_rowView != null)
				{
					_rowView.SelectedIndex = value;
				}

				// set row navigation button states
				if (_selectedRow <= 0)
				{
					_menuItemStates.SetState(MenuItemID.ViewPrevRow, false);
				}
				else
				{
					_menuItemStates.SetState(MenuItemID.ViewPrevRow, true);
				}

				if (_selectedRow >= RowCount - 1)
				{
					_menuItemStates.SetState(MenuItemID.ViewNextRow, false);
				}
				else
				{
					_menuItemStates.SetState(MenuItemID.ViewNextRow, true);
				}
			}
		}

		/// <summary>
		/// Gets the obj_id of selected row in the data set
		/// </summary>
		/// <value>An string represents the obj_id of selected row</value>
		public string SelectedRowObjId
		{
			get
			{
				return DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName, NewteraNameSpace.OBJ_ID, _selectedRow);
			}
		}

		/// <summary>
		/// Gets the bottom class type of selected row in the data set
		/// </summary>
		/// <value>An string represents the bottom class of selected row</value>
		public string SelectedRowClassType
		{
			get
			{
				return DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName, NewteraNameSpace.TYPE, _selectedRow);
			}
		}

		/// <summary>
		/// Gets the number of attachments of selected row in the data set
		/// </summary>
		/// <value>An interger represents number of attachments</value>
		public int SelectedRowANUM
		{
			get
			{
				string anumStr = DataSetHelper.GetCellValue(_dataSet, _dataView.BaseClass.ClassName, NewteraNameSpace.ATTACHMENTS, _selectedRow);
				if (anumStr != null)
				{
					return System.Convert.ToInt32(anumStr);
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Gets the number of rows in the data slide
		/// </summary>
		/// <value>The number of rows</value>
		public int RowCount
		{
			get
			{
				return DataSetHelper.GetRowCount(_dataSet, _dataView.BaseClass.ClassName);
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the current selected row
		/// is expanded showing child relation
		/// </summary>
		/// <value>True if it is expanded, false otherwise, default is false.</value>
		public bool IsRowExpanded
		{
			get
			{
				return _isRowExpanded;
			}
			set
			{
				_isRowExpanded = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the data set contains no data
		/// </summary>
		/// <value>True if it is empty data set, false otherwise</value>
		public bool IsEmptyResult
		{
			get
			{
				return DataSetHelper.IsEmptyDataSet(_dataSet, _dataView.BaseClass.ClassName);
			}
		}

		/// <summary>
		/// Gets or sets the index of current page.
		/// </summary>
		/// <value>The page index</value>
		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			set
			{
				_pageIndex = value;
				if (_dataView != null)
				{
					_dataView.PageIndex = value;
				}

				// set page navigation button states
				if (_pageIndex <= 0)
				{
					_menuItemStates.SetState(MenuItemID.ViewPrevPage, false);
				}
				else
				{
					_menuItemStates.SetState(MenuItemID.ViewPrevPage, true);
				}

				int pageCount = DataViewModel.DEFAULT_PAGE_COUNT;
				if (TotalInstanceCount >= 0)
				{
					pageCount = TotalInstanceCount / _pageSize + 1;
				}
				
				if (_pageIndex >= pageCount - 1)
				{
					_menuItemStates.SetState(MenuItemID.ViewNextPage, false);
				}
				else
				{
					_menuItemStates.SetState(MenuItemID.ViewNextPage, true);
				}			
			}
		}

		/// <summary>
		/// Gets or sets the size of a page
		/// </summary>
		/// <value>Page size</value>
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
				if (_dataView != null)
				{
					_dataView.PageSize = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the total count of rows
		/// </summary>
		/// <value>Total count, -1 for unknown total count</value>
		public int TotalInstanceCount
		{
			get
			{
				return _totalCount;
			}
			set
			{
				_totalCount = value;
			}
		}

		/// <summary>
		/// Gets or sets the id of an instance whose attachments are currently showing
		/// in the attachment view.
		/// </summary>
		/// <value>The obj id</value>
		public string AttachmentViewObjId
		{
			get
			{
				return _attachmentViewObjId;
			}
			set
			{
				_attachmentViewObjId = value;
			}
		}

        /// <summary>
        /// Gets or sets info indicate whether the current slide is good for inline edit
        /// </summary>
        /// <value>true if it is good, false otherwise</value>
        public bool GoodForInlineEdit
        {
            get
            {
                return _goodForInlineEdit;
            }
            set
            {
                _goodForInlineEdit = value;
            }
        }
	}
}