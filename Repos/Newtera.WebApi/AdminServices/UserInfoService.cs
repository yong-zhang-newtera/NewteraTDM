using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Xml;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.WebApi.Infrastructure;

namespace Newtera.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform authentication and user info related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/userInfoService")]
    public class UserInfoServiceController : ApiController
    {
        /// <summary>
        /// Gets the information indicating whether the user information is
        /// read-only (for example, active directory based)
        /// </summary>
        /// <returns>true if it is read-only, false otherwise.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("IsReadOnly")]
        public HttpResponseMessage IsReadOnly()
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                return Request.CreateResponse(HttpStatusCode.OK, userManager.IsReadOnly);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Authenticate an user of given name and password
        /// </summary>
        /// <param name="name">User's name.</param>
        /// <param name="password">User's password</param>
        /// <returns>true if the user is authenticated, false otherwise.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("Authenticate/{name}/{password}")]
        public HttpResponseMessage Authenticate(string name, string password)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                return Request.CreateResponse(HttpStatusCode.OK, userManager.Authenticate(name, password));
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets an array of user's roles of a type.
        /// </summary>
        /// <param name="name">User's name.</param>
        /// <param name="type">Role type</param>
        /// <returns>An array of user's roles</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetTypeRoles/{name}/{type}")]
        public HttpResponseMessage GetTypeRoles(string name, string type)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();
                string[] roles = null;
                if (!string.IsNullOrEmpty(type))
                {
                    roles = userManager.GetRoles(name, type);

                }
                else
                {
                    roles = userManager.GetRoles(name);
                }

                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets an array of user's roles of a type.
        /// </summary>
        /// <param name="name">User's name.</param>
        /// <param name="type">Role type</param>
        /// <returns>An array of user's roles</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetRoles/{name}")]
        public HttpResponseMessage GetRoles(string name)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] roles = userManager.GetRoles(name);

                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
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
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetUserData/{name}")]
        public HttpResponseMessage GetUserData(string name)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] userData =  userManager.GetUserData(name);

                return Request.CreateResponse(HttpStatusCode.OK, userData);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets role data of a given role.
        /// </summary>
        /// <param name="name">The role's name</param>
        /// <returns>An array of role's data</returns>
        /// <remarks>
        /// In the roleData array, the first entry is the role's display text,
        /// the second entry is the user's type
        /// </remarks>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetRoleData/{name}")]
        public HttpResponseMessage GetRoleData(string name)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] roleData = userManager.GetRoleData(name);

                return Request.CreateResponse(HttpStatusCode.OK, roleData);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets an array of all available roles.
        /// </summary>
        /// <returns>An array of all available roles</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetAllRoles")]
        public HttpResponseMessage GetAllRoles()
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] allRoles = userManager.GetAllRoles();

                return Request.CreateResponse(HttpStatusCode.OK, allRoles);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets an array of role's users.
        /// </summary>
        /// <param name="role">Role.</param>
        /// <returns>An array of role's users</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetUsers/{role}")]
        public HttpResponseMessage GetUsers(string role)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] userRoles = userManager.GetUsers(role);

                return Request.CreateResponse(HttpStatusCode.OK, userRoles);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets an array of all available users.
        /// </summary>
        /// <returns>An array of all available users</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetAllUsers")]
        public HttpResponseMessage GetAllUsers()
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] allUsers = userManager.GetAllUsers();

                return Request.CreateResponse(HttpStatusCode.OK, allUsers);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Add a new user.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="password">The user password</param>
        /// <param name="userData">The user's data</param>
        /// <remarks>
        /// In the userData array, the first entry is the user's last name,
        /// the second entry is the user's first name,
        /// and the third entry is the user's email address
        /// </remarks>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("AddUser/{userName}/{password}")]
        public HttpResponseMessage AddUser(string userName, string password)
        {
            try
            {
                string content = Request.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(content))
                {
                    string[] userData = JsonConvert.DeserializeObject<string[]>(content);

                    IUserManager userManager = new ServerSideUserManager();

                    userManager.AddUser(userName, password, userData);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Change a user's password.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="oldPassword">The old password</param>
        /// <param name="newPassword">The new password</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("ChangeUserPassword/{userName}/{oldPassword}/{newPassword}")]
        public HttpResponseMessage ChangeUserPassword(string userName, string oldPassword,
            string newPassword)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                userManager.ChangeUserPassword(userName, oldPassword, newPassword);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
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
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ChangeUserData/{userName}")]
        public HttpResponseMessage ChangeUserData(string userName)
        {
            try
            {
                string content = Request.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(content))
                {
                    string[] userData = JsonConvert.DeserializeObject<string[]>(content);
                    IUserManager userManager = new ServerSideUserManager();

                    userManager.ChangeUserData(userName, userData);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Change an role's data.
        /// </summary>
        /// <param name="roleName">The role name</param>
        /// <param name="roleData">The role's data</param>
        /// <remarks>
        /// In the roleData array, the first entry is the role's display text
        /// </remarks>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ChangeRoleData/{roleName}")]
        public HttpResponseMessage ChangeRoleData(string roleName)
        {
            try
            {
                string content = Request.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(content))
                {
                    string[] roleData = JsonConvert.DeserializeObject<string[]>(content);

                    IUserManager userManager = new ServerSideUserManager();

                    userManager.ChangeRoleData(roleName, roleData);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="userName">The user name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("DeleteUser/{userName}")]
        public HttpResponseMessage DeleteUser(string userName)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                userManager.DeleteUser(userName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Add a new role
        /// </summary>
        /// <param name="roleName">The unique role name</param>
        /// <param name="roleData">Role's data</param>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("AddRole/{roleName}")]
        public HttpResponseMessage AddRole(string roleName)
        {
            try
            {
                string content = Request.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(content))
                {
                    string[] roleData = JsonConvert.DeserializeObject<string[]>(content);

                    IUserManager userManager = new ServerSideUserManager();

                    userManager.AddRole(roleName, roleData);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Delete a role.
        /// </summary>
        /// <param name="roleName">The role name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("DeleteRole/{roleName}")]
        public HttpResponseMessage DeleteRole(string roleName)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                userManager.DeleteRole(roleName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Add a mapping between an user and a role.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="roleName">The role name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("AddUserRoleMapping/{userName}/{roleName}")]
        public HttpResponseMessage AddUserRoleMapping(string userName, string roleName)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                userManager.AddUserRoleMapping(userName, roleName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Delete a mapping between an user and a role.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="roleName">The role name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("DeleteUserRoleMapping/{userName}/{roleName}")]
        public HttpResponseMessage DeleteUserRoleMapping(string userName, string roleName)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                userManager.DeleteUserRoleMapping(userName, roleName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Get user's emails.
        /// </summary>
        /// <param name="userName">The user name</param>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetUserEmails/{userName}")]
        public HttpResponseMessage GetUserEmails(string userName)
        {
            try
            {
                IUserManager userManager = new ServerSideUserManager();

                string[] emails = userManager.GetUserEmails(userName);

                return Request.CreateResponse(HttpStatusCode.OK, emails);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets the name of the CM super user.
        /// </summary>
        /// <returns>A string represents name of the CM super user</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetSuperUserName")]
        public HttpResponseMessage GetSuperUserName()
        {
            try
            {
                CMUserManager userManager = new CMUserManager();

                string name = userManager.GetSuperUserName();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(name))
                {
                    resp.Content = new StringContent(name, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Authenticate for the super user
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">password</param>
        /// <returns>true if it is the super user, false otherwise.</returns>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("AuthenticateSuperUser/{userName}/{password}")]
        public HttpResponseMessage AuthenticateSuperUser(string userName, string password)
        {
            try
            {
                CMUserManager userManager = new CMUserManager();

                bool status = userManager.AuthenticateSuperUser(userName, password);

                return Request.CreateResponse(HttpStatusCode.OK, status);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Change the super user's password.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="oldPassword">Old password</param>
        /// <param name="newPassword">New password</param>	
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("ChangeSuperUserPassword/{userName}/{oldPassword}/{newPassword}")]
        public HttpResponseMessage ChangeSuperUserPassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                CMUserManager userManager = new CMUserManager();

                userManager.ChangeSuperUserPassword(userName, oldPassword, newPassword);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }
    }
}