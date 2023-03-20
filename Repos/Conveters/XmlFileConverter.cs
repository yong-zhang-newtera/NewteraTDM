/*
* @(#)XmlFileConverter.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Conveters
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Data;

	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// The class implements a converter for xml files.
	/// </summary>
	/// <version> 1.0.0 07 Sep 2004</version>
	internal class XmlFileConverter : DataSourceConverterBase
	{
		/// <summary>
		/// Initiate an instance of XmlFileConverter class
		/// </summary>
		public XmlFileConverter() : base()
		{
		}

		/// <summary>
		/// Initiate an instance of XmlFileConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public XmlFileConverter(string name) : base(name)
		{
		}

		/// <summary>
		/// Overriding the method to implement operation of converting an xml file. 
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies an xml file</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public override DataSet Convert(string dataSourceName)
		{
			// The DataSet to return
			DataSet dataSet = new DataSet();

			dataSet.ReadXml(dataSourceName);

			return dataSet;
		}
	}
}