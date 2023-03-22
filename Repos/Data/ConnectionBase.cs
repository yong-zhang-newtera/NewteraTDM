/*
* @(#)ConnectionBase.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Data;
	using System.Threading;
	using System.Resources;
	using System.Security.Principal;
	using System.Text.RegularExpressions;
	using System.Collections;
	using System.ComponentModel;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Cache;
    using Newtera.Server.UsrMgr;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Provide common functionality of connections to the Newtera servers.
	/// </summary>
	/// <version>  	1.0.0 15 Dec 2006 </version>
	public abstract class ConnectionBase
	{
		// Definitions of keys in the connection string
		protected const string USER_ID = "USER_ID";
        protected const string PASSWORD = "PASSWORD";
        protected const string SCHEMA_NAME = "SCHEMA_NAME";
        protected const string SCHEMA_VERSION = "SCHEMA_VERSION";
        protected const string TIMESTAMP = "TIMESTAMP";
        protected const string SAFE_MODE = "SAFE_MODE";

        protected ConnectionState _state;
        protected Hashtable _properties;
        protected IDataProvider _dataProvider;
        protected IUserManager _userManager;

        #region common

        /// <summary> 
		/// Default constructor
		/// </summary>
		public ConnectionBase()
		{	
			_dataProvider = DataProviderFactory.Instance.Create();
			_state = ConnectionState.Closed;
			_userManager = UserManagerFactory.Instance.Create();
			SetConditionRunner();
		}

        /// <summary>
        /// Gets the current state of the connection
        /// The allowed state changes are: 
        /// From Closed to Open, using the open method of the Connection object. 
        /// From Open to Closed, using either the close method of the Connection object. 
        /// </summary>
        /// <value> current ConnectionState object.</value>
        public ConnectionState State
        {
            get
            {
                return _state;
            }
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        /// <remarks> An application can call Close more than one time without
        /// generating an exception.</remarks>
        public void Close()
        {
            /*
            * Close the CM connection and set the ConnectionState
            * property.
            */
            if (_state != ConnectionState.Closed)
            {
                _state = ConnectionState.Closed;
            }
        }

        /// <summary>
        /// Opens a data source connection with the settings specified by the
        /// ConnectionString property of the provider-specific Connection object.
        /// </summary>
        /// <exception cref="CMException">
        /// CMException thrown when something go wrong with openning connection
        /// </exception>
        public void Open()
        {
            // If the client is ASP.NET user and it is authenticated, a CustomPrincipal is attached
            // at Global.Application_AuthorizeRequest method.
            //
            // If the client is an anonymouse user who has not been authenticated, it
            // will be assigned a CustomPrincipal with Anonymouse user identity
            //
            // TODO, figure out how to imposonate a Window Client that uses WebService
            // to access, so that we can attach a CustomPrincipal at global level.
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
            {
                string userId = null;

                if (_properties != null)
                {
                    userId = (string)_properties[USER_ID];
                }

                if (userId == null)
                {
                    // for unknow user, treat it as an anonymouse user
                    userId = XaclSubject.AnonymousUser;
                }

                // Attach a CustomPrincipal to the current thread
                CustomPrincipal.Attach(new ServerSideUserManager(), new ServerSideServerProxy(), userId);
            }

            _state = ConnectionState.Open;
        }
        
        /// <summary>
        /// Gets the user name specified in the connection string
        /// </summary>
        /// <value>The user name, could be null</value>
        public string UserName
        {
            get
            {
                return (string)_properties[USER_ID]; ;
            }
        }

        /// <summary>
        /// Gets the password specified in the connection string
        /// </summary>
        /// <value>The user password, could be null</value>
        public string Password
        {
            get
            {
                return (string)_properties[PASSWORD]; ;
            }
        }

        /// <summary>
        /// Gets the schema name specified in the connection string
        /// </summary>
        /// <value>The schema name, could be null</value>
        public string SchemaName
        {
            get
            {
                return (string)_properties[SCHEMA_NAME]; ;
            }
        }

        /// <summary>
        /// Gets the schema version specified in the connection string
        /// </summary>
        /// <value>The schema version, could be null</value>
        public string SchemaVersion
        {
            get
            {
                return (string)_properties[SCHEMA_VERSION]; ;
            }
        }

        /// <summary>
        /// Get the data provider associated with the connection.
        /// </summary>
        /// <value>The data provider.</value>
        public IDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }
        }

        /// <summary>
        /// Get list of SchemaInfo instances that contain schema name and version for the
        /// available schemas.
        /// </summary>
        /// <value>
        /// An array of SchemaInfo objects
        /// </value>
        public SchemaInfo[] AllSchemas
        {
            get
            {
                return MetaDataCache.Instance.GetSchemaInfos(_dataProvider);
            }
        }

        /// <summary>
        /// Get list of SchemaInfo instances that the current user are authorized to access.
        /// </summary>
        /// <value>
        /// A list of SchemaInfo instances, can be empty if the user does not have permission
        /// to access any databases.
        /// </value>
        public SchemaInfoCollection AuthorizedSchemas
        {
            get
            {
                SchemaInfoCollection accessibleSchemas = new SchemaInfoCollection();

                MetaDataAdapter metaDataAdapter = new MetaDataAdapter();

                SchemaInfo[] allSchemas = this.AllSchemas;

                MetaDataModel metaData;
                SchemaInfo schemaInfo;
                for (int i = 0; i < allSchemas.Length; i++)
                {
                    metaData = MetaDataCache.Instance.GetMetaData(allSchemas[i], _dataProvider);

                    if (PermissionChecker.Instance.HasPermission(metaData.XaclPolicy, metaData, XaclActionType.Read))
                    {
                        schemaInfo = new SchemaInfo();
                        schemaInfo.Name = allSchemas[i].Name;
                        schemaInfo.Version = allSchemas[i].Version;
                        schemaInfo.ModifiedTime = allSchemas[i].ModifiedTime;
                        accessibleSchemas.Add(schemaInfo);
                    }
                }

                return accessibleSchemas;
            }
        }

        /// <summary>
        /// Gets information indicate whether to update an existing metadata model in a safe mode.
        /// </summary>
        /// <value>True to update in a safe mode, false otherwise</value>
        protected bool IsSafeMode
        {
            get
            {
                bool status = false;
                if (_properties != null &&
                    _properties[SAFE_MODE] != null &&
                    ((string)_properties[SAFE_MODE]) == "true")
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Get key/value pairs from the connectionString and save them in a hashtable
        /// </summary>
        /// <param name="connectionString">The connectionString</param>
        /// <returns>The hashtable</returns>
        /// <exception cref="InvalidConnectionStringException">
        /// Thrown if missing some critical key/value pairs in the connection string.
        /// </exception>
        protected Hashtable GetProperties(string connectionString)
        {
            Hashtable properties = new Hashtable();

            // Compile regular expression to find "name = value" pairs
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            MatchCollection matches = regex.Matches(connectionString);
            foreach (Match match in matches)
            {
                int pos = match.Value.IndexOf("=");
                string key = match.Value.Substring(0, pos).TrimEnd();
                string val = match.Value.Substring(pos + 1).TrimStart();
                properties[key] = val;
            }

            return properties;
        }

        /// <summary>
        /// Sets a condition runner used by PermissionChecker.
        /// </summary>
        private void SetConditionRunner()
        {
            if (PermissionChecker.Instance.ConditionRunner == null)
            {
                PermissionChecker.Instance.ConditionRunner = new ServerConditionRunner();
            }
        }

        #endregion Common
	}
}