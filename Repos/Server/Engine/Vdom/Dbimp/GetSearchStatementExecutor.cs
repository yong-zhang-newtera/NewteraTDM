/*
* @(#)GetSearchStatementExecutor.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;

	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Logging;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Server.DB;
    using Newtera.Server.Logging;

	/// <summary>
	/// This class return a search statement without executing it.
	/// </summary>
	/// <version>  	1.0.0 18 Apr 2010 </version>
	public class GetSearchStatementExecutor : Executor
	{		
		/// <summary>
		/// Initiating an instance of GetSearchStatementExecutor class.
		/// </summary>
		/// <param name="metaData">the meta data model.</param>
		/// <param name="dataProvider">the database provider.</param>
		/// <param name="builder">the sql builder</param>
		/// <param name="entityTable">the hash table for associating xml elements with their entities.</param>
		public GetSearchStatementExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Hashtable entityTable) : base(metaData, dataProvider, builder)
		{
		}
		
		/// <summary>
		/// Building SQL statements,
		/// </summary>
		/// <param name="queryInfo">the information about a query</param>
        public SQLActionCollection Execute(QueryInfo queryInfo)
		{
            SQLActionCollection sqlActions = new SQLActionCollection();

			// generate a SELECT SQL statement
		    string sql = _builder.GenerateSelect(queryInfo);
			
			SQLPrettyPrint.printSql(sql);

            SQLAction sqlAction = new SQLAction();
            sqlAction.SQLTemplate = sql;
            sqlActions.Add(sqlAction);

            return sqlActions;
		}

		/// <summary>
		/// For debugging
		/// </summary>
		/// <param name="doc"></param>
		private void PrintXml(XmlDocument doc)
		{
			XmlTextWriter writer = new XmlTextWriter(System.Console.Out);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;

			doc.WriteTo(writer);
		}
	}
}