using System;
using System.Collections.Generic;
using System.Text;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.ServiceModel.Activities;
using System.Activities.Presentation.Validation;
using Microsoft.CSharp.Activities;
using System.Activities.XamlIntegration;
using System.Activities.Tracking;
using System.Net;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Collections;

using Newtera.MLActivities.MLConfig;
using Newtera.MLActivities;

namespace Newtera.MLActivities.NeuralNetworkConfig
{
    /// <summary>
    /// A network using logistic regression
    /// </summary>
    public class SGD : CodeActivity, IMLActivity
    {
        #region Arguments

        /// <summary>
        /// Epoch Size of SGD
        /// </summary>
        public string EpochSize { get; set; }

        /// <summary>
        /// Minibatch Size of SGD
        /// </summary>
        public string MinibatchSize { get; set; }

        /// <summary>
        /// Learning Rates Per Sample  of SGD
        /// </summary>
        public string LearningRatesPerSample{ get; set; }

        /// <summary>
        /// Max Epochs of SGD
        /// </summary>
        public string MaxEpochs { get; set; }

        public string MomentumAsTimeConstant { get; set; }

        public string NumMBsToShowResult { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SGD() : base()
        {
            EpochSize = "0";
            MinibatchSize = "25";
            LearningRatesPerSample = "0.04";
            MaxEpochs = "50";
            MomentumAsTimeConstant = null;
            NumMBsToShowResult = null;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            Activity parentActivity = this.GetType().GetProperty("Parent",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this, null) as Activity;
            if (parentActivity != null && parentActivity.GetType() == typeof(System.Activities.Statements.Parallel))
            {
                Hashtable sgdNames = new Hashtable();
                Collection<Activity> branches = ((Parallel)parentActivity).Branches;
                foreach (Activity branch in branches)
                {
                    if (branch is SGD)
                    {
                        if (sgdNames[branch.DisplayName] == null)
                        {
                            sgdNames[branch.DisplayName] = "1";
                        }
                        else
                        {
                            metadata.AddValidationError("The display name of a SGD activity isn't unique");
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(this.EpochSize))
            {
                metadata.AddValidationError("EpochSize property of a SGD activity is required");
            }

            if (string.IsNullOrEmpty(this.MinibatchSize))
            {
                metadata.AddValidationError("MinibatchSize property of a SGD activity is required");
            }

            if (string.IsNullOrEmpty(this.LearningRatesPerSample))
            {
                metadata.AddValidationError("LearningRatesPerSample property of a SGD activity is required");
            }

            if (string.IsNullOrEmpty(this.MaxEpochs))
            {
                metadata.AddValidationError("MaxEpochs property of a SGD activity is required");
            }
        }

        /// <summary>
        /// Execution Logic
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            if (this.ParentComponent != null)
            {
                MLSGD component = new MLSGD();
                component.Name = this.DisplayName;
                component.EpochSize = this.EpochSize;
                component.MinibatchSize = this.MinibatchSize;
                component.LearningRatesPerSample = this.LearningRatesPerSample;
                component.MaxEpochs = this.MaxEpochs;
                component.MomentumAsTimeConstant = this.MomentumAsTimeConstant;
                component.NumMBsToShowResult = this.NumMBsToShowResult;

                this.ParentComponent.Children.Add(component);
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
