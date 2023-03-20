/*
* @(#)RolesListHandler.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.Threading;
	using System.Collections.Specialized;

    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// Represents an handler that retrieve a list of roles from the user mananger
	/// </summary>
	/// <version> 1.0.0 19 Jan 2007 </version>
	public class RolesListHandler : IListHandler
	{
        private const string MyRoles = "MyRoles";
        private const string MyFunctions = "MyFunctions";
        private const string MyUnits = "MyUnits";

		/// <summary>
		/// Get a list of data view names
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
		
			if (context != null)
			{
                // get the CustomPrincipal from the thread
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

                if (principal != null)
                {
                    string[] roles = null;
                    IUserManager userManager = principal.UserManager;
                    if (string.IsNullOrEmpty(parameter))
                    {
                        roles = userManager.GetAllRoles();
                    }
                    else if (parameter == MyRoles)
                    {
                        // Get user's roles
                        roles = userManager.GetRoles(principal.Identity.Name);
                    }
                    else if (parameter == MyUnits)
                    {
                        // Get user's roles
                        roles = userManager.GetRoles(principal.Identity.Name, "Unit");
                    }
                    else if (parameter == MyFunctions)
                    {
                        // Get user's roles
                        roles = userManager.GetRoles(principal.Identity.Name, "Function");
                    }

                    if (roles != null)
                    {
                        EnumValue enumValue;
                        string displayText;
                        string[] roleData;
                        foreach (string role in roles)
                        {
                            roleData = userManager.GetRoleData(role);
                            displayText = GetRoleText(role, roleData);
                            enumValue = new EnumValue();
                            enumValue.Value = role;
                            enumValue.DisplayText = displayText;
                            values.Add(enumValue);
                        }
                    }
                }
			}

			return values;
		}

        /// <summary>
        /// Get a list of uroleser enum values
        /// </summary>
        /// <param name="userManager">The user manager</param>
        /// <returns>A collection of EnumValue object</returns>
        public EnumValueCollection GetValues(IUserManager userManager)
        {
            EnumValueCollection values = new EnumValueCollection();

            string[] roles = userManager.GetAllRoles();
            if (roles != null)
            {
                EnumValue enumValue;
                string[] roleData;
                string displayText;
                int pos;

                foreach (string role in roles)
                {
                    if (!string.IsNullOrEmpty(role))
                    {
                        // get role display text
                        roleData = userManager.GetRoleData(role);
                        displayText = GetRoleText(role, roleData);
                        enumValue = new EnumValue();
                        enumValue.Value = role;
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
		/// Gets information indicating whether a given data view name is valid
		/// </summary>
		/// <param name="val">The given data view name</param>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
		/// <returns>true if the value is valid, false, otherwise.</returns>
		public bool IsValueValid(string val, SchemaModelElement context)
		{
			return false;
		}

        private string GetRoleText(string role, string[] roleData)
        {
            string displayText;
            if (string.IsNullOrEmpty(roleData[0]))
            {
                displayText = role;
            }
            else
            {
                displayText = roleData[0];
            }

            return displayText;
        }
	}
}