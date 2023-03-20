/*
* @(#)IInstanceFormatter.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Xml;
	using System.Data;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Represents a interface for a formatter that convert an instance data
	/// to a different format
	/// </summary>
	/// <version> 1.0.0 22 Jan 2005</version>
	public interface IInstanceFormatter
	{
		/// <summary>
		/// Convert an instance data to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="instanceView">The InstanceView that stores data.</param>
		/// <param name="filePath">The file path</param>
		void Save(InstanceView instanceView, string filePath);

		/// <summary>
		/// Convert a DataTable to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable that stores data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		void Save(DataTable dataTable, string filePath, params object[] args);

		/// <summary>
		/// Convert two DataTable instances as comparison to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="beforeDataTable">The DataTable that stores before data.</param>
		/// <param name="afterDataTable">The DataTable that stores after data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		void Save(DataTable beforeDataTable, DataTable afterDataTable, string filePath, params object[] args);
	}
}