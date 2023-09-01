using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http.Description;

using Swashbuckle.Swagger.Annotations;

using Newtera.WebApi.Models;
using Newtera.WebApi.Infrastructure;
using Newtera.Common.Core;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// The Accounts Service allows you to manage user accounts and roles in your application.
    /// It has a set of APIS operations for reading, creating, updating, deleting users or roles and
    /// assigning roles to users.
    /// </summary>
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        /// <summary>
        /// Get information of all users
        /// </summary>
        //[Authorize(Roles = "Admin")]
        [Route("users")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ApplicationUser>), Description = "A collection of user models")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public IHttpActionResult GetUsers()
        {
            try
            {
                return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the account info of a user by id
        /// </summary>
        /// <param name="Id">The unique id of a user</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}", Name = "GetUserById")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserReturnModel), Description = "An user model")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            try
            {
                var user = await this.AppUserManager.FindByIdAsync(Id);

                if (user != null)
                {
                    return Ok(this.TheModelFactory.Create(user));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }

        }

        /// <summary>
        /// Get the account info of a user by name
        /// </summary>
        /// <param name="username">The username for sign in</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("user/{username}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserReturnModel), Description = "An user models")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            try {
                var user = await this.AppUserManager.FindByNameAsync(username);

                if (user != null)
                {
                    return Ok(this.TheModelFactory.Create(user));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Create an account for a user
        /// </summary>
        /// <param name="createUserModel">The account info</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("create")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "An user's account is created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new ApplicationUser()
                {
                    UserName = createUserModel.Username,
                    Email = createUserModel.Email,
                    FirstName = createUserModel.FirstName,
                    LastName = createUserModel.LastName,
                };

                IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

                if (!addUserResult.Succeeded)
                {
                    return GetErrorResult(addUserResult);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Update the account info of a user
        /// </summary>
        /// <param name="updateUserModel">The updated account info</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("update")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "An user's account updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> UpdateUser(UpdateUserBindingModel updateUserModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = new ApplicationUser()
                {
                    UserName = updateUserModel.Username,
                    Email = updateUserModel.Email,
                    PhoneNumber = updateUserModel.PhoneNumber,
                    FirstName = updateUserModel.FirstName,
                    LastName = updateUserModel.LastName,
                    Picture = updateUserModel.Picture,
                };

                IdentityResult updateUserResult = await this.AppUserManager.UpdateAsync(user);

                if (!updateUserResult.Succeeded)
                {
                    return GetErrorResult(updateUserResult);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Confirm a user's email
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="code">The confirmed email</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "Email confirmed")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                {
                    ModelState.AddModelError("", "User Id and Code are required");
                    return BadRequest(ModelState);
                }

                IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return GetErrorResult(result);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }


        /// <summary>
        /// Change a user's password
        /// </summary>
        /// <param name="model">The change password data model</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ChangePassword")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "Password changed")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Delete a user's account by user id
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "An user deleted")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            try
            {
                //Only SuperAdmin or Admin can delete users (Later when implement roles)

                var appUser = await this.AppUserManager.FindByIdAsync(id);

                if (appUser != null)
                {
                    IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }

                    return Ok();

                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Assign a set of roles to a user identified by id
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <param name="rolesToAssign">An array of role's names</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/roles")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(void), Description = "A role assigned to the user")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(string), Description = "An error occured, see error message in res.message")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {
            try
            {
                var appUser = await this.AppUserManager.FindByIdAsync(id);

                if (appUser == null)
                {
                    return NotFound();
                }

                var currentRoles = await this.AppUserManager.GetRolesAsync(appUser.Id);

                var rolesNotExists = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();

                if (rolesNotExists.Count() > 0)
                {

                    ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                    return BadRequest(ModelState);
                }

                IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());

                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to remove user roles");
                    return BadRequest(ModelState);
                }

                IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);

                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add user roles");
                    return BadRequest(ModelState);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/assignclaims")]
        [HttpPut]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IHttpActionResult> AssignClaimsToUser([FromUri] string id, [FromBody] List<ClaimBindingModel> claimsToAssign)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appUser = await this.AppUserManager.FindByIdAsync(id);

                if (appUser == null)
                {
                    return NotFound();
                }

                foreach (ClaimBindingModel claimModel in claimsToAssign)
                {
                    if (appUser.Claims.Any(c => c.ClaimType == claimModel.Type))
                    {

                        await this.AppUserManager.RemoveClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
                    }

                    await this.AppUserManager.AddClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}/removeclaims")]
        [ResponseType(typeof(void))]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        public async Task<IHttpActionResult> RemoveClaimsFromUser([FromUri] string id, [FromBody] List<ClaimBindingModel> claimsToRemove)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appUser = await this.AppUserManager.FindByIdAsync(id);

                if (appUser == null)
                {
                    return NotFound();
                }

                foreach (ClaimBindingModel claimModel in claimsToRemove)
                {
                    if (appUser.Claims.Any(c => c.ClaimType == claimModel.Type))
                    {
                        await this.AppUserManager.RemoveClaimAsync(id, ExtendedClaimsProvider.CreateClaim(claimModel.Type, claimModel.Value));
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return BadRequest(ex.GetBaseException().Message);
            }
        }
    }
}