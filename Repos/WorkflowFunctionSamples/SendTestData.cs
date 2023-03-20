using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Newtera.Common.Core;
using Newtera.Common.Wrapper;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Data;

namespace WorkflowFunctionSamples
{
    /// <summary>
    /// 向天舟系统发送XML格式的试验数据并修改该试验场景的状态为“完成”
    /// </summary>
    public class SendTestData : ICustomFunction
    {
        /// <summary>
        /// Execute an custom function
        /// </summary>
        /// <param name="instance">The instance of IInstanceWrapper 
        /// interface that represents the data instance bound to a
        /// workflow instance.</param>
        /// <returns>A string return from custome function</returns>
        public string Execute(IInstanceWrapper instance)
        {
            try
            {
                string tSISceneID = instance.GetString("SceneID"); // 试验项目的虚拟属性返回试验场景的ID

                SkyServiceProxy serviceProxy = new SkyServiceProxy();
                serviceProxy.SetServerIP();

                string rs = string.Empty;

                /* 发送试验数据代码移到CustomApiSamples项目的SendTestDataToSky中了。
                 * 不在工作流函数中发送数据了
                string[] rejectedParas = null;

                string xmlStr = GetXMLTestResult(tSISceneID, instance);
                ErrorLog.Instance.WriteLine(xmlStr);
                rs = serviceProxy.SetTestResultData(xmlStr, out rejectedParas);

                if (rejectedParas != null)
                {
                    ErrorLog.Instance.WriteLine(rejectedParas.ToString());
                }
                */

                rs = string.Empty;
                string tDMStatus = "完成";//试验项目结束时请设置为“完成”
                string tDMUrl = "";
                rs = serviceProxy.SetTestStatus(tSISceneID, tDMStatus, tDMUrl);

                return rs;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("SendTestData has error:" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// TDM生成的XML数据的例子如下，需要转换为天舟要求的XML数据格式
        /// 
        /// <仿真试验项目台账>
        ///     <项目名称>外特性试验</项目名称>
        ///     <仿真试验场景>
        ///         <试验场景名称>外特性试验</试验场景名称>
        ///         <试验场景类型>外特性试验</试验场景类型>
        ///         <试验场景编码>fe272b83-365e-4bb7-9bb1-35178bed7b22</试验场景编码>
        ///         <环境参数定义>温度;K;热力学温度;AA;kg;质量</环境参数定义>
        ///         <试验工况>额定工况;;12,34</试验工况>
        ///         <结果数据参数>冷却液出口温度;K;热力学温度;排气压力;kg;质量</结果数据参数>
        ///     </仿真试验场景>
        ///      <发动机试验数据记录>
        ///         <记录序号>23</记录序号>
        ///         <工况>工况1名称工况500</工况>
        ///         <参数名>冷却液出口温度(K)</参数名>
        ///         <参数值>12</参数值>
        ///      </发动机试验数据记录>
        ///     <发动机试验数据记录>
        ///         <记录序号>24</记录序号>
        ///         <工况>工况1名称工况500</工况>
        ///         <参数名>排气压力(kg)</参数名>
        ///         <参数值>33</参数值>
        ///      </发动机试验数据记录>
        ///      <发动机试验数据记录>
        ///         <记录序号>25</记录序号>
        ///         <工况>工况2名称工况800</工况>
        ///         <参数名>冷却液出口温度(K)</参数名>
        ///         <参数值>44</参数值>
        ///     </发动机试验数据记录>
        ///     <发动机试验数据记录>
        ///         <记录序号>26</记录序号>
        ///         <工况>工况2名称工况800</工况>
        ///         <参数名>排气压力(kg)</参数名>
        ///         <参数值>55</参数值>
        ///     </发动机试验数据记录>
        /// </仿真试验项目台账>
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private string GetXMLTestResult(string sceneID, IInstanceWrapper instance)
        {

            string xml = "";

            string xmlSchemaName = instance.GetString("XmlSchemaName"); // 获取该测试项目的XML架构名称来获得XML结果数据
            if (!string.IsNullOrEmpty(xmlSchemaName))
            {
                try
                {
                    string connectionStr = GetConnectionString(instance.SchemaId);
                    using (CMConnection connection = new CMConnection(connectionStr))
                    {
                        connection.Open();

                        MetaDataModel metaData = connection.MetaDataModel;

                        XMLSchemaModel xmlSchemaModel = metaData.XMLSchemaViews[xmlSchemaName] as XMLSchemaModel;

                        if (xmlSchemaModel != null &&
                            xmlSchemaModel.RootElement.ElementType == instance.OwnerClassName)
                        {
                            // 使用ObjId获取试验项目的数据实例
                            DataViewModel dataView = metaData.GetDetailedDataView(instance.OwnerClassName);
                            string query = dataView.GetInstanceQuery(instance.ObjId);

                            CMCommand cmd = connection.CreateCommand();
                            cmd.CommandText = query;

                            XmlDocument doc;

                            // 根据Xml架构生成XML doc
                            doc = cmd.GenerateXmlDoc(xmlSchemaModel, dataView);

                            /*
                            var stringWriter = new StringWriter(new StringBuilder());
                            var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = System.Xml.Formatting.Indented };
                            doc.Save(xmlTextWriter);
                            ErrorLog.Instance.WriteLine( stringWriter.ToString());
                            */

                            // 转换为天舟要求的XML格式
                            doc = ConvertFormat(doc);

                            // 将 Xml Doc转换为xml string
                            StringWriter sw;
                            XmlTextWriter tx;
                            sw = new StringWriter();
                            tx = new XmlTextWriter(sw);
                            tx.Formatting = Formatting.Indented;
                            doc.WriteTo(tx);
                            xml = sw.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
            } 

            return xml;
        }

        /// <summary>
        /// 转换为天舟要求的XML数据格式例子
        /// <?xml version = "1.0" encoding="utf-8" ?>
        /// <TestTDMResultData xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        ///            xmlns:xsd="http://www.w3.org/2001/XMLSchema"
        ///            Version="1.0.0">
        ///     <TestScenes>
        ///         <TestScene>
        ///             <SceneID>17d804db-79c7-492d-8181-24b0bed6f310</SceneID>
        ///             <WorkingConditions>
        ///                 <WorkingCondition>
        ///                   <Name>工况1</Name>
        ///                   <Description>第一行的数据</Description>
        ///                   <TDMRowID>行1</TDMRowID>
        ///                   <ResultParas>
        ///                     <Para>
        ///                       <Name>速度</Name>
        ///                       <Measure>速度</Measure>
        ///                       <Unit>r/m</Unit>
        ///                       <Values>
        ///                         <Value>1000</Value>
        ///                       </Values>
        ///                     </Para>
        ///                     <Para>
        ///                       <Name>角度</Name>
        ///                       <Measure>度</Measure>
        ///                       <Unit></Unit>
        ///                       <Values>
        ///                         <Value>60</Value>
        ///                       </Values>
        ///                     </Para>
        ///                     <Para>
        ///                   </ResultParas>
        ///                 </WorkingCondition>
        ///                 <WorkingCondition>
        ///                   <Name>工况2</Name>
        ///                   <Description>第二行的数据</Description>
        ///                   <TDMRowID>行2</TDMRowID>
        ///                   <ResultParas>
        ///                     <Para>
        ///                       <Name>速度</Name>
        ///                       <Measure>速度</Measure>
        ///                       <Unit>r/m</Unit>
        ///                       <Values>
        ///                         <Value>1001</Value>
        ///                       </Values>
        ///                     </Para>
        ///                     <Para>
        ///                       <Name>角度</Name>
        ///                       <Measure>度</Measure>
        ///                       <Unit></Unit>
        ///                       <Values>
        ///                         <Value>61</Value>
        ///                       </Values>
        ///                     </Para>
        ///                   </ResultParas>
        ///                 </WorkingCondition>
        ///             </WorkingConditions>
        ///             <ResultFiles>
        ///                 <ResultFile>
        ///                   <FileURL>www.baidu.com</FileURL>
        ///                   <FileName>aa</FileName>
        ///                   <FileType>doc</FileType>
        ///                 </ResultFile>
        ///                 <ResultFile>
        ///                   <FileURL>www.google.com</FileURL>
        ///                   <FileName>aaa.doc</FileName>
        ///                   <FileType>doc</FileType>
        ///                 </ResultFile>
        ///             </ResultFiles>
        ///         </TestScene>
        ///     </TestScenes>
        /// </TestTDMResultData>
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private XmlDocument ConvertFormat(XmlDocument original)
        {
            XmlDocument doc = new XmlDocument();

            //创建根元素
            XmlElement parentElement = doc.CreateElement("TestTDMResultData");
            doc.AppendChild(parentElement);

            // 创建TestScenes元素
            XmlElement childElement = doc.CreateElement("TestScenes");

            parentElement.AppendChild(childElement);

            parentElement = childElement;

            // 创建TestScene元素
            XmlElement testScene = doc.CreateElement("TestScene");
            parentElement.AppendChild(testScene);
            parentElement = testScene;

            // 创建TestSceneID元素
            childElement = doc.CreateElement("SceneID");
            XmlElement found = GetChildElement(original, @"/仿真试验项目台账/仿真试验场景/试验场景编码", 0); // 从原始XML文件中按照指定路径获得对应元素的值
            childElement.InnerText = found.InnerText;
            parentElement.AppendChild(childElement);

            // 创建WorkingConditions元素
            childElement = doc.CreateElement("WorkingConditions");
            parentElement.AppendChild(childElement);

            int numOfRecords = GetElementCount(original, @"/仿真试验项目台账/发动机试验数据记录");

            List<ParameterDef> parameters = GetParameters(original, @"/仿真试验项目台账/仿真试验场景/结果数据参数");

            XmlElement workingConditions = childElement;
            XmlElement resultParas;
            for (int i = 0; i < numOfRecords; i++)
            {
                // 获取第i个试验数据记录元素
                XmlElement recordElement = GetChildElement(original, @"/仿真试验项目台账/发动机试验数据记录", i);

                XmlElement workingConditionElement = FindWorkingConditionElement(workingConditions, recordElement["工况"].InnerText);
                if (workingConditionElement == null)
                {
                    // 这个工况第一次出现
                    workingConditionElement = doc.CreateElement("WorkingCondition");
                    workingConditions.AppendChild(workingConditionElement);
                    parentElement = workingConditionElement;

                    childElement = doc.CreateElement("Name");
                    childElement.InnerText = recordElement["工况"].InnerText;
                    parentElement.AppendChild(childElement);

                    childElement = doc.CreateElement("Description");
                    childElement.InnerText = recordElement["工况"].InnerText + "数据";
                    parentElement.AppendChild(childElement);

                    childElement = doc.CreateElement("TDMRowID");
                    childElement.InnerText = recordElement["记录序号"].InnerText;
                    parentElement.AppendChild(childElement);

                    resultParas = doc.CreateElement("ResultParas");
                    parentElement.AppendChild(resultParas);
                }
                else
                {
                    resultParas = workingConditionElement["ResultParas"];
                }

                for (int j = 0; j < parameters.Count; j++)
                {
                    if (recordElement["参数名"].InnerText.StartsWith(parameters[j].Name))
                    {
                        childElement = doc.CreateElement("Para");
                        resultParas.AppendChild(childElement);
                        parentElement = childElement;

                        childElement = doc.CreateElement("Name");
                        childElement.InnerText = parameters[j].Name;
                        parentElement.AppendChild(childElement);

                        childElement = doc.CreateElement("Unit");
                        childElement.InnerText = parameters[j].Unit;
                        parentElement.AppendChild(childElement);

                        childElement = doc.CreateElement("Measure");
                        childElement.InnerText = parameters[j].Measure;
                        parentElement.AppendChild(childElement);

                        childElement = doc.CreateElement("Values");
                        parentElement.AppendChild(childElement);
                        parentElement = childElement;

                        // 获取试验数据值
                        childElement = doc.CreateElement("Value");
                        try
                        {
                            XmlNode valueElement = recordElement["参数值"];
                            if (valueElement != null)
                            {
                                childElement.InnerText = valueElement.InnerText;
                            }
                            else
                                childElement.InnerText = "";

                            parentElement.AppendChild(childElement);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                        }

                        break;
                    }
                }
            }

            // 暂时没有文件，创建空的ResultFiles 元素
            childElement = doc.CreateElement("ResultFiles");
            testScene.AppendChild(childElement);

            return doc;
        }

        private XmlElement FindWorkingConditionElement(XmlElement workingConditions, string workingConditionName)
        {
            XmlElement workingConditionElement = null;

            // 查看同名的工况元素是否已经创建了
            foreach (XmlElement workingCondition in workingConditions.ChildNodes)
            {
                if (workingCondition["Name"].InnerText == workingConditionName)
                {
                    workingConditionElement = workingCondition;

                    break;
                }
            }

            return workingConditionElement;
        }

        private XmlElement GetChildElement(XmlDocument xmlDoc, string path, int index)
        {
            XmlElement found = null;

            // throw exception if the element(s) not found
            XmlNodeList nodeList = xmlDoc.SelectNodes(path);
            if (nodeList != null && nodeList.Count > 0)
            {
                if (nodeList.Count > index)
                {
                    found = nodeList[index] as XmlElement;
                }
                else
                    throw new Exception("在XMLDoc中找不到路径为" + path + "的第" + (index + 1) + "个元素。");
            }
            else
            {
                throw new Exception("在XMLDoc中找不到路径为" + path + "的元素。");
            }

            return found;
        }

        private int GetElementCount(XmlDocument xmlDoc, string path)
        {
            int count = 0;

            // throw exception if the element(s) not found
            XmlNodeList nodeList = xmlDoc.SelectNodes(path);
            if (nodeList != null && nodeList.Count > 0)
            {
                count = nodeList.Count;
            }
            else
            {
                throw new Exception("在XMLDoc中找不到路径为" + path + "的元素。");
            }

            return count;
        }

        private List<ParameterDef> GetParameters(XmlDocument xmlDoc, string path)
        {
            List<ParameterDef> parameters = new List<ParameterDef>();

            XmlElement found = GetChildElement(xmlDoc, path, 0);

            if (!string.IsNullOrEmpty(found.InnerText))
            {
                string[] values = found.InnerText.Split(';');

                // 值的例子： 冷却液出口温度;K;热力学温度;排气压力;kg;质量
                // 分别是Name，Unit，Measure的定义
                for (int i = 0; i < values.Length; i += 3)
                {
                    // 每三个值为一组定义
                    ParameterDef parameterDef = new ParameterDef();

                    try
                    {
                        parameterDef.Name = values[i];
                        parameterDef.Unit = values[i + 1];
                        parameterDef.Measure = values[i + 2];
                    }
                    catch (Exception)
                    {

                    }

                    parameters.Add(parameterDef);
                }
            }

            return parameters;
        }

        private string GetConnectionString(string schemaId)
        {
            string connectionString = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

            string[] strs = schemaId.Split(' ');

            return connectionString.Replace("{schemaName}", strs[0]);
        }

    }

    public class ParameterDef
    {
        public string Name;
        public string Unit;
        public string Measure;
        public string Value;
    }
}
