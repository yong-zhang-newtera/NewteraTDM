/*
* @(#)ILoggingNodeVisitor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in a logging name space.
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
	public interface ILoggingNodeVisitor
	{
		/// <summary>
		/// Viste a policy element.
		/// </summary>
		/// <param name="element">A LoggingPolicy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitLoggingPolicy(LoggingPolicy element);

        /// <summary>
		/// Viste a LoggingDefCollection element.
		/// </summary>
        /// <param name="element">A LoggingDefCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitLoggingDefCollection(LoggingDefCollection element);
        
		/// <summary>
		/// Viste a LoggingDef element.
		/// </summary>
		/// <param name="element">A LoggingDef instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitLoggingDef(LoggingDef element);

        /// <summary>
        /// Viste a LoggingObject element.
        /// </summary>
        /// <param name="element">A LoggingObject instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitLoggingObject(LoggingObject element);

        /// <summary>
        /// Viste a LoggingRuleCollection element.
        /// </summary>
        /// <param name="element">A LoggingRuleCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitLoggingRuleCollection(LoggingRuleCollection element);

		/// <summary>
		/// Viste a LoggingRule element.
		/// </summary>
		/// <param name="element">A LoggingRule instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitLoggingRule(LoggingRule element);

		/// <summary>
		/// Viste LoggingSubject element.
		/// </summary>
		/// <param name="element">A LoggingSubject instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitLoggingSubject(LoggingSubject element);

        /// <summary>
        /// Viste a LoggingActionCollection element.
        /// </summary>
        /// <param name="element">A LoggingActionCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitLoggingActionCollection(LoggingActionCollection element);

		/// <summary>
		/// Viste a LoggingAction element.
		/// </summary>
		/// <param name="element">A LoggingAction instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitLoggingAction(LoggingAction element);
	}
}