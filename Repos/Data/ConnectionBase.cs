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
	using Newtera.Server.Licensing;
    using Newtera.Server.UsrMgr;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData.XaclModel;

	using Infralution.Licensing;

	/// <summary>
	/// Provide common functionality of connections to the Newtera servers.
	/// </summary>
	/// <version>  	1.0.0 15 Dec 2006 </version>
	[LicenseProvider(typeof(NewteraLicenseProvider))]	
	public abstract class ConnectionBase
	{
		// The license parameters is generated using Infralution License Key
		// generator, based on "Newtera E-Catalog" as the product password
		const string _licenseParameters =
			@"<LicenseParameters>
				<RSAKeyValue>
					<Modulus>wTMOkJ5TZ4ghSk5+ah4fWN1PP8zNTtvU1N5IODqNYIxfrT1wloEyaiiUzEnSeywLedTsD0ocvBIHuaUnvdq8FaOPRvE0gE/GbBEeDO2lP+db+PmmOR1UaxbrWUyEk28UfEGlmODLKuPUOTTGnu/UKU8jH/LFwJF65iGE452Cd/8=</Modulus>
					<Exponent>AQAB</Exponent>
				</RSAKeyValue>
				<DesignSignature>Leg88pwXDDZa7QMnGRfBnvmhPgQQ4Xbl3p5XA0dAoOEdySXCexk3alitat6gxch2wVvGaxkhQxBlsd4lCAHOszLdjkKOq/RqLE1Sj7XLCNpzh8cV6cn7Dbxye4cggklgjAag2emCnIXLP6HyLSxoCmRr0RZovr5bDK4WV+Suz38=</DesignSignature>
				<RuntimeSignature>p2hrBzhjESXmDokOt5AUpbx9Cian7jVMhDmt6gwbDGb2F1+dg/oSZzOfe+QY7floX9n/RFtfvlC9pP+Kbt9AkAl/4Q+jFFYaqEe2dkx33XV1XZ099p+V+/R9VIBpiPDq/hbVDKyX3uiOvu1WTlCh4vu2hc8+0pkPqRnG2ZPiCDQ=</RuntimeSignature>
			</LicenseParameters>";

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
        protected License _license = null;
        protected LicenseLevel _licenseLevel = LicenseLevel.Trial;

        #region common

        /// <summary> 
		/// Default constructor
		/// </summary>
		public ConnectionBase()
		{
			// Check the license to see if the license is valid.
			CheckLicense(MessageLevel.None);
			
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

            if (_license != null)
            {
                _license.Dispose();
                _license = null;
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
        /// Gets the number of sessions available for use
        /// </summary>
        public int AvailableSessionNumber
        {
            get
            {
                return EvaluationMonitorManager.Instance.AvailableSessionNumber;
            }
        }

        /// <summary>
        /// Gets the number of advanced sessions available for use
        /// </summary>
        public int AvailableAdvancedSessionNumber
        {
            get
            {
                return EvaluationMonitorManager.Instance.AvailableAdvancedSessionNumber;
            }
        }

        /// <summary>
        /// Decrement available session number by one
        /// </summary>
        public void DecrementSessionNumber()
        {
            EvaluationMonitorManager.Instance.DecrementSessionNumber();

            if (TraceLog.Instance.Enabled)
            {
                try
                {
                    string user = Thread.CurrentPrincipal.Identity.Name;
                    if (string.IsNullOrEmpty(user))
                    {
                        user = "system";
                    }

                    // it is from a web request
                    string[] messages = {"A user license has been obtained by user : " +  user,
                            "IP Address: " + "unknown"};
                    TraceLog.Instance.WriteLines(messages);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Decrement available advanced session number by one
        /// </summary>
        public void DecrementAdvancedSessionNumber()
        {
            EvaluationMonitorManager.Instance.DecrementAdvancedSessionNumber();

            if (TraceLog.Instance.Enabled)
            {
                try
                {
                    string user = Thread.CurrentPrincipal.Identity.Name;
                    if (string.IsNullOrEmpty(user))
                    {
                        user = "system";
                    }

                    // it is from an asp.net request
                    string[] messages = {"An advanced license has been obtained by user : " +  user,
                            "IP Address: " + "unknown"};
                    TraceLog.Instance.WriteLines(messages);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Increment available session number by one
        /// </summary>
        public void IncrementSessionNumber()
        {
            EvaluationMonitorManager.Instance.IncrementSessionNumber();

            if (TraceLog.Instance.Enabled)
            {
                try
                {
                    string user = Thread.CurrentPrincipal.Identity.Name;
                    if (string.IsNullOrEmpty(user))
                    {
                        user = "system";
                    }

                    string IPAddress = "server";

                    // it is from an asp.net request
                    string[] messages = {"A user license has been released by user : " +  user,
                                "IP Address: " + IPAddress};
                    TraceLog.Instance.WriteLines(messages);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Increment available advanced session number by one
        /// </summary>
        public void IncrementAdvancedSessionNumber()
        {
            EvaluationMonitorManager.Instance.IncrementAdvancedSessionNumber();

            if (TraceLog.Instance.Enabled)
            {
                try
                {
                    string user = Thread.CurrentPrincipal.Identity.Name;
                    if (string.IsNullOrEmpty(user))
                    {
                        user = "system";
                    }

                    string IPAddress = "server";

                    // it is from an asp.net request
                    string[] messages = {"An advanced license has been released by user : " +  user,
                                "IP Address: " + IPAddress};
                    TraceLog.Instance.WriteLines(messages);
                }
                catch (Exception)
                {
                }
            }
        }


        /// <summary>
        /// Get localized out of available session message to display
        /// </summary>
        /// <returns></returns>
        public string GetOutOfSessionsMessage()
        {
            ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
            return resources.GetString("OutOfSessions");
        }

        /// <summary>
        /// Get localized out of available advanced session message to display
        /// </summary>
        /// <returns></returns>
        public string GetOutOfAdvancedSessionsMessage()
        {
            ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
            return resources.GetString("OutOfAdvancedSessions");
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

        #region Licensing

        /// <summary>
		/// Gets the message about the server license
		/// </summary>
        /// <param name="messageLevel">One of MessageLevel enum</param>
        public string GetLicenseMsg(MessageLevel messageLevel)
		{
			string licenseMsg = "Unknown Message";

			try
			{
                licenseMsg = CheckLicense(messageLevel);
			}
			catch (Exception ex)
			{
				// license is invalid, do not throw the exception,
				// get the error message instead.
				licenseMsg = ex.Message;
			}
			
			return licenseMsg;
		}

		/// <summary>
		/// Gets the license level
		/// </summary>
		/// <value>One of the LicenseLevel enum values.</value>
		public LicenseLevel LicenseLevel
		{
			get
			{
				return this._licenseLevel;
			}
		}

		/// <summary>
		/// Gets the remaining days of the evaluation.
		/// </summary>
		/// <returns>The number of remaining days, -1 if it is a permenant license.</returns>
		/// <exception cref="NewteraLicenseException">Thrown when there is no valid license.</exception>
		public int GetRemainingEvaluationDays()
		{
			int remainingDays;

			CheckLicense(out remainingDays, MessageLevel.None);

			return remainingDays;
		}

		/// <summary>
		/// Install the license key
		/// </summary>
		/// <param name="licenseKey">The license key</param>
		public static void InstallLicense(string licenseKey)
		{
            if (licenseKey != null && licenseKey.Length > 0)
			{
				// compare the serial number of the new license key with
                // the old one, the serial number of the new license key must
                // be greater than the old license key
				string oldKey = MetaDataCache.Instance.GetLicenseKey();

				if (IsLicenseKeyExpired(licenseKey, oldKey))
				{
					ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
					throw new Exception(resources.GetString("LicenseReused"));
				}

				MetaDataCache.Instance.SetLicenseKey(licenseKey);

				// reset the evaluation monitor to start counting
				NewteraLicenseChecker checker = new NewteraLicenseChecker();

				checker.ResetEvaluationMonitor();
			}
        }

        /// <summary>
        /// Disable the installed license key
        /// </summary>
        /// <returns>The id for transferring the license.</returns>
        public static string DisableLicense()
        {
            string transferId = null;

            // get the transfer id from the installed key
            string licenseKey = MetaDataCache.Instance.GetLicenseKey();

            NewteraLicenseProvider.SetParameters(_licenseParameters);

            NewteraLicenseProvider licenseProvider = new NewteraLicenseProvider();

            EncryptedLicense license = licenseProvider.ValidateLicenseKey(licenseKey);

            if (license == null)
            {
                ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
                throw new Exception(resources.GetString("InvalidLicense"));
            }

            NewteraLicenseChecker checker = new NewteraLicenseChecker();
            transferId = checker.GetParameterValue(license, NewteraLicenseChecker.TRANSFER_ID);

            // disabled the license
            checker.Disabled = true;

            return transferId;
        }

        /// <summary>
        /// Gets the value of a parameter from the license
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter value, null if the parameter does not exits.</returns>
        public string GetLicenseParameterValue(string parameterName)
        {
            string val = null;

            if (_license != null && _license is EncryptedLicense)
            {
                NewteraLicenseChecker checker = new NewteraLicenseChecker();
                val = checker.GetParameterValue((EncryptedLicense)_license, parameterName);
            }

            return val;
        }

        /// <summary>
        /// Gets the information indicating whether the license is of Standalone type
        /// </summary>
        /// <returns>true if it is a standalone license, false otherwise</returns>
        public bool IsStandaloneLicense()
        {
            bool status = false;

            if (_license != null && _license is EncryptedLicense)
            {
                NewteraLicenseChecker checker = new NewteraLicenseChecker();
                status = checker.IsStandaloneLicense((EncryptedLicense)_license);
            }

            return status;
        }

        /// <summary>
        /// Check the license to see if the license is valid
        /// </summary>
        /// <param name="messageLevel">One of MessageLevel enum</param>
        /// <returns>A message with regard to the license.</returns>
        /// <exception cref="NewteraLicenseException">Thrown when the license is invalid or expired.</exception>
        private string CheckLicense(MessageLevel messageLevel)
        {
            int remainingDays;

            return CheckLicense(out remainingDays, messageLevel);
        }

        /// <summary>
        /// Check the license to see if the license is valid
        /// </summary>
        /// <param name="remainingDays">The remaining days of an evaluation.</param>
        /// <param name="messageLevel">One of MessageLevel enum</param>
        /// <returns>A message with regard to the license.</returns>
        /// <exception cref="NewteraLicenseException">Thrown when the license is invalid or expired.</exception>
        private string CheckLicense(out int remainingDays, MessageLevel messageLevel)
        {
            string msg = "";

            remainingDays = -1; // default value for permenant license

            NewteraLicenseProvider.SetParameters(_licenseParameters);

            // check if there is a valid license for the connection
            if (!LicenseManager.IsValid(typeof(ConnectionBase), this, out _license))
            {
                ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
                throw new Exception(resources.GetString("InvalidLicense"));
            }

            // check the product info contained in license key
            if (_license is EncryptedLicense)
            {
                // check the license paramters, if not valid, it will
                // throw an exception
                NewteraLicenseChecker checker = new NewteraLicenseChecker();
                if (checker.Disabled)
                {
                    ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
                    throw new Exception(resources.GetString("LicenseDisabled"));
                }

                msg = checker.Check((EncryptedLicense)_license, out remainingDays, messageLevel);
                _licenseLevel = checker.LicenseLevel;
            }

            return msg;
        }

        /// <summary>
        /// compare the serial number of the new license key with
        /// the old one, the serial number of the new license key must be greater than
        /// the old license key
        /// </summary>
        /// <param name="newLicenseKey"></param>
        /// <param name="oldLicenseKey"></param>
        /// <returns></returns>
        private static bool IsLicenseKeyExpired(string newLicenseKey, string oldLicenseKey)
        {
            bool status = false;

            NewteraLicenseProvider.SetParameters(_licenseParameters);

            NewteraLicenseProvider licenseProvider = new NewteraLicenseProvider();

            EncryptedLicense newLicense = licenseProvider.ValidateLicenseKey(newLicenseKey);
            EncryptedLicense oldLicense = licenseProvider.ValidateLicenseKey(oldLicenseKey);

            if (newLicense == null)
            {
                ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
                throw new Exception(resources.GetString("InvalidLicense"));
            }

            NewteraLicenseChecker checker = new NewteraLicenseChecker();
            string newLicenseComputerIDs = checker.GetParameterValue(newLicense, NewteraLicenseChecker.COMPUTER_ID);
            if (newLicenseComputerIDs != null &&
                !NewteraNameSpace.IsMachineIdMatched(NewteraNameSpace.ComputerCheckSum, newLicenseComputerIDs))
            {
                ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
                throw new Exception(resources.GetString("InvalidServerID"));
            }

            string oldLicenseComputerIDs = checker.GetParameterValue(oldLicense, NewteraLicenseChecker.COMPUTER_ID);

            if (newLicenseComputerIDs != null && oldLicenseComputerIDs != null)
            {
                // Both licenses are permanent license for the same computer id, compare the serial number
                if (newLicenseComputerIDs == oldLicenseComputerIDs &&
                    newLicense.SerialNo <= oldLicense.SerialNo)
                {
                    status = true; // license expired
                }
            }
            else if (newLicenseComputerIDs == null && oldLicenseComputerIDs == null)
            {
                // both licenses are trial license, compare the serial number
                if (newLicense.SerialNo <= oldLicense.SerialNo)
                {
                    status = true; // license expired
                }
            }
            else if (oldLicenseComputerIDs != null && newLicenseComputerIDs == null)
            {
              // changing from fixed id license to a trial license, compare the serial number
              if (newLicense.SerialNo <= oldLicense.SerialNo)
              {
                  status = true; // license expired
              }
            } 
            /*
            else if (oldLicenseComputerID != null && newLicenseComputerID == null)
            {
              // changing from permanent license to trial license is not allowed
              ResourceManager resources = new ResourceManager(typeof(ConnectionBase));
              throw new Exception(resources.GetString("TrailLicenseNotAllowed"));
            }
            */
            else
            {
                // changing from trial license to permanent license is allowed
                status = false;
            }

            return status;
        }

        #endregion Licensing
	}
}