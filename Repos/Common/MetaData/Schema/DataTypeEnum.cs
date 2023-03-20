/*
* @(#)DataTypeEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
    using System.Xml.Schema;

	/// <summary>
	/// Describes the types of data for attribute values
	/// </summary>
	public enum DataType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// Boolean
		/// </summary>
		Boolean,
		/// <summary>
		/// Byte
		/// </summary>
		Byte,
		/// <summary>
		/// Date
		/// </summary>
		Date,
		/// <summary>
		/// DateTime
		/// </summary>
		DateTime,
		/// <summary>
		/// Decimal
		/// </summary>
		Decimal,
		/// <summary>
		/// Double
		/// </summary>
		Double,
		/// <summary>
		/// Float
		/// </summary>
		Float,
		/// <summary>
		/// Integer
		/// </summary>
		Integer,
		/// <summary>
		/// BigInteger
		/// </summary>
		BigInteger,
		/// <summary>
		/// String
		/// </summary>
		String,
		/// <summary>
		/// Text for string more than 2000 characters
		/// </summary>
		Text
	}

	/// <summary>
	/// Converts data type from string to enum value or from enum value to string
	/// </summary>
	public class DataTypeConverter
	{
		/// <summary>
		/// Convert a type from a string to its corresponding enum type
		/// </summary>
		/// <param name="typeStr">the type string</param>
		/// <returns>
		/// One of the DataType values.
		/// </returns>
		public static DataType ConvertToTypeEnum(string typeStr)
		{
			switch (typeStr)
			{
				case "boolean":
					return DataType.Boolean;
				case "byte":
					return DataType.Byte;
				case "date":
					return DataType.Date;
				case "dateTime":
					return DataType.DateTime;
				case "decimal":
					return DataType.Decimal;
				case "double":
					return DataType.Double;
				case "float":
					return DataType.Float;
				case "integer":
					return DataType.Integer;
				case "bigInteger":
					return DataType.BigInteger;
				case "string":
					return DataType.String;
				case "text":
					return DataType.Text;
				default:
					return DataType.Unknown;
			}
		}

		/// <summary>
		/// Convert a type from an enum value to a string
		/// </summary>
		/// <param name="typeEnum">an enum type</param>
		/// <returns>
		/// a string representtaion.
		/// </returns>
		public static string ConvertToTypeString(DataType typeEnum)
		{
			switch (typeEnum)
			{
				case DataType.Boolean:
					return "boolean";
				case DataType.Byte:
					return "byte";
				case DataType.Date:
					return "date";
				case DataType.DateTime:
					return "dateTime";
				case DataType.Decimal:
					return "decimal";
				case DataType.Double:
					return "double";
				case DataType.Float:
					return "float";
				case DataType.Integer:
					return "integer";
				case DataType.BigInteger:
					return "bigInteger";
				case DataType.String:
					return "string";
				case DataType.Text:
					return "text";
				default:
					return "string";
			}
		}

		/// <summary>
		/// Convert a DataType enum to .Net system Type object
		/// </summary>
		/// <param name="typeEnum">an enum type</param>
		/// <returns>
		/// A Type object.
		/// </returns>
		public static Type ConvertToSystemType(DataType typeEnum)
		{
			switch (typeEnum)
			{
				case DataType.Boolean:
					return typeof(bool);
				case DataType.Byte:
					return typeof(byte);
				case DataType.Date:
					return typeof(DateTime);
				case DataType.DateTime:
					return typeof(DateTime);
				case DataType.Decimal:
					return typeof(decimal);
				case DataType.Double:
					return typeof(double);
				case DataType.Float:
					return typeof(float);
				case DataType.Integer:
					return typeof(int);
				case DataType.BigInteger:
					return typeof(long);
				case DataType.String:
					return typeof(string);
				case DataType.Text:
					return typeof(string);
				default:
					return typeof(string);
			}
		}

        public static string ConvertToXMLSchemaSimpleType(DataType dataType)
        {
            string schemaDataType;

            switch (dataType)
            {
                case DataType.String:
                    schemaDataType = "string";
                    break;
                case DataType.Text:
                    schemaDataType = "string";
                    break;
                case DataType.Boolean:
                    schemaDataType = "boolean";
                    break;
                case DataType.Byte:
                    schemaDataType = "short";
                    break;
                case DataType.Integer:
                    schemaDataType = "integer";
                    break;
                case DataType.BigInteger:
                    schemaDataType = "long";
                    break;
                case DataType.Double:
                    schemaDataType = "double";
                    break;
                case DataType.Float:
                    schemaDataType = "float";
                    break;
                case DataType.Decimal:
                    schemaDataType = "decimal";
                    break;
                case DataType.Date:
                    schemaDataType = "date";
                    break;
                case DataType.DateTime:
                    schemaDataType = "dateTime";
                    break;

                default:
                    schemaDataType = "string";
                    break;
            }

            return schemaDataType;
        }
	}
}