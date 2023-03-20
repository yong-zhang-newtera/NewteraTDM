/*
* @(#)Loader.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Serialization;
using System.Text;

using Newtera.WFModel;

namespace Newtera.WorkflowMonitor
{
    /// <summary>
    /// This type is used to load the workflow definition from database
    /// </summary>
    /// <version> 1.0.0 03 Jan 2006</version>
    internal sealed class Loader : WorkflowDesignerLoader
    {
        private WorkflowModel _workflowModel = null;
        private StringBuilder tempRulesStream = null;

        internal Loader()
        {
        }

        /// <summary>
        /// Gets or sets the WorkflowModel instance to be loaded
        /// </summary>
        internal WorkflowModel WorkflowModel
        {
            get
            {
                return _workflowModel;
            }

            set
            {
                _workflowModel = value;
            }
        }

        public override TextReader GetFileReader(string filePath)
        {
            if (this.tempRulesStream != null)
                return new StringReader(this.tempRulesStream.ToString());
            else
                return null;
        }

        public override TextWriter GetFileWriter(string filePath)
        {
            this.tempRulesStream = new StringBuilder();
            return new StringWriter(this.tempRulesStream);
        }

        public override string FileName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Load the workflow definition from WorkflowMarkup
        /// </summary>
        /// <param name="serializationManager"></param>
        protected override void PerformLoad(IDesignerSerializationManager serializationManager)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));

            if (_workflowModel != null)
            {
                // create the workflow's root activity
                using (XmlReader reader = _workflowModel.CreateXomlReader())
                {
                    WorkflowMarkupSerializer xomlSerializer = new WorkflowMarkupSerializer();

                    Activity rootActivity = xomlSerializer.Deserialize(reader) as Activity;

                    //Add the rootactivity the designer
                    if (rootActivity != null && designerHost != null)
                    {
                        Helpers.AddObjectGraphToDesignerHost(designerHost, rootActivity);
                        SetBaseComponentClassName(rootActivity.Name);
                    }
                }

                // Read from rules file if one exists
                if (_workflowModel.HasRules)
                {
                    TextReader rulesReader = _workflowModel.CreateRulesTextReader();
                    try
                    {
                        this.tempRulesStream = new StringBuilder(rulesReader.ReadToEnd());
                    }
                    finally
                    {
                        rulesReader.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Note: In case of state machine workflows we need to load the layout from the layout file in the 
        /// OnEndLoad method. This is because the layout file is applied to the designer components which are
        /// created in PerformLoad and are available only on the OnEndLoad method
        /// </summary>
        /// <param name="successful"></param>
        /// <param name="errors"></param>
        protected override void OnEndLoad(bool successful, ICollection errors)
        {
            base.OnEndLoad(successful, errors);

            // Load the layout if it exists
            if (_workflowModel.HasLayout)
            {
                IList loaderrors = null;
                using (XmlReader xmlReader = _workflowModel.CreateLayoutReader())
                {
                    LoadDesignerLayout(xmlReader, out loaderrors);
                }
            }
        }

        protected override void PerformFlush(IDesignerSerializationManager manager)
        {
        }
    }
}
