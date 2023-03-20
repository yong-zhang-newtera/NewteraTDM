/*
* @(#)CMConnection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
    using System.Xml;
	using System.Threading;
	using System.Resources;
	using System.Security.Principal;
	using System.Text.RegularExpressions;
	using System.Collections;
	using System.ComponentModel;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.DataView;
	using Newtera.Server.Engine.Cache;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// Represents a connection to the Newtera Virtual XML data source.
	/// </summary>
	/// <version>  	1.0.0 26 Aug 2003 </version>
	public class CMConnection : ConnectionBase, IDbConnection
	{
        private const string TEMPLATE_XQUERY = "let $this := getCurrentInstance() return <flag>{if ($$) then 1 else 0}</flag>";

		private SchemaInfo _schemaInfo;
        private string _connectionString;
        private IDbConnection _con = null;
        private IDbTransaction _tran = null;

        #region common

        /// <summary> 
		/// Default constructor
		/// </summary>
		public CMConnection() : base()
		{
			_properties = null;
		}
		
		/// <summary>
		/// The constructor that takes a connection string
		/// </summary>
		/// <param name="connectionString">the connection string </param>
		/// <remarks>
		/// The following lists the valid names for keyword values within the ConnectionString
		/// for Newtera Catalog Data source.
		/// 
		/// Name:		SCHEMA_NAME
		/// Description: The name of a schema to be connected to. CM server manages multiple schemas
		/// The name of a schema is unique within a CM server
		/// Example:		PurchaseOrder
		/// 
		/// Name:		SCHEMA_VERSION
		/// Description: The version of a schema to be connected to. Each schema can have multiple
		/// versions.
		/// Example:		1.0
		/// 
		/// Name:		USER_ID
		/// Description: The CM Server login user id 
		/// Example:		john
		/// 
		/// Name:		PASSWORD
		/// Description: The CM Server login user password. it is optional
		/// Example:
        /// 
        /// Name:		TIMESTAMP
        /// Description: The timestamp assigned the schema when it was modified. it is optional
        /// Example:
		///  
		/// An example of a connection string looks like this:
		/// 
        /// "SCHEMA_NAME=PurchaseOrder;SCHEMA_VERSION=1.0;USER_ID=john;PASSWORD=smith;TIMESTAMP=2005-11-2 17:34:06"
		/// </remarks>
		/// <exception cref="CMException">An CMEception is thrown for invalid license.</exception>
		public CMConnection(string connectionString) : base()
		{
			_connectionString = connectionString;
			_properties = GetProperties(_connectionString);
            // validate the properties
            if (_properties[SCHEMA_NAME] == null)
            {
                throw new InvalidConnectionStringException("Missing property " + SCHEMA_NAME + " in connection string.");
            }

            if (_properties[SCHEMA_VERSION] == null)
            {
                throw new InvalidConnectionStringException("Missing property " + SCHEMA_VERSION + " in connection string.");
            }

			_schemaInfo = new SchemaInfo();
			_schemaInfo.Name = (string) _properties[SCHEMA_NAME];
			_schemaInfo.Version = (string) _properties[SCHEMA_VERSION];
            if (!string.IsNullOrEmpty((string) _properties[TIMESTAMP]))
            {
                _schemaInfo.ModifiedTime = DateTime.Parse((string)_properties[TIMESTAMP]);
            }
		}

		/// <summary> close the connection in case that the application forgets to do so.
		/// This method will be called by GC
		/// </summary>
		~CMConnection()
		{
            if (_con != null)
            {
                _con.Close();
                _con = null;
                _tran = null;
            }

			this.Close();
		}

		/// <summary>
		/// Gets or sets the string used to open a catalog data source.
		/// </summary>
		/// <value> the string used to open a data source</value>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
				_properties = GetProperties(value);
                // validate the properties
                if (_properties[SCHEMA_NAME] == null)
                {
                    throw new InvalidConnectionStringException("Missing property " + SCHEMA_NAME + " in connection string.");
                }

                if (_properties[SCHEMA_VERSION] == null)
                {
                    throw new InvalidConnectionStringException("Missing property " + SCHEMA_VERSION + " in connection string.");
                }
				_schemaInfo = new SchemaInfo();
				_schemaInfo.Name = (string) _properties[SCHEMA_NAME];
				_schemaInfo.Version = (string) _properties[SCHEMA_VERSION];
                if (!string.IsNullOrEmpty((string)_properties[TIMESTAMP]))
                {
                    _schemaInfo.ModifiedTime = DateTime.Parse((string)_properties[TIMESTAMP]);
                }
			}
		}

		/// <summary>
		/// Gets the time to wait while trying to establish a connection before terminating
		/// the attempt and generating an error.
		/// </summary>
		/// <value> the time (in seconds) to wait for a connection to open.
		/// The default value is 0 seconds, indicates an indefinite time-out period.
		/// </value>
		public int ConnectionTimeout
		{
			get
			{
				/*
				* Returns the connection time-out value set in the connection
				* string. Zero indicates an indefinite time-out period.
				*/
				return 0;
			}
        }

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
            /*
             * Dispose of the object and perform any cleanup.
             */
            if (_con != null)
            {
                _con.Close();
                _con = null;
                _tran = null;
            }
            this.Close();
        }

        #endregion Common

        #region Database

        /// <summary>
		/// Gets the name of the current schema and version to be used after a connection
		/// is opened.
		/// </summary>
		/// <value> the name and version of the schema </value>
		public string Database
		{
			get
			{
				return SCHEMA_NAME + "=" + _properties[SCHEMA_NAME] + ";" + SCHEMA_VERSION + "=" + _properties[SCHEMA_VERSION];
			}
		}

		/// <summary>
		/// Get the schema info for the connection
		/// </summary>
		/// <value>The schema info, default is null.</value>
		public SchemaInfo SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
		}

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns> An object representing the new transaction.</returns>
        /// <remarks>Once the transaction has completed, you must explicitly commit or roll back the transaction
        /// by using the Commit or Rollback methods.</remarks>
        public IDbTransaction BeginTransaction()
        {
            _con = _dataProvider.Connection;
            _tran = _con.BeginTransaction();

            return _tran;
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns> An object representing the new transaction.</returns>
        /// <remarks>Once the transaction has completed, you must explicitly commit or roll back the transaction by using the Commit or Rollback methods.</remarks>
        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            _con = _dataProvider.Connection;
            _tran = _con.BeginTransaction(level);

            return _tran;
        }

        /// <summary>
        /// Changes the current database for an open Connection object.
        /// </summary>
        /// <param name="databaseName">database string</param>
        /// <remarks>Not supported</remarks>
        public void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException("Changing database is not supported");
        }

        /// <summary>
        /// Creates and returns a CMCommand object associated with the connection.
        /// </summary>
        /// <returns> CMCommand object </returns>
        public CMCommand CreateCommand()
        {
            return new CMCommand(null, this);
        }

        /// <summary>
        /// Creates and returns a IDbCommand object associated with the connection.
        /// </summary>
        /// <returns> CMCommand object </returns>
        IDbCommand IDbConnection.CreateCommand()
        {
            return (IDbCommand)new CMCommand(null, this);
        }

        /// <summary>
        /// Gets or sets the MetaDataModel instance of the connection
        /// </summary>
        /// <value>
        /// A MetaDataModel instance
        /// </value>
        public MetaDataModel MetaDataModel
        {
            get
            {
                if (this.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must valid and open");
                }

                return MetaDataCache.Instance.GetMetaData(_schemaInfo, _dataProvider);
            }
        }

        /// <summary>
        /// Gets the information indicating whether there is a global transaction
        /// </summary>
        public bool HasGlobalTransaction
        {
            get
            {
                if (_tran != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the meta data model referred by the
        /// connection has existed
        /// </summary>
        /// <value>
        /// true if it exists, false otherwise.
        /// </value>
        public bool IsMetaDataModelExist
        {
            get
            {
                if (this.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must valid and open");
                }

                return MetaDataCache.Instance.IsSchemaExisted(_schemaInfo, _dataProvider);
            }
        }

        /// <summary>
        /// Gets the information indicating whether the meta data model referred by the
        /// connection has been modified. This method works if the connection string contains
        /// timestamp of the meta-data model stored on the client side.
        /// </summary>
        /// <value>
        /// true if the server-side meta-data model is modified, false otherwise.
        /// </value>
        public bool IsMetaDataModelModified
        {
            get
            {
                if (this.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must valid and open");
                }

                return MetaDataCache.Instance.IsSchemaModified(_schemaInfo, _dataProvider);
            }
        }

        /// <summary>
        /// Gets the log of the latest updates to the meta data
        /// </summary>
        public string MetaDataUpdateLog
        {
            get
            {
                return MetaDataCache.Instance.GetMetaDataUpdateLog(_schemaInfo, _dataProvider);
            }
        }

        /// <summary>
        /// Update a meta data of given type using a xml string.
        /// It could be a schema, xacl policy, or other meta data
        /// </summary>
        /// <param name="type">The meta data model type</param>
        /// <param name="xmlString">The xml string representing an updated metadata</param>
        /// <returns>The time when the meta-data model is modified. </returns>
        public DateTime UpdateMetaData(MetaDataType type, string xmlString)
        {
            return MetaDataCache.Instance.UpdateMetaData(type, xmlString, _schemaInfo, _dataProvider, IsSafeMode);
        }

        /// <summary>
        /// Delete the meta data stored in the database permanently.
        /// </summary>
        /// <remarks>
        /// Care must be taken when calling this method which will cause loss of
        /// the physical tables and data in the database permanently.
        /// </remarks>
        public void DeleteMetaData()
        {
            MetaDataCache.Instance.DeleteMetaData(_schemaInfo, _dataProvider);
        }

        /// <summary>
        /// Fix the discrepancies bewtween a schema model and its corresponding databases
        /// that may occure due to various reasons.
        /// </summary>
        public void FixSchemaModel()
        {
            MetaDataCache.Instance.FixSchemaModel(_schemaInfo, _dataProvider);
        }

        /// <summary>
        /// Lock the meta data for update
        /// </summary>
        /// <exception cref="LockMetaDataException">Thrown when the meta data has been locked by another user.</exception>
        public void LockMetaData()
        {
            MetaDataCache.Instance.LockMetaData(_schemaInfo, _dataProvider);
        }

        /// <summary>
        /// Unlock the meta data
        /// </summary>
        /// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
        /// <exception cref="LockMetaDataException">Thrown when the user doesn't have a right to unlock the meta-data.</exception>
        public void UnlockMetaData(bool forceUnlock)
        {
            MetaDataCache.Instance.UnlockMetaData(_schemaInfo, _dataProvider, forceUnlock);
        }

        /// <summary>
        /// Gets the name of role that has permission to modify the meta data
        /// </summary>
        /// <returns>The name of role, null for non-protected mode.</returns>
        public string GetDBARole()
        {
            return MetaDataCache.Instance.GetDBARole(_schemaInfo, _dataProvider);
        }

        /// <summary>
        /// Sets the name of role that has permission to modify the meta data
        /// </summary>
        /// <param name="role">The name of role, null to set non-protected mode.</param>
        public void SetDBARole(string role)
        {
            MetaDataCache.Instance.SetDBARole(_schemaInfo, _dataProvider, role);
        }

        /// <summary>
        /// Validate the xquery condition.
        /// </summary>
        /// <param name="conditionExpr">The condition expression</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <returns>Error message if the condition is invalid, null if it is valid.</returns>
        public string ValidateXQueryCondition(string conditionExpr, string instanceClassName)
        {
            string msg = null;

            CustomPrincipal principal = (CustomPrincipal)Thread.CurrentPrincipal;
            XmlElement originalInstance = principal.CurrentInstance;
            try
            {
                if (principal != null)
                {
                    MetaDataModel metaData = this.MetaDataModel;
                    DataViewModel dataView = metaData.GetDetailedDataView(instanceClassName);

                    if (dataView != null)
                    {
                        string query = dataView.SearchQuery;

                        Interpreter interpreter = new Interpreter();
                        XmlDocument doc = interpreter.Query(query);

                        if (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
                        {
                            principal.CurrentInstance = (XmlElement)doc.DocumentElement.ChildNodes[0]; // use the first element to evaluate the expression

                            // build a complete xquery
                            string finalCondition = TEMPLATE_XQUERY.Replace("$$", conditionExpr);

                            // execute the xquery using interpreter
                            doc = interpreter.Query(finalCondition);

                            // if the expression is valid, no exception will be thrown
                        }
                    }
                }
            }
            catch (UnknownSchemaException)
            {
                msg = null; // when database doesn't exist. this method return no error
            }
            catch (Exception ex)
            {
                // the expression is invalid, return the error message
                msg = ex.Message;
            }
            finally
            {
                // unset the current instance as a context for condition evaluation
                principal.CurrentInstance = originalInstance;
            }

            return msg;
        }

        public IDbConnection IDbConnection
        {
            get
            {
                return _con;
            }
        }

        internal IDbTransaction IDbTransaction
        {
            get
            {
                return _tran;
            }
        }

        #endregion Database
	}
}