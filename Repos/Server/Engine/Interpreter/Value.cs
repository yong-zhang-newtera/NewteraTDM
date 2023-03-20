/*
* @(#)Value.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// Represents primitive values of XQuery, including int, float, string, XmlNode, or IList,
	/// and so on.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	abstract public class Value
	{
		private DataType _type;

		/// <summary>
		/// Initiate an instance of Value object.
		/// </summary>
		protected Value(DataType type)
		{
			_type = type;
		}

		/// <summary>
		/// Gets the data type of the value
		/// </summary>
		public DataType DataType
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Get the value as an boolean value.
		/// </summary>
		/// <returns>The converted boolean</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract bool ToBoolean();

		/// <summary>
		/// Get the value as a ValueCollection.
		/// </summary>
		/// <returns>The converted collection</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract ValueCollection ToCollection();

		/// <summary>
		/// Get the value as a decimal value.
		/// </summary>
		/// <returns>The converted decimal</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract decimal ToDecimal();

		/// <summary>
		/// Get the value as a double value.
		/// </summary>
		/// <returns>The converted double</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract double ToDouble();

		/// <summary>
		/// Get the value as a float value.
		/// </summary>
		/// <returns>The converted boolean</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract float ToFloat();

		/// <summary>
		/// Get the value as an integer.
		/// </summary>
		/// <returns>The converted integer</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract int ToInt();

		/// <summary>
		/// Get the value as an XmlNode.
		/// </summary>
		/// <returns>The converted XmlNode</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public abstract XmlNode ToNode();

		/// <summary>
		/// Print the information about the value for debug purpose.
		/// </summary>
		public abstract void Print();
	}
}