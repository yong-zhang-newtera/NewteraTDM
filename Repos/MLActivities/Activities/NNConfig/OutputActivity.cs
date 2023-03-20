using System.Activities;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Newtera.MLActivities.MLConfig;
using Newtera.MLActivities;

namespace Newtera.MLActivities.NeuralNetworkConfig
{

    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation")]
    public class OutputCommand : NativeActivity, IMLActivity
    {
        private Sequence innerSequence = new Sequence();

        /// <summary>
        /// Is the command enabled. If false, the command will be skiped in execution
        /// </summary>
        [TypeConverterAttribute(typeof(BooleanConverter))]
        public bool Enabled { get; set; }

        public OutputCommand() :base()
        {
            this.Enabled = true;
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

            Activity parentActivity = this.GetType().GetProperty("Parent",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this, null) as Activity;
            if (parentActivity != null && parentActivity.GetType() != typeof(System.Activities.Statements.Sequence))
            {
                metadata.AddValidationError("OutputCommand activity can only be child activity of CustomConfiguration activity");
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            if (this.ParentComponent != null)
            {
                MLCommand command = new MLCommand();
                command.Action = "Output";
                command.Enabled = this.Enabled;

                this.ParentComponent.Children.Add(command);

                command.Variables.Add("outputPath", "\"output.txt\"");

                foreach (Activity activity in innerSequence.Activities)
                {
                    if (activity is IMLActivity)
                    {
                        ((IMLActivity)activity).ParentComponent = command;
                    }
                    else if (activity is Parallel)
                    {
                        // handle branches of a parallel activity, such as a collection of SGD components
                        MLComponentCollection mlChidren = new MLComponentCollection();
                        command.Children.Add(mlChidren);

                        foreach (Activity branch in ((Parallel)activity).Branches)
                        {
                            if (branch is IMLActivity)
                            {
                                ((IMLActivity)branch).ParentComponent = mlChidren;
                            }
                        }
                    }
                }
            }

            context.ScheduleActivity(innerSequence);
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
