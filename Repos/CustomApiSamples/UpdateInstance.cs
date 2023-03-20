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
            XmlDocument postDoc = context.InstanceData; // ���API��Post������XML��ʽ
            CMConnection con = context.Connection; //��������ݿ�����
            string className = context.ClassName; // ������޸�����ʵ����������Ӣ������URL��һ����)
            DataViewModel dataView = context.DataView; // ������޸����ݼ�¼�����������ϸ��ͼ
            string oid = context.ObjID; // �����Ҫ�޸�����ʵ����ObjID

            string query = null;
            CMCommand cmd;
            XmlDocument doc;
            InstanceView instanceView = null;

            // ���ȴ����ݿ��ȡҪ�޸ĵ�����ʵ��
            query = dataView.GetInstanceQuery(oid);

            cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            // ������ݿ��Ƿ����Ҫ�޸ĵ�����ʵ��
            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                instanceView = new InstanceView(dataView, ds);

                InstanceEditor instanceEditor = new InstanceEditor();
                instanceEditor.EditInstance = instanceView;
                instanceEditor.ConvertToModel(postDoc.DocumentElement);  // ʹ��XML��ʽ���ݵ��޸�����ʵ��

                //instanceEditor.Validate(); // У�����ݵ���ȷ��

                if (instanceView.InstanceData.IsChanged)
                {
                    query = instanceView.DataView.UpdateQuery; //�����޸����

                    cmd = con.CreateCommand();
                    cmd.CommandText = query;

                    doc = cmd.ExecuteXMLDoc(); // ִ�����ݿ��޸�
                }
            }

            JObject result = new JObject();
            result.Add("status", "OK");

            return result;
        }
    }
}
