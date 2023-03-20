/*
* @(#) TextDeEncryptor.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using System.Text;
	using System.Security.Cryptography;
	using System.Globalization;

	/// <summary>
	/// A singleton for text encryption or decryption.
	/// </summary>
	/// <version> 	1.0.0	08 Mar 2006 </version>
	/// <author> Yong Zhang </author>
	public class TextDeEncryptor
	{		
		// Static factory object, all invokers will use this factory object.
		private static TextDeEncryptor theFactory;

		// parameters for encrypting password
		//
		private static byte[] _desKey = new byte []  { 0x12, 0x75, 0xA8, 0xF1, 0x32, 0xED, 0x13, 0xF2 };
		private static byte[] _desIV = new byte []  { 0xA3, 0xEF, 0xD6, 0x21, 0x37, 0x80, 0xCC, 0xB1 };

		/// <summary>
		/// Private constructor.
		/// </summary>
		private TextDeEncryptor()
		{
		}

		/// <summary>
		/// Gets the TextDeEncryptor instance.
		/// </summary>
		/// <returns> The TextDeEncryptor instance.</returns>
		static public TextDeEncryptor Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Encrypt the given text
		/// </summary>
		/// <param name="text">The text to encrypt</param>
		/// <returns>Encrypted string</returns>
		public string Encrypt(string text)
		{
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			des.Key = _desKey; 
			des.IV = _desIV;
			byte[] data = ASCIIEncoding.ASCII.GetBytes(text);
			data = des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
			
			// convert bytes to string
			StringBuilder sb = new StringBuilder();
			for (int i=0; i < data.Length; i++)
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
			for (int i=0, j = 0; i < text.Length; i += 2, j++)
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

		static TextDeEncryptor()
		{
			// Initializing the factory.
			{
				theFactory = new TextDeEncryptor();
			}
		}
	}
}