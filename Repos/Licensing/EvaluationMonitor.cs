//
//      FILE:   EvaluationMonitor.cs
//
//    AUTHOR:   Grant Frisken
//
// COPYRIGHT:   Copyright 2004 
//              Infralution
//              6 Bruce St 
//              Mitcham Australia
//
// Modified By: Yong Zhang
//              Newtera, Inc.
//
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

using Newtera.Registry;

namespace Infralution.Licensing
{
	/// <summary>
	/// Provides a mechanism for managing time/usage limited evaluations of products.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Instantiate an instance of this class to read/write the evaluation parameters for the   
	/// given product.  The <see cref="FirstUseDate"/> is set the first time that
	/// the class is instantiated.  The <see cref="LastUseDate"/> is set each time the class
	/// is instantiated.  The <see cref="UsageCount"/> is incremented each time the class is 
	/// instantiated.
    /// </para>
    /// <para>
    /// Note that evaluation data must be stored somewhere on the users 
    /// hard disk.  It is therefore not too difficult for a sophisticated user to determine the 
    /// changes made either to registry keys or files (using file/registry monitoring software) 
    /// and restore the state of these to their pre-installation state (thus resetting the 
    /// evaluation period).  For this reason it is recommended that you don't rely on this 
    /// mechanism alone.  You should also consider limiting the functionality of your product 
    /// in some way or adding nag screens to discourage long term use of evaluation versions.
    /// </para>
    /// <para>
    /// If you have a data oriented application you can increase the security of evaluations by
    /// storing the current <see cref="UsageCount"/> somewhere in your database each time the 
    /// application runs and cross checking this with the number returned by the EvaluationMonitor.
    /// </para>
    /// </remarks>
    public class EvaluationMonitor : IDisposable
    {
        #region Member Variables

        private string _productData;
        private int _usageCount = 0;
        private DateTime _firstUseDate = DateTime.MinValue;
        private DateTime _lastUseDate = DateTime.MinValue;
        private bool _invalid = false;
        private bool _disabled = false; // added property

        private XmlRegistryKey _rootKey;
        private XmlRegistryKey _baseKey;
        private string _usageKeyName;
        private string _firstUseKeyName;
        private string _lastUseKeyName;
        private string _disabledKeyName;

        // Sub field names for saving data.  Designed to
        // blend in with their surroundings
        //
        private const string classUsageKey = "TypeLib";
        private const string classFirstUseKey = "InprocServer32";
        private const string classLastUseKey = "Control";
        private const string classDisabledKey = "ObjClassType";

        /*
        private const string userUsageKey = "Software\\Microsoft\\WAB\\WAB4";
        private const string userFirstUseKey = "Software\\Microsoft\\WAB\\WAB Sort State";
        private const string userLastUseKey = "Software\\Microsoft\\WAB\\WAB4\\LastFind";
        private const string disabledKey = "Software\\Microsoft\\WAB\\WAB5";
        */

        // parameters for encrypting evaluation data
        //
        private static byte[] _desKey = new byte []  { 0x12, 0x75, 0xA8, 0xF1, 0x32, 0xED, 0x13, 0xF2 };
        private static byte[] _desIV = new byte []  { 0xA3, 0xEF, 0xD6, 0x21, 0x37, 0x80, 0xCC, 0xB1 };
       

        #endregion

        #region Public Interface

        /// <summary>
        /// Initialize a new instance of the evaluation monitor.
        /// </summary>
        /// <remarks>
        /// The <see cref="UsageCount"/> is incremented each time a new evaluation
        /// monitor is instantiated for a product
        /// </remarks>
        /// <param name="productID">A string which uniquely identifies the product</param>
        public EvaluationMonitor(string productID)
        {
            _productData = Encrypt(productID);
            XmlRegistryKey parentKey;
            XmlRegistry theRegistry = XmlRegistryManager.Instance;

            // if that succeeded then set up the keys appropriately
            _rootKey = theRegistry.RootKey;
            parentKey = _rootKey["CLSID", true];
            _usageKeyName = classUsageKey;
            _firstUseKeyName = classFirstUseKey;
            _lastUseKeyName = classLastUseKey;
            _disabledKeyName = classDisabledKey;


            try
            {
                // find the base key
                //
                _baseKey = FindBaseKey(parentKey);

                // OK we couldn't find a key so create it
                //
                if (_baseKey == null)
                {
                    _usageCount = 0;
                    _firstUseDate = DateTime.UtcNow;
                    _lastUseDate = DateTime.UtcNow;
                    _disabled = false;
                    CreateBaseKey(parentKey);
                }
                else
                {
                    GetDateData();
                    GetUsageData();
                    GetDisabledData();
                }
            }
            catch (Exception)
            {
                _invalid = true;
            }
        }

        /// <summary>
        /// Return the number of times the product has been used 
        /// </summary>
        /// <remarks>
        /// This is calculated by the number of times the Evaluation object for the
        /// product has been instantiated, so typically you should instantiate the
        /// Evaluation object just once in your software.
        /// </remarks>
        public int UsageCount
        {
            get {
                lock (this)
                {
                    return _usageCount;
                }
            }
        }

        /// <summary>
        /// Return the date/time the product was first used
        /// </summary>
        public DateTime FirstUseDate
        {
            get {
                lock (this)
                {
                    return _firstUseDate;
                }
            }
        }

        /// <summary>
        /// Return the date/time the product was last used
        /// </summary>
        public DateTime LastUseDate
        {
            get {
                lock (this)
                {
                    return _lastUseDate;
                }
            }
        }

        /// <summary>
        /// Return the number of days since the product was first run.
        /// </summary>
        public int DaysInUse
        {
            get
            {
                // sync the access
                lock (this)
                {
                    return DateTime.UtcNow.Subtract(_firstUseDate).Days + 1;
                }
            }
        }

        /// <summary>
        /// Returns true if the license is disabled, false otherwise
        /// </summary>
        /// <remarks>This method is added by Newtera</remarks>
        public bool Disabled
        {
            get
            {
                return _disabled;
            }
            set
            {
                _disabled = value;
                // write to the registry
                SetDisabledData(_disabled);
            }
        }

        /// <summary>
        /// Returns true if the evaluation monitor detects attempts to circumvent
        /// evaluation limits by tampering with the hidden evaluation data or winding
        /// the PC clock backwards 
        /// </summary>
        public bool Invalid
        {
            get {
                // sync the access
                lock (this)
                {
                    return _invalid;
                }
            }
        }

        /// <summary>
        /// This method set today's date as the last used date. If it detects that the computer
        /// date has been winded back, it set monitor to be invalid
        /// </summary>
        public void SetLastUsedDate()
        {
            if (_baseKey == null)
            {
                throw new Exception("Failed to create a key in theRegistry, please check if the asp.net account has a permission to registry keys");
            }

            // sync the access
            lock (this)
            {
                XmlRegistryKey lastUseKey = _baseKey[_lastUseKeyName, true];

                string dateString = Decrypt(lastUseKey.GetStringValue());
                try
                {
                    _lastUseDate = DateTime.Parse(dateString);
                }
                catch (Exception)
                {
                    // value in the register has corrupted, set the current time as the last used date
                    _lastUseDate = DateTime.UtcNow;
                }

                // detect winding the clock back on the PC - give them six hours of grace to allow for
                // daylight saving adjustments etc
                //
                  
                double hoursSinceLastUse = DateTime.UtcNow.Subtract(_lastUseDate).TotalHours;
                if (hoursSinceLastUse < -6.0)
                {
                    _invalid = true;
                }
                else
                {
                    lastUseKey.SetValue(Encrypt(DateTime.UtcNow.ToString()));
                    _invalid = false;
                }

                lastUseKey.SetValue(Encrypt(DateTime.UtcNow.ToString()));
                _invalid = false;

                lastUseKey.Close();
            }
        }

        /// <summary>
        /// Allows you to reset the evaluation period.
        /// </summary>
        /// <remarks>
        /// This may be useful if a customer needs an extension or if somehow they
        /// invalidate their evaluation data by attempting to fiddle
        /// </remarks>
        public void Reset()
        {
            if (_baseKey != null)
            {
                string name = _baseKey.Name;
                int i = name.IndexOf("\\") + 1;
                name = name.Substring(i);
                _baseKey.Close();
                _baseKey = null;
                try
                {
                    _rootKey.DeleteSubKeyTree(name);
                }
                catch {} // ignore possible failures
            }
            _usageCount = 0;
            _firstUseDate = DateTime.UtcNow;
            _lastUseDate = _firstUseDate;
            _disabled = false;
            _invalid = false;
        }

        #endregion

        #region Local Methods


        /// <summary>
        /// Find the base key for this product
        /// </summary>
        /// <param name="parent">The key to search under</param>
        /// <returns>The base registry key used to store the data</returns>
        private XmlRegistryKey FindBaseKey(XmlRegistryKey parent)
        {
            return parent.GetSubKey("ProductData", false);
        }

        /// <summary>
        /// Create the base key for this product
        /// </summary>
        /// <param name="parent">The key to place the information under</param>
        private void CreateBaseKey(XmlRegistryKey parent)
        {
            // create the registry key with a unique name each time - this makes it a little
            // more difficult for people to find the key
            //
            string baseKeyName = "ProductData";
            _baseKey = parent[baseKeyName, true];
            _baseKey.SetValue(_productData);
            XmlRegistryKey dateKey = _baseKey.GetSubKey(_firstUseKeyName, true);
            dateKey.SetValue(Encrypt(_firstUseDate.ToString()));
            dateKey.Close();

            // create the usage key and set the initial value
            //
            XmlRegistryKey usageKey = _baseKey.GetSubKey(_usageKeyName, true);
            usageKey.SetValue(Encrypt(_usageCount.ToString()));
            usageKey.Close();   

            // create the last use key and set the initial value
            //
            XmlRegistryKey lastUseKey = _baseKey.GetSubKey(_lastUseKeyName, true);
            lastUseKey.SetValue(Encrypt(_lastUseDate.ToString()));
            usageKey.Close();

            // create the disabled key and set the initial value
            //
            XmlRegistryKey disabledKey = _baseKey.GetSubKey(_disabledKeyName, true);
            disabledKey.SetValue(Encrypt(_disabled.ToString()));
            disabledKey.Close();  
        }

        /// <summary>
        /// Calculate the number of days the product has been in use
        /// </summary>
        private void GetDateData()
        {
            string dateString;
            XmlRegistryKey firstUseKey = _baseKey.GetSubKey(_firstUseKeyName, false);
            dateString = Decrypt(firstUseKey.GetStringValue());
            _firstUseDate = DateTime.Parse(dateString);
            firstUseKey.Close();

            XmlRegistryKey lastUseKey = _baseKey.GetSubKey(_lastUseKeyName, false);
            dateString = Decrypt(lastUseKey.GetStringValue());
            _lastUseDate = DateTime.Parse(dateString);

            // detect winding the clock back on the PC - give them six hours of grace to allow for
            // daylight saving adjustments etc
            //
            double hoursSinceLastUse = DateTime.UtcNow.Subtract(_lastUseDate).TotalHours;
            if (hoursSinceLastUse < -6.0)
            {
                _invalid = true;
            }
            else
            {
                string test = DateTime.UtcNow.ToString();
                lastUseKey.SetValue(Encrypt(DateTime.UtcNow.ToString()));
                _invalid = false;
            }
            lastUseKey.Close();
        }

        /// <summary>
        /// Get the number of times the product has been used (and increment)
        /// </summary>
        private void GetUsageData()
        {
            // get the previous usage count
            //
            XmlRegistryKey usageKey = _baseKey.GetSubKey(_usageKeyName, false);
            string countString = Decrypt(usageKey.GetStringValue());
            _usageCount = int.Parse(countString);       

            // increment the usage count
            //
            _usageCount++;
            usageKey.SetValue(Encrypt(_usageCount.ToString()));
            usageKey.Close();
        }

        /// <summary>
        /// Set the information indicating whether the license has been disabled or not
        /// </summary>
        /// <remarks>This method is added by Newtera</remarks>
        private void SetDisabledData(bool status)
        {
            // get the previous disabled status
            //
            XmlRegistryKey disabledKey = _baseKey.GetSubKey(_disabledKeyName, false);
            // version older than 3.4.0 does not have this key
            if (disabledKey != null)
            {
                disabledKey.SetValue(Encrypt(status.ToString()));
                disabledKey.Close();
            }
        }

        /// <summary>
        /// Get the information indicating whether the license has been disabled or not
        /// </summary>
        /// <remarks>This method is added by Newtera</remarks>
        private void GetDisabledData()
        {
            // get the previous disabled status
            //
            XmlRegistryKey disabledKey = _baseKey.GetSubKey(_disabledKeyName, false);
            // version older than 3.4.0 does not have this key
            if (disabledKey != null)
            {
                string disabledString = Decrypt(disabledKey.GetStringValue());
                if (!string.IsNullOrEmpty(disabledString))
                {
                    try
                    {
                        _disabled = bool.Parse(disabledString);
                    }
                    catch (Exception)
                    {
                        _disabled = false;
                    }
                }
                else
                {
                    _disabled = false;
                }

                disabledKey.Close();
            }
        }

        /// <summary>
        /// Encrypt the given text
        /// </summary>
        /// <param name="text">The text to encrypt</param>
        /// <returns>Encrypted string</returns>
        private string Encrypt(string text)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey;
            des.IV = _desIV;
            byte[] data = ASCIIEncoding.ASCII.GetBytes(text);
            data = des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);

            // convert bytes to string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Decrypt the given text
        /// </summary>
        /// <param name="text">The text to decrypt</param>
        /// <returns>The decrypted text</returns>
        public string Decrypt(string text)
        {
            byte[] data = new byte[text.Length / 2];
            for (int i = 0, j = 0; i < text.Length; i += 2, j++)
            {
                string s = text.Substring(i, 2);
                data[j] = byte.Parse(s, NumberStyles.HexNumber);
            }

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = _desKey;
            des.IV = _desIV;
            byte[] decryptedData = des.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            return ASCIIEncoding.ASCII.GetString(decryptedData);
        }

        /// <summary>
        /// Are the contents of the two byte arrays equal
        /// </summary>
        /// <param name="a1">The first array</param>
        /// <param name="a2">The second array </param>
        /// <returns>True if the contents of the arrays is equal</returns>
        private bool Equals(byte[] a1, byte[] a2)
        {
            if (a1 == a2) return true;
            if ((a1 == null) || (a2 == null)) return false;
            if (a1.Length != a2.Length) return false;
            for (int i=0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i]) return false;
            }
            return true;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Free resources used by the EvaluationMonitor
        /// </summary>
        public void Dispose()
        {
            if (_baseKey != null)
            {
                _baseKey.Close();
                _baseKey = null;
            }
        }

        #endregion
    }
}
