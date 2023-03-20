/*
* @(#)GenerateConfigurationVisitor.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MLActivities.MLConfig
{
	using System;

	/// <summary>
	/// Vistor that replace the collection node with a child component of the given index
	/// </summary>
	public class ReplaceCollectionNodeVisitor : MLComponnetVisitorBase, IMLComponnetVisitor
    {
        private int childIndex;
        private IMLComponnet parentComponent;
        private MLConfiguration theConfiguration;

        public ReplaceCollectionNodeVisitor(int nodeIndex)
        {
            childIndex = nodeIndex;
        }

        /// <summary>
        /// Viste a MLConfiguration component.
        /// </summary>
        /// <param name="component">A MLConfiguration object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitConfiguration(MLConfiguration component)
        {
            theConfiguration = component;
            return true;
        }

        /// <summary>
        /// Viste a MLCommand component.
        /// </summary>
        /// <param name="component">A MLCommand object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitCommand(MLCommand component)
        {
            parentComponent = component;

            return true;
        }

        /// <summary>
        /// Viste a MLComponentCollection component.
        /// </summary>
        /// <param name="component">A MLComponentCollection object</param>
        /// <returns>true to visit its children, false otherwise</returns>
        public bool VisitComponentCollection(MLComponentCollection component)
        {
            if (component.Count > childIndex && parentComponent != null)
            {
                IMLComponnet child = component[childIndex];

                theConfiguration.BranchName = child.Name;

                parentComponent.Children.Remove(component);

                parentComponent.Children.Add(child);

                /*
                int pos = parentComponent.Children.IndexOf(component);
                if (pos >= 0)
                {
                    // remove the collection node from the parent
                    parentComponent.Children.RemoveAt(pos);

                    // add the child componnet to the parent
                    parentComponent.Children.Insert(pos, child);
                }
                */
            }

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