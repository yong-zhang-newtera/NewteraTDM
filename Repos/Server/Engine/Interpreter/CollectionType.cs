/*
* @(#)CollectionType.cs
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
	/// Represent an XmlNode type and define behavious of XmlNode related operations.
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class CollectionType : DataType
	{
		/// <summary>
		/// Initiate an instance of CollectionType object.
		/// </summary>
		public CollectionType()
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
				return true;
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
			throw new InvalidOperationException("! operator is invalid for Collection type");
		}

		/// <summary>
		/// Equal operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Eq(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
			ValueCollection val2 = rval.ToCollection();
			
			// TODO, not sure if == operator has been overloaded at CollectionBae
			return new XBoolean(val1 == val2);
		}

		/// <summary>
		/// Less than operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Lt(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
			ValueCollection val2 = rval.ToCollection();
			
			// TODO, not sure if == operator has been overloaded at CollectionBae
			return new XBoolean(val1 < val2);
		}

		/// <summary>
		/// Greater than operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Gt(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
			ValueCollection val2 = rval.ToCollection();
			
			// TODO, not sure if == operator has been overloaded at CollectionBae
			return new XBoolean(val1 > val2);
		}

		/// <summary>
		/// Less than and equal operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Le(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
			ValueCollection val2 = rval.ToCollection();
			
			// TODO, not sure if == operator has been overloaded at CollectionBae
			return new XBoolean(val1 <= val2);
		}

		/// <summary>
		/// Greater than and equal operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ge(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
			ValueCollection val2 = rval.ToCollection();
			
			// TODO, not sure if == operator has been overloaded at CollectionBae
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
            throw new InvalidOperationException("like operator is invalid for collection type");
        }

		/// <summary>
		/// Not equal operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ne(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
			ValueCollection val2 = rval.ToCollection();

			// TODO, not sure if != operator has been overloaded at CollectionBae
			return new XBoolean(val1 != val2);
		}

		/// <summary>
		/// in operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value In(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
            if (val1.Count == 0)
            {
                // left operand is empty, therefore as result of in operator is false
                return new XBoolean(false);
            }
			else if (val1.Count == 1)
			{
				return val1[0].DataType.In(val1[0], rval);
			}
			else
			{
				throw new InvalidOperationException("The left operand of 'in' operator can not be a Collection");
			}
		}

		/// <summary>
		/// Not in operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Ni(Value lval, Value rval)
		{
			ValueCollection val1 = lval.ToCollection();
            if (val1.Count == 0)
            {
                // left operand is empty, therefore as result of not-in operator is true
                return new XBoolean(true);
            }
            else if (val1.Count == 1)
			{
				return val1[0].DataType.Ni(val1[0], rval);
			}
			else
			{
				throw new InvalidOperationException("The left operand of 'ni' operator can not be a Collection");
			}		
		}

		/// <summary>
		/// add operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Add(Value lval, Value rval)
		{
			throw new InvalidOperationException("+ operator is invalid for collection type");
		}

		/// <summary>
		/// substract operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Sub(Value lval, Value rval)
		{
			throw new InvalidOperationException("- operator is invalid for collection type");
		}

		/// <summary>
		/// And operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value And(Value lval, Value rval)
		{
            bool val1 = lval.ToBoolean();
            bool val2 = rval.ToBoolean();

            return new XBoolean(val1 && val2);
		}

		/// <summary>
		/// Or operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Or(Value lval, Value rval)
		{
            bool val1 = lval.ToBoolean();
            bool val2 = rval.ToBoolean();

            return new XBoolean(val1 || val2);
        }

		/// <summary>
		/// Times operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Times(Value lval, Value rval)
		{
			throw new InvalidOperationException("* operator is invalid for collection type");
		}

		/// <summary>
		/// Divide operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Divide(Value lval, Value rval)
		{
			throw new InvalidOperationException("div operator is invalid for collection type");
		}

		/// <summary>
		/// mod operator on two expressions
		/// </summary>
		/// <param name="lval">The left side value</param>
		/// <param name="rval">The right side value</param>
		/// <returns>The resulting Value object</returns>
		public override Value Mod(Value lval, Value rval)
		{
			throw new InvalidOperationException("% operator is invalid for collection type");
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