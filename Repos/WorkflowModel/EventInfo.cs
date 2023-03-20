/*
* @(#)EventInfo.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// A class contains information that uniquely identifies an event from Newtera Data Source.
	/// </summary>
	/// <version>1.0.0 27 Dec 2006 </version>
	public class EventInfo
	{
        private string _schemaId;
        private string _className;
        private string _classCaption;
        private string _eventName;

        public EventInfo()
        {
            _schemaId = null;
            _className = null;
            _classCaption = null;
            _eventName = null;
        }

        /// <summary>
        /// Gets or sets the id of a schema
        /// </summary>
        public string SchemaID
        {
            get
            {
                return _schemaId;
            }
            set
            {
                _schemaId = value;
            }
        }

        /// <summary>
        /// Gets or sets name of class where the event is defined
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        /// <summary>
        /// Gets or sets caption of class where the event is defined
        /// </summary>
        public string ClassCaption
        {
            get
            {
                return _classCaption;
            }
            set
            {
                _classCaption = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the event
        /// </summary>
        public string EventName
        {
            get
            {
                return _eventName;
            }
            set
            {
                _eventName = value;
            }
        }
	}
}