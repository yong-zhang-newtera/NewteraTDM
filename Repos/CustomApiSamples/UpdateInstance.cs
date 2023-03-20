using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Ebaas.WebApi.Models;
using Newtera.WebForm;
using Newtera.Common.Core;

namespace CustomApiSamples
{
    public class UpdateInstance : IApiFunction
    {
        /// <summary>
        /// Execute an api function to create an instance
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            XmlDocument postDoc = context.InstanceData; // 这个API的Post数据是XML格式
            CMConnection con = context.Connection; //这个是数据库链接
            string className = context.ClassName; // 这个是修改数据实例的数据类英文名（URL的一部分)
            DataViewModel dataView = context.DataView; // 这个是修改数据记录的数据类的详细视图
            string oid = context.ObjID; // 这个是要修改数据实例的ObjID

            string query = null;
            CMCommand cmd;
            XmlDocument doc;
            InstanceView instanceView = null;

            // 首先从数据库获取要修改的数据实例
            query = dataView.GetInstanceQuery(oid);

            cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            // 检查数据库是否存在要修改的数据实例
            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                instanceView = new InstanceView(dataView, ds);

                InstanceEditor instanceEditor = new InstanceEditor();
                instanceEditor.EditInstance = instanceView;
                instanceEditor.ConvertToModel(postDoc.DocumentElement);  // 使用XML格式数据的修改数据实例

                //instanceEditor.Validate(); // 校验数据的正确性

                if (instanceView.InstanceData.IsChanged)
                {
                    query = instanceView.DataView.UpdateQuery; //生成修改语句

                    cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    doc = cmd.ExecuteXMLDoc(); // 执行数据库修改
                }
            }

            JObject result = new JObject();
            result.Add("status", "OK");

            return result;
        }
    }
}
