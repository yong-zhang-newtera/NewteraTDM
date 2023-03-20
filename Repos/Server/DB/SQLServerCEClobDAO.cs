/*
* @(#) SQLServerCEClobDAO.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;
	using System.Text;
	using System.Collections;
	using System.IO;
    using System.Data.SqlServerCe;

	using Newtera.Common.Core;
	using Newtera.Common.Attachment;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// SQL Server implementation of IClobDAO interface.
	/// </summary>
	/// <version> 	1.0.1	12 Apr 2016 </version>
	internal class SQLServerCEClobDAO : IClobDAO, IDisposable
	{
		private SQLServerCEProvider _dataProvider;
		private SqlCeConnection _connection;

		/// <summary>
		/// Initiate a new instance of SQLServerCEClobDAO.
		/// </summary>
		/// <param name="dataProvider">data provider</param>
		public SQLServerCEClobDAO(IDataProvider dataProvider)
		{
			_dataProvider = (SQLServerCEProvider) dataProvider;
			_connection = null;
		}

		~SQLServerCEClobDAO()
		{
			if (_connection != null)
			{
				_connection.Close();

                _connection = null;
			}
		}

		/// <summary>
		/// Read from a clob column.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="recordId">The id identifies the row</param>
		public Stream ReadClob(string tableName, string columnName, string rowId)
		{
			System.IO.Stream stream = null;
			
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobById", parameters);
			
			try
			{
				// Note: Open the DB connection on demand, some methods in this class
				// uses an external connection.
				if (_connection == null)
				{
					_connection = (SqlCeConnection) _dataProvider.Connection;
				}

				SqlCeCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", rowId);

				// Execute the command as sequential access behavior
				SqlCeDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            string xmlString = reader.GetString(0);

                            // convert the string to an UnicodeEncoding byte array
                            // so that we can return a MemoryStream.
                            // A large meta data xml string usually has size of hundreds
                            // KB, therefore, memory consumption wont be a big issue.
                            byte[] xmlBytes = Encoding.Unicode.GetBytes(xmlString);
                            stream = new MemoryStream(xmlBytes);
                        }
                    }
                }
				
			}
			catch (Exception se)
			{
				if (_connection != null)
				{
					_connection.Close();

                    _connection = null;
				}
				throw se;
			}

			return stream;
		}
		
		/// <summary>
		/// Read from a clob as text.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="rowId">The id identifies the row</param>
		/// <returns>A string</returns>
		public string ReadClobAsText(string tableName, string columnName, string rowId)
		{
			string clobText = null;
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobById", parameters);
			
			try
			{
				if (_connection == null)
				{
					_connection = (SqlCeConnection) _dataProvider.Connection;
				}

				SqlCeCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", rowId);

				// Execute the command
				SqlCeDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            clobText = reader.GetString(0);
                        }
                    }
                }
			}
			catch (Exception se)
			{
				if (_connection != null)
				{
					_connection.Close();

                    _connection = null;
				}

				throw se;
			}

			return clobText;
		}

		/// <summary>
		/// Read from a clob column.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		public Stream ReadClob(string tableName, string columnName, string schemaName, string schemaVersion)
		{
			System.IO.Stream stream = null;
			
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobByName", parameters);
			
			try
			{
				if (_connection == null)
				{
					_connection = (SqlCeConnection) _dataProvider.Connection;
				}

				SqlCeCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add("@name", SqlDbType.NVarChar, 20).Value = schemaName.ToUpper();
				cmd.Parameters.Add("@version", SqlDbType.NVarChar, 20).Value = schemaVersion;
				
				SqlCeDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            string xmlString = reader.GetString(0);

                            // convert the string to an UnicodeEncoding byte array
                            // so that we can return a MemoryStream.
                            // A large meta data xml string usually has size of hundreds
                            // KB, therefore, memory consumption wont be a big issue.
                            byte[] xmlBytes = Encoding.Unicode.GetBytes(xmlString);
                            stream = new MemoryStream(xmlBytes);
                        }
                    }
                }
			}
			catch (Exception se)
			{
				if (_connection != null)
				{
					_connection.Close();
                    _connection = null;
				}

				throw se;
			}

			return stream;
		}

		/// <summary>
		/// Read from a clob column as text.
		/// </summary>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		/// <returns>A string</returns>
		public string ReadClobAsText(string tableName, string columnName, string schemaName, string schemaVersion)
		{
			string clobText = null;
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobByName", parameters);
			
			try
			{
				if (_connection == null)
				{
					_connection = (SqlCeConnection) _dataProvider.Connection;
				}

				SqlCeCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add("@name", SqlDbType.NVarChar, 20).Value = schemaName.ToUpper();
				cmd.Parameters.Add("@version", SqlDbType.NVarChar, 20).Value = schemaVersion;
				
				// Execute the command as sequential access behavior
				SqlCeDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                using (reader)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            clobText = reader.GetString(0);
                        }
                    }
                }
			}
			catch (Exception se)
			{
				if (_connection != null)
				{
					_connection.Close();

                    _connection = null;
				}
				throw se;
			}

			return clobText;
		}

		/// <summary>
		/// Read from a clob column as text from a DataReader.
		/// </summary>
		/// <param name="dataReader">The DataReader instance.</param>
		/// <param name="columnIndex">The zero-based column index.</param>
		/// <returns></returns>
		public string ReadClobAsText(IDataReader dataReader, int columnIndex)
		{
			string val = "";

			try
			{	
				val = dataReader.GetString(columnIndex);
			}
			catch (Exception)
			{
				val = "";
			}
			
			return val;
		}

		/// <summary>
		/// Write to a clob column.
		/// </summary>
		/// <param name="stream">A stream providing data to write.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param> 
		public void WriteClob(Stream stream, string tableName, string columnName, string schemaName, string schemaVersion)
		{
			string xmlString = GetText(stream);
			
			WriteClob(xmlString, tableName, columnName, schemaName, schemaVersion);
		}

		/// <summary>
		/// Write to a clob column.
		/// </summary>
		/// <param name="xmlString">The xml string that is written to a clob.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="schemaName">The schema name</param>
		/// <param name="schemaVersion">The schema version</param>
		public void WriteClob(string xmlString, string tableName, string columnName, string schemaName, string schemaVersion)
		{
			if (_connection == null)
			{
				_connection = (SqlCeConnection) _dataProvider.Connection;
			}

			SqlCeCommand cmd = _connection.CreateCommand();

			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateClobByName", parameters);
			
			try
			{
				cmd.CommandText = updateClobSql;
				cmd.Parameters.Add("@name", SqlDbType.NVarChar,20).Value = schemaName.ToUpper();
				cmd.Parameters.Add("@version", SqlDbType.NVarChar, 20).Value = schemaVersion;
				cmd.Parameters.Add("@text", SqlDbType.NText).Value = xmlString;
				cmd.ExecuteNonQuery();
			}
			catch (Exception se)
			{
				if (_connection != null)
				{
					_connection.Close();
                    _connection = null;
				}
				throw se;
			}			
		}

        /// <summary>
        /// Write a text to a clob column.
        /// </summary>
        /// <param name="text">The text that is written to a clob.</param>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="columnName">The name of clob column</param>
        /// <param name="rowId">The id that identifies a row in the table</param>
        public void WriteClob(string text, string tableName, string columnName, string rowId)
        {
            if (_connection == null)
            {
                _connection = (SqlCeConnection)_dataProvider.Connection;
            }

            SqlCeCommand cmd = _connection.CreateCommand();

            Hashtable parameters = new Hashtable();
            parameters["TABLE_NAME"] = tableName;
            parameters["COLUMN_NAME"] = columnName;
            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateClobById", parameters);

            try
            {
                cmd.CommandText = updateClobSql;
                ((SqlCeCommand)cmd).Parameters.Add("@id", SqlDbType.BigInt, 20).Value = rowId;
                cmd.Parameters.Add("@text", SqlDbType.NText).Value = text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception se)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection = null;
                }
                throw se;
            }
        }

		/// <summary>
		/// Write to a clob column of an database instance indicated by table name,
		/// column name, and obj_id.
		/// </summary>
		/// <param name="cmd">An external provided database command</param>
		/// <param name="dataString">The data string to be written to a clob.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="objId">The schema name</param>
		public void WriteClob(IDbCommand cmd, string dataString, string tableName, string columnName, string objId)
		{
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateClobByObjId", parameters);
			
			if (dataString == SQLElement.VALUE_NULL)
			{
				dataString = "";
			}

			cmd.CommandText = updateClobSql;
            ((SqlCeCommand)cmd).Parameters.Clear();
			((SqlCeCommand) cmd).Parameters.Add("@oid", SqlDbType.BigInt,20).Value = objId;
			((SqlCeCommand) cmd).Parameters.Add("@text", SqlDbType.NText).Value = dataString;
			cmd.ExecuteNonQuery();
		}

        /// <summary>
        /// Read from a blob column.
        /// </summary>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="columnName">The name of clob column</param>
        /// <param name="idName">The name of id column</param>
        /// <param name="rowId">The id identifies the row</param>
        /// <returns>A byte array from the blob</returns>
        public byte[] ReadBlob(string tableName, string columnName, string idName, string rowId)
        {
            try
            {
                Hashtable parameters = new Hashtable();
                parameters["TABLE_NAME"] = tableName;
                parameters["COLUMN_NAME"] = columnName;
                parameters["ID_NAME"] = idName;
                string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetBlobById", parameters);
                if (_connection == null)
                {
                    _connection = (SqlCeConnection)_dataProvider.Connection;
                }

                SqlCeCommand cmd = _connection.CreateCommand();

                cmd.CommandText = sql;
                SqlCeParameter idparameter = new SqlCeParameter();
                idparameter.SqlDbType = SqlDbType.NVarChar;
                idparameter.ParameterName = "id";
                idparameter.Value = rowId;
                cmd.Parameters.Add(idparameter);

                byte[] data = (byte[])cmd.ExecuteScalar();

                return data;
            }
            catch (Exception se)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection = null;
                }
                throw se;
            }
        }

        /// <summary>
        /// Write a stream to a blob column.
        /// </summary>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="columnName">The name of clob column</param>
        /// <param name="idName">The name of id column</param>
        /// <param name="rowId">The row id</param>
        /// <param name="data">The byte array to write to the blob</param>
        public void WriteBlob(string tableName, string columnName, string idName, string rowId, byte[] data)
        {
            if (_connection == null)
            {
                _connection = (SqlCeConnection)_dataProvider.Connection;
            }

            Hashtable parameters = new Hashtable();
            parameters["TABLE_NAME"] = tableName;
            parameters["COLUMN_NAME"] = columnName;
            parameters["ID_NAME"] = idName;
            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("WriteBlobById", parameters);

            SqlCeCommand cmd;

            try
            {
                cmd = _connection.CreateCommand();

                cmd.CommandText = updateClobSql;

                SqlCeParameter parameter = new SqlCeParameter();
                parameter.SqlDbType = SqlDbType.Image;
                parameter.ParameterName = "data";
                parameter.Value = data;
                cmd.Parameters.Add(parameter);

                SqlCeParameter idparameter = new SqlCeParameter();
                idparameter.SqlDbType = SqlDbType.NVarChar;
                idparameter.ParameterName = "id";
                idparameter.Value = rowId;
                cmd.Parameters.Add(idparameter);
                cmd.ExecuteNonQuery();

            }
            catch (Exception se)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection = null;
                }
                throw se;
            }
        }

		/// <summary>
		/// Read an attachment Blob.
		/// </summary>
		/// <param name="attachmentType">One of AttachmentType enum values</param>
		/// <param name="itemId">The id of the item that an attachment belongs to.</param>
		/// <param name="attachName">The unique name of an attachment</param>
		/// <returns>A blob stream</returns>
		public Stream ReadAttachmentBlob(AttachmentType attachmentType, string itemId, string attachName)
		{
            throw new NotImplementedException("Not supported");
        }

		/// <summary>
		/// Read an attachment Blob into a provided stream.
		/// </summary>
		/// <param name="attachmentType">One of AttachmentType enum values</param>
		/// <param name="stream">The provided stream to which write a blob to</param>
		/// <param name="itemId">The id of the item that an attachment belongs to.</param>
		/// <param name="attachName">The unique name of an attachment</param>
		/// <returns>A blob stream</returns>
		public void ReadAttachmentBlob(AttachmentType attachmentType, Stream stream, string itemId, string attachName)
		{
            throw new NotImplementedException("Not supported");
		}

		/// <summary>
		/// Read an attachment Blob.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTypeEnum values.</param>
		/// <param name="attachmentId">The attachment id.</param>
		/// <returns>A blob stream</returns>
		public Stream ReadAttachmentBlob(AttachmentType attachmentType, string attachmentId)
		{
            throw new NotImplementedException("Not supported");
        }

		/// <summary>
		/// Write an attachment as Blob to database.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="stream">A stream providing attachment data to write.</param>
		/// <param name="attchmentId">The attachment id.</param>
		public void WriteAttachmentBlob(AttachmentType attachmentType, Stream stream, string attachmentId)
		{
            throw new NotImplementedException("Not supported");
        }

		/// <summary>
		/// Get xml representing a chart by chart id.
		/// </summary>
		/// <param name="chartId">The unique id that identifies a chart.</param>
		/// <returns>Xml of a chart data.</returns>
		public string ReadChartXml(string chartId)
		{
			return ReadClobAsText("CM_CHART_INFO", "XML", chartId);
		}

		/// <summary>
		/// Save the xml representimng chart data into the repository given the id.
		/// </summary>
		/// <param name="chartId">An unique id for the chart</param>
		/// <param name="xml">Xml for chart data.</param>
		public void WriteChartXml(string chartId, string xml)
		{
			if (_connection == null)
			{
				_connection = (SqlCeConnection) _dataProvider.Connection;
			}

			SqlCeCommand cmd = _connection.CreateCommand();

			string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateChartInfoClobById");
			
			try
			{
				cmd.CommandText = updateClobSql;
				cmd.Parameters.Add("@id", SqlDbType.NVarChar,100).Value = chartId;
				cmd.Parameters.Add("@text", SqlDbType.NText).Value = xml;
				cmd.ExecuteNonQuery();
			}
			catch (Exception se)
			{
				if (_connection != null)
				{
					_connection.Close();
                    _connection = null;
				}
				throw se;
			}			
		}

        /// <summary>
        /// Get xml representing a chart template by template id.
        /// </summary>
        /// <param name="templateId">The unique id that identifies a template.</param>
        /// <returns>Xml of a chart template data.</returns>
        public string ReadChartTemplateXml(string templateId)
        {
            return ReadClobAsText("CM_CHART_TEMP_INFO", "XML", templateId);
        }

        /// <summary>
        /// Save the xml representimng chart template data into the repository given the id.
        /// </summary>
        /// <param name="templateId">An unique id for the chart template</param>
        /// <param name="xml">Xml for chart template data.</param>
        public void WriteChartTemplateXml(string templateId, string xml)
        {
            if (_connection == null)
            {
                _connection = (SqlCeConnection)_dataProvider.Connection;
            }

            SqlCeCommand cmd = _connection.CreateCommand();

            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateChartTemplateClobById");

            try
            {
                cmd.CommandText = updateClobSql;
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 100).Value = templateId;
                cmd.Parameters.Add("@text", SqlDbType.NText).Value = xml;
                cmd.ExecuteNonQuery();
            }
            catch (Exception se)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection = null;
                }
                throw se;
            }			
        }

        /// <summary>
        /// Get xml representing a pivot layout by id.
        /// </summary>
        /// <param name="pivotLayoutId">The unique id that identifies a pivot layout.</param>
        /// <returns>Xml of a pivot layout data.</returns>
        public string ReadPivotLayoutXml(string pivotLayoutId)
        {
            return ReadClobAsText("CM_PIVOT_LAYOUTS", "XML", pivotLayoutId);
        }

        /// <summary>
        /// Save the xml representimng a pivot layout into the repository given the id.
        /// </summary>
        /// <param name="pivotLayoutId">An unique id for the pivot layout.</param>
        /// <param name="xml">Xml for a pivot layout.</param>
        public void WritePivotLayoutXml(string pivotLayoutId, string xml)
        {
            if (_connection == null)
            {
                _connection = (SqlCeConnection)_dataProvider.Connection;
            }

            SqlCeCommand cmd = _connection.CreateCommand();

            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdatePivotLayoutClobById");

            try
            {
                cmd.CommandText = updateClobSql;
                cmd.Parameters.Add("@id", SqlDbType.NVarChar, 100).Value = pivotLayoutId;
                cmd.Parameters.Add("@text", SqlDbType.NText).Value = xml;
                cmd.ExecuteNonQuery();
            }
            catch (Exception se)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection = null;
                }
                throw se;
            }
        }

		/// <summary>
		/// Get a text string from the clob stream.
		/// </summary>
		/// <param name="clob">The clob stream</param>
		/// <returns>A text string</returns>
		private string GetText(Stream clob)
		{
			StringBuilder builder = new StringBuilder();

			try
			{
				int actual = 0;
				StreamReader streamReader = new StreamReader(clob);
				char[] cbuffer = new char[100];
				while((actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) >0)
				{
					builder.Append(cbuffer, 0, actual);
				}

				return builder.ToString();
			}
			catch (Exception e)
			{
				throw new DBException(e.Message, e);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{			
			if (_connection != null)
			{
				_connection.Close();
			}
		}

		#endregion
	}
}