/*
* @(#) MetaDataCache.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Resources;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.DB.MetaData;
	using Newtera.Server.Attachment;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Logging;
	using Newtera.Common.MetaData.FileType;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.Attachment;
    using Newtera.Server.Util;
    using Newtera.Common.Config;

    /// <summary>
    /// This is the single cache of MetaData info for the server. It keep a copy
    /// of MetaDataModel object for each schema.
    /// </summary>
    /// <version> 	1.0.0	23 Aug 2003 </version>
    /// <author> 	Yong Zhang </author>
    public class MetaDataCache
	{		
		// Static cache object, all invokers will use this cache object.
		private static MetaDataCache theCache;
		private const string CHANNEL_NAME = "Newtera.MetaDataNeedReload";

		private Hashtable _metaDataTable;
		private IKeyValueStore _metaDataLockedBy;
		private bool _needReloadFileTypeInfo;
		private FileTypeInfoCollection _fileTypeInfo;
        private SchemaInfo[] _schemaInfos;
		private ResourceManager _resources;
        private EventCollection _timerEvents;
		private RedisPubSubBroker _pubSubBroker;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private MetaDataCache()
		{
			_metaDataTable = new Hashtable();
			_metaDataLockedBy = KeyValueStoreFactory.TheInstance.Create("MetaDataCache.LockedBy");
			_needReloadFileTypeInfo = true;
			_fileTypeInfo = null;
            _schemaInfos = null;
            _timerEvents = null;
			_resources = new ResourceManager(this.GetType());
			if (RedisConfig.Instance.DistributedCacheEnabled)
			{
				_pubSubBroker = new RedisPubSubBroker();
				_pubSubBroker.Subscribe(CHANNEL_NAME).OnMessage(channelMessage =>
				{
					var schemaId = (string)channelMessage.Message;
					MetaDataModel metaData = (MetaDataModel)_metaDataTable[schemaId];
					if (metaData != null)
					{
						metaData.NeedReload = true;
					}
				});
			}
		}

		/// <summary>
		/// Gets the MetaDataCache instance.
		/// </summary>
		/// <returns> The MetaDataCache instance.</returns>
		static public MetaDataCache Instance
		{
			get
			{
				return theCache;
			}
		}

        /// <summary>
        /// Gets all the SchemaInfo objects stored in the database
        /// </summary>
        /// <param name="dataProvider">The data provider</param>
        /// <returns>An Array of SchemaInfo objects.</returns>
        public SchemaInfo[] GetSchemaInfos(IDataProvider dataProvider)
        {
            if (_schemaInfos == null)
            {
                MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);

                _schemaInfos = adapter.GetSchemaInfos();
            }

            return _schemaInfos;
        }

		/// <summary>
		/// Get an MetaDataModel for the given schema
		/// </summary>
		/// <param name="schemaInfo">schema information.</param>
		/// <returns>A IDGenerator object for the schema.</returns>
		/// <remarks>GetMetaData method is synchronized.</remarks>
		public MetaDataModel GetMetaData(SchemaInfo schemaInfo, IDataProvider dataProvider)
		{
			lock (this)
			{
				MetaDataModel metaData = (MetaDataModel) _metaDataTable[schemaInfo.NameAndVersion];

				if (metaData == null)
				{
                    string existingSchemaId = null;
                    if (!IsSchemaExisted(schemaInfo, dataProvider, out existingSchemaId))
                    {
                        string msg = string.Format(_resources.GetString("UnknownMetaData"), schemaInfo.NameAndVersion);
                        throw new UnknownSchemaException(msg);
                    }
                    else
                    {
                        // remember the schema id
                        schemaInfo.ID = existingSchemaId;
                    }

                    metaData = new MetaDataModel(schemaInfo.Clone());
					_metaDataTable[schemaInfo.NameAndVersion] = metaData;
				}

				// when metadata info is updated or newly created, the NeedReload flag
				// is set to true. The reload isn't taken place until GetMetaData is
				// invoked. And the Timestamp of the meta data model will be reset to the
				// one stored in the database.
				if (metaData.NeedReload)
				{
					MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
					adapter.Fill(metaData);

                    SetTimestamp(metaData, dataProvider); // set the timestamp to the meta-data model

					metaData.NeedReload = false;
				}
				
				return metaData;
			}
		}

        /// <summary>
		/// Updates the mete data of the given type in the database and invalidates
		/// that part of chached metadata so that it will be reloaded upon the next
		/// request.
		/// </summary>
		/// <param name="type">One of MetaDataType values</param>
		/// <param name="xmlString">The xml string representing the new meta data</param>
		/// <param name="schemaInfo">The schema whose meta data is updated</param>
		/// <param name="dataProvider">The data provider</param>
        /// <returns>The latest modified time of the meta-data model</returns>
        public DateTime UpdateMetaData(MetaDataType type, string xmlString, SchemaInfo schemaInfo, IDataProvider dataProvider)
        {
            return UpdateMetaData(type, xmlString, schemaInfo, dataProvider, false);
        }

		/// <summary>
		/// Updates the mete data of the given type in the database and invalidates
		/// that part of chached metadata so that it will be reloaded upon the next
		/// request.
		/// </summary>
		/// <param name="type">One of MetaDataType values</param>
		/// <param name="xmlString">The xml string representing the new meta data</param>
		/// <param name="schemaInfo">The schema whose meta data is updated</param>
		/// <param name="dataProvider">The data provider</param>
        /// <param name="isSafeMode">Indicate whenther to update in safe mode</param>
        /// <returns>The latest modified time of the meta-data model</returns>
		public DateTime UpdateMetaData(MetaDataType type, string xmlString, SchemaInfo schemaInfo, IDataProvider dataProvider, bool isSafeMode)
		{
			MetaDataModel newMetaDataModel = new MetaDataModel(schemaInfo.Clone());
			MetaDataModel oldMetaDataModel;
            DateTime modifiedTime = DateTime.Now;

			if (IsSchemaExisted(schemaInfo, dataProvider))
			{
				if (IsUpdateAllowed(schemaInfo))
				{
					// get the old schema
					oldMetaDataModel = GetMetaData(schemaInfo, dataProvider);

                    // Compare the timestamps of the newer meta-data model and older
                    // meta-data model, make sure that the two timestamps are the same.
                    if (oldMetaDataModel.SchemaInfo.ModifiedTime > newMetaDataModel.SchemaInfo.ModifiedTime)
                    {
                        // meta-data model stored in database is more updated then the new one.
                        // reject the updates
                        throw new UpdateMetaDataException(_resources.GetString("MetaDataOutdated"));
                    }
				}
				else
				{
					throw new LockMetaDataException(_resources.GetString("UpdateNotAllowed"));
				}
			}
			else
			{
				// the given schema dose not exists
				oldMetaDataModel = null;
			}

			switch (type)
			{
				case MetaDataType.Schema:
					// convert meta data from string to its model
					StringReader reader = new StringReader(xmlString);
					newMetaDataModel.SchemaModel.Read(reader);

					// compare the old and new meta data to get the result of
					// comparison
					MetaDataComparator comparator = new MetaDataComparator(dataProvider);
					MetaDataCompareResult result = comparator.Compare(newMetaDataModel,
						oldMetaDataModel, type);

                    // when update schema in the safe mode, it only allow additions to the
                    // existing schema, not deletion or alteration
                    if (isSafeMode)
                    {
                        if (!result.IsAdditionOnly)
                        {
                            throw new UpdateMetaDataException(_resources.GetString("NotSafeUpdate") + result.DeleteElementInfo);
                        }
                        else if (result.MismatchedEnumElements.Count > 0)
                        {
                            EnumElement enumElement = (EnumElement) result.MismatchedEnumElements[0];
                            throw new UpdateMetaDataException(_resources.GetString("MismatchedEnumElement") + enumElement.Caption);
                        }
                    }

					// lock the cache during updating the meta data
					lock(this)
					{
						MetaDataUpdateExecutor executor = new MetaDataUpdateExecutor(result, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid

						// make sure to execute the update as the last step
						executor.Execute(); // Perform the updates to meta data in database

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);
					}

					break;
				case MetaDataType.DataViews:
					// lock the cache during updating the meta data
					lock(this)
					{
						// write data view to the clob as an xml
						MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
						adapter.WriteDataViews(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid
					}

					break;

				case MetaDataType.XaclPolicy:
					// lock the cache during updating the meta data
					lock(this)
					{
						// write xacl policy as an xml
						MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
						adapter.WriteXaclPolicy(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid
					}

					break;

				case MetaDataType.Taxonomies:
					// lock the cache during updating the meta data
					lock(this)
					{
						// write taxomomies to the clob as an xml
						MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
						adapter.WriteTaxonomies(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid
					}

					break;

				case MetaDataType.Rules:
					// lock the cache during updating the meta data
					lock(this)
					{
						// write rules to the clob as an xml
						MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
						adapter.WriteRules(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid
					}

					break;

				case MetaDataType.Mappings:
					// lock the cache during updating the meta data
					lock(this)
					{
						// write mappings to the clob as an xml
						MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
						adapter.WriteMappings(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid
					}

					break;

				case MetaDataType.Selectors:
					// lock the cache during updating the meta data
					lock(this)
					{
						// write selectors to the clob as an xml
						MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
						adapter.WriteSelectors(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

						InvalidateCache(schemaInfo, type); // make the cached schema invalid
					}

					break;

                case MetaDataType.Events:
                    // lock the cache during updating the meta data
                    lock (this)
                    {
                        // write events to the clob as an xml
                        MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
                        adapter.WriteEvents(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

                        InvalidateCache(schemaInfo, type); // make the cached schema invalid
                    }

                    break;

                case MetaDataType.LoggingPolicy:
                    // lock the cache during updating the meta data
                    lock (this)
                    {
                        // write logging policy to the clob as an xml
                        MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
                        adapter.WriteLoggingPolicy(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

                        InvalidateCache(schemaInfo, type); // make the cached schema invalid
                    }

                    break;

                case MetaDataType.Subscribers:
                    // lock the cache during updating the meta data
                    lock (this)
                    {
                        // write subscribers to the clob as an xml
                        MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
                        adapter.WriteSubscribers(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

                        InvalidateCache(schemaInfo, type); // make the cached schema invalid
                    }

                    break;

                case MetaDataType.XMLSchemaViews:
                    // lock the cache during updating the meta data
                    lock (this)
                    {
                        // write xml schema views to the clob as an xml
                        MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
                        adapter.WriteXMLSchemaViews(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

                        InvalidateCache(schemaInfo, type); // make the cached schema invalid
                    }

                    break;

                case MetaDataType.Apis:
                    // lock the cache during updating the meta data
                    lock (this)
                    {
                        // write subscribers to the clob as an xml
                        MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
                        adapter.WriteApis(schemaInfo, xmlString);

                        modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);

                        InvalidateCache(schemaInfo, type); // make the cached api invalid
                    }

                    break;
            }

            return modifiedTime;
		}

        /// <summary>
        /// Updates the classes in the schema that are marked as NeedToAlter.
        /// request.
        /// </summary>
        /// <param name="metaData">The xml string representing the new meta data</param>
        /// <param name="schemaInfo">The schema whose meta data is updated</param>
        /// <param name="dataProvider">The data provider</param>
        /// <param name="isSafeMode">Indicate whenther to update in safe mode</param>
        /// <returns>The latest modified time of the meta-data model</returns>
        public DateTime UpdateSchemaIncremental(MetaDataModel newMetaDataModel, SchemaInfo schemaInfo, IDataProvider dataProvider, bool isSafeMode)
        {
            MetaDataModel oldMetaDataModel;
            DateTime modifiedTime = DateTime.Now;

            if (IsSchemaExisted(schemaInfo, dataProvider))
            {
                // get the old schema
                oldMetaDataModel = GetMetaData(schemaInfo, dataProvider);


                // compare the old and new meta data to get the result of
                // comparison
                MetaDataComparator comparator = new MetaDataComparator(dataProvider, false);
                MetaDataCompareResult result = comparator.Compare(newMetaDataModel,
                    oldMetaDataModel, MetaDataType.Schema);

                // when update schema in the safe mode, it only allow additions to the
                // existing schema, not deletion or alteration
                if (isSafeMode)
                {
                    if (!result.IsAdditionOnly)
                    {
                        throw new UpdateMetaDataException(_resources.GetString("NotSafeUpdate") + result.DeleteElementInfo);
                    }
                }

                // lock the cache during updating the meta data
                lock (this)
                {
                    // merge the affected elements in the new  meta data model to the existing meta data model
                    // the existing meta data model could be modified after the traverse
                    MergeMetaDataVisitor visitor = new MergeMetaDataVisitor(newMetaDataModel, oldMetaDataModel);
                    oldMetaDataModel.SchemaModel.Accept(visitor);

                    MetaDataUpdateExecutor executor = new MetaDataUpdateExecutor(result, dataProvider);

                    InvalidateCache(schemaInfo, MetaDataType.Schema); // make the cached schema invalid

                    // make sure to execute the update as the last step
                    //executor.Execute(); // Perform the updates to meta data in database

                    modifiedTime = UpdateMetaDataModelTimestamp(schemaInfo, dataProvider);
                }
            }

            return modifiedTime;
        }

		/// <summary>
		/// Fix the discrepancies bewtween a schema model and its corresponding databases
		/// that may occure due to various reasons.
		/// </summary>
		/// <param name="schemaInfo">The schema info indicating the schema to be fixed.</param>
		/// <param name="dataProvider">The data provider</param>
		public void FixSchemaModel(SchemaInfo schemaInfo, IDataProvider dataProvider)
		{
			MetaDataModel metaDataModel;

			if (IsSchemaExisted(schemaInfo, dataProvider))
			{
				// get the meta data model
				metaDataModel = this.GetMetaData(schemaInfo, dataProvider);

				// lock the cache during updating the meta data
				lock(this)
				{
					// compare the schema model with the database to detect the
					// discrepancies and collection the actions to fix them
					DBDiscrepancyDetector detector = new DBDiscrepancyDetector(metaDataModel, dataProvider);
					MetaDataCompareResult result = detector.Detect();

					MetaDataUpdateExecutor executor = new MetaDataUpdateExecutor(result, dataProvider);

					// Perform the actions to fix the discrepancies in database
					executor.FixDatabase();
				}
			}
		}

		/// <summary>
		/// Updates the file type info and invalidates that part of chached data
		/// so that it will be reloaded upon the next request.
		/// </summary>
		/// <param name="xmlString">The xml string representing the new meta data</param>
		/// <param name="dataProvider">The data provider</param>
		public void UpdateFileTypeInfo(string xmlString, IDataProvider dataProvider)
		{
			// lock the cache during updating the file type info
			lock(this)
			{
				MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
				adapter.WriteFileTypeInfo(xmlString);

				_needReloadFileTypeInfo = true;
			}
		}

		/// <summary>
		/// Delete a mete data in the database and clear the cache
		/// </summary>
		/// <param name="schemaInfo">The schema whose meta data is to be deleted</param>
		/// <param name="dataProvider">The data provider</param>
		public void DeleteMetaData(SchemaInfo schemaInfo, IDataProvider dataProvider)
		{
			MetaDataModel oldMetaDataModel;

			if (IsSchemaExisted(schemaInfo, dataProvider))
			{
				// get the old schema
				oldMetaDataModel = this.GetMetaData(schemaInfo, dataProvider);
			}
			else
			{
				return; // the meta data does not exist.
			}

			// make sure that the user has the permission to delete the meta data
			if (PermissionChecker.Instance.HasPermission(oldMetaDataModel.XaclPolicy, oldMetaDataModel, XaclActionType.Delete))
			{
				// delete the attachment files associated with the schema
                try
                {
                    DeleteAttachmentFiles(oldMetaDataModel);
                }
                catch (Exception)
                {
                    // continue deleting the schema even there are problems in deleting attachments
                }

                // delete the event infos associated with the schema
                try
                {
                    DeleteEvents(oldMetaDataModel);
                }
                catch (Exception)
                {
                    // continue deleting the schema even there are problems in deleting events
                }

				// then delete schema of the meta data
				MetaDataComparator comparator = new MetaDataComparator(dataProvider);
				MetaDataCompareResult result = comparator.Compare(null, oldMetaDataModel, MetaDataType.Schema);

				// lock the cache during deleting of the meta data
				lock(this)
				{
					MetaDataUpdateExecutor executor = new MetaDataUpdateExecutor(result, dataProvider);

					// clear the cache
					_metaDataTable.Remove(schemaInfo.NameAndVersion);
					_metaDataLockedBy.Remove(schemaInfo.NameAndVersion);

					// make sure to execute the update as the last step
					executor.Execute(); // Perform the updates to meta data in database

                    // clear the SchemaInfos so that they will be reloaded later
                    _schemaInfos = null;
				}
			}
			else
			{
				throw new NoDeletePermissionException("Do not have permission to delete database: " + schemaInfo.NameAndVersion);
			}
		}

		/// <summary>
		/// Get file type information from the cache.
		/// </summary>
		/// <param name="dataProvider">Data provider</param>
		/// <returns>A FileTypeInfoCollection object.</returns>
		/// <remarks>GetFileTypeInfo method is synchronized.</remarks>
		public FileTypeInfoCollection GetFileTypeInfo(IDataProvider dataProvider)
		{
			lock (this)
			{
				if (this._needReloadFileTypeInfo)
				{
					MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
					
					_fileTypeInfo = new FileTypeInfoCollection();

					adapter.FillFileTypeInfo(_fileTypeInfo);

					// TODO: Unable to cache the FileTypeInfoCollection since each FileTypeInfo
					// contans two MemoryStream object which can not be shared among multiple
					// threads (Have to do with the reading position problem). Therefore,
					// we have to load it every time. This will only impact Studio window
					// clients' performance a little, but the ASP.NET client will not be impacted since
					// they don't rely on the MemoryStream.

					//this._needReloadFileTypeInfo = false;
				}

				return _fileTypeInfo;
			}
		}

		/// <summary>
		/// Get the log of the latest updates to a meta data
		/// </summary>
		/// <param name="schemaInfo">schema information.</param>
		/// <param name="dataProvider">Data provider</param>
		/// <returns>A meta data update log</returns>
        /// <remarks>GetMetaDataUpdateLog method is synchronized.</remarks>
		public string GetMetaDataUpdateLog(SchemaInfo schemaInfo, IDataProvider dataProvider)
		{
			lock (this)
			{
				if (!IsSchemaExisted(schemaInfo, dataProvider))
				{
                    string msg = string.Format(_resources.GetString("UnknownMetaData"), schemaInfo.NameAndVersion);
					throw new UnknownSchemaException(msg);
				}

				MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);
				
				return adapter.ReadLog(schemaInfo);
			}
		}

		/// <summary>
		/// Lock the MetaDataModel for update so that other users are unable to update
		/// the same meta data concurrently.
		/// </summary>
		/// <param name="schemaInfo">schema information indicating the meta data.</param>
		/// <param name="dataProvider">Data provider</param>
		public void LockMetaData(SchemaInfo schemaInfo, IDataProvider dataProvider)
		{
			lock (this)
			{
				MetaDataModel metaData = (MetaDataModel) _metaDataTable[schemaInfo.NameAndVersion];

                if (metaData == null)
                {
                    string existingSchemaId = null;

                    if (!IsSchemaExisted(schemaInfo, dataProvider, out existingSchemaId))
                    {
                        string msg = string.Format(_resources.GetString("UnknownMetaData"), schemaInfo.NameAndVersion);
                        throw new UnknownSchemaException(msg);
                    }
                    else
                    {
                        // remember the schema id
                        schemaInfo.ID = existingSchemaId;
                    }

                    metaData = new MetaDataModel(schemaInfo.Clone());
                    _metaDataTable[schemaInfo.NameAndVersion] = metaData;
                }
                else
                {
                    //make sure that meta-data model has not been modified since the user loaded
                    // meta-data model into the client side
                    SchemaInfo existingSchemaInfo = FindSchemaInfo(schemaInfo.NameAndVersion, dataProvider);
                    if (existingSchemaInfo != null &&
                        schemaInfo.ModifiedTime < existingSchemaInfo.ModifiedTime)
                    {
                        string msg = _resources.GetString("LockFailed");
                        throw new LockMetaDataException(msg);
                    }
                }

				string lockingUser = _metaDataLockedBy.Get<string>(schemaInfo.NameAndVersion);
				string requestUser = null;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				if (principal != null)
				{
					requestUser = principal.Identity.Name;
				}
				
				if (requestUser == null)
				{
					throw new LockMetaDataException(_resources.GetString("UnknownUser"));
				}
                else if	(lockingUser != null)
				{
					string msg = String.Format(_resources.GetString("HasLocked"), lockingUser);
					throw new LockMetaDataException(msg);
				}
				else if (lockingUser == null)
				{
					// set the lock
					_metaDataLockedBy.Add(schemaInfo.NameAndVersion, requestUser);
				}
			}
		}

		/// <summary>
		/// Unlock the MetaDataModel so that other users can obtain the lock for update.
		/// </summary>
		/// <param name="schemaInfo">schema information indicating the meta data.</param>
		/// <param name="dataProvider">Data provider</param>
		/// <param name="forceUnlock">true if the unlock is forced by user, false if the unlock is resulting as disconnection.</param>
		public void UnlockMetaData(SchemaInfo schemaInfo, IDataProvider dataProvider,
			bool forceUnlock)
		{
			lock (this)
			{
				string lockingUser = _metaDataLockedBy.Get<string>(schemaInfo.NameAndVersion);
				string requestUser = null;
				string[] roles = null;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				if (principal != null)
				{
					requestUser = principal.Identity.Name;
					roles = principal.Roles;
				}

				if (requestUser != null)
				{
					if (lockingUser != null)
					{
						// the meta data is locked
						if (requestUser == lockingUser)
						{
							_metaDataLockedBy.Remove(schemaInfo.NameAndVersion); // remove the lock
						}
						else if (forceUnlock)
						{
							// check if the requesting user is the super user
							bool isSuperUser = false;

							if (roles != null)
							{
								foreach (string role in roles)
								{
									if (role == Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE)
									{
										isSuperUser = true;
										break;
									}
								}
							}

							if (isSuperUser)
							{
								// the lock can be removed by super-user
								_metaDataLockedBy.Remove(schemaInfo.NameAndVersion);
							}
							else
							{
								// not super user, throw an exception
								throw new LockMetaDataException(_resources.GetString("UnlockFailed"));
							}
						}
					}
				}
				else
				{
					throw new LockMetaDataException(_resources.GetString("UnknownUser"));
				}
			}
		}

        // Get a list of timer events (cloned) for all schemas
        public EventCollection GetAllTimerEvents()
        {
            lock (this)
            {
                if (_timerEvents == null)
                {
                    // First time access, initialize the cache

                    _timerEvents = new EventCollection();

                    IDataProvider dataProvider = DataProviderFactory.Instance.Create();

                    SchemaInfo[] schemaInfos = GetSchemaInfos(dataProvider);

                    MetaDataModel metaData;
                    EventCollection schemaTimerEvents;

                    foreach (SchemaInfo schemaInfo in schemaInfos)
                    {
                        metaData = GetMetaData(schemaInfo, dataProvider);

                        schemaTimerEvents = metaData.EventManager.GetTimerEvents();

                        foreach (EventDef eventDef in schemaTimerEvents)
                        {
                            eventDef.MetaData = metaData; // remember the meta data in eventdef

                            _timerEvents.Add(eventDef);
                        }
                    }

                    MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);

                    DateTime lastCheckedTime;

                    // load last checked time from database
                    foreach (EventDef eventDef in _timerEvents)
                    {
                        lastCheckedTime = adapter.GetEventLastCheckedTime(eventDef.MetaData.SchemaInfo.Name,
                            eventDef.MetaData.SchemaInfo.Version,
                            eventDef.ClassName, eventDef.Name);
                        if (lastCheckedTime > DateTime.MinValue)
                        {
                            // got a time from database. else, eventDef has default to the current system time
                            eventDef.LastCheckedTime = lastCheckedTime;
                        }
                    }
                }

                return _timerEvents;
            }
        }

        /// <summary>
        /// Set a datetime to the event as the last checked time
        /// </summary>
        /// <param name="eventDef">The event</param>
        /// <param name="lastCheckedTime">The last checked time</param>
        public void SetEventLastCheckedTime(EventDef eventDef, DateTime lastCheckedTime)
        {
            eventDef.LastCheckedTime = lastCheckedTime;

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);

            adapter.SetEventLastCheckedTime(eventDef.MetaData.SchemaInfo.Name, eventDef.MetaData.SchemaInfo.Version,
                eventDef.ClassName, eventDef.Name, lastCheckedTime);
        }

		/// <summary>
		/// Make the chached meta data of the given type invalid to force it
		/// to reload from database at next request.
		/// </summary>
		/// <param name="type">One of MetaDataType values</param>
		private void InvalidateCache(SchemaInfo schemaInfo, MetaDataType type)
		{
            try
            {
                MetaDataModel metaDataModel = (MetaDataModel)_metaDataTable[schemaInfo.NameAndVersion];

                if (metaDataModel != null)
                {
                    // it is cached, make it invalid

                    // clear cached dataview models
                    metaDataModel.ClearCache();

                    switch (type)
                    {
                        case MetaDataType.Schema:
                            metaDataModel.NeedReloadSchema = true;

                            // invalidate the cached query data that depends on
                            // the schema
                            QueryDataCache.Instance.AlterDependencyKeyValue(schemaInfo, QueryDataCache.METADATA_CLASSES);
                            break;
                        case MetaDataType.DataViews:
                            metaDataModel.NeedReloadDataViews = true;

                            QueryDataCache.Instance.AlterDependencyKeyValue(schemaInfo, QueryDataCache.DATAVIEWS);
                            break;
                        case MetaDataType.XaclPolicy:
                            metaDataModel.NeedReloadXaclPolicy = true;

                            QueryDataCache.Instance.AlterDependencyKeyValue(schemaInfo, QueryDataCache.XACLS);
                            break;
                        case MetaDataType.Taxonomies:
                            metaDataModel.NeedReloadTaxonomies = true;

                            QueryDataCache.Instance.AlterDependencyKeyValue(schemaInfo, QueryDataCache.TAXONOMIES);
                            break;
                        case MetaDataType.Rules:
                            metaDataModel.NeedReloadRules = true;
                            break;

                        case MetaDataType.Mappings:
                            metaDataModel.NeedReloadMappings = true;
                            break;

                        case MetaDataType.Selectors:
                            metaDataModel.NeedReloadSelectors = true;
                            break;

                        case MetaDataType.Events:
                            _timerEvents = null;
                            metaDataModel.NeedReloadEvents = true;
                            break;

                        case MetaDataType.LoggingPolicy:
                            metaDataModel.NeedReloadLoggingPolicy = true;

                            // clear the cache of log status so that they will be
                            // re-evaluated next time using the modified policy
                            LoggingChecker.Instance.ClearCache();

                            break;

                        case MetaDataType.Subscribers:
                            metaDataModel.NeedReloadSubscribers = true;
                            break;

                        case MetaDataType.XMLSchemaViews:
                            metaDataModel.NeedReloadXMLSchemaViews = true;
                            break;

                        case MetaDataType.Apis:
                            metaDataModel.NeedReloadApis = true;
                            break;
                    }

					if (metaDataModel.NeedReload &&
						RedisConfig.Instance.DistributedCacheEnabled)
                    {
						//publish a message to the broker to let subscribers know
						// the metadata is changed
						this._pubSubBroker.Publish(CHANNEL_NAME, schemaInfo.NameAndVersion);
                    }
                }
            }
            finally
            {
                // clear the SchemaInfos so that they will be reloaded with newly
                // modified times
                _schemaInfos = null;
            }
		}

        /// <summary>
        /// Set the meta-data model's modified time to the current time
        /// </summary>
        /// <param name="schemaInfo">The schema info indicating the meta-data model.</param>
        /// <param name="dataProvider">The data provider</param>
        /// <returns>The current time when the meta-data model is modified.</returns>
        private DateTime UpdateMetaDataModelTimestamp(SchemaInfo schemaInfo, IDataProvider dataProvider)
        {
            DateTime modifiedTime = DateTime.Now;
            MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);

            adapter.SetModifiedTime(schemaInfo, modifiedTime);
            schemaInfo.ModifiedTime = modifiedTime; // update the modified time in the SchemaInfo

            return modifiedTime;
        }

        /// <summary>
		/// Gets the info indicating if the given schema is existed
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <param name="dataProvider">The data provider</param>
		/// <returns>true if it exists, false otherwise</returns>
        public bool IsSchemaExisted(SchemaInfo schemaInfo, IDataProvider dataProvider)
        {
            string schemaID;
            return IsSchemaExisted(schemaInfo, dataProvider, out schemaID);
        }

		/// <summary>
		/// Gets the info indicating if the given schema is existed
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <param name="dataProvider">The data provider</param>
		/// <returns>true if it exists, false otherwise</returns>
		public bool IsSchemaExisted(SchemaInfo schemaInfo, IDataProvider dataProvider, out string schemaID)
		{
			bool status = false;
            schemaID = null;

            SchemaInfo[] schemas = GetSchemaInfos(dataProvider);

			foreach (SchemaInfo si in schemas)
			{
				if (si.NameAndVersion == schemaInfo.NameAndVersion)
				{
					status = true;
                    schemaID = si.ID; // get the current schema id
					break;
				}
			}

			return status;
		}

        /// <summary>
        /// Gets the information indicating whether the indicated schema has been modified.
        /// </summary>
        /// <param name="schemaInfo">The schema info to check</param>
        /// <param name="dataProvider">The data provider</param>
        /// <returns>true if it has been modified, false otherwise</returns>
        public bool IsSchemaModified(SchemaInfo schemaInfo, IDataProvider dataProvider)
        {
            bool status = false;

            SchemaInfo[] schemas = GetSchemaInfos(dataProvider);

            foreach (SchemaInfo si in schemas)
            {
                if (si.NameAndVersion == schemaInfo.NameAndVersion &&
                    schemaInfo.ModifiedTime < si.ModifiedTime)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Gets the name of role that has permission to modify the meta data
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <param name="dataProvider">The data provider</param>
		/// <returns>The name of role, null for non-protected mode.</returns>
		public string GetDBARole(SchemaInfo schemaInfo, IDataProvider dataProvider)
		{
			MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);

			return adapter.GetDBARole(schemaInfo);
		}

		/// <summary>
		/// Sets the name of role that has permission to modify the meta data
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <param name="dataProvider">The data provider</param>
		/// <param name="role">The name of role, null to set non-protected mode.</param>
		public void SetDBARole(SchemaInfo schemaInfo, IDataProvider dataProvider, string role)
		{
			MetaDataAdapter adapter = new MetaDataAdapter(dataProvider);

			adapter.SetDBARole(schemaInfo, role);
		}

		/// <summary>
		/// Delete the attachment files associated with the schema
		/// </summary>
		/// <param name="metaData">The meta data</param>
		private void DeleteAttachmentFiles(MetaDataModel metaData)
		{
			IAttachmentRepository repository = AttachmentRepositoryFactory.Instance.Create();
			
			string attachmentDir = NewteraNameSpace.GetAttachmentDir();
			string path;

			// delete class attachment files first
			AttachmentInfoCollection attachmentInfos;

            int startRow = 0;
            int pageSize = 50;
            int count = repository.GetAttachmentInfosCount(AttachmentType.Class, null, metaData.SchemaModel.SchemaInfo.ID);
            while (startRow < count)
            {
                attachmentInfos = repository.GetAttachmentInfos(AttachmentType.Class, null,
                    metaData.SchemaModel.SchemaInfo.ID, startRow, pageSize);

                foreach (AttachmentInfo attachmentInfo in attachmentInfos)
                {
                    // delete the attachment file
                    path = attachmentDir + attachmentInfo.ID;
                    // the attachmen file could be saved in a sub dir (after version 5.2.0)
                    if (!File.Exists(path))
                    {
                        path = NewteraNameSpace.GetAttachmentSubDir(attachmentInfo.CreateTime) + attachmentInfo.ID;
                    }

                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {
                    }
                }

                startRow += pageSize;
            }

            startRow = 0;
            count = repository.GetAttachmentInfosCount(AttachmentType.Instance, null, metaData.SchemaModel.SchemaInfo.ID);
            while (startRow < count)
            {
                // delete instance attachment files first
                attachmentInfos = repository.GetAttachmentInfos(AttachmentType.Instance, null,
                    metaData.SchemaModel.SchemaInfo.ID, startRow, pageSize);

                foreach (AttachmentInfo attachmentInfo in attachmentInfos)
                {
                    // delete the attachment file
                    path = attachmentDir + attachmentInfo.ID;
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {
                    }
                }

                startRow += pageSize;
            }

            // delete the class and instance attachment infos from a database schema
            repository.DeleteAttachmentInfos(AttachmentType.Class, metaData.SchemaModel.SchemaInfo.ID);
            repository.DeleteAttachmentInfos(AttachmentType.Instance, metaData.SchemaModel.SchemaInfo.ID);
		}

        /// <summary>
        /// Delete the events associated with the schema
        /// </summary>
        /// <param name="metaData">The meta data</param>
        private void DeleteEvents(MetaDataModel metaData)
        {
            MetaDataAdapter adapter = new MetaDataAdapter();

            adapter.DeleteEvents(metaData.SchemaInfo.Name, metaData.SchemaInfo.Version);
        }

        /// <summary>
        /// Sets the latest modified time as timestamp to the meta-data model
        /// </summary>
        /// <param name="metaData">The MetaDataModel object</param>
        private void SetTimestamp(MetaDataModel metaData, IDataProvider dataProvider)
        {
            SchemaInfo[] schemaInfos = this.GetSchemaInfos(dataProvider);
            foreach (SchemaInfo schemaInfo in schemaInfos)
            {
                if (schemaInfo.NameAndVersion == metaData.SchemaInfo.NameAndVersion)
                {
                    metaData.SchemaInfo.ModifiedTime = schemaInfo.ModifiedTime;
                    metaData.SchemaModel.SchemaInfo.ModifiedTime = schemaInfo.ModifiedTime;
                    break;
                }
            }
        }

        /// <summary>
        /// Find the SchemaInfo object for the given name and version
        /// </summary>
        /// <param name="schemaNameAndVersion"></param>
        /// <param name="dataProvider"></param>
        /// <returns>The found schemaInfo</returns>
        private SchemaInfo FindSchemaInfo(string schemaNameAndVersion, IDataProvider dataProvider)
        {
            SchemaInfo[] schemaInfos = this.GetSchemaInfos(dataProvider);
            SchemaInfo found = null;
            foreach (SchemaInfo schemaInfo in schemaInfos)
            {
                if (schemaInfo.NameAndVersion == schemaNameAndVersion)
                {
                    found = schemaInfo;
                    break;
                }
            }

            return found;
        }

		/// <summary>
		/// Gets the information indicating whether the current user is allowed to
		/// update the meta data model
		/// </summary>
		/// <param name="schemaInfo">The schema info to check</param>
		/// <returns>true if it is allowed, false, otherwise.</returns>
		private bool IsUpdateAllowed(SchemaInfo schemaInfo)
		{
			bool status = true;

			lock (this)
			{
				string lockingUser = _metaDataLockedBy.Get<string>(schemaInfo.NameAndVersion);
				string requestUser = null;
				string[] roles = null;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				if (principal != null)
				{
					requestUser = principal.Identity.Name;
					roles = principal.Roles;
				}

				if (requestUser != null)
				{
					bool isSuperUser = false;

					if (roles != null)
					{
						foreach (string role in roles)
						{
							if (role == Newtera.Common.Core.NewteraNameSpace.CM_SUPER_USER_ROLE)
							{
								isSuperUser = true;
								break;
							}
						}
					}
				
					// make sure that the meta data model is locked by the same user
					if (lockingUser != null)
					{
						if	(requestUser != lockingUser && !isSuperUser)
						{
							status = false;
						}
					}
					else
					{
						// the meta data model has not been locked, allow update by default
						//_metaDataLockedBy[schemaInfo.NameAndVersion] = requestUser;
					}
				}
				else
				{
					status = false;
				}
			}

			return status;
		}

		static MetaDataCache()
		{
			// Initializing the cache.
			{
				theCache = new MetaDataCache();
			}
		}
	}
}