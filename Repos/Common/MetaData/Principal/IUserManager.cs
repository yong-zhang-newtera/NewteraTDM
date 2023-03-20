/*
* @(#)IUserManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Principal
{
	using System;

	using Newtera.Common.Core;

	/// <summary>
	/// Represents an interface for authenticating an user or getting the current
	/// user information. This interface will have different implementation on
	/// the window client side and server side.
	/// </summary>
	/// <version>  	1.0.0 31 Nov. 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IUserManager
	{
		/// <summary>
		/// Gets the information indicating whether the user manager is read-only.
		/// </summary>
		/// <returns>true if the user manager is read-only, false otherwise</returns>
		bool IsReadOnly {get;}

		/// <summary>
		/// Authenticate an user with given name and password
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="password">The user password</param>
		/// <returns>true if user is authenticated, false otherwise</returns>
		bool Authenticate(string userName, string password);

		/// <summary>
		/// Gets roles that an user of the given name is in, including subroles.
		/// </summary>
		/// <param name="userName">The user's name</param>
		/// <returns>An array of user's roles</returns>
		string[] GetRoles(string userName);

        /// <summary>
        /// Gets roles of a type that an user belongs to.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="type">a role type</param>
        /// <returns>An array of user's roles</returns>
        string[] GetRoles(string userName, string type);

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
        string[] GetUserData(string userName);

        /// <summary>
        /// Gets user info of a given user name.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns>An IUserInfo object</returns>
        UserRecord GetUserInfoByUserName(string userName);

        /// <summary>
        /// Gets user info of a given user key.
        /// </summary>
        /// <param name="userKey">The user's key</param>
        /// <returns>An IUserInfo object</returns>
        UserRecord GetUserInfoByUserKey(string userKey);

        /// <summary>
        /// Gets a specific data of a given user.
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <param name="parameterName">The name of a parameter in user data, such as email, telphone, or address</param>
        /// <returns>A data value , null if non-exist</returns>
        string GetUserData(string userName, string parameterName);

        /// <summary>
        /// Gets role data of a given role.
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns>An array of role's data</returns>
        /// <remarks>
        /// In the roleData array, the first entry is the role's display text,
        /// the second entry is the role's type
        /// </remarks>
        string[] GetRoleData(string roleName);

		/// <summary>
		/// Gets all roles defined for the schema
		/// </summary>
		/// <returns>An array of roles for the schema</returns>
		string[] GetAllRoles();

		/// <summary>
		/// Gets user's login names that belongs to the given role.
		/// </summary>
		/// <param name="role">The role</param>
		/// <returns>An array of role's users.</returns>
		string[] GetUsers(string role);

		/// <summary>
		/// Gets all user's login name
		/// </summary>
		/// <returns>An array of users for the schema</returns>
		string[] GetAllUsers();

        /// <summary>
        /// Add an user with optional user data
        /// </summary>
        /// <param name="userName">The user's unique id</param>
        /// <param name="password">The user's password</param>
        /// <param name="userData">The user's data</param>
        /// <remarks>
        /// In the userData array, the first entry is the user's last name,
        /// the second entry is the user's first name,
        /// and the third entry is the user's email address
        /// </remarks>
        void AddUser(string userName, string password, string[] userData);

        /// <summary>
        /// Add an user with optional user info
        /// </summary>
        /// <param name="userInfo">The user's data</param>
        void AddUser(UserRecord userInfo);

        /// <summary>
        /// Change an user's password.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="oldPassword">The old password</param>
        /// <param name="newPassword">The new password</param>
        void ChangeUserPassword(string userName, string oldPassword, string newPassword);

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
        void ChangeUserData(string userName, string[] userData);

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="userInfo">The user's info</param>
        void ChangeUserInfoByName(string userName, UserRecord userInfo);

        /// <summary>
        /// Change an user's info.
        /// </summary>
        /// <param name="userKey">The user key</param>
        /// <param name="userInfo">The user's info</param>
        void ChangeUserInfoByKey(string userKey, UserRecord userInfo);

        /// <summary>
        /// Change an role's data.
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="roleData">The role's data</param>
        /// <remarks>
        /// In the roleData array, the first entry is the role's display text,
        /// </remarks>
        void ChangeRoleData(string roleName, string[] roleData);

		/// <summary>
		/// Delete an user
		/// </summary>
		/// <param name="userName">The user's unique id</param>
		void DeleteUser(string userName);

		/// <summary>
		/// Add a new role
		/// </summary>
		/// <param name="roleName">The unique role name</param>
        /// <param name="roleData">Role's data</param>
		void AddRole(string roleName, string[] roleData);

		/// <summary>
		/// Delete a role
		/// </summary>
		/// <param name="roleName">The unique role name</param>
		void DeleteRole(string roleName);

		/// <summary>
		/// Add a mapping for user and role
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		void AddUserRoleMapping(string userName, string roleName);

		/// <summary>
		/// Delete a mapping of user and role.
		/// </summary>
		/// <param name="userName">The user name</param>
		/// <param name="roleName">The role name</param>
		void DeleteUserRoleMapping(string userName, string roleName);

        /// <summary>
		/// Gets user's emails
		/// </summary>
        /// <param name="userName">The user name</param>
        /// <returns>User's emails, do not return null.</returns>
        string[] GetUserEmails(string userName);
	}
}