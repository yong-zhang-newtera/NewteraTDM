/*
* @(#)SymbolLookup.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	
	/// <summary>
	/// A SymbolLookup abstract class declares an interface for each concrete
	/// subclass which supplies special vendor specific symbols.
	/// </summary>
	/// <version>  	1.0.1 14 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	abstract public class SymbolLookup
	{
		/// <summary>
		/// Gets an avg function.
		/// </summary>
		/// <returns> a Avg function name</returns>
		abstract public string AvgFunc{get;}

		/// <summary>
		/// Gets a count function.
		/// </summary>
		/// <returns> a Count function name</returns>
		abstract public string CountFunc{get;}

        /// <summary>
        /// Gets a distinct keyword.
        /// </summary>
        /// <returns> a distinct keyword</returns>
        abstract public string DistinctFunc { get;}

		/// <summary>
		/// Gets a min function.
		/// </summary>
		/// <returns> a Min function name.</returns>
		abstract public string MinFunc{get;}

		/// <summary>
		/// Gets a max function
		/// </summary>
		/// <returns> a Max function name</returns>
		abstract public string MaxFunc{get;}

		/// <summary>
		/// Gets a sum function
		/// </summary>
		/// <returns> a Sum function name</returns>
		abstract public string SumFunc{get;}
		
		/// <summary>
		/// Gets a function expression that translate a date string to date
		/// </summary>
		/// <param name="dateStr">a date string</param>
		/// <param name="fmt">a date format string</param>
		/// <returns> a Date function representation</returns>
		abstract public string GetDateFunc(string dateStr, string fmt);
		
		/// <summary>
		/// Gets a function expression that translate a timestamp string to timestamp
		/// </summary>
		/// <param name="timestampStr">a timestamp string</param>
		/// <param name="fmt">a timestamp format string</param>
		/// <returns> a DateTime function representation</returns>
		abstract public string GetTimestampFunc(string timestampStr, string fmt);
		
		/// <summary>
		/// Gets a function expression that convert data to Upper case
		/// </summary>
		/// <param name="dataStr">a data string</param>
		/// <returns> a function to change the data string to upper case </returns>
		abstract public string GetUpperFunc(string dataStr);
		
		/// <summary>
		/// Get a function expression that convert data to Lower case
		/// </summary>
		/// <param name="dataStr">a data string</param>
		/// <returns> a function to change the data string to lower case</returns>
		abstract public string GetLowerFunc(string dataStr);
		
		/// <summary>
		/// Gets a function expression that translates a date column to string using specfified
		/// format
		/// </summary>
		/// <param name="dateCol">a date column data</param>
		/// <param name="fmt">a format string</param>
		/// <returns> a TO_CHAR function representation</returns>
		abstract public string GetCharFunc4Date(string dateCol, string fmt);

        /// <summary>
        /// Gets a negate function
        /// </summary>
        /// <returns> a negate function name</returns>
        abstract public string NegateFunc { get;}
		
		/// <summary>
		/// Gets a function expression that translates a date stamp column to string using specfified
		/// format.
		/// </summary>
		/// <param name="timestampCol">a timestamp column data</param>
		/// <param name="dateFmt">a format date string</param>
		/// <param name="timeFmt">a format time string</param>
		/// <returns> a TO_CHAR function representation</returns>
		abstract public string GetCharFunc4Timestamp(string timestampCol, string dateFmt, string timeFmt);
		
		/// <summary>
		/// Gets a function expression that translate boolean to string.
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="trueWord">the true word string</param>
		/// <param name="falseWord">the false word string</param>
		/// <returns> a DECODE function</returns>
		abstract public string GetDecodeFunc(string field, string trueWord, string falseWord);
		
		/// <summary>
		/// Gets a function expression that perform full-text search
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="value">the search value in expression</param>
		/// <param name="lable">the lable of the function</param>
		/// <returns> a contains function</returns>
		abstract public string GetContainsFunc(string field, string val, string label);

		/// <summary>
		/// Gets a function expression that perform bitwise AND operation
		/// </summary>
		/// <param name="field">a table column name</param>
		/// <param name="operand">the right operand of bitwise AND</param>
		/// <returns> a bitwise AND function</returns>
		abstract public string GetBitwiseAndFunc(string field, string operand);
		
		/// <summary>
		/// Gets a function expressing the score of full-text search result
		/// </summary>
		/// <param name="lable">the unique lable for a contains function</param>
		/// <returns> a score function</returns>
		abstract public string GetScoreFunc(string label);
		
		/// <summary>
		/// Gets the right outer join condition.
		/// </summary>
		/// <param name="leftTable">the left side of join table</param>
		/// <param name="rightTable">the right side of join table</param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition</param>
		/// <returns> a right outer join condition</returns>
		abstract public string GetRightOuterJoin(string leftTable, string rightTable, string leftOperand, string rightOperand);
		
		/// <summary>
		/// Gets the left outer join condition
		/// </summary>
		/// <param name="leftTable">the left side of join table</param>
		/// <param name="rightTable">the right side of join table </param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition</param>
		/// <returns> a left outer join condition</returns>
		abstract public string GetLeftOuterJoin(string leftTable, string rightTable, string leftOperand, string rightOperand);
		
		/// <summary>
		/// Gets the full outer join condition
		/// </summary>
		/// <param name="leftTable">the left side of join table</param>
		/// <param name="rightTable">the right side of join table</param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition</param>
		/// <returns> a full outer join condition</returns>
		abstract public string GetFullOuterJoin(string leftTable, string rightTable, string leftOperand, string rightOperand);

		/// <summary>
		/// Gets the inner join statement
		/// </summary>
		/// <param name="leftTable">the left side of join table</param>
		/// <param name="rightTable">the right side of join table </param>
		/// <param name="leftOperand">the left operand of an outer join condition</param>
		/// <param name="rightOperand">the right operand of an outer join condition</param>
		/// <returns> an inner join statement</returns>
		abstract public string GetInnerJoin(string leftTable, string rightTable, string leftOperand, string rightOperand);
	}
}