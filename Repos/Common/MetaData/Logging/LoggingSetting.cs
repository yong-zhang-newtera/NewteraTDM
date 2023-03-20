/*
* @(#)LoggingSetting.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary>
	/// This class defines some setting at the policy level 
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
	public class LoggingSetting : LoggingNodeBase
	{
		private LoggingPropagationType _propagationType;
		
		private LoggingConflictResolutionType _conflictResolutionType;
		
		private LoggingStatus _defaultReadLogStatus;
		
		private LoggingStatus _defaultWriteLogStatus;
		
		private LoggingStatus _defaultCreateLogStatus;
	
		private LoggingStatus _defaultDeleteLogStatus;

		private LoggingStatus _defaultUploadLogStatus;

		private LoggingStatus _defaultDownloadLogStatus;

        private LoggingStatus _defaultImportLogStatus;

        private LoggingStatus _defaultExportLogStatus;

		
		/// <summary>
		/// Initiate an instance of LoggingSetting class
		/// </summary>
		public LoggingSetting() : base()
		{
			_propagationType = LoggingPropagationType.Downward;
			_conflictResolutionType = LoggingConflictResolutionType.Ontp;
			_defaultReadLogStatus = LoggingStatus.Off;
			_defaultWriteLogStatus = LoggingStatus.Off;
			_defaultCreateLogStatus = LoggingStatus.Off;
			_defaultDeleteLogStatus = LoggingStatus.Off;
			_defaultUploadLogStatus = LoggingStatus.Off;
			_defaultDownloadLogStatus = LoggingStatus.Off;
            _defaultImportLogStatus = LoggingStatus.Off;
            _defaultExportLogStatus = LoggingStatus.Off;
		}

		/// <summary>
		/// Initiating an instance of LoggingSetting class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal LoggingSetting(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the propagation type
		/// </summary>
		/// <value> One of LoggingPropagationType class.</value>
		public LoggingPropagationType PropagationType
		{
			get
			{
				return _propagationType;
			}
			set
			{
				_propagationType = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the propagation type
		/// </summary>
		/// <value> One of LoggingPropagationType class.</value>
		public LoggingConflictResolutionType ConflictResolutionType
		{
			get
			{
				return _conflictResolutionType;
			}
			set
			{
				_conflictResolutionType = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default read log status.
		/// </summary>
		/// <value> One of LoggingStatus enum values</value>
		public LoggingStatus DefaultReadLogStatus
		{
			get
			{
				return _defaultReadLogStatus;
			}
			set
			{
				_defaultReadLogStatus = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default write log status.
		/// </summary>
		/// <value> One of LoggingStatus enum values</value>
		public LoggingStatus DefaultWriteLogStatus
		{
			get
			{
				return _defaultWriteLogStatus;
			}
			set
			{
				_defaultWriteLogStatus = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default create log status.
		/// </summary>
		/// <value> One of LoggingStatus enum values</value>
		public LoggingStatus DefaultCreateLogStatus
		{
			get
			{
				return _defaultCreateLogStatus;
			}
			set
			{
				_defaultCreateLogStatus = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default delete log status.
		/// </summary>
		/// <value> One of LoggingStatus enum values</value>
		public LoggingStatus DefaultDeleteLogStatus
		{
			get
			{
				return _defaultDeleteLogStatus;
			}
			set
			{
				_defaultDeleteLogStatus = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default upload log status.
		/// </summary>
		/// <value> One of LoggingStatus enum values</value>
		public LoggingStatus DefaultUploadLogStatus
		{
			get
			{
				return _defaultUploadLogStatus;
			}
			set
			{
				_defaultUploadLogStatus = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the default download log status.
		/// </summary>
		/// <value> One of LoggingStatus enum values</value>
		public LoggingStatus DefaultDownloadLogStatus
		{
			get
			{
				return _defaultDownloadLogStatus;
			}
			set
			{
				_defaultDownloadLogStatus = value;
				FireValueChangedEvent(value);
			}
		}

        /// <summary>
        /// Gets or sets the default import log status.
        /// </summary>
        /// <value> One of LoggingStatus enum values</value>
        public LoggingStatus DefaultImportLogStatus
        {
            get
            {
                return _defaultImportLogStatus;
            }
            set
            {
                _defaultImportLogStatus = value;
                FireValueChangedEvent(value);
            }
        }

        /// <summary>
        /// Gets or sets the default export log status.
        /// </summary>
        /// <value> One of LoggingStatus enum values</value>
        public LoggingStatus DefaultExportLogStatus
        {
            get
            {
                return _defaultExportLogStatus;
            }
            set
            {
                _defaultExportLogStatus = value;
                FireValueChangedEvent(value);
            }
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.Setting;
			}
		}

        /// <summary>
        /// Accept a visitor of ILoggingNodeVisitor type to traverse its elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(ILoggingNodeVisitor visitor)
        {
        }

		/// <summary>
		/// create an LoggingSetting from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			_propagationType = (LoggingPropagationType) Enum.Parse(typeof(LoggingPropagationType), parent.GetAttribute("propagation"));
			_conflictResolutionType = (LoggingConflictResolutionType) Enum.Parse(typeof(LoggingConflictResolutionType), parent.GetAttribute("conflict"));
			_defaultReadLogStatus = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("read"));
			_defaultWriteLogStatus = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("write"));
			_defaultCreateLogStatus = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("create"));
			_defaultDeleteLogStatus = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("delete"));
		    _defaultUploadLogStatus = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("upload"));
			_defaultDownloadLogStatus = (LoggingStatus) Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("download"));
            if (!string.IsNullOrEmpty(parent.GetAttribute("import")))
            {
                _defaultImportLogStatus = (LoggingStatus)Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("import"));
            }
            
            if (!string.IsNullOrEmpty(parent.GetAttribute("export")))
            {
                _defaultExportLogStatus = (LoggingStatus)Enum.Parse(typeof(LoggingStatus), parent.GetAttribute("export"));
            }
		}

		/// <summary>
		/// write LoggingSetting to xml document
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			parent.SetAttribute("propagation", Enum.GetName(typeof(LoggingPropagationType), _propagationType));
			parent.SetAttribute("conflict", Enum.GetName(typeof(LoggingConflictResolutionType), _conflictResolutionType));
			parent.SetAttribute("read", Enum.GetName(typeof(LoggingStatus), _defaultReadLogStatus));
			parent.SetAttribute("write", Enum.GetName(typeof(LoggingStatus), _defaultWriteLogStatus));
			parent.SetAttribute("create", Enum.GetName(typeof(LoggingStatus), _defaultCreateLogStatus));
			parent.SetAttribute("delete", Enum.GetName(typeof(LoggingStatus), _defaultDeleteLogStatus));
			parent.SetAttribute("upload", Enum.GetName(typeof(LoggingStatus), _defaultUploadLogStatus));
			parent.SetAttribute("download", Enum.GetName(typeof(LoggingStatus), _defaultDownloadLogStatus));
            parent.SetAttribute("import", Enum.GetName(typeof(LoggingStatus), _defaultImportLogStatus));
            parent.SetAttribute("export", Enum.GetName(typeof(LoggingStatus), _defaultExportLogStatus));
        }
	}
}