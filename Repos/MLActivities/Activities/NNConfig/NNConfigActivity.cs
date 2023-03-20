using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtera.MLActivities.Activities.TypeConverters;
using Newtera.MLActivities;
using Newtera.MLActivities.MLConfig;

namespace Newtera.MLActivities.NeuralNetworkConfig
{

    [Designer("System.Activities.Core.Presentation.SequenceDesigner, System.Activities.Core.Presentation")]
    public class Configuration : NativeActivity, IMLActivity
    {
        private Sequence innerSequence = new Sequence();
        private string deviceId;

        /// <summary>
        /// ID of the neural network config, english letter and digits only, no space
        /// </summary>
        public string ConfigId { get; set; }

        /// <summary>
        /// file path of the trained model
        /// </summary>
        [ReadOnly(true)]
        public string ModelPath{ get; set; }

        /// <summary>
        /// DeviceId, -1 means CPU; use 0 for your first GPU, 1 for the second etc.
        /// </summary>
        [TypeConverterAttribute(typeof(DeviceIdConverter))]
        public string DeviceId { get
            {
                return this.deviceId;
            }
            set
            {
                this.deviceId = value;
            }
        }

        /// <summary>
        /// sample data dimensions
        /// </summary>
        [Browsable(false)]
        public string FeatureDimension { get; set; }

        /// <summary>
        /// label data dimensions
        /// </summary>
        [Browsable(false)]
        public string LabelDimension { get; set; }

        public Configuration() : base()
        {
            ConfigId = "Config1";
            ModelPath = "model.dnn";
            DeviceId = "auto";
            FeatureDimension = null;
            LabelDimension = null;
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

            foreach (Activity a in Activities)
            {
                if ((a.GetType() != typeof(Newtera.MLActivities.NeuralNetworkConfig.TrainCommand)) && 
                    (a.GetType() != typeof(Newtera.MLActivities.NeuralNetworkConfig.TestCommand)) &&
                    (a.GetType() != typeof(Newtera.MLActivities.NeuralNetworkConfig.OutputCommand)))
                {
                    metadata.AddValidationError("Child activity of Configuration is not one of type Tain, Output or Test activity");
                }
            }

            if (string.IsNullOrEmpty(this.ConfigId))
            {
                metadata.AddValidationError("ConfigId property of a Configuration activity is required");
            }
            else if (!ValidateIDString(this.ConfigId))
            {
                metadata.AddValidationError("ConfigId has to start with an english letter and consists of only english letters, digits, hyphen, or undercore");
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            MLExperimentManager experimentManager = context.GetExtension<MLExperimentManager>();

            MLConfiguration configuration = new MLConfiguration(experimentManager);

            configuration.Name = this.ConfigId;

            configuration.Variables.Add("modelPath", "\"" + this.ModelPath + "\"");
            configuration.Variables.Add("deviceId", GetDeviceId());
            configuration.Variables.Add("precision", "float");
            configuration.Variables.Add("traceLevel", "0");

            // add to the manager
            experimentManager.Configurations.Add(configuration);

            foreach (Activity activity in innerSequence.Activities)
            {
                if (activity is IMLActivity)
                {
                    ((IMLActivity)activity).ParentComponent = configuration;
                }
            }

            context.ScheduleActivity(innerSequence);
        }

        private bool ValidateIDString(string id)
        {
            Regex regex = new Regex(@"^[a-zA-Z]+[0-9]*[a-zA-Z_\-]*[0-9]*$");

            bool status = regex.IsMatch(id);

            return status;
        }

        private string GetDeviceId()
        {
            if (this.deviceId == "CPU")
            {
                return "-1";
            }
            else if (this.deviceId == "GPU")
            {
                // GPU
                return "0";
            }
            else
            {
                // Automatic
                return "\"auto\"";
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
