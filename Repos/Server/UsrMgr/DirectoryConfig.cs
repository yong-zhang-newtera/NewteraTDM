/*
* @(#) DirectoryConfig.cs	1.0.1		2006
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.UsrMgr
{
	using System;
	using System.Collections.Specialized;
	using System.Configuration;

	using Newtera.Common.Core;
	
	/// <summary>
	/// This is a singleton that managing the directory configuration.
	/// It loads directory configuration from Application Configuration file.
	/// </summary>
	/// <version> 	1.0.1	28 Aug 2006 </version>
	/// <author> Yong Zhang </author>
	public class DirectoryConfig
	{
		private const string DIRECTORY_TYPE = "DirectoryType";
		private const string CONNECTION_STRING = "DirectoryConnectionString";
		private const string CUSTOM_TYPE_NAME = "CustomTypeName";
		private const string USER_NAME = "DirectoryUserName";
		private const string PASSWORD = "DirectoryUserPassword";

		// Static object, all invokers will use this factory object.
		private static DirectoryConfig theDirectoryConfig;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private DirectoryConfig()
		{
		}

		/// <summary>
		/// Gets the DirectoryConfig instance.
		/// </summary>
		/// <returns> The DirectoryConfig instance.</returns>
		static public DirectoryConfig Instance
		{
			get
			{
				return theDirectoryConfig;
			}
		}
		
		/// <summary>
		/// Gets the type of directory.
		/// </summary>
		/// <returns>One of directory type enum values</returns>
		public DirectoryType GetDirectoryType()
		{
            if (ConfigurationManager.AppSettings == null)
			{
				throw new Exception("No directory settings available");
			}

			DirectoryType type = DirectoryType.Newtera; // default directory type

            string directoryType = ConfigurationManager.AppSettings[DIRECTORY_TYPE];
			
			if (directoryType != null)
			{
				switch (directoryType.ToUpper())
				{
					case "NEWTERA":
						type = DirectoryType.Newtera;
						break;
					case "ACTIVEDIRECTORY":
						type = DirectoryType.ActiveDirectory;
						break;
					case "CUSTOM":
						type = DirectoryType.Custom;
						break;
					default:
						break;
				}
			}

			return type;
		}

		/// <summary>
		/// Gets the connection string of a directory
		/// </summary>
		/// <returns>A connection string to a directory</returns>
		/// <remarks>When directory type is Newtera, no connection string is required.</remarks>
		public string GetConnectionString()
		{
            string connectionString = ConfigurationManager.AppSettings[CONNECTION_STRING];
			
			return connectionString;
		}

		/// <summary>
		/// Gets the user name
		/// </summary>
		/// <returns>A user name for connecting to a directory</returns>
		/// <remarks>username is in form of domain\username. When directory type is Newtera, no user name is required.</remarks>
		public string GetUserName()
		{
            string userName = ConfigurationManager.AppSettings[USER_NAME];
			
			return userName;
		}

		/// <summary>
		/// Gets the user password
		/// </summary>
		/// <returns>A user password for connecting to a directory</returns>
		/// <remarks>When directory type is Newtera, no user password is required.</remarks>
		public string GetUserPassword()
		{
            string password = ConfigurationManager.AppSettings[PASSWORD];
			
			return password;
		}

		/// <summary>
		/// Gets the type name of the custom user manager
		/// </summary>
		/// <returns>A type name in form of className,assemblyName</returns>
		public string GetCustomTypeName()
		{
            string typeName = ConfigurationManager.AppSettings[CUSTOM_TYPE_NAME];
			
			return typeName;
		}

		static DirectoryConfig()
		{
			// Initializing the singleton.
			{
				theDirectoryConfig = new DirectoryConfig();
			}
		}
	}

	/// <summary>
	/// Describe the options of database types
	/// </summary>
	public enum DirectoryType
	{
		Unknown,
		Newtera,
		ActiveDirectory,
		Custom
	}
}