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
    //Class to store workflow instance information - create one per workflow instance in the database
    internal class WorkflowStatusInfo
    {
        private string idValue;
        private string statusValue;
        private string createdDateTimeValue;
        private Guid instanceIdValue;
        private ListViewItem listViewItemValue;

        internal WorkflowStatusInfo(string id, string status, string createdDateTime, Guid instanceId, ListViewItem listViewItem)
        {
            this.idValue = id;
            this.statusValue = status;
            this.createdDateTimeValue = createdDateTime;
            this.instanceIdValue = instanceId;
            this.listViewItemValue = listViewItem;
        }

        internal string Status
        {
            get { return statusValue; }
            set { statusValue = value; }
        }

        internal ListViewItem WorkflowListViewItem
        {
            get { return listViewItemValue; }
        }

        internal Guid InstanceId
        {
            get { return instanceIdValue; }
        }
    }

    //Class to store activity information - create one per activity for the selected workflow
    internal class ActivityStatusInfo
    {
        private string nameValue;
        private string statusValue;

        internal ActivityStatusInfo(string name, string status)
        {
            this.nameValue = name;
            this.statusValue = status;
        }

        internal string Name
        {
            get { return nameValue; }
        }

        internal string Status
        {
            get { return statusValue; }
        }
    }
}
