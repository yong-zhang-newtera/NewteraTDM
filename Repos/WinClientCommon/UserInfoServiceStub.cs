using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WinClientCommon
{
    public class UserInfoServiceStub : WebApiServiceBase
    {
        public UserInfoServiceStub()
        {
        }

        public bool IsReadOnly()
        {
            bool status = false;

            string result = GetAPICall("api/userInfoService/IsReadOnly");

            try
            {
                status = bool.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }

        public bool Authenticate(string name, string password)
        {
            bool status = false;

            string result = GetAPICall("api/userInfoService/Authenticate/" + name + "/" + password);

            try
            {
                status = bool.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }

        public string[] GetRoles(string name)
        {
            string result = GetAPICall("api/userInfoService/GetRoles/" + name);

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string[] GetRoles(string name, string type)
        {
            string result = null;
            if (!string.IsNullOrEmpty(type))
            {
                result = GetAPICall("api/userInfoService/GetTypeRoles/" + name + "/" + type);
            }
            else
            {
                result = GetAPICall("api/userInfoService/GetRoles/" + name);
            }

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string[] GetUserData(string name)
        {
            string result = GetAPICall("api/userInfoService/GetUserData/" + name);

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string[] GetRoleData(string name)
        {
            string result = GetAPICall("api/userInfoService/GetRoleData/" + name);
            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string[] GetAllRoles()
        {
            string result = GetAPICall("api/userInfoService/GetAllRoles");

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string[] GetUsers(string role)
        {
            string result = GetAPICall("api/userInfoService/GetUsers/" + role);

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string[] GetAllUsers()
        {
            string result = GetAPICall("api/userInfoService/GetAllUsers");

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public void AddUser(string userName, string password, string[] userData)
        {
            string content = JsonConvert.SerializeObject(userData);

            PostAPICall("api/userInfoService/AddUser/" + userName + "/" + password, content, "application/json");
        }

        public void ChangeUserPassword(string userName, string oldPassword, string newPassword)
        {
            GetAPICall("api/userInfoService/ChangeUserPassword/" + userName + "/" + oldPassword + "/" + newPassword);
        }

        public void ChangeUserData(string userName, string[] userData)
        {
            string content = JsonConvert.SerializeObject(userData);

            PostAPICall("api/userInfoService/ChangeUserData/" + userName, content, "application/json");
        }

        public void ChangeRoleData(string roleName, string[] roleData)
        {
            string content = JsonConvert.SerializeObject(roleData);

            PostAPICall("api/userInfoService/ChangeRoleData/" + roleName, content, "application/json");
        }

        public void DeleteUser(string userName)
        {
            GetAPICall("api/userInfoService/DeleteUser/" + userName);
        }

        public void AddRole(string roleName, string[] roleData)
        {
            string content = JsonConvert.SerializeObject(roleData);

            PostAPICall("api/userInfoService/AddRole/" + roleName, content, "application/json");
        }

        public void DeleteRole(string roleName)
        {
            GetAPICall("api/userInfoService/DeleteRole/" + roleName);
        }

        public void AddUserRoleMapping(string userName, string roleName)
        {
            GetAPICall("api/userInfoService/AddUserRoleMapping/" + userName + "/" + roleName);
        }

        public void DeleteUserRoleMapping(string userName, string roleName)
        {
            GetAPICall("api/userInfoService/DeleteUserRoleMapping/" + userName + "/" + roleName);
        }

        public string[] GetUserEmails(string userName)
        {
            string result = GetAPICall("api/userInfoService/GetUserEmails/" + userName);

            string[] array = new string[] { };

            try
            {
                array = JsonConvert.DeserializeObject<string[]>(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return array;
        }

        public string GetSuperUserName()
        {
            return GetAPICall("api/userInfoService/GetSuperUserName");
        }

        public bool AuthenticateSuperUser(string userName, string password)
        {
            bool status = false;

            string result = GetAPICall("api/userInfoService/AuthenticateSuperUser/" + userName + "/" + password);

            try
            {
                status = bool.Parse(result);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return status;
        }

        public void ChangeSuperUserPassword(string userName, string oldPassword, string newPassword)
        {
            GetAPICall("api/userInfoService/ChangeSuperUserPassword/" + userName + "/" + oldPassword + "/" + newPassword);
        }


    }
}
