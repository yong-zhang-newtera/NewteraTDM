/*
* @(#)Restore.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio.Util
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Text;
	using System.Windows.Forms;

	using ICSharpCode.SharpZipLib.Zip;

    using Newtera.Common.Core;
    using Newtera.WFModel;
    using Newtera.WinClientCommon;
    using Newtera.WorkflowStudioControl;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Restore a workflow project from a backup file
	/// </summary>
	/// <version> 1.0.0 29 Dec 2008 </version>
	public class Restore
	{
		private bool _isCancelled;
		private bool _isOverride;
		private string _packFileName;
		private string _dataDirPath;
        private string _projectModelDir;
        private string _instanceDataDir;
        private string _errorLog;
        private ProjectModel _projectModel;
		private MethodInvoker _confirmCallback;
        private WorkflowModelServiceStub _modelService;
        private WorkflowTrackingServiceStub _trackingService;
		private WorkInProgressDialog _workingDialog;

		private const string PROJECT_MODEL_DIR = "project";
		private const string INSTANCE_DATA_DIR = "instanceData";
        private const string ERROR_LOG = "ErrorLog.txt";

		/// <summary>
		/// Instantiate an instance of Restore class
		/// </summary>
		/// <param name="packFileName">The zipped file name.</param>
		/// <param name="dataDirPath">The directory path where to put data files for packing.</param>
		public Restore(string packFileName, string dataDirPath)
		{
			_packFileName = packFileName;
			_dataDirPath = dataDirPath;
            _projectModelDir = @".\" + Restore.PROJECT_MODEL_DIR;
            _instanceDataDir = @".\" + Restore.INSTANCE_DATA_DIR;
            _errorLog = @".\" + Restore.ERROR_LOG;
			_isCancelled = false;
			_isOverride = false;
			_confirmCallback = null;
			_workingDialog = null;
            _modelService = null;
		}

		/// <summary>
		/// Gets or sets the information indicating whether to cancel a backup job.
		/// </summary>
		public bool IsCancelled
		{
			get
			{
				return _isCancelled;
			}
			set
			{
				_isCancelled = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to override an existing database.
		/// </summary>
		public bool IsOverride
		{
			get
			{
				return _isOverride;
			}
			set
			{
				_isOverride = value;
			}
		}

		/// <summary>
		/// Gets or sets the working dialog instance
		/// </summary>
		public WorkInProgressDialog WorkingDialog
		{
			get
			{
				return _workingDialog;
			}
			set
			{
				_workingDialog = value;
			}
		}

		/// <summary>
		/// Gets or sets the callback for confirming whether to override an existing database.
		/// </summary>
		public MethodInvoker ConfirmCallback
		{
			get
			{
				return _confirmCallback;
			}
			set
			{
				_confirmCallback = value;
			}
		}

		/// <summary>
		/// Restore a database from a zipped backup file.
		/// </summary>
		public void Perform()
		{
            StreamWriter sw = null;
			string msg = _workingDialog.DisplayText;

			string workingDir = Directory.GetCurrentDirectory();

			try
			{
				// delete the existing directory
				if (Directory.Exists(_dataDirPath))
				{
					Directory.Delete(_dataDirPath, true);
				}

				// create directories for exported files
				Directory.CreateDirectory(_dataDirPath);

				Directory.SetCurrentDirectory(_dataDirPath);

                if (File.Exists(this._errorLog))
                {
                    File.Delete(this._errorLog);
                }

                sw = new StreamWriter(this._errorLog);

				// unpack the backup file into separated files
				UnpackFiles();

                _projectModel = ReadProjectModel();

                // check if the project of same name exists?
                _modelService = new WorkflowModelServiceStub();

                _trackingService = new WorkflowTrackingServiceStub();

                ProjectInfo[] projectInfos = _modelService.GetExistingProjectInfos();
                bool isExist = false;
                for (int i = 0; i < projectInfos.Length; i++)
                {
                    if (projectInfos[i].Name.ToUpper() == _projectModel.Name.ToUpper() &&
                        projectInfos[i].Version == _projectModel.Version.ToUpper())
                    {
                        isExist = true;
                        break;
                    }
                }

				// confirm with user whether to override the database
				if (isExist && _confirmCallback != null)
				{
					_confirmCallback.DynamicInvoke(null);
				}

				if (isExist && IsOverride)
				{
					// delete the existing project
					try
					{
                        WorkingDialog.DisplayText = MessageResourceManager.GetString("WorkflowStudioApp.DeletingProject");
                        _modelService.DeleteProject(ConnectionStringBuilder.Instance.Create(), _projectModel.Name, _projectModel.Version);
					}
					catch (Exception ex)
					{
						// ignore the errors
                        sw.WriteLine("************ Message Begin ***********");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine("************ Message End *************");
                        sw.WriteLine("");
					}
				}

				if (!isExist || IsOverride)
				{
                    // Create the project and get saved project model which
                    // contains new ids
                    ProjectModel savedProjectModel = CreateProject();

                    // restore the workflow instances related data
                    string fileName;

                    // During the restoring project, project and workflow model's ids will be re-created,
                    // while the workflow instance ids and the subscription ids will remain the same
                    // across backup and restore
                    int pageIndex;
                    bool backupedByOldVersion;
                    foreach (WorkflowModel workflowModel in savedProjectModel.Workflows)
                    {
                        pageIndex = 0;
                        backupedByOldVersion = false;
                        while (true)
                        {
                            // backward compatibility, there was no page index before 4.2.0
                            fileName = _instanceDataDir + @"\" + workflowModel.Name + "_tracking.txt";
                            if (!File.Exists(fileName))
                            {
                                // try file name with page index
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_tracking_" + pageIndex + ".txt";

                                if (!File.Exists(fileName))
                                {
                                    // no more file to read, exit
                                    break;
                                }
                            }
                            else
                            {
                                backupedByOldVersion = true;
                            }

                            Newtera.WFModel.NewteraTrackingWorkflowInstanceCollection workflowTrackingInfos = new NewteraTrackingWorkflowInstanceCollection();
                            workflowTrackingInfos.Read(fileName);

                            RestoreTrackingInfos(workflowModel, workflowTrackingInfos);

                            if (backupedByOldVersion)
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_state.txt";
                            }
                            else
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_state_" + pageIndex + ".txt";
                            }

                            if (File.Exists(fileName))
                            {
                                Newtera.WFModel.WorkflowInstanceStateInfoCollection stateInfos = new WorkflowInstanceStateInfoCollection();
                                stateInfos.Read(fileName);

                                RestoreStateInfos(workflowModel, stateInfos);
                            }

                            if (backupedByOldVersion)
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_task.txt";
                            }
                            else
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_task_" + pageIndex + ".txt";
                            }

                            if (File.Exists(fileName))
                            {
                                Newtera.WFModel.TaskInfoCollection taskInfos = new TaskInfoCollection();
                                taskInfos.Read(fileName);

                                RestoreTaskInfos(workflowModel, taskInfos);
                            }

                            if (backupedByOldVersion)
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_wfevent.txt";
                            }
                            else
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_wfevent_" + pageIndex  + ".txt";
                            }

                            if (File.Exists(fileName))
                            {
                                Newtera.WFModel.WorkflowEventSubscriptionCollection wfEventSubscriptions = new WorkflowEventSubscriptionCollection();
                                wfEventSubscriptions.Read(fileName);

                                RestoreWorkflowEventSubscriptions(workflowModel, wfEventSubscriptions);
                            }

                            if (backupedByOldVersion)
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_dbevent.txt";
                            }
                            else
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_dbevent_" + pageIndex + ".txt";
                            }

                            if (File.Exists(fileName))
                            {
                                Newtera.WFModel.DBEventSubscriptionCollection dbEventSubscriptions = new DBEventSubscriptionCollection();
                                dbEventSubscriptions.Read(fileName);

                                RestoreDBEventSubscriptions(workflowModel, dbEventSubscriptions);
                            }

                            if (backupedByOldVersion)
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_binding.txt";
                            }
                            else
                            {
                                fileName = _instanceDataDir + @"\" + workflowModel.Name + "_binding_" + pageIndex + ".txt";
                            }

                            if (File.Exists(fileName))
                            {
                                Newtera.WFModel.WorkflowInstanceBindingInfoCollection bindingInfos = new WorkflowInstanceBindingInfoCollection();
                                bindingInfos.Read(fileName);

                                RestoreBindingInfos(workflowModel, bindingInfos);
                            }

                            if (backupedByOldVersion)
                            {
                                break; // there is no page data, just exits
                            }
                            else
                            {
                                pageIndex++;
                            }
                        }
                    }
				}
			}
			finally
			{
				// restore current working dir
				Directory.SetCurrentDirectory(workingDir);

                if (sw != null)
                {
                    sw.Close();
                }

				// cleanup data dir
				if (Directory.Exists(_dataDirPath))
				{
					Directory.Delete(_dataDirPath, true);
				}

				_workingDialog.DisplayText = msg;
			}
		}

		/// <summary>
		/// Unpack the files into a directory
		/// </summary>
		private void UnpackFiles()
		{
			FileStream streamWriter = null;
			ZipInputStream s = null;
			ZipEntry theEntry;

			try
			{
                WorkingDialog.DisplayText = MessageResourceManager.GetString("WorkflowStudioApp.UnpackData");

				s = new ZipInputStream(File.OpenRead(_packFileName));
		
				while ((theEntry = s.GetNextEntry()) != null) 
				{			
					string directoryName = Path.GetDirectoryName(theEntry.Name);
					string fileName      = Path.GetFileName(theEntry.Name);
			
					// create directory
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
			
					if (fileName != String.Empty) 
					{
						streamWriter = File.Create(theEntry.Name);
				
						int size = 2048;
						byte[] data = new byte[2048];
						while (true) 
						{
							size = s.Read(data, 0, data.Length);
							if (size > 0) 
							{
								streamWriter.Write(data, 0, size);
							} 
							else 
							{
								break;
							}
						}
				
						streamWriter.Close();
						streamWriter = null;
					}
				}				
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}

				if (s != null)
				{
					s.Close();
				}
			}
		}

        /// <summary>
        /// Read project model from the files
        /// </summary>
        /// <returns></returns>
        private ProjectModel ReadProjectModel()
        {
            ProjectModel project = null;
            string[] fileNames = Directory.GetFiles(_projectModelDir);
            string projectFilePath = null;
            // find the project file
            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(".wfproj"))
                {
                    projectFilePath = fileName;
                    break;
                }
            }

            if (projectFilePath != null)
            {
                project = new ProjectModel("");
                project.Read(projectFilePath);

                XaclPolicy xaclPolicy = project.Policy;
                string xaclFileName = projectFilePath.Replace(".wfproj", ".xacl");
                xaclPolicy.Read(xaclFileName);

                // Set the type of data sources of the workflow models, so that workflow
                // models can load data from file.
                foreach (WorkflowModel workflowModel in project.Workflows)
                {
                    workflowModel.XomlFileBaseDir = _projectModelDir;
                    workflowModel.SourceStorageType = StorageType.File;
                }

                project.LoadAll(); // load the workflow definition from files

                // also clear the IDs that may have been saved to the file since
                // the IDs will be assigned by the server when the project is saved to
                // the database
                project.ClearIds();
            }
            else
            {
                throw new Exception("Unable to find a .wfproj file in " + _projectModelDir);
            }

            return project;
        }

        /// <summary>
		/// Create the project
		/// </summary>
        /// <returns>The saved model which contains new ids</returns>
        private ProjectModel CreateProject()
        {
            ProjectModel newProjectModel = null;

            WorkingDialog.DisplayText = MessageResourceManager.GetString("WorkflowStudioApp.CreateProject");

            StringBuilder projectBuilder, policyBuilder;
            StringWriter projectWriter, policyWriter;

            // save project xml to the database
            projectBuilder = new StringBuilder();
            projectWriter = new StringWriter(projectBuilder);
            _projectModel.Write(projectWriter);

            // Save the project policy right after the project model
            policyBuilder = new StringBuilder();
            policyWriter = new StringWriter(policyBuilder);
            _projectModel.Policy.Write(policyWriter);

            DateTime modifiedTime = _modelService.SaveProject(ConnectionStringBuilder.Instance.Create(),
                _projectModel.Name,
                _projectModel.Version,
                projectBuilder.ToString(), policyBuilder.ToString());

            // saving worklfow data to the database through a database storage provider
            DatabaseStorageProvider storageProvider = new DatabaseStorageProvider(_modelService, _projectModel);
            foreach (WorkflowModel workflowModel in _projectModel.Workflows)
            {
                // currently, we have no way to find out whether data in the workflow
                // model has been changed or not, therefore, we just save the workflow
                // data anyway.
                storageProvider.WorkflowModel = workflowModel;
                workflowModel.Save(storageProvider);
            }

            // read the project model from database since it contains identifiers assigned
            // to the workflows
            string xmlString = _modelService.GetProject(ConnectionStringBuilder.Instance.Create(),
                _projectModel.Name, _projectModel.Version);
            newProjectModel = new ProjectModel(_projectModel.Name);
            StringReader reader = new StringReader(xmlString);
            newProjectModel.Read(reader);

            return newProjectModel;
        }

        /// <summary>
        /// Restore workflow instance tracking infos
        /// </summary>
        private void RestoreTrackingInfos(WorkflowModel workflowModel, NewteraTrackingWorkflowInstanceCollection workflowTrackingInfos)
        {
            string msg = string.Format(MessageResourceManager.GetString("WorkflowStudioApp.RestoreTrackingInfos"), workflowModel.Name);
            WorkingDialog.DisplayText = msg;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            workflowTrackingInfos.Write(writer);

            _trackingService.SetTrackingWorkflowInstances(ConnectionStringBuilder.Instance.Create(),
                workflowModel.ID, builder.ToString());
        }

        /// <summary>
        /// Restore workflow instance state infos
        /// </summary>
        private void RestoreStateInfos(WorkflowModel workflowModel, WorkflowInstanceStateInfoCollection stateInfos)
        {
            string msg = string.Format(MessageResourceManager.GetString("WorkflowStudioApp.RestoreStateInfos"), workflowModel.Name);
            WorkingDialog.DisplayText = msg;

            foreach (WorkflowInstanceStateInfo stateInfo in stateInfos)
            {
                WorkflowInstanceStateInfo newStateInfo = new WorkflowInstanceStateInfo();
                newStateInfo.WorkflowInstanceId = stateInfo.WorkflowInstanceId;
                newStateInfo.Unlocked = stateInfo.Unlocked;
                newStateInfo.ModifiedTime = stateInfo.ModifiedTime;
                newStateInfo.State = GetByteArray(stateInfo.WorkflowInstanceId);
                _trackingService.SetWorkflowInstanceStateInfo(ConnectionStringBuilder.Instance.Create(), newStateInfo);
            }
        }

        /// <summary>
        /// Restore workflow instance task infos
        /// </summary>
        private void RestoreTaskInfos(WorkflowModel workflowModel, TaskInfoCollection taskinfos)
        {
            string msg = string.Format(MessageResourceManager.GetString("WorkflowStudioApp.RestoreTaskInfos"), workflowModel.Name);
            WorkingDialog.DisplayText = msg;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            taskinfos.Write(writer);

            _trackingService.SetTaskInfos(ConnectionStringBuilder.Instance.Create(), builder.ToString());
        }

        /// <summary>
        /// Restore workflow instance's subscriptions to workflow events
        /// </summary>
        private void RestoreWorkflowEventSubscriptions(WorkflowModel workflowModel, WorkflowEventSubscriptionCollection subscriptions)
        {
            string msg = string.Format(MessageResourceManager.GetString("WorkflowStudioApp.RestoreWFEventSubscriptions"), workflowModel.Name);
            WorkingDialog.DisplayText = msg;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            subscriptions.Write(writer);

            _trackingService.SetWorkflowEventSubscriptions(ConnectionStringBuilder.Instance.Create(), builder.ToString());
        }

        /// <summary>
        /// Restore workflow instance's subscriptions to database events
        /// </summary>
        private void RestoreDBEventSubscriptions(WorkflowModel workflowModel, DBEventSubscriptionCollection subscriptions)
        {
            string msg = string.Format(MessageResourceManager.GetString("WorkflowStudioApp.RestoreDBEventSubscriptions"), workflowModel.Name);
            WorkingDialog.DisplayText = msg;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            subscriptions.Write(writer);

            _trackingService.SetDBEventSubscriptions(ConnectionStringBuilder.Instance.Create(), builder.ToString());
        }

        /// <summary>
        /// Restore workflow instance's bindings to the data instances
        /// </summary>
        private void RestoreBindingInfos(WorkflowModel workflowModel, WorkflowInstanceBindingInfoCollection bindingInfos)
        {
            string msg = string.Format(MessageResourceManager.GetString("WorkflowStudioApp.RestoreBindingInfos"), workflowModel.Name);
            WorkingDialog.DisplayText = msg;

            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            bindingInfos.Write(writer);

            _trackingService.SetBindingDataInstanceInfos(ConnectionStringBuilder.Instance.Create(),
                workflowModel.ID,
                builder.ToString());
        }

        /// <summary>
        /// Gets a byte array representing a workflow instance state from a file
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>A byte array</returns>
        private byte[] GetByteArray(string workflowInstanceId)
        {
            byte[] state = null;
            string fileName = _instanceDataDir + @"\" + workflowInstanceId + ".state";
            if (File.Exists(fileName))
            {
                state = File.ReadAllBytes(fileName);
            }

            return state;
        }
	}
}