/*
* @(#) BigInteger.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Data;

	/// <summary>
	/// Represent a wrapper class for UInt64 integer.
	/// </summary>
	/// <version> 	1.0.1	20 Oct 2003 </version>
	/// <author> 	Yong Zhang </author>
	public class BigInteger
	{
		public static BigInteger Zero = new BigInteger(0);
		public static BigInteger One = new BigInteger(1);

		private ulong _value;

		/// <summary>
		/// Initiating an instance of BigInteger class.
		/// </summary>
		public BigInteger(ulong val)
		{
			_value = val;
		}

		/// <summary>
		/// Gets the length of bits of the big integer.
		/// </summary>
		public int BitLength
		{
			get
			{
				int bit = 0;
				ulong divider = 1;
				double result = _value / divider;

				while (result > 0)
				{
					divider = divider << 1;
					bit++;
					result = _value / divider;
				}

				return bit;
			}
		}

		/// <summary>
		/// Reset it to a big integer value.
		/// </summary>
		/// <param name="other">The big integer that provides reset value</param>
		/// <returns>This big integer</returns>
		public BigInteger Reset(BigInteger other)
		{
			this._value = other._value;
			return this;
		}

		/// <summary>
		/// Add a BigInteger object to this BigInteger.
		/// </summary>
		/// <param name="other">The other BigInteger object.</param>
		public BigInteger Add(BigInteger other)
		{
			this._value += other.Value;
			return this;
		}

		/// <summary>
		/// Shift the big integer to left for a number of bits.
		/// </summary>
		/// <param name="bits">The shifting bits</param>
		public BigInteger ShiftLeft(int bits)
		{
			_value = _value << bits;
			return this;
		}

		/// <summary>
		/// Gets the string representation of the id
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return System.Convert.ToString(_value);
		}

		/// <summary>
		/// Get the internal UInt64 value
		/// </summary>
		internal ulong Value
		{
			get
			{
				return _value;
			}
		}
	}
}