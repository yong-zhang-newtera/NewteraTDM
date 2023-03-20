/*
* @(#) SequenceNumberGenerator.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Data;
    using System.Xml;
    using System.Threading;
    using System.Security.Principal;

    using Newtera.Common.Core;
    using Newtera.Server.UsrMgr;

	/// <summary>
	/// The purpose of this class is to support generating an
	/// sequnce numeric value for a appliaction in a distributed environment.
	/// It generates an unique integer and return it an Integer 
    /// 
	/// The database is named by SchemaInfo.The class in database is named ClassName, the column of the table is
	/// named by SequenceNumberColumn and is a integer type by default. The instance
    /// in the class is identified with KeyName and KeyValue
	///
	/// <version> 	1.0.0	03 Aug 2011 </version>
	public class SequenceNumberGenerator
	{
        private const string GetSequenceNumberQuery = "for $i in document(\"db://{db}.xml?VERSION={version}\")/{class}List/{class} where $i/{key_name} = \"{key_value}\" return <{class}> {$i/{attribute_name}}</{class}>";
        private const string UpdateSequenceNumberQuery = "for $i in document(\"db://{db}.xml?VERSION={version}\")/{class}List/{class} where $i/{key_name} = \"{key_value}\" return (setText($i/{attribute_name}, \"{attribute_value}\"), updateInstance(document(\"db://{db}.xml?VERSION={version}\"), $i))";

        private IPrincipal _superUser = null;
        private SchemaInfo _schemaInfo;
        private string _className;
        private string _keyName;
        private string _keyValue;
        private string _columnName;

        /// <summary>
        /// Instantiate a default instance
        /// </summary>
        public SequenceNumberGenerator()
        {
            _schemaInfo = null;
            _className = null;
            _columnName = null;
            _keyName = null;
            _keyValue = null;
        }

		/// <summary>
		/// Instantiate an instance of SequenceNumberGenerator class.
		/// </summary>
        /// <param name="schemaInfo">schema information.</param>
        /// <param name="className">A name of the class where the sequence number is stored</param>
        /// <param name="keyName">A name of the column which contains a key identifying the instance where the sequence number is stored</param>
        /// <param name="keyValue">The key value which identifies the instance that contain the sequence number</param>
        /// <param name="sequenceNumberColumn">A name of the column which stores the sequence number</param>
        public SequenceNumberGenerator(SchemaInfo schemaInfo, string className, string keyName, string keyValue, string sequenceNumberColumn)
		{
            _schemaInfo = schemaInfo;
            _className = className;
            _columnName = sequenceNumberColumn;
            _keyName = keyName;
            _keyValue = keyValue;

            CMUserManager userMgr = new CMUserManager();
            _superUser = userMgr.SuperUser;
		}

        /// <summary>
        /// Get current number.
        /// </summary>
        /// <returns>A integer instance</returns>
        public int CurrentNumber()
        {
            int number;

            if (_schemaInfo == null)
            {
                return 0;
            }

            // Must synchronize access to NextNumber of multiple threads
            lock (this)
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the query as a super user
                    Thread.CurrentPrincipal = _superUser;

                    // get the current number
                    number = GetNumber();
                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }

                return number;
            }
        }

		/// <summary>
		/// Generates an unique integer. The value is 32-bit long.
		/// </summary>
		/// <returns>A integer instance</returns>
		public int NextNumber()
		{
            int number;

            if (_schemaInfo == null)
            {
                return 0;
            }

            // Must synchronize access to NextNumber of multiple threads since
            // they'll all update the single number in the database
            lock (this)
			{
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the query as a super user
                    Thread.CurrentPrincipal = _superUser;

                    // get the current number
                    number = GetNumber();

                    // increase by one
                    number++;

                    // update the database
                    UpdateNumber(number);

                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }

                return number;
			}
		}

        /// <summary>
        /// Reset the current number to zero.
        /// </summary>
        public void Reset()
        {
            if (_schemaInfo == null)
            {
                return;
            }

            // Must synchronize access to Reset of multiple threads 
            lock (this)
            {
                IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                try
                {
                    // execute the query as a super user
                    Thread.CurrentPrincipal = _superUser;

                    // update the database
                    UpdateNumber(0);

                }
                finally
                {
                    // attach the original principal to the thread
                    Thread.CurrentPrincipal = originalPrincipal;
                }
            }
        }

        private int GetNumber()
        {
            int number = 0; // default starting number
            string query = GetSequenceNumberQuery.Replace(@"{db}", _schemaInfo.Name);
            query = query.Replace(@"{version}", _schemaInfo.Version);
            query = query.Replace(@"{class}", _className);
            query = query.Replace(@"{key_name}", _keyName);
            query = query.Replace(@"{key_value}", _keyValue);
            query = query.Replace(@"{attribute_name}", _columnName);

            Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
            XmlDocument doc = interpreter.Query(query);

            DataSet ds = new DataSet();
            XmlReader xmlReader = new XmlNodeReader(doc);
            ds.ReadXml(xmlReader);
            DataTable dataTable = ds.Tables[_className];

            if (dataTable != null && dataTable.Rows.Count == 1)
            {
                DataRow dataRow = dataTable.Rows[0];
                string numberStr = dataRow[_columnName].ToString();
                if (!string.IsNullOrEmpty(numberStr))
                {
                    try
                    {
                        number = int.Parse(numberStr);
                    }
                    catch (Exception)
                    {
                        number = 0;
                    }
                }
            }

            return number;
        }

        private void UpdateNumber(int number)
        {
            string query = UpdateSequenceNumberQuery.Replace(@"{db}", _schemaInfo.Name);
            query = query.Replace(@"{version}", _schemaInfo.Version);
            query = query.Replace(@"{class}", _className);
            query = query.Replace(@"{key_name}", _keyName);
            query = query.Replace(@"{key_value}", _keyValue);
            query = query.Replace(@"{attribute_name}", _columnName);
            query = query.Replace(@"{attribute_value}", number.ToString());

            Newtera.Server.Engine.Interpreter.Interpreter interpreter = new Newtera.Server.Engine.Interpreter.Interpreter();
            interpreter.Query(query);
        }
	}
}
