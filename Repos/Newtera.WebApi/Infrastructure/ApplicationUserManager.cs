using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;

namespace Newtera.WebApi.Infrastructure
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appDbContext = new ServerSideUserManager();

            var appUserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(appDbContext));

            //Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<ApplicationUser>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            //Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = false,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            //appUserManager.EmailService = new Newtera.WebApi.Services.EmailService();

            // use customized password hash
            appUserManager.PasswordHasher = new CustomPasswordHasher();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromHours(6)
                };
            }

            return appUserManager;
        }

        public override Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return base.FindByIdAsync(userId);
        }

        public override Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return base.FindByNameAsync(userName);
        }

        public override IQueryable<ApplicationUser> Users
        {
            get
            {
                return base.Users;
            }
        }

        public override Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var userManager = new ServerSideUserManager();
            if (userManager.Authenticate(userName, password))
            {
                return FindByNameAsync(userName);
            }

            return Task.FromResult<ApplicationUser>(null);
            //return base.FindAsync(userName, password);
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return base.CreateAsync(user, password);
        }

        public override Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            var store = this.Store as UserStore<ApplicationUser>;

            store.UpdateAsync(user);

            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public override Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var store = this.Store as UserStore<ApplicationUser>;

            ApplicationUser user = new ApplicationUser();
            user.UserName = userId;
            user.PasswordHash = currentPassword;

            store.SetPasswordHashAsync(user, newPassword);

            return Task.FromResult<IdentityResult>(IdentityResult.Success);

        }

        public override Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            return base.DeleteAsync(user);
        }

        public override Task<bool> IsInRoleAsync(string userId, string role)
        {
            return base.IsInRoleAsync(userId, role);
        }

        public override Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles)
        {
            return base.AddToRolesAsync(userId, roles);
        }

        public override Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            return base.AddToRoleAsync(userId, role);
        }

        public override Task<IdentityResult> RemoveFromRoleAsync(string userId, string role)
        {
            return base.RemoveFromRoleAsync(userId, role);
        }
    }
}