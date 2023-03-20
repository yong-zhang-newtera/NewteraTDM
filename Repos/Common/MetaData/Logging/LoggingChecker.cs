/*
* @(#)LoggingChecker.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;
	using System.Collections;
	using System.Xml;
    using System.Threading;

	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.Logging.Processor;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// The class mainly provides some utility methods to be called by the application
	/// </summary>
	/// <version>  	1.0.0 04 Jan 2009</version>
	public sealed class LoggingChecker
	{	
		private LoggingProcessor _processor;
        private Hashtable _table;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static LoggingChecker theChecker;
		
		static LoggingChecker()
		{
			theChecker = new LoggingChecker();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private LoggingChecker()
		{
			_processor = new LoggingProcessor();
            _table = new Hashtable();
		}

		/// <summary>
		/// Gets the LoggingChecker instance.
		/// </summary>
		/// <returns> The LoggingChecker instance.</returns>
		static public LoggingChecker Instance
		{
			get
			{
				return theChecker;
			}
		}

        /// <summary>
        /// Get information indicating whether the logging of an action to a class
        /// is turned on.
        /// </summary>
        /// <param name="metaData">the meta data that contains a logging policy</param>
        /// <param name="className">The class name</param>
        /// <param name="action">an action of request, there are 4 kind
        /// of them: read, write, create, delete, all of them defined in class
        /// LoggingActionType.
        /// </param>
        /// <returns>true if the logging is turned on, otherwise false.</returns>
        public bool IsLoggingOn(MetaDataModel metaData, string className, LoggingActionType action)
        {
            bool isOn = false;

            // Get the CustomPrincipal object from the thread
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal == null || principal.IsSilentMode)
            {
                return isOn; // do not output logging when running as silent mode
            }

            ClassElement classElement = metaData.SchemaModel.FindClass(className);

            if (classElement != null)
            {
                isOn = IsLoggingOn(metaData, classElement, action);
            }

            return isOn;
        }

        /// <summary>
        /// Get information indicating whether the logging of an action to an IXaclObject
        /// is turned on.
        /// </summary>
        /// <param name="metaData">the meta data that contains a logging policy</param>
        /// <param name="element">An IXaclObject to be logged
		/// </param>
		/// <param name="action">an action of request, there are 4 kind
		/// of them: read, write, create, delete, all of them defined in class
		/// LoggingActionType.
		/// </param>
		/// <returns>true if the logging is turned on, otherwise false.</returns>
        public bool IsLoggingOn(MetaDataModel metaData, IXaclObject element, LoggingActionType action)
		{
            bool isOn = false;

            // Get the CustomPrincipal object from the thread
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            // Disable UserInfo logging for read action, it causes recursion
            if (metaData.SchemaInfo.Name.ToUpper() == "USERINFO")
            {
                return false;
            }

            if (principal == null || principal.IsSilentMode)
            {
                return isOn; // do not output logging when running as silent mode
            }

            // use schema id + xpath of element + action type to make the log key unique
            string logKey = metaData.SchemaInfo.NameAndVersion + element.ToXPath() + Enum.GetName(typeof(LoggingActionType), action);
            if (_table[logKey] == null)
            {
                // build a access request object
                LoggingRequestBuilder builder = LoggingRequestBuilder.Instance;
                LoggingRequest request = builder.BuildLoggingRequest(element, action);

                // send the request to the logging processor and get a conclusion
                Conclusion conclusion = _processor.Evaluate(request, metaData.LoggingPolicy);
                if (conclusion.Status == LoggingStatus.On)
                {
                    isOn = true;
                }

                // keep in the cache for sake of performance
                _table[logKey] = isOn;
            }
            else
            {
                isOn = (bool)_table[logKey];
            }

            return isOn;
		}

		/// <summary>
		/// Get the flags indicating whether an user has permission to perform certain
		/// action(s) to an object
		/// </summary>
		/// <param name="policy">the logging policy model object</param>
		/// <param name="element">An LoggingObject which the actions will be performed to.
		/// </param>
		/// <param name="actions">actions requested</param>
		/// <returns>A combined flags of LoggingActionFlag values.</returns>
        public LoggingActionFlag GetLoggingActionFlags(LoggingPolicy policy, IXaclObject element, LoggingActionType actions)
		{
			LoggingActionFlag flags = 0;
			LoggingRequestBuilder builder = LoggingRequestBuilder.Instance;
			LoggingRequest request;
		
			//TODO, get flags for all action types in one pass
			if ((actions & LoggingActionType.Read) != 0)
			{
				request = builder.BuildLoggingRequest(element, LoggingActionType.Read);
			
				// send the request to the logging processor and get a conclusion
				Conclusion conclusion = _processor.Evaluate(request, policy);
				if (conclusion.Status != LoggingStatus.Off)
				{
					flags = (flags | LoggingActionFlag.LogRead);
				}
			}

			if ((actions & LoggingActionType.Write) != 0)
			{
				request = builder.BuildLoggingRequest(element, LoggingActionType.Write);
			
				// send the request to the logging processor and get a conclusion
				Conclusion conclusion = _processor.Evaluate(request, policy);
				if (conclusion.Status != LoggingStatus.Off)
				{
					flags = (flags | LoggingActionFlag.LogWrite);
				}
			}

			if ((actions & LoggingActionType.Create) != 0)
			{
				request = builder.BuildLoggingRequest(element, LoggingActionType.Create);
			
				// send the request to the logging processor and get a conclusion
				Conclusion conclusion = _processor.Evaluate(request, policy);
				if (conclusion.Status != LoggingStatus.Off)
				{
					flags = (flags | LoggingActionFlag.LogCreate);
				}
			}

			if ((actions & LoggingActionType.Delete) != 0)
			{
				request = builder.BuildLoggingRequest(element, LoggingActionType.Delete);
			
				// send the request to the logging processor and get a conclusion
				Conclusion conclusion = _processor.Evaluate(request, policy);
				if (conclusion.Status != LoggingStatus.Off)
				{
					flags = (flags | LoggingActionFlag.LogDelete);
				}
			}

			if ((actions & LoggingActionType.Upload) != 0)
			{
				request = builder.BuildLoggingRequest(element, LoggingActionType.Upload);
			
				// send the request to the logging processor and get a conclusion
				Conclusion conclusion = _processor.Evaluate(request, policy);
				if (conclusion.Status != LoggingStatus.Off)
				{
					flags = (flags | LoggingActionFlag.LogUpload);
				}
			}

			if ((actions & LoggingActionType.Download) != 0)
			{
				request = builder.BuildLoggingRequest(element, LoggingActionType.Download);
			
				// send the request to the logging processor and get a conclusion
				Conclusion conclusion = _processor.Evaluate(request, policy);
				if (conclusion.Status != LoggingStatus.Off)
				{
					flags = (flags | LoggingActionFlag.LogDownload);
				}
			}

            if ((actions & LoggingActionType.Import) != 0)
            {
                request = builder.BuildLoggingRequest(element, LoggingActionType.Import);

                // send the request to the logging processor and get a conclusion
                Conclusion conclusion = _processor.Evaluate(request, policy);
                if (conclusion.Status != LoggingStatus.Off)
                {
                    flags = (flags | LoggingActionFlag.LogImport);
                }
            }

            if ((actions & LoggingActionType.Export) != 0)
            {
                request = builder.BuildLoggingRequest(element, LoggingActionType.Export);

                // send the request to the logging processor and get a conclusion
                Conclusion conclusion = _processor.Evaluate(request, policy);
                if (conclusion.Status != LoggingStatus.Off)
                {
                    flags = (flags | LoggingActionFlag.LogExport);
                }
            }
			
			return flags;
		}
		
		/// <summary>
		/// Gets a Conclusion object as the result of evaluating a access
		/// request against the policy.
		/// </summary>
		/// <param name="policy">the logging policy model object
		/// </param>
        /// <param name="element">An IXaclObject to be logged</param>
		/// <param name="action">an action of request, there are 4 kind
		/// of them: read, write, create, delete, all of them defined in class
		/// LoggingActionType.
		/// </param>
		/// <returns> 
		/// An Conclusion object.
		/// </returns>
        public Conclusion GetConclusion(LoggingPolicy policy, IXaclObject element, LoggingActionType action)
		{
			// build a access request object
			LoggingRequestBuilder builder = LoggingRequestBuilder.Instance;
			LoggingRequest request = builder.BuildLoggingRequest(element, action);
			
			// return the conclusion
			return _processor.Evaluate(request, policy);
		}

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void ClearCache()
        {
            _table.Clear();
        }
	}
}