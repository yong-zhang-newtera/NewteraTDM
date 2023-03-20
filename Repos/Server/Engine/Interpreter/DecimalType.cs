/*
* @(#)DecimalType.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represent an integer type and define behavious of integer related operations.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class DecimalType : DataType
	{
		/// <summary>
		/// Initiate an instance of DecimalType object.
		/// </summary>
		public DecimalType()
		{
		}

		/// <summary>
		/// Gets the information indicating whether the type is a boolean type
		/// </summary>
		public override bool IsBoolean
		{
			get
			{
				return false;
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
				return true;
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
		/// <param name="val">The expression for the operation</param>
		/// <returns>The resulting Value object</returns>
		public override Value Not(Value val)
		{
			decimal data = val.ToDecimal();
			return new XBoolean(data == 0.0m ? false : true);
		}

		/// <summary>
		/// Equal operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Eq(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XBoolean(val1 == val2);
		}

		/// <summary>
		/// Less than operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Lt(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XBoolean(val1 < val2);
		}

		/// <summary>
		/// Greater than operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Gt(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XBoolean(val1 > val2);
		}

		/// <summary>
		/// Less than and equal operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Le(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XBoolean(val1 <= val2);
		}

		/// <summary>
		/// Greater than and equal operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ge(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XBoolean(val1 >= val2);
		}

        /// <summary>
        /// Like operator on two values
        /// </summary>
        /// <param name="lval">The left side value</param>
        /// <param name="rval">The right side value</param>
        /// <returns>The resulting Value object</returns>
        public override Value Like(Value lval, Value rval)
        {
            throw new InvalidOperationException("like operator is invalid for decimal type");
        }

		/// <summary>
		/// Not equal operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ne(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XBoolean(val1 != val2);
		}

		/// <summary>
		/// in operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value In(Value lval, Value rval)
		{
			if (!(rval is XCollection))
			{
				throw new InvalidOperationException("The right side value for in operator must be a XCollection");
			}
			decimal val1 = lval.ToDecimal();

			bool result = false;
			
			ValueCollection values = rval.ToCollection();
			foreach(Value val in values)
			{
				decimal data = val.ToDecimal();
				if (val1 == data)
				{
					result = true;
					break;
				}
			}

			return new XBoolean(result);
		}

		/// <summary>
		/// Not in operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ni(Value lval, Value rval)
		{
			if (!(rval is XCollection))
			{
				throw new InvalidOperationException("The right side value for in operator must be a ValueCollection");
			}
			decimal val1 = lval.ToDecimal();

			bool result = true;
			
			ValueCollection values = rval.ToCollection();
			foreach(Value val in values)
			{
				decimal data = val.ToDecimal();
				if (val1 == data)
				{
					result = false;
					break;
				}
			}

			return new XBoolean(result);
		}

		/// <summary>
		/// add operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Add(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XDecimal(val1 + val2);
		}

		/// <summary>
		/// substract operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Sub(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XDecimal(val1 - val2);
		}

		/// <summary>
		/// And operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value And(Value lval, Value rval)
		{
			bool bool1 = (lval.ToDecimal() == 0.0m? true : false);
			bool bool2 = (rval.ToDecimal() == 0.0m? true : false);

			return new XBoolean(bool1 && bool2);
		}

		/// <summary>
		/// Or operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Or(Value lval, Value rval)
		{
			bool bool1 = (lval.ToDecimal() == 0.0m? true : false);
			bool bool2 = (rval.ToDecimal() == 0.0m? true : false);

			return new XBoolean(bool1 || bool2);
		}

		/// <summary>
		/// Times operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Times(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XDecimal(val1 * val2);
		}

		/// <summary>
		/// Divide operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Divide(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XDecimal(val1 / val2);
		}

		/// <summary>
		/// mod operator on two values
		/// </summary>
		/// <param name="lval">The left side expression</param>
		/// <param name="rval">The right side expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value Mod(Value lval, Value rval)
		{
			decimal val1 = lval.ToDecimal();
			decimal val2 = rval.ToDecimal();

			return new XDecimal(val1 % val2);
		}

		/// <summary>
		/// unary minus operator on one expression
		/// </summary>
		/// <param name="val">The expression</param>
		/// <returns>The resulting Value object</returns>
		public override Value UMinus(Value val)
		{
			decimal data = val.ToDecimal();

			return new XDecimal(-data);
		}
	}
}