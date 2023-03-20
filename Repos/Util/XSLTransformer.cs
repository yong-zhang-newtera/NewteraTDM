/*
* @(#)XSLTransformer.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Newtera.Util
{
	/// <summary> 
	/// The class transform a DataSet to a file of a certain format using XSL
	/// </summary>
	/// <version> 1.0.0 16 Mar 2006</version>
	public class XSLTransformer
	{
		/// <summary>
		/// Convert a DataTable instance to the specified file format and save it
		/// in the given file path.
		/// </summary>
		/// <param name="dt">The DataTable to be converted</param>
		/// <param name="format">The converting file format</param>
		/// <returns>A string of content</returns>
		public static string Transform(DataTable dt, FileFormatEnum format)
		{
			DataSet ds = new DataSet();
			ds.Tables.Add(dt);
			return Transform(ds, format);
		}

		/// <summary>
		/// Convert a DataSet instance to the specified file format and save it
		/// in the given file path.
		/// </summary>
		/// <param name="ds">The DataSet to be converted</param>
		/// <param name="format">The converting file format</param>
		/// <returns>A string of content</returns>
		public static string Transform(DataSet ds, FileFormatEnum format)
		{
			XmlDataDocument xmlDataDoc = new XmlDataDocument(ds);
			string xml = ds.GetXml();

            XslTransform xt = new XslTransform();
			string xslFileName = "Excel.xsl";
			switch (format)
			{
				case FileFormatEnum.CSV:
					xslFileName = "Text.xsl";
					break;

				case FileFormatEnum.Excel:
					xslFileName = "Excel.xsl";
					break;
			}

			StreamReader reader =
				new StreamReader(typeof(XSLTransformer).Assembly.GetManifestResourceStream(typeof(XSLTransformer), xslFileName));
			XmlTextReader xRdr = new XmlTextReader(reader);
			xt.Load(xRdr, null, null);

			StringWriter sw = new StringWriter();
			xt.Transform(xmlDataDoc, null, sw, null);

			return sw.ToString();
		}
	}

	/// <summary>
	/// File format enum
	/// </summary>
	public enum FileFormatEnum
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,
		/// <summary>
		/// CSV file
		/// </summary>
		CSV,
		/// <summary>
		/// Excel file
		/// </summary>
		Excel
	}
}