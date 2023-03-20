/*
* @(#)ServerExecutionContext.cs
*
* Copyright (c) 2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Xml;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System.Threading;
    using System.Security.Principal;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Generator;
    using Newtera.Server.Engine.Interpreter.Parser;
    using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.Engine.Workflow;
    using Newtera.Server.DB;
    using Newtera.Server.UsrMgr;
    using Newtera.WFModel;
    using Newtera.Common.MetaData.Principal;

	/// <summary> 
	/// The class provides server side execution context for formula execution.
	/// </summary>
	/// <version> 1.0.0 11 Dec 2012</version>
    public class ServerExecutionContext : Newtera.Common.MetaData.Schema.Generator.ExecutionContext
	{
        private VirtualAttributeElement _attribute;
        private int _counter;
        private ServerSideUserManager _userManager;

        public ServerExecutionContext()
        {
            _attribute = null;
            _counter = 0;
            _userManager = new ServerSideUserManager();
        }

        /// <summary>
        /// gets or sets the virtual attribute for the execution
        /// </summary>
        public VirtualAttributeElement Attribute
        {
            get
            {
                return _attribute;
            }
            set
            {
                _attribute = value;
            }
        }

        /// <summary>
        /// Evaluate a expression
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>Evaluate result</returns>
        public object Evaluate(string expression)
        {
            Interpreter interpreter = new Interpreter();
            IExpr tree = interpreter.Parse(expression);

            XNode result = (XNode)tree.Eval();

            XmlDocument doc = (XmlDocument)result.Content;

            // The content of document root is the count result
            return doc.DocumentElement.InnerText;
        }

        /// <summary>
        /// Execute an XQuery
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>Query result in XmlDocument</returns>
        public XmlDocument ExecuteQuery(string query)
        {

            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                Thread.CurrentPrincipal = superUser;
                Interpreter interpreter = new Interpreter();

                // executing a nested query on behalf of a virtual attribute
                interpreter.IsNestedQuery = true;

                return interpreter.Query(query);
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        /// <summary>
        /// A global counter that is increased by one at each invocation
        /// </summary>
        public int GetAutoCounter()
        {
            _counter++;
            return _counter;
        }

        /// <summary>
        /// Get task owner display names of the workflow associated with an instance of the given id
        /// </summary>
        /// <param name="objId">An obj id of the instance</param>
        /// <returns>Task owner's display names, sepated by semicolon, or empty string if the instance doesn't have a binding workflow</returns>
        public string GetWorkflowTaskOwners(string objId)
        {
            string ownerNames = "";

            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByObjId(objId);

            if (bindingInfo != null)
            {
                NewteraTaskService taskService = new NewteraTaskService();
                List<TaskInfo> taskInfos = taskService.GetWorkflowInstanceTasks(bindingInfo.WorkflowInstanceId);
                string displayName;
                ServerSideUserManager userManager = new ServerSideUserManager();
                if (taskInfos.Count > 0)
                {
                    // there might be multiple tasks
                    foreach (TaskInfo taskInfo in taskInfos)
                    {
                        StringCollection taskOwners = taskService.GetTaskReceivers(taskInfo);
                        foreach (string taskOwner in taskOwners)
                        {
                            displayName = userManager.GetDisplayText(taskOwner);
                            if (string.IsNullOrEmpty(ownerNames))
                            {
                                ownerNames = displayName;
                            }
                            else
                            {
                                ownerNames += ";" + displayName;
                            }
                        }
                    }
                }
            }

            return ownerNames;
        }

        /// <summary>
        /// Get id of the current login user
        /// </summary>
        /// <returns>The id of the current login user</returns>
        public string GetCurrentUserID()
        {
            string loginId = null;

            CustomPrincipal principal = null;

            principal = (CustomPrincipal)Thread.CurrentPrincipal;
            loginId = principal.Identity.Name;

            return loginId;
        }

        /// <summary>
        /// Get display text of the current login user
        /// </summary>
        /// <returns>The displayed text of the current login user</returns>
        public string GetCurrentUserDisplayText()
        {
            string userName = null;

            CustomPrincipal principal = null;

            principal = (CustomPrincipal)Thread.CurrentPrincipal;
            if (principal != null)
            {
                userName = principal.DisplayText;
            }

            return userName;
        }

        /// <summary>
        /// Get display text of an user
        /// </summary>
        /// <returns>The displayed text of an user</returns>
        public string GetUserDisplayText(string user)
        {
            return _userManager.GetDisplayText(user);
        }

        /// <summary>
        /// Get the UserManager object which provides detailed info about the current user
        /// </summary>
        /// <returns>IUserManager object</returns>
        public IUserManager GetUserManager()
        {
            return _userManager;
        }
	}
}