/*
* @(#)ITaskService.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.IO;
using System.Data;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Workflow.Runtime;
using System.Security.Principal;

using Newtera.Common.MetaData.Principal;
using Newtera.WFModel;

namespace Newtera.WorkflowServices
{
    /// <summary>
    /// Define interface for workflow task management service used by workflow runtime
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Get the super user principle
        /// </summary>
        IPrincipal SuperUser { get; }

        /// <summary>
        /// Create a task for each of specified users
        /// </summary>
        /// <param name="subject">The task subject</param>
        /// <param name="description">The task description</param>
        /// <param name="instruction">The task instruction</param>
        /// <param name="formProperties">The properties to be displayed in the auto-generated form</param>
        /// <param name="url">The custom form url, could be null</param>
        /// <param name="users">A list of users who are assigned to the task</param>
        /// <param name="roles">A roles that a users must belong to in order to receive the assignment</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="bindingSchemaId">The schema id of the binding data instance, can be empty.</param>
        /// <param name="bindingClassName">The class name of the binding data instance, can be empty.</param>
        /// <param name="customActionsXml">The xml represents a collection of CustomAction objects</param>
        /// <param name="isVisible">Is the task visible in my task list?</param>
        /// <returns>The task id</returns>
        string CreateTask(string subject, string description, string instruction, string url,
            StringCollection formProperties, StringCollection users, StringCollection roles, string activityName,
            string bindingSchemaId, string bindingClassName, string customActionsXml, bool isVisible);

        /// <summary>
        /// Close the task that has been assigned to the users and remove it from database.
        /// </summary>
        /// <param name="activityName">The name of the CreateTaskActivity which created the task</param>
        /// <returns>Task Id</returns>
        string CloseTask(string activityName);

        /// <summary>
        /// Close the task of a given id that has been assigned to the users and remove it from database.
        /// </summary>
        /// <param name="taskId">The id of the workflow task</param>
        void CloseTaskById(string taskId);

        /// <summary>
        /// Delete tasks from database that are associated with a workflow instance.
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        void DeleteTasks(string workflowInstanceId);

        /// <summary>
        /// An utility method that returns a list of distintc qualified users specified by the users
        /// and roles.
        /// </summary>
        /// <param name="users">The explicit user collection</param>
        /// <param name="roles">The roles that a qualified users must belong to</param>
        /// <param name="userManager">IUserManager object</param>
        /// <returns>A list of distinct users</returns>
        StringCollection GetQualifiedUsers(StringCollection users, StringCollection roles, IUserManager userManager);

        /// <summary>
        /// Get a collection of user names who are actual owners of the given task
        /// </summary>
        /// <param name="taskId">The id of the task</param>
        /// <param name="users">The user collection sepcified for the task</param>
        /// <param name="roles">The role collection specified  for the task</param>
        /// <param name="workflowInstanceId">the workflow instance id that the task is associated</param>
        /// <returns>A list of users who receive the task</returns>
        StringCollection GetTaskReceivers(string taskId, StringCollection users, StringCollection roles, string workflowInstanceId);

        /// <summary>
        /// Gets the user's tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns></returns>
        List<TaskInfo> GetUserTasks(string schemaId, string userName, IUserManager userManager);

        /// <summary>
        /// Gets a workflow instance's tasks
        /// </summary>
        /// <param name="workflowInstanceId">The workflow instance id</param>
        /// <returns>A list of task infos.</returns>
        List<TaskInfo> GetWorkflowInstanceTasks(string workflowInstanceId);

        /// <summary>
        /// Gets a task by id
        /// </summary>
        /// <param name="taskId">task id</param>
        /// <returns>A TaskInfo object</returns>
        TaskInfo GetTask(string taskId);

        /// <summary>
        /// Gets a finished task by id belong to an user
        /// </summary>
        /// <param name="taskId">task id</param>
        /// <param name="userName">The user name</param>
        /// <returns>A TaskInfo object</returns>
        TaskInfo GetFinishedTask(string taskId, string userName);

        /// <summary>
        /// Gets values of an attribute from a binding data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the binding data instance</param>
        /// <returns>A collection of value strings</returns>
        StringCollection GetBindingValues(string schemaId, string className, string attributeName);

        /// <summary>
        /// Gets user ids from an attribute's value representing user's display names from a binding data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the binding data instance</param>
        /// <returns>A collection of user ids strings</returns>
        StringCollection GetBindingUsers(string schemaId, string className, string attributeName);

        /// <summary>
        /// Gets role's names from an attribute's value representing role's display names from a binding data instance indicated by the parameters
        /// </summary>
        /// <param name="schemaId">the schema of the data instance</param>
        /// <param name="className">The class of the data instance</param>
        /// <param name="attributeName">The attibute name of the binding data instance</param>
        /// <returns>A collection of role's names</returns>
        StringCollection GetBindingRoles(string schemaId, string className, string attributeName);

        /// <summary>
        /// Send a notification email to the specified email address
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="description">Email body</param>
        /// <param name="emailAddresses">A list of email addresses</param>
        /// <param name="users">the users to be noticed</param>
        /// <remarks>subject and body can contain variables</remarks>
        void SendNotice(string subject, string body, StringCollection emailAddresses, StringCollection users);

        /// <summary>
        /// Send a notification (email and message) to the specified users
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="description">Email body</param>
        /// <param name="emailAddresses">A list of email addresses</param>
        /// <param name="users">the users to be noticed</param>
        /// <param name="hasVariables">Information indicating whether the subject and body has variables that need to be replaced by values of the binding data instance</param>
        void SendNotice(string subject, string body, StringCollection emailAddresses, StringCollection users, bool hasVariables);

        /// <summary>
        /// Create a log entry for a workflow task
        /// </summary>
        /// <param name="instanceKey">The binding data instance key</param>
        /// <param name="instanceDescription">The binding data instance key description</param>
        /// <param name="expectedFinishTime">The expected finish time of the task</param>
        /// <param name="taskTakers">A list of task takers separted by semicolon</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="workflowInstanceIdStr">The id string of the workflow instance</param>
        void WriteTaskLog(string instanceKey, string instanceDescription, string expectedFinishTime,
            string taskTakers, string activityName, string workflowInstanceIdStr);

        /// <summary>
        /// Update the corresponding task log's finish time
        /// </summary>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        /// <param name="taskId">Task Id</param>
        void UpdateTaskLog(string activityName, string taskId);

        /// <summary>
        /// Get a collection of TaskLogInfo objects of given criteria
        /// </summary>
        /// <param name="instanceKey">The binding data instance key</param>
        /// <param name="activityName">The name of the CreateTaskActivity which creates the task</param>
        List<TaskLogInfo> GetTaskLogs(string instanceKey, string activityName);

        /// <summary>
        /// Save an user's finsished task into database
        /// </summary>
        /// <param name="taskInfo">The task info</param>
        /// <param name="userName">The user name</param>
        void AddUserFinishedTask(TaskInfo taskInfo, string userName);

        /// <summary>
        /// Gets the user's finished tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <param name="from">from index</param>
        /// <param name="size">page size</param>
        /// <returns></returns>
        List<TaskInfo> GetUserFinishedTasks(string schemaId, string userName, int from, int size, IUserManager userManager);

        /// <summary>
        /// Gets the count of user's finished tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns>count integer</returns>
        int GetUserFinishedTaskCount(string schemaId, string userName, IUserManager userManager);

        /// <summary>
        /// Gets the finished tasks for a worklfow instance
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="wfInstanceId">The worklfow instance id</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns></returns>
        List<TaskInfo> GetWorkflowFinishedTasks(string schemaId, string wfInstanceId, IUserManager userManager);

        /// <summary>
        /// Clear the user's finished tasks
        /// </summary>
        /// <param name="schemaId">The schema id</param>
        /// <param name="userName">The user name</param>
        /// <param name="userManager">The user mananger</param>
        /// <returns></returns>
        void ClearUserFinishedTasks(string schemaId, string userName, IUserManager userManager);
    }
}
