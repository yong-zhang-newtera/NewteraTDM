/*
* @(#)ModifySimpleAttributeAction.cs
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
	/// Modify a simple attribute to the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class ModifySimpleAttributeAction : MetaDataActionBase
	{
		private ModifyFlag _flag;

		/// <summary>
		/// Instantiate an instance of ModifySimpleAttributeAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public ModifySimpleAttributeAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider, ModifyFlag flag) : base(metaDataModel, element, dataProvider)
		{
			_flag = flag;
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public override MetaDataActionType ActionType
		{
			get
			{
				return MetaDataActionType.ModifySimpleAttribute;
			}
		}

		/// <summary>
		/// Prepare the action for modifying a simple attribute
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of modifying a column to a table
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			StringBuilder builder = new StringBuilder();

			builder.Append(generator.GetModifyColumnHeaderDDL(attribute.OwnerClass.TableName));

			builder.Append(generator.GetAddColumnDDL(attribute, _flag));

			builder.Append(generator.GetModifyColumnFooterDDL());

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