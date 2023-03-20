/*
* @(#) SequenceNumberGeneratorFactory.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Collections;
	using System.Data;
    using System.Threading;
    using System.Text.RegularExpressions;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// A factory for all SequenceNumberGenerator.
	/// </summary>
	/// <version> 	1.0.0	03 Aug 2011 </version>
	public class SequenceNumberGeneratorFactory
	{
        private const string SCHEMA_NAME = "SCHEMA_NAME";
        private const string SCHEMA_VERSION = "SCHEMA_VERSION";

		// Static factory object, all invokers will use this factory object.
		private static SequenceNumberGeneratorFactory theFactory;

        private static Hashtable _generators;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private SequenceNumberGeneratorFactory()
		{
            _generators = new Hashtable();
		}

		/// <summary>
		/// Gets the SequenceNumberGeneratorFactory instance.
		/// </summary>
		/// <returns> The SequenceNumberGeneratorFactory instance.</returns>
		static public SequenceNumberGeneratorFactory Instance
		{
			get
			{
				return theFactory;
			}
		}

        /// <summary>
		/// Creates a specific SequenceNumberGenerator.
		/// </summary>
        /// <param name="className">A name of the class where the sequence number is stored</param>
        /// <param name="keyName">A name of the column which contains a key identifying the instance where the sequence number is stored</param>
        /// <param name="keyValue">The key value which identifies the instance that contain the sequence number</param>
        /// <param name="sequenceNumberColumn">A name of the column which stores the sequence number</param>
		/// <returns>A SequenceNumberGenerator instance.</returns>
        public SequenceNumberGenerator Create(string className, string keyName, string keyValue, string sequenceNumberColumn)
        {
            string schemaName, schemaVersion;

            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;

            if (customPrincipal != null &&
                customPrincipal.IsServerSide)
            {
                // The client is a web application or web service

                string connectionString = customPrincipal.GetUserDataString(NewteraNameSpace.CURRENT_CONNECTION);
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Unable to find a database connection string in the customPrincipal UserData");
                }

                Hashtable properties = GetProperties(connectionString);
                // validate the properties
                if (properties[SCHEMA_NAME] == null)
                {
                    throw new Exception("Missing property " + SCHEMA_NAME + " in connection string.");
                }

                if (properties[SCHEMA_VERSION] == null)
                {
                    throw new Exception("Missing property " + SCHEMA_VERSION + " in connection string.");
                }

                schemaName = (string)properties[SCHEMA_NAME];
                schemaVersion = (string)properties[SCHEMA_VERSION];

                return Create(schemaName, schemaVersion, className, keyName, keyValue, sequenceNumberColumn);
            }
            else
            {
                return new SequenceNumberGenerator();
            }
        }
		
		/// <summary>
		/// Creates a specific SequenceNumberGenerator.
		/// </summary>
		/// <param name="schemaName">The schema name</param>
        /// <param name="schemaVersion">The scehma version</param>
        /// <param name="className">A name of the class where the sequence number is stored</param>
        /// <param name="keyName">A name of the column which contains a key identifying the instance where the sequence number is stored</param>
        /// <param name="keyValue">The key value which identifies the instance that contain the sequence number</param>
        /// <param name="sequenceNumberColumn">A name of the column which stores the sequence number</param>
		/// <returns>A SequenceNumberGenerator instance.</returns>
		public SequenceNumberGenerator Create(string schemaName, string schemaVersion, string className, string keyName, string keyValue, string sequenceNumberColumn)
		{
			SequenceNumberGenerator generator = null;

            lock (this)
            {
                if (schemaName == null)
                {
                    throw new Exception("schemanName is missing");
                }
                else if (className == null)
                {
                    throw new Exception("className is missing");
                }
                else if (keyName == null)
                {
                    throw new Exception("keyName is missing");
                }
                else if (keyValue == null)
                {
                    throw new Exception("keyValue is missing");
                }
                else if (sequenceNumberColumn == null)
                {
                    throw new Exception("sequenceNumberColumn is missing");
                }

                SchemaInfo schemaInfo = new SchemaInfo();
                schemaInfo.Name = schemaName;
                schemaInfo.Version = schemaVersion;

                string generatorId = schemaInfo.NameAndVersion + className + keyName + keyValue + sequenceNumberColumn;
                generator = (SequenceNumberGenerator) _generators[generatorId];
                if (generator == null)
                {
                    generator = new SequenceNumberGenerator(schemaInfo, className, keyName, keyValue, sequenceNumberColumn);
                    _generators[generatorId] = generator;
                }

                return generator;
            }
		}

        /// <summary>
        /// Get key/value pairs from the connectionString and save them in a hashtable
        /// </summary>
        /// <param name="connectionString">The connectionString</param>
        /// <returns>The hashtable</returns>
        /// <exception cref="InvalidConnectionStringException">
        /// Thrown if missing some critical key/value pairs in the connection string.
        /// </exception>
        protected Hashtable GetProperties(string connectionString)
        {
            Hashtable properties = new Hashtable();

            // Compile regular expression to find "name = value" pairs
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            MatchCollection matches = regex.Matches(connectionString);
            foreach (Match match in matches)
            {
                int pos = match.Value.IndexOf("=");
                string key = match.Value.Substring(0, pos).TrimEnd();
                string val = match.Value.Substring(pos + 1).TrimStart();
                properties[key] = val;
            }

            return properties;
        }

		static SequenceNumberGeneratorFactory()
		{
			// Initializing the factory.
			{
				theFactory = new SequenceNumberGeneratorFactory();
			}
		}
	}
}