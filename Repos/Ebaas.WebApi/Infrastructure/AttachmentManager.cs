using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Data;
using Ebaas.WebApi.Models;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Logging;
using Newtera.Server.Logging;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Attachmen manager that provides services for get, count, upload, download, and delete attachment(s)
    /// </summary>
    public class AttachmentManager : FileManagerBase, IFileManager
    {
        /// <summary>
        /// Attachmen manager that provides services for get, count, upload, download, and delete attachment(s)
        /// </summary>
        public AttachmentManager()
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
        /// <param name="path">subdirectory path</param>
        /// <returns></returns>
        public async Task<IEnumerable<FileViewModel>> Get(string schemaName, string className, string oid, int start, int pageSize, string path)
        {
            List<FileViewModel> files = new List<FileViewModel>();

            await Task.Factory.StartNew(() =>
            {
                files = GetFileViews(schemaName, className, oid, start, pageSize);
            });
                                         
            return files;
        }

        /// <summary>
        /// Get count of the files associated with a data instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="path">subdirectory path</param>
        /// <returns>The count</returns>
        public async Task<int> Count(string schemaName, string className, string oid, string path)
        {
            int count = 0;

            await Task.Factory.StartNew(() =>
            {
                count = GetFileCount(schemaName, className, oid);
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
        /// <param name="path">subdirectory path</param>
        /// <returns></returns>
        public async Task<FileActionResult> Delete(string schemaName, string className, string oid, string fileId, string path)
        {                         
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    FileViewModel fileView = DeleteFile(schemaName, fileId);

                    // get the attachment file ppath
                    string filePath = NewteraNameSpace.GetAttachmentDir() + fileId;
                    // the attachmen file could be saved in a sub dir (after version 5.2.0)
                    if (!File.Exists(filePath))
                    {
                        filePath = NewteraNameSpace.GetAttachmentSubDir(fileView.Created) + fileId;
                    }

                    // delete the file
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    else
                    {
                        throw new FileNotFoundException("File not found");
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
        /// <param name="path">subdirectory path</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetFile(string schemaName, string className, string oid, string fileId, string path)
        {
            HttpResponseMessage response = null;

            await Task.Factory.StartNew(() =>
            {
                FileViewModel fileView = GetFileView(schemaName, fileId);

                response = new HttpResponseMessage();

                if (fileView != null)
                {
                    // get the attachment file ppath
                    string filePath = NewteraNameSpace.GetAttachmentDir() + fileId;
                    // the attachmen file could be saved in a sub dir (after version 5.2.0)
                    if (!File.Exists(filePath))
                    {
                        filePath = NewteraNameSpace.GetAttachmentSubDir(fileView.Created) + fileId;
                    }

                    // get the file
                    if (File.Exists(filePath))
                    {
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        response.Headers.Add("x-filename", HttpUtility.UrlEncode(fileView.Name, Encoding.UTF8));
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = HttpUtility.UrlEncode(fileView.Name, Encoding.UTF8)
                        };

                        // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
                        // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.

                        // log the event if the particular log is on 
                        using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
                        {
                            connection.Open();
                            ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(className);
                            if (LoggingChecker.Instance.IsLoggingOn(connection.MetaDataModel, classElement, LoggingActionType.Download))
                            {
                                LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Download, connection.MetaDataModel.SchemaInfo.NameAndVersion,
                                    classElement.Name, classElement.Caption, oid, fileView.Name);

                                LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                            }
                        }
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
        /// Add files as attachments to a data instance
        /// </summary>
        /// <param name="request"></param>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="path">subdirectory path</param>
        /// <returns>Added fileViewNodel objects</returns>
        public async Task<IEnumerable<FileViewModel>> Add(HttpRequestMessage request, string schemaName, string className, string oid, string path)
        {
            using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();

                // file root path
                string rootPath = NewteraNameSpace.GetAttachmentDir();

                var provider = new AttachmentMultipartFormDataStreamProvider(connection, className, oid, rootPath);

                // create file infos in db and save files in a local disk
                await request.Content.ReadAsMultipartAsync(provider);

                var files = new List<FileViewModel>();

                string fileId;
                string localFileName;
                StringBuilder buf = new StringBuilder();
                foreach (var file in provider.FileData)
                {
                    localFileName = file.LocalFileName;
                    
                    int pos = localFileName.LastIndexOf(@"\");
                    if (pos > 0)
                    {
                        fileId = localFileName.Substring(pos + 1);
                    }
                    else
                    {
                        fileId = localFileName;
                    }

                    if (buf.Length > 0)
                        buf.Append(";");

                    buf.Append(fileId);
                    
                    FileViewModel fileView = GetFileView(schemaName, fileId);
                    files.Add(fileView);
                }

                // log the event if the particular log is on 
                ClassElement classElement = connection.MetaDataModel.SchemaModel.FindClass(className);
                if (LoggingChecker.Instance.IsLoggingOn(connection.MetaDataModel, classElement, LoggingActionType.Upload))
                {
                    LoggingMessage loggingMessage = new LoggingMessage(LoggingActionType.Upload, connection.MetaDataModel.SchemaInfo.NameAndVersion,
                        classElement.Name, classElement.Caption, oid, buf.ToString());

                    LoggingManager.Instance.AddLoggingMessage(loggingMessage); // queue the message and return right away
                }

                return files;
            }        
        }

        /// <summary>
        /// Get a json object from a file
        /// </summary>
        /// <param name="schemaId"></param>
        /// <param name="file"></param>
        /// <param name="path">subdirectory path</param>
        /// <returns>A Json object read from the file</returns>
        public async Task<JObject> GetJSonData(string schemaId, string file, string path)
        {
            JObject jsonData = new JObject();

            // Not implemented yet

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
            throw new NotImplementedException();
        }
    }
}