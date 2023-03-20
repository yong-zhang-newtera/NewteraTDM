/*
* @(#)HandleChartUtil.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
    using System.Data;
    using System.Text;
    using System.IO;
	using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    using Newtera.DataGridActiveX.Export;
    using Newtera.DataGridActiveX.ChartModel;
    using Newtera.DataGridActiveX.ActiveXControlWebService;

	/// <summary>
	/// A class represents some chart related handling methods for sharing
	/// </summary>
	/// <version>  	1.0.0 27 Feb 2008</version>
    public class HandleChartUtil
	{
		/// <summary>
		/// Fill the ChartDef object with the selected data series
		/// </summary>
        internal static void FillDataSeries(ChartDef chartDef, IDataGridControl dataGridControl)
		{
            if (chartDef is LineChartDef)
			{
                FillLineChartDataSeries(chartDef, dataGridControl);
			}
			else if (chartDef is ContourChartDef)
			{
                FillContourChartDataSeries(chartDef, dataGridControl);
			}
		}

        /// <summary>
        /// Clear the data series in the ChartDef object
        /// </summary>
        /// <param name="chartDef"></param>
        internal static void ClearDataSeries(ChartDef chartDef)
        {
            if (chartDef is LineChartDef)
            {
                ClearLineChartDataSeries(chartDef);
            }
            else if (chartDef is ContourChartDef)
            {
                ClearContourChartDataSeries(chartDef);
            }
        }

        internal static ChartInfoCollection GetChartTemplates(ActiveXControlService service, string connectionString, string className)
        {
            string xml = service.GetChartTemplates(connectionString, className);

            ChartInfoCollection chartTemplates = new ChartInfoCollection();
            StringReader reader = new StringReader(xml);
            chartTemplates.Read(reader);

            return chartTemplates;
        }

        internal static ChartFormatCollection GetChartFormats(ActiveXControlService service)
        {
            string xml = service.GetChartFormatsInXml();

            ChartFormatCollection chartFormats = new ChartFormatCollection();
            StringReader reader = new StringReader(xml);
            chartFormats.Read(reader);

            return chartFormats;
        }

        internal static ChartDef GetChartDef(ActiveXControlService service, ChartInfo chartInfo)
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            string xml = service.GetTemplateDefXmlById(chartInfo.ID);

            ChartDef chartDef = ChartDef.ConvertToChartDef(chartInfo.Type, xml);

            return chartDef;
        }

        /// <summary>
        /// Convert a enum chart type to string
        /// </summary>
        /// <param name="chartDef"></param>
        /// <returns></returns>
        internal static string GetChartTypeStr(ChartType chartType)
        {
            string chartTypeStr = null;

            switch (chartType)
            {
                case ChartType.Line:
                    chartTypeStr = Enum.GetName(typeof(ChartType), ChartType.Line);
                    break;

                case ChartType.Bar:
                    chartTypeStr = Enum.GetName(typeof(ChartType), ChartType.Bar);
                    break;

                case ChartType.Contour:
                    chartTypeStr = Enum.GetName(typeof(ChartType), ChartType.Contour);
                    break;
            }

            return chartTypeStr;
        }

        /// <summary>
        /// Fill the data series information for the line chart
        /// </summary>
        private static void FillLineChartDataSeries(ChartDef chartDef, IDataGridControl dataGridControl)
        {
            DataView dataView = dataGridControl.DataView;
            DataPoint dataPoint;
            if (dataView != null)
            {
                if (chartDef.Orientation == DataSeriesOrientation.ByColumn)
                {
                    // data series oriented by column
                    foreach (LineDef lineDef in ((LineChartDef)chartDef).Lines)
                    {
                        // only add data series for newly added line
                        if (lineDef.IsNew)
                        {
                            lineDef.DataPoints.Clear();
                            int row = 0;
                            foreach (DataRowView dataRowView in dataView)
                            {
                                if (chartDef.UseSelectedRows)
                                {
                                    if (dataGridControl.TheDataGrid.IsSelected(row))
                                    {
                                        dataPoint = new DataPoint();
                                        if (lineDef.XAxis.SeriesName != null)
                                        {
                                            if (dataRowView[lineDef.XAxis.SeriesName] == null)
                                            {
                                                string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataSeriesNotExists"), lineDef.XAxis.SeriesName);
                                                throw new Exception(msg);
                                            }

                                            // when x axis is auto-generated, there is no data series name
                                            dataPoint.X = dataRowView[lineDef.XAxis.SeriesName].ToString();
                                        }

                                        if (dataRowView[lineDef.YAxis.SeriesName] == null)
                                        {
                                            string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataSeriesNotExists"), lineDef.YAxis.SeriesName);
                                            throw new Exception(msg);
                                        }
                                        dataPoint.Y = dataRowView[lineDef.YAxis.SeriesName].ToString();
                                        lineDef.DataPoints.Add(dataPoint);
                                    }
                                }
                                else
                                {
                                    dataPoint = new DataPoint();
                                    if (lineDef.XAxis.SeriesName != null)
                                    {
                                        if (dataRowView[lineDef.XAxis.SeriesName] == null)
                                        {
                                            string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataSeriesNotExists"), lineDef.XAxis.SeriesName);
                                            throw new Exception(msg);
                                        }

                                        // when x axis is auto-generated, there is no data series name
                                        dataPoint.X = dataRowView[lineDef.XAxis.SeriesName].ToString();
                                    }

                                    if (dataRowView[lineDef.YAxis.SeriesName] == null)
                                    {
                                        string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataSeriesNotExists"), lineDef.YAxis.SeriesName);
                                        throw new Exception(msg);
                                    }
                                    dataPoint.Y = dataRowView[lineDef.YAxis.SeriesName].ToString();
                                    lineDef.DataPoints.Add(dataPoint);
                                }

                                row++;
                            }
                        }
                    }
                }
                else
                {
                    // data series oriented by rows
                    foreach (LineDef lineDef in ((LineChartDef)chartDef).Lines)
                    {
                        // only add data series for newly added line
                        if (lineDef.IsNew)
                        {
                            lineDef.DataPoints.Clear();
                            int xRow = -1;
                            int yRow = 0;
                            if (lineDef.XAxis.SeriesName != null)
                            {
                                // lineDef.XAxis.SeriesName represent a row number
                                xRow = Int32.Parse(lineDef.XAxis.SeriesName);
                            }
                            // lineDef.YAxis.SeriesName represent a row number too
                            yRow = Int32.Parse(lineDef.YAxis.SeriesName);

                            foreach (ColumnInfo columnInfo in dataGridControl.ColumnInfos)
                            {
                                if (columnInfo.IsChecked)
                                {
                                    dataPoint = new DataPoint();
                                    if (xRow >= 0)
                                    {
                                        if (dataView[xRow] == null)
                                        {
                                            string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataRowNotExists"), xRow);
                                            throw new Exception(msg);
                                        }
                                        else if (dataView[xRow][columnInfo.Name] == null)
                                        {
                                            string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataColumnNotExists"), columnInfo.Name);
                                            throw new Exception(msg);
                                        }
                                        // when x axis is auto-generated, xRow is -1
                                        dataPoint.X = dataView[xRow][columnInfo.Name].ToString();
                                    }

                                    if (dataView[yRow] == null)
                                    {
                                        string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataRowNotExists"), yRow);
                                        throw new Exception(msg);
                                    }
                                    else if (dataView[yRow][columnInfo.Name] == null)
                                    {
                                        string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataColumnNotExists"), columnInfo.Name);
                                        throw new Exception(msg);
                                    }
                                    dataPoint.Y = dataView[yRow][columnInfo.Name].ToString();
                                    lineDef.DataPoints.Add(dataPoint);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill the data series information for the contour chart
        /// </summary>
        private static void FillContourChartDataSeries(ChartDef chartDef, IDataGridControl dataGridControl)
        {
            DataView dataView = dataGridControl.DataView;
            ContourChartDef contour = (ContourChartDef)chartDef;
            if (dataView != null)
            {
                int startRow = 0;
                int rowCount = dataView.Table.Rows.Count;
                int startCol = 0;
                int colCount = dataGridControl.TheDataGrid.TableStyles[0].GridColumnStyles.Count;
                if (contour.UseSelectedRows && dataGridControl.TheDataGrid is SelectionDataGrid)
                {
                    SelectionDataGrid selectionDataGrid = (SelectionDataGrid)dataGridControl.TheDataGrid;
                    startRow = selectionDataGrid.SelectedRange.Top;
                    rowCount = selectionDataGrid.SelectedRange.Bottom - selectionDataGrid.SelectedRange.Top + 1;
                    // get the selected column range
                    startCol = selectionDataGrid.SelectedRange.Left;
                    colCount = selectionDataGrid.SelectedRange.Right - selectionDataGrid.SelectedRange.Left + 1;
                }

                DataGridTableStyle tableStyle = dataGridControl.TheDataGrid.TableStyles[0];
                DataGridColumnStyle columnStyle;
                contour.ZDataSeries.DataValues.Clear();
                if (contour.Orientation == DataSeriesOrientation.ByColumn)
                {
                    contour.XPoints = rowCount;
                    contour.YPoints = colCount;
                    // each column represents a Z data series
                    for (int col = startCol; col < colCount; col++)
                    {
                        columnStyle = tableStyle.GridColumnStyles[col];
                        for (int row = startRow; row < rowCount; row++)
                        {
                            if (dataView[row] == null)
                            {
                                string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataRowNotExists"), row);
                                throw new Exception(msg);
                            }
                            else if (dataView[row][columnStyle.MappingName] == null)
                            {
                                string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataColumnNotExists"), columnStyle.MappingName);
                                throw new Exception(msg);
                            }
                            contour.ZDataSeries.DataValues.Add(dataView[row][columnStyle.MappingName].ToString());
                        }
                    }
                }
                else
                {
                    contour.XPoints = colCount;
                    contour.YPoints = rowCount;
                    // each row represents a Z data series
                    for (int row = startRow; row < rowCount; row++)
                    {
                        for (int col = startCol; col < colCount; col++)
                        {
                            columnStyle = tableStyle.GridColumnStyles[col];
                            if (dataView[row] == null)
                            {
                                string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataRowNotExists"), row);
                                throw new Exception(msg);
                            }
                            else if (dataView[row][columnStyle.MappingName] == null)
                            {
                                string msg = string.Format(MessageResourceManager.GetString("DataGrid.DataColumnNotExists"), columnStyle.MappingName);
                                throw new Exception(msg);
                            }
                            contour.ZDataSeries.DataValues.Add(dataView[row][columnStyle.MappingName].ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear the data series information for the line chart
        /// </summary>
        private static void ClearLineChartDataSeries(ChartDef chartDef)
        {
            foreach (LineDef lineDef in ((LineChartDef)chartDef).Lines)
            {
                lineDef.DataPoints.Clear();
            }
        }

        /// <summary>
        /// Clear the data series information for the contour chart
        /// </summary>
        private static void ClearContourChartDataSeries(ChartDef chartDef)
        {
            ContourChartDef contour = (ContourChartDef)chartDef;
            contour.XPoints = 0;
            contour.YPoints = 0;
            contour.ZDataSeries.DataValues.Clear();
        }
	}

    /// <summary>
    /// Event type
    /// </summary>
    internal enum EventType
    {
        Unknown,
        ChartEvent,
        DownloadEvent
    }
}