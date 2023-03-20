/*
* @(#)AlgorithmBase.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Algorithm
{
	using System;
	using System.Xml;
	using System.Data;
    using System.Text;

	/// <summary>
	/// A base class for other implementation classes of IAlgorithm
	/// </summary>
	/// <version> 1.0.0 28 Aug 2007</version>
    /// <remarks>Deprecated in 4.0.0</remarks>
	public abstract class AlgorithmBase
	{
        /// <summary>
        /// Add two values together.
        /// </summary>
        /// <param name="val1">The first value</param>
        /// <param name="val2">The second value</param>
        /// <param name="dataType">The data type</param>
        /// <returns>Result of addition in string.</returns>
        protected string AddValues(string val1, string val2, string dataType)
        {
            string result = "0";

            try
            {
                switch (dataType)
                {
                    case "Boolean":
                        result = "0"; // invalid for boolean
                        break;
                    case "Byte":
                        result = "0"; // invalid for byte
                        break;
                    case "Date":
                    case "DateTime":
                        result = "0"; // invalid for DateTime
                        break;
                    case "Decimal":
                        Decimal decimal1 = Decimal.Parse(val1);
                        Decimal decimal2 = Decimal.Parse(val2);
                        Decimal decimal3 = (decimal1 + decimal2);
                        result = decimal3.ToString();
                        break;
                    case "Double":
                        Double double1 = Double.Parse(val1);
                        Double double2 = Double.Parse(val2);
                        Double double3 = (double1 + double2);
                        result = double3.ToString();
                        break;
                    case "Float":
                        float float1 = float.Parse(val1);
                        float float2 = float.Parse(val2);
                        float float3 = (float1 + float2);
                        result = float3.ToString();
                        break;
                    case "Integer":
                        Int32 int1 = Int32.Parse(val1);
                        Int32 int2 = Int32.Parse(val2);
                        Int32 int3 = (int1 + int2);
                        result = int3.ToString();
                        break;
                    case "BigInteger":
                        Int64 bigInt1 = Int64.Parse(val1);
                        Int64 bigInt2 = Int64.Parse(val2);
                        Int64 bigInt3 = (bigInt1 + bigInt2);
                        result = bigInt3.ToString();
                        break;
                    case "String":
                        result = "0"; // invalid for String
                        break;
                    default:
                        result = "0"; // invalid by default
                        break;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// Divide value1 with value2.
        /// </summary>
        /// <param name="val1">The first value</param>
        /// <param name="val2">The second value</param>
        /// <param name="dataType">The data type</param>
        /// <returns>Result of division in string.</returns>
        protected string DivideValues(string val1, string val2, string dataType)
        {
            string result = "0";

            try
            {
                switch (dataType)
                {
                    case "Boolean":
                        result = "0"; // invalid for boolean
                        break;
                    case "Byte":
                        result = "0"; // invalid for byte
                        break;
                    case "Date":
                    case "DateTime":
                        result = "0"; // invalid for DateTime
                        break;
                    case "Decimal":
                        Decimal decimal1 = Decimal.Parse(val1);
                        Decimal decimal2 = Decimal.Parse(val2);
                        Decimal decimal3 = (decimal1 / decimal2);
                        result = decimal3.ToString();
                        break;
                    case "Double":
                        Double double1 = Double.Parse(val1);
                        Double double2 = Double.Parse(val2);
                        Double double3 = (double1 / double2);
                        result = double3.ToString();
                        break;
                    case "Float":
                        float float1 = float.Parse(val1);
                        float float2 = float.Parse(val2);
                        float float3 = (float1 / float2);
                        result = float3.ToString();
                        break;
                    case "Integer":
                        Int32 int1 = Int32.Parse(val1);
                        Int32 int2 = Int32.Parse(val2);
                        Int32 int3 = (int1 / int2);
                        result = int3.ToString();
                        break;
                    case "BigInteger":
                        Int64 bigInt1 = Int64.Parse(val1);
                        Int64 bigInt2 = Int64.Parse(val2);
                        Int64 bigInt3 = (bigInt1 / bigInt2);
                        result = bigInt3.ToString();
                        break;
                    case "String":
                        result = "0"; // invalid for String
                        break;
                    default:
                        result = "0"; // invalid by default
                        break;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// compare two values of the same data type.
        /// </summary>
        /// <param name="val1">The first value</param>
        /// <param name="val2">The second value</param>
        /// <param name="dataType">The data type</param>
        /// <returns>0 when two values are equal; -1 when the first value is less than the second value;
        /// 1 when the first value is greater than the second value.</returns>
        protected int CompareValues(string val1, string val2, string dataType)
        {
            int result = 0;

            try
            {
                switch (dataType)
                {
                    case "Boolean":
                        result = string.Compare(val1, val2);
                        break;
                    case "Byte":
                        byte byte1 = byte.Parse(val1);
                        byte byte2 = byte.Parse(val2);
                        if (byte1 < byte2)
                        {
                            result = -1;
                        }
                        else if (byte1 > byte2)
                        {
                            result = 1;
                        }
                        break;
                    case "Date":
                    case "DateTime":
                        DateTime datetime1 = DateTime.Parse(val1);
                        DateTime datetime2 = DateTime.Parse(val2);
                        if (datetime1 < datetime2)
                        {
                            result = -1;
                        }
                        else if (datetime1 > datetime2)
                        {
                            result = 1;
                        }
                        break;
                    case "Decimal":
                        Decimal decimal1 = Decimal.Parse(val1);
                        Decimal decimal2 = Decimal.Parse(val2);
                        if (decimal1 < decimal2)
                        {
                            result = -1;
                        }
                        else if (decimal1 > decimal2)
                        {
                            result = 1;
                        }
                        break;
                    case "Double":
                        Double double1 = Double.Parse(val1);
                        Double double2 = Double.Parse(val2);
                        if (double1 < double2)
                        {
                            result = -1;
                        }
                        else if (double1 > double2)
                        {
                            result = 1;
                        }
                        break;
                    case "Float":
                        float float1 = float.Parse(val1);
                        float float2 = float.Parse(val2);
                        if (float1 < float2)
                        {
                            result = -1;
                        }
                        else if (float1 > float2)
                        {
                            result = 1;
                        }
                        break;
                    case "Integer":
                        Int32 int1 = Int32.Parse(val1);
                        Int32 int2 = Int32.Parse(val2);
                        if (int1 < int2)
                        {
                            result = -1;
                        }
                        else if (int1 > int2)
                        {
                            result = 1;
                        }
                        break;
                    case "BigInteger":
                        Int64 bigInt1 = Int64.Parse(val1);
                        Int64 bigInt2 = Int64.Parse(val2);
                        if (bigInt1 < bigInt2)
                        {
                            result = -1;
                        }
                        else if (bigInt1 > bigInt2)
                        {
                            result = 1;
                        }
                        break;
                    case "String":
                        result = string.Compare(val1, val2);
                        break;
                    default:
                        result = string.Compare(val1, val2);
                        break;
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
	}
}