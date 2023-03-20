/*
* @(#)ViewHost.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;

namespace Newtera.WorkflowMonitor
{
    /// <summary>
    /// ViewHost "hosts" the workflow designer and graphically displays the workflow definition
    /// </summary>
    /// <version> 1.0.0 03 Jan 2006</version>
    internal class ViewHost : Control
    {
        private Loader loader = null;
        private WorkflowDesignSurface surface = null;
        private WorkflowMonitorControl parentValue;

        private ISelectionService theSelectionService;

        private WorkflowView workflowViewValue = null;

        internal event EventHandler<ZoomChangedEventArgs> ZoomChanged;

        internal event EventHandler ActivitySelected;

        internal ViewHost(WorkflowMonitorControl parent)
        {
            this.parentValue = parent;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Dock = DockStyle.Fill;
            this.Name = "viewHost";

            Initialize();

            SuspendLayout();
            ResumeLayout(true);

            this.BackColor = SystemColors.Control;
        }

        //Expand or collapse all composite activities
        internal void Expand(bool expand)
        {
            IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null)
                return;

            this.SuspendLayout();

            CompositeActivity root = host.RootComponent as CompositeActivity;
            foreach (Activity activity in root.Activities)
            {
                CompositeActivityDesigner compositeActivityDesigner = host.GetDesigner((IComponent)activity) as CompositeActivityDesigner;
                if (compositeActivityDesigner != null)
                {
                    compositeActivityDesigner.Expanded = expand;
                }
            }

            this.ResumeLayout(true);
        }

        internal WorkflowView WorkflowView
        {
            get
            {
                return this.workflowViewValue;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Clear();

            base.Dispose(disposing);
        }

        protected override object GetService(Type serviceType)
        {
            if (this.surface != null)
                return this.surface.GetService(serviceType);
            else
                return null;
        }

        //Loads the workflow definition into the designer 
        internal void OpenWorkflow(Activity workflowDefinition)
        {
            Initialize();

            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host == null)
                return;

            if (this.workflowViewValue != null)
            {
                if(Controls.Contains(this.workflowViewValue))
                    Controls.Remove(this.workflowViewValue);
                this.workflowViewValue.Dispose();
                this.workflowViewValue = null;
            }

            loader.WorkflowModel = parentValue.WorkflowModel;
            this.surface.BeginLoad(this.loader);

            this.workflowViewValue = new WorkflowView(this.surface);
            workflowViewValue.ZoomChanged += new EventHandler(workflowViewValue_ZoomChanged);
            IDesignerGlyphProviderService glyphService = this.surface.GetService(typeof(IDesignerGlyphProviderService)) as IDesignerGlyphProviderService;
            WorkflowMonitorDesignerGlyphProvider glyphProvider = new WorkflowMonitorDesignerGlyphProvider(parentValue.ActivityStatusList);
            glyphService.AddGlyphProvider(glyphProvider);

            workflowViewValue.Dock = DockStyle.Fill;
            Controls.Add(workflowViewValue);

            ((IDesignerLoaderHost)host).EndLoad(host.RootComponent.Site.Name, true, null);
        }

        //Initializes the designer setting up the services, surface, and loader
        private void Initialize()
        {
            if (this.surface != null)
                this.surface.Dispose();

            this.loader = new Loader();
            this.surface = new WorkflowDesignSurface(new MemberCreationService());

            this.theSelectionService = (ISelectionService)surface.GetService(typeof(ISelectionService));
            if (theSelectionService != null)
            {
                this.theSelectionService.SelectionChanged += new EventHandler(theSelectionService_SelectionChanged);
            }
        }

        private void Clear()
        {
            if (this.surface == null)
                return;

            if (this.workflowViewValue != null)
            {
                if (Controls.Contains(this.workflowViewValue))
                    Controls.Remove(this.workflowViewValue);
                this.workflowViewValue.Dispose();
                this.workflowViewValue = null;
            }

            this.surface.Dispose();
            this.surface = null;
            this.loader = null;
        }

        void workflowViewValue_ZoomChanged(object sender, EventArgs e)
        {
            ZoomChanged(null, new ZoomChangedEventArgs(workflowViewValue.Zoom));
        }

        internal void HighlightActivity(String activityName)
        {
            ISelectionService selectionService = (ISelectionService)surface.GetService(typeof(ISelectionService));
            IReferenceService referenceService = (IReferenceService)surface.GetService(typeof(IReferenceService));
            if (selectionService != null && referenceService != null)
            {
                Activity activityComponent = (Activity)referenceService.GetReference(activityName);
                if (activityComponent != null)
                    selectionService.SetSelectedComponents(new IComponent[] { activityComponent });
            }
        }

        internal Activity GetHighlightActivity()
        {
            Activity activity = null;

            if (this.theSelectionService != null)
            {
                if (theSelectionService.PrimarySelection != null &&
                    theSelectionService.PrimarySelection is Activity)
                {
                    activity = (Activity)theSelectionService.PrimarySelection;
                }
            }

            return activity;
        }

        internal void theSelectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (this.theSelectionService != null)
            {
                if (theSelectionService.PrimarySelection != null &&
                    theSelectionService.PrimarySelection is Activity)
                {
                    if (ActivitySelected != null)
                    {
                        ActivitySelected(this, new EventArgs());
                    }
                }
            }
        }

        private class MemberCreationService : IMemberCreationService
        {
            #region IMemberCreationService Members
            // Designer host requires an IMemberCreationService - we don't need this functionality
            // so all of the methods are blank

            void IMemberCreationService.CreateField(string className, string fieldName, Type fieldType, Type[] genericParameterTypes, MemberAttributes attributes, CodeSnippetExpression initializationExpression, bool overwriteExisting) { }
            void IMemberCreationService.CreateProperty(string className, string propertyName, Type propertyType, AttributeInfo[] attributes, bool emitDependencyProperty, bool isMetaProperty, bool isAttached, Type ownerType, bool isReadOnly) { }
            void IMemberCreationService.CreateEvent(string className, string eventName, Type eventType, AttributeInfo[] attributes, bool emitDependencyProperty) { }

            void IMemberCreationService.UpdateTypeName(string oldClassName, string newClassName) { }
            void IMemberCreationService.UpdateProperty(string className, string oldPropertyName, Type oldPropertyType, string newPropertyName, Type newPropertyType, AttributeInfo[] attributes, bool emitDependencyProperty, bool isMetaProperty) { }
            void IMemberCreationService.UpdateEvent(string className, string oldEventName, Type oldEventType, string newEventName, Type newEventType, AttributeInfo[] attributes, bool emitDependencyProperty, bool isMetaProperty) { }
            void IMemberCreationService.UpdateBaseType(string className, Type baseType) { }

            void IMemberCreationService.RemoveProperty(string className, string propertyName, Type propertyType) { }
            void IMemberCreationService.RemoveEvent(string className, string eventName, Type eventType) { }

            void IMemberCreationService.ShowCode(Activity activity, string methodName, Type delegateType) { }
            void IMemberCreationService.ShowCode() { }

            #endregion
        }

        internal class ZoomChangedEventArgs : EventArgs
        {
            private Int32 zoomValue;

            public Int32 Zoom
            {
                get { return zoomValue; }
            }
            public ZoomChangedEventArgs(Int32 zoom)
            {
                zoomValue = zoom;
            }
        }
    }
}
