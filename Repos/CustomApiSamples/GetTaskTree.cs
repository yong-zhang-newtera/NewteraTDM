using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Ebaas.WebApi.Models;
using Newtera.WebForm;
using Newtera.Common.Core;

namespace CustomApiSamples
{
    public class GetTaskTree : IApiFunction
    {
        /// <summary>
        /// Execute an api function
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            TaskTreeNode root = new TaskTreeNode();

            string taskNodeAttribute = context?.Parameters?["taskNodeAttribute"];
            string itemNodeAttribute = context?.Parameters?["itemNodeAttribute"];
            string packetNodeAttribute = context?.Parameters?["packetNodeAttribute"];
            string itemClass = context?.Parameters?["itemClass"];
            string packetClass = context?.Parameters?["packetClass"];

            var taskInstance = GetTaskInstance(context);
            root.ID = taskInstance.InstanceData?.ObjId;
            root.Name = taskInstance.InstanceData?.GetAttributeStringValue(taskNodeAttribute);
            root.ClassName = taskInstance.DataView.BaseClass.ClassName;

            var itemInstanceView = GetRelatedInstances(context, taskInstance, itemClass);
            int itemCount = itemInstanceView.InstanceCount;
            for (int itemRow = 0; itemRow < itemCount; itemRow++)
            {
                itemInstanceView.SelectedIndex = itemRow;
                var itemNode = new TaskTreeNode();
                itemNode.ID = itemInstanceView.InstanceData.ObjId;
                itemNode.Name = itemInstanceView.InstanceData.GetAttributeStringValue(itemNodeAttribute);
                itemNode.ClassName = itemInstanceView.DataView.BaseClass.ClassName;
                root.Children.Add(itemNode);

                var packetInstanceView = GetRelatedInstances(context, itemInstanceView, packetClass);
                int packetCount = packetInstanceView.InstanceCount;
                for (int packetRow = 0; packetRow < packetCount; packetRow++)
                {
                    packetInstanceView.SelectedIndex = itemRow;
                    var packetNode = new TaskTreeNode();
                    packetNode.ID = packetInstanceView.InstanceData.ObjId;
                    packetNode.Name = packetInstanceView.InstanceData.GetAttributeStringValue(packetNodeAttribute);
                    packetNode.ClassName = packetInstanceView.DataView.BaseClass.ClassName;
                    itemNode.Children.Add(packetNode);
                }
            }

            return JObject.FromObject(root);
        }

        private InstanceView GetTaskInstance(ApiExecutionContext context)
        {
            DataViewModel dataView = context.DataView;
            CMConnection con = context.Connection;
            string oid = context.ObjID;

            string query = dataView.GetInstanceQuery(oid);

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                InstanceView instanceView = new InstanceView(dataView, ds);

                return instanceView;
            }
            else
            {
                return new InstanceView(dataView);
            }
        }

        private InstanceView GetRelatedInstances(ApiExecutionContext context, InstanceView masterInstanceView, string relatedClass)
        { 
            CMConnection con = context.Connection;
            var dataView = con.MetaDataModel.GetRelatedDefaultDataView(masterInstanceView, relatedClass);
            dataView.PageSize = 200;

            string query = dataView.SearchQueryWithPKValues;

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                return new InstanceView(dataView, ds);
            }
            else
            {
                return new InstanceView(dataView);
            }
        }
    }

    public class TaskTreeNode
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public List<TaskTreeNode> Children { get; }

        public TaskTreeNode()
        {
            this.Children = new List<TaskTreeNode>();
        }
    }
}
