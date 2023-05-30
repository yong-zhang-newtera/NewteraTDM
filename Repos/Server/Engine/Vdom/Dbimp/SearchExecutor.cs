/*
* @(#)SearchExecutor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
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
    using Newtera.Common.MetaData.Logging;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Server.DB;
    using Newtera.Server.Logging;

	/// <summary>
	/// This class perform search-related actions to a database.
	/// </summary>
	/// <version>  	1.0.0 18 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class SearchExecutor : Executor
	{
		private VDocument _doc;
		private Hashtable _entityTable;
        private bool _omitArrayData;
        private bool _delayCalculateVirtualValues;

        /// <summary>
        /// Initiating an instance of SearchExecutor class.
        /// </summary>
        /// <param name="metaData">the meta data model.</param>
        /// <param name="dataProvider">the database provider.</param>
        /// <param name="builder">the sql builder</param>
        /// <param name="entityTable">the hash table for associating xml elements with their entities.</param>
        public SearchExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Hashtable entityTable) : base(metaData, dataProvider, builder)
		{
			_doc = doc;
			_entityTable = entityTable;
            _omitArrayData = false; // default
            _delayCalculateVirtualValues = true; // default
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
			IDbConnection con = null;
			IDbCommand cmd = null;
			IDataReader dataReader = null;
            bool needToCloseConnection = true;
            string sql = "";

			try
			{
				// generate a SELECT SQL statement
				if (!queryInfo.IsForFunction)
				{
                    if (!string.IsNullOrEmpty(this.CachedSQL))
                    {
                        // use the cached sql to get better performance
                        sql = this.CachedSQL;
                    }
                    else
                    {
                        sql = _builder.GenerateSelect(queryInfo);

                        this.CachedSQL = sql; // remember the sql in the cache by associating the xquery with the sql
                    }
				}
				else
				{
					// Build a SQL for an aggregate function
					SQLElement statement = _builder.GenerateFunctionSQL(queryInfo);
					sql = statement.ToSQL();
				}
				
				SQLPrettyPrint.printSql(sql);

                if (principal != null && principal.CurrentConnection != null)
				{
					// reusese the connection obtained from principal if it is available
					con = principal.CurrentConnection;
                    needToCloseConnection = false; // connection will be closed by the setter
				}
				else
				{
                    if (_doc.Interpreter.HasGlobalTransaction)
                    {
                        // global transaction
                        con = _doc.Interpreter.IDbConnection;
                    }
                    else
                    {
                        // local transaction
                        con = _dataProvider.Connection; 
                    }
				}
				
				cmd = con.CreateCommand();

                if (principal != null && principal.CurrentTransaction != null)
                {
                    cmd.Transaction = principal.CurrentTransaction;
                }
                else if (_doc.Interpreter.HasGlobalTransaction)
                {
                    cmd.Transaction = _doc.Interpreter.IDbTransaction;
                }

				cmd.CommandText = sql;

				dataReader = cmd.ExecuteReader();
				
				// convert result set into xml document
				_builder.ConvertResultSet(queryInfo, _doc, dataReader, _omitArrayData, _delayCalculateVirtualValues);

				// set the doc to principal if it is needed
                if (principal != null && principal.NeedCurrentDocumentStatus)
				{
					principal.CurrentDocument = _doc;
				}
				
				// Personalize the result based on the principal's read permission
				_builder.PersonalizeResult(queryInfo, _doc, _entityTable, CheckReadPermissionOnly, ShowEncryptedData);

                // log the event if the particular log is on 
                if (queryInfo.BaseClassElement != null)
                {
                    // disable the logging for read
                    /*
                    if (LoggingChecker.Instance.IsLoggingOn(_metaData, queryInfo.BaseClassElement, LoggingActionType.Read))
                    {
                        LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Read, _metaData.SchemaInfo.NameAndVersion,
                            queryInfo.BaseClassElement.Name, queryInfo.BaseClassElement.Caption, null, null);

                        LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                    }
                    */
                }
			}
			catch (Exception ex)
			{
                if (ex is System.Data.SqlClient.SqlException ||
                    ex is System.Data.OracleClient.OracleException ||
                    ex is MySqlConnector.MySqlException)
                {
                    if (!string.IsNullOrEmpty(sql))
                    {
                        throw new Exception(ex.Message + ";\n The SQL is " + sql + "\n" + ex.StackTrace);
                    }
                    else
                    {
                        throw new Exception(ex.Message + ";" + ex.StackTrace);
                    }
                }
                else
                {
                    throw ex;
                }
			}
			finally
			{
                try
                {
                    if (dataReader != null && !dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                    if (!_doc.Interpreter.HasGlobalTransaction &&
                        con != null &&
                        needToCloseConnection)
                    {
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine("SearchExecutor got an exception in finally block " + ex.Message + "\n" + ex.StackTrace);
                }
			}
		}

		/// <summary>
		/// For debugging
		/// </summary>
		/// <param name="doc"></param>
		private void PrintXml(XmlDocument doc)
		{
			XmlTextWriter writer = new XmlTextWriter(System.Console.Out);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;

			doc.WriteTo(writer);
		}
	}
}