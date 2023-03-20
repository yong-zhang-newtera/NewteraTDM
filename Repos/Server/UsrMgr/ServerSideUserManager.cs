/*
* @(#)ServerSideUserManager.cs
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
	using System.Security.Principal;
    using System.Collections;
	
	using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Principal;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.Engine.Cache;

	/// <summary> 
	/// An implementation of IUserManager that serves as a wrapper to specific
	/// implementation of IUserManager. The main purpose if the wrapper class is
	/// to ensure that the super user authentication is independent of any
	/// specific implementation.
	/// </summary>
	/// <version> 1.0.0 22 Oct 2006 </version>
	public class ServerSideUserManager : IUserManager
	{
		IUserManager _userManager;

		/// <summary>
		/// Initiate an instance of ServerSideUserManager class
		/// </summary>
		public ServerSideUserManager()
		{
			_userManager = UserManagerFactory.Instance.Create();
		}

		#region IUserManager Members

		/// <summary>
		/// Gets the information indicating whether the user manager is read-only.
		/// </summary>
		/// <returns>return false</returns>
		public bool IsReadOnly 
		{
			get
			{
				return _userManager.IsReadOnly;
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
			bool isAuthenticated = _userManager.Authenticate(userName, password);

			if (!isAuthenticated)
			{
				// check if it is the super user
				CMUserManager cmmanager = new CMUserManager();

				if(cmmanager.AuthenticateSuperUser(userName, password))
				{
					isAuthenticated = true;
				}
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
			// check if it is the super user, then return super user role
			CMUserManager cmmanager = new CMUserManager();

			string[] roles = cmmanager.GetSuperUserRoles(userName);

			if (roles == null || roles.Length == 0)
			{
				// not a super user, get the regular roles
                roles = _userManager.GetRoles(userName);
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
            // check if it is the super user, then return super user role
            CMUserManager cmmanager = new CMUserManager();

            return _userManager.GetRoles(userName, type);
        }

        /// <summary>
        /// Gets user data of a given user.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>An array of user's data</returns>
        public string[] GetUserData(string userName)
        {
            return _userManager.GetUserData(userName);

            /*
            string[] userData = _userManager.GetUserData(userName);

            if (userData == null || userData.Length < 3)
            {
                // custom user manager doesn't return an array or return an array with
                // smaller dimension, create an empty one
                string[] newUserData = new string[3];
                if (userData != null && userData.Length >= 1)
                {
                    newUserData[0] = userData[0];
                }

                if (userData != null && userData.Length >= 2)
                {
                    newUserData[1] = userData[1];
                }

                userData = newUserData;
            }

            return userData;
            */
        }

        /// <summary>
        /// Gets user info of a given user name.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>An IUserInfo object</returns>
        public UserRecord GetUserInfoByUserName(string userName)
        {
            return _userManager.GetUserInfoByUserName(userName);
        }

        /// <summary>
        /// Gets user info of a given user key.
        /// </summary>
        /// <param name="userKey">The user's key</param>
        /// <returns>An IUserInfo object</returns>
        public UserRecord GetUserInfoByUserKey(string userKey)
        {
            return _userManager.GetUserInfoByUserKey(userKey);
        }

        /// <summary>
        /// Gets a specific data of a given user.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="parameterName">The name of a parameter in user data, such as email, telphone, or address</param>
        /// <returns>A data value , null if non-exist</returns>
        public string GetUserData(string userName, string parameterName)
        {
            return _userManager.GetUserData(userName, parameterName);
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
            string[] roleData = _userManager.GetRoleData(roleName);

            if (roleData == null || roleData.Length < 3)
            {
                // custom user manager doesn't return an array or return an array with
                // smaller dimension, create an empty one
                string[] newRoleData = new string[2];
                if (roleData != null && roleData.Length >= 1)
                {
                    newRoleData[0] = roleData[0];
                }

                if (roleData != null && roleData.Length >= 2)
                {
                    newRoleData[1] = roleData[1];
                }

                roleData = newRoleData;
            }

            return roleData;
        }

		/// <summary>
		/// Gets all roles defined for the schema
		/// </summary>
		/// <returns>An array of all roles</returns>
		public string[] GetAllRoles()
		{
            return _userManager.GetAllRoles();
		}

		/// <summary>
		/// Gets users that belongs to the given role.
		/// </summary>
		/// <param name="role">The role</param>
		/// <returns>An array of role's users.</returns>
		public string[] GetUsers(string role)
		{
            return _userManager.GetUsers(role);
		}

		/// <summary>
		/// Gets all users defined for the schema
		/// </summary>
		/// <returns>An array of all users</returns>
		public string[] GetAllUsers()
		{
            return _userManager.GetAllUsers();
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
			_userManager.AddUser(userName, password, userData);
		}

        /// <summary>
        /// Add an user with optional user info
        /// </summary>
        /// <param name="userInfo">The user's data</param>
        public void AddUser(UserRecord userInfo)
        {
            _userManager.AddUser(userInfo);
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
			_userManager.ChangeUserPassword(userName, oldPassword, newPassword);
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
            _userManager.ChangeUserData(userName, userData);
        }

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="userInfo">The user's info</param>
        public void ChangeUserInfoByName(string userName, UserRecord userInfo)
        {
            _userManager.ChangeUserInfoByName(userName, userInfo);
        }

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userKey">The user key</param>
        /// <param name="userInfo">The user's info</param>
        public void ChangeUserInfoByKey(string userKey, UserRecord userInfo)
        {
            _userManager.ChangeUserInfoByName(userKey, userInfo);
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
            _userManager.ChangeRoleData(roleName, roleData);
        }

		/// <summary>
		/// Delete an user
		/// </summary>
		/// <param name="userName">The user's unique id</param>
		public void DeleteUser(string userName)
		{
			_userManager.DeleteUser(userName);
		}

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="roleName">The unique role name</param>
        /// <param name="roleData">Role's data</param>
        public void AddRole(string roleName, string[] roleData)
		{
            _userManager.AddRole(roleName, roleData);
		}

		/// <summary>
		/// Delete a role
		/// </summary>
		/// <param name="roleName">The unique role name</param>
		public void DeleteRole(string roleName)
		{
			_userManager.DeleteRole(roleName);
		}

		/// <summary>
		/// Add a mapping for user and role
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void AddUserRoleMapping(string userName, string roleName)
		{
			_userManager.AddUserRoleMapping(userName, roleName);
		}

		/// <summary>
		/// Delete a mapping of user and role.
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void DeleteUserRoleMapping(string userName, string roleName)
		{
			_userManager.DeleteUserRoleMapping(userName, roleName);
		}

        /// <summary>
        /// Gets user's emails
        /// </summary>
        /// <param name="userName">The user name</param>
        public string[] GetUserEmails(string userName)
        {
            return _userManager.GetUserEmails(userName);
        }

		#endregion

        /// <summary>
        /// Gets the display text for the user
        /// </summary>
        public string GetDisplayText(string userName)
        {
            string displayText = null;
 
            string[] userData = this.GetUserData(userName);
            string text;
            if (userData != null)
            {
                text = UsersListHandler.GetFormatedName(userData[0], userData[1]);
                if (string.IsNullOrEmpty(text))
                {
                    text = userName;
                }
            }
            else
            {
                text = userName;
            }

            displayText = text;

            return displayText;
        }
	}
}