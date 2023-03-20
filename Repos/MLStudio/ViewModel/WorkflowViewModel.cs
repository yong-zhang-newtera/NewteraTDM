
namespace Newtera.MLStudio.ViewModel
{
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Services;
    using System.Activities.Presentation.Validation;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.ServiceModel.Activities;
    using System.Windows;
    using System.Windows.Controls;
    using Newtera.MLStudio.Execution;
    using Newtera.MLStudio.Properties;
    using Newtera.MLStudio.Utilities;
    using Newtera.MLStudio.Views;
    using Microsoft.Win32;
    using ICSharpCode.AvalonEdit;

    using Newtera.MLActivities.MLConfig;

    public class WorkflowViewModel : ViewModelBase
    {
        private static int designerCount;
        private WorkflowDesigner workflowDesigner;
        private IList<ValidationErrorInfo> validationErrors;
        private ValidationErrorService validationErrorService;
        private ValidationErrorsUserControl validationErrorsView;
        private bool modelChanged;
        private TextWriter output;
        private TextEditorWriter code;
        private TextEditorWriter rawData;
        private TextEditorWriter trainData;
        private TextEditorWriter testData;
        private TextEditorWriter evalData;
        private TextEditorWriter outputData;
        private TextEditorWriter resultData;
        private TextBox outputTextBox;
        private TextEditor codeTextBox;
        private TextEditor rawDataTextBox;
        private TextEditor trainDataTextBox;
        private TextEditor testDataTextBox;
        private TextEditor evalDataTextBox;
        private TextEditor outputDataTextBox;
        private TextEditor resultDataTextBox;
        private IWorkflowRunner runner;
        private int id;
        private string fullFilePath;
        private bool disableDebugViewOutput;

        public WorkflowViewModel(bool disableDebugViewOutput)
        {
            this.workflowDesigner = new WorkflowDesigner();
            this.id = ++designerCount;
            this.validationErrors = new List<ValidationErrorInfo>();
            this.validationErrorService = new ValidationErrorService(this.validationErrors);
            this.workflowDesigner.Context.Services.Publish<IValidationErrorService>(this.validationErrorService);

            this.workflowDesigner.ModelChanged += delegate(object sender, EventArgs args)
            {
                this.modelChanged = true;
                this.OnPropertyChanged("DisplayNameWithModifiedIndicator");
            };

            this.validationErrorsView = new ValidationErrorsUserControl();

            this.outputTextBox = new TextBox();
            this.output = new TextBoxStreamWriter(this.outputTextBox, this.DisplayName);

            this.codeTextBox = new TextEditor();
            this.codeTextBox.ShowLineNumbers = true;
            this.codeTextBox.IsReadOnly = true;
            this.code = new TextEditorWriter(this.codeTextBox, this.DisplayName);

            this.rawDataTextBox = new TextEditor();
            this.rawDataTextBox.ShowLineNumbers = true;
            this.rawDataTextBox.IsReadOnly = true;
            this.rawData = new TextEditorWriter(this.rawDataTextBox, this.DisplayName);

            this.trainDataTextBox = new TextEditor();
            this.trainDataTextBox.ShowLineNumbers = true;
            this.trainDataTextBox.IsReadOnly = true;
            this.trainData = new TextEditorWriter(this.trainDataTextBox, this.DisplayName);

            this.testDataTextBox = new TextEditor();
            this.testDataTextBox.ShowLineNumbers = true;
            this.testDataTextBox.IsReadOnly = true;
            this.testData = new TextEditorWriter(this.testDataTextBox, this.DisplayName);

            this.evalDataTextBox = new TextEditor();
            this.evalDataTextBox.ShowLineNumbers = true;
            this.evalDataTextBox.IsReadOnly = true;
            this.evalData = new TextEditorWriter(this.evalDataTextBox, this.DisplayName);

            this.outputDataTextBox = new TextEditor();
            this.outputDataTextBox.ShowLineNumbers = true;
            this.outputDataTextBox.IsReadOnly = true;
            this.outputData = new TextEditorWriter(this.outputDataTextBox, this.DisplayName);

            this.resultDataTextBox = new TextEditor();
            this.resultDataTextBox.ShowLineNumbers = true;
            this.resultDataTextBox.IsReadOnly = true;
            this.resultData = new TextEditorWriter(this.resultDataTextBox, this.DisplayName);

            this.disableDebugViewOutput = disableDebugViewOutput;

            this.RunWithLogsCleaned = false;

            this.validationErrorService.ErrorsChangedEvent += delegate(object sender, EventArgs args)
            {
                DispatcherService.Dispatch(() =>
                {
                    this.validationErrorsView.ErrorsDataGrid.ItemsSource = this.validationErrors;
                    this.validationErrorsView.ErrorsDataGrid.Items.Refresh();
                });
            };
        }

        public bool RunWithLogsCleaned { get; set; }

        #region Presentation Properties

        public UIElement DebugView
        {
            get
            {
                IWorkflowDebugger debugger = this.runner as IWorkflowDebugger;
                if (debugger != null)
                {
                    return debugger.GetDebugView();
                }
                else
                {
                    return null;
                }
            }
        }

        public UIElement ValidationErrorsView
        {
            get
            {
                return this.validationErrorsView;
            }
        }

        public bool HasValidationErrors
        {
            get
            {
                return this.validationErrors.Count > 0;
            }
        }

        public UIElement OutputView
        {
            get
            {
                return this.outputTextBox;
            }
        }

        public UIElement CodeView
        {
            get
            {
                // make sure the code has been loaded from a file
                if (!this.code.IsLoaded)
                {
                    this.code.LoadFile();
                }
                return this.codeTextBox;
            }
        }

        public UIElement RawDataView
        {
            get
            {
                // make sure the data has been loaded from a file
                if (!this.rawData.IsLoaded)
                {
                    this.rawData.LoadFile();
                }

                return this.rawDataTextBox;
            }
        }

        public UIElement TrainDataView
        {
            get
            {
                // make sure the data has been loaded from a file
                if (!this.trainData.IsLoaded)
                {
                    this.trainData.LoadFile();
                }
                
                return this.trainDataTextBox;
            }
        }

        public UIElement TestDataView
        {
            get
            {
                // make sure the data has been loaded from a file
                if (!this.testData.IsLoaded)
                {
                    this.testData.LoadFile();
                }
                return this.testDataTextBox;
            }
        }

        public UIElement EvalDataView
        {
            get
            {
                // make sure the data has been loaded from a file
                if (!this.evalData.IsLoaded)
                {
                    this.evalData.LoadFile();
                }

                return this.evalDataTextBox;
            }
        }

        public UIElement OutputDataView
        {
            get
            {
                // make sure the data has been loaded from a file
                if (!this.outputData.IsLoaded)
                {
                    this.outputData.LoadFile();
                }

                return this.outputDataTextBox;
            }
        }

        public UIElement ResultDataView
        {
            get
            {
                // make sure the data has been loaded from a file
                if (!this.resultData.IsLoaded)
                {
                    this.resultData.LoadFile();
                }
                return this.resultDataTextBox;
            }
        }

        public TextWriter Output
        {
            get
            {
                return this.output;
            }
        }

        public TextEditorWriter Code
        {
            get
            {
                return this.code;
            }
        }

        public TextEditorWriter TrainData
        {
            get
            {
                return this.trainData;
            }
        }

        public TextEditorWriter TestData
        {
            get
            {
                return this.testData;
            }
        }

        public TextEditorWriter EvalData
        {
            get
            {
                return this.evalData;
            }
        }

        public TextEditorWriter OutputData
        {
            get
            {
                return this.outputData;
            }
        }

        public TextEditorWriter ResultData
        {
            get
            {
                return this.resultData;
            }
        }

        public WorkflowDesigner Designer
        {
            get
            {
                return this.workflowDesigner;
            }
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(this.FullFilePath))
                {
                    return string.Format(Resources.NewWorkflowTabTitle, this.id);
                }
                else
                {
                    return Path.GetFileName(this.FullFilePath);
                }
            }
        }

        public string DisplayNameWithModifiedIndicator
        {
            get
            {
                string modifiedIndicator = this.modelChanged ? "*" : string.Empty;
                if (string.IsNullOrEmpty(this.FullFilePath))
                {
                    return string.Format(Resources.NewWorkflowWithModifierTabTitle, this.id, modifiedIndicator);
                }
                else
                {
                    return string.Format("{0} {1}", Path.GetFileName(this.FullFilePath), modifiedIndicator);
                }
            }
        }

        public string FullFilePath
        {
            get
            {
                return this.fullFilePath;
            }

            set
            {
                this.fullFilePath = value;
                this.output = new TextBoxStreamWriter(this.outputTextBox, Path.GetFileNameWithoutExtension(this.fullFilePath));
            }
        }

        public bool IsRunning
        {
            get
            {
                return this.runner == null ? false : this.runner.IsRunning;
            }
        }

        public bool IsModified
        {
            get
            {
                return this.modelChanged;
            }
        }

        public MLExperimentManager ExperimentManager
        {
            get
            {
                if (this.runner != null && this.runner is WorkflowRunner)
                {
                    return ((WorkflowRunner)this.runner).ExperimentManager;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        public void Abort()
        {
            if (this.runner != null)
            {
                this.runner.Abort();
            }
        }

        public void Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            if (this.IsRunning)
            {
                MessageBoxResult closingResult = MessageBox.Show(Resources.ConfirmCloseWhenRunningWorkflowDialogMessage, Resources.ConfirmCloseWhenRunningWorkflowDialogTitle, MessageBoxButton.YesNo);
                switch (closingResult)
                {
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.Yes:
                        e.Cancel = false;
                        this.Abort();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }

            if (e.Cancel == false)
            {
                if (this.modelChanged)
                {
                    MessageBoxResult closingResult = MessageBox.Show(string.Format(Resources.SaveChangesDialogMessage, this.DisplayName), Resources.SaveChangesDialogTitle, MessageBoxButton.YesNoCancel);
                    switch (closingResult)
                    {
                        case MessageBoxResult.No:
                            e.Cancel = false;
                            break;
                        case MessageBoxResult.Yes:
                            e.Cancel = !this.SaveWorkflow();
                            break;
                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            break;
                    }
                }
            }
        }

        public Type GetRootType()
        {
            ModelService modelService = this.workflowDesigner.Context.Services.GetService<ModelService>();
            if (modelService != null)
            {
                return modelService.Root.GetCurrentValue().GetType();
            }
            else
            {
                return null;
            }
        }

        public bool SaveWorkflow()
        {
            if (!string.IsNullOrEmpty(this.FullFilePath))
            {
                this.SaveWorkflow(this.FullFilePath);
                return true;
            }
            else
            {
                SaveFileDialog fileDialog = WorkflowFileDialogFactory.CreateSaveFileDialog(this.DisplayName);

                if (fileDialog.ShowDialog() == true)
                {
                    this.SaveWorkflow(fileDialog.FileName);
                    return true;
                }

                return false;
            }
        }

        public void SaveAsWorkflow()
        {
            SaveFileDialog fileDialog = WorkflowFileDialogFactory.CreateSaveFileDialog(this.DisplayName);

            if (fileDialog.ShowDialog() == true)
            {
                this.SaveWorkflow(fileDialog.FileName);
            }
        }

        public void RunWorkflow()
        {
            // clear content
            this.codeTextBox.Text = "";
            this.rawDataTextBox.Text = "";
            this.trainDataTextBox.Text = "";
            this.testDataTextBox.Text = "";
            this.evalDataTextBox.Text = "";
            this.outputDataTextBox.Text = "";
            this.resultDataTextBox.Text = "";
            TextWriterHolder writerHolder = new TextWriterHolder();
            writerHolder.output = this.output;
            writerHolder.code = this.code;
            writerHolder.rawData = this.rawData;
            writerHolder.trainData = this.trainData;
            writerHolder.testData = this.testData;
            writerHolder.evalData = this.evalData;
            writerHolder.outputData = this.outputData;
            writerHolder.resultData = this.resultData;
            this.runner = new WorkflowRunner(writerHolder, this.DisplayName, this.workflowDesigner, RunWithLogsCleaned);

            try
            {
                this.outputTextBox.Clear();
                this.runner.Run();
            } 
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Resources.ErrorRunningDialogMessage, ExceptionHelper.FormatStackTrace(e)), Resources.ErrorRunningDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // run the workflow without generating ml model(s)
        public void DryRunWorkflow(Action<object> completedCallback)
        {

            this.codeTextBox.Text = ""; // clear content
            this.rawDataTextBox.Text = "";
            this.trainDataTextBox.Text = "";
            this.testDataTextBox.Text = "";
            this.evalDataTextBox.Text = "";
            this.outputDataTextBox.Text = "";
            this.resultDataTextBox.Text = "";

            TextWriterHolder writerHolder = new TextWriterHolder();
            writerHolder.output = this.output;
            writerHolder.code = this.code;
            writerHolder.rawData = this.rawData;
            writerHolder.trainData = this.trainData;
            writerHolder.testData = this.testData;
            writerHolder.evalData = this.evalData;
            writerHolder.outputData = this.outputData;
            writerHolder.resultData = this.resultData;
            this.runner = new WorkflowRunner(writerHolder, this.DisplayName, this.workflowDesigner, RunWithLogsCleaned, true, completedCallback);

            try
            {
                this.outputTextBox.Clear();
                this.runner.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Resources.ErrorRunningDialogMessage, ExceptionHelper.FormatStackTrace(e)), Resources.ErrorRunningDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DebugWorkflow()
        {
            this.runner = new WorkflowDebugger(this.output, this.DisplayName, this.workflowDesigner, this.disableDebugViewOutput);

            try
            {
                this.outputTextBox.Clear();
                this.runner.Run();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(Resources.ErrorLoadingInDebugDialogMessage, ExceptionHelper.FormatStackTrace(e)), Resources.ErrorLoadingInDebugDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveWorkflow(string fullFilePath)
        {
            StatusViewModel.SetStatusText(Resources.SavingStatus, this.DisplayName);

            this.FullFilePath = fullFilePath;
            this.workflowDesigner.Save(this.FullFilePath);
            this.modelChanged = false;
            this.OnPropertyChanged("DisplayName");
            this.OnPropertyChanged("DisplayNameWithModifiedIndicator");

            StatusViewModel.SetStatusText(Resources.SaveSuccessfulStatus, this.DisplayName);
        }
    }
}