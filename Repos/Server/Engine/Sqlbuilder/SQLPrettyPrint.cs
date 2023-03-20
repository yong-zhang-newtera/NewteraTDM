/*
* @(#)SQLPrettyPrint.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.IO;
	using System.Collections;

    using Newtera.Common.Core;

	/// <summary>
	/// Class to do pretty-printing of text:  SQL, All-static.
	/// </summary>
	/// <version>  	1.0.0 11 Aug 2003 </version>
	/// <author>  		Yong Zhang </author>
	public class SQLPrettyPrint
	{
		private const int UNKNOWN_SQL = - 1;
		
		private static readonly string[] _sqlTypes = new string[]{"SELECT", "CREATE", "GRANT", "DROP", "REVOKE", "UPDATE"};
		
		private static readonly string[] _emptyKeywords = new string[]{};
		
		private static readonly string[] _selectKeywords = new string[]{"SELECT", "FROM", "WHERE", "ORDER", "GROUP", "HAVING"};
		
		private static readonly string[] _createKeywords = new string[]{"FOR", "AS"};
		
		private static readonly string[] _grantKeywords = new string[]{"ON", "TO"};
		
		private static readonly string[] _revokeKeywords = new string[]{"ON", "FROM"};
		
		private static readonly string[] _updateKeywords = new string[]{"SET", "WHERE"};
		
		// Must match order of _sqlTypes
		private static readonly string[][] _majorKeywordArray = {_selectKeywords, _createKeywords, _grantKeywords, _emptyKeywords, _revokeKeywords, _updateKeywords};
		
		private static readonly string[] _selectMinors = new string[]{"AND", "OR"};
		
		private static readonly string[][] _minorKeywordArray = {_selectMinors, _emptyKeywords, _emptyKeywords, _emptyKeywords, _emptyKeywords, _emptyKeywords};
		
		// Prevent instantiation
		private SQLPrettyPrint()
		{
		}

        /// <summary>
        /// Pretty-prints a SQL statement.  No provisions for
        /// quoting.
        /// </summary>
        /// <param name="sql">SQL text </param>
        public static void printSql(string sql)
        {
            // TODO
            if (TraceLog.Instance.Enabled)
            {
     
                using (TextWriter writer = TraceLog.Instance.GetTextWriter())
                {
                    printSql(writer, sql);
                }
            }
        }
		
		/// <summary>
		/// Pretty-prints a SQL statement.  No provisions for
		/// quoting.
		/// </summary>
		/// <param name="writer">Send formatted SQL to this TextWriter </param>
		/// <param name="sql">SQL text </param>
		public static void  printSql(TextWriter writer, string sql)
		{
			int sqlType = GetSqlType(sql);
			
			string[] majorKeywords = GetSqlMajorKeywords(sqlType);
			string[] minorKeywords = GetSqlMinorKeywords(sqlType);
			
			PrintSql(writer, sql, majorKeywords, minorKeywords);
		}
		
		/// <summary>
		/// Print the contents of a Vector.  Calls toString()
		/// on each element.
		/// </summary>
		/// <param name="writer">Send output to this TextWriter.</param>
		/// <param name="v">A ArrayList.  Can be null.</param>
		public static void PrintVector(TextWriter writer, ArrayList v)
		{
			PrintVector(writer, v, ToStringCaller._methods);
		}
		
		/// <summary>
		/// Print the contents of a Vector.  An ObjectCaller
		/// is applied to each element.
		/// </summary>
		/// <param name="pw">Send output to this TextWriter.</param>
		/// <param name="v">A ArrayList.  Can be null.</param>
		/// <param name="oc">An ObjectCaller; the parameter will be
		/// an element from v.</param>
		public static void PrintVector(TextWriter writer, ArrayList v, ObjectCaller oc)
		{
			if (v == null)
			{
				writer.WriteLine("(null)");
			}
			else
			{
				PrintEnum(writer, v.GetEnumerator(), oc);
			}
		}
		
		/// <summary>
		/// Print the contents of a Hashtable.  Each key/value
		/// pair is printed as <EM>key = value</EM>.
		/// </summary>
		/// <param name="writer">Send output to this TextWriter.</param>
		/// <param name="ht">A Hashtable.  Can be null. </param>
		public static void PrintHashtable(TextWriter writer, Hashtable ht)
		{
			if (ht == null)
			{
				writer.WriteLine("(null)");
				return ;
			}
			
			IEnumerator keyElems = ht.Keys.GetEnumerator();
			writer.WriteLine("Key/value pairs: ");
			while (keyElems.MoveNext())
			{
				System.Object key = keyElems.Current;
				System.Object value_Renamed = ht[key];
				
				writer.WriteLine(key + " = " + value_Renamed.ToString());
			}
		}
		
		/// <summary>
		/// Print (and consume) an enumeration.  Applies an ObjectCaller
		/// to each element.
		/// </summary>
		/// <param name="writer">Send output to this TextWriter
		/// </param>
		/// <param name="elems">An Enumeration.  Can be null.</param>
		/// <param name="oc">An ObjectCaller; the parameter will be
		/// an element from elems.</param>
		public static void PrintEnum(TextWriter writer, IEnumerator elems, ObjectCaller oc)
		{
			int idx = 0;
			while (elems.MoveNext())
			{
				writer.Write("[" + idx++ + "] ");
				System.Object o = elems.Current;
				if (o == null)
				{
					writer.WriteLine("(null)");
				}
				else
				{
					writer.WriteLine((string) (oc.Call(o)));
				}
			}
		}
		
		/// <summary>
		/// Print class name and value.  Useful for Number subclasses.
		/// </summary>
		/// <param name="o">An Object.  Can be null.</param>
		/// <returns> o's class name and value, as a String.</returns>
		public static string ClassAndValue(System.Object o)
		{
			if (o == null)
			{
				return "(null)";
			}
			
			return o.GetType().FullName + ":" + o;
		}
		
		/// <summary>
		/// Figure out the type of SQL statement.  We compare against
		/// a table of SQL keywords, and return the index into that
		/// array.  If the keyword is unknown, we return a special
		/// code. 
		/// </summary>
		private static int GetSqlType(string sql)
		{
			int firstText;
			for (firstText = 0; firstText < sql.Length; firstText++)
			{
				if (!System.Char.IsWhiteSpace(sql[firstText]))
				{
					break;
				}
			}
			
			if (firstText >= sql.Length)
			{
				return UNKNOWN_SQL;
			}
			
			for (int idx = 0; idx < _sqlTypes.Length; idx++)
			{
				if (LeadingRegionMatches(sql, firstText, _sqlTypes[idx]))
				{
					return idx;
				}
			}
			
			return UNKNOWN_SQL;
		}
		
		/// <summary>
		/// Get the keywords that introduce major syntax elements
		/// for this type of SQL statements.  Example:  "SELECT"
		/// </summary>
		private static string[] GetSqlMajorKeywords(int sqlType)
		{
			if (sqlType == UNKNOWN_SQL)
			{
				return _emptyKeywords;
			}
			else
			{
				return _majorKeywordArray[sqlType];
			}
		}
		
		/// <summary>
		/// Get the minor keywords for this type of SQL.  Minor
		/// keywords separate elements of a clause.  Example: "OR".
		/// </summary>
		private static string[] GetSqlMinorKeywords(int sqlType)
		{
			if (sqlType == UNKNOWN_SQL)
			{
				return _emptyKeywords;
			}
			else
			{
				return _minorKeywordArray[sqlType];
			}
		}
		
		/*
		* Print sql to a PrintWriter given major and minor keywords.
		*/
		private static void PrintSql(TextWriter writer, string sql, string[] majorKeywords, string[] minorKeywords)
		{
			int sqlLen = sql.Length;
			
			bool wasComma = false;
			bool wasText = true; // Prevent newline for SELECT
			int sqlIdx;
			for (sqlIdx = 0; sqlIdx < sqlLen; sqlIdx++)
			{
				if (wasComma)
				{
					writer.Write("\n");
				}
				else if (!wasText)
				{
					if (IsSqlKeyword(sql, sqlIdx, majorKeywords))
					{
						writer.Write("\n");
					}
					else if (IsSqlKeyword(sql, sqlIdx, minorKeywords))
					{
						writer.Write("\n  ");
					}
				}
				
				char idxChar = sql[sqlIdx];
				
				writer.Write(idxChar);
				
				wasText = System.Char.IsLetter(idxChar) || (idxChar == '_');
				wasComma = (idxChar == ',');
			}
			
			writer.Write("\n");
			writer.Flush();
		}
		
		/*
		* Return whether text starting at textIdx is a SQL
		* keyword.
		*/
		private static bool IsSqlKeyword(string text, int textIdx, string[] keywords)
		{
			int i;
			
			for (i = 0; i < keywords.Length; i++)
			{
				if (LeadingRegionMatches(text, textIdx, keywords[i]))
				{
					return true;
				}
			}
			
			return false;
		}
		
		private static bool LeadingRegionMatches(string fullStr, int fullStrOffset, string matchStr)
		{
			return String.Compare(fullStr, fullStrOffset, matchStr, 0, matchStr.Length, true) == 0;
		}
	}
	
	
	/*
	* ObjectCaller that calls toString()
	*/
	class ToStringCaller : ObjectCaller
	{
		public virtual object Call(object o)
		{
			return o.ToString();
		}
		
		public static readonly ToStringCaller _methods = new ToStringCaller();
	}
}