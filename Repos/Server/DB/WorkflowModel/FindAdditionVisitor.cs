/*
* @(#)FindAdditionVisitor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.WorkflowModel
{
	using System;

	using Newtera.WFModel;
	using Newtera.Server.DB;

	/// <summary>
	/// Traverse the new project model and find added elements, such as
	/// WorkfowModel etc, and create corresponding actions in the result.
	/// </summary>
	/// <version> 1.0.0 15 Dec 2006 </version>
	public class FindAdditionVisitor : IWFModelElementVisitor
	{
		private ProjectModel _newProjectModel;
		private ProjectModel _oldProjectModel;
		private ProjectModelCompareResult _result;
		private IDataProvider _dataProvider;

		/// <summary>
		/// Instantiate an instance of FindAdditionVisitor class
		/// </summary>
		/// <param name="newProjectModel">The new project model</param>
		/// <param name="oldProjectModel">The old project model</param>
		/// <param name="result">The compare result</param>
		public FindAdditionVisitor(ProjectModel newProjectModel,
			ProjectModel oldProjectModel,
			ProjectModelCompareResult result,
			IDataProvider dataProvider)
		{
			_newProjectModel = newProjectModel;
			_oldProjectModel = oldProjectModel;
			_result = result;
			_dataProvider = dataProvider;
		}

        /// <summary>
        /// Viste a ProjectModel element.
        /// </summary>
        /// <param name="element">A ProjectModel instance</param>
        public bool VisitProjectModel(ProjectModel element)
        {
            if (_oldProjectModel == null)
            {
                // add a new project
                IWorkflowModelAction action = new AddProjectAction(_newProjectModel, element, _dataProvider);
                _result.AddProject = action;
            }

            return true;
        }

		/// <summary>
		/// Viste a WorkflowModel element.
		/// </summary>
        /// <param name="element">A WorkflowModel instance</param>
        public bool VisitWorkflowModel(WorkflowModel element)
		{
			if (_oldProjectModel == null ||
				_oldProjectModel.Workflows[element.Name] == null)
			{
				// it is an added workflow model
				IWorkflowModelAction action = new AddWorkflowAction(_newProjectModel, element, _dataProvider);
				_result.AddAddWorkflowAction(action);
			}

            return false;
		}

		/// <summary>
        /// Viste a WorkflowModelCollection element.
		/// </summary>
        /// <param name="element">A WorkflowModelCollection instance</param>
        public bool VisitWorkflowModelCollection(WorkflowModelCollection element)
		{
            return true;
		}

        /// <summary>
        /// Viste a TrackingWorkflowInstance element.
        /// </summary>
        /// <param name="element">A TrackingWorkflowInstance instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitTrackingWorkflowInstance(NewteraTrackingWorkflowInstance element)
        {
            return false;
        }

        /// <summary>
        /// Viste a NewteraTrackingWorkflowInstanceCollection element.
        /// </summary>
        /// <param name="element">A NewteraTrackingWorkflowInstanceCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitTrackingWorkflowInstanceCollection(NewteraTrackingWorkflowInstanceCollection element)
        {
            return false;
        }

        /// <summary>
        /// Viste a NewteraActivityTrackingRecord element.
        /// </summary>
        /// <param name="element">A NewteraActivityTrackingRecord instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitActivityTrackingRecord(NewteraActivityTrackingRecord element)
        {
            return false;
        }

        /// <summary>
        /// Viste a NewteraActivityTrackingRecordCollection element.
        /// </summary>
        /// <param name="element">A NewteraActivityTrackingRecordCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitActivityTrackingRecordCollection(NewteraActivityTrackingRecordCollection element)
        {
            return false;
        }

        /// <summary>
        /// Viste a TaskSubstituteModel element.
        /// </summary>
        /// <param name="element">A TaskSubstituteModel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitTaskSubstituteModel(TaskSubstituteModel element)
        {
            return false;
        }

        /// <summary>
        /// Viste a SubjectEntryCollection element.
        /// </summary>
        /// <param name="element">A SubjectEntryCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubjectEntryCollection(SubjectEntryCollection element)
        {
            return false;
        }

        /// <summary>
        /// Viste a SubjectEntry element.
        /// </summary>
        /// <param name="element">A SubjectEntry instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubjectEntry(SubjectEntry element)
        {
            return false;
        }

        /// <summary>
        /// Viste a SubstituteEntryCollection element.
        /// </summary>
        /// <param name="element">A SubstituteEntryCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubstituteEntryCollection(SubstituteEntryCollection element)
        {
            return false;
        }

        /// <summary>
        /// Viste a SubstituteEntry element.
        /// </summary>
        /// <param name="element">A SubstituteEntry instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubstituteEntry(SubstituteEntry element)
        {
            return false;
        }
	}
}