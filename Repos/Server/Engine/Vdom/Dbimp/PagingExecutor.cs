/*
* @(#)PagingExecutor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Server.DB;

	/// <summary>
	/// Perform a search and obtain the result in paging mode.
	/// </summary>
	/// <version> 1.0.0 09 Jul 2006 </version>
	/// <author>  Yong Zhang </author>
	public class PagingExecutor : Executor, IDisposable
	{
		private VDocument _doc;
		private Hashtable _entityTable;
		private IDataReader _reader;
		private IDbConnection _connection;
		private int _pageSize;
        private bool _omitArrayData;
        private bool _delayCalculateVirtualValues;

        /// <summary>
        /// Initiating an instance of PagingExecutor class.
        /// </summary>
        /// <param name="metaData">the meta data model.</param>
        /// <param name="dataProvider">the database provider.</param>
        /// <param name="builder">the sql builder</param>
        /// <param name="entityTable">the hash table for associating xml elements with their entities.</param>
        public PagingExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Hashtable entityTable) : base(metaData, dataProvider, builder)
		{
			_doc = doc;
			_entityTable = entityTable;
			_reader = null;
			_connection = null;
			_pageSize = 50; // default
            _omitArrayData = false; // default
            _delayCalculateVirtualValues = true; // default
        }

		/// <summary>
		/// distructor in case the PagingExecutor is disposed by GC
		/// </summary>
		~PagingExecutor()
		{
			this.Close();
		}

		/// <summary>
		/// Gets or sets the page size
		/// </summary>
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicate whether to omit array data in search result.
        /// </summary>
        /// <value>True if omitting array data. false otherwise. default is false</value>
        public bool OmitArrayData
        {
            get
            {
                return _omitArrayData;
            }
            set
            {
                _omitArrayData = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to delay calculating values of virtual attributes
        /// </summary>
        /// <value>True to postpone the virtual value calculation at the select node stage. default is True</value>
        /// <remarks>Delay calculations to improve the query speed by avoiding calculating unneccessary virtual attributes</remarks>
        public bool DelayVirtualAttributeCalculation
        {
            get
            {
                return _delayCalculateVirtualValues;
            }
            set
            {
                _delayCalculateVirtualValues = value;
            }
        }

        /// <summary>
        /// Handles all the details of building SQL statements,
        /// executing the SQLs, convert the query result into xml elements, and
        /// add elements to the xml document.
        /// </summary>
        /// <param name="queryInfo">the information about a query</param>
        public void Execute(QueryInfo queryInfo)
		{
			CustomPrincipal principal = (CustomPrincipal) Thread.CurrentPrincipal;
				
			if (this._reader == null)
			{
				// it is the first call to this method, get the reader
				_reader = CreateDataReader(queryInfo, principal);
			}

			// convert result set into xml document
			int count = _builder.ConvertResultSet(queryInfo, _doc, _reader, _pageSize, _omitArrayData, _delayCalculateVirtualValues);

			// set the doc to principal if it is needed
			if (principal.NeedCurrentDocumentStatus)
			{
				principal.CurrentDocument = _doc;
			}
			
			if (count > 0 && queryInfo.QuestionableLeafClassIds.Count > 0)
			{
				// Personalize the result based on the principal's read permission
                _builder.PersonalizeResult(queryInfo, _doc, _entityTable, CheckReadPermissionOnly, ShowEncryptedData);
			}
		}

		/// <summary>
		/// Close the database connection and reader used by the executor
		/// </summary>
		public void Close()
		{
			if (_reader != null && !_reader.IsClosed)
			{
				_reader.Close();
				_reader = null;
			}

			if (_connection != null)
			{
				_connection.Close();
				_connection = null;
			}
		}

		/// <summary>
		/// Create a database reader first time the PagingExecutor is called
		/// </summary>
		private IDataReader CreateDataReader(QueryInfo queryInfo, CustomPrincipal principal)
		{
			IDbCommand cmd = null;

			// generate a SELECT SQL statement
			string sql;
			
			sql = _builder.GenerateSelect(queryInfo);
											
			if (principal.CurrentConnection != null)
			{
				// reusese the connection obtained from principal if it is available
				_connection = principal.CurrentConnection;
			}
			else
			{
				_connection  = _dataProvider.Connection;
			}

            SQLPrettyPrint.printSql(sql);

            cmd = _connection.CreateCommand();
			cmd.CommandText = sql;
			IDataReader dataReader = cmd.ExecuteReader();

			return dataReader;
		}

		#region IDisposable ≥…‘±

		/// <summary>
		/// Implementing IDisposal interface
		/// </summary>
		void IDisposable.Dispose() 
		{
			this.Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) 
		{
			this.Close();
		}

		#endregion
	}
}