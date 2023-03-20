/*
* @(#)BooleanType.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represent an boolean type and define behavious of integer related operations.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class BooleanType : DataType
	{
		/// <summary>
		/// Initiate an instance of BooleanType object.
		/// </summary>
		public BooleanType()
		{
		}

		/// <summary>
		/// Gets the information indicating whether the type is a boolean type
		/// </summary>
		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the type is a collection type
		/// </summary>
		public override bool IsCollection
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the type is a number type
		/// </summary>
		public override bool IsNumber
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the type is a string type
		/// </summary>
		public override bool IsString
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the type is a XmlNode type
		/// </summary>
		public override bool IsXmlNode
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Negate a value
		/// </summary>
		/// <param name="val">The value for the operation</param>
		/// <returns>The resulting Value object</returns>
		public override Value Not(Value val)
		{
			return new XBoolean(!val.ToBoolean());
		}

		/// <summary>
		/// Equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Eq(Value lval, Value rval)
		{
			bool val1 = lval.ToBoolean();
			bool val2 =  rval.ToBoolean();

			return new XBoolean(val1 == val2);
		}

		/// <summary>
		/// Less than operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Lt(Value lval, Value rval)
		{
			throw new InvalidOperationException("< operator is invalid for boolean type");
		}

		/// <summary>
		/// Greater than operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Gt(Value lval, Value rval)
		{
			throw new InvalidOperationException("> operator is invalid for boolean type");
		}

		/// <summary>
		/// Less than and equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Le(Value lval, Value rval)
		{
			throw new InvalidOperationException("<= operator is invalid for boolean type");
		}

		/// <summary>
		/// Greater than and equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ge(Value lval, Value rval)
		{
			throw new InvalidOperationException(">= operator is invalid for boolean type");
		}

        /// <summary>
        /// Like operator on two values
        /// </summary>
        /// <param name="lval">The left side value</param>
        /// <param name="rval">The right side value</param>
        /// <returns>The resulting Value object</returns>
        public override Value Like(Value lval, Value rval)
        {
            throw new InvalidOperationException("like operator is invalid for boolean type");
        }

		/// <summary>
		/// Not equal operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ne(Value lval, Value rval)
		{
			bool val1 = lval.ToBoolean();
			bool val2 =  rval.ToBoolean();

			return new XBoolean(val1 != val2);
		}

		/// <summary>
		/// in operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value In(Value lval, Value rval)
		{
			throw new InvalidOperationException("In operator is invalid for boolean type");
		}

		/// <summary>
		/// Not in operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ni(Value lval, Value rval)
		{
			throw new InvalidOperationException("Not In operator is invalid for boolean type");
		}

		/// <summary>
		/// add operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Add(Value lval, Value rval)
		{
			throw new InvalidOperationException("+ operator is invalid for boolean type");
		}

		/// <summary>
		/// substract operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Sub(Value lval, Value rval)
		{
			throw new InvalidOperationException("- operator is invalid for boolean type");
		}

		/// <summary>
		/// And operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value And(Value lval, Value rval)
		{
			bool val1 = lval.ToBoolean();
			bool val2 =  rval.ToBoolean();

			return new XBoolean(val1 && val2);
		}

		/// <summary>
		/// Or operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Or(Value lval, Value rval)
		{
			bool val1 = lval.ToBoolean();
			bool val2 =  rval.ToBoolean();

			return new XBoolean(val1 || val2);
		}

		/// <summary>
		/// Times operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Times(Value lval, Value rval)
		{
			throw new InvalidOperationException("* operator is invalid for boolean type");
		}

		/// <summary>
		/// Divide operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Divide(Value lval, Value rval)
		{
			throw new InvalidOperationException("div operator is invalid for boolean type");
		}

		/// <summary>
		/// mod operator on two values
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Mod(Value lval, Value rval)
		{
			throw new InvalidOperationException("% operator is invalid for boolean type");
		}

		/// <summary>
		/// unary minus operator on one value
		/// </summary>
		/// <param name="val">The value</param>
		/// <returns>The resulting Value object</returns>
		public override Value UMinus(Value val)
		{
			throw new InvalidOperationException("- operator is invalid for boolean type");
		}
	}
}