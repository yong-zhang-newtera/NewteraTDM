
namespace Newtera.MLStudio
{
    using System;
    using System.Activities.Core.Presentation;
    using System.Activities.Presentation;
    using System.ComponentModel;
    using System.Configuration;
    using System.Windows;
    using AvalonDock;
    using Newtera.MLStudio.Properties;
    using Newtera.MLStudio.Utilities;
    using Newtera.MLStudio.ViewModel;
    
    public class WorkflowDocumentContent : DocumentContent
    {
        private string defaultWorkflow = "..\\..\\defaultConfiguration.xaml";
        private string defaultWorkflowService = "..\\..\\defaultWorkflowService.xamlx";

        static WorkflowDocumentContent()
        {
           new DesignerMetadata().Register();
        }

        public WorkflowDocumentContent(WorkflowViewModel model)
            : this(model, WorkflowTypes.Activity)
        {
        }

        public WorkflowDocumentContent(WorkflowViewModel model, WorkflowTypes workflowType)
            : base()
        {
            this.DataContext = model;

            string defaultWorkflowValue = ConfigurationManager.AppSettings["DefaultWorkflow"];
            if (!string.IsNullOrEmpty(defaultWorkflowValue))
            {
                this.defaultWorkflow = defaultWorkflowValue;
            }

            string defaultWorkflowServiceValue = ConfigurationManager.AppSettings["DefaultWorkflowService"];
            if (!string.IsNullOrEmpty(defaultWorkflowServiceValue))
            {
                this.defaultWorkflowService = defaultWorkflowServiceValue;
            }

            WorkflowDesigner designer = model.Designer;

            try
            {
                if (string.IsNullOrEmpty(model.FullFilePath))
                {
                    if (workflowType == WorkflowTypes.Activity)
                    {
                        designer.Load(this.defaultWorkflow);
                    }
                    else
                    {
                        designer.Load(this.defaultWorkflowService);
                    }
                }
                else
                {
                    designer.Load(model.FullFilePath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorLoadingDialogMessage, ExceptionHelper.FormatStackTrace(e)), Properties.Resources.ErrorLoadingDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
                
            this.Content = model.Designer.View;

            model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.Model_PropertyChanged);
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "DisplayNameWithModifiedIndicator" || args.PropertyName == "DisplayName")
            {
                WorkflowViewModel model = this.DataContext as WorkflowViewModel;
                if (model != null)
                {
                    this.Title = model.DisplayNameWithModifiedIndicator;
                }
            }
        }
    }
}
