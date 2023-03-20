/*
* @(#)IApiNodeVisitor.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in an api name space.
	/// </summary>
	/// <version> 1.0.0 22 Sep 2013 </version>
	public interface IApiNodeVisitor
	{
		/// <summary>
        /// Viste a ApiManager element.
		/// </summary>
        /// <param name="element">A ApiManager instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitApiManager(ApiManager element);

        /// <summary>
		/// Viste an ApiGroupCollection element.
		/// </summary>
        /// <param name="element">A ApiGroupCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitApiGroupCollection(ApiGroupCollection element);
        
		/// <summary>
		/// Viste an ApiGroup element.
		/// </summary>
        /// <param name="element">A ApiGroup instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitApiGroup(ApiGroup element);

        /// <summary>
        /// Viste an ApiCollection element.
        /// </summary>
        /// <param name="element">A ApiCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitApiCollection(ApiCollection element);

		/// <summary>
		/// Viste a Api element.
		/// </summary>
        /// <param name="element">A Api instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
        bool VisitApi(Api element);
	}
}