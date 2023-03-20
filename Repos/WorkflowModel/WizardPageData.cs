/*
* @(#)WizardPageData.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// This class is used to pass data into and out of a workflow and web wizard
    /// </summary>
    [Serializable]
    public class WizardPageData
    {
        #region Members
        private object data;
        private string controlName;

        #endregion

        #region Methods
        /// <summary>
        /// Data to be used by the page
        /// </summary>
        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Name of control that accepts the data
        /// </summary>
        public string ControlName
        {
            get { return controlName; }
            set { controlName = value; }
        }
        #endregion

    }
}
