﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Xml;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.Http.Description;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Newtera.Data;
using Newtera.Server.DB;
using Newtera.Server.DB.MetaData;
using Newtera.Server.Engine.Cache;
using Newtera.Common.Core;
using Newtera.Common.Config;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Server.UsrMgr;
using Newtera.Common.MetaData.Principal;
using Ebaas.WebApi.Infrastructure;

namespace Ebaas.WebApi.Controllers
{
    /// <summary>
    /// Represents a service that perform system admin related tasks for admin tools
    /// </summary>
    /// <version>  	1.0.0 01 April 2016 </version>
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/adminService")]
    public class AdminServiceController : ApiController
    {
        private const string SCRIPT_DIR = @"\Config\DB\scripts\";
        private const string PATH_LIST_FILE = "patchlist.txt";
        private const string APP_SCHEMA_DIR = @"\Config\Templates\";
        private const string APP_SCHEMA_LIST_FILE = "AppSchemas.xml";
        private const string ORACLE_DIR = @"Oracle\";
        private const string SQLSERVER_DIR = @"SQLServer\";
        private const string SQLSERVERCE_DIR = @"SQLServerCE\";
        private const string MYSQL_DIR = @"MySql\";
        private const string SCHEMA_NAME = "SCHEMA_NAME";
        private const string SCHEMA_VERSION = "SCHEMA_VERSION";
        private const string SERVER_BASE_URL_KEY = "BaseURL";

        private XmlDocument _doc = null;

        /// <summary>
        /// Gets the version of server.
        /// </summary>
        /// <returns>Server version</returns>
        [HttpGet]
        [Route("GetServerVersion")]
        public HttpResponseMessage GetServerVersion()
        {
            try
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(NewteraNameSpace.RELEASE_VERSION, System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a specified data source name is valid or
        /// not
        /// </summary>
        /// <param name="databaseType">The database type string</param>
        /// <param name="dataSourceName">Data Source Name</param>
        /// <returns>True if it is valid data source name, false otherwise</returns>
        [HttpGet]
        [Route("IsDataSourceValid/{databaseType}/{dataSourceName}")]
        public bool IsDataSourceValid(string databaseType, string dataSourceName)
        {
            bool status = true;

            string connectionString = "Data Source=" + dataSourceName;
            DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);

            IDbConnection con = null;

            try
            {
                IDataProvider dataProvider = DataProviderFactory.Instance.Create(type, connectionString);

                con = dataProvider.Connection;
            }
            catch (InvalidDataSourceException ex)
            {
                // Data source is invalid
                status = false;

                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            catch (Newtera.Server.DB.DBException ex)
            {
                // ignore the DBException, Invalid user/password

                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a new SQL Server Database needs to be
        /// created or not for Newtera Enterprise Catalog Manangement
        /// </summary>
        /// <param name="databaseType">The database type string</param>
        /// <param name="dataSourceName">Data Source Name</param>
        /// <returns>True if a database needs to be created, false otherwise</returns>
        [HttpGet]
        [Route("NeedCreateDatabase/{databaseType}/{dataSourceName}")]
        public bool NeedCreateDatabase(string databaseType, string dataSourceName)
        {
            try
            {
                bool status = true;

                DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);
                string connectionString = GetDBConnectionString(type, dataSourceName, DatabaseConfig.Instance.TableSpace);

                IDataProvider dataProvider = DataProviderFactory.Instance.Create(type, connectionString);

                IDbConnection con = null;

                try
                {
                    con = dataProvider.Connection;
                    status = false;
                }
                finally
                {
                    if (con != null)
                    {
                        con.Close();
                    }
                }

                return status;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                return true; // no connection exception indicates that the database has not been created
            }
        }

        /// <summary>
        /// Create a SQL Server Compact Database
        /// </summary>
        /// <param name="databaseType">The database type string</param>
        /// <param name="dataSourceName">Data Source Name</param>
        [HttpGet]
        [Route("CreateDatabase/{databaseType}/{dataSourceName}")]
        public HttpResponseMessage CreateDatabase(string databaseType, string dataSourceName)
        {
            try
            {
                DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);
                string connectionString = GetDBConnectionString(type, dataSourceName);

                IDataProvider dataProvider = DataProviderFactory.Instance.Create(type, connectionString);

                dataProvider.CreateDataBase();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                return resp;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a special tablespace needs to be
        /// created or not for Newtera Enterprise Catalog Manangement
        /// </summary>
        /// <param name="databaseType">The database type string</param>
        /// <param name="dataSourceName">Data Source Name</param>
        /// <returns>True if a tablespace needs to be created, false otherwise</returns>
        [HttpGet]
        [Route("NeedCreateTablespace/{databaseType}/{dataSourceName}")]
        public bool NeedCreateTablespace(string databaseType, string dataSourceName)
        {
            bool status = true;
            string sql = null;
            IDbConnection con = null;
            IDataReader reader = null;

            try
            {
                string connectionString = "Data Source=" + dataSourceName + ";User ID=" + DatabaseConfig.Instance.DBUserID + ";Password=" + DatabaseConfig.Instance.DBUserPassword;
                DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);

                IDataProvider dataProvider = DataProviderFactory.Instance.Create(type, connectionString);

 
                con = dataProvider.Connection;
                IDbCommand cmd = con.CreateCommand();
                sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetTableSpace");

                sql = sql.Replace(":tablespace_name", "'" + DatabaseConfig.Instance.TableSpace.ToUpper() + "'");
                cmd.CommandText = sql;

                /* this method of replacing parameter deosn't work  in some situation
				parameter = cmd.CreateParameter();
				parameter.ParameterName = "tablespace_name";
				parameter.DbType = DbType.AnsiString;
				parameter.Value = DatabaseConfig.Instance.TableSpace.ToUpper();
				cmd.Parameters.Add(parameter);
                 */

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine("NeedCreateTablespace api connect database error: " + ex.Message + " \n" + ex.StackTrace);
                if (sql != null)
                {
                    ErrorLog.Instance.WriteLine("NeedCreateTablespace web method running sql is: " + sql);
                }

                status = true;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                if (con != null)
                {
                    con.Close();
                }
            }

            return status;
        }


        /// <summary>
        /// Create a special tablespace for Newtera Enterprise Catalog Manangement using
        /// an user account with DBA privilege.
        /// </summary>
        /// <param name="databaseType">The database type string.</param>
        /// <param name="dataSourceName">The data source Name.</param>
        [HttpPost]
        [Route("CreateTablespace/{databaseType}/{dataSourceName}/{userName}/{password}")]
        public HttpResponseMessage CreateTablespace(string databaseType, string dataSourceName)
        {
            string content = Request.Content.ReadAsStringAsync().Result;
            string[] apiParams = JsonConvert.DeserializeObject<string[]>(content);
            string userName = apiParams[0];
            string password = apiParams[1];
            string dataFileDir = apiParams[2];

            DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);
            string connectionString = this.GetDBConnectionString(type, dataSourceName, null, userName, password);
            IDataProvider dataProvider = DataProviderFactory.Instance.Create(type, connectionString);

            IDbConnection con = null;

            try
            {
                con = dataProvider.Connection;
                IDbCommand cmd = con.CreateCommand();

                IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(dataProvider);

                string sql = generator.GetCreateTablespaceDDL(DatabaseConfig.Instance.TableSpace, dataFileDir);
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();

                // Create a DBA user for the specified tablespace
                string[] ddls = generator.GetCreateUserDDLs(DatabaseConfig.Instance.DBUserID, DatabaseConfig.Instance.DBUserPassword, DatabaseConfig.Instance.TableSpace);

                foreach (string ddl in ddls)
                {
                    cmd.CommandText = ddl;
                    cmd.ExecuteNonQuery();
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine("CreateTablespace api connect database error: " + ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// Create or update a DB schema
        /// </summary>
        /// <param name="databaseType">The database type string.</param>
        /// <param name="dataSourceName">The data source Name.</param>
        [HttpPost]
        [Route("UpdateSchema/{databaseType}/{dataSourceName}")]
        public HttpResponseMessage UpdateSchema(string databaseType, string dataSourceName)
        {
            DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);
            string connectionString = this.GetDBConnectionString(type, dataSourceName, DatabaseConfig.Instance.TableSpace);

            IDataProvider dataProvider = DataProviderFactory.Instance.Create(type, connectionString);

            IDbConnection con = null;

            try
            {
                con = dataProvider.Connection;
                IDbCommand cmd = con.CreateCommand();

                // Get a list of script files that contains DDLs to be applied
                // to the Schema
                StringCollection scriptFiles = GetScriptFiles(dataProvider, cmd);

                foreach (string scriptFileName in scriptFiles)
                {
                    // execute DDLs in each script file
                    ExecuteScriptFile(dataProvider, scriptFileName, cmd);
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        /// <summary>
        /// Update the server config file with new appsettings.
        /// </summary>
        /// <param name="databaseType">The database type string.</param>
        /// <param name="dataSourceName">The data source Name.</param>
        [HttpPost]
        [Route("UpdateServerConfig/{databaseType}/{dataSourceName}")]
        public HttpResponseMessage UpdateServerConfig(string databaseType, string dataSourceName)
        {
            try
            {
                string content = Request.Content.ReadAsStringAsync().Result;
                string[] apiParams = JsonConvert.DeserializeObject<string[]>(content);
                string imageBaseURL = apiParams[0];
                string imageBasePath = apiParams[1];

                bool updated = false;
                DatabaseType type = DatabaseConfig.Instance.GetDatabaseType(databaseType);
                string connectionString = GetDBConnectionString(type, dataSourceName, DatabaseConfig.Instance.TableSpace);

                AppConfig config = new AppConfig();

                if (config.GetAppSetting(DatabaseConfig.Instance.DatabaseTypeKey) != databaseType)
                {
                    config.SetAppSetting(DatabaseConfig.Instance.DatabaseTypeKey, databaseType);
                    updated = true;
                }

                if (config.GetAppSetting(DatabaseConfig.Instance.DatabaseStringKey) != connectionString)
                {
                    config.SetAppSetting(DatabaseConfig.Instance.DatabaseStringKey, connectionString);
                    updated = true;
                }

                if (config.GetAppSetting(DatabaseConfig.Instance.ImageBaseURLKey) != imageBaseURL)
                {
                    config.SetAppSetting(DatabaseConfig.Instance.ImageBaseURLKey, imageBaseURL);
                    updated = true;
                }

                if (config.GetAppSetting(DatabaseConfig.Instance.ImageBasePathKey) != imageBasePath)
                {
                    config.SetAppSetting(DatabaseConfig.Instance.ImageBasePathKey, imageBasePath);
                    updated = true;
                }

                if (updated)
                {
                    // write to the config file
                    config.Flush();
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Get application schema list in a xml string
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetAppSchemaList")]
        public HttpResponseMessage GetAppSchemaList()
        {
            try
            {
                XmlDocument doc = GetSchemaListDocument();

                StringBuilder builder = new StringBuilder();
                StringWriter writer = new StringWriter(builder);
                doc.Save(writer);
                writer.Flush();

                string xml = builder.ToString();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(xml))
                {
                    resp.Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml");
                }
                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Gets home directory where Newtera Server is installed.
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetAppHomeDir")]
        public HttpResponseMessage GetAppHomeDir()
        {
            try
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(NewteraNameSpace.GetAppHomeDir(), System.Text.Encoding.UTF8, "text/plain");
                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Setup an application schemas.
        /// </summary>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetupAppSchemas")]
        public HttpResponseMessage SetupAppSchemas()
        {
            try
            {
                string[] schemaNames;

                try
                {
                    string content = Request.Content.ReadAsStringAsync().Result;

                    schemaNames = JsonConvert.DeserializeObject<string[]>(content);
                }
                catch (Exception ex)
                {
                    schemaNames = new string[] { };
                }

                foreach (string schemaName in schemaNames)
                {
                    string connectionStr = GetCMConnectionString(schemaName);
                    if (connectionStr == null)
                    {
                        throw new CMException("Failed to find info for " + schemaName);
                    }

                    using (CMConnection con = new CMConnection(connectionStr))
                    {
                        con.Open();

                        try
                        {
                            string fileName = NewteraNameSpace.GetAppHomeDir() + APP_SCHEMA_DIR + schemaName + ".schema";

                            // read the meta data files
                            MetaDataModel metaData = new MetaDataModel();
                            metaData.SchemaModel.Read(fileName);
                            fileName = fileName.Replace(".schema", ".dataview");
                            metaData.DataViews.Read(fileName);
                            fileName = fileName.Replace(".dataview", ".taxonomy");
                            metaData.Taxonomies.Read(fileName);
                            fileName = fileName.Replace(".taxonomy", ".xacl");
                            metaData.XaclPolicy.Read(fileName);
                            fileName = fileName.Replace(".xacl", ".rules");
                            metaData.RuleManager.Read(fileName);
                            fileName = fileName.Replace(".rules", ".mappings");
                            metaData.MappingManager.Read(fileName);
                            fileName = fileName.Replace(".mappings", ".selectors");
                            if (File.Exists(fileName))
                            {
                                metaData.SelectorManager.Read(fileName);
                            }
                            fileName = fileName.Replace(".selectors", ".events");
                            if (File.Exists(fileName))
                            {
                                metaData.EventManager.Read(fileName);
                            }
                            fileName = fileName.Replace(".events", ".logging");
                            if (File.Exists(fileName))
                            {
                                metaData.LoggingPolicy.Read(fileName);
                            }

                            fileName = fileName.Replace(".logging", ".subscribers");
                            if (File.Exists(fileName))
                            {
                                metaData.SubscriberManager.Read(fileName);
                            }

                            fileName = fileName.Replace(".subscribers", ".schemaviews");
                            if (File.Exists(fileName))
                            {
                                metaData.XMLSchemaViews.Read(fileName);
                            }

                            fileName = fileName.Replace(".schemaviews", ".apis");
                            if (File.Exists(fileName))
                            {
                                metaData.ApiManager.Read(fileName);
                            }

                            // Set the modified time of the schema to the connection so that it can update the
                            // existing schema which has a modified time earlier
                            con.SchemaInfo.ModifiedTime = metaData.SchemaModel.SchemaInfo.ModifiedTime;

                            // write them to database
                            StringBuilder builder = new StringBuilder();
                            StringWriter writer = new StringWriter(builder);
                            metaData.SchemaModel.Write(writer);
                            con.UpdateMetaData(MetaDataType.Schema, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.DataViews.Write(writer);
                            con.UpdateMetaData(MetaDataType.DataViews, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.XaclPolicy.Write(writer);
                            con.UpdateMetaData(MetaDataType.XaclPolicy, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.Taxonomies.Write(writer);
                            con.UpdateMetaData(MetaDataType.Taxonomies, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.RuleManager.Write(writer);
                            con.UpdateMetaData(MetaDataType.Rules, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.MappingManager.Write(writer);
                            con.UpdateMetaData(MetaDataType.Mappings, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.SelectorManager.Write(writer);
                            con.UpdateMetaData(MetaDataType.Selectors, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.EventManager.Write(writer);
                            con.UpdateMetaData(MetaDataType.Events, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.LoggingPolicy.Write(writer);
                            con.UpdateMetaData(MetaDataType.LoggingPolicy, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.SubscriberManager.Write(writer);
                            con.UpdateMetaData(MetaDataType.Subscribers, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.XMLSchemaViews.Write(writer);
                            con.UpdateMetaData(MetaDataType.XMLSchemaViews, builder.ToString());

                            builder = new StringBuilder();
                            writer = new StringWriter(builder);
                            metaData.ApiManager.Write(writer);
                            con.UpdateMetaData(MetaDataType.Apis, builder.ToString());
                        }
                        catch (Exception ex)
                        {
                            // ignore the error so that we can update the next schema
                        }
                    }
                }

                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Get server error log text
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetServerLogText")]
        public HttpResponseMessage GetServerLogText()
        {
            try
            {
                string log = ErrorLog.Instance.GetLogText();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(log))
                {
                    resp.Content = new StringContent(log, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Clear server error log
        /// </summary>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ClearServerLog")]
        public void ClearServerLog()
        {
            ErrorLog.Instance.ClearLog();
        }

        /// <summary>
        /// Get server error log text
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetServerTraceLog")]
        public HttpResponseMessage GetServerTraceLog()
        {
            try
            {
                string log = TraceLog.Instance.GetLogText();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                if (!string.IsNullOrEmpty(log))
                {
                    resp.Content = new StringContent(log, System.Text.Encoding.UTF8, "text/plain");
                }
                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Clear server error log
        /// </summary>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("ClearTraceLog")]
        public void ClearTraceLog()
        {
            TraceLog.Instance.ClearLog();
        }

        /// <summary>
        /// Get server url from the sever config file
        /// </summary>
        [HttpGet]
        [AdminAuthorizeAttribute]
        [Route("GetServerUrl")]
        public HttpResponseMessage GetServerUrl()
        {
            try
            {
                AppConfig config = new AppConfig();

                string serverUrl = config.GetAppSetting(SERVER_BASE_URL_KEY);

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(serverUrl, System.Text.Encoding.UTF8, "text/plain");
               
                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// Set a server url to the server config file
        /// </summary>
        [HttpPost]
        [AdminAuthorizeAttribute]
        [Route("SetServerUrl")]
        public HttpResponseMessage SetServerUrl()
        {
            try
            {
                string serverUrl = Request.Content.ReadAsStringAsync().Result;

                AppConfig config = new AppConfig();
                config.SetAppSetting(SERVER_BASE_URL_KEY, serverUrl);
                // write to the config file
                config.Flush();

                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                return resp;
            }
            catch (Exception ex)
            {
                // the specified table space does not exists
                ErrorLog.Instance.WriteLine(ex.Message + " \n" + ex.StackTrace);

                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        /// <summary>
        /// A collection of script file names that contains DDLs to be applied to
        /// the current DB schema.
        /// </summary>
        /// <param name="dataProvider">The data provider</param>
        /// <param name="cmd">IDbCommand object</param>
        /// <returns></returns>
        private StringCollection GetScriptFiles(IDataProvider dataProvider, IDbCommand cmd)
        {
            StringCollection scriptFiles = new StringCollection();
            IDataReader reader = null;
            String subDir = "";

            switch (dataProvider.DatabaseType)
            {
                case DatabaseType.Oracle:
                    subDir = ORACLE_DIR;
                    break;
                case DatabaseType.SQLServer:
                    subDir = SQLSERVER_DIR;
                    break;
                case DatabaseType.SQLServerCE:
                    subDir = SQLSERVERCE_DIR;
                    break;
                case DatabaseType.MySql:
                    subDir = MYSQL_DIR;
                    break;
            }

            try
            {
                // reader all script file names defined in patchlist.txt
                string patchListFile = NewteraNameSpace.GetAppHomeDir() + SCRIPT_DIR + subDir + PATH_LIST_FILE;
                using (StreamReader sr = new StreamReader(patchListFile))
                {
                    String line;
                    // Read lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        scriptFiles.Add(line.Trim());
                    }
                }

                try
                {
                    // remove those script file names that have already executed
                    string sql = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("GetPatchNames");

                    cmd.CommandText = sql;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string scriptFileName = reader.GetString(0);
                        if (scriptFiles.Contains(scriptFileName))
                        {
                            // remove it from the collection
                            scriptFiles.Remove(scriptFileName);
                        }
                    }
                }
                catch (Exception)
                {
                    // This is the first time installation, ignore it
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return scriptFiles;
        }

        /// <summary>
        /// Execute the DDLs contains in a given script file against the DB
        /// Schema.
        /// </summary>
        /// <param name="dataProvider">The data provider</param>
        /// <param name="scriptFileName">The script file name</param>
        /// <param name="cmd">IDbCommand</param>
        private void ExecuteScriptFile(IDataProvider dataProvider, string scriptFileName, IDbCommand cmd)
        {
            string currentSql = "";
            try
            {
                String subDir = "";
                String prefix = "";
                switch (dataProvider.DatabaseType)
                {
                    case DatabaseType.Oracle:
                        subDir = ORACLE_DIR;
                        prefix = ":";
                        break;
                    case DatabaseType.SQLServer:
                        subDir = SQLSERVER_DIR;
                        prefix = "@";
                        break;
                    case DatabaseType.SQLServerCE:
                        subDir = SQLSERVERCE_DIR;
                        prefix = "@";
                        break;
                    case DatabaseType.MySql:
                        subDir = MYSQL_DIR;
                        prefix = "@";
                        break;
                }

                string scriptFilePath = NewteraNameSpace.GetAppHomeDir() + SCRIPT_DIR + subDir + scriptFileName;

                StringBuilder builder = new StringBuilder();
                using (StreamReader sr = new StreamReader(scriptFilePath))
                {
                    String line;
                    // Read lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        builder.Append(line.Trim()).Append(" ");
                    }
                }

                string scriptContent = builder.ToString();

                /* 
                 * splits a script content that consists of DDL and DML SQL statements
                 * separated by ';'
                 */
                string[] sqls = null;

                if (scriptContent.Length > 0)
                {
                    Regex exp = new Regex(";");

                    sqls = exp.Split(scriptContent);
                }

                // execute sqls one by one
                foreach (string sql in sqls)
                {
                    if (sql.Trim().Length > 0)
                    {
                        currentSql = sql;
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }

                // insert name of the script file into a special table in database
                // to remember that this script file has been executed
                // remove those script file names that have already executed
                string dml = CannedSQLManager.GetCannedSQLManager(dataProvider).GetSql("AddScriptFileName");

                dml = dml.Replace(prefix + "script_name", "'" + scriptFileName + "'");

                cmd.CommandText = dml;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine("script file = " + scriptFileName + ";\n" + " sql=" + currentSql + @"\n\n" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Get a connection string for the given schema in Newtera domain
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <returns></returns>
        private string GetCMConnectionString(string schemaName)
        {
            if (_doc == null)
            {
                _doc = GetSchemaListDocument();
            }

            string connectionString = null;

            foreach (XmlNode node in _doc.DocumentElement.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement element = (XmlElement)node;
                    if (element.GetAttribute("Name") == schemaName)
                    {
                        string version = "1.0";
                        if (element.GetAttribute("Version") != null)
                        {
                            version = element.GetAttribute("Version");
                        }

                        connectionString = SCHEMA_NAME + "=" + schemaName + ";" +
                            SCHEMA_VERSION + "=" + version;

                        break;
                    }
                }
            }

            return connectionString;
        }

        /// <summary>
        /// Get an XmlDocument containing a list of application schema definitions
        /// </summary>
        /// <returns>An XmlDocument</returns>
        private XmlDocument GetSchemaListDocument()
        {
            XmlDocument doc = new XmlDocument();

            string schemaListFile = NewteraNameSpace.GetAppHomeDir() + APP_SCHEMA_DIR + APP_SCHEMA_LIST_FILE;

            doc.Load(schemaListFile);

            return doc;
        }

        private string GetDBConnectionString(DatabaseType databaseType, string dataSource, string schema = null, string userId = null, string userPassword = null)
        {
            string connectionString = null;

            if (databaseType == DatabaseType.SQLServer)
            {
                connectionString += ";Initial Catalog=" + DatabaseConfig.Instance.TableSpace;
            }
            else if (databaseType == DatabaseType.SQLServerCE)
            {
                string dbFileName = NewteraNameSpace.GetAppHomeDir() + @"Database\" + dataSource;

                connectionString = "Data Source=" + dbFileName + ";LCID=1033; Case Sensitive = TRUE";
            }
            else if (databaseType == DatabaseType.MySql)
            {
                var uid = userId ?? DatabaseConfig.Instance.DBUserID;
                var pwd = userPassword ?? DatabaseConfig.Instance.DBUserPassword;
                if (!string.IsNullOrEmpty(schema))
                {
                    connectionString = $"Server={dataSource};Database={schema};charset=utf8mb4;uid={uid};pwd={pwd};;Allow User Variables=True";
                }
                else
                {
                    connectionString = $"Server={dataSource};charset=utf8mb4;uid={uid};pwd={pwd};;Allow User Variables=True";
                }
            }

            return connectionString;
        }
    }
}