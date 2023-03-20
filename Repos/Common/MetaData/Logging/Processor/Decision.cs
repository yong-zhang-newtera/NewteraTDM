/*
* @(#)Decision.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Processor
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Logging;

	/// <summary>
	/// The class represents a decision as result of evaluating a single LoggingRule
	/// </summary>
	/// <version> 1.0.0 05 Jan 2009 </version>
	public class Decision
	{
		// the rule that results the decision
		private LoggingRule _rule;
		
		// the evaluation status
		private LoggingStatus _status;
		
		/// <summary>
		/// Initiate an instance of Decision class.
		/// </summary>
		/// <param name="rule"> The rule of a decision.</param>
		/// <param name="status">One of LoggingStatus</param>
		public Decision(LoggingRule rule, LoggingStatus status)
		{
			_rule = rule;
			_status = status;
		}

		/// <summary>
		/// Gets status of a decision
		/// </summary>
		/// <value>One of LoggingStatus values</value>
		public LoggingStatus Status
		{
			get
			{
				return _status;
			}
		}

		/// <summary>
		/// Gets the rule of a decision.
		/// </summary>
		/// <value> An LoggingRule object</value>
		public LoggingRule Rule
		{
			get
			{
				return _rule;
			}
		}
	}
}