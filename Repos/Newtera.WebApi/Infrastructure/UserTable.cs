using System;
using System.Collections.Generic;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;

namespace Newtera.WebApi.Infrastructure
{
    /// <summary>
    /// Class that represents the Users table in the Newtera database
    /// </summary>
    public class UserTable<TUser>
        where TUser : ApplicationUser
    {
        private IUserManager _customUserManager;

        public UserTable(IUserManager customUserManager)
        {
            _customUserManager = customUserManager;
        }

        public string FindUserNameByCredential(string userName, string password)
        {
            if (_customUserManager.Authenticate(userName, password))
            {
                return userName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public TUser GetUserById(string userId)
        {
            UserRecord userInfo = _customUserManager.GetUserInfoByUserKey(userId);

            if (userInfo != null)
            {
                TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = userInfo.UserId;
                user.UserName = userInfo.UserName;
                user.DisplayedName = GetUserDisplayText(userInfo);
                user.PasswordHash = userInfo.Password;
                user.SecurityStamp = userInfo.SecurityStamp;
                user.Email = userInfo.Email;
                user.EmailConfirmed = true;
                user.PhoneNumber = userInfo.PhoneNumber;
                user.PhoneNumberConfirmed = true;
                user.Picture = userInfo.Picture;
                user.Division = userInfo.Division;
                user.Address = userInfo.Address;
                user.LockoutEnabled = false;
                user.LockoutEndDateUtc = DateTime.Now;
                user.AccessFailedCount = 0;
                user.FirstName = userInfo.FirstName;
                user.LastName = userInfo.LastName;

                return user;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a list of TUser instances in database
        /// </summary>
        /// <returns></returns>
        public List<TUser> GetUsers()
        {
            List<TUser> userInfos =  new List<TUser>();
            string[] userNames = _customUserManager.GetAllUsers();

            foreach (string userName in userNames)
            {
                UserRecord userInfo = _customUserManager.GetUserInfoByUserName(userName);

                if (userInfo != null)
                {
                    TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                    user.Id = userInfo.UserId;
                    user.UserName = userInfo.UserName;
                    user.DisplayedName = GetUserDisplayText(userInfo);
                    user.PasswordHash = userInfo.Password;
                    user.SecurityStamp = userInfo.SecurityStamp;
                    user.Email = userInfo.Email;
                    user.EmailConfirmed = true;
                    user.PhoneNumber = userInfo.PhoneNumber;
                    user.PhoneNumberConfirmed = true;
                    user.Picture = userInfo.Picture;
                    user.Division = userInfo.Division;
                    user.Address = userInfo.Address;
                    user.LockoutEnabled = false;
                    user.LockoutEndDateUtc = DateTime.Now;
                    user.AccessFailedCount = 0;
                    user.FirstName = userInfo.FirstName;
                    user.LastName = userInfo.LastName;

                    AddToSortedUserList(userInfos, user);
                }
            }

            return userInfos;
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns></returns>
        public List<TUser> GetUserByName(string userName)
        {
            UserRecord userInfo = _customUserManager.GetUserInfoByUserName(userName);

            if (userInfo != null)
            {
                TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = userInfo.UserId;
                user.UserName = userInfo.UserName;
                user.DisplayedName = GetUserDisplayText(userInfo);
                user.PasswordHash = userInfo.Password;
                user.SecurityStamp = userInfo.SecurityStamp;
                user.Email = userInfo.Email;
                user.EmailConfirmed = true;
                user.PhoneNumber = userInfo.PhoneNumber;
                user.PhoneNumberConfirmed = true;
                user.Picture = userInfo.Picture;
                user.Division = userInfo.Division;
                user.Address = userInfo.Address;
                user.LockoutEnabled = false;
                user.LockoutEndDateUtc = DateTime.Now;
                user.AccessFailedCount = 0;
                user.FirstName = userInfo.FirstName;
                user.LastName = userInfo.LastName;

                List<TUser> users = new List<TUser>();
                users.Add(user);

                return users;
            }
            else
            {
                return null;
            }
        }

        public List<TUser> GetUserByEmail(string email)
        {
            return null;
        }

        /// <summary>
        /// Return the user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {
            UserRecord userInfo = _customUserManager.GetUserInfoByUserKey(userId);

            if (userInfo != null)
            {
                return userInfo.Password;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the user's password hash
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string userId, string passwordHash)
        {
            UserRecord userInfo = _customUserManager.GetUserInfoByUserKey(userId);

            if (userInfo != null)
            {
                _customUserManager.ChangeUserPassword(userInfo.UserName, userInfo.Password, passwordHash);

                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the user's security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userId)
        {
            return null;
        }

        /// <summary>
        /// Inserts a new user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(TUser user)
        {
            string userName = user.UserName;
            string userPassword = user.PasswordHash;

            UserRecord userInfo = new UserRecord();
            userInfo.UserId = user.Id;
            userInfo.UserName = user.UserName;
            userInfo.LastName = user.LastName;
            userInfo.FirstName = user.FirstName;
            userInfo.Email = user.Email;
            userInfo.PhoneNumber = user.PhoneNumber;
            userInfo.Picture = user.Picture;
            userInfo.Division = user.Division;
            userInfo.Address = user.Address;
            userInfo.Password = user.PasswordHash;
            userInfo.SecurityStamp = user.SecurityStamp;
            _customUserManager.AddUser(userInfo);

            return 1;
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="userName">The user's id</param>
        /// <returns></returns>
        private int Delete(string userName)
        {
            _customUserManager.DeleteUser(userName);

            return 1;
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(TUser user)
        {
            return Delete(user.UserName);
        }

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(TUser user)
        {
            string userName = user.UserName;
            UserRecord userInfo = new UserRecord();
            userInfo.UserId = user.Id;
            userInfo.UserName = userName;
            userInfo.LastName = user.LastName;
            userInfo.FirstName = user.FirstName;
            userInfo.Email = user.Email;
            userInfo.PhoneNumber = user.PhoneNumber;
            userInfo.Picture = user.Picture;
            userInfo.Division = user.Division;
            userInfo.Address = user.Address;
            userInfo.SecurityStamp = user.SecurityStamp;

            _customUserManager.ChangeUserInfoByName(userName, userInfo);
            return 1;
        }

        private string GetUserDisplayText(string user, string[] userData)
        {
            string displayText;
            if (string.IsNullOrEmpty(userData[0]) &&
                string.IsNullOrEmpty(userData[1]))
            {
                displayText = user;
            }
            else
            {
                displayText = UsersListHandler.GetFormatedName(userData[0], userData[1]);
            }

            return displayText;
        }

        private string GetUserDisplayText(UserRecord userInfo)
        {
            string displayText;
            if (string.IsNullOrEmpty(userInfo.LastName) &&
                string.IsNullOrEmpty(userInfo.FirstName))
            {
                displayText = userInfo.UserName;
            }
            else
            {
                displayText = UsersListHandler.GetFormatedName(userInfo.LastName, userInfo.FirstName);
            }

            return displayText;
        }

        private string GetUserEmail(string user, string[] userData)
        {
            string email = null;
            if (!string.IsNullOrEmpty(userData[2]))
            {
                email = userData[2];
            }

            return email;
        }

        private string GetUserPhoneNumber(string user, string[] userData)
        {
            string phoneNumber = null;

            return phoneNumber;
        }

        private void AddToSortedUserList(List<TUser> userInfos, TUser userInfo)
        {
            int index = -1;

            for (int i = 0; i < userInfos.Count; i++)
            {
                if (string.Compare(userInfos[i].DisplayedName, userInfo.DisplayedName) > 0)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                // append to the end
                userInfos.Add(userInfo);
            }
            else
            {
                // insert at index
                userInfos.Insert(index, userInfo);
            }
        }
    }
}