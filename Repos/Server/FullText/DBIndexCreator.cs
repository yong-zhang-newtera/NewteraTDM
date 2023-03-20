/*
* @(#) DBIndexCreator.cs
*
* Copyright (c) 2003 - 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.FullText
{
    using System;
    using System.Xml;
    using System.Text;
    using System.Collections.Specialized;
    using System.IO;
    using System.Data;
    using System.Threading;

    using Newtera.Common.Core;
    using Newtera.Server.DB;
    using Newtera.Server.DB.MetaData;
    using Newtera.Common.MetaData.Schema;

    /// <summary> 
    /// An utility that creates DB full-text indexes for attributes in those classes that have full-text search capabilities.
    /// <version>  	1.0.0 14 Apr 2104 </version>
    public class DBIndexCreator
    {
        /// <summary>
        /// Initiate an instance of DBIndexCreator class
        /// </summary>
        public DBIndexCreator()
        {
        }


        /// <summary>
        /// Create database full-text index using native database connection
        /// </summary>
        public void CreateFullTextIndex(IDataProvider dataProvider, SimpleAttributeElement attribute)
        {
            IDbConnection con = dataProvider.Connection;

            IDbCommand cmd = con.CreateCommand();

            IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(dataProvider);

            try
            {
                string indexName = DBNameComposer.GetIndexName(attribute.OwnerClass, attribute, true);

                string ddl = generator.GetCreateFullTextIndexDDL(indexName, attribute.OwnerClass.TableName, attribute.ColumnName);

                if (ddl != null)
                {
                    cmd.CommandText = ddl;

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                con.Close();
            }
        }
    }
}