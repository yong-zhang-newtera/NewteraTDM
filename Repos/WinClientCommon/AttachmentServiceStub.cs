using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WinClientCommon
{
    public class AttachmentServiceStub : WebApiServiceBase
    {
        public AttachmentServiceStub()
        {
        }

        public int GetAttachmentInfosCount(string connectionStr, string instanceId)
        {
            return 0;
        }

        public string GetAttachmentInfos(string connectionStr, string instanceId, int startRow, int pageSize)
        {
            return "";
        }

        public string AddAttachmentInfo(string connectionStr, string instanceId, string className, string attachmentName, string type, long size, bool isPublic)
        {
            return "";
        }

        public void AddAttachmentInfoWithId(string connectionStr, string instanceId, string attachmentId, string className, string attachmentName, string type, long size, bool isPublic)
        {
        }

        public void SetAttachment(string connectionStr, string attachmentId)
        {
        }

        public void GetAttachment(string connectionStr, string instanceId, string attachmentName)
        {
 
        }

        public void DeleteAttachment(string connectionStr, string instanceId, string attachmentName)
        {
 
        }

        public int GetClassAttachmentInfosCount(string connectionStr, string classId)
        {
            return 0;
        }

        public string GetClassAttachmentInfos(string connectionStr, string classId, int startRow, int pageSize)
        {
            return "";
        }

        public string AddClassAttachmentInfo(string connectionStr, string classId, string className, string attachmentName, string type, long size, bool isPublic)
        {
            return "";
        }

        public void AddClassAttachmentInfoWithId(string connectionStr, string classId, string attachmentId, string className, string attachmentName, string type, long size, bool isPublic)
        {
 
        }

        public void SetClassAttachment(string connectionStr, string attachmentId)
        {

        }

        public void GetClassAttachment(string connectionStr, string classId, string attachmentName)
        {
 
        }

        public void DeleteClassAttachment(string connectionStr, string classId, string attachmentName)
        {

        }

        public string GetFileTypeInfo()
        {
            return "";
        }

        public void GetChartFile(string connectionStr, string formatName, string chartId)
        {
        }

        public string UpdateImageAttributeValue(string connectionStr, string instanceId, string attributeName, string className, string imageFilePath, string type)
        {
            return "";
        }

        public void UploadImage(string connectionStr, string imageId)
        {

        }

        public void GetImage(string connectionStr, string imageId)
        {
 
        }

        public void DeleteImage(string connectionStr, string instanceId, string attributeName, string className, string imageId)
        {

        }


    }
}
