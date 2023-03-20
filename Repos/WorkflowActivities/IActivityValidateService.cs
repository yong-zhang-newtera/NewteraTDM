/*
* @(#)IActivityValidateService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Activities
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.MetaData;

	/// <summary>
	/// Represents a common interface for providing service for validating
    /// properties of custom activities
    /// in Newtera Database.
	/// </summary>
	/// <version>  	1.0.0 10 May 2007</version>
	public interface IActivityValidateService
	{
        /// <summary>
        /// Gets the information indicating whethere a schema id is valid
        /// </summary>
        bool IsValidSchemaId(string schemaId);

        /// <summary>
        /// Gets the information indicating whethere a class name is valid
        /// </summary>
        bool IsValidClassName(string schemaId, string className);

        /// <summary>
        /// Gets the information indicating whethere an attribute name is valid
        /// </summary>
        bool IsValidAttributeName(string schemaId, string className, string attributeName);

        /// <summary>
        /// Gets the information indicating whethere an attribute caption is valid
        /// </summary>
        bool IsValidAttributeCaption(string schemaId, string className, string attributeCaption);

        /// <summary>
        /// Gets the information indicating whethere an event name is valid
        /// </summary>
        bool IsValidEventName(string schemaId, string className, string eventName);

        /// <summary>
        /// Gets the information indicating whethere an insert statement is valid
        /// </summary>
        bool IsValidInsertQuery(string schemaId, string className, string insertQuery);

        /// <summary>
        /// Gets the information indicating whethere a search statement is valid
        /// </summary>
        bool IsValidSearchQuery(string schemaId, string className, string searchQuery);

        /// <summary>
        /// Gets the information indicating whether the format of a custom function definition is correct
        /// </summary>
        /// <param name="functionDefinition">The custom function definition</param>
        /// <returns>true if it is correct, false otherwise.</returns>
        bool IsValidCustomFunction(string functionDefinition);

        /// <summary>
        /// Gets the information indicating whethere an user name is valid
        /// </summary>
        bool IsValidUser(string userName);

        /// <summary>
        /// Gets the information indicating whethere a role name is valid
        /// </summary>
        bool IsValidRole(string roleName);

        /// <summary>
        /// Gets the information indicating whethere an action code is valid 
        /// </summary>
        /// <param name="actionCode">The action code</param>
        /// <param name="schemaId">The schema id indicates the data schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <param name="errorMessage">The error message if the method return false</param>
        /// <returns>true if the code is valid, false otherwise.</returns>
        bool IsValidActionCode(string actionCode, string schemaId, string instanceClassName, out string errorMessage);

        /// <summary>
        /// Gets the meta data of the selected schema
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A MetaDataModel instance.</returns>
        MetaDataModel GetMetaData(string schemaId);
	}
}