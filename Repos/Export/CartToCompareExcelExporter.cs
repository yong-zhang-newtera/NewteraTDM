/*
* @(#)ExcelExporter.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Export
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Data;
    using System.Web;
    using System.Collections;

	/// <summary>
	/// Export data in DataTable into an Excel file
	/// </summary>
	/// <version> 1.0.0 01 June 2006</version>
    public class CartToCompareExcelExporter : IExporter 
	{
        private string _filePath;

		/// <summary>
		/// Initialize the exporter
		/// </summary>
        public CartToCompareExcelExporter()
		{
            
		}

        /// <summary>
        /// Called at the beginning of the exporting data, allow the exporter to
        /// perform the initialization necessay for exporting process, such as
        /// open the file to write data.
        /// </summary>
        public void BeginExport(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
        public void ExportData(DataTable dataTable)
		{

		}

        /// <summary>
        /// Export Excel by fan
        /// </summary>
        public  void CreateExcel(string fileName, DataTable dt)
        {
 
           
        }

        /// <summary>
        /// Called at the end of the exporting data, allow the exporter to
        /// free up the resources used by the exporter, such as closing the file
        /// </summary>
        public void EndExport()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            File.Copy(@"C:\Newtera\Ebaas\Repos\temp\CompareResult.xls", _filePath);

        }

        /// <summary>
        /// Export the data in the Xml to a file.
        /// </summary>
        /// <param name="xmlString">The xmlstring for exporting</param>
        public void ExportXml(string xmlString)
        {
        }

   	}
            
}