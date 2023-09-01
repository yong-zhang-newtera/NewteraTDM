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
            CMConnection con = context.Connection; //这个是数据库链接
            string className = context.ClassName; // 这个是创建数据实例的数据类英文名（URL的一部分)
            DataViewModel dataView = context.DataView; // 这个是创建数据记录的数据类的详细视图

            if (context.ContentType != "application/xml")
                throw new Exception("This api expects the type of content is application/xml");

            XmlDocument postDoc = context.InstanceData; // 这个API的Post数据是XML格式
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
                // 检查数据类是否为叶类，只有叶类才能创建数据记录
                throw new Exception("It is not allowed to add an instance to an abstract class " + className);
            }

            // 处理试验环境参数及工况，将两部分数据合并为一个一维数组
            PreprocessPostDoc(postDoc);

            /*
            var stringWriter = new StringWriter(new StringBuilder());
            var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = System.Xml.Formatting.Indented };
            postDoc.Save(xmlTextWriter);
            ErrorLog.Instance.WriteLine( stringWriter.ToString());
            */

            // 创建中间客体
            InstanceView instanceView = new InstanceView(dataView);

            InstanceEditor instanceEditor = new InstanceEditor();
            instanceEditor.EditInstance = instanceView;
            instanceEditor.ConvertToModel(postDoc.DocumentElement); // 将XML数据转换为插入的数据实例

            string query = instanceView.DataView.InsertQuery; // 生成创建数据记录的XQuery

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc(); // 执行创建语句，创建实例

            string oid = doc.DocumentElement.InnerText; // 获取新创建数据记录的唯一标识ID

            if (!string.IsNullOrEmpty(template))
            {
                // 使用提供的模板创建关联数据实例并建立链接。
                CreateRelatedInstances(con, className, dataView, template, oid, postDoc);
            }

            JObject ret = new JObject();
            ret.Add("oid", oid);
            return ret; // 返回ID
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
                // 创建新的EnvironmentParasDefine元素
                XmlElement newEnvironmentParasDefine = CreateNewEnvironmentParasDefine(doc, testScene);

                XmlElement originalEnvironmentParasDemand = testScene["EnvironmentParasDemand"];
                // 用新创建的newEnvironmentParasDemand元素替换试验场景元素中原始的EnvironmentParasDemand元素
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
            int columnCount = WorkingConditionList.Count + 1; // 参数 + 工况的格式形成列的数量

            // 添加列名行
            for (int i = 0; i < columnCount; i++)
            {
                newParaDefine = doc.CreateElement("ParaDefine");
                value = doc.CreateElement("Value");
                newParaDefine.AppendChild(value);
                if (i == 0)
                    value.InnerText = COLUMN_PREFIX + "参数名";
                else if (((XmlElement)WorkingConditionList[i - 1])["Name"] != null)
                    value.InnerText = COLUMN_PREFIX +((XmlElement)WorkingConditionList[i - 1])["Name"].InnerText;
                else
                    value.InnerText = COLUMN_PREFIX + "Working Condition " + i;

                newEnvironmentParasDefine.AppendChild(newParaDefine);
            }

            // 添加参数名和工况值行
            int rowIndex = 0;
            int rowCount = (paraDefineList.Count * WorkingConditionList.Count) / WorkingConditionList.Count;
            
            foreach (XmlElement paraDefine in paraDefineList)
            {
                // 创建参数名称元素
                newParaDefine = doc.CreateElement("ParaDefine");
                value = doc.CreateElement("Value");
                newParaDefine.AppendChild(value);
                value.InnerText = paraDefine["Name"].InnerText + "(" + paraDefine["Unit"].InnerText + ")";
                newEnvironmentParasDefine.AppendChild(newParaDefine);

                // 创建参数的对应工况值
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

                // 添加下一行数据
                rowIndex++;
            }

            // 返回新创建的EnvironmentParasDemand元素
            return newEnvironmentParasDefine;
        }
    }
}
