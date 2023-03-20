using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Data;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Text.RegularExpressions;

using Microsoft.AspNet.SignalR;


using Ebaas.WebApi.Infrastructure;
using Newtera.Common.Core;

namespace Ebaas.WebApi.Hubs
{
    /// <summary>
    /// Message Hub APIs using Microsoft SignalR
    /// </summary>
    public class MessageHub : Hub
    {
        private readonly MessageHubMaster _theHubInstance;

        public MessageHub()
        {
            // get the hub singleton
            _theHubInstance = MessageHubMaster.Instance;
        }

        /// <summary>
        /// Called when an user connects to the server
        /// </summary>
        /// <returns>Task</returns>
        [HubAuthorizeAttribute]
        public override async Task OnConnected()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    if (Context.Request == null)
                    {
                        throw new InvalidOperationException("The request is null");
                    }

                    string userName = Context.Request.QueryString["user"];
                    if (string.IsNullOrEmpty(userName))
                    {
                        throw new Exception("Missing username parameter in the query string");
                    }
                    string schemaName = Context.Request.QueryString["schema"];
                    if (string.IsNullOrEmpty(schemaName))
                    {
                        throw new Exception("Missing schema parameter in the query string");
                    }

                    if (_theHubInstance.ExceedConnectionLimit())
                    {
                        throw new Exception("Out of connections");
                    }
                    else
                    {
                        _theHubInstance.AddConnectedId(Context.ConnectionId);
                    }

                    // add to user's groups
                    List<string> groupNames = _theHubInstance.GetUserGroups(schemaName, userName);
                    foreach (string groupName in groupNames)
                    {
                        // regster on the global instance
                        _theHubInstance.GlobalContext.Groups.Add(Context.ConnectionId, groupName);
                    }

                    return base.OnConnected();
                });
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// OnDisconnected
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
            await Task.Factory.StartNew(() =>
            {
                _theHubInstance.RemoveConnectedId(Context.ConnectionId);
                return base.OnDisconnected(stopCalled);
            });
        }


        /// <summary>
        /// AddToGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HubAuthorizeAttribute]
        public async Task AddToGroup(string groupName)
        {
            await Task.Factory.StartNew(() =>
            {
                string userName = Context.Request.QueryString["user"];
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Missing username parameter in the query string");
                }
                string schemaName = Context.Request.QueryString["schema"];
                if (string.IsNullOrEmpty(schemaName))
                {
                    throw new Exception("Missing schema parameter in the query string");
                }

                return _theHubInstance.AddToGroup(Context.ConnectionId, schemaName, userName, groupName);
            });
        }

        /// <summary>
        /// RemoveFromGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HubAuthorizeAttribute]
        public async Task RemoveFromGroup(string groupName)
        {
            await Task.Factory.StartNew(() =>
            {
                string userName = Context.Request.QueryString["user"];
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Missing username parameter in the query string");
                }
                string schemaName = Context.Request.QueryString["schema"];
                if (string.IsNullOrEmpty(schemaName))
                {
                    throw new Exception("Missing schema parameter in the query string");
                }

                return _theHubInstance.RemoveFromGroup(Context.ConnectionId, schemaName, userName, groupName);
            });
        }

        /// <summary>
        /// GetUserGroups
        /// </summary>
        /// <returns></returns>
        [HubAuthorizeAttribute]
        public async Task<List<string>> GetUserGroups()
        {
            try
            {
                string userName = Context.Request.QueryString["user"];
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Missing username parameter in the query string");
                }
                string schemaName = Context.Request.QueryString["schema"];
                if (string.IsNullOrEmpty(schemaName))
                {
                    throw new Exception("Missing schema parameter in the query string");
                }

                List<string> groups = new List<string>();

                await Task.Factory.StartNew(() =>
                {
                    groups = _theHubInstance.GetUserGroups(schemaName, userName);
                });

                return groups;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                throw ex;
            }
        }

        /// <summary>
        /// IsUserInGroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HubAuthorizeAttribute]
        public async Task<bool> IsUserInGroup(string groupName)
        {
            try
            {
                bool status = false;

                string userName = Context.Request.QueryString["user"];
                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Missing username parameter in the query string");
                }
                string schemaName = Context.Request.QueryString["schema"];
                if (string.IsNullOrEmpty(schemaName))
                {
                    throw new Exception("Missing schema parameter in the query string");
                }

                List<string> groups = new List<string>();

                await Task.Factory.StartNew(() =>
                {
                    status = _theHubInstance.IsUserInGroup(schemaName, userName, groupName);
                });

                return status;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                throw ex;
            }
        }
    }
}  