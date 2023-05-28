using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Class that represents the UserLogins table in the Sql Database
    /// </summary>
    public class UserLoginsTable
    {
        private IUserManager _customUserManager;

        /// <summary>
        /// Constructor that takes a Sql Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserLoginsTable(IUserManager customUserManager)
        {
            _customUserManager = customUserManager;
        }

        /// <summary>
        /// Deletes a login from a user in the UserLogins table
        /// </summary>
        /// <param name="user">User to have login deleted</param>
        /// <param name="login">Login to be deleted from user</param>
        /// <returns></returns>
        public int Delete(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotSupportedException("Delete UserLoginInfo not supported");
        }

        /// <summary>
        /// Deletes all Logins from a user in the UserLogins table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            throw new NotSupportedException("Delete all UserLoginInfo not supported");
        }

        /// <summary>
        /// Inserts a new login in the UserLogins table
        /// </summary>
        /// <param name="user">User to have new login added</param>
        /// <param name="login">Login to be added</param>
        /// <returns></returns>
        public int Insert(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotSupportedException("Insert UserLoginInfo not supported");
        }

        /// <summary>
        /// Return a userId given a user's login
        /// </summary>
        /// <param name="userLogin">The user's login info</param>
        /// <returns></returns>
        public string FindUserIdByLogin(UserLoginInfo userLogin)
        {
            throw new NotSupportedException("FindUserIdByLogin UserLoginInfo not supported");
        }

        /// <summary>
        /// Returns a list of user's logins
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<UserLoginInfo> FindByUserId(string userId)
        {
            throw new NotSupportedException("FindByUserId UserLoginInfo not supported");
        }
    }

}