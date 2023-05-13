/*
* @(#)IMetaDataAction.cs
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
	/// Represents an interface for actions applied to the meta data stored in database
	/// </summary>
	/// <version> 1.0.0 17 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public interface IMetaDataAction
	{
		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		MetaDataActionType ActionType
		{
			get;
		}

		/// <summary>
		/// Gets the MetaDataModel instance representing the meta data
		/// that the action is applying for.
		/// </summary>
		/// <value>A MetaDataModel instance</value>
		MetaDataModel MetaDataModel
		{
			get;
		}

		/// <summary>
		/// Gets or sets the Schema Model Element that is involved when
		/// performing the action.
		/// </summary>
		/// <value>A SchemaModelElement</value>
		SchemaModelElement SchemaModelElement
		{
			get;
		}

		/// <summary>
		/// Gets or sets the log to record DML, DDL, or error messages
		/// </summary>
		/// <value>A MetaDataUpdateLog</value>
		MetaDataUpdateLog Log
		{
			get;
			set;
		}

		/// <summary>
		/// Prepare the action. This give the action a chance to prepare the action
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		void Prepare(IDbConnection con, IDbTransaction trans);

		/// <summary>
		/// Perform the action. This is where DDL(s) is executed against the database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		void Do(IDbConnection con);

		/// <summary>
		/// Undo the action in case an error occures
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		void UnDo(IDbConnection con);

		/// <summary>
		/// Commit the action. This is called after all actions are executed without errors
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		void Commit(IDbConnection con);

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
		bool SkipDDLExecution { get; set;}
	}

	/// <summary>
	/// Specify the types of available actions that can be applied to meta data
	/// </summary>
	public enum MetaDataActionType
	{
		Unknown,
		AddSchema,
		DeleteSchema,
		AddClass,
		DeleteClass,
		AddClassInheritance,
		DeleteClassInheritance,
		AddClassPKConstraint,
		DeleteClassPKConstraint,
		AddSimpleAttribute,
		DeleteSimpleAttribute,
		AddRelationshipAttribute,
		DeleteRelationshipAttribute,
		AddArrayAttribute,
		DeleteArrayAttribute,
        AddImageAttribute,
        DeleteImageAttribute,
		AddRelationshipAttributeFK,
		DeleteRelationshipAttributeFK,
		AddRelationshipAttributeFKConstraint,
		DeleteRelationshipAttributeFKConstraint,
		AddSimpleAttributeUniqueConstraint,
		DeleteSimpleAttributeUnique,
		AddSimpleAttributeAutoIncrement,
		DeleteSimpleAttributeAutoIncrement,
		AddSimpleAttributeIndex,
		DeleteSimpleAttributeIndex,
        AddSimpleAttributeHistoryEdit,
        DeleteSimpleAttributeHistoryEdit,
        AddSimpleAttributeRichText,
        DeleteSimpleAttributeRichText,
        AddSimpleAttributeDefaultValue,
        DeleteSimpleAttributeDefaultValue,
		AddConstraint,
		DeleteConstraint,
		AddRelationshipAttributeJoinTable,
		DeleteRelationshipAttributeJoinTable,
        AddRelationshipAttributeIndex,
        DeleteRelationshipAttributeIndex,
		ModifySimpleAttribute,
		ModifyRelationshipAttribute,
		ModifyArrayAttribute
	}
}