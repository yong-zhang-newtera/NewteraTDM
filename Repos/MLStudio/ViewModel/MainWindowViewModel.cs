
namespace Newtera.MLStudio.ViewModel
{
    using System;
    using System.Activities;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.Services;
    using System.Activities.Presentation.Toolbox;
    using System.Activities.Statements;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using System.ServiceModel.Activities;
    using System.ServiceModel.Activities.Presentation.Factories;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using AvalonDock;
    using System.Linq;

    using Newtera.MLStudio.Properties;
    using Newtera.MLStudio.Utilities;
    using Newtera.MLStudio.Views;
    using Newtera.MLActivities.MLConfig;
    using Microsoft.Win32;

    public class MainWindowViewModel : ViewModelBase
    {
        private static List<string> namespacesToIgnore = new List<string>
            {
                "Microsoft.VisualBasic.Activities",
                "System.Activities.Expressions",
                "System.Activities.Statements",
                "System.ServiceModel.Activities",
                "System.ServiceModel.Activities.Presentation.Factories",
                "System.Activities.Presentation"
            };

        private static List<string> ML_ACTIVITIES_FILE_NAMES = new List<string>
        {
            "Newtera.MLActivities.dll"
        };

        private DockingManager dockingManager;
        private ICommand openWorkflowCommand;
        private ICommand newWorkflowCommand;
        private ICommand newServiceCommand;
        private ICommand closeWorkflowCommand;
        private ICommand closeAllWorkflowsCommand;
        private ICommand saveWorkflowCommand;
        private ICommand saveAsWorkflowCommand;
        private ICommand startWithDebuggingCommand;
        private ICommand startWithLogCleanedCommand;
        private ICommand startWithoutLogCleanedCommand;
        private ICommand viewToolboxCommand;
        private ICommand viewPropertyInspectorCommand;
        private ICommand viewOutputCommand;
        private ICommand viewErrorsCommand;
        private ICommand viewDebugCommand;
        private ICommand viewCodeCommand;
        private ICommand viewRawDataCommand;
        private ICommand viewTrainDataCommand;
        private ICommand viewTestDataCommand;
        private ICommand viewEvalDataCommand;
        private ICommand viewOutputDataCommand;
        private ICommand viewResultDataCommand;
        private ICommand abortCommand;
        private ICommand evaluateCommand;
        private ICommand deployModelCommand;
        private ICommand setServerUriCommand;
        private ICommand addReferenceCommand;
        private ICommand exitCommand;
        private ICommand saveAllWorkflowsCommand;
        private ICommand aboutCommand;
        private ResizingPanel horizontalResizingPanel;
        private ResizingPanel verticalResizingPanel;
        private DockablePane tabsPane;
        private IDictionary<ContentTypes, DockableContent> dockableContents;
        private ToolboxControl toolboxControl;
        private IDictionary<ToolboxCategory, IList<string>> loadedToolboxActivities;
        private IDictionary<string, ToolboxCategory> toolboxCategoryMap;
        private bool disableDebugViewOutput;

        public MainWindowViewModel(DockingManager dockingManager, ResizingPanel horizontalResizingPanel, ResizingPanel verticalResizingPanel, DockablePane tabsPane, ProgressBar progressBar)
        {
            this.dockingManager = dockingManager;
            ProgressBarModel.TheInstance.ProgressBar = progressBar;

            dockingManager.ActiveDocumentChanged += delegate(object sender, EventArgs args)
            {
                this.OnPropertyChanged("PropertyInspectorView");
                this.OnPropertyChanged("ToolboxView");
                this.OnPropertyChanged("ValidationErrorsView");
                this.OnPropertyChanged("DebugView");
                this.OnPropertyChanged("CodeView");
                this.OnPropertyChanged("RawDataView");
                this.OnPropertyChanged("TrainDataView");
                this.OnPropertyChanged("TestDataView");
                this.OnPropertyChanged("EvalDataView");
                this.OnPropertyChanged("OutputDataView");
                this.OnPropertyChanged("ResultDataView");
                this.OnPropertyChanged("OutputView");
            };

            this.toolboxControl = new ToolboxControl();
            this.InitialiseToolbox();

            this.AddSystemActivities();
            this.AddMLActivities();

            this.horizontalResizingPanel = horizontalResizingPanel;
            this.verticalResizingPanel = verticalResizingPanel;

            this.tabsPane = tabsPane;

            this.dockableContents = new Dictionary<ContentTypes, DockableContent>();
            this.ViewToolbox();

            string disableDebugViewOutputValue = ConfigurationManager.AppSettings["DisableDebugViewOutput"];
            if (!string.IsNullOrEmpty(disableDebugViewOutputValue))
            {
                this.disableDebugViewOutput = bool.Parse(disableDebugViewOutputValue);
            }
        }

        #region Presentation Properties

        public bool HasValidationErrors
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                return model == null ? false : model.HasValidationErrors;
            }
        }

        public UIElement ValidationErrorsView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                return model == null ? null : model.ValidationErrorsView;
            }
        }

        public UIElement DebugView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                return model == null ? null : model.DebugView;
            }
        }

        public UIElement CodeView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.CodeView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement OutputView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    Console.SetOut(model.Output);
                   
                    return model.OutputView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement RawDataView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.RawDataView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement TrainDataView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.TrainDataView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement TestDataView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.TestDataView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement EvalDataView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.EvalDataView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement OutputDataView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.OutputDataView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement ResultDataView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                if (model != null)
                {
                    return model.ResultDataView;
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement ToolboxView
        {
            get
            {
                return this.toolboxControl;
            }
        }

        public UIElement PropertyInspectorView
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                return model == null ? null : model.Designer.PropertyInspectorView;
            }
        }

        #endregion

        #region Commands

        public ICommand OpenWorkflowCommand
        {
            get
            {
                if (this.openWorkflowCommand == null)
                {
                    this.openWorkflowCommand = new RelayCommand(
                        param => this.OpenWorkflow(),
                        param => this.CanOpen);
                }

                return this.openWorkflowCommand;
            }
        }

        public ICommand NewWorkflowCommand
        {
            get
            {
                if (this.newWorkflowCommand == null)
                {
                    this.newWorkflowCommand = new RelayCommand(
                        param => this.NewWorkflow(WorkflowTypes.Activity),
                        param => this.CanNew);
                }

                return this.newWorkflowCommand;
            }
        }

        public ICommand NewServiceCommand
        {
            get
            {
                if (this.newServiceCommand == null)
                {
                    this.newServiceCommand = new RelayCommand(
                        param => this.NewWorkflow(WorkflowTypes.WorkflowService),
                        param => this.CanNew);
                }

                return this.newServiceCommand;
            }
        }

        public ICommand CloseWorkflowCommand
        {
            get
            {
                if (this.closeWorkflowCommand == null)
                {
                    this.closeWorkflowCommand = new RelayCommand(
                        param => this.CloseWorkflow(),
                        param => this.CanClose);
                }

                return this.closeWorkflowCommand;
            }
        }

        public ICommand CloseAllWorkflowsCommand
        {
            get
            {
                if (this.closeAllWorkflowsCommand == null)
                {
                    this.closeAllWorkflowsCommand = new RelayCommand(
                        param => this.CloseAllWorkflows(),
                        param => this.CanCloseAll);
                }

                return this.closeAllWorkflowsCommand;
            }
        }

        public ICommand SaveWorkflowCommand
        {
            get
            {
                if (this.saveWorkflowCommand == null)
                {
                    this.saveWorkflowCommand = new RelayCommand(
                        param => this.SaveWorkflow(),
                        param => this.CanSave);
                }

                return this.saveWorkflowCommand;
            }
        }

        public ICommand SaveAsWorkflowCommand
        {
            get
            {
                if (this.saveAsWorkflowCommand == null)
                {
                    this.saveAsWorkflowCommand = new RelayCommand(
                        param => this.SaveAsWorkflow(),
                        param => this.CanSaveAs);
                }

                return this.saveAsWorkflowCommand;
            }
        }

        public ICommand StartWithLogCleanedCommand
        {
            get
            {
                if (this.startWithLogCleanedCommand == null)
                {
                    this.startWithLogCleanedCommand = new RelayCommand(
                        param => this.StartWithLogCleaned(),
                        param => this.CanStartRunning);
                }

                return this.startWithLogCleanedCommand;
            }
        }

        public ICommand DeployModelCommand
        {
            get
            {
                if (this.deployModelCommand == null)
                {
                    this.deployModelCommand = new RelayCommand(
                        param => this.DeployModel(),
                        param => this.CanDeployModel);
                }

                return this.deployModelCommand;
            }
        }

        public ICommand SetServerUriCommand
        {
            get
            {
                if (this.setServerUriCommand == null)
                {
                    this.setServerUriCommand = new RelayCommand(
                        param => this.SetupServerUri(),
                        param => this.CanSetupServerUri);
                }

                return this.setServerUriCommand;
            }
        }

        public ICommand StartWithoutLogCleanedCommand
        {
            get
            {
                if (this.startWithoutLogCleanedCommand == null)
                {
                    this.startWithoutLogCleanedCommand = new RelayCommand(
                        param => this.StartWithoutLogCleaned(),
                        param => this.CanStartRunning);
                }

                return this.startWithoutLogCleanedCommand;
            }
        }

        public ICommand StartWithDebuggingCommand
        {
            get
            {
                if (this.startWithDebuggingCommand == null)
                {
                    this.startWithDebuggingCommand = new RelayCommand(
                        param => this.StartWithDebugging(),
                        param => this.CanStartWithDebugging);
                }

                return this.startWithDebuggingCommand;
            }
        }

        public ICommand AbortCommand
        {
            get
            {
                if (this.abortCommand == null)
                {
                    this.abortCommand = new RelayCommand(
                        param => this.Abort(),
                        param => this.CanAbort);
                }

                return this.abortCommand;
            }
        }

        public ICommand EvaluateCommand
        {
            get
            {
                if (this.evaluateCommand == null)
                {
                    this.evaluateCommand = new RelayCommand(
                        param => this.EvaluateModel(),
                        param => this.CanEvaluateModel);
                }

                return this.evaluateCommand;
            }
        }

        public ICommand ViewToolboxCommand
        {
            get
            {
                if (this.viewToolboxCommand == null)
                {
                    this.viewToolboxCommand = new RelayCommand(
                        param => this.ViewToolbox(),
                        param => this.CanViewToolbox);
                }

                return this.viewToolboxCommand;
            }
        }

        public ICommand ViewPropertyInspectorCommand
        {
            get
            {
                if (this.viewPropertyInspectorCommand == null)
                {
                    this.viewPropertyInspectorCommand = new RelayCommand(
                        param => this.ViewPropertyInspector(),
                        param => this.CanViewPropertyInspector);
                }

                return this.viewPropertyInspectorCommand;
            }
        }

        public ICommand ViewOutputCommand
        {
            get
            {
                if (this.viewOutputCommand == null)
                {
                    this.viewOutputCommand = new RelayCommand(
                        param => this.ViewOutput(),
                        param => this.CanViewOutput);
                }

                return this.viewOutputCommand;
            }
        }

        public ICommand ViewErrorsCommand
        {
            get
            {
                if (this.viewErrorsCommand == null)
                {
                    this.viewErrorsCommand = new RelayCommand(
                        param => this.ViewErrors(),
                        param => this.CanViewErrors);
                }

                return this.viewErrorsCommand;
            }
        }

        public ICommand ViewDebugCommand
        {
            get
            {
                if (this.viewDebugCommand == null)
                {
                    this.viewDebugCommand = new RelayCommand(
                        param => this.ViewDebug(),
                        param => this.CanViewDebug);
                }

                return this.viewDebugCommand;
            }
        }

        public ICommand ViewCodeCommand
        {
            get
            {
                if (this.viewCodeCommand == null)
                {
                    this.viewCodeCommand = new RelayCommand(
                        param => this.ViewCode(),
                        param => this.CanViewDebug);
                }

                return this.viewCodeCommand;
            }
        }

        public ICommand ViewRawDataCommand
        {
            get
            {
                if (this.viewRawDataCommand == null)
                {
                    this.viewRawDataCommand = new RelayCommand(
                        param => this.ViewRawData(),
                        param => this.CanViewDebug);
                }

                return this.viewRawDataCommand;
            }
        }

        public ICommand ViewTrainDataCommand
        {
            get
            {
                if (this.viewTrainDataCommand == null)
                {
                    this.viewTrainDataCommand = new RelayCommand(
                        param => this.ViewTrainData(),
                        param => this.CanViewDebug);
                }

                return this.viewTrainDataCommand;
            }
        }

        public ICommand ViewTestDataCommand
        {
            get
            {
                if (this.viewTestDataCommand == null)
                {
                    this.viewTestDataCommand = new RelayCommand(
                        param => this.ViewTestData(),
                        param => this.CanViewDebug);
                }

                return this.viewTestDataCommand;
            }
        }

        public ICommand ViewEvalDataCommand
        {
            get
            {
                if (this.viewEvalDataCommand == null)
                {
                    this.viewEvalDataCommand = new RelayCommand(
                        param => this.ViewEvalData(),
                        param => this.CanViewDebug);
                }

                return this.viewEvalDataCommand;
            }
        }

        public ICommand ViewOutputDataCommand
        {
            get
            {
                if (this.viewOutputDataCommand == null)
                {
                    this.viewOutputDataCommand = new RelayCommand(
                        param => this.ViewOutputData(),
                        param => this.CanViewDebug);
                }

                return this.viewOutputDataCommand;
            }
        }

        public ICommand ViewResultDataCommand
        {
            get
            {
                if (this.viewResultDataCommand == null)
                {
                    this.viewResultDataCommand = new RelayCommand(
                        param => this.ViewResultData(),
                        param => this.CanViewDebug);
                }

                return this.viewResultDataCommand;
            }
        }

        public ICommand AddReferenceCommand
        {
            get
            {
                if (this.addReferenceCommand == null)
                {
                    this.addReferenceCommand = new RelayCommand(
                        param => this.AddReference(),
                        param => this.CanAddReference);
                }

                return this.addReferenceCommand;
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                if (this.exitCommand == null)
                {
                    this.exitCommand = new RelayCommand(
                        param => this.Exit(),
                        param => this.CanExit);
                }

                return this.exitCommand;
            }
        }

        public ICommand SaveAllWorkflowsCommand
        {
            get
            {
                if (this.saveAllWorkflowsCommand == null)
                {
                    this.saveAllWorkflowsCommand = new RelayCommand(
                        param => this.SaveAll(),
                        param => this.CanSaveAll);
                }

                return this.saveAllWorkflowsCommand;
            }
        }

        public ICommand AboutCommand
        {
            get
            {
                if (this.aboutCommand == null)
                {
                    this.aboutCommand = new RelayCommand(
                        param => this.About(),
                        param => this.CanAbout);
                }

                return this.aboutCommand;
            }
        }

        private bool CanNew
        {
            get { return true; }
        }

        private bool CanOpen
        {
            get { return true; }
        }

        private bool CanClose
        {
            get { return this.dockingManager.ActiveDocument != null; }
        }

        private bool CanCloseAll
        {
            get { return this.dockingManager.Documents.Count > 1; }
        }

        private bool CanSave
        {
            get
            { 
                WorkflowDocumentContent document = this.dockingManager.ActiveDocument as WorkflowDocumentContent;
                if (document != null)
                {
                   WorkflowViewModel model = document.DataContext as WorkflowViewModel;
                   return model.IsModified;
                }
                else 
                {
                   return false;
                }
            }
        }

        private bool CanSaveAs
        {
            get { return this.dockingManager.ActiveDocument != null; }
        }

        private bool CanStartRunning
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                bool running = model == null ? false : model.IsRunning;
                return this.dockingManager.ActiveDocument != null && !running;
            }
        }

        private bool CanDeployModel
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                bool running = model == null ? false : model.IsRunning;
                return this.dockingManager.ActiveDocument != null && !running;
            }
        }

        private bool CanUndeployModel
        {
            get
            {
                return true;
            }
        }

        private bool CanSetupServerUri
        {
            get
            {
                return true;
            }
        }

        private bool CanAbort
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                return model == null ? false : model.IsRunning;
            }
        }

        private bool CanEvaluateModel
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                bool running = model == null ? false : model.IsRunning;
                return this.dockingManager.ActiveDocument != null && !running;
            }
        }

        private bool CanStartWithDebugging
        {
            get
            {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                bool running = model == null ? false : model.IsRunning;
                return this.dockingManager.ActiveDocument != null && !running;
            }
        }

        private bool CanViewToolbox
        {
            get { return true; }
        }

        private bool CanViewPropertyInspector
        {
            get { return true; }
        }

        private bool CanViewOutput
        {
            get { return true; }
        }

        private bool CanViewErrors
        {
            get { return true; }
        }

        private bool CanViewDebug
        {
            get {
                WorkflowViewModel model = this.ActiveWorkflowViewModel;
                bool running = model == null ? false : model.IsRunning;
                return this.dockingManager.ActiveDocument != null && !running;
            }

        }

        private bool CanAddReference
        {
            get { return true; }
        }

        private bool CanExit
        {
            get { return true; }
        }

        private bool CanSaveAll
        {
            get
            { 
                IList<WorkflowViewModel> modifiedWorkflows = this.GetModifiedWorkflows();
                return modifiedWorkflows.Count > 1;
            }
        }

        private bool CanAbout
        {
            get { return true; }
        }

        #endregion

        private WorkflowViewModel ActiveWorkflowViewModel
        {
            get
            {
                WorkflowDocumentContent content = this.dockingManager.ActiveDocument as WorkflowDocumentContent;
                if (content != null)
                {
                    WorkflowViewModel model = content.DataContext as WorkflowViewModel;
                    if (model != null)
                    {
                        return model;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        private bool HasRunningWorkflows
        {
            get
            {
                foreach (DocumentContent document in this.dockingManager.Documents)
                {
                    if (document is WorkflowDocumentContent)
                    {
                        WorkflowViewModel model = document.DataContext as WorkflowViewModel;
                        if (model.IsRunning)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = this.CancelCloseAllWorkflows();
        }

        #region Private helpers

        private void NewWorkflow(WorkflowTypes workflowType)
        {
            //OpenFileDialog fileDialog = WorkflowFileDialogFactory.CreateNewFileDialog();
            NewWorkflowDialog fileDialog = new NewWorkflowDialog();

            if (fileDialog.ShowDialog() == true)
            {
                WorkflowViewModel workspaceViewModel = new WorkflowViewModel(this.disableDebugViewOutput);
                workspaceViewModel.FullFilePath = fileDialog.FileName;
                WorkflowDocumentContent content = new WorkflowDocumentContent(workspaceViewModel);

                ModelService modelService = workspaceViewModel.Designer.Context.Services.GetService<ModelService>();
                if (modelService != null)
                {
                    List<Type> referencedActivities = this.GetReferencedActivities(modelService);
                    this.AddActivitiesToToolbox(referencedActivities);
                }

                content.Title = workspaceViewModel.DisplayNameWithModifiedIndicator;
                content.Show(this.dockingManager);
                this.dockingManager.ActiveDocument = content;
                content.Closing += new EventHandler<CancelEventArgs>(workspaceViewModel.Closing);
            }

            this.ViewPropertyInspector();
            if (this.HasValidationErrors)
            {
                this.ViewErrors();
                this.SetSelectedTab(ContentTypes.Errors);
            }

            /*
            WorkflowViewModel workspaceViewModel = new WorkflowViewModel(this.disableDebugViewOutput);
            WorkflowDocumentContent content = new WorkflowDocumentContent(workspaceViewModel, workflowType);
            content.Title = workspaceViewModel.DisplayNameWithModifiedIndicator;
            content.Show(this.dockingManager);
            this.dockingManager.ActiveDocument = content;
            content.Closing += new EventHandler<CancelEventArgs>(workspaceViewModel.Closing);
            this.ViewPropertyInspector();
            this.ViewErrors();
            this.SetSelectedTab(ContentTypes.Errors);
            */
        }

        private void OpenWorkflow()
        {
            OpenFileDialog fileDialog = WorkflowFileDialogFactory.CreateOpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                WorkflowViewModel workspaceViewModel = new WorkflowViewModel(this.disableDebugViewOutput);
                workspaceViewModel.FullFilePath = fileDialog.FileName;
                WorkflowDocumentContent content = new WorkflowDocumentContent(workspaceViewModel);

                ModelService modelService = workspaceViewModel.Designer.Context.Services.GetService<ModelService>();
                if (modelService != null)
                {
                    List<Type> referencedActivities = this.GetReferencedActivities(modelService);
                    this.AddActivitiesToToolbox(referencedActivities);
                }

                content.Title = workspaceViewModel.DisplayNameWithModifiedIndicator;
                content.Show(this.dockingManager);
                this.dockingManager.ActiveDocument = content;
                content.Closing += new EventHandler<CancelEventArgs>(workspaceViewModel.Closing);
            }

            this.ViewPropertyInspector();
            if (this.HasValidationErrors)
            {
                this.ViewErrors();
                this.SetSelectedTab(ContentTypes.Errors);
            }
        }

        private void CloseWorkflow()
        {
            this.dockingManager.ActiveDocument.Close();
        }

        private void CloseAllWorkflows()
        {
            if (!this.CancelCloseAllWorkflows())
            {
                foreach (DocumentContent documentContent in new List<DocumentContent>(this.dockingManager.Documents))
                {
                    WorkflowViewModel vm = documentContent.DataContext as WorkflowViewModel;
                    documentContent.Closing -= vm.Closing;
                    documentContent.Close();
                }
            }
        }

        private bool CancelCloseAllWorkflows()
        {
            bool cancel = false;

            if (this.HasRunningWorkflows)
            {
                MessageBoxResult closingResult = MessageBox.Show(Resources.ConfirmCloseWhenRunningWorkflowsDialogMessage, Resources.ConfirmCloseWhenRunningWorkflowsDialogTitle, MessageBoxButton.YesNo);
                switch (closingResult)
                {
                    case MessageBoxResult.No:
                        cancel = true;
                        break;
                    case MessageBoxResult.Yes:
                        cancel = false;
                        break;
                    case MessageBoxResult.Cancel:
                        cancel = true;
                        break;
                }
            }

            if (cancel == false)
            {
                IList<WorkflowViewModel> modifiedWorkflows = this.GetModifiedWorkflows();
                if (modifiedWorkflows.Count > 0)
                {
                    MessageBoxResult closingResult = MessageBox.Show(string.Format(Resources.SaveChangesDialogMessage, this.FormatUnsavedWorkflowNames(modifiedWorkflows)), Resources.SaveChangesDialogTitle, MessageBoxButton.YesNoCancel);
                    switch (closingResult)
                    {
                        case MessageBoxResult.No:
                            cancel = false;
                            break;
                        case MessageBoxResult.Yes:
                            cancel = !this.SaveAllWorkflows();
                            break;
                        case MessageBoxResult.Cancel:
                            cancel = true;
                            break;
                    }
                }
            }

            return cancel;
        }

        private void SaveWorkflow()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                model.SaveWorkflow();
            }
        }

        private void SaveAsWorkflow()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                model.SaveAsWorkflow();
            }
        }

        private void StartWithLogCleaned()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                this.ViewOutput();
                this.SetSelectedTab(ContentTypes.Output);
                if (this.HasValidationErrors)
                {
                    this.ViewErrors();
                    this.SetSelectedTab(ContentTypes.Errors);
                }

                RemoveContentTabs();

                model.RunWithLogsCleaned = true;

                model.RunWorkflow();
            }
        }

        private void DeployModel()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                //this.ViewOutput();
                //this.SetSelectedTab(ContentTypes.Output);
                if (this.HasValidationErrors)
                {
                    this.ViewErrors();
                    this.SetSelectedTab(ContentTypes.Errors);
                }
                else
                {
                    if (model.ExperimentManager != null)
                    {
                        OpenSchemaDialog openSchemaDialog = new OpenSchemaDialog();
                        if (openSchemaDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            DeployModelDialog dialog = new DeployModelDialog(model.ExperimentManager, openSchemaDialog.SelectedSchema);

                            dialog.ShowDialog();
                        }
                    }
                    else
                    {
                        model.DryRunWorkflow(WorkflowCompleted);
                    }
                }
            }
        }

        private void SetupServerUri()
        {
            SetupServerUrlDialog dialog = new SetupServerUrlDialog();
            dialog.ShowDialog();
        }

        private void WorkflowCompleted(object arg)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                if (arg != null &&
                    arg is MLExperimentManager)
                {
                    OpenSchemaDialog openSchemaDialog = new OpenSchemaDialog();
                    if (openSchemaDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        DeployModelDialog dialog = new DeployModelDialog((MLExperimentManager)arg, openSchemaDialog.SelectedSchema);

                        dialog.ShowDialog();
                    }
                }
            }));
        }

        private void EvaluateModelCallback(object arg)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                if (arg != null &&
                    arg is MLExperimentManager)
                {
                    EvaluateInfoDialog infoDialog = new EvaluateInfoDialog((MLExperimentManager)arg);
                    infoDialog.ShowDialog();
                }
            }));
        }

        private void StartWithoutLogCleaned()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                this.ViewOutput();
                this.SetSelectedTab(ContentTypes.Output);
                if (this.HasValidationErrors)
                {
                    this.ViewErrors();
                    this.SetSelectedTab(ContentTypes.Errors);
                }

                RemoveContentTabs();

                model.RunWithLogsCleaned = false;

                model.RunWorkflow();
            }
        }

        private void Abort()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                model.Abort();
            }
        }

        private void EvaluateModel()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                if (model.ExperimentManager != null)
                {
                    EvaluateInfoDialog infoDialog = new EvaluateInfoDialog(model.ExperimentManager);
                    infoDialog.ShowDialog();
                }
                else
                {
                    model.DryRunWorkflow(EvaluateModelCallback);
                }
            }
        }

        private void StartWithDebugging()
        {
            WorkflowViewModel model = this.ActiveWorkflowViewModel;
            if (model != null)
            {
                this.ViewOutput();
                this.ViewDebug();
                this.SetSelectedTab(ContentTypes.Debug);
                if (this.HasValidationErrors)
                {
                    this.ViewErrors();
                    this.SetSelectedTab(ContentTypes.Errors);
                }

                model.DebugWorkflow();
                this.OnPropertyChanged("DebugView");
            }
        }

        private void ViewToolbox()
        {
            this.CreateOrUnhideDockableContent(ContentTypes.Toolbox, Resources.ViewMenuToolbox, "ToolboxView", this.horizontalResizingPanel);
        }

        private void ViewPropertyInspector()
        {
            this.CreateOrUnhideDockableContent(ContentTypes.PropertyInspector, Resources.ViewMenuProperties, "PropertyInspectorView", this.horizontalResizingPanel);
        }

        private void ViewOutput()
        {
            this.CreateOrUnhideDockableContent(ContentTypes.Output, Resources.ViewMenuOutput, "OutputView", this.tabsPane);
        }

        private void ViewErrors()
        {
            this.CreateOrUnhideDockableContent(ContentTypes.Errors, Resources.ViewMenuErrors, "ValidationErrorsView", this.tabsPane);
        }

        private void ViewDebug()
        {
            if (!this.disableDebugViewOutput)
            {
                if (TensorBoard.TheInstance.IsAvailable)
                {
                    TensorBoard.TheInstance.LaunchBrowser();
                }
                else
                {
                    MessageBox.Show("TensorBoard tool isn't available with error: \n" + TensorBoard.TheInstance.ErrorMessage);
                }
            }
        }

        private void ViewCode()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.Code, Resources.ViewMenuCode, "CodeView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.Code);
            }
        }

        private void ViewRawData()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.RawData, Resources.ViewMenuRawData, "RawDataView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.RawData);
            }
        }

        private void ViewTrainData()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.TrainData, Resources.ViewMenuTrainData, "TrainDataView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.TrainData);
            }
        }

        private void ViewTestData()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.TestData, Resources.ViewMenuTestData, "TestDataView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.TestData);
            }
        }

        private void ViewEvalData()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.EvalData, Resources.ViewMenuEvalData, "EvalDataView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.EvalData);
            }
        }

        private void ViewOutputData()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.OutputData, Resources.ViewMenuOutputData, "OutputDataView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.OutputData);
            }
        }

        private void ViewResultData()
        {
            if (!this.disableDebugViewOutput)
            {
                this.CreateOrUnhideDockableContent(ContentTypes.ResultData, Resources.ViewMenuResultData, "ResultDataView", this.tabsPane);
                this.SetSelectedTab(ContentTypes.ResultData);
            }
        }

        private void AddReference()
        {
            OpenFileDialog fileDialog = WorkflowFileDialogFactory.CreateAddReferenceDialog();
            if (fileDialog.ShowDialog() == true)
            {
                this.AddActivitiesFromAssemblies(fileDialog.FileNames);
            }
        }

        private void AddMLActivities()
        {
            this.AddActivitiesFromAssemblies(MainWindowViewModel.ML_ACTIVITIES_FILE_NAMES);
        }

        private void AddSystemActivities()
        {
            this.AddCategoryToToolbox(
                Resources.ControlFlowCategoryToToolbox,
                new List<Type>
                {
                    //typeof(ForEach<>),
                    //typeof(If),
                    typeof(Parallel),
                    //typeof(ParallelForEach<>),
                    //typeof(DoWhile),
                    //typeof(Pick),
                    //typeof(PickBranch),
                    //typeof(Sequence),
                    //typeof(Switch<>),
                    //typeof(While)
                });

            /*
            this.AddCategoryToToolbox(
                Resources.FlowchartCategoryToToolbox,
                new List<Type>
                {
                    typeof(Flowchart),
                    typeof(FlowDecision),
                    typeof(FlowSwitch<>)
                });
                */

            /*
            this.AddCategoryToToolbox(
                Resources.MessagingCategoryToToolbox,
                new List<Type>
                {
                    typeof(CorrelationScope),
                    typeof(InitializeCorrelation),
                    typeof(Receive),
                    typeof(ReceiveAndSendReplyFactory),
                    typeof(Send),
                    typeof(SendAndReceiveReplyFactory),
                    typeof(TransactedReceiveScope)
                });
                */

            /*
            this.AddCategoryToToolbox(
                Resources.RuntimeCategoryToToolbox,
                new List<Type>
                {
                    typeof(Persist),
                    typeof(TerminateWorkflow),
                });
                */

            /*
            this.AddCategoryToToolbox(
                Resources.PrimitivesCategoryToToolbox,
                new List<Type>
                {
                    typeof(Assign),
                    //typeof(Delay),
                    //typeof(InvokeMethod),
                    typeof(WriteLine),
                });
                */

            /*
            this.AddCategoryToToolbox(
                Resources.TransactionCategoryToToolbox,
                new List<Type>
                {
                    typeof(CancellationScope),
                    typeof(CompensableActivity),
                    typeof(Compensate),
                    typeof(Confirm),
                    typeof(TransactionScope),
                });
                */

            /*
            this.AddCategoryToToolbox(
                Resources.CollectionCategoryToToolbox,
                new List<Type>
                {
                    typeof(AddToCollection<>),
                    typeof(ClearCollection<>),
                    typeof(ExistsInCollection<>),
                    typeof(RemoveFromCollection<>),
                });
                */

            /*
            this.AddCategoryToToolbox(
                Resources.ErrorHandlingCategoryToToolbox,
                new List<Type>
                {
                    typeof(Rethrow),
                    typeof(Throw),
                    typeof(TryCatch),
                });
                */
        }

        private void Exit()
        {
            if (!this.CancelCloseAllWorkflows())
            {
                foreach (DocumentContent documentContent in new List<DocumentContent>(this.dockingManager.Documents))
                {
                    WorkflowViewModel vm = documentContent.DataContext as WorkflowViewModel;
                    documentContent.Closing -= vm.Closing;
                    documentContent.Close();
                }

                // kill tensorboard process if exist
                KillTensorboardProcess();

                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Application.Current.Shutdown();
            }
        }

        private void SaveAll()
        {
            this.SaveAllWorkflows();
        }

        private void About()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show(string.Format(CultureInfo.InvariantCulture, Resources.AboutDialogMessage, version.ToString(4)), Resources.AboutDialogTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InitialiseToolbox()
        {
            this.loadedToolboxActivities = new Dictionary<ToolboxCategory, IList<string>>();
            this.toolboxCategoryMap = new Dictionary<string, ToolboxCategory>();
        }

        private void AddActivitiesFromAssemblies(IEnumerable<string> assemblyFiles)
        {
            var assemblies = new List<Assembly>();

            foreach (string fileName in assemblyFiles)
            {
                Assembly assembly = Assembly.LoadFrom(fileName);
                assemblies.Add(assembly);
                this.AddCategoryToToolbox(assemblies);
            }
        }

        private bool IsValidToolboxActivity(Type activityType)
        {
            return activityType.IsPublic && !activityType.IsNested && !activityType.IsAbstract && !activityType.ContainsGenericParameters
                        && (typeof(Activity).IsAssignableFrom(activityType) || typeof(IActivityTemplateFactory).IsAssignableFrom(activityType));
        }

        private void AddCategoryToToolbox(List<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                // sort activity by name
                Type[] activityTypes = assembly.GetTypes().OrderBy(p => p.Name).ToArray();
                foreach (Type activityType in activityTypes)
                {
                    if (this.IsValidToolboxActivity(activityType))
                    {
                        ToolboxCategory category = this.GetToolboxCategory(activityType.Namespace);

                        if (!this.loadedToolboxActivities[category].Contains(activityType.FullName))
                        {
                            this.loadedToolboxActivities[category].Add(activityType.FullName);
                            category.Add(new ToolboxItemWrapper(activityType.FullName, activityType.Assembly.FullName, null, activityType.Name));
                        }
                    }
                }
            }
        }

        private void AddCategoryToToolbox(string categoryName, List<Type> activities)
        {
            foreach (Type activityType in activities)
            {
                if (this.IsValidToolboxActivity(activityType))
                {
                    ToolboxCategory category = this.GetToolboxCategory(categoryName);

                    if (!this.loadedToolboxActivities[category].Contains(activityType.FullName))
                    {
                        string displayName;
                        string[] splitName = activityType.Name.Split('`');
                        if (splitName.Length == 1)
                        {
                            displayName = activityType.Name;
                        }
                        else
                        {
                            displayName = string.Format("{0}<>", splitName[0]);
                        }

                        this.loadedToolboxActivities[category].Add(activityType.FullName);
                        category.Add(new ToolboxItemWrapper(activityType.FullName, activityType.Assembly.FullName, null, displayName));
                    }
                }
            }
        }

        private void AddActivitiesToToolbox(List<Type> activities)
        {
            foreach (Type activityType in activities)
            {
                if (this.IsValidToolboxActivity(activityType))
                {
                    ToolboxCategory category = this.GetToolboxCategory(activityType.Namespace);

                    if (!this.loadedToolboxActivities[category].Contains(activityType.FullName))
                    {
                        this.loadedToolboxActivities[category].Add(activityType.FullName);
                        category.Add(new ToolboxItemWrapper(activityType.FullName, activityType.Assembly.FullName, null, activityType.Name));
                    }
                }
            }
        }

        private List<Type> GetReferencedActivities(ModelService modelService)
        {
            IEnumerable<ModelItem> items = modelService.Find(modelService.Root, typeof(Activity));
            List<Type> activities = new List<Type>();
            foreach (ModelItem item in items) 
            {
                if (!namespacesToIgnore.Contains(item.ItemType.Namespace))
                {
                    activities.Add(item.ItemType);
                }
            }

            return activities;
        }

        private ToolboxCategory GetToolboxCategory(string name)
        {
            string categoryName = name;
            // remove the name space from the category name
            if (categoryName.IndexOf('.') > 0)
            {
                int pos = categoryName.LastIndexOf('.');
                categoryName = categoryName.Substring(pos + 1);
            }

            string temp = Resources.ResourceManager.GetString(categoryName);
            if (!string.IsNullOrEmpty(temp))
            {
                categoryName = temp;
            }

            if (this.toolboxCategoryMap.ContainsKey(categoryName))
            {
                return this.toolboxCategoryMap[categoryName];
            }
            else
            {

                ToolboxCategory category = new ToolboxCategory(categoryName);
                this.toolboxCategoryMap[categoryName] = category;
                this.loadedToolboxActivities.Add(category, new List<string>());
                this.toolboxControl.Categories.Add(category);
                return category;
            }
        }

        private void CreateOrUnhideDockableContent(ContentTypes contentType, string title, string viewPropertyName, object parent)
        {
            if (!this.dockableContents.Keys.Contains(contentType))
            {
                DockableContent dockableContent = new DockableContent();

                ContentControl contentControl = new ContentControl();

                dockableContent.IsCloseable = true;
                dockableContent.HideOnClose = false;

                dockableContent.Title = title;

                dockableContent.Content = contentControl;

                if (parent is ResizingPanel)
                {
                    DockablePane dockablePane = new DockablePane();
                    dockablePane.Items.Add(dockableContent);
                    ResizingPanel resizingPanel = parent as ResizingPanel;

                    switch (contentType)
                    {
                        case ContentTypes.PropertyInspector:
                            resizingPanel.Children.Add(dockablePane);
                            ResizingPanel.SetResizeWidth(dockablePane, new GridLength(300));
                            break;
                        case ContentTypes.Toolbox:
                            resizingPanel.Children.Insert(0, dockablePane);
                            ResizingPanel.SetResizeWidth(dockablePane, new GridLength(210));
                            break;
                    }
                }
                else if (parent is DockablePane)
                {
                    DockablePane dockablePane = parent as DockablePane;
                    dockablePane.Items.Add(dockableContent);
                    if (dockablePane.Parent == null)
                    {
                        this.verticalResizingPanel.Children.Add(dockablePane);
                    }
                }

                Binding dataContextBinding = new Binding(viewPropertyName);
                dockableContent.SetBinding(DockableContent.DataContextProperty, dataContextBinding);

                Binding contentBinding = new Binding(".");
                contentControl.SetBinding(ContentControl.ContentProperty, contentBinding);

                this.dockableContents[contentType] = dockableContent;

                dockableContent.Closed += delegate(object sender, EventArgs args)
                {
                    contentControl.Content = null;
                    this.dockableContents[contentType].DataContext = null;
                    this.dockableContents.Remove(contentType);
                };
            }
            else
            {
                if (this.dockableContents[contentType].State == DockableContentState.Hidden)
                {
                    this.dockableContents[contentType].Show();
                }
            }
        }

        private void SetSelectedTab(ContentTypes contentType)
        {
            if (this.dockableContents.Keys.Contains(contentType))
            {
                this.tabsPane.SelectedItem = this.dockableContents[contentType];
            }
        }

        private void RemoveContentTabs()
        {
            ContentTypes[] dataContentTypes = new ContentTypes[] {
                ContentTypes.Code,
                ContentTypes.RawData,
                ContentTypes.TrainData,
                ContentTypes.TestData,
                ContentTypes.EvalData,
                ContentTypes.OutputData,
                ContentTypes.ResultData};

            foreach (ContentTypes contentType in dataContentTypes)
            {
                try
                {
                    DockableContent dockabkeContent = this.dockableContents[contentType];
                    foreach (DockableContent content in this.tabsPane.Items)
                    {
                        if (content == dockabkeContent)
                        {
                            content.Close();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private IList<WorkflowViewModel> GetModifiedWorkflows()
        {
            List<WorkflowViewModel> modifiedWorkflows = new List<WorkflowViewModel>();

            foreach (DocumentContent document in this.dockingManager.Documents)
            {
                if (document is WorkflowDocumentContent)
                {
                    WorkflowViewModel model = document.DataContext as WorkflowViewModel;
                    if (model.IsModified)
                    {
                        modifiedWorkflows.Add(model);
                    }
                }
            }

            return modifiedWorkflows;
        }

        private bool SaveAllWorkflows()
        {
            foreach (WorkflowViewModel model in this.GetModifiedWorkflows())
            {
                if (!model.SaveWorkflow())
                {
                    return false;
                }
            }

            return true;
        }

        private string FormatUnsavedWorkflowNames(IList<WorkflowViewModel> workflowViewModels)
        {
            StringBuilder sb = new StringBuilder();

            foreach (WorkflowViewModel model in this.GetModifiedWorkflows())
            {
                sb.Append(model.DisplayName);
                sb.Append("\n");
            }

            return sb.ToString();
        }

        private void KillTensorboardProcess()
        {
            string[] ProcessName = { "tensorboard" };
            try
            {
                System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    foreach (string pName in ProcessName)
                    {
                        if (pName == myProcess.ProcessName)
                        {
                            myProcess.Kill();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
