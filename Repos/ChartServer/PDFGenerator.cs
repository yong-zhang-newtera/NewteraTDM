using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Xml;

using ibex4;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Common.MetaData.Principal;
using Newtera.Data;

namespace Newtera.ChartServer
{
    public class PDFGenerator
    {
        private const int FILE_SIZE_LIMIT = 500000;
        private const string LOG_FILE_SUB_PATH = @"temp\PDFGenerateLog.txt";
        private string _logFilePath = null;

        private static object _generatorLock = new object();

        private string _webBaseUrl;

        public PDFGenerator(string webBaseUrl)
        {
            _webBaseUrl = webBaseUrl;
        }

        /// <summary>
        /// Generates a pdf doc using a xsl template and xml stream.
        /// </summary>
        /// <param name="templateFilePath">path of the xsl template file</param>
        /// <param name="destinationFilePath">path of the generated pdf file</param>
        /// <param name="connectionStr">The database connection string</param>
        /// <param name="xmlSchemaName">The name of a xml schema defined for the class</param>
        /// <param name="baseInstanceId">The base instance id</param>
        /// <param name="baseClassName">The base class name</param>
        public void Generate(string templateFilePath, string destinationFilePath, string connectionStr,
            string baseClassName, string xmlSchemaName, string baseInstanceId)
        {
            StringCollection baseInstanceIds = new StringCollection();
            baseInstanceIds.Add(baseInstanceId);

            Generate(templateFilePath, destinationFilePath, connectionStr, baseClassName, xmlSchemaName, baseInstanceIds);
        }

        /// <summary>
        /// Generates a pdf doc uisng a xsl template and xml data stream
        /// </summary>
        /// <param name="templateFilePath">path of the xsl template file</param>
        /// <param name="destinationFilePath">path of the generated pdf file</param>
        /// <param name="connectionStr">The database connection string</param>
        /// <param name="baseInstanceIds">A collection of base instance ids</param>
        /// <param name="xmlSchemaName">The name of a xml schema defined for the class</param>
        /// <param name="baseClassName">The base class name</param>
        public void Generate(string templateFilePath, string destinationFilePath, string connectionStr,
            string baseClassName, string xmlSchemaName, StringCollection baseInstanceIds)
        {
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }

            string query = null;

            MetaDataModel metaData;
            using (CMConnection connection = new CMConnection(connectionStr))
            {
                connection.Open();

                metaData = connection.MetaDataModel;

                DataViewModel dataView = metaData.GetDetailedDataView(baseClassName);
                query = GetBaseInstancesQuery(dataView, baseInstanceIds);

                XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)metaData.XMLSchemaViews[xmlSchemaName];

                // make sure the xmlSchemaModel is defined for the given base class
                if (xmlSchemaModel != null &&
                    xmlSchemaModel.RootElement.ElementType == baseClassName)
                {
                    CMCommand cmd = connection.CreateCommand();
                    cmd.CommandText = query;

                    XmlDocument xmlDoc = cmd.GenerateXmlDoc(xmlSchemaModel, dataView);


                    FileStream xslStream = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);
                    MemoryStream xmlStream = new MemoryStream();
                    xmlDoc.Save(xmlStream);
                    xmlStream.Flush();
                    xmlStream.Position = 0;

                    FileStream pdfStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);

                    FODocument foDoc = new FODocument();

                    foDoc.Settings.BaseURI_XML = NewteraNameSpace.GetAppHomeDir() + @"\Templates\Reports\" + connection.MetaDataModel.SchemaInfo.NameAndVersion + @"\" + baseClassName + @"\";

                    // set alias for SimSun
                    foDoc.setFontAlias("SimSun", "ËÎÌå");
                    foDoc.setFontAlias("simkai", "¿¬Ìå");
                    foDoc.setFontAlias("simhei", "ºÚÌå");
                    foDoc.setFontAlias("simfang", "·ÂËÎ");

                    using (xslStream)
                    using (pdfStream)
                    {
                        // generate pdf file
                        foDoc.generate(xmlStream, xslStream, pdfStream);
                    }
                }
            }
        }

        private string GetBaseInstancesQuery(DataViewModel dataView, StringCollection baseInstanceIds)
        {
            string query;

            if (baseInstanceIds.Count == 1)
            {
                query = dataView.GetInstanceQuery(baseInstanceIds[0]);
            }
            else
            {
                query = dataView.GetInstancesQuery(baseInstanceIds);
            }

            return query;
        }

        /// <summary>
        /// Write a message to the server log
        /// </summary>
        /// <param name="message">A meesage to be written to the log</param>
        private void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(_logFilePath))
            {
                _logFilePath = CreateLogFile();
            }

            StreamWriter sw = null;

            try
            {
                if (File.Exists(this._logFilePath))
                {
                    FileInfo fileInfo = new FileInfo(this._logFilePath);
                    if (fileInfo.Length > FILE_SIZE_LIMIT)
                    {
                        // log has grown too big, delete it and start a new one
                        File.Delete(this._logFilePath);

                        sw = new StreamWriter(this._logFilePath);
                    }
                    else
                    {
                        // open the log and append the message to the log
                        FileStream fs = new FileStream(this._logFilePath, FileMode.Append, FileAccess.Write);
                        sw = new StreamWriter(fs);
                    }
                }
                else
                {
                    sw = new StreamWriter(this._logFilePath);
                }

                sw.WriteLine("************ Message Begin ***********");
                sw.WriteLine("Timestamp: " + DateTime.Now.ToString());
                sw.WriteLine(message);
                sw.WriteLine("************ Message End *************");
                sw.WriteLine("");
            }
            catch (Exception)
            {
                // ignore the error
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        /// <summary>
		/// Private constructor.
		/// </summary>
		private string CreateLogFile()
        {
            string logFilePath;

            logFilePath = NewteraNameSpace.GetAppHomeDir();
            if (!logFilePath.EndsWith(@"\"))
            {
                logFilePath += @"\";
            }

            logFilePath += LOG_FILE_SUB_PATH;

            return logFilePath;
        }
    }
}
