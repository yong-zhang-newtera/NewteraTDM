/*
* @(#)Conclusion.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Processor
{
	using System;
	using System.Collections;

	using Newtera.Common.MetaData.Logging;

	/// <summary>
	/// The class represents the result for evaluation of a logging request
	/// </summary>
	/// <version> 1.0.0 05 Jan 2009 </version>
	public class Conclusion
	{
		private LoggingStatus _status;
		private ArrayList _decisionList;
		
		/// <summary> 
		/// Initiate an instance of Conclusion class
		/// </summary>
		public Conclusion()
		{
			_status = LoggingStatus.Unknown;
			_decisionList = null;
		}
		
		/// <summary>
		/// Gets or sets the resulting logging status
		/// </summary>
		/// <value> One of the LoggingStatus</value>
		public LoggingStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
			}
		}

		/// <summary>
		/// Gets or sets the decision list.
		/// </summary>
		/// <value> A list of Decision objects.</value>
		public ArrayList DecisionList
		{
			get
			{
				return _decisionList;
			}
			set
			{
				_decisionList = value;
			}
		}
	}
}