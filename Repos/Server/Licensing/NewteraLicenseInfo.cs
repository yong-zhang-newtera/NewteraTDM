/*
* @(#)NewteraLicenseInfo.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Licensing
{
	using System;
    using System.Text;
	using System.IO;
    using System.Resources;
	using System.ComponentModel;
	using System.Configuration;

	using Newtera.Common.Core;
    using Newtera.Server.Engine.Cache;

	/// <summary> 
	/// A placeholder for the license information
	/// </summary>
	/// <version> 1.0.0 02 July 2008 </version>
	public class NewteraLicenseInfo
	{
        private LicenseLevel _level = LicenseLevel.Trial;
        private string _serverId = null;
        private string _serialNo = "0";
        private int _days = 0;
        private bool _hasMatchedID = true;
        private bool _hasTimeLimit = true;
        private int _designStudioClients = 1;
        private int _workflowStudioClients = 1;
        private int _smartDosClients = 1;
        private bool _allowIntegration = false;
        private int _remainingDays = 30;
        private int _remainingDesignStudioClients = 1;
        private int _remainingWorkflowStudioClients = 1;
        private int _remainingSmartDocClients = 1;

        public LicenseLevel LicenseLevel
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        public string ServerID
        {
            get
            {
                return _serverId;
            }
            set
            {
                _serverId = value;
            }
        }

        public string SerialNo
        {
            get
            {
                return _serialNo;
            }
            set
            {
                _serialNo = value;
            }
        }

        public int Days
        {
            get
            {
                return _days;
            }
            set
            {
                _days = value;
            }
        }

        public int RemainingDays
        {
            get
            {
                return _remainingDays;
            }
            set
            {
                _remainingDays = value;
            }
        }

        public int RemainingSessions
        {
            get
            {
                return EvaluationMonitorManager.Instance.AvailableSessionNumber;
            }
        }

        public bool HasMatchedID
        {
            get
            {
                return _hasMatchedID;
            }
            set
            {
                _hasMatchedID = value;
            }
        }

        public bool HasTimeLimit
        {
            get
            {
                return _hasTimeLimit;
            }
            set
            {
                _hasTimeLimit = value;
            }
        }

        public int SessionNumber
        {
            get
            {
                return EvaluationMonitorManager.Instance.MaxSessionNumber;
            }
            set
            {
                EvaluationMonitorManager.Instance.MaxSessionNumber = value;
            }
        }

        public int AdvancedSessionNumber
        {
            get
            {
                return EvaluationMonitorManager.Instance.MaxAdvancedSessionNumber;
            }
            set
            {
                EvaluationMonitorManager.Instance.MaxAdvancedSessionNumber = value;
            }
        }


        public int DesignStudioClients
        {
            get
            {
                return _designStudioClients;
            }
            set
            {
                _designStudioClients = value;
            }
        }

        public int WorkflowStudioClients
        {
            get
            {
                return _workflowStudioClients;
            }
            set
            {
                _workflowStudioClients = value;
            }
        }

        public int SmartDocsClients
        {
            get
            {
                return _smartDosClients;
            }
            set
            {
                _smartDosClients = value;
            }
        }

        public bool AllowIntegration
        {
            get
            {
                return _allowIntegration;
            }
            set
            {
                _allowIntegration = value;
            }
        }

        public int RemainingDesignStudioClients
        {
            get
            {
                return _remainingDesignStudioClients;
            }
            set
            {
                _remainingDesignStudioClients = value;
            }
        }

        public int RemainingWorkflowStudioClients
        {
            get
            {
                return _remainingWorkflowStudioClients;
            }
            set
            {
                _remainingWorkflowStudioClients = value;
            }
        }

        public int RemainingSmartDocClients
        {
            get
            {
                return _remainingSmartDocClients;
            }
            set
            {
                _remainingSmartDocClients = value;
            }
        }

		/// <summary>
		/// Get the formated license message
		/// </summary>
        /// <param name="messageLevel">One of MessageLevel enum</param>
        /// <returns>The license message</returns>
		public string GetMessage(MessageLevel messageLevel)
		{
			string msg = "";

            try
            {
                switch (messageLevel)
                {
                    case MessageLevel.None:
                        break;

                    case MessageLevel.Brief:

                        msg = GetBriefMessage();
                        break;

                    case MessageLevel.Detailed:

                        msg = GetDetailedMessage();
                        break;
                }
            }
            catch (Exception)
            {
                // ignore the error
            }

            return msg;
		}

        // Get a short version of license message
        private string GetBriefMessage()
        {
            string msg = "";

            ResourceManager resources = new ResourceManager("Newtera.Server.Licensing.NewteraLicenseChecker",
                        typeof(Newtera.Server.Licensing.NewteraLicenseChecker).Assembly);

            string levelStr = resources.GetString(Enum.GetName(typeof(LicenseLevel), LicenseLevel));

            if (HasMatchedID)
            {
                if (!HasTimeLimit)
                {
                    if (LicenseLevel == LicenseLevel.Trial)
                    {
                        // Department license is the default for a permanent license
                        LicenseLevel = LicenseLevel.Department;
                    }

                    levelStr = resources.GetString(Enum.GetName(typeof(LicenseLevel), LicenseLevel));

                    msg = String.Format(resources.GetString("PermanentLicense"), levelStr, NewteraNameSpace.ComputerCheckSum, SerialNo);
                }
                else
                {
                    msg = String.Format(resources.GetString("DedicatedLicense"), levelStr, NewteraNameSpace.ComputerCheckSum, Days, RemainingDays, SerialNo);
                }
            }
            else
            {
                msg = String.Format(resources.GetString("TempLicense"), levelStr, Days, RemainingDays, SerialNo);
            }

            return msg;
        }

        // Get a detailed version of license message
        private string GetDetailedMessage()
        {
            int remains;
            string[] registeredClients;
            StringBuilder builder = new StringBuilder();

            ResourceManager resources = new ResourceManager("Newtera.Server.Licensing.NewteraLicenseChecker",
                         typeof(Newtera.Server.Licensing.NewteraLicenseChecker).Assembly);

            string msg;

            if (LicenseLevel == LicenseLevel.Trial)
            {
                // Department license is the default for a trial license
                msg = resources.GetString(Enum.GetName(typeof(LicenseLevel), LicenseLevel.Department));
            }
            else
            {
                msg = resources.GetString(Enum.GetName(typeof(LicenseLevel), LicenseLevel));
            }

            // get server level, such as standalone, department, or enterprise
            builder.Append(resources.GetString("ServerLevel")).Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            // Server ID
            if (HasMatchedID)
            {
                msg = ServerID;
            }
            else
            {
                msg = resources.GetString("Unknown");
            }

            builder.Append(resources.GetString("ServerID")).Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);


            // get server function, such as data management, or data&workflow management
            if (WorkflowStudioClients > 0)
            {
                msg = resources.GetString("DataAndWorkflow");
            }
            else
            {
                msg = resources.GetString("DataOnly");
            }

            builder.Append(resources.GetString("ServerFunction")).Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            builder.Append(resources.GetString("SerialNo")).Append(SerialNo).Append(Environment.NewLine).Append(Environment.NewLine);

            // max allowed users
            builder.Append(resources.GetString("MaxSessions")).Append(SessionNumber.ToString());
            
            msg = string.Format(resources.GetString("RemainingSessions"), RemainingSessions);
            builder.Append(" ").Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            // Time limits
            if (HasTimeLimit)
            {
                msg = Days.ToString();
            }
            else
            {
                msg = resources.GetString("Unknown");
            }

            builder.Append(resources.GetString("TimeLimit")).Append(msg);
            if (HasTimeLimit)
            {
                msg = string.Format(resources.GetString("RemainingDays"), RemainingDays);
                builder.Append(" ").Append(msg);
            }
            builder.Append(Environment.NewLine).Append(Environment.NewLine);

            // DesignStudio clients
            builder.Append(resources.GetString("DesignStudio")).Append(DesignStudioClients.ToString());
            registeredClients = MetaDataCache.Instance.GetRegisteredClients(NewteraNameSpace.DESIGN_STUDIO_NAME);
            remains = DesignStudioClients - registeredClients.Length;
            if (remains < 0)
            {
                remains = 0;
            }
            RemainingDesignStudioClients = remains;
            msg = string.Format(resources.GetString("RemainingDesignStudios"), RemainingDesignStudioClients);
            builder.Append(" ").Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            // WorkflowStudio clients
            builder.Append(resources.GetString("WorkflowStudio")).Append(WorkflowStudioClients.ToString());
            registeredClients = MetaDataCache.Instance.GetRegisteredClients(NewteraNameSpace.WORKFLOW_STUDIO_NAME);
            remains = WorkflowStudioClients - registeredClients.Length;
            if (remains < 0)
            {
                remains = 0;
            }

            RemainingWorkflowStudioClients = remains;
            msg = string.Format(resources.GetString("RemainingWorkflowStudios"), RemainingWorkflowStudioClients);
            builder.Append(" ").Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            // SmartDocs clients
            builder.Append(resources.GetString("SmartDocs")).Append(this.SmartDocsClients.ToString());
            registeredClients = MetaDataCache.Instance.GetRegisteredClients(NewteraNameSpace.SMART_WORD_NAME);
            remains = SmartDocsClients - registeredClients.Length;
            if (remains < 0)
            {
                remains = 0;
            }

            RemainingSmartDocClients = remains;
            msg = string.Format(resources.GetString("RemainingSmartDocs"), RemainingSmartDocClients);
            builder.Append(" ").Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            // SmartDocs clients
            if (this.AllowIntegration)
            {
                msg = "Yes";
            }
            else
            {
                msg = "No";
            }

            builder.Append(resources.GetString("AllowIntegration")).Append(msg).Append(Environment.NewLine).Append(Environment.NewLine);

            return builder.ToString();
        }
	}
}