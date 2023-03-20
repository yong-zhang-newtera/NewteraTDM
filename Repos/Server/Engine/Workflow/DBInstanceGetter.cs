/*
* @(#) DBInstanceGetter.cs
*
*/

namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Data;
    using System.Xml;
    using System.Collections;
    using System.Security.Principal;
    using System.Threading;
    using System.Text.RegularExpressions;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.Wrapper;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.Engine.Interpreter;

    /// <summary>
    /// A getter that retrieve a data instance from the local database
    public class DBInstanceGetter : IDataInstanceGetter
	{
        private const string SCHEMA_NAME = "SCHEMA_NAME";
        private const string SCHEMA_VERSION = "SCHEMA_VERSION";

        private Hashtable _properties;

        /// <summary>
        /// Get a data instance given the data type and id from a database
        /// </summary>
        /// <param name="connectionString">the database connection string</param>
        /// <param name="className">A class name to which the instance belongs</param>
        /// <param name="attributeName">A name of the attribute whose value uniquely identifies an instance</param>
        /// <param name="attributeValue"> The attribute's value uniquely indentify the data instance</param>
        /// <returns>A data instance in IInstanceWrapper type, null if not found</returns>
        public IInstanceWrapper GetInstance(string connectionString, string className, string attributeName, string attributeValue)
        {
            InstanceView instanceView = null;

            CMUserManager userMgr = new CMUserManager();
            IPrincipal superUser = userMgr.SuperUser;

            IPrincipal originalPrincipal = Thread.CurrentPrincipal;

            try
            {
                Thread.CurrentPrincipal = superUser;

                _properties = GetProperties(connectionString);

                SchemaInfo schemaInfo = new SchemaInfo();
                schemaInfo.Name = SchemaName;
                schemaInfo.Version = SchemaVersion;

                Newtera.Server.DB.IDataProvider dataProvider = Newtera.Server.DB.DataProviderFactory.Instance.Create();

                MetaDataModel metaData = Newtera.Server.Engine.Cache.MetaDataCache.Instance.GetMetaData(schemaInfo, dataProvider);

                DataViewModel dataView = metaData.GetDetailedDataView(className);
                if (dataView == null)
                {
                    throw new Exception("Unknown class " + className);
                }

                string query = dataView.GetInstanceByAttributeValueQuery(attributeName, attributeValue);

                Interpreter interpreter = new Interpreter();

                XmlDocument doc = interpreter.Query(query);

                XmlNodeReader reader = new XmlNodeReader(doc);

                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                if (!DataSetHelper.IsEmptyDataSet(ds, dataView.BaseClass.ClassName))
                {
                    instanceView = new InstanceView(dataView, ds);

                    return new InstanceWrapper(instanceView);
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                // attach the original principal to the thread
                Thread.CurrentPrincipal = originalPrincipal;
            }
        }

        private Hashtable GetProperties(string connectionString)
        {
            Hashtable properties = new Hashtable();

            // Compile regular expression to find "name = value" pairs
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            if (!string.IsNullOrEmpty(connectionString))
            {
                MatchCollection matches = regex.Matches(connectionString);
                foreach (Match match in matches)
                {
                    int pos = match.Value.IndexOf("=");
                    string key = match.Value.Substring(0, pos).TrimEnd();
                    string val = match.Value.Substring(pos + 1).TrimStart();
                    properties[key] = val;
                }
            }

            return properties;
        }

        public string SchemaName
        {
            get
            {
                return (string)_properties[SCHEMA_NAME]; ;
            }
        }

        /// <summary>
        /// Gets the schema version specified in the connection string
        /// </summary>
        /// <value>The schema version, could be null</value>
        public string SchemaVersion
        {
            get
            {
                return (string)_properties[SCHEMA_VERSION]; ;
            }
        }
    }
}
