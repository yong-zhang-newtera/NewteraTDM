using System;
using System.Data;
using System.IO;
using System.Text;

using Newtera.Common.MetaData.DataView;

namespace Newtera.Util
{
	/// <summary>
	/// To generate excel file.
	/// </summary>
	internal class ExcelConvertor : FormatterBase
	{
		/// <summary>
		/// Initiate an instance of ExcelConvertor class
		/// </summary>
		public ExcelConvertor() : base()
		{
		}

		#region IInstanceFormatter interface implementation

		/// <summary>
		/// Convert an instance data to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="instanceView">The InstanceView that stores data.</param>
		/// <param name="filePath">The file path</param>
		public override void Save(InstanceView instanceView, string filePath)
		{
		}

		/// <summary>
		/// Convert a DataTable to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable that stores data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		public override void Save(DataTable dataTable, string filePath, params object[] args)
		{
			Convert(dataTable, filePath);
		}
		
		/// <summary>
		/// Convert two DataTable instances as comparison to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="beforeDataTable">The DataTable that stores before data.</param>
		/// <param name="afterDataTable">The DataTable that stores after data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		public override void Save(DataTable beforeDataTable, DataTable afterDataTable, string filePath, params object[] args)
		{
		}

		#endregion

		/// <summary>
		/// To generate excel file.
		/// </summary>
		/// <param name="oDataTable"></param>
		/// <param name="filePath"></param>
		private void Convert(DataTable oDataTable, string filePath)	
		{			
			StringBuilder oStringBuilder = new StringBuilder();

			/********************************************************
			 * Start, check for border width
			 * ******************************************************/
			int borderWidth = 0;

			if (_ShowExcelTableBorder)	
			{
				borderWidth = 1;
			}
			/********************************************************
			 * End, Check for border width
			 * ******************************************************/

			/********************************************************
			 * Start, Check for bold heading
			 * ******************************************************/
			string boldTagStart = "";
			string boldTagEnd = "";
			if (_ExcelHeaderBold)	
			{
				boldTagStart = "<B>";
				boldTagEnd = "</B>";
			}

			/********************************************************
			 * End,Check for bold heading
			 * ******************************************************/

			oStringBuilder.Append("<Table border=" + borderWidth + ">");

			/*******************************************************************
			 * Start, Creating table header
			 * *****************************************************************/

			oStringBuilder.Append("<TR>");

			foreach(DataColumn oDataColumn in oDataTable.Columns)	
			{
				oStringBuilder.Append("<TD>" + boldTagStart + oDataColumn.ColumnName + boldTagEnd + "</TD>");
			}

			oStringBuilder.Append("</TR>");

			/*******************************************************************
			 * End, Creating table header
			 * *****************************************************************/

			/*******************************************************************
			 * Start, Creating rows
			 * *****************************************************************/

			foreach(DataRow oDataRow in oDataTable.Rows)	
			{
				oStringBuilder.Append("<TR>");

				foreach(DataColumn oDataColumn in oDataTable.Columns)	
				{
					if (oDataRow[oDataColumn.ColumnName] is long)	
					{
						oStringBuilder.Append("<TD align=right>" + oDataRow[oDataColumn.ColumnName] + "</TD>");
					}
					else	
					{
						oStringBuilder.Append("<TD>" + oDataRow[oDataColumn.ColumnName] + "</TD>");
					}
					
					
				}

				oStringBuilder.Append("</TR>");
			}
			

			/*******************************************************************
			 * End, Creating rows
			 * *****************************************************************/

			oStringBuilder.Append("</Table>");
			
			// use the default encoding
			byte[] bytes = System.Text.Encoding.Default.GetBytes(oStringBuilder.ToString());

			// Create a file to write to.
			using (FileStream fs = File.Create(filePath)) 
			{
				fs.Write(bytes, 0, bytes.Length);
			}
		}

		private bool _ShowExcelTableBorder = false;

		/// <summary>
		/// To show or hide the excel table border
		/// </summary>
		public bool ShowExcelTableBorder
		{
			get
			{
				return _ShowExcelTableBorder;
			}
			set
			{
				_ShowExcelTableBorder = value;
			}
		}

		private bool _ExcelHeaderBold = true;


		/// <summary>
		/// To make header bold or normal
		/// </summary>
		public bool ExcelHeaderBold 
		{
			get
			{
				return _ExcelHeaderBold;
			}
			set
			{
				ExcelHeaderBold = value;
			}
		}
	}
}
