using System;
using System.Data;
using System.IO;
using System.Text;

using Newtera.Common.MetaData.DataView;

namespace Newtera.Util
{
	/// <summary>
	/// Summary description for CSVConvertor.
	/// </summary>
	internal class CSVConvertor : FormatterBase
	{
		/// <summary>
		/// Initiate an instance of CSVConvertor class
		/// </summary>
		public CSVConvertor() : base()
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
		/// To generate CSV file.
		/// </summary>
		/// <param name="oDataTable"></param>
		/// <param name="file"></param>
		public void Convert(DataTable oDataTable, string filePath)	
		{
			StreamWriter SW;
			SW = File.CreateText(filePath);
			
			StringBuilder oStringBuilder = new StringBuilder();

			/*******************************************************************
			 * Start, Creating column header
			 * *****************************************************************/

			foreach(DataColumn oDataColumn in oDataTable.Columns)	
			{
				oStringBuilder.Append(oDataColumn.ColumnName + ",");
			}

			SW.WriteLine(oStringBuilder.ToString().Substring(0,oStringBuilder.ToString().Length - 1));
			oStringBuilder.Length = 0;

			/*******************************************************************
			 * End, Creating column header
			 * *****************************************************************/

			/*******************************************************************
			 * Start, Creating rows
			 * *****************************************************************/

			foreach(DataRow oDataRow in oDataTable.Rows)	
			{
				foreach(DataColumn oDataColumn in oDataTable.Columns)	
				{
					oStringBuilder.Append(oDataRow[oDataColumn.ColumnName] + ",");
				}

				SW.WriteLine(oStringBuilder.ToString().Substring(0,oStringBuilder.ToString().Length - 1));
				oStringBuilder.Length = 0;
			}
			
			/*******************************************************************
			 * End, Creating rows
			 * *****************************************************************/

			SW.Close();
		}
	}
}
