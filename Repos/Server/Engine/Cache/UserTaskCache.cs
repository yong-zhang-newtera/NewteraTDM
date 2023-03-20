/*
* @(#) UserTaskCache.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Cache
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Web;
    using System.Text;
    using System.Data;
    using System.Threading;
	using System.Web.Caching;
    using System.Collections.Specialized;
    using System.Collections.Generic;

	using Newtera.Common.Core;
    using Newtera.Server.Util;
    using Newtera.WFModel;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// This is the single cache for user tasks to speed up performance
	/// </summary>
	/// <version> 1.0.0	27 Jul 2013 </version>
	public class UserTaskCache
	{	
        private IKeyValueStore _schemaTasks;
        private IKeyValueStore _userTasks;
        private Hashtable _userLocks;

		// Static cache object, all invokers will use this cache object.
		private static UserTaskCache theCache;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private UserTaskCache()
		{
            _schemaTasks = KeyValueStoreFactory.TheInstance.Create("UserTaskCache.SchemaTasks");
            _userTasks = KeyValueStoreFactory.TheInstance.Create("UserTaskCache.UserTasks");
            _userLocks = new Hashtable();
		}

		/// <summary>
		/// Gets the UserTaskCache instance.
		/// </summary>
		/// <returns> The UserTaskCache instance.</returns>
		static public UserTaskCache Instance
		{
			get
			{
				return theCache;
			}
		}

        /// <summary>
        /// Set all tasks of a schema in the cache
        /// </summary>
        /// <param name="schemaId"></param>
        /// <param name="tasks"></param>
        public void SetSchemaTasks(string schemaId, List<TaskInfo> tasks)
        {
            if (this._schemaTasks.Contains(schemaId))
            {
                this._schemaTasks.Remove(schemaId);
            }

            _schemaTasks.Add(schemaId, tasks);
        }


        /// <summary>
        /// Gets tasks of a schema from the caceh
        /// </summary>
        /// <param name="schema"></param>
        /// <returns>The schema's tasks</returns>
        public List<TaskInfo> GetSchemaTasks(string schemaId)
        {
            if (_schemaTasks.Contains(schemaId))
            {
                return _schemaTasks.Get<List<TaskInfo>>(schemaId);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Set the tasks of an user of a given schema to the cache
        /// </summary>
        /// <param name="user"></param>
        /// <param name="schemaId">the schema of the tasks</param>
        /// <param name="tasks"></param>
        public void SetUserTasks(string user, string schemaId, List<TaskInfo> tasks)
        {
            lock (GetUserLock(user))
            {
                string key = schemaId + "-" + user;

                if (this._userTasks.Contains(key))
                {
                    this._userTasks.Remove(key);
                }

                _userTasks.Add(key,tasks);
            }
        }

        /// <summary>
        /// Clear the tasks of an user of a given schema to the cache
        /// </summary>
        /// <param name="user"></param>
        /// <param name="schemaId">the schema of the tasks</param>
        public void ClearUserTasks(string user, string schemaId)
        {
            lock (GetUserLock(user))
            {
                if (!string.IsNullOrEmpty(schemaId))
                {
                    string key = schemaId + "-" + user;

                    if (this._userTasks.Contains(key))
                    {
                        this._userTasks.Remove(key);
                    }
                }
                else
                {
                    StringCollection removedKeys = new StringCollection();

                    // clear user's task list for all schema
                    foreach (string key in _userTasks.Keys)
                    {
                        if (key.EndsWith(user))
                        {
                            removedKeys.Add(key);
                        }
                    }

                    foreach (string key in removedKeys)
                    {
                        this._userTasks.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Add a task to an user's task list
        /// </summary>
        /// <param name="user">The user name</param>
        /// <param name="schemaId">the schema of the tasks</param>
        /// <param name="taskInfo">The task info to be added</param>
        public void AddUserTask(string user, string schemaId, TaskInfo taskInfo)
        {
            lock (GetUserLock(user))
            {
                string key = schemaId + "-" + user;

                // add the user's task list if it has been initialized, otherwise, wait until GetUserTasks is called
                if (_userTasks.Contains(key))
                {
                    List<TaskInfo> cachedTaskInfos = _userTasks.Get<List<TaskInfo>>(key);

                    bool found = false;
                    foreach (TaskInfo cachedTaskInfo in cachedTaskInfos)
                    {
                        if (cachedTaskInfo.TaskId == taskInfo.TaskId)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        cachedTaskInfos.Insert(0, taskInfo.Clone());
                    }
                }
            }
        }

        /// <summary>
        /// Remove a task from an user's task list
        /// </summary>
        /// <param name="user">The user anme</param>
        /// <param name="schemaId">the schema of the tasks</param>
        /// <param name="taskInfo">The task info to be added</param>
        public void RemoveUserTask(string user, string schemaId, TaskInfo taskInfo)
        {
            lock (GetUserLock(user))
            {
                string key = schemaId + "-" + user;

                if (_userTasks.Contains(key))
                {
                    List<TaskInfo> cachedTaskInfos = _userTasks.Get<List<TaskInfo>>(key);

                    TaskInfo foundTaskInfo = null;
                    foreach (TaskInfo cachedTaskInfo in cachedTaskInfos)
                    {
                        if (cachedTaskInfo.TaskId == taskInfo.TaskId)
                        {
                            foundTaskInfo = cachedTaskInfo;
                            break;
                        }
                    }

                    if (foundTaskInfo != null)
                    {
                        cachedTaskInfos.Remove(foundTaskInfo);
                    }
                }
            }
        }


        /// <summary>
        /// Gets tasks of a user of a given schema from the caceh
        /// </summary>
        /// <param name="user"></param>
        /// <param name="schemaId">the schema of the tasks</param>
        /// <returns>The user's tasks</returns>
        public List<TaskInfo> GetUserTasks(string user, string schemaId)
        {
            lock (GetUserLock(user))
            {
                string key = schemaId + "-" + user;

                if (_userTasks.Contains(key))
                {
                    // return a copy of the cached user tasks to avoid memory leak
                    List<TaskInfo> cachedTaskInfos = _userTasks.Get<List<TaskInfo>>(key);
                    List<TaskInfo> taskInfos = new List<TaskInfo>();
                    foreach (TaskInfo cachedTaskInfo in cachedTaskInfos)
                    {
                        taskInfos.Add(cachedTaskInfo.Clone());
                    }
                    return taskInfos;
                }
                else
                {
                    return null;
                }
            }
        }

        public void SetUserIdToTask(string schemaId, Guid workflowInstanceId, string userId)
        {
            List<TaskInfo> allTasks = GetSchemaTasks(schemaId);
            string wfInstanceId = workflowInstanceId.ToString();

            if (allTasks != null)
            {
                foreach (TaskInfo taskInfo in allTasks)
                {
                    if (taskInfo.WorkflowInstanceId == wfInstanceId)
                    {
                        taskInfo.UserId = userId;

                        // assign the user to all tasks of a workflow instance, because we don't know the task id at this time
                        //break;
                    }
                }
            }
        }

        /// <summary>
        /// Clear the cache
        /// </summary>
        public void ClearTaskCache()
        {
            this._schemaTasks.Clear();
            this._userTasks.Clear();
        }

        private UserLock GetUserLock(string userName)
        {
            UserLock userLock = (UserLock)_userLocks[userName];
            if (userLock == null)
            {
                userLock = new UserLock();
                _userLocks.Add(userName, userLock);
            }

            return userLock;
        }

		static UserTaskCache()
		{
			// Initializing the cache.
			{
				theCache = new UserTaskCache();
			}
		}
	}

    internal class UserLock
    {
        public UserLock()
        {
        }
    }
}