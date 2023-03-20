/*
* @(#)Backup.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio.Util
{
	using System;
	using System.IO;
	using System.Xml;
    using System.Data;
	using System.Text;
    using System.ComponentModel;
    using System.Collections.Specialized;

	using ICSharpCode.SharpZipLib.Checksums;
	using ICSharpCode.SharpZipLib.Zip;
	using ICSharpCode.SharpZipLib.GZip;

	using Newtera.Common.Core;
    using Newtera.WFModel;
    using Newtera.WinClientCommon;
    using Newtera.WorkflowStudioControl;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Back the worklfow data, including project model, workflow models, workflow instances, and tasks </summary>
	/// <version> 1.0.0 20 Dec 2008 </version>
	public class Backup
	{
		private bool _isCancelled;
		private string _packFileName;
		private string _projectName;
        private string _projectVersion;
		private string _dataDirPath;
		private string _projectModelDir;
		private string _instanceDataDir;
		private WorkInProgressDialog _workingDialog;
        private ProjectModel _project;

		private const string PROJECT_MODEL_DIR = "project";
		private const string INSTANCE_DATA_DIR = "instanceData";

		private const int PAGE_SIZE = 50;

		/// <summary>
		/// Instantiate an instance of Backup class
		/// </summary>
		/// <param name="packFileName">The zipped file name.</param>
		/// <param name="projectName">The project name.</param>
        /// <param name="projectVersion">The project version.</param>
		/// <param name="dataDirPath">The directory path where to put data files for packing.</param>
		public Backup(string packFileName, string projectName, string projectVersion, string dataDirPath)
		{
			_packFileName = packFileName;
            _projectName = projectName;
            _projectVersion = projectVersion;
			_dataDirPath = dataDirPath;
            _projectModelDir = @".\" + Backup.PROJECT_MODEL_DIR;
            _instanceDataDir = @".\" + Backup.INSTANCE_DATA_DIR;
			_isCancelled = false;
			_workingDialog = null;
            _project = null;
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
		/// Pack data of the given schema into a zipped file.
		/// </summary>
		public void Pack()
		{
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

				// set _dataDirPath as the current working dir
				Directory.SetCurrentDirectory(_dataDirPath);

				if (!Directory.Exists(_projectModelDir))
				{
					Directory.CreateDirectory(_projectModelDir);
				}

				if (!Directory.Exists(_instanceDataDir))
				{
					Directory.CreateDirectory(_instanceDataDir);
				}

				// output the project model
                WriteProjectModel();

				if (IsCancelled)
				{
					return;
				}

				// output workflow instance data
				WriteInstanceData();

				if (IsCancelled)
				{
					return;
				}

				// create the zipped file
				PackFiles();
			}
			finally
			{
				Directory.SetCurrentDirectory(workingDir);

				// cleanup data dir
				if (Directory.Exists(_dataDirPath))
				{
					Directory.Delete(_dataDirPath, true);
				}

				_workingDialog.DisplayText = msg;
			}
		}

		/// <summary>
		/// Write the project model to files
		/// </summary>
        private void WriteProjectModel()
		{
            WorkingDialog.DisplayText = MessageResourceManager.GetString("WorkflowStudioApp.ProjectModel");

            WorkflowModelServiceStub webService = new WorkflowModelServiceStub();

			if (IsCancelled)
			{
				return;
			}

            // read the project model from database
            string xmlString = webService.GetProject(ConnectionStringBuilder.Instance.Create(), _projectName, _projectVersion);
            _project = new ProjectModel(_projectName);
            StringReader reader = new StringReader(xmlString);
            _project.Read(reader);

            // read project access control policy
            string policyString = webService.GetProjectPolicy(ConnectionStringBuilder.Instance.Create(),
                _project.Name, _project.Version);
            reader = new StringReader(policyString);
            _project.Policy.Read(reader);

            // Create a DatabaseStorageProvider to each of WorkflowModel so that
            // the various data of a workflow can be loaded from database on demand
            DatabaseStorageProvider storageProvider;
            foreach (WorkflowModel workflowModel in _project.Workflows)
            {
                storageProvider = new DatabaseStorageProvider(webService, _project);
                storageProvider.WorkflowModel = workflowModel;
                workflowModel.SourceStorageType = StorageType.Database;
                workflowModel.DatabaseStorageProvider = storageProvider;
            }

            // load workflow models contained in the project
            _project.LoadAll();

			// write the project model data into files
            string projectFileName = _projectModelDir + @"\" + _projectName + ".wfproj";

            _project.Write(projectFileName);

            // write workflow's xoml, rules, layout data to separate files
            string baseDir = Path.GetDirectoryName(projectFileName);
            foreach (WorkflowModel workflowModel in _project.Workflows)
            {
                workflowModel.Save(baseDir);
            }

            // save the xacl policy to a file
            string xaclPolicyFileName = projectFileName.Replace(".wfproj", ".xacl");
            XaclPolicy xaclPolicy = _project.Policy;
            xaclPolicy.Write(xaclPolicyFileName);
		}

		/// <summary>
		/// Write workflow instance data into files
		/// </summary>
        private void WriteInstanceData()
		{
            TrackingQueryService tackingQueryService = new TrackingQueryService();

            DateTime from = DateTime.Now;
            DateTime until = DateTime.Now.AddDays(1);
            NewteraTrackingWorkflowInstanceCollection queriedWorkflows;

            foreach (WorkflowModel workflowModel in _project.Workflows)
            {
                if (IsCancelled)
                {
                    break;
                }

                int pageIndex = 0;
                int pageSize = PAGE_SIZE;
                string fileName;
                while (true)
                {
                    // get all the workflow instance data for a given workflow without using conditions
                    queriedWorkflows = tackingQueryService.GetWorkflows(workflowModel.ID,
                        "",
                        from,
                        until,
                        false,
                        pageIndex,
                        pageSize);

                    if (queriedWorkflows == null || queriedWorkflows.Count == 0)
                    {
                        break;
                    }

                    // write the data to a fle
                    fileName = _instanceDataDir + @"\" + workflowModel.Name + "_tracking_" + pageIndex + ".txt";

                    // backup the data in WF_WORKFLOW_INSTANCE and WF_ACTIVITY_INSTANCE
                    queriedWorkflows.Write(fileName);

                    Newtera.WFModel.WorkflowInstanceBindingInfo bindingInfo;
                    Newtera.WFModel.WorkflowInstanceBindingInfoCollection bindingInfos = new WorkflowInstanceBindingInfoCollection();
                    Newtera.WFModel.TaskInfoCollection taskInfos = new TaskInfoCollection();
                    Newtera.WFModel.WorkflowInstanceStateInfo stateInfo;
                    Newtera.WFModel.WorkflowInstanceStateInfoCollection stateInfos = new WorkflowInstanceStateInfoCollection();
                    Newtera.WFModel.DBEventSubscriptionCollection dbEventSubscriptions = new DBEventSubscriptionCollection();
                    Newtera.WFModel.WorkflowEventSubscriptionCollection wfEventSubscriptions = new WorkflowEventSubscriptionCollection();

                    foreach (NewteraTrackingWorkflowInstance trackingInstance in queriedWorkflows)
                    {
                        if (IsCancelled)
                        {
                            break;
                        }

                        // Backup the data in WF_INSTANCE_MAP table
                        bindingInfo = tackingQueryService.GetBindingDataInstanceInfo(trackingInstance.WorkflowInstanceId);
                        if (bindingInfo != null)
                        {
                            bindingInfos.Add(bindingInfo);
                        }

                        // Backup the data in WF_INSTANCE_STATE table
                        stateInfo = tackingQueryService.GetWorkflowInstanceStateInfo(trackingInstance.WorkflowInstanceId);
                        if (stateInfo != null)
                        {
                            stateInfos.Add(stateInfo);
                        }

                        // Backup the data in WF_EVENT_SUBSCRIPTION table
                        Newtera.WFModel.DBEventSubscriptionCollection tempDBEventSubscriptions = tackingQueryService.GetDBEventSubscriptions(trackingInstance.WorkflowInstanceId);
                        if (tempDBEventSubscriptions != null)
                        {
                            foreach (Newtera.WFModel.DBEventSubscription subscription in tempDBEventSubscriptions)
                            {
                                dbEventSubscriptions.Add(subscription);
                            }
                        }

                        // Backup the data in WF_WFEVENT_SUBSCRIPTION table
                        Newtera.WFModel.WorkflowEventSubscriptionCollection tempWorkflowEventSubscriptions = tackingQueryService.GetWorkflowEventSubscriptions(trackingInstance.WorkflowInstanceId);
                        if (tempWorkflowEventSubscriptions != null)
                        {
                            foreach (Newtera.WFModel.WorkflowEventSubscription subscription in tempWorkflowEventSubscriptions)
                            {
                                wfEventSubscriptions.Add(subscription);
                            }
                        }

                        // back up task info
                        TaskInfoCollection tempTaskInfos = tackingQueryService.GetTaskInfos(trackingInstance.WorkflowInstanceId);
                        if (tempTaskInfos != null)
                        {
                            foreach (Newtera.WFModel.TaskInfo taskInfo in tempTaskInfos)
                            {
                                taskInfos.Add(taskInfo);
                            }
                        }
                    }

                    // Backup the data in WF_INSTANCE_MAP table
                    fileName = _instanceDataDir + @"\" + workflowModel.Name + "_binding_" +  pageIndex +".txt";
                    bindingInfos.Write(fileName);

                    // Backup the data in WF_INSTANCE_STATE table
                    fileName = _instanceDataDir + @"\" + workflowModel.Name + "_state_" + pageIndex  + ".txt";
                    stateInfos.Write(fileName);

                    // write each state data as an individual binary file
                    foreach (Newtera.WFModel.WorkflowInstanceStateInfo sInfo in stateInfos)
                    {
                        fileName = _instanceDataDir + @"\" + sInfo.WorkflowInstanceId + ".state";
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        {
                            binWriter.Write(sInfo.State);
                        }
                    }

                    // Backup the data in WF_EVENT_SUBSCRIPTION table
                    fileName = _instanceDataDir + @"\" + workflowModel.Name + "_dbevent_" + pageIndex + ".txt";
                    dbEventSubscriptions.Write(fileName);

                    // Backup the data in WF_WFEVENT_SUBSCRIPTION table
                    fileName = _instanceDataDir + @"\" + workflowModel.Name + "_wfevent_" + pageIndex + ".txt";
                    wfEventSubscriptions.Write(fileName);

                    // Backup the task infos
                    fileName = _instanceDataDir + @"\" + workflowModel.Name + "_task_" + pageIndex + ".txt";
                    taskInfos.Write(fileName);

                    pageIndex++;
                }
            }
		}

		/// <summary>
		/// Pack the files into a zipped file
		/// </summary>
		private void PackFiles()
		{
			ZipEntry entry;
			FileStream fs = null;
			ZipOutputStream s = null;

			try
			{
                WorkingDialog.DisplayText = MessageResourceManager.GetString("WorkflowStudioApp.PackFiles");

				s = new ZipOutputStream(File.Create(_packFileName));

				s.SetLevel(6); // 0 - store only to 9 - means best compression

				string[] subDirs = Directory.GetDirectories(".");

				for (int i = 0; i < subDirs.Length; i++)
				{
					if (IsCancelled)
					{
						break;
					}

					string[] filenames = Directory.GetFiles(subDirs[i]);
				
					foreach (string file in filenames) 
					{
						if (IsCancelled)
						{
							break;
						}

						fs = File.OpenRead(file);
					
						byte[] buffer = new byte[fs.Length];
						fs.Read(buffer, 0, buffer.Length);
						entry = new ZipEntry(file);
					
						entry.DateTime = DateTime.Now;
						entry.Size = fs.Length;
						fs.Close();
						fs = null;
					
						s.PutNextEntry(entry);
					
						s.Write(buffer, 0, buffer.Length);
					}
				}
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
				}

				if (s != null)
				{
					s.Finish();
					s.Close();
				}
			}
		}
    }
}