/*
* @(#)CMUserManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.UsrMgr
{
	using System;
	using System.Data;
	using System.Xml;
	using System.Web;
	using System.Text;
	using System.Threading;
	using System.Configuration;
    using System.Collections.Specialized;
	using System.Text.RegularExpressions;
	using System.Security.Principal;
	
	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Cache;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;

	/// <summary> 
	/// Newtera Implementation of IUserManager that queries database directly
	/// for user info.
	/// </summary>
	/// <version> 1.0.0 01 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class CMUserManager : IUserManager
	{
        private const string NEW_USER_INFO_VERSION = "3.1.0";
		private const string USER_INFO_SCHEMA = "UserInfoSchema";
		private const string CONNECTION_STRING = "DefaultConnectionString";
		private const string USER_INFO_CONNECTION_STRING = "SCHEMA_NAME=UserInfo;SCHEMA_VERSION=1.0";
		private const string SCHEMA_NAME ="SCHEMA_NAME";
		private const string SCHEMA_VERSION = "SCHEMA_VERSION";
        private const string USER_INFO_SCHEMA_ID = "USERINFO 1.0";
        private const string USER_CLASS_NAME = "User";
        private const string PASSWORD_ATTRIBUTE_NAME = "Password";
        private const int PAGE_SIZE = 1000;

		private static SchemaInfo _schemaInfo = null;
		private static IPrincipal _superUser = null;

		private bool _isQueryInProcess;

		private const string AuthenticateQuery = "document(\"db://UserInfo.xml\")/UserList/User[ID = \"param1\"]";
		private const string GetUserRolesQuery = "for $user in document(\"db://UserInfo.xml\")/UserList/User let $roles := $user/@roles=>UserRole/@role=>Role where $user/ID = \"param1\" return $roles/Name";
        private const string GetTypeRolesQuery = "for $role in document(\"db://UserInfo.xml\")/RoleList/Role where $role/RType = \"param1\" and $role/@users=>UserRole/@user=>User/ID = \"param2\" return $role/Name";
        private const string GetUserDataQuery = "document(\"db://UserInfo.xml\")/UserList/User[ID = \"param1\"]";
        private const string GetUserDataQueryByKey = "document(\"db://UserInfo.xml\")/UserList/User[Key = \"param1\"]";
        private const string GetRoleDataQuery = "document(\"db://UserInfo.xml\")/RoleList/Role[Name = \"param1\"]";
        private const string GetAllRolesQuery = "for $role in document(\"db://UserInfo.xml\")/RoleList/Role[start to end] return $role sortby ($role/Name)";
        private const string GetRoleUsersQuery = "for $role in document(\"db://UserInfo.xml\")/RoleList/Role let $users := $role/@users=>UserRole/@user=>User where $role/Name = \"param1\" return $users/ID sortby ($users/ID)";
        private const string GetAllUsersQuery = "for $user in document(\"db://UserInfo.xml\")/UserList/User[start to end] return $user sortby ($user/ID)";
		private const string AddUserQuery = "let $u := [[<User xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"User\"><ID>param1</ID><Password>param2</Password></User>]] return addInstance(document(\"db://UserInfo.xml\"), $u)";
        private const string AddUserInfoQuery = "let $u := [[<User xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"User\"><ID>param1</ID><Password>param2</Password><LastName>param3</LastName><FirstName>param4</FirstName><Email>param5</Email><PhoneNumber>param6</PhoneNumber><Key>param7</Key><SecurityStamp>param8</SecurityStamp></User>]] return addInstance(document(\"db://UserInfo.xml\"), $u)";
        private const string ChangeUserPwdQuery = "for $u in document(\"db://UserInfo.xml\")/UserList/User where $u/ID = \"param1\" return (setText($u/Password, \"param2\"), updateInstance(document(\"db://UserInfo.xml\"), $u))";
        private const string ChangeUserDataQuery = "for $u in document(\"db://UserInfo.xml\")/UserList/User where $u/ID = \"param1\" return (setText($u/LastName, \"param2\"), setText($u/FirstName, \"param3\"), setText($u/Email, \"param4\"), setText($u/PhoneNumber, \"param5\"), setText($u/Picture, \"param6\"), updateInstance(document(\"db://UserInfo.xml\"), $u))";
        private const string DeleteUserQuery = "for $u in document(\"db://UserInfo.xml\")/UserList/User where $u/ID = \"param1\" return deleteInstance(document(\"db://UserInfo.xml\"), $u)";
        private const string AddRoleQuery = "let $r := [[<Role xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"Role\"><Name>param1</Name><Text>param2</Text><RType>param3</RType></Role>]] return addInstance(document(\"db://UserInfo.xml\"), $r)";				
		private const string DeleteRoleQuery = "for $r in document(\"db://UserInfo.xml\")/RoleList/Role where $r/Name = \"param1\" return deleteInstance(document(\"db://UserInfo.xml\"), $r)";
		private const string AddUserRoleMapQuery = "let $map := [[<UserRole xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\" xsi:type=\"UserRole\"><user><ID>param1</ID></user><role><Name>param2</Name></role></UserRole>]] return addInstance(document(\"db://UserInfo.xml\"), $map)";				
		private const string DeleteUserRoleMapQuery = "for $map in document(\"db://UserInfo.xml\")/UserRoleList/UserRole where $map/@user=>User/ID = \"param1\" and $map/@role=>Role/Name = \"param2\" return deleteInstance(document(\"db://UserInfo.xml\"), $map)";
		private const string DeleteUserRoleMapByUserQuery = "for $map in document(\"db://UserInfo.xml\")/UserRoleList/UserRole where $map/@user=>User/ID = \"param1\" return deleteInstance(document(\"db://UserInfo.xml\"), $map)";
		private const string DeleteUserRoleMapByRoleQuery = "for $map in document(\"db://UserInfo.xml\")/UserRoleList/UserRole where $map/@role=>Role/Name = \"param1\" return deleteInstance(document(\"db://UserInfo.xml\"), $map)";
        private const string GetUserEmailQuery = "for $user in document(\"db://UserInfo.xml\")/UserList/User where $user/ID = \"param1\" return $user/Email";
        private const string ChangeRoleDataQuery = "for $r in document(\"db://UserInfo.xml\")/RoleList/Role where $r/Name = \"param1\" return (setText($r/Text, \"param2\"), updateInstance(document(\"db://UserInfo.xml\"), $r))";
        private const string GetParentRoleQuery = "document(\"db://UserInfo.xml\")/RoleList/Role[Name = \"param1\"]/@parentRole=>Role";

		/// <summary>
		/// Initiate an instance of CMUserManager class
		/// </summary>
		public CMUserManager()
		{
			if (_superUser == null)
			{
				IIdentity identity = new GenericIdentity(GetSuperUserName()); 
				_superUser = new CustomPrincipal(identity, this, new ServerSideServerProxy());

				// set the super user roles
				((CustomPrincipal) _superUser).Roles = this.GetSuperUserRole();
			}

			_isQueryInProcess = false;
		}

		/// <summary>
		/// Gets the information indicating whether the schema for UserInfo exists or not
		/// </summary>
		/// <value>true if it exists, false otherwise</value>
		public bool IsUserInfoSchemaExist
		{
			get
			{
				if (_schemaInfo == null)
				{
					_schemaInfo = ParseConnectionString(UserInfoConnectionString);
				}

				return MetaDataCache.Instance.IsSchemaExisted(_schemaInfo, DataProviderFactory.Instance.Create());
			}
		}

		/// <summary>
		/// Gets the connection string for UserInfo schema
		/// </summary>
		/// <value>The connection string for UserInfo schema</value>
		public string UserInfoConnectionString
		{
			get
			{
                string connectionString = ConfigurationManager.AppSettings[USER_INFO_SCHEMA];

				if (connectionString == null || connectionString.Length == 0)
				{
					// if USER_INFO_SCHEMA key is not defined, assume that
					// user info schema is part of the application schema
                    connectionString = ConfigurationManager.AppSettings[CONNECTION_STRING];
				}

				//TODO: Do we need to read connection string from config file?
				connectionString = USER_INFO_CONNECTION_STRING;

				return connectionString;
			}
		}

        /// <summary>
        /// Gets the super user of Newtera Server
        /// </summary>
        public IPrincipal SuperUser
        {
            get
            {
                if (_superUser == null)
                {
                    IIdentity identity = new GenericIdentity(GetSuperUserName());
                    _superUser = new CustomPrincipal(identity, this, new ServerSideServerProxy());

                    // set the super user roles
                    ((CustomPrincipal)_superUser).Roles = this.GetSuperUserRole();
                }

                return _superUser;
            }
        }

		#region IUserManager Members

		/// <summary>
		/// Gets the information indicating whether the user manager is read-only.
		/// </summary>
		/// <returns>return false</returns>
		public virtual bool IsReadOnly 
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Authenticate an user with given name and password
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="password">The user password</param>
		/// <returns>true if user is authenticated, false otherwise</returns>
		public virtual bool Authenticate(string userName, string password)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;

				bool isAuthenticated;
				
				string query = AuthenticateQuery;

				query = query.Replace("param1", userName);

                Interpreter interpreter = new Interpreter();
                interpreter.ShowEncryptedData = true; // we don't want to have password encrypted in the returned result
                XmlDocument doc = interpreter.Query(query);

                // get the instance
                if (doc.DocumentElement.ChildNodes.Count == 1)
                {
                    XmlElement userElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                    if (userElement["Password"] != null)
                    {
                        string userPassword = userElement["Password"].InnerText;
                        if (!string.IsNullOrEmpty(userPassword) &&
                            userPassword.Trim() == password.Trim())
                        {
                            isAuthenticated = true;
                        }
                        else
                        {
                            isAuthenticated = false;
                        }
                    }
                    else
                    {
                        isAuthenticated = false;
                    }
                }
                else
                {
                    isAuthenticated = false;
                }

				return isAuthenticated;
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}
		}

        /// <summary>
        /// Gets roles that an user of the given name belongs to. When a role represents an unit,
        /// this method will get all units that are above this unit.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>A collection of roles</returns>
		public string[] GetRoles(string userName)
		{
			string[] roles = null;

            // first try to get roles from the cache
            roles = UserDataCache.Instance.GetUserRoles(userName);
            if (roles == null)
            {
			    IPrincipal originalPrincipal = Thread.CurrentPrincipal;
                QueryReader reader = null;

			    // This method can be called while executing a query to get user's roles.
			    // ignore the call if the query is in process. Secondly, make sure that
			    // UserInfo schema exists.
			    if (!_isQueryInProcess)
			    {
				    try 
				    {
					    // execute the method as a super user
					    Thread.CurrentPrincipal = _superUser;

					    string query = GetUserRolesQuery;

					    query = query.Replace("param1", userName);

					    _isQueryInProcess = true;

                        StringCollection roleCollection = new StringCollection();

					    Interpreter interpreter = new Interpreter();
                        // tell the interpreter that we want a reader that gets the query result in paging mode.
                        interpreter.IsPaging = true;
                        reader = interpreter.GetQueryReader(query);

                        XmlDocument doc = reader.GetNextPage();
                        
                        while (doc.DocumentElement.ChildNodes.Count > 0)
                        {
                            foreach (XmlElement element in doc.DocumentElement.ChildNodes)
                            {
                                roleCollection.Add(element.InnerText);
                            }

                            // get next page of records
                            doc = reader.GetNextPage();
                        }

                        // Add the parent roles to the collection
                        StringCollection completeRoleCollection = new StringCollection();
                        foreach (string role in roleCollection)
                        {
                            completeRoleCollection.Add(role);
                        }

                        StringCollection parentRoles;
                        foreach (string role in roleCollection)
                        {
                            parentRoles = new StringCollection();
                            GetParentRoles(role, parentRoles);
                            foreach (string parentRole in parentRoles)
                            {
                                if (!completeRoleCollection.Contains(parentRole))
                                {
                                    completeRoleCollection.Add(parentRole);
                                }
                            }
                        }

                        _isQueryInProcess = false;

                        // get role array
                        roles = new string[completeRoleCollection.Count];
                        int index = 0;
                        foreach (string role in completeRoleCollection)
                        {
                            roles[index++] = role;
                        }
				    }
				    finally
				    {
                        if (reader != null)
                        {
                            reader.Close();
                        }

					    // attach the original principal to the thread
					    Thread.CurrentPrincipal = originalPrincipal;
				    }
			    }

                // remember in cache for performance sake
                UserDataCache.Instance.SetUserRoles(userName, roles);
            }

			return roles;
		}

        /// <summary>
        /// Gets roles of a type that an user belongs to.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="type">The role type</param>
        /// <returns>An array of user's roles</returns>
        public string[] GetRoles(string userName, string type)
        {
            string[] units = null;
            // first try to get units from the cache
            units = UserDataCache.Instance.GetUserRoles(userName + type);
            if (units == null)
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;
                QueryReader reader = null;

                // This method can be called while executing a query to get user's units.
                // ignore the call if the query is in process. Secondly, make sure that
                // UserInfo schema exists.
                if (!_isQueryInProcess)
                {
                    try
                    {
                        // execute the method as a super user
                        Thread.CurrentPrincipal = _superUser;

                        string query = GetTypeRolesQuery;

                        query = query.Replace("param1", type);
                        query = query.Replace("param2", userName);

                        _isQueryInProcess = true;

                        StringCollection unitCollection = new StringCollection();

                        Interpreter interpreter = new Interpreter();
                        // tell the interpreter that we want a reader that gets the query result in paging mode.
                        interpreter.IsPaging = true;
                        reader = interpreter.GetQueryReader(query);

                        XmlDocument doc = reader.GetNextPage();

                        while (doc.DocumentElement.ChildNodes.Count > 0)
                        {
                            foreach (XmlElement element in doc.DocumentElement.ChildNodes)
                            {
                                unitCollection.Add(element.InnerText);
                            }

                            // get next page of records
                            doc = reader.GetNextPage();
                        }

                        // Add the parent units to the collection
                        StringCollection completeUnitCollection = new StringCollection();
                        foreach (string unit in unitCollection)
                        {
                            completeUnitCollection.Add(unit);
                        }

                        StringCollection parentUnits;
                        foreach (string unit in unitCollection)
                        {
                            parentUnits = new StringCollection();
                            GetParentRoles(unit, parentUnits);
                            foreach (string parentUnit in parentUnits)
                            {
                                if (!completeUnitCollection.Contains(parentUnit))
                                {
                                    completeUnitCollection.Add(parentUnit);
                                }
                            }
                        }

                        _isQueryInProcess = false;

                        // get role array
                        units = new string[completeUnitCollection.Count];
                        int index = 0;
                        foreach (string unit in completeUnitCollection)
                        {
                            units[index++] = unit;
                        }
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }

                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }

                // remember in cache for performance sake
                UserDataCache.Instance.SetUserRoles(userName + type, units);
            }

            return units;
        }

        /// <summary>
        /// Gets user data of a given user.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>An array of user's data</returns>
        /// <remarks>
        /// In the userData array, the first entry is the user's last name,
        /// the second entry is the user's first name,
        /// and the third entry is the user's email address
        /// </remarks>
        public virtual string[] GetUserData(string userName)
        {
            string[] userData;

            // first try to get them from the cache
            userData = UserDataCache.Instance.GetUserData(userName);
            if (userData == null)
            {
                if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                {
                    IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                    try
                    {
                        // execute the method as a super user
                        Thread.CurrentPrincipal = _superUser;

                        string query = GetUserDataQuery;

                        query = query.Replace("param1", userName);

                        _isQueryInProcess = true;

                        Interpreter interpreter = new Interpreter();

                        interpreter.ShowEncryptedData = true; // we don't want to have password encrypted in the returned result

                        XmlDocument doc = interpreter.Query(query);

                        _isQueryInProcess = false;

                        // get user data
                        if (doc.DocumentElement.ChildNodes.Count == 1)
                        {
                            userData = new string[4];

                            XmlElement userDataElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                            if (userDataElement["LastName"] != null)
                            {
                                userData[0] = userDataElement["LastName"].InnerText;
                            }

                            if (userDataElement["FirstName"] != null)
                            {
                                userData[1] = userDataElement["FirstName"].InnerText;
                            }

                            if (userDataElement["Email"] != null)
                            {
                                userData[2] = userDataElement["Email"].InnerText;
                            }

                            if (userDataElement["Password"] != null)
                            {
                                userData[3] = userDataElement["Password"].InnerText;
                            }
                        }
                    }
                    finally
                    {
                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }

                // remember in cache for performance sake
                if (userData != null)
                {
                    UserDataCache.Instance.SetUserData(userName, userData);
                }
            }

            return userData;
        }

        /// <summary>
        /// Gets user info of a given user name.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>An IUserInfo object</returns>
        public virtual UserRecord GetUserInfoByUserName(string userName)
        {
            UserRecord userInfo;

            // first try to get them from the cache
            userInfo = UserDataCache.Instance.GetUserInfoByName(userName);
            if (userInfo == null)
            {
                if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                {
                    IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                    try
                    {
                        // execute the method as a super user
                        Thread.CurrentPrincipal = _superUser;

                        string query = GetUserDataQuery;

                        query = query.Replace("param1", userName);

                        _isQueryInProcess = true;

                        Interpreter interpreter = new Interpreter();
                        interpreter.ShowEncryptedData = true; // we don't want to have password encrypted in the returned result

                        XmlDocument doc = interpreter.Query(query);

                        _isQueryInProcess = false;

                        // get user data
                        if (doc.DocumentElement.ChildNodes.Count == 1)
                        {
                            userInfo = new UserRecord();

                            userInfo.UserName = userName;

                            XmlElement userDataElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                            if (userDataElement["LastName"] != null)
                            {
                                userInfo.LastName = userDataElement["LastName"].InnerText;
                            }

                            if (userDataElement["FirstName"] != null)
                            {
                                userInfo.FirstName = userDataElement["FirstName"].InnerText;
                            }

                            if (userDataElement["Email"] != null)
                            {
                                userInfo.Email = userDataElement["Email"].InnerText;
                            }

                            if (userDataElement["PhoneNumber"] != null)
                            {
                                userInfo.PhoneNumber = userDataElement["PhoneNumber"].InnerText;
                            }

                            if (userDataElement["Key"] != null)
                            {
                                //userInfo.UserId = userDataElement["Key"].InnerText;
                                userInfo.UserId = userName; // userName is unique, can be used as ID
                            }

                            if (userDataElement["Password"] != null)
                            {
                                userInfo.Password = userDataElement["Password"].InnerText;
                            }

                            if (userDataElement["SecurityStamp"] != null)
                            {
                                userInfo.SecurityStamp = userDataElement["SecurityStamp"].InnerText;
                            }

                            if (userDataElement["Picture"] != null)
                            {
                                userInfo.Picture = userDataElement["Picture"].InnerText;
                            }

                            if (userDataElement["Department"] != null)
                            {
                                userInfo.Division = userDataElement["Department"].InnerText;
                            }

                            if (userDataElement["Location"] != null)
                            {
                                userInfo.Address = userDataElement["Location"].InnerText;
                            }
                        }
                    }
                    finally
                    {
                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }

                // remember in cache for performance sake
                if (userInfo != null)
                {
                    UserDataCache.Instance.SetUserInfoByName(userName, userInfo);
                }
            }

            return userInfo;
        }

        /// <summary>
        /// Gets user info of a given user key.
        /// </summary>
        /// <param name="userKey">The user's key</param>
        /// <returns>An IUserInfo object</returns>
        public virtual UserRecord GetUserInfoByUserKey(string userKey)
        {
            UserRecord userInfo;

            // first try to get them from the cache
            userInfo = UserDataCache.Instance.GetUserInfoByKey(userKey);
            if (userInfo == null)
            {
                if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                {
                    IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                    try
                    {
                        // execute the method as a super user
                        Thread.CurrentPrincipal = _superUser;

                        //string query = GetUserDataQueryByKey;
                        string query = GetUserDataQuery;

                        query = query.Replace("param1", userKey);

                        _isQueryInProcess = true;

                        Interpreter interpreter = new Interpreter();

                        interpreter.ShowEncryptedData = true; // we don't want to have password encrypted in the returned result

                        XmlDocument doc = interpreter.Query(query);

                        _isQueryInProcess = false;

                        // get user data
                        if (doc.DocumentElement.ChildNodes.Count == 1)
                        {
                            userInfo = new UserRecord();

                            userInfo.UserId = userKey;

                            XmlElement userDataElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                            if (userDataElement["LastName"] != null)
                            {
                                userInfo.LastName = userDataElement["LastName"].InnerText;
                            }

                            if (userDataElement["FirstName"] != null)
                            {
                                userInfo.FirstName = userDataElement["FirstName"].InnerText;
                            }

                            if (userDataElement["Email"] != null)
                            {
                                userInfo.Email = userDataElement["Email"].InnerText;
                            }

                            if (userDataElement["PhoneNumber"] != null)
                            {
                                userInfo.PhoneNumber = userDataElement["PhoneNumber"].InnerText;
                            }

                            if (userDataElement["ID"] != null)
                            {
                                userInfo.UserName = userDataElement["ID"].InnerText;
                            }

                            if (userDataElement["Password"] != null)
                            {
                                userInfo.Password = userDataElement["Password"].InnerText;
                            }

                            if (userDataElement["SecurityStamp"] != null)
                            {
                                userInfo.SecurityStamp = userDataElement["SecurityStamp"].InnerText;
                            }

                            if (userDataElement["Picture"] != null)
                            {
                                userInfo.Picture = userDataElement["Picture"].InnerText;
                            }

                            if (userDataElement["Department"] != null)
                            {
                                userInfo.Division = userDataElement["Department"].InnerText;
                            }

                            if (userDataElement["Location"] != null)
                            {
                                userInfo.Address = userDataElement["Location"].InnerText;
                            }
                        }
                    }
                    finally
                    {
                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }

                // remember in cache for performance sake
                if (userInfo != null)
                {
                    UserDataCache.Instance.SetUserInfoByKey(userKey, userInfo);
                }
            }

            return userInfo;
        }

        /// <summary>
        /// Gets a specific data of a given user.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="parameterName">The name of a parameter in user data, such as email, telphone, or address</param>
        /// <returns>A data value , null if non-exist</returns>
        public string GetUserData(string userName, string parameterName)
        {
            string userDataValue = null;

            if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    string query = GetUserDataQuery;

                    query = query.Replace("param1", userName);

                    _isQueryInProcess = true;

                    Interpreter interpreter = new Interpreter();
                    XmlDocument doc = interpreter.Query(query);

                    _isQueryInProcess = false;

                    // get user data
                    if (doc.DocumentElement.ChildNodes.Count == 1)
                    {
                        XmlElement userDataElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                        if (userDataElement[parameterName] != null)
                        {
                            userDataValue = userDataElement[parameterName].InnerText;
                        }
                    }
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }

            return userDataValue;
        }

        /// <summary>
        /// Gets role data of a given role.
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns>An array of role's data</returns>
        /// <remarks>
        /// In the roleData array, the first entry is the role's display text,
        /// the second entry is the role's type
        /// </remarks>
        public string[] GetRoleData(string roleName)
        {
            // first try to get them from the cache
            string[] roleData = UserDataCache.Instance.GetRoleData(roleName);
            if (roleData == null)
            {
                roleData = new string[2];

                if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                {
                    IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                    try
                    {
                        // execute the method as a super user
                        Thread.CurrentPrincipal = _superUser;

                        string query = GetRoleDataQuery;

                        query = query.Replace("param1", roleName);

                        _isQueryInProcess = true;

                        Interpreter interpreter = new Interpreter();
                        XmlDocument doc = interpreter.Query(query);

                        _isQueryInProcess = false;

                        // get user data
                        if (doc.DocumentElement.ChildNodes.Count == 1)
                        {
                            XmlElement roleDataElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                            if (roleDataElement["Text"] != null)
                            {
                                roleData[0] = roleDataElement["Text"].InnerText;
                            }

                            if (roleDataElement["Type"] != null)
                            {
                                roleData[1] = roleDataElement["Type"].InnerText;
                            }
                        }
                    }
                    finally
                    {
                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }

                // remember in cache for performance sake
                UserDataCache.Instance.SetRoleData(roleName, roleData);
            }

            return roleData;
        }
        
		/// <summary>
		/// Gets all roles defined for the schema
		/// </summary>
		/// <returns>An array of all roles</returns>
		public string[] GetAllRoles()
		{
			string[] roles = null;
            string[] roleData;
            string role;

            if (UserDataCache.Instance.AllRoles != null)
            {
                roles = UserDataCache.Instance.AllRoles;
            }
            else
            {
                string query = GetPagedQuery(GetAllRolesQuery, 0); // replace the variables in the query

                int roleCount = GetQueryCount(query);

                if (roleCount > 0)
                {
                    roles = new string[roleCount];
                    IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                    try
                    {
                        // execute the method as a super user
                        Thread.CurrentPrincipal = _superUser;

                        int count = 0;
                        int pageIndex = 0;
                        while (count < roleCount)
                        {
                            // get pages query
                            query = GetPagedQuery(GetAllRolesQuery, pageIndex);

                            _isQueryInProcess = true;

                            Interpreter interpreter = new Interpreter();
                            XmlDocument doc = interpreter.Query(query);

                            _isQueryInProcess = false;

                            // get roles
                            foreach (XmlElement element in doc.DocumentElement.ChildNodes)
                            {
                                role = element["Name"].InnerText;
                                roles[count] = role;
                                count++;

                                if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                                {
                                    roleData = new string[2];

                                    if (element["Text"] != null)
                                    {
                                        roleData[0] = element["Text"].InnerText;
                                    }

                                    if (element["Type"] != null)
                                    {
                                        roleData[1] = element["Type"].InnerText;
                                    }

                                    // remember in cache for performance sake
                                    UserDataCache.Instance.SetRoleData(role, roleData);
                                }
                            }

                            pageIndex++;
                        }
                    }
                    finally
                    {
                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }

                UserDataCache.Instance.AllRoles = roles;
            }

			return roles;
		}

		/// <summary>
		/// Gets users that belongs to the given role.
		/// </summary>
		/// <param name="role">The role</param>
		/// <returns>An array of role's users.</returns>
		public string[] GetUsers(string role)
		{
            string[] users = UserDataCache.Instance.GetRoleUsers(role);
            if (users == null)
            {
                StringCollection userCollection = new StringCollection();
                string[] allUsers;

                if (UserDataCache.Instance.AllUsers != null)
                {
                    allUsers = UserDataCache.Instance.AllUsers;
                }
                else
                {
                    allUsers = GetAllUsers();
                    UserDataCache.Instance.AllUsers = allUsers;
                }

                foreach (string userName in allUsers)
                {
                    if (IsUserHasRole(userName, role))
                    {
                        userCollection.Add(userName);
                    }
                }

                // get user array
                users = new string[userCollection.Count];
                int index = 0;
                foreach (string usr in userCollection)
                {
                    users[index++] = usr;
                }

                UserDataCache.Instance.SetRoleUsers(role, users);
            }

			return users;
		}

		/// <summary>
		/// Gets all users defined for the schema
		/// </summary>
		/// <returns>An array of all users</returns>
		public string[] GetAllUsers()
		{
			string[] users = null;
            string[] userData;
            string user;

            lock (this)
            {
                if (UserDataCache.Instance.AllUsers != null)
                {
                    users = UserDataCache.Instance.AllUsers;
                }
                else
                {
                    string query = GetPagedQuery(GetAllUsersQuery, 0); // replace the variables in the query
                    int userCount = GetQueryCount(query);

                    if (userCount > 0)
                    {
                        users = new string[userCount];

                        IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                        try
                        {
                            // execute the method as a super user
                            Thread.CurrentPrincipal = _superUser;

                            int count = 0;
                            int pageIndex = 0;
                            while (count < userCount)
                            {
                                // get pages query
                                query = GetPagedQuery(GetAllUsersQuery, pageIndex);

                                _isQueryInProcess = true;

                                Interpreter interpreter = new Interpreter();
                                interpreter.ShowEncryptedData = true;
                                XmlDocument doc = interpreter.Query(query);

                                _isQueryInProcess = false;

                                if (doc.DocumentElement.ChildNodes.Count == 0)
                                {
                                    break;
                                }

                                // get users
                                foreach (XmlElement element in doc.DocumentElement.ChildNodes)
                                {
                                    user = element["ID"].InnerText;
                                    users[count] = user;
                                    count++;

                                    if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                                    {
                                        userData = new string[4];

                                        // get user data
                                        if (element["LastName"] != null)
                                        {
                                            userData[0] = element["LastName"].InnerText;
                                        }

                                        if (element["FirstName"] != null)
                                        {
                                            userData[1] = element["FirstName"].InnerText;
                                        }

                                        if (element["Email"] != null)
                                        {
                                            userData[2] = element["Email"].InnerText;
                                        }

                                        if (element["Password"] != null)
                                        {
                                            userData[3] = element["Password"].InnerText;
                                        }

                                        // remember in cache for performance sake
                                        UserDataCache.Instance.SetUserData(user, userData);
                                    }
                                }

                                pageIndex++;
                            }
                        }
                        finally
                        {
                            // attach the original principal to the thread
                            Thread.CurrentPrincipal = originalPrincipal;
                        }
                    }

                    UserDataCache.Instance.AllUsers = users;
                }
            }

			return users;
		}

		/// <summary>
		/// Add an user
		/// </summary>
		/// <param name="userName">The user's unique id</param>
		/// <param name="password">The user's password</param>
        /// <param name="userData">The user's data</param>
        /// <remarks>
        /// In the userData array, the first entry is the user's last name,
        /// the second entry is the user's first name,
        /// and the third entry is the user's email address
        /// </remarks>
        public void AddUser(string userName, string password, string[] userData)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

			try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;
				
				string query;

                if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
                {
                    ValidatePassword(password); // exception will be thrown if the password is invalid

                    // UserInfo schema has added FirstName, LastName, and Email
                    query = AddUserQuery;
                    query = query.Replace("param1", userName);
                    query = query.Replace("param2", password);
                    if (userData.Length > 0 && !string.IsNullOrEmpty(userData[0]))
                    {
                        query = query.Replace("param3", userData[0]); // Last Name
                    }
                    else
                    {
                        query = query.Replace("param3", "");
                    }

                    if (userData.Length > 1 && !string.IsNullOrEmpty(userData[1]))
                    {
                        query = query.Replace("param4", userData[1]); // Fisrt Name
                    }
                    else
                    {
                        query = query.Replace("param4", "");
                    }

                    if (userData.Length > 2 && !string.IsNullOrEmpty(userData[2]))
                    {
                        query = query.Replace("param5", userData[2]); // Email
                    }
                    else
                    {
                        query = query.Replace("param5", "");
                    }
                }
                else
                {
                    query = AddUserQuery;
                    query = query.Replace("param1", userName);
                    query = query.Replace("param2", password);
                }

				Interpreter interpreter = new Interpreter();
				XmlDocument doc = interpreter.Query(query);

                // clear the enum types created for the user list
                EnumTypeFactory.Instance.ClearEnumTypes();
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}

            // clear the cache, so that user's data will reloaded
            UserDataCache.Instance.ClearUserDataCaches();
		}

        /// <summary>
        /// Add an user
        /// </summary>
        /// <param name="userData">The user's info</param>
        public void AddUser(UserRecord userInfo)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query;

                // UserInfo schema has added FirstName, LastName, and Email
                query = AddUserInfoQuery;
                query = query.Replace("param1", userInfo.UserName);
                query = query.Replace("param2", userInfo.Password);

                if (!string.IsNullOrEmpty(userInfo.LastName))
                {
                    query = query.Replace("param3", userInfo.LastName); // LastName
                }
                else
                {
                    query = query.Replace("param3", "");
                }

                if (!string.IsNullOrEmpty(userInfo.FirstName))
                {
                    query = query.Replace("param4", userInfo.FirstName); // Fisrt Name
                }
                else
                {
                    query = query.Replace("param4", "");
                }

                if (!string.IsNullOrEmpty(userInfo.Email))
                {
                    query = query.Replace("param5", userInfo.Email); // Email
                }
                else
                {
                    query = query.Replace("param5", "");
                }

                if (!string.IsNullOrEmpty(userInfo.PhoneNumber))
                {
                    query = query.Replace("param6", userInfo.PhoneNumber); // PhoneNumber
                }
                else
                {
                    query = query.Replace("param6", "");
                }

                if (!string.IsNullOrEmpty(userInfo.UserId))
                {
                    query = query.Replace("param7", userInfo.UserId); // Key
                }
                else
                {
                    query = query.Replace("param7", "");
                }

                if (!string.IsNullOrEmpty(userInfo.SecurityStamp))
                {
                    query = query.Replace("param8", userInfo.SecurityStamp); // SecurityStamp
                }
                else
                {
                    query = query.Replace("param8", "");
                }

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Query(query);

                // clear the enum types created for the user list
                EnumTypeFactory.Instance.ClearEnumTypes();
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }

            // clear the cache, so that user's data will reloaded
            UserDataCache.Instance.ClearUserDataCaches();
        }

        /// <summary>
        /// Change an user's password.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="oldPassword">The old password</param>
        /// <param name="newPassword">The new password</param>
        public void ChangeUserPassword(string userName, string oldPassword,
			string newPassword)
		{
            // if oldPassword is NONE, skip to authenticate the old password
            if (oldPassword.ToUpper() == "NONE" || this.Authenticate(userName, oldPassword))
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    ValidatePassword(newPassword); // an exception is thrown if the password is invalid

                    string query = ChangeUserPwdQuery;

                    query = query.Replace("param1", userName);
                    query = query.Replace("param2", newPassword);

                    Interpreter interpreter = new Interpreter();
                    XmlDocument doc = interpreter.Query(query);
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
            else
            {
                throw new Exception("Failed to authenticate the user with the given userName and password.");
            }
		}

        /// <summary>
        /// Change an user's data.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="userData">The user's data</param>
        /// <remarks>
        /// In the userData array, the first entry is the user's last name,
        /// the second entry is the user's first name,
        /// and the third entry is the user's email address
        /// </remarks>
        public void ChangeUserData(string userName, string[] userData)
        {
            if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    string query = ChangeUserDataQuery;

                    query = query.Replace("param1", userName);

                    if (userData.Length > 0 && !string.IsNullOrEmpty(userData[0]))
                    {
                        query = query.Replace("param2", userData[0]); // Last Name
                    }
                    else
                    {
                        query = query.Replace("param2", "");
                    }

                    if (userData.Length > 1 && !string.IsNullOrEmpty(userData[1]))
                    {
                        query = query.Replace("param3", userData[1]); // Fisrt Name
                    }
                    else
                    {
                        query = query.Replace("param3", "");
                    }

                    if (userData.Length > 2 && !string.IsNullOrEmpty(userData[2]))
                    {
                        query = query.Replace("param4", userData[2]); // Email
                    }
                    else
                    {
                        query = query.Replace("param4", "");
                    }

                    if (userData.Length > 3 && !string.IsNullOrEmpty(userData[3]))
                    {
                        query = query.Replace("param5", userData[3]); // PhoneNumber
                    }
                    else
                    {
                        query = query.Replace("param5", "");
                    }

                    if (userData.Length > 4 && !string.IsNullOrEmpty(userData[4]))
                    {
                        query = query.Replace("param6", userData[4]); // Picture
                    }
                    else
                    {
                        query = query.Replace("param6", "");
                    }

                    Interpreter interpreter = new Interpreter();
                    XmlDocument doc = interpreter.Query(query);

                    // clear the enum types created for the user list
                    EnumTypeFactory.Instance.ClearEnumTypes();
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }

            // clear the cache, so that user's data will reloaded
            UserDataCache.Instance.ClearUserDataCaches();
        }

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="userInfo">The user's info</param>
        public void ChangeUserInfoByName(string userName, UserRecord userInfo)
        {
            if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    UserRecord oldUserInfo = GetUserInfoByUserName(userName);

                    string query = ChangeUserDataQuery;

                    query = query.Replace("param1", userName);

                    if (!string.IsNullOrEmpty(userInfo.LastName))
                    {
                        query = query.Replace("param2", userInfo.LastName); // Last Name
                    }
                    else
                    {
                        // use the old last name
                        query = query.Replace("param2", oldUserInfo.LastName);
                    }

                    if (!string.IsNullOrEmpty(userInfo.FirstName))
                    {
                        query = query.Replace("param3", userInfo.FirstName); // Fisrt Name
                    }
                    else
                    {
                        query = query.Replace("param3", oldUserInfo.FirstName);
                    }

                    if (!string.IsNullOrEmpty(userInfo.Email))
                    {
                        query = query.Replace("param4", userInfo.Email); // Email
                    }
                    else
                    {
                        query = query.Replace("param4", oldUserInfo.Email);
                    }

                    if (!string.IsNullOrEmpty(userInfo.PhoneNumber))
                    {
                        query = query.Replace("param5", userInfo.PhoneNumber); // PhoneNumber
                    }
                    else
                    {
                        query = query.Replace("param5", oldUserInfo.PhoneNumber);
                    }

                    if (!string.IsNullOrEmpty(userInfo.Picture))
                    {
                        query = query.Replace("param6", userInfo.Picture); // Picture
                    }
                    else
                    {
                        query = query.Replace("param6", oldUserInfo.Picture);
                    }

                    Interpreter interpreter = new Interpreter();
                    XmlDocument doc = interpreter.Query(query);

                    // clear the enum types created for the user list
                    EnumTypeFactory.Instance.ClearEnumTypes();
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userKey">The user key</param>
        /// <param name="userInfo">The user's info</param>
        public void ChangeUserInfoByKey(string userKey, UserRecord userInfo)
        {
            throw new NotImplementedException("ChangeUserInfoByKey not implemented");
        }

        /// <summary>
        /// Change an role's data.
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="roleData">The role's data</param>
        /// <remarks>
        /// In the roleData array, the first entry is the role's display text,
        /// </remarks>
        public void ChangeRoleData(string roleName, string[] roleData)
        {
            if (NewteraNameSpace.IsVersionGreaterThan(NEW_USER_INFO_VERSION))
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    string query = ChangeRoleDataQuery;

                    query = query.Replace("param1", roleName);

                    if (roleData.Length > 0 && !string.IsNullOrEmpty(roleData[0]))
                    {
                        query = query.Replace("param2", roleData[0]); // display name
                    }
                    else
                    {
                        query = query.Replace("param2", "");
                    }

                    Interpreter interpreter = new Interpreter();
                    XmlDocument doc = interpreter.Query(query);

                    // clear the enum types created for the role list
                    EnumTypeFactory.Instance.ClearEnumTypes();
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }

            // clear the cache, so that user's data will reloaded
            UserDataCache.Instance.ClearRoleDataCaches();
        }

		/// <summary>
		/// Delete an user
		/// </summary>
		/// <param name="userName">The user's unique id</param>
		public void DeleteUser(string userName)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

			try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;
				
				// delete user/role mappings first
				string query = DeleteUserRoleMapByUserQuery;
				query = query.Replace("param1", userName);
				Interpreter interpreter = new Interpreter();
				XmlDocument doc = interpreter.Query(query);

				// delete user entry
				query = DeleteUserQuery;
				query = query.Replace("param1", userName);
				interpreter = new Interpreter();
				doc = interpreter.Query(query);

                // clear the enum types created for the user list
                EnumTypeFactory.Instance.ClearEnumTypes();
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}

            // clear the cache, so that user's data will reloaded
            UserDataCache.Instance.ClearUserDataCaches();
		}

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="roleName">The unique role name</param>
        /// <param name="roleData">Role's data</param>
        public void AddRole(string roleName, string[] roleData)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

			try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;
				
				string query = AddRoleQuery;

				query = query.Replace("param1", roleName);

                // set role text
                if (roleData != null &&
                    roleData.Length > 0 &&
                    !string.IsNullOrEmpty(roleData[0]))
                {
                    query = query.Replace("param2", roleData[0]);
                }
                else
                {
                    query = query.Replace("param2", "");
                }

                // set role type
                if (roleData != null &&
                    roleData.Length > 1 &&
                    !string.IsNullOrEmpty(roleData[1]))
                {
                    query = query.Replace("param3", roleData[1]);
                }
                else
                {
                    query = query.Replace("param3", "");
                }

				Interpreter interpreter = new Interpreter();
				XmlDocument doc = interpreter.Query(query);

                // clear the enum types created for the user list
                EnumTypeFactory.Instance.ClearEnumTypes();
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}

            // clear the cache, so that role's data will reloaded
            UserDataCache.Instance.ClearRoleDataCaches();
		}

		/// <summary>
		/// Delete a role
		/// </summary>
		/// <param name="roleName">The unique role name</param>
		public void DeleteRole(string roleName)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

			try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;
				
				// delete user/role mappings first
				string query = DeleteUserRoleMapByRoleQuery;
				query = query.Replace("param1", roleName);
				Interpreter interpreter = new Interpreter();
				XmlDocument doc = interpreter.Query(query);

				// delete role entry
				query = DeleteRoleQuery;
				query = query.Replace("param1", roleName);
				interpreter = new Interpreter();
				doc = interpreter.Query(query);

                // clear the enum types created for the user list
                EnumTypeFactory.Instance.ClearEnumTypes();
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}

            // clear the cache, so that role's data will reloaded
            UserDataCache.Instance.ClearRoleDataCaches();
		}

		/// <summary>
		/// Add a mapping for user and role
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void AddUserRoleMapping(string userName, string roleName)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

			try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;
				
				string query = AddUserRoleMapQuery;

				query = query.Replace("param1", userName);
				query = query.Replace("param2", roleName);

				Interpreter interpreter = new Interpreter();
				XmlDocument doc = interpreter.Query(query);
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}


            // clear the cache, so that user's roles will be reloaded
            UserDataCache.Instance.ClearUserAndRoleCaches();
		}

		/// <summary>
		/// Delete a mapping of user and role.
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void DeleteUserRoleMapping(string userName, string roleName)
		{
			IPrincipal originalPrincipal = Thread.CurrentPrincipal;

			try
			{
				// execute the method as a super user
				Thread.CurrentPrincipal = _superUser;
				
				string query = DeleteUserRoleMapQuery;

				query = query.Replace("param1", userName);
				query = query.Replace("param2", roleName);

				Interpreter interpreter = new Interpreter();
				XmlDocument doc = interpreter.Query(query);
			}
			finally
			{
				// attach the original principal to the thread
				Thread.CurrentPrincipal = originalPrincipal;
			}

            // clear the cache, so that user's roles will be reloaded
            UserDataCache.Instance.ClearUserAndRoleCaches();
		}

        /// <summary>
        /// Gets user's emails
        /// </summary>
        /// <param name="userName">The user name</param>
        public string[] GetUserEmails(string userName)
        {
            string[] emails = null;
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetUserEmailQuery;
                query = query.Replace("param1", userName);

                _isQueryInProcess = true;

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Query(query);

                _isQueryInProcess = false;

                // get roles
                emails = new string[doc.DocumentElement.ChildNodes.Count];
                int index = 0;
                foreach (XmlElement element in doc.DocumentElement.ChildNodes)
                {
                    emails[index] = element.InnerText;
                    index++;
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }

            return emails;
        }

		#endregion

		/// <summary>
		/// Authenticate an user as the super user.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="password">User password</param>
		/// <returns>true if the user has been authenticated as super user, false otherwise.</returns>
		public bool AuthenticateSuperUser(string userName, string password)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("AuthenticateSuperUser");
			bool isSuperUser = true;

			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					if (reader.GetString(0) != userName)
					{
						isSuperUser = false;
					}
					else
					{
						string pwd = reader.GetString(1);

						try
						{
							string decrypted = TextDeEncryptor.Instance.Decrypt(pwd);
							pwd = decrypted;
						}
						catch (Exception)
						{
							// in the versions before 2.7.1,
							// the password is not encrypted, therefore,
							// we need to take care of backward compatibility
						}

						if (pwd != password)
						{
							isSuperUser = false;
						}
					}
				}

				return isSuperUser;
			}
			finally
			{
				if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Change the super user's password.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="oldPassword">Old password</param>
		/// <param name="newPassword">New password</param>
		public void ChangeSuperUserPassword(string userName, string oldPassword, string newPassword)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("ChangeSuperUserPassword");

			try
			{
				// encrypt the password before storing it to database
				string encrypted = TextDeEncryptor.Instance.Encrypt(newPassword);
				sql = sql.Replace(GetParamName("name", dataProvider), "'" + userName + "'");
				sql = sql.Replace(GetParamName("newPassword", dataProvider), "'" + encrypted + "'");

				cmd.CommandText = sql;

				cmd.ExecuteNonQuery();

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw e;
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Gets the super user's name
		/// </summary>
		/// <returns>A string representing super user.</returns>
		public string GetSuperUserName()
		{
			string userName = "";
            IDataReader reader = null;
            IDbConnection con = null;

            try
			{
                IDataProvider dataProvider = DataProviderFactory.Instance.Create();
                con = dataProvider.Connection;
                IDbCommand cmd = con.CreateCommand();
                string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetSuperUserName");

                cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					userName = reader.GetString(0);	
				}
			}
            catch (Exception)
            {
                con = null;
            }
			finally
			{
                if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

                if (con != null)
                {
                    con.Close();
                }
			}

            return userName;
		}

		/// <summary>
		/// Gets roles of a super user if the user is the super user
		/// </summary>
		/// <param name="userName">user name</param>
		/// <returns>A string array of roles, null if the user isn't the superuser.</returns>
		public string[] GetSuperUserRoles(string userName)
		{
			string[] roles = null;

			if (IsSuperUser(userName))
			{
				roles = this.GetSuperUserRole();
			}

			return roles;
		}

		/// <summary>
		/// Gets the information indicating whether an user of given name is the
		/// super user.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <returns>true if it is the super user, false otherwise.</returns>
		private bool IsSuperUser(string userName)
		{
			IDataProvider dataProvider = DataProviderFactory.Instance.Create();
			IDbConnection con = dataProvider.Connection;
			IDbCommand cmd = con.CreateCommand();
			IDataReader reader = null;
			string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetSuperUser");
			bool isSuperUser = false;

			try
			{
				cmd.CommandText = sql;

				reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					if (reader.GetString(0) == userName)
					{
						isSuperUser = true;
					}
				}

				return isSuperUser;
			}
			finally
			{
                if (reader != null && !reader.IsClosed)
				{
					reader.Close();
				}

				con.Close();
			}
		}

		/// <summary>
		/// Get the role information for the super user
		/// </summary>
		/// <returns>A role string array</returns>
		private string[] GetSuperUserRole()
		{
			string[] roles = new string[1];

			roles[0] = NewteraNameSpace.CM_SUPER_USER_ROLE;

			return roles;
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

        /// <summary>
        /// Gets the information indicating whether an user has a given role
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="role"></param>
        /// <returns>true if the user has the given role, false otherwise.</returns>
        private bool IsUserHasRole(string userName, string role)
        {
            bool status = false;

            string[] userRoles = GetRoles(userName);
            if (userRoles != null)
            {
                foreach (string userRole in userRoles)
                {
                    if (userRole == role)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }

		/// <summary>
		/// Parse the connection string to get the SchemaInfo object
		/// </summary>
		/// <param name="connectionString">The connection string</param>
		/// <returns>A SchemaInfo object</returns>
		private SchemaInfo ParseConnectionString(string connectionString)
		{
			SchemaInfo schemaInfo = new SchemaInfo();

			// The schema info string is in form of "SCHEM_NAME=schema name;SCHEMA_VERSION=schema version"
			// Compile regular expression to find "name = value" pairs
			Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

			MatchCollection matches = regex.Matches(connectionString);
			foreach (Match match in matches)
			{
				int pos = match.Value.IndexOf("=");
				string key = match.Value.Substring(0, pos).TrimEnd();
				string val = match.Value.Substring(pos + 1).TrimStart();
				if (key.Trim().ToUpper() == SCHEMA_NAME)
				{
					schemaInfo.Name = val;
				}
				else if (key.Trim().ToUpper() == SCHEMA_VERSION)
				{
					schemaInfo.Version = val;
				}
			}

			// validate the schema info
			if (schemaInfo.Name == null)
			{
				throw new ConfigurationErrorsException("Missing " + SCHEMA_NAME + " parameter in  " + CONNECTION_STRING);
			}

			if (schemaInfo.Version == null)
			{
				schemaInfo.Version = "1.0";
			}

			return schemaInfo;
		}

        /// <summary>
        /// Gets count of a query
        /// </summary>
        /// <returns>The count</returns>
        private int GetQueryCount(string query)
        {
            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                _isQueryInProcess = true;

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Count(query);

                _isQueryInProcess = false;

                string countStr = doc.DocumentElement.InnerText;

                return System.Convert.ToInt32(countStr);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// Replace the start and end parameters in a query to get a paged query
        /// </summary>
        /// <param name="originalQuery">The original query</param>
        /// <param name="pageIndex">The page index</param>
        /// <returns>The paged query</returns>
        private string GetPagedQuery(string originalQuery, int pageIndex)
        {
            int start = PAGE_SIZE * pageIndex + 1;
            int end = PAGE_SIZE * (pageIndex + 1);

            string query = originalQuery.Replace("start", start.ToString());
            query = query.Replace("end", end.ToString());

            return query;
        }
        
        /// <summary>
        /// Gets the parent roles of the given role
        /// </summary>
        /// <param name="role">The current role</param>
        /// <param name="parentRoles">A String collection of roles</param>
        /// <returns>A string collection of the parent roles</returns>
        private void GetParentRoles(string role, StringCollection parentRoles)
        {
            string parentRole;
            string query = GetParentRoleQuery;

            query = query.Replace("param1", role);

            Interpreter interpreter = new Interpreter();

            XmlDocument doc = interpreter.Query(query);

            // get parent role
            if (doc.DocumentElement.ChildNodes.Count == 1)
            {
                XmlElement roleElement = (XmlElement)doc.DocumentElement.ChildNodes[0];
                if (roleElement["Name"] != null)
                {
                    parentRole = roleElement["Name"].InnerText;

                    parentRoles.Add(parentRole);

                    // get parent role of the parent role
                    GetParentRoles(parentRole, parentRoles);
                }
            }
        }

        /// <summary>
        ///  validate the password against the regular expression defined in a constraint if exists.
        /// </summary>
        /// <param name="password"></param>
        /// <exception cref="Exception">Thrown if the password is invalid</exception>
        private void ValidatePassword(string password)
        {
            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            SchemaInfo[] schemaInfos = MetaDataCache.Instance.GetSchemaInfos(dataProvider);
            SchemaInfo theSchemaInfo = null;
            foreach (SchemaInfo schemaInfo in schemaInfos)
            {
                if (schemaInfo.NameAndVersion == USER_INFO_SCHEMA_ID)
                {
                    theSchemaInfo = schemaInfo;
                    break;
                }
            }

            if (theSchemaInfo == null)
            {
                throw new Exception("The schema " + USER_INFO_SCHEMA_ID + " doesn't exist in the database anymore.");
            }

            MetaDataModel metaData = MetaDataCache.Instance.GetMetaData(theSchemaInfo, dataProvider);
            ClassElement classElement = metaData.SchemaModel.FindClass(USER_CLASS_NAME);
            if (classElement == null)
            {
                throw new Exception("The class " + USER_CLASS_NAME + " doesn't exist in the schema " + USER_INFO_SCHEMA_ID);
            }

            SimpleAttributeElement simpleAttribute = classElement.FindSimpleAttribute(PASSWORD_ATTRIBUTE_NAME);
            if (simpleAttribute == null)
            {
                throw new Exception("The attribute " + PASSWORD_ATTRIBUTE_NAME + " doesn't exist in the class " + USER_CLASS_NAME);
            }

            if (simpleAttribute.Constraint != null && simpleAttribute.Constraint is PatternElement)
            {
                // validate the password against the pattern
                PatternElement patternElement = (PatternElement)simpleAttribute.Constraint;
                if (!patternElement.IsValueValid(password))
                {
                    string msg = "Not valid password";
                    if (!string.IsNullOrEmpty(patternElement.ErrorMessage))
                    {
                        msg = patternElement.ErrorMessage;
                    }
                    throw new Exception(msg);
                }
            }
        }
	}
}