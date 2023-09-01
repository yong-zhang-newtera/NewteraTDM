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
using Newtera.WebApi.Models;
using Newtera.WebForm;
using Newtera.Common.Core;

namespace CustomApiSamples
{
    public class CreateInstance : IApiFunction
    {
        private const string COLUMN_PREFIX = "C_";

        /// <summary>
        /// Execute an api function to create an instance
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            CMConnection con = context.Connection; //��������ݿ�����
            string className = context.ClassName; // ����Ǵ�������ʵ����������Ӣ������URL��һ����)
            DataViewModel dataView = context.DataView; // ����Ǵ������ݼ�¼�����������ϸ��ͼ

            if (context.ContentType != "application/xml")
                throw new Exception("This api expects the type of content is application/xml");

            XmlDocument postDoc = context.InstanceData; // ���API��Post������XML��ʽ
            string template = null;

            if (context.Parameters != null && context.Parameters["template"] != null)
                template = context.Parameters["template"];

            ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
            if (classElement == null)
            {
                throw new Exception("Unable to find a class with name " + className);
            }
            else if (!classElement.IsLeaf)
            {
                // ����������Ƿ�ΪҶ�ֻ࣬��Ҷ����ܴ������ݼ�¼
                throw new Exception("It is not allowed to add an instance to an abstract class " + className);
            }

            // �������黷�������������������������ݺϲ�Ϊһ��һά����
            PreprocessPostDoc(postDoc);

            /*
            var stringWriter = new StringWriter(new StringBuilder());
            var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = System.Xml.Formatting.Indented };
            postDoc.Save(xmlTextWriter);
            ErrorLog.Instance.WriteLine( stringWriter.ToString());
            */

            // �����м����
            InstanceView instanceView = new InstanceView(dataView);

            InstanceEditor instanceEditor = new InstanceEditor();
            instanceEditor.EditInstance = instanceView;
            instanceEditor.ConvertToModel(postDoc.DocumentElement); // ��XML����ת��Ϊ���������ʵ��

            string query = instanceView.DataView.InsertQuery; // ���ɴ������ݼ�¼��XQuery

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc(); // ִ�д�����䣬����ʵ��

            string oid = doc.DocumentElement.InnerText; // ��ȡ�´������ݼ�¼��Ψһ��ʶID

            if (!string.IsNullOrEmpty(template))
            {
                // ʹ���ṩ��ģ�崴����������ʵ�����������ӡ�
                CreateRelatedInstances(con, className, dataView, template, oid, postDoc);
            }

            JObject ret = new JObject();
            ret.Add("oid", oid);
            return ret; // ����ID
        }

        private void CreateRelatedInstances(CMConnection con, string className, DataViewModel dataView, string templateName, string oid, XmlDocument postDoc)
        {
            InstanceView instanceView;

            XmlElement instance = postDoc.DocumentElement;

            // create an instance query
            string query = dataView.GetInstanceQuery(oid);

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                instanceView = new InstanceView(dataView, ds);
            }
            else
            {
                instanceView = new InstanceView(dataView);
            }

            CustomFormEditor formEditor = new CustomFormEditor();
            formEditor.EditInstance = instanceView;
            formEditor.Instance = instance;
            formEditor.ConnectionString = con.ConnectionString;
            APIUtil.SetTemplateToEditor(con, formEditor, className, templateName);

            List<IInstanceEditor> relatedInstances = formEditor.ConvertToModel(instance);

            // save the data in form to the database
            formEditor.SaveToDatabase(relatedInstances);
        }

        private void PreprocessPostDoc(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;

            string path = @"TestScenes/TestScene";

            XmlNodeList testSceneList = root.SelectNodes(path);

            foreach (XmlElement testScene in testSceneList)
            {
                // �����µ�EnvironmentParasDefineԪ��
                XmlElement newEnvironmentParasDefine = CreateNewEnvironmentParasDefine(doc, testScene);

                XmlElement originalEnvironmentParasDemand = testScene["EnvironmentParasDemand"];
                // ���´�����newEnvironmentParasDemandԪ���滻���鳡��Ԫ����ԭʼ��EnvironmentParasDemandԪ��
                testScene.RemoveChild(originalEnvironmentParasDemand);
                testScene.AppendChild(newEnvironmentParasDefine);
            }
        }

        private XmlElement CreateNewEnvironmentParasDefine(XmlDocument doc, XmlElement testScene)
        {
            string path = @"EnvironmentParasDemand/EnvironmentParasDefine/ParaDefine";

            XmlNodeList paraDefineList = testScene.SelectNodes(path);

            path = @"EnvironmentParasDemand/WorkingConditions/WorkingCondition";

            XmlNodeList WorkingConditionList = testScene.SelectNodes(path);

            XmlElement newEnvironmentParasDefine = doc.CreateElement("EnvironmentParasDefine");
            
            XmlElement newParaDefine;
            XmlElement value;
            int columnCount = WorkingConditionList.Count + 1; // ���� + �����ĸ�ʽ�γ��е�����

            // ���������
            for (int i = 0; i < columnCount; i++)
            {
                newParaDefine = doc.CreateElement("ParaDefine");
                value = doc.CreateElement("Value");
                newParaDefine.AppendChild(value);
                if (i == 0)
                    value.InnerText = COLUMN_PREFIX + "������";
                else if (((XmlElement)WorkingConditionList[i - 1])["Name"] != null)
                    value.InnerText = COLUMN_PREFIX +((XmlElement)WorkingConditionList[i - 1])["Name"].InnerText;
                else
                    value.InnerText = COLUMN_PREFIX + "Working Condition " + i;

                newEnvironmentParasDefine.AppendChild(newParaDefine);
            }

            // ��Ӳ������͹���ֵ��
            int rowIndex = 0;
            int rowCount = (paraDefineList.Count * WorkingConditionList.Count) / WorkingConditionList.Count;
            
            foreach (XmlElement paraDefine in paraDefineList)
            {
                // ������������Ԫ��
                newParaDefine = doc.CreateElement("ParaDefine");
                value = doc.CreateElement("Value");
                newParaDefine.AppendChild(value);
                value.InnerText = paraDefine["Name"].InnerText + "(" + paraDefine["Unit"].InnerText + ")";
                newEnvironmentParasDefine.AppendChild(newParaDefine);

                // ���������Ķ�Ӧ����ֵ
                for (int i = 0; i < columnCount - 1; i++)
                {
                    newParaDefine = doc.CreateElement("ParaDefine");
                    value = doc.CreateElement("Value");
                    newParaDefine.AppendChild(value);

                    if (i < WorkingConditionList.Count)
                    {
                        XmlElement workingCondition = WorkingConditionList[i] as XmlElement;
                        XmlNodeList paraValueList = workingCondition.SelectNodes(@"ParaValues/Value");

                        if (rowIndex < rowCount)
                        {
                            value.InnerText = paraValueList[rowIndex].InnerText;
                        }
                        else
                            value.InnerText = "";
                    }
                    else
                        value.InnerText = "";

                    newEnvironmentParasDefine.AppendChild(newParaDefine);
                }

                // �����һ������
                rowIndex++;
            }

            // �����´�����EnvironmentParasDemandԪ��
            return newEnvironmentParasDefine;
        }
    }
}
