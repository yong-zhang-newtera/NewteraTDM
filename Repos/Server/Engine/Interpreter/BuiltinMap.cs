/*
* @(#)BuiltinMap.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;
	
	/// <summary>
	/// This is a singleton class for the builtin functions (predefined functions).
	/// It is used by Symbol table to locate the FunctionDef objects of
	/// builtin functions.
	/// </summary>
	/// <version>  1.0 2003-1-26 </version>
	/// <author>    Russell Young </author>
	internal class BuiltinMap
	{
		internal static string[][] builtinArray = {
					 new string[]{"document", "Document"},
					 new string[]{"text", "TextFunction"},
					 new string[]{"setText", "SetTextFunction"},
					 new string[]{"decimal", "DecimalFunction"},
					 new string[]{"short", "ShortFunction"},
					 new string[]{"long", "LongFunction"},
					 new string[]{"integer", "IntegerFunction"},
					 new string[]{"float", "FloatFunction"},
					 new string[]{"double", "DoubleFunction"},
					 new string[]{"concat", "ConcatFunction"},
					 new string[]{"contains", "ContainsFunction"},
					 new string[]{"like", "LikeFunction"},
					 new string[]{"startsWith", "StartsWithFunction"},
					 new string[]{"endsWith", "EndsWithFunction"},
					 new string[]{"stringLength", "StringLengthFunction"},
					 new string[]{"upperCase", "UpperCaseFunction"},
					 new string[]{"lowerCase", "LowerCaseFunction"},
					 new string[]{"addInstance", "AddInstanceFunction"},
					 new string[]{"addInstances", "AddInstancesFunction"},
					 new string[]{"deleteInstance", "DeleteInstanceFunction"},
					 new string[]{"deleteInstances", "DeleteInstancesFunction"},
					 new string[]{"updateInstance", "UpdateInstanceFunction"},
					 new string[]{"updateInstances", "UpdateInstancesFunction"},
					 new string[]{"count", "CountFunction"},
					 new string[]{"avg", "AvgFunction"},
					 new string[]{"max", "MaxFunction"},
					 new string[]{"min", "MinFunction"},
					 new string[]{"sum", "SumFunction"},
                     new string[]{"distinct", "DistinctFunction"},
					 new string[]{"macro", "MacroFunction"},
					 new string[]{"getCurrentInstance", "CurrentInstanceFunction"},
                     new string[]{"currentUser", "GetCurrentUserFunction"},
                     new string[]{"currentRoles", "GetCurrentRolesFunction"},
                     new string[]{"currentUnits", "GetCurrentUnitsFunction"},
                     new string[]{"currentRolesAndUnits", "GetCurrentRolesAndUnitsFunction"},
                     new string[]{"average", "avg"},
                     new string[]{"error", "EmitErrorFunction"},
                     new string[]{"noop", "NoopFunction"},
                     new string[]{"wfstate", "GetWorkflowStateFunction"},
                     new string[]{"before", "GetAttributeValueFunction"},
                     new string[]{"aheadDays", "AheadDaysFunction"},
                     new string[]{"aheadHours", "AheadHoursFunction"},
                     new string[]{"behindDays", "BehindDaysFunction"},
                     new string[]{"behindHours", "BehindHoursFunction"},
                     new string[]{"currentSystemHour", "CurrentHourFunction"},
                };
		
		// The name space for the common builtin implementation
		private const string builtinNameSpace = "Newtera.Server.Engine.Interpreter.Builtin.";

		// The name space for the DB builtin implementation
		private const string dbBuiltinNameSpace = "Newtera.Server.Engine.Interpreter.Builtin.Db.";

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static BuiltinMap theBuiltinMap;
		private static Hashtable builtinTable;
		
		static BuiltinMap()
		{
			// create an hashtable for builtins for fast access.
			builtinTable = new Hashtable();
			for (int i = 0; i < builtinArray.Length; i++)
			{
				builtinTable[builtinArray[i][0]] = builtinArray[i][1];
			}

			theBuiltinMap = new BuiltinMap();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private BuiltinMap()
		{
		}

		/// <summary>
		/// Gets the DocumentFactory instance.
		/// </summary>
		/// <returns> The DocumentFactory instance.</returns>
		static public BuiltinMap Instance
		{
			get
			{
				return theBuiltinMap;
			}
		}

		/// <summary>
		/// Gets the information indicating whether a function of given name is a builtin.
		/// </summary>
		/// <param name="name">The function name</param>
		/// <returns>true if it is a builtin, false otherwise.</returns>
		public bool IsBuiltin(string name)
		{
			if (builtinTable[name] != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Create a FunctionDef object for the builtin function of the given name.
		/// Since a builtin function can have different implementations,
		/// depending on the auguments context, the FunctionDef object references to a specific
		/// implementation of the builtin function.
		/// </summary>
		/// <param name="interpreter">The interpreter</param>
		/// <param name="name">builtin function name</param>
		/// <param name="arguments">the invoking arguments of the function</param>
		/// <returns> The created FunctionDef object.</returns>
		public FunctionDef Create(Interpreter interpreter, string name, ExprCollection arguments)
		{
			String className = (string) builtinTable[name];

			if (className == null)
			{
				throw new InterpreterException(name + " is not a builtin function.");
			}

			// Create an DB specific implementation of a function if the following conditions
			// are all met:
			//
			// 1. The function has one or more arguments
			// 2. The first arguments is a ITraceable object
			// 3. The document returned by calling TraceDocument on the first argument is of DocumentDB type.
			// 4. There is an implementation in the dbBuiltinNameSpace existing for the function
			Type builtinImpClass = null;
			if (arguments.Count > 0 && arguments[0] is ITraceable)
			{
                Value val = ((ITraceable)arguments[0]).TraceDocument();
                if (val != null)
                {
                    VDocument doc = val.ToNode() as VDocument;
                    if (doc != null && doc.IsDB)
                    {
                        builtinImpClass = System.Type.GetType(dbBuiltinNameSpace + className);
                    }
                }
			}

			if (builtinImpClass == null)
			{
				// no db specific implementation found for the function
				// create a common implementation for it
				builtinImpClass = System.Type.GetType(builtinNameSpace + className);
				if (builtinImpClass == null)
				{
					throw new InterpreterException("Unable to find a builtin implementation class for " + builtinNameSpace + className);
				}
			}

			// create an instance of the builtin implementation class
			IFunctionImp imp = (IFunctionImp) Activator.CreateInstance(builtinImpClass);
			imp.Initialize(interpreter, name, arguments);

			// CheckArgs may throw an exception if there are invalid arguments or number
			// of arguments are mismatched
			imp.CheckArgs(arguments);

			return new FunctionDef(name, (IExpr) imp);
		}
	}
}