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
    public class CreateTestOrder : IApiFunction
    {
        /// <summary>
        /// ������������������������������ʵ��
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            CMConnection con = context.Connection; //��������ݿ�����
            string className = context.ClassName; // �����������������������
            DataViewModel dataView = context.DataView; // �����������������������ϸ��ͼ

            if (!context.ContentType.StartsWith("application/json"))
                throw new Exception("This api expects the type of content is application/json");

            JObject testOrderInstance = context.InstanceData; // ���InstanceData��Ҫ��������������ĳ�ʼֵʵ��
            string requirementClass = null; // ������������������
            string requirementId = null; // ��������ObjID
            string requirementTemplate = null; // ��ȡ�������󼰹�������ʵ���ı�ģ��
            string testOrderTemplate = null; // �����������񼰹�������ʵ���ı�ģ��

            if (context.Parameters == null)
                throw new Exception("Missing parameters in the query string of url");

            if (context.Parameters["source"] != null)
                requirementClass = context.Parameters["source"];
            else
                throw new Exception("Missing source paramemeter in the query string of url");

            if (context.Parameters["sourceid"] != null)
                requirementId = context.Parameters["sourceid"];
            else
                throw new Exception("Missing sourceid paramemeter in the query string of url");

            if (context.Parameters["sourcetemplate"] != null)
                requirementTemplate = context.Parameters["sourcetemplate"];
            else
                throw new Exception("Missing sourcetemplate paramemeter in the query string of url");

            if (context.Parameters["targettemplate"] != null)
                testOrderTemplate = context.Parameters["targettemplate"];
            else
                throw new Exception("Missing targettemplate paramemeter in the query string of url");

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

            // ���������������ݣ�����������ʵ��(testOrderInstance)�����������Ŀ��������Ĺ�������ʵ��
            AddTestItemsAndSamples(con, testOrderInstance, requirementClass, requirementId, requirementTemplate);

            //��������������ڴ�ӡPost��JSON����,�������ServerLog.txt�ļ���
            //var jsonString = JsonConvert.SerializeObject(testOrderInstance, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine(jsonString);

            // ������������
            InstanceView instanceView = new InstanceView(dataView);

            InstanceEditor instanceEditor = new InstanceEditor();
            instanceEditor.EditInstance = instanceView;
            instanceEditor.ConvertToModel(testOrderInstance, true); // ��testOrderInstance����ת��Ϊ���������ʵ��

            string query = instanceView.DataView.InsertQuery; // ���ɴ������ݼ�¼��XQuery

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc(); // ִ�д�����䣬����ʵ��

            string oid = doc.DocumentElement.InnerText; // ��ȡ�´������ݼ�¼��Ψһ��ʶID

            // ʹ���ṩ��ģ�崴�����������Լ�������Ŀ����Ʒ�ȹ�������ʵ�����������ӡ�
            CreateRelatedInstances(con, className, dataView, testOrderTemplate, oid, testOrderInstance);

            JObject ret = new JObject();
            ret.Add("obj_id", oid);
            return ret; // ����ID
        }

        private void CreateRelatedInstances(CMConnection con, string className, DataViewModel dataView, string templateName, string oid, JObject testOrderInstance)
        {
            InstanceView instanceView;

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
            formEditor.Instance = testOrderInstance;
            formEditor.ConnectionString = con.ConnectionString;
            APIUtil.SetTemplateToEditor(con, formEditor, className, templateName);

            List<IInstanceEditor> relatedInstances = formEditor.ConvertToModel(testOrderInstance, true);

            // save the data in form to the database
            formEditor.SaveToDatabase(relatedInstances);
        }

        private void AddTestItemsAndSamples(CMConnection con, JObject testOrderInstance, string requirementClass, string requirementId, string requirementTemplate)
        {
            JObject requirementInstance = APIUtil.GetDataInstanceByTemplate(con, requirementClass, requirementId, requirementTemplate);

            // �����������л�ȡ��������͵�ֵ���ø�ֵ��ѯ��Ʒ����ʵ����������Ʒʵ�������뵽���������ʵ����
            JArray sampleInstances = GetSampleInstances(con, requirementInstance);
            JObject samples = new JObject();
            samples.Add("toTestOrder", sampleInstances);

            // ����Ʒʵ�����뵽��������ʵ����
            testOrderInstance.Add("EngineSamples", samples);

            // �����������л�ȡ����������Ŀ�������Ϣ������������������Ŀ���뵽��������ʵ����
            JArray testItemInstances = GetTestItemInstances(con, requirementInstance);
            JObject testItems = new JObject();
            testItems.Add("toTestOrder", testItemInstances);

            // ����Ʒʵ�����뵽��������ʵ����
            testOrderInstance.Add("EngineTestItemInstances", testItems);

            // add reference to the test requirement
            testOrderInstance["toTestRequirement"] = requirementInstance.GetValue("ID").ToString();
        }

        private JArray GetSampleInstances(CMConnection con, JObject requirementInstance)
        {
            JArray sampleInstances = new JArray();
            JObject sampleInstance = null;

            string sampleType = null;
            string sampleNumber = null;
            string sampleVersion = null;

            // ��ȡ���������е����������
            if (requirementInstance.GetValue("DesignSimuObjInfo") != null)
            {
                JObject simuObj = requirementInstance.GetValue("DesignSimuObjInfo") as JObject;
                if (simuObj != null && simuObj.GetValue("ObjType") != null)
                {
                    sampleType = simuObj.GetValue("ObjType").ToString();
                    sampleNumber = simuObj.GetValue("ObjID").ToString();
                    sampleVersion = simuObj.GetValue("ObjVersion").ToString();
                }
            }

            if (sampleType != null)
            {
                JObject sampleTypeInstance = APIUtil.GetDataInstanceByKey(con, "SampleTypes", "TypeName", sampleType);

                if (sampleTypeInstance != null)
                {
                    //string sampleClassName = sampleTypeInstance.GetValue("BottomClassName").ToString();
                    string sampleClassName = "EngineSamples";

                    if (!string.IsNullOrEmpty(sampleClassName))
                    {
                        //sampleInstance = APIUtil.GetInitializedInstance(con, sampleClassName);
                        sampleInstance = new JObject();
                        sampleInstance.Add("Name", sampleType);
                        sampleInstance.Add("ObjID", sampleNumber);
                        sampleInstance.Add("BatchNumber", sampleVersion);
                        sampleInstance.Add("FormName", "EngineSample.htm");

                        if (sampleInstance != null)
                        {
                            sampleInstances.Add(sampleInstance);
                        }
                        else
                            throw new Exception("Unable to get an initialized instance for " + sampleClassName);
                    }
                }
                else
                {
                    throw new Exception("Unable to find a sample type instance with name " + sampleType + " in class EngineSampleTypes");
                }
            }

            return sampleInstances;
        }

        private JArray GetTestItemInstances(CMConnection con, JObject requirementInstance)
        {
            JArray testItemInstances = new JArray();
            JArray testSceneArray = null;

            // ��ȡ���������е����鳡������
            if (requirementInstance.GetValue("TestScene") != null)
            {
                JObject testSceneClassObj = requirementInstance.GetValue("TestScene") as JObject;
                if (testSceneClassObj != null && testSceneClassObj.GetValue("toCoTDMTestRequireData") != null)
                {
                    testSceneArray = testSceneClassObj.GetValue("toCoTDMTestRequireData") as JArray;
                }
            }

            if (testSceneArray != null)
            {
                foreach (JObject testScene in testSceneArray)
                {
                    // ����ϸ��ͼ��ȡ���鳡������ʵ�����Ա������е�����
                    JObject completeTestScene = APIUtil.GetDataInstance(con, "TestScene", testScene.GetValue("obj_id").ToString());
                    string testItemType = completeTestScene.GetValue("SceneType").ToString();

                    JObject testItemTemplateInstance = APIUtil.GetDataInstanceByKey(con, "EngineTestItemTemplates", "ItemName", testItemType);

                    if (testItemTemplateInstance != null)
                    {
                        testItemTemplateInstance["toTestScene"] = completeTestScene.GetValue("ID").ToString(); // ��������Ŀ�����鳡������
                        testItemTemplateInstance["obj_id"] = null; // ���obj_id��ʹ���Ϊ������ʵ��
                        testItemInstances.Add(testItemTemplateInstance);
                    }
                    else
                    {
                        throw new Exception("Unable to find a Test Item Template with name " + testItemType + " in class EngineTestItemTemplates");
                    }
                }
            }

            return testItemInstances;
        }

    }
}
