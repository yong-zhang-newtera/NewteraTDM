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
        /// 根据试验需求数据来创建试验任务实例
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            CMConnection con = context.Connection; //这个是数据库链接
            string className = context.ClassName; // 这个是试验任务数据类名称
            DataViewModel dataView = context.DataView; // 这个是试验任务的数据类的详细视图

            if (!context.ContentType.StartsWith("application/json"))
                throw new Exception("This api expects the type of content is application/json");

            JObject testOrderInstance = context.InstanceData; // 这个InstanceData是要创建的试验任务的初始值实例
            string requirementClass = null; // 试验需求数据类名称
            string requirementId = null; // 试验需求ObjID
            string requirementTemplate = null; // 获取试验需求及关联数据实例的表单模板
            string testOrderTemplate = null; // 创建试验任务及关联数据实例的表单模板

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
                // 检查数据类是否为叶类，只有叶类才能创建数据记录
                throw new Exception("It is not allowed to add an instance to an abstract class " + className);
            }

            // 根据试验需求数据，在试验任务实例(testOrderInstance)中添加试验项目和试验件的关联数据实例
            AddTestItemsAndSamples(con, testOrderInstance, requirementClass, requirementId, requirementTemplate);

            //以下两句语句用于打印Post的JSON数据,会出现在ServerLog.txt文件中
            //var jsonString = JsonConvert.SerializeObject(testOrderInstance, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine(jsonString);

            // 创建试验任务
            InstanceView instanceView = new InstanceView(dataView);

            InstanceEditor instanceEditor = new InstanceEditor();
            instanceEditor.EditInstance = instanceView;
            instanceEditor.ConvertToModel(testOrderInstance, true); // 将testOrderInstance数据转换为插入的数据实例

            string query = instanceView.DataView.InsertQuery; // 生成创建数据记录的XQuery

            CMCommand cmd = con.CreateCommand();
            cmd.CommandText = query;

            XmlDocument doc = cmd.ExecuteXMLDoc(); // 执行创建语句，创建实例

            string oid = doc.DocumentElement.InnerText; // 获取新创建数据记录的唯一标识ID

            // 使用提供的模板创建试验任务以及试验项目和样品等关联数据实例并建立链接。
            CreateRelatedInstances(con, className, dataView, testOrderTemplate, oid, testOrderInstance);

            JObject ret = new JObject();
            ret.Add("obj_id", oid);
            return ret; // 返回ID
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

            // 从试验需求中获取试验件类型的值，用该值查询样品类型实例，创建样品实例并加入到试验任务的实例中
            JArray sampleInstances = GetSampleInstances(con, requirementInstance);
            JObject samples = new JObject();
            samples.Add("toTestOrder", sampleInstances);

            // 将样品实例加入到试验任务实例中
            testOrderInstance.Add("EngineSamples", samples);

            // 从试验需求中获取创建试验项目所需的信息，并将创建的试验项目加入到试验任务实例中
            JArray testItemInstances = GetTestItemInstances(con, requirementInstance);
            JObject testItems = new JObject();
            testItems.Add("toTestOrder", testItemInstances);

            // 将样品实例加入到试验任务实例中
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

            // 获取试验需求中的试验件类型
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

            // 获取试验需求中的试验场景数组
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
                    // 用详细视图获取试验场景数据实例，以便获得所有的属性
                    JObject completeTestScene = APIUtil.GetDataInstance(con, "TestScene", testScene.GetValue("obj_id").ToString());
                    string testItemType = completeTestScene.GetValue("SceneType").ToString();

                    JObject testItemTemplateInstance = APIUtil.GetDataInstanceByKey(con, "EngineTestItemTemplates", "ItemName", testItemType);

                    if (testItemTemplateInstance != null)
                    {
                        testItemTemplateInstance["toTestScene"] = completeTestScene.GetValue("ID").ToString(); // 将试验项目与试验场景关联
                        testItemTemplateInstance["obj_id"] = null; // 清除obj_id，使其成为新数据实例
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
