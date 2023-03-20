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
    /// A network builder activity
    /// </summary>
    public class CustomNetworkBuilder : CodeActivity, IMLActivity
    {
        #region Arguments

        /// <summary>
        /// Code that descibe the network model
        /// </summary>
        [Editor(typeof(CodeDialogPropertyValueEditor), typeof(DialogPropertyValueEditor))]
        public string NetworkBuilderCode { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomNetworkBuilder() : base()
        {
            NetworkBuilderCode = "";
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (string.IsNullOrEmpty(this.NetworkBuilderCode))
            {
                metadata.AddValidationError("NetworkBuilderCode property of a CustomNetworkBuilder activity isn't specified");
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
                MLNetworkBuilder component = new MLNetworkBuilder();
                component.Name = this.DisplayName;
                component.NetworkBuilderCode = this.NetworkBuilderCode;

                this.ParentComponent.Children.Add(component);
            }
        }

        #region IMLActivity

        [Browsable(false)]
        public IMLComponnet ParentComponent { get; set; }

        #endregion
    }
}
