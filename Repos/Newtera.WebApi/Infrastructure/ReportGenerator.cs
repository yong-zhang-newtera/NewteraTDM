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
using System.Threading;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Newtera.Common.Core;
using Newtera.Data;
using Newtera.WebApi.Models;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.FileType;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.ChartServer;

namespace Newtera.WebApi.Infrastructure
{
    /// <summary>
    /// A class that provides services for generating and downloading an excel report
    /// </summary>
    public class ReportGenerator : ReportGeneratorBase
    {
        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="xmlSchemaName"></param>
        /// <param name="oid"></param>
        /// <param name="templateFile">Report template file name(*.xlsx, *.doc, *.xsl)</param>
        /// <param name="templateProperty">The property whose value is a report template name</param>
        /// <param name="destinationFileName">The file name for the generated report</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GenerateReport(string schemaName, string className, string xmlSchemaName, string oid, string templateFile,
            string templateProperty, string destinationFileName)
        {
            HttpResponseMessage response = null;
            string baseDirAttribute = null;

            await Task.Factory.StartNew(() =>
            {
                response = new HttpResponseMessage();

                // Create an export handler
                string connectionStr = GetConnectionString(CONNECTION_STRING, schemaName);

                using (CMConnection connection = new CMConnection(connectionStr))
                {
                    connection.Open();

                    InstanceView instanceView = GetInstanceView(connection, className, oid);

                    if (string.IsNullOrEmpty(templateFile))
                    {
                        templateFile = instanceView.InstanceData.GetAttributeStringValue(templateProperty);
                        if (templateFile == null)
                        {
                            throw new Exception(templateProperty + " property doesn't contain a template name.");
                        }
                    }

                    string destinationFilePath = null;

                    string templatePath = GetTemplatePath(connection, className, templateFile);

                    ReportFileType reportType = GetFileType(templatePath);

                    if (string.IsNullOrEmpty(destinationFileName))
                    {
                        destinationFileName = GetReportFileName(templateFile, reportType);
                    }
                    else
                    {
                        // replace variables in the destinationFileName with actual values of master instance
                        destinationFileName = ReplaceVariables(destinationFileName, instanceView, reportType);
                    }

                    if (!string.IsNullOrEmpty(baseDirAttribute))
                    {
                        string reportFileDir = GetReportFileDir(instanceView, baseDirAttribute); // get partial report dir
                        reportFileDir = GetReportFilePath(reportFileDir); // make it a complete dir name by adding root dir

                        if (!string.IsNullOrEmpty(reportFileDir) && !Directory.Exists(reportFileDir))
                        {
                            // create directories for the generated report
                            Directory.CreateDirectory(reportFileDir);
                        }


                        // save the generated report to this dir instead of the temp dir
                        destinationFilePath = reportFileDir + destinationFileName;
                    }

                    if (string.IsNullOrEmpty(destinationFilePath))
                    {
                        // save the generated file under temp dir
                        destinationFilePath = GetTempFilePath(destinationFileName);
                    }

                    try
                    {
                        if (File.Exists(destinationFilePath))
                        {
                            File.Delete(destinationFilePath);
                        }

                        // populate the template with data and generate a word file
                        if (!string.IsNullOrEmpty(oid))
                        {
                            if (reportType == ReportFileType.Word)
                            {
                                WordGenerator wordGenerator = new WordGenerator();
                                wordGenerator.Generate(templatePath, destinationFilePath, connectionStr, className, oid);
                            }
                            else if (reportType == ReportFileType.Excel)
                            {
                                // pass the web url to the excel generator for macro to access the files uploaded by the users
                                ExcelGenerator excelGenerator = new ExcelGenerator(GetWebBaseURL() + NewteraNameSpace.USER_FILES_DIR + @"/");
                                excelGenerator.Generate(templatePath, destinationFilePath, connectionStr, className, oid);
                            }
                            else if (reportType == ReportFileType.PDF)
                            {
                                if (string.IsNullOrEmpty(xmlSchemaName))
                                {
                                    throw new Exception("xmlschema parameter is required to generate a pdf report");
                                }

                                // get xml schema name from an attribute if it deosn't exist in the xml schema set
                                XMLSchemaModel xmlSchemaModel = (XMLSchemaModel)connection.MetaDataModel.XMLSchemaViews[xmlSchemaName];
                                if (xmlSchemaModel == null)
                                {
                                    xmlSchemaName = instanceView.InstanceData.GetAttributeStringValue(xmlSchemaName);
                                }

                                PDFGenerator pdfGenerator = new PDFGenerator(GetWebBaseURL() + NewteraNameSpace.USER_FILES_DIR + @"/");
                                pdfGenerator.Generate(templatePath, destinationFilePath, connectionStr, className, xmlSchemaName, oid);
                            }
                            else
                            {
                                throw new Exception("Unknown report type:" + templateFile);
                            }
                        }
                        else
                        {
                            File.Copy(templatePath, destinationFilePath);
                        }

                        // get the file
                        if (File.Exists(destinationFilePath))
                        {
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Content = new StreamContent(new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read));
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            response.Headers.Add("x-filename", HttpUtility.UrlEncode(destinationFileName, Encoding.UTF8));
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = HttpUtility.UrlEncode(destinationFileName, Encoding.UTF8)
                            };

                            // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
                            // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.
                        }
                        else
                        {
                            response.StatusCode = System.Net.HttpStatusCode.Gone;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                        response.StatusCode = System.Net.HttpStatusCode.Gone;
                    }
                }
            });

            return response;
        }
        /// <summary>
        /// Get a file
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="className"></param>
        /// <param name="objIds"></param>
        /// <param name="templateFile">Report template file name</param>
        /// <param name="destinationFileName">The file name for the generated report</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GenerateReport(string schemaName, string className, StringCollection objIds, string templateFile,
            string destinationFileName)
        {
            HttpResponseMessage response = null;

            await Task.Factory.StartNew(() =>
            {
                response = new HttpResponseMessage();

                // Create an export handler
                string connectionStr = GetConnectionString(CONNECTION_STRING, schemaName);

                using (CMConnection connection = new CMConnection(connectionStr))
                {
                    connection.Open();

                    string destinationFilePath = null;

                    if (string.IsNullOrEmpty(destinationFileName))
                    {
                        destinationFileName = templateFile;
                    }

                    // save the generated file under temp dir
                    destinationFilePath = GetTempFilePath(destinationFileName);

                    try
                    {
                        if (File.Exists(destinationFilePath))
                        {
                            File.Delete(destinationFilePath);
                        }

                        string templatePath = GetTemplatePath(connection, className, templateFile);

                        ReportFileType reportType = GetFileType(templatePath);

                        if (reportType == ReportFileType.Excel)
                        {
                            // pass the web url to the excel generator for macro to access the files uploaded by the users
                            ExcelGenerator excelGenerator = new ExcelGenerator(GetWebBaseURL() + NewteraNameSpace.USER_FILES_DIR + @"/");
                            excelGenerator.Generate(templatePath, destinationFilePath, connectionStr, className, objIds, false);
                        }
                        else
                        {
                            throw new Exception("Unknown report type:" + templateFile);
                        }

                        // get the file
                        if (File.Exists(destinationFilePath))
                        {
                            response.StatusCode = System.Net.HttpStatusCode.OK;
                            response.Content = new StreamContent(new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read));
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            response.Headers.Add("x-filename", HttpUtility.UrlEncode(destinationFileName, Encoding.UTF8));
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = HttpUtility.UrlEncode(destinationFileName, Encoding.UTF8)
                            };

                            // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
                            // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.
                        }
                        else
                        {
                            response.StatusCode = System.Net.HttpStatusCode.Gone;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.Instance.WriteLine(ex.GetBaseException().Message + "\n" + ex.GetBaseException().StackTrace);

                        response.StatusCode = System.Net.HttpStatusCode.Gone;
                    }
                }
            });

            return response;
        }

        // get the file type from the file name
        private ReportFileType GetFileType(string filePath)
        {
            ReportFileType fileType = ReportFileType.Unknown;

            string fileSuffix = System.IO.Path.GetExtension(filePath).Trim();

            if (!string.IsNullOrEmpty(fileSuffix))
            {
                switch (fileSuffix)
                {
                    case ".xls":
                    case ".xlsx":
                    case ".XLS":
                    case ".XLSX":
                        fileType = ReportFileType.Excel;
                        break;
                    case ".doc":
                    case ".docx":
                    case ".DOC":
                    case ".DOCX":
                        fileType = ReportFileType.Word;
                        break;

                    case ".xsl":
                    case ".XSL":
                    case ".Xsl":
                        fileType = ReportFileType.PDF;
                        break;

                    default:
                        fileType = ReportFileType.Unknown;
                        break;
                }
            }

            return fileType;
        }
    }


    public enum ReportFileType
    {
        Unknown,
        Excel,
        Word,
        PDF
    }
}