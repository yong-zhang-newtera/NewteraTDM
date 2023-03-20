/*
* @(#)IMessageService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Workflow.Runtime;

using Newtera.WFModel;

namespace Newtera.WorkflowServices
{
    /// <summary>
    /// Define the interface for sending message
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Get an unique group name
        /// </summary>
        /// <param name="schemaId">schema name plus version number</param>
        /// <param name="className">class name</param>
        /// <param name="objId">instance id</param>
        string GetGroupName(string schemaId, string className, string objId);

        /// <summary>
        /// Send a message to a hub group
        /// </summary>
        /// <param name="groupName">The group name</param>
        /// <param name="message">The message to be sent</param>
        void SendMessageToGroup(string groupName, MessageInfo message);

        /// <summary>
        /// Send a message to an user
        /// </summary>
        /// <param name="userNames">The user names</param>
        /// <param name="message">The message to be sent</param>
        void SendMessageToUsers(List<string> userNames, MessageInfo message);

        /// <summary>
        /// Send a task notice to an user
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="message">The message to be sent</param>
        void SendTaskNoticeToUser(string userName, MessageInfo message);
    }
}
