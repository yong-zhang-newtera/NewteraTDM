
namespace Newtera.MLStudio.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Presentation;
    using System.Activities.XamlIntegration;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Newtera.MLStudio.Properties;
    using Newtera.MLStudio.Utilities;
    using Newtera.MLStudio.ViewModel;
    using Newtera.MLActivities.MLConfig;

    public class WorkflowRunner : IWorkflowRunner
    {
        private TextWriterHolder writerHolder;
        private WorkflowApplication workflowApplication;
        private bool running;
        private WorkflowDesigner workflowDesigner;
        private MLExperimentManager mlExperimentManager;
        private bool cleanLogs;
        private ExperimentRunner experimentRunner;
        private bool isDryRun;
        private Action<object> completedCallback;

        private string workflowName;

        public WorkflowRunner(TextWriterHolder writerHolder, string workflowName, WorkflowDesigner workflowDesigner, bool cleanLogs) :
            this(writerHolder, workflowName, workflowDesigner, cleanLogs, false, null)
        {
        }

        public WorkflowRunner(TextWriterHolder writerHolder, string workflowName, WorkflowDesigner workflowDesigner, bool cleanLogs, bool isDryRun, Action<object> completedCallback)
        {
            this.writerHolder = writerHolder;
            this.workflowName = workflowName;
            this.workflowDesigner = workflowDesigner;
            this.mlExperimentManager = new MLExperimentManager();
            this.cleanLogs = cleanLogs;
            this.experimentRunner = null;
            this.isDryRun = isDryRun;
            this.completedCallback = completedCallback;
        }

        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }

        public void Abort()
        {
            /*
            if (this.running && this.workflowApplication != null)
            {
                StatusViewModel.SetStatusText(Resources.AbortingStatus, this.workflowName);
                this.workflowApplication.Abort();
            }
            */
            if (experimentRunner != null)
            {
                experimentRunner.Abort();
            }
            this.running = false;
            StatusViewModel.SetStatusText(Resources.AbortedStatus, this.workflowName);
        }

        public void Run()
        {
            this.workflowDesigner.Flush();
            MemoryStream ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(this.workflowDesigner.Text));

            DynamicActivity activityToRun = ActivityXamlServices.Load(ms) as DynamicActivity;

            this.workflowApplication = new WorkflowApplication(activityToRun);

            this.workflowApplication.Extensions.Add(this.mlExperimentManager);
            this.workflowApplication.Extensions.Add(this.writerHolder.output);
            if (this.isDryRun)
            {
                this.workflowApplication.Completed = this.WorkflowDryRunCompleted;
            }
            else
            {
                this.workflowApplication.Completed = this.WorkflowCompleted;
            }
            this.workflowApplication.Aborted = this.WorkflowAborted;
            this.workflowApplication.OnUnhandledException = this.WorkflowUnhandledException;
            StatusViewModel.SetStatusText(Resources.RunningStatus, this.workflowName);

            try
            {
                this.running = true;
                if (!this.isDryRun)
                {
                    ProgressBarModel.TheInstance.SetIsIndeterminateStatus(true);
                }
                this.workflowApplication.Run();
            }
            catch (Exception e)
            {
                this.writerHolder.output.WriteLine(ExceptionHelper.FormatStackTrace(e));
                StatusViewModel.SetStatusText(Resources.ExceptionStatus, this.workflowName);
                ProgressBarModel.TheInstance.SetIsIndeterminateStatus(false);
                this.running = false;
            }
        }

        public MLExperimentManager ExperimentManager
        {
            get
            {
                return this.mlExperimentManager;
            }
        }

        private void WorkflowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            try
            {
                experimentRunner = new ExperimentRunner(this.mlExperimentManager, cleanLogs);

                try
                {
                    experimentRunner.Run(this.writerHolder);
                }
                finally
                {
                    ProgressBarModel.TheInstance.SetIsIndeterminateStatus(false);
                }
            }
            catch (Exception ex)
            {
                writerHolder.output.WriteLine("############################################################################");
                writerHolder.output.WriteLine("#");
                writerHolder.output.WriteLine("# Exception Message : " + ex.Message);
                writerHolder.output.WriteLine("#");
                writerHolder.output.WriteLine("############################################################################");
                throw ex;
            }
            finally
            {
                experimentRunner = null;
            }

            this.running = false;
            StatusViewModel.SetStatusText(string.Format(Resources.CompletedStatus, e.CompletionState.ToString()), this.workflowName);
        }

        private void WorkflowDryRunCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            // save and run each configuration in the manager
            MLComponentCollection executableConfigurations = this.mlExperimentManager.ExecutableConfigurations;

            this.running = false;
            StatusViewModel.SetStatusText(string.Format(Resources.CompletedStatus, e.CompletionState.ToString()), this.workflowName);

            if (this.completedCallback != null)
            {
                this.completedCallback(this.mlExperimentManager);
            }
        }

        private void WorkflowAborted(WorkflowApplicationAbortedEventArgs e)
        {
            this.running = false;
            StatusViewModel.SetStatusText(Resources.AbortedStatus, this.workflowName);
        }

        private UnhandledExceptionAction WorkflowUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(ExceptionHelper.FormatStackTrace(e.UnhandledException));
            return UnhandledExceptionAction.Terminate;
        }
    }
}
