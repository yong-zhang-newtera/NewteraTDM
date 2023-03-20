/*
* @(#) OracleClobDAO.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Data;
	using System.Collections;
	using System.IO;
	using System.Data.OracleClient;

	using Newtera.Common.Core;
	using Newtera.Common.Attachment;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// Oracle implementation of IClobDAO interface.
	/// </summary>
	/// <version> 	1.0.1	12 Jul 2003 </version>
	/// <author> 	Yong Zhang </author>
	internal class OracleClobDAO : IClobDAO, IDisposable
	{
		public const string PATH = @"\temp\";

		private OracleProvider _dataProvider;
		private OracleConnection _connection;
		private Stream _fileStream;
		private string _fileName;

		/// <summary>
		/// Initiate a new instance of OracleClobDAO.
		/// </summary>
		/// <param name="dataProvider">data provider</param>
		public OracleClobDAO(IDataProvider dataProvider)
		{
			_dataProvider = (OracleProvider) dataProvider;
			_connection = null;
			_fileName = null;
			_fileStream = null;
		}

		~OracleClobDAO()
		{
			if (_fileStream != null)
			{
				_fileStream.Close();
				_fileStream = null;
			}

			if (_fileName != null)
			{
				// delete the temp file
				File.Delete(_fileName);
				_fileName = null;
			}

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
		/// <param name="rowId">The id identifies the row</param>
		/// <returns>A Clob Stream</returns>
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
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("id", rowId);

				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        OracleLob oracleLob = reader.GetOracleLob(0);
                        if (!oracleLob.IsNull)
                        {
                            stream = oracleLob;
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

			// We do not want to load the schema into a string since it can consume
			// lots of memory, we first write it to a temp file and return a file stream
			// back.
			//return GetFileStream(stream, rowId);

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
			OracleLob stream = null;
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobById", parameters);
			
			try
			{
				if (_connection == null)
				{
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("id", rowId);

				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        stream = reader.GetOracleLob(0);
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

			// Get a text string from the stream
            return GetText(stream);
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
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add("name", OracleType.VarChar, 20).Value = schemaName.ToUpper();
				cmd.Parameters.Add("version", OracleType.VarChar, 20).Value = schemaVersion;
				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        OracleLob oraLob = reader.GetOracleLob(0);
                        // if the lob can be empty, return a null stream
                        if (oraLob.Length > 0)
                        {
                            stream = oraLob;
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

			// We do not want to load the schema into a string since it can consume
			// lots of memory, we first write it to a temp file and return a file stream
			// back.
			//return GetFileStream(stream, columnName + "_" + schemaName + schemaVersion);

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
			OracleLob stream = null;
			
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobByName", parameters);
			
			try
			{
				if (_connection == null)
				{
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();
				cmd.CommandText = sql;
				cmd.Parameters.Add("name", OracleType.VarChar, 20).Value = schemaName.ToUpper();
				cmd.Parameters.Add("version", OracleType.VarChar, 20).Value = schemaVersion;
				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        stream = reader.GetOracleLob(0);
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

			if (stream == null)
			{
				throw new DBException("Unable to read the clob for schema " + schemaName + "/" + schemaVersion);
			}

			// Get a text string from the stream
            return GetText(stream);
		}

		/// <summary>
		/// Read from a clob column as text from a DataReader.
		/// </summary>
		/// <param name="dataReader">The DataReader instance.</param>
		/// <param name="columnIndex">The zero-based column index.</param>
		/// <returns></returns>
		public string ReadClobAsText(IDataReader dataReader, int columnIndex)
		{
			OracleLob stream = null;
			OracleDataReader oracleDataReader = (OracleDataReader) dataReader;
			string val = "";

			try
			{	
				stream = oracleDataReader.GetOracleLob(columnIndex);

				if (stream != null && !stream.IsNull)
				{
					// Get a text string from the stream
					StringBuilder builder = new StringBuilder();

					int actual = 0;
					StreamReader streamReader = new StreamReader(stream, Encoding.Unicode);
					char[] cbuffer = new char[100];
					while((actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) >0)
					{
						builder.Append(cbuffer, 0, actual);
					}

					val = builder.ToString();
				}
			}
			catch (Exception)
			{
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
			StreamReader streamReader = new StreamReader(stream, Encoding.Unicode);
			
			WriteClob(streamReader, tableName, columnName, schemaName, schemaVersion);
		}

		/// <summary>
		/// Write to a clob column.
		/// </summary>
		/// <param name="xmlString">The xml string that is written to a clob.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="recordId">The id identifies the row</param>
		public void WriteClob(string xmlString, string tableName, string columnName, string schemaName, string schemaVersion)
		{
			StringReader stringReader = new StringReader(xmlString);
			
			WriteClob(stringReader, tableName, columnName, schemaName, schemaVersion);
		}

		/// <summary>
		/// Write to a clob column.
		/// </summary>
		/// <param name="textReader">A text reader providing data to write.</param>
		/// <param name="tableName">The table name of clob column </param>
		/// <param name="columnName">The name of clob column</param>
		/// <param name="recordId">The id identifies the row</param>
		private void WriteClob(TextReader textReader, string tableName, string columnName, string schemaName, string schemaVersion)
		{
			if (_connection == null)
			{
				_connection = (OracleConnection) _dataProvider.Connection;
			}

			OracleCommand cmd = _connection.CreateCommand();

			//Note: Updating LOB data requires a transaction.
			cmd.Transaction = cmd.Connection.BeginTransaction();
    
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClearClobByName", parameters);

			string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobByNameForUpdate", parameters);
			
			try
			{
				// clear the old content in the clob
				cmd.CommandText = clearClobSql;
				cmd.Parameters.Add("name", OracleType.VarChar, 20).Value = schemaName.ToUpper();
				cmd.Parameters.Add("version", OracleType.VarChar, 20).Value = schemaVersion;
				cmd.ExecuteNonQuery();

				cmd.CommandText = updateClobSql;
                OracleDataReader reader = cmd.ExecuteReader();

                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();
                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    StreamWriter streamWriter = new StreamWriter(clob, Encoding.Unicode);
                    int actual = 0;
                    char[] cbuffer = new char[100];
                    while ((actual = textReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                    {
                        streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                        streamWriter.Flush();
                    }
                }

                //Commit the transaction now that everything succeeded.
                //Note: On error, Transaction.Dispose is called (from the using statement)
                //and will automatically roll-back the pending transaction.
                cmd.Transaction.Commit();
			}
			catch (Exception se)
			{
                cmd.Transaction.Rollback();

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
        /// <param name="text">The text string that is written to a clob.</param>
        /// <param name="tableName">The table name of clob column </param>
        /// <param name="rowId">The id that identifies a row</param>
        public void WriteClob(string text, string tableName, string columnName, string rowId)
        {
            StringReader stringReader = new StringReader(text);

            if (_connection == null)
            {
                _connection = (OracleConnection)_dataProvider.Connection;
            }

            OracleCommand cmd = _connection.CreateCommand();

            //Note: Updating LOB data requires a transaction.
            cmd.Transaction = cmd.Connection.BeginTransaction();

            Hashtable parameters = new Hashtable();
            parameters["TABLE_NAME"] = tableName;
            parameters["COLUMN_NAME"] = columnName;
            string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClearClobById", parameters);

            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobByIdForUpdate", parameters);

            try
            {
                // clear the old content in the clob
                cmd.CommandText = clearClobSql;
                ((OracleCommand)cmd).Parameters.Add("id", OracleType.Number).Value = rowId;
                cmd.ExecuteNonQuery();

                if (text != null && text.Length > 0)
                {
                    cmd.CommandText = updateClobSql;

                    OracleDataReader reader = cmd.ExecuteReader();
                    using (reader)
                    {
                        //Obtain the first row of data.
                        reader.Read();
                        //Obtain a LOB.
                        OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                        StreamWriter streamWriter = new StreamWriter(clob, Encoding.Unicode);
                        int actual = 0;
                        char[] cbuffer = new char[100];
                        while ((actual = stringReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                        {
                            streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                            streamWriter.Flush();
                        }
                    }
                }

                //Commit the transaction now that everything succeeded.
                //Note: On error, Transaction.Dispose is called (from the using statement)
                //and will automatically roll-back the pending transaction.
                cmd.Transaction.Commit();
            }
            catch (Exception se)
            {
                cmd.Transaction.Rollback();
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
		public void WriteClob(IDbCommand cmd, string dataString, string tableName,
			string columnName, string objId)
		{
			StringReader stringReader = new StringReader(dataString);
			Hashtable parameters = new Hashtable();
			parameters["TABLE_NAME"] = tableName;
			parameters["COLUMN_NAME"] = columnName;
			string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClearClobByObjId", parameters);

			string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClobByObjIdForUpdate", parameters);
			
			// clear the old content in the clob
			cmd.CommandText = clearClobSql;
            ((OracleCommand)cmd).Parameters.Clear();
			((OracleCommand) cmd).Parameters.Add("oid", OracleType.Number).Value = objId;
			cmd.ExecuteNonQuery();

			if (dataString != SQLElement.VALUE_NULL)
			{
				cmd.CommandText = updateClobSql;
                OracleDataReader reader = ((OracleCommand)cmd).ExecuteReader();
                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();

                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    StreamWriter streamWriter = new StreamWriter(clob, Encoding.Unicode);
                    int actual = 0;
                    char[] cbuffer = new char[100];
                    while ((actual = stringReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                    {
                        streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                        streamWriter.Flush();
                    }
                }
			}
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
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            int bufferSize = 1024;                   // Size of the BLOB buffer.
            byte[] buffer = new byte[bufferSize];
            int actual = 0;

            try
            {
                Hashtable parameters = new Hashtable();
                parameters["TABLE_NAME"] = tableName;
                parameters["COLUMN_NAME"] = columnName;
                parameters["ID_NAME"] = idName;
                string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetBlobById", parameters);
                if (_connection == null)
                {
                    _connection = (OracleConnection)_dataProvider.Connection;
                }

                OracleCommand cmd = _connection.CreateCommand();

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue(":id", rowId);

                OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        OracleLob oracleLob = reader.GetOracleLob(0/*0:based ordinal*/);
                        if (!oracleLob.IsNull)
                        {
                            while ((actual = oracleLob.Read(buffer, 0/*buffer offset*/, bufferSize/*count*/)) > 0)
                            {
                                bw.Write(buffer, 0, actual);
                                bw.Flush();
                            }
                        }
                    }
                }

                return stream.ToArray();
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
                _connection = (OracleConnection) _dataProvider.Connection;
            }

            Hashtable parameters = new Hashtable();
            parameters["TABLE_NAME"] = tableName;
            parameters["COLUMN_NAME"] = columnName;
            parameters["ID_NAME"] = idName;

            string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("ClearBlobById", parameters);
            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetBlobByIdForUpdate", parameters);

            OracleCommand cmd = _connection.CreateCommand();

            //Note: Updating LOB data requires a transaction.
            cmd.Transaction = cmd.Connection.BeginTransaction();

            try
            {
                // clear the old content in the blob
                cmd.CommandText = clearClobSql;
                cmd.Parameters.AddWithValue("id", rowId);
                cmd.ExecuteNonQuery();

                cmd.CommandText = updateClobSql;
                cmd.Parameters.AddWithValue("id", rowId);
                OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();

                    MemoryStream stream = new MemoryStream(data);

                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    int actual = 0;
                    byte[] buffer = new byte[10240];
                    while ((actual = stream.Read(buffer, 0/*buffer offset*/, buffer.Length/*count*/)) > 0)
                    {
                        clob.Write(buffer, 0, actual);
                        clob.Flush();
                    }
                }

                //Commit the transaction now that everything succeeded.
                //Note: On error, Transaction.Dispose is called (from the using statement)
                //and will automatically roll-back the pending transaction.
                cmd.Transaction.Commit();
            }
            catch (Exception se)
            {
                cmd.Transaction.Rollback();
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
		/// <param name="attachmentType">One of the AttachmentType enum</param>
		/// <param name="itemId">The id of the item that an attachment belongs to.</param>
		/// <param name="attachName">The unique name of an attachment</param>
		/// <returns>A blob stream</returns>
		public Stream ReadAttachmentBlob(AttachmentType attachmentType, string itemId, string attachName)
		{
			System.IO.Stream stream = null;
			
			try
			{
				if (_connection == null)
				{
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();

				switch (attachmentType)
				{
					case AttachmentType.Instance:
						cmd.CommandText = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentBlob");
                        cmd.Parameters.AddWithValue("oid", itemId);
                        cmd.Parameters.AddWithValue("name", attachName);
						break;

					case AttachmentType.Class:
						cmd.CommandText = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentBlob");
                        cmd.Parameters.AddWithValue("cid", itemId);
                        cmd.Parameters.AddWithValue("name", attachName);
						break;
				}

				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        OracleLob oracleLob = reader.GetOracleLob(0/*0:based ordinal*/);
                        if (!oracleLob.IsNull)
                        {
                            stream = oracleLob;
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
		/// Read an attachment Blob into a provided stream.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum</param>
		/// <param name="stream">The provided stream</param>
		/// <param name="itemId">The id of the item that an attachment belongs to.</param>
		/// <param name="attachName">The unique name of an attachment</param>
		/// <returns>A blob stream</returns>
		public void ReadAttachmentBlob(AttachmentType attachmentType, Stream stream, string itemId, string attachName)
		{
			BinaryWriter bw = new BinaryWriter(stream);
			int bufferSize = 1024;                   // Size of the BLOB buffer.
			byte[] buffer = new byte[bufferSize];
			long retval;                            // The bytes returned from GetBytes.
			long startIndex = 0;                    // The starting position in the BLOB output.
			
			try
			{
				if (_connection == null)
				{
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();

				switch (attachmentType)
				{
					case AttachmentType.Instance:
						cmd.CommandText = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentBlob");
                        cmd.Parameters.AddWithValue("oid", itemId);
                        cmd.Parameters.AddWithValue("name", attachName);
						break;

					case AttachmentType.Class:
						cmd.CommandText = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentBlob");
                        cmd.Parameters.AddWithValue("cid", itemId);
                        cmd.Parameters.AddWithValue("name", attachName);
						break;
				}

				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        // Reset the starting byte for the new BLOB.
                        startIndex = 0;

                        // Read the bytes into buffer[] and retain the number of bytes returned.
                        retval = reader.GetBytes(0, startIndex, buffer, 0, bufferSize);

                        // Continue reading and writing while there are bytes beyond the size of the buffer.
                        while (retval == bufferSize)
                        {
                            bw.Write(buffer);
                            bw.Flush();

                            // Reposition the start index to the end of the last buffer and fill the buffer.
                            startIndex += bufferSize;
                            retval = reader.GetBytes(0, startIndex, buffer, 0, bufferSize);
                        }

                        // Write the remaining buffer.
                        if (retval > 0)
                        {
                            bw.Write(buffer, 0, (int)retval - 1);
                            bw.Flush();
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
		}

		/// <summary>
		/// Read an attachment Blob.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTypeEnum values.</param>
		/// <param name="attachmentId">The attachment id.</param>
		/// <returns>A blob stream</returns>
		public Stream ReadAttachmentBlob(AttachmentType attachmentType, string attachmentId)
		{
			System.IO.Stream stream = null;
			string sql = null;
			
			switch (attachmentType)
			{
				case AttachmentType.Instance:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentBlobById");
					break;

				case AttachmentType.Class:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentBlobById");
					break;
			}

			try
			{
				if (_connection == null)
				{
					_connection = (OracleConnection) _dataProvider.Connection;
				}

				OracleCommand cmd = _connection.CreateCommand();

				cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("id", attachmentId);

				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        OracleLob oracleLob = reader.GetOracleLob(0/*0:based ordinal*/);
                        if (!oracleLob.IsNull)
                        {
                            stream = oracleLob;
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
		/// Write an attachment as Blob to database.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="stream">A stream providing attachment data to write.</param>
		/// <param name="attachmentId">The attachment id</param>
		/// <param name="attachName">The unique name of an attachment</param>
		public void WriteAttachmentBlob(AttachmentType attachmentType, Stream stream, string attachmentId)
		{
			BinaryReader streamReader = new BinaryReader(stream);

			if (_connection == null)
			{
				_connection = (OracleConnection) _dataProvider.Connection;
			}

			OracleCommand cmd = _connection.CreateCommand();

			//Note: Updating LOB data requires a transaction.
			cmd.Transaction = cmd.Connection.BeginTransaction();
    
			string clearClobSql = null;
			string updateClobSql = null;
			
			switch (attachmentType)
			{
				case AttachmentType.Instance:
					clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("ClearAttachmentBlob");
					updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentBlobForUpdate");
					break;

				case AttachmentType.Class:
					clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("ClearClsAttachmentBlob");
					updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentBlobForUpdate");
					break;
			}

			try
			{
				// clear the old content in the blob
				cmd.CommandText = clearClobSql;
                cmd.Parameters.AddWithValue("id", attachmentId);
				cmd.ExecuteNonQuery();

				cmd.CommandText = updateClobSql;
                cmd.Parameters.AddWithValue("id", attachmentId);
				OracleDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();

                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    int actual = 0;
                    byte[] buffer = new byte[10240];
                    while ((actual = streamReader.Read(buffer, 0/*buffer offset*/, buffer.Length/*count*/)) > 0)
                    {
                        clob.Write(buffer, 0, actual);
                        clob.Flush();
                    }
                }

				//Commit the transaction now that everything succeeded.
				//Note: On error, Transaction.Dispose is called (from the using statement)
				//and will automatically roll-back the pending transaction.
				cmd.Transaction.Commit();
			}
			catch (Exception se)
			{
                cmd.Transaction.Rollback();

				if (_connection != null)
				{
					_connection.Close();
					_connection = null;
				}
				throw se;
			}
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
				_connection = (OracleConnection) _dataProvider.Connection;
			}

			OracleCommand cmd = _connection.CreateCommand();

			//Note: Updating LOB data requires a transaction.
			cmd.Transaction = cmd.Connection.BeginTransaction();

			StringReader stringReader = new StringReader(xml);

			string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClearChartInfoClobById");
			string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetChartInfoClobByIdForUpdate");
			
			try
			{
				// clear the old content in the clob
				cmd.CommandText = clearClobSql;
				cmd.Parameters.Add("id", OracleType.VarChar, 100).Value = chartId;
				cmd.ExecuteNonQuery();

				cmd.CommandText = updateClobSql;
				OracleDataReader reader = ((OracleCommand) cmd).ExecuteReader();
                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();
                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    StreamWriter streamWriter = new StreamWriter(clob, Encoding.Unicode);
                    int actual = 0;
                    char[] cbuffer = new char[100];
                    while ((actual = stringReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                    {
                        streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                        streamWriter.Flush();
                    }
                }

				//Commit the transaction now that everything succeeded.
				//Note: On error, Transaction.Dispose is called (from the using statement)
				//and will automatically roll-back the pending transaction.
				cmd.Transaction.Commit();
			}
			catch (Exception se)
			{
                cmd.Transaction.Rollback();

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
                _connection = (OracleConnection)_dataProvider.Connection;
            }

            OracleCommand cmd = _connection.CreateCommand();

            //Note: Updating LOB data requires a transaction.
            cmd.Transaction = cmd.Connection.BeginTransaction();

            StringReader stringReader = new StringReader(xml);

            string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClearChartTemplateClobById");
            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetChartTemplateClobByIdForUpdate");

            try
            {
                // clear the old content in the clob
                cmd.CommandText = clearClobSql;
                cmd.Parameters.Add("id", OracleType.VarChar, 100).Value = templateId;
                cmd.ExecuteNonQuery();

                cmd.CommandText = updateClobSql;
                OracleDataReader reader = ((OracleCommand)cmd).ExecuteReader();
                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();
                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    StreamWriter streamWriter = new StreamWriter(clob, Encoding.Unicode);
                    int actual = 0;
                    char[] cbuffer = new char[100];
                    while ((actual = stringReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                    {
                        streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                        streamWriter.Flush();
                    }
                }

                //Commit the transaction now that everything succeeded.
                //Note: On error, Transaction.Dispose is called (from the using statement)
                //and will automatically roll-back the pending transaction.
                cmd.Transaction.Commit();
            }
            catch (Exception se)
            {
                cmd.Transaction.Rollback();

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
                _connection = (OracleConnection)_dataProvider.Connection;
            }

            OracleCommand cmd = _connection.CreateCommand();

            //Note: Updating LOB data requires a transaction.
            cmd.Transaction = cmd.Connection.BeginTransaction();

            StringReader stringReader = new StringReader(xml);

            string clearClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClearPivotLayoutClobById");
            string updateClobSql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetPivotLayoutClobByIdForUpdate");

            try
            {
                // clear the old content in the clob
                cmd.CommandText = clearClobSql;
                cmd.Parameters.Add("id", OracleType.VarChar, 100).Value = pivotLayoutId;
                cmd.ExecuteNonQuery();

                cmd.CommandText = updateClobSql;
                OracleDataReader reader = ((OracleCommand)cmd).ExecuteReader();
                using (reader)
                {
                    //Obtain the first row of data.
                    reader.Read();
                    //Obtain a LOB.
                    OracleLob clob = reader.GetOracleLob(0/*0:based ordinal*/);
                    StreamWriter streamWriter = new StreamWriter(clob, Encoding.Unicode);
                    int actual = 0;
                    char[] cbuffer = new char[100];
                    while ((actual = stringReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                    {
                        streamWriter.Write(cbuffer, 0, actual); // a unicode counts two bytes
                        streamWriter.Flush();
                    }
                }

                //Commit the transaction now that everything succeeded.
                //Note: On error, Transaction.Dispose is called (from the using statement)
                //and will automatically roll-back the pending transaction.
                cmd.Transaction.Commit();
            }
            catch (Exception se)
            {
                cmd.Transaction.Rollback();

                if (_connection != null)
                {
                    _connection.Close();
                    _connection = null;
                }

                throw se;
            }
        }

		/// <summary>
		/// Write the clob data into a temp file and return the file stream.
		/// </summary>
		/// <param name="clob">The clob stream</param>
		/// <param name="fileName">The unique file name.</param>
		/// <returns>A file stream</returns>
		private Stream GetFileStream(Stream clob, string fileName)
		{
			_fileName = NewteraNameSpace.GetAppHomeDir() + PATH + fileName + ".xml";

			try
			{
				FileStream fs = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.Write);
				StreamWriter streamWriter = new StreamWriter(fs, Encoding.Unicode);

				int actual = 0;
				StreamReader streamReader = new StreamReader(clob, Encoding.Unicode);
				char[] cbuffer = new char[100];
				actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/);
				//HACK, get rid of the encoding attribute of the first line,
				// it caused exception at xml schema parsing time later.
				string remaining = RemoveEncoding(cbuffer);
				streamWriter.Write(remaining);
				streamWriter.Flush();
				while((actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) >0)
				{
					streamWriter.Write(cbuffer, 0, actual);
					streamWriter.Flush();
				}

				streamWriter.Close();
				fs.Close();

				_fileStream = File.OpenRead(_fileName);

				return _fileStream;
			}
			catch (Exception e)
			{
				throw new DBException(e.Message, e);
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
            int txtLength = 0;
            try
            {
                int actual = 0;
                StreamReader streamReader = new StreamReader(clob, Encoding.Unicode);
                char[] cbuffer = new char[100];
                actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/);
                txtLength += actual;
                if (txtLength > 0)
                {
                    //HACK, get rid of the encoding attribute of the first line,
                    // it caused exception at xml schema parsing time later.
                    string remaining = RemoveEncoding(cbuffer);
                    builder.Append(remaining);
                    while ((actual = streamReader.Read(cbuffer, 0/*buffer offset*/, cbuffer.Length/*count*/)) > 0)
                    {
                        builder.Append(cbuffer, 0, actual);
                        txtLength += actual;
                    }
                }

                if (txtLength > 0)
                {
                    return builder.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new DBException(e.Message, e);
            }
		}

		/// <summary>
		/// Remove the encoding attribute from the first line
		/// </summary>
		/// <param name="cbuffer">the buffer</param>
		/// <returns>The remaining string</returns>
		private string RemoveEncoding(char[] cbuffer)
		{
			Regex gex = new Regex("encoding=\"GB2312\"");

			return gex.Replace(new String(cbuffer), "");
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_fileStream != null)
			{
				_fileStream.Close();
				_fileStream = null;
			}

			if (_fileName != null)
			{
				// delete the temp file
				File.Delete(_fileName);
				_fileName = null;
			}

			if (_connection != null)
			{
				_connection.Close();
				_connection = null;
			}
		}

		#endregion
	}
}