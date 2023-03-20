using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.IO;

using Newtera.Data;
using Newtera.Common.Core;
using Newtera.Common.Attachment;
using Newtera.Common.MetaData.FileType;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Extending MultipartFormDataStreamProvider to create an unique id for a file saved as an attachment
    /// </summary>
    public class AttachmentMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        private CMConnection _connection;
        private string _className;
        private string _oid;

        public AttachmentMultipartFormDataStreamProvider(CMConnection connection, string className, string oid, string path) : base(path)   
        {
            _connection = connection;
            _className = className;
            _oid = oid;
        }
 
        /// <summary>
        /// Create an unique id as the file name
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            //Make the file name URL safe and then use it & is the only disallowed url character allowed in a windows filename
            var filename = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            filename = filename.Trim(new char[] { '"' }).Replace("&", "and");
            // remove the path
            int pos = filename.LastIndexOf(@"\");
            if (pos > 0)
            {
                filename = filename.Substring(pos + 1);
            }

            CMCommand cmd = _connection.CreateCommand();

            // if the attachment info already exist, do not create one, just replace the file
            AttachmentInfo info = cmd.GetAttachmentInfo(AttachmentType.Instance, _oid, filename);
            string fileId;

            if (info == null)
            {
                info = new AttachmentInfo();
                info.ItemId = _oid;
                info.ClassName = _className;
                info.Name = filename;

                FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByName(filename);
                info.Type = fileTypeInfo.Type;
                info.Size = 2000;
                info.IsPublic = true;

                fileId = cmd.AddAttachmentInfo(AttachmentType.Instance, info);

                // retrive the attachment info to get the create time
                info = cmd.GetAttachmentInfo(AttachmentType.Instance, fileId);
            }
            else
            {
                fileId = info.ID;
            }

            return GetFilePath(info.CreateTime, fileId);                   
        }

        private string GetFilePath(DateTime createTime, string fileId)
        {
            string baseDir = NewteraNameSpace.GetAttachmentDir();

            string subDir = createTime.Year.ToString() + "-" + createTime.Month.ToString() + @"\";

            string dirPath = baseDir + subDir;

            if (!Directory.Exists(dirPath))
            {
                // create the subdir
                Directory.CreateDirectory(dirPath);
            }

            return subDir + fileId;
        }
    }
}