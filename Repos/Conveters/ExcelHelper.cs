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
        /// ��ʼ��,���򿪹������͹�����
        /// </summary>
        public ExcelHelper()
        {

        }

        /// <summary>
        /// ��ȡ���ݼ�
        /// </summary>
        /// <param name="fileName">�ļ�·��</param>
        /// <param name="SheetName">Sheet����</param>
        /// <returns>���ݼ�</returns>
        public DataSet GetDataSet(string fileName, string SheetName)
        {
            //���������ַ���
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

            //ͨ��ǰ�洴�������ַ��������Ӷ���
            OleDbConnection conn = new OleDbConnection(strConn);
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = new DataSet();
            //��ѯ����
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
                //�ر�����
                conn.Close();
                conn.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// �õ��������ж�����
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <returns></returns>
        public int GetWorkSheetRowsCount(DataSet ds)
        {
            return ds.Tables[0].Rows.Count;
        }

        /// <summary>
        /// �õ��������ж�����
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <returns></returns>
        public int GetWorkSheetColumnsCount(DataSet ds)
        {
            return ds.Tables[0].Columns.Count;
        }

        /// <summary>
        /// ȡ�ù��������ļ���
        /// </summary>
        /// <param name="fileName">�ļ�·��</param>
        /// <param name="sheetName">Sheet����</param>
        /// <returns></returns>
        public List<string> GetSheetNames(string fileName)
        {
            DataSet dataSet = null;
            DataTable dt = null;
            OleDbConnection objConn = null;
            string[] excelSheets = null;
            List<string> list = new List<string>();

            //���������ַ���
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
                //ͨ��ǰ�洴�������ַ��������Ӷ���
                objConn = new OleDbConnection(connectionStr);

                //������
                objConn.Open();

                //��ȡ���ݱ�
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
                //�ر�����
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
        /// ��ѯ����
        /// result in a data set
        /// </summary>
        /// <param name="sheetName">Sheet����</param>
        /// <param name="dataSet">���ݼ�</param>
        /// <param name="conn">�����ַ���</param>
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
        /// �����кź��к�ȡֵ
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="rows">��</param>
        /// <param name="cells">��</param>
        /// <returns>������</returns>
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
        /// ȡ����Χ�ڵĵ�Ԫ��ֵ
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="rows">��</param>
        /// <param name="cells">��ʼ��</param>
        /// <param name="endCells">������</param>
        /// <returns>�ַ����б�</returns>
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
        /// ȡ����Χ���е�ֵ
        /// </summary>
        /// <param name="ds">���ݼ�</param>
        /// <param name="rows">��</param>
        /// <param name="cells">��ʼ��</param>
        /// <param name="endCells">������</param>
        /// <returns>�ַ���</returns>
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