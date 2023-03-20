/*
* @(#)ConfigurationCountVisitor.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MLActivities.MLConfig
{
	using System;

	/// <summary>
	/// Vistor that return count of executable configuration
	/// </summary>
	public class ConfigurationCountVisitor : MLComponnetVisitorBase, IMLComponnetVisitor
    {
        public int ExecutableConfigurationCount { get; set; }

        public ConfigurationCountVisitor()
        {
            ExecutableConfigurationCount = 0;
        }

        /// <summary>
        /// Viste a MLConfiguration component.
        /// </summary>
        /// <param name="component">A MLConfiguration object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitConfiguration(MLConfiguration component)
        {
            return true;
        }

        /// <summary>
        /// Viste a MLCommand component.
        /// </summary>
        /// <param name="component">A MLCommand object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitCommand(MLCommand component)
        {
            return true;
        }

        /// <summary>
        /// Viste a MLComponentCollection component.
        /// </summary>
        /// <param name="component">A MLComponentCollection object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitComponentCollection(MLComponentCollection component)
        {
            // assuming there is only one collection in a configuration
            ExecutableConfigurationCount = component.Count;

            return false;
        }

        /// <summary>
        /// Viste a MLReader component.
        /// </summary>
        /// <param name="component">A MLReader object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitReader(MLReader component)
        {
            return true;
        }

        /// <summary>
        /// Viste a MLSGD component.
        /// </summary>
        /// <param name="component">A MLSGD object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitSGD(MLSGD component)
        {
            return true;
        }

        /// <summary>
        /// Viste a MLNetworkBuilder component.
        /// </summary>
        /// <param name="component">A MLNetworkBuilder object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitCommonComponent(IMLComponnet component)
        {
            return true;
        }
    }
}