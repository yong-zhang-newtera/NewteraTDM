using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Class that represents the Role table in the MySQL Database
    /// </summary>
    public class RoleTable
    {
        private IUserManager _customUserManager;

        /// <summary>
        /// Constructor that takes a IUserManager instance 
        /// </summary>
        /// <param name="customUserManager"></param>
        public RoleTable(IUserManager customUserManager)
        {
            _customUserManager = customUserManager;
        }

        /// <summary>
        /// Deltes a role from the Roles table
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns></returns>
        public int Delete(string roleId)
        {
            _customUserManager.DeleteRole(roleId);
            return 1;
        }

        /// <summary>
        /// Inserts a new Role in the Roles table
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns></returns>
        public int Insert(ApplicationRole role)
        {
            string roleId = role.Id;
            string[] roleData = new string[2];
            if (!string.IsNullOrEmpty(role.Name))
            {
                roleData[0] = role.Name;
            }
            else
            {
                roleData[0] = null;
            }

            if (!string.IsNullOrEmpty(role.RoleType))
            {
                roleData[1] = role.RoleType;
            }
            else
            {
                roleData[0] = null;
            }

            _customUserManager.AddRole(roleId, roleData);

            return 1;
        }

        /// <summary>
        /// Returns a role name given the roleId
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns>Role name</returns>
        public string GetRoleName(string roleId)
        {
            string[] roleData = _customUserManager.GetRoleData(roleId);
            string roleName = null;
            if (!string.IsNullOrEmpty(roleData[0]))
            {
                roleName = roleData[0];
            }

            return roleName;
        }

        /// <summary>
        /// Returns the role Id given a role name
        /// </summary>
        /// <param name="roleName">Role's name</param>
        /// <returns>Role's Id</returns>
        public string GetRoleId(string roleName)
        {
            throw new NotSupportedException("GetRoleId not supported");
        }

        /// <summary>
        /// Gets the ApplicationRole given the role Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public ApplicationRole GetRoleById(string roleId)
        {
            ApplicationRole role = null;

            string[] roleData = _customUserManager.GetRoleData(roleId);

            if (roleData != null)
            {
                role = new ApplicationRole();
                role.Id = roleId;
                if (!string.IsNullOrEmpty(roleData[0]))
                {
                    role.Name = roleData[0];
                }

                if (!string.IsNullOrEmpty(roleData[1]))
                {
                    role.RoleType = roleData[1];
                }
            }

            return role;

        }

        /// <summary>
        /// Gets the ApplicationRole given the role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public ApplicationRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            ApplicationRole role = GetRoleById(roleId);

            return role;
        }

        public int Update(ApplicationRole role)
        {
            return 1;
        }
    }
}