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
using System.Drawing;
using System.Activities.Presentation.PropertyEditing;

using Newtera.MLActivities.MLConfig;
using Newtera.MLActivities;
using Newtera.MLActivities.Controls;

namespace Newtera.MLActivities.DataProcess
{
    /// <summary>
    /// A PreProcessing
    /// </summary>
    public class PreProcessing : CodeActivity, IMLActivity
    {
        #region Arguments

        /// <summary>
        /// Data File Path
        /// </summary>
        [Editor(typeof(FileBrowserDialogPropertyValueEditor), typeof(DialogPropertyValueEditor))]
        public string DataFile { get; set; }

        /// <summary>
        /// Data preprocessing script File Path
        /// </summary>
        [Editor(typeof(ProgramBrowserDialogPropertyValueEditor), typeof(DialogPropertyValueEditor))]
        public string ProgramFile { get; set; }

        /// <summary>
        /// File for training data
        /// </summary>
        public string TrainFileName { get; set; }

        /// <summary>
        /// File for test data
        /// </summary>
        public string TestFileName { get; set; }

        /// <summary>
        /// File for eval data
        /// </summary>
        public string EvalFileName { get; set; }

        /// <summary>
        /// input data dimensions
        /// </summary>
        public int FeatureDimension { get; set; }

        /// <summary>
        /// label data  dimensions
        /// </summary>
        public int LabelDimension { get; set; }

        /// <summary>
        /// Is Data preprocessing enabled. If false, the preprocessing will be skiped in execution
        /// </summary>
        [TypeConverterAttribute(typeof(BooleanConverter))]
        public bool Enabled { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PreProcessing() : base()
        {
            DataFile = null;
            ProgramFile = null;
            TrainFileName = "Train_Data.txt";
            TestFileName = "Test_Data.txt";
            EvalFileName = "Eval_Data.txt";
            FeatureDimension = 0;
            LabelDimension = 0;
            Enabled = true;
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (string.IsNullOrEmpty(DataFile))
            {
                metadata.AddValidationError("DataFile property of a PreProcessing activity is required");
            }

            if (string.IsNullOrEmpty(ProgramFile))
            {
                metadata.AddValidationError("ProgramFile property of a PreProcessing activity is required");
            }

            if (this.FeatureDimension <= 0)
            {
                metadata.AddValidationError("FeatureDimension property of a PreProcessing activity is required");
            }

            if (this.LabelDimension <= 0)
            {
                metadata.AddValidationError("LabelDimension property of a PreProcessing activity is required");
            }
        }

        /// <summary>
        /// Execution Logic
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            MLExperimentManager experimentManager = context.GetExtension<MLExperimentManager>();

            if (experimentManager != null)
            {
                MLPreProcessing component = new MLPreProcessing();

                component.DataFile = this.DataFile;
                component.ProgramFile = this.ProgramFile;
                component.TrainFileName = this.TrainFileName;
                component.TestFileName = this.TestFileName;
                component.EvalFileName = this.EvalFileName;
                component.FeatureDimension = this.FeatureDimension;
                component.LabelDimension = this.LabelDimension;
                component.Enabled = this.Enabled;

                experimentManager.PreProcessing = component;
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
