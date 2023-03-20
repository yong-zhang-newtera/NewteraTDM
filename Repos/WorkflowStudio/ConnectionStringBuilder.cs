/*
* @(#)ConnectionStringBuilder.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace WorkflowStudio
{
	using System;
	using System.Xml;
	using System.Text;
	using System.Threading;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// A singleton class that creates a connection string for accessing server services.
	/// </summary>
	/// <version>1.0.0 15 Dec 2006 </version>
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
		/// Creates a connection string to access server services
		/// </summary>
        /// <param name="timestamp">The time stamp associated with a project model</param>
		/// <returns>A connection string</returns>
		public string Create(DateTime timestamp)
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
            if (timestamp != null)
            {
                builder.Append(";TIMESTAMP=").Append(timestamp.ToString("s"));
            }

			return builder.ToString();
		}

        /// <summary>
        /// Creates a connection string to access a schema via web services
        /// </summary>
        /// <param name="schemaName">The name of schema</param>
        /// <param name="schemaVersion">The version of schema</param>
        /// <returns>A connection string</returns>
        public string Create(string schemaName, string schemaVersion)
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

            return builder.ToString();
        }
	}
}