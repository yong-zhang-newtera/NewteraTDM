using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Newtera.Conveters
{
    class ExcelHelper
    {
        /// <summary>
        /// 初始化,并打开工作薄和工作表
        /// </summary>
        public ExcelHelper()
        {

        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="SheetName">Sheet名称</param>
        /// <returns>数据集</returns>
        public DataSet GetDataSet(string fileName, string SheetName)
        {
            //创建连接字符串
            string strConn = strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=  \"Excel 12.0;HDR=NO;IMEX=1\"";

            /*
            if (Path.GetExtension(fileName).Trim() == ".xls")
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=  \"Excel 8.0;HDR=NO;IMEX=1\"";
            }
            else
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=  \"Excel 12.0;HDR=NO;IMEX=1\"";
            }
             */

            //通过前面创建连接字符串来连接对象
            OleDbConnection conn = new OleDbConnection(strConn);
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = new DataSet();
            //查询条件
            strExcel = "select * from [" + SheetName + "]";
            try
            {
                conn.Open();
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                myCommand.Fill(ds, "dtSource");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭链接
                conn.Close();
                conn.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// 得到工作表有多少行
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public int GetWorkSheetRowsCount(DataSet ds)
        {
            return ds.Tables[0].Rows.Count;
        }

        /// <summary>
        /// 得到工作表有多少列
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public int GetWorkSheetColumnsCount(DataSet ds)
        {
            return ds.Tables[0].Columns.Count;
        }

        /// <summary>
        /// 取得工作表名的集合
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="sheetName">Sheet名称</param>
        /// <returns></returns>
        public List<string> GetSheetNames(string fileName)
        {
            DataSet dataSet = null;
            DataTable dt = null;
            OleDbConnection objConn = null;
            string[] excelSheets = null;
            List<string> list = new List<string>();

            //创建连接字符串
            string connectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=  \"Excel 12.0;\"";

            /*
            if (Path.GetExtension(fileName).Trim() == ".xls")
            {
                connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=  \"Excel 8.0\"";
            }
            else
            {
                connectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=  \"Excel 12.0;\"";
            }
             */

            try
            {
                //通过前面创建连接字符串来连接对象
                objConn = new OleDbConnection(connectionStr);

                //打开连接
                objConn.Open();

                //获取数据表
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt != null)
                {
                    dataSet = new DataSet("Excel Data");
                    excelSheets = new string[dt.Rows.Count];

                }
                int i = 0;

                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                for (int j = 0; j < excelSheets.Length; j++)
                {
                    if (excelSheets[j].Contains("$"))
                        list.Add(excelSheets[j]);
                }
            }
            finally
            {
                //关闭链接
                if (objConn != null)
                {
                    objConn.Close();
                    objConn.Dispose();
                }

                if (dt != null)
                {
                    dt.Dispose();
                }
            }
            return list;
        }

        /// <summary>
        /// 查询数据
        /// result in a data set
        /// </summary>
        /// <param name="sheetName">Sheet名称</param>
        /// <param name="dataSet">数据集</param>
        /// <param name="conn">链接字符串</param>
        private void GetSheetData(string sheetName, DataSet dataSet, OleDbConnection conn)
        {
            string stmt;
            sheetName = sheetName.Trim('\'');
            stmt = "SELECT * FROM [" + sheetName + "]";

            OleDbDataAdapter cmd = new OleDbDataAdapter(stmt, conn);

            try
            {
                cmd.Fill(dataSet, sheetName);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 根据行号和列号取值
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="rows">行</param>
        /// <param name="cells">列</param>
        /// <returns>基类型</returns>
        public object GetCellValue(DataSet ds, int rows, int cells)
        {
            object obj = null;
            try
            {
                obj = ds.Tables[0].Rows[rows][cells];
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return obj;
        }

        /// <summary>
        /// 取出范围内的单元格值
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="rows">行</param>
        /// <param name="cells">起始列</param>
        /// <param name="endCells">结束列</param>
        /// <returns>字符串列表</returns>
        public string[] GetCellRangeValue(DataSet ds, int rows, int cells, int endCells)
        {
            string[] obj = new string[endCells - cells];
            for (int i = 0; i < endCells - cells; i++)
            {
                obj[i] = ds.Tables[0].Rows[rows][cells + i].ToString();
            }
            return obj;
        }


        /// <summary>
        /// 取出范围内行的值
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="rows">行</param>
        /// <param name="cells">起始列</param>
        /// <param name="endCells">结束列</param>
        /// <returns>字符串</returns>
        public string GetCellRangeStr(DataSet ds, int rows, int cells, int endCells)
        {
            string obj = "";
            for (int i = 0; i < endCells - cells; i++)
            {
                obj += ds.Tables[0].Rows[rows][cells + i].ToString() + ";";
            }
            return obj;
        }
    }
}