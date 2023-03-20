/*
* @(#)GetCurrentFunctionsFunction.cs
*
* Copyright (c) 2003-2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Xml;
    using System.Threading;
	using System.Collections;

	using Newtera.Common.MetaData.Principal;
    using Newtera.Server.UsrMgr;
 
	/// <summary>
	/// Gets units of the current loggined user.
	/// </summary>
	/// <version>  1.0.0 25 Feb 2008</version>
	public class GetCurrentUnitsFunction : FunctionImpBase
	{
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public GetCurrentUnitsFunction() : base()
		{
		}
		
		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 0)
			{
				throw new InterpreterException("GetCurrentFunctionsFunction expectes no argument, but got " + _arguments.Count);
			}			
		}

		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
            ValueCollection values = new ValueCollection();
            CustomPrincipal principal = null;

            principal = (CustomPrincipal)Thread.CurrentPrincipal;
            if (principal != null)
            {
                string userId = principal.Identity.Name;
                if (!string.IsNullOrEmpty(userId))
                {
                    IUserManager userManager = new ServerSideUserManager();
                    string[] units = userManager.GetRoles(userId, "Unit");
                    string displayText;
                    string[] roleData;
                    foreach (string unit in units)
                    {
                        roleData = userManager.GetRoleData(unit);
                        displayText = GetRoleText(unit, roleData);
                        values.Add(new XString(displayText));
                    }
                }
            }

            return new XCollection(values);
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