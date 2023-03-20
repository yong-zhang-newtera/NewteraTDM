/*
* @(#)ElementTypeEnum.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	/// <summary>
	/// Specify the types of elements in WFModel namespace.
	/// </summary>
	public enum ElementType
	{
		/// <summary>
		/// Unknown element
		/// </summary>
		Unknown,
        /// <summary>
        /// Project
        /// </summary>
        Project,
		/// <summary>
		/// Workflow element
		/// </summary>
		Workflow,
        /// <summary>
        /// Workflow collection
        /// </summary>
        Workflows,
        /// <summary>
        /// Collection
        /// </summary>
        Collection,

        /// <summary>
        /// TrackingInstance
        /// </summary>
        TrackingInstance,

        /// <summary>
        /// TrackingInstances
        /// </summary>
        TrackingInstances,

        /// <summary>
        /// ActivityTrackingRecord
        /// </summary>
        ActivityTrackingRecord,

        /// <summary>
        /// ActivityTrackingRecordCollection
        /// </summary>
        ActivityTrackingRecordCollection,

        /// <summary>
        /// TaskSubstituteModel
        /// </summary>
        TaskSubstituteModel,

        /// <summary>
        /// SubjectEntry
        /// </summary>
        SubjectEntry,

        /// <summary>
        /// SubjectEntries
        /// </summary>
        SubjectEntries,

        /// <summary>
        /// SubstituteEntry
        /// </summary>
        SubstituteEntry,

        /// <summary>
        /// SubstituteEntries
        /// </summary>
        SubstituteEntries
	}
}