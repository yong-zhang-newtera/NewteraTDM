/*
* @(#)DataType.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represent a base class for data types available for the interpreter. It defines
	/// methods that describe the behavious of each type, the subtypes are supposed to
	/// override the methods.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	abstract public class DataType
	{
		/// <summary>
		/// Initiate an instance of Value object.
		/// </summary>
		public DataType()
		{
		}

		/// <summary>
		/// Gets the information indicating whether the type is a boolean type
		/// </summary>
		public abstract bool IsBoolean
		{
			get;
		}

		/// <summary>
		/// Gets the information indicating whether the type is a collection type
		/// </summary>
		public abstract bool IsCollection
		{
			get;
		}

		/// <summary>
		/// Gets the information indicating whether the type is a number type
		/// </summary>
		public abstract bool IsNumber
		{
			get;
		}

		/// <summary>
		/// Gets the information indicating whether the type is a string type
		/// </summary>
		public abstract bool IsString
		{
			get;
		}

		/// <summary>
		/// Gets the information indicating whether the type is a XmlNode type
		/// </summary>
		public abstract bool IsXmlNode
		{
			get;
		}

		/// <summary>
		/// Negate a value
		/// </summary>
		/// <param name="expr">The value for the operation</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Not(Value expr);

		/// <summary>
		/// Equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Eq(Value lval, Value rval);

		/// <summary>
		/// Less than operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Lt(Value lval, Value rval);

		/// <summary>
		/// Greater than operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Gt(Value lval, Value rval);

		/// <summary>
		/// Less than and equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Le(Value lval, Value rval);

		/// <summary>
		/// Greater than and equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Ge(Value lval, Value rval);

        /// <summary>
        /// Like operator on two values
        /// </summary>
        /// <param name="lval">The left side value</param>
        /// <param name="rval">The right side value</param>
        /// <returns>The resulting Value object</returns>
        public abstract Value Like(Value lval, Value rval);

		/// <summary>
		/// Not equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Ne(Value lval, Value rval);

		/// <summary>
		/// in operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value In(Value lval, Value rval);

		/// <summary>
		/// Not in operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Ni(Value lval, Value rval);

		/// <summary>
		/// add operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Add(Value lval, Value rval);

		/// <summary>
		/// substract operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Sub(Value lval, Value rval);

		/// <summary>
		/// And operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value And(Value lval, Value rval);

		/// <summary>
		/// Or operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Or(Value lval, Value rval);

		/// <summary>
		/// Times operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Times(Value lval, Value rval);

		/// <summary>
		/// Divide operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Divide(Value lval, Value rval);

		/// <summary>
		/// mod operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value Mod(Value lval, Value rval);

		/// <summary>
		/// unary minus operator on one value
		/// </summary>
		/// <param name="expr">The value</param>
		/// <returns>The resulting Value object</returns>
		public abstract Value UMinus(Value expr);
	}
}