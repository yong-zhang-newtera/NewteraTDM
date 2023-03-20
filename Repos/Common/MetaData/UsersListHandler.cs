/*
* @(#)UsersListHandler.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.Threading;
	using System.Collections.Specialized;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// Represents an handler that retrieve a list of users from the user mananger
	/// </summary>
	/// <version> 1.0.0 19 Jan 2007 </version>
	public class UsersListHandler : IListHandler
	{
        public static string GetFormatedName(string lastName, string firstName)
        {
            string displayFormat = LocaleInfo.Instance.PersonNameFormat;

            if (!string.IsNullOrEmpty(lastName))
            {
                lastName = lastName.Trim();
            }
            else
            {
                lastName = "";
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                firstName = firstName.Trim();
            }
            else
            {
                firstName = "";
            }

            return string.Format(displayFormat, lastName, firstName);
        }

		/// <summary>
		/// Get a list of user enum values
		/// </summary>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
        /// <param name="parameter">The parameter value defined for the handler, could be null</param>
        /// <param name="filterValue">Any filter that is used to get a list, can be null</param>
        /// <param name="textField">The field that is used to get enum text, can be null</param>
        /// <param name="valueField">The field that is used to get enum value, can be null</param>
        /// <returns>A collection of EnumValue object</returns>
		public EnumValueCollection GetValues(SchemaModelElement context, string xquery, string parameter, string filterValue, string textField, string valueField)
		{
			EnumValueCollection values = new EnumValueCollection();
		
            // get the CustomPrincipal from the thread
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal != null)
            {
                StringCollection userNames = new StringCollection();

                IUserManager userManager = principal.UserManager;
                if (string.IsNullOrEmpty(parameter))
                {
                    // gets all users
                    string[]  users = userManager.GetAllUsers();
                    foreach (string user in users)
                    {
                        userNames.Add(user);
                    }
                }
                else
                {
                    // parameter represents role(s), using "|" for "or", and "&" for "and"
                    bool isAndExpression = false;
                    string[] roles = GetRoles(parameter, out isAndExpression);
                    string[] users;
                    if (isAndExpression)
                    {
                        // is and expression
                        int index = 0;
                        StringCollection commonUsers;
                        foreach (string role in roles)
                        {
                            commonUsers = new StringCollection();
                            users = userManager.GetUsers(role);
                            if (users != null)
                            {
                                foreach (string user in users)
                                {
                                    if (index == 0)
                                    {
                                        // first time, just add it to the list
                                        commonUsers.Add(user);
                                    }
                                    else if (userNames.Contains(user))
                                    {
                                        commonUsers.Add(user);
                                    }
                                }
                            }

                            userNames = commonUsers;

                            index++;
                        }
                    }
                    else
                    {
                        // is or expression
                        foreach (string role in roles)
                        {
                            users = userManager.GetUsers(role);
                            if (users != null)
                            {
                                foreach (string user in users)
                                {
                                    if (!userNames.Contains(user))
                                    {
                                        userNames.Add(user);
                                    }
                                }
                            }
                        }
                    }
                }

                EnumValue enumValue;
                string[] userData;
                string displayText;
                int pos = 0;
                foreach (string user in userNames)
                {
                    // get user display text
                    userData = userManager.GetUserData(user);
                    if (userData != null)
                    {
                        displayText = GetUserDisplayText(user, userData);
                    }
                    else
                    {
                        displayText = user;
                    }
                    enumValue = new EnumValue();
                    enumValue.Value = user;
                    enumValue.DisplayText = displayText;
                    // sorted based on display text
                    pos = 0;
                    foreach (EnumValue existing in values)
                    {
                        if (string.Compare( enumValue.DisplayText, existing.DisplayText) < 0)
                        {
                            break;
                        }

                        pos++;
                    }

                    if (pos < values.Count)
                    {
                        values.Insert(pos, enumValue); // insert
                    }
                    else
                    {
                        values.Add(enumValue); // append at the end
                    }
                }
            }

			return values;
		}

        /// <summary>
        /// Get a list of user enum values
        /// </summary>
        /// <param name="userManager">The user manager</param>
        /// <returns>A collection of EnumValue object</returns>
        public EnumValueCollection GetValues(IUserManager userManager)
        {
            EnumValueCollection values = new EnumValueCollection();

            // TODO, Get enum collection from cache to improve the performance, remember to clone the collection
            string[] users = userManager.GetAllUsers();
            if (users != null)
            {
                EnumValue enumValue;
                string[] userData;
                string displayText;
                int pos;

                foreach (string user in users)
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        // get user display text
                        userData = userManager.GetUserData(user);
                        displayText = GetUserDisplayText(user, userData);
                        enumValue = new EnumValue();
                        enumValue.Value = user;
                        enumValue.DisplayText = displayText;

                        // sorted based on display text
                        pos = 0;
                        foreach (EnumValue existing in values)
                        {
                            if (string.Compare(enumValue.DisplayText, existing.DisplayText) < 0)
                            {
                                break;
                            }

                            pos++;
                        }

                        if (pos < values.Count)
                        {
                            values.Insert(pos, enumValue); // insert
                        }
                        else
                        {
                            values.Add(enumValue); // append at the end
                        }
                    }
                }
            }

            return values;
        }

		/// <summary>
		/// Gets information indicating whether a given value name is valid
		/// </summary>
		/// <param name="val">The given data view name</param>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
		/// <returns>true if the value is valid, false, otherwise.</returns>
		public bool IsValueValid(string val, SchemaModelElement context)
		{
			return false;
		}

        private string GetUserDisplayText(string user, string[] userData)
        {
            string displayText;
            if (userData == null || string.IsNullOrEmpty(userData[0]) &&
                string.IsNullOrEmpty(userData[1]))
            {
                displayText = user;
            }
            else
            {
                displayText = GetFormatedName(userData[0], userData[1]);
            }

            return displayText;
        }

        /// <summary>
        /// Gets roles specified in parameter, the parameter value can be in the form of
        /// role(single role), role1 | role2 (role1 or role2)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="isAndExpression"></param>
        /// <returns>a string array for roles</returns>
        private string[] GetRoles(string parameter, out bool isAndExpression)
        {
            string[] roles;
            if (parameter.Contains("^"))
            {
                isAndExpression = true;
                roles = parameter.Split('^');
            }
            else
            {
                isAndExpression = false;
                roles = parameter.Split('|');
            }


            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i] != null)
                {
                    roles[i] = roles[i].Trim(); // get ride of spaces
                }
                else
                {
                    roles[i] = "";
                }
            }

            return roles;
        }
    }
}