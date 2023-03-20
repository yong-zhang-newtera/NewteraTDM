﻿
namespace Newtera.MLStudio.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class VisualTrackingParticipant : TrackingParticipant
    {
        public event EventHandler<TrackingEventArgs> TrackingRecordReceived;

        public Dictionary<string, Activity> ActivityIdToWorkflowElementMap { get; set; }
        
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            this.OnTrackingRecordReceived(record, timeout);
        }

        protected void OnTrackingRecordReceived(TrackingRecord record, TimeSpan timeout)
        {
            if (this.TrackingRecordReceived != null)
            {
                ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
                
                if ((activityStateRecord != null) && (!activityStateRecord.Activity.TypeName.Contains("System.Activities.Expressions")))
                {
                    if (this.ActivityIdToWorkflowElementMap.ContainsKey(activityStateRecord.Activity.Id))
                    {
                        this.TrackingRecordReceived(
                            this, 
                            new TrackingEventArgs(
                                record,
                                timeout,
                                this.ActivityIdToWorkflowElementMap[activityStateRecord.Activity.Id]));
                    }
                }
                else
                {
                    this.TrackingRecordReceived(this, new TrackingEventArgs(record, timeout, null));
                }
            }
        }
    }
}
