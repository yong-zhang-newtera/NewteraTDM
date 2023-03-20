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
    public class GetInstances : IApiFunction
    {
        /// <summary>
        /// Execute an api function
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            JObject result = new JObject();
            JArray instances = new JArray();

            System.Collections.Specialized.NameValueCollection parameters = context.Parameters;
            DataViewModel dataView = context.DataView;
            CMConnection con = context.Connection;

            // 从dataView生成查询语句
            string query = dataView.SearchQuery;

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

                int count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                
                JObject instance;
                for (int row = 0; row < count; row++)
                {
                    instanceEditor.EditInstance.SelectedIndex = row; // set the cursor

                    // 转换为JSON的数据实例
                    instance = instanceEditor.ConvertToViewModel(false);

                    if (instance != null)
                    {
                        instances.Add(instance);
                    }
                }
            }

            result.Add("data", instances);

            return result;
        }
    }
}
