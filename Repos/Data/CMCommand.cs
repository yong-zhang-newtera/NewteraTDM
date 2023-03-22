/*
* @(#)CMCommand.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Collections.Specialized;
	using System.IO;
	using System.Data;
	using System.Threading;
    using System.Collections;
    using System.Collections.Generic;
	using Microsoft.Win32;
    using System.Threading.Tasks;

    using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.DB.MetaData;
	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Vdom.Dbimp;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Cache;
    using Newtera.Server.Engine.XmlGenerator;
    using Newtera.Server.Engine.Workflow;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Mappings.Scripts;
	using Newtera.Common.Attachment;
	using Newtera.Server.Attachment;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.XMLSchemaView;
    using Newtera.Common.MetaData.Logging;
    using Newtera.Common.Wrapper;
    using Newtera.Server.Logging;
    using Newtera.Server.FullText;

	/// <summary> 
	/// Represents a query or command that is used when connected to a data source.
	/// </summary>
	/// <version>  	1.0.0 26 Aug 2003 </version>
	/// <author>  		Yong Zhang </author>
	public sealed class CMCommand : IDbCommand
	{
		/* private static variables*/	
		private const int DEFAULT_PAGE_SIZE = 50;

        private const int PAGE_SIZE = 1000;

		private string _cmdText = null;
		private int _cmdTimeout = 60;
		private CommandType _cmdType;
		private CMConnection _connection = null;
		private CMParameterCollection _parameters;
		private Interpreter _interpreter;
		private UpdateRowSource _updatedRowSource = UpdateRowSource.None;
		private int _pageSize = DEFAULT_PAGE_SIZE;
        private bool _omitArrayData = false;
        private XmlDocGenerator _generator = null;
        private List<InstanceView> _clonedInstanceViews = null;
		
		/// <summary>
		/// Initiate an instance of CMCommand class
		/// </summary>
		public CMCommand()
		{
			_cmdType = CommandType.Text;
			_parameters = new CMParameterCollection();
			_interpreter = new Interpreter();
		}
		
		/// <summary>
		/// Initiate an instance of CMCommand class
		/// </summary>
		/// <param name="cmdText">A command text can be
		/// either an XQuery or a path to a stored query.
		/// </param>
		public CMCommand(string cmdText)
		{
			_cmdType = CommandType.Text;
			_parameters = new CMParameterCollection();
			_interpreter = new Interpreter();
			_cmdText = cmdText;
		}
		
		/// <summary> 
		/// Initiate an instance of CMCommand class
		/// </summary>
		/// <param name="cmdText">A command text can be
		/// either an XQuery or a path to a stored query.</param>
		/// <param name="connection">The connection to a catalog</param>
		public CMCommand(string cmdText, CMConnection connection)
		{
			_cmdType = CommandType.Text;
			_parameters = new CMParameterCollection();
			_interpreter = new Interpreter();
			_cmdText = cmdText;
			_connection = connection;
		}

		/// <summary>
		/// Gets or sets the query to run against the catalog.
		/// </summary>
		/// <value> the command text</value>
		/// <remarks>
		/// When the CommandType property is set to StoredProcedure, set the CommandText property
		/// to the path of the stored query. The command will call this stored query when you
		/// call one of the Execute methods.
		/// </remarks>
		public string CommandText
		{
			get
			{
				return _cmdText;
			}
			set
			{
				_cmdText = value;
			}
		}

		/// <summary>
		/// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
		/// </summary>
		/// <value> The time (in seconds) to wait for the command to execute. The default value is 60 seconds.</value>
		public int CommandTimeout
		{
			get
			{
				return _cmdTimeout;
			}
			set
			{
				_cmdTimeout = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating how the CommandText property is to be interpreted.
		/// </summary>
		/// <value> One of the CommandType values. The default is Text.</value>
		/// <remarks>
		/// When you set the CommandType property to StoredProcedure, you should set the
		/// CommandText property to the path of the stored query. The command
		/// executes this stored query when you call one of the Execute methods.
		/// </remarks>
		public CommandType CommandType
		{
			get
			{
				return _cmdType;
			}
			set
			{
				_cmdType = value;
			}
		}

		/// <summary>
		/// Gets or sets the CMConnection used by this instance of the CMCommand.
		/// </summary>
		/// <value> The connection to a data source.
		///  The default value is a null reference.
		/// </value>
		public CMConnection Connection
		{
			get
			{
				return _connection;
			}
			
			set
			{
				_connection = value;
			}
		}

		/// <summary>
		/// Gets or sets the size of each result page.
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
        /// Gets or sets the information indicate whether to omit array data in return result.
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
        /// Gets or sets the information indicate whether to raise defined events for
        /// update query, such as Insert, Update, and Delete queries.
        /// </summary>
        /// <value>True if to raise evnets. false otherwise. default is true</value>
        public bool NeedToRaiseEvents
        {
            get
            {
                return _interpreter.NeedToRaiseEvents;
            }
            set
            {
                _interpreter.NeedToRaiseEvents = value;
            }
        }

		/// <summary>
		/// Gets or sets the IDbConnection used by this instance of the CMCommand.
		/// </summary>
		/// <value> The connection to a data source.
		///  The default value is a null reference.
		/// </value>
		IDbConnection IDbCommand.Connection
		{
			get
			{
				return _connection.IDbConnection;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the CMParameterCollection of the command.
		/// </summary>
		/// <value> The parameters of the command</value>
		public CMParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Gets the IDataParameterCollection of the command.
		/// </summary>
		/// <value> The parameters of the command</value>
		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Gets or sets the IDbTransaction within which the IDbCommand executes.
		/// </summary>
		/// <value>The IDbTransaction. The default value is a null reference </value>
		public IDbTransaction Transaction
		{
			get
			{
				return _connection.IDbTransaction;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the stored query text.
		/// </summary>
		/// <value> An XQuery string, null if the query does not exist.
		/// </value>
		/// <exception cref="InvalidOperationException"> thrown when CommandType is not StoredProcedure, or
		/// connection does not exists.
		/// </exception>
		public string StoredQuery
		{
			get
			{
				string query = "";
					
				if (_cmdType != CommandType.StoredProcedure)
				{
					throw new InvalidOperationException("Command Type is not StoredProcedure");
				}
					
				if (_connection == null)
				{
					throw new InvalidOperationException("No connection available");
				}

				// TODO
								
				return query;
			}
		}
		
		/// <summary>
		/// Gets or sets how command results are applied to the DataRow when used by the Update method of a DbDataAdapter.
		/// </summary>
		/// <value>
		/// One of the UpdateRowSource values, the default is None.
		/// </value>
		public UpdateRowSource UpdatedRowSource
		{
			get 
			{
				return _updatedRowSource; 
			}
			set 
			{ 
				_updatedRowSource = value;
			}
		}

        /// <summary>
        /// Gets or sets an exta condition to be added to the query executed by the command
        /// </summary>
        /// <value>The IDbTransaction. The default value is a null reference </value>
        public string ExtraCondition
        {
            get
            {
                if (_interpreter != null)
                {
                    return _interpreter.ExtraCondition;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (_interpreter != null)
                {
                    _interpreter.ExtraCondition = value;
                }
            }
        }

		/// <summary> 
		/// Attempts to cancels the execution of a CMCommand.
		/// Not supported for now.
		/// </summary>
		public void Cancel()
		{
			// It does not support canceling a command currently
			throw new NotSupportedException("Cancel a command is not supported");
		}
		
		/// <summary>
		/// Creates a new instance of an CMDataParameter object.
		/// </summary>
		/// <returns> CMParameter object
		/// 
		/// </returns>
		public CMParameter CreateParameter()
		{
			return new CMParameter();
		}

		/// <summary>
		/// Creates a new instance of an IDbDataParameter object.
		/// </summary>
		/// <returns> IDbDataParameter object </returns>
		IDbDataParameter IDbCommand.CreateParameter()
		{
			return (IDbDataParameter) new CMParameter();
		}
		
		/// <summary>
		/// Executes an query statement against the Connection object,
		/// and returns the number of rows affected. For UPDATE, INSERT, and DELETE queries,
		/// the return value is the number of rows affected by the command. For all other
		/// types of xqueries, the return value is -1.
		/// </summary>
		/// <exception cref="InvalidOperationException">An exception occurred when the connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> The number of rows affected.</returns>
		public int ExecuteNonQuery()
		{			
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
			
			// Get the xquery based on cmd type and substitute parameters with values
			string executableQuery = GetExecutableQuery();

            if (_connection.HasGlobalTransaction)
            {
                _interpreter.IDbConnection = _connection.IDbConnection;
                _interpreter.IDbTransaction = _connection.IDbTransaction;
            }

			XmlDocument doc = _interpreter.Query(executableQuery);
			
			// The firt child element contains result
			return int.Parse(doc.DocumentElement.FirstChild.InnerText);
		}
		
		/// <summary>
		/// Executes the CommandText against the Connection and builds a IDataReader.
		/// </summary>
		/// <exception cref="Exception">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> A IDataReader object.</returns>
		IDataReader IDbCommand.ExecuteReader()
		{
			throw new NotSupportedException("Not supported method.");
		}

		/// <summary>
		/// Executes the CommandText against the Connection and builds an CMDataReader
		/// using one of the CommandBehavior values.
		/// </summary>
		/// <param name="behavior">One of the CommandBehavior values.</param>
		/// <exception cref="NotSupportedException"> This method is currently not supported</exception>
		/// <returns> CMDataReader object.</returns>
		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			throw new NotSupportedException("Not supported method.");
		}

		/// <summary>
		/// Executes the CommandText against the Connection and builds a CMDataReader.
		/// </summary>
		/// <exception cref="Exception">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> A CMDataReader object.</returns>
		public CMDataReader ExecuteReader()
		{
			return ExecuteReader(CommandBehavior.Default);
		}
		
		/// <summary>
		/// Executes the CommandText against the Connection and builds an CMDataReader
		/// using one of the CommandBehavior values.
		/// </summary>
		/// <param name="behavior">One of the CommandBehavior values.</param>
		/// <exception cref="NotSupportedException"> This method is currently not supported</exception>
		/// <returns> CMDataReader object.</returns>
		public CMDataReader ExecuteReader(CommandBehavior behavior)
		{
			CMDataReader dataReader;
			
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
	
			// Get the xquery based on cmd type and substitute parameters with values
			string executableQuery = GetExecutableQuery();
			
			// tell the interpreter that we want a reader that gets the query result in paging mode.
			_interpreter.IsPaging = true;
			_interpreter.PageSize = _pageSize;
            _interpreter.OmitArrayData = _omitArrayData;
			QueryReader reader = _interpreter.GetQueryReader(executableQuery);

			if (behavior == CommandBehavior.CloseConnection)
			{
				dataReader = new CMDataReader(reader, _connection, true);
			}
			else
			{
				dataReader = new CMDataReader(reader, _connection);
			}
			
			return dataReader;
		}
		
		/// <summary>
		/// Executes the query, and returns the first column of the first row in the
		/// resultset returned by the query. Extra columns or rows are ignored.
		/// Use the ExecuteScalar method to retrieve a single value (for example, an
		/// aggregate value) from a catalog.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> The first column of the first row in the result.
		/// </returns>
		public object ExecuteScalar()
		{			
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			// Get the xquery based on cmd type and substitute parameters with values
			string executableQuery = GetExecutableQuery();

            // pass the global  IDbConnection and IDbTransaction objects to the interpreter
            if (_connection.HasGlobalTransaction)
            {
                _interpreter.IDbConnection = _connection.IDbConnection;
                _interpreter.IDbTransaction = _connection.IDbTransaction;
            }
			
			XmlDocument doc = _interpreter.Query(executableQuery);
			
			// The firt child element contains result
			return double.Parse(doc.DocumentElement.FirstChild.InnerText);
		}
		
		/// <summary>
		/// Sends the CommandText to the Connection and builds an XmlReader object.
		/// </summary>
		/// <exception cref="InvalidOperationException">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> An XmlReader object.</returns>
		public XmlReader ExecuteXMLReader()
		{
            return ExecuteXMLReader(CMCommandBehavior.Default);
		}

        /// <summary>
        /// Sends the CommandText to the Connection and builds an XmlReader object.
        /// </summary>
        /// <param name="behavior">One of CMCommandBehavior enum values</param>
        /// <exception cref="InvalidOperationException">The connection does not exist or
        /// The connection is not open.
        /// </exception>
        /// <returns> An XmlReader object.</returns>
        public XmlReader ExecuteXMLReader(CMCommandBehavior behavior)
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            // Get the xquery based on cmd type and substitute parameters with values
            string executableQuery = GetExecutableQuery();

            if (behavior == CMCommandBehavior.CheckReadPermissionOnly)
            {
                _interpreter.CheckReadPermissionOnly = true;
            }
            else
            {
                _interpreter.CheckReadPermissionOnly = false;
            }

            // pass the global  IDbConnection and IDbTransaction objects to the interpreter
            if (_connection.HasGlobalTransaction)
            {
                _interpreter.IDbConnection = _connection.IDbConnection;
                _interpreter.IDbTransaction = _connection.IDbTransaction;
            }

            XmlDocument doc;

            doc = _interpreter.Query(executableQuery);

            //PrintXml(doc);

            return new XmlNodeReader(doc);
        }

        /// <summary>
		/// Sends the CommandText to the Connection and builds an XmlDocument object.
		/// </summary>
		/// <exception cref="InvalidOperationException">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> An XmlDocument object.</returns>
        public XmlDocument ExecuteXMLDoc()
        {
            return ExecuteXMLDoc(CMCommandBehavior.Default);
        }

		/// <summary>
		/// Sends the CommandText to the Connection and builds an XmlDocument object.
		/// </summary>
        /// <param name="behavior">One of CMCommandBehavior enum values</param>
		/// <exception cref="InvalidOperationException">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> An XmlDocument object.</returns>
        public XmlDocument ExecuteXMLDoc(CMCommandBehavior behavior)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			// Get the xquery based on cmd type and substitute parameters with values
			string executableQuery = GetExecutableQuery();

            if (behavior == CMCommandBehavior.ObtainCachedObjId)
            {
                _interpreter.ObtainCachedObjId = true;
            }
            else
            {
                _interpreter.ObtainCachedObjId = false;
            }

            // pass the global  IDbConnection and IDbTransaction objects to the interpreter
            if (_connection.HasGlobalTransaction)
            {
                _interpreter.IDbConnection = _connection.IDbConnection;
                _interpreter.IDbTransaction = _connection.IDbTransaction;
            }
			
			XmlDocument doc = _interpreter.Query(executableQuery);
			
			//PrintXml(doc);

			return doc;
		}

		/// <summary>
		/// Sends the CommandText to the Connection and return the count of query
		/// specified in CommandText.
		/// </summary>
		/// <exception cref="InvalidOperationException">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> An Integer representing acount.</returns>
		/// <remarks>This is a CMCommand extension method, not available in IDbCommand
		/// interface. Note that, this method may return an invalid count result if
		/// the query involves multiple joins.</remarks>
		public int ExecuteCount()
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			// Get the xquery based on cmd type and substitute parameters with values
			string executableQuery = GetExecutableQuery();
			
			XmlDocument doc = _interpreter.Count(executableQuery);
			
			// The content of document root is the count result
			string countStr = doc.DocumentElement.InnerText;

			return System.Convert.ToInt32(countStr);
		}

        /// <summary>
        /// Update data instances whose attribute values have been changed indicated in
        /// the data set
        /// </summary>
        /// <exception cref="InvalidOperationException">The connection does not exist or
        /// The connection is not open.
        /// </exception>
        /// <returns> Any error message.</returns>
        /// <remarks>This is a CMCommand extension method, not available in IDbCommand
        /// interface.</remarks>
        public string BatchUpdate(string className, DataSet ds)
        {
            string msg = null;
            string query;

            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            DataViewModel instanceDataView = _connection.MetaDataModel.GetDetailedDataView(className);
            instanceDataView.VerifyChanges = false; // so that it will generate a full update query

            InstanceView instanceView = new InstanceView(instanceDataView, ds);
            int count = ds.Tables[className].Rows.Count; // number of instances to update
            for (int i = 0; i < count; i++)
            {
                try
                {
                    instanceView.SelectedIndex = i;

                    query = instanceView.DataView.UpdateQuery;

                    _interpreter.Reset(); // so that it can be reused

                    _interpreter.Query(query);
                }
                catch (PermissionViolationException)
                {
                    msg = "Failed to update some of instances due to permission violation";
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            
            return msg;
        }

        /// <summary>
        /// Sends the CommandText to the Connection and return a collection of names of leaf classes
        /// that contains at least one instance in the search result set.
        /// </summary>
        /// <exception cref="InvalidOperationException">The connection does not exist or
        /// The connection is not open.
        /// </exception>
        /// <returns> An StringCollection containing names of leaf classes.</returns>
        /// <remarks>This is a CMCommand extension method, not available in IDbCommand
        /// interface. </remarks>
        public StringCollection GetClassNames()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            StringCollection leafClasses = new StringCollection();

            // Get the xquery based on cmd type and substitute parameters with values
            string executableQuery = GetExecutableQuery();

            XmlDocument doc = _interpreter.GetClassNames(executableQuery);

            // the result has been saved in interpreter, so we do not need to get them from
            // XmlDocument
            return _interpreter.ClassNames;
        }

		/// <summary>
		/// Get the number of instances in a class.
		/// </summary>
		/// <param name="className">The class name.</param>
		/// <exception cref="InvalidOperationException">The connection does not exist or
		/// The connection is not open.
		/// </exception>
		/// <returns> An Integer representing number of instances in a class.</returns>
		/// <remarks>This is a CMCommand extension method, not available in IDbCommand
		/// interface.</remarks>
		public int GetInstanceCount(string className)
		{
			int count = 0;

			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(className);
			if (classElement == null)
			{
				throw new InvalidOperationException("The class " + className + " does not exist.");
			}

			IDbConnection dbCon = null;

			try
			{
				dbCon = _connection.DataProvider.Connection;

				IDbCommand cmd = dbCon.CreateCommand();

				cmd.CommandText = "select count(" + SQLElement.COLUMN_OBJ_ID + ") from " + classElement.TableName;

				count = System.Convert.ToInt32(cmd.ExecuteScalar().ToString());
			}
			catch(Exception)
			{
				count = 0;
			}
			finally
			{
				if (dbCon != null)
				{
					dbCon.Close();
				}
			}

			return count;
		}

		/// <summary>
		/// Add an attachment info for an attachment that is attached to a specified item
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The attachment information that specifies the attachment.</param>
		/// <returns>An unique id for the attachment.</returns>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public string AddAttachmentInfo(AttachmentType attachmentType, AttachmentInfo attachInfo)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			return repository.AddAttachmentInfo(attachmentType, attachInfo, _connection.SchemaInfo, _connection.MetaDataModel.SchemaModel.SchemaInfo.ID);
		}

		/// <summary>
		/// Set an attachment of a given id
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachmentId">The attachment id</param>
		/// <param name="stream">The stream from which to read data</param>
		public void SetAttachment(AttachmentType attachmentType, string attachmentId, Stream stream)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			// code that write the attachment to the database
			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			repository.SetAttachment(attachmentType, attachmentId, stream);
		}

		/// <summary>
		/// Add an attachment info for an attachment that is attached to a specified instance
		/// </summary>
		/// <param name="attachInfo">The attachment information that specifies the attachment.</param>
		/// <param name="stream">The stream from which to get attachment data.</param>
		/// <returns>An unique id for the attachment.</returns>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public string AddAttachmentInfo(AttachmentInfo attachInfo, Stream stream)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
		
			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			return repository.AddAttachmentInfo(AttachmentType.Instance, attachInfo, _connection.SchemaInfo, _connection.MetaDataModel.SchemaModel.SchemaInfo.ID);
		}

		/// <summary>
		/// Delete an attachment from a specified item
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be deleted.</param>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public void DeleteAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}

			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			repository.DeleteAttachment(attachmentType, attachInfo, _connection.SchemaInfo);

            // log the event if the particular log is on
            if (LoggingChecker.Instance.IsLoggingOn(_connection.MetaDataModel, attachInfo.ClassName, LoggingActionType.Delete))
            {
                ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(attachInfo.ClassName);
                string classCaption = (classElement != null ? classElement.Caption : null);
                LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Delete, _connection.MetaDataModel.SchemaInfo.NameAndVersion,
                    attachInfo.ClassName, classCaption, attachInfo.Name);

                LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
            }
		}

		/// <summary>
		/// Get an attachment of a specified item as a stream.
		/// </summary>
		/// <param name="attachmentType">One of AttachmentType enum value.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public Stream GetAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}

			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			return repository.GetAttachment(attachmentType, attachInfo);
		}

		/// <summary>
		/// Gets information of a specified attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of the attaching item.</param>
		/// <param name="name">The name of an attachment.</param>
		/// <returns>An AttachmentInfo object</returns>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string itemId, string name)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}

			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			return repository.GetAttachmentInfo(attachmentType, itemId, name);
		}

		/// <summary>
		/// Gets information of a specified attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachmentId">The id of the attachment.</param>
		/// <returns>An AttachmentInfo object</returns>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string attachmentId)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}

			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			return repository.GetAttachmentInfo(attachmentType, attachmentId);
		}

        /// <summary>
        /// Gets count of all attachments of a specified item, instance or class.
        /// </summary>
        /// <param name="attachmentType">One of the AttachmentType enum values.</param>
        /// <param name="itemId">The id of specified instance.</param>
        /// <returns>Count of AttachmentInfo objects</returns>
        /// <remarks>
        /// This is a CMCommand extension method, not available in IDbCommand interface.
        /// </remarks>
        public int GetAttachmentInfosCount(AttachmentType attachmentType,
            string itemId)
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
            return repository.GetAttachmentInfosCount(attachmentType, itemId,
                _connection.MetaDataModel.SchemaModel.SchemaInfo.ID);
        }

		/// <summary>
		/// Gets information of all attachments of a specified item, instance or class.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of specified instance.</param>
        /// <param name="startRow">The start row</param>
        /// <param name="pageSize">The page size</param>
		/// <returns>A collection of AttachmentInfo objects</returns>
		/// <remarks>
		/// This is a CMCommand extension method, not available in IDbCommand interface.
		/// </remarks>
		public AttachmentInfoCollection GetAttachmentInfos(AttachmentType attachmentType,
            string itemId, int startRow, int pageSize)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}

			/*
			bool isAuthenticated = false;
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
			if (principal != null)
			{
				if (principal.Identity.Name != XaclSubject.AnonymousUser)
				{
					isAuthenticated = true;
				}
			}
			*/

			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			return repository.GetAttachmentInfos(attachmentType, itemId,
				_connection.MetaDataModel.SchemaModel.SchemaInfo.ID, startRow, pageSize);
		}
		
		/// <summary> 
		/// Not supported by the CMCommand
		/// </summary>
		public void Prepare()
		{
		}

		/// <summary>
		/// Execute a batch script represented in xml
		/// </summary>
		/// <param name="xmlString">The script in xml string which can be read by a ScriptManager instance</param>
        /// <param name="chunkIndex">The index of chunked data being imported</param>
		/// <returns>A xml string containing the execution status, the xml string returned
		/// can be read by ScriptManager in Newtera.Common.MetaData.Mappings.Scripts package.</returns>
		/// <remarks>This is a CMCommand specific method</remarks>
		public string ExecuteScripts(string xmlString, int chunkIndex)
		{
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            _interpreter.ObtainCachedObjId = false; // TODO, get referenced objId from cache

			StringReader reader = new StringReader(xmlString);
			ScriptManager scriptManager = new ScriptManager();
			scriptManager.Read(reader);

			foreach (ClassScript classScript in scriptManager.ClassScripts)
			{
                // Set the silent mode so that it doesn't gengerate the log entries for each insert
                // Get the CustomPrincipal object from the thread
                if (principal != null)
                {
                    principal.IsSilentMode = true;
                }

				foreach (InstanceScript instanceScript in classScript.InstanceScripts)
				{
					// ExecuteInstanceScript will set the IsSucceeded flag and
					// error message if executing script failed.
					ExecuteInstanceScript(classScript.ClassName, instanceScript);

					// clear the scripts so that we do not have to return them
					// back to the client.
					instanceScript.SearchQuery = null;
					instanceScript.InsertQuery = null;
					instanceScript.UpdateQuery = null;
				}

                if (chunkIndex == 0)
                {
                    if (principal != null)
                    {
                        principal.IsSilentMode = false; // when IsSilentMode is true, IsLoggingOn always return false
                    }

                    // log the import event only for the first chunked data
                    if (LoggingChecker.Instance.IsLoggingOn(_connection.MetaDataModel, classScript.ClassName, LoggingActionType.Import))
                    {
                        ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(classScript.ClassName);
                        string classCaption = (classElement != null ? classElement.Caption : null);
                        LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Import, _connection.MetaDataModel.SchemaInfo.NameAndVersion,
                            classScript.ClassName, classCaption, "Import");

                        LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                    }
                }
			}

			// return execution results as xml string to the client
			StringBuilder builder = new StringBuilder();
			StringWriter writer = new StringWriter(builder);
			scriptManager.Write(writer);
			return builder.ToString();
		}

		/// <summary>
		/// Delete all instances in a class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>A number of instances deleted.</returns>
		/// <remarks>This is a CMCommand specific method</remarks>
		public int DeleteAllInstances(string className)
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Connection must valid and open");
			}
			int count = 0;

			// delete all instances for a class can be executed much faster
			// by deleting instances based on CID. This feature is available
			// after Release 2.9.1 in which CID column is added to the CM_ROOT.
			// all the instances added on and after 2.9.1 release will have CID
			// values filled at CM_ROOT class. For those instances added before
			// 2.9.1 release, they will still be deleted in old fashion.
			try
			{
				// faster way
				count = DeleteAllInstancesByClsId(className);
			}
			catch (Exception)
			{
				// the instances to be deleted were inserted prior to 2.9.1,
				// delete them by hand
				throw new CMException("Unable to delete all instances at once, please delete them manually.");
			}

			return count;
		}

        /// <summary>
        /// Generate a xml document based on a given xml schema for the instance(s) retrived for the specified xquery
        /// </summary>
        /// <param name="xmlSchemaModel">the xml schema model</param>
        /// <param name="instanceDataView">The dataview model of the instance(s)</param>
        /// <returns>An XmlDocument object</returns>
        public XmlDocument GenerateXmlDoc(XMLSchemaModel xmlSchemaModel, DataViewModel instanceDataView)
        {
            XmlDocument doc = null;

            if (xmlSchemaModel != null && instanceDataView != null)
            {
                if (xmlSchemaModel.RootElement.ElementType == instanceDataView.BaseClass.ClassName)
                {
                    XmlReader reader = ExecuteXMLReader();
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (!DataSetHelper.IsEmptyDataSet(ds, instanceDataView.BaseClass.ClassName))
                    {
                        InstanceView instanceView = new InstanceView(instanceDataView, ds);

                        int instanceCount = DataSetHelper.GetRowCount(ds, instanceDataView.BaseClass.ClassName);

                        XmlDocGenerator generator = new XmlDocGenerator(_connection.MetaDataModel, _connection.ConnectionString, xmlSchemaModel, instanceView, instanceCount);

                        doc = generator.Create();
                    }
                }
                else
                {
                    throw new Exception("The base class name of the data view model is different from type of the roor element defined in the xml schema");
                }
            }
            else
            {
                throw new Exception("xmlSchemaModel or dataViewModel is null");
            }

            return doc;
        }

        /// <summary>
        /// This method is simlilar to GenerateXmlDoc method, but only return a xml document without paged data
        /// </summary>
        /// <param name="xmlSchemaModel">the xml schema model</param>
        /// <param name="instanceDataView">The dataview model of the instance(s)</param>
        /// <returns>A XmlDocument instance</returns>
        public XmlDocument BeginCreateDoc(XMLSchemaModel xmlSchemaModel, DataViewModel instanceDataView)
        {
            XmlDocument doc = null;

            if (xmlSchemaModel != null && instanceDataView != null)
            {
                if (xmlSchemaModel.RootElement.ElementType == instanceDataView.BaseClass.ClassName)
                {
                    XmlReader reader = ExecuteXMLReader();
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    if (!DataSetHelper.IsEmptyDataSet(ds, instanceDataView.BaseClass.ClassName))
                    {
                        InstanceView instanceView = new InstanceView(instanceDataView, ds);

                        int instanceCount = DataSetHelper.GetRowCount(ds, instanceDataView.BaseClass.ClassName);

                        _generator = new XmlDocGenerator(_connection.MetaDataModel, _connection.ConnectionString, xmlSchemaModel, instanceView, instanceCount);

                        doc = _generator.BeginCreateDoc();
                    }
                }
                else
                {
                    throw new Exception("The base class name of the data view model is different from type of the roor element defined in the xml schema");
                }
            }
            else
            {
                throw new Exception("xmlSchemaModel or dataViewModel is null");
            }

            return doc;
        }

        /// <summary>
        /// Get the next xml document in paging mode. The BeginCreateDoc must be call initially
        /// </summary>
        /// <returns>A XmlDocument instance</returns>
        public XmlDocument CreateNextDoc()
        {
            XmlDocument doc = null;

            if (_generator != null)
            {
                doc = _generator.CreateNextDoc();
            }

            return doc;
        }

        /// <summary>
        /// Call at end of BeginCreateDoc
        /// </summary>
        public void EndCreateDoc()
        {
            if (_generator != null)
            {
                _generator.EndCreateDoc();
                _generator = null;
            }
        }

        /// <summary>
        /// Get a deep cloned instance that includes cloned related instances defined by the IsOwnedRelationship value of the relationships
        /// </summary>
        /// <param name="orignalInstanceView">The orignal instance</param>
        /// <returns>The deep cloned instance</returns>
        public InstanceView DeepCloneInstance(InstanceView orignalInstanceView)
        {
            return DeepCloneInstance(orignalInstanceView, null, true);
        }

        /// <summary>
        /// Get a deep cloned instance that includes cloned related instances defined by the IsOwnedRelationship value of the relationships
        /// </summary>
        /// <param name="orignalInstanceView">The orignal instance</param>
        /// <param name="deepClone">true to copy the related instances, false not to copy related instances</param>
        /// <returns>The cloned instance</returns>
        public InstanceView DeepCloneInstance(InstanceView orignalInstanceView, bool deepClone)
        {
            return DeepCloneInstance(orignalInstanceView, null, deepClone);
        }

        /// <summary>
        /// Get a deep cloned instance that includes cloned related instances defined by the IsOwnedRelationship value of the relationships
        /// </summary>
        /// <param name="orignalInstanceView">The orignal instance</param>
        /// <param name="stopRelationshipNames"></param>
        /// <param name="deepClone">true to copy the related instances, false not to copy related instances</param>
        /// <returns>The deep cloned instance</returns>
        public InstanceView DeepCloneInstance(InstanceView orignalInstanceView, StringCollection stopRelationshipNames, bool deepClone)
        {
            _clonedInstanceViews = new List<InstanceView>();

            DataViewModel dataView = _connection.MetaDataModel.GetDetailedDataView(orignalInstanceView.DataView.BaseClass.ClassName);

            Hashtable clonedInstanceTable = new Hashtable();

            // Create an instance view
            InstanceView clonedInstanceView = new InstanceView(dataView);

            // copy the values from the orignal instance
            clonedInstanceView.InstanceData.Copy(orignalInstanceView.InstanceData);

            RunInitFunction(clonedInstanceView); // run init function to initialize the instance

            // save the cloned instance to db so that we can clone the related instances that link to the saved one
            clonedInstanceView = SaveInstance(clonedInstanceView);

            _clonedInstanceViews.Insert(0, clonedInstanceView); // keep the cloned instanceView in the list for deletion in case an error occures

            // keep the cloned instance in hash table, using the obj_id of the original instance
            clonedInstanceTable[orignalInstanceView.InstanceData.ObjId] = clonedInstanceView;

            if (deepClone)
            {
                CloneRelatedInstances(orignalInstanceView, clonedInstanceView, clonedInstanceTable, stopRelationshipNames);
            }

            return clonedInstanceView;
        }

        /// <summary>
        /// Rollback the cloned instances
        /// </summary>
        public void RollbackClonedInstances()
        {
            DataViewModel dataView;
            CMCommand cmd;
            string query;

            if (_clonedInstanceViews != null)
            {
                foreach (InstanceView clonedInstanceView in _clonedInstanceViews)
                {
                    dataView = clonedInstanceView.DataView;

                    dataView.CurrentObjId = clonedInstanceView.InstanceData.ObjId;

                    query = dataView.DeleteQuery;

                    cmd = _connection.CreateCommand();
                    cmd.CommandText = query;

                    cmd.ExecuteXMLDoc();
                }
            }
        }

		/// <summary>
		/// Build full-text index for a given class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <remarks>This is a CMCommand specific method</remarks>
		public void BuildFullTextIndex(string className)
		{
			DataViewModel dataView;
			InstanceView clsView;
			InstanceAttributePropertyDescriptor fullTextPropertyDescriptor = null;

			ClassElement theClass = _connection.MetaDataModel.SchemaModel.FindClass(className);

			dataView = _connection.MetaDataModel.GetCompleteDataView(className);
			// Create an class view
			clsView = new InstanceView(dataView);

			// find the full-text search attribute
			foreach (InstanceAttributePropertyDescriptor pd in clsView.GetProperties(null))
			{
				if (pd.IsForFullTextSearch)
				{
					fullTextPropertyDescriptor = pd;
					break;
				}
			}

			if (fullTextPropertyDescriptor != null)
			{
                int depth = IndexingManager.Instance.GetTravelDepth();

                Spider spider = new Spider(depth); 

                spider.StartCrawling(_connection.MetaDataModel, theClass, fullTextPropertyDescriptor.Name);

				// create full-text index on the table
				if (fullTextPropertyDescriptor.DataViewElement.GetSchemaModelElement() is SimpleAttributeElement)
				{
					SimpleAttributeElement attribute = (SimpleAttributeElement) fullTextPropertyDescriptor.DataViewElement.GetSchemaModelElement();

					CreateFullTextIndex(attribute);
				}
			}
		}

        /// <summary>
        /// Get sql actions based on the xquery
        /// </summary>
        /// <returns>A SQLActionCollection object</returns>
        public object GetSQLActions()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection must valid and open");
            }

            // Get the xquery based on cmd type and substitute parameters with values
            string executableQuery = GetExecutableQuery();

            XmlDocument doc = _interpreter.GetSQLActions(executableQuery);

            // the result has been saved in interpreter, so we do not need to get them from
            // XmlDocument
            return _interpreter.SQLActions;
        }

        /// <summary>
        /// Import the data instances in a dataset to a class using the provided sql actions
        /// </summary>
        /// <param name="className">Name of the class to be imported</param>
        /// <param name="ds">The dataset containing data instances</param>
        /// <param name="sqlActions">The sql actions used to generate sql statements</param>
        /// <param name="chunkIndex">The index of the chunked data to be imported</param>
        /// <returns>Execution results containing any errors during the importing.</returns>
        public string ImportDataSet(string className, DataSet ds, object sqlActions, int chunkIndex)
        {
            ScriptManager scriptManager = new ScriptManager();
            ClassScript classScript = new ClassScript(className);
            scriptManager.AddClassScript(classScript);
            InstanceScript instanceScript;

            SQLActionCollection theSqlActions = (SQLActionCollection)sqlActions;

            DataViewModel dataView = _connection.MetaDataModel.GetDetailedDataView(className);
            InstanceData instanceData = new InstanceData(dataView, ds, true);

            DataTable dataTable = ds.Tables[className];
            int instanceCount = 0;
            if (dataTable != null)
            {
                instanceCount = dataTable.Rows.Count;
            }

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            InsertTemplateVisitor visitor = new InsertTemplateVisitor(_connection.MetaDataModel, dataView, theSqlActions, dataProvider);

            EditSQLExecutor executor = new EditSQLExecutor(_connection.MetaDataModel, dataProvider);

            try
            {
                for (int i = 0; i < instanceCount; i++)
                {

                    // Set the row index to InstanceData instance will cause
                    // it to copy values of the DataRow of the DataSet to the
                    // contained DataViewModel instance
                    instanceData.RowIndex = i;

                    // generate executable sqls in SQLActions using the current data instance stored
                    // in the data view model
                    dataView.Accept(visitor);

                    instanceScript = new InstanceScript();
                    classScript.InstanceScripts.Add(instanceScript);

                    try
                    {
                        // execute the sqls as result of InsertTemplateVisitor visiting the data view model
                        executor.Execute(theSqlActions);
                    }
                    catch (Exception ex)
                    {
                        instanceScript.IsSucceeded = false;
                        instanceScript.Message = ex.Message;
                        throw ex;
                    }
                }

                // commit changes at once
                executor.CommitChanges();
            }
            catch (Exception)
            {
                executor.RollbackChanges();
            }
            finally
            {
                executor.Close(); // close the connection
            }

            if (chunkIndex == 0)
            {
                // log the import event only for the first chunked data
                if (LoggingChecker.Instance.IsLoggingOn(_connection.MetaDataModel, className, LoggingActionType.Import))
                {
                    LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Import, _connection.MetaDataModel.SchemaInfo.NameAndVersion,
                        className, dataView.BaseClass.Caption, "Import");

                    LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                }
            }

            // return execution results as xml string to the client
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            scriptManager.Write(writer);
            return builder.ToString();
        }

		/// <summary>
		/// Implement IDisposable interface
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
		}

		/// <summary>
		/// Get an executable XQuery text based on the command type.
		/// </summary>
		/// <returns> An executable XQuery string.</returns>
		private string GetExecutableQuery()
		{
			string query = "";
			
			if (_cmdType == CommandType.Text)
			{
				query = _cmdText;
			}
			else if (_cmdType == CommandType.StoredProcedure)
			{
				// Here, cmdText specifies a path of a stored query
				query = StoredQuery;
			}
			else
			{
				/*
				* Command type must be TableDirect, create a xquery that returns all
				* the columns of the specified class(Table)
				*/
				// The command text is a class name, find corresponding class element for the class
				ClassElement classElement = MetaDataCache.Instance.GetMetaData(_connection.SchemaInfo, _connection.DataProvider).SchemaModel.FindClass(this.CommandText);
				
				// create an class entity for it
				ClassEntity classEntity = new ClassEntity(classElement);

				QueryBuilder builder = new QueryBuilder();
				query = builder.BuildSelectStatement(classEntity);
			}
			
			/*
			* Substitute the parameters in a XQuery with the values provided in the
			* parameter collections.
			*/
			VariableQuery vquery = new VariableQuery(query, this._parameters);
			
			return vquery.Substitute();
		}

		/// <summary>
		/// For debugging
		/// </summary>
		/// <param name="doc"></param>
		private void PrintXml(XmlDocument doc)
		{
			//XmlTextWriter writer = new XmlTextWriter(System.Console.Out);
			StringBuilder builder = new StringBuilder();
			StringWriter strWriter = new StringWriter(builder);
			XmlTextWriter writer = new XmlTextWriter(strWriter);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;

			doc.WriteTo(writer);

			string xml = builder.ToString();

			return;
		}

		/// <summary>
		/// Execute the insert query if the instance has not been created before,
		/// otherwise, execute the update query on the instance.
		/// </summary>
		/// <param name="className">The name of instance class.</param>
		/// <param name="instanceScript">The InstanceScript instance contains the queries.</param>
		private void ExecuteInstanceScript(string className, InstanceScript instanceScript)
		{
			XmlDocument doc;

			try
			{
				if (instanceScript.SearchQuery != null)
				{
					// run the query to see if the instance has existed
					_interpreter.Reset(); // so that it can be reused
					doc = _interpreter.Query(instanceScript.SearchQuery);

					string objId = GetObjId(doc, className);

                    if (string.IsNullOrEmpty(objId))
					{
						_interpreter.Reset(); // so that it can be reused

						// the instance does not exist, execute insert query
						doc = _interpreter.Query(instanceScript.InsertQuery);
					}
					else if (instanceScript.UpdateQuery != null && instanceScript.UpdateQuery.Length > 0)
					{
						_interpreter.Reset(); // so that it can be reused

						// the instance exists, execute the update query for
						// the instance of given obj_id
						string updateQuery = instanceScript.UpdateQuery.Replace(InstanceScript.OBJ_ID,
							objId);

						doc = _interpreter.Query(updateQuery);
					}
				}
				else
				{
					_interpreter.Reset(); // so that it can be reused

					// there isn't a search query that retrieves an instance,
					// assuming the instance doesn't exist, insert the instance.
					doc = _interpreter.Query(instanceScript.InsertQuery);
				}

				instanceScript.IsSucceeded = true;
			}
			catch (Exception ex)
			{
				instanceScript.IsSucceeded = false;
				instanceScript.Message = ex.Message;
			}
		}

		/// <summary>
		/// Get the value of obj_id from a XmlDocument that is result of running
		/// a search query for an instance.
		/// </summary>
		/// <param name="doc">The XmlDocument</param>
		/// <param name="className">The name of instance class.</param>
		/// <returns>The value of obj_id</returns>
		private string GetObjId(XmlDocument doc, string className)
		{
			string objId = null;

			if (doc != null && doc.DocumentElement.ChildNodes.Count > 0)
			{
				XmlElement instaneElement = doc.DocumentElement[className];

				if (instaneElement != null)
				{
					// obj_id is an attribute of the element
					objId = instaneElement.GetAttribute(NewteraNameSpace.OBJ_ID);
				}
			}

			return objId;
		}

		/// <summary>
		/// Update value of the full-text attribute for the specified instance
		/// </summary>
		/// <param name="instanceView">A instance view</param>
		/// <param name="fullTextPropertyDecriptor">The full-text serach property descriptor</param>
		private void UpdateInstanceContent(InstanceView instanceView, InstanceAttributePropertyDescriptor fullTextPropertyDecriptor)
		{
			string query;

			// concate values of selected instance propertties together
			StringBuilder builder = new StringBuilder();
			foreach (InstanceAttributePropertyDescriptor property in instanceView.GetProperties(null))
			{
				if (property.IsBrowsable &&
					property.IsGoodForFullTextSearch &&
					property.GetValue() != null)
				{
					object propertyValue = property.GetValue();

					if (propertyValue is ArrayDataTableView)
					{
						builder.Append(property.GetValue().ToString()).Append(" ");	                
					}
					else if (property.IsMultipleChoice)
					{
						object[] vEnums = (object[]) propertyValue;
						for (int i = 0; i < vEnums.Length; i++)
						{
							builder.Append(vEnums[i].ToString()).Append(" ");
						}
					}
					else
					{
						builder.Append(property.GetValue().ToString()).Append(" ");
					}

				}
			}

			// set the new content
			string contentVal = builder.ToString();
			fullTextPropertyDecriptor.SetValue(null, contentVal);

			// do not update if the content does not change
			if (instanceView.IsDataChanged)
			{
				query = instanceView.DataView.UpdateQuery;

				_interpreter.Reset(); // so that it can be reused

				this.CommandText = query;
					
				try
				{
					XmlDocument doc = this.ExecuteXMLDoc();
				}
				catch (Exception)
				{
					// do not stop if there is error
				}				
			}
		}

		/// <summary>
		/// Create database full-text index using native database connection
		/// </summary>
		private void CreateFullTextIndex(SimpleAttributeElement attribute)
		{
            DBIndexCreator indexCreator = new DBIndexCreator();

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            indexCreator.CreateFullTextIndex(dataProvider, attribute);
		}

		/// <summary>
		/// Gets instances count on a table with the given class id
		/// </summary>
		/// <param name="tableName">The table name</param>
		/// <param name="clsId">The class id</param>
		/// <returns></returns>
		private int GetInstanceCount(string tableName, string clsId)
		{
			int count = 0;
			string sql = "select count(CID) from " + tableName + " where CID=" + clsId;

			IDbConnection con = null;
			IDataReader reader = null;

			try
			{
				// get database connection
				con = _connection.DataProvider.Connection;
				IDbCommand cmd = con.CreateCommand();

				cmd.CommandText = sql;
				reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					count = reader.GetInt32(0);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
				}

				if (con != null)
				{
					con.Close();
				}
			}

			return count;
		}

		/// <summary>
		/// Delete all instances of a class using canned SQL based on class id
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>The number of instances deleted.</returns>
		private int DeleteAllInstancesByClsId(string className)
		{
			int count = 0;

			// all the instances in the class must be added on or after CID
			// column is added
			ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(className);
			
			if (classElement == null)
			{
				throw new CMException("Failed to find a class with name: " + className);
			}

			string clsID = classElement.ID;

			// remembers all the tables involved in the delete action with top-down order
			StringCollection tableNames = new StringCollection();
			ClassElement parent = classElement;
			while (parent != null)
			{
				tableNames.Insert(0, parent.TableName);

				parent = parent.ParentClass;
			}

			// finally insert CM_ROOT table
			tableNames.Insert(0, NewteraNameSpace.CM_ROOT_TABLE);

			// delete all instances whose CID equals the given class id
			string sql;
			IDbConnection con = _connection.DataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;

			try
			{
				foreach (string tblName in tableNames)
				{
					sql = "delete from " + tblName + " where CID=" + clsID;

					cmd.CommandText = sql;
					count = cmd.ExecuteNonQuery();
				}

				tran.Commit();

                // log the event if the particular log is on
                if (LoggingChecker.Instance.IsLoggingOn(_connection.MetaDataModel, classElement, LoggingActionType.Delete))
                {
                    LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Delete, _connection.MetaDataModel.SchemaInfo.NameAndVersion,
                        classElement.Name, classElement.Caption, "Delete All");

                    LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                }
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw e;
			}
			finally
			{
				con.Close();
			}

			return count;
		}

        /// <summary>
        /// Clone the instances in the related classes that are owned by the parent instance
        /// </summary>
        /// <remarks>This is called recursively</remarks>
        private void CloneRelatedInstances(InstanceView orignalParentInstanceView, InstanceView clonedParentInstanceView, Hashtable clonedInstanceTable, StringCollection stopRelationshipNames)
        {
            InstanceView orignalRelatedInstanceView;
            InstanceView clonedRelatedInstanceView;
            int count = 0;
            ReferencedClassCollection relatedClasses = GetRelatedClasses(orignalParentInstanceView.DataView.BaseClass);
            foreach (DataClass relatedClass in relatedClasses)
            {
                if (!relatedClass.ReferringRelationship.IsForeignKeyRequired &&
                    relatedClass.ReferringRelationship.IsOwnedRelationship
                    && !IsStopRelationship(relatedClass.ReferringRelationship.Name, stopRelationshipNames))
                {
                    orignalRelatedInstanceView = GetRelatedInstances(relatedClass, orignalParentInstanceView, out count);
                    if (count > 0)
                    {
                        // cloned related instance one by one
                        for (int i = 0; i < count; i++)
                        {
                            orignalRelatedInstanceView.SelectedIndex = i;

                            clonedRelatedInstanceView = GetClonedRelatedInstance(relatedClass, orignalRelatedInstanceView, orignalParentInstanceView, clonedParentInstanceView, clonedInstanceTable);

                            // going down the chain
                            CloneRelatedInstances(orignalRelatedInstanceView, clonedRelatedInstanceView, clonedInstanceTable, stopRelationshipNames);
                        }
                    }
                }
            }
        }

        // Get information indicating whether a relationship is one of the stop relationships
        private bool IsStopRelationship(string relationshipName, StringCollection stopRelationshipNames)
        {
            bool status = false;

            if (stopRelationshipNames != null && stopRelationshipNames.Count > 0)
            {
                status = stopRelationshipNames.Contains(relationshipName);
            }

            return status;
        }

        /// <summary>
        /// Get a collection of the DataClass objects that represents the data classes that are
        /// linked to a base data class through the relationship attributes
        /// </summary>
        /// <remarks>Unlike the default behaviouse, for many-to-many relationship, this method will return the junction class as a related class, instead of the class on the another side of junction class.</remarks>
        public ReferencedClassCollection GetRelatedClasses(DataClass baseClass)
        {
            ReferencedClassCollection referencedClasses = new ReferencedClassCollection();

            ClassElement currentClassElement = baseClass.GetSchemaModelElement() as ClassElement;
            DataClass relatedClass;
            ClassElement relatedClassElement;
            while (currentClassElement != null)
            {
                foreach (RelationshipAttributeElement relationshipAttribute in currentClassElement.RelationshipAttributes)
                {
                    if (relationshipAttribute.IsBrowsable)
                    {
                        relatedClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                            relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                        relatedClass.ReferringClassAlias = baseClass.Alias;
                        relatedClass.ReferringRelationshipName = relationshipAttribute.Name;
                        relatedClass.Caption = relationshipAttribute.LinkedClass.Caption;
                        relatedClass.ReferringRelationship = relationshipAttribute;
                        relatedClassElement = _connection.MetaDataModel.SchemaModel.FindClass(relationshipAttribute.LinkedClassName);
                        relatedClass.IsLeafClass = relatedClassElement.IsLeaf;

                        referencedClasses.Add(relatedClass);
                    }
                }

                currentClassElement = currentClassElement.ParentClass;
            }

            return referencedClasses;
        }

        /// <summary>
        /// Get the related instances from DB
        /// </summary>
        /// <returns></returns>
        private InstanceView GetRelatedInstances(DataClass relatedClass, InstanceView masterInstanceView, out int count)
        {
            InstanceView instanceView = null;
            DataViewModel realtedDataView = _connection.MetaDataModel.GetRelatedDetailedDataView(masterInstanceView, relatedClass.ClassName);

            realtedDataView.ClearSortBy();

            // add @obj_id as a sort attribute
            SortAttribute sortAttribute = new SortAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, realtedDataView.BaseClass.Alias);
            sortAttribute.SortDirection = SortDirection.Ascending;
            realtedDataView.SortBy.SortAttributes.Add(sortAttribute);

            count = 0;

            // get query
            realtedDataView.PageSize = PAGE_SIZE;

            string query = realtedDataView.SearchQuery;

            // Create a CMDataReder object for the query
            CMCommand cmd = _connection.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, realtedDataView.BaseClass.ClassName))
            {
                count = DataSetHelper.GetRowCount(ds, realtedDataView.BaseClass.Name);

                // Create an instance view with instance data
                instanceView = new InstanceView(realtedDataView, ds);
            }

            return instanceView;
        }

        /// <summary>
        /// Cloned and return a related instance
        /// </summary>
        /// <returns></returns>
        private InstanceView GetClonedRelatedInstance(DataClass relatedClass, InstanceView orignalRelatedInstanceView, InstanceView orignalParentInstanceView, InstanceView clonedParentInstanceView, Hashtable clonedInstanceTable)
        {
            InstanceView clonedInstanceView;

            // try to get the cloned instance from the hash table since the original instance may have been cloned as the result of travelling other relationship path
            clonedInstanceView = (InstanceView)clonedInstanceTable[orignalRelatedInstanceView.InstanceData.ObjId];
            if (clonedInstanceView == null)
            {
                InstanceView orignalLeafRelatedInstanceView = GetLeafInstanceView(relatedClass, orignalRelatedInstanceView);

                // get a related data view for the leaf class
                DataViewModel leafRelatedDataView = _connection.MetaDataModel.GetRelatedDetailedDataView(orignalParentInstanceView, orignalLeafRelatedInstanceView.DataView.BaseClass.ClassName);

                clonedInstanceView = new InstanceView(leafRelatedDataView);

                // copy the values from the orignal related instance
                clonedInstanceView.InstanceData.Copy(orignalLeafRelatedInstanceView.InstanceData);

                RunInitFunction(clonedInstanceView); // run init function on the related instance

                // save to the database
                clonedInstanceView = SaveInstance(clonedInstanceView);

                _clonedInstanceViews.Insert(0, clonedInstanceView); // keep the cloned instanceView in the list for deletion in case an error occures

                // keep the cloned instance in the hash table
                clonedInstanceTable[orignalRelatedInstanceView.InstanceData.ObjId] = clonedInstanceView;
            }

            if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
            {
                // it is a many-to-one relationship between the parent class and related class,
                // associated the parent instance to the related instance
                clonedParentInstanceView.InstanceData.SetAttributeStringValue(relatedClass.ReferringRelationship.Name, clonedInstanceView.InstanceData.PrimaryKeyValues);
                UpdateInstance(clonedParentInstanceView);
            }
            else
            {
                // it is a one-to-many relationship bewtween the parent class and related class
                // associated related instance to the parent instance
                clonedInstanceView.InstanceData.SetAttributeStringValue(relatedClass.ReferringRelationship.BackwardRelationship.Name, clonedParentInstanceView.InstanceData.PrimaryKeyValues);
                UpdateInstance(clonedInstanceView);
            }

            return clonedInstanceView;
        }

        private InstanceView GetLeafInstanceView(DataClass relatedClass, InstanceView orignalInstanceView)
        {
            InstanceView leafInstanceView = null;

            if (!relatedClass.IsLeafClass)
            {
                // get data view for the related leaf class
                string instanceId = orignalInstanceView.InstanceData.ObjId;
                string query = orignalInstanceView.DataView.GetInstanceQuery(instanceId);
                CMCommand cmd = _connection.CreateCommand();
                cmd.CommandText = query;
                StringCollection leafClasses = cmd.GetClassNames();
                if (leafClasses.Count > 0)
                {
                    // if there are more than one leaf classes, only uses the first one
                    DataViewModel leafDataView = _connection.MetaDataModel.GetDetailedDataView(leafClasses[0]);
                    query = leafDataView.GetInstanceQuery(instanceId);

                    cmd = _connection.CreateCommand();
                    cmd.CommandText = query;

                    XmlReader reader = cmd.ExecuteXMLReader();
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    // Create a new instance view
                    leafInstanceView = new InstanceView(leafDataView, ds);
                }
                else
                {
                    throw new Exception("Unable to find a leaf class for " + orignalInstanceView.DataView.BaseClass.ClassName + " with objId " + instanceId);
                }
            }
            else
            {
                leafInstanceView = orignalInstanceView;
            }

            return leafInstanceView;
        }

        private void RunInitFunction(InstanceView instanceView)
        {
            ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.ClassName);
            if (!string.IsNullOrEmpty(classElement.InitializationCode))
            {
                // initialize the new instance with initialization code

                // Execute the initialization code using the same connection so that changes are made within a same transaction
                IInstanceWrapper instanceWrapper = new Newtera.Server.Engine.Workflow.InstanceWrapper(instanceView);

                // run the initialization code on the instance
                ActionCodeRunner.Instance.ExecuteActionCode("GetNewAPI", "ClassInit" + classElement.ID, classElement.InitializationCode, instanceWrapper);
            }
        }

        private void RunBeforeInsertFunction(InstanceView instanceView)
        {
            ClassElement classElement = _connection.MetaDataModel.SchemaModel.FindClass(instanceView.DataView.BaseClass.ClassName);
            if (!string.IsNullOrEmpty(classElement.BeforeInsertCode))
            {
                // Execute the before insert code using the same connection so that changes are made within a same transaction
                IInstanceWrapper instanceWrapper = new Newtera.Server.Engine.Workflow.InstanceWrapper(instanceView);

                // run the initialization code on the instance
                ActionCodeRunner.Instance.ExecuteActionCode("GetNewAPI", "ClassBeforeInsert" + classElement.ID, classElement.BeforeInsertCode, instanceWrapper);
            }
        }

        private InstanceView SaveInstance(InstanceView instanceView)
        {
            InstanceView savedInstanceView = null;

            RunBeforeInsertFunction(instanceView); // run before insert function to modify the instance

            string query = instanceView.DataView.InsertQuery;

            CMCommand cmd = _connection.CreateCommand();

            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc();// insert the instance to database

            string objId = doc.DocumentElement.InnerText;

            // retrive the created data package item to get its primary key
            query = instanceView.DataView.GetInstanceQuery(objId);
            cmd = _connection.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            // Create a new instance view

            DataViewModel instanceDataView = _connection.MetaDataModel.GetDetailedDataView(instanceView.DataView.BaseClass.ClassName);
            savedInstanceView = new InstanceView(instanceDataView, ds);

            return savedInstanceView;
        }

        private void UpdateInstance(InstanceView instanceView)
        {
            if (instanceView.IsDataChanged)
            {
                string query = instanceView.DataView.UpdateQuery;

                CMCommand cmd = _connection.CreateCommand();
                cmd.CommandText = query;

                XmlDocument doc = cmd.ExecuteXMLDoc();// update the instance in database
            }
        }
	}

    public enum CMCommandBehavior
    {
        Default,
        CheckReadPermissionOnly,
        ObtainCachedObjId
    }
}