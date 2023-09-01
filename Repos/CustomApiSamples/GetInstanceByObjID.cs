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
using Newtera.WebApi.Models;
using Newtera.WebForm;
using Newtera.Common.Core;

namespace CustomApiSamples
{
    public class GetInstanceByObjID : IApiFunction
    {
        /// <summary>
        /// Execute an api function
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            JObject instance = new JObject();

            System.Collections.Specialized.NameValueCollection parameters = context.Parameters;

            string template = null;
            if (context.Parameters != null && context.Parameters["template"] != null)
            {
                template = context.Parameters["template"];
            }

            DataViewModel dataView = context.DataView;
            CMConnection con = context.Connection;
            string className = context.ClassName;
            string oid = context.ObjID;

            if (string.IsNullOrEmpty(template))
            {
                // 从dataView生成单个数据实例的查询语句（使用ObjId）
                string query = dataView.GetInstanceQuery(oid);

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                //执行查询语句
                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                // 接下来把返回的XML格式的数据记录转换为JSON格式
                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    InstanceView instanceView = new InstanceView(dataView, ds);

                    InstanceEditor instanceEditor = new InstanceEditor();
                    instanceEditor.EditInstance = instanceView;

                    // 转换为JSON的数据实例
                    instance = instanceEditor.ConvertToViewModel(false);
                }
            }
            else
            {
                instance = GetInstanceWithRelatedInstances(con, dataView, className, oid, template);
            }

            //var jsonString = JsonConvert.SerializeObject(instance, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine(jsonString);

            return instance;
        }

        private JObject GetInstanceWithRelatedInstances(CMConnection con, DataViewModel dataView, string className, string oid, string templateName)
        {
            InstanceView instanceView;

            // create an instance query
            string query = dataView.GetInstanceQuery(oid);

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            instanceView = new InstanceView(dataView, ds);

            CustomFormEditor formEditor = new CustomFormEditor();
            formEditor.EditInstance = instanceView;
            formEditor.ConnectionString = con.ConnectionString;
            APIUtil.SetTemplateToEditor(con, formEditor, className, templateName);

            return formEditor.ConvertToViewModel(false);
        }
    }
}
