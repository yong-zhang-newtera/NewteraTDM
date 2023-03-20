/*
* @(#)ExecutionContext.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Generator
{
	using System;
	using System.Xml;
	using System.Collections.Specialized;

    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Principal;

	/// <summary> 
	/// The class provides an interface for execution context for formula execution.
	/// </summary>
	/// <version> 1.0.0 11 12 2012</version>
    public interface ExecutionContext
	{
        /// <summary>
        /// gets or sets the virtual attribute for the execution
        /// </summary>
        VirtualAttributeElement Attribute
        {
            get;

            set;
        }

        /// <summary>
        /// Evaluate a expression
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>Evaluate result</returns>
        object Evaluate(string expression);

        /// <summary>
        /// Execute an XQuery
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>Query result in XmlDocument</returns>
        XmlDocument ExecuteQuery(string query);

        /// <summary>
        /// A global counter that is increased by one at each invocation
        /// </summary>
        int GetAutoCounter();

        /// <summary>
        /// Get task owner display names of the workflow associated with an instance of the given id
        /// </summary>
        /// <param name="objId">An obj id of the instance</param>
        /// <returns>Task owner's display names, sepated by semicolon, or empty string if the instance doesn't have a binding workflow</returns>
        string GetWorkflowTaskOwners(string objId);

        /// <summary>
        /// Get id of the current login user
        /// </summary>
        /// <returns>The id of the current login user</returns>
        string GetCurrentUserID();

        /// <summary>
        /// Get display text of the current login user
        /// </summary>
        /// <returns>The displayed text of the current login user</returns>
        string GetCurrentUserDisplayText();

        /// <summary>
        /// Get display text of an user
        /// </summary>
        /// <returns>The displayed text of an user</returns>
        string GetUserDisplayText(string user);

        /// <summary>
        /// Get the UserManager object which provides detailed info about the current user
        /// </summary>
        /// <returns>IUserManager object</returns>
        IUserManager GetUserManager();
	}
}