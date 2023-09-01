using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtera.Server.UsrMgr;

namespace Newtera.WebApi.Infrastructure
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var appDbContext = new ServerSideUserManager();
            var appRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(appDbContext));

            return appRoleManager;
        }
    }
}