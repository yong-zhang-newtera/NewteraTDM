/*
* @(#)AddSimpleAttributeAutoIncrementAction.cs
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
	/// Add the auto increment to an existing simple attribute in the database.
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class AddSimpleAttributeAutoIncrementAction : MetaDataActionBase
	{
		private bool _createSequence = true;
		private bool _createTrigger = true;
		private int _sequenceStart = 1;

		/// <summary>
		/// Instantiate an instance of AddSimpleAttributeAutoIncrementAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddSimpleAttributeAutoIncrementAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider) : this(metaDataModel, element, dataProvider, true, true, 1)
		{
		}

		/// <summary>
		/// Instantiate an instance of AddSimpleAttributeAutoIncrementAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public AddSimpleAttributeAutoIncrementAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider, bool createSequence, bool createTrigger, int sequenceStart) : base(metaDataModel, element, dataProvider)
		{
			_createSequence = createSequence;
			_createTrigger = createTrigger;
			_sequenceStart = sequenceStart;
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public override MetaDataActionType ActionType
		{
			get
			{
				return MetaDataActionType.AddSimpleAttributeAutoIncrement;
			}
		}

		/// <summary>
		/// Prepare the action for adding auto increment status of a simple attribute in database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
		}

		/// <summary>
		/// Peform the action of adding auto increment status of a simple attribute in database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
			IDbCommand cmd = con.CreateCommand();
			SimpleAttributeElement attribute = (SimpleAttributeElement) SchemaModelElement;
			IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

			string sequenceName = DBNameComposer.GetSequenceName(attribute.OwnerClass, attribute);
			string triggerName = DBNameComposer.GetTriggerName(attribute.OwnerClass, attribute);
			// get an array of ddls for creating a sequence for an auto-increment attribute
			string[] ddls = generator.GetAddSequenceDDLs(_sequenceStart, sequenceName, triggerName, attribute.OwnerClass.TableName, attribute.ColumnName);

			string ddl;
			for (int i = 0; i < ddls.Length; i++)
			{
				if (!_createSequence && i == 0)
				{
					// do not create the sequnce
					continue;
				}

				if (!_createTrigger && i == 1)
				{
					// do not create the sequnce
					continue;
				}

				ddl = ddls[i];
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