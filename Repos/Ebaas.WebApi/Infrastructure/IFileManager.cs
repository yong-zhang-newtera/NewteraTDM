using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Ebaas.WebApi.Models;

namespace Ebaas.WebApi.Infrastructure
{
    public interface IFileManager
    {
        Task<IEnumerable<FileViewModel>> Get(string schemaName, string className, string oid, int start, int pageSize, string path);
        Task<int> Count(string schemaName, string className, string oid, string path);
        Task<FileActionResult> Delete(string schemaName, string className, string oid, string fileId, string path);
        Task<IEnumerable<FileViewModel>> Add(HttpRequestMessage request, string schemaName, string className, string oid, string path);
        Task<HttpResponseMessage> GetFile(string schemaName, string className, string oid, string fileId, string path);

        Task<JObject> GetJSonData(string schemaName, string file, string path);

        Task<DirectoryViewModel> GetDirectoryTree(string schemaName, string className, string oid);
    }
}
