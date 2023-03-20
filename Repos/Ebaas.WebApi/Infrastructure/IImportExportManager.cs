using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ebaas.WebApi.Models;

namespace Ebaas.WebApi.Infrastructure
{
    public interface IImportExportManager
    {
        /// <summary>
        /// Get Import Scripts defined for a class
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="fileType">Indicate the file type that scripts can import, values are All, Excel, Text, Other</param>
        /// <returns></returns>
        Task<IEnumerable<ScriptViewModel>> GetScripts(string schemaName, string className, string fileType);

        /// <summary>
        /// Import data in a file to a class using the script specified
        /// </summary>
        /// <param name="request">The client request</param>
        /// <param name="schemaName">The database name</param>
        /// <param name="className">The master instance class name</param>
        /// <param name="script">The predefined import script name</param>
        /// <returns>Import status</returns>
        Task<string> ImportFiles(HttpRequestMessage request, string schemaName, string className, string script);

        /// <summary>
        /// Import data in a file to a related class to a master instance using the script specified
        /// </summary>
        /// <param name="request">The client request</param>
        /// <param name="schemaName">The database name</param>
        /// <param name="className">The master instance class name</param>
        /// <param name="oid">master instance id</param>
        /// <param name="relatedClass">The related class name</param>
        /// <param name="script">The predefined import script name</param>
        /// <returnsImport status</returns>
        Task<string> ImportFiles(HttpRequestMessage request, string schemaName, string className, string oid, string relatedClass, string script);
    }
}
