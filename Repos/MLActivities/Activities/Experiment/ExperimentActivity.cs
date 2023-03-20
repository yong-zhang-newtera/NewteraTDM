using System.Activities;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Activities.Presentation.PropertyEditing;

using Newtera.MLActivities.MLConfig;
using Newtera.MLActivities.Core;
using Newtera.MLActivities.Controls;
using Newtera.MLActivities.Activities.TypeConverters;
using Newtera.MLActivities;

namespace Newtera.MLActivities.Experiment
{

    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation")]
    public class Experiment : NativeActivity
    {
        private Sequence innerSequence = new Sequence();

        /// <summary>
        /// ID of the experiment, english letter and digits only, no space
        /// </summary>
        [Editor(typeof(ExperimentIdPropertyValueEditor), typeof(DialogPropertyValueEditor))]
        public string ExperimentId { get; set; }

        public Experiment() : base()
        {
            ExperimentId = null;
        }

        [Browsable(false)]
        public Collection<Activity> Activities
        {
            get
            {
                return innerSequence.Activities;
            }
        }

        [Browsable(false)]
        public Collection<Variable> Variables
        {
            get
            {
                return innerSequence.Variables;
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        { 
            metadata.AddImplementationChild(innerSequence);

            if (string.IsNullOrEmpty(this.ExperimentId))
            {
                metadata.AddValidationError("ExperimentId of a Experiment activity is required");
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            MLExperimentManager experimentManager = context.GetExtension<MLExperimentManager>();
            experimentManager.Name = this.ExperimentId;

            context.ScheduleActivity(innerSequence);
        }
    }
}
