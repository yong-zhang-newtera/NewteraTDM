using System.Collections.Generic;

using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Class that represents the UserRoles table in the Sql Database
    /// </summary>
    public class UserRolesTable
    {
        private IUserManager _customUserManager;

        public UserRolesTable(IUserManager customUserManager)
        {
            _customUserManager = customUserManager;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<string> FindByUserId(string userId)
        {
            List<string> roles = new List<string>();

            string[] userRoles = _customUserManager.GetRoles(userId);

            foreach (string rid in userRoles)
            {
                roles.Add(rid);
            }

            return roles;
        }

        /// <summary>
        /// Deletes all roles from a user in the UserRoles table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string[] userRoles = _customUserManager.GetRoles(userId);

            foreach (string roleId in userRoles)
            {
                _customUserManager.DeleteUserRoleMapping(userId, roleId);
            }

            return 1;
        }

        /// <summary>
        /// Deletes a role from a user in the UserRoles table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int DeleteRole(string userId, string roleId)
        {
            _customUserManager.DeleteUserRoleMapping(userId, roleId);

            return 1;
        }

        /// <summary>
        /// Inserts a new role for a user in the UserRoles table
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="roleId">The Role's id</param>
        /// <returns></returns>
        public int Insert(ApplicationUser user, string roleId)
        {
            _customUserManager.AddUserRoleMapping(user.Id, roleId);

            return 1;
        }
    }

}