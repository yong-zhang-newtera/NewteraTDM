using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.IO;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.Common.Attachment;
using Newtera.Common.MetaData.FileType;
using Ebaas.WebApi.Models;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Base class for file manager implementation 
    /// </summary>
    public abstract class FileManagerBase
    {
        protected const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        public FileManagerBase()
        {

        }

        /// <summary>
        /// Return a list of FileViewModel for the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        protected List<FileViewModel> GetFileViews(string schemaName, string className, string oid, int start, int pageSize)
        {
            using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();
                return GetAttachmentInfos(connection, oid, start, pageSize, AttachmentType.Instance);
            }
        }

        /// <summary>
        /// Return a FileViewModel object for a file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        protected FileViewModel GetFileView(string schemaName, string fileId)
        {
            using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();
                return GetAttachmentInfo(connection, AttachmentType.Instance, fileId);
            }
        }

        /// <summary>
        /// Return count of the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <returns>File count</returns>
        protected int GetFileCount(string schemaName, string className, string oid)
        {
            using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();
                return GetAttachmentsCount(connection, oid, AttachmentType.Instance);
            }
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="schemaName">The scehma name</param>
        /// <param name="fileId"></param>
        public FileViewModel DeleteFile(string schemaName, string fileId)
        {
            using (CMConnection connection = new CMConnection(GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                connection.Open();

                CMCommand cmd = connection.CreateCommand();

                AttachmentInfo info = cmd.GetAttachmentInfo(AttachmentType.Instance, fileId);
                if (info != null)
                {
                    cmd.DeleteAttachment(AttachmentType.Instance, info);
                }
                else
                {
                    throw new FileNotFoundException("File not found");
                }

                return new FileViewModel { ID = info.ID,
                    Name = info.Name,
                    Created = info.CreateTime,
                    Modified = info.ModifiedTime,
                    Size = info.Size.ToString(),
                    Suffix = info.Suffix,
                    Description = info.Description,
                    Type = info.Type,
                    InstanceId = info.ItemId,
                    ClassName = info.ClassName
                };
            }
        }

        /// <summary>
        /// Gets attachment info for an instance or a class
        /// </summary>
        /// <param name="connection">DB Connection</param>
        /// <param name="oid">id of the attachment owner, can be instance or class id</param>
        /// <param name="start">start row</param>
        /// <param name="pageSize"> page size</param>
        /// <param name="attachmentType">Instance or Class</param>
        /// <returns>The List of FileViewModel object</returns>
        private List<FileViewModel> GetAttachmentInfos(CMConnection connection, string oid, int start, int pageSize, AttachmentType attachmentType)
        {
            List<FileViewModel> fileViews = new List<FileViewModel>();

            if (pageSize > 0)
            {
                FileViewModel fileView;

                CMCommand cmd = connection.CreateCommand();
                AttachmentInfoCollection infos = cmd.GetAttachmentInfos(attachmentType, oid, start, pageSize);

                // set the description of each attachment based on its type
                foreach (AttachmentInfo info in infos)
                {
                    fileView = new FileViewModel();

                    FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Type);
                    fileView.ID = info.ID;
                    fileView.Name = info.Name;
                    fileView.Description = fileTypeInfo.Description;
                    fileView.Size = info.Size.ToString();
                    fileView.Created = info.CreateTime;
                    fileView.Modified = info.ModifiedTime;
                    fileView.Type = info.Type;
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
        /// Gets a FileViewModel object
        /// </summary>
        /// <param name="connection">DB Connection</param>
        /// <param name="attachmentType">Instance or Class</param>
        /// <param name="fileId">The file id</param>
        /// <returns>A FileViewModel object</returns>
        private FileViewModel GetAttachmentInfo(CMConnection connection, AttachmentType attachmentType, string fileId)
        {
            FileViewModel fileView = null;


            CMCommand cmd = connection.CreateCommand();

            AttachmentInfo info = cmd.GetAttachmentInfo(attachmentType, fileId);

            if (info != null)
            {
                fileView = new FileViewModel();

                FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Type);
                fileView.ID = info.ID;
                fileView.Name = info.Name;
                fileView.Description = fileTypeInfo.Description;
                fileView.Size = info.Size.ToString();
                fileView.Created = info.CreateTime;
                fileView.Modified = info.ModifiedTime;
                fileView.Type = info.Type;

                if (fileTypeInfo.Suffixes.Count > 0)
                {
                    // take the first suffix as default
                    fileView.Suffix = ((FileSuffix)fileTypeInfo.Suffixes[0]).Suffix;
                }
            }

            return fileView;
        }

        /// <summary>
        /// Get attachment count
        /// </summary>
        /// <param name="connection">The db connection</param>
        /// <param name="oid">id of the attachment owner, can be instance or class id</param>
        /// <param name="attachmentType">Instance or Class</param>
        /// <returns>attchment count</returns>
        private int GetAttachmentsCount(CMConnection connection, string oid, AttachmentType attachmentType)
        {
            int count = 0;

            CMCommand cmd = connection.CreateCommand();
            count = cmd.GetAttachmentInfosCount(attachmentType, oid);

            return count;
        }

        protected string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }
    }
}