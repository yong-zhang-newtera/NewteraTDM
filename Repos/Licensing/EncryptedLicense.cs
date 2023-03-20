//
//      FILE:   EncryptedLicense.cs
//
//    AUTHOR:   Grant Frisken
//
// COPYRIGHT:   Copyright 2005 
//              Infralution
//              6 Bruce St 
//              Mitcham Australia
//
using System;
using System.ComponentModel;
namespace Infralution.Licensing
{
	/// <summary>
	/// Defines an encrypted license for a component or control generated using the Infralution
	/// Licensing System.
	/// </summary>
	/// <remarks>
	/// The Infralution Licensing System provides a secure way of licensing .NET controls,
	/// components and applications.   Licenses are protected using public key encryption to
	/// minimize possibility of cracking.
	/// </remarks>
	/// <seealso cref="EncryptedLicenseProvider"/>
	public class EncryptedLicense : License
	{
        #region Member Variables

        private string _key;
        private UInt16 _serialNo;
        private string _productInfo;

        #endregion

        #region Public Interface

        /// <summary>
        /// Create a new Infralution Encrypted License
        /// </summary>
        /// <param name="key">The key for the license</param>
        /// <param name="serialNo">The serial number of the license</param>
        /// <param name="productInfo">The product data associated with the license</param>
		public EncryptedLicense(string key, UInt16 serialNo, string productInfo)
		{
            _key = key;
            _serialNo = serialNo;
            _productInfo = productInfo;
		}

        /// <summary>
        /// The license key for the license
        /// </summary>
        public override string LicenseKey
        {
            get { return _key; }
        }

        /// <summary>
        /// The product data associated with the license
        /// </summary>
        public string ProductInfo
        {
            get { return _productInfo; }
        }

        /// <summary>
        /// The unique serial no for the license
        /// </summary>
        public UInt16 SerialNo
        {
            get { return _serialNo; }
        }

        /// <summary>
        /// Cleans up any resources held by the license
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// Returns a four character checksum based on the given input string
        /// </summary>
        /// <param name="input">The input string to return a checksum for</param>
        /// <returns>An checksum that can be used to validate the given input string</returns>
        /// <remarks>
        /// <para>
        /// This function can be used to generate a short checksum that can be embedded in a
        /// license key as <see cref="ProductInfo"/>.  This allows you to tie the license key to 
        /// information supplied by the user (for instance the name of the purchaser) without
        /// having to include the full information in the license key.  This enables license keys
        /// to be kept reasonably short.
        /// </para>
        /// <para>
        /// When the license is checked by the application it performs a checksum on the information
        /// supplied by the user and checks that it matches the information in the ProductInfo that
        /// was generated when the license was issued.   The License Tracker application provides
        /// support for "CustomGenerators" which allow you provide the code to generate the ProductInfo
        /// from customer and other information.
        /// </para>
        /// </remarks>
        static public string Checksum(string input)
        {
            int hash = (input == null) ? 0 : input.GetHashCode();
            hash = Math.Abs(hash % 1000);
            return hash.ToString();
        }

        #endregion
	}
}
