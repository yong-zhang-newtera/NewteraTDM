

namespace Newtera.MLStudio.Execution
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;

    public class TrackingEventArgs : EventArgs
    {
        public TrackingEventArgs(TrackingRecord trackingRecord, TimeSpan timeout, Activity activity)
        {
            this.Record = trackingRecord;
            this.Timeout = timeout;
            this.Activity = activity;
        }

        public TrackingRecord Record { get; set; }

        public TimeSpan Timeout { get; set; }

        public Activity Activity { get; set; }
    }
}
