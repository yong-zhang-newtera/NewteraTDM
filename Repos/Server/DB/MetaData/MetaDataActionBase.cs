/*
* @(#)MetaDataActionBase.cs
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
	/// Represents a base class for all classes that implements IMetaDataAction interface.
	/// It provides common functionality
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public abstract class MetaDataActionBase : IMetaDataAction
	{
		private MetaDataModel _metaDataModel;
		private SchemaModelElement _schemaModelElement;
		protected IDataProvider _dataProvider;
		protected bool _skipDDLExecution = false;
		protected MetaDataUpdateLog _log;

		/// <summary>
		/// Instantiate an instance of MetaDataActionBase
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public MetaDataActionBase(MetaDataModel metaDataModel, SchemaModelElement element,
			IDataProvider dataProvider)
		{
			_metaDataModel = metaDataModel;
			_schemaModelElement = element;
			_dataProvider = dataProvider;
			_log = null;
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public abstract MetaDataActionType ActionType
		{
			get;
		}

		/// <summary>
		/// Gets or sets the MetaDataModel instance representing the meta data
		/// that the action is applying for.
		/// </summary>
		/// <value>A MetaDataModel instance</value>
		public MetaDataModel MetaDataModel
		{
			get
			{
				return _metaDataModel;
			}
		}

		/// <summary>
		/// Gets the Schema Model Element that is involved when
		/// performing the action.
		/// </summary>
		/// <value>A SchemaModelElement</value>
		public SchemaModelElement SchemaModelElement
		{
			get
			{
				return _schemaModelElement;
			}
		}

		/// <summary>
		/// Gets or sets the log to record DML, DDL, or error messages
		/// </summary>
		/// <value>A MetaDataUpdateLog</value>
		public MetaDataUpdateLog Log
		{
			get
			{
				return _log;
			}
			set
			{
				_log = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to skip the
		/// DDL execution for this action.
		/// </summary>
		/// <value>True to skip the DDL execution, false otherwise. Default is false.</value>
		/// <remarks>
		/// As part of adding or deleting a class, DDLs for adding or deleting columns
		/// are executed at the AddClassAction or DeleteClassAction, therefore,
		/// we don't have to execute the DDLs for adding or deleting columns
		/// individually. This flag is used to achieve this purpose.
		/// </remarks>
		public bool SkipDDLExecution
		{
			get
			{
				return _skipDDLExecution;
			}
			set
			{
				_skipDDLExecution = value;
			}
		}

		/// <summary>
		/// Prepare the action. This give the action a chance to prepare the action
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public abstract void Prepare(IDbConnection con, IDbTransaction trans);

		/// <summary>
		/// Perform the action This is where DDL(s) is executed against the database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public abstract void Do(IDbConnection con);

		/// <summary>
		/// Undo the action in case an error occures. It should is overrided by subclasses
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public virtual void UnDo(IDbConnection con)
		{
		}

		/// <summary>
		/// Commit the action. This is called after all actions are performed without errors.
		/// It should be overried by subclasses.
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public virtual void Commit(IDbConnection con)
		{
		}
	}
}