/*
* @(#)IWFElementVisitor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// Represents an interface for visitors that traverse elements in a WFModel namespace.
	/// </summary>
	/// <version> 1.0.0 8 Dec 2006 </version>
	public interface IWFModelElementVisitor
	{
        /// <summary>
        /// Viste a ProjectModel element.
        /// </summary>
        /// <param name="element">A ProjectModel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitProjectModel(ProjectModel element);

		/// <summary>
		/// Viste a WorkflowModel element.
		/// </summary>
        /// <param name="element">A WorkflowModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitWorkflowModel(WorkflowModel element);

        /// <summary>
        /// Viste a WorkflowModelCollection element.
        /// </summary>
        /// <param name="element">A WorkflowModelCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitWorkflowModelCollection(WorkflowModelCollection element);

        /// <summary>
        /// Viste a TrackingWorkflowInstance element.
        /// </summary>
        /// <param name="element">A TrackingWorkflowInstance instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitTrackingWorkflowInstance(NewteraTrackingWorkflowInstance element);

        /// <summary>
        /// Viste a NewteraTrackingWorkflowInstanceCollection element.
        /// </summary>
        /// <param name="element">A NewteraTrackingWorkflowInstanceCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitTrackingWorkflowInstanceCollection(NewteraTrackingWorkflowInstanceCollection element);

        /// <summary>
        /// Viste a NewteraActivityTrackingRecord element.
        /// </summary>
        /// <param name="element">A NewteraActivityTrackingRecord instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitActivityTrackingRecord(NewteraActivityTrackingRecord element);

        /// <summary>
        /// Viste a NewteraActivityTrackingRecordCollection element.
        /// </summary>
        /// <param name="element">A NewteraActivityTrackingRecordCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitActivityTrackingRecordCollection(NewteraActivityTrackingRecordCollection element);

        /// <summary>
        /// Viste a TaskSubstituteModel element.
        /// </summary>
        /// <param name="element">A TaskSubstituteModel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitTaskSubstituteModel(TaskSubstituteModel element);

        /// <summary>
        /// Viste a SubjectEntryCollection element.
        /// </summary>
        /// <param name="element">A SubjectEntryCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubjectEntryCollection(SubjectEntryCollection element);

        /// <summary>
        /// Viste a SubjectEntry element.
        /// </summary>
        /// <param name="element">A SubjectEntry instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubjectEntry(SubjectEntry element);

        /// <summary>
        /// Viste a SubstituteEntryCollection element.
        /// </summary>
        /// <param name="element">A SubstituteEntryCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubstituteEntryCollection(SubstituteEntryCollection element);

        /// <summary>
        /// Viste a SubstituteEntry element.
        /// </summary>
        /// <param name="element">A SubstituteEntry instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        bool VisitSubstituteEntry(SubstituteEntry element);
	}
}