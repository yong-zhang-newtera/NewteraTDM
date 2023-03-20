/*
* @(#)And.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Axis is a helper class used to encode and decode the path segment
	/// axis. It encodes the axis type as an int.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author> 
	public class Axis
	{
		
		private string type;
		private int code;
		
		/// constants used internally to encode the axis types.
		public const int DOCUMENT = - 1;
		public const int RELATIVE = 0;
		public const int CHILD = 1;
		public const int DESCENDANT = 2;
		public const int PARENT = 3;
		public const int ANCESTOR = 4;
		public const int FOLLOWINGSIBLING = 5;
		public const int PRECEDINGSIBLING = 6;
		public const int FOLLOWING = 7;
		public const int PRECEDING = 8;
		public const int ATTRIBUTE = 9;
		public const int NAMESPACE = 10;
		public const int SELF = 11;
		public const int ANCESTORORSELF = 12;
		public const int DEREFERENCE = 13;
		public const int FUNCTION = 14;
		
		private static readonly ArrayList types;
		
		/// <summary>
		/// Sets up the static environment
		/// </summary>
		/// <returns> ArrayList of Axis to use for lookups.</returns>
		static private ArrayList Setup()
		{
			ArrayList ret = new ArrayList(18);
			
			ret.Add(new Axis("", RELATIVE));
			ret.Add(new Axis("/", CHILD));
			ret.Add(new Axis("//", DESCENDANT));
			ret.Add(new Axis("=>", DEREFERENCE));	
			ret.Add(new Axis("/self::", SELF));
			ret.Add(new Axis("/@", ATTRIBUTE));
			ret.Add(new Axis("/child::", CHILD));
			ret.Add(new Axis("/descendant::", DESCENDANT));
			ret.Add(new Axis("/parent::", PARENT));
			ret.Add(new Axis("/ancestor::", ANCESTOR));
			ret.Add(new Axis("/following-sibling::", FOLLOWINGSIBLING));
			ret.Add(new Axis("/preceding-sibling::", PRECEDINGSIBLING));
			ret.Add(new Axis("/following::", FOLLOWING));
			ret.Add(new Axis("/preceding::", PRECEDING));
			ret.Add(new Axis("/attribute::", ATTRIBUTE));
			ret.Add(new Axis("/namespace::", NAMESPACE));
			ret.Add(new Axis("/function::", FUNCTION));
			ret.Add(new Axis("/ancestor-or-self::", ANCESTORORSELF));
			return ret;
		}
		
		/// <summary>
		/// builds a single entry in the lookup table
		/// </summary>
		/// <param name="type">giving the axis type</param>
		/// <param name="code">giving the integer code for this type.</param>
		private Axis(string type, int code)
		{
			this.type = type;
			this.code = code;
		}
		
		/// <summary>
		///  Translates from a String giving the axis to the internal code.
		/// </summary>
		/// <param name="axis">giving axis </param>
		/// <returns> the code representing the axis </returns>
		/// <exception cref="InterpreterException">if the axis is not recognized
		/// </exception>
		static public int GetCode(string axis)
		{
			int i;
			
			for (i = 0; i < types.Count; i++)
			{
				if (axis.Equals(((Axis) types[i]).type))
				{
					return ((Axis) types[i]).code;
				}
			}

			// Probably should not ever be called (the parser filters bad axes out)
			throw new InterpreterException("Unrecognized axis type \"" + axis + "\"");
		}
		
		/// <summary>
		/// Translates from a code giving the axis to the string
		/// representation. If it is a bad code no error is thrown - this
		/// should only be called by internal functions, so it should never
		/// get a bad value.
		/// </summary>
		/// <param name="code">	giving axis code. </param>
		/// <returns> String representation of axis. </returns>
		static public string GetType(int code)
		{
			for (int i = 0; i < types.Count; i++)
			{
				if (code == ((Axis) types[i]).code)
				{
					return ((Axis) types[i]).type;
				}
			}
			return "UNKNOWN CODE " + code;
		}

		static Axis()
		{
			types = Setup();
		}
	}
}