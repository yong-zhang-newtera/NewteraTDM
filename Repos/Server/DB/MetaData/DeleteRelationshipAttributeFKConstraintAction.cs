/*
* @(#)DeleteRelationshipAttributeFKConstraintAction.cs
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
	/// Delete the foreign key constraint from a relationship attribute.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class DeleteRelationshipAttributeFKConstraintAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteRelationshipAttributeFKConstraintAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteRelationshipAttributeFKConstraintAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteRelationshipAttributeFKConstraint;
			}
		}

		/// <summary>
		/// Prepare the action for deleting FK constraint from a relationship attribute.
		/// </summary>
		/// <param name="dataProvider">The data provider for preparing the action</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting FK constraint from a relationship attribute.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			RelationshipAttributeElement relationship = (RelationshipAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string constraintName = DBNameComposer.GetFKConstraintName(relationship, false);
			string ddl = generator.GetDelFKConstraintDDL(constraintName,
				relationship.OwnerClass.TableName);

            if (!string.IsNullOrEmpty(ddl))
            {
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