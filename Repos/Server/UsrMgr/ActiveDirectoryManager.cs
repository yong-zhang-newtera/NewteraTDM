/*
* @(#)ActiveDirectoryManager.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.UsrMgr
{
	using System;
	using System.Data;
	using System.Text;
	using System.DirectoryServices;
	
	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Principal;

	/// <summary> 
	/// IUserManager implementation for Microsoft Active Directory 
	/// </summary>
	/// <version> 1.0.0 28 Aug 2006 </version>
	public class ActiveDirectoryManager : IUserManager
	{
		private string _connectionString;
		private string _userName;
		private string _password;
		private string _domain;

		/// <summary>
		/// Initiate an instance of ActiveDirectoryManager class
		/// </summary>
		public ActiveDirectoryManager()
		{
			_connectionString = null;
			_userName = null;
			_password = null;
			_domain = null;
		}

		/// <summary>
		/// Gets or sets the connection string to the Active Directory
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value.Trim();
			}
		}

		/// <summary>
		/// Gets or sets the user name for connecting to the Active Directory
		/// </summary>
		/// <remarks>The user name is in form of domainname\user. It needs to be a member of
		/// Domain Administrators in order to be able to modify the active directory.</remarks>
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
		/// Gets or sets the user password for connecting to the Active Directory
		/// </summary>
		/// <remarks>The password needs to be a member of
		/// Domain Administrators in order to be able to modify the active directory.</remarks>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}

		/// <summary>
		/// Gets the domain of the Active Directory
		/// </summary>
		public string Domain
		{
			get
			{
				if (_domain == null)
				{
					_domain = "";
					if (this.UserName != null)
					{
						// UserName is in form of domain\userlogin
						int pos = this.UserName.IndexOf("\\");
						if (pos > 0)
						{
							_domain = this.UserName.Substring(0, pos);
						}
					}
				}

				return _domain;
			}
		}

		#region IUserManager Members

		/// <summary>
		/// Gets the information indicating whether the user manager is read-only.
		/// </summary>
		/// <returns>true</returns>
		public bool IsReadOnly 
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Authenticate an user with given name and password
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="password">The user password</param>
		/// <returns>true if user is authenticated, false otherwise</returns>
		public bool Authenticate(string userName, string password)
		{
			bool isAuthenticated = true;

			DirectoryEntry de = this.GetDirectoryEntry(null, this.Domain + "\\" + userName, password);

			try 
			{
				DirectorySearcher ds = new DirectorySearcher(de);
				ds.Filter = "SAMAccountName=" + userName;
				ds.PropertiesToLoad.Add("cn");
				SearchResult sr = ds.FindOne();
				if(sr == null)
				{
					throw new Exception();
				}

				isAuthenticated = true;
			} 
			catch (Exception)
			{
				isAuthenticated = false;
			}

			return isAuthenticated;
		}

		/// <summary>
		/// Gets roles that an user of the given name is in, including sub-roles.
		/// </summary>
		/// <param name="userName">The user's name</param>
		/// <returns>A collection of roles</returns>
		public string[] GetRoles(string userName)
		{
			string[] roles = null;

			DirectoryEntry de = GetDirectoryEntry();
			DirectorySearcher ds = new DirectorySearcher(de);

			ds.Filter = "SAMAccountName=" + userName;
			ds.PropertiesToLoad.Add("memberOf");
			ds.SearchScope = SearchScope.Subtree;

			SearchResult result = ds.FindOne();

			if (result != null)
			{
				int count = result.Properties["memberOf"].Count;
				roles = new string[count];
				string dn;
				int equalsIndex, commaIndex;
				for (int i = 0; i < count; i++)
				{
					dn = result.Properties["memberOf"][i].ToString();
					equalsIndex = dn.IndexOf("=", 1);
					commaIndex = dn.IndexOf(",", 1);
					if (equalsIndex > 0)
					{
						roles[i] = dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1);
					}
					else
					{
						roles[i] = "";
					}
				}
			}
			else
			{
				roles = new string[0];
			}

			return roles;
		}

        /// <summary>
        /// Gets roles of a type that an user belongs to.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="type">Role type</param>
        /// <returns>An array of user's roles</returns>
        public string[] GetRoles(string userName, string type)
        {
            return new string[0]; // not ye implemented
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
        public string[] GetUserData(string userName)
        {
            return new string[0]; // to be implemented
        }

        /// <summary>
        /// Gets user info of a given user name.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>An IUserInfo object</returns>
        public UserRecord GetUserInfoByUserName(string userName)
        {
            throw new NotImplementedException("GetUserInfoByUserName not implemented");
        }

        /// <summary>
        /// Gets user info of a given user key.
        /// </summary>
        /// <param name="userKey">The user's key</param>
        /// <returns>An IUserInfo object</returns>
        public UserRecord GetUserInfoByUserKey(string userKey)
        {
            throw new NotImplementedException("GetUserInfoByUserKey not implemented");
        }

        /// <summary>
        /// Gets a specific data of a given user.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="parameterName">The name of a parameter in user data, such as email, telphone, or address</param>
        /// <returns>A data value , null if non-exist</returns>
        public string GetUserData(string userName, string parameterName)
        {
            return null;
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
            return new string[0]; // to be implemented
        }

		/// <summary>
		/// Gets all roles defined for the schema
		/// </summary>
		/// <returns>An array of all roles</returns>
		public string[] GetAllRoles()
		{
			string[] roles = null;

			DirectoryEntry de = GetDirectoryEntry();
			DirectorySearcher ds = new DirectorySearcher(de);

			StringBuilder filter = new StringBuilder();
			filter.Append("(objectCategory=group)");

			ds.Filter = filter.ToString();
			ds.SearchScope = SearchScope.Subtree;
			ds.PropertiesToLoad.Add("cn");

			SearchResultCollection results = ds.FindAll();

			if (results != null)
			{
				roles = new string[results.Count];
				int index = 0;
				foreach (SearchResult searchResult in results)
				{
					if (searchResult.Properties.Contains("cn"))
					{
						roles[index++] = searchResult.Properties["cn"][0].ToString();
					}
					else
					{
						roles[index++] = "";
					}
				}
			}
			else
			{
				roles = new string[0];
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
			string[] users;
			
			DirectoryEntry de = GetDirectoryEntry();
			DirectorySearcher ds = new DirectorySearcher(de);

			StringBuilder filter = new StringBuilder();
			filter.Append("(&(objectCategory=person)(objectClass=user)(memberOf=" + role + "))");

			ds.Filter = filter.ToString();
			ds.SearchScope = SearchScope.Subtree;
			ds.PropertiesToLoad.Add("SAMAccountName");

			SearchResultCollection results = ds.FindAll();

			if (results != null)
			{
				users = new string[results.Count];
				int index = 0;
				foreach (SearchResult searchResult in results)
				{
					if (searchResult.Properties.Contains("SAMAccountName"))
					{
						users[index++] = searchResult.Properties["SAMAccountName"][0].ToString();
					}
					else
					{
						users[index++] = "";
					}
				}
			}
			else
			{
				users = new string[0];
			}

			return users;
		}

		/// <summary>
		/// Gets all users defined for the schema
		/// </summary>
		/// <returns>An array of all users</returns>
		public string[] GetAllUsers()
		{
			string[] users;
			
			DirectoryEntry de = GetDirectoryEntry();
			DirectorySearcher ds = new DirectorySearcher(de);

			StringBuilder filter = new StringBuilder();
			filter.Append("(&(objectCategory=person)(objectClass=user))");

			ds.Filter = filter.ToString();
			ds.SearchScope = SearchScope.Subtree;
			ds.PropertiesToLoad.Add("SAMAccountName");

			SearchResultCollection results = ds.FindAll();

			if (results != null)
			{
				users = new string[results.Count];
				int index = 0;
				foreach (SearchResult searchResult in results)
				{
					if (searchResult.Properties.Contains("SAMAccountName"))
					{
						users[index++] = searchResult.Properties["SAMAccountName"][0].ToString();
					}
					else
					{
						users[index++] = "";
					}
				}
			}
			else
			{
				users = new string[0];
			}

			return users;
		}

		/// <summary>
		/// Add an user
		/// </summary>
		/// <param name="userName">The user's unique id</param>
		/// <param name="password">The user's password</param>
        /// <param name="userData">The user's data</param>
		public void AddUser(string userName, string password, string[] userData)
		{

		}

        /// <summary>
        /// Add an user with optional user info
        /// </summary>
        /// <param name="userInfo">The user's data</param>
        public void AddUser(UserRecord userInfo)
        {
            throw new NotImplementedException("AddUser not implemented");
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
        }

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="userInfo">The user's info</param>
        public void ChangeUserInfoByName(string userName, UserRecord userInfo)
        {
            throw new NotImplementedException("ChangeUserInfoByName not implemented");
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
        }

		/// <summary>
		/// Delete an user
		/// </summary>
		/// <param name="userName">The user's unique id</param>
		public void DeleteUser(string userName)
		{

		}

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="roleName">The unique role name</param>
        /// <param name="roleData">Role's data</param>
        public void AddRole(string roleName, string[] roleData)
		{

		}

		/// <summary>
		/// Delete a role
		/// </summary>
		/// <param name="roleName">The unique role name</param>
		public void DeleteRole(string roleName)
		{

		}

		/// <summary>
		/// Add a mapping for user and role
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void AddUserRoleMapping(string userName, string roleName)
		{

		}

		/// <summary>
		/// Delete a mapping of user and role.
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void DeleteUserRoleMapping(string userName, string roleName)
		{

		}

        /// <summary>
        /// Gets user's emails
        /// </summary>
        /// <param name="userName">The user name</param>
        public string[] GetUserEmails(string userName)
        {
            return new string[0]; // TODO
        }

		#endregion

		/// <summary>
		/// Create an entry in the Active Directory
		/// </summary>
		/// <returns>DirectoryEntry</returns>
		private DirectoryEntry GetDirectoryEntry()
		{
			if (this.ConnectionString == null || this.ConnectionString.Length == 0)
			{
				throw new Exception("The conection string of the directory isn't specified.");
			}

			if (this.UserName == null || this.UserName.Length == 0)
			{
				throw new Exception("The user name for conecting to the directory isn't specified.");
			}

			if (this.Password == null || this.Password.Length == 0)
			{
				throw new Exception("The user password for conecting to the directory isn't specified.");
			}

			DirectoryEntry de = new DirectoryEntry();
			de.Path = this.ConnectionString;
			de.Username = this.UserName;
			de.Password = this.Password;

			return de;
		}

		/// <summary>
		/// Create an entry in the Active Directory
		/// </summary>
		/// <returns>DirectoryEntry</returns>
		private DirectoryEntry GetDirectoryEntry(string path, string userName, string password)
		{
			string thePath;
			string theUserName;
			string thePassWord;

			if (path != null)
			{
				thePath = path;
			}
			else
			{
				thePath = this.ConnectionString;
			}

			if (userName != null)
			{
				theUserName = userName;
			}
			else
			{
				theUserName = this.UserName;
			}

			if (password != null)
			{
				thePassWord = password;
			}
			else
			{
				thePassWord = this.Password;
			}


			DirectoryEntry de = new DirectoryEntry();
			de.Path = thePath;
			de.Username = theUserName;
			de.Password = thePassWord;

			return de;
		}
	}
}