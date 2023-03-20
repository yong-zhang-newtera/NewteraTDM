/*
* @(#) SmtpConfig.cs	1.0.1		2006
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Collections.Specialized;
	using System.Configuration;

	using Newtera.Common.Core;
	
	/// <summary>
	/// This is a singleton that managing the smtp server configuration.
	/// It loads configuration from Application Configuration file.
	/// </summary>
	/// <version> 	1.0.1	10 Jan 2007 </version>
	public class SmtpConfig
	{
		private const string SMTP_HOST = "SmtpHost";
		private const string SMTP_PORT = "SmtpPort";
        private const string SMTP_FROM = "From";
        private const string SMTP_USER = "EmailUser";
        private const string SMTP_PWD = "EmailPassword";
        private const string SMTP_ENCODING = "EmailEncoding";

		// Static object, all invokers will use this factory object.
		private static SmtpConfig theConfig;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private SmtpConfig()
		{
		}

		/// <summary>
		/// Gets the SmtpConfig instance.
		/// </summary>
		/// <returns> The SmtpConfig instance.</returns>
		static public SmtpConfig Instance
		{
			get
			{
				return theConfig;
			}
		}
		
		/// <summary>
		/// Gets the smtp host name
		/// </summary>
		/// <returns>A smtp host</returns>
		public string GetSmtpHost()
		{
            string str = ConfigurationManager.AppSettings[SMTP_HOST];
            if (str != null)
            {
                return str;
            }
            else
            {
                return "localhost";
            }
		}

		/// <summary>
		/// Gets the smtp port
		/// </summary>
		/// <returns>Smtp port, default is 25</returns>
		public int GetSmtpPort()
		{
            int port = 25;
            string str = ConfigurationManager.AppSettings[SMTP_PORT];
            if (str != null)
            {
                try
                {
                    port = int.Parse(str);
                }
                catch
                {
                    port = 25;
                }
            }

            return port;
		}

        /// <summary>
        /// Gets the from address
        /// </summary>
        /// <returns>From Address, default is info@newtera.com</returns>
        public string GetFromAddress()
        {
            string from = ConfigurationManager.AppSettings[SMTP_FROM];
            if (from == null)
            {
                from = "dts@newtera.com"; // default from address
            }

            return from;
        }

        /// <summary>
        /// Gets the user name of the email account
        /// </summary>
        /// <returns>user name, default is info@newtera.com</returns>
        public string GetEmailUser()
        {
            string user = ConfigurationManager.AppSettings[SMTP_USER];
            if (user == null)
            {
                user = "dts@newtera.com"; // default user
            }

            return user;
        }

        /// <summary>
        /// Gets the password of the email account
        /// </summary>
        /// <returns>password, default is newtera</returns>
        public string GetEmailPassword()
        {
            string pwd = ConfigurationManager.AppSettings[SMTP_PWD];
            if (pwd == null)
            {
                pwd = "newtera"; // default pwd
            }

            return pwd;
        }

        /// <summary>
        /// Gets the encoing of the email subject and body messages
        /// </summary>
        /// <returns>default is null</returns>
        public string GetEmailEncoding()
        {
            string encoding = ConfigurationManager.AppSettings[SMTP_ENCODING];
            if (string.IsNullOrEmpty(encoding))
            {
                encoding = "default"; // default
            }

            return encoding;
        }

		static SmtpConfig()
		{
			// Initializing the singleton.
			{
				theConfig = new SmtpConfig();
			}
		}
	}
}