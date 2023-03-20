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
using System.Activities.Presentation.PropertyEditing;

using Newtera.MLActivities.MLConfig;
using Newtera.MLActivities.Activities.TypeConverters;
using Newtera.MLActivities.Controls;
using Newtera.MLActivities;

namespace Newtera.MLActivities.NeuralNetworkConfig
{
    /// <summary>
    /// A simple network builder activity
    /// </summary>
    public class SimpleNetworkBuilder : CodeActivity, IMLActivity
    {
        #region Arguments

        public string InputDimension { get; set; }

        public string OutputDimension { get; set; }

        /// <summary>
        /// Code that descibe the network model
        /// </summary>
        public string HiddenLayers { get; set; }

        /// <summary>
        /// Layer type, Sigmoid, Tanh, and RectifiedLinear
        /// </summary>
        [TypeConverterAttribute(typeof(LayerTypesConverter))]
        public string LayerTypes { get; set; }

        /// <summary>
        /// Criteria function used to training
        /// </summary>
        [TypeConverterAttribute(typeof(CriteriaFunctionConverter))]
        public string TrainingCriterion { get; set; }

        /// <summary>
        /// If specified to true, a drop-out node will be applied to the input node and the output of every hidden layer
        /// </summary>
        [TypeConverterAttribute(typeof(BooleanConverter))]
        public bool AddDropoutNodes { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SimpleNetworkBuilder() : base()
        {
            LayerTypes = "Sigmoid";
            TrainingCriterion = "CrossEntropyWithSoftmax";
            InputDimension = "$dimension$";
            OutputDimension = "$labelDimension$";
            HiddenLayers = "";
            AddDropoutNodes = false;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (string.IsNullOrEmpty(this.InputDimension))
            {
                metadata.AddValidationError("InputDimension property of a SimpleNetworkBuilder activity isn't specified");
            }

            if (string.IsNullOrEmpty(this.OutputDimension))
            {
                metadata.AddValidationError("OutputDimension property of a SimpleNetworkBuilder activity isn't specified");
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
                MLSimpleNetworkBuilder component = new MLSimpleNetworkBuilder();
                component.Name = this.DisplayName;
                component.InputDimension = this.InputDimension;
                component.OutputDimension = this.OutputDimension;
                component.HiddenLayers = this.HiddenLayers;
                component.LayerTypes = this.LayerTypes;
                component.TrainingCriterion = this.TrainingCriterion;
                component.AddDropoutNodes = this.AddDropoutNodes;

                this.ParentComponent.Children.Add(component);
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
