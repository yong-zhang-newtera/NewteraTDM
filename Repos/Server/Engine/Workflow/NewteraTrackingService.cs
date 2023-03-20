/*
* @(#)NewteraTrackingService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Workflow.Runtime;
using System.Workflow.Runtime.Tracking;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime.Hosting;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary> 
    /// Implements a workflow tracking service using Newtera Data Store
    /// </summary>
    /// <version> 1.0.0 04 Jan 2007 </version>
    public class NewteraTrackingService : TrackingService
    {
        public NewteraTrackingService()
        {
        }

        protected override bool TryGetProfile(Type workflowType, out TrackingProfile profile)
        {
            //Depending on the workflowType, service can return different tracking profiles
            //In this sample we're returning the same profile for all running types
            profile = GetProfile();
            return true;
        }

        protected override TrackingProfile GetProfile(Guid workflowInstanceId)
        {
            // return the same profile for all workflow instances
            return GetProfile();
        }

        protected override TrackingProfile GetProfile(Type workflowType, Version profileVersionId)
        {
            // Return the version of the tracking profile that runtime requests (profileVersionId)
            return GetProfile();
        }

        protected override bool TryReloadProfile(Type workflowType, Guid workflowInstanceId, out TrackingProfile profile)
        {
            profile = GetProfile();
            return true;
        }

        protected override TrackingChannel GetTrackingChannel(TrackingParameters parameters)
        {
            return new NewteraCustomTrackingChannel(parameters);
        }

        #region Tracking Profile Creation

        // Reads a file containing an XML representation of a Tracking Profile
        private static TrackingProfile GetProfile()
        {
            // Create a Tracking Profile
            TrackingProfile profile = new TrackingProfile();
            profile.Version = new Version("1.0.0.0");

            // Add a TrackPoint to cover all activity status events
            ActivityTrackPoint activityTrackPoint = new ActivityTrackPoint();
            ActivityTrackingLocation activityLocation = new ActivityTrackingLocation(typeof(Activity));
            activityLocation.MatchDerivedTypes = true;
            WorkflowTrackingLocation wLocation = new WorkflowTrackingLocation();

            IEnumerable<ActivityExecutionStatus> statuses = Enum.GetValues(typeof(ActivityExecutionStatus)) as IEnumerable<ActivityExecutionStatus>;
            foreach (ActivityExecutionStatus status in statuses)
            {
                activityLocation.ExecutionStatusEvents.Add(status);
            }

            activityTrackPoint.MatchingLocations.Add(activityLocation);
            profile.ActivityTrackPoints.Add(activityTrackPoint);

            // Add a TrackPoint to cover all workflow status events
            WorkflowTrackPoint workflowTrackPoint = new WorkflowTrackPoint();
            workflowTrackPoint.MatchingLocation = new WorkflowTrackingLocation();
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Aborted);
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Completed);
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Idle);
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Resumed);
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Started);
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Suspended);
            workflowTrackPoint.MatchingLocation.Events.Add(TrackingWorkflowEvent.Terminated);
            profile.WorkflowTrackPoints.Add(workflowTrackPoint);

            // Add a TrackPoint to cover all user track points
            /*
            UserTrackPoint userTrackPoint = new UserTrackPoint();
            UserTrackingLocation userLocation = new UserTrackingLocation();
            userLocation.ActivityType = typeof(Activity);
            userLocation.MatchDerivedActivityTypes = true;
            userLocation.ArgumentType = typeof(object);
            userLocation.MatchDerivedArgumentTypes = true;
            userTrackPoint.MatchingLocations.Add(userLocation);
            profile.UserTrackPoints.Add(userTrackPoint);
            */

            return profile;
        }

        #endregion
    }
}
