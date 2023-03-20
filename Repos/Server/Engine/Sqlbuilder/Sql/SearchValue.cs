/*
* @(#)SearchValue.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A SearchValue object represents a search value in a condition of WHERE clause.
	/// </summary>
	/// <version>  	1.0.0 12 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class SearchValue : SQLElement
	{
		// private instance members
		private string _value;
		private DataType _type;
		private CaseStyle _caseStyle;
		private SymbolLookup _lookup;
        private bool _isPattern;
		
		/// <summary>
		/// Initiating a SearchValue object
		/// </summary>
		/// <param name="value">the search value.</param>
		/// <param name="type">the type of the value</param>
		/// <param name="dataProvider">the database provider</param>
		public SearchValue(string val, DataType type, IDataProvider dataProvider) : base()
		{
			Initialize(val, type, CaseStyle.CaseSensitive, dataProvider);
		}
		
		/// <summary>
		/// Initiating a SearchValue object
		/// </summary>
		/// <param name="value">the search value.</param>
		/// <param name="type">the type of the value</param>
		/// <param name="caseStyle">the case style (upper or lower)</param>
		/// <param name="dataProvider">the database provider</param>
		public SearchValue(string val, DataType type, CaseStyle caseStyle, IDataProvider dataProvider) : base()
		{
			Initialize(val, type, caseStyle, dataProvider);
		}
		
		/// <summary>
		/// Initiating a SearchValue object
		/// </summary>
		/// <param name="value">the search value.</param>
		/// <param name="type">the type of the value.</param>
		/// <param name="caseStyle">the case style (upper or lower).</param>
		/// <param name="dataProvider">the database provider</param>
		private void Initialize(string val, DataType type, CaseStyle caseStyle, IDataProvider dataProvider)
		{
			_value = val;
			_caseStyle = caseStyle;
			_type = type;
            _isPattern = false;
			
			_lookup = SymbolLookupFactory.Instance.Create(dataProvider.DatabaseType);
		}

		/// <summary>
		/// Sets a search value string to the object
		/// </summary>
		/// <value>the search value string</value>
		public string Value
		{
            get
            {
                return _value;
            }
			set
			{
				_value = value;
			}
		}

        /// <summary>
        /// Gets or sets the info indicating whether the search value is used as a pattern
        /// for like operator.
        /// </summary>
        public bool IsPattern
        {
            get
            {
                return _isPattern;
            }
            set
            {
                _isPattern = value;
            }
        }
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			string val = _value;
			
			if (val == SQLElement.VALUE_NULL)
			{
				return "null";
			}
			
			switch (_type)
			{
				case DataType.String:
				case DataType.Text:
					if (val.IndexOf("'") != - 1)
					{
						val = AddSingleQuotes(val);
					}

                    if (val.IndexOf(@"\\") != -1)
                    {
                        val = UnescapeSlashChar(val);
                    }

                    if (_isPattern)
                    {
                        // search the values that contains the pattern defined in val, replace '*' with '%'
                        val = val.Replace('*', '%');
                        val = "'%" + val + "%'";
                    }
                    else
                    {
                        val = "'" + val + "'";
                    }
					
					// handle case style
					switch (_caseStyle)
					{
						case CaseStyle.CaseInsensitive:
							val = _lookup.GetUpperFunc(val);
							break;
						case CaseStyle.Upper:
							val = _lookup.GetUpperFunc(val);
							break;
						case CaseStyle.Lower:
							val = _lookup.GetLowerFunc(val);
							break;
						default:
							break;
					}
					
					break;
				
				case DataType.Date:
					int pos = val.IndexOf(" ");
					if (pos > 0)
					{
						// get rid of time portion
						val = val.Substring(0, pos);
					}
					val = _lookup.GetDateFunc(val, LocaleInfo.Instance.DateFormat);
					break;
				
				case DataType.DateTime:
                    System.DateTime timestamp;
                    try
                    {
                        timestamp = System.DateTime.Parse(val, Thread.CurrentThread.CurrentCulture);
                    }
                    catch (Exception)
                    {
                        // val is an invalid datetime string, use the current time instead
                        timestamp = DateTime.Now;
                    }
                    val = timestamp.ToString("s");
					val = _lookup.GetTimestampFunc(val, LocaleInfo.Instance.DateTimeFormat);
					break;
				
				case DataType.Boolean: 
					if (LocaleInfo.Instance.IsTrue(val))
					{
						val = "1";
					}
					else if (LocaleInfo.Instance.IsFalse(val))
					{
						val = "0";
					}
					else if (val == "")
					{
						val = "NULL";
					}
					else
					{
                        throw new Exception("Unknown boolean value " + val);
					}
					break;
				
				case DataType.Integer: 
				case DataType.BigInteger:
				case DataType.Float: 
				case DataType.Double: 
				case DataType.Decimal: 
					if (val == "")
					{
						val = "0";
					}
					break;
				
				default: 
					break;
				
			}
			
			return val;
		}
		
		/// <summary>
		/// Add an additional single quote for every quote in the input string
		/// </summary>
		private String AddSingleQuotes(String input)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '\'')
				{
					builder.Append("''");
				}
				else
				{
					builder.Append(input[i]);
				}
			}
			
			return builder.ToString();
		}

        /// <summary>
        /// Unescape the double slash chars with a signle char in the input string
        /// </summary>
        private String UnescapeSlashChar(String input)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            bool firstEncounter = true;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    if (firstEncounter)
                    {
                        builder.Append(@"\");
                        firstEncounter = false;
                    }
                    else
                    {
                        // ignore the second slash
                        firstEncounter = true;
                    }
                }
                else
                {
                    builder.Append(input[i]);
                    firstEncounter = true;
                }
            }

            return builder.ToString();
        }
	}
}