/*
* @(#) LoggingManager.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Logging
{
	using System;
	using System.IO;
	using System.Collections;
    using System.Collections.Generic;
	using System.Threading;
    using System.Xml;
    using System.Data;
    using System.Security.Principal;
    using System.Text.RegularExpressions;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Logging;
    using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.UsrMgr;

	/// <summary>
	/// This is the single object that manages a queue for logging messages and the queue
    /// uses a single worker thread to write the logging messages to the database.
	/// </summary>
	/// <version> 1.0.0 07 Jan 2009 </version>
	public class LoggingManager
	{
        private const string AddDataOperationLogQuery = "let $entry := [[<DataOperations xmlns:xsi=\"http://www.w3.org/2003/XMLSchema-instance\"  xsi:type=\"DataOperations\"><oType>param10</oType><oTime>param11</oTime><uName>param12</uName><uRoles>param13</uRoles><ipAddress>param14</ipAddress><dbId>param15</dbId><cName>param16</cName><oData>param17</oData><cCaption>param18</cCaption><uCaption>param19</uCaption><OID>param20</OID></DataOperations>]] return addInstance(document(\"db://logginginfo.xml\"), $entry)";
        private const string GetPropertyUpdateLogsQuery = "for $log in document(\"db://logginginfo.xml\")/DataOperationsList/DataOperations where ($log/oType=\"param10\" or  $log/oType=\"param11\") and $log/dbId=\"param12\" and $log/cName=\"param13\" and $log/OID=\"param14\" and contains($log/oData, \"param15\") return <DataOperations {$log/@obj_id}> {$log/oType} {$log/oTime} {$log/uName} {$log/uCaption} {$log/uRoles} {$log/ipAddress} {$log/dbId} {$log/cName} {$log/cCaption} {$log/oData}</DataOperations> sortby ($log/@obj_id descending)";

		// Static cache object, all invokers will use this cache object.
		private static LoggingManager theManager;

        private ProcessingQueue<LoggingMessage> _processQueue; // logging message queue
        private IPrincipal _superUser = null;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private LoggingManager()
		{
            _processQueue = new ProcessingQueue<LoggingMessage>();
            _processQueue.DoWork += new ProcessingQueue<LoggingMessage>.DoWorkDelegate(OnDoWork);
            _processQueue.WorkException +=
                 new ProcessingQueue<LoggingMessage>.WorkExceptionDelegate(OnWorkException);

            CMUserManager userMgr = new CMUserManager();
            _superUser = userMgr.SuperUser;
		}

		/// <summary>
		/// Gets the LoggingManager instance.
		/// </summary>
		/// <returns> The LoggingManager instance.</returns>
		static public LoggingManager Instance
		{
			get
			{
				return theManager;
			}
		}

		/// <summary>
		/// Add a looging message to the queue
		/// </summary>
		/// <param name="loggingMessage">The logging message</param>
        public void AddLoggingMessage(LoggingMessage loggingMessage)
		{
            lock (this)
            {
                _processQueue.QueueForWork(loggingMessage);
            }
		}

        /// <summary>
        /// Get count of the logging messages that are related a column of an instance
        /// </summary>
        public int GetMatchedLoggingMessagesCount(string schemaId, string className, string propertyName, string objId)
        {
            int count = 0;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetPropertyUpdateLogsQuery;

                // parameter number starts from 10 to prevent error during the replacement
                query = query.Replace("param10", Enum.GetName(typeof(LoggingActionType), LoggingActionType.Create));
                query = query.Replace("param11", Enum.GetName(typeof(LoggingActionType), LoggingActionType.Write));
                query = query.Replace("param12", schemaId);
                query = query.Replace("param13", className);
                query = query.Replace("param14", objId);
                query = query.Replace("param15", propertyName);

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Query(query);

                DataSet ds = new DataSet();
                XmlReader xmlReader = new XmlNodeReader(doc);
                ds.ReadXml(xmlReader);
                DataTable logTable = ds.Tables["DataOperations"];

                if (logTable != null)
                {
                    count = logTable.Rows.Count;
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }

            return count;
        }

        /// <summary>
        /// Get matched logging messages
        /// </summary>
        public List<LoggingMessage> GetMatchedLoggingMessages(string schemaId, string className, string propertyName, string objId)
        {
            List<LoggingMessage> loggingMessages = new List<LoggingMessage>();

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                // execute the method as a super user
                Thread.CurrentPrincipal = _superUser;

                string query = GetPropertyUpdateLogsQuery;

                // parameter number starts from 10 to prevent error during the replacement
                query = query.Replace("param10", Enum.GetName(typeof(LoggingActionType), LoggingActionType.Create));
                query = query.Replace("param11", Enum.GetName(typeof(LoggingActionType), LoggingActionType.Write));
                query = query.Replace("param12", schemaId);
                query = query.Replace("param13", className);
                query = query.Replace("param14", objId);
                query = query.Replace("param15", propertyName);

                Interpreter interpreter = new Interpreter();
                XmlDocument doc = interpreter.Query(query);

                DataSet ds = new DataSet();
                XmlReader xmlReader = new XmlNodeReader(doc);
                ds.ReadXml(xmlReader);
                DataTable logTable = ds.Tables["DataOperations"];

                LoggingMessage logInfo;
                if (logTable != null && logTable.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in logTable.Rows)
                    {
                        logInfo = new LoggingMessage();
                        logInfo.ActionType = (LoggingActionType)Enum.Parse(typeof(LoggingActionType), dataRow["oType"].ToString());
                        logInfo.SchemaID = schemaId;
                        logInfo.ClassName = className;
                        logInfo.ClassCaption = dataRow["cCaption"].ToString();
                        logInfo.UserName = dataRow["uName"].ToString();
                        logInfo.UserDisplayText = dataRow["uCaption"].ToString();
                        if (!dataRow.IsNull("uRoles"))
                        {
                            logInfo.UserRoles = dataRow["uRoles"].ToString().Split(';');
                        }
                        if (!dataRow.IsNull("oTime"))
                        {
                            try
                            {
                                logInfo.ActionTime = DateTime.Parse(dataRow["oTime"].ToString());
                            }
                            catch (Exception)
                            {
                            }
                        }
                        logInfo.ActionData = GetColumnValueFromActionData(dataRow["oData"].ToString(), propertyName);

                        loggingMessages.Add(logInfo);
                    }
                }

            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }

            return loggingMessages;
        }

        private string GetColumnValueFromActionData(string actionDate, string propertyName)
        {
            string propertyVal = "";

            // The action data string is in form of "property1=value1;property2=value2"
            // Compile regular expression to find "name = value" pairs
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            MatchCollection matches = regex.Matches(actionDate);
            foreach (Match match in matches)
            {
                int pos = match.Value.IndexOf("=");
                string key = match.Value.Substring(0, pos).TrimEnd();
                string val = match.Value.Substring(pos + 1).TrimStart();
                if (key.Trim() == propertyName)
                {
                    propertyVal = val;
                    break;
                }
            }


            return propertyVal;
        }

        private void OnDoWork(object sender, ProcessingQueueEventArgs<LoggingMessage> args)
        {
            LoggingMessage msg = args.Work;

            if (msg != null)
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the method as a super user
                    Thread.CurrentPrincipal = _superUser;

                    string query = AddDataOperationLogQuery;

                    // parameter number starts from 10 to prevent error during the replacement
                    query = query.Replace("param10", Enum.GetName(typeof(LoggingActionType), msg.ActionType));
                    query = query.Replace("param11", msg.ActionTime.ToString());
                    query = query.Replace("param12", msg.UserName);
                    query = query.Replace("param13", GetRoleString(msg.UserRoles));
                    query = query.Replace("param14", (msg.IPAddress != null ? msg.IPAddress : ""));
                    query = query.Replace("param15", msg.SchemaID);
                    query = query.Replace("param16", msg.ClassName);
                    query = query.Replace("param17", (msg.ActionData != null ? msg.ActionData : ""));
                    query = query.Replace("param18", (msg.ClassCaption != null ? msg.ClassCaption : ""));
                    query = query.Replace("param19", (msg.UserDisplayText != null ? msg.UserDisplayText : ""));
                    query = query.Replace("param20", (msg.OID != null ? msg.OID : ""));

                    Interpreter interpreter = new Interpreter();
                    interpreter.Query(query);
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }

        private void OnWorkException(object sender, ProcessingQueueExceptionEventArgs args)
        {
        }

        private string GetRoleString(string[] roles)
        {
            string roleString = "";

            if (roles != null && roles.Length > 0)
            {
                int index = 0;
                foreach (string role in roles)
                {
                    if (index == 0)
                    {
                        roleString = role;
                    }
                    else
                    {
                        roleString += ";" + role;
                    }

                    index++;
                }
            }

            return roleString;
        }

		static LoggingManager()
		{
			// Initializing the manager.
			{
				theManager = new LoggingManager();
			}
		}
	}
}