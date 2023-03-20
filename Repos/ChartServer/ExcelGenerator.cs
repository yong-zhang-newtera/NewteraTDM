using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Missing = System.Reflection.Missing;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.Common.MetaData.Principal;
using Newtera.Data;

namespace Newtera.ChartServer
{
    public class ExcelGenerator
    {
        private const int FILE_SIZE_LIMIT = 500000;
        private const string LOG_FILE_SUB_PATH = @"temp\ExcelGenerateLog.txt";
        private string _logFilePath = null;

        private static object _generatorLock = new object();

        private string _webBaseUrl;

        public ExcelGenerator(string webBaseUrl)
        {
            _webBaseUrl = webBaseUrl;
        }

               /// <summary>
        /// Generates an excel doc by filling an excel template with a base instance view object.
        /// </summary>
        /// <param name="templateFilePath">path of the word template file</param>
        /// <param name="destinationFilePath">path of the generated word file</param>
        /// <param name="connectionStr">The database connection string</param>
        /// <param name="baseInstanceId">The base instance id</param>
        /// <param name="baseClassName">The base class name</param>
        public void Generate(string templateFilePath, string destinationFilePath, string connectionStr,
            string baseClassName, string baseInstanceId)
        {
            StringCollection baseInstanceIds = new StringCollection();
            baseInstanceIds.Add(baseInstanceId);

            Generate(templateFilePath, destinationFilePath, connectionStr, baseClassName, baseInstanceIds, true);
        }

        /// <summary>
        /// Generates an excel doc by filling an excel template with a collection of base instances.
        /// </summary>
        /// <param name="templateFilePath">path of the word template file</param>
        /// <param name="destinationFilePath">path of the generated word file</param>
        /// <param name="connectionStr">The database connection string</param>
        /// <param name="baseInstanceIds">A collection of base instance ids</param>
        /// <param name="baseClassName">The base class name</param>
        /// <param name="blockLoading">Load xml doc in blocks</param>
        public void Generate(string templateFilePath, string destinationFilePath, string connectionStr,
            string baseClassName, StringCollection baseInstanceIds, bool blockLoading)
        {
            Excel.Application application = new Excel.Application();
            application.DisplayAlerts = false; // to prevent open excl hangs when there is an error or alert
            application.ScreenUpdating = false;
            Excel.Workbook workBook = null;
            Excel.Worksheet workSheet = null;
            Excel.PivotTables pivotTables = null;
            Excel.Sheets sheets = null;

            try
            {
                // tell excel not to show itself
                application.Visible = false;

                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }

                // copy the template to destination file so that the template can be used by multiple users
                File.Copy(templateFilePath, destinationFilePath);

                // open the excel template as a workbook
                workBook = application.Workbooks.Open(destinationFilePath,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        true,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value,
                        Missing.Value);

                sheets = workBook.Sheets;

                //WriteLine("开始生成XML数据和加载XML到Excel，包括运行抽取脚本");

                if (workBook.XmlMaps.Count > 0)
                {
                    string query = null;

                    MetaDataModel metaData;
                    using (CMConnection connection = new CMConnection(connectionStr))
                    {
                        connection.Open();

                        metaData = connection.MetaDataModel;

                        DataViewModel dataView = metaData.GetDetailedDataView(baseClassName);
                        query = GetBaseInstancesQuery(dataView, baseInstanceIds);

                        // going through all xml maps in the workbook and load the xml to each matched maps
                        foreach (Excel.XmlMap xmlMap in workBook.XmlMaps)
                        {
                            string xmlSchemaName = xmlMap.Name;

                            XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)metaData.XMLSchemaViews[xmlSchemaName];
   
                            // make sure the xmlSchemaModel is defined for the given base class
                            if (xmlSchemaModel != null &&
                                xmlSchemaModel.RootElement.ElementType == baseClassName)
                            {
                                CMCommand cmd = connection.CreateCommand();
                                cmd.CommandText = query;

                                try
                                {
                                    XmlDocument doc;

                                    if (blockLoading)
                                    {
                                        doc = cmd.BeginCreateDoc(xmlSchemaModel, dataView);
                                    }
                                    else
                                    {
                                        doc = cmd.GenerateXmlDoc(xmlSchemaModel, dataView);
                                    }

                                    StringWriter sw;
                                    XmlTextWriter tx;
                                    string xml;
                                    // convert xml doc into text
                                    sw = new StringWriter();
                                    tx = new XmlTextWriter(sw);
                                    doc.WriteTo(tx);
                                    xml = sw.ToString();

                                    xmlMap.ImportXml(xml, true); // load the firts document with Override to true

                                    if (blockLoading)
                                    {
                                        while ((doc = cmd.CreateNextDoc()) != null)
                                        {
                                            // convert xml doc into text
                                            sw = new StringWriter();
                                            tx = new XmlTextWriter(sw);
                                            doc.WriteTo(tx);
                                            xml = sw.ToString();

                                            xmlMap.ImportXml(xml, false); // The folowing document must set Override to be false, so that the xml elements can be appended
                                        }
                                    }

                                }
                                finally
                                {
                                    if (blockLoading)
                                    {
                                        cmd.EndCreateDoc();
                                    }
                                }
                            }
                        }

                        //WriteLine("完成生成XML数据和加载XML到Excel，包括运行抽取脚本");

                        if (workBook.Sheets.Count > 0)
                        {
                            workSheet = (Excel.Worksheet)workBook.Sheets[1];

                            try
                            {
                                pivotTables = (Excel.PivotTables)workSheet.PivotTables(Missing.Value);
                                int pivotTablesCount = pivotTables.Count;
                                if (pivotTablesCount > 0)
                                {
                                    for (int i = 1; i <= pivotTablesCount; i++)
                                    {
                                        pivotTables.Item(i).RefreshTable(); //The Item method throws an exception
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        bool hasError = false;

                        // Run the macro with base web path

                        //WriteLine("开始运行Excel宏");

                        try
                        {
                            application.Run("SmartExelMacro",
                                             _webBaseUrl,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value,
                                             Missing.Value
                                        );
                        }
                        catch (Exception)
                        {
                            hasError = true;
                        }

                        // Run the macro without base web path
                        if (hasError)
                        {
                            try
                            {
                                application.Run("SmartExelMacro",
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value,
                                                 Missing.Value
                                            );
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                else if (workBook.XmlMaps.Count == 0)
                {
                    //throw new Exception("Excel tempplate located at " + templateFilePath + " doesn't have XML Map defined.");
                    ErrorLog.Instance.WriteLine("Excel tempplate located at " + templateFilePath + " doesn't have XML Map defined.");
                }

                //WriteLine("完成运行Excel宏");

                application.ScreenUpdating = true;

                // save data-populated workbook as an excel file
                workBook.Save();

                /*
                workBook.SaveAs(destinationFilePath,
                    Excel.XlFileFormat.xlWorkbookNormal,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Excel.XlSaveAsAccessMode.xlExclusive,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value,
                    Missing.Value);
                 */

            }
            finally
            {
                if (workBook != null)
                {
                    while (Marshal.ReleaseComObject(workBook) > 0) { }
                    workBook = null;

                    if (sheets != null)
                    {
                        while (Marshal.ReleaseComObject(sheets) > 0) { }
                        sheets = null;
                    }

                    if (workSheet != null)
                    {
                        while (Marshal.ReleaseComObject(workSheet) > 0) { }
                        workSheet = null;
                    }
                }

                if (application != null)
                {
                    try
                    {
                        application.Quit();
                        while (Marshal.ReleaseComObject(application) > 0) { }
                    }
                    catch (Exception)
                    {
                        // when open xlt file, quit may fail when opened by multiple people
                    }

                    application = null;
                    GC();
                }
            }
        }

        public static void GC()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        // Save to the  output file
        private void SaveAs(string outDoc) 
        {   
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