using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Ebaas.WebApi.Models;
using Newtera.Common.Core;

namespace CustomApiSamples
{
    public class GetChartData : IApiFunction
    {
        /// <summary>
        /// Execute an api function
        /// </summary>
        /// <param name="context">The api execution context</param>
        /// <returns>The execution result as a JObject</returns>
        public JObject Execute(ApiExecutionContext context)
        {
            System.Collections.Specialized.NameValueCollection parameters = context.Parameters;

            JObject chartData = new JObject();
            if (parameters != null)
            {
                if (!string.IsNullOrEmpty(parameters["file"]))
                {
                    string baseDir = NewteraNameSpace.GetUserFilesDir();

                    string filePath = baseDir + context.SchemaId + @"\" + context.ClassName + @"\" + parameters["file"];

                    if (File.Exists(filePath))
                    {
                        using (StreamReader r = new StreamReader(filePath))
                        {
                            string json = r.ReadToEnd();
                            chartData = JsonConvert.DeserializeObject<JObject>(json);
                        }
                    }
                    else
                    {
                        throw new Exception("File " + filePath + " doesn't exist");
                    }
                }
                else
                {
                    throw new Exception("URL missing 'file' parameter");
                }
            }

            return chartData;
        }
    }
}
