/*
*  @(#)MacroUserInfo.cs
*
*  Copyright (c) 2003 Newtera, Inc. All rights reserved.
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Threading;
	using System.Collections.Specialized;
	using Newtera.Common.MetaData.Principal;
	
	/// <summary>
	/// User Info related macro implementations
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class MacroUserInfo : IMacroDefinition
	{
		private const string USER_ID = "userid";
		private const string USER_ROLES = "userroles";

		private string _name = null;
		
		/// <summary>
		/// Initiate an instance of MacroUserInfo class.
		/// </summary>
		/// <param name="name">name of the macro</param>
		public MacroUserInfo(string name)
		{
			_name = name.ToLower();
		}

		/// <summary>
		/// Gets the result of the macro.
		/// </summary>
		/// <returns> the result of macro</returns>
		public object MacroResult
		{
			get
			{
				switch (_name)
				{
					case USER_ID:
						return UserId;
					case USER_ROLES:
						return UserRoles;
					default:
						throw new InterpreterException("Unknown macro name :"+ _name);
				}
			}
		}

		/// <summary>
		/// Gets the name of macro defination
		/// </summary>
		/// <value> name of macro</value>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary> 
		/// Gets the current user id
		/// </summary>
		/// <value> current user id</value>
		private string UserId
		{
			get
			{
				return Principal.Identity.Name;
			}
		}

		/// <summary> 
		/// Gets the current user roles
		/// </summary>
		/// <value> current user roles</value> 
		private string[] UserRoles
		{
			get
			{
				return Principal.Roles;
			}
		}

		/// <summary> 
		/// Gets the current user info object
		/// </summary>
		/// <value> current user info object</value>  
		private CustomPrincipal Principal
		{
			get
			{
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				
				if (principal == null)
				{
					throw new InterpreterException("The custom principal does not exist");
				}
				
				return principal;
			}
		}		
	}
}