/*
* @(#)IndexingContext.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
	using System;
	using System.Xml;
	using System.Collections;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Events;
    using Newtera.Server.FullText;

	/// <summary> 
	/// Full-text search index context for the insert, update, and delete events.
	/// </summary>
	/// <version>  	1.0.0 24 Nov 2017</version>
	public class IndexingContext
	{
        private ClassElement _classElement;
        private string _objId;
		private MetaDataModel _metaData;
        private OperationType _operationType;

        /// <summary>
        /// Initiating an IndexingContext
        /// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="classElement">The class element that the instance belongs to</param>
        /// <param name="objId">The obj_id of the instance</param>
        /// <param name="operationType">One of OperationType enum values</param>
        public IndexingContext(MetaDataModel metaData, ClassElement classElement, string objId,
            OperationType operationType)
        {
            _metaData = metaData;
            _classElement = classElement;
            _objId = objId;
            _operationType = operationType;
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
	}
}