/*
* @(#)NewteraLicenseProvider.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Licensing
{
	using System;
	using System.IO;
	using System.ComponentModel;
	using System.Configuration;

	using Newtera.Common.Core;
	using Newtera.Server.Engine.Cache;

	using Infralution.Licensing;

	/// <summary> 
	/// Provides a specific implementation of a LicenseProvider for Newtera Server.
	/// </summary>
	/// <version> 1.0.0 23 Sep 2005 </version>
	/// <author> Yong Zhang </author>
	public class NewteraLicenseProvider : EncryptedLicenseProvider 
	{
		internal const string PASSWORD = "TEST";
		internal const ushort SERIAL_NO = 0;
		internal const string TEMP_LICENSE_KEY = "DF36-86E9-8155-5DDA-5CCC-D1BD-94F8-BB60-EFE5-8495-9844-4FB7-08AC-5909-5A67-1BA7";
		internal const string LICENSE_KEY = "LicenseKey";

		/// <summary>
		/// Encrypt a string data using the same algorithm used to generate a license key.
		/// </summary>
		/// <param name="originalData">The original data.</param>
		/// <returns>The encrypted data string</returns>
		public string EncryptData(string originalData)
		{
			// the original data is treated as the product info in a license key.
			return GenerateKey(NewteraLicenseProvider.PASSWORD, originalData,
				NewteraLicenseProvider.SERIAL_NO);
		}

		/// <summary>
		/// Decrypt a string using the same algorithm used to decrypt a license key
		/// </summary>
		/// <param name="encryptedData">The encrypted data</param>
		/// <returns>The decrypted data string.</returns>
		public string DecryptData(string encryptedData)
		{
			string decryptedData;

			decryptedData = GetProductInfo(encryptedData);

			return decryptedData;
		}

		/// <summary>
		/// Get the license key from the web.cofig AppSettings
		/// </summary>
		/// <param name="context"></param>
		/// <param name="type"></param>
		/// <returns>The license key</returns>
		protected override string GetLicenseKey(LicenseContext context, Type type)
		{
			string licenseKey = null;
			try
			{
				licenseKey = MetaDataCache.Instance.GetLicenseKey();
			
				if (licenseKey == null)
				{
					licenseKey = GetTempLicenseKey();
				}
			}
			catch (Exception)
			{
				// the database connection has not been set up correctly
				// get a trial license from the configuration file to get things going
				licenseKey = GetTempLicenseKey();
			}

			return licenseKey;
		}

		/// <summary>
		/// Get a temporory license key to complete the installation
		/// </summary>
		/// <returns></returns>
		private string GetTempLicenseKey()
		{
			string licenseKey = null;
            if (ConfigurationManager.AppSettings != null)
			{
                licenseKey = ConfigurationManager.AppSettings[LICENSE_KEY];
			}

			if (licenseKey == null)
			{
				licenseKey = TEMP_LICENSE_KEY;
			}

			return licenseKey;
		}

	}
}