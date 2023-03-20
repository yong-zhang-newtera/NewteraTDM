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
	/// The class provides execution context for formula execution.
	/// </summary>
	/// <version> 1.0.0 27 May 2006</version>
	public class DefaultExecutionContext : ExecutionContext
	{
        private VirtualAttributeElement _attribute;

        public DefaultExecutionContext()
        {
            _attribute = null;
        }

        /// <summary>
        /// gets or sets the virtual attribute for the execution
        /// </summary>
        public VirtualAttributeElement Attribute
        {
            get
            {
                return _attribute;
            }
            set
            {
                _attribute = value;
            }
        }

        /// <summary>
        /// Evaluate a expression
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>Evaluate result</returns>
        public object Evaluate(string expression)
        {
            return null;
        }

        /// <summary>
        /// Execute an XQuery
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>Query result in XmlDocument</returns>
        public XmlDocument ExecuteQuery(string query)
        {
            return null;
        }

        /// <summary>
        /// A global counter that is increased by one at each invocation
        /// </summary>
        public int GetAutoCounter()
        {
            return 0;
        }

        /// <summary>
        /// Get task owner display names of the workflow associated with an instance of the given id
        /// </summary>
        /// <param name="objId">An obj id of the instance</param>
        /// <returns>Task owner's display names, sepated by semicolon, or empty string if the instance doesn't have a binding workflow</returns>
        public string GetWorkflowTaskOwners(string objId)
        {
            return "";
        }

        /// <summary>
        /// Get id of the current login user
        /// </summary>
        /// <returns>The id of the current login user</returns>
        public string GetCurrentUserID()
        {
            return "";
        }

        /// <summary>
        /// Get display text of the current login user
        /// </summary>
        /// <returns>The displayed text of the current login user</returns>
        public string GetCurrentUserDisplayText()
        {
            return "";
        }

        /// <summary>
        /// Get display text of an user
        /// </summary>
        /// <returns>The displayed text of an user</returns>
        public string GetUserDisplayText(string user)
        {
            return "";
        }

        /// <summary>
        /// Get the UserManager object which provides detailed info about the current user
        /// </summary>
        /// <returns>IUserManager object</returns>
        public IUserManager GetUserManager()
        {
            return null;
        }
	}
}