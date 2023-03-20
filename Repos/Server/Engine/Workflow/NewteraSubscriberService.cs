/*
* @(#)NewteraSubscriberService.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Workflow.Runtime;
using System.Threading;
using System.Security.Principal;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;

using Newtera.Common.Core;
using Newtera.WorkflowServices;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Server.UsrMgr;
using Newtera.Server.DB;
using Newtera.Server.Engine.Interpreter;
using Newtera.Server.Engine.Cache;
using Newtera.WFModel;
using Newtera.Common.Wrapper;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Events;
using Newtera.Common.MetaData.Subscribers;

namespace Newtera.Server.Engine.Workflow
{
    /// <summary>
    /// Provide the service for actions specified in event subscribers
    /// </summary>
    public class NewteraSubscriberService
    {
        private const string VariablePattern = @"\{[^\}]+\}";

        public NewteraSubscriberService()
        {
        }

        /// <summary>
        /// Perform the actions defined in the subscriber
        /// </summary>
        /// <param name="classElement"></param>
        /// <param name="subscriber"></param>
        /// <param name="eventDef"></param>
        /// <param name="instanceWrapper"></param>
        public void PerformActions(ClassElement classElement, Subscriber subscriber, EventDef eventDef, IInstanceWrapper instanceWrapper)
        {
            // perform send notifications or emails to users
            if (subscriber.SendEmail || subscriber.SendMessage)
            {
                StringCollection users = this.GetCombinedUsers(subscriber, instanceWrapper);
                StringCollection roles = this.GetCombinedRoles(subscriber, instanceWrapper);

                IUserManager userManager = new ServerSideUserManager();

                ITaskService taskService = new NewteraTaskService();

                // notify the assigned users now
                NotifyUsers(userManager, taskService, users, roles, subscriber, instanceWrapper);
            }

            // perform inline handler code
            if (!string.IsNullOrEmpty(subscriber.InlineHandler))
            {
                ActionCodeRunner.Instance.ExecuteActionCode("SubscriberInline" + classElement.ID + subscriber.Name, subscriber.InlineHandler, instanceWrapper);

                // Execution of the inline handler may cause changes to the values of the instance, save the changes of the instance to database.
                instanceWrapper.Save();
            }

            // perform external handler code
            if (!string.IsNullOrEmpty(subscriber.ExternalHanlder))
            {
                ICustomFunction function = CreateCustomFunction(subscriber.ExternalHanlder);
                if (function != null)
                {
                    function.Execute(instanceWrapper);

                    // Execution of the external handler may cause changes to the values of the instance, save the changes of the instance to database.
                    instanceWrapper.Save();
                }
            }
        }

        private void NotifyUsers(IUserManager userManager, ITaskService taskService, StringCollection users, StringCollection roles, Subscriber subscriber, IInstanceWrapper instanceWrapper)
        {
            StringCollection emailAddresses = new StringCollection();

            StringCollection qualifiedUsers = taskService.GetQualifiedUsers(users, roles, userManager);

            string subject = ReplaceVariables(subscriber.Subject, instanceWrapper);
            string body = ReplaceVariables(subscriber.Description, instanceWrapper);

            if (subscriber.SendEmail)
            {
                // send emails
                string[] usrEmails;
                foreach (string user in qualifiedUsers)
                {
                    usrEmails = userManager.GetUserEmails(user);
                    for (int i = 0; i < usrEmails.Length; i++)
                    {
                        if (usrEmails[i].Length > 0)
                        {
                            AddEmailAddress(emailAddresses, usrEmails[i]);
                        }
                    }
                }

                // send notice without replacing variables in subject and body
                taskService.SendNotice(subject, body, emailAddresses, null, false);
            }

            if (subscriber.SendMessage)
            {
                IMessageService messageService = Newtera.Server.Engine.Workflow.NewteraWorkflowRuntime.Instance.GetWorkflowRunTime().GetService<IMessageService>();

                string groupName = messageService.GetGroupName(instanceWrapper.SchemaId, instanceWrapper.OwnerClassName, instanceWrapper.ObjId);

                MessageInfo messageInfo = new MessageInfo();
                messageInfo.Subject = subject;
                messageInfo.Content = body;
                messageInfo.SenderName = GetSenderName(subscriber, instanceWrapper);
                messageInfo.SendTime = DateTime.Now.ToShortDateString();
                messageInfo.Url = subscriber.Url;
                messageInfo.UrlParams = subscriber.Params;
                messageInfo.SchemaName = GetSchemaName(instanceWrapper);
                messageInfo.ClassName = instanceWrapper.OwnerClassName;
                messageInfo.ObjId = instanceWrapper.ObjId;

                if (qualifiedUsers != null && qualifiedUsers.Count > 0)
                {
                    // send the message to the specified users
                    List<string> receivers = new List<string>();
                    foreach (string user in qualifiedUsers)
                    {
                        receivers.Add(user);
                    }

                    messageService.SendMessageToUsers(receivers, messageInfo);
                }
                else
                {
                    // send message the group
                    messageService.SendMessageToGroup(groupName, messageInfo);
                }
            }
        }

        // add email address without duplication
        private void AddEmailAddress(StringCollection emails, string email)
        {
            bool status = false;

            foreach (string addr in emails)
            {
                if (addr == email)
                {
                    status = true;
                    break;
                }
            }

            if (!status)
            {
                emails.Add(email);
            }
        }

        private string ReplaceVariables(string pattern, IInstanceWrapper instanceWrapper)
        {
            string text = pattern;

            if (!string.IsNullOrEmpty(text))
            {
                Regex patternExp = new Regex(NewteraSubscriberService.VariablePattern);

                MatchCollection matches = patternExp.Matches(pattern);
                if (matches.Count > 0)
                {
                    // contains variables
                    string propertyName;
                    object propertyValue;

                    foreach (Match match in matches)
                    {
                        if (match.Value.Length > 2)
                        {
                            // variable is in form of {propertyName}
                            propertyName = match.Value.Substring(1, (match.Value.Length - 2));
                            try
                            {
                                propertyValue = instanceWrapper.GetValue(propertyName);

                                if (propertyValue != null)
                                {
                                    // replace the variable with value
                                    text = text.Replace(match.Value, propertyValue.ToString());
                                }
                            }
                            catch (Exception)
                            {
                                // ignore
                            }
                        }
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Gets user names defined in subscriber
        /// </summary>
        /// <returns></returns>
        private StringCollection GetCombinedUsers(Subscriber subscriber, IInstanceWrapper instanceWrapper)
        {
            StringCollection users = new StringCollection();

            // get static assigned users
            if (subscriber.Users != null)
            {
                foreach (string user in subscriber.Users)
                {
                    users.Add(user);
                }
            }

            // get dynamic assigned users
            if (!string.IsNullOrEmpty(subscriber.UsersBindingAttribute))
            {
                // get the user displat name
                string userDisplayName = instanceWrapper.GetString(subscriber.UsersBindingAttribute);

                if (!string.IsNullOrEmpty(userDisplayName))
                {
                    UsersListHandler usersHandler = new UsersListHandler();

                    EnumValueCollection enumValues = usersHandler.GetValues(new ServerSideUserManager());

                    // convert fom user display name to the user id
                    foreach (EnumValue enumValue in enumValues)
                    {
                        if (enumValue.DisplayText == userDisplayName)
                        {
                            users.Add(enumValue.Value);
                            break;
                        }
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Gets role names defined in subscriber
        /// </summary>
        /// <returns></returns>
        private StringCollection GetCombinedRoles(Subscriber subscriber, IInstanceWrapper instanceWrapper)
        {
            StringCollection roles = new StringCollection();

            // get static assigned roles
            if (subscriber.Roles != null)
            {
                foreach (string role in subscriber.Roles)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }

        private string GetSenderName(Subscriber subscriber, IInstanceWrapper instanceWrapper)
        {
            string senderName = "admin"; // default sender

            // get dynamic assigned users
            if (!string.IsNullOrEmpty(subscriber.SenderBindingAttribute))
            {
                // get the user displat name
                string userDisplayName = instanceWrapper.GetString(subscriber.SenderBindingAttribute);

                if (!string.IsNullOrEmpty(userDisplayName))
                {
                    UsersListHandler usersHandler = new UsersListHandler();

                    EnumValueCollection enumValues = usersHandler.GetValues(new ServerSideUserManager());

                    // convert fom user display name to the user id
                    foreach (EnumValue enumValue in enumValues)
                    {
                        if (enumValue.DisplayText == userDisplayName)
                        {
                            senderName = enumValue.Value;
                            break;
                        }
                    }
                }
            }
            return senderName;
        }

        private string GetSchemaName(IInstanceWrapper instanceWrapper)
        {
            string[] schemaInfo = instanceWrapper.SchemaId.Split(' ');

            if (schemaInfo.Length > 1)
            {
                return schemaInfo[0];
            }
            else
            {
                return instanceWrapper.SchemaId;
            }
        }

        /// <summary>
        /// Create an instance of the ICustomFunction as specified.
        /// </summary>
        /// <param name="functionDefinition">The definition of the custom function</param>
        /// <returns>An instance of the ICustomFunction, null if failed to create the instance.</returns>
        private ICustomFunction CreateCustomFunction(string functionDefinition)
        {
            ICustomFunction function = null;

            if (!string.IsNullOrEmpty(functionDefinition))
            {
                int index = functionDefinition.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = functionDefinition.Substring(0, index).Trim();
                    assemblyName = functionDefinition.Substring(index + 1).Trim();
                }
                else
                {
                    className = functionDefinition.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    function = (ICustomFunction)obj.Unwrap();
                }
                catch
                {
                    function = null;
                }
            }

            return function;
        }
    }
}
