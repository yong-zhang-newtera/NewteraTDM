/*
* @(#)ClassIdField.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

    using Newtera.Common.Core;
	
	/// <summary>
	/// A ClassIdField represent CID column in a Table
	/// </summary>
	/// <version>  	1.0.1 01 Jun 2007 </version>
	public class ClassIdField:SQLElement
	{
		// private instance members
		private string _keyword;
        private string _tableAlias;
		
		/// <summary>
		/// Initiating a ClassIdField object.
		/// </summary>
		/// <param name="keyword">the name of distinct keyword</param>
		public ClassIdField(string tableAlias, string keyword) : base()
		{
            _tableAlias = tableAlias;
			_keyword = keyword;
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
            string sql;
            if (_keyword != null)
            {
                if (_tableAlias != null)
                {
                    sql = _keyword + " " + _tableAlias + ".CID";
                }
                else
                {
                    sql = _keyword + "CID";
                }
            }
            else
            {
                if (_tableAlias != null)
                {
                    sql = _tableAlias + ".CID";
                }
                else
                {
                    sql = "CID";
                }
            }

            return sql;
		}
	}
}