/*
* @(#)ActivityValidatingServiceProvider.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Activities
{
	using System;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;

	/// <summary>
	/// A singleton class that provide a service instance for validating custom activities
	/// </summary>
	/// <version>  	1.0.0 10 May 2007 </version>
	/// <author> Yong Zhang </author>
	public class ActivityValidatingServiceProvider
	{
        IActivityValidateService _service;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ActivityValidatingServiceProvider theInstance;
		
		static ActivityValidatingServiceProvider()
		{
			theInstance = new ActivityValidatingServiceProvider();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ActivityValidatingServiceProvider()
		{
            _service = new DefaultActivityValidateService();
		}

		/// <summary>
		/// Gets the ActivityValidatingServiceProvider instance.
		/// </summary>
		/// <returns> The ActivityValidatingServiceProvider instance.</returns>
		static public ActivityValidatingServiceProvider Instance
		{
			get
			{
				return theInstance;
			}
		}
		
		/// <summary>
		/// Gets or set the activity validate service.
		/// </summary>
        /// <value>The IActivityValidateService instance.</value>
        public IActivityValidateService ValidateService
		{
			get
			{
                return _service;
			}
            set
            {
                _service = value;
            }
		}
	}

    internal class DefaultActivityValidateService : IActivityValidateService
    {
        /// <summary>
        /// Gets the information indicating whethere a schema id is valid
        /// </summary>
        public bool IsValidSchemaId(string schemaId)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere a class name is valid
        /// </summary>
        public bool IsValidClassName(string schemaId, string className)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere an attribute name is valid
        /// </summary>
        public bool IsValidAttributeName(string schemaId, string className, string attributeName)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere an attribute caption is valid
        /// </summary>
        public bool IsValidAttributeCaption(string schemaId, string className, string attributeCaption)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere an event name is valid
        /// </summary>
        public bool IsValidEventName(string schemaId, string className, string eventName)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere a search statement is valid
        /// </summary>
        public bool IsValidSearchQuery(string schemaId, string className, string searchQuery)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere an event name is valid
        /// </summary>
        public bool IsValidInsertQuery(string schemaId, string className, string insertQuery)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whether the format of a custom function definition is correct
        /// </summary>
        /// <param name="functionDefinition">The custom function definition</param>
        /// <returns>true if it is correct, false otherwise.</returns>
        public bool IsValidCustomFunction(string functionDefinition)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere an user name is valid
        /// </summary>
        public bool IsValidUser(string userName)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere a role name is valid
        /// </summary>
        public bool IsValidRole(string roleName)
        {
            return true;
        }

        /// <summary>
        /// Gets the information indicating whethere an action code is valid 
        /// </summary>
        /// <param name="actionCode">The action code</param>
        /// <param name="schemaId">The schema id indicates the data schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <param name="errorMessage">The error message if the method return false</param>
        /// <returns>true if the code is valid, false otherwise.</returns>
        public bool IsValidActionCode(string actionCode, string schemaId, string instanceClassName, out string errorMessage)
        {
            errorMessage = "";

            return true;
        }

        /// <summary>
        /// Gets the meta data of the selected schema
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A MetaDataModel instance.</returns>
        public MetaDataModel GetMetaData(string schemaId)
        {
            return null;
        }
    }
}