/*
* @(#) UserDataCache.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Web;
    using System.Text;
    using System.Data;
    using System.Threading;
	using System.Web.Caching;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.Util;

	/// <summary>
	/// This is the single cache for user data to speed up performance
	/// </summary>
	/// <version> 	1.0.0	14 Nov 2011 </version>
	public class UserDataCache
	{	
        private IKeyValueStore _cachedUserRoles;
        private IKeyValueStore _cachedUserData;
        private IKeyValueStore _cachedUserInfoByName;
        private IKeyValueStore _cachedUserInfoByKey;
        private IKeyValueStore _cachedRoleData;
        private IKeyValueStore _cachedRoleUsers;
        private string[] _allUsers;
        private string[] _allRoles;
        private UserInfoVersion _userInfoVersion;

        // Static cache object, all invokers will use this cache object.
        private static UserDataCache theCache;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private UserDataCache()
		{
            _cachedUserRoles = KeyValueStoreFactory.TheInstance.Create("UserDataCache.UserRoles");
            _cachedUserData = KeyValueStoreFactory.TheInstance.Create("UserDataCache.UserData");
            _cachedUserInfoByName = KeyValueStoreFactory.TheInstance.Create("UserDataCache.UserInfoByName");
            _cachedUserInfoByKey = KeyValueStoreFactory.TheInstance.Create("UserDataCache.UserInfoByKey");
            _cachedRoleData = KeyValueStoreFactory.TheInstance.Create("UserDataCache.RoleData");
            _cachedRoleUsers = KeyValueStoreFactory.TheInstance.Create("UserDataCache.RoleUsers");
            _allUsers = null;
            _allRoles = null;
            _userInfoVersion = UserInfoVersion.Unknown; // default
        }

		/// <summary>
		/// Gets the UserDataCache instance.
		/// </summary>
		/// <returns> The UserDataCache instance.</returns>
		static public UserDataCache Instance
		{
			get
			{
				return theCache;
			}
		}

        public void Initialize()
        {
            try
            {
                CMUserManager userManager = new CMUserManager();

                // load all user and set user data to the cache
                string[] allUsers = userManager.GetAllUsers();

                // load all user's roles to the cache
                foreach (string user in allUsers)
                {
                    userManager.GetRoles(user);
                }

                // load all roles and role data to the cache
                string[] allRoles = userManager.GetAllRoles();

                // load role's users for each role to the cache
                foreach (string role in allRoles)
                {
                    userManager.GetUsers(role);
                }
            }
            catch (Exception)
            {
                // the database connection string may be inavlid, ignore the error. the cache will be initialized later
            }
        }

        /// <summary>
        /// Gets the enum value to indicate whether the UserInfo database was created for the versions older than 7.0 release
        /// </summary>
        public UserInfoVersion UserInfoVersion
        {
            get
            {
                return _userInfoVersion;
            }
            set
            {
                _userInfoVersion = value;
            }
        }

        /// <summary>
        /// Cached string array which contains loging ids of all users
        /// </summary>
        public string[] AllUsers
        {
            get
            {
                // return a copy of all users to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                if (_allUsers != null)
                {
                    return (string[])_allUsers.Clone();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _allUsers = value;
            }
        }

        /// <summary>
        /// Cached string array which contains names of all roles
        /// </summary>
        public string[] AllRoles
        {
            get
            {
                // return a copy of all roles to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                if (_allRoles != null)
                {
                    return (string[])_allRoles.Clone();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _allRoles = value;
            }
        }

        /// <summary>
        /// Set the roles of an user in the cache
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        public void SetUserRoles(string user, string[] roles)
        {
            if (this._cachedUserRoles.Contains(user))
            {
                this._cachedUserRoles.Remove(user);
            }

            _cachedUserRoles.Add<string[]>(user, roles);
        }

        /// <summary>
        /// Gets roles of an user from the caceh
        /// </summary>
        /// <param name="user"></param>
        /// <returns>An array of roles</returns>
        public string[] GetUserRoles(string user)
        {
            if (_cachedUserRoles.Contains(user))
            {
                // return a copy of cached roles to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                return (string[])(_cachedUserRoles.Get<string[]>(user)).Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the users of a role in the cache
        /// </summary>
        /// <param name="role"></param>
        /// <param name="users"></param>
        public void SetRoleUsers(string role, string[] users)
        {
            if (this._cachedRoleUsers.Contains(role))
            {
                this._cachedRoleUsers.Remove(role);
            }

            _cachedRoleUsers.Add<string[]>(role, users);
        }

        /// <summary>
        /// Gets users of a role from the caceh
        /// </summary>
        /// <param name="role"></param>
        /// <returns>An array of users</returns>
        public string[] GetRoleUsers(string role)
        {
            if (_cachedRoleUsers.Contains(role))
            {
                // return a copy of cached roles to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                return (string[])(_cachedRoleUsers.Get<string[]>(role)).Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clear the caches that keep the user and role data
        /// </summary>
        public void ClearUserAndRoleCaches()
        {
            this._cachedUserRoles.Clear();
            this._cachedRoleUsers.Clear();
        }

        /// <summary>
        /// Set an user's data in the cache
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userData"></param>
        public void SetUserData(string userName, string[] userData)
        {
            if (this._cachedUserData.Contains(userName))
            {
                this._cachedUserData.Remove(userName);
            }

            this._cachedUserData.Add<string[]>(userName, userData);
        }

        /// <summary>
        /// Set an user's info in the cache
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userInfo"></param>
        public void SetUserInfoByName(string userName, UserRecord userInfo)
        {
            if (this._cachedUserInfoByName.Contains(userName))
            {
                this._cachedUserInfoByName.Remove(userName);
            }

            this._cachedUserInfoByName.Add<UserRecord>(userName, userInfo);
        }

        /// <summary>
        /// Set an user's info in the cache
        /// </summary>
        /// <param name="userKey"></param>
        /// <param name="userInfo"></param>
        public void SetUserInfoByKey(string userKey, UserRecord userInfo)
        {
            if (this._cachedUserInfoByKey.Contains(userKey))
            {
                this._cachedUserInfoByKey.Remove(userKey);
            }

            this._cachedUserInfoByKey.Add<UserRecord>(userKey, userInfo);
        }

        /// <summary>
        /// Gets an user's data from the caceh
        /// </summary>
        /// <param name="user"></param>
        /// <returns>An array of data</returns>
        public string[] GetUserData(string user)
        {
            if (_cachedUserData.Contains(user))
            {
                // return a copy of cached user data to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                return (string[])(_cachedUserData.Get<string[]>(user)).Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an user's info from the caceh
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>An IUserInfo object, null if not exists</returns>
        public UserRecord GetUserInfoByName(string userName)
        { 
            if (_cachedUserInfoByName.Contains(userName))
            {
                // return a copy of cached user info to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                return _cachedUserInfoByName.Get<UserRecord>(userName).Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an user's info from the caceh
        /// </summary>
        /// <param name="userKey"></param>
        /// <returns>An IUserInfo object, null if not exists</returns>
        public UserRecord GetUserInfoByKey(string userKey)
        {
            if (_cachedUserInfoByKey.Contains(userKey))
            {
                // return a copy of cached user info to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                return _cachedUserInfoByKey.Get<UserRecord>(userKey).Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set an role's data in the cache
        /// </summary>
        /// <param name="role"></param>
        /// <param name="roleData"></param>
        public void SetRoleData(string role, string[] roleData)
        {
            if (this._cachedRoleData.Contains(role))
            {
                this._cachedRoleData.Remove(role);
            }

            this._cachedRoleData.Add<string[]>(role, roleData);
        }

        /// <summary>
        /// Gets an role's data from the caceh
        /// </summary>
        /// <param name="role"></param>
        /// <returns>An array of data</returns>
        public string[] GetRoleData(string role)
        {
            if (_cachedRoleData.Contains(role))
            {
                // return a copy of cached role data to avoid memory leak. since caller may create a large number
                // of objects that could be associated with the returned user list. if the return objects are
                // cached, the associated objects will not be collected by GC
                return (string[])_cachedRoleData.Get<string[]>(role).Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clear the caches that keep the user's data
        /// </summary>
        public void ClearUserDataCaches()
        {
            this._cachedUserData.Clear();
            this._cachedUserInfoByName.Clear();
            this._cachedUserInfoByKey.Clear();
            this._allUsers = null;
        }

        /// <summary>
        /// Clear the caches that keep the role's data
        /// </summary>
        public void ClearRoleDataCaches()
        {
            this._cachedRoleData.Clear();
            this._allRoles = null;
        }

		static UserDataCache()
		{
			// Initializing the cache.
			{
				theCache = new UserDataCache();
			}
		}
	}

    public enum UserInfoVersion
    {
        Unknown,
        PriorTo7,
        Version7
    }
}