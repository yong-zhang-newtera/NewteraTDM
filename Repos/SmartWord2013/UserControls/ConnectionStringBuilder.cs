/*
* @(#)ConnectionStringBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace SmartWord2013
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// A singleton class that creates a connection string for accessing web services.
	/// </summary>
	/// <version>1.0.0 15 Dec 2003 </version>
	/// <author> Yong Zhang </author>
	public class ConnectionStringBuilder
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ConnectionStringBuilder theBuilder;
		
		private WindowClientUserManager _userManager;

		static ConnectionStringBuilder()
		{
			theBuilder = new ConnectionStringBuilder();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ConnectionStringBuilder()
		{
			_userManager = new WindowClientUserManager();
		}

		/// <summary>
		/// Gets the ConnectionStringBuilder instance.
		/// </summary>
		/// <returns> The ConnectionStringBuilder instance.</returns>
		static public ConnectionStringBuilder Instance
		{
			get
			{
				return theBuilder;
			}
		}

        /// <summary>
        /// Creates a connection string to access server services
        /// </summary>
        /// <returns>A connection string</returns>
        public string Create()
        {
            StringBuilder builder = new StringBuilder();
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName;
            if (principal != null)
            {
                userName = principal.Identity.Name;
            }
            else
            {
                userName = _userManager.GetSuperUserName();
            }

            builder.Append("USER_ID=").Append(userName);

            return builder.ToString();
        }

		
		/// <summary>
		/// Creates a connection string to access a schema via web services
		/// </summary>
		/// <param name="schemaInfo">The schema info to be accessed.</param>
		/// <returns>A connection string</returns>
		public string Create(SchemaInfo schemaInfo)
		{
			StringBuilder builder = new StringBuilder();
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
			string userName;
			if (principal != null)
			{
				userName = principal.Identity.Name;
			}
			else
			{
				userName = _userManager.GetSuperUserName();
			}

			builder.Append("SCHEMA_NAME=").Append(schemaInfo.Name).Append(";SCHEMA_VERSION=").Append(schemaInfo.Version);
			builder.Append(";USER_ID=").Append(userName);
            if (schemaInfo.ModifiedTime != null)
            {
                builder.Append(";TIMESTAMP=").Append(schemaInfo.ModifiedTime.ToString("s"));
            }

			return builder.ToString();
		}

		/// <summary>
		/// Creates a connection string to access a schema via web services
		/// </summary>
		/// <param name="schemaName">The name of schema</param>
		/// <param name="schemaVersion">The version of schema</param>
        /// <param name="timestamp">The schema time stamp</param>
        /// <returns>A connection string</returns>
        public string Create(string schemaName, string schemaVersion, DateTime timestamp)
		{
			StringBuilder builder = new StringBuilder();
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
			string userName;
			if (principal != null)
			{
				userName = principal.Identity.Name;
			}
			else
			{
				userName = _userManager.GetSuperUserName();
			}

			builder.Append("SCHEMA_NAME=").Append(schemaName).Append(";SCHEMA_VERSION=").Append(schemaVersion);
			builder.Append(";USER_ID=").Append(userName);
            if (timestamp != null)
            {
                builder.Append(";TIMESTAMP=").Append(timestamp.ToString("s"));
            }

			return builder.ToString();
		}

		/// <summary>
		/// Creates a connection string to access a schema via web services
		/// </summary>
		/// <param name="schemaInfo">The schema info element.</param>
		/// <returns>A connection string</returns>
		public string Create(SchemaInfoElement schemaInfoElement)
		{
			StringBuilder builder = new StringBuilder();
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
			string userName;
			if (principal != null)
			{
				userName = principal.Identity.Name;
			}
			else
			{
				userName = _userManager.GetSuperUserName();
			}

			builder.Append("SCHEMA_NAME=").Append(schemaInfoElement.Name).Append(";SCHEMA_VERSION=").Append(schemaInfoElement.Version);
			builder.Append(";USER_ID=").Append(userName);
            if (schemaInfoElement.ModifiedTime != null)
            {
                builder.Append(";TIMESTAMP=").Append(schemaInfoElement.ModifiedTime.ToString("s"));
            }

			return builder.ToString();
		}

		/// <summary>
		/// Get the web service name from a class name
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>The service name</returns>
		public string GetServiceName(string className)
		{
			string serviceName = className;
			int pos = serviceName.IndexOf('.');
			if (pos > 0)
			{
				// truncate the "Newtera" part
				serviceName = serviceName.Substring(pos + 1);
			}

			return serviceName;
		}
	}
}