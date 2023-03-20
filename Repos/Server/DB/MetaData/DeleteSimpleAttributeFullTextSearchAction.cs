/*
* @(#)DeleteSimpleAttributeFullTextSearchAction.cs
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
	/// Delete the full text search index for an existing simple attribute.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteSimpleAttributeFullTextSearchAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteSimpleAttributeFullTextSearchAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteSimpleAttributeFullTextSearchAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteSimpleAttributeFullTextSearch;
			}
		}

		/// <summary>
		/// Prepare the action for deleting full text search index for a simple attribute.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting a full text index
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string indexName = DBNameComposer.GetIndexName(attribute.OwnerClass, attribute, true);
			
			string[] ddls = generator.GetDelFullTextIndexDDLs(indexName, attribute.OwnerClass.TableName, attribute.ColumnName);

			for (int i = 0; i < ddls.Length; i++)
			{
				cmd.CommandText = ddls[i];

				if (_log != null)
				{
					_log.Append(ddls[i], LogType.DDL);
				}

				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception)
				{
					// ignore the errors
				}
			}
		}
	}
}