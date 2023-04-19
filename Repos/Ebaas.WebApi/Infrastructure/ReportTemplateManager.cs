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

using Newtera.Common.Core;
using Newtera.Data;
using Ebaas.WebApi.Models;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.FileType;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Report template manager that provides services for gettting, adding, or delete the report templates in a local disk for data classes
    /// </summary>
    public class ReportTemplateManager
    {
        /// <summary>
        /// File manager that provides services for gettting, uploading, downloading, delete the files in a local disk for data instances
        /// </summary>
        public ReportTemplateManager()
        {
        }

        /// <summary>
        /// Get a list of ReportTemplateModel objects representing the report templates associated with a data class
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FileViewModel>> Get(string schemaName, string className)
        {
            List<FileViewModel> files = new List<FileViewModel>();

            await Task.Factory.StartNew(() =>
            {
                files = GetFileViews(schemaName, className);
            });
                                         
            return files;
        }

        /// <summary>
        /// Return a list of FileViewModel for the files associated with an instance
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        private List<FileViewModel> GetFileViews(string schemaName, string className)
        {
            List<FileViewModel> fileViews = new List<FileViewModel>();

            string baseDir = GetFileBaseDir(schemaName, className);

            if (!string.IsNullOrEmpty(baseDir) && Directory.Exists(baseDir))
            {
                FileViewModel fileView;

                DirectoryInfo dirInfo = new DirectoryInfo(baseDir);

                FileInfo[] fileInfos = dirInfo.GetFiles();

                foreach (FileInfo info in fileInfos)
                {
                    fileView = new FileViewModel();

                    FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByType(info.Extension);
                    fileView.ID = info.Name;
                    fileView.Name = info.Name;
                    fileView.Description = fileTypeInfo.Description;
                    fileView.Size = (info.Length / 1024).ToString();
                    fileView.Created = info.CreationTime;
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


        private string GetFileBaseDir(string schemaName, string className)
        {
            string dirPath = NewteraNameSpace.GetReportTemplateDir(schemaName + " 1.0", className);
            return dirPath;
        }
    }
}