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
    /// Extending MultipartFormDataStreamProvider to create an unique id for a file saved in local disk
    /// </summary>
    public class FileMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public FileMultipartFormDataStreamProvider(string path) : base(path)
        {
        }
 
        /// <summary>
        /// Get the file name
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
            if (!Directory.Exists(this.RootPath))
            {
                // create the subdir
                Directory.CreateDirectory(this.RootPath);
            }

            string filePath = this.RootPath + filename;

            return filePath;                
        }
    }
}