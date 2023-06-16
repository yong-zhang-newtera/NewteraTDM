/*
* @(#)FormatterFactory.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Data;
	using System.Xml;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// A singleton class that creates an instance of IInstanceFormatter
	/// </summary>
	/// <version>1.0.0 20 Jan 2005 </version>
	public class FormatterFactory
	{
		/// <summary>
		/// Formatter definitions, including formatter type and its file type info
		/// based on the file_types.xml file under Config directory
		/// </summary>
		public string[][] FormatterTypes = {
					new string[]{"Text", "text/plain"},
					new string[]{"Xml", "text/xml"}
		};

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static FormatterFactory theFactory;
		
		static FormatterFactory()
		{
			theFactory = new FormatterFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private FormatterFactory()
		{
		}

		/// <summary>
		/// Gets the FormatterFactory instance.
		/// </summary>
		/// <returns> The FormatterFactory instance.</returns>
		static public FormatterFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IInstanceFormatter type
		/// </summary>
		/// <param name="formatterType">The type of formatter</param>
		/// <returns>A IInstanceFormatter instance</returns>
		public IInstanceFormatter Create(string formatterType)
		{
			IInstanceFormatter obj = null;

			switch (formatterType)
			{
				case "Text":
					obj = new TextFormatter();
					break;
				case "Xml":
					obj = new XmlFormatter();
					break;
				case "CSV":
					obj = new CSVConvertor();
					break;
				case "Excel":
					obj = new ExcelConvertor();
					break;
			}
			
			return obj;
		}

		/// <summary>
		/// Get the file type for the file to to send back to the client
		/// </summary>
		/// <param name="formatterType">The type of formatter</param>
		/// <returns>One of file types defined in Config/file_types.xml</returns>
		public string GetFileType(string formatterType)
		{
			string fileType = "text/plain"; //default

			for (int i = 0; i < FormatterTypes.Length; i++)
			{
				if (FormatterTypes[i][0] == formatterType)
				{
					fileType = FormatterTypes[i][1];
					break;
				}
			}

			return fileType;
		}
	}
}