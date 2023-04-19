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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Data;
using Ebaas.WebApi.Models;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.FileType;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// File manager that provides services for gettting, uploading, downloading, delete the files in a local disk for data instances
    /// </summary>
    public class LocalFileManager : IFileManager
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private const string FILE_BASE_DIR_PROPERTY = "fileBaseDir";

        /// <summary>
        /// File manager that provides services for gettting, uploading, downloading, delete the files in a local disk for data instances
        /// </summary>
        public LocalFileManager()
        {
        }

        /// <summary>
        /// Get a list of FileViewModel objects representing the files associated with a data instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <param name="path">The subdirectory path</param>
        /// <returns></returns>
        public async Task<IEnumerable<FileViewModel>> Get(string schemaName, string className, string oid, int start, int pageSize, string path)
        {
            List<FileViewModel> files = new List<FileViewModel>();

            await Task.Factory.StartNew(() =>
            {
                files = GetFileViews(schemaName, className, oid, path);
            });
                                         
            return files;
        }

        /// <summary>
        /// Get count of the files associated with a data instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <returns>The count</returns>
        public async Task<int> Count(string schemaName, string className, string oid, string path)
        {
            int count = 0;

            await Task.Factory.StartNew(() =>
            {
                count = GetFileCount(schemaName, className, oid, path);
            });

            return count;
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<FileActionResult> Delete(string schemaName, string className, string oid, string fileId, string path)
        {                         
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    // get the file path
                    string fileDir = GetFileDir(schemaName, className, oid, path);
                    if (!string.IsNullOrEmpty(fileDir))
                    {
                        string filePath = fileDir + fileId;

                        // delete the file
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        else
                        {
                            throw new FileNotFoundException("File not found");
                        }
                    }
                });

                return new FileActionResult { Successful = true, Message = "File deleted successfully" };
            }
            catch (FileNotFoundException)
            {
                return new FileActionResult { Successful = false, NotFound=true, Message = "File not found" };
            }
            catch (Exception ex)
            {
                return new FileActionResult { Successful = false, Message = "error deleting file " + ex.GetBaseException().Message};
            }            
        }

        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetFile(string schemaName, string className, string oid, string fileId, string path)
        {
            HttpResponseMessage response = null;

            await Task.Factory.StartNew(() =>
            {
                response = new HttpResponseMessage();

                // get the file path
                string fileDir = GetFileDir(schemaName, className, oid, path);
                if (!string.IsNullOrEmpty(fileDir))
                {
                    // get the file absolute path
                    string filePath = fileDir + fileId;
                   
                    // get the file
                    if (File.Exists(filePath))
                    {
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Headers.Add("x-filename", HttpUtility.UrlEncode(fileId, Encoding.UTF8));
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = HttpUtility.UrlEncode(fileId, Encoding.UTF8)
                        };

                        // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
                        // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.
                    }
                    else
                    {
                        response.StatusCode = System.Net.HttpStatusCode.Gone;
                    }
                }
                else
                {
                    response.StatusCode = System.Net.HttpStatusCode.Gone;
                }
            });

            return response;
        }

        /// <summary>
        /// Add files  to a data instance
        /// </summary>
        /// <param name="request"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="path"></param>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<IEnumerable<FileViewModel>> Add(HttpRequestMessage request, string schemaName, string className, string oid, string path)
        {
            var files = new List<FileViewModel>();

            using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();

                // file root path
                string fileDir = GetFileDir(schemaName, className, oid, path);

                if (string.IsNullOrEmpty(fileDir))
                {
                    throw new Exception("There isn't a property with name " + FILE_BASE_DIR_PROPERTY + " in the class " + className + " or the property doesn't return a directory path");
                }

                if (!Directory.Exists(fileDir))
                {
                    // create the directory
                    Directory.CreateDirectory(fileDir);
                }

                if (!string.IsNullOrEmpty(fileDir))
                {
                    var provider = new FileMultipartFormDataStreamProvider(fileDir);

                    // create file infos in db and save files in a local disk
                    await request.Content.ReadAsMultipartAsync(provider);

                    string localFileName;

                    foreach (var file in provider.FileData)
                    {
                        localFileName = file.LocalFileName;
                        FileInfo info = new FileInfo(localFileName);

                        FileViewModel fileView = new FileViewModel();

                        FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                        fileView.ID = info.Name;
                        fileView.Name = info.Name;
                        fileView.Description = fileTypeInfo.Description;
                        fileView.Size = (info.Length / 1024).ToString();
                        fileView.Created = info.CreationTime;
                        fileView.Modified = info.LastWriteTime;
                        fileView.Type = info.Extension;
                        if (fileTypeInfo.Suffixes.Count > 0)
                        {
                            // take the first suffix as default
                            fileView.Suffix = ((FileSuffix)fileTypeInfo.Suffixes[0]).Suffix;
                        }

                        files.Add(fileView);
                    }
                }

                return files;
            }        
        }

        /// <summary>
        /// Get a json object from a file
        /// </summary>
        /// <param name="schemaId"></param>
        /// <param name="file"></param>
        /// <returns>A Json object read from the file</returns>
        public async Task<JObject> GetJSonData(string schemaId, string file, string path)
        {
            JObject jsonData = new JObject();

            await Task.Factory.StartNew(() =>
            {
                string baseDir = NewteraNameSpace.GetUserFilesDir();

                string localFileName = baseDir + schemaId + @"\" + file;
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.EndsWith(@"\"))
                    {
                        localFileName = baseDir + schemaId + @"\" + path + @"\" + file;
                    }
                    else
                    {
                        localFileName = baseDir + schemaId + @"\" + path + file;
                    }
                }
                else
                {
                    localFileName = baseDir + schemaId + @"\" + file;
                }

                if (File.Exists(localFileName))
                {
                    using (StreamReader r = new StreamReader(localFileName))
                    {
                        string json = r.ReadToEnd();
                        jsonData = JsonConvert.DeserializeObject<JObject>(json);
                    }
                }
                else
                {
                    throw new Exception("File " + localFileName + " doesn't exist");
                }
            });

            return jsonData;
        }

        /// <summary>
        /// Get directory tree for the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public async Task<DirectoryViewModel> GetDirectoryTree(string schemaName, string className, string oid)
        {
            DirectoryViewModel tree = null;

            await Task.Factory.StartNew(() =>
            {
                tree = GetFileDirectory(schemaName, className, oid);
            });

            return tree;
        }

        /// <summary>
        /// Return a list of FileViewModel for the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<FileViewModel> GetFileViews(string schemaName, string className, string oid, string path)
        {
            List<FileViewModel> fileViews = new List<FileViewModel>();

            string fileDir = GetFileDir(schemaName, className, oid, path);

            if (string.IsNullOrEmpty(fileDir))
            {
                return fileViews;
            }

            if (!Directory.Exists(fileDir))
            {
                // create the directory
                Directory.CreateDirectory(fileDir);
            }

            FileViewModel fileView;

            DirectoryInfo dirInfo = new DirectoryInfo(fileDir);

            FileInfo[] fileInfos = dirInfo.GetFiles();

            foreach (FileInfo info in fileInfos)
            {
                if (!IsSystemFile(info))
                {
                    fileView = new FileViewModel();

                    FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                    fileView.ID = info.Name;
                    fileView.Name = info.Name;
                    fileView.Description = fileTypeInfo.Description;
                    fileView.Size = (info.Length / 1024).ToString();
                    fileView.Created = info.CreationTime;
                    fileView.Modified = info.LastWriteTime;
                    fileView.Type = info.Extension;
                    if (fileTypeInfo.Suffixes.Count > 0)
                    {
                        // take the first suffix as default
                        fileView.Suffix = ((FileSuffix)fileTypeInfo.Suffixes[0]).Suffix;
                    }

                    fileViews.Add(fileView);
                }
            }

            return fileViews;
        }

        /// <summary>
        /// Return directory tree for the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        private DirectoryViewModel GetFileDirectory(string schemaName, string className, string oid)
        {
            DirectoryViewModel tree = null;

            string baseDir = GetFileBaseDir(schemaName, className, oid);

            if (string.IsNullOrEmpty(baseDir))
            {
                throw new Exception("There isn't a property with name " + FILE_BASE_DIR_PROPERTY + " in the class " + className + " or the property doesn't return a directory path");
            }

            if (!Directory.Exists(baseDir))
            {
                // create the directory
                Directory.CreateDirectory(baseDir);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(baseDir);

            tree = new DirectoryViewModel(dirInfo.Name);

            AddSubdirs(tree, dirInfo);

            return tree;
        }

        /// <summary>
        /// Return count of the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private int GetFileCount(string schemaName, string className, string oid, string path)
        {
            int count = 0;

            string fileDir = GetFileDir(schemaName, className, oid, path);

            if (!string.IsNullOrEmpty(fileDir) &&
                Directory.Exists(fileDir))
            {
                FileViewModel fileView;

                DirectoryInfo dirInfo = new DirectoryInfo(fileDir);

                FileInfo[] fileInfos = dirInfo.GetFiles();

                count = fileInfos.Length;
            }

            return count;
        }

        private string GetFileDir(string schemaName, string className, string oid, string path)
        {
            string baseDir = GetFileBaseDir(schemaName, className, oid);
            string fileDir = baseDir;

            if (!string.IsNullOrEmpty(baseDir))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    fileDir += path;
                    if (!fileDir.EndsWith(@"\"))
                    {
                        fileDir += @"\";
                    }
                }
            }

            return fileDir;
        }

        private string GetFileBaseDir(string schemaName, string className, string oid)
        {
            string baseDir = null;

            using (CMConnection con = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                DataViewModel dataView = con.MetaDataModel.GetDetailedDataView(className);

                // create an instance query
                string query = dataView.GetInstanceQuery(oid);

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

        private void AddSubdirs(DirectoryViewModel parent, DirectoryInfo parentDirInfo)
        {
            System.IO.DirectoryInfo[] subDirs = parentDirInfo.GetDirectories();

            DirectoryViewModel subDirModel;

            foreach (DirectoryInfo subDirInfo in subDirs)
            {
                subDirModel = new DirectoryViewModel(subDirInfo.Name);

                parent.Subdirs.Add(subDirModel);

                AddSubdirs(subDirModel, subDirInfo);
            }
        }

        private string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }

        private bool IsSystemFile(FileInfo fi)
        {
            return false;
        }
    }
}