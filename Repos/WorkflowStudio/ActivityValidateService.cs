/*
* @(#)ActivityValidateService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;
    using Newtera.Activities;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Events;
    using Newtera.Common.MetaData.Principal;
    using Newtera.WinClientCommon;

	/// <summary>
	/// providing service for validating properties of custom activities.
	/// </summary>
	/// <version>  	1.0.0 10 May 2007</version>
	public class ActivityValidateService : IActivityValidateService
	{
        private Newtera.Common.Core.SchemaInfo[] _schemaInfos;
        private IUserManager _userManager;
        private string[] _users;
        private string[] _roles;

        public ActivityValidateService()
        {
            _schemaInfos = null;
            _userManager = new WindowClientUserManager();
            _users = null;
            _roles = null;
        }

        /// <summary>
        /// Gets the information indicating whethere a schema id is valid
        /// </summary>
        public bool IsValidSchemaId(string schemaId)
        {
            bool status = false;

            if (_schemaInfos == null)
            {
                _schemaInfos = GetAuthorizedSchemaInfos();
            }

            foreach (Newtera.Common.Core.SchemaInfo schemaInfo in _schemaInfos)
            {
                if (schemaInfo.NameAndVersion == schemaId)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere a class name is valid
        /// </summary>
        public bool IsValidClassName(string schemaId, string className)
        {
            bool status = false;

            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(schemaId);
            if (metaData == null)
            {
                metaData = this.GetMetaData(schemaId);
                MetaDataStore.Instance.PutMetaData(metaData);
            }

            if (metaData != null &&
                metaData.SchemaModel.FindClass(className) != null)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere an attribute name is valid
        /// </summary>
        public bool IsValidAttributeName(string schemaId, string className, string attributeName)
        {
            bool status = false;

            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(schemaId);
            if (metaData == null)
            {
                metaData = this.GetMetaData(schemaId);
                MetaDataStore.Instance.PutMetaData(metaData);
            }

            if (metaData != null)
            {
                ClassElement clsElement = metaData.SchemaModel.FindClass(className);
                if (clsElement != null)
                {
                    if (clsElement.FindInheritedSimpleAttribute(attributeName) != null)
                    {
                        status = true;
                    }
                    else if (clsElement.FindInheritedArrayAttribute(attributeName) != null)
                    {
                        status = true;
                    }
                    else if (clsElement.FindInheritedVirtualAttribute(attributeName) != null)
                    {
                        status = true;
                    }
                    else if (clsElement.FindInheritedImageAttribute(attributeName) != null)
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere an attribute caption is valid
        /// </summary>
        public bool IsValidAttributeCaption(string schemaId, string className, string attributeCaption)
        {
            bool status = false;

            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(schemaId);
            if (metaData == null)
            {
                metaData = this.GetMetaData(schemaId);
                MetaDataStore.Instance.PutMetaData(metaData);
            }

            if (metaData != null)
            {
                DataViewModel dataView = metaData.GetDetailedDataView(className);
                if (dataView != null)
                {
                    foreach (IDataViewElement attribute in dataView.ResultAttributes)
                    {
                        if (attribute.Caption == attributeCaption)
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere an event name is valid
        /// </summary>
        public bool IsValidEventName(string schemaId, string className, string eventName)
        {
            bool status = false;

            MetaDataModel metaData = MetaDataStore.Instance.GetMetaData(schemaId);
            if (metaData == null)
            {
                metaData = this.GetMetaData(schemaId);
                MetaDataStore.Instance.PutMetaData(metaData);
            }

            if (metaData != null)
            {
                ClassElement clsElement = metaData.SchemaModel.FindClass(className);
                if (clsElement != null)
                {
                    EventCollection events = metaData.EventManager.GetClassEvents(clsElement);

                    foreach (EventDef eventDef in events)
                    {
                        if (eventDef.Name == eventName)
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere a search statement is valid
        /// </summary>
        public bool IsValidSearchQuery(string schemaId, string className, string searchQuery)
        {
            WorkflowModelServiceStub workflowModelService = new WorkflowModelServiceStub();

            bool status = workflowModelService.IsQueryValid(ConnectionStringBuilder.Instance.Create(),
                searchQuery);

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere an event name is valid
        /// </summary>
        public bool IsValidInsertQuery(string schemaId, string className, string insertQuery)
        {
            WorkflowModelServiceStub workflowModelService = new WorkflowModelServiceStub();

            bool status = workflowModelService.IsQueryValid(ConnectionStringBuilder.Instance.Create(),
                insertQuery);

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the format of a custom function definition is correct
        /// </summary>
        /// <param name="functionDefinition">The custom function definition</param>
        /// <returns>true if it is correct, false otherwise.</returns>
        public bool IsValidCustomFunction(string functionDefinition)
        {
            WorkflowModelServiceStub workflowModelService = new WorkflowModelServiceStub();

            bool status = workflowModelService.IsValidCustomFunctionDefinition(ConnectionStringBuilder.Instance.Create(), 
                functionDefinition);

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere an user name is valid
        /// </summary>
        public bool IsValidUser(string userName)
        {
            bool status = false;

            if (_users == null)
            {
                _users = _userManager.GetAllUsers();
            }

            if (_users != null)
            {
                foreach (string user in _users)
                {
                    if (user == userName)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere a role name is valid
        /// </summary>
        public bool IsValidRole(string roleName)
        {
            bool status = false;

            if (_roles == null)
            {
                _roles = _userManager.GetAllRoles();
            }

            if (_roles != null)
            {
                foreach (string role in _roles)
                {
                    if (role == roleName)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whethere an action code is valid 
        /// </summary>
        /// <param name="actionCode">The action code</param>
        /// <param name="errorMessage">The error message if the method return false</param>
        /// <param name="schemaId">The schema id indicates the schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <returns>true if the code is valid, false otherwise.</returns>
        public bool IsValidActionCode(string actionCode, string schemaId, string instanceClassName, out string errorMessage)
        {
            WorkflowModelServiceStub workflowModelService = new WorkflowModelServiceStub();

            string msg = workflowModelService.ValidateActionCode(ConnectionStringBuilder.Instance.Create(),
                actionCode, schemaId, instanceClassName);

            if (!string.IsNullOrEmpty(msg))
            {
                // there are errors
                errorMessage = msg;
                return false;
            }
            else
            {
                // there are no errors
                errorMessage = "";
                return true;
            }
        }

        /// <summary>
        /// Gets the meta data of the selected schema
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <returns>A MetaDataModel instance.</returns>
        public MetaDataModel GetMetaData(string schemaId)
        {
            MetaDataModel metaData = null;

            if (_schemaInfos == null)
            {
                _schemaInfos = GetAuthorizedSchemaInfos();
            }

            if (_schemaInfos != null)
            {
                foreach (Newtera.Common.Core.SchemaInfo schemaInfo in _schemaInfos)
                {
                    if (schemaInfo.NameAndVersion == schemaId)
                    {
                        metaData = GetMetaData(schemaInfo);
                        break;
                    }
                }
            }

            return metaData;
        }

        /// <summary>
        /// Gets the meta data of the selected schema
        /// </summary>
        /// <param name="schemaInfo"></param>
        /// <returns>A MetaDataModel instance.</returns>
        internal MetaDataModel GetMetaData(Newtera.Common.Core.SchemaInfo schemaInfo)
        {
            MetaDataServiceStub service = new MetaDataServiceStub();

            // Create an meta data object to hold the schema model
            MetaDataModel metaData = new MetaDataModel();
            metaData.SchemaInfo = schemaInfo;

            // create a MetaDataModel instance from the xml strings retrieved from the database
            string[] xmlStrings = service.GetMetaData(ConnectionStringBuilder.Instance.Create(metaData.SchemaInfo.Name, metaData.SchemaInfo.Version));
            metaData.Load(xmlStrings);

            return metaData;
        }

        /// <summary>
        /// Gets the schema infos that the user is authorized to access.
        /// </summary>
        /// <returns>An array of SchemaInfo objects.</returns>
        internal Newtera.Common.Core.SchemaInfo[] GetAuthorizedSchemaInfos()
        {
            if (_schemaInfos == null)
            {
                WorkflowModelServiceStub workflowModelServic = new WorkflowModelServiceStub();

                SchemaInfo[] schemas = workflowModelServic.GetAuthorizedSchemaInfos(ConnectionStringBuilder.Instance.Create());
                _schemaInfos = new Newtera.Common.Core.SchemaInfo[schemas.Length];
                for (int i = 0; i < schemas.Length; i++)
                {
                    _schemaInfos[i] = new Newtera.Common.Core.SchemaInfo();
                    _schemaInfos[i].Name = schemas[i].Name;
                    _schemaInfos[i].Version = schemas[i].Version;
                }
            }

            return _schemaInfos;
        }
	}
}