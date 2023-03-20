/*
* @(#) QueryDataCache.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using System.IO;
	using System.Collections;
    using System.Collections.Generic;
	using System.Web;
    using System.Text;
    using System.Data;
    using System.Threading;
	using System.Web.Caching;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Server.Util;

	/// <summary>
	/// This is the single cache for data retrieved by varuous queries. It uses unique
	/// keys to identify cached entries.
	/// </summary>
	/// <version> 	1.0.0	03 Aug 2004 </version>
	/// <remarks>The caching only works for web applications</remarks>
	public class QueryDataCache
	{	
		public const string METADATA_CLASSES = "MetaDataClasses";
		public const string METADATA_ATTRIBUTES = "MetaDataAttributes";
		public const string TAXONOMIES = "Taxonomies";
		public const string DATAVIEWS = "DataViews";
		public const string XACLS = "Xacls";
        public const string TEMP_DIR = @"\temp\";

		private TimeSpan _slidingExpiration = new TimeSpan(0, 30, 0); // 30 min

        private IKeyValueStore _objects; // cached objects

        private IKeyValueStore _cachedSqls; // cached sqls

		// Static cache object, all invokers will use this cache object.
		private static QueryDataCache theCache;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private QueryDataCache()
		{
            _objects = KeyValueStoreFactory.TheInstance.Create("QueryDataCache.Objects");
            _cachedSqls = KeyValueStoreFactory.TheInstance.Create("QueryDataCache.CachedSqls");
        }

		/// <summary>
		/// Gets the QueryDataCache instance.
		/// </summary>
		/// <returns> The QueryDataCache instance.</returns>
		static public QueryDataCache Instance
		{
			get
			{
				return theCache;
			}
		}

		/// <summary>
		/// Get a cached query result
		/// </summary>
		/// <param name="query">The query</param>
		/// <returns>The cached result</returns>
		public object GetQueryResult(string query)
		{
            return _objects.Get<object>(query);
		}

		/// <summary>
		/// Get a cached object
		/// </summary>
		/// <param name="key">Cache object key</param>
		/// <returns>The cached object</returns>
		public object GetObject(string key)
		{
            return _objects.Get<object>(key);
		}

        /// <summary>
        /// Get a cached sql
        /// </summary>
        /// <param name="xqeruy">The corresponding xquery</param>
        /// <returns>The cached sql</returns>
        public string GetCachedSQL(string xquery)
        {
            return _cachedSqls.Get<string>(xquery);
        }

        /// <summary>
        /// Set a cached sql for a xquery
        /// </summary>
        /// <param name="xqeruy">The corresponding xquery</param>
        /// 
        public void SetCachedSQL(string xquery, string sql)
        {
            _cachedSqls.Add<string>(xquery, sql);
        }

        /// <summary>
        /// Clear cached sqls
        /// </summary>
        /// 
        public void ClearCachedSQL()
        {
            _cachedSqls.Clear();
        }

        /// <summary>
        /// Get a cached object
        /// </summary>
        /// <param name="key">Cache object key</param>
        /// <param name="schemaInfo">The info indicating the schema the cached object belongs to</param>
        /// <returns>The cached object</returns>
        public object GetObject(string key, SchemaInfo schemaInfo)
		{
            return _objects.Get<object>(schemaInfo.NameAndVersion + key);
		}

		/// <summary>
		/// Add a cached object to the cache
		/// </summary>
		/// <param name="key">Cache object key</param>
		/// <param name="cachedObject">The object to be cached</param>
		public void AddObject(string key, object cachedObject)
		{
            if (_objects.Contains(key))
            {
                _objects.Remove(key);
            }
            _objects.Add(key, cachedObject);
		}

        /// <summary>
        /// Add a DataTable to the cache
        /// </summary>
        /// <param name="key">Cache object key</param>
        /// <param name="dataTable">The data table to be cached</param>
        public void AddDataTable(string key, DataTable dataTable)
        {
            if (_objects.Contains(key))
            {
                _objects.Remove(key);
            }
            _objects.Add(key, dataTable);

            // write the data table xml to a temp file so that we can create
            // data table from the file in case that web client and windows clien
            // are running in different application domains
            string filePath = NewteraNameSpace.GetAppHomeDir() + TEMP_DIR + "datatable.xml";
            try
            {
                dataTable.WriteXml(filePath);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        /// <summary>
        /// Get a cached data table
        /// </summary>
        /// <param name="key">Cache object key</param>
        /// <returns>The cached data table</returns>
        public DataTable GetDataTable(string key)
        {
            DataTable dataTable = _objects.Get<DataTable>(key);

            if (dataTable == null)
            {
                // Get the data table from a xml file in case that web client and windows clien
                // are running in different application domains
                string filePath = NewteraNameSpace.GetAppHomeDir() + TEMP_DIR + "datatable.xml";
                try
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(filePath);

                    if (ds.Tables.Count > 0)
                    {
                        dataTable = ds.Tables[0];
                    }
                }
                catch (Exception)
                {
                    dataTable = null;
                }
            }

            return dataTable;
        }

		/// <summary>
		/// Add a cached object to the cache
		/// </summary>
		/// <param name="key">Cache object key</param>
		/// <param name="schemaInfo">The info indicating the schema the cached object belongs to</param>
		/// <param name="cachedObject">The object to be cached</param>
		/// <param name="dependencyKeys">An array of dependency keys</param>
		public void AddObject(string key, SchemaInfo schemaInfo, object cachedObject,
			string[] dependencyKeys)
		{
            _objects.Add(schemaInfo.NameAndVersion + key, cachedObject);
		}

        /// <summary>
        /// Add an entry to the session oriented cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cachedObject"></param>
        public void AddSessionObject(string key, object cachedObject)
        {
            string sessionId = GetSessionId();

            IDictionary<string, object> sessionCache = _objects.Get<IDictionary<string, object>>(sessionId);

            if (sessionCache == null)
            {
                sessionCache = new Dictionary<string, object>();
                _objects.Add<IDictionary<string, object>>(sessionId, sessionCache);
            }

            sessionCache[key] = cachedObject;
        }

        /// <summary>
        /// Get an object that are cached in session cache
        /// </summary>
        /// <param name="key"></param>
        public object GetSessionObject(string key)
        {
            object cachedObject = null;

            string sessionId = GetSessionId();
            IDictionary<string, object> sessionCache = _objects.Get<IDictionary<string, object>>(sessionId);
            if (sessionCache != null)
            {
                cachedObject = sessionCache[key];
            }

            return cachedObject;
        }

        /// <summary>
        /// Clear the expiring objects for the current user
        /// </summary>
        public void ClearExpiringObjects()
        {
            string sessionId = GetSessionId();

            _objects.Remove(sessionId);
        }

        private string GetSessionId()
        {
            string sessionId = null;

            // use the user name as session id
            sessionId = Thread.CurrentPrincipal.Identity.Name;

            return sessionId;
        }

		/// <summary>
		/// Remove a cached object from the cache
		/// </summary>
		/// <param name="key">Cache object key</param>
		/// <returns>The removed object</returns>
		public object RemoveObject(string key)
		{
            object cached = _objects.Get<object>(key);
            _objects.Remove(key);
            return cached;
		}

        /// <summary>
        /// Create a key for keep an object in the cache based on schema info and current user
        /// </summary>
        /// <param name="schemaInfo">The schema info</param>
        /// <returns>A key generated using information availabe</returns>
        public string CreateKey(SchemaInfo schemaInfo)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(schemaInfo.Name).Append(schemaInfo.Version);
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal != null)
            {
                builder.Append(principal.Identity.Name);
            }

            // return the hashcode
            return builder.ToString();
        }

		/// <summary>
		/// Alter the value of the given dependency key, therefore, invalidate
		/// the cached query data that depends on the key.
		/// </summary>
		/// <param name="schemaInfo">The schema info</param>
		/// <param name="key">The key name</param>
		public void AlterDependencyKeyValue(SchemaInfo schemaInfo, string key)
		{
            // to be implemented
		}


		/// <summary>
		/// Create dependency keys for meta data entities of a schema
		/// </summary>
		/// <param name="schemaInfo">The schema info</param>
		public void InitDependencyKeys(SchemaInfo schemaInfo)
		{
            // To be implemented
		}

		static QueryDataCache()
		{
			// Initializing the cache.
			{
				theCache = new QueryDataCache();
			}
		}
	}
}