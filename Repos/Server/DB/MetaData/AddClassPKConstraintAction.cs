/*
* @(#)AddClassPKConstraintAction.cs
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
	/// Add the user-defined primary key constraint to a class.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddClassPKConstraintAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of AddClassPKConstraintAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddClassPKConstraintAction(MetaDataModel metaDataModel,
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
				return MetaDataActionType.AddClassPKConstraint;
			}
		}

		/// <summary>
		/// Prepare the action for adding primary key constraint to a class.
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of adding primary key constraint to a class.
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			ClassElement classElement = (ClassElement) SchemaModelElement;

			// Create a DDL for creating a table for this class
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			StringBuilder builder = new StringBuilder();

			// Create primary key constraint for all primary keys
			if (classElement.PrimaryKeys.Count > 0)
			{
				builder = new StringBuilder();

				string constraintName = DBNameComposer.GetUniqueKeyName(classElement, null);
				builder.Append(generator.GetAddUniqueConstraintHeaderDDL(classElement.TableName, constraintName));

				int index = 0;
				foreach (SimpleAttributeElement pk in classElement.PrimaryKeys)
				{
					builder.Append(pk.ColumnName);

					if (index < classElement.PrimaryKeys.Count - 1)
					{
						builder.Append(", ");
					}

					index ++;
				}

				builder.Append(generator.GetAddUniqueConstraintFooterDDL());

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