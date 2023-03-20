/*
* @(#)EventInfo.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Logging
{
	using System;
    using System.Threading;

    using Newtera.Common.MetaData.Logging;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// A class contains information that needs to be written to the database log.
	/// </summary>
	/// <version>1.0.0 07 Jan 2009 </version>
	public class LoggingMessage
	{
        private DateTime _actionTime;
        private LoggingActionType _actionType;
        private string _userName;
        private string _userDisplayText;
        private string[] _userRoles;
        private string _ipAddress;
        private string _schemaId;
        private string _className;
        private string _classCaption;
        private string _oid;
        private string _actionData;

        public LoggingMessage()
        {
            _actionTime = DateTime.Now;
            _actionType = LoggingActionType.Unknown;
            _schemaId = null;
            _className = null;
            _classCaption = null;
            _oid = null;
            _actionData = null;
            _userName = null;
            _userDisplayText = null;
            _userRoles = null;
            _ipAddress = null;
        }

        public LoggingMessage(LoggingActionType actionType, string schemaId, string className, string classCaption, string actionData) :
            this(actionType, schemaId, className, classCaption, null, actionData)
        {
        }

        public LoggingMessage(LoggingActionType actionType, string schemaId, string className, string classCaption, string oid, string actionData)
        {
            _actionTime = DateTime.Now;
            _actionType = actionType;
            _schemaId = schemaId;
            _className = className;
            _classCaption = classCaption;
            _oid = oid;
            _actionData = actionData;

            // Get the CustomPrincipal object from the thread
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal != null)
            {
                _userName = principal.Identity.Name;
                _userDisplayText = principal.DisplayText;
                _userRoles = principal.Roles;
                _ipAddress = principal.IPAddress;
            }
            else
            {
                _userName = null;
                _userDisplayText = null;
                _userRoles = null;
                _ipAddress = null;
            }
        }

        /// <summary>
        /// Gets or sets datetime of the action
        /// </summary>
        public DateTime ActionTime
        {
            get
            {
                return _actionTime;
            }
            set
            {
                _actionTime = value;
            }
        }

        /// <summary>
        /// Gets or sets type of the action, one of LoggingActionType
        /// </summary>
        public LoggingActionType ActionType
        {
            get
            {
                return _actionType;
            }
            set
            {
                _actionType = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the user who perform the action
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        /// <summary>
        /// Gets or sets the display text of the user who perform the action
        /// </summary>
        public string UserDisplayText
        {
            get
            {
                return _userDisplayText;
            }
            set
            {
                _userDisplayText = value;
            }
        }

        /// <summary>
        /// Gets or sets the roles of the user who perform the action
        /// </summary>
        public string[] UserRoles
        {
            get
            {
                return _userRoles;
            }
            set
            {
                _userRoles = value;
            }
        }

        /// <summary>
        /// Gets or sets the ip address from which the user perform the action
        /// </summary>
        public string IPAddress
        {
            get
            {
                return _ipAddress;
            }
            set
            {
                _ipAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of a schema
        /// </summary>
        public string SchemaID
        {
            get
            {
                return _schemaId;
            }
            set
            {
                _schemaId = value;
            }
        }

        /// <summary>
        /// Gets or sets name of class where the event is defined
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        /// <summary>
        /// Gets or sets caption of class where the event is defined
        /// </summary>
        public string ClassCaption
        {
            get
            {
                return _classCaption;
            }
            set
            {
                _classCaption = value;
            }
        }

        /// <summary>
        /// Gets or sets id of the data instance that triggers the event
        /// </summary>
        public string OID
        {
            get
            {
                return _oid;
            }
            set
            {
                _oid = value;
            }
        }

        /// <summary>
        /// Gets or sets the data to be associated with the action, succh as the query statements.
        /// Can be null.
        /// </summary>
        public string ActionData
        {
            get
            {
                return _actionData;
            }
            set
            {
                _actionData = value;
            }
        }
	}
}