/*
* @(#)TextFormatter.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Data;
	using System.Text;
	using System.ComponentModel;
	using System.Collections;

	using Newtera.Common.MetaData.DataView;

	/// <summary> 
	/// The class converts an instance to a text format
	/// </summary>
	/// <version> 1.0.0 20 Jan 2005</version>
	internal class TextFormatter : FormatterBase
	{
		/// <summary>
		/// Initiate an instance of TextFormatter class
		/// </summary>
		public TextFormatter() : base()
		{
		}

		#region IInstanceFormatter interface implementation

		/// <summary>
		/// Convert an instance data to a text format and save it to a file.
		/// </summary>
		/// <param name="instanceView">The InstanceView that stores data.</param>
		/// <param name="filePath">The file path</param>
		public override void Save(InstanceView instanceView, string filePath)
		{
			StringBuilder builder = new StringBuilder();

			PropertyDescriptorCollection properties = instanceView.GetProperties(null);

			foreach(InstanceAttributePropertyDescriptor pd in properties)
			{
				if (pd.IsArray)
				{
					FormatArray(pd, builder);
				}
				else if (pd.IsRelationship)
				{
					FormatRelationship(pd, builder);
				}
				else
				{
					FormatAttribute(pd, builder);
				}
			}

			// use the default encoding
			byte[] bytes = System.Text.Encoding.Default.GetBytes(builder.ToString());

			// Create a file to write to.
			using (FileStream fs = File.Create(filePath)) 
			{
				fs.Write(bytes, 0, bytes.Length);
			}
		}

		/// <summary>
		/// Convert a DataTable to a corresponding format, such as XML,
		/// Text, or other binary format etc, and save it to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable that stores data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a text formatter.
		public override void Save(DataTable dataTable, string filePath, params object[] args)
		{
			Save(dataTable, null, filePath, args);
		}

		/// <summary>
		/// Convert two DataTable instances as comparison to a corresponding Text format
		/// and save it to a file.
		/// </summary>
		/// <param name="beforeDataTable">The DataTable that stores before data.</param>
		/// <param name="afterDataTable">The DataTable that stores after data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		public override void Save(DataTable beforeDataTable, DataTable afterDataTable, string filePath, params object[] args)
		{
			StringBuilder builder = new StringBuilder();
			
			FormatDataTable(beforeDataTable, builder);

			// use the default encoding
			byte[] bytes = System.Text.Encoding.Default.GetBytes(builder.ToString());

			// Create a file to write to.
			using (FileStream fs = File.Create(filePath)) 
			{
				fs.Write(bytes, 0, bytes.Length);
			}
		}

		#endregion

		/// <summary>
		/// Convert a value of simple attribute to a text representation
		/// </summary>
		/// <param name="pd">InstanceAttributePropertyDescriptor</param>
		/// <param name="builder">StringBuilder</param>
		private void FormatAttribute(InstanceAttributePropertyDescriptor pd,
			StringBuilder builder)
		{
			object obj = pd.GetValue();
			string val;
			if (obj == null)
			{
				val = "";
			}
			else if (pd.PropertyType.IsEnum && pd.IsMultipleChoice)
			{
				object[] vEnums = (object[]) obj;
				StringBuilder buffer = new StringBuilder();
				for (int i = 0; i < vEnums.Length; i++)
				{
					if (i > 0)
					{
						buffer.Append(",");
					}

					buffer.Append(vEnums[i].ToString());
				}
				val = buffer.ToString();
			}
			else
			{
				val = obj.ToString();
			}

			builder.Append(pd.DisplayName).Append("=").Append(val).Append(Environment.NewLine);
		}

		/// <summary>
		/// Convert a value of relationship attribute to a text representation
		/// </summary>
		/// <param name="pd">InstanceAttributePropertyDescriptor</param>
		/// <param name="builder">StringBuilder</param>
		private void FormatRelationship(InstanceAttributePropertyDescriptor pd,
			StringBuilder builder)
		{
			builder.Append(pd.DisplayName).Append("=").Append(pd.GetValue()).Append(Environment.NewLine);
		}

		/// <summary>
		/// Convert a value of array attribute to a text representation
		/// </summary>
		/// <param name="pd">InstanceAttributePropertyDescriptor</param>
		/// <param name="builder">StringBuilder</param>
		private void FormatArray(InstanceAttributePropertyDescriptor pd,
			StringBuilder builder)
		{
			ArrayDataTableView arrayView = (ArrayDataTableView) pd.GetValue();
			DataTable arrayTable = arrayView.ArrayAttributeValue;

			FormatDataTable(arrayTable, builder);
		}

		/// <summary>
		/// Convert a DataTable to a text representation
		/// </summary>
		/// <param name="dataTable">DataTable instance</param>
		/// <param name="builder">StringBuilder</param>
		private void FormatDataTable(DataTable dataTable, StringBuilder builder)
		{
			// output datatable column names first
			int index = 0;
			foreach (DataColumn col in dataTable.Columns)
			{
				if (index > 0)
				{
					builder.Append("\t");
				}

				builder.Append(col.ColumnName);

				index++;
			}

			builder.Append(Environment.NewLine);

			// output array data
			foreach (DataRow row in dataTable.Rows)
			{
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					if (i > 0)
					{
						builder.Append("\t");
					}

					builder.Append(row[i].ToString());
				}

				builder.Append(Environment.NewLine);
			}
		}
	}
}