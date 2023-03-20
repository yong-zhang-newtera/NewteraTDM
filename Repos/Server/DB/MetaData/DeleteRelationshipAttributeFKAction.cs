/*
* @(#)DeleteRelationshipAttributeFKAction.cs
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
	/// Delete a relationship foreign key from the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteRelationshipAttributeFKAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteRelationshipAttributeFKAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteRelationshipAttributeFKAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteRelationshipAttributeFK;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a relationship foreign key from database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting a relationship foreign key from database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			RelationshipAttributeElement attribute = (RelationshipAttributeElement) SchemaModelElement;

			if (attribute.IsForeignKeyRequired) 
			{
				IDbCommand cmd = con.CreateCommand();
				IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

				string ddl = generator.GetDelColumnDDL(attribute.OwnerClass.TableName,
					attribute.ColumnName);

				cmd.CommandText = ddl;

				if (_log != null)
				{
					_log.Append(ddl, LogType.DDL);
				}

				cmd.ExecuteNonQuery();
			}
		}
	}
}