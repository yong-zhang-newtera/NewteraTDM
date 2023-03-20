/*
* @(#)ModelTreeBuilder.cs
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
	/// Build a model tree for the view tree
	/// </summary>
	/// <version>1.0.0 9 Dec 2006</version>
    public class ModelTreeBuilder
    {
        /// <summary>
        /// Builds a tree for a project model
        /// </summary>
        /// <param name="project">The ProjectModel</param>
        /// <returns>The root of the tree</returns>
        public static ModelElementTreeNode BuildTree(ProjectModel project)
        {
            // the root represents the project
            ModelElementTreeNode root = CreateTreeNode(project);

            // add workflow models as children of the tree
            foreach (IWFModelElement workflow in project.Workflows)
            {
                ModelElementTreeNode childNode = CreateTreeNode(workflow);

                root.Nodes.Add(childNode);
            }

            return root;
        }

        /// <summary>
        /// Create a tree node based on a IWFModelElement
        /// </summary>
        /// <param name="element">An IWFModelElement object.</param>
        /// <returns>A TreeNode instance</returns>
        public static ModelElementTreeNode CreateTreeNode(IWFModelElement element)
        {
            ModelElementTreeNode treeNode = new ModelElementTreeNode(element);

            switch (element.ElementType)
            {
                case ElementType.Project:
                    treeNode.Text = element.Name + " " + ((ProjectModel) element).Version;
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 0;
                    break;

                case ElementType.Workflow:
                    treeNode.Text = element.Name;
                    switch (((WorkflowModel)element).WorkflowType)
                    {
                        case WorkflowType.Sequential:
                            treeNode.ImageIndex = 1;
                            treeNode.SelectedImageIndex = 1;
                            if (((WorkflowModel)element).ID != null)
                            {
                                treeNode.ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.ExecutableSequential");
                            }
                            else
                            {
                                treeNode.ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.NonExecutableSequential");
                            }
                            break;

                        case WorkflowType.StateMachine:
                            treeNode.ImageIndex = 2;
                            treeNode.SelectedImageIndex = 2;
                            if (((WorkflowModel)element).ID != null)
                            {
                                treeNode.ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.ExecutableStateMachine");
                            }
                            else
                            {
                                treeNode.ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.NonExecutableStateMachine");
                            }

                            break;

                        case WorkflowType.Wizard:
                            treeNode.ImageIndex = 3;
                            treeNode.SelectedImageIndex = 3;
                            if (((WorkflowModel)element).ID != null)
                            {
                                treeNode.ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.ExecutableWizard");
                            }
                            else
                            {
                                treeNode.ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.NonExecutableWizard");
                            }
                            break;
                    }

                    break;
            }

            return treeNode;
        }
    }
}