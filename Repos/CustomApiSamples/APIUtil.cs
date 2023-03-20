using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Data;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Ebaas.WebApi.Models;
using Newtera.Common.Wrapper;
using Newtera.Server.Engine.Workflow;
using Newtera.WebForm;
using Newtera.Common.Core;

namespace CustomApiSamples
{
    public class APIUtil
    {
        static public void SetTemplateToEditor(CMConnection con, CustomFormEditor formEditor, string className, string templateName)
        {
            bool foundTemplate = false;
            string templateBaseClass = className;
            string schemaId = con.MetaDataModel.SchemaInfo.NameAndVersion;

            while (true)
            {
                string formTemplatePath = NewteraNameSpace.GetFormTemplateDir(schemaId, templateBaseClass) + templateName;
                if (File.Exists(formTemplatePath))
                {
                    formEditor.TemplatePath = formTemplatePath;
                    foundTemplate = true;
                    break;
                }
                else
                {
                    // try to see if the template is defined for one of the parent classes
                    templateBaseClass = APIUtil.GetParentClassName(con, templateBaseClass);
                    if (templateBaseClass == null)
                    {
                        // no parent class
                        break;
                    }
                }
            }

            if (!foundTemplate)
            {
                throw new Exception("Unable to find a template with name " + templateName);
            }
        }

        // 该方法使用数据类的详细视图获取数据实例
        static public JObject GetDataInstance(CMConnection con, string className, string oid)
        {
            return GetDataInstanceByTemplate(con, className, oid, null);
        }

            // 该方法使用HTML模板一次性获取数据实例及其关联的数据实例
        static public JObject GetDataInstanceByTemplate(CMConnection con, string className, string oid, string templateName)
        {
            JObject instance = null;

            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

            if (dataView != null)
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

                if (!string.IsNullOrEmpty(templateName))
                {
                    CustomFormEditor formEditor = new CustomFormEditor();
                    formEditor.EditInstance = instanceView;
                    formEditor.ConnectionString = con.ConnectionString;
                    APIUtil.SetTemplateToEditor(con, formEditor, className, templateName);

                    instance = formEditor.ConvertToViewModel(true);
                }
                else
                {
                    InstanceEditor formEditor = new InstanceEditor();
                    formEditor.EditInstance = instanceView;
                    instance = formEditor.ConvertToViewModel(true);
                }
            }
            else
            {
                throw new Exception("Unable to create a data view for class " + className);
            }

            return instance;
        }

        static public JObject GetDataInstanceByKey(CMConnection con, string className, string keyName, string keyValue)
        {
            JObject instance = null;

            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

            if (dataView != null)
            {
                InstanceView instanceView;

                // create an instance query
                string query = dataView.GetInstanceByAttributeValueQuery(keyName, keyValue);

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    instanceView = new InstanceView(dataView, ds);

                    InstanceEditor instanceEditor = new InstanceEditor();
                    instanceEditor.EditInstance = instanceView;

                    instance = instanceEditor.ConvertToViewModel(true);
                }
            }
            else
            {
                throw new Exception("Unable to create a data view for class " + className);
            }

            return instance;
        }

        /// <summary>
        /// Get an initialized data instance
        /// </summary>
        /// <param name="con"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        static public JObject GetInitializedInstance(CMConnection con, string className)
        {
            JObject instance;

            DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

            InstanceView instanceView = new InstanceView(dataView);
            ClassElement classElement = con.MetaDataModel.SchemaModel.FindClass(className);
            string initCode = GetClassCustomCode(classElement, ClassElement.CLASS_INITIALIZATION_CODE);
            if (!string.IsNullOrEmpty(initCode))
            {
                // initialize the new instance with initialization code

                // Execute the initialization code using the same connection so that changes are made within a same transaction
                IInstanceWrapper instanceWrapper = new InstanceViewWrapper(instanceView);

                // run the initialization code on the instance
                ActionCodeRunner.Instance.ExecuteActionCode("GetNewAPI", "ClassInit" + classElement.ID, initCode, instanceWrapper);
            }

            InstanceEditor instanceEditor = new InstanceEditor();
            instanceEditor.EditInstance = instanceView;
            instance = instanceEditor.ConvertToViewModel(true);

            return instance;
        }

        static private string GetParentClassName(CMConnection con, string childClassName)
        {
            ClassElement childClassElement = con.MetaDataModel.SchemaModel.FindClass(childClassName);
            if (childClassElement != null && childClassElement.ParentClass != null)
            {
                return childClassElement.ParentClass.Name;
            }
            else
            {
                return null;
            }
        }

        static private string GetClassCustomCode(ClassElement classElement, string codeBlock)
        {
            string code = null;

            code = GetCustomCode(classElement, codeBlock);

            if (string.IsNullOrEmpty(code) &&
                classElement.ParentClass != null)
            {
                // use the code defined in the parent class
                code = GetClassCustomCode(classElement.ParentClass, codeBlock);
            }

            return code;
        }

        static private string GetCustomCode(ClassElement classElement, string codeBlock)
        {
            string code = null;

            switch (codeBlock)
            {
                case ClassElement.CLASS_INITIALIZATION_CODE:
                    code = classElement.InitializationCode;
                    break;

                case ClassElement.CLASS_BEFORE_INSERT_CODE:

                    code = classElement.BeforeInsertCode;
                    break;

                case ClassElement.CLASS_BEFORE_UPDATE_CODE:

                    code = classElement.BeforeUpdateCode;
                    break;

                case ClassElement.CLASS_CALLBACK_CODE:

                    code = classElement.CallbackFunctionCode;

                    break;
            }

            if (!string.IsNullOrEmpty(code))
            {
                code = code.Trim();
            }

            return code;
        }
    }
}
