/*
* @(#)EventContext.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;

	/// <summary> 
	/// Holding the event context for the insert, update, and delete instance.
	/// </summary>
	/// <version>  	1.0.0 27 Dec 2006</version>
	public class EventContext
	{
        private string _eventConextId;
        private ClassElement _classElement;
        private string _objId;
		private MetaDataModel _metaData;
        private OperationType _operationType;
        private StringCollection _attributesUpdated;
        private XmlElement _xmlInstance;
        private string _userId;
		
		/// <summary>
        /// Initiating an EventContext
		/// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="classElement">The class element that the instance belongs to</param>
        /// <param name="objId">The obj_id of the instance</param>
        /// <param name="operationType">One of OperationType enum values</param>
        public EventContext(string eventContextId, MetaDataModel metaData, ClassElement classElement, string objId,
            OperationType operationType)
            : this(eventContextId, metaData, classElement, objId, operationType,
            new StringCollection(), null)
		{
		}

        /// <summary>
        /// Initiating an EventContext
        /// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="classElement">The class element that the instance belongs to</param>
        /// <param name="objId">The obj_id of the instance</param>
        /// <param name="operationType">One of OperationType enum values</param>
        /// <param name="userId">Id of the user whose action generates the event</param>
        public EventContext(string eventContextId, MetaDataModel metaData, ClassElement classElement, string objId,
            OperationType operationType, StringCollection updatedAttributes)
            : this(eventContextId, metaData, classElement, objId, operationType,
            new StringCollection(), null)
        {
        }

        /// <summary>
        /// Initiating an EventContext
        /// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="classElement">The class element that the instance belongs to</param>
        /// <param name="objId">The obj_id of the instance</param>
        /// <param name="operationType">One of OperationType enum values</param>
        /// <param name="updatedAttributes">Updated attributes</param>
        public EventContext(string eventContextId, MetaDataModel metaData, ClassElement classElement, string objId,
            OperationType operationType, StringCollection updatedAttributes, string userId)
        {
            _eventConextId = eventContextId;
            _metaData = metaData;
            _classElement = classElement;
            _objId = objId;
            _operationType = operationType;
            _attributesUpdated = updatedAttributes;
            _xmlInstance = null;
            _userId = userId;
        }

        /// <summary>
        /// Initiating an EventContext
        /// </summary>
        public EventContext()
        {
            _eventConextId = null;
            _metaData = null;
            _classElement = null;
            _objId = null;
            _operationType = OperationType.Unknown;
            _attributesUpdated = null;
            _xmlInstance = null;
        }

        /// <summary>
        /// Gets the event context id
        /// </summary>
        public string EventContextId
        {
            get
            {
                return _eventConextId;
            }
            set
            {
                _eventConextId = value;
            }
        }

        /// <summary>
        /// Gets the MetaDataModel object
        /// </summary>
        public MetaDataModel MetaData
        {
            get
            {
                return _metaData;
            }
            set
            {
                _metaData = value;
            }
        }

        /// <summary>
        /// Gets the ClassElement object
        /// </summary>
        public ClassElement ClassElement
        {
            get
            {
                return _classElement;
            }
            set
            {
                _classElement = value;
            }
        }

        /// <summary>
        /// Gets the obj_id of the data instance
        /// </summary>
        public string ObjId
        {
            get
            {
                return _objId;
            }
            set
            {
                _objId = value;
            }
        }

        /// <summary>
        /// Gets the operation type
        /// </summary>
        public OperationType OperationType
        {
            get
            {
                return _operationType;
            }
            set
            {
                _operationType = value;
            }
        }

        /// <summary>
        /// Gets the collection of attribute names whose values have been updated
        /// </summary>
        public StringCollection AttributesUpdated
        {
            get
            {
                return _attributesUpdated;
            }
            set
            {
                _attributesUpdated = value;
            }
        }

        /// <summary>
        /// The xml element representing the instance to be evalutated against the event conditions
        /// </summary>
        public XmlElement XmlInstance
        {
            get
            {
                return _xmlInstance;
            }
            set
            {
                _xmlInstance = value;
            }
        }

        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        public bool ContainsUpdatedAttribute(string attributeName)
        {
            bool status = false;

            if (_attributesUpdated != null)
            {
                foreach (string name in _attributesUpdated)
                {
                    if (name == attributeName)
                    {
                        status = true;
                        break;
                    }
                }
            }

            return status;
        }
	}
}