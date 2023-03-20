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
    /// A PostProcessing
    /// </summary>
    public class PostProcessing : CodeActivity, IMLActivity
    {
        #region Arguments

        /// <summary>
        /// Data postprocessing script File Path
        /// </summary>
        [Editor(typeof(ProgramBrowserDialogPropertyValueEditor), typeof(DialogPropertyValueEditor))]
        public string ProgramFile { get; set; }

        /// <summary>
        /// File for output
        /// </summary>
        public string OutputFileName { get; set; }

        /// <summary>
        /// Is Data preprocessing enabled. If false, the preprocessing will be skiped in execution
        /// </summary>
        [TypeConverterAttribute(typeof(BooleanConverter))]
        public bool Enabled { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PostProcessing() : base()
        {
            ProgramFile = null;
            OutputFileName = "Output_Data.txt";
            Enabled = true;
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (string.IsNullOrEmpty(ProgramFile))
            {
                metadata.AddValidationError("ProgramFile property of a PostProcessing activity is required");
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
                MLPostProcessing component = new MLPostProcessing();

                component.ProgramFile = this.ProgramFile;
                component.OutputFileName = this.OutputFileName;
                component.Enabled = this.Enabled;

                experimentManager.PostProcessing = component;
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
