/*
* @(#)TecPlotFormatter.cs
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
	/// The class converts an instance to a TecPlot format. TecPlot is an specialized
	/// graphic tool.
	/// </summary>
	/// <version> 1.0.0 20 Jan 2005</version>
	internal class TecPlotFormatter : FormatterBase
	{
		/// <summary>
		/// Initiate an instance of TecPlotFormatter class
		/// </summary>
		public TecPlotFormatter() : base()
		{
		}

		#region IInstanceFormatter interface implementation

		/// <summary>
		/// Convert an instance data to a TecPlotter format and save it to a file.
		/// </summary>
		/// <param name="instanceView">The InstanceView that stores data.</param>
		/// <param name="filePath">The file path</param>
		/// <returns>A formatted data in byte array</returns>
		public override void Save(InstanceView instanceView, string filePath)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("TITLE=\"").Append(instanceView.DataView.BaseClass.Caption).Append("\"").Append(Environment.NewLine);

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
		/// <param name="args">The vary-lengthed arguments used by a tecplot formatter.
		public override void Save(DataTable dataTable, string filePath, params object[] args)
		{
			Save(dataTable, null, filePath, args);
		}

		/// <summary>
		/// Convert two DataTable instances as comparison to a corresponding TecPlot format
		/// and save it to a file.
		/// </summary>
		/// <param name="beforeDataTable">The DataTable that stores before data.</param>
		/// <param name="afterDataTable">The DataTable that stores after data.</param>
		/// <param name="filePath">The file path</param>
		/// <param name="args">The vary-lengthed arguments used by a formatter. Each formatter may require different set of arguments.</param>
		/// <remarks>Both DataTable instances should have some structure in terms of columns and order.</remarks>
		public override void Save(DataTable beforeDataTable, DataTable afterDataTable, string filePath, params object[] args)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("TITLE=\"").Append(beforeDataTable.TableName).Append("\"").Append(Environment.NewLine);

			// show the interpolated graph first
			if (afterDataTable != null)
			{
				FormatDataTable(afterDataTable, builder, args);
			}

			FormatDataTable(beforeDataTable, builder, args);

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
			else if (pd.IsMultipleChoice)
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

			// output array column names first
			FormatDataTable(arrayTable, builder, TecPlotDisplayType.SignleZone, 0);
		}

		/// <summary>
		/// Convert a DataTable to a text representation
		/// </summary>
		/// <param name="dataTable">A data table instance.</param>
		/// <param name="builder">StringBuilder</param>
		/// <param name="xAxisIndex">The index of a column in the DataTable that serves as x-axis.</param>
		/// <param name="isMultipleLined">The information indicating whether to convert columns in the DataTable to a multiple lines, true for multiple lines, false, otherwise.</param>
		private void FormatDataTable(DataTable dataTable, StringBuilder builder,
			params object[] args)
		{
			TecPlotDisplayType displayType = (TecPlotDisplayType) args[0];

			switch (displayType)
			{
				case TecPlotDisplayType.SignleZone:
					FormatSingleZone(dataTable, builder, (int) args[1] /* x axis index */);
					break;

				case TecPlotDisplayType.MultipleZone:
					FormatMultipleZone(dataTable, builder, (int) args[1] /* x axis index */);
					break;
				case TecPlotDisplayType.TwoDimension:
					FormatTwoDimension(dataTable, builder, (int) args[1], /* x axis index */
						(int) args[2], /* y axis index */
						(int) args[3], /* i */
						(int) args[4] /* j */);
					break;
			}
		}

		/// <summary>
		/// Convert to a single zone format
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="builder"></param>
		/// <param name="xAxisIndex"></param>
		private void FormatSingleZone(DataTable dataTable, StringBuilder builder,
			int xAxisIndex)
		{
			// output column names as variable
			builder.Append("VARIABLES=");
			// place the x axis first
			builder.Append("\"").Append(dataTable.Columns[xAxisIndex].ColumnName).Append("\" ");
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				if (i != xAxisIndex)
				{
					builder.Append("\"").Append(dataTable.Columns[i].ColumnName).Append("\" ");
				}
			}

			builder.Append(Environment.NewLine);

			// define a one-dimension zone
			builder.Append("ZONE ").Append(" F=POINT");

			builder.Append(Environment.NewLine);

			// output array data
			foreach (DataRow row in dataTable.Rows)
			{
				// output x axis first
				builder.Append(row[xAxisIndex].ToString()).Append(" ");
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					if (i !=  xAxisIndex)
					{
						builder.Append(row[i].ToString()).Append(" ");
					}
				}

				builder.Append(Environment.NewLine);
			}
		}

		/// <summary>
		/// Convert to multiple zone format
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="builder"></param>
		/// <param name="xAxisIndex"></param>
		private void FormatMultipleZone(DataTable dataTable, StringBuilder builder,
			int xAxisIndex)
		{
			// output X and Y variables
			builder.Append("VARIABLES=\"X\" \"Y\"");
			
			builder.Append(Environment.NewLine);

			// create one zone for each columns in data table, except for x-axis column
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				if (i != xAxisIndex)
				{
					// define a one-dimension zone
					builder.Append("ZONE ").Append("T=\"").Append(dataTable.Columns[i].ColumnName).Append("\" F=POINT");

					builder.Append(Environment.NewLine);

					// output X and Y data for the given column
					foreach (DataRow row in dataTable.Rows)
					{
						// output x axis first
						builder.Append(row[xAxisIndex].ToString()).Append(" ");
						
						builder.Append(row[i].ToString());

						builder.Append(Environment.NewLine);
					}
				}
			}
		}

		/// <summary>
		/// Convert to a single zone format
		/// </summary>
		/// <param name="dataTable">DataTable instnace</param>
		/// <param name="builder">The stringBuilder</param>
		/// <param name="xAxisIndex">x axis index</param>
		/// <param name="yAxisIndex">y axis index</param>
		/// <param name="iNumber">The number of points in 1st dimension </param>
		/// <param name="jNumber">The number of points in 2nd dimension.</param>
		private void FormatTwoDimension(DataTable dataTable, StringBuilder builder,
			int xAxisIndex, int yAxisIndex, int iNumber, int jNumber)
		{
			// output column names as variable
			builder.Append("VARIABLES=");
			// place the x axis first, and y axis second
			builder.Append("\"").Append(dataTable.Columns[xAxisIndex].ColumnName).Append("\" ");
			builder.Append("\"").Append(dataTable.Columns[yAxisIndex].ColumnName).Append("\" ");
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				// rest of variables
				if (i != xAxisIndex && i != yAxisIndex)
				{
					builder.Append("\"").Append(dataTable.Columns[i].ColumnName).Append("\" ");
				}
			}

			builder.Append(Environment.NewLine);

			// define a one-dimension zone
			builder.Append("ZONE ").Append("I=").Append(iNumber).Append(", J=").Append(jNumber).Append(", F=POINT");

			builder.Append(Environment.NewLine);

			// output array data
			foreach (DataRow row in dataTable.Rows)
			{
				// output x axis first and y axis second
				builder.Append(row[xAxisIndex].ToString()).Append(" ");
				builder.Append(row[yAxisIndex].ToString()).Append(" ");
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					if (i !=  xAxisIndex && i != yAxisIndex)
					{
						builder.Append(row[i].ToString()).Append(" ");
					}
				}

				builder.Append(Environment.NewLine);
			}
		}
	}

	/// <summary>
	/// Display types for TecPlot
	/// </summary>
	public enum TecPlotDisplayType
	{
		SignleZone,
		MultipleZone,
		TwoDimension
	}
}