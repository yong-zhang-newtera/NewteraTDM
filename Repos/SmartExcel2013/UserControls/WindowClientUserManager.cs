/*
* @(#)WindowClientUserManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace SmartExcel2013
{
	using System;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Principal;
    using Newtera.WinClientCommon;

	/// <summary> 
	/// Windows client side implementation of IUserManager that uses a web service
	/// for getting user info
	/// </summary>
	/// <version> 1.0.0 13 Dec 2006 </version>
	public class WindowClientUserManager : IUserManager
	{
		private UserInfoServiceStub _service;

		/// <summary>
		/// Initiate an instance of WindowClientUserManager class
		/// </summary>
		internal WindowClientUserManager()
		{
			_service = new UserInfoServiceStub();
		}

		#region IUserManager Members

		/// <summary>
		/// Gets the information indicating whether the user manager is read-only.
		/// </summary>
		/// <returns>true if the user manager is read-only, false otherwise</returns>
		public bool IsReadOnly 
		{
			get
			{
				return _service.IsReadOnly();
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
			return _service.Authenticate(userName, password);
		}

        /// <summary>
        /// Gets roles that an user of the given name is in, including sub-roles.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>A collection of roles</returns>
        public string[] GetRoles(string userName)
        {
            return _service.GetRoles(userName, null);
        }

		/// <summary>
		/// Gets roles that an user of the given name is in, including sub-roles.
		/// </summary>
		/// <param name="userName">The user's name</param>
        /// <param name="type">The role type</param>
		/// <returns>A collection of roles</returns>
		public string[] GetRoles(string userName, string type)
		{
			return _service.GetRoles(userName, type);
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
            return _service.GetUserData(userName);
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
            return null; // TODO
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
            return _service.GetRoleData(roleName);
        }

		/// <summary>
		/// Gets all roles available for the schema.
		/// </summary>
		/// <returns>An array of all roles</returns>
		public string[] GetAllRoles()
		{
			return _service.GetAllRoles();
		}

		/// <summary>
		/// Gets users that belongs to the given role.
		/// </summary>
		/// <param name="role">The role</param>
		/// <returns>An array of role's users.</returns>
		public string[] GetUsers(string role)
		{
			return _service.GetUsers(role);
		}

		/// <summary>
		/// Gets all users available for the schema.
		/// </summary>
		/// <returns>An array of all users</returns>
		public string[] GetAllUsers()
		{
			return _service.GetAllUsers();
		}

		/// <summary>
		/// Add a user
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="password">The user password</param>
        /// <param name="userData">The user's data</param>
        /// <remarks>
        /// In the userData array, the first entry is the user's last name,
        /// the second entry is the user's first name,
        /// and the third entry is the user's email address
        /// </remarks>
        public void AddUser(string userName, string password, string[] userData)
		{
			_service.AddUser(userName, password, userData);
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
        /// Change a user's password.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="oldPassword">The old password</param>
        /// <param name="newPassword">The new password</param>
        public void ChangeUserPassword(string userName, string oldPassword,
			string newPassword)
		{		
			_service.ChangeUserPassword(userName, oldPassword, newPassword);
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
            _service.ChangeUserData(userName, userData);
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
            _service.ChangeRoleData(roleName, roleData);
        }

		/// <summary>
		/// Delete a user
		/// </summary>
		/// <param name="userName">The user name</param>
		public void DeleteUser(string userName)
		{
			_service.DeleteUser(userName);
		}

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="roleName">The unique role name</param>
        /// <param name="roleData">Role's data</param>
        public void AddRole(string roleName, string[] roleData)
		{
            _service.AddRole(roleName, roleData);
		}

		/// <summary>
		/// Delete a role
		/// </summary>
		/// <param name="rolerName">The role name</param>
		public void DeleteRole(string roleName)
		{
			_service.DeleteRole(roleName);
		}

		/// <summary>
		/// Add a mapping for user and role
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void AddUserRoleMapping(string userName, string roleName)
		{
			_service.AddUserRoleMapping(userName, roleName);
		}

		/// <summary>
		/// Delete a mapping of user and role.
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		public void DeleteUserRoleMapping(string userName, string roleName)
		{
			_service.DeleteUserRoleMapping(userName, roleName);
		}

        /// <summary>
        /// Gets user's emails
        /// </summary>
        /// <param name="userName">The user name</param>
        public string[] GetUserEmails(string userName)
        {
            return _service.GetUserEmails(userName);
        }

		#endregion

		/// <summary>
		/// Gets the name of the CM super user
		/// </summary>
		/// <returns>A string represents name of super user</returns>
		public string GetSuperUserName()
		{
			return _service.GetSuperUserName();
		}

		/// <summary>
		/// Authenticate for the super user.
		/// </summary>
		/// <param name="userName">user name</param>
		/// <param name="password">password</param>
		/// <returns>true if it is the super user, false otherwise.</returns>
		public bool AuthenticateSuperUser(string userName, string password)
		{
			return _service.AuthenticateSuperUser(userName, password);
		}

		/// <summary>
		/// Change the super user's password.
		/// </summary>
		/// <param name="userName">User name</param>
		/// <param name="oldPassword">Old password</param>
		/// <param name="newPassword">New password</param>
		public void ChangeSuperUserPassword(string userName,
			string oldPassword, string newPassword)
		{
			_service.ChangeSuperUserPassword(userName, oldPassword, newPassword);
		}
	}
}