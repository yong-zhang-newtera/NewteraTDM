namespace Newtera.WebApi.Utils
{
    using System;
    using System.Xml;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Web.UI.HtmlControls;
    using System.Runtime.Remoting;
    using System.Security.Principal;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using ICSharpCode.SharpZipLib.Checksums;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.GZip;

    using Newtera.Data;
    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.Attachment;
    using Newtera.ChartServer;
    using Newtera.WebApi.Infrastructure;

    /// <summary>
    /// Utility to create a datapackage
    /// </summary>
    public class PackDataPackageUtil
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
        private StringCollection _imageFileNames;
        private StringCollection _formTemplateNames;
        private StringCollection _reportTemplateNames;
        private AttachmentInfoCollection _attachmentInfos;
        private string _schemaId;
        private string _masterClassName;
        private string _masterIds;
        private string _destinationFile;
        private string _formAttribute;
        private string _reportAttribute;
        private string _connectionString;

        private const string META_DATA_DIR = "metaData";
        private const string CLASS_DATA_DIR = "classData";
        private const string ATTACHEMENTS_DIR = "attachments";
        private const string FORM_TEMPLATE_DIR = @"templates\forms";
        private const string REPORT_TEMPLATE_DIR = @"templates\reports";
        private const string DESTINATION_FILE_NAME = "data.pak";

        private const int PAGE_SIZE = 50;

        public PackDataPackageUtil(string connectionString, string destinationFile, string schemaId, string masterClassName, string masterIds, string formAttribute, string reportAttribute)
        {
            _schemaId = schemaId;
            _masterClassName = masterClassName;
            _masterIds = masterIds;
            _destinationFile = destinationFile;
            _formAttribute = formAttribute;
            _reportAttribute = reportAttribute;
            _connectionString = connectionString;

            if (string.IsNullOrEmpty(_destinationFile))
            {
                _destinationFile = DESTINATION_FILE_NAME;
            }

            _imageFileNames = new StringCollection();

            _formTemplateNames = new StringCollection();

            _reportTemplateNames = new StringCollection();

            _attachmentInfos = new AttachmentInfoCollection();
        }

        public async Task<HttpResponseMessage> DownloaddDataPackage()
        {
            HttpResponseMessage response = null;

            await Task.Factory.StartNew(() =>
            {
                response = new HttpResponseMessage();

                string path = CreateDataPackage();

                 // get the file
                if (File.Exists(path))
                {
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read));
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Headers.Add("x-filename", HttpUtility.UrlEncode(_destinationFile, Encoding.UTF8));
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = HttpUtility.UrlEncode(_destinationFile, Encoding.UTF8)
                    };

                    // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
                    // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.Gone;
                }
            });

            return response;
        }

        /// <summary>
        /// Create a data pack file containing data of the master instance and its asscoaited instances
        /// </summary>
        /// <returns></returns>
        public string CreateDataPackage()
        {
            _dataPackageDir = GetDataPackageFileDir(); // create a directory to store datapackage files
            if (!Directory.Exists(_dataPackageDir))
            {
                // create directories for data package  files
                Directory.CreateDirectory(_dataPackageDir);
            }

            _metaDataDir = _dataPackageDir + @"\" + META_DATA_DIR;

            if (!Directory.Exists(_metaDataDir))
            {
                Directory.CreateDirectory(_metaDataDir);
            }
            else
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

            _classDataDir = _dataPackageDir + @"\" + CLASS_DATA_DIR;

            if (!Directory.Exists(_classDataDir))
            {
                Directory.CreateDirectory(_classDataDir);
            }
            else
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

            _attachmentDir = _dataPackageDir + @"\" + ATTACHEMENTS_DIR;

            if (!Directory.Exists(_attachmentDir))
            {
                Directory.CreateDirectory(_attachmentDir);
            }
            else
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

            _formTemplateDir = _dataPackageDir + @"\" + FORM_TEMPLATE_DIR;
            _actualFormTemplateDir = NewteraNameSpace.GetFormTemplateBaseDir(_schemaId);

            if (!Directory.Exists(_formTemplateDir))
            {
                Directory.CreateDirectory(_formTemplateDir);
            }
            else
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

            _reportTemplateDir = _dataPackageDir + @"\" + REPORT_TEMPLATE_DIR;
            _actualReportTemplateDir = NewteraNameSpace.GetReportTemplateBaseDir(_schemaId);

            if (!Directory.Exists(_reportTemplateDir))
            {
                Directory.CreateDirectory(_reportTemplateDir);
            }
            else
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

            string destinationFilePath = null;

            using (CMConnection connection = new CMConnection(_connectionString))
            {
                connection.Open();

                _metaData = connection.MetaDataModel;

                // output the meta data
                WriteMetaData(connection);

                // output class data
                WiteDataPackage(connection);

                // output image files
                WriteImageFiles(connection);

                // output instance attachments
                WriteInstanceAttachments(connection);

                // output form templates
                WriteFormTemplates(connection);

                // output report templates
                WriteReportTemplates(connection);

                destinationFilePath = _dataPackageDir + @"\" + _destinationFile;
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                // create the zipped file
                PackFiles(_dataPackageDir, destinationFilePath);
            }

            return destinationFilePath;
        }

        // Create a context name
        private string CreateContextName(string className, string objId)
        {
            string contextName = "DownloadDataPackage" + className;
            if (!string.IsNullOrEmpty(objId))
            {
                contextName += objId;
            }

            return contextName;
        }

        private string GetDataPackageFileDir()
        {
            string userName = Thread.CurrentPrincipal.Identity.Name;

            string dir = WebApiNameSpace.GetTempDir() + userName;

            return dir;
        }

        /// <summary>
        /// Write the meta data files
        /// </summary>
        private void WriteMetaData(CMConnection connection)
        {
            // write the meta data into files
            string schemaFileName = _metaDataDir + @"\" + connection.MetaDataModel.SchemaInfo.Name + ".schema";

            _metaData.SchemaModel.Write(schemaFileName);

            /*
            string xaclPolicyFileName = schemaFileName.Replace(".schema", ".xacl");
            _metaData.XaclPolicy.Write(xaclPolicyFileName);

            string taxonomyFileName = schemaFileName.Replace(".schema", ".taxonomy");
            _metaData.Taxonomies.Write(taxonomyFileName);
            string dataViewFileName = schemaFileName.Replace(".schema", ".dataview");
            _metaData.DataViews.Write(dataViewFileName);
            string ruleFileName = schemaFileName.Replace(".schema", ".rules");
            _metaData.RuleManager.Write(ruleFileName);
            string mappingFileName = schemaFileName.Replace(".schema", ".mappings");
            _metaData.MappingManager.Write(mappingFileName);
            string selectorFileName = schemaFileName.Replace(".schema", ".selectors");
            _metaData.SelectorManager.Write(selectorFileName);
            string eventFileName = schemaFileName.Replace(".schema", ".events");
            _metaData.EventManager.Write(eventFileName);
            string loggingFileName = schemaFileName.Replace(".schema", ".logging");
            _metaData.LoggingPolicy.Write(loggingFileName);
            string subscriberFileName = schemaFileName.Replace(".schema", ".subscribers");
            _metaData.SubscriberManager.Write(subscriberFileName);
            string xmlSchemaViewsFileName = schemaFileName.Replace(".schema", ".schemaviews");
            _metaData.XMLSchemaViews.Write(xmlSchemaViewsFileName);
             string apiFileName = schemaFileName.Replace(".schema", ".apis");
            _metaData.ApiManager.Write(xmlSchemaViewsFileName);

             */

        }

        /// <summary>
        /// Write the xml data of the selected data instances in the specified classes into files
        /// </summary>
        private void WiteDataPackage(CMConnection connection)
        {
            InstanceView masterInstanceView = WriteMasterInstanceData(connection);

            WriteRelatedInstanceData(connection, masterInstanceView);
        }

        /// <summary>
        /// Write the master instance data to a file
        /// </summary>
        /// <param name="connection"></param>
        private InstanceView WriteMasterInstanceData(CMConnection connection)
        {
            DataViewModel masterDataView = connection.MetaDataModel.GetDetailedDataView(_masterClassName);

            StringCollection instanceIds = new StringCollection();
            string[] idArray = _masterIds.Split(',');
            foreach (string instanceId in idArray)
            {
                instanceIds.Add(instanceId);
            }
            string query = masterDataView.GetInstancesQuery(instanceIds);

            DataSet ds = WriteInstanceData(connection, masterDataView, query);

            return new InstanceView(masterDataView, ds);
        }

        /// <summary>
        /// Write the instance data in the related classes that are owned by the master instance
        /// </summary>
        /// <remarks>This is called recursively</remarks>
        private void WriteRelatedInstanceData(CMConnection connection, InstanceView masterInstanceView)
        {
            InstanceView relatedInstanceView;
            int count = 0;
            ReferencedClassCollection relatedClasses = GetRelatedClasses(connection, masterInstanceView.DataView.BaseClass);
            foreach (DataClass relatedClass in relatedClasses)
            {
                if (relatedClass.ReferringRelationship.IsOwnedRelationship)
                {
                    relatedInstanceView = SaveRelatedInstancesToFile(connection, relatedClass, masterInstanceView, out count);

                    for (int i = 0; i < count; i++)
                    {
                        relatedInstanceView.SelectedIndex = i;

                        // going down the chain
                        WriteRelatedInstanceData(connection, relatedInstanceView);
                    }
                }
            }
        }

        /// <summary>
        /// Get a collection of the DataClass objects that represents the data classes that are
        /// linked to a base data class through the relationship attributes
        /// </summary>
        /// <remarks>Unlike the default behaviouse, for many-to-many relationship, this method will return the junction class as a related class, instead of the class on the another side of junction class.</remarks>
        public ReferencedClassCollection GetRelatedClasses(CMConnection connection, DataClass baseClass)
        {
            ReferencedClassCollection referencedClasses = new ReferencedClassCollection();

            ClassElement currentClassElement = baseClass.GetSchemaModelElement() as ClassElement;
            DataClass relatedClass;
            ClassElement relatedClassElement;
            while (currentClassElement != null)
            {
                foreach (RelationshipAttributeElement relationshipAttribute in currentClassElement.RelationshipAttributes)
                {
                    if (relationshipAttribute.IsBrowsable &&
                        PermissionChecker.Instance.HasPermission(connection.MetaDataModel.XaclPolicy, relationshipAttribute, XaclActionType.Read) &&
                          PermissionChecker.Instance.HasPermission(connection.MetaDataModel.XaclPolicy, relationshipAttribute.LinkedClass, XaclActionType.Read))
                    {
                        relatedClass = new DataClass(relationshipAttribute.LinkedClassAlias,
                            relationshipAttribute.LinkedClassName, DataClassType.ReferencedClass);
                        relatedClass.ReferringClassAlias = baseClass.Alias;
                        relatedClass.ReferringRelationshipName = relationshipAttribute.Name;
                        relatedClass.Caption = relationshipAttribute.LinkedClass.Caption;
                        relatedClass.ReferringRelationship = relationshipAttribute;
                        relatedClassElement = connection.MetaDataModel.SchemaModel.FindClass(relationshipAttribute.LinkedClassName);
                        relatedClass.IsLeafClass = relatedClassElement.IsLeaf;
 
                        referencedClasses.Add(relatedClass);
                    }
                }

                currentClassElement = currentClassElement.ParentClass;
            }

            return referencedClasses;
        }

        /// <summary>
        /// Get the related instance data from DB and save it to a file
        /// </summary>
        /// <returns></returns>
        private InstanceView SaveRelatedInstancesToFile(CMConnection connection, DataClass relatedClass, InstanceView masterInstanceView, out int count)
        {
            InstanceView instanceView;
            DataViewModel dataView = GetRelatedDataView(connection, relatedClass, masterInstanceView);

            // get query
            dataView.PageSize = PAGE_SIZE;

            // clear any sort clauses, since we want to backup the data instances in the order
            // of being created. This way is faster too.
            dataView.ClearSortBy();

            string query = dataView.SearchQuery;

            DataSet ds = WriteInstanceData(connection, dataView, query);
            if (ds != null)
            {
                count = DataSetHelper.GetRowCount(ds, dataView.BaseClass.ClassName);
                instanceView = new InstanceView(dataView, ds);
            }
            else
            {
                count = 0;
                instanceView = new InstanceView(dataView);
            }

            return instanceView;
        }

        private DataViewModel GetRelatedDataView(CMConnection connection, DataClass relatedClass, InstanceView masterInstanceView)
        {
            DataViewModel dataView = connection.MetaDataModel.GetDetailedDataView(relatedClass.ClassName);

            // build a search expression that retrieve the data instances of the related class that
            // are asspciated with the master instance
            string searchValue = null;
            if (relatedClass.ReferringRelationship.IsForeignKeyRequired)
            {
                // it is a many-to-one relationship between the master class and related class,
                // gets the obj_id of the instance for the related class from the master instance view

                if (masterInstanceView != null && masterInstanceView.DataSet != null)
                {
                    DataTable relationshipTable = masterInstanceView.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(masterInstanceView.DataView.BaseClass.ClassName, relatedClass.ReferringRelationship.Name)];
                    if (relationshipTable != null)
                    {
                        if (relationshipTable.Rows[0].IsNull(NewteraNameSpace.OBJ_ID) == false)
                        {
                            searchValue = relationshipTable.Rows[0][NewteraNameSpace.OBJ_ID].ToString();
                        }

                        if (string.IsNullOrEmpty(searchValue))
                        {
                            searchValue = "0"; // default
                        }
                    }
                }
                else
                {
                    searchValue = "0"; // default
                }
            }
            else
            {
                // it is a one-to-many relationship bewtween the master class and related class
                // use the obj_id of the instance expanded in the master class
                searchValue = masterInstanceView.InstanceData.ObjId;
            }

            IDataViewElement expr = null;
            if (!string.IsNullOrEmpty(searchValue))
            {
                SearchExpressionBuilder builder = new SearchExpressionBuilder();
                expr = builder.BuildSearchExprForRelationship(dataView, relatedClass, searchValue);

                // add search expression to the dataview
                dataView.AddSearchExpr(expr, ElementType.And);
            }

            ClassElement relatedClassElement = connection.MetaDataModel.SchemaModel.FindClass(dataView.BaseClass.ClassName);
            if (!relatedClassElement.IsLeaf)
            {
                // get  data view for the related leaf class
                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = dataView.SearchQuery;
                StringCollection leafClasses = cmd.GetClassNames();
                if (leafClasses.Count > 0)
                {
                    // if there are more than one leaf classes, only uses the first one
                    dataView = connection.MetaDataModel.GetDetailedDataView(leafClasses[0]);
                    // add search expression to the dataview
                    dataView.AddSearchExpr(expr, ElementType.And);
                }
            }

            return dataView;
        }

        /// <summary>
        /// Write the instances retrieved by the given query to a file
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="query"></param>
        private DataSet WriteInstanceData(CMConnection connection, DataViewModel dataView, string query)
        {
            DataSet ds = null;
            XmlDocument doc;
            XmlReader xmlReader;
            ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(dataView.BaseClass.ClassName);

            // one result set per file
            string fileName = _classDataDir + @"\" + dataView.BaseClass.ClassName + ".";

            // Create a CMDataReder object for the query
            CMCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;

            //ErrorLog.Instance.WriteLine("query=" + query);
            cmd.PageSize = PAGE_SIZE;
            CMDataReader reader = cmd.ExecuteReader();

            int pageIndex = 0;
            XmlDocument partialDoc;

            // create an XML document to store the query result across multiple pages
            doc = new XmlDocument();

            try
            {
                while (true)
                {
                    if (reader.Read())
                    {
                        partialDoc = reader.GetXmlDocument();

                        if (IsEmptyDataDoc(partialDoc, dataView))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    XmlNode importedNode = doc.ImportNode(partialDoc.DocumentElement, true);
                    if (doc.DocumentElement == null)
                    {
                        doc.AppendChild(importedNode);
                    }
                    else
                    {
                        foreach (XmlNode childNode in importedNode.ChildNodes)
                        {
                            doc.DocumentElement.AppendChild(childNode);
                        }
                    }

                    ds = new DataSet();
                    xmlReader = new XmlNodeReader(partialDoc);
                    ds.ReadXml(xmlReader);

                    // remember the image names so that we can save them up later
                    if (HasImageAttributes(classElement))
                    {
                        KeepImageNames(dataView, ds);
                    }

                    // remember the form template names so that we can save them up later
                    if (HasFormTemplateAttribute(dataView))
                    {
                        KeepFormTemplateNames(dataView, ds);
                    }

                    // remember the report template names so that we can save them up later
                    if (HasReportTemplateAttribute(dataView))
                    {
                        KeepReportTemplateNames(dataView, ds);
                    }

                    // remember the attachment infos
                    KeepAttachmentInfos(connection, dataView, ds);
                }

                // make sure there is no file with the same name exists
                while (true)
                {
                    if (File.Exists(fileName + pageIndex))
                    {
                        pageIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (doc.DocumentElement != null)
                {
                    doc.Save(fileName + pageIndex); // save the xml to a file
                }

                xmlReader = new XmlNodeReader(doc);
                ds = new DataSet();
                ds.ReadXml(xmlReader);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return ds;
        }

        private bool IsEmptyDataDoc(XmlDocument doc, DataViewModel dataView)
        {
            bool status = false;

            XmlReader xmlReader = new XmlNodeReader(doc);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);

            if (DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Write image files
        /// </summary>
        private void WriteImageFiles(CMConnection connection)
        {
            if (_imageFileNames.Count > 0)
            {
                // write the image file names into a file
                string fileName = _attachmentDir + @"\images.xml";
                try
                {
                    //Open the stream and read XSD from it.
                    using (FileStream fs = File.OpenWrite(fileName))
                    {
                        // create xml document to hold the image file names
                        XmlDocument doc = new XmlDocument();
                        XmlElement root = doc.CreateElement("ImageFiles");
                        doc.AppendChild(root);
                        XmlElement element;
                        foreach (string imageId in _imageFileNames)
                        {
                            element = doc.CreateElement("ImageFile");
                            element.InnerText = imageId;
                            root.AppendChild(element);
                        }

                        doc.Save(fs);
                        fs.Flush();
                    }
                }
                catch (System.IO.IOException)
                {
                    throw new AttachmentException("Failed to write the image files" + fileName);
                }

                string imageFile;

                // get the image files and write them to the directory
                // use the unique image id as file name
                foreach (string imageId in _imageFileNames)
                {
                    try
                    {
                        imageFile = _attachmentDir + @"\" + imageId;
                        if (File.Exists(imageFile))
                        {
                            File.Delete(imageFile);
                        }

                        // since the image file is stored in an actual attachment directory which can be changed
                        // by web.config, we need to copy the image to the temp image directory
                        // Note: Unlike attachment files, image files are stored at the base dir
                        string actualAttachmentDir = NewteraNameSpace.GetAttachmentDir();
 
                        if (File.Exists(actualAttachmentDir + imageId))
                        {
                            File.Copy(actualAttachmentDir + imageId, imageFile);
                        }
                    }
                    catch (Exception)
                    {
                        // image may be deleted, ignore the error for now
                    }
                }
            }
        }

        /// <summary>
        /// Write instances attachments to files
        /// </summary>
        private void WriteInstanceAttachments(CMConnection connection)
        {
            if (_attachmentInfos.Count > 0)
            {
                // write the attachment infos into a file
                string fileName = _attachmentDir + @"\attachments.xml";
                this._attachmentInfos.Write(fileName);

                string attachmentFile;
                string actualAttachmentDir = NewteraNameSpace.GetAttachmentDir();
                string attachmentPath;

                // get the attachment files and write them to the directory
                // in order to avoid name conflicts, use the unique attachment id as file name
                foreach (AttachmentInfo info in _attachmentInfos)
                {
                    try
                    {
                        // when packing, put all attachments under one direcotry, instead of under many subdirs
                        attachmentFile = _attachmentDir + @"\" + info.ID;

                        if (File.Exists(attachmentFile))
                        {
                            File.Delete(attachmentFile);
                        }

                        attachmentPath = actualAttachmentDir + info.ID;
                        if (!File.Exists(attachmentPath))
                        {
                            // The attachment file could be saved under a subdirectory (after 5.2.0)
                            attachmentPath = NewteraNameSpace.GetAttachmentSubDir(info.CreateTime) + info.ID;
                        }

                        // copy the attachment file to be packed
                        if (File.Exists(attachmentPath))
                        {
                            File.Copy(attachmentPath, attachmentFile);
                        }
                    }
                    catch (Exception)
                    {
                        // attachment may be deleted, ignore the error for now
                    }
                }
            }
        }

        /// <summary>
        /// Write form template files
        /// </summary>
        private void WriteFormTemplates(CMConnection connection)
        {
            if (this._formTemplateNames.Count > 0)
            {
                string formTemplateFile;

                // get the form template files and write them to the form template directory
                foreach (string formTemplateName in _formTemplateNames)
                {
                    try
                    {
                        formTemplateFile = _formTemplateDir + @"\" + formTemplateName;
                        if (File.Exists(formTemplateFile))
                        {
                            File.Delete(formTemplateFile);
                        }

                        if (File.Exists(_actualFormTemplateDir + formTemplateName))
                        {
                            File.Copy(_actualFormTemplateDir + formTemplateName, formTemplateFile);
                        }
                    }
                    catch (Exception)
                    {
                        // template may be deleted, ignore the error for now
                    }
                }
            }
        }

        /// <summary>
        /// Write report template files
        /// </summary>
        private void WriteReportTemplates(CMConnection connection)
        {
            if (this._reportTemplateNames.Count > 0)
            {
                string reportTemplateFile;

                // get the report template files and write them to the report template directory
                foreach (string reportTemplateName in _reportTemplateNames)
                {
                    try
                    {
                        reportTemplateFile = _reportTemplateDir + @"\" + reportTemplateName;
                        if (File.Exists(reportTemplateFile))
                        {
                            File.Delete(reportTemplateFile);
                        }

                        if (File.Exists(_actualReportTemplateDir + reportTemplateName))
                        {
                            File.Copy(_actualReportTemplateDir + reportTemplateName, reportTemplateFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        // image may be deleted, ignore the error for now
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Pack the files into a zipped file
        /// </summary>
        private void PackFiles(string dataPackageDir, string packFileName)
        {
            ZipOutputStream s = null;

            string workingDir = Directory.GetCurrentDirectory();

            try
            {
                // set dataPackageDir as the current working dir
                Directory.SetCurrentDirectory(dataPackageDir);

                s = new ZipOutputStream(File.Create(packFileName));

                s.SetLevel(6); // 0 - store only to 9 - means best compression

                string[] subDirs = Directory.GetDirectories(".");

                WriteFilesToZip(s, subDirs);
            }
            finally
            {
                Directory.SetCurrentDirectory(workingDir);

                if (s != null)
                {
                    s.Finish();
                    s.Close();
                }
            }
        }

        private void WriteFilesToZip(ZipOutputStream s, string[] subDirs)
        {
            ZipEntry entry;
            FileStream fs = null;
            string[] nestedSubDirs;

            try
            {
                for (int i = 0; i < subDirs.Length; i++)
                {
                    string[] filenames = Directory.GetFiles(subDirs[i]);

                    foreach (string file in filenames)
                    {
                        fs = File.OpenRead(file);

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        entry = new ZipEntry(file);
                        entry.IsUnicodeText = true;

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        fs.Close();
                        fs = null;

                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }

                    nestedSubDirs = Directory.GetDirectories(subDirs[i]);
                    if (nestedSubDirs != null && nestedSubDirs.Length > 0)
                    {
                        WriteFilesToZip(s, nestedSubDirs);
                    }
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        // return true if the class contains image attributes
        private bool HasImageAttributes(ClassElement classElement)
        {
            bool status = false;
            ClassElement currentClassElement = classElement;

            while (currentClassElement != null)
            {
                if (currentClassElement.ImageAttributes != null &&
                    currentClassElement.ImageAttributes.Count > 0)
                {
                    status = true;
                    break;
                }

                currentClassElement = currentClassElement.ParentClass;
            }

            return status;
        }

        // return true if the class contains attribute that keeps the name of form template
        private bool HasFormTemplateAttribute(DataViewModel dataView)
        {
            bool status = false;
            if (!string.IsNullOrEmpty(_formAttribute))
            {
                if (dataView.ResultAttributes[_formAttribute] != null)
                {
                    status = true;
                }
            }

            return status;
        }

        // return true if the class contains attribute that keeps the name of report template
        private bool HasReportTemplateAttribute(DataViewModel dataView)
        {
            bool status = false;

            if (!string.IsNullOrEmpty(_reportAttribute))
            {
                if (dataView.ResultAttributes[_reportAttribute] != null)
                {
                    status = true;
                }
            }

            return status;
        }

        // Keep the images names in a collection
        private void KeepImageNames(DataViewModel dataView, DataSet ds)
        {
            if (ds.Tables[dataView.BaseClass.Name] != null)
            {
                InstanceView instanceView = new InstanceView(dataView, ds);
                int count = ds.Tables[dataView.BaseClass.Name].Rows.Count;
                PropertyDescriptorCollection propertyDescriptors = instanceView.GetProperties(null);
                for (int i = 0; i < count; i++)
                {
                    instanceView.SelectedIndex = i;
                    foreach (InstanceAttributePropertyDescriptor pd in propertyDescriptors)
                    {
                        if (pd.IsImage)
                        {
                            object val = pd.GetValue();
                            if (val != null && ((string)val).Length > 0)
                            {
                                // keep the image id
                                _imageFileNames.Add((string)val);
                            }
                        }
                    }
                }
            }
        }

        // Keep the attachment infos in a collection
        private void KeepAttachmentInfos(CMConnection connection, DataViewModel dataView, DataSet ds)
        {
            if (ds.Tables[dataView.BaseClass.Name] != null)
            {
                InstanceView instanceView = new InstanceView(dataView, ds);
                int count = ds.Tables[dataView.BaseClass.Name].Rows.Count;
                CMCommand cmd = connection.CreateCommand();
                AttachmentInfoCollection infos = null;

                int attachmentCount;
                int startRow = 0;
                int pageSize = 50;
                for (int i = 0; i < count; i++)
                {
                    instanceView.SelectedIndex = i;
                    attachmentCount = cmd.GetAttachmentInfosCount(AttachmentType.Instance, instanceView.InstanceData.ObjId);
                    startRow = 0;
                    while (startRow < attachmentCount)
                    {
                        infos = cmd.GetAttachmentInfos(AttachmentType.Instance, instanceView.InstanceData.ObjId, startRow, pageSize);
                        if (infos != null)
                        {
                            foreach (AttachmentInfo attachInfo in infos)
                            {
                                _attachmentInfos.Add(attachInfo);
                            }
                        }

                        startRow += pageSize;
                    }
                }
            }
        }

        // Keep the form template names in a collection
        private void KeepFormTemplateNames(DataViewModel dataView, DataSet ds)
        {
            string templateName;

            if (ds.Tables[dataView.BaseClass.Name] != null)
            {
                InstanceView instanceView = new InstanceView(dataView, ds);
                int count = ds.Tables[dataView.BaseClass.Name].Rows.Count;
 
                for (int i = 0; i < count; i++)
                {
                    instanceView.SelectedIndex = i;
                    templateName = instanceView.InstanceData.GetAttributeStringValue(_formAttribute);
                    if (!string.IsNullOrEmpty(templateName))
                    {
                        templateName = dataView.BaseClass.ClassName + @"\" + templateName;

                        if (!Directory.Exists(_formTemplateDir + @"\" + dataView.BaseClass.ClassName))
                        {
                            Directory.CreateDirectory(_formTemplateDir + @"\" + dataView.BaseClass.ClassName);
                        }

                        if (!_formTemplateNames.Contains(templateName))
                        {
                            _formTemplateNames.Add(templateName);
                        }
                    }
                }
            }
        }

        // Keep the report template names in a collection
        private void KeepReportTemplateNames(DataViewModel dataView, DataSet ds)
        {
            string templateName;

            if (ds.Tables[dataView.BaseClass.Name] != null)
            {
                InstanceView instanceView = new InstanceView(dataView, ds);
                int count = ds.Tables[dataView.BaseClass.Name].Rows.Count;

                for (int i = 0; i < count; i++)
                {
                    instanceView.SelectedIndex = i;
                    templateName = instanceView.InstanceData.GetAttributeStringValue(_reportAttribute);
                    if (!string.IsNullOrEmpty(templateName))
                    {
                        templateName = dataView.BaseClass.ClassName + @"\" + templateName;

                        if (!Directory.Exists(_reportTemplateDir + @"\" + dataView.BaseClass.ClassName))
                        {
                            Directory.CreateDirectory(_reportTemplateDir + @"\" + dataView.BaseClass.ClassName);
                        }

                        if (!_reportTemplateNames.Contains(templateName))
                        {
                            _reportTemplateNames.Add(templateName);
                        }
                    }
                }
            }
        }
    }
}
