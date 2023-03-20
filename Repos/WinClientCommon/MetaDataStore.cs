/*
* @(#)MetaDataStore.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WinClientCommon
{
	using System;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;

	/// <summary>
	/// A singleton class that serves as a global place to keep meta data models
	/// loaded from database, not including the meta data model loaded from files.
	/// </summary>
	/// <version>  	1.0.0 16 Nov 2003 </version>
	/// <author> Yong Zhang </author>
	public class MetaDataStore
	{
		private MetaDataModel _currentMetaData;
		private Hashtable _metaDataTable;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static MetaDataStore theStore;
		
		static MetaDataStore()
		{
			theStore = new MetaDataStore();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private MetaDataStore()
		{
			_currentMetaData = null;
			_metaDataTable = new Hashtable();
		}

		/// <summary>
		/// Gets the MetaDataStore instance.
		/// </summary>
		/// <returns> The MetaDataStore instance.</returns>
		static public MetaDataStore Instance
		{
			get
			{
				return theStore;
			}
		}
		
		/// <summary>
		/// Gets the current active meta data model
		/// </summary>
		/// <value>The MetaDataModel instance.</value>
		public MetaDataModel CurrentMetaData
		{
			get
			{
				return _currentMetaData;
			}
		}

		/// <summary>
		/// Gets all connected schemas
		/// </summary>
		/// <value>An array of SchemaInfo instances</value>
		public SchemaInfo[] ConnectedSchemas
		{
			get
			{
				SchemaInfo[] schemas = new SchemaInfo[_metaDataTable.Count];

				int index = 0;
				foreach (MetaDataModel metaData in _metaDataTable.Values)
				{
					SchemaInfo schemaInfo = new SchemaInfo();
					schemaInfo.Name = metaData.SchemaInfo.Name;
					schemaInfo.Version = metaData.SchemaInfo.Version;
                    schemaInfo.ModifiedTime = metaData.SchemaInfo.ModifiedTime;
					schemas[index++] = schemaInfo;
				}

				return schemas;
			}
		}

		/// <summary>
		/// Select a meta data model as the current meta data model
		/// </summary>
        /// <param name="schemaInfo">The schema info</param>
        public void SelectMetaData(SchemaInfo schemaInfo)
		{
			// syncronize the access to the hashtable
			lock(this)
			{
                string key = schemaInfo.NameAndVersion;
				
				_currentMetaData = (MetaDataModel) _metaDataTable[key];
			}
		}

        /// <summary>
        /// Select a meta data model as the current meta data model
        /// </summary>
        /// <param name="schemaId">The schema Id</param>
        public void SelectMetaData(string schemaId)
        {
            // syncronize the access to the hashtable
            lock (this)
            {
                _currentMetaData = (MetaDataModel)_metaDataTable[schemaId];
            }
        }

        /// <summary>
        /// Get a meta data model
        /// </summary>
        /// <param name="name">The schema name</param>
        /// <param name="version">The schema version</param>
        /// <returns>A MetaDataModel</returns>
        public MetaDataModel GetMetaData(string name, string version)
        {
            // syncronize the access to the hashtable
            lock (this)
            {
                SchemaInfo schemaInfo = new SchemaInfo();
                schemaInfo.Name = name;
                schemaInfo.Version = version;
                string key = schemaInfo.NameAndVersion;

                return (MetaDataModel)_metaDataTable[key];
            }
        }

		/// <summary>
		/// Get a meta data model
		/// </summary>
        /// <param name="schemaInfo">The schema info</param>
		/// <returns>A MetaDataModel</returns>
        public MetaDataModel GetMetaData(SchemaInfo schemaInfo)
		{
			// syncronize the access to the hashtable
			lock(this)
			{
                string key = schemaInfo.NameAndVersion;
				
				return (MetaDataModel) _metaDataTable[key];
			}
		}

        /// <summary>
        /// Get a meta data model
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A MetaDataModel</returns>
        public MetaDataModel GetMetaData(string schemaId)
        {
            // syncronize the access to the hashtable
            lock (this)
            {
                if (schemaId != null)
                {
                    return (MetaDataModel)_metaDataTable[schemaId];
                }
                else
                {
                    return null;
                }
            }
        }

		/// <summary>
		/// Put a meta data model in the store
		/// </summary>
		/// <param name="metaData">The meta data model</param>
		/// <returns>A MetaDataModel</returns>
		public void PutMetaData(MetaDataModel metaData)
		{
			// syncronize the access to the hashtable
			lock(this)
			{
                if (metaData != null)
                {
                    string key = metaData.SchemaInfo.NameAndVersion;

                    _metaDataTable[key] = metaData;

                    // make it as current meta data model
                    _currentMetaData = metaData;
                }
			}
		}

		/// <summary>
		/// Remove a meta data model from the store
		/// </summary>
		/// <param name="schemaInfo">The schema info</param>
		public void RemoveMetaData(SchemaInfo schemaInfo)
		{
			// syncronize the access to the hashtable
			lock(this)
			{
                string key = schemaInfo.NameAndVersion;
				
				_metaDataTable.Remove(key);

				if (_currentMetaData != null &&
                    _currentMetaData.SchemaInfo.Name == schemaInfo.Name &&
                    _currentMetaData.SchemaInfo.Version == schemaInfo.Version)
				{
					_currentMetaData = null;
				}
			}
		}
	}
}