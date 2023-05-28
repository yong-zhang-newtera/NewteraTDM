/*
* @(#) MetaDataAdapter.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Data;
	using System.IO;
	using System.Collections;
	using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.FileType;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// Represents the bridge between a data source and a MetaDataModel.
	/// </summary>
	/// <version> 	1.0.0	15 Aug 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class MetaDataAdapter
	{
		/* MM_SCHEMA */
		public const string MM_SCHEMA            = "MM_SCHEMA";
		public const string XML_SCHEMA           = "XML_SCHEMA";
		public const string DATA_VIEWS           = "QUERIES";
		public const string XACL_POLICY          = "XACL";
		public const string TAXONOMIES			 = "TAXONOMY";
		public const string RULES				 = "RULES";
		public const string MAPPINGS			 = "MAPPINGS";
		public const string SELECTORS			 = "SELECTORS";
        public const string EVENTS               = "EVENTS";
        public const string SUBSCRIBERS          = "SUBSCRIBERS";
        public const string LOGGING              = "LOGGING";
        public const string XMLSCHEMAVIEWS       = "XMLSCHEMAVIEWS";
        public const string APIS                 = "APIS";
		public const string LOG					 = "LOG";

		// File name for FileTypeInfos
		public const string CONFIG_DIR = @"\Config\";
		public const string FileTypesName = "file_types.xml";

		private IDataProvider _dataProvider;

		/// <summary>
		/// Intiating an instance of MetaDataAdapter class.
		/// </summary>
		public MetaDataAdapter()
		{
			_dataProvider = null;
		}

		/// <summary>
		/// Intiating an instance of MetaDataAdapter class.
		/// </summary>
		/// <param name="dataProvider">The data provider.</param>
		public MetaDataAdapter(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Gets all Schemas from database.
		/// </summary>
		/// <returns>
		/// An Array of SchemaInfo objects
		/// </returns>
		/// <exception cref="DBException">If a database access error occurs.</exception>
		public SchemaInfo[] GetSchemaInfos()
		{
			ArrayList schemas = new ArrayList();
            ArrayList missingTimestampSchemas = new ArrayList();
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetMMSchemas");
			
			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				/* save the schema data into SchemaInfo */
				while (reader.Read())
				{
					SchemaInfo schemaInfo = new SchemaInfo();
					schemaInfo.ID = System.Convert.ToString(reader[("ID")]);
					schemaInfo.Name = System.Convert.ToString(reader[("NAME")]);
					//schemaInfo.Description = System.Convert.ToString(reader[("DESCRIPTION")]);
					schemaInfo.Version = System.Convert.ToString(reader[("VERSION")]);
                    if (!reader.IsDBNull(4))
                    {
                        schemaInfo.ModifiedTime = DateTime.Parse(reader["MODIFIED_TIME"].ToString());
                    }
                    else
                    {
                        // this is the schema info saved using 3.1.0 or earlier,
                        // write the current time as the latest modified time later
                        schemaInfo.ModifiedTime = DateTime.Now; // default
                        missingTimestampSchemas.Add(schemaInfo);
                    }

					schemas.Add(schemaInfo);
				}

				SchemaInfo[] schemaArray = new SchemaInfo[schemas.Count];

				int idx = 0;
				foreach (SchemaInfo schema in schemas)
				{
					schemaArray[idx++] = schema;
				}

                // write the timestamp back to the database for those that do not have
                // timestamps
                foreach (SchemaInfo schemaInfo in missingTimestampSchemas)
                {
                    SetModifiedTime(schemaInfo, schemaInfo.ModifiedTime);

                    // for some reason, the value of DateTime.Now is less than what
                    // stored in the database. Therefore, we need to retrieave the modified time
                    // from database
                    GetModifiedTime(schemaInfo);
                }

				return schemaArray;
			}
			catch (Exception ex)
			{
				throw new DBException("Failed to get all schema info", ex);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Adds or refreshes meta data in the MetaDataModel to match those in the
		/// data source using the schema info.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with data</param>
		/// <remarks>It will fill a particular meta data if the specific needReload flag is true</remarks>
		public void Fill(MetaDataModel metaData)
		{
			if (metaData.NeedReloadSchema)
			{
				FillSchema(metaData);
			}

			if (metaData.NeedReloadDataViews)
			{
				FillDataViews(metaData);
			}

			if (metaData.NeedReloadXaclPolicy)
			{
				FillXaclPolicy(metaData);
			}

			if (metaData.NeedReloadTaxonomies)
			{
				FillTaxonomies(metaData);
			}

			if (metaData.NeedReloadRules)
			{
				FillRules(metaData);
			}

			if (metaData.NeedReloadMappings)
			{
				FillMappings(metaData);
			}

			if (metaData.NeedReloadSelectors)
			{
				FillSelectors(metaData);
			}

            if (metaData.NeedReloadEvents)
            {
                FillEvents(metaData);
            }

            if (metaData.NeedReloadLoggingPolicy)
            {
                FillLogging(metaData);
            }

            if (metaData.NeedReloadSubscribers)
            {
                FillSubscribers(metaData);
            }

            if (metaData.NeedReloadXMLSchemaViews)
            {
                FillXMLSchemaViews(metaData);
            }

            if (metaData.NeedReloadApis)
            {
                FillApis(metaData);
            }
        }

		/// <summary>
		/// Fill the File Type Information.
		/// </summary>
		/// <param name="fileTypeInfo">A FileTypeInfoCollection object</param>
		public void FillFileTypeInfo(FileTypeInfoCollection fileTypeInfos)
		{
			string fullPath = NewteraNameSpace.GetAppHomeDir() + MetaDataAdapter.CONFIG_DIR + MetaDataAdapter.FileTypesName;

			using (Stream stream = File.OpenRead(fullPath))
			{
				fileTypeInfos.Read(stream);
			}

			// load the icon images from files into the memory
			foreach (FileTypeInfo info in fileTypeInfos)
			{
				info.SmallIconStream = GetMemoryStream(info.SmallIconPath);
				info.LargeIconStream = GetMemoryStream(info.LargeIconPath);
			}
		}

		/// <summary>
		/// Write a meta data as xml string to the corresponding clobs.
		/// </summary>
		/// <param name="metaData">The meta data to be written</param>
		public void WriteMetaData(MetaDataModel metaData)
		{
			SchemaInfo schemaInfo = metaData.SchemaInfo;
			StringBuilder builder = new StringBuilder();
			StringWriter writer = new StringWriter(builder);
			metaData.SchemaModel.Write(writer);

			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(builder.ToString(), MM_SCHEMA, XML_SCHEMA, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Write an xml representing data views to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
		/// <param name="xmlString">A xml string representing data views</param>
		public void WriteDataViews(SchemaInfo schemaInfo, string xmlString)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(xmlString, MM_SCHEMA, DATA_VIEWS, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Write an xml representing xacl policy to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
		/// <param name="xmlString">A xml string representing data views</param>
		public void WriteXaclPolicy(SchemaInfo schemaInfo, string xmlString)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(xmlString, MM_SCHEMA, XACL_POLICY, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Write an xml representing taxonomies to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
		/// <param name="xmlString">A xml string representing taxonomies</param>
		public void WriteTaxonomies(SchemaInfo schemaInfo, string xmlString)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(xmlString, MM_SCHEMA, TAXONOMIES, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Write an xml representing rules to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
		/// <param name="xmlString">A xml string representing rules</param>
		public void WriteRules(SchemaInfo schemaInfo, string xmlString)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(xmlString, MM_SCHEMA, RULES, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Write an xml representing mappings to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
		/// <param name="xmlString">A xml string representing mappings</param>
		public void WriteMappings(SchemaInfo schemaInfo, string xmlString)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(xmlString, MM_SCHEMA, MAPPINGS, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Write an xml representing selectors to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
		/// <param name="xmlString">A xml string representing selectors</param>
		public void WriteSelectors(SchemaInfo schemaInfo, string xmlString)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(xmlString, MM_SCHEMA, SELECTORS, schemaInfo.Name, schemaInfo.Version);
			}
		}

        /// <summary>
        /// Write an xml representing events to the database.
        /// </summary>
        /// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
        /// <param name="xmlString">A xml string representing events</param>
        public void WriteEvents(SchemaInfo schemaInfo, string xmlString)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(xmlString, MM_SCHEMA, EVENTS, schemaInfo.Name, schemaInfo.Version);
            }
        }

        /// <summary>
        /// Write an xml representing logging policy to the database.
        /// </summary>
        /// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
        /// <param name="xmlString">A xml string representing logging policy</param>
        public void WriteLoggingPolicy(SchemaInfo schemaInfo, string xmlString)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(xmlString, MM_SCHEMA, LOGGING, schemaInfo.Name, schemaInfo.Version);
            }
        }

        /// <summary>
        /// Write an xml representing subscribers to the database.
        /// </summary>
        /// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
        /// <param name="xmlString">A xml string representing subscribers</param>
        public void WriteSubscribers(SchemaInfo schemaInfo, string xmlString)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(xmlString, MM_SCHEMA, SUBSCRIBERS, schemaInfo.Name, schemaInfo.Version);
            }
        }

        /// <summary>
        /// Write an xml representing xml schema views to the database.
        /// </summary>
        /// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
        /// <param name="xmlString">A xml string representing xml schema views</param>
        public void WriteXMLSchemaViews(SchemaInfo schemaInfo, string xmlString)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(xmlString, MM_SCHEMA, XMLSCHEMAVIEWS, schemaInfo.Name, schemaInfo.Version);
            }
        }

        /// <summary>
        /// Write an xml representing apis to the database.
        /// </summary>
        /// <param name="schemaInfo">Info indicating the schema whose corresponding clob to be updated</param>
        /// <param name="xmlString">A xml string representing apis</param>
        public void WriteApis(SchemaInfo schemaInfo, string xmlString)
        {
            // get the clob object
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                clobDAO.WriteClob(xmlString, MM_SCHEMA, APIS, schemaInfo.Name, schemaInfo.Version);
            }
        }

        /// <summary>
        /// Write an xml representing file type info to a file.
        /// </summary>
        /// <param name="xmlString">A xml string representing file type info</param>
        public void WriteFileTypeInfo(string xmlString)
		{
			string fullPath = NewteraNameSpace.GetAppHomeDir() + MetaDataAdapter.CONFIG_DIR + MetaDataAdapter.FileTypesName;
			
			using (FileStream stream = File.OpenWrite(fullPath))
			{
				StringReader stringReader = new StringReader(xmlString);
				
				StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode);
				int actual = 0;
				char[] cbuffer = new char[100];
				while((actual = stringReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) >0)
				{
					streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
					streamWriter.Flush();
				}			
			}
		}

		/// <summary>
		/// Write a meta data update log to the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema that has been updated</param>
		/// <param name="log">The meta data update log</param>
		public void WriteLog(SchemaInfo schemaInfo, string log)
		{
			// get the clob object
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				clobDAO.WriteClob(log, MM_SCHEMA, LOG, schemaInfo.Name, schemaInfo.Version);
			}
		}

		/// <summary>
		/// Read a meta data update log from the database.
		/// </summary>
		/// <param name="schemaInfo">Info indicating the schema of the update log</param>
		/// <returns>The latest meta data update log</returns>
		public string ReadLog(SchemaInfo schemaInfo)
		{
			// get a stream to read schema model from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				string log = clobDAO.ReadClobAsText(MM_SCHEMA, LOG, schemaInfo.Name, schemaInfo.Version);

				if (log == null)
				{
					log = " ";
				}

				return log;
			}
		}

		/// <summary>
		/// Gets the name of role that has permission to modify the meta data
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <returns>The name of role, null for non-protected mode.</returns>
		/// <exception cref="DBException">If a database access error occurs.</exception>
		public string GetDBARole(SchemaInfo schemaInfo)
		{
			string role = null;
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetDBARole");
			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
					sql = sql.Replace("@name", "'" + schemaInfo.Name + "'");
					sql = sql.Replace("@version", "'" + schemaInfo.Version + "'");
					break;

				case DatabaseType.Oracle:
					sql = sql.Replace(":name", "'" + schemaInfo.Name + "'");
					sql = sql.Replace(":version", "'" + schemaInfo.Version + "'");
					break;
			}

			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					if (!reader.IsDBNull(0))
					{
						role = reader.GetString(0);
						if (role.Length == 0)
						{
							role = null;
						}
					}
				}

				return role;
			}
			catch (Exception ex)
			{
				throw new DBException("Failed to get dba role", ex);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Sets the name of role that has permission to modify the meta data
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <param name="role">The name of role, null to set non-protected mode.</param>
		/// <exception cref="DBException">If a database access error occurs.</exception>		
		public void SetDBARole(SchemaInfo schemaInfo, string role)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("SetDBARole");
			
			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + schemaInfo.Name + "'");
					sql = sql.Replace("@version", "'" + schemaInfo.Version + "'");
					if (role != null)
					{
						sql = sql.Replace("@dba_role", "'" + role + "'");
					}
					else
					{
						sql = sql.Replace("@dba_role", "''");
					}
					break;

				case DatabaseType.Oracle:
					sql = sql.Replace(":name", "'" + schemaInfo.Name + "'");
					sql = sql.Replace(":version", "'" + schemaInfo.Version + "'");
					if (role != null)
					{
						sql = sql.Replace(":dba_role", "'" + role + "'");
					}
					else
					{
						sql = sql.Replace(":dba_role", "''");
					}

					break;
			}

			try
			{
				cmd.CommandText = sql;

				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw new DBException("Failed to set dba role", ex);
			}
			finally
			{
				con.Close();
			}
		}

        /// <summary>
        /// Sets the time as the latest modified time of the given meta data model
        /// </summary>
        /// <param name="schemaInfo">The schema info to check</param>
        /// <param name="modifiedTime">The modified time.</param>
        /// <exception cref="DBException">If a database access error occurs.</exception>		
        public void SetModifiedTime(SchemaInfo schemaInfo, DateTime modifiedTime)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("SetMetaModelModifiedTime");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
            string modifiedDateTime = lookup.GetTimestampFunc(modifiedTime.ToString("s"), LocaleInfo.Instance.DateTimeFormat);

            switch (dataProvider.DatabaseType)
            {
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + schemaInfo.Name + "'");
                    sql = sql.Replace("@version", "'" + schemaInfo.Version + "'");
                    sql = sql.Replace("@modified_time", modifiedDateTime);
                    
                    break;

                case DatabaseType.Oracle:
					sql = sql.Replace(":name", "'" + schemaInfo.Name + "'");
                    sql = sql.Replace(":version", "'" + schemaInfo.Version + "'");
                    sql = sql.Replace(":modified_time", modifiedDateTime);

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw new DBException("Failed to set modified time of meta-data model.", ex);
            }
            finally
            {
                con.Close();
            }
        }

		/// <summary>
		/// Gets a list of identifiers of registered clients from database table.
		/// </summary>
		/// <param name="clientName">The client name.</param>
		/// <returns>A list of client ids.</returns>
		public string[] GetRegisteredClients(string clientName)
		{
			string[] ids = null;
			StringCollection clientIds = new StringCollection();

			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetRegisteredClients");
			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + clientName + "'");
					break;

				case DatabaseType.Oracle:
					sql = sql.Replace(":name", "'" + clientName + "'");
					break;
			}

			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();
				string id;
				while (reader.Read())
				{
					id = reader.GetString(0);
					if (id != null)
					{
                        clientIds.Add(id);
					}
				}

				// convert to string array
                ids = new string[clientIds.Count];
                for (int i = 0; i < clientIds.Count; i++)
				{
                    ids[i] = clientIds[i];
				}

				return ids;
			}
			catch (Exception ex)
			{
				throw new DBException("Failed to get information of registered clients", ex);
			}
			finally
			{
                if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

        /// <summary>
        /// Gets the machine name of a registered client
        /// </summary>
        /// <param name="clientName">The window client name</param>
        /// <param name="clientId">The client id</param>
        /// <returns>The name of registered client machine.</returns>
        /// <exception cref="DBException">If a database access error occurs.</exception>
        public string GetRegisteredClientMachine(string clientName, string clientId)
        {
            string machine = null;
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetRegisteredClientMachine");

            switch (dataProvider.DatabaseType)
            {
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + clientName + "'");
                    sql = sql.Replace("@id", "'" + clientId + "'");
                    break;

                case DatabaseType.Oracle:
					sql = sql.Replace(":name", "'" + clientName + "'");
                    sql = sql.Replace(":id", "'" + clientId + "'");
                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        machine = reader.GetString(0);
                        if (machine.Length == 0)
                        {
                            machine = null;
                        }
                    }
                }

                return machine;
            }
            catch (Exception ex)
            {
                throw new DBException("Failed to get registered client machine", ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

		/// <summary>
		/// Add a registered client to database table.
		/// </summary>
		/// <param name="clientName">The client name.</param>
		/// <param name="clientId">The client id</param>
        /// <param name="machineName">The name of the client machine</param>
		public void AddRegisteredClient(string clientName, string clientId, string machineName)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("AddRegisteredClient");

			try
			{
				switch (dataProvider.DatabaseType)
				{
					case DatabaseType.MySql:
					case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@name", "'" + clientName + "'");
                        sql = sql.Replace("@id", "'" + clientId + "'");
                        sql = sql.Replace("@machine", "'" + machineName + "'");
						break;

					case DatabaseType.Oracle:
						sql = sql.Replace(":name", "'" + clientName + "'");
                        sql = sql.Replace(":id", "'" + clientId + "'");
                        sql = sql.Replace(":machine", "'" + machineName + "'");
						break;
				}

				cmd.CommandText = sql;

				cmd.ExecuteNonQuery();

				tran.Commit();
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
		}

		/// <summary>
		/// Remove a registered client from database table.
		/// </summary>
		/// <param name="clientName">The client name.</param>
		/// <param name="clientId">The client id</param>
		public void RemoveRegisteredClient(string clientName, string clientIdText)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("RemoveRegisteredClient");

			try
			{
                // get id part from text
                string clientId = clientIdText;
                int pos = clientIdText.IndexOf(':');
                if (pos > 0)
                {
                    clientId = clientIdText.Substring(0, pos);
                }

				switch (dataProvider.DatabaseType)
				{
					case DatabaseType.MySql:
					case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@name", "'" + clientName + "'");
                        sql = sql.Replace("@id", "'" + clientId + "'");
						break;

					case DatabaseType.Oracle:
						sql = sql.Replace(":name", "'" + clientName + "'");
                        sql = sql.Replace(":id", "'" + clientId + "'");
						break;
				}

				cmd.CommandText = sql;

				cmd.ExecuteNonQuery();

				tran.Commit();
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
		}

		/// <summary>
		/// Get the license key from the database
		/// </summary>
		/// <returns>license key</returns>
		public string GetLicenseKey()
		{
			string licenseKey = null;
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetLicenseKey");

			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					if (!reader.IsDBNull(0))
					{
						licenseKey = reader.GetString(0).Trim();
					}
				}

				return licenseKey;
			}
			finally
			{
                if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Sets license key to the database
		/// </summary>
		/// <param name="licenseKey">license key</param>
		public void SetLicenseKey(string licenseKey)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("SetLicenseKey");

			try
			{
				switch (dataProvider.DatabaseType)
				{
					case DatabaseType.MySql:
					case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@lic_key", "'" + licenseKey + "'");
						break;

					case DatabaseType.Oracle:
						sql = sql.Replace(":lic_key", "'" + licenseKey + "'");
						break;
				}

				cmd.CommandText = sql;

				cmd.ExecuteNonQuery();

				tran.Commit();
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
		}

        /// <summary>
        /// Delete all pivot layouts associated with a given schema.
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="version">The schema version</param>
        public void DeletePivotLayouts(string schemaName, string version)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("DelPivotLayouts");

            try
            {
                sql = sql.Replace(GetParamName("schemaname", dataProvider), "'" + schemaName + "'");
                sql = sql.Replace(GetParamName("version", dataProvider), "'" + version + "'");

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
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
        }

        /// <summary>
        /// Delete all event infos associated with a given schema.
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="version">The schema version</param>
        public void DeleteEvents(string schemaName, string version)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("DelEvents");

            try
            {
                sql = sql.Replace(GetParamName("schemaname", dataProvider), "'" + schemaName + "'");
                sql = sql.Replace(GetParamName("version", dataProvider), "'" + version + "'");

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
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
        }

        /// <summary>
        /// Gets the last checked time of a given timer event
        /// </summary>
        /// <param name="schemaName">The schema name of the event.</param>
        /// <param name="schemaVersion">The schema version of the event.</param>
        /// <param name="className">The class name of the event</param>
        /// <param name="eventName">The event name</param>
        /// <returns>A datetime object, DateTime.MinValue if the record doesn't exist in database</returns>
        public DateTime GetEventLastCheckedTime(string schemaName, string schemaVersion, string className, string eventName)
        {
            DateTime lastCheckedTime = DateTime.MinValue;

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetEventLastCheckedTime");
            switch (dataProvider.DatabaseType)
            {
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@schemaname", "'" + schemaName + "'");
                    sql = sql.Replace("@version", "'" + schemaVersion + "'");
                    sql = sql.Replace("@classname", "'" + className + "'");
                    sql = sql.Replace("@eventname", "'" + eventName + "'");

                    break;

                case DatabaseType.Oracle:
					sql = sql.Replace(":schemaname", "'" + schemaName + "'");
                    sql = sql.Replace(":version", "'" + schemaVersion + "'");
                    sql = sql.Replace(":classname", "'" + className + "'");
                    sql = sql.Replace(":eventname", "'" + eventName + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        lastCheckedTime = DateTime.Parse(reader[0].ToString());
                    }
                }

                return lastCheckedTime;
            }
            catch (Exception ex)
            {
                throw new DBException("Failed to get last checked time of an event.", ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Sets last checked time of an event to database.
        /// </summary>
        /// <param name="schemaName">The schema name of the event.</param>
        /// <param name="schemaVersion">The schema version of the event.</param>
        /// <param name="className">The class name of the event</param>
        /// <param name="eventName">The event name</param>
        /// <param name="lastCheckedTime">The last checked time of event</param>
        public void SetEventLastCheckedTime(string schemaName, string schemaVersion, string className, string eventName, DateTime lastCheckedTime)
        {
            DateTime existingCheckedTime = GetEventLastCheckedTime(schemaName, schemaVersion, className, eventName);

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = null;
            if (existingCheckedTime > DateTime.MinValue)
            {
                sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("UpdateEventLastCheckedTime");
            }
            else
            {
                sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("AddEventLastCheckedTime");
            }

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
            string lastCheckedTimeStr = lookup.GetTimestampFunc(lastCheckedTime.ToString("s"), LocaleInfo.Instance.DateTimeFormat);

            try
            {
                switch (dataProvider.DatabaseType)
                {
					case DatabaseType.MySql:
					case DatabaseType.SQLServer:
                    case DatabaseType.SQLServerCE:
                        sql = sql.Replace("@schemaname", "'" + schemaName + "'");
                        sql = sql.Replace("@version", "'" + schemaVersion + "'");
                        sql = sql.Replace("@classname", "'" + className + "'");
                        sql = sql.Replace("@eventname", "'" + eventName + "'");
                        sql = sql.Replace("@checkedtime", lastCheckedTimeStr);

                        break;

                    case DatabaseType.Oracle:
						sql = sql.Replace(":schemaname", "'" + schemaName + "'");
                        sql = sql.Replace(":version", "'" + schemaVersion + "'");
                        sql = sql.Replace(":classname", "'" + className + "'");
                        sql = sql.Replace(":eventname", "'" + eventName + "'");
                        sql = sql.Replace(":checkedtime", lastCheckedTimeStr);

                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
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
        }

        /// <summary>
        /// Gets the modified time of the given schema
        /// </summary>
        /// <param name="schemaInfo">The schema info to check</param>
        /// <exception cref="DBException">If a database access error occurs.</exception>		
        private void GetModifiedTime(SchemaInfo schemaInfo)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();
            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetMetaModelModifiedTime");

            switch (dataProvider.DatabaseType)
            {
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    sql = sql.Replace("@name", "'" + schemaInfo.Name + "'");
                    sql = sql.Replace("@version", "'" + schemaInfo.Version + "'");

                    break;

                case DatabaseType.Oracle:
					sql = sql.Replace(":name", "'" + schemaInfo.Name + "'");
                    sql = sql.Replace(":version", "'" + schemaInfo.Version + "'");

                    break;
            }

            try
            {
                cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        schemaInfo.ModifiedTime = DateTime.Parse(reader["MODIFIED_TIME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DBException("Failed to get modified time of meta-data model.", ex);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }
        }

		/// <summary>
		/// Adds or refreshess schema model in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with schema</param>
		private void FillSchema(MetaDataModel metaData)
		{
			// get a stream to read schema model from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.SchemaModel.Read(clobDAO.ReadClob(MM_SCHEMA, XML_SCHEMA, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
                // replace the outdated schema id with the current one, since the schema id is re-created
                // at schema restore time
                metaData.SchemaModel.SchemaInfo.ID = metaData.SchemaInfo.ID;
			}

			// Fill in the table and column names, not needed anymore
			// (No longer needed)
			//FillTableNames(metaData.SchemaModel);
			//FillColumnNames(metaData.SchemaModel);
		}
	
		/// <summary>
		/// Adds or refreshess data views in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with data views</param>
		private void FillDataViews(MetaDataModel metaData)
		{
			// get a stream to read schema model from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.DataViews.Read(clobDAO.ReadClob(MM_SCHEMA, DATA_VIEWS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
			}
		}

		/// <summary>
		/// Adds or refreshess xacl policy in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with xacl policy</param>
		private void FillXaclPolicy(MetaDataModel metaData)
		{
			// get a stream to read schema model from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.XaclPolicy.Read(clobDAO.ReadClob(MM_SCHEMA, XACL_POLICY, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
			}
		}
	
		/// <summary>
		/// Adds or refreshess taxonomies in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with taxonomies</param>
		private void FillTaxonomies(MetaDataModel metaData)
		{
			// get a stream to read staxonomies from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.Taxonomies.Read(clobDAO.ReadClob(MM_SCHEMA, TAXONOMIES, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
			}
		}

		/// <summary>
		/// Adds or refreshess rules in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with rules</param>
		private void FillRules(MetaDataModel metaData)
		{
			// get a stream to read staxonomies from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.RuleManager.Read(clobDAO.ReadClob(MM_SCHEMA, RULES, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
			}
		}

		/// <summary>
		/// Adds or refreshess mappings in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with mappings</param>
		private void FillMappings(MetaDataModel metaData)
		{
			// get a stream to read staxonomies from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.MappingManager.Read(clobDAO.ReadClob(MM_SCHEMA, MAPPINGS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
			}
		}

		/// <summary>
		/// Adds or refreshess selectors in the MetaDataModel to match those in the
		/// data source.
		/// </summary>
		/// <param name="metaData">A MetaDataModel to fill with selectors</param>
		private void FillSelectors(MetaDataModel metaData)
		{
			// get a stream to read staxonomies from
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

			using (clobDAO)
			{
				metaData.SelectorManager.Read(clobDAO.ReadClob(MM_SCHEMA, SELECTORS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
			}
		}

        /// <summary>
        /// Adds or refreshess events in the MetaDataModel to match those in the
        /// data source.
        /// </summary>
        /// <param name="metaData">A MetaDataModel to fill with events</param>
        private void FillEvents(MetaDataModel metaData)
        {
            // get a stream to read evnets from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                metaData.EventManager.Read(clobDAO.ReadClob(MM_SCHEMA, EVENTS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
            }
        }

        /// <summary>
        /// Adds or refreshess logging policy in the MetaDataModel to match those in the
        /// data source.
        /// </summary>
        /// <param name="metaData">A MetaDataModel to fill with logging policy</param>
        private void FillLogging(MetaDataModel metaData)
        {
            // get a stream to read evnets from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                metaData.LoggingPolicy.Read(clobDAO.ReadClob(MM_SCHEMA, LOGGING, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
            }
        }

        /// <summary>
        /// Adds or refreshess subscribers in the MetaDataModel to match those in the
        /// data source.
        /// </summary>
        /// <param name="metaData">A MetaDataModel to fill with subscribers</param>
        private void FillSubscribers(MetaDataModel metaData)
        {
            // get a stream to read subscribers from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                metaData.SubscriberManager.Read(clobDAO.ReadClob(MM_SCHEMA, SUBSCRIBERS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
            }
        }

        /// <summary>
        /// Adds or refresh xmlschema views in the MetaDataModel to match those in the
        /// data source.
        /// </summary>
        /// <param name="metaData">A MetaDataModel to fill with xmlschema views</param>
        private void FillXMLSchemaViews(MetaDataModel metaData)
        {
            // get a stream to read staxonomies from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                metaData.XMLSchemaViews.Read(clobDAO.ReadClob(MM_SCHEMA, XMLSCHEMAVIEWS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
            }
        }

        /// <summary>
        /// Adds or refreshess apis in the MetaDataModel to match those in the
        /// data source.
        /// </summary>
        /// <param name="metaData">A MetaDataModel to fill with apis</param>
        private void FillApis(MetaDataModel metaData)
        {
            // get a stream to read subscribers from
            IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);

            using (clobDAO)
            {
                metaData.ApiManager.Read(clobDAO.ReadClob(MM_SCHEMA, APIS, metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
            }
        }

        /// <summary>
        /// Gets the physical table name for each class.
        /// </summary>
        /// <param name="schemaModel">The schema model to be filled.</param>
        private void FillTableNames(SchemaModel schemaModel)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;

			try
			{
				string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetTableName");
			
				cmd.CommandText = sql;
				IDbDataParameter parameter = cmd.CreateParameter();
				parameter.ParameterName = "id";
				parameter.DbType = DbType.Int32;
				parameter.Value = schemaModel.SchemaInfo.ID;
				cmd.Parameters.Add(parameter);

				reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					string className = reader.GetString(0);
					string tableName = reader.GetString(1);
					ClassElement classElement = schemaModel.FindClass(className);
					if (classElement == null)
					{
						throw new InvalidClassNameException("The class " + className + " does not exist.");
					}

					classElement.TableName = tableName;
				}
			}
			catch (Exception ex)
			{
				throw new DBException("Failed to fill table names for schema model:" + schemaModel.SchemaInfo.Name + "/" + schemaModel.SchemaInfo.Version, ex);
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Gets the physical column names for attributes.
		/// </summary>
		/// <param name="schemaModel">The schema model to be filled.</param>
		private void FillColumnNames(SchemaModel schemaModel)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			IDbDataParameter parameter;
			ClassElement classElement;

			try
			{
				string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetColumnName");
			
				// create parameters
				cmd.CommandText = sql;
				parameter = cmd.CreateParameter();
				parameter.ParameterName = "id";
				parameter.DbType = DbType.Int32;
				parameter.Value = schemaModel.SchemaInfo.ID;
				cmd.Parameters.Add(parameter);

				reader = cmd.ExecuteReader();

				AttributeElementBase found;

				while (reader.Read())
				{
					string className = reader.GetString(0);
					string attributeName = reader.GetString(1);
					// HACK some rows do not have data for this column
					string columnName = null;
					if (!reader.IsDBNull(2))
					{
						columnName = reader.GetString(2);
					}
					
					// find the class element
					classElement = schemaModel.FindClass(className);

					// the attribute name can be that of a simple attribute or relationship attribute
					found = classElement.FindSimpleAttribute(attributeName);

					if (found == null)
					{
						found = classElement.FindRelationshipAttribute(attributeName);
					}

					if (found == null)
					{
						throw new DBException("Invalid attribute name " + attributeName);
					}

					found.ColumnName = columnName;
				}
			}
			catch (Exception ex)
			{
				throw new DBException("Failed to fill column names for schema model:" + schemaModel.SchemaInfo.Name + "/" + schemaModel.SchemaInfo.Version, ex);
			}
			finally
			{
                if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Create a MemoryStream to store the image of icon specified by the filePath
		/// </summary>
		/// <param name="filePath">The file path</param>
		/// <returns>A Stream object</returns>
		private Stream GetMemoryStream(string filePath)
		{
			string fullPath = NewteraNameSpace.GetAppHomeDir() + MetaDataAdapter.CONFIG_DIR + filePath;
			MemoryStream outStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(outStream);
			BinaryReader reader;

			if (File.Exists(fullPath))
			{
				using (Stream inStream = File.OpenRead(fullPath))
				{
					reader = new BinaryReader(inStream);
					int actual = 0;
					byte[] buffer = new byte[200];
					while((actual = reader.Read(buffer, 0/*buffer offset*/, buffer.Length/*count*/)) >0)
					{
						writer.Write(buffer, 0, actual); // a unicode counts two bytes
						writer.Flush();
					}
				}
			}

			return outStream;
		}

        /// <summary>
        /// Get the appropriate parameter name for the specific database type
        /// </summary>
        /// <param name="name">The bare parameter name.</param>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns>The parameter name</returns>
        private string GetParamName(string name, IDataProvider dataProvider)
        {
            string param;

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.Oracle:
					param = ":" + name;
                    break;
				case DatabaseType.MySql:
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
                    param = "@" + name;
                    break;
                default:
                    param = ":" + name;
                    break;
            }

            return param;
        }
	}
}