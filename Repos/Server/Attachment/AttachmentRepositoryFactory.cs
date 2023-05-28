/*
* @(#) AttachmentRepositoryFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Attachment
{
	using System;
	using System.Configuration;

	using Newtera.Server.DB;

	/// <summary>
	/// Creates a AttachmentRepository based on settings in the application configuration file.
	/// It currently supports storing attachments in database and file system.
	/// </summary>
	/// <version> 1.0.0	09 Jan 2004 </version>
	/// <author> Yong Zhang </author>
	public class AttachmentRepositoryFactory
	{	
		private const string ATTACHMENT_REPOSITORY = "AttachmentRepository";
	
		// Static factory object, all invokers will use this factory object.
		private static AttachmentRepositoryFactory theFactory;

		private IAttachmentRepository _repository;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private AttachmentRepositoryFactory()
		{
			_repository = null;
		}

		/// <summary>
		/// Gets the AttachmentRepositoryFactory instance.
		/// </summary>
		/// <returns> The AttachmentRepositoryFactory instance.</returns>
		static public AttachmentRepositoryFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates a specific AttachmentRepository for a schema.
		/// </summary>
		/// <param name="schemaInfo">schema information.</param>
		/// <returns>A AttachmentRepository object for the schema.</returns>
		public IAttachmentRepository Create()
		{
			if (_repository == null)
			{
                string type = ConfigurationManager.AppSettings[ATTACHMENT_REPOSITORY].ToUpper();
				if (type == null)
				{
					type = "Database"; // Default is database
				}

				switch (type.ToUpper())
				{
					case "DATABASE":
						DatabaseType dbType = DatabaseConfig.Instance.GetDatabaseType();
						if (dbType == DatabaseType.Oracle)
						{
							_repository = new AttachmentOracleRepository();
						}
						else if (dbType == DatabaseType.SQLServer)
						{
							_repository = new AttachmentSQLServerRepository();
						}
                        else if (dbType == DatabaseType.SQLServerCE)
                        {
                            _repository = new AttachmentSQLServerCERepository();
                        }
						else if (dbType == DatabaseType.MySql)
						{
							_repository = new AttachmentMySqlRepository();
						}

						break;
					case "FILE":
						_repository = new AttachmentFileRepository();
						break;
				}
			}

			return _repository;
		}

		static AttachmentRepositoryFactory()
		{
			// Initializing the factory.
			{
				theFactory = new AttachmentRepositoryFactory();
			}
		}
	}
}