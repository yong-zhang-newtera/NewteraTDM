/*
* @(#)ModelElementTreeNode.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowStudioControl
{
	using System;
	using System.Windows.Forms;

    using Newtera.WFModel;
	
	/// <summary>
	/// Represents a tree node for a IWFModelElement object
	/// </summary>
	/// <version>  1.0.1 9 Dec 2006</version>
    public class ModelElementTreeNode : TreeNode
    {
        private IWFModelElement _modelElement;

        /// <summary>
        /// Create a new instance of the ModelElementTreeNode class.
        /// </summary>
        /// <param name="name">The workflow model element</param>
        public ModelElementTreeNode(IWFModelElement modelElement)
        {
            _modelElement = modelElement;
        }

        /// <summary> 
        /// Gets or sets the workflow model element.
        /// </summary>
        public IWFModelElement ModelElement
        {
            get
            {
                return _modelElement;
            }
            set
            {
                _modelElement = value;
            }
        }
    }
}