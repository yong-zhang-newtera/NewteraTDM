using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Threading.Tasks;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData;
using Newtera.Data;
using Newtera.Common.Wrapper;

namespace WorkflowFunctionSamples
{
    /// <summary>
    /// 根据试验场景的工况种类及结果参数个数，在试验数据记录数据类创建多个数据记录实例，填写
    /// 工况名称和结果参数名，以便用户填写参数的值，而无需手工输入工况和结果参数名。
    /// </summary>
    public class CreateTestDataRecords : ICustomFunction
    {
        private const string TEST_RECORD_CLASS = "EngineTestRecords";
        private const string COLUMN_PREFIX = "C_";

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
                // 获取工况的名称集合
                List<string> workingConditions = GetWorkingConditions(instance);

                // 获取结果参数名称集合
                List<string> resultParaNames = GetResultParaNames(instance);

                string connectionStr = GetConnectionString(instance.SchemaId);
                using (CMConnection connection = new CMConnection(connectionStr))
                {
                    connection.Open();
                    // 在试验数据记录数据类创建工况和结果参数组合的记录实例，参数值为空，等待用户填写
                    foreach (string workingCondition in workingConditions)
                    {
                        foreach (string resultParaName in resultParaNames)
                        {
                            CreateOneToManyRelationship(connection, connection.MetaDataModel, instance, TEST_RECORD_CLASS, workingCondition, resultParaName);
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("SendTestOrderStatus has error:" + ex.Message + "\n" + ex.StackTrace);
                return "";
            }
        }

        private List<string> GetWorkingConditions(IInstanceWrapper instance)
        {
            // 从试验项目台账中获取试验场景实例，试验项目台账指向试验场景的多对一关系属性为“toTestScene”
            IInstanceWrapper testSceneInstance = instance.GetValue("toTestScene") as IInstanceWrapper;
            List<string> workingConditions = new List<string>();
            string[] workingConditionArray = testSceneInstance.GetValue("EnvironmentParasDefine") as string[]; //数组属性返回string[]

            // 参数名，工况1，工况2，...等等, 因而跳过第一个参数名
            for (int i = 1; i < workingConditionArray.Length; i++)
            {
                if (workingConditionArray[i].StartsWith(COLUMN_PREFIX))
                {
                    workingConditions.Add(workingConditionArray[i].ToString().Substring(COLUMN_PREFIX.Length));
                }
                else
                    break;
            }

            return workingConditions;
        }

        private List<string> GetResultParaNames(IInstanceWrapper instance)
        {
            // 从试验项目台账中获取试验场景实例，试验项目台账指向试验场景的多对一关系属性为“toTestScene”
            IInstanceWrapper testSceneInstance = instance.GetValue("toTestScene") as IInstanceWrapper;

            List<string> resultParaNames = new List<string>();
            string[] resultParaArray = testSceneInstance.GetValue("ResultParasDemand") as string[];

            int i = 0;
            while (i < resultParaArray.Length)
            {
                // Name;Unit;Measure;Name;Unit;Measure;Name;Unit;Measure;...
                resultParaNames.Add(resultParaArray[i] + "(" + resultParaArray[i + 1] + ")"); // Name(Unit)

                i += 3;
            }

            return resultParaNames;
        }

        private void CreateOneToManyRelationship(CMConnection connection, MetaDataModel metaData, IInstanceWrapper masterInstance, string relatedClassName, string workingCondition, string resultPara)
        {
            // find the relationship attribute that is a mant-to-one relationship from related class to the master class
            DataViewModel relatedClassDataView = metaData.GetDetailedDataView(relatedClassName);

            InstanceView relatedInstanceView = new InstanceView(relatedClassDataView);

            DataRelationshipAttribute relationshipAttribute = null;
            foreach (IDataViewElement resultAttribute in relatedClassDataView.ResultAttributes)
            {
                relationshipAttribute = resultAttribute as DataRelationshipAttribute;
                if (relationshipAttribute != null &&
                    relationshipAttribute.IsForeignKeyRequired)
                {
                    if (relationshipAttribute.LinkedClassName == masterInstance.OwnerClassName ||
                        IsParentOf(metaData, relationshipAttribute.LinkedClassName, masterInstance.OwnerClassName))
                    {
                        DataViewModel masterClassDataView = metaData.GetDetailedDataView(masterInstance.OwnerClassName);

                        SetRelationshipValue(connection, relatedInstanceView, relationshipAttribute, masterClassDataView, masterInstance.ObjId);

                        break;
                    }
                }
            }

            // 设置工况名及结果参数名
            relatedInstanceView.InstanceData.SetAttributeStringValue("WorkingCondition", workingCondition);
            relatedInstanceView.InstanceData.SetAttributeStringValue("ParaName", resultPara);

            string query = relatedClassDataView.InsertQuery;

            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.ExecuteXMLDoc(); // 创建记录
        }

        private void SetRelationshipValue(CMConnection connection, InstanceView instanceView,
            DataRelationshipAttribute relationshipAttribute, DataViewModel relatedDataView, string relatedInstanceId)
        {
            // first, we need get the primary key values of the related instance
            string query = relatedDataView.GetInstanceQuery(relatedInstanceId);
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            XmlReader reader = cmd.ExecuteXMLReader();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);

            InstanceView relatedInstanceView = new InstanceView(relatedDataView, ds);
            string[] pkValues = relatedInstanceView.InstanceData.PrimaryKeyValues.Split('&');
            // then set the primary key value as foreign key values to the instance view
            DataViewElementCollection primaryKeys = relationshipAttribute.PrimaryKeys;
            if (primaryKeys != null)
            {
                int index = 0;
                foreach (DataSimpleAttribute pk in primaryKeys)
                {
                    if (index < pkValues.Length && pkValues[index] != null)
                    {
                        // to set a pk value, the name combines that of relationship attribute and primary key
                        instanceView.InstanceData.SetAttributeValue(relationshipAttribute.Name + "_" + pk.Name, pkValues[index].Trim());
                    }
                    index++;
                }
            }
        }

        private bool IsParentOf(MetaDataModel metaData, string parentClassName, string childClassName)
        {
            bool status = false;

            ClassElement childClassElement = metaData.SchemaModel.FindClass(childClassName);
            if (childClassElement == null)
            {
                throw new Exception("Unable to find class element for " + childClassName);
            }

            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                status = true;
            }

            return status;
        }

        private string GetConnectionString(string schemaId)
        {
            string connectionString = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

            string[] strs = schemaId.Split(' ');

            return connectionString.Replace("{schemaName}", strs[0]);
        }

    }
}
