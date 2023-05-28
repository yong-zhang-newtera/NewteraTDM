/*
* @(#)MySqlSymbolLookup.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// A MySqlSymbolLookup subclass supplies special MySql specific symbols.
	/// </summary>
	public class MySqlSymbolLookup : SymbolLookup
	{
		/// <summary>
		/// Gets an avg function for MySql
		/// </summary>
		/// <value> a Avg function name</value>
		public override string AvgFunc
		{
			get
			{
				return "AVG";
			}
		}

		/// <summary>
		/// Gets a count function for MySql
		/// </summary>
		/// <value> a Count function name</value>
		public override string CountFunc
		{
			get
			{
				return "COUNT";
			}
		}

        /// <summary>
        /// Gets a distinct keyword.
        /// </summary>
        /// <returns> a distinct keyword</returns>
        public override string DistinctFunc
        {
            get
            {
                return "DISTINCT";
            }
        }

		/// <summary>
		/// Gets a min function for MySql
		/// </summary>
		/// <value> a Min function name </value>
		public override string MinFunc
		{
			get
			{
				return "MIN";
			}
		}

		/// <summary>
		/// Gets a max function for MySql 
		/// </summary>
		/// <value> a Max function name</value>
		public override string MaxFunc
		{
			get
			{
				return "MAX";
			}
		}

		/// <summary>
		/// Gets a sum function for MySql.
		/// </summary>
		/// <value> a Sum function name</value>
		public override string SumFunc
		{
			get
			{
				return "SUM";
			}
		}

        /// <summary>
        /// Gets a negate function
        /// </summary>
        /// <returns> a negate function name</returns>
        public override string NegateFunc
        {
            get
            {
                return "NOT";
            }
        }
		
		/// <summary>
		/// Gets a function expression that translate a date string to date for MySql
		/// </summary>
		/// <param name="dateStr">a date string</param>
		/// <param name="fmt">a date format string</param>
		/// <returns> an Oracle Date function representation</returns>
		public override string GetDateFunc(string dateStr, string fmt)
		{
			return "CONVERT('" + dateStr + "', DATE)";
		}
		
		/// <summary>
		/// Gets a function expression that translate a timestamp string to timestamp
		/// </summary>
		/// <param name="timestampStr">a timestamp string</param>
		/// <param name="fmt">a timestamp format string</param>
		/// <returns> an Oracle DateTime function representation</returns>
		public override string GetTimestampFunc(string timestampStr, string fmt)
		{
			/*
			* Since the ISO format yyyy-mm-ddThh:mm:ss is used for representing a
			* dateTime string, we have to replace the letter T with a space for the
			* reason that DB2 currently do not support ISO way of representing dateTime
			*/
			return "CONVERT('" + timestampStr + "', DATETIME)";
		}
		
		/// <summary>
		/// Gets a function expression that convert data to Upper case
		/// </summary>
		/// <param name="dataStr">a data string</param>
		/// <returns> an Oracle function to change the data string to upper case</returns>
		public override string GetUpperFunc(string dataStr)
		{
			return "UPPER(" + dataStr + ")";
		}
		
		/// <summary>
		/// Gets a function expression that convert data to Lower case
		/// </summary>
		/// <param name="dataStr">a data string</param>
		/// <returns> an Oracle function to change the data string to lower case </returns>
		public override string GetLowerFunc(string dataStr)
		{
			return "LOWER(" + dataStr + ")";
		}
		
		/// <summary>
		/// Gets a function expression that translates a date column to string using
		/// specfified format.
		/// </summary>
		/// <param name="dateCol">a date column data</param>
		/// <param name="fmt">a format string</param>
		/// <returns> an Oracle TO_CHAR function representation </returns>
		public override string GetCharFunc4Date(string dateCol, string fmt)
		{
			return "DATE_FORMAT(" + dateCol + ", '%Y%m%d')";
		}

		/// <summary>
		/// Get a function expression that translates a timestamp column to string using
		/// specfified format, if the timestamp value is null, no translation is done.
		/// </summary>
		/// <param name="timestampCol">a timestamp column data</param>
		/// <param name="dateFmt">a format date string</param>
		/// <param name="timeFmt">a format time string</param>
		/// <returns> an Oracle TO_CHAR function representation </returns>
		public override string GetCharFunc4Timestamp(string timestampCol, string dateFmt, string timeFmt)
		{
			return "DATE_FORMAT(" + timestampCol + ", '%Y-%m-%d %T')";
		}
		
		/// <summary>
		/// Gets a function expression that translate boolean to string
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="trueWord">the true word string</param>
		/// <param name="falseWord">the false word string</param>
		/// <returns> an Oracle DECODE function </returns>
		public override string GetDecodeFunc(string field, string trueWord, string falseWord)
		{
			return "CASE " + field + " WHEN 0 THEN '" + falseWord + "' WHEN 1 THEN '" + trueWord + "' WHEN NULL THEN NULL END";
		}
		
		/// <summary>
		/// Gets a function expression that perform full-text search.
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="value">the search value in expression</param>
		/// <param name="lable">the lable of the function</param>
		/// <returns> a contains function</returns>
		public override string GetContainsFunc(string field, string val, string label)
		{
			return $"INSTR({field}, '{val}') > 0";
		}

		/// <summary>
		/// Gets a function expression that perform bitwise AND operation
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="operand">the right operand of bitwise AND</param>
		/// <returns> a bitwise AND function</returns>
		public override string GetBitwiseAndFunc(string field, string operand)
		{
			return field + "&" + operand;
		}
		
		/// <summary>
		/// Gets a function expressing the score of full-text search result.
		/// </summary>
		/// <param name="lable">the unique lable for a contains function</param>
		/// <returns> a score function</returns>
		public override string GetScoreFunc(string label)
		{
			return " "; // TODO
		}
		
		/// <summary>
		/// Gets the right outer join condition
		/// </summary>
		/// <param name="leftTable">the left side of join table, ignored for MySql</param>
		/// <param name="rightTable">the right side of join table, ignored for MySql</param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition </param>
		/// <returns> a right outer join condition</returns>
		public override string GetRightOuterJoin(string leftTable, string rightTable, string leftOperand, string rightOperand)
		{
			return leftTable + " RIGHT OUTER JOIN " + rightTable + " ON " + leftOperand + " = " + rightOperand;
		}
		
		/// <summary>
		/// Gets the left outer join condition.
		/// </summary>
		/// <param name="leftTable">the left side of join table, ignored for MySql</param>
		/// <param name="rightTable">the right side of join table, ignored for MySql </param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition </param>
		/// <returns> a left outer join condition</returns>
		public override string GetLeftOuterJoin(string leftTable, string rightTable, string leftOperand, string rightOperand)
		{
			return leftTable + " LEFT OUTER JOIN " + rightTable + " ON " + leftOperand + " = " + rightOperand;
		}
		
		/// <summary>
		/// Gets the full outer join condition
		/// </summary>
		/// <param name="leftTable">the left side of join table, ignored for MySql</param>
		/// <param name="rightTable">the right side of join table, ignored for MySql </param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition </param>
		/// <returns> a full outer join condition</returns>
		public override string GetFullOuterJoin(string leftTable, string rightTable, string leftOperand, string rightOperand)
		{
			return leftTable + " FULL OUTER JOIN " + rightTable + " ON " + leftOperand + " = " + rightOperand;
		}

		/// <summary>
		/// Gets the inner join statement
		/// </summary>
		/// <param name="leftTable">the left side of join table, ignored for oracle</param>
		/// <param name="rightTable">the right side of join table, ignored for oracle </param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition </param>
		/// <returns> an inner join statement</returns>
		public override string GetInnerJoin(string leftTable, string rightTable, string leftOperand, string rightOperand)
		{
			return leftTable + " INNER JOIN " + rightTable + " ON " + leftOperand + " = " + rightOperand;
		}
	}
}