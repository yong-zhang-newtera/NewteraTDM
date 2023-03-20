/*
* @(#)WorkflowLoader.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WorkflowStudioControl
{
	using System;
	using System.IO;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.ComponentModel.Design.Serialization;
	using System.Workflow.ComponentModel;
    using System.Workflow.ComponentModel.Compiler;
    using System.Workflow.ComponentModel.Design;
    using System.Workflow.ComponentModel.Serialization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing.Design;
    using System.Workflow.Activities;
    using System.Xml;
    using System.Text;
    using System.CodeDom;
    using Microsoft.CSharp;
    using System.CodeDom.Compiler;
    using System.Windows.Forms;

    using Newtera.WFModel;

    #region WorkflowLoader
    internal sealed class WorkflowLoader : WorkflowDesignerLoader
    {
        private const string DefaultNameSpace = "Newtera";

        #region Members

        private WorkflowModel _workflowModel = null;
        private StringBuilder tempRulesStream = null;
        private string _xomlFilePath = null;
        
        // we maintain two code compile units :1 for code beside file and 1 for xaml file
        private CodeCompileUnit codeBesideccu;
        private CodeCompileUnit xamlccu;
        #endregion

        #region ctor
        internal WorkflowLoader()
        {
            // Create the code compile unit 

            codeBesideccu = new CodeCompileUnit();
            codeBesideccu.ReferencedAssemblies.Add("System.Workflow.Activities");
            codeBesideccu.ReferencedAssemblies.Add("Newtera.WorkflowServices");
            CodeNamespace ns = new CodeNamespace(DefaultNameSpace);
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel.Design"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.ComponentModel.Design"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.ComponentModel"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.ComponentModel.Serialization"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.ComponentModel.Compiler"));
            ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.Activities"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.ComponentModel"));
            ns.Imports.Add(new CodeNamespaceImport("System.Workflow.Runtime"));
            codeBesideccu.Namespaces.Add(ns);
            CodeTypeDeclaration ctd = new CodeTypeDeclaration("Workflow1");
            ctd.BaseTypes.Add("System.Workflow.Activities.SequentialWorkflowActivity");
            ctd.IsPartial = true;
            ctd.Attributes = MemberAttributes.Public;
            ns.Types.Add(ctd);
        }

        #endregion

        #region Overrides from WorkflowDesignerLoader
        protected override void Initialize()
        {
            base.Initialize();

            // Add all the services to the loaderhost
            IDesignerLoaderHost host = LoaderHost;
            if (host != null)
            {
                this.SetBaseComponentClassName("Newtera.Workflow1");
                host.AddService(typeof(IMenuCommandService), new WorkflowMenuCommandService(host));
                host.AddService(typeof(IToolboxService), new ToolboxService(host));
                host.AddService(typeof(IExtendedUIService), new ExtendedUIService()); // to get rid of error when selecting an interface type for CallExternalMethodActivity
                TypeProvider typeProvider = new TypeProvider(host);
                typeProvider.AddCodeCompileUnit(this.CodeBesideCCU);
                typeProvider.AddAssemblyReference(typeof(System.EventHandler).Assembly.Location);
                typeProvider.AddAssemblyReference(typeof(System.ComponentModel.AttributeCollection).Assembly.Location);
                typeProvider.AddAssemblyReference(typeof(System.Workflow.ComponentModel.CompositeActivity).Assembly.Location);
                typeProvider.AddAssemblyReference(typeof(System.Workflow.Activities.SequentialWorkflowActivity).Assembly.Location);
                typeProvider.AddAssemblyReference("Newtera.Activities.dll");
                typeProvider.AddAssemblyReference("Newtera.WorkflowServices.dll");
                
                host.AddService(typeof(ITypeProvider), typeProvider, true);
                host.AddService(typeof(IMemberCreationService), new MemberCreationService(host, this));
                host.AddService(typeof(IPropertyValueUIService), new PropertyValueUIService());
                host.AddService(typeof(IEventBindingService), new EventBindingService(host,this));
            }
        }

        public override void Dispose()
        {
            IDesignerLoaderHost host = LoaderHost;
            if (host != null)
            {
                host.RemoveService(typeof(IIdentifierCreationService));
                host.RemoveService(typeof(IMenuCommandService));
                host.RemoveService(typeof(IToolboxService));
                host.RemoveService(typeof(ITypeProvider), true);
                host.RemoveService(typeof(IWorkflowCompilerOptionsService));
                host.RemoveService(typeof(IEventBindingService));
            }

            base.Dispose();
        }

        public override TextReader GetFileReader(string filePath)
        {
            if (this.tempRulesStream != null)
                return new StringReader(this.tempRulesStream.ToString());
            else
                return null;
        }

        public override TextWriter GetFileWriter(string filePath)
        {
            this.tempRulesStream = new StringBuilder();
            return new StringWriter(this.tempRulesStream);
        }

        public override string FileName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Note: In case of state machine workflows we need to load the layout from the layout file in the 
        /// OnEndLoad method. This is because the layout file is applied to the designer components which are
        /// created in PerformLoad and are available only on the OnEndLoad method
        /// </summary>
        /// <param name="successful"></param>
        /// <param name="errors"></param>
        protected override void OnEndLoad(bool successful, ICollection errors)
        {
            base.OnEndLoad(successful, errors);

            // Load the layout if it exists
            if (_workflowModel.HasLayout)
            {
                IList loaderrors = null;
                using (XmlReader xmlReader = _workflowModel.CreateLayoutReader())
                {
                    LoadDesignerLayout(xmlReader, out loaderrors);
                }
            }
        }

        /// <summary>
        /// Load the workflow : This will create the activity tree and its corresponding Designer tree
        /// </summary>
        /// <param name="serializationManager"></param>
        protected override void PerformLoad(IDesignerSerializationManager serializationManager)
        {
            base.PerformLoad(serializationManager);
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));

            // get the root activity and add the corresponding object graph to the designer host
            Activity rootActivity = _workflowModel.RootActivity;
            if (rootActivity == null)
            {
                // the root activity has not been created, create it
                rootActivity = _workflowModel.CreateRootActivity();
                // cache the root activity
                _workflowModel.RootActivity = rootActivity;
            }

            if (rootActivity != null)
            {
                if (designerHost != null)
                {
                    AddObjectGraphToDesignerHost(designerHost, rootActivity);
                    Type companionType = rootActivity.GetValue(WorkflowMarkupSerializer.XClassProperty) as Type;
                    if (companionType != null)
                        SetBaseComponentClassName(companionType.FullName);
                }
                else
                {
                    throw new ApplicationException("Unable to create a designerHost.");
                }
            }
            else
            {
                throw new ApplicationException("Unable to create a workflow root activity.");
            }

            designerHost.Activate();

            // Read from rules file if one exists
            if (_workflowModel.HasRules)
            {
                TextReader rulesReader = _workflowModel.CreateRulesTextReader();
                try
                {
                    this.tempRulesStream = new StringBuilder(rulesReader.ReadToEnd());
                }
                finally
                {
                    rulesReader.Close();
                }
            }
        }

        public override void Flush()
        {
            PerformFlush(null);
        }

        /// <summary>
        /// Save the Xaml file and flush the code compile unit into a .CS file
        /// </summary>
        /// <param name="manager"></param>
        protected override void PerformFlush(IDesignerSerializationManager manager)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            Activity rootActivity = null;
            if (host != null && host.RootComponent != null)
            {
                rootActivity = host.RootComponent as Activity;

                if (rootActivity != null)
                {
                    XmlWriter xmlWriter = _workflowModel.CreateXomlWriter();
                    try
                    {
                        WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();
                        xomlSerializer.Serialize(xmlWriter, rootActivity);
                    }
                    finally
                    {
                        xmlWriter.Close();
                    }
                }
            }
            
            // Flush CS code
            CSharpCodeProvider generator = new CSharpCodeProvider();

            // Just chooses some formatting options, like four space indenting
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.BracingStyle = "C";
            options.ElseOnClosing = false;
            options.IndentString = "    ";

            TextWriter writer = _workflowModel.CreateCodeWriter();
            try
            {
                generator.GenerateCodeFromCompileUnit(this.CodeBesideCCU, writer, options);
            }
            finally
            {
                writer.Close();
            }

            // Flush the rules file
            if (this.tempRulesStream != null)
            {
                using (TextWriter rulesWriter = _workflowModel.CreateRulesTextWriter())
                {
                    rulesWriter.Write(this.tempRulesStream.ToString());
                }
            }

            #region State Machine Workflow specific code

            // Need to save the layout in case of State Machine Workflow
            if (rootActivity != null &&
                (_workflowModel.WorkflowType == WorkflowType.StateMachine ||
                 _workflowModel.WorkflowType == WorkflowType.Wizard))
            {
                ActivityDesigner rootdesigner = host.GetDesigner(rootActivity) as ActivityDesigner;
                XmlWriter layoutwriter = _workflowModel.CreateLayoutWriter();
                try
                {
                    IList errors = null;
                    SaveDesignerLayout(layoutwriter, rootdesigner, out errors);
                }
                finally
                {
                    layoutwriter.Close();
                }
            }

            #endregion
        }

        #endregion

        #region Public properties
        public string DefaultNamespace
        {
            get
            {
                return DefaultNameSpace;
            }
        }

        public CodeCompileUnit CodeBesideCCU
        {
            get { return codeBesideccu; }
            set { codeBesideccu = value; }
        }

        public CodeCompileUnit XamlCodeCompileUnit
        {
            get { return xamlccu; }
            set { xamlccu = value; }
        }

        /// <summary>
        /// Gets or sets the WorkflowModel instance to be loaded
        /// </summary>
        public WorkflowModel WorkflowModel
        {
            get
            {
                return _workflowModel;
            }

            set
            {
                _workflowModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the full file path for xoml file
        /// </summary>
        public string XomlFilePath
        {
            get
            {
                return _xomlFilePath;
            }
            set
            {
                _xomlFilePath = value;
            }
        }

        #endregion

        #region Private helpers
        private static void AddObjectGraphToDesignerHost(IDesignerHost designerHost, Activity activity)
        {
            Guid Definitions_Class = new Guid("3FA84B23-B15B-4161-8EB8-37A54EFEEFC7");

            if (designerHost == null)
                throw new ArgumentNullException("designerHost");
            if (activity == null)
                throw new ArgumentNullException("activity");

            string rootSiteName = activity.QualifiedName;
            if (activity.Parent == null)
            {
                string fullClassName = activity.UserData[Definitions_Class] as string;
                if (fullClassName == null)
                    fullClassName = activity.GetType().FullName;
                rootSiteName = (fullClassName.LastIndexOf('.') != -1) ? fullClassName.Substring(fullClassName.LastIndexOf('.') + 1) : fullClassName;
                designerHost.Container.Add(activity, rootSiteName);
            }
            else
            {
                designerHost.Container.Add(activity, activity.QualifiedName);
            }

            if (activity is CompositeActivity)
            {
                foreach (Activity activity2 in GetNestedActivities(activity as CompositeActivity))
                    designerHost.Container.Add(activity2, activity2.QualifiedName);
            }
        }

        private static Activity[] GetNestedActivities(CompositeActivity compositeActivity)
        {
            if (compositeActivity == null)
                throw new ArgumentNullException("compositeActivity");

            IList<Activity> childActivities = null;
            ArrayList nestedActivities = new ArrayList();
            Queue compositeActivities = new Queue();
            compositeActivities.Enqueue(compositeActivity);
            while (compositeActivities.Count > 0)
            {
                CompositeActivity compositeActivity2 = (CompositeActivity)compositeActivities.Dequeue();
                childActivities = compositeActivity2.Activities;

                foreach (Activity activity in childActivities)
                {
                    nestedActivities.Add(activity);
                    if (activity is CompositeActivity)
                        compositeActivities.Enqueue(activity);
                }
            }
            return (Activity[])nestedActivities.ToArray(typeof(Activity));
        }

        internal static void DestroyObjectGraphFromDesignerHost(IDesignerHost designerHost, Activity activity)
        {
            if (designerHost == null)
                throw new ArgumentNullException("designerHost");
            if (activity == null)
                throw new ArgumentNullException("activity");

            designerHost.DestroyComponent(activity);

            if (activity is CompositeActivity)
            {
                foreach (Activity activity2 in GetNestedActivities(activity as CompositeActivity))
                    designerHost.DestroyComponent(activity2);
            }
        }

        #endregion

    }
    #endregion

    public class ExtendedUIService : IExtendedUIService
    {

        #region IExtendedUIService Members

        public void AddAssemblyReference(System.Reflection.AssemblyName assemblyName)
        {
        }

        public void AddDesignerActions(DesignerAction[] actions)
        {
        }

        public DialogResult AddWebReference(out Uri url, out Type proxyClass)
        {

            throw new Exception("The method or operation is not implemented.");

        }

        public Type GetProxyClassForUrl(Uri url)
        {

            throw new Exception("The method or operation is not implemented.");

        }

        public ITypeDescriptorContext GetSelectedPropertyContext()
        {

            //throw new Exception("The method or operation is not implemented.");
            return null;
        }

        public Uri GetUrlForProxyClass(Type proxyClass)
        {

            throw new Exception("The method or operation is not implemented.");

        }

        public Dictionary<string, Type> GetXsdProjectItemsInfo()
        {

            throw new Exception("The method or operation is not implemented.");

        }

        public bool NavigateToProperty(string propName)
        {

            throw new Exception("The method or operation is not implemented.");

        }

        public void RemoveDesignerActions()
        {
        }

        public void ShowToolsOptions()
        {
        }

        #endregion

    }
}
