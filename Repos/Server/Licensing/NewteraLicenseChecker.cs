/*
* @(#)NewteraLicenseChecker.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Licensing
{
	using System;
	using System.Text;
	using System.Resources;
	using System.Security.Cryptography;
	using System.ComponentModel;
	using System.Text.RegularExpressions;

	using Newtera.Common.Core;
	using Newtera.Server.DB;

	using Infralution.Licensing;

	/// <summary> 
	/// An class that validates the license.
	/// </summary>
	/// <version> 1.0.0 23 Sep 2005 </version>
	/// <author> Yong Zhang </author>
	public class NewteraLicenseChecker
	{
		internal static int DAYS_ELAPSED = 0;
		internal static int DEFAULT_CLIENT_NUMBER = 1;
        internal static int DEFAULT_DURATION = 180;

		/// <summary>
		/// EVALUATION DURATION
		/// </summary>
		public const string DURATION = "DAYS";

		/// <summary>
		/// COMPUTER UNIQUE ID
		/// </summary>
		public const string COMPUTER_ID = "ID";

		/// <summary>
		/// License Level
		/// </summary>
		public const string LEVEL = "L";

        /// <summary>
        /// Connection numbers
        /// </summary>
        public const string CONNECTION_NUMBER = "CN";

        /// <summary>
        /// Advanced Connection numbers
        /// </summary>
        public const string ADVANCED_CONNECTION_NUMBER = "AN";

        /// <summary>
        /// Transfer id
        /// </summary>
        public const string TRANSFER_ID = "TI";

        /// <summary>
        /// Integration enabled?
        /// </summary>
        public const string INTEGRATION_ENABLED = "I";

        /// <summary>
        /// Is Permanent license?
        /// </summary>
        public const string PERMANENT_LICENSE = "P";

        /// <summary>
        /// Number of DesignStudio Clients
        /// </summary>
        public const string DS = "DS";

        /// <summary>
        /// Number of WorkflowStudio Clients
        /// </summary>
        public const string WS = "WS";

        /// <summary>
        /// Number of SmartWord or Smart Excel Clients
        /// </summary>
        public const string SMART_DOCS = "SW";

        /// <summary>
        /// Nax Number of Trial Days
        /// </summary>
        public const int MAX_DAYS = 365;

        private NewteraLicenseInfo _licenseInfo;

        /// <summary>
		/// Initializing a NewteraLicenseChecker object
		/// </summary>
        public NewteraLicenseChecker()
		{
            _licenseInfo = new NewteraLicenseInfo();
		}

		/// <summary>
		/// Gets the license level
		/// </summary>
		public LicenseLevel LicenseLevel
		{
			get
			{
				return _licenseInfo.LicenseLevel;
			}
		}

        /// <summary>
        /// Gets the session number available
        /// </summary>
        public int SessionNumber
        {
            get
            {
                return _licenseInfo.SessionNumber;
            }
        }

        /// <summary>
        /// Gets the advanced session number available
        /// </summary>
        public int AdvancedSessionNumber
        {
            get
            {
                return _licenseInfo.AdvancedSessionNumber;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the license is disabled.
        /// </summary>
        public bool Disabled
        {
            get
            {
                Infralution.Licensing.EvaluationMonitor monitor = EvaluationMonitorManager.Instance.Monitor;

                return monitor.Disabled;
            }
            set
            {
                Infralution.Licensing.EvaluationMonitor monitor = EvaluationMonitorManager.Instance.Monitor;

                monitor.Disabled = value;
            }
        }

		/// <summary>
		/// Gets the days that have been elapsed for an evaluation period
		/// </summary>
		public int GetElapsedDays()
		{
            // create a new monitor so that it can detect if the computer time has been wind back or not
            Infralution.Licensing.EvaluationMonitor monitor = EvaluationMonitorManager.Instance.Monitor;

            monitor.SetLastUsedDate(); // set today as the last used date

            if (monitor.Invalid)
            {
                ResourceManager resources = new ResourceManager(this.GetType());
                // the computer time has been winded back, throw an exception
                throw new NewteraLicenseException(resources.GetString("ComputerTimeChanged"));
            }

            return monitor.DaysInUse;
		}

		/// <summary>
		/// Reset the evaluation monitor to start from scratch.
		/// </summary>
		public void ResetEvaluationMonitor()
		{
            Infralution.Licensing.EvaluationMonitor monitor = EvaluationMonitorManager.Instance.Monitor;

			monitor.Reset();

            EvaluationMonitorManager.Instance.Reset(); // re-create the monitor
		}

		/// <summary>
		/// Check the license based on the parameters contained in it.
		/// </summary>
		/// <param name="license">The encrypted license</param>
		/// <param name="remainingDays">The remaining days of an evaluation.</param>
        /// <param name="messageLevel">One of MessageLevel enum</param>
		/// <returns>The string describing license message.</returns>
		/// <exception cref="NewteraLicenseException">A NewteraLicenseException is thrown when license is invalid or expired.</exception>
        public string Check(EncryptedLicense license, out int remainingDays, MessageLevel messageLevel)
		{
			string msg = "";
			remainingDays = _licenseInfo.RemainingDays; // default
            int elapsedDays;
			bool embeddedDBOnly = true;
            bool hasDaysParameter = false;
            bool isPermanentLicense = false;
			_licenseInfo.Days = DEFAULT_DURATION; // default duration
            _licenseInfo.SerialNo = license.SerialNo.ToString();

			ResourceManager resources = new ResourceManager(this.GetType());

			// Compile regular expression to find "name = value" pairs in the
			// product info string
			Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

			string name, val;
			int pos;

			MatchCollection matches = regex.Matches(license.ProductInfo);

			// check license parameters
			foreach (Match match in matches)
			{
				pos = match.Value.IndexOf("=");
				name = match.Value.Substring(0, pos).TrimEnd();
				val = match.Value.Substring(pos + 1).TrimStart();

				switch (name.ToUpper())
				{
					case NewteraLicenseChecker.COMPUTER_ID:
                        if (!NewteraNameSpace.IsMachineIdMatched(NewteraNameSpace.ComputerCheckSum, val))
						{
                            // Machine Id isn't match, determine whether to throw exception later
                            _licenseInfo.HasMatchedID = false;
                            _licenseInfo.ServerID = val;
						}
						else
						{
							_licenseInfo.HasMatchedID = true;
                            _licenseInfo.ServerID = val;
						}

						break;

					case NewteraLicenseChecker.LEVEL:

						switch (val)
						{
							case "E":
								_licenseInfo.LicenseLevel = LicenseLevel.Enterprise;
								embeddedDBOnly = false; // it's an enterprise version, therefore, support all database types
								break;

							case "D":
                                _licenseInfo.LicenseLevel = LicenseLevel.Department;
								embeddedDBOnly = true; 
								break;

							case "S":
                                _licenseInfo.LicenseLevel = LicenseLevel.Standalone;
                                embeddedDBOnly = true;  // allow only embedded database
								break;
						}
						
						break;

                    case NewteraLicenseChecker.CONNECTION_NUMBER:
            			try
						{
							_licenseInfo.SessionNumber = Int32.Parse(val);
						}
						catch (Exception)
						{
							_licenseInfo.SessionNumber = 1;
						}
						
						break;

                    case NewteraLicenseChecker.ADVANCED_CONNECTION_NUMBER:
                        try
                        {
                            _licenseInfo.AdvancedSessionNumber = Int32.Parse(val);
                        }
                        catch (Exception)
                        {
                            _licenseInfo.AdvancedSessionNumber = 1;
                        }

                        break;

					case NewteraLicenseChecker.DURATION:
					
						try
						{
                            _licenseInfo.Days = Int32.Parse(val);
                            if (_licenseInfo.Days > MAX_DAYS)
                            {
                                _licenseInfo.Days = MAX_DAYS;
                            }
						}
						catch (Exception)
						{
                            _licenseInfo.Days = DEFAULT_DURATION;
						}

                        hasDaysParameter = true;

                        elapsedDays = GetElapsedDays();

                        if (elapsedDays < 0 || elapsedDays >= _licenseInfo.Days)
						{
                            if (messageLevel == MessageLevel.Detailed)
                            {
                                _licenseInfo.RemainingDays = 0;
                            }
                            else
                            {
                                // license expired, throw an exception
                                throw new NewteraLicenseException(resources.GetString("EvalExpired"));
                            }
						}
						else
						{
                            _licenseInfo.RemainingDays = _licenseInfo.Days - elapsedDays;
						}

						break;

                    case NewteraLicenseChecker.DS:

                        if (messageLevel == MessageLevel.Detailed)
                        {
                            try
                            {
                                _licenseInfo.DesignStudioClients = Int32.Parse(val);
                            }
                            catch (Exception)
                            {
                                _licenseInfo.DesignStudioClients = 1;
                            }
                        }

                        break;

                    case NewteraLicenseChecker.WS:

                        if (messageLevel == MessageLevel.Detailed)
                        {
                            try
                            {
                                _licenseInfo.WorkflowStudioClients = Int32.Parse(val);
                            }
                            catch (Exception)
                            {
                                _licenseInfo.WorkflowStudioClients = 1;
                            }
                        }

                        break;

                    case NewteraLicenseChecker.SMART_DOCS:

                        if (messageLevel == MessageLevel.Detailed)
                        {
                            try
                            {
                                _licenseInfo.SmartDocsClients = Int32.Parse(val);
                            }
                            catch (Exception)
                            {
                                _licenseInfo.SmartDocsClients = 1;
                            }
                        }

                        break;

                    case NewteraLicenseChecker.INTEGRATION_ENABLED:

                        if (messageLevel == MessageLevel.Detailed)
                        {
                            if (!string.IsNullOrEmpty(val) &&
                                val == "Y")
                            {
                                _licenseInfo.AllowIntegration = true;
                            }
                            else
                            {
                                _licenseInfo.AllowIntegration = false;
                            }
                        }

                        break;

                    case NewteraLicenseChecker.PERMANENT_LICENSE:

                        if (!string.IsNullOrEmpty(val) &&
                            val == "Y")
                        {
                            isPermanentLicense = true;
                            _licenseInfo.HasTimeLimit = false;
                        }
                        else
                        {
                            isPermanentLicense = false;
                            _licenseInfo.HasTimeLimit = true;
                        }

                        break;
				}
			}

            // throw an exception if the machine id doesn't matche the license id spec when the license isn't trial or standalone level
            if (!_licenseInfo.HasMatchedID &&
                messageLevel != MessageLevel.Detailed &&
                _licenseInfo.LicenseLevel != LicenseLevel.Trial &&
                _licenseInfo.LicenseLevel != LicenseLevel.Standalone)
            {
                // the computer id doesn't match, throw an exception
                throw new NewteraLicenseException(resources.GetString("UnmatchedID"));
            }

            // double check if time limit
            if (!hasDaysParameter && !isPermanentLicense)
            {
                _licenseInfo.Days = DEFAULT_DURATION;

                elapsedDays = GetElapsedDays();

                if (elapsedDays < 0 || elapsedDays >= _licenseInfo.Days)
                {
                    if (messageLevel == MessageLevel.Detailed)
                    {
                        _licenseInfo.RemainingDays = 0;
                    }
                    else
                    {
                        // license expired, throw an exception
                        throw new NewteraLicenseException(resources.GetString("EvalExpired"));
                    }
                }
                else
                {
                    _licenseInfo.RemainingDays = _licenseInfo.Days - elapsedDays;
                }
            }

			// check database support is allowed by the license
			DatabaseType dbType = DatabaseConfig.Instance.GetDatabaseType();
			if (embeddedDBOnly &&  dbType != DatabaseType.SQLServerCE)
			{
				string message = String.Format(resources.GetString("UnsupportedDatabase"), Enum.GetName(typeof(DatabaseType), dbType));

				// unsupported database type, throw an exception
				throw new NewteraLicenseException(message);
			}

            msg = _licenseInfo.GetMessage(messageLevel);

            remainingDays = _licenseInfo.RemainingDays;
			return msg;
		}

		/// <summary>
		/// Check if existing registered clients reach or exceed the licensed number.
		/// </summary>
		/// <param name="existingNum">The existing client number</param>
		/// <param name="licensedNumStr">The licensed client number</param>
		/// <exception cref="Exception">An exception is thrown if it reaches or exceeds the license limit.</exception>
		public void CheckClientLimit(int existingNum, string licensedNumStr)
		{
			int licensedNum = DEFAULT_CLIENT_NUMBER;
			if (licensedNumStr != null)
			{
				try
				{
					licensedNum = Int32.Parse(licensedNumStr);
				}
				catch (Exception)
				{
					licensedNum = DEFAULT_CLIENT_NUMBER;
				}
			}

			if (existingNum >= licensedNum)
			{
				ResourceManager resources = new ResourceManager(this.GetType());

				throw new NewteraLicenseException(resources.GetString("ClientLimitExceeded"));
			}
		}

		/// <summary>
		/// Check in the given client, throw an exception if check in failed.
		/// </summary>
		/// <param name="clientIds">A list of registered clients</param>
        /// <param name="licensedNumStr">The licensed client number</param>
		/// <param name="clientName">The client name</param>
		/// <param name="clientId">The client id</param>
		/// <exception cref="NewteraLicenseException">Thrown if check in the client failed.</exception>
		public void CheckInClient(string[] clientIds, string licensedNumStr, string clientName, string clientId)
		{
			bool status = false;
			ResourceManager resources = new ResourceManager(this.GetType());

            if (clientIds != null)
            {
                int licensedNum = DEFAULT_CLIENT_NUMBER;
                if (licensedNumStr != null)
                {
                    try
                    {
                        licensedNum = Int32.Parse(licensedNumStr);
                    }
                    catch (Exception)
                    {
                        licensedNum = DEFAULT_CLIENT_NUMBER;
                    }
                }

                if (clientIds.Length > licensedNum)
                {
                    string message = String.Format(resources.GetString("TooManyRegisteredClients"));
                    throw new NewteraLicenseException(message);
                }

                for (int i = 0; i < clientIds.Length; i++)
                {
                    if (clientIds[i] == clientId)
                    {
                        status = true;
                        break;
                    }
                }
            }

			if (!status)
			{
				string message = String.Format(resources.GetString("CheckInRegisteredClient"), clientId);
				throw new NewteraLicenseException(message);
			}
		}

		/// <summary>
		/// Get value of a parameter contained in license product info.
		/// </summary>
		/// <param name="license">The encrypted license</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <returns>The parameter value, null if the parameter does not exist.</returns>
		public string GetParameterValue(EncryptedLicense license, string parameterName)
		{
			// Compile regular expression to find "name = value" pairs in the
			// product info string
			Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

			string name, val;
			string parameterValue = null;
			int pos;

			MatchCollection matches = regex.Matches(license.ProductInfo);

			// check if it is a licensed installation
			foreach (Match match in matches)
			{
				pos = match.Value.IndexOf("=");
				name = match.Value.Substring(0, pos).TrimEnd();
				val = match.Value.Substring(pos + 1).TrimStart();

				if (name.ToUpper() == parameterName.ToUpper())
				{
					parameterValue = val;

					break;
				}
			}

			return parameterValue;
		}

        /// <summary>
        /// Gets the information indicating whether the license is of Standalone type
        /// </summary>
        /// <returns>true if it is a standalone license, false otherwise</returns>
        public bool IsStandaloneLicense(EncryptedLicense license)
        {
            bool status = false;

            // Compile regular expression to find "name = value" pairs in the
            // product info string
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            string name, val;
            int pos;

            MatchCollection matches = regex.Matches(license.ProductInfo);

            foreach (Match match in matches)
            {
                pos = match.Value.IndexOf("=");
                name = match.Value.Substring(0, pos).TrimEnd();
                val = match.Value.Substring(pos + 1).TrimStart();

                if (name.ToUpper() == NewteraLicenseChecker.LEVEL && val.ToUpper() == "S")
                {
                    status = true;

                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Get message of the licensing status about a particular client
		/// </summary>
		/// <param name="clientIds">A list of registered clients</param>
		/// <param name="clientName">The client name</param>
		/// <param name="clientId">The client id</param>
		/// <returns>The registration message.</returns>
		public string GetClientRegistrationMessage(string[] clientIds, string clientName, string clientId)
		{
			bool status = false;
			string message;
			ResourceManager resources = new ResourceManager(this.GetType());

            if (clientIds != null)
            {
                for (int i = 0; i < clientIds.Length; i++)
                {
                    if (clientIds[i] == clientId)
                    {
                        status = true;
                        break;
                    }
                }
            }

			if (status)
			{
				message = String.Format(resources.GetString("RegisteredClient"), clientId);
			}
			else
			{
				message = String.Format(resources.GetString("UnregisteredClient"), clientId);
			}

			return message;
		}
	}

	/// <summary>
	/// Enum definition for license level
	/// </summary>
	public enum LicenseLevel
	{
		/// <summary>
		/// Trial license, default
		/// </summary>
		Trial,
        /// <summary>
        /// Standalone license, L=S
        /// </summary>
        Standalone,
		/// <summary>
		/// Department license, L=D
		/// </summary>
		Department,
		/// <summary>
		/// Enterprise license, L=E
		/// </summary>
		Enterprise
	}

    /// <summary>
    /// Enum definition for license message level
    /// </summary>
    public enum MessageLevel
    {
        /// <summary>
        /// No message
        /// </summary>
        None,
        /// <summary>
        /// Brief message
        /// </summary>
        Brief,
        /// <summary>
        /// Detailed message
        /// </summary>
        Detailed
    }
}