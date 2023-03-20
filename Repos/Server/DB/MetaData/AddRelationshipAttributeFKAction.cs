/*
* @(#)AddRelationshipAttributeFKAction.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Text;
	using System.Data;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Add a foreign key column for a relationship attribute.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddRelationshipAttributeFKAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddRelationshipAttributeFKAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddRelationshipAttributeFKAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddRelationshipAttributeFK;
			}
		}

		/// <summary>
		/// Prepare the action for adding a relationship foreign key to database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans)
		{
		}

		/// <summary>
		/// Peform the action of adding a relationship foreign key to database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			RelationshipAttributeElement relationship = (RelationshipAttributeElement) SchemaModelElement;

			if (relationship.IsForeignKeyRequired)
			{
				IDbCommand cmd = con.CreateCommand();
				IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

				StringBuilder builder = new StringBuilder();

				builder.Append(generator.GetAddColumnHeaderDDL(relationship.OwnerClass.TableName));

				builder.Append(generator.GetAddFKColumnDDL(relationship));

				builder.Append(generator.GetAddColumnFooterDDL());

				string ddl = builder.ToString();
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