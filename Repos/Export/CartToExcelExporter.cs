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
    using org.in2bits.MyXls;

	/// <summary>
	/// Export data in DataTable into an Excel file
	/// </summary>
	/// <version> 1.0.0 01 June 2006</version>
    public class CartToExcelExporter : IExporter 
	{
        private string _filePath;
        private string _tableName;

        org.in2bits.MyXls.XlsDocument doc;

		/// <summary>
		/// Initialize the exporter
		/// </summary>
        public CartToExcelExporter()
		{
            
		}

        /// <summary>
        /// Called at the beginning of the exporting data, allow the exporter to
        /// perform the initialization necessay for exporting process, such as
        /// open the file to write data.
        /// </summary>
        public void BeginExport(string filePath)
        {
            doc = new XlsDocument();
            _filePath = filePath;
            _tableName = "";
        }

        /// <summary>
		/// Export the data in the TableTable to a file.
		/// </summary>
		/// <param name="dataTable">The DataTable contains data rows for exporting</param>
        public void ExportData(DataTable dataTable)
		{
            if (!String.IsNullOrEmpty(_filePath) && dataTable.TableName != _tableName)
               CreateExcel(_filePath, dataTable);

		}

        /// <summary>
        /// Export Excel by fan
        /// </summary>
        public  void CreateExcel(string fileName, DataTable dt)
        {
            doc.FileName =  fileName;

            #region 样式
            org.in2bits.MyXls.XF xf = getbold(doc); //表头样式
            ArrayList xfs = new ArrayList();
            xfs.Add(getred(doc));
            xfs.Add(getnormal(doc));
            #endregion

            try
            {
                #region 读取当前表
                string sheetName = dt.TableName;
                #endregion

                #region 颜色信息
                org.in2bits.MyXls.XF cf = getnormal(doc);
                #endregion

                #region 生成工作薄
                org.in2bits.MyXls.Worksheet sheets = doc.Workbook.Worksheets.Add(sheetName);
                org.in2bits.MyXls.Cells cell = sheets.Cells;

                org.in2bits.MyXls.ColumnInfo cif = new org.in2bits.MyXls.ColumnInfo(doc, sheets);
                cif.ColumnIndexStart = 4;
                cif.ColumnIndexEnd = 6;
                cif.Width = 15 * 400;
                sheets.AddColumnInfo(cif);
                #endregion

                #region 添加表头
                for (int i = 0; i < dt.Columns.Count; i++)
                    cell.Add(1, i + 1, dt.Columns[i].ColumnName.ToString(), xf);
                #endregion

                #region 生成内容
                for (int i = 0; i < dt.Rows.Count; i++)
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string typename = dt.Columns[j].DataType.Name;
                        if (dt.Rows[i][j] != null)
                        {
                            string value = dt.Rows[i][j].ToString();
                            AddCell(cell, (i + 2), (j + 1), typename, value, cf);
                        }
                    }
                #endregion

            }
            catch
            {

            }
            doc.SummaryInformation.Comments = "";
            doc.SummaryInformation.NameOfCreatingApplication = "";
            //通过HTTP直接发送到客户端
            //doc.Send();
            //直接存在服务器端网站目录里
            //doc.Save(_filePath,true);
           
        }

        #region 样式

        private static org.in2bits.MyXls.Color defBorderColor = Colors.Default16;
        private static org.in2bits.MyXls.XF getyellow(org.in2bits.MyXls.XlsDocument doc)
        {
            org.in2bits.MyXls.XF yellow = doc.NewXF();
            yellow.HorizontalAlignment = HorizontalAlignments.Left;
            yellow.VerticalAlignment = VerticalAlignments.Centered;
            yellow.Pattern = 2;
            yellow.PatternColor = Colors.Yellow;
            yellow.PatternBackgroundColor = Colors.White;
            yellow.UseBorder = true;
            yellow.TopLineStyle = 1;
            yellow.TopLineColor = defBorderColor;
            yellow.BottomLineStyle = 1;
            yellow.BottomLineColor = defBorderColor;
            yellow.LeftLineStyle = 1;
            yellow.LeftLineColor = defBorderColor;
            yellow.RightLineStyle = 1;
            yellow.RightLineColor = defBorderColor;
            yellow.Font.Bold = false;

            return yellow;
        }

        private static org.in2bits.MyXls.XF getyellowbold(org.in2bits.MyXls.XlsDocument doc)
        {
            org.in2bits.MyXls.XF xf = doc.NewXF();
            xf.Pattern = 2;
            xf.PatternColor = Colors.Yellow;
            xf.UseBorder = true;
            xf.TopLineStyle = 1;
            xf.TopLineColor = defBorderColor;
            xf.BottomLineStyle = 1;
            xf.BottomLineColor = defBorderColor;
            xf.LeftLineStyle = 1;
            xf.LeftLineColor = defBorderColor;
            xf.RightLineStyle = 1;
            xf.RightLineColor = defBorderColor;
            xf.Font.Bold = true;
            return xf;
        }

        private static org.in2bits.MyXls.XF getyellowhead(org.in2bits.MyXls.XlsDocument doc)
        {
            org.in2bits.MyXls.XF xf = doc.NewXF();
            xf.Pattern = 2;
            xf.PatternColor = Colors.Yellow;
            xf.UseBorder = true;
            xf.TopLineStyle = 1;
            xf.TopLineColor = defBorderColor;
            xf.BottomLineStyle = 1;
            xf.BottomLineColor = defBorderColor;
            xf.LeftLineStyle = 1;
            xf.LeftLineColor = defBorderColor;
            xf.RightLineStyle = 1;
            xf.RightLineColor = defBorderColor;
            xf.Font.Bold = true;
            xf.HorizontalAlignment = HorizontalAlignments.Centered;
            return xf;
        }

        private static org.in2bits.MyXls.XF getred(org.in2bits.MyXls.XlsDocument doc)
        {
            org.in2bits.MyXls.XF red = doc.NewXF();
            red.Pattern = 1;
            red.PatternColor = Colors.Red;
            red.UseBorder = true;
            red.TopLineStyle = 1;
            red.TopLineColor = defBorderColor;
            red.BottomLineStyle = 1;
            red.BottomLineColor = defBorderColor;
            red.LeftLineStyle = 1;
            red.LeftLineColor = defBorderColor;
            red.RightLineStyle = 1;
            red.RightLineColor = defBorderColor;
            red.Font.Bold = false;

            return red;
        }

        private static org.in2bits.MyXls.XF getbold(org.in2bits.MyXls.XlsDocument doc)
        {
            org.in2bits.MyXls.XF xf = doc.NewXF();
            xf.Font.Bold = true;
            return xf;
        }

        private static org.in2bits.MyXls.XF getnormal(org.in2bits.MyXls.XlsDocument doc)
        {
            org.in2bits.MyXls.XF xf = doc.NewXF();
            return xf;
        }

        #endregion

        #region 生成内容
        private static void AddCell(org.in2bits.MyXls.Cells cell, int row, int col, string typename, string value, org.in2bits.MyXls.XF cf)
        {
            if (value != null || value.Trim() != "")
                try
                {
                    switch (typename)
                    {
                        case "Int32":
                            int a = Convert.ToInt32(value);
                            if (a != 0)
                                cell.Add(row, col, a, cf);
                            else
                                cell.Add(row, col, null, cf);
                            break;
                        case "Double":
                            cell.Add(row, col, Convert.ToDouble(value), cf);
                            break;
                        case "float":
                            cell.Add(row, col, Convert.ToSingle(value), cf);
                            break;
                        case "Decimal":
                            cell.Add(row, col, Convert.ToDouble(value), cf);
                            break;
                        case "DateTime":
                            cell.Add(row, col, Convert.ToDateTime(value).ToLongDateString(), cf);
                            break;
                        default:
                            cell.Add(row, col, value, cf);
                            break;
                    }
                }
                catch
                {
                    cell.Add(row, col, null, cf);
                }
            else
                cell.Add(row, col, null, cf);
        }
        #endregion

        /// <summary>
        /// Called at the end of the exporting data, allow the exporter to
        /// free up the resources used by the exporter, such as closing the file
        /// </summary>
        public void EndExport()
        {
            doc.Save(_filePath, true);

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