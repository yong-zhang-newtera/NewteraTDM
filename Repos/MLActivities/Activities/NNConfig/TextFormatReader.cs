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

namespace Newtera.MLActivities.NeuralNetworkConfig
{
    /// <summary>
    /// A TextFormatReader
    /// </summary>
    public class TextFormatReader : CodeActivity, IMLActivity
    {
        #region Arguments

        /// <summary>
        /// Reader Type 
        /// </summary>
        [ReadOnly(true)]
        public string ReaderType { get; set; }

        /// <summary>
        /// Input Data File path
        /// </summary>
        [Editor(typeof(FileBrowserDialogPropertyValueEditor), typeof(DialogPropertyValueEditor))]
        public string DataFile { get; set; }

        /// <summary>
        /// Specifies whether the input should be randomized, default to true
        /// </summary>
        [TypeConverterAttribute(typeof(BooleanConverter))]
        public bool Randomize { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TextFormatReader() : base()
        {
            ReaderType = "CNTKTextFormatReader";
            DataFile = null;
            Randomize = true;
        }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        /// <summary>
        /// Execution Logic
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            if (this.ParentComponent != null)
            {
                MLExperimentManager experimentManager = context.GetExtension<MLExperimentManager>();

                MLReader component = new MLReader(experimentManager);

                component.Name = this.DisplayName;
                component.DataFile = this.DataFile;
                component.ReaderType = this.ReaderType;
                component.ExperimentId = experimentManager.Name;
                component.Randomize = this.Randomize;

                if (this.ParentComponent is MLCommand)
                {
                    MLCommand cmd = (MLCommand)this.ParentComponent;
                    switch (cmd.Action)
                    {
                        case "Test":
                            component.FileUsage = DataFileUsage.Test;
                            break;

                        case "Train":
                            component.FileUsage = DataFileUsage.Train;
                            break;

                        case "Output":
                            component.FileUsage = DataFileUsage.Output;
                            break;
                    }
                }

                this.ParentComponent.Children.Add(component);
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
