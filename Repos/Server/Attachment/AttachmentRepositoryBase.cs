/*
* @(#) AttachmentRepositoryBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Attachment
{
	using System;
	using System.IO;
	using System.Data;
	using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.Attachment;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Cache;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Server.Engine.Interpreter;

	/// <summary>
	/// Represents a base class for all IAttachmentRepository implementation classes.
	/// </summary>
	/// <version> 1.0.1	10 Jan 2004 </version>
	/// <author>Yong Zhang </author>
	public abstract class AttachmentRepositoryBase : IAttachmentRepository
	{
        private const string UpdateImageColumnQuery = "for $i in document(\"db://{db}.xml?VERSION={version}\")/{class}List/{class} where $i/@obj_id = \"{obj_id}\" return (setText($i/{attribute_name}, \"{attribute_value}\"), updateInstance(document(\"db://{db}.xml?VERSION={version}\"), $i))";

		protected IDataProvider _dataProvider;

		/// <summary>
		/// Instantiate an instance of AttachmentRepositoryBase class.
		/// </summary>
		public AttachmentRepositoryBase()
		{
			_dataProvider = DataProviderFactory.Instance.Create();
		}

		#region IAttachmentRepository methods

		/// <summary>
		/// Set an attachment to a specified instance
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="id">An unique id for the attachment</param>
		/// <param name="stream">A stream from which to read attachment data.</param>
		public abstract void SetAttachment(AttachmentType attachmentType, string id, Stream stream);

		/// <summary>
		/// Delete an attachment from a specified instance
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="instanceId">The id of specified instance.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be deleted.</param>
		/// <param name="schemaInfo">The schema info</param>		
		public abstract void DeleteAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo, SchemaInfo schemaInfo);

		/// <summary>
		/// Get an attachment of a specified instance as a stream.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <returns>A Stream object from which to read data of an attachment</returns>
		public abstract Stream GetAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo);

		/// <summary>
		/// Get an attachment of a specified instance as a buffered byte array.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <returns>A buffered byte array that contains binary data of an attachment</returns>
		public abstract byte[] GetBufferedAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo);

		/// <summary>
		/// Save an attachment to a stream
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be saved.</param>
		/// <param name="stream">The steam to write attachment to.</param>
		public abstract void SaveAttachmentAs(AttachmentType attachmentType, AttachmentInfo attachInfo, Stream stream);

		/// <summary>
		/// Add an attachment info for an attachment that is attached to a specified item
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The attachment information that specifies the attachment.</param>
		/// <param name="schemaInfo">The schema info</param>
		/// <param name="schemaId">The schema id</param>		
		/// <returns>An unique id for the attachment</returns>		
		public string AddAttachmentInfo(AttachmentType attachmentType, AttachmentInfo attachInfo, SchemaInfo schemaInfo, string schemaId)
		{
            if (attachmentType != AttachmentType.Image)
            {
                // assign an unique id to the attachment if it doesn't have a one
                string id = attachInfo.ID;
                if (id == null || id.Length == 0)
                {
                    id = "id_" + Guid.NewGuid().ToString();
                    attachInfo.ID = id;

                    // Create an record in database for storing the information of the attachment
                    InsertAttachmentInfo(attachmentType, attachInfo, schemaId);
                }
                else if (IsAttachmentInfoExist(attachmentType, id))
                {
                    // update the record in cm_attachment table
                    UpdateAttachmentInfo(attachmentType, attachInfo, schemaId);
                }
                else
                {
                    // Create an record in database using the existing id
                    InsertAttachmentInfo(attachmentType, attachInfo, schemaId);
                }

                if (attachmentType == AttachmentType.Instance)
                {
                    // Update the ANUM column of physical tables storing instance data
                    IncreamentANUMValue(attachInfo, schemaInfo);
                }

                return id;
            }
            else
            {
                // create an unique id as image name. combinded with file suffix
                int pos = attachInfo.Name.LastIndexOf(".");
                string suffix = "GIF";
                if (pos > 0)
                {
                    suffix = attachInfo.Name.Substring(pos + 1);
                }
                string id = NewteraNameSpace.GetImageId(attachInfo.ClassName, attachInfo.AttributeName, attachInfo.ItemId, suffix);
                attachInfo.ID = id;

                // the attachment is treated as an image for an image column
                UpdateImageAttributeValue(attachInfo, schemaInfo);

                return id;
            }
		}

		/// <summary>
		/// Gets information of a specified attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of the attaching item.</param>
		/// <param name="name">The name of an attachment.</param>
		/// <returns>An AttachmentInfo object</returns>
		public AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string itemId, string name)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
			string sql = null;

			try
			{
				switch (attachmentType)
				{
					case AttachmentType.Instance:
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentInfo");
						sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + itemId + "'");
						sql = sql.Replace(GetParamName("name", _dataProvider), "'" + name + "'");
						break;

					case AttachmentType.Class:
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentInfo");
						sql = sql.Replace(GetParamName("cid", _dataProvider), "'" + itemId + "'");
						sql = sql.Replace(GetParamName("name", _dataProvider), "'" + name + "'");
						break;
				}

				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				AttachmentInfo info = null;
				if (reader.Read())
				{
					info = new AttachmentInfo();
					info.ID = reader.GetString(0);
					info.ItemId = System.Convert.ToString(reader.GetValue(1));
					info.Name = reader.GetString(2);
					info.ClassName = reader.GetString(3);
					info.Type = reader.GetString(4);
					info.CreateTime = reader.GetDateTime(5);
                    //info.Size = reader.GetInt32(6);
                    SetFileSizeAndModifiedTime(info);
                    info.IsPublic = (reader.GetValue(7).ToString() != "0" ? true : false);
				}

				return info;
			}
			catch (Exception e)
			{
				throw new AttachmentException(e.Message, e);
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
		/// Gets information of a specified attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachmentId">The id of attachment.</param>
		/// <returns>An AttachmentInfo object</returns>
		public AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string attachmentId)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
			string sql = null;

			switch (attachmentType)
			{
				case AttachmentType.Instance:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentInfoById");
					break;

				case AttachmentType.Class:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentInfoById");
					break;
			}

			try
			{
				sql = sql.Replace(GetParamName("id", _dataProvider), "'" + attachmentId + "'");

				cmd.CommandText = sql;
				
				reader = cmd.ExecuteReader();

				AttachmentInfo info = null;
				if (reader.Read())
				{
					info = new AttachmentInfo();
					info.ID = attachmentId;
					info.ItemId = System.Convert.ToString(reader.GetValue(0));
					info.Name = reader.GetString(1);
					info.ClassName = reader.GetString(2);
					info.Type = reader.GetString(3);
					info.CreateTime = reader.GetDateTime(4);
                    //info.Size = reader.GetInt32(5);
                    SetFileSizeAndModifiedTime(info);
                    info.IsPublic = (reader.GetValue(6).ToString() != "0" ? true : false);
				}

				return info;
			}
			catch (Exception e)
			{
				throw new AttachmentException(e.Message, e);
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
		/// Gets information of all attachments of a specified instance.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of specified item.</param>
        /// <param name="startRow">The start row</param>
        /// <param name="pageSize">The page size</param>
		/// <returns>A collection of AttachmentInfo objects</returns>
        public AttachmentInfoCollection GetAttachmentInfos(AttachmentType attachmentType, string itemId, string schemaId, int startRow, int pageSize)
		{
			AttachmentInfoCollection infos = new AttachmentInfoCollection();
			IDbConnection con = _dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
			string sql = null;

			switch (attachmentType)
			{
				case AttachmentType.Instance:
					if (itemId != null)
					{
						// get attachment infos for a particular instance
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentInfos");
						sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + itemId + "'");
					}
					else
					{
						// get all attachment infos
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentInfosBySchema");
						sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
					}
					break;

				case AttachmentType.Class:
					if (itemId != null)
					{
						// get attachment infos for a particular class
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentInfos");
						sql = sql.Replace(GetParamName("cid", _dataProvider), "'" + itemId + "'");
					}
					else
					{
						// get all attachment infos
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentInfosBySchema");
						sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
					}
					break;
			}

			try
			{				
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				AttachmentInfo info;

                /*
                 * move the cursor of result set to the position indicated by the from
                 * of range
                 */
                int row = 0;
                int endRow = startRow + pageSize;

                while (row < startRow && reader.Read())
                {
                    row++;
                }

                // Now process the rows fall in the range
                while (row < endRow && reader.Read())
				{
					info = new AttachmentInfo();
					info.ID = reader.GetString(0);
					info.ItemId = System.Convert.ToString(reader.GetValue(1));
					info.Name = reader.GetString(2);
					info.ClassName = reader.GetString(3);
					info.Type = reader.GetString(4);
					info.CreateTime = reader.GetDateTime(5);
                    //info.Size = reader.GetInt32(6);
                    SetFileSizeAndModifiedTime(info);
                    info.IsPublic = (reader.GetValue(7).ToString() != "0" ? true : false);

					infos.Add(info);

                    row++;
				}

				return infos;
			}
			catch (Exception e)
			{
				throw new AttachmentException(e.Message, e);
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
        /// Gets count of all attachments of a specified item. If the user is
        /// unauthenticated, only return count of the public attachments
        /// </summary>
        /// <param name="attachmentType">One of the AttachmentType enum values.</param>
        /// <param name="itemId">The id of specified item.</param>
        /// <param name="schemaId">The id of the schema the instance belongs to.</param>
        /// <returns>A count of AttachmentInfo objects</returns>
        public int GetAttachmentInfosCount(AttachmentType attachmentType, string itemId, string schemaId)
        {
            int count = 0;
            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            string sql = null;

            switch (attachmentType)
            {
                case AttachmentType.Instance:
                    if (itemId != null)
                    {
                        // get attachment infos count for a particular instance
                        sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentInfosCount");
                        sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + itemId + "'");
                    }
                    else
                    {
                        // get all attachment infos
                        sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetAttachmentInfosBySchemaCount");
                        sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
                    }
                    break;

                case AttachmentType.Class:
                    if (itemId != null)
                    {
                        // get attachment infos for a particular class
                        sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentInfosCount");
                        sql = sql.Replace(GetParamName("cid", _dataProvider), "'" + itemId + "'");
                    }
                    else
                    {
                        // get all attachment infos
                        sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetClsAttachmentInfosBySchemaCount");
                        sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
                    }
                    break;
            }

            try
            {
                cmd.CommandText = sql;

                count = System.Convert.ToInt32(cmd.ExecuteScalar().ToString());

                return count;
            }
            catch (Exception e)
            {
                throw new AttachmentException(e.Message, e);
            }
            finally
            {
                con.Close();
            }
        }


        /// <summary>
        /// Delete class and instance attachments belongs to a database schema.
        /// Used when deleting a database schema.
        /// </summary>
        /// <param name="attachmentType">One of the AttachmentType enum values.</param>
        /// <param name="schemaId">The id of the schem.</param>
        public void DeleteAttachmentInfos(AttachmentType attachmentType, string schemaId)
        {
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = null;

            try
            {
                switch (attachmentType)
                {
                    case AttachmentType.Instance:
                        sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelAttachmentInfos");
                        sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
                        break;

                    case AttachmentType.Class:
                        sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelClsAttachmentInfos");
                        sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
                        break;
                }

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                /* the attachment may have been deleted by anther client, ignore the error */
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete attachment infos associated with a specific data instance.
        /// </summary>
        /// <param name="objId">The id of the instance that the attachments are associated with.</param>
        /// <returns>The number of records deleted</returns>
        public int DeleteInstanceAttachmentInfos(string objId)
        {
            int count = 0;
            IDbConnection con = _dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;
            string sql = null;

            try
            {
                sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelInstanceAttachmentInfos");
                sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + objId + "'");

                cmd.CommandText = sql;

                count = cmd.ExecuteNonQuery();

                tran.Commit();

                return count;
            }
            catch (Exception)
            {
                tran.Rollback();
                /* the attachment may have been deleted by anther client, ignore the error */
                return 0;
                //throw new AttachmentException(e.Message, e);
            }
            finally
            {
                con.Close();
            }
        }

		#endregion

		/// <summary>
		/// Insert arecord in database to store the attachment info.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The attachment information that describes the attachment.</param>
		/// <param name="schemaId">The id of the schema the instance belongs to.</param>
		private void InsertAttachmentInfo(AttachmentType attachmentType,
			AttachmentInfo attachInfo, string schemaId)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = null;
			
			switch (attachmentType)
			{
				case AttachmentType.Instance:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddAttachmentInfo");
					sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + attachInfo.ItemId + "'");
					break;

				case AttachmentType.Class:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddClsAttachmentInfo");
					sql = sql.Replace(GetParamName("cid", _dataProvider), "'" + attachInfo.ItemId + "'");
					break;
			}

			try
			{
				sql = sql.Replace(GetParamName("name", _dataProvider), "'" + attachInfo.Name + "'");
				sql = sql.Replace(GetParamName("id", _dataProvider), "'" + attachInfo.ID + "'");
				sql = sql.Replace(GetParamName("classname", _dataProvider), "'" + attachInfo.ClassName + "'");
				sql = sql.Replace(GetParamName("type", _dataProvider), "'" + attachInfo.Type + "'");
				sql = sql.Replace(GetParamName("asize", _dataProvider), attachInfo.Size + "");
				sql = sql.Replace(GetParamName("is_public", _dataProvider), (attachInfo.IsPublic ? "1" : "0"));
				sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");

				cmd.CommandText = sql;
				
				cmd.ExecuteNonQuery();

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw new AttachmentException(e.Message, e);
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Update a record of cm_attachment in database representing an attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="attachInfo">The attachment information that describes the attachment.</param>
		/// <param name="schemaId">The id of the schema the instance belongs to.</param>
		private void UpdateAttachmentInfo(AttachmentType attachmentType,
			AttachmentInfo attachInfo, string schemaId)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = null;

			switch (attachmentType)
			{
				case AttachmentType.Instance:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateAttachmentInfo");
					sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + attachInfo.ItemId + "'");
					break;

				case AttachmentType.Class:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateClsAttachmentInfo");
					sql = sql.Replace(GetParamName("cid", _dataProvider), "'" + attachInfo.ItemId + "'");
					break;
			}

			try
			{
				sql = sql.Replace(GetParamName("id", _dataProvider), "'" + attachInfo.ID + "'");
				sql = sql.Replace(GetParamName("name", _dataProvider), "'" + attachInfo.Name + "'");
				sql = sql.Replace(GetParamName("classname", _dataProvider), "'" + attachInfo.ClassName + "'");
				sql = sql.Replace(GetParamName("type", _dataProvider), "'" + attachInfo.Type + "'");
				sql = sql.Replace(GetParamName("asize", _dataProvider), attachInfo.Size + "");
				sql = sql.Replace(GetParamName("is_public", _dataProvider), (attachInfo.IsPublic ? "1" : "0"));
				sql = sql.Replace(GetParamName("schema_id", _dataProvider), "'" + schemaId + "'");
				
				cmd.CommandText = sql;
				
				cmd.ExecuteNonQuery();

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw new AttachmentException(e.Message, e);
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Delete an attachment info by deleting the record in CM_ATTACHMENT table
		/// that represents the attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The attachment info to be deleted.</param>
		/// <returns>The number of records deleted, it should always return one.</returns>
		protected int DeleteAttachmentInfo(AttachmentType attachmentType, AttachmentInfo attachInfo)
		{
			int count = 0;
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = null;

			try
			{
				switch (attachmentType)
				{
					case AttachmentType.Instance:
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelAttachmentInfo");
						sql = sql.Replace(GetParamName("oid", _dataProvider), "'" + attachInfo.ItemId + "'");
						sql = sql.Replace(GetParamName("name", _dataProvider), "'" + attachInfo.Name + "'");
						break;

					case AttachmentType.Class:
						sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelClsAttachmentInfo");
						sql = sql.Replace(GetParamName("cid", _dataProvider), "'" + attachInfo.ItemId + "'");
						sql = sql.Replace(GetParamName("name", _dataProvider), "'" + attachInfo.Name + "'");
						break;
				}

				cmd.CommandText = sql;

				count = cmd.ExecuteNonQuery();

				tran.Commit();

				return count;
			}
			catch (Exception)
			{
				tran.Rollback();
				/* the attachment may have been deleted by anther client, ignore the error */
				return 0;
				//throw new AttachmentException(e.Message, e);
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Increament the ANUM value by one to ANUM column of physical tables
		/// that store the instance data.
		/// </summary>
		/// <param name="attachInfo">The attachment info to be deleted.</param>
		/// <param name="schemaInfo">The schema info</param>		
		protected void IncreamentANUMValue(AttachmentInfo attachInfo, SchemaInfo schemaInfo)
		{		
			UpdateANUMValue(attachInfo, schemaInfo, true);
		}

		/// <summary>
		/// Decreament the ANUM value by one to ANUM column of physical tables
		/// that store the instance data.
		/// </summary>
		/// <param name="attachInfo">The attachment info to be deleted.</param>
		/// <param name="schemaInfo">The schema info</param>		
		protected void DecreamentANUMValue(AttachmentInfo attachInfo, SchemaInfo schemaInfo)
		{
			UpdateANUMValue(attachInfo, schemaInfo, false);
		}

		/// <summary>
		/// Gets the information indicating whether an attachmnet info with the given id
		/// exists or not?
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachmentId">The id of attachment.</param>
		/// <returns>true if it exists, false otherwise.</returns>
		private bool IsAttachmentInfoExist(AttachmentType attachmentType, string attachmentId)
		{
			bool status = false;
			IDbConnection con = _dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			string sql = null;
			
			switch (attachmentType)
			{
				case AttachmentType.Instance:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("IsAttachmentInfoExist");
					break;

				case AttachmentType.Class:
					sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("IsClsAttachmentInfoExist");
					break;
			}

			try
			{
				sql = sql.Replace(GetParamName("id", _dataProvider), "'" + attachmentId + "'");
				
				cmd.CommandText = sql;
				
                int count = System.Convert.ToInt32(cmd.ExecuteScalar().ToString());

				if (count == 1)
				{
					status = true;
				}

			}
			catch (Exception)
			{
			}
			finally
			{
				con.Close();
			}

			return status;
		}

		/// <summary>
		/// Increament/decreament the ANUM value by one to ANUM column of physical tables
		/// that store the instance data.
		/// </summary>
		/// <param name="attachInfo">The attachment info to be deleted.</param>
		/// <param name="schemaInfo">The schema info</param>		
		/// <param name="isIncreament">true if the update is increament, false for decreament.</param>
		private void UpdateANUMValue(AttachmentInfo attachInfo, SchemaInfo schemaInfo, bool isIncreament)
		{		
			ClassEntity baseClass;
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			
			try
			{
				MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(schemaInfo, _dataProvider);
	
				ClassElement clsElement = (ClassElement) metaData.SchemaModel.FindClass(attachInfo.ClassName);
			
				baseClass = new ClassEntity(clsElement);
				baseClass.MakeFullBlown();
				
				SQLBuilder builder = new SQLBuilder(metaData, _dataProvider);
				StringCollection sqls = builder.GenerateANUMUpdates(baseClass, attachInfo.ItemId, isIncreament);
				
				/*
				* Deletion starts from the bottom class
				*/
				foreach (string sql in sqls)
				{
					//SQLPrettyPrint.printSql(sql);
				
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
				
				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw new AttachmentException(e.Message, e);
			}
			finally
			{
				con.Close();
			}
		}

        private void SetFileSizeAndModifiedTime(AttachmentInfo info)
        {
            string filePath = NewteraNameSpace.GetAttachmentDir() + info.ID;
            // the attachmen file could be saved in a sub dir (after version 5.2.0)
            if (!File.Exists(filePath))
            {
                filePath = NewteraNameSpace.GetAttachmentSubDir(info.CreateTime) + info.ID;
            }

            // get the file
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                info.Size = fileInfo.Length / 1024;
                info.ModifiedTime = fileInfo.LastWriteTime;
            }
            else
            {
                info.Size = 0;
            }
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

        /// <summary>
        /// Update value of an image attribute with an unique image name.
        /// </summary>
        /// <param name="attachInfo">The attachment information that describes the attachment.</param>
        /// <param name="schemaId">The id of the schema the instance belongs to.</param>
        /// <returns>The unique image name</returns>
        protected void UpdateImageAttributeValue(AttachmentInfo attachInfo, SchemaInfo schemaInfo)
        {
            string query = UpdateImageColumnQuery.Replace(@"{db}", schemaInfo.Name);
            query = query.Replace(@"{version}", schemaInfo.Version);
            query = query.Replace(@"{class}", attachInfo.ClassName);
            query = query.Replace(@"{obj_id}", attachInfo.ItemId);
            query = query.Replace(@"{attribute_name}", attachInfo.AttributeName);
            query = query.Replace(@"{attribute_value}", attachInfo.ID);

            Interpreter interpreter = new Interpreter();
            interpreter.Query(query);
        }

        // get image id
        private string GetImageId(string className, string attributeName, string objId, string suffix)
        {
            return className + "-" + attributeName + "-" + objId + "." + suffix;
        }
	}
}