/*
* @(#)DeleteSimpleAttributeDefaultValueAction.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
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
	/// Delete the default value constraint on a simple attribute in the database.
	/// </summary>
	/// <version> 1.0.0 17 Feb 2008 </version>
	public class DeleteSimpleAttributeDefaultValueAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteSimpleAttributeDefaultValueAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteSimpleAttributeDefaultValueAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.DeleteSimpleAttributeDefaultValue;
			}
		}

		/// <summary>
		/// Prepare the action for deleting unique status of a simple attribute in database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of deleting unique status of a simple attribute in database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string constraintName = DBNameComposer.GetDefaultValueConstraintName(attribute.OwnerClass, attribute);

            string ddl = generator.GetDelDefaultValueConstraintDDL(attribute.OwnerClass.TableName, constraintName);
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