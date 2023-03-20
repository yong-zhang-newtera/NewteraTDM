/*
* @(#)AddSimpleAttributeFullTextSearchAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Data;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Add the full text search status of an existing simple attribute in the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddSimpleAttributeFullTextSearchAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddSimpleAttributeFullTextSearchAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddSimpleAttributeFullTextSearchAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider) : base(metaDataModel, element, dataProvider)
		{
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public override MetaDataActionType ActionType
		{
			get
			{
				return MetaDataActionType.AddSimpleAttributeFullTextSearch;
			}
		}

		/// <summary>
		/// Prepare the action for creating full text index for a simple attribute in database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of creating full text search index for a simple attribute in database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string indexName = DBNameComposer.GetIndexName(attribute.OwnerClass, attribute, true);
			
			string dataStore = attribute.FullTextInfo.DataStore;
			bool isFilter = attribute.FullTextInfo.IsFilter;

			string[] ddls = generator.GetAddFullTextIndexDDLs(indexName, attribute.OwnerClass.TableName, attribute.ColumnName, dataStore, isFilter);

			for (int i = 0; i < ddls.Length; i++)
			{
				cmd.CommandText = ddls[i];

				if (_log != null)
				{
					_log.Append(ddls[i], LogType.DDL);
				}

				cmd.ExecuteNonQuery();
			}
		}
	}
}