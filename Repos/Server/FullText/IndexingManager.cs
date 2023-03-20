/*
* @(#) IndexingManager.cs
*
* Copyright (c) 2003 - 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
	using System.IO;
	using System.Collections;
    using System.Collections.Generic;
	using System.Threading;
    using System.Xml;
    using System.Data;
    using System.Timers;
    using System.Security.Principal;
    using System.Text.RegularExpressions;

	using Newtera.Common.Core;
    using Newtera.Common.Config;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Server.Engine.Cache;
    using Newtera.Server.DB;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// This is the single object that manages a queue for full-text indexing spiders and the queue
    /// uses multiple worker threads to run the spiders, one for each full-text index defined in all schemas.
	/// </summary>
	/// <version> 1.0.0 15 April 2014 </version>
	public class IndexingManager
	{
		// Static cache object, all invokers will use this cache object.
		private static IndexingManager theManager;

        private const string INDEX_METHOD_KEY = "method";
        private const string INDEX_METHOD_AUTO = "auto";
        private const string INDEX_INTERVAL_KEY = "interval";
        private const string INDEX_DEPTH_KEY = "depth";
        private const string INDEX_ENABLED = "enabled";
        private const string INDEX_START_TIME_KEY = "startTime";

        private System.Timers.Timer _theTimer;

        private FullTextIndexConfig _indexConfig;
        private double _indexingInterval;
        private int _startClock;

        private List<IndexBuilder> _indexBuilders;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private IndexingManager()
		{
            // start the timer
            _theTimer = new System.Timers.Timer(3600000); // one hour interval      
            _theTimer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            _theTimer.Interval = 3600000;
            _theTimer.Enabled = true;
            _theTimer.AutoReset = true; // repeat timer

            _indexBuilders = null;

            _indexConfig = null;

		}

		/// <summary>
		/// Gets the IndexingManager instance.
		/// </summary>
		/// <returns> The IndexingManager instance.</returns>
		static public IndexingManager Instance
		{
			get
			{
				return theManager;
			}
		}

        public List<IndexBuilder> IndexBuilders
        {
            get
            {
                return _indexBuilders;
            }
        }

        public double IndexingInterval
        {
            get
            {
                return _indexingInterval;
            }
        }

        public int StartClock
        {
            get
            {
                return _startClock;
            }
        }

        public void Start()
        {
            _indexConfig = new FullTextIndexConfig();

            string method = _indexConfig.GetAppSetting(IndexingManager.INDEX_METHOD_KEY);
            if (!string.IsNullOrEmpty(method) && method.Trim().ToLower() == IndexingManager.INDEX_METHOD_AUTO)
            {
                try
                {
                    _indexingInterval = double.Parse(_indexConfig.GetAppSetting(IndexingManager.INDEX_INTERVAL_KEY));
                }
                catch (Exception)
                {
                    _indexingInterval = 7; // default interval value is seven day
                }

                try
                {
                    _startClock = int.Parse(_indexConfig.GetAppSetting(IndexingManager.INDEX_START_TIME_KEY));
                }
                catch (Exception)
                {
                    _startClock = 1; // default start clock is 1 o'clock
                }

                InitializeIndexBuilders(); // Adding a new index attribute to a schema, requires re-start of the process

                _theTimer.Start();
            }
        }

        /// <summary>
        /// Called by the thread when it completes index building work
        /// </summary>
        public void IndexBuildingCompleted(IndexBuilder indexBuilder)
        {
            // write the time when the indexing is done
            indexBuilder.LastIndexTime = DateTime.Now;

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            // write the time to database
            IDbConnection con = dataProvider.Connection;
            IDbTransaction tran = con.BeginTransaction();
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = tran;

            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("SetLastIndexTime");

            SymbolLookup lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
            string lastIndexTimeStr = lookup.GetTimestampFunc(indexBuilder.LastIndexTime.ToString("s"), LocaleInfo.Instance.DateTimeFormat);

            try
            {
                sql = sql.Replace(GetParamName("attribute_id", dataProvider), "'" + indexBuilder.IndexingAttribute.ID + "'");
                sql = sql.Replace(GetParamName("index_time", dataProvider), lastIndexTimeStr);

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                tran.Rollback();
                throw ex;
            }
            finally
            {
                con.Close();
            }

        }

        public int GetTravelDepth()
        {
            int depth = 1; // default depth

            if (_indexConfig != null)
            {
                string depthStr = _indexConfig.GetAppSetting(IndexingManager.INDEX_DEPTH_KEY);
                if (!string.IsNullOrEmpty(depthStr))
                {
                    try
                    {
                        depth = int.Parse(depthStr);
                    }
                    catch (Exception)
                    {
                        depth = 1;
                    }
                }
            }

            return depth;
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                //ErrorLog.Instance.WriteLine("Timer elapsed");

                List<IndexBuilder> indexBuilders = IndexingManager.Instance.IndexBuilders;

                if (indexBuilders != null)
                {
                    // start a thread to run the index builder if the builder isn't in process and indexing interval expired
                    foreach (IndexBuilder indexBuilder in indexBuilders)
                    {
                        if (!indexBuilder.IsInProcess &&
                            IsIntervalExpired(indexBuilder) &&
                            IsStartClock())
                        {
                            // run the index builder in a worker thread from the thread pool
                            // when the index builder completes the processing, it will notify the
                            // the index manager
                            ThreadPool.QueueUserWorkItem(indexBuilder.Run);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void InitializeIndexBuilders()
        {
            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;
            MetaDataModel metaData;
            SimpleAttributeElement indexingAttribute;
            IndexBuilder indexBuilder;
            DateTime lastIndexedTime;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = superUser;

                int depth = GetTravelDepth();

                IDataProvider dataProvider = DataProviderFactory.Instance.Create();
                _indexBuilders = new List<IndexBuilder>();
                SchemaInfo[] schemaInfos = MetaDataCache.Instance.GetSchemaInfos(dataProvider);

                foreach (SchemaInfo schemaInfo in schemaInfos)
                {
                    if (IsFullTextIndexEnabled(schemaInfo.NameAndVersion))
                    {
                        metaData = MetaDataCache.Instance.GetMetaData(schemaInfo, dataProvider);

                        // go over all root classes from the metadata model to run index builder for each full-text index defined, note: only root classes can have full-text indexing attributes defined.
                        // Subclasses cannot have full-text indexing attributes
                        foreach (ClassElement rootClass in metaData.SchemaModel.RootClasses)
                        {
                            indexingAttribute = GetFullTextIndexAttribute(rootClass);
                            if (indexingAttribute != null)
                            {
                                lastIndexedTime = GetLastIndexedTime(dataProvider, indexingAttribute);
                                // add an index builder to the list, each index builder will run on a thread of its own
                                indexBuilder = new IndexBuilder(metaData, rootClass, indexingAttribute, lastIndexedTime,depth);
                                _indexBuilders.Add(indexBuilder);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private bool IsIntervalExpired(IndexBuilder indexBuilder)
        {
            bool status = false;

            DateTime lastIndexTime = indexBuilder.LastIndexTime;

            TimeSpan timeSpan = DateTime.Now.Subtract(lastIndexTime);

            double days = timeSpan.TotalDays;

            if (days > IndexingManager.Instance.IndexingInterval)
            {
                status = true;
            }

            return status;
        }

        private bool IsStartClock()
        {
            bool status = false;

            if (DateTime.Now.Hour == IndexingManager.Instance.StartClock)
            {
                status = true;
            }

            return status;
        }


        private SimpleAttributeElement GetFullTextIndexAttribute(ClassElement classElement)
        {
            SimpleAttributeElement indexingAttribute = null;
            foreach (SimpleAttributeElement simpleAttribute in classElement.SimpleAttributes)
            {
                if (simpleAttribute.IsFullTextSearchable)
                {
                    indexingAttribute = simpleAttribute;
                    break;
                }
            }

            return indexingAttribute;
        }

        private bool IsFullTextIndexEnabled(string schemaId)
        {
            bool status = false;

            string val = _indexConfig.GetAppSetting(schemaId);
            if (!string.IsNullOrEmpty(val) && val.ToLower() == IndexingManager.INDEX_ENABLED)
            {
                status = true;
            }

            return status;
        }

        private DateTime GetLastIndexedTime(IDataProvider dataProvider, SimpleAttributeElement indexingAttribute)
        {
            DateTime lastIndexTime = DateTime.MinValue;

            IDbConnection con = dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            IDataReader reader = null;
            
            string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetLastIndexTime");
  
            try
            {
                sql = sql.Replace(GetParamName("attribute_id", dataProvider), "'" + indexingAttribute.ID + "'");

                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        lastIndexTime = reader.GetDateTime(0);

                        // Keep it in the schema element
                    }
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                con.Close();
            }

            return lastIndexTime;
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

		static IndexingManager()
		{
			// Initializing the manager.
			{
				theManager = new IndexingManager();
			}
		}
	}
}