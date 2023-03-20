/*
* @(#)IMLComponnetVisitor.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MLActivities.MLConfig
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse componnets in a MLConfiguration model.
	/// </summary>
	public interface IMLComponnetVisitor
	{
        /// <summary>
        /// Viste a MLConfiguration component.
        /// </summary>
        /// <param name="component">A MLConfiguration object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        bool VisitConfiguration(MLConfiguration component);

        /// <summary>
        /// Viste a MLCommand component.
        /// </summary>
        /// <param name="component">A MLCommand object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        bool VisitCommand(MLCommand component);

        /// <summary>
        /// Viste a MLComponentCollection component.
        /// </summary>
        /// <param name="component">A MLComponentCollection object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        bool VisitComponentCollection(MLComponentCollection component);

        /// <summary>
        /// Viste a MLReader component.
        /// </summary>
        /// <param name="component">A MLReader object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        bool VisitReader(MLReader component);

        /// <summary>
        /// Viste a MLSGD component.
        /// </summary>
        /// <param name="component">A MLSGD object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        bool VisitSGD(MLSGD component);

        /// <summary>
        /// Viste a component of MLNetworkBuilder component.
        /// </summary>
        /// <param name="component">A MLNetworkBuilder object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        bool VisitCommonComponent(IMLComponnet component);
    }
}