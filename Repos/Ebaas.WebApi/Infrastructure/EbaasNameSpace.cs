/*
* @(#)EbaasNameSpace.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
namespace Ebaas.WebApi.Infrastructure
{
	using System;
	using System.IO;
	using System.Configuration;

    using Newtera.Common.Core;

	/// <summary>
	/// The EbaasNameSpace class is a centralized place where keywords belong to 
	/// Ebaas package are defined as constants. Application will refer to the 
	/// constants rather than directly to the keywords. Therefore, if any changes 
	/// of keywords in Ebaas name space won't affect the application code.
	/// </summary>
	/// <version> 1.0.0 03 Mar 2016 </version>
	public class EbaasNameSpace
	{
		// directory definition
		public const string TEMP_DIR = "temp";
		public const string ATTACHMENTS_DIR = "attachments";
		public const string ATTACHMENTS_DIR_KEY = "AttachmentBasePath";
        public const string CUSTOM_IMAGE_BASE_DIR = @"styles\custom\";
        public const string USER_ICON_DIR = @"styles\custom\icons";
        public const string USER_IMAGE_DIR = @"styles\custom\images";

        /// <summary>
        /// Get the directory for storing attachments
        /// </summary>
        public static string GetAttachmentDir(string rootDir) 
		{
			string dir = ConfigurationSettings.AppSettings[ATTACHMENTS_DIR_KEY];

			if (dir != null && dir.Trim().Length > 0)
			{
				if (!dir.EndsWith(@"\"))
				{
					dir += @"\";
				}
			}
			else
			{
				// use default attachment directory
                dir = rootDir;
                if (!dir.EndsWith(@"\"))
                {
                    dir += @"\";
                }
                
                dir += ATTACHMENTS_DIR + @"\";
			}

			return dir;
		}

        public static string GetTempDir()
        {
            // for web client
            string rootDir = NewteraNameSpace.GetAppHomeDir();
            if (rootDir.EndsWith(@"\"))
            {
                return rootDir + TEMP_DIR + @"\";
            }
            else
            {
                return rootDir + @"\" + TEMP_DIR + @"\";
            }
        }
    }
}