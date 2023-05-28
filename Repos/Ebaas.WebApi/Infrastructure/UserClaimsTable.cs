using System.Collections.Generic;
using System.Security.Claims;

using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Class that represents the UserClaims table in the Sql Database
    /// </summary>
    public class UserClaimsTable
    {
        private IUserManager _customUserManager;

        public UserClaimsTable(IUserManager customUserManager)
        {
            _customUserManager = customUserManager;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string userId)
        {
            ClaimsIdentity claims = new ClaimsIdentity();

            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            return 1;
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public int Insert(Claim userClaim, string userId)
        {
            return 1;
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public int Delete(ApplicationUser user, Claim claim)
        {
            return 1;
        }
    }

}