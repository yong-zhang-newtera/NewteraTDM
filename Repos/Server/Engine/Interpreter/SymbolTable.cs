/*
* @(#)SymbolTable.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// The single place to store symbols, such as variables, Constants, and functions,
	/// in a XQuery so that they can referenced quickly by the interpreter.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang </author>
	public class SymbolTable
	{
		private Interpreter _interpreter;
		private Hashtable _variables;
		private Hashtable _functions;
		
		/// <summary>
		/// Initiate an instance of SymbolTable object.
		/// </summary>
		public SymbolTable(Interpreter interpreter)
		{
			_interpreter = interpreter;
			_variables = new Hashtable();
			_functions = new Hashtable();
		}

		/// <summary>
		/// Add a SymbolTableEntry to the symbol table
		/// </summary>
		/// <param name="entry">The entry</param>
		public void Add(SymbolTableEntry entry)
		{
			if (entry.IsVariable)
			{
				_variables[entry.Name] = entry;
			}
			else if (entry.IsFunction)
			{
				_functions[entry.Name] = entry;
			}
		}

		/// <summary>
		/// Remove a SymbolTableEntry from the table
		/// </summary>
		/// <param name="entry">The entry</param>
		public void Remove(SymbolTableEntry entry)
		{
			if (entry.IsVariable)
			{
				_variables.Remove(entry.Name);
			}
			else if (entry.IsFunction)
			{
				_functions.Remove(entry.Name);
			}
		}

		/// <summary>
		/// Find a SymbolTableEntry given a symbol name
		/// </summary>
		/// <param name="name">The symbol name</param>
		/// <returns>The SymbolTableEntry found, null if not found</returns>
		public SymbolTableEntry Lookup(string name)
		{
			return (SymbolTableEntry) _variables[name];
		}

		/// <summary>
		/// Find a SymbolTableEntry given a symbol name and arguments
		/// </summary>
		/// <param name="name">The symbol name</param>
		/// <param name="arguments">Invoking arguments</param>
		/// <returns>The SymbolTableEntry found, null if not found</returns>
		/// <remarks>There are two type of functions, builtin functions and user-defined functions. The name conflicts are not allowed.</remarks>
		public SymbolTableEntry LookupFunction(string name, ExprCollection arguments)
		{
			// TODO, cache the FunctionDef for builtin object in symbol table

			// look up in builtin map first
			if (BuiltinMap.Instance.IsBuiltin(name))
			{
				return BuiltinMap.Instance.Create(_interpreter, name, arguments);
			}
			else
			{
				return (SymbolTableEntry) _functions[name];
			}
		}
	}
}