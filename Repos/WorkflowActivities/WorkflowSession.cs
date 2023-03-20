/*
* @(#)WorkflowSession.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Collections.Generic;
using System.Text;

using Newtera.WFModel;

namespace Newtera.Activities
{
	[Serializable]
    public class WorkflowSession
	{
        private Dictionary<string,WizardPageData> pageDataCollection = new Dictionary<string,WizardPageData>();

        public Dictionary<string,WizardPageData> PageDataCollection
        {
            get { return pageDataCollection; }
            set { pageDataCollection = value; }
        }
	
	}
}
