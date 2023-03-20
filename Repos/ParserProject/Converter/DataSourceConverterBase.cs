/*
* @(#)DataSourceConverterBase.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.Converter
{
	using System;
	using System.Xml;
	using System.Data;

	/// <summary> 
	/// The base class that implements common properties and methods for all
	/// data source converters.
	/// </summary>
	/// <version> 1.0.0 07 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public abstract class DataSourceConverterBase : IDataSourceConverter
	{
		private string _name;
	
		/// <summary>
		/// Initiate an instance of DataSourceConverterBase class
		/// </summary>
		public DataSourceConverterBase()
		{
			_name = null;
		}

		/// <summary>
		/// Initiate an instance of DataSourceConverterBase class
		/// </summary>
		/// <param name="name">The converters name</param>
		public DataSourceConverterBase(string name)
		{
			_name = name;
		}

		#region IDataSourceParser interface implementation
		
		/// <summary>
		/// Gets or sets the name of converter
		/// </summary>
		/// <value>A string representing an unique name of the converter.</value>
		public string Name 
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the converter supports reading data in
		/// pages
		/// </summary>
		/// <value>true if it supports paging, false otherwise. Default is false.</value>
		public virtual bool SupportPaging
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Convert data of a data source into a DataSet instance. This method must be
		/// overrided by the subclasses.
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies a data source</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public abstract DataSet Convert(string dataSourceName);

		/// <summary>
		/// Used in converting data in a large file, convert the first page of data into a DataSet instance.
		/// To be called first.
		/// </summary>
		/// <param name="dataSourceName">The name of the data source file.</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data, or NULL it reaches the end of file.</returns>
		public virtual DataSet ConvertFirstPage(string dataSourceName, int pageSize)
		{
			throw new Exception("This converter doesn't support paging mode.");
		}

		/// <summary>
		/// Used in converting data in a large file, convert data of a next page
		/// to a DataSet instance. To be called subsequently after ConvertFirstPage is called.
		/// </summary>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data, or NULL it reaches the end of file.</returns>
		/// <exception cref="Exception">Thrown if the ConvertFirstPage isn't called.</exception>
		public virtual DataSet ConvertNextPage()
		{
			throw new Exception("This converter doesn't support paging mode.");
		}

		/// <summary>
		/// Used in converting data in a large file, close the file opened at ConvertFirstPage
		/// method call.
		/// </summary>
		public virtual void Close()
		{
			// do nothing by default
		}

		#endregion

		/// <summary>
		/// Get the text file name from the path
		/// </summary>
		/// <param name="path">The file path</param>
		/// <returns>The file name with suffix</returns>
		protected string GetFileName(string path)
		{
			int pos = path.LastIndexOf(@"\");
			if (pos > 0 && (pos + 1) < path.Length)
			{
				return path.Substring(pos + 1);
			}
			else
			{
				return path;
			}
		}
	}
}