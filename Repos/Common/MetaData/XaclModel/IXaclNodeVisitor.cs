/*
* @(#)IXaclNodeVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in a xacl model.
	/// </summary>
	/// <version> 1.0.0 16 Apr 2007 </version>
	public interface IXaclNodeVisitor
	{
		/// <summary>
		/// Viste a policy element.
		/// </summary>
		/// <param name="element">A XaclPolicy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitXaclPolicy(XaclPolicy element);

        /// <summary>
		/// Viste a XaclDefCollection element.
		/// </summary>
        /// <param name="element">A XaclDefCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitXaclDefCollection(XaclDefCollection element);
        
		/// <summary>
		/// Viste a XaclDef element.
		/// </summary>
		/// <param name="element">A XaclDef instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitXaclDef(XaclDef element);

        /// <summary>
        /// Viste a XaclObject element.
        /// </summary>
        /// <param name="element">A XaclObject instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitXaclObject(XaclObject element);

        /// <summary>
        /// Viste a XaclRuleCollection element.
        /// </summary>
        /// <param name="element">A XaclRuleCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitXaclRuleCollection(XaclRuleCollection element);

		/// <summary>
		/// Viste a XaclRule element.
		/// </summary>
		/// <param name="element">A XaclRule instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitXaclRule(XaclRule element);

		/// <summary>
		/// Viste XaclSubject element.
		/// </summary>
		/// <param name="element">A XaclSubject instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitXaclSubject(XaclSubject element);

        /// <summary>
        /// Viste XaclCondition element.
        /// </summary>
        /// <param name="element">A XaclCondition instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitXaclCondition(XaclCondition element);

        /// <summary>
        /// Viste a XaclActionCollection element.
        /// </summary>
        /// <param name="element">A XaclActionCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitXaclActionCollection(XaclActionCollection element);

		/// <summary>
		/// Viste a XaclAction element.
		/// </summary>
		/// <param name="element">A XaclAction instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitXaclAction(XaclAction element);
	}
}