/*
* @(#)WorkflowStudioControl.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.Activities;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using System.CodeDom;
using System.CodeDom.Compiler;

using Newtera.WFModel;
using Newtera.Activities;
using Newtera.Common.MetaData.XaclModel;

namespace Newtera.WorkflowStudioControl
{
    public partial class WorkflowStudioControl : UserControl, IDisposable, IServiceProvider, ISite
    {
        public event EventHandler AddWorkflowEvent;
        public event EventHandler TabViewChanged;
        public event EventHandler StartWorkflowEvent;
        public event EventHandler MonitorWorkflowsEvent;
        public event EventHandler SetAccessControlEvent;

        private ProjectModel _project;
        private WorkflowView _workflowView;
        private DesignSurface _designSurface;
        private WorkflowLoader _loader;
        private ToolboxService _toolbox;
        private string _typeName;
        private string _nameSpace;
        private TreeNode _selectedModelTreeNode;
        private WorkflowModel _workflowModel;
        private MenuItemStates _menuItemStates;

        public WorkflowStudioControl()
        {
            _project = null;
            _workflowModel = null;
            _menuItemStates = new MenuItemStates();

            _menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

            InitializeComponent();

            _toolbox = new ToolboxService(this);
            this.activitiesPanel.Controls.Add(_toolbox);
            _toolbox.Dock = DockStyle.Fill;
            _toolbox.BackColor = BackColor;
            _toolbox.Font = WorkflowTheme.CurrentTheme.AmbientTheme.Font;

            WorkflowTheme.CurrentTheme.ReadOnly = false;
            WorkflowTheme.CurrentTheme.AmbientTheme.ShowConfigErrors = false;
            WorkflowTheme.CurrentTheme.ReadOnly = true;

            this.propertyGrid.BackColor = BackColor;
            this.propertyGrid.Font = WorkflowTheme.CurrentTheme.AmbientTheme.Font;
            this.propertyGrid.Site = this;
        }

        /// <summary>
        /// Gets the menu item states
        /// </summary>
        public MenuItemStates MenuItemStates
        {
            get
            {
                return this._menuItemStates;
            }
        }

        /// <summary>
        /// Gets or sets the project model
        /// </summary>
        public ProjectModel Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;

                // keep the current project in a global context to be used by workflow activity
                ProjectModelContext.Instance.Project = value;
            }
        }

        /// <summary>
        /// Gets or sets the NameSpace
        /// </summary>
        public string NameSpace
        {
            get
            { 
                return _nameSpace; 
            }
            set
            {
                _nameSpace = value;
            }
        }

        /// <summary>
        /// Gets or sets the TypeName
        /// </summary>
        public string TypeName
        {
            get
            {
                return _typeName; 
            }
            set 
            { 
                _typeName = value;
            }
        }

        /// <summary>
        /// Get the current selected workflow model
        /// </summary>
        public WorkflowModel CurrentWorkflowModel
        {
            get
            {
                if (this._selectedModelTreeNode != null &&
                    ((ModelElementTreeNode)_selectedModelTreeNode).ModelElement is WorkflowModel)
                {
                    return (WorkflowModel)((ModelElementTreeNode)_selectedModelTreeNode).ModelElement;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the workflow view
        /// </summary>
        public WorkflowView WorkflowView
        {
            get
            {
                return this._workflowView;
            }
        }

        /// <summary>
        /// Unload the workflow
        /// </summary>
        /// <param name="disposing">true if to dispose.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                UnloadWorkflow();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the service from the workflow view
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>A Service</returns>
        new public object GetService(Type serviceType)
        {
            return (this._workflowView != null) ? ((IServiceProvider)this._workflowView).GetService(serviceType) : null;
        }

        /// <summary>
        /// Handle onLoadEvent
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //ShowDefaultWorkflow();
        }

        /// <summary>
        /// Create a brand new project from scratch
        /// </summary>
        public void CreateNewProject()
        {
            this.Project = new ProjectModel(MessageResourceManager.GetString("WorkflowStudioControl.DefaultName"));

            // show the empty project in the project tab
            DisplayProject();

            InitializeMenuItemStates();
        }

        /// <summary>
        /// Open a project saved in a file
        /// </summary>
        /// <param name="projectFilePath">The project file path</param>
        public void OpenProject(string projectFilePath)
        {
            this.Project = new ProjectModel("");
            this.Project.Read(projectFilePath);

            XaclPolicy xaclPolicy = this.Project.Policy;
            string xaclFileName = projectFilePath.Replace(".wfproj", ".xacl");
            xaclPolicy.Read(xaclFileName);

            // Set the type of data sources of the workflow models, so that workflow
            // models can load data on demand.
            string baseDir = Path.GetDirectoryName(projectFilePath);
            foreach (WorkflowModel workflowModel in this.Project.Workflows)
            {
                workflowModel.XomlFileBaseDir = baseDir;
                workflowModel.SourceStorageType = StorageType.File;
            }

            // also clear the IDs that may have been saved to the file since
            // the IDs will be assigned by the server when the project is saved to
            // the database
            this.Project.ClearIds();

            DisplayProject();

            InitializeMenuItemStates();
        }

        /// <summary>
        /// Open a project provided
        /// </summary>
        /// <param name="projectModel">The project model</param>
        public void OpenProject(ProjectModel projectModel)
        {
            this.Project = projectModel;

            DisplayProject();

            InitializeMenuItemStates();
        }

        /// <summary>
        /// Load a workflow from a workflow model into the designer
        /// </summary>
        /// <param name="workflowModel">An WorkflowModel instance</param>
        private void LoadWorkflow(WorkflowModel workflowModel)
        {
            SuspendLayout();

            DesignSurface designSurface = new DesignSurface();
            WorkflowLoader loader = new WorkflowLoader();
            loader.WorkflowModel = workflowModel;
            designSurface.BeginLoad(loader);

            IDesignerHost designerHost = designSurface.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (designerHost != null && designerHost.RootComponent != null)
            {
                IRootDesigner rootDesigner = designerHost.GetDesigner(designerHost.RootComponent) as IRootDesigner;
                if (rootDesigner != null)
                {
                    // flush the previous workflow data
                    if (_loader != null)
                    {
                        this._loader.Flush();
                    }

                    UnloadWorkflow();

                    this._designSurface = designSurface;
                    this._loader = loader;
                    this._workflowView = rootDesigner.GetView(ViewTechnology.Default) as WorkflowView;
                    this.workflowStudioSplitContainer.Panel2.Controls.Add(this._workflowView);
                    this._workflowView.Dock = DockStyle.Fill;
                    this._workflowView.TabIndex = 1;
                    this._workflowView.TabStop = true;
                    this._workflowView.HScrollBar.TabStop = false;
                    this._workflowView.VScrollBar.TabStop = false;
                    this._workflowView.Focus();
                    this.propertyGrid.Site = designerHost.RootComponent.Site;

                    ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
                    if (selectionService != null)
                        selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
                }
                else
                {
                    throw new ApplicationException("Unable to create design designer.");
                }
            }
            else
            {
                throw new ApplicationException("Unable to create designHost or its root component.");
            }

            ResumeLayout(true);

            // Add the code compile unit for the xaml file
            /*
            TypeProvider typeProvider = (TypeProvider)GetService(typeof(ITypeProvider));
            this._loader.XamlCodeCompileUnit = new CodeCompileUnit();
            this._loader.XamlCodeCompileUnit.Namespaces.Add(Helpers.GenerateCodeFromXomlDocument(workflowModel.CreateRootActivity(), this, ref this._nameSpace, ref this._typeName));
            typeProvider.AddCodeCompileUnit(this._loader.XamlCodeCompileUnit);
            */

            // remember the current workflow model
            this._workflowModel = workflowModel;

            if (workflowModel.RootActivity is INewteraWorkflow)
            {
                INewteraWorkflow rootActivity = (INewteraWorkflow)workflowModel.RootActivity;

                // register the event handler for StartEventChanged event
                rootActivity.StartEventChanged += new EventHandler(workflowStudioControl_StartEventChanged);
            }
        }

        /// <summary>
        /// Unload the workflow from the designer
        /// </summary>
        private void UnloadWorkflow()
        {
            IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (designerHost != null && designerHost.Container.Components.Count > 0)
                WorkflowLoader.DestroyObjectGraphFromDesignerHost(designerHost, designerHost.RootComponent as Activity);

            ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
            if (selectionService != null)
                selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);

            if (this._designSurface != null)
            {
                this._designSurface.Dispose();
                this._designSurface = null;
            }

            if (this._workflowView != null)
            {
                Controls.Remove(this._workflowView);
                this._workflowView.Dispose();
                this._workflowView = null;
            }

            if (_workflowModel != null && _workflowModel.RootActivity != null &&
                _workflowModel.RootActivity is INewteraWorkflow)
            {
                INewteraWorkflow rootActivity = (INewteraWorkflow)_workflowModel.RootActivity;
                rootActivity.StartEventChanged -= new EventHandler(this.workflowStudioControl_StartEventChanged);
            }
        }

        /// <summary>
        /// show the workflow in the designer windows for the given workflow model.
        /// </summary>
        public void ShowWorkflow(WorkflowModel workflowModel)
        {
            try
            {
                //this.Cursor = Cursors.WaitCursor;

                if (workflowModel.RootActivity == null && workflowModel.SourceStorageType == StorageType.Unknown)
                {
                    // this is a brand new workflow model, create a root activity
                    // based on the workflow type
                    XmlWriter xmlWriter;
                    WorkflowMarkupSerializer serializer;

                    switch (workflowModel.WorkflowType)
                    {
                        case WorkflowType.Sequential:
                            NewteraSequentialWorkflowActivity sequentialWorkflow = new NewteraSequentialWorkflowActivity();
                            //SequentialWorkflowActivity sequentialWorkflow = new SequentialWorkflowActivity();
                            sequentialWorkflow.Name = workflowModel.Name;
                            //sequentialWorkflow.SetValue(WorkflowMarkupSerializer.XClassProperty, workflowModel.QualifiedWorkflowClass);
                            workflowModel.RootActivity = sequentialWorkflow;
                            xmlWriter = workflowModel.CreateXomlWriter();
                            serializer = new WorkflowMarkupSerializer();
                            serializer.Serialize(xmlWriter, sequentialWorkflow);
                            xmlWriter.Close();
                            break;

                        case WorkflowType.StateMachine:
                            StateMachineWorkflowActivity stateMachineWorkflow = new NewteraStateMachineWorkflowActivity();
                            stateMachineWorkflow.Name = workflowModel.Name;
                            //stateMachineWorkflow.SetValue(WorkflowMarkupSerializer.XClassProperty, workflowModel.QualifiedWorkflowClass);
                            workflowModel.RootActivity = stateMachineWorkflow;
                            xmlWriter = workflowModel.CreateXomlWriter();
                            serializer = new WorkflowMarkupSerializer();
                            serializer.Serialize(xmlWriter, stateMachineWorkflow);
                            xmlWriter.Close();
                            break;

                        case WorkflowType.Wizard:
                            NewteraWizardWorkflowActivity wizardWorkflow = new NewteraWizardWorkflowActivity();
                            wizardWorkflow.Name = workflowModel.Name;
                            workflowModel.RootActivity = wizardWorkflow;
                            xmlWriter = workflowModel.CreateXomlWriter();
                            serializer = new WorkflowMarkupSerializer();
                            serializer.Serialize(xmlWriter, wizardWorkflow);
                            xmlWriter.Close();
                            break;
                    }
                }

                LoadWorkflow(workflowModel);
            }
            finally
            {
                //Cursor.Current = this.Cursor;
            }
        }

        /// <summary>
        /// show the tab view in navigation tab control
        /// </summary>
        /// <param name="viewType"></param>
        public void ShowTabView(NavigationTabView viewType)
        {
            switch (viewType)
            {
                case NavigationTabView.Project:
                    this.navigateTabControl.SelectedTab = this.projectTabPage;
                    break;

                case NavigationTabView.Properties:
                    this.navigateTabControl.SelectedTab = this.propertiesTabPage;
                    break;

                case NavigationTabView.Activities:
                    this.navigateTabControl.SelectedTab = this.activitiesTabPage;
                    break;
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
            if (selectionService != null)
                this.propertyGrid.SelectedObjects = new ArrayList(selectionService.GetSelectedComponents()).ToArray();
        }


        public void InvokeStandardCommand(CommandID cmd)
        {
            IMenuCommandService menuService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            if (menuService != null)
                menuService.GlobalInvoke(cmd);
        }

        /// <summary>
        /// Save the project and its workflows
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveProject(string filePath)
        {
            if (this.Project != null)
            {
                // write project hierarchy to the file
                this.Project.Write(filePath);

                // flush the workflow that is currently display at the designer
                this.Flush();

                // write workflow's xoml, rules, layout data to separate files
                string baseDir = Path.GetDirectoryName(filePath);
                foreach (WorkflowModel workflowModel in this.Project.Workflows)
                {
                    workflowModel.Save(baseDir);
                }

                // save the xacl policy to a file
                string xaclPolicyFileName = filePath.Replace(".wfproj", ".xacl");
                XaclPolicy xaclPolicy = this.Project.Policy;
                xaclPolicy.Write(xaclPolicyFileName);

                this.Project.NeedToSave = false;
            }
        }

        /// <summary>
        /// Validate the workflows in the project, return validation error collection or null if
        /// there isn't any validation errors.
        /// </summary>
        public ValidationErrorCollection ValidateWorkflows()
        {
            ValidationErrorCollection errors = null;
            if (this.Project != null)
            {
                this.Flush();

                using (WorkflowRuntime workflowRunTime = new WorkflowRuntime())
                {
                    // add custom types
                    TypeProvider typeProvider = new TypeProvider(null);
                    typeProvider.AddAssemblyReference("Newtera.Activities.dll");
                    workflowRunTime.AddService(typeProvider);

                    foreach (WorkflowModel workflowModel in this.Project.Workflows)
                    {
                        XmlReader xomlReader, rulesReader;

                        try
                        {
                            if (workflowModel.HasRules)
                            {
                                xomlReader = workflowModel.CreateXomlReader();
                                rulesReader = workflowModel.CreateRulesXmlReader();

                                workflowRunTime.CreateWorkflow(xomlReader, rulesReader, null);
                            }
                            else
                            {
                                xomlReader = workflowModel.CreateXomlReader();
                                WorkflowInstance wfInstance = workflowRunTime.CreateWorkflow(xomlReader);
                            }
                        }
                        catch (WorkflowValidationFailedException ex)
                        {
                            errors = ex.Errors;
                            foreach (ValidationError error in errors)
                            {
                                error.UserData["Workflow"] = workflowModel.Name;
                            }

                            break;
                        }
                        catch (Exception ex2)
                        {
                            errors = new ValidationErrorCollection();
                            ValidationError err = new ValidationError(ex2.Message, 0);
                            err.UserData["Workflow"] = workflowModel.Name;
                            errors.Add(err);
                            break;
                        }
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Display the project tree
        /// </summary>
        public void DisplayProjectTree()
        {
            ModelElementTreeNode root = ModelTreeBuilder.BuildTree(this.Project);

            this.projectTreeView.BeginUpdate();
            this.projectTreeView.Nodes.Clear();
            this.projectTreeView.Nodes.Add(root);

            this.projectTreeView.EndUpdate();

            // Select the first workflow if the project contains children
            if (root.Nodes.Count > 0)
            {
                this.projectTreeView.SelectedNode = root.Nodes[0];
            }
        }

        /// <summary>
        /// Update the project tree node's tooltips
        /// </summary>
        public void UpdateProjectTree()
        {
            this.projectTreeView.BeginUpdate();

            if (this.projectTreeView.Nodes.Count > 0)
            {
                TreeNode root = this.projectTreeView.Nodes[0];
                int index = 0;

                foreach (WorkflowModel workflowModel in _project.Workflows)
                {
                    if (workflowModel.ID != null)
                    {
                        if (workflowModel.WorkflowType == WorkflowType.Sequential)
                        {
                            (root.Nodes[index]).ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.ExecutableSequential");
                        }
                        else if (workflowModel.WorkflowType == WorkflowType.StateMachine)
                        {
                            (root.Nodes[index]).ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.ExecutableStateMachine");
                        }
                        else if (workflowModel.WorkflowType == WorkflowType.Wizard)
                        {
                            (root.Nodes[index]).ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.Wizard");
                        }
                    }
                    else
                    {
                        if (workflowModel.WorkflowType == WorkflowType.Sequential)
                        {
                            (root.Nodes[index]).ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.NonExecutableSequential");
                        }
                        else if (workflowModel.WorkflowType == WorkflowType.StateMachine)
                        {
                            (root.Nodes[index]).ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.NonExecutableStateMachine");
                        }
                        else if (workflowModel.WorkflowType == WorkflowType.Wizard)
                        {
                            (root.Nodes[index]).ToolTipText = MessageResourceManager.GetString("WorkflowStudioControl.NonExecutableWizard");
                        }
                    }

                    index++;
                }
            }

            this.projectTreeView.EndUpdate();
        }

        /// <summary>
        /// Flush the current display workflow to memory.
        /// We also refresh the type provider when we save the file
        /// </summary>
        public void Flush()
        {
            if (this._loader != null)
            {
                this._loader.Flush();
            }

            // Referesh the code compile unit every time the file is saved
            /*
            TypeProvider typeProvider = (TypeProvider)GetService(typeof(ITypeProvider));
            typeProvider.RemoveCodeCompileUnit(this._loader.XamlCodeCompileUnit);
            this._loader.XamlCodeCompileUnit = new CodeCompileUnit();
            this._loader.XamlCodeCompileUnit.Namespaces.Add(Helpers.GenerateCodeFromXomlDocument(_workflowModel.CreateRootActivity(), this, ref this._nameSpace, ref this._typeName));
            typeProvider.AddCodeCompileUnit(this._loader.XamlCodeCompileUnit);
            */
        }

        /// <summary>
        /// Display the project in the project tab
        /// </summary>
        private void DisplayProject()
        {
            if (this.Project != null)
            {
                //  unload the workflow currently displayed in the designer
                this.UnloadWorkflow();

                // display project tree
                DisplayProjectTree();

                // show the project tab on top
                this.navigateTabControl.SelectedTab = this.projectTabPage;
            }
        }

        /// <summary>
        /// We listen to the event when the property browser gets the focus and save the xaml
        /// Reason: This is done to support using activities in the rule set editor. In VS, the xaml file is 
        /// flushed when the application is idle or when you save or change the view to new file. Only after the
        /// file is flushed, the type provider is updated with the contents of the xaml file. Hence in this case
        /// we listen to the event where the property grid receives the focus and do a save. This will update the
        /// type provider with the contents of the xaml file (i.e the activities present in the xaml) and the activities
        /// can now be used in the rule editor which works off the type provider. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void propertyGrid_GotFocus(object sender, System.EventArgs e)
        {
            if (_workflowModel != null)
            {
                this.Flush();
            }
        }

        /// <summary>
        /// Add a WorkflowModel to the project
        /// </summary>
        /// <param name="name">The name of workflow model</param>
        /// <param name="workflowType">The workflow type</param>
        public void AddWorkflowModel(string name, WorkflowType workflowType)
        {
            // make sure the name doesn't exists
            int index = 1;
            IWFModelElement existing = this.Project.Workflows[name];
            while (existing != null)
            {
                // add index at the end of name until it is an unique name
                name = name + index;
                existing = this.Project.Workflows[name];
                index++;
            }

            WorkflowModel wfModel = new WorkflowModel(name, workflowType);
            this.Project.AddWorkflowModel(wfModel);

            // Add the new node to the tree view
            ModelElementTreeNode newNode = ModelTreeBuilder.CreateTreeNode(wfModel);

            // add the new tree node to a position in project tree view
            AddToProjectTree(newNode);

            // select the new node
            this.projectTreeView.SelectedNode = newNode;
        }

        /// <summary>
        /// Delete the selected workflow model from the project.
        /// </summary>
        public void DeleteWorkflowModel()
        {
            if (_selectedModelTreeNode != null)
            {
                WorkflowModel workflowModel = ((ModelElementTreeNode) _selectedModelTreeNode).ModelElement as WorkflowModel;

                if (workflowModel != null)
                {
                    this.Project.Workflows.Remove(workflowModel);

                    if (projectTreeView.SelectedNode == _selectedModelTreeNode)
                    {
                        this.UnloadWorkflow(); // unload the workflow from the designer
                    }

                    this.projectTreeView.BeginUpdate();

                    TreeNode parent = this._selectedModelTreeNode.Parent;
                    parent.Nodes.Remove(_selectedModelTreeNode);

                    this.projectTreeView.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Add a child tree node at the end of project's children list
        /// </summary>
        /// <param name="child">The child tree node to be added.</param>
        private void AddToProjectTree(ModelElementTreeNode child)
        {
            if (this.projectTreeView.Nodes.Count > 0)
            {
                ModelElementTreeNode parent = (ModelElementTreeNode)this.projectTreeView.Nodes[0];
                child.ModelElement.Position = parent.Nodes.Count;
                parent.Nodes.Add(child);
            }
        }

        private void MenuItemStateChanged(object sender, System.EventArgs e)
        {
            StateChangedEventArgs args = (StateChangedEventArgs)e;

            // set the toolbar button states
            switch (args.ID)
            {
                case MenuItemID.EditAdd:
                    this.AddWorkflowStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditDelete:
                    this.DeleteWorkflowMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.EditRename:
                    this.RenameStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.WorkflowStart:
                    this.startWorkflowToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ViewWorkflowMonitor:
                    this.monitorWorkflowsToolStripMenuItem.Enabled = args.State;
                    break;
                case MenuItemID.ToolAccessControl:
                    this.AccessControlToolStripMenuItem.Enabled = args.State;
                    break;
            }
        }

        /// <summary>
        /// Initialize the menu item states
        /// </summary>
        private void InitializeMenuItemStates()
        {
            _menuItemStates.SetState(MenuItemID.EditAdd, true);
            _menuItemStates.SetState(MenuItemID.EditCopy, true);
            _menuItemStates.SetState(MenuItemID.EditCut, true);
            _menuItemStates.SetState(MenuItemID.EditDelete, false);
            _menuItemStates.SetState(MenuItemID.EditPaste, false);
            _menuItemStates.SetState(MenuItemID.WorkflowStart, false);
            _menuItemStates.SetState(MenuItemID.ViewWorkflowMonitor, false);
            _menuItemStates.SetState(MenuItemID.FileSaveAsFile, true);
            _menuItemStates.SetState(MenuItemID.FileSaveFile, true);
            _menuItemStates.SetState(MenuItemID.ToolUnlock, true);
            _menuItemStates.SetState(MenuItemID.ToolAccessControl, false);
        }

        /// <summary>
        /// Show activities of the selected category
        /// </summary>
        private void ShowActivityCategory()
        {
            if (this.categoryComboBox.SelectedIndex != -1)
            {
                switch (this.categoryComboBox.SelectedIndex)
                {
                    case 0:
                        this._toolbox.SelectedCategory = ToolboxService.CATEGORY_ALL;
                        break;

                    case 1:
                        this._toolbox.SelectedCategory = ToolboxService.CATEGORY_GENERAL;
                        break;
                    case 2:
                        this._toolbox.SelectedCategory = ToolboxService.CATEGORY_CUSTOM;
                        break;
                }
            }
        }

        #region ISite Members

        public IComponent Component
        {
            get { return this; }
        }

        public new bool DesignMode
        {
            get { return true; }
        }

        #endregion

        #region event handler

        private void RenameStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedModelTreeNode != null)
            {
                projectTreeView.SelectedNode = _selectedModelTreeNode;
                projectTreeView.LabelEdit = true;
                if (!_selectedModelTreeNode.IsEditing)
                {
                    _selectedModelTreeNode.BeginEdit();
                }
            }
        }

        /// <summary>
        /// Get the tree node under the mouse pointer and 
        /// save it in the _selectedModelTreeNode variable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectTreeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _selectedModelTreeNode = this.projectTreeView.GetNodeAt(e.X, e.Y);

            ModelElementTreeNode modelTreeNode = (ModelElementTreeNode)_selectedModelTreeNode;
            if (modelTreeNode != null)
            {
                if (modelTreeNode.ModelElement is ProjectModel)
                {
                    _menuItemStates.SetState(MenuItemID.ToolAccessControl, true);
                    _menuItemStates.SetState(MenuItemID.EditAdd, true);
                    _menuItemStates.SetState(MenuItemID.EditDelete, false);
                    _menuItemStates.SetState(MenuItemID.WorkflowStart, false);
                    _menuItemStates.SetState(MenuItemID.ViewWorkflowMonitor, false);
                }
                else if (modelTreeNode.ModelElement is WorkflowModel)
                {
                    _menuItemStates.SetState(MenuItemID.ToolAccessControl, false);
                    _menuItemStates.SetState(MenuItemID.EditAdd, false);
                    _menuItemStates.SetState(MenuItemID.EditDelete, true);

                    if (this.Project.IsLoadedFromDB && ((WorkflowModel) modelTreeNode.ModelElement).ID != null)
                    {
                        _menuItemStates.SetState(MenuItemID.WorkflowStart, true);
                        _menuItemStates.SetState(MenuItemID.ViewWorkflowMonitor, true);
                        _menuItemStates.SetState(MenuItemID.EditRename, false);
                    }
                    else
                    {
                        _menuItemStates.SetState(MenuItemID.WorkflowStart, false);
                        _menuItemStates.SetState(MenuItemID.ViewWorkflowMonitor, false);
                        _menuItemStates.SetState(MenuItemID.EditRename, true);
                    }
                }

                if (this.Project.IsLoadedFromDB)
                {
                    _menuItemStates.SetState(MenuItemID.EditRename, false);
                }
                else
                {
                    _menuItemStates.SetState(MenuItemID.EditRename, true);
                }
            }
        }

        private void projectTreeView_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        ModelElementTreeNode modelTreeNode = (ModelElementTreeNode)_selectedModelTreeNode;
                        if (modelTreeNode.ModelElement is WorkflowModel)
                        {
                            // make sure that the label is not duplicated among the workflows
                            WorkflowModel workflow = (WorkflowModel)this.Project.Workflows[e.Label];
                            if (workflow != null && workflow != modelTreeNode.ModelElement)
                            {
                                /* Cancel the label edit action, inform the user that the name has already
                                 * exists, and place the node in edit mode again. */
                                e.CancelEdit = true;
                                MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioControl.DuplicateLabel"),
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                e.Node.BeginEdit();
                            }
                            else
                            {
                                // Stop editing without canceling the label change.
                                e.Node.EndEdit(false);

                                // set the label to the selected tree node
                                ((ModelElementTreeNode)_selectedModelTreeNode).ModelElement.Name = e.Label;
                            }
                        }
                        else
                        {
                            // Stop editing without canceling the label change.
                            e.Node.EndEdit(false);

                            // set the label to the selected tree node
                            ((ModelElementTreeNode)_selectedModelTreeNode).ModelElement.Name = e.Label;
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioControl.InvalidLabelChar"),
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowStudioControl.EmptyLabel"),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Node.BeginEdit();
                }
                this.projectTreeView.LabelEdit = false;
            }
        }

        private void AddWorkflowStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.AddWorkflowEvent != null)
            {
                AddWorkflowEvent(sender, e);
            }
        }

        private void projectTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get the selected node
            ModelElementTreeNode node = (ModelElementTreeNode)e.Node;

            if (node != null && node.ModelElement is WorkflowModel)
            {
                // show the selected workflow in the designer window
                this.ShowWorkflow((WorkflowModel)node.ModelElement);

                this._menuItemStates.SetState(MenuItemID.EditAdd, false);
            }
            else
            {
                // clear the design surface
                // flush the previous workflow data
                if (_loader != null)
                {
                    this._loader.Flush();
                }
                this.UnloadWorkflow();
            }
        }

        private void navigateTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabViewChanged != null)
            {
                NavigationTabView viewType;
                if (navigateTabControl.SelectedTab == this.projectTabPage)
                {
                    viewType = NavigationTabView.Project;
                }
                else if (navigateTabControl.SelectedTab == this.propertiesTabPage)
                {
                    viewType = NavigationTabView.Properties;
                }
                else
                {
                    viewType = NavigationTabView.Activities;
                }

                TabViewChanged(sender, new TabViewChangedEventArgs(viewType));
            }
        }

        private void DeleteWorkflowMenuItem_Click(object sender, EventArgs e)
        {
            DeleteWorkflowModel();
        }

        private void categoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowActivityCategory();
        }

        private void monitorWorkflowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MonitorWorkflowsEvent != null)
            {
                MonitorWorkflowsEvent(sender, e);
            }
        }

        private void startWorkflowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.StartWorkflowEvent != null)
            {
                StartWorkflowEvent(sender, e);
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object theInstance = this.propertyGrid.SelectedObject;
            if (theInstance != null)
            {
                PropertyDescriptor descriptor = this.propertyGrid.SelectedGridItem.PropertyDescriptor;
                descriptor.ResetValue(theInstance);
                this.propertyGrid.Refresh();
            }
        }

        private void descriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // make the help area visible if it is not, otherwise, invisible
            this.propertyGrid.HelpVisible = !(this.descriptionToolStripMenuItem.Checked);
            this.descriptionToolStripMenuItem.Checked = !(this.descriptionToolStripMenuItem.Checked);
        }

        private void AccessControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SetAccessControlEvent != null)
            {
                SetAccessControlEvent(sender, e);
            }
        }

        private void workflowStudioControl_StartEventChanged(object sender, EventArgs e)
        {
            // keep the start event in WorkflowModel in sync with the root activity
            INewteraWorkflow rootActivity = (INewteraWorkflow)_workflowModel.RootActivity;
            _workflowModel.StartEvent.SchemaID = rootActivity.SchemaId;
            _workflowModel.StartEvent.ClassCaption = rootActivity.ClassCaption;
            _workflowModel.StartEvent.ClassName = rootActivity.ClassName;
            _workflowModel.StartEvent.EventName = rootActivity.EventName;
            _workflowModel.IsAltered = true;
        }

        private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            object activity = this.propertyGrid.SelectedObject;

            if (activity != null && activity is Activity)
            {
                Type type = activity.GetType();
                this.activityTypeTextBox.Text = type.Name;
                ActivityCache.Instance.CurrentActivity = (Activity)activity;
            }
            else
            {
                this.activityTypeTextBox.Text = "";
            }
        }

        #endregion
    }

    /// <summary>
    /// Specify the types of tab view in navigation tab control
    /// </summary>
    public enum NavigationTabView
    {
        Project,
        Properties,
        Activities
    }

    public class TabViewChangedEventArgs : EventArgs
    {
        private NavigationTabView _viewType;

        public TabViewChangedEventArgs(NavigationTabView viewType)
        {
            _viewType = viewType;
        }

        public NavigationTabView ViewType
        {
            get
            {
                return _viewType;
            }
        }
    }
}