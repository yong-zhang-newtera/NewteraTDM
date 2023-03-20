/*
* @(#)IDataGridControl.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Data;
	using System.Xml;
	using System.Windows.Forms;

	using Newtera.DataGridActiveX.ActiveXControlWebService;

	/// <summary>
	/// Represents a common interface for the controls that contains data for charting.
	/// </summary>
	/// <version> 1.0.0 15 May 2006</version>
	public interface IDataGridControl
	{
        /// <summary>
        /// Gets the base class name of data instances currently displayed in the datagrid 
        /// </summary>
        string BaseClassName { get;}

		/// <summary>
		/// Gets a collection of ColumnInfo objects that describe the columns in the datagrid
		/// </summary>
		/// <value>A ColumnInfoCollection</value>
		ColumnInfoCollection ColumnInfos {get;}

		/// <summary>
		/// Gets the DataView that is in the same order as what displayed on the datagrid
		/// </summary>
		DataView DataView {get;}

        /// <summary>
        /// Gets the name of data table currently used by the DataGridControl
		/// </summary>
        string TableName { get;}

		/// <summary>
		/// Get the DataGrid control
		/// </summary>
		DataGrid TheDataGrid { get; }

		/// <summary>
		/// Fire the graph event
		/// </summary>
		void FireGraphEvent();

		/// <summary>
		/// Fire the download graph event
		/// </summary>
		/// <param name="formatName">Format Name</param>
		void FireDownloadGraphEvent(string formatName, string fileSuffix);

		/// <summary>
		/// Create a web service proxy for chart related services
		/// </summary>
		/// <returns></returns>
		ActiveXControlService CreateActiveXControlWebService();
	}
}