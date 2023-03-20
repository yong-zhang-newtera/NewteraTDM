/*
* @(#)SearchValueSet.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Text;

	/// <summary>
	/// The SearchValueSet class contains a set of SearchValue objects. It is for
	/// constructing IN condition in SQL WHERE clause
	/// </summary>
	/// <version>  	1.0.1 20 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	public class SearchValueSet : SQLElement
	{
		// private instance member
		private SQLElementCollection _searchValues;
		
		/// <summary> 
		/// Initiating a SearchValueSet
		/// </summary>
		public SearchValueSet():base()
		{
			_searchValues = new SQLElementCollection();
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			StringBuilder builder = new StringBuilder();
						
			// For empty set, put a single '' to prevent SQL error
			if (_searchValues.Count > 0)
			{
				for (int i = 0; i < _searchValues.Count; i++)
				{
					builder.Append(((SQLElement)_searchValues[i]).ToSQL());
					if (i < _searchValues.Count - 1)
					{
						builder.Append(", ");
					}
				}
			}
			else
			{
				builder.Append("''");
			}
			
			return builder.ToString();
		}
		
		/// <summary>
		/// Add a SearchValue object to the set.
		/// </summary>
		/// <param name="element">a SQLElement object to be added</param>
		public void Add(SearchValue searchValue)
		{
			_searchValues.Add(searchValue);
		}
	}
}