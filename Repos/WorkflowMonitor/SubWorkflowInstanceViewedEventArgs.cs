/*
* @(#)StatusDefinition.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Windows.Forms;

namespace Newtera.WorkflowMonitor
{
    //Class to store the parameters of viewing a sub workflow instance
    public class SubWorkflowInstanceViewedEventArgs : EventArgs
    {
        private string _workflowInstanceId;
        private string _workflowModelName;

        public SubWorkflowInstanceViewedEventArgs(string workflowInstanceId, string workflowModelName) : base()
        {
            _workflowInstanceId = workflowInstanceId;
            _workflowModelName = workflowModelName;
        }

        public string WorkflowInstanceId
        {
            get { return _workflowInstanceId; }
            set { _workflowInstanceId = value; }
        }

        public string WorkflowModelName
        {
            get { return _workflowModelName; }
            set { _workflowModelName = value; }
        }
    }
}
