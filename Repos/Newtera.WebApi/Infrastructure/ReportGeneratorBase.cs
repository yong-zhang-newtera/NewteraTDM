using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.WebApi.Models;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.FileType;
using Newtera.ChartServer;

namespace Newtera.WebApi.Infrastructure
{
    /// <summary>
    /// A class that provides services for generating and downloading an excel report
    /// </summary>
    public class ReportGeneratorBase
    {
        protected const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        protected const string VariablePattern = @"\{[^\}]+\}";
        protected const string FILE_BASE_DIR_PROPERTY = "fileBaseDir";


        /// <summary>
        /// Get a InstanceView object
        /// </summary>
        /// <param name="connection">CMConnection</param>
        /// <param name="className">Class name of the instance view</param>
        /// <param name="objId">ObjId of the instance</param>
        /// <returns>An InstanceView object</returns>
        protected InstanceView GetInstanceView(CMConnection connection, string className, string objId)
        {
            InstanceView instanceView;

            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel instanceDataView = metaData.GetDetailedDataView(className);

            if (objId != null)
            {
                string query = instanceDataView.GetInstanceQuery(objId);

                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                instanceView = new InstanceView(instanceDataView, ds);
            }
            else
            {
                instanceView = new InstanceView(instanceDataView);
            }

            return instanceView;
        }

        protected string GetTemplatePath(CMConnection con, string currentClassName, string templateName)
        {
            bool foundTemplate = false;
            string repoartTemplatePath = null;
            string templateBaseClass = currentClassName;
            string schemaId = con.MetaDataModel.SchemaInfo.NameAndVersion;

            while (true)
            {
                repoartTemplatePath = NewteraNameSpace.GetReportTemplateDir(schemaId, templateBaseClass) + templateName;
                if (File.Exists(repoartTemplatePath))
                {
                    foundTemplate = true;
                    break;
                }
                else
                {
                    // try to see if the template is defined for one of the parent classes
                    templateBaseClass = GetParentClassName(con, templateBaseClass);
                    if (templateBaseClass == null)
                    {
                        // no parent class
                        break;
                    }
                }
            }

            if (!foundTemplate)
            {
                throw new Exception("Unable to find a report template at " + repoartTemplatePath);
            }

            return repoartTemplatePath;
        }

        protected string GetReportFileDir(InstanceView masterInstance, string baseDirAttribute)
        {
            string instanceFileDir = masterInstance.InstanceData.GetAttributeStringValue(baseDirAttribute);
            if (!string.IsNullOrEmpty(instanceFileDir) && instanceFileDir.Length > 0)
            {
                // fix the path
                if (instanceFileDir.StartsWith(@"\"))
                {
                    instanceFileDir = instanceFileDir.Substring(1); // remove the \ at beginning since the base path ends with one
                }

                if (!instanceFileDir.EndsWith(@"\"))
                {
                    instanceFileDir += @"\";
                }
            }

            return instanceFileDir;
        }

        protected string GetReportFilePath(string instanceFileDir)
        {
            string path = NewteraNameSpace.GetAttachmentDir();

            path = path + instanceFileDir;

            return path;
        }

        protected string GetTempFilePath(string fileName)
        {
            string userName = Thread.CurrentPrincipal.Identity.Name;

            fileName = userName + "-" + fileName;

            string dir = WebApiNameSpace.GetTempDir() + fileName;

            return dir;
        }

        protected string ReplaceVariables(string pattern, InstanceView masterInstanceView, ReportFileType reportType)
        {
            string text = pattern;

            if (!string.IsNullOrEmpty(text))
            {
                Regex patternExp = new Regex(ReportGeneratorBase.VariablePattern);

                MatchCollection matches = patternExp.Matches(pattern);
                if (matches.Count > 0)
                {
                    // contains variables
                    string propertyName;
                    string propertyValue;
                    if (masterInstanceView != null)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Value.Length > 2)
                            {
                                // variable is in form of {propertyName}
                                propertyName = match.Value.Substring(1, (match.Value.Length - 2));
                                try
                                {
                                    propertyValue = masterInstanceView.InstanceData.GetAttributeStringValue(propertyName);
                                    if (!string.IsNullOrEmpty(propertyValue))
                                    {
                                        // replace the variable with value
                                        text = text.Replace(match.Value, propertyValue);
                                    }
                                }
                                catch (Exception)
                                {
                                    // ignore
                                }
                            }
                        }
                    }
                }
            }

            return GetReportFileName(text, reportType);
        }

        protected string GetReportFileName(string templateFile, ReportFileType reportType)
        {
            if (templateFile != null && reportType == ReportFileType.PDF)
            {
                // replace the suffix with .pdf
                return templateFile.ToLower().Replace(".xsl", ".pdf");
            }
            else
            {
                // no change
                return templateFile;
            }
        }

        protected string GetParentClassName(CMConnection con, string childClassName)
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

        protected string GetWebBaseURL()
        {
            string baseURL = "";
            if (!baseURL.EndsWith("/"))
            {
                baseURL += "/";
            }

            return baseURL;
        }

        protected string GetFileBaseDir(string schemaName, string className, string oid)
        {
            string baseDir = null;

            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                // create an instance query
                string query = dataView.GetInstanceQuery(oid);

                //ErrorLog.Instance.WriteLine("GetOne Search Query is " + query);

                CMCommand cmd = con.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    InstanceView instanceView = new InstanceView(dataView, ds);

                    baseDir = instanceView.InstanceData.GetAttributeStringValue(FILE_BASE_DIR_PROPERTY);
                    if (!string.IsNullOrEmpty(baseDir))
                    {
                        if (!baseDir.EndsWith(@"\"))
                        {
                            baseDir += @"\";
                        }

                        baseDir = NewteraNameSpace.GetUserFilesDir() + baseDir;
                    }
                }
            }

            return baseDir;
        }

        protected string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}