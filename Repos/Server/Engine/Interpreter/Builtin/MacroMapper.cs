/*
*  @(#)MacroMapper.cs
*
*  Copyright (c) 2003 Newtera, Inc. All rights reserved.
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Collections;
	
	/// <summary>
	/// A sigleton class that mapps a macro name to its definition.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class MacroMapper
	{
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static MacroMapper theMacroMap;

		private Hashtable _macros;
		
		static MacroMapper()
		{
			theMacroMap = new MacroMapper();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private MacroMapper()
		{
			_macros = new Hashtable();
			_macros["userid"] = new MacroUserInfo("UserId");
			_macros["userroles"] = new MacroUserInfo("UserRoles");
		}

		/// <summary>
		/// Gets the MacroMapper instance.
		/// </summary>
		/// <returns> The MacroMapper instance.</returns>
		static public MacroMapper Instance
		{
			get
			{
				return theMacroMap;
			}
		}
		
		/// <summary>
		/// Gets a IMacroDefinition object given a name
		/// </summary>
		/// <param name="name">macro name</param>
		/// <returns> IMacroDefinition object, throw an exception if not found</returns>
		public IMacroDefinition GetMacroDefination(string name)
		{
			IMacroDefinition def = (IMacroDefinition) _macros[name.ToLower()];
			
			if (def == null)
			{
				throw new InterpreterException("Invalid macro name " + name);
			}
			
			return def;
		}
	}
}