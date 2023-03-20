/*
* @(#)CustomPrincipal.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Principal
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Web;
	using System.Threading;
    using System.Collections;
	using System.Security.Principal;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The sole purpose of CustomPrincipal is to replace the default security
	/// principal and service access control and PrincipalPermission classes.
	/// </summary>
	/// <version>1.0.0 31 Nov 2003</version>
	public class CustomPrincipal : IPrincipal
	{
		private IIdentity _user;
		private IUserManager _userManager;
        private IServerProxy _serverProxy = null;
        private string _displayText = null;
        private string _ipAddress = null;
        private bool _isSilentMode = false; // true if to turn off the logging
        private Hashtable _userData = new Hashtable();
		
		// principal's roles
		private string[] _roles = null;

		//current instance
		private XmlElement currentInstance = null;
		
		//current document
		private XmlDocument currentDocument = null;
		
		// boolean flag to tell whether the document is needed
		private bool needCurrentDocument = false;
		
		// Current db connection used by the previouse query
		private IDbConnection connection = null;

        private IDbTransaction transaction = null;

		/// <summary>
		/// Attach a CustomPrincipal as a current principal.
		/// </summary>
		static public void Attach(IUserManager userManager, IServerProxy serverProxy)
		{
			CustomPrincipal customPrincipal;

            if (Thread.CurrentPrincipal != null)
            {
                // The client is ASP.NET application
                IPrincipal defaultPrincipal = Thread.CurrentPrincipal;
                string userName = defaultPrincipal.Identity.Name;
                // remove the domain name from the login ID if exists
                int pos = userName.LastIndexOf("\\");
                if (pos > 0)
                {
                    userName = userName.Substring(pos + 1);
                }

                IIdentity identity = new GenericIdentity(userName, defaultPrincipal.Identity.AuthenticationType);
                customPrincipal = new CustomPrincipal(identity, userManager, serverProxy);
                customPrincipal.IPAddress = "client";
            }
		}

		/// <summary>
		/// Attach a CustomPrincipal as a current principal. This method is called
		/// by a windows client to attach a CustomPrincipal of a given user name to
		/// the current thread.
		/// </summary>
		static public void Attach(IUserManager userManager, IServerProxy serverProxy, string userName)
		{
			IIdentity identity;
			CustomPrincipal customPrincipal;
			IPrincipal defaultPrincipal = Thread.CurrentPrincipal;

			if (defaultPrincipal is WindowsPrincipal)
			{
				// the client is a Window Form application that call the first time
				AppDomain currentDomain = Thread.GetDomain();

				identity = new GenericIdentity(userName);
                customPrincipal = new CustomPrincipal(identity, userManager, serverProxy);

				// make sure all future threads in this app domain use this principal
				//currentDomain.SetThreadPrincipal(customPrincipal); // didn't work when runing with windows domain service
                Thread.CurrentPrincipal = customPrincipal;
			}
			else
			{
				// the client is either console or ASP.NET application
				identity = new GenericIdentity(userName);
				customPrincipal = new CustomPrincipal(identity, userManager, serverProxy);
			}
		}

		/// <summary>
		/// Initiating an instance of CustomPrincipal class
		/// </summary>
        /// <param name="user">The user</param>
        /// <param name="userManager">The user manager</param>
        /// <param name="serverProxy">The server proxy</param>
		/// <remarks>The constructor is private so that no one can
		/// new a CustomPrincipal instance, except the Attach method.</remarks>
		public CustomPrincipal(IIdentity user, IUserManager userManager, IServerProxy serverProxy)
		{
			_user = user;
			_userManager = userManager;
            _serverProxy = serverProxy;
			_roles = null;

			// Make this object the principal for the thread
			Thread.CurrentPrincipal = this;
		}
		
		/// <summary>
		/// Gets the Identity of the principal
		/// </summary>
		/// <value>IIdentity object</value>
		public IIdentity Identity
		{
			get
			{
				return _user;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the principal is in the
		/// given role.
		/// </summary>
		/// <param name="role">The role</param>
		/// <returns>true if the principal is in given role, false otherwise.</returns>
		public bool IsInRole(string role)
		{
			bool isInRole = false;

			if (_roles == null)
			{
				_roles = GetRoles();
			}

			if (_roles != null)
			{
				foreach (string aRole in _roles)
				{
					if (aRole == role)
					{
						isInRole = true;
						break;
					}
				}
			}

			return isInRole;
		}

		/// <summary>
		/// Gets or sets roles which associated this principal.
		/// </summary>
		/// <value> a collection of roles that the principal is in.</value>
		public string[] Roles
		{
			get
			{
				if (_roles == null)
				{
					_roles = GetRoles();
				}

				return _roles;
			}
			set
			{
				_roles = value;
			}
		}

		/// <summary>
		/// Gets or sets the current xml instance being processed for the user.
		/// </summary>
		/// <value> current instance</value>
		public XmlElement CurrentInstance
		{
			get
			{
				return currentInstance;
			}
			set
			{
				currentInstance = value;
			}
		}

		/// <summary>
		/// Get or sets the current xml document being processed for the user.
		/// </summary>
		/// <value> current document </value>
		public XmlDocument CurrentDocument
		{
			get
			{
				return currentDocument;
			}
			set
			{
				currentDocument = value;
			}
		}

		/// <summary> 
		/// Gets or sets the information indicating whether the current documnet
		/// being processed for the user need cached here.
		/// </summary>
		/// <value> true if it is needed, false otherwise, default is false.</value>
		public bool NeedCurrentDocumentStatus
		{
			get
			{
				return needCurrentDocument;
			}
			set
			{
				needCurrentDocument = value;
			}
		}

		/// <summary>
		/// Gets or sets the current db connection being used by the user.
		/// </summary>
		/// <value> connection object</value>
		public IDbConnection CurrentConnection
		{
			get
			{
				return connection;
			}
			set
			{
				connection = value;
			}
		}

        /// <summary>
        /// Gets or sets the current transaction being used by the user.
        /// </summary>
        /// <value> transaction object</value>
        public IDbTransaction CurrentTransaction
        {
            get
            {
                return transaction;
            }
            set
            {
                transaction = value;
            }
        }

        /// <summary>
        /// Gets the User Manager associated with the Principal
        /// </summary>
        public IUserManager UserManager
        {
            get
            {
                return _userManager;
            }
        }

        /// <summary>
        /// Gets or sets the server proxy which provides the access to server-side information
        /// </summary>
        public IServerProxy ServerProxy
        {
            get
            {
                return _serverProxy;
            }
        }

        /// <summary>
        /// Gets the display text for the principle
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_displayText == null)
                {
                    string userName = _user.Name;
                    string[] userData = _userManager.GetUserData(userName);
                    if (userData != null)
                    {
                        string text = UsersListHandler.GetFormatedName(userData[0], userData[1]);
                        if (string.IsNullOrEmpty(text))
                        {
                            text = userName;
                        }

                        _displayText = text;
                    }
                    else
                    {
                        _displayText = userName;
                    }
                }

                return _displayText;
            }
        }

        /// <summary>
        /// Gets or sets a IP address from where the principle connects to the server
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
        /// Gets or sets the information indicating whether the principle is performing a task in silent mode,
        /// which means no generating log
        /// </summary>
        public bool IsSilentMode
        {
            get
            {
                return _isSilentMode;
            }
            set
            {
                _isSilentMode = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the thread is running on the server side
        /// </summary>
        /// <value>true if it is at server side, false if it is at client side</value>
        public bool IsServerSide
        {
            get
            {
                return _serverProxy.IsServerSide;
            }
        }

        /// <summary>
        /// Get an user data by key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>data in object</returns>
        public object GetUserData(string key)
        {
            return _userData[key];
        }

        /// <summary>
        /// Get an user data by key as a string
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>data in object</returns>
        public string GetUserDataString(string key)
        {
            if (_userData[key] != null)
            {
                return _userData[key].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set an user data
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">an user data</param>
        public void SetUserData(string key, object data)
        {
            _userData[key] = data;
        }

        /// <summary>
        /// Get roles of the user
        /// </summary>
        /// <returns></returns>
        private string[] GetRoles()
		{
            try
            {
                return _userManager.GetRoles(_user.Name);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to call GetRoles method for user:" + _user.Name + " due to " + ex.Message);
            }
		}
	}
}