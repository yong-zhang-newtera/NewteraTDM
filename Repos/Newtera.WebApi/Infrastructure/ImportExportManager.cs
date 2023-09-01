using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Import;
using Newtera.WebApi.Models;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Mappings;

namespace Newtera.WebApi.Infrastructure
{
    /// <summary>
    /// Import and Export utility class
    /// </summary>
    public class ImportExportManager : IImportExportManager
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        /// <summary>
        /// Get Import Scripts defined for a class
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="fileType">Indicate the file type that scripts can import, values are All, Excel, Text, Other</param>
        /// <returns></returns>
        public async Task<IEnumerable<ScriptViewModel>> GetScripts(string schemaName, string className, string fileType)
        {
            List<ScriptViewModel> scripts = new List<ScriptViewModel>();

            await Task.Factory.StartNew(() =>
            {
                scripts = GetScriptViews(schemaName, className, fileType);
            });

            return scripts;
        }

        /// <summary>
        /// Import data in a file to a class using the script specified
        /// </summary>
        /// <param name="request"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="script"></param>
        /// <returns>Import status</returns>
        public async Task<string> ImportFiles(HttpRequestMessage request, string schemaName, string className, string script)
        {
            var result = "";

            // save the file in subdir of temp dir
            string baseDir = GetTempDir();
            var provider = new FileMultipartFormDataStreamProvider(baseDir);

            // create file infos in db and save files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            string localFileName;

            foreach (var file in provider.FileData)
            {
                localFileName = file.LocalFileName;

                PerformImports(GetConnectionString(CONNECTION_STRING, schemaName),
                    className, script, localFileName);
            }

            return result;
        }

        /// <summary>
        /// Import data in a file to a related class to a master instance using the script specified
        /// </summary>
        /// <param name="request">The client request</param>
        /// <param name="schemaName">The database name</param>
        /// <param name="className">The master instance class name</param>
        /// <param name="oid">master instance id</param>
        /// <param name="relatedClass">The related class name</param>
        /// <param name="script">The predefined import script name</param>
        /// <returns>Import status</returns>
        public async Task<string> ImportFiles(HttpRequestMessage request, string schemaName, string className, string oid, string relatedClass, string script)
        {
            string result = "";

            // save the file in subdir of temp dir
            string baseDir = GetTempDir();
            var provider = new FileMultipartFormDataStreamProvider(baseDir);

            // create file infos in db and save files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            string localFileName;

            foreach (var file in provider.FileData)
            {
                localFileName = file.LocalFileName;

                PerformImportsToRelated(GetConnectionString(CONNECTION_STRING, schemaName),
                    className, oid, relatedClass, script, localFileName);
            }

            return result;
        }

        private string PerformImports(string connectionString, string className, string script, string filePath)
        {
            string result = "";
            try
            {
                ImportManager importManager = new ImportManager();

                MappingPackage mappingPackage = importManager.GetMappingPackageByName(connectionString, script);

                if (mappingPackage != null)
                {
                    ImportResult importResult = importManager.ImportFile(connectionString,
                        filePath, mappingPackage);

                    if (importResult.HasError)
                    {
                        if (importResult.Errors.Count > 0)
                        {
                            for (int i = 0; i < importResult.Errors.Count; i++)
                            {
                                if (result.Length > 0)
                                {
                                    result += "\n";
                                }
                                result += importResult.Errors[i].Source + ": " + importResult.Errors[i].Message;
                            }

                            throw new Exception(result);
                        }
                    }
                }
                else
                {
                    throw new Exception("Unable to find an import script with name " + script);
                }
            }
            finally
            {
                // delete the file after importing
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        // TODO: the file is locked by the process, unable to delete. ignore for now
                    }
                }
            }

            return result;
        }

        private string PerformImportsToRelated(string connectionString, string className, string oid, string relatedClass,
            string script, string filePath)
        {
            string result = "";
            try
            {
                ImportManager importManager = new ImportManager();

                MappingPackage mappingPackage = importManager.GetMappingPackageByName(connectionString, script);

                if (mappingPackage != null)
                {
                    using (CMConnection connection = new CMConnection(connectionString))
                    {
                        connection.Open();

                        InstanceView masterInstanceView = GetInstanceView(connection, className, oid);

                        string masterInstancePKValue = masterInstanceView.InstanceData.PrimaryKeyValues;
 
                        string relationshipName = GetManyToOneRelationshipName(connection.MetaDataModel, masterInstanceView, relatedClass);

                        // set relationship value to associate the import data to the master instance
                        foreach (ClassMapping classMapping in mappingPackage.ClassMappings)
                        {
                            if (classMapping.DestinationClassName == relatedClass)
                            {
                                DataViewModel destinationDataView;
                                if (classMapping.DestinationDataView == null)
                                {
                                    destinationDataView = connection.MetaDataModel.GetDetailedDataView(classMapping.DestinationClassName);
                                    classMapping.DestinationDataView = destinationDataView;
                                }

                                // clear the overriding default values if exist
                                if (classMapping.OverridingDefaultValues != null)
                                {
                                    classMapping.OverridingDefaultValues.Clear();
                                }

                                // set the overriding default value 
                                classMapping.OverrideRelationshipDefaultValue(relationshipName, masterInstancePKValue);
                                break;
                            }
                        }

                        ImportResult importResult = importManager.ImportFile(connectionString,
                            filePath, mappingPackage);

                        if (importResult.HasError)
                        {
                            if (importResult.Errors.Count > 0)
                            {
                                for (int i = 0; i < importResult.Errors.Count; i++)
                                {
                                    if (result.Length > 0)
                                    {
                                        result += "\n";
                                    }
                                    result += importResult.Errors[i].Source + ": " + importResult.Errors[i].Message;
                                }

                                throw new Exception(result);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Unable to find an import script with name " + script);
                }
            }
            finally
            {
                // delete the file after importing
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception)
                    {
                        // TODO: the file is locked by the process, unable to delete. ignore for now
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Return a list of ScriptViewModel for the import scripts defined for a class
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private List<ScriptViewModel> GetScriptViews(string schemaName, string className, string fileType)
        {
            List<ScriptViewModel> scriptViews = new List<ScriptViewModel>();

            ScriptViewModel scriptView;

            ImportManager importManager = new ImportManager();

            MappingPackageCollection mappingPackages = importManager.GetMappingPackagesByClass(GetConnectionString(CONNECTION_STRING, schemaName),
                                                             DataSourceType.Unknown,
                                                             className);
            foreach (MappingPackage mappingPackage in mappingPackages)
            {
                if (IsFileTypeMatched(mappingPackage, fileType))
                {
                    scriptView = new ScriptViewModel();

                    scriptView.Name = mappingPackage.Name;
                    scriptView.Description = mappingPackage.Description;

                    scriptViews.Add(scriptView);
                }
            }
  
            return scriptViews;
        }

        /// <summary>
        /// Get the many-to-one relationship name between a related class and a master class
        protected string GetManyToOneRelationshipName(MetaDataModel metaData, InstanceView masterInstance, string relatedClassName)
        {
            string relationshipName = null;
            DataClass relatedDataClass = null;
            foreach (DataClass relatedClass in masterInstance.DataView.BaseClass.RelatedClasses)
            {
                if (relatedClass.ClassName == relatedClassName ||
                    IsParentOf(metaData, relatedClass.ClassName, relatedClassName))
                {
                    relatedDataClass = relatedClass;
                    break;
                }
            }

            if (relatedDataClass != null)
            {
                if (!relatedDataClass.ReferringRelationship.IsForeignKeyRequired)
                {
                    // it is a one-to-many relationship between the master class and related class,
                    relationshipName = relatedDataClass.ReferringRelationship.BackwardRelationshipName;
                }
            }

            return relationshipName;
        }

        private string GetTempDir()
        {
            string fileDir;

            string userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;

            fileDir = WebApiNameSpace.GetTempDir() + @"\" +  userName;

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            return fileDir;
        }

        private bool IsFileTypeMatched(MappingPackage mappingPackage, string fileType)
        {
            bool status = false;

            if (fileType == "All")
            {
                status = true;
            }
            else
            {
                string dataSourceType = Enum.GetName(typeof(DataSourceType), mappingPackage.DataSourceType);

                if (dataSourceType == fileType)
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Get a InstanceView object
        /// </summary>
        /// <param name="connection">DB connection</param>
        /// <param name="className">Class name of the instance view</param>
        /// <param name="oid">ObjId of the instance</param>
        /// <returns>An InstanceView object</returns>
        private InstanceView GetInstanceView(CMConnection connection, string className, string oid)
        {
            InstanceView instanceView;

            MetaDataModel metaData = connection.MetaDataModel;

            DataViewModel dataView = metaData.GetDetailedDataView(className);

            string query = dataView.GetInstanceQuery(oid);

            CMCommand cmd = connection.CreateCommand();
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
                throw new Exception("Unable to find the data instance for id " + oid);
            }

            return instanceView;
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

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}
