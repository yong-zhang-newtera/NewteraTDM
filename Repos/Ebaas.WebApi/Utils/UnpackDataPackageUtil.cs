using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Drawing;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web.SessionState;
using System.Web.UI;
using System.Threading;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

using ICSharpCode.SharpZipLib.Zip;

using Newtera.Data;
using Newtera.Common.MetaData.Logging;
using Newtera.Server.Logging;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.Attachment;
using Newtera.Server.Engine.Cache;
using Newtera.Server.DB;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Utils
{
    /// <summary>
    /// Utility that unpacks a data package and restore data in the package to the database
    /// </summary>
    public class UnpackDataPackageUtil
    {
        private MetaDataModel _metaData;
        private string _dataPackageDir;
        private string _metaDataDir;
        private string _classDataDir;
        private string _attachmentDir;
        private string _formTemplateDir;
        private string _actualFormTemplateDir;
        private string _reportTemplateDir;
        private string _actualReportTemplateDir;
        private InstanceLoader _instanceLoader;
        private AttachmentLoader _attachmentLoader;
        private string _connectionString;
        private string _schemaId;

        private bool _isOverride;

        private const string META_DATA_DIR = "metaData";
        private const string CLASS_DATA_DIR = "classData";
        private const string ATTACHEMENTS_DIR = "attachments";
        private const string FORM_TEMPLATE_DIR = @"templates\forms";
        private const string REPORT_TEMPLATE_DIR = @"templates\reports";

        public UnpackDataPackageUtil(string connectionString, string schemaId, bool isOverride)
        {
            _isOverride = isOverride;
            _schemaId = schemaId;
            _connectionString = connectionString;

            _dataPackageDir = GetDataPackageFileDir(); // create a directory to store datapackage files
            if (!Directory.Exists(_dataPackageDir))
            {
                // create directories for data package  files
                Directory.CreateDirectory(_dataPackageDir);
            }
            else
            {
                // delete old files
                string[] arrFiles = Directory.GetFiles(_dataPackageDir);
                foreach (string tempFile in arrFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            _metaDataDir = _dataPackageDir + @"\" + META_DATA_DIR;

            _classDataDir = _dataPackageDir + @"\" + CLASS_DATA_DIR;

            _attachmentDir = _dataPackageDir + @"\" + ATTACHEMENTS_DIR;

            _formTemplateDir = _dataPackageDir + @"\" + FORM_TEMPLATE_DIR;

            _actualFormTemplateDir = NewteraNameSpace.GetFormTemplateBaseDir(_schemaId);

            _reportTemplateDir = _dataPackageDir + @"\" + REPORT_TEMPLATE_DIR;

            _actualReportTemplateDir = NewteraNameSpace.GetReportTemplateBaseDir(_schemaId);

        }

        /// <summary>
        /// Import a data package file
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Import status</returns>
        public async Task<string> ImportDataPackage(HttpRequestMessage request)
        {
            var result = "";

            // save the file in subdir of temp dir
            var provider = new FileMultipartFormDataStreamProvider(_dataPackageDir);

            // create file infos in db and save files in a local disk
            await request.Content.ReadAsMultipartAsync(provider);

            string localFileName;

            foreach (var file in provider.FileData)
            {
                localFileName = file.LocalFileName;

                RestoreDataPackaged(localFileName);
            }

            return result;
        }

        public void RestoreDataPackaged(string dataPackageFilePath)
        {
            PrepareForImport();

            string workingDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(_dataPackageDir);

                UnpackDataPackage(dataPackageFilePath);
            }
            finally
            {
                // restore current working dir
                Directory.SetCurrentDirectory(workingDir);
            }
        }

        private void PrepareForImport()
        {
            if (Directory.Exists(_metaDataDir))
            {
                // delete old files
                string[] arrFiles = Directory.GetFiles(_metaDataDir);
                foreach (string tempFile in arrFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (Directory.Exists(_classDataDir))
            {
                // delete old files
                string[] arrFiles = Directory.GetFiles(_classDataDir);
                foreach (string tempFile in arrFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (Directory.Exists(_attachmentDir))
            {
                // delete old files
                string[] arrFiles = Directory.GetFiles(_attachmentDir);
                foreach (string tempFile in arrFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (Directory.Exists(_formTemplateDir))
            {
                // delete old files
                string[] arrFiles = Directory.GetFiles(_formTemplateDir);
                foreach (string tempFile in arrFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (Directory.Exists(_reportTemplateDir))
            {
                // delete old files
                string[] arrFiles = Directory.GetFiles(_reportTemplateDir);
                foreach (string tempFile in arrFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void UnpackDataPackage(string dataPackageFilePath)
        {
            // unpack data package into separate files
            UnpackFiles(dataPackageFilePath);

            _metaData = ReadMetaData();

            using (CMConnection connection = new CMConnection(_connectionString))
            {
                connection.Open();

                bool isExist = false;
                SchemaInfo[] schemaInfos = connection.AllSchemas;
                for (int i = 0; i < schemaInfos.Length; i++)
                {
                    if (schemaInfos[i].Name.ToUpper() == _metaData.SchemaInfo.Name.ToUpper() &&
                        schemaInfos[i].Version == _metaData.SchemaInfo.Version)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist)
                {
                    // modify the database according to the meta data
                    if (_isOverride)
                    {
                        ModifySchema(connection);
                    }

                    // Load instances into the database
                    LoadInstances(connection);

                    // Establish relationships among the instances
                    UpdateRelationships();

                    // upload the images
                    LoadImages(connection);

                    // upload instance attachments
                    LoadInstanceAttachements(connection);

                    // upload input form templates 
                    LoadFormTemplates(connection);

                    // upload word report templates 
                    LoadWordTemplates(connection);
                }
            }
        }

        private string GetDataPackageFileDir()
        {
            string userName = Thread.CurrentPrincipal.Identity.Name;

            string dir = EbaasNameSpace.GetTempDir() + userName;

            return dir;
        }

        /// <summary>
        /// Unpack the files into a directory
        /// </summary>
        private void UnpackFiles(string dataPackageFilePath)
        {
            FileStream streamWriter = null;
            ZipInputStream s = null;
            ZipEntry theEntry;

            try
            {
                s = new ZipInputStream(File.OpenRead(dataPackageFilePath));

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        streamWriter = File.Create(theEntry.Name);

                        int size = 1024;
                        byte[] data = new byte[1024];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                        streamWriter = null;
                    }
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }

                if (s != null)
                {
                    s.Close();
                }
            }
        }

        /// <summary>
        /// Read meta data from the files
        /// </summary>
        /// <returns></returns>
        private MetaDataModel ReadMetaData()
        {
            string[] fileNames = Directory.GetFiles(_metaDataDir);

            MetaDataModel metaData = new MetaDataModel();

            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(".schema"))
                {
                    metaData.SchemaModel.Read(fileName);
                }
                else if (fileName.EndsWith(".dataview"))
                {
                    metaData.DataViews.Read(fileName);
                }
                else if (fileName.EndsWith(".taxonomy"))
                {
                    metaData.Taxonomies.Read(fileName);
                }
                else if (fileName.EndsWith(".xacl"))
                {
                    metaData.XaclPolicy.Read(fileName);
                }
                else if (fileName.EndsWith(".rules"))
                {
                    metaData.RuleManager.Read(fileName);
                }
                else if (fileName.EndsWith(".mappings"))
                {
                    metaData.MappingManager.Read(fileName);
                }
                else if (fileName.EndsWith(".selectors"))
                {
                    metaData.SelectorManager.Read(fileName);
                }
                else if (fileName.EndsWith(".events"))
                {
                    metaData.EventManager.Read(fileName);
                }
                else if (fileName.EndsWith(".logging"))
                {
                    metaData.LoggingPolicy.Read(fileName);
                }
                else if (fileName.EndsWith(".subscribers"))
                {
                    metaData.SubscriberManager.Read(fileName);
                }
                else if (fileName.EndsWith(".schemaviews"))
                {
                    metaData.XMLSchemaViews.Read(fileName);
                }
                else if (fileName.EndsWith(".apis"))
                {
                    metaData.ApiManager.Read(fileName);
                }
            }

            // clean up any post-data left in the saved schema
            CleanupSchemaVisitor visitor = new CleanupSchemaVisitor();
            metaData.SchemaModel.Accept(visitor);

            return metaData;
        }

        /// <summary>
        /// Create the database according to the meta data
        /// </summary>
        private void ModifySchema(CMConnection connection)
        {
            // Since we only allow modify the classes in the schema that have data instances in the data package
            // therefore, mark the bottom classes in the meta-data
            StringCollection bottomClasses = GetInstanceClassNames();
            ClassElement classElement;

            foreach (string bottomClass in bottomClasses)
            {
                classElement = _metaData.SchemaModel.FindClass(bottomClass);
                if (classElement != null)
                {
                    classElement.NeedToAlter = true;

                }
            }

            IDataProvider dataProvider = DataProviderFactory.Instance.Create();

            MetaDataCache.Instance.UpdateSchemaIncremental(_metaData, connection.MetaDataModel.SchemaInfo,
                dataProvider, true);

            // Get updated meta data
            _metaData = connection.MetaDataModel;
        }

        // get classes for the instances included in the data package
        private StringCollection GetInstanceClassNames()
        {
            string[] filenames = Directory.GetFiles(_classDataDir);
            StringCollection classNames = new StringCollection();
            string className;
                
            foreach (string file in filenames)
            {
                className = file;
                int pos = className.LastIndexOf(@"\");
                if (pos > 0 && pos < className.Length)
                {
                    className = file.Substring(pos + 1);
                }

                pos = className.IndexOf(".");
                if (pos > 0)
                {
                    className = className.Substring(0, pos);
                }

                if (!classNames.Contains(className))
                {
                    classNames.Add(className);
                }
            }

            return classNames;
        }

        /// <summary>
        /// Load the instnaces into the database
        /// </summary>
        private void LoadInstances(CMConnection connection)
        {
            _instanceLoader = new InstanceLoader(connection, connection.MetaDataModel, _classDataDir, _isOverride);
            _instanceLoader.ObjIdMappings.Clear();

            _instanceLoader.LoadInstances(); // load the instances
        }

        /// <summary>
        /// Establish the relationships among instances
        /// </summary>
        private void UpdateRelationships()
        {
            _instanceLoader.UpdateRelationships();
        }

        /// <summary>
        /// Upload images to the database
        /// </summary>
        private void LoadImages(CMConnection connection)
        {
            _attachmentLoader = new AttachmentLoader(connection, connection.MetaDataModel, _attachmentDir, _instanceLoader.ObjIdMappings);

            _attachmentLoader.LoadImages(); // load the images
        }

        /// <summary>
        /// Upload instace attachments to the database
        /// </summary>
        private void LoadInstanceAttachements(CMConnection connection)
        {
            _attachmentLoader.LoadInstanceAttachments(); // load the instance attachments
        }

        /// <summary>
        /// Upload input form templates
        /// </summary>
        private void LoadFormTemplates(CMConnection connection)
        {
            if (Directory.Exists(_formTemplateDir))
            {
                string[] subDirs = Directory.GetDirectories(_formTemplateDir);
                string targetFormDir;
                string className, templateName;
                int pos;
                foreach (string subDir in subDirs)
                {
                    pos = subDir.LastIndexOf(@"\");
                    if (pos > 0)
                    {
                        className = subDir.Substring(pos + 1);
                        targetFormDir = _actualFormTemplateDir + @"\" + className;

                        if (!Directory.Exists(targetFormDir))
                        {
                            // create a new dir
                            Directory.CreateDirectory(targetFormDir);
                        }

                        string[] formTemplates = Directory.GetFiles(subDir);

                        foreach (string formTemplate in formTemplates)
                        {
                            pos = formTemplate.LastIndexOf(@"\");
                            if (pos > 0)
                            {
                                templateName = formTemplate.Substring(pos + 1);
                                if (!File.Exists(targetFormDir + @"\" + templateName))
                                {
                                    // copy the form template to the target dir
                                    File.Copy(formTemplate, targetFormDir + @"\" + templateName);
                                }
                                else if (_isOverride)
                                {
                                    File.Delete(targetFormDir + @"\" + templateName);

                                    // copy the form template to the target dir
                                    File.Copy(formTemplate, targetFormDir + @"\" + templateName);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Upload word templates
        /// </summary>
        private void LoadWordTemplates(CMConnection connection)
        {
            if (Directory.Exists(_reportTemplateDir))
            {
                string[] subDirs = Directory.GetDirectories(_reportTemplateDir);
                string targetTemplateDir;
                string className, templateName;
                int pos;
                foreach (string subDir in subDirs)
                {
                    pos = subDir.LastIndexOf(@"\");
                    if (pos > 0)
                    {
                        className = subDir.Substring(pos + 1);
                        targetTemplateDir = _actualReportTemplateDir + @"\" + className;

                        if (!Directory.Exists(targetTemplateDir))
                        {
                            // create a new dir
                            Directory.CreateDirectory(targetTemplateDir);
                        }

                        string[] reportTemplates = Directory.GetFiles(subDir);

                        foreach (string reportTemplate in reportTemplates)
                        {
                            pos = reportTemplate.LastIndexOf(@"\");
                            if (pos > 0)
                            {
                                templateName = reportTemplate.Substring(pos + 1);
                                if (!File.Exists(targetTemplateDir + @"\" + templateName))
                                {
                                    // copy the word template to the target dir
                                    File.Copy(reportTemplate, targetTemplateDir + @"\" + templateName);
                                }
                                else if (_isOverride)
                                {
                                    File.Delete(targetTemplateDir + @"\" + templateName);

                                    // copy the word template to the target dir
                                    File.Copy(reportTemplate, targetTemplateDir + @"\" + templateName);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
